using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Utils
{
    public static class GraphicsHelpers
    {
        public static void DrawImage_RGBA (this Graphics canvas, Image image, int x, int y, int w, int h, float r, float g, float b, float a)
        {
            ImageAttributes image_attribs = new ImageAttributes ();

            float [][] color_martix_values =
            {
                new float[] { r, 0, 0, 0, 0 },
                new float[] { 0, g, 0, 0, 0 },
                new float[] { 0, 0, b, 0, 0 },
                new float[] { 0, 0, 0, a, 0 },
                new float[] { 0, 0, 0, 0, 1 },
            };

            ColorMatrix color_matrix = new ColorMatrix(color_martix_values);

            image_attribs.SetColorMatrix
            (
                color_matrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap
            );

            canvas.DrawImage
            (
                image,
                new Rectangle(x, y, w, h),
                0, 0,
                image.Width, image.Height,
                GraphicsUnit.Pixel,
                image_attribs
            );
        }



        public static void DrawImage_Alpha (this Graphics canvas, Image image, int x, int y, int w, int h, float alpha)
        {
            ImageAttributes image_attribs = new ImageAttributes ();

            float [][] color_martix_values =
            {
                new float[] { 1, 0, 0, 0,       0 },
                new float[] { 0, 1, 0, 0,       0 },
                new float[] { 0, 0, 1, 0,       0 },
                new float[] { 0, 0, 0, alpha,   0 },
                new float[] { 0, 0, 0, 0,       1 },
            };

            ColorMatrix color_matrix = new ColorMatrix(color_martix_values);

            image_attribs.SetColorMatrix
            (
                color_matrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap
            );

            canvas.DrawImage
            (
                image,
                new Rectangle(x, y, w, h),
                0, 0,
                image.Width, image.Height,
                GraphicsUnit.Pixel,
                image_attribs
            );
        }
    }
}
