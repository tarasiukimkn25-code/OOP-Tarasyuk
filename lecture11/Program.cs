Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=========================================================");
Console.WriteLine("  ЛЕКЦІЯ №11. ВИНЯТКИ У .NET: ПАТЕРНИ ТА RETRY");
Console.WriteLine("=========================================================\n");

Console.WriteLine("--- 1. Власний виняток і валідація ---");
try
{
    PaymentValidator.ValidateAmount(-25m);
}
catch (InvalidPaymentAmountException exception)
{
    Console.WriteLine($"Очікувана помилка валідації: {exception.Message}");
}
finally
{
    Console.WriteLine("Блок finally виконується незалежно від результату.");
}

Console.WriteLine("\n--- 2. Повторні спроби для тимчасової помилки ---");
var paymentGateway = new UnstablePaymentGateway(failuresBeforeSuccess: 2);
var receipt = Retry.Execute(
    operation: paymentGateway.Charge,
    maxAttempts: 5,
    initialDelay: TimeSpan.FromSeconds(2),
    backoff: BackoffStrategy.Fixed,
    shouldRetry: exception => exception is InvalidOperationException,
    onRetry: (attempt, delay, exception) =>
        Console.WriteLine($"Спроба {attempt} не вдалася: {exception.Message}. Повтор через {delay.TotalMilliseconds:0} мс."));

Console.WriteLine($"Платіж успішний після {paymentGateway.Attempts} спроб: {receipt}\n");

Console.WriteLine("--- 3. Не повторюємо неповторювану помилку ---");
try
{
    Retry.Execute<string>(
        operation: () => throw new InvalidPaymentAmountException("Сума має бути додатною."),
        maxAttempts: 3,
        initialDelay: TimeSpan.FromMilliseconds(50),
        backoff: BackoffStrategy.Fixed,
        shouldRetry: exception => exception is InvalidOperationException,
        onRetry: (attempt, delay, exception) =>
            Console.WriteLine($"Повтор {attempt}: {exception.Message}"));
}
catch (InvalidPaymentAmountException exception)
{
    Console.WriteLine($"Помилка не є тимчасовою, тому Retry її не повторює: {exception.Message}");
}

Console.WriteLine("\nВисновок: повторюйте тільки тимчасові помилки, обмежуйте кількість спроб " +
    "і додавайте затримку між ними.");
Console.WriteLine("=========================================================\n");

public sealed class InvalidPaymentAmountException(string message) : Exception(message);

public static class PaymentValidator
{
    public static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidPaymentAmountException("Сума платежу має бути більшою за нуль.");
    }
}

public enum BackoffStrategy
{
    Fixed,
    Linear,
    Exponential
}

public static class Retry
{
    public static T Execute<T>(
        Func<T> operation,
        int maxAttempts,
        TimeSpan initialDelay,
        BackoffStrategy backoff,
        Func<Exception, bool> shouldRetry,
        Action<int, TimeSpan, Exception>? onRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(shouldRetry);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAttempts);
        ArgumentOutOfRangeException.ThrowIfLessThan(initialDelay, TimeSpan.Zero);

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return operation();
            }
            catch (Exception exception) when (attempt < maxAttempts && shouldRetry(exception))
            {
                var delay = GetDelay(initialDelay, attempt, backoff);
                onRetry?.Invoke(attempt, delay, exception);
                Thread.Sleep(delay);
            }
        }
    }

    private static TimeSpan GetDelay(TimeSpan initialDelay, int attempt, BackoffStrategy backoff) => backoff switch
    {
        BackoffStrategy.Fixed => initialDelay,
        BackoffStrategy.Linear => TimeSpan.FromTicks(initialDelay.Ticks * attempt),
        BackoffStrategy.Exponential => TimeSpan.FromTicks(initialDelay.Ticks * (1L << Math.Min(attempt - 1, 20))),
        _ => throw new ArgumentOutOfRangeException(nameof(backoff), backoff, "Невідома стратегія затримки.")
    };
}

public sealed class UnstablePaymentGateway(int failuresBeforeSuccess)
{
    public int Attempts { get; private set; }

    public string Charge()
    {
        Attempts++;
        if (Attempts <= failuresBeforeSuccess)
            throw new InvalidOperationException("Тимчасова помилка платіжного сервісу.");

        return "PAY-2026-001";
    }
}
