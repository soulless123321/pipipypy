using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Здравствуйте!");
            Console.Write("Введите номер билета: ");
            int ticket = Convert.ToInt32(Console.ReadLine());
            switch (ticket >= 100000 && ticket <= 999999)
            {
                case true:
                    {
                        int b = ticket / 100000; // проверка первого числа
                        int c = (ticket / 10000) % 10; // проверка второго числа
                        int d = (ticket / 1000) % 10; // проверка третьего числа
                        int e = (ticket / 100) % 10; // проверка четвертого числа
                        int f = (ticket / 10) % 10; // проверка пятого числа
                        int g = (ticket % 10); // проверка шестого числа
                        switch (b + c + d == e + f + g) // проверка седьмого числа
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
