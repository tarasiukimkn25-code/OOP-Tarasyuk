# Самостійна робота №11. Інвертований індекс

**Тема:** Об'єктно-орієнтоване програмування (C#) | **Виконав:** Тарасюк | **Варіант:** спільне завдання

## Опис проєкту

Консольний застосунок будує інвертований індекс для п'яти заданих документів. `InvertedIndex` зберігає відповідність «слово → набір ID документів» у `Dictionary<string, HashSet<int>>` і самі документи в окремому словнику.

1. `TextProcessor.Tokenize` переводить текст у нижній регістр і виділяє слова регулярним виразом `\w+`.
2. `RemoveStopWords` прибирає задані англійські стоп-слова під час індексації та пошуку.
3. Підтримано пошук одного слова, AND-пошук через перетин множин і OR-пошук через об'єднання.
4. Виконується порівняння лінійного пошуку й індексу на 10, 100, 1 000 і 10 000 документах з однаковою нормалізацією та перевіркою однакових результатів.
5. Інтерактивний AND-пошук приймає запити до введення `exit`.

## Зразок результату

```text
Документів: 5; унікальних слів: 31
Пошук одного слова: programming
- [1] Introduction to C#: C# is a modern object-oriented programming language developed by Microsoft.
- [2] Java Basics: Java is a popular programming language known for its portability and performance.
- [3] Python Overview: Python is a high-level programming language with simple syntax.
- [4] C# Advanced Features: C# supports LINQ, async/await, and many modern programming paradigms.
- [5] Comparing Languages: C#, Java, and Python are among the most popular programming languages today.
Пошук AND: c# programming
- [1] Introduction to C#: C# is a modern object-oriented programming language developed by Microsoft.
- [4] C# Advanced Features: C# supports LINQ, async/await, and many modern programming paradigms.
- [5] Comparing Languages: C#, Java, and Python are among the most popular programming languages today.
```

Час у таблиці залежить від обладнання та навантаження системи. Фактичні результати зберігаються у `REPORT.md`.

## Як запустити проєкт

```bash
cd InvertedIndexDemo
dotnet run
```

У режимі інтерактивного пошуку введіть слова через пробіл для AND-пошуку або `exit` для завершення.
<img width="762" height="367" alt="Знімок екрана 2026-11-18 о 22 29 20" src="https://github.com/user-attachments/assets/754e9b03-df3c-470b-b114-1b521fb2a66b" />

