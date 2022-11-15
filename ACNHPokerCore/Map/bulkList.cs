using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class BulkList : Form
    {
        private DataTable items;

        private DataGridViewRow lastRow = null;

        private int RowMouseDown = -1;
        private int RowMouseUp = -1;
        private bool HoldingMouseDown = false;

        private bool sound;
        public bool CancelFormClose = false;

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
                HeaderText = "Image",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            ItemGridView.Columns.Insert(items.Columns.Count, imageColumn);

            ItemGridView.Columns["ID"].Visible = false;
            ItemGridView.Columns["Count"].Visible = false;
            ItemGridView.Columns["ImagePath"].Visible = false;

            ItemGridView.Columns["#"].Width = 50;
            ItemGridView.Columns["Name"].Width = 195;
            ItemGridView.Columns["Image"].Width = 128;

            foreach (DataGridViewColumn column in ItemGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            ItemGridView.DefaultCellStyle.Font = new Font("Microsoft JhengHei UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);

            sound = Sound;
        }

        public void ReceiveItem(string id, string count, string name, string path)
        {
            string[] dataRow = new string[5];
            dataRow[0] = (items.Rows.Count + 1).ToString();
            dataRow[1] = id;
            dataRow[2] = count;
            dataRow[3] = name;
            dataRow[4] = path;

            items.Rows.Add(dataRow);

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
            else
            {
                SwapItem(RowMouseDown, RowMouseUp);
                ItemGridView.Rows[RowMouseUp].Selected = true;
                //ItemGridView_CellMouseClick(sender, e);
            }
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
            else
            {
                ItemGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Orange;
            }
        }

        private void ItemGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            //Debug.Print("Leave " + e.RowIndex);
            if (e.RowIndex < 0)
                return;
            else
            {
                ItemGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
            }
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
            ItemGridView.Height = this.Height - 63;
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
            this.CloseForm();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            int numOfItem = items.Rows.Count;
            if (numOfItem == 0) return;

            SaveFileDialog file = new()
            {
                Filter = "New Horizons Bulk Spawn (*.nhbs)|*.nhbs|New Horizons Inventory(*.nhi) | *.nhi",
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
                string first = Utilities.flip(Utilities.precedingZeros(items.Rows[i]["ID"].ToString(), 8));
                string second = Utilities.flip(Utilities.precedingZeros(items.Rows[i]["Count"].ToString(), 8));

                Bank = Bank + first + second;
            }

            for (int i = 0; i < Bank.Length / 2 - 1; i++)
            {
                string data = String.Concat(Bank[(i * 2)].ToString(), Bank[((i * 2) + 1)].ToString());
                save[i] = Convert.ToByte(data, 16);
            }

            File.WriteAllBytes(file.FileName, save);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }
    }
}
