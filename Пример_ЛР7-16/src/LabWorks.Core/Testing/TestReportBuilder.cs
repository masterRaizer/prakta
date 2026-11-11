using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using LabWorks.Core.Formulas;

namespace LabWorks.Core.Testing
{
    /// <summary>Итоговый отчёт о результатах тестирования (ЛР 16) по структуре из методички.</summary>
    public sealed class TestReportBuilder
    {
        public string Author { get; set; }
        public string BuildNumber { get; set; }
        public string ProjectLink { get; set; }
        public string Platform { get; set; }

        public TestReportBuilder()
        {
            Author = "Студент";
            BuildNumber = "1.0.0";
            ProjectLink = "LabWorks.sln";
            Platform = Environment.OSVersion + ", .NET CLR " + Environment.Version;
        }

        public static string Quality(IList<TestOutcome> results)
        {
            int failed = results.Count(r => !r.Passed);
            return results.Count > 0 && failed * 100.0 / results.Count < 10 ? "Приемлемое" : "Неприемлемое";
        }

        public string BuildText(IList<TestOutcome> results, DateTime date)
        {
            Formula formula = DemoVariant.Formula;
            int failed = results.Count(r => !r.Passed);
            var sb = new StringBuilder();
            sb.AppendLine("ИТОГОВЫЙ ОТЧЁТ О РЕЗУЛЬТАТАХ ТЕСТИРОВАНИЯ");
            sb.AppendLine("Программа вычисления параметра c, демонстрационный вариант (" + formula.Text + ")");
            sb.AppendLine();
            sb.AppendLine("1. Приветствие");
            sb.AppendLine("Здравствуйте! Направляю итоговый отчёт о тестировании программы. Тестирование выполнено в полном объёме, "
                          + "задержек и внешних факторов, мешающих проверке, не было.");
            sb.AppendLine();
            sb.AppendLine("2. Общая информация");
            sb.AppendLine("Проведены виды тестов: модульные (методы ParseOperand, Compute, FormatResult), функциональные "
                          + "(классы эквивалентности и граничные значения), интеграционные (взаимодействие трёх методов в Process).");
            foreach (var g in results.GroupBy(r => r.Category))
                sb.AppendLine("  • " + g.Key + ": " + g.Count() + " тестов, пройдено " + g.Count(r => r.Passed));
            sb.AppendLine();
            sb.AppendLine("3. Тестовое окружение");
            sb.AppendLine("  • Название проекта: LabWorks (решение Visual Studio, C#, Windows Forms)");
            sb.AppendLine("  • Номер сборки: " + BuildNumber);
            sb.AppendLine("  • Ссылка на проект: " + ProjectLink);
            sb.AppendLine("  • Платформа: " + Platform);
            sb.AppendLine("  • Дата тестирования: " + date.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture));
            sb.AppendLine();
            sb.AppendLine("4. Рекомендации QA");
            sb.AppendLine("  • Качество сборки: " + Quality(results) + " (не пройдено " + failed + " из " + results.Count + ", "
                          + (results.Count == 0 ? 0 : failed * 100.0 / results.Count).ToString("0.#", CultureInfo.InvariantCulture) + "%; порог — 10%).");
            if (failed == 0)
            {
                sb.AppendLine("  • Заблокированного функционала нет, критичных дефектов не обнаружено.");
                sb.AppendLine("  • Рекомендации: сохранить набор тестов как регрессионный и выполнять его при каждом изменении формулы или проверок ввода.");
            }
            else
            {
                sb.AppendLine("  • Наиболее критичные дефекты (исправить в первую очередь):");
                foreach (var r in results.Where(r => !r.Passed))
                    sb.AppendLine("    – " + r.Category + " " + r.Id + ": " + r.Description + " — ожидалось «" + r.Expected + "», получено «" + r.Actual + "»");
            }
            sb.AppendLine("  • Область определения формулы: " + formula.DomainText + ".");
            sb.AppendLine();
            sb.AppendLine("5. Детализированная информация");
            sb.AppendLine("Smoke: " + (results.Where(r => r.Category == SelfTestRunner.Unit).All(r => r.Passed) ? "Приемлемое" : "Неприемлемое")
                          + " — основные методы программы работают.");
            foreach (var r in results)
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "  [{0}] {1} {2}: {3}; вход: {4}; ожидалось: {5}; получено: {6}; {7:0.###} мс",
                    r.Passed ? "ПРОЙДЕН" : "НЕ ПРОЙДЕН", r.Category, r.Id, r.Description, r.Input, r.Expected, r.Actual, r.Milliseconds));
            sb.AppendLine();
            sb.AppendLine("6. Окончание");
            sb.AppendLine("  • Тест-план: раздел «Интеграционное тестирование» отчёта по ЛР 15.");
            sb.AppendLine("  • Общее количество новых дефектов: " + failed + ".");
            sb.AppendLine("  • Отчёт подготовил: " + Author + ".");
            return sb.ToString();
        }

        public string BuildHtml(IList<TestOutcome> results, DateTime date)
        {
            var sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><html lang=\"ru\"><head><meta charset=\"utf-8\"><title>Отчёт о тестировании</title><style>body{font-family:'Times New Roman',serif;font-size:12pt;margin:2em}")
              .Append("table{border-collapse:collapse}td,th{border:1px solid #888;padding:3px 6px}.f{color:#b00}</style></head><body><pre>")
              .Append(WebUtility.HtmlEncode(BuildText(results, date))).Append("</pre><table><tr><th>Вид</th><th>№</th><th>Описание</th>")
              .Append("<th>Вход</th><th>Ожидалось</th><th>Получено</th><th>Итог</th></tr>");
            foreach (var r in results)
                sb.Append("<tr").Append(r.Passed ? "" : " class=\"f\"").Append("><td>").Append(WebUtility.HtmlEncode(r.Category)).Append("</td><td>")
                  .Append(WebUtility.HtmlEncode(r.Id)).Append("</td><td>").Append(WebUtility.HtmlEncode(r.Description)).Append("</td><td>")
                  .Append(WebUtility.HtmlEncode(r.Input)).Append("</td><td>").Append(WebUtility.HtmlEncode(r.Expected)).Append("</td><td>")
                  .Append(WebUtility.HtmlEncode(r.Actual)).Append("</td><td>").Append(r.Passed ? "пройден" : "не пройден").Append("</td></tr>");
            sb.Append("</table></body></html>");
            return sb.ToString();
        }
    }
}
