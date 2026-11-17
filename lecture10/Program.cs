using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=========================================================");
Console.WriteLine("  ЛЕКЦІЯ №10. LINQ: ОПТИМІЗАЦІЯ, РОЗШИРЕННЯ ТА PLINQ");
Console.WriteLine("=========================================================\n");

var orders = Enumerable.Range(1, 12)
    .Select(id => new Order(id, 100m + id * 35m, id % 3 == 0))
    .ToArray();

Console.WriteLine("--- 1. Відкладене виконання та повторне перебирання ---");
var predicateCalls = 0;
var expensiveQuery = orders.Where(order =>
{
    predicateCalls++;
    return order.IsPaid;
});

var paidCount = expensiveQuery.Count();
var paidTotal = expensiveQuery.Sum(order => order.Total);
Console.WriteLine($"Оплачених замовлень: {paidCount}; сума: {paidTotal:C}");
Console.WriteLine($"Перевірок після двох запитів: {predicateCalls} (умова виконалася двічі)\n");

predicateCalls = 0;
var paidOrders = orders.Where(order =>
{
    predicateCalls++;
    return order.IsPaid;
}).ToArray();
paidCount = paidOrders.Length;
paidTotal = paidOrders.Sum(order => order.Total);
Console.WriteLine($"Після ToArray: {paidCount} замовлень; сума: {paidTotal:C}");
Console.WriteLine($"Перевірок після матеріалізації: {predicateCalls} (умова виконалася один раз)\n");

Console.WriteLine("--- 2. Проєкція, агрегація та метод-розширення ---");
var highValuePaidOrders = orders
    .Where(order => order.IsPaid && order.Total >= 300m)
    .Select(order => new { order.Id, order.Total })
    .OrderByDescending(order => order.Total)
    .ToArray();
Console.WriteLine("Оплачені замовлення від 300 грн: " +
    string.Join(", ", highValuePaidOrders.Select(order => $"#{order.Id}: {order.Total:C}")));

var batches = Enumerable.Range(1, 10).InBatches(4);
Console.WriteLine("Групи чисел: " + string.Join(" | ", batches.Select(batch => $"[{string.Join(", ", batch)}]")));
Console.WriteLine();

Console.WriteLine("--- 3. PLINQ для незалежних CPU-обчислень ---");
var input = Enumerable.Range(1, 100_000).ToArray();
var timer = Stopwatch.StartNew();
var sequential = input
    .Where(IsPrime)
    .Select(number => number * number)
    .ToArray();
timer.Stop();
var sequentialMilliseconds = timer.ElapsedMilliseconds;

timer.Restart();
var parallel = input
    .AsParallel()
    .WithDegreeOfParallelism(Math.Max(1, Environment.ProcessorCount))
    .Where(IsPrime)
    .Select(number => number * number)
    .ToArray();
timer.Stop();

Console.WriteLine($"Послідовно: {sequential.Length} результатів за {sequentialMilliseconds} мс.");
Console.WriteLine($"PLINQ:      {parallel.Length} результатів за {timer.ElapsedMilliseconds} мс.");
Console.WriteLine("Час залежить від процесора й навантаження; для невеликої задачі PLINQ може бути повільнішим.");

var orderedParallel = input
    .AsParallel()
    .AsOrdered()
    .Where(number => number <= 30 && number % 2 == 0)
    .Select(number => number * number)
    .ToArray();
Console.WriteLine("Порядок джерела з AsOrdered: " + string.Join(", ", orderedParallel));

Console.WriteLine("\nВисновок: матеріалізуйте запит, коли результат потрібен кілька разів; " +
    "використовуйте PLINQ для великих незалежних CPU-задач і не покладайтеся на порядок без AsOrdered().");
Console.WriteLine("=========================================================\n");

static bool IsPrime(int number)
{
    if (number < 2) return false;
    if (number == 2) return true;
    if (number % 2 == 0) return false;
    for (var divisor = 3; divisor * divisor <= number; divisor += 2)
    {
        if (number % divisor == 0) return false;
    }
    return true;
}

public sealed record Order(int Id, decimal Total, bool IsPaid);

public static class EnumerableBatchExtensions
{
    public static IEnumerable<IReadOnlyList<T>> InBatches<T>(this IEnumerable<T> source, int size)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        var batch = new List<T>(size);
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count == size)
            {
                yield return batch.ToArray();
                batch.Clear();
            }
        }

        if (batch.Count > 0)
            yield return batch.ToArray();
    }
}
