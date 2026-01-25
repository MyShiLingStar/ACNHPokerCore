using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    #region event
    public delegate void CloseHandler();
    public delegate void OverrideHandler();
    public delegate void ValidationHandler();
    public delegate void SoundHandler(bool SoundOn);
    public delegate void CaptureHandler(bool CaptureOn);
    public delegate void ReceiveVariationHandler(InventorySlot item, int type);
    public delegate void ThreadAbortHandler();
    public delegate void UpdateTurnipPriceHandler();
    #endregion

    public partial class Main : Form
    {
        #region Variable
        private static bool DEBUGGING;

        private static Socket socket;
        private static USBBot usb;
        private readonly string version = "ACNHPokerCore R25 for v3.0.0";
        private string hardwareId;

        private Panel currentPanel;

        private static DataTable itemSource;
        private static DataTable recipeSource;
        private static DataTable flowerSource;
        private static DataTable variationSource;
        private DataTable favSource;
        private static Dictionary<string, string> OverrideDict;

        private DataGridViewRow lastRow;
        private DataGridViewRow recipelastRow;
        private DataGridViewRow flowerlastRow;
        private DataGridViewRow favlastRow;

        private Setting setting;

        private Variation selection;
        private Map M;
        private MapRegenerator R;
        private Freezer F;
        private Bulldozer B;
        private Dodo D;
        private RoadRoller Ro;
        private Chat Ch;

        private InventorySlot selectedButton;
        private int selectedSlot = 1;
        private int counter;

        private static byte[] header;
        private string IslandName = "";
        private int CurrentPlayerIndex;
        private Villager[] V;
        private int[] HouseList;
        private Button[] villagerButton;
        private Button selectedVillagerButton;

        private bool VillagerFirstLoad = true;
        private bool VillagerLoading;

        private bool overrideSetting;
        private bool validation = true;
        private bool connecting = false;
        public bool sound = true;
        public bool capturesetting = false;
        public bool autorefill = false;
        private bool init = true;
        private static string languageSetting = "eng";

        private const string insectAppearFileName = @"InsectAppearParam.bin";
        private const string fishRiverAppearFileName = @"FishAppearRiverParam.bin";
        private const string fishSeaAppearFileName = @"FishAppearSeaParam.bin";
        private const string CreatureSeaAppearFileName = @"CreatureAppearSeaParam.bin";
        private static byte[] InsectAppearParam = LoadBinaryFile(insectAppearFileName);
        private static byte[] FishRiverAppearParam = LoadBinaryFile(fishRiverAppearFileName);
        private static byte[] FishSeaAppearParam = LoadBinaryFile(fishSeaAppearFileName);
        private static byte[] CreatureSeaAppearParam = LoadBinaryFile(CreatureSeaAppearFileName);
        private int[] insectRate;
        private int[] riverFishRate;
        private int[] seaFishRate;
        private int[] seaCreatureRate;
        private DataGridView currentGridView;

        private bool offline = true;
        private bool AllowInventoryUpdate = true;
        private bool ChineseFlag;

        private int maxPage = 1;
        private int currentPage = 1;

        private string ChasingAddress = "";

        private static readonly Lock itemLock = new();
        private static readonly Lock villagerLock = new();

        private WaveOut waveOut;

        private static readonly int MaxHouseStorage = 9000;

        [GeneratedRegex(" ")]
        private static partial Regex Space();

        #endregion

        #region Form Load
        public Main()
        {
            InitializeComponent();

            IHost host = Host.CreateDefaultBuilder().ConfigureServices(services => services.AddMemoryCache()).Build();
            IMemoryCache cache = host.Services.GetRequiredService<IMemoryCache>();

            ImageCacher.Setup(cache);
        }



        private void Main_Load(object sender, EventArgs e)
        {
            hardwareId = libc.hwid.HwId.Generate();

            Text = version;

            IPAddressInputBox.Text = ConfigurationManager.AppSettings["ipAddress"];
            IPAddressInputBox.SelectionAlignment = HorizontalAlignment.Center;

            if (ConfigurationManager.AppSettings["override"] == "true")
            {
                overrideSetting = true;
                EasterEggButton.BackColor = Color.FromArgb(80, 80, 255);
            }
            if (ConfigurationManager.AppSettings["validation"] == "false")
            {
                validation = false;
            }

            if (ConfigurationManager.AppSettings["Sound"] == "false")
            {
                sound = false;
            }

            if (ConfigurationManager.AppSettings["Capture"] == "false")
            {
                capturesetting = false;
            }
            else
            {
                capturesetting = true;
            }
            if (ConfigurationManager.AppSettings["AutoRefill"] == "false")
            {
                autorefill = false;
            }
            else
            {
                autorefill = true;
            }
            setting = new Setting(overrideSetting, validation, sound, capturesetting);
            setting.ToggleOverride += Setting_toggleOverride;
            setting.ToggleValidation += Setting_toggleValidation;
            setting.ToggleSound += Setting_toggleSound;
            setting.ToggleCapture += Setting_toggleCapture;
            if (overrideSetting)
                setting.OverrideAddresses();

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\" + Utilities.imagePath) || ConfigurationManager.AppSettings["ForcedImageDownload"] == "true")
            {
                config.AppSettings.Settings["ForcedImageDownload"].Value = "false";
                config.Save(ConfigurationSaveMode.Minimal);

                ImgRetriever MoonMoon = new();
                MoonMoon.ShowDialog();
            }

            if (config.AppSettings.Settings["RestartRequired"].Value == "true")
            {
                config.AppSettings.Settings["RestartRequired"].Value = "false";
                config.Save(ConfigurationSaveMode.Minimal);
                Application.Restart();
            }

            if (File.Exists(Utilities.itemPath))
            {
                //load the csv
                itemSource = LoadItemCSV(Utilities.itemPath);
                ItemGridView.DataSource = itemSource;

                //set the ID row invisible
                ItemGridView.Columns["id"]!.Visible = false;
                ItemGridView.Columns["iName"]!.Visible = false;
                ItemGridView.Columns["jpn"]!.Visible = false;
                ItemGridView.Columns["tchi"]!.Visible = false;
                ItemGridView.Columns["schi"]!.Visible = false;
                ItemGridView.Columns["kor"]!.Visible = false;
                ItemGridView.Columns["fre"]!.Visible = false;
                ItemGridView.Columns["ger"]!.Visible = false;
                ItemGridView.Columns["spa"]!.Visible = false;
                ItemGridView.Columns["ita"]!.Visible = false;
                ItemGridView.Columns["dut"]!.Visible = false;
                ItemGridView.Columns["rus"]!.Visible = false;
                ItemGridView.Columns["color"]!.Visible = false;
                ItemGridView.Columns["size"]!.Visible = false;

                ItemGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                ItemGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
                ItemGridView.DefaultCellStyle.ForeColor = Color.FromArgb(255, 114, 105, 110);
                ItemGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                ItemGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 57, 60, 67);
                ItemGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                ItemGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                ItemGridView.EnableHeadersVisualStyles = false;
                ItemGridView.ShowCellToolTips = false;
                //create the image column
                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = @"Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                ItemGridView.Columns.Insert(13, imageColumn);
                imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                ItemGridView.Columns["eng"]!.Width = 195;
                ItemGridView.Columns["jpn"]!.Width = 195;
                ItemGridView.Columns["tchi"]!.Width = 195;
                ItemGridView.Columns["schi"]!.Width = 195;
                ItemGridView.Columns["kor"]!.Width = 195;
                ItemGridView.Columns["fre"]!.Width = 195;
                ItemGridView.Columns["ger"]!.Width = 195;
                ItemGridView.Columns["spa"]!.Width = 195;
                ItemGridView.Columns["ita"]!.Width = 195;
                ItemGridView.Columns["dut"]!.Width = 195;
                ItemGridView.Columns["rus"]!.Width = 195;
                ItemGridView.Columns["Image"]!.Width = 128;

                ItemGridView.Columns["eng"].HeaderText = @"Name";
                ItemGridView.Columns["jpn"].HeaderText = @"Name";
                ItemGridView.Columns["tchi"].HeaderText = @"Name";
                ItemGridView.Columns["schi"].HeaderText = @"Name";
                ItemGridView.Columns["kor"].HeaderText = @"Name";
                ItemGridView.Columns["fre"].HeaderText = @"Name";
                ItemGridView.Columns["ger"].HeaderText = @"Name";
                ItemGridView.Columns["spa"].HeaderText = @"Name";
                ItemGridView.Columns["ita"].HeaderText = @"Name";
                ItemGridView.Columns["dut"].HeaderText = @"Name";
                ItemGridView.Columns["rus"].HeaderText = @"Name";

                ItemGridView.DefaultCellStyle.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            }
            else
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                MyMessageBox.Show("[Warning] Missing items.csv file!", "Missing CSV file!\n" +
                                    "Please make sure you have extracted all the files from the downloaded archive before running the program!"
                                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (File.Exists(Utilities.overridePath))
            {
                OverrideDict = CreateOverride(Utilities.overridePath);
            }

            if (File.Exists(Utilities.recipePath))
            {
                recipeSource = LoadItemCSV(Utilities.recipePath);
                RecipeGridView.DataSource = recipeSource;

                RecipeGridView.Columns["id"]!.Visible = false;
                RecipeGridView.Columns["iName"]!.Visible = false;
                RecipeGridView.Columns["jpn"]!.Visible = false;
                RecipeGridView.Columns["tchi"]!.Visible = false;
                RecipeGridView.Columns["schi"]!.Visible = false;
                RecipeGridView.Columns["kor"]!.Visible = false;
                RecipeGridView.Columns["fre"]!.Visible = false;
                RecipeGridView.Columns["ger"]!.Visible = false;
                RecipeGridView.Columns["spa"]!.Visible = false;
                RecipeGridView.Columns["ita"]!.Visible = false;
                RecipeGridView.Columns["dut"]!.Visible = false;
                RecipeGridView.Columns["rus"]!.Visible = false;

                RecipeGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                RecipeGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
                RecipeGridView.DefaultCellStyle.ForeColor = Color.FromArgb(255, 114, 105, 110);
                RecipeGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                RecipeGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 57, 60, 67);
                RecipeGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                RecipeGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                RecipeGridView.EnableHeadersVisualStyles = false;

                DataGridViewImageColumn recipeImageColumn = new()
                {
                    Name = "Image",
                    HeaderText = @"Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };

                RecipeGridView.Columns.Insert(13, recipeImageColumn);
                recipeImageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                RecipeGridView.Columns["eng"]!.Width = 195;
                RecipeGridView.Columns["jpn"]!.Width = 195;
                RecipeGridView.Columns["tchi"]!.Width = 195;
                RecipeGridView.Columns["schi"]!.Width = 195;
                RecipeGridView.Columns["kor"]!.Width = 195;
                RecipeGridView.Columns["fre"]!.Width = 195;
                RecipeGridView.Columns["ger"]!.Width = 195;
                RecipeGridView.Columns["spa"]!.Width = 195;
                RecipeGridView.Columns["ita"]!.Width = 195;
                RecipeGridView.Columns["dut"]!.Width = 195;
                RecipeGridView.Columns["rus"]!.Width = 195;
                RecipeGridView.Columns["Image"]!.Width = 128;

                RecipeGridView.Columns["eng"].HeaderText = @"Name";
                RecipeGridView.Columns["jpn"].HeaderText = @"Name";
                RecipeGridView.Columns["tchi"].HeaderText = @"Name";
                RecipeGridView.Columns["schi"].HeaderText = @"Name";
                RecipeGridView.Columns["kor"].HeaderText = @"Name";
                RecipeGridView.Columns["fre"].HeaderText = @"Name";
                RecipeGridView.Columns["ger"].HeaderText = @"Name";
                RecipeGridView.Columns["spa"].HeaderText = @"Name";
                RecipeGridView.Columns["ita"].HeaderText = @"Name";
                RecipeGridView.Columns["dut"].HeaderText = @"Name";
                RecipeGridView.Columns["rus"].HeaderText = @"Name";

                RecipeGridView.DefaultCellStyle.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
                RecipeGridView.ShowCellToolTips = false;
            }
            else
            {
                RecipeModeButton.Visible = false;
                RecipeGridView.Visible = false;
            }

            if (File.Exists(Utilities.flowerPath))
            {
                flowerSource = LoadItemCSV(Utilities.flowerPath);
                FlowerGridView.DataSource = flowerSource;

                FlowerGridView.Columns["id"]!.Visible = false;
                FlowerGridView.Columns["iName"]!.Visible = false;
                FlowerGridView.Columns["jpn"]!.Visible = false;
                FlowerGridView.Columns["tchi"]!.Visible = false;
                FlowerGridView.Columns["schi"]!.Visible = false;
                FlowerGridView.Columns["kor"]!.Visible = false;
                FlowerGridView.Columns["fre"]!.Visible = false;
                FlowerGridView.Columns["ger"]!.Visible = false;
                FlowerGridView.Columns["spa"]!.Visible = false;
                FlowerGridView.Columns["ita"]!.Visible = false;
                FlowerGridView.Columns["dut"]!.Visible = false;
                FlowerGridView.Columns["rus"]!.Visible = false;
                FlowerGridView.Columns["value"]!.Visible = false;

                FlowerGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                FlowerGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
                FlowerGridView.DefaultCellStyle.ForeColor = Color.FromArgb(255, 114, 105, 110);
                FlowerGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                FlowerGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 57, 60, 67);
                FlowerGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                FlowerGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                FlowerGridView.EnableHeadersVisualStyles = false;

                DataGridViewImageColumn flowerImageColumn = new()
                {
                    Name = "Image",
                    HeaderText = @"Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };

                FlowerGridView.Columns.Insert(13, flowerImageColumn);
                flowerImageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                FlowerGridView.Columns["eng"]!.Width = 195;
                FlowerGridView.Columns["jpn"]!.Width = 195;
                FlowerGridView.Columns["tchi"]!.Width = 195;
                FlowerGridView.Columns["schi"]!.Width = 195;
                FlowerGridView.Columns["kor"]!.Width = 195;
                FlowerGridView.Columns["fre"]!.Width = 195;
                FlowerGridView.Columns["ger"]!.Width = 195;
                FlowerGridView.Columns["spa"]!.Width = 195;
                FlowerGridView.Columns["ita"]!.Width = 195;
                FlowerGridView.Columns["dut"]!.Width = 195;
                FlowerGridView.Columns["rus"]!.Width = 195;
                FlowerGridView.Columns["Image"]!.Width = 128;

                FlowerGridView.Columns["eng"].HeaderText = @"Name";
                FlowerGridView.Columns["jpn"].HeaderText = @"Name";
                FlowerGridView.Columns["tchi"].HeaderText = @"Name";
                FlowerGridView.Columns["schi"].HeaderText = @"Name";
                FlowerGridView.Columns["kor"].HeaderText = @"Name";
                FlowerGridView.Columns["fre"].HeaderText = @"Name";
                FlowerGridView.Columns["ger"].HeaderText = @"Name";
                FlowerGridView.Columns["spa"].HeaderText = @"Name";
                FlowerGridView.Columns["ita"].HeaderText = @"Name";
                FlowerGridView.Columns["dut"].HeaderText = @"Name";
                FlowerGridView.Columns["rus"].HeaderText = @"Name";

                FlowerGridView.DefaultCellStyle.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
                FlowerGridView.ShowCellToolTips = false;
            }
            else
            {
                FlowerModeButton.Visible = false;
                FlowerGridView.Visible = false;
            }

            if (File.Exists(Utilities.variationPath))
            {
                variationSource = LoadItemCSV(Utilities.variationPath);
            }
            else
            {
                VariationButton.Visible = false;
            }

            if (!File.Exists(Utilities.favPath))
            {
                string favHeader = "id" + " ; " + "iName" + " ; " + "Name" + " ; " + "value" + " ; ";

                string directoryPath = Directory.GetCurrentDirectory() + "\\" + Utilities.csvFolder;

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using StreamWriter sw = File.CreateText(Utilities.favPath);
                sw.WriteLine(favHeader);
            }

            if (File.Exists(Utilities.favPath))
            {
                favSource = LoadCSVwoKey(Utilities.favPath);
                FavGridView.DataSource = favSource;

                FavGridView.Columns["id"]!.Visible = false;
                FavGridView.Columns["iName"]!.Visible = false;
                FavGridView.Columns["value"]!.Visible = false;

                FavGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                FavGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
                FavGridView.DefaultCellStyle.ForeColor = Color.FromArgb(255, 114, 105, 110);
                FavGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                FavGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 57, 60, 67);
                FavGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                FavGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                FavGridView.EnableHeadersVisualStyles = false;

                DataGridViewImageColumn favImageColumn = new()
                {
                    Name = "Image",
                    HeaderText = @"Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                FavGridView.Columns.Insert(4, favImageColumn);
                favImageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                FavGridView.Columns["Name"].Width = 195;
                FavGridView.Columns["Image"].Width = 128;

                FavGridView.DefaultCellStyle.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
                FavGridView.ShowCellToolTips = false;
            }

            currentPanel = ItemModePanel;

            LanguageSetup(config.AppSettings.Settings["language"].Value);

            Utilities.BuildDictionary();

            KeyPreview = true;

            if (!DEBUGGING) return;

            OtherTabButton.Visible = true;
            CritterTabButton.Visible = true;
            MapDropperButton.Visible = true;
            //RegeneratorButton.Visible = true;
            FreezerButton.Visible = true;
            RoadRollerButton.Visible = true;
            DodoHelperButton.Visible = true;
            BulldozerButton.Visible = true;
            chatButton.Visible = true;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyLog.LogEvent("MainForm", "Form Closed");
        }

        private void Main_LocationChanged(object sender, EventArgs e)
        {
            if (selection != null)
            {
                selection.Location = new Point(Location.X + 7, Location.Y + Height);
            }
            if (Ch != null)
            {
                Ch.Location = new Point(Location.X + Width - Ch.Width - 7, Location.Y + Height);
            }
        }

        #endregion

        #region Setting
        private void Setting_toggleSound(bool soundOn)
        {
            sound = soundOn;
        }
        private void Setting_toggleCapture(bool CaptureOn)
        {
            capturesetting = CaptureOn;
        }

        private void Setting_toggleValidation()
        {
            validation = !validation;
        }

        private void Setting_toggleOverride()
        {
            if (overrideSetting)
            {
                overrideSetting = false;
                EasterEggButton.BackColor = Color.FromArgb(114, 137, 218);
            }
            else
            {
                overrideSetting = true;
                EasterEggButton.BackColor = Color.Red;
            }
        }

        private void SettingButton_Click(object sender, EventArgs e)
        {
            setting.ShowDialog();
        }

        #endregion

        #region Load CSV
        private static DataTable LoadItemCSV(string filePath)
        {
            var dt = new DataTable();

            File.ReadLines(filePath).Take(1)
                .SelectMany(x => x.Split([" ; "], StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            File.ReadLines(filePath).Skip(1)
                .Select(x => x.Split([" ; "], StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));

            if (dt.Columns.Contains("id"))
                dt.PrimaryKey = [dt.Columns["id"]];

            return dt;
        }

        private static DataTable LoadCSVwoKey(string filePath)
        {
            var dt = new DataTable();

            File.ReadLines(filePath).Take(1)
                .SelectMany(x => x.Split([" ; "], StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            File.ReadLines(filePath).Skip(1)
                .Select(x => x.Split([" ; "], StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));

            return dt;
        }

        private static byte[] LoadBinaryFile(string file)
        {
            return File.Exists(file) ? File.ReadAllBytes(file) : null;
        }
        private static Dictionary<string, string> CreateOverride(string path)
        {
            Dictionary<string, string> dict = [];

            if (!File.Exists(path)) return dict;

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string[] parts = line.Split([" ; "], StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    dict.Add(parts[1], parts[2]);
                }
            }

            return dict;
        }

        #endregion

        #region Images

        private void ItemGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= ItemGridView.Rows.Count) return;
            if (e.ColumnIndex != 13) return;
            if (ItemGridView.Rows[e.RowIndex] == null) return;

            string path;
            if (ItemGridView.Rows[e.RowIndex].Cells["iName"].Value == null)
                return;

            var value = ItemGridView.Rows[e.RowIndex].Cells["iName"].Value;
            if (value == null)
                return;

            string imageName = value.ToString();

            if (imageName != null && OverrideDict.TryGetValue(imageName, out string filename))
            {
                path = Utilities.imagePath + filename + ".png";
                if (File.Exists(path))
                {
                    Image img = ImageCacher.GetImage(path);
                    //e.CellStyle.BackColor = Color.Green;
                    e.Value = img;

                    return;
                }
            }

            path = Utilities.imagePath + imageName + "_Remake_0_0.png";
            if (File.Exists(path))
            {
                Image img = ImageCacher.GetImage(path);
                e.CellStyle.BackColor = Color.FromArgb(56, 77, 162);
                e.Value = img;
            }
            else
            {
                path = Utilities.imagePath + RemoveNumber(imageName) + ".png";
                if (File.Exists(path))
                {
                    Image img = ImageCacher.GetImage(path);
                    e.Value = img;
                }
                else
                {
                    path = Utilities.imagePath + imageName + ".png";
                    if (File.Exists(path))
                    {
                        Image img = ImageCacher.GetImage(path);
                        e.Value = img;
                    }
                    else
                    {
                        e.CellStyle.BackColor = Color.Red;
                    }
                }
            }
        }

        private static string RemoveNumber(string filename)
        {
            char[] myChar = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
            return filename.Trim(myChar);
        }

        private void ItemGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (lastRow != null)
                {
                    lastRow.Height = 22;
                }


                string hexValue = "0";

                if (e.RowIndex <= -1 || e.RowIndex >= ItemGridView.Rows.Count) return;

                lastRow = ItemGridView.Rows[e.RowIndex];
                ItemGridView.Rows[e.RowIndex].Height = 128;
                if (HexModeButton.Tag.ToString() == "Normal")
                {
                    if (AmountOrCountTextbox.Text is "" or @"0")
                    {
                        AmountOrCountTextbox.Text = @"1";
                    }

                    int decValue = Convert.ToInt32(AmountOrCountTextbox.Text) - 1;
                    if (decValue >= 0)
                        hexValue = decValue.ToString("X");
                }
                else
                {
                    if (AmountOrCountTextbox.Text is "" or @"0")
                    {
                        AmountOrCountTextbox.Text = Utilities.PrecedingZeros("0", 8);
                    }

                    hexValue = AmountOrCountTextbox.Text;
                    //HexModeButton_Click(sender, e);
                    //AmountOrCountTextbox.Text = "1";
                }


                string id = ItemGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();
                string name = ItemGridView.Rows[e.RowIndex].Cells[languageSetting].Value.ToString();

                IDTextbox.Text = id;

                UInt16 intId = Convert.ToUInt16(id, 16);

                string front = Utilities.PrecedingZeros(hexValue, 8).Substring(0, 4);
                string back = Utilities.PrecedingZeros(hexValue, 8).Substring(4, 4);

                if (IDTextbox.Text is "315A" or "1618" or "342F") // Wall-Mounted
                {
                    SelectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID(back, itemSource));
                }
                else if (IDTextbox.Text is "114A") // Money Tree
                {
                    SelectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID(back, itemSource));
                }
                else if (ItemAttr.HasFenceWithVariation(intId))  // Fence Variation
                {
                    SelectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, itemSource, Convert.ToUInt32("0x" + front, 16)), true, "");
                }
                else
                {
                    SelectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, "");
                }

                selection?.ReceiveID(Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4), languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (lastRow != null)
                {
                    lastRow.Height = 22;
                }

                if (e.RowIndex > -1 && e.RowIndex < ItemGridView.Rows.Count)
                {
                    lastRow = ItemGridView.Rows[e.RowIndex];
                    ItemGridView.Rows[e.RowIndex].Height = 128;

                    string name = SelectedItem.DisplayItemName();
                    string id = SelectedItem.DisplayItemID();
                    string path = SelectedItem.GetPath();

                    if (HexModeButton.Tag.ToString() == "Normal")
                    {
                        HexModeButton_Click(sender, e);
                    }
                    else
                    {

                    }

                    if (IDTextbox.Text != "")
                    {
                        if (IDTextbox.Text is "315A" or "1618" or "342F") // Wall-Mounted
                        {
                            AmountOrCountTextbox.Text = Utilities.PrecedingZeros(ItemGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), 8);
                            SelectedItem.Setup(name, Convert.ToUInt16(id, 16), Convert.ToUInt32("0x" + AmountOrCountTextbox.Text, 16), path, true, GetImagePathFromID(ItemGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), itemSource));
                        }
                        else if (IDTextbox.Text is "114A") // Money Tree
                        {
                            AmountOrCountTextbox.Text = @"0020" + Utilities.PrecedingZeros(ItemGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), 4);
                            SelectedItem.Setup(name, Convert.ToUInt16(id, 16), Convert.ToUInt32("0x" + AmountOrCountTextbox.Text, 16), path, true, GetImagePathFromID(ItemGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), itemSource));
                        }
                        else
                        {
                            AmountOrCountTextbox.Text = Utilities.PrecedingZeros(ItemGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), 8);
                            SelectedItem.Setup(name, Convert.ToUInt16(id, 16), Convert.ToUInt32("0x" + AmountOrCountTextbox.Text, 16), path, true, GetNameFromID(ItemGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), itemSource));
                        }

                        selection?.ReceiveID(Utilities.Turn2bytes(SelectedItem.FillItemData()), languageSetting);

                        UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
                    }

                }
            }
        }

        public static string GetImagePathFromID(string itemID, DataTable source, UInt32 data = 0)
        {
            if (source == null)
            {
                return "";
            }

            DataRow row = source.Rows.Find(itemID);
            DataRow varRow = null;
            if (variationSource != null)
                varRow = variationSource.Rows.Find(itemID);

            if (row == null)
            {
                return ""; //row not found
            }
            else
            {

                string path;
                if (varRow != null & source != recipeSource)
                {
                    string main = (data & 0xF).ToString();
                    string sub = (((data & 0xFF) - (data & 0xF)) / 0x20).ToString();
                    //Debug.Print("data " + data.ToString("X") + " Main " + main + " Sub " + sub);
                    path = Utilities.imagePath + varRow["iName"] + "_Remake_" + main + "_" + sub + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }

                    path = Utilities.imagePath + varRow["iName"] + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }

                string imageName = row[1].ToString();

                if (imageName != null && OverrideDict.TryGetValue(imageName, out string filename))
                {
                    path = Utilities.imagePath + filename + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }

                path = Utilities.imagePath + imageName + "_Remake_0_0.png";
                if (File.Exists(path))
                {
                    return path;
                }
                else
                {
                    path = Utilities.imagePath + imageName + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }
                    else
                    {
                        path = Utilities.imagePath + RemoveNumber(imageName) + ".png";
                        if (File.Exists(path))
                        {
                            return path;
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }
        }

        public static string GetImagePathFromID(string ID, UInt32 data, bool IsRecipe = false)
        {
            if (IsRecipe)
                return GetImagePathFromID(ID, recipeSource);
            else
                return GetImagePathFromID(ID, itemSource, data);
        }

        #endregion

        #region Get Item Info
        public static string GetNameFromID(string itemID, DataTable source)
        {
            if (source == null)
            {
                return "";
            }

            DataRow row = source.Rows.Find(itemID);

            if (row == null)
            {
                return ""; //row not found
            }
            else
            {
                //row found set the index and find the name
                return (string)row[languageSetting];
            }
        }

        public static string GetNameFromID(string itemID, bool IsRecipe = false)
        {
            if (!IsRecipe)
                return GetNameFromID(itemID, itemSource);
            else
                return GetNameFromID(itemID, recipeSource);
        }

        public void UpdateSelectedItemInfo(string itemName, string id, string data)
        {
            SelectedItemName.Text = itemName;
            selectedID.Text = id;
            selectedData.Text = data;
            selectedFlag0.Text = SelectedItem.GetFlag0();
            selectedFlag1.Text = SelectedItem.GetFlag1();
        }
        #endregion

        #region Island Name
        private string UpdateTownID()
        {
            if (socket == null && usb == null && Utilities.isEmulator == false)
                return "";

            MyLog.LogEvent("MainForm", "Reading Island Name :");

            byte[] townID = Utilities.GetTownID(socket, usb);
            IslandName = Utilities.GetString(townID, 0x04, 10);

            MyLog.LogEvent("MainForm", IslandName);

            return "  |  Island Name : " + IslandName;
        }

        #endregion

        #region Language
        private void LanguageSetup(string configLanguage)
        {
            LanguageSelector.SelectedIndex = configLanguage switch
            {
                "eng" => 0,
                "jpn" => 1,
                "tchi" => 2,
                "schi" => 3,
                "kor" => 4,
                "fre" => 5,
                "ger" => 6,
                "spa" => 7,
                "ita" => 8,
                "dut" => 9,
                "rus" => 10,
                _ => 0,
            };
        }

        private void LanguageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ItemSearchBox.Text != @"Search...")
            {
                ItemSearchBox.Clear();
            }

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            switch (LanguageSelector.SelectedIndex)
            {
                case 0:
                    languageSetting = "eng";
                    config.AppSettings.Settings["language"].Value = "eng";
                    break;
                case 1:
                    languageSetting = "jpn";
                    config.AppSettings.Settings["language"].Value = "jpn";
                    break;
                case 2:
                    languageSetting = "tchi";
                    config.AppSettings.Settings["language"].Value = "tchi";
                    break;
                case 3:
                    languageSetting = "schi";
                    config.AppSettings.Settings["language"].Value = "schi";
                    break;
                case 4:
                    languageSetting = "kor";
                    config.AppSettings.Settings["language"].Value = "kor";
                    break;
                case 5:
                    languageSetting = "fre";
                    config.AppSettings.Settings["language"].Value = "fre";
                    break;
                case 6:
                    languageSetting = "ger";
                    config.AppSettings.Settings["language"].Value = "ger";
                    break;
                case 7:
                    languageSetting = "spa";
                    config.AppSettings.Settings["language"].Value = "spa";
                    break;
                case 8:
                    languageSetting = "ita";
                    config.AppSettings.Settings["language"].Value = "ita";
                    break;
                case 9:
                    languageSetting = "dut";
                    config.AppSettings.Settings["language"].Value = "dut";
                    break;
                case 10:
                    languageSetting = "rus";
                    config.AppSettings.Settings["language"].Value = "rus";
                    break;
                default:
                    languageSetting = "eng";
                    config.AppSettings.Settings["language"].Value = "eng";
                    break;
            }

            config.Save(ConfigurationSaveMode.Minimal);

            if (ItemGridView.Columns.Contains(languageSetting))
            {
                HideAllLanguage();
                ItemGridView.Columns[languageSetting]!.Visible = true;
                RecipeGridView.Columns[languageSetting]!.Visible = true;
                FlowerGridView.Columns[languageSetting]!.Visible = true;
            }
        }

        private void HideAllLanguage()
        {
            if (ItemGridView.Columns.Contains("id"))
            {
                ItemGridView.Columns["eng"]!.Visible = false;
                ItemGridView.Columns["jpn"]!.Visible = false;
                ItemGridView.Columns["tchi"]!.Visible = false;
                ItemGridView.Columns["schi"]!.Visible = false;
                ItemGridView.Columns["kor"]!.Visible = false;
                ItemGridView.Columns["fre"]!.Visible = false;
                ItemGridView.Columns["ger"]!.Visible = false;
                ItemGridView.Columns["spa"]!.Visible = false;
                ItemGridView.Columns["ita"]!.Visible = false;
                ItemGridView.Columns["dut"]!.Visible = false;
                ItemGridView.Columns["rus"]!.Visible = false;
            }

            if (RecipeGridView.Columns.Contains("id"))
            {
                RecipeGridView.Columns["eng"]!.Visible = false;
                RecipeGridView.Columns["jpn"]!.Visible = false;
                RecipeGridView.Columns["tchi"]!.Visible = false;
                RecipeGridView.Columns["schi"]!.Visible = false;
                RecipeGridView.Columns["kor"]!.Visible = false;
                RecipeGridView.Columns["fre"]!.Visible = false;
                RecipeGridView.Columns["ger"]!.Visible = false;
                RecipeGridView.Columns["spa"]!.Visible = false;
                RecipeGridView.Columns["ita"]!.Visible = false;
                RecipeGridView.Columns["dut"]!.Visible = false;
                RecipeGridView.Columns["rus"]!.Visible = false;
            }

            if (FlowerGridView.Columns.Contains("id"))
            {
                FlowerGridView.Columns["eng"]!.Visible = false;
                FlowerGridView.Columns["jpn"]!.Visible = false;
                FlowerGridView.Columns["tchi"]!.Visible = false;
                FlowerGridView.Columns["schi"]!.Visible = false;
                FlowerGridView.Columns["kor"]!.Visible = false;
                FlowerGridView.Columns["fre"]!.Visible = false;
                FlowerGridView.Columns["ger"]!.Visible = false;
                FlowerGridView.Columns["spa"]!.Visible = false;
                FlowerGridView.Columns["ita"]!.Visible = false;
                FlowerGridView.Columns["dut"]!.Visible = false;
                FlowerGridView.Columns["rus"]!.Visible = false;
            }
        }
        #endregion

        #region Easter Egg
        private void EasterEggButton_Click(object sender, EventArgs e)
        {
            if (waveOut == null)
            {
                MyLog.LogEvent("MainForm", "EasterEgg Started");

                Thread songThread = new(Egg);
                songThread.Start();
            }
            else
            {
                MyLog.LogEvent("MainForm", "EasterEgg Stopped");
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
        }

        private void Egg()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            AudioFileReader audioFileReader = new(path + Utilities.villagerPath + "Io.nhv2");
            LoopStream loop = new(audioFileReader);
            waveOut = new WaveOut();
            waveOut.Init(loop);
            waveOut.Play();
        }

        #endregion

        #region Tab
        private void InventoryTabButton_Paint(object sender, PaintEventArgs e)
        {
            Font font = new("Arial", 10, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.White);
            e.Graphics.TranslateTransform(21, 7);
            e.Graphics.RotateTransform(90);
            e.Graphics.DrawString("Inventory", font, brush, 0, 0);
        }

        private void OtherTabButton_Paint(object sender, PaintEventArgs e)
        {
            Font font = new("Arial", 10, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.White);
            e.Graphics.TranslateTransform(21, 14);
            e.Graphics.RotateTransform(90);
            e.Graphics.DrawString("Other", font, brush, 0, 0);
        }

        private void CritterTabButton_Paint(object sender, PaintEventArgs e)
        {
            Font font = new("Arial", 10, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.White);
            e.Graphics.TranslateTransform(21, 14);
            e.Graphics.RotateTransform(90);
            e.Graphics.DrawString("Critter", font, brush, 0, 0);
        }

        private void VillagerTabButton_Paint(object sender, PaintEventArgs e)
        {
            Font font = new("Arial", 10, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.White);
            e.Graphics.TranslateTransform(21, 14);
            e.Graphics.RotateTransform(90);
            e.Graphics.DrawString("Villager", font, brush, 0, 0);
        }
        #endregion

        private void StartConnectionButton_Click(object sender, EventArgs e)
        {
            if (StartConnectionButton.Tag.ToString() == "connect")
            {
                if (connecting)
                    return;

                string ipPattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";

                if (!Regex.IsMatch(IPAddressInputBox.Text, ipPattern))
                {
                    IPAddressInputBackground.BackColor = Color.Orange;
                    return;
                }

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint ep = new(IPAddress.Parse(IPAddressInputBox.Text), 6000);

                if (socket.Connected == false)
                {
                    new Thread(() =>
                    {
                        connecting = true;

                        Thread.CurrentThread.IsBackground = true;

                        IAsyncResult result = socket.BeginConnect(ep, null, null);
                        bool conSucceded = result.AsyncWaitHandle.WaitOne(3000, true);


                        if (conSucceded)
                        {
                            try
                            {
                                socket.EndConnect(result);
                            }
                            catch
                            {
                                IPAddressInputBackground.Invoke((MethodInvoker)delegate
                                {
                                    MyLog.LogEvent("MainForm", "Connection Failed : " + IPAddressInputBox.Text);
                                    IPAddressInputBackground.BackColor = Color.Red;
                                });

                                if (MyMessageBox.Show("Sys-botbase not responding. Details?", "Error Code : 5318008 - Missing Sys-botbase Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                {
                                    MyMessageBox.Show("You have successfully started a connection!\n" +
                                                        "However, Sys-botbase is not responding...\n" +
                                                        " \n" +
                                                        "1) \n" +
                                                        "Check that your Switch is running in CFW mode.\n" +
                                                        "On your Switch, go to [ System Settings ] -> [ System ]\n" +
                                                        "Under [ System Update ], check [ Current version: ] and make sure you have [ AMS ] in it.\n" +
                                                        " \n" +
                                                        "2) \n" +
                                                        "Check that you are connecting to the correct IP address.\n" +
                                                        "On your Switch, go to [ System Settings ] -> [ Internet ]\n" +
                                                        "Check the [ IP Address ] under [ Connection Status ]\n" +
                                                        " \n" +
                                                        "3) \n" +
                                                        "Sys-botbase might have crashed.\n" +
                                                        "Please try holding down the power button and restart your Switch.\n" +
                                                        " \n" +
                                                        "4) \n" +
                                                        "Check that you have the latest version of Sys-botbase installed.\n" +
                                                        "You can get the latest version at \n        https://github.com/olliz0r/sys-botbase/releases \n" +
                                                        "Double-check your installation and make sure that the folder \n [ 430000000000000B ] can be located at [ SD: \\ atmosphere \\ contents \\ ] .\n" +
                                                        " \n" +
                                                        "5) \n" +
                                                        "When your Switch is booting up, \n" +
                                                        "Check that the LED of the [🏠 Home button] on your Joy-Con is lighting up.\n" +
                                                        " \n" +
                                                        "https://github.com/MyShiLingStar/ACNHPokerCore/wiki/Connection-Troubleshooting#where-are-you-my-socket-6000"
                                                        , "Where are you, my socket 6000?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }

                                connecting = false;
                                return;
                            }

                            Invoke((MethodInvoker)delegate
                            {
                                IPAddressInputBackground.BackColor = Color.Green;

                                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                                config.AppSettings.Settings["ipAddress"].Value = IPAddressInputBox.Text;
                                config.Save(ConfigurationSaveMode.Minimal);
                                if (config.AppSettings.Settings["autoRefresh"].Value == "true")
                                {
                                    InventoryAutoRefreshToggle.Checked = true;
                                }
                                else
                                {
                                    InventoryAutoRefreshToggle.Checked = false;
                                }

                                MyLog.LogEvent("MainForm", "Connection Succeeded : " + IPAddressInputBox.Text);

                                if (DataValidation())
                                {
                                    MyLog.LogEvent("MainForm", "Checking sys-botbase version");

                                    string sysbotbaseVersion = Utilities.CheckSysBotBase(socket, usb);

                                    string gameVersion = version.Split("v")[1];

                                    MyLog.LogEvent("MainForm", "sys-botbase version : " + sysbotbaseVersion);

                                    if (MyMessageBox.Show("Data validation failed. Details?", "Error Code : 71077345 - Data Validation Error!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                    {
                                        MyMessageBox.Show("You have successfully established a connection!\n" +
                                                    "However, data validation has failed!\n" +
                                                    " \n" +
                                                    "1) \n" +
                                                    "Check that you have booted the game up.\n" +
                                                    "The best place to start the connection is at the title screen.\n" +
                                                    " \n" +
                                                    "2) \n" +
                                                    "Check that you have the correct matching version.\n" +
                                                    "You are using [ " + version + " ] right now.\n" +
                                                    "You can find the latest version at : \n        https://github.com/MyShiLingStar/ACNHPokerCore \n" +
                                                    "Please update the game if your game version is below [ " + gameVersion + " ].\n" +
                                                    "You can find your game version at the top-right corner of the title screen.\n" +
                                                    " \n" +
                                                    "3) \n" +
                                                    "Please try holding down the power button and restart your Switch.\n" +
                                                    "Then press and HOLD the [ L button ] while you are selecting the game to boot up.\n" +
                                                    "Keep holding the [ L button ] and release it once you can see the title screen.\n" +
                                                    "Then retry the connection.\n" +
                                                    " \n" +
                                                    "4) \n" +
                                                    "Some installed Mods or sys-modules might conflict with Sys-botbase.\n" +
                                                    "Please try to remove or disable any unnecessary Mods.\n" +
                                                    " \n" +
                                                    "https://github.com/MyShiLingStar/ACNHPokerCore/wiki/Connection-Troubleshooting#sys-botbase-validation"
                                                    , "Sys-botbase Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }

                                    socket.Close();
                                    connecting = false;
                                    return;
                                }



                                RefreshButton.Visible = true;
                                PlayerInventorySelector.Visible = true;

                                InventoryAutoRefreshToggle.Visible = true;
                                AutoRefreshLabel.Visible = true;

                                AutoRefill.Visible = true;
                                AutoRefillLabel.Visible = true;

                                OtherTabButton.Visible = true;
                                CritterTabButton.Visible = true;
                                VillagerTabButton.Visible = true;

                                WrapSelector.SelectedIndex = 0;

                                StartConnectionButton.Tag = @"disconnect";
                                StartConnectionButton.Text = @"Disconnect";
                                USBConnectionButton.Visible = false;
                                SettingButton.Visible = false;
                                MapDropperButton.Visible = true;
                                //RegeneratorButton.Visible = true;
                                FreezerButton.Visible = true;
                                DodoHelperButton.Visible = true;
                                BulldozerButton.Visible = true;
                                RoadRollerButton.Visible = true;
                                chatButton.Visible = true;

                                offline = false;

                                CurrentPlayerIndex = UpdateDropdownBox();

                                PlayerInventorySelector.SelectedIndex = CurrentPlayerIndex;
                                PlayerInventorySelectorOther.SelectedIndex = CurrentPlayerIndex;
                                Text += UpdateTownID();

                                SetEatButton();
                                UpdateTurnipPrices();
                                ReadWeatherSeed();
                                ReadAirportColor();
                                ReadActivatedCheat();

                                currentGridView = InsectGridView;

                                MyLog.LogEvent("MainForm", "Loading Param Files");

                                LoadGridView(InsectAppearParam, InsectGridView, ref insectRate, Utilities.InsectDataSize, Utilities.InsectNumRecords);
                                LoadGridView(FishRiverAppearParam, RiverFishGridView, ref riverFishRate, Utilities.FishDataSize, Utilities.FishRiverNumRecords, 1);
                                LoadGridView(FishSeaAppearParam, SeaFishGridView, ref seaFishRate, Utilities.FishDataSize, Utilities.FishSeaNumRecords, 1);
                                LoadGridView(CreatureSeaAppearParam, SeaCreatureGridView, ref seaCreatureRate, Utilities.SeaCreatureDataSize, Utilities.SeaCreatureNumRecords, 1);

                                MyLog.LogEvent("MainForm", "Start Teleport and Controller");

                                Teleport.Init(socket);
                                Controller.Init(socket, IslandName);


                                if (Utilities.GetFreezeCount(socket) > 0)
                                {
                                    Utilities.SendString(socket, Utilities.UnFreeze(Utilities.ItemSlotBase));
                                    Utilities.SendString(socket, Utilities.UnFreeze(Utilities.ItemSlot21Base));
                                }

                                //auto refill 
                                if (autorefill)
                                {

                                    byte[] bank01To20 = Utilities.GetInventoryBank(socket, null, 1);
                                    byte[] bank21To40 = Utilities.GetInventoryBank(socket, null, 21);

                                    Utilities.SendString(socket, Utilities.Freeze(Utilities.ItemSlotBase, bank01To20));
                                    Utilities.SendString(socket, Utilities.Freeze(Utilities.ItemSlot21Base, bank21To40));

                                    //int freezeCount = Utilities.GetFreezeCount(socket);

                                    config.AppSettings.Settings["AutoRefill"].Value = "true";
                                    autorefill = true;
                                    AutoRefill.Checked = true;
                                }
                                else
                                {

                                    Utilities.SendString(socket, Utilities.UnFreeze(Utilities.ItemSlotBase));
                                    Utilities.SendString(socket, Utilities.UnFreeze(Utilities.ItemSlot21Base));

                                    //int freezeCount = Utilities.GetFreezeCount(socket);

                                    config.AppSettings.Settings["AutoRefill"].Value = "false";
                                    autorefill = false;
                                    AutoRefill.Checked = false;
                                }

                                init = false;
                            });

                            if (ModifierKeys == Keys.Shift)
                            {
                                if (!Directory.Exists(@"Debug\"))
                                    Directory.CreateDirectory(@"Debug\");

                                byte[] CollisionResult = Utilities.PeekMainAddress(socket, null, Utilities.CollisionAddress, 1000);
                                File.WriteAllBytes(@"Debug\Collision", CollisionResult);

                                byte[] TimeResult = Utilities.PeekMainAddress(socket, null, Utilities.freezeTimeAddress, 1000);
                                File.WriteAllBytes(@"Debug\Time", TimeResult);

                                string AddressResult = "Collision : " + Utilities.CollisionAddress.ToString("X") + "\n" +
                                                       "Time : " + Utilities.freezeTimeAddress.ToString("X");

                                if (overrideSetting)
                                {
                                    MyMessageBox.Show("[Warning] You have address override activated!" + "\n" + "\n" +
                                                      AddressResult, "Bytes Saved!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MyMessageBox.Show(AddressResult, "Bytes Saved!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }

                            MyLog.LogEvent("MainForm", "Data Reading Ended");
                        }
                        else
                        {
                            socket.Close();
                            socket = null;
                            connecting = false;
                            IPAddressInputBackground.Invoke((MethodInvoker)delegate
                            {
                                IPAddressInputBackground.BackColor = Color.Red;
                            });
                            MyMessageBox.Show("Unable to connect to the Sys-botbase server.\n" +
                                            "Please double check your IP address and Sys-botbase installation.\n" +
                                            " \n" +
                                            "You might also need to disable your firewall or antivirus temporary to allow outgoing connection."
                                            , "Unable to establish connection!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }).Start();
                }
            }
            else
            {
                socket.Close();
                connecting = false;
                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    btn.Reset();
                }

                RefreshButton.Visible = false;
                PlayerInventorySelector.Visible = false;

                InventoryAutoRefreshToggle.Visible = false;
                AutoRefreshLabel.Visible = false;

                AutoRefill.Visible = false;
                AutoRefillLabel.Visible = false;

                //this.USBConnectionButton.Visible = true;
                InventoryTabButton_Click(sender, e);
                OtherTabButton.Visible = false;
                CritterTabButton.Visible = false;
                VillagerTabButton.Visible = false;
                SettingButton.Visible = true;
                CleanVillagerPage();
                MapDropperButton.Visible = false;
                RegeneratorButton.Visible = false;
                FreezerButton.Visible = false;
                DodoHelperButton.Visible = false;
                BulldozerButton.Visible = false;
                RoadRollerButton.Visible = false;
                chatButton.Visible = false;
                if (Ch != null)
                {
                    Ch.Close();
                    Ch = null;
                }
                offline = true;

                StartConnectionButton.Tag = @"connect";
                StartConnectionButton.Text = @"Connect";
                USBConnectionButton.Visible = true;

                Text = version;
                HidePagination();
                init = true;
            }
        }

        #region Validation
        private Boolean DataValidation()
        {
            //return true;
            if (!validation)
            {
                MyLog.LogEvent("MainForm", "Skip Data Validation");
                return false;
            }

            try
            {
                MyLog.LogEvent("MainForm", "Start Data Validation");

                byte[] bank1 = Utilities.PeekAddress(socket, usb, Utilities.TownNameddress, 150); //TownNameddress
                byte[] bank2 = Utilities.PeekAddress(socket, usb, Utilities.TurnipPurchasePriceAddr, 150); //TurnipPurchasePriceAddr
                byte[] bank3 = Utilities.PeekAddress(socket, usb, Utilities.MasterRecyclingBase, 150); //MasterRecyclingBase
                byte[] bank4 = Utilities.PeekAddress(socket, usb, Utilities.playerReactionAddress, 150); //reactionAddress
                byte[] bank5 = Utilities.PeekAddress(socket, usb, Utilities.staminaAddress, 150); //staminaAddress

                string hexString1 = Utilities.ByteToHexString(bank1);
                string hexString2 = Utilities.ByteToHexString(bank2);
                string hexString3 = Utilities.ByteToHexString(bank3);
                string hexString4 = Utilities.ByteToHexString(bank4);
                string hexString5 = Utilities.ByteToHexString(bank5);

                MyLog.LogEvent("MainForm", "Data Validation : ");
                MyLog.LogEvent("MainForm", hexString1);
                MyLog.LogEvent("MainForm", hexString2);
                MyLog.LogEvent("MainForm", hexString3);
                MyLog.LogEvent("MainForm", hexString4);
                MyLog.LogEvent("MainForm", hexString5);

                Debug.Print(hexString1);
                Debug.Print(hexString2);
                Debug.Print(hexString3);
                Debug.Print(hexString4);
                Debug.Print(hexString5);

                int count1 = 0;
                if (hexString1 == hexString2)
                { count1++; }
                if (hexString1 == hexString3)
                { count1++; }
                if (hexString1 == hexString4)
                { count1++; }
                if (hexString1 == hexString5)
                { count1++; }

                int count2 = 0;
                if (hexString2 == hexString3)
                { count2++; }
                if (hexString2 == hexString4)
                { count2++; }
                if (hexString2 == hexString5)
                { count2++; }

                int count3 = 0;
                if (hexString3 == hexString4)
                { count3++; }
                if (hexString3 == hexString5)
                { count3++; }

                Debug.Print("Count : " + count1 + " " + count2 + " " + count3);
                if (count1 > 1 || count2 > 1 || count3 > 1)
                { return true; }
                else
                { return false; }
            }
            catch (Exception e)
            {
                MyMessageBox.Show(e.Message, "Todo : this is dumb");
                return false;
            }
        }

        #endregion

        #region Inventory Name
        private static string[] GetInventoryName()
        {
            string[] namelist = new string[8];
            //Debug.Print("Peek 8 Name:");
            byte[] tempHeader = null;
            Boolean headerFound = false;

            for (int i = 0; i < 8; i++)
            {
                byte[] b = Utilities.GetInventoryName(socket, usb, i);
                if (b == null)
                {
                    namelist[i] = "NULL";
                }
                else
                    namelist[i] = Encoding.Unicode.GetString(b, 32, 20);
                namelist[i] = namelist[i].Replace("\0", string.Empty);
                if (namelist[i].Equals(string.Empty) && !headerFound)
                {
                    header = tempHeader;
                    headerFound = true;
                }
                tempHeader = b;
            }
            return namelist;
        }

        public static byte[] GetHeader()
        {
            return header;
        }

        private int UpdateDropdownBox()
        {
            MyLog.LogEvent("MainForm", "Reading Player Name :");

            string[] namelist = GetInventoryName();
            int currentPlayer = 0;
            for (int i = 7; i >= 0; i--)
            {
                if (namelist[i] != string.Empty)
                {
                    MyLog.LogEvent("MainForm", namelist[i]);

                    PlayerInventorySelector.Items.RemoveAt(i);
                    PlayerInventorySelector.Items.Insert(i, namelist[i]);
                    PlayerInventorySelector.Items.RemoveAt(i + 8);
                    PlayerInventorySelector.Items.Insert(i + 8, namelist[i] + "'s House");

                    PlayerInventorySelectorOther.Items.RemoveAt(i);
                    PlayerInventorySelectorOther.Items.Insert(i, namelist[i]);
                    if (i > currentPlayer)
                        currentPlayer = i;
                }
            }
            return currentPlayer;
        }
        #endregion

        #region Auto Refresh
        private void AutoRefill_CheckedChanged(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            if (AutoRefill.Checked)
            {


                byte[] bank01To20 = Utilities.GetInventoryBank(socket, null, 1);
                byte[] bank21To40 = Utilities.GetInventoryBank(socket, null, 21);

                Utilities.SendString(socket, Utilities.Freeze(Utilities.ItemSlotBase, bank01To20));
                Utilities.SendString(socket, Utilities.Freeze(Utilities.ItemSlot21Base, bank21To40));

                //int freezeCount = Utilities.GetFreezeCount(socket);

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();

                config.AppSettings.Settings["AutoRefill"].Value = "true";
                autorefill = true;

            }
            else
            {

                Utilities.SendString(socket, Utilities.UnFreeze(Utilities.ItemSlotBase));
                Utilities.SendString(socket, Utilities.UnFreeze(Utilities.ItemSlot21Base));

                //int freezeCount = Utilities.GetFreezeCount(socket);


                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();

                config.AppSettings.Settings["AutoRefill"].Value = "false";
                autorefill = false;
            }

            config.Save(ConfigurationSaveMode.Minimal);

        }

        private void InventoryRefreshTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (socket is { Connected: true } && InventoryAutoRefreshToggle.Checked && AllowInventoryUpdate)
                    Invoke((MethodInvoker)delegate
                    {
                        UpdateInventory();
                    });
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("MainForm", "RefreshTimer: " + ex.Message);
                Invoke((MethodInvoker)delegate { InventoryAutoRefreshToggle.Checked = false; });
                InventoryRefreshTimer.Stop();
                MyMessageBox.Show("Lost connection to the Switch...\nDid the Switch go to sleep?", "Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InventoryAutoRefreshToggle_CheckedChanged(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            if (InventoryAutoRefreshToggle.Checked)
            {
                InventoryRefreshTimer.Start();
                config.AppSettings.Settings["autoRefresh"].Value = "true";
            }
            else
            {
                InventoryRefreshTimer.Stop();
                config.AppSettings.Settings["autoRefresh"].Value = "false";
            }

            config.Save(ConfigurationSaveMode.Minimal);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }
        #endregion

        #region Update Inventroy
        private void UpdateInventory()
        {
            AllowInventoryUpdate = false;

            try
            {
                byte[] bank01To20 = Utilities.GetInventoryBank(socket, usb, 1);
                if (bank01To20 == null)
                {
                    return;
                }
                byte[] bank21To40 = Utilities.GetInventoryBank(socket, usb, 21);
                if (bank21To40 == null)
                {
                    return;
                }
                //string Bank1 = Utilities.ByteToHexString(Bank01to20);
                //string Bank2 = Utilities.ByteToHexString(Bank21to40);

                //Debug.Print(Bank1);
                //Debug.Print(Bank2);

                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    if (btn.Tag == null)
                        continue;

                    if (btn.Tag.ToString() == "")
                        continue;

                    int slotId = int.Parse(btn.Tag.ToString());

                    byte[] slotBytes = new byte[2];
                    byte[] flag0Bytes = new byte[1];
                    byte[] flag1Bytes = new byte[1];
                    byte[] dataBytes = new byte[4];
                    byte[] recipeBytes = new byte[2];
                    byte[] fenceBytes = new byte[2];

                    int slotOffset;
                    int countOffset;
                    int flag0Offset;
                    int flag1Offset;
                    if (slotId < 21)
                    {
                        slotOffset = ((slotId - 1) * 0x8);
                        flag0Offset = 0x3 + ((slotId - 1) * 0x8);
                        flag1Offset = 0x2 + ((slotId - 1) * 0x8);
                        countOffset = 0x4 + ((slotId - 1) * 0x8);
                    }
                    else
                    {
                        slotOffset = ((slotId - 21) * 0x8);
                        flag0Offset = 0x3 + ((slotId - 21) * 0x8);
                        flag1Offset = 0x2 + ((slotId - 21) * 0x8);
                        countOffset = 0x4 + ((slotId - 21) * 0x8);
                    }

                    if (slotId < 21)
                    {
                        Buffer.BlockCopy(bank01To20, slotOffset, slotBytes, 0x0, 0x2);
                        Buffer.BlockCopy(bank01To20, flag0Offset, flag0Bytes, 0x0, 0x1);
                        Buffer.BlockCopy(bank01To20, flag1Offset, flag1Bytes, 0x0, 0x1);
                        Buffer.BlockCopy(bank01To20, countOffset, dataBytes, 0x0, 0x4);
                        Buffer.BlockCopy(bank01To20, countOffset, recipeBytes, 0x0, 0x2);
                        Buffer.BlockCopy(bank01To20, countOffset + 0x2, fenceBytes, 0x0, 0x2);
                    }
                    else
                    {
                        Buffer.BlockCopy(bank21To40, slotOffset, slotBytes, 0x0, 0x2);
                        Buffer.BlockCopy(bank21To40, flag0Offset, flag0Bytes, 0x0, 0x1);
                        Buffer.BlockCopy(bank21To40, flag1Offset, flag1Bytes, 0x0, 0x1);
                        Buffer.BlockCopy(bank21To40, countOffset, dataBytes, 0x0, 0x4);
                        Buffer.BlockCopy(bank21To40, countOffset, recipeBytes, 0x0, 0x2);
                        Buffer.BlockCopy(bank21To40, countOffset + 0x2, fenceBytes, 0x0, 0x2);
                    }

                    string itemId = Utilities.Flip(Utilities.ByteToHexString(slotBytes));
                    string itemData = Utilities.Flip(Utilities.ByteToHexString(dataBytes));
                    string recipeData = Utilities.Flip(Utilities.ByteToHexString(recipeBytes));
                    string fenceData = Utilities.Flip(Utilities.ByteToHexString(fenceBytes));
                    string flag0 = Utilities.ByteToHexString(flag0Bytes);
                    string flag1 = Utilities.ByteToHexString(flag1Bytes);
                    UInt16 intId = Convert.ToUInt16(itemId, 16);

                    //Debug.Print("Slot : " + slotId.ToString() + " ID : " + itemID + " Data : " + itemData + " recipeData : " + recipeData + " Flag0 : " + flag0 + " Flag1 : " + flag1);

                    if (itemId == "FFFE") //Nothing
                    {
                        btn.Setup("", 0xFFFE, 0x0, "", "00", "00");
                    }
                    else if (itemId == "16A2") //Recipe
                    {
                        btn.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                    }
                    else if (itemId == "1095") //Delivery
                    {
                        btn.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                    }
                    else if (itemId == "16A1") //Bottle Message
                    {
                        btn.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                    }
                    else if (itemId == "0A13") // Fossil
                    {
                        btn.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                    }
                    else if (itemId == "114A") // Money Tree
                    {
                        btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                    }
                    else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                    {
                        btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                    }
                    else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                    {
                        btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                    }
                    else
                    {
                        btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("MainForm", "UpdateInventory: " + ex.Message);
                Invoke((MethodInvoker)delegate { InventoryAutoRefreshToggle.Checked = false; });
                InventoryRefreshTimer.Stop();
                MyMessageBox.Show(ex.Message, "This seems like a bad idea but it's fine for now.");
                return;
            }

            AllowInventoryUpdate = true;
        }
        #endregion

        #region Fav
        private void DeletedSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = FavGridView.CurrentCell.RowIndex;
            if (FavGridView.CurrentCell != null)
            {
                //Debug.Print(index.ToString());

                string id = FavGridView.Rows[index].Cells["id"].Value.ToString();
                string iName = FavGridView.Rows[index].Cells["iName"].Value.ToString();
                string value = FavGridView.Rows[index].Cells["value"].Value.ToString();

                FileDeleteLine(Utilities.favPath, id, iName, value);

                FavGridView.Rows.RemoveAt(index);
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
        }

        private static void FileDeleteLine(string path, string key1, string key2, string key3)
        {
            StringBuilder sb = new();
            using (StreamReader sr = new(path))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line != null && (!line.Contains(key1) || !line.Contains(key2) || !line.Contains(key3)))
                    {
                        using StringWriter sw = new(sb);
                        sw.WriteLine(line);
                    }
                    else
                    {
                        //MessageBox.Show(line);
                    }
                }
            }
            using (StreamWriter sw = new(path))
            {
                sw.Write(sb.ToString());
            }
        }

        private void AddToFavoriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = (sender as ToolStripItem);
            if (item != null)
            {
                if (item.Owner is ContextMenuStrip owner)
                {
                    var btn = (InventorySlot)owner.SourceControl;
                    if (btn.FillItemID() == "FFFE")
                    {
                        return;
                    }
                    else
                    {
                        DataTable dt = (DataTable)FavGridView.DataSource;
                        DataRow dr = dt.NewRow();
                        dr["id"] = Utilities.Turn2bytes(btn.FillItemID());
                        dr["iName"] = btn.GetiName();
                        dr["Name"] = btn.DisplayItemName();
                        dr["value"] = Utilities.PrecedingZeros(btn.FillItemData(), 8);

                        dt.Rows.Add(dr);
                        FavGridView.DataSource = dt;

                        string line = dr["id"] + " ; " + dr["iName"] + " ; " + dr["Name"] + " ; " + dr["value"] + " ; ";

                        if (!File.Exists(Utilities.favPath))
                        {
                            string favheader = "id" + " ; " + "iName" + " ; " + "Name" + " ; " + "value" + " ; ";

                            using StreamWriter sw = File.CreateText(Utilities.favPath);
                            sw.WriteLine(favheader);
                        }

                        using (StreamWriter sw = File.AppendText(Utilities.favPath))
                        {
                            sw.WriteLine(line);
                        }

                    }
                    if (sound)
                        System.Media.SystemSounds.Asterisk.Play();
                }
            }
        }
        #endregion

        #region Variation
        private void OpenVariationMenu()
        {
            selection = new Variation();
            selection.SendVariationData += Selection_sendVariationData;
            selection.Show(this);
            selection.Location = new Point(Location.X + 7, Location.Y + Height);
            string id = Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4);
            string value = Utilities.PrecedingZeros(SelectedItem.FillItemData(), 8);
            UInt16 intId = Convert.ToUInt16(id, 16);
            if (ItemAttr.HasFenceWithVariation(intId))  // Fence Variation
            {
                selection.ReceiveID(Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4), languageSetting, value);
            }
            else if (id == "315A" || id == "1618" || id == "342F")
            {
                selection.ReceiveID(Utilities.Turn2bytes(SelectedItem.FillItemData()), languageSetting);
            }
            else
            {
                selection.ReceiveID(Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4), languageSetting);
            }
            VariationButton.BackColor = Color.FromArgb(80, 80, 255);
        }

        private void Selection_sendVariationData(InventorySlot item, int type)
        {
            if (type == 0) //Left click
            {
                SelectedItem.Setup(item);
                if (HexModeButton.Tag.ToString() == "Normal")
                {
                    HexModeButton_Click(null, null);
                }
                UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
                AmountOrCountTextbox.Text = Utilities.PrecedingZeros(SelectedItem.FillItemData(), 8);
                IDTextbox.Text = Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4);
            }
            else if (type == 1) // Right click
            {
                if (IDTextbox.Text is "315A" or "1618" or "342F")
                {
                    if (HexModeButton.Tag.ToString() == "Normal")
                    {
                        HexModeButton_Click(null, null);
                    }

                    string count = Utilities.TranslateVariationValue(item.FillItemData()) + Utilities.PrecedingZeros(item.FillItemID(), 4);

                    SelectedItem.Setup(GetNameFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + count, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), true, item.GetPath(), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                    AmountOrCountTextbox.Text = count;
                }
            }
        }

        private void CloseVariationMenu()
        {
            if (selection != null)
            {
                selection.Dispose();
                selection = null;
                VariationButton.BackColor = Color.FromArgb(114, 137, 218);
            }
        }

        private void VariationButton_Click(object sender, EventArgs e)
        {
            if (selection == null)
            {
                OpenVariationMenu();
            }
            else
            {
                CloseVariationMenu();
            }
        }
        #endregion

        #region Spawn, Delete, Copy

        private void SelectedItem_Click(object sender, EventArgs e)
        {
            if (selectedButton == null)
            {
                int slot = FindEmpty();
                if (slot > 0)
                {
                    selectedSlot = slot;
                    UpdateSlot(slot);
                }
            }

            if (currentPanel == ItemModePanel)
            {
                NormalItemSpawn();
            }
            else if (currentPanel == RecipeModePanel)
            {
                RecipeSpawn();
            }
            else if (currentPanel == FlowerModePanel)
            {
                FlowerSpawn();
            }
            int firstSlot = FindEmpty();
            if (firstSlot > 0)
            {
                selectedSlot = firstSlot;
                UpdateSlot(firstSlot);
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void UpdateSlot(int select)
        {
            foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
            {
                if (btn.Tag == null)
                    continue;
                btn.BackColor = Color.FromArgb(114, 137, 218);
                if (int.Parse(btn.Tag.ToString()) == select)
                {
                    selectedButton = btn;
                    btn.BackColor = Color.LightSeaGreen;
                }
            }
        }

        private int FindEmpty()
        {
            InventorySlot[] slotPointer = new InventorySlot[40];

            foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
            {
                int slotId = int.Parse(btn.Tag.ToString());
                slotPointer[slotId - 1] = btn;
            }
            for (int i = 0; i < slotPointer.Length; i++)
            {
                if (slotPointer[i].FillItemID() == "FFFE")
                    return i + 1;
            }

            return -1;
        }

        private void NormalItemSpawn()
        {
            if (IDTextbox.Text == "")
            {
                MessageBox.Show(@"Please enter an ID before sending item");
                return;
            }

            if (AmountOrCountTextbox.Text == "")
            {
                MessageBox.Show(@"Please enter an amount");
                return;
            }

            if (selectedButton == null)
            {
                MessageBox.Show(@"Please select a slot");
                return;
            }

            string hexValue = "00000000";
            if (HexModeButton.Tag.ToString() == "Normal")
            {
                int decValue = Convert.ToInt32(AmountOrCountTextbox.Text) - 1;
                if (decValue >= 0)
                    hexValue = Utilities.PrecedingZeros(decValue.ToString("X"), 8);
            }
            else
                hexValue = Utilities.PrecedingZeros(AmountOrCountTextbox.Text, 8);

            UInt16 intId = Convert.ToUInt16(IDTextbox.Text, 16);

            string front = Utilities.PrecedingZeros(hexValue, 8).Substring(0, 4);
            //string back = Utilities.precedingZeros(hexValue, 8).Substring(4, 4);

            try
            {
                if (!offline)
                    Utilities.SpawnItem(socket, usb, selectedSlot, SelectedItem.GetFlag0() + SelectedItem.GetFlag1() + IDTextbox.Text, Utilities.PrecedingZeros(hexValue, 8));

                if (IDTextbox.Text is "16A2") //recipe
                {
                    selectedButton.Setup(GetNameFromID(hexValue, recipeSource), 0x16A2, Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(hexValue), recipeSource));
                }
                else if (IDTextbox.Text is "114A") // Money Tree
                {
                    selectedButton.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), GetImagePathFromID(Utilities.Turn2bytes(hexValue), itemSource), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                }
                else if (IDTextbox.Text is "315A" or "1618" or "342F") // Wall-Mounted
                {
                    selectedButton.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(IDTextbox.Text, itemSource, Convert.ToUInt32("0x" + hexValue, 16)), GetImagePathFromID(Utilities.Turn2bytes(hexValue), itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16)), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                }
                else if (ItemAttr.HasFenceWithVariation(intId))  // Fence Variation
                {
                    selectedButton.Setup(GetNameFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + front, 16)), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                }
                else
                {
                    selectedButton.Setup(GetNameFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                }
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("MainForm", "SpawnItem: " + ex.Message);
                MyMessageBox.Show(ex.Message, "FIXME: This doesn't account for children of hierarchy... too bad!");
            }

            //this.ShowMessage(IDTextbox.Text);
        }

        private void RecipeSpawn()
        {
            if (RecipeIDTextbox.Text == "")
            {
                MessageBox.Show(@"Please enter a recipe ID before sending item");
                return;
            }

            if (selectedButton == null)
            {
                MessageBox.Show(@"Please select a slot");
                return;
            }

            if (!offline)
                Utilities.SpawnRecipe(socket, usb, selectedSlot, "16A2", Utilities.Turn2bytes(RecipeIDTextbox.Text));

            //this.ShowMessage(Utilities.turn2bytes(RecipeIDTextbox.Text));

            selectedButton.Setup(GetNameFromID(Utilities.Turn2bytes(RecipeIDTextbox.Text), recipeSource), 0x16A2, Convert.ToUInt32("0x" + RecipeIDTextbox.Text, 16), GetImagePathFromID(Utilities.Turn2bytes(RecipeIDTextbox.Text), recipeSource));
        }

        private void FlowerSpawn()
        {
            if (FlowerIDTextbox.Text == "")
            {
                MessageBox.Show(@"Please select a flower");
                return;
            }

            if (selectedButton == null)
            {
                MessageBox.Show(@"Please select a slot");
                return;
            }

            if (!offline)
                Utilities.SpawnFlower(socket, usb, selectedSlot, FlowerIDTextbox.Text, FlowerValueTextbox.Text);

            //this.ShowMessage(FlowerIDTextbox.Text);

            selectedButton.Setup(GetNameFromID(FlowerIDTextbox.Text, itemSource), Convert.ToUInt16("0x" + FlowerIDTextbox.Text, 16), Convert.ToUInt32("0x" + FlowerValueTextbox.Text, 16), GetImagePathFromID(FlowerIDTextbox.Text, itemSource));

        }

        private void DeleteItem()
        {
            if (selectedButton == null)
            {
                MessageBox.Show(@"Please select a slot");
                return;
            }

            if (!offline)
            {
                try
                {
                    Utilities.DeleteSlot(socket, usb, int.Parse(selectedButton.Tag.ToString()));
                }
                catch (Exception ex)
                {
                    MyLog.LogEvent("MainForm", "DeleteItemKeyBoard: " + ex.Message);
                    MyMessageBox.Show(ex.Message, "Because nobody could *ever* possible attempt to parse bad data.");
                }
            }
            selectedButton.Reset();
            ButtonToolTip.RemoveAll();

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void CopyItem(object sender, KeyEventArgs e)
        {
            ItemModeButton_Click(sender, e);
            if (HexModeButton.Tag.ToString() == "Normal")
            {
                HexModeButton_Click(sender, e);
            }

            string hexValue = "0";
            int decValue = Convert.ToInt32("0x" + AmountOrCountTextbox.Text, 16) - 1;
            if (decValue >= 0)
                hexValue = decValue.ToString("X");

            SelectedItem.Setup(selectedButton);
            selection?.ReceiveID(Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4), languageSetting, Utilities.PrecedingZeros(hexValue, 8));
            UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
            AmountOrCountTextbox.Text = Utilities.PrecedingZeros(SelectedItem.FillItemData(), 8);
            IDTextbox.Text = Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4);
        }

        private void InventorySlot_Click(object sender, EventArgs e)
        {
            var button = (InventorySlot)sender;

            foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
            {
                if (btn.Tag == null)
                    continue;
                btn.BackColor = Color.FromArgb(114, 137, 218);
            }

            button.BackColor = Color.LightSeaGreen;
            selectedButton = button;
            selectedSlot = int.Parse(button.Tag.ToString());

            if (ModifierKeys == Keys.Shift)
            {
                if (currentPanel == ItemModePanel)
                {
                    NormalItemSpawn();
                }
                else if (currentPanel == RecipeModePanel)
                {
                    RecipeSpawn();
                }
                else if (currentPanel == FlowerModePanel)
                {
                    FlowerSpawn();
                }

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            else if (ModifierKeys == Keys.Alt)
            {
                DeleteItem();
            }
        }

        private void CopyItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = (sender as ToolStripItem);
            if (item != null)
            {
                if (item.Owner is ContextMenuStrip owner)
                {
                    ItemModeButton_Click(sender, e);
                    if (HexModeButton.Tag.ToString() == "Normal")
                    {
                        HexModeButton_Click(sender, e);
                    }
                    var btn = (InventorySlot)owner.SourceControl;

                    SelectedItem.Setup(btn);

                    if (SelectedItem.FillItemID() == "FFFE")
                    {
                        HexModeButton_Click(sender, e);
                        AmountOrCountTextbox.Text = "";
                        IDTextbox.Text = "";
                    }
                    else
                    {
                        AmountOrCountTextbox.Text = Utilities.PrecedingZeros(SelectedItem.FillItemData(), 8);
                        IDTextbox.Text = Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4);
                    }

                    string hexValue = Utilities.PrecedingZeros(IDTextbox.Text, 8);

                    selection?.ReceiveID(Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4), languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                    UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
                    if (sound)
                        System.Media.SystemSounds.Asterisk.Play();
                }
            }
        }

        private void DeleteItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = (sender as ToolStripItem);
            if (item != null)
            {
                if (item.Owner is ContextMenuStrip owner)
                {
                    if (!offline)
                    {
                        int slotId = int.Parse(owner.SourceControl.Tag.ToString());
                        try
                        {
                            Utilities.DeleteSlot(socket, usb, slotId);
                        }
                        catch (Exception ex)
                        {
                            MyLog.LogEvent("MainForm", "DeleteItemRightClick: " + ex.Message);
                            MyMessageBox.Show(ex.Message, "Bizarre vector flip inherited from earlier code, WTF?");
                        }
                    }

                    var btnParent = (InventorySlot)owner.SourceControl;
                    btnParent.Reset();
                    ButtonToolTip.RemoveAll();
                    if (sound)
                        System.Media.SystemSounds.Asterisk.Play();
                }
            }
        }

        #endregion

        #region Wrap Item

        private void WrapItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WrapSelector.SelectedIndex < 0)
                WrapSelector.SelectedIndex = 0;

            string[] flagBuffer = WrapSelector.SelectedItem.ToString()?.Split(' ');
            byte flagByte = Utilities.StringToByte(flagBuffer?[flagBuffer.Length - 1])[0];

            ToolStripItem item = (sender as ToolStripItem);
            if (item != null)
            {
                if (item.Owner is ContextMenuStrip owner)
                {
                    string flag;
                    if (RetainNameToggle.Checked)
                    {
                        flag = Utilities.PrecedingZeros((flagByte + 0x40).ToString("X"), 2);
                    }
                    else
                    {
                        flag = Utilities.PrecedingZeros((flagByte).ToString("X"), 2);
                    }

                    if (!offline)
                    {
                        byte[] bank01To20 = Utilities.GetInventoryBank(socket, usb, 1);
                        byte[] bank21To40 = Utilities.GetInventoryBank(socket, usb, 21);

                        int slot = int.Parse(owner.SourceControl.Tag.ToString());
                        byte[] slotBytes = new byte[2];

                        int slotOffset;
                        if (slot < 21)
                        {
                            slotOffset = ((slot - 1) * 0x8);
                        }
                        else
                        {
                            slotOffset = ((slot - 21) * 0x8);
                        }

                        if (slot < 21)
                        {
                            Buffer.BlockCopy(bank01To20, slotOffset, slotBytes, 0x0, 0x2);
                        }
                        else
                        {
                            Buffer.BlockCopy(bank21To40, slotOffset, slotBytes, 0x0, 0x2);
                        }

                        string slotId = Utilities.Flip(Utilities.ByteToHexString(slotBytes));

                        if (slotId == "FFFE")
                        {
                            return;
                        }

                        Utilities.SetFlag1(socket, usb, slot, flag);

                        var btnParent = (InventorySlot)owner.SourceControl;
                        btnParent.SetFlag0(flag);
                        btnParent.Refresh(false);
                    }
                    else
                    {
                        var btnParent = (InventorySlot)owner.SourceControl;
                        if (btnParent.FillItemID() != "FFFE")
                        {
                            btnParent.SetFlag0(flag);
                            btnParent.Refresh(false);
                        }
                    }
                    if (sound)
                        System.Media.SystemSounds.Asterisk.Play();
                }
            }
        }

        private void WrapAllItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WrapSelector.SelectedIndex < 0)
                WrapSelector.SelectedIndex = 0;

            string[] flagBuffer = WrapSelector.SelectedItem.ToString()?.Split(' ');
            byte flagByte = Utilities.StringToByte(flagBuffer?[flagBuffer.Length - 1])[0];
            Thread wrapAllThread = new(delegate () { WrapAll(flagByte); });
            wrapAllThread.Start();
        }

        private void WrapAll(byte flagByte)
        {
            ShowWait();

            string flag;
            if (RetainNameToggle.Checked)
            {
                flag = Utilities.PrecedingZeros((flagByte + 0x40).ToString("X"), 2);
            }
            else
            {
                flag = Utilities.PrecedingZeros((flagByte).ToString("X"), 2);
            }

            if (!offline)
            {
                byte[] bank01To20 = Utilities.GetInventoryBank(socket, usb, 1);
                byte[] bank21To40 = Utilities.GetInventoryBank(socket, usb, 21);

                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    int slot = int.Parse(btn.Tag.ToString());
                    byte[] slotBytes = new byte[2];

                    int slotOffset;
                    if (slot < 21)
                    {
                        slotOffset = ((slot - 1) * 0x8);
                    }
                    else
                    {
                        slotOffset = ((slot - 21) * 0x8);
                    }

                    if (slot < 21)
                    {
                        Buffer.BlockCopy(bank01To20, slotOffset, slotBytes, 0x0, 0x2);
                    }
                    else
                    {
                        Buffer.BlockCopy(bank21To40, slotOffset, slotBytes, 0x0, 0x2);
                    }

                    string slotId = Utilities.Flip(Utilities.ByteToHexString(slotBytes));

                    if (slotId != "FFFE")
                    {
                        Utilities.SetFlag1(socket, usb, slot, flag);
                        Invoke((MethodInvoker)delegate
                        {
                            btn.SetFlag0(flag);
                            btn.Refresh(false);
                        });
                    }
                }
            }
            else
            {
                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    if (btn.FillItemID() != "FFFE")
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            btn.SetFlag0(flag);
                            btn.Refresh(false);
                        });
                    }
                }
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideWait();
        }

        #endregion

        #region Unwrap Item

        private void UnwrapAllItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread unwrapAllThread = new(UnwrapAll);
            unwrapAllThread.Start();
        }

        private void UnwrapAll()
        {
            ShowWait();

            if (!offline)
            {
                byte[] bank01To20 = Utilities.GetInventoryBank(socket, usb, 1);
                byte[] bank21To40 = Utilities.GetInventoryBank(socket, usb, 21);

                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    int slot = int.Parse(btn.Tag.ToString());
                    byte[] slotBytes = new byte[2];

                    int slotOffset;
                    if (slot < 21)
                    {
                        slotOffset = ((slot - 1) * 0x8);
                    }
                    else
                    {
                        slotOffset = ((slot - 21) * 0x8);
                    }

                    if (slot < 21)
                    {
                        Buffer.BlockCopy(bank01To20, slotOffset, slotBytes, 0x0, 0x2);
                    }
                    else
                    {
                        Buffer.BlockCopy(bank21To40, slotOffset, slotBytes, 0x0, 0x2);
                    }

                    string slotId = Utilities.Flip(Utilities.ByteToHexString(slotBytes));

                    if (slotId != "FFFE")
                    {
                        Utilities.SetFlag1(socket, usb, slot, "00");
                        Invoke((MethodInvoker)delegate
                        {
                            btn.SetFlag0("00");
                            btn.Refresh(false);
                        });
                    }
                }
            }
            else
            {
                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    if (btn.FillItemID() != "FFFE")
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            btn.SetFlag0("00");
                            btn.Refresh(false);
                        });
                    }
                }
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideWait();
        }

        #endregion

        #region Main Mode Change

        private void ItemModeButton_Click(object sender, EventArgs e)
        {
            ItemModeButton.BackColor = Color.FromArgb(80, 80, 255);
            RecipeModeButton.BackColor = Color.FromArgb(114, 137, 218);
            FlowerModeButton.BackColor = Color.FromArgb(114, 137, 218);
            FavoriteModeButton.BackColor = Color.FromArgb(114, 137, 218);

            RecipeModePanel.Visible = false;
            ItemModePanel.Visible = true;
            FlowerModePanel.Visible = false;

            ItemGridView.Visible = true;
            RecipeGridView.Visible = false;
            FlowerGridView.Visible = false;
            FavGridView.Visible = false;

            VariationButton.Visible = true;


            string hexValue = "00000000";
            if (HexModeButton.Tag.ToString() == "Normal")
            {
                if (AmountOrCountTextbox.Text == "")
                    AmountOrCountTextbox.Text = @"1";
                int decValue = Convert.ToInt32(AmountOrCountTextbox.Text) - 1;
                if (decValue >= 0)
                    hexValue = Utilities.PrecedingZeros(decValue.ToString("X"), 8);
            }
            else
            {
                if (AmountOrCountTextbox.Text == "")
                    AmountOrCountTextbox.Text = @"00000000";
                hexValue = Utilities.PrecedingZeros(AmountOrCountTextbox.Text, 8);
            }

            currentPanel = ItemModePanel;

            if (IDTextbox.Text == "")
                return;

            UInt16 IntId = Convert.ToUInt16("0x" + IDTextbox.Text, 16);
            string front = hexValue.Substring(0, 4);
            //string back = hexValue.Substring(4, 4);

            if (IDTextbox.Text != "")
            {
                if (IDTextbox.Text is "16A2") //recipe
                {
                    SelectedItem.Setup(GetNameFromID(hexValue, recipeSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(hexValue), recipeSource), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                }
                else if (IDTextbox.Text is "114A") // Money Tree
                {
                    SelectedItem.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID(Utilities.Turn2bytes(hexValue), itemSource), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                }
                else if (IDTextbox.Text is "315A" or "1618" or "342F") // Wall-Mounted
                {
                    SelectedItem.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(IDTextbox.Text, itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID((Utilities.Turn2bytes(hexValue)), itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16)), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                }
                else if (ItemAttr.HasFenceWithVariation(IntId)) // Fence Variation
                {
                    SelectedItem.Setup(GetNameFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + front, 16)), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());

                    selection?.ReceiveID(IDTextbox.Text, languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                }
                else
                {
                    SelectedItem.Setup(GetNameFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());

                    selection?.ReceiveID(IDTextbox.Text, languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                }
                UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
            }
            else
            {
                SelectedItem.Reset();
                SelectedItemName.Text = "";
            }

            if (ItemSearchBox.Text != @"Search...")
            {
                ItemSearchBox.Clear();
            }
        }

        private void RecipeModeButton_Click(object sender, EventArgs e)
        {
            ItemModeButton.BackColor = Color.FromArgb(114, 137, 218);
            RecipeModeButton.BackColor = Color.FromArgb(80, 80, 255);
            FlowerModeButton.BackColor = Color.FromArgb(114, 137, 218);
            FavoriteModeButton.BackColor = Color.FromArgb(114, 137, 218);

            ItemModePanel.Visible = false;
            RecipeModePanel.Visible = true;
            FlowerModePanel.Visible = false;

            ItemGridView.Visible = false;
            RecipeGridView.Visible = true;
            FlowerGridView.Visible = false;
            FavGridView.Visible = false;

            VariationButton.Visible = false;
            CloseVariationMenu();

            if (RecipeIDTextbox.Text != "")
            {
                //Debug.Print(GetNameFromID(RecipeIDTextbox.Text, recipeSource));
                SelectedItem.Setup(GetNameFromID(Utilities.Turn2bytes(RecipeIDTextbox.Text), recipeSource), 0x16A2, Convert.ToUInt32("0x" + RecipeIDTextbox.Text, 16), GetImagePathFromID(Utilities.Turn2bytes(RecipeIDTextbox.Text), recipeSource), true);
                UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
            }
            else
            {
                SelectedItem.Reset();
                SelectedItemName.Text = "";
            }

            currentPanel = RecipeModePanel;

            if (ItemSearchBox.Text != @"Search...")
            {
                ItemSearchBox.Clear();
            }
        }

        private void FlowerModeButton_Click(object sender, EventArgs e)
        {
            ItemModeButton.BackColor = Color.FromArgb(114, 137, 218);
            RecipeModeButton.BackColor = Color.FromArgb(114, 137, 218);
            FlowerModeButton.BackColor = Color.FromArgb(80, 80, 255);
            FavoriteModeButton.BackColor = Color.FromArgb(114, 137, 218);

            ItemModePanel.Visible = false;
            RecipeModePanel.Visible = false;
            FlowerModePanel.Visible = true;

            ItemGridView.Visible = false;
            RecipeGridView.Visible = false;
            FlowerGridView.Visible = true;
            FavGridView.Visible = false;

            VariationButton.Visible = false;
            CloseVariationMenu();

            if (FlowerIDTextbox.Text != "")
            {
                SelectedItem.Setup(GetNameFromID(FlowerIDTextbox.Text, itemSource), Convert.ToUInt16("0x" + FlowerIDTextbox.Text, 16), Convert.ToUInt32("0x" + FlowerValueTextbox.Text, 16), GetImagePathFromID(FlowerIDTextbox.Text, itemSource), true);
                UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
            }
            else
            {
                SelectedItem.Reset();
                SelectedItemName.Text = "";
            }

            currentPanel = FlowerModePanel;

            if (ItemSearchBox.Text != @"Search...")
            {
                ItemSearchBox.Clear();
            }
        }

        private void FavoriteModeButton_Click(object sender, EventArgs e)
        {
            ItemModeButton.BackColor = Color.FromArgb(114, 137, 218);
            RecipeModeButton.BackColor = Color.FromArgb(114, 137, 218);
            FlowerModeButton.BackColor = Color.FromArgb(114, 137, 218);
            FavoriteModeButton.BackColor = Color.FromArgb(80, 80, 255);

            RecipeModePanel.Visible = false;
            ItemModePanel.Visible = true;
            FlowerModePanel.Visible = false;

            ItemGridView.Visible = false;
            RecipeGridView.Visible = false;
            FlowerGridView.Visible = false;
            FavGridView.Visible = true;

            VariationButton.Visible = true;

            currentPanel = ItemModePanel;

            string hexValue = "00000000";
            if (HexModeButton.Tag.ToString() == "Normal")
            {
                if (AmountOrCountTextbox.Text == "")
                    AmountOrCountTextbox.Text = @"1";
                int decValue = Convert.ToInt32(AmountOrCountTextbox.Text) - 1;
                if (decValue >= 0)
                    hexValue = Utilities.PrecedingZeros(decValue.ToString("X"), 8);
            }
            else
            {
                if (AmountOrCountTextbox.Text == "")
                    AmountOrCountTextbox.Text = @"00000000";
                hexValue = Utilities.PrecedingZeros(AmountOrCountTextbox.Text, 8);
            }

            if (IDTextbox.Text == "")
                return;

            UInt16 intId = Convert.ToUInt16("0x" + IDTextbox.Text, 16);
            string front = hexValue.Substring(0, 4);
            //string back = hexValue.Substring(4, 4);

            if (IDTextbox.Text != "")
            {
                if (IDTextbox.Text is "16A2") //recipe
                {
                    SelectedItem.Setup(GetNameFromID(hexValue, recipeSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(hexValue), recipeSource), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                }
                else if (IDTextbox.Text is "114A") // Money Tree
                {
                    SelectedItem.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID(Utilities.Turn2bytes(hexValue), itemSource), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                }
                else if (IDTextbox.Text is "315A" or "1618" or "342F") // Wall-Mounted
                {
                    SelectedItem.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(IDTextbox.Text, itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID((Utilities.Turn2bytes(hexValue)), itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16)), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                }
                else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                {
                    SelectedItem.Setup(GetNameFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + front, 16)), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());

                    selection?.ReceiveID(IDTextbox.Text, languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                }
                else
                {
                    SelectedItem.Setup(GetNameFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());

                    selection?.ReceiveID(IDTextbox.Text, languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                }
                UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
            }
            else
            {
                SelectedItem.Reset();
                SelectedItemName.Text = "";
            }

            if (ItemSearchBox.Text != @"Search...")
            {
                ItemSearchBox.Clear();
            }
        }

        #endregion

        #region Progessbar
        private void ShowWait()
        {
            if (InvokeRequired)
            {
                MethodInvoker method = ShowWait;
                Invoke(method);
                return;
            }

            LoadingPanel.Visible = true;

            ItemModePanel.Visible = false;
            RecipeModePanel.Visible = false;
            FlowerModePanel.Visible = false;

            ItemModeButton.Visible = false;
            RecipeModeButton.Visible = false;
            FlowerModeButton.Visible = false;
            FavoriteModeButton.Visible = false;

            WrappingPanel.Visible = false;

            SelectedItem.Visible = false;
            SelectedItemName.Visible = false;

            ClearAllButton.Visible = false;
        }

        private void HideWait()
        {
            if (InvokeRequired)
            {
                MethodInvoker method = HideWait;
                Invoke(method);
                return;
            }

            LoadingPanel.Visible = false;
            currentPanel.Visible = true;

            ItemModeButton.Visible = true;
            RecipeModeButton.Visible = true;
            FlowerModeButton.Visible = true;
            FavoriteModeButton.Visible = true;

            WrappingPanel.Visible = true;

            SelectedItem.Visible = true;
            SelectedItemName.Visible = true;

            ClearAllButton.Visible = true;
        }
        #endregion

        #region Save/Load Item

        private void SaveNHIButton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog file = new()
                {
                    Filter = @"New Horizons Inventory (*.nhi)|*.nhi",
                    //FileName = "items.nhi",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                string savepath;

                if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
                else
                    savepath = config.AppSettings.Settings["LastSave"].Value;

                //Debug.Print(savepath);
                if (Directory.Exists(savepath))
                {
                    file.InitialDirectory = savepath;
                }
                else
                {
                    file.InitialDirectory = @"C:\";
                }

                if (file.ShowDialog() != DialogResult.OK)
                    return;

                string[] temp = file.FileName.Split('\\');
                string path = "";
                for (int i = 0; i < temp.Length - 1; i++)
                    path = path + temp[i] + "\\";

                config.AppSettings.Settings["LastSave"].Value = path;
                config.Save(ConfigurationSaveMode.Minimal);


                string bank = "";

                if (!offline)
                {
                    byte[] bank01To20 = Utilities.GetInventoryBank(socket, usb, 1);
                    byte[] bank21To40 = Utilities.GetInventoryBank(socket, usb, 21);
                    //bank = Utilities.ByteToHexString(bank01To20) + Utilities.ByteToHexString(bank21To40);


                    byte[] save = new byte[320];

                    Array.Copy(bank01To20, 0, save, 0, 160);
                    Array.Copy(bank21To40, 0, save, 160, 160);

                    File.WriteAllBytes(file.FileName, save);
                }
                else
                {
                    InventorySlot[] slotPointer = new InventorySlot[40];
                    foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                    {
                        int slotId = int.Parse(btn.Tag.ToString());
                        slotPointer[slotId - 1] = btn;
                    }
                    for (int i = 0; i < slotPointer.Length; i++)
                    {
                        string first = Utilities.Flip(Utilities.PrecedingZeros(slotPointer[i].GetFlag0() + slotPointer[i].GetFlag1() + Utilities.PrecedingZeros(slotPointer[i].FillItemID(), 4), 8));
                        string second = Utilities.Flip(Utilities.PrecedingZeros(slotPointer[i].FillItemData(), 8));
                        //Debug.Print(first + " " + second + " " + SlotPointer[i].getFlag0() + " " + SlotPointer[i].getFlag1() + " " + SlotPointer[i].fillItemID());
                        bank = bank + first + second;
                    }

                    byte[] save = new byte[320];

                    for (int i = 0; i < bank.Length / 2 - 1; i++)
                    {
                        string data = String.Concat(bank[(i * 2)].ToString(), bank[((i * 2) + 1)].ToString());
                        //Debug.Print(i.ToString() + " " + data);
                        save[i] = Convert.ToByte(data, 16);
                    }

                    File.WriteAllBytes(file.FileName, save);
                }
                //Debug.Print(Bank);
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch
            {
                // ignored
            }
        }

        private void LoadNHIButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new()
                {
                    Filter = @"New Horizons Inventory (*.nhi)|*.nhi|All files (*.*)|*.*",
                    FileName = "items.nhi",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                string savepath;

                if (config.AppSettings.Settings["LastLoad"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
                else
                    savepath = config.AppSettings.Settings["LastLoad"].Value;

                if (Directory.Exists(savepath))
                {
                    file.InitialDirectory = savepath;
                }
                else
                {
                    file.InitialDirectory = @"C:\";
                }

                if (file.ShowDialog() != DialogResult.OK)
                    return;

                string[] temp = file.FileName.Split('\\');
                string path = "";
                for (int i = 0; i < temp.Length - 1; i++)
                    path = path + temp[i] + "\\";

                config.AppSettings.Settings["LastLoad"].Value = path;
                config.Save(ConfigurationSaveMode.Minimal);

                byte[] data = File.ReadAllBytes(file.FileName);

                ButtonToolTip.RemoveAll();

                Thread loadThread = new(delegate () { LoadInventory(data); });
                loadThread.Start();
            }
            catch
            {
                // ignored
            }
        }

        private void LoadInventory(byte[] data)
        {
            ShowWait();

            byte[][] item = ProcessNHI(data);

            string bank = "";

            byte[] b1 = new byte[160];
            byte[] b2 = new byte[160];

            if (!offline)
            {
                byte[] bank01To20 = Utilities.GetInventoryBank(socket, usb, 1);
                byte[] bank21To40 = Utilities.GetInventoryBank(socket, usb, 21);

                byte[] currentInventory = new byte[320];

                Array.Copy(bank01To20, 0, currentInventory, 0, 160);
                Array.Copy(bank21To40, 0, currentInventory, 160, 160);

                int emptyspace = NumOfEmpty(currentInventory);

                if (emptyspace < item.Length)
                {
                    DialogResult dialogResult = MyMessageBox.Show("Empty Spaces in your inventory : " + emptyspace + "\n" +
                                                                "Number of items to Spawn : " + item.Length + "\n" +
                                                                "\n" +
                                                                "Press  [Yes]  to clear your inventory and spawn the items " + "\n" +
                                                                "or  [No]  to cancel the spawn." + "\n" + "\n" +
                                                                "[Warning] You will lose your items in your inventory!"
                                                                , "Not enough inventory spaces!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        for (int i = 0; i < b1.Length; i++)
                        {
                            b1[i] = data[i];
                            b2[i] = data[i + 160];
                        }

                        Utilities.OverwriteAll(socket, usb, b1, b2, ref counter);
                    }
                    else
                    {
                        HideWait();
                        if (sound)
                            System.Media.SystemSounds.Asterisk.Play();
                        return;
                    }
                }
                else
                {
                    b1 = bank01To20;
                    b2 = bank21To40;
                    FillInventory(ref b1, ref b2, item);

                    Utilities.OverwriteAll(socket, usb, b1, b2, ref counter);
                }
            }
            else
            {
                InventorySlot[] slotPointer = new InventorySlot[40];
                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    int slotId = int.Parse(btn.Tag.ToString());
                    slotPointer[slotId - 1] = btn;
                }
                for (int i = 0; i < slotPointer.Length; i++)
                {
                    string first = Utilities.Flip(Utilities.PrecedingZeros(slotPointer[i].GetFlag0() + slotPointer[i].GetFlag1() + Utilities.PrecedingZeros(slotPointer[i].FillItemID(), 4), 8));
                    string second = Utilities.Flip(Utilities.PrecedingZeros(slotPointer[i].FillItemData(), 8));
                    bank = bank + first + second;
                }

                byte[] currentInventory = new byte[320];

                for (int i = 0; i < bank.Length / 2 - 1; i++)
                {
                    string tempStr = String.Concat(bank[(i * 2)].ToString(), bank[((i * 2) + 1)].ToString());
                    //Debug.Print(i.ToString() + " " + data);
                    currentInventory[i] = Convert.ToByte(tempStr, 16);
                }

                int emptyspace = NumOfEmpty(currentInventory);

                if (emptyspace < item.Length)
                {
                    DialogResult dialogResult = MyMessageBox.Show("Empty Spaces in your inventory : " + emptyspace + "\n" +
                                                                "Number of items to Spawn : " + item.Length + "\n" +
                                                                "\n" +
                                                                "Press  [Yes]  to clear your inventory and spawn the new items " + "\n" +
                                                                "or  [No]  to cancel the spawn." + "\n" + "\n" +
                                                                "[Warning] You will lose your items in your inventory!"
                                                                , "Not enough inventory spaces!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        for (int i = 0; i < b1.Length; i++)
                        {
                            b1[i] = data[i];
                            b2[i] = data[i + 160];
                        }
                    }
                    else
                    {
                        HideWait();
                        if (sound)
                            System.Media.SystemSounds.Asterisk.Play();
                        return;
                    }
                }
                else
                {
                    Array.Copy(currentInventory, 0, b1, 0, 160);
                    Array.Copy(currentInventory, 160, b2, 0, 160);

                    FillInventory(ref b1, ref b2, item);
                }
            }

            byte[] newInventory = new byte[320];

            Array.Copy(b1, 0, newInventory, 0, 160);
            Array.Copy(b2, 0, newInventory, 160, 160);

            Invoke((MethodInvoker)delegate
            {
                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    if (btn.Tag == null)
                        continue;

                    if (btn.Tag.ToString() == "")
                        continue;

                    int slotId = int.Parse(btn.Tag.ToString());

                    byte[] slotBytes = new byte[2];
                    byte[] flag0Bytes = new byte[1];
                    byte[] flag1Bytes = new byte[1];
                    byte[] dataBytes = new byte[4];
                    byte[] recipeBytes = new byte[2];
                    byte[] fenceBytes = new byte[2];

                    int slotOffset = ((slotId - 1) * 0x8);
                    int flag0Offset = 0x3 + ((slotId - 1) * 0x8);
                    int flag1Offset = 0x2 + ((slotId - 1) * 0x8);
                    int countOffset = 0x4 + ((slotId - 1) * 0x8);

                    Buffer.BlockCopy(newInventory, slotOffset, slotBytes, 0, 2);
                    Buffer.BlockCopy(newInventory, flag0Offset, flag0Bytes, 0, 1);
                    Buffer.BlockCopy(newInventory, flag1Offset, flag1Bytes, 0, 1);
                    Buffer.BlockCopy(newInventory, countOffset, dataBytes, 0, 4);
                    Buffer.BlockCopy(newInventory, countOffset, recipeBytes, 0, 2);
                    Buffer.BlockCopy(newInventory, countOffset + 0x2, fenceBytes, 0x0, 0x2);

                    string itemId = Utilities.Flip(Utilities.ByteToHexString(slotBytes));
                    string itemData = Utilities.Flip(Utilities.ByteToHexString(dataBytes));
                    string recipeData = Utilities.Flip(Utilities.ByteToHexString(recipeBytes));
                    string fenceData = Utilities.Flip(Utilities.ByteToHexString(fenceBytes));
                    string flag0 = Utilities.ByteToHexString(flag0Bytes);
                    string flag1 = Utilities.ByteToHexString(flag1Bytes);
                    UInt16 intId = Convert.ToUInt16(itemId, 16);

                    //Debug.Print("Slot : " + slotId.ToString() + " ID : " + itemID + " Data : " + itemData + " recipeData : " + recipeData + " Flag0 : " + flag0 + " Flag1 : " + flag1);

                    if (itemId == "FFFE") //Nothing
                    {
                        btn.Setup("", 0xFFFE, 0x0, "", "00", "00");
                    }
                    else if (itemId == "16A2") //Recipe
                    {
                        btn.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                    }
                    else if (itemId == "1095") //Delivery
                    {
                        btn.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                    }
                    else if (itemId == "16A1") //Bottle Message
                    {
                        btn.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                    }
                    else if (itemId == "0A13") // Fossil
                    {
                        btn.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                    }
                    else if (itemId == "114A") // Money Tree
                    {
                        btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                    }
                    else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                    {
                        btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                    }
                    else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                    {
                        btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                    }
                    else
                    {
                        btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                    }
                }
            });

            HideWait();
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private static byte[][] ProcessNHI(byte[] data)
        {
            byte[] tempItem = new byte[8];
            bool[] isItem = new bool[40];
            int numOfitem = 0;

            for (int i = 0; i < 40; i++)
            {
                Buffer.BlockCopy(data, 0x8 * i, tempItem, 0, 8);
                if (!Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                {
                    isItem[i] = true;
                    numOfitem++;
                }
            }

            byte[][] item = new byte[numOfitem][];
            int itemNum = 0;
            for (int j = 0; j < 40; j++)
            {
                if (isItem[j])
                {
                    item[itemNum] = new byte[8];
                    Buffer.BlockCopy(data, 0x8 * j, item[itemNum], 0, 8);
                    itemNum++;
                }
            }

            return item;
        }

        private static int NumOfEmpty(byte[] data)
        {
            byte[] tempItem = new byte[8];
            int num = 0;

            for (int i = 0; i < 40; i++)
            {
                Buffer.BlockCopy(data, 0x8 * i, tempItem, 0, 8);
                if (Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                    num++;
            }
            return num;
        }

        private static void FillInventory(ref byte[] b1, ref byte[] b2, byte[][] item)
        {
            byte[] tempItem = new byte[8];
            int num = 0;

            for (int i = 0; i < 20; i++)
            {
                Buffer.BlockCopy(b1, 0x8 * i, tempItem, 0, 8);
                if (Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                {
                    Buffer.BlockCopy(item[num], 0, b1, 0x8 * i, 8);
                    num++;
                }
                if (num >= item.Length)
                    return;
            }

            for (int j = 0; j < 20; j++)
            {
                Buffer.BlockCopy(b2, 0x8 * j, tempItem, 0, 8);
                if (Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                {
                    Buffer.BlockCopy(item[num], 0, b2, 0x8 * j, 8);
                    num++;
                }
                if (num >= item.Length)
                    return;
            }
        }

        #endregion

        #region Fill/Clear

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            UpdateInventory();
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void FillRemainButton_Click(object sender, EventArgs e)
        {
            if (IDTextbox.Text == "")
            {
                MessageBox.Show(@"Please enter an ID before sending item");
                return;
            }

            if (AmountOrCountTextbox.Text == "")
            {
                MessageBox.Show(@"Please enter an amount");
                return;
            }

            string itemID = IDTextbox.Text;

            if (HexModeButton.Tag.ToString() == "Normal")
            {
                int decValue = Convert.ToInt32(AmountOrCountTextbox.Text) - 1;
                string itemAmount = decValue.ToString("X");
                Thread fillRemainThread = new(delegate () { FillRemain(itemID, itemAmount); });
                fillRemainThread.Start();
            }
            else
            {
                string itemAmount = AmountOrCountTextbox.Text;
                Thread fillRemainThread = new(delegate () { FillRemain(itemID, itemAmount); });
                fillRemainThread.Start();
            }
            //this.ShowMessage(IDTextbox.Text);
        }

        private void FillRemain(string itemId, string itemAmount)
        {
            lock (itemLock)
            {
                ShowWait();

                if (!offline)
                {
                    try
                    {
                        byte[] bank01To20 = Utilities.GetInventoryBank(socket, usb, 1);
                        byte[] bank21To40 = Utilities.GetInventoryBank(socket, usb, 21);

                        byte[] newItem = Utilities.Add(Utilities.StringToByte(Utilities.Flip(SelectedItem.GetFlag0() + SelectedItem.GetFlag1() + itemId)), Utilities.StringToByte(Utilities.Flip(Utilities.PrecedingZeros(itemAmount, 8))));

                        foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                        {
                            int slot = int.Parse(btn.Tag.ToString());
                            byte[] slotBytes = new byte[2];

                            int slotOffset;
                            if (slot < 21)
                            {
                                slotOffset = ((slot - 1) * 0x8);
                            }
                            else
                            {
                                slotOffset = ((slot - 21) * 0x8);
                            }

                            if (slot < 21)
                            {
                                Buffer.BlockCopy(bank01To20, slotOffset, slotBytes, 0x0, 0x2);
                            }
                            else
                            {
                                Buffer.BlockCopy(bank21To40, slotOffset, slotBytes, 0x0, 0x2);
                            }

                            string slotId = Utilities.Flip(Utilities.ByteToHexString(slotBytes));

                            if (slotId == "FFFE")
                            {

                                if (slot < 21)
                                {
                                    Buffer.BlockCopy(newItem, 0, bank01To20, (slot - 1) * 0x8, 0x8);
                                }
                                else
                                {
                                    Buffer.BlockCopy(newItem, 0, bank21To40, (slot - 21) * 0x8, 0x8);
                                }


                                Invoke((MethodInvoker)delegate
                                {
                                    if (itemId == "16A2") //Recipe
                                    {
                                        btn.Setup(GetNameFromID(Utilities.Turn2bytes(itemAmount), recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemAmount, 16), GetImagePathFromID(Utilities.Turn2bytes(itemAmount), recipeSource), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                                    }
                                    else
                                    {
                                        btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemAmount, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemAmount, 16)), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                                    }
                                });
                            }
                        }

                        Utilities.OverwriteAll(socket, usb, bank01To20, bank21To40, ref counter);

                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        MyLog.LogEvent("MainForm", "FillRemain: " + ex.Message);
                        MyMessageBox.Show(ex.Message, "This code didn't port easily. WTF does it do?");
                    }
                }
                else
                {
                    foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                    {
                        if (btn.FillItemID() == "FFFE")
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                if (itemId == "16A2") //Recipe
                                {
                                    btn.Setup(GetNameFromID(Utilities.Turn2bytes(itemAmount), recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemAmount, 16), GetImagePathFromID(Utilities.Turn2bytes(itemAmount), recipeSource), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                                }
                                else
                                {
                                    btn.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemAmount, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemAmount, 16)), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                                }
                            });
                        }
                    }
                }
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                HideWait();
            }
        }

        private void SpawnAllButton_Click(object sender, EventArgs e)
        {
            if (IDTextbox.Text == "")
            {
                MessageBox.Show(@"Please enter an ID before sending item");
                return;
            }

            if (AmountOrCountTextbox.Text == "")
            {
                MessageBox.Show(@"Please enter an amount");
                return;
            }

            string itemID = SelectedItem.GetFlag0() + SelectedItem.GetFlag1() + IDTextbox.Text;

            if (HexModeButton.Tag.ToString() == "Normal")
            {
                int decValue = Convert.ToInt32(AmountOrCountTextbox.Text) - 1;
                string itemAmount;
                if (decValue < 0)
                    itemAmount = "0";
                else
                    itemAmount = decValue.ToString("X");
                Thread spawnAllThread = new(delegate () { SpawnAll(itemID, itemAmount); });
                spawnAllThread.Start();
            }
            else
            {
                string itemAmount = AmountOrCountTextbox.Text;
                Thread spawnAllThread = new(delegate () { SpawnAll(itemID, itemAmount); });
                spawnAllThread.Start();
            }
            //this.ShowMessage(IDTextbox.Text);
        }

        private void SpawnAll(string itemId, string itemAmount)
        {
            ShowWait();

            if (!offline)
            {
                byte[] b = new byte[160];
                byte[] id = Utilities.StringToByte(Utilities.Flip(Utilities.PrecedingZeros(itemId, 8)));
                byte[] data = Utilities.StringToByte(Utilities.Flip(Utilities.PrecedingZeros(itemAmount, 8)));


                for (int i = 0; i < b.Length; i += 8)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        b[i + j] = id[j];
                        b[i + j + 4] = data[j];
                    }
                }

                try
                {
                    Utilities.OverwriteAll(socket, usb, b, b, ref counter);
                }
                catch (Exception ex)
                {
                    MyLog.LogEvent("MainForm", "SpawnAll: " + ex.Message);
                    MyMessageBox.Show(ex.Message, "Multithreading badness. This will cause a crash later!");
                }

                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    Invoke((MethodInvoker)delegate
                    {
                        if (Utilities.Turn2bytes(itemId) == "16A2") //Recipe
                        {
                            btn.Setup(GetNameFromID(Utilities.Turn2bytes(itemAmount), recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemAmount, 16), GetImagePathFromID(Utilities.Turn2bytes(itemAmount), recipeSource), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                        }
                        else
                        {
                            btn.Setup(GetNameFromID(Utilities.Turn2bytes(itemId), itemSource), Convert.ToUInt16("0x" + Utilities.Turn2bytes(itemId), 16), Convert.ToUInt32("0x" + itemAmount, 16), GetImagePathFromID(Utilities.Turn2bytes(itemId), itemSource, Convert.ToUInt32("0x" + itemAmount, 16)), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                        }
                    });
                }

                Thread.Sleep(1000);
            }
            else
            {
                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    Invoke((MethodInvoker)delegate
                    {
                        if (Utilities.Turn2bytes(itemId) == "16A2") //Recipe
                        {
                            btn.Setup(GetNameFromID(Utilities.Turn2bytes(itemAmount), recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemAmount, 16), GetImagePathFromID(Utilities.Turn2bytes(itemAmount), recipeSource), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                        }
                        else
                        {
                            btn.Setup(GetNameFromID(Utilities.Turn2bytes(itemId), itemSource), Convert.ToUInt16("0x" + Utilities.Turn2bytes(itemId), 16), Convert.ToUInt32("0x" + itemAmount, 16), GetImagePathFromID(Utilities.Turn2bytes(itemId), itemSource, Convert.ToUInt32("0x" + itemAmount, 16)), "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                        }
                    });
                }
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
            HideWait();
        }

        private void ClearAllButton_Click(object sender, EventArgs e)
        {
            Thread clearThread = new(ClearInventory);
            clearThread.Start();
        }

        private void ClearInventory()
        {
            ShowWait();

            try
            {
                if (!offline)
                {
                    byte[] b = new byte[160];

                    //Debug.Print(Utilities.precedingZeros(itemID, 8));
                    //Debug.Print(Utilities.precedingZeros(itemAmount, 8));

                    for (int i = 0; i < b.Length; i += 8)
                    {
                        b[i] = 0xFE;
                        b[i + 1] = 0xFF;
                        for (int j = 0; j < 6; j++)
                        {
                            b[i + 2 + j] = 0x00;
                        }
                    }

                    Utilities.OverwriteAll(socket, usb, b, b, ref counter);
                    //string result = Encoding.ASCII.GetString(Utilities.transform(b));
                    //Debug.Print(result);

                    foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            btn.Reset();
                        });
                    }
                    Invoke((MethodInvoker)delegate
                    {
                        ButtonToolTip.RemoveAll();
                    });

                    Thread.Sleep(1000);
                }
                else
                {
                    foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            btn.Reset();
                        });
                    }
                    Invoke((MethodInvoker)delegate
                    {
                        ButtonToolTip.RemoveAll();
                    });
                }
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("MainForm", "ClearInventory: " + ex.Message);
                MyMessageBox.Show(ex.Message, "This is catastrophically bad, don't do this. Someone needs to fix this.");
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
            HideWait();
        }

        #endregion

        #region Search    
        private void ItemSearchBox_TextChanged(object sender, EventArgs e)
        {
            if (ItemSearchBox.Text == @"Search...")
                return;
            try
            {
                if (ItemGridView.DataSource != null)
                    ((DataTable)ItemGridView.DataSource).DefaultView.RowFilter = string.Format(languageSetting + " LIKE '%{0}%'", EscapeLikeValue(ItemSearchBox.Text));
                if (RecipeGridView.DataSource != null)
                    ((DataTable)RecipeGridView.DataSource).DefaultView.RowFilter = string.Format(languageSetting + " LIKE '%{0}%'", EscapeLikeValue(ItemSearchBox.Text));
                if (FlowerGridView.DataSource != null)
                    ((DataTable)FlowerGridView.DataSource).DefaultView.RowFilter = string.Format(languageSetting + " LIKE '%{0}%'", EscapeLikeValue(ItemSearchBox.Text));
                if (FavGridView.DataSource != null)
                    ((DataTable)FavGridView.DataSource).DefaultView.RowFilter = string.Format("Name" + " LIKE '%{0}%'", EscapeLikeValue(ItemSearchBox.Text));

                ItemSearchBox.Font = new Font("Microsoft Sans Serif", 14F);
                ItemSearchBox.ForeColor = Color.White;
            }
            catch
            {
                ItemSearchBox.Clear();
            }
        }

        public static string EscapeLikeValue(string valueWithoutWildcards)
        {
            StringBuilder sb = new();
            for (int i = 0; i < valueWithoutWildcards.Length; i++)
            {
                char c = valueWithoutWildcards[i];
                if (c == '*' || c == '%' || c == '[' || c == ']')
                    sb.Append('[').Append(c).Append(']');
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private void ItemSearchBox_Click(object sender, EventArgs e)
        {
            if (ItemSearchBox.Text == @"Search...")
            {
                ItemSearchBox.Text = "";
                ItemSearchBox.ForeColor = Color.White;
            }
        }

        #endregion

        #region CellFormatting
        private void RecipeGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < RecipeGridView.Rows.Count)
            {
                if (e.ColumnIndex == 13)
                {
                    if (RecipeGridView.Rows[e.RowIndex] == null) return;

                    string imageName = RecipeGridView.Rows[e.RowIndex].Cells["iName"].Value.ToString();
                    string path;

                    if (imageName != null && OverrideDict.TryGetValue(imageName, out string filename))
                    {
                        path = Utilities.imagePath + filename + ".png";
                        if (File.Exists(path))
                        {
                            Image img = ImageCacher.GetImage(path);
                            //e.CellStyle.BackColor = Color.Green;
                            e.Value = img;

                            return;
                        }
                    }

                    path = Utilities.imagePath + imageName + ".png";
                    if (File.Exists(path))
                    {
                        Image img = ImageCacher.GetImage(path);
                        e.Value = img;
                    }
                    else
                    {
                        path = Utilities.imagePath + imageName + "_Remake_0_0.png";
                        if (File.Exists(path))
                        {
                            Image img = ImageCacher.GetImage(path);
                            e.CellStyle.BackColor = Color.FromArgb(56, 77, 162);
                            e.Value = img;
                        }
                        else
                        {
                            e.CellStyle.BackColor = Color.Red;
                        }
                    }
                }
            }
        }

        private void RecipeGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (recipelastRow != null)
            {
                recipelastRow.Height = 22;
            }

            if (e.RowIndex > -1 && e.RowIndex < RecipeGridView.Rows.Count)
            {
                recipelastRow = RecipeGridView.Rows[e.RowIndex];
                RecipeGridView.Rows[e.RowIndex].Height = 128;
                RecipeIDTextbox.Text = RecipeGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();

                SelectedItem.Setup(RecipeGridView.Rows[e.RowIndex].Cells[languageSetting].Value.ToString(), 0x16A2, Convert.ToUInt32("0x" + RecipeGridView.Rows[e.RowIndex].Cells["id"].Value, 16), GetImagePathFromID(RecipeGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), recipeSource), true);
                UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
            }
        }

        private void FlowerGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < FlowerGridView.Rows.Count)
            {
                if (e.ColumnIndex == 13)
                {
                    if (FlowerGridView.Rows[e.RowIndex] == null) return;

                    string imageName = FlowerGridView.Rows[e.RowIndex].Cells["iName"].Value.ToString();

                    if (imageName != null && OverrideDict.TryGetValue(imageName, out string filename))
                    {
                        string path = Utilities.imagePath + filename + ".png";
                        if (File.Exists(path))
                        {
                            Image img = ImageCacher.GetImage(path);
                            //e.CellStyle.BackColor = Color.Green;
                            e.Value = img;
                        }
                    }
                    else
                    {
                        e.CellStyle.BackColor = Color.Red;
                    }
                }
            }
        }

        private void FlowerGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (flowerlastRow != null)
            {
                flowerlastRow.Height = 22;
            }
            if (e.RowIndex > -1 && e.RowIndex < FlowerGridView.Rows.Count)
            {
                flowerlastRow = FlowerGridView.Rows[e.RowIndex];
                FlowerGridView.Rows[e.RowIndex].Height = 128;
                FlowerIDTextbox.Text = FlowerGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();
                FlowerValueTextbox.Text = FlowerGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();

                SelectedItem.Setup(FlowerGridView.Rows[e.RowIndex].Cells[languageSetting].Value.ToString(), Convert.ToUInt16("0x" + FlowerGridView.Rows[e.RowIndex].Cells["id"].Value, 16), Convert.ToUInt32("0x" + FlowerGridView.Rows[e.RowIndex].Cells["value"].Value, 16), GetImagePathFromID(FlowerGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), itemSource), true);
                UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
            }
        }

        private void FavGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < FavGridView.Rows.Count)
            {
                if (e.ColumnIndex == 4)
                {
                    if (FavGridView.Rows[e.RowIndex] == null) return;

                    string imageName = FavGridView.Rows[e.RowIndex].Cells["iName"].Value.ToString();
                    string path;

                    if (imageName != null && OverrideDict.TryGetValue(imageName, out string filename))
                    {
                        path = Utilities.imagePath + filename + ".png";
                        if (File.Exists(path))
                        {
                            Image img = ImageCacher.GetImage(path);
                            //e.CellStyle.BackColor = Color.Green;
                            e.Value = img;

                            return;
                        }
                    }

                    path = Utilities.imagePath + imageName + ".png";
                    if (File.Exists(path))
                    {
                        Image img = ImageCacher.GetImage(path);
                        e.Value = img;
                    }
                    else
                    {
                        path = Utilities.imagePath + imageName + "_Remake_0_0.png";
                        if (File.Exists(path))
                        {
                            Image img = ImageCacher.GetImage(path);
                            e.CellStyle.BackColor = Color.FromArgb(56, 77, 162);
                            e.Value = img;
                        }
                        else
                        {
                            path = Utilities.imagePath + RemoveNumber(imageName) + ".png";
                            if (File.Exists(path))
                            {
                                Image img = ImageCacher.GetImage(path);
                                e.Value = img;
                            }
                            else
                            {
                                e.CellStyle.BackColor = Color.Red;
                            }
                        }
                    }
                }
            }
        }

        private void FavGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (favlastRow != null)
                {
                    favlastRow.Height = 22;
                }
                if (e.RowIndex > -1 && e.RowIndex < FavGridView.Rows.Count)
                {
                    favlastRow = FavGridView.Rows[e.RowIndex];
                    FavGridView.Rows[e.RowIndex].Height = 128;
                    if (HexModeButton.Tag.ToString() == "Normal")
                    {
                        HexModeButton_Click(sender, e);
                    }

                    string id = FavGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();
                    //string iName = FavGridView.Rows[e.RowIndex].Cells["iName"].Value.ToString();
                    string name = FavGridView.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                    string data = FavGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();

                    AmountOrCountTextbox.Text = Utilities.PrecedingZeros(data, 8);
                    IDTextbox.Text = Utilities.PrecedingZeros(id, 4);

                    string hexValue = Utilities.PrecedingZeros(data, 8);
                    UInt16 intId = Convert.ToUInt16("0x" + id, 16);

                    string front = Utilities.PrecedingZeros(hexValue, 8).Substring(0, 4);
                    //string back = Utilities.precedingZeros(hexValue, 8).Substring(4, 4);

                    if (id == "16A2") //recipe
                    {
                        SelectedItem.Setup(GetNameFromID(hexValue, recipeSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(hexValue), recipeSource), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                    }
                    else if (id == "114A") // Money Tree
                    {
                        SelectedItem.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID(Utilities.Turn2bytes(hexValue), itemSource), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                    }
                    else if (id == "315A" || id == "1618" || id == "342F") // Wall-Mounted
                    {
                        SelectedItem.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(IDTextbox.Text, itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID((Utilities.Turn2bytes(hexValue)), itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16)), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
                    }
                    else if (ItemAttr.HasFenceWithVariation(intId))  // Fence Variation
                    {
                        SelectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + data, 16), GetImagePathFromID(id, itemSource, Convert.ToUInt32("0x" + front, 16)), true, "");
                    }
                    else
                    {
                        SelectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + data, 16), GetImagePathFromID(id, itemSource, Convert.ToUInt32("0x" + data, 16)), true, "");
                    }

                    selection?.ReceiveID(Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4), languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                    UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
                }
            }
        }

        #endregion

        #region Player Selector

        private void PlayerInventorySelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PlayerInventorySelector.SelectedIndex < 0)
                return;

            switch (PlayerInventorySelector.SelectedIndex)
            {
                case 0:
                    Utilities.SetAddress(1);
                    maxPage = 1;
                    currentPage = 1;
                    HidePagination();
                    UpdateInventory();
                    break;
                case 1:
                    Utilities.SetAddress(2);
                    maxPage = 1;
                    currentPage = 1;
                    HidePagination();
                    UpdateInventory();
                    break;
                case 2:
                    Utilities.SetAddress(3);
                    maxPage = 1;
                    currentPage = 1;
                    HidePagination();
                    UpdateInventory();
                    break;
                case 3:
                    Utilities.SetAddress(4);
                    maxPage = 1;
                    currentPage = 1;
                    HidePagination();
                    UpdateInventory();
                    break;
                case 4:
                    Utilities.SetAddress(5);
                    maxPage = 1;
                    currentPage = 1;
                    HidePagination();
                    UpdateInventory();
                    break;
                case 5:
                    Utilities.SetAddress(6);
                    maxPage = 1;
                    currentPage = 1;
                    HidePagination();
                    UpdateInventory();
                    break;
                case 6:
                    Utilities.SetAddress(7);
                    maxPage = 1;
                    currentPage = 1;
                    HidePagination();
                    UpdateInventory();
                    break;
                case 7:
                    Utilities.SetAddress(8);
                    maxPage = 1;
                    currentPage = 1;
                    HidePagination();
                    UpdateInventory();
                    break;
                case 8: // House
                    Utilities.SetAddress(11);
                    maxPage = MaxHouseStorage / 40;
                    currentPage = 1;
                    ShowPagination();
                    UpdateInventory();
                    break;
                case 9:
                    Utilities.SetAddress(12);
                    maxPage = MaxHouseStorage / 40;
                    currentPage = 1;
                    ShowPagination();
                    UpdateInventory();
                    break;
                case 10:
                    Utilities.SetAddress(13);
                    maxPage = MaxHouseStorage / 40;
                    currentPage = 1;
                    ShowPagination();
                    UpdateInventory();
                    break;
                case 11:
                    Utilities.SetAddress(14);
                    maxPage = MaxHouseStorage / 40;
                    currentPage = 1;
                    ShowPagination();
                    UpdateInventory();
                    break;
                case 12:
                    Utilities.SetAddress(15);
                    maxPage = MaxHouseStorage / 40;
                    currentPage = 1;
                    ShowPagination();
                    UpdateInventory();
                    break;
                case 13:
                    Utilities.SetAddress(16);
                    maxPage = MaxHouseStorage / 40;
                    currentPage = 1;
                    ShowPagination();
                    UpdateInventory();
                    break;
                case 14:
                    Utilities.SetAddress(17);
                    maxPage = MaxHouseStorage / 40;
                    currentPage = 1;
                    ShowPagination();
                    UpdateInventory();
                    break;
                case 15:
                    Utilities.SetAddress(18);
                    maxPage = MaxHouseStorage / 40;
                    currentPage = 1;
                    ShowPagination();
                    UpdateInventory();
                    break;
                case 16: // Re
                    Utilities.SetAddress(9);
                    maxPage = 2;
                    currentPage = 1;
                    ShowPagination();
                    UpdateInventory();
                    break;
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        #endregion

        #region Pagination

        private void FastBackButton_Click(object sender, EventArgs e)
        {
            if (currentPage == 1)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            if (PlayerInventorySelector.SelectedIndex == 16)
            {
                Utilities.GotoRecyclingPage(1);
                currentPage = 1;
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            else if (PlayerInventorySelector.SelectedIndex > 7)
            {
                if (currentPage - 10 > 1)
                {
                    Utilities.GotoHousePage((uint)(currentPage - 10), PlayerInventorySelector.SelectedIndex - 7);
                    currentPage -= 10;
                }
                else
                {
                    Utilities.GotoHousePage(1, PlayerInventorySelector.SelectedIndex - 7);
                    currentPage = 1;
                    if (sound)
                        System.Media.SystemSounds.Asterisk.Play();
                }
            }

            SetPageLabel();
            UpdateInventory();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                if (PlayerInventorySelector.SelectedIndex == 16)
                {
                    Utilities.GotoRecyclingPage((uint)(currentPage - 1));
                }
                else if (PlayerInventorySelector.SelectedIndex > 7)
                {
                    Utilities.GotoHousePage((uint)(currentPage - 1), PlayerInventorySelector.SelectedIndex - 7);
                }
                currentPage--;
                SetPageLabel();
                UpdateInventory();
            }
            else
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (currentPage < maxPage)
            {
                if (PlayerInventorySelector.SelectedIndex == 16)
                {
                    Utilities.GotoRecyclingPage((uint)(currentPage + 1));
                }
                else if (PlayerInventorySelector.SelectedIndex > 7)
                {
                    Utilities.GotoHousePage((uint)(currentPage + 1), PlayerInventorySelector.SelectedIndex - 7);
                }
                currentPage++;
                SetPageLabel();
                UpdateInventory();
            }
            else
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
        }

        private void FastNextButton_Click(object sender, EventArgs e)
        {
            if (PlayerInventorySelector.SelectedIndex == 16)
            {
                Utilities.GotoRecyclingPage((uint)maxPage);
                currentPage = maxPage;
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            else if (PlayerInventorySelector.SelectedIndex > 7)
            {
                if (currentPage + 10 <= maxPage)
                {
                    Utilities.GotoHousePage((uint)(currentPage + 10), PlayerInventorySelector.SelectedIndex - 7);
                    currentPage += 10;
                }
                else
                {
                    Utilities.GotoHousePage((uint)maxPage, PlayerInventorySelector.SelectedIndex - 7);
                    currentPage = maxPage;
                    if (sound)
                        System.Media.SystemSounds.Asterisk.Play();
                }
            }

            SetPageLabel();
            UpdateInventory();
        }

        private void SetPageLabel()
        {
            PageLabel.Text = @"Page " + currentPage;
        }

        private void ShowPagination()
        {
            SetPageLabel();
            PaginationPanel.Visible = true;
        }

        private void HidePagination()
        {
            SetPageLabel();
            PaginationPanel.Visible = false;
        }

        #endregion

        #region Gene
        private void SetGeneComboBox(string firstByte, string secondByte)
        {
            switch (firstByte)
            {
                case "0":
                    FlowerGeneW.SelectedIndex = 0;
                    FlowerGeneS.SelectedIndex = 0;
                    break;
                case "1":
                    FlowerGeneW.SelectedIndex = 1;
                    FlowerGeneS.SelectedIndex = 0;
                    break;
                case "3":
                    FlowerGeneW.SelectedIndex = 2;
                    FlowerGeneS.SelectedIndex = 0;
                    break;
                case "4":
                    FlowerGeneW.SelectedIndex = 0;
                    FlowerGeneS.SelectedIndex = 1;
                    break;
                case "5":
                    FlowerGeneW.SelectedIndex = 1;
                    FlowerGeneS.SelectedIndex = 1;
                    break;
                case "7":
                    FlowerGeneW.SelectedIndex = 2;
                    FlowerGeneS.SelectedIndex = 1;
                    break;
                case "C":
                    FlowerGeneW.SelectedIndex = 0;
                    FlowerGeneS.SelectedIndex = 2;
                    break;
                case "D":
                    FlowerGeneW.SelectedIndex = 1;
                    FlowerGeneS.SelectedIndex = 2;
                    break;
                case "F":
                    FlowerGeneW.SelectedIndex = 2;
                    FlowerGeneS.SelectedIndex = 2;
                    break;
                default:
                    FlowerGeneW.SelectedIndex = -1;
                    FlowerGeneS.SelectedIndex = -1;
                    break;
            }
            switch (secondByte)
            {
                case "0":
                    FlowerGeneR.SelectedIndex = 0;
                    FlowerGeneY.SelectedIndex = 0;
                    break;
                case "1":
                    FlowerGeneR.SelectedIndex = 1;
                    FlowerGeneY.SelectedIndex = 0;
                    break;
                case "3":
                    FlowerGeneR.SelectedIndex = 2;
                    FlowerGeneY.SelectedIndex = 0;
                    break;
                case "4":
                    FlowerGeneR.SelectedIndex = 0;
                    FlowerGeneY.SelectedIndex = 1;
                    break;
                case "5":
                    FlowerGeneR.SelectedIndex = 1;
                    FlowerGeneY.SelectedIndex = 1;
                    break;
                case "7":
                    FlowerGeneR.SelectedIndex = 2;
                    FlowerGeneY.SelectedIndex = 1;
                    break;
                case "C":
                    FlowerGeneR.SelectedIndex = 0;
                    FlowerGeneY.SelectedIndex = 2;
                    break;
                case "D":
                    FlowerGeneR.SelectedIndex = 1;
                    FlowerGeneY.SelectedIndex = 2;
                    break;
                case "F":
                    FlowerGeneR.SelectedIndex = 2;
                    FlowerGeneY.SelectedIndex = 2;
                    break;
                default:
                    FlowerGeneR.SelectedIndex = -1;
                    FlowerGeneY.SelectedIndex = -1;
                    break;
            }
        }

        private void GeneSelectionChangeCommitted(object sender, EventArgs e)
        {
            int R = FlowerGeneR.SelectedIndex;
            int Y = FlowerGeneY.SelectedIndex;
            int W = FlowerGeneW.SelectedIndex;
            int S = FlowerGeneS.SelectedIndex;

            SetGeneTextBox(R, Y, W, S);
        }

        private void SetGeneTextBox(int R, int Y, int W, int S)
        {
            string firstByte = W switch
            {
                0 => S switch
                {
                    0 => "0",
                    1 => "4",
                    2 => "C",
                    _ => "8",
                },
                1 => S switch
                {
                    0 => "1",
                    1 => "5",
                    2 => "D",
                    _ => "9",
                },
                2 => S switch
                {
                    0 => "3",
                    1 => "7",
                    2 => "F",
                    _ => "B",
                },
                _ => S switch
                {
                    0 => "2",
                    1 => "6",
                    2 => "E",
                    _ => "A",
                },
            };
            string secondByte = R switch
            {
                0 => Y switch
                {
                    0 => "0",
                    1 => "4",
                    2 => "C",
                    _ => "8",
                },
                1 => Y switch
                {
                    0 => "1",
                    1 => "5",
                    2 => "D",
                    _ => "9",
                },
                2 => Y switch
                {
                    0 => "3",
                    1 => "7",
                    2 => "F",
                    _ => "B",
                },
                _ => Y switch
                {
                    0 => "2",
                    1 => "6",
                    2 => "E",
                    _ => "A",
                },
            };
            //Debug.Print(firstByte + secondByte);
            if (HexModeButton.Tag.ToString() == "Normal")
            {
                HexModeButton_Click(null, null);
            }
            AmountOrCountTextbox.Text = string.Concat(AmountOrCountTextbox.Text.AsSpan(0, 6), firstByte, secondByte);
            SelectedItem.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + AmountOrCountTextbox.Text, 16), GetImagePathFromID(IDTextbox.Text, itemSource, Convert.ToUInt32("0x" + AmountOrCountTextbox.Text, 16)), true);
            UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
        }

        #endregion

        #region Reaction
        private void LoadReaction(int player = 0)
        {
            byte[] reactionBank = Utilities.GetReaction(socket, usb, player);
            Debug.Print(Utilities.ByteToHexString(reactionBank));

            byte[] reactions1 = new byte[1];
            byte[] reactions2 = new byte[1];
            byte[] reactions3 = new byte[1];
            byte[] reactions4 = new byte[1];
            byte[] reactions5 = new byte[1];
            byte[] reactions6 = new byte[1];
            byte[] reactions7 = new byte[1];
            byte[] reactions8 = new byte[1];

            if (reactionBank == null)
                return;
            Buffer.BlockCopy(reactionBank, 0, reactions1, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 1, reactions2, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 2, reactions3, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 3, reactions4, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 4, reactions5, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 5, reactions6, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 6, reactions7, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 7, reactions8, 0x0, 0x1);

            SetReactionBox(Utilities.ByteToHexString(reactions1), ReactionSlot1, ReactionSlot1.Items.Count);
            SetReactionBox(Utilities.ByteToHexString(reactions2), ReactionSlot2, ReactionSlot2.Items.Count);
            SetReactionBox(Utilities.ByteToHexString(reactions3), ReactionSlot3, ReactionSlot3.Items.Count);
            SetReactionBox(Utilities.ByteToHexString(reactions4), ReactionSlot4, ReactionSlot4.Items.Count);
            SetReactionBox(Utilities.ByteToHexString(reactions5), ReactionSlot5, ReactionSlot5.Items.Count);
            SetReactionBox(Utilities.ByteToHexString(reactions6), ReactionSlot6, ReactionSlot6.Items.Count);
            SetReactionBox(Utilities.ByteToHexString(reactions7), ReactionSlot7, ReactionSlot7.Items.Count);
            SetReactionBox(Utilities.ByteToHexString(reactions8), ReactionSlot8, ReactionSlot8.Items.Count);
        }

        private static void SetReactionBox(string reaction, ComboBox box, int max)
        {
            if (reaction == "00")
            {
                box.SelectedIndex = -1;
                return;
            }
            string hexValue = reaction;
            int decValue = Convert.ToInt32(hexValue, 16) - 1;
            if (decValue >= max)
                box.SelectedIndex = -1;
            else
                box.SelectedIndex = decValue;
        }

        private void PlayerInventorySelectorOther_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReaction(PlayerInventorySelectorOther.SelectedIndex);
        }

        private void SetReactionWheelButton_Click(object sender, EventArgs e)
        {
            int player = PlayerInventorySelectorOther.SelectedIndex;

            string reactionFirstHalf = (Utilities.PrecedingZeros((ReactionSlot1.SelectedIndex + 1).ToString("X"), 2) + Utilities.PrecedingZeros((ReactionSlot2.SelectedIndex + 1).ToString("X"), 2) + Utilities.PrecedingZeros((ReactionSlot3.SelectedIndex + 1).ToString("X"), 2) + Utilities.PrecedingZeros((ReactionSlot4.SelectedIndex + 1).ToString("X"), 2));
            string reactionSecondHalf = (Utilities.PrecedingZeros((ReactionSlot5.SelectedIndex + 1).ToString("X"), 2) + Utilities.PrecedingZeros((ReactionSlot6.SelectedIndex + 1).ToString("X"), 2) + Utilities.PrecedingZeros((ReactionSlot7.SelectedIndex + 1).ToString("X"), 2) + Utilities.PrecedingZeros((ReactionSlot8.SelectedIndex + 1).ToString("X"), 2));
            Utilities.SetReaction(socket, usb, player, reactionFirstHalf, reactionSecondHalf);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        #endregion

        #region Other Cheat

        private void SetEatButton()
        {
            EatButton.Visible = true;
            Random rnd = new();
            int dice = rnd.Next(1, 8);

            EatButton.Text = dice switch
            {
                1 => "Eat 10 Apples",
                2 => "Eat 10 Oranges",
                3 => "Eat 10 Cherries",
                4 => "Eat 10 Pears",
                5 => "Eat 10 Peaches",
                6 => "Eat 10 Coconuts",
                _ => "Eat 10 Turnips",
            };
        }

        private void ReadWeatherSeed()
        {
            MyLog.LogEvent("MainForm", "Reading Weather Seed :");

            byte[] b = Utilities.GetWeatherSeed(socket, usb);
            string result = Utilities.ByteToHexString(b);
            UInt32 decValue = Convert.ToUInt32(Utilities.Flip(result), 16);
            UInt32 seed = decValue - 2147483648;
            WeatherSeedTextbox.Text = seed.ToString();

            MyLog.LogEvent("MainForm", seed.ToString());
        }

        private void ReadAirportColor()
        {
            MyLog.LogEvent("MainForm", "Airport Color :");

            byte b = Utilities.GetAirportColor(socket, usb);
            int colorValue = Convert.ToInt32(b);
            if (colorValue < 0 || colorValue > 3)
            {
                colorValue = 0;
                MyLog.LogEvent("MainForm", "AirportColor Invalid");
            }
            else
            {
                AirportColor.SelectedIndex = colorValue;
                MyLog.LogEvent("MainForm", "AirportColor : " + colorValue.ToString());
            }
        }


        private void EatButton_Click(object sender, EventArgs e)
        {
            Utilities.SetStamina(socket, usb, "0A");

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            SetEatButton();
        }

        private void PoopButton_Click(object sender, EventArgs e)
        {
            Utilities.SetStamina(socket, usb, "00");

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void SetAirportColorBtn_Click(object sender, EventArgs e)
        {
            string selectedColor = AirportColor.SelectedIndex.ToString();

            Utilities.SetAirportColor(socket, usb, selectedColor);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private static void ButtonSelected(Button selected, Button[] NotSelected)
        {
            selected.BackColor = Color.FromArgb(80, 80, 255);
            foreach (Button b in NotSelected)
            {
                b.BackColor = Color.FromArgb(114, 137, 218);
            }
        }

        private void MaxSpeedX1Btn_Click(object sender, EventArgs e)
        {
            ButtonSelected(maxSpeedX1Btn, [maxSpeedX2Btn, maxSpeedX3Btn, maxSpeedX5Btn, maxSpeedX100Btn]);

            Utilities.SetMaxSpeed(socket, usb, Utilities.MaxSpeedX1);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void MaxSpeedX2Btn_Click(object sender, EventArgs e)
        {
            ButtonSelected(maxSpeedX2Btn, [maxSpeedX1Btn, maxSpeedX3Btn, maxSpeedX5Btn, maxSpeedX100Btn]);

            Utilities.SetMaxSpeed(socket, usb, Utilities.MaxSpeedX2);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void MaxSpeedX3Btn_Click(object sender, EventArgs e)
        {
            ButtonSelected(maxSpeedX3Btn, [maxSpeedX2Btn, maxSpeedX1Btn, maxSpeedX5Btn, maxSpeedX100Btn]);

            Utilities.SetMaxSpeed(socket, usb, Utilities.MaxSpeedX3);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void MaxSpeedX5Btn_Click(object sender, EventArgs e)
        {
            ButtonSelected(maxSpeedX5Btn, [maxSpeedX2Btn, maxSpeedX3Btn, maxSpeedX1Btn, maxSpeedX100Btn]);

            Utilities.SetMaxSpeed(socket, usb, Utilities.MaxSpeedX5);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void MaxSpeedX100Btn_Click(object sender, EventArgs e)
        {
            ButtonSelected(maxSpeedX100Btn, [maxSpeedX2Btn, maxSpeedX3Btn, maxSpeedX5Btn, maxSpeedX1Btn]);

            Utilities.SetMaxSpeed(socket, usb, Utilities.MaxSpeedX100);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void DisableCollisionToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            if (DisableCollisionToggle.Checked)
            {
                Utilities.PokeMainAddress(socket, usb, Utilities.CollisionAddress.ToString("X"), Utilities.CollisionDisable);
                Utilities.PokeMainAddress(socket, usb, Utilities.ActorCollisionAddress.ToString("X"), Utilities.ActorCollisionDisable);
            }
            else
            {
                Utilities.PokeMainAddress(socket, usb, Utilities.CollisionAddress.ToString("X"), Utilities.CollisionEnable);
                Utilities.PokeMainAddress(socket, usb, Utilities.ActorCollisionAddress.ToString("X"), Utilities.ActorCollisionEnable);
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void FastSwimToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            if (FastSwimToggle.Checked)
            {
                Utilities.SetFastSwimSpeed(socket, usb, true);
            }
            else
            {
                Utilities.SetFastSwimSpeed(socket, usb, false);
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void AnimationSpeedx50_Click(object sender, EventArgs e)
        {
            ButtonSelected(animationSpeedx50, [animationSpeedx1, animationSpeedx2, animationSpeedx0_1, animationSpeedx5]);

            Utilities.PokeMainAddress(socket, usb, Utilities.aSpeedAddress.ToString("X"), Utilities.aSpeedX50);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void AnimationSpeedx5_Click(object sender, EventArgs e)
        {
            ButtonSelected(animationSpeedx5, [animationSpeedx1, animationSpeedx2, animationSpeedx0_1, animationSpeedx50]);

            Utilities.PokeMainAddress(socket, usb, Utilities.aSpeedAddress.ToString("X"), Utilities.aSpeedX5);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void AnimationSpeedx2_Click(object sender, EventArgs e)
        {
            ButtonSelected(animationSpeedx2, [animationSpeedx1, animationSpeedx50, animationSpeedx0_1, animationSpeedx5]);

            Utilities.PokeMainAddress(socket, usb, Utilities.aSpeedAddress.ToString("X"), Utilities.aSpeedX2);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void AnimationSpeedx0_1_Click(object sender, EventArgs e)
        {
            ButtonSelected(animationSpeedx0_1, [animationSpeedx1, animationSpeedx2, animationSpeedx50, animationSpeedx5]);

            Utilities.PokeMainAddress(socket, usb, Utilities.aSpeedAddress.ToString("X"), Utilities.aSpeedX01);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void AnimationSpeedx1_Click(object sender, EventArgs e)
        {
            ButtonSelected(animationSpeedx1, [animationSpeedx50, animationSpeedx2, animationSpeedx0_1, animationSpeedx5]);

            Utilities.PokeMainAddress(socket, usb, Utilities.aSpeedAddress.ToString("X"), Utilities.aSpeedX1);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void ReadActivatedCheat()
        {
            string MaxSpeed = Utilities.ByteToHexString(Utilities.PeekAddress(socket, usb, Utilities.MaxSpeedAddress, 4));
            string SwimSpeed = Utilities.ByteToHexString(Utilities.PeekAddress(socket, usb, Utilities.SwimSpeed, 4));
            string Collision = Utilities.Flip(Utilities.ByteToHexString(Utilities.PeekMainAddress(socket, usb, Utilities.CollisionAddress, 4)));
            string AnimationSpeed = Utilities.Flip(Utilities.ByteToHexString(Utilities.PeekMainAddress(socket, usb, Utilities.aSpeedAddress, 4)));
            string Magic = Utilities.Flip(Utilities.ByteToHexString(Utilities.PeekMainAddress(socket, usb, Utilities.MagicAddress, 4)));

            if (MaxSpeed.Equals(Utilities.MaxSpeedX1))
            { }
            else if (MaxSpeed.Equals(Utilities.MaxSpeedX2))
            { ButtonSelected(maxSpeedX2Btn, [maxSpeedX1Btn, maxSpeedX3Btn, maxSpeedX5Btn, maxSpeedX100Btn]); }
            else if (MaxSpeed.Equals(Utilities.MaxSpeedX3))
            { ButtonSelected(maxSpeedX3Btn, [maxSpeedX2Btn, maxSpeedX1Btn, maxSpeedX5Btn, maxSpeedX100Btn]); }
            else if (MaxSpeed.Equals(Utilities.MaxSpeedX5))
            { ButtonSelected(maxSpeedX5Btn, [maxSpeedX2Btn, maxSpeedX3Btn, maxSpeedX1Btn, maxSpeedX100Btn]); }
            else if (MaxSpeed.Equals(Utilities.MaxSpeedX100))
            { ButtonSelected(maxSpeedX100Btn, [maxSpeedX2Btn, maxSpeedX3Btn, maxSpeedX5Btn, maxSpeedX1Btn]); }

            if (AnimationSpeed.Equals(Utilities.aSpeedX1))
            { }
            else if (AnimationSpeed.Equals(Utilities.aSpeedX2))
            { ButtonSelected(animationSpeedx2, [animationSpeedx50, animationSpeedx1, animationSpeedx0_1, animationSpeedx5]); }
            else if (AnimationSpeed.Equals(Utilities.aSpeedX5))
            { ButtonSelected(animationSpeedx5, [animationSpeedx50, animationSpeedx2, animationSpeedx0_1, animationSpeedx1]); }
            else if (AnimationSpeed.Equals(Utilities.aSpeedX50))
            { ButtonSelected(animationSpeedx50, [animationSpeedx1, animationSpeedx2, animationSpeedx0_1, animationSpeedx5]); }
            else if (AnimationSpeed.Equals(Utilities.aSpeedX01))
            { ButtonSelected(animationSpeedx0_1, [animationSpeedx50, animationSpeedx2, animationSpeedx1, animationSpeedx5]); }

            if (SwimSpeed.Equals(Utilities.FastSwimSpeed))
            { FastSwimToggle.Checked = true; }

            if (Collision.Equals(Utilities.CollisionDisable))
            { DisableCollisionToggle.Checked = true; }

            if (Magic.Equals(Utilities.MagicOn))
            { StarFragmentToggle.Checked = true; }
        }

        #endregion

        #region Za warudo

        private void FreezeTimeButton_Click(object sender, EventArgs e)
        {
            FreezeTimeButton.BackColor = Color.FromArgb(80, 80, 255);
            UnFreezeTimeButton.BackColor = Color.FromArgb(114, 137, 218);

            Utilities.PokeMainAddress(socket, usb, Utilities.freezeTimeAddress.ToString("X"), Utilities.freezeTimeValue);
            Readtime();
            DateAndTimeControlPanel.Visible = true;

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void UnFreezeTimeButton_Click(object sender, EventArgs e)
        {
            UnFreezeTimeButton.BackColor = Color.FromArgb(80, 80, 255);
            FreezeTimeButton.BackColor = Color.FromArgb(114, 137, 218);

            Utilities.PokeMainAddress(socket, usb, Utilities.freezeTimeAddress.ToString("X"), Utilities.unfreezeTimeValue);
            DateAndTimeControlPanel.Visible = false;
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void Readtime()
        {
            byte[] b = Utilities.PeekAddress(socket, usb, Utilities.readTimeAddress, 6);
            string time = Utilities.ByteToHexString(b);

            Debug.Print(time);

            Int32 year = Convert.ToInt32(Utilities.Flip(time.Substring(0, 4)), 16);
            Int32 month = Convert.ToInt32((time.Substring(4, 2)), 16);
            Int32 day = Convert.ToInt32((time.Substring(6, 2)), 16);
            Int32 hour = Convert.ToInt32((time.Substring(8, 2)), 16);
            Int32 min = Convert.ToInt32((time.Substring(10, 2)), 16);

            if (year > 3000 || month > 12 || day > 31 || hour > 24 || min > 60) //Try for Chineses
            {
                b = Utilities.PeekAddress(socket, usb, Utilities.readTimeAddress + Utilities.ChineseLanguageOffset, 6);
                time = Utilities.ByteToHexString(b);

                year = Convert.ToInt32(Utilities.Flip(time.Substring(0, 4)), 16);
                month = Convert.ToInt32((time.Substring(4, 2)), 16);
                day = Convert.ToInt32((time.Substring(6, 2)), 16);
                hour = Convert.ToInt32((time.Substring(8, 2)), 16);
                min = Convert.ToInt32((time.Substring(10, 2)), 16);

                if (!(year > 3000 || month > 12 || day > 31 || hour > 24 || min > 60))
                    ChineseFlag = true;
            }

            YearTextbox.Clear();
            YearTextbox.SelectionAlignment = HorizontalAlignment.Center;
            YearTextbox.Text = year.ToString();

            MonthTextbox.Clear();
            MonthTextbox.SelectionAlignment = HorizontalAlignment.Center;
            MonthTextbox.Text = month.ToString();

            DayTextbox.Clear();
            DayTextbox.SelectionAlignment = HorizontalAlignment.Center;
            DayTextbox.Text = day.ToString();

            HourTextbox.Clear();
            HourTextbox.SelectionAlignment = HorizontalAlignment.Center;
            HourTextbox.Text = hour.ToString();

            MinuteTextbox.Clear();
            MinuteTextbox.SelectionAlignment = HorizontalAlignment.Center;
            MinuteTextbox.Text = min.ToString();
        }

        private void SetDateAndTimeButton_Click(object sender, EventArgs e)
        {
            int decYear = int.Parse(YearTextbox.Text);
            if (decYear > 2060)
            {
                decYear = 2060;
            }
            else if (decYear < 1970)
            {
                decYear = 1970;
            }
            YearTextbox.Text = decYear.ToString();
            string hexYear = decYear.ToString("X");

            int decMonth = int.Parse(MonthTextbox.Text);
            if (decMonth > 12)
            {
                decMonth = 12;
            }
            else if (decMonth < 0)
            {
                decMonth = 1;
            }
            MonthTextbox.Text = decMonth.ToString();
            string hexMonth = decMonth.ToString("X");

            int decDay = int.Parse(DayTextbox.Text);
            if (decDay > 31)
            {
                decDay = 31;
            }
            else if (decDay < 0)
            {
                decDay = 1;
            }
            DayTextbox.Text = decDay.ToString();
            string hexDay = decDay.ToString("X");

            int decHour = int.Parse(HourTextbox.Text);
            if (decHour > 23)
            {
                decHour = 23;
            }
            else if (decHour < 0)
            {
                decHour = 0;
            }
            HourTextbox.Text = decHour.ToString();
            string hexHour = decHour.ToString("X");


            int decMin = int.Parse(MinuteTextbox.Text);
            if (decMin > 59)
            {
                decMin = 59;
            }
            else if (decMin < 0)
            {
                decMin = 0;
            }
            MinuteTextbox.Text = decMin.ToString();
            string hexMin = decMin.ToString("X");

            if (ChineseFlag)
            {
                Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.Flip(Utilities.PrecedingZeros(hexYear, 4)));
                Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x2 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.PrecedingZeros(hexMonth, 2) + Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2) + Utilities.PrecedingZeros(hexMin, 2));
            }
            else
            {
                Utilities.PokeAddress(socket, usb, Utilities.readTimeAddress.ToString("X"), Utilities.Flip(Utilities.PrecedingZeros(hexYear, 4)));
                Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x2).ToString("X"), Utilities.PrecedingZeros(hexMonth, 2) + Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2) + Utilities.PrecedingZeros(hexMin, 2));
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void Add1HourButton_Click(object sender, EventArgs e)
        {
            int decHour = int.Parse(HourTextbox.Text) + 1;
            if (decHour >= 24)
            {
                decHour = 0;
                string hexHour = decHour.ToString("X");

                int decDay = int.Parse(DayTextbox.Text) + 1;
                if (decDay > 31)
                {
                    decDay = 1;
                    string hexDay = decDay.ToString("X");

                    int decMonth = int.Parse(MonthTextbox.Text) + 1;
                    if (decMonth > 12)
                    {
                        decMonth = 1;
                        string hexMonth = decMonth.ToString("X");

                        int decYear = int.Parse(YearTextbox.Text) + 1;
                        string hexYear = decYear.ToString("X");

                        if (ChineseFlag)
                            Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.PrecedingZeros(hexYear, 4) + Utilities.PrecedingZeros(hexMonth, 2) + Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                        else
                            Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress).ToString("X"), Utilities.PrecedingZeros(hexYear, 4) + Utilities.PrecedingZeros(hexMonth, 2) + Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                    }
                    else
                    {
                        string hexMonth = decMonth.ToString("X");
                        if (ChineseFlag)
                            Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x2 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.PrecedingZeros(hexMonth, 2) + Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                        else
                            Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x2).ToString("X"), Utilities.PrecedingZeros(hexMonth, 2) + Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                    }
                }
                else
                {
                    string hexDay = decDay.ToString("X");
                    if (ChineseFlag)
                        Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x3 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                    else
                        Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x3).ToString("X"), Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                }
            }
            else
            {
                string hexHour = decHour.ToString("X");
                if (ChineseFlag)
                    Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x4 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.PrecedingZeros(hexHour, 2));
                else
                    Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x4).ToString("X"), Utilities.PrecedingZeros(hexHour, 2));
            }
            Readtime();
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void Minus1HourButton_Click(object sender, EventArgs e)
        {
            int decHour = int.Parse(HourTextbox.Text) - 1;
            if (decHour < 0)
            {
                decHour = 23;
                string hexHour = decHour.ToString("X");

                int decDay = int.Parse(DayTextbox.Text) - 1;
                if (decDay < 1)
                {
                    decDay = 28;
                    string hexDay = decDay.ToString("X");

                    int decMonth = int.Parse(MonthTextbox.Text) + 1;
                    if (decMonth < 1)
                    {
                        decMonth = 12;
                        string hexMonth = decMonth.ToString("X");

                        int decYear = int.Parse(YearTextbox.Text) + 1;
                        string hexYear = decYear.ToString("X");

                        if (ChineseFlag)
                            Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.PrecedingZeros(hexYear, 4) + Utilities.PrecedingZeros(hexMonth, 2) + Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                        else
                            Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress).ToString("X"), Utilities.PrecedingZeros(hexYear, 4) + Utilities.PrecedingZeros(hexMonth, 2) + Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                    }
                    else
                    {
                        string hexMonth = decMonth.ToString("X");
                        if (ChineseFlag)
                            Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x2 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.PrecedingZeros(hexMonth, 2) + Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                        else
                            Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x2).ToString("X"), Utilities.PrecedingZeros(hexMonth, 2) + Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                    }
                }
                else
                {
                    string hexDay = decDay.ToString("X");
                    if (ChineseFlag)
                        Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x3 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                    else
                        Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x3).ToString("X"), Utilities.PrecedingZeros(hexDay, 2) + Utilities.PrecedingZeros(hexHour, 2));
                }
            }
            else
            {
                string hexHour = decHour.ToString("X");
                if (ChineseFlag)
                    Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x4 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.PrecedingZeros(hexHour, 2));
                else
                    Utilities.PokeAddress(socket, usb, (Utilities.readTimeAddress + 0x4).ToString("X"), Utilities.PrecedingZeros(hexHour, 2));
            }
            Readtime();
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        #endregion

        #region Turnip

        private void UpdateTurnipPrices()
        {
            MyLog.LogEvent("MainForm", "Reading Turnip Prices :");

            ulong[] turnipPrices = Utilities.GetTurnipPrices(socket, usb);
            turnipBuyPrice.Clear();
            turnipBuyPrice.SelectionAlignment = HorizontalAlignment.Center;
            turnipBuyPrice.Text = $@"{turnipPrices[12]}";
            ulong buyPrice = ulong.Parse($"{turnipPrices[12]}");

            turnipSell1AM.Clear();
            turnipSell1AM.Text = $@"{turnipPrices[0]}";
            ulong MondayAM = ulong.Parse($"{turnipPrices[0]}");
            SetTurnipColor(buyPrice, MondayAM, turnipSell1AM);

            turnipSell1PM.Clear();
            turnipSell1PM.Text = $@"{turnipPrices[1]}";
            ulong MondayPM = ulong.Parse($"{turnipPrices[1]}");
            SetTurnipColor(buyPrice, MondayPM, turnipSell1PM);

            turnipSell2AM.Clear();
            turnipSell2AM.Text = $@"{turnipPrices[2]}";
            ulong TuesdayAM = ulong.Parse($"{turnipPrices[2]}");
            SetTurnipColor(buyPrice, TuesdayAM, turnipSell2AM);

            turnipSell2PM.Clear();
            turnipSell2PM.Text = $@"{turnipPrices[3]}";
            ulong TuesdayPM = ulong.Parse($"{turnipPrices[3]}");
            SetTurnipColor(buyPrice, TuesdayPM, turnipSell2PM);

            turnipSell3AM.Clear();
            turnipSell3AM.Text = $@"{turnipPrices[4]}";
            ulong WednesdayAM = ulong.Parse($"{turnipPrices[4]}");
            SetTurnipColor(buyPrice, WednesdayAM, turnipSell3AM);

            turnipSell3PM.Clear();
            turnipSell3PM.Text = $@"{turnipPrices[5]}";
            ulong WednesdayPM = ulong.Parse($"{turnipPrices[5]}");
            SetTurnipColor(buyPrice, WednesdayPM, turnipSell3PM);

            turnipSell4AM.Clear();
            turnipSell4AM.Text = $@"{turnipPrices[6]}";
            ulong ThursdayAM = ulong.Parse($"{turnipPrices[6]}");
            SetTurnipColor(buyPrice, ThursdayAM, turnipSell4AM);

            turnipSell4PM.Clear();
            turnipSell4PM.Text = $@"{turnipPrices[7]}";
            ulong ThursdayPM = ulong.Parse($"{turnipPrices[7]}");
            SetTurnipColor(buyPrice, ThursdayPM, turnipSell4PM);

            turnipSell5AM.Clear();
            turnipSell5AM.Text = $@"{turnipPrices[8]}";
            ulong FridayAM = ulong.Parse($"{turnipPrices[8]}");
            SetTurnipColor(buyPrice, FridayAM, turnipSell5AM);

            turnipSell5PM.Clear();
            turnipSell5PM.Text = $@"{turnipPrices[9]}";
            ulong FridayPM = ulong.Parse($"{turnipPrices[9]}");
            SetTurnipColor(buyPrice, FridayPM, turnipSell5PM);

            turnipSell6AM.Clear();
            turnipSell6AM.Text = $@"{turnipPrices[10]}";
            ulong SaturdayAM = ulong.Parse($"{turnipPrices[10]}");
            SetTurnipColor(buyPrice, SaturdayAM, turnipSell6AM);

            turnipSell6PM.Clear();
            turnipSell6PM.Text = $@"{turnipPrices[11]}";
            ulong SaturdayPM = ulong.Parse($"{turnipPrices[11]}");
            SetTurnipColor(buyPrice, SaturdayPM, turnipSell6PM);

            MyLog.LogEvent("MainForm", "BuyPrice : " + $"{turnipPrices[12]}");
            MyLog.LogEvent("MainForm", "MondayAM : " + $"{turnipPrices[0]}");
            MyLog.LogEvent("MainForm", "MondayPM : " + $"{turnipPrices[1]}");
            MyLog.LogEvent("MainForm", "TuesdayAM : " + $"{turnipPrices[2]}");
            MyLog.LogEvent("MainForm", "TuesdayPM : " + $"{turnipPrices[3]}");
            MyLog.LogEvent("MainForm", "WednesdayAM : " + $"{turnipPrices[4]}");
            MyLog.LogEvent("MainForm", "WednesdayPM : " + $"{turnipPrices[5]}");
            MyLog.LogEvent("MainForm", "ThursdayAM : " + $"{turnipPrices[6]}");
            MyLog.LogEvent("MainForm", "ThursdayPM : " + $"{turnipPrices[7]}");
            MyLog.LogEvent("MainForm", "FridayAM : " + $"{turnipPrices[8]}");
            MyLog.LogEvent("MainForm", "FridayPM : " + $"{turnipPrices[9]}");
            MyLog.LogEvent("MainForm", "SaturdayAM : " + $"{turnipPrices[10]}");
            MyLog.LogEvent("MainForm", "SaturdayPM : " + $"{turnipPrices[11]}");

            ulong[] price = [MondayAM, MondayPM, TuesdayAM, TuesdayPM, WednesdayAM, WednesdayPM, ThursdayAM, ThursdayPM, FridayAM, FridayPM, SaturdayAM, SaturdayPM];
            ulong highest = FindHighest(price);
            SetStar(highest, MondayAM, MondayPM, TuesdayAM, TuesdayPM, WednesdayAM, WednesdayPM, ThursdayAM, ThursdayPM, FridayAM, FridayPM, SaturdayAM, SaturdayPM);
        }
        private static void SetTurnipColor(UInt64 buyPrice, UInt64 comparePrice, RichTextBox target)
        {
            target.SelectionAlignment = HorizontalAlignment.Center;

            if (comparePrice > buyPrice)
            {
                target.BackColor = Color.FromArgb(255, 0, 0);
            }
            else if (comparePrice < buyPrice)
            {
                target.BackColor = Color.FromArgb(0, 255, 0);
            }
        }

        private static UInt64 FindHighest(UInt64[] price)
        {
            UInt64 highest = 0;
            for (int i = 0; i < price.Length; i++)
            {
                if (price[i] > highest)
                {
                    highest = price[i];
                }
            }
            return highest;
        }

        private void SetStar(UInt64 highest, UInt64 MondayAM, UInt64 MondayPM, UInt64 TuesdayAM, UInt64 TuesdayPM, UInt64 WednesdayAM, UInt64 WednesdayPM, UInt64 ThursdayAM, UInt64 ThursdayPM, UInt64 FridayAM, UInt64 FridayPM, UInt64 SaturdayAM, UInt64 SaturdayPM)
        {
            if (MondayAM >= highest) { mondayAMStar.Visible = true; } else { mondayAMStar.Visible = false; }
            if (MondayPM >= highest) { mondayPMStar.Visible = true; } else { mondayPMStar.Visible = false; }

            if (TuesdayAM >= highest) { tuesdayAMStar.Visible = true; } else { tuesdayAMStar.Visible = false; }
            if (TuesdayPM >= highest) { tuesdayPMStar.Visible = true; } else { tuesdayPMStar.Visible = false; }

            if (WednesdayAM >= highest) { wednesdayAMStar.Visible = true; } else { wednesdayAMStar.Visible = false; }
            if (WednesdayPM >= highest) { wednesdayPMStar.Visible = true; } else { wednesdayPMStar.Visible = false; }

            if (ThursdayAM >= highest) { thursdayAMStar.Visible = true; } else { thursdayAMStar.Visible = false; }
            if (ThursdayPM >= highest) { thursdayPMStar.Visible = true; } else { thursdayPMStar.Visible = false; }

            if (FridayAM >= highest) { fridayAMStar.Visible = true; } else { fridayAMStar.Visible = false; }
            if (FridayPM >= highest) { fridayPMStar.Visible = true; } else { fridayPMStar.Visible = false; }

            if (SaturdayAM >= highest) { saturdayAMStar.Visible = true; } else { saturdayAMStar.Visible = false; }
            if (SaturdayPM >= highest) { saturdayPMStar.Visible = true; } else { saturdayPMStar.Visible = false; }
        }

        private void SetTurnipPriceMaxButton_Click(object sender, EventArgs e)
        {
            string max = "999999999";
            string min = "1";
            DialogResult dialogResult = MyMessageBox.Show("Are you sure you want to set all the turnip prices to MAX?\n[Warning] All original prices will be overwritten!", "Set all turnip prices", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                UInt32[] prices = [
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(min, 10)
                ];

                try
                {
                    Utilities.ChangeTurnipPrices(socket, usb, prices);
                    UpdateTurnipPrices();
                }
                catch (Exception ex)
                {
                    MyLog.LogEvent("MainForm", "SetAllTurnip: " + ex.Message);
                    MyMessageBox.Show(ex.Message, "This is a terrible way of doing this!");
                }

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
        }

        public void SetTurnipPriceMax()
        {
            string max = "999999999";
            string min = "1";

            UInt32[] prices = [
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(max, 10), Convert.ToUInt32(max, 10),
                Convert.ToUInt32(min, 10)
                ];

            try
            {
                Utilities.ChangeTurnipPrices(socket, usb, prices);
                UpdateTurnipPrices();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("MainForm", "SetAllTurnip: " + ex.Message.ToString());
                MyMessageBox.Show(ex.Message.ToString(), "This is a terrible way of doing this!");
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void SetTurnipPriceButton_Click(object sender, EventArgs e)
        {
            if (turnipBuyPrice.Text == "" ||
                turnipSell1AM.Text == "" || turnipSell1PM.Text == "" ||
                turnipSell2AM.Text == "" || turnipSell2PM.Text == "" ||
                turnipSell3AM.Text == "" || turnipSell3PM.Text == "" ||
                turnipSell4AM.Text == "" || turnipSell4PM.Text == "" ||
                turnipSell5AM.Text == "" || turnipSell5PM.Text == "" ||
                turnipSell6AM.Text == "" || turnipSell6PM.Text == "")
            {
                MessageBox.Show(@"Turnip prices cannot be empty");
                return;
            }

            DialogResult dialogResult = MyMessageBox.Show("Are you sure you want to set the turnip prices?\n[Warning] All original prices will be overwritten!", "Set turnip prices", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                UInt32[] prices = [
                Convert.ToUInt32(turnipSell1AM.Text, 10), Convert.ToUInt32(turnipSell1PM.Text, 10),
                Convert.ToUInt32(turnipSell2AM.Text, 10), Convert.ToUInt32(turnipSell2PM.Text, 10),
                Convert.ToUInt32(turnipSell3AM.Text, 10), Convert.ToUInt32(turnipSell3PM.Text, 10),
                Convert.ToUInt32(turnipSell4AM.Text, 10), Convert.ToUInt32(turnipSell4PM.Text, 10),
                Convert.ToUInt32(turnipSell5AM.Text, 10), Convert.ToUInt32(turnipSell5PM.Text, 10),
                Convert.ToUInt32(turnipSell6AM.Text, 10), Convert.ToUInt32(turnipSell6PM.Text, 10),
                Convert.ToUInt32(turnipBuyPrice.Text, 10)
                ];

                try
                {
                    Utilities.ChangeTurnipPrices(socket, usb, prices);
                    UpdateTurnipPrices();
                }
                catch (Exception ex)
                {
                    MyLog.LogEvent("MainForm", "SetTurnip: " + ex.Message);
                    MyMessageBox.Show(ex.Message, "This is a terrible way of doing this!");
                }

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
        }

        #endregion

        #region Cell Click
        private void GridView_SelectionChanged(object sender, EventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            senderGrid.ClearSelection();
        }

        private void GridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                if (senderGrid == InsectGridView)
                    CellContentClick(senderGrid, e.RowIndex, e.ColumnIndex, ref insectRate);
                else if (senderGrid == RiverFishGridView)
                    CellContentClick(senderGrid, e.RowIndex, e.ColumnIndex, ref riverFishRate);
                else if (senderGrid == SeaFishGridView)
                    CellContentClick(senderGrid, e.RowIndex, e.ColumnIndex, ref seaFishRate);
                else if (senderGrid == SeaCreatureGridView)
                    CellContentClick(senderGrid, e.RowIndex, e.ColumnIndex, ref seaCreatureRate);
            }
        }

        private void CellContentClick(DataGridView grid, int row, int col, ref int[] rate)
        {
            DataGridViewCellStyle btnStyle = new()
            {
                BackColor = Color.FromArgb(114, 137, 218),
                Font = new Font("Arial", 8F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            DataGridViewCellStyle selectedbtnStyle = new()
            {
                BackColor = Color.FromArgb(80, 80, 255),
                Font = new Font("Arial", 8F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            var Minbtn = (DataGridViewButtonCell)grid.Rows[row].Cells[6];
            var Defbtn = (DataGridViewButtonCell)grid.Rows[row].Cells[8];
            var Maxbtn = (DataGridViewButtonCell)grid.Rows[row].Cells[10];

            var cell = (DataGridViewProgressCell)grid.Rows[row].Cells[5];
            var index = (int)grid.Rows[row].Cells[1].Value;
            if (col == 6)
            {
                rate[index] = 0;
                cell.setValue = 0;

                if (grid == InsectGridView)
                    SetSpawnRate(InsectAppearParam, Utilities.InsectDataSize, (int)grid.Rows[row].Cells[1].Value, 0, 0);
                else if (grid == RiverFishGridView)
                    SetSpawnRate(FishRiverAppearParam, Utilities.FishDataSize, (int)grid.Rows[row].Cells[1].Value, 0, 1);
                else if (grid == SeaFishGridView)
                    SetSpawnRate(FishSeaAppearParam, Utilities.FishDataSize, (int)grid.Rows[row].Cells[1].Value, 0, 2);
                else if (grid == SeaCreatureGridView)
                    SetSpawnRate(CreatureSeaAppearParam, Utilities.SeaCreatureDataSize, (int)grid.Rows[row].Cells[1].Value, 0, 3);

                Minbtn.Style = selectedbtnStyle;
                Defbtn.Style = btnStyle;
                Maxbtn.Style = btnStyle;
            }
            else if (col == 8)
            {
                if (grid.Rows[row].Cells[4].Value != null)
                {
                    rate[index] = (int)grid.Rows[row].Cells[4].Value;
                    cell.setValue = (int)grid.Rows[row].Cells[4].Value;
                    cell.remark = GetRemark($"{grid.Rows[row].Cells[3].Value:X4}");

                    if (grid == InsectGridView)
                        SetSpawnRate(InsectAppearParam, Utilities.InsectDataSize, (int)grid.Rows[row].Cells[1].Value, 1, 0);
                    else if (grid == RiverFishGridView)
                        SetSpawnRate(FishRiverAppearParam, Utilities.FishDataSize, (int)grid.Rows[row].Cells[1].Value, 1, 1);
                    else if (grid == SeaFishGridView)
                        SetSpawnRate(FishSeaAppearParam, Utilities.FishDataSize, (int)grid.Rows[row].Cells[1].Value, 1, 2);
                    else if (grid == SeaCreatureGridView)
                        SetSpawnRate(CreatureSeaAppearParam, Utilities.SeaCreatureDataSize, (int)grid.Rows[row].Cells[1].Value, 1, 3);

                    Minbtn.Style = btnStyle;
                    Defbtn.Style = selectedbtnStyle;
                    Maxbtn.Style = btnStyle;
                }
            }
            else if (col == 10)
            {
                rate[index] = 255;
                cell.setValue = 255;

                if (grid == InsectGridView)
                    SetSpawnRate(InsectAppearParam, Utilities.InsectDataSize, (int)grid.Rows[row].Cells[1].Value, 2, 0);
                else if (grid == RiverFishGridView)
                    SetSpawnRate(FishRiverAppearParam, Utilities.FishDataSize, (int)grid.Rows[row].Cells[1].Value, 2, 1);
                else if (grid == SeaFishGridView)
                    SetSpawnRate(FishSeaAppearParam, Utilities.FishDataSize, (int)grid.Rows[row].Cells[1].Value, 2, 2);
                else if (grid == SeaCreatureGridView)
                    SetSpawnRate(CreatureSeaAppearParam, Utilities.SeaCreatureDataSize, (int)grid.Rows[row].Cells[1].Value, 2, 3);

                Minbtn.Style = btnStyle;
                Defbtn.Style = btnStyle;
                Maxbtn.Style = selectedbtnStyle;
            }
            grid.InvalidateCell(cell);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void SetSpawnRate(byte[] source, int size, int index, int mode, int type)
        {
            if (source == null)
            {
                MessageBox.Show(@"Please load the critter data first.", @"Missing critter data!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int localIndex = index * 2;
                byte[] b;
                if (source == InsectAppearParam)
                    b = new byte[12 * 6 * 2];
                else
                    b = new byte[78]; //[12 * 3 * 2];


                if (mode == 0) // min
                {
                    for (int i = 0; i < b.Length; i++)
                        b[i] = 0;
                }
                else if (mode == 1) // default
                {
                    for (int i = 0; i < b.Length; i++)
                        b[i] = source[size * localIndex + 2 + i];
                }
                else if (mode == 2) // max
                    for (int i = 0; i < b.Length; i += 2)
                    {
                        b[i] = 0xFF;
                        b[i + 1] = 0;
                    }
                //Debug.Print(Encoding.UTF8.GetString(Utilities.transform(b)));
                Utilities.SendSpawnRate(socket, usb, b, localIndex, type, ref counter);
                localIndex++;
                if (mode == 1)
                {
                    for (int i = 0; i < b.Length; i++)
                        b[i] = source[size * localIndex + 2 + i];
                }
                Utilities.SendSpawnRate(socket, usb, b, localIndex, type, ref counter);
            }
            catch (Exception e)
            {
                MyMessageBox.Show(e.Message, "NOTE: This isn't particularly efficient. Too bad!");
            }
        }
        #endregion

        #region Search box
        private void CritterSearchBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (currentGridView.DataSource == null)
                    return;
                if (currentGridView == InsectGridView)
                    SearchBox_TextChanged(currentGridView, ref insectRate);
                else if (currentGridView == RiverFishGridView)
                    SearchBox_TextChanged(currentGridView, ref riverFishRate, 1);
                else if (currentGridView == SeaFishGridView)
                    SearchBox_TextChanged(currentGridView, ref seaFishRate, 1);
                else if (currentGridView == SeaCreatureGridView)
                    SearchBox_TextChanged(currentGridView, ref seaCreatureRate, 1);
            }
            catch
            {
                CritterSearchBox.Clear();
            }
        }

        private void SearchBox_TextChanged(DataGridView grid, ref int[] rate, int mode = 0)
        {
            ((DataTable)grid.DataSource).DefaultView.RowFilter = $"Name LIKE '%{CritterSearchBox.Text}%'";

            DataGridViewCellStyle btnStyle = new()
            {
                BackColor = Color.FromArgb(114, 137, 218),
                Font = new Font("Arial", 8F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            DataGridViewCellStyle selectedbtnStyle = new()
            {
                BackColor = Color.FromArgb(80, 80, 255),
                Font = new Font("Arial", 8F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            for (int i = 0; i < grid.Rows.Count; i++)
            {
                var cell = (DataGridViewProgressCell)grid.Rows[i].Cells[5];
                var Minbtn = (DataGridViewButtonCell)grid.Rows[i].Cells[6];
                var Defbtn = (DataGridViewButtonCell)grid.Rows[i].Cells[8];
                var Maxbtn = (DataGridViewButtonCell)grid.Rows[i].Cells[10];
                int spawnrate = rate[(int)grid.Rows[i].Cells[1].Value];

                cell.setValue = spawnrate;
                cell.remark = GetRemark($"{grid.Rows[i].Cells[3].Value:X4}");
                cell.mode = mode;
                if (spawnrate <= 0)
                {
                    Minbtn.Style = selectedbtnStyle;
                    Defbtn.Style = btnStyle;
                    Maxbtn.Style = btnStyle;
                }
                else if (spawnrate >= 255)
                {
                    Minbtn.Style = btnStyle;
                    Defbtn.Style = btnStyle;
                    Maxbtn.Style = selectedbtnStyle;
                }
            }
        }

        private void CritterSearchBox_Click(object sender, EventArgs e)
        {
            if (CritterSearchBox.Text == @"Search...")
                CritterSearchBox.Clear();
        }
        #endregion

        #region Critter Spawner
        private void GridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (e.RowIndex >= 0 && e.RowIndex < senderGrid.Rows.Count)
            {
                int row = e.RowIndex;
                if (e.ColumnIndex == 0)
                {
                    CellFormatting(senderGrid, row, e);
                }
            }
        }

        private static void CellFormatting(DataGridView grid, int row, DataGridViewCellFormattingEventArgs e)
        {
            if (grid.Rows[row] == null)
                return;
            if (grid.Rows[row].Cells.Count <= 3)
                return;
            if (grid.Rows[row].Cells[3].Value == null)
                return;
            string id = grid.Rows[row].Cells[3].Value.ToString();
            //Debug.Print(Id);
            string imagePath = GetImagePathFromID(id, itemSource);
            if (imagePath != "")
            {
                e.Value = ImageCacher.GetImage(imagePath);
            }
        }

        private void LoadGridView(byte[] source, DataGridView grid, ref int[] rate, int size, int num, int mode = 0)
        {
            if (source != null)
            {
                grid.DataSource = null;
                grid.Rows.Clear();
                grid.Columns.Clear();

                DataTable dt = new();

                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
                grid.DefaultCellStyle.ForeColor = Color.White;
                //grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 57, 60, 67);


                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 57, 60, 67);
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);
                grid.EnableHeadersVisualStyles = false;

                grid.ShowCellToolTips = false;

                DataGridViewCellStyle btnStyle = new()
                {
                    BackColor = Color.FromArgb(114, 137, 218),
                    Font = new Font("Arial", 8F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter

                };

                DataGridViewCellStyle selectedbtnStyle = new()
                {
                    BackColor = Color.FromArgb(80, 80, 255),
                    Font = new Font("Arial", 8F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                };

                DataGridViewCellStyle fontStyle = new()
                {
                    Font = new Font("Arial", 10F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                };

                dt.Columns.Add("Index", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("ID", typeof(string));
                dt.Columns.Add(" ", typeof(int));

                UInt16 id;

                for (int i = 0; i < num / 2; i++)
                {
                    id = (UInt16)(source[i * size * 2]
                             + (source[i * size * 2 + 1] << 8));
                    var critterName = GetNameFromID($"{id:X4}", itemSource);
                    int spawnRate;
                    if (grid == InsectGridView)
                    {
                        spawnRate = GetSpawnRate(source, i * size * 2 + 2, size - 12);
                    }
                    else if (grid == SeaCreatureGridView)
                    {
                        spawnRate = GetSpawnRate(source, i * size * 2 + 2, size - 10);
                    }
                    else
                    {
                        spawnRate = GetSpawnRate(source, i * size * 2 + 2, size - 8);
                    }
                    dt.Rows.Add(i, critterName, $"{id:X4}", spawnRate);
                }

                grid.DataSource = dt;

                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = @"Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                };
                grid.Columns.Insert(0, imageColumn);


                // Index
                grid.Columns[1].DefaultCellStyle = fontStyle;
                grid.Columns[1].Width = 50;
                //grid.Columns[1].Visible = false;
                grid.Columns[1].Resizable = DataGridViewTriState.False;

                // Name
                grid.Columns[2].DefaultCellStyle = fontStyle;
                grid.Columns[2].Width = 250;
                grid.Columns[2].Resizable = DataGridViewTriState.False;

                // ID
                grid.Columns[3].DefaultCellStyle = fontStyle;
                //grid.Columns[3].Visible = false;
                grid.Columns[3].Width = 60;
                grid.Columns[3].Resizable = DataGridViewTriState.False;

                // Rate
                grid.Columns[4].DefaultCellStyle = fontStyle;
                grid.Columns[4].Width = 50;
                grid.Columns[4].Visible = false;
                //grid.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.Columns[4].Resizable = DataGridViewTriState.False;

                DataGridViewProgressColumn barColumn = new()
                {
                    Name = "Bar",
                    HeaderText = "",
                    DefaultCellStyle = fontStyle,
                    Width = 320,
                    Resizable = DataGridViewTriState.False,
                };
                grid.Columns.Add(barColumn);

                DataGridViewButtonColumn minColumn = new()
                {
                    Name = "Min",
                    HeaderText = "",
                    FlatStyle = FlatStyle.Popup,
                    DefaultCellStyle = btnStyle,
                    Width = 100,
                    Text = "Disable Spawn",
                    UseColumnTextForButtonValue = true,
                    Resizable = DataGridViewTriState.False,
                };
                grid.Columns.Add(minColumn);

                DataGridViewTextBoxColumn separator1 = new()
                {
                    Width = 10,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                };
                grid.Columns.Add(separator1);

                DataGridViewButtonColumn defaultColumn = new()
                {
                    Name = "Default",
                    HeaderText = "",
                    FlatStyle = FlatStyle.Popup,
                    DefaultCellStyle = selectedbtnStyle,
                    Width = 100,
                    Text = "Default",
                    UseColumnTextForButtonValue = true,
                    Resizable = DataGridViewTriState.False,
                };
                grid.Columns.Add(defaultColumn);

                DataGridViewTextBoxColumn separator2 = new()
                {
                    Width = 10,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                };
                grid.Columns.Add(separator2);

                DataGridViewButtonColumn maxColumn = new()
                {
                    Name = "Max",
                    HeaderText = "",
                    FlatStyle = FlatStyle.Popup,
                    DefaultCellStyle = btnStyle,
                    Width = 100,
                    Text = "Max Spawn",
                    UseColumnTextForButtonValue = true,
                    Resizable = DataGridViewTriState.False,
                };
                grid.Columns.Add(maxColumn);

                rate = new int[num / 2];

                for (int i = 0; i < num / 2; i++)
                {
                    id = (UInt16)(source[i * size * 2]
                            + (source[i * size * 2 + 1] << 8));

                    int spawnRate;
                    if (grid == InsectGridView)
                    {
                        spawnRate = GetSpawnRate(source, i * size * 2 + 2, size - 12);
                    }
                    else if (grid == SeaCreatureGridView)
                    {
                        spawnRate = GetSpawnRate(source, i * size * 2 + 2, size - 10);
                    }
                    else
                    {
                        spawnRate = GetSpawnRate(source, i * size * 2 + 2, size - 8);
                    }

                    rate[i] = spawnRate;

                    DataGridViewProgressCell pc = new()
                    {
                        setValue = spawnRate,
                        remark = GetRemark($"{id:X4}"),
                        mode = mode,
                    };
                    grid.Rows[i].Cells[5] = pc;
                }

                grid.ColumnHeadersVisible = false;
                grid.ClearSelection();
            }
        }

        private static int GetSpawnRate(byte[] source, int index, int size)
        {
            int max = 0;
            for (int i = 0; i < size; i++)
            {
                if (source[index + i] > max)
                    max = source[index + i];
            }
            return max;
        }

        private static string GetRemark(string ID)
        {
            return ID switch
            {
                //Common butterfly
                ("0272") => ("Except on rainy days"),
                //Yellow butterfly
                ("0271") => ("Except on rainy days"),
                //Tiger butterfly
                ("0247") => ("Except on rainy days"),
                //Peacock butterfly
                ("0262") => ("Catch 20 or more bugs to spawn\nExcept on rainy days"),
                //Common bluebottle
                ("0D95") => ("Except on rainy days"),
                //Paper kite butterfly
                ("0D96") => ("Except on rainy days"),
                //Great purple emperor
                ("0D97") => ("Catch 50 or more bugs to spawn\nExcept on rainy days"),
                //Monarch butterfly
                ("027C") => ("Except on rainy days"),
                //Emperor butterfly
                ("0273") => ("Catch 20 or more bugs to spawn\nExcept on rainy days"),
                //Agrias butterfly
                ("026C") => ("Catch 20 or more bugs to spawn\nExcept on rainy days"),
                //Rajah brooke's birdwing
                ("0248") => ("Catch 20 or more bugs to spawn\nExcept on rainy days"),
                //Queen Alexandra's birdwing
                ("024A") => ("Catch 50 or more bugs to spawn\nExcept on rainy days"),
                //Moth
                ("0250") => ("Except on rainy days"),
                //Atlas moth
                ("028C") => ("Catch 20 or more bugs to spawn"),
                //Madagascan sunset moth
                ("0D9C") => ("Catch 20 or more bugs to spawn\nExcept on rainy days"),
                //Long locust
                ("0284") => (""),
                //Migratory Locust
                ("0288") => ("Catch 20 or more bugs to spawn"),
                //Rice grasshopper
                ("025D") => (""),
                //Grasshopper
                ("0265") => ("Except on rainy days"),
                //Cricket
                ("0269") => ("Except on rainy days"),
                //Bell cricket
                ("0282") => ("Except on rainy days"),
                //Mantis
                ("025F") => ("Except on rainy days"),
                //Orchid mantis
                ("0256") => ("Catch 20 or more bugs to spawn\nExcept on rainy days"),
                //Honeybee
                ("026F") => ("Except on rainy days"),
                //Wasp
                ("0283") => ("✶ Spawn rate seems to have no effect\nSpawn when nest falls from tree"),
                //Brown cicada
                ("0246") => (""),
                //Robust cicada
                ("026D") => (""),
                //Giant cicada
                ("026A") => ("Catch 20 or more bugs to spawn"),
                //Walker cicada
                ("0289") => (""),
                //Evening cicada
                ("0259") => (""),
                //Cicada shell
                ("0281") => ("Catch 50 or more bugs to spawn"),
                //Red dragonfly
                ("0249") => ("Except on rainy days"),
                //Darner dragonfly
                ("0253") => ("Except on rainy days"),
                //Banded dragonfly
                ("027B") => ("Catch 50 or more bugs to spawn\nExcept on rainy days"),
                //Damselfly
                ("14DB") => ("Except on rainy days"),
                //Firefly
                ("025B") => ("Except on rainy days"),
                //Mole cricket
                ("027A") => (""),
                //Pondskater
                ("024B") => (""),
                //Diving beetle
                ("0252") => (""),
                //Giant water bug
                ("1425") => ("Catch 50 or more bugs to spawn"),
                //Stinkbug
                ("0260") => ("Except on rainy days"),
                //Man-faced stink bug
                ("0D9B") => ("Catch 20 or more bugs to spawn\nExcept on rainy days"),
                //Ladybug
                ("0287") => ("Except on rainy days"),
                //Tiger beetle
                ("0257") => ("Catch 20 or more bugs to spawn\nExcept on rainy days"),
                //Jewel beetle
                ("0285") => ("Catch 20 or more bugs to spawn\nExcept on rainy days"),
                //Violin beetle
                ("028A") => ("Except on rainy days"),
                //Citrus long-horned beetle
                ("0261") => ("Except on rainy days"),
                //Rosalia batesi beetle
                ("0D9F") => ("Catch 20 or more bugs to spawn\nExcept on rainy days"),
                //Blue weevil beetle
                ("0D9D") => (""),
                //Dung beetle
                ("025C") => ("Spawn when there is snowball on the ground"),
                //Earth-boring dung beetle
                ("0266") => (""),
                //Scarab beetle
                ("027F") => ("Catch 50 or more bugs to spawn"),
                //Drone beetle
                ("0D98") => (""),
                //Goliath beetle
                ("0254") => ("Catch 100 or more bugs to spawn"),
                //Saw stag
                ("0278") => (""),
                //Miyama stag
                ("0270") => (""),
                //Giant stag
                ("027D") => ("Catch 50 or more bugs to spawn"),
                //Rainbow stag
                ("0277") => ("Catch 50 or more bugs to spawn"),
                //Cyclommatus stag
                ("025A") => ("Catch 100 or more bugs to spawn"),
                //Golden stag
                ("027E") => ("Catch 100 or more bugs to spawn"),
                //Giraffe stag
                ("0D9A") => ("Catch 100 or more bugs to spawn"),
                //Horned dynastid
                ("0264") => (""),
                //Horned atlas
                ("0267") => ("Catch 100 or more bugs to spawn"),
                //Horned elephant
                ("028D") => ("Catch 100 or more bugs to spawn"),
                //Horned hercules
                ("0258") => ("Catch 100 or more bugs to spawn"),
                //Walking stick
                ("0276") => ("Catch 20 or more bugs to spawn"),
                //Walking leaf
                ("0268") => ("Catch 20 or more bugs to spawn"),
                //Bagworm
                ("026E") => ("✶ Spawn rate seems to have no effect\nSpawn when shaking tree"),
                //Ant
                ("024C") => ("✶ Spawn rate seems to have no effect\nSpawn when there is rotten turnip"),
                //Hermit crab
                ("028B") => ("Spawn on beach"),
                //Wharf roach
                ("024F") => ("Spawn on rocky formations at beach"),
                //Fly
                ("0255") => ("✶ Spawn rate seems to have no effect\nSpawn when there is trash item"),
                //Mosquito
                ("025E") => ("Except on rainy days"),
                //Flea
                ("0279") => ("Spawn on villagers"),
                //Snail
                ("0263") => ("Rainy days only"),
                //Pill bug
                ("024E") => ("Spawn underneath rocks"),
                //Centipede
                ("0274") => ("Spawn underneath rocks"),
                //Spider
                ("026B") => ("✶ Spawn rate seems to have no effect\nSpawn when shaking tree"),
                //Tarantula
                ("0286") => (""),
                //Scorpion
                ("0280") => (""),
                //Snowflake
                ("0DD3") => ("Spawn when in season/time"),
                //Cherry-blossom petal
                ("16E3") => ("Spawn when in season/time"),
                //Maple leaf
                ("1CCE") => ("Spawn when in season/time"),
                //Koi
                ("08AC") => ("Catch 20 or more fishes to spawn"),
                //Ranchu Goldfish
                ("1486") => ("Catch 20 or more fishes to spawn"),
                //Soft-shelled Turtle
                ("08B0") => ("Catch 20 or more fishes to spawn"),
                //Giant Snakehead
                ("08B7") => ("Catch 50 or more fishes to spawn"),
                //Pike
                ("08BB") => ("Catch 20 or more fishes to spawn"),
                //Char
                ("08BF") => ("Catch 20 or more fishes to spawn"),
                //Golden Trout
                ("1061") => ("Catch 100 or more fishes to spawn"),
                //Stringfish
                ("08C1") => ("Catch 100 or more fishes to spawn"),
                //King Salmon
                ("08C3") => ("Catch 20 or more fishes to spawn"),
                //Mitten Crab
                ("08C4") => ("Catch 20 or more fishes to spawn"),
                //Nibble Fish
                ("08C6") => ("Catch 20 or more fishes to spawn"),
                //Angelfish
                ("08C7") => ("Catch 20 or more fishes to spawn"),
                //Betta
                ("105F") => ("Catch 20 or more fishes to spawn"),
                //Piranha
                ("08C9") => ("Catch 20 or more fishes to spawn"),
                //Arowana
                ("08CA") => ("Catch 50 or more fishes to spawn"),
                //Dorado
                ("08CB") => ("Catch 100 or more fishes to spawn"),
                //Gar
                ("08CC") => ("Catch 50 or more fishes to spawn"),
                //Arapaima
                ("08CD") => ("Catch 50 or more fishes to spawn"),
                //Saddled Bichir
                ("08CE") => ("Catch 20 or more fishes to spawn"),
                //Sturgeon
                ("105D") => ("Catch 20 or more fishes to spawn"),
                //Napoleonfish
                ("08D4") => ("Catch 50 or more fishes to spawn"),
                //Blowfish
                ("08D6") => ("Catch 20 or more fishes to spawn"),
                //Barred Knifejaw
                ("08D9") => ("Catch 20 or more fishes to spawn"),
                //Moray Eel
                ("08DF") => ("Catch 20 or more fishes to spawn"),
                //Tuna
                ("08E2") => ("Catch 50 or more fishes to spawn"),
                //Blue Marlin
                ("08E3") => ("Catch 50 or more fishes to spawn"),
                //Giant Trevally
                ("08E4") => ("Catch 20 or more fishes to spawn"),
                //Mahi-mahi
                ("106A") => ("Catch 50 or more fishes to spawn"),
                //Ocean Sunfish
                ("08E6") => ("Catch 20 or more fishes to spawn"),
                //Ray
                ("08E5") => ("Catch 20 or more fishes to spawn"),
                //Saw Shark
                ("08E9") => ("Catch 50 or more fishes to spawn"),
                //Hammerhead Shark
                ("08E7") => ("Catch 20 or more fishes to spawn"),
                //Great White Shark
                ("08E8") => ("Catch 50 or more fishes to spawn"),
                //Whale Shark
                ("08EA") => ("Catch 50 or more fishes to spawn"),
                //Suckerfish
                ("106B") => ("Catch 20 or more fishes to spawn"),
                //Football Fish
                ("08E1") => ("Catch 20 or more fishes to spawn"),
                //Oarfish
                ("08EB") => ("Catch 50 or more fishes to spawn"),
                //Barreleye
                ("106C") => ("Catch 100 or more fishes to spawn"),
                //Coelacanth
                ("08EC") => ("Catch 100 or more fishes to spawn\nRainy days only"),
                _ => "",
            };
        }
        #endregion

        #region Disable & Reset Button
        private void DisableAllButton_Click(object sender, EventArgs e)
        {
            if (CritterSearchBox.Text != @"Search...")
            {
                CritterSearchBox.Clear();
            }

            if (currentGridView == InsectGridView)
                DisableAll(currentGridView, ref insectRate);
            else if (currentGridView == RiverFishGridView)
                DisableAll(currentGridView, ref riverFishRate, 1);
            else if (currentGridView == SeaFishGridView)
                DisableAll(currentGridView, ref seaFishRate, 1);
            else if (currentGridView == SeaCreatureGridView)
                DisableAll(currentGridView, ref seaCreatureRate, 1);
        }

        private void DisableAll(DataGridView grid, ref int[] rate, int mode = 0)
        {
            if (rate == null)
                return;
            string temp = null;
            if (CritterSearchBox.Text != @"Search...")
            {
                temp = CritterSearchBox.Text;
                CritterSearchBox.Clear();
            }
            //CritterSearchBox.Clear();

            for (int i = 0; i < rate.Length; i++)
            {
                rate[i] = 0;
            }

            DisableBtn();

            Thread disableThread = new(delegate () { DisableAll(grid, mode, temp); });
            disableThread.Start();
        }

        private void DisableAll(DataGridView grid, int mode, string temp)
        {

            DataGridViewCellStyle btnStyle = new()
            {
                BackColor = Color.FromArgb(114, 137, 218),
                Font = new Font("Arial", 8F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            DataGridViewCellStyle selectedbtnStyle = new()
            {
                BackColor = Color.FromArgb(80, 80, 255),
                Font = new Font("Arial", 8F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            for (int i = 0; i < grid.Rows.Count; i++)
            {
                var cell = (DataGridViewProgressCell)grid.Rows[i].Cells[5];
                var Minbtn = (DataGridViewButtonCell)grid.Rows[i].Cells[6];
                var Defbtn = (DataGridViewButtonCell)grid.Rows[i].Cells[8];
                var Maxbtn = (DataGridViewButtonCell)grid.Rows[i].Cells[10];

                if (grid == InsectGridView)
                    SetSpawnRate(InsectAppearParam, Utilities.InsectDataSize, (int)grid.Rows[i].Cells[1].Value, 0, 0);
                else if (grid == RiverFishGridView)
                    SetSpawnRate(FishRiverAppearParam, Utilities.FishDataSize, (int)grid.Rows[i].Cells[1].Value, 0, 1);
                else if (grid == SeaFishGridView)
                    SetSpawnRate(FishSeaAppearParam, Utilities.FishDataSize, (int)grid.Rows[i].Cells[1].Value, 0, 2);
                else if (grid == SeaCreatureGridView)
                    SetSpawnRate(CreatureSeaAppearParam, Utilities.SeaCreatureDataSize, (int)grid.Rows[i].Cells[1].Value, 0, 3);

                cell.setValue = 0;
                cell.remark = GetRemark($"{grid.Rows[i].Cells[3].Value:X4}");
                cell.mode = mode;
                Minbtn.Style = selectedbtnStyle;
                Defbtn.Style = btnStyle;
                Maxbtn.Style = btnStyle;

                grid.InvalidateCell(cell);
            }

            Invoke((MethodInvoker)delegate
            {
                if (temp != null)
                    CritterSearchBox.Text = temp;
                EnableBtn();
            });
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void ResetAllButton_Click(object sender, EventArgs e)
        {
            if (CritterSearchBox.Text != @"Search...")
            {
                CritterSearchBox.Clear();
            }

            if (currentGridView == InsectGridView)
                ResetAll(currentGridView, ref insectRate);
            else if (currentGridView == RiverFishGridView)
                ResetAll(currentGridView, ref riverFishRate, 1);
            else if (currentGridView == SeaFishGridView)
                ResetAll(currentGridView, ref seaFishRate, 1);
            else if (currentGridView == SeaCreatureGridView)
                ResetAll(currentGridView, ref seaCreatureRate, 1);
        }

        private void ResetAll(DataGridView grid, ref int[] rate, int mode = 0)
        {
            if (rate == null)
                return;
            string temp = null;
            if (CritterSearchBox.Text != @"Search...")
            {
                temp = CritterSearchBox.Text;
                CritterSearchBox.Clear();
            }
            //CritterSearchBox.Clear();

            for (int i = 0; i < rate.Length; i++)
            {
                rate[i] = (int)grid.Rows[i].Cells[4].Value;
            }

            DisableBtn();

            Thread resetThread = new(delegate () { ResetAll(grid, mode, temp); });
            resetThread.Start();
        }

        private void ResetAll(DataGridView grid, int mode, string temp)
        {

            DataGridViewCellStyle btnStyle = new()
            {
                BackColor = Color.FromArgb(114, 137, 218),
                Font = new Font("Arial", 8F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            DataGridViewCellStyle selectedbtnStyle = new()
            {
                BackColor = Color.FromArgb(80, 80, 255),
                Font = new Font("Arial", 8F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            for (int i = 0; i < grid.Rows.Count; i++)
            {
                var cell = (DataGridViewProgressCell)grid.Rows[i].Cells[5];
                var Minbtn = (DataGridViewButtonCell)grid.Rows[i].Cells[6];
                var Defbtn = (DataGridViewButtonCell)grid.Rows[i].Cells[8];
                var Maxbtn = (DataGridViewButtonCell)grid.Rows[i].Cells[10];

                if (grid == InsectGridView)
                    SetSpawnRate(InsectAppearParam, Utilities.InsectDataSize, (int)grid.Rows[i].Cells[1].Value, 1, 0);
                else if (grid == RiverFishGridView)
                    SetSpawnRate(FishRiverAppearParam, Utilities.FishDataSize, (int)grid.Rows[i].Cells[1].Value, 1, 1);
                else if (grid == SeaFishGridView)
                    SetSpawnRate(FishSeaAppearParam, Utilities.FishDataSize, (int)grid.Rows[i].Cells[1].Value, 1, 2);
                else if (grid == SeaCreatureGridView)
                    SetSpawnRate(CreatureSeaAppearParam, Utilities.SeaCreatureDataSize, (int)grid.Rows[i].Cells[1].Value, 1, 3);

                cell.setValue = (int)grid.Rows[i].Cells[4].Value;
                cell.remark = GetRemark($"{grid.Rows[i].Cells[3].Value:X4}");
                cell.mode = mode;
                Minbtn.Style = btnStyle;
                Defbtn.Style = selectedbtnStyle;
                Maxbtn.Style = btnStyle;

                grid.InvalidateCell(cell);
            }

            Invoke((MethodInvoker)delegate
            {
                if (temp != null)
                    CritterSearchBox.Text = temp;
                EnableBtn();
            });
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }
        #endregion

        #region Button Control
        private void DisableBtn()
        {
            CritterNowLoadingPanel.Visible = true;

            DisableAllButton.Visible = false;
            ResetAllButton.Visible = false;
            ReadCritterDataButton.Visible = false;
            currentGridView.Enabled = false;
            CritterSearchBox.Enabled = false;

            InsectButton.Enabled = false;
            RiverFishButton.Enabled = false;
            SeaFishButton.Enabled = false;
            SeaCreatureButton.Enabled = false;
        }

        private void EnableBtn()
        {
            CritterNowLoadingPanel.Visible = false;

            DisableAllButton.Visible = true;
            ResetAllButton.Visible = true;
            ReadCritterDataButton.Visible = true;
            currentGridView.Enabled = true;
            CritterSearchBox.Enabled = true;

            InsectButton.Enabled = true;
            RiverFishButton.Enabled = true;
            SeaFishButton.Enabled = true;
            SeaCreatureButton.Enabled = true;
        }
        #endregion

        #region Mode Button
        private void InsectButton_Click(object sender, EventArgs e)
        {
            if (CritterSearchBox.Text != @"Search...")
            {
                CritterSearchBox.Clear();
            }

            currentGridView = InsectGridView;

            InsectButton.BackColor = Color.FromArgb(80, 80, 255);
            RiverFishButton.BackColor = Color.FromArgb(114, 137, 218);
            SeaFishButton.BackColor = Color.FromArgb(114, 137, 218);
            SeaCreatureButton.BackColor = Color.FromArgb(114, 137, 218);

            InsectGridView.Visible = true;
            RiverFishGridView.Visible = false;
            SeaFishGridView.Visible = false;
            SeaCreatureGridView.Visible = false;

            InsectGridView.ClearSelection();
        }

        private void RiverFishButton_Click(object sender, EventArgs e)
        {
            if (CritterSearchBox.Text != @"Search...")
            {
                CritterSearchBox.Clear();
            }

            currentGridView = RiverFishGridView;

            InsectButton.BackColor = Color.FromArgb(114, 137, 218);
            RiverFishButton.BackColor = Color.FromArgb(80, 80, 255);
            SeaFishButton.BackColor = Color.FromArgb(114, 137, 218);
            SeaCreatureButton.BackColor = Color.FromArgb(114, 137, 218);


            InsectGridView.Visible = false;
            RiverFishGridView.Visible = true;
            SeaFishGridView.Visible = false;
            SeaCreatureGridView.Visible = false;

            RiverFishGridView.ClearSelection();
        }

        private void SeaFishButton_Click(object sender, EventArgs e)
        {
            if (CritterSearchBox.Text != @"Search...")
            {
                CritterSearchBox.Clear();
            }

            currentGridView = SeaFishGridView;

            InsectButton.BackColor = Color.FromArgb(114, 137, 218);
            RiverFishButton.BackColor = Color.FromArgb(114, 137, 218);
            SeaFishButton.BackColor = Color.FromArgb(80, 80, 255);
            SeaCreatureButton.BackColor = Color.FromArgb(114, 137, 218);


            InsectGridView.Visible = false;
            RiverFishGridView.Visible = false;
            SeaFishGridView.Visible = true;
            SeaCreatureGridView.Visible = false;

            SeaFishGridView.ClearSelection();
        }

        private void SeaCreatureButton_Click(object sender, EventArgs e)
        {
            if (CritterSearchBox.Text != @"Search...")
            {
                CritterSearchBox.Clear();
            }

            currentGridView = SeaCreatureGridView;

            InsectButton.BackColor = Color.FromArgb(114, 137, 218);
            RiverFishButton.BackColor = Color.FromArgb(114, 137, 218);
            SeaFishButton.BackColor = Color.FromArgb(114, 137, 218);
            SeaCreatureButton.BackColor = Color.FromArgb(80, 80, 255);


            InsectGridView.Visible = false;
            RiverFishGridView.Visible = false;
            SeaFishGridView.Visible = false;
            SeaCreatureGridView.Visible = true;

            SeaCreatureGridView.ClearSelection();
        }
        #endregion

        #region Read Data
        private void ReadCritterDataButton_Click(object sender, EventArgs e)
        {
            DisableBtn();

            Thread readThread = new(ReadData);
            readThread.Start();

        }

        private void ReadData()
        {
            try
            {
                if (currentGridView == InsectGridView)
                {
                    InsectAppearParam = Utilities.GetCritterData(socket, usb, 0);
                    File.WriteAllBytes(insectAppearFileName, InsectAppearParam);

                    Invoke((MethodInvoker)delegate
                    {
                        InsectGridView.DataSource = null;
                        InsectGridView.Rows.Clear();
                        InsectGridView.Columns.Clear();
                        LoadGridView(InsectAppearParam, InsectGridView, ref insectRate, Utilities.InsectDataSize, Utilities.InsectNumRecords);
                    });
                }
                else if (currentGridView == RiverFishGridView)
                {
                    FishRiverAppearParam = Utilities.GetCritterData(socket, usb, 1);
                    File.WriteAllBytes(fishRiverAppearFileName, FishRiverAppearParam);

                    Invoke((MethodInvoker)delegate
                    {
                        RiverFishGridView.DataSource = null;
                        RiverFishGridView.Rows.Clear();
                        RiverFishGridView.Columns.Clear();
                        LoadGridView(FishRiverAppearParam, RiverFishGridView, ref riverFishRate, Utilities.FishDataSize, Utilities.FishRiverNumRecords, 1);
                    });
                }
                else if (currentGridView == SeaFishGridView)
                {
                    FishSeaAppearParam = Utilities.GetCritterData(socket, usb, 2);
                    File.WriteAllBytes(fishSeaAppearFileName, FishSeaAppearParam);

                    Invoke((MethodInvoker)delegate
                    {
                        SeaFishGridView.DataSource = null;
                        SeaFishGridView.Rows.Clear();
                        SeaFishGridView.Columns.Clear();
                        LoadGridView(FishSeaAppearParam, SeaFishGridView, ref seaFishRate, Utilities.FishDataSize, Utilities.FishSeaNumRecords, 1);
                    });
                }
                else if (currentGridView == SeaCreatureGridView)
                {
                    CreatureSeaAppearParam = Utilities.GetCritterData(socket, usb, 3);
                    File.WriteAllBytes(CreatureSeaAppearFileName, CreatureSeaAppearParam);

                    Invoke((MethodInvoker)delegate
                    {
                        SeaCreatureGridView.DataSource = null;
                        SeaCreatureGridView.Rows.Clear();
                        SeaCreatureGridView.Columns.Clear();
                        LoadGridView(CreatureSeaAppearParam, SeaCreatureGridView, ref seaCreatureRate, Utilities.SeaCreatureDataSize, Utilities.SeaCreatureNumRecords, 1);
                    });
                }
            }
            catch (Exception e)
            {
                MyMessageBox.Show(e.Message, "This is a stupid fix, but I don't have time to da a cleaner implementation");
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            Invoke((MethodInvoker)delegate
            {
                EnableBtn();
            });
        }
        #endregion

        #region Villager

        private void LoadAllVillager()
        {
            lock (villagerLock)
            {
                if (usb == null)
                    ShowVillagerWait(25000, "Acquiring villager data...");
                else
                    ShowVillagerWait(15000, "Acquiring villager data...");

                if ((socket == null || socket.Connected == false) && usb == null && !Utilities.isEmulator)
                {
                    HideVillagerWait();
                    return;
                }

                VillagerLoading = true;
                selectedVillagerButton = null;

                HouseList = new int[10];

                for (int i = 0; i < 10; i++)
                {
                    byte b = Utilities.GetHouseOwner(socket, usb, i, ref counter);
                    if (b == 0xDD)
                    {
                        HideVillagerWait();
                        return;
                    }
                    HouseList[i] = Convert.ToInt32(b);
                }
                Debug.Print(string.Join(" ", HouseList));


                V = new Villager[10];
                villagerButton = new Button[10];

                for (int i = 0; i < 10; i++)
                {
                    byte[] b = Utilities.GetVillager(socket, usb, i, (int)(Utilities.VillagerMemoryTinySize), ref counter);
                    V[i] = new Villager(b, i)
                    {
                        HouseIndex = Utilities.FindHouseIndex(i, HouseList)
                    };

                    byte f = Utilities.GetVillagerHouseFlag(socket, usb, V[i].HouseIndex, 0x8, ref counter);
                    V[i].MoveInFlag = Convert.ToInt32(f);

                    byte[] move = Utilities.GetMoveout(socket, usb, i, 0x33, ref counter);
                    V[i].AbandonedHouseFlag = Convert.ToInt32(move[0]);
                    V[i].InvitedFlag = Convert.ToInt32(move[0x14]);
                    V[i].ForceMoveOutFlag = Convert.ToInt32(move[move.Length - 1]);
                    byte[] catchphrase = Utilities.GetCatchphrase(socket, usb, i, ref counter);
                    V[i].Catchphrase = catchphrase;

                    int friendship = V[i].Friendship[0];

                    Image img;
                    if (V[i].GetRealName() == "ERROR")
                    {
                        string path = Utilities.GetVillagerImage(V[i].GetRealName());
                        if (!path.Equals(string.Empty))
                            img = ImageCacher.GetImage(path);
                        else
                            img = new Bitmap(Properties.Resources.Leaf, new Size(110, 110));
                    }
                    else
                    {
                        string path = Utilities.GetVillagerImage(V[i].GetInternalName());
                        if (!path.Equals(string.Empty))
                            img = ImageCacher.GetImage(path);
                        else
                            img = new Bitmap(Properties.Resources.Leaf, new Size(110, 110));
                    }

                    villagerButton[i] = new Button
                    {
                        TextAlign = ContentAlignment.TopCenter,
                        ForeColor = Color.White,
                        Font = new Font("Arial", 8F, FontStyle.Bold),
                        BackColor = Color.FromArgb(friendship, friendship / 2, friendship),
                        FlatStyle = FlatStyle.Flat,
                        Name = "villagerBtn" + i,
                        Tag = i,
                        Size = new Size(128, 128),
                        UseVisualStyleBackColor = false,
                        Text = V[i].GetRealName() + " : " + V[i].GetInternalName(),
                        Image = new Bitmap(img, new Size(110, 110)),
                        ImageAlign = ContentAlignment.BottomCenter
                    };

                    villagerButton[i].FlatAppearance.BorderSize = 0;

                    if (i < 5)
                        villagerButton[i].Location = new Point((i * 128) + (i * 10) + 50, 54);
                    else
                        villagerButton[i].Location = new Point(((i - 5) * 128) + ((i - 5) * 10) + 50, 192);

                    if (V[i].MoveInFlag == 0xC || V[i].MoveInFlag == 0xB)
                    {
                        if (V[i].IsReal())
                        {
                            if (V[i].IsInvited())
                                villagerButton[i].Text += "\n(Tom Nook Invited)";
                            else
                                villagerButton[i].Text += "\n(Moving In)";
                        }
                        else
                            villagerButton[i].Text += "\n(Just Move Out)";
                    }
                    else if (V[i].InvitedFlag == 0x2)
                        villagerButton[i].Text += "\n(Invited by Visitor)";
                    else if (V[i].AbandonedHouseFlag == 0x1 && V[i].ForceMoveOutFlag == 0x0)
                        villagerButton[i].Text += "\n(Floor Sweeping)";
                    else if (V[i].AbandonedHouseFlag == 0x2 && V[i].ForceMoveOutFlag == 0x1)
                        villagerButton[i].Text += "\n(Moving Out 1)";
                    else if (V[i].AbandonedHouseFlag == 0x2 && V[i].ForceMoveOutFlag == 0x0)
                        villagerButton[i].Text += "\n(Moving Out 2)";

                    villagerButton[i].MouseDown += VillagerButton_MouseDown;
                }


                Invoke((MethodInvoker)delegate
                {
                    for (int i = 0; i < 10; i++)
                    {
                        VillagerLargePanel.Controls.Add(villagerButton[i]);
                        villagerButton[i].BringToFront();
                    }

                    VillagerIndex.Text = "";
                    VillagerName.Text = "";
                    VillagerIName.Text = "";
                    VillagerPersonality.Text = "";
                    VillagerFriendship.Text = "";
                    VillagerHouseIndex.Text = "";
                    VillagerCatchphrase.Text = "";

                    VillagerMoveInFlag.Text = "";
                    VillagerAbandonedHouseFlag.Text = "";
                    VillagerForceMoveoutFlag.Text = "";
                    VillagerHeader.Text = "";
                });

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();


                VillagerLoading = false;

                HideVillagerWait();

            }
        }

        private void VillagerButton_MouseDown(object sender, MouseEventArgs e)
        {
            selectedVillagerButton = (Button)sender;

            RefreshVillagerUI(false);
        }

        private void VillagerProgressTimer_Tick(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                if (counter <= VillagerNowLoadingProgressBar.Maximum)
                    VillagerNowLoadingProgressBar.Value = counter;
                else
                    VillagerNowLoadingProgressBar.Value = VillagerNowLoadingProgressBar.Maximum;
            });
        }

        private void ShowVillagerWait(int size, string msg = "")
        {
            Invoke((MethodInvoker)delegate
            {
                VillagerControlPanel.Enabled = false;
                VillagerNowLoadingLongMessage.SelectionAlignment = HorizontalAlignment.Center;
                VillagerNowLoadingLongMessage.Text = msg;
                counter = 0;
                if (usb == null)
                    VillagerNowLoadingProgressBar.Maximum = size / 500 + 5;
                else
                    VillagerNowLoadingProgressBar.Maximum = size / 300 + 5;
                Debug.Print("Max : " + VillagerNowLoadingProgressBar.Maximum);
                VillagerNowLoadingProgressBar.Value = counter;
                VillagerNowLoadingPanel.Visible = true;
                VillagerProgressTimer.Start();
            });
        }
        private void HideVillagerWait()
        {
            if (InvokeRequired)
            {
                MethodInvoker method = HideVillagerWait;
                Invoke(method);
                return;
            }
            if (!VillagerLoading)
            {
                VillagerNowLoadingPanel.Visible = false;
                VillagerControlPanel.Enabled = true;
                if (VillagerNowLoadingPanel.Location.X != 780)
                    VillagerNowLoadingPanel.Location = new Point(780, 330);
                VillagerProgressTimer.Stop();
            }
        }

        public void RefreshVillagerUI(bool clear)
        {
            for (int j = 0; j < 10; j++)
            {
                int friendship = V[j].Friendship[0];
                if (villagerButton[j] != selectedVillagerButton)
                    villagerButton[j].BackColor = Color.FromArgb(friendship, friendship / 2, friendship);

                villagerButton[j].Text = V[j].GetRealName() + " : " + V[j].GetInternalName();

                if (V[j].MoveInFlag == 0xC || V[j].MoveInFlag == 0xB)
                {
                    if (V[j].IsReal())
                    {
                        if (V[j].IsInvited())
                            villagerButton[j].Text += "\n(Tom Nook Invited)";
                        else
                            villagerButton[j].Text += "\n(Moving In)";
                    }
                    else
                        villagerButton[j].Text += "\n(Just Move Out)";
                }
                else if (V[j].InvitedFlag == 0x2)
                    villagerButton[j].Text += "\n(Invited by Visitor)";
                else if (V[j].AbandonedHouseFlag == 0x1 && V[j].ForceMoveOutFlag == 0x0)
                    villagerButton[j].Text += "\n(Floor Sweeping)";
                else if (V[j].AbandonedHouseFlag == 0x2 && V[j].ForceMoveOutFlag == 0x1)
                    villagerButton[j].Text += "\n(Moving Out 1)";
                else if (V[j].AbandonedHouseFlag == 0x2 && V[j].ForceMoveOutFlag == 0x0)
                    villagerButton[j].Text += "\n(Moving Out 2)";

                Image img;
                if (V[j].GetRealName() == "ERROR")
                {
                    string path = Utilities.GetVillagerImage(V[j].GetRealName());
                    if (!path.Equals(string.Empty))
                        img = ImageCacher.GetImage(path);
                    else
                        img = new Bitmap(Properties.Resources.Leaf, new Size(110, 110));
                }
                else
                {
                    string path = Utilities.GetVillagerImage(V[j].GetInternalName());
                    if (!path.Equals(string.Empty))
                        img = ImageCacher.GetImage(path);
                    else
                        img = new Bitmap(Properties.Resources.Leaf, new Size(110, 110));
                }
                villagerButton[j].Image = new Bitmap(img, new Size(110, 110));

            }
            if (!clear)
            {
                if (selectedVillagerButton != null)
                {
                    selectedVillagerButton.BackColor = Color.LightSeaGreen;

                    int i = Int16.Parse(selectedVillagerButton.Tag.ToString());

                    if (VillagerHeaderIgnore.Checked)
                    {
                        VillagerControlPanel.Enabled = true;
                    }
                    else
                    {
                        if (VillagerLoading == false)
                            VillagerControlPanel.Enabled = true;
                        else
                            VillagerControlPanel.Enabled = false;

                        if (V[i].MoveInFlag == 0xC || V[i].MoveInFlag == 0xB)
                        {
                            if (V[i].IsReal())
                                VillagerControlPanel.Enabled = false;
                        }
                    }

                    VillagerIndex.Text = V[i].Index.ToString();
                    VillagerName.Text = V[i].GetRealName();
                    VillagerIName.Text = V[i].GetInternalName();
                    VillagerPersonality.Text = V[i].GetPersonality();
                    VillagerFriendship.Text = V[i].Friendship[0].ToString();
                    VillagerHouseIndex.Text = V[i].HouseIndex.ToString();
                    if (V[i].HouseIndex < 0 && V[i].IsReal())
                    {
                        VillagerHouseIndex.ForeColor = Color.Tomato;
                        VillagerHouseIndexLabel.ForeColor = Color.Tomato;
                    }
                    else
                    {
                        VillagerHouseIndex.ForeColor = Color.White;
                        VillagerHouseIndexLabel.ForeColor = Color.White;
                    }
                    VillagerCatchphrase.Text = Encoding.Unicode.GetString(V[i].Catchphrase, 0, 44);

                    VillagerMoveInFlag.Text = "0x" + V[i].MoveInFlag.ToString("X");
                    VillagerAbandonedHouseFlag.Text = "0x" + V[i].AbandonedHouseFlag.ToString("X");
                    VillagerForceMoveoutFlag.Text = "0x" + V[i].ForceMoveOutFlag.ToString("X");
                    VillagerHeader.Text = Utilities.ByteToHexString(V[i].GetHeader());
                }
            }

            //if (sound)
            //System.Media.SystemSounds.Asterisk.Play();
        }

        private void CleanVillagerPage()
        {
            if (villagerButton != null)
            {
                for (int i = 0; i < 10; i++)
                    VillagerLargePanel.Controls.Remove(villagerButton[i]);
            }

            V = null;
            villagerButton = null;
            VillagerFirstLoad = true;
            VillagerNowLoadingPanel.Location = new Point(170, 150);
        }

        private void VillagerRefreshButton_Click(object sender, EventArgs e)
        {
            if (villagerButton != null)
            {
                for (int i = 0; i < 10; i++)
                    VillagerLargePanel.Controls.Remove(villagerButton[i]);
            }

            Thread loadAllVillagerThread = new(LoadAllVillager);
            loadAllVillagerThread.Start();
        }

        private void FriendshipButton_Click(object sender, EventArgs e)
        {
            if (VillagerIndex.Text == "")
                return;
            int i = Int16.Parse(VillagerIndex.Text);

            Image img;
            string path;
            if (V[i].GetRealName() == "ERROR")
            {
                path = Utilities.GetVillagerImage(V[i].GetRealName());
            }
            else
            {
                path = Utilities.GetVillagerImage(V[i].GetInternalName());
            }

            if (!path.Equals(string.Empty))
                img = ImageCacher.GetImage(path);
            else
                img = new Bitmap(Properties.Resources.Leaf, new Size(128, 128));

            Friendship friendship = new(i, socket, usb, img, V[i], sound);
            friendship.ShowDialog();

            RefreshVillagerUI(false);
        }

        private void VillagerCatchphraseSetButton_Click(object sender, EventArgs e)
        {
            if (VillagerIndex.Text == "")
                return;
            int i = Int16.Parse(VillagerIndex.Text);
            byte[] phrase = new byte[44];
            byte[] temp = Encoding.Unicode.GetBytes(VillagerCatchphrase.Text);

            for (int j = 0; j < temp.Length; j++)
            {
                phrase[j] = temp[j];
            }

            Utilities.SetCatchphrase(socket, usb, i, phrase);

            V[i].Catchphrase = phrase;
            RefreshVillagerUI(false);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void VillagerCatchphraseClearButton_Click(object sender, EventArgs e)
        {
            if (VillagerIndex.Text == "")
                return;
            int i = Int16.Parse(VillagerIndex.Text);
            byte[] phrase = new byte[44];

            Utilities.SetCatchphrase(socket, usb, i, phrase);

            V[i].Catchphrase = phrase;
            RefreshVillagerUI(false);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void ReplaceVillagerButton_Click(object sender, EventArgs e)
        {
            if (VillagerIndex.Text == "")
                return;
            int i = Int16.Parse(VillagerIndex.Text);

            int j = V[i].HouseIndex;

            if (j > 9)
                return;

            if (j < 0)
            {
                var emptyHouseNumber = FindEmptyHouse();
                if (emptyHouseNumber >= 0)
                    j = emptyHouseNumber;
                else
                    return;
            }

            if (VillagerReplaceSelector.SelectedIndex < 0)
                return;
            string[] lines = VillagerReplaceSelector.SelectedItem.ToString()?.Split([" "], StringSplitOptions.None);
            //byte[] IName = Encoding.Default.GetBytes(lines[lines.Length - 1]);
            if (lines != null)
            {
                string IName = lines[lines.Length - 1];
                string RealName = lines[0];
                string IVpath = Utilities.villagerPath + IName + ".nhv2";
                string RVpath = Utilities.villagerPath + RealName + ".nhv2";
                byte[] villagerData;
                byte[] houseData;

                if (CheckDuplicate(IName))
                {
                    DialogResult dialogResult = MyMessageBox.Show(RealName + " is currently living on your island!" +
                                                                  "                                                   \nAre you sure you want to continue the replacement?" +
                                                                  "                                                   \nNote that the game will attempt to remove any duplicated villager!", "Villager already exists!", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (dialogResult == DialogResult.No)
                    {
                        return;
                    }
                }

                if (File.Exists(IVpath))
                {
                    Debug.Print("FOUND: " + IVpath);
                    villagerData = File.ReadAllBytes(IVpath);
                }
                else if (File.Exists(RVpath))
                {
                    Debug.Print("FOUND: " + RVpath);
                    villagerData = File.ReadAllBytes(RVpath);
                }
                else
                {
                    MessageBox.Show("Villager files \"" + IName + ".nhv2\" " + "/ \"" + RealName + ".nhv2\" " + "not found!", @"Villager file not found");
                    return;
                }

                string IHpath = Utilities.villagerPath + IName + ".nhvh2";
                string RHpath = Utilities.villagerPath + RealName + ".nhvh2";
                if (File.Exists(IHpath))
                {
                    Debug.Print("FOUND: " + IHpath);
                    houseData = File.ReadAllBytes(IHpath);
                }
                else if (File.Exists(RHpath))
                {
                    Debug.Print("FOUND: " + RHpath);
                    houseData = File.ReadAllBytes(RHpath);
                }
                else
                {
                    MessageBox.Show("Villager house files \"" + IName + ".nhvh2\" " + "/ \"" + RealName + ".nhvh2\" " + "not found!", @"House file not found");
                    return;
                }

                string msg = "Replacing Villager...";

                Thread loadBothThread = new(delegate () { LoadBoth(i, j, villagerData, houseData, msg); });
                loadBothThread.Start();
            }
        }

        private void LoadBoth(int i, int j, byte[] villager, byte[] house, string msg)
        {
            if (villager.Length != Utilities.VillagerSize)
            {
                MessageBox.Show(@"Villager file size incorrect!", @"Villager file invalid");
                return;
            }
            if (house.Length != Utilities.VillagerHouseSize)
            {
                MessageBox.Show(@"House file size incorrect!", @"House file invalid");
                return;
            }

            ShowVillagerWait((int)Utilities.VillagerSize * 2 + (int)Utilities.VillagerHouseSize * 2, msg);

            VillagerLoading = true;

            byte[] modifiedVillager = villager;
            if (!VillagerHeaderIgnore.Checked)
            {
                if (header[0] != 0x0 && header[1] != 0x0 && header[2] != 0x0)
                {
                    Buffer.BlockCopy(header, 0x0, modifiedVillager, 0x4, 52);
                }
            }

            V[i].LoadData(modifiedVillager);

            V[i].AbandonedHouseFlag = Convert.ToInt32(villager[Utilities.VillagerMoveoutOffset]);
            V[i].ForceMoveOutFlag = Convert.ToInt32(villager[Utilities.VillagerForceMoveoutOffset]);

            byte[] phrase = new byte[44];
            Buffer.BlockCopy(villager, (int)Utilities.VillagerCatchphraseOffset, phrase, 0x0, 44);
            V[i].Catchphrase = phrase;


            byte[] modifiedHouse = house;

            byte h = (Byte)i;
            modifiedHouse[Utilities.VillagerHouseOwnerOffset] = h;
            V[i].HouseIndex = j;
            HouseList[j] = i;

            Utilities.LoadVillager(socket, usb, i, modifiedVillager, ref counter);
            Utilities.LoadHouse(socket, usb, j, modifiedHouse, ref counter);

            Invoke((MethodInvoker)delegate
            {
                RefreshVillagerUI(false);
            });

            VillagerLoading = false;

            HideVillagerWait();

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void ForcedMoveoutButton_Click(object sender, EventArgs e)
        {
            if (VillagerIndex.Text == "")
                return;
            int i = Int16.Parse(VillagerIndex.Text);

            Utilities.SetMoveout(socket, usb, i);

            V[i].AbandonedHouseFlag = 2;
            V[i].ForceMoveOutFlag = 1;
            V[i].InvitedFlag = 0;
            RefreshVillagerUI(false);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void IrregularMoveoutButton_Click(object sender, EventArgs e)
        {
            if (VillagerIndex.Text == "")
                return;
            int i = Int16.Parse(VillagerIndex.Text);

            Utilities.SetMoveout(socket, usb, i, "2", "0");

            V[i].AbandonedHouseFlag = 2;
            V[i].ForceMoveOutFlag = 0;
            V[i].InvitedFlag = 0;
            RefreshVillagerUI(false);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void CancelMoveoutButton_Click(object sender, EventArgs e)
        {
            if (VillagerIndex.Text == "")
                return;
            int i = Int16.Parse(VillagerIndex.Text);

            Utilities.SetMoveout(socket, usb, i, "0", "0");

            V[i].AbandonedHouseFlag = 0;
            V[i].ForceMoveOutFlag = 0;
            V[i].InvitedFlag = 0;
            RefreshVillagerUI(false);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void ForcedMoveoutAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                Utilities.SetMoveout(socket, usb, i);

                V[i].AbandonedHouseFlag = 2;
                V[i].ForceMoveOutFlag = 1;
                V[i].InvitedFlag = 0;
            }
            RefreshVillagerUI(false);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void IrregularMoveoutAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                Utilities.SetMoveout(socket, usb, i, "2", "0");

                V[i].AbandonedHouseFlag = 2;
                V[i].ForceMoveOutFlag = 0;
                V[i].InvitedFlag = 0;
            }
            RefreshVillagerUI(false);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void CancelMoveoutAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                Utilities.SetMoveout(socket, usb, i, "0", "0");

                V[i].AbandonedHouseFlag = 0;
                V[i].ForceMoveOutFlag = 0;
                V[i].InvitedFlag = 0;
            }
            RefreshVillagerUI(false);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void SaveVillagerButton_Click(object sender, EventArgs e)
        {
            if (VillagerIndex.Text == "")
                return;
            int i = Int16.Parse(VillagerIndex.Text);

            SaveFileDialog file = new()
            {
                Filter = @"New Horizons Villager (*.nhv2)|*.nhv2",
                FileName = V[i].GetInternalName() + ".nhv2",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            string savepath;

            if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
            else
                savepath = config.AppSettings.Settings["LastSave"].Value;

            if (Directory.Exists(savepath))
            {
                file.InitialDirectory = savepath;
            }
            else
            {
                file.InitialDirectory = @"C:\";
            }

            if (file.ShowDialog() != DialogResult.OK)
                return;

            string[] temp = file.FileName.Split('\\');
            string path = "";
            for (int j = 0; j < temp.Length - 1; j++)
                path = path + temp[j] + "\\";

            config.AppSettings.Settings["LastSave"].Value = path;
            config.Save(ConfigurationSaveMode.Minimal);

            Thread dumpThread = new(delegate () { SaveVillager(i, file); });
            dumpThread.Start();
        }

        private void SaveVillager(int i, SaveFileDialog file)
        {
            ShowVillagerWait((int)Utilities.VillagerSize, "Dumping " + V[i].GetRealName() + " ...");

            VillagerLoading = true;

            byte[] villagerData = Utilities.GetVillager(socket, usb, i, (int)Utilities.VillagerSize, ref counter);
            File.WriteAllBytes(file.FileName, villagerData);

            byte[] checkData = File.ReadAllBytes(file.FileName);
            byte[] checkHeader = new byte[52];
            if (header[0] != 0x0 && header[1] != 0x0 && header[2] != 0x0)
            {
                Buffer.BlockCopy(checkData, 0x4, checkHeader, 0x0, 52);
            }

            if (!checkHeader.SequenceEqual(header))
            {
                Debug.Print(Utilities.ByteToHexString(checkHeader));
                Debug.Print(Utilities.ByteToHexString(header));
                MessageBox.Show(@"Wait something is wrong here!? Header Mismatch!", @"Warning");
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            VillagerLoading = false;

            HideVillagerWait();
        }

        private void SaveHouseButton_Click(object sender, EventArgs e)
        {
            if (VillagerIndex.Text == "")
                return;
            int i = Int16.Parse(VillagerIndex.Text);

            int j = V[i].HouseIndex;

            if (j < 0 || j > 9)
                return;

            SaveFileDialog file = new()
            {
                Filter = @"New Horizons Villager House (*.nhvh2)|*.nhvh2",
                FileName = V[i].GetInternalName() + ".nhvh2",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            string savepath;

            if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
            else
                savepath = config.AppSettings.Settings["LastSave"].Value;

            if (Directory.Exists(savepath))
            {
                file.InitialDirectory = savepath;
            }
            else
            {
                file.InitialDirectory = @"C:\";
            }

            if (file.ShowDialog() != DialogResult.OK)
                return;

            string[] temp = file.FileName.Split('\\');
            string path = "";
            for (int k = 0; k < temp.Length - 1; k++)
                path = path + temp[k] + "\\";

            config.AppSettings.Settings["LastSave"].Value = path;
            config.Save(ConfigurationSaveMode.Minimal);

            Thread dumpThread = new(delegate () { SaveHouse(i, j, file); });
            dumpThread.Start();
        }

        private void SaveHouse(int i, int j, SaveFileDialog file)
        {
            ShowVillagerWait((int)Utilities.VillagerHouseSize, "Dumping " + V[i].GetRealName() + "'s House ...");

            byte[] house = Utilities.GetHouse(socket, usb, j, ref counter);
            File.WriteAllBytes(file.FileName, house);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideVillagerWait();
        }

        private void LoadVillagerButton_Click(object sender, EventArgs e)
        {
            if (VillagerIndex.Text == "")
                return;
            int i = Int16.Parse(VillagerIndex.Text);

            OpenFileDialog file = new()
            {
                Filter = @"New Horizons Villager (*.nhv2)|*.nhv2|New Horizons Villager (*.nhv)|*.nhv",
                //FileName = V[i].GetInternalName() + ".nhv",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            string savepath;

            if (config.AppSettings.Settings["LastLoad"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
            else
                savepath = config.AppSettings.Settings["LastLoad"].Value;

            if (Directory.Exists(savepath))
            {
                file.InitialDirectory = savepath;
            }
            else
            {
                file.InitialDirectory = @"C:\";
            }

            if (file.ShowDialog() != DialogResult.OK)
                return;

            string[] temp = file.FileName.Split('\\');
            string path = "";
            for (int j = 0; j < temp.Length - 1; j++)
                path = path + temp[j] + "\\";

            config.AppSettings.Settings["LastLoad"].Value = path;
            config.Save(ConfigurationSaveMode.Minimal);

            byte[] data = File.ReadAllBytes(file.FileName);

            if (data.Length == Utilities.VillagerOldSize)
            {
                data = ConvertToNew(data);
            }

            if (data.Length != Utilities.VillagerSize)
            {
                MessageBox.Show(@"Villager file size incorrect!", @"Villager file invalid");
                return;
            }

            string msg = "Loading villager...";

            Thread loadVillagerThread = new(delegate () { LoadVillager(i, data, msg); });
            loadVillagerThread.Start();
        }

        private void LoadVillager(int i, byte[] villager, string msg)
        {
            ShowVillagerWait((int)Utilities.VillagerSize * 2, msg);

            VillagerLoading = true;

            byte[] modifiedVillager = villager;
            if (!VillagerHeaderIgnore.Checked)
            {
                if (header[0] != 0x0 && header[1] != 0x0 && header[2] != 0x0)
                {
                    Buffer.BlockCopy(header, 0x0, modifiedVillager, 0x4, 52);
                }
            }

            V[i].LoadData(modifiedVillager);

            //byte[] move = Utilities.GetMoveout(s, bot, i, (int)0x33, ref counter);
            V[i].AbandonedHouseFlag = Convert.ToInt32(villager[Utilities.VillagerMoveoutOffset]);
            V[i].ForceMoveOutFlag = Convert.ToInt32(villager[Utilities.VillagerForceMoveoutOffset]);

            byte[] phrase = new byte[44];
            //buffer.BlockCopy(Bank01to20, slotOffset, slotBytes, 0x0, 0x4);
            Buffer.BlockCopy(villager, (int)Utilities.VillagerCatchphraseOffset, phrase, 0x0, 44);
            V[i].Catchphrase = phrase;

            Utilities.LoadVillager(socket, usb, i, modifiedVillager, ref counter);

            Invoke((MethodInvoker)delegate
            {
                RefreshVillagerUI(false);
            });

            VillagerLoading = false;

            HideVillagerWait();
        }

        private void LoadHouseButton_Click(object sender, EventArgs e)
        {
            if (VillagerIndex.Text == "")
                return;
            int i = Int16.Parse(VillagerIndex.Text);

            int j = V[i].HouseIndex;

            if (j < 0 || j > 9)
                return;

            OpenFileDialog file = new()
            {
                Filter = @"New Horizons Villager House (*.nhvh2)|*.nhvh2",
                //FileName = V[i].GetInternalName() + ".nhvh2",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            string savepath;

            if (config.AppSettings.Settings["LastLoad"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
            else
                savepath = config.AppSettings.Settings["LastLoad"].Value;

            if (Directory.Exists(savepath))
            {
                file.InitialDirectory = savepath;
            }
            else
            {
                file.InitialDirectory = @"C:\";
            }

            if (file.ShowDialog() != DialogResult.OK)
                return;

            string[] temp = file.FileName.Split('\\');
            string path = "";
            for (int k = 0; k < temp.Length - 1; k++)
                path = path + temp[k] + "\\";

            config.AppSettings.Settings["LastLoad"].Value = path;
            config.Save(ConfigurationSaveMode.Minimal);

            byte[] data = File.ReadAllBytes(file.FileName);

            if (data.Length == Utilities.VillagerHouseOldSize)
            {
                data = ConvertToNewHouse(data);
            }

            if (data.Length != Utilities.VillagerHouseSize)
            {
                MessageBox.Show(@"House file size incorrect!", @"House file invalid");
                return;
            }

            string msg = "Loading house...";

            Thread loadHouseThread = new(delegate () { LoadHouse(i, j, data, msg); });
            loadHouseThread.Start();

        }

        private void LoadHouse(int i, int j, byte[] house, string msg)
        {
            ShowVillagerWait((int)Utilities.VillagerHouseSize * 2, msg);

            byte[] modifiedHouse = house;

            byte h = (Byte)i;
            modifiedHouse[Utilities.VillagerHouseOwnerOffset] = h;
            V[i].HouseIndex = j;
            HouseList[j] = i;

            Utilities.LoadHouse(socket, usb, j, modifiedHouse, ref counter);

            Invoke((MethodInvoker)delegate
            {
                RefreshVillagerUI(false);
            });

            HideVillagerWait();
        }

        private void ReadMysVillagerButton_Click(object sender, EventArgs e)
        {
            byte[] IName = Utilities.GetMysVillagerName(socket, usb);
            string StrName = Encoding.ASCII.GetString(Utilities.ByteTrim(IName));
            string RealName = Utilities.GetVillagerRealName(StrName);

            Image img;
            if (RealName == "ERROR")
            {
                string path = Utilities.GetVillagerImage(RealName);
                if (!path.Equals(string.Empty))
                    img = ImageCacher.GetImage(path);
                else
                    img = new Bitmap(Properties.Resources.Leaf, new Size(110, 110));
                MysVillagerDisplay.Text = "";
            }
            else
            {
                string path = Utilities.GetVillagerImage(StrName);
                if (!path.Equals(string.Empty))
                    img = ImageCacher.GetImage(path);
                else
                    img = new Bitmap(Properties.Resources.Leaf, new Size(110, 110));
                MysVillagerDisplay.Text = RealName + " : " + StrName;
            }
            MysVillagerDisplay.Image = new Bitmap(img, new Size(110, 110));
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void ReplaceMysVillagerButton_Click(object sender, EventArgs e)
        {
            if (MysVillagerReplaceSelector.SelectedIndex < 0)
                return;
            string[] lines = MysVillagerReplaceSelector.SelectedItem.ToString()?.Split([" "], StringSplitOptions.None);

            if (lines != null)
            {
                byte[] IName = Encoding.Default.GetBytes(lines[lines.Length - 1]);
                byte[] species = [Utilities.CheckSpecies[(lines[lines.Length - 1]).Substring(0, 3)]];
                Utilities.SetMysVillager(socket, usb, IName, species, ref counter);

                Image img;
                string path = Utilities.GetVillagerImage(lines[lines.Length - 1]);
                if (!path.Equals(String.Empty))
                    img = ImageCacher.GetImage(path);
                else
                    img = new Bitmap(Properties.Resources.Leaf, new Size(110, 110));
                MysVillagerDisplay.Text = lines[0] + " : " + lines[lines.Length - 1];
                MysVillagerDisplay.Image = new Bitmap(img, new Size(110, 110));
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
        }

        private int FindEmptyHouse()
        {
            for (int i = 0; i < 10; i++)
            {
                if (HouseList[i] == 255)
                    return i;
            }
            return -1;
        }

        private bool CheckDuplicate(string iName)
        {
            if (iName == "non00")
                return false;
            for (int i = 0; i < 10; i++)
            {
                if (V[i].GetInternalName() == iName)
                    return true;
            }
            return false;
        }

        private static byte[] ConvertToNew(byte[] oldVillager)
        {
            byte[] newVillager = new byte[Utilities.VillagerSize];

            Array.Copy(oldVillager, 0, newVillager, 0, 0x2f84);

            for (int i = 0; i < 160; i++)
            {
                var src = 0x2f84 + (0x14C * i);
                var dest = 0x2f84 + (0x158 * i);

                Array.Copy(oldVillager, src, newVillager, dest, 0x14C);
            }

            Array.Copy(oldVillager, 0xff04, newVillager, 0x10684, oldVillager.Length - 0xff04);

            return newVillager;
        }

        public static byte[] ConvertToNewHouse(byte[] oldHouse)
        {
            byte[] newHouse = new byte[Utilities.VillagerHouseSize];

            byte[] NoItem = [0xFE, 0xFF];

            byte[] NewHouseFooter =
            [
                0x4B, 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4B, 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x9B, 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x00, 0xF8, 0x00, 0xF8, 0x00, 0x40,
                0x10, 0x00, 0x00, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xFE, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            ];

            Array.Copy(oldHouse, 0, newHouse, 0, Utilities.VillagerHouseOldSize);

            for (int i = 0; i < 236; i++)
            {
                Array.Copy(NoItem, 0x0, newHouse, 0x1D8 + (i * 0xC), NoItem.Length);
            }

            Array.Copy(NewHouseFooter, 0x0, newHouse, 0x1270, NewHouseFooter.Length);

            return newHouse;
        }

        private void ReplaceVillagerSearchBox_Click(object sender, EventArgs e)
        {
            ReplaceVilllagerSearchBox.Text = "";
            ReplaceVilllagerSearchBox.ForeColor = Color.White;
        }

        private void ReplaceMysVillagerSearchBox_Click(object sender, EventArgs e)
        {
            ReplaceMysVilllagerSearchBox.Text = "";
            ReplaceMysVilllagerSearchBox.ForeColor = Color.White;
        }

        private void ReplaceVillagerSearchBox_Leave(object sender, EventArgs e)
        {
            if (ReplaceVilllagerSearchBox.Text == "")
            {
                ReplaceVilllagerSearchBox.Text = @"Search...";
                ReplaceVilllagerSearchBox.ForeColor = Color.FromArgb(255, 114, 118, 125);
            }
        }

        private void ReplaceMysVillagerSearchBox_Leave(object sender, EventArgs e)
        {
            if (ReplaceMysVilllagerSearchBox.Text == "")
            {
                ReplaceMysVilllagerSearchBox.Text = @"Search...";
                ReplaceMysVilllagerSearchBox.ForeColor = Color.FromArgb(255, 114, 118, 125);
            }
        }

        private void ReplaceVillagerSearchBox_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < VillagerReplaceSelector.Items.Count; i++)
            {
                string[] lines = VillagerReplaceSelector.Items[i].ToString()?.Split([" "], StringSplitOptions.None);
                if (lines != null)
                {
                    string RealName = lines[0];
                    if (ReplaceVilllagerSearchBox.Text.Split([" "], StringSplitOptions.None)[0].Equals(RealName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        VillagerReplaceSelector.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void ReplaceMysVillagerSearchBox_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < MysVillagerReplaceSelector.Items.Count; i++)
            {
                string[] lines = MysVillagerReplaceSelector.Items[i].ToString()?.Split([" "], StringSplitOptions.None);
                //byte[] IName = Encoding.Default.GetBytes(lines[lines.Length - 1]);
                //string IName = lines[lines.Length - 1];
                if (lines != null)
                {
                    string RealName = lines[0];
                    if (ReplaceMysVilllagerSearchBox.Text.Split([" "], StringSplitOptions.None)[0].Equals(RealName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        MysVillagerReplaceSelector.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        #endregion

        private void InventorySlot_MouseHover(object sender, EventArgs e)
        {
            var button = (InventorySlot)sender;
            if (!button.IsEmpty())
            {
                ButtonToolTip.SetToolTip(button, button.DisplayItemName() +
                                                "\n\nID : " + button.DisplayItemID() + "" +
                                                "\nCount : " + button.DisplayItemData() +
                                                "\nFlag : " + button.GetFlag0() + " " + button.GetFlag1());
            }
        }

        private void AmountOrCountTextbox_DoubleClick(object sender, EventArgs e)
        {
            if (currentPanel == RecipeModePanel || currentPanel == FlowerModePanel)
                return;

            string id = Utilities.PrecedingZeros(IDTextbox.Text, 4);

            UInt16 intId = Convert.ToUInt16("0x" + IDTextbox.Text, 16);

            if (Utilities.itemkind.ContainsKey(id))
            {
                if (!Utilities.CountByKind.TryGetValue(Utilities.itemkind[id], out int value))
                {
                    if (HexModeButton.Tag.ToString() == "Normal")
                        AmountOrCountTextbox.Text = @"1";
                    else
                        AmountOrCountTextbox.Text = Utilities.PrecedingZeros("0", 8);
                }
                else
                {
                    if (ItemAttr.HasFenceWithVariation(intId))  // Fence Variation
                    {
                        string hexValue = "00000000";
                        if (HexModeButton.Tag.ToString() == "Normal")
                        {
                            int decValue = Convert.ToInt32(AmountOrCountTextbox.Text) - 1;
                            if (decValue >= 0)
                                hexValue = Utilities.PrecedingZeros(decValue.ToString("X"), 8);
                        }
                        else
                            hexValue = Utilities.PrecedingZeros(AmountOrCountTextbox.Text, 8);


                        string front = Utilities.PrecedingZeros(hexValue, 8).Substring(0, 4);
                        //string back = Utilities.precedingZeros(hexValue, 8).Substring(4, 4);

                        if (HexModeButton.Tag.ToString() == "Normal")
                        {
                            int decValue = value - 1;
                            if (decValue >= 0)
                                AmountOrCountTextbox.Text = (int.Parse(front + Utilities.PrecedingZeros(decValue.ToString("X"), 4), System.Globalization.NumberStyles.HexNumber) + 1).ToString();
                            else
                                AmountOrCountTextbox.Text = (int.Parse(front + Utilities.PrecedingZeros("0", 4), System.Globalization.NumberStyles.HexNumber) + 1).ToString();
                        }
                        else
                        {
                            int decValue = value - 1;
                            if (decValue >= 0)
                                AmountOrCountTextbox.Text = front + Utilities.PrecedingZeros(decValue.ToString("X"), 4);
                            else
                                AmountOrCountTextbox.Text = front + Utilities.PrecedingZeros("0", 4);
                        }
                    }
                    else
                    {
                        if (HexModeButton.Tag.ToString() == "Normal")
                        {
                            AmountOrCountTextbox.Text = value.ToString();
                        }
                        else
                        {
                            int decValue = value - 1;
                            if (decValue >= 0)
                                AmountOrCountTextbox.Text = Utilities.PrecedingZeros(decValue.ToString("X"), 8);
                            else
                                AmountOrCountTextbox.Text = Utilities.PrecedingZeros("0", 8);
                        }
                    }
                }
            }
            else
            {
                if (HexModeButton.Tag.ToString() == "Normal")
                    AmountOrCountTextbox.Text = @"1";
                else
                    AmountOrCountTextbox.Text = Utilities.PrecedingZeros("0", 8);
            }

            AmountOrCountTextbox_KeyUp(sender, null);
        }

        private void AmountOrCountTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (IDTextbox.Text == "" | AmountOrCountTextbox.Text == "")
                return;

            string hexValue = "00000000";
            if (HexModeButton.Tag.ToString() == "Normal")
            {
                int decValue = Convert.ToInt32(AmountOrCountTextbox.Text) - 1;
                if (decValue >= 0)
                    hexValue = Utilities.PrecedingZeros(decValue.ToString("X"), 8);
            }
            else
                hexValue = Utilities.PrecedingZeros(AmountOrCountTextbox.Text, 8);

            if (IDTextbox.Text == "")
                return;

            UInt16 intId = Convert.ToUInt16("0x" + IDTextbox.Text, 16);

            string front = Utilities.PrecedingZeros(hexValue, 8).Substring(0, 4);
            //string back = Utilities.precedingZeros(hexValue, 8).Substring(4, 4);

            if (IDTextbox.Text is "16A2") //recipe
            {
                SelectedItem.Setup(GetNameFromID(hexValue, recipeSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(hexValue), recipeSource), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
            }
            else if (IDTextbox.Text is "114A") // Money Tree
            {
                SelectedItem.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID(Utilities.Turn2bytes(hexValue), itemSource), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
            }
            else if (IDTextbox.Text is "315A" or "1618" or "342F") // Wall-Mounted
            {
                SelectedItem.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID(Utilities.Turn2bytes(hexValue), itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16)), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
            }
            else if (ItemAttr.HasFenceWithVariation(intId))  // Fence Variation
            {
                SelectedItem.Setup(GetNameFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + front, 16)), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());

                selection?.ReceiveID(IDTextbox.Text, languageSetting, Utilities.PrecedingZeros(hexValue, 8));
            }
            else
            {
                if (ItemAttr.HasGenetics(Convert.ToUInt16("0x" + IDTextbox.Text, 16)))
                {
                    string value = AmountOrCountTextbox.Text;
                    int length = value.Length;
                    string firstByte;
                    string secondByte;
                    if (length < 2)
                    {
                        firstByte = "0";
                        secondByte = value;
                    }
                    else
                    {
                        firstByte = value.Substring(length - 2, 1);
                        secondByte = value.Substring(length - 1, 1);
                    }

                    SetGeneComboBox(firstByte, secondByte);
                    GenePanel.Visible = true;
                }

                SelectedItem.Setup(GetNameFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());

                selection?.ReceiveID(IDTextbox.Text, languageSetting, Utilities.PrecedingZeros(hexValue, 8));
            }
            UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
        }

        private void IDTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (IDTextbox.Text == "" | AmountOrCountTextbox.Text == "")
                return;

            string hexValue = "00000000";
            if (HexModeButton.Tag.ToString() == "Normal")
            {
                int decValue = Convert.ToInt32(AmountOrCountTextbox.Text) - 1;
                if (decValue >= 0)
                    hexValue = Utilities.PrecedingZeros(decValue.ToString("X"), 8);
            }
            else
                hexValue = Utilities.PrecedingZeros(AmountOrCountTextbox.Text, 8);

            if (IDTextbox.Text == "")
                return;

            UInt16 intId = Convert.ToUInt16("0x" + IDTextbox.Text, 16);

            string front = Utilities.PrecedingZeros(hexValue, 8).Substring(0, 4);
            //string back = Utilities.precedingZeros(hexValue, 8).Substring(4, 4);

            if (IDTextbox.Text is "16A2") //recipe
            {
                SelectedItem.Setup(GetNameFromID(hexValue, recipeSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(hexValue), recipeSource), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
            }
            else if (IDTextbox.Text is "114A") // Money Tree
            {
                SelectedItem.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID(Utilities.Turn2bytes(hexValue), itemSource), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
            }
            else if (IDTextbox.Text is "315A" or "1618" or "342F") // Wall-Mounted
            {
                SelectedItem.Setup(GetNameFromID(IDTextbox.Text, itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID(Utilities.Turn2bytes(hexValue), itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16)), SelectedItem.GetFlag0(), SelectedItem.GetFlag1());
            }
            else if (ItemAttr.HasFenceWithVariation(intId))  // Fence Variation
            {
                SelectedItem.Setup(GetNameFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + front, 16)), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());

                selection?.ReceiveID(IDTextbox.Text, languageSetting, Utilities.PrecedingZeros(hexValue, 8));
            }
            else
            {
                if (ItemAttr.HasGenetics(Convert.ToUInt16("0x" + IDTextbox.Text, 16)))
                {
                    string value = AmountOrCountTextbox.Text;
                    int length = value.Length;
                    string firstByte;
                    string secondByte;
                    if (length < 2)
                    {
                        firstByte = "0";
                        secondByte = value;
                    }
                    else
                    {
                        firstByte = value.Substring(length - 2, 1);
                        secondByte = value.Substring(length - 1, 1);
                    }

                    SetGeneComboBox(firstByte, secondByte);
                    GenePanel.Visible = true;
                }

                SelectedItem.Setup(GetNameFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource), Convert.ToUInt16("0x" + IDTextbox.Text, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(IDTextbox.Text), itemSource, Convert.ToUInt32("0x" + hexValue, 16)), true, "", SelectedItem.GetFlag0(), SelectedItem.GetFlag1());

                selection?.ReceiveID(IDTextbox.Text, languageSetting, Utilities.PrecedingZeros(hexValue, 8));
            }
            UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
        }

        private void IDTextbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (((RichTextBox)sender).Modified)
                {
                    if (IDTextbox.Text != "")
                    {
                        if (ItemAttr.HasGenetics(Convert.ToUInt16("0x" + IDTextbox.Text, 16)))
                        {
                            if (HexModeButton.Tag.ToString() == "Normal")
                            {
                                HexModeButton_Click(sender, e);
                            }

                            string value = AmountOrCountTextbox.Text;
                            int length = value.Length;
                            string firstByte;
                            string secondByte;
                            if (length < 2)
                            {
                                firstByte = "0";
                                secondByte = value;
                            }
                            else
                            {
                                firstByte = value.Substring(length - 2, 1);
                                secondByte = value.Substring(length - 1, 1);
                            }

                            SetGeneComboBox(firstByte, secondByte);
                            GenePanel.Visible = true;
                        }
                        else
                        {
                            GenePanel.Visible = false;
                        }

                        if (IDTextbox.Text is "315A" or "1618" or "342F")
                        {
                            WallMountMsg.Visible = true;
                        }
                        else
                        {
                            WallMountMsg.Visible = false;
                        }
                    }
                    else
                    {
                        GenePanel.Visible = false;
                        WallMountMsg.Visible = false;
                    }
                }
                else
                {
                    GenePanel.Visible = false;
                    WallMountMsg.Visible = false;
                }

            }
            catch
            {
                IDTextbox.Clear();
            }
        }

        private void HexModeButton_Click(object sender, EventArgs e)
        {
            if (HexModeButton.Tag.ToString() == "Normal")
            {
                AmountOrCountLabel.Text = @"Hex Value";
                HexModeButton.Tag = "Hex";
                HexModeButton.Text = @"Normal Mode";
                if (AmountOrCountTextbox.Text != "")
                {
                    int decValue = Convert.ToInt32(AmountOrCountTextbox.Text) - 1;
                    string hexValue;
                    if (decValue < 0)
                        hexValue = "0";
                    else
                        hexValue = decValue.ToString("X");
                    AmountOrCountTextbox.Text = Utilities.PrecedingZeros(hexValue, 8);
                }
            }
            else
            {
                AmountOrCountLabel.Text = @"Amount";
                HexModeButton.Tag = "Normal";
                HexModeButton.Text = @"Hex Mode";
                if (AmountOrCountTextbox.Text != "")
                {
                    string hexValue = AmountOrCountTextbox.Text;
                    int decValue = Convert.ToInt32(hexValue, 16) + 1;
                    AmountOrCountTextbox.Text = decValue.ToString();
                }
            }
        }

        private void Hex_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')))
            {
                e.Handled = true;
            }
            if (c >= 'a' && c <= 'f') e.KeyChar = char.ToUpper(c);
        }

        private void Dec_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!((c >= '0' && c <= '9')))
            {
                e.Handled = true;
            }
        }

        private void DecAndHex_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (HexModeButton.Tag.ToString() == "Normal")
            {
                if (!((c >= '0' && c <= '9')))
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')))
                {
                    e.Handled = true;
                }
                if (c >= 'a' && c <= 'f') e.KeyChar = char.ToUpper(c);
            }
        }

        #region Keyboard Hotkey

        public void KeyboardKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "F2" || e.KeyCode.ToString() == "Insert")
            {
                if (selectedButton == null & (socket != null))
                {
                    int firstSlot = FindEmpty();
                    if (firstSlot > 0)
                    {
                        selectedSlot = firstSlot;
                        UpdateSlot(firstSlot);
                    }
                }

                if (currentPanel == ItemModePanel)
                {
                    NormalItemSpawn();
                }
                else if (currentPanel == RecipeModePanel)
                {
                    RecipeSpawn();
                }
                else if (currentPanel == FlowerModePanel)
                {
                    FlowerSpawn();
                }

                int nextSlot = FindEmpty();
                if (nextSlot > 0)
                {
                    selectedSlot = nextSlot;
                    UpdateSlot(nextSlot);
                }
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            else if (e.KeyCode.ToString() == "F1")
            {
                DeleteItem();
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            else if (e.KeyCode.ToString() == "F3")
            {
                CopyItem(sender, e);
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            else if (e.KeyCode.ToString() == "F12")
            {
                Text += @" DEBUG";
                DEBUGGING = true;

                OtherTabButton.Visible = true;
                CritterTabButton.Visible = true;
                MapDropperButton.Visible = true;
                RegeneratorButton.Visible = true;
                FreezerButton.Visible = true;
                RoadRollerButton.Visible = true;
                DodoHelperButton.Visible = true;
                BulldozerButton.Visible = true;
                chatButton.Visible = true;
            }
            else if (e.KeyCode.ToString() == "F10")
            {
                EmulatorButton_Click(null, null);
            }
            else if (e.KeyCode.ToString() == "End")
            {
                if (ItemGridView.CurrentRow == null)
                    return;
                if (currentPanel == ItemModePanel)
                {
                    if (ItemGridView.Rows.Count <= 0)
                    {
                        return;
                    }
                    if (ItemGridView.CurrentRow == null)
                    {
                        return;
                    }

                    if (ItemGridView.Rows.Count == 1)
                    {
                        lastRow = ItemGridView.Rows[ItemGridView.CurrentRow.Index];
                        ItemGridView.Rows[ItemGridView.CurrentRow.Index].Height = 160;

                        if (HexModeButton.Tag.ToString() == "Normal")
                        {
                            if (AmountOrCountTextbox.Text is "" or "0")
                            {
                                AmountOrCountTextbox.Text = @"1";
                            }
                        }
                        else
                        {
                            HexModeButton_Click(sender, e);
                        }

                        string hexValue = "0";
                        int decValue = int.Parse(AmountOrCountTextbox.Text) - 1;
                        if (decValue >= 0)
                            hexValue = decValue.ToString("X");

                        IDTextbox.Text = ItemGridView.Rows[ItemGridView.CurrentRow.Index].Cells["id"].Value.ToString();

                        SelectedItem.Setup(ItemGridView.Rows[ItemGridView.CurrentRow.Index].Cells[languageSetting].Value.ToString(), Convert.ToUInt16("0x" + ItemGridView.Rows[ItemGridView.CurrentRow.Index].Cells["id"].Value, 16), 0x0, GetImagePathFromID(ItemGridView.Rows[ItemGridView.CurrentRow.Index].Cells["id"].Value.ToString(), itemSource), true);
                        selection?.ReceiveID(Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4), languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                        UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
                    }
                    else if (ItemGridView.CurrentRow.Index + 1 < ItemGridView.Rows.Count)
                    {
                        if (lastRow != null)
                        {
                            lastRow.Height = 22;
                        }
                        lastRow = ItemGridView.Rows[ItemGridView.CurrentRow.Index + 1];
                        ItemGridView.Rows[ItemGridView.CurrentRow.Index + 1].Height = 160;

                        if (HexModeButton.Tag.ToString() == "Normal")
                        {
                            if (AmountOrCountTextbox.Text is "" or "0")
                            {
                                AmountOrCountTextbox.Text = @"1";
                            }
                        }
                        else
                        {
                            HexModeButton_Click(sender, e);
                            //AmountOrCountTextbox.Text = "1";
                        }

                        string hexValue = "0";
                        int decValue = int.Parse(AmountOrCountTextbox.Text) - 1;
                        if (decValue >= 0)
                            hexValue = decValue.ToString("X");

                        IDTextbox.Text = ItemGridView.Rows[ItemGridView.CurrentRow.Index + 1].Cells["id"].Value.ToString();

                        SelectedItem.Setup(ItemGridView.Rows[ItemGridView.CurrentRow.Index + 1].Cells[languageSetting].Value.ToString(), Convert.ToUInt16("0x" + ItemGridView.Rows[ItemGridView.CurrentRow.Index + 1].Cells["id"].Value, 16), 0x0, GetImagePathFromID(ItemGridView.Rows[ItemGridView.CurrentRow.Index + 1].Cells["id"].Value.ToString(), itemSource), true);
                        selection?.ReceiveID(Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4), languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                        UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());

                        ItemGridView.CurrentCell = ItemGridView.Rows[ItemGridView.CurrentRow.Index + 1].Cells[languageSetting];

                        //Debug.Print(ItemGridView.CurrentRow.Index.ToString());
                    }
                }
                else if (currentPanel == RecipeModePanel)
                {
                    if (RecipeGridView.Rows.Count <= 0)
                    {
                        return;
                    }
                    if (RecipeGridView.CurrentRow == null)
                    {
                        return;
                    }

                    if (RecipeGridView.Rows.Count == 1)
                    {
                        recipelastRow = RecipeGridView.Rows[RecipeGridView.CurrentRow.Index];
                        RecipeGridView.Rows[RecipeGridView.CurrentRow.Index].Height = 160;

                        RecipeIDTextbox.Text = RecipeGridView.Rows[RecipeGridView.CurrentRow.Index].Cells["id"].Value.ToString();

                        SelectedItem.Setup(RecipeGridView.Rows[RecipeGridView.CurrentRow.Index].Cells[languageSetting].Value.ToString(), 0x16A2, Convert.ToUInt32("0x" + RecipeGridView.Rows[RecipeGridView.CurrentRow.Index].Cells["id"].Value, 16), GetImagePathFromID(RecipeGridView.Rows[RecipeGridView.CurrentRow.Index].Cells["id"].Value.ToString(), recipeSource), true);
                        UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
                    }
                    else if (RecipeGridView.CurrentRow.Index + 1 < RecipeGridView.Rows.Count)
                    {
                        if (recipelastRow != null)
                        {
                            recipelastRow.Height = 22;
                        }

                        recipelastRow = RecipeGridView.Rows[RecipeGridView.CurrentRow.Index + 1];
                        RecipeGridView.Rows[RecipeGridView.CurrentRow.Index + 1].Height = 160;

                        RecipeIDTextbox.Text = RecipeGridView.Rows[RecipeGridView.CurrentRow.Index + 1].Cells["id"].Value.ToString();

                        SelectedItem.Setup(RecipeGridView.Rows[RecipeGridView.CurrentRow.Index + 1].Cells[languageSetting].Value.ToString(), 0x16A2, Convert.ToUInt32("0x" + RecipeGridView.Rows[RecipeGridView.CurrentRow.Index + 1].Cells["id"].Value, 16), GetImagePathFromID(RecipeGridView.Rows[RecipeGridView.CurrentRow.Index + 1].Cells["id"].Value.ToString(), recipeSource), true);
                        UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());

                        RecipeGridView.CurrentCell = RecipeGridView.Rows[RecipeGridView.CurrentRow.Index + 1].Cells[languageSetting];
                    }
                }
                else if (currentPanel == FlowerModePanel)
                {

                }
            }
            else if (e.KeyCode.ToString() == "Home")
            {
                if (currentPanel == ItemModePanel)
                {
                    if (ItemGridView.Rows.Count <= 0)
                    {
                        return;
                    }
                    if (ItemGridView.CurrentRow == null)
                    {
                        return;
                    }

                    if (ItemGridView.Rows.Count == 1)
                    {
                        lastRow = ItemGridView.Rows[ItemGridView.CurrentRow.Index];
                        ItemGridView.Rows[ItemGridView.CurrentRow.Index].Height = 160;

                        if (HexModeButton.Tag.ToString() == "Normal")
                        {
                            if (AmountOrCountTextbox.Text is "" or "0")
                            {
                                AmountOrCountTextbox.Text = @"1";
                            }
                        }
                        else
                        {
                            HexModeButton_Click(sender, e);
                            //AmountOrCountTextbox.Text = "1";
                        }

                        string hexValue = "0";
                        int decValue = int.Parse(AmountOrCountTextbox.Text) - 1;
                        if (decValue >= 0)
                            hexValue = decValue.ToString("X");

                        IDTextbox.Text = ItemGridView.Rows[ItemGridView.CurrentRow.Index].Cells["id"].Value.ToString();

                        SelectedItem.Setup(ItemGridView.Rows[ItemGridView.CurrentRow.Index].Cells[languageSetting].Value.ToString(), Convert.ToUInt16("0x" + ItemGridView.Rows[ItemGridView.CurrentRow.Index].Cells["id"].Value, 16), 0x0, GetImagePathFromID(ItemGridView.Rows[ItemGridView.CurrentRow.Index].Cells["id"].Value.ToString(), itemSource), true);
                        selection?.ReceiveID(Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4), languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                        UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
                    }
                    else if (ItemGridView.CurrentRow.Index > 0)
                    {
                        if (lastRow != null)
                        {
                            lastRow.Height = 22;
                        }

                        lastRow = ItemGridView.Rows[ItemGridView.CurrentRow.Index - 1];
                        ItemGridView.Rows[ItemGridView.CurrentRow.Index - 1].Height = 160;

                        if (HexModeButton.Tag.ToString() == "Normal")
                        {
                            if (AmountOrCountTextbox.Text is "" or "0")
                            {
                                AmountOrCountTextbox.Text = @"1";
                            }
                        }
                        else
                        {
                            HexModeButton_Click(sender, e);
                            //AmountOrCountTextbox.Text = "1";
                        }

                        string hexValue = "0";
                        int decValue = int.Parse(AmountOrCountTextbox.Text) - 1;
                        if (decValue >= 0)
                            hexValue = decValue.ToString("X");

                        IDTextbox.Text = ItemGridView.Rows[ItemGridView.CurrentRow.Index - 1].Cells["id"].Value.ToString();

                        SelectedItem.Setup(ItemGridView.Rows[ItemGridView.CurrentRow.Index - 1].Cells[languageSetting].Value.ToString(), Convert.ToUInt16("0x" + ItemGridView.Rows[ItemGridView.CurrentRow.Index - 1].Cells["id"].Value, 16), 0x0, GetImagePathFromID(ItemGridView.Rows[ItemGridView.CurrentRow.Index - 1].Cells["id"].Value.ToString(), itemSource), true);
                        selection?.ReceiveID(Utilities.PrecedingZeros(SelectedItem.FillItemID(), 4), languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                        UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());

                        ItemGridView.CurrentCell = ItemGridView.Rows[ItemGridView.CurrentRow.Index - 1].Cells[languageSetting];
                    }
                }
                else if (currentPanel == RecipeModePanel)
                {
                    if (RecipeGridView.Rows.Count <= 0)
                    {
                        return;
                    }
                    if (RecipeGridView.CurrentRow == null)
                    {
                        return;
                    }

                    if (RecipeGridView.Rows.Count == 1)
                    {
                        recipelastRow = RecipeGridView.Rows[RecipeGridView.CurrentRow.Index];
                        RecipeGridView.Rows[RecipeGridView.CurrentRow.Index].Height = 160;

                        RecipeIDTextbox.Text = RecipeGridView.Rows[RecipeGridView.CurrentRow.Index].Cells["id"].Value.ToString();

                        SelectedItem.Setup(RecipeGridView.Rows[RecipeGridView.CurrentRow.Index].Cells[languageSetting].Value.ToString(), 0x16A2, Convert.ToUInt32("0x" + RecipeGridView.Rows[RecipeGridView.CurrentRow.Index].Cells["id"].Value, 16), GetImagePathFromID(RecipeGridView.Rows[RecipeGridView.CurrentRow.Index].Cells["id"].Value.ToString(), recipeSource), true);
                        UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());
                    }
                    else if (RecipeGridView.CurrentRow.Index > 0)
                    {
                        if (recipelastRow != null)
                        {
                            recipelastRow.Height = 22;
                        }

                        recipelastRow = RecipeGridView.Rows[RecipeGridView.CurrentRow.Index - 1];
                        RecipeGridView.Rows[RecipeGridView.CurrentRow.Index - 1].Height = 160;

                        RecipeIDTextbox.Text = RecipeGridView.Rows[RecipeGridView.CurrentRow.Index - 1].Cells["id"].Value.ToString();

                        SelectedItem.Setup(RecipeGridView.Rows[RecipeGridView.CurrentRow.Index - 1].Cells[languageSetting].Value.ToString(), 0x16A2, Convert.ToUInt32("0x" + RecipeGridView.Rows[RecipeGridView.CurrentRow.Index - 1].Cells["id"].Value, 16), GetImagePathFromID(RecipeGridView.Rows[RecipeGridView.CurrentRow.Index - 1].Cells["id"].Value.ToString(), recipeSource), true);
                        UpdateSelectedItemInfo(SelectedItem.DisplayItemName(), SelectedItem.DisplayItemID(), SelectedItem.DisplayItemData());

                        RecipeGridView.CurrentCell = RecipeGridView.Rows[RecipeGridView.CurrentRow.Index - 1].Cells[languageSetting];
                    }
                }
                else if (currentPanel == FlowerModePanel)
                {

                }
            }
        }

        #endregion

        private void ProcessCheat(string[] cheattext)
        {


            int slotId = 0;
            string myItemId = "";
            string myItemData = "";

            Invoke((MethodInvoker)delegate
            {

                //online
                if (!offline)
                {

                    slotId = 0;
                    foreach (string line in cheattext)
                    {
                        //slotId++; //works online                    
                        slotId++;
                        Regex regex = Space();         // Split on space.
                        string[] substrings = regex.Split(line);
                        MyLog.LogEvent("Online Split String", substrings[0] + " " + substrings[1] + " " + substrings[2] + " " + substrings[3]);

                        myItemId = substrings[3].Substring(4); //substrings[3];
                        myItemData = substrings[2].Substring(4); //substrings[2];

                        byte[] newInventory = new byte[320];
                        byte[] slotBytes = new byte[2];
                        byte[] flag0Bytes = new byte[1];
                        byte[] flag1Bytes = new byte[1];
                        byte[] dataBytes = new byte[4];
                        byte[] recipeBytes = new byte[2];
                        byte[] fenceBytes = new byte[2];

                        int slotOffset = ((slotId - 1) * 0x8);
                        int flag0Offset = 0x3 + ((slotId - 1) * 0x8);
                        int flag1Offset = 0x2 + ((slotId - 1) * 0x8);
                        int countOffset = 0x4 + ((slotId - 1) * 0x8);

                        Buffer.BlockCopy(newInventory, slotOffset, slotBytes, 0, 2);
                        Buffer.BlockCopy(newInventory, flag0Offset, flag0Bytes, 0, 1);
                        Buffer.BlockCopy(newInventory, flag1Offset, flag1Bytes, 0, 1);
                        Buffer.BlockCopy(newInventory, countOffset, dataBytes, 0, 4);
                        Buffer.BlockCopy(newInventory, countOffset, recipeBytes, 0, 2);
                        Buffer.BlockCopy(newInventory, countOffset + 0x2, fenceBytes, 0x0, 0x2);

                        string itemId = myItemId; //Utilities.Flip(Utilities.ByteToHexString(slotBytes));
                        string itemData = myItemData; //Utilities.Flip(Utilities.ByteToHexString(dataBytes));
                        string recipeData = myItemData; //Utilities.Flip(Utilities.ByteToHexString(recipeBytes));
                        string fenceData = myItemData; //Utilities.Flip(Utilities.ByteToHexString(fenceBytes));
                        string flag0 = Utilities.ByteToHexString(flag0Bytes);
                        string flag1 = Utilities.ByteToHexString(flag1Bytes);
                        UInt16 intId = Convert.ToUInt16(itemId, 16);
                        string hexValue = Utilities.PrecedingZeros(itemData, 8);

                        Utilities.SpawnItem(socket, usb, slotId, flag0 + flag1 + itemId, Utilities.PrecedingZeros(itemData, 8));
                        UpdateInventory();
                        MyLog.LogEvent("Online", "slotId: " + slotId.ToString() + " itemId: " + itemId + " itemData: " + itemData + " recipeData: " + recipeData + " Flag0: " + flag0 + " Flag1: " + flag1);


                    } //cheatline 

                } //online

                //offline
                else
                {
                    //make sure we start from first slot
                    slotId = 0;
                    foreach (string line in cheattext)
                    {


                        slotId++;


                        Regex regex = Space();         // Split on space.
                        string[] substrings = regex.Split(line);
                        MyLog.LogEvent("Offline Split String", substrings[0] + " " + substrings[1] + " " + substrings[2] + " " + substrings[3]);
                        myItemId = substrings[3].Substring(4); //substrings[3];
                        myItemData = substrings[2].Substring(4); //substrings[2];

                        byte[] newInventory = new byte[320];
                        byte[] slotBytes = new byte[2];
                        byte[] flag0Bytes = new byte[1];
                        byte[] flag1Bytes = new byte[1];
                        byte[] dataBytes = new byte[4];
                        byte[] recipeBytes = new byte[2];
                        byte[] fenceBytes = new byte[2];

                        int slotOffset = ((slotId - 1) * 0x8);
                        int flag0Offset = 0x3 + ((slotId - 1) * 0x8);
                        int flag1Offset = 0x2 + ((slotId - 1) * 0x8);
                        int countOffset = 0x4 + ((slotId - 1) * 0x8);

                        Buffer.BlockCopy(newInventory, slotOffset, slotBytes, 0, 2);
                        Buffer.BlockCopy(newInventory, flag0Offset, flag0Bytes, 0, 1);
                        Buffer.BlockCopy(newInventory, flag1Offset, flag1Bytes, 0, 1);
                        Buffer.BlockCopy(newInventory, countOffset, dataBytes, 0, 4);
                        Buffer.BlockCopy(newInventory, countOffset, recipeBytes, 0, 2);
                        Buffer.BlockCopy(newInventory, countOffset + 0x2, fenceBytes, 0x0, 0x2);

                        string itemId = myItemId; //Utilities.Flip(Utilities.ByteToHexString(slotBytes));
                        string itemData = myItemData; //Utilities.Flip(Utilities.ByteToHexString(dataBytes));
                        string recipeData = myItemData; //Utilities.Flip(Utilities.ByteToHexString(recipeBytes));
                        string fenceData = myItemData; //Utilities.Flip(Utilities.ByteToHexString(fenceBytes));
                        string flag0 = Utilities.ByteToHexString(flag0Bytes);
                        string flag1 = Utilities.ByteToHexString(flag1Bytes);
                        UInt16 intId = Convert.ToUInt16(itemId, 16);
                        string hexValue = Utilities.PrecedingZeros(itemData, 8);
                        string myHexValue = Utilities.PrecedingZeros(myItemData, 8);

                        MyLog.LogEvent("Offline", "slotId: " + slotId.ToString() + " myItemId: " + myItemId + " myItemData: " + myItemData + " recipeData: " + recipeData + " Flag0: " + flag0 + " Flag1: " + flag1);


                        /// I know this is horrible way of coding... I can not get two foreach loops to work together///
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        if (slotId == 1)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot1.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot1.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot1.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot1.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot1.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot1.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot1.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot1.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot1.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 2)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot2.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot2.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot2.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot2.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot2.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot2.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot2.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot2.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot2.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 3)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot3.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot3.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot3.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot3.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot3.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot3.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot3.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot3.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot3.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 4)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot4.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot4.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot4.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot4.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot4.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot4.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot4.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot4.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot4.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 5)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot5.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot5.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot5.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot5.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot5.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot5.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot5.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot5.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot5.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 6)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot6.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot6.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot6.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot6.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot6.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot6.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot6.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot6.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot6.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 7)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot7.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot7.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot7.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot7.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot7.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot7.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot7.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot7.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot7.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 8)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot8.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot8.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot8.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot8.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot8.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot8.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot8.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot8.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot8.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 9)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot9.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot9.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot9.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot9.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot9.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot9.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot9.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot9.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot9.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 10)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot10.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot10.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot10.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot10.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot10.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot10.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot10.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot10.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot10.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 11)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot11.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot11.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot11.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot11.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot11.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot11.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot11.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot11.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot11.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 12)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot12.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot12.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot12.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot12.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot12.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot12.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot12.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot12.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot12.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 13)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot13.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot13.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot13.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot13.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot13.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot13.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot13.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot13.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot13.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 14)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot14.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot14.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot14.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot14.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot14.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot14.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot14.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot14.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot14.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 15)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot15.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot15.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot15.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot15.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot15.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot15.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot15.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot15.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot15.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 16)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot16.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot16.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot16.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot16.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot16.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot16.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot16.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot16.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot16.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 17)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot17.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot17.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot17.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot17.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot17.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot17.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot17.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot17.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot17.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 18)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot18.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot18.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot18.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot18.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot18.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot18.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot18.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot18.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot18.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 19)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot19.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot19.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot19.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot19.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot19.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot19.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot19.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot19.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot19.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 20)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot20.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot20.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot20.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot20.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot20.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot20.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot20.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot20.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot20.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 21)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot21.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot21.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot21.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot21.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot21.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot21.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot21.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot21.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot21.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 22)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot22.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot22.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot22.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot22.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot22.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot22.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot22.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot22.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot22.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 23)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot23.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot23.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot23.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot23.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot23.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot23.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot23.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot23.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot23.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 24)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot24.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot24.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot24.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot24.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot24.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot24.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot24.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot24.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot24.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 25)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot25.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot25.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot25.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot25.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot25.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot25.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot25.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot25.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot25.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 26)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot26.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot26.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot26.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot26.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot26.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot26.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot26.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot26.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot26.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 27)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot27.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot27.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot27.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot27.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot27.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot27.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot27.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot27.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot27.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 28)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot28.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot28.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot28.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot28.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot28.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot28.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot28.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot28.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot28.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 29)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot29.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot29.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot29.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot29.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot29.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot29.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot29.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot29.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot29.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 30)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot30.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot30.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot30.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot30.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot30.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot30.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot30.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot30.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot30.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 31)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot31.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot31.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot31.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot31.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot31.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot31.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot31.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot31.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot31.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 32)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot32.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot32.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot32.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot32.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot32.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot32.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot32.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot32.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot32.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 33)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot33.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot33.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot33.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot33.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot33.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot33.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot33.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot33.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot33.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 34)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot34.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot34.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot34.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot34.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot34.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot34.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot34.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot34.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot34.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 35)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot35.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot35.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot35.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot35.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot35.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot35.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot35.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot35.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot35.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 36)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot36.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot36.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot36.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot36.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot36.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot36.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot36.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot36.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot36.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 37)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot37.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot37.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot37.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot37.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot37.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot37.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot37.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot37.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot37.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 38)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot38.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot38.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot38.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot38.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot38.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot38.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot38.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot38.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot38.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 39)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot39.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot39.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot39.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot39.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot39.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot39.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot39.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot39.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot39.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }

                        if (slotId == 40)
                        {
                            if (itemId == "FFFE") //Nothing
                            {
                                inventorySlot40.Setup("", 0xFFFE, 0x0, "", "00", "00");
                            }
                            else if (itemId == "16A2") //Recipe
                            {
                                inventorySlot40.Setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "1095") //Delivery
                            {
                                inventorySlot40.Setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                            else if (itemId == "16A1") //Bottle Message
                            {
                                inventorySlot40.Setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag0, flag1);
                            }
                            else if (itemId == "0A13") // Fossil
                            {
                                inventorySlot40.Setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag0, flag1);
                            }
                            else if (itemId == "114A") // Money Tree
                            {
                                inventorySlot40.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource), flag0, flag1);
                            }
                            else if (itemId == "315A" || itemId == "1618" || itemId == "342F") // Wall-Mounted
                            {
                                inventorySlot40.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(fenceData), 16)), flag0, flag1);
                            }
                            else if (ItemAttr.HasFenceWithVariation(intId)) // Fence Variation
                            {
                                inventorySlot40.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + fenceData, 16)), "", flag0, flag1);
                            }
                            else
                            {
                                inventorySlot40.Setup(GetNameFromID(itemId, itemSource), Convert.ToUInt16("0x" + itemId, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemId, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag0, flag1);
                            }
                        }
                        //////////////////////////////////////////////////////////////////////////////////////////////////////


                    }  //cheat line


                } //offline

            }); //invoke

        }

        private void LoadCheat_Click(global::System.Object sender, global::System.EventArgs e)
        {

            try
            {
                OpenFileDialog file = new()
                {
                    Filter = @"Cheat Text (*.txt)|*.txt|All files (*.*)|*.*",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                string savepath;

                if (config.AppSettings.Settings["LastLoad"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
                else
                    savepath = config.AppSettings.Settings["LastLoad"].Value;

                if (Directory.Exists(savepath))
                {
                    file.InitialDirectory = savepath;
                }
                else
                {
                    file.InitialDirectory = @"C:\";
                }

                if (file.ShowDialog() != DialogResult.OK)
                    return;

                string[] temp = file.FileName.Split('\\');
                string path = "";
                for (int i = 0; i < temp.Length - 1; i++)
                    path = path + temp[i] + "\\";

                config.AppSettings.Settings["LastLoad"].Value = path;
                config.Save(ConfigurationSaveMode.Minimal);

                string[] cheattext = File.ReadAllLines(file.FileName);
                ProcessCheat(cheattext);

            }
            catch
            {
                // ignored
            }

        }


        private void LoadCheatfromClipBoard_Click(global::System.Object sender, global::System.EventArgs e)
        {

            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                var text = Clipboard.GetText();
                string file = "cheat.txt";
                File.WriteAllText(file, text);
                string[] cheattext = File.ReadAllLines(file);
                ProcessCheat(cheattext);

            }

        }

        #region Convert to Cheat

        private void Converttocheat_Click(object sender, EventArgs e)
        {
            StringBuilder cheatmaker = new();

            string cheatHead = "08100000 ";

            string[] offsets = new string[40];
            for (int i = 0; i < 40; i++)
            {
                offsets[i] = cheatHead + Utilities.GetItemSlotUIntAddress(i + 1).ToString("X");
            }

            try
            {
                cheatmaker.AppendLine("\n[Your Cheat Name Here]");

                InventorySlot[] slotPointer = new InventorySlot[40];
                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    int slotId = int.Parse(btn.Tag.ToString());
                    slotPointer[slotId - 1] = btn;
                }
                for (int i = 0; i < slotPointer.Length; i++)
                {

                    string first = Utilities.PrecedingZeros(slotPointer[i].GetFlag0() + slotPointer[i].GetFlag1() + Utilities.PrecedingZeros(slotPointer[i].FillItemID(), 4), 8);
                    string second = Utilities.PrecedingZeros(slotPointer[i].FillItemData(), 8);

                    cheatmaker.AppendLine(offsets[i] + " " + second + " " + first);

                    Debug.Print(first + " " + second + " " + slotPointer[i].GetFlag0() + " " + slotPointer[i].GetFlag1() + " " + slotPointer[i].FillItemID());
                }

                //save to your own filename :)   
                SaveFileDialog file = new()
                {
                    Filter = @"Cheat Text (*.txt)|*.txt",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                string savepath;

                if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
                else
                    savepath = config.AppSettings.Settings["LastSave"].Value;

                //Debug.Print(savepath);
                if (Directory.Exists(savepath))
                {
                    file.InitialDirectory = savepath;
                }
                else
                {
                    file.InitialDirectory = @"C:\";
                }

                if (file.ShowDialog() != DialogResult.OK)
                    return;

                string[] temp = file.FileName.Split('\\');
                string path = "";
                for (int i = 0; i < temp.Length - 1; i++)
                    path = path + temp[i] + "\\";

                config.AppSettings.Settings["LastSave"].Value = path;
                config.Save(ConfigurationSaveMode.Minimal);


                //write to cheat//  
                File.WriteAllText(file.FileName, cheatmaker.ToString());

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("MainForm", "Convert to Cheat txt: " + ex.Message);
                MyMessageBox.Show(ex.Message, "Convert to Cheat Crashed");
            }
        }

        #endregion

        #region USB Connect

        private void USBConnectionButton_Click(object sender, EventArgs e)
        {
            if (USBConnectionButton.Tag.ToString() == "connect")
            {
                if (ModifierKeys == Keys.Shift)
                    usb = new USBBot(true);
                else
                    usb = new USBBot(false);

                if (usb.Connect())
                {
                    MyLog.LogEvent("MainForm", "Connection Succeeded : USB");

                    RefreshButton.Visible = true;
                    PlayerInventorySelector.Visible = true;

                    InventoryAutoRefreshToggle.Visible = true;
                    AutoRefreshLabel.Visible = true;

                    OtherTabButton.Visible = true;
                    CritterTabButton.Visible = true;
                    VillagerTabButton.Visible = true;

                    WrapSelector.SelectedIndex = 0;

                    USBConnectionButton.Text = @"Disconnect";
                    USBConnectionButton.Tag = "Disconnect";
                    StartConnectionButton.Visible = false;
                    SettingButton.Visible = false;

                    //MapDropperButton.Visible = true;
                    //BulldozerButton.Visible = true;

                    offline = false;

                    CurrentPlayerIndex = UpdateDropdownBox();

                    PlayerInventorySelector.SelectedIndex = CurrentPlayerIndex;
                    PlayerInventorySelectorOther.SelectedIndex = CurrentPlayerIndex;
                    Text = Text + UpdateTownID() + @" | [Connected via USB]";

                    SetEatButton();
                    UpdateTurnipPrices();
                    ReadWeatherSeed();
                    ReadAirportColor();


                    IPAddressInputBox.Visible = false;
                    IPAddressInputBackground.Visible = false;

                    currentGridView = InsectGridView;
                    LoadGridView(InsectAppearParam, InsectGridView, ref insectRate, Utilities.InsectDataSize, Utilities.InsectNumRecords);
                    LoadGridView(FishRiverAppearParam, RiverFishGridView, ref riverFishRate, Utilities.FishDataSize, Utilities.FishRiverNumRecords, 1);
                    LoadGridView(FishSeaAppearParam, SeaFishGridView, ref seaFishRate, Utilities.FishDataSize, Utilities.FishSeaNumRecords, 1);
                    LoadGridView(CreatureSeaAppearParam, SeaCreatureGridView, ref seaCreatureRate, Utilities.SeaCreatureDataSize, Utilities.SeaCreatureNumRecords, 1);
                }
                else
                {
                    MyLog.LogEvent("MainForm", "Connection Failed : USB");
                    usb = null;
                }
            }
            else
            {
                usb.Disconnect();

                foreach (InventorySlot btn in InventoryPanel.Controls.OfType<InventorySlot>())
                {
                    btn.Reset();
                }

                RefreshButton.Visible = false;
                PlayerInventorySelector.Visible = false;

                InventoryAutoRefreshToggle.Visible = false;
                AutoRefreshLabel.Visible = false;

                OtherTabButton.Visible = false;
                CritterTabButton.Visible = false;
                VillagerTabButton.Visible = false;

                SettingButton.Visible = true;

                IPAddressInputBox.Visible = true;
                IPAddressInputBackground.Visible = true;

                InventoryTabButton_Click(sender, e);
                CleanVillagerPage();

                USBConnectionButton.Text = @"USB";
                USBConnectionButton.Tag = "connect";
                Text = version;
            }
        }

        #endregion

        #region Debug

        int startID = 0x38c8;

        private void FillButton_Click(object sender, EventArgs e)
        {
            string bank1 = "";
            string bank2 = "";
            int iterator = 0;

            do
            {
                string id = Utilities.PrecedingZeros(startID.ToString("X"), 4);
                if (ItemExist(id))
                {

                }
                else
                {
                    string first = Utilities.Flip(Utilities.PrecedingZeros("00" + "00" + id, 8));
                    string second = "00000000";
                    if (iterator < 20)
                        bank1 = bank1 + first + second;
                    else
                        bank2 = bank2 + first + second;
                    iterator++;
                }

                startID++;

            } while (iterator < 40);

            byte[] inventory1 = new byte[160];
            byte[] inventory2 = new byte[160];

            for (int i = 0; i < bank1.Length / 2 - 1; i++)
            {
                string tempStr1 = String.Concat(bank1[(i * 2)].ToString(), bank1[((i * 2) + 1)].ToString());
                string tempStr2 = String.Concat(bank2[(i * 2)].ToString(), bank2[((i * 2) + 1)].ToString());
                //Debug.Print(i.ToString() + " " + data);
                inventory1[i] = Convert.ToByte(tempStr1, 16);
                inventory2[i] = Convert.ToByte(tempStr2, 16);
            }

            Utilities.OverwriteAll(socket, usb, inventory1, inventory2, ref iterator);

            UpdateInventory();
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private static bool ItemExist(string id)
        {
            if (itemSource == null)
            {
                return false;
            }

            DataRow row = itemSource.Rows.Find(id);

            if (row == null)
                return false;
            else
                return true;
        }

        private void PeekButton_Click(object sender, EventArgs e)
        {
            var address = Convert.ToUInt32(DebugAddress.Text, 16);
            byte[] addressBank = Utilities.PeekAddress(socket, null, address, 256);

            byte[] firstBytes = new byte[4];
            byte[] secondBytes = new byte[4];
            byte[] thirdBytes = new byte[4];
            byte[] fourthBytes = new byte[4];

            byte[] firstFullResult = new byte[32];
            byte[] secondFullResult = new byte[32];
            byte[] thirdFullResult = new byte[32];
            byte[] fourthFullResult = new byte[32];
            byte[] fifthFullResult = new byte[32];

            Buffer.BlockCopy(addressBank, 0x0, firstBytes, 0x0, 0x4);
            Buffer.BlockCopy(addressBank, 0x4, secondBytes, 0x0, 0x4);
            Buffer.BlockCopy(addressBank, 0x8, thirdBytes, 0x0, 0x4);
            Buffer.BlockCopy(addressBank, 0xC, fourthBytes, 0x0, 0x4);

            Buffer.BlockCopy(addressBank, 0x0, firstFullResult, 0x0, 0x20);
            Buffer.BlockCopy(addressBank, 0x20, secondFullResult, 0x0, 0x20);
            Buffer.BlockCopy(addressBank, 0x40, thirdFullResult, 0x0, 0x20);
            Buffer.BlockCopy(addressBank, 0x60, fourthFullResult, 0x0, 0x20);
            Buffer.BlockCopy(addressBank, 0x80, fifthFullResult, 0x0, 0x20);

            string firstResult = Utilities.ByteToHexString(firstBytes);
            string secondResult = Utilities.ByteToHexString(secondBytes);
            string thirdResult = Utilities.ByteToHexString(thirdBytes);
            string fourthResult = Utilities.ByteToHexString(fourthBytes);

            string fullResult1 = Utilities.ByteToHexString(firstFullResult);
            string fullResult2 = Utilities.ByteToHexString(secondFullResult);
            string fullResult3 = Utilities.ByteToHexString(thirdFullResult);
            string fullResult4 = Utilities.ByteToHexString(fourthFullResult);
            string fullResult5 = Utilities.ByteToHexString(fifthFullResult);

            PeekResult1.Text = Utilities.Flip(firstResult);
            PeekResult2.Text = Utilities.Flip(secondResult);
            PeekResult3.Text = Utilities.Flip(thirdResult);
            PeekResult4.Text = Utilities.Flip(fourthResult);

            FullPeekResult1.Text = fullResult1.Insert(56, " ").Insert(48, " ").Insert(40, " ").Insert(32, " ").Insert(24, " ").Insert(16, " ").Insert(8, " ");
            FullPeekResult2.Text = fullResult2.Insert(56, " ").Insert(48, " ").Insert(40, " ").Insert(32, " ").Insert(24, " ").Insert(16, " ").Insert(8, " ");
            FullPeekResult3.Text = fullResult3.Insert(56, " ").Insert(48, " ").Insert(40, " ").Insert(32, " ").Insert(24, " ").Insert(16, " ").Insert(8, " ");
            FullPeekResult4.Text = fullResult4.Insert(56, " ").Insert(48, " ").Insert(40, " ").Insert(32, " ").Insert(24, " ").Insert(16, " ").Insert(8, " ");
            FullPeekResult5.Text = fullResult5.Insert(56, " ").Insert(48, " ").Insert(40, " ").Insert(32, " ").Insert(24, " ").Insert(16, " ").Insert(8, " ");
        }

        private void PokeButton_Click(object sender, EventArgs e)
        {
            Utilities.PokeAddress(socket, null, DebugAddress.Text, DebugValue.Text);
        }

        private void PeekMButton_Click(object sender, EventArgs e)
        {
            var address = Convert.ToUInt32(DebugAddress.Text, 16);
            byte[] addressBank = Utilities.PeekMainAddress(socket, null, address, 256);

            byte[] firstBytes = new byte[4];
            byte[] secondBytes = new byte[4];
            byte[] thirdBytes = new byte[4];
            byte[] fourthBytes = new byte[4];

            byte[] firstFullResult = new byte[32];
            byte[] secondFullResult = new byte[32];
            byte[] thirdFullResult = new byte[32];
            byte[] fourthFullResult = new byte[32];
            byte[] fifthFullResult = new byte[32];

            Buffer.BlockCopy(addressBank, 0x0, firstBytes, 0x0, 0x4);
            Buffer.BlockCopy(addressBank, 0x4, secondBytes, 0x0, 0x4);
            Buffer.BlockCopy(addressBank, 0x8, thirdBytes, 0x0, 0x4);
            Buffer.BlockCopy(addressBank, 0xC, fourthBytes, 0x0, 0x4);

            Buffer.BlockCopy(addressBank, 0x0, firstFullResult, 0x0, 0x20);
            Buffer.BlockCopy(addressBank, 0x20, secondFullResult, 0x0, 0x20);
            Buffer.BlockCopy(addressBank, 0x40, thirdFullResult, 0x0, 0x20);
            Buffer.BlockCopy(addressBank, 0x60, fourthFullResult, 0x0, 0x20);
            Buffer.BlockCopy(addressBank, 0x80, fifthFullResult, 0x0, 0x20);

            string firstResult = Utilities.ByteToHexString(firstBytes);
            string secondResult = Utilities.ByteToHexString(secondBytes);
            string thirdResult = Utilities.ByteToHexString(thirdBytes);
            string fourthResult = Utilities.ByteToHexString(fourthBytes);

            string fullResult1 = Utilities.ByteToHexString(firstFullResult);
            string fullResult2 = Utilities.ByteToHexString(secondFullResult);
            string fullResult3 = Utilities.ByteToHexString(thirdFullResult);
            string fullResult4 = Utilities.ByteToHexString(fourthFullResult);
            string fullResult5 = Utilities.ByteToHexString(fifthFullResult);

            PeekResult1.Text = Utilities.Flip(firstResult);
            PeekResult2.Text = Utilities.Flip(secondResult);
            PeekResult3.Text = Utilities.Flip(thirdResult);
            PeekResult4.Text = Utilities.Flip(fourthResult);

            FullPeekResult1.Text = fullResult1.Insert(56, " ").Insert(48, " ").Insert(40, " ").Insert(32, " ").Insert(24, " ").Insert(16, " ").Insert(8, " ");
            FullPeekResult2.Text = fullResult2.Insert(56, " ").Insert(48, " ").Insert(40, " ").Insert(32, " ").Insert(24, " ").Insert(16, " ").Insert(8, " ");
            FullPeekResult3.Text = fullResult3.Insert(56, " ").Insert(48, " ").Insert(40, " ").Insert(32, " ").Insert(24, " ").Insert(16, " ").Insert(8, " ");
            FullPeekResult4.Text = fullResult4.Insert(56, " ").Insert(48, " ").Insert(40, " ").Insert(32, " ").Insert(24, " ").Insert(16, " ").Insert(8, " ");
            FullPeekResult5.Text = fullResult5.Insert(56, " ").Insert(48, " ").Insert(40, " ").Insert(32, " ").Insert(24, " ").Insert(16, " ").Insert(8, " ");
        }
        private void PokeMButton_Click(object sender, EventArgs e)
        {
            Utilities.PokeMainAddress(socket, null, DebugAddress.Text, DebugValue.Text);
        }

        private void UnhideButton_Click(object sender, EventArgs e)
        {
            if (currentPanel == ItemModePanel)
            {
                ItemGridView.Columns["id"]!.Visible = true;
                ItemGridView.Columns["iName"]!.Visible = true;
                ItemGridView.Columns["color"]!.Visible = true;
                ItemGridView.Columns["size"]!.Visible = true;
            }
            else if (currentPanel == RecipeModePanel)
            {
                RecipeGridView.Columns["id"]!.Visible = true;
                RecipeGridView.Columns["iName"]!.Visible = true;
            }
        }

        private void VersionButton_Click(object sender, EventArgs e)
        {
            MyMessageBox.Show(Utilities.CheckSysBotBase(socket, usb), "Sys-botbase Version");
        }

        private void CheckStateButton_Click(object sender, EventArgs e)
        {
            Thread stateThread = new(TryState);
            stateThread.Start();
        }
        private static void TryState()
        {
            do
            {
                Debug.Print(Teleport.GetOverworldState().ToString());
                Debug.Print(Teleport.GetLocationState().ToString());
                Thread.Sleep(2000);
            } while (true);
        }

        private void VillagerHeaderIgnore_CheckedChanged(object sender, EventArgs e)
        {
            RefreshVillagerUI(false);
        }

        private void StarFragmentToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (init) return;
            if (StarFragmentToggle.Checked)
            {
                Utilities.PokeMainAddress(socket, usb, Utilities.MagicAddress.ToString("X"), Utilities.MagicOn);
                Utilities.PokeMainAddress(socket, usb, (Utilities.MagicAddress + 0x14).ToString("X"), Utilities.MagicOn);

                Utilities.PokeMainAddress(socket, usb, Utilities.PetalsAddress.ToString("X"), Utilities.PetalsEnable);
                Utilities.PokeMainAddress(socket, usb, Utilities.PetalsIntensityAddress.ToString("X"), Utilities.PetalsIntensityMAX);
                //Utilities.PokeMainAddress(socket, usb, Utilities.ParticleScaleAddress.ToString("X"), Utilities.ParticleScaleMAX);
                Utilities.PokeMainAddress(socket, usb, Utilities.EatAllAddress.ToString("X"), Utilities.EatAllEnable);
                Utilities.PokeMainAddress(socket, usb, Utilities.EatAll2Address.ToString("X"), Utilities.EatAll2Enable);

                //Utilities.PokeMainAddress(socket, usb, Utilities.LeavesAddress.ToString("X"), Utilities.LeavesEnable);
                Utilities.PokeMainAddress(socket, usb, Utilities.ShopAddress.ToString("X"), Utilities.ShopOpen);
                //Utilities.PokeMainAddress(socket, usb, Utilities.BGMAddress.ToString("X"), Utilities.BGMDisable);
            }
            else
            {
                Utilities.PokeMainAddress(socket, usb, Utilities.MagicAddress.ToString("X"), Utilities.MagicOff);
                Utilities.PokeMainAddress(socket, usb, (Utilities.MagicAddress + 0x14).ToString("X"), Utilities.MagicOff);

                Utilities.PokeMainAddress(socket, usb, Utilities.PetalsAddress.ToString("X"), Utilities.PetalsDisable);
                Utilities.PokeMainAddress(socket, usb, Utilities.PetalsIntensityAddress.ToString("X"), Utilities.PetalsIntensityNormal);
                //Utilities.PokeMainAddress(socket, usb, Utilities.ParticleScaleAddress.ToString("X"), Utilities.ParticleScaleNormal);
                Utilities.PokeMainAddress(socket, usb, Utilities.EatAllAddress.ToString("X"), Utilities.EatAllDisable);
                Utilities.PokeMainAddress(socket, usb, Utilities.EatAll2Address.ToString("X"), Utilities.EatAll2Disable);

                //Utilities.PokeMainAddress(socket, usb, Utilities.LeavesAddress.ToString("X"), Utilities.LeavesDisable);
                Utilities.PokeMainAddress(socket, usb, Utilities.ShopAddress.ToString("X"), Utilities.ShopNormal);
                //Utilities.PokeMainAddress(socket, usb, Utilities.BGMAddress.ToString("X"), Utilities.BGMEnable);
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void WebhookBtn_Click(object sender, EventArgs e)
        {
            Controller.testWebhook();
        }

        private void CacheButton_Click(object sender, EventArgs e)
        {
            CacheImage.Image = ImageCacher.GetImage(GetImagePathFromID(IDTextbox.Text, itemSource));
        }

        #endregion

        #region RoadRoller

        private void RoadRollerButton_Click(object sender, EventArgs e)
        {
            if (Ro == null)
            {
                Ro = new(socket, usb, sound, DEBUGGING);
                Ro.CloseForm += Ro_closeForm;
                Ro.Show();
            }
        }

        private void Ro_closeForm()
        {
            Ro = null;
        }

        #endregion

        #region DodoHelper

        private void DodoHelperButton_Click(object sender, EventArgs e)
        {
            if (D == null)
            {
                D = new Dodo(socket, true, DEBUGGING)
                {
                    ControlBox = true,
                    ShowInTaskbar = true
                };
                D.CloseForm += DodoHelperCloseForm;
                D.AbortAll += DodoHelperAbortAll;
                D.UpdateTurnipPriceHandler += Dodo_updateTurnipPriceHandler;
                D.Show();
                D.WriteLog("[You have started dodo helper in standalone mode.]\n\n" +
                                    "1. Disconnect all controller by selecting \"Controllers\" > \"Change Grip/Order\"\n" +
                                    "2. Leave only the Joy-Con docked on your Switch.\n" +
                                    "3. Return to the game and dock your Switch if needed. Try pressing the buttons below to test the virtual controller.\n" +
                                    "4. If the virtual controller does not response, try the \"Detach\" button first, then the \"A\" button.\n" +
                                    "5. If the virtual controller still does not appear, try restart your Switch.\n\n" +
                                    ">> Please try the buttons below to test the virtual controller. <<"
                                    );
            }
        }

        private void Dodo_updateTurnipPriceHandler()
        {
            SetTurnipPriceMax();
        }

        private void DodoHelperAbortAll()
        {
            MyMessageBox.Show("Dodo Helper Aborted!\nPlease remember to exit the airport first if you want to restart!", "Slamming on the brakes?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DodoHelperCloseForm()
        {
            D = null;
        }
        #endregion

        #region Form Control

        private void InventoryTabButton_Click(object sender, EventArgs e)
        {
            InventoryTabButton.BackColor = Color.FromArgb(80, 80, 255);
            CritterTabButton.BackColor = Color.FromArgb(114, 137, 218);
            OtherTabButton.BackColor = Color.FromArgb(114, 137, 218);
            VillagerTabButton.BackColor = Color.FromArgb(114, 137, 218);

            InventoryLargePanel.Visible = true;
            OtherLargePanel.Visible = false;
            CritterLargePanel.Visible = false;
            VillagerLargePanel.Visible = false;
        }

        private void OtherTabButton_Click(object sender, EventArgs e)
        {
            InventoryTabButton.BackColor = Color.FromArgb(114, 137, 218);
            CritterTabButton.BackColor = Color.FromArgb(114, 137, 218);
            OtherTabButton.BackColor = Color.FromArgb(80, 80, 255);
            VillagerTabButton.BackColor = Color.FromArgb(114, 137, 218);

            InventoryLargePanel.Visible = false;
            OtherLargePanel.Visible = true;
            CritterLargePanel.Visible = false;
            VillagerLargePanel.Visible = false;
            CloseVariationMenu();
        }

        private void CritterTabButton_Click(object sender, EventArgs e)
        {
            InventoryTabButton.BackColor = Color.FromArgb(114, 137, 218);
            CritterTabButton.BackColor = Color.FromArgb(80, 80, 255);
            OtherTabButton.BackColor = Color.FromArgb(114, 137, 218);
            VillagerTabButton.BackColor = Color.FromArgb(114, 137, 218);

            InventoryLargePanel.Visible = false;
            OtherLargePanel.Visible = false;
            CritterLargePanel.Visible = true;
            VillagerLargePanel.Visible = false;
            CloseVariationMenu();
        }

        private void VillagerTabButton_Click(object sender, EventArgs e)
        {
            InventoryTabButton.BackColor = Color.FromArgb(114, 137, 218);
            CritterTabButton.BackColor = Color.FromArgb(114, 137, 218);
            OtherTabButton.BackColor = Color.FromArgb(114, 137, 218);
            VillagerTabButton.BackColor = Color.FromArgb(80, 80, 255);

            InventoryLargePanel.Visible = false;
            OtherLargePanel.Visible = false;
            CritterLargePanel.Visible = false;
            VillagerLargePanel.Visible = true;
            CloseVariationMenu();

            if (V == null && VillagerFirstLoad)
            {
                VillagerFirstLoad = true;
                Thread loadAllVillagerThread = new(LoadAllVillager);
                loadAllVillagerThread.Start();
            }
        }

        private void MapDropperButton_Click(object sender, EventArgs e)
        {
            ItemSearchBox.Clear();

            if (M == null)
            {
                if (DEBUGGING)
                    M = new Map(socket, usb, Utilities.itemPath, Utilities.recipePath, Utilities.flowerPath, Utilities.variationPath, Utilities.favPath, Utilities.imagePath, languageSetting, OverrideDict, sound, true);
                else
                    M = new Map(socket, usb, Utilities.itemPath, Utilities.recipePath, Utilities.flowerPath, Utilities.variationPath, Utilities.favPath, Utilities.imagePath, languageSetting, OverrideDict, sound);
                M.CloseForm += MapDropperCloseForm;
                M.Show();
            }

            ItemSearchBox.Text = @"Search...";
        }

        private void MapDropperCloseForm()
        {
            M = null;
        }

        private void RegeneratorButton_Click(object sender, EventArgs e)
        {
            if (R == null)
            {
                R = new MapRegenerator(socket, sound, DEBUGGING);
                R.CloseForm += RegeneratorCloseForm;
                R.UpdateTurnipPriceHandler += Regenerator_updateTurnipPriceHandler;
                R.Show();
            }
        }

        private void Regenerator_updateTurnipPriceHandler()
        {
            SetTurnipPriceMax();
        }

        private void RegeneratorCloseForm()
        {
            R = null;
        }

        private void BulldozerButton_Click(object sender, EventArgs e)
        {
            if (B == null)
            {
                B = new Bulldozer(socket, usb, sound, DEBUGGING);
                B.CloseForm += BulldozerCloseForm;
                B.Show();
            }
        }

        private void BulldozerCloseForm()
        {
            B = null;
        }

        private void FreezerButton_Click(object sender, EventArgs e)
        {
            if (F == null)
            {
                F = new Freezer(socket, sound, DEBUGGING);
                F.CloseForm += FreezerCloseForm;
                F.Show();
            }
        }

        private void FreezerCloseForm()
        {
            F = null;
        }

        #endregion

        #region Chat
        private void ChatButton_Click(object sender, EventArgs e)
        {
            if (Ch == null)
            {
                Ch = new Chat(socket);
                Ch.CloseForm += Ch_closeForm;
                Ch.Show(this);
                Ch.Location = new Point(Location.X + Width - Ch.Width - 7, Location.Y + Height);
            }
            else
            {
                Ch.Close();
                Ch = null;
            }
        }

        private void Ch_closeForm()
        {
            Ch = null;
        }
        #endregion

        #region Chaser

        private void ChaseBtn_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));
            if (config.AppSettings.Settings["override"].Value == "true")
            {
                DialogResult dialogResult = MyMessageBox.Show("Would you like to disable [Address Override] now?", "Please disable [Address Override] before starting chaser!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    config.AppSettings.Settings["override"].Value = "false";
                    config.Save(ConfigurationSaveMode.Minimal);

                    Application.Restart();
                }
                return;
            }

            UInt32 startAddress = Convert.ToUInt32(DebugAddress.Text, 16);

            ChaseTimer.Start();

            byte[] pattern = [0xC4, 0x09, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00]; // 3 tree branches

            Thread chaserThread = new(delegate () { Chaser(startAddress, pattern); });
            chaserThread.Start();
        }

        private void Chaser(UInt32 startAddress, byte[] pattern)
        {
            ShowWait();

            int offset = 0;

            for (int i = 0; i < 20000; i++)
            {
                ChasingAddress = (startAddress + offset - 4).ToString("X");
                Debug.Print("Chasing : " + ChasingAddress);
                byte[] b = Utilities.ReadByteArray(socket, startAddress + offset - 4, 8192);
                int result = Search(b, pattern);
                if (result >= 0)
                {
                    long fakeAddress = startAddress + offset + result - 4;

                    byte[] nameBytes = Utilities.PeekAddress(socket, usb, (uint)fakeAddress + Utilities.InventoryNameOffset, 0x34);
                    string IslandName = Utilities.GetString(nameBytes, 4, 10);
                    string CharacterName = Utilities.GetString(nameBytes, 32, 10);

                    DialogResult dialogResult = MyMessageBox.Show("Character Name : [   " + CharacterName + "   ]\n" +
                                              "Island Name : [   " + IslandName + "   ]\n\n" +
                                              "Are these correct?"
                                            , "Character & Island Name Found!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        byte[] namePattern = new byte[8];
                        Buffer.BlockCopy(nameBytes, 4, namePattern, 0, 8);

                        int offsetNumber = 0;

                        for (int j = 1; j < 8; j++)
                        {
                            byte[] bBefore = Utilities.PeekAddress(socket, usb, (uint)(fakeAddress - (j * Utilities.playerOffset)) + Utilities.InventoryNameOffset, 0x34);

                            if (Search(bBefore, namePattern) >= 0)
                                offsetNumber++;
                        }


                        UInt32 FinalHeadAddress = (uint)(fakeAddress - (Utilities.playerOffset * offsetNumber));
                        UInt32 FinalOffset = FinalHeadAddress - startAddress;

                        if (FinalOffset != 0)
                        {
                            DialogResult dialogResult2 = MyMessageBox.Show("Tree Branch Address : " + fakeAddress.ToString("X") + "\n" +
                                                                           "Number of Players Before Tree Branch: " + (offsetNumber).ToString() + "\n\n" +
                                                                           "Final Head Address : " + FinalHeadAddress.ToString("X") + "\n" +
                                                                           "Offset : " + FinalOffset.ToString("X") + "\n\n" +
                                                                           "Apply offset ?"
                                                                         , "Address & Header Found!", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            if (dialogResult2 == DialogResult.OK)
                            {
                                Configuration Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));
                                Config.AppSettings.Settings["override"].Value = "true";

                                Config.AppSettings.Settings["PlayerSlot"].Value = (Utilities.masterAddress + FinalOffset).ToString("X");

                                Config.AppSettings.Settings["Villager"].Value = (Utilities.VillagerAddress + FinalOffset).ToString("X");

                                Config.AppSettings.Settings["VillagerHouse"].Value = (Utilities.VillagerHouseAddress + FinalOffset).ToString("X");

                                Config.AppSettings.Settings["RecyclingBin"].Value = (Utilities.MasterRecyclingBase + FinalOffset).ToString("X");
                                Config.AppSettings.Settings["Turnip"].Value = (Utilities.TurnipPurchasePriceAddr + FinalOffset).ToString("X");
                                Config.AppSettings.Settings["Stamina"].Value = (Utilities.staminaAddress + FinalOffset).ToString("X");

                                Config.AppSettings.Settings["WeatherSeed"].Value = (Utilities.weatherSeed + FinalOffset).ToString("X");
                                Config.AppSettings.Settings["MapZero"].Value = (Utilities.mapZero + FinalOffset).ToString("X");

                                Config.Save(ConfigurationSaveMode.Minimal);
                                Invoke((MethodInvoker)delegate { Application.Restart(); });
                                return;
                            }
                        }
                        else
                        {
                            MyMessageBox.Show("Tree Branch Address : " + fakeAddress.ToString("X") + "\n" +
                                              "Number of Players Before Tree Branch: " + (offsetNumber).ToString() + "\n\n" +
                                              "Final Head Address : " + FinalHeadAddress.ToString("X") + "\n" +
                                              "Offset : " + FinalOffset.ToString("X")
                                            , "No Change Needed!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MyMessageBox.Show("Tree branch Address : " + fakeAddress.ToString("X"), "Not Found!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    Debug.Print(result.ToString("X") + "  " + offset.ToString("X") + " Final : " + fakeAddress.ToString("X") + " Offset : " + (fakeAddress - Utilities.masterAddress).ToString("X"));
                    break;
                }
                offset += 8188;
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            ChaseTimer.Stop();
            HideWait();
        }

        private void Chase2Btn_Click(object sender, EventArgs e)
        {
            UInt32 startAddress = Convert.ToUInt32(DebugAddress.Text, 16);

            Debug.Print(startAddress.ToString("X"));
            byte[] b = [];
            for (int i = 0; i < 100; i++)
            {
                b = Utilities.Add(b, Utilities.ReadByteArray(socket, startAddress + i * 8192, 8192));
            }
            File.WriteAllBytes("Chaser2.bin", b);
        }

        private static int Search(byte[] src, byte[] pattern)
        {
            int maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (int i = 0; i < maxFirstCharSlot; i++)
            {
                if (src[i] != pattern[0])
                    continue;

                for (int j = pattern.Length - 1; j >= 1; j--)
                {
                    if (src[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }
            return -1;
        }

        private void ChaseTimer_Tick(object sender, EventArgs e)
        {
            ChasingAddressLabel.Text = ChasingAddress;
        }

        #endregion

        #region Emulator
        private void EmulatorButton_Click(object sender, EventArgs e)
        {
            if (Utilities.FileCheck(hardwareId))
            {

            }
            else
            {
                MyCheck checker = new(hardwareId);
                checker.ShowDialog(this);
                if (!Utilities.FileCheck(hardwareId))
                    return;
            }

            if (Utilities.isEmulator)
                return;

            bool headFound = false;

            /*
            int numOfChase = 20000;
            long startAddress = 0x2A000000000;

            if (ModifierKeys == Keys.Shift)
            {
                numOfChase = 500000;
                startAddress = 0x0;
            }
            */

            new Thread(() =>
            {
                connecting = true;

                Thread.CurrentThread.IsBackground = true;

                //ShowWait();

                Process[] processListYuzu = Process.GetProcessesByName("yuzu");
                Process[] processListRyu = Process.GetProcessesByName("ryujinx");
                Process process;
                if (processListYuzu.Length <= 0 && processListRyu.Length <= 0)
                {
                    MyMessageBox.Show("Unable to locate the process \"yuzu.exe\" or \"ryujinx.exe\" !",
                        "Piece of shit drill (1) !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    HideWait();
                    return;
                }
                else
                {
                    if (processListYuzu.Length != 0)
                    {
                        process = processListYuzu[0];
                        Utilities.EmulatorType = 0;
                    }
                    else
                    {
                        process = processListRyu[0];
                        Utilities.EmulatorType = 1;
                    }
                }

                Invoke((MethodInvoker)delegate
                {
                    StartConnectionButton.Visible = false;
                    USBConnectionButton.Visible = false;
                    IPAddressInputBox.Visible = false;
                    IPAddressInputBackground.Visible = false;
                });

                Utilities.ReadProcessHandle = Utilities.OpenProcess(Utilities.PROCESS_WM_READ, false, process.Id);
                Utilities.WriteProcessHandle = Utilities.OpenProcess(Utilities.PROCESS_ALL_ACCESS, false, process.Id);




                Utilities.SYSTEM_INFO sys_info = new();
                Utilities.GetSystemInfo(out sys_info);

                IntPtr proc_min_address = sys_info.minimumApplicationAddress;
                IntPtr proc_max_address = sys_info.maximumApplicationAddress;

                long proc_min_address_l = proc_min_address;
                long proc_max_address_l = proc_max_address;

                IntPtr processHandle = Utilities.OpenProcess(Utilities.PROCESS_QUERY_INFORMATION | Utilities.PROCESS_WM_READ, false, process.Id);

                Utilities.MEMORY_BASIC_INFORMATION64 mem_basic_info = new();

                long firstHeadAddress = 0x0;

                long lookingForSize;

                if (Utilities.EmulatorType == 0)
                    lookingForSize = 0x100000000;
                else
                    lookingForSize = 0xC0000000; //0xCCE00000;

                while (proc_min_address_l < proc_max_address_l)
                {
                    _ = Utilities.VirtualQueryEx(processHandle, proc_min_address, out mem_basic_info, 48);

                    //Debug.Print(proc_min_address.ToString("X") + " " + mem_basic_info.RegionSize.ToString("X"));

                    if (mem_basic_info.RegionSize == lookingForSize)
                    {
                        headFound = true;
                        if (Utilities.EmulatorType == 0)
                        {
                            firstHeadAddress = mem_basic_info.BaseAddress;
                            Debug.Print("[Y] FirstBase = 0x" + firstHeadAddress.ToString("X"));
                        }
                        else
                        {
                            firstHeadAddress = mem_basic_info.BaseAddress; // + 0xCE00000;
                            Debug.Print("[R] FirstBase = 0x" + firstHeadAddress.ToString("X"));
                        }
                        break;
                    }

                    proc_min_address_l += mem_basic_info.RegionSize;
                    proc_min_address = new IntPtr(proc_min_address_l);
                }


                /*
                ProcessModule myProcessModule;

                for (int i = 0; i < process.Modules.Count; i++)
                {
                    myProcessModule = process.Modules[i];
                    //Debug.Print(myProcessModule.ModuleName + " : " + myProcessModule.BaseAddress.ToString("x8"));
                }

                Debug.Print("The process's main module's base address is: 0x" + process.MainModule.BaseAddress.ToString("x8"));
                */

                /*
                int numOfChase = 20000;
                long startAddress = firstHeadAddress;

                int bytesRead = 0;
                int bufferSize = 0x5000000;
                byte[] buffer = new byte[bufferSize];
                long offset = 0;
                byte[] pattern = new byte[] { 0x00, 0x20, 0x00, 0x00,
                                          0x00, 0x00, 0x00, 0x00,
                                          0x48, 0x0A, 0x60, 0x06,
                                          0x21, 0x00, 0x00, 0x00,
                                          0x48, 0x0A, 0x61, 0x06,
                                          0x21, 0x00, 0x00, 0x00 };

                long diff = bufferSize - pattern.Length;

                for (int i = 0; i < numOfChase; i++)
                {
                    ChasingAddress = (startAddress + offset).ToString("X");

                    Debug.Print("Chasing : " + i + " " + ChasingAddress + " " + bytesRead);
                    bool ReadSuccess = Utilities.ReadProcessMemory((int)Utilities.ReadProcessHandle, startAddress + offset, buffer, buffer.Length, ref bytesRead);
                    if (ReadSuccess)
                    {
                        int result = Search(buffer, pattern);
                        if (result >= 0)
                        {
                            long finalAddress = startAddress + offset + result;

                            Debug.Print(result.ToString("X") + "  " + offset.ToString("X") + " Final : " + finalAddress.ToString("X"));

                                headFound = true;
                            Utilities.EmulatorHeadAddress = finalAddress;
                                break;
                        }

                    }
                    offset += diff;
                }

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();

                HideWait();
                */

                if (!headFound)
                {
                    MyMessageBox.Show("Unable to locate the memory head!",
                      "Piece of shit drill (2) !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    HideWait();
                    return;
                }
                else
                {

                    long heapBaseAddress;
                    long personalidAddress;
                    int i = 0;

                    if (Utilities.EmulatorType == 0)
                    {
                        do
                        {
                            i++;

                            heapBaseAddress = firstHeadAddress - 0xA1FD28 + 0x8B1E0 + 0x12CFCA8 + 0x3000 + 0x13EC248 + 0x1570F68 + 0x11000 + 0x400000;
                            personalidAddress = heapBaseAddress + 0xAC43C4C8 - 0xC10 + 0xB0B8;

                            byte[] idBuffer = new byte[4];
                            int bytesRead2 = 0;
                            bool ReadSuccess = Utilities.ReadProcessMemory(checked((int)Utilities.ReadProcessHandle), personalidAddress, idBuffer, idBuffer.Length, ref bytesRead2);
                            int id = BitConverter.ToInt32(idBuffer, 0);

                            if (id == 0)
                            {
                                firstHeadAddress += 0x40000000;
                            }
                            else
                            {
                                Utilities.HeapOffsetValue = firstHeadAddress - mem_basic_info.BaseAddress;
                                Debug.Print("Offset =  + 0x" + Utilities.HeapOffsetValue.ToString("X"));
                                Utilities.EmulatorHeadAddress = firstHeadAddress;
                                break;
                            }

                        } while (i <= 4);
                    }
                    else
                    {
                        /*
                        do
                        {
                            i++;

                            heapBaseAddress = firstHeadAddress - 0xA1FD28 + 0x8B1E0 + 0x12CFCA8 + 0x3000 + 0x13EC248 + 0x1570F68 + 0x11000 + 0x400000;
                            personalidAddress = heapBaseAddress + 0xAC43C4C8 - 0xC10 + 0xB0B8;

                            byte[] idBuffer = new byte[4];
                            int bytesRead2 = 0;
                            bool ReadSuccess = Utilities.ReadProcessMemory(checked((int)Utilities.ReadProcessHandle), personalidAddress, idBuffer, idBuffer.Length, ref bytesRead2);
                            int id = BitConverter.ToInt32(idBuffer, 0);

                            if (id == 0)
                            {
                                firstHeadAddress += 0x40000000;
                            }
                            else
                            {
                                Utilities.HeapOffsetValue = firstHeadAddress - mem_basic_info.BaseAddress;
                                Debug.Print("Offset =  + 0x" + Utilities.HeapOffsetValue.ToString("X"));
                                Utilities.EmulatorHeadAddress = firstHeadAddress;
                                break;
                            }

                        } while (i <= 4);
                        */
                        Utilities.EmulatorHeadAddress = firstHeadAddress;
                    }


                    //Utilities.EmulatorHeadAddress = firstHeadAddress;

                    if (Utilities.EmulatorHeadAddress == 0x0)
                    {
                        MyMessageBox.Show("Unable to locate the heap offset!",
                          "Piece of shit drill (3) !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        HideWait();
                        return;
                    }


                    Utilities.isEmulator = true;


                    Invoke((MethodInvoker)delegate
                    {

                        RefreshButton.Visible = true;
                        PlayerInventorySelector.Visible = true;

                        InventoryAutoRefreshToggle.Visible = true;
                        AutoRefreshLabel.Visible = true;


                        OtherTabButton.Visible = true;
                        CritterTabButton.Visible = true;
                        VillagerTabButton.Visible = true;

                        WrapSelector.SelectedIndex = 0;

                        FreezeTimeLabel.Visible = false;
                        FreezeTimeButton.Visible = false;
                        UnFreezeTimeButton.Visible = false;
                        AnimationSpeedLabel.Visible = false;
                        animationSpeedx0_1.Visible = false;
                        animationSpeedx1.Visible = false;
                        animationSpeedx2.Visible = false;
                        animationSpeedx5.Visible = false;
                        animationSpeedx50.Visible = false;
                        DisableCollisionLabel.Visible = false;
                        DisableCollisionToggle.Visible = false;
                        MaxWalkSpeedLabel.Visible = false;
                        maxSpeedX1Btn.Visible = false;
                        maxSpeedX2Btn.Visible = false;
                        maxSpeedX3Btn.Visible = false;
                        maxSpeedX5Btn.Visible = false;
                        maxSpeedX100Btn.Visible = false;
                        SwimClubModeLabel.Visible = false;
                        FastSwimToggle.Visible = false;

                        SettingButton.Visible = false;

                        MapDropperButton.Visible = true;
                        //RegeneratorButton.Visible = true;
                        //FreezerButton.Visible = true;
                        //DodoHelperButton.Visible = true;
                        BulldozerButton.Visible = true;
                        RoadRollerButton.Visible = true;
                        //chatButton.Visible = true;

                        offline = false;

                        CurrentPlayerIndex = UpdateDropdownBox();


                        PlayerInventorySelector.SelectedIndex = CurrentPlayerIndex;
                        PlayerInventorySelectorOther.SelectedIndex = CurrentPlayerIndex;

                        if (Utilities.EmulatorType == 0)
                            Text += UpdateTownID() + " [Yuzu] + 0x" + Utilities.HeapOffsetValue.ToString("X");
                        else
                            Text += UpdateTownID() + " [Ryujinx] + 0x" + Utilities.HeapOffsetValue.ToString("X");


                        SetEatButton();
                        UpdateTurnipPrices();
                        ReadWeatherSeed();
                        ReadAirportColor();
                        //ReadActivatedCheat();

                        currentGridView = InsectGridView;

                        MyLog.LogEvent("MainForm", "Loading Param Files");

                        LoadGridView(InsectAppearParam, InsectGridView, ref insectRate, Utilities.InsectDataSize, Utilities.InsectNumRecords);
                        LoadGridView(FishRiverAppearParam, RiverFishGridView, ref riverFishRate, Utilities.FishDataSize, Utilities.FishRiverNumRecords, 1);
                        LoadGridView(FishSeaAppearParam, SeaFishGridView, ref seaFishRate, Utilities.FishDataSize, Utilities.FishSeaNumRecords, 1);
                        LoadGridView(CreatureSeaAppearParam, SeaCreatureGridView, ref seaCreatureRate, Utilities.SeaCreatureDataSize, Utilities.SeaCreatureNumRecords, 1);

                    });
                }
            }).Start();
        }

        #endregion

    }
}
