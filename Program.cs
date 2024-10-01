//*************************************************************************
//* практическая работа 6 
//* Выполнил:  студент 2-ИСПд Шемонаев.И.А
//* Задание: переписать практическую 5 на switch
//*************************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace практическая_6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите номер билета: ");
            int ticket = Convert.ToInt32(Console.ReadLine());
            switch (ticket >= 100000 && ticket <= 999999)
            {
                case true:
                {
                    int b = ticket / 100000;
                    int c = (ticket / 10000) % 10;
                    int d = (ticket / 1000) % 10;
                    int e = (ticket / 100) % 10;
                    int f = (ticket / 10) % 10;
                    int g = (ticket % 10);
                    switch (b + c + d == e + f + g)
                    {
                        case true:
                        {
                            Console.WriteLine("Билет счастливый");
                            break;
                        }
                        case false:
                        {
                            Console.WriteLine("не счастливый билет");
                            break;
                        }
                    }
                        break;
                    }
                    case false:
                    {
                        Console.WriteLine("Номер билета не шестизначный");
                        break;
                    }
            }
            Console.ReadKey();
        }
    }
}

