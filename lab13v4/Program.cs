using System;
using System.Collections.Generic;
using System.Text;

namespace Lab13App
{
    // =========================================================================
    // 1. КЛАС З GENERIC МЕТОДАМИ (FunctionalUtils)
    // =========================================================================
    public static class FunctionalUtils
    {
        // 1. Map: Перетворює List<T> у List<TResult> за допомогою Func<T, TResult>
        public static List<TResult> Map<T, TResult>(List<T> list, Func<T, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in list)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        // 2. Filter: Повертає елементи, що задовольняють предикату Func<T, bool>
        public static List<T> Filter<T>(List<T> list, Func<T, bool> predicate)
        {
            var result = new List<T>();
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        // 3. GroupBy (спрощений): Групує елементи за ключем у Dictionary<TKey, List<T>>
        public static Dictionary<TKey, List<T>> GroupBy<T, TKey>(List<T> list, Func<T, TKey> keySelector)
            where TKey : notnull
        {
            var result = new Dictionary<TKey, List<T>>();
            foreach (var item in list)
            {
                TKey key = keySelector(item);
                if (!result.ContainsKey(key))
                {
                    result[key] = new List<T>();
                }
                result[key].Add(item);
            }
            return result;
        }

        // 4. OrderBy: Сортує список за ключем TKey (IComparable<TKey>)
        public static List<T> OrderBy<T, TKey>(List<T> list, Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            var sortedList = new List<T>(list);
            sortedList.Sort((x, y) => keySelector(x).CompareTo(keySelector(y)));
            return sortedList;
        }

        // 5. ThenBy: Вторинне сортування за другим ключем для елементів з однаковим первинним ключем
        public static List<T> ThenBy<T, TKey1, TKey2>(
            List<T> list,
            Func<T, TKey1> primaryKeySelector,
            Func<T, TKey2> secondaryKeySelector)
            where TKey1 : IComparable<TKey1>
            where TKey2 : IComparable<TKey2>
        {
            var sortedList = new List<T>(list);
            sortedList.Sort((x, y) =>
            {
                int primaryCompare = primaryKeySelector(x).CompareTo(primaryKeySelector(y));
                if (primaryCompare != 0)
                {
                    return primaryCompare;
                }
                return secondaryKeySelector(x).CompareTo(secondaryKeySelector(y));
            });
            return sortedList;
        }
    }

    // =========================================================================
    // 2. ТЕСТОВА СУТНІСТЬ (Product)
    // =========================================================================
    public class Product
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }

        public Product(string name, string category, decimal price)
        {
            Name = name;
            Category = category;
            Price = price;
        }

        public override string ToString()
        {
            return $"[{Category}] {Name} — {Price:C}";
        }
    }

    // =========================================================================
    // 3. ДЕМОНСТРАЦІЯ РОБОТИ (Main)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("==========================================================================");
            Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №13. GENERIC-МЕТОДИ ТА ФУНКЦІЇ ВИЩОГО ПОРЯДКУ");
            Console.WriteLine("==========================================================================\n");

            // Тестові дані
            List<Product> products = new List<Product>
            {
                new Product("Ноутбук", "Електроніка", 25000m),
                new Product("Смартфон", "Електроніка", 18000m),
                new Product("Мишка", "Аксесуари", 800m),
                new Product("Клавіатура", "Аксесуари", 1500m),
                new Product("Монітор", "Електроніка", 12000m),
                new Product("Навушники", "Аксесуари", 1500m)
            };

            // 1. Map (Product -> string та int -> string)
            Console.WriteLine("--- 1. MAP: Трансформація об'єктів у список назв із ціною ---");
            List<string> productLabels = FunctionalUtils.Map(products, p => $"{p.Name.ToUpper()} ({p.Price:C})");
            foreach (var label in productLabels)
            {
                Console.WriteLine($" • {label}");
            }

            // Перевірка універсальності Map для чисел
            List<int> numbers = new List<int> { 10, 20, 30 };
            List<string> formattedNumbers = FunctionalUtils.Map(numbers, n => $"Елемент #{n}");
            Console.WriteLine($"Універсальність Map (int -> string): {string.Join(", ", formattedNumbers)}\n");

            // 2. Filter
            Console.WriteLine("--- 2. FILTER: Товари з ціною > 5000 ₴ ---");
            List<Product> expensiveProducts = FunctionalUtils.Filter(products, p => p.Price > 5000m);
            foreach (var product in expensiveProducts)
            {
                Console.WriteLine($" • {product}");
            }
            Console.WriteLine();

            // 3. GroupBy
            Console.WriteLine("--- 3. GROUPBY: Групування товарів за категорією ---");
            var groupedByCategory = FunctionalUtils.GroupBy(products, p => p.Category);
            foreach (var group in groupedByCategory)
            {
                Console.WriteLine($" [{group.Key}]:");
                foreach (var product in group.Value)
                {
                    Console.WriteLine($"    - {product.Name} ({product.Price:C})");
                }
            }
            Console.WriteLine();

            // 4. OrderBy
            Console.WriteLine("--- 4. ORDERBY: Сортування за ціною (за зростанням) ---");
            List<Product> sortedByPrice = FunctionalUtils.OrderBy(products, p => p.Price);
            foreach (var product in sortedByPrice)
            {
                Console.WriteLine($" • {product}");
            }
            Console.WriteLine();

            // 5. ThenBy
            Console.WriteLine("--- 5. THENBY: Сортування за категорією, а потім за ціною ---");
            List<Product> sortedByCategoryThenPrice = FunctionalUtils.ThenBy(
                products,
                p => p.Category,
                p => p.Price
            );
            foreach (var product in sortedByCategoryThenPrice)
            {
                Console.WriteLine($" • {product}");
            }

            Console.WriteLine("\n==========================================================================");
            Console.WriteLine("  ОБРОБКУ GENERIC-МЕТОДІВ УСПІШНО ЗАВЕРШЕНО");
            Console.WriteLine("==========================================================================");
        }
    }
}