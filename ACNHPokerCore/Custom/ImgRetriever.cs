using Ionic.Zip;
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class ImgRetriever : Form
    {
        public ImgRetriever()
        {
            InitializeComponent();
        }

        private void NoBtn_Click(object sender, EventArgs e)
        {
            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));
            Config.AppSettings.Settings["RestartRequired"].Value = "false";
            Config.Save(ConfigurationSaveMode.Minimal);
            Close();
        }

        private async void YesBtn_Click(object sender, EventArgs e)
        {
            yesBtn.Visible = false;
            noBtn.Visible = false;
            NowLoadingImage.Visible = true;
            waitmsg.Visible = true;

            await Task.Factory.StartNew(OldFileCleanup);
            await StartArchiveDownload();

            if (File.Exists(@"img.zip"))
            {
                waitmsg.Text = "Archive Download Complete! \r\nProceed to extract images...";
                waitmsg.Location = new System.Drawing.Point(122, 87);

                Thread unzipThread = new(ExtractHere);
                unzipThread.Start();
            }
            else
            {
                MessageBox.Show("Downloaded archive seems to be mssing/corrupted");
            }
        }

        private void OldFileCleanup()
        {
            if (Directory.Exists(@"img"))
                Directory.Delete(@"img", true);

            if (File.Exists(@"img.zip"))
                File.Delete(@"img.zip");
        }

        private async Task StartArchiveDownload()
        {
            string downloadFileUrl = "https://github.com/MyShiLingStar/ACNHPokerCore/releases/download/ImgPack8/img.zip";
            string destinationFilePath = Path.GetFullPath("img.zip");

            using (var client = new HttpClientDownloadWithProgress(downloadFileUrl, destinationFilePath))
            {
                client.ProgressChanged += (_, _, progressPercentage) =>
                {
                    if (progressPercentage != null) progressBar.Value = (int)progressPercentage;
                };

                await client.StartDownload();
            }
        }

        private void ExtractHere()
        {
            //Thread.Sleep(10000);

            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));
            Config.AppSettings.Settings["RestartRequired"].Value = "true";
            Config.Save(ConfigurationSaveMode.Minimal);

            string path = @"" + Environment.CurrentDirectory + "\\img.zip";

            try
            {
                using (ZipFile imgZip = ZipFile.Read(path))
                {
                    imgZip.ExtractProgress += ZipExtractProgress;

                    imgZip.ExtractAll(@"" + Environment.CurrentDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch (ZipException e)
            {
                MessageBox.Show("Downloaded archive seems to be corrupted: " + e.Message);
            }

            Invoke((MethodInvoker)delegate { Close(); });
        }

        private void ZipExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry)
            {
                Invoke((MethodInvoker)delegate { { progressBar.Value = e.EntriesExtracted * 100 / e.EntriesTotal; } });
            }
        }
    }
}
