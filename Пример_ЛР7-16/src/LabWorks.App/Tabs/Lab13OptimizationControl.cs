using System;
using System.Globalization;
using System.Windows.Forms;
using LabWorks.Core.Optimization;

namespace LabWorks.App.Tabs
{
    /// <summary>ЛР 13. Сравнение времени исходной и оптимизированной версий модуля табулирования.</summary>
    public class Lab13OptimizationControl : UserControl
    {
        private readonly NumericUpDown _aStep = new NumericUpDown { Minimum = 1, Maximum = 500, Value = 10, Width = 70 };
        private readonly NumericUpDown _bStep = new NumericUpDown { Minimum = 1, Maximum = 500, Value = 50, Width = 70 };
        private readonly DataGridView _grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private readonly Label _summary = new Label { AutoSize = true };

        public Lab13OptimizationControl()
        {
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44 };
            top.Controls.Add(UiHelpers.MakeLabel("Шаг по a:"));
            top.Controls.Add(_aStep);
            top.Controls.Add(UiHelpers.MakeLabel("Шаг по b:"));
            top.Controls.Add(_bStep);
            top.Controls.Add(UiHelpers.MakeButton("Сравнить версии", Run, 160));
            var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 60 };
            bottom.Controls.Add(_summary);
            _grid.Columns.Add("name", "Версия");
            _grid.Columns.Add("ms", "Время, мс");
            _grid.Columns.Add("ex", "Исключений");
            _grid.Columns.Add("points", "Точек всего / вычислено");
            _grid.Columns.Add("sum", "Сумма c");
            Controls.Add(_grid);
            Controls.Add(bottom);
            Controls.Add(top);
        }

        private void Run(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                BenchmarkResult[] results = new Tabulator().Benchmark((double)_aStep.Value, (double)_bStep.Value, 3);
                _grid.Rows.Clear();
                foreach (var r in results)
                    _grid.Rows.Add(r.Name, r.Milliseconds.ToString("0.0", CultureInfo.CurrentCulture), r.ExceptionsThrown,
                        r.Summary.Points + " / " + r.Summary.Valid, r.Summary.Sum.ToString("G10", CultureInfo.CurrentCulture));
                bool same = results[0].Summary.Csv == results[1].Summary.Csv;
                _summary.Text = "Ускорение: в " + (results[0].Milliseconds / Math.Max(results[1].Milliseconds, 0.001)).ToString("0.0", CultureInfo.CurrentCulture)
                                + " раза. Результаты " + (same ? "совпадают — семантика программы сохранена." : "НЕ совпадают — ошибка оптимизации!");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
