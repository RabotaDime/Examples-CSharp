using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using My.StateMachine;



namespace VectorClock
{
    //   Простая машина состояний для управления тремя векторами одновременно 
    //   с использованием трех кнопок мыши. Используется для примера кода,
    //   а не потому, что это самое короткое или лучшее решение.  
    public class СMouseStateMachine : CMachine
    {
        public class CStates : IEnumerable<IState>
        {
            private List<IState> OrderedSet; 

            public CStates (params IState[] all_states)
            {
                OrderedSet = new List<IState> (all_states);
            }

            public IState this [int index]
            {
                get { return OrderedSet[index]; }
            }

            public static IEnumerable<IState> GetStates (CStates obj)
            {
                foreach (IState s in obj.OrderedSet)
                    yield return s;
            }

            public IEnumerator<IState> GetEnumerator ()
            {
                return GetStates(this).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator ()
            {
                return GetStates(this).GetEnumerator();
            }
        }

        public class CTransitions
        {
            public CTransition FirstCapture;
            public CTransition SecondCapture;
            public CTransition ThirdCapture;
        }

        public CStates AllStates; 

        public СMouseStateMachine (CStates states) : base (states.ToArray())
        {
            AllStates = states;
        }



        public void PrintDebugInfo (ListBox output)
        {
            int n, index = -1;

            output.BeginUpdate();
            output.Items.Clear();
            {
                n = 0;
                foreach (IState s in this.States)
                {
                    output.Items.Add
                    (
                        (s == this.State ? " --> " : "     ") +
                        " [" + s + "]"
                    );

                    if (s == this.State) index = n;
                    n++;
                }
            }
            output.SelectedIndex = index;
            output.EndUpdate();
        }
    }



    public delegate void FMouseCaptureEnterState (CMouseCaptureState this_state, CMouseCaptureState prev_state); 
    public delegate void FMouseCaptureLeaveState (CMouseCaptureState this_state, CMouseCaptureState next_state); 



    public class CMouseCaptureState : IState
    {
        private string CustomID;

        public MouseButtons CapturedButton;

        public event FMouseCaptureEnterState OnEnter;
        public event FMouseCaptureLeaveState OnLeave;

        public CMouseCaptureState (string id)
        {
            CustomID = id;
        }

        public void Enter (out bool return_success, IState prev_state, object data)
        {
            {
                if (OnEnter != null) OnEnter(this, prev_state as CMouseCaptureState);
            }
            return_success = true;
        }

        public void Leave (out bool return_success, IState next_state, object data)
        {
            {
                if (OnLeave != null) OnLeave(this, next_state as CMouseCaptureState);
            }
            return_success = true;
        }

        public override string ToString ()
        {
            return $"STATE[{this.CustomID} for {this.CapturedButton}]";
        }
    }
}
