using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ACNHPokerCore
{
    public class MiniMap
    {
        private byte[][] ItemMapData;
        private int[][] tilesType;

        private readonly byte[] AcreMapByte;
        private readonly byte[] BuildingByte;
        private byte[][] buildingList;
        private byte[] TerrainByte;
        private byte[] CustomDesignByte;
        private TerrainUnit[][] terrainUnits;

        private const int numOfColumn = 0x70;
        private const int numOfRow = 0x60;
        private const int columnSize = 0xC00;

        private const int fullNumOfColumn = 0x90;
        private const int fullNumOfRow = 0x80;

        private static int mapSize = 2;
        private static int plazaX = -1;
        private static int plazaY = -1;
        private static int plazaTopX = -1;
        private static int plazaTopY = -1;


        private static readonly byte[] AcreData = Properties.Resources.acre;
        private static Color[][] floorBackgroundColor;
        private static Color[][] floorBackgroundColorLess;
        private static Color[][] floorBuildingColor;

        public const int MapTileCount16x16 = 16 * 16 * 7 * 6;
        public const int TerrainTileSize = 0xE;

        public const int AllTerrainSize = MapTileCount16x16 * TerrainTileSize;

        private const int BuildingSize = 0x14;
        private const int TerrainSize = 0xE;
        private const int NumOfBuilding = 46;

        public const int AcreWidth = 7 + (2 * 1);
        private const int AcreHeight = 6 + (2 * 1);
        private const int AcreMax = AcreWidth * AcreHeight;
        private const int AllAcreSize = AcreMax * 2;
        public MiniMap(byte[] ItemMapByte, byte[] acreMapByte, byte[] buildingByte, byte[] terrainByte, byte[] customDesignByte, int size = 2)
        {
            AcreMapByte = acreMapByte;
            BuildingByte = buildingByte;
            TerrainByte = terrainByte;
            CustomDesignByte = customDesignByte;

            UpdatePlaza();

            if (ItemMapByte != null)
            {
                ItemMapData = new byte[numOfColumn][];

                mapSize = size;
                for (int i = 0; i < numOfColumn; i++)
                {
                    ItemMapData[i] = new byte[columnSize];
                    Buffer.BlockCopy(ItemMapByte, i * columnSize, ItemMapData[i], 0x0, columnSize);
                }

                TransformItemMap();
            }

            BuildTerrainUnits();
        }

        public Bitmap DrawBackground()
        {
            int[] AllAcre = new int[AcreMax];

            for (int i = 0; i < AcreMax; i++)
            {
                byte[] AcreBytes = new byte[2];
                AcreBytes[0] = AcreMapByte[i * 2];
                AcreBytes[1] = AcreMapByte[i * 2 + 1];
                AllAcre[i] = BitConverter.ToInt16(AcreBytes, 0);
            }

            int[] AcreWOOutside = new int[7 * 6];
            int counter = 0;

            for (int i = 1; i <= 6; i++)
            {
                for (int j = 1; j <= 7; j++)
                {
                    AcreWOOutside[counter] = AllAcre[i * 9 + j];
                    counter++;
                }
            }

            Bitmap terrainMap = DrawTerrainMap();

            Bitmap buildingMap = DrawBuildingMap();

            BuildBackgroundColor(AcreWOOutside);
            BuildBackgroundColorLess(AcreWOOutside);
            BuildBuildingColor();

            Bitmap[] AcreImage = new Bitmap[7 * 6];

            for (int i = 0; i < AcreImage.Length; i++)
            {
                AcreImage[i] = DrawAcre(GetAcreData(AcreWOOutside[i]));
                //AcreImage[i].Save(i + ".bmp");
            }

            return CombineMap(CombineMap(ToFullMap(AcreImage, 7, 6), terrainMap), buildingMap);
        }


        public Bitmap DrawFullBackground()
        {
            int[] AllAcre = new int[AcreMax];

            for (int i = 0; i < AcreMax; i++)
            {
                byte[] AcreBytes = new byte[2];
                AcreBytes[0] = AcreMapByte[i * 2];
                AcreBytes[1] = AcreMapByte[i * 2 + 1];
                AllAcre[i] = BitConverter.ToInt16(AcreBytes, 0);
            }

            Bitmap[] AllAcreImage = new Bitmap[9 * 8];

            for (int i = 0; i < AllAcreImage.Length; i++)
            {
                AllAcreImage[i] = DrawAcre(GetAcreData(AllAcre[i]));
                //AllAcreImage[i].Save(i + ".bmp");
            }

            Bitmap fullmap = ToFullMap(AllAcreImage, 9, 8);

            Bitmap terrainMap = DrawTerrainMap();

            Bitmap buildingMap = DrawFullBuildingMap();

            return CombineMap(CombineMap(fullmap, terrainMap, 16 * mapSize, 16 * mapSize), buildingMap);
        }

        public Bitmap DrawBuildingMap()
        {
            if (BuildingByte != null)
            {
                buildingList = new byte[NumOfBuilding][];
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    buildingList[i] = new byte[BuildingSize];
                    Buffer.BlockCopy(BuildingByte, i * BuildingSize, buildingList[i], 0x0, BuildingSize);
                }
            }


            Bitmap myBitmap;

            myBitmap = new Bitmap(numOfColumn * mapSize, numOfRow * mapSize);

            using (Graphics gr = Graphics.FromImage(myBitmap))
            {
                gr.SmoothingMode = SmoothingMode.None;

                //plaza

                for (int m = plazaTopX; m <= plazaTopX + 11; m++)
                {
                    if (m < 0 || m >= numOfColumn)
                        continue;
                    for (int n = plazaTopY; n <= plazaTopY + 9; n++)
                    {
                        if (n < 0 || n >= numOfRow)
                            continue;
                        PutPixel(gr, m * mapSize, n * mapSize, Color.DarkSalmon);
                    }
                }

                //========================================
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    int buildingTopX = -1;
                    int buildingTopY = -1;
                    int buildingBottomX = -1;
                    int buildingBottomY = -1;
                    Color BuildingColor;

                    BuildingType CurrentBuilding = (BuildingType)buildingList[i][0];
                    int BuildingX = (buildingList[i][2] - 0x20) / 2;
                    int BuildingY = (buildingList[i][4] - 0x20) / 2;

                    if (CurrentBuilding != BuildingType.None)
                    {
                        BuildingColor = DrawBuildingSetup(buildingList[i][0], BuildingX, BuildingY, ref buildingTopX, ref buildingTopY, ref buildingBottomX, ref buildingBottomY, false);

                        for (int j = buildingTopX; j <= buildingBottomX; j++)
                        {
                            if (j < 0 || j >= numOfColumn)
                                continue;
                            for (int k = buildingTopY; k <= buildingBottomY; k++)
                            {
                                if (k < 0 || k >= numOfRow)
                                    continue;
                                PutPixel(gr, j * mapSize, k * mapSize, BuildingColor);
                            }
                        }

                        PutPixel(gr, BuildingX * mapSize, BuildingY * mapSize, Color.Red);
                    }
                }
            }

            return myBitmap;
        }

        public Bitmap DrawTerrainMap()
        {
            Bitmap myBitmap;

            myBitmap = new Bitmap(numOfColumn * mapSize, numOfRow * mapSize);

            using (Graphics gr = Graphics.FromImage(myBitmap))
            {
                gr.SmoothingMode = SmoothingMode.None;

                Color terrainColor = Color.Black;

                for (int i = 0; i < numOfColumn; i++)
                {
                    for (int j = 0; j < numOfRow; j++)
                    {
                        if (terrainUnits[i][j].HaveCustomDesign())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Design];
                            PutPixel(gr, i * mapSize, j * mapSize, terrainColor);
                        }
                        else if (terrainUnits[i][j].HasRoad())
                        {
                            if (terrainUnits[i][j].HasRoadWood())
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadWood];
                            }
                            else if (terrainUnits[i][j].HasRoadTile())
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadTile];
                            }
                            else if (terrainUnits[i][j].HasRoadSand())
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadSand];
                            }
                            else if (terrainUnits[i][j].HasRoadPattern())
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadPattern];
                            }
                            else if (terrainUnits[i][j].HasRoadDarkSoil())
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadDarkSoil];
                            }
                            else if (terrainUnits[i][j].HasRoadBrick())
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadBrick];
                            }
                            else if (terrainUnits[i][j].HasRoadStone())
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadStone];
                            }
                            else if (terrainUnits[i][j].HasRoadSoil())
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadSoil];
                            }
                            PutPixel(gr, i * mapSize, j * mapSize, terrainColor);
                        }
                        else
                        {
                            if (terrainUnits[i][j].IsCliff())
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Cliff];
                                PutPixel(gr, i * mapSize, j * mapSize, terrainColor);
                            }
                            else if (terrainUnits[i][j].IsFall())
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Fall];
                                PutPixel(gr, i * mapSize, j * mapSize, terrainColor);
                            }
                            else if (terrainUnits[i][j].IsRiver())
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.River];
                                PutPixel(gr, i * mapSize, j * mapSize, terrainColor);
                            }
                            else
                            {
                                if (terrainUnits[i][j].GetElevation() == 1)
                                {
                                    terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation1];
                                    PutPixel(gr, i * mapSize, j * mapSize, terrainColor);
                                }
                                else if (terrainUnits[i][j].GetElevation() == 2)
                                {
                                    terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation2];
                                    PutPixel(gr, i * mapSize, j * mapSize, terrainColor);
                                }
                                else if (terrainUnits[i][j].GetElevation() >= 3)
                                {
                                    terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation3];
                                    PutPixel(gr, i * mapSize, j * mapSize, terrainColor);
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
            }

            return myBitmap;
        }

        public string GetTerrainData(int x, int y)
        {
            return terrainUnits[x][y].DisplayData();
        }

        public Bitmap DrawFullBuildingMap()
        {
            if (BuildingByte != null)
            {
                buildingList = new byte[NumOfBuilding][];
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    buildingList[i] = new byte[BuildingSize];
                    Buffer.BlockCopy(BuildingByte, i * BuildingSize, buildingList[i], 0x0, BuildingSize);
                }
            }


            Bitmap myBitmap;

            myBitmap = new Bitmap(fullNumOfColumn * mapSize, fullNumOfRow * mapSize);

            using (Graphics gr = Graphics.FromImage(myBitmap))
            {
                gr.SmoothingMode = SmoothingMode.None;

                //plaza

                for (int m = plazaX; m <= plazaX + (11 * 2); m++)
                {
                    if (m < 0 || m >= fullNumOfColumn * 2)
                        continue;
                    for (int n = plazaY; n <= plazaY + (9 * 2); n++)
                    {
                        if (n < 0 || n >= fullNumOfRow * 2)
                            continue;
                        PutPixel(gr, m * (mapSize / 2), n * (mapSize / 2), Color.DarkSalmon, (mapSize / 2));
                    }
                }


                //========================================
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    int buildingTopX = -1;
                    int buildingTopY = -1;
                    int buildingBottomX = -1;
                    int buildingBottomY = -1;
                    Color BuildingColor;

                    BuildingType CurrentBuilding = (BuildingType)buildingList[i][0];
                    int BuildingX = buildingList[i][2];
                    int BuildingY = buildingList[i][4];

                    if (CurrentBuilding != BuildingType.None)
                    {
                        BuildingColor = DrawBuildingSetup(buildingList[i][0], BuildingX, BuildingY, ref buildingTopX, ref buildingTopY, ref buildingBottomX, ref buildingBottomY, true);

                        for (int j = buildingTopX; j <= buildingBottomX; j++)
                        {
                            if (j < 0 || j >= fullNumOfColumn * 2)
                                continue;
                            for (int k = buildingTopY; k <= buildingBottomY; k++)
                            {
                                if (k < 0 || k >= fullNumOfRow * 2)
                                    continue;
                                PutPixel(gr, j * (mapSize / 2), k * (mapSize / 2), BuildingColor, (mapSize / 2));
                            }
                        }

                        PutPixel(gr, BuildingX * (mapSize / 2), BuildingY * (mapSize / 2), Color.Red, (mapSize / 2));
                    }
                }
            }

            return myBitmap;
        }

        private static byte[] GetAcreData(int AcreNumber)
        {
            byte[] data = new byte[0x100];
            if (AcreNumber * 0x100 < AcreData.Length)
                Buffer.BlockCopy(AcreData, AcreNumber * 0x100, data, 0, 0x100);
            return data;
        }

        public static Bitmap DrawAcre(byte[] OneAcre)
        {
            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * mapSize, 16 * mapSize);

            using (Graphics gr = Graphics.FromImage(myBitmap))
            {
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        PutPixel(gr, j * mapSize, i * mapSize, Pixel[OneAcre[i * 0x10 + j]]);
                    }
                }
            }

            return myBitmap;
        }

        private static Bitmap ToFullMap(Bitmap[] AcreImage, int width, int height)
        {
            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * width * mapSize, 16 * height * mapSize);

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                var cm = new ColorMatrix
                {
                    Matrix33 = 1
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                int ImageNum = 0;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        graphics.DrawImage(AcreImage[ImageNum], new Rectangle(j * 16 * mapSize, i * 16 * mapSize, AcreImage[ImageNum].Width, AcreImage[ImageNum].Height), 0, 0, AcreImage[ImageNum].Width, AcreImage[ImageNum].Height, GraphicsUnit.Pixel, ia);
                        ImageNum++;
                    }
                }
            }

            return myBitmap;
        }

        public static Bitmap CombineMap(Bitmap bottom, Bitmap top, int x = 0, int y = 0)
        {
            Bitmap myBitmap = bottom;

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                var cm = new ColorMatrix
                {
                    Matrix33 = 1
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(top, new Rectangle(x, y, top.Width, top.Height), 0, 0, top.Width, top.Height, GraphicsUnit.Pixel, ia);
            }


            return myBitmap;
        }

        public Image RefreshItemMap(byte[] itemData)
        {
            ItemMapData = null;
            tilesType = null;

            ItemMapData = new byte[numOfColumn][];

            for (int i = 0; i < numOfColumn; i++)
            {
                ItemMapData[i] = new byte[columnSize];
                Buffer.BlockCopy(itemData, i * columnSize, ItemMapData[i], 0x0, columnSize);
            }
            TransformItemMap();

            Image itemMap = DrawItemMap();
            Image myBitmap = DrawBackground();

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                var cm = new ColorMatrix
                {
                    Matrix33 = 1
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(itemMap, new Rectangle(0, 0, itemMap.Width, itemMap.Height), 0, 0, itemMap.Width, itemMap.Height, GraphicsUnit.Pixel, ia);
            }

            return myBitmap;
        }

        public static Bitmap DrawSelectSquare(int x, int y)
        {
            Bitmap square = new(7 * mapSize + 2, 7 * mapSize + 2);
            Pen p = new(Color.Red, 2 * mapSize);
            using (Graphics g = Graphics.FromImage(square))
            {
                g.Clear(Color.Transparent);
                g.DrawRectangle(p, new Rectangle(0, 0, square.Width, square.Height));
            }


            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * 7 * mapSize, 16 * 6 * mapSize);

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                graphics.Clear(Color.Transparent);

                var cm = new ColorMatrix
                {
                    Matrix33 = 1
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(square, new Rectangle(x * mapSize - square.Width / 2 + mapSize / 2, y * mapSize - square.Height / 2 + mapSize / 2, square.Width, square.Height), 0, 0, square.Width, square.Height, GraphicsUnit.Pixel, ia);
            }
            return myBitmap;
        }

        public static Bitmap DrawSelectSquare16(int x, int y)
        {
            Bitmap square = new(16 * mapSize + 2, 16 * mapSize + 2);
            Pen p = new(Color.Red, 2 * mapSize);
            using (Graphics g = Graphics.FromImage(square))
            {
                g.Clear(Color.Transparent);
                g.DrawRectangle(p, new Rectangle(0, 0, square.Width, square.Height));
            }


            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * 7 * mapSize, 16 * 6 * mapSize);

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                graphics.Clear(Color.Transparent);

                var cm = new ColorMatrix
                {
                    Matrix33 = 1
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(square, new Rectangle(x * mapSize - square.Width / 2 + mapSize / 2, y * mapSize - square.Height / 2 + mapSize / 2, square.Width, square.Height), 0, 0, square.Width, square.Height, GraphicsUnit.Pixel, ia);
            }
            return myBitmap;
        }

        public static Bitmap DrawSelectAcre(int x, int y)
        {
            Bitmap square = new(16 * mapSize + 2, 16 * mapSize + 2);
            Pen p = new(Color.Red, 2 * mapSize);
            using (Graphics g = Graphics.FromImage(square))
            {
                g.Clear(Color.Transparent);
                g.DrawRectangle(p, new Rectangle(0, 0, square.Width, square.Height));
            }


            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * 9 * mapSize, 16 * 8 * mapSize);

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                graphics.Clear(Color.Transparent);

                var cm = new ColorMatrix
                {
                    Matrix33 = 1
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(square, new Rectangle(x * 16 * mapSize, y * 16 * mapSize, square.Width, square.Height), 0, 0, square.Width, square.Height, GraphicsUnit.Pixel, ia);
            }
            return myBitmap;
        }

        public static Bitmap DrawEdge()
        {
            Bitmap Rectangle = new(16 * 7 * mapSize + 2, 16 * 6 * mapSize + 2);
            Pen p = new(Color.Tomato, 4);
            using (Graphics g = Graphics.FromImage(Rectangle))
            {
                g.Clear(Color.Transparent);
                g.DrawRectangle(p, new Rectangle(0, 0, Rectangle.Width, Rectangle.Height));
            }


            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * 9 * mapSize, 16 * 8 * mapSize);

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                graphics.Clear(Color.Transparent);

                var cm = new ColorMatrix
                {
                    Matrix33 = 1
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(Rectangle, new Rectangle(16 * mapSize, 16 * mapSize, Rectangle.Width, Rectangle.Height), 0, 0, Rectangle.Width, Rectangle.Height, GraphicsUnit.Pixel, ia);
            }
            return myBitmap;
        }

        public static Bitmap DrawMarker(int x, int y)
        {
            Bitmap marker = new(Properties.Resources.marker);

            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * 7 * mapSize, 16 * 6 * mapSize);

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                graphics.Clear(Color.Transparent);

                var cm = new ColorMatrix
                {
                    Matrix33 = 1
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(marker, new Rectangle(x * mapSize - marker.Width / 2 + mapSize / 2, y * mapSize - marker.Height + mapSize / 2, marker.Width, marker.Height), 0, 0, marker.Width, marker.Height, GraphicsUnit.Pixel, ia);
            }
            return myBitmap;
        }

        public static Bitmap DrawLargeMarker(int OrgX, int OrgY, int NewX, int NewY, int lastbuilding = 0, byte buildingType = 0xFE)
        {
            Bitmap marker = new(Properties.Resources.marker);

            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * 9 * mapSize, 16 * 8 * mapSize);

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                graphics.Clear(Color.Transparent);

                var cm = new ColorMatrix
                {
                    Matrix33 = 1
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                if (buildingType != 0xFE) // 0xFE = no draw
                {
                    int buildingTopX = -1;
                    int buildingTopY = -1;
                    int buildingBottomX = -1;
                    int buildingBottomY = -1;
                    int BuildingX;
                    int BuildingY;
                    Color BuildingColor;

                    BuildingType CurrentBuilding = (BuildingType)buildingType;

                    if (OrgX != NewX || OrgY != NewY)
                    {
                        BuildingX = NewX;
                        BuildingY = NewY;
                        //=============================================== New
                        if (CurrentBuilding != BuildingType.None)
                        {
                            DrawBuildingSetup(buildingType, BuildingX, BuildingY, ref buildingTopX, ref buildingTopY, ref buildingBottomX, ref buildingBottomY, true);

                            BuildingColor = Color.FromArgb(150, Color.DeepPink);

                            for (int j = buildingTopX; j <= buildingBottomX; j++)
                            {
                                if (j < 0 || j >= fullNumOfColumn * 2)
                                    continue;
                                for (int k = buildingTopY; k <= buildingBottomY; k++)
                                {
                                    if (k < 0 || k >= fullNumOfRow * 2)
                                        continue;
                                    PutPixel(graphics, j * (mapSize / 2), k * (mapSize / 2), BuildingColor, (mapSize / 2));
                                }
                            }

                            if (lastbuilding != 0)
                            {
                                Pen linePen = new(Color.Red, 2);
                                graphics.DrawLine(linePen, OrgX * (mapSize / 2), OrgY * (mapSize / 2), NewX * (mapSize / 2), NewY * (mapSize / 2));
                            }
                        }
                    }
                    else
                    {
                        BuildingX = OrgX;
                        BuildingY = OrgY;

                        //=============================================== Org
                        if (CurrentBuilding != BuildingType.None)
                        {
                            DrawBuildingSetup(buildingType, BuildingX, BuildingY, ref buildingTopX, ref buildingTopY, ref buildingBottomX, ref buildingBottomY, true);

                            BuildingColor = Color.FromArgb(200, Color.LightPink);

                            for (int j = buildingTopX; j <= buildingBottomX; j++)
                            {
                                if (j < 0 || j >= fullNumOfColumn * 2)
                                    continue;
                                for (int k = buildingTopY; k <= buildingBottomY; k++)
                                {
                                    if (k < 0 || k >= fullNumOfRow * 2)
                                        continue;
                                    PutPixel(graphics, j * (mapSize / 2), k * (mapSize / 2), BuildingColor, (mapSize / 2));
                                }
                            }
                        }
                    }


                }

                graphics.DrawImage(marker, new Rectangle(NewX * mapSize / 2 - marker.Width / 2 + mapSize / 4, NewY * mapSize / 2 - marker.Height + mapSize / 4, marker.Width, marker.Height), 0, 0, marker.Width, marker.Height, GraphicsUnit.Pixel, ia);
            }
            return myBitmap;
        }

        public static Bitmap DrawPreview(int row, int column, int x, int y, bool right, bool[] isSpace = null)
        {
            int[][] previewtilesType;
            int Counter = 0;

            if (right)
            {
                previewtilesType = new int[numOfColumn][];

                for (int i = 0; i < numOfColumn; i++)
                {
                    previewtilesType[i] = new int[numOfRow];

                    for (int j = 0; j < numOfRow; j++)
                    {
                        if (i == x && j == y)
                        {
                            previewtilesType[i][j] = 2;
                            Counter++;
                        }
                        else if (i >= x && i < x + column && j >= y && j < y + row)
                        {
                            if (isSpace != null)
                            {
                                if (Counter >= isSpace.Length)
                                    previewtilesType[i][j] = 3;
                                else if (isSpace[Counter])
                                    previewtilesType[i][j] = 3;
                                else
                                    previewtilesType[i][j] = 1;
                            }
                            else
                                previewtilesType[i][j] = 1;
                            Counter++;
                        }
                        else
                            previewtilesType[i][j] = 0;
                    }
                }
            }
            else
            {
                previewtilesType = new int[numOfColumn][];

                for (int i = numOfColumn - 1; i >= 0; i--)
                {
                    previewtilesType[i] = new int[numOfRow];

                    for (int j = 0; j < numOfRow; j++)
                    {
                        if (i == x && j == y)
                        {
                            previewtilesType[i][j] = 2;
                            Counter++;
                        }
                        else if (i <= x && i > x - column && j >= y && j < y + row)
                        {
                            if (isSpace != null)
                            {
                                if (Counter >= isSpace.Length)
                                    previewtilesType[i][j] = 3;
                                else if (isSpace[Counter])
                                    previewtilesType[i][j] = 3;
                                else
                                    previewtilesType[i][j] = 1;
                            }
                            else
                                previewtilesType[i][j] = 1;
                            Counter++;
                        }
                        else
                            previewtilesType[i][j] = 0;
                    }
                }
            }



            Bitmap myBitmap;

            myBitmap = new Bitmap(numOfColumn * mapSize, numOfRow * mapSize);

            using (Graphics gr = Graphics.FromImage(myBitmap))
            {
                gr.SmoothingMode = SmoothingMode.None;

                for (int i = 0; i < numOfColumn; i++)
                {
                    for (int j = 0; j < numOfRow; j++)
                    {
                        if (previewtilesType[i][j] == 1)
                            PutPixel(gr, i * mapSize, j * mapSize, Color.FromArgb(200, Color.Red));
                        else if (previewtilesType[i][j] == 2)
                            PutPixel(gr, i * mapSize, j * mapSize, Color.LightSkyBlue);
                        else if (previewtilesType[i][j] == 3)
                            PutPixel(gr, i * mapSize, j * mapSize, Color.FromArgb(200, Color.HotPink));

                    }
                }
            }

            return myBitmap;
        }

        public static Bitmap DrawPreviewHori(int row, int column, int x, int y, bool right, bool[] isSpace = null)
        {
            int[][] previewtilesType;
            int Counter = 0;

            if (right)
            {
                previewtilesType = new int[numOfRow][];

                for (int i = 0; i < numOfRow; i++)
                {
                    previewtilesType[i] = new int[numOfColumn];

                    for (int j = 0; j < numOfColumn; j++)
                    {
                        if (j == x && i == y)
                        {
                            previewtilesType[i][j] = 2;
                            Counter++;
                        }
                        else if (j >= x && j < x + column && i >= y && i < y + row)
                        {
                            if (isSpace != null)
                            {
                                if (Counter >= isSpace.Length)
                                    previewtilesType[i][j] = 3;
                                else if (isSpace[Counter])
                                    previewtilesType[i][j] = 3;
                                else
                                    previewtilesType[i][j] = 1;
                            }
                            else
                                previewtilesType[i][j] = 1;
                            Counter++;
                        }
                        else
                            previewtilesType[i][j] = 0;
                    }
                }
            }
            else
            {
                previewtilesType = new int[numOfRow][];

                for (int i = 0; i < numOfRow; i++)
                {
                    previewtilesType[i] = new int[numOfColumn];

                    for (int j = numOfColumn - 1; j >= 0; j--)
                    {
                        if (j == x && i == y)
                        {
                            previewtilesType[i][j] = 2;
                            Counter++;
                        }
                        else if (j <= x && j > x - column && i >= y && i < y + row)
                        {
                            if (isSpace != null)
                            {
                                if (Counter >= isSpace.Length)
                                    previewtilesType[i][j] = 3;
                                else if (isSpace[Counter])
                                    previewtilesType[i][j] = 3;
                                else
                                    previewtilesType[i][j] = 1;
                            }
                            else
                                previewtilesType[i][j] = 1;
                            Counter++;
                        }
                        else
                            previewtilesType[i][j] = 0;
                    }
                }
            }



            Bitmap myBitmap;

            myBitmap = new Bitmap(numOfColumn * mapSize, numOfRow * mapSize);

            using (Graphics gr = Graphics.FromImage(myBitmap))
            {
                gr.SmoothingMode = SmoothingMode.None;

                for (int i = 0; i < numOfRow; i++)
                {
                    for (int j = 0; j < numOfColumn; j++)
                    {
                        if (previewtilesType[i][j] == 1)
                            PutPixel(gr, j * mapSize, i * mapSize, Color.FromArgb(200, Color.Red));
                        else if (previewtilesType[i][j] == 2)
                            PutPixel(gr, j * mapSize, i * mapSize, Color.LightSkyBlue);
                        else if (previewtilesType[i][j] == 3)
                            PutPixel(gr, j * mapSize, i * mapSize, Color.FromArgb(200, Color.HotPink));
                    }
                }
            }

            return myBitmap;
        }


        private void TransformItemMap()
        {
            tilesType = new int[numOfColumn][];

            for (int i = 0; i < numOfColumn; i++)
            {
                tilesType[i] = new int[numOfRow];

                for (int j = 0; j < numOfRow; j++)
                {
                    byte[] tempPart1 = new byte[0x8];
                    byte[] tempPart2 = new byte[0x8];
                    byte[] tempPart3 = new byte[0x8];
                    byte[] tempPart4 = new byte[0x8];
                    byte[] IDByte = new byte[0x2];

                    Buffer.BlockCopy(ItemMapData[i], j * 0x10, tempPart1, 0x0, 0x8);
                    Buffer.BlockCopy(ItemMapData[i], j * 0x10 + 0x8, tempPart2, 0x0, 0x8);
                    Buffer.BlockCopy(ItemMapData[i], j * 0x10 + 0x600, tempPart3, 0x0, 0x8);
                    Buffer.BlockCopy(ItemMapData[i], j * 0x10 + 0x600 + 0x8, tempPart4, 0x0, 0x8);
                    Buffer.BlockCopy(ItemMapData[i], j * 0x10, IDByte, 0x0, 0x2);

                    string strPart1 = Utilities.ByteToHexString(tempPart1);
                    string strPart2 = Utilities.ByteToHexString(tempPart2);
                    string strPart3 = Utilities.ByteToHexString(tempPart3);
                    string strPart4 = Utilities.ByteToHexString(tempPart4);
                    ushort itemID = Convert.ToUInt16(Utilities.Flip(Utilities.ByteToHexString(IDByte)), 16);

                    if (ItemAttr.isTree(itemID))
                    {
                        tilesType[i][j] = 1; // Tree
                    }
                    else if (ItemAttr.hasGenetics(itemID))
                    {
                        tilesType[i][j] = 2; // Flower
                    }
                    else if (ItemAttr.isShell(itemID))
                    {
                        tilesType[i][j] = 3; // Shell
                    }
                    else if (ItemAttr.isWeed(itemID))
                    {
                        tilesType[i][j] = 4; // Weed
                    }
                    else if (ItemAttr.isFence(itemID))
                    {
                        tilesType[i][j] = 5; // Fence
                    }
                    else if (ItemAttr.isStone(itemID))
                    {
                        tilesType[i][j] = 6; // Stone
                    }
                    else if (ItemAttr.hasQuantity(itemID))
                    {
                        tilesType[i][j] = 9; // Material
                    }
                    else if (itemID == 0x16A2)
                    {
                        tilesType[i][j] = 10; // Recipe
                    }
                    else if (itemID == 0x315A || itemID == 0x1618 || itemID == 0x342F) // Wall-mount
                    {
                        tilesType[i][j] = 11;
                    }
                    else if (itemID == 0x1A9F || itemID == 0x1AF1 || itemID == 0x12BA) // transparent
                    {
                        tilesType[i][j] = 12;
                    }
                    else if (ItemAttr.isPlacedFence(itemID))
                    {
                        tilesType[i][j] = 13; // PlacedFence
                    }
                    else if (strPart1.Equals("FEFF000000000000") && strPart2.Equals("FEFF000000000000") && strPart3.Equals("FEFF000000000000") && strPart4.Equals("FEFF000000000000"))
                    {
                        tilesType[i][j] = 0; // Empty
                    }
                    else
                    {
                        tilesType[i][j] = 69;
                    }
                }
            }
        }

        private void BuildTerrainUnits()
        {
            int counter = 0;

            terrainUnits = new TerrainUnit[numOfColumn][];
            for (int i = 0; i < numOfColumn; i++)
            {
                terrainUnits[i] = new TerrainUnit[numOfRow];
                for (int j = 0; j < numOfRow; j++)
                {
                    byte[] currentTile = new byte[TerrainSize];
                    Buffer.BlockCopy(TerrainByte, counter * TerrainSize, currentTile, 0, TerrainSize);
                    terrainUnits[i][j] = new TerrainUnit(currentTile);

                    if (CustomDesignByte != null)
                    {
                        byte[] currentDesign = new byte[2];
                        Buffer.BlockCopy(CustomDesignByte, (i * numOfRow + j) * 2, currentDesign, 0, 2);
                        terrainUnits[i][j].SetCustomDesign(currentDesign);
                    }

                    counter++;
                }
            }
        }

        public Bitmap DrawItemMap()
        {
            Bitmap myBitmap;

            myBitmap = new Bitmap(numOfColumn * mapSize, numOfRow * mapSize);

            using (Graphics gr = Graphics.FromImage(myBitmap))
            {
                gr.SmoothingMode = SmoothingMode.None;

                for (int i = 0; i < numOfColumn; i++)
                {
                    for (int j = 0; j < numOfRow; j++)
                    {
                        if (tilesType[i][j] == 0)
                        {
                            //PutPixel(gr, i * mapSize, j* mapSize, Color.White);
                        }
                        else if (tilesType[i][j] == 1)
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.GreenYellow);
                        }
                        else if (tilesType[i][j] == 2)
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.Pink);
                        }
                        else if (tilesType[i][j] == 3)
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.Blue);
                        }
                        else if (tilesType[i][j] == 4)
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.MediumSeaGreen);
                        }
                        else if (tilesType[i][j] == 5)
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.Purple);
                        }
                        else if (tilesType[i][j] == 6)
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.Black);
                        }
                        else if (tilesType[i][j] == 9)
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.Yellow);
                        }
                        else if (tilesType[i][j] == 10)
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.Gold);
                        }
                        else if (tilesType[i][j] == 11)
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.Khaki);
                        }
                        else if (tilesType[i][j] == 12)
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.Maroon);
                        }
                        else if (tilesType[i][j] == 13)
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.Tan);
                        }
                        else
                        {
                            PutPixel(gr, i * mapSize, j * mapSize, Color.Gray);
                        }
                    }
                }
            }

            return myBitmap;
            /*
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Bitmap myBitmap = new Bitmap(16 * 7 * mapSize, 16 * 6 * mapSize);
                Graphics g = Graphics.FromImage(myBitmap);
                g.Clear(Color.Transparent);
                return myBitmap;
            }
            */
        }

        private void BuildBackgroundColor(int[] AcreWOOutside)
        {
            floorBackgroundColor = new Color[numOfRow][];

            for (int i = 0; i < numOfRow; i++)
            {
                floorBackgroundColor[i] = new Color[numOfColumn];
                for (int j = 0; j < numOfColumn; j++)
                {
                    floorBackgroundColor[i][j] = Color.Tomato;
                }
            }

            for (int i = 0; i < 6; i++) // y
            {
                for (int j = 0; j < 7; j++) // x
                {
                    byte[] Acre = GetAcreData(AcreWOOutside[i * 7 + j]);

                    for (int m = 0; m < 16; m++) // y
                    {
                        for (int n = 0; n < 16; n++) // x
                        {
                            if (floorBackgroundColor[i * 16 + m][j * 16 + n] != Color.Tomato)
                                Debug.Print(i + " " + j + " " + m + " " + n);
                            floorBackgroundColor[i * 16 + m][j * 16 + n] = Pixel[Acre[m * 16 + n]];
                        }
                    }
                }
            }



            for (int i = 0; i < numOfRow; i++)
            {
                for (int j = 0; j < numOfColumn; j++)
                {
                    Color terrainColor = Color.Black;

                    if (terrainUnits[j][i].HaveCustomDesign())
                    {
                        terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Design];
                        floorBackgroundColor[i][j] = terrainColor;
                    }
                    else if (terrainUnits[j][i].HasRoad())
                    {
                        if (terrainUnits[j][i].HasRoadWood())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadWood];
                        }
                        else if (terrainUnits[j][i].HasRoadTile())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadTile];
                        }
                        else if (terrainUnits[j][i].HasRoadSand())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadSand];
                        }
                        else if (terrainUnits[j][i].HasRoadPattern())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadPattern];
                        }
                        else if (terrainUnits[j][i].HasRoadDarkSoil())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadDarkSoil];
                        }
                        else if (terrainUnits[j][i].HasRoadBrick())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadBrick];
                        }
                        else if (terrainUnits[j][i].HasRoadStone())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadStone];
                        }
                        else if (terrainUnits[j][i].HasRoadSoil())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.RoadSoil];
                        }
                        floorBackgroundColor[i][j] = terrainColor;
                    }
                    else
                    {
                        if (terrainUnits[j][i].IsCliff())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Cliff];
                            floorBackgroundColor[i][j] = terrainColor;
                        }
                        else if (terrainUnits[j][i].IsFall())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Fall];
                            floorBackgroundColor[i][j] = terrainColor;
                        }
                        else if (terrainUnits[j][i].IsRiver())
                        {
                            terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.River];
                            floorBackgroundColor[i][j] = terrainColor;
                        }
                        else
                        {
                            if (terrainUnits[j][i].GetElevation() == 1)
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation1];
                                floorBackgroundColor[i][j] = terrainColor;
                            }
                            else if (terrainUnits[j][i].GetElevation() == 2)
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation2];
                                floorBackgroundColor[i][j] = terrainColor;
                            }
                            else if (terrainUnits[j][i].GetElevation() >= 3)
                            {
                                terrainColor = TerrainUnit.TerrainColor[(int)TerrainUnit.TerrainType.Elevation3];
                                floorBackgroundColor[i][j] = terrainColor;
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }

            for (int m = plazaTopX; m <= plazaTopX + 11; m++)
            {
                if (m < 0 || m >= numOfColumn)
                    continue;
                for (int n = plazaTopY; n <= plazaTopY + 9; n++)
                {
                    if (n < 0 || n >= numOfRow)
                        continue;
                    floorBackgroundColor[n][m] = Color.DarkSalmon;
                }
            }

            if (buildingList != null)
            {
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    int buildingTopX;
                    int buildingTopY;
                    int buildingBottomX;
                    int buildingBottomY;
                    Color BuildingColor;

                    BuildingType CurrentBuilding = (BuildingType)buildingList[i][0];
                    int BuildingX = (buildingList[i][2] - 0x20) / 2;
                    int BuildingY = (buildingList[i][4] - 0x20) / 2;

                    if (CurrentBuilding != BuildingType.None)
                    {
                        if (BuildingX < 0 || BuildingX >= numOfColumn || BuildingY < 0 || BuildingY >= numOfRow)
                        {

                        }
                        else
                            floorBackgroundColor[BuildingY][BuildingX] = Color.Red;

                        if (CurrentBuilding == BuildingType.ResidentServicesBuilding)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY;
                        }
                        else if (CurrentBuilding >= BuildingType.PlayerHouse1 && CurrentBuilding <= BuildingType.PlayerHouse8)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY;
                        }
                        else if (CurrentBuilding >= BuildingType.VillagerHouse1 && CurrentBuilding <= BuildingType.VillagerHouse10)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 1;
                            buildingBottomY = BuildingY;
                        }
                        else if (CurrentBuilding == BuildingType.NooksCranny)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY + 2;
                        }
                        else if (CurrentBuilding == BuildingType.Museum)
                        {
                            buildingTopX = BuildingX - 4;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 3;
                            buildingBottomY = BuildingY + 1;
                        }
                        else if (CurrentBuilding == BuildingType.AblesSisters)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY + 1;
                        }
                        else if (CurrentBuilding == BuildingType.Airport)
                        {
                            buildingTopX = BuildingX - 5;
                            buildingTopY = BuildingY - 5;
                            buildingBottomX = BuildingX + 4;
                            buildingBottomY = BuildingY + 3;
                        }
                        else if (CurrentBuilding == BuildingType.Campsite)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 1;
                            buildingBottomY = BuildingY + 1;
                        }
                        else if (CurrentBuilding == BuildingType.ReddsTreasureTrawler)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY;
                        }
                        else if (CurrentBuilding == BuildingType.Bridge || CurrentBuilding == BuildingType.Incline)
                        {
                            buildingTopX = BuildingX - 1;
                            buildingTopY = BuildingY - 1;
                            buildingBottomX = BuildingX;
                            buildingBottomY = BuildingY;
                        }
                        else
                        {
                            buildingTopX = BuildingX - 1;
                            buildingTopY = BuildingY - 1;
                            buildingBottomX = BuildingX;
                            buildingBottomY = BuildingY;
                        }

                        if (ByteToBuildingColor.ContainsKey(buildingList[i][0]))
                            BuildingColor = ByteToBuildingColor[buildingList[i][0]];
                        else
                            BuildingColor = Color.Black;

                        for (int j = buildingTopX; j <= buildingBottomX; j++)
                        {
                            if (j < 0 || j >= numOfColumn)
                                continue;
                            for (int k = buildingTopY; k <= buildingBottomY; k++)
                            {
                                if (k < 0 || k >= numOfRow)
                                    continue;
                                floorBackgroundColor[k][j] = BuildingColor;
                                if (k == BuildingY && j == BuildingX)
                                    floorBackgroundColor[k][j] = Color.Red;
                            }
                        }
                    }
                }
            }
        }

        private static void BuildBackgroundColorLess(int[] AcreWOOutside)
        {
            floorBackgroundColorLess = new Color[numOfRow][];

            for (int i = 0; i < numOfRow; i++)
            {
                floorBackgroundColorLess[i] = new Color[numOfColumn];
                for (int j = 0; j < numOfColumn; j++)
                {
                    floorBackgroundColorLess[i][j] = Color.Tomato;
                }
            }

            for (int i = 0; i < 6; i++) // y
            {
                for (int j = 0; j < 7; j++) // x
                {
                    byte[] Acre = GetAcreData(AcreWOOutside[i * 7 + j]);

                    for (int m = 0; m < 16; m++) // y
                    {
                        for (int n = 0; n < 16; n++) // x
                        {
                            if (floorBackgroundColorLess[i * 16 + m][j * 16 + n] != Color.Tomato)
                                Debug.Print(i + " " + j + " " + m + " " + n);
                            floorBackgroundColorLess[i * 16 + m][j * 16 + n] = Pixel[Acre[m * 16 + n]];
                        }
                    }
                }
            }
        }

        public void BuildBuildingColor()
        {
            floorBuildingColor = new Color[numOfRow][];

            for (int i = 0; i < numOfRow; i++)
            {
                floorBuildingColor[i] = new Color[numOfColumn];
                for (int j = 0; j < numOfColumn; j++)
                {
                    floorBuildingColor[i][j] = Color.Transparent;
                }
            }

            for (int m = plazaTopX; m <= plazaTopX + 11; m++)
            {
                if (m < 0 || m >= numOfColumn)
                    continue;
                for (int n = plazaTopY; n <= plazaTopY + 9; n++)
                {
                    if (n < 0 || n >= numOfRow)
                        continue;
                    floorBuildingColor[n][m] = Color.DarkSalmon;
                }
            }

            if (buildingList != null)
            {
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    int buildingTopX;
                    int buildingTopY;
                    int buildingBottomX;
                    int buildingBottomY;
                    Color BuildingColor;

                    BuildingType CurrentBuilding = (BuildingType)buildingList[i][0];
                    int BuildingX = (buildingList[i][2] - 0x20) / 2;
                    int BuildingY = (buildingList[i][4] - 0x20) / 2;

                    if (CurrentBuilding != BuildingType.None)
                    {
                        if (BuildingX < 0 || BuildingX >= numOfColumn || BuildingY < 0 || BuildingY >= numOfRow)
                        {

                        }
                        else
                            floorBuildingColor[BuildingY][BuildingX] = Color.Red;

                        if (CurrentBuilding == BuildingType.ResidentServicesBuilding)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY;
                        }
                        else if (CurrentBuilding >= BuildingType.PlayerHouse1 && CurrentBuilding <= BuildingType.PlayerHouse8)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY;
                        }
                        else if (CurrentBuilding >= BuildingType.VillagerHouse1 && CurrentBuilding <= BuildingType.VillagerHouse10)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 1;
                            buildingBottomY = BuildingY;
                        }
                        else if (CurrentBuilding == BuildingType.NooksCranny)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY + 2;
                        }
                        else if (CurrentBuilding == BuildingType.Museum)
                        {
                            buildingTopX = BuildingX - 4;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 3;
                            buildingBottomY = BuildingY + 1;
                        }
                        else if (CurrentBuilding == BuildingType.AblesSisters)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY + 1;
                        }
                        else if (CurrentBuilding == BuildingType.Airport)
                        {
                            buildingTopX = BuildingX - 5;
                            buildingTopY = BuildingY - 5;
                            buildingBottomX = BuildingX + 4;
                            buildingBottomY = BuildingY + 3;
                        }
                        else if (CurrentBuilding == BuildingType.Campsite)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 1;
                            buildingBottomY = BuildingY + 1;
                        }
                        else if (CurrentBuilding == BuildingType.ReddsTreasureTrawler)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY;
                        }
                        else if (CurrentBuilding == BuildingType.Bridge || CurrentBuilding == BuildingType.Incline)
                        {
                            buildingTopX = BuildingX - 1;
                            buildingTopY = BuildingY - 1;
                            buildingBottomX = BuildingX;
                            buildingBottomY = BuildingY;
                        }
                        else
                        {
                            buildingTopX = BuildingX - 1;
                            buildingTopY = BuildingY - 1;
                            buildingBottomX = BuildingX;
                            buildingBottomY = BuildingY;
                        }

                        if (ByteToBuildingColor.ContainsKey(buildingList[i][0]))
                            BuildingColor = ByteToBuildingColor[buildingList[i][0]];
                        else
                            BuildingColor = Color.Black;

                        for (int j = buildingTopX; j <= buildingBottomX; j++)
                        {
                            if (j < 0 || j >= numOfColumn)
                                continue;
                            for (int k = buildingTopY; k <= buildingBottomY; k++)
                            {
                                if (k < 0 || k >= numOfRow)
                                    continue;
                                floorBuildingColor[k][j] = BuildingColor;
                                if (k == BuildingY && j == BuildingX)
                                    floorBuildingColor[k][j] = Color.Red;
                            }
                        }
                    }
                }
            }
        }

        public static Color GetBackgroundColor(int x, int y, bool Layer1 = true)
        {
            if (floorBackgroundColor == null)
                return Color.White;
            else if (Layer1)
                return floorBackgroundColor[y][x];
            else
            {
                Color newColor = Color.FromArgb(150, floorBackgroundColor[y][x]);
                return newColor;
            }
        }

        public static Color GetBackgroundColorLess(int x, int y, bool Layer1 = true)
        {
            if (floorBackgroundColorLess == null)
                return Color.White;
            else if (x < 0 || y < 0)
                return Color.White;
            else if (x >= 112 || y >= 96)
                return Color.White;
            else if (Layer1)
                return floorBackgroundColorLess[y][x];
            else
            {
                Color newColor = Color.FromArgb(150, floorBackgroundColorLess[y][x]);
                return newColor;
            }
        }

        public static Color GetBuildingColor(int x, int y)
        {
            if (floorBuildingColor == null)
                return Color.Transparent;
            else
                return floorBuildingColor[y][x];
        }

        private static void PutPixel(Graphics g, int x, int y, Color c)
        {
            Bitmap Bmp = new(mapSize, mapSize);

            using (Graphics gfx = Graphics.FromImage(Bmp))
            using (SolidBrush brush = new(c))
            {
                gfx.SmoothingMode = SmoothingMode.None;
                gfx.FillRectangle(brush, 0, 0, mapSize, mapSize);
            }

            g.DrawImageUnscaled(Bmp, x, y);
        }

        private static void PutPixel(Graphics g, int x, int y, Color c, int size)
        {
            Bitmap Bmp = new(size, size);

            using (Graphics gfx = Graphics.FromImage(Bmp))
            using (SolidBrush brush = new(c))
            {
                gfx.SmoothingMode = SmoothingMode.None;
                gfx.FillRectangle(brush, 0, 0, size, size);
            }

            g.DrawImageUnscaled(Bmp, x, y);
        }

        public static Bitmap GetAcreImage(int id, int size)
        {
            Bitmap AcreImage;
            AcreImage = new Bitmap(16 * size, 16 * size);
            byte[] AcreData = GetAcreData(id);


            using (Graphics gr = Graphics.FromImage(AcreImage))
            {
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        PutPixel(gr, j * size, i * size, Pixel[AcreData[i * 0x10 + j]], size);
                    }
                }
            }

            return AcreImage;
        }

        public void UpdatePlaza()
        {
            if (AcreMapByte != null)
            {
                plazaX = AcreMapByte[AllAcreSize + 4];
                plazaY = AcreMapByte[AllAcreSize + 8];
                plazaTopX = (plazaX - 0x20) / 2;
                plazaTopY = (plazaY - 0x20) / 2;
            }
        }
        public static Color DrawBuildingSetup(byte BuildingByte, int BuildingX, int BuildingY, ref int buildingTopX, ref int buildingTopY, ref int buildingBottomX, ref int buildingBottomY, bool TimesTwo = false)
        {
            Color BuildingColor;

            BuildingType CurrentBuilding = (BuildingType)BuildingByte;

            if (TimesTwo)
            {
                if (CurrentBuilding == BuildingType.ResidentServicesBuilding)
                {
                    buildingTopX = BuildingX - 3 * 2;
                    buildingTopY = BuildingY - 2 * 2;
                    buildingBottomX = BuildingX + 2 * 2;
                    buildingBottomY = BuildingY;
                }
                else if (CurrentBuilding >= BuildingType.PlayerHouse1 && CurrentBuilding <= BuildingType.PlayerHouse8)
                {
                    buildingTopX = BuildingX - 2 * 2;
                    buildingTopY = BuildingY - 2 * 2;
                    buildingBottomX = BuildingX + 2 * 2;
                    buildingBottomY = BuildingY;
                }
                else if (CurrentBuilding >= BuildingType.VillagerHouse1 && CurrentBuilding <= BuildingType.VillagerHouse10)
                {
                    buildingTopX = BuildingX - 2 * 2;
                    buildingTopY = BuildingY - 2 * 2;
                    buildingBottomX = BuildingX + 1 * 2;
                    buildingBottomY = BuildingY;
                }
                else if (CurrentBuilding == BuildingType.NooksCranny)
                {
                    buildingTopX = BuildingX - 3 * 2;
                    buildingTopY = BuildingY - 2 * 2;
                    buildingBottomX = BuildingX + 2 * 2;
                    buildingBottomY = BuildingY + 2 * 2;
                }
                else if (CurrentBuilding == BuildingType.Museum)
                {
                    buildingTopX = BuildingX - 4 * 2;
                    buildingTopY = BuildingY - 2 * 2;
                    buildingBottomX = BuildingX + 3 * 2;
                    buildingBottomY = BuildingY + 1 * 2;
                }
                else if (CurrentBuilding == BuildingType.AblesSisters)
                {
                    buildingTopX = BuildingX - 3 * 2;
                    buildingTopY = BuildingY - 2 * 2;
                    buildingBottomX = BuildingX + 2 * 2;
                    buildingBottomY = BuildingY + 1 * 2;
                }
                else if (CurrentBuilding == BuildingType.Airport)
                {
                    buildingTopX = BuildingX - 5 * 2;
                    buildingTopY = BuildingY - 5 * 2;
                    buildingBottomX = BuildingX + 4 * 2;
                    buildingBottomY = BuildingY + 3 * 2;
                }
                else if (CurrentBuilding == BuildingType.Campsite)
                {
                    buildingTopX = BuildingX - 2 * 2;
                    buildingTopY = BuildingY - 2 * 2;
                    buildingBottomX = BuildingX + 1 * 2;
                    buildingBottomY = BuildingY + 1 * 2;
                }
                else if (CurrentBuilding == BuildingType.ReddsTreasureTrawler)
                {
                    buildingTopX = BuildingX - 2 * 2;
                    buildingTopY = BuildingY - 2 * 2;
                    buildingBottomX = BuildingX + 2 * 2;
                    buildingBottomY = BuildingY;
                }
                else if (CurrentBuilding == BuildingType.Bridge || CurrentBuilding == BuildingType.Incline)
                {
                    buildingTopX = BuildingX - 1 * 2;
                    buildingTopY = BuildingY - 1 * 2;
                    buildingBottomX = BuildingX;
                    buildingBottomY = BuildingY;
                }
                else if (BuildingByte == 0xFF)
                {
                    buildingTopX = BuildingX;
                    buildingTopY = BuildingY;
                    buildingBottomX = BuildingX + 11 * 2;
                    buildingBottomY = BuildingY + 9 * 2;
                }
                else
                {
                    buildingTopX = BuildingX - 1 * 2;
                    buildingTopY = BuildingY - 1 * 2;
                    buildingBottomX = BuildingX;
                    buildingBottomY = BuildingY;
                }
            }
            else
            {
                if (CurrentBuilding == BuildingType.ResidentServicesBuilding)
                {
                    buildingTopX = BuildingX - 3;
                    buildingTopY = BuildingY - 2;
                    buildingBottomX = BuildingX + 2;
                    buildingBottomY = BuildingY;
                }
                else if (CurrentBuilding >= BuildingType.PlayerHouse1 && CurrentBuilding <= BuildingType.PlayerHouse8)
                {
                    buildingTopX = BuildingX - 2;
                    buildingTopY = BuildingY - 2;
                    buildingBottomX = BuildingX + 2;
                    buildingBottomY = BuildingY;
                }
                else if (CurrentBuilding >= BuildingType.VillagerHouse1 && CurrentBuilding <= BuildingType.VillagerHouse10)
                {
                    buildingTopX = BuildingX - 2;
                    buildingTopY = BuildingY - 2;
                    buildingBottomX = BuildingX + 1;
                    buildingBottomY = BuildingY;
                }
                else if (CurrentBuilding == BuildingType.NooksCranny)
                {
                    buildingTopX = BuildingX - 3;
                    buildingTopY = BuildingY - 2;
                    buildingBottomX = BuildingX + 2;
                    buildingBottomY = BuildingY + 2;
                }
                else if (CurrentBuilding == BuildingType.Museum)
                {
                    buildingTopX = BuildingX - 4;
                    buildingTopY = BuildingY - 2;
                    buildingBottomX = BuildingX + 3;
                    buildingBottomY = BuildingY + 1;
                }
                else if (CurrentBuilding == BuildingType.AblesSisters)
                {
                    buildingTopX = BuildingX - 3;
                    buildingTopY = BuildingY - 2;
                    buildingBottomX = BuildingX + 2;
                    buildingBottomY = BuildingY + 1;
                }
                else if (CurrentBuilding == BuildingType.Airport)
                {
                    buildingTopX = BuildingX - 5;
                    buildingTopY = BuildingY - 5;
                    buildingBottomX = BuildingX + 4;
                    buildingBottomY = BuildingY + 3;
                }
                else if (CurrentBuilding == BuildingType.Campsite)
                {
                    buildingTopX = BuildingX - 2;
                    buildingTopY = BuildingY - 2;
                    buildingBottomX = BuildingX + 1;
                    buildingBottomY = BuildingY + 1;
                }
                else if (CurrentBuilding == BuildingType.Bridge || CurrentBuilding == BuildingType.Incline)
                {
                    buildingTopX = BuildingX - 1;
                    buildingTopY = BuildingY - 1;
                    buildingBottomX = BuildingX;
                    buildingBottomY = BuildingY;
                }
                else if (BuildingByte == 0xFF)
                {
                    buildingTopX = BuildingX;
                    buildingTopY = BuildingY;
                    buildingBottomX = BuildingX + 11;
                    buildingBottomY = BuildingY + 9;
                }
                else
                {
                    buildingTopX = BuildingX - 1;
                    buildingTopY = BuildingY - 1;
                    buildingBottomX = BuildingX;
                    buildingBottomY = BuildingY;
                }
            }

            if (ByteToBuildingColor.ContainsKey(BuildingByte))
                BuildingColor = ByteToBuildingColor[BuildingByte];
            else
                BuildingColor = Color.Black;

            return BuildingColor;
        }

        public void UpdateTerrain(byte[] NewTerrain, byte[] NewDesign = null)
        {
            if (NewTerrain != null)
                TerrainByte = NewTerrain;
            if (NewDesign != null)
                CustomDesignByte = NewDesign;
            BuildTerrainUnits();
        }

        public static readonly Dictionary<byte, Color> Pixel = new()
        {
                {0x00, Color.FromArgb(70, 116, 71)}, // Grass

                {0x04, Color.FromArgb(228, 216, 156)}, // Sand
                {0x05, Color.FromArgb(128, 200, 175)}, // Sea
                {0x06, Color.FromArgb(187, 121, 109)}, // Studio Bridge
                {0x07, Color.FromArgb(70, 116, 71)}, // Grass

                {0x0C, Color.FromArgb(21, 147, 229)}, // River Mouth

                {0x0F, Color.FromArgb(21, 147, 229)}, // River Mouth - River Edge

                {0x16, Color.FromArgb(187, 121, 109)}, // Studio Bridge
                {0x1D, Color.FromArgb(255, 244, 193)}, // Beach - Sea Edge

                {0x29, Color.FromArgb(109, 113, 124)}, // Walkable Rock
                {0x2A, Color.FromArgb(78, 83, 96)}, // High Rock
                {0x2B, Color.FromArgb(169, 255, 255)}, // Rock pool
                {0x2C, Color.FromArgb(144, 115, 104)}, // Beach - Grass Edge
                {0x2D, Color.FromArgb(187, 121, 109)}, // Pier
                {0x2E, Color.FromArgb(169, 255, 255)}, // Sea - Beach Edge

                {0x33, Color.FromArgb(70, 116, 71)}, // Tiny Grass - Brach Corner
                {0x46, Color.FromArgb(109, 113, 124)}, // Stacked Rock in kappn‘s Island?

                {0xF9, Color.FromArgb(179, 207, 252)}, // Walkable Ice
                {0xFA, Color.FromArgb(61, 119, 212)}, // High Ice
        };
        private enum BuildingType : byte
        {
            None = 0x0,
            PlayerHouse1 = 0x1,
            PlayerHouse2 = 0x2,
            PlayerHouse3 = 0x3,
            PlayerHouse4 = 0x4,
            PlayerHouse5 = 0x5,
            PlayerHouse6 = 0x6,
            PlayerHouse7 = 0x7,
            PlayerHouse8 = 0x8,
            VillagerHouse1 = 0x9,
            VillagerHouse2 = 0xA,
            VillagerHouse3 = 0xB,
            VillagerHouse4 = 0xC,
            VillagerHouse5 = 0xD,
            VillagerHouse6 = 0xE,
            VillagerHouse7 = 0xF,
            VillagerHouse8 = 0x10,
            VillagerHouse9 = 0x11,
            VillagerHouse10 = 0x12,
            NooksCranny = 0x13,
            ResidentServicesBuilding = 0x14,
            Museum = 0x15,
            Airport = 0x16,
            ResidentServicesTent = 0x17,
            AblesSisters = 0x18,
            Campsite = 0x19,
            Bridge = 0x1A,
            Incline = 0x1B,
            ReddsTreasureTrawler = 0x1C,
            Studio = 0x1D,
        }

        public static readonly Dictionary<byte, Color> ByteToBuildingColor = new()
        {
            {0x0, Color.White},
            {0x1, Color.RoyalBlue},
            {0x2, Color.RoyalBlue},
            {0x3, Color.RoyalBlue},
            {0x4, Color.RoyalBlue},
            {0x5, Color.RoyalBlue},
            {0x6, Color.RoyalBlue},
            {0x7, Color.RoyalBlue},
            {0x8, Color.RoyalBlue},
            {0x9, Color.Chocolate},
            {0xA, Color.Chocolate},
            {0xB, Color.Chocolate},
            {0xC, Color.Chocolate},
            {0xD, Color.Chocolate},
            {0xE, Color.Chocolate},
            {0xF, Color.Chocolate},
            {0x10, Color.Chocolate},
            {0x11, Color.Chocolate},
            {0x12, Color.Chocolate},
            {0x13, Color.Gold},
            {0x14, Color.MediumSlateBlue},
            {0x15, Color.NavajoWhite},
            {0x16, Color.IndianRed},
            {0x17, Color.DarkSeaGreen},
            {0x18, Color.DarkBlue},
            {0x19, Color.LightGoldenrodYellow},
            {0x1A, Color.Red},
            {0x1B, Color.Red},
            {0x1C, Color.OrangeRed},
            {0x1D, Color.Black},
        };
    }
}
