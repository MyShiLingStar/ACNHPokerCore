namespace ACNHPokerCore
{
    partial class MyCheck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyCheck));
            hwidLabel = new System.Windows.Forms.RichTextBox();
            answerBox3 = new System.Windows.Forms.RichTextBox();
            SubmitButton = new System.Windows.Forms.Button();
            answerBox4 = new System.Windows.Forms.RichTextBox();
            answerBox5 = new System.Windows.Forms.RichTextBox();
            hwidDisplay = new System.Windows.Forms.RichTextBox();
            answerBox2 = new System.Windows.Forms.RichTextBox();
            answerBox1 = new System.Windows.Forms.RichTextBox();
            answerBox6 = new System.Windows.Forms.RichTextBox();
            SuspendLayout();
            // 
            // hwidLabel
            // 
            hwidLabel.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            hwidLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            hwidLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            hwidLabel.ForeColor = System.Drawing.Color.White;
            hwidLabel.Location = new System.Drawing.Point(11, 12);
            hwidLabel.Multiline = false;
            hwidLabel.Name = "hwidLabel";
            hwidLabel.ReadOnly = true;
            hwidLabel.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            hwidLabel.Size = new System.Drawing.Size(300, 24);
            hwidLabel.TabIndex = 0;
            hwidLabel.TabStop = false;
            hwidLabel.Text = "Your Hardware ID : ";
            // 
            // answerBox3
            // 
            answerBox3.BackColor = System.Drawing.Color.FromArgb(64, 68, 75);
            answerBox3.Font = new System.Drawing.Font("Arial", 22F, System.Drawing.FontStyle.Bold);
            answerBox3.ForeColor = System.Drawing.Color.White;
            answerBox3.Location = new System.Drawing.Point(11, 72);
            answerBox3.MaxLength = 4;
            answerBox3.Multiline = false;
            answerBox3.Name = "answerBox3";
            answerBox3.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            answerBox3.Size = new System.Drawing.Size(96, 41);
            answerBox3.TabIndex = 4;
            answerBox3.Text = "";
            // 
            // SubmitButton
            // 
            SubmitButton.BackColor = System.Drawing.Color.Red;
            SubmitButton.FlatAppearance.BorderSize = 0;
            SubmitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            SubmitButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            SubmitButton.ForeColor = System.Drawing.Color.White;
            SubmitButton.Location = new System.Drawing.Point(11, 119);
            SubmitButton.Name = "SubmitButton";
            SubmitButton.Size = new System.Drawing.Size(300, 31);
            SubmitButton.TabIndex = 10;
            SubmitButton.Text = "Submit";
            SubmitButton.UseVisualStyleBackColor = false;
            SubmitButton.Click += SubmitButton_Click;
            // 
            // answerBox4
            // 
            answerBox4.BackColor = System.Drawing.Color.FromArgb(64, 68, 75);
            answerBox4.Font = new System.Drawing.Font("Arial", 22F, System.Drawing.FontStyle.Bold);
            answerBox4.ForeColor = System.Drawing.Color.White;
            answerBox4.Location = new System.Drawing.Point(113, 72);
            answerBox4.MaxLength = 20;
            answerBox4.Multiline = false;
            answerBox4.Name = "answerBox4";
            answerBox4.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            answerBox4.Size = new System.Drawing.Size(96, 41);
            answerBox4.TabIndex = 5;
            answerBox4.Text = "";
            // 
            // answerBox5
            // 
            answerBox5.BackColor = System.Drawing.Color.FromArgb(64, 68, 75);
            answerBox5.Font = new System.Drawing.Font("Arial", 22F, System.Drawing.FontStyle.Bold);
            answerBox5.ForeColor = System.Drawing.Color.White;
            answerBox5.Location = new System.Drawing.Point(215, 72);
            answerBox5.MaxLength = 4;
            answerBox5.Multiline = false;
            answerBox5.Name = "answerBox5";
            answerBox5.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            answerBox5.Size = new System.Drawing.Size(96, 41);
            answerBox5.TabIndex = 6;
            answerBox5.Text = "";
            // 
            // hwidDisplay
            // 
            hwidDisplay.BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            hwidDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            hwidDisplay.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            hwidDisplay.ForeColor = System.Drawing.Color.White;
            hwidDisplay.Location = new System.Drawing.Point(11, 44);
            hwidDisplay.Multiline = false;
            hwidDisplay.Name = "hwidDisplay";
            hwidDisplay.ReadOnly = true;
            hwidDisplay.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            hwidDisplay.Size = new System.Drawing.Size(300, 24);
            hwidDisplay.TabIndex = 1;
            hwidDisplay.TabStop = false;
            hwidDisplay.Text = "DDDDDDDDDDDDDDDDDDDDDDDDDD";
            // 
            // answerBox2
            // 
            answerBox2.BackColor = System.Drawing.Color.FromArgb(64, 68, 75);
            answerBox2.Enabled = false;
            answerBox2.Font = new System.Drawing.Font("Arial", 22F, System.Drawing.FontStyle.Bold);
            answerBox2.ForeColor = System.Drawing.Color.White;
            answerBox2.Location = new System.Drawing.Point(164, 184);
            answerBox2.MaxLength = 10;
            answerBox2.Multiline = false;
            answerBox2.Name = "answerBox2";
            answerBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            answerBox2.Size = new System.Drawing.Size(0, 0);
            answerBox2.TabIndex = 249;
            answerBox2.Text = "enter";
            answerBox2.Visible = false;
            // 
            // answerBox1
            // 
            answerBox1.BackColor = System.Drawing.Color.FromArgb(64, 68, 75);
            answerBox1.Enabled = false;
            answerBox1.Font = new System.Drawing.Font("Arial", 22F, System.Drawing.FontStyle.Bold);
            answerBox1.ForeColor = System.Drawing.Color.White;
            answerBox1.Location = new System.Drawing.Point(62, 184);
            answerBox1.MaxLength = 10;
            answerBox1.Multiline = false;
            answerBox1.Name = "answerBox1";
            answerBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            answerBox1.Size = new System.Drawing.Size(0, 0);
            answerBox1.TabIndex = 248;
            answerBox1.Text = "Please";
            answerBox1.Visible = false;
            // 
            // answerBox6
            // 
            answerBox6.BackColor = System.Drawing.Color.FromArgb(64, 68, 75);
            answerBox6.Enabled = false;
            answerBox6.Font = new System.Drawing.Font("Arial", 22F, System.Drawing.FontStyle.Bold);
            answerBox6.ForeColor = System.Drawing.Color.White;
            answerBox6.Location = new System.Drawing.Point(113, 231);
            answerBox6.MaxLength = 10;
            answerBox6.Multiline = false;
            answerBox6.Name = "answerBox6";
            answerBox6.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            answerBox6.Size = new System.Drawing.Size(0, 0);
            answerBox6.TabIndex = 250;
            answerBox6.Text = "66666";
            answerBox6.Visible = false;
            // 
            // MyCheck
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            ClientSize = new System.Drawing.Size(321, 160);
            Controls.Add(answerBox6);
            Controls.Add(answerBox2);
            Controls.Add(answerBox1);
            Controls.Add(hwidDisplay);
            Controls.Add(answerBox5);
            Controls.Add(answerBox4);
            Controls.Add(SubmitButton);
            Controls.Add(answerBox3);
            Controls.Add(hwidLabel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MyCheck";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Please enter your AC:NH Key!";
            TopMost = true;
            KeyDown += MyWarning_KeyDown;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.RichTextBox hwidLabel;
        private System.Windows.Forms.Label pleaseTypeLabel;
        private System.Windows.Forms.Label toConfirmLabel;
        private System.Windows.Forms.RichTextBox sampleBox;
        private System.Windows.Forms.RichTextBox answerBox3;
        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.RichTextBox answerBox4;
        private System.Windows.Forms.RichTextBox answerBox5;
        private System.Windows.Forms.RichTextBox hwidDisplay;
        private System.Windows.Forms.PictureBox warningImg1;
        private System.Windows.Forms.PictureBox warningImg2;
        private System.Windows.Forms.Panel PleaseWaitPanel;
        private System.Windows.Forms.Label PleaseWaitLabel;
        private System.Windows.Forms.PictureBox PleaseWaitImage;
        private System.Windows.Forms.RichTextBox answerBox2;
        private System.Windows.Forms.RichTextBox answerBox1;
        private System.Windows.Forms.RichTextBox answerBox6;
    }
}