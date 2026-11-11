using System;
using LabWorks.Core.Calculator;
using LabWorks.Core.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LabWorks.Tests
{
    /// <summary>ЛР 8. Обработка исключительных ситуаций в калькуляторе.</summary>
    [TestClass]
    public class Lab08CalculatorTests
    {
        private static void ExpectException(string expected, Action action)
        {
            try
            {
                action();
                Assert.Fail("Ожидалось исключение " + expected);
            }
            catch (AssertFailedException) { throw; }
            catch (Exception ex) { Assert.AreEqual(expected, ex.GetType().Name); }
        }

        [TestMethod]
        public void BasicOperations()
        {
            var calc = new SafeCalculator();
            foreach (var c in OracleData.Basic())
            {
                if (c.ExpectedException == "None")
                    Assert.AreEqual(c.ExpectedValue, calc.Basic(c.Operation, c.A, c.B), 1e-12, c.Operation);
                else
                    ExpectException(c.ExpectedException, () => calc.Basic(c.Operation, c.A, c.B));
            }
        }

        [TestMethod]
        public void IntegerRemainder()
        {
            var calc = new SafeCalculator();
            foreach (var c in OracleData.Calculator())
            {
                if (c.ExpectedException == "None")
                    Assert.AreEqual((int)c.ExpectedValue, calc.Remainder((int)c.A, (int)c.B));
                else
                    ExpectException(c.ExpectedException, () => calc.Remainder((int)c.A, (int)c.B));
            }
        }

        [TestMethod]
        public void DoubleDivisionByZero_DoesNotThrowByItself()
        {
            // Поэтому catch (DivideByZeroException) в примере из методички (float) не срабатывает.
            double zero = 0;
            Assert.IsTrue(double.IsPositiveInfinity(1 / zero));
            Assert.ThrowsException<DivideByZeroException>(() => new SafeCalculator().Divide(1, 0));
        }

        [TestMethod]
        public void Parsing_FormatAndOverflow()
        {
            Assert.AreEqual(-3.25, SafeCalculator.ParseNumber("-3,25", "a"));
            Assert.ThrowsException<FormatException>(() => SafeCalculator.ParseNumber("", "a"));
            Assert.ThrowsException<FormatException>(() => SafeCalculator.ParseNumber("12a", "a"));
            Assert.ThrowsException<FormatException>(() => SafeCalculator.ParseNumber("NaN", "a"));
            Assert.ThrowsException<FormatException>(() => SafeCalculator.ParseInteger("2.5", "a"));
            Assert.ThrowsException<OverflowException>(() => SafeCalculator.ParseInteger("2147483648", "a"));
        }
    }
}
