using System;
using System.Collections.Generic;
using System.Text;

namespace Lab9v4
{
    // Custom Exception для помилок експорту
    public class ExportException : Exception
    {
        public ExportException(string message) : base(message) { }
    }

    // Базовий абстрактний клас
    public abstract class DataExporter
    {
        public string FilePath { get; set; }

        protected DataExporter(string filePath)
        {
            FilePath = filePath;
        }

        // Абстрактний метод експорту
        public abstract string Export(List<string> data);

        // Метод валідації вхідних даних
        protected virtual void Validate(List<string> data)
        {
            if (data == null || data.Count == 0)
            {
                throw new ExportException("Список даних порожній або дорівнює null.");
            }

            if (string.IsNullOrWhiteSpace(FilePath))
            {
                throw new ExportException("Шлях до файлу не вказано або він некоректний.");
            }
        }
    }

    // Похідний клас 1: CSV Exporter
    public class CSVExporter : DataExporter
    {
        public char Delimiter { get; set; }

        public CSVExporter(string filePath, char delimiter = ',') : base(filePath)
        {
            Delimiter = delimiter;
        }

        public override string Export(List<string> data)
        {
            Validate(data);

            // Симуляція помилки, якщо шлях містить "invalid"
            if (FilePath.Contains("invalid"))
            {
                throw new ExportException($"[CSV] Неможливо зберегти файл за шляхом: {FilePath}");
            }

            string result = string.Join(Delimiter.ToString(), data);
            return $"--- [CSV Exporter -> {FilePath}] ---\n{result}\n";
        }
    }

    // Похідний клас 2: JSON Exporter
    public class JSONExporter : DataExporter
    {
        public bool FormattedOutput { get; set; }

        public JSONExporter(string filePath, bool formattedOutput = true) : base(filePath)
        {
            FormattedOutput = formattedOutput;
        }

        public override string Export(List<string> data)
        {
            Validate(data);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"--- [JSON Exporter -> {FilePath}] ---");
            sb.AppendLine("[");
            for (int i = 0; i < data.Count; i++)
            {
                string indent = FormattedOutput ? "  " : "";
                string comma = (i < data.Count - 1) ? "," : "";
                sb.AppendLine($"{indent}\"{data[i]}\"{comma}");
            }
            sb.AppendLine("]");

            return sb.ToString();
        }
    }

    // Похідний клас 3: XML Exporter
    public class XMLExporter : DataExporter
    {
        public string RootNodeName { get; set; }

        public XMLExporter(string filePath, string rootNodeName = "Data") : base(filePath)
        {
            RootNodeName = rootNodeName;
        }

        public override string Export(List<string> data)
        {
            Validate(data);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"--- [XML Exporter -> {FilePath}] ---");
            sb.AppendLine($"<{RootNodeName}>");
            foreach (var item in data)
            {
                sb.AppendLine($"  <Item>{item}</Item>");
            }
            sb.AppendLine($"</{RootNodeName}>");

            return sb.ToString();
        }
    }

    // Сервісний клас для управління експортом
    public class ExportService
    {
        public void ExportAll(List<DataExporter> exporters, List<string> data)
        {
            Console.WriteLine("=== Запуск поліморфної обробки експорту ===\n");

            foreach (var exporter in exporters)
            {
                try
                {
                    // Поліморфний виклик
                    string result = exporter.Export(data);
                    Console.WriteLine(result);
                }
                catch (ExportException ex)
                {
                    Console.WriteLine($"[ПОМИЛКА ЕКСПОРТУ] ({exporter.GetType().Name}): {ex.Message}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[КРИТИЧНА ПОМИЛКА] ({exporter.GetType().Name}): {ex.Message}\n");
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Набір коректних даних
            List<string> sampleData = new List<string>
            {
                "Apple",
                "Banana",
                "Cherry",
                "Date"
            };

            // Створення списку базового типу (DataExporter) з різними реалізаціями
            List<DataExporter> exporters = new List<DataExporter>
            {
                new CSVExporter("reports/data.csv", ';'),
                new JSONExporter("reports/data.json", formattedOutput: true),
                new XMLExporter("reports/data.xml", "Products"),
                
                // Експортер з помилковим шляхом для демонстрації catch (винятку)
                new CSVExporter("invalid_path/data.csv")
            };

            ExportService service = new ExportService();

            // 1. Успішна демонстрація + обробка винятку на 4-му елементі
            service.ExportAll(exporters, sampleData);

            // 2. Демонстрація валідації порожніх даних
            Console.WriteLine("=== Перевірка валідації порожнього списку ===");
            List<string> emptyData = new List<string>();
            service.ExportAll(new List<DataExporter> { new JSONExporter("empty.json") }, emptyData);
        }
    }
}