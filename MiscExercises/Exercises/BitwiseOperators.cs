using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using My.Utils;
using static My.Windows.API.CWindowFunctions;

using Console = My.Utils.CustomConsole.Console;



namespace Misc.Exercises
{
    class CBitwiseOperators
    {
        public static string Name = "Тест побитовых операторов";

        public static void Info ()
        {
            Console.Write
            (
                " Задача: " + 
                "Разбор нескольких ситуаций для битовых операторов и перечислений"
            );
        }



        public static void Execute ()
        {
            //   Известный трюк: обмен значений двух переменных без третьего контейнера
            //   (xor swap)
            int a = 1234;
            int b = 9876;
            a = a ^ b;
            b = a ^ b;
            a = a ^ b;
            Console.WriteLine($"a = {a}, b = {b}");


            //   ulong.ToBinary -- это мой метод. 
            //   В C# у класса Convert отсутствует конверсия в строку с числовой базой. 
            //   Я специально сделал метод с использованием битового оператора сдвига ">>"
            //   для примера, а не использовал системный BitConverter класс.
            ulong big_number;

            big_number = 5_764_607_523_034_234_890;
            Console.WriteLine($"0b{big_number.ToBinary(32)}");

            big_number = 1;
            Console.WriteLine($"0b{big_number.ToBinary(4)}");



            //   Проверка перечисляемого типа на наличие битов 
            //   в XOR режиме (должен быть только один бит)
            BoundsSpecified test_flags;


            //   У класса System.Enum мне не хватает полезных битовых методов,
            //   которые бы упростили читаемость кода и написание условий. 
            //   Далее я рассматриваю два своих метода-расширения, и имеющийся:
            //     1. Энумерация имеет один включенный бит из перечисленных
            //        в маске.
            //     2. Энумерация имеет включенным все биты из заданных
            //        в маске. Этот метод есть в стандартном System.Enum описании.
            //     3. Энумерация не имеет ни одного бита из перечисленных в маске.
            //   Кроме повышения читаемости кода, основной интерес в данном примере 
            //   во внутренней функции расчета количества бит в любом числе.
            //     См. CBitwiseOps.BitsCount

            //   XOR (x или width)
            test_flags = BoundsSpecified.X | BoundsSpecified.Height | BoundsSpecified.Y;
            if (test_flags.HasOneOf(BoundsSpecified.X | BoundsSpecified.Width))
            {
                Console.WriteLine
                (
                    $"В {nameof(test_flags)} есть флаг " +
                    $"'{nameof(BoundsSpecified.X)}' или " +
                    $"'{nameof(BoundsSpecified.Width)}'" +
                    $", но не оба."
                );
            }
            else
                Console.WriteLine($"Ошибка в расчетах. Сюда программа никогда не должна попасть.");

            if (test_flags.HasOneOf(BoundsSpecified.X | BoundsSpecified.Height))
                Console.WriteLine($"Ошибка в расчетах. Сюда программа никогда не должна попасть.");
            else
            {
                Console.WriteLine
                (
                    $"В {nameof(test_flags)} есть флаг " +
                    $"'{nameof(BoundsSpecified.X)}' и " +
                    $"'{nameof(BoundsSpecified.Height)}'" +
                    $", и это хорошо."
                );
            }


            //   AND (x и width)
            test_flags = BoundsSpecified.X | BoundsSpecified.Height | BoundsSpecified.Y; 
            if (test_flags.HasFlag(BoundsSpecified.X | BoundsSpecified.Height))
            {
                Console.WriteLine
                (
                    $"В {nameof(test_flags)} есть оба флага: " +
                    $"'{nameof(BoundsSpecified.X)}' и " +
                    $"'{nameof(BoundsSpecified.Height)}'."
                );
            }
            else
                Console.WriteLine($"Ошибка в расчетах. Сюда программа никогда не должна попасть.");

            test_flags = BoundsSpecified.X | BoundsSpecified.Height | BoundsSpecified.Y; 
            if (test_flags.HasFlag(BoundsSpecified.X | BoundsSpecified.Width))
                Console.WriteLine($"Ошибка в расчетах. Сюда программа никогда не должна попасть.");
            else
            {
                Console.WriteLine
                (
                    $"В {nameof(test_flags)} есть оба флага: " +
                    $"'{nameof(BoundsSpecified.X)}' и " +
                    $"'{nameof(BoundsSpecified.Width)}'."
                );
            }


            //   NONE (ни x, ни width)
            test_flags = BoundsSpecified.X | BoundsSpecified.Height;
            if (test_flags.HasNoneOf(BoundsSpecified.Width | BoundsSpecified.Y))
            {
                Console.WriteLine
                (
                    $"В {nameof(test_flags)} нет ни флага {nameof(BoundsSpecified.Y)}, " +
                    $"ни флага '{nameof(BoundsSpecified.Width)}'."
                );
            }
            else
                Console.WriteLine($"Ошибка в расчетах. Сюда программа никогда не должна попасть.");

            test_flags = BoundsSpecified.X | BoundsSpecified.Height | BoundsSpecified.Y;
            if (test_flags.HasNoneOf(BoundsSpecified.X | BoundsSpecified.Width))
                Console.WriteLine($"Ошибка в расчетах. Сюда программа никогда не должна попасть.");
            else
            {
                Console.WriteLine
                (
                    $"В {nameof(test_flags)} есть флаг {nameof(BoundsSpecified.X)} или " +
                    $"флаг '{nameof(BoundsSpecified.Width)}' " +
                    $"и это хорошо."
                );
            }
        }
    }
}
