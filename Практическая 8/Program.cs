//********************************************************************************
//*Практическая № 8
//*Выполнил Шемонаев.И.А 2-ИСПд
//*Задание: Дано целое чсило N(>1). Вывести наибольшее из целых чисел K его номер,
//для которых сумма 1+2+...+K будет больше или равна N, и саму эту сумму.
//*******************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Практическая_8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Здравствуйте");
            do//бесконечный цикл для постоянного запроса ввода
            {
                Console.Write("Введите целое число N (> 1) или 'стоп' для завершения: ");
                string input = Console.ReadLine();
                if (input.ToLower() == "стоп") break;
                if (!Int32.TryParse(input, out int N) || N <= 1) //Проверка является ли ввод целым числом и больше ли оно 1
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка: Введите целое число больше 1.");
                    Console.ResetColor();
                    Console.WriteLine("Нажмите любую клавишу для продолжения: ");
                    Console.ReadKey();
                    Console.Clear();

                    continue; //Возврат в начало цикла
                }
                int K = 0;
                int sum = 0;
                //Находим K и сумму 1 + 2 + ... + K
                do
                {
                    K++;
                    sum += K;
                } while (sum < N);
                Console.WriteLine($"Наибольшее K: {K}, сумма 1 + 2+ ... + {K}: {sum}");
                Console.WriteLine("Нажмите любую клавишу для продолжения: ");
                Console.ReadKey();
                Console.Clear();
            } while (true);
        }
    }
}
