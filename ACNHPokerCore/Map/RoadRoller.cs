using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class RoadRoller : Form
    {
        private readonly Socket socket;
        private readonly USBBot usb;
        private readonly bool sound;
        private readonly bool debugging;

        private byte[] Layer1;
        private byte[] Acre;
        private byte[] Building;
        private byte[] Terrain;
        private byte[] MapCustomDesgin;
        private byte[] MyDesign;

        private DesignPattern[] designPatterns;
        private readonly int MyDesignIconSize = 36;

        private const int numOfColumn = 0x70;
        private const int numOfRow = 0x60;
        private const int TerrainSize = 0xE;

        private int counter;
        private int GridSize;
        private int LastX = -1;
        private int LastY = -1;
        private int LastChangedX = -1;
        private int LastChangedY = -1;
        private bool wasPlacing;
        private bool wasRemoving;

        private TerrainUnit[][] terrainUnits;
        private bool mapDrawReady;
        private MiniMap miniMap;
        private int anchorX;
        private int anchorY;
        private bool mapDrawing;
        private bool highlightRoadCorner;
        private bool highlightCliffCorner;
        private bool highlightRiverCorner;
        private bool displayCustomDesign;
        private bool cornerMode;
        private bool displayBuilding;
        private bool displayRoad = true;
        private bool firstEdit = true;
        private bool firstCustomEdit = true;
        private bool terrainSaving;
        private bool haveCustomEdit;

        private ushort selectedRoad = 99;
        private Button selectedButton;

        private Bitmap CurrentMainMap;
        private Bitmap CurrentMiniMap;

        private bool manualModeActivated;
        private bool customModeActivated;
        private Panel selectedManualMode;
        private ushort manualSelectedRoad = 7;
        private ushort manualSelectedRoadDirection;
        private Button manualSelectedRoadModel;

        private ushort manualSelectedCliffDirection;
        private ushort manualSelectedCliffElevation = 1;
        private Button manualSelectedCliffModel;

        private ushort manualSelectedRiverDirection;
        private ushort manualSelectedRiverElevation;
        private Button manualSelectedRiverModel;

        private bool isBigControl;
        private readonly Dictionary<string, Size> ControlSize = [];
        private readonly Dictionary<string, Point> ControlLocation = [];

        public event CloseHandler CloseForm;

        private static readonly Lock lockObject = new();

        private int currentSize;

        private readonly string debugTerrain = @"YourTerrain.nht";
        private readonly string debugAcres = @"YourAcre.nha";
        private readonly string debugBuilding = @"YourBuilding.nhb";
        private readonly string debugDesign = @"YourCustomDesignMap.nhdm";

        public RoadRoller(Socket S, USBBot USB, bool Sound, bool Debugging)
        {
            socket = S;
            usb = USB;
            sound = Sound;
            debugging = Debugging;
            InitializeComponent();
            SetSubDivider();
            Bitmap CliffImg = new(Properties.Resources.cliff, new Size(60, 60));
            CliffBtn.Image = CliffImg;
            Bitmap RiverImg = new(Properties.Resources.river, new Size(60, 60));
            RiverBtn.Image = RiverImg;
            Bitmap CornerImg = new(Properties.Resources.corner, new Size(50, 50));
            CornerBtn.Image = CornerImg;
            Bitmap WoodImg = new(Properties.Resources.wood, new Size(50, 50));
            WoodBtn.Image = WoodImg;
            Bitmap TileImg = new(Properties.Resources.tile, new Size(50, 50));
            TileBtn.Image = TileImg;
            Bitmap SandImg = new(Properties.Resources.sand, new Size(50, 50));
            SandBtn.Image = SandImg;
            Bitmap PatternImg = new(Properties.Resources.pattern, new Size(50, 50));
            ArchBtn.Image = PatternImg;
            Bitmap DarkDirtImg = new(Properties.Resources.darksoil, new Size(50, 50));
            DarkDirtBtn.Image = DarkDirtImg;
            Bitmap BrickImg = new(Properties.Resources.brick, new Size(50, 50));
            BrickBtn.Image = BrickImg;
            Bitmap StoneImg = new(Properties.Resources.stone, new Size(50, 50));
            StoneBtn.Image = StoneImg;
            Bitmap DirtImg = new(Properties.Resources.dirt, new Size(50, 50));
            DirtBtn.Image = DirtImg;

            Bitmap MiniCliffImg = new(Properties.Resources.cliff, new Size(36, 36));
            ManualCliffModeButton.BackgroundImage = MiniCliffImg;
            Bitmap MiniRiverImg = new(Properties.Resources.river, new Size(36, 36));
            ManualRiverModeButton.BackgroundImage = MiniRiverImg;
            Bitmap RotateImg = new(Properties.Resources.rotate, new Size(36, 36));
            RotateRoadButton.BackgroundImage = RotateImg;
            RotateCliffButton.BackgroundImage = RotateImg;
            RotateRiverButton.BackgroundImage = RotateImg;

            currentSize = 36;

            if (RoadDropdownBox.Items.Count > 0)
                RoadDropdownBox.SelectedIndex = RoadDropdownBox.Items.Count - 1;

            DrawRoadImages();
            DrawCliffImages();
            DrawRiverImages();
            CreateDict();

            selectedManualMode = ManualRoadPanel;


            if (debugging)
            {
                OpenFileDialog Acresfile = new()
                {
                    Filter = "New Horizons Acres (*.nha)|*.nha",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                string savepath;

                if (config.AppSettings.Settings["LastLoad"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;
                else
                    savepath = config.AppSettings.Settings["LastLoad"].Value;

                if (Directory.Exists(savepath))
                {
                    Acresfile.InitialDirectory = savepath;
                }
                else
                {
                    Acresfile.InitialDirectory = @"C:\";
                }

                byte[] emptyPlaza = new byte[12];

                if (File.Exists(savepath + "\\" + debugAcres))
                {
                    Acre = Utilities.Add(File.ReadAllBytes(savepath + "\\" + debugAcres), emptyPlaza);

                    if (File.Exists(savepath + "\\" + debugTerrain))
                    {
                        Terrain = File.ReadAllBytes(savepath + "\\" + debugTerrain);
                    }
                    else
                    {
                        Terrain = new byte[Utilities.AllTerrainSize];
                    }

                    if (File.Exists(savepath + "\\" + debugBuilding))
                    {
                        Building = File.ReadAllBytes(savepath + "\\" + debugBuilding);
                    }
                    else
                    {
                        Building = new byte[Utilities.AllBuildingSize];
                    }
                }
                else
                {
                    if (Acresfile.ShowDialog() != DialogResult.OK)
                    {
                        Acre = new byte[Utilities.AcreAndPlaza];
                        Terrain = new byte[Utilities.AllTerrainSize];
                    }
                    else
                    {
                        Acre = Utilities.Add(File.ReadAllBytes(Acresfile.FileName), emptyPlaza);

                        OpenFileDialog Terrainfile = new()
                        {
                            Filter = "New Horizons Terrain (*.nht)|*.nht",
                        };

                        if (Directory.Exists(savepath))
                        {
                            Terrainfile.InitialDirectory = savepath;
                        }
                        else
                        {
                            Terrainfile.InitialDirectory = @"C:\";
                        }

                        if (Terrainfile.ShowDialog() != DialogResult.OK)
                        {
                            Terrain = new byte[Utilities.AllTerrainSize];
                        }
                        else
                        {
                            Terrain = File.ReadAllBytes(Terrainfile.FileName);
                        }
                    }



                    Building = new byte[Utilities.AllBuildingSize];
                }

                Layer1 = new byte[Utilities.mapSize];

                if (File.Exists(savepath + debugDesign))
                {
                    MapCustomDesgin = File.ReadAllBytes(savepath + "\\" + debugDesign);
                }
                else
                {
                    MapCustomDesgin = new byte[Utilities.MapTileCount16x16 * 2];
                    byte[] EmptyDesign = [0x00, 0xF8];
                    for (int i = 0; i < Utilities.MapTileCount16x16; i++)
                        Buffer.BlockCopy(EmptyDesign, 0, MapCustomDesgin, i * 2, 2);
                }

                if (miniMap == null)
                    miniMap = new MiniMap(Layer1, Acre, Building, Terrain, MapCustomDesgin, 2);

                int iterator = 0;

                terrainUnits = new TerrainUnit[numOfColumn][];
                for (int i = 0; i < numOfColumn; i++)
                {
                    terrainUnits[i] = new TerrainUnit[numOfRow];
                    for (int j = 0; j < numOfRow; j++)
                    {
                        byte[] currentTile = new byte[TerrainSize];
                        Buffer.BlockCopy(Terrain, iterator * TerrainSize, currentTile, 0, TerrainSize);
                        terrainUnits[i][j] = new TerrainUnit(currentTile);

                        if (MapCustomDesgin != null)
                        {
                            byte[] currentDesign = new byte[2];
                            Buffer.BlockCopy(MapCustomDesgin, (i * numOfRow + j) * 2, currentDesign, 0, 2);
                            terrainUnits[i][j].SetCustomDesign(currentDesign);
                        }

                        iterator++;
                    }
                }

                CurrentMiniMap = miniMap.DrawBackground();
                miniMapBox.BackgroundImage = CurrentMiniMap;
                miniMapBox.Image = MiniMap.DrawSelectSquare16(8, 8);
                DrawMainMap(anchorX, anchorY);
                MaximizeBox = true;
                mapDrawReady = true;
            }
            else
            {
                ShowMapWait(42);

                Thread LoadThread = new(FetchMap);
                LoadThread.Start();
            }
        }

        private void CreateDict()
        {
            foreach (Control ctrl in ManualButtonPanel.Controls)
            {
                ControlSize.Add(ctrl.Name, ctrl.Size);
                ControlLocation.Add(ctrl.Name, ctrl.Location);
            }
            foreach (Control ctrl in ManualRoadPanel.Controls)
            {
                ControlSize.Add(ctrl.Name, ctrl.Size);
                ControlLocation.Add(ctrl.Name, ctrl.Location);
            }
            foreach (Control ctrl in ManualCliffPanel.Controls)
            {
                ControlSize.Add(ctrl.Name, ctrl.Size);
                ControlLocation.Add(ctrl.Name, ctrl.Location);
            }
            foreach (Control ctrl in ManualRiverPanel.Controls)
            {
                ControlSize.Add(ctrl.Name, ctrl.Size);
                ControlLocation.Add(ctrl.Name, ctrl.Location);
            }
        }

        private void RoadRoller_Resize(object sender, EventArgs e)
        {
            if (terrainUnits != null && WindowState != FormWindowState.Minimized && mapDrawReady)
            {
                SetSubDivider();
                DrawMainMap(anchorX, anchorY);

                int DiffHeight = LeftMenuPanel.Height - 605;
                CustomDesignList.Height = 340 + DiffHeight;
                int DiffWidth = LeftMenuPanel.Width - 225;
                CustomDesignList.Width = 205 + DiffWidth;

                int size = CustomDesignList.Width / 5 - 5;
                if (size > 90)
                    size = 90;

                if (CustomDesignList.Width <= 300)
                    CustomDesignList.TileSize = new Size(50, 50);
                else
                    CustomDesignList.TileSize = new Size(size, size);

                CustomDesignList.Width += 1; // Force a refresh

                if (LeftMenuPanel.Width > 355 && LeftMenuPanel.Height > 790)
                {
                    if (!isBigControl)
                    {
                        isBigControl = true;
                        TurnBig();
                    }
                }
                else
                {
                    if (isBigControl)
                    {
                        isBigControl = false;
                        TurnSmall();
                    }
                }
            }
        }

        private void TurnBig()
        {
            int newSize = 60;

            ManualButtonPanel.Width = 350;
            ManualButtonPanel.Height = 550;

            ManualCliffModeButton.Width = newSize;
            ManualCliffModeButton.Height = newSize;

            ManualRiverModeButton.Width = newSize;
            ManualRiverModeButton.Height = newSize;
            ManualRiverModeButton.Location = new Point(69, 3);

            ManualRoadModeButton.Width = newSize;
            ManualRoadModeButton.Height = newSize;
            ManualRoadModeButton.Location = new Point(135, 3);

            RoadDropdownBox.Width = 146;
            RoadDropdownBox.Location = new Point(201, 14);

            //--
            ManualRoadPanel.Size = new Size(350, 486);
            ManualRoadPanel.Location = new Point(0, 65);

            RoadButton0A.Size = new Size(newSize, newSize);

            RoadButton0B.Size = new Size(newSize, newSize);
            RoadButton0B.Location = new Point(69, 3);
            RoadButton1A.Size = new Size(newSize, newSize);
            RoadButton1A.Location = new Point(3, 69);
            RoadButton1B.Size = new Size(newSize, newSize);
            RoadButton1B.Location = new Point(69, 69);
            RoadButton1C.Size = new Size(newSize, newSize);
            RoadButton1C.Location = new Point(135, 69);
            RoadButton2A.Size = new Size(newSize, newSize);
            RoadButton2A.Location = new Point(3, 135);
            RoadButton2B.Size = new Size(newSize, newSize);
            RoadButton2B.Location = new Point(69, 135);
            RoadButton2C.Size = new Size(newSize, newSize);
            RoadButton2C.Location = new Point(135, 135);
            RoadButton3A.Size = new Size(newSize, newSize);
            RoadButton3A.Location = new Point(3, 201);
            RoadButton3B.Size = new Size(newSize, newSize);
            RoadButton3B.Location = new Point(69, 201);
            RoadButton3C.Size = new Size(newSize, newSize);
            RoadButton3C.Location = new Point(135, 201);
            RoadButton4A.Size = new Size(newSize, newSize);
            RoadButton4A.Location = new Point(3, 267);
            RoadButton4B.Size = new Size(newSize, newSize);
            RoadButton4B.Location = new Point(69, 267);
            RoadButton4C.Size = new Size(newSize, newSize);
            RoadButton4C.Location = new Point(135, 267);
            RoadButton5A.Size = new Size(newSize, newSize);
            RoadButton5A.Location = new Point(3, 333);
            RoadButton5B.Size = new Size(newSize, newSize);
            RoadButton5B.Location = new Point(69, 333);
            RoadButton6A.Size = new Size(newSize, newSize);
            RoadButton6A.Location = new Point(135, 333);
            RoadButton6B.Size = new Size(newSize, newSize);
            RoadButton6B.Location = new Point(201, 333);
            RoadButton7A.Size = new Size(newSize, newSize);
            RoadButton7A.Location = new Point(3, 399);
            RoadButton8A.Size = new Size(newSize, newSize);
            RoadButton8A.Location = new Point(69, 399);

            RotateRoadButton.Size = new Size(newSize, newSize);
            RotateRoadButton.Location = new Point(267, 3);


            //--
            ManualCliffPanel.Size = new Size(350, 486);
            ManualCliffPanel.Location = new Point(0, 65);

            CliffButton0A.Size = new Size(newSize, newSize);

            CliffButton1A.Size = new Size(newSize, newSize);
            CliffButton1A.Location = new Point(3, 69);
            CliffButton2A.Size = new Size(newSize, newSize);
            CliffButton2A.Location = new Point(3, 135);
            CliffButton2B.Size = new Size(newSize, newSize);
            CliffButton2B.Location = new Point(69, 135);
            CliffButton2C.Size = new Size(newSize, newSize);
            CliffButton2C.Location = new Point(135, 135);
            CliffButton3A.Size = new Size(newSize, newSize);
            CliffButton3A.Location = new Point(3, 201);
            CliffButton3B.Size = new Size(newSize, newSize);
            CliffButton3B.Location = new Point(69, 201);
            CliffButton3C.Size = new Size(newSize, newSize);
            CliffButton3C.Location = new Point(135, 201);
            CliffButton4A.Size = new Size(newSize, newSize);
            CliffButton4A.Location = new Point(3, 267);
            CliffButton4B.Size = new Size(newSize, newSize);
            CliffButton4B.Location = new Point(69, 267);
            CliffButton4C.Size = new Size(newSize, newSize);
            CliffButton4C.Location = new Point(135, 267);
            CliffButton5A.Size = new Size(newSize, newSize);
            CliffButton5A.Location = new Point(3, 333);
            CliffButton5B.Size = new Size(newSize, newSize);
            CliffButton5B.Location = new Point(69, 333);
            CliffButton6A.Size = new Size(newSize, newSize);
            CliffButton6A.Location = new Point(135, 333);
            CliffButton6B.Size = new Size(newSize, newSize);
            CliffButton6B.Location = new Point(201, 333);
            CliffButton7A.Size = new Size(newSize, newSize);
            CliffButton7A.Location = new Point(3, 399);
            CliffButton8.Size = new Size(newSize, newSize);
            CliffButton8.Location = new Point(69, 399);
            CliffButton8A.Size = new Size(newSize, newSize);
            CliffButton8A.Location = new Point(267, 399);
            RotateCliffButton.Size = new Size(newSize, newSize);
            RotateCliffButton.Location = new Point(267, 3);

            ManualTerrainElevationLabel.Location = new Point(255, 85);
            ManualCliffElevationBar.Location = new Point(287, 94);
            ManualTerrainElevation3Label.Location = new Point(314, 100);
            ManualTerrainElevation2Label.Location = new Point(314, 124);
            ManualTerrainElevation1Label.Location = new Point(314, 148);

            //--
            ManualRiverPanel.Size = new Size(350, 486);
            ManualRiverPanel.Location = new Point(0, 65);

            RiverButton0A.Size = new Size(newSize, newSize);

            RiverButton1A.Size = new Size(newSize, newSize);
            RiverButton1A.Location = new Point(3, 69);
            RiverButton2A.Size = new Size(newSize, newSize);
            RiverButton2A.Location = new Point(3, 135);
            RiverButton2B.Size = new Size(newSize, newSize);
            RiverButton2B.Location = new Point(69, 135);
            RiverButton2C.Size = new Size(newSize, newSize);
            RiverButton2C.Location = new Point(135, 135);
            RiverButton3A.Size = new Size(newSize, newSize);
            RiverButton3A.Location = new Point(3, 201);
            RiverButton3B.Size = new Size(newSize, newSize);
            RiverButton3B.Location = new Point(69, 201);
            RiverButton3C.Size = new Size(newSize, newSize);
            RiverButton3C.Location = new Point(135, 201);
            RiverButton4A.Size = new Size(newSize, newSize);
            RiverButton4A.Location = new Point(3, 267);
            RiverButton4B.Size = new Size(newSize, newSize);
            RiverButton4B.Location = new Point(69, 267);
            RiverButton4C.Size = new Size(newSize, newSize);
            RiverButton4C.Location = new Point(135, 267);
            RiverButton5A.Size = new Size(newSize, newSize);
            RiverButton5A.Location = new Point(3, 333);
            RiverButton5B.Size = new Size(newSize, newSize);
            RiverButton5B.Location = new Point(69, 333);
            RiverButton6A.Size = new Size(newSize, newSize);
            RiverButton6A.Location = new Point(135, 333);
            RiverButton6B.Size = new Size(newSize, newSize);
            RiverButton6B.Location = new Point(201, 333);
            RiverButton7A.Size = new Size(newSize, newSize);
            RiverButton7A.Location = new Point(3, 399);
            RiverButton8A.Size = new Size(newSize, newSize);
            RiverButton8A.Location = new Point(69, 399);
            RotateRiverButton.Size = new Size(newSize, newSize);
            RotateRiverButton.Location = new Point(267, 3);

            ManualRiverElevationLabel.Location = new Point(255, 85);
            ManualRiverElevationBar.Location = new Point(287, 94);
            ManualRiverElevation3Label.Location = new Point(314, 100);
            ManualRiverElevation2Label.Location = new Point(314, 116);
            ManualRiverElevation1Label.Location = new Point(314, 132);
            ManualRiverElevation0Label.Location = new Point(314, 148);

            Bitmap MiniCliffImg = new(Properties.Resources.cliff, new Size(newSize, newSize));
            ManualCliffModeButton.BackgroundImage = MiniCliffImg;
            Bitmap MiniRiverImg = new(Properties.Resources.river, new Size(newSize, newSize));
            ManualRiverModeButton.BackgroundImage = MiniRiverImg;
            Bitmap RotateImg = new(Properties.Resources.rotate, new Size(newSize, newSize));
            RotateRoadButton.BackgroundImage = RotateImg;
            RotateCliffButton.BackgroundImage = RotateImg;
            RotateRiverButton.BackgroundImage = RotateImg;

            UpdateSelectedRoad();
            DrawRoadImages(manualSelectedRoadDirection, manualSelectedRoad, newSize);
            DrawCliffImages(manualSelectedCliffDirection, manualSelectedCliffElevation, newSize);
            DrawRiverImages(manualSelectedRiverDirection, manualSelectedRiverElevation, newSize);
            currentSize = newSize;
        }

        private void TurnSmall()
        {
            int oldSize = 36;

            ManualButtonPanel.Width = 225;
            ManualButtonPanel.Height = 340;

            foreach (Control ctrl in ManualButtonPanel.Controls)
            {
                ctrl.Size = ControlSize[ctrl.Name];
                ctrl.Location = ControlLocation[ctrl.Name];
            }
            foreach (Control ctrl in ManualRoadPanel.Controls)
            {
                ctrl.Size = ControlSize[ctrl.Name];
                ctrl.Location = ControlLocation[ctrl.Name];
            }
            foreach (Control ctrl in ManualCliffPanel.Controls)
            {
                ctrl.Size = ControlSize[ctrl.Name];
                ctrl.Location = ControlLocation[ctrl.Name];
            }
            foreach (Control ctrl in ManualRiverPanel.Controls)
            {
                ctrl.Size = ControlSize[ctrl.Name];
                ctrl.Location = ControlLocation[ctrl.Name];
            }

            Bitmap MiniCliffImg = new(Properties.Resources.cliff, new Size(oldSize, oldSize));
            ManualCliffModeButton.BackgroundImage = MiniCliffImg;
            Bitmap MiniRiverImg = new(Properties.Resources.river, new Size(oldSize, oldSize));
            ManualRiverModeButton.BackgroundImage = MiniRiverImg;
            Bitmap RotateImg = new(Properties.Resources.rotate, new Size(oldSize, oldSize));
            RotateRoadButton.BackgroundImage = RotateImg;
            RotateCliffButton.BackgroundImage = RotateImg;
            RotateRiverButton.BackgroundImage = RotateImg;

            UpdateSelectedRoad();
            DrawRoadImages(manualSelectedRoadDirection, manualSelectedRoad, oldSize);
            DrawCliffImages(manualSelectedCliffDirection, manualSelectedCliffElevation, oldSize);
            DrawRiverImages(manualSelectedRiverDirection, manualSelectedRiverElevation, oldSize);
            currentSize = oldSize;
        }

        private void SetSubDivider()
        {
            int w = SubDivider.Width;
            int h = SubDivider.Height;
            if (w - h > 0)
            {
                SubDivider.ColumnStyles[0].Width = (w - h) / 2;
                SubDivider.ColumnStyles[2].Width = (w - h) / 2;
                SubDivider.RowStyles[0].Height = 0;
                SubDivider.RowStyles[2].Height = 0;
            }
            else if (h - w > 0)
            {
                SubDivider.ColumnStyles[0].Width = 0;
                SubDivider.ColumnStyles[2].Width = 0;
                SubDivider.RowStyles[0].Height = (h - w) / 2;
                SubDivider.RowStyles[2].Height = (h - w) / 2;
            }
            else
            {
                SubDivider.ColumnStyles[0].Width = 0;
                SubDivider.ColumnStyles[2].Width = 0;
                SubDivider.RowStyles[0].Height = 0;
                SubDivider.RowStyles[2].Height = 0;
            }
        }

        private void FetchMap()
        {
            try
            {
                if (socket != null || usb != null || Utilities.isEmulator)
                {
                    Layer1 = Utilities.GetMapLayer(socket, usb, Utilities.mapZero, ref counter);
                    Acre = Utilities.GetAcre(socket, usb);
                    Building = Utilities.GetBuilding(socket, usb);
                    Terrain = Utilities.GetTerrain(socket, usb);
                    MyDesign = Utilities.GetMyDesign(socket, usb, ref counter);
                    MapCustomDesgin = Utilities.GetCustomDesignMap(socket, usb, ref counter);

                    if (Acre != null)
                    {
                        if (miniMap == null)
                            miniMap = new MiniMap(Layer1, Acre, Building, Terrain, MapCustomDesgin, 2);
                    }
                    else
                        throw new NullReferenceException("Layer1/Layer2/Acre");

                    if (MyDesign != null || MapCustomDesgin != null)
                    {

                        Invoke((MethodInvoker)delegate
                        {
                            var imageList = new ImageList
                            {
                                ImageSize = new Size(MyDesignIconSize, MyDesignIconSize)
                            };
                            CustomDesignList.LargeImageList = imageList;
                            CustomDesignList.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;

                            CustomDesignList.TileSize = new Size(50, 50);
                            CustomDesignList.View = View.Tile;
                            CustomDesignList.OwnerDraw = true;
                            CustomDesignList.DrawItem += DesignList_DrawItem;

                            designPatterns = new DesignPattern[Utilities.PatternCount];
                            for (int i = 0; i < Utilities.PatternCount; i++)
                            {
                                byte[] currentDesign = new byte[DesignPattern.SIZE];
                                Buffer.BlockCopy(MyDesign, i * DesignPattern.SIZE, currentDesign, 0, DesignPattern.SIZE);
                                designPatterns[i] = new DesignPattern(currentDesign);
                                Image currentDesignImage = designPatterns[i].GetBitmap();
                                imageList.Images.Add(i.ToString(), currentDesignImage);
                                CustomDesignList.Items.Add(i.ToString(), i.ToString());
                                CustomDesignList.Items[i].ToolTipText = designPatterns[i].DesignName + "\n" + designPatterns[i].TownName + "\n" + designPatterns[i].PlayerName;
                            }
                        });


                    }
                    else
                        throw new NullReferenceException("Custom Design");


                    if (Terrain != null)
                    {
                        int iterator = 0;

                        terrainUnits = new TerrainUnit[numOfColumn][];
                        for (int i = 0; i < numOfColumn; i++)
                        {
                            terrainUnits[i] = new TerrainUnit[numOfRow];
                            for (int j = 0; j < numOfRow; j++)
                            {
                                byte[] currentTile = new byte[TerrainSize];
                                Buffer.BlockCopy(Terrain, iterator * TerrainSize, currentTile, 0, TerrainSize);
                                terrainUnits[i][j] = new TerrainUnit(currentTile);

                                if (MapCustomDesgin != null)
                                {
                                    byte[] currentDesign = new byte[2];
                                    Buffer.BlockCopy(MapCustomDesgin, (i * numOfRow + j) * 2, currentDesign, 0, 2);
                                    terrainUnits[i][j].SetCustomDesign(currentDesign);
                                }

                                iterator++;
                            }
                        }

                        while (!this.IsHandleCreated)
                            Thread.Sleep(100);

                        Invoke((MethodInvoker)delegate
                        {
                            CurrentMiniMap = miniMap.DrawBackground();
                            miniMapBox.BackgroundImage = CurrentMiniMap;
                            miniMapBox.Image = MiniMap.DrawSelectSquare16(8, 8);
                            DrawMainMap(anchorX, anchorY);
                            MaximizeBox = true;
                        });
                    }
                    else
                        throw new NullReferenceException("Terrain");

                    mapDrawReady = true;

                }

                HideMapWait();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("RoadRoller", "FetchMap: " + ex.Message);
                MyMessageBox.Show(ex.Message, "Oof");
            }
        }

        private void DesignList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            using Graphics g = e.Graphics;
            if (e.Item.Selected)
            {
                if ((e.State & ListViewItemStates.Selected) != 0)
                {
                    SolidBrush Select = new(Color.Orange);
                    g.FillRectangle(Select, e.Bounds);
                    e.DrawFocusRectangle();
                }
            }

            g.DrawImage(e.Item.ImageList.Images[e.ItemIndex], new Rectangle(e.Bounds.X + 4, e.Bounds.Y + 4, e.Bounds.Width - 8, e.Bounds.Height - 8));
        }
        private void DrawMainMap(int x, int y)
        {
            if (mapDrawing)
                return;

            if (x < 0 || y < 0)
                return;

            mapDrawing = true;

            int size;
            int w = SubDivider.Width;
            int h = SubDivider.Height;
            if (w - h >= 0)
                size = h;
            else
                size = w;

            Bitmap myBitmap;
            GridSize = (size / 16) - 1;

            myBitmap = new Bitmap(GridSize * 16, GridSize * 16);

            bool highlightMouth = fixRiverMouthToggle.Checked;

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                var cm = new ColorMatrix
                {
                    Matrix33 = 1
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                bool[,] mouthMarker = new bool[16, 16];
                if (highlightMouth)
                    mouthMarker = BuildMouthMarker();

                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        Bitmap tile;

                        TerrainUnit CurrentUnit = terrainUnits[x + i][y + j];

                        if (displayCustomDesign && CurrentUnit.HaveCustomDesign())
                        {
                            tile = DrawTileWithCustomDesign(CurrentUnit, GridSize);
                        }
                        else
                        {
                            if (highlightMouth && mouthMarker[i, j])
                            {
                                tile = new Bitmap(CurrentUnit.GetImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner, true));
                            }
                            else
                            {
                                tile = new Bitmap(CurrentUnit.GetImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner));
                            }
                        }
                        graphics.DrawImage(tile, new Rectangle(i * GridSize, j * GridSize, GridSize, GridSize), 0, 0, GridSize, GridSize, GraphicsUnit.Pixel, ia);
                    }
                }
            }

            MainMap.Size = new Size(GridSize * 16, GridSize * 16);
            MainMap.Location = new Point((size - (GridSize * 16)) / 2 - 2, (size - (GridSize * 16)) / 2 - 2);
            MainMap.BackgroundImage = myBitmap;
            CurrentMainMap = myBitmap;

            mapDrawing = false;
        }

        private Bitmap DrawTileWithCustomDesign(TerrainUnit currentUnit, int size)
        {
            Color borderColor = Color.Orange;
            Bitmap BottomImage = new(size, size);
            using Graphics gr = Graphics.FromImage(BottomImage);
            gr.SmoothingMode = SmoothingMode.None;
            gr.Clear(borderColor);

            Bitmap TopImage;
            int DesignID = BitConverter.ToInt16(currentUnit.GetCustomDesign(), 0);
            TopImage = designPatterns[DesignID].GetBitmap(size - 2);

            Bitmap Final = BottomImage;

            using Graphics graphics = Graphics.FromImage(Final);
            var cm = new ColorMatrix
            {
                Matrix33 = 1
            };

            var ia = new ImageAttributes();
            ia.SetColorMatrix(cm);

            graphics.DrawImage(TopImage, new Rectangle(1, 1, TopImage.Width, TopImage.Height), 0, 0, TopImage.Width, TopImage.Height, GraphicsUnit.Pixel, ia);

            return Final;
        }

        private async Task DrawMainMapAsync(int x, int y, bool force = false)
        {
            if (terrainUnits == null)
                return;

            if (!force)
            {
                if (mapDrawing)
                    return;
            }

            if (x < 0 || y < 0)
                return;

            mapDrawing = true;

            int size;
            int w = SubDivider.Width;
            int h = SubDivider.Height;
            if (w - h >= 0)
                size = h;
            else
                size = w;

            Bitmap myBitmap;
            GridSize = (size / 16) - 1;

            myBitmap = new Bitmap(GridSize * 16, GridSize * 16);

            bool highlightMouth = fixRiverMouthToggle.Checked;

            await Task.Run(() =>
            {
                lock (lockObject)
                {
                    using (Graphics graphics = Graphics.FromImage(myBitmap))
                    {
                        var cm = new ColorMatrix
                        {
                            Matrix33 = 1
                        };

                        var ia = new ImageAttributes();
                        ia.SetColorMatrix(cm);

                        for (int i = 0; i < 16; i++)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                Bitmap tile;

                                TerrainUnit CurrentUnit = terrainUnits[x + i][y + j];

                                if (displayCustomDesign && CurrentUnit.HaveCustomDesign())
                                {
                                    tile = DrawTileWithCustomDesign(CurrentUnit, GridSize);
                                }
                                else
                                {
                                    bool[,] mouthMarker = new bool[16, 16];
                                    if (highlightMouth)
                                        mouthMarker = BuildMouthMarker();

                                    if (highlightMouth && mouthMarker[i, j])
                                    {
                                        tile = new Bitmap(CurrentUnit.GetImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner, true));
                                    }
                                    else
                                    {
                                        tile = new Bitmap(CurrentUnit.GetImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner));
                                    }
                                }

                                graphics.DrawImage(tile, new Rectangle(i * GridSize, j * GridSize, GridSize, GridSize), 0, 0, GridSize, GridSize, GraphicsUnit.Pixel, ia);
                            }
                        }
                    }

                    MainMap.Size = new Size(GridSize * 16, GridSize * 16);
                    MainMap.Location = new Point((size - (GridSize * 16)) / 2 - 2, (size - (GridSize * 16)) / 2 - 2);
                    MainMap.BackgroundImage = myBitmap;
                    CurrentMainMap = myBitmap;
                }
            });

            mapDrawing = false;
        }

        private async Task UpdateMainMapAsync(int x, int y)
        {
            if (mapDrawing)
                return;

            if (x < 0 || y < 0)
                return;

            mapDrawing = true;

            int size;
            int w = SubDivider.Width;
            int h = SubDivider.Height;
            if (w - h >= 0)
                size = h;
            else
                size = w;

            GridSize = (size / 16) - 1;

            Bitmap myBitmap = new(CurrentMainMap);

            bool highlightMouth = fixRiverMouthToggle.Checked;

            await Task.Run(() =>
            {
                lock (lockObject)
                {
                    using (Graphics graphics = Graphics.FromImage(myBitmap))
                    {
                        var cm = new ColorMatrix
                        {
                            Matrix33 = 1
                        };

                        var ia = new ImageAttributes();
                        ia.SetColorMatrix(cm);

                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                if (x + i >= 0 && x + i < numOfColumn && y + j >= 0 && y + j < numOfRow)
                                {
                                    Bitmap tile;

                                    TerrainUnit CurrentUnit = terrainUnits[x + i][y + j];

                                    if (displayCustomDesign && CurrentUnit.HaveCustomDesign())
                                    {
                                        tile = DrawTileWithCustomDesign(CurrentUnit, GridSize);
                                    }
                                    else if (highlightMouth && IsRiverMouth(x + i, y + j))
                                    {
                                        tile = new Bitmap(CurrentUnit.GetImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner, true));
                                    }
                                    else
                                    {
                                        tile = new Bitmap(CurrentUnit.GetImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner));
                                    }
                                    graphics.DrawImage(tile, new Rectangle((x + i - anchorX) * GridSize, (y + j - anchorY) * GridSize, GridSize, GridSize), 0, 0, GridSize, GridSize, GraphicsUnit.Pixel, ia);
                                }
                            }
                        }
                    }

                    MainMap.BackgroundImage = myBitmap;
                    CurrentMainMap = myBitmap;
                }
            });

            mapDrawing = false;
        }

        private void ShowMapWait(int size)
        {
            PleaseWaitPanel.Visible = true;
            counter = 0;
            MapProgressBar.Maximum = size + 5;
            MapProgressBar.Value = counter;
            PleaseWaitPanel.Visible = true;
            ProgressTimer.Start();
        }

        private void HideMapWait()
        {
            Invoke((MethodInvoker)delegate
            {
                PleaseWaitPanel.Visible = false;
                ProgressTimer.Stop();
            });
        }

        private void PlaceDesign(byte[] value, int x, int y)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];

            CurrentUnit.SetCustomDesign(value);
            _ = UpdateMainMapAsync(x, y);
            AddMiniMapPixel(x, y, TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Design]);
        }

        private void RemoveDesign(int x, int y)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];

            ushort elevation = CurrentUnit.GetElevation();
            CurrentUnit.RemoveCustomDesign();
            _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void PlaceRoad(int x, int y)
        {
            ushort road = selectedRoad;
            ushort elevation = terrainUnits[x][y].GetElevation();

            bool[,] neighbour = FindSameNeighbourRoad(road, elevation, x, y);
            terrainUnits[x][y].UpdateRoad(road, neighbour);
            FixNeighbourRoad(x, y, road, neighbour);
            _ = UpdateMainMapAsync(x, y);
            AddMiniMapPixel(x, y, TerrainUnit.TerrainColor[road]);
        }

        private void PlaceManualRoad(int x, int y)
        {
            ushort road = manualSelectedRoad;
            string type = FindModel(manualSelectedRoadModel);

            terrainUnits[x][y].SetRoad(road, type, manualSelectedRoadDirection);
            _ = UpdateMainMapAsync(x, y);
            AddMiniMapPixel(x, y, TerrainUnit.TerrainColor[road]);
        }

        private void ChangeRoadCorner(int x, int y)
        {
            terrainUnits[x][y].ChangeRoadCorner();
            _ = UpdateMainMapAsync(x, y);
        }

        private void ChangeRiverCorner(int x, int y)
        {
            terrainUnits[x][y].ChangeRiverCorner();
            _ = UpdateMainMapAsync(x, y);
        }

        private void ChangeCliffCorner(int x, int y)
        {
            terrainUnits[x][y].ChangeCliffCorner();
            _ = UpdateMainMapAsync(x, y);
        }

        private void PlaceCliff(int x, int y, ushort placeElevation)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];

            if (CurrentUnit.HasRoad()) // Remove Road
            {
                DeleteRoad(x, y, false);
            }

            if ((CurrentUnit.IsFlat() || CurrentUnit.IsCliff()) && CurrentUnit.GetElevation() == placeElevation)
                return;

            bool[,] neighbour = FindSameNeighbourCliff(placeElevation, x, y);

            CleanUpRiverOrFall(x, y, neighbour, placeElevation, CurrentUnit.GetElevation());

            if (placeElevation == 0)
            {
                DeleteCliff(x, y, false);
            }
            else
            {
                CurrentUnit.UpdateCliff(neighbour, placeElevation);
                bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y);
                FixNeighbourTerrain(x, y, TerrainNeighbour);
            }

            _ = UpdateMainMapAsync(x, y);

            Color c;
            if (placeElevation == 0)
                c = MiniMap.GetBackgroundColorLess(x, y);
            else
            {
                int terrainNum = placeElevation + 19;
                c = TerrainUnit.TerrainColor[terrainNum];
            }
            AddMiniMapPixel(x, y, c);
        }

        private void PlaceManualCliff(int x, int y)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            string type = FindModel(manualSelectedCliffModel);

            CurrentUnit.SetCliff(type, manualSelectedCliffElevation, manualSelectedCliffDirection);

            _ = UpdateMainMapAsync(x, y);

            Color c;
            if (manualSelectedCliffElevation == 0)
                c = MiniMap.GetBackgroundColorLess(x, y);
            else
            {
                int terrainNum = manualSelectedCliffElevation + 19;
                c = TerrainUnit.TerrainColor[terrainNum];
            }
            AddMiniMapPixel(x, y, c);
        }

        private void PlaceRiverOrFall(int x, int y)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];

            if (CurrentUnit.HasRoad()) // Remove Road
            {
                DeleteRoad(x, y, false);
            }

            if (CurrentUnit.IsCliff()) // Place Fall
            {
                if (CurrentUnit.IsFallCliff())
                {
                    //ushort elevation = CurrentUnit.getElevation();
                    ushort CliffDirection = CurrentUnit.GetTerrainAngle();
                    bool[,] ConnectNeighbour = FindSameNeighbourFall(x, y);
                    CurrentUnit.UpdateFall(ConnectNeighbour, CliffDirection);

                    bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y);
                    FixNeighbourTerrain(x, y, TerrainNeighbour);
                    _ = UpdateMainMapAsync(x, y);
                    AddMiniMapPixel(x, y, TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Fall]);
                }
            }
            else if (!CurrentUnit.IsFall()) // Place River
            {
                ushort elevation = CurrentUnit.GetElevation();

                bool highlightMouth = fixRiverMouthToggle.Checked;

                if (highlightMouth)
                {
                    bool[,] mouthMarker = BuildMouthMarker();

                    if (mouthMarker[x - anchorX, y - anchorY])
                    {
                        bool[,] neighbour = FindSameNeighbourRiver(elevation, x, y, true);
                        CurrentUnit.UpdateRiver(neighbour, elevation);
                        bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y, true);
                        FixNeighbourTerrain(x, y, TerrainNeighbour, true);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    bool[,] neighbour = FindSameNeighbourRiver(elevation, x, y, true);
                    CurrentUnit.UpdateRiver(neighbour, elevation);
                    bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y, true);
                    FixNeighbourTerrain(x, y, TerrainNeighbour, true);
                }

                _ = UpdateMainMapAsync(x, y);
                AddMiniMapPixel(x, y, TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.River]);
            }
        }

        private void PlaceManualRiver(int x, int y)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            string type = FindModel(manualSelectedRiverModel);

            CurrentUnit.SetRiver(type, manualSelectedRiverElevation, manualSelectedRiverDirection);
            _ = UpdateMainMapAsync(x, y);
            AddMiniMapPixel(x, y, TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.River]);
        }

        private void CleanUpRiverOrFall(int x, int y, bool[,] neighbour, ushort PlacingElevation, ushort CurrentElevation)
        {
            if (PlacingElevation < CurrentElevation) //Removing
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (neighbour[i + 1, j + 1])
                        {
                            TerrainUnit CurrentNeighbour = terrainUnits[x + i][y + j];
                            if (CurrentNeighbour.GetElevation() != PlacingElevation)
                            {
                                if (CurrentNeighbour.IsFall())
                                    DeleteFall(x + i, y + j, false);
                                else if (CurrentNeighbour.IsRiver())
                                    DeleteRiver(x + i, y + j, false);
                            }
                        }
                    }
                }
            }
            else //Adding
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (neighbour[i + 1, j + 1])
                        {
                            TerrainUnit CurrentNeighbour = terrainUnits[x + i][y + j];
                            if (CurrentNeighbour.GetElevation() <= PlacingElevation)
                            {
                                if (CurrentNeighbour.IsFall())
                                    DeleteFall(x + i, y + j, false);
                                else if (CurrentNeighbour.IsRiver())
                                    DeleteRiver(x + i, y + j, false);
                            }
                        }
                    }
                }
            }
        }

        private void DeleteRoad(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = CurrentUnit.GetElevation();
            ushort road = 99;

            bool[,] neighbour = FindSameNeighbourRoad(road, elevation, x, y);
            CurrentUnit.UpdateRoad(road, neighbour);
            FixNeighbourRoad(x, y, road, neighbour);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteManualRoad(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = CurrentUnit.GetElevation();
            ushort road = 99;

            CurrentUnit.SetRoad(road, "", 0);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteFall(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = CurrentUnit.GetElevation();

            bool[,] neighbour = FindSameNeighbourCliff(elevation, x, y);
            CurrentUnit.UpdateCliff(neighbour, elevation);
            bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y);
            FixNeighbourTerrain(x, y, TerrainNeighbour);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteManualFall(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = CurrentUnit.GetElevation();

            bool[,] neighbour = FindSameNeighbourCliff(elevation, x, y);
            CurrentUnit.UpdateCliff(neighbour, elevation);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteRiver(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = CurrentUnit.GetElevation();
            bool[,] neighbour;

            if (elevation == 0)
            {
                neighbour = new bool[3, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        neighbour[i, j] = true;
                    }
                }
            }
            else
            {
                neighbour = FindSameNeighbourCliff(elevation, x, y);
            }

            CurrentUnit.UpdateCliff(neighbour, elevation);
            bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y, true);
            FixNeighbourTerrain(x, y, TerrainNeighbour, true);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteManualRiver(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = CurrentUnit.GetElevation();

            CurrentUnit.SetCliff("8", elevation, 0);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteCliff(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = 0;

            bool[,] neighbour = new bool[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    neighbour[i, j] = true;
                }
            }
            CurrentUnit.UpdateCliff(neighbour, elevation);
            bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y);
            FixNeighbourTerrain(x, y, TerrainNeighbour);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteManualCliff(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = 0;
            CurrentUnit.SetCliff("8", elevation, 0);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void UpdateToolTip(int Xcoordinate, int Ycoordinate)
        {
            if (DisplayInfoToggle.Checked)
            {
                string CoordinateText = "( " + (Xcoordinate + anchorX) + " , " + (Ycoordinate + anchorY) + " )";
                string NameText = "";
                if (terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].HasRoadWood())
                    NameText = "Wooden";
                else if (terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].HasRoadTile())
                    NameText = "Terra-cotta";
                else if (terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].HasRoadSand())
                    NameText = "Sand";
                else if (terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].HasRoadPattern())
                    NameText = "Arched tile";
                else if (terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].HasRoadDarkSoil())
                    NameText = "Dark dirt";
                else if (terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].HasRoadBrick())
                    NameText = "Brick";
                else if (terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].HasRoadStone())
                    NameText = "Stone";
                else if (terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].HasRoadSoil())
                    NameText = "Dirt";

                MapToolTip.Show(CoordinateText + " " + NameText + "\n" + terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].DisplayData(), MainMap, Xcoordinate * GridSize + GridSize, Ycoordinate * GridSize + GridSize);
            }
        }

        private void AddMiniMapPixel(int x, int y, Color c)
        {
            Bitmap myBitmap;

            myBitmap = new Bitmap(CurrentMiniMap);

            using (Graphics g = Graphics.FromImage(myBitmap))
            {
                g.SmoothingMode = SmoothingMode.None;
                Bitmap Bmp = new(2, 2);

                using (Graphics gfx = Graphics.FromImage(Bmp))
                using (SolidBrush brush = new(c))
                {
                    gfx.SmoothingMode = SmoothingMode.None;
                    gfx.FillRectangle(brush, 0, 0, 2, 2);
                }

                g.DrawImageUnscaled(Bmp, x * 2, y * 2);
            }

            miniMapBox.BackgroundImage = myBitmap;
            CurrentMiniMap = myBitmap;
        }

        private void RemoveMiniMapPixel(int x, int y, ushort elevation)
        {
            Bitmap myBitmap;

            myBitmap = new Bitmap(CurrentMiniMap);

            Color c;

            if (elevation == 0)
                c = MiniMap.GetBackgroundColorLess(x, y);
            else
            {
                int terrainNum = elevation + 19;
                c = TerrainUnit.TerrainColor[terrainNum];
            }

            using (Graphics g = Graphics.FromImage(myBitmap))
            {
                g.SmoothingMode = SmoothingMode.None;
                Bitmap Bmp = new(2, 2);

                using (Graphics gfx = Graphics.FromImage(Bmp))
                using (SolidBrush brush = new(c))
                {
                    gfx.SmoothingMode = SmoothingMode.None;
                    gfx.FillRectangle(brush, 0, 0, 2, 2);
                }

                g.DrawImageUnscaled(Bmp, x * 2, y * 2);
            }

            miniMapBox.BackgroundImage = myBitmap;
            CurrentMiniMap = myBitmap;
        }

        private void FixNeighbourRoad(int x, int y, ushort CurrentRoad, bool[,] CurrentNeighbour)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (x + i >= 0 && x + i < numOfColumn && y + j >= 0 && y + j < numOfRow)
                    {
                        if (CurrentNeighbour[i + 1, j + 1])
                        {
                            ushort NeighbourRoad = terrainUnits[x + i][y + j].GetRoadType();

                            bool[,] NeighbourNeighbour = FindSameNeighbourRoad(NeighbourRoad, terrainUnits[x + i][y + j].GetElevation(), x + i, y + j);
                            terrainUnits[x + i][y + j].UpdateRoad(NeighbourRoad, NeighbourNeighbour, terrainUnits[x + i][y + j].IsRoundCornerRoad());
                        }
                        else if (terrainUnits[x + i][y + j].HasRoad())
                        {
                            ushort NeighbourRoad = terrainUnits[x + i][y + j].GetRoadType();

                            if (NeighbourRoad != CurrentRoad)
                            {
                                bool[,] NeighbourNeighbour = FindSameNeighbourRoad(NeighbourRoad, terrainUnits[x + i][y + j].GetElevation(), x + i, y + j);
                                terrainUnits[x + i][y + j].UpdateRoad(NeighbourRoad, NeighbourNeighbour, terrainUnits[x + i][y + j].IsRoundCornerRoad());
                            }
                        }
                    }
                }
            }
        }

        private void FixNeighbourTerrain(int x, int y, bool[,] CurrentNeighbour, bool mouth = false)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (x + i >= 0 && x + i < numOfColumn && y + j >= 0 && y + j < numOfRow)
                    {
                        if (CurrentNeighbour[i + 1, j + 1])
                        {
                            ushort NeighbourElevation = terrainUnits[x + i][y + j].GetElevation();

                            if (terrainUnits[x + i][y + j].IsRiver())
                            {
                                bool[,] NeighbourNeighbour = FindSameNeighbourRiver(NeighbourElevation, x + i, y + j, mouth);
                                terrainUnits[x + i][y + j].UpdateRiver(NeighbourNeighbour, NeighbourElevation, terrainUnits[x + i][y + j].IsRoundCornerTerrain());
                            }
                            else if (terrainUnits[x + i][y + j].IsFall())
                            {
                                bool[,] NeighbourNeighbour = FindSameNeighbourFall(x + i, y + j);
                                ushort CurrentDirecttion = terrainUnits[x + i][y + j].GetTerrainAngle();
                                ushort CliffDirection = 0;
                                if (CurrentDirecttion == 0)
                                    CliffDirection = 1;
                                else if (CurrentDirecttion == 1)
                                    CliffDirection = 2;
                                else if (CurrentDirecttion == 2)
                                    CliffDirection = 3;
                                else if (CurrentDirecttion == 3)
                                    CliffDirection = 0;
                                terrainUnits[x + i][y + j].UpdateFall(NeighbourNeighbour, CliffDirection);
                            }
                            else
                            {
                                bool[,] NeighbourNeighbour = FindSameNeighbourCliff(NeighbourElevation, x + i, y + j);
                                terrainUnits[x + i][y + j].UpdateCliff(NeighbourNeighbour, NeighbourElevation, terrainUnits[x + i][y + j].IsRoundCornerTerrain());
                            }
                        }
                    }
                }
            }
        }

        private bool[,] FindTerrainNeighbourForFix(int x, int y, bool mouth = false)
        {
            bool[,] TerrainNeighbour = new bool[3, 3];
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == -1 && x <= 0)
                        TerrainNeighbour[i + 1, j + 1] = false;
                    else if (i == 1 && x >= numOfColumn - 1)
                        TerrainNeighbour[i + 1, j + 1] = false;
                    else if (j == -1 && y <= 0)
                        TerrainNeighbour[i + 1, j + 1] = false;
                    else if (j == 1 && y >= numOfRow - 1)
                        TerrainNeighbour[i + 1, j + 1] = false;
                    else
                    {
                        if (mouth)
                        {
                            if (terrainUnits[x + i][y + j].HasTerrain())
                                TerrainNeighbour[i + 1, j + 1] = true;
                            else if (MiniMap.GetBackgroundColorLess(x + i, y + j) == MiniMap.Pixel[0x0C])
                                TerrainNeighbour[i + 1, j + 1] = true;
                            else
                                TerrainNeighbour[i + 1, j + 1] = false;
                        }
                        else
                            TerrainNeighbour[i + 1, j + 1] = terrainUnits[x + i][y + j].HasTerrain();
                    }
                }
            }

            TerrainNeighbour[1, 1] = false;
            return TerrainNeighbour;
        }

        private bool[,] FindSameNeighbourRoad(ushort road, ushort elevation, int x, int y)
        {
            bool[,] sameNeighbour = new bool[3, 3];
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == -1 && x <= 0)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (i == 1 && x >= numOfColumn - 1)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (j == -1 && y <= 0)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (j == 1 && y >= numOfRow - 1)
                        sameNeighbour[i + 1, j + 1] = false;
                    else
                        sameNeighbour[i + 1, j + 1] = terrainUnits[x + i][y + j].IsSameRoadAndElevation(road, elevation);
                }
            }

            if (sameNeighbour[0, 1] == false) // Left
            {
                for (int j = 0; j < 3; j++)
                {
                    sameNeighbour[0, j] = false;
                }
            }
            if (sameNeighbour[1, 0] == false) // Up
            {
                for (int i = 0; i < 3; i++)
                {
                    sameNeighbour[i, 0] = false;
                }
            }
            if (sameNeighbour[2, 1] == false) // Right
            {
                for (int j = 0; j < 3; j++)
                {
                    sameNeighbour[2, j] = false;
                }
            }
            if (sameNeighbour[1, 2] == false) // Down
            {
                for (int i = 0; i < 3; i++)
                {
                    sameNeighbour[i, 2] = false;
                }
            }

            sameNeighbour[1, 1] = false;
            return sameNeighbour;
        }

        private bool[,] FindSameNeighbourCliff(ushort elevation, int x, int y)
        {
            bool[,] sameNeighbour = new bool[3, 3];
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == -1 && x <= 0)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (i == 1 && x >= numOfColumn - 1)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (j == -1 && y <= 0)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (j == 1 && y >= numOfRow - 1)
                        sameNeighbour[i + 1, j + 1] = false;
                    else
                        sameNeighbour[i + 1, j + 1] = terrainUnits[x + i][y + j].IsSameOrHigherElevationTerrain(elevation);
                }
            }

            if (sameNeighbour[0, 1] == false) // Left
            {
                for (int j = 0; j < 3; j++)
                {
                    sameNeighbour[0, j] = false;
                }
            }
            if (sameNeighbour[1, 0] == false) // Up
            {
                for (int i = 0; i < 3; i++)
                {
                    sameNeighbour[i, 0] = false;
                }
            }
            if (sameNeighbour[2, 1] == false) // Right
            {
                for (int j = 0; j < 3; j++)
                {
                    sameNeighbour[2, j] = false;
                }
            }
            if (sameNeighbour[1, 2] == false) // Down
            {
                for (int i = 0; i < 3; i++)
                {
                    sameNeighbour[i, 2] = false;
                }
            }

            sameNeighbour[1, 1] = false;
            return sameNeighbour;
        }

        private bool[,] FindSameNeighbourRiver(ushort elevation, int x, int y, bool mouth = false)
        {
            bool[,] sameNeighbour = new bool[3, 3];
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == -1 && x <= 0)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (i == 1 && x >= numOfColumn - 1)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (j == -1 && y <= 0)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (j == 1 && y >= numOfRow - 1)
                        sameNeighbour[i + 1, j + 1] = false;
                    else
                    {
                        if (mouth)
                        {
                            if (terrainUnits[x + i][y + j].IsSameOrHigherElevationRiverOrFall(elevation))
                                sameNeighbour[i + 1, j + 1] = true;
                            else if (MiniMap.GetBackgroundColorLess(x + i, y + j) == MiniMap.Pixel[0x0C])
                                sameNeighbour[i + 1, j + 1] = true;
                            else if (MiniMap.GetBackgroundColorLess(x + i, y + j) == MiniMap.Pixel[0x2A])
                                sameNeighbour[i + 1, j + 1] = true;
                            else
                                sameNeighbour[i + 1, j + 1] = false;
                        }
                        else
                            sameNeighbour[i + 1, j + 1] = terrainUnits[x + i][y + j].IsSameOrHigherElevationRiverOrFall(elevation);
                    }
                }
            }

            if (sameNeighbour[0, 1] == false) // Left
            {
                for (int j = 0; j < 3; j++)
                {
                    sameNeighbour[0, j] = false;
                }
            }
            if (sameNeighbour[1, 0] == false) // Up
            {
                for (int i = 0; i < 3; i++)
                {
                    sameNeighbour[i, 0] = false;
                }
            }
            if (sameNeighbour[2, 1] == false) // Right
            {
                for (int j = 0; j < 3; j++)
                {
                    sameNeighbour[2, j] = false;
                }
            }
            if (sameNeighbour[1, 2] == false) // Down
            {
                for (int i = 0; i < 3; i++)
                {
                    sameNeighbour[i, 2] = false;
                }
            }

            sameNeighbour[1, 1] = false;
            return sameNeighbour;
        }

        private bool[,] FindSameNeighbourFall(int x, int y)
        {
            bool[,] sameNeighbour = new bool[3, 3];
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == -1 && x <= 0)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (i == 1 && x >= numOfColumn - 1)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (j == -1 && y <= 0)
                        sameNeighbour[i + 1, j + 1] = false;
                    else if (j == 1 && y >= numOfRow - 1)
                        sameNeighbour[i + 1, j + 1] = false;
                    else
                        sameNeighbour[i + 1, j + 1] = terrainUnits[x + i][y + j].IsFallOrRiver();
                }
            }

            if (sameNeighbour[0, 1] == false) // Left
            {
                for (int j = 0; j < 3; j++)
                {
                    sameNeighbour[0, j] = false;
                }
            }
            if (sameNeighbour[1, 0] == false) // Up
            {
                for (int i = 0; i < 3; i++)
                {
                    sameNeighbour[i, 0] = false;
                }
            }
            if (sameNeighbour[2, 1] == false) // Right
            {
                for (int j = 0; j < 3; j++)
                {
                    sameNeighbour[2, j] = false;
                }
            }
            if (sameNeighbour[1, 2] == false) // Down
            {
                for (int i = 0; i < 3; i++)
                {
                    sameNeighbour[i, 2] = false;
                }
            }

            sameNeighbour[1, 1] = false;
            return sameNeighbour;
        }

        private void MainMap_MouseLeave(object sender, EventArgs e)
        {
            LastX = -1;
            LastY = -1;
            MapToolTip.Hide(MainMap);
        }

        private void MiniMapBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (miniMap == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                int x;
                int y;

                if (e.X / 2 < 8)
                    x = 8;
                else if (e.X / 2 > 104)
                    x = 104;
                else
                    x = e.X / 2;

                if (e.Y / 2 < 8)
                    y = 8;
                else if (e.Y / 2 > 88)
                    y = 88;
                else
                    y = e.Y / 2;

                anchorX = x - 8;
                anchorY = y - 8;
                LastChangedX = -1;
                LastChangedY = -1;

                miniMapBox.Image = MiniMap.DrawSelectSquare16(x, y);
                DrawMainMap(anchorX, anchorY);
                fixRiverMouthToggle.Checked = false;
            }
        }

        private void MiniMapBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (miniMap == null)
                return;

            if (mapDrawing)
                return;

            if (e.Button == MouseButtons.Left)
            {
                int x;
                int y;

                if (e.X / 2 < 8)
                    x = 8;
                else if (e.X / 2 > 104)
                    x = 104;
                else
                    x = e.X / 2;

                if (e.Y / 2 < 8)
                    y = 8;
                else if (e.Y / 2 > 88)
                    y = 88;
                else
                    y = e.Y / 2;

                anchorX = x - 8;
                anchorY = y - 8;
                LastChangedX = -1;
                LastChangedY = -1;

                miniMapBox.Image = MiniMap.DrawSelectSquare16(x, y);
                _ = DrawMainMapAsync(anchorX, anchorY);
                fixRiverMouthToggle.Checked = false;
            }
        }

        private void MainMap_MouseUp(object sender, MouseEventArgs e)
        {
            _ = DrawMainMapAsync(anchorX, anchorY, true);
        }

        private void ResetBtnColor(bool refresh)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            wasPlacing = false;
            wasRemoving = false;

            foreach (Button btn in AutoButtonPanel.Controls.OfType<Button>())
            {
                if (btn == ConfirmBtn)
                    continue;
                btn.BackColor = Color.FromArgb(114, 137, 218);
            }

            if (selectedButton != null)
            {
                selectedButton.BackColor = Color.LightSeaGreen;
            }

            if (cornerMode)
            {
                CornerBtn_Click(null, null);
            }

            UpdateHighlight();

            if (refresh)
                _ = DrawMainMapAsync(anchorX, anchorY);
        }
        private void CliffBtn_Click(object sender, EventArgs e)
        {
            selectedButton = CliffBtn;

            ResetBtnColor(true);
        }
        private void RiverBtn_Click(object sender, EventArgs e)
        {
            selectedButton = RiverBtn;

            ResetBtnColor(true);
        }
        private void CornerBtn_Click(object sender, EventArgs e)
        {
            if (selectedButton == null)
                return;

            if (cornerMode)
            {
                CornerBtn.BackColor = Color.FromArgb(114, 137, 218);
                HighlightCornerToggle.Checked = false;
                cornerMode = false;
            }
            else
            {
                CornerBtn.BackColor = Color.LightSeaGreen;
                cornerMode = true;
                HighlightCornerToggle.Checked = true;
            }

            UpdateHighlight();
        }
        private void DirtBtn_Click(object sender, EventArgs e)
        {
            selectedButton = DirtBtn;
            selectedRoad = 7;

            if (displayRoad)
                ResetBtnColor(true);
            else
            {
                ResetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void StoneBtn_Click(object sender, EventArgs e)
        {
            selectedButton = StoneBtn;
            selectedRoad = 6;

            if (displayRoad)
                ResetBtnColor(true);
            else
            {
                ResetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void BrickBtn_Click(object sender, EventArgs e)
        {
            selectedButton = BrickBtn;
            selectedRoad = 5;

            if (displayRoad)
                ResetBtnColor(true);
            else
            {
                ResetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void DarkDirtBtn_Click(object sender, EventArgs e)
        {
            selectedButton = DarkDirtBtn;
            selectedRoad = 4;

            if (displayRoad)
                ResetBtnColor(true);
            else
            {
                ResetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void ArchBtn_Click(object sender, EventArgs e)
        {
            selectedButton = ArchBtn;
            selectedRoad = 3;

            if (displayRoad)
                ResetBtnColor(true);
            else
            {
                ResetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void SandBtn_Click(object sender, EventArgs e)
        {
            selectedButton = SandBtn;
            selectedRoad = 2;

            if (displayRoad)
                ResetBtnColor(true);
            else
            {
                ResetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void TileBtn_Click(object sender, EventArgs e)
        {
            selectedButton = TileBtn;
            selectedRoad = 1;

            if (displayRoad)
                ResetBtnColor(true);
            else
            {
                ResetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void WoodBtn_Click(object sender, EventArgs e)
        {
            selectedButton = WoodBtn;
            selectedRoad = 0;

            if (displayRoad)
                ResetBtnColor(true);
            else
            {
                ResetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }

        private void UpdateHighlight()
        {
            highlightRoadCorner = false;
            highlightCliffCorner = false;
            highlightRiverCorner = false;

            if (HighlightCornerToggle.Checked)
            {
                if (selectedButton == CliffBtn)
                    highlightCliffCorner = true;
                else if (selectedButton == RiverBtn)
                    highlightRiverCorner = true;
                else if (selectedButton == DirtBtn || selectedButton == StoneBtn || selectedButton == BrickBtn ||
                         selectedButton == DarkDirtBtn || selectedButton == ArchBtn || selectedButton == SandBtn ||
                         selectedButton == TileBtn || selectedButton == WoodBtn)
                    highlightRoadCorner = true;
            }
        }

        private void MainMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (terrainSaving)
                return;

            int x = e.X;
            int y = e.Y;

            if (x < 0)
                x = 0;
            if (x > GridSize * 15)
                x = GridSize * 15;
            if (y < 0)
                y = 0;
            if (y > GridSize * 15)
                y = GridSize * 15;

            int Xcoordinate = x / GridSize;
            int Ycoordinate = y / GridSize;

            if (Xcoordinate != LastX || Ycoordinate != LastY)
            {
                LastX = Xcoordinate;
                LastY = Ycoordinate;
            }

            TerrainUnit CurrentUnit = terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY];

            if (customModeActivated)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (CustomDesignList.FocusedItem == null)
                        return;
                    if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                        return;

                    byte[] CustomDesignBytes = BitConverter.GetBytes(CustomDesignList.FocusedItem.Index);
                    byte[] Half = [CustomDesignBytes[0], CustomDesignBytes[1]];

                    PlaceDesign(Half, Xcoordinate + anchorX, Ycoordinate + anchorY);

                    if (firstCustomEdit)
                    {
                        firstCustomEdit = false;
                        haveCustomEdit = true;
                        ConfirmBtn.Visible = true;
                    }

                    wasPlacing = true;
                    wasRemoving = false;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;
                    UpdateToolTip(Xcoordinate, Ycoordinate);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasRemoving)
                        return;

                    RemoveDesign(Xcoordinate + anchorX, Ycoordinate + anchorY);

                    if (firstCustomEdit)
                    {
                        firstCustomEdit = false;
                        haveCustomEdit = true;
                        ConfirmBtn.Visible = true;
                    }

                    wasPlacing = false;
                    wasRemoving = true;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;

                    UpdateToolTip(Xcoordinate, Ycoordinate);
                }
            }
            else if (manualModeActivated)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (selectedManualMode == ManualCliffPanel)
                    {
                        if (manualSelectedCliffModel == null)
                            return;
                        else
                        {
                            if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                                return;

                            PlaceManualCliff(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        }
                    }
                    else if (selectedManualMode == ManualRiverPanel)
                    {
                        if (manualSelectedRiverModel == null)
                            return;
                        else
                        {
                            if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                                return;

                            PlaceManualRiver(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        }
                    }
                    else if (selectedManualMode == ManualRoadPanel)
                    {
                        if (manualSelectedRoadModel == null)
                            return;
                        else
                        {
                            if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                                return;

                            if (CurrentUnit.IsFallOrRiver())
                                return;

                            if (CurrentUnit.IsRoundCornerTerrain())
                                return;

                            PlaceManualRoad(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        }
                    }

                    if (firstEdit)
                    {
                        firstEdit = false;
                        ConfirmBtn.Visible = true;
                    }

                    wasPlacing = true;
                    wasRemoving = false;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;
                    UpdateToolTip(Xcoordinate, Ycoordinate);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasRemoving)
                        return;

                    if (selectedManualMode == ManualCliffPanel)
                    {
                        DeleteManualCliff(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }
                    else if (selectedManualMode == ManualRiverPanel)
                    {
                        if (CurrentUnit.IsFall())
                            DeleteManualFall(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.IsRiver())
                            DeleteManualRiver(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }
                    else if (selectedManualMode == ManualRoadPanel)
                    {
                        DeleteManualRoad(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }

                    if (firstEdit)
                    {
                        firstEdit = false;
                        ConfirmBtn.Visible = true;
                    }

                    wasPlacing = false;
                    wasRemoving = true;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;

                    UpdateToolTip(Xcoordinate, Ycoordinate);
                }
            }
            else
            {
                if (selectedButton == null)
                    return;

                if (e.Button == MouseButtons.Left)
                {
                    if (cornerMode)
                    {
                        if (CurrentUnit.CanChangeCornerRoad())
                            ChangeRoadCorner(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.CanChangeCornerCliff())
                            ChangeCliffCorner(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.CanChangeCornerRiver())
                            ChangeRiverCorner(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else
                            return;
                    }
                    else if (selectedButton == CliffBtn)
                    {
                        if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                            return;

                        ushort elevation = (ushort)ElevationBar.Value;
                        PlaceCliff(Xcoordinate + anchorX, Ycoordinate + anchorY, elevation);
                    }
                    else if (selectedButton == RiverBtn)
                    {
                        if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                            return;

                        if (CurrentUnit.IsFallOrRiver())
                            return;

                        PlaceRiverOrFall(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }
                    else
                    {
                        if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                            return;

                        if (CurrentUnit.IsFallOrRiver())
                            return;

                        if (CurrentUnit.IsRoundCornerTerrain())
                            return;

                        PlaceRoad(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }

                    if (firstEdit)
                    {
                        firstEdit = false;
                        ConfirmBtn.Visible = true;
                    }

                    wasPlacing = true;
                    wasRemoving = false;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;
                    UpdateToolTip(Xcoordinate, Ycoordinate);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasRemoving)
                        return;

                    if (selectedButton == CliffBtn)
                    {
                        PlaceCliff(Xcoordinate + anchorX, Ycoordinate + anchorY, 0);
                    }
                    else if (selectedButton == RiverBtn)
                    {
                        if (CurrentUnit.IsFall())
                            DeleteFall(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.IsRiver())
                            DeleteRiver(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }
                    else
                    {
                        DeleteRoad(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }

                    if (firstEdit)
                    {
                        firstEdit = false;
                        ConfirmBtn.Visible = true;
                    }

                    wasPlacing = false;
                    wasRemoving = true;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;

                    UpdateToolTip(Xcoordinate, Ycoordinate);
                }
            }
        }

        private void MainMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (terrainSaving)
                return;

            int x = e.X;
            int y = e.Y;

            if (x < 0)
                x = 0;
            if (x > GridSize * 15)
                x = GridSize * 15;
            if (y < 0)
                y = 0;
            if (y > GridSize * 15)
                y = GridSize * 15;

            int Xcoordinate = x / GridSize;
            int Ycoordinate = y / GridSize;

            if (Xcoordinate != LastX || Ycoordinate != LastY)
            {
                LastX = Xcoordinate;
                LastY = Ycoordinate;
                UpdateToolTip(Xcoordinate, Ycoordinate);
            }

            TerrainUnit CurrentUnit = terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY];
            if (customModeActivated)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (CustomDesignList.FocusedItem == null)
                        return;
                    if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                        return;
                    byte[] CustomDesignBytes = BitConverter.GetBytes(CustomDesignList.FocusedItem.Index);
                    byte[] Half = [CustomDesignBytes[0], CustomDesignBytes[1]];

                    PlaceDesign(Half, Xcoordinate + anchorX, Ycoordinate + anchorY);

                    if (firstCustomEdit)
                    {
                        firstCustomEdit = false;
                        haveCustomEdit = true;
                        ConfirmBtn.Visible = true;
                    }

                    wasPlacing = true;
                    wasRemoving = false;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;
                    UpdateToolTip(Xcoordinate, Ycoordinate);

                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasRemoving)
                        return;

                    RemoveDesign(Xcoordinate + anchorX, Ycoordinate + anchorY);

                    if (firstCustomEdit)
                    {
                        firstCustomEdit = false;
                        haveCustomEdit = true;
                        ConfirmBtn.Visible = true;
                    }

                    wasPlacing = false;
                    wasRemoving = true;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;

                    UpdateToolTip(Xcoordinate, Ycoordinate);
                }
            }
            else if (manualModeActivated)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (selectedManualMode == ManualCliffPanel)
                    {
                        if (manualSelectedCliffModel == null)
                            return;
                        else
                        {
                            if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                                return;

                            PlaceManualCliff(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        }
                    }
                    else if (selectedManualMode == ManualRiverPanel)
                    {
                        if (manualSelectedRiverModel == null)
                            return;
                        else
                        {
                            if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                                return;

                            PlaceManualRiver(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        }
                    }
                    else if (selectedManualMode == ManualRoadPanel)
                    {
                        if (manualSelectedRoadModel == null)
                            return;
                        else
                        {
                            if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                                return;

                            if (CurrentUnit.IsFallOrRiver())
                                return;

                            if (CurrentUnit.IsRoundCornerTerrain())
                                return;

                            PlaceManualRoad(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        }
                    }

                    if (firstEdit)
                    {
                        firstEdit = false;
                        ConfirmBtn.Visible = true;
                    }

                    wasPlacing = true;
                    wasRemoving = false;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;
                    UpdateToolTip(Xcoordinate, Ycoordinate);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasRemoving)
                        return;

                    if (selectedManualMode == ManualCliffPanel)
                    {
                        DeleteManualCliff(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }
                    else if (selectedManualMode == ManualRiverPanel)
                    {
                        if (CurrentUnit.IsFall())
                            DeleteManualFall(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.IsRiver())
                            DeleteManualRiver(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }
                    else if (selectedManualMode == ManualRoadPanel)
                    {
                        DeleteManualRoad(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }

                    if (firstEdit)
                    {
                        firstEdit = false;
                        ConfirmBtn.Visible = true;
                    }

                    wasPlacing = false;
                    wasRemoving = true;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;

                    UpdateToolTip(Xcoordinate, Ycoordinate);
                }
            }
            else
            {
                if (selectedButton == null)
                    return;

                if (e.Button == MouseButtons.Left)
                {
                    if (cornerMode)
                    {
                        if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                            return;

                        if (CurrentUnit.CanChangeCornerRoad())
                            ChangeRoadCorner(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.CanChangeCornerCliff())
                            ChangeCliffCorner(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.CanChangeCornerRiver())
                            ChangeRiverCorner(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else
                            return;
                    }
                    else if (selectedButton == CliffBtn)
                    {
                        if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                            return;

                        ushort elevation = (ushort)ElevationBar.Value;
                        PlaceCliff(Xcoordinate + anchorX, Ycoordinate + anchorY, elevation);
                    }
                    else if (selectedButton == RiverBtn)
                    {
                        if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                            return;

                        if (CurrentUnit.IsFallOrRiver())
                            return;

                        PlaceRiverOrFall(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }
                    else
                    {
                        if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                            return;

                        if (CurrentUnit.IsFallOrRiver())
                            return;

                        if (CurrentUnit.IsRoundCornerTerrain())
                            return;

                        PlaceRoad(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }

                    wasPlacing = true;
                    wasRemoving = false;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;
                    UpdateToolTip(Xcoordinate, Ycoordinate);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasRemoving)
                        return;

                    if (selectedButton == CliffBtn)
                    {
                        PlaceCliff(Xcoordinate + anchorX, Ycoordinate + anchorY, 0);
                    }
                    else if (selectedButton == RiverBtn)
                    {
                        if (CurrentUnit.IsFall())
                            DeleteFall(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.IsRiver())
                            DeleteRiver(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }
                    else
                    {
                        DeleteRoad(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }

                    wasPlacing = false;
                    wasRemoving = true;

                    LastChangedX = Xcoordinate;
                    LastChangedY = Ycoordinate;

                    UpdateToolTip(Xcoordinate, Ycoordinate);
                }
            }
        }

        private void DisplayRoadToggle_CheckedChanged(object sender, EventArgs e)
        {
            displayRoad = DisplayRoadToggle.Checked;
            _ = DrawMainMapAsync(anchorX, anchorY);
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            Thread LoadThread = new(FetchMap);
            LoadThread.Start();
        }

        private void ElevationBar_ValueChanged(object sender, EventArgs e)
        {
            CliffBtn_Click(null, null);
            LastChangedX = -1;
            LastChangedY = -1;
        }

        private void HighlightCornerToggle_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHighlight();
            _ = DrawMainMapAsync(anchorX, anchorY);
        }

        private void DisplayBuildingToggle_CheckedChanged(object sender, EventArgs e)
        {
            displayBuilding = DisplayBuildingToggle.Checked;
            _ = DrawMainMapAsync(anchorX, anchorY);
        }

        private void ConfirmBtn_Click(object sender, EventArgs e)
        {
            terrainSaving = true;
            ConfirmBtn.Visible = false;

            SaveFileDialog file = new();

            byte[] CurrentTerrainData = Terrain;

            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("en-US");
            file.FileName = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder + localDate.ToString(culture).Replace(" ", "_").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("|", "-").Replace(".", "-") + ".nht";
            File.WriteAllBytes(file.FileName, CurrentTerrainData);

            byte[] newTerrain = new byte[numOfRow * numOfColumn * TerrainSize];

            for (int i = 0; i < numOfColumn; i++)
            {
                for (int j = 0; j < numOfRow; j++)
                {
                    byte[] TileData = terrainUnits[i][j].GetTerrainData();
                    Buffer.BlockCopy(TileData, 0, newTerrain, (i * numOfRow * TerrainSize) + (j * TerrainSize), TerrainSize);
                }
            }

            byte[] newCustomMap = null;

            if (haveCustomEdit)
            {
                newCustomMap = new byte[numOfRow * numOfColumn * 2];

                for (int i = 0; i < numOfColumn; i++)
                {
                    for (int j = 0; j < numOfRow; j++)
                    {
                        byte[] CustomDesignData = terrainUnits[i][j].GetCustomDesign();
                        Buffer.BlockCopy(CustomDesignData, 0, newCustomMap, (i * numOfRow * 2) + (j * 2), 2);
                    }
                }
            }

            ShowMapWait(40);

            Thread SendTerrainThread = new(delegate () { SendTerrain(newTerrain, newCustomMap); });
            SendTerrainThread.Start();
        }

        private void SendTerrain(byte[] newTerrain, byte[] newCustomMap)
        {
            int wait = 0;
            while (Utilities.IsAboutToSave(socket, null, 20))
            {
                if (wait > 15)
                    break;
                Thread.Sleep(2000);
                wait++;
            }

            Utilities.SendTerrain(socket, null, newTerrain, ref counter);

            if (newCustomMap != null)
            {
                Utilities.SendCustomMap(socket, null, newCustomMap, ref counter);
            }

            Invoke((MethodInvoker)delegate
            {
                miniMap.UpdateTerrain(newTerrain, newCustomMap);
                CurrentMiniMap = miniMap.DrawBackground();
                miniMapBox.BackgroundImage = CurrentMiniMap;
                terrainSaving = false;
                firstEdit = true;
                firstCustomEdit = true;
                haveCustomEdit = false;
                HideMapWait();
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (counter <= MapProgressBar.Maximum)
                MapProgressBar.Value = counter;
            else
                MapProgressBar.Value = MapProgressBar.Maximum;
        }

        private void RoadRoller_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseForm?.Invoke();
        }

        private void AutoModeButton_Click(object sender, EventArgs e)
        {
            manualModeActivated = false;
            customModeActivated = false;
            DisplayCustomDesignToggle.Checked = false;

            AutoModeButton.BackColor = Color.FromArgb(80, 80, 255);
            ManualModeButton.BackColor = Color.FromArgb(114, 137, 218);
            CustomModeButton.BackColor = Color.FromArgb(114, 137, 218);

            AutoButtonPanel.Visible = true;
            ManualButtonPanel.Visible = false;
            CustomDesignList.Visible = false;

            LastChangedX = -1;
            LastChangedY = -1;

            wasPlacing = false;
            wasRemoving = false;
        }

        private void ManualModeButton_Click(object sender, EventArgs e)
        {
            manualModeActivated = true;
            customModeActivated = false;
            DisplayCustomDesignToggle.Checked = false;

            ManualModeButton.BackColor = Color.FromArgb(80, 80, 255);
            AutoModeButton.BackColor = Color.FromArgb(114, 137, 218);
            CustomModeButton.BackColor = Color.FromArgb(114, 137, 218);

            AutoButtonPanel.Visible = false;
            ManualButtonPanel.Visible = true;
            CustomDesignList.Visible = false;

            LastChangedX = -1;
            LastChangedY = -1;

            wasPlacing = false;
            wasRemoving = false;

            selectedButton = null;
            ResetBtnColor(false);
        }

        private void CustomModeButton_Click(object sender, EventArgs e)
        {
            manualModeActivated = false;
            customModeActivated = true;
            DisplayCustomDesignToggle.Checked = true;

            ManualModeButton.BackColor = Color.FromArgb(114, 137, 218);
            AutoModeButton.BackColor = Color.FromArgb(114, 137, 218);
            CustomModeButton.BackColor = Color.FromArgb(80, 80, 255);

            AutoButtonPanel.Visible = false;
            ManualButtonPanel.Visible = false;
            CustomDesignList.Visible = true;

            LastChangedX = -1;
            LastChangedY = -1;

            wasPlacing = false;
            wasRemoving = false;

            selectedButton = null;
            ResetBtnColor(false);
        }

        private void DrawRoadImages(int direction = 0, int road = 7, int size = 36)
        {
            var RoadColor = road switch
            {
                0 => TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadWood],
                1 => TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadTile],
                2 => TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadSand],
                3 => TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadPattern],
                4 => TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadDarkSoil],
                5 => TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadBrick],
                6 => TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadStone],
                _ => TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadSoil],
            };
            Bitmap Road0A = new(size, size);
            Bitmap Road0B = new(size, size);
            Bitmap Road1A = new(size, size);
            Bitmap Road1B = new(size, size);
            Bitmap Road1C = new(size, size);
            Bitmap Road2A = new(size, size);
            Bitmap Road2B = new(size, size);
            Bitmap Road2C = new(size, size);
            Bitmap Road3A = new(size, size);
            Bitmap Road3B = new(size, size);
            Bitmap Road3C = new(size, size);
            Bitmap Road4A = new(size, size);
            Bitmap Road4B = new(size, size);
            Bitmap Road4C = new(size, size);
            Bitmap Road5A = new(size, size);
            Bitmap Road5B = new(size, size);
            Bitmap Road6A = new(size, size);
            Bitmap Road6B = new(size, size);
            Bitmap Road7A = new(size, size);
            Bitmap Road8A = new(size, size);

            SolidBrush RoadBrush = new(RoadColor);

            using (Graphics gr = Graphics.FromImage(Road0A))
            {
                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 16);
            }
            using (Graphics gr = Graphics.FromImage(Road0B))
            {
                Rectangle pieRect = new(8, 8, (size - 16) * 2, (size - 16) * 2);
                gr.FillPie(RoadBrush, pieRect, -90, -90);
            }
            using (Graphics gr = Graphics.FromImage(Road1A))
            {
                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 9);
            }
            using (Graphics gr = Graphics.FromImage(Road1B))
            {
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                Rectangle pieRect = new(8, 8, (size - 16) * 2, (size - 16) * 2);
                gr.FillPie(RoadBrush, pieRect, -90, -90);
            }
            using (Graphics gr = Graphics.FromImage(Road1C))
            {
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                Rectangle pieRect = new(8 - (size - 16), 8, (size - 16) * 2, (size - 16) * 2);
                gr.FillPie(RoadBrush, pieRect, 0, -90);
            }
            using (Graphics gr = Graphics.FromImage(Road2A))
            {
                gr.FillRectangle(RoadBrush, 8, 1, size - 16, 7); //Top
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 10);
            }
            using (Graphics gr = Graphics.FromImage(Road2B))
            {
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                Rectangle pieRect = new(8, 8, (size - 16) * 2, (size - 16) * 2);
                gr.FillPie(RoadBrush, pieRect, -90, -90);
            }
            using (Graphics gr = Graphics.FromImage(Road2C))
            {
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 16);
            }
            using (Graphics gr = Graphics.FromImage(Road3A))
            {
                gr.FillRectangle(RoadBrush, 8, 1, size - 16, 7); //Top
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 10);
            }
            using (Graphics gr = Graphics.FromImage(Road3B))
            {
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8 + (size - 16), 7, 7); //Bottom Right

                Rectangle pieRect = new(8, 8, (size - 16) * 2, (size - 16) * 2);
                gr.FillPie(RoadBrush, pieRect, -90, -90);
            }
            using (Graphics gr = Graphics.FromImage(Road3C))
            {
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8 + (size - 16), 7, 7); //Bottom Right

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 16);
            }
            using (Graphics gr = Graphics.FromImage(Road4A))
            {
                gr.FillRectangle(RoadBrush, 8, 1, size - 16, 7); //Top
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8 + (size - 16), 7, 7); //Bottom Right

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 10);
            }
            using (Graphics gr = Graphics.FromImage(Road4B))
            {
                gr.FillRectangle(RoadBrush, 8, 1, size - 16, 7); //Top
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 1, 7, 7); //Top Right

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 10);
            }
            using (Graphics gr = Graphics.FromImage(Road4C))
            {
                gr.FillRectangle(RoadBrush, 8, 1, size - 16, 7); //Top
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                gr.FillRectangle(RoadBrush, 1, 8, 7, size - 16); //Left

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 10);
            }
            using (Graphics gr = Graphics.FromImage(Road5A))
            {
                gr.FillRectangle(RoadBrush, 8, 1, size - 16, 7); //Top
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                gr.FillRectangle(RoadBrush, 1, 8, 7, size - 16); //Left
                gr.FillRectangle(RoadBrush, 1, 8 + (size - 16), 7, 7); //Bottom Left

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 10);
            }
            using (Graphics gr = Graphics.FromImage(Road5B))
            {
                gr.FillRectangle(RoadBrush, 8, 1, size - 16, 7); //Top
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                gr.FillRectangle(RoadBrush, 8 + (size - 16), 1, 7, 7); //Top Right
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8 + (size - 16), 7, 7); //Bottom Right

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 10);
            }
            using (Graphics gr = Graphics.FromImage(Road6A))
            {
                gr.FillRectangle(RoadBrush, 8, 1, size - 16, 7); //Top
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                gr.FillRectangle(RoadBrush, 1, 8, 7, size - 16); //Left
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 1, 7, 7); //Top Right
                gr.FillRectangle(RoadBrush, 1, 8 + (size - 16), 7, 7); //Bottom Left

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 10);
            }
            using (Graphics gr = Graphics.FromImage(Road6B))
            {
                gr.FillRectangle(RoadBrush, 8, 1, size - 16, 7); //Top
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                gr.FillRectangle(RoadBrush, 1, 8, 7, size - 16); //Left
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 1, 7, 7); //Top Right
                gr.FillRectangle(RoadBrush, 1, 1, 7, 7); //Top Left

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 10);
            }
            using (Graphics gr = Graphics.FromImage(Road7A))
            {
                gr.FillRectangle(RoadBrush, 8, 1, size - 16, 7); //Top
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                gr.FillRectangle(RoadBrush, 1, 8, 7, size - 16); //Left
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 1, 7, 7); //Top Right
                gr.FillRectangle(RoadBrush, 1, 1, 7, 7); //Top Left
                gr.FillRectangle(RoadBrush, 8 + (size - 16), 8 + (size - 16), 7, 7); //Bottom Right

                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 10);
            }
            using (Graphics gr = Graphics.FromImage(Road8A))
            {
                gr.FillRectangle(RoadBrush, 1, 1, size - 2, size - 2);
            }

            if (direction == 1)
            {
                Road0B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road1A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road1B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road1C.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road2A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road2B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road2C.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road3A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road3B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road3C.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road4A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road4B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road5A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road5B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road6A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road6B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Road7A.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            else if (direction == 2)
            {
                Road0B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road1A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road1B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road1C.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road2A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road2B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road2C.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road3A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road3B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road3C.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road4A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road4B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road5A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road5B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road6A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road6B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Road7A.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
            else if (direction == 3)
            {
                Road0B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road1A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road1B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road1C.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road2A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road2B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road2C.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road3A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road3B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road3C.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road4A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road4B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road5A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road5B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road6A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road6B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Road7A.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }

            RoadButton0A.BackgroundImage = Road0A;
            RoadButton0B.BackgroundImage = Road0B;
            RoadButton1A.BackgroundImage = Road1A;
            RoadButton1B.BackgroundImage = Road1B;
            RoadButton1C.BackgroundImage = Road1C;
            RoadButton2A.BackgroundImage = Road2A;
            RoadButton2B.BackgroundImage = Road2B;
            RoadButton2C.BackgroundImage = Road2C;
            RoadButton3A.BackgroundImage = Road3A;
            RoadButton3B.BackgroundImage = Road3B;
            RoadButton3C.BackgroundImage = Road3C;
            RoadButton4A.BackgroundImage = Road4A;
            RoadButton4B.BackgroundImage = Road4B;
            RoadButton4C.BackgroundImage = Road4C;
            RoadButton5A.BackgroundImage = Road5A;
            RoadButton5B.BackgroundImage = Road5B;
            RoadButton6A.BackgroundImage = Road6A;
            RoadButton6B.BackgroundImage = Road6B;
            RoadButton7A.BackgroundImage = Road7A;
            RoadButton8A.BackgroundImage = Road8A;
        }

        private void DrawCliffImages(int direction = 0, int elevation = 1, int size = 36)
        {
            Color CliffColor;

            if (elevation == 1)
            {
                CliffColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation1];

                foreach (Button btn in ManualCliffPanel.Controls.OfType<Button>())
                {
                    if (btn == RotateCliffButton)
                        continue;
                    btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation0];
                }
            }
            else if (elevation == 2)
            {
                CliffColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation2];

                foreach (Button btn in ManualCliffPanel.Controls.OfType<Button>())
                {
                    if (btn == RotateCliffButton)
                        continue;
                    btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation1];
                }
            }
            else if (elevation >= 3)
            {
                CliffColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation3];

                foreach (Button btn in ManualCliffPanel.Controls.OfType<Button>())
                {
                    if (btn == RotateCliffButton)
                        continue;
                    btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation2];
                }
            }
            else
            {
                CliffColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation1];

                foreach (Button btn in ManualCliffPanel.Controls.OfType<Button>())
                {
                    if (btn == RotateCliffButton)
                        continue;
                    btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation0];
                }
            }

            Bitmap Cliff0A = new(size, size);
            Bitmap Cliff1A = new(size, size);
            Bitmap Cliff2A = new(size, size);
            Bitmap Cliff2B = new(size, size);
            Bitmap Cliff2C = new(size, size);
            Bitmap Cliff3A = new(size, size);
            Bitmap Cliff3B = new(size, size);
            Bitmap Cliff3C = new(size, size);
            Bitmap Cliff4A = new(size, size);
            Bitmap Cliff4B = new(size, size);
            Bitmap Cliff4C = new(size, size);
            Bitmap Cliff5A = new(size, size);
            Bitmap Cliff5B = new(size, size);
            Bitmap Cliff6A = new(size, size);
            Bitmap Cliff6B = new(size, size);
            Bitmap Cliff7A = new(size, size);
            Bitmap Cliff8 = new(size, size);

            SolidBrush CliffBrush = new(CliffColor);

            using (Graphics gr = Graphics.FromImage(Cliff0A))
            {
                gr.FillRectangle(CliffBrush, 4, 4, size - 8, size - 8);
            }
            using (Graphics gr = Graphics.FromImage(Cliff1A))
            {
                gr.FillRectangle(CliffBrush, 4, 4, size - 8, size - 5);
            }
            using (Graphics gr = Graphics.FromImage(Cliff2A))
            {
                gr.FillRectangle(CliffBrush, 4, 1, size - 8, size - 2);
            }
            using (Graphics gr = Graphics.FromImage(Cliff2B))
            {
                Point[] Terrain2B = [new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1)];
                gr.FillPolygon(CliffBrush, Terrain2B);
            }
            using (Graphics gr = Graphics.FromImage(Cliff2C))
            {
                Point[] Terrain2C = [new Point(4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1)];
                gr.FillPolygon(CliffBrush, Terrain2C);
            }
            using (Graphics gr = Graphics.FromImage(Cliff3A))
            {
                Point[] Terrain3A = [new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1)];
                gr.FillPolygon(CliffBrush, Terrain3A);
            }
            using (Graphics gr = Graphics.FromImage(Cliff3B))
            {
                Point[] Terrain3B = [new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1)];
                gr.FillPolygon(CliffBrush, Terrain3B);
            }
            using (Graphics gr = Graphics.FromImage(Cliff3C))
            {
                gr.FillRectangle(CliffBrush, 4, 4, size - 5, size - 5);
            }
            using (Graphics gr = Graphics.FromImage(Cliff4A))
            {
                Point[] Terrain4A = [new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1)];
                gr.FillPolygon(CliffBrush, Terrain4A);
            }
            using (Graphics gr = Graphics.FromImage(Cliff4B))
            {
                Point[] Terrain4B = [new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1)];
                gr.FillPolygon(CliffBrush, Terrain4B);
            }
            using (Graphics gr = Graphics.FromImage(Cliff4C))
            {
                Point[] Terrain4C = [new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4)];
                gr.FillPolygon(CliffBrush, Terrain4C);
            }
            using (Graphics gr = Graphics.FromImage(Cliff5A))
            {
                Point[] Terrain5A = [new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4)];
                gr.FillPolygon(CliffBrush, Terrain5A);
            }
            using (Graphics gr = Graphics.FromImage(Cliff5B))
            {
                gr.FillRectangle(CliffBrush, 4, 1, size - 5, size - 2);
            }
            using (Graphics gr = Graphics.FromImage(Cliff6A))
            {
                Point[] Terrain6A = [new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4)];
                gr.FillPolygon(CliffBrush, Terrain6A);
            }
            using (Graphics gr = Graphics.FromImage(Cliff6B))
            {
                Point[] Terrain6B = [new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4)];
                gr.FillPolygon(CliffBrush, Terrain6B);
            }
            using (Graphics gr = Graphics.FromImage(Cliff7A))
            {
                Point[] Terrain7A = [new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4)];
                gr.FillPolygon(CliffBrush, Terrain7A);
            }
            using (Graphics gr = Graphics.FromImage(Cliff8))
            {
                gr.FillRectangle(CliffBrush, 1, 1, size - 2, size - 2);
            }

            if (direction == 1)
            {
                Cliff1A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff2A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff2B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff2C.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff3A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff3B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff3C.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff4A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff4B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff5A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff5B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff6A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff6B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                Cliff7A.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            else if (direction == 2)
            {
                Cliff1A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff2A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff2B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff2C.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff3A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff3B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff3C.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff4A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff4B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff5A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff5B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff6A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff6B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                Cliff7A.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
            else if (direction == 3)
            {
                Cliff1A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff2A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff2B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff2C.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff3A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff3B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff3C.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff4A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff4B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff5A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff5B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff6A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff6B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                Cliff7A.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }

            CliffButton0A.BackgroundImage = Cliff0A;
            CliffButton1A.BackgroundImage = Cliff1A;
            CliffButton2A.BackgroundImage = Cliff2A;
            CliffButton2B.BackgroundImage = Cliff2B;
            CliffButton2C.BackgroundImage = Cliff2C;
            CliffButton3A.BackgroundImage = Cliff3A;
            CliffButton3B.BackgroundImage = Cliff3B;
            CliffButton3C.BackgroundImage = Cliff3C;
            CliffButton4A.BackgroundImage = Cliff4A;
            CliffButton4B.BackgroundImage = Cliff4B;
            CliffButton4C.BackgroundImage = Cliff4C;
            CliffButton5A.BackgroundImage = Cliff5A;
            CliffButton5B.BackgroundImage = Cliff5B;
            CliffButton6A.BackgroundImage = Cliff6A;
            CliffButton6B.BackgroundImage = Cliff6B;
            CliffButton7A.BackgroundImage = Cliff7A;
            CliffButton8.BackgroundImage = Cliff8;

            if (manualSelectedCliffModel != null)
                manualSelectedCliffModel.BackColor = Color.Violet;
        }

        private void DrawRiverImages(int direction = 0, int elevation = 0, int size = 36)
        {
            Color RiverColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.River];

            if (elevation == 0)
            {
                foreach (Button btn in ManualRiverPanel.Controls.OfType<Button>())
                {
                    if (btn == RotateRiverButton)
                        continue;
                    btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation0];
                }
            }
            else if (elevation == 1)
            {
                foreach (Button btn in ManualRiverPanel.Controls.OfType<Button>())
                {
                    if (btn == RotateRiverButton)
                        continue;
                    btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation1];
                }
            }
            else if (elevation == 2)
            {
                foreach (Button btn in ManualRiverPanel.Controls.OfType<Button>())
                {
                    if (btn == RotateRiverButton)
                        continue;
                    btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation2];
                }
            }
            else if (elevation >= 3)
            {
                foreach (Button btn in ManualRiverPanel.Controls.OfType<Button>())
                {
                    if (btn == RotateRiverButton)
                        continue;
                    btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation3];
                }
            }
            else
            {
                foreach (Button btn in ManualCliffPanel.Controls.OfType<Button>())
                {
                    if (btn == RotateCliffButton)
                        continue;
                    btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation0];
                }
            }

            Bitmap River0A = new(size, size);
            Bitmap River1A = new(size, size);
            Bitmap River2A = new(size, size);
            Bitmap River2B = new(size, size);
            Bitmap River2C = new(size, size);
            Bitmap River3A = new(size, size);
            Bitmap River3B = new(size, size);
            Bitmap River3C = new(size, size);
            Bitmap River4A = new(size, size);
            Bitmap River4B = new(size, size);
            Bitmap River4C = new(size, size);
            Bitmap River5A = new(size, size);
            Bitmap River5B = new(size, size);
            Bitmap River6A = new(size, size);
            Bitmap River6B = new(size, size);
            Bitmap River7A = new(size, size);
            Bitmap River8A = new(size, size);

            SolidBrush RiverBrush = new(RiverColor);

            using (Graphics gr = Graphics.FromImage(River0A))
            {
                gr.FillRectangle(RiverBrush, 8, 8, size - 16, size - 16);
            }
            using (Graphics gr = Graphics.FromImage(River1A))
            {
                gr.FillRectangle(RiverBrush, 8, 8, size - 16, size - 9);
            }
            using (Graphics gr = Graphics.FromImage(River2A))
            {
                gr.FillRectangle(RiverBrush, 8, 1, size - 16, size - 2);
            }
            using (Graphics gr = Graphics.FromImage(River2B))
            {
                Point[] Terrain2B = [new Point(size - 1, 8), new Point(size - 1, size - 8), new Point(size - 8, size - 1), new Point(8, size - 1)];
                gr.FillPolygon(RiverBrush, Terrain2B);
            }
            using (Graphics gr = Graphics.FromImage(River2C))
            {
                Point[] Terrain2C = [new Point(8, 8), new Point(size - 1, 8), new Point(size - 1, size - 8), new Point(size - 8, size - 8), new Point(size - 8, size - 1), new Point(8, size - 1)];
                gr.FillPolygon(RiverBrush, Terrain2C);
            }
            using (Graphics gr = Graphics.FromImage(River3A))
            {
                Point[] Terrain3A = [new Point(8, 1), new Point(size - 8, 1), new Point(size - 8, 8), new Point(size - 1, 8), new Point(size - 1, size - 8), new Point(size - 8, size - 8), new Point(size - 8, size - 1), new Point(8, size - 1)];
                gr.FillPolygon(RiverBrush, Terrain3A);
            }
            using (Graphics gr = Graphics.FromImage(River3B))
            {
                Point[] Terrain3B = [new Point(size - 1, 8), new Point(size - 1, size - 1), new Point(8, size - 1)];
                gr.FillPolygon(RiverBrush, Terrain3B);
            }
            using (Graphics gr = Graphics.FromImage(River3C))
            {
                gr.FillRectangle(RiverBrush, 8, 8, size - 9, size - 9);
            }
            using (Graphics gr = Graphics.FromImage(River4A))
            {
                Point[] Terrain4A = [new Point(8, 1), new Point(size - 8, 1), new Point(size - 8, 8), new Point(size - 1, 8), new Point(size - 1, size - 1), new Point(8, size - 1)];
                gr.FillPolygon(RiverBrush, Terrain4A);
            }
            using (Graphics gr = Graphics.FromImage(River4B))
            {
                Point[] Terrain4B = [new Point(8, 1), new Point(size - 1, 1), new Point(size - 1, size - 8), new Point(size - 8, size - 8), new Point(size - 8, size - 1), new Point(8, size - 1)];
                gr.FillPolygon(RiverBrush, Terrain4B);
            }
            using (Graphics gr = Graphics.FromImage(River4C))
            {
                Point[] Terrain4C = [new Point(8, 1), new Point(size - 8, 1), new Point(size - 8, 8), new Point(size - 1, 8), new Point(size - 1, size - 8), new Point(size - 8, size - 8), new Point(size - 8, size - 1), new Point(8, size - 1), new Point(8, size - 8), new Point(1, size - 8), new Point(1, 8), new Point(8, 8)];
                gr.FillPolygon(RiverBrush, Terrain4C);
            }
            using (Graphics gr = Graphics.FromImage(River5A))
            {
                Point[] Terrain5A = [new Point(8, 1), new Point(size - 8, 1), new Point(size - 8, 8), new Point(size - 1, 8), new Point(size - 1, size - 8), new Point(size - 8, size - 8), new Point(size - 8, size - 1), new Point(1, size - 1), new Point(1, 8), new Point(8, 8)];
                gr.FillPolygon(RiverBrush, Terrain5A);
            }
            using (Graphics gr = Graphics.FromImage(River5B))
            {
                gr.FillRectangle(RiverBrush, 8, 1, size - 9, size - 2);
            }
            using (Graphics gr = Graphics.FromImage(River6A))
            {
                Point[] Terrain6A = [new Point(8, 1), new Point(size - 1, 1), new Point(size - 1, size - 8), new Point(size - 8, size - 8), new Point(size - 8, size - 1), new Point(1, size - 1), new Point(1, 8), new Point(8, 8)];
                gr.FillPolygon(RiverBrush, Terrain6A);
            }
            using (Graphics gr = Graphics.FromImage(River6B))
            {
                Point[] Terrain6B = [new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 8), new Point(size - 8, size - 8), new Point(size - 8, size - 1), new Point(8, size - 1), new Point(8, size - 8), new Point(1, size - 8)];
                gr.FillPolygon(RiverBrush, Terrain6B);
            }
            using (Graphics gr = Graphics.FromImage(River7A))
            {
                Point[] Terrain7A = [new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 1), new Point(8, size - 1), new Point(8, size - 8), new Point(1, size - 8)];
                gr.FillPolygon(RiverBrush, Terrain7A);
            }
            using (Graphics gr = Graphics.FromImage(River8A))
            {
                gr.FillRectangle(RiverBrush, 1, 1, size - 2, size - 2);
            }

            if (direction == 1)
            {
                River1A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River2A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River2B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River2C.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River3A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River3B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River3C.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River4A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River4B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River5A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River5B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River6A.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River6B.RotateFlip(RotateFlipType.Rotate270FlipNone);
                River7A.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            else if (direction == 2)
            {
                River1A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River2A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River2B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River2C.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River3A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River3B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River3C.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River4A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River4B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River5A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River5B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River6A.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River6B.RotateFlip(RotateFlipType.Rotate180FlipNone);
                River7A.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
            else if (direction == 3)
            {
                River1A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River2A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River2B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River2C.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River3A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River3B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River3C.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River4A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River4B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River5A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River5B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River6A.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River6B.RotateFlip(RotateFlipType.Rotate90FlipNone);
                River7A.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }

            RiverButton0A.BackgroundImage = River0A;
            RiverButton1A.BackgroundImage = River1A;
            RiverButton2A.BackgroundImage = River2A;
            RiverButton2B.BackgroundImage = River2B;
            RiverButton2C.BackgroundImage = River2C;
            RiverButton3A.BackgroundImage = River3A;
            RiverButton3B.BackgroundImage = River3B;
            RiverButton3C.BackgroundImage = River3C;
            RiverButton4A.BackgroundImage = River4A;
            RiverButton4B.BackgroundImage = River4B;
            RiverButton4C.BackgroundImage = River4C;
            RiverButton5A.BackgroundImage = River5A;
            RiverButton5B.BackgroundImage = River5B;
            RiverButton6A.BackgroundImage = River6A;
            RiverButton6B.BackgroundImage = River6B;
            RiverButton7A.BackgroundImage = River7A;
            RiverButton8A.BackgroundImage = River8A;

            if (manualSelectedRiverModel != null)
                manualSelectedRiverModel.BackColor = Color.Violet;
        }
        private void RotateRoadButton_Click(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            manualSelectedRoadDirection += 1;
            if (manualSelectedRoadDirection > 3)
                manualSelectedRoadDirection = 0;

            DrawRoadImages(manualSelectedRoadDirection, manualSelectedRoad, currentSize);
        }

        private void RotateCliffButton_Click(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            manualSelectedCliffDirection += 1;
            if (manualSelectedCliffDirection > 3)
                manualSelectedCliffDirection = 0;

            DrawCliffImages(manualSelectedCliffDirection, manualSelectedCliffElevation, currentSize);
        }

        private void RotateRiverButton_Click(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            manualSelectedRiverDirection += 1;
            if (manualSelectedRiverDirection > 3)
                manualSelectedRiverDirection = 0;

            DrawRiverImages(manualSelectedRiverDirection, manualSelectedRiverElevation, currentSize);
        }

        private void RoadDropdownBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ManualRoadModeButton_Click(sender, null);

            UpdateSelectedRoad();
        }

        private void UpdateSelectedRoad()
        {
            manualSelectedRoad = (ushort)RoadDropdownBox.SelectedIndex;

            int size = 24;
            if (isBigControl)
                size = 48;
            Bitmap MiniRoadImg = manualSelectedRoad switch
            {
                0 => new Bitmap(Properties.Resources.wood, new Size(size, size)),
                1 => new Bitmap(Properties.Resources.tile, new Size(size, size)),
                2 => new Bitmap(Properties.Resources.sand, new Size(size, size)),
                3 => new Bitmap(Properties.Resources.pattern, new Size(size, size)),
                4 => new Bitmap(Properties.Resources.darksoil, new Size(size, size)),
                5 => new Bitmap(Properties.Resources.brick, new Size(size, size)),
                6 => new Bitmap(Properties.Resources.stone, new Size(size, size)),
                7 => new Bitmap(Properties.Resources.dirt, new Size(size, size)),
                _ => new Bitmap(size, size),
            };
            ManualRoadModeButton.Image = MiniRoadImg;

            DrawRoadImages(manualSelectedRoadDirection, manualSelectedRoad, currentSize);
        }

        private void ManualRoadButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            manualSelectedRoadModel = button;

            LastChangedX = -1;
            LastChangedY = -1;

            ResetRoadButton();
        }

        private void ResetRoadButton()
        {
            foreach (Button btn in ManualRoadPanel.Controls.OfType<Button>())
            {
                if (btn == RotateRoadButton)
                    continue;
                if (btn == manualSelectedRoadModel)
                    btn.BackColor = Color.Violet;
                else
                    btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation0];
            }
        }

        private void ManualCliffButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            manualSelectedCliffModel = button;

            LastChangedX = -1;
            LastChangedY = -1;

            ResetCliffButton();
        }

        private void ResetCliffButton()
        {
            foreach (Button btn in ManualCliffPanel.Controls.OfType<Button>())
            {
                if (btn == RotateCliffButton)
                    continue;
                if (btn == manualSelectedCliffModel)
                    btn.BackColor = Color.Violet;
                else
                {
                    if (manualSelectedCliffElevation == 1)
                    {
                        btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation0];
                    }
                    else if (manualSelectedCliffElevation == 2)
                    {
                        btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation1];
                    }
                    else if (manualSelectedCliffElevation >= 3)
                    {
                        btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation2];
                    }
                    else
                    {
                        btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation0];
                    }
                }
            }
        }

        private void ManualRiverButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            manualSelectedRiverModel = button;

            LastChangedX = -1;
            LastChangedY = -1;

            ResetRiverButton();
        }

        private void ResetRiverButton()
        {
            foreach (Button btn in ManualRiverPanel.Controls.OfType<Button>())
            {
                if (btn == RotateRiverButton)
                    continue;
                if (btn == manualSelectedRiverModel)
                    btn.BackColor = Color.Violet;
                else
                {
                    if (manualSelectedRiverElevation == 0)
                    {
                        btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation0];
                    }
                    else if (manualSelectedRiverElevation == 1)
                    {
                        btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation1];
                    }
                    else if (manualSelectedRiverElevation == 2)
                    {
                        btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation2];
                    }
                    else if (manualSelectedRiverElevation >= 3)
                    {
                        btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation3];
                    }
                    else
                    {
                        btn.BackColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation0];
                    }
                }
            }
        }

        private void ManualCliffElevationBar_ValueChanged(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            manualSelectedCliffElevation = (ushort)ManualCliffElevationBar.Value;

            DrawCliffImages(manualSelectedCliffDirection, manualSelectedCliffElevation, currentSize);
        }
        private void ManualRiverElevationBar_ValueChanged(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            manualSelectedRiverElevation = (ushort)ManualRiverElevationBar.Value;

            DrawRiverImages(manualSelectedRiverDirection, manualSelectedRiverElevation, currentSize);
        }
        private void ManualCliffModeButton_Click(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            ManualCliffModeButton.BackColor = Color.LightSeaGreen;
            ManualRiverModeButton.BackColor = Color.FromArgb(114, 137, 218);
            ManualRoadModeButton.BackColor = Color.FromArgb(114, 137, 218);

            ManualCliffPanel.Visible = true;
            ManualRiverPanel.Visible = false;
            ManualRoadPanel.Visible = false;

            selectedManualMode = ManualCliffPanel;

            manualSelectedRoadModel = null;
            ResetRoadButton();
            manualSelectedRiverModel = null;
            ResetRiverButton();
        }
        private void ManualRiverModeButton_Click(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            ManualCliffModeButton.BackColor = Color.FromArgb(114, 137, 218);
            ManualRiverModeButton.BackColor = Color.LightSeaGreen;
            ManualRoadModeButton.BackColor = Color.FromArgb(114, 137, 218);

            ManualCliffPanel.Visible = false;
            ManualRiverPanel.Visible = true;
            ManualRoadPanel.Visible = false;

            selectedManualMode = ManualRiverPanel;

            manualSelectedRoadModel = null;
            ResetRoadButton();
            manualSelectedCliffModel = null;
            ResetCliffButton();
        }
        private void ManualRoadModeButton_Click(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            ManualCliffModeButton.BackColor = Color.FromArgb(114, 137, 218);
            ManualRiverModeButton.BackColor = Color.FromArgb(114, 137, 218);
            ManualRoadModeButton.BackColor = Color.LightSeaGreen;

            ManualCliffPanel.Visible = false;
            ManualRiverPanel.Visible = false;
            ManualRoadPanel.Visible = true;

            selectedManualMode = ManualRoadPanel;

            manualSelectedRiverModel = null;
            ResetRiverButton();
            manualSelectedCliffModel = null;
            ResetCliffButton();
        }

        private static string FindModel(Button SelectedButton)
        {
            if (SelectedButton == null)
                return "0A";

            return SelectedButton.Tag.ToString();
        }

        private void FixRiverMouthToggle_CheckedChanged(object sender, EventArgs e)
        {
            _ = DrawMainMapAsync(anchorX, anchorY, false);
        }

        private bool[,] BuildMouthMarker()
        {
            bool[,] mouthMaker = new bool[16, 16];

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Color c = MiniMap.GetBackgroundColorLess(anchorX + i, anchorY + j);
                    if (c == MiniMap.Pixel[0x0])
                    {
                        if (MiniMap.GetBackgroundColorLess(anchorX + i, anchorY + j - 1) == MiniMap.Pixel[0xC])
                        {
                            if (j > 0)
                                mouthMaker[i, j - 1] = true;
                        }
                        if (MiniMap.GetBackgroundColorLess(anchorX + i, anchorY + j + 1) == MiniMap.Pixel[0xC])
                        {
                            if (j < 14)
                                mouthMaker[i, j + 1] = true;
                        }
                        if (MiniMap.GetBackgroundColorLess(anchorX + i - 1, anchorY + j) == MiniMap.Pixel[0xC])
                        {
                            if (i > 0)
                                mouthMaker[i - 1, j] = true;
                        }
                        if (MiniMap.GetBackgroundColorLess(anchorX + i + 1, anchorY + j) == MiniMap.Pixel[0xC])
                        {
                            if (i < 14)
                                mouthMaker[i + 1, j] = true;
                        }
                    }
                }
            }

            return mouthMaker;
        }

        private bool IsRiverMouth(int x, int y)
        {
            bool[,] mouthMaker = BuildMouthMarker();
            return mouthMaker[x - anchorX, y - anchorY];
        }

        private void DisplayCustomDesignToggle_CheckedChanged(object sender, EventArgs e)
        {
            displayCustomDesign = DisplayCustomDesignToggle.Checked;
            if (mapDrawReady)
                DrawMainMap(anchorX, anchorY);
        }

        private void CustomDesignList_Click(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;
        }
    }
}
