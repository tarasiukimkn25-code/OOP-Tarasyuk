namespace IndependentWork7;

public sealed class Cache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, TValue> _items = new();
    private readonly Queue<TKey> _insertionOrder = new();

    public Cache(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        _capacity = capacity;
    }

    public int Count => _items.Count;

    public void Add(TKey key, TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (_items.ContainsKey(key))
        {
            _items[key] = value;
            return;
        }

        if (_items.Count == _capacity)
        {
            var oldestKey = _insertionOrder.Dequeue();
            _items.Remove(oldestKey);
        }

        _items.Add(key, value);
        _insertionOrder.Enqueue(key);
    }

    public TValue Get(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (!_items.TryGetValue(key, out var value))
            throw new KeyNotFoundException($"Ключ '{key}' відсутній у кеші.");
        return value;
    }

    public bool ContainsKey(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return _items.ContainsKey(key);
    }
}
