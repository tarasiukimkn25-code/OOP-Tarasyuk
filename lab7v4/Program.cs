using System;
using System.Collections.Generic;

namespace lab7v4
{
    // ==========================================
    // Базовий клас: Animal
    // ==========================================
    public class Animal
    {
        public string Name { get; set; }

        public Animal(string name)
        {
            Name = name;
        }

        // Віртуальний метод для дихання
        public virtual void Breathe()
        {
            Console.WriteLine($"{Name} (Animal) дихає повітрям через загальні органи дихання.");
        }
    }

    // ==========================================
    // Похідний клас 1: Mammal (використовує override)
    // ==========================================
    public class Mammal : Animal
    {
        public Mammal(string name) : base(name) { }

        // Перевизначення (override) — забезпечує поліморфізм
        public override void Breathe()
        {
            Console.WriteLine($"{Name} (Mammal) дихає легенями (перевизначено через override).");
        }
    }

    // ==========================================
    // Похідний клас 2: Bird (використовує new)
    // ==========================================
    public class Bird : Animal
    {
        public Bird(string name) : base(name) { }

        // Приховування (new) — створює новий метод, не бере участі в поліморфізмі
        public new void Breathe()
        {
            Console.WriteLine($"{Name} (Bird) дихає за допомогою легень та повітряних мішків (приховано через new).");
        }
    }

    // ==========================================
    // Головний клас програми
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №7 (Варіант 4) ===");
            Console.WriteLine("=== Тема: Детальне дослідження Override та New ===\n");

            // 1. Створення об'єктів через власні типи
            Mammal mammal = new Mammal("Ссавець (Лев)");
            Bird bird = new Bird("Птах (Орел)");

            Console.WriteLine("--- 1. Виклик методів через прямі посилання (Mammal та Bird) ---");
            mammal.Breathe();
            bird.Breathe();
            Console.WriteLine();

            // 2. Виклик методів через посилання базового типу Animal
            Console.WriteLine("--- 2. Виклик методів через посилання типу Animal ---");
            Animal animalRefToMammal = mammal;
            Animal animalRefToBird = bird;

            // Mammal: викликається Mammal.Breathe(), бо використано override (динамічний зв'язок)
            Console.Write("[Animal ref -> Mammal]: ");
            animalRefToMammal.Breathe();

            // Bird: викликається Animal.Breathe(), бо використано new (статичний зв'язок за типом посилання)
            Console.Write("[Animal ref -> Bird]: ");
            animalRefToBird.Breathe();
            Console.WriteLine();

            // 3. Явне приведення типів для Bird
            Console.WriteLine("--- 3. Виклик прихованого методу Bird через явне приведення типів ---");
            Console.Write("[((Bird)animalRefToBird)]: ");
            ((Bird)animalRefToBird).Breathe();
            Console.WriteLine();

            // 4. Демонстрація у колекції List<Animal> (Поліморфна поведінка)
            Console.WriteLine("--- 4. Перебір у колекції List<Animal> ---");
            List<Animal> animals = new List<Animal>
            {
                new Animal("Звичайна тварина"),
                mammal,
                bird
            };

            foreach (var a in animals)
            {
                a.Breathe();
            }
        }
    }
}