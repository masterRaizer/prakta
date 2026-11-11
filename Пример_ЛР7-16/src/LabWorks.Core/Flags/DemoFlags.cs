namespace LabWorks.Core.Flags
{
    /// <summary>Флаг из полос (ЛР 7). Цвета — в формате 0xRRGGBB.</summary>
    public sealed class FlagInfo
    {
        public FlagInfo(string caption, bool vertical, params int[] stripes)
        {
            Caption = caption;
            Vertical = vertical;
            Stripes = stripes;
        }

        /// <summary>Подпись, которая выводится в Label (например, «Флаг Сьерра-Леоне»).</summary>
        public string Caption { get; private set; }
        public bool Vertical { get; private set; }
        public int[] Stripes { get; private set; }
    }

    /// <summary>
    /// Флаги примера. В методичке используются файлы рисунков (LoadPicture); здесь флаги рисуются программно,
    /// поэтому проект не зависит от путей к файлам. Работа с файлом показана отдельной кнопкой («Ошибка в модуле»).
    /// </summary>
    public static class DemoFlags
    {
        public static readonly FlagInfo First = new FlagInfo("Флаг Сьерра-Леоне", false, 0x1EB53A, 0xFFFFFF, 0x0072C6);
        public static readonly FlagInfo Second = new FlagInfo("Флаг Кот-д’Ивуара", true, 0xF77F00, 0xFFFFFF, 0x009E60);
        public const int PictureBackColor = 0xFF80FF;   // как &HFF80FF в методичке
        public const int FormBackColor = 0x400000;      // RGB(64, 0, 0)
    }
}
