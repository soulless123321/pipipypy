//***************************************************************
//*Практическая № 11
//*Выполнил Шемонаев.И.А 2-ИСПд
//*
//***************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Практическая_11
{
    internal class Program
    {
        static string RemoveDuplicates(string str)
        {
            string result = "";

            foreach (char c in str)
            {
                if (!result.Contains(c))
                {
                    result += c;
                }
            }
            return result;
        }
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Привет:3");
                Console.Write("Введите слово (или 'стоп' для завершения): ");
                string input = Console.ReadLine();

                if (input.ToLower() == "стоп")
                {
                    break;
                }
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Введите корректное слово.");
                    continue;
                }
                Console.WriteLine("Результат: " + RemoveDuplicates(input));
            }
        }       
    }
}
