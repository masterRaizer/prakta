using System;
using System.Threading;
using System.Windows.Forms;
using LabWorks.Core.Diagnostics;

namespace LabWorks.App
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // ЛР 8, 10: глобальная обработка необработанных исключений — программа не завершается аварийно.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                MessageBox.Show(e.ExceptionObject.ToString(), "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            DebugLog.Write("Необработанное исключение: " + e.Exception);
            MessageBox.Show("Произошла ошибка: " + e.Exception.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
