using System;

namespace LabWorks.Core.Formulas
{
    /// <summary>Формула варианта c = f(a, b) с проверкой области определения.</summary>
    public sealed class Formula
    {
        private readonly Func<double, double, double> _function;
        private readonly Func<double, double, string> _domainCheck;

        public Formula(int variant, string text, string domainText,
                       Func<double, double, double> function, Func<double, double, string> domainCheck)
        {
            Variant = variant;
            Text = text;
            DomainText = domainText;
            _function = function;
            _domainCheck = domainCheck;
        }

        public int Variant { get; private set; }

        /// <summary>Запись формулы, например «c = (a + b) / (a − b)».</summary>
        public string Text { get; private set; }

        /// <summary>Описание области определения.</summary>
        public string DomainText { get; private set; }

        public bool HasDomainRestriction { get { return _domainCheck != null; } }

        /// <summary>Причина, по которой (a, b) вне области определения, или null.</summary>
        public string CheckDomain(double a, double b)
        {
            return _domainCheck == null ? null : _domainCheck(a, b);
        }

        /// <summary>Вычисление с исключениями: DomainException, OverflowException.</summary>
        public double Evaluate(double a, double b)
        {
            string reason = CheckDomain(a, b);
            if (reason != null)
                throw new DomainException(reason);
            double c = _function(a, b);
            if (double.IsNaN(c))
                throw new DomainException("Результат не определён для заданных значений");
            if (double.IsInfinity(c))
                throw new OverflowException("Результат слишком велик для типа double");
            return c;
        }

        /// <summary>Вычисление без исключений (используется оптимизированным модулем ЛР 13).</summary>
        public bool TryEvaluate(double a, double b, out double c)
        {
            c = 0;
            if (_domainCheck != null && _domainCheck(a, b) != null)
                return false;
            c = _function(a, b);
            return !double.IsNaN(c) && !double.IsInfinity(c);
        }
    }
}
