namespace LabWorks.Core.Formulas
{
    /// <summary>
    /// Демонстрационный вариант: c = (a² − b) / (a + 2b).
    /// Это пример, он не совпадает ни с одним вариантом из таблицы заданий.
    /// Для своего варианта замените формулу, текст и проверку области определения.
    /// </summary>
    public static class DemoVariant
    {
        public static readonly Formula Formula = new Formula(
            0,
            "c = (a² − b) / (a + 2b)",
            "a + 2b ≠ 0",
            (a, b) => (a * a - b) / (a + 2 * b),
            (a, b) => a + 2 * b == 0 ? "a + 2b = 0: знаменатель равен нулю" : null);
    }
}
