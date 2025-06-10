
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class Filter : Form
    {
        public event ApplyFilter ApplyFilter;
        public event CloseFilter CloseFilter;
        public Filter()
        {
            InitializeComponent();
        }

        private void Filter_Click(object sender, System.EventArgs e)
        {
            foreach (Button button in Controls)
            {
                if (button != ClearBtn)
                    button.BackColor = Color.FromArgb(114, 137, 218);
            }

            Button b = (Button)sender;
            b.BackColor = Color.Orange;
            ApplyFilter?.Invoke(b.Tag.ToString());
        }

        private void Filter_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseFilter?.Invoke();
        }

        private void ChangeLanguage(string lang)
        {
            foreach (Control c in Controls)
            {
                ComponentResourceManager resources = new(typeof(Filter));
                resources.ApplyResources(c, c.Name, new CultureInfo(lang));
            }
        }

        private void ChangeLanguageBtn_Click(object sender, System.EventArgs e)
        {
            ChangeLanguage("zh");
        }
    }
}
