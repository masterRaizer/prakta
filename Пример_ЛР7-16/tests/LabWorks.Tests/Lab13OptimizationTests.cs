using LabWorks.Core.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LabWorks.Tests
{
    /// <summary>ЛР 13. Оптимизация не должна менять результат (сохранение семантики).</summary>
    [TestClass]
    public class Lab13OptimizationTests
    {
        [TestMethod]
        public void OptimizedEqualsNaive()
        {
            var t = new Tabulator();
            int exceptions;
            var naive = t.Naive(50, 100, out exceptions);
            var optimized = t.Optimized(50, 100);
            Assert.AreEqual(naive.Points, optimized.Points);
            Assert.AreEqual(naive.Valid, optimized.Valid);
            Assert.AreEqual(naive.Invalid, exceptions);
            Assert.AreEqual(naive.Csv, optimized.Csv);
            Assert.AreEqual(naive.Sum, optimized.Sum);
        }
    }
}
