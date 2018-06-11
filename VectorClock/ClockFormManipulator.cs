using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using My.Utils;
using My.MathUtils;
using My.StateMachine;

using TransDir = My.StateMachine.TransitionDirection;



namespace VectorClock
{
    public partial class ClockForm : Form
    {
        private CManipulator VectorManipulator;



        private class CManipulator
        {
            public Arrows.Arrow             MasterArrow;
            public HashSet<Arrows.Arrow>    SlaveArrows;
            public ClockForm                HostForm;

            public Dictionary<MouseButtons, Arrows.Arrow> ButtonToArrowMap;

            //  Простая машина состояний для визуального редактирования векторов
            //  с помощью всех трех кнопок мыши. Машина в данном случае использована для 
            //  примера, а не как простейшее или самое правильное решение. 
            public СMouseStateMachine Machine;
            
            //  Состояния для машины. 
            public CMouseCaptureState Released;
            public CMouseCaptureState CapturedMaster;
            public CMouseCaptureState CapturedSecond;
            public CMouseCaptureState CapturedThird;



            public CManipulator (ClockForm host)
            {
                HostForm = host;

                MasterArrow = null;
                SlaveArrows = new HashSet<Arrows.Arrow> ();

                ButtonToArrowMap = new Dictionary<MouseButtons, Arrows.Arrow>
                {
                    [MouseButtons.Left]     = HostForm.AllArrows.User1,
                    [MouseButtons.Right]    = HostForm.AllArrows.User2,
                    [MouseButtons.Middle]   = HostForm.AllArrows.User3,
                };


                Released          = new CMouseCaptureState ("Released");
                CapturedMaster    = new CMouseCaptureState ("Captured Master : First button");
                CapturedSecond    = new CMouseCaptureState ("Captured Slave : Second button");
                CapturedThird     = new CMouseCaptureState ("Captured Slave : Third button");

                Released        .OnEnter += DropEverything;

                CapturedMaster  .OnEnter += CaptureMaster;

                CapturedSecond  .OnEnter += CaptureSlave;
                CapturedSecond  .OnLeave += DropSlave;

                CapturedThird   .OnEnter += CaptureSlave;
                CapturedThird   .OnLeave += DropSlave;

                //   Простая машина состояний для управления тремя векторами одновременно 
                //   с использованием трех кнопок мыши. Используется для примера кода,
                //   а не потому, что это самое короткое или лучшее решение.
                //   
                //   Три уровня «захвата» кнопок мыши. 
                //
                //                                           .-------------------.
                //               .------------------------→  |   Зажата первая   |
                //               |               .---[R1]----|  "мастер" кнопка  |
                //               |               |           '-------------------'
                //              [C1]             |                   |    ↑
                //               |               |                 [C2]  [B1]
                //               |               |                   ↓    |
                //     .--------------------.    |           .---------------------.
                //     |  Мышь в свободном  | ←--O---[R2]----|  Две кнопки зажаты  |
                //     |      состоянии     |    |           '---------------------'
                //     '--------------------'    |                   |    ↑
                //                               |                 [C3]  [B2]
                //                               |                   ↓    |
                //                               |           .-------------------------.
                //                               '---[R3]----|  Все три кнопки зажаты  |
                //                                           '-------------------------'
                //                                    
                //   Сокращения: C -- захват, B -- возврат, R -- сброс                                  
                //   
                //   Анализ состояния [CapturedMaster] 
                //                                    
                //     Точки входа:                               
                //     [C1]  ->  CapturedMaster.Enter  ->  делегат CaptureMaster из [C1]                              
                //     [B1]  ->  CapturedMaster.Enter  ->  делегат CaptureMaster из [B1]                               
                //                                    
                //   
                Machine = new СMouseStateMachine
                (
                    new СMouseStateMachine.CStates
                    (
                        Released,
                        CapturedMaster,
                        CapturedSecond,
                        CapturedThird
                    )
                );

                Machine.AttachTransition ( CapturedMaster,   CapturedSecond,     TransDir.Any );
                Machine.AttachTransition ( CapturedSecond,  CapturedThird,      TransDir.Any );
                Machine.AttachTransition ( Released,        CapturedMaster,      TransDir.Forward );
                Machine.AttachTransition ( Machine.Any,     Released,           TransDir.Forward );
            }

            public Arrows.Arrow ButtonToArrow (MouseButtons button)
            {
                if (ButtonToArrowMap.ContainsKey(button))
                    return ButtonToArrowMap[button];
                else
                    return null;
            }



            //  Ответные события для машины состояний 
            //---------------------------------------------------------------------------

            public void CaptureMaster (CMouseCaptureState this_state, CMouseCaptureState prev_state)
            {
                //   Захватываем основную "ведущую" стрелку. 
                //   Нужно только в ситуации если вход в это состояние произошел 
                //   из ситуации, когда ни одна кнопка мыши не была захвачена. 
                if (prev_state == Released)
                {
                    HostForm.listBox1.Items.Add("CaptureMaster : " + this_state);

                    Arrows.Arrow arrow = ButtonToArrow(this_state.CapturedButton);
                    MasterArrow = arrow;
                    SlaveArrows.Clear();
                }
            }

            public void CaptureSlave (CMouseCaptureState this_state, CMouseCaptureState prev_state)
            {
                if
                (
                    //   Переход машины состояний [C1]  
                    ((this_state == CapturedSecond) && (prev_state == CapturedMaster)) ||
                    //   Или переход машины состояний [C2]  
                    ((this_state == CapturedThird) && (prev_state == CapturedSecond))
                )
                {
                    HostForm.listBox1.Items.Add("CaptureSlave : " + this_state);

                    Arrows.Arrow arrow = ButtonToArrow(this_state.CapturedButton);
                    SlaveArrows.Add(arrow);
                }
            }

            public void DropSlave (CMouseCaptureState this_state, CMouseCaptureState next_state)
            {
                //   Переходы машины состояний [B1] и [B2] 
                if ((next_state == CapturedSecond) || (next_state == CapturedMaster))
                {
                    HostForm.listBox1.Items.Add("DropSlave : " + this_state);

                    Arrows.Arrow arrow = ButtonToArrow(this_state.CapturedButton);
                    SlaveArrows.Remove(arrow);

                    this_state.CapturedButton = MouseButtons.None;
                }
            }

            public void DropEverything (CMouseCaptureState this_state, CMouseCaptureState next_state)
            {
                HostForm.listBox1.Items.Add("DropEverything : " + this_state);

                MasterArrow = null;
                SlaveArrows.Clear();
            }

            //---------------------------------------------------------------------------



            public void Movement (int x, int y)
            {
                if (MasterArrow == null) return;


                Vector2D mouse_point = HostForm.CreateMouseVector(x, y);

                Angle delta_angle = MasterArrow.Vector.Angle.Mirror;


                MasterArrow.Vector = mouse_point;
                
                foreach (Arrows.Arrow arrow in SlaveArrows)
                {
                    Vector2D angle_between = arrow.Vector;
                    angle_between.Rotate(delta_angle);
                    Angle delta_rotation = angle_between.Angle;

                    Vector2D new_direction = mouse_point;
                    new_direction.Rotate(delta_rotation);
                    arrow.Vector = new_direction;
                }


                HostForm.DebugPieStartAngle      = HostForm.AllArrows.User1.Vector.AbsAngle.Degrees;
                HostForm.DebugPieRelativeAngle   = HostForm.AllArrows.User1.Vector.AngleBetween(HostForm.AllArrows.User2.Vector).Degrees;
            }



            public bool IncreaseCaptureLevel (MouseButtons button)
            {
                CMouseCaptureState higher_state = Machine.AllStates.NextTo(Machine.State) as CMouseCaptureState;

                if (higher_state == null)
                {
                    return false;
                }

                foreach (CMouseCaptureState s in Machine.AllStates)
                {
                    if (s == Machine.State) break;
                }

                higher_state.CapturedButton = button;

                Machine.GoTo(higher_state);

                return true;
            }

            public bool DecreaseCaptureLevel (MouseButtons button)
            {
                if
                (
                    (Machine.State is CMouseCaptureState) &&
                    ((Machine.State as CMouseCaptureState).CapturedButton == button)
                )
                {
                    IState prev_state = Machine.AllStates.PreviousTo(Machine.State);

                    if (prev_state != null)
                    {
                        Machine.GoTo(prev_state);
                        return true;
                    }
                }

                if (Machine.State != Released)
                {
                    Machine.GoTo(Released);
                }

                return false;
            }

            public bool CaptureMode
            {
                get { return ( Machine.State != Released ); }
            }
        }
    }
}
