using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACNHPokerCore
{

    #region event
    public delegate void ObeySizeHandler(bool toggle, int itemHeight = 0, int itemWidth = 0, int newSpawnHeight = 0, int newSpawnWidth = 0, bool wallmount = false, bool ceiling = false);
    public delegate void UpdateRowAndColumnHandler(int row, int column);
    public delegate void ApplyFilter(string itemkind);
    public delegate void CloseFilter();
    #endregion

    public partial class Map : Form
    {
        #region Variable
        private static Socket s;
        private readonly USBBot usb;

        private readonly DataTable source;
        private readonly DataTable recipeSource;
        private readonly DataTable flowerSource;
        private readonly DataTable variationSource;
        private readonly DataTable favSource;
        private readonly DataTable fieldSource;
        private FloorSlot selectedButton;
        private string selectedSize;
        private readonly FloorSlot[] floorSlots;
        private Variation selection;
        private MiniMap miniMap;
        public BulkSpawn bulkSpawn;
        private Filter itemFilter;
        private BulkList bulkList;
        private int counter;
        private int saveTime = -1;
        private bool drawing;

        private DataGridViewRow lastRow;
        private readonly string imagePath;
        private string languageSetting = "eng";

        private readonly Dictionary<string, string> OverrideDict;

        private int anchorX = -1;
        private int anchorY = -1;
        private int Corner1X = -1;
        private int Corner1Y = -1;
        private int Corner2X = -1;
        private int Corner2Y = -1;
        private bool CornerOne = true;
        private bool AreaSet;
        private readonly ToolStripMenuItem CopyAreaMenu;
        private bool AreaCopied;
        private readonly ToolStripMenuItem PasteAreaMenu;
        private readonly ToolStripMenuItem SaveAreaMenu;
        private byte[][] SavedArea;


        private DataTable currentDataTable;
        private readonly bool sound;
        private bool ignore;
        private bool keepProtection;
        private int keepProtectionCounter;
        public int numOfColumn = 1;
        public int numOfRow = 1;

        private InventorySlot[,] variationList;
        private bool obeySize;
        private int itemWidth;
        private int itemHeight;
        private int newSpawnWidth;
        private int newSpawnHeight;
        private bool wallmount;
        private bool ceiling;

        private byte[] Layer1;
        private byte[] Layer2;
        private byte[] Acre;
        private byte[] Building;
        private byte[] Terrain;
        private byte[] ActivateLayer1;
        private byte[] ActivateLayer2;
        private byte[] MapCustomDesgin;

        private bool[,] ActivateTable1;
        private bool[,] ActivateTable2;

        private bool shiftRight;
        private bool shiftDown;
        private bool coreOnly;
        private bool isPillar;
        private bool isHalfTileItem;
        private readonly bool debugging;
        private bool filterOn;
        private string filterKind = "";

        private readonly ToolStripMenuItem ActivateItem;
        private readonly ToolStripMenuItem DeactivateItem;
        private readonly ToolStripMenuItem SetBaseItem;
        private readonly ToolStripMenuItem CreateExtension;

        private bool hasBaseItem;
        private ushort BaseID;
        private int BaseMapX = -1;
        private int BaseMapY = -1;

        public event CloseHandler CloseForm;
        private static readonly Lock lockObject = new();
        private readonly Color[] target =
        [
            Color.FromArgb(252, 3, 3),
            Color.FromArgb(252, 78, 3),
            Color.FromArgb(252, 227, 3),
            Color.FromArgb(0, 82, 7),
            Color.FromArgb(3, 255, 32),
            Color.FromArgb(5, 106, 230),
            Color.FromArgb(157, 40, 224),
        ];
        private int targetValue;

        private readonly string debugTerrain = @"terrainAcres.nht";
        private readonly string debugAcres = @"acres.nha";
        private readonly string debugBuilding = @"buildings.nhb";
        private readonly string debugItem = @"item.nhf";

        private const int ExtendedMapOffset = (16 * 6 * 16 * 1) * 2; // One extra column of Acre
        #endregion

        #region Form Load
        public Map(Socket S, USBBot USB, string itemPath, string recipePath, string flowerPath, string variationPath, string favPath, string ImagePath, string LanguageSetting, Dictionary<string, string> overrideDict, bool Sound, bool Debugging = false)
        {
            try
            {
                s = S;
                usb = USB;

                if (File.Exists(itemPath))
                    source = LoadItemCSVWithKind(itemPath);
                if (File.Exists(recipePath))
                    recipeSource = LoadItemCSV(recipePath);
                if (File.Exists(flowerPath))
                    flowerSource = LoadItemCSV(flowerPath);
                if (File.Exists(variationPath))
                    variationSource = LoadItemCSV(variationPath);
                if (File.Exists(favPath))
                    favSource = LoadItemCSVWithKind(favPath, false);
                if (File.Exists(Utilities.fieldPath))
                    fieldSource = LoadItemCSV(Utilities.fieldPath);

                imagePath = ImagePath;
                OverrideDict = overrideDict;
                sound = Sound;
                debugging = Debugging;

                floorSlots = new FloorSlot[49];

                InitializeComponent();

                foreach (FloorSlot btn in BtnPanel.Controls.OfType<FloorSlot>())
                {
                    int i = int.Parse(btn.Tag.ToString());
                    floorSlots[i] = btn;
                }


                if (source != null)
                {
                    FieldGridView.DataSource = source;

                    //set the ID row invisible
                    FieldGridView.Columns["id"].Visible = false;
                    FieldGridView.Columns["iName"].Visible = false;
                    FieldGridView.Columns["jpn"].Visible = false;
                    FieldGridView.Columns["tchi"].Visible = false;
                    FieldGridView.Columns["schi"].Visible = false;
                    FieldGridView.Columns["kor"].Visible = false;
                    FieldGridView.Columns["fre"].Visible = false;
                    FieldGridView.Columns["ger"].Visible = false;
                    FieldGridView.Columns["spa"].Visible = false;
                    FieldGridView.Columns["ita"].Visible = false;
                    FieldGridView.Columns["dut"].Visible = false;
                    FieldGridView.Columns["rus"].Visible = false;
                    FieldGridView.Columns["color"].Visible = false;
                    FieldGridView.Columns["size"].Visible = false;
                    FieldGridView.Columns["Kind"].Visible = false;

                    //select the full row and change color cause windows blue sux
                    FieldGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    FieldGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
                    FieldGridView.DefaultCellStyle.ForeColor = Color.FromArgb(255, 114, 105, 110);
                    FieldGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                    FieldGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 57, 60, 67);
                    FieldGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    FieldGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                    FieldGridView.EnableHeadersVisualStyles = false;

                    //create the image column
                    DataGridViewImageColumn imageColumn = new()
                    {
                        Name = "Image",
                        HeaderText = "Image",
                        ImageLayout = DataGridViewImageCellLayout.Zoom
                    };
                    FieldGridView.Columns.Insert(13, imageColumn);
                    imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                    FieldGridView.Columns["eng"].Width = 195;
                    FieldGridView.Columns["jpn"].Width = 195;
                    FieldGridView.Columns["tchi"].Width = 195;
                    FieldGridView.Columns["schi"].Width = 195;
                    FieldGridView.Columns["kor"].Width = 195;
                    FieldGridView.Columns["fre"].Width = 195;
                    FieldGridView.Columns["ger"].Width = 195;
                    FieldGridView.Columns["spa"].Width = 195;
                    FieldGridView.Columns["ita"].Width = 195;
                    FieldGridView.Columns["dut"].Width = 195;
                    FieldGridView.Columns["rus"].Width = 195;
                    FieldGridView.Columns["Image"].Width = 128;

                    FieldGridView.Columns["eng"].HeaderText = "Name";
                    FieldGridView.Columns["jpn"].HeaderText = "Name";
                    FieldGridView.Columns["tchi"].HeaderText = "Name";
                    FieldGridView.Columns["schi"].HeaderText = "Name";
                    FieldGridView.Columns["kor"].HeaderText = "Name";
                    FieldGridView.Columns["fre"].HeaderText = "Name";
                    FieldGridView.Columns["ger"].HeaderText = "Name";
                    FieldGridView.Columns["spa"].HeaderText = "Name";
                    FieldGridView.Columns["ita"].HeaderText = "Name";
                    FieldGridView.Columns["dut"].HeaderText = "Name";
                    FieldGridView.Columns["rus"].HeaderText = "Name";

                    currentDataTable = source;

                    FieldGridView.DefaultCellStyle.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
                }


                CopyAreaMenu = new ToolStripMenuItem("Copy Area", null, CopyAreaToolStripMenuItem_Click)
                {
                    ForeColor = Color.White
                };

                PasteAreaMenu = new ToolStripMenuItem("Paste Area", null, PasteAreaToolStripMenuItem_Click)
                {
                    ForeColor = Color.White
                };

                SaveAreaMenu = new ToolStripMenuItem("Save Area to File", null, SaveAreaToolStripMenuItem_Click)
                {
                    ForeColor = Color.White
                };

                ActivateItem = new ToolStripMenuItem("Activate", null, ActivateItemToolStripMenuItem_Click)
                {
                    ForeColor = Color.White,
                    BackColor = Color.Green
                };

                DeactivateItem = new ToolStripMenuItem("Deactivate", null, DeactivateItemToolStripMenuItem_Click)
                {
                    ForeColor = Color.White,
                    BackColor = Color.Crimson
                };

                SetBaseItem = new ToolStripMenuItem("Set As Base", null, SetBaseItemToolStripMenuItem_Click)
                {
                    ForeColor = Color.White,
                    BackColor = Color.Blue
                };

                CreateExtension = new ToolStripMenuItem("Create Extension", null, CreateExtensionToolStripMenuItem_Click)
                {
                    ForeColor = Color.White,
                    BackColor = Color.Orange
                };

                KeyPreview = true;

                LanguageSetup(LanguageSetting);
                languageSetting = LanguageSetting;

                if (FieldGridView.Columns.Contains(languageSetting))
                {
                    HideAllLanguage();
                    FieldGridView.Columns[languageSetting].Visible = true;
                }

                //FlashTimer.Start();

                ((TextBox)HexTextbox.Controls[1]).MaxLength = 8;
                ((TextBox)FlagTextbox.Controls[1]).MaxLength = 2;

                MyLog.LogEvent("Map", "MapForm Started Successfully");
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "MapForm Construct: " + ex.Message);
                NextSaveTimer.Stop();
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Fetch Map
        private async void FetchMapBtn_Click(object sender, EventArgs e)
        {
            fetchMapBtn.Enabled = false;

            btnToolTip.RemoveAll();

            if (debugging)
            {
                OpenFileDialog file = new()
                {
                    Filter = "New Horizons Fasil (*.nhf)|*.nhf",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                string savepath;

                if (config.AppSettings.Settings["LastLoad"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
                else
                    savepath = config.AppSettings.Settings["LastLoad"].Value;

                if (File.Exists(savepath + debugItem))
                {
                    Layer1 = File.ReadAllBytes(savepath + debugItem);
                }
                else
                {
                    if (Directory.Exists(savepath))
                    {
                        file.InitialDirectory = savepath;
                    }
                    else
                    {
                        file.InitialDirectory = @"C:\";
                    }

                    if (file.ShowDialog() != DialogResult.OK)
                        Layer1 = new byte[Utilities.mapSize];
                    else
                        Layer1 = File.ReadAllBytes(file.FileName);
                }

                byte[] emptyPlaza = new byte[12];

                if (File.Exists(savepath + debugAcres))
                {
                    Acre = Utilities.Add(File.ReadAllBytes(savepath + debugAcres), emptyPlaza);

                    if (File.Exists(savepath + debugTerrain))
                    {
                        Terrain = File.ReadAllBytes(savepath + debugTerrain);
                    }
                    else
                    {
                        Terrain = new byte[Utilities.AllTerrainSize];
                    }

                    if (File.Exists(savepath + debugBuilding))
                    {
                        Building = File.ReadAllBytes(savepath + debugBuilding);
                    }
                    else
                    {
                        Building = new byte[Utilities.AllBuildingSize];
                    }
                }
                else
                {
                    Acre = new byte[Utilities.AcreAndPlaza];
                    Building = new byte[Utilities.AllBuildingSize];
                    Terrain = new byte[Utilities.AllTerrainSize];
                }


                Layer2 = new byte[Utilities.mapSize];
                ActivateLayer1 = new byte[Utilities.mapActivateSize];
                ActivateLayer2 = new byte[Utilities.mapActivateSize];
                MapCustomDesgin = new byte[Utilities.MapTileCount16x16 * 2];

                byte[] EmptyDesign = [0x00, 0xF8];
                for (int i = 0; i < Utilities.MapTileCount16x16; i++)
                    Buffer.BlockCopy(EmptyDesign, 0, MapCustomDesgin, i * 2, 2);

                if (miniMap == null)
                    miniMap = new MiniMap(Layer1, Acre, Building, Terrain, MapCustomDesgin, 2);

                anchorX = 56;
                anchorY = 48;

                UpdateUI(() =>
                {
                    miniMapBox.BackgroundImage = MiniMap.CombineMap(miniMap.DrawBackground(), miniMap.DrawItemMap());
                    DisplayAnchorAsync();
                    xCoordinate.Text = anchorX.ToString();
                    yCoordinate.Text = anchorY.ToString();
                    EnableBtn();
                    fetchMapBtn.Visible = false;
                    reAnchorBtn.Visible = true;
                });
                return;
            }

            if ((s == null || s.Connected == false) && usb == null && !Utilities.isEmulator)
            {
                MessageBox.Show(@"Please connect to the Switch first!");
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    if (ModifierKeys == Keys.Shift)
                        FetchMap(Utilities.mapZero, Utilities.mapZero + Utilities.mapSize, true);
                    else
                        FetchMap(Utilities.mapZero, Utilities.mapZero + Utilities.mapSize, false);

                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void FetchMap(UInt32 layer1Address, UInt32 layer2Address, bool saveFile)
        {
            try
            {
                ShowMapWait((42 + 2) * 2, "Fetching Map...");

                Layer1 = Utilities.GetMapLayer(s, usb, layer1Address, ref counter);
                Layer2 = Utilities.GetMapLayer(s, usb, layer2Address, ref counter);
                Acre = Utilities.GetAcre(s, usb);
                Building = Utilities.GetBuilding(s, usb);
                Terrain = Utilities.GetTerrain(s, usb);
                ActivateLayer1 = Utilities.GetActivate(s, usb, Utilities.mapActivate, ref counter);
                ActivateLayer2 = Utilities.GetActivate(s, usb, Utilities.mapActivate + Utilities.mapActivateSize, ref counter);
                MapCustomDesgin = Utilities.GetCustomDesignMap(s, usb, ref counter);

                if (saveFile)
                {
                    if (!Directory.Exists(@"Your\"))
                        Directory.CreateDirectory(@"Your\");

                    File.WriteAllBytes(@"Your\YourLayer1.nhl", Layer1);
                    File.WriteAllBytes(@"Your\YourLayer2.nhl", Layer2);
                    File.WriteAllBytes(@"Your\YourAcre.nha", Acre);
                    File.WriteAllBytes(@"Your\YourBuilding.nhb", Building);
                    File.WriteAllBytes(@"Your\YourTerrain.nht", Terrain);
                    File.WriteAllBytes(@"Your\YourActivateLayer1.nhal", ActivateLayer1);
                    File.WriteAllBytes(@"Your\YourActivateLayer2.nhal", ActivateLayer2);
                    File.WriteAllBytes(@"Your\YourCustomDesignMap.nhdm", MapCustomDesgin);
                    MyMessageBox.Show("File saved!", "Nanomachines, Son!", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }


                if (Layer1 != null && Layer2 != null && Acre != null)
                {
                    if (miniMap == null)
                        miniMap = new MiniMap(Layer1, Acre, Building, Terrain, MapCustomDesgin, 2);
                }
                else
                    throw new NullReferenceException("Layer1/Layer2/Acre");

                BuildActivateTable(ActivateLayer1, ref ActivateTable1);
                BuildActivateTable(ActivateLayer2, ref ActivateTable2);

                /*
                byte[] Coordinate = Utilities.GetCoordinate(s, usb);

                if (Coordinate != null)
                {
                    int x = BitConverter.ToInt32(Coordinate, 0);
                    int y = BitConverter.ToInt32(Coordinate, 4);

                    anchorX = x - 0x24;
                    anchorY = y - 0x18;
                */
                    if (anchorX < 3 || anchorY < 3 || anchorX > 108 || anchorY > 92)
                    {
                        anchorX = 56;
                        anchorY = 48;
                    }

                    UpdateUI(() =>
                    {
                        miniMapBox.BackgroundImage = MiniMap.CombineMap(miniMap.DrawBackground(), miniMap.DrawItemMap());
                        DisplayAnchorAsync();
                        xCoordinate.Text = anchorX.ToString();
                        yCoordinate.Text = anchorY.ToString();
                        EnableBtn();
                        fetchMapBtn.Visible = false;
                        reAnchorBtn.Visible = true;
                        //NextSaveTimer.Start();
                    });
                /*
                }
                else
                    throw new NullReferenceException("Coordinate");
                */

                HideMapWait();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "FetchMap: " + ex.Message);
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message, "!!! THIS SHIT DOESN'T WORK!! WHY? HAS I EVER?");
            }
        }

        private void Map_Load(object sender, EventArgs e)
        {
            //FetchMapBtn_Click(null, null);
        }
        #endregion

        #region Anchor
        private byte[][] GetMapColumns(int x, int y)
        {
            byte[] Layer;
            if (layer1Btn.Checked)
                Layer = Layer1;
            else if (layer2Btn.Checked)
                Layer = Layer2;
            else
                return null;

            byte[][] floorByte = new byte[14][];
            for (int i = 0; i < 14; i++)
            {
                floorByte[i] = new byte[0x70];
                Buffer.BlockCopy(Layer, ((x - 3) * 2 + i) * 0x600 + (y - 3) * 0x10, floorByte[i], 0x0, 0x70);
            }

            return floorByte;
        }

        public void MoveAnchor(int x, int y)
        {
            UpdateUI(() =>
            {
                btnToolTip.RemoveAll();
            });

            if (x < 3)
                anchorX = 3;
            else if (x > 108)
                anchorX = 108;
            else
                anchorX = x;

            if (y < 3)
                anchorY = 3;
            else if (y > 92)
                anchorY = 92;
            else
                anchorY = y;

            UpdateUI(() =>
            {
                xCoordinate.Text = anchorX.ToString();
                yCoordinate.Text = anchorY.ToString();
            });

            selectedButton = floor25;

            DisplayAnchorAsync();
        }

        private void SetupBtnCoordinate(int x, int y)
        {
            int iterator = 0;

            for (int i = -3; i <= 3; i++)
            {
                for (int j = -3; j <= 3; j++)
                {
                    floorSlots[iterator].MapX = x + i;
                    floorSlots[iterator].MapY = y + j;
                    iterator++;
                }
            }
        }

        private void BtnSetup(byte[] b, byte[] b2, int x, int y, FloorSlot slot1, FloorSlot slot2, FloorSlot slot3, FloorSlot slot4, FloorSlot slot5, FloorSlot slot6, FloorSlot slot7, Boolean anchor = false)
        {
            byte[] idBytes = new byte[2];
            byte[] flag0Bytes = new byte[1];
            byte[] flag1Bytes = new byte[1];
            byte[] dataBytes = new byte[4];

            byte[] part2IdBytes = new byte[4];
            byte[] part2DataBytes = new byte[4];
            byte[] part3IdBytes = new byte[4];
            byte[] part3DataBytes = new byte[4];
            byte[] part4IdBytes = new byte[4];
            byte[] part4DataBytes = new byte[4];

            //byte[] idFull = new byte[4];

            FloorSlot currentBtn = null;

            for (int i = 0; i < 7; i++)
            {
                Buffer.BlockCopy(b, (i * 0x10) + 0x0, idBytes, 0x0, 0x2);
                Buffer.BlockCopy(b, (i * 0x10) + 0x2, flag1Bytes, 0x0, 0x1);
                Buffer.BlockCopy(b, (i * 0x10) + 0x3, flag0Bytes, 0x0, 0x1);
                Buffer.BlockCopy(b, (i * 0x10) + 0x4, dataBytes, 0x0, 0x4);

                Buffer.BlockCopy(b, (i * 0x10) + 0x8, part2IdBytes, 0x0, 0x4);
                Buffer.BlockCopy(b, (i * 0x10) + 0xC, part2DataBytes, 0x0, 0x4);
                Buffer.BlockCopy(b2, (i * 0x10) + 0x0, part3IdBytes, 0x0, 0x4);
                Buffer.BlockCopy(b2, (i * 0x10) + 0x4, part3DataBytes, 0x0, 0x4);
                Buffer.BlockCopy(b2, (i * 0x10) + 0x8, part4IdBytes, 0x0, 0x4);
                Buffer.BlockCopy(b2, (i * 0x10) + 0xC, part4DataBytes, 0x0, 0x4);

                string itemID = Utilities.Flip(Utilities.ByteToHexString(idBytes));
                string flag1 = Utilities.ByteToHexString(flag1Bytes);
                string flag0 = Utilities.ByteToHexString(flag0Bytes);
                string itemData = Utilities.Flip(Utilities.ByteToHexString(dataBytes));

                string part2Id = Utilities.Flip(Utilities.ByteToHexString(part2IdBytes));
                string part2Data = Utilities.Flip(Utilities.ByteToHexString(part2DataBytes));
                string part3Id = Utilities.Flip(Utilities.ByteToHexString(part3IdBytes));
                string part3Data = Utilities.Flip(Utilities.ByteToHexString(part3DataBytes));
                string part4Id = Utilities.Flip(Utilities.ByteToHexString(part4IdBytes));
                string part4Data = Utilities.Flip(Utilities.ByteToHexString(part4DataBytes));

                if (i == 0)
                {
                    currentBtn = slot1;
                    SetBtn(slot1, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag0, flag1);
                }
                else if (i == 1)
                {
                    currentBtn = slot2;
                    SetBtn(slot2, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag0, flag1);
                }
                else if (i == 2)
                {
                    currentBtn = slot3;
                    SetBtn(slot3, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag0, flag1);
                }
                else if (i == 3)
                {
                    currentBtn = slot4;
                    SetBtn(slot4, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag0, flag1);
                    if (anchor)
                    {
                        //slot4.BackColor = Color.Red;
                    }
                }
                else if (i == 4)
                {
                    currentBtn = slot5;
                    SetBtn(slot5, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag0, flag1);
                }
                else if (i == 5)
                {
                    currentBtn = slot6;
                    SetBtn(slot6, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag0, flag1);
                }
                else if (i == 6)
                {
                    currentBtn = slot7;
                    SetBtn(slot7, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag0, flag1);
                }

                currentBtn.MapX = x;
                currentBtn.MapY = y + i;
            }
        }

        private async Task SetBtn(FloorSlot btn, string itemID, string itemData, string part2Id, string part2Data, string part3Id, string part3Data, string part4Id, string part4Data, string flag0, string flag1)
        {
            string Name = GetNameFromID(itemID, source);
            UInt16 ID = Convert.ToUInt16("0x" + itemID, 16);
            UInt32 Data = Convert.ToUInt32("0x" + itemData, 16);
            UInt32 IntP2Id = Convert.ToUInt32("0x" + part2Id, 16);
            UInt32 IntP2Data = Convert.ToUInt32("0x" + part2Data, 16);
            UInt32 IntP3Id = Convert.ToUInt32("0x" + part3Id, 16);
            UInt32 IntP3Data = Convert.ToUInt32("0x" + part3Data, 16);
            UInt32 IntP4Id = Convert.ToUInt32("0x" + part4Id, 16);
            UInt32 IntP4Data = Convert.ToUInt32("0x" + part4Data, 16);

            string P1Id = itemID;
            string P2Id = Utilities.Turn2bytes(part2Id);
            string P3Id = Utilities.Turn2bytes(part3Id);
            string P4Id = Utilities.Turn2bytes(part4Id);

            string P1Data = Utilities.Turn2bytes(itemData);
            string P2Data = Utilities.Turn2bytes(part2Data);
            string P3Data = Utilities.Turn2bytes(part3Data);
            string P4Data = Utilities.Turn2bytes(part4Data);

            string Path1;
            string Path2;
            string Path3;
            string Path4;
            string ContainPath = "";

            string front = Utilities.PrecedingZeros(Data.ToString("X"), 8).Substring(0, 4);
            string back = Utilities.PrecedingZeros(Data.ToString("X"), 8).Substring(4, 4);

            if (P1Id == "FFFD")
                Path1 = GetImagePathFromID(P1Data, source);
            else if (P1Id == "16A2")
            {
                Path1 = GetImagePathFromID(P1Data, recipeSource, Data);
                Name = GetNameFromID(P1Data, recipeSource);
            }
            else if (P1Id == "114A" || P1Id == "EC9C")
            {
                Path1 = GetImagePathFromID(itemID, source, Data);
                ContainPath = GetImagePathFromID(back, source);
            }
            else if (P1Id == "315A" || P1Id == "1618" || P1Id == "342F")
            {
                Path1 = GetImagePathFromID(itemID, source, Data);
                ContainPath = GetImagePathFromID(P1Data, source, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16));
            }
            else if (ItemAttr.HasFenceWithVariation(ID))  // Fence Variation
            {
                Path1 = GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + front, 16));
            }
            else
                Path1 = GetImagePathFromID(itemID, source, Data);

            if (P2Id == "FFFD")
                Path2 = GetImagePathFromID(P2Data, source);
            else
                Path2 = GetImagePathFromID(P2Id, source, IntP2Data);

            if (P3Id == "FFFD")
                Path3 = GetImagePathFromID(P3Data, source);
            else
                Path3 = GetImagePathFromID(P3Id, source, IntP3Data);

            if (P4Id == "FFFD")
                Path4 = GetImagePathFromID(P4Data, source);
            else
                Path4 = GetImagePathFromID(P4Id, source, IntP4Data);

            btn.Setup(Name, ID, Data, IntP2Id, IntP2Data, IntP3Id, IntP3Data, IntP4Id, IntP4Data, Path1, Path2, Path3, Path4, ContainPath, flag0, flag1);
        }

        private async Task UpdateBtn(FloorSlot btn)
        {
            byte[] Left = new byte[16];
            byte[] Right = new byte[16];

            if (layer1Btn.Checked)
            {
                Buffer.BlockCopy(Layer1, (btn.MapX + 16) * 0xC00 + btn.MapY * 0x10, Left, 0, 16);
                Buffer.BlockCopy(Layer1, (btn.MapX + 16) * 0xC00 + 0x600 + btn.MapY * 0x10, Right, 0, 16);
            }
            else if (layer2Btn.Checked)
            {
                Buffer.BlockCopy(Layer2, (btn.MapX + 16) * 0xC00 + btn.MapY * 0x10, Left, 0, 16);
                Buffer.BlockCopy(Layer2, (btn.MapX + 16) * 0xC00 + 0x600 + btn.MapY * 0x10, Right, 0, 16);
            }

            string LeftStr = Utilities.ByteToHexString(Left);
            string RightStr = Utilities.ByteToHexString(Right);

            string P1Id = Utilities.Flip(LeftStr.Substring(0, 4));
            string flag1 = LeftStr.Substring(4, 2);
            string flag0 = LeftStr.Substring(6, 2);
            string P1Data = Utilities.Flip(LeftStr.Substring(8, 8));
            string P2Id = Utilities.Flip(LeftStr.Substring(16, 4));
            string P2Data = Utilities.Flip(LeftStr.Substring(24, 8));

            string P3Id = Utilities.Flip(RightStr.Substring(0, 4));
            string P3Data = Utilities.Flip(RightStr.Substring(8, 8));
            string P4Id = Utilities.Flip(RightStr.Substring(16, 4));
            string P4Data = Utilities.Flip(RightStr.Substring(24, 8));

            string Path1 = "";
            string Path2 = "";
            string Path3 = "";
            string Path4 = "";
            string ContainPath = "";


            string Name = GetNameFromID(P1Id, source);
            UInt16 ID = Convert.ToUInt16("0x" + P1Id, 16);
            UInt32 Data = Convert.ToUInt32("0x" + P1Data, 16);
            UInt32 IntP2Id = Convert.ToUInt32("0x" + P2Id, 16);
            UInt32 IntP2Data = Convert.ToUInt32("0x" + P2Data, 16);
            UInt32 IntP3Id = Convert.ToUInt32("0x" + P3Id, 16);
            UInt32 IntP3Data = Convert.ToUInt32("0x" + P3Data, 16);
            UInt32 IntP4Id = Convert.ToUInt32("0x" + P4Id, 16);
            UInt32 IntP4Data = Convert.ToUInt32("0x" + P4Data, 16);

            string front = P1Data.Substring(0, 4);
            string back = P1Data.Substring(4, 4);

            if (P1Id == "FFFD")
                Path1 = GetImagePathFromID(back, source);
            else if (P1Id == "16A2")
            {
                Path1 = GetImagePathFromID(back, recipeSource, Data);
                Name = GetNameFromID(back, recipeSource) + " (Recipe)";
            }
            else if (P1Id == "114A" || P1Id == "EC9C")
            {
                Path1 = GetImagePathFromID(P1Id, source, Data);
                ContainPath = GetImagePathFromID(back, source);
                Name = GetNameFromID(P1Id, source) + " (" + GetNameFromID(back, source) + ")";
            }
            else if (P1Id == "315A" || P1Id == "1618" || P1Id == "342F")
            {
                Path1 = GetImagePathFromID(P1Id, source, Data);
                ContainPath = GetImagePathFromID(back, source, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16));
            }
            else if (ItemAttr.HasFenceWithVariation(ID))  // Fence Variation
            {
                Path1 = GetImagePathFromID(P1Id, source, Convert.ToUInt32("0x" + front, 16));
            }
            else
                Path1 = GetImagePathFromID(P1Id, source, Data);

            if (P2Id == "FFFD")
                Path2 = GetImagePathFromID(P2Data.Substring(4, 4), source);
            else
                Path2 = GetImagePathFromID(P2Id, source, IntP2Data);

            if (P3Id == "FFFD")
                Path3 = GetImagePathFromID(P3Data.Substring(4, 4), source);
            else
                Path3 = GetImagePathFromID(P3Id, source, IntP3Data);

            if (P4Id == "FFFD")
                Path4 = GetImagePathFromID(P4Data.Substring(4, 4), source);
            else
                Path4 = GetImagePathFromID(P4Id, source, IntP4Data);

            await btn.Setup(Name, ID, Data, IntP2Id, IntP2Data, IntP3Id, IntP3Data, IntP4Id, IntP4Data, Path1, Path2, Path3, Path4, ContainPath, flag0, flag1);
            await btn.setImage(false);
        }

        private async Task UpdateNearBtn(int BtnNum)
        {
            if (shiftRight && BtnNum < 42)
                UpdateBtn(floorSlots[BtnNum + 7]);
            if (shiftDown && ((BtnNum + 1) % 7) != 0)
                UpdateBtn(floorSlots[BtnNum + 1]);
            if (shiftRight && shiftDown && ((BtnNum + 1) % 7) != 0 && BtnNum < 42)
                UpdateBtn(floorSlots[BtnNum + 8]);
        }

        private bool isUpdating = false;

        private async Task UpdateAllBtn()
        {
            /*
            for (int i = 0; i < floorSlots.Length; i++)
            {
                UpdateBtn(floorSlots[i]);
            }
            */

            var tasks = new List<Task>();
            for (int i = 0; i < floorSlots.Length; i++)
            {
                tasks.Add(UpdateBtn(floorSlots[i]));
                //UpdateBtn(floorSlots[i]);
            }
            await Task.WhenAll(tasks);
        }

        /*
        private async Task UpdateAllBtn()
        {
            if (isUpdating)
            {
                return;
            }

            isUpdating = true;

            try
            {
                //var tasks = new List<Task>();
                for (int i = 0; i < floorSlots.Length; i++)
                {
                    //tasks.Add(UpdateBtn(floorSlots[i]));
                    UpdateBtn(floorSlots[i]);
                }
                //await Task.WhenAll(tasks);


                // Do ALL heavy work in parallel on background threads
                var imageLoadTasks = new List<Task<(int index, Image image)>>();

                for (int i = 0; i < floorSlots.Length; i++)
                {
                    int index = i; // Capture index for closure
                    imageLoadTasks.Add(Task.Run(async () =>
                    {
                        var image = await floorSlots[index].LoadImageForSlot(false); // Your heavy work here
                        return (index, image);
                    }));
                }

                // Wait for all background work to complete
                var results = await Task.WhenAll(imageLoadTasks);

                foreach (var (index, image) in results)
                {
                    floorSlots[index].Image = image;
                }
            }
            finally
            {
                isUpdating = false;
            }
        }
        */

        #endregion

        private void DisplayAnchorAsync()
        {
            if (drawing)
                return;

            drawing = true;
            miniMapBox.Image = MiniMap.DrawSelectSquare(anchorX, anchorY);

            SetupBtnCoordinate(anchorX, anchorY);

            UpdateAllBtn();

            ResetBtnColor();

            drawing = false;
        }

        private static UInt32 GetAddress(int x, int y, bool right = false, bool down = false)
        {
            int shiftRightValue = 0;
            int shiftDownValue = 0;

            if (right)
                shiftRightValue = 0x600;
            if (down)
                shiftDownValue = 0x8;

            return (UInt32)(Utilities.mapZero + (0xC00 * (x + 16) + shiftRightValue) + (0x10 * y + shiftDownValue));
        }

        #region Arrow Buttons
        private void MoveRightBtn_Click(object sender, EventArgs e)
        {
            int newX = anchorX + 1;
            int newY = anchorY;

            if (newX < 3 || newY < 3 || newX > 108 || newY > 92)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            MoveAnchor(newX, newY);
        }

        private void MoveLeftBtn_Click(object sender, EventArgs e)
        {
            int newX = anchorX - 1;
            int newY = anchorY;

            if (newX < 3 || newY < 3 || newX > 108 || newY > 92)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            MoveAnchor(newX, newY);
        }

        private void MoveDownBtn_Click(object sender, EventArgs e)
        {
            int newX = anchorX;
            int newY = anchorY + 1;

            if (newX < 3 || newY < 3 || newX > 108 || newY > 92)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            MoveAnchor(newX, newY);
        }

        private void MoveUpBtn_Click(object sender, EventArgs e)
        {
            int newX = anchorX;
            int newY = anchorY - 1;

            if (newX < 3 || newY < 3 || newX > 108 || newY > 92)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            MoveAnchor(newX, newY);
        }

        private void MoveUpRightBtn_Click(object sender, EventArgs e)
        {
            int newX = anchorX + 1;
            int newY = anchorY - 1;

            if (newX < 3 || newY < 3 || newX > 108 || newY > 92)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            MoveAnchor(newX, newY);
        }

        private void MoveDownRightBtn_Click(object sender, EventArgs e)
        {
            int newX = anchorX + 1;
            int newY = anchorY + 1;

            if (newX < 3 || newY < 3 || newX > 108 || newY > 92)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            MoveAnchor(newX, newY);
        }

        private void MoveDownLeftBtn_Click(object sender, EventArgs e)
        {
            int newX = anchorX - 1;
            int newY = anchorY + 1;

            if (newX < 3 || newY < 3 || newX > 108 || newY > 92)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            MoveAnchor(newX, newY);
        }

        private void MoveUpLeftBtn_Click(object sender, EventArgs e)
        {
            int newX = anchorX - 1;
            int newY = anchorY - 1;

            if (newX < 3 || newY < 3 || newX > 108 || newY > 92)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            MoveAnchor(newX, newY);
        }

        private void MoveUp7Btn_Click(object sender, EventArgs e)
        {
            if (anchorY <= 3)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            int newX = anchorX;
            int newY = anchorY - 7;

            if (newY < 3)
                newY = 3;

            MoveAnchor(newX, newY);
        }

        private void MoveRight7Btn_Click(object sender, EventArgs e)
        {
            if (anchorX >= 108)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            int newX = anchorX + 7;
            int newY = anchorY;

            if (newX > 108)
                newX = 108;

            MoveAnchor(newX, newY);
        }

        private void MoveDown7Btn_Click(object sender, EventArgs e)
        {
            if (anchorY >= 92)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            int newX = anchorX;
            int newY = anchorY + 7;

            if (newY > 92)
                newY = 92;

            MoveAnchor(newX, newY);
        }

        private void MoveLeft7Btn_Click(object sender, EventArgs e)
        {
            if (anchorX <= 3)
            {
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            int newX = anchorX - 7;
            int newY = anchorY;

            if (newX < 3)
                newX = 3;

            MoveAnchor(newX, newY);
        }

        #endregion

        #region Tooltip
        private void Floor_MouseHover(object sender, EventArgs e)
        {
            var button = (FloorSlot)sender;

            string name = button.ItemName;
            if (name.Equals(string.Empty))
            {
                if (button.IsExtension())
                {
                    name = GetNameFromID(Utilities.PrecedingZeros((button.ItemData & 0x0000FFFF).ToString("X"), 4), source);

                    if (name.Equals(string.Empty))
                    {
                        name = "[ Extension ]";
                    }
                    else
                    {
                        name = "[ Extension of " + name + " ]";
                    }
                }
            }
            /*
            string locked;
            if (button.locked)
                locked = "✓ True";
            else
                locked = "✘ False";
            */
            string activateInfo;
            if (layer1Btn.Checked)
                activateInfo = DisplayActivate(button.MapX, button.MapY, ActivateTable1);
            else
                activateInfo = DisplayActivate(button.MapX, button.MapY, ActivateTable2);

            btnToolTip.SetToolTip(button,
                                    name +
                                    "\n\n" + "" +
                                    "ID : " + Utilities.PrecedingZeros(button.ItemID.ToString("X"), 4) + "\n" +
                                    "Count : " + Utilities.PrecedingZeros(button.ItemData.ToString("X"), 8) + "\n" +
                                    "Flag0 : 0x" + button.Flag0 + "\n" +
                                    "Flag1 : 0x" + button.Flag1 + "\n" +
                                    "Coordinate : " + button.MapX + " " + button.MapY + "\n\n" +
                                    "Part2 : " + Utilities.PrecedingZeros(button.Part2.ToString("X"), 4) + " " + Utilities.PrecedingZeros(button.Part2Data.ToString("X"), 8) + "\n" +
                                    "Part3 : " + Utilities.PrecedingZeros(button.Part3.ToString("X"), 4) + " " + Utilities.PrecedingZeros(button.Part3Data.ToString("X"), 8) + "\n" +
                                    "Part4 : " + Utilities.PrecedingZeros(button.Part4.ToString("X"), 4) + " " + Utilities.PrecedingZeros(button.Part4Data.ToString("X"), 8) + "\n" +
                                    //"Locked : " + locked + 
                                    "Terrain : " + miniMap.GetTerrainData(button.MapX, button.MapY) + "\n" +
                                    "Activate : " + activateInfo
                                    );
        }
        #endregion

        #region Images
        private static string RemoveNumber(string filename)
        {
            char[] MyChar = ['0', '1', '2', '3', '4'];
            return filename.Trim(MyChar);
        }

        public string GetImagePathFromID(string itemID, DataTable source, UInt32 data = 0)
        {
            if (source == null || itemID == "FFFE")
            {
                return "";
            }

            if (fieldSource != null)
            {
                string path;

                DataRow FieldRow = fieldSource.Rows.Find(itemID);
                if (FieldRow != null)
                {
                    string imageName = FieldRow[1].ToString();

                    if (OverrideDict.TryGetValue(imageName, out string value))
                    {
                        path = imagePath + value + "_Remake_0_0.png";
                        if (File.Exists(path))
                        {
                            return path;
                        }
                        else
                        {
                            path = imagePath + OverrideDict[imageName] + ".png";
                            if (File.Exists(path))
                            {
                                return path;
                            }
                        }
                    }
                    else
                    {
                        path = imagePath + imageName + "_Remake_0_0.png";
                        if (File.Exists(path))
                        {
                            return path;
                        }
                        else
                        {
                            path = imagePath + imageName + ".png";
                            if (File.Exists(path))
                            {
                                return path;
                            }
                            else
                            {
                                path = imagePath + RemoveNumber(imageName) + ".png";
                                if (File.Exists(path))
                                {
                                    return path;
                                }
                            }
                        }
                    }
                }

            }

            DataRow row = source.Rows.Find(itemID);
            DataRow VarRow = null;
            if (variationSource != null)
                VarRow = variationSource.Rows.Find(itemID);

            if (row == null)
            {
                return ""; //row not found
            }
            else
            {

                string path;
                if (VarRow != null & source != recipeSource)
                {
                    string main = (data & 0xF).ToString();
                    string sub = (((data & 0xFF) - (data & 0xF)) / 0x20).ToString();
                    //Debug.Print("data " + data.ToString("X") + " Main " + main + " Sub " + sub);
                    path = imagePath + VarRow["iName"] + "_Remake_" + main + "_" + sub + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }

                    path = imagePath + VarRow["iName"] + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }

                string imageName = row[1].ToString();

                if (OverrideDict.TryGetValue(imageName, out string value))
                {
                    path = imagePath + value + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }
                    else
                    {
                        path = imagePath + imageName + "_Remake_0_0.png";
                        if (File.Exists(path))
                        {
                            return path;
                        }
                    }
                }

                path = imagePath + imageName + "_Remake_0_0.png";
                if (File.Exists(path))
                {
                    return path;
                }
                else
                {
                    path = imagePath + imageName + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }
                    else
                    {
                        path = imagePath + RemoveNumber(imageName) + ".png";
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

        private string GetSize(string itemID)
        {
            DataRow row = source.Rows.Find(itemID);

            if (row == null)
            {
                return ""; //row not found
            }
            else
            {
                return (string)row["size"];
            }
        }

        public string GetNameFromID(string itemID, DataTable table)
        {
            string ItemName = "";

            if (itemID == "FFFE")
                return ItemName;

            if (fieldSource != null)
            {
                DataRow FieldRow = fieldSource.Rows.Find(itemID);
                if (FieldRow != null)
                {
                    ItemName = (string)FieldRow["name"];
                    return ItemName;
                }
            }

            if (table == null)
            {
                return ItemName;
            }

            DataRow row = table.Rows.Find(itemID);

            if (row == null)
            {
                return ItemName; //row not found
            }
            else
            {
                ItemName = (string)row[languageSetting];
            }

            return ItemName;
        }

        private void FieldGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            AddImage(FieldGridView, e);
        }

        private void AddImage(DataGridView Grid, DataGridViewCellFormattingEventArgs e)
        {
            if (Grid.Columns["Image"] == null)
                return;
            if (e.RowIndex >= 0 && e.RowIndex < Grid.Rows.Count)
            {
                if (e.ColumnIndex == Grid.Columns["Image"].Index)
                {
                    string path;
                    if (Grid.Rows[e.RowIndex].Cells["iName"].Value == null)
                        return;
                    string imageName = Grid.Rows[e.RowIndex].Cells["iName"].Value.ToString();

                    if (OverrideDict.TryGetValue(imageName, out string value))
                    {
                        path = imagePath + value + "_Remake_0_0.png";
                        if (File.Exists(path))
                        {
                            Image img = ImageCacher.GetImage(path);
                            e.CellStyle.BackColor = Color.Green;
                            e.Value = img;

                            return;
                        }
                        else
                        {
                            path = imagePath + OverrideDict[imageName] + ".png";
                            if (File.Exists(path))
                            {
                                Image img = ImageCacher.GetImage(path);
                                e.CellStyle.BackColor = Color.Green;
                                e.Value = img;

                                return;
                            }
                        }
                    }

                    path = imagePath + imageName + "_Remake_0_0.png";
                    if (File.Exists(path))
                    {
                        Image img = ImageCacher.GetImage(path);
                        e.CellStyle.BackColor = Color.FromArgb(56, 77, 162);
                        e.Value = img;
                    }
                    else
                    {
                        path = imagePath + imageName + ".png";
                        if (File.Exists(path))
                        {
                            Image img = ImageCacher.GetImage(path);
                            e.Value = img;
                        }
                        else
                        {
                            path = imagePath + RemoveNumber(imageName) + ".png";
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
        #endregion

        #region GridView Control

        private void FieldGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isHalfTileItem = false;
                isPillar = false;
                ShiftRightToggle.Checked = false;
                ShiftDownToggle.Checked = false;

                if (lastRow != null)
                {
                    lastRow.Height = 22;
                }

                if (e.RowIndex > -1)
                {
                    lastRow = FieldGridView.Rows[e.RowIndex];
                    FieldGridView.Rows[e.RowIndex].Height = 128;

                    if (currentDataTable == source)
                    {
                        string id = FieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();
                        string name = FieldGridView.Rows[e.RowIndex].Cells[languageSetting].Value.ToString();

                        IdTextbox.Text = id;
                        HexTextbox.Text = "00000000";
                        selectedSize = FieldGridView.Rows[e.RowIndex].Cells["size"].Value.ToString();
                        if (selectedSize.Contains("Pillar"))
                            isPillar = true;
                        if (selectedSize.Contains("x0_5") && !selectedSize.Contains("_Wall") && !selectedSize.Contains("_Pillar"))
                            isHalfTileItem = true;
                        SizeBox.Text = selectedSize.Replace("_5", ".5 ").Replace("_0", ".0 ").Replace("_Wall", "Wall").Replace("_Rug", "Rug").Replace("_Pillar", "Pillar").Replace("_Ceiling", "Ceiling").Replace("x", "x ");

                        selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), 0x0, GetImagePathFromID(id, source), true, "");

                        if (ModifierKeys == Keys.Control && FieldGridView.MultiSelect == false)
                        {
                            AddToListBtn_MouseClick(sender, e);
                        }
                    }
                    else if (currentDataTable == recipeSource)
                    {
                        string id = "16A2"; // Recipe;
                        string name = FieldGridView.Rows[e.RowIndex].Cells[languageSetting].Value.ToString();
                        string hexValue = FieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();

                        IdTextbox.Text = id;
                        HexTextbox.Text = Utilities.PrecedingZeros(hexValue, 8);
                        SizeBox.Text = "";

                        selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(hexValue, recipeSource), true, "");
                    }
                    else if (currentDataTable == flowerSource)
                    {
                        string id = FieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();
                        string name = FieldGridView.Rows[e.RowIndex].Cells[languageSetting].Value.ToString();
                        string hexValue = FieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();

                        IdTextbox.Text = id;
                        HexTextbox.Text = Utilities.PrecedingZeros(hexValue, 8);

                        selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source), true, "");
                        SizeBox.Text = "";
                    }
                    else if (currentDataTable == favSource)
                    {
                        string id = FieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();
                        string name = FieldGridView.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                        string hexValue = FieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();

                        IdTextbox.Text = id;
                        HexTextbox.Text = Utilities.PrecedingZeros(hexValue, 8);
                        SizeBox.Text = "";

                        selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + hexValue, 16)), true, "");
                    }
                    else if (currentDataTable == fieldSource)
                    {
                        string id = FieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();
                        string name = FieldGridView.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                        string hexValue = FieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();

                        IdTextbox.Text = id;
                        HexTextbox.Text = Utilities.PrecedingZeros(hexValue, 8);
                        SizeBox.Text = "";

                        selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, fieldSource), true, "");
                    }

                    if (selection != null)
                    {
                        string hexValue = "00000000";

                        if (currentDataTable == flowerSource)
                            hexValue = FieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();
                        else if (currentDataTable == favSource)
                            hexValue = FieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();
                        else if (currentDataTable == fieldSource)
                            hexValue = FieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();

                        selection.ReceiveID(Utilities.PrecedingZeros(selectedItem.FillItemID(), 4), languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                    }

                    //updateSelectedItemInfo(selectedItem.displayItemName(), selectedItem.displayItemID(), selectedItem.displayItemData());

                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (lastRow != null)
                {
                    lastRow.Height = 22;
                }

                if (e.RowIndex > -1)
                {
                    lastRow = FieldGridView.Rows[e.RowIndex];
                    FieldGridView.Rows[e.RowIndex].Height = 128;

                    string name = selectedItem.DisplayItemName();
                    string id = selectedItem.DisplayItemID();
                    string path = selectedItem.GetPath();

                    if (IdTextbox.Text != "")
                    {
                        if (IdTextbox.Text == "114A" || IdTextbox.Text == "EC9C") // Money Tree
                        {
                            HexTextbox.Text = @"0020" + Utilities.PrecedingZeros(FieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), 4);
                            selectedItem.Setup(name, Convert.ToUInt16(id, 16), Convert.ToUInt32("0x" + HexTextbox.Text, 16), path, true, GetImagePathFromID(FieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), source));
                        }
                        else if (IdTextbox.Text == "315A" || IdTextbox.Text == "1618" || IdTextbox.Text == "342F") // Wall-Mounted
                        {
                            HexTextbox.Text = Utilities.PrecedingZeros("00" + FieldGridView.Rows[e.RowIndex].Cells["id"].Value, 8);
                            selectedItem.Setup(name, Convert.ToUInt16(id, 16), Convert.ToUInt32("0x" + HexTextbox.Text, 16), path, true, GetImagePathFromID(FieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), source));
                        }
                        else
                        {
                            HexTextbox.Text = Utilities.PrecedingZeros(FieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), 8);
                            selectedItem.Setup(name, Convert.ToUInt16(id, 16), Convert.ToUInt32("0x" + HexTextbox.Text, 16), path, true, GetNameFromID(FieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), source));
                        }

                        selection?.ReceiveID(Utilities.Turn2bytes(selectedItem.FillItemData()), languageSetting);
                    }

                }
            }
        }

        private void FieldGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && ModifierKeys == Keys.Control && FieldGridView.MultiSelect == false)
            {
                EnableMultiSelectMenu.Show(FieldGridView, new Point(e.X, e.Y));
            }
        }

        private void ItemModeBtn_Click(object sender, EventArgs e)
        {
            itemModeBtn.BackColor = Color.FromArgb(80, 80, 255);
            recipeModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            flowerModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            favModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            //fieldModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            if (itemSearchBox.Text != "Search...")
            {
                itemSearchBox.Clear();
            }

            FieldGridView.Columns.Remove("Image");

            if (source != null)
            {
                FieldGridView.DataSource = source;

                //set the ID row invisible
                FieldGridView.Columns["id"].Visible = false;
                FieldGridView.Columns["iName"].Visible = false;
                FieldGridView.Columns["color"].Visible = false;
                FieldGridView.Columns["size"].Visible = false;
                FieldGridView.Columns["Kind"].Visible = false;

                if (FieldGridView.Columns.Contains(languageSetting))
                {
                    HideAllLanguage();
                    FieldGridView.Columns[languageSetting].Visible = true;
                }

                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                FieldGridView.Columns.Insert(13, imageColumn);
                imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                FieldGridView.Columns["eng"].Width = 195;
                FieldGridView.Columns["jpn"].Width = 195;
                FieldGridView.Columns["tchi"].Width = 195;
                FieldGridView.Columns["schi"].Width = 195;
                FieldGridView.Columns["kor"].Width = 195;
                FieldGridView.Columns["fre"].Width = 195;
                FieldGridView.Columns["ger"].Width = 195;
                FieldGridView.Columns["spa"].Width = 195;
                FieldGridView.Columns["ita"].Width = 195;
                FieldGridView.Columns["dut"].Width = 195;
                FieldGridView.Columns["rus"].Width = 195;
                FieldGridView.Columns["Image"].Width = 128;

                FieldGridView.Columns["eng"].HeaderText = "Name";
                FieldGridView.Columns["jpn"].HeaderText = "Name";
                FieldGridView.Columns["tchi"].HeaderText = "Name";
                FieldGridView.Columns["schi"].HeaderText = "Name";
                FieldGridView.Columns["kor"].HeaderText = "Name";
                FieldGridView.Columns["fre"].HeaderText = "Name";
                FieldGridView.Columns["ger"].HeaderText = "Name";
                FieldGridView.Columns["spa"].HeaderText = "Name";
                FieldGridView.Columns["ita"].HeaderText = "Name";
                FieldGridView.Columns["dut"].HeaderText = "Name";
                FieldGridView.Columns["rus"].HeaderText = "Name";

                currentDataTable = source;
            }

            FlagTextbox.Text = "20";

            FilterBtn.Enabled = true;
            ResetFilter();
            FieldGridView.ClearSelection();
        }

        private void RecipeModeBtn_Click(object sender, EventArgs e)
        {
            itemModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            recipeModeBtn.BackColor = Color.FromArgb(80, 80, 255);
            flowerModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            favModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            //fieldModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            if (itemSearchBox.Text != "Search...")
            {
                itemSearchBox.Clear();
            }

            FieldGridView.Columns.Remove("Image");

            if (recipeSource != null)
            {
                FieldGridView.DataSource = recipeSource;

                FieldGridView.Columns["id"].Visible = false;
                FieldGridView.Columns["iName"].Visible = false;

                if (FieldGridView.Columns.Contains(languageSetting))
                {
                    HideAllLanguage();
                    FieldGridView.Columns[languageSetting].Visible = true;
                }

                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                FieldGridView.Columns.Insert(13, imageColumn);
                imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                FieldGridView.Columns["eng"].Width = 195;
                FieldGridView.Columns["jpn"].Width = 195;
                FieldGridView.Columns["tchi"].Width = 195;
                FieldGridView.Columns["schi"].Width = 195;
                FieldGridView.Columns["kor"].Width = 195;
                FieldGridView.Columns["fre"].Width = 195;
                FieldGridView.Columns["ger"].Width = 195;
                FieldGridView.Columns["spa"].Width = 195;
                FieldGridView.Columns["ita"].Width = 195;
                FieldGridView.Columns["dut"].Width = 195;
                FieldGridView.Columns["rus"].Width = 195;
                FieldGridView.Columns["Image"].Width = 128;

                FieldGridView.Columns["eng"].HeaderText = "Name";
                FieldGridView.Columns["jpn"].HeaderText = "Name";
                FieldGridView.Columns["tchi"].HeaderText = "Name";
                FieldGridView.Columns["schi"].HeaderText = "Name";
                FieldGridView.Columns["kor"].HeaderText = "Name";
                FieldGridView.Columns["fre"].HeaderText = "Name";
                FieldGridView.Columns["ger"].HeaderText = "Name";
                FieldGridView.Columns["spa"].HeaderText = "Name";
                FieldGridView.Columns["ita"].HeaderText = "Name";
                FieldGridView.Columns["dut"].HeaderText = "Name";
                FieldGridView.Columns["rus"].HeaderText = "Name";

                currentDataTable = recipeSource;
            }

            FlagTextbox.Text = "00";

            FilterBtn.Enabled = false;
            ResetFilter();
            FieldGridView.ClearSelection();
        }

        private void FlowerModeBtn_Click(object sender, EventArgs e)
        {
            itemModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            recipeModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            flowerModeBtn.BackColor = Color.FromArgb(80, 80, 255);
            favModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            //fieldModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            if (itemSearchBox.Text != "Search...")
            {
                itemSearchBox.Clear();
            }

            FieldGridView.Columns.Remove("Image");

            if (flowerSource != null)
            {
                FieldGridView.DataSource = flowerSource;

                FieldGridView.Columns["id"].Visible = false;
                FieldGridView.Columns["iName"].Visible = false;
                FieldGridView.Columns["value"].Visible = false;

                if (FieldGridView.Columns.Contains(languageSetting))
                {
                    HideAllLanguage();
                    FieldGridView.Columns[languageSetting].Visible = true;
                }

                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                FieldGridView.Columns.Insert(13, imageColumn);
                imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                FieldGridView.Columns["eng"].Width = 195;
                FieldGridView.Columns["jpn"].Width = 195;
                FieldGridView.Columns["tchi"].Width = 195;
                FieldGridView.Columns["schi"].Width = 195;
                FieldGridView.Columns["kor"].Width = 195;
                FieldGridView.Columns["fre"].Width = 195;
                FieldGridView.Columns["ger"].Width = 195;
                FieldGridView.Columns["spa"].Width = 195;
                FieldGridView.Columns["ita"].Width = 195;
                FieldGridView.Columns["dut"].Width = 195;
                FieldGridView.Columns["rus"].Width = 195;
                FieldGridView.Columns["Image"].Width = 128;

                FieldGridView.Columns["eng"].HeaderText = "Name";
                FieldGridView.Columns["jpn"].HeaderText = "Name";
                FieldGridView.Columns["tchi"].HeaderText = "Name";
                FieldGridView.Columns["schi"].HeaderText = "Name";
                FieldGridView.Columns["kor"].HeaderText = "Name";
                FieldGridView.Columns["fre"].HeaderText = "Name";
                FieldGridView.Columns["ger"].HeaderText = "Name";
                FieldGridView.Columns["spa"].HeaderText = "Name";
                FieldGridView.Columns["ita"].HeaderText = "Name";
                FieldGridView.Columns["dut"].HeaderText = "Name";
                FieldGridView.Columns["rus"].HeaderText = "Name";

                currentDataTable = flowerSource;
            }

            FlagTextbox.Text = "20";

            FilterBtn.Enabled = false;
            ResetFilter();
            FieldGridView.ClearSelection();
        }

        private void FavModeBtn_Click(object sender, EventArgs e)
        {
            itemModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            recipeModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            flowerModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            favModeBtn.BackColor = Color.FromArgb(80, 80, 255);
            //fieldModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            if (itemSearchBox.Text != "Search...")
            {
                itemSearchBox.Clear();
            }

            FieldGridView.Columns.Remove("Image");

            if (favSource != null)
            {
                FieldGridView.DataSource = favSource;

                FieldGridView.Columns["id"].Visible = false;
                FieldGridView.Columns["iName"].Visible = false;
                FieldGridView.Columns["Name"].Visible = true;
                FieldGridView.Columns["value"].Visible = false;
                FieldGridView.Columns["Kind"].Visible = false;

                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                FieldGridView.Columns.Insert(4, imageColumn);
                imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                FieldGridView.Columns["Name"].Width = 195;
                FieldGridView.Columns["Image"].Width = 128;

                currentDataTable = favSource;
            }

            FlagTextbox.Text = "20";

            FilterBtn.Enabled = true;
            ResetFilter();
            FieldGridView.ClearSelection();
        }

        private void FieldModeBtn_Click(object sender, EventArgs e)
        {
            FlashTimer.Stop();

            itemModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            recipeModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            flowerModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            favModeBtn.BackColor = Color.FromArgb(114, 137, 218);
            //fieldModeBtn.BackColor = Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));

            if (itemSearchBox.Text != "Search...")
            {
                itemSearchBox.Clear();
            }

            FieldGridView.Columns.Remove("Image");

            if (favSource != null)
            {
                FieldGridView.DataSource = fieldSource;

                FieldGridView.Columns["id"].Visible = false;
                FieldGridView.Columns["iName"].Visible = false;
                FieldGridView.Columns["name"].Visible = true;
                FieldGridView.Columns["value"].Visible = false;

                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                FieldGridView.Columns.Insert(3, imageColumn);
                imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                FieldGridView.Columns["name"].Width = 195;
                FieldGridView.Columns["Image"].Width = 128;

                FieldGridView.Columns["name"].HeaderText = "Name";

                currentDataTable = fieldSource;
            }

            FlagTextbox.Text = "00";

            FilterBtn.Enabled = false;
            ResetFilter();
            FieldGridView.ClearSelection();
        }

        private static DataTable LoadItemCSV(string filePath, bool key = true)
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

            if (key)
            {
                if (dt.Columns.Contains("id"))
                    dt.PrimaryKey = [dt.Columns["id"]];
            }

            return dt;
        }

        private static DataTable LoadItemCSVWithKind(string filePath, bool key = true)
        {
            var dt = new DataTable();

            var lines = File.ReadLines(filePath);
            foreach (var line in lines.Take(1))
            {
                var headers = (line + "Kind ; ").Split([" ; "], StringSplitOptions.RemoveEmptyEntries);
                foreach (var header in headers)
                {
                    dt.Columns.Add(header.Trim());
                }
            }

            foreach (var line in lines.Skip(1))
            {
                string[] dataRow = line.Split([" ; "], StringSplitOptions.RemoveEmptyEntries);
                string id = dataRow[0];

                if (Utilities.itemkind.TryGetValue(id, out string value))
                    dataRow = dataRow.ToList().Append(value).ToArray();
                else
                    _ = dataRow.ToList().Append("Null").ToArray();

                dt.Rows.Add(dataRow);
            }

            if (key)
            {
                if (dt.Columns.Contains("id"))
                    dt.PrimaryKey = [dt.Columns["id"]];
            }

            return dt;
        }

        private void ItemSearchBox_TextChanged(object sender, EventArgs e)
        {
            if (itemSearchBox.Text.Equals("Search...") || itemSearchBox.Text.Equals(String.Empty))
            {
                if (filterOn)
                {
                    if (currentDataTable == source || currentDataTable == favSource)
                    {
                        ((DataTable)FieldGridView.DataSource).DefaultView.RowFilter = "Kind = '" + filterKind + "'";
                        FieldGridView.ClearSelection();
                    }
                    return;
                }
                else
                {
                    ((DataTable)FieldGridView.DataSource).DefaultView.RowFilter = null;
                    FieldGridView.ClearSelection();
                    return;
                }
            }

            try
            {
                if (FieldGridView.DataSource != null)
                {
                    if (currentDataTable == source)
                    {
                        if (filterOn)
                        {
                            ((DataTable)FieldGridView.DataSource).DefaultView.RowFilter = string.Format(languageSetting + " LIKE '%{0}%' AND Kind = '{1}'", EscapeLikeValue(itemSearchBox.Text), filterKind);
                        }
                        else
                            ((DataTable)FieldGridView.DataSource).DefaultView.RowFilter = string.Format(languageSetting + " LIKE '%{0}%'", EscapeLikeValue(itemSearchBox.Text));
                    }
                    else if (currentDataTable == recipeSource)
                    {
                        ((DataTable)FieldGridView.DataSource).DefaultView.RowFilter = string.Format(languageSetting + " LIKE '%{0}%'", EscapeLikeValue(itemSearchBox.Text));
                    }
                    else if (currentDataTable == flowerSource)
                    {
                        ((DataTable)FieldGridView.DataSource).DefaultView.RowFilter = string.Format(languageSetting + " LIKE '%{0}%'", EscapeLikeValue(itemSearchBox.Text));
                    }
                    else if (currentDataTable == favSource)
                    {
                        if (filterOn)
                        {
                            ((DataTable)FieldGridView.DataSource).DefaultView.RowFilter = string.Format("name" + " LIKE '%{0}%' AND Kind = '{1}'", EscapeLikeValue(itemSearchBox.Text), filterKind);
                        }
                        else
                            ((DataTable)FieldGridView.DataSource).DefaultView.RowFilter = string.Format("name" + " LIKE '%{0}%'", EscapeLikeValue(itemSearchBox.Text));
                    }
                    else if (currentDataTable == fieldSource)
                    {
                        ((DataTable)FieldGridView.DataSource).DefaultView.RowFilter = string.Format("name" + " LIKE '%{0}%'", EscapeLikeValue(itemSearchBox.Text));
                    }
                }
            }
            catch
            {
                itemSearchBox.Clear();
            }
            FieldGridView.ClearSelection();
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
            if (itemSearchBox.Text == "Search...")
            {
                itemSearchBox.Text = "";
                itemSearchBox.ForeColor = Color.White;
            }
        }

        #endregion

        #region Hotkeys
        private void Floor_MouseDown(object sender, MouseEventArgs e)
        {
            var button = (FloorSlot)sender;

            if (ModifierKeys == Keys.Control)
            {
                SetCorner(button);
            }
            else
            {
                selectedButton = button;

                if (ModifierKeys == Keys.Shift)
                {
                    SelectedItem_Click(sender, e);
                }
                else if (ModifierKeys == Keys.Alt)
                {
                    DeleteItem(button);
                }
            }

            ResetBtnColor();
        }

        private void DropItem(FloorSlot btn)
        {
            long address;

            if (layer1Btn.Checked)
            {
                address = GetAddress(btn.MapX, btn.MapY);
            }
            else if (layer2Btn.Checked)
            {
                address = GetAddress(btn.MapX, btn.MapY) + Utilities.mapSize;
            }
            else
                return;

            string itemID = Utilities.PrecedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.PrecedingZeros(HexTextbox.Text, 8);
            string flag1 = Utilities.PrecedingZeros(FlagTextbox.Text, 2);
            string flag0 = selectedItem.GetFlag0();

            MoveToNextTile();

            Thread spawnThread = new(delegate () { DropItem(address, itemID, itemData, flag0, flag1, btn); });
            spawnThread.Start();
        }

        private void DeleteItem(FloorSlot btn)
        {
            long address;

            if (layer1Btn.Checked)
                address = GetAddress(btn.MapX, btn.MapY);
            else if (layer2Btn.Checked)
                address = GetAddress(btn.MapX, btn.MapY) + Utilities.mapSize;
            else
                return;

            DisableBtn();

            Thread deleteThread = new(delegate () { DeleteItem(address, btn); });
            deleteThread.Start();
        }

        private void CopyItem(FloorSlot btn)
        {
            string id = Utilities.PrecedingZeros(btn.ItemID.ToString("X"), 4);
            string name = btn.Name;
            string hexValue = Utilities.PrecedingZeros(btn.ItemData.ToString("X"), 8);
            string flag0 = btn.Flag0;
            string flag1 = btn.Flag1;

            IdTextbox.Text = id;
            HexTextbox.Text = hexValue;
            FlagTextbox.Text = flag1;

            UInt16 IntId = Convert.ToUInt16("0x" + id, 16);
            string front = Utilities.PrecedingZeros(hexValue, 8).Substring(0, 4);
            string back = Utilities.PrecedingZeros(hexValue, 8).Substring(4, 4);


            if (id == "16A2")
                selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(hexValue), recipeSource), true, "", flag0, flag1);
            else if (id == "114A" || id == "EC9C") // Money Tree
                selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source), true, GetImagePathFromID(back, source), flag0, flag1);
            else if (ItemAttr.HasFenceWithVariation(IntId))  // Fence Variation
                selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + front, 16)), true, "", flag0, flag1);
            else if (id == "315A" || id == "1618" || id == "342F")
                selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID(back, source, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16)), flag0, flag1);
            else
                selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + hexValue, 16)), true, "", flag0, flag1);

            if (id == "315A" || id == "1618" || id == "342F")
                selectedSize = GetSize(back);
            else
                selectedSize = GetSize(id);
            SizeBox.Text = selectedSize.Replace("_5", ".5 ").Replace("_0", ".0 ").Replace("_Wall", "Wall").Replace("_Rug", "Rug").Replace("_Pillar", "Pillar").Replace("_Ceiling", "Ceiling").Replace("x", "x ");
        }

        private void KeyboardKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "F2" || e.KeyCode.ToString() == "Insert")
            {
                if (selectedButton != null & (s != null || usb != null))
                {
                    DropItem(selectedButton);
                }
            }
            else if (e.KeyCode.ToString() == "F1") // Delete
            {
                if (selectedButton != null & (s != null || usb != null))
                {
                    DeleteItem(selectedButton);
                }
            }
            else if (e.KeyCode.ToString() == "F3") // Copy
            {
                if (selectedButton != null & (s != null || usb != null))
                {
                    CopyItem(selectedButton);
                }
            }
            else if (e.KeyCode.ToString() == "End")
            {
                if (FieldGridView.Rows.Count <= 0)
                {
                    return;
                }
                else if (FieldGridView.Rows.Count == 1)
                {
                    lastRow = FieldGridView.Rows[FieldGridView.CurrentRow.Index];
                    FieldGridView.Rows[FieldGridView.CurrentRow.Index].Height = 160;

                    KeyPressSetup(FieldGridView.CurrentRow.Index);
                }
                else if (FieldGridView.CurrentRow.Index + 1 < FieldGridView.Rows.Count)
                {
                    if (lastRow != null)
                    {
                        lastRow.Height = 22;
                    }
                    lastRow = FieldGridView.Rows[FieldGridView.CurrentRow.Index + 1];
                    FieldGridView.Rows[FieldGridView.CurrentRow.Index + 1].Height = 160;

                    KeyPressSetup(FieldGridView.CurrentRow.Index + 1);
                    FieldGridView.CurrentCell = FieldGridView.Rows[FieldGridView.CurrentRow.Index + 1].Cells[FieldGridView.CurrentCell.ColumnIndex];
                }

                if (selection != null)
                {
                    string hexValue = "00000000";

                    if (currentDataTable == flowerSource)
                        hexValue = FieldGridView.Rows[FieldGridView.CurrentRow.Index + 1].Cells["value"].Value.ToString();
                    else if (currentDataTable == favSource)
                        hexValue = FieldGridView.Rows[FieldGridView.CurrentRow.Index + 1].Cells["value"].Value.ToString();
                    else if (currentDataTable == fieldSource)
                        hexValue = FieldGridView.Rows[FieldGridView.CurrentRow.Index + 1].Cells["value"].Value.ToString();

                    selection.ReceiveID(Utilities.PrecedingZeros(selectedItem.FillItemID(), 4), languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                }

            }
            else if (e.KeyCode.ToString() == "Home")
            {
                if (FieldGridView.Rows.Count <= 0)
                {
                    return;
                }
                else if (FieldGridView.Rows.Count == 1)
                {
                    lastRow = FieldGridView.Rows[FieldGridView.CurrentRow.Index];
                    FieldGridView.Rows[FieldGridView.CurrentRow.Index].Height = 160;

                    KeyPressSetup(FieldGridView.CurrentRow.Index);
                }
                else if (FieldGridView.CurrentRow.Index > 0)
                {
                    if (lastRow != null)
                    {
                        lastRow.Height = 22;
                    }

                    lastRow = FieldGridView.Rows[FieldGridView.CurrentRow.Index - 1];
                    FieldGridView.Rows[FieldGridView.CurrentRow.Index - 1].Height = 160;

                    KeyPressSetup(FieldGridView.CurrentRow.Index - 1);
                    FieldGridView.CurrentCell = FieldGridView.Rows[FieldGridView.CurrentRow.Index - 1].Cells[FieldGridView.CurrentCell.ColumnIndex];
                }

                if (selection != null)
                {
                    string hexValue = "00000000";

                    if (currentDataTable == flowerSource)
                        hexValue = FieldGridView.Rows[FieldGridView.CurrentRow.Index - 1].Cells["value"].Value.ToString();
                    else if (currentDataTable == favSource)
                        hexValue = FieldGridView.Rows[FieldGridView.CurrentRow.Index - 1].Cells["value"].Value.ToString();
                    else if (currentDataTable == fieldSource)
                        hexValue = FieldGridView.Rows[FieldGridView.CurrentRow.Index - 1].Cells["value"].Value.ToString();

                    selection.ReceiveID(Utilities.PrecedingZeros(selectedItem.FillItemID(), 4), languageSetting, Utilities.PrecedingZeros(hexValue, 8));
                }
            }
        }

        private void MoveToNextTile()
        {
            int index = int.Parse(selectedButton.Tag.ToString());
            if (index >= 48)
                selectedButton = floorSlots[0];
            else
                selectedButton = floorSlots[index + 1];
        }

        private void KeyPressSetup(int index)
        {
            if (currentDataTable == source)
            {
                string id = FieldGridView.Rows[index].Cells["id"].Value.ToString();
                string name = FieldGridView.Rows[index].Cells[languageSetting].Value.ToString();

                IdTextbox.Text = id;
                HexTextbox.Text = "00000000";
                selectedSize = FieldGridView.Rows[index].Cells["size"].Value.ToString();
                SizeBox.Text = selectedSize.Replace("_5", ".5 ").Replace("_0", ".0 ").Replace("_Wall", "Wall").Replace("_Rug", "Rug").Replace("_Pillar", "Pillar").Replace("_Ceiling", "Ceiling").Replace("x", "x ");


                selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), 0x0, GetImagePathFromID(id, source), true, "");
            }
            else if (currentDataTable == recipeSource)
            {
                string id = "16A2"; // Recipe;
                string name = FieldGridView.Rows[index].Cells[languageSetting].Value.ToString();
                string hexValue = FieldGridView.Rows[index].Cells["id"].Value.ToString();

                IdTextbox.Text = id;
                HexTextbox.Text = Utilities.PrecedingZeros(hexValue, 8);
                SizeBox.Text = "";

                selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(hexValue, recipeSource), true, "");
            }
            else if (currentDataTable == flowerSource)
            {
                string id = FieldGridView.Rows[index].Cells["id"].Value.ToString();
                string name = FieldGridView.Rows[index].Cells[languageSetting].Value.ToString();
                string hexValue = FieldGridView.Rows[index].Cells["value"].Value.ToString();

                IdTextbox.Text = id;
                HexTextbox.Text = Utilities.PrecedingZeros(hexValue, 8);
                SizeBox.Text = "";

                selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source), true, "");

            }
            else if (currentDataTable == favSource)
            {
                string id = FieldGridView.Rows[index].Cells["id"].Value.ToString();
                string name = FieldGridView.Rows[index].Cells["Name"].Value.ToString();
                string hexValue = FieldGridView.Rows[index].Cells["value"].Value.ToString();

                IdTextbox.Text = id;
                HexTextbox.Text = Utilities.PrecedingZeros(hexValue, 8);
                SizeBox.Text = "";

                selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source), true, "");
            }
            else if (currentDataTable == fieldSource)
            {
                string id = FieldGridView.Rows[index].Cells["id"].Value.ToString();
                string name = FieldGridView.Rows[index].Cells["Name"].Value.ToString();
                string hexValue = FieldGridView.Rows[index].Cells["value"].Value.ToString();

                IdTextbox.Text = id;
                HexTextbox.Text = Utilities.PrecedingZeros(hexValue, 8);
                SizeBox.Text = "";

                selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, fieldSource), true, "");
            }
        }
        #endregion

        #region Color
        private void ResetBtnColor()
        {
            foreach (FloorSlot btn in BtnPanel.Controls.OfType<FloorSlot>())
            {
                btn.FlatAppearance.BorderSize = 0;
                if (AreaCopied)
                {
                    if (layer1Btn.Checked)
                        btn.SetBackColor(true, Corner1X, Corner1Y, Corner2X, Corner2Y, true);
                    else
                        btn.SetBackColor(false, Corner1X, Corner1Y, Corner2X, Corner2Y, true);
                }
                else
                {
                    if (layer1Btn.Checked)
                        btn.SetBackColor(true, Corner1X, Corner1Y, Corner2X, Corner2Y);
                    else
                        btn.SetBackColor(false, Corner1X, Corner1Y, Corner2X, Corner2Y);
                }
            }

            if (selectedButton != null)
            {
                selectedButton.BackColor = Color.LightSeaGreen;
                selectedButton.FlatAppearance.BorderSize = 2;
                selectedButton.FlatAppearance.BorderColor = Color.Black;
            }
        }
        #endregion

        #region Single Spawn
        private void SelectedItem_Click(object sender, EventArgs e)
        {
            if (IdTextbox.Text == "" || HexTextbox.Text == "" || FlagTextbox.Text == "")
            {
                return;
            }

            if (ModifierKeys == Keys.Control)
            {
                if (Corner1X < 0 || Corner1Y < 0 || Corner2X < 0 || Corner2Y < 0)
                {
                    MyMessageBox.Show("Selection area Invalid!", "Do You Know Da Wae ?", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                AreaSpawn();

                return;
            }

            if (selectedButton == null)
            {
                MessageBox.Show(@"Please select a slot!");
                return;
            }

            if (selectedButton.MapX == 112 - 1)
            {
                shiftRight = false;
                ShiftRightToggle.Checked = false;
            }
            else
                shiftRight = ShiftRightToggle.Checked;

            if (selectedButton.MapY == 96 - 1)
            {
                shiftDown = false;
                ShiftDownToggle.Checked = false;
            }
            else
                shiftDown = ShiftDownToggle.Checked;

            coreOnly = CoreOnlyToggle.Checked;

            long address;

            if (layer1Btn.Checked)
            {
                address = GetAddress(selectedButton.MapX, selectedButton.MapY, shiftRight, shiftDown);
            }
            else if (layer2Btn.Checked)
            {
                address = GetAddress(selectedButton.MapX, selectedButton.MapY, shiftRight, shiftDown) + Utilities.mapSize;
            }
            else
                return;

            string itemID = Utilities.PrecedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.PrecedingZeros(HexTextbox.Text, 8);
            string flag1 = Utilities.PrecedingZeros(FlagTextbox.Text, 2);
            string flag0 = selectedItem.GetFlag0();

            if (isPillar || isHalfTileItem)
            {
                if (flag1.Equals("00") || flag1.Equals("01") || flag1.Equals("02") || flag1.Equals("03"))
                    coreOnly = true;
            }

            DisableBtn();

            Thread spawnThread = new(delegate () { DropItem(address, itemID, itemData, flag0, flag1, selectedButton); });
            spawnThread.Start();
        }

        private void DropItem(long address, string itemID, string itemData, string flag0, string flag1, FloorSlot btn)
        {
            ShowMapWait(2, "Spawning Item...");

            if (!debugging)
            {

                /*
                int c = 0;

                while (Utilities.IsAboutToSave(s, usb, 5, saveTime, ignore))
                {
                    if (c > 10)
                    {
                        if (IgnoreAutosave())
                        {
                            break;
                        }
                        else
                        {
                            UpdateUI(() =>
                            {
                                EnableBtn();
                            });

                            if (sound)
                                System.Media.SystemSounds.Asterisk.Play();

                            HideMapWait();
                            return;
                        }
                    }
                    c++;
                    Thread.Sleep(3000);
                }
                */

                if (coreOnly)
                {
                    Utilities.DropCore(s, usb, address, itemID, itemData, flag0, flag1);
                }
                else
                    Utilities.DropItem(s, usb, address, itemID, itemData, flag0, flag1);
            }

            UpdateUI(() =>
            {
                /*
                if (itemID == "FFFE")
                    SetBtn(btn, itemID, itemData, "0000FFFE", "00000000", "0000FFFE", "00000000", "0000FFFE", "00000000", "00", flag1);
                else
                    SetBtn(btn, itemID, itemData, "0000FFFD", "0100" + itemID, "0000FFFD", "0001" + itemID, "0000FFFD", "0101" + itemID, "00", flag1);
                */
                if (coreOnly)
                {
                    UpdataDataCoreOnly(btn.MapX, btn.MapY, itemID, itemData, flag0, flag1, shiftRight, shiftDown);
                    UpdateBtn(btn);
                }
                else
                {
                    UpdataData(btn.MapX, btn.MapY, itemID, itemData, flag0, flag1, shiftRight, shiftDown);
                    UpdateBtn(btn);
                    if (shiftRight || shiftDown)
                        _ = UpdateNearBtn(int.Parse(btn.Tag.ToString()));
                }
                ResetBtnColor();
                EnableBtn();
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();
        }
        #endregion

        #region Area Spawn/Area Copy
        private void SetCorner(FloorSlot button)
        {
            if (AreaCopied && CornerOne) // Just selected corner2
            {
                int differenceX = button.MapX - Corner1X;
                int differenceY = button.MapY - Corner1Y;

                Corner1X = button.MapX;
                Corner1Y = button.MapY;
                Corner2X += differenceX;
                Corner2Y += differenceY;

                Corner1XBox.Text = Corner1X.ToString();
                Corner1YBox.Text = Corner1Y.ToString();
                Corner2XBox.Text = Corner2X.ToString();
                Corner2YBox.Text = Corner2Y.ToString();
            }
            else if (AreaCopied && !CornerOne) // Just selected corner1
            {
                int differenceX = button.MapX - Corner2X;
                int differenceY = button.MapY - Corner2Y;

                Corner2X = button.MapX;
                Corner2Y = button.MapY;
                Corner1X += differenceX;
                Corner1Y += differenceY;

                Corner1XBox.Text = Corner1X.ToString();
                Corner1YBox.Text = Corner1Y.ToString();
                Corner2XBox.Text = Corner2X.ToString();
                Corner2YBox.Text = Corner2Y.ToString();
            }
            else if (CornerOne)
            {
                CornerOne = false;
                Corner1X = button.MapX;
                Corner1Y = button.MapY;
                Corner1XBox.Text = Corner1X.ToString();
                Corner1YBox.Text = Corner1Y.ToString();
            }
            else
            {
                CornerOne = true;
                Corner2X = button.MapX;
                Corner2Y = button.MapY;
                Corner2XBox.Text = Corner2X.ToString();
                Corner2YBox.Text = Corner2Y.ToString();
            }

            if (Corner1X >= 0 && Corner1Y >= 0 && Corner2X >= 0 && Corner2Y >= 0)
            {
                AreaSet = true;
                ClearCopiedAreaBtn.Visible = true;

                int TopLeftX;
                int TopLeftY;
                int BottomRightX;
                int BottomRightY;

                if (Corner1X <= Corner2X)
                {
                    if (Corner1Y <= Corner2Y) // Top Left
                    {
                        TopLeftX = Corner1X;
                        TopLeftY = Corner1Y;
                        BottomRightX = Corner2X;
                        BottomRightY = Corner2Y;
                    }
                    else // Bottom Left
                    {
                        TopLeftX = Corner1X;
                        TopLeftY = Corner2Y; //
                        BottomRightX = Corner2X;
                        BottomRightY = Corner1Y; //
                    }
                }
                else
                {
                    if (Corner1Y <= Corner2Y) // Top Right
                    {
                        TopLeftX = Corner2X; //
                        TopLeftY = Corner1Y;
                        BottomRightX = Corner1X; //
                        BottomRightY = Corner2Y;
                    }
                    else // Bottom Left
                    {
                        TopLeftX = Corner2X;
                        TopLeftY = Corner2Y;
                        BottomRightX = Corner1X;
                        BottomRightY = Corner1Y;
                    }
                }

                int numberOfColumn = BottomRightX - TopLeftX + 1;
                int numberOfRow = BottomRightY - TopLeftY + 1;

                miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.CombineMap(miniMap.DrawBackground(), miniMap.DrawItemMap()), MiniMap.DrawPreview(numberOfRow, numberOfColumn, TopLeftX, TopLeftY, true));
                return;
            }

            miniMapBox.BackgroundImage = MiniMap.CombineMap(miniMap.DrawBackground(), miniMap.DrawItemMap());
        }

        private void AreaSpawn()
        {
            int TopLeftX;
            int TopLeftY;
            int BottomRightX;
            int BottomRightY;

            if (Corner1X <= Corner2X)
            {
                if (Corner1Y <= Corner2Y) // Top Left
                {
                    TopLeftX = Corner1X;
                    TopLeftY = Corner1Y;
                    BottomRightX = Corner2X;
                    BottomRightY = Corner2Y;
                }
                else // Bottom Left
                {
                    TopLeftX = Corner1X;
                    TopLeftY = Corner2Y; //
                    BottomRightX = Corner2X;
                    BottomRightY = Corner1Y; //
                }
            }
            else
            {
                if (Corner1Y <= Corner2Y) // Top Right
                {
                    TopLeftX = Corner2X; //
                    TopLeftY = Corner1Y;
                    BottomRightX = Corner1X; //
                    BottomRightY = Corner2Y;
                }
                else // Bottom Left
                {
                    TopLeftX = Corner2X;
                    TopLeftY = Corner2Y;
                    BottomRightX = Corner1X;
                    BottomRightY = Corner1Y;
                }
            }

            long address;

            if (layer1Btn.Checked)
            {
                address = Utilities.mapZero;
            }
            else if (layer2Btn.Checked)
            {
                address = Utilities.mapZero + Utilities.mapSize;
            }
            else
                return;

            DisableBtn();

            btnToolTip.RemoveAll();

            byte[][] spawnArea = BuildSpawnArea(TopLeftX, TopLeftY, BottomRightX, BottomRightY);

            Thread SpawnThread = new(delegate () { AreaSpawnThread(address, spawnArea, TopLeftX, TopLeftY); });
            SpawnThread.Start();
        }

        private byte[][] BuildSpawnArea(int TopLeftX, int TopLeftY, int BottomRightX, int BottomRightY)
        {
            int numberOfColumn = BottomRightX - TopLeftX + 1;
            int numberOfRow = BottomRightY - TopLeftY + 1;
            int sizeOfRow = 16;

            byte[][] b = new byte[numberOfColumn * 2][];

            for (int i = 0; i < numberOfColumn * 2; i++)
            {
                b[i] = new byte[numberOfRow * sizeOfRow];
            }

            string itemID = Utilities.PrecedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.PrecedingZeros(HexTextbox.Text, 8);
            string flag0 = selectedItem.GetFlag0();
            string flag1 = Utilities.PrecedingZeros(FlagTextbox.Text, 2);

            byte[] ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
            byte[] ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

            for (int i = 0; i < numberOfColumn; i++)
            {
                for (int j = 0; j < numberOfRow; j++)
                {
                    Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                    Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                }
            }

            return b;
        }

        private void AreaSpawnThread(long address, byte[][] SpawnArea, int TopLeftX, int TopLeftY)
        {
            ShowMapWait(SpawnArea.Length, "Spawning Items...");

            if (!debugging)
            {
                try
                {
                    int time = SpawnArea.Length / 4;

                    //Debug.Print("Length :" + SpawnArea.Length + " Time : " + time);

                    /*
                    int c = 0;

                    while (Utilities.IsAboutToSave(s, usb, time + 10, saveTime, ignore))
                    {
                        if (c > 10)
                        {
                            if (IgnoreAutosave())
                            {
                                break;
                            }
                            else
                            {
                                UpdateUI(() =>
                                {
                                    EnableBtn();
                                });

                                if (sound)
                                    System.Media.SystemSounds.Asterisk.Play();

                                HideMapWait();
                                return;
                            }
                        }
                        c++;
                        Thread.Sleep(3000);
                    }
                    */

                    for (int i = 0; i < SpawnArea.Length / 2; i++)
                    {
                        UInt32 currentColumn = (UInt32)(address + (0xC00 * (TopLeftX + i + 16)) + (0x10 * (TopLeftY)));

                        Utilities.DropColumn(s, usb, currentColumn, currentColumn + 0x600, SpawnArea[i * 2], SpawnArea[i * 2 + 1], ref counter);
                    }

                }
                catch (Exception ex)
                {
                    MyLog.LogEvent("Map", "areaSpawn: " + ex.Message);
                    NextSaveTimer.Stop();
                    MyMessageBox.Show(ex.Message, "I'm sorry.");
                }

                if (SpawnArea.Length > 2)
                    Thread.Sleep(3000);
            }

            UpdateUI(() =>
            {
                UpdataData(TopLeftX, TopLeftY, SpawnArea, false, true);
                MoveAnchor(anchorX, anchorY);
                btnToolTip.RemoveAll();
                //resetBtnColor();
                EnableBtn();
            });


            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();

        }

        private void FloorRightClick_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (AreaSet && !floorRightClick.Items.Contains(CopyAreaMenu))
                floorRightClick.Items.Add(CopyAreaMenu);
            if (AreaCopied && !floorRightClick.Items.Contains(PasteAreaMenu))
                floorRightClick.Items.Add(PasteAreaMenu);
            if (AreaSet && !floorRightClick.Items.Contains(SaveAreaMenu))
                floorRightClick.Items.Add(SaveAreaMenu);

            if (layer1Btn.Checked)
            {
                if (IsActivate(selectedButton.MapX, selectedButton.MapY, ActivateTable1))
                {
                    floorRightClick.Items.Add(DeactivateItem);
                    if (floorRightClick.Items.Contains(ActivateItem))
                        floorRightClick.Items.Remove(ActivateItem);
                }
                else
                {
                    floorRightClick.Items.Add(ActivateItem);
                    if (floorRightClick.Items.Contains(DeactivateItem))
                        floorRightClick.Items.Remove(DeactivateItem);
                }
            }
            else
            {
                if (IsActivate(selectedButton.MapX, selectedButton.MapY, ActivateTable2))
                {
                    floorRightClick.Items.Add(DeactivateItem);
                    if (floorRightClick.Items.Contains(ActivateItem))
                        floorRightClick.Items.Remove(ActivateItem);
                }
                else
                {
                    floorRightClick.Items.Add(ActivateItem);
                    if (floorRightClick.Items.Contains(DeactivateItem))
                        floorRightClick.Items.Remove(DeactivateItem);
                }
            }


            floorRightClick.Items.Add(SetBaseItem);

            if (hasBaseItem && (selectedButton.MapX >= BaseMapX || selectedButton.MapY >= BaseMapY))
            {
                if (selectedButton.MapX < BaseMapX || selectedButton.MapY < BaseMapY)
                {
                    if (floorRightClick.Items.Contains(CreateExtension))
                        floorRightClick.Items.Remove(CreateExtension);
                }
                else if (selectedButton.MapX == BaseMapX && selectedButton.MapY == BaseMapY)
                {
                    if (floorRightClick.Items.Contains(CreateExtension))
                        floorRightClick.Items.Remove(CreateExtension);
                }
                else if (selectedButton.MapX - BaseMapX > 8 || selectedButton.MapY - BaseMapY > 8)
                {
                    if (floorRightClick.Items.Contains(CreateExtension))
                        floorRightClick.Items.Remove(CreateExtension);
                }
                else
                {
                    floorRightClick.Items.Add(CreateExtension);
                }
            }
            else
            {
                if (floorRightClick.Items.Contains(CreateExtension))
                    floorRightClick.Items.Remove(CreateExtension);
            }
        }

        private void CopyAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AreaCopied = true;
            ClearCopiedAreaBtn.Visible = true;

            int TopLeftX;
            int TopLeftY;
            int BottomRightX;
            int BottomRightY;

            if (Corner1X <= Corner2X)
            {
                if (Corner1Y <= Corner2Y) // Top Left
                {
                    TopLeftX = Corner1X;
                    TopLeftY = Corner1Y;
                    BottomRightX = Corner2X;
                    BottomRightY = Corner2Y;
                }
                else // Bottom Left
                {
                    TopLeftX = Corner1X;
                    TopLeftY = Corner2Y; //
                    BottomRightX = Corner2X;
                    BottomRightY = Corner1Y; //
                }
            }
            else
            {
                if (Corner1Y <= Corner2Y) // Top Right
                {
                    TopLeftX = Corner2X; //
                    TopLeftY = Corner1Y;
                    BottomRightX = Corner1X; //
                    BottomRightY = Corner2Y;
                }
                else // Bottom Left
                {
                    TopLeftX = Corner2X;
                    TopLeftY = Corner2Y;
                    BottomRightX = Corner1X;
                    BottomRightY = Corner1Y;
                }
            }

            int numberOfColumn = BottomRightX - TopLeftX + 1;
            int numberOfRow = BottomRightY - TopLeftY + 1;

            /*
            long address;

            if (layer1Btn.Checked)
            {
                address = Utilities.mapZero;
            }
            else if (layer2Btn.Checked)
            {
                address = Utilities.mapZero + Utilities.mapSize;
            }
            else
                return;
            */
            //disableBtn();

            Thread ReadThread = new(delegate () { ReadArea(TopLeftX, TopLeftY, numberOfColumn, numberOfRow); });
            ReadThread.Start();
        }

        private void ReadArea(int TopLeftX, int TopLeftY, int numberOfColumn, int numberOfRow)
        {
            int sizeOfRow = 16;

            SavedArea = new byte[numberOfColumn * 2][];

            for (int i = 0; i < numberOfColumn * 2; i++)
            {
                SavedArea[i] = new byte[numberOfRow * sizeOfRow];
            }

            for (int i = 0; i < numberOfColumn; i++)
            {
                for (int j = 0; j < numberOfRow; j++)
                {
                    if (layer1Btn.Checked)
                    {
                        Buffer.BlockCopy(Layer1, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)), SavedArea[i * 2], 0x10 * j, 0x10);
                        Buffer.BlockCopy(Layer1, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)) + 0x600, SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                    }
                    else
                    {
                        Buffer.BlockCopy(Layer2, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)), SavedArea[i * 2], 0x10 * j, 0x10);
                        Buffer.BlockCopy(Layer2, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)) + 0x600, SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                    }
                }
            }

            MoveAnchor(anchorX, anchorY);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void PasteAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Corner1X < 0 || Corner1Y < 0 || Corner2X < 0 || Corner2Y < 0 || Corner1X > 111 || Corner1Y > 95 || Corner2X > 111 || Corner2Y > 95)
            {
                MyMessageBox.Show("Selected Area Out of Bounds!", "Please use your brain, My Master.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int TopLeftX;
            int TopLeftY;
            int BottomRightX;
            int BottomRightY;

            if (Corner1X <= Corner2X)
            {
                if (Corner1Y <= Corner2Y) // Top Left
                {
                    TopLeftX = Corner1X;
                    TopLeftY = Corner1Y;
                    BottomRightX = Corner2X;
                    BottomRightY = Corner2Y;
                }
                else // Bottom Left
                {
                    TopLeftX = Corner1X;
                    TopLeftY = Corner2Y; //
                    BottomRightX = Corner2X;
                    BottomRightY = Corner1Y; //
                }
            }
            else
            {
                if (Corner1Y <= Corner2Y) // Top Right
                {
                    TopLeftX = Corner2X; //
                    TopLeftY = Corner1Y;
                    BottomRightX = Corner1X; //
                    BottomRightY = Corner2Y;
                }
                else // Bottom Left
                {
                    TopLeftX = Corner2X;
                    TopLeftY = Corner2Y;
                    BottomRightX = Corner1X;
                    BottomRightY = Corner1Y;
                }
            }

            int numberOfColumn = BottomRightX - TopLeftX + 1;
            int numberOfRow = BottomRightY - TopLeftY + 1;

            long address;

            if (layer1Btn.Checked)
            {
                address = Utilities.mapZero;
            }
            else if (layer2Btn.Checked)
            {
                address = Utilities.mapZero + Utilities.mapSize;
            }
            else
                return;

            DisableBtn();

            Thread pasteAreaThread = new(delegate () { PasteArea(TopLeftX, TopLeftY, numberOfColumn); });
            pasteAreaThread.Start();
        }

        private void PasteArea(int TopLeftX, int TopLeftY, int numberOfColumn)
        {
            ShowMapWait(numberOfColumn, "Kicking Babies...");

            try
            {
                int time = numberOfColumn;

                Debug.Print("Length :" + numberOfColumn + " Time : " + time);

                /*
                int c = 0;

                while (Utilities.IsAboutToSave(s, usb, time + 10, saveTime, ignore))
                {
                    if (c > 10)
                    {
                        if (IgnoreAutosave())
                        {
                            break;
                        }
                        else
                        {
                            UpdateUI(() =>
                            {
                                EnableBtn();
                            });

                            if (sound)
                                System.Media.SystemSounds.Asterisk.Play();

                            HideMapWait();
                            return;
                        }
                    }
                    c++;
                    Thread.Sleep(3000);
                }
                */

                for (int i = 0; i < numberOfColumn; i++)
                {
                    UInt32 CurAddress = GetAddress(TopLeftX + i, TopLeftY);

                    if (layer2Btn.Checked)
                    {
                        CurAddress += Utilities.mapSize;
                    }

                    Utilities.DropColumn(s, usb, CurAddress, CurAddress + 0x600, SavedArea[i * 2], SavedArea[i * 2 + 1], ref counter);
                }

                UpdateUI(() =>
                {
                    UpdataData(TopLeftX, TopLeftY, SavedArea, false, true);
                });

            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "PasteArea: " + ex.Message);
                MyMessageBox.Show(ex.Message, "Dafuq?");
            }

            Thread.Sleep(5000);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();

            UpdateUI(() =>
            {
                MoveAnchor(anchorX, anchorY);
                EnableBtn();
            });
        }

        private void ClearCopiedAreaBtn_Click(object sender, EventArgs e)
        {
            AreaCopied = false;
            ClearCopiedAreaBtn.Visible = false;
            CornerOne = true;
            Corner1X = -1;
            Corner1Y = -1;
            Corner2X = -1;
            Corner2Y = -1;

            Corner1XBox.Text = "";
            Corner1YBox.Text = "";
            Corner2XBox.Text = "";
            Corner2YBox.Text = "";

            CornerOne = true;
            AreaSet = false;
            if (floorRightClick.Items.Contains(CopyAreaMenu))
                floorRightClick.Items.Remove(CopyAreaMenu);
            if (floorRightClick.Items.Contains(PasteAreaMenu))
                floorRightClick.Items.Remove(PasteAreaMenu);
            if (floorRightClick.Items.Contains(SaveAreaMenu))
                floorRightClick.Items.Remove(SaveAreaMenu);
            MoveAnchor(anchorX, anchorY);
            miniMapBox.BackgroundImage = MiniMap.CombineMap(miniMap.DrawBackground(), miniMap.DrawItemMap());
        }

        private void SaveAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int TopLeftX;
            int TopLeftY;
            int BottomRightX;
            int BottomRightY;

            if (Corner1X <= Corner2X)
            {
                if (Corner1Y <= Corner2Y) // Top Left
                {
                    TopLeftX = Corner1X;
                    TopLeftY = Corner1Y;
                    BottomRightX = Corner2X;
                    BottomRightY = Corner2Y;
                }
                else // Bottom Left
                {
                    TopLeftX = Corner1X;
                    TopLeftY = Corner2Y; //
                    BottomRightX = Corner2X;
                    BottomRightY = Corner1Y; //
                }
            }
            else
            {
                if (Corner1Y <= Corner2Y) // Top Right
                {
                    TopLeftX = Corner2X; //
                    TopLeftY = Corner1Y;
                    BottomRightX = Corner1X; //
                    BottomRightY = Corner2Y;
                }
                else // Bottom Left
                {
                    TopLeftX = Corner2X;
                    TopLeftY = Corner2Y;
                    BottomRightX = Corner1X;
                    BottomRightY = Corner1Y;
                }
            }

            int numberOfColumn = BottomRightX - TopLeftX + 1;
            int numberOfRow = BottomRightY - TopLeftY + 1;

            int sizeOfRow = 0x8;
            byte[] save = [];
            byte[] tempItem = new byte[sizeOfRow];

            for (int i = 0; i < numberOfColumn; i++)
            {
                for (int j = 0; j < numberOfRow; j++)
                {
                    if (layer1Btn.Checked)
                        Buffer.BlockCopy(Layer1, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)), tempItem, 0, sizeOfRow);
                    else
                        Buffer.BlockCopy(Layer2, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)), tempItem, 0, sizeOfRow);

                    save = Utilities.Add(save, tempItem);
                }
            }

            MoveAnchor(anchorX, anchorY);

            SaveFileDialog file = new()
            {
                Filter = "New Horizons Bulk Spawn (*.nhbs)|*.nhbs|New Horizons Inventory(*.nhi) | *.nhi",
                FileName = "(" + numberOfRow + ")" + "filename",
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
            for (int i = 0; i < temp.Length - 1; i++)
                path = path + temp[i] + "\\";

            config.AppSettings.Settings["LastSave"].Value = path;
            config.Save(ConfigurationSaveMode.Minimal);

            File.WriteAllBytes(file.FileName, save);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void ActivateItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (layer1Btn.Checked)
                SetActivate(selectedButton.MapX, selectedButton.MapY, ref ActivateLayer1, ref ActivateTable1);
            else
                SetActivate(selectedButton.MapX, selectedButton.MapY, ref ActivateLayer2, ref ActivateTable2);
        }

        private void DeactivateItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (layer1Btn.Checked)
                SetDeactivate(selectedButton.MapX, selectedButton.MapY, ref ActivateLayer1, ref ActivateTable1);
            else
                SetDeactivate(selectedButton.MapX, selectedButton.MapY, ref ActivateLayer2, ref ActivateTable2);
        }

        private void CreateExtensionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!hasBaseItem)
                return;
            int DiffX = selectedButton.MapX - BaseMapX;
            int DiffY = selectedButton.MapY - BaseMapY;

            int sizeOfRow = 16;

            string flag0 = "00";
            string flag1 = "00";

            long address;

            if (layer1Btn.Checked)
            {
                address = Utilities.mapZero;
            }
            else if (layer2Btn.Checked)
            {
                address = Utilities.mapZero + Utilities.mapSize;
            }
            else
                return;

            byte[][] b = new byte[2][];

            for (int i = 0; i < 2; i++)
            {
                b[i] = new byte[sizeOfRow];
            }

            b[0] = Utilities.StringToByte(Utilities.BuildLeftExtension(Utilities.PrecedingZeros(BaseID.ToString("X"), 4), flag0, flag1, DiffX, DiffY));
            b[1] = Utilities.StringToByte(Utilities.BuildRightExtension(Utilities.PrecedingZeros(BaseID.ToString("X"), 4), flag0, flag1, DiffX, DiffY));

            Thread SpawnThread = new(delegate () { AreaSpawnThread(address, b, selectedButton.MapX, selectedButton.MapY); });
            SpawnThread.Start();
        }

        private void SetBaseItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hasBaseItem = true;
            BaseID = selectedButton.ItemID;
            BaseMapX = selectedButton.MapX;
            BaseMapY = selectedButton.MapY;
        }

        #endregion

        #region Delete Item
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = (sender as ToolStripItem);
            if (item != null)
            {
                if (item.Owner is ContextMenuStrip owner)
                {
                    var btn = (FloorSlot)owner.SourceControl;

                    long address;

                    if (layer1Btn.Checked)
                    {
                        address = GetAddress(btn.MapX, btn.MapY);
                    }
                    else if (layer2Btn.Checked)
                    {
                        address = (GetAddress(btn.MapX, btn.MapY) + Utilities.mapSize);
                    }
                    else
                        return;

                    DisableBtn();

                    Thread deleteThread = new(delegate () { DeleteItem(address, btn); });
                    deleteThread.Start();
                }
            }
        }

        private void DeleteItem(long address, FloorSlot btn)
        {
            ShowMapWait(2, "Deleting Item...");

            /*
            int c = 0;

            while (Utilities.IsAboutToSave(s, usb, 5, saveTime, ignore))
            {
                if (c > 10)
                {
                    if (IgnoreAutosave())
                    {
                        break;
                    }
                    else
                    {
                        UpdateUI(() =>
                        {
                            EnableBtn();
                        });

                        if (sound)
                            System.Media.SystemSounds.Asterisk.Play();

                        HideMapWait();
                        return;
                    }
                }
                c++;
                Thread.Sleep(3000);
            }
            */

            Utilities.DeleteFloorItem(s, usb, address);

            UpdateUI(() =>
            {
                UpdataData(selectedButton.MapX, selectedButton.MapY);
                btn.Reset();
                btnToolTip.RemoveAll();
                ResetBtnColor();
                EnableBtn();
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();
        }
        #endregion

        #region Copy Item
        private void CopyItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = (sender as ToolStripItem);
            if (item != null)
            {
                if (item.Owner is ContextMenuStrip owner)
                {
                    var btn = (FloorSlot)owner.SourceControl;
                    string id = Utilities.PrecedingZeros(btn.ItemID.ToString("X"), 4);
                    string name = btn.Name;
                    string hexValue = Utilities.PrecedingZeros(btn.ItemData.ToString("X"), 8);
                    string flag0 = btn.Flag0;
                    string flag1 = btn.Flag1;

                    IdTextbox.Text = id;
                    HexTextbox.Text = hexValue;
                    FlagTextbox.Text = flag1;

                    UInt16 IntId = Convert.ToUInt16("0x" + id, 16);
                    string front = Utilities.PrecedingZeros(hexValue, 8).Substring(0, 4);
                    string back = Utilities.PrecedingZeros(hexValue, 8).Substring(4, 4);


                    if (id == "16A2")
                        selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.Turn2bytes(hexValue), recipeSource), true, "", flag0, flag1);
                    else if (id == "114A" || id == "EC9C") // Money Tree
                        selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source), true, GetImagePathFromID(back, source), flag0, flag1);
                    else if (ItemAttr.HasFenceWithVariation(IntId))  // Fence Variation
                        selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + front, 16)), true, "", flag0, flag1);
                    else if (id == "315A" || id == "1618" || id == "342F")
                        selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID((Utilities.Turn2bytes(hexValue)), source, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16)), flag0, flag1);
                    else
                        selectedItem.Setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + hexValue, 16)), true, "", flag0, flag1);

                    if (id == "315A" || id == "1618" || id == "342F")
                        selectedSize = GetSize(back);
                    else
                        selectedSize = GetSize(id);
                    SizeBox.Text = selectedSize.Replace("_5", ".5 ").Replace("_0", ".0 ").Replace("_Wall", "Wall").Replace("_Rug", "Rug").Replace("_Pillar", "Pillar").Replace("_Ceiling", "Ceiling").Replace("x", "x ");

                    if (sound)
                        System.Media.SystemSounds.Asterisk.Play();
                }
            }
        }
        #endregion

        #region Input Check
        private void Hex_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || c == (char)Keys.Back))
            {
                e.Handled = true;
            }
            if (c >= 'a' && c <= 'f') e.KeyChar = char.ToUpper(c);

            UpdateVariation();
        }

        private void Hex_KeyUp(object sender, KeyEventArgs e)
        {
            if (IdTextbox.Text.Equals(string.Empty) || HexTextbox.Text.Equals(string.Empty))
                return;

            string itemID = Utilities.PrecedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.PrecedingZeros(HexTextbox.Text, 8);
            string flag1 = Utilities.PrecedingZeros(FlagTextbox.Text, 2);

            UInt16 IntId = Convert.ToUInt16("0x" + itemID, 16);
            string front = Utilities.PrecedingZeros(itemData, 8).Substring(0, 4);
            string back = Utilities.PrecedingZeros(itemData, 8).Substring(4, 4);

            if (itemID.Equals("315A") || itemID.Equals("1618") || itemID.Equals("342F"))
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source), true, GetImagePathFromID(Utilities.Turn2bytes(itemData), source, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16)), "00", flag1);
            }
            else if (itemID.Equals("114A") || itemID.Equals("EC9C")) // Money Tree
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source), true, GetImagePathFromID(back, source), "00", flag1);
            }
            else if (itemID.Equals("16A2"))
            {
                selectedItem.Setup(GetNameFromID(itemID, recipeSource), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(Utilities.Turn2bytes(itemData), recipeSource), true, "", "00", flag1);
            }
            else if (ItemAttr.HasFenceWithVariation(IntId))  // Fence Variation
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + front, 16)), true, "", "00", flag1);
            }
            else
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + itemData, 16)), true, "", "00", flag1);
            }
        }

        private void FlagTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || c == (char)Keys.Back))
            {
                e.Handled = true;
            }
            if (c >= 'a' && c <= 'f') e.KeyChar = char.ToUpper(c);

            UpdateVariation();
        }

        private void HexTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || c == (char)Keys.Back))
            {
                e.Handled = true;
            }
            if (c >= 'a' && c <= 'f') e.KeyChar = char.ToUpper(c);

            UpdateVariation();
        }

        private void HexTextbox_ValueChanged(object sender, EventArgs e)
        {
            if (IdTextbox.Text.Equals(string.Empty) || HexTextbox.Text.Equals(string.Empty))
                return;

            HexUpDown me = (HexUpDown)sender;
            var NewValue = me.Value;
            long data = (long)NewValue;
            string hexValue = data.ToString("X");

            string itemID = Utilities.PrecedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.PrecedingZeros(hexValue, 8);
            string flag1 = Utilities.PrecedingZeros(FlagTextbox.Text, 2);

            UInt16 IntId = Convert.ToUInt16("0x" + itemID, 16);
            string front = Utilities.PrecedingZeros(itemData, 8).Substring(0, 4);
            string back = Utilities.PrecedingZeros(itemData, 8).Substring(4, 4);

            if (itemID.Equals("315A") || itemID.Equals("1618") || itemID.Equals("342F"))
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source), true, GetImagePathFromID(Utilities.Turn2bytes(itemData), source, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16)), "00", flag1);
            }
            else if (itemID.Equals("114A") || itemID.Equals("EC9C")) // Money Tree
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source), true, GetImagePathFromID(back, source), "00", flag1);
            }
            else if (itemID.Equals("16A2"))
            {
                selectedItem.Setup(GetNameFromID(itemID, recipeSource), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(Utilities.Turn2bytes(itemData), recipeSource), true, "", "00", flag1);
            }
            else if (ItemAttr.HasFenceWithVariation(IntId))  // Fence Variation
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + front, 16)), true, "", "00", flag1);
            }
            else
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + itemData, 16)), true, "", "00", flag1);
            }


            if (selection != null)
            {
                //selection.Dispose();
                //string id = Utilities.PrecedingZeros(selectedItem.FillItemID(), 4);
                string value = Utilities.PrecedingZeros(selectedItem.FillItemData(), 8);

                if (ItemAttr.HasFenceWithVariation(IntId))  // Fence Variation
                {
                    selection.ReceiveID(Utilities.PrecedingZeros(selectedItem.FillItemID(), 4), languageSetting, value);
                }
            }
        }
        #endregion

        #region Refresh
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            if (anchorX < 0 || anchorY < 0)
                return;

            DisableBtn();

            Thread LoadThread = new(delegate () { RefreshMap(Utilities.mapZero, Utilities.mapZero + Utilities.mapSize); });
            LoadThread.Start();
        }

        private void RefreshMap(UInt32 layer1Address, UInt32 layer2Address)
        {
            ShowMapWait((42 + 2) * 2, "Fetching Map...");

            try
            {
                Layer1 = Utilities.GetMapLayer(s, usb, layer1Address, ref counter);
                Layer2 = Utilities.GetMapLayer(s, usb, layer2Address, ref counter);

                if (layer1Btn.Checked)
                    miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer1);
                else
                    miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer2);

                ActivateLayer1 = Utilities.GetActivate(s, usb, Utilities.mapActivate, ref counter);
                ActivateLayer2 = Utilities.GetActivate(s, usb, Utilities.mapActivate + Utilities.mapActivateSize, ref counter);

                if (Layer1 != null && Layer2 != null && Acre != null)
                {
                    if (miniMap == null)
                        miniMap = new MiniMap(Layer1, Acre, Building, Terrain, MapCustomDesgin, 2);
                }
                else
                    throw new NullReferenceException("Layer1/Layer2/Acre");

                BuildActivateTable(ActivateLayer1, ref ActivateTable1);
                BuildActivateTable(ActivateLayer2, ref ActivateTable2);

                UpdateUI(() =>
                {
                    DisplayAnchorAsync();
                    EnableBtn();
                });

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "RefreshMap: " + ex.Message);
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message, "For the brave souls who get this far: You are the chosen ones.");
            }

            HideMapWait();
        }

        #endregion

        #region Clear
        private void ClearGridBtn_Click(object sender, EventArgs e)
        {
            if (anchorX < 0 || anchorY < 0)
                return;

            DisableBtn();

            Thread clearGridThread = new(ClearGrid);
            clearGridThread.Start();
        }

        private void ClearGrid()
        {
            ShowMapWait(14, "Clearing Grid...");

            try
            {
                byte[][] b = new byte[14][];

                FillFloor(ref b, null);

                if (!debugging)
                {
                    UInt32 address1;
                    UInt32 address2;
                    UInt32 address3;
                    UInt32 address4;
                    UInt32 address5;
                    UInt32 address6;
                    UInt32 address7;

                    if (layer1Btn.Checked)
                    {
                        address1 = GetAddress(anchorX - 3, anchorY - 3);
                        address2 = GetAddress(anchorX - 2, anchorY - 3);
                        address3 = GetAddress(anchorX - 1, anchorY - 3);
                        address4 = GetAddress(anchorX, anchorY - 3);
                        address5 = GetAddress(anchorX + 1, anchorY - 3);
                        address6 = GetAddress(anchorX + 2, anchorY - 3);
                        address7 = GetAddress(anchorX + 3, anchorY - 3);
                    }
                    else if (layer2Btn.Checked)
                    {
                        address1 = GetAddress(anchorX - 3, anchorY - 3) + Utilities.mapSize;
                        address2 = GetAddress(anchorX - 2, anchorY - 3) + Utilities.mapSize;
                        address3 = GetAddress(anchorX - 1, anchorY - 3) + Utilities.mapSize;
                        address4 = GetAddress(anchorX, anchorY - 3)     + Utilities.mapSize;
                        address5 = GetAddress(anchorX + 1, anchorY - 3) + Utilities.mapSize;
                        address6 = GetAddress(anchorX + 2, anchorY - 3) + Utilities.mapSize;
                        address7 = GetAddress(anchorX + 3, anchorY - 3) + Utilities.mapSize;
                    }
                    else
                        return;

                    /*
                    int c = 0;

                    while (Utilities.IsAboutToSave(s, usb, 10, saveTime, ignore))
                    {
                        if (c > 10)
                        {
                            if (IgnoreAutosave())
                            {
                                break;
                            }
                            else
                            {
                                UpdateUI(() =>
                                {
                                    EnableBtn();
                                });

                                if (sound)
                                    System.Media.SystemSounds.Asterisk.Play();

                                HideMapWait();
                                return;
                            }
                        }
                        c++;
                        Thread.Sleep(3000);
                    }
                    */

                    Utilities.DropColumn(s, usb, address1, address1 + 0x600, b[0], b[1], ref counter);
                    Utilities.DropColumn(s, usb, address2, address2 + 0x600, b[2], b[3], ref counter);
                    Utilities.DropColumn(s, usb, address3, address3 + 0x600, b[4], b[5], ref counter);
                    Utilities.DropColumn(s, usb, address4, address4 + 0x600, b[6], b[7], ref counter);
                    Utilities.DropColumn(s, usb, address5, address5 + 0x600, b[8], b[9], ref counter);
                    Utilities.DropColumn(s, usb, address6, address6 + 0x600, b[10], b[11], ref counter);
                    Utilities.DropColumn(s, usb, address7, address7 + 0x600, b[12], b[13], ref counter);
                }


                UpdateUI(() =>
                {
                    UpdataData(anchorX, anchorY, b);
                    UpdateAllBtn();
                    ResetBtnColor();
                    EnableBtn();
                });

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "ClearingGrid: " + ex.Message);
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message, "You are not meant to understand this.");
            }

            HideMapWait();
        }

        #endregion

        #region Fill Remain
        private void FillRemainBtn_Click(object sender, EventArgs e)
        {
            if (anchorX < 0 || anchorY < 0)
                return;

            if (IdTextbox.Text == "" || HexTextbox.Text == "" || FlagTextbox.Text == "")
            {
                return;
            }

            string itemID = Utilities.PrecedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.PrecedingZeros(HexTextbox.Text, 8);
            string flag1 = Utilities.PrecedingZeros(FlagTextbox.Text, 2);

            DisableBtn();

            Thread fillRemainThread = new(delegate () { FillRemain(itemID, itemData, flag1); });
            fillRemainThread.Start();
        }

        private void FillRemain(string itemID, string itemData, string flag1)
        {
            ShowMapWait(14, "Filling Empty Tiles...");

            try
            {
                byte[][] b = new byte[14][];

                if (debugging)
                {
                    byte[] CurrentLayer;
                    if (layer1Btn.Checked)
                        CurrentLayer = Layer1;
                    else
                        CurrentLayer = Layer2;

                    byte[] curFloor = new byte[1568];

                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x0, curFloor, 0x0, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x600, curFloor, 0x70, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0xC00, curFloor, 0xE0, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x1200, curFloor, 0x150, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x1800, curFloor, 0x1C0, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x1E00, curFloor, 0x230, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x2400, curFloor, 0x2A0, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x2A00, curFloor, 0x310, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x3000, curFloor, 0x380, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x3600, curFloor, 0x3F0, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x3C00, curFloor, 0x460, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x4200, curFloor, 0x4D0, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x4800, curFloor, 0x540, 0x70);
                    Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)) + 0x4E00, curFloor, 0x5B0, 0x70);

                    bool[,] isEmpty = new bool[7, 7];

                    NumOfEmpty(curFloor, ref isEmpty);

                    FillFloor(ref b, curFloor, isEmpty, itemID, itemData, flag1);
                }
                else
                {
                    UInt32 address;

                    if (layer1Btn.Checked)
                        address = GetAddress(anchorX - 3, anchorY - 3);
                    else
                        address = GetAddress(anchorX - 3, anchorY - 3) + Utilities.mapSize;

                    byte[] readFloor = Utilities.Read7x7Floor(s, usb, address);
                    byte[] curFloor = new byte[1568];

                    Buffer.BlockCopy(readFloor, 0x0, curFloor, 0x0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x600, curFloor, 0x70, 0x70);
                    Buffer.BlockCopy(readFloor, 0xC00, curFloor, 0xE0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x1200, curFloor, 0x150, 0x70);
                    Buffer.BlockCopy(readFloor, 0x1800, curFloor, 0x1C0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x1E00, curFloor, 0x230, 0x70);
                    Buffer.BlockCopy(readFloor, 0x2400, curFloor, 0x2A0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x2A00, curFloor, 0x310, 0x70);
                    Buffer.BlockCopy(readFloor, 0x3000, curFloor, 0x380, 0x70);
                    Buffer.BlockCopy(readFloor, 0x3600, curFloor, 0x3F0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x3C00, curFloor, 0x460, 0x70);
                    Buffer.BlockCopy(readFloor, 0x4200, curFloor, 0x4D0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x4800, curFloor, 0x540, 0x70);
                    Buffer.BlockCopy(readFloor, 0x4E00, curFloor, 0x5B0, 0x70);

                    bool[,] isEmpty = new bool[7, 7];

                    NumOfEmpty(curFloor, ref isEmpty);

                    FillFloor(ref b, curFloor, isEmpty, itemID, itemData, flag1);

                    UInt32 address1;
                    UInt32 address2;
                    UInt32 address3;
                    UInt32 address4;
                    UInt32 address5;
                    UInt32 address6;
                    UInt32 address7;

                    if (layer1Btn.Checked)
                    {
                        address1 = GetAddress(anchorX - 3, anchorY - 3);
                        address2 = GetAddress(anchorX - 2, anchorY - 3);
                        address3 = GetAddress(anchorX - 1, anchorY - 3);
                        address4 = GetAddress(anchorX, anchorY - 3);
                        address5 = GetAddress(anchorX + 1, anchorY - 3);
                        address6 = GetAddress(anchorX + 2, anchorY - 3);
                        address7 = GetAddress(anchorX + 3, anchorY - 3);
                    }
                    else if (layer2Btn.Checked)
                    {
                        address1 = GetAddress(anchorX - 3, anchorY - 3) + Utilities.mapSize;
                        address2 = GetAddress(anchorX - 2, anchorY - 3) + Utilities.mapSize;
                        address3 = GetAddress(anchorX - 1, anchorY - 3) + Utilities.mapSize;
                        address4 = GetAddress(anchorX, anchorY - 3) + Utilities.mapSize;
                        address5 = GetAddress(anchorX + 1, anchorY - 3) + Utilities.mapSize;
                        address6 = GetAddress(anchorX + 2, anchorY - 3) + Utilities.mapSize;
                        address7 = GetAddress(anchorX + 3, anchorY - 3) + Utilities.mapSize;
                    }
                    else
                        return;

                    /*
                    int c = 0;

                    while (Utilities.IsAboutToSave(s, usb, 10, saveTime, ignore))
                    {
                        if (c > 10)
                        {
                            if (IgnoreAutosave())
                            {
                                break;
                            }
                            else
                            {
                                UpdateUI(() =>
                                {
                                    EnableBtn();
                                });

                                if (sound)
                                    System.Media.SystemSounds.Asterisk.Play();

                                HideMapWait();
                                return;
                            }
                        }
                        c++;
                        Thread.Sleep(3000);
                    }
                    */

                    Utilities.DropColumn(s, usb, address1, address1 + 0x600, b[0], b[1], ref counter);
                    Utilities.DropColumn(s, usb, address2, address2 + 0x600, b[2], b[3], ref counter);
                    Utilities.DropColumn(s, usb, address3, address3 + 0x600, b[4], b[5], ref counter);
                    Utilities.DropColumn(s, usb, address4, address4 + 0x600, b[6], b[7], ref counter);
                    Utilities.DropColumn(s, usb, address5, address5 + 0x600, b[8], b[9], ref counter);
                    Utilities.DropColumn(s, usb, address6, address6 + 0x600, b[10], b[11], ref counter);
                    Utilities.DropColumn(s, usb, address7, address7 + 0x600, b[12], b[13], ref counter);
                }

                UpdateUI(() =>
                {
                    UpdataData(anchorX, anchorY, b);
                    UpdateAllBtn();
                    ResetBtnColor();
                    EnableBtn();
                });

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "FillRemain: " + ex.Message);
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message, " The valiant knights of programming who toil away, without rest,");
            }

            HideMapWait();
        }

        #endregion

        #region Save
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (anchorX < 0 || anchorY < 0)
                {
                    return;
                }

                SaveFileDialog file = new()
                {
                    Filter = "New Horizons Grid (*.nhg)|*.nhg",
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
                for (int i = 0; i < temp.Length - 1; i++)
                    path = path + temp[i] + "\\";

                config.AppSettings.Settings["LastSave"].Value = path;
                config.Save(ConfigurationSaveMode.Minimal);

                UInt32 address;

                if (layer1Btn.Checked)
                    address = GetAddress(anchorX - 3, anchorY - 3);
                else
                    address = GetAddress(anchorX - 3, anchorY - 3) + Utilities.mapSize;

                byte[] b = Utilities.Read7x7Floor(s, usb, address);
                byte[] save = new byte[1568];

                Buffer.BlockCopy(b, 0x0, save, 0x0, 0x70);
                Buffer.BlockCopy(b, 0x600, save, 0x70, 0x70);
                Buffer.BlockCopy(b, 0xC00, save, 0xE0, 0x70);
                Buffer.BlockCopy(b, 0x1200, save, 0x150, 0x70);
                Buffer.BlockCopy(b, 0x1800, save, 0x1C0, 0x70);
                Buffer.BlockCopy(b, 0x1E00, save, 0x230, 0x70);
                Buffer.BlockCopy(b, 0x2400, save, 0x2A0, 0x70);
                Buffer.BlockCopy(b, 0x2A00, save, 0x310, 0x70);
                Buffer.BlockCopy(b, 0x3000, save, 0x380, 0x70);
                Buffer.BlockCopy(b, 0x3600, save, 0x3F0, 0x70);
                Buffer.BlockCopy(b, 0x3C00, save, 0x460, 0x70);
                Buffer.BlockCopy(b, 0x4200, save, 0x4D0, 0x70);
                Buffer.BlockCopy(b, 0x4800, save, 0x540, 0x70);
                Buffer.BlockCopy(b, 0x4E00, save, 0x5B0, 0x70);

                File.WriteAllBytes(file.FileName, save);
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "Save: " + ex.Message);
            }
        }
        #endregion

        #region Load
        private void LoadNHGNHIBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (anchorX < 0 || anchorY < 0)
                {
                    return;
                }
                OpenFileDialog file = new()
                {
                    Filter = "New Horizons Grid/New Horizons Inventory (*.nhg;*.nhi)|*.nhg;*.nhi|All files (*.*)|*.*",
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
                bool nhi;

                if (file.FileName.Contains(".nhi") || file.FileName.Contains(".nhbs"))
                    nhi = true;
                else
                    nhi = false;

                if (nhi && data.Length > 320)
                {
                    MyMessageBox.Show("Your file seems to contain more than 40 items.\nPlease use the [Bulk Spawn] option under [Remove Items...] to spawn your items.", "It's too big to fit in here!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                DisableBtn();

                btnToolTip.RemoveAll();
                Thread LoadThread = new(delegate () { LoadFloor(data, nhi); });
                LoadThread.Start();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "Load: " + ex.Message);
            }
        }

        private async void LoadFloor(byte[] data, bool nhi)
        {
            ShowMapWait(14, "Loading...");

            try
            {
                byte[][] b = new byte[14][];

                if (nhi)
                {
                    byte[][] item = ProcessNHI(data);
                    byte[] curFloor;

                    if (debugging)
                    {
                        byte[] CurrentLayer;
                        if (layer1Btn.Checked)
                            CurrentLayer = Layer1;
                        else
                            CurrentLayer = Layer2;

                        curFloor = new byte[1568];

                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x0, curFloor, 0x0, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x600, curFloor, 0x70, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0xC00, curFloor, 0xE0, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x1200, curFloor, 0x150, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x1800, curFloor, 0x1C0, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x1E00, curFloor, 0x230, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x2400, curFloor, 0x2A0, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x2A00, curFloor, 0x310, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x3000, curFloor, 0x380, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x3600, curFloor, 0x3F0, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x3C00, curFloor, 0x460, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x4200, curFloor, 0x4D0, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x4800, curFloor, 0x540, 0x70);
                        Buffer.BlockCopy(CurrentLayer, (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)) + 0x4E00, curFloor, 0x5B0, 0x70);
                    }
                    else
                    {
                        UInt32 address;

                        if (layer1Btn.Checked)
                            address = GetAddress(anchorX - 3, anchorY - 3);
                        else
                            address = GetAddress(anchorX - 3, anchorY - 3) + Utilities.mapSize;

                        byte[] readFloor = Utilities.Read7x7Floor(s, usb, address);
                        curFloor = new byte[1568];

                        Buffer.BlockCopy(readFloor, 0x0, curFloor, 0x0, 0x70);
                        Buffer.BlockCopy(readFloor, 0x600, curFloor, 0x70, 0x70);
                        Buffer.BlockCopy(readFloor, 0xC00, curFloor, 0xE0, 0x70);
                        Buffer.BlockCopy(readFloor, 0x1200, curFloor, 0x150, 0x70);
                        Buffer.BlockCopy(readFloor, 0x1800, curFloor, 0x1C0, 0x70);
                        Buffer.BlockCopy(readFloor, 0x1E00, curFloor, 0x230, 0x70);
                        Buffer.BlockCopy(readFloor, 0x2400, curFloor, 0x2A0, 0x70);
                        Buffer.BlockCopy(readFloor, 0x2A00, curFloor, 0x310, 0x70);
                        Buffer.BlockCopy(readFloor, 0x3000, curFloor, 0x380, 0x70);
                        Buffer.BlockCopy(readFloor, 0x3600, curFloor, 0x3F0, 0x70);
                        Buffer.BlockCopy(readFloor, 0x3C00, curFloor, 0x460, 0x70);
                        Buffer.BlockCopy(readFloor, 0x4200, curFloor, 0x4D0, 0x70);
                        Buffer.BlockCopy(readFloor, 0x4800, curFloor, 0x540, 0x70);
                        Buffer.BlockCopy(readFloor, 0x4E00, curFloor, 0x5B0, 0x70);
                    }

                    bool[,] isEmpty = new bool[7, 7];

                    int emptyspace = NumOfEmpty(curFloor, ref isEmpty);

                    if (emptyspace < item.Length)
                    {
                        DialogResult dialogResult = MyMessageBox.Show("Empty tiles around anchor : " + emptyspace + "\n" +
                                                                    "Number of items to Spawn : " + item.Length + "\n" +
                                                                    "\n" +
                                                                    "Press  [Yes]  to clear the floor and spawn the items " + "\n" +
                                                                    "or  [No]  to cancel the spawn." + "\n" + "\n" +
                                                                    "[Warning] You will lose your items on the ground!"
                                                                    , "Not enough empty tiles!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                            FillFloor(ref b, item);
                        }
                        else
                        {
                            UpdateUI(() =>
                            {
                                EnableBtn();
                            });
                            if (sound)
                                System.Media.SystemSounds.Asterisk.Play();

                            HideMapWait();

                            return;
                        }
                    }
                    else
                    {
                        FillFloor(ref b, curFloor, isEmpty, item);
                    }
                }
                else
                {
                    for (int i = 0; i < 14; i++)
                    {
                        b[i] = new byte[112];
                        for (int j = 0; j < 112; j++)
                        {
                            b[i][j] = data[j + 112 * i];
                        }
                    }
                }


                if (!debugging)
                {
                    UInt32 address1;
                    UInt32 address2;
                    UInt32 address3;
                    UInt32 address4;
                    UInt32 address5;
                    UInt32 address6;
                    UInt32 address7;

                    if (layer1Btn.Checked)
                    {
                        address1 = GetAddress(anchorX - 3, anchorY - 3);
                        address2 = GetAddress(anchorX - 2, anchorY - 3);
                        address3 = GetAddress(anchorX - 1, anchorY - 3);
                        address4 = GetAddress(anchorX, anchorY - 3);
                        address5 = GetAddress(anchorX + 1, anchorY - 3);
                        address6 = GetAddress(anchorX + 2, anchorY - 3);
                        address7 = GetAddress(anchorX + 3, anchorY - 3);
                    }
                    else if (layer2Btn.Checked)
                    {
                        address1 = GetAddress(anchorX - 3, anchorY - 3) + Utilities.mapSize;
                        address2 = GetAddress(anchorX - 2, anchorY - 3) + Utilities.mapSize;
                        address3 = GetAddress(anchorX - 1, anchorY - 3) + Utilities.mapSize;
                        address4 = GetAddress(anchorX, anchorY - 3)     + Utilities.mapSize;
                        address5 = GetAddress(anchorX + 1, anchorY - 3) + Utilities.mapSize;
                        address6 = GetAddress(anchorX + 2, anchorY - 3) + Utilities.mapSize;
                        address7 = GetAddress(anchorX + 3, anchorY - 3) + Utilities.mapSize;
                    }
                    else
                        return;

                    List<Task> tasks =
                    [
                        Task.Run(() => Utilities.DropColumn(s, usb, address1, address1 + 0x600, b[0], b[1])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address2, address2 + 0x600, b[2], b[3])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address3, address3 + 0x600, b[4], b[5])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address4, address4 + 0x600, b[6], b[7])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address5, address5 + 0x600, b[8], b[9])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address6, address6 + 0x600, b[10], b[11])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address7, address7 + 0x600, b[12], b[13]))
                    ];

                    await Task.WhenAll(tasks);
                }

                UpdateUI(() =>
                {
                    UpdataData(anchorX, anchorY, b);
                    UpdateAllBtn();
                    ResetBtnColor();
                    EnableBtn();
                });

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "LoadFloor: " + ex.Message);
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message, "I say this: never gonna give you up, never gonna let you down.");
            }

            HideMapWait();
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
        #endregion

        #region Processing
        private static int NumOfEmpty(byte[] data, ref bool[,] isEmpty)
        {
            byte[] tempItem = new byte[16];
            byte[] tempItem2 = new byte[16];
            int num = 0;

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Buffer.BlockCopy(data, 0xE0 * i + 0x10 * j, tempItem, 0, 16);
                    if (Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000FEFF000000000000"))
                    {
                        Buffer.BlockCopy(data, 0xE0 * i + 0x10 * j + 0x70, tempItem2, 0, 16);
                        if (Utilities.ByteToHexString(tempItem2).Equals("FEFF000000000000FEFF000000000000"))
                        {
                            isEmpty[i, j] = true;
                            num++;
                        }
                    }
                }
            }
            return num;
        }

        private static void FillFloor(ref byte[][] b, byte[] cur, bool[,] isEmpty, byte[][] item)
        {
            int itemNum = 0;

            for (int i = 0; i < 14; i++)
            {
                b[i] = new byte[112];
            }

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (isEmpty[i, j] && itemNum < item.Length)
                    {
                        TransformToFloorItem(ref b[i * 2], ref b[i * 2 + 1], j, item[itemNum]);
                        itemNum++;
                    }
                    else
                    {
                        Buffer.BlockCopy(cur, 0xE0 * i + 0x10 * j, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(cur, 0xE0 * i + 0x10 * j + 0x70, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }
        }

        private static void FillFloor(ref byte[][] b, byte[][] item)
        {
            int itemNum = 0;
            byte[] emptyLeft = Utilities.StringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.StringToByte("FEFF000000000000FEFF000000000000");

            for (int i = 0; i < 14; i++)
            {
                b[i] = new byte[112];
            }

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (item == null || itemNum >= item.Length)
                    {
                        Buffer.BlockCopy(emptyLeft, 0, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(emptyRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                    }
                    else
                    {
                        TransformToFloorItem(ref b[i * 2], ref b[i * 2 + 1], j, item[itemNum]);
                        itemNum++;
                    }
                }
            }
        }

        private static void FillFloor(ref byte[][] b, byte[] cur, bool[,] isEmpty, string itemID, string itemData, string flag1)
        {
            int itemNum = 0;

            for (int i = 0; i < 14; i++)
            {
                b[i] = new byte[112];
            }

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (isEmpty[i, j])
                    {
                        TransformToFloorItem(ref b[i * 2], ref b[i * 2 + 1], j, itemID, itemData, flag1);
                        itemNum++;
                    }
                    else
                    {
                        Buffer.BlockCopy(cur, 0xE0 * i + 0x10 * j, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(cur, 0xE0 * i + 0x10 * j + 0x70, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }
        }

        private static void TransformToFloorItem(ref byte[] b1, ref byte[] b2, int slot, byte[] item)
        {
            byte[] slotBytes = new byte[2];
            byte[] flag0Bytes = new byte[1];
            byte[] flag1Bytes = new byte[1];
            byte[] dataBytes = new byte[4];

            Buffer.BlockCopy(item, 0x0, slotBytes, 0, 2);
            Buffer.BlockCopy(item, 0x3, flag0Bytes, 0, 1);
            Buffer.BlockCopy(item, 0x2, flag1Bytes, 0, 1);
            Buffer.BlockCopy(item, 0x4, dataBytes, 0, 4);

            string itemID = Utilities.Flip(Utilities.ByteToHexString(slotBytes));
            string itemData = Utilities.Flip(Utilities.ByteToHexString(dataBytes));
            string flag0 = Utilities.ByteToHexString(flag0Bytes);
            string flag1 = "20";

            byte[] dropItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
            byte[] dropItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

            /*
            Debug.Print(Utilities.ByteToHexString(b1));
            Debug.Print(Utilities.ByteToHexString(b2));
            Debug.Print(Utilities.ByteToHexString(dropItemLeft));
            Debug.Print(Utilities.ByteToHexString(dropItemRight));
            */

            Buffer.BlockCopy(dropItemLeft, 0, b1, slot * 0x10, 16);
            Buffer.BlockCopy(dropItemRight, 0, b2, slot * 0x10, 16);

            /*
            Debug.Print(Utilities.ByteToHexString(b1));
            Debug.Print(Utilities.ByteToHexString(b2));
            Debug.Print(Utilities.ByteToHexString(item));
            */
        }

        private static void TransformToFloorItem(ref byte[] b1, ref byte[] b2, int slot, string itemID, string itemData, string flag1)
        {
            string flag0 = "00";

            byte[] dropItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
            byte[] dropItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

            Buffer.BlockCopy(dropItemLeft, 0, b1, slot * 0x10, 16);
            Buffer.BlockCopy(dropItemRight, 0, b2, slot * 0x10, 16);
        }

        private void UpdataData(int x, int y, string itemID, string itemData, string flag0, string flag1, bool shiftRight, bool shiftDown)
        {
            byte[] Left = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
            byte[] Right = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

            int shiftRightValue = 0;
            int shiftDownValue = 0;

            if (shiftRight)
                shiftRightValue = 0x600;
            if (shiftDown)
                shiftDownValue = 0x8;

            if (layer1Btn.Checked)
            {
                Buffer.BlockCopy(Left, 0, Layer1, (x + 16) * 0xC00 + shiftRightValue + y * 0x10 + shiftDownValue, 16);
                Buffer.BlockCopy(Right, 0, Layer1, (x + 16) * 0xC00 + 0x600 + shiftRightValue + y * 0x10 + shiftDownValue, 16);
                miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer1);
            }
            else if (layer2Btn.Checked)
            {
                Buffer.BlockCopy(Left, 0, Layer2, (x + 16) * 0xC00 + shiftRightValue + y * 0x10 + shiftDownValue, 16);
                Buffer.BlockCopy(Right, 0, Layer2, (x + 16) * 0xC00 + 0x600 + shiftRightValue + y * 0x10 + shiftDownValue, 16);
                miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer2);
            }
        }

        private void UpdataDataCoreOnly(int x, int y, string itemID, string itemData, string flag0, string flag1, bool shiftRight, bool shiftDown)
        {
            byte[] Left = Utilities.StringToByte(Utilities.BuildDropCore(itemID, itemData, flag0, flag1));

            int shiftRightValue = 0;
            int shiftDownValue = 0;

            if (shiftRight)
                shiftRightValue = 0x600;
            if (shiftDown)
                shiftDownValue = 0x8;

            if (layer1Btn.Checked)
            {
                Buffer.BlockCopy(Left, 0, Layer1, (x + 16) * 0xC00 + shiftRightValue + y * 0x10 + shiftDownValue, 8);
                miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer1);
            }
            else if (layer2Btn.Checked)
            {
                Buffer.BlockCopy(Left, 0, Layer2, (x + 16) * 0xC00 + shiftRightValue + y * 0x10 + shiftDownValue, 8);
                miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer2);
            }
        }

        private void UpdataData(int x, int y)
        {
            byte[] Left = Utilities.StringToByte(Utilities.BuildDropStringLeft("FFFE", "00000000", "00", "00", true));
            byte[] Right = Utilities.StringToByte(Utilities.BuildDropStringRight("FFFE", true));

            if (layer1Btn.Checked)
            {
                Buffer.BlockCopy(Left, 0, Layer1, (x + 16) * 0xC00 + y * 0x10, 16);
                Buffer.BlockCopy(Right, 0, Layer1, (x + 16) * 0xC00 + 0x600 + y * 0x10, 16);
                miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer1);
            }
            else if (layer2Btn.Checked)
            {
                Buffer.BlockCopy(Left, 0, Layer2, (x + 16) * 0xC00 + y * 0x10, 16);
                Buffer.BlockCopy(Right, 0, Layer2, (x + 16) * 0xC00 + 0x600 + y * 0x10, 16);
                miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer2);
            }
        }

        public void UpdataData(int x, int y, byte[][] newData, bool topleft = true, bool leftToRight = true)
        {
            if (topleft)
            {
                for (int i = 0; i < newData.Length / 2; i++)
                {
                    if (layer1Btn.Checked)
                    {
                        Buffer.BlockCopy(newData[i * 2], 0, Layer1, (x - 3 + i + 16) * 0xC00 + (y - 3) * 0x10, newData[i * 2].Length);
                        Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer1, (x - 3 + i + 16) * 0xC00 + 0x600 + (y - 3) * 0x10, newData[i * 2 + 1].Length);
                    }
                    else if (layer2Btn.Checked)
                    {
                        Buffer.BlockCopy(newData[i * 2], 0, Layer2, (x - 3 + i + 16) * 0xC00 + (y - 3) * 0x10, newData[i * 2].Length);
                        Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer2, (x - 3 + i + 16) * 0xC00 + 0x600 + (y - 3) * 0x10, newData[i * 2 + 1].Length);
                    }
                }
            }
            else
            {
                if (leftToRight)
                {
                    for (int i = 0; i < newData.Length / 2; i++)
                    {
                        if (layer1Btn.Checked)
                        {
                            Buffer.BlockCopy(newData[i * 2], 0, Layer1, (x + i + 16) * 0xC00 + (y) * 0x10, newData[i * 2].Length);
                            Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer1, (x + i + 16) * 0xC00 + 0x600 + (y) * 0x10, newData[i * 2 + 1].Length);
                        }
                        else if (layer2Btn.Checked)
                        {
                            Buffer.BlockCopy(newData[i * 2], 0, Layer2, (x + i + 16) * 0xC00 + (y) * 0x10, newData[i * 2].Length);
                            Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer2, (x + i + 16) * 0xC00 + 0x600 + (y) * 0x10, newData[i * 2 + 1].Length);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < newData.Length / 2; i++)
                    {
                        if (layer1Btn.Checked)
                        {
                            Buffer.BlockCopy(newData[i * 2], 0, Layer1, (x - i + 16) * 0xC00 + (y) * 0x10, newData[i * 2].Length);
                            Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer1, (x - i + 16) * 0xC00 + 0x600 + (y) * 0x10, newData[i * 2 + 1].Length);
                        }
                        else if (layer2Btn.Checked)
                        {
                            Buffer.BlockCopy(newData[i * 2], 0, Layer2, (x - i + 16) * 0xC00 + (y) * 0x10, newData[i * 2].Length);
                            Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer2, (x - i + 16) * 0xC00 + 0x600 + (y) * 0x10, newData[i * 2 + 1].Length);
                        }
                    }
                }
            }

            if (layer1Btn.Checked)
            {
                miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer1);
            }
            else if (layer2Btn.Checked)
            {
                miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer2);
            }
        }

        private void UpdataData(byte[] newLayer)
        {
            if (layer1Btn.Checked)
            {
                Layer1 = newLayer;
                miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer1);
            }
            else if (layer2Btn.Checked)
            {
                Layer2 = newLayer;
                miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer2);
            }
        }

        #endregion

        #region Layer
        private void Layer1Btn_Click(object sender, EventArgs e)
        {
            if (Layer1 == null)
                return;
            //bulkSpawnBtn.Enabled = true;
            //saveBtn.Enabled = true;
            //loadBtn.Enabled = true;
            //fillRemainBtn.Enabled = true;

            miniMapBox.BackgroundImage = null;
            miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer1);
            btnToolTip.RemoveAll();
            DisplayAnchorAsync();
            //ResetBtnColor();
        }

        private void Layer2Btn_Click(object sender, EventArgs e)
        {
            if (Layer2 == null)
                return;
            //bulkSpawnBtn.Enabled = false;
            //saveBtn.Enabled = false;
            //loadBtn.Enabled = false;
            //fillRemainBtn.Enabled = false;

            miniMapBox.BackgroundImage = null;
            miniMapBox.BackgroundImage = miniMap.RefreshItemMap(Layer2);
            btnToolTip.RemoveAll();
            DisplayAnchorAsync();
            //ResetBtnColor();
        }
        #endregion

        #region Variation
        private void Map_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyLog.LogEvent("Map", "Form Closed");
            CloseForm?.Invoke();
        }

        private void Map_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bulkList != null)
            {
                bulkList.Close();
                if (bulkList != null && bulkList.CancelFormClose)
                {
                    e.Cancel = true;
                    bulkList.CancelFormClose = false;
                    return;
                }
                bulkList = null;
            }

            if (selection != null)
            {
                selection.Close();
                selection = null;
            }

            if (itemFilter != null)
            {
                itemFilter.Close();
                itemFilter = null;
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

        private void OpenVariationMenu()
        {
            selection = new Variation(115);
            selection.SendVariationData += Selection_sendVariationData;
            selection.Show(this);
            selection.Location = new Point(Location.X + 528, Location.Y + 660);
            string id = Utilities.PrecedingZeros(selectedItem.FillItemID(), 4);
            string value = Utilities.PrecedingZeros(selectedItem.FillItemData(), 8);
            UInt16 IntId = Convert.ToUInt16("0x" + id, 16);
            if (ItemAttr.HasFenceWithVariation(IntId))  // Fence Variation
            {
                selection.ReceiveID(Utilities.PrecedingZeros(selectedItem.FillItemID(), 4), languageSetting, value);
            }
            else if (id == "315A" || id == "1618" || id == "342F")
            {
                selection.ReceiveID(Utilities.Turn2bytes(selectedItem.FillItemData()), languageSetting);
            }
            else
            {
                selection.ReceiveID(Utilities.PrecedingZeros(selectedItem.FillItemID(), 4), languageSetting);
            }
            variationBtn.BackColor = Color.FromArgb(80, 80, 255);
        }

        private void Selection_sendVariationData(InventorySlot item, int type)
        {
            if (type == 0) //Left click
            {
                selectedItem.Setup(item);
                //updateSelectedItemInfo(selectedItem.displayItemName(), selectedItem.displayItemID(), selectedItem.displayItemData());
                IdTextbox.Text = Utilities.PrecedingZeros(selectedItem.FillItemID(), 4);
                HexTextbox.Text = Utilities.PrecedingZeros(selectedItem.FillItemData(), 8);
            }
            else if (type == 1) // Right click
            {
                if (IdTextbox.Text == "315A" || IdTextbox.Text == "1618" || IdTextbox.Text == "342F")
                {
                    string count = TranslateVariationValue(item.FillItemData()) + Utilities.PrecedingZeros(item.FillItemID(), 4);
                    HexTextbox.Text = count;
                    selectedItem.Setup(GetNameFromID(Utilities.Turn2bytes(IdTextbox.Text), source), Convert.ToUInt16("0x" + IdTextbox.Text, 16), Convert.ToUInt32("0x" + count, 16), GetImagePathFromID(Utilities.Turn2bytes(IdTextbox.Text), source), true, item.GetPath(), selectedItem.GetFlag0(), selectedItem.GetFlag1());
                }
            }
        }

        private void CloseVariationMenu()
        {
            if (selection != null)
            {
                selection.Dispose();
                selection = null;
                variationBtn.BackColor = Color.FromArgb(114, 137, 218);
            }
        }

        private static string TranslateVariationValue(string input)
        {
            int hexValue = Convert.ToUInt16("0x" + input, 16);
            int firstHalf = 0;
            int secondHalf = 0;
            string output;

            if (hexValue <= 0x7)
            {
                return Utilities.PrecedingZeros(input, 4);
            }
            else if (hexValue <= 0x27)
            {
                firstHalf = (0x20 / 4);
                secondHalf = (hexValue - 0x20);
            }
            else if (hexValue <= 0x47)
            {
                firstHalf = (0x40 / 4);
                secondHalf = (hexValue - 0x40);
            }
            else if (hexValue <= 0x67)
            {
                firstHalf = (0x60 / 4);
                secondHalf = (hexValue - 0x60);
            }
            else if (hexValue <= 0x87)
            {
                firstHalf = (0x80 / 4);
                secondHalf = (hexValue - 0x80);
            }
            else if (hexValue <= 0xA7)
            {
                firstHalf = (0xA0 / 4);
                secondHalf = (hexValue - 0xA0);
            }
            else if (hexValue <= 0xC7)
            {
                firstHalf = (0xC0 / 4);
                secondHalf = (hexValue - 0xC0);
            }
            else if (hexValue <= 0xE7)
            {
                firstHalf = (0xE0 / 4);
                secondHalf = (hexValue - 0xE0);
            }

            output = Utilities.PrecedingZeros((firstHalf + secondHalf).ToString("X"), 4);
            return output;
        }

        private void Map_LocationChanged(object sender, EventArgs e)
        {
            if (selection != null)
            {
                selection.Location = new Point(Location.X + 528, Location.Y + 660);
            }

            if (itemFilter != null)
            {
                itemFilter.Location = new Point(Location.X + Width, Location.Y);
            }
        }

        private void UpdateVariation()
        {
            if (IdTextbox.Text.Equals(string.Empty) || HexTextbox.Text.Equals(string.Empty))
                return;

            long data = Convert.ToUInt32(HexTextbox.Text, 16);
            string hexValue = data.ToString("X");

            string itemID = Utilities.PrecedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.PrecedingZeros(hexValue, 8);
            string flag1 = Utilities.PrecedingZeros(FlagTextbox.Text, 2);

            UInt16 IntId = Convert.ToUInt16("0x" + itemID, 16);
            string front = Utilities.PrecedingZeros(itemData, 8).Substring(0, 4);
            string back = Utilities.PrecedingZeros(itemData, 8).Substring(4, 4);

            if (itemID.Equals("315A") || itemID.Equals("1618") || itemID.Equals("342F"))
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source), true, GetImagePathFromID(Utilities.Turn2bytes(itemData), source, Convert.ToUInt32("0x" + Utilities.TranslateVariationValueBack(front), 16)), "00", flag1);
            }
            else if (itemID.Equals("114A") || itemID.Equals("EC9C")) // Money Tree
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source), true, GetImagePathFromID(back, source), "00", flag1);
            }
            else if (itemID.Equals("16A2"))
            {
                selectedItem.Setup(GetNameFromID(itemID, recipeSource), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(Utilities.Turn2bytes(itemData), recipeSource), true, "", "00", flag1);
            }
            else if (ItemAttr.HasFenceWithVariation(IntId))  // Fence Variation
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + front, 16)), true, "", "00", flag1);
            }
            else
            {
                selectedItem.Setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + itemData, 16)), true, "", "00", flag1);
            }

            if (selection != null)
            {
                //selection.Dispose();
                string id = Utilities.PrecedingZeros(selectedItem.FillItemID(), 4);
                string value = Utilities.PrecedingZeros(selectedItem.FillItemData(), 8);

                if (ItemAttr.HasFenceWithVariation(IntId))  // Fence Variation
                {
                    selection.ReceiveID(Utilities.PrecedingZeros(selectedItem.FillItemID(), 4), languageSetting, value);
                }
                else if (id == "315A" || id == "1618" || id == "342F")
                {
                    selection.ReceiveID(Utilities.Turn2bytes(selectedItem.FillItemData()), languageSetting);
                }
                else
                {
                    selection.ReceiveID(Utilities.PrecedingZeros(selectedItem.FillItemID(), 4), languageSetting);
                }
            }
        }
        #endregion

        #region MiniMap
        private void MiniMapBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (drawing)
                return;

            if (e.Button == MouseButtons.Left)
            {
                int x;
                int y;

                if (e.X / 2 < 3)
                    x = 3;
                else if (e.X / 2 > 108)
                    x = 108;
                else
                    x = e.X / 2;

                if (e.Y / 2 < 3)
                    y = 3;
                else if (e.Y / 2 > 92)
                    y = 92;
                else
                    y = e.Y / 2;


                if (drawing)
                    return;

                anchorX = x;
                anchorY = y;

                xCoordinate.Text = x.ToString();
                yCoordinate.Text = y.ToString();
                selectedButton = floor25;

                DisplayAnchorAsync();
            }

        }


        private void MiniMapBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
                return;
            if (e.Button == MouseButtons.Left)
            {
                int x;
                int y;

                if (e.X / 2 < 3)
                    x = 3;
                else if (e.X / 2 > 108)
                    x = 108;
                else
                    x = e.X / 2;

                if (e.Y / 2 < 3)
                    y = 3;
                else if (e.Y / 2 > 92)
                    y = 92;
                else
                    y = e.Y / 2;


                if (drawing)
                    return;

                anchorX = x;
                anchorY = y;

                xCoordinate.Text = x.ToString();
                yCoordinate.Text = y.ToString();
                selectedButton = floor25;

                DisplayAnchorAsync();
            }
        }

        private void SaveTopngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiniMap big = new(Layer1, Acre, Building, Terrain, MapCustomDesgin, 4);
            SaveFileDialog file = new()
            {
                Filter = "Portable Network Graphics (*.png)|*.png",
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
            for (int i = 0; i < temp.Length - 1; i++)
                path = path + temp[i] + "\\";

            config.AppSettings.Settings["LastSave"].Value = path;
            config.Save(ConfigurationSaveMode.Minimal);

            MiniMap.CombineMap(big.DrawBackground(), big.DrawItemMap()).Save(file.FileName);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        #endregion

        #region ProgressBar
        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            UpdateUI(() =>
            {
                if (counter <= MapProgressBar.Maximum)
                    MapProgressBar.Value = counter;
                else
                    MapProgressBar.Value = MapProgressBar.Maximum;
            });
        }

        private void ShowMapWait(int size, string msg = "")
        {
            UpdateUI(() =>
            {
                WaitMessagebox.SelectionAlignment = HorizontalAlignment.Center;
                WaitMessagebox.Text = msg;
                counter = 0;
                MapProgressBar.Maximum = size + 5;
                MapProgressBar.Value = counter;
                PleaseWaitPanel.Visible = true;
                ProgressTimer.Start();
            });
        }

        private void HideMapWait()
        {
            UpdateUI(() =>
            {
                PleaseWaitPanel.Visible = false;
                ProgressTimer.Stop();
            });
        }

        private void UpdateUI(Action action)
        {
            this.BeginInvoke((Action)(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    // Handle or log the exception
                    MyMessageBox.Show("UI update error: " + ex.Message);
                }
            }));
        }

        private void DisableBtn()
        {
            BtnPanel.Enabled = false;
            functionPanel.Enabled = false;
            selectedItem.Enabled = false;
            moveRightBtn.Enabled = false;
            moveLeftBtn.Enabled = false;
            moveUpBtn.Enabled = false;
            moveDownBtn.Enabled = false;
            moveUpRightBtn.Enabled = false;
            moveUpLeftBtn.Enabled = false;
            moveDownRightBtn.Enabled = false;
            moveDownLeftBtn.Enabled = false;
            moveRight7Btn.Enabled = false;
            moveLeft7Btn.Enabled = false;
            moveUp7Btn.Enabled = false;
            moveDown7Btn.Enabled = false;
        }

        private void EnableBtn()
        {
            BtnPanel.Enabled = true;
            functionPanel.Enabled = true;
            selectedItem.Enabled = true;
            moveRightBtn.Enabled = true;
            moveLeftBtn.Enabled = true;
            moveUpBtn.Enabled = true;
            moveDownBtn.Enabled = true;
            moveUpRightBtn.Enabled = true;
            moveUpLeftBtn.Enabled = true;
            moveDownRightBtn.Enabled = true;
            moveDownLeftBtn.Enabled = true;
            moveRight7Btn.Enabled = true;
            moveLeft7Btn.Enabled = true;
            moveUp7Btn.Enabled = true;
            moveDown7Btn.Enabled = true;
        }
        #endregion

        #region ReAnchor
        private void ReAnchorBtn_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                /*
                byte[] Coordinate = Utilities.GetCoordinate(s, usb);
                int x = BitConverter.ToInt32(Coordinate, 0);
                int y = BitConverter.ToInt32(Coordinate, 4);

                anchorX = x - 0x24;
                anchorY = y - 0x18;
                */

                if (anchorX < 3 || anchorY < 3 || anchorX > 108 || anchorY > 92)
                {
                    anchorX = 56;
                    anchorY = 48;
                }

                DisplayAnchorAsync();

                xCoordinate.Text = anchorX.ToString();
                yCoordinate.Text = anchorY.ToString();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "getCoordinate: " + ex.Message);
                MyMessageBox.Show(ex.Message, "Weed Effect !");

                anchorX = 56;
                anchorY = 48;

                DisplayAnchorAsync();

                xCoordinate.Text = anchorX.ToString();
                yCoordinate.Text = anchorY.ToString();
            }
        }
        #endregion

        #region Bulk Spawn/Remove
        private void BulkSpawnBtn_Click(object sender, EventArgs e)
        {
            removeItemClick.Show(bulkSpawnBtn, new Point(0, bulkSpawnBtn.Height));
        }

        private void BulkSpawnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bulkSpawn == null)
            {
                if (layer1Btn.Checked)
                    bulkSpawn = new BulkSpawn(s, usb, Layer1, Layer2, Acre, Building, Terrain, MapCustomDesgin, anchorX, anchorY, ignore, sound, debugging, true);
                else
                    bulkSpawn = new BulkSpawn(s, usb, Layer1, Layer2, Acre, Building, Terrain, MapCustomDesgin, anchorX, anchorY, ignore, sound, debugging, false);
            }
            bulkSpawn.StartPosition = FormStartPosition.CenterParent;
            bulkSpawn.SetOwner(this);
            bulkSpawn.ShowDialog(this);
        }

        static readonly byte[] EMPTY_TILE = { 0xFE, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        ushort GetItemID(byte[] data, int offset)
        {
            return (ushort)((data[offset + 1] << 8) | data[offset]);
        }

        private void WeedsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MyMessageBox.Show("Are you sure you want to remove all weeds on your island (Layer 1 only)?", "Oh No! Not the Weeds!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;

            DisableBtn();

            byte[] emptyLeft = EMPTY_TILE;
            byte[] emptyRight = EMPTY_TILE;

            byte[] processLayer = (byte[])Layer1.Clone();

            bool[] change = new bool[Utilities.ExtendedMapNumOfColumn / 2];

            for (int i = 0; i < Utilities.ExtendedMapNumOfColumn / 2; i++)
            {
                for (int j = 0; j < Utilities.ExtendedMapNumOfRow; j++)
                {
                    // Process first section
                    int offset1 = i * 0x1800 + j * 0x10;
                    ushort itemID1 = GetItemID(Layer1, offset1);
                    if (ItemAttr.IsWeed(itemID1))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset1, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset1 + 0x600, 16);
                        change[i] = true;
                    }

                    // Process second section
                    int offset2 = i * 0x1800 + 0xC00 + j * 0x10;
                    ushort itemID2 = GetItemID(Layer1, offset2);
                    if (ItemAttr.IsWeed(itemID2))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset2, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset2 + 0x600, 16);
                        change[i] = true;
                    }
                }
            }

            Thread renewThread = new(delegate () { Renew(processLayer, change); });
            renewThread.Start();
        }

        private void FlowersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MyMessageBox.Show("Are you sure you want to remove all flowers on your island (Layer 1 only)?", "Photoshop Flowey", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;

            DisableBtn();

            byte[] emptyLeft = EMPTY_TILE;
            byte[] emptyRight = EMPTY_TILE;

            byte[] processLayer = (byte[])Layer1.Clone();

            bool[] change = new bool[Utilities.ExtendedMapNumOfColumn / 2];

            for (int i = 0; i < Utilities.ExtendedMapNumOfColumn / 2; i++)
            {
                for (int j = 0; j < Utilities.ExtendedMapNumOfRow; j++)
                {
                    // Process first section
                    int offset1 = i * 0x1800 + j * 0x10;
                    ushort itemID1 = GetItemID(Layer1, offset1);
                    if (ItemAttr.HasGenetics(itemID1))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset1, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset1 + 0x600, 16);
                        change[i] = true;
                    }

                    // Process second section
                    int offset2 = i * 0x1800 + 0xC00 + j * 0x10;
                    ushort itemID2 = GetItemID(Layer1, offset2);
                    if (ItemAttr.HasGenetics(itemID2))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset2, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset2 + 0x600, 16);
                        change[i] = true;
                    }
                }
            }

            Thread renewThread = new(delegate () { Renew(processLayer, change); });
            renewThread.Start();
        }

        private void TreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MyMessageBox.Show("Are you sure you want to remove all trees on your island (Layer 1 only)?", "Team Trees is stupid!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;

            DisableBtn();

            byte[] emptyLeft = EMPTY_TILE;
            byte[] emptyRight = EMPTY_TILE;

            byte[] processLayer = (byte[])Layer1.Clone();

            bool[] change = new bool[Utilities.ExtendedMapNumOfColumn / 2];

            for (int i = 0; i < Utilities.ExtendedMapNumOfColumn / 2; i++)
            {
                for (int j = 0; j < Utilities.ExtendedMapNumOfRow; j++)
                {
                    // Process first section
                    int offset1 = i * 0x1800 + j * 0x10;
                    ushort itemID1 = GetItemID(Layer1, offset1);
                    if (ItemAttr.IsTree(itemID1))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset1, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset1 + 0x600, 16);
                        change[i] = true;
                    }

                    // Process second section
                    int offset2 = i * 0x1800 + 0xC00 + j * 0x10;
                    ushort itemID2 = GetItemID(Layer1, offset2);
                    if (ItemAttr.IsTree(itemID2))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset2, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset2 + 0x600, 16);
                        change[i] = true;
                    }
                }
            }

            Thread renewThread = new(delegate () { Renew(processLayer, change); });
            renewThread.Start();
        }

        private void BushesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MyMessageBox.Show("Are you sure you want to remove all bushes on your island (Layer 1 only)?", "Have you ever seen an elephant hiding in the bushes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;

            DisableBtn();

            byte[] emptyLeft = EMPTY_TILE;
            byte[] emptyRight = EMPTY_TILE;

            byte[] processLayer = (byte[])Layer1.Clone();

            bool[] change = new bool[Utilities.ExtendedMapNumOfColumn / 2];

            for (int i = 0; i < Utilities.ExtendedMapNumOfColumn / 2; i++)
            {
                for (int j = 0; j < Utilities.ExtendedMapNumOfRow; j++)
                {
                    // Process first section
                    int offset1 = i * 0x1800 + j * 0x10;
                    ushort itemID1 = GetItemID(Layer1, offset1);
                    if (ItemAttr.IsBush(itemID1))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset1, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset1 + 0x600, 16);
                        change[i] = true;
                    }

                    // Process second section
                    int offset2 = i * 0x1800 + 0xC00 + j * 0x10;
                    ushort itemID2 = GetItemID(Layer1, offset2);
                    if (ItemAttr.IsBush(itemID2))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset2, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset2 + 0x600, 16);
                        change[i] = true;
                    }
                }
            }

            Thread renewThread = new(delegate () { Renew(processLayer, change); });
            renewThread.Start();
        }

        private void FencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MyMessageBox.Show("Are you sure you want to remove all fences on your island (Layer 1 only)?", "I said to my mate Noah: \"You should change your surname to Fence...\"", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;

            DisableBtn();

            byte[] emptyLeft = EMPTY_TILE;
            byte[] emptyRight = EMPTY_TILE;

            byte[] processLayer = (byte[])Layer1.Clone();

            bool[] change = new bool[Utilities.ExtendedMapNumOfColumn / 2];

            for (int i = 0; i < Utilities.ExtendedMapNumOfColumn / 2; i++)
            {
                for (int j = 0; j < Utilities.ExtendedMapNumOfRow; j++)
                {
                    // Process first section
                    int offset1 = i * 0x1800 + j * 0x10;
                    ushort itemID1 = GetItemID(Layer1, offset1);
                    if (ItemAttr.IsPlacedFence(itemID1))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset1, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset1 + 0x600, 16);
                        change[i] = true;
                    }

                    // Process second section
                    int offset2 = i * 0x1800 + 0xC00 + j * 0x10;
                    ushort itemID2 = GetItemID(Layer1, offset2);
                    if (ItemAttr.IsPlacedFence(itemID2))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset2, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset2 + 0x600, 16);
                        change[i] = true;
                    }
                }
            }

            Thread renewThread = new(delegate () { Renew(processLayer, change); });
            renewThread.Start();
        }

        private void ShellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MyMessageBox.Show("Are you sure you want to remove all shells on your island (Layer 1 only)?", "You would think that a snail without a shell would move a bit faster...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;

            DisableBtn();

            byte[] emptyLeft = EMPTY_TILE;
            byte[] emptyRight = EMPTY_TILE;

            byte[] processLayer = (byte[])Layer1.Clone();

            bool[] change = new bool[Utilities.ExtendedMapNumOfColumn / 2];

            for (int i = 0; i < Utilities.ExtendedMapNumOfColumn / 2; i++)
            {
                for (int j = 0; j < Utilities.ExtendedMapNumOfRow; j++)
                {
                    // Process first section
                    int offset1 = i * 0x1800 + j * 0x10;
                    ushort itemID1 = GetItemID(Layer1, offset1);
                    if (ItemAttr.IsShell(itemID1))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset1, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset1 + 0x600, 16);
                        change[i] = true;
                    }

                    // Process second section
                    int offset2 = i * 0x1800 + 0xC00 + j * 0x10;
                    ushort itemID2 = GetItemID(Layer1, offset2);
                    if (ItemAttr.IsShell(itemID2))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset2, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset2 + 0x600, 16);
                        change[i] = true;
                    }
                }
            }

            Thread renewThread = new(delegate () { Renew(processLayer, change); });
            renewThread.Start();
        }

        private void DiysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MyMessageBox.Show("Are you sure you want to remove all DIYs on your island (Layer 1 only)?", "DiWHY - Reddit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;

            DisableBtn();

            byte[] emptyLeft = EMPTY_TILE;
            byte[] emptyRight = EMPTY_TILE;

            byte[] processLayer = (byte[])Layer1.Clone();

            bool[] change = new bool[Utilities.ExtendedMapNumOfColumn / 2];

            for (int i = 0; i < Utilities.ExtendedMapNumOfColumn / 2; i++)
            {
                for (int j = 0; j < Utilities.ExtendedMapNumOfRow; j++)
                {
                    // Process first section
                    int offset1 = i * 0x1800 + j * 0x10;
                    ushort itemID1 = GetItemID(Layer1, offset1);
                    if (itemID1.Equals(0x16A2))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset1, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset1 + 0x600, 16);
                        change[i] = true;
                    }

                    // Process second section
                    int offset2 = i * 0x1800 + 0xC00 + j * 0x10;
                    ushort itemID2 = GetItemID(Layer1, offset2);
                    if (itemID2.Equals(0x16A2))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset2, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset2 + 0x600, 16);
                        change[i] = true;
                    }
                }
            }

            Thread renewThread = new(delegate () { Renew(processLayer, change); });
            renewThread.Start();
        }

        private void RocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MyMessageBox.Show("Are you sure you want to remove all ore/bell rocks on your island (Layer 1 only)?", "Girls are like rocks, the flat ones get skipped...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;

            DisableBtn();

            byte[] emptyLeft = EMPTY_TILE;
            byte[] emptyRight = EMPTY_TILE;

            byte[] processLayer = (byte[])Layer1.Clone();

            bool[] change = new bool[Utilities.ExtendedMapNumOfColumn / 2];

            for (int i = 0; i < Utilities.ExtendedMapNumOfColumn / 2; i++)
            {
                for (int j = 0; j < Utilities.ExtendedMapNumOfRow; j++)
                {
                    // Process first section
                    int offset1 = i * 0x1800 + j * 0x10;
                    ushort itemID1 = GetItemID(Layer1, offset1);
                    if (ItemAttr.IsStone(itemID1))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset1, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset1 + 0x600, 16);
                        change[i] = true;
                    }

                    // Process second section
                    int offset2 = i * 0x1800 + 0xC00 + j * 0x10;
                    ushort itemID2 = GetItemID(Layer1, offset2);
                    if (ItemAttr.IsStone(itemID2))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, offset2, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, offset2 + 0x600, 16);
                        change[i] = true;
                    }
                }
            }

            Thread renewThread = new(delegate () { Renew(processLayer, change); });
            renewThread.Start();
        }

        private void EverythingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult;
            if (layer1Btn.Checked)
            {
                dialogResult = MyMessageBox.Show("Are you sure you want to remove all dropped/placed item on your island (Layer 1 only)?", "Is everything a joke to you ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                    return;

                DisableBtn();

                ProcessLayer(Layer1, Utilities.mapZero);
            }
            else
            {
                dialogResult = MyMessageBox.Show("Are you sure you want to remove all dropped/placed item on your island (Layer 2 only)?", "Is everything a joke to you ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                    return;

                DisableBtn();

                ProcessLayer(Layer2, Utilities.mapZero + Utilities.mapSize);
            }
        }
        private void ProcessLayer(byte[] Layer, long Address)
        {
            byte[] empty = { 0xFE, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            byte[] processLayer = (byte[])Layer.Clone();

            bool[] HasChange = new bool[Utilities.ExtendedMapNumOfColumn / 2];

            for (int i = 0; i < Utilities.ExtendedMapNumOfColumn / 2; i++)
            {
                for (int j = 0; j < Utilities.ExtendedMapNumOfRow * 8; j++)
                {
                    int offset = i * 0x1800 + j * 0x8;
                    ushort itemID = GetItemID(Layer, offset);

                    if (itemID != 0xFFFE)
                    {
                        Buffer.BlockCopy(empty, 0, processLayer, offset, 8);
                        HasChange[i] = true;
                    }
                }
            }

            Thread renewThread = new(delegate () { Renew(processLayer, HasChange, Address); });
            renewThread.Start();
        }

        private void Renew(byte[] newLayer, Boolean[] change)
        {
            int num = NumOfWrite(change);
            if (num == 0)
            {
                UpdateUI(() =>
                {
                    EnableBtn();
                });
                return;
            }

            ShowMapWait(num * 2, "Removing Item...");

            try
            {
                if (!debugging)
                {
                    Debug.Print("Length :" + num + " Time : " + (num + 3));

                    /*
                    int c = 0;

                    while (Utilities.IsAboutToSave(s, usb, num, saveTime, ignore))
                    {
                        if (c > 10)
                        {
                            if (IgnoreAutosave())
                            {
                                break;
                            }
                            else
                            {
                                UpdateUI(() =>
                                {
                                    EnableBtn();
                                });

                                if (sound)
                                    System.Media.SystemSounds.Asterisk.Play();

                                HideMapWait();
                                return;
                            }
                        }
                        c++;
                        Thread.Sleep(3000);
                    }
                    */

                    for (int i = 0; i < Utilities.ExtendedMapNumOfColumn / 2; i++)
                    {
                        if (change[i])
                        {
                            byte[] column = new byte[0x1800];
                            Buffer.BlockCopy(newLayer, i * 0x1800, column, 0, 0x1800);
                            Utilities.DropRenewColumn(s, usb, (uint)(Utilities.mapZero + (i * 0x1800)), column);
                        }
                    }
                }

                UpdateUI(() =>
                {
                    UpdataData(newLayer);
                    MoveAnchor(anchorX, anchorY);
                    ResetBtnColor();
                    EnableBtn();
                });

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "Renew: " + ex.Message);
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message, "Fixing our most awful code. To you, true saviors, kings of men.");
            }

            HideMapWait();
        }

        private void Renew(byte[] newLayer, Boolean[] change, long address)
        {
            int num = NumOfWrite(change);
            if (num == 0)
            {
                UpdateUI(() =>
                {
                    EnableBtn();
                });
                return;
            }

            ShowMapWait(num * 2, "Removing Item...");

            try
            {
                if (!debugging)
                {
                    Debug.Print("Length :" + num + " Time : " + (num + 3));

                    /*
                    int c = 0;

                    while (Utilities.IsAboutToSave(s, usb, num + 10, saveTime, ignore))
                    {
                        if (c > 10)
                        {
                            if (IgnoreAutosave())
                            {
                                break;
                            }
                            else
                            {
                                UpdateUI(() =>
                                {
                                    EnableBtn();
                                });

                                if (sound)
                                    System.Media.SystemSounds.Asterisk.Play();

                                HideMapWait();
                                return;
                            }
                        }
                        c++;
                        Thread.Sleep(3000);
                    }
                    */
                }

                for (int i = 0; i < Utilities.ExtendedMapNumOfColumn / 2; i++)
                {
                    if (change[i])
                    {
                        byte[] column = new byte[0x1800];
                        Buffer.BlockCopy(newLayer, i * 0x1800, column, 0, 0x1800);
                        Utilities.DropRenewColumn(s, usb, (uint)(address + (i * 0x1800)), column);
                    }
                }

                UpdateUI(() =>
                {
                    UpdataData(newLayer);
                    MoveAnchor(anchorX, anchorY);
                    ResetBtnColor();
                    EnableBtn();
                });

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "Renew: " + ex.Message);
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message, "Fixing our most awful code. To you, true saviors, kings of men.");
            }

            HideMapWait();
        }

        private static int NumOfWrite(Boolean[] change)
        {
            int num = 0;
            for (int i = 0; i < change.Length; i++)
            {
                if (change[i])
                    num++;
            }
            return num;
        }
        #endregion

        #region AutoSave

        
        private int NextAutosave()
        {
            try
            {
                byte[] b = Utilities.GetSaving(s, usb) ?? throw new NullReferenceException("Save");
                byte[] currentFrame = new byte[4];
                byte[] lastFrame = new byte[4];
                Buffer.BlockCopy(b, 12, currentFrame, 0, 4);
                Buffer.BlockCopy(b, 16, lastFrame, 0, 4);

                int currentFrameStr = Convert.ToInt32("0x" + Utilities.Flip(Utilities.ByteToHexString(currentFrame)), 16);
                int lastFrameStr = Convert.ToInt32("0x" + Utilities.Flip(Utilities.ByteToHexString(lastFrame)), 16);

                return (((0x1518 - (currentFrameStr - lastFrameStr))) / 30);
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "NextAutoSave: " + ex.Message);
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message + "\nThe connection to the Switch ended.\n\nDid the Switch enter sleep mode?", "Ugandan Knuckles: \"Oh No!\"", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 69;
            }
        }
        

        private void NextSaveTimer_Tick(object sender, EventArgs e)
        {
            if (saveTime <= -30)
            {
                if (!keepProtection)
                {
                    NextSaveTimer.Stop();
                    DialogResult result = MyMessageBox.Show("It seems autosave have been paused.\n" +
                                                    "You might have a visitor on your island, or your inventory stay open.\n" +
                                                    "Or you are at the title screen waiting to \"Press A\".\n" +
                                                    "Or you are still listening to Isabelle's useless announcement...\n\n" +
                                                    "Anyhow, would you like the Map Dropper to ignore the autosave protection at the moment?\n\n" +
                                                    "Note that spawning item during autosave might crash the game."
                                                    , "Waiting for autosave to complete...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ignore = true;
                        nextAutoSaveSecond.ForeColor = Color.Red;
                        MyLog.LogEvent("Map", "Autosave Ignored");
                    }
                    else
                    {
                        keepProtection = true;
                        saveTime = NextAutosave();
                        nextAutoSaveSecond.Text = saveTime.ToString();
                        NextSaveTimer.Start();
                        keepProtectionCounter++;
                    }
                }
                else
                {
                    if (keepProtectionCounter % 10 == 0)
                    {
                        saveTime = NextAutosave();
                        nextAutoSaveSecond.Text = saveTime.ToString();
                    }
                    keepProtectionCounter++;
                }
            }
            else if (saveTime <= 0)
            {
                saveTime = NextAutosave();
                nextAutoSaveSecond.Text = saveTime.ToString();
            }
            else
            {
                saveTime--;
                nextAutoSaveSecond.Text = saveTime.ToString();
            }
        }

        #endregion

        #region Replace
        private void ReplaceItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = (sender as ToolStripItem);

            if (item != null)
            {
                if (item.Owner is ContextMenuStrip owner)
                {
                    var btn = (FloorSlot)owner.SourceControl;

                    if (anchorX < 0 || anchorY < 0)
                    {
                        return;
                    }

                    if (IdTextbox.Text == "" || HexTextbox.Text == "" || FlagTextbox.Text == "")
                    {
                        return;
                    }

                    long address;
                    byte[] processLayer = [];
                    if (layer1Btn.Checked)
                    {
                        address = Utilities.mapZero;
                        processLayer = Utilities.Add(processLayer, Layer1);
                    }
                    else if (layer2Btn.Checked)
                    {
                        address = Utilities.mapZero + Utilities.mapSize;
                        processLayer = Utilities.Add(processLayer, Layer2);
                    }
                    else
                        return;

                    string itemID = Utilities.PrecedingZeros(IdTextbox.Text, 4);
                    string itemData = Utilities.PrecedingZeros(HexTextbox.Text, 8);
                    string flag1 = Utilities.PrecedingZeros(FlagTextbox.Text, 2);

                    DisableBtn();

                    btnToolTip.RemoveAll();

                    if (ModifierKeys == Keys.Shift)
                    {
                        try
                        {
                            byte[] tempLeft = new byte[16];
                            byte[] tempRight = new byte[16];

                            string targetLeft = Utilities.BuildDropStringLeft(Utilities.PrecedingZeros(btn.ItemID.ToString("X"), 4), Utilities.PrecedingZeros(btn.ItemData.ToString("X"), 8), btn.Flag0, btn.Flag1);
                            string targetRight = Utilities.BuildDropStringRight(Utilities.PrecedingZeros(btn.ItemID.ToString("X"), 4));

                            byte[] resultLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, "00", flag1));
                            byte[] resultRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

                            Boolean[] change = new Boolean[56];

                            for (int i = 0; i < 56; i++)
                            {
                                for (int j = 0; j < 96; j++)
                                {
                                    Buffer.BlockCopy(processLayer, i * 0x1800 + j * 0x10, tempLeft, 0, 16);
                                    Buffer.BlockCopy(processLayer, i * 0x1800 + 0x600 + j * 0x10, tempRight, 0, 16);

                                    if (Utilities.ByteToHexString(tempLeft).Equals(targetLeft) && Utilities.ByteToHexString(tempRight).Equals(targetRight))
                                    {
                                        Buffer.BlockCopy(resultLeft, 0, processLayer, i * 0x1800 + j * 0x10, 16);
                                        Buffer.BlockCopy(resultRight, 0, processLayer, i * 0x1800 + 0x600 + j * 0x10, 16);
                                        change[i] = true;
                                    }

                                    Buffer.BlockCopy(processLayer, i * 0x1800 + 0xC00 + j * 0x10, tempLeft, 0, 16);
                                    Buffer.BlockCopy(processLayer, i * 0x1800 + 0xC00 + 0x600 + j * 0x10, tempRight, 0, 16);

                                    if (Utilities.ByteToHexString(tempLeft).Equals(targetLeft) && Utilities.ByteToHexString(tempRight).Equals(targetRight))
                                    {
                                        Buffer.BlockCopy(resultLeft, 0, processLayer, i * 0x1800 + 0xC00 + j * 0x10, 16);
                                        Buffer.BlockCopy(resultRight, 0, processLayer, i * 0x1800 + 0xC00 + 0x600 + j * 0x10, 16);
                                        change[i] = true;
                                    }
                                }
                            }

                            Thread renewThread = new(delegate () { Renew(processLayer, change); });
                            renewThread.Start();

                        }
                        catch (Exception ex)
                        {
                            MyLog.LogEvent("Map", "Replace: " + ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            Thread ReplaceThread = new(delegate () { ReplaceGrid(address, btn, itemID, itemData, flag1); });
                            ReplaceThread.Start();
                        }
                        catch (Exception ex)
                        {
                            MyLog.LogEvent("Map", "Replace: " + ex.Message);
                        }
                    }
                }
            }
        }

        private async void ReplaceGrid(long address, FloorSlot btn, string itemID, string itemData, string flag1)
        {
            ShowMapWait(14, "Replacing Items...");

            try
            {
                byte[][] b = new byte[14][];

                if (debugging)
                {
                    byte[] curFloor = new byte[1568];

                    int start = (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3));

                    Buffer.BlockCopy(Layer1, start, curFloor, 0x0, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x600, curFloor, 0x70, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0xC00, curFloor, 0xE0, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x1200, curFloor, 0x150, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x1800, curFloor, 0x1C0, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x1E00, curFloor, 0x230, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x2400, curFloor, 0x2A0, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x2A00, curFloor, 0x310, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x3000, curFloor, 0x380, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x3600, curFloor, 0x3F0, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x3C00, curFloor, 0x460, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x4200, curFloor, 0x4D0, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x4800, curFloor, 0x540, 0x70);
                    Buffer.BlockCopy(Layer1, start + 0x4E00, curFloor, 0x5B0, 0x70);

                    ReplaceItem(ref b, curFloor, itemID, itemData, flag1, btn);
                }
                else
                {

                    UInt32 currentColumn = (UInt32)(address + (0xC00 * (anchorX - 3 + 16)) + (0x10 * (anchorY - 3)));

                    byte[] readFloor = Utilities.Read7x7Floor(s, usb, currentColumn);
                    byte[] curFloor = new byte[1568];

                    Buffer.BlockCopy(readFloor, 0x0, curFloor, 0x0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x600, curFloor, 0x70, 0x70);
                    Buffer.BlockCopy(readFloor, 0xC00, curFloor, 0xE0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x1200, curFloor, 0x150, 0x70);
                    Buffer.BlockCopy(readFloor, 0x1800, curFloor, 0x1C0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x1E00, curFloor, 0x230, 0x70);
                    Buffer.BlockCopy(readFloor, 0x2400, curFloor, 0x2A0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x2A00, curFloor, 0x310, 0x70);
                    Buffer.BlockCopy(readFloor, 0x3000, curFloor, 0x380, 0x70);
                    Buffer.BlockCopy(readFloor, 0x3600, curFloor, 0x3F0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x3C00, curFloor, 0x460, 0x70);
                    Buffer.BlockCopy(readFloor, 0x4200, curFloor, 0x4D0, 0x70);
                    Buffer.BlockCopy(readFloor, 0x4800, curFloor, 0x540, 0x70);
                    Buffer.BlockCopy(readFloor, 0x4E00, curFloor, 0x5B0, 0x70);

                    ReplaceItem(ref b, curFloor, itemID, itemData, flag1, btn);

                    UInt32 address1;
                    UInt32 address2;
                    UInt32 address3;
                    UInt32 address4;
                    UInt32 address5;
                    UInt32 address6;
                    UInt32 address7;

                    if (layer1Btn.Checked)
                    {
                        address1 = GetAddress(anchorX - 3, anchorY - 3);
                        address2 = GetAddress(anchorX - 2, anchorY - 3);
                        address3 = GetAddress(anchorX - 1, anchorY - 3);
                        address4 = GetAddress(anchorX, anchorY - 3);
                        address5 = GetAddress(anchorX + 1, anchorY - 3);
                        address6 = GetAddress(anchorX + 2, anchorY - 3);
                        address7 = GetAddress(anchorX + 3, anchorY - 3);
                    }
                    else if (layer2Btn.Checked)
                    {
                        address1 = GetAddress(anchorX - 3, anchorY - 3) + Utilities.mapSize;
                        address2 = GetAddress(anchorX - 2, anchorY - 3) + Utilities.mapSize;
                        address3 = GetAddress(anchorX - 1, anchorY - 3) + Utilities.mapSize;
                        address4 = GetAddress(anchorX, anchorY - 3) + Utilities.mapSize;
                        address5 = GetAddress(anchorX + 1, anchorY - 3) + Utilities.mapSize;
                        address6 = GetAddress(anchorX + 2, anchorY - 3) + Utilities.mapSize;
                        address7 = GetAddress(anchorX + 3, anchorY - 3) + Utilities.mapSize;
                    }
                    else
                        return;

                    /*
                    int c = 0;

                    while (Utilities.IsAboutToSave(s, usb, 10, saveTime, ignore))
                    {
                        if (c > 10)
                        {
                            if (IgnoreAutosave())
                            {
                                break;
                            }
                            else
                            {
                                UpdateUI(() =>
                                {
                                    EnableBtn();
                                });

                                if (sound)
                                    System.Media.SystemSounds.Asterisk.Play();

                                HideMapWait();
                                return;
                            }
                        }
                        c++;
                        Thread.Sleep(3000);
                    }
                    */

                    List<Task> tasks =
                    [
                        Task.Run(() => Utilities.DropColumn(s, usb, address1, address1 + 0x600, b[0], b[1])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address2, address2 + 0x600, b[2], b[3])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address3, address3 + 0x600, b[4], b[5])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address4, address4 + 0x600, b[6], b[7])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address5, address5 + 0x600, b[8], b[9])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address6, address6 + 0x600, b[10], b[11])),
                        Task.Run(() => Utilities.DropColumn(s, usb, address7, address7 + 0x600, b[12], b[13]))
                    ];

                    await Task.WhenAll(tasks);
                }

                UpdateUI(() =>
                {
                    UpdataData(anchorX, anchorY, b);
                    UpdateAllBtn();
                    ResetBtnColor();
                    EnableBtn();
                });

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "ReplaceItem: " + ex.Message);
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message, "I say this: Never gonna run around and desert you.");
            }

            HideMapWait();
        }

        private static void ReplaceItem(ref byte[][] b, byte[] cur, string itemID, string itemData, string flag1, FloorSlot btn)
        {
            byte[] tempLeft = new byte[16];
            byte[] tempRight = new byte[16];

            string targetLeft = Utilities.BuildDropStringLeft(Utilities.PrecedingZeros(btn.ItemID.ToString("X"), 4), Utilities.PrecedingZeros(btn.ItemData.ToString("X"), 8), btn.Flag0, btn.Flag1);
            string targetRight = Utilities.BuildDropStringRight(Utilities.PrecedingZeros(btn.ItemID.ToString("X"), 4));

            byte[] resultLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, "00", flag1));
            byte[] resultRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

            for (int i = 0; i < 14; i++)
            {
                b[i] = new byte[112];
            }

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Buffer.BlockCopy(cur, 0xE0 * i + 0x10 * j, tempLeft, 0, 16);
                    Buffer.BlockCopy(cur, 0xE0 * i + 0x10 * j + 0x70, tempRight, 0, 16);

                    if (!Utilities.ByteToHexString(tempLeft).Equals(targetLeft) || !Utilities.ByteToHexString(tempRight).Equals(targetRight))
                    {
                        Buffer.BlockCopy(cur, 0xE0 * i + 0x10 * j, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(cur, 0xE0 * i + 0x10 * j + 0x70, b[i * 2 + 1], 0x10 * j, 16);
                    }
                    else
                    {
                        Buffer.BlockCopy(resultLeft, 0, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(resultRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }
        }
        #endregion

        #region FlagEdit
        private void Flag20ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AreaSet || AreaCopied)
            {
                if (Corner1X < 0 || Corner1Y < 0 || Corner2X < 0 || Corner2Y < 0 || Corner1X > 111 || Corner1Y > 95 || Corner2X > 111 || Corner2Y > 95)
                {
                    MyMessageBox.Show("Selected Area Out of Bounds!", "Please use your brain, My Master.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int TopLeftX;
                int TopLeftY;
                int BottomRightX;
                int BottomRightY;

                if (Corner1X <= Corner2X)
                {
                    if (Corner1Y <= Corner2Y) // Top Left
                    {
                        TopLeftX = Corner1X;
                        TopLeftY = Corner1Y;
                        BottomRightX = Corner2X;
                        BottomRightY = Corner2Y;
                    }
                    else // Bottom Left
                    {
                        TopLeftX = Corner1X;
                        TopLeftY = Corner2Y; //
                        BottomRightX = Corner2X;
                        BottomRightY = Corner1Y; //
                    }
                }
                else
                {
                    if (Corner1Y <= Corner2Y) // Top Right
                    {
                        TopLeftX = Corner2X; //
                        TopLeftY = Corner1Y;
                        BottomRightX = Corner1X; //
                        BottomRightY = Corner2Y;
                    }
                    else // Bottom Left
                    {
                        TopLeftX = Corner2X;
                        TopLeftY = Corner2Y;
                        BottomRightX = Corner1X;
                        BottomRightY = Corner1Y;
                    }
                }

                int numberOfColumn = BottomRightX - TopLeftX + 1;
                int numberOfRow = BottomRightY - TopLeftY + 1;

                int sizeOfRow = 16;

                SavedArea = new byte[numberOfColumn * 2][];

                for (int i = 0; i < numberOfColumn * 2; i++)
                {
                    SavedArea[i] = new byte[numberOfRow * sizeOfRow];
                }

                for (int i = 0; i < numberOfColumn; i++)
                {
                    for (int j = 0; j < numberOfRow; j++)
                    {
                        if (layer1Btn.Checked)
                        {
                            Buffer.BlockCopy(Layer1, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer1, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)) + 0x600, SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                            if (SavedArea[i * 2][0x10 * j] != 0xFE || SavedArea[i * 2][0x10 * j + 1] != 0xFF)
                                SavedArea[i * 2][0x10 * j + 2] = 0x20;
                        }
                        else
                        {
                            Buffer.BlockCopy(Layer2, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer2, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)) + 0x600, SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                            if (SavedArea[i * 2][0x10 * j] != 0xFE || SavedArea[i * 2][0x10 * j + 1] != 0xFF)
                                SavedArea[i * 2][0x10 * j + 2] = 0x20;
                        }
                    }
                }

                long address;

                if (layer1Btn.Checked)
                {
                    address = Utilities.mapZero;
                }
                else if (layer2Btn.Checked)
                {
                    address = Utilities.mapZero + Utilities.mapSize;
                }
                else
                    return;

                DisableBtn();

                Thread pasteAreaThread = new(delegate () { PasteArea(TopLeftX, TopLeftY, numberOfColumn); });
                pasteAreaThread.Start();

                ClearCopiedAreaBtn_Click(this, e);
            }
            else
            {
                ToolStripItem item = (sender as ToolStripItem);
                if (item != null)
                {
                    if (item.Owner is ContextMenuStrip owner)
                    {
                        var btn = (FloorSlot)owner.SourceControl;

                        try
                        {

                            if (anchorX < 0 || anchorY < 0)
                            {
                                return;
                            }

                            long address;

                            if (layer1Btn.Checked)
                            {
                                address = GetAddress(btn.MapX, btn.MapY);
                            }
                            else if (layer2Btn.Checked)
                            {
                                address = (GetAddress(btn.MapX, btn.MapY) + Utilities.mapSize);
                            }
                            else
                                return;

                            DisableBtn();

                            btnToolTip.RemoveAll();

                            string itemID = Utilities.PrecedingZeros(btn.ItemID.ToString("X"), 4);
                            string itemData = Utilities.PrecedingZeros(btn.ItemData.ToString("X"), 8);
                            string flag0 = btn.Flag0;
                            string flag1 = "20";

                            Thread dropThread = new(delegate () { DropItem(address, itemID, itemData, flag0, flag1, btn); });
                            dropThread.Start();
                        }
                        catch (Exception ex)
                        {
                            MyLog.LogEvent("Map", "Flag20: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void Flag00ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AreaSet || AreaCopied)
            {
                if (Corner1X < 0 || Corner1Y < 0 || Corner2X < 0 || Corner2Y < 0 || Corner1X > 111 || Corner1Y > 95 || Corner2X > 111 || Corner2Y > 95)
                {
                    MyMessageBox.Show("Selected Area Out of Bounds!", "Please use your brain, My Master.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int TopLeftX;
                int TopLeftY;
                int BottomRightX;
                int BottomRightY;

                if (Corner1X <= Corner2X)
                {
                    if (Corner1Y <= Corner2Y) // Top Left
                    {
                        TopLeftX = Corner1X;
                        TopLeftY = Corner1Y;
                        BottomRightX = Corner2X;
                        BottomRightY = Corner2Y;
                    }
                    else // Bottom Left
                    {
                        TopLeftX = Corner1X;
                        TopLeftY = Corner2Y; //
                        BottomRightX = Corner2X;
                        BottomRightY = Corner1Y; //
                    }
                }
                else
                {
                    if (Corner1Y <= Corner2Y) // Top Right
                    {
                        TopLeftX = Corner2X; //
                        TopLeftY = Corner1Y;
                        BottomRightX = Corner1X; //
                        BottomRightY = Corner2Y;
                    }
                    else // Bottom Left
                    {
                        TopLeftX = Corner2X;
                        TopLeftY = Corner2Y;
                        BottomRightX = Corner1X;
                        BottomRightY = Corner1Y;
                    }
                }

                int numberOfColumn = BottomRightX - TopLeftX + 1;
                int numberOfRow = BottomRightY - TopLeftY + 1;

                int sizeOfRow = 16;

                SavedArea = new byte[numberOfColumn * 2][];

                for (int i = 0; i < numberOfColumn * 2; i++)
                {
                    SavedArea[i] = new byte[numberOfRow * sizeOfRow];
                }

                for (int i = 0; i < numberOfColumn; i++)
                {
                    for (int j = 0; j < numberOfRow; j++)
                    {
                        if (layer1Btn.Checked)
                        {
                            Buffer.BlockCopy(Layer1, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer1, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)) + 0x600, SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                            SavedArea[i * 2][0x10 * j + 2] = 0x00;
                        }
                        else
                        {
                            Buffer.BlockCopy(Layer2, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer2, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)) + 0x600, SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                            SavedArea[i * 2][0x10 * j + 2] = 0x00;
                        }
                    }
                }

                long address;

                if (layer1Btn.Checked)
                {
                    address = Utilities.mapZero;
                }
                else if (layer2Btn.Checked)
                {
                    address = Utilities.mapZero + Utilities.mapSize;
                }
                else
                    return;

                DisableBtn();

                Thread pasteAreaThread = new(delegate () { PasteArea(TopLeftX, TopLeftY, numberOfColumn); });
                pasteAreaThread.Start();

                ClearCopiedAreaBtn_Click(this, e);
            }
            else
            {
                ToolStripItem item = (sender as ToolStripItem);
                if (item != null)
                {
                    if (item.Owner is ContextMenuStrip owner)
                    {
                        var btn = (FloorSlot)owner.SourceControl;

                        try
                        {

                            if (anchorX < 0 || anchorY < 0)
                            {
                                return;
                            }

                            long address;

                            if (layer1Btn.Checked)
                            {
                                address = GetAddress(btn.MapX, btn.MapY);
                            }
                            else if (layer2Btn.Checked)
                            {
                                address = (GetAddress(btn.MapX, btn.MapY) + Utilities.mapSize);
                            }
                            else
                                return;

                            DisableBtn();

                            btnToolTip.RemoveAll();

                            string itemID = Utilities.PrecedingZeros(btn.ItemID.ToString("X"), 4);
                            string itemData = Utilities.PrecedingZeros(btn.ItemData.ToString("X"), 8);
                            string flag0 = btn.Flag0;
                            string flag1 = "00";

                            Thread dropThread = new(delegate () { DropItem(address, itemID, itemData, flag0, flag1, btn); });
                            dropThread.Start();
                        }
                        catch (Exception ex)
                        {
                            MyLog.LogEvent("Map", "Flag00: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void Flag00To04ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AreaSet || AreaCopied)
            {
                if (Corner1X < 0 || Corner1Y < 0 || Corner2X < 0 || Corner2Y < 0 || Corner1X > 111 || Corner1Y > 95 || Corner2X > 111 || Corner2Y > 95)
                {
                    MyMessageBox.Show("Selected Area Out of Bounds!", "Please use your brain, My Master.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int TopLeftX;
                int TopLeftY;
                int BottomRightX;
                int BottomRightY;

                if (Corner1X <= Corner2X)
                {
                    if (Corner1Y <= Corner2Y) // Top Left
                    {
                        TopLeftX = Corner1X;
                        TopLeftY = Corner1Y;
                        BottomRightX = Corner2X;
                        BottomRightY = Corner2Y;
                    }
                    else // Bottom Left
                    {
                        TopLeftX = Corner1X;
                        TopLeftY = Corner2Y; //
                        BottomRightX = Corner2X;
                        BottomRightY = Corner1Y; //
                    }
                }
                else
                {
                    if (Corner1Y <= Corner2Y) // Top Right
                    {
                        TopLeftX = Corner2X; //
                        TopLeftY = Corner1Y;
                        BottomRightX = Corner1X; //
                        BottomRightY = Corner2Y;
                    }
                    else // Bottom Left
                    {
                        TopLeftX = Corner2X;
                        TopLeftY = Corner2Y;
                        BottomRightX = Corner1X;
                        BottomRightY = Corner1Y;
                    }
                }

                int numberOfColumn = BottomRightX - TopLeftX + 1;
                int numberOfRow = BottomRightY - TopLeftY + 1;

                int sizeOfRow = 16;

                SavedArea = new byte[numberOfColumn * 2][];

                for (int i = 0; i < numberOfColumn * 2; i++)
                {
                    SavedArea[i] = new byte[numberOfRow * sizeOfRow];
                }

                for (int i = 0; i < numberOfColumn; i++)
                {
                    for (int j = 0; j < numberOfRow; j++)
                    {
                        if (layer1Btn.Checked)
                        {
                            Buffer.BlockCopy(Layer1, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer1, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)) + 0x600, SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                            if (SavedArea[i * 2][0x10 * j] != 0xFE || SavedArea[i * 2][0x10 * j + 1] != 0xFF)
                            {
                                if (SavedArea[i * 2][0x10 * j + 2] == 0x00)
                                    SavedArea[i * 2][0x10 * j + 2] = 0x04;
                            }
                        }
                        else
                        {
                            Buffer.BlockCopy(Layer2, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer2, (0xC00 * (i + TopLeftX + 16)) + (0x10 * (j + TopLeftY)) + 0x600, SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                            if (SavedArea[i * 2][0x10 * j] != 0xFE || SavedArea[i * 2][0x10 * j + 1] != 0xFF)
                            {
                                if (SavedArea[i * 2][0x10 * j + 2] == 0x00)
                                    SavedArea[i * 2][0x10 * j + 2] = 0x04;
                            }
                        }
                    }
                }

                long address;

                if (layer1Btn.Checked)
                {
                    address = Utilities.mapZero;
                }
                else if (layer2Btn.Checked)
                {
                    address = Utilities.mapZero + Utilities.mapSize;
                }
                else
                    return;

                DisableBtn();

                Thread pasteAreaThread = new(delegate () { PasteArea(TopLeftX, TopLeftY, numberOfColumn); });
                pasteAreaThread.Start();

                ClearCopiedAreaBtn_Click(this, e);
            }
            else
            {
                ToolStripItem item = (sender as ToolStripItem);
                if (item != null)
                {
                    if (item.Owner is ContextMenuStrip owner)
                    {
                        var btn = (FloorSlot)owner.SourceControl;

                        if (btn.Flag1 != "00")
                            return;

                        try
                        {

                            if (anchorX < 0 || anchorY < 0)
                            {
                                return;
                            }

                            long address;

                            if (layer1Btn.Checked)
                            {
                                address = GetAddress(btn.MapX, btn.MapY);
                            }
                            else if (layer2Btn.Checked)
                            {
                                address = (GetAddress(btn.MapX, btn.MapY) + Utilities.mapSize);
                            }
                            else
                                return;

                            DisableBtn();

                            btnToolTip.RemoveAll();

                            string itemID = Utilities.PrecedingZeros(btn.ItemID.ToString("X"), 4);
                            string itemData = Utilities.PrecedingZeros(btn.ItemData.ToString("X"), 8);
                            string flag0 = btn.Flag0;
                            string flag1 = "04";

                            Thread dropThread = new(delegate () { DropItem(address, itemID, itemData, flag0, flag1, btn); });
                            dropThread.Start();
                        }
                        catch (Exception ex)
                        {
                            MyLog.LogEvent("Map", "Flag04: " + ex.Message);
                        }
                    }
                }
            }
        }
        #endregion

        #region Variation Spawn
        private void PlaceVariationBtn_Click(object sender, EventArgs e)
        {
            if (IdTextbox.Text == "" || HexTextbox.Text == "" || FlagTextbox.Text == "")
            {
                MessageBox.Show(@"Please select an item!");
                return;
            }

            if (selectedButton == null)
            {
                MessageBox.Show(@"Please select a slot!");
                return;
            }

            string flag0 = selectedItem.GetFlag0();
            string flag1 = Utilities.PrecedingZeros(FlagTextbox.Text, 2);

            variationList = Variation.GetVariationList(IdTextbox.Text, flag0, flag1, HexTextbox.Text);
            byte[][] spawnArea;


            if (variationList != null)
            {

                int TopLeftX = selectedButton.MapX;
                int TopLeftY = selectedButton.MapY;
                int row;
                int column;

                int main = variationList.GetLength(0);
                int sub = variationList.GetLength(1);

                VariationSpawn variationSpawner = new(variationList, Layer1, Acre, Building, Terrain, MapCustomDesgin, TopLeftX, TopLeftY, flag1, selectedSize);
                variationSpawner.SendObeySizeEvent += VariationSpawner_SendObeySizeEvent;
                variationSpawner.SendRowAndColumnEvent += VariationSpawner_SendRowAndColumnEvent;

                int result = (int)variationSpawner.ShowDialog(this);

                if (result == 1) // Main
                {
                    row = variationList.GetLength(0);
                    if (obeySize)
                    {
                        spawnArea = BuildVariationAreaObeySize(variationList, row, numOfColumn, 1);
                    }
                    else
                    {
                        spawnArea = BuildVariationArea(variationList, row, numOfColumn, 1);
                    }
                }
                else if (result == 6) // Sub
                {
                    row = variationList.GetLength(1);
                    if (obeySize)
                    {
                        spawnArea = BuildVariationAreaObeySize(variationList, row, numOfColumn, 6);
                    }
                    else
                    {
                        spawnArea = BuildVariationArea(variationList, row, numOfColumn, 6);
                    }
                }
                else if (result == 5) // All
                {
                    row = sub;
                    column = main;
                    if (obeySize)
                    {
                        spawnArea = BuildVariationAreaObeySize(variationList, row, column, 5);
                    }
                    else
                    {
                        spawnArea = BuildVariationArea(variationList, row, column, 5);
                    }
                }
                else if (result == 3) // Main H
                {
                    column = variationList.GetLength(0);
                    if (obeySize)
                    {
                        spawnArea = BuildVertVariationAreaObeySize(variationList, column, numOfRow, 3);
                    }
                    else
                    {
                        spawnArea = BuildVertVariationArea(variationList, column, numOfRow, 3);
                    }
                }
                else if (result == 7) // Sub H
                {
                    column = variationList.GetLength(1);
                    if (obeySize)
                    {
                        spawnArea = BuildVertVariationAreaObeySize(variationList, column, numOfRow, 7);
                    }
                    else
                    {
                        spawnArea = BuildVertVariationArea(variationList, column, numOfRow, 7);
                    }
                }
                else if (result == 4) // All H
                {
                    row = main;
                    column = sub;
                    if (obeySize)
                    {
                        spawnArea = BuildVertVariationAreaObeySize(variationList, column, row, 4);
                    }
                    else
                    {
                        spawnArea = BuildVertVariationArea(variationList, column, row, 4);
                    }
                }
                else
                {
                    return;
                }

                long address;

                if (layer1Btn.Checked)
                {
                    address = Utilities.mapZero;
                }
                else if (layer2Btn.Checked)
                {
                    address = Utilities.mapZero + Utilities.mapSize;
                }
                else
                    return;

                DisableBtn();

                btnToolTip.RemoveAll();

                Thread SpawnThread = new(delegate () { AreaSpawnThread(address, spawnArea, TopLeftX, TopLeftY); });
                SpawnThread.Start();
            }
            else
            {
                MyMessageBox.Show("No variation found for the selected item!", "Error 404", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void VariationSpawner_SendRowAndColumnEvent(int row, int column)
        {
            numOfColumn = column;
            numOfRow = row;
        }

        private void VariationSpawner_SendObeySizeEvent(bool toggle, int ItemHeight = 0, int ItemWidth = 0, int NewSpawnHeight = 0, int NewSpawnWidth = 0, bool Wallmount = false, bool Ceiling = false)
        {
            obeySize = toggle;
            itemHeight = ItemHeight;
            itemWidth = ItemWidth;
            newSpawnHeight = NewSpawnHeight;
            newSpawnWidth = NewSpawnWidth;
            wallmount = Wallmount;
            ceiling = Ceiling;
        }

        private static byte[][] BuildVariationArea(InventorySlot[,] variation, int numberOfRow, int multiple, int mode)
        {
            int numberOfColumn = multiple;
            int sizeOfRow = 16;

            byte[][] b = new byte[numberOfColumn * 2][];

            for (int i = 0; i < numberOfColumn * 2; i++)
            {
                b[i] = new byte[numberOfRow * sizeOfRow];
            }

            if (mode == 1) // Main
            {
                for (int i = 0; i < numberOfColumn; i++)
                {
                    for (int j = 0; j < numberOfRow; j++)
                    {
                        string itemID = Utilities.PrecedingZeros(variation[j, 0].FillItemID(), 4);
                        string itemData = Utilities.PrecedingZeros(variation[j, 0].FillItemData(), 8);
                        string flag0 = Utilities.PrecedingZeros(variation[j, 0].GetFlag0(), 2);
                        string flag1 = Utilities.PrecedingZeros(variation[j, 0].GetFlag1(), 2);

                        byte[] ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                        byte[] ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

                        Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }
            else if (mode == 6) // Sub
            {
                for (int i = 0; i < numberOfColumn; i++)
                {
                    for (int j = 0; j < numberOfRow; j++)
                    {
                        string itemID = Utilities.PrecedingZeros(variation[0, j].FillItemID(), 4);
                        string itemData = Utilities.PrecedingZeros(variation[0, j].FillItemData(), 8);
                        string flag0 = Utilities.PrecedingZeros(variation[0, j].GetFlag0(), 2);
                        string flag1 = Utilities.PrecedingZeros(variation[0, j].GetFlag1(), 2);

                        byte[] ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                        byte[] ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

                        Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }
            else // All
            {
                for (int i = 0; i < numberOfColumn; i++)
                {
                    for (int j = 0; j < numberOfRow; j++)
                    {
                        string itemID = Utilities.PrecedingZeros(variation[i, j].FillItemID(), 4);
                        string itemData = Utilities.PrecedingZeros(variation[i, j].FillItemData(), 8);
                        string flag0 = Utilities.PrecedingZeros(variation[i, j].GetFlag0(), 2);
                        string flag1 = Utilities.PrecedingZeros(variation[i, j].GetFlag1(), 2);

                        byte[] ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                        byte[] ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

                        Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }

            return b;
        }

        private byte[][] BuildVariationAreaObeySize(InventorySlot[,] variation, int oldRow, int oldColumn, int mode)
        {
            int sizeOfRow = 16;

            byte[][] b = new byte[newSpawnWidth * 2][];

            for (int i = 0; i < newSpawnWidth * 2; i++)
            {
                b[i] = new byte[newSpawnHeight * sizeOfRow];
            }

            int iterator = 0;
            InventorySlot[] serialList = new InventorySlot[oldRow * oldColumn];

            if (mode == 1) // Main
            {
                for (int i = 0; i < oldColumn; i++)
                {
                    for (int j = 0; j < oldRow; j++)
                    {
                        serialList[iterator] = variation[j, 0];
                        iterator++;
                    }
                }
            }
            else if (mode == 6) // Sub
            {
                for (int i = 0; i < oldColumn; i++)
                {
                    for (int j = 0; j < oldRow; j++)
                    {
                        serialList[iterator] = variation[0, j];
                        iterator++;
                    }
                }
            }
            else if (mode == 5) // All
            {
                for (int i = 0; i < oldColumn; i++)
                {
                    for (int j = 0; j < oldRow; j++)
                    {
                        serialList[iterator] = variation[i, j];
                        iterator++;
                    }
                }
            }

            iterator = 0;
            byte[] ItemLeft;
            byte[] ItemRight;

            string flag = Utilities.PrecedingZeros(FlagTextbox.Text, 2);

            for (int i = 0; i < newSpawnWidth; i++)
            {
                for (int j = 0; j < newSpawnHeight; j++)
                {
                    if (i % itemWidth == 0 && j % itemHeight == 0)
                    {
                        if (wallmount && flag != "20")
                        {
                            string itemID = "1618";
                            string itemData = Utilities.TranslateVariationValue(serialList[iterator].FillItemData()) + Utilities.PrecedingZeros(serialList[iterator].FillItemID(), 4);
                            string flag0 = Utilities.PrecedingZeros(serialList[iterator].GetFlag0(), 2);
                            string flag1 = Utilities.PrecedingZeros(serialList[iterator].GetFlag1(), 2);

                            ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                            ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));
                            iterator++;
                        }
                        else if (ceiling && flag != "20")
                        {
                            string itemID = "342F";
                            string itemData = Utilities.TranslateVariationValue(serialList[iterator].FillItemData()) + Utilities.PrecedingZeros(serialList[iterator].FillItemID(), 4);
                            string flag0 = Utilities.PrecedingZeros(serialList[iterator].GetFlag0(), 2);
                            string flag1 = Utilities.PrecedingZeros(serialList[iterator].GetFlag1(), 2);

                            ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                            ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));
                            iterator++;
                        }
                        else
                        {
                            string itemID = Utilities.PrecedingZeros(serialList[iterator].FillItemID(), 4);
                            string itemData = Utilities.PrecedingZeros(serialList[iterator].FillItemData(), 8);
                            string flag0 = Utilities.PrecedingZeros(serialList[iterator].GetFlag0(), 2);
                            string flag1 = Utilities.PrecedingZeros(serialList[iterator].GetFlag1(), 2);

                            ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                            ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));
                            iterator++;
                        }
                    }
                    else
                    {
                        ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft("FFFE", "00000000", "00", "00", true));
                        ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight("FFFE", true));
                    }

                    Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                    Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                }
            }

            return b;
        }

        private static byte[][] BuildVertVariationArea(InventorySlot[,] variation, int numberOfColumn, int multiple, int mode)
        {
            int numberOfRow = multiple;
            int sizeOfRow = 16;

            byte[][] b = new byte[numberOfColumn * 2][];

            for (int i = 0; i < numberOfColumn * 2; i++)
            {
                b[i] = new byte[numberOfRow * sizeOfRow];
            }

            if (mode == 3) // Main
            {
                for (int i = 0; i < numberOfColumn; i++)
                {
                    for (int j = 0; j < numberOfRow; j++)
                    {
                        string itemID = Utilities.PrecedingZeros(variation[i, 0].FillItemID(), 4);
                        string itemData = Utilities.PrecedingZeros(variation[i, 0].FillItemData(), 8);
                        string flag0 = Utilities.PrecedingZeros(variation[i, 0].GetFlag0(), 2);
                        string flag1 = Utilities.PrecedingZeros(variation[i, 0].GetFlag1(), 2);

                        byte[] ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                        byte[] ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

                        Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }
            else if (mode == 7) // Sub
            {
                for (int i = 0; i < numberOfColumn; i++)
                {
                    for (int j = 0; j < numberOfRow; j++)
                    {
                        string itemID = Utilities.PrecedingZeros(variation[0, i].FillItemID(), 4);
                        string itemData = Utilities.PrecedingZeros(variation[0, i].FillItemData(), 8);
                        string flag0 = Utilities.PrecedingZeros(variation[0, i].GetFlag0(), 2);
                        string flag1 = Utilities.PrecedingZeros(variation[0, i].GetFlag1(), 2);

                        byte[] ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                        byte[] ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

                        Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }
            else // All
            {
                for (int i = 0; i < numberOfColumn; i++)
                {
                    for (int j = 0; j < numberOfRow; j++)
                    {
                        string itemID = Utilities.PrecedingZeros(variation[j, i].FillItemID(), 4);
                        string itemData = Utilities.PrecedingZeros(variation[j, i].FillItemData(), 8);
                        string flag0 = Utilities.PrecedingZeros(variation[j, i].GetFlag0(), 2);
                        string flag1 = Utilities.PrecedingZeros(variation[j, i].GetFlag1(), 2);

                        byte[] ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                        byte[] ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));

                        Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }

            return b;
        }

        private byte[][] BuildVertVariationAreaObeySize(InventorySlot[,] variation, int oldRow, int oldColumn, int mode)
        {
            int sizeOfRow = 16;

            byte[][] b = new byte[newSpawnWidth * 2][];

            for (int i = 0; i < newSpawnWidth * 2; i++)
            {
                b[i] = new byte[newSpawnHeight * sizeOfRow];
            }

            int iterator = 0;
            InventorySlot[] serialList = new InventorySlot[oldRow * oldColumn];

            if (mode == 3) // Main
            {
                for (int i = 0; i < oldRow; i++)
                {
                    for (int j = 0; j < oldColumn; j++)
                    {
                        serialList[iterator] = variation[i, 0];
                        iterator++;
                    }
                }
            }
            else if (mode == 7) // Sub
            {
                for (int i = 0; i < oldRow; i++)
                {
                    for (int j = 0; j < oldColumn; j++)
                    {
                        serialList[iterator] = variation[0, i];
                        iterator++;
                    }
                }
            }
            else if (mode == 4) // All
            {
                for (int i = 0; i < oldRow; i++)
                {
                    for (int j = 0; j < oldColumn; j++)
                    {
                        serialList[iterator] = variation[j, i];
                        iterator++;
                    }
                }
            }

            iterator = 0;
            byte[] ItemLeft;
            byte[] ItemRight;

            string flag = Utilities.PrecedingZeros(FlagTextbox.Text, 2);

            for (int i = 0; i < newSpawnWidth; i++)
            {
                for (int j = 0; j < newSpawnHeight; j++)
                {
                    if (i % itemWidth == 0 && j % itemHeight == 0)
                    {
                        if (wallmount && flag != "20")
                        {
                            string itemID = "1618";
                            string itemData = Utilities.TranslateVariationValue(serialList[iterator].FillItemData()) + Utilities.PrecedingZeros(serialList[iterator].FillItemID(), 4);
                            string flag0 = Utilities.PrecedingZeros(serialList[iterator].GetFlag0(), 2);
                            string flag1 = Utilities.PrecedingZeros(serialList[iterator].GetFlag1(), 2);

                            ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                            ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));
                            iterator++;
                        }
                        else if (ceiling && flag != "20")
                        {
                            string itemID = "342F";
                            string itemData = Utilities.TranslateVariationValue(serialList[iterator].FillItemData()) + Utilities.PrecedingZeros(serialList[iterator].FillItemID(), 4);
                            string flag0 = Utilities.PrecedingZeros(serialList[iterator].GetFlag0(), 2);
                            string flag1 = Utilities.PrecedingZeros(serialList[iterator].GetFlag1(), 2);

                            ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                            ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));
                            iterator++;
                        }
                        else
                        {
                            string itemID = Utilities.PrecedingZeros(serialList[iterator].FillItemID(), 4);
                            string itemData = Utilities.PrecedingZeros(serialList[iterator].FillItemData(), 8);
                            string flag0 = Utilities.PrecedingZeros(serialList[iterator].GetFlag0(), 2);
                            string flag1 = Utilities.PrecedingZeros(serialList[iterator].GetFlag1(), 2);

                            ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft(itemID, itemData, flag0, flag1));
                            ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight(itemID));
                            iterator++;
                        }
                    }
                    else
                    {
                        ItemLeft = Utilities.StringToByte(Utilities.BuildDropStringLeft("FFFE", "00000000", "00", "00", true));
                        ItemRight = Utilities.StringToByte(Utilities.BuildDropStringRight("FFFE", true));
                    }

                    Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                    Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                }
            }

            return b;
        }
        #endregion

        #region Language
        private void LanguageSetup(string configLanguage)
        {
            Language.SelectedIndex = configLanguage switch
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

        private void Language_SelectedIndexChanged(object sender, EventArgs e)
        {
            itemSearchBox.Clear();

            languageSetting = Language.SelectedIndex switch
            {
                0 => "eng",
                1 => "jpn",
                2 => "tchi",
                3 => "schi",
                4 => "kor",
                5 => "fre",
                6 => "ger",
                7 => "spa",
                8 => "ita",
                9 => "dut",
                10 => "rus",
                _ => "eng",
            };
            if (FieldGridView.Columns.Contains(languageSetting))
            {
                HideAllLanguage();
                FieldGridView.Columns[languageSetting].Visible = true;
            }

            itemSearchBox.Text = "Search...";
        }

        private void HideAllLanguage()
        {
            if (FieldGridView.Columns.Contains("id"))
            {
                FieldGridView.Columns["eng"].Visible = false;
                FieldGridView.Columns["jpn"].Visible = false;
                FieldGridView.Columns["tchi"].Visible = false;
                FieldGridView.Columns["schi"].Visible = false;
                FieldGridView.Columns["kor"].Visible = false;
                FieldGridView.Columns["fre"].Visible = false;
                FieldGridView.Columns["ger"].Visible = false;
                FieldGridView.Columns["spa"].Visible = false;
                FieldGridView.Columns["ita"].Visible = false;
                FieldGridView.Columns["dut"].Visible = false;
                FieldGridView.Columns["rus"].Visible = false;
            }
        }
        #endregion

        #region Rainbow
        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            var rand = new Random();
            int CurrentR = fieldModeBtn.BackColor.R;
            int CurrentG = fieldModeBtn.BackColor.G;
            int CurrentB = fieldModeBtn.BackColor.B;
            int NewR;
            int NewG;
            int NewB;

            if (CurrentR == target[targetValue].R && CurrentG == target[targetValue].G && CurrentB == target[targetValue].B)
            {
                targetValue++;
                if (targetValue >= 7)
                    targetValue = 0;
            }
            else
            {
                if (CurrentR > target[targetValue].R)
                    NewR = CurrentR - rand.Next(1, 20);
                else if (CurrentR < target[targetValue].R)
                    NewR = CurrentR + rand.Next(1, 20);
                else
                    NewR = CurrentR;
                if (NewR < 0)
                    NewR = 0;
                if (NewR > 255)
                    NewR = 255;

                if (CurrentG > target[targetValue].G)
                    NewG = CurrentG - rand.Next(1, 20);
                else if (CurrentG < target[targetValue].G)
                    NewG = CurrentG + rand.Next(1, 20);
                else
                    NewG = CurrentG;
                if (NewG < 0)
                    NewG = 0;
                if (NewG > 255)
                    NewG = 255;

                if (CurrentB > target[targetValue].B)
                    NewB = CurrentB - rand.Next(1, 20);
                else if (CurrentB < target[targetValue].B)
                    NewB = CurrentB + rand.Next(1, 20);
                else
                    NewB = CurrentB;
                if (NewB < 0)
                    NewB = 0;
                if (NewB > 255)
                    NewB = 255;

                UpdateUI(() =>
                {
                    fieldModeBtn.BackColor = Color.FromArgb(NewR, NewG, NewB);
                });
            }
        }
        #endregion

        #region Mouse DoubleClick
        private void FlagTextbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.X <= FlagTextbox.Controls[1].Width + 1)
            {
                //Console.WriteLine("EditBox");
                if (FlagTextbox.Value == 0x0)
                    FlagTextbox.Value = 0x20;
                else if (FlagTextbox.Value == 0x20)
                    FlagTextbox.Value = 0x04;
                else
                    FlagTextbox.Value = 0x00;
            }
            /*
            else if (e.Y <= FlagTextbox.Controls[1].Height / 2)
                Console.WriteLine("UpArrow");
            else if (e.X >= FlagTextbox.Controls[0].Width + FlagTextbox.Controls[0].Left)
                Console.WriteLine("Right border");
            else
                Console.WriteLine("DownArrow");
            */
        }

        private void HexTextbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.X <= HexTextbox.Controls[1].Width + 1)
            {
                if (currentDataTable == recipeSource || currentDataTable == flowerSource)
                    return;

                if (IdTextbox.Text == "")
                    return;

                string id = Utilities.PrecedingZeros(IdTextbox.Text, 4);

                UInt16 IntId = Convert.ToUInt16("0x" + IdTextbox.Text, 16);

                if (Utilities.itemkind.ContainsKey(id))
                {
                    if (!Utilities.CountByKind.TryGetValue(Utilities.itemkind[id], out int value))
                        HexTextbox.Text = "00000000";
                    else
                    {
                        if (ItemAttr.HasFenceWithVariation(IntId))  // Fence Variation
                        {
                            string hexValue = Utilities.PrecedingZeros(HexTextbox.Text, 8);


                            string front = Utilities.PrecedingZeros(hexValue, 8).Substring(0, 4);
                            //string back = Utilities.precedingZeros(hexValue, 8).Substring(4, 4);

                            int decValue = value - 1;
                            if (decValue >= 0)
                                HexTextbox.Text = front + Utilities.PrecedingZeros(decValue.ToString("X"), 4);
                            else
                                HexTextbox.Text = front + Utilities.PrecedingZeros("0", 4);
                        }
                        else
                        {
                            int decValue = value - 1;
                            if (decValue >= 0)
                                HexTextbox.Text = Utilities.PrecedingZeros(decValue.ToString("X"), 8);
                            else
                                HexTextbox.Text = Utilities.PrecedingZeros("0", 8);
                        }
                    }
                }
                else
                {
                    HexTextbox.Text = "00000000";
                }

                UpdateVariation();
            }
        }

        private void NextAutoSaveSecond_DoubleClick(object sender, EventArgs e)
        {
            if (!ignore)
            {
                ignore = true;
                nextAutoSaveSecond.ForeColor = Color.Red;
                NextSaveTimer.Stop();
                MyLog.LogEvent("Map", "Autosave Ignored");
            }
            else
            {
                ignore = false;
                nextAutoSaveSecond.ForeColor = Color.White;
                NextSaveTimer.Start();
                MyLog.LogEvent("Map", "Autosave Protection Activated");
            }
        }

        private bool IgnoreAutosave()
        {
            DialogResult result = MyMessageBox.Show("Something seems to be wrong with the autosave detection.\n" +
                                            "Would you like to ignore the autosave protection and spawn the item(s) anyway?\n\n" +
                                            "Please be noted that spawning item during autosave might crash the game."
                                            , "Waiting for autosave to complete...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                UpdateUI(() =>
                {
                    NextSaveTimer.Stop();
                    ignore = true;
                    nextAutoSaveSecond.ForeColor = Color.Red;
                });
                return true;
            }
            else
                return false;
        }
        #endregion

        #region Activate Item
        private static void BuildActivateTable(byte[] ActivateLayer, ref bool[,] ActivateTable)
        {
            int width = Utilities.ExtendedMapNumOfColumn;
            int height = Utilities.ExtendedMapNumOfRow * 2;

            ActivateTable = new bool[width, height];

            for (int i = 0; i < ActivateLayer.Length; i++)
            {
                var left = (ActivateLayer[i] & 0x0F);
                var right = (ActivateLayer[i] & 0xF0) >> 4;

                CheckActivate(left, i % (width / 4) * 4, i % (width / 4) * 4 + 1, i / (width / 4), ref ActivateTable);
                CheckActivate(right, i % (width / 4) * 4 + 2, i % (width / 4) * 4 + 3, i / (width / 4), ref ActivateTable);
            }
        }

        private static void CheckActivate(int value, int left, int right, int y, ref bool[,] ActivateTable)
        {
            if (value == 0 || value == 2 || value == 8 || value == 0xA)
            {
                ActivateTable[left, y] = false;
                ActivateTable[right, y] = false;
            }
            else if (value == 1 || value == 3 || value == 9 || value == 0xB)
            {
                ActivateTable[left, y] = true;
                ActivateTable[right, y] = false;
            }
            else if (value == 4 || value == 6 || value == 0xC || value == 0xE)
            {
                ActivateTable[left, y] = false;
                ActivateTable[right, y] = true;
            }
            else if (value == 5 || value == 7 || value == 0xD || value == 0xF)
            {
                ActivateTable[left, y] = true;
                ActivateTable[right, y] = true;
            }
        }

        private static string DisplayActivate(int x, int y, bool[,] ActivateTable)
        {
            if (ActivateTable == null)
                return "";

            string top;
            string bottom;

            if (ActivateTable[x, y * 2])
                top = "✓ True";
            else
                top = "✘ False";

            if (ActivateTable[x, y * 2 + 1])
                bottom = "✓ True";
            else
                bottom = "✘ False";

            return top + " " + bottom;
        }

        private static bool IsActivate(int x, int y, bool[,] ActivateTable)
        {
            if (ActivateTable == null)
                return false;
            else
                return ActivateTable[x, y * 2]; // || ActivateTable[x, y * 2 + 1];
        }

        private void SetActivate(int x, int y, ref byte[] ActivateLayer, ref bool[,] ActivateTable)
        {
            int offset = (x / 4) + (y * 2 * 28);
            var b = ActivateLayer[offset];

            byte upper = (byte)(b & 0xF0);
            byte lower = (byte)(b & 0x0F);

            byte newValue = 0x0;

            if (x % 4 == 0)
            {
                if (IsActivate(x + 1, y, ActivateTable))
                {
                    newValue = (byte)(upper + 0xF);
                }
                else
                {
                    newValue = (byte)(upper + 0x3);
                }
            }
            else if (x % 4 == 1)
            {
                if (IsActivate(x - 1, y, ActivateTable))
                {
                    newValue = (byte)(upper + 0xF);
                }
                else
                {
                    newValue = (byte)(upper + 0xC);
                }
            }
            else if (x % 4 == 2)
            {
                if (IsActivate(x + 1, y, ActivateTable))
                {
                    newValue = (byte)(0xF0 + lower);
                }
                else
                {
                    newValue = (byte)(0x30 + lower);
                }
            }
            else if (x % 4 == 3)
            {
                if (IsActivate(x - 1, y, ActivateTable))
                {
                    newValue = (byte)(0xF0 + lower);
                }
                else
                {
                    newValue = (byte)(0xC0 + lower);
                }
            }

            DisableBtn();

            btnToolTip.RemoveAll();

            long Address1;
            long Address2;

            if (layer1Btn.Checked)
            {
                Address1 = Utilities.mapActivate + offset;
                Address2 = Utilities.mapActivate + Utilities.SaveFileBuffer + offset;
            }
            else
            {
                Address1 = Utilities.mapActivate + Utilities.mapActivateSize + offset;
                Address2 = Utilities.mapActivate + Utilities.mapActivateSize + Utilities.SaveFileBuffer + offset;
            }

            Thread toggleThread = new(delegate () { ToggleItem(Address1, Address2, newValue); });
            toggleThread.Start();

            ActivateTable[x, y * 2] = true;
            ActivateTable[x, y * 2 + 1] = true;
            ActivateLayer[offset] = newValue;
            ActivateLayer[offset + 0x1C] = newValue;
        }

        private void SetDeactivate(int x, int y, ref byte[] ActivateLayer, ref bool[,] ActivateTable)
        {
            int offset = (x / 4) + (y * 2 * 28);
            var b = ActivateLayer[offset];

            byte upper = (byte)(b & 0xF0);
            byte lower = (byte)(b & 0x0F);

            byte newValue = 0x0;

            if (x % 4 == 0)
            {
                if (IsActivate(x + 1, y, ActivateTable))
                {
                    newValue = (byte)(upper + 0xC); // c
                }
                else
                {
                    newValue = upper; // 0
                }
            }
            else if (x % 4 == 1)
            {
                if (IsActivate(x - 1, y, ActivateTable))
                {
                    newValue = (byte)(upper + 0x3); // 3
                }
                else
                {
                    newValue = upper; // 0
                }
            }
            else if (x % 4 == 2)
            {
                if (IsActivate(x + 1, y, ActivateTable))
                {
                    newValue = (byte)(0xC0 + lower); // c
                }
                else
                {
                    newValue = lower; // 0
                }
            }
            else if (x % 4 == 3)
            {
                if (IsActivate(x - 1, y, ActivateTable))
                {
                    newValue = (byte)(0x30 + lower); // 3
                }
                else
                {
                    newValue = lower; // 0
                }
            }

            /*
            int c = 0;

            while (Utilities.IsAboutToSave(s, usb, 2, saveTime, ignore))
            {
                if (c > 10)
                {
                    if (IgnoreAutosave())
                    {
                        break;
                    }
                    else
                    {
                        if (sound)
                            System.Media.SystemSounds.Asterisk.Play();
                        return;
                    }
                }
                c++;
                Thread.Sleep(3000);
            }
            */

            DisableBtn();

            btnToolTip.RemoveAll();

            long Address1;
            long Address2;

            if (layer1Btn.Checked)
            {
                Address1 = Utilities.mapActivate + offset;
                Address2 = Utilities.mapActivate + Utilities.SaveFileBuffer + offset;
            }
            else
            {
                Address1 = Utilities.mapActivate + Utilities.mapActivateSize + offset;
                Address2 = Utilities.mapActivate + Utilities.mapActivateSize + Utilities.SaveFileBuffer + offset;
            }

            Thread toggleThread = new(delegate () { ToggleItem(Address1, Address2, newValue); });
            toggleThread.Start();

            ActivateTable[x, y * 2] = false;
            ActivateTable[x, y * 2 + 1] = false;
            ActivateLayer[offset] = newValue;
            ActivateLayer[offset + 0x1C] = newValue;
        }

        private void ToggleItem(long Address1, long Address2, byte value)
        {
            ShowMapWait(2, "Toggling Item...");

            /*
            int c = 0;

            while (Utilities.IsAboutToSave(s, usb, 2, saveTime, ignore))
            {
                if (c > 10)
                {
                    if (IgnoreAutosave())
                    {
                        break;
                    }
                    else
                    {
                        UpdateUI(() =>
                        {
                            EnableBtn();
                        });

                        if (sound)
                            System.Media.SystemSounds.Asterisk.Play();

                        HideMapWait();
                        return;
                    }
                }
                c++;
                Thread.Sleep(3000);
            }
            */

            Utilities.PokeAddress(s, usb, Address1.ToString("X"), value.ToString("X"));
            Utilities.PokeAddress(s, usb, (Address1 + 0x1C).ToString("X"), value.ToString("X"));
            Utilities.PokeAddress(s, usb, Address2.ToString("X"), value.ToString("X"));
            Utilities.PokeAddress(s, usb, (Address2 + 0x1C).ToString("X"), value.ToString("X"));

            UpdateUI(() =>
            {
                EnableBtn();
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();
        }

        #endregion

        #region Item Filter
        private void FilterBtn_Click(object sender, EventArgs e)
        {
            if (itemFilter == null)
            {
                itemFilter = new Filter();
                itemFilter.ApplyFilter += ItemFilter_applyFilter;
                itemFilter.CloseFilter += ItemFilter_closeFilter;
                itemFilter.Show(this);
                itemFilter.Location = new Point(Location.X + Width, Location.Y);
            }
        }

        private void ItemFilter_closeFilter()
        {
            itemFilter = null;
        }

        private void ItemFilter_applyFilter(string itemkind)
        {
            if (itemkind.Equals("Clear"))
            {
                ResetFilter();
            }
            else
            {
                filterOn = true;
                FilterBtn.BackColor = Color.Orange;
                filterKind = itemkind;
            }

            ItemSearchBox_TextChanged(null, null);
        }

        private void ResetFilter()
        {
            filterOn = false;
            FilterBtn.BackColor = Color.FromArgb(114, 137, 218);
            filterKind = "";

            itemFilter?.Close();

            itemFilter = null;

            ((DataTable)FieldGridView.DataSource).DefaultView.RowFilter = null;
        }
        #endregion

        #region Bulk Selector
        private void AddToListBtn_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (bulkList == null)
                {
                    bulkList = new BulkList(sound);
                    bulkList.CloseForm += BulkList_CloseForm;
                    bulkList.Show();
                }

                if (IdTextbox.Text.Equals(String.Empty))
                    return;

                if (ModifierKeys == Keys.Shift)
                {
                    if (selectedItem.FillItemID().Equals("16A2"))
                        AddAllRecipes();
                    else
                        AddAllVariation(selectedItem.FillItemID(), selectedItem.DisplayItemName());
                }
                else
                {
                    if (selectedItem.FillItemID().Equals("16A2"))
                        bulkList.ReceiveItem(Utilities.PrecedingZeros(selectedItem.FillItemID(), 4), Utilities.PrecedingZeros(selectedItem.FillItemData(), 8), "[DIY] " + selectedItem.DisplayItemName(), selectedItem.GetPath());
                    else
                        bulkList.ReceiveItem(Utilities.PrecedingZeros(selectedItem.FillItemID(), 4), Utilities.PrecedingZeros(selectedItem.FillItemData(), 8), selectedItem.DisplayItemName(), selectedItem.GetPath());

                    bulkList.ScrollUpdate();
                }
            }
        }

        private void AddAllRecipes()
        {
            if (recipeSource == null)
                return;

            for (int i = 0; i < recipeSource.Rows.Count; i++)
            {
                bulkList.ReceiveItem("16A2", Utilities.PrecedingZeros(recipeSource.Rows[i]["id"].ToString(), 8), "[DIY] " + recipeSource.Rows[i][languageSetting], GetImagePathFromID(recipeSource.Rows[i]["id"].ToString(), recipeSource));
            }
        }

        private void AddAllVariation(string itemID, string name, int mode = 0)
        {
            variationList = Variation.GetVariationList(Utilities.PrecedingZeros(itemID, 4), "00", "00", "00000000", languageSetting);

            if (variationList == null)
            {
                bulkList.ReceiveItem(Utilities.PrecedingZeros(itemID, 4), "00000000", name, GetImagePathFromID(itemID, source));
            }
            else
            {
                int main = variationList.GetLength(0);
                int sub = variationList.GetLength(1);

                if (mode == 0)
                {
                    for (int j = 0; j < main; j++)
                    {
                        for (int k = 0; k < sub; k++)
                        {
                            bulkList.ReceiveItem(Utilities.PrecedingZeros(variationList[j, k].FillItemID(), 4), Utilities.PrecedingZeros(variationList[j, k].FillItemData(), 8), variationList[j, k].DisplayItemName(), variationList[j, k].GetPath());
                        }
                    }
                }
                else if (mode == 1) //Main
                {
                    for (int j = 0; j < main; j++)
                    {
                        bulkList.ReceiveItem(Utilities.PrecedingZeros(variationList[j, 0].FillItemID(), 4), Utilities.PrecedingZeros(variationList[j, 0].FillItemData(), 8), variationList[j, 0].DisplayItemName(), variationList[j, 0].GetPath());
                    }
                }
                else //Sub
                {
                    for (int k = 0; k < sub; k++)
                    {
                        bulkList.ReceiveItem(Utilities.PrecedingZeros(variationList[0, k].FillItemID(), 4), Utilities.PrecedingZeros(variationList[0, k].FillItemData(), 8), variationList[0, k].DisplayItemName(), variationList[0, k].GetPath());
                    }
                }
            }

            bulkList.ScrollUpdate();
        }



        private void BulkList_CloseForm()
        {
            bulkList = null;
        }

        private void EnableMultiSelectOption_Click(object sender, EventArgs e)
        {
            FieldGridView.MultiSelect = true;
            FieldGridView.DefaultCellStyle.SelectionBackColor = Color.Orange;
            FieldGridView.ContextMenuStrip = AddToBulkMenu;
        }

        private void AddToBulkOption_Click(object sender, EventArgs e)
        {
            if (FieldGridView.SelectedRows.Count <= 0)
                return;

            if (bulkList == null)
            {
                bulkList = new BulkList(sound);
                bulkList.CloseForm += BulkList_CloseForm;
                bulkList.Show();
            }


            for (int i = FieldGridView.SelectedRows.Count - 1; i >= 0; i--)
            {
                string id = FieldGridView.SelectedRows[i].Cells["ID"].Value.ToString();
                string name = FieldGridView.SelectedRows[i].Cells[languageSetting].Value.ToString();
                bulkList.ReceiveItem(Utilities.PrecedingZeros(id, 4), "00000000", name, GetImagePathFromID(id, source));
            }

            bulkList.ScrollUpdate();
        }

        private void AddALLToBulkOption_Click(object sender, EventArgs e)
        {
            if (FieldGridView.SelectedRows.Count <= 0)
                return;

            if (bulkList == null)
            {
                bulkList = new BulkList(sound);
                bulkList.CloseForm += BulkList_CloseForm;
                bulkList.Show();
            }


            for (int i = FieldGridView.SelectedRows.Count - 1; i >= 0; i--)
            {
                string id = FieldGridView.SelectedRows[i].Cells["ID"].Value.ToString();
                string name = FieldGridView.SelectedRows[i].Cells[languageSetting].Value.ToString();
                AddAllVariation(id, name, 0);
            }

            bulkList.ScrollUpdate();
        }

        private void AddMainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FieldGridView.SelectedRows.Count <= 0)
                return;

            if (bulkList == null)
            {
                bulkList = new BulkList(sound);
                bulkList.CloseForm += BulkList_CloseForm;
                bulkList.Show();
            }


            for (int i = FieldGridView.SelectedRows.Count - 1; i >= 0; i--)
            {
                string id = FieldGridView.SelectedRows[i].Cells["ID"].Value.ToString();
                string name = FieldGridView.SelectedRows[i].Cells[languageSetting].Value.ToString();
                AddAllVariation(id, name, 1);
            }

            bulkList.ScrollUpdate();
        }

        private void AddSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FieldGridView.SelectedRows.Count <= 0)
                return;

            if (bulkList == null)
            {
                bulkList = new BulkList(sound);
                bulkList.CloseForm += BulkList_CloseForm;
                bulkList.Show();
            }


            for (int i = FieldGridView.SelectedRows.Count - 1; i >= 0; i--)
            {
                string id = FieldGridView.SelectedRows[i].Cells["ID"].Value.ToString();
                string name = FieldGridView.SelectedRows[i].Cells[languageSetting].Value.ToString();
                AddAllVariation(id, name, 2);
            }

            bulkList.ScrollUpdate();
        }

        private void DisableMultiSelectOption_Click(object sender, EventArgs e)
        {
            FieldGridView.MultiSelect = false;
            FieldGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);
            FieldGridView.ClearSelection();
            FieldGridView.ContextMenuStrip = null;
        }
        #endregion

    }
}