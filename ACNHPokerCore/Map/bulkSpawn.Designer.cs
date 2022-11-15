
namespace ACNHPokerCore
{
    partial class BulkSpawn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BulkSpawn));
            this.LeftBtn = new System.Windows.Forms.RadioButton();
            this.RightBtn = new System.Windows.Forms.RadioButton();
            this.miniMapBox = new System.Windows.Forms.PictureBox();
            this.selectBtn = new System.Windows.Forms.Button();
            this.SpawnBtn = new System.Windows.Forms.Button();
            this.numOfItemBox = new System.Windows.Forms.RichTextBox();
            this.numOfItemLabel = new System.Windows.Forms.Label();
            this.VertHeightImageBox = new System.Windows.Forms.PictureBox();
            this.VertHeightLabel = new System.Windows.Forms.Label();
            this.VertDirectionImageBox = new System.Windows.Forms.PictureBox();
            this.VertDirectionLabel = new System.Windows.Forms.Label();
            this.XLabel = new System.Windows.Forms.Label();
            this.YLabel = new System.Windows.Forms.Label();
            this.VertSettingPanel = new System.Windows.Forms.Panel();
            this.VertHeightNumber = new System.Windows.Forms.NumericUpDown();
            this.VertWidthNumber = new System.Windows.Forms.RichTextBox();
            this.VertWidthLabel = new System.Windows.Forms.Label();
            this.IgnoreSpaceLabel = new System.Windows.Forms.Label();
            this.IgnoreSpaceToggle = new JCS.ToggleSwitch();
            this.WarningMessage = new System.Windows.Forms.RichTextBox();
            this.PleaseWaitPanel = new System.Windows.Forms.Panel();
            this.WaitMessagebox = new System.Windows.Forms.RichTextBox();
            this.MapProgressBar = new System.Windows.Forms.ProgressBar();
            this.NowLoading = new System.Windows.Forms.PictureBox();
            this.PleaseWaitLabel = new System.Windows.Forms.Label();
            this.ProgressTimer = new System.Windows.Forms.Timer(this.components);
            this.numOfSpaceLabel = new System.Windows.Forms.Label();
            this.numOfSpaceBox = new System.Windows.Forms.RichTextBox();
            this.xCoordinate = new System.Windows.Forms.NumericUpDown();
            this.yCoordinate = new System.Windows.Forms.NumericUpDown();
            this.HoriSettingPanel = new System.Windows.Forms.Panel();
            this.HoriWidthNumber = new System.Windows.Forms.NumericUpDown();
            this.HoriHeightNumber = new System.Windows.Forms.RichTextBox();
            this.HoriHeightLabel = new System.Windows.Forms.Label();
            this.HoriWidthImageBox = new System.Windows.Forms.PictureBox();
            this.HoriWidthLabel = new System.Windows.Forms.Label();
            this.VertThenHoriz = new System.Windows.Forms.RadioButton();
            this.HorizThenVert = new System.Windows.Forms.RadioButton();
            this.FlagTextbox = new ACNHPokerCore.HexUpDown();
            this.FlagLabel = new System.Windows.Forms.Label();
            this.OtherSettingPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VertHeightImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VertDirectionImageBox)).BeginInit();
            this.VertSettingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VertHeightNumber)).BeginInit();
            this.PleaseWaitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xCoordinate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yCoordinate)).BeginInit();
            this.HoriSettingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HoriWidthNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HoriWidthImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FlagTextbox)).BeginInit();
            this.OtherSettingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // LeftBtn
            // 
            this.LeftBtn.AutoSize = true;
            this.LeftBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LeftBtn.ForeColor = System.Drawing.Color.White;
            this.LeftBtn.Location = new System.Drawing.Point(136, 58);
            this.LeftBtn.Margin = new System.Windows.Forms.Padding(4);
            this.LeftBtn.Name = "LeftBtn";
            this.LeftBtn.Size = new System.Drawing.Size(49, 20);
            this.LeftBtn.TabIndex = 76;
            this.LeftBtn.Text = "Left";
            this.LeftBtn.UseVisualStyleBackColor = true;
            this.LeftBtn.CheckedChanged += new System.EventHandler(this.LeftBtn_CheckedChanged);
            // 
            // RightBtn
            // 
            this.RightBtn.AutoSize = true;
            this.RightBtn.Checked = true;
            this.RightBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RightBtn.ForeColor = System.Drawing.Color.White;
            this.RightBtn.Location = new System.Drawing.Point(136, 40);
            this.RightBtn.Margin = new System.Windows.Forms.Padding(4);
            this.RightBtn.Name = "RightBtn";
            this.RightBtn.Size = new System.Drawing.Size(58, 20);
            this.RightBtn.TabIndex = 77;
            this.RightBtn.TabStop = true;
            this.RightBtn.Text = "Right";
            this.RightBtn.UseVisualStyleBackColor = true;
            this.RightBtn.CheckedChanged += new System.EventHandler(this.RightBtn_CheckedChanged);
            // 
            // miniMapBox
            // 
            this.miniMapBox.Location = new System.Drawing.Point(12, 12);
            this.miniMapBox.Name = "miniMapBox";
            this.miniMapBox.Size = new System.Drawing.Size(448, 384);
            this.miniMapBox.TabIndex = 78;
            this.miniMapBox.TabStop = false;
            this.miniMapBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MiniMapBox_MouseDown);
            // 
            // selectBtn
            // 
            this.selectBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.selectBtn.FlatAppearance.BorderSize = 0;
            this.selectBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.selectBtn.ForeColor = System.Drawing.Color.White;
            this.selectBtn.Location = new System.Drawing.Point(469, 12);
            this.selectBtn.Margin = new System.Windows.Forms.Padding(4);
            this.selectBtn.Name = "selectBtn";
            this.selectBtn.Size = new System.Drawing.Size(188, 26);
            this.selectBtn.TabIndex = 0;
            this.selectBtn.Text = "Select File";
            this.selectBtn.UseVisualStyleBackColor = false;
            this.selectBtn.Click += new System.EventHandler(this.SelectBtn_Click);
            // 
            // SpawnBtn
            // 
            this.SpawnBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.SpawnBtn.FlatAppearance.BorderSize = 0;
            this.SpawnBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SpawnBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SpawnBtn.ForeColor = System.Drawing.Color.White;
            this.SpawnBtn.Location = new System.Drawing.Point(5, 121);
            this.SpawnBtn.Margin = new System.Windows.Forms.Padding(4);
            this.SpawnBtn.Name = "SpawnBtn";
            this.SpawnBtn.Size = new System.Drawing.Size(188, 28);
            this.SpawnBtn.TabIndex = 222;
            this.SpawnBtn.Text = "Spawn";
            this.SpawnBtn.UseVisualStyleBackColor = false;
            this.SpawnBtn.Click += new System.EventHandler(this.SpawnBtn_Click);
            // 
            // numOfItemBox
            // 
            this.numOfItemBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.numOfItemBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numOfItemBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.numOfItemBox.ForeColor = System.Drawing.Color.Gray;
            this.numOfItemBox.Location = new System.Drawing.Point(596, 45);
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
            this.numOfItemLabel.Location = new System.Drawing.Point(465, 47);
            this.numOfItemLabel.Name = "numOfItemLabel";
            this.numOfItemLabel.Size = new System.Drawing.Size(106, 16);
            this.numOfItemLabel.TabIndex = 228;
            this.numOfItemLabel.Text = "Num of Items :";
            this.numOfItemLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VertHeightImageBox
            // 
            this.VertHeightImageBox.Image = global::ACNHPokerCore.Properties.Resources.height;
            this.VertHeightImageBox.Location = new System.Drawing.Point(4, 3);
            this.VertHeightImageBox.Name = "VertHeightImageBox";
            this.VertHeightImageBox.Size = new System.Drawing.Size(50, 50);
            this.VertHeightImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.VertHeightImageBox.TabIndex = 229;
            this.VertHeightImageBox.TabStop = false;
            // 
            // VertHeightLabel
            // 
            this.VertHeightLabel.AutoSize = true;
            this.VertHeightLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VertHeightLabel.ForeColor = System.Drawing.Color.White;
            this.VertHeightLabel.Location = new System.Drawing.Point(55, 20);
            this.VertHeightLabel.Name = "VertHeightLabel";
            this.VertHeightLabel.Size = new System.Drawing.Size(61, 16);
            this.VertHeightLabel.TabIndex = 230;
            this.VertHeightLabel.Text = "Height :";
            this.VertHeightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VertDirectionImageBox
            // 
            this.VertDirectionImageBox.Image = global::ACNHPokerCore.Properties.Resources.arrows_horizontal;
            this.VertDirectionImageBox.Location = new System.Drawing.Point(8, 35);
            this.VertDirectionImageBox.Name = "VertDirectionImageBox";
            this.VertDirectionImageBox.Size = new System.Drawing.Size(45, 45);
            this.VertDirectionImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.VertDirectionImageBox.TabIndex = 231;
            this.VertDirectionImageBox.TabStop = false;
            // 
            // VertDirectionLabel
            // 
            this.VertDirectionLabel.AutoSize = true;
            this.VertDirectionLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VertDirectionLabel.ForeColor = System.Drawing.Color.White;
            this.VertDirectionLabel.Location = new System.Drawing.Point(56, 50);
            this.VertDirectionLabel.Name = "VertDirectionLabel";
            this.VertDirectionLabel.Size = new System.Drawing.Size(79, 16);
            this.VertDirectionLabel.TabIndex = 232;
            this.VertDirectionLabel.Text = "Direction :";
            this.VertDirectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // XLabel
            // 
            this.XLabel.AutoSize = true;
            this.XLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.XLabel.ForeColor = System.Drawing.Color.White;
            this.XLabel.Location = new System.Drawing.Point(465, 96);
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
            this.YLabel.Location = new System.Drawing.Point(565, 96);
            this.YLabel.Name = "YLabel";
            this.YLabel.Size = new System.Drawing.Size(25, 16);
            this.YLabel.TabIndex = 234;
            this.YLabel.Text = "Y :";
            this.YLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VertSettingPanel
            // 
            this.VertSettingPanel.Controls.Add(this.VertHeightNumber);
            this.VertSettingPanel.Controls.Add(this.VertWidthNumber);
            this.VertSettingPanel.Controls.Add(this.VertWidthLabel);
            this.VertSettingPanel.Controls.Add(this.VertHeightImageBox);
            this.VertSettingPanel.Controls.Add(this.VertHeightLabel);
            this.VertSettingPanel.Location = new System.Drawing.Point(465, 167);
            this.VertSettingPanel.Name = "VertSettingPanel";
            this.VertSettingPanel.Size = new System.Drawing.Size(198, 74);
            this.VertSettingPanel.TabIndex = 235;
            this.VertSettingPanel.Visible = false;
            // 
            // VertHeightNumber
            // 
            this.VertHeightNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.VertHeightNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.VertHeightNumber.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VertHeightNumber.ForeColor = System.Drawing.Color.White;
            this.VertHeightNumber.Location = new System.Drawing.Point(130, 16);
            this.VertHeightNumber.Maximum = new decimal(new int[] {
            96,
            0,
            0,
            0});
            this.VertHeightNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.VertHeightNumber.Name = "VertHeightNumber";
            this.VertHeightNumber.Size = new System.Drawing.Size(61, 25);
            this.VertHeightNumber.TabIndex = 239;
            this.VertHeightNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.VertHeightNumber.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.VertHeightNumber.ValueChanged += new System.EventHandler(this.VertHeightNumber_ValueChanged);
            // 
            // VertWidthNumber
            // 
            this.VertWidthNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.VertWidthNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.VertWidthNumber.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VertWidthNumber.ForeColor = System.Drawing.Color.Gray;
            this.VertWidthNumber.Location = new System.Drawing.Point(130, 50);
            this.VertWidthNumber.Margin = new System.Windows.Forms.Padding(4);
            this.VertWidthNumber.MaxLength = 3;
            this.VertWidthNumber.Multiline = false;
            this.VertWidthNumber.Name = "VertWidthNumber";
            this.VertWidthNumber.ReadOnly = true;
            this.VertWidthNumber.Size = new System.Drawing.Size(61, 18);
            this.VertWidthNumber.TabIndex = 233;
            this.VertWidthNumber.Text = "0";
            this.VertWidthNumber.Visible = false;
            // 
            // VertWidthLabel
            // 
            this.VertWidthLabel.AutoSize = true;
            this.VertWidthLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VertWidthLabel.ForeColor = System.Drawing.Color.White;
            this.VertWidthLabel.Location = new System.Drawing.Point(61, 51);
            this.VertWidthLabel.Name = "VertWidthLabel";
            this.VertWidthLabel.Size = new System.Drawing.Size(55, 16);
            this.VertWidthLabel.TabIndex = 234;
            this.VertWidthLabel.Text = "Width :";
            this.VertWidthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.VertWidthLabel.Visible = false;
            // 
            // IgnoreSpaceLabel
            // 
            this.IgnoreSpaceLabel.AutoSize = true;
            this.IgnoreSpaceLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.IgnoreSpaceLabel.ForeColor = System.Drawing.Color.White;
            this.IgnoreSpaceLabel.Location = new System.Drawing.Point(8, 2);
            this.IgnoreSpaceLabel.Name = "IgnoreSpaceLabel";
            this.IgnoreSpaceLabel.Size = new System.Drawing.Size(108, 16);
            this.IgnoreSpaceLabel.TabIndex = 237;
            this.IgnoreSpaceLabel.Text = "Ignore Space :";
            this.IgnoreSpaceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // IgnoreSpaceToggle
            // 
            this.IgnoreSpaceToggle.Checked = true;
            this.IgnoreSpaceToggle.Location = new System.Drawing.Point(136, 0);
            this.IgnoreSpaceToggle.Name = "IgnoreSpaceToggle";
            this.IgnoreSpaceToggle.OffFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IgnoreSpaceToggle.OnFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IgnoreSpaceToggle.Size = new System.Drawing.Size(50, 20);
            this.IgnoreSpaceToggle.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Carbon;
            this.IgnoreSpaceToggle.TabIndex = 236;
            this.IgnoreSpaceToggle.UseAnimation = false;
            this.IgnoreSpaceToggle.CheckedChanged += new JCS.ToggleSwitch.CheckedChangedDelegate(this.IgnoreSpaceToggle_CheckedChanged);
            // 
            // WarningMessage
            // 
            this.WarningMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.WarningMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.WarningMessage.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.WarningMessage.ForeColor = System.Drawing.Color.Firebrick;
            this.WarningMessage.Location = new System.Drawing.Point(674, 17);
            this.WarningMessage.Multiline = false;
            this.WarningMessage.Name = "WarningMessage";
            this.WarningMessage.ReadOnly = true;
            this.WarningMessage.Size = new System.Drawing.Size(186, 18);
            this.WarningMessage.TabIndex = 235;
            this.WarningMessage.Text = "Spawn area out of bounds!";
            this.WarningMessage.Visible = false;
            // 
            // PleaseWaitPanel
            // 
            this.PleaseWaitPanel.Controls.Add(this.WaitMessagebox);
            this.PleaseWaitPanel.Controls.Add(this.MapProgressBar);
            this.PleaseWaitPanel.Controls.Add(this.NowLoading);
            this.PleaseWaitPanel.Controls.Add(this.PleaseWaitLabel);
            this.PleaseWaitPanel.Location = new System.Drawing.Point(674, 41);
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
            // numOfSpaceLabel
            // 
            this.numOfSpaceLabel.AutoSize = true;
            this.numOfSpaceLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.numOfSpaceLabel.ForeColor = System.Drawing.Color.White;
            this.numOfSpaceLabel.Location = new System.Drawing.Point(465, 69);
            this.numOfSpaceLabel.Name = "numOfSpaceLabel";
            this.numOfSpaceLabel.Size = new System.Drawing.Size(111, 16);
            this.numOfSpaceLabel.TabIndex = 237;
            this.numOfSpaceLabel.Text = "Num of Space :";
            this.numOfSpaceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numOfSpaceBox
            // 
            this.numOfSpaceBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.numOfSpaceBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numOfSpaceBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.numOfSpaceBox.ForeColor = System.Drawing.Color.Gray;
            this.numOfSpaceBox.Location = new System.Drawing.Point(596, 68);
            this.numOfSpaceBox.Margin = new System.Windows.Forms.Padding(4);
            this.numOfSpaceBox.MaxLength = 3;
            this.numOfSpaceBox.Multiline = false;
            this.numOfSpaceBox.Name = "numOfSpaceBox";
            this.numOfSpaceBox.ReadOnly = true;
            this.numOfSpaceBox.Size = new System.Drawing.Size(61, 18);
            this.numOfSpaceBox.TabIndex = 238;
            this.numOfSpaceBox.Text = "0";
            // 
            // xCoordinate
            // 
            this.xCoordinate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.xCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.xCoordinate.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.xCoordinate.ForeColor = System.Drawing.Color.White;
            this.xCoordinate.Location = new System.Drawing.Point(495, 92);
            this.xCoordinate.Maximum = new decimal(new int[] {
            111,
            0,
            0,
            0});
            this.xCoordinate.Name = "xCoordinate";
            this.xCoordinate.Size = new System.Drawing.Size(61, 25);
            this.xCoordinate.TabIndex = 240;
            this.xCoordinate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.xCoordinate.ValueChanged += new System.EventHandler(this.CoordinateChanged);
            // 
            // yCoordinate
            // 
            this.yCoordinate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.yCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.yCoordinate.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.yCoordinate.ForeColor = System.Drawing.Color.White;
            this.yCoordinate.Location = new System.Drawing.Point(596, 92);
            this.yCoordinate.Maximum = new decimal(new int[] {
            95,
            0,
            0,
            0});
            this.yCoordinate.Name = "yCoordinate";
            this.yCoordinate.Size = new System.Drawing.Size(61, 25);
            this.yCoordinate.TabIndex = 241;
            this.yCoordinate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.yCoordinate.ValueChanged += new System.EventHandler(this.CoordinateChanged);
            // 
            // HoriSettingPanel
            // 
            this.HoriSettingPanel.Controls.Add(this.HoriWidthNumber);
            this.HoriSettingPanel.Controls.Add(this.HoriHeightNumber);
            this.HoriSettingPanel.Controls.Add(this.HoriHeightLabel);
            this.HoriSettingPanel.Controls.Add(this.HoriWidthImageBox);
            this.HoriSettingPanel.Controls.Add(this.HoriWidthLabel);
            this.HoriSettingPanel.Location = new System.Drawing.Point(465, 167);
            this.HoriSettingPanel.Name = "HoriSettingPanel";
            this.HoriSettingPanel.Size = new System.Drawing.Size(198, 74);
            this.HoriSettingPanel.TabIndex = 242;
            this.HoriSettingPanel.Visible = false;
            // 
            // HoriWidthNumber
            // 
            this.HoriWidthNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.HoriWidthNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.HoriWidthNumber.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HoriWidthNumber.ForeColor = System.Drawing.Color.White;
            this.HoriWidthNumber.Location = new System.Drawing.Point(130, 16);
            this.HoriWidthNumber.Maximum = new decimal(new int[] {
            112,
            0,
            0,
            0});
            this.HoriWidthNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HoriWidthNumber.Name = "HoriWidthNumber";
            this.HoriWidthNumber.Size = new System.Drawing.Size(61, 25);
            this.HoriWidthNumber.TabIndex = 239;
            this.HoriWidthNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.HoriWidthNumber.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.HoriWidthNumber.ValueChanged += new System.EventHandler(this.HoriWidthNumber_ValueChanged);
            // 
            // HoriHeightNumber
            // 
            this.HoriHeightNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.HoriHeightNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.HoriHeightNumber.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HoriHeightNumber.ForeColor = System.Drawing.Color.Gray;
            this.HoriHeightNumber.Location = new System.Drawing.Point(130, 51);
            this.HoriHeightNumber.Margin = new System.Windows.Forms.Padding(4);
            this.HoriHeightNumber.MaxLength = 3;
            this.HoriHeightNumber.Multiline = false;
            this.HoriHeightNumber.Name = "HoriHeightNumber";
            this.HoriHeightNumber.ReadOnly = true;
            this.HoriHeightNumber.Size = new System.Drawing.Size(61, 18);
            this.HoriHeightNumber.TabIndex = 233;
            this.HoriHeightNumber.Text = "0";
            this.HoriHeightNumber.Visible = false;
            // 
            // HoriHeightLabel
            // 
            this.HoriHeightLabel.AutoSize = true;
            this.HoriHeightLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HoriHeightLabel.ForeColor = System.Drawing.Color.White;
            this.HoriHeightLabel.Location = new System.Drawing.Point(55, 52);
            this.HoriHeightLabel.Name = "HoriHeightLabel";
            this.HoriHeightLabel.Size = new System.Drawing.Size(61, 16);
            this.HoriHeightLabel.TabIndex = 234;
            this.HoriHeightLabel.Text = "Height :";
            this.HoriHeightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.HoriHeightLabel.Visible = false;
            // 
            // HoriWidthImageBox
            // 
            this.HoriWidthImageBox.Image = global::ACNHPokerCore.Properties.Resources.height;
            this.HoriWidthImageBox.Location = new System.Drawing.Point(4, 3);
            this.HoriWidthImageBox.Name = "HoriWidthImageBox";
            this.HoriWidthImageBox.Size = new System.Drawing.Size(50, 50);
            this.HoriWidthImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.HoriWidthImageBox.TabIndex = 229;
            this.HoriWidthImageBox.TabStop = false;
            // 
            // HoriWidthLabel
            // 
            this.HoriWidthLabel.AutoSize = true;
            this.HoriWidthLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HoriWidthLabel.ForeColor = System.Drawing.Color.White;
            this.HoriWidthLabel.Location = new System.Drawing.Point(61, 20);
            this.HoriWidthLabel.Name = "HoriWidthLabel";
            this.HoriWidthLabel.Size = new System.Drawing.Size(55, 16);
            this.HoriWidthLabel.TabIndex = 230;
            this.HoriWidthLabel.Text = "Width :";
            this.HoriWidthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VertThenHoriz
            // 
            this.VertThenHoriz.AutoSize = true;
            this.VertThenHoriz.Checked = true;
            this.VertThenHoriz.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.VertThenHoriz.ForeColor = System.Drawing.Color.White;
            this.VertThenHoriz.Location = new System.Drawing.Point(488, 121);
            this.VertThenHoriz.Margin = new System.Windows.Forms.Padding(4);
            this.VertThenHoriz.Name = "VertThenHoriz";
            this.VertThenHoriz.Size = new System.Drawing.Size(156, 20);
            this.VertThenHoriz.TabIndex = 244;
            this.VertThenHoriz.TabStop = true;
            this.VertThenHoriz.Text = "Vertical ➤ Horizontal";
            this.VertThenHoriz.UseVisualStyleBackColor = true;
            this.VertThenHoriz.Visible = false;
            this.VertThenHoriz.CheckedChanged += new System.EventHandler(this.VertThenHoriz_CheckedChanged);
            // 
            // HorizThenVert
            // 
            this.HorizThenVert.AutoSize = true;
            this.HorizThenVert.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HorizThenVert.ForeColor = System.Drawing.Color.White;
            this.HorizThenVert.Location = new System.Drawing.Point(488, 145);
            this.HorizThenVert.Margin = new System.Windows.Forms.Padding(4);
            this.HorizThenVert.Name = "HorizThenVert";
            this.HorizThenVert.Size = new System.Drawing.Size(156, 20);
            this.HorizThenVert.TabIndex = 243;
            this.HorizThenVert.Text = "Horizontal ➤ Vertical";
            this.HorizThenVert.UseVisualStyleBackColor = true;
            this.HorizThenVert.Visible = false;
            // 
            // FlagTextbox
            // 
            this.FlagTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.FlagTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FlagTextbox.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.FlagTextbox.ForeColor = System.Drawing.Color.White;
            this.FlagTextbox.Hexadecimal = true;
            this.FlagTextbox.HexLength = 2;
            this.FlagTextbox.Location = new System.Drawing.Point(131, 89);
            this.FlagTextbox.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.FlagTextbox.Name = "FlagTextbox";
            this.FlagTextbox.Size = new System.Drawing.Size(61, 25);
            this.FlagTextbox.TabIndex = 245;
            this.FlagTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.FlagTextbox.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.FlagTextbox.ValueChanged += new System.EventHandler(this.FlagTextbox_ValueChanged);
            // 
            // FlagLabel
            // 
            this.FlagLabel.AutoSize = true;
            this.FlagLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.FlagLabel.ForeColor = System.Drawing.Color.White;
            this.FlagLabel.Location = new System.Drawing.Point(72, 93);
            this.FlagLabel.Name = "FlagLabel";
            this.FlagLabel.Size = new System.Drawing.Size(45, 16);
            this.FlagLabel.TabIndex = 246;
            this.FlagLabel.Text = "Flag :";
            this.FlagLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OtherSettingPanel
            // 
            this.OtherSettingPanel.Controls.Add(this.IgnoreSpaceLabel);
            this.OtherSettingPanel.Controls.Add(this.SpawnBtn);
            this.OtherSettingPanel.Controls.Add(this.IgnoreSpaceToggle);
            this.OtherSettingPanel.Controls.Add(this.VertDirectionImageBox);
            this.OtherSettingPanel.Controls.Add(this.FlagLabel);
            this.OtherSettingPanel.Controls.Add(this.VertDirectionLabel);
            this.OtherSettingPanel.Controls.Add(this.RightBtn);
            this.OtherSettingPanel.Controls.Add(this.FlagTextbox);
            this.OtherSettingPanel.Controls.Add(this.LeftBtn);
            this.OtherSettingPanel.Location = new System.Drawing.Point(465, 242);
            this.OtherSettingPanel.Name = "OtherSettingPanel";
            this.OtherSettingPanel.Size = new System.Drawing.Size(198, 154);
            this.OtherSettingPanel.TabIndex = 247;
            this.OtherSettingPanel.Visible = false;
            // 
            // BulkSpawn
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(674, 411);
            this.Controls.Add(this.WarningMessage);
            this.Controls.Add(this.PleaseWaitPanel);
            this.Controls.Add(this.OtherSettingPanel);
            this.Controls.Add(this.VertThenHoriz);
            this.Controls.Add(this.HorizThenVert);
            this.Controls.Add(this.yCoordinate);
            this.Controls.Add(this.xCoordinate);
            this.Controls.Add(this.numOfSpaceBox);
            this.Controls.Add(this.numOfSpaceLabel);
            this.Controls.Add(this.VertSettingPanel);
            this.Controls.Add(this.YLabel);
            this.Controls.Add(this.XLabel);
            this.Controls.Add(this.numOfItemLabel);
            this.Controls.Add(this.numOfItemBox);
            this.Controls.Add(this.selectBtn);
            this.Controls.Add(this.miniMapBox);
            this.Controls.Add(this.HoriSettingPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(900, 450);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(690, 450);
            this.Name = "BulkSpawn";
            this.Opacity = 0.95D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bulk Spawn";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BulkSpawn_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VertHeightImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VertDirectionImageBox)).EndInit();
            this.VertSettingPanel.ResumeLayout(false);
            this.VertSettingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VertHeightNumber)).EndInit();
            this.PleaseWaitPanel.ResumeLayout(false);
            this.PleaseWaitPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xCoordinate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yCoordinate)).EndInit();
            this.HoriSettingPanel.ResumeLayout(false);
            this.HoriSettingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HoriWidthNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HoriWidthImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FlagTextbox)).EndInit();
            this.OtherSettingPanel.ResumeLayout(false);
            this.OtherSettingPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RadioButton LeftBtn;
        private System.Windows.Forms.RadioButton RightBtn;
        private System.Windows.Forms.PictureBox miniMapBox;
        private System.Windows.Forms.Button selectBtn;
        private System.Windows.Forms.Button SpawnBtn;
        private System.Windows.Forms.RichTextBox numOfItemBox;
        private System.Windows.Forms.Label numOfItemLabel;
        private System.Windows.Forms.PictureBox VertHeightImageBox;
        private System.Windows.Forms.Label VertHeightLabel;
        private System.Windows.Forms.PictureBox VertDirectionImageBox;
        private System.Windows.Forms.Label VertDirectionLabel;
        private System.Windows.Forms.Label XLabel;
        private System.Windows.Forms.Label YLabel;
        private System.Windows.Forms.Panel VertSettingPanel;
        private System.Windows.Forms.RichTextBox VertWidthNumber;
        private System.Windows.Forms.Label VertWidthLabel;
        private System.Windows.Forms.RichTextBox WarningMessage;
        private System.Windows.Forms.Panel PleaseWaitPanel;
        private System.Windows.Forms.RichTextBox WaitMessagebox;
        private System.Windows.Forms.ProgressBar MapProgressBar;
        private System.Windows.Forms.PictureBox NowLoading;
        private System.Windows.Forms.Label PleaseWaitLabel;
        private System.Windows.Forms.Timer ProgressTimer;
        private System.Windows.Forms.Label IgnoreSpaceLabel;
        private JCS.ToggleSwitch IgnoreSpaceToggle;
        private System.Windows.Forms.Label numOfSpaceLabel;
        private System.Windows.Forms.RichTextBox numOfSpaceBox;
        private System.Windows.Forms.NumericUpDown VertHeightNumber;
        private System.Windows.Forms.NumericUpDown xCoordinate;
        private System.Windows.Forms.NumericUpDown yCoordinate;
        private System.Windows.Forms.Panel HoriSettingPanel;
        private System.Windows.Forms.NumericUpDown HoriWidthNumber;
        private System.Windows.Forms.RichTextBox HoriHeightNumber;
        private System.Windows.Forms.Label HoriHeightLabel;
        private System.Windows.Forms.PictureBox HoriWidthImageBox;
        private System.Windows.Forms.Label HoriWidthLabel;
        private System.Windows.Forms.RadioButton VertThenHoriz;
        private System.Windows.Forms.RadioButton HorizThenVert;
        private HexUpDown FlagTextbox;
        private System.Windows.Forms.Label FlagLabel;
        private System.Windows.Forms.Panel OtherSettingPanel;
    }
}