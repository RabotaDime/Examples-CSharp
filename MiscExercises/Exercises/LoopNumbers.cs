using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using My.Utils;



namespace Misc.Exercises
{
    class CLoopNumbers
    {
        public static string Name = "Вывод чисел с условиями";

        public static void Info ()
        {
            Console.Write
            (
                " Задача: " + 
                "Вывести числа от 0 до 1000, кратные трем, не кратные пяти, " +
                "и в которых сумма цифр меньше десяти."
            );
        }



        public static void Execute ()
        {
            Console.Write("{");

            string Joint = String.Empty;

            for (int N = 0; N <= 1000; N += 3)
            {
                if (N % 5 == 0) continue;

                int TempN = N, DigitsSum = 0;
                while (TempN > 0)
                {
                    DigitsSum += (TempN % 10);
                    TempN = (TempN / 10);
                }
                if (DigitsSum >= 10) continue;

                Console.Write(Joint);
                Console.Write(N);
                //Console.WriteLine($" sum({DigitsSum})");

                Joint = ", ";
            }

            Console.WriteLine("}");
        }
    }
}
