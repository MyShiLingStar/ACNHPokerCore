using System.Drawing;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    class RoadSelector : ComboBox
    {
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                Image img = e.Index switch
                {
                    0 => new Bitmap(Properties.Resources.wood, new Size(24, 24)),
                    1 => new Bitmap(Properties.Resources.tile, new Size(24, 24)),
                    2 => new Bitmap(Properties.Resources.sand, new Size(24, 24)),
                    3 => new Bitmap(Properties.Resources.pattern, new Size(24, 24)),
                    4 => new Bitmap(Properties.Resources.darksoil, new Size(24, 24)),
                    5 => new Bitmap(Properties.Resources.brick, new Size(24, 24)),
                    6 => new Bitmap(Properties.Resources.stone, new Size(24, 24)),
                    7 => new Bitmap(Properties.Resources.dirt, new Size(24, 24)),
                    _ => new Bitmap(Properties.Resources.Leaf, new Size(24, 24)),
                };
                e.Graphics.DrawImage(img, e.Bounds.Left, e.Bounds.Top + 3);

                e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + img.Width, e.Bounds.Top + 6);
            }
            base.OnDrawItem(e);
        }
    }
}
