using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LabWorks.Core.Diagnostics;
using LabWorks.Core.Flags;

namespace LabWorks.App.Tabs
{
    /// <summary>
    /// ЛР 7. Аналог формы из методички (там — VBA UserForm с Image и кнопками) на Windows Forms:
    /// PictureBox вместо Image, Label для надписи и пять кнопок. Кнопка «Ошибка в модуле» показывает,
    /// как отладчик и обработка ошибок помогают найти ошибку выполнения (неверный путь к файлу рисунка).
    /// </summary>
    public class Lab07FlagsControl : UserControl
    {
        private readonly PictureBox _picture;
        private readonly Label _caption;
        private readonly Panel _stage;
        private readonly TextBox _log;

        public Lab07FlagsControl()
        {
            _caption = new Label { Text = "Надпись", AutoSize = true, Font = new Font("Segoe UI", 11F), Location = new Point(10, 10), ForeColor = Color.Black };
            _picture = new PictureBox { Size = new Size(180, 120), Location = new Point(10, 45), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };
            _stage = new Panel { Height = 190, Dock = DockStyle.Top, BorderStyle = BorderStyle.FixedSingle };
            _stage.Controls.Add(_caption);
            _stage.Controls.Add(_picture);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(0, 6, 0, 0) };
            buttons.Controls.Add(UiHelpers.MakeButton("Измени надпись", ChangeCaption));
            buttons.Controls.Add(UiHelpers.MakeButton("Сдвинь рисунок вправо", MoveRight));
            buttons.Controls.Add(UiHelpers.MakeButton("Измени цвет фона и формы", ChangeColors));
            buttons.Controls.Add(UiHelpers.MakeButton("Мозаика", Tile));
            buttons.Controls.Add(UiHelpers.MakeButton("Смена флага и надписи", ChangeFlag));
            buttons.Controls.Add(UiHelpers.MakeButton("Ошибка в модуле (файл)", LoadMissingFile));
            buttons.Controls.Add(UiHelpers.MakeButton("Сброс", (s, e) => Reset()));

            _log = UiHelpers.MakeLog();
            Controls.Add(_log);
            Controls.Add(UiHelpers.MakeLabel("Журнал отладки:", true));
            Controls[Controls.Count - 1].Dock = DockStyle.Top;
            Controls.Add(buttons);
            Controls.Add(_stage);
            Reset();
        }

        private void Reset()
        {
            _picture.Image = null;
            _picture.BackgroundImage = null;
            _picture.BackColor = SystemColors.Control;
            _picture.Left = 10;
            _caption.Text = "Надпись";
            _caption.Font = new Font("Segoe UI", 11F);
            _caption.ForeColor = Color.Black;
            _stage.BackColor = SystemColors.Control;
        }

        // CommandButton1 из методички: Label1.Caption = ...; Label1.Font.Size = 14 (там ошибка — Sie вместо Size).
        private void ChangeCaption(object sender, EventArgs e)
        {
            _caption.Text = DemoFlags.First.Caption;
            // В C# опечатка вида _caption.Font.Sie не скомпилируется (ошибка CS1061) — это ошибка компиляции.
            _caption.Font = new Font(_caption.Font.FontFamily, 14F);
            _picture.BackgroundImage = null;
            _picture.Image = FlagPainter.Draw(DemoFlags.First, 180, 120);
            Log("Измени надпись: " + DemoFlags.First.Caption);
        }

        // CommandButton2: Image1.PictureAlignment = 4 (рисунок вправо).
        private void MoveRight(object sender, EventArgs e)
        {
            _picture.Left = _stage.ClientSize.Width - _picture.Width - 10;
            Log("Сдвинь рисунок вправо: Left = " + _picture.Left);
        }

        // CommandButton4: Image1.BackColor = &HFF80FF; UserForm2.BackColor = RGB(64, 0, 0).
        private void ChangeColors(object sender, EventArgs e)
        {
            _picture.BackColor = Color.FromArgb(255, Color.FromArgb(DemoFlags.PictureBackColor));
            _stage.BackColor = Color.FromArgb(255, Color.FromArgb(DemoFlags.FormBackColor));
            _caption.ForeColor = Color.White;
            Log("Цвет фона рисунка и формы изменён");
        }

        // CommandButton3: Image1.PictureTiling = True.
        private void Tile(object sender, EventArgs e)
        {
            Image current = _picture.Image ?? FlagPainter.Draw(DemoFlags.First, 180, 120);
            _picture.BackgroundImage = new Bitmap(current, new Size(45, 30));
            _picture.BackgroundImageLayout = ImageLayout.Tile;
            _picture.Image = null;
            Log("Мозаика: рисунок повторяется по всей площади");
        }

        // CommandButton5: другая надпись, шрифт Arial Black, другой рисунок.
        private void ChangeFlag(object sender, EventArgs e)
        {
            _caption.Text = DemoFlags.Second.Caption;
            _caption.Font = new Font("Arial Black", 14F);
            _picture.BackgroundImage = null;
            _picture.Image = FlagPainter.Draw(DemoFlags.Second, 180, 120);
            Log("Смена флага и надписи: " + DemoFlags.Second.Caption);
        }

        // Аналог LoadPicture("C:\FlgRUS.gif") с неверным путём: ошибка выполнения.
        // Для отладки: поставьте точку останова (F9) на строке Image.FromFile и посмотрите переменную path в окне Locals.
        private void LoadMissingFile(object sender, EventArgs e)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FlgNoSuchFile.gif");
            try
            {
                _picture.Image = Image.FromFile(path);
            }
            catch (FileNotFoundException ex)
            {
                Log("Ошибка выполнения: " + ex.GetType().Name + " — " + ex.Message);
                MessageBox.Show("Файл рисунка не найден:\n" + path + "\n\nПравильно описывайте путь к графическим файлам.",
                    "Ошибка выполнения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Log(string text)
        {
            UiHelpers.Append(_log, DateTime.Now.ToString("HH:mm:ss") + "  " + text);
            DebugLog.Write("ЛР 7: " + text);
        }
    }
}
