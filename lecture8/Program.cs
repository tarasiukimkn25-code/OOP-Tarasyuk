using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture8App
{
    // 1. Оголошення власного кастомного делегата
    public delegate double DiscountStrategy(decimal price);

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("=========================================================");
            Console.WriteLine("  ЛЕКЦІЯ №8. ДЕЛЕГАТИ, АНОНІМНІ ФУНКЦІЇ ТА ЛЯМБДА-ВИРАЗИ");
            Console.WriteLine("=========================================================\n");

            // --- 1. Кастомні та багатоадресні делегати ---
            Console.WriteLine("--- 1. Власний делегат та знижка ---");
            DiscountStrategy studentDiscount = price => (double)(price * 0.20m); // 20% знижки
            decimal originalPrice = 1000m;
            double discountAmount = studentDiscount(originalPrice);
            Console.WriteLine($"Початкова ціна: {originalPrice:C}, Знижка: {discountAmount:C}\n");

            // --- 2. Вбудовані делегати: Action, Func, Predicate ---
            Console.WriteLine("--- 2. Вбудовані делегати (.NET) ---");
            
            // Action<T> - не повертає значення (void)
            Action<string> logMessage = msg => Console.WriteLine($" [Action Log]: {msg}");
            logMessage("Система завантажена успішно");

            // Func<T, TResult> - повертає значення
            Func<int, int, int> multiply = (x, y) => x * y;
            Console.WriteLine($" [Func] Множення 6 * 7 = {multiply(6, 7)}");

            // Predicate<T> - повертає bool
            Predicate<int> isEven = number => number % 2 == 0;
            Console.WriteLine($" [Predicate] Чи є число 10 парним? {isEven(10)}\n");

            // --- 3. Анонімні методи vs Лямбда-вирази ---
            Console.WriteLine("--- 3. Анонімні методи vs Лямбда-вирази ---");
            
            // Анонімний метод (C# 2.0)
            Func<int, bool> isPositiveOld = delegate(int x)
            {
                return x > 0;
            };

            // Лямбда-вираз (C# 3.0+)
            Func<int, bool> isPositiveNew = x => x > 0;

            Console.WriteLine($"Анонімний метод (5 > 0): {isPositiveOld(5)}");
            Console.WriteLine($"Лямбда-вираз (5 > 0):    {isPositiveNew(5)}\n");

            // --- 4. Використання з колекціями та LINQ ---
            Console.WriteLine("--- 4. Робота з колекціями та LINQ ---");
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Передаємо лямбда-вирази в LINQ-методи
            var evenSquares = numbers
                .Where(n => n % 2 == 0)      // Фільтрація
                .Select(n => n * n);         // Проекція

            Console.WriteLine("Квадрати парних чисел: " + string.Join(", ", evenSquares));

            Console.WriteLine("\n=========================================================");
            Console.WriteLine("  ДЕМОНСТРАЦІЮ ЛЕКЦІЇ №8 УСПІШНО ЗАВЕРШЕНО");
            Console.WriteLine("=========================================================");
        }
    }
}