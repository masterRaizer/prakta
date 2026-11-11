namespace LabWorks.Core
{
    /// <summary>Вид результата расчёта: успех или категория ошибки.</summary>
    public enum ErrorKind
    {
        /// <summary>Ошибок нет.</summary>
        None,
        /// <summary>Значение не является числом или не задано.</summary>
        Format,
        /// <summary>Значение вне допустимого диапазона [−1000; 1000].</summary>
        Range,
        /// <summary>Значения вне области определения формулы.</summary>
        Domain,
        /// <summary>Результат слишком велик (переполнение).</summary>
        Overflow
    }
}
