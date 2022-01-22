using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Twitch;

namespace ACNHPokerCore
{
    public partial class dodo : Form
    {
        private static Socket s;
        public Boolean dodoSetupDone = false;
        private Boolean idleEmote = false;
        private int idleNum = 0;
        private Boolean HoldingL = false;
        private PubSub? MyPubSub = null;
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

        public event CloseHandler closeForm;
        public event ThreadAbortHandler abortAll;

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

        public dodo(Socket S, bool standaloneMode = false)
        {
            InitializeComponent();
            s = S;

            if (teleport.allAnchorValid())
            {
                Point Done = new Point(-4200, 0);
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
            resetBtn();
            Point page1 = new Point(-600, 0);
            FullPanel.Location = page1;
        }

        private void Anchor0NextBtn_Click(object sender, EventArgs e)
        {
            resetBtn();
            Point page2 = new Point(-1200, 0);
            FullPanel.Location = page2;
        }

        private void Anchor1NextBtn_Click(object sender, EventArgs e)
        {
            resetBtn();
            Point page3 = new Point(-1800, 0);
            FullPanel.Location = page3;
        }

        private void Anchor2NextBtn_Click(object sender, EventArgs e)
        {
            resetBtn();
            Point page4 = new Point(-2400, 0);
            FullPanel.Location = page4;
        }

        private void Anchor3NextBtn_Click(object sender, EventArgs e)
        {
            resetBtn();
            Point page5 = new Point(-3000, 0);
            FullPanel.Location = page5;
        }

        private void Anchor4NextBtn_Click(object sender, EventArgs e)
        {
            resetBtn();
            Point page6 = new Point(-3600, 0);
            FullPanel.Location = page6;
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            if (teleport.allAnchorValid())
            {
                resetBtn();
                Point Done = new Point(-4200, 0);
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
            resetBtn();
            Point Start = new Point(0, 0);
            FullPanel.Location = Start;
        }

        private void Anchor1PreviousBtn_Click(object sender, EventArgs e)
        {
            resetBtn();
            Point page1 = new Point(-600, 0);
            FullPanel.Location = page1;
        }

        private void Anchor2PreviousBtn_Click(object sender, EventArgs e)
        {
            resetBtn();
            Point page2 = new Point(-1200, 0);
            FullPanel.Location = page2;
        }

        private void Anchor3PreviousBtn_Click(object sender, EventArgs e)
        {
            resetBtn();
            Point page3 = new Point(-1800, 0);
            FullPanel.Location = page3;
        }

        private void Anchor4PreviousBtn_Click(object sender, EventArgs e)
        {
            resetBtn();
            Point page4 = new Point(-2400, 0);
            FullPanel.Location = page4;
        }

        private void DonePreviousBtn_Click(object sender, EventArgs e)
        {
            resetBtn();
            Point page5 = new Point(-3000, 0);
            FullPanel.Location = page5;
        }

        private void Anchor0SetBtn_Click(object sender, EventArgs e)
        {
            teleport.SetAnchor(0);
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

            Thread TeleportThread = new Thread(delegate () { TestTeleport(0, btn); });
            TeleportThread.Start();
        }

        private void Anchor1SetBtn_Click(object sender, EventArgs e)
        {
            teleport.SetAnchor(1);
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

            Thread TeleportThread = new Thread(delegate () { TestTeleport(1, btn); });
            TeleportThread.Start();
        }

        private void Anchor2SetBtn_Click(object sender, EventArgs e)
        {
            teleport.SetAnchor(2);
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

            Thread TeleportThread = new Thread(delegate () { TestTeleport(2, btn); });
            TeleportThread.Start();
        }

        private void Anchor3SetBtn_Click(object sender, EventArgs e)
        {
            teleport.SetAnchor(3);
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

            Thread TeleportThread = new Thread(delegate () { TestTeleport(3, btn); });
            TeleportThread.Start();
        }

        private void Anchor4SetBtn_Click(object sender, EventArgs e)
        {
            teleport.SetAnchor(4);
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

            Thread TeleportThread = new Thread(delegate () { TestTeleport(4, btn); });
            TeleportThread.Start();
        }

        private void DoneAnchor0TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new Thread(delegate () { TestTeleport(0, btn); });
            TeleportThread.Start();
        }

        private void DoneAnchor1TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new Thread(delegate () { TestTeleport(1, btn); });
            TeleportThread.Start();
        }

        private void DoneAnchor2TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new Thread(delegate () { TestTeleport(2, btn); });
            TeleportThread.Start();
        }

        private void DoneAnchor3TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new Thread(delegate () { TestTeleport(3, btn); });
            TeleportThread.Start();
        }

        private void DoneAnchor4TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            btn.Enabled = false;

            Thread TeleportThread = new Thread(delegate () { TestTeleport(4, btn); });
            TeleportThread.Start();
        }

        private void TestTeleport(int num, Button btn)
        {
            if (teleport.TeleportToAnchor(num))
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

        private void resetBtn()
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
            Thread TeleportThread = new Thread(delegate () { TestNormalRestore(token); });
            TeleportThread.Start();
        }

        private void TestNormalRestore(CancellationToken token)
        {
            controller.clickDown(); // Hide Weapon
            Thread.Sleep(1000);

            teleport.TeleportToAnchor(2);

            Debug.Print("Teleport to Airport");

            do
            {
                //Debug.Print(teleport.GetOverworldState().ToString());
                Debug.Print("Try Enter Airport");
                controller.EnterAirport();
                Thread.Sleep(2000);
            }
            while (teleport.GetOverworldState() != teleport.OverworldState.OverworldOrInAirport);

            Debug.Print("Inside Airport");
            Thread.Sleep(2000);

            teleport.TeleportToAnchor(3);

            Debug.Print("Get Dodo");
            controller.talkAndGetDodoCode(token);
            Debug.Print("Finish getting Dodo");

            teleport.TeleportToAnchor(4);

            do
            {
                //Debug.Print(teleport.GetOverworldState().ToString());
                Debug.Print("Try Exit Airport");
                controller.ExitAirport();
                Thread.Sleep(2000);
            }
            while (teleport.GetOverworldState() != teleport.OverworldState.OverworldOrInAirport);

            Debug.Print("Back to Overworld");
            Thread.Sleep(2000);

            teleport.TeleportToAnchor(1);

            controller.emote(0);

            controller.detachController();

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

        private void dodo_FormClosed(object sender, FormClosedEventArgs e)
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

            this.closeForm();
        }

        private void BackToSetupBtn_Click(object sender, EventArgs e)
        {
            resetBtn();
            Point Start = new Point(0, 0);
            FullPanel.Location = Start;
        }

        public int CheckOnlineStatus()
        {
            if (teleport.checkOnlineStatus() == 0x1)
            {
                onlineLabel.Invoke((MethodInvoker)delegate
                {
                    onlineLabel.Text = "ONLINE";
                    onlineLabel.ForeColor = Color.Green;
                });
                return 1;
            }
            else if (teleport.checkOnlineStatus(true) == 0x1) //Check again for Chinese
            {
                onlineLabel.Invoke((MethodInvoker)delegate
                {
                    onlineLabel.Text = "ONLINE";
                    onlineLabel.ForeColor = Color.Lime;
                });
                return 1;
            }
            else
            {
                onlineLabel.Invoke((MethodInvoker)delegate
                {
                    onlineLabel.Text = "OFFLINE";
                    onlineLabel.ForeColor = Color.Red;
                });
                return 0;
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

        private void unLockControl()
        {
            this.Invoke((MethodInvoker)delegate
            {
                controllerPanel.Enabled = true;
                functionPanel.Enabled = true;
                AbortBtn.Visible = false;
            });
        }

        public teleport.OverworldState DodoMonitor(CancellationToken token)
        {
            teleport.OverworldState state = teleport.GetOverworldState();
            //WriteLog(state.ToString() + " " + idleNum, true);

            if (token.IsCancellationRequested)
                return state;

            if (CheckOnlineStatus() == 1)
            {
                if (state == teleport.OverworldState.Loading || state == teleport.OverworldState.UserArriveLeavingOrTitleScreen)
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
                        if (teleport.GetLocationState() == teleport.LocationState.Announcement)
                        {
                            WriteLog("In Announcement", true);
                            if (!HoldingL)
                            {
                                controller.pressL();
                                HoldingL = true;
                            }
                            retry = 0;
                        }
                        else
                        {
                            WriteLog("Waiting for Overworld", true);
                        }
                        controller.clickA();
                        Thread.Sleep(3000);
                        retry++;
                    }
                    while (teleport.GetOverworldState() != teleport.OverworldState.OverworldOrInAirport);

                    //Thread.Sleep(2000);
                    controller.releaseL();
                    HoldingL = false;
                    WriteLog("[Warning] Start Normal Restore", true);
                    WriteLog("Please wait for the bot to finish the sequence.", true);
                    NormalRestore(token);
                    unLockControl();
                    WriteLog("Restore sequence finished.", true);
                    idleNum = 0;
                    state = teleport.OverworldState.OverworldOrInAirport;
                }
            }

            if (token.IsCancellationRequested)
                return state;

            if (state != teleport.OverworldState.Loading && state != teleport.OverworldState.UserArriveLeavingOrTitleScreen)
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
                                        controller.dropRecipe();
                                    else
                                        controller.dropItem();
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
                                    state = teleport.OverworldState.ItemDropping;
                            }
                        }

                        if (PubSub.DropOrderList.Count > 0)
                            state = teleport.OverworldState.ItemDropping;
                    }

                    if (injectVillager)
                    {
                        if (PubSub.VillagerOrderList.Count <= 0)
                        {
                            Debug.Print("No Villager Order");
                        }
                        else if (state != teleport.OverworldState.ItemDropping && idleNum >= 2)
                        {
                            _ = InjectVillager(PubSub.VillagerOrderList.ElementAt(0));
                        }
                    }
                }

                if (idleEmote && state == teleport.OverworldState.OverworldOrInAirport)
                {
                    if (idleNum >= 10 && idleNum % 10 == 0)
                    {
                        Random random = new Random();
                        int v = random.Next(0, 8);
                        controller.emote(v);
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
                if (teleport.GetLocationState() == teleport.LocationState.Announcement)
                {
                    WriteLog("In Announcement", true);
                    if (!HoldingL)
                    {
                        controller.pressL();
                        HoldingL = true;
                    }
                }
                else
                {
                    WriteLog("Confirming Overworld", true);
                }
                controller.clickA();
                Thread.Sleep(3000);
            }
            while (teleport.GetOverworldState() != teleport.OverworldState.OverworldOrInAirport);

            controller.releaseL();
            HoldingL = false;
            Thread.Sleep(2000);
            controller.clickDown(); // Hide Weapon
            Thread.Sleep(1000);
            if (token.IsCancellationRequested)
                return;

            string locationState = teleport.GetLocationState().ToString();
            //Debug.Print(">>>>>>>>>>>>>>>>>>>>>>>>>>" + locationState);

            if (teleport.GetLocationState() == teleport.LocationState.Indoor)
            {
                WriteLog("Indoor Detected", true);
            }
            else
            {
                teleport.TeleportToAnchor(2);
                WriteLog("Teleport to Airport", true);

                do
                {
                    if (token.IsCancellationRequested)
                        return;
                    WriteLog("Try Entering Airport", true);
                    controller.EnterAirport();
                    Thread.Sleep(2000);
                }
                while (teleport.GetOverworldState() != teleport.OverworldState.OverworldOrInAirport);

                WriteLog("Inside Airport", true);
                Thread.Sleep(2000);
            }

            teleport.TeleportToAnchor(3);

            WriteLog("Try Getting Dodo", true);
            if (skipDialogCheckBox.Checked)
                DisplayDodo(controller.talkAndGetDodoCode(token));
            else
                DisplayDodo(controller.talkAndGetDodoCodeLegacy(token));
            WriteLog("Finish Getting Dodo", true);

            if (token.IsCancellationRequested)
                return;

            CheckOnlineStatus();

            teleport.TeleportToAnchor(4);

            do
            {
                if (token.IsCancellationRequested)
                    return;
                //Debug.Print(teleport.GetOverworldState().ToString());
                WriteLog("Try Exiting Airport", true);
                controller.ExitAirport();
                Thread.Sleep(2000);
            }
            while (teleport.GetOverworldState() != teleport.OverworldState.OverworldOrInAirport);

            WriteLog("Back to Overworld", true);
            Thread.Sleep(2000);

            if (token.IsCancellationRequested)
                return;
            teleport.TeleportToAnchor(1);

            controller.emote(0);

            //controller.detachController();
        }

        public void HardRestore(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            WriteLog("Capturing the Crash", true);
            controller.clickCAPTURE();
            Thread.Sleep(2000);
            if (token.IsCancellationRequested)
                return;
            WriteLog("Open Home Menu", true);
            controller.clickHOME();
            Thread.Sleep(5000);
            if (token.IsCancellationRequested)
                return;
            controller.clickX();
            Thread.Sleep(1000);
            if (token.IsCancellationRequested)
                return;
            controller.clickA(); //Close Game
            WriteLog("Try Closing the Game", true);
            Thread.Sleep(15000);
            if (token.IsCancellationRequested)
                return;
            controller.clickA(); //Select Game
            Thread.Sleep(2000);
            if (token.IsCancellationRequested)
                return;
            controller.clickA(); //Select first user
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
                if (teleport.GetLocationState() == teleport.LocationState.Announcement)
                {
                    WriteLog("In Announcement", true);
                    if (!HoldingL)
                    {
                        controller.pressL();
                        HoldingL = true;
                    }
                }
                controller.clickA();
                Thread.Sleep(2000);
                retry++;
            }
            while (teleport.GetOverworldState() != teleport.OverworldState.OverworldOrInAirport);

            controller.releaseL();
            HoldingL = false;
            WriteLog("Exiting House", true);
            Thread.Sleep(2000);
            if (token.IsCancellationRequested)
                return;
        }

        private void CloseGate()
        {
            controller.clickDown(); // Hide Weapon
            Thread.Sleep(1000);

            teleport.TeleportToAnchor(2);

            Debug.Print("Teleport to Airport");

            do
            {
                //Debug.Print(teleport.GetOverworldState().ToString());
                Debug.Print("Try Enter Airport");
                controller.EnterAirport();
                Thread.Sleep(2000);
            }
            while (teleport.GetOverworldState() != teleport.OverworldState.OverworldOrInAirport);

            Debug.Print("Inside Airport");
            Thread.Sleep(2000);

            teleport.TeleportToAnchor(3);

            Debug.Print("Close Gate");
            controller.talkAndCloseGate();
            Debug.Print("Finish Close Gate");
            CheckOnlineStatus();

            teleport.TeleportToAnchor(4);

            do
            {
                //Debug.Print(teleport.GetOverworldState().ToString());
                Debug.Print("Try Exit Airport");
                controller.ExitAirport();
                Thread.Sleep(2000);
            }
            while (teleport.GetOverworldState() != teleport.OverworldState.OverworldOrInAirport);

            Debug.Print("Back to Overworld");
            Thread.Sleep(2000);

        }

        public void EndSession(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            controller.clickDown(); // Hide Weapon
            Thread.Sleep(1000);
            if (token.IsCancellationRequested)
                return;
            controller.clickMINUS(); // Open menu
            Thread.Sleep(2000);
            if (token.IsCancellationRequested)
                return;
            controller.clickA(); // Open Selection
            Thread.Sleep(1000);
            if (token.IsCancellationRequested)
                return;
            controller.clickA(); // Select End Session.
            Thread.Sleep(10000);
        }

        private async Task DropItem(ItemOrder CurrentOrder)
        {
            string flag1;
            if (CurrentOrder.id == "16A2" || CurrentOrder.name.Contains("wrapping paper") || CurrentOrder.id == "3107" || CurrentOrder.id == "3106") // Vine Glowing Moss
                flag1 = "00";
            else
                flag1 = "7F";

            string flag2 = "00";

            if (itemDisplay != null)
            {
                string path = Main.GetImagePathFromID(CurrentOrder.id, Convert.ToUInt32("0x" + Utilities.precedingZeros(CurrentOrder.count, 8), 16));
                if (File.Exists(path))
                {
                    Image image = Image.FromFile(path);
                    itemDisplay.setItemdisplay(image);
                }
            }

            Utilities.SpawnItem(s, null, 0, flag1 + flag2 + CurrentOrder.id, Utilities.precedingZeros(CurrentOrder.count, 8));
            //Thread.Sleep(500);
            if (CurrentOrder.id == "16A2" || CurrentOrder.id == "3107" || CurrentOrder.id == "3106")
            {
                controller.dropRecipe();
                lastOrderIsRecipe = true;
            }
            else
            {
                controller.dropItem();
                lastOrderIsRecipe = false;
            }
            if (!CurrentOrder.color.Equals(string.Empty))
                await MyTwitchBot.SendMessage($"{CurrentOrder.owner}, your order of \"{CurrentOrder.name}\" ({CurrentOrder.color}) have been dropped.");
            else
                await MyTwitchBot.SendMessage($"{CurrentOrder.owner}, your order of \"{CurrentOrder.name}\" have been dropped.");

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

                await MyTwitchBot.SendMessage($"{CurrentOrder.owner}, \"{CurrentOrder.RealName}\" is already waiting for you on the island.");

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
                Buffer.BlockCopy(Main.getHeader(), 0x0, modifiedVillager, 0x4, 52);

                byte[] modifiedHouse = houseData;

                byte h = (Byte)villagerIndex;
                modifiedHouse[Utilities.VillagerHouseOwnerOffset] = h;

                await Utilities.loadBoth(s, villagerIndex, villagerData, houseIndex, houseData);
                await Utilities.SetMoveout(s, villagerIndex, "2", "0");

                await MyTwitchBot.SendMessage($"{CurrentOrder.owner}, \"{CurrentOrder.RealName}\" is now waiting for you on the island.");

                PubSub.VillagerOrderList.RemoveAt(0);

                //MapRegenerator.updateVillager(s, villagerIndex);
                WriteLog($"Villager \"{CurrentOrder.RealName}\" loaded!", true);
            }

        }

        private void dodoLog_TextChanged(object sender, EventArgs e)
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
            controller.clickX();
        }

        private void ABtn_Click(object sender, EventArgs e)
        {
            controller.clickA();
        }

        private void BBtn_Click(object sender, EventArgs e)
        {
            controller.clickB();
        }

        private void YBtn_Click(object sender, EventArgs e)
        {
            controller.clickY();
        }

        private void emoteUPBtn_Click(object sender, EventArgs e)
        {
            controller.emote(0);
        }

        private void emoteRIGHTBtn_Click(object sender, EventArgs e)
        {
            controller.emote(2);
        }

        private void emoteDOWNBtn_Click(object sender, EventArgs e)
        {
            controller.emote(4);
        }

        private void emoteLEFTBtn_Click(object sender, EventArgs e)
        {
            controller.emote(6);
        }

        private void emoteTopRightBtn_Click(object sender, EventArgs e)
        {
            controller.emote(1);
        }

        private void emoteBottomRightBtn_Click(object sender, EventArgs e)
        {
            controller.emote(3);
        }

        private void emoteTopLeftBtn_Click(object sender, EventArgs e)
        {
            controller.emote(7);
        }

        private void emoteBottomLeftBtn_Click(object sender, EventArgs e)
        {
            controller.emote(5);
        }

        private void LeftStickBtn_Click(object sender, EventArgs e)
        {
            controller.clickLeftStick();
        }

        private void RightStickBtn_Click(object sender, EventArgs e)
        {
            controller.clickRightStick();
        }

        private void DupBtn_Click(object sender, EventArgs e)
        {
            controller.clickUp();
        }

        private void DrightBtn_Click(object sender, EventArgs e)
        {
            controller.clickRight();
        }

        private void DdownBtn_Click(object sender, EventArgs e)
        {
            controller.clickDown();
        }

        private void DleftBtn_Click(object sender, EventArgs e)
        {
            controller.clickLeft();
        }

        private void LBtn_Click(object sender, EventArgs e)
        {
            controller.clickL();
        }

        private void RBtn_Click(object sender, EventArgs e)
        {
            controller.clickR();
        }

        private void ZLBtn_Click(object sender, EventArgs e)
        {
            controller.clickZL();
        }

        private void ZRBtn_Click(object sender, EventArgs e)
        {
            controller.clickZR();
        }

        private void minusBtn_Click(object sender, EventArgs e)
        {
            controller.clickMINUS();
        }

        private void plusBtn_Click(object sender, EventArgs e)
        {
            controller.clickPLUS();
        }

        private void caputureBtn_Click(object sender, EventArgs e)
        {
            controller.clickCAPTURE();
        }

        private void HomeBtn_Click(object sender, EventArgs e)
        {
            controller.clickHOME();
        }

        private void RstickMouseUp(object sender, MouseEventArgs e)
        {
            controller.resetRightStick();
        }

        private void RstickUPBtn_MouseDown(object sender, MouseEventArgs e)
        {
            controller.RstickUp();
        }

        private void RstickRIGHTBtn_MouseDown(object sender, MouseEventArgs e)
        {
            controller.RstickRight();
        }

        private void RstickDOWNBtn_MouseDown(object sender, MouseEventArgs e)
        {
            controller.RstickDown();
        }

        private void RstickLEFTBtn_MouseDown(object sender, MouseEventArgs e)
        {
            controller.RstickLeft();
        }

        private void DetachBtn_Click(object sender, EventArgs e)
        {
            controller.detachController();
        }

        private void DoneAnchor0Btn_Click(object sender, EventArgs e)
        {
            teleport.TeleportToAnchor(0);
        }

        private void DoneAnchor1Btn_Click(object sender, EventArgs e)
        {
            teleport.TeleportToAnchor(1);
        }

        private void DoneAnchor2Btn_Click(object sender, EventArgs e)
        {
            teleport.TeleportToAnchor(2);
        }

        #endregion

        private void idleEmoteCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (idleEmoteCheckBox.Checked)
                idleEmote = true;
            else
                idleEmote = false;
        }

        private void dropItemBox_CheckedChanged(object sender, EventArgs e)
        {
            if (dropItemBox.Checked)
                dropItem = true;
            else
                dropItem = false;
        }

        private void injectVillagerBox_CheckedChanged(object sender, EventArgs e)
        {
            if (injectVillagerBox.Checked)
                injectVillager = true;
            else
                injectVillager = false;
        }

        private void restoreDodobox_CheckedChanged(object sender, EventArgs e)
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

            this.abortAll();

            WriteLog(">> Restore Sequence Aborted! <<");
            unLockControl();
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

        private void itemDisplayBtn_Click(object sender, EventArgs e)
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

        private void standaloneStart_Click(object sender, EventArgs e)
        {
            if (standaloneStart.Tag.ToString().Equals("Start"))
            {
                standaloneStart.Text = "Stop";
                standaloneStart.Tag = "Stop";
                standaloneRunning = true;

                cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                standaloneThread = new Thread(delegate () { standaloneLoop(token); });
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

        private int GetVisitorNumber()
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

        private void standaloneLoop(CancellationToken token)
        {
            bool init = true;
            do
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                teleport.OverworldState state = teleport.GetOverworldState();
                WriteLog(state.ToString() + " " + idleNum, true);

                if (CheckOnlineStatus() == 1)
                {
                    if (init)
                    {
                        DisplayDodo(controller.setupDodo());
                        init = false;
                    }
                    if (state == teleport.OverworldState.Loading || state == teleport.OverworldState.UserArriveLeavingOrTitleScreen)
                    {
                        idleNum = 0;
                        wasLoading = true;
                    }

                    if (stopWatch != null)
                    {
                        int visitorNum = GetVisitorNumber();
                        if (resetSession && stopWatch.isDone())
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
                            if (teleport.GetLocationState() == teleport.LocationState.Announcement)
                            {
                                WriteLog("In Announcement", true);
                                if (!HoldingL)
                                {
                                    controller.pressL();
                                    HoldingL = true;
                                }
                                retry = 0;
                            }
                            else
                            {
                                WriteLog("Waiting for Overworld", true);
                            }
                            controller.clickA();
                            Thread.Sleep(3000);
                            retry++;
                        }
                        while (teleport.GetOverworldState() != teleport.OverworldState.OverworldOrInAirport);

                        //Thread.Sleep(2000);
                        controller.releaseL();
                        HoldingL = false;
                        WriteLog("[Warning] Start Normal Restore", true);
                        WriteLog("Please wait for the bot to finish the sequence.", true);
                        NormalRestore(token);
                        unLockControl();

                        if (stopWatch != null && resetSession)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                stopWatch.reset();
                                stopWatch.start();
                            });
                        }

                        WriteLog("Restore sequence finished.", true);
                    }
                }

                if (state != teleport.OverworldState.Loading && state != teleport.OverworldState.UserArriveLeavingOrTitleScreen)
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
                                            controller.dropRecipe();
                                        else
                                            controller.dropItem();
                                    }
                                    wasLoading = false;
                                }

                                if (PubSub.DropOrderList.Count <= 0)
                                    Debug.Print("No Item Drop Order");
                                else
                                {
                                    _ = DropItem(PubSub.DropOrderList.ElementAt(0));
                                    if (PubSub.DropOrderList.Count > 0)
                                        state = teleport.OverworldState.ItemDropping;
                                }
                            }

                            if (PubSub.DropOrderList.Count > 0)
                                state = teleport.OverworldState.ItemDropping;
                        }

                        if (injectVillager)
                        {
                            if (PubSub.VillagerOrderList.Count <= 0)
                                Debug.Print("No Villager Order");
                            else if (state != teleport.OverworldState.ItemDropping && idleNum >= 2)
                            {
                                _ = InjectVillager(PubSub.VillagerOrderList.ElementAt(0));
                            }
                        }
                    }

                    if (idleEmote && state == teleport.OverworldState.OverworldOrInAirport)
                    {
                        if (idleNum >= 10 && idleNum % 10 == 0)
                        {
                            Random random = new Random();
                            int v = random.Next(0, 8);
                            controller.emote(v);
                        }
                    }
                }

                if (wasLoading)
                {
                    //MapRegenerator.prepareVillager(s);
                    GetVisitorList();
                }
                idleNum++;

                Thread.Sleep(2000);

            } while (standaloneRunning);
        }

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

        private void dodo_Move(object sender, EventArgs e)
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
        private void min1Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.set(1, stopWatch.seconds);
            }
        }

        private void min3Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.set(3, stopWatch.seconds);
            }
        }

        private void min5Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.set(5, stopWatch.seconds);
            }
        }

        private void min10Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.set(10, stopWatch.seconds);
            }
        }

        private void min15Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.set(15, stopWatch.seconds);
            }
        }

        private void sce0Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.set(stopWatch.minutes, 0);
            }
        }

        private void sce30Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.set(stopWatch.minutes, 30);
            }
        }

        private void minMinus1Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.minusMin(1);
            }
        }

        private void minPlus1Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.addMin(1);
            }
        }

        private void minMinus5Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.minusMin(5);
            }
        }

        private void minPlus5Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.addMin(5);
            }
        }

        private void secMinus1Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.minusSec(1);
            }
        }

        private void secPlus1Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.addSec(1);
            }
        }

        private void secMinus5Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.minusSec(5);
            }
        }

        private void secPlus5Btn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.addSec(5);
            }
        }

        private void timerStartBtn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                timerSettingPanel.Enabled = false;
                stopWatch.start();
            }
        }

        private void timerPauseBtn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                stopWatch.pause();
            }
        }

        private void timerResetBtn_Click(object sender, EventArgs e)
        {
            if (stopWatch != null)
            {
                timerSettingPanel.Enabled = true;
                stopWatch.reset();
            }
        }
        #endregion

        #region Keyboard Controller
        private void dodo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
                controller.clickMINUS();
            else if (e.KeyCode == Keys.Y)
                controller.clickPLUS();
            else if (e.KeyCode == Keys.Q)
                controller.clickZL();
            else if (e.KeyCode == Keys.O)
                controller.clickZR();

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

        private void dodo_KeyUp(object sender, KeyEventArgs e)
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

        private void controllerTimer_Tick(object sender, EventArgs e)
        {
            if (!W && !A && !S && !D)
            {
                if (!resetted)
                {
                    currentDirection = MovingDirection.Null;
                    resetted = true;
                    //Debug.Print("Reset Stick");
                    controller.resetLeftStick();
                }
            }
            else if (W && D)
            {
                if (currentDirection != MovingDirection.UpRight)
                {
                    currentDirection = MovingDirection.UpRight;
                    resetted = false;
                    Debug.Print("TopRight");
                    controller.LstickTopRight();
                }
            }
            else if (W && A)
            {
                if (currentDirection != MovingDirection.UpLeft)
                {
                    currentDirection = MovingDirection.UpLeft;
                    resetted = false;
                    //Debug.Print("TopLeft");
                    controller.LstickTopLeft();
                }
            }
            else if (W)
            {
                if (currentDirection != MovingDirection.Up)
                {
                    currentDirection = MovingDirection.Up;
                    resetted = false;
                    //Debug.Print("Up");
                    controller.LstickUp();
                }
            }
            else if (S && D)
            {
                if (currentDirection != MovingDirection.DownRight)
                {
                    currentDirection = MovingDirection.DownRight;
                    resetted = false;
                    //Debug.Print("BottomRight");
                    controller.LstickBottomRight();
                }
            }
            else if (S && A)
            {
                if (currentDirection != MovingDirection.DownLeft)
                {
                    currentDirection = MovingDirection.DownLeft;
                    resetted = false;
                    //Debug.Print("BottomLeft");
                    controller.LstickBottomLeft();
                }
            }
            else if (S)
            {
                if (currentDirection != MovingDirection.Down)
                {
                    currentDirection = MovingDirection.Down;
                    resetted = false;
                    //Debug.Print("Down");
                    controller.LstickDown();
                }
            }
            else if (A)
            {
                if (currentDirection != MovingDirection.Left)
                {
                    currentDirection = MovingDirection.Left;
                    resetted = false;
                    //Debug.Print("Left");
                    controller.LstickLeft();
                }
            }
            else if (D)
            {
                if (currentDirection != MovingDirection.Right)
                {
                    currentDirection = MovingDirection.Right;
                    resetted = false;
                    //Debug.Print("Right");
                    controller.LstickRight();
                }
            }

            if (I)
            {
                if (!holdingI)
                {
                    holdingI = true;
                    //Debug.Print("Press X");
                    controller.pressX();
                }
            }
            else if (!I)
            {
                if (holdingI)
                {
                    holdingI = false;
                    //Debug.Print("Release X");
                    controller.releaseX();
                }
            }

            if (J)
            {
                if (!holdingJ)
                {
                    holdingJ = true;
                    //Debug.Print("Press Y");
                    controller.pressY();
                }
            }
            else if (!J)
            {
                if (holdingJ)
                {
                    holdingJ = false;
                    //Debug.Print("Release Y");
                    controller.releaseY();
                }
            }

            if (K)
            {
                if (!holdingK)
                {
                    holdingK = true;
                    //Debug.Print("Press B");
                    controller.pressB();
                }
            }
            else if (!K)
            {
                if (holdingK)
                {
                    holdingK = false;
                    //Debug.Print("Release B");
                    controller.releaseB();
                }
            }

            if (L)
            {
                if (!holdingL)
                {
                    holdingL = true;
                    //Debug.Print("Press A");
                    controller.pressA();
                }
            }
            else if (!L)
            {
                if (holdingL)
                {
                    holdingL = false;
                    //Debug.Print("Release A");
                    controller.releaseA();
                }
            }
        }
        #endregion

        private void clearInvBtn_Click(object sender, EventArgs e)
        {
            Utilities.DeleteSlot(s, null, 0);
            WriteLog("First inventory slot cleared!", true);
            WriteLog("Please remember to reset your cursor to the first inventory slot for the drop bot to function properly!", true);
        }

        private void sessionBox_CheckedChanged(object sender, EventArgs e)
        {
            resetSession = sessionBox.Checked;
        }
    }
}
