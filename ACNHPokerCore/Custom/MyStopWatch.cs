using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class MyStopWatch : Form, IDisposable
    {
        CountDownTimer timer;

        public int minutes = 10;
        public int seconds = 0;
        public bool done = false;

        public MyStopWatch()
        {
            InitializeComponent();
            timer = new CountDownTimer();

            //set to 30 mins
            timer.SetTime(minutes, seconds);

            //update label text
            timer.TimeChanged += () => timeLabel.Text = timer.TimeLeftStr;

            // show messageBox on timer = 00:00.000
            timer.CountDownFinished += () => toRed();

            timer.StepMs = 77;
        }

        private void toRed()
        {
            timeLabel.ForeColor = Color.Red;
            done = true;
        }

        private void toOrange()
        {
            timeLabel.ForeColor = Color.FromArgb(243, 152, 8);
            done = false;
        }

        public bool isDone()
        {
            return done;
        }

        public void set(int min, int sec)
        {
            minutes = min;
            seconds = sec;
            timer.SetTime(minutes, seconds);
            updateLabel();
            done = false;
        }

        public void addMin(int min)
        {
            if (minutes + min >= 59)
                minutes = 59;
            else
                minutes += min;

            timer.SetTime(minutes, seconds);
            updateLabel();
        }

        public void minusMin(int min)
        {
            if (minutes - min <= 0)
                minutes = 0;
            else
                minutes -= min;

            timer.SetTime(minutes, seconds);
            updateLabel();
        }

        public void addSec(int sec)
        {
            if (seconds + sec >= 59)
                seconds = 59;
            else
                seconds += sec;

            timer.SetTime(minutes, seconds);
            updateLabel();
        }

        public void minusSec(int sec)
        {
            if (seconds - sec <= 0)
                seconds = 0;
            else
                seconds -= sec;

            timer.SetTime(minutes, seconds);
            updateLabel();
        }

        private void updateLabel()
        {
            timeLabel.Text = timer.TimeLeftStr;
        }

        public void start()
        {
            timer.Start();
        }

        public void pause()
        {
            timer.Pause();
        }

        public void restart()
        {
            toOrange();
            timer.Restart();
        }

        public void reset()
        {
            toOrange();
            timer.Reset();
            updateLabel();
        }

        public void stop()
        {
            timer.Stop();
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            start();
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            pause();
        }

        private void restartBtn_Click(object sender, EventArgs e)
        {
            restart();
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void MyStopWatch_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Dispose();
        }
    }
}
