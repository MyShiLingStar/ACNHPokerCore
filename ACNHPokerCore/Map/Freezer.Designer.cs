
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Freezer));
            this.saveMapBtn = new System.Windows.Forms.Button();
            this.formToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.changeRateBtn = new System.Windows.Forms.Button();
            this.startBtn = new System.Windows.Forms.Button();
            this.UnFreezeAllBtn = new System.Windows.Forms.Button();
            this.FreezeCountLabel = new System.Windows.Forms.Label();
            this.SlotLabel = new System.Windows.Forms.Label();
            this.RateBar = new System.Windows.Forms.TrackBar();
            this.RateValue = new System.Windows.Forms.Label();
            this.EnableTextBtn = new System.Windows.Forms.Button();
            this.DisableTextBtn = new System.Windows.Forms.Button();
            this.UnFreezeInvBtn = new System.Windows.Forms.Button();
            this.FreezeInvBtn = new System.Windows.Forms.Button();
            this.FreezeMapBtn = new System.Windows.Forms.Button();
            this.UnFreezeMapBtn = new System.Windows.Forms.Button();
            this.PleaseWaitPanel = new System.Windows.Forms.Panel();
            this.WaitMessagebox = new System.Windows.Forms.Label();
            this.MapProgressBar = new System.Windows.Forms.ProgressBar();
            this.NowLoading = new System.Windows.Forms.PictureBox();
            this.ProgressTimer = new System.Windows.Forms.Timer(this.components);
            this.mainPanel = new System.Windows.Forms.Panel();
            this.unfreezeAllVillagerBtn = new System.Windows.Forms.Button();
            this.freezeAllVillagerBtn = new System.Windows.Forms.Button();
            this.FinMsg = new System.Windows.Forms.RichTextBox();
            this.mapPanel = new System.Windows.Forms.Panel();
            this.FreezeMap2Btn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.yCoordinate = new System.Windows.Forms.RichTextBox();
            this.miniMapBox = new System.Windows.Forms.PictureBox();
            this.xCoordinate = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.RateBar)).BeginInit();
            this.PleaseWaitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).BeginInit();
            this.mainPanel.SuspendLayout();
            this.mapPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).BeginInit();
            this.SuspendLayout();
            // 
            // saveMapBtn
            // 
            this.saveMapBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.saveMapBtn.FlatAppearance.BorderSize = 0;
            this.saveMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveMapBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.saveMapBtn.ForeColor = System.Drawing.Color.White;
            this.saveMapBtn.Location = new System.Drawing.Point(13, 177);
            this.saveMapBtn.Margin = new System.Windows.Forms.Padding(4);
            this.saveMapBtn.Name = "saveMapBtn";
            this.saveMapBtn.Size = new System.Drawing.Size(208, 25);
            this.saveMapBtn.TabIndex = 218;
            this.saveMapBtn.Text = "Create Map Template";
            this.formToolTip.SetToolTip(this.saveMapBtn, "Create a Map template and save it to a .nhf2 file. (Layer 1 & 2)\r\n");
            this.saveMapBtn.UseVisualStyleBackColor = false;
            this.saveMapBtn.Click += new System.EventHandler(this.SaveMapBtn_Click);
            // 
            // formToolTip
            // 
            this.formToolTip.AutomaticDelay = 100;
            this.formToolTip.AutoPopDelay = 10000;
            this.formToolTip.InitialDelay = 100;
            this.formToolTip.IsBalloon = true;
            this.formToolTip.ReshowDelay = 20;
            this.formToolTip.ShowAlways = true;
            this.formToolTip.UseAnimation = false;
            this.formToolTip.UseFading = false;
            // 
            // changeRateBtn
            // 
            this.changeRateBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.changeRateBtn.FlatAppearance.BorderSize = 0;
            this.changeRateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.changeRateBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.changeRateBtn.ForeColor = System.Drawing.Color.White;
            this.changeRateBtn.Location = new System.Drawing.Point(13, 61);
            this.changeRateBtn.Margin = new System.Windows.Forms.Padding(4);
            this.changeRateBtn.Name = "changeRateBtn";
            this.changeRateBtn.Size = new System.Drawing.Size(208, 25);
            this.changeRateBtn.TabIndex = 229;
            this.changeRateBtn.Text = "Change Refresh Delay";
            this.formToolTip.SetToolTip(this.changeRateBtn, "Change the refresh rate. \r\nLower number means it refresh more frequently.\r\nYou sh" +
        "ould keep this as high as possible to reduce the load.");
            this.changeRateBtn.UseVisualStyleBackColor = false;
            this.changeRateBtn.Click += new System.EventHandler(this.ChangeRateBtn_Click);
            // 
            // startBtn
            // 
            this.startBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.startBtn.FlatAppearance.BorderSize = 0;
            this.startBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.startBtn.ForeColor = System.Drawing.Color.White;
            this.startBtn.Location = new System.Drawing.Point(143, 4);
            this.startBtn.Margin = new System.Windows.Forms.Padding(4);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(86, 25);
            this.startBtn.TabIndex = 236;
            this.startBtn.Tag = "Start";
            this.startBtn.Text = "Start";
            this.formToolTip.SetToolTip(this.startBtn, "Start the regen with only the area selected being ignored.\r\n[WARNING] Item droppe" +
        "d/placed on the empty space outside the area will be deleted.");
            this.startBtn.UseVisualStyleBackColor = false;
            this.startBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // UnFreezeAllBtn
            // 
            this.UnFreezeAllBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.UnFreezeAllBtn.FlatAppearance.BorderSize = 0;
            this.UnFreezeAllBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UnFreezeAllBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.UnFreezeAllBtn.ForeColor = System.Drawing.Color.White;
            this.UnFreezeAllBtn.Location = new System.Drawing.Point(13, 326);
            this.UnFreezeAllBtn.Margin = new System.Windows.Forms.Padding(4);
            this.UnFreezeAllBtn.Name = "UnFreezeAllBtn";
            this.UnFreezeAllBtn.Size = new System.Drawing.Size(208, 25);
            this.UnFreezeAllBtn.TabIndex = 237;
            this.UnFreezeAllBtn.Text = "UnFreeze Everything";
            this.UnFreezeAllBtn.UseVisualStyleBackColor = false;
            this.UnFreezeAllBtn.Click += new System.EventHandler(this.UnFreezeAllBtn_Click);
            // 
            // FreezeCountLabel
            // 
            this.FreezeCountLabel.AutoSize = true;
            this.FreezeCountLabel.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.FreezeCountLabel.ForeColor = System.Drawing.Color.White;
            this.FreezeCountLabel.Location = new System.Drawing.Point(54, 0);
            this.FreezeCountLabel.Name = "FreezeCountLabel";
            this.FreezeCountLabel.Size = new System.Drawing.Size(86, 29);
            this.FreezeCountLabel.TabIndex = 226;
            this.FreezeCountLabel.Text = "0 / 255";
            this.FreezeCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SlotLabel
            // 
            this.SlotLabel.AutoSize = true;
            this.SlotLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SlotLabel.ForeColor = System.Drawing.Color.White;
            this.SlotLabel.Location = new System.Drawing.Point(12, 7);
            this.SlotLabel.Name = "SlotLabel";
            this.SlotLabel.Size = new System.Drawing.Size(42, 16);
            this.SlotLabel.TabIndex = 227;
            this.SlotLabel.Text = "Slot :";
            this.SlotLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RateBar
            // 
            this.RateBar.LargeChange = 1000;
            this.RateBar.Location = new System.Drawing.Point(5, 23);
            this.RateBar.Maximum = 10000;
            this.RateBar.Minimum = 100;
            this.RateBar.Name = "RateBar";
            this.RateBar.Size = new System.Drawing.Size(165, 45);
            this.RateBar.SmallChange = 100;
            this.RateBar.TabIndex = 228;
            this.RateBar.TickFrequency = 1000;
            this.RateBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.RateBar.Value = 100;
            this.RateBar.ValueChanged += new System.EventHandler(this.RateBar_ValueChanged);
            // 
            // RateValue
            // 
            this.RateValue.AutoSize = true;
            this.RateValue.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RateValue.ForeColor = System.Drawing.Color.White;
            this.RateValue.Location = new System.Drawing.Point(166, 36);
            this.RateValue.Name = "RateValue";
            this.RateValue.Size = new System.Drawing.Size(55, 16);
            this.RateValue.TabIndex = 230;
            this.RateValue.Text = "100 ms";
            this.RateValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EnableTextBtn
            // 
            this.EnableTextBtn.BackColor = System.Drawing.Color.DarkGreen;
            this.EnableTextBtn.FlatAppearance.BorderSize = 0;
            this.EnableTextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EnableTextBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.EnableTextBtn.ForeColor = System.Drawing.Color.White;
            this.EnableTextBtn.Location = new System.Drawing.Point(13, 91);
            this.EnableTextBtn.Margin = new System.Windows.Forms.Padding(4);
            this.EnableTextBtn.Name = "EnableTextBtn";
            this.EnableTextBtn.Size = new System.Drawing.Size(100, 38);
            this.EnableTextBtn.TabIndex = 231;
            this.EnableTextBtn.Text = "Enable Instant Text";
            this.EnableTextBtn.UseVisualStyleBackColor = false;
            this.EnableTextBtn.Click += new System.EventHandler(this.EnableTextBtn_Click);
            // 
            // DisableTextBtn
            // 
            this.DisableTextBtn.BackColor = System.Drawing.Color.DarkRed;
            this.DisableTextBtn.FlatAppearance.BorderSize = 0;
            this.DisableTextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DisableTextBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DisableTextBtn.ForeColor = System.Drawing.Color.White;
            this.DisableTextBtn.Location = new System.Drawing.Point(121, 91);
            this.DisableTextBtn.Margin = new System.Windows.Forms.Padding(4);
            this.DisableTextBtn.Name = "DisableTextBtn";
            this.DisableTextBtn.Size = new System.Drawing.Size(100, 38);
            this.DisableTextBtn.TabIndex = 232;
            this.DisableTextBtn.Text = "Disable Instant Text";
            this.DisableTextBtn.UseVisualStyleBackColor = false;
            this.DisableTextBtn.Click += new System.EventHandler(this.DisableTextBtn_Click);
            // 
            // UnFreezeInvBtn
            // 
            this.UnFreezeInvBtn.BackColor = System.Drawing.Color.DarkRed;
            this.UnFreezeInvBtn.FlatAppearance.BorderSize = 0;
            this.UnFreezeInvBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UnFreezeInvBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.UnFreezeInvBtn.ForeColor = System.Drawing.Color.White;
            this.UnFreezeInvBtn.Location = new System.Drawing.Point(121, 134);
            this.UnFreezeInvBtn.Margin = new System.Windows.Forms.Padding(4);
            this.UnFreezeInvBtn.Name = "UnFreezeInvBtn";
            this.UnFreezeInvBtn.Size = new System.Drawing.Size(100, 38);
            this.UnFreezeInvBtn.TabIndex = 234;
            this.UnFreezeInvBtn.Text = "UnFreeze Inventory";
            this.UnFreezeInvBtn.UseVisualStyleBackColor = false;
            this.UnFreezeInvBtn.Click += new System.EventHandler(this.UnFreezeInvBtn_Click);
            // 
            // FreezeInvBtn
            // 
            this.FreezeInvBtn.BackColor = System.Drawing.Color.DarkGreen;
            this.FreezeInvBtn.FlatAppearance.BorderSize = 0;
            this.FreezeInvBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FreezeInvBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.FreezeInvBtn.ForeColor = System.Drawing.Color.White;
            this.FreezeInvBtn.Location = new System.Drawing.Point(13, 134);
            this.FreezeInvBtn.Margin = new System.Windows.Forms.Padding(4);
            this.FreezeInvBtn.Name = "FreezeInvBtn";
            this.FreezeInvBtn.Size = new System.Drawing.Size(100, 38);
            this.FreezeInvBtn.TabIndex = 233;
            this.FreezeInvBtn.Text = "Freeze Inventory";
            this.FreezeInvBtn.UseVisualStyleBackColor = false;
            this.FreezeInvBtn.Click += new System.EventHandler(this.FreezeInvBtn_Click);
            // 
            // FreezeMapBtn
            // 
            this.FreezeMapBtn.BackColor = System.Drawing.Color.DarkGreen;
            this.FreezeMapBtn.FlatAppearance.BorderSize = 0;
            this.FreezeMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FreezeMapBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.FreezeMapBtn.ForeColor = System.Drawing.Color.White;
            this.FreezeMapBtn.Location = new System.Drawing.Point(13, 207);
            this.FreezeMapBtn.Margin = new System.Windows.Forms.Padding(4);
            this.FreezeMapBtn.Name = "FreezeMapBtn";
            this.FreezeMapBtn.Size = new System.Drawing.Size(100, 38);
            this.FreezeMapBtn.TabIndex = 235;
            this.FreezeMapBtn.Text = "Freeze Map";
            this.FreezeMapBtn.UseVisualStyleBackColor = false;
            this.FreezeMapBtn.Click += new System.EventHandler(this.FreezeMapBtn_Click);
            // 
            // UnFreezeMapBtn
            // 
            this.UnFreezeMapBtn.BackColor = System.Drawing.Color.DarkRed;
            this.UnFreezeMapBtn.FlatAppearance.BorderSize = 0;
            this.UnFreezeMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UnFreezeMapBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.UnFreezeMapBtn.ForeColor = System.Drawing.Color.White;
            this.UnFreezeMapBtn.Location = new System.Drawing.Point(121, 207);
            this.UnFreezeMapBtn.Margin = new System.Windows.Forms.Padding(4);
            this.UnFreezeMapBtn.Name = "UnFreezeMapBtn";
            this.UnFreezeMapBtn.Size = new System.Drawing.Size(100, 38);
            this.UnFreezeMapBtn.TabIndex = 236;
            this.UnFreezeMapBtn.Text = "UnFreeze Map";
            this.UnFreezeMapBtn.UseVisualStyleBackColor = false;
            this.UnFreezeMapBtn.Click += new System.EventHandler(this.UnFreezeMapBtn_Click);
            // 
            // PleaseWaitPanel
            // 
            this.PleaseWaitPanel.Controls.Add(this.WaitMessagebox);
            this.PleaseWaitPanel.Controls.Add(this.MapProgressBar);
            this.PleaseWaitPanel.Controls.Add(this.NowLoading);
            this.PleaseWaitPanel.Location = new System.Drawing.Point(2, 293);
            this.PleaseWaitPanel.Name = "PleaseWaitPanel";
            this.PleaseWaitPanel.Size = new System.Drawing.Size(230, 35);
            this.PleaseWaitPanel.TabIndex = 238;
            this.PleaseWaitPanel.Visible = false;
            // 
            // WaitMessagebox
            // 
            this.WaitMessagebox.AutoSize = true;
            this.WaitMessagebox.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.WaitMessagebox.ForeColor = System.Drawing.Color.White;
            this.WaitMessagebox.Location = new System.Drawing.Point(85, 6);
            this.WaitMessagebox.Name = "WaitMessagebox";
            this.WaitMessagebox.Size = new System.Drawing.Size(56, 16);
            this.WaitMessagebox.TabIndex = 240;
            this.WaitMessagebox.Text = "testing";
            this.WaitMessagebox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MapProgressBar
            // 
            this.MapProgressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.MapProgressBar.ForeColor = System.Drawing.Color.LawnGreen;
            this.MapProgressBar.Location = new System.Drawing.Point(4, 28);
            this.MapProgressBar.Maximum = 260;
            this.MapProgressBar.Name = "MapProgressBar";
            this.MapProgressBar.Size = new System.Drawing.Size(220, 3);
            this.MapProgressBar.TabIndex = 215;
            // 
            // NowLoading
            // 
            this.NowLoading.Image = global::ACNHPokerCore.Properties.Resources.loading;
            this.NowLoading.Location = new System.Drawing.Point(61, 2);
            this.NowLoading.Name = "NowLoading";
            this.NowLoading.Size = new System.Drawing.Size(24, 24);
            this.NowLoading.TabIndex = 216;
            this.NowLoading.TabStop = false;
            // 
            // ProgressTimer
            // 
            this.ProgressTimer.Interval = 500;
            this.ProgressTimer.Tick += new System.EventHandler(this.ProgressTimer_Tick);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.unfreezeAllVillagerBtn);
            this.mainPanel.Controls.Add(this.freezeAllVillagerBtn);
            this.mainPanel.Controls.Add(this.changeRateBtn);
            this.mainPanel.Controls.Add(this.RateBar);
            this.mainPanel.Controls.Add(this.SlotLabel);
            this.mainPanel.Controls.Add(this.RateValue);
            this.mainPanel.Controls.Add(this.saveMapBtn);
            this.mainPanel.Controls.Add(this.FreezeCountLabel);
            this.mainPanel.Controls.Add(this.EnableTextBtn);
            this.mainPanel.Controls.Add(this.FinMsg);
            this.mainPanel.Controls.Add(this.DisableTextBtn);
            this.mainPanel.Controls.Add(this.UnFreezeAllBtn);
            this.mainPanel.Controls.Add(this.FreezeInvBtn);
            this.mainPanel.Controls.Add(this.UnFreezeMapBtn);
            this.mainPanel.Controls.Add(this.UnFreezeInvBtn);
            this.mainPanel.Controls.Add(this.FreezeMapBtn);
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(236, 364);
            this.mainPanel.TabIndex = 240;
            // 
            // unfreezeAllVillagerBtn
            // 
            this.unfreezeAllVillagerBtn.BackColor = System.Drawing.Color.DarkRed;
            this.unfreezeAllVillagerBtn.FlatAppearance.BorderSize = 0;
            this.unfreezeAllVillagerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.unfreezeAllVillagerBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.unfreezeAllVillagerBtn.ForeColor = System.Drawing.Color.White;
            this.unfreezeAllVillagerBtn.Location = new System.Drawing.Point(121, 250);
            this.unfreezeAllVillagerBtn.Margin = new System.Windows.Forms.Padding(4);
            this.unfreezeAllVillagerBtn.Name = "unfreezeAllVillagerBtn";
            this.unfreezeAllVillagerBtn.Size = new System.Drawing.Size(100, 38);
            this.unfreezeAllVillagerBtn.TabIndex = 246;
            this.unfreezeAllVillagerBtn.Text = "UnFreeze All Villager";
            this.unfreezeAllVillagerBtn.UseVisualStyleBackColor = false;
            this.unfreezeAllVillagerBtn.Click += new System.EventHandler(this.UnfreezeAllVillagerBtn_Click);
            // 
            // freezeAllVillagerBtn
            // 
            this.freezeAllVillagerBtn.BackColor = System.Drawing.Color.DarkBlue;
            this.freezeAllVillagerBtn.FlatAppearance.BorderSize = 0;
            this.freezeAllVillagerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.freezeAllVillagerBtn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.freezeAllVillagerBtn.ForeColor = System.Drawing.Color.White;
            this.freezeAllVillagerBtn.Location = new System.Drawing.Point(13, 250);
            this.freezeAllVillagerBtn.Margin = new System.Windows.Forms.Padding(4);
            this.freezeAllVillagerBtn.Name = "freezeAllVillagerBtn";
            this.freezeAllVillagerBtn.Size = new System.Drawing.Size(100, 38);
            this.freezeAllVillagerBtn.TabIndex = 245;
            this.freezeAllVillagerBtn.Text = "Freeze All Villager";
            this.freezeAllVillagerBtn.UseVisualStyleBackColor = false;
            this.freezeAllVillagerBtn.Click += new System.EventHandler(this.FreezeAllVillagerBtn_Click);
            // 
            // FinMsg
            // 
            this.FinMsg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.FinMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FinMsg.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.FinMsg.ForeColor = System.Drawing.Color.White;
            this.FinMsg.Location = new System.Drawing.Point(14, 298);
            this.FinMsg.Multiline = false;
            this.FinMsg.Name = "FinMsg";
            this.FinMsg.ReadOnly = true;
            this.FinMsg.Size = new System.Drawing.Size(207, 21);
            this.FinMsg.TabIndex = 239;
            this.FinMsg.Text = "";
            this.FinMsg.Visible = false;
            // 
            // mapPanel
            // 
            this.mapPanel.Controls.Add(this.FreezeMap2Btn);
            this.mapPanel.Controls.Add(this.startBtn);
            this.mapPanel.Controls.Add(this.label4);
            this.mapPanel.Controls.Add(this.label3);
            this.mapPanel.Controls.Add(this.yCoordinate);
            this.mapPanel.Controls.Add(this.miniMapBox);
            this.mapPanel.Controls.Add(this.xCoordinate);
            this.mapPanel.Location = new System.Drawing.Point(239, 2);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(235, 364);
            this.mapPanel.TabIndex = 241;
            // 
            // FreezeMap2Btn
            // 
            this.FreezeMap2Btn.BackColor = System.Drawing.Color.DarkGreen;
            this.FreezeMap2Btn.FlatAppearance.BorderSize = 0;
            this.FreezeMap2Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FreezeMap2Btn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.FreezeMap2Btn.ForeColor = System.Drawing.Color.White;
            this.FreezeMap2Btn.Location = new System.Drawing.Point(5, 4);
            this.FreezeMap2Btn.Margin = new System.Windows.Forms.Padding(4);
            this.FreezeMap2Btn.Name = "FreezeMap2Btn";
            this.FreezeMap2Btn.Size = new System.Drawing.Size(132, 25);
            this.FreezeMap2Btn.TabIndex = 242;
            this.FreezeMap2Btn.Text = "Freeze Map";
            this.FreezeMap2Btn.UseVisualStyleBackColor = false;
            this.FreezeMap2Btn.Click += new System.EventHandler(this.FreezeMap2Btn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(140, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 16);
            this.label4.TabIndex = 238;
            this.label4.Text = "Y :";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(49, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 16);
            this.label3.TabIndex = 236;
            this.label3.Text = "X :";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // yCoordinate
            // 
            this.yCoordinate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.yCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.yCoordinate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.yCoordinate.ForeColor = System.Drawing.Color.White;
            this.yCoordinate.Location = new System.Drawing.Point(166, 46);
            this.yCoordinate.MaxLength = 3;
            this.yCoordinate.Multiline = false;
            this.yCoordinate.Name = "yCoordinate";
            this.yCoordinate.Size = new System.Drawing.Size(63, 20);
            this.yCoordinate.TabIndex = 237;
            this.yCoordinate.Text = "";
            // 
            // miniMapBox
            // 
            this.miniMapBox.BackColor = System.Drawing.Color.Transparent;
            this.miniMapBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.miniMapBox.ErrorImage = null;
            this.miniMapBox.InitialImage = null;
            this.miniMapBox.Location = new System.Drawing.Point(5, 72);
            this.miniMapBox.Name = "miniMapBox";
            this.miniMapBox.Size = new System.Drawing.Size(224, 192);
            this.miniMapBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.miniMapBox.TabIndex = 190;
            this.miniMapBox.TabStop = false;
            this.miniMapBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MiniMapBox_MouseDown);
            this.miniMapBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MiniMapBox_MouseMove);
            // 
            // xCoordinate
            // 
            this.xCoordinate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.xCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.xCoordinate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.xCoordinate.ForeColor = System.Drawing.Color.White;
            this.xCoordinate.Location = new System.Drawing.Point(74, 46);
            this.xCoordinate.MaxLength = 3;
            this.xCoordinate.Multiline = false;
            this.xCoordinate.Name = "xCoordinate";
            this.xCoordinate.Size = new System.Drawing.Size(63, 20);
            this.xCoordinate.TabIndex = 236;
            this.xCoordinate.Text = "";
            // 
            // Freezer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(234, 356);
            this.Controls.Add(this.mapPanel);
            this.Controls.Add(this.PleaseWaitPanel);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(250, 395);
            this.Name = "Freezer";
            this.Opacity = 0.95D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Freezer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Freezer_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.RateBar)).EndInit();
            this.PleaseWaitPanel.ResumeLayout(false);
            this.PleaseWaitPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).EndInit();
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.mapPanel.ResumeLayout(false);
            this.mapPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).EndInit();
            this.ResumeLayout(false);

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