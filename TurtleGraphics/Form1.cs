using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using My.MathUtils;

using Цвет = System.Drawing.Color;

namespace TurtleGraphics
{
    public partial class Form1 : Form
    {
        double X, Y;
        double Угол;
        Цвет Цвет;

        public Form1 ()
        {
            InitializeComponent();
        }



        private void Линия (Graphics холст, Цвет цвет, double X1, double Y1, double X2, double Y2)
        {
            float центрX, центрY;

            центрX = pictureBox1.ClientSize.Width / 2;
            центрY = pictureBox1.ClientSize.Height / 2;

            холст.DrawLine
            (
                new Pen(цвет, 10),
                центрX + (float) X1,
                центрY + (float) Y1,
                центрX + (float) X2,
                центрY + (float) Y2
            );
        }

        private void pictureBox1_Paint (object sender, PaintEventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.BeginUpdate();

            Graphics Холст = e.Graphics; 
            Цвет[] цвета = new Цвет[] { Цвет.Blue, Цвет.Red, Цвет.Yellow, Цвет.Green };

            foreach (int x in Enumerable.Range(1, 8))
            {
                // Идем вперед
                double УголРад = Angle.DegToRad((float) Угол);
                double СдвигX = Math.Cos(УголРад) * x * 30;
                double СдвигY = Math.Sin(-УголРад) * x * 30;
                
                listBox1.Items.Add
                (
                    $"X = {X:0.###}, " +
                    $"Y = {Y:0.###}, " +
                    $"Угол = {Угол:0.###}"
                );

                Цвет = цвета[x % 4];

                Линия
                (
                    Холст,
                    Цвет,
                    X,
                    Y,
                    X + СдвигX,
                    Y + СдвигY
                );
                X += СдвигX;
                Y += СдвигY;

                // Разворот налево
                Угол += 90.0;
            }

            listBox1.EndUpdate();
        }
    }
}
