
namespace ACNHPokerCore
{
    partial class MapRegenerator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapRegenerator));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.PleaseWaitPanel = new System.Windows.Forms.Panel();
            this.PauseTimeLabel = new System.Windows.Forms.Label();
            this.WaitMessagebox = new System.Windows.Forms.RichTextBox();
            this.MapProgressBar = new System.Windows.Forms.ProgressBar();
            this.NowLoading = new System.Windows.Forms.PictureBox();
            this.label29 = new System.Windows.Forms.Label();
            this.loadMapBtn = new System.Windows.Forms.Button();
            this.saveMapBtn = new System.Windows.Forms.Button();
            this.ProgressTimer = new System.Windows.Forms.Timer(this.components);
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.startRegen = new System.Windows.Forms.Button();
            this.hideBtn = new System.Windows.Forms.Button();
            this.backBtn = new System.Windows.Forms.Button();
            this.timeLabel = new System.Windows.Forms.Label();
            this.startRegen2 = new System.Windows.Forms.Button();
            this.FinMsg = new System.Windows.Forms.RichTextBox();
            this.delay = new System.Windows.Forms.RichTextBox();
            this.ms = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.visitorNameBox = new System.Windows.Forms.RichTextBox();
            this.formToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.logBtn = new System.Windows.Forms.Button();
            this.newLogBtn = new System.Windows.Forms.Button();
            this.selectLogBtn = new System.Windows.Forms.Button();
            this.startBtn = new System.Windows.Forms.Button();
            this.keepVillagerBox = new System.Windows.Forms.CheckBox();
            this.dodoSetupBtn = new System.Windows.Forms.Button();
            this.changeDodoBtn = new System.Windows.Forms.Button();
            this.PauseTimer = new System.Windows.Forms.Timer(this.components);
            this.logGridView = new System.Windows.Forms.DataGridView();
            this.logName = new System.Windows.Forms.Label();
            this.logPanel = new System.Windows.Forms.Panel();
            this.mapPanel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.yCoordinate = new System.Windows.Forms.RichTextBox();
            this.miniMapBox = new System.Windows.Forms.PictureBox();
            this.xCoordinate = new System.Windows.Forms.RichTextBox();
            this.PleaseWaitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logGridView)).BeginInit();
            this.logPanel.SuspendLayout();
            this.mapPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PleaseWaitPanel
            // 
            this.PleaseWaitPanel.Controls.Add(this.PauseTimeLabel);
            this.PleaseWaitPanel.Controls.Add(this.WaitMessagebox);
            this.PleaseWaitPanel.Controls.Add(this.MapProgressBar);
            this.PleaseWaitPanel.Controls.Add(this.NowLoading);
            this.PleaseWaitPanel.Controls.Add(this.label29);
            this.PleaseWaitPanel.Location = new System.Drawing.Point(2, 256);
            this.PleaseWaitPanel.Name = "PleaseWaitPanel";
            this.PleaseWaitPanel.Size = new System.Drawing.Size(230, 60);
            this.PleaseWaitPanel.TabIndex = 218;
            this.PleaseWaitPanel.Visible = false;
            // 
            // PauseTimeLabel
            // 
            this.PauseTimeLabel.AutoSize = true;
            this.PauseTimeLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.PauseTimeLabel.ForeColor = System.Drawing.Color.White;
            this.PauseTimeLabel.Location = new System.Drawing.Point(206, 44);
            this.PauseTimeLabel.Name = "PauseTimeLabel";
            this.PauseTimeLabel.Size = new System.Drawing.Size(23, 16);
            this.PauseTimeLabel.TabIndex = 229;
            this.PauseTimeLabel.Text = "70";
            this.PauseTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PauseTimeLabel.Visible = false;
            // 
            // WaitMessagebox
            // 
            this.WaitMessagebox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.WaitMessagebox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.WaitMessagebox.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.WaitMessagebox.ForeColor = System.Drawing.Color.White;
            this.WaitMessagebox.Location = new System.Drawing.Point(4, 32);
            this.WaitMessagebox.Multiline = false;
            this.WaitMessagebox.Name = "WaitMessagebox";
            this.WaitMessagebox.ReadOnly = true;
            this.WaitMessagebox.Size = new System.Drawing.Size(220, 27);
            this.WaitMessagebox.TabIndex = 215;
            this.WaitMessagebox.Text = "";
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
            this.NowLoading.Location = new System.Drawing.Point(44, 2);
            this.NowLoading.Name = "NowLoading";
            this.NowLoading.Size = new System.Drawing.Size(24, 24);
            this.NowLoading.TabIndex = 216;
            this.NowLoading.TabStop = false;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label29.ForeColor = System.Drawing.Color.White;
            this.label29.Location = new System.Drawing.Point(70, 7);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(99, 16);
            this.label29.TabIndex = 215;
            this.label29.Text = "Please Wait...";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // loadMapBtn
            // 
            this.loadMapBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.loadMapBtn.FlatAppearance.BorderSize = 0;
            this.loadMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadMapBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.loadMapBtn.ForeColor = System.Drawing.Color.White;
            this.loadMapBtn.Location = new System.Drawing.Point(13, 43);
            this.loadMapBtn.Margin = new System.Windows.Forms.Padding(4);
            this.loadMapBtn.Name = "loadMapBtn";
            this.loadMapBtn.Size = new System.Drawing.Size(208, 25);
            this.loadMapBtn.TabIndex = 217;
            this.loadMapBtn.Text = "Load Map Template";
            this.formToolTip.SetToolTip(this.loadMapBtn, "Load a .nhf file and overwrite the whole map. (Layer 1 only)\r\n[WARNING] You will " +
        "lost every item on your map.");
            this.loadMapBtn.UseVisualStyleBackColor = false;
            this.loadMapBtn.Click += new System.EventHandler(this.loadMapBtn_Click);
            // 
            // saveMapBtn
            // 
            this.saveMapBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.saveMapBtn.FlatAppearance.BorderSize = 0;
            this.saveMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveMapBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.saveMapBtn.ForeColor = System.Drawing.Color.White;
            this.saveMapBtn.Location = new System.Drawing.Point(13, 10);
            this.saveMapBtn.Margin = new System.Windows.Forms.Padding(4);
            this.saveMapBtn.Name = "saveMapBtn";
            this.saveMapBtn.Size = new System.Drawing.Size(208, 25);
            this.saveMapBtn.TabIndex = 216;
            this.saveMapBtn.Text = "Create Map Template";
            this.formToolTip.SetToolTip(this.saveMapBtn, "Create a Map template and save it to a .nhf file. (Layer 1 only)");
            this.saveMapBtn.UseVisualStyleBackColor = false;
            this.saveMapBtn.Click += new System.EventHandler(this.saveMapBtn_Click);
            // 
            // ProgressTimer
            // 
            this.ProgressTimer.Interval = 500;
            this.ProgressTimer.Tick += new System.EventHandler(this.ProgressTimer_Tick);
            // 
            // trayIcon
            // 
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "ACNHPoker : Map Regenerator";
            this.trayIcon.Visible = true;
            this.trayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClick);
            // 
            // startRegen
            // 
            this.startRegen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.startRegen.FlatAppearance.BorderSize = 0;
            this.startRegen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startRegen.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.startRegen.ForeColor = System.Drawing.Color.White;
            this.startRegen.Location = new System.Drawing.Point(13, 76);
            this.startRegen.Margin = new System.Windows.Forms.Padding(4);
            this.startRegen.Name = "startRegen";
            this.startRegen.Size = new System.Drawing.Size(208, 25);
            this.startRegen.TabIndex = 219;
            this.startRegen.Tag = "Start";
            this.startRegen.Text = "Cast Regen";
            this.formToolTip.SetToolTip(this.startRegen, "Keep refreshing the map with a saved map template (.nhf). (Layer 1 only)\r\n[WARNIN" +
        "G] This option will delete every item dropped/placed on empty space.\r\n");
            this.startRegen.UseVisualStyleBackColor = false;
            this.startRegen.Click += new System.EventHandler(this.startRegen_Click);
            // 
            // hideBtn
            // 
            this.hideBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.hideBtn.FlatAppearance.BorderSize = 0;
            this.hideBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hideBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.hideBtn.ForeColor = System.Drawing.Color.White;
            this.hideBtn.Location = new System.Drawing.Point(156, 324);
            this.hideBtn.Margin = new System.Windows.Forms.Padding(4);
            this.hideBtn.Name = "hideBtn";
            this.hideBtn.Size = new System.Drawing.Size(65, 25);
            this.hideBtn.TabIndex = 220;
            this.hideBtn.Text = "Hide";
            this.formToolTip.SetToolTip(this.hideBtn, "Hide this window to tray.");
            this.hideBtn.UseVisualStyleBackColor = false;
            this.hideBtn.Click += new System.EventHandler(this.hideBtn_Click);
            // 
            // backBtn
            // 
            this.backBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.backBtn.FlatAppearance.BorderSize = 0;
            this.backBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.backBtn.ForeColor = System.Drawing.Color.White;
            this.backBtn.Location = new System.Drawing.Point(13, 324);
            this.backBtn.Margin = new System.Windows.Forms.Padding(4);
            this.backBtn.Name = "backBtn";
            this.backBtn.Size = new System.Drawing.Size(65, 25);
            this.backBtn.TabIndex = 221;
            this.backBtn.Text = "Back";
            this.backBtn.UseVisualStyleBackColor = false;
            this.backBtn.Click += new System.EventHandler(this.backBtn_Click);
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.timeLabel.ForeColor = System.Drawing.Color.White;
            this.timeLabel.Location = new System.Drawing.Point(86, 308);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(0, 16);
            this.timeLabel.TabIndex = 222;
            this.timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // startRegen2
            // 
            this.startRegen2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.startRegen2.FlatAppearance.BorderSize = 0;
            this.startRegen2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startRegen2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.startRegen2.ForeColor = System.Drawing.Color.White;
            this.startRegen2.Location = new System.Drawing.Point(13, 109);
            this.startRegen2.Margin = new System.Windows.Forms.Padding(4);
            this.startRegen2.Name = "startRegen2";
            this.startRegen2.Size = new System.Drawing.Size(208, 25);
            this.startRegen2.TabIndex = 223;
            this.startRegen2.Tag = "Start";
            this.startRegen2.Text = "Cast Moogle Regenja";
            this.formToolTip.SetToolTip(this.startRegen2, "Keep refreshing the map with a saved map template (.nhf). (Layer 1 only)\r\n[WARNIN" +
        "G] This option will ignore empty space to preserve dropped item.\r\n");
            this.startRegen2.UseVisualStyleBackColor = false;
            this.startRegen2.Click += new System.EventHandler(this.startRegen2_Click);
            // 
            // FinMsg
            // 
            this.FinMsg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.FinMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FinMsg.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.FinMsg.ForeColor = System.Drawing.Color.White;
            this.FinMsg.Location = new System.Drawing.Point(6, 281);
            this.FinMsg.Multiline = false;
            this.FinMsg.Name = "FinMsg";
            this.FinMsg.ReadOnly = true;
            this.FinMsg.Size = new System.Drawing.Size(220, 27);
            this.FinMsg.TabIndex = 217;
            this.FinMsg.Text = "";
            this.FinMsg.Visible = false;
            // 
            // delay
            // 
            this.delay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.delay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.delay.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.delay.ForeColor = System.Drawing.Color.White;
            this.delay.Location = new System.Drawing.Point(127, 218);
            this.delay.MaxLength = 8;
            this.delay.Multiline = false;
            this.delay.Name = "delay";
            this.delay.Size = new System.Drawing.Size(72, 20);
            this.delay.TabIndex = 224;
            this.delay.Text = "50";
            this.delay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.delay_KeyPress);
            // 
            // ms
            // 
            this.ms.AutoSize = true;
            this.ms.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ms.ForeColor = System.Drawing.Color.White;
            this.ms.Location = new System.Drawing.Point(198, 219);
            this.ms.Name = "ms";
            this.ms.Size = new System.Drawing.Size(27, 16);
            this.ms.TabIndex = 217;
            this.ms.Text = "ms";
            this.ms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(10, 219);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 16);
            this.label1.TabIndex = 225;
            this.label1.Text = "Refresh Delay :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(30, 241);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 16);
            this.label2.TabIndex = 227;
            this.label2.Text = "Last Visitor :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // visitorNameBox
            // 
            this.visitorNameBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.visitorNameBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.visitorNameBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.visitorNameBox.ForeColor = System.Drawing.Color.White;
            this.visitorNameBox.Location = new System.Drawing.Point(127, 240);
            this.visitorNameBox.MaxLength = 8;
            this.visitorNameBox.Multiline = false;
            this.visitorNameBox.Name = "visitorNameBox";
            this.visitorNameBox.Size = new System.Drawing.Size(94, 20);
            this.visitorNameBox.TabIndex = 228;
            this.visitorNameBox.Text = "";
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
            // logBtn
            // 
            this.logBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.logBtn.FlatAppearance.BorderSize = 0;
            this.logBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.logBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.logBtn.ForeColor = System.Drawing.Color.White;
            this.logBtn.Location = new System.Drawing.Point(84, 324);
            this.logBtn.Margin = new System.Windows.Forms.Padding(4);
            this.logBtn.Name = "logBtn";
            this.logBtn.Size = new System.Drawing.Size(66, 25);
            this.logBtn.TabIndex = 229;
            this.logBtn.Text = "Log";
            this.formToolTip.SetToolTip(this.logBtn, "Show/Hide the visitor log.");
            this.logBtn.UseVisualStyleBackColor = false;
            this.logBtn.Click += new System.EventHandler(this.logBtn_Click);
            // 
            // newLogBtn
            // 
            this.newLogBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.newLogBtn.FlatAppearance.BorderSize = 0;
            this.newLogBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newLogBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.newLogBtn.ForeColor = System.Drawing.Color.White;
            this.newLogBtn.Location = new System.Drawing.Point(4, 4);
            this.newLogBtn.Margin = new System.Windows.Forms.Padding(4);
            this.newLogBtn.Name = "newLogBtn";
            this.newLogBtn.Size = new System.Drawing.Size(68, 25);
            this.newLogBtn.TabIndex = 231;
            this.newLogBtn.Text = "New";
            this.formToolTip.SetToolTip(this.newLogBtn, "Create a new visitor log file.");
            this.newLogBtn.UseVisualStyleBackColor = false;
            this.newLogBtn.Click += new System.EventHandler(this.newLogBtn_Click);
            // 
            // selectLogBtn
            // 
            this.selectLogBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.selectLogBtn.FlatAppearance.BorderSize = 0;
            this.selectLogBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectLogBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.selectLogBtn.ForeColor = System.Drawing.Color.White;
            this.selectLogBtn.Location = new System.Drawing.Point(80, 4);
            this.selectLogBtn.Margin = new System.Windows.Forms.Padding(4);
            this.selectLogBtn.Name = "selectLogBtn";
            this.selectLogBtn.Size = new System.Drawing.Size(68, 25);
            this.selectLogBtn.TabIndex = 232;
            this.selectLogBtn.Text = "Select...";
            this.formToolTip.SetToolTip(this.selectLogBtn, "Select another visitor log file.");
            this.selectLogBtn.UseVisualStyleBackColor = false;
            this.selectLogBtn.Click += new System.EventHandler(this.selectLogBtn_Click);
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
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // keepVillagerBox
            // 
            this.keepVillagerBox.AutoSize = true;
            this.keepVillagerBox.BackColor = System.Drawing.Color.Transparent;
            this.keepVillagerBox.Checked = true;
            this.keepVillagerBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.keepVillagerBox.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.keepVillagerBox.ForeColor = System.Drawing.Color.White;
            this.keepVillagerBox.Location = new System.Drawing.Point(41, 137);
            this.keepVillagerBox.Name = "keepVillagerBox";
            this.keepVillagerBox.Size = new System.Drawing.Size(154, 20);
            this.keepVillagerBox.TabIndex = 238;
            this.keepVillagerBox.Text = "Keep Village State";
            this.formToolTip.SetToolTip(this.keepVillagerBox, "For keeping villagers in the moving out state (In boxes & sweeping floor).\r\n\r\nPle" +
        "ase set the villager(s) to moving out state BEFORE you start the regenerator.");
            this.keepVillagerBox.UseVisualStyleBackColor = false;
            // 
            // dodoSetupBtn
            // 
            this.dodoSetupBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.dodoSetupBtn.FlatAppearance.BorderSize = 0;
            this.dodoSetupBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dodoSetupBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.dodoSetupBtn.ForeColor = System.Drawing.Color.White;
            this.dodoSetupBtn.Location = new System.Drawing.Point(13, 158);
            this.dodoSetupBtn.Margin = new System.Windows.Forms.Padding(4);
            this.dodoSetupBtn.Name = "dodoSetupBtn";
            this.dodoSetupBtn.Size = new System.Drawing.Size(208, 25);
            this.dodoSetupBtn.TabIndex = 240;
            this.dodoSetupBtn.Tag = "Enable";
            this.dodoSetupBtn.Text = "Enable Dodo Helper";
            this.formToolTip.SetToolTip(this.dodoSetupBtn, resources.GetString("dodoSetupBtn.ToolTip"));
            this.dodoSetupBtn.UseVisualStyleBackColor = false;
            this.dodoSetupBtn.Click += new System.EventHandler(this.dodoHelperBtn_Click);
            // 
            // changeDodoBtn
            // 
            this.changeDodoBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.changeDodoBtn.FlatAppearance.BorderSize = 0;
            this.changeDodoBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.changeDodoBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.changeDodoBtn.ForeColor = System.Drawing.Color.White;
            this.changeDodoBtn.Location = new System.Drawing.Point(13, 191);
            this.changeDodoBtn.Margin = new System.Windows.Forms.Padding(4);
            this.changeDodoBtn.Name = "changeDodoBtn";
            this.changeDodoBtn.Size = new System.Drawing.Size(208, 25);
            this.changeDodoBtn.TabIndex = 241;
            this.changeDodoBtn.Tag = "";
            this.changeDodoBtn.Text = "Change Dodo Path";
            this.formToolTip.SetToolTip(this.changeDodoBtn, "Change the path where the dodo code is stored.");
            this.changeDodoBtn.UseVisualStyleBackColor = false;
            this.changeDodoBtn.Click += new System.EventHandler(this.changeDodoBtn_Click);
            // 
            // PauseTimer
            // 
            this.PauseTimer.Interval = 1000;
            this.PauseTimer.Tick += new System.EventHandler(this.PauseTimer_Tick);
            // 
            // logGridView
            // 
            this.logGridView.AllowUserToAddRows = false;
            this.logGridView.AllowUserToDeleteRows = false;
            this.logGridView.AllowUserToResizeRows = false;
            this.logGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(49)))), ((int)(((byte)(54)))));
            this.logGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(60)))), ((int)(((byte)(67)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(60)))), ((int)(((byte)(67)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.logGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.logGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(49)))), ((int)(((byte)(54)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(105)))), ((int)(((byte)(110)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(60)))), ((int)(((byte)(67)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.logGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.logGridView.EnableHeadersVisualStyles = false;
            this.logGridView.Location = new System.Drawing.Point(4, 36);
            this.logGridView.MultiSelect = false;
            this.logGridView.Name = "logGridView";
            this.logGridView.ReadOnly = true;
            this.logGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.logGridView.RowHeadersVisible = false;
            this.logGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.logGridView.Size = new System.Drawing.Size(345, 265);
            this.logGridView.TabIndex = 230;
            // 
            // logName
            // 
            this.logName.AutoSize = true;
            this.logName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.logName.ForeColor = System.Drawing.Color.White;
            this.logName.Location = new System.Drawing.Point(150, 9);
            this.logName.Name = "logName";
            this.logName.Size = new System.Drawing.Size(205, 16);
            this.logName.TabIndex = 233;
            this.logName.Text = "FFFFFFFFFFFFFFFFFFFFFF";
            this.logName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logPanel
            // 
            this.logPanel.Controls.Add(this.newLogBtn);
            this.logPanel.Controls.Add(this.logName);
            this.logPanel.Controls.Add(this.logGridView);
            this.logPanel.Controls.Add(this.selectLogBtn);
            this.logPanel.Location = new System.Drawing.Point(233, 9);
            this.logPanel.Name = "logPanel";
            this.logPanel.Size = new System.Drawing.Size(377, 304);
            this.logPanel.TabIndex = 234;
            this.logPanel.Visible = false;
            // 
            // mapPanel
            // 
            this.mapPanel.Controls.Add(this.startBtn);
            this.mapPanel.Controls.Add(this.label4);
            this.mapPanel.Controls.Add(this.label3);
            this.mapPanel.Controls.Add(this.yCoordinate);
            this.mapPanel.Controls.Add(this.miniMapBox);
            this.mapPanel.Controls.Add(this.xCoordinate);
            this.mapPanel.Location = new System.Drawing.Point(233, 9);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(377, 304);
            this.mapPanel.TabIndex = 235;
            this.mapPanel.Visible = false;
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
            this.miniMapBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.miniMapBox_MouseDown);
            this.miniMapBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.miniMapBox_MouseMove);
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
            // MapRegenerator
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(234, 356);
            this.Controls.Add(this.changeDodoBtn);
            this.Controls.Add(this.dodoSetupBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.backBtn);
            this.Controls.Add(this.logBtn);
            this.Controls.Add(this.hideBtn);
            this.Controls.Add(this.visitorNameBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.ms);
            this.Controls.Add(this.delay);
            this.Controls.Add(this.startRegen2);
            this.Controls.Add(this.startRegen);
            this.Controls.Add(this.PleaseWaitPanel);
            this.Controls.Add(this.loadMapBtn);
            this.Controls.Add(this.saveMapBtn);
            this.Controls.Add(this.FinMsg);
            this.Controls.Add(this.keepVillagerBox);
            this.Controls.Add(this.mapPanel);
            this.Controls.Add(this.logPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(250, 395);
            this.Name = "MapRegenerator";
            this.Opacity = 0.95D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Map Regenerator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MapRegenerator_FormClosed);
            this.Move += new System.EventHandler(this.MapRegenerator_Move);
            this.PleaseWaitPanel.ResumeLayout(false);
            this.PleaseWaitPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logGridView)).EndInit();
            this.logPanel.ResumeLayout(false);
            this.logPanel.PerformLayout();
            this.mapPanel.ResumeLayout(false);
            this.mapPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel PleaseWaitPanel;
        private System.Windows.Forms.RichTextBox WaitMessagebox;
        private System.Windows.Forms.ProgressBar MapProgressBar;
        private System.Windows.Forms.PictureBox NowLoading;
        private System.Windows.Forms.Button loadMapBtn;
        private System.Windows.Forms.Button saveMapBtn;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Timer ProgressTimer;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.Button startRegen;
        private System.Windows.Forms.Button hideBtn;
        private System.Windows.Forms.Button backBtn;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Button startRegen2;
        private System.Windows.Forms.RichTextBox FinMsg;
        private System.Windows.Forms.RichTextBox delay;
        private System.Windows.Forms.Label ms;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox visitorNameBox;
        private System.Windows.Forms.ToolTip formToolTip;
        private System.Windows.Forms.Timer PauseTimer;
        private System.Windows.Forms.Label PauseTimeLabel;
        private System.Windows.Forms.Button logBtn;
        private System.Windows.Forms.DataGridView logGridView;
        private System.Windows.Forms.Button newLogBtn;
        private System.Windows.Forms.Button selectLogBtn;
        private System.Windows.Forms.Label logName;
        private System.Windows.Forms.Panel logPanel;
        private System.Windows.Forms.Panel mapPanel;
        private System.Windows.Forms.PictureBox miniMapBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox yCoordinate;
        private System.Windows.Forms.RichTextBox xCoordinate;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.CheckBox keepVillagerBox;
        private System.Windows.Forms.Button dodoSetupBtn;
        private System.Windows.Forms.Button changeDodoBtn;
    }
}