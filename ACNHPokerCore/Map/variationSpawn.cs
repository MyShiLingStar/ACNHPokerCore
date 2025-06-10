using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class VariationSpawn : Form
    {
        readonly InventorySlot[,] mainSlot = new InventorySlot[1, 3];
        readonly InventorySlot[,] subSlot = new InventorySlot[1, 3];
        readonly InventorySlot[,] allSlot = new InventorySlot[2, 3];
        readonly InventorySlot[,] mainHSlot = new InventorySlot[3, 1];
        readonly InventorySlot[,] subHSlot = new InventorySlot[3, 1];
        readonly InventorySlot[,] allHSlot = new InventorySlot[2, 3];

        private readonly MiniMap MiniMap;
        private readonly byte[] Layer1;
        private readonly byte[] Acre;
        private readonly byte[] Building;
        private readonly byte[] Terrain;
        private readonly byte[] Design;
        private readonly string Flag;
        private bool previewOn;

        private readonly int main;
        private readonly int sub;
        private readonly int X;
        private readonly int Y;

        private double width;
        private double height;
        private bool wallmount;
        //private bool rug = false;
        private bool ceiling;

        private readonly bool init = true;

        public event ObeySizeHandler SendObeySizeEvent;
        public event UpdateRowAndColumnHandler SendRowAndColumnEvent;

        public VariationSpawn(InventorySlot[,] variationList, byte[] layer1, byte[] acre, byte[] building, byte[] terrain, byte[] design, int x, int y, string flag, string size)
        {
            Layer1 = layer1;
            Acre = acre;
            Building = building;
            Terrain = terrain;
            Design = design;
            Flag = flag;
            X = x;
            Y = y;

            InitializeComponent();
            ProcessSize(size);

            mainSlot[0, 0] = main00;
            mainSlot[0, 1] = main01;
            mainSlot[0, 2] = main02;
            subSlot[0, 0] = sub00;
            subSlot[0, 1] = sub01;
            subSlot[0, 2] = sub02;
            allSlot[0, 0] = all00;
            allSlot[0, 1] = all01;
            allSlot[0, 2] = all02;
            allSlot[1, 0] = all10;
            allSlot[1, 1] = all11;
            allSlot[1, 2] = all12;

            mainHSlot[0, 0] = mainH00;
            mainHSlot[1, 0] = mainH01;
            mainHSlot[2, 0] = mainH02;
            subHSlot[0, 0] = subH00;
            subHSlot[1, 0] = subH01;
            subHSlot[2, 0] = subH02;

            allHSlot[0, 0] = allH00;
            allHSlot[0, 1] = allH01;
            allHSlot[0, 2] = allH02;
            allHSlot[1, 0] = allH10;
            allHSlot[1, 1] = allH11;
            allHSlot[1, 2] = allH12;


            main = variationList.GetLength(0);
            sub = variationList.GetLength(1);

            if (main <= 1)
            {
                mainOnly.Enabled = false;
                mainPanel.Enabled = false;
                all.Enabled = false;
                allPanel.Enabled = false;

                mainOnly.Checked = false;
                subOnly.Checked = true;
                okBtn.DialogResult = DialogResult.Yes;

                mainHOnly.Enabled = false;
                mainHPanel.Enabled = false;
                allH.Enabled = false;
                allHPanel.Enabled = false;

                mainHOnly.Checked = false;
                subHOnly.Checked = true;
                okHBtn.DialogResult = DialogResult.No;
            }
            else if (sub <= 1)
            {
                subOnly.Enabled = false;
                subPanel.Enabled = false;
                all.Enabled = false;
                allPanel.Enabled = false;

                mainOnly.Checked = true;
                subOnly.Checked = false;
                okBtn.DialogResult = DialogResult.OK;

                subHOnly.Enabled = false;
                subHPanel.Enabled = false;
                allH.Enabled = false;
                allHPanel.Enabled = false;

                mainHOnly.Checked = true;
                subHOnly.Checked = false;
                okHBtn.DialogResult = DialogResult.Abort;
            }

            //main
            for (int i = 0; i < main; i++)
            {
                if (i >= 3)
                    break;
                mainSlot[0, i].Setup(variationList[i, 0]);
                mainHSlot[i, 0].Setup(variationList[i, 0]);
            }

            //sub
            for (int j = 0; j < sub; j++)
            {
                if (j >= 3)
                    break;
                subSlot[0, j].Setup(variationList[0, j]);
                subHSlot[j, 0].Setup(variationList[0, j]);
            }

            for (int m = 0; m < main; m++)
            {
                if (m >= 2)
                    break;
                for (int n = 0; n < sub; n++)
                {
                    if (n >= 3)
                        continue;
                    allSlot[m, n].Setup(variationList[m, n]);
                }
            }

            for (int m = 0; m < main; m++)
            {
                if (m >= 3)
                    break;
                for (int n = 0; n < sub; n++)
                {
                    if (n >= 2)
                        continue;
                    allHSlot[n, m].Setup(variationList[m, n]);
                }
            }

            if (Flag == "00" || Flag == "01" || Flag == "02" || Flag == "03" || Flag == "04")
                ObeySizeToggle.Checked = true;

            MiniMap = new MiniMap(Layer1, Acre, Building, Terrain, Design, 4);
            miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawBackground(), MiniMap.DrawItemMap());
            init = false;

            UpdateSize();
        }

        private void MainOnly_CheckedChanged(object sender, EventArgs e)
        {
            okBtn.DialogResult = DialogResult.OK; // 1
            columnPanel.Enabled = true;
            UpdateSize();
        }

        private void SubOnly_CheckedChanged(object sender, EventArgs e)
        {
            okBtn.DialogResult = DialogResult.Yes; // 6
            columnPanel.Enabled = true;
            UpdateSize();
        }

        private void All_CheckedChanged(object sender, EventArgs e)
        {
            okBtn.DialogResult = DialogResult.Ignore; // 5
            columnPanel.Enabled = false;
            UpdateSize();
        }

        private void MainHOnly_CheckedChanged(object sender, EventArgs e)
        {
            okHBtn.DialogResult = DialogResult.Abort; // 3
            rowPanel.Enabled = true;
            UpdateSize();
        }

        private void SubHOnly_CheckedChanged(object sender, EventArgs e)
        {
            okHBtn.DialogResult = DialogResult.No; // 7
            rowPanel.Enabled = true;
            UpdateSize();
        }

        private void AllH_CheckedChanged(object sender, EventArgs e)
        {
            okHBtn.DialogResult = DialogResult.Retry; // 4
            rowPanel.Enabled = false;
            UpdateSize();
        }

        private void ColumnBox_ValueChanged(object sender, EventArgs e)
        {
            SendRowAndColumnEvent?.Invoke((int)rowBox.Value, (int)columnBox.Value);
            timesLabel1.Text = "× " + columnBox.Value;
            timesLabel2.Text = "× " + columnBox.Value;
            UpdateSize();
        }
        private void RowBox_ValueChanged(object sender, EventArgs e)
        {
            SendRowAndColumnEvent?.Invoke((int)rowBox.Value, (int)columnBox.Value);
            timesHLabel1.Text = "× " + rowBox.Value;
            timesHLabel2.Text = "× " + rowBox.Value;
            UpdateSize();
        }

        private void UpdateSize()
        {
            if (init)
                return;
            int totalHeight = 0;
            int totalWidth = 0;
            int extraHeight = 0;
            int extraWidth = 0;
            int extraColumn = (int)ExtraColumnBox.Value;
            int extraRow = (int)ExtraRowBox.Value;
            int wallMountExtra = 0;

            if (wallmount)
                wallMountExtra = 1;

            if (toggleBtn.Tag.ToString().Equals("Vertical"))
            {
                int column = (int)columnBox.Value;

                if (mainOnly.Checked)
                {
                    if (ObeySizeToggle.Checked)
                    {
                        if (PlaceHorizontal())
                        {
                            totalHeight = main * ((int)Math.Ceiling(height) + extraRow);
                            totalWidth = column * ((int)Math.Ceiling(width) + extraColumn + wallMountExtra);
                        }
                        else
                        {
                            totalHeight = main * ((int)Math.Ceiling(width) + extraRow + wallMountExtra);
                            totalWidth = column * ((int)Math.Ceiling(height) + extraColumn);
                        }
                        extraHeight = totalHeight - main;
                        extraWidth = totalWidth - column;
                    }

                    size.Text = (main + extraHeight) + " × " + (column + extraWidth);

                    if (previewOn)
                        miniMapBox.Image = MiniMap.DrawPreview(main + extraHeight, column + extraWidth, X, Y, true);
                }
                else if (subOnly.Checked)
                {
                    if (ObeySizeToggle.Checked)
                    {
                        if (PlaceHorizontal())
                        {
                            totalHeight = sub * ((int)Math.Ceiling(height) + extraRow);
                            totalWidth = column * ((int)Math.Ceiling(width) + extraColumn + wallMountExtra);
                        }
                        else
                        {
                            totalHeight = sub * ((int)Math.Ceiling(width) + extraRow + wallMountExtra);
                            totalWidth = column * ((int)Math.Ceiling(height) + extraColumn);
                        }
                        extraHeight = totalHeight - sub;
                        extraWidth = totalWidth - column;
                    }

                    size.Text = (sub + extraHeight) + " × " + (column + extraWidth);

                    if (previewOn)
                        miniMapBox.Image = MiniMap.DrawPreview(sub + extraHeight, column + extraWidth, X, Y, true);
                }
                else
                {
                    if (ObeySizeToggle.Checked)
                    {
                        if (PlaceHorizontal())
                        {
                            totalHeight = sub * ((int)Math.Ceiling(height) + extraRow);
                            totalWidth = main * ((int)Math.Ceiling(width) + extraColumn + wallMountExtra);
                        }
                        else
                        {
                            totalHeight = sub * ((int)Math.Ceiling(width) + extraRow + wallMountExtra);
                            totalWidth = main * ((int)Math.Ceiling(height) + extraColumn);
                        }
                        extraHeight = totalHeight - sub;
                        extraWidth = totalWidth - main;
                    }

                    size.Text = (sub + extraHeight) + " × " + (main + extraWidth);

                    if (previewOn)
                        miniMapBox.Image = MiniMap.DrawPreview(sub + extraHeight, main + extraWidth, X, Y, true);
                }
            }
            else
            {
                int row = (int)rowBox.Value;

                if (mainHOnly.Checked)
                {
                    if (ObeySizeToggle.Checked)
                    {
                        if (PlaceHorizontal())
                        {
                            totalHeight = row * ((int)Math.Ceiling(height) + extraRow);
                            totalWidth = main * ((int)Math.Ceiling(width) + extraColumn + wallMountExtra);
                        }
                        else
                        {
                            totalHeight = row * ((int)Math.Ceiling(width) + extraRow + wallMountExtra);
                            totalWidth = main * ((int)Math.Ceiling(height) + extraColumn);
                        }
                        extraHeight = totalHeight - row;
                        extraWidth = totalWidth - main;
                    }

                    sizeH.Text = (row + extraHeight) + " × " + (main + extraWidth);

                    if (previewOn)
                        miniMapBox.Image = MiniMap.DrawPreview(row + extraHeight, main + extraWidth, X, Y, true);
                }
                else if (subHOnly.Checked)
                {
                    if (ObeySizeToggle.Checked)
                    {
                        if (PlaceHorizontal())
                        {
                            totalHeight = row * ((int)Math.Ceiling(height) + extraRow);
                            totalWidth = sub * ((int)Math.Ceiling(width) + extraColumn + wallMountExtra);
                        }
                        else
                        {
                            totalHeight = row * ((int)Math.Ceiling(width) + extraRow + wallMountExtra);
                            totalWidth = sub * ((int)Math.Ceiling(height) + extraColumn);
                        }
                        extraHeight = totalHeight - row;
                        extraWidth = totalWidth - sub;
                    }

                    sizeH.Text = (row + extraHeight) + " × " + (sub + extraWidth);

                    if (previewOn)
                        miniMapBox.Image = MiniMap.DrawPreview(row + extraHeight, sub + extraWidth, X, Y, true);
                }
                else
                {
                    if (ObeySizeToggle.Checked)
                    {
                        if (PlaceHorizontal())
                        {
                            totalHeight = main * ((int)Math.Ceiling(height) + extraRow);
                            totalWidth = sub * ((int)Math.Ceiling(width) + extraColumn + wallMountExtra);
                        }
                        else
                        {
                            totalHeight = main * ((int)Math.Ceiling(width) + extraRow + wallMountExtra);
                            totalWidth = sub * ((int)Math.Ceiling(height) + extraColumn);
                        }
                        extraHeight = totalHeight - main;
                        extraWidth = totalWidth - sub;
                    }

                    sizeH.Text = (main + extraHeight) + " × " + (sub + extraWidth);

                    if (previewOn)
                        miniMapBox.Image = MiniMap.DrawPreview(main + extraHeight, sub + extraWidth, X, Y, true);
                }
            }

            if (ObeySizeToggle.Checked)
            {
                if (SendObeySizeEvent != null)
                {
                    if (PlaceHorizontal())
                    {
                        if (wallmount)
                            SendObeySizeEvent(true, ((int)Math.Ceiling(height) + extraRow), ((int)Math.Ceiling(width) + extraColumn + wallMountExtra), totalHeight, totalWidth, true, false);
                        else if (ceiling)
                            SendObeySizeEvent(true, ((int)Math.Ceiling(height) + extraRow), ((int)Math.Ceiling(width) + extraColumn), totalHeight, totalWidth, false, true);
                        else
                            SendObeySizeEvent(true, ((int)Math.Ceiling(height) + extraRow), ((int)Math.Ceiling(width) + extraColumn), totalHeight, totalWidth, false, false);
                    }
                    else
                    {
                        if (wallmount)
                            SendObeySizeEvent(true, ((int)Math.Ceiling(width) + extraRow + wallMountExtra), ((int)Math.Ceiling(height) + extraColumn), totalHeight, totalWidth, true, false);
                        else if (ceiling)
                            SendObeySizeEvent(true, ((int)Math.Ceiling(width) + extraRow), ((int)Math.Ceiling(height) + extraColumn), totalHeight, totalWidth, false, true);
                        else
                            SendObeySizeEvent(true, ((int)Math.Ceiling(width) + extraRow), ((int)Math.Ceiling(height) + extraColumn), totalHeight, totalWidth, false, false);
                    }
                }
            }
            else
            {
                SendObeySizeEvent?.Invoke(false);
            }
        }

        private void ToggleBtn_Click(object sender, EventArgs e)
        {
            if (toggleBtn.Tag.ToString().Equals("Vertical"))
            {
                toggleBtn.Tag = "Horizontal";
                toggleBtn.Text = "Horizontal";
                toggleBtn.BackColor = Color.Orange;
                horiPanel.Visible = true;
                vertPanel.Visible = false;
                UpdateSize();
            }
            else
            {
                toggleBtn.Tag = "Vertical";
                toggleBtn.Text = "Vertical";
                toggleBtn.BackColor = Color.FromArgb(114, 137, 218);
                horiPanel.Visible = false;
                vertPanel.Visible = true;
                UpdateSize();
            }
        }

        private void PreviewBtn_Click(object sender, EventArgs e)
        {
            if (previewOn)
            {
                Width = 690;
                previewOn = false;
            }
            else
            {
                Width = 1150;
                previewOn = true;
                UpdateSize();
            }
        }

        private void PreviewHBtn_Click(object sender, EventArgs e)
        {
            if (previewOn)
            {
                Width = 690;
                previewOn = false;
            }
            else
            {
                Width = 1150;
                previewOn = true;
                UpdateSize();
            }
        }

        private void ProcessSize(string size)
        {
            if (size == null)
            {
                height = 1;
                width = 1;
                wallmount = false;
                //rug = false;
                MyMessageBox.Show("Missing Size data", "Sadge", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            else if (size == "")
            {
                height = 1;
                width = 1;
                wallmount = false;
                //rug = false;
                MyMessageBox.Show("Missing Size data", "Sadge", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }

            if (size.Contains("_Wall"))
            {
                wallmount = true;
                //rug = false;
                ceiling = false;
            }
            else if (size.Contains("_Rug"))
            {
                wallmount = false;
                //rug = true;
                ceiling = false;
            }
            else if (size.Contains("_Ceiling"))
            {
                wallmount = false;
                //rug = false;
                ceiling = true;
            }
            else
            {
                wallmount = false;
                //rug = false;
                ceiling = false;
            }

            if (size.Contains('x'))
            {
                string[] dimension = size.Replace("_Wall", "").Replace("_Rug", "").Replace("_Pillar", "").Replace("_Ceiling", "").Split("x");
                string[] front = dimension[0].Split("_");
                string[] back = dimension[1].Split("_");

                width = double.Parse(front[0], System.Globalization.CultureInfo.InvariantCulture);
                if (front[1] == "5")
                    width += 0.5;
                height = double.Parse(back[0], System.Globalization.CultureInfo.InvariantCulture);
                if (back[1] == "5")
                    height += 0.5;
            }
        }

        private bool PlaceHorizontal()
        {
            if (Flag == "00" || Flag == "02" || Flag == "04")
                return true;
            else
                return false;
        }

        private void ObeySizeToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (ObeySizeToggle.Checked)
            {
                ExtraColumnLabel.Enabled = true;
                ExtraRowLabel.Enabled = true;
                ExtraColumnBox.Enabled = true;
                ExtraRowBox.Enabled = true;
            }
            else
            {
                ExtraColumnLabel.Enabled = false;
                ExtraRowLabel.Enabled = false;
                ExtraColumnBox.Enabled = false;
                ExtraRowBox.Enabled = false;
            }
            if (init)
                return;

            UpdateSize();
        }

        private void OkHBtn_Click(object sender, EventArgs e)
        {
            UpdateSize();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            UpdateSize();
        }

        private void ExtraColumnBox_ValueChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }

        private void ExtraRowBox_ValueChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }
    }
}
