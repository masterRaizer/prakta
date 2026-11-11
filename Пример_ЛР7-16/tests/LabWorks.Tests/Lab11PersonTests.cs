using System;
using LabWorks.Core.People;
using LabWorks.Core.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LabWorks.Tests
{
    /// <summary>ЛР 11. Класс Person.</summary>
    [TestClass]
    public class Lab11PersonTests
    {
        [TestMethod]
        public void AgeAndYearsTo100()
        {
            foreach (var c in OracleData.Person())
            {
                var p = new Person { FirstName = "Иван", LastName = "Петров", BirthDate = c.Birthday };
                Assert.AreEqual(c.ExpectedAge, p.Age(c.Today), c.Birthday.ToShortDateString());
                Assert.AreEqual(c.ExpectedFeature, p.YearsTo100(c.Today).ToString(), c.Birthday.ToShortDateString());
            }
        }

        [TestMethod]
        public void ManualAgeFormula_IsWrongOnBirthday()
        {
            // Найдено при инспекции кода: Int(дни / 365.25) ошибается в день рождения.
            var birthday = new DateTime(1952, 11, 11);
            var today = new DateTime(2026, 11, 11);
            Assert.AreEqual(73, Person.AgeAsInManual(birthday, today));
            Assert.AreEqual(74, Person.AgeOn(birthday, today));
        }

        [TestMethod]
        public void Person_ValidatesInput()
        {
            var p = new Person { FirstName = " Иван ", LastName = "Петров", BirthDate = new DateTime(2000, 5, 1) };
            Assert.AreEqual("Петров Иван", p.FullName);
            Assert.ThrowsException<ArgumentException>(() => p.FirstName = " ");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => p.Age(new DateTime(1999, 1, 1)));
        }
    }
}
