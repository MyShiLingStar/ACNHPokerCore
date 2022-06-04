using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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

        private const int numOfColumn = 0x70;
        private const int numOfRow = 0x60;
        private const int columnSize = 0xC00;
        private const int TerrainSize = 0xE;

        private static byte[] AcreData = Properties.Resources.acre;
        public const int AcreWidth = 7 + (2 * 1);
        private const int AcreHeight = 6 + (2 * 1);
        private const int AcreMax = AcreWidth * AcreHeight;
        private static Color[][] floorBackgroundColor; 

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
        private bool cornerMode = false;
        private bool displayBuilding = false;
        private bool displayRoad = true;
        private bool firstEdit = true;
        private bool terrainSaving = false;

        private ushort selectedRoad = 99;
        private Button selectedButton = null;

        private Bitmap CurrentMainMap;
        private Bitmap CurrentMiniMap;


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

            showMapWait(42);

            Thread LoadThread = new Thread(delegate () { fetchMap(); });
            LoadThread.Start();
        }

        private void RoadRoller_Resize(object sender, EventArgs e)
        {
            if (terrainUnits != null && WindowState != FormWindowState.Minimized && mapDrawReady)
            {
                setSubDivider();
                DrawMainMap(anchorX, anchorY);
            }
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
                Layer1 = Utilities.getMapLayer(socket, usb, Utilities.mapZero, ref counter);
                Acre = Utilities.getAcre(socket, usb);
                Building = Utilities.getBuilding(socket, usb);
                Terrain = Utilities.getTerrain(socket, usb);

                if (Acre != null)
                {
                    if (MiniMap == null)
                        MiniMap = new miniMap(Layer1, Acre, Building, Terrain, 2);
                }
                else
                    throw new NullReferenceException("Layer1/Layer2/Acre");

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
                hideMapWait();
            }
            catch (Exception ex)
            {
                MyLog.logEvent("RoadRoller", "FetchMap: " + ex.Message.ToString());
                MyMessageBox.Show(ex.Message.ToString(), "Oof");
            }
        }

        private void DrawMainMap(int x, int y, bool force = false)
        {
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

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                var cm = new ColorMatrix();
                cm.Matrix33 = 1;

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                int ImageNum = 0;

                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        Bitmap tile = new Bitmap(terrainUnits[x + i][y + j].getImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner));
                        graphics.DrawImage(tile, new Rectangle(i * GridSize, j * GridSize, GridSize, GridSize), 0, 0, GridSize, GridSize, GraphicsUnit.Pixel, ia);
                        ImageNum++;
                    }
                }
            }

            MainMap.Size = new Size(GridSize * 16, GridSize * 16);
            MainMap.Location = new Point((size - (GridSize * 16)) / 2 - 2, (size - (GridSize * 16)) / 2 - 2);
            MainMap.BackgroundImage = myBitmap;
            CurrentMainMap = myBitmap;

            mapDrawing = false;
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

                        int ImageNum = 0;

                        for (int i = 0; i < 16; i++)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                Bitmap tile = new Bitmap(terrainUnits[x + i][y + j].getImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner));
                                graphics.DrawImage(tile, new Rectangle(i * GridSize, j * GridSize, GridSize, GridSize), 0, 0, GridSize, GridSize, GraphicsUnit.Pixel, ia);
                                ImageNum++;
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
                                Bitmap tile = new Bitmap(terrainUnits[x + i][y + j].getImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner));
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
                                    Bitmap tile = new Bitmap(terrainUnits[x + i][y + j].getImage(GridSize, x + i, y + j, displayRoad, displayBuilding, highlightRoadCorner, highlightCliffCorner, highlightRiverCorner));
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

        private void PlaceRoad(int x, int y)
        {
            ushort road = selectedRoad;
            ushort elevation = terrainUnits[x][y].getElevation();

            bool[,] neighbour = FindSameNeighbourRoad(road, elevation, x, y);
            terrainUnits[x][y].updateRoad(road, neighbour);
            fixNeighbourRoad(x, y, road, neighbour);
            _ = UpdateMainMapAsync(x, y);
            AddMiniMapPixel(x, y, miniMap.TerrainColor[road]);
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
                c = miniMap.TerrainColor[terrainNum];
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
                    AddMiniMapPixel(x, y, miniMap.TerrainColor[10]);
                }
            }
            else if (!CurrentUnit.isFall()) // Place River
            {
                ushort elevation = CurrentUnit.getElevation();
                bool[,] neighbour = FindSameNeighbourRiver(elevation, x, y);
                CurrentUnit.updateRiver(neighbour, elevation);

                bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y);
                fixNeighbourTerrain(x, y, TerrainNeighbour);
                _ = UpdateMainMapAsync(x, y);
                AddMiniMapPixel(x, y, miniMap.TerrainColor[12]);
            }
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
            ushort road = 99;
            TerrainUnit CurrentUnit = terrainUnits[x][y];
            ushort elevation = CurrentUnit.getElevation();

            bool[,] neighbour = FindSameNeighbourRoad(road, elevation, x, y);
            CurrentUnit.updateRoad(road, neighbour);
            fixNeighbourRoad(x, y, road, neighbour);

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
            bool[,] TerrainNeighbour = FindTerrainNeighbourForFix(x, y);
            fixNeighbourTerrain(x, y, TerrainNeighbour);

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

        /*
        private void RemoveMiniMapPixel(int x, int y)
        {
            Bitmap myBitmap;

            myBitmap = new Bitmap(CurrentMiniMap);

            Color c = miniMap.GetBackgroundColorLess(x, y);

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
        }*/

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
                c = miniMap.TerrainColor[terrainNum];
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

        private void fixNeighbourTerrain(int x, int y, bool[,] CurrentNeighbour)
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
                                bool[,] NeighbourNeighbour = FindSameNeighbourRiver(NeighbourElevation, x + i, y + j);
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

        private bool[,] FindTerrainNeighbourForFix(int x, int y)
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
                        TerrainNeighbour[i + 1, j + 1] = terrainUnits[x + i][y + j].HasTerrain();
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

        private bool[,] FindSameNeighbourRiver(ushort elevation, int x, int y)
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
                        sameNeighbour[i + 1, j + 1] = terrainUnits[x + i][y + j].isSameOrHigherElevationRiverOrFall(elevation);
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

            foreach (Button btn in ButtonPanel.Controls.OfType<Button>())
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

            if (selectedButton == null)
                return;

            if (firstEdit)
            {
                firstEdit = false;
                ConfirmBtn.Visible = true;
            }

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
                    else
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

            if (selectedButton == null)
                return;

            TerrainUnit CurrentUnit = terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY];

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
                    else
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


            /*

            if (e.Button == MouseButtons.Left)
            {
                if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY)
                    return;

                if (selectedButton == CornerBtn)
                {
                    if (terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].canChangeCornerRoad())
                    {
                        ChangeRoadCorner(Xcoordinate + anchorX, Ycoordinate + anchorY);
                    }
                    else
                        return;
                }
                else if (selectedButton == CliffBtn)
                {
                    if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                        return;
                    PlaceCliff(Xcoordinate + anchorX, Ycoordinate + anchorY);
                }
                else if (selectedButton == RiverBtn)
                {
                    if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                        return;
                    PlaceRiverOrFall(Xcoordinate + anchorX, Ycoordinate + anchorY);
                }
                else
                {
                    if (Xcoordinate == LastChangedX && Ycoordinate == LastChangedY && wasPlacing)
                        return;

                    if (terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].isRoundCornerTerrain())
                        return;
                    else if (terrainUnits[Xcoordinate + anchorX][Ycoordinate + anchorY].isFallOrRiver())
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

                DeleteRoad(Xcoordinate + anchorX, Ycoordinate + anchorY);

                wasPlacing = false;
                wasRemoving = true;

                LastChangedX = Xcoordinate;
                LastChangedY = Ycoordinate;

                UpdateToolTip(Xcoordinate, Ycoordinate);
            }

            */
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

            byte[] newTerrain = new byte[numOfRow * numOfColumn * TerrainSize];

            for (int i = 0; i < numOfColumn; i++)
            {
                for (int j = 0; j < numOfRow; j++)
                {
                    byte[] TileData = terrainUnits[i][j].getTerrainData();
                    Buffer.BlockCopy(TileData, 0, newTerrain, (i * numOfRow * TerrainSize) + (j * TerrainSize), TerrainSize);
                }
            }

            showMapWait(40);

            Thread SendTerrainThread = new Thread(delegate () { SendTerrain(newTerrain); });
            SendTerrainThread.Start();
        }

        private void SendTerrain(byte[] newTerrain)
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

            this.Invoke((MethodInvoker)delegate
            {
                MiniMap.updateTerrain(newTerrain);
                CurrentMiniMap = MiniMap.drawBackground();
                miniMapBox.BackgroundImage = CurrentMiniMap;
                terrainSaving = false;
                firstEdit = true;
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
    }
}
