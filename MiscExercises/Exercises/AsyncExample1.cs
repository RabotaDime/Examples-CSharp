using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using My.Utils;

using Console = My.Utils.CustomConsole.Console;



namespace Misc.Exercises
{
    class CAsyncExample1
    {
        public static string Name = "Простой пример асинхронной работы";

        public static void Info ()
        {
            Console.Write
            (
                " Задача: " + 
                "Определить конечный вывод в разных ситуациях "
            );
        }



        private static string _Result;

        public static void Execute ()
        {
            //  В данном примере я намеренно вызываю асинхронную функцию
            //  без ключевого слова await. Поэтому предупреждение нужно выключить,
            //  чтобы оно не смешивалось с настоящими ошибками и предупреждениями
            //  всего проекта, и не отвлекало тем самым от реальных проблем. 
            _Result = string.Empty;
            #pragma warning disable CS4014
            SaySomething1();
            #pragma warning restore CS4014
            Console.WriteLine("Проверка № 1", ConsoleColor.DarkGreen, "main thread");
            Console.WriteLine(_Result, ConsoleColor.DarkGreen, "main thread");

            Console.WriteLine(SaySomething1().Result, ConsoleColor.DarkGreen, "main thread");
            Console.WriteLine(_Result, ConsoleColor.DarkGreen, "main thread");

            _Result = string.Empty;
            Console.WriteLine(SaySomething2(), ConsoleColor.DarkGreen, "main thread");
            Console.WriteLine(_Result, ConsoleColor.DarkGreen, "main thread");
        }

        static async Task<string> SaySomething1 ()
        {
            await Task.Delay(5);
            _Result = "Привет, мир! Случай № 1";
            Console.WriteLine("Другой привет", ConsoleColor.Yellow, "second thread");
            return "Проверка № 2";
        }

        static string SaySomething2 ()
        {
            Thread.Sleep(5);
            _Result = "Привет, мир! Случай № 2";
            Console.WriteLine("Еще привет привет", ConsoleColor.DarkGreen, "main thread");
            return "Проверка № 3";
        }
    }
}
