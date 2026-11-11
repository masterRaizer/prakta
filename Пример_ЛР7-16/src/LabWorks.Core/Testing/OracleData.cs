using System;
using System.Collections.Generic;

namespace LabWorks.Core.Testing
{
    /// <summary>Функциональный тест (ЛР 14): класс эквивалентности или граничное значение.</summary>
    public sealed class FunctionalCase
    {
        public int Variant { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public ErrorKind ExpectedKind { get; set; }
        public double ExpectedValue { get; set; }
    }

    public sealed class CalculatorCase
    {
        public int Variant { get; set; }
        public string Operation { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        /// <summary>"None" или имя ожидаемого исключения.</summary>
        public string ExpectedException { get; set; }
        public double ExpectedValue { get; set; }
    }

    public sealed class PersonCase
    {
        public int Variant { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime Today { get; set; }
        public int ExpectedAge { get; set; }
        public string ExpectedFeature { get; set; }
    }

    /// <summary>
    /// Эталонные данные для тестов демонстрационного варианта. Ожидаемые значения посчитаны отдельно
    /// (вручную/калькулятором), а не самой программой — иначе тест проверял бы программу ею же.
    /// </summary>
    public static class OracleData
    {
        private static readonly List<FunctionalCase> FunctionalCases = new List<FunctionalCase>();
        private static readonly List<CalculatorCase> CalculatorCases = new List<CalculatorCase>();
        private static readonly List<CalculatorCase> BasicCases = new List<CalculatorCase>();
        private static readonly List<PersonCase> PersonCases = new List<PersonCase>();

        static OracleData()
        {
            // ЛР 14: классы эквивалентности (К) и граничные значения (Г) для c = (a² − b) / (a + 2b).
            AddFunctional(0, "К1", "Допустимые значения (типичный случай)", "3", "1", ErrorKind.None, 1.6);
            AddFunctional(0, "К2", "Десятичная запятая во входных данных", "2,5", "1,5", ErrorKind.None, 0.8636363636363636);
            AddFunctional(0, "К3", "Отрицательные значения", "-3", "-2", ErrorKind.None, -1.5714285714285714);
            AddFunctional(0, "К4", "Нечисловое значение a", "abc", "1", ErrorKind.Format, double.NaN);
            AddFunctional(0, "К5", "Пустое значение b", "3", "", ErrorKind.Format, double.NaN);
            AddFunctional(0, "К6", "Значение NaN", "NaN", "1", ErrorKind.Format, double.NaN);
            AddFunctional(0, "К7", "a больше верхней границы диапазона", "1000.001", "1", ErrorKind.Range, double.NaN);
            AddFunctional(0, "К8", "b меньше нижней границы диапазона", "3", "-1000.001", ErrorKind.Range, double.NaN);
            AddFunctional(0, "К9", "Значения вне области определения (a + 2b = 0)", "4", "-2", ErrorKind.Domain, double.NaN);
            AddFunctional(0, "Г1", "Граница диапазона: a = 1000", "1000", "1", ErrorKind.None, 998.002994011976);
            AddFunctional(0, "Г2", "Граница диапазона: b = −1000", "3", "-1000", ErrorKind.None, -0.5052578868302454);
            AddFunctional(0, "Г3", "Рядом с границей области: a + 2b = 1", "4", "-1.5", ErrorKind.None, 17.5);

            // ЛР 8: основные операции (double) и остаток от деления целых чисел.
            AddBasic("+", 2.5, 1.5, "None", 4.0);
            AddBasic("−", 2.5, 4, "None", -1.5);
            AddBasic("×", -3, 4, "None", -12.0);
            AddBasic("÷", 7, 2, "None", 3.5);
            AddBasic("÷", 7, 0, "DivideByZeroException", double.NaN);
            AddBasic("×", 1e308, 10, "OverflowException", double.NaN);
            AddCalculator(0, 17, 5, "None", 2);
            AddCalculator(0, -17, 5, "None", -2);
            AddCalculator(0, 5, 0, "DivideByZeroException", double.NaN);
            AddCalculator(0, int.MinValue, -1, "OverflowException", double.NaN);

            // ЛР 11: возраст и «лет до 100-летия».
            AddPerson(0, new DateTime(2005, 3, 15), new DateTime(2026, 11, 11), 21, "79");
            AddPerson(0, new DateTime(2000, 2, 29), new DateTime(2026, 11, 11), 26, "74");
            AddPerson(0, new DateTime(1952, 11, 11), new DateTime(2026, 11, 11), 74, "26");
            AddPerson(0, new DateTime(1920, 1, 1), new DateTime(2026, 11, 11), 106, "0");
        }

        public static IEnumerable<FunctionalCase> Functional()
        {
            return FunctionalCases;
        }

        public static IEnumerable<CalculatorCase> Calculator()
        {
            return CalculatorCases;
        }

        public static IEnumerable<CalculatorCase> Basic()
        {
            return BasicCases;
        }

        public static IEnumerable<PersonCase> Person()
        {
            return PersonCases;
        }

        private static void AddFunctional(int v, string id, string description, string a, string b, ErrorKind kind, double value)
        {
            FunctionalCases.Add(new FunctionalCase { Variant = v, Id = id, Description = description, A = a, B = b, ExpectedKind = kind, ExpectedValue = value });
        }

        private static void AddCalculator(int v, double a, double b, string expected, double value)
        {
            CalculatorCases.Add(new CalculatorCase { Variant = v, Operation = "доп.", A = a, B = b, ExpectedException = expected, ExpectedValue = value });
        }

        private static void AddBasic(string operation, double a, double b, string expected, double value)
        {
            BasicCases.Add(new CalculatorCase { Variant = 0, Operation = operation, A = a, B = b, ExpectedException = expected, ExpectedValue = value });
        }

        private static void AddPerson(int v, DateTime birthday, DateTime today, int age, string feature)
        {
            PersonCases.Add(new PersonCase { Variant = v, Birthday = birthday, Today = today, ExpectedAge = age, ExpectedFeature = feature });
        }

        /// <summary>Сравнение чисел с относительной погрешностью 1e−9.</summary>
        public static bool Close(double expected, double actual)
        {
            if (double.IsNaN(expected) || double.IsNaN(actual))
                return double.IsNaN(expected) && double.IsNaN(actual);
            double scale = Math.Max(1.0, Math.Max(Math.Abs(expected), Math.Abs(actual)));
            return Math.Abs(expected - actual) <= 1e-9 * scale;
        }
    }
}
