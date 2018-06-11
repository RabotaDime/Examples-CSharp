using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;



namespace My.Windows.API
{
    public struct WndHandle
    {
        public static readonly WndHandle Zero = new WndHandle (IntPtr.Zero);

        public IntPtr RawHandle;

        public WndHandle (IntPtr handle_value)
        {
            this.RawHandle = handle_value;
        }

        public bool IsNull { get { return (RawHandle == IntPtr.Zero); } }

        public bool IsValid
        {
            get
            {
                if (this.RawHandle == IntPtr.Zero)
                    return false;
                else
                    return CWindowFunctions.IsWindow(this.RawHandle);
            }
        }

        public override string ToString ()
        {
            if (RawHandle == IntPtr.Zero)
                return "Zero Handle";
            else if (RawHandle == CWindowFunctions.MessageOnlyHandle)
                return "Message-Only Container";
            else if (RawHandle == CWindowFunctions.GetDesktopWindow())
                return "Desktop Handle";
            else if (CWindowFunctions.IsWindow(RawHandle))
                return $"Valid Handle [0x{RawHandle.ToInt64():X8}]";
            else
                return $"Invalid Handle [0x{RawHandle.ToInt64():X8}]";
        }
    }



    public class CWindowFunctions
    {
        //   Оригинал: windows.h > WinUser.h
        //   #define HWND_BROADCAST ((HWND)0xffff)
        public static readonly IntPtr BroadcastHandle = new IntPtr (0xffff);

        //   Оригинал: windows.h > WinUser.h
        //   #define HWND_MESSAGE ((HWND)-3)
        public static readonly IntPtr MessageOnlyHandle = new IntPtr (-3); 



        //   Поиск окна по заданным данным. Расширенная функция. 
        //   
        //   Оригинал: windows.h > WinUser.h
        //=====================================================================================
        //   Документация: 
        //   https://msdn.microsoft.com/en-us/library/windows/desktop/ms633500(v=vs.85).aspx 
        //===================================================================================== 
        [DllImport(CLibraries.User32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx
        (
            //   Родительское окно, среди дочерних окон которого будет произведен поиск.
            //   Если равен нулю, то за родительское используется окно рабочего стола.
            //   Особое значение "MessageOnlyHandle" для скрытых окон, обработчиков сообщений
            //   (message-only windows). 
            IntPtr parent_handle,
            //   Дочернее окно, после которого будет произведен поиск. 
            //   Если равен нулю, то поиск работает по всем дочерним окнам с самого начала. 
            IntPtr child_after_handle,
            //   Идентификатор класса окна. 
            string window_class,
            //   Заголовок окна. 
            string window_title
        );

        [DllImport(CLibraries.User32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx
        (
            IntPtr parent_handle,
            IntPtr child_after_handle,
            IntPtr window_class,
            string window_title
        );

        [DllImport(CLibraries.User32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx
        (
            IntPtr parent_handle,
            IntPtr child_after_handle,
            string window_class,
            IntPtr window_title
        );

        public static IntPtr FindWindowByClass (string window_class)
        {
            return FindWindowEx(IntPtr.Zero, IntPtr.Zero, window_class, IntPtr.Zero);
        }

        public static IntPtr FindWindowByClass (string window_class, IntPtr parent_handle)
        {
            return FindWindowEx(parent_handle, IntPtr.Zero, window_class, IntPtr.Zero);
        }

        public static IntPtr FindWindowByTitle (string window_title)
        {
            return FindWindowEx(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, window_title);
        }

        public static IntPtr FindWindowByTitle (string window_title, IntPtr parent_handle)
        {
            return FindWindowEx(parent_handle, IntPtr.Zero, IntPtr.Zero, window_title);
        }



        //   Проверка указателя на то, что в данный момент существует такое окно
        //   и это корректный Window Handle. 
        //   
        //   Оригинал: windows.h > WinUser.h
        //=====================================================================================
        //   Документация: 
        //   https://msdn.microsoft.com/en-us/library/windows/desktop/ms633528(v=vs.85).aspx
        //===================================================================================== 
        [DllImport(CLibraries.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow (IntPtr handle_value);



        //   Получение указателя на окно рабочего стола. 
        //   
        //   Оригинал: windows.h > WinUser.h
        //=====================================================================================
        //   Документация: 
        //   https://msdn.microsoft.com/en-us/library/windows/desktop/ms633528(v=vs.85).aspx
        //===================================================================================== 
        [DllImport(CLibraries.User32, SetLastError = true)]
        public static extern IntPtr GetDesktopWindow ();



        //   Получение указателя на окно рабочего стола. 
        //   
        //   Оригинал: windows.h > WinUser.h
        //=====================================================================================
        //   Документация: 
        //   https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx
        //===================================================================================== 
        [DllImport(CLibraries.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos
        (
            IntPtr window_handle,
            IntPtr insert_after_handle,
            int x,
            int y,
            int w,
            int h,
            SWP_Flags flags
        );

        public static readonly IntPtr SWP_Handle_NoTopMost  = new IntPtr (-2);
        public static readonly IntPtr SWP_Handle_TopMost    = new IntPtr (-1);
        public static readonly IntPtr SWP_Handle_Top        = new IntPtr (0);
        public static readonly IntPtr SWP_Handle_Bottom     = new IntPtr (1);

        public enum SWP_Flags : uint
        {
            NoSize          = 0x0001,
            NoMove          = 0x0002,
            NoZOrder        = 0x0004,
            NoRedraw        = 0x0008,
            NoActivate      = 0x0010,
            DrawFrame       = 0x0020,
            FrameChanged    = 0x0020,
            ShowWindow      = 0x0040,
            HideWindow      = 0x0080,
            NoCopyBits      = 0x0100,
            NoOwnerZOrder   = 0x0200,
            NoReposition    = 0x0200,
            NoSendChanging  = 0x0400,
            DeferErase      = 0x2000,
            AsyncWindowPos  = 0x4000,
        }



        //   Получение полной области окна (не клиентская часть, а полная). 
        //   
        //   Оригинал: windows.h > WinUser.h
        //=====================================================================================
        //   Документация: 
        //   https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx
        //===================================================================================== 
        [DllImport(CLibraries.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect
        (
            IntPtr window_handle,
            out WndRect rectangle
        );
    }



    [StructLayout(LayoutKind.Sequential)]
    public struct WndRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Rectangle AsRectangle
        {
            get
            {
                return new Rectangle
                (
                    Left,
                    Top,
                    Right - Left,
                    Bottom - Top
                );
            }
        }
    }



    public static class CRectangleExtension
    {
        public static WndRect AsWndRect (this Rectangle r)
        {
            return new WndRect
            {
                Top     = r.Top,
                Right   = r.Right,
                Bottom  = r.Bottom,
                Left    = r.Left,
            };
        }
    }
}



