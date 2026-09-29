using System;
using System.Collections.Generic;

namespace lab6v4
{
    // ==========================================
    // Базовий клас: Animal
    // ==========================================
    public class Animal
    {
        // Приватні поля
        private string name;
        private int age;

        // Публічні властивості
        public string Name
        {
            get => name;
            set => name = value;
        }

        public int Age
        {
            get => age;
            set => age = value >= 0 ? value : 0;
        }

        // Конструктор базового класу
        public Animal(string name, int age)
        {
            Name = name;
            Age = age;
        }

        // Віртуальний метод для перевизначення (override)
        public virtual void MakeSound()
        {
            Console.WriteLine($"{Name} видає невизначений звук.");
        }

        // Звичайний метод для демонстрації приховування (new)
        public string GetSpecies()
        {
            return "Тварина (Animal)";
        }
    }

    // ==========================================
    // Похідний клас: Dog
    // ==========================================
    public class Dog : Animal
    {
        public string Breed { get; set; }

        // Конструктор, що викликає базовий за допомогою base(...)
        public Dog(string name, int age, string breed) : base(name, age)
        {
            Breed = breed;
        }

        // Перевизначення віртуального методу
        public override void MakeSound()
        {
            Console.WriteLine($"{Name} (Собака, порода: {Breed}) лає: Гав-гав!");
        }

        // Унікальний метод класу Dog
        public void Fetch()
        {
            Console.WriteLine($"{Name} приносить кинуту паличку!");
        }

        // Приховування (new) методу базового класу
        public new string GetSpecies()
        {
            return "Собака (Dog)";
        }
    }

    // ==========================================
    // Похідний клас: Cat
    // ==========================================
    public class Cat : Animal
    {
        public bool IsIndoor { get; set; }

        // Конструктор, що викликає базовий за допомогою base(...)
        public Cat(string name, int age, bool isIndoor) : base(name, age)
        {
            IsIndoor = isIndoor;
        }

        // Перевизначення віртуального методу
        public override void MakeSound()
        {
            string location = IsIndoor ? "домашній" : "вуличний";
            Console.WriteLine($"{Name} (Кіт, {location}) нявкає: Мяу-мяу!");
        }

        // Унікальний метод класу Cat
        public void Purr()
        {
            Console.WriteLine($"{Name} муркоче: Мррр-мррр...");
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
            Console.WriteLine("=== Лабораторна робота №6 (Варіант 4) ===");
            Console.WriteLine("=== Тема: Наслідування та Поліморфізм ===\n");

            // 1. Створення об'єктів
            Animal genericAnimal = new Animal("Невідома істота", 5);
            Dog dog = new Dog("Рекс", 3, "Німецька вівчарка");
            Cat cat = new Cat("Мурчик", 2, true);

            // 2. Демонстрація унікальних методів похідних класів
            Console.WriteLine("--- 1. Викликаємо унікальні методи ---");
            dog.Fetch();
            cat.Purr();
            Console.WriteLine();

            // 3. Демонстрація поліморфізму (override)
            Console.WriteLine("--- 2. Демонстрація поліморфізму (override) ---");
            List<Animal> animals = new List<Animal> { genericAnimal, dog, cat };

            foreach (var animal in animals)
            {
                animal.MakeSound();
            }
            Console.WriteLine();

            // 4. Демонстрація різниці між override та new
            Console.WriteLine("--- 3. Демонстрація різниці між override та new ---");
            
            Console.WriteLine($"[Dog reference] dog.GetSpecies(): {dog.GetSpecies()}");

            Animal animalRefToDog = dog;
            Console.WriteLine($"[Animal reference to Dog] animalRefToDog.GetSpecies(): {animalRefToDog.GetSpecies()}");
            
            Console.WriteLine("\nПояснення:");
            Console.WriteLine("- При 'override' викликається метод фактичного об'єкта в пам'яті.");
            Console.WriteLine("- При 'new' метод приховується, і його виклик залежить від типу ПОСИЛАННЯ (Animal викликає Animal.GetSpecies(), а Dog викликає Dog.GetSpecies()).");
        }
    }
}