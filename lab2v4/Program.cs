using System;

namespace Lab2
{
    public class Teacher
    {
        private string _name;
        private string _subject;
        private int _experienceYears;

        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? "Unknown Teacher" : value;
        }

        public string Subject
        {
            get => _subject;
            set => _subject = string.IsNullOrWhiteSpace(value) ? "General" : value;
        }

        public int ExperienceYears
        {
            get => _experienceYears;
            set
            {
                if (value < 0)
                {
                    Console.WriteLine("Помилка: досвід не може бути від'ємним! Встановлено 0.");
                    _experienceYears = 0;
                }
                else
                {
                    _experienceYears = value;
                }
            }
        }

        public Teacher() : this("New Teacher", "General", 0)
        {
            Console.WriteLine("-> Викликано конструктор за замовчуванням");
        }

        public Teacher(string name, string subject, int experienceYears)
        {
            Name = name;
            Subject = subject;
            ExperienceYears = experienceYears;
            Console.WriteLine($"-> Викликано параметризований конструктор для {Name}");
        }

        public void Introduce()
        {
            Console.WriteLine($"[Викладач] Ім'я: {Name} | Предмет: {Subject} | Стаж: {ExperienceYears} років");
        }

        ~Teacher()
        {
            Console.WriteLine($"[Деструктор] Об'єкт викладача '{_name}' знищено з пам'яті.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Creating objects ===");
            
            Teacher teacher1 = new Teacher();
            teacher1.Introduce();

            Console.WriteLine();

            Teacher teacher2 = new Teacher("Олена Петрівна", "ООП (C#)", 8);
            teacher2.Introduce();

            Console.WriteLine();

            Teacher teacher3 = new Teacher("Іван Васильович", "Математика", -5);
            teacher3.Introduce();

            Console.WriteLine("=== Objects created ===");
            Console.WriteLine();

            teacher1 = null;
            teacher2 = null;
            teacher3 = null;

            Console.WriteLine("=== End of Main, preparing for GC ===");
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}