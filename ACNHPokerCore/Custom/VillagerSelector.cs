using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    class VillagerSelector : ComboBox
    {
        public VillagerSelector()
        {
            //DrawMode = DrawMode.OwnerDrawFixed;
            //DropDownStyle = ComboBoxStyle.DropDownList;
            //this.BackColor = Color.Red;
        }

        // Draws the items into the ColorSelector object
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                e.DrawFocusRectangle();

                string[] lines = Items[e.Index].ToString()?.Split(new[] { " " }, StringSplitOptions.None);
                Image img;
                if (lines != null && File.Exists(Utilities.GetVillagerImage(lines[lines.Length - 1])))
                    img = ImageCacher.GetImage(Utilities.GetVillagerImage(lines[lines.Length - 1]));
                else
                    img = new Bitmap(Properties.Resources.Leaf, new Size(60, 60));
                // Draw the colored 16 x 16 square
                Bitmap resize = new(img, new Size(60, 60));
                e.Graphics.DrawImage(resize, e.Bounds.Left, e.Bounds.Top);
                // Draw the value (in this case, the color name)
                //e.Graphics.DrawString(this.Items[e.Index].ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
                e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + resize.Width, e.Bounds.Top + 22);
            }
            base.OnDrawItem(e);
        }

        /*
        private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            //Allows the keyboard to bring the items into view as expected:
            if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Up))
                return;

            // Allows to bring the selected item into view:
            if (((System.Windows.Controls.ComboBoxItem)e.TargetObject).Content == this.SelectedItem)
                return;

            e.Handled = true;
        }
        */
    }
}
