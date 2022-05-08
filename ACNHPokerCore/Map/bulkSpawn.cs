using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class bulkSpawn : Form
    {
        private Socket s;
        private USBBot bot;
        private miniMap MiniMap = null;
        private map main;
        private int counter = 0;
        private int anchorX = -1;
        private int anchorY = -1;
        private bool ignore = false;
        private bool sound;
        private byte[] Layer1 = null;
        private byte[] Layer2 = null;
        private byte[] Acre = null;
        private byte[] Building = null;
        private byte[] Terrain = null;

        private byte[][] item = null;
        private byte[][] itemWithSpace = null;
        private int numOfitem = 0;
        private int numOfSpace = 0;
        private bool[] isSpace;
        private int rowNum;
        private byte[][] SpawnArea = null;
        private bool spawnlock = false;
        public bulkSpawn(Socket S, USBBot Bot, byte[] layer1, byte[] layer2, byte[] acre, byte[] building, byte[] terrain, int x, int y, map Map, bool Ignore, bool Sound)
        {
            try
            {
                s = S;
                bot = Bot;
                Layer1 = layer1;
                Layer2 = layer2;
                Acre = acre;
                Building = building;
                Terrain = terrain;
                anchorX = x;
                anchorY = y;
                main = Map;
                ignore = Ignore;
                sound = Sound;
                MiniMap = new miniMap(Layer1, Acre, Building, Terrain, 4);
                InitializeComponent();
                xCoordinate.Text = x.ToString();
                yCoordinate.Text = y.ToString();
                miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawBackground(), MiniMap.drawItemMap());
                miniMapBox.Image = MiniMap.drawMarker(anchorX, anchorY);
                warningMessage.SelectionAlignment = HorizontalAlignment.Center;
                MyLog.logEvent("BulkSpawn", "BulkSpawnForm Started Successfully");
            }
            catch (Exception ex)
            {
                MyLog.logEvent("BulkSpawn", "Form Construct: " + ex.Message.ToString());
            }
        }
        private void selectBtn_Click(object sender, EventArgs e)
        {
            spawnBtn.Visible = false;

            OpenFileDialog file = new OpenFileDialog()
            {
                Filter = "New Horizons Bulk Spawn (*.nhbs)|*.nhbs|New Horizons Inventory(*.nhi) | *.nhi",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            string savepath;

            if (config.AppSettings.Settings["LastLoad"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + @"\save";
            else
                savepath = config.AppSettings.Settings["LastLoad"].Value;

            if (Directory.Exists(savepath))
            {
                file.InitialDirectory = savepath;
            }
            else
            {
                file.InitialDirectory = @"C:\";
            }

            if (file.ShowDialog() != DialogResult.OK)
                return;

            string[] temp = file.FileName.Split('\\');
            string path = "";
            for (int i = 0; i < temp.Length - 1; i++)
                path = path + temp[i] + "\\";

            config.AppSettings.Settings["LastLoad"].Value = path;
            config.Save(ConfigurationSaveMode.Minimal);

            byte[] data = File.ReadAllBytes(file.FileName);

            processNHBS(data);
            numOfItemBox.Text = numOfitem.ToString();
            numOfSpaceBox.Text = numOfSpace.ToString();
            settingPanel.Visible = true;
        }

        private void processNHBS(byte[] data)
        {
            byte[] tempItem = new byte[8];
            bool[] isItem = new bool[data.Length / 8];
            isSpace = new bool[data.Length / 8];

            numOfitem = 0;
            numOfSpace = 0;

            for (int i = 0; i < data.Length / 8; i++)
            {
                Buffer.BlockCopy(data, 0x8 * i, tempItem, 0, 8);
                if (!Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                {
                    isItem[i] = true;
                    numOfitem++;
                    isSpace[i] = false;
                }
                else
                {
                    numOfSpace++;
                    isSpace[i] = true;
                }
            }

            item = new byte[numOfitem][];
            itemWithSpace = new byte[numOfitem + numOfSpace][];

            int itemNum = 0;

            for (int j = 0; j < data.Length / 8; j++)
            {
                if (isItem[j])
                {
                    item[itemNum] = new byte[8];
                    Buffer.BlockCopy(data, 0x8 * j, item[itemNum], 0, 8);
                    itemNum++;
                }

                itemWithSpace[j] = new byte[8];
                Buffer.BlockCopy(data, 0x8 * j, itemWithSpace[j], 0, 8);
            }
        }

        private void previewBtn_Click(object sender, EventArgs e)
        {
            miniMapBox.Image = null;

            if (heightNumber.Text.Equals(string.Empty) || item == null)
                return;
            else
            {
                rowNum = int.Parse(heightNumber.Text);
                if (rowNum <= 0)
                    return;
                widthLabel.Visible = true;
                widthNumber.Visible = true;
                SpawnArea = buildSpawnArea(rowNum);
                widthNumber.Text = (SpawnArea.Length / 2).ToString();

                bool right;
                if (leftBtn.Checked)
                    right = false;
                else
                    right = true;

                xCoordinate.Text = anchorX.ToString();
                yCoordinate.Text = anchorY.ToString();

                if (IgnoreSpaceToggle.Checked)
                    miniMapBox.Image = MiniMap.drawPreview(rowNum, SpawnArea.Length / 2, anchorX, anchorY, right);
                else
                    miniMapBox.Image = MiniMap.drawPreview(rowNum, SpawnArea.Length / 2, anchorX, anchorY, right, isSpace);

                if (anchorY + rowNum > 96)
                {
                    warningMessage.Visible = true;
                    spawnBtn.Visible = false;
                }
                else if (right && anchorX + SpawnArea.Length / 2 > 112)
                {
                    warningMessage.Visible = true;
                    spawnBtn.Visible = false;
                }
                else if (!right && anchorX - SpawnArea.Length / 2 < -1)
                {
                    warningMessage.Visible = true;
                    spawnBtn.Visible = false;
                }
                else
                {
                    warningMessage.Visible = false;
                    spawnBtn.Visible = true;
                }
            }
        }

        private byte[][] buildSpawnArea(int t)
        {
            int itemNum = 0;
            byte[] emptyLeft = Utilities.stringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.stringToByte("FEFF000000000000FEFF000000000000");
            byte[][] processingArea;

            if (IgnoreSpaceToggle.Checked)
            {
                processingArea = item;
            }
            else
            {
                processingArea = itemWithSpace;
            }

            int numberOfColumn;
            if (processingArea.Length % t == 0)
                numberOfColumn = (processingArea.Length / t);
            else
                numberOfColumn = (processingArea.Length / t + 1);

            int sizeOfRow = 16;

            byte[][] b = new byte[numberOfColumn * 2][];

            for (int i = 0; i < numberOfColumn * 2; i++)
            {
                b[i] = new byte[t * sizeOfRow];
            }

            for (int i = 0; i < numberOfColumn; i++)
            {
                for (int j = 0; j < t; j++)
                {
                    if (itemNum < processingArea.Length)
                    {
                        transformToFloorItem(ref b[i * 2], ref b[i * 2 + 1], j, processingArea[itemNum]);
                        itemNum++;
                    }
                    else
                    {
                        Buffer.BlockCopy(emptyLeft, 0, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(emptyRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }
            return b;
        }

        private void transformToFloorItem(ref byte[] b1, ref byte[] b2, int slot, byte[] item)
        {
            byte[] slotBytes = new byte[2];
            byte[] flag1Bytes = new byte[1];
            byte[] flag2Bytes = new byte[1];
            byte[] dataBytes = new byte[4];

            Buffer.BlockCopy(item, 0x0, slotBytes, 0, 2);
            Buffer.BlockCopy(item, 0x3, flag1Bytes, 0, 1);
            Buffer.BlockCopy(item, 0x2, flag2Bytes, 0, 1);
            Buffer.BlockCopy(item, 0x4, dataBytes, 0, 4);

            string itemID = Utilities.flip(Utilities.ByteToHexString(slotBytes));
            string itemData = Utilities.flip(Utilities.ByteToHexString(dataBytes));
            string flag1 = Utilities.ByteToHexString(flag1Bytes);
            string flag2;
            if (itemID == "FFFE" || itemID == "FFFD")
                flag2 = "00";
            else
                flag2 = "20";

            byte[] dropItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
            byte[] dropItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

            Buffer.BlockCopy(dropItemLeft, 0, b1, slot * 0x10, 16);
            Buffer.BlockCopy(dropItemRight, 0, b2, slot * 0x10, 16);

        }

        private void bulkSpawn_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyLog.logEvent("BulkSpawn", "Form Closed");
            main.bulk = null;
        }

        private void spawnBtn_Click(object sender, EventArgs e)
        {
            settingPanel.Visible = false;
            spawnlock = true;

            if (rightBtn.Checked)
            {
                Thread SpawnThread = new Thread(delegate () { bulkSpawnConfirm(true); });
                SpawnThread.Start();
            }
            else
            {
                Thread SpawnThread = new Thread(delegate () { bulkSpawnConfirm(false); });
                SpawnThread.Start();
            }
        }
        private void bulkSpawnConfirm(bool right)
        {
            showMapWait(SpawnArea.Length, "Spawning items...");

            try
            {
                int time = SpawnArea.Length / 4;

                Debug.Print("Length :" + SpawnArea.Length + " Time : " + time);

                int counter = 0;

                while (isAboutToSave(time + 10))
                {
                    if (counter > 5)
                    {
                        DialogResult result = MyMessageBox.Show("Something seems to be wrong with the autosave detection.\n" +
                                                        "Would you like to ignore the autosave protection and spawn the item(s) anyway?\n\n" +
                                                        "Please be noted that spawning item during autosave might crash the game."
                                                        , "Waiting for autosave to complete...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            break;
                        }
                        else
                        {
                            if (sound)
                                System.Media.SystemSounds.Asterisk.Play();

                            hideMapWait();

                            spawnlock = false;

                            this.Invoke((MethodInvoker)delegate
                            {
                                main.moveAnchor(anchorX, anchorY);
                                this.Close();
                            });
                            return;
                        }
                    }
                    counter++;
                    Thread.Sleep(5000);
                }


                if (right)
                {
                    for (int i = 0; i < SpawnArea.Length / 2; i++)
                    {
                        UInt32 address = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + i)) + (0x10 * (anchorY)));

                        Utilities.dropColumn(s, bot, address, address + 0x600, SpawnArea[i * 2], SpawnArea[i * 2 + 1], ref counter);
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        main.updataData(anchorX, anchorY, SpawnArea, false, true);
                    });
                }
                else
                {
                    for (int i = 0; i < SpawnArea.Length / 2; i++)
                    {
                        UInt32 address = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - i)) + (0x10 * (anchorY)));

                        Utilities.dropColumn(s, bot, address, address + 0x600, SpawnArea[i * 2], SpawnArea[i * 2 + 1], ref counter);
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        main.updataData(anchorX, anchorY, SpawnArea, false, false);
                    });
                }
            }
            catch (Exception ex)
            {
                MyLog.logEvent("BulkSpawn", "ConfirmSpawn: " + ex.Message.ToString());
                MyMessageBox.Show(ex.Message.ToString(), "When I wrote this, only God and I understood what I was doing.");
            }

            Thread.Sleep(5000);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            hideMapWait();

            spawnlock = false;

            this.Invoke((MethodInvoker)delegate
            {
                main.moveAnchor(anchorX, anchorY);
                this.Close();
            });
        }

        private void showMapWait(int size, string msg = "")
        {
            this.Invoke((MethodInvoker)delegate
            {
                WaitMessagebox.SelectionAlignment = HorizontalAlignment.Center;
                WaitMessagebox.Text = msg;
                counter = 0;
                MapProgressBar.Maximum = size + 5;
                MapProgressBar.Value = counter;
                PleaseWaitPanel.Visible = true;
                ProgressTimer.Start();
            });
        }

        private void hideMapWait()
        {
            this.Invoke((MethodInvoker)delegate
            {
                PleaseWaitPanel.Visible = false;
                ProgressTimer.Stop();
            });
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                if (counter <= MapProgressBar.Maximum)
                    MapProgressBar.Value = counter;
                else
                    MapProgressBar.Value = MapProgressBar.Maximum;
            });
        }

        private void miniMapBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (spawnlock)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int x;
                int y;

                if (e.X / 4 < 0)
                    x = 0;
                else if (e.X / 4 > 111)
                    x = 111;
                else
                    x = e.X / 4;

                if (e.Y / 4 < 0)
                    y = 0;
                else if (e.Y / 4 > 95)
                    y = 95;
                else
                    y = e.Y / 4;

                anchorX = x;
                anchorY = y;

                xCoordinate.Text = x.ToString();
                yCoordinate.Text = y.ToString();

                miniMapBox.Image = MiniMap.drawMarker(anchorX, anchorY);
                warningMessage.Visible = false;
                spawnBtn.Visible = false;
            }
        }

        private bool isAboutToSave(int second)
        {
            if (ignore)
                return false;

            try
            {
                byte[] b = Utilities.getSaving(s, bot);

                if (b == null)
                    return true;
                if (b[0] == 1)
                    return true;
                else
                {
                    byte[] currentFrame = new byte[4];
                    byte[] lastFrame = new byte[4];
                    Buffer.BlockCopy(b, 12, currentFrame, 0, 4);
                    Buffer.BlockCopy(b, 16, lastFrame, 0, 4);

                    int currentFrameStr = Convert.ToInt32("0x" + Utilities.flip(Utilities.ByteToHexString(currentFrame)), 16);
                    int lastFrameStr = Convert.ToInt32("0x" + Utilities.flip(Utilities.ByteToHexString(lastFrame)), 16);
                    int FrameRemain = ((0x1518 - (currentFrameStr - lastFrameStr)));

                    if (FrameRemain < 30 * second) // Not enough
                        return true;
                    else if (FrameRemain >= 30 * 300) // Have too too many for some reason?
                        return false;
                    else if (FrameRemain >= 30 * 175) // Just finish save buffer
                        return true;
                    else
                    {
                        Debug.Print(((0x1518 - (currentFrameStr - lastFrameStr))).ToString());
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MyLog.logEvent("BulkSpawn", "isAboutToSave: " + ex.Message.ToString());
                MyMessageBox.Show(ex.Message.ToString(), "This is utterly fucking retarded.");
                return false;
            }
        }

        private void heightNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!(c >= '0' && c <= '9'))
            {
                e.Handled = true;
            }
        }

        private void CoordinateChanged(object sender, EventArgs e)
        {
            if (spawnlock)
                return;
            if (xCoordinate.Text.Equals(string.Empty) || yCoordinate.Text.Equals(string.Empty))
                return;

            int x = int.Parse(xCoordinate.Text);
            int y = int.Parse(yCoordinate.Text);

            if (x < 0)
                x = 0;
            else if (x > 111)
                x = 111;

            if (y < 0)
                y = 0;
            else if (y > 95)
                y = 95;

            anchorX = x;
            anchorY = y;

            //xCoordinate.Text = x.ToString();
            //yCoordinate.Text = y.ToString();

            miniMapBox.Image = MiniMap.drawMarker(anchorX, anchorY);
            warningMessage.Visible = false;
            spawnBtn.Visible = false;
        }

        private void CoordinateKeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!(c >= '0' && c <= '9'))
            {
                e.Handled = true;
            }
        }

        private void IgnoreSpaceToggle_CheckedChanged(object sender, EventArgs e)
        {
            miniMapBox.Image = null;
            warningMessage.Visible = false;
            spawnBtn.Visible = false;

            if (IgnoreSpaceToggle.Checked)
            {
                numOfSpaceLabel.Visible = false;
                numOfSpaceBox.Visible = false;
            }
            else
            {
                numOfSpaceLabel.Visible = true;
                numOfSpaceBox.Visible = true;
            }
        }
    }
}
