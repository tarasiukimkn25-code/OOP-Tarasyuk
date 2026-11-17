using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
var random = new Random(42);
var categories = new[] { "Електроніка", "Побут", "Канцелярія", "Спорт", "Книги" };
var cities = new[] { "Рівне", "Львів", "Київ", "Одеса", "Харків" };

List<Product> products = Enumerable.Range(1, 100)
    .Select(id => new Product(id, $"Товар {id:000}", categories[(id - 1) % categories.Length], 100m + random.Next(1, 9000) / 10m))
    .ToList();
List<Customer> customers = Enumerable.Range(1, 100)
    .Select(id => new Customer(id, $"Клієнт {id:000}", cities[(id - 1) % cities.Length]))
    .ToList();
List<Sale> sales = Enumerable.Range(1, 5000)
    .Select(id => new Sale(
        id,
        random.Next(1, products.Count + 1),
        random.Next(1, 8),
        new DateTime(2024, 1, 1).AddDays(random.Next(0, 730)),
        random.Next(1, customers.Count + 1)))
    .ToList();

Console.WriteLine("Самостійна робота №12. LINQ-процеси для ETL-аналітики продажів");
Console.WriteLine($"Товарів: {products.Count}; клієнтів: {customers.Count}; продажів: {sales.Count}\n");

RunAndReport("1. Топ-3 товарів у кожній категорії за кількістю продажів", () =>
    sales.Join(products, sale => sale.ProductId, product => product.Id,
            (sale, product) => new { product.Id, product.Name, product.Category, sale.Quantity })
        .GroupBy(item => item.Category)
        .SelectMany(categoryGroup => categoryGroup
            .GroupBy(item => new { item.Id, item.Name })
            .Select(itemGroup => new
            {
                Category = categoryGroup.Key,
                Product = itemGroup.Key.Name,
                UnitsSold = itemGroup.Sum(item => item.Quantity)
            })
            .OrderByDescending(item => item.UnitsSold)
            .ThenBy(item => item.Product)
            .Take(3)));

RunAndReport("2. Дохід за містами клієнтів", () =>
    sales.Join(products, sale => sale.ProductId, product => product.Id, (sale, product) => new { sale, product })
        .Join(customers, item => item.sale.CustomerId, customer => customer.Id,
            (item, customer) => new { customer.City, Revenue = item.sale.Quantity * item.product.UnitPrice })
        .GroupBy(item => item.City)
        .Select(group => new { City = group.Key, Revenue = group.Sum(item => item.Revenue) })
        .OrderByDescending(item => item.Revenue));

RunAndReport("3. Продажі за роком і місяцем", () =>
    sales.Join(products, sale => sale.ProductId, product => product.Id,
            (sale, product) => new { sale.SaleDate, sale.Quantity, Revenue = sale.Quantity * product.UnitPrice })
        .GroupBy(item => new { item.SaleDate.Year, item.SaleDate.Month })
        .Select(group => new
        {
            group.Key.Year,
            group.Key.Month,
            Units = group.Sum(item => item.Quantity),
            Revenue = group.Sum(item => item.Revenue)
        })
        .OrderBy(item => item.Year)
        .ThenBy(item => item.Month));

RunAndReport("4. Дохід за категорією та містом (складний запит)", () =>
    sales.Join(products, sale => sale.ProductId, product => product.Id, (sale, product) => new { sale, product })
        .Join(customers, item => item.sale.CustomerId, customer => customer.Id,
            (item, customer) => new
            {
                item.product.Category,
                customer.City,
                item.sale.Quantity,
                Revenue = item.sale.Quantity * item.product.UnitPrice
            })
        .GroupBy(item => new { item.Category, item.City })
        .Select(group => new
        {
            group.Key.Category,
            group.Key.City,
            Units = group.Sum(item => item.Quantity),
            Revenue = group.Sum(item => item.Revenue)
        })
        .OrderByDescending(item => item.Revenue)
        .Take(10));

static void RunAndReport<T>(string title, Func<IEnumerable<T>> query)
{
    var timer = Stopwatch.StartNew();
    var results = query().ToList();
    timer.Stop();
    Console.WriteLine($"--- {title} ---");
    foreach (var item in results.Take(15))
        Console.WriteLine(item);
    if (results.Count > 15)
        Console.WriteLine($"... ще {results.Count - 15} рядків");
    Console.WriteLine($"Рядків: {results.Count}; час: {timer.Elapsed.TotalMilliseconds:F3} мс\n");
}

public sealed record Sale(int Id, int ProductId, int Quantity, DateTime SaleDate, int CustomerId);
public sealed record Product(int Id, string Name, string Category, decimal UnitPrice);
public sealed record Customer(int Id, string Name, string City);
