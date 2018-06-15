using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinFormsLayout
{
    public class SystemCursorsFix
    {
        const int IDC_HAND = 32649;

        private static Cursor Cursor = null;

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor (int hInstance, int lpCursorName);

        public static Cursor GetHandCursor ()
        {
            if (SystemCursorsFix.Cursor == null)
            {
                SystemCursorsFix.Cursor = new Cursor(LoadCursor(0, IDC_HAND));
            }

            return SystemCursorsFix.Cursor;
        }
    }
}
