
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IntegerSetConverter
{
    class CSharpEnvironmentConverter : INumericConverter
    {
        string INumericConverter.ConverterName
        {
            get { return "CSharpEnvironmentConverter"; }
        }



        public static int[] Convert
        (
            string aData,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        )
        {
            var Result = new List<int> { };


            string[] Subsets = aData.Split(new string[] { aSeparator }, StringSplitOptions.RemoveEmptyEntries);

            for (int I = 0; I < Subsets.Length; I++)
            {
                string[] RangeSubsetFound = Subsets[I].Split(new string[] { aJoint }, StringSplitOptions.None);

                if (RangeSubsetFound.Length == 1)
                {
                    int Value       = int.TryParse(RangeSubsetFound[0], out Value) && (Value > 0) ?
                                      Value :
                                      throw new ArgumentException("Invalid subset value.", "Data");

                    Result.Add(Value);
                }
                else if (RangeSubsetFound.Length == 2)
                {
                    int RangeStart  = int.TryParse(RangeSubsetFound[0], out RangeStart) && (RangeStart > 0) ?
                                      RangeStart :
                                      throw new ArgumentException("Invalid range subset value. From case.", "Data");

                    int RangeFinish = int.TryParse(RangeSubsetFound[1], out RangeFinish) && (RangeFinish > 0) ?
                                      RangeFinish :
                                      throw new ArgumentException("Invalid range subset value. To case.", "Data");

                    for (int R = RangeStart; R <= RangeFinish; R++)
                        Result.Add(R);
                }
                else
                {
                    throw new ArgumentException("Invalid range subset values. Too many or no data entries.", "Data");
                }
            }


            return Result.ToArray();
        }



        public static string Convert
        (
            int[] aData,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        )
        {
            StringBuilder Result = new StringBuilder ();


            //   Создаем пронумерованный набор данных. 
            IEnumerable<NumberRecord> DataSet = aData.Select
            (
                (NumericValue, Index) =>
                new NumberRecord
                {
                    OrderedIndex    = Index + 1,
                    Data            = NumericValue,
                    Difference      = NumericValue - (Index + 1), 
                }
            );

            //   Делаем выборку по группам. 
            IEnumerable<NumberRange> ResultSet =
                from Data in DataSet
                orderby Data.OrderedIndex
                group Data by Data.Difference into GroupedData
                select new NumberRange()
                {
                    DifferenceKey   = GroupedData.Key,
                    StartValue      = GroupedData.Min(x => x.Data),
                    Count           = GroupedData.Count(),
                };

            //   Формируем строку из полученных данных. 
            foreach (var R in ResultSet)
            {
                //Result.AppendFormat("\n[Key={0}, Val={1}, Count={2}]", R.DifferenceKey, R.StartValue, R.Count);

                if (R.Count == 1)
                    Result.AppendFormat
                    (
                        "{0}{1}",
                        /* 0 */ R.StartValue,
                        /* 1 */ aSeparator
                    );
                else
                    Result.AppendFormat
                    (
                        "{0}{2}{1}{3}",
                        /* 0 */ R.StartValue,
                        /* 1 */ R.StartValue + R.Count - 1,
                        /* 2 */ aJoint,
                        /* 3 */ aSeparator
                    );
            }

            //   Удаляем побыстрому последний разделитель. 
            //   При условии, что хотя бы один элемент был вставлен. 
            if (ResultSet.Count() > 0)
                Result.Length -= aSeparator.Length;


            return Result.ToString();
        }



        struct NumberRecord
        {
            public int OrderedIndex;
            public int Data;
            public int Difference;
        }

        struct NumberRange
        {
            public int DifferenceKey;
            public int StartValue;
            public int Count;
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

