using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IntegerSetConverter
{
    public class CConvertOptions
    {
        public const String DefaultSeparator     = ",";
        public const String DefaultJoint         = "..";

        public readonly Char[] DefaultWhitespaceSet = new Char[] { ' ', '\t' };



        public string   SeparatorMask       = DefaultSeparator;
        public string   JointMask           = DefaultJoint;
        public int      MaxElementLength    = 20;                 //  Макс. кол-во символов в UInt64 литерале. 



        public static bool Validate (string aSeparator, string aJoint)
        {
            if (aSeparator == aJoint)
                return false;

            return true;
        }
    }



    interface INumericConverter
    {
        string Convert
        (
            int[] aData,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        );

        int[] Convert
        (
            string aData,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        );

        string ConverterName { get; }
    }



    interface INumericConverterEx
    {
        string Convert (int[] aData, CConvertOptions aOptions);
        int[] Convert (string aData, CConvertOptions aOptions);
    }
}

