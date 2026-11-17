using System.Text.RegularExpressions;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Самостійна робота №9. Dictionary та HashSet\n");

var metadataCache = new FileMetadataCache();
metadataCache.Add(new FileMetadata("report.md", 2048, DateTime.UtcNow, "sha256-a1b2"));
metadataCache.Add(new FileMetadata("notes.txt", 512, DateTime.UtcNow, "sha256-c3d4"));
Console.WriteLine("Файл report.md: " + metadataCache.Get("report.md"));
Console.WriteLine("Містить notes.txt: " + metadataCache.Contains("notes.txt"));

var tagManager = new TagManager();
tagManager.AddTag("doc-1", "csharp");
tagManager.AddTag("doc-1", "linq");
tagManager.AddTag("doc-2", "csharp");
tagManager.AddTag("doc-2", "collections");
Console.WriteLine("Усі теги: " + string.Join(", ", tagManager.GetAllUniqueTags()));
Console.WriteLine("Спільні теги doc-1 і doc-2: " + string.Join(", ", tagManager.FindCommonTags("doc-1", "doc-2")));
Console.WriteLine("Лише doc-1: " + string.Join(", ", tagManager.FindTagsOnlyOn("doc-1", "doc-2")));

var searchIndex = new SearchIndex();
searchIndex.AddDocument(1, "C# collections provide fast lookups.");
searchIndex.AddDocument(2, "HashSet stores unique tags and values.");
searchIndex.AddDocument(3, "Dictionary maps keys to values; HashSet checks membership.");
Console.WriteLine("Пошук 'hashset': " + string.Join(", ", searchIndex.Search("hashset")));
Console.WriteLine("AND-пошук 'hashset values': " + string.Join(", ", searchIndex.SearchMultiple("hashset values")));

public sealed record FileMetadata(string FileName, long Size, DateTime LastModified, string Hash)
{
    public override string ToString() => $"{FileName}, {Size} байт, {LastModified:yyyy-MM-dd}, {Hash}";
}

public sealed class FileMetadataCache
{
    private readonly Dictionary<string, FileMetadata> _files = new(StringComparer.OrdinalIgnoreCase);

    public void Add(FileMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        _files[metadata.FileName] = metadata;
    }

    public FileMetadata Get(string fileName) => _files.TryGetValue(fileName, out var metadata)
        ? metadata
        : throw new KeyNotFoundException($"Файл '{fileName}' не знайдено.");

    public bool Remove(string fileName) => _files.Remove(fileName);
    public bool Contains(string fileName) => _files.ContainsKey(fileName);
}

public sealed class TagManager
{
    private readonly Dictionary<string, HashSet<string>> _tagsByItem = new(StringComparer.OrdinalIgnoreCase);

    public void AddTag(string itemId, string tag)
    {
        var tags = GetOrCreate(itemId);
        tags.Add(tag.Trim().ToLowerInvariant());
    }

    public bool RemoveTag(string itemId, string tag)
    {
        if (!_tagsByItem.TryGetValue(itemId, out var tags))
            return false;
        var removed = tags.Remove(tag.Trim().ToLowerInvariant());
        if (tags.Count == 0)
            _tagsByItem.Remove(itemId);
        return removed;
    }

    public HashSet<string> GetAllUniqueTags()
    {
        var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tags in _tagsByItem.Values)
            allTags.UnionWith(tags);
        return allTags;
    }

    public HashSet<string> FindCommonTags(string firstItemId, string secondItemId)
    {
        var common = _tagsByItem.TryGetValue(firstItemId, out var first)
            ? new HashSet<string>(first, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (_tagsByItem.TryGetValue(secondItemId, out var second))
            common.IntersectWith(second);
        else
            common.Clear();
        return common;
    }

    public HashSet<string> FindTagsOnlyOn(string firstItemId, string secondItemId)
    {
        var onlyFirst = _tagsByItem.TryGetValue(firstItemId, out var first)
            ? new HashSet<string>(first, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (_tagsByItem.TryGetValue(secondItemId, out var second))
            onlyFirst.ExceptWith(second);
        return onlyFirst;
    }

    private HashSet<string> GetOrCreate(string itemId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        if (!_tagsByItem.TryGetValue(itemId, out var tags))
        {
            tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _tagsByItem.Add(itemId, tags);
        }
        return tags;
    }
}

public sealed class SearchIndex
{
    private readonly Dictionary<string, HashSet<int>> _documentIdsByWord = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<int, string> _documents = [];

    public void AddDocument(int id, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        if (!_documents.TryAdd(id, text))
            throw new ArgumentException($"Документ з ID {id} уже існує.", nameof(id));

        foreach (var word in Tokenize(text))
        {
            if (!_documentIdsByWord.TryGetValue(word, out var ids))
            {
                ids = [];
                _documentIdsByWord.Add(word, ids);
            }
            ids.Add(id);
        }
    }

    public IReadOnlyCollection<int> Search(string word) =>
        _documentIdsByWord.TryGetValue(Normalize(word), out var ids)
            ? ids.Order().ToArray()
            : Array.Empty<int>();

    public IReadOnlyCollection<int> SearchMultiple(string query)
    {
        var words = Tokenize(query);
        if (words.Count == 0)
            return Array.Empty<int>();
        var iterator = words.GetEnumerator();
        iterator.MoveNext();
        if (!_documentIdsByWord.TryGetValue(iterator.Current, out var first))
            return Array.Empty<int>();
        var matches = new HashSet<int>(first);
        while (iterator.MoveNext())
        {
            if (!_documentIdsByWord.TryGetValue(iterator.Current, out var next))
                return Array.Empty<int>();
            matches.IntersectWith(next);
        }
        return matches.Order().ToArray();
    }

    private static HashSet<string> Tokenize(string text) =>
        Regex.Matches(text, @"\w+").Select(match => Normalize(match.Value)).ToHashSet(StringComparer.OrdinalIgnoreCase);

    private static string Normalize(string word) => Regex.Match(word, @"\w+").Value.ToLowerInvariant();
}
