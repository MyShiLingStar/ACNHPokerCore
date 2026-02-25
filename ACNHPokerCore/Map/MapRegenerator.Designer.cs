
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapRegenerator));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            PleaseWaitPanel = new System.Windows.Forms.Panel();
            PauseTimeLabel = new System.Windows.Forms.Label();
            WaitMessagebox = new System.Windows.Forms.RichTextBox();
            MapProgressBar = new System.Windows.Forms.ProgressBar();
            NowLoading = new System.Windows.Forms.PictureBox();
            label29 = new System.Windows.Forms.Label();
            loadMapBtn = new System.Windows.Forms.Button();
            saveMapBtn = new System.Windows.Forms.Button();
            ProgressTimer = new System.Windows.Forms.Timer(components);
            trayIcon = new System.Windows.Forms.NotifyIcon(components);
            startRegen = new System.Windows.Forms.Button();
            hideBtn = new System.Windows.Forms.Button();
            backBtn = new System.Windows.Forms.Button();
            timeLabel = new System.Windows.Forms.Label();
            startRegen2 = new System.Windows.Forms.Button();
            FinMsg = new System.Windows.Forms.RichTextBox();
            delay = new System.Windows.Forms.RichTextBox();
            ms = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            visitorNameBox = new System.Windows.Forms.RichTextBox();
            formToolTip = new System.Windows.Forms.ToolTip(components);
            logBtn = new System.Windows.Forms.Button();
            newLogBtn = new System.Windows.Forms.Button();
            selectLogBtn = new System.Windows.Forms.Button();
            startBtn = new System.Windows.Forms.Button();
            keepVillagerBox = new System.Windows.Forms.CheckBox();
            dodoSetupBtn = new System.Windows.Forms.Button();
            changeDodoBtn = new System.Windows.Forms.Button();
            PauseTimer = new System.Windows.Forms.Timer(components);
            logGridView = new System.Windows.Forms.DataGridView();
            logName = new System.Windows.Forms.Label();
            logPanel = new System.Windows.Forms.Panel();
            mapPanel = new System.Windows.Forms.Panel();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            yCoordinate = new System.Windows.Forms.RichTextBox();
            miniMapBox = new System.Windows.Forms.PictureBox();
            xCoordinate = new System.Windows.Forms.RichTextBox();
            PleaseWaitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NowLoading).BeginInit();
            ((System.ComponentModel.ISupportInitialize)logGridView).BeginInit();
            logPanel.SuspendLayout();
            mapPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)miniMapBox).BeginInit();
            SuspendLayout();
            // 
            // PleaseWaitPanel
            // 
            PleaseWaitPanel.Controls.Add(PauseTimeLabel);
            PleaseWaitPanel.Controls.Add(WaitMessagebox);
            PleaseWaitPanel.Controls.Add(MapProgressBar);
            PleaseWaitPanel.Controls.Add(NowLoading);
            PleaseWaitPanel.Controls.Add(label29);
            PleaseWaitPanel.Location = new System.Drawing.Point(2, 256);
            PleaseWaitPanel.Name = "PleaseWaitPanel";
            PleaseWaitPanel.Size = new System.Drawing.Size(230, 60);
            PleaseWaitPanel.TabIndex = 218;
            PleaseWaitPanel.Visible = false;
            // 
            // PauseTimeLabel
            // 
            PauseTimeLabel.AutoSize = true;
            PauseTimeLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            PauseTimeLabel.ForeColor = System.Drawing.Color.White;
            PauseTimeLabel.Location = new System.Drawing.Point(206, 44);
            PauseTimeLabel.Name = "PauseTimeLabel";
            PauseTimeLabel.Size = new System.Drawing.Size(23, 16);
            PauseTimeLabel.TabIndex = 229;
            PauseTimeLabel.Text = "70";
            PauseTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            PauseTimeLabel.Visible = false;
            // 
            // WaitMessagebox
            // 
            WaitMessagebox.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            WaitMessagebox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            WaitMessagebox.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            WaitMessagebox.ForeColor = System.Drawing.Color.White;
            WaitMessagebox.Location = new System.Drawing.Point(4, 32);
            WaitMessagebox.Multiline = false;
            WaitMessagebox.Name = "WaitMessagebox";
            WaitMessagebox.ReadOnly = true;
            WaitMessagebox.Size = new System.Drawing.Size(220, 27);
            WaitMessagebox.TabIndex = 215;
            WaitMessagebox.Text = "";
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
            NowLoading.Location = new System.Drawing.Point(44, 2);
            NowLoading.Name = "NowLoading";
            NowLoading.Size = new System.Drawing.Size(24, 24);
            NowLoading.TabIndex = 216;
            NowLoading.TabStop = false;
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            label29.ForeColor = System.Drawing.Color.White;
            label29.Location = new System.Drawing.Point(70, 7);
            label29.Name = "label29";
            label29.Size = new System.Drawing.Size(99, 16);
            label29.TabIndex = 215;
            label29.Text = "Please Wait...";
            label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // loadMapBtn
            // 
            loadMapBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            loadMapBtn.FlatAppearance.BorderSize = 0;
            loadMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            loadMapBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            loadMapBtn.ForeColor = System.Drawing.Color.White;
            loadMapBtn.Location = new System.Drawing.Point(13, 43);
            loadMapBtn.Margin = new System.Windows.Forms.Padding(4);
            loadMapBtn.Name = "loadMapBtn";
            loadMapBtn.Size = new System.Drawing.Size(208, 25);
            loadMapBtn.TabIndex = 217;
            loadMapBtn.Text = "Load Map Template";
            formToolTip.SetToolTip(loadMapBtn, "Load a map template file and overwrite the current map.\r\n[WARNING] You will lost every item on your map.");
            loadMapBtn.UseVisualStyleBackColor = false;
            loadMapBtn.Click += LoadMapBtn_Click;
            // 
            // saveMapBtn
            // 
            saveMapBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            saveMapBtn.FlatAppearance.BorderSize = 0;
            saveMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            saveMapBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            saveMapBtn.ForeColor = System.Drawing.Color.White;
            saveMapBtn.Location = new System.Drawing.Point(13, 10);
            saveMapBtn.Margin = new System.Windows.Forms.Padding(4);
            saveMapBtn.Name = "saveMapBtn";
            saveMapBtn.Size = new System.Drawing.Size(208, 25);
            saveMapBtn.TabIndex = 216;
            saveMapBtn.Text = "Create Map Template";
            formToolTip.SetToolTip(saveMapBtn, "Create a map template and save it as a file.");
            saveMapBtn.UseVisualStyleBackColor = false;
            saveMapBtn.Click += SaveMapBtn_Click;
            // 
            // ProgressTimer
            // 
            ProgressTimer.Interval = 500;
            ProgressTimer.Tick += ProgressTimer_Tick;
            // 
            // trayIcon
            // 
            trayIcon.Icon = (System.Drawing.Icon)resources.GetObject("trayIcon.Icon");
            trayIcon.Text = "ACNHPoker : Map Regenerator";
            trayIcon.Visible = true;
            trayIcon.DoubleClick += TrayIcon_DoubleClick;
            // 
            // startRegen
            // 
            startRegen.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            startRegen.FlatAppearance.BorderSize = 0;
            startRegen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            startRegen.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            startRegen.ForeColor = System.Drawing.Color.White;
            startRegen.Location = new System.Drawing.Point(13, 76);
            startRegen.Margin = new System.Windows.Forms.Padding(4);
            startRegen.Name = "startRegen";
            startRegen.Size = new System.Drawing.Size(208, 25);
            startRegen.TabIndex = 219;
            startRegen.Tag = "Start";
            startRegen.Text = "Cast Regen";
            formToolTip.SetToolTip(startRegen, "Keep refreshing the map with a saved map template file.\r\n[WARNING] This option will delete every item dropped/placed on empty space.\r\n");
            startRegen.UseVisualStyleBackColor = false;
            startRegen.Click += StartRegen_Click;
            // 
            // hideBtn
            // 
            hideBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            hideBtn.FlatAppearance.BorderSize = 0;
            hideBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            hideBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            hideBtn.ForeColor = System.Drawing.Color.White;
            hideBtn.Location = new System.Drawing.Point(156, 324);
            hideBtn.Margin = new System.Windows.Forms.Padding(4);
            hideBtn.Name = "hideBtn";
            hideBtn.Size = new System.Drawing.Size(65, 25);
            hideBtn.TabIndex = 220;
            hideBtn.Text = "Hide";
            formToolTip.SetToolTip(hideBtn, "Hide this window to tray.");
            hideBtn.UseVisualStyleBackColor = false;
            hideBtn.Click += HideBtn_Click;
            // 
            // backBtn
            // 
            backBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            backBtn.FlatAppearance.BorderSize = 0;
            backBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            backBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            backBtn.ForeColor = System.Drawing.Color.White;
            backBtn.Location = new System.Drawing.Point(13, 324);
            backBtn.Margin = new System.Windows.Forms.Padding(4);
            backBtn.Name = "backBtn";
            backBtn.Size = new System.Drawing.Size(65, 25);
            backBtn.TabIndex = 221;
            backBtn.Text = "Back";
            backBtn.UseVisualStyleBackColor = false;
            backBtn.Click += BackBtn_Click;
            // 
            // timeLabel
            // 
            timeLabel.AutoSize = true;
            timeLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            timeLabel.ForeColor = System.Drawing.Color.White;
            timeLabel.Location = new System.Drawing.Point(86, 308);
            timeLabel.Name = "timeLabel";
            timeLabel.Size = new System.Drawing.Size(0, 16);
            timeLabel.TabIndex = 222;
            timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // startRegen2
            // 
            startRegen2.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            startRegen2.FlatAppearance.BorderSize = 0;
            startRegen2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            startRegen2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            startRegen2.ForeColor = System.Drawing.Color.White;
            startRegen2.Location = new System.Drawing.Point(13, 109);
            startRegen2.Margin = new System.Windows.Forms.Padding(4);
            startRegen2.Name = "startRegen2";
            startRegen2.Size = new System.Drawing.Size(208, 25);
            startRegen2.TabIndex = 223;
            startRegen2.Tag = "Start";
            startRegen2.Text = "Cast Moogle Regenja";
            formToolTip.SetToolTip(startRegen2, "Keep refreshing the map with a saved map template file.\r\n[WARNING] This option will ignore empty space to preserve dropped item.\r\n");
            startRegen2.UseVisualStyleBackColor = false;
            startRegen2.Click += StartRegen2_Click;
            // 
            // FinMsg
            // 
            FinMsg.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            FinMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            FinMsg.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            FinMsg.ForeColor = System.Drawing.Color.White;
            FinMsg.Location = new System.Drawing.Point(6, 281);
            FinMsg.Multiline = false;
            FinMsg.Name = "FinMsg";
            FinMsg.ReadOnly = true;
            FinMsg.Size = new System.Drawing.Size(220, 27);
            FinMsg.TabIndex = 217;
            FinMsg.Text = "";
            FinMsg.Visible = false;
            // 
            // delay
            // 
            delay.BackColor = System.Drawing.Color.FromArgb(64, 68, 75);
            delay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            delay.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            delay.ForeColor = System.Drawing.Color.White;
            delay.Location = new System.Drawing.Point(127, 218);
            delay.MaxLength = 8;
            delay.Multiline = false;
            delay.Name = "delay";
            delay.Size = new System.Drawing.Size(72, 20);
            delay.TabIndex = 224;
            delay.Text = "50";
            delay.KeyPress += Delay_KeyPress;
            // 
            // ms
            // 
            ms.AutoSize = true;
            ms.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            ms.ForeColor = System.Drawing.Color.White;
            ms.Location = new System.Drawing.Point(198, 219);
            ms.Name = "ms";
            ms.Size = new System.Drawing.Size(27, 16);
            ms.TabIndex = 217;
            ms.Text = "ms";
            ms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(10, 219);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(113, 16);
            label1.TabIndex = 225;
            label1.Text = "Refresh Delay :";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            label2.ForeColor = System.Drawing.Color.White;
            label2.Location = new System.Drawing.Point(30, 241);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(93, 16);
            label2.TabIndex = 227;
            label2.Text = "Last Visitor :";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // visitorNameBox
            // 
            visitorNameBox.BackColor = System.Drawing.Color.FromArgb(64, 68, 75);
            visitorNameBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            visitorNameBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            visitorNameBox.ForeColor = System.Drawing.Color.White;
            visitorNameBox.Location = new System.Drawing.Point(127, 240);
            visitorNameBox.MaxLength = 8;
            visitorNameBox.Multiline = false;
            visitorNameBox.Name = "visitorNameBox";
            visitorNameBox.Size = new System.Drawing.Size(94, 20);
            visitorNameBox.TabIndex = 228;
            visitorNameBox.Text = "";
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
            // logBtn
            // 
            logBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            logBtn.FlatAppearance.BorderSize = 0;
            logBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            logBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            logBtn.ForeColor = System.Drawing.Color.White;
            logBtn.Location = new System.Drawing.Point(84, 324);
            logBtn.Margin = new System.Windows.Forms.Padding(4);
            logBtn.Name = "logBtn";
            logBtn.Size = new System.Drawing.Size(66, 25);
            logBtn.TabIndex = 229;
            logBtn.Text = "Log";
            formToolTip.SetToolTip(logBtn, "Show/Hide the visitor log.");
            logBtn.UseVisualStyleBackColor = false;
            logBtn.Click += LogBtn_Click;
            // 
            // newLogBtn
            // 
            newLogBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            newLogBtn.FlatAppearance.BorderSize = 0;
            newLogBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            newLogBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            newLogBtn.ForeColor = System.Drawing.Color.White;
            newLogBtn.Location = new System.Drawing.Point(4, 4);
            newLogBtn.Margin = new System.Windows.Forms.Padding(4);
            newLogBtn.Name = "newLogBtn";
            newLogBtn.Size = new System.Drawing.Size(68, 25);
            newLogBtn.TabIndex = 231;
            newLogBtn.Text = "New";
            formToolTip.SetToolTip(newLogBtn, "Create a new visitor log file.");
            newLogBtn.UseVisualStyleBackColor = false;
            newLogBtn.Click += NewLogBtn_Click;
            // 
            // selectLogBtn
            // 
            selectLogBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            selectLogBtn.FlatAppearance.BorderSize = 0;
            selectLogBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            selectLogBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            selectLogBtn.ForeColor = System.Drawing.Color.White;
            selectLogBtn.Location = new System.Drawing.Point(80, 4);
            selectLogBtn.Margin = new System.Windows.Forms.Padding(4);
            selectLogBtn.Name = "selectLogBtn";
            selectLogBtn.Size = new System.Drawing.Size(68, 25);
            selectLogBtn.TabIndex = 232;
            selectLogBtn.Text = "Select...";
            formToolTip.SetToolTip(selectLogBtn, "Select another visitor log file.");
            selectLogBtn.UseVisualStyleBackColor = false;
            selectLogBtn.Click += SelectLogBtn_Click;
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
            // keepVillagerBox
            // 
            keepVillagerBox.AutoSize = true;
            keepVillagerBox.BackColor = System.Drawing.Color.Transparent;
            keepVillagerBox.Checked = true;
            keepVillagerBox.CheckState = System.Windows.Forms.CheckState.Checked;
            keepVillagerBox.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            keepVillagerBox.ForeColor = System.Drawing.Color.White;
            keepVillagerBox.Location = new System.Drawing.Point(41, 137);
            keepVillagerBox.Name = "keepVillagerBox";
            keepVillagerBox.Size = new System.Drawing.Size(154, 20);
            keepVillagerBox.TabIndex = 238;
            keepVillagerBox.Text = "Keep Village State";
            formToolTip.SetToolTip(keepVillagerBox, "For keeping villagers in the moving out state (In boxes & sweeping floor).\r\n\r\nPlease set the villager(s) to moving out state BEFORE you start the regenerator.");
            keepVillagerBox.UseVisualStyleBackColor = false;
            // 
            // dodoSetupBtn
            // 
            dodoSetupBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            dodoSetupBtn.FlatAppearance.BorderSize = 0;
            dodoSetupBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            dodoSetupBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            dodoSetupBtn.ForeColor = System.Drawing.Color.White;
            dodoSetupBtn.Location = new System.Drawing.Point(13, 158);
            dodoSetupBtn.Margin = new System.Windows.Forms.Padding(4);
            dodoSetupBtn.Name = "dodoSetupBtn";
            dodoSetupBtn.Size = new System.Drawing.Size(208, 25);
            dodoSetupBtn.TabIndex = 240;
            dodoSetupBtn.Tag = "Enable";
            dodoSetupBtn.Text = "Enable Dodo Helper";
            formToolTip.SetToolTip(dodoSetupBtn, resources.GetString("dodoSetupBtn.ToolTip"));
            dodoSetupBtn.UseVisualStyleBackColor = false;
            dodoSetupBtn.Click += DodoHelperBtn_Click;
            // 
            // changeDodoBtn
            // 
            changeDodoBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            changeDodoBtn.FlatAppearance.BorderSize = 0;
            changeDodoBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            changeDodoBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            changeDodoBtn.ForeColor = System.Drawing.Color.White;
            changeDodoBtn.Location = new System.Drawing.Point(13, 191);
            changeDodoBtn.Margin = new System.Windows.Forms.Padding(4);
            changeDodoBtn.Name = "changeDodoBtn";
            changeDodoBtn.Size = new System.Drawing.Size(208, 25);
            changeDodoBtn.TabIndex = 241;
            changeDodoBtn.Tag = "";
            changeDodoBtn.Text = "Change Dodo Path";
            formToolTip.SetToolTip(changeDodoBtn, "Change the path where the dodo code is stored.");
            changeDodoBtn.UseVisualStyleBackColor = false;
            changeDodoBtn.Click += ChangeDodoBtn_Click;
            // 
            // PauseTimer
            // 
            PauseTimer.Interval = 1000;
            PauseTimer.Tick += PauseTimer_Tick;
            // 
            // logGridView
            // 
            logGridView.AllowUserToAddRows = false;
            logGridView.AllowUserToDeleteRows = false;
            logGridView.AllowUserToResizeRows = false;
            logGridView.BackgroundColor = System.Drawing.Color.FromArgb(47, 49, 54);
            logGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle17.BackColor = System.Drawing.Color.FromArgb(57, 60, 67);
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle17.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.Color.FromArgb(57, 60, 67);
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            logGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
            logGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.Color.FromArgb(47, 49, 54);
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle18.ForeColor = System.Drawing.Color.FromArgb(114, 105, 110);
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.Color.FromArgb(57, 60, 67);
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            logGridView.DefaultCellStyle = dataGridViewCellStyle18;
            logGridView.EnableHeadersVisualStyles = false;
            logGridView.Location = new System.Drawing.Point(4, 36);
            logGridView.MultiSelect = false;
            logGridView.Name = "logGridView";
            logGridView.ReadOnly = true;
            logGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            logGridView.RowHeadersVisible = false;
            logGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            logGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            logGridView.Size = new System.Drawing.Size(345, 265);
            logGridView.TabIndex = 230;
            // 
            // logName
            // 
            logName.AutoSize = true;
            logName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            logName.ForeColor = System.Drawing.Color.White;
            logName.Location = new System.Drawing.Point(150, 9);
            logName.Name = "logName";
            logName.Size = new System.Drawing.Size(205, 16);
            logName.TabIndex = 233;
            logName.Text = "FFFFFFFFFFFFFFFFFFFFFF";
            logName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logPanel
            // 
            logPanel.Controls.Add(newLogBtn);
            logPanel.Controls.Add(logName);
            logPanel.Controls.Add(logGridView);
            logPanel.Controls.Add(selectLogBtn);
            logPanel.Location = new System.Drawing.Point(233, 9);
            logPanel.Name = "logPanel";
            logPanel.Size = new System.Drawing.Size(377, 304);
            logPanel.TabIndex = 234;
            logPanel.Visible = false;
            // 
            // mapPanel
            // 
            mapPanel.Controls.Add(startBtn);
            mapPanel.Controls.Add(label4);
            mapPanel.Controls.Add(label3);
            mapPanel.Controls.Add(yCoordinate);
            mapPanel.Controls.Add(miniMapBox);
            mapPanel.Controls.Add(xCoordinate);
            mapPanel.Location = new System.Drawing.Point(233, 9);
            mapPanel.Name = "mapPanel";
            mapPanel.Size = new System.Drawing.Size(377, 304);
            mapPanel.TabIndex = 235;
            mapPanel.Visible = false;
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
            // MapRegenerator
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            ClientSize = new System.Drawing.Size(234, 356);
            Controls.Add(changeDodoBtn);
            Controls.Add(dodoSetupBtn);
            Controls.Add(label1);
            Controls.Add(backBtn);
            Controls.Add(logBtn);
            Controls.Add(hideBtn);
            Controls.Add(visitorNameBox);
            Controls.Add(label2);
            Controls.Add(timeLabel);
            Controls.Add(ms);
            Controls.Add(delay);
            Controls.Add(startRegen2);
            Controls.Add(startRegen);
            Controls.Add(PleaseWaitPanel);
            Controls.Add(loadMapBtn);
            Controls.Add(saveMapBtn);
            Controls.Add(FinMsg);
            Controls.Add(keepVillagerBox);
            Controls.Add(mapPanel);
            Controls.Add(logPanel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(250, 395);
            Name = "MapRegenerator";
            Opacity = 0.95D;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Map Regenerator";
            FormClosed += MapRegenerator_FormClosed;
            Move += MapRegenerator_Move;
            PleaseWaitPanel.ResumeLayout(false);
            PleaseWaitPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NowLoading).EndInit();
            ((System.ComponentModel.ISupportInitialize)logGridView).EndInit();
            logPanel.ResumeLayout(false);
            logPanel.PerformLayout();
            mapPanel.ResumeLayout(false);
            mapPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)miniMapBox).EndInit();
            ResumeLayout(false);
            PerformLayout();

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