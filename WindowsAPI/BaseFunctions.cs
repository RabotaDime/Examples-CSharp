using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



namespace My.Windows.API
{
    public struct SysError
    {
        public SysErrorCode     KnownValue;
        public uint             Code;

        public override string ToString ()
        {
            return $"{KnownValue} ({Code})";
        }
    }



    public class CBaseFunctions
    {
        //===================================================================================== 
        //   Как использовать GetLastError и SetLastError в C# среде:
        //   https://blogs.msdn.microsoft.com/adam_nathan/2003/04/25/getlasterror-and-managed-code/
        //===================================================================================== 



        //   Получение последнего кода ошибки от Windows API для текущей «нитки». 
        //   
        //   Оригинал: windows.h > WinUser.h > errhandlingapi.h
        //=====================================================================================
        //   Документация: 
        //   https://msdn.microsoft.com/en-us/library/windows/desktop/ms679360(v=vs.85).aspx
        //===================================================================================== 
        //[DllImport(CLibraries.Kernel32)]
        //public static extern uint GetLastError ();
        public static uint GetLastError ()
        {
            return (uint) Marshal.GetLastWin32Error();
        }

        //   Установка последнего кода ошибки Windows API для текущей «нитки». 
        //   
        //   Оригинал: windows.h > WinUser.h > errhandlingapi.h
        //=====================================================================================
        //   Документация: 
        //   https://msdn.microsoft.com/en-us/library/windows/desktop/ms679360(v=vs.85).aspx
        //===================================================================================== 
        //[DllImport(CLibraries.Kernel32)]
        //public static extern void SetLastError (uint error_code);
        public static void SetLastError (uint error_code)
        {
            throw new InvalidOperationException("Please don't use SetLastError in managed environment.");
        }

        public static SysError LastError
        {
            get
            {
                uint error_code = (uint) Marshal.GetLastWin32Error();

                if (Enum.IsDefined(typeof(SysErrorCode), error_code))
                {
                    return new SysError { KnownValue = (SysErrorCode) error_code, Code = error_code };
                }
                else
                {
                    return new SysError { KnownValue = SysErrorCode.UndefinedError, Code = error_code };
                }
            }
        }

        public static uint LastErrorCode
        {
            get
            {
                return (uint) Marshal.GetLastWin32Error();
            }
        }
    }
}
