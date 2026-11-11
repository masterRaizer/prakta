using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using LabWorks.Core.Formulas;

namespace LabWorks.Core.Testing
{
    /// <summary>
    /// Выполнение тестов программы варианта из самой программы (ЛР 14–16):
    /// модульные тесты каждого из трёх методов, функциональные (классы эквивалентности, границы)
    /// и интеграционные (связка ParseOperand → Compute → FormatResult).
    /// </summary>
    public sealed class SelfTestRunner
    {
        public const string Unit = "Модульный";
        public const string FunctionalCategory = "Функциональный";
        public const string Integration = "Интеграционный";

        private readonly VariantCalculator _calculator = new VariantCalculator();

        public List<TestOutcome> RunAll()
        {
            var list = new List<TestOutcome>();
            list.AddRange(RunUnit());
            list.AddRange(RunFunctional());
            list.AddRange(RunIntegration());
            return list;
        }

        public List<TestOutcome> RunUnit()
        {
            var list = new List<TestOutcome>();
            list.Add(Measure(Unit, "М1", "ParseOperand: число с запятой", "«2,5»", "2.5", () =>
                _calculator.ParseOperand("2,5", "a").ToString(CultureInfo.InvariantCulture)));
            list.Add(Measure(Unit, "М2", "ParseOperand: нечисловая строка", "«x1»", "InputFormatException", () =>
                Expect<InputFormatException>(() => _calculator.ParseOperand("x1", "a"))));
            list.Add(Measure(Unit, "М3", "ParseOperand: значение вне диапазона", "«-1000.5»", "InputRangeException", () =>
                Expect<InputRangeException>(() => _calculator.ParseOperand("-1000.5", "a"))));
            FunctionalCase typical = null;
            foreach (var c in OracleData.Functional())
                if (c.Id == "К1") typical = c;
            if (typical != null)
            {
                double a = double.Parse(typical.A, CultureInfo.InvariantCulture);
                double b = double.Parse(typical.B, CultureInfo.InvariantCulture);
                list.Add(Measure(Unit, "М4", "Compute: типичные значения", "a = " + typical.A + ", b = " + typical.B,
                    typical.ExpectedValue.ToString("R", CultureInfo.InvariantCulture), () =>
                    {
                        double c = _calculator.Compute(a, b);
                        return OracleData.Close(typical.ExpectedValue, c) ? typical.ExpectedValue.ToString("R", CultureInfo.InvariantCulture) : c.ToString("R", CultureInfo.InvariantCulture);
                    }));
            }
            list.Add(Measure(Unit, "М5", "FormatResult: 10 значащих цифр", "1/3", "0.3333333333", () =>
                _calculator.FormatResult(1.0 / 3.0, CultureInfo.InvariantCulture)));
            return list;
        }

        public List<TestOutcome> RunFunctional()
        {
            var list = new List<TestOutcome>();
            foreach (var c in OracleData.Functional())
            {
                FunctionalCase test = c;
                string expected = Describe(test.ExpectedKind, test.ExpectedValue);
                list.Add(Measure(FunctionalCategory, test.Id, test.Description, "a = «" + test.A + "», b = «" + test.B + "»", expected, () =>
                {
                    CalculationResult r = _calculator.Process(test.A, test.B, CultureInfo.InvariantCulture);
                    if (r.Kind == test.ExpectedKind && (r.Kind != ErrorKind.None || OracleData.Close(test.ExpectedValue, r.Value)))
                        return expected;
                    return Describe(r.Kind, r.Value);
                }));
            }
            return list;
        }

        public List<TestOutcome> RunIntegration()
        {
            var list = new List<TestOutcome>();
            int n = 0;
            foreach (var c in OracleData.Functional())
            {
                if (c.ExpectedKind != ErrorKind.None) continue;
                FunctionalCase test = c;
                n++;
                list.Add(Measure(Integration, "И" + n, "Process = FormatResult(Compute(Parse(a), Parse(b)))",
                    "a = «" + test.A + "», b = «" + test.B + "»", "совпадение", () =>
                    {
                        var calc = new VariantCalculator();
                        string chain = calc.FormatResult(calc.Compute(calc.ParseOperand(test.A, "a"), calc.ParseOperand(test.B, "b")), CultureInfo.InvariantCulture);
                        string process = calc.Process(test.A, test.B, CultureInfo.InvariantCulture).Text;
                        return chain == process ? "совпадение" : "расхождение: " + chain + " ≠ " + process;
                    }));
            }
            n++;
            list.Add(Measure(Integration, "И" + n, "Ошибка метода 1 передаётся в результат Process", "a = «abc»", "Format", () =>
                _calculator.Process("abc", "1").Kind.ToString()));
            return list;
        }

        public static string Describe(ErrorKind kind, double value)
        {
            return kind == ErrorKind.None ? "c = " + value.ToString("G10", CultureInfo.InvariantCulture) : "ошибка " + kind;
        }

        private static string Expect<T>(Action action) where T : Exception
        {
            try
            {
                action();
                return "исключения нет";
            }
            catch (T)
            {
                return typeof(T).Name;
            }
            catch (Exception ex)
            {
                return ex.GetType().Name;
            }
        }

        private static TestOutcome Measure(string category, string id, string description, string input, string expected, Func<string> run)
        {
            var sw = Stopwatch.StartNew();
            string actual;
            try
            {
                actual = run();
            }
            catch (Exception ex)
            {
                actual = "необработанное исключение " + ex.GetType().Name;
            }
            sw.Stop();
            return new TestOutcome
            {
                Category = category, Id = id, Description = description, Input = input,
                Expected = expected, Actual = actual, Passed = actual == expected, Milliseconds = sw.Elapsed.TotalMilliseconds
            };
        }
    }
}
