using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace My.Windows.API
{
    //   Основные коды ошибок Windows API, которые используются в моих функциях. 
    //=====================================================================================
    //   https://msdn.microsoft.com/en-us/library/cc231199.aspx
    //=====================================================================================
    public enum SysErrorCode : uint
    {
        Success = 0,
        UndefinedError = uint.MaxValue,

        AccessDenied    = 5,
        Busy            = 170,

        CancelledByUser     = 1223, 

        InvalidHandle       = 6,
        InvalidName         = 123,

        FileNotFound    = 2,
        PathNotFound    = 3,
        DiskFull        = 112,
        ErrorDirectory  = 267,

        NotEnoughMemory     = 8,
        OutOfMemory         = 14,
    }
}

