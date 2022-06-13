namespace ACNHPokerCore
{
    partial class Chat
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
            this.chatBox = new System.Windows.Forms.RichTextBox();
            this.chatButton = new System.Windows.Forms.Button();
            this.RetainChat = new System.Windows.Forms.CheckBox();
            this.SafetyCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chatBox
            // 
            this.chatBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(68)))), ((int)(((byte)(75)))));
            this.chatBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chatBox.DetectUrls = false;
            this.chatBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.chatBox.ForeColor = System.Drawing.Color.White;
            this.chatBox.Location = new System.Drawing.Point(12, 12);
            this.chatBox.MaxLength = 24;
            this.chatBox.Name = "chatBox";
            this.chatBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.chatBox.Size = new System.Drawing.Size(365, 90);
            this.chatBox.TabIndex = 3;
            this.chatBox.Text = "";
            this.chatBox.TextChanged += new System.EventHandler(this.chatBox_TextChanged);
            this.chatBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chatBox_KeyDown);
            // 
            // chatButton
            // 
            this.chatButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.chatButton.FlatAppearance.BorderSize = 0;
            this.chatButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chatButton.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.chatButton.ForeColor = System.Drawing.Color.White;
            this.chatButton.Location = new System.Drawing.Point(383, 40);
            this.chatButton.Name = "chatButton";
            this.chatButton.Size = new System.Drawing.Size(54, 62);
            this.chatButton.TabIndex = 41;
            this.chatButton.Tag = "";
            this.chatButton.Text = "Send";
            this.chatButton.UseVisualStyleBackColor = false;
            this.chatButton.Click += new System.EventHandler(this.chatButton_Click);
            // 
            // RetainChat
            // 
            this.RetainChat.AutoSize = true;
            this.RetainChat.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RetainChat.ForeColor = System.Drawing.Color.White;
            this.RetainChat.Location = new System.Drawing.Point(383, 21);
            this.RetainChat.Name = "RetainChat";
            this.RetainChat.Size = new System.Drawing.Size(62, 19);
            this.RetainChat.TabIndex = 42;
            this.RetainChat.Text = "Retain";
            this.RetainChat.UseVisualStyleBackColor = true;
            // 
            // SafetyCheck
            // 
            this.SafetyCheck.AutoSize = true;
            this.SafetyCheck.Checked = true;
            this.SafetyCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SafetyCheck.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SafetyCheck.ForeColor = System.Drawing.Color.White;
            this.SafetyCheck.Location = new System.Drawing.Point(383, 3);
            this.SafetyCheck.Name = "SafetyCheck";
            this.SafetyCheck.Size = new System.Drawing.Size(62, 19);
            this.SafetyCheck.TabIndex = 43;
            this.SafetyCheck.Text = "Safety";
            this.SafetyCheck.UseVisualStyleBackColor = true;
            this.SafetyCheck.CheckedChanged += new System.EventHandler(this.SafetyCheck_CheckedChanged);
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.ClientSize = new System.Drawing.Size(448, 114);
            this.ControlBox = false;
            this.Controls.Add(this.chatButton);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.RetainChat);
            this.Controls.Add(this.SafetyCheck);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Chat";
            this.Opacity = 0.97D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Chat";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Chat_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox chatBox;
        private System.Windows.Forms.Button chatButton;
        private System.Windows.Forms.CheckBox RetainChat;
        private System.Windows.Forms.CheckBox SafetyCheck;
    }
}