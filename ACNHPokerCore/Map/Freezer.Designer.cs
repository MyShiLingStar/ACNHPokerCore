
namespace ACNHPokerCore
{
    partial class Freezer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Freezer));
            saveMapBtn = new System.Windows.Forms.Button();
            formToolTip = new System.Windows.Forms.ToolTip(components);
            changeRateBtn = new System.Windows.Forms.Button();
            startBtn = new System.Windows.Forms.Button();
            UnFreezeAllBtn = new System.Windows.Forms.Button();
            FreezeCountLabel = new System.Windows.Forms.Label();
            SlotLabel = new System.Windows.Forms.Label();
            RateBar = new System.Windows.Forms.TrackBar();
            RateValue = new System.Windows.Forms.Label();
            EnableTextBtn = new System.Windows.Forms.Button();
            DisableTextBtn = new System.Windows.Forms.Button();
            UnFreezeInvBtn = new System.Windows.Forms.Button();
            FreezeInvBtn = new System.Windows.Forms.Button();
            FreezeMapBtn = new System.Windows.Forms.Button();
            UnFreezeMapBtn = new System.Windows.Forms.Button();
            PleaseWaitPanel = new System.Windows.Forms.Panel();
            WaitMessagebox = new System.Windows.Forms.Label();
            MapProgressBar = new System.Windows.Forms.ProgressBar();
            NowLoading = new System.Windows.Forms.PictureBox();
            ProgressTimer = new System.Windows.Forms.Timer(components);
            mainPanel = new System.Windows.Forms.Panel();
            unfreezeAllVillagerBtn = new System.Windows.Forms.Button();
            freezeAllVillagerBtn = new System.Windows.Forms.Button();
            FinMsg = new System.Windows.Forms.RichTextBox();
            mapPanel = new System.Windows.Forms.Panel();
            FreezeMap2Btn = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            yCoordinate = new System.Windows.Forms.RichTextBox();
            miniMapBox = new System.Windows.Forms.PictureBox();
            xCoordinate = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)RateBar).BeginInit();
            PleaseWaitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NowLoading).BeginInit();
            mainPanel.SuspendLayout();
            mapPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)miniMapBox).BeginInit();
            SuspendLayout();
            // 
            // saveMapBtn
            // 
            saveMapBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            saveMapBtn.FlatAppearance.BorderSize = 0;
            saveMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            saveMapBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            saveMapBtn.ForeColor = System.Drawing.Color.White;
            saveMapBtn.Location = new System.Drawing.Point(13, 177);
            saveMapBtn.Margin = new System.Windows.Forms.Padding(4);
            saveMapBtn.Name = "saveMapBtn";
            saveMapBtn.Size = new System.Drawing.Size(208, 25);
            saveMapBtn.TabIndex = 218;
            saveMapBtn.Text = "Create Map Template";
            formToolTip.SetToolTip(saveMapBtn, "Create a Map template and save it to a .nhf3 file. (Layer 1 & 2)\r\n");
            saveMapBtn.UseVisualStyleBackColor = false;
            saveMapBtn.Click += SaveMapBtn_Click;
            // 
            // formToolTip
            // 
            formToolTip.AutomaticDelay = 100;
            formToolTip.AutoPopDelay = 10000;
            formToolTip.InitialDelay = 100;
            formToolTip.IsBalloon = true;
            formToolTip.ReshowDelay = 20;
            formToolTip.ShowAlways = true;
            formToolTip.UseAnimation = false;
            formToolTip.UseFading = false;
            // 
            // changeRateBtn
            // 
            changeRateBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            changeRateBtn.FlatAppearance.BorderSize = 0;
            changeRateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            changeRateBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            changeRateBtn.ForeColor = System.Drawing.Color.White;
            changeRateBtn.Location = new System.Drawing.Point(13, 61);
            changeRateBtn.Margin = new System.Windows.Forms.Padding(4);
            changeRateBtn.Name = "changeRateBtn";
            changeRateBtn.Size = new System.Drawing.Size(208, 25);
            changeRateBtn.TabIndex = 229;
            changeRateBtn.Text = "Change Refresh Delay";
            formToolTip.SetToolTip(changeRateBtn, "Change the refresh rate. \r\nLower number means it refresh more frequently.\r\nYou should keep this as high as possible to reduce the load.");
            changeRateBtn.UseVisualStyleBackColor = false;
            changeRateBtn.Click += ChangeRateBtn_Click;
            // 
            // startBtn
            // 
            startBtn.BackColor = System.Drawing.Color.FromArgb(255, 128, 128);
            startBtn.FlatAppearance.BorderSize = 0;
            startBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            startBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            startBtn.ForeColor = System.Drawing.Color.White;
            startBtn.Location = new System.Drawing.Point(143, 4);
            startBtn.Margin = new System.Windows.Forms.Padding(4);
            startBtn.Name = "startBtn";
            startBtn.Size = new System.Drawing.Size(86, 25);
            startBtn.TabIndex = 236;
            startBtn.Tag = "Start";
            startBtn.Text = "Start";
            formToolTip.SetToolTip(startBtn, "Start the regen with only the area selected being ignored.\r\n[WARNING] Item dropped/placed on the empty space outside the area will be deleted.");
            startBtn.UseVisualStyleBackColor = false;
            startBtn.Click += StartBtn_Click;
            // 
            // UnFreezeAllBtn
            // 
            UnFreezeAllBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            UnFreezeAllBtn.FlatAppearance.BorderSize = 0;
            UnFreezeAllBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            UnFreezeAllBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            UnFreezeAllBtn.ForeColor = System.Drawing.Color.White;
            UnFreezeAllBtn.Location = new System.Drawing.Point(13, 326);
            UnFreezeAllBtn.Margin = new System.Windows.Forms.Padding(4);
            UnFreezeAllBtn.Name = "UnFreezeAllBtn";
            UnFreezeAllBtn.Size = new System.Drawing.Size(208, 25);
            UnFreezeAllBtn.TabIndex = 237;
            UnFreezeAllBtn.Text = "UnFreeze Everything";
            UnFreezeAllBtn.UseVisualStyleBackColor = false;
            UnFreezeAllBtn.Click += UnFreezeAllBtn_Click;
            // 
            // FreezeCountLabel
            // 
            FreezeCountLabel.AutoSize = true;
            FreezeCountLabel.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            FreezeCountLabel.ForeColor = System.Drawing.Color.White;
            FreezeCountLabel.Location = new System.Drawing.Point(54, 0);
            FreezeCountLabel.Name = "FreezeCountLabel";
            FreezeCountLabel.Size = new System.Drawing.Size(86, 29);
            FreezeCountLabel.TabIndex = 226;
            FreezeCountLabel.Text = "0 / 255";
            FreezeCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SlotLabel
            // 
            SlotLabel.AutoSize = true;
            SlotLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            SlotLabel.ForeColor = System.Drawing.Color.White;
            SlotLabel.Location = new System.Drawing.Point(12, 7);
            SlotLabel.Name = "SlotLabel";
            SlotLabel.Size = new System.Drawing.Size(42, 16);
            SlotLabel.TabIndex = 227;
            SlotLabel.Text = "Slot :";
            SlotLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RateBar
            // 
            RateBar.LargeChange = 1000;
            RateBar.Location = new System.Drawing.Point(5, 23);
            RateBar.Maximum = 10000;
            RateBar.Minimum = 100;
            RateBar.Name = "RateBar";
            RateBar.Size = new System.Drawing.Size(165, 45);
            RateBar.SmallChange = 100;
            RateBar.TabIndex = 228;
            RateBar.TickFrequency = 1000;
            RateBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            RateBar.Value = 100;
            RateBar.ValueChanged += RateBar_ValueChanged;
            // 
            // RateValue
            // 
            RateValue.AutoSize = true;
            RateValue.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            RateValue.ForeColor = System.Drawing.Color.White;
            RateValue.Location = new System.Drawing.Point(166, 36);
            RateValue.Name = "RateValue";
            RateValue.Size = new System.Drawing.Size(55, 16);
            RateValue.TabIndex = 230;
            RateValue.Text = "100 ms";
            RateValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EnableTextBtn
            // 
            EnableTextBtn.BackColor = System.Drawing.Color.DarkGreen;
            EnableTextBtn.FlatAppearance.BorderSize = 0;
            EnableTextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            EnableTextBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            EnableTextBtn.ForeColor = System.Drawing.Color.White;
            EnableTextBtn.Location = new System.Drawing.Point(13, 91);
            EnableTextBtn.Margin = new System.Windows.Forms.Padding(4);
            EnableTextBtn.Name = "EnableTextBtn";
            EnableTextBtn.Size = new System.Drawing.Size(100, 38);
            EnableTextBtn.TabIndex = 231;
            EnableTextBtn.Text = "Enable Instant Text";
            EnableTextBtn.UseVisualStyleBackColor = false;
            EnableTextBtn.Click += EnableTextBtn_Click;
            // 
            // DisableTextBtn
            // 
            DisableTextBtn.BackColor = System.Drawing.Color.DarkRed;
            DisableTextBtn.FlatAppearance.BorderSize = 0;
            DisableTextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            DisableTextBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            DisableTextBtn.ForeColor = System.Drawing.Color.White;
            DisableTextBtn.Location = new System.Drawing.Point(121, 91);
            DisableTextBtn.Margin = new System.Windows.Forms.Padding(4);
            DisableTextBtn.Name = "DisableTextBtn";
            DisableTextBtn.Size = new System.Drawing.Size(100, 38);
            DisableTextBtn.TabIndex = 232;
            DisableTextBtn.Text = "Disable Instant Text";
            DisableTextBtn.UseVisualStyleBackColor = false;
            DisableTextBtn.Click += DisableTextBtn_Click;
            // 
            // UnFreezeInvBtn
            // 
            UnFreezeInvBtn.BackColor = System.Drawing.Color.DarkRed;
            UnFreezeInvBtn.FlatAppearance.BorderSize = 0;
            UnFreezeInvBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            UnFreezeInvBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            UnFreezeInvBtn.ForeColor = System.Drawing.Color.White;
            UnFreezeInvBtn.Location = new System.Drawing.Point(121, 134);
            UnFreezeInvBtn.Margin = new System.Windows.Forms.Padding(4);
            UnFreezeInvBtn.Name = "UnFreezeInvBtn";
            UnFreezeInvBtn.Size = new System.Drawing.Size(100, 38);
            UnFreezeInvBtn.TabIndex = 234;
            UnFreezeInvBtn.Text = "UnFreeze Inventory";
            UnFreezeInvBtn.UseVisualStyleBackColor = false;
            UnFreezeInvBtn.Click += UnFreezeInvBtn_Click;
            // 
            // FreezeInvBtn
            // 
            FreezeInvBtn.BackColor = System.Drawing.Color.DarkGreen;
            FreezeInvBtn.FlatAppearance.BorderSize = 0;
            FreezeInvBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            FreezeInvBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            FreezeInvBtn.ForeColor = System.Drawing.Color.White;
            FreezeInvBtn.Location = new System.Drawing.Point(13, 134);
            FreezeInvBtn.Margin = new System.Windows.Forms.Padding(4);
            FreezeInvBtn.Name = "FreezeInvBtn";
            FreezeInvBtn.Size = new System.Drawing.Size(100, 38);
            FreezeInvBtn.TabIndex = 233;
            FreezeInvBtn.Text = "Freeze Inventory";
            FreezeInvBtn.UseVisualStyleBackColor = false;
            FreezeInvBtn.Click += FreezeInvBtn_Click;
            // 
            // FreezeMapBtn
            // 
            FreezeMapBtn.BackColor = System.Drawing.Color.DarkGreen;
            FreezeMapBtn.FlatAppearance.BorderSize = 0;
            FreezeMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            FreezeMapBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            FreezeMapBtn.ForeColor = System.Drawing.Color.White;
            FreezeMapBtn.Location = new System.Drawing.Point(13, 207);
            FreezeMapBtn.Margin = new System.Windows.Forms.Padding(4);
            FreezeMapBtn.Name = "FreezeMapBtn";
            FreezeMapBtn.Size = new System.Drawing.Size(100, 38);
            FreezeMapBtn.TabIndex = 235;
            FreezeMapBtn.Text = "Freeze Map";
            FreezeMapBtn.UseVisualStyleBackColor = false;
            FreezeMapBtn.Click += FreezeMapBtn_Click;
            // 
            // UnFreezeMapBtn
            // 
            UnFreezeMapBtn.BackColor = System.Drawing.Color.DarkRed;
            UnFreezeMapBtn.FlatAppearance.BorderSize = 0;
            UnFreezeMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            UnFreezeMapBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            UnFreezeMapBtn.ForeColor = System.Drawing.Color.White;
            UnFreezeMapBtn.Location = new System.Drawing.Point(121, 207);
            UnFreezeMapBtn.Margin = new System.Windows.Forms.Padding(4);
            UnFreezeMapBtn.Name = "UnFreezeMapBtn";
            UnFreezeMapBtn.Size = new System.Drawing.Size(100, 38);
            UnFreezeMapBtn.TabIndex = 236;
            UnFreezeMapBtn.Text = "UnFreeze Map";
            UnFreezeMapBtn.UseVisualStyleBackColor = false;
            UnFreezeMapBtn.Click += UnFreezeMapBtn_Click;
            // 
            // PleaseWaitPanel
            // 
            PleaseWaitPanel.Controls.Add(WaitMessagebox);
            PleaseWaitPanel.Controls.Add(MapProgressBar);
            PleaseWaitPanel.Controls.Add(NowLoading);
            PleaseWaitPanel.Location = new System.Drawing.Point(2, 293);
            PleaseWaitPanel.Name = "PleaseWaitPanel";
            PleaseWaitPanel.Size = new System.Drawing.Size(230, 35);
            PleaseWaitPanel.TabIndex = 238;
            PleaseWaitPanel.Visible = false;
            // 
            // WaitMessagebox
            // 
            WaitMessagebox.AutoSize = true;
            WaitMessagebox.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            WaitMessagebox.ForeColor = System.Drawing.Color.White;
            WaitMessagebox.Location = new System.Drawing.Point(85, 6);
            WaitMessagebox.Name = "WaitMessagebox";
            WaitMessagebox.Size = new System.Drawing.Size(56, 16);
            WaitMessagebox.TabIndex = 240;
            WaitMessagebox.Text = "testing";
            WaitMessagebox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MapProgressBar
            // 
            MapProgressBar.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            MapProgressBar.ForeColor = System.Drawing.Color.LawnGreen;
            MapProgressBar.Location = new System.Drawing.Point(4, 28);
            MapProgressBar.Maximum = 260;
            MapProgressBar.Name = "MapProgressBar";
            MapProgressBar.Size = new System.Drawing.Size(220, 3);
            MapProgressBar.TabIndex = 215;
            // 
            // NowLoading
            // 
            NowLoading.Image = Properties.Resources.loading;
            NowLoading.Location = new System.Drawing.Point(61, 2);
            NowLoading.Name = "NowLoading";
            NowLoading.Size = new System.Drawing.Size(24, 24);
            NowLoading.TabIndex = 216;
            NowLoading.TabStop = false;
            // 
            // ProgressTimer
            // 
            ProgressTimer.Interval = 500;
            ProgressTimer.Tick += ProgressTimer_Tick;
            // 
            // mainPanel
            // 
            mainPanel.Controls.Add(unfreezeAllVillagerBtn);
            mainPanel.Controls.Add(freezeAllVillagerBtn);
            mainPanel.Controls.Add(changeRateBtn);
            mainPanel.Controls.Add(RateBar);
            mainPanel.Controls.Add(SlotLabel);
            mainPanel.Controls.Add(RateValue);
            mainPanel.Controls.Add(saveMapBtn);
            mainPanel.Controls.Add(FreezeCountLabel);
            mainPanel.Controls.Add(EnableTextBtn);
            mainPanel.Controls.Add(FinMsg);
            mainPanel.Controls.Add(DisableTextBtn);
            mainPanel.Controls.Add(UnFreezeAllBtn);
            mainPanel.Controls.Add(FreezeInvBtn);
            mainPanel.Controls.Add(UnFreezeMapBtn);
            mainPanel.Controls.Add(UnFreezeInvBtn);
            mainPanel.Controls.Add(FreezeMapBtn);
            mainPanel.Location = new System.Drawing.Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new System.Drawing.Size(236, 364);
            mainPanel.TabIndex = 240;
            // 
            // unfreezeAllVillagerBtn
            // 
            unfreezeAllVillagerBtn.BackColor = System.Drawing.Color.DarkRed;
            unfreezeAllVillagerBtn.FlatAppearance.BorderSize = 0;
            unfreezeAllVillagerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            unfreezeAllVillagerBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            unfreezeAllVillagerBtn.ForeColor = System.Drawing.Color.White;
            unfreezeAllVillagerBtn.Location = new System.Drawing.Point(121, 250);
            unfreezeAllVillagerBtn.Margin = new System.Windows.Forms.Padding(4);
            unfreezeAllVillagerBtn.Name = "unfreezeAllVillagerBtn";
            unfreezeAllVillagerBtn.Size = new System.Drawing.Size(100, 38);
            unfreezeAllVillagerBtn.TabIndex = 246;
            unfreezeAllVillagerBtn.Text = "UnFreeze All Villager";
            unfreezeAllVillagerBtn.UseVisualStyleBackColor = false;
            unfreezeAllVillagerBtn.Click += UnfreezeAllVillagerBtn_Click;
            // 
            // freezeAllVillagerBtn
            // 
            freezeAllVillagerBtn.BackColor = System.Drawing.Color.DarkBlue;
            freezeAllVillagerBtn.FlatAppearance.BorderSize = 0;
            freezeAllVillagerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            freezeAllVillagerBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            freezeAllVillagerBtn.ForeColor = System.Drawing.Color.White;
            freezeAllVillagerBtn.Location = new System.Drawing.Point(13, 250);
            freezeAllVillagerBtn.Margin = new System.Windows.Forms.Padding(4);
            freezeAllVillagerBtn.Name = "freezeAllVillagerBtn";
            freezeAllVillagerBtn.Size = new System.Drawing.Size(100, 38);
            freezeAllVillagerBtn.TabIndex = 245;
            freezeAllVillagerBtn.Text = "Freeze All Villager";
            freezeAllVillagerBtn.UseVisualStyleBackColor = false;
            freezeAllVillagerBtn.Click += FreezeAllVillagerBtn_Click;
            // 
            // FinMsg
            // 
            FinMsg.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            FinMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            FinMsg.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            FinMsg.ForeColor = System.Drawing.Color.White;
            FinMsg.Location = new System.Drawing.Point(14, 298);
            FinMsg.Multiline = false;
            FinMsg.Name = "FinMsg";
            FinMsg.ReadOnly = true;
            FinMsg.Size = new System.Drawing.Size(207, 21);
            FinMsg.TabIndex = 239;
            FinMsg.Text = "";
            FinMsg.Visible = false;
            // 
            // mapPanel
            // 
            mapPanel.Controls.Add(FreezeMap2Btn);
            mapPanel.Controls.Add(startBtn);
            mapPanel.Controls.Add(label4);
            mapPanel.Controls.Add(label3);
            mapPanel.Controls.Add(yCoordinate);
            mapPanel.Controls.Add(miniMapBox);
            mapPanel.Controls.Add(xCoordinate);
            mapPanel.Location = new System.Drawing.Point(239, 2);
            mapPanel.Name = "mapPanel";
            mapPanel.Size = new System.Drawing.Size(235, 364);
            mapPanel.TabIndex = 241;
            // 
            // FreezeMap2Btn
            // 
            FreezeMap2Btn.BackColor = System.Drawing.Color.DarkGreen;
            FreezeMap2Btn.FlatAppearance.BorderSize = 0;
            FreezeMap2Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            FreezeMap2Btn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            FreezeMap2Btn.ForeColor = System.Drawing.Color.White;
            FreezeMap2Btn.Location = new System.Drawing.Point(5, 4);
            FreezeMap2Btn.Margin = new System.Windows.Forms.Padding(4);
            FreezeMap2Btn.Name = "FreezeMap2Btn";
            FreezeMap2Btn.Size = new System.Drawing.Size(132, 25);
            FreezeMap2Btn.TabIndex = 242;
            FreezeMap2Btn.Text = "Freeze Map";
            FreezeMap2Btn.UseVisualStyleBackColor = false;
            FreezeMap2Btn.Click += FreezeMap2Btn_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            label4.ForeColor = System.Drawing.Color.White;
            label4.Location = new System.Drawing.Point(140, 48);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(25, 16);
            label4.TabIndex = 238;
            label4.Text = "Y :";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            label3.ForeColor = System.Drawing.Color.White;
            label3.Location = new System.Drawing.Point(49, 48);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(24, 16);
            label3.TabIndex = 236;
            label3.Text = "X :";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // yCoordinate
            // 
            yCoordinate.BackColor = System.Drawing.Color.FromArgb(64, 68, 75);
            yCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            yCoordinate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            yCoordinate.ForeColor = System.Drawing.Color.White;
            yCoordinate.Location = new System.Drawing.Point(166, 46);
            yCoordinate.MaxLength = 3;
            yCoordinate.Multiline = false;
            yCoordinate.Name = "yCoordinate";
            yCoordinate.Size = new System.Drawing.Size(63, 20);
            yCoordinate.TabIndex = 237;
            yCoordinate.Text = "";
            // 
            // miniMapBox
            // 
            miniMapBox.BackColor = System.Drawing.Color.Transparent;
            miniMapBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            miniMapBox.ErrorImage = null;
            miniMapBox.InitialImage = null;
            miniMapBox.Location = new System.Drawing.Point(5, 72);
            miniMapBox.Name = "miniMapBox";
            miniMapBox.Size = new System.Drawing.Size(224, 192);
            miniMapBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            miniMapBox.TabIndex = 190;
            miniMapBox.TabStop = false;
            miniMapBox.MouseDown += MiniMapBox_MouseDown;
            miniMapBox.MouseMove += MiniMapBox_MouseMove;
            // 
            // xCoordinate
            // 
            xCoordinate.BackColor = System.Drawing.Color.FromArgb(64, 68, 75);
            xCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            xCoordinate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            xCoordinate.ForeColor = System.Drawing.Color.White;
            xCoordinate.Location = new System.Drawing.Point(74, 46);
            xCoordinate.MaxLength = 3;
            xCoordinate.Multiline = false;
            xCoordinate.Name = "xCoordinate";
            xCoordinate.Size = new System.Drawing.Size(63, 20);
            xCoordinate.TabIndex = 236;
            xCoordinate.Text = "";
            // 
            // Freezer
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            ClientSize = new System.Drawing.Size(234, 356);
            Controls.Add(mapPanel);
            Controls.Add(PleaseWaitPanel);
            Controls.Add(mainPanel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(250, 395);
            Name = "Freezer";
            Opacity = 0.95D;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Freezer";
            FormClosed += Freezer_FormClosed;
            ((System.ComponentModel.ISupportInitialize)RateBar).EndInit();
            PleaseWaitPanel.ResumeLayout(false);
            PleaseWaitPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NowLoading).EndInit();
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            mapPanel.ResumeLayout(false);
            mapPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)miniMapBox).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button saveMapBtn;
        private System.Windows.Forms.ToolTip formToolTip;
        private System.Windows.Forms.Label FreezeCountLabel;
        private System.Windows.Forms.Label SlotLabel;
        private System.Windows.Forms.TrackBar RateBar;
        private System.Windows.Forms.Button changeRateBtn;
        private System.Windows.Forms.Label RateValue;
        private System.Windows.Forms.Button EnableTextBtn;
        private System.Windows.Forms.Button DisableTextBtn;
        private System.Windows.Forms.Button UnFreezeInvBtn;
        private System.Windows.Forms.Button FreezeInvBtn;
        private System.Windows.Forms.Button FreezeMapBtn;
        private System.Windows.Forms.Button UnFreezeMapBtn;
        private System.Windows.Forms.Button UnFreezeAllBtn;
        private System.Windows.Forms.Panel PleaseWaitPanel;
        private System.Windows.Forms.ProgressBar MapProgressBar;
        private System.Windows.Forms.PictureBox NowLoading;
        private System.Windows.Forms.Timer ProgressTimer;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Label WaitMessagebox;
        private System.Windows.Forms.RichTextBox FinMsg;
        private System.Windows.Forms.Panel mapPanel;
        private System.Windows.Forms.Button FreezeMap2Btn;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox yCoordinate;
        private System.Windows.Forms.PictureBox miniMapBox;
        private System.Windows.Forms.RichTextBox xCoordinate;
        private System.Windows.Forms.Button freezeAllVillagerBtn;
        private System.Windows.Forms.Button unfreezeAllVillagerBtn;
    }
}