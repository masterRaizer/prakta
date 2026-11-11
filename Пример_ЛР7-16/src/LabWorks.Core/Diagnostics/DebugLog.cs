using System;
using System.Diagnostics;

namespace LabWorks.Core.Diagnostics
{
    /// <summary>
    /// Отладочный журнал (ЛР 10). В конфигурации Debug сообщения идут в окно Output (Debug.WriteLine),
    /// в любой конфигурации — подписчикам события Message (например, в окно журнала программы).
    /// </summary>
    public static class DebugLog
    {
        public static event Action<string> Message;

        public static void Write(string text)
        {
            string line = DateTime.Now.ToString("HH:mm:ss.fff") + "  " + text;
#if DEBUG
            Debug.WriteLine(line, "LabWorks");
#endif
            Trace.WriteLine(line);
            var handler = Message;
            if (handler != null)
                handler(line);
        }
    }
}
