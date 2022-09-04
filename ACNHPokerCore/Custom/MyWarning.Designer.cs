namespace ACNHPokerCore
{
    partial class MyWarning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyWarning));
            this.message1 = new System.Windows.Forms.RichTextBox();
            this.pleaseTypeLabel = new System.Windows.Forms.Label();
            this.toConfirmLabel = new System.Windows.Forms.Label();
            this.sampleBox = new System.Windows.Forms.RichTextBox();
            this.answerBox = new System.Windows.Forms.RichTextBox();
            this.confirmBtn = new System.Windows.Forms.Button();
            this.warningImg1 = new System.Windows.Forms.PictureBox();
            this.warningImg2 = new System.Windows.Forms.PictureBox();
            this.PleaseWaitPanel = new System.Windows.Forms.Panel();
            this.PleaseWaitLabel = new System.Windows.Forms.Label();
            this.PleaseWaitImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.warningImg1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningImg2)).BeginInit();
            this.PleaseWaitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PleaseWaitImage)).BeginInit();
            this.SuspendLayout();
            // 
            // message1
            // 
            this.message1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.message1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.message1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.message1.ForeColor = System.Drawing.Color.White;
            this.message1.Location = new System.Drawing.Point(12, 12);
            this.message1.Multiline = false;
            this.message1.Name = "message1";
            this.message1.ReadOnly = true;
            this.message1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.message1.Size = new System.Drawing.Size(678, 24);
            this.message1.TabIndex = 0;
            this.message1.TabStop = false;
            this.message1.Text = "Flattening terrain might leave your river mouth(s) in a state that is not easily " +
    "recoverable.";
            // 
            // pleaseTypeLabel
            // 
            this.pleaseTypeLabel.AutoSize = true;
            this.pleaseTypeLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.pleaseTypeLabel.ForeColor = System.Drawing.Color.White;
            this.pleaseTypeLabel.Location = new System.Drawing.Point(12, 197);
            this.pleaseTypeLabel.Name = "pleaseTypeLabel";
            this.pleaseTypeLabel.Size = new System.Drawing.Size(97, 19);
            this.pleaseTypeLabel.TabIndex = 1;
            this.pleaseTypeLabel.Text = "Please type";
            // 
            // toConfirmLabel
            // 
            this.toConfirmLabel.AutoSize = true;
            this.toConfirmLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.toConfirmLabel.ForeColor = System.Drawing.Color.White;
            this.toConfirmLabel.Location = new System.Drawing.Point(600, 197);
            this.toConfirmLabel.Name = "toConfirmLabel";
            this.toConfirmLabel.Size = new System.Drawing.Size(90, 19);
            this.toConfirmLabel.TabIndex = 2;
            this.toConfirmLabel.Text = "to confirm.";
            // 
            // sampleBox
            // 
            this.sampleBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.sampleBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sampleBox.Font = new System.Drawing.Font("Arial", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.sampleBox.ForeColor = System.Drawing.Color.Gainsboro;
            this.sampleBox.Location = new System.Drawing.Point(115, 195);
            this.sampleBox.Multiline = false;
            this.sampleBox.Name = "sampleBox";
            this.sampleBox.ReadOnly = true;
            this.sampleBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.sampleBox.Size = new System.Drawing.Size(479, 24);
            this.sampleBox.TabIndex = 3;
            this.sampleBox.TabStop = false;
            this.sampleBox.Text = "I will not ask for help about fixing river mouth";
            // 
            // answerBox
            // 
            this.answerBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.answerBox.Font = new System.Drawing.Font("Arial", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.answerBox.ForeColor = System.Drawing.Color.White;
            this.answerBox.Location = new System.Drawing.Point(12, 228);
            this.answerBox.Multiline = false;
            this.answerBox.Name = "answerBox";
            this.answerBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.answerBox.Size = new System.Drawing.Size(678, 41);
            this.answerBox.TabIndex = 4;
            this.answerBox.Text = "";
            this.answerBox.TextChanged += new System.EventHandler(this.AnswerBox_TextChanged);
            // 
            // confirmBtn
            // 
            this.confirmBtn.BackColor = System.Drawing.Color.Red;
            this.confirmBtn.FlatAppearance.BorderSize = 0;
            this.confirmBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.confirmBtn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.confirmBtn.ForeColor = System.Drawing.Color.White;
            this.confirmBtn.Location = new System.Drawing.Point(12, 277);
            this.confirmBtn.Name = "confirmBtn";
            this.confirmBtn.Size = new System.Drawing.Size(678, 31);
            this.confirmBtn.TabIndex = 244;
            this.confirmBtn.Text = "I understand, flatten all terrain.";
            this.confirmBtn.UseVisualStyleBackColor = false;
            this.confirmBtn.Visible = false;
            this.confirmBtn.Click += new System.EventHandler(this.ConfirmBtn_Click);
            // 
            // warningImg1
            // 
            this.warningImg1.Image = global::ACNHPokerCore.Properties.Resources.w1;
            this.warningImg1.Location = new System.Drawing.Point(100, 39);
            this.warningImg1.Name = "warningImg1";
            this.warningImg1.Size = new System.Drawing.Size(225, 150);
            this.warningImg1.TabIndex = 245;
            this.warningImg1.TabStop = false;
            // 
            // warningImg2
            // 
            this.warningImg2.Image = global::ACNHPokerCore.Properties.Resources.w2;
            this.warningImg2.Location = new System.Drawing.Point(393, 39);
            this.warningImg2.Name = "warningImg2";
            this.warningImg2.Size = new System.Drawing.Size(225, 150);
            this.warningImg2.TabIndex = 246;
            this.warningImg2.TabStop = false;
            // 
            // PleaseWaitPanel
            // 
            this.PleaseWaitPanel.Controls.Add(this.PleaseWaitLabel);
            this.PleaseWaitPanel.Controls.Add(this.PleaseWaitImage);
            this.PleaseWaitPanel.Location = new System.Drawing.Point(259, 277);
            this.PleaseWaitPanel.Name = "PleaseWaitPanel";
            this.PleaseWaitPanel.Size = new System.Drawing.Size(185, 30);
            this.PleaseWaitPanel.TabIndex = 247;
            this.PleaseWaitPanel.Visible = false;
            // 
            // PleaseWaitLabel
            // 
            this.PleaseWaitLabel.AutoSize = true;
            this.PleaseWaitLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.PleaseWaitLabel.ForeColor = System.Drawing.Color.White;
            this.PleaseWaitLabel.Location = new System.Drawing.Point(29, 4);
            this.PleaseWaitLabel.Name = "PleaseWaitLabel";
            this.PleaseWaitLabel.Size = new System.Drawing.Size(130, 22);
            this.PleaseWaitLabel.TabIndex = 242;
            this.PleaseWaitLabel.Text = "Please Wait...";
            // 
            // PleaseWaitImage
            // 
            this.PleaseWaitImage.Image = global::ACNHPokerCore.Properties.Resources.loading;
            this.PleaseWaitImage.Location = new System.Drawing.Point(2, 3);
            this.PleaseWaitImage.Name = "PleaseWaitImage";
            this.PleaseWaitImage.Size = new System.Drawing.Size(24, 24);
            this.PleaseWaitImage.TabIndex = 217;
            this.PleaseWaitImage.TabStop = false;
            // 
            // MyWarning
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(702, 318);
            this.Controls.Add(this.PleaseWaitPanel);
            this.Controls.Add(this.warningImg2);
            this.Controls.Add(this.warningImg1);
            this.Controls.Add(this.confirmBtn);
            this.Controls.Add(this.answerBox);
            this.Controls.Add(this.sampleBox);
            this.Controls.Add(this.toConfirmLabel);
            this.Controls.Add(this.pleaseTypeLabel);
            this.Controls.Add(this.message1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MyWarning";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "You are about to obliterate your river mouth(s)!";
            this.TopMost = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MyWarning_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.warningImg1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningImg2)).EndInit();
            this.PleaseWaitPanel.ResumeLayout(false);
            this.PleaseWaitPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PleaseWaitImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox message1;
        private System.Windows.Forms.Label pleaseTypeLabel;
        private System.Windows.Forms.Label toConfirmLabel;
        private System.Windows.Forms.RichTextBox sampleBox;
        private System.Windows.Forms.RichTextBox answerBox;
        private System.Windows.Forms.Button confirmBtn;
        private System.Windows.Forms.PictureBox warningImg1;
        private System.Windows.Forms.PictureBox warningImg2;
        private System.Windows.Forms.Panel PleaseWaitPanel;
        private System.Windows.Forms.Label PleaseWaitLabel;
        private System.Windows.Forms.PictureBox PleaseWaitImage;
    }
}