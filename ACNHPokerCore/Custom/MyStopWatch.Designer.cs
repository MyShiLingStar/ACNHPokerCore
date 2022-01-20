
namespace ACNHPokerCore
{
    partial class MyStopWatch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyStopWatch));
            this.StopWatchImageBox = new System.Windows.Forms.PictureBox();
            this.timeLabel = new System.Windows.Forms.Label();
            this.StartBtn = new System.Windows.Forms.Button();
            this.pauseBtn = new System.Windows.Forms.Button();
            this.restartBtn = new System.Windows.Forms.Button();
            this.resetBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.StopWatchImageBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // StopWatchImageBox
            // 
            this.StopWatchImageBox.Image = global::ACNHPokerCore.Properties.Resources.timer;
            this.StopWatchImageBox.Location = new System.Drawing.Point(1, 1);
            this.StopWatchImageBox.Name = "StopWatchImageBox";
            this.StopWatchImageBox.Size = new System.Drawing.Size(269, 103);
            this.StopWatchImageBox.TabIndex = 0;
            this.StopWatchImageBox.TabStop = false;
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(241)))), ((int)(((byte)(205)))));
            this.timeLabel.Font = new System.Drawing.Font("Arial", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.timeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(152)))), ((int)(((byte)(8)))));
            this.timeLabel.Location = new System.Drawing.Point(-5, 5);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(149, 56);
            this.timeLabel.TabIndex = 1;
            this.timeLabel.Text = "10:00";
            // 
            // StartBtn
            // 
            this.StartBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(152)))), ((int)(((byte)(8)))));
            this.StartBtn.FlatAppearance.BorderSize = 0;
            this.StartBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.StartBtn.ForeColor = System.Drawing.Color.White;
            this.StartBtn.Location = new System.Drawing.Point(12, 109);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(99, 34);
            this.StartBtn.TabIndex = 173;
            this.StartBtn.Text = "Start";
            this.StartBtn.UseVisualStyleBackColor = false;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // pauseBtn
            // 
            this.pauseBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(152)))), ((int)(((byte)(8)))));
            this.pauseBtn.FlatAppearance.BorderSize = 0;
            this.pauseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pauseBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.pauseBtn.ForeColor = System.Drawing.Color.White;
            this.pauseBtn.Location = new System.Drawing.Point(135, 109);
            this.pauseBtn.Name = "pauseBtn";
            this.pauseBtn.Size = new System.Drawing.Size(99, 34);
            this.pauseBtn.TabIndex = 174;
            this.pauseBtn.Text = "pause";
            this.pauseBtn.UseVisualStyleBackColor = false;
            this.pauseBtn.Click += new System.EventHandler(this.pauseBtn_Click);
            // 
            // restartBtn
            // 
            this.restartBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(152)))), ((int)(((byte)(8)))));
            this.restartBtn.FlatAppearance.BorderSize = 0;
            this.restartBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.restartBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.restartBtn.ForeColor = System.Drawing.Color.White;
            this.restartBtn.Location = new System.Drawing.Point(12, 186);
            this.restartBtn.Name = "restartBtn";
            this.restartBtn.Size = new System.Drawing.Size(99, 34);
            this.restartBtn.TabIndex = 175;
            this.restartBtn.Text = "restart";
            this.restartBtn.UseVisualStyleBackColor = false;
            this.restartBtn.Click += new System.EventHandler(this.restartBtn_Click);
            // 
            // resetBtn
            // 
            this.resetBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(152)))), ((int)(((byte)(8)))));
            this.resetBtn.FlatAppearance.BorderSize = 0;
            this.resetBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetBtn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.resetBtn.ForeColor = System.Drawing.Color.White;
            this.resetBtn.Location = new System.Drawing.Point(153, 186);
            this.resetBtn.Name = "resetBtn";
            this.resetBtn.Size = new System.Drawing.Size(99, 34);
            this.resetBtn.TabIndex = 176;
            this.resetBtn.Text = "reset";
            this.resetBtn.UseVisualStyleBackColor = false;
            this.resetBtn.Click += new System.EventHandler(this.resetBtn_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(241)))), ((int)(((byte)(205)))));
            this.panel1.Controls.Add(this.timeLabel);
            this.panel1.Location = new System.Drawing.Point(93, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(137, 66);
            this.panel1.TabIndex = 177;
            // 
            // MyStopWatch
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Lime;
            this.ClientSize = new System.Drawing.Size(264, 106);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.resetBtn);
            this.Controls.Add(this.restartBtn);
            this.Controls.Add(this.pauseBtn);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.StopWatchImageBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(280, 280);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(280, 145);
            this.Name = "MyStopWatch";
            this.ShowInTaskbar = false;
            this.Text = "PocketCham\'s Timer";
            this.TransparencyKey = System.Drawing.Color.Lime;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MyStopWatch_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.StopWatchImageBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox StopWatchImageBox;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Button pauseBtn;
        private System.Windows.Forms.Button restartBtn;
        private System.Windows.Forms.Button resetBtn;
        private System.Windows.Forms.Panel panel1;
    }
}