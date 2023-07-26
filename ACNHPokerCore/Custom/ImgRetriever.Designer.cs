namespace ACNHPokerCore
{
    partial class ImgRetriever
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImgRetriever));
            progressBar = new System.Windows.Forms.ProgressBar();
            yesBtn = new System.Windows.Forms.Button();
            noBtn = new System.Windows.Forms.Button();
            msg = new System.Windows.Forms.Label();
            waitmsg = new System.Windows.Forms.Label();
            configBtn = new System.Windows.Forms.Button();
            NowLoadingImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)NowLoadingImage).BeginInit();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.BackColor = System.Drawing.Color.Black;
            progressBar.ForeColor = System.Drawing.Color.Pink;
            progressBar.Location = new System.Drawing.Point(12, 127);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(410, 20);
            progressBar.TabIndex = 5;
            // 
            // yesBtn
            // 
            yesBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            yesBtn.FlatAppearance.BorderSize = 0;
            yesBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            yesBtn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            yesBtn.ForeColor = System.Drawing.Color.Transparent;
            yesBtn.Location = new System.Drawing.Point(85, 79);
            yesBtn.Name = "yesBtn";
            yesBtn.Size = new System.Drawing.Size(116, 32);
            yesBtn.TabIndex = 4;
            yesBtn.Text = "Yes";
            yesBtn.UseVisualStyleBackColor = false;
            yesBtn.Click += YesBtn_Click;
            // 
            // noBtn
            // 
            noBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            noBtn.FlatAppearance.BorderSize = 0;
            noBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            noBtn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            noBtn.ForeColor = System.Drawing.Color.Transparent;
            noBtn.Location = new System.Drawing.Point(236, 79);
            noBtn.Name = "noBtn";
            noBtn.Size = new System.Drawing.Size(116, 32);
            noBtn.TabIndex = 3;
            noBtn.Text = "No";
            noBtn.UseVisualStyleBackColor = false;
            noBtn.Click += NoBtn_Click;
            // 
            // msg
            // 
            msg.AutoSize = true;
            msg.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            msg.ForeColor = System.Drawing.Color.White;
            msg.Location = new System.Drawing.Point(23, 9);
            msg.Name = "msg";
            msg.Size = new System.Drawing.Size(387, 57);
            msg.TabIndex = 2;
            msg.Text = "Would you like to download the item sprites now?\r\n\r\n(You can find this dialog in           again if needed.)";
            msg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // waitmsg
            // 
            waitmsg.AutoSize = true;
            waitmsg.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            waitmsg.ForeColor = System.Drawing.Color.White;
            waitmsg.Location = new System.Drawing.Point(122, 95);
            waitmsg.Name = "waitmsg";
            waitmsg.Size = new System.Drawing.Size(213, 16);
            waitmsg.TabIndex = 1;
            waitmsg.Text = "Downloading Image Archive...";
            waitmsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            waitmsg.Visible = false;
            // 
            // configBtn
            // 
            configBtn.BackColor = System.Drawing.Color.FromArgb(114, 137, 218);
            configBtn.FlatAppearance.BorderSize = 0;
            configBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            configBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            configBtn.ForeColor = System.Drawing.Color.White;
            configBtn.Image = Properties.Resources.gear;
            configBtn.Location = new System.Drawing.Point(241, 43);
            configBtn.Name = "configBtn";
            configBtn.Size = new System.Drawing.Size(25, 25);
            configBtn.TabIndex = 0;
            configBtn.UseVisualStyleBackColor = false;
            // 
            // NowLoadingImage
            // 
            NowLoadingImage.Image = Properties.Resources.loading;
            NowLoadingImage.Location = new System.Drawing.Point(92, 91);
            NowLoadingImage.Name = "NowLoadingImage";
            NowLoadingImage.Size = new System.Drawing.Size(24, 24);
            NowLoadingImage.TabIndex = 6;
            NowLoadingImage.TabStop = false;
            NowLoadingImage.Visible = false;
            // 
            // ImgRetriever
            // 
            AcceptButton = yesBtn;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            CancelButton = noBtn;
            ClientSize = new System.Drawing.Size(434, 161);
            Controls.Add(progressBar);
            Controls.Add(configBtn);
            Controls.Add(msg);
            Controls.Add(noBtn);
            Controls.Add(yesBtn);
            Controls.Add(NowLoadingImage);
            Controls.Add(waitmsg);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(450, 255);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(450, 200);
            Name = "ImgRetriever";
            Opacity = 0.95D;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Golden Image Retriever";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)NowLoadingImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button yesBtn;
        private System.Windows.Forms.Button noBtn;
        private System.Windows.Forms.Label msg;
        private System.Windows.Forms.Label waitmsg;
        private System.Windows.Forms.Button configBtn;
        private System.Windows.Forms.PictureBox NowLoadingImage;
    }
}