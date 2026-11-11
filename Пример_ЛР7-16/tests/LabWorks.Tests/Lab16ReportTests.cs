using System;
using LabWorks.Core.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LabWorks.Tests
{
    /// <summary>ЛР 16. Отчёт содержит все разделы из методички.</summary>
    [TestClass]
    public class Lab16ReportTests
    {
        [TestMethod]
        public void Report_HasAllSections()
        {
            var results = new SelfTestRunner().RunAll();
            string text = new TestReportBuilder().BuildText(results, new DateTime(2026, 11, 11));
            foreach (string section in new[] { "Приветствие", "Общая информация", "Тестовое окружение", "Рекомендации QA", "Детализированная информация", "Окончание" })
                StringAssert.Contains(text, section);
            StringAssert.Contains(new TestReportBuilder().BuildHtml(results, DateTime.Now), "<table>");
        }
    }
}
