using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Практическая_работа___10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Введите размер квадратной матрицы (или 'стоп' для завершения): ");
                string input = Console.ReadLine();
                if (input.ToLower() == "стоп")
                {
                    break; // Завершение программы
                }

                if (!int.TryParse(input, out int n) || n <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Ошибка: введите корректное положительное целое число.");
                    Console.ResetColor();
                    continue; // Переход к следующей итерации
                }

                int[,] matrix = new int[n, n];

                // Ввод матрицы
                Console.WriteLine("Введите элементы матрицы:");
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        while (true)
                        {
                            Console.Write($"Элемент [{i}, {j}]: ");
                            if (int.TryParse(Console.ReadLine(), out matrix[i, j]))
                            {
                                break; // Выход из цикла при корректном вводе
                            }
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Ошибка: введите корректное целое число.");
                            Console.ResetColor();
                        }
                    }
                }

                // Поиск минимального по модулю диагонального элемента
                int minIndex = 0;
                for (int i = 1; i < n; i++)
                {
                    if (Math.Abs(matrix[i, i]) < Math.Abs(matrix[minIndex, minIndex]))
                    {
                        minIndex = i;
                    }
                }

                // Запоминаем значение минимального по модулю диагонального элемента
                int minValue = matrix[minIndex, minIndex];

                // Изменение матрицы по условиям задачи
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i == j)
                        {
                            matrix[i, j] = minValue; // Присвоение минимального диагонального элемента
                        }
                        else if (i < j)
                        {
                            matrix[i, j] = 1; // Элементы выше главной диагонали
                        }
                        else
                        {
                            matrix[i, j] = 2; // Элементы ниже главной диагонали
                        }
                    }
                }

                // Вывод измененной матрицы
                Console.WriteLine("Измененная матрица:");
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        Console.Write(matrix[i, j] + "\t");
                    }
                    Console.WriteLine();
                }
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
