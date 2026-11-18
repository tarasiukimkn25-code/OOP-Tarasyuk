# Самостійна робота №9. Практичне застосування Dictionary і HashSet

**Тема:** Об'єктно-орієнтоване програмування (C#) | **Виконав:** Тарасюк

## Опис проєкту

У роботі розглянуто три відкриті проєкти та реалізовано три власні компоненти: кеш метаданих файлів, менеджер тегів і пошуковий індекс.

## Приклади з відкритих проєктів

1. **Dictionary для налаштувань Azure App Configuration.** У [Azure App Configuration provider](https://github.com/Azure/AppConfiguration-DotnetProvider/blob/main/src/Microsoft.Extensions.Configuration.AzureAppConfiguration/AzureAppConfigurationProvider.cs#L3522) локальна змінна `data` має тип `Dictionary<string, ConfigurationSetting>` і накопичує налаштування за ключем. Це зручніше за список, бо завантажений запис можна швидко знайти за його ключем без повного перебору.
2. **HashSet для унікальних ключів feature flags.** У тому самому [провайдері](https://github.com/Azure/AppConfiguration-DotnetProvider/blob/main/src/Microsoft.Extensions.Configuration.AzureAppConfiguration/AzureAppConfigurationProvider.cs#L2603) `HashSet<string> ffKeys` збирає ключі прапорців без дублікатів і підтримує швидку перевірку членства.
3. **Спільне використання Dictionary і HashSet.** Під час завантаження feature flags провайдер створює словники для значень та ETag, а `HashSet` — для унікальних ключів ([код ініціалізації](https://github.com/Azure/AppConfiguration-DotnetProvider/blob/main/src/Microsoft.Extensions.Configuration.AzureAppConfiguration/AzureAppConfigurationProvider.cs#L3425)). Такий поділ дає і пошук значення за ключем, і унікальне накопичення ID.

У середньому `Dictionary` і `HashSet` забезпечують додавання, пошук і видалення за O(1); у найгіршому разі через колізії це може стати O(n). `List` має O(1) індексацію та амортизоване O(1) додавання в кінець, але пошук, перевірка членства й видалення за значенням потребують O(n).

## Таблиця складності

| Операція | `List<T>` | `Dictionary<TKey,TValue>` | `HashSet<T>` |
|---|---:|---:|---:|
| Додати елемент | O(1) амортизовано | O(1) у середньому / O(n) найгірше | O(1) у середньому / O(n) найгірше |
| Знайти значення або ключ | O(n) | O(1) у середньому / O(n) найгірше | O(1) у середньому / O(n) найгірше |
| Перевірити наявність | O(n) | O(1) у середньому / O(n) найгірше | O(1) у середньому / O(n) найгірше |
| Видалити за ключем/значенням | O(n) | O(1) у середньому / O(n) найгірше | O(1) у середньому / O(n) найгірше |
| Зберігати дублікати | Так | Ключі — ні | Ні |

Таблиця узгоджується з [оглядом колекцій .NET](https://github.com/dotnet/docs/blob/main/docs/standard/collections/index.md). Реальний час залежить від якості хеш-коду та навантаження.

## Власні приклади

`FileMetadataCache` зберігає метадані за назвою файлу. `TagManager` використовує `Dictionary<string, HashSet<string>>` і операції `UnionWith`, `IntersectWith` та `ExceptWith` для об'єднання, пошуку спільних і унікальних тегів. `SearchIndex` будує відповідність «слово → ID документів» і повертає перетин для AND-запиту.

## Висновки та підводні камені

Словник варто вибирати, коли є стабільний ключ і потрібен доступ до пов'язаного значення; множину — коли важливі унікальність і перевірка членства. `IEqualityComparer<T>` визначає, коли ключі вважаються однаковими: наприклад, `StringComparer.OrdinalIgnoreCase` робить пошук назв файлів нечутливим до регістру. Невдале порівняння може об'єднати різні ключі або, навпаки, не знайти логічно той самий ключ. Хеш-колізії збільшують час пошуку, а колекції на основі хешу не варто використовувати, якщо потрібне стабільне сортування.

## Зразок результату та запуск

Застосунок друкує прочитані метадані, унікальні й спільні теги, а також результати пошуку слова та AND-пошуку.

```bash
cd IndependentWork8
dotnet run
```
<img width="450" height="170" alt="Знімок екрана 2026-11-18 о 22 31 09" src="https://github.com/user-attachments/assets/42b433f0-aa0a-4e73-98f5-0b96535ca022" />
