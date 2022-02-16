using Ionic.Zip;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class ImgRetriever : Form
    {
        public ImgRetriever()
        {
            InitializeComponent();
        }

        private void noBtn_Click(object sender, EventArgs e)
        {
            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));
            Config.AppSettings.Settings["RestartRequired"].Value = "false";
            Config.Save(ConfigurationSaveMode.Minimal);
            this.Close();
        }

        private void yesBtn_Click(object sender, EventArgs e)
        {
            yesBtn.Visible = false;
            noBtn.Visible = false;
            progressBar.Visible = true;

            string path = Path.Combine(Application.StartupPath, "img.zip");

            if (File.Exists(@"img.zip"))
            {
                File.Delete(@"img.zip");
            }

            WebClient webClient = new WebClient
            {
                Proxy = null
            };

            webClient.DownloadProgressChanged += (s, ez) =>
            {
                progressBar.Value = ez.ProgressPercentage;
            };

            webClient.DownloadFileCompleted += (s, ez) =>
            {
                waitmsg.Visible = true;
                progressBar.Visible = false;

                Thread unzipThread = new Thread(delegate () { extractHere(); });
                unzipThread.Start();
            };

            webClient.DownloadFileAsync(new Uri("https://github.com/MyShiLingStar/ACNHPokerCore/releases/download/ImgPack8/img.zip"), "img.zip");

        }

        private void extractHere()
        {
            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));
            Config.AppSettings.Settings["RestartRequired"].Value = "true";
            Config.Save(ConfigurationSaveMode.Minimal);

            try
            {
                using (ZipFile archive = new ZipFile(@"" + System.Environment.CurrentDirectory + "\\img.zip"))
                {
                    archive.ExtractAll(@"" + System.Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch
            {

            }

            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }

    }
}
