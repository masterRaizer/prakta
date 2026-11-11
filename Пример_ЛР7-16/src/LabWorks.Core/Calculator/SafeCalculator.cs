using System;
using System.Globalization;

namespace LabWorks.Core.Calculator
{
    /// <summary>
    /// Калькулятор с обработкой исключительных ситуаций (ЛР 8).
    ///
    /// На что обратить внимание:
    /// 1) Для double деление на ноль НЕ вызывает DivideByZeroException — результат равен бесконечности.
    ///    Поэтому в примере из методички (float) блок catch (DivideByZeroException) никогда не сработает.
    ///    Здесь исключение генерируется явно оператором throw.
    /// 2) Для целых чисел (int) деление на ноль само вызывает DivideByZeroException —
    ///    это показывает дополнительная операция «остаток от деления целых чисел».
    /// 3) Переполнение int ловится только в блоке checked, иначе результат «перескакивает» через границу.
    /// </summary>
    public sealed class SafeCalculator
    {
        /// <summary>Разбор вещественного числа; допускаются запятая и точка. Иначе — FormatException.</summary>
        public static double ParseNumber(string text, string name)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new FormatException("Не введено " + name);
            double value;
            if (!double.TryParse(text.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || double.IsNaN(value) || double.IsInfinity(value))
                throw new FormatException("Неправильный формат числа: «" + text + "» (" + name + ")");
            return value;
        }

        /// <summary>Разбор целого числа: FormatException или OverflowException (число вне диапазона int).</summary>
        public static int ParseInteger(string text, string name)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new FormatException("Не введено " + name);
            // int.Parse сам генерирует FormatException и OverflowException — их перехватывает вызывающий код.
            return int.Parse(text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        public double Add(double a, double b) { return Check(a + b); }
        public double Subtract(double a, double b) { return Check(a - b); }
        public double Multiply(double a, double b) { return Check(a * b); }

        public double Divide(double a, double b)
        {
            if (b == 0)
                throw new DivideByZeroException("Нельзя делить на ноль!");
            return Check(a / b);
        }

        /// <summary>Операция по символу: +, −, ×, ÷.</summary>
        public double Basic(string symbol, double a, double b)
        {
            switch (symbol)
            {
                case "+": return Add(a, b);
                case "−": case "-": return Subtract(a, b);
                case "×": case "*": return Multiply(a, b);
                case "÷": case "/": return Divide(a, b);
                default: throw new ArgumentException("Неизвестная операция: " + symbol);
            }
        }

        /// <summary>Дополнительная операция примера: остаток от деления целых чисел (DivideByZeroException, OverflowException).</summary>
        public int Remainder(int a, int b)
        {
            // int.MinValue % −1 вызывает OverflowException; при b = 0 — DivideByZeroException (без явного throw).
            return checked(a % b);
        }

        /// <summary>Проверка результата: бесконечность или NaN означают переполнение.</summary>
        internal static double Check(double result)
        {
            if (double.IsInfinity(result) || double.IsNaN(result))
                throw new OverflowException("Результат вне диапазона типа double");
            return result;
        }
    }
}
