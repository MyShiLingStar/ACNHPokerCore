
namespace ACNHPokerCore
{
    partial class bulkSpawn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(bulkSpawn));
            this.heightNumber = new System.Windows.Forms.RichTextBox();
            this.yCoordinate = new System.Windows.Forms.RichTextBox();
            this.xCoordinate = new System.Windows.Forms.RichTextBox();
            this.leftBtn = new System.Windows.Forms.RadioButton();
            this.rightBtn = new System.Windows.Forms.RadioButton();
            this.miniMapBox = new System.Windows.Forms.PictureBox();
            this.selectBtn = new System.Windows.Forms.Button();
            this.spawnBtn = new System.Windows.Forms.Button();
            this.previewBtn = new System.Windows.Forms.Button();
            this.numOfItemBox = new System.Windows.Forms.RichTextBox();
            this.numOfItemLabel = new System.Windows.Forms.Label();
            this.HeightImageBox = new System.Windows.Forms.PictureBox();
            this.heightLabel = new System.Windows.Forms.Label();
            this.DirectionImageBox = new System.Windows.Forms.PictureBox();
            this.directionLabel = new System.Windows.Forms.Label();
            this.XLabel = new System.Windows.Forms.Label();
            this.YLabel = new System.Windows.Forms.Label();
            this.settingPanel = new System.Windows.Forms.Panel();
            this.warningMessage = new System.Windows.Forms.RichTextBox();
            this.widthNumber = new System.Windows.Forms.RichTextBox();
            this.widthLabel = new System.Windows.Forms.Label();
            this.PleaseWaitPanel = new System.Windows.Forms.Panel();
            this.WaitMessagebox = new System.Windows.Forms.RichTextBox();
            this.MapProgressBar = new System.Windows.Forms.ProgressBar();
            this.NowLoading = new System.Windows.Forms.PictureBox();
            this.PleaseWaitLabel = new System.Windows.Forms.Label();
            this.ProgressTimer = new System.Windows.Forms.Timer(this.components);
            this.IgnoreSpaceToggle = new JCS.ToggleSwitch();
            this.ignoreSpaceLabel = new System.Windows.Forms.Label();
            this.numOfSpaceLabel = new System.Windows.Forms.Label();
            this.numOfSpaceBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DirectionImageBox)).BeginInit();
            this.settingPanel.SuspendLayout();
            this.PleaseWaitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // heightNumber
            // 
            this.heightNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.heightNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.heightNumber.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.heightNumber.ForeColor = System.Drawing.Color.White;
            this.heightNumber.Location = new System.Drawing.Point(130, 21);
            this.heightNumber.Margin = new System.Windows.Forms.Padding(4);
            this.heightNumber.MaxLength = 3;
            this.heightNumber.Multiline = false;
            this.heightNumber.Name = "heightNumber";
            this.heightNumber.Size = new System.Drawing.Size(61, 18);
            this.heightNumber.TabIndex = 73;
            this.heightNumber.Text = "32";
            this.heightNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.heightNumber_KeyPress);
            // 
            // yCoordinate
            // 
            this.yCoordinate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.yCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.yCoordinate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.yCoordinate.ForeColor = System.Drawing.Color.White;
            this.yCoordinate.Location = new System.Drawing.Point(596, 12);
            this.yCoordinate.Margin = new System.Windows.Forms.Padding(4);
            this.yCoordinate.MaxLength = 3;
            this.yCoordinate.Multiline = false;
            this.yCoordinate.Name = "yCoordinate";
            this.yCoordinate.Size = new System.Drawing.Size(61, 18);
            this.yCoordinate.TabIndex = 75;
            this.yCoordinate.Text = "0";
            this.yCoordinate.TextChanged += new System.EventHandler(this.CoordinateChanged);
            this.yCoordinate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CoordinateKeyPress);
            // 
            // xCoordinate
            // 
            this.xCoordinate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.xCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.xCoordinate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.xCoordinate.ForeColor = System.Drawing.Color.White;
            this.xCoordinate.Location = new System.Drawing.Point(495, 12);
            this.xCoordinate.Margin = new System.Windows.Forms.Padding(4);
            this.xCoordinate.MaxLength = 3;
            this.xCoordinate.Multiline = false;
            this.xCoordinate.Name = "xCoordinate";
            this.xCoordinate.Size = new System.Drawing.Size(61, 18);
            this.xCoordinate.TabIndex = 74;
            this.xCoordinate.Text = "0";
            this.xCoordinate.TextChanged += new System.EventHandler(this.CoordinateChanged);
            this.xCoordinate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CoordinateKeyPress);
            // 
            // leftBtn
            // 
            this.leftBtn.AutoSize = true;
            this.leftBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.leftBtn.ForeColor = System.Drawing.Color.White;
            this.leftBtn.Location = new System.Drawing.Point(135, 93);
            this.leftBtn.Margin = new System.Windows.Forms.Padding(4);
            this.leftBtn.Name = "leftBtn";
            this.leftBtn.Size = new System.Drawing.Size(49, 20);
            this.leftBtn.TabIndex = 76;
            this.leftBtn.Text = "Left";
            this.leftBtn.UseVisualStyleBackColor = true;
            // 
            // rightBtn
            // 
            this.rightBtn.AutoSize = true;
            this.rightBtn.Checked = true;
            this.rightBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.rightBtn.ForeColor = System.Drawing.Color.White;
            this.rightBtn.Location = new System.Drawing.Point(135, 75);
            this.rightBtn.Margin = new System.Windows.Forms.Padding(4);
            this.rightBtn.Name = "rightBtn";
            this.rightBtn.Size = new System.Drawing.Size(58, 20);
            this.rightBtn.TabIndex = 77;
            this.rightBtn.TabStop = true;
            this.rightBtn.Text = "Right";
            this.rightBtn.UseVisualStyleBackColor = true;
            // 
            // miniMapBox
            // 
            this.miniMapBox.Location = new System.Drawing.Point(12, 12);
            this.miniMapBox.Name = "miniMapBox";
            this.miniMapBox.Size = new System.Drawing.Size(448, 384);
            this.miniMapBox.TabIndex = 78;
            this.miniMapBox.TabStop = false;
            this.miniMapBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.miniMapBox_MouseDown);
            // 
            // selectBtn
            // 
            this.selectBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.selectBtn.FlatAppearance.BorderSize = 0;
            this.selectBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.selectBtn.ForeColor = System.Drawing.Color.White;
            this.selectBtn.Location = new System.Drawing.Point(471, 42);
            this.selectBtn.Margin = new System.Windows.Forms.Padding(4);
            this.selectBtn.Name = "selectBtn";
            this.selectBtn.Size = new System.Drawing.Size(186, 28);
            this.selectBtn.TabIndex = 0;
            this.selectBtn.Text = "Select File";
            this.selectBtn.UseVisualStyleBackColor = false;
            this.selectBtn.Click += new System.EventHandler(this.selectBtn_Click);
            // 
            // spawnBtn
            // 
            this.spawnBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.spawnBtn.FlatAppearance.BorderSize = 0;
            this.spawnBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spawnBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.spawnBtn.ForeColor = System.Drawing.Color.White;
            this.spawnBtn.Location = new System.Drawing.Point(6, 235);
            this.spawnBtn.Margin = new System.Windows.Forms.Padding(4);
            this.spawnBtn.Name = "spawnBtn";
            this.spawnBtn.Size = new System.Drawing.Size(186, 28);
            this.spawnBtn.TabIndex = 222;
            this.spawnBtn.Text = "Spawn";
            this.spawnBtn.UseVisualStyleBackColor = false;
            this.spawnBtn.Visible = false;
            this.spawnBtn.Click += new System.EventHandler(this.spawnBtn_Click);
            // 
            // previewBtn
            // 
            this.previewBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.previewBtn.FlatAppearance.BorderSize = 0;
            this.previewBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.previewBtn.ForeColor = System.Drawing.Color.White;
            this.previewBtn.Location = new System.Drawing.Point(6, 199);
            this.previewBtn.Margin = new System.Windows.Forms.Padding(4);
            this.previewBtn.Name = "previewBtn";
            this.previewBtn.Size = new System.Drawing.Size(186, 28);
            this.previewBtn.TabIndex = 224;
            this.previewBtn.Text = "Preview";
            this.previewBtn.UseVisualStyleBackColor = false;
            this.previewBtn.Click += new System.EventHandler(this.previewBtn_Click);
            // 
            // numOfItemBox
            // 
            this.numOfItemBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.numOfItemBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numOfItemBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.numOfItemBox.ForeColor = System.Drawing.Color.Gray;
            this.numOfItemBox.Location = new System.Drawing.Point(596, 80);
            this.numOfItemBox.Margin = new System.Windows.Forms.Padding(4);
            this.numOfItemBox.MaxLength = 3;
            this.numOfItemBox.Multiline = false;
            this.numOfItemBox.Name = "numOfItemBox";
            this.numOfItemBox.ReadOnly = true;
            this.numOfItemBox.Size = new System.Drawing.Size(61, 18);
            this.numOfItemBox.TabIndex = 227;
            this.numOfItemBox.Text = "0";
            // 
            // numOfItemLabel
            // 
            this.numOfItemLabel.AutoSize = true;
            this.numOfItemLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.numOfItemLabel.ForeColor = System.Drawing.Color.White;
            this.numOfItemLabel.Location = new System.Drawing.Point(468, 82);
            this.numOfItemLabel.Name = "numOfItemLabel";
            this.numOfItemLabel.Size = new System.Drawing.Size(106, 16);
            this.numOfItemLabel.TabIndex = 228;
            this.numOfItemLabel.Text = "Num of Items :";
            this.numOfItemLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HeightImageBox
            // 
            this.HeightImageBox.Image = global::ACNHPokerCore.Properties.Resources.height;
            this.HeightImageBox.Location = new System.Drawing.Point(4, 5);
            this.HeightImageBox.Name = "HeightImageBox";
            this.HeightImageBox.Size = new System.Drawing.Size(50, 50);
            this.HeightImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.HeightImageBox.TabIndex = 229;
            this.HeightImageBox.TabStop = false;
            // 
            // heightLabel
            // 
            this.heightLabel.AutoSize = true;
            this.heightLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.heightLabel.ForeColor = System.Drawing.Color.White;
            this.heightLabel.Location = new System.Drawing.Point(55, 22);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(61, 16);
            this.heightLabel.TabIndex = 230;
            this.heightLabel.Text = "Height :";
            this.heightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DirectionImageBox
            // 
            this.DirectionImageBox.Image = global::ACNHPokerCore.Properties.Resources.arrows_horizontal;
            this.DirectionImageBox.Location = new System.Drawing.Point(7, 70);
            this.DirectionImageBox.Name = "DirectionImageBox";
            this.DirectionImageBox.Size = new System.Drawing.Size(45, 45);
            this.DirectionImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.DirectionImageBox.TabIndex = 231;
            this.DirectionImageBox.TabStop = false;
            // 
            // directionLabel
            // 
            this.directionLabel.AutoSize = true;
            this.directionLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.directionLabel.ForeColor = System.Drawing.Color.White;
            this.directionLabel.Location = new System.Drawing.Point(55, 85);
            this.directionLabel.Name = "directionLabel";
            this.directionLabel.Size = new System.Drawing.Size(79, 16);
            this.directionLabel.TabIndex = 232;
            this.directionLabel.Text = "Direction :";
            this.directionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // XLabel
            // 
            this.XLabel.AutoSize = true;
            this.XLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.XLabel.ForeColor = System.Drawing.Color.White;
            this.XLabel.Location = new System.Drawing.Point(470, 13);
            this.XLabel.Name = "XLabel";
            this.XLabel.Size = new System.Drawing.Size(24, 16);
            this.XLabel.TabIndex = 233;
            this.XLabel.Text = "X :";
            this.XLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // YLabel
            // 
            this.YLabel.AutoSize = true;
            this.YLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.YLabel.ForeColor = System.Drawing.Color.White;
            this.YLabel.Location = new System.Drawing.Point(570, 13);
            this.YLabel.Name = "YLabel";
            this.YLabel.Size = new System.Drawing.Size(25, 16);
            this.YLabel.TabIndex = 234;
            this.YLabel.Text = "Y :";
            this.YLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // settingPanel
            // 
            this.settingPanel.Controls.Add(this.ignoreSpaceLabel);
            this.settingPanel.Controls.Add(this.IgnoreSpaceToggle);
            this.settingPanel.Controls.Add(this.warningMessage);
            this.settingPanel.Controls.Add(this.widthNumber);
            this.settingPanel.Controls.Add(this.widthLabel);
            this.settingPanel.Controls.Add(this.rightBtn);
            this.settingPanel.Controls.Add(this.heightNumber);
            this.settingPanel.Controls.Add(this.leftBtn);
            this.settingPanel.Controls.Add(this.directionLabel);
            this.settingPanel.Controls.Add(this.HeightImageBox);
            this.settingPanel.Controls.Add(this.DirectionImageBox);
            this.settingPanel.Controls.Add(this.heightLabel);
            this.settingPanel.Controls.Add(this.previewBtn);
            this.settingPanel.Controls.Add(this.spawnBtn);
            this.settingPanel.Location = new System.Drawing.Point(465, 127);
            this.settingPanel.Name = "settingPanel";
            this.settingPanel.Size = new System.Drawing.Size(198, 269);
            this.settingPanel.TabIndex = 235;
            this.settingPanel.Visible = false;
            // 
            // warningMessage
            // 
            this.warningMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.warningMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.warningMessage.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.warningMessage.ForeColor = System.Drawing.Color.Firebrick;
            this.warningMessage.Location = new System.Drawing.Point(6, 179);
            this.warningMessage.Multiline = false;
            this.warningMessage.Name = "warningMessage";
            this.warningMessage.ReadOnly = true;
            this.warningMessage.Size = new System.Drawing.Size(186, 18);
            this.warningMessage.TabIndex = 235;
            this.warningMessage.Text = "Spawn area out of bounds!";
            this.warningMessage.Visible = false;
            // 
            // widthNumber
            // 
            this.widthNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.widthNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.widthNumber.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.widthNumber.ForeColor = System.Drawing.Color.Gray;
            this.widthNumber.Location = new System.Drawing.Point(130, 144);
            this.widthNumber.Margin = new System.Windows.Forms.Padding(4);
            this.widthNumber.MaxLength = 3;
            this.widthNumber.Multiline = false;
            this.widthNumber.Name = "widthNumber";
            this.widthNumber.ReadOnly = true;
            this.widthNumber.Size = new System.Drawing.Size(61, 18);
            this.widthNumber.TabIndex = 233;
            this.widthNumber.Text = "0";
            this.widthNumber.Visible = false;
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.widthLabel.ForeColor = System.Drawing.Color.White;
            this.widthLabel.Location = new System.Drawing.Point(55, 145);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(55, 16);
            this.widthLabel.TabIndex = 234;
            this.widthLabel.Text = "Width :";
            this.widthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.widthLabel.Visible = false;
            // 
            // PleaseWaitPanel
            // 
            this.PleaseWaitPanel.Controls.Add(this.WaitMessagebox);
            this.PleaseWaitPanel.Controls.Add(this.MapProgressBar);
            this.PleaseWaitPanel.Controls.Add(this.NowLoading);
            this.PleaseWaitPanel.Controls.Add(this.PleaseWaitLabel);
            this.PleaseWaitPanel.Location = new System.Drawing.Point(465, 328);
            this.PleaseWaitPanel.Name = "PleaseWaitPanel";
            this.PleaseWaitPanel.Size = new System.Drawing.Size(198, 60);
            this.PleaseWaitPanel.TabIndex = 236;
            this.PleaseWaitPanel.Visible = false;
            // 
            // WaitMessagebox
            // 
            this.WaitMessagebox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.WaitMessagebox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.WaitMessagebox.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.WaitMessagebox.ForeColor = System.Drawing.Color.White;
            this.WaitMessagebox.Location = new System.Drawing.Point(1, 32);
            this.WaitMessagebox.Multiline = false;
            this.WaitMessagebox.Name = "WaitMessagebox";
            this.WaitMessagebox.ReadOnly = true;
            this.WaitMessagebox.Size = new System.Drawing.Size(193, 27);
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
            this.MapProgressBar.Size = new System.Drawing.Size(190, 10);
            this.MapProgressBar.TabIndex = 215;
            // 
            // NowLoading
            // 
            this.NowLoading.Image = global::ACNHPokerCore.Properties.Resources.loading;
            this.NowLoading.Location = new System.Drawing.Point(38, 2);
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
            this.PleaseWaitLabel.Location = new System.Drawing.Point(64, 7);
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
            // IgnoreSpaceToggle
            // 
            this.IgnoreSpaceToggle.Checked = true;
            this.IgnoreSpaceToggle.Location = new System.Drawing.Point(140, 121);
            this.IgnoreSpaceToggle.Name = "IgnoreSpaceToggle";
            this.IgnoreSpaceToggle.OffFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IgnoreSpaceToggle.OnFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IgnoreSpaceToggle.Size = new System.Drawing.Size(35, 16);
            this.IgnoreSpaceToggle.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Carbon;
            this.IgnoreSpaceToggle.TabIndex = 236;
            this.IgnoreSpaceToggle.UseAnimation = false;
            this.IgnoreSpaceToggle.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.IgnoreSpaceToggle_CheckedChanged);
            // 
            // ignoreSpaceLabel
            // 
            this.ignoreSpaceLabel.AutoSize = true;
            this.ignoreSpaceLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ignoreSpaceLabel.ForeColor = System.Drawing.Color.White;
            this.ignoreSpaceLabel.Location = new System.Drawing.Point(26, 121);
            this.ignoreSpaceLabel.Name = "ignoreSpaceLabel";
            this.ignoreSpaceLabel.Size = new System.Drawing.Size(108, 16);
            this.ignoreSpaceLabel.TabIndex = 237;
            this.ignoreSpaceLabel.Text = "Ignore Space :";
            this.ignoreSpaceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numOfSpaceLabel
            // 
            this.numOfSpaceLabel.AutoSize = true;
            this.numOfSpaceLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.numOfSpaceLabel.ForeColor = System.Drawing.Color.White;
            this.numOfSpaceLabel.Location = new System.Drawing.Point(468, 104);
            this.numOfSpaceLabel.Name = "numOfSpaceLabel";
            this.numOfSpaceLabel.Size = new System.Drawing.Size(111, 16);
            this.numOfSpaceLabel.TabIndex = 237;
            this.numOfSpaceLabel.Text = "Num of Space :";
            this.numOfSpaceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.numOfSpaceLabel.Visible = false;
            // 
            // numOfSpaceBox
            // 
            this.numOfSpaceBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.numOfSpaceBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numOfSpaceBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.numOfSpaceBox.ForeColor = System.Drawing.Color.Gray;
            this.numOfSpaceBox.Location = new System.Drawing.Point(596, 102);
            this.numOfSpaceBox.Margin = new System.Windows.Forms.Padding(4);
            this.numOfSpaceBox.MaxLength = 3;
            this.numOfSpaceBox.Multiline = false;
            this.numOfSpaceBox.Name = "numOfSpaceBox";
            this.numOfSpaceBox.ReadOnly = true;
            this.numOfSpaceBox.Size = new System.Drawing.Size(61, 18);
            this.numOfSpaceBox.TabIndex = 238;
            this.numOfSpaceBox.Text = "0";
            this.numOfSpaceBox.Visible = false;
            // 
            // bulkSpawn
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(674, 411);
            this.Controls.Add(this.numOfSpaceBox);
            this.Controls.Add(this.numOfSpaceLabel);
            this.Controls.Add(this.PleaseWaitPanel);
            this.Controls.Add(this.settingPanel);
            this.Controls.Add(this.YLabel);
            this.Controls.Add(this.XLabel);
            this.Controls.Add(this.numOfItemLabel);
            this.Controls.Add(this.numOfItemBox);
            this.Controls.Add(this.selectBtn);
            this.Controls.Add(this.miniMapBox);
            this.Controls.Add(this.yCoordinate);
            this.Controls.Add(this.xCoordinate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(690, 450);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(690, 450);
            this.Name = "bulkSpawn";
            this.Opacity = 0.95D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bulk Spawn";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.bulkSpawn_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DirectionImageBox)).EndInit();
            this.settingPanel.ResumeLayout(false);
            this.settingPanel.PerformLayout();
            this.PleaseWaitPanel.ResumeLayout(false);
            this.PleaseWaitPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox heightNumber;
        private System.Windows.Forms.RichTextBox yCoordinate;
        private System.Windows.Forms.RichTextBox xCoordinate;
        private System.Windows.Forms.RadioButton leftBtn;
        private System.Windows.Forms.RadioButton rightBtn;
        private System.Windows.Forms.PictureBox miniMapBox;
        private System.Windows.Forms.Button selectBtn;
        private System.Windows.Forms.Button spawnBtn;
        private System.Windows.Forms.Button previewBtn;
        private System.Windows.Forms.RichTextBox numOfItemBox;
        private System.Windows.Forms.Label numOfItemLabel;
        private System.Windows.Forms.PictureBox HeightImageBox;
        private System.Windows.Forms.Label heightLabel;
        private System.Windows.Forms.PictureBox DirectionImageBox;
        private System.Windows.Forms.Label directionLabel;
        private System.Windows.Forms.Label XLabel;
        private System.Windows.Forms.Label YLabel;
        private System.Windows.Forms.Panel settingPanel;
        private System.Windows.Forms.RichTextBox widthNumber;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.RichTextBox warningMessage;
        private System.Windows.Forms.Panel PleaseWaitPanel;
        private System.Windows.Forms.RichTextBox WaitMessagebox;
        private System.Windows.Forms.ProgressBar MapProgressBar;
        private System.Windows.Forms.PictureBox NowLoading;
        private System.Windows.Forms.Label PleaseWaitLabel;
        private System.Windows.Forms.Timer ProgressTimer;
        private System.Windows.Forms.Label ignoreSpaceLabel;
        private JCS.ToggleSwitch IgnoreSpaceToggle;
        private System.Windows.Forms.Label numOfSpaceLabel;
        private System.Windows.Forms.RichTextBox numOfSpaceBox;
    }
}