//***************************************
//*Практическая № 9
//*Выполнил Шемонаев.И.А 2-ИСПд
//*Задание:Найдите наименьший элемент массива, стоящий на чётном месте
//*(размерность – 10 элементов). Если такого нет, то выведите первый элемент.
//***************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Практическая___9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Здравствуйте");
            Console.WriteLine();
                while (true)
                {
                    int[] numbers = new int[10];

                    Console.WriteLine("Введите 10 целых чисел (или 'стоп' для завершения):");

                    // Ввод элементов массива с проверкой на ошибки
                    for (int i = 0; i < numbers.Length; i++)
                    {
                        Console.Write($"Элемент {i + 1}: ");
                        string input = Console.ReadLine();

                        if (input.ToLower() == "стоп") return; // Завершение программы

                        if (!int.TryParse(input, out numbers[i]))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Ошибка: Введите корректное целое число.");
                            Console.ResetColor();
                            i--; // Повторный ввод для текущего элемента
                        }
                    }
                    // Поиск наименьшего элемента на четных индексах
                    int minEvenElement = numbers[0]; // Инициализация с первым элементом
                    bool found = false; // Флаг для отслеживания найденного элемента

                    for (int i = 0; i < numbers.Length; i += 2)
                    {
                        if (numbers[i] < minEvenElement)
                        {
                            minEvenElement = numbers[i];
                            found = true; // Устанавливаем флаг, если нашли меньший элемент
                        }
                    }
                    if (found)
                    {
                        Console.WriteLine($"Наименьший элемент на четных местах: {minEvenElement}");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else
                    {
                        Console.WriteLine($"Наименьший элемент на четных местах не найден. Первый элемент: {numbers[0]}");
                        Console.ReadKey();
                        Console.Clear();
                }
                }   
        }

    }
}
