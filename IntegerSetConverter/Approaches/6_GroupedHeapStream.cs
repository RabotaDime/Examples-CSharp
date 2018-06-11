using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IntegerSetConverter
{
    class GroupedHeapStreamConverter : INumericConverter
    {
        string INumericConverter.ConverterName
        {
            get { return "GroupedHeapStreamConverter"; }
        }



        public static int[] Convert
        (
            string aData,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        )
        {
            throw new NotImplementedException();
        }



        public static string Convert
        (
            int[] aData,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        )
        {
            throw new NotImplementedException();
        }



        string INumericConverter.Convert (int[] aData, string aSeparator, string aJoint)
        {
            return Convert(aData, aSeparator, aJoint);
        }

        int[] INumericConverter.Convert (string aData, string aSeparator, string aJoint)
        {
            return Convert(aData, aSeparator, aJoint);
        }
    }
}
