using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class MyStopWatch : Form
    {
        readonly CountDownTimer timer;

        public int minutes = 10;
        public int seconds;
        public bool done;

        public MyStopWatch()
        {
            InitializeComponent();
            timer = new CountDownTimer();

            //set to 30 mins
            timer.SetTime(minutes, seconds);

            //update label text
            timer.TimeChanged += () => timeLabel.Text = timer.TimeLeftStr;

            // show messageBox on timer = 00:00.000
            timer.CountDownFinished += () => ToRed();

            timer.StepMs = 77;
        }

        private void ToRed()
        {
            timeLabel.ForeColor = Color.Red;
            done = true;
        }

        private void ToOrange()
        {
            timeLabel.ForeColor = Color.FromArgb(243, 152, 8);
            done = false;
        }

        public bool IsDone()
        {
            return done;
        }

        public void Set(int min, int sec)
        {
            minutes = min;
            seconds = sec;
            timer.SetTime(minutes, seconds);
            UpdateLabel();
            done = false;
        }

        public void AddMin(int min)
        {
            if (minutes + min >= 59)
                minutes = 59;
            else
                minutes += min;

            timer.SetTime(minutes, seconds);
            UpdateLabel();
        }

        public void MinusMin(int min)
        {
            if (minutes - min <= 0)
                minutes = 0;
            else
                minutes -= min;

            timer.SetTime(minutes, seconds);
            UpdateLabel();
        }

        public void AddSec(int sec)
        {
            if (seconds + sec >= 59)
                seconds = 59;
            else
                seconds += sec;

            timer.SetTime(minutes, seconds);
            UpdateLabel();
        }

        public void MinusSec(int sec)
        {
            if (seconds - sec <= 0)
                seconds = 0;
            else
                seconds -= sec;

            timer.SetTime(minutes, seconds);
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            timeLabel.Text = timer.TimeLeftStr;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Pause()
        {
            timer.Pause();
        }

        public void Restart()
        {
            ToOrange();
            timer.Restart();
        }

        public void Reset()
        {
            ToOrange();
            timer.Reset();
            UpdateLabel();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void PauseBtn_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void RestartBtn_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void MyStopWatch_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Dispose();
        }
    }
}
