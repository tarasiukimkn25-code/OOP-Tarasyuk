using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LibraryLinqApp
{
    // =========================================================================
    // 1. КЛАСИ СУТНОСТЕЙ (Author, Book)
    // =========================================================================
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public Author(int id, string name, string country)
        {
            Id = id;
            Name = name;
            Country = country;
        }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public int Year { get; set; }

        public Book(int id, string title, int authorId, int year)
        {
            Id = id;
            Title = title;
            AuthorId = authorId;
            Year = year;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // =========================================================================
            // 2. ГЕНЕРАЦІЯ ТЕСТОВИХ ДАНИХ
            // =========================================================================
            List<Author> authors = new List<Author>
            {
                new Author(1, "Stephen King", "USA"),
                new Author(2, "George Orwell", "UK"),
                new Author(3, "J.K. Rowling", "UK"),
                new Author(4, "Agatha Christie", "UK"),
                new Author(5, "Taras Shevchenko", "Ukraine")
            };

            List<Book> books = new List<Book>
            {
                new Book(1, "The Shining", 1, 1977),
                new Book(2, "It", 1, 1986),
                new Book(3, "The Green Mile", 1, 1996),
                new Book(4, "11/22/63", 1, 2011),
                new Book(5, "Doctor Sleep", 1, 2013),
                new Book(6, "1984", 2, 1949),
                new Book(7, "Animal Farm", 2, 1945),
                new Book(8, "Harry Potter and the Philosopher's Stone", 3, 1997),
                new Book(9, "Harry Potter and the Prisoner of Azkaban", 3, 1999),
                new Book(10, "Murder on the Orient Express", 4, 1934),
                new Book(11, "And Then There Were None", 4, 1939),
                new Book(12, "Kobzar", 5, 1840),
                new Book(13, "Cell", 1, 2006),
                new Book(14, "Under the Dome", 1, 2009)
            };

            Stopwatch sw = new Stopwatch();

            Console.WriteLine("=========================================================");
            Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №15. LINQ ТА АНАЛІЗ ПРОДУКТИВНОСТІ");
            Console.WriteLine("=========================================================\n");

            // =========================================================================
            // ЗАПИТ 1: Книги Stephen King після 1990 року
            // =========================================================================
            Console.WriteLine("--- ЗАПИТ 1: Книги Стівена Кінга, видані після 1990 року ---");

            // Method Syntax
            sw.Restart();
            var q1Method = books
                .Where(b => b.AuthorId == authors.First(a => a.Name == "Stephen King").Id && b.Year > 1990)
                .ToList();
            sw.Stop();
            long timeQ1Method = sw.ElapsedTicks;

            // Query Syntax
            sw.Restart();
            var q1Query = (from b in books
                          join a in authors on b.AuthorId equals a.Id
                          where a.Name == "Stephen King" && b.Year > 1990
                          select b).ToList();
            sw.Stop();
            long timeQ1Query = sw.ElapsedTicks;

            foreach (var b in q1Query)
            {
                Console.WriteLine($" • [{b.Year}] {b.Title}");
            }
            Console.WriteLine($" Час (Method Syntax): {timeQ1Method} тиків");
            Console.WriteLine($" Час (Query Syntax):  {timeQ1Query} тиків\n");

            // =========================================================================
            // ЗАПИТ 2: Об'єднання (Join) книг з авторами
            // =========================================================================
            Console.WriteLine("--- ЗАПИТ 2: Список усіх книг із зазначенням автора (Join) ---");

            // Query Syntax
            sw.Restart();
            var q2Query = (from b in books
                          join a in authors on b.AuthorId equals a.Id
                          select new { BookTitle = b.Title, AuthorName = a.Name }).ToList();
            sw.Stop();
            long timeQ2Query = sw.ElapsedTicks;

            // Method Syntax
            sw.Restart();
            var q2Method = books
                .Join(authors,
                      b => b.AuthorId,
                      a => a.Id,
                      (b, a) => new { BookTitle = b.Title, AuthorName = a.Name })
                .ToList();
            sw.Stop();
            long timeQ2Method = sw.ElapsedTicks;

            foreach (var item in q2Query.Take(5)) // виведемо перші 5 для лаконічності
            {
                Console.WriteLine($" • \"{item.BookTitle}\" — {item.AuthorName}");
            }
            Console.WriteLine($" ... (всього елементів: {q2Query.Count})");
            Console.WriteLine($" Час (Query Syntax):  {timeQ2Query} тиків");
            Console.WriteLine($" Час (Method Syntax): {timeQ2Method} тиків\n");

            // =========================================================================
            // ЗАПИТ 3: Кількість книг для кожного автора (GroupJoin / GroupBy)
            // =========================================================================
            Console.WriteLine("--- ЗАПИТ 3: Кількість книг для кожного автора ---");

            // Method Syntax
            sw.Restart();
            var q3Method = authors
                .Select(a => new
                {
                    AuthorName = a.Name,
                    BookCount = books.Count(b => b.AuthorId == a.Id)
                })
                .ToList();
            sw.Stop();
            long timeQ3Method = sw.ElapsedTicks;

            // Query Syntax
            sw.Restart();
            var q3Query = (from a in authors
                          join b in books on a.Id equals b.AuthorId into authorBooks
                          select new
                          {
                              AuthorName = a.Name,
                              BookCount = authorBooks.Count()
                          }).ToList();
            sw.Stop();
            long timeQ3Query = sw.ElapsedTicks;

            foreach (var item in q3Method)
            {
                Console.WriteLine($" • {item.AuthorName}: {item.BookCount} кн.");
            }
            Console.WriteLine($" Час (Method Syntax): {timeQ3Method} тиків");
            Console.WriteLine($" Час (Query Syntax):  {timeQ3Query} тиків\n");

            // =========================================================================
            // ЗАПИТ 4: Найстаріша книга (Агрегація / Пошук мінімуму)
            // =========================================================================
            Console.WriteLine("--- ЗАПИТ 4: Найстаріша книга в бібліотеці ---");

            // Method Syntax (Aggregate / MinBy)
            sw.Restart();
            int minYear = books.Min(b => b.Year);
            var q4Method = books.FirstOrDefault(b => b.Year == minYear);
            sw.Stop();
            long timeQ4Method = sw.ElapsedTicks;

            // Query Syntax
            sw.Restart();
            var q4Query = (from b in books
                          orderby b.Year ascending
                          select b).FirstOrDefault();
            sw.Stop();
            long timeQ4Query = sw.ElapsedTicks;

            if (q4Method != null)
            {
                var author = authors.FirstOrDefault(a => a.Id == q4Method.AuthorId);
                Console.WriteLine($" • \"{q4Method.Title}\" ({q4Method.Year} р.) — {author?.Name}");
            }
            Console.WriteLine($" Час (Min + FirstOrDefault): {timeQ4Method} тиків");
            Console.WriteLine($" Час (OrderBy + FirstOrDefault): {timeQ4Query} тиків\n");

            Console.WriteLine("=========================================================");
            Console.WriteLine("  ОБРОБКУ LINQ-ЗАПИТІВ УСПІШНО ЗАВЕРШЕНО");
            Console.WriteLine("=========================================================");
        }
    }
}
