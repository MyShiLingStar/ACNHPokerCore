
namespace ACNHPokerCore
{
    partial class OrderDisplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderDisplay));
            this.itemImage1 = new System.Windows.Forms.PictureBox();
            this.itemImage2 = new System.Windows.Forms.PictureBox();
            this.itemImage3 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.itemImage1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemImage2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemImage3)).BeginInit();
            this.SuspendLayout();
            // 
            // itemImage1
            // 
            this.itemImage1.Location = new System.Drawing.Point(160, 0);
            this.itemImage1.Name = "itemImage1";
            this.itemImage1.Size = new System.Drawing.Size(128, 128);
            this.itemImage1.TabIndex = 0;
            this.itemImage1.TabStop = false;
            // 
            // itemImage2
            // 
            this.itemImage2.Location = new System.Drawing.Point(64, 32);
            this.itemImage2.Name = "itemImage2";
            this.itemImage2.Size = new System.Drawing.Size(96, 96);
            this.itemImage2.TabIndex = 1;
            this.itemImage2.TabStop = false;
            // 
            // itemImage3
            // 
            this.itemImage3.Location = new System.Drawing.Point(0, 64);
            this.itemImage3.Name = "itemImage3";
            this.itemImage3.Size = new System.Drawing.Size(64, 64);
            this.itemImage3.TabIndex = 2;
            this.itemImage3.TabStop = false;
            // 
            // OrderDisplay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Lime;
            this.ClientSize = new System.Drawing.Size(288, 128);
            this.Controls.Add(this.itemImage3);
            this.Controls.Add(this.itemImage2);
            this.Controls.Add(this.itemImage1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(304, 167);
            this.MinimumSize = new System.Drawing.Size(304, 167);
            this.Name = "OrderDisplay";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "OrderDisplay";
            ((System.ComponentModel.ISupportInitialize)(this.itemImage1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemImage2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemImage3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox itemImage1;
        private System.Windows.Forms.PictureBox itemImage2;
        private System.Windows.Forms.PictureBox itemImage3;
    }
}