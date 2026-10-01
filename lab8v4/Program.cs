using System;
using System.Collections.Generic;

namespace Lab8v4
{
    // Базовий клас Animal
    public class Animal
    {
        public string Name { get; set; }

        public Animal(string name)
        {
            Name = name;
        }

        // Віртуальний метод базового класу
        public virtual void Breathe()
        {
            Console.WriteLine($"[Animal]  {Name} дихає базово (легенями/зябрами).");
        }
    }

    // Похідний клас Mammal (Ссавець) — перевизначення (override)
    public class Mammal : Animal
    {
        public Mammal(string name) : base(name) { }

        // Перевизначення віртуального методу (активує динамічне зв'язування)
        public override void Breathe()
        {
            Console.WriteLine($"[Mammal]  {Name} (ссавець) дихає легенями за допомогою діафрагми.");
        }
    }

    // Похідний клас Bird (Птах) — приховування (new)
    public class Bird : Animal
    {
        public Bird(string name) : base(name) { }

        // Приховування методу (new) — динамічне зв'язування відключається для посилань Animal
        public new void Breathe()
        {
            Console.WriteLine($"[Bird]    {Name} (птах) дихає за допомогою легень та повітряних мішків.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Створення колекції об'єктів базового типу List<Animal>
            List<Animal> zoo = new List<Animal>
            {
                new Animal("Амеба"),
                new Mammal("Лев"),
                new Bird("Орел"),
                new Mammal("Дельфін"),
                new Bird("Сокіл"),
                new Mammal("Кіт")
            };

            Console.WriteLine("=== 1. ПОЛІМОРФНИЙ ВИКЛИК ЧЕРЕЗ КОЛЕКЦІЮ List<Animal> ===");
            
            // Лічильники для агрегації результатів
            int totalAnimals = zoo.Count;
            int mammalCount = 0;
            int birdCount = 0;
            int baseAnimalCount = 0;

            // 2. Обхід колекції в циклі та виклик Breathe()
            foreach (var animal in zoo)
            {
                // Поліморфний виклик:
                // - Для Mammal спрацює Mammal.Breathe() (бо override)
                // - Для Bird спрацює Animal.Breathe() (бо new НЕ забезпечує поліморфізму через посилання Animal!)
                animal.Breathe();

                // Агрегація за типами об'єктів
                if (animal is Mammal)
                    mammalCount++;
                else if (animal is Bird)
                    birdCount++;
                else
                    baseAnimalCount++;
            }

            Console.WriteLine("\n=== 2. ДЕМОНСТРАЦІЯ РІЗНИЦІ МІЖ OVERRIDE ТА NEW ===");
            
            Bird myBird = new Bird("Фламінго");
            Animal birdAsAnimal = myBird; // Приведення до базового типу

            Console.Write("Виклик через посилання Bird:   ");
            myBird.Breathe();         // Викличе Bird.Breathe()

            Console.Write("Виклик через посилання Animal: ");
            birdAsAnimal.Breathe();   // Викличе Animal.Breathe(), бо 'new' приховав метод лише для типу Bird!

            Console.WriteLine("\n=== 3. АГРЕГАЦІЯ РЕЗУЛЬТАТІВ ===");
            Console.WriteLine($"Загальна кількість тварин у списку: {totalAnimals}");
            Console.WriteLine($"• Ссавців (Mammal - override):     {mammalCount}");
            Console.WriteLine($"• Птахів (Bird - new):              {birdCount}");
            Console.WriteLine($"• Базових тварин (Animal):          {baseAnimalCount}");
        }
    }
}
