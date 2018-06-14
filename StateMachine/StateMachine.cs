using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace My.StateMachine
{
    public delegate void FEnterState (out bool return_success, object data);
    public delegate void FLeaveState (out bool return_success, object data);



    public enum TransitionResult
    {
        Undefined   = 0,

        Successful  = 1,    //   Успешный переход.
        StateReject = 2,    //   Отказ состояния при входе / выходе.
        WrongRoute  = 3,    //   Недопустимый переход. Нет такого пути. 
    }

    public enum TransitionDirection
    {
        Undefined   = 0,
        MaximumValue = Any,

        Forward     = 0x01,    //   От А до Б.
        Backward    = 0x10,    //   От Б до А.

        Any         = Forward | Backward,  //   В обоих направлениях. 
    }

    public static class CTransitionDirection
    {
        public static readonly Dictionary<TransitionDirection, TransitionDirection> Inverter =
            new Dictionary<TransitionDirection, TransitionDirection>
            {
                [TransitionDirection.Undefined] = TransitionDirection.Undefined,
                [TransitionDirection.Forward]   = TransitionDirection.Backward,
                [TransitionDirection.Backward]  = TransitionDirection.Forward,
                [TransitionDirection.Any]       = TransitionDirection.Any,
            };

        //  Пример расширения № 1. 
        public static TransitionDirection Invert (this TransitionDirection dir)
        {
            if (dir > TransitionDirection.MaximumValue)
                return TransitionDirection.Undefined;
            else
                return Inverter[dir];
        }

        //  Пример расширения № 2. 
        public static TransitionDirection Validate (this TransitionDirection dir)
        {
            if (dir > TransitionDirection.MaximumValue)
                return TransitionDirection.Undefined;
            else
                return dir;
        }
    }



    public class CMachine
    {
        private IState _CurrentState = null;



        private Dictionary<IState, HashSet<IState>> _RoutingTable;
        private HashSet<IState> _AllStates;



        //public int Nothing;



        public CMachine (params IState[] start_and_others)
        {
            if (start_and_others.Length < 1)
            {
                throw new ArgumentNullException("Отсутствует стартовое состояние. Машина не может работать.");
            }

            foreach (IState s in start_and_others)
            {
                if (s == null)
                    throw new ArgumentNullException("Ошибочное состояние.");
            }


            _RoutingTable = new Dictionary<IState, HashSet<IState>> ();
            _AllStates = new HashSet<IState> (start_and_others);


            //_CurrentState = null;

            for (int i = 0; i < start_and_others.Length; i++)
            {
                RegisterState(start_and_others[i]);
            }


            _CurrentState = start_and_others[0];
            _CurrentState.Enter(out bool enter_successful, null);

            if (! enter_successful)
            {
                throw new InvalidOperationException($"{nameof(CMachine)}, constructor, default state reject entering");
            }
        }

        public CMachine (IState start) : this (new IState[] { start })
        {
        }



        public void RegisterState (IState s)
        {
            _AllStates.Add(s);
        }

        public void ForgetState (IState s, bool drop_transitions = true)
        {
            _AllStates.Remove(s);

            throw new NotImplementedException("TODO: drop_transitions");
        }



        public CTransition AttachTransition
        (
            //  Связываемые состояния машины. Null указывает на ситуацию *. 
            //  Допустимо использовать Null в обоих аргументах, чтобы разрешить
            //  все переходы между каждым состоянием. 
            IState a, IState b,
            
            //  Направление связывания. 
            TransitionDirection dir = TransitionDirection.Forward
        )
        {
            //  -------------------------------------------------------------------------
            //    Анализ ситуаций: 
            //  -------------------------------------------------------------------------
            //                                        s3
            //                                                 .-- dir -- (>>)   s8   
            //                                   .---( A )----<--- dir -- (<<)   s9   
            //                                  /              '-- dir -- (<>)   s10   
            //                   s2            /      s4
            //                                /                .-- dir -- (>>)   s11
            //               .--( A )--- b --<-------( B )----<--- dir -- (<<)   s12
            //              /                 \                '-- dir -- (<>)   s13
            //             /                   \      s5
            //            /                     \      *       .-- dir -- (>>)   s14
            //    s1     /                       '--( null )--<--- dir -- (<<)   s15
            //          /                                      '-- dir -- (<>)   s16
            //     a --<                                                              
            //          \                             s6     .-- dir -- (>>)  s17   
            //           \      s3             .----( B )---<--- dir -- (<<)  s18   
            //            \      *            /              '-- dir -- (<>)  s19   
            //             '--( null )-- b --<       s7
            //                                \      *       .-- dir -- (>>)  s20   
            //                                 '--( null )--<--- dir -- (<<)  s21   
            //                                               '-- dir -- (<>)  s22   
            //    Определение:
            //       Все узлы графа ситуаций пронумерованы
            //       
            //       a = A      -- 1-ое состояние определено
            //       a = null*  -- 1-ое состояние не определено (равно любому * из множества)
            //
            //       b = A      -- 2-ое состояние определено и совпадает по значению с А
            //       b = B      -- 2-ое состояние определено
            //       b = null*  -- 2-ое состояние не определено (равно любому * из множества)
            //       
            //        >>   -- Направление от 1(а) к 2(b)
            //        <<   -- Направление от 1(b) к 2(a)
            //        <>   -- Оба направления
            //       
            //  -------------------------------------------------------------------------


            switch (dir)
            {
                case TransitionDirection.Forward:
                case TransitionDirection.Backward:
                case TransitionDirection.Any:
                    break;

                default:
                {
                    //  Это не допустимый аргумент. 
                    throw new ArgumentException($"{nameof(GoTo)}:0, {nameof(dir)}");
                }
            }
            

            if (a == b) {
                if (a == null)
                {
                    //  Вершины ситуаций: s20, s21, s22
                    //  Ситуация привязки всех возможных переходов друг с другом. 
                    foreach (IState s in _AllStates)
                    {
                        AttachTransition(s, null, TransitionDirection.Forward);
                    }
                }
                else
                {
                    //  Вершина ситуации: s8 
                    //  Оба состояния не равны Null, но равны по значению.
                    //  Это не допустимый переход. 
                    throw new ArgumentException($"{nameof(GoTo)}:1");
                }
            }


            //  Вершины ситуаций: s1 -> s3 -> s6 -> s17, s18, s19
            //  Меняем местами значения, чтобы только b могло быть Null-звездочкой (обычный swap)
            if (a == null)
            {
                a = b;
                b = null;

                dir = dir.Invert();

                //  Выключенный код. 
                //switch (dir)
                //{
                //    case TransitionDirection.Forward    : dir = TransitionDirection.Backward; break;
                //    case TransitionDirection.Backward   : dir = TransitionDirection.Forward; break;
                //}
            }

           
            //  Для обработки остались только две ситуации.
            if (b == null)
            {
                //  Из A можно перейти в любое состояние. 
                foreach (IState s in _AllStates)
                {
                    if (a != s) AttachTransition(a, s, dir);
                }
            }
            else
            {
                //  Из А можно перейти в B. Оба состояния явно определены 
                //  и не равны друг другу.

                IState state_from, state_to;

                if (dir.HasFlag(TransitionDirection.Forward))
                {
                    state_from  = a;
                    state_to    = b;

                    goto inline_MakeTrans;
                }
                else
                {
                    goto NextCheck;
                }

                inline_MakeTrans:

                //  Выключенный код. 
                //  Другой способ сделать то же самое. 
                //  Func<IState, IState, bool> inline_MakeTrans = delegate (IState state_a, IState state_b)
                {
                    if (! _RoutingTable.ContainsKey(state_from))
                    {
                        _RoutingTable.Add(state_from, new HashSet<IState> ());
                    }

                    HashSet<IState> allowed_routes = _RoutingTable[state_from];

                    allowed_routes.Add(state_to);
                };
                
                NextCheck:


                if (dir.HasFlag(TransitionDirection.Backward))
                {
                    state_from  = b;
                    state_to    = a;

                    dir = TransitionDirection.Undefined;

                    goto inline_MakeTrans;
                }

                //  Выключенный код. 
                //if (dir.HasFlag(TransitionDirection.Forward))
                //{
                //    inline_MakeTrans(a, b);
                //}
                
                //  Выключенный код. 
                //if (dir.HasFlag(TransitionDirection.Backward))
                //{
                //    inline_MakeTrans(b, a);
                //}
            }


            //var t = new CInstantTransition (this)
            //{
            //};

            return null;
        }

        public void ReleaseTransition (IState a, IState b, TransitionDirection dir)
        {
        }



        public void GoTo
        (
            //  Новое состояние. 
            IState new_state,

            //  Не допускать вызов перехода в то же самое состояние. 
            bool error_on_equal = true
        )
        {
            //  Ошибка: Переход в нулевое состояние не допустим в моей машине состояний.
            //          Пользователь машины обязан сам создать и стартовое, и состояние 
            //          по умолчанию (например, с именем Idle, Start и т. д.) если у него 
            //          изначально это не предусмотрено в графе. 
            if (new_state == null)
            {
                throw new ArgumentNullException($"{nameof(GoTo)}:1");
            }

            //  Ошибка: Состояние не зарегистрировано в машине.
            if (! _AllStates.Contains(new_state))
            {

            }

            if (_CurrentState == new_state)
            {
                if (error_on_equal)
                    throw new InvalidOperationException($"{nameof(GoTo)}:2");
                else
                    return;
            }


            //  Ошибка: Внутренняя таблица графа переходов ошибочна. 
            if (! _RoutingTable.ContainsKey(_CurrentState))
            {
                throw new InvalidOperationException($"{nameof(GoTo)}:3");
            }


            HashSet<IState> AllowedDestinations = _RoutingTable[_CurrentState];

            //   Если список допустимых переходов не указывает на особую ситуацию (null = *)
            //   И если в указанное состояние нельзя перейти. 
            if ((AllowedDestinations != null) && (! AllowedDestinations.Contains(new_state)))
            {
                throw new InvalidOperationException($"{nameof(GoTo)}:4");
            }


            _CurrentState.Leave(out bool leave_successful, new_state);

            if (! leave_successful)
            {
                //throw new InvalidOperationException($"{nameof(GoTo)}:5, Unable to leave current state");

                return;
            }


            new_state.Enter(out bool enter_successful, _CurrentState);

            if (! enter_successful)
            {
                //throw new InvalidOperationException($"{nameof(GoTo)}:6, Unable to enter new state");

                return;
            }


            _CurrentState = new_state;
        }



        public IState State
        {
            get { return _CurrentState; }
        }

        public IState[] States
        {
            get { return _AllStates.ToArray(); }
        }



        //  TODO: Особое состояние машины, для не моментального перехода из одного
        //        состояния в другое. 
        //private CLinearTransition LinearTransition = null;
        public class CLinearTransition : IState
        {
            public void Enter (out bool return_success, IState prev_state, object data) { return_success = true; }
            public void Leave (out bool return_success, IState next_state, object data) { return_success = true; }
        }



        //  Пока что, Null означает ситуацию «любой» *
        //private static IState _AnyState = new CAnyState ();
        public IState Any { get { return null; } }



        public class EGoToUndefinedState : InvalidOperationException
        {
            public EGoToUndefinedState (IState s) : base
            (
                $"{nameof(s)}"
            )
            {
            }
        }

        public static EGoToUndefinedState GoToUndefinedState (IState s)
        {
            return new EGoToUndefinedState (s);
        }
    }



    public abstract class CTransition
    {
        public CTransition (CMachine base_machine)
        {
        }
    }

    public class CInstantTransition : CTransition
    {
        public CInstantTransition (CMachine base_machine) : base (base_machine)
        {
        }
    }

    public class CLinearTransition : CTransition
    {
        public CLinearTransition (CMachine base_machine) : base (base_machine)
        {
        }
    }



    public interface IState
    {
        void Enter (out bool return_success, IState prev_state, object data = null);
        void Leave (out bool return_success, IState next_state, object data = null);
    }
}

