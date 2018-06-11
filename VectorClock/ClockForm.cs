using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using My.MathUtils;
using My.Utils;

namespace VectorClock
{
    public partial class ClockForm : Form
    {
        public class Arrows : IEnumerable
        {
            public enum ArrowIndex : int
            {
                Alarm       = 0,
                Hour        = 1,
                Minute      = 2,
            }

            public class Arrow
            {
                public Vector2D Vector;
                public ClockVisualStyle.ClockArrow VisualStyle;

                public float        ClockValue;
                public ClockCycle   ClockCycle;

                public Arrow (ClockCycle aCycle)
                {
                    Vector = new Vector2D (0, -1);
                    ClockValue = 0;
                    ClockCycle = aCycle;
                }
            }

            private Arrow[] AllArrows = new Arrow [8] /*
            {
                new Arrow (ClockCycle.Minutes) { VisualStyle = new ClockVisualStyle.ClockArrow {
                }},
            }*/;

            public Arrows (ClockVisualStyle BaseStyle)
            {
                ClockStyle = BaseStyle;

                for (int I = 0; I < BaseStyle.Arrows.All.Length; I++)
                {
                    AllArrows[I] = new Arrow(ClockCycle.None)
                    {
                        VisualStyle = BaseStyle.Arrows.All[I]
                    };
                }

                Alarm       .ClockCycle = ClockCycle.Minutes;
                Hour        .ClockCycle = ClockCycle.Hours;
                Minute      .ClockCycle = ClockCycle.Minutes;
                Second      .ClockCycle = ClockCycle.Seconds;
                SecGhost    .ClockCycle = ClockCycle.Seconds;

                User1.Vector.Rotate(new Angle(30, AngleType.Degrees));
                User2.Vector.Rotate(new Angle(120, AngleType.Degrees));
                //User3.Vector.Rotate(new Angle(-90, AngleType.Degrees));
            }

            public ClockVisualStyle ClockStyle;

            public Arrow SecGhost   { get { return AllArrows[0]; } }
            public Arrow Alarm      { get { return AllArrows[1]; } }
            public Arrow Hour       { get { return AllArrows[2]; } }
            public Arrow Minute     { get { return AllArrows[3]; } }
            public Arrow Second     { get { return AllArrows[4]; } }

            public Arrow User1      { get { return AllArrows[5]; } }
            public Arrow User2      { get { return AllArrows[6]; } }
            public Arrow User3      { get { return AllArrows[7]; } }

            public Arrow this [int Index]
            {
                get { return AllArrows[Index]; }
            }

            public IEnumerator GetEnumerator ()
            {
                return AllArrows.GetEnumerator();
            }

            public void UpdateVectors ()
            {
                foreach (Arrow A in AllArrows)
                {
                    if (A.ClockCycle != ClockCycle.None)
                    {
                        A.Vector = Vector2D.UnitVectorFromAngle
                        (
                            GetClockAngle(A.ClockValue, A.ClockCycle)
                        );
                    }
                }
            }
        }

        public Arrows AllArrows = new Arrows (new ClockVisualStyle ());

        public NumericUpDown[] ClockInputs = new NumericUpDown [4];

        public float DebugPieStartAngle     = 0;
        public float DebugPieRelativeAngle  = 0;



        public ClockForm ()
        {
            InitializeComponent();

            VectorManipulator = new CManipulator (this);

            AllArrows.ClockStyle.Numbers.SecondaryMinutesFont = this.MinFontLabel.Font;
            AllArrows.ClockStyle.Numbers.PrimaryMinutesFont   = this.BMinFontLabel.Font;
            AllArrows.ClockStyle.Numbers.HoursFont            = this.HrsFontLabel.Font;

            ClockInputs[0] = this.numericUpDown1;
            ClockInputs[1] = this.numericUpDown2;
            ClockInputs[2] = this.numericUpDown3;
            //ClockInputs[3] = this.numericUpDown4;
            //ClockInputs[4] = this.numericUpDown5;
        }



        public enum ClockCycle : int
        {
            None = 0,
            Hours = 12,
            Minutes = 60,
            Seconds = 60,
        }

        public static Angle GetClockAngle (float aClockValue, ClockCycle aCycle, float aMod = -90.0f)
        {
            if (aCycle == ClockCycle.None) return new Angle (aClockValue, AngleType.Undefined);

            float CycleBase = (int) aCycle;

            //  Вычитаем превышение часов (более 12) или минут (более 60). 
            aClockValue = aClockValue - ((float) Math.Round(aClockValue / CycleBase) * CycleBase);

            float Result = (360.0f / CycleBase) * aClockValue;
            Result += aMod;
            return new Angle (Result, AngleType.Degrees);
        }



        public void RenderClockArrow
        (
            Graphics aCanvas,
            float aBaseX,
            float aBaseY,
            Vector2D aV,
            ClockVisualStyle.ClockArrow aArrowStyle,
            ClockVisualStyle aClockStyle
        )
        {
            aCanvas.DrawLine
            (
                aArrowStyle.Style.LinePen,
                aBaseX - (aV.A * (aClockStyle.Size + aArrowStyle.LineBackwardTip.Constant) * aArrowStyle.LineBackwardTip.Scaled),
                aBaseY - (aV.B * (aClockStyle.Size + aArrowStyle.LineBackwardTip.Constant) * aArrowStyle.LineBackwardTip.Scaled),
                aBaseX + (aV.A * (aClockStyle.Size + aArrowStyle.LineSize.Constant) * aArrowStyle.LineSize.Scaled),
                aBaseY + (aV.B * (aClockStyle.Size + aArrowStyle.LineSize.Constant) * aArrowStyle.LineSize.Scaled)
            );

            aCanvas.FillEllipse
            (
                aArrowStyle.Style.FillBrush,
                aBaseX - aArrowStyle.CircleSize.Constant,
                aBaseY - aArrowStyle.CircleSize.Constant,
                2 * aArrowStyle.CircleSize.Constant,
                2 * aArrowStyle.CircleSize.Constant
            );
        }

        public void RenderClockLabel
        (
            Graphics aCanvas,
            string aCaption,
            float aBaseX,
            float aBaseY,
            Vector2D aV,
            SizeFactor aPos,
            StyleInfo aStyle,
            Font aFont,
            ClockVisualStyle aClockStyle
        )
        {
            float TextRectSize = 100.0f;
            RectangleF TR = new RectangleF
            (
                aBaseX + (aV.A * (aClockStyle.Size + aPos.Constant) * aPos.Scaled) - (TextRectSize / 2),
                aBaseY + (aV.B * (aClockStyle.Size + aPos.Constant) * aPos.Scaled) - (TextRectSize / 2),
                TextRectSize,
                TextRectSize
            );

            StringFormat Fmt = new StringFormat
            {
                Alignment       = StringAlignment.Center,
                LineAlignment   = StringAlignment.Center
            };

            aCanvas.DrawString(aCaption, aFont, aStyle.FillBrush, TR, Fmt);
        }

        public void RenderClockMark
        (
            Graphics aCanvas,
            float aBaseX,
            float aBaseY,
            Vector2D aV,
            SizeFactor aPosSize,
            StyleInfo aStyle,
            ClockVisualStyle aClockStyle
        )
        {
            aCanvas.DrawLine
            (
                aStyle.LinePen,
                aBaseX + (aV.A * aClockStyle.Size * aPosSize.Scaled),
                aBaseY + (aV.B * aClockStyle.Size * aPosSize.Scaled),
                aBaseX + (aV.A * (aClockStyle.Size + aPosSize.Constant) * aPosSize.Scaled),
                aBaseY + (aV.B * (aClockStyle.Size + aPosSize.Constant) * aPosSize.Scaled)
            );
        }

        public void RenderClock
        (
            Graphics aCanvas,
            float aBaseX,
            float aBaseY,
            ClockVisualStyle aStyle
        )
        {
            for (int M = 1, H = 1; M <= 60; M++)
            {
                bool IsHourLineMark = (M % 5 == 0);

                Vector2D LineVector = Vector2D.UnitVectorFromAngle( GetClockAngle(M, ClockCycle.Minutes) );

                if (IsHourLineMark)
                {
                    RenderClockMark(aCanvas, aBaseX, aBaseY, LineVector, aStyle.LineMarks.HourPosSize,
                        aStyle.LineMarks.Hours, aStyle);
                }
                else
                {
                    RenderClockMark(aCanvas, aBaseX, aBaseY, LineVector, aStyle.LineMarks.MinPosSize,
                        aStyle.LineMarks.Minutes, aStyle);
                }

                RenderClockLabel(aCanvas, M.ToString(), aBaseX, aBaseY, LineVector,
                    aStyle.Numbers.MinutesPosition,
                    IsHourLineMark ? aStyle.Numbers.PrimaryMinutes : aStyle.Numbers.SecondaryMinutes,
                    IsHourLineMark ? aStyle.Numbers.PrimaryMinutesFont : aStyle.Numbers.SecondaryMinutesFont,
                    aStyle
                );

                if (IsHourLineMark)
                {
                    RenderClockLabel(aCanvas, H.ToString(), aBaseX, aBaseY, LineVector,
                        aStyle.Numbers.HoursPosition,
                        aStyle.Numbers.Hours,
                        aStyle.Numbers.HoursFont,
                        aStyle
                    );

                    H++;
                }
            }

            foreach (Arrows.Arrow A in AllArrows)
            {
                if (RadioBtn_TestVectors.Checked && (A.ClockCycle == ClockCycle.None))
                {
                    RenderClockArrow(aCanvas, aBaseX, aBaseY, A.Vector, A.VisualStyle, aStyle);
                }
                else if ((!RadioBtn_TestVectors.Checked) && (A.ClockCycle != ClockCycle.None))
                {
                    RenderClockArrow(aCanvas, aBaseX, aBaseY, A.Vector, A.VisualStyle, aStyle);
                }
            }
        }



        private void TimerObject_Tick (object sender, EventArgs e)
        {
            if (RadioBtn_CurTime.Checked)
            {
                DateTime N = DateTime.Now;

                AllArrows.Hour      .ClockValue = N.Hour + (N.Minute / 60.0f);
                AllArrows.Minute    .ClockValue = N.Minute + (N.Second / 60.0f);
                AllArrows.Second    .ClockValue = N.Second;
                AllArrows.SecGhost  .ClockValue = N.Second + (N.Millisecond / 1000.0f);

                this.UpdateClockUserInputs();    

                //this.Text = $"{this.Hour,00:F3} : {this.Minute,00:F3} : {this.Second,00:F3}";
            }

            if (! RadioBtn_TestVectors.Checked)
            {
                DebugPieStartAngle      = AllArrows.Minute.Vector.AbsAngle.Degrees;
                DebugPieRelativeAngle   = AllArrows.Minute.Vector.AngleBetween(AllArrows.SecGhost.Vector).Degrees;
            }

            this.Repaint();

            VectorManipulator.Machine.PrintDebugInfo(listBox2);
        }

        private void UpdateClockUserInputs ()
        {
            var InputValues = new float [5]
            {
                AllArrows.Hour      .ClockValue,
                AllArrows.Minute    .ClockValue,
                AllArrows.Second    .ClockValue,
                0,
                0,
            };

            int I = 0;
            foreach (NumericUpDown InputCtrl in ClockInputs) if (InputCtrl != null)
            {
                InputCtrl.ValueChanged -= NumericInput_ValueChanged;

                InputCtrl.Value = (decimal) InputValues[I++];

                InputCtrl.ValueChanged += NumericInput_ValueChanged;
            }
        }

        private void ClockForm_Paint (object sender, PaintEventArgs e)
        {
            Graphics G = e.Graphics;

            G.SmoothingMode = SmoothingMode.HighQuality;

            Vector2D Center = this.ClockCenter;
            float CX = Center.A;
            float CY = Center.B;

            AllArrows.UpdateVectors();

            Vector2D HourVector = AllArrows.Hour.Vector;
            Vector2D MinVector  = AllArrows.Minute.Vector;
            Vector2D SecVector  = AllArrows.Second.Vector;

            //Angle AngleBetween_H_M = new Angle(HourVector.AngleBetween2(MinVector).Radians * -1.0f, AngleType.Radians);
            //Angle AngleBetween_M_S = new Angle(MinVector.AngleBetween2(SecVector).Radians * -1.0f, AngleType.Radians);

            float PieSize = 150.0f;

            using
            (
                Brush G1 = new SolidBrush(Color.FromArgb(128, Color.Gold)) /* new LinearGradientBrush
                (
                    new PointF
                    {
                        X = MinVector.A * PieSize,
                        Y = MinVector.B * PieSize,
                    },
                    new PointF
                    {
                        X = HourVector.A * PieSize + 1,
                        Y = HourVector.B * PieSize + 1,
                    },
                    ClockStyle.Arrows.Minute.Style.LineColor,
                    ClockStyle.Arrows.Hour.Style.LineColor
                ) */
            )
            {
                G.FillPie
                (
                    G1,
                    CX - PieSize,
                    CY - PieSize,
                    2 * PieSize,
                    2 * PieSize,
                    DebugPieStartAngle,
                    DebugPieRelativeAngle
                );
            }

            RenderClock(G, CX, CY, AllArrows.ClockStyle);

            G.DrawArc
            (
                Pens.Orange,
                CX - 150.0f,
                CY - 150.0f,
                2  * 150.0f,
                2  * 150.0f,
                DebugPieStartAngle,
                DebugPieRelativeAngle
            );


            Vector2D AVec = AllArrows.User1.Vector;
            Vector2D BVec = AllArrows.User2.Vector;
            if (! RadioBtn_TestVectors.Checked)
            {
                AVec = AllArrows.Minute.Vector;
                BVec = AllArrows.SecGhost.Vector;
            }

            label1.Text =
                $"UsrArr1.fang = {AllArrows.User1.Vector.AbsAngle.Degrees:0.###} " +
                    $"({AllArrows.User1.Vector.Angle.Degrees:0.###})\n" +
                $"UsrArr2.fang = {AllArrows.User2.Vector.AbsAngle.Degrees:0.###} " +
                    $"({AllArrows.User2.Vector.Angle.Degrees:0.###})\n" +
                $"Angle = {AVec.AngleBetween(BVec).Degrees:0.###}";
        }



        private void NumericInput_ValueChanged (object aSender, EventArgs e)
        {
            RadioBtn_UserTime.Checked = true;

            AllArrows.Hour      .ClockValue = (float) numericUpDown1.Value;
            AllArrows.Minute    .ClockValue = (float) numericUpDown2.Value;
            AllArrows.Second    .ClockValue = (float) numericUpDown3.Value;
            AllArrows.SecGhost  .ClockValue = AllArrows.Second.ClockValue;

            this.Repaint();
        }

        private void LinkLabel1_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e)
        {
            RadioBtn_UserTime.Checked = true;

            AllArrows.Hour      .ClockValue = 0;
            AllArrows.Minute    .ClockValue = 0;
            AllArrows.Second    .ClockValue = 0;
            AllArrows.SecGhost  .ClockValue = AllArrows.Second.ClockValue;

            UpdateClockUserInputs();

            this.Repaint();
        }

        public void Repaint ()
        {
            this.Invalidate();
            this.Update();
        }



        public Vector2D ClockCenter
        {
            get
            {
                // HrsTrack.Bottom + (this.ClientRectangle.Height - HrsTrack.Bottom) / 2;

                return new Vector2D
                {
                    A = (this.ClientRectangle.Width  / 2),
                    B = (this.ClientRectangle.Height / 2),
                };
            }
        }

        public Vector2D CreateMouseVector (int aX, int aY)
        {
            Vector2D MouseVector;
            
            Vector2D Center = this.ClockCenter;
            float CX = Center.A;
            float CY = Center.B;

            MouseVector.A = aX;
            MouseVector.B = aY;

            MouseVector -= Center;

            MouseVector = MouseVector.UnitVector;

            return MouseVector;
        }



        private void ClockForm_MouseDown (object sender, MouseEventArgs e)
        {
            if (VectorManipulator.IncreaseCaptureLevel(e.Button))
            {
                //  Переключаем часы в режим трех управляемых мышью стрелок. 
                this.RadioBtn_TestVectors.Checked = true;

                VectorManipulator.Movement(e.X, e.Y);
                this.Repaint();
            }
        }

        private void ClockForm_MouseMove (object sender, MouseEventArgs e)
        {
            if (VectorManipulator.CaptureMode)
            {
                VectorManipulator.Movement(e.X, e.Y);
                this.Repaint();
            }
        }

        private void ClockForm_MouseUp (object sender, MouseEventArgs e)
        {
            if (VectorManipulator.DecreaseCaptureLevel(e.Button))
            {
                VectorManipulator.Movement(e.X, e.Y);
                this.Repaint();
            }
        }



        private void RadioBtn_TestVectors_MouseDown (object sender, MouseEventArgs e)
        {
        }

        private void RadioBtn_TestVectors_CheckedChanged (object sender, EventArgs e)
        {
            DebugPieStartAngle      = AllArrows.User1.Vector.AbsAngle.Degrees;
            DebugPieRelativeAngle   = AllArrows.User1.Vector.AngleBetween(AllArrows.User2.Vector).Degrees;

            this.Repaint();
        }
    }
}
