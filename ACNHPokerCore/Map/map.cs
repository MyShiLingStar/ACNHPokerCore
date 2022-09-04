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
        private FloorSlot selectedButton = null;
        private string selectedSize;
        private readonly FloorSlot[] floorSlots;
        private variation selection = null;
        private MiniMap MiniMap = null;
        public BulkSpawn bulk = null;
        private int counter = 0;
        private int saveTime = -1;
        private bool drawing = false;

        private DataGridViewRow lastRow = null;
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
        private bool AreaSet = false;
        private readonly ToolStripMenuItem CopyAreaMenu;
        private bool AreaCopied = false;
        private readonly ToolStripMenuItem PasteAreaMenu;
        private readonly ToolStripMenuItem SaveAreaMenu;
        private byte[][] SavedArea;


        private DataTable currentDataTable;
        private readonly bool sound;
        private bool ignore = false;
        private bool keepProtection = false;
        private int keepProtectionCounter = 0;
        public int numOfColumn = 0;
        public int numOfRow = 0;

        private inventorySlot[,] variationList;
        private bool obeySize = false;
        private int itemWidth = 0;
        private int itemHeight = 0;
        private int newSpawnWidth = 0;
        private int newSpawnHeight = 0;
        private bool wallmount = false;
        private bool ceiling = false;

        byte[] Layer1 = null;
        byte[] Layer2 = null;
        byte[] Acre = null;
        byte[] Building = null;
        byte[] Terrain = null;
        byte[] ActivateLayer1 = null;
        byte[] ActivateLayer2 = null;

        bool[,] ActivateTable1;
        bool[,] ActivateTable2;

        private readonly ToolStripMenuItem ActivateItem;
        private readonly ToolStripMenuItem DeactivateItem;

        public event CloseHandler CloseForm;
        private static readonly object lockObject = new();
        readonly Color[] target =
        {
            Color.FromArgb(252, 3, 3),
            Color.FromArgb(252, 78, 3),
            Color.FromArgb(252, 227, 3),
            Color.FromArgb(0, 82, 7),
            Color.FromArgb(3, 255, 32),
            Color.FromArgb(5, 106, 230),
            Color.FromArgb(157, 40, 224),
        };
        int targetValue = 0;
        #endregion

        #region Form Load
        public Map(Socket S, USBBot USB, string itemPath, string recipePath, string flowerPath, string variationPath, string favPath, string ImagePath, string LanguageSetting, Dictionary<string, string> overrideDict, bool Sound)
        {
            try
            {
                s = S;
                usb = USB;
                if (File.Exists(itemPath))
                    source = LoadItemCSV(itemPath);
                if (File.Exists(recipePath))
                    recipeSource = LoadItemCSV(recipePath);
                if (File.Exists(flowerPath))
                    flowerSource = LoadItemCSV(flowerPath);
                if (File.Exists(variationPath))
                    variationSource = LoadItemCSV(variationPath);
                if (File.Exists(favPath))
                    favSource = LoadItemCSV(favPath, false);
                if (File.Exists(Utilities.fieldPath))
                    fieldSource = LoadItemCSV(Utilities.fieldPath);
                imagePath = ImagePath;
                OverrideDict = overrideDict;
                sound = Sound;

                floorSlots = new FloorSlot[49];

                InitializeComponent();

                foreach (FloorSlot btn in BtnPanel.Controls.OfType<FloorSlot>())
                {
                    int i = int.Parse(btn.Tag.ToString());
                    floorSlots[i] = btn;
                }


                if (source != null)
                {
                    fieldGridView.DataSource = source;

                    //set the ID row invisible
                    fieldGridView.Columns["id"].Visible = false;
                    fieldGridView.Columns["iName"].Visible = false;
                    fieldGridView.Columns["jpn"].Visible = false;
                    fieldGridView.Columns["tchi"].Visible = false;
                    fieldGridView.Columns["schi"].Visible = false;
                    fieldGridView.Columns["kor"].Visible = false;
                    fieldGridView.Columns["fre"].Visible = false;
                    fieldGridView.Columns["ger"].Visible = false;
                    fieldGridView.Columns["spa"].Visible = false;
                    fieldGridView.Columns["ita"].Visible = false;
                    fieldGridView.Columns["dut"].Visible = false;
                    fieldGridView.Columns["rus"].Visible = false;
                    fieldGridView.Columns["color"].Visible = false;
                    fieldGridView.Columns["size"].Visible = false;

                    //select the full row and change color cause windows blue sux
                    fieldGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    fieldGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
                    fieldGridView.DefaultCellStyle.ForeColor = Color.FromArgb(255, 114, 105, 110);
                    fieldGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                    fieldGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 57, 60, 67);
                    fieldGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    fieldGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                    fieldGridView.EnableHeadersVisualStyles = false;

                    //create the image column
                    DataGridViewImageColumn imageColumn = new()
                    {
                        Name = "Image",
                        HeaderText = "Image",
                        ImageLayout = DataGridViewImageCellLayout.Zoom
                    };
                    fieldGridView.Columns.Insert(13, imageColumn);
                    imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                    fieldGridView.Columns["eng"].Width = 195;
                    fieldGridView.Columns["jpn"].Width = 195;
                    fieldGridView.Columns["tchi"].Width = 195;
                    fieldGridView.Columns["schi"].Width = 195;
                    fieldGridView.Columns["kor"].Width = 195;
                    fieldGridView.Columns["fre"].Width = 195;
                    fieldGridView.Columns["ger"].Width = 195;
                    fieldGridView.Columns["spa"].Width = 195;
                    fieldGridView.Columns["ita"].Width = 195;
                    fieldGridView.Columns["dut"].Width = 195;
                    fieldGridView.Columns["rus"].Width = 195;
                    fieldGridView.Columns["Image"].Width = 128;

                    fieldGridView.Columns["eng"].HeaderText = "Name";
                    fieldGridView.Columns["jpn"].HeaderText = "Name";
                    fieldGridView.Columns["tchi"].HeaderText = "Name";
                    fieldGridView.Columns["schi"].HeaderText = "Name";
                    fieldGridView.Columns["kor"].HeaderText = "Name";
                    fieldGridView.Columns["fre"].HeaderText = "Name";
                    fieldGridView.Columns["ger"].HeaderText = "Name";
                    fieldGridView.Columns["spa"].HeaderText = "Name";
                    fieldGridView.Columns["ita"].HeaderText = "Name";
                    fieldGridView.Columns["dut"].HeaderText = "Name";
                    fieldGridView.Columns["rus"].HeaderText = "Name";

                    currentDataTable = source;

                    fieldGridView.DefaultCellStyle.Font = new Font("Microsoft JhengHei UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
                }


                //this.BringToFront();
                //this.Focus();
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

                this.KeyPreview = true;

                LanguageSetup(LanguageSetting);
                languageSetting = LanguageSetting;

                if (fieldGridView.Columns.Contains(languageSetting))
                {
                    HideAllLanguage();
                    fieldGridView.Columns[languageSetting].Visible = true;
                }

                FlashTimer.Start();

                ((TextBox)HexTextbox.Controls[1]).MaxLength = 8;
                ((TextBox)FlagTextbox.Controls[1]).MaxLength = 2;

                MyLog.LogEvent("Map", "MapForm Started Successfully");
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "MapForm Construct: " + ex.Message.ToString());
                NextSaveTimer.Stop();
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Fetch Map
        private void FetchMapBtn_Click(object sender, EventArgs e)
        {
            fetchMapBtn.Enabled = false;

            btnToolTip.RemoveAll();

            if ((s == null || s.Connected == false) & usb == null)
            {
                MessageBox.Show("Please connect to the Switch first");
                return;
            }

            Thread LoadThread = new(delegate () { FetchMap(Utilities.mapZero, Utilities.mapZero + Utilities.mapSize); });
            LoadThread.Start();
        }

        private void FetchMap(UInt32 layer1Address, UInt32 layer2Address)
        {
            try
            {
                ShowMapWait((42 + 2) * 2, "Fetching Map...");

                Layer1 = Utilities.getMapLayer(s, usb, layer1Address, ref counter);
                Layer2 = Utilities.getMapLayer(s, usb, layer2Address, ref counter);
                Acre = Utilities.getAcre(s, usb);
                Building = Utilities.getBuilding(s, usb);
                Terrain = Utilities.getTerrain(s, usb);
                ActivateLayer1 = Utilities.getActivate(s, usb, Utilities.mapActivate, ref counter);
                ActivateLayer2 = Utilities.getActivate(s, usb, Utilities.mapActivate + Utilities.mapActivateSize, ref counter);

                if (Layer1 != null && Layer2 != null && Acre != null)
                {
                    if (MiniMap == null)
                        MiniMap = new MiniMap(Layer1, Acre, Building, Terrain, 2);
                }
                else
                    throw new NullReferenceException("Layer1/Layer2/Acre");

                BuildActivateTable(ActivateLayer1, ref ActivateTable1);
                BuildActivateTable(ActivateLayer2, ref ActivateTable2);

                byte[] Coordinate = Utilities.getCoordinate(s, usb);

                if (Coordinate != null)
                {
                    int x = BitConverter.ToInt32(Coordinate, 0);
                    int y = BitConverter.ToInt32(Coordinate, 4);

                    anchorX = x - 0x24;
                    anchorY = y - 0x18;

                    if (anchorX < 3 || anchorY < 3 || anchorX > 108 || anchorY > 92)
                    {
                        anchorX = 56;
                        anchorY = 48;
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawBackground(), MiniMap.DrawItemMap());
                        DisplayAnchor(GetMapColumns(anchorX, anchorY));
                        xCoordinate.Text = anchorX.ToString();
                        yCoordinate.Text = anchorY.ToString();
                        EnableBtn();
                        fetchMapBtn.Visible = false;
                        NextSaveTimer.Start();
                    });
                }
                else
                    throw new NullReferenceException("Coordinate");

                HideMapWait();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "FetchMap: " + ex.Message.ToString());
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message.ToString(), "!!! THIS SHIT DOESN'T WORK!! WHY? HAS I EVER?");
            }
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
            this.Invoke((MethodInvoker)delegate
            {
                btnToolTip.RemoveAll();

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

                xCoordinate.Text = anchorX.ToString();
                yCoordinate.Text = anchorY.ToString();

                selectedButton = floor25;

                DisplayAnchor(GetMapColumns(anchorX, anchorY));
            });
        }

        private void DisplayAnchor(byte[][] floorByte)
        {
            miniMapBox.Image = MiniMap.DrawSelectSquare(anchorX, anchorY);

            lock (lockObject)
            {
                /*
                BtnSetup(floorByte[0], floorByte[1], (anchorX - 3), (anchorY - 3), floor1, floor2, floor3, floor4, floor5, floor6, floor7, false);
                BtnSetup(floorByte[2], floorByte[3], (anchorX - 2), (anchorY - 3), floor8, floor9, floor10, floor11, floor12, floor13, floor14, false);
                BtnSetup(floorByte[4], floorByte[5], (anchorX - 1), (anchorY - 3), floor15, floor16, floor17, floor18, floor19, floor20, floor21, false);
                BtnSetup(floorByte[6], floorByte[7], (anchorX - 0), (anchorY - 3), floor22, floor23, floor24, floor25, floor26, floor27, floor28, true);
                BtnSetup(floorByte[8], floorByte[9], (anchorX + 1), (anchorY - 3), floor29, floor30, floor31, floor32, floor33, floor34, floor35, false);
                BtnSetup(floorByte[10], floorByte[11], (anchorX + 2), (anchorY - 3), floor36, floor37, floor38, floor39, floor40, floor41, floor42, false);
                BtnSetup(floorByte[12], floorByte[13], (anchorX + 3), (anchorY - 3), floor43, floor44, floor45, floor46, floor47, floor48, floor49, false);
                */
                SetupBtnCoordinate(anchorX, anchorY);
                UpdateAllBtn();
                ResetBtnColor();
            }
        }

        private void SetupBtnCoordinate(int x, int y)
        {
            int iterator = 0;

            for (int i = -3; i <= 3; i++)
            {
                for (int j = -3; j <= 3; j++)
                {
                    floorSlots[iterator].mapX = x + i;
                    floorSlots[iterator].mapY = y + j;
                    iterator++;
                }
            }
        }

        private void BtnSetup(byte[] b, byte[] b2, int x, int y, FloorSlot slot1, FloorSlot slot2, FloorSlot slot3, FloorSlot slot4, FloorSlot slot5, FloorSlot slot6, FloorSlot slot7, Boolean anchor = false)
        {
            byte[] idBytes = new byte[2];
            byte[] flag1Bytes = new byte[1];
            byte[] flag2Bytes = new byte[1];
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
                Buffer.BlockCopy(b, (i * 0x10) + 0x2, flag2Bytes, 0x0, 0x1);
                Buffer.BlockCopy(b, (i * 0x10) + 0x3, flag1Bytes, 0x0, 0x1);
                Buffer.BlockCopy(b, (i * 0x10) + 0x4, dataBytes, 0x0, 0x4);

                Buffer.BlockCopy(b, (i * 0x10) + 0x8, part2IdBytes, 0x0, 0x4);
                Buffer.BlockCopy(b, (i * 0x10) + 0xC, part2DataBytes, 0x0, 0x4);
                Buffer.BlockCopy(b2, (i * 0x10) + 0x0, part3IdBytes, 0x0, 0x4);
                Buffer.BlockCopy(b2, (i * 0x10) + 0x4, part3DataBytes, 0x0, 0x4);
                Buffer.BlockCopy(b2, (i * 0x10) + 0x8, part4IdBytes, 0x0, 0x4);
                Buffer.BlockCopy(b2, (i * 0x10) + 0xC, part4DataBytes, 0x0, 0x4);

                string itemID = Utilities.flip(Utilities.ByteToHexString(idBytes));
                string flag2 = Utilities.ByteToHexString(flag2Bytes);
                string flag1 = Utilities.ByteToHexString(flag1Bytes);
                string itemData = Utilities.flip(Utilities.ByteToHexString(dataBytes));

                string part2Id = Utilities.flip(Utilities.ByteToHexString(part2IdBytes));
                string part2Data = Utilities.flip(Utilities.ByteToHexString(part2DataBytes));
                string part3Id = Utilities.flip(Utilities.ByteToHexString(part3IdBytes));
                string part3Data = Utilities.flip(Utilities.ByteToHexString(part3DataBytes));
                string part4Id = Utilities.flip(Utilities.ByteToHexString(part4IdBytes));
                string part4Data = Utilities.flip(Utilities.ByteToHexString(part4DataBytes));

                if (i == 0)
                {
                    currentBtn = slot1;
                    SetBtn(slot1, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag1, flag2);
                }
                else if (i == 1)
                {
                    currentBtn = slot2;
                    SetBtn(slot2, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag1, flag2);
                }
                else if (i == 2)
                {
                    currentBtn = slot3;
                    SetBtn(slot3, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag1, flag2);
                }
                else if (i == 3)
                {
                    currentBtn = slot4;
                    SetBtn(slot4, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag1, flag2);
                    if (anchor)
                    {
                        //slot4.BackColor = Color.Red;
                    }
                }
                else if (i == 4)
                {
                    currentBtn = slot5;
                    SetBtn(slot5, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag1, flag2);
                }
                else if (i == 5)
                {
                    currentBtn = slot6;
                    SetBtn(slot6, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag1, flag2);
                }
                else if (i == 6)
                {
                    currentBtn = slot7;
                    SetBtn(slot7, itemID, itemData, part2Id, part2Data, part3Id, part3Data, part4Id, part4Data, flag1, flag2);
                }

                currentBtn.mapX = x;
                currentBtn.mapY = y + i;
            }
        }

        private void SetBtn(FloorSlot btn, string itemID, string itemData, string part2Id, string part2Data, string part3Id, string part3Data, string part4Id, string part4Data, string flag1, string flag2)
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
            string P2Id = Utilities.turn2bytes(part2Id);
            string P3Id = Utilities.turn2bytes(part3Id);
            string P4Id = Utilities.turn2bytes(part4Id);

            string P1Data = Utilities.turn2bytes(itemData);
            string P2Data = Utilities.turn2bytes(part2Data);
            string P3Data = Utilities.turn2bytes(part3Data);
            string P4Data = Utilities.turn2bytes(part4Data);

            string Path1;
            string Path2;
            string Path3;
            string Path4;
            string ContainPath = "";

            string front = Utilities.precedingZeros(Data.ToString("X"), 8).Substring(0, 4);
            //string back = Utilities.precedingZeros(Data.ToString("X"), 8).Substring(4, 4);

            if (P1Id == "FFFD")
                Path1 = GetImagePathFromID(P1Data, source);
            else if (P1Id == "16A2")
            {
                Path1 = GetImagePathFromID(P1Data, recipeSource, Data);
                Name = GetNameFromID(P1Data, recipeSource);
            }
            else if (P1Id == "315A" || P1Id == "1618" || P1Id == "342F")
            {
                Path1 = GetImagePathFromID(itemID, source, Data);
                ContainPath = GetImagePathFromID(P1Data, source, Convert.ToUInt32("0x" + Utilities.translateVariationValueBack(front), 16));
            }
            else if (ItemAttr.hasFenceWithVariation(ID))  // Fence Variation
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

            btn.setup(Name, ID, Data, IntP2Id, IntP2Data, IntP3Id, IntP3Data, IntP4Id, IntP4Data, Path1, Path2, Path3, Path4, ContainPath, flag1, flag2);
        }

        private void UpdateBtn(FloorSlot btn)
        {
            byte[] Left = new byte[16];
            byte[] Right = new byte[16];

            if (layer1Btn.Checked)
            {
                Buffer.BlockCopy(Layer1, btn.mapX * 0xC00 + btn.mapY * 0x10, Left, 0, 16);
                Buffer.BlockCopy(Layer1, btn.mapX * 0xC00 + 0x600 + btn.mapY * 0x10, Right, 0, 16);
            }
            else if (layer2Btn.Checked)
            {
                Buffer.BlockCopy(Layer2, btn.mapX * 0xC00 + btn.mapY * 0x10, Left, 0, 16);
                Buffer.BlockCopy(Layer2, btn.mapX * 0xC00 + 0x600 + btn.mapY * 0x10, Right, 0, 16);
            }

            string LeftStr = Utilities.ByteToHexString(Left);
            string RightStr = Utilities.ByteToHexString(Right);

            string P1Id = Utilities.flip(LeftStr.Substring(0, 4));
            string flag2 = LeftStr.Substring(4, 2);
            string flag1 = LeftStr.Substring(6, 2);
            string P1Data = Utilities.flip(LeftStr.Substring(8, 8));
            string P2Id = Utilities.flip(LeftStr.Substring(16, 4));
            string P2Data = Utilities.flip(LeftStr.Substring(24, 8));

            string P3Id = Utilities.flip(RightStr.Substring(0, 4));
            string P3Data = Utilities.flip(RightStr.Substring(8, 8));
            string P4Id = Utilities.flip(RightStr.Substring(16, 4));
            string P4Data = Utilities.flip(RightStr.Substring(24, 8));

            string Path1;
            string Path2;
            string Path3;
            string Path4;
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
            else if (P1Id == "315A" || P1Id == "1618" || P1Id == "342F")
            {
                Path1 = GetImagePathFromID(P1Id, source, Data);
                ContainPath = GetImagePathFromID(back, source, Convert.ToUInt32("0x" + Utilities.translateVariationValueBack(front), 16));
            }
            else if (ItemAttr.hasFenceWithVariation(ID))  // Fence Variation
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

            btn.setup(Name, ID, Data, IntP2Id, IntP2Data, IntP3Id, IntP3Data, IntP4Id, IntP4Data, Path1, Path2, Path3, Path4, ContainPath, flag1, flag2);
        }

        private void UpdateAllBtn()
        {
            for (int i = 0; i < floorSlots.Length; i++)
            {
                UpdateBtn(floorSlots[i]);
            }
        }

        #endregion

        #region Async

        private async Task DisplayAnchorAsync()
        {
            if (drawing)
                return;

            drawing = true;

            lock (lockObject)
            {
                miniMapBox.Image = MiniMap.DrawSelectSquare(anchorX, anchorY);
            }

            await Task.Run(() => SetupBtnCoordinate(anchorX, anchorY));
            await Task.Run(() => UpdateAllBtn());

            lock (lockObject)
            {
                ResetBtnColor();
                drawing = false;
            }
        }
        #endregion

        private static UInt32 GetAddress(int x, int y)
        {
            return (UInt32)(Utilities.mapZero + (0xC00 * x) + (0x10 * (y)));
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

            /*
            string locked;
            if (button.locked)
                locked = "✓ True";
            else
                locked = "✘ False";
            */
            string activateInfo;
            if (layer1Btn.Checked)
                activateInfo = DisplayActivate(button.mapX, button.mapY, ActivateTable1);
            else
                activateInfo = DisplayActivate(button.mapX, button.mapY, ActivateTable2);

            btnToolTip.SetToolTip(button,
                                    button.itemName +
                                    "\n\n" + "" +
                                    "ID : " + Utilities.precedingZeros(button.itemID.ToString("X"), 4) + "\n" +
                                    "Count : " + Utilities.precedingZeros(button.itemData.ToString("X"), 8) + "\n" +
                                    "Flag1 : 0x" + button.flag1 + "\n" +
                                    "Flag2 : 0x" + button.flag2 + "\n" +
                                    "Coordinate : " + button.mapX + " " + button.mapY + "\n\n" +
                                    "Part2 : " + Utilities.precedingZeros(button.part2.ToString("X"), 4) + " " + Utilities.precedingZeros(button.part2Data.ToString("X"), 8) + "\n" +
                                    "Part3 : " + Utilities.precedingZeros(button.part3.ToString("X"), 4) + " " + Utilities.precedingZeros(button.part3Data.ToString("X"), 8) + "\n" +
                                    "Part4 : " + Utilities.precedingZeros(button.part4.ToString("X"), 4) + " " + Utilities.precedingZeros(button.part4Data.ToString("X"), 8) + "\n" +
                                    //"Locked : " + locked + 
                                    "Terrain : " + MiniMap.GetTerrainData(button.mapX, button.mapY) + "\n" +
                                    "Activate : " + activateInfo
                                    );
        }
        #endregion

        #region Images
        private static string RemoveNumber(string filename)
        {
            char[] MyChar = { '0', '1', '2', '3', '4' };
            return filename.Trim(MyChar);
        }

        public string GetImagePathFromID(string itemID, DataTable source, UInt32 data = 0)
        {
            if (source == null)
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

                    if (OverrideDict.ContainsKey(imageName))
                    {
                        path = imagePath + OverrideDict[imageName] + "_Remake_0_0.png";
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

                if (OverrideDict.ContainsKey(imageName))
                {
                    path = imagePath + OverrideDict[imageName] + ".png";
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
            if (fieldSource != null)
            {
                DataRow FieldRow = fieldSource.Rows.Find(itemID);
                if (FieldRow != null)
                {
                    return (string)FieldRow["name"];
                }
            }

            if (table == null)
            {
                return "";
            }

            DataRow row = table.Rows.Find(itemID);

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

        private void FieldGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            AddImage(fieldGridView, e);
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

                    if (OverrideDict.ContainsKey(imageName))
                    {
                        path = imagePath + OverrideDict[imageName] + "_Remake_0_0.png";
                        if (File.Exists(path))
                        {
                            Image img = Image.FromFile(path);
                            e.CellStyle.BackColor = Color.Green;
                            e.Value = img;

                            return;
                        }
                        else
                        {
                            path = imagePath + OverrideDict[imageName] + ".png";
                            if (File.Exists(path))
                            {
                                Image img = Image.FromFile(path);
                                e.CellStyle.BackColor = Color.Green;
                                e.Value = img;

                                return;
                            }
                        }
                    }

                    path = imagePath + imageName + "_Remake_0_0.png";
                    if (File.Exists(path))
                    {
                        Image img = Image.FromFile(path);
                        e.CellStyle.BackColor = Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(77)))), ((int)(((byte)(162)))));
                        e.Value = img;
                    }
                    else
                    {
                        path = imagePath + imageName + ".png";
                        if (File.Exists(path))
                        {
                            Image img = Image.FromFile(path);
                            e.Value = img;
                        }
                        else
                        {
                            path = imagePath + RemoveNumber(imageName) + ".png";
                            if (File.Exists(path))
                            {
                                Image img = Image.FromFile(path);
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
        private void FieldGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (lastRow != null)
                {
                    lastRow.Height = 22;
                }

                if (e.RowIndex > -1)
                {
                    lastRow = fieldGridView.Rows[e.RowIndex];
                    fieldGridView.Rows[e.RowIndex].Height = 128;

                    if (currentDataTable == source)
                    {
                        string id = fieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();
                        string name = fieldGridView.Rows[e.RowIndex].Cells[languageSetting].Value.ToString();

                        IdTextbox.Text = id;
                        HexTextbox.Text = "00000000";
                        selectedSize = fieldGridView.Rows[e.RowIndex].Cells["size"].Value.ToString();
                        SizeBox.Text = selectedSize.Replace("_5", ".5 ").Replace("_0", ".0 ").Replace("_Wall", "Wall").Replace("_Rug", "Rug").Replace("_Pillar", "Pillar").Replace("_Ceiling", "Ceiling").Replace("x", "x ");

                        selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), 0x0, GetImagePathFromID(id, source), true, "");
                    }
                    else if (currentDataTable == recipeSource)
                    {
                        string id = "16A2"; // Recipe;
                        string name = fieldGridView.Rows[e.RowIndex].Cells[languageSetting].Value.ToString();
                        string hexValue = fieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();

                        IdTextbox.Text = id;
                        HexTextbox.Text = Utilities.precedingZeros(hexValue, 8);
                        SizeBox.Text = "";

                        selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(hexValue, recipeSource), true, "");
                    }
                    else if (currentDataTable == flowerSource)
                    {
                        string id = fieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();
                        string name = fieldGridView.Rows[e.RowIndex].Cells[languageSetting].Value.ToString();
                        string hexValue = fieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();

                        IdTextbox.Text = id;
                        HexTextbox.Text = Utilities.precedingZeros(hexValue, 8);

                        selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source), true, "");
                        SizeBox.Text = "";
                    }
                    else if (currentDataTable == favSource)
                    {
                        string id = fieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();
                        string name = fieldGridView.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                        string hexValue = fieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();

                        IdTextbox.Text = id;
                        HexTextbox.Text = Utilities.precedingZeros(hexValue, 8);
                        SizeBox.Text = "";

                        selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + hexValue, 16)), true, "");
                    }
                    else if (currentDataTable == fieldSource)
                    {
                        string id = fieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString();
                        string name = fieldGridView.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                        string hexValue = fieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();

                        IdTextbox.Text = id;
                        HexTextbox.Text = Utilities.precedingZeros(hexValue, 8);
                        SizeBox.Text = "";

                        selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, fieldSource), true, "");
                    }

                    if (selection != null)
                    {
                        string hexValue = "00000000";

                        if (currentDataTable == flowerSource)
                            hexValue = fieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();
                        else if (currentDataTable == favSource)
                            hexValue = fieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();
                        else if (currentDataTable == fieldSource)
                            hexValue = fieldGridView.Rows[e.RowIndex].Cells["value"].Value.ToString();

                        selection.ReceiveID(Utilities.precedingZeros(selectedItem.fillItemID(), 4), languageSetting, Utilities.precedingZeros(hexValue, 8));
                    }

                    //updateSelectedItemInfo(selectedItem.displayItemName(), selectedItem.displayItemID(), selectedItem.displayItemData());

                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (lastRow != null)
                {
                    lastRow.Height = 22;
                }

                if (e.RowIndex > -1)
                {
                    lastRow = fieldGridView.Rows[e.RowIndex];
                    fieldGridView.Rows[e.RowIndex].Height = 128;

                    string name = selectedItem.displayItemName();
                    string id = selectedItem.displayItemID();
                    string path = selectedItem.getPath();

                    if (IdTextbox.Text != "")
                    {
                        if (IdTextbox.Text == "315A" || IdTextbox.Text == "1618" || IdTextbox.Text == "342F") // Wall-Mounted
                        {
                            HexTextbox.Text = Utilities.precedingZeros("00" + fieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), 8);
                            selectedItem.setup(name, Convert.ToUInt16(id, 16), Convert.ToUInt32("0x" + HexTextbox.Text, 16), path, true, GetImagePathFromID(fieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), source));
                        }
                        else
                        {
                            HexTextbox.Text = Utilities.precedingZeros(fieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), 8);
                            selectedItem.setup(name, Convert.ToUInt16(id, 16), Convert.ToUInt32("0x" + HexTextbox.Text, 16), path, true, GetNameFromID(fieldGridView.Rows[e.RowIndex].Cells["id"].Value.ToString(), source));
                        }

                        if (selection != null)
                        {
                            selection.ReceiveID(Utilities.turn2bytes(selectedItem.fillItemData()), languageSetting);
                        }
                    }

                }
            }
        }

        private void ItemModeBtn_Click(object sender, EventArgs e)
        {
            itemModeBtn.BackColor = Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            recipeModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            flowerModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            favModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            //fieldModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            if (itemSearchBox.Text != "Search...")
            {
                itemSearchBox.Clear();
            }

            fieldGridView.Columns.Remove("Image");

            if (source != null)
            {
                fieldGridView.DataSource = source;

                //set the ID row invisible
                fieldGridView.Columns["id"].Visible = false;
                fieldGridView.Columns["iName"].Visible = false;
                fieldGridView.Columns["color"].Visible = false;

                if (fieldGridView.Columns.Contains(languageSetting))
                {
                    HideAllLanguage();
                    fieldGridView.Columns[languageSetting].Visible = true;
                }

                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                fieldGridView.Columns.Insert(13, imageColumn);
                imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                fieldGridView.Columns["eng"].Width = 195;
                fieldGridView.Columns["jpn"].Width = 195;
                fieldGridView.Columns["tchi"].Width = 195;
                fieldGridView.Columns["schi"].Width = 195;
                fieldGridView.Columns["kor"].Width = 195;
                fieldGridView.Columns["fre"].Width = 195;
                fieldGridView.Columns["ger"].Width = 195;
                fieldGridView.Columns["spa"].Width = 195;
                fieldGridView.Columns["ita"].Width = 195;
                fieldGridView.Columns["dut"].Width = 195;
                fieldGridView.Columns["rus"].Width = 195;
                fieldGridView.Columns["Image"].Width = 128;

                fieldGridView.Columns["eng"].HeaderText = "Name";
                fieldGridView.Columns["jpn"].HeaderText = "Name";
                fieldGridView.Columns["tchi"].HeaderText = "Name";
                fieldGridView.Columns["schi"].HeaderText = "Name";
                fieldGridView.Columns["kor"].HeaderText = "Name";
                fieldGridView.Columns["fre"].HeaderText = "Name";
                fieldGridView.Columns["ger"].HeaderText = "Name";
                fieldGridView.Columns["spa"].HeaderText = "Name";
                fieldGridView.Columns["ita"].HeaderText = "Name";
                fieldGridView.Columns["dut"].HeaderText = "Name";
                fieldGridView.Columns["rus"].HeaderText = "Name";

                currentDataTable = source;
            }

            FlagTextbox.Text = "20";
        }

        private void RecipeModeBtn_Click(object sender, EventArgs e)
        {
            itemModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            recipeModeBtn.BackColor = Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            flowerModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            favModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            //fieldModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            if (itemSearchBox.Text != "Search...")
            {
                itemSearchBox.Clear();
            }

            fieldGridView.Columns.Remove("Image");

            if (recipeSource != null)
            {
                fieldGridView.DataSource = recipeSource;

                fieldGridView.Columns["id"].Visible = false;
                fieldGridView.Columns["iName"].Visible = false;

                if (fieldGridView.Columns.Contains(languageSetting))
                {
                    HideAllLanguage();
                    fieldGridView.Columns[languageSetting].Visible = true;
                }

                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                fieldGridView.Columns.Insert(13, imageColumn);
                imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                fieldGridView.Columns["eng"].Width = 195;
                fieldGridView.Columns["jpn"].Width = 195;
                fieldGridView.Columns["tchi"].Width = 195;
                fieldGridView.Columns["schi"].Width = 195;
                fieldGridView.Columns["kor"].Width = 195;
                fieldGridView.Columns["fre"].Width = 195;
                fieldGridView.Columns["ger"].Width = 195;
                fieldGridView.Columns["spa"].Width = 195;
                fieldGridView.Columns["ita"].Width = 195;
                fieldGridView.Columns["dut"].Width = 195;
                fieldGridView.Columns["rus"].Width = 195;
                fieldGridView.Columns["Image"].Width = 128;

                fieldGridView.Columns["eng"].HeaderText = "Name";
                fieldGridView.Columns["jpn"].HeaderText = "Name";
                fieldGridView.Columns["tchi"].HeaderText = "Name";
                fieldGridView.Columns["schi"].HeaderText = "Name";
                fieldGridView.Columns["kor"].HeaderText = "Name";
                fieldGridView.Columns["fre"].HeaderText = "Name";
                fieldGridView.Columns["ger"].HeaderText = "Name";
                fieldGridView.Columns["spa"].HeaderText = "Name";
                fieldGridView.Columns["ita"].HeaderText = "Name";
                fieldGridView.Columns["dut"].HeaderText = "Name";
                fieldGridView.Columns["rus"].HeaderText = "Name";

                currentDataTable = recipeSource;
            }

            FlagTextbox.Text = "00";
        }

        private void FlowerModeBtn_Click(object sender, EventArgs e)
        {
            itemModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            recipeModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            flowerModeBtn.BackColor = Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            favModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            //fieldModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            if (itemSearchBox.Text != "Search...")
            {
                itemSearchBox.Clear();
            }

            fieldGridView.Columns.Remove("Image");

            if (flowerSource != null)
            {
                fieldGridView.DataSource = flowerSource;

                fieldGridView.Columns["id"].Visible = false;
                fieldGridView.Columns["iName"].Visible = false;
                fieldGridView.Columns["value"].Visible = false;

                if (fieldGridView.Columns.Contains(languageSetting))
                {
                    HideAllLanguage();
                    fieldGridView.Columns[languageSetting].Visible = true;
                }

                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                fieldGridView.Columns.Insert(13, imageColumn);
                imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                fieldGridView.Columns["eng"].Width = 195;
                fieldGridView.Columns["jpn"].Width = 195;
                fieldGridView.Columns["tchi"].Width = 195;
                fieldGridView.Columns["schi"].Width = 195;
                fieldGridView.Columns["kor"].Width = 195;
                fieldGridView.Columns["fre"].Width = 195;
                fieldGridView.Columns["ger"].Width = 195;
                fieldGridView.Columns["spa"].Width = 195;
                fieldGridView.Columns["ita"].Width = 195;
                fieldGridView.Columns["dut"].Width = 195;
                fieldGridView.Columns["rus"].Width = 195;
                fieldGridView.Columns["Image"].Width = 128;

                fieldGridView.Columns["eng"].HeaderText = "Name";
                fieldGridView.Columns["jpn"].HeaderText = "Name";
                fieldGridView.Columns["tchi"].HeaderText = "Name";
                fieldGridView.Columns["schi"].HeaderText = "Name";
                fieldGridView.Columns["kor"].HeaderText = "Name";
                fieldGridView.Columns["fre"].HeaderText = "Name";
                fieldGridView.Columns["ger"].HeaderText = "Name";
                fieldGridView.Columns["spa"].HeaderText = "Name";
                fieldGridView.Columns["ita"].HeaderText = "Name";
                fieldGridView.Columns["dut"].HeaderText = "Name";
                fieldGridView.Columns["rus"].HeaderText = "Name";

                currentDataTable = flowerSource;
            }

            FlagTextbox.Text = "20";
        }

        private void FavModeBtn_Click(object sender, EventArgs e)
        {
            itemModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            recipeModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            flowerModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            favModeBtn.BackColor = Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            //fieldModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            if (itemSearchBox.Text != "Search...")
            {
                itemSearchBox.Clear();
            }

            fieldGridView.Columns.Remove("Image");

            if (favSource != null)
            {
                fieldGridView.DataSource = favSource;

                fieldGridView.Columns["id"].Visible = false;
                fieldGridView.Columns["iName"].Visible = false;
                fieldGridView.Columns["Name"].Visible = true;
                fieldGridView.Columns["value"].Visible = false;

                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                fieldGridView.Columns.Insert(4, imageColumn);
                imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                fieldGridView.Columns["Name"].Width = 195;
                fieldGridView.Columns["Image"].Width = 128;

                currentDataTable = favSource;
            }

            FlagTextbox.Text = "20";
        }

        private void FieldModeBtn_Click(object sender, EventArgs e)
        {
            FlashTimer.Stop();

            itemModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            recipeModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            flowerModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            favModeBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            //fieldModeBtn.BackColor = Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));

            if (itemSearchBox.Text != "Search...")
            {
                itemSearchBox.Clear();
            }

            fieldGridView.Columns.Remove("Image");

            if (favSource != null)
            {
                fieldGridView.DataSource = fieldSource;

                fieldGridView.Columns["id"].Visible = false;
                fieldGridView.Columns["iName"].Visible = false;
                fieldGridView.Columns["name"].Visible = true;
                fieldGridView.Columns["value"].Visible = false;

                DataGridViewImageColumn imageColumn = new()
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                fieldGridView.Columns.Insert(3, imageColumn);
                imageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                fieldGridView.Columns["name"].Width = 195;
                fieldGridView.Columns["Image"].Width = 128;

                fieldGridView.Columns["name"].HeaderText = "Name";

                currentDataTable = fieldSource;
            }

            FlagTextbox.Text = "00";
        }

        private static DataTable LoadItemCSV(string filePath, bool key = true)
        {
            var dt = new DataTable();

            File.ReadLines(filePath).Take(1)
                .SelectMany(x => x.Split(new[] { " ; " }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            File.ReadLines(filePath).Skip(1)
                .Select(x => x.Split(new[] { " ; " }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));

            if (key)
            {
                if (dt.Columns.Contains("id"))
                    dt.PrimaryKey = new DataColumn[1] { dt.Columns["id"] };
            }

            return dt;
        }

        private void ItemSearchBox_TextChanged(object sender, EventArgs e)
        {
            if (itemSearchBox.Text == "Search...")
                return;
            try
            {
                if (fieldGridView.DataSource != null)
                {
                    if (currentDataTable == source)
                    {
                        (fieldGridView.DataSource as DataTable).DefaultView.RowFilter = string.Format(languageSetting + " LIKE '%{0}%'", EscapeLikeValue(itemSearchBox.Text));
                    }
                    else if (currentDataTable == recipeSource)
                    {
                        (fieldGridView.DataSource as DataTable).DefaultView.RowFilter = string.Format(languageSetting + " LIKE '%{0}%'", EscapeLikeValue(itemSearchBox.Text));
                    }
                    else if (currentDataTable == flowerSource)
                    {
                        (fieldGridView.DataSource as DataTable).DefaultView.RowFilter = string.Format(languageSetting + " LIKE '%{0}%'", EscapeLikeValue(itemSearchBox.Text));
                    }
                    else if (currentDataTable == favSource)
                    {
                        (fieldGridView.DataSource as DataTable).DefaultView.RowFilter = string.Format("name" + " LIKE '%{0}%'", EscapeLikeValue(itemSearchBox.Text));
                    }
                    else if (currentDataTable == fieldSource)
                    {
                        (fieldGridView.DataSource as DataTable).DefaultView.RowFilter = string.Format("name" + " LIKE '%{0}%'", EscapeLikeValue(itemSearchBox.Text));
                    }
                }
            }
            catch
            {
                itemSearchBox.Clear();
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

            if (Control.ModifierKeys == Keys.Control)
            {
                SetCorner(button);
            }
            else
            {
                selectedButton = button;

                if (Control.ModifierKeys == Keys.Shift)
                {
                    SelectedItem_Click(sender, e);
                }
                else if (Control.ModifierKeys == Keys.Alt)
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
                address = GetAddress(btn.mapX, btn.mapY);
            }
            else if (layer2Btn.Checked)
            {
                address = (GetAddress(btn.mapX, btn.mapY) + Utilities.mapSize);
            }
            else
                return;

            string itemID = Utilities.precedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.precedingZeros(HexTextbox.Text, 8);
            string flag2 = Utilities.precedingZeros(FlagTextbox.Text, 2);
            string flag1 = selectedItem.getFlag1();

            MoveToNextTile();

            Thread spawnThread = new(delegate () { DropItem(address, itemID, itemData, flag1, flag2, btn); });
            spawnThread.Start();
        }

        private void DeleteItem(FloorSlot btn)
        {
            long address;

            if (layer1Btn.Checked)
                address = GetAddress(btn.mapX, btn.mapY);
            else if (layer2Btn.Checked)
                address = GetAddress(btn.mapX, btn.mapY) + Utilities.mapSize;
            else
                return;

            DisableBtn();

            Thread deleteThread = new(delegate () { DeleteItem(address, btn); });
            deleteThread.Start();
        }

        private void CopyItem(FloorSlot btn)
        {
            string id = Utilities.precedingZeros(btn.itemID.ToString("X"), 4);
            string name = btn.Name;
            string hexValue = Utilities.precedingZeros(btn.itemData.ToString("X"), 8);
            string flag1 = btn.flag1;
            string flag2 = btn.flag2;

            IdTextbox.Text = id;
            HexTextbox.Text = hexValue;
            FlagTextbox.Text = flag2;

            UInt16 IntId = Convert.ToUInt16("0x" + id, 16);
            string front = Utilities.precedingZeros(hexValue, 8).Substring(0, 4);
            string back = Utilities.precedingZeros(hexValue, 8).Substring(4, 4);


            if (id == "16A2")
                selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.turn2bytes(hexValue), recipeSource), true, "", flag1, flag2);
            else if (ItemAttr.hasFenceWithVariation(IntId))  // Fence Variation
                selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + front, 16)), true, "", flag1, flag2);
            else if (id == "315A" || id == "1618" || id == "342F")
                selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID((Utilities.turn2bytes(hexValue)), source, Convert.ToUInt32("0x" + Utilities.translateVariationValueBack(front), 16)), flag1, flag2);
            else
                selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + hexValue, 16)), true, "", flag1, flag2);

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
                if (fieldGridView.Rows.Count <= 0)
                {
                    return;
                }
                else if (fieldGridView.Rows.Count == 1)
                {
                    lastRow = fieldGridView.Rows[fieldGridView.CurrentRow.Index];
                    fieldGridView.Rows[fieldGridView.CurrentRow.Index].Height = 160;

                    KeyPressSetup(fieldGridView.CurrentRow.Index);
                }
                else if (fieldGridView.CurrentRow.Index + 1 < fieldGridView.Rows.Count)
                {
                    if (lastRow != null)
                    {
                        lastRow.Height = 22;
                    }
                    lastRow = fieldGridView.Rows[fieldGridView.CurrentRow.Index + 1];
                    fieldGridView.Rows[fieldGridView.CurrentRow.Index + 1].Height = 160;

                    KeyPressSetup(fieldGridView.CurrentRow.Index + 1);
                    fieldGridView.CurrentCell = fieldGridView.Rows[fieldGridView.CurrentRow.Index + 1].Cells[fieldGridView.CurrentCell.ColumnIndex];
                }

                if (selection != null)
                {
                    string hexValue = "00000000";

                    if (currentDataTable == flowerSource)
                        hexValue = fieldGridView.Rows[fieldGridView.CurrentRow.Index + 1].Cells["value"].Value.ToString();
                    else if (currentDataTable == favSource)
                        hexValue = fieldGridView.Rows[fieldGridView.CurrentRow.Index + 1].Cells["value"].Value.ToString();
                    else if (currentDataTable == fieldSource)
                        hexValue = fieldGridView.Rows[fieldGridView.CurrentRow.Index + 1].Cells["value"].Value.ToString();

                    selection.ReceiveID(Utilities.precedingZeros(selectedItem.fillItemID(), 4), languageSetting, Utilities.precedingZeros(hexValue, 8));
                }

            }
            else if (e.KeyCode.ToString() == "Home")
            {
                if (fieldGridView.Rows.Count <= 0)
                {
                    return;
                }
                else if (fieldGridView.Rows.Count == 1)
                {
                    lastRow = fieldGridView.Rows[fieldGridView.CurrentRow.Index];
                    fieldGridView.Rows[fieldGridView.CurrentRow.Index].Height = 160;

                    KeyPressSetup(fieldGridView.CurrentRow.Index);
                }
                else if (fieldGridView.CurrentRow.Index > 0)
                {
                    if (lastRow != null)
                    {
                        lastRow.Height = 22;
                    }

                    lastRow = fieldGridView.Rows[fieldGridView.CurrentRow.Index - 1];
                    fieldGridView.Rows[fieldGridView.CurrentRow.Index - 1].Height = 160;

                    KeyPressSetup(fieldGridView.CurrentRow.Index - 1);
                    fieldGridView.CurrentCell = fieldGridView.Rows[fieldGridView.CurrentRow.Index - 1].Cells[fieldGridView.CurrentCell.ColumnIndex];
                }

                if (selection != null)
                {
                    string hexValue = "00000000";

                    if (currentDataTable == flowerSource)
                        hexValue = fieldGridView.Rows[fieldGridView.CurrentRow.Index - 1].Cells["value"].Value.ToString();
                    else if (currentDataTable == favSource)
                        hexValue = fieldGridView.Rows[fieldGridView.CurrentRow.Index - 1].Cells["value"].Value.ToString();
                    else if (currentDataTable == fieldSource)
                        hexValue = fieldGridView.Rows[fieldGridView.CurrentRow.Index - 1].Cells["value"].Value.ToString();

                    selection.ReceiveID(Utilities.precedingZeros(selectedItem.fillItemID(), 4), languageSetting, Utilities.precedingZeros(hexValue, 8));
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
                string id = fieldGridView.Rows[index].Cells["id"].Value.ToString();
                string name = fieldGridView.Rows[index].Cells[languageSetting].Value.ToString();

                IdTextbox.Text = id;
                HexTextbox.Text = "00000000";
                selectedSize = fieldGridView.Rows[index].Cells["size"].Value.ToString();
                SizeBox.Text = selectedSize.Replace("_5", ".5 ").Replace("_0", ".0 ").Replace("_Wall", "Wall").Replace("_Rug", "Rug").Replace("_Pillar", "Pillar").Replace("_Ceiling", "Ceiling").Replace("x", "x ");


                selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), 0x0, GetImagePathFromID(id, source), true, "");
            }
            else if (currentDataTable == recipeSource)
            {
                string id = "16A2"; // Recipe;
                string name = fieldGridView.Rows[index].Cells[languageSetting].Value.ToString();
                string hexValue = fieldGridView.Rows[index].Cells["id"].Value.ToString();

                IdTextbox.Text = id;
                HexTextbox.Text = Utilities.precedingZeros(hexValue, 8);
                SizeBox.Text = "";

                selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(hexValue, recipeSource), true, "");
            }
            else if (currentDataTable == flowerSource)
            {
                string id = fieldGridView.Rows[index].Cells["id"].Value.ToString();
                string name = fieldGridView.Rows[index].Cells[languageSetting].Value.ToString();
                string hexValue = fieldGridView.Rows[index].Cells["value"].Value.ToString();

                IdTextbox.Text = id;
                HexTextbox.Text = Utilities.precedingZeros(hexValue, 8);
                SizeBox.Text = "";

                selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source), true, "");

            }
            else if (currentDataTable == favSource)
            {
                string id = fieldGridView.Rows[index].Cells["id"].Value.ToString();
                string name = fieldGridView.Rows[index].Cells["Name"].Value.ToString();
                string hexValue = fieldGridView.Rows[index].Cells["value"].Value.ToString();

                IdTextbox.Text = id;
                HexTextbox.Text = Utilities.precedingZeros(hexValue, 8);
                SizeBox.Text = "";

                selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source), true, "");
            }
            else if (currentDataTable == fieldSource)
            {
                string id = fieldGridView.Rows[index].Cells["id"].Value.ToString();
                string name = fieldGridView.Rows[index].Cells["Name"].Value.ToString();
                string hexValue = fieldGridView.Rows[index].Cells["value"].Value.ToString();

                IdTextbox.Text = id;
                HexTextbox.Text = Utilities.precedingZeros(hexValue, 8);
                SizeBox.Text = "";

                selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, fieldSource), true, "");
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
                        btn.setBackColor(true, Corner1X, Corner1Y, Corner2X, Corner2Y, true);
                    else
                        btn.setBackColor(false, Corner1X, Corner1Y, Corner2X, Corner2Y, true);
                }
                else
                {
                    if (layer1Btn.Checked)
                        btn.setBackColor(true, Corner1X, Corner1Y, Corner2X, Corner2Y);
                    else
                        btn.setBackColor(false, Corner1X, Corner1Y, Corner2X, Corner2Y);
                }
            }

            if (selectedButton != null)
            {
                selectedButton.BackColor = System.Drawing.Color.LightSeaGreen;
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

            if (Control.ModifierKeys == Keys.Control)
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
                MessageBox.Show("Please select a slot!");
                return;
            }

            long address;

            if (layer1Btn.Checked)
            {
                address = GetAddress(selectedButton.mapX, selectedButton.mapY);
            }
            else if (layer2Btn.Checked)
            {
                address = (GetAddress(selectedButton.mapX, selectedButton.mapY) + Utilities.mapSize);
            }
            else
                return;

            string itemID = Utilities.precedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.precedingZeros(HexTextbox.Text, 8);
            string flag2 = Utilities.precedingZeros(FlagTextbox.Text, 2);
            string flag1 = selectedItem.getFlag1();

            DisableBtn();

            Thread spawnThread = new(delegate () { DropItem(address, itemID, itemData, flag1, flag2, selectedButton); });
            spawnThread.Start();
        }

        private void DropItem(long address, string itemID, string itemData, string flag1, string flag2, FloorSlot btn)
        {
            ShowMapWait(2, "Spawning Item...");

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
                        this.Invoke((MethodInvoker)delegate
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

            Utilities.dropItem(s, usb, address, itemID, itemData, flag1, flag2);

            this.Invoke((MethodInvoker)delegate
            {
                /*
                if (itemID == "FFFE")
                    SetBtn(btn, itemID, itemData, "0000FFFE", "00000000", "0000FFFE", "00000000", "0000FFFE", "00000000", "00", flag2);
                else
                    SetBtn(btn, itemID, itemData, "0000FFFD", "0100" + itemID, "0000FFFD", "0001" + itemID, "0000FFFD", "0101" + itemID, "00", flag2);
                */

                UpdataData(btn.mapX, btn.mapY, itemID, itemData, flag1, flag2);
                UpdateBtn(btn);
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
                int differenceX = button.mapX - Corner1X;
                int differenceY = button.mapY - Corner1Y;

                Corner1X = button.mapX;
                Corner1Y = button.mapY;
                Corner2X += differenceX;
                Corner2Y += differenceY;

                Corner1XBox.Text = Corner1X.ToString();
                Corner1YBox.Text = Corner1Y.ToString();
                Corner2XBox.Text = Corner2X.ToString();
                Corner2YBox.Text = Corner2Y.ToString();
            }
            else if (AreaCopied && !CornerOne) // Just selected corner1
            {
                int differenceX = button.mapX - Corner2X;
                int differenceY = button.mapY - Corner2Y;

                Corner2X = button.mapX;
                Corner2Y = button.mapY;
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
                Corner1X = button.mapX;
                Corner1Y = button.mapY;
                Corner1XBox.Text = Corner1X.ToString();
                Corner1YBox.Text = Corner1Y.ToString();
            }
            else
            {
                CornerOne = true;
                Corner2X = button.mapX;
                Corner2Y = button.mapY;
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

                miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.CombineMap(MiniMap.DrawBackground(), MiniMap.DrawItemMap()), MiniMap.DrawPreview(numberOfRow, numberOfColumn, TopLeftX, TopLeftY, true));
                return;
            }

            miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawBackground(), MiniMap.DrawItemMap());
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

            string itemID = Utilities.precedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.precedingZeros(HexTextbox.Text, 8);
            string flag1 = selectedItem.getFlag1();
            string flag2 = Utilities.precedingZeros(FlagTextbox.Text, 2);

            byte[] ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
            byte[] ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

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

            try
            {
                int time = SpawnArea.Length / 4;

                Debug.Print("Length :" + SpawnArea.Length + " Time : " + time);

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
                            this.Invoke((MethodInvoker)delegate
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

                for (int i = 0; i < SpawnArea.Length / 2; i++)
                {
                    UInt32 currentColumn = (UInt32)(address + (0xC00 * (TopLeftX + i)) + (0x10 * (TopLeftY)));

                    Utilities.dropColumn(s, usb, currentColumn, currentColumn + 0x600, SpawnArea[i * 2], SpawnArea[i * 2 + 1], ref counter);
                }

            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "areaSpawn: " + ex.Message.ToString());
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message.ToString(), "I'm sorry.");
            }


            Thread.Sleep(5000);

            this.Invoke((MethodInvoker)delegate
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
                if (IsActivate(selectedButton.mapX, selectedButton.mapY, ActivateTable1))
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
                if (IsActivate(selectedButton.mapX, selectedButton.mapY, ActivateTable2))
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
                        Buffer.BlockCopy(Layer1, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY))), SavedArea[i * 2], 0x10 * j, 0x10);
                        Buffer.BlockCopy(Layer1, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY)) + 0x600), SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                    }
                    else
                    {
                        Buffer.BlockCopy(Layer2, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY))), SavedArea[i * 2], 0x10 * j, 0x10);
                        Buffer.BlockCopy(Layer2, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY)) + 0x600), SavedArea[i * 2 + 1], 0x10 * j, 0x10);
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

            Thread pasteAreaThread = new(delegate () { PasteArea(address, TopLeftX, TopLeftY, numberOfColumn); });
            pasteAreaThread.Start();
        }

        private void PasteArea(long address, int TopLeftX, int TopLeftY, int numberOfColumn)
        {
            ShowMapWait(numberOfColumn, "Kicking Babies...");

            try
            {
                int time = numberOfColumn;

                Debug.Print("Length :" + numberOfColumn + " Time : " + time);

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
                            this.Invoke((MethodInvoker)delegate
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

                for (int i = 0; i < numberOfColumn; i++)
                {
                    UInt32 CurAddress = (UInt32)(address + (0xC00 * (TopLeftX + i)) + (0x10 * (TopLeftY)));

                    Utilities.dropColumn(s, usb, CurAddress, CurAddress + 0x600, SavedArea[i * 2], SavedArea[i * 2 + 1], ref counter);
                }

                this.Invoke((MethodInvoker)delegate
                {
                    UpdataData(TopLeftX, TopLeftY, SavedArea, false, true);
                });

            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "PasteArea: " + ex.Message.ToString());
                MyMessageBox.Show(ex.Message.ToString(), "Dafuq?");
            }

            Thread.Sleep(5000);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();

            this.Invoke((MethodInvoker)delegate
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
            miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawBackground(), MiniMap.DrawItemMap());
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
            byte[] save = Array.Empty<byte>();
            byte[] tempItem = new byte[sizeOfRow];

            for (int i = 0; i < numberOfColumn; i++)
            {
                for (int j = 0; j < numberOfRow; j++)
                {
                    if (layer1Btn.Checked)
                        Buffer.BlockCopy(Layer1, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY))), tempItem, 0, sizeOfRow);
                    else
                        Buffer.BlockCopy(Layer2, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY))), tempItem, 0, sizeOfRow);

                    save = Utilities.add(save, tempItem);
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
                savepath = Directory.GetCurrentDirectory() + @"\save";
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
                SetActivate(selectedButton.mapX, selectedButton.mapY, ref ActivateLayer1, ref ActivateTable1);
            else
                SetActivate(selectedButton.mapX, selectedButton.mapY, ref ActivateLayer2, ref ActivateTable2);
        }

        private void DeactivateItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (layer1Btn.Checked)
                SetDeactivate(selectedButton.mapX, selectedButton.mapY, ref ActivateLayer1, ref ActivateTable1);
            else
                SetDeactivate(selectedButton.mapX, selectedButton.mapY, ref ActivateLayer2, ref ActivateTable2);
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
                        address = GetAddress(btn.mapX, btn.mapY);
                    }
                    else if (layer2Btn.Checked)
                    {
                        address = (GetAddress(btn.mapX, btn.mapY) + Utilities.mapSize);
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
                        this.Invoke((MethodInvoker)delegate
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

            Utilities.deleteFloorItem(s, usb, address);

            this.Invoke((MethodInvoker)delegate
            {
                UpdataData(selectedButton.mapX, selectedButton.mapY);
                btn.reset();
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
                    string id = Utilities.precedingZeros(btn.itemID.ToString("X"), 4);
                    string name = btn.Name;
                    string hexValue = Utilities.precedingZeros(btn.itemData.ToString("X"), 8);
                    string flag1 = btn.flag1;
                    string flag2 = btn.flag2;

                    IdTextbox.Text = id;
                    HexTextbox.Text = hexValue;
                    FlagTextbox.Text = flag2;

                    UInt16 IntId = Convert.ToUInt16("0x" + id, 16);
                    string front = Utilities.precedingZeros(hexValue, 8).Substring(0, 4);
                    string back = Utilities.precedingZeros(hexValue, 8).Substring(4, 4);


                    if (id == "16A2")
                        selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(Utilities.turn2bytes(hexValue), recipeSource), true, "", flag1, flag2);
                    else if (ItemAttr.hasFenceWithVariation(IntId))  // Fence Variation
                        selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + front, 16)), true, "", flag1, flag2);
                    else if (id == "315A" || id == "1618" || id == "342F")
                        selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + hexValue, 16)), true, GetImagePathFromID((Utilities.turn2bytes(hexValue)), source, Convert.ToUInt32("0x" + Utilities.translateVariationValueBack(front), 16)), flag1, flag2);
                    else
                        selectedItem.setup(name, Convert.ToUInt16("0x" + id, 16), Convert.ToUInt32("0x" + hexValue, 16), GetImagePathFromID(id, source, Convert.ToUInt32("0x" + hexValue, 16)), true, "", flag1, flag2);

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

            string itemID = Utilities.precedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.precedingZeros(HexTextbox.Text, 8);
            string flag2 = Utilities.precedingZeros(FlagTextbox.Text, 2);

            UInt16 IntId = Convert.ToUInt16("0x" + itemID, 16);
            string front = Utilities.precedingZeros(itemData, 8).Substring(0, 4);
            //string back = Utilities.precedingZeros(itemData, 8).Substring(4, 4);

            if (itemID.Equals("315A") || itemID.Equals("1618") || itemID.Equals("342F"))
            {
                selectedItem.setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source), true, GetImagePathFromID(Utilities.turn2bytes(itemData), source, Convert.ToUInt32("0x" + Utilities.translateVariationValueBack(front), 16)), "00", flag2);
            }
            else if (itemID.Equals("16A2"))
            {
                selectedItem.setup(GetNameFromID(itemID, recipeSource), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(Utilities.turn2bytes(itemData), recipeSource), true, "", "00", flag2);
            }
            else if (ItemAttr.hasFenceWithVariation(IntId))  // Fence Variation
            {
                selectedItem.setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + front, 16)), true, "", "00", flag2);
            }
            else
            {
                selectedItem.setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + itemData, 16)), true, "", "00", flag2);
            }
        }

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
                Layer1 = Utilities.getMapLayer(s, usb, layer1Address, ref counter);
                Layer2 = Utilities.getMapLayer(s, usb, layer2Address, ref counter);

                if (layer1Btn.Checked)
                    miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer1);
                else
                    miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer2);

                ActivateLayer1 = Utilities.getActivate(s, usb, Utilities.mapActivate, ref counter);
                ActivateLayer2 = Utilities.getActivate(s, usb, Utilities.mapActivate + Utilities.mapActivateSize, ref counter);

                if (Layer1 != null && Layer2 != null && Acre != null)
                {
                    if (MiniMap == null)
                        MiniMap = new MiniMap(Layer1, Acre, Building, Terrain, 2);
                }
                else
                    throw new NullReferenceException("Layer1/Layer2/Acre");

                BuildActivateTable(ActivateLayer1, ref ActivateTable1);
                BuildActivateTable(ActivateLayer2, ref ActivateTable2);

                this.Invoke((MethodInvoker)delegate
                {
                    DisplayAnchor(GetMapColumns(anchorX, anchorY));
                    EnableBtn();
                });

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "RefreshMap: " + ex.Message.ToString());
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message.ToString(), "For the brave souls who get this far: You are the chosen ones.");
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

            Thread clearGridThread = new(delegate () { ClearGrid(); });
            clearGridThread.Start();
        }

        private void ClearGrid()
        {
            ShowMapWait(14, "Clearing Grid...");

            try
            {
                byte[][] b = new byte[14][];

                UInt32 address = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)));

                FillFloor(ref b, null);

                UInt32 address1;
                UInt32 address2;
                UInt32 address3;
                UInt32 address4;
                UInt32 address5;
                UInt32 address6;
                UInt32 address7;

                if (layer1Btn.Checked)
                {
                    address1 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)));
                    address2 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 2)) + (0x10 * (anchorY - 3)));
                    address3 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 1)) + (0x10 * (anchorY - 3)));
                    address4 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 0)) + (0x10 * (anchorY - 3)));
                    address5 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 1)) + (0x10 * (anchorY - 3)));
                    address6 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 2)) + (0x10 * (anchorY - 3)));
                    address7 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 3)) + (0x10 * (anchorY - 3)));
                }
                else if (layer2Btn.Checked)
                {
                    address1 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address2 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 2)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address3 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 1)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address4 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 0)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address5 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 1)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address6 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 2)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address7 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 3)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                }
                else
                    return;

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
                            this.Invoke((MethodInvoker)delegate
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

                Utilities.dropColumn(s, usb, address1, address1 + 0x600, b[0], b[1], ref counter);
                Utilities.dropColumn(s, usb, address2, address2 + 0x600, b[2], b[3], ref counter);
                Utilities.dropColumn(s, usb, address3, address3 + 0x600, b[4], b[5], ref counter);
                Utilities.dropColumn(s, usb, address4, address4 + 0x600, b[6], b[7], ref counter);
                Utilities.dropColumn(s, usb, address5, address5 + 0x600, b[8], b[9], ref counter);
                Utilities.dropColumn(s, usb, address6, address6 + 0x600, b[10], b[11], ref counter);
                Utilities.dropColumn(s, usb, address7, address7 + 0x600, b[12], b[13], ref counter);

                this.Invoke((MethodInvoker)delegate
                {
                    /*
                    BtnSetup(b[0], b[1], anchorX - 3, anchorY - 3, floor1, floor2, floor3, floor4, floor5, floor6, floor7, false);
                    BtnSetup(b[2], b[3], anchorX - 2, anchorY - 3, floor8, floor9, floor10, floor11, floor12, floor13, floor14, false);
                    BtnSetup(b[4], b[5], anchorX - 1, anchorY - 3, floor15, floor16, floor17, floor18, floor19, floor20, floor21, false);
                    BtnSetup(b[6], b[7], anchorX - 0, anchorY - 3, floor22, floor23, floor24, floor25, floor26, floor27, floor28, false);
                    BtnSetup(b[8], b[9], anchorX + 1, anchorY - 3, floor29, floor30, floor31, floor32, floor33, floor34, floor35, false);
                    BtnSetup(b[10], b[11], anchorX + 2, anchorY - 3, floor36, floor37, floor38, floor39, floor40, floor41, floor42, false);
                    BtnSetup(b[12], b[13], anchorX + 3, anchorY - 3, floor43, floor44, floor45, floor46, floor47, floor48, floor49, false);
                    */
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
                MyLog.LogEvent("Map", "ClearingGrid: " + ex.Message.ToString());
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message.ToString(), "You are not meant to understand this.");
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

            string itemID = Utilities.precedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.precedingZeros(HexTextbox.Text, 8);
            string flag2 = Utilities.precedingZeros(FlagTextbox.Text, 2);

            DisableBtn();

            Thread fillRemainThread = new(delegate () { FillRemain(itemID, itemData, flag2); });
            fillRemainThread.Start();
        }

        private void FillRemain(string itemID, string itemData, string flag2)
        {
            ShowMapWait(14, "Filling Empty Tiles...");

            try
            {
                byte[][] b = new byte[14][];

                UInt32 address = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)));
                byte[] readFloor = Utilities.ReadByteArray8(s, address, 0x4E70);
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

                int emptyspace = NumOfEmpty(curFloor, ref isEmpty);

                FillFloor(ref b, curFloor, isEmpty, itemID, itemData, flag2);

                UInt32 address1;
                UInt32 address2;
                UInt32 address3;
                UInt32 address4;
                UInt32 address5;
                UInt32 address6;
                UInt32 address7;

                if (layer1Btn.Checked)
                {
                    address1 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)));
                    address2 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 2)) + (0x10 * (anchorY - 3)));
                    address3 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 1)) + (0x10 * (anchorY - 3)));
                    address4 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 0)) + (0x10 * (anchorY - 3)));
                    address5 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 1)) + (0x10 * (anchorY - 3)));
                    address6 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 2)) + (0x10 * (anchorY - 3)));
                    address7 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 3)) + (0x10 * (anchorY - 3)));
                }
                else if (layer2Btn.Checked)
                {
                    address1 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address2 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 2)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address3 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 1)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address4 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 0)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address5 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 1)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address6 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 2)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address7 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 3)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                }
                else
                    return;

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
                            this.Invoke((MethodInvoker)delegate
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

                Utilities.dropColumn(s, usb, address1, address1 + 0x600, b[0], b[1], ref counter);
                Utilities.dropColumn(s, usb, address2, address2 + 0x600, b[2], b[3], ref counter);
                Utilities.dropColumn(s, usb, address3, address3 + 0x600, b[4], b[5], ref counter);
                Utilities.dropColumn(s, usb, address4, address4 + 0x600, b[6], b[7], ref counter);
                Utilities.dropColumn(s, usb, address5, address5 + 0x600, b[8], b[9], ref counter);
                Utilities.dropColumn(s, usb, address6, address6 + 0x600, b[10], b[11], ref counter);
                Utilities.dropColumn(s, usb, address7, address7 + 0x600, b[12], b[13], ref counter);

                this.Invoke((MethodInvoker)delegate
                {
                    /*
                    BtnSetup(b[0], b[1], anchorX - 3, anchorY - 3, floor1, floor2, floor3, floor4, floor5, floor6, floor7, false);
                    BtnSetup(b[2], b[3], anchorX - 2, anchorY - 3, floor8, floor9, floor10, floor11, floor12, floor13, floor14, false);
                    BtnSetup(b[4], b[5], anchorX - 1, anchorY - 3, floor15, floor16, floor17, floor18, floor19, floor20, floor21, false);
                    BtnSetup(b[6], b[7], anchorX - 0, anchorY - 3, floor22, floor23, floor24, floor25, floor26, floor27, floor28, false);
                    BtnSetup(b[8], b[9], anchorX + 1, anchorY - 3, floor29, floor30, floor31, floor32, floor33, floor34, floor35, false);
                    BtnSetup(b[10], b[11], anchorX + 2, anchorY - 3, floor36, floor37, floor38, floor39, floor40, floor41, floor42, false);
                    BtnSetup(b[12], b[13], anchorX + 3, anchorY - 3, floor43, floor44, floor45, floor46, floor47, floor48, floor49, false);
                    */
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
                MyLog.LogEvent("Map", "FillRemain: " + ex.Message.ToString());
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message.ToString(), " The valiant knights of programming who toil away, without rest,");
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
                    savepath = Directory.GetCurrentDirectory() + @"\save";
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

                UInt32 address = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)));

                byte[] b = Utilities.ReadByteArray8(s, address, 0x4E70);
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
                MyLog.LogEvent("Map", "Save: " + ex.Message.ToString());
                return;
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
                    Filter = "New Horizons Grid (*.nhg)|*.nhg|New Horizons Inventory(*.nhi) | *.nhi|All files (*.*)|*.*",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                string savepath;

                if (config.AppSettings.Settings["LastLoad"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + @"\save";
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

                if (file.FileName.Contains(".nhi"))
                    nhi = true;
                else
                    nhi = false;

                DisableBtn();

                btnToolTip.RemoveAll();
                Thread LoadThread = new(delegate () { LoadFloor(data, nhi); });
                LoadThread.Start();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "Load: " + ex.Message.ToString());
                return;
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

                    UInt32 address = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)));

                    byte[] readFloor = Utilities.ReadByteArray8(s, address, 0x4E70);
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
                            if (sound)
                                System.Media.SystemSounds.Asterisk.Play();
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

                UInt32 address1;
                UInt32 address2;
                UInt32 address3;
                UInt32 address4;
                UInt32 address5;
                UInt32 address6;
                UInt32 address7;

                if (layer1Btn.Checked)
                {
                    address1 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)));
                    address2 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 2)) + (0x10 * (anchorY - 3)));
                    address3 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 1)) + (0x10 * (anchorY - 3)));
                    address4 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 0)) + (0x10 * (anchorY - 3)));
                    address5 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 1)) + (0x10 * (anchorY - 3)));
                    address6 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 2)) + (0x10 * (anchorY - 3)));
                    address7 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 3)) + (0x10 * (anchorY - 3)));
                }
                else if (layer2Btn.Checked)
                {
                    address1 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address2 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 2)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address3 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 1)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address4 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX - 0)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address5 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 1)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address6 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 2)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                    address7 = (UInt32)(Utilities.mapZero + (0xC00 * (anchorX + 3)) + (0x10 * (anchorY - 3))) + Utilities.mapSize;
                }
                else
                    return;

                List<Task> tasks = new()
                {
                    Task.Run(() => Utilities.dropColumn2(s, usb, address1, address1 + 0x600, b[0], b[1])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address2, address2 + 0x600, b[2], b[3])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address3, address3 + 0x600, b[4], b[5])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address4, address4 + 0x600, b[6], b[7])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address5, address5 + 0x600, b[8], b[9])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address6, address6 + 0x600, b[10], b[11])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address7, address7 + 0x600, b[12], b[13]))
                };

                await Task.WhenAll(tasks);

                this.Invoke((MethodInvoker)delegate
                {
                    /*
                    BtnSetup(b[0], b[1], anchorX - 3, anchorY - 3, floor1, floor2, floor3, floor4, floor5, floor6, floor7, false);
                    BtnSetup(b[2], b[3], anchorX - 2, anchorY - 3, floor8, floor9, floor10, floor11, floor12, floor13, floor14, false);
                    BtnSetup(b[4], b[5], anchorX - 1, anchorY - 3, floor15, floor16, floor17, floor18, floor19, floor20, floor21, false);
                    BtnSetup(b[6], b[7], anchorX - 0, anchorY - 3, floor22, floor23, floor24, floor25, floor26, floor27, floor28, false);
                    BtnSetup(b[8], b[9], anchorX + 1, anchorY - 3, floor29, floor30, floor31, floor32, floor33, floor34, floor35, false);
                    BtnSetup(b[10], b[11], anchorX + 2, anchorY - 3, floor36, floor37, floor38, floor39, floor40, floor41, floor42, false);
                    BtnSetup(b[12], b[13], anchorX + 3, anchorY - 3, floor43, floor44, floor45, floor46, floor47, floor48, floor49, false);
                    */
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
                MyLog.LogEvent("Map", "LoadFloor: " + ex.Message.ToString());
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message.ToString(), "I say this: never gonna give you up, never gonna let you down.");
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
            byte[] emptyLeft = Utilities.stringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.stringToByte("FEFF000000000000FEFF000000000000");

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

        private static void FillFloor(ref byte[][] b, byte[] cur, bool[,] isEmpty, string itemID, string itemData, string flag2)
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
                        TransformToFloorItem(ref b[i * 2], ref b[i * 2 + 1], j, itemID, itemData, flag2);
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
            byte[] flag1Bytes = new byte[1];
            byte[] flag2Bytes = new byte[1];
            byte[] dataBytes = new byte[4];

            Buffer.BlockCopy(item, 0x0, slotBytes, 0, 2);
            Buffer.BlockCopy(item, 0x3, flag1Bytes, 0, 1);
            Buffer.BlockCopy(item, 0x2, flag2Bytes, 0, 1);
            Buffer.BlockCopy(item, 0x4, dataBytes, 0, 4);

            string itemID = Utilities.flip(Utilities.ByteToHexString(slotBytes));
            string itemData = Utilities.flip(Utilities.ByteToHexString(dataBytes));
            string flag1 = Utilities.ByteToHexString(flag1Bytes);
            string flag2 = "20";

            byte[] dropItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
            byte[] dropItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

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

        private static void TransformToFloorItem(ref byte[] b1, ref byte[] b2, int slot, string itemID, string itemData, string flag2)
        {
            string flag1 = "00";

            byte[] dropItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
            byte[] dropItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

            Buffer.BlockCopy(dropItemLeft, 0, b1, slot * 0x10, 16);
            Buffer.BlockCopy(dropItemRight, 0, b2, slot * 0x10, 16);
        }

        private void UpdataData(int x, int y, string itemID, string itemData, string flag1, string flag2)
        {
            byte[] Left = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
            byte[] Right = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

            if (layer1Btn.Checked)
            {
                Buffer.BlockCopy(Left, 0, Layer1, x * 0xC00 + y * 0x10, 16);
                Buffer.BlockCopy(Right, 0, Layer1, x * 0xC00 + 0x600 + y * 0x10, 16);
                miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer1);
            }
            else if (layer2Btn.Checked)
            {
                Buffer.BlockCopy(Left, 0, Layer2, x * 0xC00 + y * 0x10, 16);
                Buffer.BlockCopy(Right, 0, Layer2, x * 0xC00 + 0x600 + y * 0x10, 16);
                miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer2);
            }
        }

        private void UpdataData(int x, int y)
        {
            byte[] Left = Utilities.stringToByte(Utilities.buildDropStringLeft("FFFE", "00000000", "00", "00", true));
            byte[] Right = Utilities.stringToByte(Utilities.buildDropStringRight("FFFE", true));

            if (layer1Btn.Checked)
            {
                Buffer.BlockCopy(Left, 0, Layer1, x * 0xC00 + y * 0x10, 16);
                Buffer.BlockCopy(Right, 0, Layer1, x * 0xC00 + 0x600 + y * 0x10, 16);
                miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer1);
            }
            else if (layer2Btn.Checked)
            {
                Buffer.BlockCopy(Left, 0, Layer2, x * 0xC00 + y * 0x10, 16);
                Buffer.BlockCopy(Right, 0, Layer2, x * 0xC00 + 0x600 + y * 0x10, 16);
                miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer2);
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
                        Buffer.BlockCopy(newData[i * 2], 0, Layer1, (x - 3 + i) * 0xC00 + (y - 3) * 0x10, newData[i * 2].Length);
                        Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer1, (x - 3 + i) * 0xC00 + 0x600 + (y - 3) * 0x10, newData[i * 2 + 1].Length);
                    }
                    else if (layer2Btn.Checked)
                    {
                        Buffer.BlockCopy(newData[i * 2], 0, Layer2, (x - 3 + i) * 0xC00 + (y - 3) * 0x10, newData[i * 2].Length);
                        Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer2, (x - 3 + i) * 0xC00 + 0x600 + (y - 3) * 0x10, newData[i * 2 + 1].Length);
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
                            Buffer.BlockCopy(newData[i * 2], 0, Layer1, (x + i) * 0xC00 + (y) * 0x10, newData[i * 2].Length);
                            Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer1, (x + i) * 0xC00 + 0x600 + (y) * 0x10, newData[i * 2 + 1].Length);
                        }
                        else if (layer2Btn.Checked)
                        {
                            Buffer.BlockCopy(newData[i * 2], 0, Layer2, (x + i) * 0xC00 + (y) * 0x10, newData[i * 2].Length);
                            Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer2, (x + i) * 0xC00 + 0x600 + (y) * 0x10, newData[i * 2 + 1].Length);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < newData.Length / 2; i++)
                    {
                        if (layer1Btn.Checked)
                        {
                            Buffer.BlockCopy(newData[i * 2], 0, Layer1, (x - i) * 0xC00 + (y) * 0x10, newData[i * 2].Length);
                            Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer1, (x - i) * 0xC00 + 0x600 + (y) * 0x10, newData[i * 2 + 1].Length);
                        }
                        else if (layer2Btn.Checked)
                        {
                            Buffer.BlockCopy(newData[i * 2], 0, Layer2, (x - i) * 0xC00 + (y) * 0x10, newData[i * 2].Length);
                            Buffer.BlockCopy(newData[i * 2 + 1], 0, Layer2, (x - i) * 0xC00 + 0x600 + (y) * 0x10, newData[i * 2 + 1].Length);
                        }
                    }
                }
            }

            if (layer1Btn.Checked)
            {
                miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer1);
            }
            else if (layer2Btn.Checked)
            {
                miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer2);
            }
        }

        private void UpdataData(byte[] newLayer)
        {
            if (layer1Btn.Checked)
            {
                Layer1 = newLayer;
                miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer1);
            }
            else if (layer2Btn.Checked)
            {
                Layer2 = newLayer;
                miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer2);
            }
        }

        #endregion

        #region Layer
        private void Layer1Btn_Click(object sender, EventArgs e)
        {
            if (Layer1 == null)
                return;
            bulkSpawnBtn.Enabled = true;
            saveBtn.Enabled = true;
            loadBtn.Enabled = true;
            fillRemainBtn.Enabled = true;

            miniMapBox.BackgroundImage = null;
            miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer1);
            btnToolTip.RemoveAll();
            DisplayAnchor(GetMapColumns(anchorX, anchorY));
            //ResetBtnColor();
        }

        private void Layer2Btn_Click(object sender, EventArgs e)
        {
            if (Layer2 == null)
                return;
            //bulkSpawnBtn.Enabled = false;
            saveBtn.Enabled = false;
            loadBtn.Enabled = false;
            fillRemainBtn.Enabled = false;

            miniMapBox.BackgroundImage = null;
            miniMapBox.BackgroundImage = MiniMap.RefreshItemMap(Layer2);
            btnToolTip.RemoveAll();
            DisplayAnchor(GetMapColumns(anchorX, anchorY));
            //ResetBtnColor();
        }
        #endregion

        #region Variation
        private void Map_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyLog.LogEvent("Map", "Form Closed");
            this.CloseForm();
            if (selection != null)
            {
                selection.Close();
                selection = null;
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
            selection = new variation(115);
            selection.SendVariationData += Selection_sendVariationData;
            selection.Show(this);
            selection.Location = new System.Drawing.Point(this.Location.X + 533, this.Location.Y + 660);
            string id = Utilities.precedingZeros(selectedItem.fillItemID(), 4);
            string value = Utilities.precedingZeros(selectedItem.fillItemData(), 8);
            UInt16 IntId = Convert.ToUInt16("0x" + id, 16);
            if (ItemAttr.hasFenceWithVariation(IntId))  // Fence Variation
            {
                selection.ReceiveID(Utilities.precedingZeros(selectedItem.fillItemID(), 4), languageSetting, value);
            }
            else if (id == "315A" || id == "1618" || id == "342F")
            {
                selection.ReceiveID(Utilities.turn2bytes(selectedItem.fillItemData()), languageSetting);
            }
            else
            {
                selection.ReceiveID(Utilities.precedingZeros(selectedItem.fillItemID(), 4), languageSetting);
            }
            variationBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
        }

        private void Selection_sendVariationData(inventorySlot item, int type)
        {
            if (type == 0) //Left click
            {
                selectedItem.setup(item);
                //updateSelectedItemInfo(selectedItem.displayItemName(), selectedItem.displayItemID(), selectedItem.displayItemData());
                IdTextbox.Text = Utilities.precedingZeros(selectedItem.fillItemID(), 4);
                HexTextbox.Text = Utilities.precedingZeros(selectedItem.fillItemData(), 8);
            }
            else if (type == 1) // Right click
            {
                if (IdTextbox.Text == "315A" || IdTextbox.Text == "1618" || IdTextbox.Text == "342F")
                {
                    string count = TranslateVariationValue(item.fillItemData()) + Utilities.precedingZeros(item.fillItemID(), 4);
                    HexTextbox.Text = count;
                    selectedItem.setup(GetNameFromID(Utilities.turn2bytes(IdTextbox.Text), source), Convert.ToUInt16("0x" + IdTextbox.Text, 16), Convert.ToUInt32("0x" + count, 16), GetImagePathFromID(Utilities.turn2bytes(IdTextbox.Text), source), true, item.getPath(), selectedItem.getFlag1(), selectedItem.getFlag2());
                }
            }
        }

        private void CloseVariationMenu()
        {
            if (selection != null)
            {
                selection.Dispose();
                selection = null;
                variationBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
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
                return Utilities.precedingZeros(input, 4);
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

            output = Utilities.precedingZeros((firstHalf + secondHalf).ToString("X"), 4);
            return output;
        }

        private void Map_LocationChanged(object sender, EventArgs e)
        {
            if (selection != null)
            {
                selection.Location = new System.Drawing.Point(this.Location.X + 533, this.Location.Y + 660);
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

                _ = DisplayAnchorAsync();
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

                _ = DisplayAnchorAsync();
            }
        }

        private void SaveTopngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiniMap big = new(Layer1, Acre, Building, Terrain, 4);
            SaveFileDialog file = new()
            {
                Filter = "Portable Network Graphics (*.png)|*.png",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            string savepath;

            if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + @"\save";
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
            Invoke((MethodInvoker)delegate
            {
                if (counter <= MapProgressBar.Maximum)
                    MapProgressBar.Value = counter;
                else
                    MapProgressBar.Value = MapProgressBar.Maximum;
            });
        }

        private void ShowMapWait(int size, string msg = "")
        {
            this.Invoke((MethodInvoker)delegate
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
            this.Invoke((MethodInvoker)delegate
            {
                PleaseWaitPanel.Visible = false;
                ProgressTimer.Stop();
            });
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
                byte[] Coordinate = Utilities.getCoordinate(s, usb);
                int x = BitConverter.ToInt32(Coordinate, 0);
                int y = BitConverter.ToInt32(Coordinate, 4);

                anchorX = x - 0x24;
                anchorY = y - 0x18;

                if (anchorX < 3 || anchorY < 3 || anchorX > 108 || anchorY > 92)
                {
                    anchorX = 56;
                    anchorY = 48;
                }

                DisplayAnchor(GetMapColumns(anchorX, anchorY));

                xCoordinate.Text = anchorX.ToString();
                yCoordinate.Text = anchorY.ToString();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "getCoordinate: " + ex.Message.ToString());
                MyMessageBox.Show(ex.Message.ToString(), "Weed Effect !");

                anchorX = 56;
                anchorY = 48;

                DisplayAnchor(GetMapColumns(anchorX, anchorY));

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
            if (bulk == null)
            {
                if (layer1Btn.Checked)
                    bulk = new BulkSpawn(s, usb, Layer1, Layer2, Acre, Building, Terrain, anchorX, anchorY, this, ignore, sound, true);
                else
                    bulk = new BulkSpawn(s, usb, Layer1, Layer2, Acre, Building, Terrain, anchorX, anchorY, this, ignore, sound, false);
            }
            bulk.StartPosition = FormStartPosition.CenterParent;
            bulk.ShowDialog();
        }

        private void WeedsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MyMessageBox.Show("Are you sure you want to remove all weeds on your island (Layer 1 only)?", "Oh No! Not the Weeds!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;

            byte[] emptyLeft = Utilities.stringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.stringToByte("FEFF000000000000FEFF000000000000");

            byte[] processLayer = Layer1;
            Boolean[] change = new Boolean[56];
            byte[] tempID = new byte[2];
            ushort itemID;
            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 96; j++)
                {
                    Buffer.BlockCopy(Layer1, i * 0x1800 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isWeed(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0x600 + j * 0x10, 16);
                        change[i] = true;
                    }

                    Buffer.BlockCopy(Layer1, i * 0x1800 + 0xC00 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isWeed(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + 0xC00 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0xC00 + 0x600 + j * 0x10, 16);
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

            byte[] emptyLeft = Utilities.stringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.stringToByte("FEFF000000000000FEFF000000000000");

            byte[] processLayer = Layer1;
            Boolean[] change = new Boolean[56];
            byte[] tempID = new byte[2];
            ushort itemID;
            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 96; j++)
                {
                    Buffer.BlockCopy(Layer1, i * 0x1800 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.hasGenetics(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0x600 + j * 0x10, 16);
                        change[i] = true;
                    }

                    Buffer.BlockCopy(Layer1, i * 0x1800 + 0xC00 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.hasGenetics(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + 0xC00 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0xC00 + 0x600 + j * 0x10, 16);
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

            byte[] emptyLeft = Utilities.stringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.stringToByte("FEFF000000000000FEFF000000000000");

            byte[] processLayer = Layer1;
            Boolean[] change = new Boolean[56];
            byte[] tempID = new byte[2];
            ushort itemID;
            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 96; j++)
                {
                    Buffer.BlockCopy(Layer1, i * 0x1800 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isTree(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0x600 + j * 0x10, 16);
                        change[i] = true;
                    }

                    Buffer.BlockCopy(Layer1, i * 0x1800 + 0xC00 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isTree(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + 0xC00 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0xC00 + 0x600 + j * 0x10, 16);
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

            byte[] emptyLeft = Utilities.stringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.stringToByte("FEFF000000000000FEFF000000000000");

            byte[] processLayer = Layer1;
            Boolean[] change = new Boolean[56];
            byte[] tempID = new byte[2];
            ushort itemID;
            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 96; j++)
                {
                    Buffer.BlockCopy(Layer1, i * 0x1800 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isBush(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0x600 + j * 0x10, 16);
                        change[i] = true;
                    }

                    Buffer.BlockCopy(Layer1, i * 0x1800 + 0xC00 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isBush(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + 0xC00 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0xC00 + 0x600 + j * 0x10, 16);
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

            byte[] emptyLeft = Utilities.stringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.stringToByte("FEFF000000000000FEFF000000000000");

            byte[] processLayer = Layer1;
            Boolean[] change = new Boolean[56];
            byte[] tempID = new byte[2];
            ushort itemID;
            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 96; j++)
                {
                    Buffer.BlockCopy(Layer1, i * 0x1800 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isPlacedFence(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0x600 + j * 0x10, 16);
                        change[i] = true;
                    }

                    Buffer.BlockCopy(Layer1, i * 0x1800 + 0xC00 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isPlacedFence(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + 0xC00 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0xC00 + 0x600 + j * 0x10, 16);
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

            byte[] emptyLeft = Utilities.stringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.stringToByte("FEFF000000000000FEFF000000000000");

            byte[] processLayer = Layer1;
            Boolean[] change = new Boolean[56];
            byte[] tempID = new byte[2];
            ushort itemID;
            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 96; j++)
                {
                    Buffer.BlockCopy(Layer1, i * 0x1800 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isShell(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0x600 + j * 0x10, 16);
                        change[i] = true;
                    }

                    Buffer.BlockCopy(Layer1, i * 0x1800 + 0xC00 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isShell(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + 0xC00 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0xC00 + 0x600 + j * 0x10, 16);
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

            byte[] emptyLeft = Utilities.stringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.stringToByte("FEFF000000000000FEFF000000000000");

            byte[] processLayer = Layer1;
            Boolean[] change = new Boolean[56];
            byte[] tempID = new byte[2];
            ushort itemID;
            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 96; j++)
                {
                    Buffer.BlockCopy(Layer1, i * 0x1800 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (itemID.Equals(0x16A2))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0x600 + j * 0x10, 16);
                        change[i] = true;
                    }

                    Buffer.BlockCopy(Layer1, i * 0x1800 + 0xC00 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (itemID.Equals(0x16A2))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + 0xC00 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0xC00 + 0x600 + j * 0x10, 16);
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

            byte[] emptyLeft = Utilities.stringToByte("FEFF000000000000FEFF000000000000");
            byte[] emptyRight = Utilities.stringToByte("FEFF000000000000FEFF000000000000");

            byte[] processLayer = Layer1;
            Boolean[] change = new Boolean[56];
            byte[] tempID = new byte[2];
            ushort itemID;
            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 96; j++)
                {
                    Buffer.BlockCopy(Layer1, i * 0x1800 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isStone(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0x600 + j * 0x10, 16);
                        change[i] = true;
                    }

                    Buffer.BlockCopy(Layer1, i * 0x1800 + 0xC00 + j * 0x10, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (ItemAttr.isStone(itemID))
                    {
                        Buffer.BlockCopy(emptyLeft, 0, processLayer, i * 0x1800 + 0xC00 + j * 0x10, 16);
                        Buffer.BlockCopy(emptyRight, 0, processLayer, i * 0x1800 + 0xC00 + 0x600 + j * 0x10, 16);
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
                ProcessLayer(Layer1, Utilities.mapZero);
            }
            else
            {
                dialogResult = MyMessageBox.Show("Are you sure you want to remove all dropped/placed item on your island (Layer 2 only)?", "Is everything a joke to you ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                    return;
                ProcessLayer(Layer2, Utilities.mapZero + Utilities.mapSize);
            }
        }
        private void ProcessLayer(byte[] Layer, long Address)
        {
            byte[] empty = Utilities.stringToByte("FEFF000000000000");

            byte[] processLayer = Layer;
            Boolean[] HasChange = new Boolean[56];

            byte[] tempID = new byte[2];
            ushort itemID;
            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 96 * 8; j++)
                {
                    Buffer.BlockCopy(Layer, i * 0x1800 + j * 0x8, tempID, 0, 2);
                    itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(tempID)), 16);
                    if (!itemID.Equals(0xFFFE))
                    {
                        Buffer.BlockCopy(empty, 0, processLayer, i * 0x1800 + j * 0x8, 8);
                        HasChange[i] = true;
                    }
                }
            }

            DisableBtn();

            Thread renewThread = new(delegate () { Renew(processLayer, HasChange, Address); });
            renewThread.Start();
        }

        private void Renew(byte[] newLayer, Boolean[] change)
        {
            int num = NumOfWrite(change);
            if (num == 0)
                return;

            ShowMapWait(num * 2, "Removing Item...");

            try
            {
                Debug.Print("Length :" + num + " Time : " + (num + 3));

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
                            this.Invoke((MethodInvoker)delegate
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

                for (int i = 0; i < 56; i++)
                {
                    if (change[i])
                    {
                        byte[] column = new byte[0x1800];
                        Buffer.BlockCopy(newLayer, i * 0x1800, column, 0, 0x1800);
                        Utilities.SendByteArray8(s, Utilities.mapZero + (i * 0x1800), column, 0x1800, ref counter);
                        Utilities.SendByteArray8(s, Utilities.mapZero + (i * 0x1800) + Utilities.mapOffset, column, 0x1800, ref counter);
                    }
                }

                this.Invoke((MethodInvoker)delegate
                {
                    UpdataData(newLayer);
                    MoveAnchor(anchorX, anchorY);
                    ResetBtnColor();
                });

                this.Invoke((MethodInvoker)delegate
                {
                    EnableBtn();
                });

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "Renew: " + ex.Message.ToString());
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message.ToString(), "Fixing our most awful code. To you, true saviors, kings of men.");
            }

            HideMapWait();
        }

        private void Renew(byte[] newLayer, Boolean[] change, long address)
        {
            int num = NumOfWrite(change);
            if (num == 0)
                return;

            ShowMapWait(num * 2, "Removing Item...");

            try
            {
                Debug.Print("Length :" + num + " Time : " + (num + 3));

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
                            this.Invoke((MethodInvoker)delegate
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

                for (int i = 0; i < 56; i++)
                {
                    if (change[i])
                    {
                        byte[] column = new byte[0x1800];
                        Buffer.BlockCopy(newLayer, i * 0x1800, column, 0, 0x1800);
                        Utilities.SendByteArray8(s, address + (i * 0x1800), column, 0x1800, ref counter);
                        Utilities.SendByteArray8(s, address + (i * 0x1800) + Utilities.mapOffset, column, 0x1800, ref counter);
                    }
                }

                this.Invoke((MethodInvoker)delegate
                {
                    UpdataData(newLayer);
                    MoveAnchor(anchorX, anchorY);
                    ResetBtnColor();
                });

                this.Invoke((MethodInvoker)delegate
                {
                    EnableBtn();
                });

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "Renew: " + ex.Message.ToString());
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message.ToString(), "Fixing our most awful code. To you, true saviors, kings of men.");
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

        #region Debug
        private void Ext_Click(object sender, EventArgs e)
        {
            if (IdTextbox.Text == "" || HexTextbox.Text == "" || FlagTextbox.Text == "")
            {
                return;
            }

            if (Control.ModifierKeys == Keys.Control)
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
                MessageBox.Show("Please select a slot!");
                return;
            }

            long address;

            if (layer1Btn.Checked)
            {
                address = GetAddress(selectedButton.mapX, selectedButton.mapY);
            }
            else if (layer2Btn.Checked)
            {
                address = (GetAddress(selectedButton.mapX, selectedButton.mapY) + Utilities.mapSize);
            }
            else
                return;

            string itemID = Utilities.precedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.precedingZeros(HexTextbox.Text, 8);
            string flag2 = Utilities.precedingZeros(FlagTextbox.Text, 2);
            string flag1 = selectedItem.getFlag1();

            DisableBtn();

            Thread spawnThread = new(delegate () { ExtDropItem(address, itemID, itemData, flag1, flag2, selectedButton); });
            spawnThread.Start();
        }

        private void ExtDropItem(long address, string itemID, string itemData, string flag1, string flag2, FloorSlot btn)
        {
            ShowMapWait(2, "Spawning Item...");

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
                        this.Invoke((MethodInvoker)delegate
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

            Utilities.ExtDropItem(s, usb, address, itemID, itemData, flag1, flag2);

            this.Invoke((MethodInvoker)delegate
            {
                //SetBtn(btn, itemID, itemData, "0000FFFE", "00000000", "0000FFFE", "00000000", "0000FFFE", "00000000", "00", flag2);
                UpdataData(btn.mapX, btn.mapY, itemID, itemData, flag1, flag2);
                UpdateBtn(btn);
                ResetBtnColor();
                EnableBtn();
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();
        }

        #endregion

        #region AutoSave

        private int NextAutosave()
        {
            try
            {
                byte[] b = Utilities.getSaving(s, usb);

                if (b == null)
                    throw new NullReferenceException("Save");

                byte[] currentFrame = new byte[4];
                byte[] lastFrame = new byte[4];
                Buffer.BlockCopy(b, 12, currentFrame, 0, 4);
                Buffer.BlockCopy(b, 16, lastFrame, 0, 4);

                int currentFrameStr = Convert.ToInt32("0x" + Utilities.flip(Utilities.ByteToHexString(currentFrame)), 16);
                int lastFrameStr = Convert.ToInt32("0x" + Utilities.flip(Utilities.ByteToHexString(lastFrame)), 16);

                return (((0x1518 - (currentFrameStr - lastFrameStr))) / 30);
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Map", "NextAutoSave: " + ex.Message.ToString());
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message.ToString() + "\nThe connection to the Switch ended.\n\nDid the Switch enter sleep mode?", "Ugandan Knuckles: \"Oh No!\"", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    try
                    {

                        if (anchorX < 0 || anchorY < 0)
                        {
                            return;
                        }

                        if (IdTextbox.Text == "" || HexTextbox.Text == "" || FlagTextbox.Text == "")
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

                        string itemID = Utilities.precedingZeros(IdTextbox.Text, 4);
                        string itemData = Utilities.precedingZeros(HexTextbox.Text, 8);
                        string flag2 = Utilities.precedingZeros(FlagTextbox.Text, 2);

                        Thread ReplaceThread = new(delegate () { ReplaceGrid(address, btn, itemID, itemData, flag2); });
                        ReplaceThread.Start();
                    }
                    catch (Exception ex)
                    {
                        MyLog.LogEvent("Map", "Replace: " + ex.Message.ToString());
                        return;
                    }
                }
            }
        }

        private async void ReplaceGrid(long address, FloorSlot btn, string itemID, string itemData, string flag2)
        {
            ShowMapWait(14, "Replacing Items...");

            try
            {
                byte[][] b = new byte[14][];

                UInt32 currentColumn = (UInt32)(address + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)));

                byte[] readFloor = Utilities.ReadByteArray8(s, currentColumn, 0x4E70);
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

                ReplaceItem(ref b, curFloor, itemID, itemData, flag2, btn);

                UInt32 address1 = (UInt32)(address + (0xC00 * (anchorX - 3)) + (0x10 * (anchorY - 3)));
                UInt32 address2 = (UInt32)(address + (0xC00 * (anchorX - 2)) + (0x10 * (anchorY - 3)));
                UInt32 address3 = (UInt32)(address + (0xC00 * (anchorX - 1)) + (0x10 * (anchorY - 3)));
                UInt32 address4 = (UInt32)(address + (0xC00 * (anchorX - 0)) + (0x10 * (anchorY - 3)));
                UInt32 address5 = (UInt32)(address + (0xC00 * (anchorX + 1)) + (0x10 * (anchorY - 3)));
                UInt32 address6 = (UInt32)(address + (0xC00 * (anchorX + 2)) + (0x10 * (anchorY - 3)));
                UInt32 address7 = (UInt32)(address + (0xC00 * (anchorX + 3)) + (0x10 * (anchorY - 3)));

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
                            this.Invoke((MethodInvoker)delegate
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

                List<Task> tasks = new()
                {
                    Task.Run(() => Utilities.dropColumn2(s, usb, address1, address1 + 0x600, b[0], b[1])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address2, address2 + 0x600, b[2], b[3])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address3, address3 + 0x600, b[4], b[5])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address4, address4 + 0x600, b[6], b[7])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address5, address5 + 0x600, b[8], b[9])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address6, address6 + 0x600, b[10], b[11])),
                    Task.Run(() => Utilities.dropColumn2(s, usb, address7, address7 + 0x600, b[12], b[13]))
                };

                await Task.WhenAll(tasks);

                this.Invoke((MethodInvoker)delegate
                {
                    /*
                    BtnSetup(b[0], b[1], anchorX - 3, anchorY - 3, floor1, floor2, floor3, floor4, floor5, floor6, floor7, false);
                    BtnSetup(b[2], b[3], anchorX - 2, anchorY - 3, floor8, floor9, floor10, floor11, floor12, floor13, floor14, false);
                    BtnSetup(b[4], b[5], anchorX - 1, anchorY - 3, floor15, floor16, floor17, floor18, floor19, floor20, floor21, false);
                    BtnSetup(b[6], b[7], anchorX - 0, anchorY - 3, floor22, floor23, floor24, floor25, floor26, floor27, floor28, false);
                    BtnSetup(b[8], b[9], anchorX + 1, anchorY - 3, floor29, floor30, floor31, floor32, floor33, floor34, floor35, false);
                    BtnSetup(b[10], b[11], anchorX + 2, anchorY - 3, floor36, floor37, floor38, floor39, floor40, floor41, floor42, false);
                    BtnSetup(b[12], b[13], anchorX + 3, anchorY - 3, floor43, floor44, floor45, floor46, floor47, floor48, floor49, false);
                    */
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
                MyLog.LogEvent("Map", "ReplaceItem: " + ex.Message.ToString());
                NextSaveTimer.Stop();
                MyMessageBox.Show(ex.Message.ToString(), "I say this: Never gonna run around and desert you.");
            }

            HideMapWait();
        }

        private static void ReplaceItem(ref byte[][] b, byte[] cur, string itemID, string itemData, string flag2, FloorSlot btn)
        {
            byte[] tempLeft = new byte[16];
            byte[] tempRight = new byte[16];

            string targetLeft = Utilities.buildDropStringLeft(Utilities.precedingZeros(btn.itemID.ToString("X"), 4), Utilities.precedingZeros(btn.itemData.ToString("X"), 8), btn.flag1, btn.flag2);
            string targetRight = Utilities.buildDropStringRight(Utilities.precedingZeros(btn.itemID.ToString("X"), 4));

            byte[] resultLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, "00", flag2));
            byte[] resultRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

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
                            Buffer.BlockCopy(Layer1, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY))), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer1, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY)) + 0x600), SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                            if (SavedArea[i * 2][0x10 * j] != 0xFE || SavedArea[i * 2][0x10 * j + 1] != 0xFF)
                                SavedArea[i * 2][0x10 * j + 2] = 0x20;
                        }
                        else
                        {
                            Buffer.BlockCopy(Layer2, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY))), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer2, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY)) + 0x600), SavedArea[i * 2 + 1], 0x10 * j, 0x10);
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

                Thread pasteAreaThread = new(delegate () { PasteArea(address, TopLeftX, TopLeftY, numberOfColumn); });
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
                                address = GetAddress(btn.mapX, btn.mapY);
                            }
                            else if (layer2Btn.Checked)
                            {
                                address = (GetAddress(btn.mapX, btn.mapY) + Utilities.mapSize);
                            }
                            else
                                return;

                            DisableBtn();

                            btnToolTip.RemoveAll();

                            string itemID = Utilities.precedingZeros(btn.itemID.ToString("X"), 4);
                            string itemData = Utilities.precedingZeros(btn.itemData.ToString("X"), 8);
                            string flag1 = btn.flag1;
                            string flag2 = "20";

                            Thread dropThread = new(delegate () { DropItem(address, itemID, itemData, flag1, flag2, btn); });
                            dropThread.Start();
                        }
                        catch (Exception ex)
                        {
                            MyLog.LogEvent("Map", "Flag20: " + ex.Message.ToString());
                            return;
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
                            Buffer.BlockCopy(Layer1, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY))), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer1, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY)) + 0x600), SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                            SavedArea[i * 2][0x10 * j + 2] = 0x00;
                        }
                        else
                        {
                            Buffer.BlockCopy(Layer2, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY))), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer2, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY)) + 0x600), SavedArea[i * 2 + 1], 0x10 * j, 0x10);
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

                Thread pasteAreaThread = new(delegate () { PasteArea(address, TopLeftX, TopLeftY, numberOfColumn); });
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
                                address = GetAddress(btn.mapX, btn.mapY);
                            }
                            else if (layer2Btn.Checked)
                            {
                                address = (GetAddress(btn.mapX, btn.mapY) + Utilities.mapSize);
                            }
                            else
                                return;

                            DisableBtn();

                            btnToolTip.RemoveAll();

                            string itemID = Utilities.precedingZeros(btn.itemID.ToString("X"), 4);
                            string itemData = Utilities.precedingZeros(btn.itemData.ToString("X"), 8);
                            string flag1 = btn.flag1;
                            string flag2 = "00";

                            Thread dropThread = new(delegate () { DropItem(address, itemID, itemData, flag1, flag2, btn); });
                            dropThread.Start();
                        }
                        catch (Exception ex)
                        {
                            MyLog.LogEvent("Map", "Flag00: " + ex.Message.ToString());
                            return;
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
                            Buffer.BlockCopy(Layer1, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY))), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer1, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY)) + 0x600), SavedArea[i * 2 + 1], 0x10 * j, 0x10);
                            if (SavedArea[i * 2][0x10 * j] != 0xFE || SavedArea[i * 2][0x10 * j + 1] != 0xFF)
                            {
                                if (SavedArea[i * 2][0x10 * j + 2] == 0x00)
                                    SavedArea[i * 2][0x10 * j + 2] = 0x04;
                            }
                        }
                        else
                        {
                            Buffer.BlockCopy(Layer2, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY))), SavedArea[i * 2], 0x10 * j, 0x10);
                            Buffer.BlockCopy(Layer2, (int)((0xC00 * (i + TopLeftX)) + (0x10 * (j + TopLeftY)) + 0x600), SavedArea[i * 2 + 1], 0x10 * j, 0x10);
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

                Thread pasteAreaThread = new(delegate () { PasteArea(address, TopLeftX, TopLeftY, numberOfColumn); });
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

                        if (btn.flag2 != "00")
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
                                address = GetAddress(btn.mapX, btn.mapY);
                            }
                            else if (layer2Btn.Checked)
                            {
                                address = (GetAddress(btn.mapX, btn.mapY) + Utilities.mapSize);
                            }
                            else
                                return;

                            DisableBtn();

                            btnToolTip.RemoveAll();

                            string itemID = Utilities.precedingZeros(btn.itemID.ToString("X"), 4);
                            string itemData = Utilities.precedingZeros(btn.itemData.ToString("X"), 8);
                            string flag1 = btn.flag1;
                            string flag2 = "04";

                            Thread dropThread = new(delegate () { DropItem(address, itemID, itemData, flag1, flag2, btn); });
                            dropThread.Start();
                        }
                        catch (Exception ex)
                        {
                            MyLog.LogEvent("Map", "Flag04: " + ex.Message.ToString());
                            return;
                        }
                    }
                }
            }
        }

        private void PlaceVariationBtn_Click(object sender, EventArgs e)
        {
            if (IdTextbox.Text == "" || HexTextbox.Text == "" || FlagTextbox.Text == "")
            {
                MessageBox.Show("Please select an item!");
                return;
            }

            if (selectedButton == null)
            {
                MessageBox.Show("Please select a slot!");
                return;
            }

            string flag1 = selectedItem.getFlag1();
            string flag2 = Utilities.precedingZeros(FlagTextbox.Text, 2);

            variationList = variation.GetVariationList(IdTextbox.Text, flag1, flag2, HexTextbox.Text);
            byte[][] spawnArea = null;


            if (variationList != null)
            {

                int TopLeftX = selectedButton.mapX;
                int TopLeftY = selectedButton.mapY;
                int row;
                int column;
                int BottomRightX = 0;
                int BottomRightY = 0;

                int main = variationList.GetLength(0);
                int sub = variationList.GetLength(1);

                variationSpawn variationSpawner = new(variationList, Layer1, Acre, Building, Terrain, TopLeftX, TopLeftY, flag2, selectedSize);
                variationSpawner.SendObeySizeEvent += VariationSpawner_SendObeySizeEvent;
                variationSpawner.SendRowAndColumnEvent += VariationSpawner_SendRowAndColumnEvent;
                int result = (int)variationSpawner.ShowDialog(this);

                if (result == 1) // Main
                {
                    BottomRightX = selectedButton.mapX + numOfColumn - 1;
                    row = variationList.GetLength(0);
                    BottomRightY = TopLeftY + row - 1;
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
                    BottomRightX = selectedButton.mapX + numOfColumn - 1;
                    row = variationList.GetLength(1);
                    BottomRightY = TopLeftY + row - 1;
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
                    BottomRightX = selectedButton.mapX + main - 1;
                    row = sub;
                    column = main;
                    BottomRightY = TopLeftY + row - 1;
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
                    BottomRightY = selectedButton.mapY + numOfRow - 1;
                    column = variationList.GetLength(0);
                    BottomRightX = TopLeftX + column - 1;
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
                    BottomRightY = selectedButton.mapY + numOfRow - 1;
                    column = variationList.GetLength(1);
                    BottomRightX = TopLeftX + column - 1;
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
                    BottomRightY = selectedButton.mapY + main - 1;
                    row = main;
                    column = sub;
                    BottomRightX = TopLeftX + column - 1;
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
                return;
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

        private static byte[][] BuildVariationArea(inventorySlot[,] variation, int numberOfRow, int multiple, int mode)
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
                        string itemID = Utilities.precedingZeros(variation[j, 0].fillItemID(), 4);
                        string itemData = Utilities.precedingZeros(variation[j, 0].fillItemData(), 8);
                        string flag1 = Utilities.precedingZeros(variation[j, 0].getFlag1(), 2);
                        string flag2 = Utilities.precedingZeros(variation[j, 0].getFlag2(), 2);

                        byte[] ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                        byte[] ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

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
                        string itemID = Utilities.precedingZeros(variation[0, j].fillItemID(), 4);
                        string itemData = Utilities.precedingZeros(variation[0, j].fillItemData(), 8);
                        string flag1 = Utilities.precedingZeros(variation[0, j].getFlag1(), 2);
                        string flag2 = Utilities.precedingZeros(variation[0, j].getFlag2(), 2);

                        byte[] ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                        byte[] ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

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
                        string itemID = Utilities.precedingZeros(variation[i, j].fillItemID(), 4);
                        string itemData = Utilities.precedingZeros(variation[i, j].fillItemData(), 8);
                        string flag1 = Utilities.precedingZeros(variation[i, j].getFlag1(), 2);
                        string flag2 = Utilities.precedingZeros(variation[i, j].getFlag2(), 2);

                        byte[] ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                        byte[] ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

                        Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }

            return b;
        }

        private byte[][] BuildVariationAreaObeySize(inventorySlot[,] variation, int oldRow, int oldColumn, int mode)
        {
            int sizeOfRow = 16;

            byte[][] b = new byte[newSpawnWidth * 2][];

            for (int i = 0; i < newSpawnWidth * 2; i++)
            {
                b[i] = new byte[newSpawnHeight * sizeOfRow];
            }

            int iterator = 0;
            inventorySlot[] serialList = new inventorySlot[oldRow * oldColumn];

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

            string flag = Utilities.precedingZeros(FlagTextbox.Text, 2);

            for (int i = 0; i < newSpawnWidth; i++)
            {
                for (int j = 0; j < newSpawnHeight; j++)
                {
                    if (i % itemWidth == 0 && j % itemHeight == 0)
                    {
                        if (wallmount && flag != "20")
                        {
                            string itemID = "1618";
                            string itemData = Utilities.translateVariationValue(serialList[iterator].fillItemData()) + Utilities.precedingZeros(serialList[iterator].fillItemID(), 4);
                            string flag1 = Utilities.precedingZeros(serialList[iterator].getFlag1(), 2);
                            string flag2 = Utilities.precedingZeros(serialList[iterator].getFlag2(), 2);

                            ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                            ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));
                            iterator++;
                        }
                        else if (ceiling && flag != "20")
                        {
                            string itemID = "342F";
                            string itemData = Utilities.translateVariationValue(serialList[iterator].fillItemData()) + Utilities.precedingZeros(serialList[iterator].fillItemID(), 4);
                            string flag1 = Utilities.precedingZeros(serialList[iterator].getFlag1(), 2);
                            string flag2 = Utilities.precedingZeros(serialList[iterator].getFlag2(), 2);

                            ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                            ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));
                            iterator++;
                        }
                        else
                        {
                            string itemID = Utilities.precedingZeros(serialList[iterator].fillItemID(), 4);
                            string itemData = Utilities.precedingZeros(serialList[iterator].fillItemData(), 8);
                            string flag1 = Utilities.precedingZeros(serialList[iterator].getFlag1(), 2);
                            string flag2 = Utilities.precedingZeros(serialList[iterator].getFlag2(), 2);

                            ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                            ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));
                            iterator++;
                        }
                    }
                    else
                    {
                        ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft("FFFE", "00000000", "00", "00", true));
                        ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight("FFFE", true));
                    }

                    Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                    Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                }
            }

            return b;
        }

        private static byte[][] BuildVertVariationArea(inventorySlot[,] variation, int numberOfColumn, int multiple, int mode)
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
                        string itemID = Utilities.precedingZeros(variation[i, 0].fillItemID(), 4);
                        string itemData = Utilities.precedingZeros(variation[i, 0].fillItemData(), 8);
                        string flag1 = Utilities.precedingZeros(variation[i, 0].getFlag1(), 2);
                        string flag2 = Utilities.precedingZeros(variation[i, 0].getFlag2(), 2);

                        byte[] ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                        byte[] ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

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
                        string itemID = Utilities.precedingZeros(variation[0, i].fillItemID(), 4);
                        string itemData = Utilities.precedingZeros(variation[0, i].fillItemData(), 8);
                        string flag1 = Utilities.precedingZeros(variation[0, i].getFlag1(), 2);
                        string flag2 = Utilities.precedingZeros(variation[0, i].getFlag2(), 2);

                        byte[] ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                        byte[] ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

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
                        string itemID = Utilities.precedingZeros(variation[j, i].fillItemID(), 4);
                        string itemData = Utilities.precedingZeros(variation[j, i].fillItemData(), 8);
                        string flag1 = Utilities.precedingZeros(variation[j, i].getFlag1(), 2);
                        string flag2 = Utilities.precedingZeros(variation[j, i].getFlag2(), 2);

                        byte[] ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                        byte[] ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));

                        Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                        Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                    }
                }
            }

            return b;
        }

        private byte[][] BuildVertVariationAreaObeySize(inventorySlot[,] variation, int oldRow, int oldColumn, int mode)
        {
            int sizeOfRow = 16;

            byte[][] b = new byte[newSpawnWidth * 2][];

            for (int i = 0; i < newSpawnWidth * 2; i++)
            {
                b[i] = new byte[newSpawnHeight * sizeOfRow];
            }

            int iterator = 0;
            inventorySlot[] serialList = new inventorySlot[oldRow * oldColumn];

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

            string flag = Utilities.precedingZeros(FlagTextbox.Text, 2);

            for (int i = 0; i < newSpawnWidth; i++)
            {
                for (int j = 0; j < newSpawnHeight; j++)
                {
                    if (i % itemWidth == 0 && j % itemHeight == 0)
                    {
                        if (wallmount && flag != "20")
                        {
                            string itemID = "1618";
                            string itemData = Utilities.translateVariationValue(serialList[iterator].fillItemData()) + Utilities.precedingZeros(serialList[iterator].fillItemID(), 4);
                            string flag1 = Utilities.precedingZeros(serialList[iterator].getFlag1(), 2);
                            string flag2 = Utilities.precedingZeros(serialList[iterator].getFlag2(), 2);

                            ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                            ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));
                            iterator++;
                        }
                        else if (ceiling && flag != "20")
                        {
                            string itemID = "342F";
                            string itemData = Utilities.translateVariationValue(serialList[iterator].fillItemData()) + Utilities.precedingZeros(serialList[iterator].fillItemID(), 4);
                            string flag1 = Utilities.precedingZeros(serialList[iterator].getFlag1(), 2);
                            string flag2 = Utilities.precedingZeros(serialList[iterator].getFlag2(), 2);

                            ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                            ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));
                            iterator++;
                        }
                        else
                        {
                            string itemID = Utilities.precedingZeros(serialList[iterator].fillItemID(), 4);
                            string itemData = Utilities.precedingZeros(serialList[iterator].fillItemData(), 8);
                            string flag1 = Utilities.precedingZeros(serialList[iterator].getFlag1(), 2);
                            string flag2 = Utilities.precedingZeros(serialList[iterator].getFlag2(), 2);

                            ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft(itemID, itemData, flag1, flag2));
                            ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight(itemID));
                            iterator++;
                        }
                    }
                    else
                    {
                        ItemLeft = Utilities.stringToByte(Utilities.buildDropStringLeft("FFFE", "00000000", "00", "00", true));
                        ItemRight = Utilities.stringToByte(Utilities.buildDropStringRight("FFFE", true));
                    }

                    Buffer.BlockCopy(ItemLeft, 0, b[i * 2], 0x10 * j, 16);
                    Buffer.BlockCopy(ItemRight, 0, b[i * 2 + 1], 0x10 * j, 16);
                }
            }

            return b;
        }

        private void UpdateVariation()
        {
            if (IdTextbox.Text.Equals(string.Empty) || HexTextbox.Text.Equals(string.Empty))
                return;

            long data = Convert.ToUInt32(HexTextbox.Text.ToString(), 16);
            string hexValue = data.ToString("X");

            string itemID = Utilities.precedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.precedingZeros(hexValue, 8);
            string flag2 = Utilities.precedingZeros(FlagTextbox.Text, 2);

            UInt16 IntId = Convert.ToUInt16("0x" + itemID, 16);
            string front = Utilities.precedingZeros(itemData, 8).Substring(0, 4);
            //string back = Utilities.precedingZeros(itemData, 8).Substring(4, 4);

            if (itemID.Equals("315A") || itemID.Equals("1618") || itemID.Equals("342F"))
            {
                selectedItem.setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source), true, GetImagePathFromID(Utilities.turn2bytes(itemData), source, Convert.ToUInt32("0x" + Utilities.translateVariationValueBack(front), 16)), "00", flag2);
            }
            else if (itemID.Equals("16A2"))
            {
                selectedItem.setup(GetNameFromID(itemID, recipeSource), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(Utilities.turn2bytes(itemData), recipeSource), true, "", "00", flag2);
            }
            else if (ItemAttr.hasFenceWithVariation(IntId))  // Fence Variation
            {
                selectedItem.setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + front, 16)), true, "", "00", flag2);
            }
            else
            {
                selectedItem.setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + itemData, 16)), true, "", "00", flag2);
            }

            if (selection != null)
            {
                //selection.Dispose();
                string id = Utilities.precedingZeros(selectedItem.fillItemID(), 4);
                string value = Utilities.precedingZeros(selectedItem.fillItemData(), 8);

                if (ItemAttr.hasFenceWithVariation(IntId))  // Fence Variation
                {
                    selection.ReceiveID(Utilities.precedingZeros(selectedItem.fillItemID(), 4), languageSetting, value);
                }
                else if (id == "315A" || id == "1618" || id == "342F")
                {
                    selection.ReceiveID(Utilities.turn2bytes(selectedItem.fillItemData()), languageSetting);
                }
                else
                {
                    selection.ReceiveID(Utilities.precedingZeros(selectedItem.fillItemID(), 4), languageSetting);
                }
            }
        }

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
            if (fieldGridView.Columns.Contains(languageSetting))
            {
                HideAllLanguage();
                fieldGridView.Columns[languageSetting].Visible = true;
            }

            itemSearchBox.Text = "Search...";
        }

        private void HideAllLanguage()
        {
            if (fieldGridView.Columns.Contains("id"))
            {
                fieldGridView.Columns["eng"].Visible = false;
                fieldGridView.Columns["jpn"].Visible = false;
                fieldGridView.Columns["tchi"].Visible = false;
                fieldGridView.Columns["schi"].Visible = false;
                fieldGridView.Columns["kor"].Visible = false;
                fieldGridView.Columns["fre"].Visible = false;
                fieldGridView.Columns["ger"].Visible = false;
                fieldGridView.Columns["spa"].Visible = false;
                fieldGridView.Columns["ita"].Visible = false;
                fieldGridView.Columns["dut"].Visible = false;
                fieldGridView.Columns["rus"].Visible = false;
            }
        }

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
                    NewR = CurrentR - rand.Next(1, 5);
                else if (CurrentR < target[targetValue].R)
                    NewR = CurrentR + rand.Next(1, 5);
                else
                    NewR = CurrentR;
                if (NewR < 0)
                    NewR = 0;
                if (NewR > 255)
                    NewR = 255;

                if (CurrentG > target[targetValue].G)
                    NewG = CurrentG - rand.Next(1, 5);
                else if (CurrentG < target[targetValue].G)
                    NewG = CurrentG + rand.Next(1, 5);
                else
                    NewG = CurrentG;
                if (NewG < 0)
                    NewG = 0;
                if (NewG > 255)
                    NewG = 255;

                if (CurrentB > target[targetValue].B)
                    NewB = CurrentB - rand.Next(1, 5);
                else if (CurrentB < target[targetValue].B)
                    NewB = CurrentB + rand.Next(1, 5);
                else
                    NewB = CurrentB;
                if (NewB < 0)
                    NewB = 0;
                if (NewB > 255)
                    NewB = 255;

                fieldModeBtn.BackColor = Color.FromArgb(NewR, NewG, NewB);
            }
        }

        private void FlagTextbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.X <= FlagTextbox.Controls[1].Width + 1)
            {
                Console.WriteLine("EditBox");
                if (FlagTextbox.Value == 0x0)
                    FlagTextbox.Value = 0x20;
                else
                    FlagTextbox.Value = 0x00;
            }
            else if (e.Y <= FlagTextbox.Controls[1].Height / 2)
                Console.WriteLine("UpArrow");
            else if (e.X >= FlagTextbox.Controls[0].Width + FlagTextbox.Controls[0].Left)
                Console.WriteLine("Right border");
            else
                Console.WriteLine("DownArrow");
        }

        private void HexTextbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.X <= HexTextbox.Controls[1].Width + 1)
            {
                if (currentDataTable == recipeSource || currentDataTable == flowerSource)
                    return;

                if (IdTextbox.Text == "")
                    return;

                string id = Utilities.precedingZeros(IdTextbox.Text, 4);

                UInt16 IntId = Convert.ToUInt16("0x" + IdTextbox.Text, 16);

                if (Utilities.itemkind.ContainsKey(id))
                {
                    int value = Utilities.CountByKind[Utilities.itemkind[id]];

                    if (ItemAttr.hasFenceWithVariation(IntId))  // Fence Variation
                    {
                        string hexValue = Utilities.precedingZeros(HexTextbox.Text, 8);


                        string front = Utilities.precedingZeros(hexValue, 8).Substring(0, 4);
                        //string back = Utilities.precedingZeros(hexValue, 8).Substring(4, 4);

                        int decValue = value - 1;
                        if (decValue >= 0)
                            HexTextbox.Text = front + Utilities.precedingZeros(decValue.ToString("X"), 4);
                        else
                            HexTextbox.Text = front + Utilities.precedingZeros("0", 4);
                    }
                    else
                    {
                        int decValue = value - 1;
                        if (decValue >= 0)
                            HexTextbox.Text = Utilities.precedingZeros(decValue.ToString("X"), 8);
                        else
                            HexTextbox.Text = Utilities.precedingZeros("0", 8);
                    }
                }
                else
                {
                    HexTextbox.Text = "00000000";
                }

                UpdateVariation();
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

            string itemID = Utilities.precedingZeros(IdTextbox.Text, 4);
            string itemData = Utilities.precedingZeros(hexValue, 8);
            string flag2 = Utilities.precedingZeros(FlagTextbox.Text, 2);

            UInt16 IntId = Convert.ToUInt16("0x" + itemID, 16);
            string front = Utilities.precedingZeros(itemData, 8).Substring(0, 4);
            //string back = Utilities.precedingZeros(itemData, 8).Substring(4, 4);

            if (itemID.Equals("315A") || itemID.Equals("1618") || itemID.Equals("342F"))
            {
                selectedItem.setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source), true, GetImagePathFromID(Utilities.turn2bytes(itemData), source, Convert.ToUInt32("0x" + Utilities.translateVariationValueBack(front), 16)), "00", flag2);
            }
            else if (itemID.Equals("16A2"))
            {
                selectedItem.setup(GetNameFromID(itemID, recipeSource), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(Utilities.turn2bytes(itemData), recipeSource), true, "", "00", flag2);
            }
            else if (ItemAttr.hasFenceWithVariation(IntId))  // Fence Variation
            {
                selectedItem.setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + front, 16)), true, "", "00", flag2);
            }
            else
            {
                selectedItem.setup(GetNameFromID(itemID, source), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, source, Convert.ToUInt32("0x" + itemData, 16)), true, "", "00", flag2);
            }

            if (selection != null)
            {
                //selection.Dispose();
                string id = Utilities.precedingZeros(selectedItem.fillItemID(), 4);
                string value = Utilities.precedingZeros(selectedItem.fillItemData(), 8);

                if (ItemAttr.hasFenceWithVariation(IntId))  // Fence Variation
                {
                    selection.ReceiveID(Utilities.precedingZeros(selectedItem.fillItemID(), 4), languageSetting, value);
                }
                else if (id == "315A" || id == "1618" || id == "342F")
                {
                    selection.ReceiveID(Utilities.turn2bytes(selectedItem.fillItemData()), languageSetting);
                }
                else
                {
                    selection.ReceiveID(Utilities.precedingZeros(selectedItem.fillItemID(), 4), languageSetting);
                }
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
                this.Invoke((MethodInvoker)delegate
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

        private static void BuildActivateTable(byte[] ActivateLayer, ref bool[,] ActivateTable)
        {
            int width = 112;
            int height = 96 * 2;

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
                Address2 = Utilities.mapActivate + Utilities.mapOffset + offset;
            }
            else
            {
                Address1 = Utilities.mapActivate + Utilities.mapActivateSize + offset;
                Address2 = Utilities.mapActivate + Utilities.mapActivateSize + Utilities.mapOffset + offset;
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

            DisableBtn();

            btnToolTip.RemoveAll();

            long Address1;
            long Address2;

            if (layer1Btn.Checked)
            {
                Address1 = Utilities.mapActivate + offset;
                Address2 = Utilities.mapActivate + Utilities.mapOffset + offset;
            }
            else
            {
                Address1 = Utilities.mapActivate + Utilities.mapActivateSize + offset;
                Address2 = Utilities.mapActivate + Utilities.mapActivateSize + Utilities.mapOffset + offset;
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
                        this.Invoke((MethodInvoker)delegate
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

            Utilities.pokeAddress(s, usb, Address1.ToString("X"), value.ToString("X"));
            Utilities.pokeAddress(s, usb, (Address1 + 0x1C).ToString("X"), value.ToString("X"));
            Utilities.pokeAddress(s, usb, Address2.ToString("X"), value.ToString("X"));
            Utilities.pokeAddress(s, usb, (Address2 + 0x1C).ToString("X"), value.ToString("X"));

            this.Invoke((MethodInvoker)delegate
            {
                EnableBtn();
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            HideMapWait();
        }
    }
}