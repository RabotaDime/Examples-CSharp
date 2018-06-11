using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My.Utils;

namespace IOChain
{
    public interface IIOQuery
    {
        bool Success { get; set; }
    }

    public interface IIONumberQuery : IIOQuery
    {
        int Number { get; set; }
    }

    public interface IIOStringQuery : IIOQuery
    {
        string Data { get; set; } 
    }

    public interface IIOEOLQuery : IIOQuery
    {
        string EOL { get; set; }
    }

    //public interface IIOSingleMarkerQuery : IIOQuery
    //{
    //    string Marker { get; set; }
    //}

    public interface IIOMultiMarkerQuery : IIOQuery
    {
        string[] MarkerSet { get; set; }
        string MatchedMarker { get; set; }
    }



    public interface IInputOutput
    {
        bool In (IIOQuery q1, params IIOQuery[] query);
        bool NotIn (IIOQuery q1, params IIOQuery[] query);
    }

    public struct QNumber : IIONumberQuery
    {
        public bool Success { get; set; }
        public int Number { get; set; }
    }

    public struct QEOL : IIOEOLQuery
    {
        public bool Success { get; set; }
        public string EOL { get; set; }
    }

    public struct QString : IIOStringQuery
    {
        public bool Success { get; set; }
        public string Data { get; set; }
    }

    public struct QMultiMarker : IIOMultiMarkerQuery
    {
        public bool Success { get; set; }
        public string[] MarkerSet { get; set; }
        public string MatchedMarker { get; set; }
    }



    public class CIOBufferedReader // : TextReader
    {
        char[] DataBuffer;
        int Position;
        int ContentSize;

        TextReader BaseReader;

        public CIOBufferedReader (TextReader base_reader, int max_buffer_size = 1024 / 4)
        {
            BaseReader = base_reader;
            if (base_reader == null) throw new ArgumentNullException("Переданный чтец равен нулю");

            DataBuffer = new char[max_buffer_size];
            Position = 0;
            ContentSize = 0;
        }

        //   Конструктор для чтеца по умолчанию, взятого из консоли. 
        public CIOBufferedReader (int max_buffer_size = 1024 / 4) : this (Console.In, max_buffer_size)
        {
        }

        public bool Peek (out char return_char, int depth = 0)
        {
            if (depth >= DataBuffer.Length)
            {
                throw new ArgumentOutOfRangeException("Запрашиваемая глубина обзора для буферизированного потока превысила максимум");
            }

            //  Инлайн-переменная, улучшающая читаемость кода.
            //  С ее помощью будет понятно, что означает функция (ContentSize - 1)
            int data_last_index;

            //  Пытаюсь наполнить буфер данными до тех пор, пока не дойду
            //  до обозначенной в просмотре глубины. 
            while (depth > (data_last_index = ContentSize - 1))
            {
                int char_value = BaseReader.Read();
                if ((char_value >= char.MinValue) && (char_value <= char.MaxValue))
                {
                    DataBuffer[Position] = Convert.ToChar(char_value);
                    Position++;
                    ContentSize++;
                }
                else
                    //  Операция просмотра буфера невозможна из-за нехватки данных
                    //  во внутреннем потоке. 
                    goto PeekFailed;
            }

            //  Если возможно просмотреть буфер на данную глубину. 
            if (depth <= (data_last_index = ContentSize - 1))
            {
                return_char = DataBuffer[depth];
                return true;
            }

            PeekFailed:

            return_char = char.MinValue;
            return false;
        }

        public void Pop ()
        {
            Position = 0;
            ContentSize = 0;
        }

        public void Pop (int count)
        {
            if (count > ContentSize)
                throw new ArgumentOutOfRangeException("В буфере меньше данных, чем нужно для попытки сброса такого количества");

            Array.Copy(DataBuffer, count, DataBuffer, 0, ContentSize - count);

            ContentSize -= count;
            Position = ContentSize;
        }

        public int Length
        {
            get { return DataBuffer.Length; }
        }
    }



    public class CIO : IInputOutput
    {
        CIOBufferedReader Buffer;



        public CIO (int peek_depth = 1024 / 4)
        {
            Buffer = new CIOBufferedReader(peek_depth);
        }



        public bool In (IIOQuery q1, params IIOQuery[] query)
        {
            foreach (IIOQuery q in query)
            {
                if (! q.Success) return false;
            }

            return true;
        }

        public bool NotIn (IIOQuery q1, params IIOQuery[] query)
        {
            return !In(q1, query);
        }

        public bool Out (IIOQuery q1, params IIOQuery[] query)
        {
            foreach (IIOQuery q in query)
            {
                if (! q.Success) return false;
            }

            return true;
        }

        public bool NotOut (IIOQuery q1, params IIOQuery[] query)
        {
            return !Out(q1, query);
        }



        public void Pop (int count)
        {
            Buffer.Pop(count);
        }



        //public bool BufferedRead (out char result_char)
        //{
        //    //   Если в буфере пусто, пытаемся считать и сохранить в него новые данные. 
        //    if (PeekBufferSize <= 0)
        //    {
        //        int char_value = Console.Read();
        //        if ((char_value >= char.MinValue) && (char_value <= char.MaxValue))
        //        {
        //            PeekBufferSize = 1;
        //            PeekBuffer = Convert.ToChar(char_value);
        //        }
        //        else
        //        {
        //            //  Ошибка чтения потока данных. Функция ниже вернет false.
        //            //  Если необходимо, то внутренний поток сам вызовет исключение,
        //            //  либо логика обработает отрицательный результат, поэтому 
        //            //  здесь дополнительное исключение не нужно.
        //        }
        //    }

        //    //   Если в буфере появился или был символ, то все время отдаем его
        //    //   до тех пор, пока он не будет сброшен логикой, через 
        //    //   метод BufferFlush. Сброс означает "принятие" символа в обработку,
        //    //   после чего он больше не нужен.  
        //    if (PeekBufferSize > 0)
        //    {
        //        result_char = PeekBuffer;
        //        return true;
        //    }
        //    else
        //    {
        //        result_char = char.MinValue;
        //        return false; 
        //    }
        //}

        //public void BufferFlush ()
        //{
        //    PeekBuffer = char.MinValue;
        //    PeekBufferSize = 0;
        //}



        //  Out : Number(int)



        public IIONumberQuery Number (int value)
        {
            Console.Write(value);
            return new QNumber { Number = value, Success = true };
        }

        public char NumberFormat_DecimalPoint1 = '.';
        public char NumberFormat_DecimalPoint2 = ',';
        public readonly string[] NumberFormat_Cultures = { "en-US", "ru-RU" };

        //  In : Number(int) 
        public IIONumberQuery Number (out int return_number)
        {
            var text = new StringBuilder (64);

            while (Buffer.Peek(out char c))
            {
                if
                (
                    char.IsNumber(c) ||
                    (c == NumberFormat_DecimalPoint1) ||
                    (c == NumberFormat_DecimalPoint2) ||
                    (c == '+') ||
                    (c == '-')
                )
                {
                    Buffer.Pop();
                    text.Append(c);
                }
                else
                    break;
            }

            var result = new QNumber
            {
                Success = int.TryParse(text.ToString(), out return_number),
                Number = return_number,
            };

            //  Альтернативная попытка
            if (!result.Success)
            {
                foreach (string culture_id in NumberFormat_Cultures)
                {
                    NumberStyles n_style =
                        NumberStyles.AllowDecimalPoint |
                        NumberStyles.AllowLeadingSign;

                    IFormatProvider n_fmt = CultureInfo.GetCultureInfo(culture_id);

                    result.Success   = double.TryParse(text.ToString(), n_style, n_fmt, out double real_number);
                    result.Number    = Convert.ToInt32(real_number);

                    if (result.Success)
                    {
                        return_number = result.Number;
                        return result;
                    }
                }
            }

            return result;
        }



        //   Out : String
        public IIOStringQuery Text (string value)
        {
            Console.Write(value);
            return new QString { Data = value, Success = true };
        }

        public IIOStringQuery TextLine (string value)
        {
            Console.WriteLine(value);
            return new QString { Data = value, Success = true };
        }

        //   In : String
        public IIOStringQuery Text (int length, out string value)
        {
            var text = new StringBuilder(length);

            while ((length > 0) && Buffer.Peek(out char c))
            {
                Buffer.Pop();
                text.Append(c);
                length--;
            }

            value = text.ToString();

            return new QString
            {
                Success   = (length > 0),
                Data      = value,
            };
        }

        public IIOStringQuery TextLine (out string value)
        {
            var text = new StringBuilder();
            bool valid_terminator = false;

            while (Buffer.Peek(out char c))
            {
                switch (c) {
                    case '\r':   continue;
                    case '\n':   valid_terminator = true; break;
                    default:     text.Append(c); break;
                }

                Buffer.Pop();
            }

            value = text.ToString();

            return new QString
            {
                Success   = valid_terminator,
                Data      = value,
            };
        }

        //   In : One or more markers. Один или несколько маркеров. 
        //   Метод только для входящих данных, так как для вывода нелогично выводить
        //   одновременно несколько серий данных. 
        public IIOMultiMarkerQuery OneOf (string m1, params string[] markers)
        {
            var matcher = new CStringMatcher (m1, markers);

            //   До тех пор, пока в буфере есть входящие данные,
            //   и пока совпадение строк имеет "выжившие" маркеры,
            //   мы продолжаем анализ. 
            while (Buffer.Peek(out char c) && (matcher[c] > 0))
            {
                matcher.Continue(c);

                //   Если есть совпавшие маркеры. 
                if (matcher.MatchCount > 0)
                {
                    //   Буфер съедает совпавший маркер и переходит дальше. 
                    Buffer.Pop(matcher.FirstMatch.Length);

                    return new QMultiMarker
                    {
                        MatchedMarker = matcher.FirstMatch,
                        MarkerSet = new string[] { matcher.FirstMatch },
                        Success = true,
                    };
                }
            }

            return new QMultiMarker
            {
                MatchedMarker = string.Empty,
                MarkerSet = new string[] { },
                Success = false,
            };
        }

        //   Более глубокая версия функции OneOf. Возвращает все маркеры, 
        //   которые совпали, чтобы можно было произвести анализ. 
        public IIOMultiMarkerQuery OneOfDeep (string m1, params string[] markers)
        {
            var matcher = new CStringMatcher (m1, markers);

            //   До тех пор, пока в буфере есть входящие данные,
            //   и пока совпадение строк имеет "выжившие" маркеры,
            //   мы продолжаем анализ. 
            while (Buffer.Peek(out char c) && (matcher[c] > 0))
            {
                matcher.Continue(c);
            }

            return new QMultiMarker
            {
                MatchedMarker = matcher.FirstMatch,
                MarkerSet = matcher.MatchResult.ToArray(),
                Success = (matcher.MatchCount > 0),
            };
        }



        public IIOEOLQuery OutEOL ()
        {
            Console.WriteLine();
            return new QEOL { EOL = Environment.NewLine, Success = true };
        }

        public IIOEOLQuery OutEOL (string eol_value)
        {
            Console.Write(eol_value);
            return new QEOL { EOL = Environment.NewLine, Success = true };
        }

        public IIOEOLQuery InEOL ()
        {
            int pos = 0;
            bool success = true;

            while ((pos < Environment.NewLine.Length) && Buffer.Peek(out char c))
            {
                if (c != Environment.NewLine[pos])
                {
                    success = false;
                    break;
                }

                Buffer.Pop();
                pos++;
            }

            return new QEOL { EOL = Environment.NewLine, Success = success };
        }
    }
}




