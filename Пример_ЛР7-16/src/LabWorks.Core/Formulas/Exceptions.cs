using System;

namespace LabWorks.Core.Formulas
{
    /// <summary>Значение не задано или не является числом.</summary>
    public class InputFormatException : FormatException
    {
        public InputFormatException(string message) : base(message) { }
    }

    /// <summary>Значение вне допустимого диапазона.</summary>
    public class InputRangeException : ArgumentOutOfRangeException
    {
        public InputRangeException(string paramName, string message) : base(paramName, message)
        {
            Reason = message;
        }

        /// <summary>Сообщение без добавляемого платформой имени параметра.</summary>
        public string Reason { get; private set; }
    }

    /// <summary>Значения вне области определения формулы варианта.</summary>
    public class DomainException : ArgumentException
    {
        public DomainException(string message) : base(message) { }
    }
}
