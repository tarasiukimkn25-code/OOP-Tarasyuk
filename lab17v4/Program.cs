using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Lab17App
{
    // =========================================================================
    // 1. УЗАГАЛЬНЕНИЙ ДОПОМІЖНИЙ КЛАС ПАТЕРНУ RETRY (RetryHelper)
    // =========================================================================
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool>? shouldRetry = null)
        {
            if (initialDelay == default)
            {
                initialDelay = TimeSpan.FromMilliseconds(200);
            }

            int attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;
                    return operation();
                }
                catch (Exception ex)
                {
                    // Перевіряємо, чи вичерпано кількість спроб або чи підходить виняток для повтору
                    bool canRetry = attempt <= retryCount && (shouldRetry == null || shouldRetry(ex));

                    if (!canRetry)
                    {
                        Console.WriteLine($" [RetryHelper] Помилка не підлягає повтору або вичерпано ліміт спроб ({attempt - 1}/{retryCount}).");
                        throw;
                    }

                    // Експоненційна затримка: initialDelay * 2^(attempt - 1)
                    double delayMs = initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1);
                    TimeSpan currentDelay = TimeSpan.FromMilliseconds(delayMs);

                    Console.WriteLine($" [RetryHelper] Спроба #{attempt} невдала ({ex.GetType().Name}: {ex.Message}). " +
                                      $"Повтор через {currentDelay.TotalMilliseconds} мс...");

                    Thread.Sleep(currentDelay);
                }
            }
        }
    }

    // =========================================================================
    // 2. КЛАС ІМІТАЦІЇ РОБОТИ З ФАЙЛАМИ (FileProcessor)
    // =========================================================================
    public class FileProcessor
    {
        private int _readAttempts = 0;

        // Імітує FileNotFoundException перші 3 рази, на 4-й — успіх
        public byte[] ReadImage(string path)
        {
            _readAttempts++;
            Console.WriteLine($"  -> [FileProcessor] Спроба #{_readAttempts} читання файлу '{path}'...");

            if (_readAttempts <= 3)
            {
                throw new FileNotFoundException($"Файл тимчасово недоступний або ще завантажується: {path}");
            }

            Console.WriteLine($"  -> [FileProcessor] Файл '{path}' успішно прочитано!");
            return new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // Заголовок PNG
        }

        public void Reset() => _readAttempts = 0;
    }

    // =========================================================================
    // 3. КЛАС ІМІТАЦІЇ МЕРЕЖЕВИХ ЗАПИТІВ (NetworkClient)
    // =========================================================================
    public class NetworkClient
    {
        private int _downloadAttempts = 0;

        // Імітує HttpRequestException перші 2 рази, на 3-й — успіх
        public byte[] DownloadImage(string url)
        {
            _downloadAttempts++;
            Console.WriteLine($"  -> [NetworkClient] Спроба #{_downloadAttempts} завантаження з '{url}'...");

            if (_downloadAttempts <= 2)
            {
                throw new HttpRequestException($"503 Service Unavailable при зверненні до {url}");
            }

            Console.WriteLine($"  -> [NetworkClient] Зображення з '{url}' успішно завантажено!");
            return new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10 }; // Заголовок JPEG
        }

        // Метод для демонстрації критичної помилки (яку не варто повторювати)
        public byte[] DownloadWithInvalidUrl(string url)
        {
            Console.WriteLine($"  -> [NetworkClient] Спроба запиту на некоректну адресу '{url}'...");
            throw new ArgumentException("Некоректний формат URL-адреси");
        }
    }

    // =========================================================================
    // 4. ДЕМОНСТРАЦІЯ РОБОТИ (Program)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("==========================================================================");
            Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №17. ОБРОБКА IO/МЕРЕЖЕВИХ ПОМИЛОК ТА ПАТЕРН RETRY");
            Console.WriteLine("==========================================================================\n");

            // Делегат умови повтору згідно з варіантом (FileNotFoundException та HttpRequestException)
            Func<Exception, bool> shouldRetryPolicy = ex =>
                ex is FileNotFoundException || ex is HttpRequestException;

            // -------------------------------------------------------------------------
            // СЦЕНАРІЙ 1: Завантаження зображення з мережі (2 невдачі -> успіх на 3-й)
            // -------------------------------------------------------------------------
            Console.WriteLine("--- СЦЕНАРІЙ 1: NetworkClient.DownloadImage (2 помилки -> успіх) ---");
            NetworkClient networkClient = new NetworkClient();

            try
            {
                byte[] downloadedBytes = RetryHelper.ExecuteWithRetry(
                    operation: () => networkClient.DownloadImage("https://cdn.example.com/avatar.jpg"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: shouldRetryPolicy
                );
                Console.WriteLine($" РЕЗУЛЬТАТ: Отримано {downloadedBytes.Length} байт зображення.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" КРИТИЧНА ПОМИЛКА: {ex.Message}\n");
            }

            // -------------------------------------------------------------------------
            // СЦЕНАРІЙ 2: Читання локального файлу (3 невдачі -> успіх на 4-й спробі)
            // -------------------------------------------------------------------------
            Console.WriteLine("--- СЦЕНАРІЙ 2: FileProcessor.ReadImage (3 помилки -> успіх на 4-й) ---");
            FileProcessor fileProcessor = new FileProcessor();

            try
            {
                byte[] fileBytes = RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.ReadImage("C:/images/banner.png"),
                    retryCount: 4, // Потрібно мінімум 3 повтори після першої невдалої спроби
                    initialDelay: TimeSpan.FromMilliseconds(150),
                    shouldRetry: shouldRetryPolicy
                );
                Console.WriteLine($" РЕЗУЛЬТАТ: Прочитано {fileBytes.Length} байт файлу.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" КРИТИЧНА ПОМИЛКА: {ex.Message}\n");
            }

            // -------------------------------------------------------------------------
            // СЦЕНАРІЙ 3: Вибіркова фільтрація через shouldRetry (не повторюємо ArgumentException)
            // -------------------------------------------------------------------------
            Console.WriteLine("--- СЦЕНАРІЙ 3: Ігнорування повторів для невідповідних винятків ---");
            try
            {
                RetryHelper.ExecuteWithRetry(
                    operation: () => networkClient.DownloadWithInvalidUrl("invalid-url"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: shouldRetryPolicy
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ПЕРЕХОПЛЕНО В MAIN (без зайвих повторів): {ex.GetType().Name} — {ex.Message}\n");
            }
            finally
            {
                Console.WriteLine(" [Finally] Звільнення ресурсів та завершення роботи клієнтів.");
            }

            Console.WriteLine("\n==========================================================================");
            Console.WriteLine("  ДЕМОНСТРАЦІЮ ПАТЕРНУ RETRY УСПІШНО ЗАВЕРШЕНО");
            Console.WriteLine("==========================================================================");
        }
    }
}