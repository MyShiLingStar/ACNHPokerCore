using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{

    public partial class Bulldozer : Form
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private static Socket s;
        private static USBBot usb;
        private bool sound;
        private miniMap MiniMap = null;

        private byte[] Layer1;
        private byte[] Acre;
        private byte[] Building;
        private byte[] Terrain;

        private byte[][] buildingList = null;
        private const int BuildingSize = 0x14;
        private const int NumOfBuilding = 46;

        private int counter;
        private int selectedAcre;
        private int selectedAcreValue;

        private Panel selectedPanel;
        private bool MapOrGridViewChange = false;
        private bool plazaEdited = false;
        private bool valueUpdated = false;

        public event CloseHandler closeForm;

        public Bulldozer(Socket S, USBBot USB, bool Sound)
        {
            s = S;
            usb = USB;
            sound = Sound;

            InitializeComponent();

            Thread LoadThread = new Thread(delegate () { loadMap(); });
            LoadThread.Start();
        }

        private void loadMap()
        {
            var layer1Address = Utilities.mapZero;

            var imageList = new ImageList();
            imageList.ImageSize = new Size(64, 64);
            acreList.LargeImageList = imageList;
            for (int i = 0; i < 0xF5; i++)
            {
                imageList.Images.Add(i.ToString(), miniMap.getAcreImage(i, 4));
                acreList.Items.Add(i.ToString(), i.ToString());
            }
            ListViewItem_SetSpacing(acreList, 64 + 15, 80 + 15);

            if (s != null || usb != null)
            {
                //Layer1 = Utilities.getMapLayer(s, usb, layer1Address, ref counter);
                Layer1 = null;
                //Layer2 = Utilities.getMapLayer(s, bot, layer2Address, ref counter);
                Acre = Utilities.getAcre(s, usb);
                Building = Utilities.getBuilding(s, usb);
                Terrain = Utilities.getTerrain(s, usb);

                if (Acre != null && Building != null && Terrain != null)
                {
                    if (MiniMap == null)
                        MiniMap = new miniMap(Layer1, Acre, Building, Terrain, 4);
                }
                else
                    throw new NullReferenceException("Acre/Building/Terrain");

                selectedPanel = acrePanel;

                miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawFullBackground(), MiniMap.drawEdge());

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
                BuildingX.HeaderText = "X-Coordinate";
                BuildingX.Name = "X-Coordinate";
                BuildingX.SortMode = DataGridViewColumnSortMode.NotSortable;
                BuildingX.Width = 60;
                BuildingY.HeaderText = "Y-Coordinate";
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

                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    style.BackColor = Color.DarkSalmon;
                    buildingGridView.Rows[0].Cells["Color"].Style = style;
                }

                if (Building != null)
                    fillBuilding();

                buildingGridView.CurrentCell = buildingGridView.Rows[0].Cells[2];
            }

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

        private void fillBuilding()
        {
            if (Building != null)
            {
                buildingList = new byte[NumOfBuilding][];
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    buildingList[i] = new byte[BuildingSize];
                    Buffer.BlockCopy(Building, i * BuildingSize, buildingList[i], 0x0, BuildingSize);

                    DataGridViewCellStyle style = new DataGridViewCellStyle();

                    byte key = buildingList[i][0];
                    if (BuildingName.ContainsKey(key))
                    {
                        buildingGridView.Rows.Add(buildingList[i][0x0], "", BuildingName[key], buildingList[i][0x2], buildingList[i][0x4], buildingList[i][0x6], buildingList[i][0x8]);
                        style.BackColor = miniMap.ByteToBuildingColor[buildingList[i][0x0]];
                    }
                    else
                    {
                        buildingGridView.Rows.Add(buildingList[i][0x0], "", "???", buildingList[i][0x2], buildingList[i][0x4], buildingList[i][0x6], buildingList[i][0x8]);
                        style.BackColor = Color.Black;
                    }

                    buildingGridView.Rows[buildingGridView.RowCount - 1].Cells["Color"].Style = style;
                }
            }
        }

        private void miniMapBox_MouseDown(object sender, MouseEventArgs e)
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
                    miniMapBox.Image = miniMap.drawSelectAcre(AcreX, AcreY);
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
                            miniMapBox.Image = miniMap.drawLargeMarker(OrgX, OrgY, Realx, Realy, type);
                        }
                        else
                            miniMapBox.Image = miniMap.drawLargeMarker(Realx, Realy, Realx, Realy);

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
                selectedAcreValue = Acre[selectedAcre * 2];
                selectedAcreValueBox.Text = "0x" + Acre[selectedAcre * 2].ToString("X");
            }
        }

        private void miniMapBox_MouseMove(object sender, MouseEventArgs e)
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
                            miniMapBox.Image = miniMap.drawLargeMarker(OrgX, OrgY, Realx, Realy, type);
                        }
                        else
                            miniMapBox.Image = miniMap.drawLargeMarker(Realx, Realy, Realx, Realy);

                        MapOrGridViewChange = false;
                    }
                }
            }
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            byte[] AcreOnly = new byte[0x90];
            Buffer.BlockCopy(Acre, 0x0, AcreOnly, 0x0, 0x90);
            int counter = 0;
            Utilities.sendAcre(s, usb, AcreOnly, ref counter);
            sendBtn.BackColor = Color.FromArgb(114, 137, 218);

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void replaceBtn_Click(object sender, EventArgs e)
        {
            if (selectedAcre < 0)
                return;
            if (acreList.FocusedItem == null)
                return;
            Acre[selectedAcre * 2] = (byte)Int32.Parse(acreList.Items[acreList.FocusedItem.Index].Text);
            miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawFullBackground(), MiniMap.drawEdge());
            sendBtn.BackColor = Color.Orange;
        }

        private void allFlatBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Utilities.AllAcreSize; i++)
                Acre[i] = 0x00;
            miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawFullBackground(), MiniMap.drawEdge());
            sendBtn.BackColor = Color.Orange;
        }

        private int MakeLong(short lowPart, short highPart)
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }

        private void ListViewItem_SetSpacing(ListView listview, short leftPadding, short topPadding)
        {
            this.Invoke((MethodInvoker)delegate
            {
                const int LVM_FIRST = 0x1000;
                const int LVM_SETICONSPACING = LVM_FIRST + 53;
                SendMessage(listview.Handle, LVM_SETICONSPACING, IntPtr.Zero, (IntPtr)MakeLong(leftPadding, topPadding));
            });
        }

        private void acreList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (acreList.FocusedItem == null)
                System.Diagnostics.Debug.Print("NULL");
            else
                System.Diagnostics.Debug.Print(acreList.Items[acreList.FocusedItem.Index].Text.ToString());
        }

        private void Bulldozer_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyLog.logEvent("Bulldozer", "Form Closed");
            this.closeForm();
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

        private void buildingGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left)
            {
                if (e.RowIndex > -1)
                {
                    BuildingControl.Visible = true;

                    MapOrGridViewChange = true;

                    XUpDown.Value = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["X-Coordinate"].Value.ToString());
                    YUpDown.Value = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["Y-Coordinate"].Value.ToString());
                    AUpDown.Value = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["Angle"].Value.ToString());
                    TUpDown.Value = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["Type"].Value.ToString());

                    int index = Int16.Parse(buildingGridView.Rows[e.RowIndex].Cells["ID"].Value.ToString());
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
                    miniMapBox.Image = miniMap.drawLargeMarker(OrgX, OrgY, (int)XUpDown.Value, (int)YUpDown.Value, type);

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

                        switch (TUpDown.Value)
                        {
                            case 0:
                                inclineTypeSelect.SelectedIndex = 0;
                                break;
                            case 1:
                                inclineTypeSelect.SelectedIndex = 1;
                                break;
                            case 2:
                                inclineTypeSelect.SelectedIndex = 2;
                                break;
                            case 3:
                                inclineTypeSelect.SelectedIndex = 3;
                                break;
                            case 4:
                                inclineTypeSelect.SelectedIndex = 4;
                                break;
                            case 29:
                                inclineTypeSelect.SelectedIndex = 5;
                                break;
                            case 30:
                                inclineTypeSelect.SelectedIndex = 6;
                                break;
                            case 31:
                                inclineTypeSelect.SelectedIndex = 7;
                                break;
                            default:
                                inclineTypeSelect.SelectedIndex = 0;
                                break;
                        }

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

        public static Dictionary<byte, string> BuildingName = new Dictionary<byte, string>
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

        private void saveAcreBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog file = new SaveFileDialog()
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

        private void loadAcreBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog()
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

                miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawFullBackground(), MiniMap.drawEdge());
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

                miniMapBox.Image = miniMap.drawLargeMarker(OrgX, OrgY, (int)XUpDown.Value, (int)YUpDown.Value, type);
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

                miniMapBox.Image = miniMap.drawLargeMarker(OrgX, OrgY, (int)XUpDown.Value, (int)YUpDown.Value, type);
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

                miniMapBox.Image = miniMap.drawLargeMarker(OrgX, OrgY, (int)XUpDown.Value, (int)YUpDown.Value, type);
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

        private void updateBtn_Click(object sender, EventArgs e)
        {
            if (buildingGridView.CurrentCell.RowIndex == 0) // Plaza
            {
                plazaEdited = true;
                buildingGridView.Rows[0].Cells["X-Coordinate"].Value = XUpDown.Value;
                buildingGridView.Rows[0].Cells["Y-Coordinate"].Value = YUpDown.Value;

                Acre[0x94] = (byte)XUpDown.Value;
                Acre[0x98] = (byte)YUpDown.Value;
                MiniMap.updatePlaza();
                miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawFullBackground(), MiniMap.drawEdge());
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
                buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Name"].Value = BuildingName[type];


                DataGridViewCellStyle style = new DataGridViewCellStyle();

                if (BuildingName.ContainsKey(type))
                    style.BackColor = miniMap.ByteToBuildingColor[type];
                else
                    style.BackColor = Color.Black;

                buildingGridView.Rows[buildingGridView.CurrentCell.RowIndex].Cells["Color"].Style = style;

                Building[(buildingGridView.CurrentCell.RowIndex - 1) * 0x14] = type;
                Building[(buildingGridView.CurrentCell.RowIndex - 1) * 0x14 + 0x2] = (byte)XUpDown.Value;
                Building[(buildingGridView.CurrentCell.RowIndex - 1) * 0x14 + 0x4] = (byte)YUpDown.Value;
                Building[(buildingGridView.CurrentCell.RowIndex - 1) * 0x14 + 0x6] = (byte)AUpDown.Value;
                Building[(buildingGridView.CurrentCell.RowIndex - 1) * 0x14 + 0x8] = (byte)TUpDown.Value;

                miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawFullBackground(), MiniMap.drawEdge());
                miniMapBox.Image = null;
            }

            buildingConfirmBtn.BackColor = Color.Orange;
        }

        private void buildingConfirmBtn_Click(object sender, EventArgs e)
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
            miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawFullBackground(), MiniMap.drawEdge());
            miniMapBox.Image = null;

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void saveBuildingBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog file = new SaveFileDialog()
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

        private void loadBuildingBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog()
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

                    DataGridViewCellStyle style = new DataGridViewCellStyle();

                    byte key = buildingList[i][0];
                    if (BuildingName.ContainsKey(key))
                    {
                        buildingGridView.Rows[i + 1].Cells["ID"].Value = buildingList[i][0x0];
                        buildingGridView.Rows[i + 1].Cells["Name"].Value = BuildingName[key];
                        buildingGridView.Rows[i + 1].Cells["X-Coordinate"].Value = buildingList[i][0x2];
                        buildingGridView.Rows[i + 1].Cells["Y-Coordinate"].Value = buildingList[i][0x4];
                        style.BackColor = miniMap.ByteToBuildingColor[buildingList[i][0x0]];
                    }
                    else
                    {
                        buildingGridView.Rows[i + 1].Cells["ID"].Value = buildingList[i][0x0];
                        buildingGridView.Rows[i + 1].Cells["Name"].Value = "???";
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

                miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawFullBackground(), MiniMap.drawEdge());
                miniMapBox.Image = null;
                buildingConfirmBtn.BackColor = Color.Orange;
            }
            catch
            {
                return;
            }
        }

        private void inclineAngleSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (valueUpdated)
                return;
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex > -1)
                AUpDown.Value = comboBox.SelectedIndex;
        }

        private void inclineTypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (valueUpdated)
                return;
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedIndex > -1)
            {
                switch (comboBox.SelectedIndex)
                {
                    case 0:
                        TUpDown.Value = 0;
                        break;
                    case 1:
                        TUpDown.Value = 1;
                        break;
                    case 2:
                        TUpDown.Value = 2;
                        break;
                    case 3:
                        TUpDown.Value = 3;
                        break;
                    case 4:
                        TUpDown.Value = 4;
                        break;
                    case 5:
                        TUpDown.Value = 29;
                        break;
                    case 6:
                        TUpDown.Value = 30;
                        break;
                    case 7:
                        TUpDown.Value = 31;
                        break;
                    default:
                        TUpDown.Value = 0;
                        break;
                }
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
            bridgeSettingChanged();
        }

        private void BridgeLengthUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (valueUpdated)
                return;
            bridgeSettingChanged();
        }

        private void BridgeDiagonalToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (valueUpdated)
                return;
            bridgeSettingChanged();
        }

        private void bridgeSettingChanged()
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

        private void flattenAllBtn_Click(object sender, EventArgs e)
        {
            MyWarning flattenWarning = new MyWarning(s, sound, MiniMap);
            flattenWarning.ShowDialog();
            miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawFullBackground(), MiniMap.drawEdge());
            miniMapBox.Image = null;
        }

        private void saveTerrianBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog file = new SaveFileDialog()
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

                Thread SaveThread = new Thread(delegate () { SaveTerrain(file); });
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

                this.Invoke((MethodInvoker)delegate
                {
                    PleaseWaitPanel.Visible = false;
                    terrainPanel.Enabled = true;
                });
            }
            catch (Exception ex)
            {
                MyLog.logEvent("Bulldozer", "SaveTerrain: " + ex.Message.ToString());
                MyMessageBox.Show(ex.Message.ToString(), "drunk, fix later");
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void loadTerrianBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog()
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

                MiniMap.updateTerrain(data);

                Thread LoadThread = new Thread(delegate () { LoadTerrain(data); });
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

                while (isAboutToSave(10))
                {
                    if (counter > 15)
                        break;
                    Thread.Sleep(2000);
                    counter++;
                }

                counter = 0;

                Utilities.sendTerrain(s, usb, terrain, ref counter);

                this.Invoke((MethodInvoker)delegate
                {
                    PleaseWaitPanel.Visible = false;
                    terrainPanel.Enabled = true;
                    miniMapBox.BackgroundImage = MiniMap.combineMap(MiniMap.drawFullBackground(), MiniMap.drawEdge());
                    miniMapBox.Image = null;
                });
            }
            catch (Exception ex)
            {
                MyLog.logEvent("Bulldozer", "LoadTerrain: " + ex.Message.ToString());
                MyMessageBox.Show(ex.Message.ToString(), "drunk, fix later");
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private bool isAboutToSave(int second)
        {
            byte[] b = Utilities.getSaving(s, usb);

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
    }
}
