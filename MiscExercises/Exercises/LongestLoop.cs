using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using My.Utils;



namespace Misc.Exercises
{
    class CLongestLoop
    {
        public static string Name = "Подсчет оставшегося времени выполнения (в долгом цикле)";

        public static void Info ()
        {
            Console.Write
            (
                " Задача: " + 
                "Подсчет оставшегося времени для очень долгой операции. " +
                "Для примера выбрана бесполезная операция проверки работы " +
                "внутренней функции."
            );
        }



        public static void Execute ()
        {
            //   В цикле идет подсчет оставшегося времени. 

            //   Небольшой бесполезный тест для проверки работы внутренней 
            //   функции, которая использована в коде в других примерах.
            //   Создан, опять же, ради примера. 

            LongUselessLoopInt32();

            LongUselessLoopInt64();
        }



        //   В .Net Framework 4.6+ еще нет удобного именованного типа ValueTuple
        public struct LoopUInt64
        {
            public ulong Step;
            public bool Continue;
        }

        public static void LongUselessLoopInt64 ()
        {
            Console.WriteLine($"CBitwiseOps.BitsCount : Starting self-test ({nameof(UInt64)})...");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.CursorVisible = false;

            //ulong min = ulong.MinValue;
            //ulong max = ulong.MaxValue;

            const long one_step = 10_000_000;
            long info_sum = 0;
            long info_steps = 0, info_totalsteps = unchecked( (long) (ulong.MaxValue / 10_000_000UL) );

            Stopwatch timer = Stopwatch.StartNew();

            for
            (
                //   Начало цикла: создаю кортеж (tuple), счетчик и флаг останова цикла. 
                var loop = new LoopUInt64 { Step = ulong.MinValue, Continue = true };

                //   Цикл работает при условии, что флаг останова не включен.
                //   Далее будет понятно, почему. 
                loop.Continue;

                //   Пост-действие цикла:
                //   Обрабатываем флаг останова. Здесь разрешается вопрос "Выполнять 
                //   ли *следующую* итерацию цикла?". Если да, то допускаем увеличение
                //   счетчика в этой итерации. Он достигает возможного предела для
                //   чисел UInt64 и выполняется. Но последующая итерация завершит цикл
                //   и арифметического переполнения не будет. 
                loop.Continue   = loop.Step < ulong.MaxValue,
                loop.Step       = (loop.Continue) ? ++loop.Step : 0
            )
            {
                int sum = 0, i = 0;
                ulong value = loop.Step;
                while (i < sizeof(ulong) * 8)
                {
                    if ((value & 1) > 0) sum++;
                    value = value >> 1;
                    i++;
                }

                //  Проверка функции
                if (sum != CBitwiseOps.BitsCount(loop.Step))
                {
                    throw new InvalidOperationException("CBitwiseOps.BitsCount : Fatal error in lookup table");
                }

                info_sum++;
                if (info_sum > one_step)
                {
                    info_sum = 0;
                    info_steps++;

                    long elapsed_ms = timer.ElapsedMilliseconds;
                    timer = Stopwatch.StartNew();

                    long sec_left = ((info_totalsteps - info_steps) * elapsed_ms) / 1000;
                    long yrs_left = sec_left / 60 / 60 / 24 / 365;
                    sec_left -= (yrs_left * 365 * 24 * 60 * 60);
                    long day_left = sec_left / 60 / 60 / 24;
                    sec_left -= (day_left * 24 * 60 * 60);
                    long hrs_left = sec_left / 60 / 60;
                    sec_left -= (hrs_left * 60 * 60);
                    long min_left = sec_left / 60;

                    Console.WriteLine("".PadLeft(Console.BufferWidth - 1, ' '));
                    Console.CursorTop--;

                    Console.CursorTop--;

                    string info =
                        $"{(info_steps / (info_totalsteps + 0.0f)) * 100:000.000}% " + 
                        $"({info_steps}0M from {info_totalsteps}0M), remaining time ~ " +
                        $"{yrs_left:0}:{day_left:00}:{hrs_left:00}:{min_left :00} y:d:h:m";

                    Console.WriteLine(info.Substring(0, Math.Min(Console.BufferWidth, info.Length)));
                }
            }

            Console.CursorVisible = true;
            Console.ResetColor();
            Console.WriteLine($"CBitwiseOps.BitsCount : Self-test completed ({nameof(UInt64)}).");
        }



        public static void LongUselessLoopInt32 ()
        {
            Console.WriteLine($"CBitwiseOps.BitsCount : Starting self-test ({nameof(UInt32)})...");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("");
            const uint one_step = 1_000_000U;
            int info_sum = 0;
            uint info_steps = 0, info_totalsteps = uint.MaxValue / one_step;

            Stopwatch timer = Stopwatch.StartNew();

            unchecked
            {
                for (uint b = 0; b <= uint.MaxValue; b++)
                {
                    int sum = 0, i = 0;
                    uint value = b;
                    while (i < sizeof(uint) * 8)
                    {
                        if ((value & 1) > 0) sum++;
                        value = value >> 1;
                        i++;
                    }

                    //  Проверка функции
                    if (sum != CBitwiseOps.BitsCount(b))
                    {
                        throw new InvalidOperationException("CBitwiseOps.BitsCount : Fatal error in lookup table");
                    }

                    info_sum++;
                    if (info_sum > one_step)
                    {
                        info_sum = 0;
                        info_steps++;

                        long elapsed_ms = timer.ElapsedMilliseconds;
                        timer = Stopwatch.StartNew();

                        long sec_left = ((info_totalsteps - info_steps) * elapsed_ms) / 1000;
                        long yrs_left = sec_left / 60 / 60 / 24 / 365;
                        sec_left -= (yrs_left * 365 * 24 * 60 * 60);
                        long day_left = sec_left / 60 / 60 / 24;
                        sec_left -= (day_left * 24 * 60 * 60);
                        long hrs_left = sec_left / 60 / 60;
                        sec_left -= (hrs_left * 60 * 60);
                        long min_left = sec_left / 60;

                        Console.WriteLine("".PadLeft(Console.BufferWidth - 1, ' '));
                        Console.CursorTop--;

                        Console.CursorTop--;

                        string info =
                            $"{(info_steps / (info_totalsteps + 0.0f)) * 100:000.000}% " + 
                            $"({info_steps}0M from {info_totalsteps}0M), remaining time ~ " +
                            $"{yrs_left:0}:{day_left:00}:{hrs_left:00}:{min_left :00} y:d:h:m";

                        Console.WriteLine(info.Substring(0, Math.Min(Console.BufferWidth, info.Length)));
                    }
                }
            }

            Console.ResetColor();
            Console.WriteLine($"CBitwiseOps.BitsCount : Self-test completed ({nameof(UInt32)}).");
        }
    }
}
