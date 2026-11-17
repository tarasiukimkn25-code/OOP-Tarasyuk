using System.Diagnostics;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var documents = CreateRequiredDocuments();
var index = new InvertedIndex();
foreach (var document in documents)
    index.AddDocument(document);

Console.WriteLine("Самостійна робота №11. Інвертований індекс");
Console.WriteLine($"Документів: {index.DocumentCount}; унікальних слів: {index.UniqueWords}\n");

Console.WriteLine("Пошук одного слова: programming");
PrintResults(index.SearchSingleWord("programming"));
Console.WriteLine("Пошук AND: c# programming");
PrintResults(index.SearchAnd("c# programming"));
Console.WriteLine("Пошук OR: java python");
PrintResults(index.SearchOr("java python"));

Console.WriteLine("\nПорівняння лінійного пошуку й індексу (AND: programming language)");
Console.WriteLine("Документи | Лінійний пошук, мс/запит | Інвертований індекс, мс/запит | Збіг результатів");
foreach (var count in new[] { 10, 100, 1_000, 10_000 })
{
    var data = CreateBenchmarkDocuments(count);
    var builtIndex = new InvertedIndex();
    foreach (var document in data)
        builtIndex.AddDocument(document);

    var normalized = data.ToDictionary(
        document => document.Id,
        document => TextProcessor.RemoveStopWords(TextProcessor.Tokenize(document.Content)));
    var queryTerms = TextProcessor.RemoveStopWords(TextProcessor.Tokenize("programming language"));
    var repetitions = Math.Max(100, 100_000 / count);

    var linearTimer = Stopwatch.StartNew();
    IReadOnlyCollection<Document> linearResults = Array.Empty<Document>();
    for (var i = 0; i < repetitions; i++)
        linearResults = SearchLinearAnd(data, normalized, queryTerms);
    linearTimer.Stop();

    var indexTimer = Stopwatch.StartNew();
    IReadOnlyCollection<Document> indexedResults = Array.Empty<Document>();
    for (var i = 0; i < repetitions; i++)
        indexedResults = builtIndex.SearchAndTerms(queryTerms);
    indexTimer.Stop();

    var sameResults = linearResults.Select(document => document.Id).Order().SequenceEqual(
        indexedResults.Select(document => document.Id).Order());
    var linearPerQuery = linearTimer.Elapsed.TotalMilliseconds / repetitions;
    var indexPerQuery = indexTimer.Elapsed.TotalMilliseconds / repetitions;
    Console.WriteLine($"{count,10} | {linearPerQuery,25:F6} | {indexPerQuery,29:F6} | {sameResults}");
}

Console.WriteLine("\nІнтерактивний AND-пошук. Введіть слова через пробіл або 'exit' для виходу.");
while (true)
{
    Console.Write("Пошук> ");
    var query = Console.ReadLine();
    if (query is null || query.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    var results = index.SearchAnd(query);
    if (results.Count == 0)
    {
        Console.WriteLine("Збігів не знайдено.");
        continue;
    }

    PrintResults(results);
}

static void PrintResults(IEnumerable<Document> results)
{
    foreach (var document in results)
    {
        var preview = document.Content.Length <= 100
            ? document.Content
            : document.Content[..100] + "…";
        Console.WriteLine($"- [{document.Id}] {document.Title}: {preview}");
    }
}

static List<Document> CreateRequiredDocuments() =>
[
    new(1, "Introduction to C#", "C# is a modern object-oriented programming language developed by Microsoft."),
    new(2, "Java Basics", "Java is a popular programming language known for its portability and performance."),
    new(3, "Python Overview", "Python is a high-level programming language with simple syntax."),
    new(4, "C# Advanced Features", "C# supports LINQ, async/await, and many modern programming paradigms."),
    new(5, "Comparing Languages", "C#, Java, and Python are among the most popular programming languages today.")
];

static List<Document> CreateBenchmarkDocuments(int count)
{
    var data = CreateRequiredDocuments();
    for (var id = data.Count + 1; id <= count; id++)
        data.Add(new Document(id, $"Synthetic Document {id}",
            $"A programming language document about indexing and search. Synthetic item {id}."));
    return data;
}

static IReadOnlyCollection<Document> SearchLinearAnd(
    IReadOnlyCollection<Document> documents,
    IReadOnlyDictionary<int, HashSet<string>> normalizedDocuments,
    HashSet<string> queryTerms) => documents
        .Where(document => queryTerms.All(term => normalizedDocuments[document.Id].Contains(term)))
        .OrderBy(document => document.Id)
        .ToArray();

public sealed class Document(int id, string title, string content)
{
    public int Id { get; } = id;
    public string Title { get; } = title;
    public string Content { get; } = content;

    public override string ToString() => $"[{Id}] {Title}";
}

public sealed class InvertedIndex
{
    private readonly Dictionary<string, HashSet<int>> _wordToDocumentIds = new(StringComparer.Ordinal);
    private readonly Dictionary<int, Document> _documents = [];

    public int DocumentCount => _documents.Count;
    public int UniqueWords => _wordToDocumentIds.Count;

    public void AddDocument(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (!_documents.TryAdd(document.Id, document))
            throw new ArgumentException($"Документ з ID {document.Id} уже додано.", nameof(document));

        var terms = TextProcessor.RemoveStopWords(TextProcessor.Tokenize(document.Content));
        foreach (var term in terms)
        {
            if (!_wordToDocumentIds.TryGetValue(term, out var documentIds))
            {
                documentIds = [];
                _wordToDocumentIds.Add(term, documentIds);
            }
            documentIds.Add(document.Id);
        }
    }

    public IReadOnlyCollection<Document> SearchSingleWord(string word)
    {
        var terms = TextProcessor.RemoveStopWords(TextProcessor.Tokenize(word));
        if (terms.Count != 1 || !_wordToDocumentIds.TryGetValue(terms.First(), out var ids))
            return Array.Empty<Document>();
        return Resolve(ids);
    }

    public IReadOnlyCollection<Document> SearchAnd(string query) =>
        SearchAndTerms(TextProcessor.RemoveStopWords(TextProcessor.Tokenize(query)));

    public IReadOnlyCollection<Document> SearchAndTerms(HashSet<string> terms)
    {
        if (terms.Count == 0)
            return Array.Empty<Document>();

        var iterator = terms.GetEnumerator();
        iterator.MoveNext();
        if (!_wordToDocumentIds.TryGetValue(iterator.Current, out var first))
            return Array.Empty<Document>();

        var matches = new HashSet<int>(first);
        while (iterator.MoveNext())
        {
            if (!_wordToDocumentIds.TryGetValue(iterator.Current, out var next))
                return Array.Empty<Document>();
            matches.IntersectWith(next);
            if (matches.Count == 0)
                return Array.Empty<Document>();
        }
        return Resolve(matches);
    }

    public IReadOnlyCollection<Document> SearchOr(string query)
    {
        var terms = TextProcessor.RemoveStopWords(TextProcessor.Tokenize(query));
        if (terms.Count == 0)
            return Array.Empty<Document>();

        var matches = new HashSet<int>();
        foreach (var term in terms)
        {
            if (_wordToDocumentIds.TryGetValue(term, out var ids))
                matches.UnionWith(ids);
        }
        return Resolve(matches);
    }

    private IReadOnlyCollection<Document> Resolve(IEnumerable<int> ids) => ids
        .Where(_documents.ContainsKey)
        .Select(id => _documents[id])
        .OrderBy(document => document.Id)
        .ToArray();
}

public static class TextProcessor
{
    private static readonly HashSet<string> StopWords = new(StringComparer.Ordinal)
    {
        "a", "an", "the", "is", "are", "was", "were", "in", "on", "at", "to", "for", "of", "and", "or"
    };

    public static HashSet<string> Tokenize(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return System.Text.RegularExpressions.Regex.Matches(text.ToLowerInvariant(), @"\w+")
            .Select(match => match.Value)
            .ToHashSet(StringComparer.Ordinal);
    }

    public static HashSet<string> RemoveStopWords(IEnumerable<string> words) =>
        words.Where(word => !StopWords.Contains(word)).ToHashSet(StringComparer.Ordinal);
}
