using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LabWorks.Core;
using LabWorks.Core.Formulas;
using LabWorks.Core.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LabWorks.Tests
{
    /// <summary>
    /// ЛР 14–15. Тестирующий класс: функциональные тесты (классы эквивалентности, граничные значения)
    /// и интеграционные тесты — один вызов Process проверяет все три метода сразу.
    /// </summary>
    [TestClass]
    public class Lab15IntegrationTests
    {
        public static IEnumerable<object[]> Cases()
        {
            return OracleData.Functional().Select(c => new object[] { c.Id, c.A, c.B, c.ExpectedKind, c.ExpectedValue });
        }

        [DataTestMethod]
        [DynamicData(nameof(Cases), DynamicDataSourceType.Method)]
        public void Process_MatchesExpected(string id, string a, string b, ErrorKind expectedKind, double expectedValue)
        {
            var result = new VariantCalculator().Process(a, b, CultureInfo.InvariantCulture);
            Assert.AreEqual(expectedKind, result.Kind, id + ": " + result.Message);
            if (expectedKind == ErrorKind.None)
                Assert.IsTrue(OracleData.Close(expectedValue, result.Value), id + ": ожидалось " + expectedValue + ", получено " + result.Value);
        }

        [TestMethod]
        public void Process_EqualsChainOfThreeMethods()
        {
            var calc = new VariantCalculator();
            foreach (var c in OracleData.Functional().Where(c => c.ExpectedKind == ErrorKind.None))
            {
                string chain = calc.FormatResult(calc.Compute(calc.ParseOperand(c.A, "a"), calc.ParseOperand(c.B, "b")), CultureInfo.InvariantCulture);
                Assert.AreEqual(chain, calc.Process(c.A, c.B, CultureInfo.InvariantCulture).Text, c.Id);
            }
        }

        [TestMethod]
        public void SelfTestRunner_AllTestsPass()
        {
            var results = new SelfTestRunner().RunAll();
            foreach (var r in results)
                Assert.IsTrue(r.Passed, r.Category + " " + r.Id + ": ожидалось «" + r.Expected + "», получено «" + r.Actual + "»");
            Assert.AreEqual("Приемлемое", TestReportBuilder.Quality(results));
        }

        [TestMethod]
        public void ParseOperand_AcceptsCommaAndPoint()
        {
            var calc = new VariantCalculator();
            Assert.AreEqual(2.5, calc.ParseOperand("2,5", "a"));
            Assert.AreEqual(2.5, calc.ParseOperand(" 2.5 ", "a"));
            Assert.AreEqual(-1000, calc.ParseOperand("-1000", "a"));
        }

        [TestMethod]
        public void ParseOperand_RejectsBadInput()
        {
            var calc = new VariantCalculator();
            foreach (string bad in new[] { "", "  ", "abc", "NaN", "Infinity", "1..2" })
                Assert.ThrowsException<InputFormatException>(() => calc.ParseOperand(bad, "a"), "«" + bad + "»");
            Assert.ThrowsException<InputRangeException>(() => calc.ParseOperand("1000.001", "a"));
        }
    }
}
