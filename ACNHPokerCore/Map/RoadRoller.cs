using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Socket socket;
        private USBBot usb;
        private bool sound;

        private byte[] Layer1 = null;
        private byte[] Acre = null;
        private byte[] Building = null;
        private byte[] Terrain = null;
        private byte[] MapCustomDesgin = null;
        private byte[] MyDesign = null;

        private DesignPattern[] designPatterns;
        private int MyDesignIconSize = 36;

        private const int numOfColumn = 0x70;
        private const int numOfRow = 0x60;
        private const int columnSize = 0xC00;
        private const int TerrainSize = 0xE;

        public const int AcreWidth = 7 + (2 * 1);
        private const int AcreHeight = 6 + (2 * 1);
        private const int AcreMax = AcreWidth * AcreHeight;

        private int counter = 0;
        private int GridSize = 0;
        private int LastX = -1;
        private int LastY = -1;
        private int LastChangedX = -1;
        private int LastChangedY = -1;
        private bool wasPlacing = false;
        private bool wasRemoving = false;

        private TerrainUnit[][] terrainUnits = null;
        private bool mapDrawReady = false;
        private miniMap MiniMap = null;
        private int anchorX = 0;
        private int anchorY = 0;
        private bool mapDrawing = false;
        private bool highlightRoadCorner = false;
        private bool highlightCliffCorner = false;
        private bool highlightRiverCorner = false;
        private bool displayCustomDesign = false;
        private bool cornerMode = false;
        private bool displayBuilding = false;
        private bool displayRoad = true;
        private bool firstEdit = true;
        private bool terrainSaving = false;
        private bool haveCustomEdit = false;

        private ushort selectedRoad = 99;
        private Button selectedButton = null;

        private Bitmap CurrentMainMap;
        private Bitmap CurrentMiniMap;

        private bool manualModeActivated = false;
        private bool customModeActivated = false;
        private Panel selectedManualMode;
        private ushort manualSelectedRoad = 7;
        private ushort manualSelectedRoadDirection = 0;
        private Button manualSelectedRoadModel = null;

        private ushort manualSelectedCliffDirection = 0;
        private ushort manualSelectedCliffElevation = 1;
        private Button manualSelectedCliffModel = null;

        private ushort manualSelectedRiverDirection = 0;
        private ushort manualSelectedRiverElevation = 0;
        private Button manualSelectedRiverModel = null;

        private bool isBigControl = false;
        private Dictionary<string, Size> ControlSize = new Dictionary<string, Size>();
        private Dictionary<string, Point> ControlLocation = new Dictionary<string, Point>();

        public event CloseHandler closeForm;

        private static object lockObject = new object();
        public RoadRoller(Socket S, USBBot USB, bool Sound)
        {
            socket = S;
            usb = USB;
            sound = Sound;
            InitializeComponent();
            setSubDivider();
            Bitmap CliffImg = new Bitmap(Properties.Resources.cliff, new Size(60, 60));
            CliffBtn.Image = CliffImg;
            Bitmap RiverImg = new Bitmap(Properties.Resources.river, new Size(60, 60));
            RiverBtn.Image = RiverImg;
            Bitmap CornerImg = new Bitmap(Properties.Resources.corner, new Size(50, 50));
            CornerBtn.Image = CornerImg;
            Bitmap WoodImg = new Bitmap(Properties.Resources.wood, new Size(50, 50));
            WoodBtn.Image = WoodImg;
            Bitmap TileImg = new Bitmap(Properties.Resources.tile, new Size(50, 50));
            TileBtn.Image = TileImg;
            Bitmap SandImg = new Bitmap(Properties.Resources.sand, new Size(50, 50));
            SandBtn.Image = SandImg;
            Bitmap PatternImg = new Bitmap(Properties.Resources.pattern, new Size(50, 50));
            ArchBtn.Image = PatternImg;
            Bitmap DarkDirtImg = new Bitmap(Properties.Resources.darksoil, new Size(50, 50));
            DarkDirtBtn.Image = DarkDirtImg;
            Bitmap BrickImg = new Bitmap(Properties.Resources.brick, new Size(50, 50));
            BrickBtn.Image = BrickImg;
            Bitmap StoneImg = new Bitmap(Properties.Resources.stone, new Size(50, 50));
            StoneBtn.Image = StoneImg;
            Bitmap DirtImg = new Bitmap(Properties.Resources.dirt, new Size(50, 50));
            DirtBtn.Image = DirtImg;

            Bitmap MiniCliffImg = new Bitmap(Properties.Resources.cliff, new Size(36, 36));
            ManualCliffModeButton.BackgroundImage = MiniCliffImg;
            Bitmap MiniRiverImg = new Bitmap(Properties.Resources.river, new Size(36, 36));
            ManualRiverModeButton.BackgroundImage = MiniRiverImg;
            Bitmap RotateImg = new Bitmap(Properties.Resources.rotate, new Size(36, 36));
            RotateRoadButton.BackgroundImage = RotateImg;
            RotateCliffButton.BackgroundImage = RotateImg;
            RotateRiverButton.BackgroundImage = RotateImg;

            if (RoadDropdownBox.Items.Count > 0)
                RoadDropdownBox.SelectedIndex = RoadDropdownBox.Items.Count - 1;

            drawRoadImages();
            drawCliffImages();
            drawRiverImages();
            createDict();

            selectedManualMode = ManualRoadPanel;

            showMapWait(42);



            Thread LoadThread = new Thread(delegate () { fetchMap(); });
            LoadThread.Start();
        }

        private void createDict()
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
                setSubDivider();
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
            ManualButtonPanel.Width = 350;
            ManualButtonPanel.Height = 550;

            ManualCliffModeButton.Width = 60;
            ManualCliffModeButton.Height = 60;

            ManualRiverModeButton.Width = 60;
            ManualRiverModeButton.Height = 60;
            ManualRiverModeButton.Location = new Point(69, 3);

            ManualRoadModeButton.Width = 60;
            ManualRoadModeButton.Height = 60;
            ManualRoadModeButton.Location = new Point(135, 3);

            RoadDropdownBox.Width = 146;
            RoadDropdownBox.Location = new Point(201, 14);

            //--
            ManualRoadPanel.Size = new Size(350, 486);
            ManualRoadPanel.Location = new Point(0, 65);

            RoadButton0A.Size = new Size(60, 60);

            RoadButton0B.Size = new Size(60, 60);
            RoadButton0B.Location = new Point(69, 3);
            RoadButton1A.Size = new Size(60, 60);
            RoadButton1A.Location = new Point(3, 69);
            RoadButton1B.Size = new Size(60, 60);
            RoadButton1B.Location = new Point(69, 69);
            RoadButton1C.Size = new Size(60, 60);
            RoadButton1C.Location = new Point(135, 69);
            RoadButton2A.Size = new Size(60, 60);
            RoadButton2A.Location = new Point(3, 135);
            RoadButton2B.Size = new Size(60, 60);
            RoadButton2B.Location = new Point(69, 135);
            RoadButton2C.Size = new Size(60, 60);
            RoadButton2C.Location = new Point(135, 135);
            RoadButton3A.Size = new Size(60, 60);
            RoadButton3A.Location = new Point(3, 201);
            RoadButton3B.Size = new Size(60, 60);
            RoadButton3B.Location = new Point(69, 201);
            RoadButton3C.Size = new Size(60, 60);
            RoadButton3C.Location = new Point(135, 201);
            RoadButton4A.Size = new Size(60, 60);
            RoadButton4A.Location = new Point(3, 267);
            RoadButton4B.Size = new Size(60, 60);
            RoadButton4B.Location = new Point(69, 267);
            RoadButton4C.Size = new Size(60, 60);
            RoadButton4C.Location = new Point(135, 267);
            RoadButton5A.Size = new Size(60, 60);
            RoadButton5A.Location = new Point(3, 333);
            RoadButton5B.Size = new Size(60, 60);
            RoadButton5B.Location = new Point(69, 333);
            RoadButton6A.Size = new Size(60, 60);
            RoadButton6A.Location = new Point(135, 333);
            RoadButton6B.Size = new Size(60, 60);
            RoadButton6B.Location = new Point(201, 333);
            RoadButton7A.Size = new Size(60, 60);
            RoadButton7A.Location = new Point(3, 399);
            RoadButton8A.Size = new Size(60, 60);
            RoadButton8A.Location = new Point(69, 399);

            RotateRoadButton.Size = new Size(60, 60);
            RotateRoadButton.Location = new Point(267, 3);


            //--
            ManualCliffPanel.Size = new Size(350, 486);
            ManualCliffPanel.Location = new Point(0, 65);

            CliffButton0A.Size = new Size(60, 60);

            CliffButton1A.Size = new Size(60, 60);
            CliffButton1A.Location = new Point(3, 69);
            CliffButton2A.Size = new Size(60, 60);
            CliffButton2A.Location = new Point(3, 135);
            CliffButton2B.Size = new Size(60, 60);
            CliffButton2B.Location = new Point(69, 135);
            CliffButton2C.Size = new Size(60, 60);
            CliffButton2C.Location = new Point(135, 135);
            CliffButton3A.Size = new Size(60, 60);
            CliffButton3A.Location = new Point(3, 201);
            CliffButton3B.Size = new Size(60, 60);
            CliffButton3B.Location = new Point(69, 201);
            CliffButton3C.Size = new Size(60, 60);
            CliffButton3C.Location = new Point(135, 201);
            CliffButton4A.Size = new Size(60, 60);
            CliffButton4A.Location = new Point(3, 267);
            CliffButton4B.Size = new Size(60, 60);
            CliffButton4B.Location = new Point(69, 267);
            CliffButton4C.Size = new Size(60, 60);
            CliffButton4C.Location = new Point(135, 267);
            CliffButton5A.Size = new Size(60, 60);
            CliffButton5A.Location = new Point(3, 333);
            CliffButton5B.Size = new Size(60, 60);
            CliffButton5B.Location = new Point(69, 333);
            CliffButton6A.Size = new Size(60, 60);
            CliffButton6A.Location = new Point(135, 333);
            CliffButton6B.Size = new Size(60, 60);
            CliffButton6B.Location = new Point(201, 333);
            CliffButton7A.Size = new Size(60, 60);
            CliffButton7A.Location = new Point(3, 399);
            CliffButton8.Size = new Size(60, 60);
            CliffButton8.Location = new Point(69, 399);
            RotateCliffButton.Size = new Size(60, 60);
            RotateCliffButton.Location = new Point(267, 3);

            ManualTerrainElevationLabel.Location = new Point(255, 85);
            ManualCliffElevationBar.Location = new Point(287, 94);
            ManualTerrainElevation3Label.Location = new Point(314, 100);
            ManualTerrainElevation2Label.Location = new Point(314, 124);
            ManualTerrainElevation1Label.Location = new Point(314, 148);

            //--
            ManualRiverPanel.Size = new Size(350, 486);
            ManualRiverPanel.Location = new Point(0, 65);

            RiverButton0A.Size = new Size(60, 60);

            RiverButton1A.Size = new Size(60, 60);
            RiverButton1A.Location = new Point(3, 69);
            RiverButton2A.Size = new Size(60, 60);
            RiverButton2A.Location = new Point(3, 135);
            RiverButton2B.Size = new Size(60, 60);
            RiverButton2B.Location = new Point(69, 135);
            RiverButton2C.Size = new Size(60, 60);
            RiverButton2C.Location = new Point(135, 135);
            RiverButton3A.Size = new Size(60, 60);
            RiverButton3A.Location = new Point(3, 201);
            RiverButton3B.Size = new Size(60, 60);
            RiverButton3B.Location = new Point(69, 201);
            RiverButton3C.Size = new Size(60, 60);
            RiverButton3C.Location = new Point(135, 201);
            RiverButton4A.Size = new Size(60, 60);
            RiverButton4A.Location = new Point(3, 267);
            RiverButton4B.Size = new Size(60, 60);
            RiverButton4B.Location = new Point(69, 267);
            RiverButton4C.Size = new Size(60, 60);
            RiverButton4C.Location = new Point(135, 267);
            RiverButton5A.Size = new Size(60, 60);
            RiverButton5A.Location = new Point(3, 333);
            RiverButton5B.Size = new Size(60, 60);
            RiverButton5B.Location = new Point(69, 333);
            RiverButton6A.Size = new Size(60, 60);
            RiverButton6A.Location = new Point(135, 333);
            RiverButton6B.Size = new Size(60, 60);
            RiverButton6B.Location = new Point(201, 333);
            RiverButton7A.Size = new Size(60, 60);
            RiverButton7A.Location = new Point(3, 399);
            RiverButton8A.Size = new Size(60, 60);
            RiverButton8A.Location = new Point(69, 399);
            RotateRiverButton.Size = new Size(60, 60);
            RotateRiverButton.Location = new Point(267, 3);

            ManualRiverElevationLabel.Location = new Point(255, 85);
            ManualRiverElevationBar.Location = new Point(287, 94);
            ManualRiverElevation3Label.Location = new Point(314, 100);
            ManualRiverElevation2Label.Location = new Point(314, 116);
            ManualRiverElevation1Label.Location = new Point(314, 132);
            ManualRiverElevation0Label.Location = new Point(314, 148);

            Bitmap MiniCliffImg = new Bitmap(Properties.Resources.cliff, new Size(60, 60));
            ManualCliffModeButton.BackgroundImage = MiniCliffImg;
            Bitmap MiniRiverImg = new Bitmap(Properties.Resources.river, new Size(60, 60));
            ManualRiverModeButton.BackgroundImage = MiniRiverImg;
            Bitmap RotateImg = new Bitmap(Properties.Resources.rotate, new Size(60, 60));
            RotateRoadButton.BackgroundImage = RotateImg;
            RotateCliffButton.BackgroundImage = RotateImg;
            RotateRiverButton.BackgroundImage = RotateImg;

            updateSelectedRoad();
        }

        private void TurnSmall()
        {
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

            Bitmap MiniCliffImg = new Bitmap(Properties.Resources.cliff, new Size(36, 36));
            ManualCliffModeButton.BackgroundImage = MiniCliffImg;
            Bitmap MiniRiverImg = new Bitmap(Properties.Resources.river, new Size(36, 36));
            ManualRiverModeButton.BackgroundImage = MiniRiverImg;
            Bitmap RotateImg = new Bitmap(Properties.Resources.rotate, new Size(36, 36));
            RotateRoadButton.BackgroundImage = RotateImg;
            RotateCliffButton.BackgroundImage = RotateImg;
            RotateRiverButton.BackgroundImage = RotateImg;

            updateSelectedRoad();
        }

        private void setSubDivider()
        {
            int w = SubDivider.Width;
            int h = SubDivider.Height;
            if (w - h > 0)
            {
                this.SubDivider.ColumnStyles[0].Width = (w - h) / 2;
                this.SubDivider.ColumnStyles[2].Width = (w - h) / 2;
                this.SubDivider.RowStyles[0].Height = 0;
                this.SubDivider.RowStyles[2].Height = 0;
            }
            else if (h - w > 0)
            {
                this.SubDivider.ColumnStyles[0].Width = 0;
                this.SubDivider.ColumnStyles[2].Width = 0;
                this.SubDivider.RowStyles[0].Height = (h - w) / 2;
                this.SubDivider.RowStyles[2].Height = (h - w) / 2;
            }
            else
            {
                this.SubDivider.ColumnStyles[0].Width = 0;
                this.SubDivider.ColumnStyles[2].Width = 0;
                this.SubDivider.RowStyles[0].Height = 0;
                this.SubDivider.RowStyles[2].Height = 0;
            }
        }

        private void fetchMap()
        {
            try
            {
                if (socket != null || usb != null)
                {
                    Layer1 = Utilities.getMapLayer(socket, usb, Utilities.mapZero, ref counter);
                    Acre = Utilities.getAcre(socket, usb);
                    Building = Utilities.getBuilding(socket, usb);
                    Terrain = Utilities.getTerrain(socket, usb);
                    MyDesign = Utilities.getMyDesign(socket, usb, ref counter);
                    MapCustomDesgin = Utilities.getCustomDesignMap(socket, usb, ref counter);

                    if (Acre != null)
                    {
                        if (MiniMap == null)
                            MiniMap = new miniMap(Layer1, Acre, Building, Terrain, 2);
                    }
                    else
                        throw new NullReferenceException("Layer1/Layer2/Acre");

                    if (MyDesign != null || MapCustomDesgin != null)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            var imageList = new ImageList();
                            imageList.ImageSize = new Size(MyDesignIconSize, MyDesignIconSize);
                            CustomDesignList.LargeImageList = imageList;
                            CustomDesignList.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;

                            CustomDesignList.TileSize = new Size(50, 50);
                            CustomDesignList.View = View.Tile;
                            CustomDesignList.OwnerDraw = true;
                            CustomDesignList.DrawItem += designList_DrawItem;

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
                                    terrainUnits[i][j].setCustomDesign(currentDesign);
                                }

                                iterator++;
                            }
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            CurrentMiniMap = MiniMap.drawBackground();
                            miniMapBox.BackgroundImage = CurrentMiniMap;
                            miniMapBox.Image = MiniMap.drawSelectSquare16(8, 8);
                            DrawMainMap(anchorX, anchorY);
                            this.MaximizeBox = true;
                        });
                    }
                    else
                        throw new NullReferenceException("Terrain");

                    mapDrawReady = true;

                }

                hideMapWait();
            }
            catch (Exception ex)
            {
                MyLog.logEvent("RoadRoller", "FetchMap: " + ex.Message.ToString());
                MyMessageBox.Show(ex.Message.ToString(), "Oof");
            }
        }

        private void designList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            using (Graphics g = e.Graphics)
            {
                if (e.Item.Selected)
                {
                    if ((e.State & ListViewItemStates.Selected) != 0)
                    {
                        SolidBrush Select = new SolidBrush(Color.Orange);
                        g.FillRectangle(Select, e.Bounds);
                        e.DrawFocusRectangle();
                    }
                }

                g.DrawImage(e.Item.ImageList.Images[e.ItemIndex], new Rectangle(e.Bounds.X + 4, e.Bounds.Y + 4, e.Bounds.Width - 8, e.Bounds.Height - 8));
            }
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
                var cm = new ColorMatrix();
                cm.Matrix33 = 1;

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                bool[,] mouthMarker = new bool[16, 16];
                if (highlightMouth)
                    mouthMarker = buildMouthMarker();

                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        Bitmap tile;

                        TerrainUnit CurrentUnit = terrainUnits[x + i][y + j];

                        if (displayCustomDesign && CurrentUnit.haveCustomDesign())
                        {
                            tile = drawTileWithCustomDesign(CurrentUnit, GridSize);
                        }
                        else
                        {
                            if (highlightMouth && mouthMarker[i, j])
                            {
                                tile = new Bitmap(CurrentUnit.getImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner, true));
                            }
                            else
                            {
                                tile = new Bitmap(CurrentUnit.getImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner));
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

        private Bitmap drawTileWithCustomDesign(TerrainUnit currentUnit, int size)
        {
            Color borderColor = Color.Orange;
            Bitmap Bottom;
            Bottom = new Bitmap(size, size);
            using (Graphics gr = Graphics.FromImage(Bottom))
            {
                gr.SmoothingMode = SmoothingMode.None;
                gr.Clear(borderColor);
            }

            Bitmap Top;
            int DesignID = BitConverter.ToInt16(currentUnit.getCustomDesign(), 0);
            Top = designPatterns[DesignID].GetBitmap(size - 2);

            Bitmap Final = Bottom;

            using (Graphics graphics = Graphics.FromImage(Final))
            {
                var cm = new ColorMatrix();
                cm.Matrix33 = 1;

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(Top, new Rectangle(1, 1, Top.Width, Top.Height), 0, 0, Top.Width, Top.Height, GraphicsUnit.Pixel, ia);
            }

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
                        var cm = new ColorMatrix();
                        cm.Matrix33 = 1;

                        var ia = new ImageAttributes();
                        ia.SetColorMatrix(cm);

                        for (int i = 0; i < 16; i++)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                Bitmap tile;

                                TerrainUnit CurrentUnit = terrainUnits[x + i][y + j];

                                if (displayCustomDesign && CurrentUnit.haveCustomDesign())
                                {
                                    tile = drawTileWithCustomDesign(CurrentUnit, GridSize);
                                }
                                else
                                {
                                    bool[,] mouthMarker = new bool[16, 16];
                                    if (highlightMouth)
                                        mouthMarker = buildMouthMarker();

                                    if (highlightMouth && mouthMarker[i, j])
                                    {
                                        tile = new Bitmap(CurrentUnit.getImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner, true));
                                    }
                                    else
                                    {
                                        tile = new Bitmap(CurrentUnit.getImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner));
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
                };
            });

            mapDrawing = false;
        }

        private void UpdateMainMap(int x, int y)
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

            Bitmap myBitmap = new Bitmap(CurrentMainMap);
            lock (lockObject)
            {
                using (Graphics graphics = Graphics.FromImage(myBitmap))
                {
                    var cm = new ColorMatrix();
                    cm.Matrix33 = 1;

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

                                if (displayCustomDesign && CurrentUnit.haveCustomDesign())
                                {
                                    tile = drawTileWithCustomDesign(CurrentUnit, GridSize);
                                }
                                else
                                {
                                    tile = new Bitmap(CurrentUnit.getImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner));
                                }

                                graphics.DrawImage(tile, new Rectangle((x + i - anchorX) * GridSize, (y + j - anchorY) * GridSize, GridSize, GridSize), 0, 0, GridSize, GridSize, GraphicsUnit.Pixel, ia);
                            }
                        }
                    }
                }

                MainMap.BackgroundImage = myBitmap;
                CurrentMainMap = myBitmap;
            };

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

            Bitmap myBitmap = new Bitmap(CurrentMainMap);

            bool highlightMouth = fixRiverMouthToggle.Checked;

            await Task.Run(() =>
            {
                lock (lockObject)
                {
                    using (Graphics graphics = Graphics.FromImage(myBitmap))
                    {
                        var cm = new ColorMatrix();
                        cm.Matrix33 = 1;

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

                                    if (displayCustomDesign && CurrentUnit.haveCustomDesign())
                                    {
                                        tile = drawTileWithCustomDesign(CurrentUnit, GridSize);
                                    }
                                    else if (highlightMouth && isRiverMouth(x + i, y + j))
                                    {
                                        tile = new Bitmap(CurrentUnit.getImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner, true));
                                    }
                                    else
                                    {
                                        tile = new Bitmap(CurrentUnit.getImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner));
                                    }
                                    graphics.DrawImage(tile, new Rectangle((x + i - anchorX) * GridSize, (y + j - anchorY) * GridSize, GridSize, GridSize), 0, 0, GridSize, GridSize, GraphicsUnit.Pixel, ia);
                                }
                            }
                        }
                    }

                    MainMap.BackgroundImage = myBitmap;
                    CurrentMainMap = myBitmap;
                };
            });

            mapDrawing = false;
        }

        private void showMapWait(int size)
        {
            PleaseWaitPanel.Visible = true;
            counter = 0;
            MapProgressBar.Maximum = size + 5;
            MapProgressBar.Value = counter;
            PleaseWaitPanel.Visible = true;
            ProgressTimer.Start();
        }

        private void hideMapWait()
        {
            this.Invoke((MethodInvoker)delegate
            {
                PleaseWaitPanel.Visible = false;
                ProgressTimer.Stop();
            });
        }

        private void PlaceDesign(byte[] value, int x, int y)
        {
            terrainUnits[x][y].setCustomDesign(value);
            _ = UpdateMainMapAsync(x, y);
        }

        private void RemoveDesign(int x, int y)
        {
            terrainUnits[x][y].removeCustomDesign();
            _ = UpdateMainMapAsync(x, y);
        }

        private void PlaceRoad(int x, int y)
        {
            ushort road = selectedRoad;
            ushort elevation = terrainUnits[x][y].getElevation();

            bool[,] neighbour = FindSameNeighbourRoad(road, elevation, x, y);
            terrainUnits[x][y].updateRoad(road, neighbour);
            fixNeighbourRoad(x, y, road, neighbour);
            _ = UpdateMainMapAsync(x, y);
            AddMiniMapPixel(x, y, TerrainUnit.TerrainColor[road]);
        }

        private void PlaceManualRoad(int x, int y)
        {
            ushort road = manualSelectedRoad;
            string type = findModel(manualSelectedRoadModel);

            terrainUnits[x][y].setRoad(road, type, manualSelectedRoadDirection);
            _ = UpdateMainMapAsync(x, y);
            AddMiniMapPixel(x, y, TerrainUnit.TerrainColor[road]);
        }

        private void ChangeRoadCorner(int x, int y)
        {
            terrainUnits[x][y].changeRoadCorner();
            _ = UpdateMainMapAsync(x, y);
        }

        private void ChangeRiverCorner(int x, int y)
        {
            terrainUnits[x][y].changeRiverCorner();
            _ = UpdateMainMapAsync(x, y);
        }

        private void ChangeCliffCorner(int x, int y)
        {
            terrainUnits[x][y].changeCliffCorner();
            _ = UpdateMainMapAsync(x, y);
        }

        private void PlaceCliff(int x, int y, ushort placeElevation)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];

            if (CurrentUnit.HasRoad()) // Remove Road
            {
                DeleteRoad(x, y, false);
            }

            if ((CurrentUnit.isFlat() || CurrentUnit.isCliff()) && CurrentUnit.getElevation() == placeElevation)
                return;

            bool[,] neighbour = FindSameNeighbourCliff(placeElevation, x, y);

            CleanUpRiverOrFall(x, y, neighbour, placeElevation, CurrentUnit.getElevation());

            if (placeElevation == 0)
            {
                DeleteCliff(x, y, false);
            }
            else
            {
                CurrentUnit.updateCliff(neighbour, placeElevation);
                bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y);
                fixNeighbourTerrain(x, y, TerrainNeighbour);
            }

            _ = UpdateMainMapAsync(x, y);

            Color c;
            if (placeElevation == 0)
                c = miniMap.GetBackgroundColorLess(x, y);
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
            string type = findModel(manualSelectedCliffModel);

            CurrentUnit.setCliff(type, manualSelectedCliffElevation, manualSelectedCliffDirection);

            _ = UpdateMainMapAsync(x, y);

            Color c;
            if (manualSelectedCliffElevation == 0)
                c = miniMap.GetBackgroundColorLess(x, y);
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

            if (CurrentUnit.isCliff()) // Place Fall
            {
                if (CurrentUnit.isFallCliff())
                {
                    ushort elevation = CurrentUnit.getElevation();
                    ushort CliffDirection = CurrentUnit.getTerrainAngle();
                    bool[,] ConnectNeighbour = FindSameNeighbourFall(x, y);
                    CurrentUnit.updateFall(ConnectNeighbour, CliffDirection, elevation);

                    bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y);
                    fixNeighbourTerrain(x, y, TerrainNeighbour);
                    _ = UpdateMainMapAsync(x, y);
                    AddMiniMapPixel(x, y, TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Fall]);
                }
            }
            else if (!CurrentUnit.isFall()) // Place River
            {
                ushort elevation = CurrentUnit.getElevation();

                bool highlightMouth = fixRiverMouthToggle.Checked;

                if (highlightMouth)
                {
                    bool[,] mouthMarker = buildMouthMarker();

                    if (mouthMarker[x - anchorX, y - anchorY])
                    {
                        bool[,] neighbour = FindSameNeighbourRiver(elevation, x, y, true);
                        CurrentUnit.updateRiver(neighbour, elevation);
                        bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y, true);
                        fixNeighbourTerrain(x, y, TerrainNeighbour, true);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    bool[,] neighbour = FindSameNeighbourRiver(elevation, x, y, true);
                    CurrentUnit.updateRiver(neighbour, elevation);
                    bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y, true);
                    fixNeighbourTerrain(x, y, TerrainNeighbour, true);
                }

                _ = UpdateMainMapAsync(x, y);
                AddMiniMapPixel(x, y, TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.River]);
            }
        }

        private void PlaceManualRiver(int x, int y)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            string type = findModel(manualSelectedRiverModel);

            CurrentUnit.setRiver(type, manualSelectedRiverElevation, manualSelectedRiverDirection);
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
                            if (CurrentNeighbour.getElevation() != PlacingElevation)
                            {
                                if (CurrentNeighbour.isFall())
                                    DeleteFall(x + i, y + j, false);
                                else if (CurrentNeighbour.isRiver())
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
                            if (CurrentNeighbour.getElevation() <= PlacingElevation)
                            {
                                if (CurrentNeighbour.isFall())
                                    DeleteFall(x + i, y + j, false);
                                else if (CurrentNeighbour.isRiver())
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
            ushort elevation = CurrentUnit.getElevation();
            ushort road = 99;

            bool[,] neighbour = FindSameNeighbourRoad(road, elevation, x, y);
            CurrentUnit.updateRoad(road, neighbour);
            fixNeighbourRoad(x, y, road, neighbour);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteManualRoad(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = CurrentUnit.getElevation();
            ushort road = 99;

            CurrentUnit.setRoad(road, "", 0);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteFall(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = CurrentUnit.getElevation();

            bool[,] neighbour = FindSameNeighbourCliff(elevation, x, y);
            CurrentUnit.updateCliff(neighbour, elevation);
            bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y);
            fixNeighbourTerrain(x, y, TerrainNeighbour);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteRiver(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = CurrentUnit.getElevation();
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

            CurrentUnit.updateCliff(neighbour, elevation);
            bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y, true);
            fixNeighbourTerrain(x, y, TerrainNeighbour, true);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteManualRiver(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = CurrentUnit.getElevation();

            CurrentUnit.setCliff("8", elevation, 0);

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
            CurrentUnit.updateCliff(neighbour, elevation);
            bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y);
            fixNeighbourTerrain(x, y, TerrainNeighbour);

            if (MainUpdateNeeded)
                _ = UpdateMainMapAsync(x, y);
            RemoveMiniMapPixel(x, y, elevation);
        }

        private void DeleteManualCliff(int x, int y, bool MainUpdateNeeded = true)
        {
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = 0;
            CurrentUnit.setCliff("8", elevation, 0);

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
                Bitmap Bmp = new Bitmap(2, 2);

                using (Graphics gfx = Graphics.FromImage(Bmp))
                using (SolidBrush brush = new SolidBrush(c))
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
                c = miniMap.GetBackgroundColorLess(x, y);
            else
            {
                int terrainNum = elevation + 19;
                c = TerrainUnit.TerrainColor[terrainNum];
            }

            using (Graphics g = Graphics.FromImage(myBitmap))
            {
                g.SmoothingMode = SmoothingMode.None;
                Bitmap Bmp = new Bitmap(2, 2);

                using (Graphics gfx = Graphics.FromImage(Bmp))
                using (SolidBrush brush = new SolidBrush(c))
                {
                    gfx.SmoothingMode = SmoothingMode.None;
                    gfx.FillRectangle(brush, 0, 0, 2, 2);
                }

                g.DrawImageUnscaled(Bmp, x * 2, y * 2);
            }

            miniMapBox.BackgroundImage = myBitmap;
            CurrentMiniMap = myBitmap;
        }

        private void fixNeighbourRoad(int x, int y, ushort CurrentRoad, bool[,] CurrentNeighbour)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (x + i >= 0 && x + i < numOfColumn && y + j >= 0 && y + j < numOfRow)
                    {
                        if (CurrentNeighbour[i + 1, j + 1])
                        {
                            ushort NeighbourRoad = terrainUnits[x + i][y + j].getRoadType();

                            bool[,] NeighbourNeighbour = FindSameNeighbourRoad(NeighbourRoad, terrainUnits[x + i][y + j].getElevation(), x + i, y + j);
                            terrainUnits[x + i][y + j].updateRoad(NeighbourRoad, NeighbourNeighbour, terrainUnits[x + i][y + j].isRoundCornerRoad());
                        }
                        else if (terrainUnits[x + i][y + j].HasRoad())
                        {
                            ushort NeighbourRoad = terrainUnits[x + i][y + j].getRoadType();

                            if (NeighbourRoad != CurrentRoad)
                            {
                                bool[,] NeighbourNeighbour = FindSameNeighbourRoad(NeighbourRoad, terrainUnits[x + i][y + j].getElevation(), x + i, y + j);
                                terrainUnits[x + i][y + j].updateRoad(NeighbourRoad, NeighbourNeighbour, terrainUnits[x + i][y + j].isRoundCornerRoad());
                            }
                        }
                    }
                }
            }
        }

        private void fixNeighbourTerrain(int x, int y, bool[,] CurrentNeighbour, bool mouth = false)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (x + i >= 0 && x + i < numOfColumn && y + j >= 0 && y + j < numOfRow)
                    {
                        if (CurrentNeighbour[i + 1, j + 1])
                        {
                            ushort NeighbourElevation = terrainUnits[x + i][y + j].getElevation();

                            if (terrainUnits[x + i][y + j].isRiver())
                            {
                                bool[,] NeighbourNeighbour = FindSameNeighbourRiver(NeighbourElevation, x + i, y + j, mouth);
                                terrainUnits[x + i][y + j].updateRiver(NeighbourNeighbour, NeighbourElevation, terrainUnits[x + i][y + j].isRoundCornerTerrain());
                            }
                            else if (terrainUnits[x + i][y + j].isFall())
                            {
                                bool[,] NeighbourNeighbour = FindSameNeighbourFall(x + i, y + j);
                                ushort CurrentDirecttion = terrainUnits[x + i][y + j].getTerrainAngle();
                                ushort CliffDirection = 0;
                                if (CurrentDirecttion == 0)
                                    CliffDirection = 1;
                                else if (CurrentDirecttion == 1)
                                    CliffDirection = 2;
                                else if (CurrentDirecttion == 2)
                                    CliffDirection = 3;
                                else if (CurrentDirecttion == 3)
                                    CliffDirection = 0;
                                terrainUnits[x + i][y + j].updateFall(NeighbourNeighbour, CliffDirection, NeighbourElevation);
                            }
                            else
                            {
                                bool[,] NeighbourNeighbour = FindSameNeighbourCliff(NeighbourElevation, x + i, y + j);
                                terrainUnits[x + i][y + j].updateCliff(NeighbourNeighbour, NeighbourElevation, terrainUnits[x + i][y + j].isRoundCornerTerrain());
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
                            else if (miniMap.GetBackgroundColorLess(x + i, y + j) == miniMap.Pixel[0x0C])
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
                        sameNeighbour[i + 1, j + 1] = terrainUnits[x + i][y + j].isSameRoadAndElevation(road, elevation);
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
                        sameNeighbour[i + 1, j + 1] = terrainUnits[x + i][y + j].isSameOrHigherElevationTerrain(elevation);
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
                            if (terrainUnits[x + i][y + j].isSameOrHigherElevationRiverOrFall(elevation))
                                sameNeighbour[i + 1, j + 1] = true;
                            else if (miniMap.GetBackgroundColorLess(x + i, y + j) == miniMap.Pixel[0x0C])
                                sameNeighbour[i + 1, j + 1] = true;
                            else
                                sameNeighbour[i + 1, j + 1] = false;
                        }
                        else
                            sameNeighbour[i + 1, j + 1] = terrainUnits[x + i][y + j].isSameOrHigherElevationRiverOrFall(elevation);
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
                        sameNeighbour[i + 1, j + 1] = terrainUnits[x + i][y + j].isFallOrRiver();
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

        private void miniMapBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (MiniMap == null)
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

                miniMapBox.Image = MiniMap.drawSelectSquare16(x, y);
                DrawMainMap(anchorX, anchorY);
                fixRiverMouthToggle.Checked = false;
            }
        }

        private void miniMapBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (MiniMap == null)
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

                miniMapBox.Image = MiniMap.drawSelectSquare16(x, y);
                _ = DrawMainMapAsync(anchorX, anchorY);
                fixRiverMouthToggle.Checked = false;
            }
        }

        private void MainMap_MouseUp(object sender, MouseEventArgs e)
        {
            _ = DrawMainMapAsync(anchorX, anchorY, true);
        }

        private void resetBtnColor(bool refresh)
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

            updateHighlight();

            if (refresh)
                _ = DrawMainMapAsync(anchorX, anchorY);
        }
        private void CliffBtn_Click(object sender, EventArgs e)
        {
            selectedButton = CliffBtn;

            resetBtnColor(true);
        }
        private void RiverBtn_Click(object sender, EventArgs e)
        {
            selectedButton = RiverBtn;

            resetBtnColor(true);
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

            updateHighlight();
        }
        private void DirtBtn_Click(object sender, EventArgs e)
        {
            selectedButton = DirtBtn;
            selectedRoad = 7;

            if (displayRoad)
                resetBtnColor(true);
            else
            {
                resetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void StoneBtn_Click(object sender, EventArgs e)
        {
            selectedButton = StoneBtn;
            selectedRoad = 6;

            if (displayRoad)
                resetBtnColor(true);
            else
            {
                resetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void BrickBtn_Click(object sender, EventArgs e)
        {
            selectedButton = BrickBtn;
            selectedRoad = 5;

            if (displayRoad)
                resetBtnColor(true);
            else
            {
                resetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void DarkDirtBtn_Click(object sender, EventArgs e)
        {
            selectedButton = DarkDirtBtn;
            selectedRoad = 4;

            if (displayRoad)
                resetBtnColor(true);
            else
            {
                resetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void ArchBtn_Click(object sender, EventArgs e)
        {
            selectedButton = ArchBtn;
            selectedRoad = 3;

            if (displayRoad)
                resetBtnColor(true);
            else
            {
                resetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void SandBtn_Click(object sender, EventArgs e)
        {
            selectedButton = SandBtn;
            selectedRoad = 2;

            if (displayRoad)
                resetBtnColor(true);
            else
            {
                resetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void TileBtn_Click(object sender, EventArgs e)
        {
            selectedButton = TileBtn;
            selectedRoad = 1;

            if (displayRoad)
                resetBtnColor(true);
            else
            {
                resetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }
        private void WoodBtn_Click(object sender, EventArgs e)
        {
            selectedButton = WoodBtn;
            selectedRoad = 0;

            if (displayRoad)
                resetBtnColor(true);
            else
            {
                resetBtnColor(false);
                DisplayRoadToggle.Checked = true;
            }
        }

        private void updateHighlight()
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
                    byte[] Half = new byte[] { CustomDesignBytes[0], CustomDesignBytes[1] };

                    PlaceDesign(Half, Xcoordinate + anchorX, Ycoordinate + anchorY);

                    if (firstEdit)
                    {
                        firstEdit = false;
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

                    if (firstEdit)
                    {
                        firstEdit = false;
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

                            if (CurrentUnit.isFallOrRiver())
                                return;

                            if (CurrentUnit.isRoundCornerTerrain())
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
                        if (CurrentUnit.isRiver())
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
                        if (CurrentUnit.canChangeCornerRoad())
                            ChangeRoadCorner(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.canChangeCornerCliff())
                            ChangeCliffCorner(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.canChangeCornerRiver())
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

                        if (CurrentUnit.isFallOrRiver())
                            return;

                        PlaceRiverOrFall(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }
                    else
                    {
                        if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                            return;

                        if (CurrentUnit.isFallOrRiver())
                            return;

                        if (CurrentUnit.isRoundCornerTerrain())
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
                        if (CurrentUnit.isFall())
                            DeleteFall(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.isRiver())
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
                    byte[] Half = new byte[] { CustomDesignBytes[0], CustomDesignBytes[1] };

                    PlaceDesign(Half, Xcoordinate + anchorX, Ycoordinate + anchorY);

                    if (firstEdit)
                    {
                        firstEdit = false;
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

                    if (firstEdit)
                    {
                        firstEdit = false;
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

                            if (CurrentUnit.isFallOrRiver())
                                return;

                            if (CurrentUnit.isRoundCornerTerrain())
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
                        if (CurrentUnit.isRiver())
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

                        if (CurrentUnit.canChangeCornerRoad())
                            ChangeRoadCorner(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.canChangeCornerCliff())
                            ChangeCliffCorner(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.canChangeCornerRiver())
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

                        if (CurrentUnit.isFallOrRiver())
                            return;

                        PlaceRiverOrFall(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }
                    else
                    {
                        if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                            return;

                        if (CurrentUnit.isFallOrRiver())
                            return;

                        if (CurrentUnit.isRoundCornerTerrain())
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
                        if (CurrentUnit.isFall())
                            DeleteFall(Xcoordinate + anchorX, Ycoordinate + anchorY);
                        else if (CurrentUnit.isRiver())
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
            Thread LoadThread = new Thread(delegate () { fetchMap(); });
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
            updateHighlight();
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

            SaveFileDialog file = new SaveFileDialog();

            byte[] CurrentTerrainData = Terrain;

            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("en-US");
            file.FileName = Directory.GetCurrentDirectory() + @"\save\" + localDate.ToString(culture).Replace(" ", "_").Replace(":", "-").Replace("/", "-").Replace("\\", "-").Replace("|", "-").Replace(".", "-") + ".nht";
            File.WriteAllBytes(file.FileName, CurrentTerrainData);

            byte[] newTerrain = new byte[numOfRow * numOfColumn * TerrainSize];

            for (int i = 0; i < numOfColumn; i++)
            {
                for (int j = 0; j < numOfRow; j++)
                {
                    byte[] TileData = terrainUnits[i][j].getTerrainData();
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
                        byte[] CustomDesignData = terrainUnits[i][j].getCustomDesign();
                        Buffer.BlockCopy(CustomDesignData, 0, newCustomMap, (i * numOfRow * 2) + (j * 2), 2);
                    }
                }
            }

            showMapWait(40);

            Thread SendTerrainThread = new Thread(delegate () { SendTerrain(newTerrain, newCustomMap); });
            SendTerrainThread.Start();
        }

        private void SendTerrain(byte[] newTerrain, byte[] newCustomMap)
        {
            int wait = 0;
            while (isAboutToSave(10))
            {
                if (wait > 15)
                    break;
                Thread.Sleep(2000);
                wait++;
            }

            Utilities.sendTerrain(socket, null, newTerrain, ref counter);

            if (newCustomMap != null)
            {
                Utilities.sendCustomMap(socket, null, newCustomMap, ref counter);
            }

            this.Invoke((MethodInvoker)delegate
            {
                MiniMap.updateTerrain(newTerrain);
                CurrentMiniMap = MiniMap.drawBackground();
                miniMapBox.BackgroundImage = CurrentMiniMap;
                terrainSaving = false;
                firstEdit = true;
                haveCustomEdit = false;
                hideMapWait();
            });

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private bool isAboutToSave(int second)
        {
            byte[] b = Utilities.getSaving(socket, null);

            if (b == null)
                return true;
            if (b[0] == 1)
                return true;
            else
            {
                byte[] currentFrame = new byte[4];
                byte[] lastFrame = new byte[4];
                Buffer.BlockCopy(b, 12, currentFrame, 0, 4);
                Buffer.BlockCopy(b, 16, lastFrame, 0, 4);

                int currentFrameStr = Convert.ToInt32("0x" + Utilities.flip(Utilities.ByteToHexString(currentFrame)), 16);
                int lastFrameStr = Convert.ToInt32("0x" + Utilities.flip(Utilities.ByteToHexString(lastFrame)), 16);
                int FrameRemain = ((0x1518 - (currentFrameStr - lastFrameStr)));

                if (FrameRemain < 30 * second) // Not enough
                    return true;
                else if (FrameRemain >= 30 * 300) // Have too too many for some reason?
                    return false;
                else if (FrameRemain >= 30 * 175) // Just finish save buffer
                    return true;
                else
                {
                    Debug.Print(((0x1518 - (currentFrameStr - lastFrameStr))).ToString());
                    return false;
                }
            }
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
            this.closeForm();
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
            resetBtnColor(false);
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
            resetBtnColor(false);
        }

        private void drawRoadImages(int direction = 0, int road = 7, int size = 36)
        {
            Color RoadColor;
            switch (road)
            {
                case 0:
                    RoadColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadWood];
                    break;
                case 1:
                    RoadColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadTile];
                    break;
                case 2:
                    RoadColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadSand];
                    break;
                case 3:
                    RoadColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadPattern];
                    break;
                case 4:
                    RoadColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadDarkSoil];
                    break;
                case 5:
                    RoadColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadBrick];
                    break;
                case 6:
                    RoadColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadStone];
                    break;
                default:
                    RoadColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadSoil];
                    break;
            }

            Bitmap Road0A = new Bitmap(size, size);
            Bitmap Road0B = new Bitmap(size, size);
            Bitmap Road1A = new Bitmap(size, size);
            Bitmap Road1B = new Bitmap(size, size);
            Bitmap Road1C = new Bitmap(size, size);
            Bitmap Road2A = new Bitmap(size, size);
            Bitmap Road2B = new Bitmap(size, size);
            Bitmap Road2C = new Bitmap(size, size);
            Bitmap Road3A = new Bitmap(size, size);
            Bitmap Road3B = new Bitmap(size, size);
            Bitmap Road3C = new Bitmap(size, size);
            Bitmap Road4A = new Bitmap(size, size);
            Bitmap Road4B = new Bitmap(size, size);
            Bitmap Road4C = new Bitmap(size, size);
            Bitmap Road5A = new Bitmap(size, size);
            Bitmap Road5B = new Bitmap(size, size);
            Bitmap Road6A = new Bitmap(size, size);
            Bitmap Road6B = new Bitmap(size, size);
            Bitmap Road7A = new Bitmap(size, size);
            Bitmap Road8A = new Bitmap(size, size);

            SolidBrush RoadBrush = new SolidBrush(RoadColor);

            using (Graphics gr = Graphics.FromImage(Road0A))
            {
                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 16);
            }
            using (Graphics gr = Graphics.FromImage(Road0B))
            {
                Rectangle pieRect = new Rectangle(8, 8, (size - 16) * 2, (size - 16) * 2);
                gr.FillPie(RoadBrush, pieRect, -90, -90);
            }
            using (Graphics gr = Graphics.FromImage(Road1A))
            {
                gr.FillRectangle(RoadBrush, 8, 8, size - 16, size - 9);
            }
            using (Graphics gr = Graphics.FromImage(Road1B))
            {
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                Rectangle pieRect = new Rectangle(8, 8, (size - 16) * 2, (size - 16) * 2);
                gr.FillPie(RoadBrush, pieRect, -90, -90);
            }
            using (Graphics gr = Graphics.FromImage(Road1C))
            {
                gr.FillRectangle(RoadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                Rectangle pieRect = new Rectangle(8 - (size - 16), 8, (size - 16) * 2, (size - 16) * 2);
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

                Rectangle pieRect = new Rectangle(8, 8, (size - 16) * 2, (size - 16) * 2);
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

                Rectangle pieRect = new Rectangle(8, 8, (size - 16) * 2, (size - 16) * 2);
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

        private void drawCliffImages(int direction = 0, int elevation = 1, int size = 36)
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

            Bitmap Cliff0A = new Bitmap(size, size);
            Bitmap Cliff1A = new Bitmap(size, size);
            Bitmap Cliff2A = new Bitmap(size, size);
            Bitmap Cliff2B = new Bitmap(size, size);
            Bitmap Cliff2C = new Bitmap(size, size);
            Bitmap Cliff3A = new Bitmap(size, size);
            Bitmap Cliff3B = new Bitmap(size, size);
            Bitmap Cliff3C = new Bitmap(size, size);
            Bitmap Cliff4A = new Bitmap(size, size);
            Bitmap Cliff4B = new Bitmap(size, size);
            Bitmap Cliff4C = new Bitmap(size, size);
            Bitmap Cliff5A = new Bitmap(size, size);
            Bitmap Cliff5B = new Bitmap(size, size);
            Bitmap Cliff6A = new Bitmap(size, size);
            Bitmap Cliff6B = new Bitmap(size, size);
            Bitmap Cliff7A = new Bitmap(size, size);
            Bitmap Cliff8 = new Bitmap(size, size);

            SolidBrush CliffBrush = new SolidBrush(CliffColor);

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
                Point[] Terrain2B = new Point[] { new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(CliffBrush, Terrain2B);
            }
            using (Graphics gr = Graphics.FromImage(Cliff2C))
            {
                Point[] Terrain2C = new Point[] { new Point(4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(CliffBrush, Terrain2C);
            }
            using (Graphics gr = Graphics.FromImage(Cliff3A))
            {
                Point[] Terrain3A = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(CliffBrush, Terrain3A);
            }
            using (Graphics gr = Graphics.FromImage(Cliff3B))
            {
                Point[] Terrain3B = new Point[] { new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(CliffBrush, Terrain3B);
            }
            using (Graphics gr = Graphics.FromImage(Cliff3C))
            {
                gr.FillRectangle(CliffBrush, 4, 4, size - 5, size - 5);
            }
            using (Graphics gr = Graphics.FromImage(Cliff4A))
            {
                Point[] Terrain4A = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(CliffBrush, Terrain4A);
            }
            using (Graphics gr = Graphics.FromImage(Cliff4B))
            {
                Point[] Terrain4B = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(CliffBrush, Terrain4B);
            }
            using (Graphics gr = Graphics.FromImage(Cliff4C))
            {
                Point[] Terrain4C = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4) };
                gr.FillPolygon(CliffBrush, Terrain4C);
            }
            using (Graphics gr = Graphics.FromImage(Cliff5A))
            {
                Point[] Terrain5A = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4) };
                gr.FillPolygon(CliffBrush, Terrain5A);
            }
            using (Graphics gr = Graphics.FromImage(Cliff5B))
            {
                gr.FillRectangle(CliffBrush, 4, 1, size - 5, size - 2);
            }
            using (Graphics gr = Graphics.FromImage(Cliff6A))
            {
                Point[] Terrain6A = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4) };
                gr.FillPolygon(CliffBrush, Terrain6A);
            }
            using (Graphics gr = Graphics.FromImage(Cliff6B))
            {
                Point[] Terrain6B = new Point[] { new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                gr.FillPolygon(CliffBrush, Terrain6B);
            }
            using (Graphics gr = Graphics.FromImage(Cliff7A))
            {
                Point[] Terrain7A = new Point[] { new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
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

        private void drawRiverImages(int direction = 0, int elevation = 0, int size = 36)
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

            Bitmap River0A = new Bitmap(size, size);
            Bitmap River1A = new Bitmap(size, size);
            Bitmap River2A = new Bitmap(size, size);
            Bitmap River2B = new Bitmap(size, size);
            Bitmap River2C = new Bitmap(size, size);
            Bitmap River3A = new Bitmap(size, size);
            Bitmap River3B = new Bitmap(size, size);
            Bitmap River3C = new Bitmap(size, size);
            Bitmap River4A = new Bitmap(size, size);
            Bitmap River4B = new Bitmap(size, size);
            Bitmap River4C = new Bitmap(size, size);
            Bitmap River5A = new Bitmap(size, size);
            Bitmap River5B = new Bitmap(size, size);
            Bitmap River6A = new Bitmap(size, size);
            Bitmap River6B = new Bitmap(size, size);
            Bitmap River7A = new Bitmap(size, size);
            Bitmap River8A = new Bitmap(size, size);

            SolidBrush RiverBrush = new SolidBrush(RiverColor);

            using (Graphics gr = Graphics.FromImage(River0A))
            {
                gr.FillRectangle(RiverBrush, 4, 4, size - 8, size - 8);
            }
            using (Graphics gr = Graphics.FromImage(River1A))
            {
                gr.FillRectangle(RiverBrush, 4, 4, size - 8, size - 5);
            }
            using (Graphics gr = Graphics.FromImage(River2A))
            {
                gr.FillRectangle(RiverBrush, 4, 1, size - 8, size - 2);
            }
            using (Graphics gr = Graphics.FromImage(River2B))
            {
                Point[] Terrain2B = new Point[] { new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(RiverBrush, Terrain2B);
            }
            using (Graphics gr = Graphics.FromImage(River2C))
            {
                Point[] Terrain2C = new Point[] { new Point(4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(RiverBrush, Terrain2C);
            }
            using (Graphics gr = Graphics.FromImage(River3A))
            {
                Point[] Terrain3A = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(RiverBrush, Terrain3A);
            }
            using (Graphics gr = Graphics.FromImage(River3B))
            {
                Point[] Terrain3B = new Point[] { new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(RiverBrush, Terrain3B);
            }
            using (Graphics gr = Graphics.FromImage(River3C))
            {
                gr.FillRectangle(RiverBrush, 4, 4, size - 5, size - 5);
            }
            using (Graphics gr = Graphics.FromImage(River4A))
            {
                Point[] Terrain4A = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(RiverBrush, Terrain4A);
            }
            using (Graphics gr = Graphics.FromImage(River4B))
            {
                Point[] Terrain4B = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                gr.FillPolygon(RiverBrush, Terrain4B);
            }
            using (Graphics gr = Graphics.FromImage(River4C))
            {
                Point[] Terrain4C = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4) };
                gr.FillPolygon(RiverBrush, Terrain4C);
            }
            using (Graphics gr = Graphics.FromImage(River5A))
            {
                Point[] Terrain5A = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4) };
                gr.FillPolygon(RiverBrush, Terrain5A);
            }
            using (Graphics gr = Graphics.FromImage(River5B))
            {
                gr.FillRectangle(RiverBrush, 4, 1, size - 5, size - 2);
            }
            using (Graphics gr = Graphics.FromImage(River6A))
            {
                Point[] Terrain6A = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4) };
                gr.FillPolygon(RiverBrush, Terrain6A);
            }
            using (Graphics gr = Graphics.FromImage(River6B))
            {
                Point[] Terrain6B = new Point[] { new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                gr.FillPolygon(RiverBrush, Terrain6B);
            }
            using (Graphics gr = Graphics.FromImage(River7A))
            {
                Point[] Terrain7A = new Point[] { new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
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

            drawRoadImages(manualSelectedRoadDirection, manualSelectedRoad);
        }

        private void RotateCliffButton_Click(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            manualSelectedCliffDirection += 1;
            if (manualSelectedCliffDirection > 3)
                manualSelectedCliffDirection = 0;

            drawCliffImages(manualSelectedCliffDirection, manualSelectedCliffElevation);
        }

        private void RotateRiverButton_Click(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            manualSelectedRiverDirection += 1;
            if (manualSelectedRiverDirection > 3)
                manualSelectedRiverDirection = 0;

            drawRiverImages(manualSelectedRiverDirection, manualSelectedRiverElevation);
        }

        private void RoadDropdownBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ManualRoadModeButton_Click(sender, null);

            updateSelectedRoad();
        }

        private void updateSelectedRoad()
        {
            manualSelectedRoad = (ushort)RoadDropdownBox.SelectedIndex;

            int size = 24;
            if (isBigControl)
                size = 48;


            Bitmap MiniRoadImg;
            switch (manualSelectedRoad)
            {
                case 0:
                    MiniRoadImg = new Bitmap(Properties.Resources.wood, new Size(size, size));
                    break;
                case 1:
                    MiniRoadImg = new Bitmap(Properties.Resources.tile, new Size(size, size));
                    break;
                case 2:
                    MiniRoadImg = new Bitmap(Properties.Resources.sand, new Size(size, size));
                    break;
                case 3:
                    MiniRoadImg = new Bitmap(Properties.Resources.pattern, new Size(size, size));
                    break;
                case 4:
                    MiniRoadImg = new Bitmap(Properties.Resources.darksoil, new Size(size, size));
                    break;
                case 5:
                    MiniRoadImg = new Bitmap(Properties.Resources.brick, new Size(size, size));
                    break;
                case 6:
                    MiniRoadImg = new Bitmap(Properties.Resources.stone, new Size(size, size));
                    break;
                case 7:
                    MiniRoadImg = new Bitmap(Properties.Resources.dirt, new Size(size, size));
                    break;
                default:
                    MiniRoadImg = new Bitmap(size, size);
                    break;
            }
            ManualRoadModeButton.Image = MiniRoadImg;

            drawRoadImages(manualSelectedRoadDirection, manualSelectedRoad);
        }

        private void ManualRoadButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            manualSelectedRoadModel = button;

            LastChangedX = -1;
            LastChangedY = -1;

            resetRoadButton();
        }

        private void resetRoadButton()
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

            resetCliffButton();
        }

        private void resetCliffButton()
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

            resetRiverButton();
        }

        private void resetRiverButton()
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

            drawCliffImages(manualSelectedCliffDirection, manualSelectedCliffElevation);
        }
        private void ManualRiverElevationBar_ValueChanged(object sender, EventArgs e)
        {
            LastChangedX = -1;
            LastChangedY = -1;

            manualSelectedRiverElevation = (ushort)ManualRiverElevationBar.Value;

            drawRiverImages(manualSelectedRiverDirection, manualSelectedRiverElevation);
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
            resetRoadButton();
            manualSelectedRiverModel = null;
            resetRiverButton();
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
            resetRoadButton();
            manualSelectedCliffModel = null;
            resetCliffButton();
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
            resetRiverButton();
            manualSelectedCliffModel = null;
            resetCliffButton();
        }

        private string findModel(Button SelectedButton)
        {
            if (SelectedButton == null)
                return "0A";

            return SelectedButton.Tag.ToString();
        }

        private void fixRiverMouthToggle_CheckedChanged(object sender, EventArgs e)
        {
            _ = DrawMainMapAsync(anchorX, anchorY, false);
        }

        private bool[,] buildMouthMarker()
        {
            bool[,] mouthMaker = new bool[16, 16];

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Color c = miniMap.GetBackgroundColorLess(anchorX + i, anchorY + j);
                    if (c == miniMap.Pixel[0x0])
                    {
                        if (miniMap.GetBackgroundColorLess(anchorX + i, anchorY + j - 1) == miniMap.Pixel[0xC])
                        {
                            if (j > 0)
                                mouthMaker[i, j - 1] = true;
                        }
                        if (miniMap.GetBackgroundColorLess(anchorX + i, anchorY + j + 1) == miniMap.Pixel[0xC])
                        {
                            if (j < 14)
                                mouthMaker[i, j + 1] = true;
                        }
                        if (miniMap.GetBackgroundColorLess(anchorX + i - 1, anchorY + j) == miniMap.Pixel[0xC])
                        {
                            if (i > 0)
                                mouthMaker[i - 1, j] = true;
                        }
                        if (miniMap.GetBackgroundColorLess(anchorX + i + 1, anchorY + j) == miniMap.Pixel[0xC])
                        {
                            if (i < 14)
                                mouthMaker[i + 1, j] = true;
                        }
                    }
                }
            }

            return mouthMaker;
        }

        private bool isRiverMouth(int x, int y)
        {
            bool[,] mouthMaker = buildMouthMarker();
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
