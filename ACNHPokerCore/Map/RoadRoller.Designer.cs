namespace ACNHPokerCore
{
    partial class RoadRoller
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoadRoller));
            this.MainDivider = new System.Windows.Forms.TableLayoutPanel();
            this.SubDivider = new System.Windows.Forms.TableLayoutPanel();
            this.MiddlePanel = new System.Windows.Forms.Panel();
            this.MainMap = new System.Windows.Forms.PictureBox();
            this.TopMenuPanel = new System.Windows.Forms.Panel();
            this.DisplayBuildingLabel = new System.Windows.Forms.Label();
            this.DisplayBuildingToggle = new JCS.ToggleSwitch();
            this.HighlightCornerLabel = new System.Windows.Forms.Label();
            this.HighlightCornerToggle = new JCS.ToggleSwitch();
            this.DisplayInfoToggle = new JCS.ToggleSwitch();
            this.DisplayInfoLabel = new System.Windows.Forms.Label();
            this.DisplayRoadLabel = new System.Windows.Forms.Label();
            this.DisplayRoadToggle = new JCS.ToggleSwitch();
            this.LeftMenuPanel = new System.Windows.Forms.Panel();
            this.miniMapBox = new System.Windows.Forms.PictureBox();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.ConfirmBtn = new System.Windows.Forms.Button();
            this.ArchBtn = new System.Windows.Forms.Button();
            this.Elevation0Label = new System.Windows.Forms.Label();
            this.StoneBtn = new System.Windows.Forms.Button();
            this.ElevationLabel = new System.Windows.Forms.Label();
            this.Elevation2Label = new System.Windows.Forms.Label();
            this.Elevation1Label = new System.Windows.Forms.Label();
            this.ElevationBar = new System.Windows.Forms.TrackBar();
            this.CliffBtn = new System.Windows.Forms.Button();
            this.CornerBtn = new System.Windows.Forms.Button();
            this.RiverBtn = new System.Windows.Forms.Button();
            this.DirtBtn = new System.Windows.Forms.Button();
            this.WoodBtn = new System.Windows.Forms.Button();
            this.TileBtn = new System.Windows.Forms.Button();
            this.BrickBtn = new System.Windows.Forms.Button();
            this.SandBtn = new System.Windows.Forms.Button();
            this.DarkDirtBtn = new System.Windows.Forms.Button();
            this.CornerPanel = new System.Windows.Forms.Panel();
            this.PleaseWaitPanel = new System.Windows.Forms.Panel();
            this.MapProgressBar = new System.Windows.Forms.ProgressBar();
            this.NowLoading = new System.Windows.Forms.PictureBox();
            this.PleaseWaitLabel = new System.Windows.Forms.Label();
            this.ProgressTimer = new System.Windows.Forms.Timer(this.components);
            this.MapToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.MainDivider.SuspendLayout();
            this.SubDivider.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainMap)).BeginInit();
            this.TopMenuPanel.SuspendLayout();
            this.LeftMenuPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).BeginInit();
            this.ButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ElevationBar)).BeginInit();
            this.CornerPanel.SuspendLayout();
            this.PleaseWaitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // MainDivider
            // 
            this.MainDivider.ColumnCount = 2;
            this.MainDivider.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.MainDivider.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainDivider.Controls.Add(this.SubDivider, 1, 1);
            this.MainDivider.Controls.Add(this.TopMenuPanel, 1, 0);
            this.MainDivider.Controls.Add(this.LeftMenuPanel, 0, 1);
            this.MainDivider.Controls.Add(this.CornerPanel, 0, 0);
            this.MainDivider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainDivider.Location = new System.Drawing.Point(0, 0);
            this.MainDivider.Name = "MainDivider";
            this.MainDivider.RowCount = 2;
            this.MainDivider.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.MainDivider.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainDivider.Size = new System.Drawing.Size(984, 661);
            this.MainDivider.TabIndex = 0;
            // 
            // SubDivider
            // 
            this.SubDivider.ColumnCount = 3;
            this.SubDivider.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.SubDivider.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SubDivider.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.SubDivider.Controls.Add(this.MiddlePanel, 1, 1);
            this.SubDivider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SubDivider.Location = new System.Drawing.Point(253, 53);
            this.SubDivider.Name = "SubDivider";
            this.SubDivider.RowCount = 3;
            this.SubDivider.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.SubDivider.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SubDivider.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.SubDivider.Size = new System.Drawing.Size(728, 605);
            this.SubDivider.TabIndex = 0;
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.BackColor = System.Drawing.Color.Transparent;
            this.MiddlePanel.Controls.Add(this.MainMap);
            this.MiddlePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MiddlePanel.Location = new System.Drawing.Point(3, 3);
            this.MiddlePanel.Name = "MiddlePanel";
            this.MiddlePanel.Size = new System.Drawing.Size(722, 599);
            this.MiddlePanel.TabIndex = 0;
            // 
            // MainMap
            // 
            this.MainMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainMap.BackColor = System.Drawing.Color.White;
            this.MainMap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.MainMap.Location = new System.Drawing.Point(0, 0);
            this.MainMap.Name = "MainMap";
            this.MainMap.Size = new System.Drawing.Size(50, 50);
            this.MainMap.TabIndex = 0;
            this.MainMap.TabStop = false;
            this.MainMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainMap_MouseDown);
            this.MainMap.MouseLeave += new System.EventHandler(this.MainMap_MouseLeave);
            this.MainMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainMap_MouseMove);
            this.MainMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainMap_MouseUp);
            // 
            // TopMenuPanel
            // 
            this.TopMenuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.TopMenuPanel.Controls.Add(this.DisplayBuildingLabel);
            this.TopMenuPanel.Controls.Add(this.DisplayBuildingToggle);
            this.TopMenuPanel.Controls.Add(this.HighlightCornerLabel);
            this.TopMenuPanel.Controls.Add(this.HighlightCornerToggle);
            this.TopMenuPanel.Controls.Add(this.DisplayInfoToggle);
            this.TopMenuPanel.Controls.Add(this.DisplayInfoLabel);
            this.TopMenuPanel.Controls.Add(this.DisplayRoadLabel);
            this.TopMenuPanel.Controls.Add(this.DisplayRoadToggle);
            this.TopMenuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TopMenuPanel.Location = new System.Drawing.Point(253, 3);
            this.TopMenuPanel.Name = "TopMenuPanel";
            this.TopMenuPanel.Size = new System.Drawing.Size(728, 44);
            this.TopMenuPanel.TabIndex = 1;
            // 
            // DisplayBuildingLabel
            // 
            this.DisplayBuildingLabel.AutoSize = true;
            this.DisplayBuildingLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DisplayBuildingLabel.ForeColor = System.Drawing.Color.White;
            this.DisplayBuildingLabel.Location = new System.Drawing.Point(28, 22);
            this.DisplayBuildingLabel.Name = "DisplayBuildingLabel";
            this.DisplayBuildingLabel.Size = new System.Drawing.Size(131, 16);
            this.DisplayBuildingLabel.TabIndex = 242;
            this.DisplayBuildingLabel.Text = "Display Building : ";
            this.DisplayBuildingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DisplayBuildingToggle
            // 
            this.DisplayBuildingToggle.Location = new System.Drawing.Point(165, 22);
            this.DisplayBuildingToggle.Name = "DisplayBuildingToggle";
            this.DisplayBuildingToggle.OffFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.DisplayBuildingToggle.OnFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.DisplayBuildingToggle.Size = new System.Drawing.Size(35, 16);
            this.DisplayBuildingToggle.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Carbon;
            this.DisplayBuildingToggle.TabIndex = 241;
            this.DisplayBuildingToggle.UseAnimation = false;
            this.DisplayBuildingToggle.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.DisplayBuildingToggle_CheckedChanged);
            // 
            // HighlightCornerLabel
            // 
            this.HighlightCornerLabel.AutoSize = true;
            this.HighlightCornerLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HighlightCornerLabel.ForeColor = System.Drawing.Color.White;
            this.HighlightCornerLabel.Location = new System.Drawing.Point(225, 3);
            this.HighlightCornerLabel.Name = "HighlightCornerLabel";
            this.HighlightCornerLabel.Size = new System.Drawing.Size(131, 16);
            this.HighlightCornerLabel.TabIndex = 240;
            this.HighlightCornerLabel.Text = "Highlight Corner :";
            this.HighlightCornerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HighlightCornerToggle
            // 
            this.HighlightCornerToggle.Location = new System.Drawing.Point(362, 3);
            this.HighlightCornerToggle.Name = "HighlightCornerToggle";
            this.HighlightCornerToggle.OffFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.HighlightCornerToggle.OnFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.HighlightCornerToggle.Size = new System.Drawing.Size(35, 16);
            this.HighlightCornerToggle.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Carbon;
            this.HighlightCornerToggle.TabIndex = 239;
            this.HighlightCornerToggle.UseAnimation = false;
            this.HighlightCornerToggle.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.HighlightCornerToggle_CheckedChanged);
            // 
            // DisplayInfoToggle
            // 
            this.DisplayInfoToggle.Checked = true;
            this.DisplayInfoToggle.Location = new System.Drawing.Point(362, 22);
            this.DisplayInfoToggle.Name = "DisplayInfoToggle";
            this.DisplayInfoToggle.OffFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.DisplayInfoToggle.OnFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.DisplayInfoToggle.Size = new System.Drawing.Size(35, 16);
            this.DisplayInfoToggle.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Carbon;
            this.DisplayInfoToggle.TabIndex = 236;
            this.DisplayInfoToggle.UseAnimation = false;
            // 
            // DisplayInfoLabel
            // 
            this.DisplayInfoLabel.AutoSize = true;
            this.DisplayInfoLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DisplayInfoLabel.ForeColor = System.Drawing.Color.White;
            this.DisplayInfoLabel.Location = new System.Drawing.Point(260, 22);
            this.DisplayInfoLabel.Name = "DisplayInfoLabel";
            this.DisplayInfoLabel.Size = new System.Drawing.Size(100, 16);
            this.DisplayInfoLabel.TabIndex = 217;
            this.DisplayInfoLabel.Text = "Display Info : ";
            this.DisplayInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DisplayRoadLabel
            // 
            this.DisplayRoadLabel.AutoSize = true;
            this.DisplayRoadLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DisplayRoadLabel.ForeColor = System.Drawing.Color.White;
            this.DisplayRoadLabel.Location = new System.Drawing.Point(50, 3);
            this.DisplayRoadLabel.Name = "DisplayRoadLabel";
            this.DisplayRoadLabel.Size = new System.Drawing.Size(109, 16);
            this.DisplayRoadLabel.TabIndex = 238;
            this.DisplayRoadLabel.Text = "Display Road : ";
            this.DisplayRoadLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DisplayRoadToggle
            // 
            this.DisplayRoadToggle.Checked = true;
            this.DisplayRoadToggle.Location = new System.Drawing.Point(165, 3);
            this.DisplayRoadToggle.Name = "DisplayRoadToggle";
            this.DisplayRoadToggle.OffFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.DisplayRoadToggle.OnFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.DisplayRoadToggle.Size = new System.Drawing.Size(35, 16);
            this.DisplayRoadToggle.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Carbon;
            this.DisplayRoadToggle.TabIndex = 237;
            this.DisplayRoadToggle.UseAnimation = false;
            this.DisplayRoadToggle.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.DisplayRoadToggle_CheckedChanged);
            // 
            // LeftMenuPanel
            // 
            this.LeftMenuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.LeftMenuPanel.Controls.Add(this.miniMapBox);
            this.LeftMenuPanel.Controls.Add(this.ButtonPanel);
            this.LeftMenuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LeftMenuPanel.Location = new System.Drawing.Point(3, 53);
            this.LeftMenuPanel.Name = "LeftMenuPanel";
            this.LeftMenuPanel.Size = new System.Drawing.Size(244, 605);
            this.LeftMenuPanel.TabIndex = 2;
            // 
            // miniMapBox
            // 
            this.miniMapBox.BackColor = System.Drawing.Color.Transparent;
            this.miniMapBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.miniMapBox.ErrorImage = null;
            this.miniMapBox.InitialImage = null;
            this.miniMapBox.Location = new System.Drawing.Point(9, 3);
            this.miniMapBox.Margin = new System.Windows.Forms.Padding(0);
            this.miniMapBox.MaximumSize = new System.Drawing.Size(224, 192);
            this.miniMapBox.MinimumSize = new System.Drawing.Size(224, 192);
            this.miniMapBox.Name = "miniMapBox";
            this.miniMapBox.Size = new System.Drawing.Size(224, 192);
            this.miniMapBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.miniMapBox.TabIndex = 236;
            this.miniMapBox.TabStop = false;
            this.miniMapBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.miniMapBox_MouseDown);
            this.miniMapBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.miniMapBox_MouseMove);
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Controls.Add(this.ConfirmBtn);
            this.ButtonPanel.Controls.Add(this.ArchBtn);
            this.ButtonPanel.Controls.Add(this.Elevation0Label);
            this.ButtonPanel.Controls.Add(this.StoneBtn);
            this.ButtonPanel.Controls.Add(this.ElevationLabel);
            this.ButtonPanel.Controls.Add(this.Elevation2Label);
            this.ButtonPanel.Controls.Add(this.Elevation1Label);
            this.ButtonPanel.Controls.Add(this.ElevationBar);
            this.ButtonPanel.Controls.Add(this.CliffBtn);
            this.ButtonPanel.Controls.Add(this.CornerBtn);
            this.ButtonPanel.Controls.Add(this.RiverBtn);
            this.ButtonPanel.Controls.Add(this.DirtBtn);
            this.ButtonPanel.Controls.Add(this.WoodBtn);
            this.ButtonPanel.Controls.Add(this.TileBtn);
            this.ButtonPanel.Controls.Add(this.BrickBtn);
            this.ButtonPanel.Controls.Add(this.SandBtn);
            this.ButtonPanel.Controls.Add(this.DarkDirtBtn);
            this.ButtonPanel.Location = new System.Drawing.Point(9, 198);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(225, 398);
            this.ButtonPanel.TabIndex = 237;
            // 
            // ConfirmBtn
            // 
            this.ConfirmBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfirmBtn.BackColor = System.Drawing.Color.Orange;
            this.ConfirmBtn.FlatAppearance.BorderSize = 0;
            this.ConfirmBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ConfirmBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ConfirmBtn.ForeColor = System.Drawing.Color.White;
            this.ConfirmBtn.Location = new System.Drawing.Point(4, 367);
            this.ConfirmBtn.Margin = new System.Windows.Forms.Padding(4);
            this.ConfirmBtn.Name = "ConfirmBtn";
            this.ConfirmBtn.Size = new System.Drawing.Size(191, 30);
            this.ConfirmBtn.TabIndex = 243;
            this.ConfirmBtn.Text = "Confirm";
            this.ConfirmBtn.UseVisualStyleBackColor = false;
            this.ConfirmBtn.Visible = false;
            this.ConfirmBtn.Click += new System.EventHandler(this.ConfirmBtn_Click);
            // 
            // ArchBtn
            // 
            this.ArchBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.ArchBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ArchBtn.FlatAppearance.BorderSize = 0;
            this.ArchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ArchBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ArchBtn.ForeColor = System.Drawing.Color.White;
            this.ArchBtn.Image = global::ACNHPokerCore.Properties.Resources.pattern;
            this.ArchBtn.Location = new System.Drawing.Point(69, 276);
            this.ArchBtn.Name = "ArchBtn";
            this.ArchBtn.Size = new System.Drawing.Size(60, 60);
            this.ArchBtn.TabIndex = 230;
            this.ArchBtn.Tag = "Other";
            this.ArchBtn.UseVisualStyleBackColor = false;
            this.ArchBtn.Click += new System.EventHandler(this.ArchBtn_Click);
            // 
            // Elevation0Label
            // 
            this.Elevation0Label.AutoSize = true;
            this.Elevation0Label.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Elevation0Label.ForeColor = System.Drawing.Color.White;
            this.Elevation0Label.Location = new System.Drawing.Point(126, 47);
            this.Elevation0Label.Name = "Elevation0Label";
            this.Elevation0Label.Size = new System.Drawing.Size(13, 14);
            this.Elevation0Label.TabIndex = 242;
            this.Elevation0Label.Text = "0";
            this.Elevation0Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StoneBtn
            // 
            this.StoneBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.StoneBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.StoneBtn.FlatAppearance.BorderSize = 0;
            this.StoneBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StoneBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.StoneBtn.ForeColor = System.Drawing.Color.White;
            this.StoneBtn.Image = global::ACNHPokerCore.Properties.Resources.stone;
            this.StoneBtn.Location = new System.Drawing.Point(3, 276);
            this.StoneBtn.Name = "StoneBtn";
            this.StoneBtn.Size = new System.Drawing.Size(60, 60);
            this.StoneBtn.TabIndex = 227;
            this.StoneBtn.Tag = "Other";
            this.StoneBtn.UseVisualStyleBackColor = false;
            this.StoneBtn.Click += new System.EventHandler(this.StoneBtn_Click);
            // 
            // ElevationLabel
            // 
            this.ElevationLabel.AutoSize = true;
            this.ElevationLabel.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ElevationLabel.ForeColor = System.Drawing.Color.White;
            this.ElevationLabel.Location = new System.Drawing.Point(67, 1);
            this.ElevationLabel.Name = "ElevationLabel";
            this.ElevationLabel.Size = new System.Drawing.Size(62, 14);
            this.ElevationLabel.TabIndex = 239;
            this.ElevationLabel.Text = "Elevation :";
            this.ElevationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Elevation2Label
            // 
            this.Elevation2Label.AutoSize = true;
            this.Elevation2Label.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Elevation2Label.ForeColor = System.Drawing.Color.White;
            this.Elevation2Label.Location = new System.Drawing.Point(126, 17);
            this.Elevation2Label.Name = "Elevation2Label";
            this.Elevation2Label.Size = new System.Drawing.Size(13, 14);
            this.Elevation2Label.TabIndex = 241;
            this.Elevation2Label.Text = "2";
            this.Elevation2Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Elevation1Label
            // 
            this.Elevation1Label.AutoSize = true;
            this.Elevation1Label.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Elevation1Label.ForeColor = System.Drawing.Color.White;
            this.Elevation1Label.Location = new System.Drawing.Point(126, 32);
            this.Elevation1Label.Name = "Elevation1Label";
            this.Elevation1Label.Size = new System.Drawing.Size(13, 14);
            this.Elevation1Label.TabIndex = 240;
            this.Elevation1Label.Text = "1";
            this.Elevation1Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ElevationBar
            // 
            this.ElevationBar.LargeChange = 1;
            this.ElevationBar.Location = new System.Drawing.Point(99, 10);
            this.ElevationBar.Maximum = 2;
            this.ElevationBar.Name = "ElevationBar";
            this.ElevationBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.ElevationBar.Size = new System.Drawing.Size(45, 57);
            this.ElevationBar.TabIndex = 1;
            this.ElevationBar.Value = 1;
            this.ElevationBar.ValueChanged += new System.EventHandler(this.ElevationBar_ValueChanged);
            // 
            // CliffBtn
            // 
            this.CliffBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.CliffBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CliffBtn.FlatAppearance.BorderSize = 0;
            this.CliffBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CliffBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CliffBtn.ForeColor = System.Drawing.Color.White;
            this.CliffBtn.Image = global::ACNHPokerCore.Properties.Resources.cliff;
            this.CliffBtn.Location = new System.Drawing.Point(3, 2);
            this.CliffBtn.Name = "CliffBtn";
            this.CliffBtn.Size = new System.Drawing.Size(60, 60);
            this.CliffBtn.TabIndex = 224;
            this.CliffBtn.Tag = "Other";
            this.CliffBtn.UseVisualStyleBackColor = false;
            this.CliffBtn.Click += new System.EventHandler(this.CliffBtn_Click);
            // 
            // CornerBtn
            // 
            this.CornerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.CornerBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CornerBtn.FlatAppearance.BorderSize = 0;
            this.CornerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CornerBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CornerBtn.ForeColor = System.Drawing.Color.White;
            this.CornerBtn.Image = global::ACNHPokerCore.Properties.Resources.corner;
            this.CornerBtn.Location = new System.Drawing.Point(163, 2);
            this.CornerBtn.Name = "CornerBtn";
            this.CornerBtn.Size = new System.Drawing.Size(60, 60);
            this.CornerBtn.TabIndex = 235;
            this.CornerBtn.Tag = "Other";
            this.CornerBtn.UseVisualStyleBackColor = false;
            this.CornerBtn.Click += new System.EventHandler(this.CornerBtn_Click);
            // 
            // RiverBtn
            // 
            this.RiverBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.RiverBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.RiverBtn.FlatAppearance.BorderSize = 0;
            this.RiverBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RiverBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RiverBtn.ForeColor = System.Drawing.Color.White;
            this.RiverBtn.Image = global::ACNHPokerCore.Properties.Resources.river;
            this.RiverBtn.Location = new System.Drawing.Point(3, 68);
            this.RiverBtn.Name = "RiverBtn";
            this.RiverBtn.Size = new System.Drawing.Size(60, 60);
            this.RiverBtn.TabIndex = 225;
            this.RiverBtn.Tag = "Other";
            this.RiverBtn.UseVisualStyleBackColor = false;
            this.RiverBtn.Click += new System.EventHandler(this.RiverBtn_Click);
            // 
            // DirtBtn
            // 
            this.DirtBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.DirtBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DirtBtn.FlatAppearance.BorderSize = 0;
            this.DirtBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DirtBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DirtBtn.ForeColor = System.Drawing.Color.White;
            this.DirtBtn.Image = global::ACNHPokerCore.Properties.Resources.dirt;
            this.DirtBtn.Location = new System.Drawing.Point(3, 144);
            this.DirtBtn.Name = "DirtBtn";
            this.DirtBtn.Size = new System.Drawing.Size(60, 60);
            this.DirtBtn.TabIndex = 226;
            this.DirtBtn.Tag = "Other";
            this.DirtBtn.UseVisualStyleBackColor = false;
            this.DirtBtn.Click += new System.EventHandler(this.DirtBtn_Click);
            // 
            // WoodBtn
            // 
            this.WoodBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.WoodBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.WoodBtn.FlatAppearance.BorderSize = 0;
            this.WoodBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.WoodBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.WoodBtn.ForeColor = System.Drawing.Color.White;
            this.WoodBtn.Image = global::ACNHPokerCore.Properties.Resources.wood;
            this.WoodBtn.Location = new System.Drawing.Point(69, 210);
            this.WoodBtn.Name = "WoodBtn";
            this.WoodBtn.Size = new System.Drawing.Size(60, 60);
            this.WoodBtn.TabIndex = 233;
            this.WoodBtn.Tag = "Other";
            this.WoodBtn.UseVisualStyleBackColor = false;
            this.WoodBtn.Click += new System.EventHandler(this.WoodBtn_Click);
            // 
            // TileBtn
            // 
            this.TileBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.TileBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.TileBtn.FlatAppearance.BorderSize = 0;
            this.TileBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TileBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.TileBtn.ForeColor = System.Drawing.Color.White;
            this.TileBtn.Image = global::ACNHPokerCore.Properties.Resources.tile;
            this.TileBtn.Location = new System.Drawing.Point(3, 210);
            this.TileBtn.Name = "TileBtn";
            this.TileBtn.Size = new System.Drawing.Size(60, 60);
            this.TileBtn.TabIndex = 232;
            this.TileBtn.Tag = "Other";
            this.TileBtn.UseVisualStyleBackColor = false;
            this.TileBtn.Click += new System.EventHandler(this.TileBtn_Click);
            // 
            // BrickBtn
            // 
            this.BrickBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.BrickBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BrickBtn.FlatAppearance.BorderSize = 0;
            this.BrickBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BrickBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.BrickBtn.ForeColor = System.Drawing.Color.White;
            this.BrickBtn.Image = global::ACNHPokerCore.Properties.Resources.brick;
            this.BrickBtn.Location = new System.Drawing.Point(135, 210);
            this.BrickBtn.Name = "BrickBtn";
            this.BrickBtn.Size = new System.Drawing.Size(60, 60);
            this.BrickBtn.TabIndex = 228;
            this.BrickBtn.Tag = "Other";
            this.BrickBtn.UseVisualStyleBackColor = false;
            this.BrickBtn.Click += new System.EventHandler(this.BrickBtn_Click);
            // 
            // SandBtn
            // 
            this.SandBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.SandBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.SandBtn.FlatAppearance.BorderSize = 0;
            this.SandBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SandBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SandBtn.ForeColor = System.Drawing.Color.White;
            this.SandBtn.Image = global::ACNHPokerCore.Properties.Resources.sand;
            this.SandBtn.Location = new System.Drawing.Point(135, 144);
            this.SandBtn.Name = "SandBtn";
            this.SandBtn.Size = new System.Drawing.Size(60, 60);
            this.SandBtn.TabIndex = 231;
            this.SandBtn.Tag = "Other";
            this.SandBtn.UseVisualStyleBackColor = false;
            this.SandBtn.Click += new System.EventHandler(this.SandBtn_Click);
            // 
            // DarkDirtBtn
            // 
            this.DarkDirtBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.DarkDirtBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DarkDirtBtn.FlatAppearance.BorderSize = 0;
            this.DarkDirtBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DarkDirtBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DarkDirtBtn.ForeColor = System.Drawing.Color.White;
            this.DarkDirtBtn.Image = global::ACNHPokerCore.Properties.Resources.darksoil;
            this.DarkDirtBtn.Location = new System.Drawing.Point(69, 144);
            this.DarkDirtBtn.Name = "DarkDirtBtn";
            this.DarkDirtBtn.Size = new System.Drawing.Size(60, 60);
            this.DarkDirtBtn.TabIndex = 229;
            this.DarkDirtBtn.Tag = "Other";
            this.DarkDirtBtn.UseVisualStyleBackColor = false;
            this.DarkDirtBtn.Click += new System.EventHandler(this.DarkDirtBtn_Click);
            // 
            // CornerPanel
            // 
            this.CornerPanel.Controls.Add(this.PleaseWaitPanel);
            this.CornerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CornerPanel.Location = new System.Drawing.Point(3, 3);
            this.CornerPanel.Name = "CornerPanel";
            this.CornerPanel.Size = new System.Drawing.Size(244, 44);
            this.CornerPanel.TabIndex = 3;
            // 
            // PleaseWaitPanel
            // 
            this.PleaseWaitPanel.Controls.Add(this.MapProgressBar);
            this.PleaseWaitPanel.Controls.Add(this.NowLoading);
            this.PleaseWaitPanel.Controls.Add(this.PleaseWaitLabel);
            this.PleaseWaitPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PleaseWaitPanel.Location = new System.Drawing.Point(0, 0);
            this.PleaseWaitPanel.Name = "PleaseWaitPanel";
            this.PleaseWaitPanel.Size = new System.Drawing.Size(244, 44);
            this.PleaseWaitPanel.TabIndex = 220;
            this.PleaseWaitPanel.Visible = false;
            // 
            // MapProgressBar
            // 
            this.MapProgressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.MapProgressBar.ForeColor = System.Drawing.Color.LawnGreen;
            this.MapProgressBar.Location = new System.Drawing.Point(12, 31);
            this.MapProgressBar.Maximum = 260;
            this.MapProgressBar.Name = "MapProgressBar";
            this.MapProgressBar.Size = new System.Drawing.Size(217, 3);
            this.MapProgressBar.TabIndex = 217;
            // 
            // NowLoading
            // 
            this.NowLoading.Image = global::ACNHPokerCore.Properties.Resources.loading;
            this.NowLoading.Location = new System.Drawing.Point(55, 3);
            this.NowLoading.Name = "NowLoading";
            this.NowLoading.Size = new System.Drawing.Size(24, 24);
            this.NowLoading.TabIndex = 216;
            this.NowLoading.TabStop = false;
            // 
            // PleaseWaitLabel
            // 
            this.PleaseWaitLabel.AutoSize = true;
            this.PleaseWaitLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.PleaseWaitLabel.ForeColor = System.Drawing.Color.White;
            this.PleaseWaitLabel.Location = new System.Drawing.Point(81, 8);
            this.PleaseWaitLabel.Name = "PleaseWaitLabel";
            this.PleaseWaitLabel.Size = new System.Drawing.Size(99, 16);
            this.PleaseWaitLabel.TabIndex = 215;
            this.PleaseWaitLabel.Text = "Please Wait...";
            this.PleaseWaitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProgressTimer
            // 
            this.ProgressTimer.Interval = 1000;
            this.ProgressTimer.Tick += new System.EventHandler(this.ProgressTimer_Tick);
            // 
            // MapToolTip
            // 
            this.MapToolTip.AutomaticDelay = 1000;
            // 
            // RoadRoller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.MainDivider);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.Name = "RoadRoller";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Road Roller";
            this.Resize += new System.EventHandler(this.RoadRoller_Resize);
            this.MainDivider.ResumeLayout(false);
            this.SubDivider.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainMap)).EndInit();
            this.TopMenuPanel.ResumeLayout(false);
            this.TopMenuPanel.PerformLayout();
            this.LeftMenuPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).EndInit();
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ElevationBar)).EndInit();
            this.CornerPanel.ResumeLayout(false);
            this.PleaseWaitPanel.ResumeLayout(false);
            this.PleaseWaitPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainDivider;
        private System.Windows.Forms.TableLayoutPanel SubDivider;
        private System.Windows.Forms.Panel TopMenuPanel;
        private System.Windows.Forms.Panel LeftMenuPanel;
        private System.Windows.Forms.Panel MiddlePanel;
        private System.Windows.Forms.Panel PleaseWaitPanel;
        private System.Windows.Forms.PictureBox NowLoading;
        private System.Windows.Forms.Label PleaseWaitLabel;
        private System.Windows.Forms.Timer ProgressTimer;
        private System.Windows.Forms.PictureBox MainMap;
        private System.Windows.Forms.ToolTip MapToolTip;
        private System.Windows.Forms.Button CliffBtn;
        private System.Windows.Forms.Button CornerBtn;
        private System.Windows.Forms.Button WoodBtn;
        private System.Windows.Forms.Button TileBtn;
        private System.Windows.Forms.Button SandBtn;
        private System.Windows.Forms.Button ArchBtn;
        private System.Windows.Forms.Button DarkDirtBtn;
        private System.Windows.Forms.Button BrickBtn;
        private System.Windows.Forms.Button StoneBtn;
        private System.Windows.Forms.Button DirtBtn;
        private System.Windows.Forms.Button RiverBtn;
        private System.Windows.Forms.Panel CornerPanel;
        private System.Windows.Forms.PictureBox miniMapBox;
        private System.Windows.Forms.Panel ButtonPanel;
        private System.Windows.Forms.Label DisplayInfoLabel;
        private JCS.ToggleSwitch DisplayInfoToggle;
        private System.Windows.Forms.Label DisplayRoadLabel;
        private JCS.ToggleSwitch DisplayRoadToggle;
        private System.Windows.Forms.Label ElevationLabel;
        private System.Windows.Forms.Label Elevation2Label;
        private System.Windows.Forms.Label Elevation1Label;
        private System.Windows.Forms.TrackBar ElevationBar;
        private System.Windows.Forms.Label HighlightCornerLabel;
        private JCS.ToggleSwitch HighlightCornerToggle;
        private System.Windows.Forms.Label DisplayBuildingLabel;
        private JCS.ToggleSwitch DisplayBuildingToggle;
        private System.Windows.Forms.Label Elevation0Label;
        private System.Windows.Forms.Button ConfirmBtn;
        private System.Windows.Forms.ProgressBar MapProgressBar;
    }
}