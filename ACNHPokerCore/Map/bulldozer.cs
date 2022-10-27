using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{

    public partial class Bulldozer : Form
    {

        private static Socket s;
        private static USBBot usb;
        private readonly bool sound;
        private MiniMap MiniMap = null;
        private byte[] Layer1;
        private byte[] Acre;
        private byte[] Building;
        private byte[] Terrain;
        private byte[] MapCustomDesgin;
        private int counter = 0;

        private byte[][] buildingList = null;
        private const int BuildingSize = 0x14;
        private const int NumOfBuilding = 46;

        private const int numOfColumn = 0x70;
        private const int numOfRow = 0x60;

        private int selectedAcre;

        private Panel selectedPanel;
        private bool MapOrGridViewChange = false;
        private bool plazaEdited = false;
        private bool valueUpdated = false;
        private Point lastDisplayTooltip = new(-1, -1);

        private int lastBuilding = 0;

        public event CloseHandler CloseForm;
        private bool formClosed = false;

        public Bulldozer(Socket S, USBBot USB, bool Sound)
        {
            s = S;
            usb = USB;
            sound = Sound;

            InitializeComponent();

            Thread LoadThread = new(delegate () { LoadMap(); });
            LoadThread.Start();

            var imageList = new ImageList
            {
                ImageSize = new Size(64, 64)
            };
            acreList.LargeImageList = imageList;
            acreList.TileSize = new Size(80, 80);
            acreList.View = View.Tile;
            acreList.OwnerDraw = true;
            acreList.DrawItem += AcreList_DrawItem;

            for (ushort i = 0; i < 0x13E; i++)
            {
                if (Enum.IsDefined(typeof(Utilities.Acre), i))
                {
                    var AcreName = (Utilities.Acre)i;

                    imageList.Images.Add(i.ToString(), MiniMap.GetAcreImage(i, 4));
                    acreList.Items.Add(i.ToString(), i.ToString());
                    acreList.Items[acreList.Items.Count - 1].ToolTipText = AcreName.ToString();
                }
            }
        }

        private void AcreList_DrawItem(object sender, DrawListViewItemEventArgs e)
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

        private void LoadMap()
        {
            var layer1Address = Utilities.mapZero;

            if (s != null || usb != null)
            {
                counter = 0;

                //Layer1 = Utilities.getMapLayer(s, usb, layer1Address, ref counter);
                Layer1 = null;
                //Layer2 = Utilities.getMapLayer(s, bot, layer2Address, ref counter);
                Acre = Utilities.getAcre(s, usb);
                Building = Utilities.getBuilding(s, usb);
                Terrain = Utilities.getTerrain(s, usb);
                MapCustomDesgin = Utilities.getCustomDesignMap(s, null, ref counter);

                if (Acre != null && Building != null && Terrain != null)
                {
                    if (MiniMap == null)
                        MiniMap = new MiniMap(Layer1, Acre, Building, Terrain, MapCustomDesgin, 4);
                }
                else
                    throw new NullReferenceException("Acre/Building/Terrain");

                selectedPanel = acrePanel;

                miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());

                buildingGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
                buildingGridView.DefaultCellStyle.BackColor = Color.FromArgb(255, 47, 49, 54);
                buildingGridView.DefaultCellStyle.ForeColor = Color.White;
                buildingGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 114, 137, 218);
                buildingGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 57, 60, 67);
                buildingGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                buildingGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 57, 60, 67);
                buildingGridView.EnableHeadersVisualStyles = false;

                DataGridViewColumn BuildingColor = new DataGridViewTextBoxColumn();
                DataGridViewColumn BuildingID = new DataGridViewTextBoxColumn();
                DataGridViewColumn BuildingName = new DataGridViewTextBoxColumn();
                DataGridViewColumn BuildingX = new DataGridViewTextBoxColumn();
                DataGridViewColumn BuildingY = new DataGridViewTextBoxColumn();

                DataGridViewColumn BuildingA = new DataGridViewTextBoxColumn();
                DataGridViewColumn BuildingT = new DataGridViewTextBoxColumn();

                BuildingColor.HeaderText = "";
                BuildingColor.Name = "Color";
                BuildingColor.SortMode = DataGridViewColumnSortMode.NotSortable;
                BuildingColor.Width = 23;
                BuildingID.HeaderText = "";
                BuildingID.Name = "ID";
                BuildingID.SortMode = DataGridViewColumnSortMode.NotSortable;
                BuildingID.Width = 60;
                BuildingName.HeaderText = "";
                BuildingName.Name = "Name";
                BuildingName.SortMode = DataGridViewColumnSortMode.NotSortable;
                BuildingName.Width = 200;
                BuildingX.HeaderText = "X";
                BuildingX.Name = "X-Coordinate";
                BuildingX.SortMode = DataGridViewColumnSortMode.NotSortable;
                BuildingX.Width = 60;
                BuildingY.HeaderText = "Y";
                BuildingY.Name = "Y-Coordinate";
                BuildingY.SortMode = DataGridViewColumnSortMode.NotSortable;
                BuildingY.Width = 60;
                BuildingA.HeaderText = "Angle";
                BuildingA.Name = "Angle";
                BuildingA.SortMode = DataGridViewColumnSortMode.NotSortable;
                BuildingA.Width = 60;
                BuildingT.HeaderText = "Type";
                BuildingT.Name = "Type";
                BuildingT.SortMode = DataGridViewColumnSortMode.NotSortable;
                BuildingT.Width = 60;

                buildingGridView.Columns.Add(BuildingID);
                buildingGridView.Columns.Add(BuildingColor);
                buildingGridView.Columns.Add(BuildingName);
                buildingGridView.Columns.Add(BuildingX);
                buildingGridView.Columns.Add(BuildingY);
                buildingGridView.Columns.Add(BuildingA);
                buildingGridView.Columns.Add(BuildingT);

                buildingGridView.Columns["ID"].Visible = false;

                buildingGridView.Columns["X-Coordinate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                buildingGridView.Columns["Y-Coordinate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                buildingGridView.Columns["X-Coordinate"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                buildingGridView.Columns["Y-Coordinate"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                buildingGridView.Columns["Angle"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                buildingGridView.Columns["Type"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                buildingGridView.Columns["Angle"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                buildingGridView.Columns["Type"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;


                if (Acre != null)
                {
                    buildingGridView.Rows.Add(0x00FF, "", "Plaza", Acre[0x94], Acre[0x98], 0, 0);

                    DataGridViewCellStyle style = new()
                    {
                        BackColor = Color.DarkSalmon
                    };
                    buildingGridView.Rows[0].Cells["Color"].Style = style;
                }

                if (Building != null)
                    FillBuilding();

                buildingGridView.CurrentCell = buildingGridView.Rows[0].Cells[2];
            }

            if (!formClosed)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    LoadingPanel.Visible = false;
                    miniMapBox.Visible = true;
                    AcreBtn.Visible = true;
                    BuildingBtn.Visible = true;
                    TerrainBtn.Visible = true;
                    acrePanel.Visible = true;
                });
            }
        }

        private void FillBuilding()
        {
            if (Building != null)
            {
                buildingList = new byte[NumOfBuilding][];
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    buildingList[i] = new byte[BuildingSize];
                    Buffer.BlockCopy(Building, i * BuildingSize, buildingList[i], 0x0, BuildingSize);

                    DataGridViewCellStyle style = new();

                    byte key = buildingList[i][0];
                    if (BuildingName.ContainsKey(key))
                    {
                        buildingGridView.Rows.Add(buildingList[i][0x0], "", BuildingName[key], buildingList[i][0x2], buildingList[i][0x4], buildingList[i][0x6], buildingList[i][0x8]);
                        style.BackColor = MiniMap.ByteToBuildingColor[buildingList[i][0x0]];
                    }
                    else
                    {
                        buildingGridView.Rows.Add(buildingList[i][0x0], "", key.ToString("X"), buildingList[i][0x2], buildingList[i][0x4], buildingList[i][0x6], buildingList[i][0x8]);
                        style.BackColor = Color.Black;
                    }

                    buildingGridView.Rows[buildingGridView.RowCount - 1].Cells["Color"].Style = style;
                }
            }
        }

        private void MiniMapBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int Realx;
                int Realy;
                int x = (e.X / 2 - 32) / 2;
                int y = (e.Y / 2 - 32) / 2;


                if (e.X < 0)
                    Realx = 0;
                else if (e.X >= 16 * 9 * 4)
                    Realx = 287;
                else
                    Realx = e.X / 2;

                if (e.Y < 0)
                    Realy = 0;
                else if (e.Y >= 16 * 8 * 4)
                    Realy = 255;
                else
                    Realy = e.Y / 2;

                if (x < 0)
                    x = 0;
                else if (x > 111)
                    x = 111;

                if (y < 0)
                    y = 0;
                else if (y > 95)
                    y = 95;

                RealXCoordinate.Text = Realx.ToString();
                RealYCoordinate.Text = Realy.ToString();
                xCoordinate.Text = x.ToString();
                yCoordinate.Text = y.ToString();

                int AcreX = Realx / 32;
                int AcreY = Realy / 32;

                if (selectedPanel == acrePanel)
                {
                    miniMapBox.Image = MiniMap.DrawSelectAcre(AcreX, AcreY);
                }
                else
                {
                    if (BuildingControl.Visible)
                    {
                        if (Realx > 255)
                            Realx = 255;
                        if (Realy > 255)
                            Realy = 255;

                        MapOrGridViewChange = true;

                        XUpDown.Value = Realx;
                        YUpDown.Value = Realy;

                        if (buildingGridView.CurrentCell.RowIndex != -1)
                        {
                            int OrgX = Int16.Parse(buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["X-Coordinate"].Value.ToString());
                            int OrgY = Int16.Parse(buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Y-Coordinate"].Value.ToString());
                            int index = BuildingType.SelectedIndex;
                            byte type = (byte)index;
                            miniMapBox.Image = MiniMap.DrawLargeMarker(OrgX, OrgY, Realx, Realy, lastBuilding, type);
                        }
                        else
                            miniMapBox.Image = MiniMap.DrawLargeMarker(Realx, Realy, Realx, Realy);

                        MapOrGridViewChange = false;
                    }
                }

                if (AcreY <= 0)
                    selectedAcre = AcreX;
                else if (AcreY == 1)
                    selectedAcre = AcreX + 9;
                else if (AcreY == 2)
                    selectedAcre = AcreX + 18;
                else if (AcreY == 3)
                    selectedAcre = AcreX + 27;
                else if (AcreY == 4)
                    selectedAcre = AcreX + 36;
                else if (AcreY == 5)
                    selectedAcre = AcreX + 45;
                else if (AcreY == 6)
                    selectedAcre = AcreX + 54;
                else if (AcreY >= 7)
                    selectedAcre = AcreX + 63;

                selectedAcreBox.Text = selectedAcre.ToString();
                selectedAcreValueBox.Text = "0x" + Acre[selectedAcre * 2].ToString("X");
            }
        }

        private void MiniMapBox_MouseMove(object sender, MouseEventArgs e)
        {
            int Realx;
            int Realy;

            if (e.X < 0)
                Realx = 0;
            else if (e.X >= 16 * 9 * 4)
                Realx = 287;
            else
                Realx = e.X / 2;

            if (e.Y < 0)
                Realy = 0;
            else if (e.Y >= 16 * 8 * 4)
                Realy = 255;
            else
                Realy = e.Y / 2;

            int AcreX = Realx / 32;
            int AcreY = Realy / 32;

            if (selectedPanel == acrePanel)
            {
                if (AcreX != lastDisplayTooltip.X || AcreY != lastDisplayTooltip.Y)
                {
                    string CoordinateText = "( " + AcreX + " , " + AcreY + " )";

                    int HoverAcre = 0;
                    if (AcreY <= 0)
                        HoverAcre = AcreX;
                    else if (AcreY == 1)
                        HoverAcre = AcreX + 9;
                    else if (AcreY == 2)
                        HoverAcre = AcreX + 18;
                    else if (AcreY == 3)
                        HoverAcre = AcreX + 27;
                    else if (AcreY == 4)
                        HoverAcre = AcreX + 36;
                    else if (AcreY == 5)
                        HoverAcre = AcreX + 45;
                    else if (AcreY == 6)
                        HoverAcre = AcreX + 54;
                    else if (AcreY >= 7)
                        HoverAcre = AcreX + 63;

                    byte[] AcreBytes = new byte[2];
                    AcreBytes[0] = Acre[HoverAcre * 2];
                    AcreBytes[1] = Acre[HoverAcre * 2 + 1];
                    int AcreNumber = BitConverter.ToInt16(AcreBytes, 0);
                    var AcreName = (Utilities.Acre)AcreNumber;
                    MapToolTip.Show(CoordinateText + "\n" + AcreName + "\n" + AcreNumber.ToString(), miniMapBox, AcreX * 64 + 64, AcreY * 64 + 64);
                    lastDisplayTooltip = new Point(AcreX, AcreY);
                }
            }


            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int x = (e.X / 2 - 32) / 2;
                int y = (e.Y / 2 - 32) / 2;

                if (x < 0)
                    x = 0;
                else if (x > 111)
                    x = 111;

                if (y < 0)
                    y = 0;
                else if (y > 95)
                    y = 95;

                RealXCoordinate.Text = Realx.ToString();
                RealYCoordinate.Text = Realy.ToString();
                xCoordinate.Text = x.ToString();
                yCoordinate.Text = y.ToString();

                if (selectedPanel == buildingPanel)
                {
                    if (BuildingControl.Visible)
                    {
                        if (Realx > 255)
                            Realx = 255;
                        if (Realy > 255)
                            Realy = 255;

                        MapOrGridViewChange = true;

                        XUpDown.Value = Realx;
                        YUpDown.Value = Realy;

                        if (buildingGridView.CurrentCell.RowIndex != -1)
                        {
                            int OrgX = Int16.Parse(buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["X-Coordinate"].Value.ToString());
                            int OrgY = Int16.Parse(buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Y-Coordinate"].Value.ToString());
                            int index = BuildingType.SelectedIndex;
                            byte type = (byte)index;
                            miniMapBox.Image = MiniMap.DrawLargeMarker(OrgX, OrgY, Realx, Realy, lastBuilding, type);
                        }
                        else
                            miniMapBox.Image = MiniMap.DrawLargeMarker(Realx, Realy, Realx, Realy);

                        MapOrGridViewChange = false;
                    }
                }
            }
        }

        private void SendBtn_Click(object sender, EventArgs e)
        {
            byte[] AcreOnly = new byte[0x90];
            Buffer.BlockCopy(Acre, 0x0, AcreOnly, 0x0, 0x90);
            int counter = 0;
            Utilities.sendAcre(s, usb, AcreOnly, ref counter);
            sendBtn.BackColor = Color.FromArgb(114, 137, 218);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void ReplaceBtn_Click(object sender, EventArgs e)
        {
            if (selectedAcre < 0)
                return;
            if (acreList.FocusedItem == null)
                return;
            byte[] value = BitConverter.GetBytes(Int32.Parse(acreList.Items[acreList.FocusedItem.Index].Text));
            Acre[selectedAcre * 2] = value[0];
            Acre[selectedAcre * 2 + 1] = value[1];
            miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());
            sendBtn.BackColor = Color.Orange;
        }

        private void AllFlatBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Utilities.AllAcreSize; i++)
                Acre[i] = 0x00;
            miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());
            sendBtn.BackColor = Color.Orange;
        }

        private static int MakeLong(short lowPart, short highPart)
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }

        /*
        private void ListViewItem_SetSpacing(ListView listview, short leftPadding, short topPadding)
        {
            const int LVM_FIRST = 0x1000;
            const int LVM_SETICONSPACING = LVM_FIRST + 53;
            _ = SendMessage(listview.Handle, LVM_SETICONSPACING, IntPtr.Zero, (IntPtr)MakeLong(leftPadding, topPadding));
        }
        */

        private void AcreList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (acreList.FocusedItem == null)
                Debug.Print("NULL");
            else
                Debug.Print(acreList.Items[acreList.FocusedItem.Index].Text.ToString());
        }

        private void Bulldozer_FormClosed(object sender, FormClosedEventArgs e)
        {
            formClosed = true;
            MyLog.LogEvent("Bulldozer", "Form Closed");
            this.CloseForm();
        }

        private void BuildingBtn_Click(object sender, EventArgs e)
        {
            BuildingBtn.BackColor = Color.FromArgb(80, 80, 255);
            AcreBtn.BackColor = Color.FromArgb(114, 137, 218);
            TerrainBtn.BackColor = Color.FromArgb(114, 137, 218);

            acrePanel.Visible = false;
            buildingPanel.Visible = true;
            terrainPanel.Visible = false;
            selectedPanel = buildingPanel;
            miniMapBox.Image = null;
            selectedAcre = -1;
        }

        private void AcreBtn_Click(object sender, EventArgs e)
        {
            BuildingBtn.BackColor = Color.FromArgb(114, 137, 218);
            AcreBtn.BackColor = Color.FromArgb(80, 80, 255);
            TerrainBtn.BackColor = Color.FromArgb(114, 137, 218);

            acrePanel.Visible = true;
            buildingPanel.Visible = false;
            terrainPanel.Visible = false;
            selectedPanel = acrePanel;
            miniMapBox.Image = null;
            selectedAcre = -1;
        }

        private void TerrainBtn_Click(object sender, EventArgs e)
        {
            BuildingBtn.BackColor = Color.FromArgb(114, 137, 218);
            AcreBtn.BackColor = Color.FromArgb(114, 137, 218);
            TerrainBtn.BackColor = Color.FromArgb(80, 80, 255);

            acrePanel.Visible = false;
            buildingPanel.Visible = false;
            terrainPanel.Visible = true;
            selectedPanel = terrainPanel;
            miniMapBox.Image = null;
            selectedAcre = -1;
        }

        private void BuildingGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex > -1)
                {
                    if (e.RowIndex == 0)
                        RemoveBuildingBtn.Visible = false;
                    else
                        RemoveBuildingBtn.Visible = true;

                    BuildingControl.Visible = true;

                    MapOrGridViewChange = true;

                    XUpDown.Value = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["X-Coordinate"].Value.ToString());
                    YUpDown.Value = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["Y-Coordinate"].Value.ToString());
                    AUpDown.Value = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["Angle"].Value.ToString());
                    TUpDown.Value = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["Type"].Value.ToString());

                    int index = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["ID"].Value.ToString());
                    lastBuilding = index;

                    byte type = (byte)index;
                    if (index <= BuildingType.Items.Count)
                    {
                        BuildingType.Enabled = true;
                        BuildingType.SelectedIndex = index;
                    }
                    else
                    {
                        //Plaza
                        BuildingType.Enabled = false;
                        BuildingType.SelectedIndex = -1;
                    }

                    int OrgX = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["X-Coordinate"].Value.ToString());
                    int OrgY = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["Y-Coordinate"].Value.ToString());
                    miniMapBox.Image = MiniMap.DrawLargeMarker(OrgX, OrgY, (int)XUpDown.Value, (int)YUpDown.Value, lastBuilding, type);

                    MapOrGridViewChange = false;

                    if (e.ColumnIndex != 2)
                    {
                        buildingGridView.CurrentCell = buildingGridView.Rows[e.RowIndex].Cells[2];
                    }

                    valueUpdated = true;

                    if (index == 27) // incline
                    {
                        inclinePanel.Visible = true;
                        bridgePanel.Visible = false;

                        int Angle = (int)AUpDown.Value;
                        if (Angle <= 3)
                            inclineAngleSelect.SelectedIndex = (int)AUpDown.Value;
                        else
                            inclineAngleSelect.SelectedIndex = 0;

                        inclineTypeSelect.SelectedIndex = TUpDown.Value switch
                        {
                            0 => 0,
                            1 => 1,
                            2 => 2,
                            3 => 3,
                            4 => 4,
                            29 => 5,
                            30 => 6,
                            31 => 7,
                            _ => 0,
                        };
                        if (this.Width > 1110)
                            this.Width = 1110;
                    }
                    else if (index == 26) // bridge
                    {
                        inclinePanel.Visible = false;
                        bridgePanel.Visible = true;

                        int Angle = (int)AUpDown.Value;
                        if (Angle <= 3)
                            BridgeAngleSelect.SelectedIndex = (int)AUpDown.Value;
                        else
                            BridgeAngleSelect.SelectedIndex = 0;

                        switch (TUpDown.Value)
                        {
                            case 0:
                                BridgeTypeSelect.SelectedIndex = 2;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 1:
                                BridgeTypeSelect.SelectedIndex = 2;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 2:
                                BridgeTypeSelect.SelectedIndex = 2;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 3:
                                BridgeTypeSelect.SelectedIndex = 2;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 4:
                                BridgeTypeSelect.SelectedIndex = 2;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 5:
                                BridgeTypeSelect.SelectedIndex = 2;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            //======================================
                            case 6:
                                BridgeTypeSelect.SelectedIndex = 1;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 7:
                                BridgeTypeSelect.SelectedIndex = 1;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 8:
                                BridgeTypeSelect.SelectedIndex = 1;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 9:
                                BridgeTypeSelect.SelectedIndex = 1;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 10:
                                BridgeTypeSelect.SelectedIndex = 1;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 11:
                                BridgeTypeSelect.SelectedIndex = 1;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            //======================================
                            case 12:
                                BridgeTypeSelect.SelectedIndex = 7;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            //======================================
                            case 13:
                                BridgeTypeSelect.SelectedIndex = 0;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 14:
                                BridgeTypeSelect.SelectedIndex = 0;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 15:
                                BridgeTypeSelect.SelectedIndex = 0;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 16:
                                BridgeTypeSelect.SelectedIndex = 0;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 17:
                                BridgeTypeSelect.SelectedIndex = 0;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 18:
                                BridgeTypeSelect.SelectedIndex = 0;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            //======================================
                            case 19:
                                BridgeTypeSelect.SelectedIndex = 7;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 20:
                                BridgeTypeSelect.SelectedIndex = 7;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 21:
                                BridgeTypeSelect.SelectedIndex = 7;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 22:
                                BridgeTypeSelect.SelectedIndex = 7;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 23:
                                BridgeTypeSelect.SelectedIndex = 7;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            //======================================
                            case 24:
                                BridgeTypeSelect.SelectedIndex = 6;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 25:
                                BridgeTypeSelect.SelectedIndex = 5;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            //======================================
                            case 26:
                                BridgeTypeSelect.SelectedIndex = 8;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 27:
                                BridgeTypeSelect.SelectedIndex = 8;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 28:
                                BridgeTypeSelect.SelectedIndex = 8;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 29:
                                BridgeTypeSelect.SelectedIndex = 8;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 30:
                                BridgeTypeSelect.SelectedIndex = 8;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 31:
                                BridgeTypeSelect.SelectedIndex = 8;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            //======================================
                            case 32:
                                BridgeTypeSelect.SelectedIndex = 6;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 33:
                                BridgeTypeSelect.SelectedIndex = 6;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 34:
                                BridgeTypeSelect.SelectedIndex = 6;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 35:
                                BridgeTypeSelect.SelectedIndex = 6;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 36:
                                BridgeTypeSelect.SelectedIndex = 6;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            //======================================
                            case 37:
                                BridgeTypeSelect.SelectedIndex = 3;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 38:
                                BridgeTypeSelect.SelectedIndex = 4;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            //======================================
                            case 39:
                                BridgeTypeSelect.SelectedIndex = 5;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 40:
                                BridgeTypeSelect.SelectedIndex = 5;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 41:
                                BridgeTypeSelect.SelectedIndex = 5;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 42:
                                BridgeTypeSelect.SelectedIndex = 5;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 43:
                                BridgeTypeSelect.SelectedIndex = 5;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            //======================================
                            case 44:
                                BridgeTypeSelect.SelectedIndex = 3;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 45:
                                BridgeTypeSelect.SelectedIndex = 3;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 46:
                                BridgeTypeSelect.SelectedIndex = 3;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 47:
                                BridgeTypeSelect.SelectedIndex = 3;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 48:
                                BridgeTypeSelect.SelectedIndex = 3;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            //======================================
                            case 49:
                                BridgeTypeSelect.SelectedIndex = 4;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 52:
                                BridgeTypeSelect.SelectedIndex = 4;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                            case 53:
                                BridgeTypeSelect.SelectedIndex = 4;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 54:
                                BridgeTypeSelect.SelectedIndex = 4;
                                BridgeLengthUpDown.Value = 4;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            case 55:
                                BridgeTypeSelect.SelectedIndex = 4;
                                BridgeLengthUpDown.Value = 5;
                                BridgeDiagonalToggle.Checked = true;
                                break;
                            //======================================
                            default:
                                BridgeTypeSelect.SelectedIndex = 0;
                                BridgeLengthUpDown.Value = 3;
                                BridgeDiagonalToggle.Checked = false;
                                break;
                        }
                        UpdateBridgeImage((int)TUpDown.Value, (int)AUpDown.Value);
                    }
                    else
                    {
                        bridgePanel.Visible = false;
                        inclinePanel.Visible = false;

                        if (this.Width > 1110)
                            this.Width = 1110;
                    }

                    valueUpdated = false;
                }
            }
        }

        public readonly static Dictionary<byte, string> BuildingName = new()
        {
            {0x0, "Empty"},
            {0x1, "Player House 1"},
            {0x2, "Player House 2"},
            {0x3, "Player House 3"},
            {0x4, "Player House 4"},
            {0x5, "Player House 5"},
            {0x6, "Player House 6"},
            {0x7, "Player House 7"},
            {0x8, "Player House 8"},
            {0x9, "Villager House 1"},
            {0xA, "Villager House 2"},
            {0xB, "Villager House 3"},
            {0xC, "Villager House 4"},
            {0xD, "Villager House 5"},
            {0xE, "Villager House 6"},
            {0xF, "Villager House 7"},
            {0x10, "Villager House 8"},
            {0x11, "Villager House 9"},
            {0x12, "Villager House 10"},
            {0x13, "Nook's Cranny"},
            {0x14, "Resident Services (Building)"},
            {0x15, "Museum"},
            {0x16, "Airport"},
            {0x17, "Resident Services (Tent)"},
            {0x18, "Able Sisters"},
            {0x19, "Campsite"},
            {0x1A, "Bridge"},
            {0x1B, "Incline"},
            {0x1C, "Redd's Treasure Trawler"},
            {0x1D, "Studio"},
        };

        private void SaveAcreBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog file = new()
                {
                    Filter = "New Horizons Acres (*.nha)|*.nha",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                string savepath;

                if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + @"\save";
                else
                    savepath = config.AppSettings.Settings["LastSave"].Value;

                //Debug.Print(savepath);
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

                byte[] AcreOnly = new byte[0x90];
                Buffer.BlockCopy(Acre, 0x0, AcreOnly, 0x0, 0x90);

                File.WriteAllBytes(file.FileName, AcreOnly);

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch
            {
                return;
            }
        }

        private void LoadAcreBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new()
                {
                    Filter = "New Horizons Acres (*.nha)|*.nha| All files (*.*)|*.*",
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

                if (data.Length != 0x90)
                {
                    MyMessageBox.Show("Incorrect file size!" + " (0x" + data.Length.ToString("X") + ")\n" +
                                        "Correct file size should be (0x90)", "Who gave you that file?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Buffer.BlockCopy(data, 0x0, Acre, 0, 0x90);

                miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());
                sendBtn.BackColor = Color.Orange;
            }
            catch
            {
                return;
            }
        }

        private void XUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (MapOrGridViewChange)
                return;

            if (buildingGridView.CurrentCell.RowIndex != -1)
            {
                int OrgX = Int16.Parse(buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["X-Coordinate"].Value.ToString());
                int OrgY = Int16.Parse(buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Y-Coordinate"].Value.ToString());
                int index = BuildingType.SelectedIndex;
                byte type = (byte)index;

                miniMapBox.Image = MiniMap.DrawLargeMarker(OrgX, OrgY, (int)XUpDown.Value, (int)YUpDown.Value, lastBuilding, type);
            }
        }

        private void YUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (MapOrGridViewChange)
                return;

            if (buildingGridView.CurrentCell.RowIndex != -1)
            {
                int OrgX = Int16.Parse(buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["X-Coordinate"].Value.ToString());
                int OrgY = Int16.Parse(buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Y-Coordinate"].Value.ToString());
                int index = BuildingType.SelectedIndex;
                byte type = (byte)index;

                miniMapBox.Image = MiniMap.DrawLargeMarker(OrgX, OrgY, (int)XUpDown.Value, (int)YUpDown.Value, lastBuilding, type);
            }
        }

        private void BuildingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MapOrGridViewChange)
                return;

            if (buildingGridView.CurrentCell.RowIndex != -1)
            {
                int OrgX = Int16.Parse(buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["X-Coordinate"].Value.ToString());
                int OrgY = Int16.Parse(buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Y-Coordinate"].Value.ToString());
                int index = BuildingType.SelectedIndex;
                byte type = (byte)index;

                miniMapBox.Image = MiniMap.DrawLargeMarker(OrgX, OrgY, (int)XUpDown.Value, (int)YUpDown.Value, lastBuilding, type);
            }

            if (BuildingType.SelectedIndex == 27) // incline
            {
                inclinePanel.Visible = true;
                bridgePanel.Visible = false;
            }
            else if (BuildingType.SelectedIndex == 26) // bridge
            {
                inclinePanel.Visible = false;
                bridgePanel.Visible = true;

                TUpDown.Value = 0;
                AUpDown.Value = 0;
                BridgeTypeSelect.SelectedIndex = 0;
                BridgeAngleSelect.SelectedIndex = 0;
                BridgeLengthUpDown.Value = 3;
                BridgeDiagonalToggle.Checked = false;
            }
            else
            {
                inclinePanel.Visible = false;
                bridgePanel.Visible = false;
            }
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            if (buildingGridView.CurrentCell.RowIndex == 0) // Plaza
            {
                plazaEdited = true;
                buildingGridView.Rows[0].Cells["X-Coordinate"].Value = XUpDown.Value;
                buildingGridView.Rows[0].Cells["Y-Coordinate"].Value = YUpDown.Value;

                Acre[0x94] = (byte)XUpDown.Value;
                Acre[0x98] = (byte)YUpDown.Value;
                MiniMap.UpdatePlaza();
                miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());
                miniMapBox.Image = null;
            }
            else if (buildingGridView.CurrentCell.RowIndex != -1)
            {
                buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["X-Coordinate"].Value = XUpDown.Value;
                buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Y-Coordinate"].Value = YUpDown.Value;
                buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Angle"].Value = AUpDown.Value;
                buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Type"].Value = TUpDown.Value;

                int index = BuildingType.SelectedIndex;
                byte type = (byte)index;

                buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["ID"].Value = type;
                if (BuildingName.ContainsKey(type))
                    buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Name"].Value = BuildingName[type];
                else
                    buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Name"].Value = type.ToString("X");


                DataGridViewCellStyle style = new();

                if (BuildingName.ContainsKey(type))
                    style.BackColor = MiniMap.ByteToBuildingColor[type];
                else
                    style.BackColor = Color.Black;

                buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Color"].Style = style;

                Building[(buildingGridView.CurrentCell.RowIndex - 1) * 0x14] = type;
                Building[(buildingGridView.CurrentCell.RowIndex - 1) * 0x14 + 0x2] = (byte)XUpDown.Value;
                Building[(buildingGridView.CurrentCell.RowIndex - 1) * 0x14 + 0x4] = (byte)YUpDown.Value;
                Building[(buildingGridView.CurrentCell.RowIndex - 1) * 0x14 + 0x6] = (byte)AUpDown.Value;
                Building[(buildingGridView.CurrentCell.RowIndex - 1) * 0x14 + 0x8] = (byte)TUpDown.Value;

                miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());
                miniMapBox.Image = null;
            }

            buildingConfirmBtn.BackColor = Color.Orange;
        }

        private void RemoveBuildingBtn_Click(object sender, EventArgs e)
        {
            BuildingType.SelectedIndex = 0;
            XUpDown.Value = 0;
            YUpDown.Value = 0;
            TUpDown.Value = 0;
            AUpDown.Value = 0;

            UpdateBtn_Click(null, null);
        }

        private void BuildingConfirmBtn_Click(object sender, EventArgs e)
        {
            int counter = 0;

            if (plazaEdited)
            {
                byte[] PlazaOnly = new byte[0x8];
                Buffer.BlockCopy(Acre, 0x94, PlazaOnly, 0x0, 0x8);
                Utilities.sendPlaza(s, usb, PlazaOnly, ref counter);
            }

            Utilities.sendBuilding(s, usb, Building, ref counter);
            buildingConfirmBtn.BackColor = Color.FromArgb(114, 137, 218);
            miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());
            miniMapBox.Image = null;

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void SaveBuildingBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog file = new()
                {
                    Filter = "New Horizons Building List (*.nhb)|*.nhb",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

                string savepath;

                if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + @"\save";
                else
                    savepath = config.AppSettings.Settings["LastSave"].Value;

                //Debug.Print(savepath);
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

                File.WriteAllBytes(file.FileName, Building);

                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
            catch
            {
                return;
            }
        }

        private void LoadBuildingBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new()
                {
                    Filter = "New Horizons Building List (*.nhb)|*.nhb| All files (*.*)|*.*",
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

                if (data.Length != 0x398)
                {
                    MyMessageBox.Show("Incorrect file size!" + " (0x" + data.Length.ToString("X") + ")\n" +
                                        "Correct file size should be (0x398)", "Who gave you that file?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Buffer.BlockCopy(data, 0x0, Building, 0, 0x398);

                buildingList = new byte[NumOfBuilding][];
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    buildingList[i] = new byte[BuildingSize];
                    Buffer.BlockCopy(Building, i * BuildingSize, buildingList[i], 0x0, BuildingSize);

                    DataGridViewCellStyle style = new();

                    byte key = buildingList[i][0];
                    if (BuildingName.ContainsKey(key))
                    {
                        buildingGridView.Rows[i + 1].Cells["ID"].Value = buildingList[i][0x0];
                        buildingGridView.Rows[i + 1].Cells["Name"].Value = BuildingName[key];
                        buildingGridView.Rows[i + 1].Cells["X-Coordinate"].Value = buildingList[i][0x2];
                        buildingGridView.Rows[i + 1].Cells["Y-Coordinate"].Value = buildingList[i][0x4];
                        style.BackColor = MiniMap.ByteToBuildingColor[buildingList[i][0x0]];
                    }
                    else
                    {
                        buildingGridView.Rows[i + 1].Cells["ID"].Value = buildingList[i][0x0];
                        buildingGridView.Rows[i + 1].Cells["Name"].Value = key.ToString("X");
                        buildingGridView.Rows[i + 1].Cells["X-Coordinate"].Value = buildingList[i][0x2];
                        buildingGridView.Rows[i + 1].Cells["Y-Coordinate"].Value = buildingList[i][0x4];
                        style.BackColor = Color.Black;
                    }

                    buildingGridView.Rows[i + 1].Cells["Color"].Style = style;
                }

                buildingGridView.CurrentCell = buildingGridView.Rows[0].Cells[2];
                XUpDown.Value = Int16.Parse(buildingGridView.Rows[0].Cells["X-Coordinate"].Value.ToString());
                YUpDown.Value = Int16.Parse(buildingGridView.Rows[0].Cells["Y-Coordinate"].Value.ToString());
                int index = Int16.Parse(buildingGridView.Rows[0].Cells["ID"].Value.ToString());
                byte type = (byte)index;
                if (index <= BuildingType.Items.Count)
                {
                    BuildingType.Enabled = true;
                    BuildingType.SelectedIndex = index;
                }
                else
                {
                    //Plaza
                    BuildingType.Enabled = false;
                    BuildingType.SelectedIndex = -1;
                }

                miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());
                miniMapBox.Image = null;
                buildingConfirmBtn.BackColor = Color.Orange;
            }
            catch
            {
                return;
            }
        }

        private void InclineAngleSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (valueUpdated)
                return;
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex > -1)
                AUpDown.Value = comboBox.SelectedIndex;
        }

        private void InclineTypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (valueUpdated)
                return;
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex > -1)
            {
                TUpDown.Value = comboBox.SelectedIndex switch
                {
                    0 => 0,
                    1 => 1,
                    2 => 2,
                    3 => 3,
                    4 => 4,
                    5 => 29,
                    6 => 30,
                    7 => 31,
                    _ => (decimal)0,
                };
            }
        }

        private void BridgeAngleSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (valueUpdated)
                return;
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex > -1)
                AUpDown.Value = comboBox.SelectedIndex;

            UpdateBridgeImage((int)TUpDown.Value, (int)AUpDown.Value);
        }

        private void BridgeTypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (valueUpdated)
                return;
            BridgeSettingChanged();
        }

        private void BridgeLengthUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (valueUpdated)
                return;
            BridgeSettingChanged();
        }

        private void BridgeDiagonalToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (valueUpdated)
                return;
            BridgeSettingChanged();
        }

        private void BridgeSettingChanged()
        {
            switch (BridgeTypeSelect.SelectedIndex)
            {
                case 0: // Log
                    switch ((int)BridgeLengthUpDown.Value)
                    {
                        case 3:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 16;
                            else
                                TUpDown.Value = 14;
                            break;
                        case 4:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 17;
                            else
                                TUpDown.Value = 13;
                            break;
                        case 5:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 18;
                            else
                                TUpDown.Value = 15;
                            break;
                        default:
                            TUpDown.Value = 14;
                            break;
                    }
                    break;
                case 1: // Suspension
                    switch ((int)BridgeLengthUpDown.Value)
                    {
                        case 3:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 9;
                            else
                                TUpDown.Value = 7;
                            break;
                        case 4:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 10;
                            else
                                TUpDown.Value = 6;
                            break;
                        case 5:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 11;
                            else
                                TUpDown.Value = 8;
                            break;
                        default:
                            TUpDown.Value = 7;
                            break;
                    }
                    break;
                case 2: // Stone
                    switch ((int)BridgeLengthUpDown.Value)
                    {
                        case 3:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 3;
                            else
                                TUpDown.Value = 0;
                            break;
                        case 4:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 4;
                            else
                                TUpDown.Value = 1;
                            break;
                        case 5:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 5;
                            else
                                TUpDown.Value = 2;
                            break;
                        default:
                            TUpDown.Value = 0;
                            break;
                    }
                    break;
                case 3: // Wooden
                    switch ((int)BridgeLengthUpDown.Value)
                    {
                        case 3:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 46;
                            else
                                TUpDown.Value = 44;
                            break;
                        case 4:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 47;
                            else
                                TUpDown.Value = 37;
                            break;
                        case 5:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 48;
                            else
                                TUpDown.Value = 45;
                            break;
                        default:
                            TUpDown.Value = 44;
                            break;
                    }
                    break;
                case 4: // Brick
                    switch ((int)BridgeLengthUpDown.Value)
                    {
                        case 3:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 53;
                            else
                                TUpDown.Value = 49;
                            break;
                        case 4:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 54;
                            else
                                TUpDown.Value = 38;
                            break;
                        case 5:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 55;
                            else
                                TUpDown.Value = 52;
                            break;
                        default:
                            TUpDown.Value = 49;
                            break;
                    }
                    break;
                case 5: // Iron
                    switch ((int)BridgeLengthUpDown.Value)
                    {
                        case 3:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 41;
                            else
                                TUpDown.Value = 39;
                            break;
                        case 4:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 42;
                            else
                                TUpDown.Value = 25;
                            break;
                        case 5:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 43;
                            else
                                TUpDown.Value = 40;
                            break;
                        default:
                            TUpDown.Value = 39;
                            break;
                    }
                    break;
                case 6: // Red Zen
                    switch ((int)BridgeLengthUpDown.Value)
                    {
                        case 3:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 34;
                            else
                                TUpDown.Value = 32;
                            break;
                        case 4:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 35;
                            else
                                TUpDown.Value = 24;
                            break;
                        case 5:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 36;
                            else
                                TUpDown.Value = 33;
                            break;
                        default:
                            TUpDown.Value = 32;
                            break;
                    }
                    break;
                case 7: // Zen
                    switch ((int)BridgeLengthUpDown.Value)
                    {
                        case 3:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 21;
                            else
                                TUpDown.Value = 19;
                            break;
                        case 4:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 22;
                            else
                                TUpDown.Value = 12;
                            break;
                        case 5:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 23;
                            else
                                TUpDown.Value = 20;
                            break;
                        default:
                            TUpDown.Value = 19;
                            break;
                    }
                    break;
                case 8: // Under Construction
                    switch ((int)BridgeLengthUpDown.Value)
                    {
                        case 3:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 29;
                            else
                                TUpDown.Value = 26;
                            break;
                        case 4:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 30;
                            else
                                TUpDown.Value = 27;
                            break;
                        case 5:
                            if (BridgeDiagonalToggle.Checked)
                                TUpDown.Value = 31;
                            else
                                TUpDown.Value = 28;
                            break;
                        default:
                            TUpDown.Value = 26;
                            break;
                    }
                    break;
                default:
                    TUpDown.Value = 0;
                    break;
            }
            UpdateBridgeImage((int)TUpDown.Value, (int)AUpDown.Value);
        }

        private void UpdateBridgeImage(int value, int angle)
        {
            string ImagePath = Utilities.BridgeImagePath + Utilities.precedingZeros(value.ToString(), 2) + ".png";
            if (File.Exists(ImagePath))
            {
                BridgeImage.Visible = true;
                Image image = new Bitmap(Image.FromFile(ImagePath), new Size(BridgeImage.Width, BridgeImage.Height));
                switch (angle)
                {
                    case 0:
                        break;
                    case 1:
                        image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    case 2:
                        image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 3:
                        image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    default:
                        break;
                }
                BridgeImage.Image = image;
                this.Width = 1320;
            }
        }

        private void FlattenAllBtn_Click(object sender, EventArgs e)
        {
            MyWarning flattenWarning = new(s, sound, MiniMap);
            flattenWarning.ShowDialog();
            miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());
            miniMapBox.Image = null;
        }

        private void removeAllRoadsBtn_Click(object sender, EventArgs e)
        {
            if (MyMessageBox.Show("Are you sure you would like to remove ALL roads on your island?", "Long road ahead...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                LoadingPanel.Visible = true;
                AcreBtn.Visible = false;
                BuildingBtn.Visible = false;
                TerrainBtn.Visible = false;
                terrainPanel.Visible = false;

                Thread RemoveRoadThread = new(delegate () { RemoveRoad(); });
                RemoveRoadThread.Start();
            }
        }

        private void RemoveRoad()
        {
            byte[] CurrentTerrainData = Utilities.getTerrain(s, usb);

            int counter = 0;

            while (Utilities.IsAboutToSave(s, usb, 10))
            {
                if (counter > 15)
                    break;
                Thread.Sleep(2000);
                counter++;
            }

            counter = 0;

            byte[] EmptyRoadData = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            for (int i = 0; i < Utilities.MapTileCount16x16; i++)
            {
                Buffer.BlockCopy(EmptyRoadData, 0, CurrentTerrainData, i * Utilities.TerrainTileSize + 6, 6);
            }

            MiniMap.UpdateTerrain(CurrentTerrainData);

            Utilities.sendTerrain(s, usb, CurrentTerrainData, ref counter);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            if (!formClosed)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    LoadingPanel.Visible = false;
                    AcreBtn.Visible = true;
                    BuildingBtn.Visible = true;
                    TerrainBtn.Visible = true;
                    terrainPanel.Visible = true;

                    miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());
                    miniMapBox.Image = null;
                });
            }
        }

        private void removeAllDesignBtn_Click(object sender, EventArgs e)
        {
            if (MyMessageBox.Show("Are you sure you would like to remove ALL custom designs on your island?", "Deep Cleansing Scrub...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                LoadingPanel.Visible = true;
                AcreBtn.Visible = false;
                BuildingBtn.Visible = false;
                TerrainBtn.Visible = false;
                terrainPanel.Visible = false;

                Thread RemoveDesignThread = new(delegate () { RemoveDesign(); });
                RemoveDesignThread.Start();
            }
        }

        private void RemoveDesign()
        {
            int counter = 0;

            while (Utilities.IsAboutToSave(s, usb, 10))
            {
                if (counter > 15)
                    break;
                Thread.Sleep(2000);
                counter++;
            }

            counter = 0;

            byte[] newCustomMap = new byte[numOfRow * numOfColumn * 2];

            for (int i = 0; i < numOfColumn; i++)
            {
                for (int j = 0; j < numOfRow; j++)
                {
                    byte[] EmptyCustomDesignData = new byte[] { 0x00, 0xF8 };
                    Buffer.BlockCopy(EmptyCustomDesignData, 0, newCustomMap, (i * numOfRow * 2) + (j * 2), 2);
                }
            }

            Utilities.sendCustomMap(s, usb, newCustomMap, ref counter);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            if (!formClosed)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    LoadingPanel.Visible = false;
                    AcreBtn.Visible = true;
                    BuildingBtn.Visible = true;
                    TerrainBtn.Visible = true;
                    terrainPanel.Visible = true;

                    MiniMap.UpdateTerrain(null, newCustomMap);
                    miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());
                    miniMapBox.Image = null;
                });
            }
        }

        private void SaveTerrianBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog file = new()
                {
                    Filter = "New Horizons Terrain (*.nht)|*.nht",
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

                terrainPanel.Enabled = false;
                PleaseWaitPanel.Visible = true;

                Thread SaveThread = new(delegate () { SaveTerrain(file); });
                SaveThread.Start();
            }
            catch
            {
                return;
            }
        }

        private void SaveTerrain(SaveFileDialog file)
        {
            try
            {
                byte[] terrain = Utilities.getTerrain(s, usb);

                File.WriteAllBytes(file.FileName, terrain);

                if (!formClosed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        PleaseWaitPanel.Visible = false;
                        terrainPanel.Enabled = true;
                    });
                }
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Bulldozer", "SaveTerrain: " + ex.Message.ToString());
                MyMessageBox.Show(ex.Message.ToString(), "drunk, fix later");
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void LoadTerrianBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new()
                {
                    Filter = "New Horizons Terrain (*.nht)|*.nht| All files (*.*)|*.*",
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

                if (data.Length != Utilities.AllTerrainSize)
                {
                    MyMessageBox.Show("Incorrect file size!" + " (0x" + data.Length.ToString("X") + ")\n" +
                                        "Correct file size should be (0x" + Utilities.AllTerrainSize.ToString("X") + ")", "Who gave you that file?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                PleaseWaitPanel.Visible = true;
                terrainPanel.Enabled = false;

                MiniMap.UpdateTerrain(data);

                Thread LoadThread = new(delegate () { LoadTerrain(data); });
                LoadThread.Start();
            }
            catch
            {
                return;
            }
        }

        private void LoadTerrain(byte[] terrain)
        {
            try
            {
                int counter = 0;

                while (Utilities.IsAboutToSave(s, usb, 10))
                {
                    if (counter > 15)
                        break;
                    Thread.Sleep(2000);
                    counter++;
                }

                counter = 0;

                Utilities.sendTerrain(s, usb, terrain, ref counter);

                if (!formClosed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        PleaseWaitPanel.Visible = false;
                        terrainPanel.Enabled = true;
                        miniMapBox.BackgroundImage = MiniMap.CombineMap(MiniMap.DrawFullBackground(), MiniMap.DrawEdge());
                        miniMapBox.Image = null;
                    });
                }
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Bulldozer", "LoadTerrain: " + ex.Message.ToString());
                MyMessageBox.Show(ex.Message.ToString(), "drunk, fix later");
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void MiniMapBox_MouseLeave(object sender, EventArgs e)
        {
            MapToolTip.Hide(miniMapBox);
            lastDisplayTooltip = new Point(-1, -1);
        }
    }
}
