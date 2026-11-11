using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using LabWorks.Core.Formulas;

namespace LabWorks.Core.Optimization
{
    /// <summary>Итог табулирования: одинаков для исходной и оптимизированной версий.</summary>
    public sealed class TabulationSummary
    {
        public int Points { get; set; }
        public int Valid { get; set; }
        public int Invalid { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Sum { get; set; }
        public string Csv { get; set; }
    }

    public sealed class BenchmarkResult
    {
        public string Name { get; set; }
        public double Milliseconds { get; set; }
        public int ExceptionsThrown { get; set; }
        public TabulationSummary Summary { get; set; }
    }

    /// <summary>
    /// Модуль табулирования c(a, b) на сетке (ЛР 13). Параметр оптимизации — время выполнения.
    /// Naive — исходная версия, Optimized — после оптимизации; результаты совпадают (семантика сохранена).
    /// Применённые способы оптимизации (по методичке):
    ///  • разгрузка участков повторяемости — объект-калькулятор и сетка значений b создаются один раз, а не в цикле;
    ///  • упрощение действий — нет преобразования числа в строку и обратно для каждой точки;
    ///  • чистка программы — исключения не используются для управления ходом (TryEvaluate вместо try/catch);
    ///  • экономия памяти — StringBuilder вместо конкатенации строк (конкатенация копирует всю строку каждый раз).
    /// </summary>
    public sealed class Tabulator
    {
        private readonly Formula _formula;

        public Tabulator()
            : this(DemoVariant.Formula)
        {
        }

        public Tabulator(Formula formula)
        {
            _formula = formula;
        }

        /// <summary>Исходная версия: строки вместо чисел, исключения для управления ходом, конкатенация строк.</summary>
        public TabulationSummary Naive(double aStep, double bStep, out int exceptions)
        {
            exceptions = 0;
            var summary = new TabulationSummary { Min = double.MaxValue, Max = double.MinValue };
            string csv = "a;b;c\n";
            for (double a = -VariantCalculator.Limit; a <= VariantCalculator.Limit; a += aStep)
            {
                for (double b = -VariantCalculator.Limit; b <= VariantCalculator.Limit; b += bStep)
                {
                    // Каждый раз создаётся новый объект и числа преобразуются в строки и обратно.
                    var calculator = new VariantCalculator(_formula);
                    summary.Points++;
                    try
                    {
                        double pa = calculator.ParseOperand(a.ToString("R", CultureInfo.InvariantCulture), "a");
                        double pb = calculator.ParseOperand(b.ToString("R", CultureInfo.InvariantCulture), "b");
                        double c = calculator.Formula.Evaluate(pa, pb);
                        summary.Valid++;
                        summary.Sum += c;
                        summary.Min = Math.Min(summary.Min, c);
                        summary.Max = Math.Max(summary.Max, c);
                        csv = csv + Line(a, b, c);
                    }
                    catch (Exception)
                    {
                        exceptions++;
                        summary.Invalid++;
                    }
                }
            }
            summary.Csv = csv;
            return summary;
        }

        /// <summary>Оптимизированная версия: без исключений, без лишних объектов и преобразований, StringBuilder.</summary>
        public TabulationSummary Optimized(double aStep, double bStep)
        {
            Formula formula = _formula;
            double[] bValues = Grid(bStep);
            var summary = new TabulationSummary { Min = double.MaxValue, Max = double.MinValue };
            var csv = new StringBuilder(64 * bValues.Length);
            csv.Append("a;b;c\n");
            for (double a = -VariantCalculator.Limit; a <= VariantCalculator.Limit; a += aStep)
            {
                for (int j = 0; j < bValues.Length; j++)
                {
                    double b = bValues[j];
                    double c;
                    summary.Points++;
                    if (Math.Abs(a) > VariantCalculator.Limit || Math.Abs(b) > VariantCalculator.Limit || !formula.TryEvaluate(a, b, out c))
                    {
                        summary.Invalid++;
                        continue;
                    }
                    summary.Valid++;
                    summary.Sum += c;
                    if (c < summary.Min) summary.Min = c;
                    if (c > summary.Max) summary.Max = c;
                    csv.Append(Line(a, b, c));
                }
            }
            summary.Csv = csv.ToString();
            return summary;
        }

        /// <summary>Значения сетки по b — те же, что даёт цикл с накоплением шага в исходной версии.</summary>
        private static double[] Grid(double step)
        {
            int count = 0;
            for (double x = -VariantCalculator.Limit; x <= VariantCalculator.Limit; x += step) count++;
            var values = new double[count];
            int i = 0;
            for (double x = -VariantCalculator.Limit; x <= VariantCalculator.Limit; x += step) values[i++] = x;
            return values;
        }

        private static string Line(double a, double b, double c)
        {
            return a.ToString("R", CultureInfo.InvariantCulture) + ";" + b.ToString("R", CultureInfo.InvariantCulture) + ";"
                   + c.ToString("R", CultureInfo.InvariantCulture) + "\n";
        }

        /// <summary>Замер времени обеих версий (с предварительным «прогревом»).</summary>
        public BenchmarkResult[] Benchmark(double aStep, double bStep, int repeats)
        {
            int ex;
            Naive(aStep * 10, bStep * 10, out ex);
            Optimized(aStep * 10, bStep * 10);
            var naive = new BenchmarkResult { Name = "Исходная версия" };
            var optimized = new BenchmarkResult { Name = "Оптимизированная версия" };
            var sw = new Stopwatch();
            for (int r = 0; r < repeats; r++)
            {
                sw.Restart();
                naive.Summary = Naive(aStep, bStep, out ex);
                sw.Stop();
                naive.Milliseconds += sw.Elapsed.TotalMilliseconds / repeats;
                naive.ExceptionsThrown = ex;
                sw.Restart();
                optimized.Summary = Optimized(aStep, bStep);
                sw.Stop();
                optimized.Milliseconds += sw.Elapsed.TotalMilliseconds / repeats;
            }
            return new[] { naive, optimized };
        }
    }
}
