using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc.Exercises
{
    class CSharpDataTypes
    {
        public static string Name = "Примитивные структуры данных C#";

        public static void Info ()
        {
            Console.Write
            (
                " Задача: " + 
                "Проверка знания примитивных структур данных языка C#."
            );
        }



        struct Forecast
        {
            public int Temperature  { get; set; }
            public int Pressure     { get; set; }
        }


        static void SetString (string V) { V = "дождливая"; }

        static void SetArray (string[] V) { V[1] = "Суббота"; }

        static void SetStructure (Forecast V) { V.Temperature = 35; }


        public static void Execute ()
        {
            string Weather = "солнечная";
            SetString(Weather);
            Console.WriteLine($"Сегодня {Weather} погода.\n");
            //   Строка будет выведена без изменений, потому что строки в C# это
            //   константные ссылки на созданный массив символов, а передача в аргумент
            //   функции (без использования ref/out) не может поменять внешнюю
            //   переменную. 


            string[] RainyDays = new [] { "Понедельник", "Пятница" };
            SetArray(RainyDays);
            Console.WriteLine($"Дождливые дни: {RainyDays[0]}, {RainyDays[1]}.\n");
            //   Массив передает в функцию ссылку на себя. Саму ссылку (без ref/out)
            //   поменять нельзя, но менять элементы самого массива допустимо. 
            //   Функция успешно изменит один из элементов массива. 


            var Forecast = new Forecast { Pressure = 700, Temperature = 20 };
            SetStructure(Forecast);
            Console.WriteLine($"Температура равна {Forecast.Temperature}° по цельсию.");
            //   Выводимая температура не будет изменена, потому что структуры,
            //   в отличие от классов, это значимый/value тип данных.  
        }
    }
}
