using System.Drawing;
using System.Windows.Forms;
using LabWorks.App.Tabs;
using LabWorks.Core.Formulas;

namespace LabWorks.App
{
    /// <summary>Главное окно: по вкладке на каждую лабораторную работу.</summary>
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Лабораторные работы 7–16 — пример (демонстрационный вариант: " + DemoVariant.Formula.Text + ")";
            ClientSize = new Size(980, 640);
            MinimumSize = new Size(820, 560);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);

            var tabs = new TabControl { Dock = DockStyle.Fill };
            AddTab(tabs, "ЛР 7. Отладка модулей", new Lab07FlagsControl());
            AddTab(tabs, "ЛР 8. Исключения", new Lab08CalculatorControl());
            AddTab(tabs, "ЛР 10, 15. Программа варианта", new Lab10ProgramControl());
            AddTab(tabs, "ЛР 11. Класс Person", new Lab11PersonControl());
            AddTab(tabs, "ЛР 13. Оптимизация", new Lab13OptimizationControl());
            AddTab(tabs, "ЛР 14–16. Тестирование", new Lab16TestingControl());
            Controls.Add(tabs);
        }

        private static void AddTab(TabControl tabs, string title, Control content)
        {
            var page = new TabPage(title) { Padding = new Padding(8) };
            content.Dock = DockStyle.Fill;
            page.Controls.Add(content);
            tabs.TabPages.Add(page);
        }
    }
}
