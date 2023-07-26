using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace ACNHPokerCore
{
    public partial class BulkList : Form
    {
        private DataTable items;

        private DataGridViewRow lastRow;

        private int RowMouseDown = -1;
        private int RowMouseUp = -1;
        private bool HoldingMouseDown;

        private bool sound;
        public bool CancelFormClose;

        public event CloseHandler CloseForm;

        public BulkList(bool Sound)
        {
            InitializeComponent();

            items = new DataTable();
            items.Columns.Add("#");
            items.Columns.Add("ID");
            items.Columns.Add("Count");
            items.Columns.Add("Name");
            items.Columns.Add("ImagePath");
            items.Columns.Add("Random");

            ItemGridView.DataSource = items;

            ItemGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
            ItemGridView.DefaultCellStyle.ForeColor = Color.FromArgb(255, 114, 105, 110);
            ItemGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 114, 137, 218);

            ItemGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 57, 60, 67);
            ItemGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            ItemGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);

            ItemGridView.EnableHeadersVisualStyles = false;

            DataGridViewImageColumn imageColumn = new()
            {
                Name = "Image",
                HeaderText = @"Image",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            ItemGridView.Columns.Insert(items.Columns.Count, imageColumn);

            ItemGridView.Columns["ID"]!.Visible = false;
            ItemGridView.Columns["Count"]!.Visible = false;
            ItemGridView.Columns["ImagePath"]!.Visible = false;
            ItemGridView.Columns["Random"]!.Visible = false;

            ItemGridView.Columns["#"]!.Width = 60;
            ItemGridView.Columns["Name"]!.Width = 185;
            ItemGridView.Columns["Image"]!.Width = 128;

            ItemGridView.DefaultCellStyle.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold, GraphicsUnit.Point);

            WrapSelector.SelectedIndex = 0;

            sound = Sound;
        }

        public void ReceiveItem(string id, string count, string name, string path)
        {
            string[] dataRow = new string[6];
            dataRow[0] = (items.Rows.Count + 1).ToString();
            dataRow[1] = id;
            dataRow[2] = count;
            dataRow[3] = name;
            dataRow[4] = path;

            items.Rows.Add(dataRow);
        }

        public void ReceiveItem(byte[] itemByte)
        {
            byte[] slotBytes = new byte[2];
            byte[] flag0Bytes = new byte[1];
            byte[] flag1Bytes = new byte[1];
            byte[] dataBytes = new byte[4];
            byte[] recipeBytes = new byte[2];

            Buffer.BlockCopy(itemByte, 0, slotBytes, 0, 2);
            Buffer.BlockCopy(itemByte, 3, flag0Bytes, 0, 1);
            Buffer.BlockCopy(itemByte, 2, flag1Bytes, 0, 1);
            Buffer.BlockCopy(itemByte, 4, dataBytes, 0, 4);
            Buffer.BlockCopy(itemByte, 4, recipeBytes, 0, 2);

            string itemId = Utilities.PrecedingZeros(Utilities.Flip(Utilities.ByteToHexString(slotBytes)), 4);
            string itemData = Utilities.PrecedingZeros(Utilities.Flip(Utilities.ByteToHexString(dataBytes)), 8);
            string recipeData = Utilities.PrecedingZeros(Utilities.Flip(Utilities.ByteToHexString(recipeBytes)), 4);
            string flag0 = Utilities.ByteToHexString(flag0Bytes);
            string flag1 = Utilities.ByteToHexString(flag1Bytes);


            string[] dataRow = new string[5];
            dataRow[0] = (items.Rows.Count + 1).ToString();
            dataRow[1] = itemId;
            dataRow[2] = itemData;
            if (itemId.Equals("16A2"))
            {
                dataRow[3] = "[Recipe] " + Main.GetNameFromID(recipeData, true);
                dataRow[4] = Main.GetImagePathFromID(recipeData, Convert.ToUInt32("0x" + itemData, 16), true);
            }
            else
            {
                dataRow[3] = Main.GetNameFromID(itemId, false);
                dataRow[4] = Main.GetImagePathFromID(itemId, Convert.ToUInt32("0x" + itemData, 16), false);
            }

            items.Rows.Add(dataRow);
        }

        public void ScrollUpdate()
        {
            if (ItemGridView.RowCount > 0)
                ItemGridView.FirstDisplayedScrollingRowIndex = ItemGridView.RowCount - 1;
            ItemGridView.ClearSelection();
        }

        private void ItemGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (ItemGridView.Columns["Image"] == null)
                return;
            if (e.RowIndex >= 0 && e.RowIndex < ItemGridView.Rows.Count)
            {
                if (e.ColumnIndex == ItemGridView.Columns["Image"].Index)
                {
                    if (ItemGridView.Rows[e.RowIndex].Cells["ImagePath"].Value != null)
                    {
                        Image img = Image.FromFile(ItemGridView.Rows[e.RowIndex].Cells["ImagePath"].Value.ToString());
                        e.Value = img;
                    }
                }
            }
        }

        private void ItemGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //Debug.Print("Click " + e.RowIndex);
            if (e.RowIndex < 0)
                return;
            if (lastRow != null)
            {
                lastRow.Height = 22;
            }

            if (e.RowIndex > -1)
            {
                lastRow = ItemGridView.Rows[e.RowIndex];
                ItemGridView.Rows[e.RowIndex].Height = 128;
            }
        }

        private void ItemGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            //Debug.Print("Down " + e.RowIndex);
            RowMouseDown = e.RowIndex;
            HoldingMouseDown = true;
        }

        private void ItemGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            //Debug.Print("Up " + e.RowIndex);
            RowMouseUp = e.RowIndex;
            HoldingMouseDown = false;

            if (RowMouseDown == RowMouseUp || RowMouseDown < 0 || RowMouseUp < 0)
                return;
            SwapItem(RowMouseDown, RowMouseUp);
            ItemGridView.Rows[RowMouseUp].Selected = true;
            //ItemGridView_CellMouseClick(sender, e);
        }

        private void SwapItem(int FirstRowIndex, int SecondRowIndex)
        {
            DataRow FirstRow;
            DataRow SecondRow;
            FirstRow = items.NewRow();
            SecondRow = items.NewRow();

            FirstRow[0] = items.Rows[SecondRowIndex][0];
            SecondRow[0] = items.Rows[FirstRowIndex][0];

            for (int i = 1; i < items.Columns.Count; i++)
            {
                FirstRow[i] = items.Rows[FirstRowIndex][i];
                SecondRow[i] = items.Rows[SecondRowIndex][i];
            }

            items.Rows[FirstRowIndex].Delete();
            items.Rows.InsertAt(SecondRow, FirstRowIndex);
            items.Rows[SecondRowIndex].Delete();
            items.Rows.InsertAt(FirstRow, SecondRowIndex);
        }

        private void ItemGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            //Debug.Print("Enter " + e.RowIndex);
            if (!HoldingMouseDown || e.RowIndex < 0)
                return;
            ItemGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Orange;
        }

        private void ItemGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            //Debug.Print("Leave " + e.RowIndex);
            if (e.RowIndex < 0)
                return;
            ItemGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
        }

        private void ItemGridView_MouseUp(object sender, MouseEventArgs e)
        {
            HoldingMouseDown = false;
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (ItemGridView.SelectedRows.Count == 0) return;

            int index = ItemGridView.SelectedRows[0].Index;
            items.Rows[index].Delete();

            for (int i = 0; i < items.Rows.Count; i++)
            {
                items.Rows[i][0] = (i + 1).ToString();
            }
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MyMessageBox.Show("Are you sure?", "Slay them all!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                items.Clear();
            }
        }

        private void BulkList_Resize(object sender, EventArgs e)
        {
            ItemGridView.Height = Height - 120;
        }

        private void BulkList_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MyMessageBox.Show("Are you sure? Closing the form will lose all unsaved changes.", "Let them burn!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.No)
            {
                CancelFormClose = true;
                e.Cancel = true;
                return;
            }

            CloseForm?.Invoke();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            int numOfItem = items.Rows.Count;
            if (numOfItem == 0) return;

            if (WrapSelector.SelectedIndex < 0)
                WrapSelector.SelectedIndex = 0;

            string[] flagBuffer = WrapSelector.SelectedItem.ToString()?.Split(' ');
            string flagValue = flagBuffer?[flagBuffer.Length - 1];
            byte flagByte = 0;
            if (!flagValue.Equals("XX"))
                flagByte = Utilities.StringToByte(flagValue)[0];

            string flag0;
            bool randomWrap = false;
            byte[] RandomWrapList = null;

            if (flagValue.Equals("00"))
                flag0 = "00";
            else if (flagValue.Equals("XX"))
            {
                randomWrap = true;
                RandomWrapList = BuildRandomWrapList();
                flag0 = "00";
            }
            else
            {
                if (RetainNameToggle.Checked)
                    flag0 = Utilities.PrecedingZeros((flagByte + 0x40).ToString("X"), 2);
                else
                    flag0 = Utilities.PrecedingZeros((flagByte).ToString("X"), 2);
            }

            string flag1 = "00";

            SaveFileDialog file = new()
            {
                Filter = @"New Horizons Bulk Spawn (*.nhbs)|*.nhbs|New Horizons Inventory(*.nhi) | *.nhi",
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

            byte[] save = new byte[numOfItem * 8];

            string Bank = "";

            for (int i = 0; i < numOfItem; i++)
            {
                if (randomWrap)
                {
                    Random rad = new();
                    int num = rad.Next(0, RandomWrapList.Length);

                    if (RetainNameToggle.Checked)
                        flag0 = Utilities.PrecedingZeros((RandomWrapList[num] + 0x40).ToString("X"), 2);
                    else
                        flag0 = Utilities.PrecedingZeros(RandomWrapList[num].ToString("X"), 2);

                }
                string first = Utilities.Flip(Utilities.PrecedingZeros(flag0 + flag1 + Utilities.PrecedingZeros(items.Rows[i]["ID"].ToString(), 4), 8));
                string second = Utilities.Flip(Utilities.PrecedingZeros(items.Rows[i]["Count"].ToString(), 8));

                Bank = Bank + first + second;
            }

            for (int i = 0; i < Bank.Length / 2 - 1; i++)
            {
                string data = string.Concat(Bank[(i * 2)].ToString(), Bank[((i * 2) + 1)].ToString());
                save[i] = Convert.ToByte(data, 16);
            }

            File.WriteAllBytes(file.FileName, save);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private byte[] BuildRandomWrapList()
        {
            byte[] list = new byte[WrapSelector.Items.Count - 2];

            for (int i = 2; i < WrapSelector.Items.Count; i++)
            {
                list[i - 2] = Utilities.StringToByte((WrapSelector.Items[i].ToString()?.Split(' '))[2])[0];
            }
            return list;
        }

        private void LoadBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new()
                {
                    Filter = @"New Horizons Inventory(*.nhi) | *.nhi|New Horizons Bulk Spawn (*.nhbs)| *.nhbs|All files (*.*)|*.*",
                    FileName = "items.nhi",
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

                byte[][] item = ProcessNHI(data);

                for (int i = 0; i < item.Length; i++)
                {
                    ReceiveItem(item[i]);
                }

                ScrollUpdate();
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("BulkList", "Load " + ex.Message);
                MyMessageBox.Show(ex.Message, "How CPUs use Multiple Cores?");
            }
        }

        private static byte[][] ProcessNHI(byte[] data)
        {
            int numOfItem = data.Length / 8;
            byte[] tempItem = new byte[8];
            bool[] isItem = new bool[numOfItem];
            int numOfitem = 0;

            for (int i = 0; i < numOfItem; i++)
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
            for (int j = 0; j < numOfItem; j++)
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

        private void ShuffleBtn_Click(object sender, EventArgs e)
        {
            Random r = new();
            int rInt;

            for (int i = 0; i < items.Rows.Count; i++)
            {
                rInt = r.Next(10000, 65535);
                items.Rows[i]["Random"] = rInt.ToString();
            }

            items.DefaultView.Sort = "Random ASC";
            items = items.DefaultView.ToTable();

            for (int i = 0; i < items.Rows.Count; i++)
            {
                items.Rows[i][0] = (i + 1).ToString();
            }

            ItemGridView.DataSource = items;

            ScrollUpdate();
        }
    }
}
