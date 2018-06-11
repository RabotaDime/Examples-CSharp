using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace My.Utils
{
    public static class CEnumExtensions
    {
        public static bool HasOneOf (this Enum e, Enum flags)
        {
            ulong value  = Convert.ToUInt64(e);
            ulong mask   = Convert.ToUInt64(flags);

            //   Очищаю биты, которые не используются для проверки. 
            ulong clean_value = value & mask;

            //   Контрольные биты остались, а общее число битов равно единице.
            //   То есть, используется *только* один бит. 
            return (clean_value > 0) && (CBitwiseOps.BitsCount(clean_value) == 1);
        }



        public static bool HasNoneOf (this Enum e, Enum flags)
        {
            ulong value  = Convert.ToUInt64(e);
            ulong mask   = Convert.ToUInt64(flags);

            //   Очищаю биты, которые не используются для проверки. 
            ulong clean_value = value & mask;

            return CBitwiseOps.BitsCount(clean_value) == 0;
        }
    }
}
