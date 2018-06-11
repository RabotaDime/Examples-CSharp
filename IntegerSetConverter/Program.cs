using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegerSetConverter
{
    //                                                                                            
    //   Задача обработчика связанного набора данных сводится к двум подзадачам:                  
    //                                                                                            
    //     А) Связывание набора элементов (например, объединение набора строк в одну строку       
    //        разделенную запятыми, как это происходит в стандартных функциях String.Join         
    //        во многих языках и средах.                                                          
    //                                                                                            
    //     Б) Обработка цепочек данных, которые могут быть связаны между собой определенными      
    //        условиями.                                                                          
    //____________________________________________________________________________________________
    //                                                                                            
    //                                                                                            
    class CProgram
    {
        static void Main(String[] args)
        {
            //Console.Clear();

            var CSet = new Dictionary<int,INumericConverter>
            {
                { 0, new RangeIteratorConverter             { } },

                //   Преобразователь на основе двух простых функций. 
                { 1, new SingleFunctionConverter            { } },

                //   Преобразователь, работающий (по возможности) на основе обычных C# методов. 
                { 2, new CSharpEnvironmentConverter         { } },

                //   Простейший текстовый парсер. 
                { 5, new BasicTextParserConverter           { } },
            };

            int CKey = 1;

        AskNumber:

            Console.WriteLine("Список преобразователей:");
            foreach (var C in CSet)
            {
                Console.WriteLine(String.Format("  {0}) {1}", C.Key, C.Value.ConverterName));
            }
            
            Console.Write("Пожалуйста, введите номер интересующего преобразователя: ");

            if
            (
                //   Пытаемся распознать введенный номер. 
                //   Если результат безуспешный, спрашиваем снова. 
                (! int.TryParse(Console.ReadLine(), out CKey)) ||
                //   Или если номер введен верно, но такого номера нет в наборе, 
                //   то тоже спрашиваем снова. 
                (! CSet.ContainsKey(CKey))
            )
                goto AskNumber;


            //   Тест преобразования числового набора в строку. 
            CTests.IntegerToString(CSet[CKey]);

            //   Тест преобразования строки в числовой набор. 
            CTests.StringToInteger(CSet[CKey]);


            /*  TODO: Пытался предотвратить передачу клавиш в VS после завершения программы,
                но пока не придумал быстрого решения. Вспомни, как у тебя было в другом проекте. 
            */
            //System.Threading.Thread.Sleep(1000);
        }



        class CTests
        {
            public static void IntegerToString (INumericConverter aConverter)
            {
                //                                                                                    
                //   Создание наборов данных. 
                //____________________________________________________________________________________
                int[] Set0 = new int[] { };
                int[] Set1 = new int[] { 1, 3, 4, 5, 7, 9 };
                int[] Set2 = new int[] { 1, 2, 3, 4, 5 };
                int[] Set3 = new int[] { 2, 1, 3 };
                int[] Set4 = new int[] { 1, 2, 3, 3, 4, 5 };

                var Sets = new Dictionary<string, int[]> ()
                {
                    { "Пустой набор"                                    , Set0 },
                    { "Тестовый набор № 1"                              , Set1 },
                    { "Полностью последовательный набор"                , Set2 },
                    { "Ошибочный набор № 1"                             , Set3 },
                    { "Ошибочный набор № 2"                             , Set4 },
                };

                Random R = new Random();

                for (int N = 1; N <= 0; N++)
                {
                    Sets.Add
                    (
                        String.Format("Большой случайный набор № {0}", N),
                        CreateRandomSortedList(R.Next(1, 1024), 1, 9999, R).ToArray()
                    );
                }


                ///                                                                                    
                ///   Обработка наборов. 
                ///____________________________________________________________________________________
                foreach (var S in Sets)
                {
                    CTests.Begin(S.Key);
                    CTests.IntroduceConverterMethod(aConverter, "int[] -> string");


                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine(String.Format
                        (
                            "Set({0}) = [{1}]",
                            /* 0 */ S.Value.Length,
                            /* 1 */ String.Join(", ", S.Value)
                        ));
                        Console.ResetColor();


                        Console.Write("Result(int[] -> string) = [");
                        try
                        {
                            Console.WriteLine(String.Format("{0}]", aConverter.Convert(S.Value, ",", "..")));
                        }
                        catch (Exception E)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write("Error");
                            Console.ResetColor();
                            Console.WriteLine("]");

                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine(E);
                            Console.ResetColor();
                        }
                    
                    CTests.End();
                }
            }



            public static void StringToInteger (INumericConverter aConverter)
            {
                ///                                                                                    
                ///   Создание наборов данных.                                                         
                ///____________________________________________________________________________________
                string Set0 = "";
                string Set1 = "1,3..5,7,9";
                string Set2 = "1..1000";
                string Set3 = "  1  \t,  3..5, 7, 9";
                string Set4 = "9..3";
                string Set5 = "1, a, 4-9";
                string Set6 = "1..213";
                string Set7 = "  1  \t,  3..5,3..5, 7, 9";

                var Sets = new Dictionary<string, string> ()
                {
                    { "Пустой набор"                                    , Set0 },
                    { "Тестовый набор № 1"                              , Set6 },
                    { "Тестовый набор № 2"                              , Set1 },
                    { "Полностью последовательный набор"                , Set2 },
                    { "Правильный набор с разрешенными пробелами"       , Set3 },
                    { "Ошибочный набор № 1"                             , Set4 },
                    { "Ошибочный набор № 2"                             , Set5 },
                    { "Ошибочный набор № 3 (дублирующиеся данные)"      , Set7 },
                };



                ///                                                                                    
                ///   Обработка наборов.                                                               
                ///____________________________________________________________________________________
                foreach (var S in Sets)
                {
                    CTests.Begin(S.Key);
                    CTests.IntroduceConverterMethod(aConverter, "string -> int[]");


                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine(String.Format
                        (
                            "Set() = [{0}]",
                            /* 0 */ S.Value
                        ));
                        Console.ResetColor();


                        Console.Write("Result(string -> int[");
                        try
                        {
                            int[] Result = aConverter.Convert(S.Value, ",", "..");

                            Console.WriteLine(String.Format
                            (
                                "{0}]) = [{1}]",
                                /* 0 */ Result.Length,
                                /* 1 */ String.Join(", ", Result)
                            ));
                        }
                        catch (Exception E)
                        {
                            Console.Write("]) = [");
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write("Error");
                            Console.ResetColor();
                            Console.WriteLine("]");

                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine(E);
                            Console.ResetColor();
                        }
                    
                    CTests.End();
                }
            }



            public static void Begin (string aName, string aDescription = "")
            {
                Console.Clear();

                Console.WriteLine(String.Format
                (
                    "{0} {1} {0}",
                    /* 0 */ "-----",
                    /* 1 */ aDescription.Length > 0 ? aName + " : " + aDescription : aName
                ));
            }

            public static void IntroduceConverterMethod (INumericConverter aConverter, string aMethod)
            {
                Console.Write("Converter=");
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(String.Format("{0}", aConverter != null ? aConverter.ConverterName : "INVALID_CONVERTER"));
                Console.ResetColor();

                Console.Write(", Method=( ");
                Console.Write(aMethod);
                Console.WriteLine(" )");
            }

            public static void End ()
            {
                Console.WriteLine(String.Empty);

                Console.WriteLine("Пожалуйста, нажмите почти любую кнопку для продолжения...");
                //Console.ReadKey(false);

                while (! Console.KeyAvailable) {
                    System.Threading.Thread.Sleep(100);
                }
                ConsoleKey K = Console.ReadKey(true).Key;
            
                if (K == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }
            }
        }



        static List<int> CreateRandomSortedList (int aCount = 1024, int aMinimum = 1, int aMaximum = int.MaxValue, Random aR = null)
        {
            if (aCount <= 0) throw new ArgumentException("The number of elements can't be zero or less.", "Count");

            aMinimum = Math.Min(aMinimum, aMaximum);
            aMaximum = Math.Max(aMaximum, aMinimum);


            if (aR == null) aR = new Random();

            List<int> Result = new List<int> (aCount);

            while (aCount > 0)
            {
                ///   Создаю набор случайных данных в нужном количестве. Шанс на создание последовательности 50/50. 
                int Value       = aR.Next(aMinimum, aMaximum + 1);
                int Sequence    = 1 + ( (aR.Next(0, 100) >= 50) ? Math.Min(1 + aR.Next(0, aCount), aMaximum + 1 - Value) : 0 );

                for (int I = 0, V = Value; I < Sequence; I++, Value++)
                    Result.Add(Value);

                aCount -= Sequence;
            }

            Result.Sort();

            return Result.Distinct().ToList();
        }
    }
}
