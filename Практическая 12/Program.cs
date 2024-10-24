//***********************************************************************
//*Практическая № 4
//*Выполнил: Шемонаев.И.А 2-ИСПд
//*Задание: Написвть код по функции
//***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Практическая_12
{
    internal class Program
    {
        static void ExeptionWrait(string extext)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка: " + extext);
            Console.ResetColor();
        }

        static string ToUpperLatin(string str)
        {
            char[] result = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c >= 'a' && c <= 'z') //Проверка на латинские буквы в нижнем регистре
                {
                    result[i] = (char)(c - 32); //Преобразование в верхний регистр
                }
                else
                {
                    result[i] = c; //Оставляем символ без изминений
                }
            }
            return new string(result);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Здравствуйте");
            while (true)
            {
                try
                {
                    Console.Write("Введите строку (или 'стоп' для завершения): ");
                    string input = Console.ReadLine();
                    if (input.ToLower() == "стоп") break; //Прекращение работы кода
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        
                        throw new ArgumentNullException("Строка не должна быть пустой или состоять только из пробелов.");
                        
                    }
                    string result = ToUpperLatin(input); // Вызов функции преобразования
                    Console.WriteLine("Результат: " + result); // Вывод результата               
                }
                catch (FormatException ex)
                {
                    ExeptionWrait(ex.Message);
                }
                
                catch (Exception ex)
                {
                    ExeptionWrait(ex.Message);
                }
            }
        }
    }
}
