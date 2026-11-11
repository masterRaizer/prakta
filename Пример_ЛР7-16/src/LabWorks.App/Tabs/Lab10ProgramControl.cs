using System;
using System.Globalization;
using System.Windows.Forms;
using LabWorks.Core.Diagnostics;
using LabWorks.Core.Formulas;

namespace LabWorks.App.Tabs
{
    /// <summary>
    /// ЛР 10, 15. Программа варианта: ввод a и b (демо-данные, ручной ввод или случайные значения),
    /// вычисление c, таблица и график c(a) при фиксированном b, журнал отладки.
    /// Для отладки (ЛР 10): поставьте точку останова в методе Calculate, добавьте a, b, result в окно Watch,
    /// пройдите по шагам (F10 — шаг с обходом, F11 — шаг с заходом в VariantCalculator.Process).
    /// </summary>
    public class Lab10ProgramControl : UserControl
    {
        private readonly VariantCalculator _calculator = new VariantCalculator();
        private readonly Random _random = new Random();
        private readonly TextBox _a = new TextBox { Width = 110 };
        private readonly TextBox _b = new TextBox { Width = 110 };
        private readonly RadioButton _demo = new RadioButton { Text = "Демо-данные", AutoSize = true, Checked = true };
        private readonly RadioButton _manual = new RadioButton { Text = "Ввод вручную", AutoSize = true };
        private readonly RadioButton _randomData = new RadioButton { Text = "Случайные", AutoSize = true };
        private readonly Label _result = new Label { AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12F) };
        private readonly ErrorProvider _errors = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };
        private readonly DataGridView _grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private readonly PlotPanel _chart = new PlotPanel { Dock = DockStyle.Fill };
        private readonly TextBox _log = UiHelpers.MakeLog();

        public Lab10ProgramControl()
        {
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 74 };
            top.Controls.Add(UiHelpers.MakeLabel(_calculator.Formula.Text + "   (область: " + _calculator.Formula.DomainText + "; a, b ∈ [−1000; 1000])", true));
            top.SetFlowBreak(top.Controls[0], true);
            top.Controls.Add(_demo);
            top.Controls.Add(_manual);
            top.Controls.Add(_randomData);
            top.Controls.Add(UiHelpers.MakeLabel("a ="));
            top.Controls.Add(_a);
            top.Controls.Add(UiHelpers.MakeLabel("b ="));
            top.Controls.Add(_b);
            top.Controls.Add(UiHelpers.MakeButton("Вычислить c", (s, e) => Calculate(), 120));
            top.Controls.Add(UiHelpers.MakeButton("Таблица и график c(a)", (s, e) => Tabulate(), 170));

            var resultPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 32 };
            resultPanel.Controls.Add(_result);


            var split = new SplitContainer { Dock = DockStyle.Fill };
            split.Panel1.Controls.Add(_grid);
            var right = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
            right.Panel1.Controls.Add(_chart);
            right.Panel2.Controls.Add(_log);
            split.Panel2.Controls.Add(right);

            Controls.Add(split);
            Controls.Add(resultPanel);
            Controls.Add(top);
            // Размеры панелей задаются после появления окна: раньше ширина SplitContainer ещё не известна,
            // и слишком большое значение SplitterDistance вызвало бы исключение.
            Load += (s, e) =>
            {
                split.SplitterDistance = Math.Min(300, Math.Max(split.Panel1MinSize, split.Width / 3));
                right.SplitterDistance = Math.Min(260, Math.Max(right.Panel1MinSize, right.Height * 2 / 3));
            };

            _demo.CheckedChanged += (s, e) => { if (_demo.Checked) LoadDemo(); };
            _randomData.CheckedChanged += (s, e) => { if (_randomData.Checked) LoadRandom(); };
            _manual.CheckedChanged += (s, e) => { if (_manual.Checked) { _a.ReadOnly = false; _b.ReadOnly = false; _a.Focus(); } };
            DebugLog.Message += line => { if (IsHandleCreated) BeginInvoke((Action)(() => UiHelpers.Append(_log, line))); };
            LoadDemo();
        }

        private void LoadDemo()
        {
            _a.Text = "3";
            _b.Text = "1";
            _a.ReadOnly = _b.ReadOnly = true;
        }

        private void LoadRandom()
        {
            // Случайные значения из диапазона [−100; 100] с шагом 0,5.
            _a.Text = (_random.Next(-200, 201) / 2.0).ToString(CultureInfo.CurrentCulture);
            _b.Text = (_random.Next(-200, 201) / 2.0).ToString(CultureInfo.CurrentCulture);
            _a.ReadOnly = _b.ReadOnly = true;
        }

        private void Calculate()
        {
            if (_randomData.Checked)
                LoadRandom();
            _errors.Clear();
            CalculationResult result = _calculator.Process(_a.Text, _b.Text);
            if (result.Success)
            {
                _result.ForeColor = System.Drawing.Color.DarkGreen;
                _result.Text = "c = " + result.Text;
                return;
            }
            _result.ForeColor = System.Drawing.Color.DarkRed;
            _result.Text = result.ToString();
            // Подсветка поля с ошибкой (ErrorProvider).
            if (result.Message.Contains(" a ") || result.Message.StartsWith("Значение a"))
                _errors.SetError(_a, result.Message);
            else if (result.Message.Contains(" b ") || result.Message.StartsWith("Значение b"))
                _errors.SetError(_b, result.Message);
        }

        private void Tabulate()
        {
            double b;
            try
            {
                b = _calculator.ParseOperand(_b.Text, "b");
            }
            catch (Exception ex)
            {
                _errors.SetError(_b, ex.Message);
                return;
            }
            var table = new System.Data.DataTable();
            table.Columns.Add("a", typeof(double));
            table.Columns.Add("c", typeof(string));
            var points = new System.Collections.Generic.List<System.Drawing.PointF>();
            for (int i = -20; i <= 20; i++)
            {
                double a = i;
                double c;
                if (_calculator.Formula.TryEvaluate(a, b, out c))
                {
                    table.Rows.Add(a, _calculator.FormatResult(c));
                    points.Add(new System.Drawing.PointF((float)a, (float)c));
                }
                else
                {
                    table.Rows.Add(a, "не определено");
                }
            }
            _grid.DataSource = table;
            _chart.SetData(points, "c(a) при b = " + _calculator.FormatResult(b));
            DebugLog.Write("Таблица c(a) построена для b = " + b);
        }
    }
}
