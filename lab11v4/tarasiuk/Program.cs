using System;
using System.Collections.Generic;

namespace OrderProcessingApp
{
    // =========================================================================
    // 1. АБСТРАКТНИЙ БАЗОВИЙ КЛАС (Оголошення Template Method та його кроків)
    // =========================================================================
    public abstract class OrderProcessor
    {
        // Властивості для зберігання стану та даних обробки
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal FinalAmount { get; protected set; }
        public bool IsValidated { get; protected set; }
        public bool IsPaid { get; protected set; }
        public string Status { get; protected set; }

        protected OrderProcessor(int orderId, string customerName, decimal baseAmount)
        {
            OrderId = orderId;
            CustomerName = customerName;
            BaseAmount = baseAmount;
            FinalAmount = baseAmount;
            IsValidated = false;
            IsPaid = false;
            Status = "Created";
        }

        // --- Template Method (Шаблонний метод) ---
        // Визначає чіткий скелет/послідовність алгоритму обробки замовлення
        public void ProcessOrder()
        {
            Console.WriteLine($"\n==========================================");
            Console.WriteLine($"Розпочато обробку замовлення #{OrderId} ({CustomerName})");
            Console.WriteLine($"==========================================");

            if (!ValidateOrder())
            {
                Status = "Validation Failed";
                Console.WriteLine($"[Error] Замовлення #{OrderId} не пройшло валідацію! Обробку зупинено.");
                return;
            }

            CalculateTotal();
            ApplyDiscounts();
            
            if (ProcessPayment())
            {
                IsPaid = true;
                Status = "Completed";
                SendConfirmation();
            }
            else
            {
                Status = "Payment Failed";
                Console.WriteLine($"[Error] Помилка оплати для замовлення #{OrderId}.");
            }

            Console.WriteLine($"[Статус замовлення #{OrderId}]: {Status}\n");
        }

        // --- Абстрактні кроки (Обов'язкові для реалізації в похідних класах) ---
        protected abstract bool ValidateOrder();
        protected abstract bool ProcessPayment();

        // --- Віртуальні кроки (За замовчуванням мають базову реалізацію, можна перевизначати) ---
        protected virtual void CalculateTotal()
        {
            Console.WriteLine($"[1. Розрахунок]: Базова вартість = {BaseAmount:C}");
            FinalAmount = BaseAmount;
        }

        protected virtual void ApplyDiscounts()
        {
            Console.WriteLine($"[2. Знижки]: Знижки за замовчуванням відсутні.");
        }

        protected virtual void SendConfirmation()
        {
            Console.WriteLine($"[3. Підтвердження]: Надіслано загальне сповіщення для {CustomerName}.");
        }
    }

    // =========================================================================
    // 2. ПОХІДНИЙ КЛАС 1: OnlineOrder (Онлайн-замовлення)
    // =========================================================================
    public class OnlineOrder : OrderProcessor
    {
        public string DeliveryAddress { get; set; }
        public string PromoCode { get; set; }

        public OnlineOrder(int orderId, string customerName, decimal baseAmount, string deliveryAddress, string promoCode = "")
            : base(orderId, customerName, baseAmount)
        {
            DeliveryAddress = deliveryAddress;
            PromoCode = promoCode;
        }

        protected override bool ValidateOrder()
        {
            Console.WriteLine($"[Валідація Online]: Перевірка адреси доставки ({DeliveryAddress}) та наявності товарів...");
            IsValidated = !string.IsNullOrWhiteSpace(DeliveryAddress) && BaseAmount > 0;
            return IsValidated;
        }

        protected override void ApplyDiscounts()
        {
            if (!string.IsNullOrEmpty(PromoCode))
            {
                decimal discount = FinalAmount * 0.10m; // 10% знижки за промокодом
                FinalAmount -= discount;
                Console.WriteLine($"[Знижка Online]: Застосовано промокод '{PromoCode}' (-10%: -{discount:C}). Підсумок: {FinalAmount:C}");
            }
            else
            {
                Console.WriteLine($"[Знижка Online]: Промокод відсутній.");
            }
        }

        protected override bool ProcessPayment()
        {
            Console.WriteLine($"[Оплата Online]: Списання суми {FinalAmount:C} через електронний платіжний шлюз (LiqPay/Stripe)...");
            return true; // Успішно
        }

        protected override void SendConfirmation()
        {
            Console.WriteLine($"[Підтвердження Online]: Email із квитанцією та трек-номером надіслано клієнту {CustomerName}.");
        }
    }

    // =========================================================================
    // 3. ПОХІДНИЙ КЛАС 2: InStoreOrder (Замовлення в магазині / Каса)
    // =========================================================================
    public class InStoreOrder : OrderProcessor
    {
        public int RegisterNumber { get; set; }
        public bool HasLoyaltyCard { get; set; }

        public InStoreOrder(int orderId, string customerName, decimal baseAmount, int registerNumber, bool hasLoyaltyCard)
            : base(orderId, customerName, baseAmount)
        {
            RegisterNumber = registerNumber;
            HasLoyaltyCard = hasLoyaltyCard;
        }

        protected override bool ValidateOrder()
        {
            Console.WriteLine($"[Валідація InStore]: Перевірка касової зміни на касі №{RegisterNumber}...");
            IsValidated = BaseAmount > 0 && RegisterNumber > 0;
            return IsValidated;
        }

        protected override void ApplyDiscounts()
        {
            if (HasLoyaltyCard)
            {
                decimal discount = FinalAmount * 0.05m; // 5% знижки за карткою лояльності
                FinalAmount -= discount;
                Console.WriteLine($"[Знижка InStore]: Картка лояльності сканована (-5%: -{discount:C}). Підсумок: {FinalAmount:C}");
            }
            else
            {
                Console.WriteLine($"[Знижка InStore]: Картка лояльності відсутня.");
            }
        }

        protected override bool ProcessPayment()
        {
            Console.WriteLine($"[Оплата InStore]: Прийом готівки або оплати через POS-термінал на касі №{RegisterNumber} на суму {FinalAmount:C}...");
            return true; // Успішно
        }

        protected override void SendConfirmation()
        {
            Console.WriteLine($"[Підтвердження InStore]: Друкується паперовий фіскальний чек для {CustomerName}.");
        }
    }

    // =========================================================================
    // 4. ПОХІДНИЙ КЛАС 3: PhoneOrder (Телефонне замовлення)
    // =========================================================================
    public class PhoneOrder : OrderProcessor
    {
        public string OperatorName { get; set; }
        public string PhoneNumber { get; set; }

        public PhoneOrder(int orderId, string customerName, decimal baseAmount, string operatorName, string phoneNumber)
            : base(orderId, customerName, baseAmount)
        {
            OperatorName = operatorName;
            PhoneNumber = phoneNumber;
        }

        protected override bool ValidateOrder()
        {
            Console.WriteLine($"[Валідація Phone]: Оператор {OperatorName} підтверджує номер телефону ({PhoneNumber})...");
            IsValidated = !string.IsNullOrWhiteSpace(PhoneNumber) && BaseAmount > 0;
            return IsValidated;
        }

        protected override void CalculateTotal()
        {
            base.CalculateTotal();
            decimal serviceFee = 20.00m; // Додаткова комісія за обробку оператором
            FinalAmount += serviceFee;
            Console.WriteLine($"[Розрахунок Phone]: Додано комісію за телефонне обслуговування (+{serviceFee:C}). До сплати: {FinalAmount:C}");
        }

        protected override bool ProcessPayment()
        {
            Console.WriteLine($"[Оплата Phone]: Створення рахунку для оплати при отриманні (накладений платіж) на суму {FinalAmount:C}...");
            return true; // Успішно
        }

        protected override void SendConfirmation()
        {
            Console.WriteLine($"[Підтвердження Phone]: Надіслано SMS-підтвердження на номер {PhoneNumber}.");
        }
    }

    // =========================================================================
    // 5. ДЕМОНСТРАЦІЯ РОБОТИ (Метод Main)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Створення списку замовлень типу базового класу (Поліморфізм)
            List<OrderProcessor> orders = new List<OrderProcessor>
            {
                new OnlineOrder(101, "Олексій Попов", 1500.00m, "м. Київ, вул. Хрещатик, 1", "SALE2026"),
                new InStoreOrder(102, "Марія Ковальчук", 450.00m, 3, true),
                new PhoneOrder(103, "Іван Петренко", 800.00m, "Анна", "+380671234567")
            };

            Console.WriteLine("=== СИСТЕМА ОБРОБКИ ЗАМОВЛЕНЬ (Template Method Pattern) ===");

            // Виклик шаблонного методу для кожного замовлення
            foreach (var order in orders)
            {
                order.ProcessOrder();
            }

            Console.WriteLine("Усі замовлення успішно оброблені!");
        }
    }
}