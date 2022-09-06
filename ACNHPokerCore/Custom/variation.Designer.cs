namespace ACNHPokerCore
{
    partial class variation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(variation));
            this.furnitureGridView = new System.Windows.Forms.DataGridView();
            this.selectedItem = new ACNHPokerCore.inventorySlot();
            this.itemIDLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.furnitureGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // furnitureGridView
            // 
            this.furnitureGridView.AllowUserToAddRows = false;
            this.furnitureGridView.AllowUserToDeleteRows = false;
            this.furnitureGridView.AllowUserToResizeRows = false;
            this.furnitureGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(49)))), ((int)(((byte)(54)))));
            this.furnitureGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.furnitureGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.furnitureGridView.Location = new System.Drawing.Point(692, 0);
            this.furnitureGridView.MultiSelect = false;
            this.furnitureGridView.Name = "furnitureGridView";
            this.furnitureGridView.ReadOnly = true;
            this.furnitureGridView.RowHeadersVisible = false;
            this.furnitureGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.furnitureGridView.Size = new System.Drawing.Size(514, 133);
            this.furnitureGridView.TabIndex = 30;
            this.furnitureGridView.Visible = false;
            this.furnitureGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.FurnitureGridView_CellClick);
            this.furnitureGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.FurnitureGridView_CellFormatting);
            // 
            // selectedItem
            // 
            this.selectedItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.selectedItem.FlatAppearance.BorderSize = 0;
            this.selectedItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectedItem.flowerQuantity = ((ushort)(0));
            this.selectedItem.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.selectedItem.ForeColor = System.Drawing.Color.White;
            this.selectedItem.itemDurability = ((ushort)(0));
            this.selectedItem.itemQuantity = ((ushort)(0));
            this.selectedItem.Location = new System.Drawing.Point(692, 145);
            this.selectedItem.Margin = new System.Windows.Forms.Padding(0);
            this.selectedItem.Name = "selectedItem";
            this.selectedItem.Size = new System.Drawing.Size(128, 128);
            this.selectedItem.TabIndex = 0;
            this.selectedItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.selectedItem.UseVisualStyleBackColor = false;
            this.selectedItem.Visible = false;
            // 
            // itemIDLabel
            // 
            this.itemIDLabel.AutoSize = true;
            this.itemIDLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.itemIDLabel.ForeColor = System.Drawing.Color.White;
            this.itemIDLabel.Location = new System.Drawing.Point(597, 9);
            this.itemIDLabel.Name = "itemIDLabel";
            this.itemIDLabel.Size = new System.Drawing.Size(55, 16);
            this.itemIDLabel.TabIndex = 37;
            this.itemIDLabel.Text = "Item ID";
            this.itemIDLabel.Visible = false;
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.infoLabel.ForeColor = System.Drawing.Color.White;
            this.infoLabel.Location = new System.Drawing.Point(5, 5);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(0, 16);
            this.infoLabel.TabIndex = 38;
            // 
            // variation
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(680, 265);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.itemIDLabel);
            this.Controls.Add(this.selectedItem);
            this.Controls.Add(this.furnitureGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(680, 265);
            this.MinimumSize = new System.Drawing.Size(680, 115);
            this.Name = "variation";
            this.Opacity = 0.9D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "variation";
            this.Load += new System.EventHandler(this.Variation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.furnitureGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView furnitureGridView;
        private inventorySlot selectedItem;
        private System.Windows.Forms.Label itemIDLabel;
        private System.Windows.Forms.Label infoLabel;
    }
}