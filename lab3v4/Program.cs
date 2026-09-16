using System;
using System.Collections.Generic;

namespace Lab3
{
    // Клас MemoryCache із реалізацією паттерну Dispose (IDisposable)
    public class MemoryCache : IDisposable
    {
        private bool _disposed = false;
        private Dictionary<string, string> _cacheData;
        private bool _isActive;

        public bool IsActive => _isActive;
        public int Count => _cacheData != null ? _cacheData.Count : 0;

        public MemoryCache()
        {
            _cacheData = new Dictionary<string, string>();
            _isActive = true;
            Console.WriteLine("[MemoryCache] Кеш успішно ініціалізовано. Ресурс виділено.");
        }

        public void Set(string key, string value)
        {
            CheckDisposed();
            _cacheData[key] = value;
            Console.WriteLine($"[MemoryCache] Записано: '{key}' = '{value}'");
        }

        public string Get(string key)
        {
            CheckDisposed();
            if (_cacheData.TryGetValue(key, out var value))
            {
                return value;
            }
            Console.WriteLine($"[MemoryCache] Ключ '{key}' не знайдено.");
            return null;
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MemoryCache), "Помилка: спроба звернутися до очищеного кешу!");
            }
        }

        // Паттерн Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Звільнення керованих ресурсів
                    Console.WriteLine("[Dispose] Очищення керованих ресурсів: очищаємо _cacheData.");
                    if (_cacheData != null)
                    {
                        _cacheData.Clear();
                        _cacheData = null;
                    }
                }

                // Звільнення некерованих ресурсів / деактивація
                if (_isActive)
                {
                    Console.WriteLine("[Dispose] Звільнення ресурсу: деактивація _isActive = false.");
                    _isActive = false;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MemoryCache()
        {
            Console.WriteLine("[~MemoryCache] Деструктор: автоматичне звільнення некерованих ресурсів через GC.");
            Dispose(false);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== 1. Демонстрація роботи з конструкцією using ===");
            using (MemoryCache cache1 = new MemoryCache())
            {
                cache1.Set("user_1", "Олена");
                cache1.Set("user_2", "Іван");
                Console.WriteLine($"Отримано з кешу user_1: {cache1.Get("user_1")}");
            } // Тут автоматично викликається Dispose()
            Console.WriteLine("Кеш 1 вийшов з блоку using та був очищений.");
            Console.WriteLine();

            Console.WriteLine("=== 2. Демонстрація явного виклику Dispose() ===");
            MemoryCache cache2 = new MemoryCache();
            cache2.Set("session_token", "ABC-12345");
            Console.WriteLine($"Отримано з кешу session_token: {cache2.Get("session_token")}");
            Console.WriteLine("Явно викликаємо cache2.Dispose()...");
            cache2.Dispose();
            Console.WriteLine();

            Console.WriteLine("=== 3. Демонстрація роботи деструктора через GC.Collect() ===");
            CreateUnmanagedCache();

            Console.WriteLine("Об'єкт втратив посилання. Викликаємо GC.Collect()...");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\n=== Виконання програми завершено ===");
        }

        // Метод для створення об'єкта без явного Dispose, щоб спрацював деструктор
        static void CreateUnmanagedCache()
        {
            MemoryCache cache3 = new MemoryCache();
            cache3.Set("temp_key", "temp_value");
            // Dispose() свідомо НЕ викликається
        }
    }
}
