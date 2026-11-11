namespace LabWorks.Core.Testing
{
    /// <summary>Результат выполнения одного теста программой (ЛР 14–16).</summary>
    public sealed class TestOutcome
    {
        public string Category { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public string Input { get; set; }
        public string Expected { get; set; }
        public string Actual { get; set; }
        public bool Passed { get; set; }
        public double Milliseconds { get; set; }
    }
}
