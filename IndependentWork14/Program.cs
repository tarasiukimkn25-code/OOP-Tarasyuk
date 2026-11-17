using System.Diagnostics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Самостійна робота №15. RetryPolicy з експоненційною затримкою та jitter\n");

var policy = new RetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromMilliseconds(80), jitterFactor: 0.2);
var flakyOperation = new FlakyOperation(failuresBeforeSuccess: 2);
var result = policy.Execute(flakyOperation.Run, exception => exception is InvalidOperationException);
Console.WriteLine($"Результат першої операції: {result}\n");

var alwaysFailingAttempts = 0;
var secondPolicy = new RetryPolicy(maxRetries: 2, initialDelay: TimeSpan.FromMilliseconds(60), jitterFactor: 0.1);
try
{
    secondPolicy.Execute<string>(() =>
    {
        alwaysFailingAttempts++;
        throw new InvalidOperationException("Симульований постійний збій.");
    }, exception => exception is InvalidOperationException);
}
catch (InvalidOperationException exception)
{
    Console.WriteLine($"Операція зупинена після {alwaysFailingAttempts} спроб: {exception.Message}");
}

public sealed class RetryPolicy
{
    private readonly int _maxRetries;
    private readonly TimeSpan _initialDelay;
    private readonly double _jitterFactor;

    public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxRetries);
        ArgumentOutOfRangeException.ThrowIfLessThan(initialDelay, TimeSpan.Zero);
        if (jitterFactor is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(jitterFactor), "Значення має бути в межах від 0 до 1.");
        _maxRetries = maxRetries;
        _initialDelay = initialDelay;
        _jitterFactor = jitterFactor;
    }

    public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);
        shouldRetry ??= _ => true;

        for (var retryAttempt = 0; ; retryAttempt++)
        {
            var attemptNumber = retryAttempt + 1;
            Console.WriteLine($"Спроба {attemptNumber} з {_maxRetries + 1}.");
            try
            {
                var result = operation();
                Console.WriteLine($"Спроба {attemptNumber}: успіх.");
                return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Спроба {attemptNumber}: {exception.GetType().Name}: {exception.Message}");
                if (retryAttempt >= _maxRetries || !shouldRetry(exception))
                {
                    Console.WriteLine("Повтор не виконується; виняток передається виклику.");
                    throw;
                }

                var jitter = 1 + Random.Shared.NextDouble() * _jitterFactor * 2 - _jitterFactor;
                var delayMilliseconds = _initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt) * jitter;
                var delay = TimeSpan.FromMilliseconds(delayMilliseconds);
                Console.WriteLine($"Повтор через {delay.TotalMilliseconds:F0} мс.");
                Thread.Sleep(delay);
            }
        }
    }
}

public sealed class FlakyOperation(int failuresBeforeSuccess)
{
    private int _attempts;

    public string Run()
    {
        _attempts++;
        if (_attempts <= failuresBeforeSuccess)
            throw new InvalidOperationException("Тимчасова помилка під час збереження.");
        return $"Дані збережено на спробі {_attempts}.";
    }
}
