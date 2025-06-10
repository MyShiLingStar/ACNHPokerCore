using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ACNHPokerCore
{
    public class InventorySlot : System.Windows.Forms.Button
    {
        private string itemName;
        private UInt16 itemID = 0xFFFE;
        private UInt32 itemData = 0x0;
        private string imagePath = "";
        private bool hide = false;
        private string flag0 = "00";
        private string flag1 = "00";

        private int mapX = -1;
        private int mapY = -1;

        private Image recipe = null;

        private string containItemPath = "";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UInt16 ItemDurability
        {
            get
            {
                return (UInt16)((itemData >> 16) & 0xFFFF);
            }
            set
            {
                itemData = (itemData & 0xFFFF) + ((UInt32)value << 16);
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UInt16 ItemQuantity
        {
            get
            {
                return (UInt16)(itemData & 0xFFFF);
            }
            set
            {
                itemData = (itemData & 0xFFFF0000) + value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UInt16 FlowerQuantity
        {
            get
            {
                return (UInt16)(itemData & 0xFF);
            }
            set
            {
                itemData = (itemData & 0xFFFFFF00) + value;
            }
        }

        public InventorySlot()
        {

        }

        public string DisplayItemID()
        {
            return "0x" + String.Format("{0:X4}", itemID);
        }

        public string FillItemID()
        {
            return itemID.ToString("X");
        }

        public string DisplayItemData()
        {
            return "0x" + itemData.ToString("X");
        }
        public string FillItemData()
        {
            return itemData.ToString("X");
        }

        public string DisplayItemName()
        {
            return itemName;
        }

        public string GetPath()
        {
            return imagePath;
        }

        public string GetiName()
        {

            string[] firstSplit = imagePath.Split('.');
            string[] SecondSplit = firstSplit[0].Split('\\');

            return SecondSplit[SecondSplit.Length - 1];
        }

        public Boolean IsEmpty()
        {
            if (itemID == 0x0 || itemID == 0xFFFE)
            {
                return true;
            }
            return false;
        }

        public Image DisplayItemImage(Boolean large)
        {
            if (recipe == null)
            {
                if (File.Exists(Utilities.RecipeOverlayPath))
                    recipe = ImageCacher.GetImage(Utilities.RecipeOverlayPath);
            }

            if (large)
            {
                if (imagePath == "" & itemID != 0xFFFE)
                {
                    return new Bitmap(Properties.Resources.Leaf, new Size(128, 128));
                }
                else if (itemID == 0x16A2) //recipe
                {
                    Image background = new Bitmap(ImageCacher.GetImage(imagePath));
                    int imageSize = (int)(background.Width * 0.3);

                    if (recipe != null)
                    {
                        Image icon = (new Bitmap(recipe, new Size(imageSize, imageSize)));

                        Image img = PlaceImageOverImage(background, icon, background.Width - imageSize - 10, background.Width - imageSize - 10, 1);

                        return new Bitmap(img, new Size(128, 128));
                    }
                    else
                        return new Bitmap(background, new Size(128, 128));
                }
                else if (itemID == 0x315A || itemID == 0x1618 || itemID == 0x342F || itemID == 0x114A || itemID == 0xEC9C) // Wall-Mount/Money Tree
                {
                    if (File.Exists(containItemPath))
                    {
                        Image background = new Bitmap(ImageCacher.GetImage(imagePath));
                        int imageSize = (int)(background.Width * 0.45);
                        Image icon = (new Bitmap(ImageCacher.GetImage(containItemPath), new Size(imageSize, imageSize)));

                        Image img = PlaceImageOverImage(background, icon, background.Width - imageSize - 10, background.Width - imageSize - 10, 1);

                        return new Bitmap(img, new Size(128, 128));
                    }
                    else
                    {
                        Image img = ImageCacher.GetImage(imagePath);
                        return new Bitmap(img, new Size(128, 128));
                    }
                }
                else if (imagePath != "")
                {
                    Image img = ImageCacher.GetImage(imagePath);
                    return new Bitmap(img, new Size(128, 128));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (imagePath == "" & itemID != 0xFFFE)
                {
                    return new Bitmap(Properties.Resources.Leaf, new Size(64, 64));
                }
                else if (itemID == 0x315A || itemID == 0x1618 || itemID == 0x342F || itemID == 0x114A || itemID == 0xEC9C) // Wall-Mount/Money Tree
                {
                    if (File.Exists(containItemPath))
                    {
                        Image background = new Bitmap(ImageCacher.GetImage(imagePath));
                        int imageSize = (int)(background.Width * 0.6);
                        Image icon = (new Bitmap(ImageCacher.GetImage(containItemPath), new Size(imageSize, imageSize)));

                        Image img = PlaceImageOverImage(background, icon, background.Width - imageSize, background.Width - imageSize, 1);

                        return new Bitmap(img, new Size(64, 64));
                    }
                    else
                    {
                        Image img = ImageCacher.GetImage(imagePath);
                        return new Bitmap(img, new Size(64, 64));
                    }
                }
                else if (itemID == 0x16A2) // recipe
                {
                    Image background = new Bitmap(ImageCacher.GetImage(imagePath));
                    int imageSize = (int)(background.Width * 0.35);
                    if (recipe != null)
                    {
                        Image icon = (new Bitmap(recipe, new Size(imageSize, imageSize)));

                        Image img = PlaceImageOverImage(background, icon, background.Width - imageSize, background.Width - imageSize, 1);

                        return new Bitmap(img, new Size(64, 64));
                    }
                    else
                        return new Bitmap(background, new Size(64, 64));

                }
                else if (imagePath != "")
                {
                    Image img = ImageCacher.GetImage(imagePath);
                    return new Bitmap(img, new Size(64, 64));

                }
                else
                {
                    return null;
                }
            }
        }

        public void SetFlag0(string flag)
        {
            flag0 = flag;
        }
        public void SetFlag1(string flag)
        {
            flag1 = flag;
        }
        public string GetFlag0() // Wrapping
        {
            return flag0;
        }
        public string GetFlag1() // Direction ?
        {
            return flag1;
        }
        public void SetmapX(int x)
        {
            mapX = x;
        }
        public void SetmapY(int y)
        {
            mapY = y;
        }
        public int GetmapX()
        {
            return mapX;
        }
        public int GetmapY()
        {
            return mapY;
        }

        public void SetHide(Boolean flag)
        {
            hide = flag;
        }
        public string GetContainItemPath()
        {
            return containItemPath;
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
            /*
            { "0", "0000" },
            { "1", "0001" },
            { "2", "0010" },
            { "3", "0011" },
            { "4", "0100" },
            { "5", "0101" },
            { "6", "0110" },
            { "7", "0111" },
            { "8", "1000" },
            { "9", "1001" },
            { "a", "1010" },
            { "b", "1011" },
            { "c", "1100" },
            { "d", "1101" },
            { "e", "1110" },
            { "f", "1111" }
            */
        };

        public static string HexToBinary(string hex)
        {
            return hexCharacterToBinary[hex.ToLower()];
        }

        public void Reset()
        {
            itemName = "";
            itemID = 0xFFFE;
            flag0 = "00";
            flag1 = "00";
            itemData = 0x0;
            imagePath = "";
            hide = false;
            containItemPath = "";
            Image = null;
            Text = "";
            //init = false;
        }

        public void Setup(string Name, UInt16 ID, UInt32 Data, string Path, string containPath = "", string flagA = "00", string flagB = "00")
        {
            itemName = Name;
            itemID = ID;
            flag0 = flagA;
            flag1 = flagB;
            itemData = Data;
            imagePath = Path;
            containItemPath = containPath;
            Refresh(false);
            //init = true;
        }

        public void Setup(string Name, UInt16 ID, UInt32 Data, string Path, Boolean large, string containPath = "", string flagA = "00", string flagB = "00")
        {
            itemName = Name;
            itemID = ID;
            flag0 = flagA;
            flag1 = flagB;
            itemData = Data;
            imagePath = Path;
            containItemPath = containPath;
            Refresh(large);
            //init = true;
        }

        public void Setup(InventorySlot btn)
        {
            itemName = btn.itemName;
            itemID = btn.itemID;
            flag0 = btn.flag0;
            flag1 = btn.flag1;
            itemData = btn.itemData;
            imagePath = btn.imagePath;
            containItemPath = btn.containItemPath;
            Refresh(true);
            //init = true;
        }

        public void Refresh(Boolean large)
        {
            ForeColor = Color.White;
            TextAlign = ContentAlignment.TopLeft;
            Text = "";

            if (itemID != 0xFFFE) //Empty
            {
                Font = new Font("Arial", 10F, FontStyle.Bold);
                Image = DisplayItemImage(large);
                if (hide)
                {
                }
                else if (flag0 != "00") //Wrapped
                {
                    if (itemID == 0x16A1) //Inside Bottle
                    {
                        Text = @"Bottle";
                        ForeColor = Color.LightGreen;
                        TextAlign = ContentAlignment.TopRight;
                    }
                    else if (itemID == 0x16A2) // Recipe
                    {
                        Text = @"Wrap";
                        ForeColor = Color.LightSalmon;
                    }
                    else
                    {
                        Text = @"Wrap";
                        ForeColor = Color.LightSalmon;
                    }
                }
                else if (ItemAttr.HasDurability(itemID)) //Tools
                {
                    TextAlign = ContentAlignment.BottomLeft;
                    Text = @"Dur: " + ItemDurability;
                }
                else if (ItemAttr.HasUse(itemID)) // Food/Drink
                {
                    TextAlign = ContentAlignment.BottomLeft;
                    Text = @"Use: " + ItemDurability;
                }
                else if (ItemAttr.IsFlower(itemID)) //Flowers
                {
                    TextAlign = ContentAlignment.BottomRight;
                    ForeColor = Color.Yellow;
                    Text = (FlowerQuantity + 1).ToString();
                }
                else if (ItemAttr.HasQuantity(itemID)) // Materials
                {
                    TextAlign = ContentAlignment.BottomRight;
                    Text = (ItemQuantity + 1).ToString();
                }
                else if (ItemAttr.HasGenetics(itemID))
                {
                    if (DisplayItemData().Contains("83E0") || DisplayItemData().Contains("8642")) // Flower
                    {
                        TextAlign = ContentAlignment.TopRight;
                        Text = @"★";
                        return;
                    }

                    TextAlign = ContentAlignment.TopRight;
                    string value = itemData.ToString("X");
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
                else if (itemID == 0x16A2) // Recipe
                {
                    Text = "";
                }
                else if (itemID == 0x1095) // Villager Delivery
                {
                    Text = @"Delivery";
                    ForeColor = Color.Red;
                    TextAlign = ContentAlignment.TopRight;
                }
                else if (itemID == 0x0A13) // Fossil
                {
                    Text = @"Fossil";
                    ForeColor = Color.Blue;
                    TextAlign = ContentAlignment.TopRight;
                }
                else if (itemID == 0x114A || itemID == 0xEC9C) // Money Tree
                {
                    Font = new Font("Arial", 8F, FontStyle.Bold);
                    string containItemName = Main.GetNameFromID(Utilities.PrecedingZeros(FillItemData(), 8).Substring(4, 4));
                    if (containItemName.Length > 10)
                    {
                        Text = string.Concat(containItemName.AsSpan(0, 8), "...");
                    }
                    else
                    {
                        Text = containItemName;
                    }
                    ForeColor = Color.White;
                    TextAlign = ContentAlignment.BottomLeft;
                }
                else if (itemID == 0x315A || itemID == 0x1618 || itemID == 0x342F) // Wall-Mounted
                {

                }
                else if (itemData > 0)
                {
                    ForeColor = Color.Gold;
                    TextAlign = ContentAlignment.BottomRight;
                    Text = @"Var: " + itemData.ToString("X");
                }
            }
            else
            {
                Image = null;
                Text = "";
            }
        }

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
    }
}
