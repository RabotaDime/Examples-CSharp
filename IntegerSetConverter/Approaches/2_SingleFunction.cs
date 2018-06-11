using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IntegerSetConverter
{
    class SingleFunctionConverter : INumericConverter
    {
        string INumericConverter.ConverterName
        {
            get { return "SingleFunctionConverter"; }
        }



        public static string Convert
        (
            int[] aData,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        )
        {
            if (aData.Length <= 0)
            {
                return String.Empty;
            }


            //   Первый шаг. Обработка первого элемента. 
            int I = 0, Value;
            int Prev = aData[0];

            int SequenceCounter = 0;

            StringBuilder ResultText = new StringBuilder();

            ResultText.AppendFormat("{0}", Prev);


            //===  Инлайн функция для повторяющегося кода.  =======================================                                        
            Action inline_SequenceOutput = delegate ()
            {
                if (SequenceCounter > 0)
                {
                    ResultText.AppendFormat("{0}{1}", aJoint, Prev);
                    SequenceCounter = 0;
                }
            };
            //=====================================================================================


            //   Обход остальных элементов. 
            for (I = 1; I < aData.Length; I++, Prev = Value)
            {
                Value = aData[I];

                //   Проверка значений. 
                if (Value <= Prev)
                    throw new ArgumentException(String.Format("The input set contains invalid data at index [{0}]. The value is less or equal to previous element.", I), "Data");

                //   Если предыдущий элемент на один шаг следует за данным, 
                //   значит его можно записать в последовательность. 
                if (Value == Prev + 1)
                {
                    SequenceCounter++;
                }
                else
                {
                    //   Вывод данных об обнаруженной последовательности. 
                    inline_SequenceOutput();

                    //   Вывод отдельного элемента. 
                    ResultText.AppendFormat("{0}{1}", aSeparator, Value);
                }
            }

            //   Вывод данных об обнаруженной последовательности. 
            inline_SequenceOutput();


            return ResultText.ToString();
        }



        public static int[] Convert
        (
            string aData,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        )
        {
            const int DataNumericBase = 10;

            var ResultList = new List<int> ();


            if (aData.Length <= 0)
                return ResultList.ToArray();

            if (aSeparator  .Length <= 0) throw new ArgumentException("", "Separator");
            if (aJoint      .Length <= 0) throw new ArgumentException("", "Joint");
            if (! CConvertOptions.Validate(aSeparator, aJoint)) throw new ArgumentException("", "Joint, Separator");


            Char C;
            ParseMode PrevMode = ParseMode.Undefined;

            int I = -1;

            int ParseSeparatorIndex = 0;
            int ParseJointIndex     = 0;
            int ParseNumber         = 0;
            int ParseNumberIndex    = 0;
            int ParseRangeLowIndex  = 0;

            int SavedRangeStart     = 0;
            bool SavedRangeMode     = false;

            int LastValue           = int.MinValue;


            //===  Две инлайн функции для повторяющегося кода. ====================================                                     
            Action inline_Add = delegate ()
            {
                int Value = ParseNumber;

                if (Value < 0)
                {
                    throw new ArgumentException("Invalid elements in input data (invalid value).", "Data");
                }

                if (Value <= LastValue)
                {
                    throw new ArgumentException(String.Format("The input set contains invalid data at index [{0}]. The value is less or equal to previous element.", I), "Data");
                }

                ResultList.Add(Value);
                LastValue = Value;
            };
            //=====================================================================================
            Action inline_AddRange = delegate ()
            {
                int RStart = SavedRangeStart + 1;
                int RCount = ParseNumber - SavedRangeStart;

                if ( (RStart < 0) || (RCount <= 0) )
                {
                    throw new ArgumentException("Invalid elements in input data (invalid range).", "Data");
                }

                if (RStart <= LastValue)
                {
                    throw new ArgumentException(String.Format("The input set contains invalid data range from [{0}] to [{1}]. The value is less or equal to previous element.", ParseRangeLowIndex, I ), "Data");
                }

                ResultList.AddRange(Enumerable.Range(RStart, RCount));
                LastValue = RStart + RCount - 1;

                SavedRangeStart = 0;
                SavedRangeMode = false;
            };
            //=====================================================================================


            for (I = 0; I < aData.Length; I++)
            {
                C = aData[I];


                //   Выбираем режим работы. 
                ParseMode Mode = ParseMode.Error;

                if (Char.IsWhiteSpace(C))
                {
                    Mode = ParseMode.Whitespace;
                }
                else if ((ParseSeparatorIndex < aSeparator.Length) && (C == aSeparator[ParseSeparatorIndex]))
                {
                    ParseSeparatorIndex++;
                    Mode = ParseMode.Separator;
                }
                else if ((ParseJointIndex < aJoint.Length) && (C == aJoint[ParseJointIndex]))
                {
                    ParseJointIndex++;
                    Mode = ParseMode.Joint;
                }
                //   Если мы не вошли в режим разделителя или связки, 
                //   и обнаружен символ, отвечающий за цифры. 
                else if (Char.IsNumber(C)) // (ParseJointIndex == 0) && (ParseSeparatorIndex == 0) && 
                {
                    //   Повышаем разряд числа и прибавляем новую часть. 
                    ParseNumber = (ParseNumber * DataNumericBase) + (int) Char.GetNumericValue(C); 
                    ParseNumberIndex++;
                    Mode = ParseMode.Number;
                }


                if (Mode == ParseMode.Error)
                    throw new ArgumentException("Invalid input data.", "Data");


                //  Режим данных изменился = это означает поступление нового элемента
                if (PrevMode != Mode)
                {
                    if ((PrevMode == ParseMode.Joint) && (ParseJointIndex > 0))
                    {
                        if (ParseJointIndex == aJoint.Length)
                        {
                            //   Обнаружена связка двух чисел (диапазон). 

                            SavedRangeMode = true;

                            ParseJointIndex = 0;
                        }
                        else if (ParseJointIndex > aJoint.Length)
                        {
                            throw new ArgumentException(String.Format("Invalid joint sequence detected at pos [{0}]", I), "Data");
                        }
                    }
                    else if ((PrevMode == ParseMode.Separator) && (ParseSeparatorIndex > 0))
                    {
                        if (ParseSeparatorIndex == aSeparator.Length)
                        {
                            //   Обнаружен разделитель между элементами. 

                            ParseSeparatorIndex = 0;
                        }
                        else if (ParseSeparatorIndex > aSeparator.Length)
                        {
                            throw new ArgumentException(String.Format("Invalid separator sequence detected at pos [{0}]", I), "Data");
                        }
                    }
                    else if ((PrevMode == ParseMode.Number) && (ParseNumberIndex > 0))
                    {
                        //   Обнаружено начало или продолжение числа. 

                        if (SavedRangeMode)
                            inline_AddRange();
                        else
                            inline_Add();

                        SavedRangeStart = ParseNumber;

                        ParseNumber = 0;
                        ParseNumberIndex = 0;
                    }


                    PrevMode = Mode;
                }
            }


            if (SavedRangeMode)
                inline_AddRange();
            else
                inline_Add();


            return ResultList.ToArray();
        }



        enum ParseMode
        {
            Undefined   = 0,
            Number      = 1,
            Separator   = 2,
            Joint       = 3,
            Whitespace  = 4,
            Error       = 10,
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
