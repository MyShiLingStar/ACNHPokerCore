using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class MapRegenerator : Form
    {
        private readonly Socket s;
        private int counter;
        private bool loop;
        private bool wasLoading = true;
        private Thread RegenThread;
        private readonly Lock mapLock = new();
        private int delayTime = 50;
        private int pauseTime = 70;
        private readonly bool sound;

        private MiniMap MiniMap;
        private int anchorX = -1;
        private int anchorY = -1;
        private byte[] tempData;
        private string tempFilename;

        private byte[][] villagerFlag;
        private byte[][] villager;
        private bool[] haveVillager;
        private bool FormIsClosing;
        private readonly bool debugging;


        public Dodo dodoSetup;

        private string[] CurrentVisitorList;

        private CancellationTokenSource cts;

        public event CloseHandler CloseForm;
        public event UpdateTurnipPriceHandler UpdateTurnipPriceHandler;

        private readonly int LoadSize = 0x2000;
        private readonly int LoadSize2 = 0x1800;

        #region Form Load
        public MapRegenerator(Socket S, bool Sound, bool Debugging)
        {
            try
            {
                s = S;
                sound = Sound;
                debugging = Debugging;

                InitializeComponent();
                FinMsg.SelectionAlignment = HorizontalAlignment.Center;
                logName.Text = Utilities.VisitorLogFile;

                MyLog.LogEvent("Regen", "RegenForm Started Successfully");
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Regen", "Form Construct: " + ex.Message);
            }
        }
        #endregion

        #region Save
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

            byte[] save = Utilities.ReadByteArray(s, address, (int)Utilities.NewMapSize * 2, ref counter);

            File.WriteAllBytes(file.FileName, save);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();

            if (!FormIsClosing)
            {
                Invoke((MethodInvoker)delegate
                {
                    FinMsg.Visible = true;
                    FinMsg.Text = "Template Saved!";
                });
            }
        }
        #endregion

        #region Load
        private void LoadMapBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new()
                {
                    Filter = "New Horizons Fasil 3 (*.nhf3)|*.nhf3|New Horizons Fasil (*.nhf)|*.nhf|All files (*.*)|*.*",
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

                uint address;
                int NumOfPart;

                if (data.Length == Utilities.OldMapSize) // nhf (Old Single Layer)
                {
                    address = Utilities.mapZero + (Utilities.ExtendedMapOffset * Utilities.FloorItemByteSize);
                    NumOfPart = data.Length / LoadSize;
                }
                else if (data.Length == Utilities.NewMapSize) // nhf2 (New Single Layer)
                {
                    address = Utilities.mapZero;
                    NumOfPart = data.Length / LoadSize;
                }
                else if (data.Length == Utilities.NewMapSize * 2) // nhf3 (New Double Layerr)
                {
                    address = Utilities.mapZero;
                    NumOfPart = data.Length / LoadSize;
                }
                else
                {
                    MyMessageBox.Show("Invalid File Size!", "Your map file size is invalid!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                byte[][] b = new byte[NumOfPart][];

                for (int i = 0; i < NumOfPart; i++)
                {
                    b[i] = new byte[LoadSize];
                    Buffer.BlockCopy(data, i * LoadSize, b[i], 0x0, LoadSize);
                }

                Thread LoadThread = new(delegate () { LoadMapFloor(b, address, NumOfPart); });
                LoadThread.Start();

            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Regen", "Load: " + ex.Message);
            }
        }

        private void LoadMapFloor(byte[][] b, uint address, int NumOfPart)
        {
            ShowMapWait(NumOfPart, "Loading...");

            counter = 0;

            for (int i = 0; i < NumOfPart; i++)
            {
                Utilities.SendByteArray(s, address + (i * LoadSize), b[i], LoadSize, ref counter);
                Utilities.SendByteArray(s, address + (i * LoadSize) + Utilities.SaveFileBuffer, b[i], LoadSize, ref counter);
            }

            Thread.Sleep(3000);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();

            if (!FormIsClosing)
            {
                Invoke((MethodInvoker)delegate
                {
                    FinMsg.Visible = true;
                    FinMsg.Text = "Template Loaded!";
                });
            }
        }
        #endregion

        #region Regen 1
        private void StartRegen_Click(object sender, EventArgs e)
        {
            FinMsg.Text = "";
            delayTime = int.Parse(delay.Text);

            if (startRegen.Tag.ToString().Equals("Start"))
            {
                //updateVisitorName();

                OpenFileDialog file = new()
                {
                    Filter = "New Horizons Fasil 3 (*.nhf3)|*.nhf3|New Horizons Fasil (*.nhf)|*.nhf|All files (*.*)|*.*",
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

                loop = true;
                startRegen.Tag = "Stop";
                startRegen.BackColor = Color.Orange;
                startRegen.Text = "Stop Regen";
                saveMapBtn.Enabled = false;
                loadMapBtn.Enabled = false;
                backBtn.Enabled = false;
                startRegen2.Enabled = false;
                keepVillagerBox.Enabled = false;
                dodoSetupBtn.Enabled = false;

                byte[] data = File.ReadAllBytes(file.FileName);

                uint address;
                int NumOfPart;

                if (data.Length == Utilities.OldMapSize) // nhf (Old Single Layer)
                {
                    address = Utilities.mapZero + (Utilities.ExtendedMapOffset * Utilities.FloorItemByteSize);
                    NumOfPart = data.Length / LoadSize;
                }
                else if (data.Length == Utilities.NewMapSize) // nhf2 (New Single Layer)
                {
                    address = Utilities.mapZero;
                    NumOfPart = data.Length / LoadSize;
                }
                else if (data.Length == Utilities.NewMapSize * 2) // nhf3 (New Double Layerr)
                {
                    address = Utilities.mapZero;
                    NumOfPart = data.Length / LoadSize;
                }
                else
                {
                    MyMessageBox.Show("Invalid File Size!", "Your map file size is invalid!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                byte[][] b = new byte[NumOfPart][];

                for (int i = 0; i < NumOfPart; i++)
                {
                    b[i] = new byte[LoadSize];
                    Buffer.BlockCopy(data, i * LoadSize, b[i], 0x0, LoadSize);
                }

                string[] name = file.FileName.Split('\\');

                MyLog.LogEvent("Regen", "Regen1 Started: " + name[name.Length - 1]);

                string dodo = Controller.SetupDodo();
                MyLog.LogEvent("Regen", "Regen1 Dodo: " + dodo);

                cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                RegenThread = new Thread(delegate () { RegenMapFloor(b, address, name[name.Length - 1], NumOfPart, token); });
                RegenThread.Start();
            }
            else
            {
                MyLog.LogEvent("Regen", "Regen1 Stopped");
                cts.Cancel();
                WaitMessagebox.Text = "Stopping Regen...";
                loop = false;
                startRegen.Tag = "Start";
                startRegen.BackColor = Color.FromArgb(114, 137, 218);
                startRegen.Text = "Cast Regen";
                saveMapBtn.Enabled = true;
                loadMapBtn.Enabled = true;
                backBtn.Enabled = true;
                startRegen2.Enabled = true;
                keepVillagerBox.Enabled = true;
                dodoSetupBtn.Enabled = true;
            }
        }
        #endregion

        #region Regen 2
        private void StartRegen2_Click(object sender, EventArgs e)
        {
            FinMsg.Text = "";
            delayTime = int.Parse(delay.Text);

            if (startRegen2.Tag.ToString().Equals("Start"))
            {
                OpenFileDialog file = new()
                {
                    Filter = "New Horizons Fasil 3 (*.nhf3)|*.nhf3|New Horizons Fasil (*.nhf)|*.nhf|All files (*.*)|*.*",
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

                string[] name = file.FileName.Split('\\');
                tempFilename = name[name.Length - 1];

                uint address;
                int NumOfPart;

                if (data.Length == Utilities.OldMapSize) // nhf (Old Single Layer)
                {
                    address = Utilities.mapZero + (Utilities.ExtendedMapOffset * Utilities.FloorItemByteSize);
                    NumOfPart = data.Length / LoadSize2;
                }
                else if (data.Length == Utilities.NewMapSize) // nhf2 (New Single Layer)
                {
                    address = Utilities.mapZero;
                    NumOfPart = data.Length / LoadSize2;
                }
                else if (data.Length == Utilities.NewMapSize * 2) // nhf3 (New Double Layerr)
                {
                    address = Utilities.mapZero;
                    NumOfPart = data.Length / LoadSize2;
                }
                else
                {
                    MyMessageBox.Show("Invalid File Size!", "Your map file size is invalid!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult dialogResult = MyMessageBox.Show("Would you like to limit the \"ignore empty tiles\" area?" + "\n\n" +
                                                            "This would allow you to pick a 7 x 7 area which the regenerator would only ignore."
                                                            , "Choose an area ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    mapPanel.Visible = true;
                    logPanel.Visible = false;

                    tempData = data;

                    Width = 485;
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
                }
                else
                {
                    loop = true;
                    startRegen2.Tag = "Stop";
                    startRegen2.BackColor = Color.Orange;
                    startRegen2.Text = "Stop Moogle Regenja";
                    saveMapBtn.Enabled = false;
                    loadMapBtn.Enabled = false;
                    backBtn.Enabled = false;
                    startRegen.Enabled = false;
                    keepVillagerBox.Enabled = false;
                    dodoSetupBtn.Enabled = false;

                    byte[][] b = new byte[NumOfPart][];
                    bool[][] isEmpty = new bool[NumOfPart][];


                    for (int i = 0; i < NumOfPart; i++)
                    {
                        b[i] = new byte[LoadSize2];
                        Buffer.BlockCopy(data, i * LoadSize2, b[i], 0x0, LoadSize2);

                        isEmpty[i] = new bool[LoadSize2];
                        BuildEmptyTable(b[i], ref isEmpty[i]);
                    }

                    MyLog.LogEvent("Regen", "Regen2Normal Started: " + tempFilename);

                    string dodo = Controller.SetupDodo();
                    MyLog.LogEvent("Regen", "Regen2 Dodo: " + dodo);

                    cts = new CancellationTokenSource();
                    CancellationToken token = cts.Token;

                    RegenThread = new Thread(delegate () { RegenMapFloor2(b, address, isEmpty, tempFilename, NumOfPart, token); });
                    RegenThread.Start();
                }
            }
            else
            {
                MyLog.LogEvent("Regen", "Regen2 Stopped");
                cts.Cancel();
                WaitMessagebox.Text = "Stopping Regen...";
                loop = false;
                startRegen2.Tag = "Start";
                startRegen2.BackColor = Color.FromArgb(114, 137, 218);
                startRegen2.Text = "Cast Moogle Regenja";
                saveMapBtn.Enabled = true;
                loadMapBtn.Enabled = true;
                backBtn.Enabled = true;
                startRegen.Enabled = true;
                keepVillagerBox.Enabled = true;
                dodoSetupBtn.Enabled = true;
            }
        }
        #endregion

        #region Regen 2.5
        private void StartBtn_Click(object sender, EventArgs e)
        {
            mapPanel.Visible = false;
            MiniMap = null;
            Width = 250;

            uint address;
            int NumOfPart;
            int ShiftedAnchorX = anchorX;

            if (tempData.Length == Utilities.OldMapSize) // nhf (Old Single Layer)
            {
                address = Utilities.mapZero + (Utilities.ExtendedMapOffset * Utilities.FloorItemByteSize);
                NumOfPart = tempData.Length / LoadSize2;
                ShiftedAnchorX = anchorX + 16;
            }
            else if (tempData.Length == Utilities.NewMapSize) // nhf2 (New Single Layer)
            {
                address = Utilities.mapZero;
                NumOfPart = tempData.Length / LoadSize2;
            }
            else if (tempData.Length == Utilities.NewMapSize * 2) // nhf3 (New Double Layerr)
            {
                address = Utilities.mapZero;
                NumOfPart = tempData.Length / LoadSize2;
            }
            else
            {
                return;
            }

            loop = true;
            startRegen2.Tag = "Stop";
            startRegen2.BackColor = Color.Orange;
            startRegen2.Text = "Stop Moogle Regenja";
            saveMapBtn.Enabled = false;
            loadMapBtn.Enabled = false;
            backBtn.Enabled = false;
            startRegen.Enabled = false;
            dodoSetupBtn.Enabled = false;


            byte[][] b = new byte[NumOfPart][];
            bool[][] isEmpty = new bool[NumOfPart][];


            for (int i = 0; i < NumOfPart; i++)
            {
                b[i] = new byte[LoadSize2];
                Buffer.BlockCopy(tempData, i * LoadSize2, b[i], 0x0, LoadSize2);

                isEmpty[i] = new bool[LoadSize2];
                BuildEmptyTable(b[i], ref isEmpty[i], i * 2, i * 2 + 1, ShiftedAnchorX);
            }
            MyLog.LogEvent("Regen", "Regen2Limit Started: " + tempFilename);
            MyLog.LogEvent("Regen", "Regen2Limit Area: " + anchorX + " " + anchorY);

            string dodo = Controller.SetupDodo();
            MyLog.LogEvent("Regen", "Regen2 Dodo: " + dodo);

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            RegenThread = new Thread(delegate () { RegenMapFloor2(b, address, isEmpty, tempFilename, NumOfPart, token); });
            RegenThread.Start();
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
        #endregion

        private void RegenMapFloor(byte[][] b, uint address, string name, int NumOfPart, CancellationToken token)
        {
            string regenMsg = "Regenerating... " + name;

            ShowMapWait(NumOfPart, regenMsg);

            if (keepVillagerBox.Checked)
                PrepareVillager(s);

            int writeCount;
            int runCount = 0;
            int PauseCount = 0;

            Stopwatch stopWatch = new();
            stopWatch.Start();

            string newVisitor;
            string newVisitorIsland;
            TimeSpan ts;

            dodoSetup?.LockBtn();

            Utilities.SendBlankName(s);
            Teleport.OverworldState state;

            do
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    if (dodoSetup != null && dodoSetup.dodoSetupDone)
                    {
                        state = dodoSetup.DodoMonitor(token);
                        if (dodoSetup.CheckOnlineStatus())
                            dodoSetup.DisplayDodo(Controller.SetupDodo());
                    }
                    else
                    {
                        state = Teleport.GetOverworldState();
                        Controller.SetupDodo();
                    }

                    counter = 0;
                    writeCount = 0;

                    newVisitor = GetVisitorName();
                    newVisitorIsland = GetVisitorIslandName();

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (!newVisitor.Equals(string.Empty))
                    {
                        if (!FormIsClosing)
                        {
                            var visitor = newVisitor;
                            var island = newVisitorIsland;
                            Invoke((MethodInvoker)delegate
                            {
                                visitorNameBox.Text = visitor;
                                WaitMessagebox.Text = "Paused. " + visitor + " arriving!";
                                CreateLog(visitor, island, "In");
                                PauseTimeLabel.Visible = true;
                                PauseTimer.Start();
                            });
                        }

                        Thread.Sleep(70000);
                        Utilities.SendBlankName(s);
                        if (!FormIsClosing)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                PauseTimeLabel.Visible = false;
                                PauseTimer.Stop();
                                pauseTime = 70;
                                PauseTimeLabel.Text = pauseTime.ToString();
                                WaitMessagebox.Text = regenMsg;
                            });
                        }

                        wasLoading = true;
                        state = Teleport.GetOverworldState();
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (state == Teleport.OverworldState.Loading || state == Teleport.OverworldState.UserArriveLeavingOrTitleScreen)
                    {
                        Debug.Print("Loading Detected");
                        wasLoading = true;
                        PauseCount = 0;
                        Thread.Sleep(5000);
                    }
                    else if (state == Teleport.OverworldState.ItemDropping)
                    {
                        Debug.Print("Item Dropping");
                        PauseCount = 0;
                    }
                    else
                    {
                        if (wasLoading || PauseCount >= 30)
                        {
                            CurrentVisitorList = GetVisitorList(CurrentVisitorList);
                            wasLoading = false;
                            PauseCount = 0;

                            //--------------------------------------------------------------------------------------------------
                            for (int i = 0; i < NumOfPart; i++)
                            {
                                lock (mapLock)
                                {
                                    var c = Utilities.ReadByteArray(s, address + (i * LoadSize), LoadSize, ref counter);

                                    if (c != null)
                                    {
                                        if (SafeEquals(b[i], c))
                                        {
                                            //Debug.Print("Same " + i);
                                            Thread.Sleep(delayTime);
                                        }
                                        else
                                        {
                                            Debug.Print("Replace " + i);
                                            Utilities.SendByteArray(s, address + (i * LoadSize), b[i], LoadSize, ref writeCount);
                                            Thread.Sleep(500);
                                        }
                                    }
                                    else
                                    {
                                        Debug.Print("Null " + i);
                                        Thread.Sleep(10000);
                                    }
                                }
                                if (!loop)
                                    break;
                            }
                            //--------------------------------------------------------------------------------------------------
                        }
                        else
                        {
                            PauseCount++;
                            counter = PauseCount * 2;
                            Thread.Sleep(2000);
                        }
                    }

                    if (!FormIsClosing)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            ts = stopWatch.Elapsed;
                            timeLabel.Text = Utilities.PrecedingZeros(ts.Hours.ToString(), 2) + ":" + Utilities.PrecedingZeros(ts.Minutes.ToString(), 2) + ":" + Utilities.PrecedingZeros(ts.Seconds.ToString(), 2);
                            if (keepVillagerBox.Checked)
                            {
                                int index = runCount % 10;
                                CheckAndResetVillager(villagerFlag[index], haveVillager[index], index, ref writeCount);
                            }
                            runCount++;
                            if (PauseCount > 0)
                            {
                                WaitMessagebox.Text = "Regen idling...";
                            }
                            else
                            {
                                WaitMessagebox.Text = regenMsg;
                            }
                        });
                    }
                    Debug.Print("------ " + runCount + " " + PauseCount);
                }
                catch (Exception ex)
                {
                    if (!FormIsClosing)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            MyLog.LogEvent("Regen", "Regen1: " + ex.Message);
                            //DateTime localDate = DateTime.Now;
                            MyMessageBox.Show("Connection to the Switch has been lost.", "Yeeted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CreateLog("Connection to the Switch has been lost.");
                            Close();
                        });
                    }
                    break;
                }
            } while (loop);

            if (token.IsCancellationRequested)
            {
                MyLog.LogEvent("Regen", "Regen1: Cancelled");
                if (dodoSetup != null)
                    MyMessageBox.Show("Dodo Helper & Regen Aborted!\nPlease remember to exit the airport first if you want to restart!", "Airbag deployment!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (!FormIsClosing)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        stopWatch.Stop();
                        HideMapWait();
                        MyLog.LogEvent("Regen", "Regen1 Stopped");
                        loop = false;
                        startRegen.Tag = "Start";
                        startRegen.BackColor = Color.FromArgb(114, 137, 218);
                        startRegen.Text = "Cast Regen";
                        saveMapBtn.Enabled = true;
                        loadMapBtn.Enabled = true;
                        backBtn.Enabled = true;
                        startRegen2.Enabled = true;
                        keepVillagerBox.Enabled = true;
                        dodoSetupBtn.Enabled = true;
                    });
                }
            }

            stopWatch.Stop();

            HideMapWait();

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void RegenMapFloor2(byte[][] b, uint address, bool[][] isEmpty, string name, int NumOfPart, CancellationToken token)
        {
            string regenMsg = "Regenerating... " + name;

            ShowMapWait(NumOfPart, regenMsg);

            byte[][] u = new byte[NumOfPart][];

            for (int i = 0; i < NumOfPart; i++)
            {
                u[i] = new byte[LoadSize2];
                Buffer.BlockCopy(b[i], 0, u[i], 0x0, LoadSize2);
            }

            if (keepVillagerBox.Checked)
                PrepareVillager(s);

            int writeCount;
            int runCount = 0;
            int PauseCount = 0;

            Stopwatch stopWatch = new();
            stopWatch.Start();

            string newVisitor;
            string newVisitorIsland;
            TimeSpan ts;

            dodoSetup?.LockBtn();

            Utilities.SendBlankName(s);
            Teleport.OverworldState state;

            do
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    if (dodoSetup != null && dodoSetup.dodoSetupDone)
                    {
                        state = dodoSetup.DodoMonitor(token);
                        if (dodoSetup.CheckOnlineStatus())
                            dodoSetup.DisplayDodo(Controller.SetupDodo());
                    }
                    else
                    {
                        state = Teleport.GetOverworldState();
                        Controller.SetupDodo();
                    }

                    counter = 0;
                    writeCount = 0;

                    newVisitor = GetVisitorName();
                    newVisitorIsland = GetVisitorIslandName();

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (!newVisitor.Equals(string.Empty))
                    {
                        if (!FormIsClosing)
                        {
                            var visitor = newVisitor;
                            var island = newVisitorIsland;
                            Invoke((MethodInvoker)delegate
                            {
                                visitorNameBox.Text = visitor;
                                WaitMessagebox.Text = "Paused. " + visitor + " arriving!";
                                CreateLog(visitor, island, "In");
                                PauseTimeLabel.Visible = true;
                                PauseTimer.Start();
                            });
                        }

                        Thread.Sleep(70000);
                        Utilities.SendBlankName(s);

                        if (!FormIsClosing)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                PauseTimeLabel.Visible = false;
                                PauseTimer.Stop();
                                pauseTime = 70;
                                PauseTimeLabel.Text = pauseTime.ToString();
                                WaitMessagebox.Text = regenMsg;
                            });
                        }

                        wasLoading = true;
                        state = Teleport.GetOverworldState();
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (state == Teleport.OverworldState.Loading || state == Teleport.OverworldState.UserArriveLeavingOrTitleScreen)
                    {
                        Debug.Print("Loading Detected");
                        wasLoading = true;
                        PauseCount = 0;
                        Thread.Sleep(5000);
                    }
                    else if (state == Teleport.OverworldState.ItemDropping)
                    {
                        Debug.Print("Item Dropping");
                        PauseCount = 0;
                    }
                    else
                    {
                        if (wasLoading || PauseCount >= 30)
                        {
                            CurrentVisitorList = GetVisitorList(CurrentVisitorList);
                            wasLoading = false;
                            PauseCount = 0;

                            //--------------------------------------------------------------------------------------------------
                            for (int i = 0; i < NumOfPart; i++)
                            {
                                if (token.IsCancellationRequested)
                                {
                                    break;
                                }

                                lock (mapLock)
                                {
                                    var c = Utilities.ReadByteArray(s, address + (i * LoadSize2), LoadSize2, ref counter);

                                    if (c != null)
                                    {
                                        if (Difference(b[i], ref u[i], isEmpty[i], c))
                                        {
                                            //Debug.Print("Same " + i);
                                            Thread.Sleep(delayTime);
                                        }
                                        else
                                        {
                                            Debug.Print("Replace " + i);
                                            Utilities.SendByteArray(s, address + (i * LoadSize2), u[i], LoadSize2, ref writeCount);
                                            Thread.Sleep(500);
                                        }
                                    }
                                    else
                                    {
                                        Debug.Print("Null " + i);
                                        Thread.Sleep(10000);
                                    }
                                }
                                if (!loop)
                                    break;
                            }
                            //--------------------------------------------------------------------------------------------------
                        }
                        else
                        {
                            PauseCount++;
                            counter = PauseCount * 2;
                            Thread.Sleep(2000);
                        }
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (!FormIsClosing)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            ts = stopWatch.Elapsed;
                            timeLabel.Text = Utilities.PrecedingZeros(ts.Hours.ToString(), 2) + ":" + Utilities.PrecedingZeros(ts.Minutes.ToString(), 2) + ":" + Utilities.PrecedingZeros(ts.Seconds.ToString(), 2);
                            if (keepVillagerBox.Checked)
                            {
                                int index = runCount % 10;
                                CheckAndResetVillager(villagerFlag[index], haveVillager[index], index, ref writeCount);
                            }
                            runCount++;
                            if (PauseCount > 0)
                            {
                                WaitMessagebox.Text = "Regen idling...";
                            }
                            else
                            {
                                WaitMessagebox.Text = regenMsg;
                            }
                        });
                    }
                    Debug.Print("------ " + runCount + " " + PauseCount);
                }
                catch (Exception ex)
                {
                    if (!FormIsClosing)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            MyLog.LogEvent("Regen", "Regen2: " + ex.Message);
                            //DateTime localDate = DateTime.Now;
                            MyMessageBox.Show("Connection to the Switch has been lost.", "Yeeted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CreateLog("Connection to the Switch has been lost.");
                            Close();
                        });
                    }
                    break;
                }
            } while (loop);

            if (token.IsCancellationRequested)
            {
                MyLog.LogEvent("Regen", "Regen2: Cancelled");
                if (dodoSetup != null)
                    MyMessageBox.Show("Dodo Helper & Regen Aborted!\nPlease remember to exit the airport first if you want to restart!", "Slamming on the brakes?", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (!FormIsClosing)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        stopWatch.Stop();
                        HideMapWait();
                        MyLog.LogEvent("Regen", "Regen2 Stopped");
                        loop = false;
                        startRegen2.Tag = "Start";
                        startRegen2.BackColor = Color.FromArgb(114, 137, 218);
                        startRegen2.Text = "Cast Moogle Regenja";
                        saveMapBtn.Enabled = true;
                        loadMapBtn.Enabled = true;
                        backBtn.Enabled = true;
                        startRegen.Enabled = true;
                        keepVillagerBox.Enabled = true;
                        dodoSetupBtn.Enabled = true;
                    });
                }
            }

            stopWatch.Stop();

            HideMapWait();

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        #region Form functions/controls
        private static bool SafeEquals(byte[] strA, byte[] strB)
        {
            int length = strA.Length;
            if (length != strB.Length)
            {
                return false;
            }
            for (int i = 0; i < length; i++)
            {
                if (strA[i] != strB[i]) return false;
            }
            return true;
        }

        private void ShowMapWait(int size, string msg = "")
        {
            if (!FormIsClosing)
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
        }

        private void HideMapWait()
        {
            if (!FormIsClosing)
            {
                Invoke((MethodInvoker)delegate
                {
                    PleaseWaitPanel.Visible = false;
                    ProgressTimer.Stop();
                });
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (!FormIsClosing)
            {
                Invoke((MethodInvoker)delegate
                {
                    if (counter <= MapProgressBar.Maximum)
                        MapProgressBar.Value = counter;
                    else
                        MapProgressBar.Value = MapProgressBar.Maximum;
                });
            }
        }

        private void MapRegenerator_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormIsClosing = true;
            CloseCleaning();
        }

        private void CloseCleaning()
        {
            MyLog.LogEvent("Regen", "Form Closed");
            if (RegenThread != null)
            {
                MyLog.LogEvent("Regen", "Regen Force Closed");
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                cts.Cancel();
            }
            if (dodoSetup != null)
            {
                //controller.detachController();
                dodoSetup.Close();
                dodoSetup = null;
            }

            CloseForm?.Invoke();
        }

        private void HideBtn_Click(object sender, EventArgs e)
        {
            if (dodoSetup != null)
            {
                dodoSetup.Hide();
                dodoSetup.BringToFront();
            }
            ShowInTaskbar = false;
            Hide();
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            if (RegenThread != null)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                cts.Cancel();
                if (dodoSetup != null)
                {
                    dodoSetup.Close();
                    dodoSetup = null;
                }
            }
            Close();
        }

        private static void BuildEmptyTable(byte[] org, ref bool[] table)
        {
            byte[] Part1 = new byte[0xC00];
            byte[] Part2 = new byte[0xC00];

            Buffer.BlockCopy(org, 0, Part1, 0, 0xC00);
            Buffer.BlockCopy(org, 0xc00, Part2, 0, 0xC00);

            byte[] blockLeft = new byte[16];
            byte[] blockRight = new byte[16];

            for (int i = 0; i < 96; i++)
            {
                Buffer.BlockCopy(Part1, i * 16, blockLeft, 0, 16);
                Buffer.BlockCopy(Part1, (i + 96) * 16, blockRight, 0, 16);

                if ((Utilities.ByteToHexString(blockLeft)).Equals("FEFF000000000000FEFF000000000000") && (Utilities.ByteToHexString(blockRight)).Equals("FEFF000000000000FEFF000000000000"))
                {
                    for (int j = 0; j < 16; j++)
                    {
                        table[i * 16 + j] = true;
                        table[i * 16 + 96 * 16 + j] = true;
                    }
                }
                else
                {
                    for (int j = 0; j < 16; j++)
                    {
                        table[i * 16 + j] = false;
                        table[i * 16 + 96 * 16 + j] = false;
                    }
                }
            }

            for (int i = 0; i < 96; i++)
            {
                Buffer.BlockCopy(Part2, i * 16, blockLeft, 0, 16);
                Buffer.BlockCopy(Part2, (i + 96) * 16, blockRight, 0, 16);

                if ((Utilities.ByteToHexString(blockLeft)).Equals("FEFF000000000000FEFF000000000000") && (Utilities.ByteToHexString(blockRight)).Equals("FEFF000000000000FEFF000000000000"))
                {
                    for (int j = 0; j < 16; j++)
                    {
                        table[i * 16 + j + 1536 * 2] = true;
                        table[i * 16 + 96 * 16 + j + 1536 * 2] = true;
                    }
                }
                else
                {
                    for (int j = 0; j < 16; j++)
                    {
                        table[i * 16 + j + 1536 * 2] = false;
                        table[i * 16 + 96 * 16 + j + 1536 * 2] = false;
                    }
                }
            }
        }

        private void BuildEmptyTable(byte[] org, ref bool[] table, int Part1x, int Part2x, int ShiftedAnchorX)
        {
            byte[] Part1 = new byte[0xC00];
            byte[] Part2 = new byte[0xC00];

            Buffer.BlockCopy(org, 0, Part1, 0, 0xC00);
            Buffer.BlockCopy(org, 0xc00, Part2, 0, 0xC00);

            byte[] blockLeft = new byte[16];
            byte[] blockRight = new byte[16];

            for (int i = 0; i < 96; i++)
            {
                Buffer.BlockCopy(Part1, i * 16, blockLeft, 0, 16);
                Buffer.BlockCopy(Part1, (i + 96) * 16, blockRight, 0, 16);

                if ((Math.Abs(ShiftedAnchorX - Part1x) <= 3) && (Math.Abs(anchorY - i) <= 3) && (Utilities.ByteToHexString(blockLeft)).Equals("FEFF000000000000FEFF000000000000") && (Utilities.ByteToHexString(blockRight)).Equals("FEFF000000000000FEFF000000000000"))
                {
                    for (int j = 0; j < 16; j++)
                    {
                        table[i * 16 + j] = true;
                        table[i * 16 + 96 * 16 + j] = true;
                    }
                }
                else
                {
                    for (int j = 0; j < 16; j++)
                    {
                        table[i * 16 + j] = false;
                        table[i * 16 + 96 * 16 + j] = false;
                    }
                }
            }

            for (int i = 0; i < 96; i++)
            {
                Buffer.BlockCopy(Part2, i * 16, blockLeft, 0, 16);
                Buffer.BlockCopy(Part2, (i + 96) * 16, blockRight, 0, 16);

                if ((Math.Abs(ShiftedAnchorX - Part2x) <= 3) && (Math.Abs(anchorY - i) <= 3) && (Utilities.ByteToHexString(blockLeft)).Equals("FEFF000000000000FEFF000000000000") && (Utilities.ByteToHexString(blockRight)).Equals("FEFF000000000000FEFF000000000000"))
                {
                    for (int j = 0; j < 16; j++)
                    {
                        table[i * 16 + j + 1536 * 2] = true;
                        table[i * 16 + 96 * 16 + j + 1536 * 2] = true;
                    }
                }
                else
                {
                    for (int j = 0; j < 16; j++)
                    {
                        table[i * 16 + j + 1536 * 2] = false;
                        table[i * 16 + 96 * 16 + j + 1536 * 2] = false;
                    }
                }
            }
        }

        private static bool Difference(byte[] org, ref byte[] upd, bool[] isEmpty, byte[] cur)
        {
            bool output = true;
            bool output2 = true;
            bool pass = true;
            int NotEqualNum = 0;
            for (int i = 0; i < cur.Length; i++)
            {
                if (cur[i] != org[i])
                {
                    if (isEmpty[i])
                    {
                        if (output)
                        {
                            //Debug.Print("Empty Space Changed");
                            output = false;
                        }
                        upd[i] = cur[i];
                    }
                    else
                    {
                        pass = false;
                    }
                    NotEqualNum++;
                }
                else
                {
                    if (upd[i] != cur[i])
                    {
                        if (output2)
                        {
                            //Debug.Print("Back to Normal");
                            output2 = false;
                        }
                        upd[i] = cur[i];
                    }
                }
            }
            if (NotEqualNum > 0x140)
            {
                Debug.Print("Large byte different");
                MyLog.LogEvent("Regen", "[Warning] Shifted? " + NotEqualNum + " bytes different.");
            }
            return pass;
        }

        private void Delay_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!((c >= '0' && c <= '9')))
            {
                e.Handled = true;
            }
        }

        private string GetVisitorName()
        {
            byte[] b = Utilities.GetVisitorName(s);
            if (b == null)
            {
                return string.Empty;
            }
            //Debug.Print("Byte :   " +Utilities.ByteToHexString(b));
            string tempName = Encoding.Unicode.GetString(b, 0, 20);
            return tempName.Replace("\0", string.Empty);
        }

        private string GetVisitorIslandName()
        {
            byte[] b = Utilities.GetVisitorIslandName(s);
            if (b == null)
            {
                return string.Empty;
            }
            //Debug.Print("Byte :   " +Utilities.ByteToHexString(b));
            string tempName = Encoding.Unicode.GetString(b, 0, 20);
            return tempName.Replace("\0", string.Empty);
        }

        private void PauseTimer_Tick(object sender, EventArgs e)
        {
            if (!FormIsClosing)
            {
                Invoke((MethodInvoker)delegate
                {
                    pauseTime--;
                    PauseTimeLabel.Text = pauseTime.ToString();
                });
            }
        }

        private void LogBtn_Click(object sender, EventArgs e)
        {
            if (logGridView.DataSource == null)
            {
                if (!File.Exists(Utilities.VisitorLogPath))
                {
                    string logheader = "Timestamp" + "," + "Name" + "," + "Island" + "," + "I/O";

                    using StreamWriter sw = File.CreateText(Utilities.VisitorLogPath);
                    sw.WriteLine(logheader);
                }
                try
                {
                    logGridView.DataSource = LoadCSV(Utilities.VisitorLogPath);
                    logGridView.Columns["Timestamp"].Width = 140;
                    logGridView.Columns["Name"].Width = 75;
                    logGridView.Columns["Island"].Width = 75;
                    logGridView.Columns["I/O"].Width = 40;
                    logGridView.Sort(logGridView.Columns[0], ListSortDirection.Descending);
                    logPanel.Visible = true;
                }
                catch
                {
                    File.Move(Utilities.VisitorLogPath, Utilities.saveFolder + @"OldVisitorLog.csv");

                    string logheader = "Timestamp" + "," + "Name" + "," + "Island" + "," + "I/O";

                    using (StreamWriter sw = File.CreateText(Utilities.VisitorLogPath))
                    {
                        sw.WriteLine(logheader);
                    }

                    MyMessageBox.Show("An error was encountered while loading the visitor log file.\n\n" +
                                      "But don't worry! A new visitor log file have been created.\n" +
                                      "Your existing visitor log file has been renamed to \"OldVisitorLog.csv\".", "Error loading existing visitor log file!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    logGridView.DataSource = LoadCSV(Utilities.VisitorLogPath);
                    logGridView.Columns["Timestamp"].Width = 140;
                    logGridView.Columns["Name"].Width = 75;
                    logGridView.Columns["Island"].Width = 75;
                    logGridView.Columns["I/O"].Width = 40;
                    logGridView.Sort(logGridView.Columns[0], ListSortDirection.Descending);
                    logPanel.Visible = true;
                }
            }
            if (Width < 610)
            {
                Width = 610;
                logPanel.Visible = true;
            }
            else
            {
                Width = 250;
                logPanel.Visible = false;
            }
        }
        #endregion

        #region Log
        private static DataTable LoadCSV(string filePath)
        {
            var dt = new DataTable();

            File.ReadLines(filePath).Take(1)
                .SelectMany(x => x.Split([","], StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            dt.Columns["Timestamp"].DataType = typeof(DateTime);

            File.ReadLines(filePath).Skip(1)
                .Select(x => x.Split([","], StringSplitOptions.None))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));

            return dt;
        }

        private void CreateLog(string newVisitor, string newVisitorIsland = "", string state = "")
        {
            if (!File.Exists(Utilities.VisitorLogPath))
            {
                string logheader = "Timestamp" + "," + "Name" + "," + "Island" + "," + "I/O";

                using StreamWriter sw = File.CreateText(Utilities.VisitorLogPath);
                sw.WriteLine(logheader);
            }

            DateTime localDate = DateTime.Now;
            string newLog = localDate + "," + newVisitor + "," + newVisitorIsland + "," + state;

            using (StreamWriter sw = File.AppendText(Utilities.VisitorLogPath))
            {
                sw.WriteLine(newLog);
            }

            if (logGridView.DataSource != null)
            {
                logGridView.DataSource = LoadCSV(Utilities.VisitorLogPath);
                logGridView.Sort(logGridView.Columns[0], ListSortDirection.Descending);
            }
        }

        private void NewLogBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog file = new()
            {
                Filter = "Comma-Separated Values file (*.csv)|*.csv",
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

            string logheader = "Timestamp" + "," + "Name" + "," + "Island" + "," + "I/O";

            using (StreamWriter sw = File.CreateText(file.FileName))
            {
                sw.WriteLine(logheader);
            }

            string[] s = file.FileName.Split('\\');

            logName.Text = s[s.Length - 1];
            Utilities.VisitorLogPath = file.FileName;

            if (logGridView.DataSource != null)
            {
                logGridView.DataSource = LoadCSV(Utilities.VisitorLogPath);
                logGridView.Sort(logGridView.Columns[0], ListSortDirection.Descending);
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void SelectLogBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new()
            {
                Filter = "Comma-Separated Values file (*.csv)|*.csv",
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

            string[] s = file.FileName.Split('\\');

            logName.Text = s[s.Length - 1];
            Utilities.VisitorLogPath = file.FileName;

            if (logGridView.DataSource != null)
            {
                logGridView.DataSource = LoadCSV(Utilities.VisitorLogPath);
                logGridView.Sort(logGridView.Columns[0], ListSortDirection.Descending);
            }
        }

        #endregion

        #region Villager

        public void PrepareVillager(Socket s)
        {
            villagerFlag = new byte[10][];
            villager = new byte[10][];
            haveVillager = new bool[10];

            for (int i = 0; i < 10; i++)
            {
                villager[i] = Utilities.GetVillager(s, null, i, 0x3);
                villagerFlag[i] = Utilities.GetMoveout(s, null, i, 0x33);
                haveVillager[i] = CheckHaveVillager(villager[i]);
            }
            WriteVillager(villager, haveVillager);
        }

        public void UpdateVillager(Socket s, int index)
        {
            if (villager == null)
                PrepareVillager(s);
            else
            {
                villager[index] = Utilities.GetVillager(s, null, index, 0x3);
                villagerFlag[index] = Utilities.GetMoveout(s, null, index, 0x33);
                haveVillager[index] = true;

                WriteVillager(villager, haveVillager);
            }
        }

        public static bool CheckHaveVillager(byte[] villager)
        {
            if (villager[0] >= 0x23)
                return false;
            else
                return true;
        }

        public static void WriteVillager(byte[][] villager, bool[] haveVillager)
        {
            string villagerStr = " | ";
            for (int i = 0; i < 10; i++)
            {
                if (haveVillager[i])
                    villagerStr += Utilities.GetVillagerRealName(villager[i][0], villager[i][1]) + " | ";
            }

            using StreamWriter sw = File.CreateText(Utilities.CurrentVillagerPath);
            sw.WriteLine(villagerStr);
        }

        private void CheckAndResetVillager(byte[] villagerFlag, bool haveVillager, int index, ref int counter)
        {
            if (!haveVillager)
            {
            }
            else
            {
                string ByteString = Utilities.ByteToHexString(Utilities.GetMoveout(s, null, index, 0x33, ref counter));
                if (!ByteString.Equals(Utilities.ByteToHexString(villagerFlag)))
                {
                    Utilities.SetMoveout(s, null, index, villagerFlag, ref counter);
                    Debug.Print("Reset Villager " + index);
                    MyLog.LogEvent("Regen", "Villager Reset : " + index);
                }
                //else
                //Debug.Print("No Reset Needed " + index);
            }
        }
        #endregion

        #region Visitor

        private string[] GetVisitorList(string[] oldVisitorList)
        {
            string[] newVisitorList = new string[8];
            int num = 0;

            using StreamWriter sw = File.CreateText(Utilities.CurrentVisitorPath);
            for (int i = 0; i < 8; i++)
            {
                if (i == 0)
                {
                    newVisitorList[i] = string.Empty;
                    continue;
                }
                else
                    newVisitorList[i] = Utilities.GetVisitorNameFromList(s, null, i);

                if (newVisitorList[i].Equals(string.Empty))
                    sw.WriteLine("[Empty]");
                else
                {
                    sw.WriteLine(newVisitorList[i]);
                    num++;
                }
            }
            sw.WriteLine("Num of Visitor : " + num);

            string[] Leaver = new string[8];
            bool StillHere;

            if (oldVisitorList != null)
            {
                for (int i = 0; i < oldVisitorList.Length; i++)
                {
                    StillHere = false;

                    for (int j = 0; j < newVisitorList.Length; j++)
                    {
                        if (oldVisitorList[i] == newVisitorList[j])
                        {
                            StillHere = true;
                            break;
                        }
                    }

                    if (StillHere)
                    {
                        Leaver[i] = string.Empty;
                    }
                    else
                    {
                        Leaver[i] = oldVisitorList[i];
                    }
                }

                foreach (string visitor in Leaver)
                {
                    if (visitor != string.Empty && visitor != null)
                    {
                        if (!FormIsClosing)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                CreateLog(visitor, "", "Out");
                            });
                        }
                    }
                }
            }

            return newVisitorList;

            /*if (num >= 7)
            {
                sw.WriteLine(" [Island Full] ");
            }*/
            //Debug.Print("Visitor Update");
        }

        #endregion

        #region Dodo Helper
        private void DodoHelperBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn.Tag.ToString().Equals("Enable"))
            {
                btn.Text = "Disable Dodo Helper";
                btn.Tag = "Disable";
                btn.BackColor = Color.Orange;

                dodoSetup = new Dodo(s, false, debugging);
                dodoSetup.CloseForm += DodoSetup_closeForm;
                dodoSetup.AbortAll += DodoSetup_abortAll;
                dodoSetup.UpdateTurnipPriceHandler += DodoSetup_updateTurnipPriceHandler;
                dodoSetup.Show(this);
                dodoSetup.Location = new Point(Location.X - 590, Location.Y);
                dodoSetup.ControlBox = false;
                dodoSetup.WriteLog("[Dodo Helper Ready! Waiting for Regen.]\n\n" +
                    "1. Disconnect all controller by selecting \"Controllers\" > \"Change Grip/Order\"\n" +
                    "2. Leave only the Joy-Con docked on your Switch.\n" +
                    "3. Return to the game and dock your Switch if needed. Try pressing the buttons below to test the virtual controller.\n" +
                    "4. If the virtual controller does not response, try the \"Detach\" button first, then the \"A\" button.\n" +
                    "5. If the virtual controller still does not appear, try restart your Switch.\n\n" +
                    ">> Please try the buttons below to test the virtual controller. <<"
                    );
            }
            else
            {
                btn.Text = "Enable Dodo Helper";
                btn.Tag = "Enable";
                btn.BackColor = Color.FromArgb(114, 137, 218);
                dodoSetup.Close();
                dodoSetup = null;
            }
        }

        private void DodoSetup_updateTurnipPriceHandler()
        {
            UpdateTurnipPriceHandler?.Invoke();
        }

        private void DodoSetup_abortAll()
        {
            Abort();
        }

        private void DodoSetup_closeForm()
        {
            dodoSetupBtn.Text = "Enable Dodo Helper";
            dodoSetupBtn.Tag = "Enable";
            dodoSetupBtn.BackColor = Color.FromArgb(114, 137, 218);
            dodoSetup = null;
        }

        private void MapRegenerator_Move(object sender, EventArgs e)
        {
            if (dodoSetup != null)
            {
                dodoSetup.Location = new Point(Location.X - 590, Location.Y);
                dodoSetup.BringToFront();
            }
        }

        private void ChangeDodoBtn_Click(object sender, EventArgs e)
        {
            string newPath = Controller.ChangeDodoPath();

            dodoSetup?.WriteLog(">> Dodo path change to : " + newPath);
        }

        public void Abort()
        {
            cts.Cancel();
        }

        #endregion

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            dodoSetup?.Show();
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            Show();
        }
    }
}
