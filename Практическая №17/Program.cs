using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Практическая__17
{
    class WaterTransport
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public double Speed { get; set; }

        public void InputData() //ввод данных о водном средстве
        {
            Console.WriteLine("Введите название водного средства: ");
            Name = Console.ReadLine();

            while (true)
            {
                Console.Write("Введите вместимость: ");
                if (int.TryParse(Console.ReadLine(), out int capacity))
                {
                    Capacity = capacity;
                    break;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка: введите корректное число для вместимости");
                Console.ResetColor();
            }
            while (true)
            {
                Console.Write("Введите скорость: ");
                if (double.TryParse(Console.ReadLine(), out double speed) && speed > 0)
                {
                    Speed = speed;
                    break;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка: введите корректное положительное число для скорости.");
                Console.ResetColor();
            }

        }
        public void DisplayData() // выводит данные о судне
        {
            Console.WriteLine($"Название: {Name}");
            Console.WriteLine($"Вместимость: {Capacity} человек");
            Console.WriteLine($"Скорость: {Speed}км/ч");
        }
        public void CalculateTravelTime(double distance) //расчет времени в пути
        {
            double time = distance / Speed;
            Console.WriteLine($"Время в пути на {distance} км: {time:F2} часов");
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                WaterTransport transport = new WaterTransport();
                transport.InputData();
                transport.DisplayData();

                Console.Write("Введите расстояние для расчета времени в пути: ");
                if (double.TryParse(Console.ReadLine(), out double distance))
                {
                    transport.CalculateTravelTime(distance);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка: введите корректное число для расстояния.");
                    Console.ResetColor();
                }

                Console.WriteLine("Введите 'стоп' для завершения программы или любую клавишу для продолжения: ");
                string command = Console.ReadLine();
                if (command.ToLower() == "стоп")
                {
                    break;
                }
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
