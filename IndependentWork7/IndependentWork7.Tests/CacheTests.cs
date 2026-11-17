using IndependentWork7;
using Xunit;

namespace IndependentWork7.Tests;

public sealed class CacheTests
{
    [Fact]
    public void AddAndGet_ReturnsStoredValue()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("answer", 42);
        Assert.Equal(42, cache.Get("answer"));
    }

    [Fact]
    public void Add_WhenCapacityIsExceeded_EvictsOldestItem()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "first");
        cache.Add(2, "second");
        cache.Add(3, "third");
        Assert.False(cache.ContainsKey(1));
        Assert.Equal("second", cache.Get(2));
        Assert.Equal("third", cache.Get(3));
    }

    [Fact]
    public void Get_WhenKeyIsMissing_ThrowsKeyNotFoundException()
    {
        var cache = new Cache<string, string>(1);
        Assert.Throws<KeyNotFoundException>(() => cache.Get("missing"));
    }

    [Fact]
    public void ContainsKeyAndCount_ReflectCurrentItems()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "one");
        cache.Add(2, "two");
        Assert.True(cache.ContainsKey(1));
        Assert.False(cache.ContainsKey(3));
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void UpdatingExistingKey_DoesNotChangeItsFifoPosition()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "old");
        cache.Add(2, "two");
        cache.Add(1, "updated");
        cache.Add(3, "three");
        Assert.False(cache.ContainsKey(1));
        Assert.True(cache.ContainsKey(2));
    }
}
