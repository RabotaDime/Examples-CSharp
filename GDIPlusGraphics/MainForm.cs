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
using My.Utils;

namespace GDIPlusGraphics
{
    public partial class MainForm : Form
    {
        public MainForm ()
        {
            InitializeComponent();
        }

        private void MainForm_Paint (object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;


            using
            (
                var gradient_brush = new LinearGradientBrush
                (
                    this.ClientRectangle,
                    Color.LightBlue, Color.LightCoral,
                    45.0F
                )
            )
            {
                canvas.FillRectangle(gradient_brush, this.ClientRectangle);
            }


            int cell_size = 16;
            int zoom = 3;

            canvas.SmoothingMode     = SmoothingMode.Default;
            canvas.InterpolationMode = InterpolationMode.Default;
            canvas.PixelOffsetMode   = PixelOffsetMode.Default;

            canvas.DrawImage
            (
                Properties.Resources.tree_0,
                50 + 0,
                50,
                (3 * cell_size) * zoom,
                (6 * cell_size) * zoom
            );

            canvas.SmoothingMode     = SmoothingMode.None;
            canvas.InterpolationMode = InterpolationMode.NearestNeighbor;
            canvas.PixelOffsetMode   = PixelOffsetMode.Half;

            canvas.DrawImage
            (
                Properties.Resources.tree_0,
                50 + 200,
                50,
                (3 * cell_size) * zoom,
                (6 * cell_size) * zoom
            );

            canvas.DrawImage_Alpha
            (
                Properties.Resources.tree_1,
                50 + 400,
                50,
                (3 * cell_size) * zoom,
                (6 * cell_size) * zoom,
                0.75F
            );

            canvas.DrawImage_Alpha
            (
                Properties.Resources.tree_1,
                50 + 400 + 50,
                50 + 50,
                (3 * cell_size) * zoom,
                (6 * cell_size) * zoom,
                0.25F
            );

            canvas.DrawImage_RGBA
            (
                Properties.Resources.tree_1,
                50 + 600 + 50,
                50 + 50,
                (3 * cell_size) * zoom,
                (6 * cell_size) * zoom,

                1,
                1,
                10,
                1
            );

            canvas.DrawImage_RGBA
            (
                Properties.Resources.tree_1,
                50 + 600,
                50,
                (3 * cell_size) * zoom,
                (6 * cell_size) * zoom,

                0.5F,
                2,
                0.5F,
                1
            );
        }

        private void MainForm_Resize (object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
