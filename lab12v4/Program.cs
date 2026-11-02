using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab12App
{
    // =========================================================================
    // 1. УЗАГАЛЬНЕНИЙ КЛАС З GENERIC МЕТОДАМИ (FunctionalUtils)
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

        // 4. OrderBy: Сортує список за ключем TKey
        public static List<T> OrderBy<T, TKey>(List<T> list, Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            var sortedList = new List<T>(list);
            sortedList.Sort((x, y) => keySelector(x).CompareTo(keySelector(y)));
            return sortedList;
        }

        // 5. ThenBy: Додаткове сортування для вже відсортованого списку (первинні рівні ключі порівнюються за другим)
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
    // 2. ТЕСТОВА СУТНІСТЬ (Student)
    // =========================================================================
    public class Student
    {
        public string Name { get; set; }
        public string Course { get; set; }
        public double AverageGrade { get; set; }

        public Student(string name, string course, double averageGrade)
        {
            Name = name;
            Course = course;
            AverageGrade = averageGrade;
        }

        public override string ToString()
        {
            return $"{Name} (Курс: {Course}, Сер. бал: {AverageGrade:F1})";
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
            Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №12. GENERIC-МЕТОДИ ТА ФУНКЦІЇ ВИЩОГО ПОРЯДКУ");
            Console.WriteLine("==========================================================================\n");

            // Тестові дані
            List<Student> students = new List<Student>
            {
                new Student("Олексій", "2 курс", 88.5),
                new Student("Марія", "1 курс", 95.0),
                new Student("Іван", "2 курс", 74.0),
                new Student("Анна", "1 курс", 91.2),
                new Student("Дмитро", "3 курс", 85.0),
                new Student("Олена", "2 курс", 95.0)
            };

            // 1. Демонстрація Map (Student -> string)
            Console.WriteLine("--- 1. MAP: Отримання списку імен студентів у верхньому регістрі ---");
            List<string> uppercaseNames = FunctionalUtils.Map(students, s => s.Name.ToUpper());
            foreach (var name in uppercaseNames)
            {
                Console.WriteLine($" • {name}");
            }

            // Демонстрація Map на іншому типі (int -> string)
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            List<string> formattedNumbers = FunctionalUtils.Map(numbers, n => $"Число #{n}");
            Console.WriteLine($"Узагальненість Map (int -> string): {string.Join(", ", formattedNumbers)}\n");

            // 2. Демонстрація Filter
            Console.WriteLine("--- 2. FILTER: Студенти з середнім балом >= 90 ---");
            List<Student> topStudents = FunctionalUtils.Filter(students, s => s.AverageGrade >= 90.0);
            foreach (var student in topStudents)
            {
                Console.WriteLine($" • {student}");
            }
            Console.WriteLine();

            // 3. Демонстрація GroupBy
            Console.WriteLine("--- 3. GROUPBY: Групування студентів за курсами ---");
            var groupedByCourse = FunctionalUtils.GroupBy(students, s => s.Course);
            foreach (var group in groupedByCourse)
            {
                Console.WriteLine($" [{group.Key}]:");
                foreach (var student in group.Value)
                {
                    Console.WriteLine($"    - {student.Name}");
                }
            }
            Console.WriteLine();

            // 4. Демонстрація OrderBy
            Console.WriteLine("--- 4. ORDERBY: Сортування за середнім балом (за зростанням) ---");
            List<Student> sortedByGrade = FunctionalUtils.OrderBy(students, s => s.AverageGrade);
            foreach (var student in sortedByGrade)
            {
                Console.WriteLine($" • {student}");
            }
            Console.WriteLine();

            // 5. Демонстрація ThenBy (Сортування за курсом, а потім за середнім балом)
            Console.WriteLine("--- 5. THENBY: Сортування за курсом, а потім за баллом ---");
            List<Student> sortedByCourseThenGrade = FunctionalUtils.ThenBy(
                students, 
                s => s.Course, 
                s => s.AverageGrade
            );
            foreach (var student in sortedByCourseThenGrade)
            {
                Console.WriteLine($" • {student}");
            }

            Console.WriteLine("\n==========================================================================");
            Console.WriteLine("  ОБРОБКУ МЕТОДІВ УСПІШНО ЗАВЕРШЕНО");
            Console.WriteLine("==========================================================================");
        }
    }
}