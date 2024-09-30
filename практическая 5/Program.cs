//*************************************************************************
//* практическая работа 5 
//* Выполнил студент 2-ИСПд Шемонаев.И.А
//* адание:написать программу определяющую счастливый билет
//*************************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace практическая_5
{
    internal class Program
    {
        static void Main(string[] args)
        {


            Console.WriteLine("Введите номер билета: ");
            int ticket = Convert.ToInt32(Console.ReadLine());
            if (ticket >= 100000 && ticket <= 999999)
            {
                int b = ticket / 100000;
                int c = (ticket / 10000) % 10;
                int d = (ticket / 1000) % 10;
                int e = (ticket / 100) % 10;
                int f = (ticket / 10) % 10;
                int g = (ticket % 10);
                if (b + c + d == e + f + g)

                    Console.WriteLine("Билет счастливый");
                else
                    Console.WriteLine("не счастливый билет");
            }
            else
                Console.WriteLine("Номер билета не шестизначный");
                Console.ReadKey();

            

        }



    }
}
