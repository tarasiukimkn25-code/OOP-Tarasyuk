using IndependentWork7;

var cache = new Cache<string, string>(capacity: 3);
cache.Add("user:1", "Олена");
cache.Add("user:2", "Андрій");
cache.Add("user:3", "Марія");
Console.WriteLine($"Користувач user:2: {cache.Get("user:2")}");

cache.Add("user:4", "Богдан");
Console.WriteLine($"Після переповнення кеш містить user:1: {cache.ContainsKey("user:1")}");
Console.WriteLine($"Елементів у кеші: {cache.Count}");
