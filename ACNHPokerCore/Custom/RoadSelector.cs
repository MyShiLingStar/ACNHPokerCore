using System.Drawing;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    class RoadSelector : ComboBox
    {
        public RoadSelector()
        {

        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                e.DrawFocusRectangle();

                Image img;
                switch (e.Index)
                {
                    case 0:
                        img = new Bitmap(Properties.Resources.wood, new Size(24, 24));
                        break;
                    case 1:
                        img = new Bitmap(Properties.Resources.tile, new Size(24, 24));
                        break;
                    case 2:
                        img = new Bitmap(Properties.Resources.sand, new Size(24, 24));
                        break;
                    case 3:
                        img = new Bitmap(Properties.Resources.pattern, new Size(24, 24));
                        break;
                    case 4:
                        img = new Bitmap(Properties.Resources.darksoil, new Size(24, 24));
                        break;
                    case 5:
                        img = new Bitmap(Properties.Resources.brick, new Size(24, 24));
                        break;
                    case 6:
                        img = new Bitmap(Properties.Resources.stone, new Size(24, 24));
                        break;
                    case 7:
                        img = new Bitmap(Properties.Resources.dirt, new Size(24, 24));
                        break;
                    default:
                        img = new Bitmap(Properties.Resources.Leaf, new Size(24, 24));
                        break;
                }

                e.Graphics.DrawImage(img, e.Bounds.Left, e.Bounds.Top + 3);

                e.Graphics.DrawString(this.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + img.Width, e.Bounds.Top + 6);
            }
            base.OnDrawItem(e);
        }
    }
}
