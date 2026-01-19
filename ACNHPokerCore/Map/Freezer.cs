using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class Freezer : Form
    {
        private static Socket s;
        private readonly bool debugging;
        private readonly bool sound;
        private int counter;
        private MiniMap MiniMap;
        private int anchorX = -1;
        private int anchorY = -1;
        private byte[] tempData;
        private static byte[][] villagerFlag;
        private static byte[][] villager;
        private static bool[] haveVillager;

        public event CloseHandler CloseForm;

        public bool isChinese = false;

        public Freezer(Socket S, bool Sound, bool Debugging)
        {
            s = S;
            sound = Sound;
            debugging = Debugging;

            InitializeComponent();

            if (!debugging)
            {
                int freezeCount = Utilities.GetFreezeCount(s);
                UpdateFreezeCountLabel(freezeCount);
                isChinese = Utilities.IsChinese(s);
            }

            FinMsg.SelectionAlignment = HorizontalAlignment.Center;

            MyLog.LogEvent("Freeze", "Freeze Form Started Successfully");
        }

        private void SaveMapBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog file = new()
                {
                    Filter = "New Horizons Fasil 3 (*.nhf3)|*.nhf3",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                string savepath;

                if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
                else
                    savepath = config.AppSettings.Settings["LastSave"].Value;

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

                config.AppSettings.Settings["LastSave"].Value = path;
                config.Save(ConfigurationSaveMode.Minimal);

                uint address = Utilities.mapZero;

                Thread LoadThread = new(delegate () { SaveMapFloor(address, file); });
                LoadThread.Start();

            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Regen", "Save: " + ex.Message);
            }
        }

        private void SaveMapFloor(uint address, SaveFileDialog file)
        {
            ShowMapWait(60, "Saving...");

            LockControl();

            byte[] save = Utilities.ReadByteArray(s, address, (int)(Utilities.NewMapSize * 2), ref counter);

            File.WriteAllBytes(file.FileName, save);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();

            UnlockControl();

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Template Saved!";
            });
        }

        private void ShowMapWait(int size, string msg = "")
        {
            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = false;
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

        private void Freezer_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseCleaning();
        }

        private void CloseCleaning()
        {
            MyLog.LogEvent("Freeze", "Form Closed");
            CloseForm?.Invoke();
        }

        private void UnFreezeAllBtn_Click(object sender, EventArgs e)
        {
            ShowMapWait(1, "Unfreezing...");
            LockControl();
            Utilities.SendString(s, Utilities.FreezeClear());
            Thread.Sleep(100);
            int freezeCount = Utilities.GetFreezeCount(s);
            Thread.Sleep(3000);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
            HideMapWait();
            UnlockControl();
            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Build a snowman?";
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void UpdateFreezeCountLabel(int value)
        {
            FreezeCountLabel.Text = value + " / 255";
        }

        private void ChangeRateBtn_Click(object sender, EventArgs e)
        {
            string value = RateBar.Value.ToString();
            Utilities.SendString(s, Utilities.FreezeRate(value));

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Delay Updated! " + value + " ms";
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void EnableTextBtn_Click(object sender, EventArgs e)
        {
            if (isChinese)
                Utilities.SendString(s, Utilities.Freeze(Utilities.TextSpeedAddress + Utilities.ChineseLanguageOffset, [3]));
            else
                Utilities.SendString(s, Utilities.Freeze(Utilities.TextSpeedAddress, [3]));

            int freezeCount = Utilities.GetFreezeCount(s);

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Instant Text Activated!";
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void DisableTextBtn_Click(object sender, EventArgs e)
        {
            if (isChinese)
                Utilities.SendString(s, Utilities.UnFreeze(Utilities.TextSpeedAddress + Utilities.ChineseLanguageOffset));
            else
                Utilities.SendString(s, Utilities.UnFreeze(Utilities.TextSpeedAddress));

            int freezeCount = Utilities.GetFreezeCount(s);

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Instant Text Deactivated!";
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void RateBar_ValueChanged(object sender, EventArgs e)
        {
            RateValue.Text = RateBar.Value + " ms";
        }

        private void FreezeInvBtn_Click(object sender, EventArgs e)
        {
            byte[] Bank01to20 = Utilities.GetInventoryBank(s, null, 1);
            byte[] Bank21to40 = Utilities.GetInventoryBank(s, null, 21);

            Utilities.SendString(s, Utilities.Freeze(Utilities.ItemSlotBase, Bank01to20));
            Utilities.SendString(s, Utilities.Freeze(Utilities.ItemSlot21Base, Bank21to40));

            int freezeCount = Utilities.GetFreezeCount(s);

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Inventory Freeze Activated!";
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void UnFreezeInvBtn_Click(object sender, EventArgs e)
        {
            Utilities.SendString(s, Utilities.UnFreeze(Utilities.ItemSlotBase));
            Utilities.SendString(s, Utilities.UnFreeze(Utilities.ItemSlot21Base));

            int freezeCount = Utilities.GetFreezeCount(s);

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Inventory Freeze Deactivated!";
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void FreezeMapBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new()
            {
                Filter = "All files (*.*)|*.*|New Horizons Fasil 3 (*.nhf3)|*.nhf3",
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

            if (data.Length == Utilities.OldMapSize || data.Length == Utilities.OldMapSize * 2)
            {
                DialogResult dialogResult = MyMessageBox.Show("As of game version v3.0.0, " + "\n" +
                                                            "The size of the map layer has changed!" + "\n" +
                                                            "\n" +
                                                            "Press  [Yes]  to convert it to the new format (.nhf3)." + "\n" +
                                                            "or  [No]  to cancel." + "\n" + "\n"
                                                            , "Looks like you are using an old map layer!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    UpdateMapLayer(data);
                }

                return;
            }

            if (data.Length != Utilities.NewMapSize * 2)
            {
                MyMessageBox.Show("Invalid File Size!", "Your map file size is invalid!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            uint address = Utilities.mapZero;

            string[] name = file.FileName.Split('\\');

            MyLog.LogEvent("Regen", "Regen3 Started: " + name[name.Length - 1]);

            Thread FreezeThread = new(delegate () { FreezeMapFloor(address, data); });
            FreezeThread.Start();
        }

        private void FreezeMapFloor(uint address, byte[] data)
        {
            ShowMapWait(110, "Casting...");

            LockControl();

            counter = 0;

            int FreezeSize = 0x2000;
            int NumOfPart = (int)(Utilities.NewMapSize * 2 / FreezeSize);

            byte[][] b = new byte[NumOfPart][];

            for (int i = 0; i < NumOfPart; i++)
            {
                b[i] = new byte[FreezeSize];
                Buffer.BlockCopy(data, i * FreezeSize, b[i], 0x0, FreezeSize);
                Utilities.SendString(s, Utilities.Freeze((uint)(address + (i * FreezeSize)), b[i]));
                counter++;
                Thread.Sleep(100);
            }

            int freezeCount = Utilities.GetFreezeCount(s);

            HideMapWait();

            UnlockControl();

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Let it go!";
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void UnFreezeMapBtn_Click(object sender, EventArgs e)
        {
            MyLog.LogEvent("Regen", "Regen3 Stopped");

            uint address = Utilities.mapZero;

            Thread UnFreezeThread = new(delegate () { UnFreezeMapFloor(address); });
            UnFreezeThread.Start();
        }

        private void UnFreezeMapFloor(uint address)
        {
            ShowMapWait(84, "Casting...");

            LockControl();

            for (int i = 0; i < 84; i++)
            {
                Utilities.SendString(s, Utilities.UnFreeze((uint)(address + (i * 0x2000))));
                counter++;
                Thread.Sleep(100);
            }

            int freezeCount = Utilities.GetFreezeCount(s);

            HideMapWait();

            UnlockControl();

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Build a snowman?";
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void LockControl()
        {
            Invoke((MethodInvoker)delegate
            {
                mainPanel.Enabled = false;
            });
        }

        private void UnlockControl()
        {
            Invoke((MethodInvoker)delegate
            {
                mainPanel.Enabled = true;
            });
        }

        private void FreezeMap2Btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new()
            {
                Filter = "New Horizons Fasil 3 (*.nhf3)|*.nhf3|All files (*.*)|*.*",
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

            if (data.Length != Utilities.NewMapSize * 2)
            {
                MyMessageBox.Show("Invalid File Size!", "Your map file size is invalid!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            tempData = data;

            Width = 505;

            if (MiniMap == null)
            {
                counter = 0;

                byte[] Acre = Utilities.GetAcre(s, null);
                byte[] Building = Utilities.GetBuilding(s, null);
                byte[] Terrain = Utilities.GetTerrain(s, null);
                byte[] MapCustomDesgin = null; //Utilities.GetCustomDesignMap(s, null, ref counter);

                if (MiniMap == null)
                    MiniMap = new MiniMap(data, Acre, Building, Terrain, MapCustomDesgin);

                miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawBackground(), MiniMap.DrawItemMap());
            }
            else
                return;
            try
            {
                /*
                byte[] Coordinate = Utilities.GetCoordinate(s, null);
                int x = BitConverter.ToInt32(Coordinate, 0);
                int y = BitConverter.ToInt32(Coordinate, 4);

                anchorX = x - 0x24;
                anchorY = y - 0x18;
                */

                if (anchorX < 3 || anchorY < 3 || anchorX > 108 || anchorY > 92)
                {
                    anchorX = 3;
                    anchorY = 3;
                }
                xCoordinate.Text = anchorX.ToString();
                yCoordinate.Text = anchorY.ToString();
                miniMapBox.Image = MiniMap.DrawSelectSquare(anchorX, anchorY);
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Regen", "getCoordinate: " + ex.Message);
                MyMessageBox.Show("Something doesn't feel right at all. You should restart the program...\n\n" + ex.Message, "!!! THIS SHIT DOESN'T WORK!! WHY? HAS I EVER?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



            /*
            Log.logEvent("Regen", "Regen4 Started: " + name[name.Length - 1]);

            Thread FreezeThread = new Thread(delegate () { FreezeMapFloor(address, data); });
            FreezeThread.Start();
            */
        }

        private void MiniMapBox_MouseDown(object sender, MouseEventArgs e)
        {
            Debug.Print(e.X + " " + e.Y);

            int x;
            int y;

            if (e.X / 2 < 3)
                x = 3;
            else if (e.X / 2 > 108)
                x = 108;
            else
                x = e.X / 2;

            if (e.Y / 2 < 3)
                y = 3;
            else if (e.Y / 2 > 92)
                y = 92;
            else
                y = e.Y / 2;

            anchorX = x;
            anchorY = y;

            xCoordinate.Text = x.ToString();
            yCoordinate.Text = y.ToString();

            miniMapBox.Image = MiniMap.DrawSelectSquare(anchorX, anchorY);
        }

        private void MiniMapBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x;
                int y;

                if (e.X / 2 < 3)
                    x = 3;
                else if (e.X / 2 > 108)
                    x = 108;
                else
                    x = e.X / 2;

                if (e.Y / 2 < 3)
                    y = 3;
                else if (e.Y / 2 > 92)
                    y = 92;
                else
                    y = e.Y / 2;

                anchorX = x;
                anchorY = y;

                xCoordinate.Text = x.ToString();
                yCoordinate.Text = y.ToString();

                miniMapBox.Image = MiniMap.DrawSelectSquare(anchorX, anchorY);
            }
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            if (tempData.Length != Utilities.NewMapSize * 2)
            {
                MyMessageBox.Show("Invalid File Size!", "Your map file size is invalid!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            uint address = Utilities.mapZero;

            Thread FreezeThread = new(delegate () { FreezeMapFloor2(address, tempData, anchorX, anchorY); });
            FreezeThread.Start();
        }

        private void FreezeMapFloor2(uint address, byte[] data, int x, int y)
        {
            ShowMapWait(124, "Casting...");

            LockControl();

            byte[][] b = new byte[112][];

            for (int i = 0; i < 112; i++)
            {
                b[i] = new byte[0x1800];
                Buffer.BlockCopy(data, i * 0x1800, b[i], 0x0, 0x1800);
                if ((x % 2 == 0) && (i == x / 2 - 1))
                {

                }
                else if ((x % 2 != 0) && (i == x / 2))
                {

                }
                else if ((x % 2 == 0) && (i == x / 2))
                {
                    Spliter((uint)(address + ((i - 1) * 0x1800)), b[i - 1], b[i], y, false);
                }
                else if ((x % 2 != 0) && (i == x / 2 + 1))
                {
                    Spliter((uint)(address + ((i - 1) * 0x1800)), b[i - 1], b[i], y, true);
                }
                else
                {
                    Utilities.SendString(s, Utilities.Freeze((uint)(address + (i * 0x1800)), b[i]));
                }
                counter++;
                Thread.Sleep(100);
            }

            int freezeCount = Utilities.GetFreezeCount(s);

            HideMapWait();

            UnlockControl();

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Let it go!";
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void Spliter(uint address, byte[] first, byte[] second, int y, bool front)
        {
            int size = 0x10;
            byte[][] parts = new byte[13][];
            int[] offsets = new int[13];
            int topLength = y - 1;
            int bottomLength = 96 - 3 - topLength;

            if (front)
            {
                //====================================================

                offsets[0] = 0x0;
                parts[0] = new byte[size * topLength];
                Buffer.BlockCopy(first, 0x0, parts[0], 0x0, size * topLength);

                offsets[1] = size * (y + 2);
                parts[1] = new byte[size * bottomLength];
                Buffer.BlockCopy(first, size * (y + 2), parts[1], 0x0, size * bottomLength);

                //====================================================

                offsets[2] = 0x600;
                parts[2] = new byte[size * topLength];
                Buffer.BlockCopy(first, 0x600, parts[2], 0x0, size * topLength);

                offsets[3] = 0x600 + size * (y + 2);
                parts[3] = new byte[size * bottomLength];
                Buffer.BlockCopy(first, 0x600 + size * (y + 2), parts[3], 0x0, size * bottomLength);

                //====================================================

                offsets[4] = 0xC00;
                parts[4] = new byte[size * topLength];
                Buffer.BlockCopy(first, 0xC00, parts[4], 0x0, size * topLength);

                offsets[5] = 0xC00 + size * (y + 2);
                parts[5] = new byte[size * bottomLength];
                Buffer.BlockCopy(first, 0xC00 + size * (y + 2), parts[5], 0x0, size * bottomLength);

                //====================================================

                offsets[6] = 0x1200;
                parts[6] = new byte[size * topLength];
                Buffer.BlockCopy(first, 0x1200, parts[6], 0x0, size * topLength);

                offsets[7] = 0x1200 + size * (y + 2);
                parts[7] = new byte[size * bottomLength];
                Buffer.BlockCopy(first, 0x1200 + size * (y + 2), parts[7], 0x0, size * bottomLength);

                //====================================================

                offsets[8] = 0x1800;
                parts[8] = new byte[size * topLength];
                Buffer.BlockCopy(second, 0x0, parts[8], 0x0, size * topLength);

                offsets[9] = 0x1800 + size * (y + 2);
                parts[9] = new byte[size * bottomLength];
                Buffer.BlockCopy(second, size * (y + 2), parts[9], 0x0, size * bottomLength);

                //====================================================

                offsets[10] = 0x1E00;
                parts[10] = new byte[size * topLength];
                Buffer.BlockCopy(second, 0x600, parts[10], 0x0, size * topLength);

                offsets[11] = 0x1E00 + size * (y + 2);
                parts[11] = new byte[size * bottomLength];
                Buffer.BlockCopy(second, 0x600 + size * (y + 2), parts[11], 0x0, size * bottomLength);

                //====================================================

                offsets[12] = 0x2400;
                parts[12] = new byte[0xC00];
                Buffer.BlockCopy(second, 0xC00, parts[12], 0x0, 0xC00);

            }
            else
            {
                //====================================================

                offsets[0] = 0x0;
                parts[0] = new byte[0xC00];
                Buffer.BlockCopy(first, 0x0, parts[0], 0x0, 0xC00);

                //====================================================

                offsets[1] = 0xC00;
                parts[1] = new byte[size * topLength];
                Buffer.BlockCopy(first, 0xC00, parts[1], 0x0, size * topLength);

                offsets[2] = 0xC00 + size * (y + 2);
                parts[2] = new byte[size * bottomLength];
                Buffer.BlockCopy(first, 0xC00 + size * (y + 2), parts[2], 0x0, size * bottomLength);

                //====================================================

                offsets[3] = 0x1200;
                parts[3] = new byte[size * topLength];
                Buffer.BlockCopy(first, 0x1200, parts[3], 0x0, size * topLength);

                offsets[4] = 0x1200 + size * (y + 2);
                parts[4] = new byte[size * bottomLength];
                Buffer.BlockCopy(first, 0x1200 + size * (y + 2), parts[4], 0x0, size * bottomLength);

                //====================================================

                offsets[5] = 0x1800;
                parts[5] = new byte[size * topLength];
                Buffer.BlockCopy(second, 0, parts[5], 0x0, size * topLength);

                offsets[6] = 0x1800 + size * (y + 2);
                parts[6] = new byte[size * bottomLength];
                Buffer.BlockCopy(second, size * (y + 2), parts[6], 0x0, size * bottomLength);

                //====================================================

                offsets[7] = 0x1E00;
                parts[7] = new byte[size * topLength];
                Buffer.BlockCopy(second, 0x600, parts[7], 0x0, size * topLength);

                offsets[8] = 0x1E00 + size * (y + 2);
                parts[8] = new byte[size * bottomLength];
                Buffer.BlockCopy(second, 0x600 + size * (y + 2), parts[8], 0x0, size * bottomLength);

                //====================================================

                offsets[9] = 0x2400;
                parts[9] = new byte[size * topLength];
                Buffer.BlockCopy(second, 0xC00, parts[9], 0x0, size * topLength);

                offsets[10] = 0x2400 + size * (y + 2);
                parts[10] = new byte[size * bottomLength];
                Buffer.BlockCopy(second, 0xC00 + size * (y + 2), parts[10], 0x0, size * bottomLength);

                //====================================================

                offsets[11] = 0x2A00;
                parts[11] = new byte[size * topLength];
                Buffer.BlockCopy(second, 0x1200, parts[11], 0x0, size * topLength);

                offsets[12] = 0x2A00 + size * (y + 2);
                parts[12] = new byte[size * bottomLength];
                Buffer.BlockCopy(second, 0x1200 + size * (y + 2), parts[12], 0x0, size * bottomLength);

                //====================================================
            }

            for (int i = 0; i < parts.Length; i++)
            {
                Utilities.SendString(s, Utilities.Freeze((uint)(address + offsets[i]), parts[i]));
                Thread.Sleep(100);
                counter++;
            }
        }

        private void FreezeVillagerBtn_Click(object sender, EventArgs e)
        {
            villagerFlag = new byte[10][];
            villager = new byte[10][];
            haveVillager = new bool[10];

            for (int i = 0; i < 10; i++)
            {
                villager[i] = Utilities.GetVillager(s, null, i, 0x3);
                villagerFlag[i] = Utilities.GetMoveout(s, null, i, 0x33);
                haveVillager[i] = MapRegenerator.CheckHaveVillager(villager[i]);
                if (haveVillager[i])
                {
                    Utilities.SendString(s, Utilities.Freeze((uint)(Utilities.VillagerAddress + (i * Utilities.VillagerSize) + Utilities.VillagerMoveoutOffset), villagerFlag[i]));
                }
            }
            MapRegenerator.WriteVillager(villager, haveVillager);

            int freezeCount = Utilities.GetFreezeCount(s);

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Stay!";
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void UnfreezeVillagerBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                if (haveVillager[i])
                {
                    Utilities.SendString(s, Utilities.UnFreeze((uint)(Utilities.VillagerAddress + (i * Utilities.VillagerSize) + Utilities.VillagerMoveoutOffset)));
                }
            }

            int freezeCount = Utilities.GetFreezeCount(s);

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Go!";
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void FreezeAllVillagerBtn_Click(object sender, EventArgs e)
        {
            Thread FreezeAllVillagerThread = new(FreezeAllVillager);
            FreezeAllVillagerThread.Start();

        }

        private void FreezeAllVillager()
        {
            ShowMapWait(124, "Casting...");

            LockControl();

            int counter = 0;

            villagerFlag = new byte[10][];

            for (int i = 0; i < 10; i++)
            {
                // Set every one to irregular move out
                Utilities.SetMoveout(s, null, i, "2", "0");
                villagerFlag[i] = Utilities.GetMoveout(s, null, i, 0x33);
                if (i > 0) // Freeze all other 9 villagers' flag
                    Utilities.FreezeBig(s, (uint)(Utilities.VillagerAddress + (i * Utilities.VillagerSize) + Utilities.VillagerMoveoutOffset), villagerFlag[i], (uint)villagerFlag[i].Length);
            }


            // Freeze the first villager and his/her house
            byte[] VillagerData = Utilities.GetVillager(s, null, 0, (int)Utilities.VillagerSize, ref counter);

            int[] HouseList = new int[10];

            for (int i = 0; i < 10; i++)
            {
                byte b = Utilities.GetHouseOwner(s, null, i, ref counter);
                HouseList[i] = Convert.ToInt32(b);
            }

            Villager V = new(VillagerData, 0)
            {
                HouseIndex = Utilities.FindHouseIndex(0, HouseList)
            };


            byte[] HouseData = Utilities.GetHouse(s, null, V.HouseIndex, ref counter);

            byte[] head = new byte[0x2F83];
            byte[] tail = new byte[0xBB6];

            Buffer.BlockCopy(VillagerData, 0, head, 0, 0x2F83);
            Buffer.BlockCopy(VillagerData, (int)Utilities.VillagerMoveoutOffset, tail, 0, 0xBB6);

            Utilities.FreezeBig(s, Utilities.VillagerAddress, head, 0x2F83);
            Utilities.FreezeBig(s, Utilities.VillagerAddress + Utilities.VillagerMoveoutOffset, tail, 0xBB6);
            Utilities.FreezeBig(s, (uint)(Utilities.VillagerHouseAddress + (V.HouseIndex * (Utilities.VillagerHouseSize))), HouseData, Utilities.VillagerHouseSize);


            int freezeCount = Utilities.GetFreezeCount(s);

            HideMapWait();

            UnlockControl();

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = V.GetRealName() + " is in Purgatory! " + V.HouseIndex;
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void UnfreezeAllVillagerBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                if (i > 0)
                    Utilities.SendString(s, Utilities.UnFreeze((uint)(Utilities.VillagerAddress + (i * Utilities.VillagerSize) + Utilities.VillagerMoveoutOffset)));
            }

            Utilities.UnFreezeBig(s, Utilities.VillagerAddress, 0x2F83);
            Utilities.UnFreezeBig(s, Utilities.VillagerAddress + Utilities.VillagerMoveoutOffset, 0xBB6);

            int[] HouseList = new int[10];

            for (int i = 0; i < 10; i++)
            {
                byte b = Utilities.GetHouseOwner(s, null, i, ref counter);
                HouseList[i] = Convert.ToInt32(b);
            }

            int HouseIndex = Utilities.FindHouseIndex(0, HouseList);
            Utilities.UnFreezeBig(s, (uint)(Utilities.VillagerHouseAddress + (HouseIndex * (Utilities.VillagerHouseSize))), Utilities.VillagerHouseSize);

            int freezeCount = Utilities.GetFreezeCount(s);

            Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Limbo!";
                UpdateFreezeCountLabel(freezeCount);
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void UpdateMapLayer(byte[] OldLayer)
        {
            byte[] empty = { 0xFE, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] newLayer = new byte[Utilities.NewMapSize * 2];

            int newMapOffset = 0xC000;

            for (int i = 0; i < newLayer.Length; i += empty.Length)
            {
                Buffer.BlockCopy(empty, 0, newLayer, i, empty.Length);
            }

            if (OldLayer.Length == Utilities.OldMapSize)
            {
                Buffer.BlockCopy(OldLayer, 0, newLayer, newMapOffset, OldLayer.Length);
            }
            else if (OldLayer.Length == Utilities.OldMapSize * 2)
            {
                Buffer.BlockCopy(OldLayer, 0, newLayer, newMapOffset, (int)Utilities.OldMapSize);
                Buffer.BlockCopy(OldLayer, (int)Utilities.OldMapSize, newLayer, (int)(Utilities.NewMapSize + newMapOffset), (int)Utilities.OldMapSize);
            }


            SaveFileDialog file = new()
            {
                Filter = "New Horizons Fasil 3 (*.nhf3)|*.nhf3",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            string savepath;

            if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
            else
                savepath = config.AppSettings.Settings["LastSave"].Value;

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

            config.AppSettings.Settings["LastSave"].Value = path;
            config.Save(ConfigurationSaveMode.Minimal);

            File.WriteAllBytes(file.FileName, newLayer);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }
    }
}
