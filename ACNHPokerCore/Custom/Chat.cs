using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class Chat : Form
    {
        private readonly Socket socket;

        private static readonly string chat = "[main+5254A40]+40";//"[main+4AA9CD8]+40";

        private static readonly int GameCap = 24;
        private static readonly int SoftCap = 32;
        private static readonly int MidCap = 57;
        private static readonly int HardCap = 64;

        private static bool sendLock;

        public event CloseHandler CloseForm;
        public Chat(Socket Socket)
        {
            socket = Socket;

            InitializeComponent();
            Random rad = new();
            int num = rad.Next(1, 20);
            switch (num)
            {
                case 1:
                    chatBox.Text = @"Hello !";
                    break;
                case 2:
                    chatBox.Text = @"こんにちは！";
                    break;
                case 3:
                    chatBox.Text = @"你好！";
                    break;
                case 4:
                    chatBox.Text = @"Bonjour !";
                    break;
                case 5:
                    chatBox.Text = @"Hallo !";
                    break;
                case 6:
                    chatBox.Text = @"Hola !";
                    break;
                case 7:
                    chatBox.Text = @"Ciao !";
                    break;
                case 8:
                    chatBox.Text = @"Olá !";
                    break;
                case 9:
                    chatBox.Text = @"Привет !";
                    break;
                case 10:
                    chatBox.Text = @"안녕하세요！";
                    break;
            }
            chatBox.SelectAll();
        }

        private void Chat_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseForm?.Invoke();
        }

        private void ChatButton_Click(object sender, EventArgs e)
        {
            string cleanStr = chatBox.Text.Trim().Replace("\n", " ");

            if (sendLock)
                return;
            if (cleanStr.Equals(""))
                return;
            else if (cleanStr.Length > HardCap)
                return;

            if (!RetainChat.Checked)
                chatBox.Clear();
            sendLock = true;

            Thread sendThread = new(delegate () { SendChat(cleanStr); });
            sendThread.Start();
        }

        private void SendChat(string message)
        {
            ulong ChatAddress = Teleport.GetCoordinateAddress(chat);

            Controller.ClickR();
            Thread.Sleep(800);
            Controller.ClickY();

            byte[] StrBytes = Encoding.Unicode.GetBytes(message);
            byte[] sendBytes = new byte[StrBytes.Length * 2];
            Buffer.BlockCopy(StrBytes, 0, sendBytes, 0, StrBytes.Length);
            Utilities.PokeAbsoluteAddress(socket, ChatAddress.ToString("X"), Utilities.ByteToHexString(sendBytes));

            Controller.ClickPLUS();
            Thread.Sleep(400);

            Controller.ClickB();
            Thread.Sleep(200);
            Controller.ClickB();
            Thread.Sleep(200);
            Controller.ClickB();
            Thread.Sleep(200);

            sendLock = false;
        }

        private void ChatBox_TextChanged(object sender, EventArgs e)
        {
            string cleanStr = chatBox.Text.Trim().Replace("\n", " ");

            if (cleanStr.Length <= GameCap)
                chatBox.ForeColor = Color.White;
            else if (cleanStr.Length <= SoftCap)
                chatBox.ForeColor = Color.Pink;
            else if (cleanStr.Length <= MidCap)
                chatBox.ForeColor = Color.Yellow;
            else if (cleanStr.Length < HardCap)
                chatBox.ForeColor = Color.Orange;
            else
                chatBox.ForeColor = Color.Red;
        }

        private void ChatBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.V) && e.Control && !e.Alt && !e.Shift)
            {
                if (Clipboard.ContainsText(TextDataFormat.Html) ||
                Clipboard.ContainsText(TextDataFormat.Rtf))
                {
                    string plainText = Clipboard.GetText();
                    Clipboard.Clear();
                    Clipboard.SetText(plainText);
                }
            }

            if ((e.KeyCode == Keys.Enter) && !e.Control && !e.Shift)
            {
                ChatButton_Click(null, null);
                e.Handled = true;
            }
        }

        private void SafetyCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (SafetyCheck.Checked)
            {
                chatBox.MaxLength = 24;
                SafetyCheck.ForeColor = Color.White;
            }
            else
            {
                chatBox.MaxLength = 64;
                SafetyCheck.ForeColor = Color.Red;
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            Controller.DetachController();
            Controller.ClickA();
            Controller.ClickZR();
            Thread.Sleep(1000);
            Controller.ClickB();
            Controller.ClickB();
            Controller.ClickB();
            chatButton.Enabled = true;
            ConnectButton.BackColor = Color.Orange;
        }
    }
}
