 />
# Самостійна робота №14. Стійкість системи за допомогою Polly

**Тема:** Об'єктно-орієнтоване програмування (C#) | **Виконав:** Тарасюк

## Опис проєкту

Три консольні сценарії показують різні способи відновлення після збоїв. Використано `Polly.Core` 8.9.1 та API `ResiliencePipelineBuilder` з лінійкою Polly v8. Сценарії працюють на симульованих збоях і не звертаються до зовнішніх сервісів.

1. **Зовнішній API:** обмежений Retry з експоненційним backoff і jitter, обгорнутий timeout-стратегією.
2. **База даних:** Circuit Breaker відкривається після двох помилок тайм-ауту та швидко відхиляє наступний виклик.
3. **Каталог:** Fallback повертає значення з локального кешу, якщо основне джерело недоступне.

Приклади узгоджені з [офіційним посібником Polly v8](https://github.com/App-vNext/Polly) та його документацією про [Retry](https://github.com/App-vNext/Polly/blob/main/docs/strategies/retry.md), [Circuit Breaker](https://github.com/App-vNext/Polly/blob/main/docs/strategies/circuit-breaker.md), [Fallback](https://github.com/App-vNext/Polly/blob/main/docs/strategies/fallback.md) і [Timeout](https://github.com/App-vNext/Polly/blob/main/docs/strategies/timeout.md).

## Аналіз сценаріїв

| Сценарій | Проблема | Обране рішення | Обмеження |
|---|---|---|---|
| API | Короткий мережевий збій може зникнути сам | Retry з backoff+jitter та timeout | Retry лише для тимчасових помилок; безпечний для ідемпотентного читання |
| База даних | Послідовні тайм-аути перевантажують залежність | Circuit Breaker припиняє нові виклики на період відновлення | Не відновлює дані сам і потребує вибору порогів |
| Каталог | Основне джерело недоступне, але старий каталог можна показати | Fallback повертає локальне значення | Кеш може бути застарілим і має позначатися як резервний |

**Комбінація API:** у побудовнику Polly Retry є зовнішньою стратегією, а timeout — внутрішньою. Це обмежує час кожної спроби, залишаючи повтори під контролем Retry. Для грошових операцій повторення без idempotency key може створити дубль; такі операції не слід повторювати без гарантії ідемпотентності.

## Як запустити проєкт

```bash <img width="787" height="218" alt="Знімок екрана 2026-11-18 о 22 40 23" src="https://github.com/user-attachments/assets/7f7614a2-0a99-41e4-9e4a-16b05edab2ee"
dotnet run
```
<img width="418" height="215" alt="Знімок екрана 2026-11-18 о 22 42 18" src="https://github.com/user-attachments/assets/4a6f13ce-432e-48db-ac7a-60f95bfbaaee" />
