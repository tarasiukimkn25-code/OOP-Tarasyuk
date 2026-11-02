using System;
using System.Collections.Generic;
using System.Text;

namespace Lab14App
{
    // =========================================================================
    // 1. КЛАС СУТНОСТІ (WebRequestData)
    // =========================================================================
    public class WebRequestData
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public DateTime Timestamp { get; set; }

        public WebRequestData(string url, string method = "GET")
        {
            Url = url;
            Method = method;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {Method} {Url}";
        }
    }

    // =========================================================================
    // 2. СЕРВІС ОБРОБКИ ВЕБ-ЗАПИТІВ (WebRequestProcessor)
    // =========================================================================
    public class WebRequestProcessor
    {
        // HashSet для швидкої перевірки та збереження унікальних URL (O(1))
        private readonly HashSet<string> _uniqueUrls = new HashSet<string>();

        // Queue для обробки запитів у порядку їх надходження (FIFO)
        private readonly Queue<WebRequestData> _requestQueue = new Queue<WebRequestData>();

        // Stack для моделювання історії навігації браузеру (LIFO - Back button)
        private readonly Stack<string> _navigationHistory = new Stack<string>();

        // --- РОБОТА З HASHSET (Унікальні URL) ---
        public bool RegisterUrl(string url)
        {
            bool isAdded = _uniqueUrls.Add(url);
            if (isAdded)
            {
                Console.WriteLine($" [HashSet] URL додано до унікальних: {url}");
            }
            else
            {
                Console.WriteLine($" [HashSet] Дублікат! URL вже відвідувався: {url}");
            }
            return isAdded;
        }

        public bool IsUrlVisited(string url)
        {
            return _uniqueUrls.Contains(url);
        }

        // --- РОБОТА З QUEUE (Черга запитів - FIFO) ---
        public void EnqueueRequest(WebRequestData request)
        {
            _requestQueue.Enqueue(request);
            Console.WriteLine($" [Queue] Запит додано в чергу: {request}");
            RegisterUrl(request.Url);
        }

        public void ProcessNextRequest()
        {
            if (_requestQueue.Count > 0)
            {
                WebRequestData currentRequest = _requestQueue.Dequeue();
                Console.WriteLine($" [Queue] Оброблено запит: {currentRequest}");
                
                // Після обробки сторінки додаємо її в історію навігації
                _navigationHistory.Push(currentRequest.Url);
            }
            else
            {
                Console.WriteLine(" [Queue] Черга запитів порожня!");
            }
        }

        // --- РОБОТА З STACK (Історія навігації - LIFO / Back button) ---
        public void NavigateBack()
        {
            if (_navigationHistory.Count > 1)
            {
                string previousPage = _navigationHistory.Pop();
                string currentPage = _navigationHistory.Peek();
                Console.WriteLine($" [Stack] Повернення назад (Back) з '{previousPage}' на '{currentPage}'");
            }
            else if (_navigationHistory.Count == 1)
            {
                string lastPage = _navigationHistory.Pop();
                Console.WriteLine($" [Stack] Закрито останню сторінку '{lastPage}'. Історія порожня.");
            }
            else
            {
                Console.WriteLine(" [Stack] Історія навігації порожня!");
            }
        }

        public string GetCurrentPage()
        {
            return _navigationHistory.Count > 0 ? _navigationHistory.Peek() : "Початкова сторінка";
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
            Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №14. HASHSET / QUEUE / STACK (ВЕБ-ЗАПИТИ)");
            Console.WriteLine("==========================================================================\n");

            WebRequestProcessor processor = new WebRequestProcessor();

            // 1. Демонстрація Queue та HashSet
            Console.WriteLine("--- 1. ДОДАВАННЯ ВЕБ-ЗАПИТІВ У ЧЕРГУ (QUEUE + HASHSET) ---");
            processor.EnqueueRequest(new WebRequestData("https://example.com/home"));
            processor.EnqueueRequest(new WebRequestData("https://example.com/products"));
            processor.EnqueueRequest(new WebRequestData("https://example.com/home")); // Дублікат URL
            processor.EnqueueRequest(new WebRequestData("https://example.com/cart"));
            Console.WriteLine();

            // 2. Послідовна обробка черги (FIFO)
            Console.WriteLine("--- 2. ПОСЛІДОВНА ОБРОБКА ЗАПИТІВ (QUEUE - FIFO) ---");
            processor.ProcessNextRequest();
            processor.ProcessNextRequest();
            processor.ProcessNextRequest();
            Console.WriteLine($"Поточна активна сторінка: {processor.GetCurrentPage()}\n");

            // 3. Перевірка наявності через HashSet
            Console.WriteLine("--- 3. ПЕРЕВІРКА УНІКАЛЬНИХ URL В HASHSET (O(1)) ---");
            string testUrl1 = "https://example.com/products";
            string testUrl2 = "https://example.com/contact";
            Console.WriteLine($"Чи відвідували '{testUrl1}'? {processor.IsUrlVisited(testUrl1)}");
            Console.WriteLine($"Чи відвідували '{testUrl2}'? {processor.IsUrlVisited(testUrl2)}\n");

            // 4. Демонстрація історії навігації через Stack (LIFO - Back)
            Console.WriteLine("--- 4. НАВІГАЦІЯ НАЗАД КНОПКОЮ BACK (STACK - LIFO) ---");
            processor.NavigateBack();
            processor.NavigateBack();
            processor.NavigateBack();

            Console.WriteLine("\n==========================================================================");
            Console.WriteLine("  ОБРОБКУ СПЕЦІАЛІЗОВАНИХ КОЛЕКЦІЙ УСПІШНО ЗАВЕРШЕНО");
            Console.WriteLine("==========================================================================");
        }
    }
}