using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    class FloorSlot : Button
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ItemName { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Flag0 { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Flag1 { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ushort ItemID { get; set; } // flag 1 + flag 2 + itemID
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public uint ItemData { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public uint Part2 { get; set; } // FDFF0000
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public uint Part2Data { get; set; } // 01 + 00 + itemID
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public uint Part3 { get; set; } // FDFF0000
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public uint Part3Data { get; set; } // 00 + 01 + itemID
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public uint Part4 { get; set; } // FDFF0000
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public uint Part4Data { get; set; } // 01 + 01 + itemID

        private string image1Path = "";
        private string image2Path = "";
        private string image3Path = "";
        private string image4Path = "";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MapX { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MapY { get; set; }

        private readonly Image recipe;

        private string containItemPath = "";

        public bool locked = false;
        public bool corner = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ushort ItemDurability
        {
            get
            {
                return (ushort)((ItemData >> 16) & 0xFFFF);
            }
            set
            {
                ItemData = (ItemData & 0xFFFF) + ((uint)value << 16);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ushort ItemQuantity
        {
            get
            {
                return (ushort)(ItemData & 0xFFFF);
            }
            set
            {
                ItemData = (ItemData & 0xFFFF0000) + value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ushort FlowerQuantity
        {
            get
            {
                return (ushort)(ItemData & 0xFF);
            }
            set
            {
                ItemData = (ItemData & 0xFFFFFF00) + value;
            }
        }

        public FloorSlot()
        {
            if (File.Exists(Utilities.RecipeOverlayPath))
                recipe = ImageCacher.GetImage(Utilities.RecipeOverlayPath);

            ItemName = "";
            Flag0 = "00";
            Flag1 = "00";
            ItemID = 0xFFFE;
            ItemData = 0x00000000;
            MapX = -1;
            MapY = -1;
        }

        public void Setup(string Name, ushort ID, uint Data, uint P2, uint P2Data, uint P3, uint P3Data, uint P4, uint P4Data, string Path1, string Path2, string Path3, string Path4, string containPath = "", string flagA = "00", string flagB = "00")
        {
            ItemName = Name;
            ItemID = ID;
            Flag0 = flagA;
            Flag1 = flagB;
            ItemData = Data;

            Part2 = P2;
            Part2Data = P2Data;
            Part3 = P3;
            Part3Data = P3Data;
            Part4 = P4;
            Part4Data = P4Data;

            image1Path = Path1;
            image2Path = Path2;
            image3Path = Path3;
            image4Path = Path4;

            containItemPath = containPath;

            ForeColor = Color.White;
            TextAlign = ContentAlignment.TopLeft;
            Text = "";
            //Image = null;
            locked = false;

            setImage(false);
        }

        public void setImage(bool large)
        {
            Image = LoadImageForSlot(large);
        }

        public Image LoadImageForSlot(bool large)
        {
            uint P1Id = ItemData & 0x0000FFFF;
            uint P2Id = Part2Data & 0x0000FFFF;
            uint P3Id = Part3Data & 0x0000FFFF;
            uint P4Id = Part4Data & 0x0000FFFF;

            if (ItemID != 0xFFFE && (ItemID == P2Id && P2Id == P3Id && P3Id == P4Id) && (Part2 == Part3 && Part3 == Part4)) // Filled Slot
            {

                if (Flag1 != "20" || Flag0 != "00")
                {
                    locked = true;
                }
                //this.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);

                return DisplayItemImage(large, false);
            }
            else if (IsExtension())
            {
                return DisplayItemImage(large, true);
            }
            else if (ItemID != 0xFFFE && Flag0 != "00") // wrapped
            {
                return DisplayItemImage(large, false);
            }
            else // seperate
            {
                locked = true;

                if (Flag0 != "00")
                {
                    locked = true;
                }

                return DisplayItemImage(large, true);
            }
        }

        /*
        public async Task Refresh(bool large)
        {
            //this.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            uint P1Id = ItemData & 0x0000FFFF;
            uint P2Id = Part2Data & 0x0000FFFF;
            uint P3Id = Part3Data & 0x0000FFFF;
            uint P4Id = Part4Data & 0x0000FFFF;

            if (ItemID != 0xFFFE && (ItemID == P2Id && P2Id == P3Id && P3Id == P4Id) && (Part2 == Part3 && Part3 == Part4)) // Filled Slot
            {

                if (Flag1 != "20" || Flag0 != "00")
                {
                    locked = true;
                }
                //this.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);

                UpdateUI(() =>
                {
                    Image = DisplayItemImage(large, false);
                });
            }
            else if (IsExtension())
            {
                UpdateUI(() =>
                {
                    Image = DisplayItemImage(large, true);
                });
            }
            else if (ItemID == 0xFFFE && Part2 == 0xFFFE && Part3 == 0xFFFE && Part4 == 0xFFFE) // Empty
            {
                //this.BackColor = Color.LightSalmon;
            }
            else if (ItemID != 0xFFFE && Flag0 != "00") // wrapped
            {
                UpdateUI(() =>
                {
                    Image = DisplayItemImage(large, false);
                });
            }
            else // seperate
            {
                locked = true;

                if (Flag0 != "00")
                {
                    locked = true;
                }

                UpdateUI(() =>
                {
                    Image = DisplayItemImage(large, true);
                });
            }
        }
        */

        public void SetBackColor(bool Layer1 = true, int Corner1X = -1, int Corner1Y = -1, int Corner2X = -1, int Corner2Y = -1, bool AreaSelected = false)
        {
            if (Layer1)
            {
                //this.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
                BackColor = MiniMap.GetBackgroundColor(MapX, MapY);
            }
            else
            {
                //this.BackColor = Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
                BackColor = MiniMap.GetBackgroundColor(MapX, MapY, false);
            }


            if (ItemAttr.HasDurability(ItemID)) //Tools
            {
                TextAlign = ContentAlignment.BottomLeft;
                Text = "Dur: " + ItemDurability;
            }
            else if (ItemAttr.HasUse(ItemID)) // Food/Drink
            {
                TextAlign = ContentAlignment.BottomLeft;
                Text = "Use: " + ItemDurability;
            }
            else if (ItemAttr.IsFlower(ItemID)) //Flowers
            {
                TextAlign = ContentAlignment.BottomRight;
                ForeColor = Color.Yellow;
                Text = (FlowerQuantity + 1).ToString();
            }
            else if (ItemAttr.HasQuantity(ItemID)) // Materials
            {
                TextAlign = ContentAlignment.BottomRight;
                Text = (ItemQuantity + 1).ToString();
            }
            else if (ItemAttr.HasGenetics(ItemID))
            {
                if (ItemData.ToString("X").Contains("83E0") || (ItemData.ToString("X").Contains("8642"))) // Flower
                {
                    TextAlign = ContentAlignment.TopRight;
                    Text = "★";
                }
                else
                {
                    TextAlign = ContentAlignment.TopRight;
                    string value = ItemData.ToString("X");
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
                    Text = HexToBinary(secondByte) + "-" + HexToBinary(firstByte);
                    //System.Diagnostics.Debug.Print(secondByte + " " + firstByte + " " + HexToBinary(secondByte) + " " + HexToBinary(firstByte));
                }
            }

            if (Flag0 != "00") //Wrapped
            {
                if (ItemID == 0x16A1) //Inside Bottle
                {
                    BackColor = Color.LightSalmon;
                }
                else if (ItemID == 0x16A2) // Recipe
                {
                    BackColor = Color.LightSalmon;
                }
                else
                {
                    BackColor = Color.LightSalmon;
                }
            }
            else if (Flag1 == "04" || Flag1 == "24") //Bury
            {
                BackColor = Color.DarkKhaki;
            }
            else if (Flag1 == "20")
            {
                BackColor = Color.Green;
            }
            else if (IsExtension())
            {
                //this.BackColor = Color.Gray;
            }
            else if (locked)
            {
                //this.BackColor = Color.Gray;
            }

            if (Corner1X == MapX && Corner1Y == MapY)
            {
                BackColor = Color.DeepPink;
            }
            else if (Corner2X == MapX && Corner2Y == MapY)
            {
                BackColor = Color.HotPink;
            }
            else if (InRange(Corner1X, Corner1Y, Corner2X, Corner2Y))
            {
                if (AreaSelected)
                    BackColor = Color.Crimson;
                else
                    BackColor = Color.Pink;
            }
        }

        private bool InRange(int Corner1X, int Corner1Y, int Corner2X, int Corner2Y)
        {
            if (Corner1X < 0 || Corner1Y < 0 || Corner2X < 0 || Corner2Y < 0)
                return false;
            else
            {
                if (Corner1X <= Corner2X)
                {
                    if (Corner1Y <= Corner2Y) // Top Left
                    {
                        if (Corner1X <= MapX && Corner2X >= MapX && Corner1Y <= MapY && Corner2Y >= MapY)
                            return true;
                        else
                            return false;
                    }
                    else // Bottom Left
                    {
                        if (Corner1X <= MapX && Corner2X >= MapX && Corner2Y <= MapY && Corner1Y >= MapY)
                            return true;
                        else
                            return false;
                    }
                }
                else
                {
                    if (Corner1Y <= Corner2Y) // Top Right
                    {
                        if (Corner2X <= MapX && Corner1X >= MapX && Corner1Y <= MapY && Corner2Y >= MapY)
                            return true;
                        else
                            return false;
                    }
                    else // Bottom Left
                    {
                        if (Corner2X <= MapX && Corner1X >= MapX && Corner2Y <= MapY && Corner1Y >= MapY)
                            return true;
                        else
                            return false;
                    }
                }
            }
        }

        public bool IsExtension()
        {
            uint P1Id = ItemData & 0x0000FFFF;
            uint P2Id = Part2Data & 0x0000FFFF;
            uint P3Id = Part3Data & 0x0000FFFF;
            uint P4Id = Part4Data & 0x0000FFFF;

            if (ItemID == 0xFFFD && Part2 == 0xFFFD && Part3 == 0xFFFD && Part4 == 0xFFFD && P1Id == P2Id && P2Id == P3Id && P3Id == P4Id)
                return true;
            else
                return false;
        }

        public void Reset()
        {
            ItemName = "";
            Flag0 = "00";
            Flag1 = "00";
            ItemID = 0xFFFE;
            ItemData = 0x00000000;
            //mapX = -1;
            //mapY = -1;

            Part2 = 0x0000FFFE;
            Part2Data = 0x00000000;
            Part3 = 0x0000FFFE;
            Part3Data = 0x00000000;
            Part4 = 0x0000FFFE;
            Part4Data = 0x00000000;

            image1Path = "";
            image2Path = "";
            image3Path = "";
            image4Path = "";

            containItemPath = "";
            Image = null;
        }

        public Image DisplayItemImage(bool large, bool separate)
        {
            if (separate)
            {
                Size size;

                size = new Size(64, 64);

                Image background = new Bitmap(75, 75);
                Image topleft = null;
                Image topright = null;
                Image bottomleft = null;
                Image bottomright = null;

                uint P1Id = ItemData & 0x0000FFFF;
                uint P2Id = Part2Data & 0x0000FFFF;
                uint P3Id = Part3Data & 0x0000FFFF;
                uint P4Id = Part4Data & 0x0000FFFF;

                if (image1Path != "")
                {
                    if (ItemID == 0xFFFD)
                        topleft = (new Bitmap(ImageCacher.GetImage(image1Path), new Size((Width) / 3, (Height) / 3)));
                    else
                        topleft = (new Bitmap(ImageCacher.GetImage(image1Path), new Size((Width) / 2, (Height) / 2)));
                }
                else if (ItemID != 0xFFFE)
                {
                    if (P1Id == 0x16A2)
                        topleft = (new Bitmap(recipe, new Size((Width) / 3, (Height) / 3)));
                    else
                        topleft = (new Bitmap(Properties.Resources.Leaf, new Size((Width) / 2, (Height) / 2)));
                }

                if (image2Path != "")
                {
                    if (Part2 == 0x0000FFFD)
                        bottomleft = (new Bitmap(ImageCacher.GetImage(image2Path), new Size((Width) / 3, (Height) / 3)));
                    else
                        bottomleft = (new Bitmap(ImageCacher.GetImage(image2Path), new Size((Width) / 2, (Height) / 2)));
                }
                else if (Part2 != 0x0000FFFE)
                {
                    if (P2Id == 0x16A2)
                        bottomleft = (new Bitmap(recipe, new Size((Width) / 3, (Height) / 3)));
                    else
                        bottomleft = (new Bitmap(Properties.Resources.Leaf, new Size((Width) / 2, (Height) / 2)));
                }

                if (image3Path != "")
                {
                    if (Part3 == 0x0000FFFD)
                        topright = (new Bitmap(ImageCacher.GetImage(image3Path), new Size((Width) / 3, (Height) / 3)));
                    else
                        topright = (new Bitmap(ImageCacher.GetImage(image3Path), new Size((Width) / 2, (Height) / 2)));
                }
                else if (Part3 != 0x0000FFFE)
                {
                    if (P3Id == 0x16A2)
                        topright = (new Bitmap(recipe, new Size((Width) / 3, (Height) / 3)));
                    else
                        topright = (new Bitmap(Properties.Resources.Leaf, new Size((Width) / 2, (Height) / 2)));
                }

                if (image4Path != "")
                {
                    if (Part4 == 0x0000FFFD)
                        bottomright = (new Bitmap(ImageCacher.GetImage(image4Path), new Size((Width) / 3, (Height) / 3)));
                    else
                        bottomright = (new Bitmap(ImageCacher.GetImage(image4Path), new Size((Width) / 2, (Height) / 2)));
                }
                else if (Part4 != 0x0000FFFE)
                {
                    if (P4Id == 0x16A2)
                        bottomright = (new Bitmap(recipe, new Size((Width) / 3, (Height) / 3)));
                    else
                        bottomright = (new Bitmap(Properties.Resources.Leaf, new Size((Width) / 2, (Height) / 2)));
                }

                Image img = PlaceImages(background, topleft, topright, bottomleft, bottomright, 1);
                return new Bitmap(img, size);
            }
            else
            {
                Size size;
                double recipeMultiplier;
                double wallMultiplier;

                if (large)
                {
                    size = new Size(128, 128);
                    recipeMultiplier = 0.3;
                    wallMultiplier = 0.45;
                }
                else
                {
                    size = new Size(64, 64);
                    recipeMultiplier = 0.5;
                    wallMultiplier = 0.6;
                }

                if (image1Path == "" & ItemID != 0xFFFE)
                {
                    return new Bitmap(Properties.Resources.Leaf, size);
                }
                else if (ItemID == 0x16A2) // recipe
                {
                    Image background = new Bitmap(ImageCacher.GetImage(image1Path));
                    int imageSize = (int)(background.Width * recipeMultiplier);
                    Image icon = (new Bitmap(recipe, new Size(imageSize, imageSize)));

                    Image img = PlaceImageOverImage(background, icon, background.Width - (imageSize - 5), background.Width - (imageSize - 5), 1);
                    return new Bitmap(img, size);
                }
                else if (ItemID == 0x315A || ItemID == 0x1618 || ItemID == 0x342F || ItemID == 0x114A || ItemID == 0xEC9C) // Wall-Mount/Money Tree
                {
                    if (File.Exists(containItemPath))
                    {
                        Image background = new Bitmap(ImageCacher.GetImage(image1Path));
                        int imageSize = (int)(background.Width * wallMultiplier);
                        Image icon = (new Bitmap(ImageCacher.GetImage(containItemPath), new Size(imageSize, imageSize)));

                        Image img = PlaceImageOverImage(background, icon, background.Width - (imageSize - 5), background.Width - (imageSize - 5), 1);
                        return new Bitmap(img, size);
                    }
                    else
                    {
                        Image img = ImageCacher.GetImage(image1Path);
                        return new Bitmap(img, size);
                    }
                }
                else if (image1Path != "")
                {
                    Image img = ImageCacher.GetImage(image1Path);
                    return new Bitmap(img, size);
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsEmpty()
        {
            if (ItemID == 0xFFFE && Part2 == 0xFFFE && Part3 == 0xFFFE && Part4 == 0xFFFE)
            {
                return true;
            }
            return false;
        }
        public static string HexToBinary(string hex)
        {
            return hexCharacterToBinary[hex.ToLower()];
        }

        private static readonly Dictionary<string, string> hexCharacterToBinary = new()
        {
            { "0", "0-0" },
            { "1", "1-0" },
            { "2", "X-0" },//
            { "3", "2-0" },
            { "4", "0-1" },
            { "5", "1-1" },
            { "6", "X-1" },//
            { "7", "2-1" },
            { "8", "0-X" },//
            { "9", "1-X" },//
            { "a", "X-X" },//
            { "b", "2-X" },//
            { "c", "0-2" },
            { "d", "1-2" },
            { "e", "X-2" },//
            { "f", "2-2" }
        };

        private static Image PlaceImageOverImage(Image bottom, Image top, int x, int y, float alpha)
        {
            Image output = bottom;
            using (Graphics graphics = Graphics.FromImage(output))
            {
                var cm = new ColorMatrix
                {
                    Matrix33 = alpha
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(top, new Rectangle(x, y, top.Width, top.Height), 0, 0, top.Width, top.Height, GraphicsUnit.Pixel, ia);
            }

            return output;
        }

        private Image PlaceImages(Image bottom, Image topleft, Image topright, Image bottomleft, Image bottomright, float alpha)
        {
            Image output = bottom;
            using (Graphics graphics = Graphics.FromImage(output))
            {
                var cm = new ColorMatrix
                {
                    Matrix33 = alpha
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                int tinyOffset;

                if (topleft != null)
                {
                    if (topleft.Width >= Width / 2)
                        tinyOffset = 0;
                    else
                        tinyOffset = 6;

                    graphics.DrawImage(topleft, new Rectangle(0 + tinyOffset, 0 + tinyOffset, topleft.Width, topleft.Height), 0, 0, topleft.Width, topleft.Height, GraphicsUnit.Pixel, ia);
                }
                if (topright != null)
                {
                    if (topright.Width >= Width / 2)
                        tinyOffset = 0;
                    else
                        tinyOffset = 6;

                    graphics.DrawImage(topright, new Rectangle(38 + tinyOffset, 0 + tinyOffset, topright.Width, topright.Height), 0, 0, topright.Width, topright.Height, GraphicsUnit.Pixel, ia);
                }
                if (bottomleft != null)
                {
                    if (bottomleft.Width >= Width / 2)
                        tinyOffset = 0;
                    else
                        tinyOffset = 6;

                    graphics.DrawImage(bottomleft, new Rectangle(0 + tinyOffset, 38 + tinyOffset, bottomleft.Width, bottomleft.Height), 0, 0, bottomleft.Width, bottomleft.Height, GraphicsUnit.Pixel, ia);
                }
                if (bottomright != null)
                {
                    if (bottomright.Width >= Width / 2)
                        tinyOffset = 0;
                    else
                        tinyOffset = 6;

                    graphics.DrawImage(bottomright, new Rectangle(38 + tinyOffset, 38 + tinyOffset, bottomright.Width, bottomright.Height), 0, 0, bottomright.Width, bottomright.Height, GraphicsUnit.Pixel, ia);
                }
            }

            return output;
        }

        /*
        private async static Task<Image> PlaceImagesAsync(Image bottom, Image topleft, Image topright, Image bottomleft, Image bottomright, float alpha)
        {
            Image output = bottom;

            await Task.Run(() =>
            {
                using Graphics graphics = Graphics.FromImage(output);
                lock (syncRoot)
                {
                    var cm = new ColorMatrix
                    {
                        Matrix33 = alpha
                    };

                    var ia = new ImageAttributes();
                    ia.SetColorMatrix(cm);

                    if (topleft != null)
                        graphics.DrawImage(topleft, new Rectangle(0, 0, topleft.Width, topleft.Height), 0, 0, topleft.Width, topleft.Height, GraphicsUnit.Pixel, ia);
                    if (topright != null)
                        graphics.DrawImage(topright, new Rectangle(38, 0, topright.Width, topright.Height), 0, 0, topright.Width, topright.Height, GraphicsUnit.Pixel, ia);
                    if (bottomleft != null)
                        graphics.DrawImage(bottomleft, new Rectangle(0, 38, bottomleft.Width, bottomleft.Height), 0, 0, bottomleft.Width, bottomleft.Height, GraphicsUnit.Pixel, ia);
                    if (bottomright != null)
                        graphics.DrawImage(bottomright, new Rectangle(38, 38, bottomright.Width, bottomright.Height), 0, 0, bottomright.Width, bottomright.Height, GraphicsUnit.Pixel, ia);
                }
            });

            return output;
        }
        */
    }
}
