using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Самостійна робота №14. Стійкість до відмов за допомогою Polly\n");

Console.WriteLine("--- Сценарій 1. Зовнішній API: timeout + retry ---");
var api = new SimulatedApi(failuresBeforeSuccess: 2);
var apiPipeline = new ResiliencePipelineBuilder<string>()
    .AddRetry(new RetryStrategyOptions<string>
    {
        ShouldHandle = new PredicateBuilder<string>().Handle<HttpRequestException>(),
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromMilliseconds(50),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        OnRetry = arguments =>
        {
            Console.WriteLine($"Retry {arguments.AttemptNumber + 1}: {arguments.Outcome.Exception?.Message}");
            return default;
        }
    })
    .AddTimeout(TimeSpan.FromMilliseconds(500))
    .Build();
Console.WriteLine("API result: " + await apiPipeline.ExecuteAsync(token => api.GetAsync(token)));
try
{
    Console.WriteLine("Повільний API result: " + await apiPipeline.ExecuteAsync(token => api.GetAsync(token)));
}
catch (TimeoutRejectedException)
{
    Console.WriteLine("Повільний виклик завершено timeout-стратегією.");
}

Console.WriteLine("\n--- Сценарій 2. База даних: circuit breaker ---");
var database = new SimulatedDatabase(failures: 2);
var databasePipeline = new ResiliencePipelineBuilder<string>()
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions<string>
    {
        ShouldHandle = new PredicateBuilder<string>().Handle<TimeoutRejectedException>(),
        FailureRatio = 0.5,
        SamplingDuration = TimeSpan.FromSeconds(10),
        MinimumThroughput = 2,
        BreakDuration = TimeSpan.FromSeconds(2)
    })
    .Build();

for (var call = 1; call <= 3; call++)
{
    try
    {
        Console.WriteLine($"Запит {call}: {databasePipeline.Execute(database.Read)}");
    }
    catch (BrokenCircuitException)
    {
        Console.WriteLine($"Запит {call}: circuit відкритий — виклик до бази не виконано.");
    }
    catch (TimeoutRejectedException exception)
    {
        Console.WriteLine($"Запит {call}: збій бази — {exception.Message}");
    }
}

Console.WriteLine("\n--- Сценарій 3. Читання каталогу: fallback на кеш ---");
var catalogPipeline = new ResiliencePipelineBuilder<string>()
    .AddFallback(new FallbackStrategyOptions<string>
    {
        ShouldHandle = new PredicateBuilder<string>().Handle<HttpRequestException>(),
        FallbackAction = _ => Outcome.FromResultAsValueTask("Каталог із локального кешу"),
        OnFallback = _ =>
        {
            Console.WriteLine("Основне джерело недоступне; повертаю резервне значення.");
            return default;
        }
    })
    .Build();
Console.WriteLine("Catalog result: " + catalogPipeline.Execute<string>(
    () => throw new HttpRequestException("Сервіс каталогу недоступний.")));

public sealed class SimulatedApi(int failuresBeforeSuccess)
{
    private int _attempts;

    public async ValueTask<string> GetAsync(CancellationToken cancellationToken)
    {
        _attempts++;
        if (_attempts <= failuresBeforeSuccess)
            throw new HttpRequestException("Тимчасова помилка API.");
        var responseDelay = _attempts == failuresBeforeSuccess + 1
            ? TimeSpan.FromMilliseconds(100)
            : TimeSpan.FromSeconds(1);
        await Task.Delay(responseDelay, cancellationToken);
        return "Дані API отримано.";
    }
}

public sealed class SimulatedDatabase(int failures)
{
    private int _attempts;

    public string Read()
    {
        _attempts++;
        if (_attempts <= failures)
            throw new TimeoutRejectedException("Тайм-аут читання бази даних.");
        return "Рядок прочитано.";
    }
}
