using System;
using System.Collections.Generic;
using System.Text;

namespace Lab18App
{
    // =========================================================================
    // 1. ВЛАСНІ ВИНЯТКИ (Custom Exceptions)
    // =========================================================================
    public class InsufficientFundsException : Exception
    {
        public decimal CurrentBalance { get; }
        public decimal AttemptedAmount { get; }

        public InsufficientFundsException()
            : base("Недостатньо коштів на рахунку для виконання операції.") { }

        public InsufficientFundsException(string message)
            : base(message) { }

        public InsufficientFundsException(string message, Exception inner)
            : base(message, inner) { }

        public InsufficientFundsException(decimal currentBalance, decimal attemptedAmount)
            : base($"Недостатньо коштів: баланс {currentBalance:C}, запитувана сума {attemptedAmount:C}.")
        {
            CurrentBalance = currentBalance;
            AttemptedAmount = attemptedAmount;
        }
    }

    public class AccountBlockedException : Exception
    {
        public string AccountNumber { get; }

        public AccountBlockedException(string accountNumber)
            : base($"Операція відхилена: рахунок '{accountNumber}' заблоковано.")
        {
            AccountNumber = accountNumber;
        }
    }

    // =========================================================================
    // 2. УЗАГАЛЬНЕНИЙ КЛАС ПАТЕРНУ RESULT (Result<T>)
    // =========================================================================
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string Error { get; }

        private Result(T? value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, string.Empty);

        public static Result<T> Failure(string error) => new Result<T>(default, false, error);

        public static Result<T> Failure(Exception ex) => new Result<T>(default, false, ex.Message);
    }

    // =========================================================================
    // 3. МОДЕЛЬ ДАНИХ (BankAccount)
    // =========================================================================
    public class BankAccount
    {
        public string AccountNumber { get; }
        public string Owner { get; }
        public decimal Balance { get; set; }
        public bool IsBlocked { get; set; }

        public BankAccount(string accountNumber, string owner, decimal initialBalance, bool isBlocked = false)
        {
            AccountNumber = accountNumber;
            Owner = owner;
            Balance = initialBalance;
            IsBlocked = isBlocked;
        }

        public override string ToString() =>
            $"Рахунок {AccountNumber} ({Owner}) — Баланс: {Balance:C} [{(IsBlocked ? "Заблоковано" : "Активний")}]";
    }

    // =========================================================================
    // 4. БІЗНЕС-СЕРВІС (BankService)
    // =========================================================================
    public class BankService
    {
        private readonly Dictionary<string, BankAccount> _accounts = new Dictionary<string, BankAccount>();

        public BankService()
        {
            _accounts["UA1001"] = new BankAccount("UA1001", "Тарасюк Іван", 5000m);
            _accounts["UA1002"] = new BankAccount("UA1002", "Коваленко Марія", 1200m);
            _accounts["UA9999"] = new BankAccount("UA9999", "Петренко Олег", 300m, isBlocked: true);
        }

        // Метод 1: Використовує власні винятки для критичних помилок
        public decimal WithdrawWithException(string accountNumber, decimal amount)
        {
            if (!_accounts.TryGetValue(accountNumber, out var account))
            {
                throw new KeyNotFoundException($"Рахунок '{accountNumber}' не знайдено в системі.");
            }

            if (account.IsBlocked)
            {
                throw new AccountBlockedException(accountNumber);
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Сума зняття повинна бути більшою за нуль.");
            }

            if (account.Balance < amount)
            {
                throw new InsufficientFundsException(account.Balance, amount);
            }

            account.Balance -= amount;
            return account.Balance;
        }

        // Метод 2: Використовує патерн Result<T> для очікуваних сценаріїв невдачі (без винятків)
        public Result<decimal> TryTransfer(string fromAccountNum, string toAccountNum, decimal amount)
        {
            if (amount <= 0)
            {
                return Result<decimal>.Failure("Сума переказу має бути більшою за 0.");
            }

            if (!_accounts.TryGetValue(fromAccountNum, out var fromAccount))
            {
                return Result<decimal>.Failure($"Рахунок відправника '{fromAccountNum}' не існує.");
            }

            if (!_accounts.TryGetValue(toAccountNum, out var toAccount))
            {
                return Result<decimal>.Failure($"Рахунок отримувача '{toAccountNum}' не існує.");
            }

            if (fromAccount.IsBlocked || toAccount.IsBlocked)
            {
                return Result<decimal>.Failure("Один із рахунків заблоковано. Переказ неможливий.");
            }

            if (fromAccount.Balance < amount)
            {
                return Result<decimal>.Failure(
                    $"Недостатньо коштів для переказу. Доступно: {fromAccount.Balance:C}, потрібно: {amount:C}."
                );
            }

            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            return Result<decimal>.Success(fromAccount.Balance);
        }

        // Метод 3: Пошук рахунку через Result<T>
        public Result<BankAccount> FindAccount(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return Result<BankAccount>.Failure("Номер рахунку не може бути порожнім.");
            }

            if (_accounts.TryGetValue(accountNumber, out var account))
            {
                return Result<BankAccount>.Success(account);
            }

            return Result<BankAccount>.Failure($"Рахунок з номером '{accountNumber}' не знайдено.");
        }
    }

    // =========================================================================
    // 5. ДЕМОНСТРАЦІЯ РОБОТИ (Program)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("==========================================================================");
            Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №18. ВЛАСНІ ВИНЯТКИ ТА ПАТЕРН RESULT");
            Console.WriteLine("==========================================================================\n");

            BankService bankService = new BankService();

            // -------------------------------------------------------------------------
            // ЧАСТИНА 1: Робота з власними винятками (try-catch)
            // -------------------------------------------------------------------------
            Console.WriteLine("--- 1. ОБРОБКА КРИТИЧНИХ ПОМИЛОК ЧЕРЕЗ ВЛАСНІ ВИНЯТКИ (try-catch) ---");

            // Спроба 1: Недостатньо коштів
            try
            {
                Console.WriteLine("Спроба зняття 8000 грн з рахунку UA1001...");
                decimal newBalance = bankService.WithdrawWithException("UA1001", 8000m);
                Console.WriteLine($"Успішно! Новий баланс: {newBalance:C}");
            }
            catch (InsufficientFundsException ex)
            {
                Console.WriteLine($" [InsufficientFundsException] {ex.Message}");
                Console.WriteLine($"   Деталі: поточний баланс = {ex.CurrentBalance:C}, спроба зняти = {ex.AttemptedAmount:C}\n");
            }

            // Спроба 2: Заблокований рахунок
            try
            {
                Console.WriteLine("Спроба зняття 100 грн із заблокованого рахунку UA9999...");
                bankService.WithdrawWithException("UA9999", 100m);
            }
            catch (AccountBlockedException ex)
            {
                Console.WriteLine($" [AccountBlockedException] {ex.Message} (Рахунок: {ex.AccountNumber})\n");
            }

            // -------------------------------------------------------------------------
            // ЧАСТИНА 2: Робота з патерном Result<T> (без викидання винятків)
            // -------------------------------------------------------------------------
            Console.WriteLine("--- 2. КОНТРОЛЬОВАНА ОБРОБКА ПОМИЛОК ЧЕРЕЗ ПАТЕРН RESULT<T> ---");

            // Сценарій A: Очікувана помилка (неіснуючий рахунок)
            var searchResult = bankService.FindAccount("UA0000");
            if (searchResult.IsSuccess)
            {
                Console.WriteLine($" Знайдено: {searchResult.Value}");
            }
            else
            {
                Console.WriteLine($" [Result Failure] Пошук рахунку: {searchResult.Error}");
            }

            // Сценарій B: Очікувана помилка при переказі (недостатньо коштів)
            var failedTransfer = bankService.TryTransfer("UA1002", "UA1001", 5000m);
            if (!failedTransfer.IsSuccess)
            {
                Console.WriteLine($" [Result Failure] Переказ коштів: {failedTransfer.Error}");
            }

            // Сценарій C: Успішний переказ коштів
            var successTransfer = bankService.TryTransfer("UA1001", "UA1002", 1500m);
            if (successTransfer.IsSuccess)
            {
                Console.WriteLine($" [Result Success] Переказ 1500 грн виконано! Залишок на UA1001: {successTransfer.Value:C}");
            }

            Console.WriteLine("\n==========================================================================");
            Console.WriteLine("  ДЕМОНСТРАЦІЮ ВЛАСНИХ ВИНЯТКІВ ТА ПАТЕРНУ RESULT ЗАВЕРШЕНО");
            Console.WriteLine("==========================================================================");
        }
    }
}