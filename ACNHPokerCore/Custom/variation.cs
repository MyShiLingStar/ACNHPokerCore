using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class variation : Form
    {
        private static DataTable itemSource;
        private DataGridViewRow lastRow;
        private inventorySlot[,] selection;
        private int lengthX = 0;
        private int lengthY = 0;

        public event ReceiveVariationHandler sendVariationData;

        public variation(int height = 265)
        {
            InitializeComponent();
            this.Size = new Size(this.Width, height);
        }

        private static DataTable loadItemCSV(string filePath)
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

            if (dt.Columns.Contains("id"))
                dt.PrimaryKey = new DataColumn[1] { dt.Columns["id"] };

            return dt;
        }

        private string removeSpace(string input)
        {
            return input.Replace(" ", String.Empty);
        }

        protected override Point ScrollToControl(Control activeControl)
        {
            return this.AutoScrollPosition;
        }

        public string GetImagePathFromID(string itemID, DataTable source)
        {
            DataRow row = source.Rows.Find(itemID);

            if (row == null)
            {
                return ""; //row not found
            }
            else
            {
                string path = Utilities.imagePath + row["iName"] + "_Remake_0_0" + ".png";
                if (File.Exists(path))
                {
                    return path;
                }

                //row found set the index and find the file
                path = Utilities.imagePath + row["iName"] + ".png";
                if (File.Exists(path))
                {
                    return path;
                }

                return "";
            }

        }

        private void Variation_Load(object sender, EventArgs e)
        {
            if (File.Exists(Utilities.variationPath))
            {
                itemSource = loadItemCSV(Utilities.variationPath);

                /*
                furnitureGridView.DataSource = loadItemCSV(variationPath);
                //furnitureGridView.Columns["ID"].Visible = false;
                //furnitureGridView.Columns["iName"].Visible = false;
                furnitureGridView.Columns[0].Width = 150;
                furnitureGridView.Columns[1].Width = 110;

                furnitureGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                furnitureGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
                furnitureGridView.DefaultCellStyle.ForeColor = Color.FromArgb(255, 114, 105, 110);
                furnitureGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                furnitureGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 57, 60, 67);
                furnitureGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                furnitureGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

                furnitureGridView.EnableHeadersVisualStyles = false;

                furnitureGridView.Font = new Font("Arial", 10);

                DataGridViewImageColumn furnitureImageColumn = new DataGridViewImageColumn
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                furnitureGridView.Columns.Insert(3, furnitureImageColumn);
                furnitureImageColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

                foreach (DataGridViewColumn c in furnitureGridView.Columns)
                {
                    c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    c.HeaderCell.Style.Font = new Font("Arial", 9, FontStyle.Bold);
                }
                */
            }
        }

        private void furnitureGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            /*
            if (e.RowIndex >= 0 && e.RowIndex < this.furnitureGridView.Rows.Count)
            {
                if (e.ColumnIndex == 3)
                {
                    string path = imgPath + furnitureGridView.Rows[e.RowIndex].Cells[1].Value.ToString() + @"\" + removeSpace(furnitureGridView.Rows[e.RowIndex].Cells[4].Value.ToString()) + ".png";
                    if (File.Exists(path))
                    {
                        Image img = Image.FromFile(path);
                        e.Value = img;
                        return;
                    }

                    path = imgPath + furnitureGridView.Rows[e.RowIndex].Cells[1].Value.ToString() + @"\" + removeSpace(furnitureGridView.Rows[e.RowIndex].Cells[4].Value.ToString()) + "_0_0" + ".png";
                    if (File.Exists(path))
                    {
                        Image img = Image.FromFile(path);
                        e.Value = img;
                        return;
                    }

                }
            }
            */
        }

        private void furnitureGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (lastRow != null)
            {
                lastRow.Height = 22;
            }

            if (e.RowIndex > -1)
            {
                lastRow = furnitureGridView.Rows[e.RowIndex];
                furnitureGridView.Rows[e.RowIndex].Height = 160;
                /*
                customIdTextbox.Text = itemGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
                if (hexModeBtn.Tag.ToString() == "Normal")
                {
                    if (customAmountTxt.Text == "" || customAmountTxt.Text == "0")
                    {
                        customAmountTxt.Text = "1";
                    }
                }
                else
                {
                    hexMode_Click(sender, e);
                    customAmountTxt.Text = "1";
                }
                */
                //Debug.Print(itemGridView.Rows[e.RowIndex].Cells[2].Value.ToString());

                string name = furnitureGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
                string idString = furnitureGridView.Rows[e.RowIndex].Cells[2].Value.ToString();
                UInt16 id = Convert.ToUInt16("0x" + idString, 16);
                UInt16 data = 0x0;
                string category = furnitureGridView.Rows[e.RowIndex].Cells[1].Value.ToString();
                string iName = furnitureGridView.Rows[e.RowIndex].Cells[4].Value.ToString();
                string path = GetImagePathFromID(idString, itemSource);

                selectedItem.setup(name, id, data, path, true);
                //updateSelectedItemInfo(selectedItem.displayItemName(), selectedItem.displayItemID(), selectedItem.displayItemData());
            }
        }

        private void showVariation(string name, UInt16 id, int main, int sub, string iName, string value)
        {
            selection = new inventorySlot[main + 1, sub + 1];

            for (int j = 0; j <= main; j++)
            {
                for (int k = 0; k <= sub; k++)
                {
                    string path = Utilities.imagePath + iName + "_Remake_" + j.ToString() + "_" + k.ToString() + ".png";

                    selection[j, k] = new inventorySlot();
                    selection[j, k].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
                    selection[j, k].FlatAppearance.BorderSize = 0;
                    selection[j, k].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    if (sub > main)
                    {
                        selection[j, k].Location = new System.Drawing.Point(5 + (k * 82), 5 + (j * 82));
                    }
                    else
                    {
                        selection[j, k].Location = new System.Drawing.Point(5 + (j * 82), 5 + (k * 82));
                    }
                    selection[j, k].Margin = new System.Windows.Forms.Padding(0);
                    selection[j, k].Size = new System.Drawing.Size(80, 80);
                    selection[j, k].Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
                    //selection[j, k].setHide(true);

                    if (ItemAttr.hasFenceWithVariation(id)) // Fence with Variation
                    {
                        string front = Utilities.precedingZeros((j + (0x20 * k)).ToString("X"), 4);
                        string back = Utilities.turn2bytes(value);
                        uint newValue = Convert.ToUInt32(front + back, 16);
                        selection[j, k].setup(name, id, newValue, path, true);
                    }
                    else
                    {
                        selection[j, k].setup(name, id, (uint)(j + (0x20 * k)), path, true);
                    }
                    selection[j, k].MouseDown += new System.Windows.Forms.MouseEventHandler(this.variation_MouseClick);
                    this.Controls.Add(selection[j, k]);

                    //            this.selectedItem.MouseClick += new System.Windows.Forms.MouseEventHandler(this.selectedItem_MouseClick);
                }
            }
            //Debug.Print(selection.GetLength(0).ToString());
            //Debug.Print(selection.GetLength(1).ToString());
            this.lengthX = selection.GetLength(0);
            this.lengthY = selection.GetLength(1);
        }

        private void variation_MouseClick(object sender, MouseEventArgs e)
        {
            var button = (inventorySlot)sender;

            MouseEventArgs me = (MouseEventArgs)e;

            if (me.Button == System.Windows.Forms.MouseButtons.Left)
            {

                foreach (inventorySlot btn in this.Controls.OfType<inventorySlot>())
                {
                    btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
                }

                button.BackColor = System.Drawing.Color.LightSeaGreen;

                this.sendVariationData((inventorySlot)sender, 0);
            }
            else
            {
                foreach (inventorySlot btn in this.Controls.OfType<inventorySlot>())
                {
                    btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
                }

                button.BackColor = System.Drawing.Color.Orange;

                this.sendVariationData((inventorySlot)sender, 1);
            }

        }

        private void removeVariation()
        {
            for (int j = 0; j < this.lengthX; j++)
            {
                for (int k = 0; k < this.lengthY; k++)
                {
                    this.Controls.Remove(selection[j, k]);
                }
            }
        }

        /*
            this.selectedItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.selectedItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.selectedItem.FlatAppearance.BorderSize = 0;
            this.selectedItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectedItem.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.selectedItem.ForeColor = System.Drawing.Color.White;
            this.selectedItem.Location = new System.Drawing.Point(9, 9);
            this.selectedItem.Margin = new System.Windows.Forms.Padding(0);
            this.selectedItem.Name = "selectedItem";
            this.selectedItem.Size = new System.Drawing.Size(128, 128);
            this.selectedItem.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.selectedItem.UseVisualStyleBackColor = false;
         */

        private static int findMaxVariation(string name)
        {
            for (int i = 9; i >= 0; i--)
            {
                string path = Utilities.imagePath + name + "_Remake_" + i.ToString() + "_0" + ".png";
                if (File.Exists(path))
                {
                    return i;
                }
            }
            return -1;
        }
        private static int findMaxSubVariation(string name)
        {
            for (int i = 9; i >= 0; i--)
            {
                string path = Utilities.imagePath + name + "_Remake_0_" + i.ToString() + ".png";
                if (File.Exists(path))
                {
                    return i;
                }
            }
            return -1;
        }

        public void receiveID(string id, string language, string value = "00000000")
        {
            removeVariation();
            this.itemIDLabel.Text = id;
            DataRow row = GetRowFromID(id);
            if (row != null)
            {
                this.infoLabel.Text = "";
                string name = row[language].ToString();
                //string idString = row["id"].ToString();
                UInt16 itemID = Convert.ToUInt16("0x" + row["id"].ToString(), 16);
                //UInt16 data = 0x0;
                //string category = row[1].ToString();
                string iName = row["iName"].ToString();
                //string path = GetImagePathFromID(idString, itemSource);

                //updateSelectedItemInfo(selectedItem.displayItemName(), selectedItem.displayItemID(), selectedItem.displayItemData());
                //Debug.Print(row[0].ToString() + " " + row[1].ToString() + " " + row[2].ToString() + " " + row[3].ToString() + " ");
                int MaxVariation = findMaxVariation(iName);
                int MaxSubxVariation = findMaxSubVariation(iName);

                if (MaxVariation >= 0 && MaxSubxVariation >= 0)
                    showVariation(name, itemID, findMaxVariation(iName), findMaxSubVariation(iName), iName, value);
                else
                    this.infoLabel.Text = "Did you forget the image pack?";
            }
            else
            {
                this.infoLabel.Text = "No variation found.";
            }
        }

        public static DataRow GetRowFromID(string id)
        {
            if (itemSource == null)
            {
                if (File.Exists(Utilities.variationPath))
                    itemSource = loadItemCSV(Utilities.variationPath);
                if (itemSource == null)
                    return null;
            }
            DataRow row = itemSource.Rows.Find(id);

            return row;
        }

        public static inventorySlot[,] getVariationList(string id, string flag1 = "00", string flag2 = "00", string value = "00000000", string language = "eng")
        {
            DataRow row = GetRowFromID(id);
            if (row != null)
            {
                UInt16 itemID = Convert.ToUInt16("0x" + row["id"].ToString(), 16);
                string iName = row["iName"].ToString();

                string name = row[language].ToString();
                int main = findMaxVariation(iName);
                int sub = findMaxSubVariation(iName);

                if (main >= 0 && sub >= 0)
                {
                    inventorySlot[,] variationList = new inventorySlot[main + 1, sub + 1];

                    for (int j = 0; j <= main; j++)
                    {
                        for (int k = 0; k <= sub; k++)
                        {
                            variationList[j, k] = new inventorySlot();

                            string path = Utilities.imagePath + iName + "_Remake_" + j.ToString() + "_" + k.ToString() + ".png";

                            if (ItemAttr.hasFenceWithVariation(itemID)) // Fence with Variation
                            {
                                string front = Utilities.precedingZeros((j + (0x20 * k)).ToString("X"), 4);
                                string back = Utilities.turn2bytes(value);
                                uint newValue = Convert.ToUInt32(front + back, 16);
                                variationList[j, k].setup(name, itemID, newValue, path, true, "", flag1, flag2);
                            }
                            else
                            {
                                variationList[j, k].setup(name, itemID, (uint)(j + (0x20 * k)), path, true, "", flag1, flag2);
                            }
                        }
                    }
                    return variationList;
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
    }
}
