using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Twitch;

namespace ACNHPokerCore
{
    public partial class Dodo : Form
    {
        private static Socket s;
        public Boolean dodoSetupDone = false;
        private Boolean idleEmote = false;
        private int idleNum = 0;
        private Boolean HoldingL = false;
        private PubSub MyPubSub = null;
        private TwitchBot MyTwitchBot;
        private string TwitchBotUserName;
        private string TwitchBotOauth;
        private string TwitchChannelName;
        private string TwitchChannelAccessToken;
        private string TwitchChannelid;
        private bool wasLoading = false;
        private bool lastOrderIsRecipe = false;
        private static OrderDisplay itemDisplay;
        private static MyStopWatch stopWatch;
        bool dropItem = false;
        bool injectVillager = false;
        bool restoreDodo = true;

        Thread standaloneThread;
        private bool standaloneRunning = false;
        private bool resetSession = false;


        bool W = false;
        bool A = false;
        bool S = false;
        bool D = false;
        bool resetted = true;
        private MovingDirection currentDirection = MovingDirection.Null;

        bool I = false;
        bool holdingI = false;
        bool J = false;
        bool holdingJ = false;
        bool K = false;
        bool holdingK = false;
        bool L = false;
        bool holdingL = false;

        private CancellationTokenSource cts;

        public event CloseHandler CloseForm;
        public event ThreadAbortHandler AbortAll;

        public enum MovingDirection
        {
            Up,
            Down,
            Left,
            Right,
            UpRight,
            UpLeft,
            DownRight,
            DownLeft,
            Null
        }

        public Dodo(Socket S, bool standaloneMode = false)
        {
            InitializeComponent();
            s = S;

            if (Teleport.AllAnchorValid())
            {
                Point Done = new(-4200, 0);
                FullPanel.Location = Done;
                dodoSetupDone = true;
            }

            if (File.Exists(Utilities.TwitchSettingPath))
            {
                TwitchBtn.Visible = true;
                itemDisplayBtn.Visible = true;
                dropItemBox.Enabled = true;
                injectVillagerBox.Enabled = true;
            }

            if (standaloneMode)
            {
                standaloneStart.Visible = true;
            }
            controllerTimer.Start();
            this.KeyPreview = true;
        }

        #region Teleport Setup

        private void StartNextBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point page1 = new(-600, 0);
            FullPanel.Location = page1;
        }

        private void Anchor0NextBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point page2 = new(-1200, 0);
            FullPanel.Location = page2;
        }

        private void Anchor1NextBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point page3 = new(-1800, 0);
            FullPanel.Location = page3;
        }

        private void Anchor2NextBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point page4 = new(-2400, 0);
            FullPanel.Location = page4;
        }

        private void Anchor3NextBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point page5 = new(-3000, 0);
            FullPanel.Location = page5;
        }

        private void Anchor4NextBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point page6 = new(-3600, 0);
            FullPanel.Location = page6;
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            if (Teleport.AllAnchorValid())
            {
                ResetBtn();
                Point Done = new(-4200, 0);
                FullPanel.Location = Done;
                dodoSetupDone = true;
            }
            else
            {
                MyMessageBox.Show("One or more anchors have not been setup correctly.", "Skip leg day?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Anchor0PreviousBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point Start = new(0, 0);
            FullPanel.Location = Start;
        }

        private void Anchor1PreviousBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point page1 = new(-600, 0);
            FullPanel.Location = page1;
        }

        private void Anchor2PreviousBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point page2 = new(-1200, 0);
            FullPanel.Location = page2;
        }

        private void Anchor3PreviousBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point page3 = new(-1800, 0);
            FullPanel.Location = page3;
        }

        private void Anchor4PreviousBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point page4 = new(-2400, 0);
            FullPanel.Location = page4;
        }

        private void DonePreviousBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point page5 = new(-3000, 0);
            FullPanel.Location = page5;
        }

        private void Anchor0SetBtn_Click(object sender, EventArgs e)
        {
            Teleport.SetAnchor(0);
            Anchor0SetBtn.ForeColor = Color.Green;
            Anchor0Line3.Visible = true;
            Anchor0TestBtn.Visible = true;
            Anchor0TestBtn.Enabled = true;
            Anchor0TestBtn.Text = "Test";
            Anchor0TestBtn.ForeColor = Color.White;
        }

        private void Anchor0TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new(delegate () { TestTeleport(0, btn); });
            TeleportThread.Start();
        }

        private void Anchor1SetBtn_Click(object sender, EventArgs e)
        {
            Teleport.SetAnchor(1);
            Anchor1SetBtn.ForeColor = Color.Green;
            Anchor1Line3.Visible = true;
            Anchor1TestBtn.Visible = true;
            Anchor1TestBtn.Enabled = true;
            Anchor1TestBtn.Text = "Test";
            Anchor1TestBtn.ForeColor = Color.White;
        }

        private void Anchor1TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new(delegate () { TestTeleport(1, btn); });
            TeleportThread.Start();
        }

        private void Anchor2SetBtn_Click(object sender, EventArgs e)
        {
            Teleport.SetAnchor(2);
            Anchor2SetBtn.ForeColor = Color.Green;
            Anchor2Line3.Visible = true;
            Anchor2TestBtn.Visible = true;
            Anchor2TestBtn.Enabled = true;
            Anchor2TestBtn.Text = "Test";
            Anchor2TestBtn.ForeColor = Color.White;
        }

        private void Anchor2TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new(delegate () { TestTeleport(2, btn); });
            TeleportThread.Start();
        }

        private void Anchor3SetBtn_Click(object sender, EventArgs e)
        {
            Teleport.SetAnchor(3);
            Anchor3SetBtn.ForeColor = Color.Green;
            Anchor3Line3.Visible = true;
            Anchor3TestBtn.Visible = true;
            Anchor3TestBtn.Enabled = true;
            Anchor3TestBtn.Text = "Test";
            Anchor3TestBtn.ForeColor = Color.White;
        }

        private void Anchor3TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new(delegate () { TestTeleport(3, btn); });
            TeleportThread.Start();
        }

        private void Anchor4SetBtn_Click(object sender, EventArgs e)
        {
            Teleport.SetAnchor(4);
            Anchor4SetBtn.ForeColor = Color.Green;
            Anchor4Line3.Visible = true;
            Anchor4TestBtn.Visible = true;
            Anchor4TestBtn.Enabled = true;
            Anchor4TestBtn.Text = "Test";
            Anchor4TestBtn.ForeColor = Color.White;
        }

        private void Anchor4TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new(delegate () { TestTeleport(4, btn); });
            TeleportThread.Start();
        }

        private void DoneAnchor0TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new(delegate () { TestTeleport(0, btn); });
            TeleportThread.Start();
        }

        private void DoneAnchor1TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new(delegate () { TestTeleport(1, btn); });
            TeleportThread.Start();
        }

        private void DoneAnchor2TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new(delegate () { TestTeleport(2, btn); });
            TeleportThread.Start();
        }

        private void DoneAnchor3TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new(delegate () { TestTeleport(3, btn); });
            TeleportThread.Start();
        }

        private void DoneAnchor4TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new(delegate () { TestTeleport(4, btn); });
            TeleportThread.Start();
        }

        private static void TestTeleport(int num, Button btn)
        {
            if (Teleport.TeleportToAnchor(num))
            {
                btn.Invoke((MethodInvoker)delegate
                {
                    btn.Enabled = true;
                    btn.Text = "Success";
                    btn.ForeColor = Color.Green;
                });
            }
            else
            {
                btn.Invoke((MethodInvoker)delegate
                {
                    btn.Enabled = true;
                    btn.Text = "Failed";
                    btn.ForeColor = Color.Red;
                });
            }
        }

        private void ResetBtn()
        {
            Anchor0SetBtn.ForeColor = Color.White;
            Anchor0TestBtn.Enabled = true;
            Anchor0TestBtn.Text = "Test";
            Anchor0TestBtn.ForeColor = Color.White;
            Anchor1SetBtn.ForeColor = Color.White;
            Anchor1TestBtn.Enabled = true;
            Anchor1TestBtn.Text = "Test";
            Anchor1TestBtn.ForeColor = Color.White;
            Anchor2SetBtn.ForeColor = Color.White;
            Anchor2TestBtn.Enabled = true;
            Anchor2TestBtn.Text = "Test";
            Anchor2TestBtn.ForeColor = Color.White;
            Anchor3SetBtn.ForeColor = Color.White;
            Anchor3TestBtn.Enabled = true;
            Anchor3TestBtn.Text = "Test";
            Anchor3TestBtn.ForeColor = Color.White;
            Anchor4SetBtn.ForeColor = Color.White;
            Anchor4TestBtn.Enabled = true;
            Anchor4TestBtn.Text = "Test";
            Anchor4TestBtn.ForeColor = Color.White;
            DoneAnchor0TestBtn.Text = "Test";
            DoneAnchor0TestBtn.ForeColor = Color.White;
            DoneAnchor1TestBtn.Text = "Test";
            DoneAnchor1TestBtn.ForeColor = Color.White;
            DoneAnchor2TestBtn.Text = "Test";
            DoneAnchor2TestBtn.ForeColor = Color.White;
            DoneAnchor3TestBtn.Text = "Test";
            DoneAnchor3TestBtn.ForeColor = Color.White;
            DoneAnchor4TestBtn.Text = "Test";
            DoneAnchor4TestBtn.ForeColor = Color.White;
        }

        private void DoneFullTestBtn_Click(object sender, EventArgs e)
        {
            DoneAnchor0TestBtn.Enabled = false;
            DoneAnchor1TestBtn.Enabled = false;
            DoneAnchor2TestBtn.Enabled = false;
            DoneAnchor3TestBtn.Enabled = false;
            DoneAnchor4TestBtn.Enabled = false;
            DoneFullTestBtn.Enabled = false;

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            Thread TeleportThread = new(delegate () { TestNormalRestore(token); });
            TeleportThread.Start();
        }

        private void TestNormalRestore(CancellationToken token)
        {
            Controller.ClickDown(); // Hide Weapon
            Thread.Sleep(1000);

            Teleport.TeleportToAnchor(2);

            Debug.Print("Teleport to Airport");

            do
            {
                //Debug.Print(teleport.GetOverworldState().ToString());
                Debug.Print("Try Enter Airport");
                Controller.EnterAirport();
                Thread.Sleep(2000);
            }
            while (Teleport.GetOverworldState() != Teleport.OverworldState.OverworldOrInAirport);

            Debug.Print("Inside Airport");
            Thread.Sleep(2000);

            Teleport.TeleportToAnchor(3);

            Debug.Print("Get Dodo");
            Controller.TalkAndGetDodoCode(token);
            Debug.Print("Finish getting Dodo");

            Teleport.TeleportToAnchor(4);

            do
            {
                //Debug.Print(teleport.GetOverworldState().ToString());
                Debug.Print("Try Exit Airport");
                Controller.ExitAirport();
                Thread.Sleep(2000);
            }
            while (Teleport.GetOverworldState() != Teleport.OverworldState.OverworldOrInAirport);

            Debug.Print("Back to Overworld");
            Thread.Sleep(2000);

            Teleport.TeleportToAnchor(1);

            Controller.Emote(0);

            Controller.DetachController();

            this.Invoke((MethodInvoker)delegate
            {
                DoneFullTestBtn.Text = "Done";
                DoneFullTestBtn.ForeColor = Color.Green;
                DoneAnchor0TestBtn.Enabled = true;
                DoneAnchor1TestBtn.Enabled = true;
                DoneAnchor2TestBtn.Enabled = true;
                DoneAnchor3TestBtn.Enabled = true;
                DoneAnchor4TestBtn.Enabled = true;
                DoneFullTestBtn.Enabled = true;
            });
        }

        #endregion

        private void Dodo_FormClosed(object sender, FormClosedEventArgs e)
        {
            controllerTimer.Stop();

            if (MyPubSub != null)
            {
                MyPubSub.Dispose();
                MyPubSub = null;
            }
            if (MyTwitchBot != null)
            {
                MyTwitchBot = null;
            }
            if (itemDisplay != null)
            {
                itemDisplay.Close();
                itemDisplay = null;
            }
            if (stopWatch != null)
            {
                stopWatch.Close();
                stopWatch.Dispose();
                stopWatch = null;
            }

            this.CloseForm();
        }

        private void BackToSetupBtn_Click(object sender, EventArgs e)
        {
            ResetBtn();
            Point Start = new(0, 0);
            FullPanel.Location = Start;
        }

        public bool CheckOnlineStatus()
        {
            if (Teleport.CheckOnlineStatus() == 0x1)
            {
                onlineLabel.Invoke((MethodInvoker)delegate
                {
                    onlineLabel.Text = "ONLINE";
                    onlineLabel.ForeColor = Color.Green;
                });
                return true;
            }
            else if (Teleport.CheckOnlineStatus(true) == 0x1) //Check again for Chinese
            {
                onlineLabel.Invoke((MethodInvoker)delegate
                {
                    onlineLabel.Text = "ONLINE";
                    onlineLabel.ForeColor = Color.Lime;
                });
                return true;
            }
            else
            {
                onlineLabel.Invoke((MethodInvoker)delegate
                {
                    onlineLabel.Text = "OFFLINE";
                    onlineLabel.ForeColor = Color.Red;
                });
                return false;
            }
        }

        public void DisplayDodo(string dodo)
        {
            dodoCode.Invoke((MethodInvoker)delegate
            {
                dodoCode.Text = dodo;
            });
        }

        public void WriteLog(string line, Boolean time = false)
        {
            dodoLog.Invoke((MethodInvoker)delegate
            {
                if (time)
                {
                    DateTime localDate = DateTime.Now;
                    dodoLog.AppendText(localDate.ToString() + " : " + line + "\n");
                }
                else
                    dodoLog.AppendText(line + "\n");
            });
        }

        public void LockBtn()
        {
            BackToSetupBtn.Invoke((MethodInvoker)delegate
            {
                BackToSetupBtn.Enabled = false;
            });
        }

        private void LockControl()
        {
            this.Invoke((MethodInvoker)delegate
            {
                controllerPanel.Enabled = false;
                functionPanel.Enabled = false;
                AbortBtn.Visible = true;
            });
        }

        private void UnLockControl()
        {
            this.Invoke((MethodInvoker)delegate
            {
                controllerPanel.Enabled = true;
                functionPanel.Enabled = true;
                AbortBtn.Visible = false;
            });
        }

        public Teleport.OverworldState DodoMonitor(CancellationToken token)
        {
            Teleport.OverworldState state = Teleport.GetOverworldState();
            //WriteLog(state.ToString() + " " + idleNum, true);

            if (token.IsCancellationRequested)
                return state;

            if (CheckOnlineStatus() == true)
            {
                if (state == Teleport.OverworldState.Loading || state == Teleport.OverworldState.UserArriveLeavingOrTitleScreen)
                {
                    idleNum = 0;
                    wasLoading = true;
                }
            }
            else
            {
                if (restoreDodo)
                {
                    WriteLog("[Warning] Disconnected.", true);
                    WriteLog("Please wait a moment for the restore.", true);
                    LockControl();

                    int retry = 0;
                    do
                    {
                        if (token.IsCancellationRequested)
                            return state;
                        if (retry >= 30)
                        {
                            WriteLog("[Warning] Start Hard Restore", true);
                            HardRestore(token);
                            break;
                        }
                        if (Teleport.GetLocationState() == Teleport.LocationState.Announcement)
                        {
                            WriteLog("In Announcement", true);
                            if (!HoldingL)
                            {
                                Controller.PressL();
                                HoldingL = true;
                            }
                            retry = 0;
                        }
                        else
                        {
                            WriteLog("Waiting for Overworld", true);
                        }
                        Controller.ClickA();
                        Thread.Sleep(3000);
                        retry++;
                    }
                    while (Teleport.GetOverworldState() != Teleport.OverworldState.OverworldOrInAirport);

                    //Thread.Sleep(2000);
                    Controller.ReleaseL();
                    HoldingL = false;
                    WriteLog("[Warning] Start Normal Restore", true);
                    WriteLog("Please wait for the bot to finish the sequence.", true);
                    NormalRestore(token);
                    UnLockControl();
                    WriteLog("Restore sequence finished.", true);
                    idleNum = 0;
                    state = Teleport.OverworldState.OverworldOrInAirport;
                }
            }

            if (token.IsCancellationRequested)
                return state;

            if (state != Teleport.OverworldState.Loading && state != Teleport.OverworldState.UserArriveLeavingOrTitleScreen)
            {
                if (MyPubSub != null)
                {
                    if (dropItem)
                    {
                        if (idleNum >= 2)
                        {
                            if (wasLoading)
                            {
                                if (Utilities.hasItemInFirstSlot(s))
                                {
                                    if (lastOrderIsRecipe)
                                        Controller.DropRecipe();
                                    else
                                        Controller.DropItem();
                                }

                                wasLoading = false;
                            }

                            if (PubSub.DropOrderList.Count <= 0)
                            {
                                Debug.Print("No Item Drop Order");
                            }
                            else
                            {
                                _ = DropItem(PubSub.DropOrderList.ElementAt(0));
                                if (PubSub.DropOrderList.Count > 0)
                                    state = Teleport.OverworldState.ItemDropping;
                            }
                        }

                        if (PubSub.DropOrderList.Count > 0)
                            state = Teleport.OverworldState.ItemDropping;
                    }

                    if (injectVillager)
                    {
                        if (PubSub.VillagerOrderList.Count <= 0)
                        {
                            Debug.Print("No Villager Order");
                        }
                        else if (state != Teleport.OverworldState.ItemDropping && idleNum >= 2)
                        {
                            _ = InjectVillager(PubSub.VillagerOrderList.ElementAt(0));
                        }
                    }
                }

                if (idleEmote && state == Teleport.OverworldState.OverworldOrInAirport)
                {
                    if (idleNum >= 10 && idleNum % 10 == 0)
                    {
                        Random random = new();
                        int v = random.Next(0, 8);
                        Controller.Emote(v);
                    }
                }
            }

            idleNum++;
            return state;
        }

        public void NormalRestore(CancellationToken token)
        {
            do
            {
                if (token.IsCancellationRequested)
                    return;
                if (Teleport.GetLocationState() == Teleport.LocationState.Announcement)
                {
                    WriteLog("In Announcement", true);
                    if (!HoldingL)
                    {
                        Controller.PressL();
                        HoldingL = true;
                    }
                }
                else
                {
                    WriteLog("Confirming Overworld", true);
                }
                Controller.ClickA();
                Thread.Sleep(3000);
            }
            while (Teleport.GetOverworldState() != Teleport.OverworldState.OverworldOrInAirport);

            Controller.ReleaseL();
            HoldingL = false;
            Thread.Sleep(2000);
            Controller.ClickDown(); // Hide Weapon
            Thread.Sleep(1000);
            if (token.IsCancellationRequested)
                return;

            //string locationState = Teleport.GetLocationState().ToString();
            //Debug.Print(">>>>>>>>>>>>>>>>>>>>>>>>>>" + locationState);

            if (Teleport.GetLocationState() == Teleport.LocationState.Indoor)
            {
                WriteLog("Indoor Detected", true);
            }
            else
            {
                Teleport.TeleportToAnchor(2);
                WriteLog("Teleport to Airport", true);

                do
                {
                    if (token.IsCancellationRequested)
                        return;
                    WriteLog("Try Entering Airport", true);
                    Controller.EnterAirport();
                    Thread.Sleep(2000);
                }
                while (Teleport.GetOverworldState() != Teleport.OverworldState.OverworldOrInAirport);

                WriteLog("Inside Airport", true);
                Thread.Sleep(2000);
            }

            Teleport.TeleportToAnchor(3);

            WriteLog("Try Getting Dodo", true);
            string NewDodo;
            if (skipDialogCheckBox.Checked)
                NewDodo = Controller.TalkAndGetDodoCode(token);
            else
                NewDodo = Controller.TalkAndGetDodoCodeLegacy(token);

            if (token.IsCancellationRequested)
                return;

            DisplayDodo(NewDodo);

            WriteLog("Finish Getting Dodo", true);

            if (token.IsCancellationRequested)
                return;

            CheckOnlineStatus();

            Teleport.TeleportToAnchor(4);

            do
            {
                if (token.IsCancellationRequested)
                    return;
                //Debug.Print(teleport.GetOverworldState().ToString());
                WriteLog("Try Exiting Airport", true);
                Controller.ExitAirport();
                Thread.Sleep(2000);
            }
            while (Teleport.GetOverworldState() != Teleport.OverworldState.OverworldOrInAirport);

            WriteLog("Back to Overworld", true);
            Thread.Sleep(2000);

            if (token.IsCancellationRequested)
                return;
            Teleport.TeleportToAnchor(1);

            Controller.Emote(0);

            //controller.detachController();
        }

        public void HardRestore(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            WriteLog("Capturing the Crash", true);
            Controller.ClickCAPTURE();
            Thread.Sleep(2000);
            if (token.IsCancellationRequested)
                return;
            WriteLog("Open Home Menu", true);
            Controller.ClickHOME();
            Thread.Sleep(5000);
            if (token.IsCancellationRequested)
                return;
            Controller.ClickX();
            Thread.Sleep(1000);
            if (token.IsCancellationRequested)
                return;
            Controller.ClickA(); //Close Game
            WriteLog("Try Closing the Game", true);
            Thread.Sleep(15000);
            if (token.IsCancellationRequested)
                return;
            Controller.ClickA(); //Select Game
            Thread.Sleep(2000);
            if (token.IsCancellationRequested)
                return;
            Controller.ClickA(); //Select first user
            WriteLog("Game & User Selected", true);
            Thread.Sleep(10000);
            if (token.IsCancellationRequested)
                return;

            int retry = 0;
            do
            {
                if (token.IsCancellationRequested)
                    return;
                //Debug.Print(teleport.GetOverworldState().ToString());
                //Debug.Print("Waiting for Overworld");
                if (Teleport.GetLocationState() == Teleport.LocationState.Announcement)
                {
                    WriteLog("In Announcement", true);
                    if (!HoldingL)
                    {
                        Controller.PressL();
                        HoldingL = true;
                    }
                }
                Controller.ClickA();
                Thread.Sleep(2000);
                retry++;
            }
            while (Teleport.GetOverworldState() != Teleport.OverworldState.OverworldOrInAirport);

            Controller.ReleaseL();
            HoldingL = false;
            WriteLog("Exiting House", true);
            Thread.Sleep(2000);
            if (token.IsCancellationRequested)
                return;
        }

        private void CloseGate()
        {
            Controller.ClickDown(); // Hide Weapon
            Thread.Sleep(1000);

            Teleport.TeleportToAnchor(2);

            Debug.Print("Teleport to Airport");

            do
            {
                //Debug.Print(teleport.GetOverworldState().ToString());
                Debug.Print("Try Enter Airport");
                Controller.EnterAirport();
                Thread.Sleep(2000);
            }
            while (Teleport.GetOverworldState() != Teleport.OverworldState.OverworldOrInAirport);

            Debug.Print("Inside Airport");
            Thread.Sleep(2000);

            Teleport.TeleportToAnchor(3);

            Debug.Print("Close Gate");
            Controller.TalkAndCloseGate();
            Debug.Print("Finish Close Gate");
            CheckOnlineStatus();

            Teleport.TeleportToAnchor(4);

            do
            {
                //Debug.Print(teleport.GetOverworldState().ToString());
                Debug.Print("Try Exit Airport");
                Controller.ExitAirport();
                Thread.Sleep(2000);
            }
            while (Teleport.GetOverworldState() != Teleport.OverworldState.OverworldOrInAirport);

            Debug.Print("Back to Overworld");
            Thread.Sleep(2000);

        }

        public static void EndSession(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            Controller.ClickDown(); // Hide Weapon
            Thread.Sleep(1000);
            if (token.IsCancellationRequested)
                return;
            Controller.ClickMINUS(); // Open menu
            Thread.Sleep(2000);
            if (token.IsCancellationRequested)
                return;
            Controller.ClickA(); // Open Selection
            Thread.Sleep(1000);
            if (token.IsCancellationRequested)
                return;
            Controller.ClickA(); // Select End Session.
            Thread.Sleep(10000);
        }

        private async Task DropItem(ItemOrder CurrentOrder)
        {
            string flag1;
            if (CurrentOrder.Id == "16A2" || CurrentOrder.Name.Contains("wrapping paper") || CurrentOrder.Id == "3107" || CurrentOrder.Id == "3106") // Vine Glowing Moss
                flag1 = "00";
            else
                flag1 = "7F";

            string flag2 = "00";

            if (itemDisplay != null)
            {
                string path = Main.GetImagePathFromID(CurrentOrder.Id, Convert.ToUInt32("0x" + Utilities.precedingZeros(CurrentOrder.Count, 8), 16));
                if (File.Exists(path))
                {
                    Image image = Image.FromFile(path);
                    itemDisplay.SetItemdisplay(image);
                }
            }

            Utilities.SpawnItem(s, null, 0, flag1 + flag2 + CurrentOrder.Id, Utilities.precedingZeros(CurrentOrder.Count, 8));
            //Thread.Sleep(500);
            if (CurrentOrder.Id == "16A2" || CurrentOrder.Id == "3107" || CurrentOrder.Id == "3106")
            {
                Controller.DropRecipe();
                lastOrderIsRecipe = true;
            }
            else
            {
                Controller.DropItem();
                lastOrderIsRecipe = false;
            }
            if (!CurrentOrder.Color.Equals(string.Empty))
                await MyTwitchBot.SendMessage($"{CurrentOrder.Owner}, your order of \"{CurrentOrder.Name}\" ({CurrentOrder.Color}) have been dropped.");
            else
                await MyTwitchBot.SendMessage($"{CurrentOrder.Owner}, your order of \"{CurrentOrder.Name}\" have been dropped.");

            //await MyTwitchBot.SendMessage($"If you can't find your order, people flying in/out might have canceled it. We are very sorry. Feel free to place your order again.");

            PubSub.DropOrderList.RemoveAt(0);
        }

        private async Task InjectVillager(VillagerOrder CurrentOrder)
        {
            List<string> VillagerList = Utilities.GetVillagerList(s);

            if (VillagerList.Contains(CurrentOrder.InternalName))
            {
                int i = VillagerList.IndexOf(CurrentOrder.InternalName);

                Utilities.SetMoveout(s, null, i, "2", "0");

                await MyTwitchBot.SendMessage($"{CurrentOrder.Owner}, \"{CurrentOrder.RealName}\" is already waiting for you on the island.");

                PubSub.VillagerOrderList.RemoveAt(0);

                //MapRegenerator.updateVillager(s, i);
            }
            else
            {
                int houseIndex = 9;
                int villagerIndex = Convert.ToInt32(Utilities.GetHouseOwner(s, null, houseIndex));

                string IVpath = Utilities.villagerPath + CurrentOrder.InternalName + ".nhv2";
                string RVpath = Utilities.villagerPath + CurrentOrder.RealName + ".nhv2";

                byte[] villagerData;
                byte[] houseData;

                if (File.Exists(IVpath))
                    villagerData = File.ReadAllBytes(IVpath);
                else if (File.Exists(RVpath))
                    villagerData = File.ReadAllBytes(RVpath);
                else
                {
                    WriteLog("Villager files \"" + CurrentOrder.InternalName + ".nhv2\" " + "/ \"" + CurrentOrder.RealName + ".nhv2\" " + "not found!", true);
                    PubSub.VillagerOrderList.RemoveAt(0);
                    return;
                }

                string IHpath = Utilities.villagerPath + CurrentOrder.InternalName + ".nhvh2";
                string RHpath = Utilities.villagerPath + CurrentOrder.RealName + ".nhvh2";
                if (File.Exists(IHpath))
                    houseData = File.ReadAllBytes(IHpath);
                else if (File.Exists(RHpath))
                    houseData = File.ReadAllBytes(RHpath);
                else
                {
                    WriteLog("Villager house files \"" + CurrentOrder.InternalName + ".nhvh2\" " + "/ \"" + CurrentOrder.RealName + ".nhvh2\" " + "not found!", true);
                    PubSub.VillagerOrderList.RemoveAt(0);
                    return;
                }

                WriteLog($"Loading villager... \"{CurrentOrder.RealName}\"", true);

                byte[] modifiedVillager = villagerData;
                Buffer.BlockCopy(Main.GetHeader(), 0x0, modifiedVillager, 0x4, 52);

                byte[] modifiedHouse = houseData;

                byte h = (Byte)villagerIndex;
                modifiedHouse[Utilities.VillagerHouseOwnerOffset] = h;

                await Utilities.loadBoth(s, villagerIndex, villagerData, houseIndex, houseData);
                await Utilities.SetMoveout(s, villagerIndex, "2", "0");

                await MyTwitchBot.SendMessage($"{CurrentOrder.Owner}, \"{CurrentOrder.RealName}\" is now waiting for you on the island.");

                PubSub.VillagerOrderList.RemoveAt(0);

                //MapRegenerator.updateVillager(s, villagerIndex);
                WriteLog($"Villager \"{CurrentOrder.RealName}\" loaded!", true);
            }

        }

        private void DodoLog_TextChanged(object sender, EventArgs e)
        {
            dodoLog.SelectionStart = dodoLog.Text.Length;
            dodoLog.ScrollToCaret();
        }

        #region Virtual Controller

        private void LstickMouseUp(object sender, MouseEventArgs e)
        {
            W = false;
            A = false;
            S = false;
            D = false;
        }

        private void LstickUPBtn_MouseDown(object sender, MouseEventArgs e)
        {
            W = true;
        }

        private void LstickRIGHTBtn_MouseDown(object sender, MouseEventArgs e)
        {
            D = true;
        }

        private void LstickDOWNBtn_MouseDown(object sender, MouseEventArgs e)
        {
            S = true;
        }

        private void LstickLEFTBtn_MouseDown(object sender, MouseEventArgs e)
        {
            A = true;
        }

        private void XBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickX();
        }

        private void ABtn_Click(object sender, EventArgs e)
        {
            Controller.ClickA();
        }

        private void BBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickB();
        }

        private void YBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickY();
        }

        private void EmoteUPBtn_Click(object sender, EventArgs e)
        {
            Controller.Emote(0);
        }

        private void EmoteRIGHTBtn_Click(object sender, EventArgs e)
        {
            Controller.Emote(2);
        }

        private void EmoteDOWNBtn_Click(object sender, EventArgs e)
        {
            Controller.Emote(4);
        }

        private void EmoteLEFTBtn_Click(object sender, EventArgs e)
        {
            Controller.Emote(6);
        }

        private void EmoteTopRightBtn_Click(object sender, EventArgs e)
        {
            Controller.Emote(1);
        }

        private void EmoteBottomRightBtn_Click(object sender, EventArgs e)
        {
            Controller.Emote(3);
        }

        private void EmoteTopLeftBtn_Click(object sender, EventArgs e)
        {
            Controller.Emote(7);
        }

        private void EmoteBottomLeftBtn_Click(object sender, EventArgs e)
        {
            Controller.Emote(5);
        }

        private void LeftStickBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickLeftStick();
        }

        private void RightStickBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickRightStick();
        }

        private void DupBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickUp();
        }

        private void DrightBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickRight();
        }

        private void DdownBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickDown();
        }

        private void DleftBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickLeft();
        }

        private void LBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickL();
        }

        private void RBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickR();
        }

        private void ZLBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickZL();
        }

        private void ZRBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickZR();
        }

        private void MinusBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickMINUS();
        }

        private void PlusBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickPLUS();
        }

        private void CaputureBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickCAPTURE();
        }

        private void HomeBtn_Click(object sender, EventArgs e)
        {
            Controller.ClickHOME();
        }

        private void RstickMouseUp(object sender, MouseEventArgs e)
        {
            Controller.ResetRightStick();
        }

        private void RstickUPBtn_MouseDown(object sender, MouseEventArgs e)
        {
            Controller.RstickUp();
        }

        private void RstickRIGHTBtn_MouseDown(object sender, MouseEventArgs e)
        {
            Controller.RstickRight();
        }

        private void RstickDOWNBtn_MouseDown(object sender, MouseEventArgs e)
        {
            Controller.RstickDown();
        }

        private void RstickLEFTBtn_MouseDown(object sender, MouseEventArgs e)
        {
            Controller.RstickLeft();
        }

        private void DetachBtn_Click(object sender, EventArgs e)
        {
            Controller.DetachController();
        }

        private void DoneAnchor0Btn_Click(object sender, EventArgs e)
        {
            Teleport.TeleportToAnchor(0);
        }

        private void DoneAnchor1Btn_Click(object sender, EventArgs e)
        {
            Teleport.TeleportToAnchor(1);
        }

        private void DoneAnchor2Btn_Click(object sender, EventArgs e)
        {
            Teleport.TeleportToAnchor(2);
        }

        #endregion

        private void IdleEmoteCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (idleEmoteCheckBox.Checked)
                idleEmote = true;
            else
                idleEmote = false;
        }

        private void DropItemBox_CheckedChanged(object sender, EventArgs e)
        {
            if (dropItemBox.Checked)
                dropItem = true;
            else
                dropItem = false;
        }

        private void InjectVillagerBox_CheckedChanged(object sender, EventArgs e)
        {
            if (injectVillagerBox.Checked)
                injectVillager = true;
            else
                injectVillager = false;
        }

        private void RestoreDodobox_CheckedChanged(object sender, EventArgs e)
        {
            if (restoreDodobox.Checked)
                restoreDodo = true;
            else
                restoreDodo = false;
        }

        private void AbortBtn_Click(object sender, EventArgs e)
        {
            if (standaloneThread != null)
            {
                cts.Cancel();

                standaloneStart.Text = "Start";
                standaloneStart.Tag = "Start";
                standaloneRunning = false;
                standaloneThread = null;
            }

            this.AbortAll();

            WriteLog(">> Restore Sequence Aborted! <<");
            UnLockControl();
            dodoCode.Text = "";
            onlineLabel.Text = "";
            idleNum = 0;
            wasLoading = false;
            lastOrderIsRecipe = false;
        }

        private void TwitchBtn_Click(object sender, EventArgs e)
        {
            TwitchBotUserName = Utilities.GetJsonSetting(Utilities.TwitchSettingPath, "TwitchBotUserName");
            TwitchBotOauth = Utilities.GetJsonSetting(Utilities.TwitchSettingPath, "TwitchBotOauth");
            TwitchChannelName = Utilities.GetJsonSetting(Utilities.TwitchSettingPath, "TwitchChannelName");
            TwitchChannelAccessToken = Utilities.GetJsonSetting(Utilities.TwitchSettingPath, "TwitchChannelAccessToken");

            if (TwitchBotUserName.Equals(string.Empty) || TwitchBotOauth.Equals(string.Empty) || TwitchChannelName.Equals(string.Empty) || TwitchChannelAccessToken.Equals(string.Empty))
            {
                MyMessageBox.Show("Invalid Twitch setting!", "Error Code: 2124-4007", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TwitchChannelid = Utilities.getChannelId(TwitchChannelName);

            if (TwitchChannelid.Equals(string.Empty))
            {
                MyMessageBox.Show("Unable to retrieve your channel ID!", "Error Code: 2181-4008", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TwitchBtn.Enabled = false;
            itemDisplayBtn.Enabled = true;
            WriteLog("--------------------------------------------------------------------------------------------");
            MyTwitchBot = new TwitchBot(TwitchBotUserName, TwitchBotOauth, TwitchChannelName);
            WriteLog($"Your chat bot name is {TwitchBotUserName}", true);
            WriteLog($"Your channel name is {TwitchChannelName}", true);
            WriteLog($"Check your Twitch chat for the start up message!", true);

            MyPubSub = new PubSub(MyTwitchBot, TwitchChannelid, TwitchChannelAccessToken, ref dodoLog);
            WriteLog($"Your channel ID is {TwitchChannelid}", true);
            WriteLog("--------------------------------------------------------------------------------------------");
            WriteLog($"Create/Edit a \"Custom Rewards\" on Twitch to get the ID!", true);
        }

        private void ItemDisplayBtn_Click(object sender, EventArgs e)
        {
            if (itemDisplay == null)
            {
                itemDisplay = new OrderDisplay();
                itemDisplay.Show();
                itemDisplay.Location = new Point(this.Location.X + 290, this.Location.Y - 160);
            }
            else
            {
                itemDisplay.Close();
                itemDisplay = null;
            }
        }

        private void StandaloneStart_Click(object sender, EventArgs e)
        {
            if (standaloneStart.Tag.ToString().Equals("Start"))
            {
                standaloneStart.Text = "Stop";
                standaloneStart.Tag = "Stop";
                standaloneRunning = true;

                cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                standaloneThread = new Thread(delegate () { StandaloneLoop(token); });
                standaloneThread.Start();
            }
            else
            {
                standaloneStart.Text = "Start";
                standaloneStart.Tag = "Start";
                standaloneRunning = false;
                standaloneThread = null;
            }
        }

        private static int GetVisitorNumber()
        {
            string[] namelist = new string[8];
            int num = 0;
            for (int i = 0; i < 8; i++)
            {
                if (i == 0)
                    continue;
                namelist[i] = Utilities.GetVisitorName(s, null, i);
                if (!namelist[i].Equals(String.Empty))
                    num++;
            }
            return num;
        }

        private void StandaloneLoop(CancellationToken token)
        {
            bool init = true;
            idleNum = 0;
            string newVisitor;
            string newVisitorIsland;

            do
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                Teleport.OverworldState state = Teleport.GetOverworldState();
                WriteLog(state.ToString() + " " + idleNum, true);

                if (CheckOnlineStatus() == true)
                {
                    if (init)
                    {
                        DisplayDodo(Controller.SetupDodo());
                        init = false;
                    }

                    if (state == Teleport.OverworldState.Loading || state == Teleport.OverworldState.UserArriveLeavingOrTitleScreen)
                    {
                        idleNum = 0;
                        wasLoading = true;

                        newVisitor = GetVisitorName();
                        newVisitorIsland = GetVisitorIslandName();

                        if (!newVisitor.Equals(string.Empty))
                        {
                            CreateLog(newVisitor);
                            WriteLog("Visitor: "+newVisitor+ " Island: "+newVisitorIsland, true);
							/* uncomment if you want to take pics of visitor's pretty faces lol
                            //capture visitor's arrival lol
                            Thread.Sleep(300);
                            Controller.ClickCAPTURE();
                            Thread.Sleep(300);
                            Controller.ClickCAPTURE();
                            Thread.Sleep(300);
                            Controller.ClickCAPTURE();
                            Thread.Sleep(300);
                            Controller.ClickCAPTURE();
                            Thread.Sleep(300);
                            Controller.ClickCAPTURE();
                            Thread.Sleep(300);
                            Controller.ClickCAPTURE();
                            Thread.Sleep(300);
                            Controller.ClickCAPTURE();
                            Thread.Sleep(300);
                            Controller.ClickCAPTURE();
                            Thread.Sleep(300);
                            Controller.ClickCAPTURE();
                            WriteLog("Took Captures", true);
							*/
                            Thread.Sleep(70000);
                            Utilities.sendBlankName(s);
                            state = Teleport.GetOverworldState();
                        }
                    }

                    if (stopWatch != null)
                    {
                        int visitorNum = GetVisitorNumber();
                        if (resetSession && stopWatch.IsDone())
                        {
                            if (visitorNum >= 1)
                            {
                                WriteLog("Time's up! Resetting session!", true);
                                EndSession(token);
                                stopWatch.done = false;
                                continue;
                            }
                            else
                            {
                                WriteLog("Time's up! Save and reboot!", true);
                                EndSession(token);
                                stopWatch.done = false;
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (restoreDodo)
                    {
                        WriteLog("[Warning] Disconnected.", true);
                        WriteLog("Please wait a moment for the restore.", true);
                        LockControl();

                        int retry = 0;
                        do
                        {
                            if (token.IsCancellationRequested)
                            {
                                return;
                            }

                            if (retry >= 30)
                            {
                                WriteLog("[Warning] Start Hard Restore", true);
                                HardRestore(token);
                                break;
                            }
                            if (Teleport.GetLocationState() == Teleport.LocationState.Announcement)
                            {
                                WriteLog("In Announcement", true);
                                if (!HoldingL)
                                {
                                    Controller.PressL();
                                    HoldingL = true;
                                }
                                retry = 0;
                            }
                            else
                            {
                                WriteLog("Waiting for Overworld", true);
                            }
                            Controller.ClickA();
                            Thread.Sleep(3000);
                            retry++;
                        }
                        while (Teleport.GetOverworldState() != Teleport.OverworldState.OverworldOrInAirport);

                        //Thread.Sleep(2000);
                        Controller.ReleaseL();
                        HoldingL = false;
                        WriteLog("[Warning] Start Normal Restore", true);
                        WriteLog("Please wait for the bot to finish the sequence.", true);
                        NormalRestore(token);

                        UnLockControl();

                        if (stopWatch != null && resetSession)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                stopWatch.Reset();
                                stopWatch.Start();
                            });
                        }

                        WriteLog("Restore sequence finished.", true);
                    }
                }

                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (state != Teleport.OverworldState.Loading && state != Teleport.OverworldState.UserArriveLeavingOrTitleScreen)
                {
                    if (MyPubSub != null)
                    {
                        if (dropItem)
                        {
                            if (idleNum >= 2)
                            {
                                if (wasLoading)
                                {
                                    if (Utilities.hasItemInFirstSlot(s))
                                    {
                                        if (lastOrderIsRecipe)
                                            Controller.DropRecipe();
                                        else
                                            Controller.DropItem();
                                    }
                                    wasLoading = false;
                                }

                                if (PubSub.DropOrderList.Count <= 0)
                                    Debug.Print("No Item Drop Order");
                                else
                                {
                                    _ = DropItem(PubSub.DropOrderList.ElementAt(0));
                                    if (PubSub.DropOrderList.Count > 0)
                                        state = Teleport.OverworldState.ItemDropping;
                                }
                            }

                            if (PubSub.DropOrderList.Count > 0)
                                state = Teleport.OverworldState.ItemDropping;
                        }

                        if (injectVillager)
                        {
                            if (PubSub.VillagerOrderList.Count <= 0)
                                Debug.Print("No Villager Order");
                            else if (state != Teleport.OverworldState.ItemDropping && idleNum >= 2)
                            {
                                _ = InjectVillager(PubSub.VillagerOrderList.ElementAt(0));
                            }
                        }
                    }

                    if (idleEmote && state == Teleport.OverworldState.OverworldOrInAirport)
                    {
                        if (idleNum >= 10 && idleNum % 10 == 0)
                        {
                            Random random = new();
                            int v = random.Next(0, 8);
                            Controller.Emote(v);
                        }
                    }
                }

                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (wasLoading)
                {
                    GetVisitorList();
                }
                idleNum++;

                Thread.Sleep(2000);

            } while (standaloneRunning);
        }

        private static string GetVisitorName()
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

        private static string GetVisitorIslandName()
        {
            byte[] b = Utilities.getVisitorIslandName(s);
            if (b == null)
            {
                return string.Empty;
            }
            //Debug.Print("Byte :   " +Utilities.ByteToHexString(b));
            string tempName = Encoding.Unicode.GetString(b, 0, 20);
            return tempName.Replace("\0", string.Empty);
        }

        private static void GetVisitorList()
        {
            string[] namelist = new string[8];
            int num = 0;
            using StreamWriter sw = File.CreateText(Utilities.CurrentVisitorPath);
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
            //Debug.Print("Visitor Update");
        }

        private static void CreateLog(string newVisitor)
        {
            if (!File.Exists(Utilities.VisitorLogPath))
            {
                string logheader = "Timestamp" + "," + "Name";

                using StreamWriter sw = File.CreateText(Utilities.VisitorLogPath);
                sw.WriteLine(logheader);
            }

            DateTime localDate = DateTime.Now;
            string newLog = localDate.ToString() + "," + newVisitor;

            using (StreamWriter sw = File.AppendText(Utilities.VisitorLogPath))
            {
                sw.WriteLine(newLog);
            }
        }

        private void StopWatchBtn_Click(object sender, EventArgs e)
        {
            if (this.Height < 440)
            {
                this.Height = 440;
                if (stopWatch == null)
                {
                    stopWatch = new MyStopWatch();
                    stopWatch.Show();
                    timerSettingPanel.Enabled = true;
                    stopWatch.Location = new Point(this.Location.X, this.Location.Y - 140);
                    stopWatch.ControlBox = false;
                }
            }
            else
            {
                this.Height = 330;
                if (stopWatch != null)
                {
                    stopWatch.Close();
                    stopWatch.Dispose();
                    stopWatch = null;
                }
            }
        }

        private void Dodo_Move(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.Location = new Point(this.Location.X, this.Location.Y - 140);
                stopWatch.BringToFront();
            }

            if (itemDisplay != null)
            {
                itemDisplay.Location = new Point(this.Location.X + 290, this.Location.Y - 160);
                itemDisplay.BringToFront();
            }
        }

        #region Timer Control
        private void Min1Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.Set(1, stopWatch.seconds);
            }
        }

        private void Min3Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.Set(3, stopWatch.seconds);
            }
        }

        private void Min5Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.Set(5, stopWatch.seconds);
            }
        }

        private void Min10Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.Set(10, stopWatch.seconds);
            }
        }

        private void Min15Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.Set(15, stopWatch.seconds);
            }
        }

        private void Sce0Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.Set(stopWatch.minutes, 0);
            }
        }

        private void Sce30Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.Set(stopWatch.minutes, 30);
            }
        }

        private void MinMinus1Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.MinusMin(1);
            }
        }

        private void MinPlus1Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.AddMin(1);
            }
        }

        private void MinMinus5Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.MinusMin(5);
            }
        }

        private void MinPlus5Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.AddMin(5);
            }
        }

        private void SecMinus1Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.MinusSec(1);
            }
        }

        private void SecPlus1Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.AddSec(1);
            }
        }

        private void SecMinus5Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.MinusSec(5);
            }
        }

        private void SecPlus5Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.AddSec(5);
            }
        }

        private void TimerStartBtn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                timerSettingPanel.Enabled = false;
                stopWatch.Start();
            }
        }

        private void TimerPauseBtn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.Pause();
            }
        }

        private void TimerResetBtn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                timerSettingPanel.Enabled = true;
                stopWatch.Reset();
            }
        }
        #endregion

        #region Keyboard Controller
        private void Dodo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
                Controller.ClickMINUS();
            else if (e.KeyCode == Keys.Y)
                Controller.ClickPLUS();
            else if (e.KeyCode == Keys.Q)
                Controller.ClickZL();
            else if (e.KeyCode == Keys.O)
                Controller.ClickZR();

            if (e.KeyCode == Keys.I)
                I = true;
            if (e.KeyCode == Keys.K)
                K = true;
            if (e.KeyCode == Keys.J)
                J = true;
            if (e.KeyCode == Keys.L)
                L = true;
            if (e.KeyCode == Keys.W)
                W = true;
            if (e.KeyCode == Keys.S)
                S = true;
            if (e.KeyCode == Keys.A)
                A = true;
            if (e.KeyCode == Keys.D)
                D = true;
        }

        private void Dodo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.I)
                I = false;
            if (e.KeyCode == Keys.K)
                K = false;
            if (e.KeyCode == Keys.J)
                J = false;
            if (e.KeyCode == Keys.L)
                L = false;
            if (e.KeyCode == Keys.W)
                W = false;
            if (e.KeyCode == Keys.S)
                S = false;
            if (e.KeyCode == Keys.A)
                A = false;
            if (e.KeyCode == Keys.D)
                D = false;
        }

        private void ControllerTimer_Tick(object sender, EventArgs e)
        {
            if (!W && !A && !S && !D)
            {
                if (!resetted)
                {
                    currentDirection = MovingDirection.Null;
                    resetted = true;
                    //Debug.Print("Reset Stick");
                    Controller.ResetLeftStick();
                }
            }
            else if (W && D)
            {
                if (currentDirection != MovingDirection.UpRight)
                {
                    currentDirection = MovingDirection.UpRight;
                    resetted = false;
                    //Debug.Print("TopRight");
                    Controller.LstickTopRight();
                }
            }
            else if (W && A)
            {
                if (currentDirection != MovingDirection.UpLeft)
                {
                    currentDirection = MovingDirection.UpLeft;
                    resetted = false;
                    //Debug.Print("TopLeft");
                    Controller.LstickTopLeft();
                }
            }
            else if (W)
            {
                if (currentDirection != MovingDirection.Up)
                {
                    currentDirection = MovingDirection.Up;
                    resetted = false;
                    //Debug.Print("Up");
                    Controller.LstickUp();
                }
            }
            else if (S && D)
            {
                if (currentDirection != MovingDirection.DownRight)
                {
                    currentDirection = MovingDirection.DownRight;
                    resetted = false;
                    //Debug.Print("BottomRight");
                    Controller.LstickBottomRight();
                }
            }
            else if (S && A)
            {
                if (currentDirection != MovingDirection.DownLeft)
                {
                    currentDirection = MovingDirection.DownLeft;
                    resetted = false;
                    //Debug.Print("BottomLeft");
                    Controller.LstickBottomLeft();
                }
            }
            else if (S)
            {
                if (currentDirection != MovingDirection.Down)
                {
                    currentDirection = MovingDirection.Down;
                    resetted = false;
                    //Debug.Print("Down");
                    Controller.LstickDown();
                }
            }
            else if (A)
            {
                if (currentDirection != MovingDirection.Left)
                {
                    currentDirection = MovingDirection.Left;
                    resetted = false;
                    //Debug.Print("Left");
                    Controller.LstickLeft();
                }
            }
            else if (D)
            {
                if (currentDirection != MovingDirection.Right)
                {
                    currentDirection = MovingDirection.Right;
                    resetted = false;
                    //Debug.Print("Right");
                    Controller.LstickRight();
                }
            }

            if (I)
            {
                if (!holdingI)
                {
                    holdingI = true;
                    //Debug.Print("Press X");
                    Controller.PressX();
                }
            }
            else if (!I)
            {
                if (holdingI)
                {
                    holdingI = false;
                    //Debug.Print("Release X");
                    Controller.ReleaseX();
                }
            }

            if (J)
            {
                if (!holdingJ)
                {
                    holdingJ = true;
                    //Debug.Print("Press Y");
                    Controller.PressY();
                }
            }
            else if (!J)
            {
                if (holdingJ)
                {
                    holdingJ = false;
                    //Debug.Print("Release Y");
                    Controller.ReleaseY();
                }
            }

            if (K)
            {
                if (!holdingK)
                {
                    holdingK = true;
                    //Debug.Print("Press B");
                    Controller.PressB();
                }
            }
            else if (!K)
            {
                if (holdingK)
                {
                    holdingK = false;
                    //Debug.Print("Release B");
                    Controller.ReleaseB();
                }
            }

            if (L)
            {
                if (!holdingL)
                {
                    holdingL = true;
                    //Debug.Print("Press A");
                    Controller.PressA();
                }
            }
            else if (!L)
            {
                if (holdingL)
                {
                    holdingL = false;
                    //Debug.Print("Release A");
                    Controller.ReleaseA();
                }
            }
        }
        #endregion

        private void ClearInvBtn_Click(object sender, EventArgs e)
        {
            Utilities.DeleteSlot(s, null, 0);
            WriteLog("First inventory slot cleared!", true);
            WriteLog("Please remember to reset your cursor to the first inventory slot for the drop bot to function properly!", true);
        }

        private void SessionBox_CheckedChanged(object sender, EventArgs e)
        {
            resetSession = sessionBox.Checked;
        }
    }
}
