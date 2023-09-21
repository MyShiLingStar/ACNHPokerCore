using System.IO;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class MyCheck : Form
    {
        readonly string hwid;
        readonly string[] list;

        public MyCheck(string HWID)
        {
            hwid = HWID;
            InitializeComponent();
            list = Text.Split(" ");
            hwidDisplay.Text = hwid;
            KeyPreview = true;
        }

        private void MyWarning_KeyDown(object sender, KeyEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                if (e.KeyCode.ToString() == "V")
                {
                    MyMessageBox.Show("You are being lazy, aren't you?", "Epic's Easy Anti-Cheat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void SubmitButton_Click(object sender, System.EventArgs e)
        {
            if (answerBox1.Equals(string.Empty) ||
                answerBox2.Equals(string.Empty) ||
                answerBox3.Equals(string.Empty) ||
                answerBox4.Equals(string.Empty) ||
                answerBox5.Equals(string.Empty) ||
                answerBox6.Equals(string.Empty))
            {
                MyMessageBox.Show("Invalid Game Key!", "Arabia Anti-Cheat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (KeyCheck())
            {
                byte[] buffer = Utilities.StringToByte(hwid);
                File.WriteAllBytes(Utilities.saveFolder + Utilities.fileName.Replace("F", ""), buffer);
                MyMessageBox.Show("Thank you for your support!", "Arabia Anti-Cheat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
            else
            {
                MyMessageBox.Show("Invalid Game Key!", "Arabia Anti-Cheat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool KeyCheck()
        {
            string Answer1 = answerBox1.Text;
            string aNswer2 = answerBox2.Text;
            string anSwer3 = answerBox3.Text;
            string ansWer4 = answerBox4.Text;
            string answEr5 = answerBox5.Text;
            string answeR6 = answerBox6.Text;

            if (answEr5 == list[4] &&
                Answer1.Equals(list[0]) &&
                anSwer3.Equals(list[2]) &&
                aNswer2 == list[1] &&
                ansWer4 == list[3] &&
                answEr5 == list[4]) { return true; }

            return false;
        }
    }
}
