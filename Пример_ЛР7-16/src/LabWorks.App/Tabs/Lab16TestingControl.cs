using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using LabWorks.Core.Testing;

namespace LabWorks.App.Tabs
{
    /// <summary>ЛР 14–16. Выполнение тестов из программы и итоговый отчёт о тестировании.</summary>
    public class Lab16TestingControl : UserControl
    {
        private readonly DataGridView _grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells };
        private readonly Label _summary = new Label { AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
        private List<TestOutcome> _results = new List<TestOutcome>();

        public Lab16TestingControl()
        {
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44 };
            top.Controls.Add(UiHelpers.MakeButton("Выполнить все тесты", Run, 170));
            top.Controls.Add(UiHelpers.MakeButton("Сохранить отчёт (ЛР 16)", Save, 190));
            top.Controls.Add(_summary);
            foreach (string c in new[] { "Вид", "№", "Описание", "Входные данные", "Ожидалось", "Получено", "Итог", "мс" })
                _grid.Columns.Add(c, c);
            Controls.Add(_grid);
            Controls.Add(top);
        }

        private void Run(object sender, EventArgs e)
        {
            _results = new SelfTestRunner().RunAll();
            _grid.Rows.Clear();
            int passed = 0;
            foreach (var r in _results)
            {
                int i = _grid.Rows.Add(r.Category, r.Id, r.Description, r.Input, r.Expected, r.Actual, r.Passed ? "пройден" : "НЕ пройден",
                    r.Milliseconds.ToString("0.###", CultureInfo.CurrentCulture));
                _grid.Rows[i].DefaultCellStyle.BackColor = r.Passed ? Color.Honeydew : Color.MistyRose;
                if (r.Passed) passed++;
            }
            _summary.Text = "Пройдено " + passed + " из " + _results.Count + ". Качество: " + TestReportBuilder.Quality(_results);
        }

        private void Save(object sender, EventArgs e)
        {
            if (_results.Count == 0)
                Run(sender, e);
            using (var dialog = new SaveFileDialog { Filter = "HTML (*.html)|*.html|Текст (*.txt)|*.txt", FileName = "Отчёт_о_тестировании.html" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                var builder = new TestReportBuilder { Author = Environment.UserName, ProjectLink = Application.ExecutablePath };
                try
                {
                    string content = dialog.FilterIndex == 1 ? builder.BuildHtml(_results, DateTime.Now) : builder.BuildText(_results, DateTime.Now);
                    File.WriteAllText(dialog.FileName, content, Encoding.UTF8);
                    MessageBox.Show("Отчёт сохранён: " + dialog.FileName, "ЛР 16");
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Не удалось сохранить отчёт: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show("Нет доступа к файлу: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
