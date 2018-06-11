
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IntegerSetConverter
{
    class BasicTextParserConverter : INumericConverter, INumericConverterEx
    {
        string INumericConverter.ConverterName
        {
            get { return "BasicTextParserConverter"; }
        }



        private enum TextLiteralType
        {
            Undefined           = 0,

            Number              = 10,

            Whitespace          = 20,

            Separator           = 30,
            Joint               = 31,

            Error               = 100,
            Uncertain           = 101,
        }



        private class Literal
        {
            public TextLiteralType     MatchType;
            public Boolean             MatchSuccess;
            public Int32               MatchCount;
            public Int32               MatchNumber;

            public Literal (TextLiteralType aMatchType, Int32 aMatchCount, Boolean aMatchResult)
            {
                this.MatchType      = aMatchType;
                this.MatchSuccess   = aMatchResult;
                this.MatchCount     = aMatchCount;
                this.MatchNumber    = 0;
            }

            public ConsoleColor GetColorFromType ()
            {
                switch (this.MatchType)
                {
                    case TextLiteralType.Number     : return ConsoleColor.Cyan;
                    case TextLiteralType.Whitespace : return ConsoleColor.White;
                    case TextLiteralType.Separator  : return ConsoleColor.Yellow;
                    case TextLiteralType.Joint      : return ConsoleColor.DarkYellow;
                    case TextLiteralType.Uncertain  : return ConsoleColor.DarkGray;
                    default                         : return ConsoleColor.DarkRed;
                }
            }

            public static Literal InvalidLiteral
            {
                get
                {
                    return new Literal
                    (
                        TextLiteralType.Undefined,
                        0,
                        false
                    );
                }
            }

            public bool IsWhiteSpace ()
            {
                return (this.MatchType == TextLiteralType.Whitespace);
            }

            public override string ToString ()
            {
                return String.Format("[{0}:{1}]", this.MatchType, this.MatchCount);
            }
        }



        private static TextLiteralType PeekSymbol (Char aC, Int32 aMaskDepth, CConvertOptions aPCO)
        {
            switch (aC)
            {
                //   Цифры. 
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                {
                    return TextLiteralType.Number;
                }
                //break;

                //   Пробелы и табуляция. 
                case ' ':
                case '\t':
                {
                    return TextLiteralType.Whitespace;
                }
                //break;

                //   Иные символы для уточнения. 
                default:
                {
                }
                break;
            }


            if ((aMaskDepth < aPCO.SeparatorMask.Length) && (aC == aPCO.SeparatorMask[aMaskDepth]))
                return TextLiteralType.Separator;


            if ((aMaskDepth < aPCO.JointMask.Length) && (aC == aPCO.JointMask[aMaskDepth]))
                return TextLiteralType.Joint;


            return TextLiteralType.Error;
        }



        private static Literal PeekLiteral
        (
            String          aData,
            Int32           aStartIndex,
            CConvertOptions  aPCO
        )
        {
            if (aData.Length <= 0)              throw new ArgumentException("Empty input text", "Data");
            if (aStartIndex >= aData.Length)    throw new ArgumentException("Index is out of range.", "StartIndex");


            Literal Result = Literal.InvalidLiteral;

            Int32 PeekI = aStartIndex;
            Int32 MaskI = 0;

            TextLiteralType Mode = PeekSymbol(aData[PeekI], MaskI, aPCO);
            Result.MatchType = Mode;
            Result.MatchCount++;
            PeekI++;

            while ((PeekI < aData.Length) && (PeekI < aStartIndex + aPCO.MaxElementLength))
            {
                Char               C = aData[PeekI];
                TextLiteralType    NewMode = PeekSymbol(C, MaskI, aPCO);

                if (Mode != NewMode)
                {
                    break;
                }

                Result.MatchCount++;
                PeekI++;
                MaskI++;
            }

            string NumberData = aData.Substring(aStartIndex, Result.MatchCount);
            int Number;
            Result.MatchNumber = int.TryParse(NumberData, out Number) ? Number : 0;

            return Result;
        }



        private static void ParseElement (String aData, ref int rParseIndex, out Literal rParseResult, CConvertOptions aPCO)
        {
            rParseResult = PeekLiteral(aData, rParseIndex, aPCO);

            //   TODO: Временный отладочный вывод. 
            Console.ForegroundColor = rParseResult.GetColorFromType();
            Console.WriteLine(String.Format
            (
                "{0}:{1} = [{2}]",
                /* 0 */ rParseResult.MatchType.ToString(),
                /* 1 */ rParseResult.MatchCount,
                /* 2 */ aData.Substring(rParseIndex, rParseResult.MatchCount)
            ));
            Console.ResetColor();

            rParseIndex += rParseResult.MatchCount;
        }



        public static int[] Convert
        (
            string aData,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        )
        {
            //   TODO: Временный отладочный вывод. 
            Console.Write("\n");

            List<int> Result = new List<int> (1024);

            Literal Prev = Literal.InvalidLiteral;
            Literal Next = Literal.InvalidLiteral;

            Literal LastNumeric = Literal.InvalidLiteral;

            Literal ParseResult;
            CConvertOptions PCO = new CConvertOptions { SeparatorMask = aSeparator, JointMask = aJoint };
            

            int I = 0;
            Prev.MatchType = TextLiteralType.Separator;


            for (; I < aData.Length; )
            {
                ParseElement(aData, ref I, out ParseResult, PCO);

                if (ParseResult.IsWhiteSpace())
                {
                    //  Пробелы просто пропускаем. В любом количестве. 
                    continue;
                }

                Next = ParseResult;

                //   Если обнаруженные элементы совпадают по типу, значит 
                //   мы можем уже сказать, что в данных ошибка (например, два числа следуют
                //   друг за другом, без иных возможных разделителей). 
                if (Next.MatchType == Prev.MatchType)
                {
                    throw new ArgumentException("Invalid elements in input data (duplicate data).", "Data");
                }

                switch (Next.MatchType)
                {
                    case TextLiteralType.Number:
                    {
                        if (Prev.MatchType == TextLiteralType.Joint)
                        {
                            //   Связка перед числом => Новый диапазон значений. 

                            int From = LastNumeric.MatchNumber + 1;
                            int Count = Next.MatchNumber - LastNumeric.MatchNumber;

                            if (Count <= 0)
                            {
                                throw new ArgumentException("Invalid elements in input data (invalid range).", "Data");
                            }

                            Result.AddRange(Enumerable.Range(From, Count));
                        }
                        else if (Prev.MatchType == TextLiteralType.Separator)
                        {
                            //   Разделитель перед числом => Добавляем число. 
                            Result.Add(Next.MatchNumber);
                        }
                        else if (Prev.MatchType == TextLiteralType.Uncertain)
                        {
                            //   Первый неопределенный элемент (пустышка) = Все в порядке. 
                        }
                    }
                    break;

                    case TextLiteralType.Separator:
                    {
                        if (Prev.MatchType == TextLiteralType.Number)
                        {
                            //   Разделитель групп перед числовым литералом = Все в порядке. 
                        }
                        else
                            //   Выходим с ошибкой парсера. 
                            goto default;
                    }
                    break;

                    case TextLiteralType.Joint:
                    {
                        if (Prev.MatchType == TextLiteralType.Number)
                        {
                            //   Связка перед числовым литералом = Все в порядке. 
                        }
                        else
                            //   Выходим с ошибкой парсера. 
                            goto default;
                    }
                    break;

                    default:
                    {
                        throw new ArgumentException("Error element in input data.", "Data");
                    }
                    //break;
                }


                if (ParseResult.MatchType == TextLiteralType.Number) LastNumeric = ParseResult;

                Prev = Next;
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
            int ErrorCount = 0;
            StringBuilder Result = new StringBuilder(1024 * 4);


            if (aData.Length <= 0)
            {
                return String.Empty;
            }


            //   Б: Память для обработки последовательностей. 
            int? SequenceBuffer = null;


            //   A: Выводим первый элемент связки. 
            //   Б: Это автоматическое начало новой последовательности. 
            Result.Append(aData[0]);
            SequenceBuffer = null;


            //   А: Выводим набор последующих элементов, перебирая каждую связку. 
            //      Так как обращение к элементам (отличным от массива и набора чисел) 
            //      может требовать накладных ресурсов, используем дополнительный буфер 
            //      Prev для сохранения предыдущего элемента в наборе. 
            //      
            //      [D(1)] \ 
            //              [Связка J] 
            //      [D(2)] / 
            //      
            //       Вариант цикла без учета накладных ресурсов при обращении к данным:
            //          for (int J = 1; J < aData.Length; J++) {
            //              int Prev = aData[J - 1];
            //              int Next = aData[J + 0];
            //              ...
            //          }
            for (int J = 1, Prev = aData[0]; J < aData.Length; J++)
            {
                int Next = aData[J];

                //   Б: Если соблюдается первое условие между двумя элементами 
                if (Prev < Next)
                {
                    //   Б: Если предыдущий элемент можно связать с последующим, 
                    //      значит мы сохраняем данные об этой последовательности 
                    //      и в данный момент не выводим строку. 
                    if (Prev + 1 == Next)
                    {
                        SequenceBuffer = Next;
                    }
                    //   Б: Иначе, если последующий элемент не связан условием с предыдущим 
                    else
                    {
                        //   Б: Выводим сохраненную информацию о прошедшей найденной 
                        //      последовательности. 
                        if (SequenceBuffer != null)
                        {
                            Result.AppendFormat("{0}{1}", /* 0 */ aJoint, /* 1 */ SequenceBuffer);
                            SequenceBuffer = null;
                        }

                        //   А: Выводим последующий, не связанный с предыдущим элемент. 
                        Result.AppendFormat("{0}{1}", /* 0 */ aSeparator, /* 1 */ Next);
                    }
                }
                else
                {
                    ErrorCount++;
                }

                //   А: Устанавливаем предыдущий элемент для обхода связок. 
                Prev = Next;
            }


            //   Б: Сброс буфера и вывод сохраненной информации о последней найденной 
            //      последовательности. 
            if (SequenceBuffer != null)
            {
                Result.AppendFormat("{0}{1}", /* 0 */ aJoint, /* 1 */ SequenceBuffer);
                SequenceBuffer = null;
            }


            if (ErrorCount <= 0)
                return Result.ToString();
            else
                throw new ArgumentException("The input set contains invalid data.", "Data");
        }



        string INumericConverter.Convert (int[] aData, string aSeparator, string aJoint)
        {
            return Convert(aData, aSeparator, aJoint);
        }

        int[] INumericConverter.Convert (string aData, string aSeparator, string aJoint)
        {
            return Convert(aData, aSeparator, aJoint);
        }

        string INumericConverterEx.Convert(int[] aData, CConvertOptions aOptions)
        {
            return Convert(aData, aOptions.SeparatorMask, aOptions.JointMask);
        }

        int[] INumericConverterEx.Convert(string aData, CConvertOptions aOptions)
        {
            return Convert(aData, aOptions.SeparatorMask, aOptions.JointMask);
        }
    }
}
