using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaseConsole = System.Console; 



namespace My.Utils.CustomConsole
{
    public class CustomConsole
    {
    }



    public static class Console
    {
        public static CustomConsole PrimaryConsole = new CustomConsole ();

        public const string TitleDecorStart = "( ";
        public const string TitleDecorEnd   = " )";
        public const string HorizontalFill  = "-";

        public const string EmptyString  = "";

        public const char CR = '\r';
        public const char LF = '\n';

        public const string WinEOL = "\r\n";

        public const string DefaultAnyKeyMsg = "Press any key to quit...";



        //   Иногда вывод в консоль требует показать какое-то сообщение
        //   вне потока вывода. Это один из странных, но работоспособных
        //   способов. 
        public static void MsgBox (string msg)
        {
            // TODO : Организовать правильный поиск окна консоли. 
            //BaseConsole.Title

            //IntPtr console_handle = Process.GetCurrentProcess().MainWindowHandle;
            //IWin32Window w = Control.FromHandle(console_handle);
            //MessageBox.Show(w, msg);

            MessageBox.Show(msg);
        }



        //  Console.Horizontal("+12345-", "///START!!!", "\\\\\\END!!!");



        public static void HorizontalTitle
        (
            string title,
            int x_start,
            string fill         = HorizontalFill,
            string title_start  = TitleDecorStart, 
            string title_end    = TitleDecorEnd
        )
        {
            int max_size = BaseConsole.WindowWidth;

            const int MaxDecorSize = 8;
            if (title_start .Length > MaxDecorSize) title_start = title_start .Substring(0, MaxDecorSize);
            if (title_end   .Length > MaxDecorSize) title_end   = title_end   .Substring(0, MaxDecorSize);

            int title_size = Math.Min(max_size - title_start.Length - title_end.Length, title.Length);
            if (title.Length > title_size)
            {
                title_size = title_size - 3;
                if (title_size > 0)
                    title = title.Substring(0, title_size) + "...";
                else
                    title = title.Substring(0, 3);
            }

            Console.Horizontal(fill);
            BaseConsole.CursorTop--;

            BaseConsole.CursorLeft += x_start;
            BaseConsole.Write(title_start);
            BaseConsole.Write(title);
            BaseConsole.Write(title_end);

            BaseConsole.CursorLeft = 0;
            BaseConsole.CursorTop++;
        }

        public static void Horizontal
        (
            string fill     = HorizontalFill,
            string start    = EmptyString,
            string end      = EmptyString
        )
        {
            int max_size = BaseConsole.WindowWidth;

            int start_size  = Math.Min(start.Length, max_size);
            int end_size    = Math.Min(end.Length, max_size - start_size);
            int fill_size   = max_size - start_size - end_size;

            BaseConsole.Write(start.Substring(0, start_size));

            int step = 0;
            while (fill_size > 0)
            {
                BaseConsole.Write(fill[step]);

                fill_size--;

                step++;
                if (step >= fill.Length) step = 0;
            }

            BaseConsole.Write(end.Substring(0, end_size));
        }



        public static ConsoleColor TextColor
        {
            get { return BaseConsole.ForegroundColor; }
            set { BaseConsole.ForegroundColor = value; }
        }

        public static ConsoleColor ForegroundColor
        {
            get { return BaseConsole.ForegroundColor; }
            set { BaseConsole.ForegroundColor = value; }
        }

        public static ConsoleColor BackColor
        {
            get { return BaseConsole.BackgroundColor; }
            set { BaseConsole.BackgroundColor = value; }
        }

        public static ConsoleColor BackgroundColor
        {
            get { return BaseConsole.BackgroundColor; }
            set { BaseConsole.BackgroundColor = value; }
        }

        public static void ResetColor ()
        {
            BaseConsole.ResetColor();
        }



        public static void SetCursorPosition (int x, int y)
        {
            BaseConsole.SetCursorPosition(x, y);
        }

        public static int CursorLeft
        {
            get { return BaseConsole.CursorLeft; }
            set { BaseConsole.CursorLeft = value; }
        }

        public static int CursorTop
        {
            get { return BaseConsole.CursorTop; }
            set { BaseConsole.CursorTop = value; }
        }

        public static bool CursorVisible
        {
            get { return BaseConsole.CursorVisible; }
            set { BaseConsole.CursorVisible = value; }
        }

        public static int CursorSize
        {
            get { return BaseConsole.CursorSize; }
            set { BaseConsole.CursorSize = value; }
        }



        public static void FlushOutput ()
        {
            BaseConsole.Out.Flush();
        }



        public static void Write (int value)
        {
            BaseConsole.Write(value);
        }

        public static void Write (string value)
        {
            BaseConsole.Write(value);
        }

        public static void WriteLine (int value)
        {
            BaseConsole.WriteLine(value);
        }

        public static void WriteLine (string value)
        {
            BaseConsole.WriteLine(value);
        }

        public static void WriteLine (params object[] args)
        {
            ConsoleColor prev_color = BaseConsole.ForegroundColor;
            string sep = String.Empty;

            foreach (object a in args)
            {
                if (a is ConsoleColor)
                {
                    BaseConsole.ForegroundColor = (ConsoleColor) a;
                }
                else
                {
                    BaseConsole.Write(sep);

                    if (a is string)        BaseConsole.Write(a as string);
                    else if (a is int)      BaseConsole.Write((int) a);
                    else if (a is uint)     BaseConsole.Write((uint) a);
                    else if (a is byte)     BaseConsole.Write((byte) a);
                    else if (a is sbyte)    BaseConsole.Write((sbyte) a);
                    else if (a is short)    BaseConsole.Write((short) a);
                    else if (a is ushort)   BaseConsole.Write((ushort) a);
                    else if (a is long)     BaseConsole.Write((long) a);
                    else if (a is ulong)    BaseConsole.Write((ulong) a);

                    sep = " ";
                }

            }

            BaseConsole.WriteLine();
            BaseConsole.ForegroundColor = prev_color;
        }

        public static void WriteLine ()
        {
            BaseConsole.WriteLine();
        }



        public static char ReadChar (char stop_char = char.MinValue)
        {
            int char_value = BaseConsole.Read();

            if ((char_value >= char.MinValue) && (char_value <= char.MaxValue))
                return Convert.ToChar(char_value);
            else
                return stop_char;
        }

        public static string ReadLine ()
        {
            return BaseConsole.ReadLine();
        }

        public static ConsoleKey ReadAnyKey (string info_message = Console.DefaultAnyKeyMsg)
        {
            if (info_message.Length > 0) BaseConsole.WriteLine(info_message);

            //  Считываю клавишу без показа ее на экран (флаг intercept: true)
            ConsoleKeyInfo key_info = BaseConsole.ReadKey(intercept: true);

            return key_info.Key;
        }
    }
}
