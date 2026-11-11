using System.Drawing;
using LabWorks.Core.Flags;

namespace LabWorks.App
{
    /// <summary>Рисование флага из полос средствами GDI+ (класс Graphics).</summary>
    internal static class FlagPainter
    {
        public static Bitmap Draw(FlagInfo flag, int width, int height)
        {
            var bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                int n = flag.Stripes.Length;
                for (int i = 0; i < n; i++)
                {
                    Color color = Color.FromArgb(255, Color.FromArgb(flag.Stripes[i]));
                    Rectangle r = flag.Vertical
                        ? new Rectangle(i * width / n, 0, (i + 1) * width / n - i * width / n, height)
                        : new Rectangle(0, i * height / n, width, (i + 1) * height / n - i * height / n);
                    using (var brush = new SolidBrush(color))
                        g.FillRectangle(brush, r);
                }
                g.DrawRectangle(Pens.Gray, 0, 0, width - 1, height - 1);
            }
            return bmp;
        }
    }
}
