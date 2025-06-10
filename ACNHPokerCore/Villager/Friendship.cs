using System;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class Friendship : Form
    {
        readonly int[] friendshipValue = new int[8];
        readonly Socket S;
        readonly USBBot USB;
        readonly int Index;
        readonly bool sound;
        readonly Villager V;
        public Friendship(int i, Socket s, USBBot usb, Image img, Villager v, bool Sound)
        {
            InitializeComponent();
            S = s;
            USB = usb;
            Index = i;
            V = v;
            sound = Sound;
            VillagerImage.Image = img;

            Text = V.GetRealName();

            for (int p = 0; p < 8; p++)
            {
                //byte[] b = Utilities.GetVillager(s, bot, i, (int)(Utilities.VillagerMemoryTinySize), ref counter);
                byte[] b = Utilities.GetPlayerDataVillager(S, USB, i, p, (int)(Utilities.VillagerMemoryTinySize));
                if (b == null)
                    break;
                friendshipValue[p] = b[70];
                V.TempData[p] = b;
                V.Friendship[p] = b[70];

                switch (p)
                {
                    case 0:
                        Name1.Text = V.GetPlayerName(p);
                        FriendshipBar1.Value = friendshipValue[p];
                        FriendshipValue1.Text = friendshipValue[p].ToString();
                        break;
                    case 1:
                        Name2.Text = V.GetPlayerName(p);
                        FriendshipBar2.Value = friendshipValue[p];
                        FriendshipValue2.Text = friendshipValue[p].ToString();
                        break;
                    case 2:
                        Name3.Text = V.GetPlayerName(p);
                        FriendshipBar3.Value = friendshipValue[p];
                        FriendshipValue3.Text = friendshipValue[p].ToString();
                        break;
                    case 3:
                        Name4.Text = V.GetPlayerName(p);
                        FriendshipBar4.Value = friendshipValue[p];
                        FriendshipValue4.Text = friendshipValue[p].ToString();
                        break;
                    case 4:
                        Name5.Text = V.GetPlayerName(p);
                        FriendshipBar5.Value = friendshipValue[p];
                        FriendshipValue5.Text = friendshipValue[p].ToString();
                        break;
                    case 5:
                        Name6.Text = V.GetPlayerName(p);
                        FriendshipBar6.Value = friendshipValue[p];
                        FriendshipValue6.Text = friendshipValue[p].ToString();
                        break;
                    case 6:
                        Name7.Text = V.GetPlayerName(p);
                        FriendshipBar7.Value = friendshipValue[p];
                        FriendshipValue7.Text = friendshipValue[p].ToString();
                        break;
                    case 7:
                        Name8.Text = V.GetPlayerName(p);
                        FriendshipBar8.Value = friendshipValue[p];
                        FriendshipValue8.Text = friendshipValue[p].ToString();
                        break;
                }
            }
        }

        private void FriendshipBar1_ValueChanged(object sender, EventArgs e)
        {
            FriendshipValue1.Text = FriendshipBar1.Value.ToString();
        }

        private void FriendshipBar2_ValueChanged(object sender, EventArgs e)
        {
            FriendshipValue2.Text = FriendshipBar2.Value.ToString();
        }

        private void FriendshipBar3_ValueChanged(object sender, EventArgs e)
        {
            FriendshipValue3.Text = FriendshipBar3.Value.ToString();
        }

        private void FriendshipBar4_ValueChanged(object sender, EventArgs e)
        {
            FriendshipValue4.Text = FriendshipBar4.Value.ToString();
        }

        private void FriendshipBar5_ValueChanged(object sender, EventArgs e)
        {
            FriendshipValue5.Text = FriendshipBar5.Value.ToString();
        }

        private void FriendshipBar6_ValueChanged(object sender, EventArgs e)
        {
            FriendshipValue6.Text = FriendshipBar6.Value.ToString();
        }

        private void FriendshipBar7_ValueChanged(object sender, EventArgs e)
        {
            FriendshipValue7.Text = FriendshipBar7.Value.ToString();
        }

        private void FriendshipBar8_ValueChanged(object sender, EventArgs e)
        {
            FriendshipValue8.Text = FriendshipBar8.Value.ToString();
        }

        private void FriendshipValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void FriendshipValue1_Validated(object sender, EventArgs e)
        {
            RichTextBox box = (RichTextBox)sender;
            int value = Int16.Parse(box.Text);
            if (value > 255)
            {
                value = 255;
                box.Text = "255";
            }
            FriendshipBar1.Value = value;
        }

        private void FriendshipValue2_Validated(object sender, EventArgs e)
        {
            RichTextBox box = (RichTextBox)sender;
            int value = Int16.Parse(box.Text);
            if (value > 255)
            {
                value = 255;
                box.Text = "255";
            }
            FriendshipBar2.Value = value;
        }

        private void FriendshipValue3_Validated(object sender, EventArgs e)
        {
            RichTextBox box = (RichTextBox)sender;
            int value = Int16.Parse(box.Text);
            if (value > 255)
            {
                value = 255;
                box.Text = "255";
            }
            FriendshipBar3.Value = value;
        }

        private void FriendshipValue4_Validated(object sender, EventArgs e)
        {
            RichTextBox box = (RichTextBox)sender;
            int value = Int16.Parse(box.Text);
            if (value > 255)
            {
                value = 255;
                box.Text = "255";
            }
            FriendshipBar4.Value = value;
        }

        private void FriendshipValue5_Validated(object sender, EventArgs e)
        {
            RichTextBox box = (RichTextBox)sender;
            int value = Int16.Parse(box.Text);
            if (value > 255)
            {
                value = 255;
                box.Text = "255";
            }
            FriendshipBar5.Value = value;
        }

        private void FriendshipValue6_Validated(object sender, EventArgs e)
        {
            RichTextBox box = (RichTextBox)sender;
            int value = Int16.Parse(box.Text);
            if (value > 255)
            {
                value = 255;
                box.Text = "255";
            }
            FriendshipBar6.Value = value;
        }

        private void FriendshipValue7_Validated(object sender, EventArgs e)
        {
            RichTextBox box = (RichTextBox)sender;
            int value = Int16.Parse(box.Text);
            if (value > 255)
            {
                value = 255;
                box.Text = "255";
            }
            FriendshipBar7.Value = value;
        }

        private void FriendshipValue8_Validated(object sender, EventArgs e)
        {
            RichTextBox box = (RichTextBox)sender;
            int value = Int16.Parse(box.Text);
            if (value > 255)
            {
                value = 255;
                box.Text = "255";
            }
            FriendshipBar8.Value = value;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            int[] SetValue =
            [
                FriendshipBar1.Value,
                FriendshipBar2.Value,
                FriendshipBar3.Value,
                FriendshipBar4.Value,
                FriendshipBar5.Value,
                FriendshipBar6.Value,
                FriendshipBar7.Value,
                FriendshipBar8.Value,
            ];
            for (int p = 0; p < 8; p++)
            {
                if (SetValue[p] != friendshipValue[p])
                {
                    Utilities.SetFriendship(S, USB, Index, p, SetValue[p].ToString("X"));
                    V.Friendship[p] = (byte)SetValue[p];
                }
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            Close();
        }
    }
}
