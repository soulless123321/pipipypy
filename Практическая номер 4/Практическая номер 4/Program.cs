//******************************************************
//*Практическая № 4
//*Выполнил Шемонаев.И.А 2-ИСПд
//*Задание: Написать код по функции
//******************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Практическая_номер_4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const double e = 2.718;                         //Число Эйлера
            Console.Write("Введите значение x: ");
            double x = Double.Parse(Console.ReadLine());    //Ввел число X и перевел в double  
            Console.Write("Введите значение b: ");
            double b = Double.Parse(Console.ReadLine()); 
            double v1 = Math.Cos(Math.Pow(x,3));
            double v2 = Math.Pow(b + Math.Atan(x * 2),0.2);
            double v3 = 3 * Math.Pow(e, Math.Sqrt(b * x));
            double v4 = v2 / v3;
            double v5 = Math.Abs(v4);
            double v6 = v1 + v5;                            //Ответ
            Console.WriteLine($"Ответ: {Math.Round(v6, 5)}");
            Console.ReadKey();
        }
    }
}
