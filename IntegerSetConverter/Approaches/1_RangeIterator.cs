using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



namespace IntegerSetConverter
{
    class RangeIteratorConverter : INumericConverter
    {
        string INumericConverter.ConverterName
        {
            get { return "RangeIteratorConverter"; }
        }



        static IEnumerable<Range> SequenceIterator
        (
            string aInputText,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        )
        {
            if (aInputText.Length <= 0) yield break;


            var Separators      = new string[] { aSeparator };
            var Joints          = new string[] { aJoint };
            var AllowedChars    = new char[] { ' ' };


            var NumberGroup = new Range (0, 0);

            foreach (string SequencePart in aInputText.Split(Separators, StringSplitOptions.None))
            {
                var RangeParts = SequencePart.Split(Joints, StringSplitOptions.None);
                string MinPart, MaxPart;

                if (RangeParts.Length >= 3) goto Error;

                if (RangeParts.Length > 1)
                {
                    MinPart = RangeParts[0].Trim(AllowedChars);
                    MaxPart = RangeParts[1].Trim(AllowedChars);
                }
                else
                {
                    MinPart = RangeParts[0].Trim(AllowedChars);
                    MaxPart = MinPart;
                }

                if (! int.TryParse(MinPart, out int MinValue)) goto Error;
                if (! int.TryParse(MaxPart, out int MaxValue)) goto Error;

                NumberGroup.Reset(MinValue, MaxValue);

                yield return NumberGroup;
            }

            yield break;

        Error:

            throw new ArgumentException
            (
                $"The input set contains invalid data. " +
                $"Set value in invalid format.",
                "InputText"
            );
        }



        static IEnumerable<Range> SequenceIterator (int[] aInputList)
        {
            if (aInputList.Length <= 0) yield break;


            var NumberGroup = new Range (0, 0);

            // {Reset, Push}
            NumberGroup.Reset(aInputList[0]);

            for (int I = 1, Current; I < aInputList.Length; I++)
            {
                Current = aInputList[I];

                while (Current == NumberGroup.Maximum + 1)
                {
                    // {Push}
                    NumberGroup.Maximum = Current;

                    I++;
                    if (I == aInputList.Length)
                    {
                        yield return NumberGroup;
                        yield break;
                    }

                    Current = aInputList[I];
                }

                yield return NumberGroup;

                // {Reset, Push}
                NumberGroup.Reset(Current);
            }

            yield return NumberGroup;
        }



        public static string Convert
        (
            int[] aData,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        )
        {
            StringBuilder ResultText = new StringBuilder();

            string Separator = string.Empty;
            string NormalSeparator = aSeparator;

            var Previous = new Range(int.MinValue, int.MinValue);

            foreach (Range R in SequenceIterator(aData))
            {
                if (R.Minimum > Previous.Maximum)
                {
                    ResultText.Append(Separator);
                    ResultText.Append(R.AsString(aJoint));
                    Separator = NormalSeparator;
                }
                else
                {
                    throw new ArgumentException
                    (
                        $"The input set contains invalid data. " +
                        $"The value is less or equal to previous element.",
                        "Data"
                    );
                }

                Previous = R;
            }

            return ResultText.ToString();
        }



        public static int[] Convert
        (
            string aData,
            string aSeparator   = CConvertOptions.DefaultSeparator,
            string aJoint       = CConvertOptions.DefaultJoint
        )
        {
            var ResultList = new List<int>();


            var Previous = new Range(int.MinValue, int.MinValue);

            foreach (Range R in SequenceIterator(aData, aSeparator, aJoint))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"\n{{{R}}}");
                Console.ResetColor();

                if (R.Minimum > Previous.Maximum)
                {
                    if (R.Length > 1)
                        ResultList.AddRange(Enumerable.Range(R.Minimum, R.Length));
                    else
                        ResultList.Add(R.Minimum);
                }
                else
                {
                    throw new ArgumentException
                    (
                        $"The input set contains invalid data. " +
                        $"The value is less or equal to previous element.",
                        "Data"
                    );
                }

                Previous = R;
            }

            Console.WriteLine();


            return ResultList.ToArray();
        }



        //[StructLayout(LayoutKind.Explicit)]
        struct Range
        {
            int _Minimum;
            int _Maximum;
            int _Length;

            public int Minimum
            {
                get { return _Minimum; }
                set
                {
                    _Minimum = value;

                    if (_Minimum > _Maximum)
                    {
                        throw new InvalidOperationException
                        (
                            $"Minimum value {_Minimum} exceeds range maximum {_Maximum}."
                        );
                    }

                    _Length = _Maximum - _Minimum + 1;
                }
            }

            public int Maximum
            {
                get { return _Maximum; }
                set
                {
                    _Maximum = value;
                    if (_Maximum < _Minimum)
                    {
                        throw new InvalidOperationException
                        (
                            $"Maximum value {_Maximum} conflicts with range minimum {_Maximum}."
                        );
                    }

                    _Length = _Maximum - _Minimum + 1;
                }
            }

            //  Некрасивая компенсация отсутствия union-типов в C#, 
            //  без использования unsafe/fixed описаний. 
            public int this [int RangePartIndex]
            {
                get
                {
                    switch (RangePartIndex)
                    {
                        case 0: return _Minimum;
                        case 1: return _Maximum;
                        default: throw new ArgumentException("Range doesn't have more than two elements.", "Index");
                    }
                }

                set
                {
                    switch (RangePartIndex)
                    {
                        case 0: this._Minimum = value; break;
                        case 1: this._Maximum = value; break;
                        default: throw new ArgumentException("Range doesn't have more than two elements.", "Index");
                    }

                    this._Length = this._Maximum - this._Minimum + 1;
                }
            }

            public int Length
            {
                get { return _Maximum - _Minimum + 1; }
            }

            public Range (int aFrom, int aTo)
            {
                this._Minimum = aFrom;
                this._Maximum = aTo;
                this._Length  = _Maximum - _Minimum + 1;

                if (this._Minimum > this._Maximum)
                    throw new ArgumentException($"Invalid range from {aFrom} to {aTo}.", "From, To");
            }

            public void Reset (int aFrom)
            {
                this._Minimum = aFrom;
                this._Maximum = aFrom;
                this._Length = 1;
            }

            public void Reset (int aFrom, int aTo)
            {
                this._Minimum = aFrom;
                this._Maximum = aTo;
                this._Length  = _Maximum - _Minimum + 1;

                if (this._Minimum > this._Maximum)
                    throw new ArgumentException($"Invalid range from {aFrom} to {aTo}.", "From, To");
            }

            public string AsString (string aJoint)
            {
                if (this._Maximum == this._Minimum)
                    return $"{_Minimum}";
                else
                    return $"{_Minimum}{aJoint}{_Maximum}";
            }

            public override string ToString ()
            {
                return $"{_Minimum}{CConvertOptions.DefaultJoint}{_Maximum}";
            }

        /*
            [FieldOffset(0)] int _Minimum;
            [FieldOffset(4)] int _Maximum;
            [FieldOffset(8)] int _Length;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            [FieldOffset(0)] readonly int[] _Values = new int [2];
        */
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
