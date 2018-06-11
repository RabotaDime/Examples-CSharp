using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc.Exercises
{
    class CLoopULongExample
    {
        public static string Name = "Цикл с \"отложенной\" само остановкой (пример: полный UInt64 диапазон)";

        public static void Info ()
        {
            Console.Write
            (
                " Задача: " +
                "Цикл с счетчиком, который \"выключает\" сам себя уже" +
                "в пост-действии, а не пре-условии. Тип ulong использован " +
                "для категоричного примера, когда технически невозможно " +
                "использовать более емкий тип данных. Но при желании можно " +
                "использовать любой тип."
            );
        }



        public static void Execute ()
        {
            bool skip = true, loop = true;

            ulong min = ulong.MinValue;
            ulong max = ulong.MaxValue;

            Console.WriteLine($"Начало цикла от {min} до {max}");

            //  С помощью дополнительного флага цикл остается "чистым"
            //  от инструкций обслуживания самого цикла. Флаг при этом
            //  выключает цикл (сам себя) уже после последней итерации. 
            for (ulong i = min; loop; loop = i < max, i = (loop) ? ++i : 0)
            {
                //  Пропуск: 
                if (skip && (i > 3))
                {
                    i = max - 3;
                    skip = false;

                    Console.WriteLine("  ...");
                    Console.WriteLine("  прошло несколько лет жизни");
                    Console.WriteLine("  ...");
                }
                else
                {
                    Console.Write($"  ulong i = {i} ");
                    if (i == min)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("minimum reached");
                        Console.ResetColor();
                    }
                    else if (i == max)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("maximum reached");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                }
            }

            Console.WriteLine($"Конец цикла");
        }
    }
}
