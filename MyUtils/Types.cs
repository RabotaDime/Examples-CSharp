using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Utils
{
    public static class CTypesUtils
    {
        public const char DefaultFillChar = '0'; 

        public const int ByteSize       = 8 * sizeof(byte);
        public const int SByteSize      = 8 * sizeof(sbyte);
        public const int ShortSize      = 8 * sizeof(short);
        public const int UShortSize     = 8 * sizeof(ushort);
        public const int IntSize        = 8 * sizeof(int);
        public const int UIntSize       = 8 * sizeof(uint);
        public const int LongSize       = 8 * sizeof(long);
        public const int ULongSize      = 8 * sizeof(ulong);



        public static string ToBinary (this byte value, int width = ByteSize, char fill = DefaultFillChar)
        {
            return Convert.ToString(value, 2).PadLeft(width, fill);
        }

        public static string ToBinary (this sbyte value, int width = SByteSize, char fill = DefaultFillChar)
        {
            return Convert.ToString(value, 2).PadLeft(width, fill);
        }

        public static string ToBinary (this short value, int width = IntSize, char fill = DefaultFillChar)
        {
            return Convert.ToString(value, 2).PadLeft(width, fill);
        }

        public static string ToBinary (this ushort value, int width = UIntSize, char fill = DefaultFillChar)
        {
            return Convert.ToString(value, 2).PadLeft(width, fill);
        }

        public static string ToBinary (this int value, int width = IntSize, char fill = DefaultFillChar)
        {
            return Convert.ToString(value, 2).PadLeft(width, fill);
        }

        public static string ToBinary (this uint value, int width = UIntSize, char fill = DefaultFillChar)
        {
            return Convert.ToString(value, 2).PadLeft(width, fill);
        }

        public static string ToBinary (this long value, int width = LongSize, char fill = DefaultFillChar)
        {
            return Convert.ToString(value, 2).PadLeft(width, fill);
        }

        public static string ToBinary (this ulong value, int width = ULongSize, char fill = DefaultFillChar)
        {
            int bit_count = 8 * sizeof(ulong);
            int last_bit = 0;

            StringBuilder sb = new StringBuilder(bit_count, bit_count)
            {
                Length = bit_count
            };

            for (int i = 0; i < bit_count; i++)
            {
                ulong bit = (ulong) 1 & (ulong) value;
                sb[bit_count - 1 - i] = (bit == 1) ? '1' : '0';
                if (bit == 1) last_bit = i;
                value = value >> 1;
            }

            return sb.ToString().Substring(bit_count - 1 - last_bit).PadLeft(width, fill);
        }
    }
}
