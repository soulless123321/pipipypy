using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Практическая_работа__17
{
    class WaterTransport
    {
        string Name { get; set; } // Название водного средства
        int Capacity { get; set; } // Вместимость в людях
        double Speed { get; set; } // Скорость в км/ч

        // Метод для ввода данных о водном средстве
        void InputData()
        {
            Console.Write("Введите название водного средства: ");
            Name = Console.ReadLine();

            // Проверка ввода вместимости
            while (true)
            {
                Console.Write("Введите вместимость: ");
                if (int.TryParse(Console.ReadLine(), out int capacity))
                {
                    Capacity = capacity;
                    break; // Выход из цикла при корректном вводе
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка: введите корректное число для вместимости.");
                Console.ResetColor();
            }

            // Проверка ввода скорости
            while (true)
            {
                Console.Write("Введите скорость: ");
                if (double.TryParse(Console.ReadLine(), out double speed) && speed > 0)
                {
                    Speed = speed;
                    break; // Выход из цикла при корректном вводе
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка: введите корректное положительное число для скорости.");
                Console.ResetColor();
            }
        }

        // Метод для отображения данных о водном средстве
        void DisplayData()
        {
            Console.WriteLine($"Название: {Name}");
            Console.WriteLine($"Вместимость: {Capacity} человек");
            Console.WriteLine($"Скорость: {Speed} км/ч");
        }

        // Метод для расчета времени в пути
        void CalculateTravelTime(double distance)
        {
            double time = distance / Speed; // Расчет времени
            Console.WriteLine($"Время в пути на {distance} км: {time:F2} часов");
        }

        // Метод для выполнения всех операций
        public void Execute()
        {
            InputData(); // Запрашиваем данные у пользователя
            DisplayData(); // Отображаем введенные данные

            // Запрашиваем расстояние для расчета времени в пути
            Console.Write("Введите расстояние для расчета времени в пути: ");
            if (double.TryParse(Console.ReadLine(), out double distance))
            {
                CalculateTravelTime(distance); // Рассчитываем и выводим время в пути
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка: введите корректное число для расстояния.");
                Console.ResetColor();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Основной цикл программы
            while (true)
            {
                WaterTransport transport = new WaterTransport(); // Создаем новый объект водного средства
                transport.Execute(); // Выполняем все операции

                // Запрос на завершение программы
                Console.Write("Введите 'стоп' для завершения программы или любую клавишу для продолжения: ");
                string command = Console.ReadLine();
                if (command.ToLower() == "стоп")
                {
                    break; // Выход из цикла и завершение программы
                }
            }
        }
    }
}
