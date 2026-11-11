using System;
using System.Windows.Forms;
using LabWorks.Core.People;

namespace LabWorks.App.Tabs
{
    /// <summary>
    /// ЛР 11. Форма из методички: метка, два текстовых поля, выбор даты, кнопка «Отобразить запись».
    /// Объект Employee создаётся из собственного класса Person.
    /// </summary>
    public class Lab11PersonControl : UserControl
    {
        private readonly TextBox _firstName = new TextBox { Width = 220, Text = "Имя" };
        private readonly TextBox _lastName = new TextBox { Width = 220, Text = "Фамилия" };
        private readonly DateTimePicker _birthday = new DateTimePicker { Width = 220, Format = DateTimePickerFormat.Short };
        private readonly TextBox _log = UiHelpers.MakeLog();

        public Lab11PersonControl()
        {
            var panel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 190, FlowDirection = FlowDirection.TopDown };
            panel.Controls.Add(UiHelpers.MakeLabel("Введите имя, фамилию и дату рождения сотрудника.", true));
            panel.Controls.Add(_firstName);
            panel.Controls.Add(_lastName);
            panel.Controls.Add(_birthday);
            panel.Controls.Add(UiHelpers.MakeButton("Отобразить запись", ShowRecord));
            _birthday.Value = new DateTime(2005, 3, 15);
            Controls.Add(_log);
            Controls.Add(panel);
        }

        private void ShowRecord(object sender, EventArgs e)
        {
            try
            {
                var employee = new Person
                {
                    FirstName = _firstName.Text,
                    LastName = _lastName.Text,
                    BirthDate = _birthday.Value
                };
                DateTime today = DateTime.Today;
                string text = employee.FullName + Environment.NewLine
                              + "Возраст: " + employee.Age(today) + Environment.NewLine
                              + "До 100-летия: " + employee.YearsTo100(today) + " лет" + Environment.NewLine
                              + "(формула из методички, дни / 365.25: " + Person.AgeAsInManual(employee.BirthDate, today) + ")";
                UiHelpers.Append(_log, text.Replace(Environment.NewLine, "; "));
                MessageBox.Show(text, "Запись сотрудника");
            }
            catch (ArgumentException ex)
            {
                // ArgumentOutOfRangeException — наследник ArgumentException, поэтому перехватывается здесь же.
                MessageBox.Show(ex.Message, "Неверные данные", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
