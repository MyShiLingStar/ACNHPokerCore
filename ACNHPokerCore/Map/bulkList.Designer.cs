namespace ACNHPokerCore
{
    partial class BulkList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BulkList));
            ItemGridView = new System.Windows.Forms.DataGridView();
            DeleteBtn = new System.Windows.Forms.Button();
            SaveBtn = new System.Windows.Forms.Button();
            ClearBtn = new System.Windows.Forms.Button();
            WrapSelector = new System.Windows.Forms.ComboBox();
            RetainNameLabel = new System.Windows.Forms.Label();
            RetainNameToggle = new JCS.ToggleSwitch();
            WrappingLabel = new System.Windows.Forms.Label();
            LoadBtn = new System.Windows.Forms.Button();
            ShuffleBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)ItemGridView).BeginInit();
            SuspendLayout();
            // 
            // ItemGridView
            // 
            ItemGridView.AllowUserToAddRows = false;
            ItemGridView.AllowUserToDeleteRows = false;
            ItemGridView.AllowUserToResizeRows = false;
            ItemGridView.BackgroundColor = System.Drawing.Color.FromArgb(47, 49, 54);
            ItemGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            ItemGridView.Location = new System.Drawing.Point(12, 12);
            ItemGridView.MultiSelect = false;
            ItemGridView.Name = "ItemGridView";
            ItemGridView.ReadOnly = true;
            ItemGridView.RowHeadersVisible = false;
            ItemGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            ItemGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            ItemGridView.ShowCellToolTips = false;
            ItemGridView.ShowEditingIcon = false;
            ItemGridView.Size = new System.Drawing.Size(395, 382);
            ItemGridView.TabIndex = 148;
            ItemGridView.CellFormatting += ItemGridView_CellFormatting;
            ItemGridView.CellMouseClick += ItemGridView_CellMouseClick;
            ItemGridView.CellMouseDown += ItemGridView_CellMouseDown;
            ItemGridView.CellMouseEnter += ItemGridView_CellMouseEnter;
            ItemGridView.CellMouseLeave += ItemGridView_CellMouseLeave;
            ItemGridView.CellMouseUp += ItemGridView_CellMouseUp;
            ItemGridView.MouseUp += ItemGridView_MouseUp;
            // 
            // DeleteBtn
            // 
            DeleteBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            DeleteBtn.FlatAppearance.BorderSize = 0;
            DeleteBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            DeleteBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            DeleteBtn.ForeColor = System.Drawing.Color.White;
            DeleteBtn.Location = new System.Drawing.Point(425, 12);
            DeleteBtn.Margin = new System.Windows.Forms.Padding(0);
            DeleteBtn.Name = "DeleteBtn";
            DeleteBtn.Size = new System.Drawing.Size(50, 50);
            DeleteBtn.TabIndex = 149;
            DeleteBtn.Text = "Del Item";
            DeleteBtn.UseVisualStyleBackColor = false;
            DeleteBtn.Click += DeleteBtn_Click;
            // 
            // SaveBtn
            // 
            SaveBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            SaveBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            SaveBtn.FlatAppearance.BorderSize = 0;
            SaveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            SaveBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            SaveBtn.ForeColor = System.Drawing.Color.White;
            SaveBtn.Location = new System.Drawing.Point(425, 399);
            SaveBtn.Margin = new System.Windows.Forms.Padding(0);
            SaveBtn.Name = "SaveBtn";
            SaveBtn.Size = new System.Drawing.Size(50, 50);
            SaveBtn.TabIndex = 150;
            SaveBtn.Text = "Save";
            SaveBtn.UseVisualStyleBackColor = false;
            SaveBtn.Click += SaveBtn_Click;
            // 
            // ClearBtn
            // 
            ClearBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            ClearBtn.FlatAppearance.BorderSize = 0;
            ClearBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ClearBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            ClearBtn.ForeColor = System.Drawing.Color.White;
            ClearBtn.Location = new System.Drawing.Point(425, 102);
            ClearBtn.Margin = new System.Windows.Forms.Padding(0);
            ClearBtn.Name = "ClearBtn";
            ClearBtn.Size = new System.Drawing.Size(50, 50);
            ClearBtn.TabIndex = 151;
            ClearBtn.Text = "Clear All";
            ClearBtn.UseVisualStyleBackColor = false;
            ClearBtn.Click += ClearBtn_Click;
            // 
            // WrapSelector
            // 
            WrapSelector.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            WrapSelector.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            WrapSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            WrapSelector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            WrapSelector.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            WrapSelector.ForeColor = System.Drawing.Color.White;
            WrapSelector.FormattingEnabled = true;
            WrapSelector.Items.AddRange(new object[] { "None : 00", "Random : XX", "Yellow : 01", "Pink : 05", "Orange : 09", "Chartreuse : 0D", "Green : 11", "Mint : 15", "Light-blue : 19", "Purple : 1D", "Navy : 21", "Blue : 25", "White : 29", "Red : 2D", "Gold : 31", "Brown : 35", "Gray : 39", "Black : 3D", "Present : 02", "Box : 03", "Festive : 3F" });
            WrapSelector.Location = new System.Drawing.Point(307, 415);
            WrapSelector.Name = "WrapSelector";
            WrapSelector.Size = new System.Drawing.Size(100, 23);
            WrapSelector.TabIndex = 152;
            // 
            // RetainNameLabel
            // 
            RetainNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            RetainNameLabel.AutoSize = true;
            RetainNameLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            RetainNameLabel.ForeColor = System.Drawing.Color.White;
            RetainNameLabel.Location = new System.Drawing.Point(298, 441);
            RetainNameLabel.Name = "RetainNameLabel";
            RetainNameLabel.Size = new System.Drawing.Size(79, 15);
            RetainNameLabel.TabIndex = 155;
            RetainNameLabel.Text = "Retain Name";
            // 
            // RetainNameToggle
            // 
            RetainNameToggle.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            RetainNameToggle.Location = new System.Drawing.Point(376, 441);
            RetainNameToggle.Name = "RetainNameToggle";
            RetainNameToggle.OffFont = new System.Drawing.Font("Segoe UI", 9F);
            RetainNameToggle.OnFont = new System.Drawing.Font("Segoe UI", 9F);
            RetainNameToggle.Size = new System.Drawing.Size(35, 16);
            RetainNameToggle.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Carbon;
            RetainNameToggle.TabIndex = 154;
            RetainNameToggle.UseAnimation = false;
            // 
            // WrappingLabel
            // 
            WrappingLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            WrappingLabel.AutoSize = true;
            WrappingLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            WrappingLabel.ForeColor = System.Drawing.Color.White;
            WrappingLabel.Location = new System.Drawing.Point(324, 399);
            WrappingLabel.Name = "WrappingLabel";
            WrappingLabel.Size = new System.Drawing.Size(62, 15);
            WrappingLabel.TabIndex = 153;
            WrappingLabel.Text = "Wrapping";
            // 
            // LoadBtn
            // 
            LoadBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            LoadBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            LoadBtn.FlatAppearance.BorderSize = 0;
            LoadBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            LoadBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            LoadBtn.ForeColor = System.Drawing.Color.White;
            LoadBtn.Location = new System.Drawing.Point(425, 344);
            LoadBtn.Margin = new System.Windows.Forms.Padding(0);
            LoadBtn.Name = "LoadBtn";
            LoadBtn.Size = new System.Drawing.Size(50, 50);
            LoadBtn.TabIndex = 156;
            LoadBtn.Text = "Load";
            LoadBtn.UseVisualStyleBackColor = false;
            LoadBtn.Click += LoadBtn_Click;
            // 
            // ShuffleBtn
            // 
            ShuffleBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            ShuffleBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            ShuffleBtn.FlatAppearance.BorderSize = 0;
            ShuffleBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ShuffleBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            ShuffleBtn.ForeColor = System.Drawing.Color.White;
            ShuffleBtn.Location = new System.Drawing.Point(233, 399);
            ShuffleBtn.Margin = new System.Windows.Forms.Padding(0);
            ShuffleBtn.Name = "ShuffleBtn";
            ShuffleBtn.Size = new System.Drawing.Size(60, 50);
            ShuffleBtn.TabIndex = 157;
            ShuffleBtn.Text = "Shuffle";
            ShuffleBtn.UseVisualStyleBackColor = false;
            ShuffleBtn.Click += ShuffleBtn_Click;
            // 
            // BulkList
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            ClientSize = new System.Drawing.Size(484, 461);
            Controls.Add(ShuffleBtn);
            Controls.Add(LoadBtn);
            Controls.Add(RetainNameToggle);
            Controls.Add(RetainNameLabel);
            Controls.Add(WrapSelector);
            Controls.Add(WrappingLabel);
            Controls.Add(ClearBtn);
            Controls.Add(SaveBtn);
            Controls.Add(DeleteBtn);
            Controls.Add(ItemGridView);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximumSize = new System.Drawing.Size(500, 50000);
            MinimumSize = new System.Drawing.Size(500, 500);
            Name = "BulkList";
            Opacity = 0.95D;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Bulk Selector";
            FormClosing += BulkList_FormClosing;
            Resize += BulkList_Resize;
            ((System.ComponentModel.ISupportInitialize)ItemGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView ItemGridView;
        private System.Windows.Forms.Button DeleteBtn;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.Button ClearBtn;
        private System.Windows.Forms.ComboBox WrapSelector;
        private System.Windows.Forms.Label RetainNameLabel;
        private JCS.ToggleSwitch RetainNameToggle;
        private System.Windows.Forms.Label WrappingLabel;
        private System.Windows.Forms.Button LoadBtn;
        private System.Windows.Forms.Button ShuffleBtn;
    }
}