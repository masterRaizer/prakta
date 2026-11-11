using System.Drawing;
using System.Windows.Forms;

namespace LabWorks.App.Tabs
{
    /// <summary>Мелкие помощники для построения интерфейса в коде.</summary>
    internal static class UiHelpers
    {
        public static Button MakeButton(string text, System.EventHandler handler, int width = 190)
        {
            var b = new Button { Text = text, Width = width, Height = 30, Margin = new Padding(3) };
            b.Click += handler;
            return b;
        }

        public static Label MakeLabel(string text, bool bold = false)
        {
            return new Label
            {
                Text = text, AutoSize = true, Margin = new Padding(3, 8, 3, 3),
                Font = bold ? new Font("Segoe UI", 9F, FontStyle.Bold) : null
            };
        }

        public static TextBox MakeLog()
        {
            return new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill, Font = new Font("Consolas", 9F) };
        }

        public static void Append(TextBox log, string line)
        {
            log.AppendText(line + System.Environment.NewLine);
        }
    }

}
