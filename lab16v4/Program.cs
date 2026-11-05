using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab16App
{
    // =========================================================================
    // 1. КЛАСИ СУТНОСТЕЙ (Project, Employee, Company)
    // =========================================================================
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public Project(int id, string title)
        {
            Id = id;
            Title = title;
        }

        public override string ToString() => $"Проєкт #{Id}: \"{Title}\"";
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Project> Projects { get; set; } = new List<Project>();

        public Employee(int id, string name, List<Project> projects)
        {
            Id = id;
            Name = name;
            Projects = projects;
        }
    }

    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();

        public Company(int id, string name, List<Employee> employees)
        {
            Id = id;
            Name = name;
            Employees = employees;
        }
    }

    // =========================================================================
    // 2. МЕТОДИ РОЗШИРЕННЯ (Custom Extension Methods)
    // =========================================================================
    public static class CompanyExtensions
    {
        // 3. Custom Extension: Метод TotalEmployees() для IEnumerable<Company>
        public static int TotalEmployees(this IEnumerable<Company> companies)
        {
            if (companies == null) return 0;
            return companies.Sum(c => c.Employees?.Count ?? 0);
        }
    }

    // =========================================================================
    // 3. ГОЛОВНА ПРОГРАМА (Program)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("==========================================================================");
            Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №16. LINQ: SELECTMANY, ПАГІНАЦІЯ Й CUSTOM EXTENSIONS");
            Console.WriteLine("==========================================================================\n");

            // --- Генерація тестових даних ---
            var p1 = new Project(1, "E-Commerce Platform");
            var p2 = new Project(2, "Mobile Banking App");
            var p3 = new Project(3, "AI Chatbot Engine");
            var p4 = new Project(4, "CRM Cloud System");
            var p5 = new Project(5, "CyberSecurity Scanner");
            var p6 = new Project(6, "Logistics Tracker");

            List<Company> companies = new List<Company>
            {
                new Company(1, "Global Tech Solutions", new List<Employee>
                {
                    new Employee(101, "Олексій Іванов", new List<Project> { p1, p2 }),
                    new Employee(102, "Марія Коваленко", new List<Project> { p2, p3 })
                }),
                new Company(2, "SoftServe Innovations", new List<Employee>
                {
                    new Employee(201, "Дмитро Сидоренко", new List<Project> { p3, p4 }),
                    new Employee(202, "Олена Бондаренко", new List<Project> { p4, p5 }),
                    new Employee(203, "Тарас Шевченко", new List<Project> { p1, p5 })
                }),
                new Company(3, "DataArt Systems", new List<Employee>
                {
                    new Employee(301, "Іван Петренко", new List<Project> { p6 }),
                    new Employee(302, "Анна Ткаченко", new List<Project> { p2, p6 })
                }),
                new Company(4, "EPAM Digital", new List<Employee>
                {
                    new Employee(401, "Максим Кравченко", new List<Project> { p1, p3, p5 })
                }),
                new Company(5, "Genesis Tech", new List<Employee>
                {
                    new Employee(501, "Юлія Бойко", new List<Project> { p4 }),
                    new Employee(502, "Роман Мельник", new List<Project> { p5, p6 })
                })
            };

            // =========================================================================
            // ЗАПИТ 1: SelectMany — Розгортання вкладених колекцій проєктів
            // =========================================================================
            Console.WriteLine("--- 1. SELECTMANY: Усі проєкти всіх співробітників усіх компаній ---");
            
            // Дворазовий SelectMany для розгортання: Company -> Employees -> Projects
            var allProjects = companies
                .SelectMany(c => c.Employees)
                .SelectMany(e => e.Projects)
                .DistinctBy(p => p.Id) // Унікальні проєкти
                .ToList();

            foreach (var proj in allProjects)
            {
                Console.WriteLine($" • {proj}");
            }
            Console.WriteLine($" Всього унікальних проєктів: {allProjects.Count}\n");

            // =========================================================================
            // ЗАПИТ 2: Пагінація — Посторінковий вивід (Skip / Take)
            // =========================================================================
            Console.WriteLine("--- 2. ПАГІНАЦІЯ: Посторінковий вивід компаній (по 2 на сторінку) ---");
            
            int pageSize = 2;
            int totalCompanies = companies.Count;
            int totalPages = (int)Math.Ceiling((double)totalCompanies / pageSize);

            for (int page = 1; page <= totalPages; page++)
            {
                Console.WriteLine($"\n [Сторінка {page} з {totalPages}]:");
                var pagedCompanies = companies
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                foreach (var comp in pagedCompanies)
                {
                    Console.WriteLine($"  - [{comp.Id}] {comp.Name} (Співробітників: {comp.Employees.Count})");
                }
            }
            Console.WriteLine();

            // =========================================================================
            // ЗАПИТ 3: Custom Extension — Виклик TotalEmployees()
            // =========================================================================
            Console.WriteLine("\n--- 3. CUSTOM EXTENSION: Обчислення загальної кількості співробітників ---");
            
            int totalEmp = companies.TotalEmployees();
            Console.WriteLine($" Загальна кількість співробітників у всіх компаніях: {totalEmp}");

            Console.WriteLine("\n==========================================================================");
            Console.WriteLine("  ОБРОБКУ LINQ-ЗАПИТІВ ТА EXTENSION-МЕТОДІВ УСПІШНО ЗАВЕРШЕНО");
            Console.WriteLine("==========================================================================");
        }
    }
}