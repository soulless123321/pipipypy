using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Практическая__10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Введите размер квадратной матрицы (или 'стоп' для завершения): ");
                string input = Console.ReadLine();
                if (input.ToLower() == "стоп") { break; }
                if (!int.TryParse(input, out int n) || n <= 0)
                {
                    Console.WriteLine("Ошибка: введите корректное положетельное целое число.");
                    continue;
                }

                int[,] matrix = new int[n, n];

                //ввод матрицы
                Console.WriteLine("Введите элементы матрицы: ");
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        while (true)
                        {
                            Console.Write($"Элемент [{i}, {j}]: ");
                            if(Int32.TryParse(Console.ReadLine(), out matrix[i, j])) { break; }
                            Console.WriteLine("Ошибка: введите корректное целое число.");
                        }
                    }
                }
                //поиск минимального по модулю диагонального элемента
                int minIndex = 0;
                for (int i = 1;i < n; i++)
                {
                    if (Math.Abs(matrix[i, i]) < Math.Abs(matrix[minIndex, minIndex]))
                    {
                        minIndex = i;
                    }
                }
                //Запоминаем значение минимального по модулю диагонального элемента 
                int minValue = matrix[minIndex, minIndex];
                for (int i = 0; i < n ; i++)
                {
                    for(int j = 0;j < n; j++)
                    {
                        if(i == j)
                        {
                            matrix[i, j] = minValue; //присвоение минимального диагонального элемента
                        }
                        else if(i < j)
                        {
                            matrix[i, j] = 1; 
                        }
                        else
                        {
                            matrix[i,j] = 2;
                        }
                    }
                }
                Console.WriteLine("Изменяемая матрица:");
                for (int i = 0;i < n ; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        Console.Write(matrix[i,j] + "\t");
                    }
                    Console.WriteLine();
                }
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
