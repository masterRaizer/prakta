using System;

namespace LabWorks.Core.People
{
    /// <summary>
    /// Класс Person (ЛР 11): закрытые поля, свойства с проверкой значений и методы.
    /// В методичке пример на Visual Basic; здесь то же на C#, с исправлениями, найденными при инспекции кода.
    /// </summary>
    public class Person
    {
        // Закрытые поля класса (в методичке — Name1, Name2).
        private string _firstName;
        private string _lastName;
        private DateTime _birthDate;

        /// <summary>Имя. Замечание инспекции №1: в методичке нет проверки на пустую строку.</summary>
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Имя не может быть пустым", "FirstName");
                _firstName = value.Trim();
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Фамилия не может быть пустой", "LastName");
                _lastName = value.Trim();
            }
        }

        public DateTime BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value.Date; }
        }

        public string FullName { get { return LastName + " " + FirstName; } }

        /// <summary>Полное число лет на дату today.</summary>
        public int Age(DateTime today)
        {
            return AgeOn(BirthDate, today);
        }

        /// <summary>
        /// Замечание инспекции №2: формула методички Int(дни / 365.25) в некоторые дни ошибается на год
        /// (например, 11.11.1952 → 11.11.2026 даёт 73 вместо 74). Правильно — сравнивать даты.
        /// Замечание инспекции №3: дата рождения в будущем должна отклоняться.
        /// </summary>
        public static int AgeOn(DateTime birthday, DateTime today)
        {
            birthday = birthday.Date;
            today = today.Date;
            if (birthday > today)
                throw new ArgumentOutOfRangeException("birthday", "Дата рождения позже текущей даты");
            int years = today.Year - birthday.Year;
            if (birthday.AddYears(years) > today)
                years--;
            return years;
        }

        /// <summary>Метод Age в том виде, как в методичке (оставлен для сравнения при инспекции).</summary>
        public static int AgeAsInManual(DateTime birthday, DateTime today)
        {
            return (int)((today.Date - birthday.Date).Days / 365.25);
        }

        /// <summary>Дополнительный метод примера: сколько полных лет осталось до 100-летия.</summary>
        public int YearsTo100(DateTime today)
        {
            return Math.Max(0, 100 - Age(today));
        }
    }
}
