using System.Drawing;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public class MyProgressBar : ProgressBar
    {
        public MyProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rec = e.ClipRectangle;

            rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
            if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rec.Height -= 4;
            e.Graphics.FillRectangle(Brushes.Red, 2, 2, rec.Width, rec.Height);
        }
    }
}
