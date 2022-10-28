
using System.Drawing;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class Filter : Form
    {
        public event ApplyFilter applyFilter;

        public Filter()
        {
            InitializeComponent();
        }

        private void filter_Click(object sender, System.EventArgs e)
        {
            foreach (Button button in this.Controls)
            {
                if (button != ClearBtn)
                    button.BackColor = Color.FromArgb(114, 137, 218);
            }

            Button b = (Button)sender;
            b.BackColor = Color.Orange;
            applyFilter(b.Tag.ToString());
        }
    }
}
