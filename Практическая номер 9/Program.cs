//************************************************************
//*Практическая работа номер 9
//*Выполнил: студент 2-ИСПд Шемонаев.И.А
//*Задание:Дан массив целых чисел.Найти кол-во тех элементов,
//значения которых положительны и не превосходят заданного
//натурального числа А
//************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Практическая_номер_9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите натуральное число: A  ");
            int num1 = Convert.ToInt32(Console.ReadLine());
            int[] array = new int[20];
            Random num2 = new Random();
            int num3 = 0;
            for (int i = 1; i < array.Length; i++)
            {
                array[i] = num2.Next(0, 100);
                Console.WriteLine(array[i]);
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > num1 && array[i]> 0) 
                {
                    num3 ++;  
                }
                Console.WriteLine(num3);
                Console.ReadKey();
            }
        }
    }
}
