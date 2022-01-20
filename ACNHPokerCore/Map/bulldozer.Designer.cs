
namespace ACNHPokerCore
{
    partial class Bulldozer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Bulldozer));
            this.miniMapBox = new System.Windows.Forms.PictureBox();
            this.realYLabel = new System.Windows.Forms.Label();
            this.realXLabel = new System.Windows.Forms.Label();
            this.RealYCoordinate = new System.Windows.Forms.RichTextBox();
            this.RealXCoordinate = new System.Windows.Forms.RichTextBox();
            this.FieldYLabel = new System.Windows.Forms.Label();
            this.FieldXLabel = new System.Windows.Forms.Label();
            this.yCoordinate = new System.Windows.Forms.RichTextBox();
            this.xCoordinate = new System.Windows.Forms.RichTextBox();
            this.acreList = new System.Windows.Forms.ListView();
            this.sendBtn = new System.Windows.Forms.Button();
            this.selectedAcreBox = new System.Windows.Forms.RichTextBox();
            this.selectedAcreValueBox = new System.Windows.Forms.RichTextBox();
            this.replaceBtn = new System.Windows.Forms.Button();
            this.AcreBtn = new System.Windows.Forms.Button();
            this.BuildingBtn = new System.Windows.Forms.Button();
            this.acrePanel = new System.Windows.Forms.Panel();
            this.saveAcreBtn = new System.Windows.Forms.Button();
            this.loadAcreBtn = new System.Windows.Forms.Button();
            this.allFlatBtn = new System.Windows.Forms.Button();
            this.TerrainBtn = new System.Windows.Forms.Button();
            this.buildingPanel = new System.Windows.Forms.Panel();
            this.BuildingControl = new System.Windows.Forms.Panel();
            this.inclinePanel = new System.Windows.Forms.Panel();
            this.inclineTypeSelect = new System.Windows.Forms.ComboBox();
            this.inclineAngleSelect = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.saveBuildingBtn = new System.Windows.Forms.Button();
            this.loadBuildingBtn = new System.Windows.Forms.Button();
            this.PreviewBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.updateBtn = new System.Windows.Forms.Button();
            this.buildingConfirmBtn = new System.Windows.Forms.Button();
            this.BuildingType = new System.Windows.Forms.ComboBox();
            this.LargeXLabel = new System.Windows.Forms.Label();
            this.XUpDown = new System.Windows.Forms.NumericUpDown();
            this.TUpDown = new System.Windows.Forms.NumericUpDown();
            this.LargeYLabel = new System.Windows.Forms.Label();
            this.typeLabel = new System.Windows.Forms.Label();
            this.YUpDown = new System.Windows.Forms.NumericUpDown();
            this.AUpDown = new System.Windows.Forms.NumericUpDown();
            this.angleLabel = new System.Windows.Forms.Label();
            this.buildingGridView = new System.Windows.Forms.DataGridView();
            this.LoadingPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.NowLoading = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).BeginInit();
            this.acrePanel.SuspendLayout();
            this.buildingPanel.SuspendLayout();
            this.BuildingControl.SuspendLayout();
            this.inclinePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.XUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.YUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildingGridView)).BeginInit();
            this.LoadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // miniMapBox
            // 
            this.miniMapBox.BackColor = System.Drawing.Color.Transparent;
            this.miniMapBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.miniMapBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.miniMapBox.ErrorImage = null;
            this.miniMapBox.InitialImage = null;
            this.miniMapBox.Location = new System.Drawing.Point(0, 0);
            this.miniMapBox.Margin = new System.Windows.Forms.Padding(0);
            this.miniMapBox.MaximumSize = new System.Drawing.Size(576, 512);
            this.miniMapBox.MinimumSize = new System.Drawing.Size(576, 512);
            this.miniMapBox.Name = "miniMapBox";
            this.miniMapBox.Size = new System.Drawing.Size(576, 512);
            this.miniMapBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.miniMapBox.TabIndex = 190;
            this.miniMapBox.TabStop = false;
            this.miniMapBox.Visible = false;
            this.miniMapBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.miniMapBox_MouseDown);
            this.miniMapBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.miniMapBox_MouseMove);
            // 
            // realYLabel
            // 
            this.realYLabel.AutoSize = true;
            this.realYLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.realYLabel.ForeColor = System.Drawing.Color.White;
            this.realYLabel.Location = new System.Drawing.Point(656, 519);
            this.realYLabel.Name = "realYLabel";
            this.realYLabel.Size = new System.Drawing.Size(25, 16);
            this.realYLabel.TabIndex = 226;
            this.realYLabel.Text = "Y :";
            // 
            // realXLabel
            // 
            this.realXLabel.AutoSize = true;
            this.realXLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.realXLabel.ForeColor = System.Drawing.Color.White;
            this.realXLabel.Location = new System.Drawing.Point(581, 519);
            this.realXLabel.Name = "realXLabel";
            this.realXLabel.Size = new System.Drawing.Size(24, 16);
            this.realXLabel.TabIndex = 225;
            this.realXLabel.Text = "X :";
            // 
            // RealYCoordinate
            // 
            this.RealYCoordinate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.RealYCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RealYCoordinate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RealYCoordinate.ForeColor = System.Drawing.Color.White;
            this.RealYCoordinate.Location = new System.Drawing.Point(682, 518);
            this.RealYCoordinate.MaxLength = 3;
            this.RealYCoordinate.Multiline = false;
            this.RealYCoordinate.Name = "RealYCoordinate";
            this.RealYCoordinate.ReadOnly = true;
            this.RealYCoordinate.Size = new System.Drawing.Size(40, 18);
            this.RealYCoordinate.TabIndex = 224;
            this.RealYCoordinate.Text = "0";
            // 
            // RealXCoordinate
            // 
            this.RealXCoordinate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.RealXCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RealXCoordinate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RealXCoordinate.ForeColor = System.Drawing.Color.White;
            this.RealXCoordinate.Location = new System.Drawing.Point(606, 518);
            this.RealXCoordinate.MaxLength = 3;
            this.RealXCoordinate.Multiline = false;
            this.RealXCoordinate.Name = "RealXCoordinate";
            this.RealXCoordinate.ReadOnly = true;
            this.RealXCoordinate.Size = new System.Drawing.Size(40, 18);
            this.RealXCoordinate.TabIndex = 223;
            this.RealXCoordinate.Text = "0";
            // 
            // FieldYLabel
            // 
            this.FieldYLabel.AutoSize = true;
            this.FieldYLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.FieldYLabel.ForeColor = System.Drawing.Color.White;
            this.FieldYLabel.Location = new System.Drawing.Point(656, 553);
            this.FieldYLabel.Name = "FieldYLabel";
            this.FieldYLabel.Size = new System.Drawing.Size(25, 16);
            this.FieldYLabel.TabIndex = 230;
            this.FieldYLabel.Text = "Y :";
            // 
            // FieldXLabel
            // 
            this.FieldXLabel.AutoSize = true;
            this.FieldXLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.FieldXLabel.ForeColor = System.Drawing.Color.White;
            this.FieldXLabel.Location = new System.Drawing.Point(581, 553);
            this.FieldXLabel.Name = "FieldXLabel";
            this.FieldXLabel.Size = new System.Drawing.Size(24, 16);
            this.FieldXLabel.TabIndex = 229;
            this.FieldXLabel.Text = "X :";
            // 
            // yCoordinate
            // 
            this.yCoordinate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.yCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.yCoordinate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yCoordinate.ForeColor = System.Drawing.Color.White;
            this.yCoordinate.Location = new System.Drawing.Point(682, 552);
            this.yCoordinate.MaxLength = 3;
            this.yCoordinate.Multiline = false;
            this.yCoordinate.Name = "yCoordinate";
            this.yCoordinate.ReadOnly = true;
            this.yCoordinate.Size = new System.Drawing.Size(40, 18);
            this.yCoordinate.TabIndex = 228;
            this.yCoordinate.Text = "0";
            // 
            // xCoordinate
            // 
            this.xCoordinate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.xCoordinate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.xCoordinate.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xCoordinate.ForeColor = System.Drawing.Color.White;
            this.xCoordinate.Location = new System.Drawing.Point(606, 552);
            this.xCoordinate.MaxLength = 3;
            this.xCoordinate.Multiline = false;
            this.xCoordinate.Name = "xCoordinate";
            this.xCoordinate.ReadOnly = true;
            this.xCoordinate.Size = new System.Drawing.Size(40, 18);
            this.xCoordinate.TabIndex = 227;
            this.xCoordinate.Text = "0";
            // 
            // acreList
            // 
            this.acreList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.acreList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.acreList.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.acreList.ForeColor = System.Drawing.Color.White;
            this.acreList.GridLines = true;
            this.acreList.HideSelection = false;
            this.acreList.Location = new System.Drawing.Point(0, 0);
            this.acreList.MultiSelect = false;
            this.acreList.Name = "acreList";
            this.acreList.Size = new System.Drawing.Size(505, 393);
            this.acreList.TabIndex = 231;
            this.acreList.TileSize = new System.Drawing.Size(1, 1);
            this.acreList.UseCompatibleStateImageBehavior = false;
            this.acreList.SelectedIndexChanged += new System.EventHandler(this.acreList_SelectedIndexChanged);
            // 
            // sendBtn
            // 
            this.sendBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.sendBtn.FlatAppearance.BorderSize = 0;
            this.sendBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendBtn.ForeColor = System.Drawing.Color.White;
            this.sendBtn.Location = new System.Drawing.Point(416, 435);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(86, 30);
            this.sendBtn.TabIndex = 232;
            this.sendBtn.Text = "Confirm";
            this.sendBtn.UseVisualStyleBackColor = false;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // selectedAcreBox
            // 
            this.selectedAcreBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.selectedAcreBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.selectedAcreBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedAcreBox.ForeColor = System.Drawing.Color.White;
            this.selectedAcreBox.Location = new System.Drawing.Point(741, 519);
            this.selectedAcreBox.MaxLength = 3;
            this.selectedAcreBox.Multiline = false;
            this.selectedAcreBox.Name = "selectedAcreBox";
            this.selectedAcreBox.ReadOnly = true;
            this.selectedAcreBox.Size = new System.Drawing.Size(40, 18);
            this.selectedAcreBox.TabIndex = 233;
            this.selectedAcreBox.Text = "0";
            // 
            // selectedAcreValueBox
            // 
            this.selectedAcreValueBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.selectedAcreValueBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.selectedAcreValueBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedAcreValueBox.ForeColor = System.Drawing.Color.White;
            this.selectedAcreValueBox.Location = new System.Drawing.Point(805, 519);
            this.selectedAcreValueBox.MaxLength = 3;
            this.selectedAcreValueBox.Multiline = false;
            this.selectedAcreValueBox.Name = "selectedAcreValueBox";
            this.selectedAcreValueBox.ReadOnly = true;
            this.selectedAcreValueBox.Size = new System.Drawing.Size(40, 18);
            this.selectedAcreValueBox.TabIndex = 234;
            this.selectedAcreValueBox.Text = "0";
            // 
            // replaceBtn
            // 
            this.replaceBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.replaceBtn.FlatAppearance.BorderSize = 0;
            this.replaceBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.replaceBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.replaceBtn.ForeColor = System.Drawing.Color.White;
            this.replaceBtn.Location = new System.Drawing.Point(7, 435);
            this.replaceBtn.Name = "replaceBtn";
            this.replaceBtn.Size = new System.Drawing.Size(86, 30);
            this.replaceBtn.TabIndex = 235;
            this.replaceBtn.Text = "◀ Replace";
            this.replaceBtn.UseVisualStyleBackColor = false;
            this.replaceBtn.Click += new System.EventHandler(this.replaceBtn_Click);
            // 
            // AcreBtn
            // 
            this.AcreBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            this.AcreBtn.FlatAppearance.BorderSize = 0;
            this.AcreBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AcreBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AcreBtn.ForeColor = System.Drawing.Color.White;
            this.AcreBtn.Location = new System.Drawing.Point(585, 6);
            this.AcreBtn.Name = "AcreBtn";
            this.AcreBtn.Size = new System.Drawing.Size(86, 30);
            this.AcreBtn.TabIndex = 236;
            this.AcreBtn.Text = "Acre";
            this.AcreBtn.UseVisualStyleBackColor = false;
            this.AcreBtn.Visible = false;
            this.AcreBtn.Click += new System.EventHandler(this.AcreBtn_Click);
            // 
            // BuildingBtn
            // 
            this.BuildingBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.BuildingBtn.FlatAppearance.BorderSize = 0;
            this.BuildingBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BuildingBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BuildingBtn.ForeColor = System.Drawing.Color.White;
            this.BuildingBtn.Location = new System.Drawing.Point(677, 6);
            this.BuildingBtn.Name = "BuildingBtn";
            this.BuildingBtn.Size = new System.Drawing.Size(86, 30);
            this.BuildingBtn.TabIndex = 237;
            this.BuildingBtn.Text = "Building";
            this.BuildingBtn.UseVisualStyleBackColor = false;
            this.BuildingBtn.Visible = false;
            this.BuildingBtn.Click += new System.EventHandler(this.BuildingBtn_Click);
            // 
            // acrePanel
            // 
            this.acrePanel.Controls.Add(this.saveAcreBtn);
            this.acrePanel.Controls.Add(this.loadAcreBtn);
            this.acrePanel.Controls.Add(this.allFlatBtn);
            this.acrePanel.Controls.Add(this.acreList);
            this.acrePanel.Controls.Add(this.replaceBtn);
            this.acrePanel.Controls.Add(this.sendBtn);
            this.acrePanel.Location = new System.Drawing.Point(578, 42);
            this.acrePanel.Name = "acrePanel";
            this.acrePanel.Size = new System.Drawing.Size(513, 470);
            this.acrePanel.TabIndex = 238;
            this.acrePanel.Visible = false;
            // 
            // saveAcreBtn
            // 
            this.saveAcreBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.saveAcreBtn.FlatAppearance.BorderSize = 0;
            this.saveAcreBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveAcreBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveAcreBtn.ForeColor = System.Drawing.Color.White;
            this.saveAcreBtn.Location = new System.Drawing.Point(324, 399);
            this.saveAcreBtn.Name = "saveAcreBtn";
            this.saveAcreBtn.Size = new System.Drawing.Size(86, 30);
            this.saveAcreBtn.TabIndex = 238;
            this.saveAcreBtn.Text = "Save";
            this.saveAcreBtn.UseVisualStyleBackColor = false;
            this.saveAcreBtn.Click += new System.EventHandler(this.saveAcreBtn_Click);
            // 
            // loadAcreBtn
            // 
            this.loadAcreBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.loadAcreBtn.FlatAppearance.BorderSize = 0;
            this.loadAcreBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadAcreBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadAcreBtn.ForeColor = System.Drawing.Color.White;
            this.loadAcreBtn.Location = new System.Drawing.Point(416, 399);
            this.loadAcreBtn.Name = "loadAcreBtn";
            this.loadAcreBtn.Size = new System.Drawing.Size(86, 30);
            this.loadAcreBtn.TabIndex = 237;
            this.loadAcreBtn.Text = "Load";
            this.loadAcreBtn.UseVisualStyleBackColor = false;
            this.loadAcreBtn.Click += new System.EventHandler(this.loadAcreBtn_Click);
            // 
            // allFlatBtn
            // 
            this.allFlatBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.allFlatBtn.FlatAppearance.BorderSize = 0;
            this.allFlatBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.allFlatBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.allFlatBtn.ForeColor = System.Drawing.Color.White;
            this.allFlatBtn.Location = new System.Drawing.Point(99, 435);
            this.allFlatBtn.Name = "allFlatBtn";
            this.allFlatBtn.Size = new System.Drawing.Size(125, 30);
            this.allFlatBtn.TabIndex = 236;
            this.allFlatBtn.Text = "◀ All Flat Acres";
            this.allFlatBtn.UseVisualStyleBackColor = false;
            this.allFlatBtn.Click += new System.EventHandler(this.allFlatBtn_Click);
            // 
            // TerrainBtn
            // 
            this.TerrainBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.TerrainBtn.FlatAppearance.BorderSize = 0;
            this.TerrainBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TerrainBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TerrainBtn.ForeColor = System.Drawing.Color.White;
            this.TerrainBtn.Location = new System.Drawing.Point(769, 6);
            this.TerrainBtn.Name = "TerrainBtn";
            this.TerrainBtn.Size = new System.Drawing.Size(86, 30);
            this.TerrainBtn.TabIndex = 239;
            this.TerrainBtn.Text = "Terrain";
            this.TerrainBtn.UseVisualStyleBackColor = false;
            this.TerrainBtn.Visible = false;
            // 
            // buildingPanel
            // 
            this.buildingPanel.Controls.Add(this.BuildingControl);
            this.buildingPanel.Controls.Add(this.buildingGridView);
            this.buildingPanel.Location = new System.Drawing.Point(578, 42);
            this.buildingPanel.Name = "buildingPanel";
            this.buildingPanel.Size = new System.Drawing.Size(513, 470);
            this.buildingPanel.TabIndex = 240;
            this.buildingPanel.Visible = false;
            // 
            // BuildingControl
            // 
            this.BuildingControl.Controls.Add(this.inclinePanel);
            this.BuildingControl.Controls.Add(this.saveBuildingBtn);
            this.BuildingControl.Controls.Add(this.loadBuildingBtn);
            this.BuildingControl.Controls.Add(this.PreviewBtn);
            this.BuildingControl.Controls.Add(this.label1);
            this.BuildingControl.Controls.Add(this.updateBtn);
            this.BuildingControl.Controls.Add(this.buildingConfirmBtn);
            this.BuildingControl.Controls.Add(this.BuildingType);
            this.BuildingControl.Controls.Add(this.LargeXLabel);
            this.BuildingControl.Controls.Add(this.XUpDown);
            this.BuildingControl.Controls.Add(this.TUpDown);
            this.BuildingControl.Controls.Add(this.LargeYLabel);
            this.BuildingControl.Controls.Add(this.typeLabel);
            this.BuildingControl.Controls.Add(this.YUpDown);
            this.BuildingControl.Controls.Add(this.AUpDown);
            this.BuildingControl.Controls.Add(this.angleLabel);
            this.BuildingControl.Location = new System.Drawing.Point(0, 333);
            this.BuildingControl.Name = "BuildingControl";
            this.BuildingControl.Size = new System.Drawing.Size(513, 137);
            this.BuildingControl.TabIndex = 95;
            this.BuildingControl.Visible = false;
            // 
            // inclinePanel
            // 
            this.inclinePanel.Controls.Add(this.inclineTypeSelect);
            this.inclinePanel.Controls.Add(this.inclineAngleSelect);
            this.inclinePanel.Controls.Add(this.label4);
            this.inclinePanel.Controls.Add(this.label3);
            this.inclinePanel.Location = new System.Drawing.Point(209, 29);
            this.inclinePanel.Name = "inclinePanel";
            this.inclinePanel.Size = new System.Drawing.Size(300, 70);
            this.inclinePanel.TabIndex = 242;
            this.inclinePanel.Visible = false;
            // 
            // inclineTypeSelect
            // 
            this.inclineTypeSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.inclineTypeSelect.DropDownHeight = 200;
            this.inclineTypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inclineTypeSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.inclineTypeSelect.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.inclineTypeSelect.ForeColor = System.Drawing.Color.White;
            this.inclineTypeSelect.FormattingEnabled = true;
            this.inclineTypeSelect.IntegralHeight = false;
            this.inclineTypeSelect.ItemHeight = 16;
            this.inclineTypeSelect.Items.AddRange(new object[] {
            "Stone : 0x0",
            "Red Steel : 0x1",
            "White-Plank : 0x2",
            "Log : 0x3",
            "Brick : 0x4",
            "Natural : 0x1D",
            "Blue-Plank : 0x1E",
            "Blue Steel : 0x1F"});
            this.inclineTypeSelect.Location = new System.Drawing.Point(162, 30);
            this.inclineTypeSelect.MaxDropDownItems = 10;
            this.inclineTypeSelect.Name = "inclineTypeSelect";
            this.inclineTypeSelect.Size = new System.Drawing.Size(135, 24);
            this.inclineTypeSelect.TabIndex = 244;
            this.inclineTypeSelect.SelectedIndexChanged += new System.EventHandler(this.inclineTypeSelect_SelectedIndexChanged);
            // 
            // inclineAngleSelect
            // 
            this.inclineAngleSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.inclineAngleSelect.DropDownHeight = 200;
            this.inclineAngleSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inclineAngleSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.inclineAngleSelect.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.inclineAngleSelect.ForeColor = System.Drawing.Color.White;
            this.inclineAngleSelect.FormattingEnabled = true;
            this.inclineAngleSelect.IntegralHeight = false;
            this.inclineAngleSelect.ItemHeight = 16;
            this.inclineAngleSelect.Items.AddRange(new object[] {
            "West → East : 0x0",
            "South ↑ North : 0x1",
            "East ← West : 0x2",
            "North ↓ South : 0x3"});
            this.inclineAngleSelect.Location = new System.Drawing.Point(4, 30);
            this.inclineAngleSelect.MaxDropDownItems = 10;
            this.inclineAngleSelect.Name = "inclineAngleSelect";
            this.inclineAngleSelect.Size = new System.Drawing.Size(143, 24);
            this.inclineAngleSelect.TabIndex = 243;
            this.inclineAngleSelect.SelectedIndexChanged += new System.EventHandler(this.inclineAngleSelect_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(158, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 22);
            this.label4.TabIndex = 242;
            this.label4.Text = "Type :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(0, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 22);
            this.label3.TabIndex = 242;
            this.label3.Text = "Angle :";
            // 
            // saveBuildingBtn
            // 
            this.saveBuildingBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.saveBuildingBtn.FlatAppearance.BorderSize = 0;
            this.saveBuildingBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBuildingBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveBuildingBtn.ForeColor = System.Drawing.Color.White;
            this.saveBuildingBtn.Location = new System.Drawing.Point(232, 101);
            this.saveBuildingBtn.Name = "saveBuildingBtn";
            this.saveBuildingBtn.Size = new System.Drawing.Size(86, 30);
            this.saveBuildingBtn.TabIndex = 241;
            this.saveBuildingBtn.Text = "Save";
            this.saveBuildingBtn.UseVisualStyleBackColor = false;
            this.saveBuildingBtn.Click += new System.EventHandler(this.saveBuildingBtn_Click);
            // 
            // loadBuildingBtn
            // 
            this.loadBuildingBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.loadBuildingBtn.FlatAppearance.BorderSize = 0;
            this.loadBuildingBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loadBuildingBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadBuildingBtn.ForeColor = System.Drawing.Color.White;
            this.loadBuildingBtn.Location = new System.Drawing.Point(324, 101);
            this.loadBuildingBtn.Name = "loadBuildingBtn";
            this.loadBuildingBtn.Size = new System.Drawing.Size(86, 30);
            this.loadBuildingBtn.TabIndex = 240;
            this.loadBuildingBtn.Text = "Load";
            this.loadBuildingBtn.UseVisualStyleBackColor = false;
            this.loadBuildingBtn.Click += new System.EventHandler(this.loadBuildingBtn_Click);
            // 
            // PreviewBtn
            // 
            this.PreviewBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.PreviewBtn.FlatAppearance.BorderSize = 0;
            this.PreviewBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreviewBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PreviewBtn.ForeColor = System.Drawing.Color.White;
            this.PreviewBtn.Location = new System.Drawing.Point(99, 101);
            this.PreviewBtn.Name = "PreviewBtn";
            this.PreviewBtn.Size = new System.Drawing.Size(86, 30);
            this.PreviewBtn.TabIndex = 239;
            this.PreviewBtn.Text = "Preview";
            this.PreviewBtn.UseVisualStyleBackColor = false;
            this.PreviewBtn.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 22);
            this.label1.TabIndex = 39;
            this.label1.Text = "Building Type :";
            // 
            // updateBtn
            // 
            this.updateBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.updateBtn.FlatAppearance.BorderSize = 0;
            this.updateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updateBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateBtn.ForeColor = System.Drawing.Color.White;
            this.updateBtn.Location = new System.Drawing.Point(7, 101);
            this.updateBtn.Name = "updateBtn";
            this.updateBtn.Size = new System.Drawing.Size(86, 30);
            this.updateBtn.TabIndex = 238;
            this.updateBtn.Text = "◀ Update";
            this.updateBtn.UseVisualStyleBackColor = false;
            this.updateBtn.Click += new System.EventHandler(this.updateBtn_Click);
            // 
            // buildingConfirmBtn
            // 
            this.buildingConfirmBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.buildingConfirmBtn.FlatAppearance.BorderSize = 0;
            this.buildingConfirmBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buildingConfirmBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buildingConfirmBtn.ForeColor = System.Drawing.Color.White;
            this.buildingConfirmBtn.Location = new System.Drawing.Point(416, 101);
            this.buildingConfirmBtn.Name = "buildingConfirmBtn";
            this.buildingConfirmBtn.Size = new System.Drawing.Size(86, 30);
            this.buildingConfirmBtn.TabIndex = 237;
            this.buildingConfirmBtn.Text = "Confirm";
            this.buildingConfirmBtn.UseVisualStyleBackColor = false;
            this.buildingConfirmBtn.Click += new System.EventHandler(this.buildingConfirmBtn_Click);
            // 
            // BuildingType
            // 
            this.BuildingType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.BuildingType.DropDownHeight = 200;
            this.BuildingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BuildingType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BuildingType.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.BuildingType.ForeColor = System.Drawing.Color.White;
            this.BuildingType.FormattingEnabled = true;
            this.BuildingType.IntegralHeight = false;
            this.BuildingType.ItemHeight = 16;
            this.BuildingType.Items.AddRange(new object[] {
            "Empty : 0x0",
            "Player House 1 : 0x1",
            "Player House 2 : 0x2",
            "Player House 3 : 0x3",
            "Player House 4 : 0x4",
            "Player House 5 : 0x5",
            "Player House 6 : 0x6",
            "Player House 7 : 0x7",
            "Player House 8 : 0x8",
            "Villager House 1 : 0x9",
            "Villager House 2 : 0xA",
            "Villager House 3 : 0xB",
            "Villager House 4 : 0xC",
            "Villager House 5 : 0xD",
            "Villager House 6 : 0xE",
            "Villager House 7 : 0xF",
            "Villager House 8 : 0x10",
            "Villager House 9 : 0x11",
            "Villager House 10 : 0x12",
            "Nook\'s Cranny : 0x13",
            "Resident Services (Building) : 0x14",
            "Museum : 0x15",
            "Airport : 0x16",
            "Resident Services (Tent) : 0x17",
            "Able Sisters : 0x18",
            "Campsite : 0x19",
            "Bridge : 0x1A",
            "Incline : 0x1B",
            "Redd\'s Treasure Trawler : 0x1C",
            "Studio : 0x1D"});
            this.BuildingType.Location = new System.Drawing.Point(7, 34);
            this.BuildingType.MaxDropDownItems = 10;
            this.BuildingType.Name = "BuildingType";
            this.BuildingType.Size = new System.Drawing.Size(196, 24);
            this.BuildingType.TabIndex = 94;
            this.BuildingType.SelectedIndexChanged += new System.EventHandler(this.BuildingType_SelectedIndexChanged);
            // 
            // LargeXLabel
            // 
            this.LargeXLabel.AutoSize = true;
            this.LargeXLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.LargeXLabel.ForeColor = System.Drawing.Color.White;
            this.LargeXLabel.Location = new System.Drawing.Point(249, 4);
            this.LargeXLabel.Name = "LargeXLabel";
            this.LargeXLabel.Size = new System.Drawing.Size(35, 22);
            this.LargeXLabel.TabIndex = 32;
            this.LargeXLabel.Text = "X :";
            // 
            // XUpDown
            // 
            this.XUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.XUpDown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.XUpDown.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.XUpDown.ForeColor = System.Drawing.Color.White;
            this.XUpDown.Location = new System.Drawing.Point(291, 3);
            this.XUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.XUpDown.Name = "XUpDown";
            this.XUpDown.Size = new System.Drawing.Size(65, 25);
            this.XUpDown.TabIndex = 31;
            this.XUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.XUpDown.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.XUpDown.ValueChanged += new System.EventHandler(this.XUpDown_ValueChanged);
            // 
            // TUpDown
            // 
            this.TUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.TUpDown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TUpDown.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.TUpDown.ForeColor = System.Drawing.Color.White;
            this.TUpDown.Location = new System.Drawing.Point(441, 33);
            this.TUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.TUpDown.Name = "TUpDown";
            this.TUpDown.Size = new System.Drawing.Size(65, 25);
            this.TUpDown.TabIndex = 37;
            this.TUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TUpDown.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // LargeYLabel
            // 
            this.LargeYLabel.AutoSize = true;
            this.LargeYLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.LargeYLabel.ForeColor = System.Drawing.Color.White;
            this.LargeYLabel.Location = new System.Drawing.Point(400, 4);
            this.LargeYLabel.Name = "LargeYLabel";
            this.LargeYLabel.Size = new System.Drawing.Size(35, 22);
            this.LargeYLabel.TabIndex = 34;
            this.LargeYLabel.Text = "Y :";
            // 
            // typeLabel
            // 
            this.typeLabel.AutoSize = true;
            this.typeLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.typeLabel.ForeColor = System.Drawing.Color.White;
            this.typeLabel.Location = new System.Drawing.Point(367, 34);
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size(68, 22);
            this.typeLabel.TabIndex = 38;
            this.typeLabel.Text = "Type :";
            // 
            // YUpDown
            // 
            this.YUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.YUpDown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.YUpDown.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.YUpDown.ForeColor = System.Drawing.Color.White;
            this.YUpDown.Location = new System.Drawing.Point(441, 3);
            this.YUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.YUpDown.Name = "YUpDown";
            this.YUpDown.Size = new System.Drawing.Size(65, 25);
            this.YUpDown.TabIndex = 33;
            this.YUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.YUpDown.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.YUpDown.ValueChanged += new System.EventHandler(this.YUpDown_ValueChanged);
            // 
            // AUpDown
            // 
            this.AUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.AUpDown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AUpDown.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.AUpDown.ForeColor = System.Drawing.Color.White;
            this.AUpDown.Location = new System.Drawing.Point(291, 33);
            this.AUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.AUpDown.Name = "AUpDown";
            this.AUpDown.Size = new System.Drawing.Size(65, 25);
            this.AUpDown.TabIndex = 35;
            this.AUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.AUpDown.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // angleLabel
            // 
            this.angleLabel.AutoSize = true;
            this.angleLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.angleLabel.ForeColor = System.Drawing.Color.White;
            this.angleLabel.Location = new System.Drawing.Point(209, 34);
            this.angleLabel.Name = "angleLabel";
            this.angleLabel.Size = new System.Drawing.Size(75, 22);
            this.angleLabel.TabIndex = 36;
            this.angleLabel.Text = "Angle :";
            // 
            // buildingGridView
            // 
            this.buildingGridView.AllowUserToAddRows = false;
            this.buildingGridView.AllowUserToDeleteRows = false;
            this.buildingGridView.AllowUserToResizeRows = false;
            this.buildingGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(49)))), ((int)(((byte)(54)))));
            this.buildingGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft JhengHei", 10F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.buildingGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.buildingGridView.Location = new System.Drawing.Point(0, 0);
            this.buildingGridView.MultiSelect = false;
            this.buildingGridView.Name = "buildingGridView";
            this.buildingGridView.ReadOnly = true;
            this.buildingGridView.RowHeadersVisible = false;
            this.buildingGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.buildingGridView.Size = new System.Drawing.Size(513, 332);
            this.buildingGridView.TabIndex = 30;
            this.buildingGridView.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.buildingGridView_CellMouseUp);
            // 
            // LoadingPanel
            // 
            this.LoadingPanel.Controls.Add(this.label2);
            this.LoadingPanel.Controls.Add(this.NowLoading);
            this.LoadingPanel.Location = new System.Drawing.Point(470, 241);
            this.LoadingPanel.Name = "LoadingPanel";
            this.LoadingPanel.Size = new System.Drawing.Size(185, 30);
            this.LoadingPanel.TabIndex = 241;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(29, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 22);
            this.label2.TabIndex = 242;
            this.label2.Text = "Now Loading...";
            // 
            // NowLoading
            // 
            this.NowLoading.Image = global::ACNHPokerCore.Properties.Resources.loading;
            this.NowLoading.Location = new System.Drawing.Point(2, 3);
            this.NowLoading.Name = "NowLoading";
            this.NowLoading.Size = new System.Drawing.Size(24, 24);
            this.NowLoading.TabIndex = 217;
            this.NowLoading.TabStop = false;
            // 
            // Bulldozer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(1094, 511);
            this.Controls.Add(this.LoadingPanel);
            this.Controls.Add(this.TerrainBtn);
            this.Controls.Add(this.BuildingBtn);
            this.Controls.Add(this.AcreBtn);
            this.Controls.Add(this.selectedAcreValueBox);
            this.Controls.Add(this.selectedAcreBox);
            this.Controls.Add(this.FieldYLabel);
            this.Controls.Add(this.FieldXLabel);
            this.Controls.Add(this.yCoordinate);
            this.Controls.Add(this.xCoordinate);
            this.Controls.Add(this.realYLabel);
            this.Controls.Add(this.realXLabel);
            this.Controls.Add(this.RealYCoordinate);
            this.Controls.Add(this.RealXCoordinate);
            this.Controls.Add(this.miniMapBox);
            this.Controls.Add(this.buildingPanel);
            this.Controls.Add(this.acrePanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1110, 550);
            this.Name = "Bulldozer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bulldozer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Bulldozer_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.miniMapBox)).EndInit();
            this.acrePanel.ResumeLayout(false);
            this.buildingPanel.ResumeLayout(false);
            this.BuildingControl.ResumeLayout(false);
            this.BuildingControl.PerformLayout();
            this.inclinePanel.ResumeLayout(false);
            this.inclinePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.XUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.YUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buildingGridView)).EndInit();
            this.LoadingPanel.ResumeLayout(false);
            this.LoadingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NowLoading)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox miniMapBox;
        private System.Windows.Forms.Label realYLabel;
        private System.Windows.Forms.Label realXLabel;
        private System.Windows.Forms.RichTextBox RealYCoordinate;
        private System.Windows.Forms.RichTextBox RealXCoordinate;
        private System.Windows.Forms.Label FieldYLabel;
        private System.Windows.Forms.Label FieldXLabel;
        private System.Windows.Forms.RichTextBox yCoordinate;
        private System.Windows.Forms.RichTextBox xCoordinate;
        private System.Windows.Forms.ListView acreList;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.RichTextBox selectedAcreBox;
        private System.Windows.Forms.RichTextBox selectedAcreValueBox;
        private System.Windows.Forms.Button replaceBtn;
        private System.Windows.Forms.Button AcreBtn;
        private System.Windows.Forms.Button BuildingBtn;
        private System.Windows.Forms.Panel acrePanel;
        private System.Windows.Forms.Button allFlatBtn;
        private System.Windows.Forms.Button TerrainBtn;
        private System.Windows.Forms.Panel buildingPanel;
        private System.Windows.Forms.DataGridView buildingGridView;
        private System.Windows.Forms.NumericUpDown XUpDown;
        private System.Windows.Forms.Label LargeXLabel;
        private System.Windows.Forms.NumericUpDown YUpDown;
        private System.Windows.Forms.Label LargeYLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown TUpDown;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.NumericUpDown AUpDown;
        private System.Windows.Forms.Label angleLabel;
        private System.Windows.Forms.ComboBox BuildingType;
        private System.Windows.Forms.Panel BuildingControl;
        private System.Windows.Forms.Button PreviewBtn;
        private System.Windows.Forms.Button updateBtn;
        private System.Windows.Forms.Button buildingConfirmBtn;
        private System.Windows.Forms.Button saveAcreBtn;
        private System.Windows.Forms.Button loadAcreBtn;
        private System.Windows.Forms.Button saveBuildingBtn;
        private System.Windows.Forms.Button loadBuildingBtn;
        private System.Windows.Forms.Panel LoadingPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox NowLoading;
        private System.Windows.Forms.Panel inclinePanel;
        private System.Windows.Forms.ComboBox inclineTypeSelect;
        private System.Windows.Forms.ComboBox inclineAngleSelect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}