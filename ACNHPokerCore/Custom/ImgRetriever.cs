using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
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
        private readonly bool skipDownload = false;
        int entryTotal = 0;
        int fileCounter = 0;
        private readonly string directoryName = @"img";
        private readonly string fileName = @"img.zip";
        private readonly string downloadFileUrl = "https://github.com/MyShiLingStar/ACNHPokerCore/releases/download/ImgPack9/img.zip";
        private readonly System.Windows.Forms.Timer progressTimer = new();

        public ImgRetriever()
        {
            InitializeComponent();
            progressTimer.Interval = 500;
            progressTimer.Tick += UpdateProgress;
        }

        private void UpdateProgress(object sender, EventArgs e)
        {
            if (entryTotal <= 0) { return; }
            Invoke((MethodInvoker)delegate { { progressBar.Value = fileCounter * 100 / entryTotal; } });
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

            if (!skipDownload)
            {
                await Task.Factory.StartNew(OldFileCleanup);
                await StartArchiveDownload();
            }

            if (File.Exists(fileName))
            {
                waitmsg.Text = "Archive Download Complete! \r\nProceed to extract images...";
                waitmsg.Location = new System.Drawing.Point(122, 87);

                progressTimer.Start();
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
            if (Directory.Exists(directoryName))
                Directory.Delete(directoryName, true);

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        private async Task StartArchiveDownload()
        {
            string destinationFilePath = Path.GetFullPath(fileName);

            using var client = new HttpClientDownloadWithProgress(downloadFileUrl, destinationFilePath);
            client.ProgressChanged += (_, _, progressPercentage) =>
            {
                if (progressPercentage != null) progressBar.Value = (int)progressPercentage;
            };

            await client.StartDownload();
        }

        private void ExtractHere()
        {
            //Thread.Sleep(10000);

            Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));
            Config.AppSettings.Settings["RestartRequired"].Value = "true";
            Config.Save(ConfigurationSaveMode.Minimal);

            //string path = @"" + Environment.CurrentDirectory + "\\img.zip";

            FastZipEvents events = null;
            FastZip.Overwrite overwrite = FastZip.Overwrite.Always;
            FastZip.ConfirmOverwriteDelegate confirmOverwrite = null;
            TimeSpan interval = TimeSpan.FromSeconds(1);

            bool restoreDates = false;
            bool restoreAttributes = false;
            bool createEmptyDirs = false;

            string fileFilter = null;
            string dirFilter = null;

            events = new FastZipEvents
            {
                CompletedFile = new CompletedFileHandler(CompletedFile)
            };

            var fastZip = new FastZip(events)
            {
                CreateEmptyDirectories = createEmptyDirs,
                RestoreAttributesOnExtract = restoreAttributes,
                RestoreDateTimeOnExtract = restoreDates
            };

            ZipFile zipFile = new(fileName);
            entryTotal = (int)zipFile.Count;

            try
            {
                fastZip.ExtractZip(fileName, Environment.CurrentDirectory, overwrite, confirmOverwrite, fileFilter, dirFilter, restoreDates);
            }
            catch (ZipException e)
            {
                MessageBox.Show("Downloaded archive seems to be corrupted: " + e.Message);
            }

            progressTimer.Stop();
            Invoke((MethodInvoker)delegate { Close(); });
        }

        private void CompletedFile(object sender, ScanEventArgs e)
        {
            fileCounter++;
        }

        /*
        private void ZipExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry)
            {
                Invoke((MethodInvoker)delegate { { progressBar.Value = e.EntriesExtracted * 100 / e.EntriesTotal; } });
            }
        }
        */
    }
}
