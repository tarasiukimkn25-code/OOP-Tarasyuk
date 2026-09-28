using System;

namespace lab4v4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №4 (Варіант 4) ===");
            Console.WriteLine($"Статичний член (Абсолютний нуль): {Temperature.AbsoluteZeroCelsius}°C\n");

            try
            {
                // Створення об'єктів
                Temperature t1 = new Temperature(22.5, "Celsius");
                Temperature t2 = new Temperature(10.0, "Celsius");

                Console.WriteLine($"t1: {t1}");
                Console.WriteLine($"t2: {t2}");

                // 1. Демонстрація оператора +
                Temperature sum = t1 + t2;
                Console.WriteLine($"Результат додавання (t1 + t2): {sum}");

                // 2. Демонстрація індексатора
                Console.WriteLine($"Індексатор t1[0] (Value): {t1[0]}");
                Console.WriteLine($"Індексатор t1[1] (Unit): {t1[1]}");

                // 3. Демонстрація операторів порівняння == та !=
                Temperature t3 = new Temperature(22.5, "Celsius");
                Console.WriteLine($"t1 == t3: {t1 == t3}");
                Console.WriteLine($"t1 != t2: {t1 != t2}");

                // 4. Демонстрація валідації (помилка)
                Console.WriteLine("\nСпроба створити температуру нижче абсолютного нуля:");
                Temperature invalid = new Temperature(-300, "Celsius");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Отримано очікуваний виняток: {ex.Message}");
            }
        }
    }
}

