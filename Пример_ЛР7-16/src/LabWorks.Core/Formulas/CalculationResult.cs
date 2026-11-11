namespace LabWorks.Core.Formulas
{
    /// <summary>Результат обработки входных строк программой варианта.</summary>
    public sealed class CalculationResult
    {
        public ErrorKind Kind { get; set; }
        public double Value { get; set; }
        public string Text { get; set; }
        public string Message { get; set; }
        public bool Success { get { return Kind == ErrorKind.None; } }

        public override string ToString()
        {
            return Success ? "c = " + Text : "Ошибка (" + Kind + "): " + Message;
        }
    }
}
