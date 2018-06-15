using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsIdleLoop
{
    public partial class CIdleLoopForm : Form, IIdleListener
    {
        public int CellSize = 16;
        public int PixelZoom = 3;

        public float ViewX = 0, ViewY = 0; 
        public float MovementX = 0, MovementY = 0;
        public float MovementSpeed = 200.0F;

        public int MovementXL = 0, MovementXR = 0;
        public int MovementYU = 0, MovementYD = 0;

        Color RenderBackColor = Color.FromArgb(255, 78, 61, 66); // Color.DarkGreen;
        Color GridPenColor = Color.Red;

        public StringBuilder HintText1 = new StringBuilder();



        public CIdleLoopForm ()
        {
            InitializeComponent();

            //
            MovementX = 0.5F;
            MovementY = -0.15F;
        }



        void IIdleListener.IdleLoop (float delta_time)
        {
            //  Перемещение
            ViewX += (MovementX * MovementSpeed * delta_time);
            ViewY += (MovementY * MovementSpeed * delta_time);

            HintText1.Clear();
            HintText1.Append
            (
                $"Обзор = (x: {ViewX:00000.000}, y: {ViewY:00000.000}),\n" +
                $"Elapsed = (ms: {CAppIdleLoop.ElapsedMS:0000000000.000}" +
                $", ticks: {CAppIdleLoop.ElapsedTicks:0000000000.000}),\n" +
                $"DeltaTime = ({delta_time:0.#####})"
            );

            //  Перерисовка  
            RenderBox1.Invalidate();
            RenderBox1.Update();

            System.Threading.Thread.Sleep(5);
            Application.DoEvents();
        }



        private void RenderBox1_Paint (object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            Rectangle inner_area = (sender as PictureBox).ClientRectangle;

            //  Свойства вывода
            canvas.SmoothingMode     = SmoothingMode.None;
            canvas.InterpolationMode = InterpolationMode.NearestNeighbor;
            canvas.PixelOffsetMode   = PixelOffsetMode.Half;

            //  Фон
            using (Brush back_brush = new SolidBrush(RenderBackColor))
            {
                canvas.FillRectangle(back_brush, inner_area);
            }

            //  Сетка
            using (Pen grid_pen = new Pen(GridPenColor, 0.5F))
            {
                RenderGrid(canvas, grid_pen);
            }

            canvas.DrawString(HintText1.ToString(), this.Font, Brushes.White, 25, 25);
        }



        public void RenderGrid (Graphics canvas, Pen grid_pen)
        {
            Rectangle inner_area = RenderBox1.ClientRectangle;

            int remainder;

            //  Расчет шага для сетки 
            int grid_gradation = CellSize * PixelZoom;

            //  Горизонтальная сетка 
            Math.DivRem((int) ViewY, grid_gradation, out remainder);
            int grid_y = grid_gradation - remainder - grid_gradation + 1;
            while (grid_y < inner_area.Width)
            {
                int line_from   = 0;
                int line_to     = inner_area.Width + 1;

                //  Рисуем одну линию сетки
                canvas.DrawLine(grid_pen, line_from, grid_y, line_to, grid_y);

                grid_y += grid_gradation;
            }

            //  Вертикальная сетка 
            Math.DivRem((int) ViewX, grid_gradation, out remainder);
            int grid_x = grid_gradation - remainder - grid_gradation + 1;
            while (grid_x < inner_area.Width)
            {
                int line_from   = 0;
                int line_to     = inner_area.Height + 1;

                //  Рисуем одну линию сетки
                canvas.DrawLine(grid_pen, grid_x, line_from, grid_x, line_to);

                grid_x += grid_gradation;
            }
        }



        private void CMainForm_KeyDown (object sender, KeyEventArgs e)
        {
            float fast_mode = (e.Shift) ? 2.0F : 1.0F; 

            if (e.KeyCode == Keys.Left)     MovementXL = -1;
            if (e.KeyCode == Keys.A)        MovementXL = -1;
            if (e.KeyCode == Keys.Right)    MovementXR = +1;
            if (e.KeyCode == Keys.D)        MovementXR = +1;
            if (e.KeyCode == Keys.Up)       MovementYU = -1;
            if (e.KeyCode == Keys.W)        MovementYU = -1;
            if (e.KeyCode == Keys.Down)     MovementYD = +1;
            if (e.KeyCode == Keys.S)        MovementYD = +1;

            //  Сложение компонентов (флаги движения по кнопкам)
            MovementX = (MovementXL + MovementXR) * fast_mode;
            MovementY = (MovementYU + MovementYD) * fast_mode;
        }

        private void CMainForm_KeyUp (object sender, KeyEventArgs e)
        {
            float fast_mode = (e.Shift) ? 2.0F : 1.0F; 

            if (e.KeyCode == Keys.Left)     MovementXL = 0;
            if (e.KeyCode == Keys.A)        MovementXL = 0;
            if (e.KeyCode == Keys.Right)    MovementXR = 0;
            if (e.KeyCode == Keys.D)        MovementXR = 0;
            if (e.KeyCode == Keys.Up)       MovementYU = 0;
            if (e.KeyCode == Keys.W)        MovementYU = 0;
            if (e.KeyCode == Keys.Down)     MovementYD = 0;
            if (e.KeyCode == Keys.S)        MovementYD = 0;

            //  Сложение компонентов (флаги движения по кнопкам)
            MovementX = (MovementXL + MovementXR) * fast_mode;
            MovementY = (MovementYU + MovementYD) * fast_mode;
        }
    }
}
