using System;

namespace lab4v4
{
    public class Temperature
    {
        // 1. Приватні поля
        private double _value;
        private string _unit;

        // 3. Статичний член
        public static readonly double AbsoluteZeroCelsius = -273.15;

        // Конструктор
        public Temperature(double value, string unit)
        {
            Unit = unit;   // Валідація одиниці виміру
            Value = value; // Валідація значення
        }

        // 2. Публічні властивості з валідацією
        public string Unit
        {
            get => _unit;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Одиниця виміру не може бути порожньою.");

                string formattedUnit = value.Trim();

                if (!formattedUnit.Equals("Celsius", StringComparison.OrdinalIgnoreCase) &&
                    !formattedUnit.Equals("Fahrenheit", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Некоректна одиниця виміру. Дозволено лише 'Celsius' або 'Fahrenheit'.");
                }

                _unit = char.ToUpper(formattedUnit[0]) + formattedUnit.Substring(1).ToLower();
            }
        }

        public double Value
        {
            get => _value;
            set
            {
                if (_unit == "Celsius" && value < AbsoluteZeroCelsius)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"Температура не може бути нижчою за абсолютний нуль ({AbsoluteZeroCelsius}°C).");
                }

                double absoluteZeroFahrenheit = AbsoluteZeroCelsius * 9 / 5 + 32;
                if (_unit == "Fahrenheit" && value < absoluteZeroFahrenheit)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"Температура не може бути нижчою за абсолютний нуль ({absoluteZeroFahrenheit}°F).");
                }

                _value = value;
            }
        }

        // 4. Індексатор
        public object this[int index]
        {
            get
            {
                return index switch
                {
                    0 => Value,
                    1 => Unit,
                    _ => throw new IndexOutOfRangeException("Індекс має бути 0 (Value) або 1 (Unit).")
                };
            }
        }

        // 5. Перевантажені оператори
        public static Temperature operator +(Temperature t1, Temperature t2)
        {
            if (t1 is null || t2 is null)
                throw new ArgumentNullException("Об'єкти не можуть бути null.");

            if (!t1.Unit.Equals(t2.Unit, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Додавання можливе лише для однакових одиниць виміру.");
            }

            return new Temperature(t1.Value + t2.Value, t1.Unit);
        }

        public static bool operator ==(Temperature t1, Temperature t2)
        {
            if (ReferenceEquals(t1, t2)) return true;
            if (t1 is null || t2 is null) return false;

            return t1.Equals(t2);
        }

        public static bool operator !=(Temperature t1, Temperature t2)
        {
            return !(t1 == t2);
        }

        // 6. Перевизначені методи ToString(), Equals(), GetHashCode()
        public override string ToString()
        {
            string symbol = Unit == "Celsius" ? "°C" : "°F";
            return $"{Value} {symbol}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Temperature other)
            {
                return Math.Abs(this.Value - other.Value) < 0.0001 &&
                       this.Unit.Equals(other.Unit, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Unit.ToLower());
        }
    }
}
