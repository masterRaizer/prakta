using System;
using System.Globalization;
using System.Windows.Forms;
using LabWorks.Core.Calculator;

namespace LabWorks.App.Tabs
{
    /// <summary>
    /// ЛР 8. Калькулятор с обработкой исключений: тот же пример, что в методичке (консольный), но в Windows Forms.
    /// Каждая ошибка перехватывается своим блоком catch, программа продолжает работу.
    /// </summary>
    public class Lab08CalculatorControl : UserControl
    {
        private readonly TextBox _a = new TextBox { Width = 140, Text = "7" };
        private readonly TextBox _b = new TextBox { Width = 140, Text = "0" };
        private readonly Label _result = new Label { AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12F) };
        private readonly TextBox _log = UiHelpers.MakeLog();
        private readonly SafeCalculator _calculator = new SafeCalculator();

        public Lab08CalculatorControl()
        {
            var input = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
            input.Controls.Add(UiHelpers.MakeLabel("Первое число:"));
            input.Controls.Add(_a);
            input.Controls.Add(UiHelpers.MakeLabel("Второе число:"));
            input.Controls.Add(_b);

            var ops = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44 };
            foreach (string op in new[] { "+", "−", "×", "÷" })
            {
                string symbol = op;
                ops.Controls.Add(UiHelpers.MakeButton(symbol, (s, e) => Calculate(symbol), 60));
            }
            ops.Controls.Add(UiHelpers.MakeButton("a mod b (целые числа)", (s, e) => Remainder(), 200));

            var resultPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 36 };
            resultPanel.Controls.Add(_result);

            Controls.Add(_log);
            var header = UiHelpers.MakeLabel("Перехваченные исключения:", true);
            header.Dock = DockStyle.Top;
            Controls.Add(header);
            Controls.Add(resultPanel);
            Controls.Add(ops);
            Controls.Add(input);
        }

        private void Calculate(string op)
        {
            try
            {
                double a = SafeCalculator.ParseNumber(_a.Text, "первое число");
                double b = SafeCalculator.ParseNumber(_b.Text, "второе число");
                double r = _calculator.Basic(op, a, b);
                Show(a.ToString(CultureInfo.CurrentCulture) + " " + op + " " + b.ToString(CultureInfo.CurrentCulture) + " = " + r.ToString("G10", CultureInfo.CurrentCulture));
            }
            catch (FormatException ex) { Error("FormatException", ex.Message); }
            catch (DivideByZeroException ex) { Error("DivideByZeroException", ex.Message); }
            catch (OverflowException ex) { Error("OverflowException", ex.Message); }
        }

        private void Remainder()
        {
            try
            {
                int a = SafeCalculator.ParseInteger(_a.Text, "первое число");
                int b = SafeCalculator.ParseInteger(_b.Text, "второе число");
                Show(a + " mod " + b + " = " + _calculator.Remainder(a, b));
            }
            catch (FormatException ex) { Error("FormatException", "Нужно целое число: " + ex.Message); }
            catch (OverflowException ex) { Error("OverflowException", ex.Message); }
            catch (DivideByZeroException ex) { Error("DivideByZeroException", ex.Message); }
            finally
            {
                // Блок finally выполняется всегда — и при успехе, и после catch.
                UiHelpers.Append(_log, "  (finally: операция mod завершена)");
            }
        }

        private void Show(string text)
        {
            _result.ForeColor = System.Drawing.Color.DarkGreen;
            _result.Text = text;
        }

        private void Error(string type, string message)
        {
            _result.ForeColor = System.Drawing.Color.DarkRed;
            _result.Text = "Ошибка: " + message;
            UiHelpers.Append(_log, DateTime.Now.ToString("HH:mm:ss") + "  " + type + ": " + message);
        }
    }
}
