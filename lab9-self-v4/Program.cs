using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Самостійна робота №10. Узагальнені методи та делегати\n");

Console.WriteLine("--- List<int> ---");
List<int> numbers = [12, 4, 19, 7, 2, 15];
var doubled = CollectionUtils.Map(numbers, number => number * 2);
Console.WriteLine("Map (x2): " + string.Join(", ", doubled));
Console.WriteLine("MaxBy: " + CollectionUtils.MaxBy(numbers, number => number));
Console.WriteLine("Filter (парні): " + string.Join(", ", CollectionUtils.Filter(numbers, number => number % 2 == 0)));
Console.Write("ForEach: ");
CollectionUtils.ForEach(numbers, number => Console.Write($"{number} "));
Console.WriteLine("\n");

Console.WriteLine("--- List<string> ---");
List<string> names = ["Олена", "Андрій", "Марія", "Богдан"];
Console.WriteLine("Map (довжина): " + string.Join(", ", CollectionUtils.Map(names, name => name.Length)));
Console.WriteLine("MaxBy (найдовше ім'я): " + CollectionUtils.MaxBy(names, name => name.Length));
Console.WriteLine("Filter (починаються на М): " + string.Join(", ", CollectionUtils.Filter(names, name => name.StartsWith('М'))));
Console.Write("ForEach: ");
CollectionUtils.ForEach(names, name => Console.Write($"{name}; "));
Console.WriteLine("\n");

Console.WriteLine("--- List<Product> ---");
List<Product> products =
[
    new("Клавіатура", 1299.99m),
    new("Миша", 799.50m),
    new("Монітор", 7499.00m)
];
Console.WriteLine("Map (назви): " + string.Join(", ", CollectionUtils.Map(products, product => product.Name)));
Console.WriteLine("MaxBy (найдорожчий): " + CollectionUtils.MaxBy(products, product => product.Price));
Console.WriteLine("Filter (до 2000 грн): " + string.Join("; ", CollectionUtils.Filter(products, product => product.Price < 2000m)));
Console.WriteLine("ForEach:");
CollectionUtils.ForEach(products, product => Console.WriteLine($"  {product.Name}: {product.Price:N2} грн"));

public sealed record Product(string Name, decimal Price)
{
    public override string ToString() => $"{Name} ({Price:N2} грн)";
}

public static class CollectionUtils
{
    public static IEnumerable<TResult> Map<TSource, TResult>(
        IEnumerable<TSource> source,
        Func<TSource, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);
        foreach (var item in source)
            yield return selector(item);
    }

    public static T MaxBy<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new InvalidOperationException("Не можна знайти максимум у порожній колекції.");

        var maximum = enumerator.Current;
        var maximumKey = keySelector(maximum);
        while (enumerator.MoveNext())
        {
            var candidate = enumerator.Current;
            var candidateKey = keySelector(candidate);
            if (candidateKey.CompareTo(maximumKey) > 0)
            {
                maximum = candidate;
                maximumKey = candidateKey;
            }
        }

        return maximum;
    }

    public static IEnumerable<T> Filter<T>(IEnumerable<T> source, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        foreach (var item in source)
            if (predicate(item))
                yield return item;
    }

    public static void ForEach<T>(IEnumerable<T> source, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);
        foreach (var item in source)
            action(item);
    }
}
