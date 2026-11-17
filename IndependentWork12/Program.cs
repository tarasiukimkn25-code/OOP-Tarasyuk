using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Самостійна робота №13. LINQ, PLINQ і побічні ефекти\n");

foreach (var count in new[] { 1_000_000, 5_000_000, 10_000_000 })
{
    var random = new Random(2026);
    var values = new int[count];
    for (var i = 0; i < values.Length; i++)
        values[i] = random.Next(1, 100_000);

    var timer = Stopwatch.StartNew();
    var linqTotal = values.Select(HeavyWork).Sum();
    timer.Stop();
    var linqMilliseconds = timer.Elapsed.TotalMilliseconds;

    timer.Restart();
    var plinqTotal = values.AsParallel()
        .WithDegreeOfParallelism(Math.Max(1, Environment.ProcessorCount))
        .Select(HeavyWork)
        .Sum();
    timer.Stop();

    Console.WriteLine($"{count:N0} значень | LINQ: {linqMilliseconds:F1} мс | " +
        $"PLINQ: {timer.Elapsed.TotalMilliseconds:F1} мс | Прискорення: {linqMilliseconds / timer.Elapsed.TotalMilliseconds:F2}x | " +
        $"Результати збігаються: {linqTotal == plinqTotal}");
}

Console.WriteLine("\n--- Гонка даних: спільний лічильник без синхронізації ---");
const int increments = 500_000;
for (var run = 1; run <= 3; run++)
{
    var unsafeCounter = 0;
    Parallel.ForEach(Enumerable.Range(0, increments), _ =>
    {
        var current = unsafeCounter;
        Thread.SpinWait(5);
        unsafeCounter = current + 1;
    });
    Console.WriteLine($"Запуск {run}: очікувалось {increments:N0}, отримано {unsafeCounter:N0}");
}

Console.WriteLine("\n--- Виправлення через Interlocked.Increment ---");
var safeCounter = 0;
Parallel.ForEach(Enumerable.Range(0, increments), _ => Interlocked.Increment(ref safeCounter));
Console.WriteLine($"Очікувалось {increments:N0}, отримано {safeCounter:N0}");

static long HeavyWork(int value)
{
    var current = (double)value;
    for (var iteration = 1; iteration <= 12; iteration++)
        current = Math.Sqrt(current * current + iteration) + Math.Log(current + 1);
    return (long)(current * 1000);
}
