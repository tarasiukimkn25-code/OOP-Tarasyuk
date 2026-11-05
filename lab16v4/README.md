# Лабораторна робота №16. LINQ: SelectMany, пагінація й custom extensions (Companies & Employees)

**Тема:** Об'єктно-орієнтоване програмування (C#) | **Виконав:** Тарасюк

## Опис проєкту

У цій лабораторній роботі реалізовано обробку даних про компанії, їх співробітників та проєкти (Варіант 4) із використанням розширеного функціоналу **LINQ**:

* **`SelectMany`:** Реалізовано "розгортання" (flattening) дворічневої вкладеної структури колекцій `Company` -> `Employees` -> `Projects` в один плаский список проєктів.
* **Пагінація (`Skip` / `Take`):** Реалізовано посторінкову навігацію по списку компаній (розбиття по 2 компанії на сторінку).
* **Custom Extension Method (`TotalEmployees`):** Створено власний метод розширення для `IEnumerable<Company>`, який підраховує підсумкову кількість співробітників у всіх компаніях.

## Опис методу розширення

```csharp
public static class CompanyExtensions
{
    public static int TotalEmployees(this IEnumerable<Company> companies)
    {
        return companies?.Sum(c => c.Employees?.Count ?? 0) ?? 0;
    }
}

<img width="601" height="485" alt="Знімок екрана 2026-11-05 о 13 43 05" src="https://github.com/user-attachments/assets/8b0ed870-fac0-4493-832f-3304db8aee80" />
