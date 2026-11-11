using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace LabWorks.App
{
    /// <summary>
    /// Простой график y(x), нарисованный средствами GDI+ в событии Paint.
    /// Точки, где функция не определена, не передаются — в этих местах линия разрывается.
    /// </summary>
    public class PlotPanel : Panel
    {
        private List<PointF> _points = new List<PointF>();
        private string _title = "";

        public PlotPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
            ResizeRedraw = true;
        }

        public void SetData(List<PointF> points, string title)
        {
            _points = points ?? new List<PointF>();
            _title = title;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawString(_title, Font, Brushes.Black, 6, 4);
            if (_points.Count < 2)
                return;

            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            foreach (var p in _points)
            {
                minX = Math.Min(minX, p.X); maxX = Math.Max(maxX, p.X);
                minY = Math.Min(minY, p.Y); maxY = Math.Max(maxY, p.Y);
            }
            if (maxY - minY < 1e-6f) { maxY += 1; minY -= 1; }
            var area = new RectangleF(50, 24, Width - 70, Height - 50);
            Func<PointF, PointF> map = p => new PointF(
                area.Left + (p.X - minX) / (maxX - minX) * area.Width,
                area.Bottom - (p.Y - minY) / (maxY - minY) * area.Height);

            g.DrawRectangle(Pens.Silver, area.X, area.Y, area.Width, area.Height);
            if (minY < 0 && maxY > 0)
            {
                float y0 = map(new PointF(minX, 0)).Y;
                g.DrawLine(Pens.Gray, area.Left, y0, area.Right, y0);
            }
            g.DrawString(maxY.ToString("G4", CultureInfo.CurrentCulture), Font, Brushes.DimGray, 2, area.Top - 6);
            g.DrawString(minY.ToString("G4", CultureInfo.CurrentCulture), Font, Brushes.DimGray, 2, area.Bottom - 10);
            g.DrawString(minX.ToString("G4", CultureInfo.CurrentCulture), Font, Brushes.DimGray, area.Left - 8, area.Bottom + 4);
            g.DrawString(maxX.ToString("G4", CultureInfo.CurrentCulture), Font, Brushes.DimGray, area.Right - 20, area.Bottom + 4);

            using (var pen = new Pen(Color.SteelBlue, 2))
            {
                for (int i = 1; i < _points.Count; i++)
                {
                    // Разрыв линии, если между точками пропущены значения (функция не определена).
                    if (_points[i].X - _points[i - 1].X > 1.5f)
                        continue;
                    g.DrawLine(pen, map(_points[i - 1]), map(_points[i]));
                }
                foreach (var p in _points)
                {
                    PointF q = map(p);
                    g.FillEllipse(Brushes.SteelBlue, q.X - 2.5f, q.Y - 2.5f, 5, 5);
                }
            }
        }
    }
}
