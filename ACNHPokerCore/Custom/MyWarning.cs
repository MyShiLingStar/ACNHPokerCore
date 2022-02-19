using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public MyWarning(Socket S, Boolean Sound)
        {
            socket = S;
            sound = Sound;
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
            SaveFileDialog file = new SaveFileDialog();
            DateTime localDate = DateTime.Now;
            file.FileName = Directory.GetCurrentDirectory() + @"\save\" + localDate.ToString().Replace(" ","").Replace(":","-") + ".nht";

            confirmBtn.Visible = false;
            answerBox.Enabled = false;
            PleaseWaitPanel.Visible = true;

            Thread FlattenThread = new Thread(delegate () { FlattenTerrain(file); });
            FlattenThread.Start();
        }

        private void FlattenTerrain(SaveFileDialog file)
        {
            byte[] CurrentTerrainData = Utilities.getTerrain(socket, null);
            File.WriteAllBytes(file.FileName, CurrentTerrainData);

            while (isAboutToSave(10))
            {
                Thread.Sleep(2000);
            }

            byte[] EmptyTerrainData = new byte[Utilities.AllTerrainSize];
            int counter = 0;
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
