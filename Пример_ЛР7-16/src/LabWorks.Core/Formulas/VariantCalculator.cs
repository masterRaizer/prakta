using System;
using System.Diagnostics;
using System.Globalization;
using LabWorks.Core.Diagnostics;

namespace LabWorks.Core.Formulas
{
    /// <summary>
    /// Класс программы варианта (ЛР 10, 15). Три взаимодействующих метода:
    /// ParseOperand → Compute → FormatResult; метод Process объединяет их (интеграционное тестирование
    /// проверяет именно эту связку: один вызов Process затрагивает все три метода).
    /// </summary>
    public sealed class VariantCalculator
    {
        /// <summary>Допустимый диапазон входных значений: [−Limit; Limit].</summary>
        public const double Limit = 1000;

        public VariantCalculator()
            : this(DemoVariant.Formula)
        {
        }

        public VariantCalculator(Formula formula)
        {
            Formula = formula;
        }

        public Formula Formula { get; private set; }

        /// <summary>Метод 1: разбор и проверка входного значения (запятая и точка допускаются).</summary>
        public double ParseOperand(string text, string name)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new InputFormatException("Значение " + name + " не задано");
            string normalized = text.Trim().Replace(',', '.');
            double value;
            if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || double.IsNaN(value) || double.IsInfinity(value))
                throw new InputFormatException("Значение " + name + " = «" + text + "» не является числом");
            if (Math.Abs(value) > Limit)
                throw new InputRangeException(name, "Значение " + name + " = " + value.ToString(CultureInfo.InvariantCulture)
                    + " вне диапазона [−" + Limit + "; " + Limit + "]");
            return value;
        }

        /// <summary>Метод 2: вычисление параметра c по формуле варианта.</summary>
        public double Compute(double a, double b)
        {
            Debug.Assert(Math.Abs(a) <= Limit && Math.Abs(b) <= Limit, "Значения должны быть проверены методом ParseOperand");
            double c = Formula.Evaluate(a, b);
            DebugLog.Write("Compute: a = " + a + ", b = " + b + ", c = " + c);
            return c;
        }

        /// <summary>Метод 3: форматирование результата (10 значащих цифр).</summary>
        public string FormatResult(double c, IFormatProvider provider)
        {
            return c.ToString("G10", provider ?? CultureInfo.CurrentCulture);
        }

        public string FormatResult(double c)
        {
            return FormatResult(c, null);
        }

        /// <summary>Интеграция трёх методов: строки a, b → результат с видом ошибки.</summary>
        public CalculationResult Process(string aText, string bText, IFormatProvider provider = null)
        {
            var result = new CalculationResult();
            try
            {
                double a = ParseOperand(aText, "a");
                double b = ParseOperand(bText, "b");
                result.Value = Compute(a, b);
                result.Text = FormatResult(result.Value, provider);
                result.Kind = ErrorKind.None;
            }
            catch (InputFormatException ex) { Fail(result, ErrorKind.Format, ex.Message); }
            catch (InputRangeException ex) { Fail(result, ErrorKind.Range, ex.Reason); }
            catch (DomainException ex) { Fail(result, ErrorKind.Domain, ex.Message); }
            catch (OverflowException ex) { Fail(result, ErrorKind.Overflow, ex.Message); }
            return result;
        }

        private static void Fail(CalculationResult result, ErrorKind kind, string message)
        {
            result.Kind = kind;
            result.Value = double.NaN;
            result.Message = message;
            DebugLog.Write("Ошибка " + kind + ": " + message);
        }
    }
}
