using System.Windows.Forms;

namespace ACNHPokerCore
{
    public class MyComboBox : ComboBox
    {
        protected override bool IsInputKey(Keys keyData)
        {
            switch ((keyData & (Keys.Alt | Keys.KeyCode)))
            {
                case Keys.Enter:
                case Keys.Escape:
                    if (DroppedDown)
                    {
                        DroppedDown = false;
                        return false;
                    }
                    break;
            }
            return base.IsInputKey(keyData);
        }
    }
}
