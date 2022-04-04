using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class MyWarning : Form
    {
        Socket socket;
        bool sound;
        miniMap map;
        public MyWarning(Socket S, Boolean Sound, miniMap Map)
        {
            socket = S;
            sound = Sound;
            map = Map;
            InitializeComponent();
            this.KeyPreview = true;
        }

        private void answerBox_TextChanged(object sender, EventArgs e)
        {
            if (answerBox.Text.ToString().ToLower().Equals(sampleBox.Text.ToString().ToLower()))
                confirmBtn.Visible = true;
            else
                confirmBtn.Visible = false;
        }

        private void confirmBtn_Click(object sender, EventArgs e)
        {
            confirmBtn.Visible = false;
            answerBox.Enabled = false;
            PleaseWaitPanel.Visible = true;

            Thread FlattenThread = new Thread(delegate () { FlattenTerrain(); });
            FlattenThread.Start();
        }

        private void FlattenTerrain()
        {
            SaveFileDialog file = new SaveFileDialog();

            byte[] CurrentTerrainData = Utilities.getTerrain(socket, null);

            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("en-US");
            file.FileName = Directory.GetCurrentDirectory() + @"\save\" + localDate.ToString(culture).Replace(" ", "_").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("|", "-").Replace(".", "-") + ".nht";
            File.WriteAllBytes(file.FileName, CurrentTerrainData);

            int counter = 0;

            while (isAboutToSave(10))
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

            map.updateTerrain(EmptyTerrainData);

            Utilities.sendTerrain(socket, null, EmptyTerrainData, ref counter);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }

        private void MyWarning_KeyDown(object sender, KeyEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
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

        private bool isAboutToSave(int second)
        {
            byte[] b = Utilities.getSaving(socket, null);

            if (b == null)
                return true;
            if (b[0] != 0)
                return true;
            else
            {
                byte[] currentFrame = new byte[4];
                byte[] lastFrame = new byte[4];
                Buffer.BlockCopy(b, 12, currentFrame, 0, 4);
                Buffer.BlockCopy(b, 16, lastFrame, 0, 4);

                int currentFrameStr = Convert.ToInt32("0x" + Utilities.flip(Utilities.ByteToHexString(currentFrame)), 16);
                int lastFrameStr = Convert.ToInt32("0x" + Utilities.flip(Utilities.ByteToHexString(lastFrame)), 16);

                if (((0x1518 - (currentFrameStr - lastFrameStr))) < 30 * second)
                    return true;
                else if (((0x1518 - (currentFrameStr - lastFrameStr))) >= 30 * 175)
                    return true;
                else
                {
                    Debug.Print(((0x1518 - (currentFrameStr - lastFrameStr))).ToString());
                    return false;
                }
            }
        }
    }
}
