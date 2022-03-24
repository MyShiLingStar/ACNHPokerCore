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
        private static Socket s;
        private int counter = 0;
        private Boolean loop = false;
        private Boolean wasLoading = true;
        private Thread RegenThread = null;
        private static Object mapLock = new Object();
        private int delayTime = 50;
        private int pauseTime = 70;
        private bool sound;

        private miniMap MiniMap = null;
        private int anchorX = -1;
        private int anchorY = -1;
        private byte[] tempData;
        private string tempFilename;

        private static byte[][] villagerFlag;
        private static byte[][] villager;
        private static Boolean[] haveVillager;

        public dodo dodoSetup = null;

        private CancellationTokenSource cts;

        public event CloseHandler closeForm;

        #region Form Load
        public MapRegenerator(Socket S, bool Sound)
        {
            try
            {
                s = S;
                sound = Sound;

                InitializeComponent();
                FinMsg.SelectionAlignment = HorizontalAlignment.Center;
                logName.Text = Utilities.VisitorLogFile;
                Random random = new Random();
                int v = random.Next(8192);
                Debug.Print(v.ToString());

                MyLog.logEvent("Regen", "RegenForm Started Successfully");
            }
            catch (Exception ex)
            {
                MyLog.logEvent("Regen", "Form Construct: " + ex.Message.ToString());
            }
        }
        #endregion

        #region Save
        private void saveMapBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog file = new SaveFileDialog()
                {
                    Filter = "New Horizons Fasil (*.nhf)|*.nhf",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                string savepath;

                if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + @"\save";
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

                UInt32 address = Utilities.mapZero;

                Thread LoadThread = new Thread(delegate () { saveMapFloor(address, file); });
                LoadThread.Start();

            }
            catch (Exception ex)
            {
                MyLog.logEvent("Regen", "Save: " + ex.Message.ToString());
                return;
            }
        }

        private void saveMapFloor(UInt32 address, SaveFileDialog file)
        {
            showMapWait(42, "Saving...");

            byte[] save = Utilities.ReadByteArray8(s, address, 0x54000, ref counter);

            File.WriteAllBytes(file.FileName, save);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            hideMapWait();

            this.Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Template Saved!";
            });
        }
        #endregion

        #region Load
        private void loadMapBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog()
                {
                    Filter = "New Horizons Fasil (*.nhf)|*.nhf|All files (*.*)|*.*",
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

                UInt32 address = Utilities.mapZero;

                byte[][] b = new byte[42][];

                for (int i = 0; i < 42; i++)
                {
                    b[i] = new byte[0x2000];
                    Buffer.BlockCopy(data, i * 0x2000, b[i], 0x0, 0x2000);
                }

                Thread LoadThread = new Thread(delegate () { loadMapFloor(b, address); });
                LoadThread.Start();
            }
            catch (Exception ex)
            {
                MyLog.logEvent("Regen", "Load: " + ex.Message.ToString());
                return;
            }
        }

        private void loadMapFloor(byte[][] b, UInt32 address)
        {
            showMapWait(42 * 2, "Loading...");

            for (int i = 0; i < 42; i++)
            {
                Utilities.SendByteArray8(s, address + (i * 0x2000), b[i], 0x2000, ref counter);
                Utilities.SendByteArray8(s, address + (i * 0x2000) + Utilities.mapOffset, b[i], 0x2000, ref counter);
            }

            Thread.Sleep(3000);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            hideMapWait();

            this.Invoke((MethodInvoker)delegate
            {
                FinMsg.Visible = true;
                FinMsg.Text = "Template Loaded!";
            });
        }
        #endregion

        #region Regen 1
        private void startRegen_Click(object sender, EventArgs e)
        {
            FinMsg.Text = "";
            delayTime = int.Parse(delay.Text);

            if (startRegen.Tag.ToString().Equals("Start"))
            {
                //updateVisitorName();

                OpenFileDialog file = new OpenFileDialog()
                {
                    Filter = "New Horizons Fasil (*.nhf)|*.nhf|All files (*.*)|*.*",
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

                UInt32 address = Utilities.mapZero;

                byte[][] b = new byte[42][];

                for (int i = 0; i < 42; i++)
                {
                    b[i] = new byte[0x2000];
                    Buffer.BlockCopy(data, i * 0x2000, b[i], 0x0, 0x2000);
                }

                string[] name = file.FileName.Split('\\');

                MyLog.logEvent("Regen", "Regen1 Started: " + name[name.Length - 1]);

                string dodo = controller.setupDodo();
                MyLog.logEvent("Regen", "Regen1 Dodo: " + dodo);

                cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                RegenThread = new Thread(delegate () { regenMapFloor(b, address, name[name.Length - 1], token); });
                RegenThread.Start();
            }
            else
            {
                MyLog.logEvent("Regen", "Regen1 Stopped");
                loop = false;
                startRegen.Tag = "Start";
                startRegen.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
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
        private void startRegen2_Click(object sender, EventArgs e)
        {
            FinMsg.Text = "";
            delayTime = int.Parse(delay.Text);

            if (startRegen2.Tag.ToString().Equals("Start"))
            {
                OpenFileDialog file = new OpenFileDialog()
                {
                    Filter = "New Horizons Fasil (*.nhf)|*.nhf|All files (*.*)|*.*",
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

                string[] name = file.FileName.Split('\\');
                tempFilename = name[name.Length - 1];

                UInt32 address = Utilities.mapZero;

                DialogResult dialogResult = MyMessageBox.Show("Would you like to limit the \"ignore empty tiles\" area?" + "\n\n" +
                                                            "This would allow you to pick a 7 x 7 area which the regenerator would only ignore."
                                                            , "Choose an area ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    mapPanel.Visible = true;
                    logPanel.Visible = false;

                    tempData = data;

                    this.Width = 485;
                    if (MiniMap == null)
                    {
                        byte[] Acre = Utilities.getAcre(s, null);
                        byte[] Building = Utilities.getBuilding(s, null);
                        Byte[] Terrain = Utilities.getTerrain(s, null);

                        if (MiniMap == null)
                            MiniMap = new miniMap(data, Acre, Building, Terrain);

                        miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawBackground(), MiniMap.drawItemMap());
                    }
                    else
                        return;
                    try
                    {
                        byte[] Coordinate = Utilities.getCoordinate(s, null);
                        int x = BitConverter.ToInt32(Coordinate, 0);
                        int y = BitConverter.ToInt32(Coordinate, 4);

                        anchorX = x - 0x24;
                        anchorY = y - 0x18;

                        if (anchorX < 3 || anchorY < 3 || anchorX > 108 || anchorY > 92)
                        {
                            anchorX = 3;
                            anchorY = 3;
                        }
                        xCoordinate.Text = anchorX.ToString();
                        yCoordinate.Text = anchorY.ToString();
                        miniMapBox.Image = MiniMap.drawSelectSquare(anchorX, anchorY);
                    }
                    catch (Exception ex)
                    {
                        MyLog.logEvent("Regen", "getCoordinate: " + ex.Message.ToString());
                        MyMessageBox.Show("Something doesn't feel right at all. You should restart the program...\n\n" + ex.Message.ToString(), "!!! THIS SHIT DOESN'T WORK!! WHY? HAS I EVER?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    byte[][] b = new byte[56][];
                    bool[][] isEmpty = new bool[56][];


                    for (int i = 0; i < 56; i++)
                    {
                        b[i] = new byte[0x1800];
                        Buffer.BlockCopy(data, i * 0x1800, b[i], 0x0, 0x1800);

                        isEmpty[i] = new bool[0x1800];
                        buildEmptyTable(b[i], ref isEmpty[i]);
                    }

                    MyLog.logEvent("Regen", "Regen2Normal Started: " + tempFilename);

                    string dodo = controller.setupDodo();
                    MyLog.logEvent("Regen", "Regen2 Dodo: " + dodo);

                    cts = new CancellationTokenSource();
                    CancellationToken token = cts.Token;

                    RegenThread = new Thread(delegate () { regenMapFloor2(b, address, isEmpty, tempFilename, token); });
                    RegenThread.Start();
                }
            }
            else
            {
                MyLog.logEvent("Regen", "Regen2 Stopped");
                WaitMessagebox.Text = "Stopping Regen...";
                loop = false;
                startRegen2.Tag = "Start";
                startRegen2.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
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
        private void startBtn_Click(object sender, EventArgs e)
        {
            mapPanel.Visible = false;
            MiniMap = null;
            this.Width = 250;

            UInt32 address = Utilities.mapZero;

            loop = true;
            startRegen2.Tag = "Stop";
            startRegen2.BackColor = Color.Orange;
            startRegen2.Text = "Stop Moogle Regenja";
            saveMapBtn.Enabled = false;
            loadMapBtn.Enabled = false;
            backBtn.Enabled = false;
            startRegen.Enabled = false;
            dodoSetupBtn.Enabled = false;


            byte[][] b = new byte[56][];
            bool[][] isEmpty = new bool[56][];


            for (int i = 0; i < 56; i++)
            {
                b[i] = new byte[0x1800];
                Buffer.BlockCopy(tempData, i * 0x1800, b[i], 0x0, 0x1800);

                isEmpty[i] = new bool[0x1800];
                buildEmptyTable(b[i], ref isEmpty[i], i * 2, i * 2 + 1);
            }
            MyLog.logEvent("Regen", "Regen2Limit Started: " + tempFilename);
            MyLog.logEvent("Regen", "Regen2Limit Area: " + anchorX + " " + anchorY);

            string dodo = controller.setupDodo();
            MyLog.logEvent("Regen", "Regen2 Dodo: " + dodo);

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            RegenThread = new Thread(delegate () { regenMapFloor2(b, address, isEmpty, tempFilename, token); });
            RegenThread.Start();
        }

        private void miniMapBox_MouseDown(object sender, MouseEventArgs e)
        {
            Debug.Print(e.X.ToString() + " " + e.Y.ToString());

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

            miniMapBox.Image = MiniMap.drawSelectSquare(anchorX, anchorY);
        }

        private void miniMapBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
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

                miniMapBox.Image = MiniMap.drawSelectSquare(anchorX, anchorY);
            }
        }
        #endregion

        private void regenMapFloor(byte[][] b, UInt32 address, string name, CancellationToken token)
        {
            string regenMsg = "Regenerating... " + name;

            showMapWait(42, regenMsg);

            if (keepVillagerBox.Checked)
                prepareVillager(s);

            byte[] c = new byte[0x2000];

            int writeCount;
            int runCount = 0;
            int PauseCount = 0;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string newVisitor;
            TimeSpan ts;

            if (dodoSetup != null)
            {
                dodoSetup.LockBtn();
            }

            Utilities.sendBlankName(s);
            teleport.OverworldState state;

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
                        if (dodoSetup.CheckOnlineStatus() == true)
                            dodoSetup.DisplayDodo(controller.setupDodo());
                    }
                    else
                    {
                        state = teleport.GetOverworldState();
                        controller.setupDodo();
                    }

                    counter = 0;
                    writeCount = 0;

                    newVisitor = getVisitorName();

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (!newVisitor.Equals(string.Empty))
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            visitorNameBox.Text = newVisitor;
                            WaitMessagebox.Text = "Paused. " + newVisitor + " arriving!";
                            CreateLog(newVisitor);
                            PauseTimeLabel.Visible = true;
                            PauseTimer.Start();

                        });

                        Thread.Sleep(70000);
                        Utilities.sendBlankName(s);

                        this.Invoke((MethodInvoker)delegate
                        {
                            PauseTimeLabel.Visible = false;
                            PauseTimer.Stop();
                            pauseTime = 70;
                            PauseTimeLabel.Text = pauseTime.ToString();
                            WaitMessagebox.Text = regenMsg;
                        });
                        wasLoading = true;
                        state = teleport.GetOverworldState();
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (state == teleport.OverworldState.Loading || state == teleport.OverworldState.UserArriveLeavingOrTitleScreen)
                    {
                        Debug.Print("Loading Detected");
                        wasLoading = true;
                        PauseCount = 0;
                        Thread.Sleep(5000);
                    }
                    else if (state == teleport.OverworldState.ItemDropping)
                    {
                        Debug.Print("Item Dropping");
                        PauseCount = 0;
                    }
                    else
                    {
                        if (wasLoading || PauseCount >= 30)
                        {
                            GetVisitorList();
                            wasLoading = false;
                            PauseCount = 0;

                            //--------------------------------------------------------------------------------------------------
                            for (int i = 0; i < 42; i++)
                            {
                                lock (mapLock)
                                {
                                    c = Utilities.ReadByteArray8(s, address + (i * 0x2000), 0x2000, ref counter);

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
                                            Utilities.SendByteArray8(s, address + (i * 0x2000), b[i], 0x2000, ref writeCount);
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

                    this.Invoke((MethodInvoker)delegate
                    {
                        ts = stopWatch.Elapsed;
                        timeLabel.Text = Utilities.precedingZeros(ts.Hours.ToString(), 2) + ":" + Utilities.precedingZeros(ts.Minutes.ToString(), 2) + ":" + Utilities.precedingZeros(ts.Seconds.ToString(), 2);
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
                    Debug.Print("------ " + runCount + " " + PauseCount);
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MyLog.logEvent("Regen", "Regen1: " + ex.Message.ToString());
                        //DateTime localDate = DateTime.Now;
                        //myMessageBox.Show("Hey you! Stop messing with the switch!\n\n" + "Lost connection to the switch on " + localDate.ToString(), "Hey Listen!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MyMessageBox.Show("Connection to the switch has been lost", "Yeeted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        CreateLog("Connection to the switch has been lost");
                        this.Close();
                    });
                    break;
                }
            } while (loop);

            if (token.IsCancellationRequested)
            {
                MyLog.logEvent("Regen", "Regen1: Cancelled");
                MyMessageBox.Show("Dodo Helper & Regen Aborted!\nPlease remember to exit the airport first if you want to restart!", "Airbag deployment!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Invoke((MethodInvoker)delegate
                {
                    stopWatch.Stop();
                    hideMapWait();
                    MyLog.logEvent("Regen", "Regen1 Stopped");
                    loop = false;
                    startRegen.Tag = "Start";
                    startRegen.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
                    startRegen.Text = "Cast Regen";
                    saveMapBtn.Enabled = true;
                    loadMapBtn.Enabled = true;
                    backBtn.Enabled = true;
                    startRegen2.Enabled = true;
                    keepVillagerBox.Enabled = true;
                    dodoSetupBtn.Enabled = true;
                });
            }

            stopWatch.Stop();

            hideMapWait();

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void regenMapFloor2(byte[][] b, UInt32 address, bool[][] isEmpty, string name, CancellationToken token)
        {
            string regenMsg = "Regenerating... " + name;
            showMapWait(56, regenMsg);

            byte[][] u = new byte[56][];

            for (int i = 0; i < 56; i++)
            {
                u[i] = new byte[0x1800];
                Buffer.BlockCopy(b[i], 0, u[i], 0x0, 0x1800);
            }

            if (keepVillagerBox.Checked)
                prepareVillager(s);

            byte[] c = new byte[0x2000];

            int writeCount;
            int runCount = 0;
            int PauseCount = 0;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string newVisitor;
            TimeSpan ts;

            if (dodoSetup != null)
            {
                dodoSetup.LockBtn();
            }

            Utilities.sendBlankName(s);
            teleport.OverworldState state;

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
                        if (dodoSetup.CheckOnlineStatus() == true)
                            dodoSetup.DisplayDodo(controller.setupDodo());
                    }
                    else
                    {
                        state = teleport.GetOverworldState();
                        controller.setupDodo();
                    }

                    counter = 0;
                    writeCount = 0;

                    newVisitor = getVisitorName();

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (!newVisitor.Equals(string.Empty))
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            visitorNameBox.Text = newVisitor;
                            WaitMessagebox.Text = "Paused. " + newVisitor + " arriving!";
                            CreateLog(newVisitor);
                            PauseTimeLabel.Visible = true;
                            PauseTimer.Start();
                        });

                        Thread.Sleep(70000);
                        Utilities.sendBlankName(s);

                        this.Invoke((MethodInvoker)delegate
                        {
                            PauseTimeLabel.Visible = false;
                            PauseTimer.Stop();
                            pauseTime = 70;
                            PauseTimeLabel.Text = pauseTime.ToString();
                            WaitMessagebox.Text = regenMsg;
                        });
                        wasLoading = true;
                        state = teleport.GetOverworldState();
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (state == teleport.OverworldState.Loading || state == teleport.OverworldState.UserArriveLeavingOrTitleScreen)
                    {
                        Debug.Print("Loading Detected");
                        wasLoading = true;
                        PauseCount = 0;
                        Thread.Sleep(5000);
                    }
                    else if (state == teleport.OverworldState.ItemDropping)
                    {
                        Debug.Print("Item Dropping");
                        PauseCount = 0;
                    }
                    else
                    {
                        if (wasLoading || PauseCount >= 30)
                        {
                            GetVisitorList();
                            wasLoading = false;
                            PauseCount = 0;

                            //--------------------------------------------------------------------------------------------------
                            for (int i = 0; i < 56; i++)
                            {
                                if (token.IsCancellationRequested)
                                {
                                    break;
                                }

                                lock (mapLock)
                                {
                                    c = Utilities.ReadByteArray8(s, address + (i * 0x1800), 0x1800, ref counter);

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
                                            Utilities.SendByteArray8(s, address + (i * 0x1800), u[i], 0x1800, ref writeCount);
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

                    this.Invoke((MethodInvoker)delegate
                    {
                        ts = stopWatch.Elapsed;
                        timeLabel.Text = Utilities.precedingZeros(ts.Hours.ToString(), 2) + ":" + Utilities.precedingZeros(ts.Minutes.ToString(), 2) + ":" + Utilities.precedingZeros(ts.Seconds.ToString(), 2);
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
                    Debug.Print("------ " + runCount + " " + PauseCount);
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MyLog.logEvent("Regen", "Regen2: " + ex.Message.ToString());
                        //DateTime localDate = DateTime.Now;
                        //myMessageBox.Show("Hey you! Stop messing with the switch!\n\n" + "Lost connection to the switch on " + localDate.ToString(), "Hey Listen!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MyMessageBox.Show("Connection to the switch has been lost", "Yeeted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        CreateLog("Connection to the switch has been lost");
                        this.Close();
                    });
                    break;
                }
            } while (loop);

            if (token.IsCancellationRequested)
            {
                MyLog.logEvent("Regen", "Regen2: Cancelled");
                MyMessageBox.Show("Dodo Helper & Regen Aborted!\nPlease remember to exit the airport first if you want to restart!", "Slamming on the brakes?", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Invoke((MethodInvoker)delegate
                {
                    stopWatch.Stop();
                    hideMapWait();
                    MyLog.logEvent("Regen", "Regen2 Stopped");
                    loop = false;
                    startRegen2.Tag = "Start";
                    startRegen2.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
                    startRegen2.Text = "Cast Moogle Regenja";
                    saveMapBtn.Enabled = true;
                    loadMapBtn.Enabled = true;
                    backBtn.Enabled = true;
                    startRegen.Enabled = true;
                    keepVillagerBox.Enabled = true;
                    dodoSetupBtn.Enabled = true;
                });
            }

            stopWatch.Stop();

            hideMapWait();

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

        private void MapRegenerator_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseCleaning();
        }

        private void CloseCleaning()
        {
            MyLog.logEvent("Regen", "Form Closed");
            if (RegenThread != null)
            {
                MyLog.logEvent("Regen", "Regen Force Closed");
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

            this.closeForm();
        }

        private void hideBtn_Click(object sender, EventArgs e)
        {
            if (dodoSetup != null)
            {
                dodoSetup.Hide();
                dodoSetup.BringToFront();
            }
            this.ShowInTaskbar = false;
            this.Hide();
        }

        private void backBtn_Click(object sender, EventArgs e)
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
            this.Close();
        }

        private void buildEmptyTable(byte[] org, ref bool[] table)
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

        private void buildEmptyTable(byte[] org, ref bool[] table, int Part1x, int Part2x)
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

                if ((Math.Abs(anchorX - Part1x) <= 3) && (Math.Abs(anchorY - i) <= 3) && (Utilities.ByteToHexString(blockLeft)).Equals("FEFF000000000000FEFF000000000000") && (Utilities.ByteToHexString(blockRight)).Equals("FEFF000000000000FEFF000000000000"))
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

                if ((Math.Abs(anchorX - Part2x) <= 3) && (Math.Abs(anchorY - i) <= 3) && (Utilities.ByteToHexString(blockLeft)).Equals("FEFF000000000000FEFF000000000000") && (Utilities.ByteToHexString(blockRight)).Equals("FEFF000000000000FEFF000000000000"))
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
                MyLog.logEvent("Regen", "[Warning] Shifted? " + NotEqualNum + " bytes different.");
            }
            return pass;
        }

        private void delay_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!((c >= '0' && c <= '9')))
            {
                e.Handled = true;
            }
        }

        private string getVisitorName()
        {
            byte[] b = Utilities.getVisitorName(s);
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
            this.Invoke((MethodInvoker)delegate
            {
                pauseTime--;
                PauseTimeLabel.Text = pauseTime.ToString();
            });
        }

        private void logBtn_Click(object sender, EventArgs e)
        {
            if (logGridView.DataSource == null)
            {
                if (!File.Exists(Utilities.VisitorLogPath))
                {
                    string logheader = "Timestamp" + "," + "Name";

                    using (StreamWriter sw = File.CreateText(Utilities.VisitorLogPath))
                    {
                        sw.WriteLine(logheader);
                    }
                }
                logGridView.DataSource = loadCSV(Utilities.VisitorLogPath);
                logGridView.Columns["Timestamp"].Width = 195;
                logGridView.Columns["Name"].Width = 128;
                logGridView.Sort(logGridView.Columns[0], ListSortDirection.Descending);
                logPanel.Visible = true;
            }
            if (this.Width < 610)
            {
                this.Width = 610;
                logPanel.Visible = true;
            }
            else
            {
                this.Width = 250;
                logPanel.Visible = false;
            }
        }
        #endregion

        #region Log
        private DataTable loadCSV(string filePath)
        {
            var dt = new DataTable();

            File.ReadLines(filePath).Take(1)
                .SelectMany(x => x.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            dt.Columns["Timestamp"].DataType = typeof(DateTime);

            File.ReadLines(filePath).Skip(1)
                .Select(x => x.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));

            return dt;
        }

        private void CreateLog(string newVisitor)
        {
            if (!File.Exists(Utilities.VisitorLogPath))
            {
                string logheader = "Timestamp" + "," + "Name";

                using (StreamWriter sw = File.CreateText(Utilities.VisitorLogPath))
                {
                    sw.WriteLine(logheader);
                }
            }

            DateTime localDate = DateTime.Now;
            string newLog = localDate.ToString() + "," + newVisitor;

            using (StreamWriter sw = File.AppendText(Utilities.VisitorLogPath))
            {
                sw.WriteLine(newLog);
            }

            if (logGridView.DataSource != null)
            {
                logGridView.DataSource = loadCSV(Utilities.VisitorLogPath);
                logGridView.Sort(logGridView.Columns[0], ListSortDirection.Descending);
            }
        }

        private void newLogBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog()
            {
                Filter = "Comma-Separated Values file (*.csv)|*.csv",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            string savepath;

            if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + @"\save";
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

            string logheader = "Timestamp" + "," + "Name";

            using (StreamWriter sw = File.CreateText(file.FileName))
            {
                sw.WriteLine(logheader);
            }

            string[] s = file.FileName.Split('\\');

            logName.Text = s[s.Length - 1];
            Utilities.VisitorLogPath = file.FileName;

            if (logGridView.DataSource != null)
            {
                logGridView.DataSource = loadCSV(Utilities.VisitorLogPath);
                logGridView.Sort(logGridView.Columns[0], ListSortDirection.Descending);
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void selectLogBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog()
            {
                Filter = "Comma-Separated Values file (*.csv)|*.csv",
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

            string[] s = file.FileName.Split('\\');

            logName.Text = s[s.Length - 1];
            Utilities.VisitorLogPath = file.FileName;

            if (logGridView.DataSource != null)
            {
                logGridView.DataSource = loadCSV(Utilities.VisitorLogPath);
                logGridView.Sort(logGridView.Columns[0], ListSortDirection.Descending);
            }
        }

        #endregion

        #region Villager

        public static void prepareVillager(Socket s)
        {
            villagerFlag = new byte[10][];
            villager = new byte[10][];
            haveVillager = new Boolean[10];

            for (int i = 0; i < 10; i++)
            {
                villager[i] = Utilities.GetVillager(s, null, i, 0x3);
                villagerFlag[i] = Utilities.GetMoveout(s, null, i, (int)0x33);
                haveVillager[i] = checkHaveVillager(villager[i]);
            }
            writeVillager(villager, haveVillager);
        }

        public static void updateVillager(Socket s, int index)
        {
            if (villager == null)
                prepareVillager(s);
            else
            {
                villager[index] = Utilities.GetVillager(s, null, index, 0x3);
                villagerFlag[index] = Utilities.GetMoveout(s, null, index, (int)0x33);
                haveVillager[index] = true;

                writeVillager(villager, haveVillager);
            }
        }

        public static Boolean checkHaveVillager(byte[] villager)
        {
            if (villager[0] >= 0x23)
                return false;
            else
                return true;
        }

        public static void writeVillager(byte[][] villager, Boolean[] haveVillager)
        {
            string villagerStr = " | ";
            for (int i = 0; i < 10; i++)
            {
                if (haveVillager[i])
                    villagerStr += Utilities.GetVillagerRealName(villager[i][0], villager[i][1]) + " | ";
            }

            using (StreamWriter sw = File.CreateText(Utilities.CurrentVillagerPath))
            {
                sw.WriteLine(villagerStr);
            }
        }

        private void CheckAndResetVillager(byte[] villagerFlag, Boolean haveVillager, int index, ref int counter)
        {
            if (!haveVillager)
            {
                return;
            }
            else
            {
                string ByteString = Utilities.ByteToHexString(Utilities.GetMoveout(s, null, index, (int)0x33, ref counter));
                if (!ByteString.Equals(Utilities.ByteToHexString(villagerFlag)))
                {
                    Utilities.SetMoveout(s, null, index, villagerFlag, ref counter);
                    Debug.Print("Reset Villager " + index);
                    MyLog.logEvent("Regen", "Villager Reset : " + index);
                }
                //else
                //Debug.Print("No Reset Needed " + index);
            }
        }
        #endregion

        #region Visitor

        private void GetVisitorList()
        {
            string[] namelist = new string[8];
            int num = 0;
            using (StreamWriter sw = File.CreateText(Utilities.CurrentVisitorPath))
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i == 0)
                        continue;
                    namelist[i] = Utilities.GetVisitorName(s, null, i);
                    if (namelist[i].Equals(String.Empty))
                        sw.WriteLine("[Empty]");
                    else
                    {
                        sw.WriteLine(namelist[i]);
                        num++;
                    }
                }
                sw.WriteLine("Num of Visitor : " + num);
                /*if (num >= 7)
                {
                    sw.WriteLine(" [Island Full] ");
                }*/
            }
            //Debug.Print("Visitor Update");
        }

        #endregion

        #region Dodo Helper
        private void dodoHelperBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn.Tag.ToString().Equals("Enable"))
            {
                btn.Text = "Disable Dodo Helper";
                btn.Tag = "Disable";
                btn.BackColor = Color.Orange;

                dodoSetup = new dodo(s);
                dodoSetup.closeForm += DodoSetup_closeForm;
                dodoSetup.abortAll += DodoSetup_abortAll;
                dodoSetup.Show();
                dodoSetup.Location = new Point(this.Location.X - 590, this.Location.Y);
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
                btn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
                dodoSetup.Close();
                dodoSetup = null;
            }
        }

        private void DodoSetup_abortAll()
        {
            abort();
        }

        private void DodoSetup_closeForm()
        {
            dodoSetupBtn.Text = "Enable Dodo Helper";
            dodoSetupBtn.Tag = "Enable";
            dodoSetupBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            dodoSetup = null;
        }

        private void MapRegenerator_Move(object sender, EventArgs e)
        {
            if (dodoSetup != null)
            {
                dodoSetup.Location = new Point(this.Location.X - 590, this.Location.Y);
                dodoSetup.BringToFront();
            }
        }

        private void changeDodoBtn_Click(object sender, EventArgs e)
        {
            string newPath = controller.changeDodoPath();

            if (dodoSetup != null)
            {
                dodoSetup.WriteLog(">> Dodo path change to : " + newPath);
            }
        }

        public void abort()
        {
            cts.Cancel();
        }

        #endregion

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (dodoSetup != null)
                dodoSetup.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }
    }
}
