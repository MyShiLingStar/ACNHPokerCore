using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class MyWarning : Form
    {
        readonly Socket socket;
        readonly USBBot usb;
        readonly bool sound;
        readonly MiniMap map;
        public MyWarning(Socket S, USBBot USB, bool Sound, MiniMap Map)
        {
            socket = S;
            usb = USB;
            sound = Sound;
            map = Map;
            InitializeComponent();
            KeyPreview = true;
        }

        private void AnswerBox_TextChanged(object sender, EventArgs e)
        {
            if (answerBox.Text.ToLower().Equals(sampleBox.Text.ToLower()))
                confirmBtn.Visible = true;
            else
                confirmBtn.Visible = false;
        }

        private void ConfirmBtn_Click(object sender, EventArgs e)
        {
            confirmBtn.Visible = false;
            answerBox.Enabled = false;
            PleaseWaitPanel.Visible = true;

            Thread FlattenThread = new(FlattenTerrain);
            FlattenThread.Start();
        }

        private void FlattenTerrain()
        {
            SaveFileDialog file = new();

            byte[] CurrentTerrainData = Utilities.GetTerrain(socket, usb);

            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("en-US");
            file.FileName = Directory.GetCurrentDirectory() + @"\save\" + localDate.ToString(culture).Replace(" ", "_").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("|", "-").Replace(".", "-") + ".nht";
            File.WriteAllBytes(file.FileName, CurrentTerrainData);

            int counter = 0;

            while (Utilities.IsAboutToSave(socket, usb, 10))
            {
                if (counter > 15)
                    break;
                Thread.Sleep(2000);
                counter++;
            }

            counter = 0;

            byte[] EmptyTerrainData = new byte[Utilities.AllTerrainSize];

            for (int i = 0; i < Utilities.MapTileCount16x16; i++)
            {
                Buffer.BlockCopy(CurrentTerrainData, i * Utilities.TerrainTileSize + 6, EmptyTerrainData, i * Utilities.TerrainTileSize + 6, 6);
            }

            map.UpdateTerrain(EmptyTerrainData);

            Utilities.SendTerrain(socket, usb, EmptyTerrainData, ref counter);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            Invoke((MethodInvoker)delegate
            {
                Close();
            });
        }

        private void MyWarning_KeyDown(object sender, KeyEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                if (e.KeyCode.ToString() == "C" || e.KeyCode.ToString() == "V")
                {
                    MyMessageBox.Show("You are being lazy, aren't you?", "Epic's Easy Anti-Cheat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (e.KeyCode.ToString() == "F1")
                {
                    answerBox.Text = sampleBox.Text;
                }
            }
        }
    }
}
