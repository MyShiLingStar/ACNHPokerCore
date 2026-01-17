using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class BulkSpawn : Form
    {
        private readonly Socket s;
        private readonly USBBot bot;
        private Map MainMap;
        private int counter;
        private int anchorX = -1;
        private int anchorY = -1;
        private readonly bool ignore;
        private readonly bool sound;

        private readonly long SpawnAddress;

        private byte[][] item;
        private byte[][] itemWithSpace;
        private int numOfitem;
        private int numOfSpace;
        private bool[] isSpace;
        private int rowNum;
        private int colNum;
        private byte[][] SpawnArea;
        private bool spawnlock;
        private readonly bool debugging;
        public BulkSpawn(Socket S, USBBot Bot, byte[] layer1, byte[] layer2, byte[] acre, byte[] building, byte[] terrain, byte[] design, int x, int y, bool Ignore, bool Sound, bool Debugging, bool Layer1Selected)
        {
            try
            {
                s = S;
                bot = Bot;
                anchorX = x;
                anchorY = y;
                ignore = Ignore;
                sound = Sound;
                debugging = Debugging;
                var miniMap = new MiniMap(layer1, acre, building, terrain, design, 4);

                InitializeComponent();

                xCoordinate.Value = x;
                yCoordinate.Value = y;

                miniMapBox.BackgroundImage = MiniMap.CombineMap(miniMap.DrawBackground(), miniMap.DrawItemMap());
                if (Layer1Selected)
                {
                    SpawnAddress = Utilities.mapZero;
                }
                else
                {
                    miniMapBox.BackgroundImage = miniMap.RefreshItemMap(layer2);
                    SpawnAddress = Utilities.mapZero + Utilities.NewMapSize;
                }
                miniMapBox.Image = MiniMap.DrawMarker(anchorX, anchorY);
                WarningMessage.SelectionAlignment = HorizontalAlignment.Center;
                WarningMessage.Location = new Point(471, 368);
                PleaseWaitPanel.Location = new Point(465, 330);
                Bitmap HoriWidthImage = new(Properties.Resources.height, new Size(50, 50));
                HoriWidthImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                HoriWidthImageBox.Image = HoriWidthImage;

                MyLog.LogEvent("BulkSpawn", "BulkSpawnForm Started Successfully");
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("BulkSpawn", "Form Construct: " + ex.Message);
            }
        }

        public void SetOwner(Map map)
        {
            MainMap = map;
        }

        private void SelectBtn_Click(object sender, EventArgs e)
        {
            SpawnBtn.Visible = false;

            OpenFileDialog file = new()
            {
                Filter = @"New Horizons Bulk Spawn/New Horizons Inventory (*.nhbs;*.nhi)|*.nhbs;*.nhi|All files (*.*)|*.*",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            string savepath;

            if (config.AppSettings.Settings["LastLoad"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
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

            ProcessNHBS(data);

            numOfItemBox.Text = numOfitem.ToString();
            numOfSpaceBox.Text = numOfSpace.ToString();
            int total = numOfitem + numOfSpace;

            VertThenHoriz.Visible = true;
            HorizThenVert.Visible = true;
            OtherSettingPanel.Visible = true;

            if (total < 96)
                VertHeightNumber.Maximum = total;
            else
                VertHeightNumber.Maximum = 96;

            if (total < 112)
                HoriWidthNumber.Maximum = total;
            else
                HoriWidthNumber.Maximum = 112;

            if (VertThenHoriz.Checked)
                VertSettingPanel.Visible = true;
            else if (HorizThenVert.Checked)
                HoriSettingPanel.Visible = true;

            UpdatePreview();
        }

        private void ProcessNHBS(byte[] data)
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

        private byte[][] BuildSpawnArea(int row)
        {
            int itemNum = 0;
            string flag = Utilities.PrecedingZeros(FlagTextbox.Text, 2);
            byte[] emptyLeft = Utilities.StringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.StringToByte("FEFF000000000000FEFF000000000000");
            byte[][] processingArea;

            if (IgnoreSpaceToggle.Checked)
                processingArea = item;
            else
                processingArea = itemWithSpace;

            int numberOfColumn;
            if (processingArea.Length % row == 0)
                numberOfColumn = (processingArea.Length / row);
            else
                numberOfColumn = (processingArea.Length / row + 1);

            int sizeOfRow = 16;

            byte[][] b = new byte[numberOfColumn * 2][];

            for (int i = 0; i < numberOfColumn * 2; i++)
            {
                b[i] = new byte[row * sizeOfRow];
            }

            for (int i = 0; i < numberOfColumn; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    if (itemNum < processingArea.Length)
                    {
                        TransformToFloorItem(ref b[i * 2], ref b[i * 2 + 1], j, processingArea[itemNum], flag);
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

        private byte[][] BuildSpawnAreaHoriz(int col)
        {
            int itemNum = 0;
            string flag = Utilities.PrecedingZeros(FlagTextbox.Text, 2);
            byte[] emptyLeft = Utilities.StringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.StringToByte("FEFF000000000000FEFF000000000000");
            byte[][] processingArea;

            if (IgnoreSpaceToggle.Checked)
                processingArea = item;
            else
                processingArea = itemWithSpace;

            int numberOfRow;
            if (processingArea.Length % col == 0)
                numberOfRow = (processingArea.Length / col);
            else
                numberOfRow = (processingArea.Length / col + 1);

            int sizeOfRow = 16;

            byte[][] b = new byte[col * 2][];

            for (int i = 0; i < col * 2; i++)
            {
                b[i] = new byte[numberOfRow * sizeOfRow];
            }

            for (int i = 0; i < numberOfRow; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (itemNum < processingArea.Length)
                    {
                        TransformToFloorItem(ref b[j * 2], ref b[j * 2 + 1], i, processingArea[itemNum], flag);
                        itemNum++;
                    }
                    else
                    {
                        Buffer.BlockCopy(emptyLeft, 0, b[j * 2], 0x10 * i, 16);
                        Buffer.BlockCopy(emptyRight, 0, b[j * 2 + 1], 0x10 * i, 16);
                    }
                }
            }

            return b;
        }

        private static void TransformToFloorItem(ref byte[] b1, ref byte[] b2, int slot, byte[] item, string flag)
        {
            byte[] slotBytes = new byte[2];
            byte[] flag0Bytes = new byte[1];
            byte[] flag1Bytes = new byte[1];
            byte[] dataBytes = new byte[4];

            Buffer.BlockCopy(item, 0x0, slotBytes, 0, 2);
            Buffer.BlockCopy(item, 0x3, flag0Bytes, 0, 1);
            Buffer.BlockCopy(item, 0x2, flag1Bytes, 0, 1);
            Buffer.BlockCopy(item, 0x4, dataBytes, 0, 4);

            string itemID = Utilities.Flip(Utilities.ByteToHexString(slotBytes));
            string itemData = Utilities.Flip(Utilities.ByteToHexString(dataBytes));
            string flag0 = Utilities.ByteToHexString(flag0Bytes);
            string flag1;
            if (itemID == "FFFE" || itemID == "FFFD")
                flag1 = "00";
            else
                flag1 = flag;

            byte[] dropItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
            byte[] dropItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

            Buffer.BlockCopy(dropItemLeft, 0, b1, slot * 0x10, 16);
            Buffer.BlockCopy(dropItemRight, 0, b2, slot * 0x10, 16);

        }

        private void BulkSpawn_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyLog.LogEvent("BulkSpawn", "Form Closed");
            MainMap.bulkSpawn = null;
        }

        private void SpawnBtn_Click(object sender, EventArgs e)
        {
            VertSettingPanel.Visible = false;
            OtherSettingPanel.Visible = false;

            spawnlock = true;

            if (RightBtn.Checked)
            {
                Thread SpawnThread = new(delegate () { BulkSpawnConfirm(true); });
                SpawnThread.Start();
            }
            else
            {
                Thread SpawnThread = new(delegate () { BulkSpawnConfirm(false); });
                SpawnThread.Start();
            }
        }
        private void BulkSpawnConfirm(bool right)
        {
            ShowMapWait(SpawnArea.Length, "Spawning items...");

            try
            {
                if (!debugging)
                {
                    int time = SpawnArea.Length / 4;

                    Debug.Print("Length :" + SpawnArea.Length + " Time : " + time);

                    /*
                    int counter = 0;

                    while (Utilities.IsAboutToSave(s, bot, time + 10, 0, ignore))
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

                                HideMapWait();

                                spawnlock = false;

                                Invoke((MethodInvoker)delegate
                                {
                                    MainMap.MoveAnchor(anchorX, anchorY);
                                    Close();
                                });
                                return;
                            }
                        }
                        counter++;
                        Thread.Sleep(5000);
                    }
                    */
                }

                if (right)
                {
                    if (!debugging)
                    {
                        for (int i = 0; i < SpawnArea.Length / 2; i++)
                        {
                            UInt32 address = (UInt32)(SpawnAddress + (0xC00 * (anchorX + i + 16)) + (0x10 * (anchorY)));

                            Utilities.DropColumn(s, bot, address, address + 0x600, SpawnArea[i * 2], SpawnArea[i * 2 + 1], ref counter);
                        }
                    }

                    Invoke((MethodInvoker)delegate
                    {
                        MainMap.UpdataData(anchorX, anchorY, SpawnArea, false, true);
                    });
                }
                else
                {
                    if (!debugging)
                    {
                        for (int i = 0; i < SpawnArea.Length / 2; i++)
                        {
                            UInt32 address = (UInt32)(SpawnAddress + (0xC00 * (anchorX - i + 16)) + (0x10 * (anchorY)));

                            Utilities.DropColumn(s, bot, address, address + 0x600, SpawnArea[i * 2], SpawnArea[i * 2 + 1], ref counter);
                        }
                    }

                    Invoke((MethodInvoker)delegate
                    {
                        MainMap.UpdataData(anchorX, anchorY, SpawnArea, false, false);
                    });
                }
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("BulkSpawn", "ConfirmSpawn: " + ex.Message);
                MyMessageBox.Show(ex.Message, "When I wrote this, only God and I understood what I was doing.");
            }

            if (!debugging)
            {
                Thread.Sleep(5000);
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();

            spawnlock = false;

            Invoke((MethodInvoker)delegate
            {
                MainMap.MoveAnchor(anchorX, anchorY);
                Close();
            });
        }

        private void ShowMapWait(int size, string msg = "")
        {
            Invoke((MethodInvoker)delegate
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

        private void HideMapWait()
        {
            Invoke((MethodInvoker)delegate
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

        private void MiniMapBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (spawnlock)
                return;
            if (e.Button == MouseButtons.Left)
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

                bool hasChange = false;
                if (anchorX != x || anchorY != y)
                    hasChange = true;

                anchorX = x;
                anchorY = y;

                miniMapBox.Image = MiniMap.DrawMarker(anchorX, anchorY);

                WarningMessage.Visible = false;

                xCoordinate.Value = x;
                yCoordinate.Value = y;

                if (!hasChange)
                    UpdatePreview();
                //VertSpawnBtn.Visible = false;
            }
        }

        private void CoordinateChanged(object sender, EventArgs e)
        {
            if (spawnlock)
                return;
            if (xCoordinate.Text.Equals(string.Empty) || yCoordinate.Text.Equals(string.Empty))
                return;

            int x = (int)xCoordinate.Value;
            int y = (int)yCoordinate.Value;

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

            miniMapBox.Image = MiniMap.DrawMarker(anchorX, anchorY);
            WarningMessage.Visible = false;
            //VertSpawnBtn.Visible = false;

            UpdatePreview();
        }

        private void IgnoreSpaceToggle_CheckedChanged(object sender, EventArgs e)
        {
            //miniMapBox.Image = null;
            WarningMessage.Visible = false;
            //VertSpawnBtn.Visible = false;

            /*
            if (VertIgnoreSpaceToggle.Checked)
            {
                numOfSpaceLabel.Visible = false;
                numOfSpaceBox.Visible = false;
            }
            else
            {
                numOfSpaceLabel.Visible = true;
                numOfSpaceBox.Visible = true;
            }
            */

            UpdatePreview();
        }

        private void VertThenHoriz_CheckedChanged(object sender, EventArgs e)
        {
            if (VertThenHoriz.Checked)
            {
                VertSettingPanel.Visible = true;
                HoriSettingPanel.Visible = false;
            }
            else
            {
                VertSettingPanel.Visible = false;
                HoriSettingPanel.Visible = true;
            }

            UpdatePreview();
        }

        private void VertHeightNumber_ValueChanged(object sender, EventArgs e)
        {
            if (VertThenHoriz.Checked)
            {
                if (VertHeightNumber.Value > HoriWidthNumber.Maximum)
                    HoriWidthNumber.Value = HoriWidthNumber.Maximum;
                else
                    HoriWidthNumber.Value = VertHeightNumber.Value;
                UpdatePreview();
            }
        }

        private void LeftBtn_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void RightBtn_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (VertThenHoriz.Checked)
            {
                if (VertHeightNumber.Text.Equals(string.Empty) || item == null)
                    return;
                rowNum = (int)VertHeightNumber.Value;
                if (rowNum <= 0)
                    return;
                //VertWidthLabel.Visible = true;
                //VertWidthNumber.Visible = true;
                SpawnArea = BuildSpawnArea(rowNum);
                VertWidthNumber.Text = (SpawnArea.Length / 2).ToString();

                bool right;
                if (LeftBtn.Checked)
                    right = false;
                else
                    right = true;

                xCoordinate.Value = anchorX;
                yCoordinate.Value = anchorY;

                if (IgnoreSpaceToggle.Checked)
                    miniMapBox.Image = MiniMap.DrawPreview(rowNum, SpawnArea.Length / 2, anchorX, anchorY, right);
                else
                    miniMapBox.Image = MiniMap.DrawPreview(rowNum, SpawnArea.Length / 2, anchorX, anchorY, right, isSpace);

                if (anchorY + rowNum > 96)
                {
                    WarningMessage.Visible = true;
                    SpawnBtn.Visible = false;
                }
                else if (right && anchorX + SpawnArea.Length / 2 > 112)
                {
                    WarningMessage.Visible = true;
                    SpawnBtn.Visible = false;
                }
                else if (!right && anchorX - SpawnArea.Length / 2 < -1)
                {
                    WarningMessage.Visible = true;
                    SpawnBtn.Visible = false;
                }
                else
                {
                    WarningMessage.Visible = false;
                    SpawnBtn.Visible = true;
                }
            }
            else if (HorizThenVert.Checked)
            {
                if (HoriWidthNumber.Text.Equals(string.Empty) || item == null)
                    return;
                colNum = (int)HoriWidthNumber.Value;
                if (colNum <= 0)
                    return;

                SpawnArea = BuildSpawnAreaHoriz(colNum);
                HoriHeightNumber.Text = (SpawnArea[0].Length / 16).ToString();

                bool right;
                if (LeftBtn.Checked)
                    right = false;
                else
                    right = true;

                xCoordinate.Value = anchorX;
                yCoordinate.Value = anchorY;

                if (IgnoreSpaceToggle.Checked)
                    miniMapBox.Image = MiniMap.DrawPreviewHori(SpawnArea[0].Length / 16, colNum, anchorX, anchorY, right);
                else
                    miniMapBox.Image = MiniMap.DrawPreviewHori(SpawnArea[0].Length / 16, colNum, anchorX, anchorY, right, isSpace);

                if (anchorY + (SpawnArea[0].Length / 16) > 96)
                {
                    WarningMessage.Visible = true;
                    SpawnBtn.Visible = false;
                }
                else if (right && anchorX + colNum > 112)
                {
                    WarningMessage.Visible = true;
                    SpawnBtn.Visible = false;
                }
                else if (!right && anchorX - colNum < -1)
                {
                    WarningMessage.Visible = true;
                    SpawnBtn.Visible = false;
                }
                else
                {
                    WarningMessage.Visible = false;
                    SpawnBtn.Visible = true;
                }
            }
        }

        private void FlagTextbox_ValueChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void HoriWidthNumber_ValueChanged(object sender, EventArgs e)
        {
            if (HorizThenVert.Checked)
            {
                if (HoriWidthNumber.Value > VertHeightNumber.Maximum)
                    VertHeightNumber.Value = VertHeightNumber.Maximum;
                else
                    VertHeightNumber.Value = HoriWidthNumber.Value;
                UpdatePreview();
            }
        }

    }
}
