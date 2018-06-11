using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console = My.Utils.CustomConsole.Console;



namespace Misc.Exercises
{
    class CProgram
    {
        struct Exercise
        {
            public string              Name;
            public FExerciseInfo       InfoFunc;
            public FExerciseExecute    ExecFunc;
        }



        static void Main (string[] args)
        {
            int? AutoExercise = null;

            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out int number))
                {
                    AutoExercise = number;
                }
            }


            var AllExercises = new List<Exercise>
            {
                new Exercise
                {
                    Name        = CLoopULongExample.Name,
                    InfoFunc    = CLoopULongExample.Info,
                    ExecFunc    = CLoopULongExample.Execute
                },
                new Exercise
                {
                    Name        = CBitwiseOperators.Name,
                    InfoFunc    = CBitwiseOperators.Info,
                    ExecFunc    = CBitwiseOperators.Execute
                },
                new Exercise
                {
                    Name        = CLoopNumbers.Name,
                    InfoFunc    = CLoopNumbers.Info,
                    ExecFunc    = CLoopNumbers.Execute
                },
                new Exercise
                {
                    Name        = CSharpDataTypes.Name,
                    InfoFunc    = CSharpDataTypes.Info,
                    ExecFunc    = CSharpDataTypes.Execute
                },
                new Exercise
                {
                    Name        = CAsyncExample1.Name,
                    InfoFunc    = CAsyncExample1.Info,
                    ExecFunc    = CAsyncExample1.Execute
                },
            };


            Start:
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.HorizontalTitle("Выбор примера", 5, "=", "<", ">");
            Console.ResetColor();

            Console.WriteLine("Номера примеров для запуска: ");

            int n = 1;
            foreach (Exercise exe in AllExercises)
            {
                Console.Write(n);
                Console.Write(". ");
                Console.Write(exe.Name);
                Console.WriteLine();

                n++;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Horizontal("=");
            Console.ResetColor();

            int choice;
            if (AutoExercise == null)
            {
                Console.CursorTop -= 2;
                string input_number = Console.ReadLine();
                Console.CursorTop += 1;


                if (! int.TryParse(input_number, out choice))
                {
                    Console.TextColor = ConsoleColor.Red;
                    Console.WriteLine("Число введено неправильно");
                    Console.ResetColor();
                    goto Start;
                }
            }
            else
            {
                choice = AutoExercise.Value;
            }


            if ((choice < 1) || (choice > AllExercises.Count))
            {
                Console.TextColor = ConsoleColor.Red;
                Console.WriteLine("Указанное число вышло за пределы допустимого");
                Console.ResetColor();
                goto Start;
            }

            choice--;

            
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.HorizontalTitle(AllExercises[choice].Name, 5);
            Console.ResetColor();

            AllExercises[choice].ExecFunc();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Horizontal();
            Console.ResetColor();

            goto Start;
        }
    }



    internal delegate void FExerciseInfo ();
    internal delegate void FExerciseExecute ();
}                              
