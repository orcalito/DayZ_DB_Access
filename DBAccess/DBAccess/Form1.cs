using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using MySql.Data.MySqlClient;

namespace DBAccess
{
    public partial class Form1 : Form
    {
        private MySqlConnection cnx;
        private bool bConnected = false;
        private static int bUserAction = 0;
        private static Mutex mtxUpdateDB = new Mutex();
        private static Mutex mtxUseDS = new Mutex();
        public DlgUpdateIcons dlgUpdateIcons;
        public myConfig mycfg = new myConfig();
        private Bitmap bitmapHQ;
        private DataSet dsInstances = new DataSet();
        private DataSet dsTents = new DataSet();
        private DataSet dsAlivePlayers = new DataSet();
        private DataSet dsOnlinePlayers = new DataSet();
        private DataSet dsVehicles = new DataSet();
        private DataSet dsVehicleSpawnPoints = new DataSet();
        private float zoomFactor;
        private List<iconDB> listIcons = new List<iconDB>();
        private RadioButton currDisplayedItems;
        private Point lastPositionMouse;
        private Point MapPos = new Point(0, 0);
        private Size MapSize = new Size(400, 400);
        private Point MapPosTmp = new Point(0, 0);
        private Size offsetMap = new Size(0, 0);
        private Dictionary<UInt64, UIDdata> dicUIDdata = new Dictionary<UInt64, UIDdata>();

        private List<iconDB> iconsDB = new List<iconDB>();
        private List<InvisibleControl> iconPlayers = new List<InvisibleControl>();
        private List<InvisibleControl> iconVehicles = new List<InvisibleControl>();
        private List<InvisibleControl> iconTents = new List<InvisibleControl>();

        private string configPath;
        private string configFile;

        public Form1()
        {
            InitializeComponent();

            configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DayZDBAccess";
            configFile = configPath + "\\config.xml";

            try
            {
                Assembly asb = System.Reflection.Assembly.GetExecutingAssembly();
                Version version = asb.GetName().Version;
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                    version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;

                this.Text = asb.GetName().Name + " - v" + version.ToString();
            }
            catch
            {
                this.Text = "DayZ DB Access unknown version";
            }

            this.Resize += Form1Resize;
            this.MouseWheel += imgMap_MouseWheel;

            //
            LoadConfigFile();

            bgWorker.RunWorkerAsync();

            Enable(false);
        }
        void ApplyMapChanges()
        {
            if (imgMap.Image == null)
                return;

            Size sizePanel = splitContainer1.Panel1.Size;

            Point halfPanel = new Point((int)(sizePanel.Width * 0.5f), (int)(sizePanel.Height * 0.5f));

            MapPos.X = Math.Min(halfPanel.X - 16, MapPos.X);
            MapPos.Y = Math.Min(halfPanel.Y - 16, MapPos.Y);

            MapPos.X = Math.Max(halfPanel.X - MapSize.Width + 16, MapPos.X);
            MapPos.Y = Math.Max(halfPanel.Y - MapSize.Height + 16, MapPos.Y);

            imgMap.Size = MapSize;
            imgMap.Location = MapPos;
            
            RefreshIcons();
        }
        void Form1Resize(object sender, EventArgs e)
        {
            ApplyMapChanges();
        }
        private void imgMap_MouseClick(object sender, MouseEventArgs e)
        {
            lastPositionMouse = MousePosition;
            System.Threading.Interlocked.CompareExchange(ref bUserAction, 1, 0);
        }
        private void imgMap_MouseUp(object sender, MouseEventArgs e)
        {
            System.Threading.Interlocked.CompareExchange(ref bUserAction, 0, 1);
        }
        private void imgMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left))
            {
                offsetMap = new Size((int)(MousePosition.X - lastPositionMouse.X), (int)(MousePosition.Y - lastPositionMouse.Y));

                MapPos = Point.Add(MapPosTmp, offsetMap);
                ApplyMapChanges();
            }
            else
            {
                MapPosTmp = MapPos;
                offsetMap.Width = offsetMap.Height = 0;
            }
        }
        private void imgMap_MouseWheel(object sender, MouseEventArgs e)
        {
            if (imgMap.Image == null)
                return;

            System.Threading.Interlocked.CompareExchange(ref bUserAction, 1, 0);

            float newZoom = zoomFactor * (1.0f + ((e.Delta / 120.0f) * 0.1f));
            //
            MapSize = new Size((int)(imgMap.Image.Size.Width * newZoom), (int)(imgMap.Image.Size.Height * newZoom));
            MapSize = new Size(Math.Max(400, Math.Min(15000, MapSize.Width)),
                                Math.Max(400, Math.Min(15000, MapSize.Height)));
            //
            newZoom = MapSize.Width / (float)imgMap.Image.Size.Width;

            float deltaZ = newZoom - zoomFactor;

            if( deltaZ != 0.0f )
            {
                Point mousePos = new Point(e.Location.X - MapPos.X, e.Location.Y - MapPos.Y);

                //
                Size delta = new Size((int)(mousePos.X * deltaZ / zoomFactor), (int)(mousePos.Y * deltaZ / zoomFactor));
                MapPos = Point.Subtract(MapPos, delta);

                zoomFactor = newZoom;

                ApplyMapChanges();
            }

            System.Threading.Interlocked.CompareExchange(ref bUserAction, 0, 1);
        }
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            mtxUpdateDB.WaitOne();

            this.Cursor = Cursors.WaitCursor;

            //  "Server=localhost;Database=testdb;Uid=root;Pwd=pass;";
            string strCnx = "";
            strCnx += "Server=" + textBoxURL.Text;
            strCnx += ";Port=" + textBoxPort.Text;
            strCnx += ";Database=" + textBoxBaseName.Text;
            strCnx += ";Uid=" + textBoxUser.Text;
            strCnx += ";Pwd=" + textBoxPassword.Text + ";";

            mycfg.instance_id = textBoxInstanceId.Text;

            cnx = new MySqlConnection(strCnx);

            try
            {
                cnx.Open();

                Enable(true);

                DB_OnConnection();
                DB_OnRefresh();

            }
            catch(Exception ex)
            {
                textBoxCmdStatus.Text = ex.ToString();
                Enable(false);
            }

            if (bConnected)
                textBoxStatus.Text = "connected";
            else
                textBoxStatus.Text = "Error !";

            this.Cursor = Cursors.Arrow;

            mtxUpdateDB.ReleaseMutex();
        }
        private void CloseConnection()
        {
            mtxUseDS.WaitOne();

            propertyGrid1.SelectedObject = null;

            currDisplayedItems = null;

            dsTents.Clear();
            dsAlivePlayers.Clear();
            dsOnlinePlayers.Clear();
            dsVehicleSpawnPoints.Clear();
            dsVehicles.Clear();

            try
            {
                foreach (iconDB idb in listIcons)
                    imgMap.Controls.Remove(idb.icon);

                foreach( KeyValuePair<UInt64,UIDdata> pair in dicUIDdata)
                    pair.Value.path.Reset();

                listIcons.Clear();

                Enable(false);
                textBoxStatus.Text = "disconnected";
            }
            catch (Exception)
            {
                Enable(false);
            }

            mtxUseDS.ReleaseMutex();

            mtxUpdateDB.WaitOne();

            if (cnx != null)
                cnx.Close();
            
            mtxUpdateDB.ReleaseMutex();
        }
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!bConnected)
                return;

            this.Cursor = Cursors.WaitCursor;
            RadioButton senderRB = sender as RadioButton;
            if (senderRB.Checked)
            {
                propertyGrid1.SelectedObject = null;

                currDisplayedItems = senderRB;

                BuildIcons();
            }
            this.Cursor = Cursors.Arrow;
        }
        private void BuildIcons()
        {
            mtxUseDS.WaitOne();
            try
            {
                if (bConnected)
                {
                    tbOnlinePlayers.Text = dsOnlinePlayers.Tables[0].Rows.Count.ToString();
                    tbAlivePlayers.Text = dsAlivePlayers.Tables[0].Rows.Count.ToString();
                    tbVehicles.Text = dsVehicles.Tables[0].Rows.Count.ToString();
                    tbVehicleSpawn.Text = dsVehicleSpawnPoints.Tables[0].Rows.Count.ToString();
                    tbTents.Text = dsTents.Tables[0].Rows.Count.ToString();

                    if ((propertyGrid1.SelectedObject != null) && (propertyGrid1.SelectedObject is PropObjectBase))
                    {
                        PropObjectBase obj = propertyGrid1.SelectedObject as PropObjectBase;

                        obj.Rebuild();

                        propertyGrid1.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                textBoxCmdStatus.Text = ex.ToString();
            }
            mtxUseDS.ReleaseMutex();

            if (currDisplayedItems == null)
                return;

            mtxUseDS.WaitOne();

            try
            {
                foreach (iconDB idb in listIcons)
                    imgMap.Controls.Remove(idb.icon);

                listIcons.Clear();

                switch (currDisplayedItems.Name)
                {
                    case "radioButtonOnline": BuildOnlineIcons(); break;
                    case "radioButtonAlive": BuildAliveIcons(); break;
                    case "radioButtonVehicles": BuildVehicleIcons(); break;
                    case "radioButtonSpawn": BuildSpawnIcons(); break;
                    case "radioButtonTents": BuildTentIcons(); break;
                }

                RefreshIcons();
            }
            catch (Exception ex)
            {
                textBoxCmdStatus.Text = ex.ToString();
            }

            mtxUseDS.ReleaseMutex();
        }
        private void OnPlayerClick(object sender, EventArgs e)
        {
            InvisibleControl pb = sender as InvisibleControl;

            Survivor player = new Survivor(pb.Tag as iconDB);
            player.Rebuild();
            propertyGrid1.SelectedObject = player;
            propertyGrid1.ExpandAllGridItems();
        }
        private void OnVehicleClick(object sender, EventArgs e)
        {
            InvisibleControl pb = sender as InvisibleControl;

            iconDB idb = pb.Tag as iconDB;
            if (GetUIDType(idb.uid) == UIDType.TypeVehicle)
            {
                Vehicle vehicle = new Vehicle(pb.Tag as iconDB);
                vehicle.Rebuild();
                propertyGrid1.SelectedObject = vehicle;
                propertyGrid1.ExpandAllGridItems();
            }
        }
        private void OnTentClick(object sender, EventArgs e)
        {
            InvisibleControl pb = sender as InvisibleControl;

            Tent tent = new Tent(pb.Tag as iconDB);
            tent.Rebuild();
            propertyGrid1.SelectedObject = tent;
            propertyGrid1.ExpandAllGridItems();
        }
        private void RefreshIcons()
        {
            if (currDisplayedItems == null)
                return;

            mtxUseDS.WaitOne();

            foreach (iconDB idb in listIcons)
                idb.icon.Location = GetMapPosFromIcon(idb);

            mtxUseDS.ReleaseMutex();
        }
        private void BuildOnlineIcons()
        {
            int idx = 0;
            foreach (DataRow row in dsOnlinePlayers.Tables[0].Rows)
            {
                if (idx >= iconsDB.Count)
                    iconsDB.Add(new iconDB());

                iconDB idb = iconsDB[idx];

                idb.uid = UInt64.Parse(row.Field<string>("unique_id")) | (UInt64)UIDType.TypePlayer;
                idb.row = row;
                idb.pos = GetMapPosFromString(row.Field<string>("worldspace"));

                if (idx >= iconPlayers.Count)
                {
                    InvisibleControl icon = new InvisibleControl();
                    icon.Image = null;
                    icon.Size = new Size(24, 24);
                    icon.Tag = null;
                    icon.Click += OnPlayerClick;
                    iconPlayers.Add(icon);
                }

                idb.icon = iconPlayers[idx];
                idb.icon.Image = global::DBAccess.Properties.Resources.iconOnline;
                idb.icon.Tag = idb;

                toolTip1.SetToolTip(idb.icon, row.Field<string>("name"));

                imgMap.Controls.Add(idb.icon);

                listIcons.Add(idb);

                if (checkBoxShowTrail.Checked)
                    GetUIDdata(idb.uid).AddPoint(idb.pos);

                idx++;
            }
        }
        private void BuildAliveIcons()
        {
            int idx = 0;
            foreach (DataRow row in dsAlivePlayers.Tables[0].Rows)
            {
                if (idx >= iconsDB.Count)
                    iconsDB.Add(new iconDB());

                iconDB idb = iconsDB[idx];

                idb.uid = UInt64.Parse(row.Field<string>("unique_id")) | (UInt64)UIDType.TypePlayer;
                idb.row = row;
                idb.pos = GetMapPosFromString(row.Field<string>("worldspace"));

                if (idx >= iconPlayers.Count)
                {
                    InvisibleControl icon = new InvisibleControl();
                    icon.Image = null;
                    icon.Size = new Size(24, 24);
                    icon.Tag = null;
                    icon.Click += OnPlayerClick;
                    iconPlayers.Add(icon);
                }

                idb.icon = iconPlayers[idx];
                idb.icon.Image = global::DBAccess.Properties.Resources.iconAlive;
                idb.icon.Tag = idb;

                toolTip1.SetToolTip(idb.icon, row.Field<string>("name"));
                
                imgMap.Controls.Add(idb.icon);

                listIcons.Add(idb);

                if (checkBoxShowTrail.Checked)
                    GetUIDdata(idb.uid).AddPoint(idb.pos);

                idx++;
            }
        }
        private void BuildVehicleIcons()
        {
            int idx=0;
            foreach (DataRow row in dsVehicles.Tables[0].Rows)
            {
                double damage = row.Field<double>("damage");

                if (idx >= iconsDB.Count)
                    iconsDB.Add(new iconDB());

                iconDB idb = iconsDB[idx];

                idb.uid = row.Field<UInt64>("id") | (UInt64)UIDType.TypeVehicle;
                idb.row = row;
                idb.pos = GetMapPosFromString(row.Field<string>("worldspace"));

                if( idx >= iconVehicles.Count)
                {
                    InvisibleControl icon = new InvisibleControl();
                    icon.Image = null;
                    icon.Tag = null;
                    icon.Size = new Size(24, 24);
                    icon.Click += OnVehicleClick;
                    iconVehicles.Add(icon);
                }

                idb.icon = iconVehicles[idx];

                if(damage < 1.0f)
                {
                    DataRow rowT = mycfg.vehicle_types.Tables[0].Rows.Find(row.Field<string>("class_name"));

                    string classname = (rowT != null) ? rowT.Field<string>("Type") : "";
                    switch (classname)
                    {
                        case "Air": idb.icon.Image = global::DBAccess.Properties.Resources.air; break;
                        case "Bicycle": idb.icon.Image = global::DBAccess.Properties.Resources.bike; break;
                        case "Boat": idb.icon.Image = global::DBAccess.Properties.Resources.boat; break;
                        case "Bus": idb.icon.Image = global::DBAccess.Properties.Resources.bus; break;
                        case "Car": idb.icon.Image = global::DBAccess.Properties.Resources.car; break;
                        case "Helicopter": idb.icon.Image = global::DBAccess.Properties.Resources.helicopter; break;
                        case "Motorcycle": idb.icon.Image = global::DBAccess.Properties.Resources.motorcycle; break;
                        case "Truck": idb.icon.Image = global::DBAccess.Properties.Resources.truck; break;
                        default: idb.icon.Image = global::DBAccess.Properties.Resources.car; break;
                    }
                }
                else
                {
                    idb.icon.Image = global::DBAccess.Properties.Resources.iconDestroyed;
                }

                idb.icon.Tag = idb;
                idb.icon.ContextMenuStrip = contextMenuStripVehicle;

                toolTip1.SetToolTip(idb.icon, row.Field<UInt64>("id").ToString() + ": "+ row.Field<string>("class_name"));

                imgMap.Controls.Add(idb.icon);

                listIcons.Add(idb);

                if (checkBoxShowTrail.Checked)
                    GetUIDdata(idb.uid).AddPoint(idb.pos);

                idx++;
            }
        }
        private void BuildSpawnIcons()
        {
            int idx = 0;
            foreach (DataRow row in dsVehicleSpawnPoints.Tables[0].Rows)
            {
                if (idx >= iconsDB.Count)
                    iconsDB.Add(new iconDB());

                iconDB idb = iconsDB[idx];

                idb.uid = row.Field<UInt64>("id") | (UInt64)UIDType.TypeSpawn;
                idb.row = row;
                idb.pos = GetMapPosFromString(row.Field<string>("worldspace"));

                if( idx >= iconVehicles.Count)
                {
                    InvisibleControl icon = new InvisibleControl();
                    icon.Image = null;
                    icon.Tag = null;
                    icon.Size = new Size(24, 24);
                    icon.Click += OnVehicleClick;
                    iconVehicles.Add(icon);
                }

                idb.icon = iconVehicles[idx];

                DataRow rowT = mycfg.vehicle_types.Tables[0].Rows.Find(row.Field<string>("class_name"));

                string classname = (rowT != null) ? rowT.Field<string>("Type") : "";
                switch (classname)
                {
                    case "Air": idb.icon.Image = global::DBAccess.Properties.Resources.air; break;
                    case "Bicycle": idb.icon.Image = global::DBAccess.Properties.Resources.bike; break;
                    case "Boat": idb.icon.Image = global::DBAccess.Properties.Resources.boat; break;
                    case "Bus": idb.icon.Image = global::DBAccess.Properties.Resources.bus; break;
                    case "Car": idb.icon.Image = global::DBAccess.Properties.Resources.car; break;
                    case "Helicopter": idb.icon.Image = global::DBAccess.Properties.Resources.helicopter; break;
                    case "Motorcycle": idb.icon.Image = global::DBAccess.Properties.Resources.motorcycle; break;
                    case "Truck": idb.icon.Image = global::DBAccess.Properties.Resources.truck; break;
                    default: idb.icon.Image = global::DBAccess.Properties.Resources.car; break;
                }

                idb.icon.Tag = idb;
                idb.icon.ContextMenuStrip = contextMenuStripSpawn;

                toolTip1.SetToolTip(idb.icon, row.Field<UInt64>("id").ToString() + ": " + row.Field<string>("class_name"));

                imgMap.Controls.Add(idb.icon);

                listIcons.Add(idb);

                idx++;
            }
        }
        private void BuildTentIcons()
        {
            int idx = 0;
            foreach (DataRow row in dsTents.Tables[0].Rows)
            {
                if (idx >= iconsDB.Count)
                    iconsDB.Add(new iconDB());

                iconDB idb = iconsDB[idx];

                idb.uid = row.Field<UInt64>("id") | (UInt64)UIDType.TypeTent;
                idb.row = row;
                idb.pos = GetMapPosFromString(row.Field<string>("worldspace"));

                if (idx >= iconTents.Count)
                {
                    InvisibleControl icon = new InvisibleControl();
                    icon.Image = global::DBAccess.Properties.Resources.tent;
                    icon.Size = new Size(24, 24);
                    icon.Tag = null;
                    icon.Click += OnTentClick;
                    iconTents.Add(icon);
                }

                idb.icon = iconTents[idx];
                idb.icon.Tag = idb;

                imgMap.Controls.Add(idb.icon);

                listIcons.Add(idb);

                idx++;
            }
        }
        private void Enable(bool bState)
        {
            bConnected = bState;

            buttonConnect.Enabled = !bState;

            radioButtonOnline.Enabled = bState;
            radioButtonAlive.Enabled = bState;
            radioButtonVehicles.Enabled = bState;
            radioButtonSpawn.Enabled = bState;
            radioButtonTents.Enabled = bState;

            textBoxURL.Enabled = !bState;
            textBoxBaseName.Enabled = !bState;
            textBoxPort.Enabled = !bState;
            textBoxUser.Enabled = !bState;
            textBoxPassword.Enabled = !bState;
            textBoxInstanceId.Enabled = !bState;

            buttonRemoveDestroyed.Enabled = bState;
            buttonSpawnNew.Enabled = bState;
            buttonRemoveBodies.Enabled = bState;
            buttonRemoveTents.Enabled = bState;

            if (!bState)
            {
                radioButtonOnline.Checked = false;
                radioButtonAlive.Checked = false;
                radioButtonVehicles.Checked = false;
                radioButtonSpawn.Checked = false;
                radioButtonTents.Checked = false;

                tbAlivePlayers.Text = "";
                tbOnlinePlayers.Text = "";
                tbVehicles.Text = "";
                tbVehicleSpawn.Text = "";
                tbTents.Text = "";
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseConnection();

            //
            SaveConfigFile();
        }
        private void LoadConfigFile()
        {
            try
            {
                if (Directory.Exists(configPath) == false)
                    Directory.CreateDirectory(configPath);

                XmlSerializer xs = new XmlSerializer(typeof(myConfig));
                using (StreamReader re = new StreamReader(configFile))
                {
                    mycfg = xs.Deserialize(re) as myConfig;
                }
            }
            catch (Exception)
            {
                Enable(false);
            }

            if (NullOrEmpty(mycfg.url)) mycfg.url = "";
            if (NullOrEmpty(mycfg.port)) mycfg.port = "3306";
            if (NullOrEmpty(mycfg.basename)) mycfg.basename = "basename";
            if (NullOrEmpty(mycfg.username)) mycfg.username = "username";
            if (NullOrEmpty(mycfg.password)) mycfg.password = "password";
            if (NullOrEmpty(mycfg.instance_id)) mycfg.instance_id = "1";
            if (NullOrEmpty(mycfg.vehicle_limit)) mycfg.vehicle_limit = "50";
            if (NullOrEmpty(mycfg.body_time_limit)) mycfg.body_time_limit = "7";
            if (NullOrEmpty(mycfg.tent_time_limit)) mycfg.tent_time_limit = "7";
            if (NullOrEmpty(mycfg.online_time_limit)) mycfg.online_time_limit = "5";
            if (mycfg.worlds_def.Tables.Count == 0)
            {
                DataTable table = mycfg.worlds_def.Tables.Add();
                table.Columns.Add(new DataColumn("World ID", typeof(UInt16)));
                table.Columns.Add(new DataColumn("World Name", typeof(string)));
                table.Columns.Add(new DataColumn("Width", typeof(UInt32)));
                table.Columns.Add(new DataColumn("Height", typeof(UInt32)));
                table.Columns.Add(new DataColumn("Filepath", typeof(string)));
                DataColumn[] keys = new DataColumn[1];
                keys[0] = mycfg.worlds_def.Tables[0].Columns[0];
                mycfg.worlds_def.Tables[0].PrimaryKey = keys;

                //table.Rows.Add(1, "not connected", 1, 1, "");
                //table.Rows.Add(2, "not connected", 1, 1, "");
                //table.Rows.Add(3, "not connected", 1, 1, "");
                //table.Rows.Add(4, "not connected", 1, 1, "");
                //table.Rows.Add(5, "not connected", 1, 1, "");
                //table.Rows.Add(6, "not connected", 1, 1, "");
                //table.Rows.Add(7, "not connected", 1, 1, "");
                //table.Rows.Add(8, "not connected", 1, 1, "");
                table.Rows.Add(9, "not connected", 12288, 12288, "");
                //table.Rows.Add(10, "not connected", 1, 1, "");
            }
            if (mycfg.vehicle_types.Tables.Count == 0)
            {
                DataTable table = mycfg.vehicle_types.Tables.Add();
                table.Columns.Add(new DataColumn("ClassName", typeof(string)));
                table.Columns.Add(new DataColumn("Type", typeof(string)));
                DataColumn[] keys = new DataColumn[1];
                keys[0] = mycfg.vehicle_types.Tables[0].Columns[0];
                mycfg.vehicle_types.Tables[0].PrimaryKey = keys;
            }

            try
            {
                textBoxURL.Text = mycfg.url;
                textBoxPort.Text = mycfg.port;
                textBoxBaseName.Text = mycfg.basename;
                textBoxUser.Text = mycfg.username;
                textBoxPassword.Text = mycfg.password;
                textBoxInstanceId.Text = mycfg.instance_id;
                textBoxVehicleMax.Text = mycfg.vehicle_limit;
                textBoxOldBodyLimit.Text = mycfg.body_time_limit;
                textBoxOldTentLimit.Text = mycfg.tent_time_limit;

                dataGridViewMaps.Columns["ColumnID"].DataPropertyName = "World ID";
                dataGridViewMaps.Columns["ColumnName"].DataPropertyName = "World Name";
                dataGridViewMaps.Columns["ColumnPath"].DataPropertyName = "Filepath";
                dataGridViewMaps.Columns["ColumnWidth"].DataPropertyName = "Width";
                dataGridViewMaps.Columns["ColumnHeight"].DataPropertyName = "Height";
                dataGridViewMaps.DataSource = mycfg.worlds_def.Tables[0];
                dataGridViewMaps.Sort(dataGridViewMaps.Columns["ColumnID"], ListSortDirection.Ascending);

                dataGridViewVehicleTypes.Columns["ColumnClassName"].DataPropertyName = "ClassName";
                dataGridViewVehicleTypes.Columns["ColumnType"].DataPropertyName = "Type";
                dataGridViewVehicleTypes.DataSource = mycfg.vehicle_types.Tables[0];
                dataGridViewVehicleTypes.Sort(dataGridViewVehicleTypes.Columns["ColumnClassName"], ListSortDirection.Ascending);
            }
            catch (Exception ex)
            {
                textBoxCmdStatus.Text = ex.ToString();
            }
        }
        private void SaveConfigFile()
        {
            try
            {
                mycfg.url = textBoxURL.Text;
                mycfg.port = textBoxPort.Text;
                mycfg.basename = textBoxBaseName.Text;
                mycfg.username = textBoxUser.Text;
                mycfg.password = textBoxPassword.Text;
                mycfg.instance_id = textBoxInstanceId.Text;
                mycfg.vehicle_limit = textBoxVehicleMax.Text;
                mycfg.body_time_limit = textBoxOldBodyLimit.Text;
                mycfg.tent_time_limit = textBoxOldTentLimit.Text;

                XmlSerializer xs = new XmlSerializer(typeof(myConfig));
                using (StreamWriter wr = new StreamWriter(configFile))
                {
                    xs.Serialize(wr, mycfg);
                }
            }
            catch (Exception)
            {
            }
        }
        private PointF GetMapPosFromString(string from)
        {
            ArrayList arr = ParseInventoryString(from);
            // [angle, [X, Y, Z]]
            
            double x = 0;
            double y = 0;

            if (arr.Count >= 2)
            {
                arr = arr[1] as ArrayList;
                x = double.Parse(arr[0] as string, CultureInfo.InvariantCulture.NumberFormat);
                y = double.Parse(arr[1] as string, CultureInfo.InvariantCulture.NumberFormat);
            }

            x = x / mycfg.db_map_size.Width;
            y = 1.0f - (y / mycfg.db_map_size.Height);

            return new PointF((float)x, (float)y);
        }
        private Point GetMapPosFromIcon(iconDB from)
        {
            float x = from.pos.X * MapSize.Width - 12;
            float y = from.pos.Y * MapSize.Height - 12;

            return new Point((int)x, (int)y);
        }
        private void DB_OnConnection()
        {
            if (!bConnected)
                return;

            mtxUpdateDB.WaitOne();

            DataSet _dsWorlds = new DataSet();
            DataSet _dsAllVehicleTypes = new DataSet();

            this.textBoxCmdStatus.Text = "";

            try
            {
                MySqlCommand cmd = cnx.CreateCommand();
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                //
                //  Instance -> World Id
                //
                cmd.CommandText = "SELECT * FROM instance";
                this.dsInstances.Clear();
                adapter.Fill(this.dsInstances);
                DataColumn[] keys = new DataColumn[1];
                keys[0] = dsInstances.Tables[0].Columns[0];
                dsInstances.Tables[0].PrimaryKey = keys;

                //
                //  Worlds
                //
                cmd.CommandText = "SELECT * FROM world";
                _dsWorlds.Clear();
                adapter.Fill(_dsWorlds);

                //
                //  Vehicle types
                //
                cmd.CommandText = "SELECT class_name FROM vehicle";
                _dsAllVehicleTypes.Clear();
                adapter.Fill(_dsAllVehicleTypes);
            }
            catch (Exception ex)
            {
                this.textBoxCmdStatus.Text += ex.ToString();
            }

            mtxUpdateDB.ReleaseMutex();

            mtxUseDS.WaitOne();

            try
            {
                foreach(DataRow row in _dsAllVehicleTypes.Tables[0].Rows)
                {
                    string name = row.Field<string>("class_name");

                    if (mycfg.vehicle_types.Tables[0].Rows.Find(name) == null)
                        mycfg.vehicle_types.Tables[0].Rows.Add(name, "Car");
                }

                foreach (DataRow row in _dsWorlds.Tables[0].Rows)
                {
                    DataRow rowWD = mycfg.worlds_def.Tables[0].Rows.Find(row.Field<UInt16>("id"));
                    if (rowWD == null)
                    {
                        mycfg.worlds_def.Tables[0].Rows.Add(row.Field<UInt16>("id"),
                                                            row.Field<string>("name"),
                                                            (UInt32)row.Field<Int32>("max_x"),
                                                            (UInt32)row.Field<Int32>("max_y"),
                                                            "");
                    }
                    else
                    {
                        rowWD.SetField<string>("World Name", row.Field<string>("name"));
                    }
                }

                DataRow rowI = this.dsInstances.Tables[0].Rows.Find(UInt16.Parse(mycfg.instance_id));
                if (rowI != null)
                {
                    mycfg.world_id = rowI.Field<UInt16>("world_id");

                    DataRow rowW = mycfg.worlds_def.Tables[0].Rows.Find(mycfg.world_id);
                    if (rowW != null)
                        textBoxWorld.Text = rowW.Field<string>("World Name");

                    SetCurrentMap();
                }
            }
            catch (Exception ex)
            {
                textBoxCmdStatus.Text += ex.ToString();
            }

            mtxUseDS.ReleaseMutex();

            if (mycfg.world_id == 0)
            {
                CloseConnection();
                MessageBox.Show("Instance id '" + mycfg.instance_id + "' not found in database", "Warning");
            }
        }
        private void SetCurrentMap()
        {
            try
            {
                DataRow rowW = mycfg.worlds_def.Tables[0].Rows.Find(mycfg.world_id);
                if (rowW != null)
                {
                    textBoxWorld.Text = rowW.Field<string>("World Name");

                    string filepath = rowW.Field<string>("Filepath");

                    if (File.Exists(filepath))
                        bitmapHQ = new Bitmap(filepath);
                    else
                        bitmapHQ = global::DBAccess.Properties.Resources.InvalidMap;
                }

                imgMap.Image = bitmapHQ;

                mycfg.db_map_size = new Size((int)rowW.Field<UInt32>("Width"), (int)rowW.Field<UInt32>("Height"));

                if (imgMap.Image != null)
                    zoomFactor = MapSize.Width / (float)imgMap.Image.Size.Width;

                Size sizePanel = splitContainer1.Panel1.Size;
                Point halfPanel = new Point((int)(sizePanel.Width * 0.5f), (int)(sizePanel.Height * 0.5f));
                MapPos.X = (int)(halfPanel.X - MapSize.Width * 0.5f);
                MapPos.Y = (int)(halfPanel.Y - MapSize.Height * 0.5f);

                ApplyMapChanges();
            }
            catch (Exception ex)
            {
                imgMap.Image = null;
                textBoxCmdStatus.Text += ex.ToString();
            }
        }
        private void DB_OnRefresh()
        {
            if (bConnected)
            {
                DataSet _dsAlivePlayers = new DataSet();
                DataSet _dsOnlinePlayers = new DataSet();
                DataSet _dsTents = new DataSet();
                DataSet _dsVehicles = new DataSet();
                DataSet _dsVehicleSpawnPoints = new DataSet();

                mtxUpdateDB.WaitOne();

                try
                {
                    MySqlCommand cmd = cnx.CreateCommand();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    //
                    //  Players alive
                    //
                    cmd.CommandText = "SELECT s.id id, s.unique_id unique_id, p.name name, p.humanity humanity, s.worldspace worldspace,"
                                    + " s.inventory inventory, s.backpack backpack, s.medical medical, s.state state, s.last_updated last_updated"
                                    + " FROM survivor as s, profile as p WHERE s.unique_id=p.unique_id AND s.world_id=" + mycfg.world_id + " AND s.is_dead=0";
                    _dsAlivePlayers.Clear();
                    adapter.Fill(_dsAlivePlayers);

                    //
                    //  Players online
                    //
                    cmd.CommandText += " AND s.last_updated > now() - interval " + mycfg.online_time_limit + " minute";
                    _dsOnlinePlayers.Clear();
                    adapter.Fill(_dsOnlinePlayers);

                    //
                    //  Vehicles
                    //
                    cmd.CommandText = "SELECT iv.id id, wv.id spawn_id, v.class_name class_name, iv.worldspace worldspace, iv.inventory inventory,"
                                    + " iv.fuel fuel, iv.damage damage, iv.last_updated last_updated"
                                    + " FROM vehicle as v, world_vehicle as wv, instance_vehicle as iv"
                                    + " WHERE iv.instance_id=" + mycfg.instance_id
                                    + " AND iv.world_vehicle_id=wv.id AND wv.vehicle_id=v.id";
                    _dsVehicles.Clear();
                    adapter.Fill(_dsVehicles);

                    //
                    //  Vehicle Spawn points
                    //
                    cmd.CommandText = "SELECT w.id id, w.worldspace worldspace, v.class_name class_name FROM world_vehicle as w, vehicle as v"
                                    + " WHERE w.world_id=" + mycfg.world_id + " AND w.vehicle_id=v.id";
                    _dsVehicleSpawnPoints.Clear();
                    adapter.Fill(_dsVehicleSpawnPoints);

                    //
                    //  Tents
                    //
                    cmd.CommandText = "SELECT * FROM instance_deployable WHERE instance_id=" + mycfg.instance_id + " AND deployable_id=1";
                    _dsTents.Clear();
                    adapter.Fill(_dsTents);
                }
                catch (Exception ex)
                {
                    textBoxCmdStatus.Text = ex.ToString();
                }

                mtxUpdateDB.ReleaseMutex();

                mtxUseDS.WaitOne();

                dsTents = _dsTents.Copy();
                dsAlivePlayers = _dsAlivePlayers.Copy();
                dsOnlinePlayers = _dsOnlinePlayers.Copy();
                dsVehicles = _dsVehicles.Copy();
                dsVehicleSpawnPoints = _dsVehicleSpawnPoints.Copy();

                mtxUseDS.ReleaseMutex();
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            while (!bw.CancellationPending)
            {
                Thread.Sleep(5000);

                DB_OnRefresh();

                if (System.Threading.Interlocked.CompareExchange(ref bUserAction, 1, 0) == 0)
                {
                    dlgUpdateIcons = this.BuildIcons;
                    this.Invoke(dlgUpdateIcons);
                    System.Threading.Interlocked.Exchange(ref bUserAction, 0);
                }
            }
        }
        private void buttonRemoveDestroyed_Click(object sender, EventArgs e)
        {
            int res = ExecuteSqlNonQuery("DELETE FROM instance_vehicle WHERE instance_id=" + mycfg.instance_id + " AND damage=1");

            textBoxCmdStatus.Text = "removed " + res + " destroyed vehicles.";
        }
        private void buttonSpawnNew_Click(object sender, EventArgs e)
        {
            if (bConnected)
            {
                this.Cursor = Cursors.WaitCursor;
                mtxUpdateDB.WaitOne();

                try
                {
                    MySqlCommand cmd = cnx.CreateCommand();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    cmd.CommandText = "SELECT count(*) FROM instance_vehicle WHERE instance_id ="+mycfg.instance_id;
                    object result = cmd.ExecuteScalar();
                    long vehicle_count = (long)result;

                    cmd.CommandText = "SELECT wv.id world_vehicle_id, v.id vehicle_id, wv.worldspace, v.inventory, coalesce(v.parts, '') parts, v.limit_max,"
                                    + " round(least(greatest(rand(), v.damage_min), v.damage_max), 3) damage, round(least(greatest(rand(), v.fuel_min), v.fuel_max), 3) fuel"
                                    + " FROM world_vehicle wv JOIN vehicle v ON wv.vehicle_id = v.id LEFT JOIN instance_vehicle iv ON iv.world_vehicle_id = wv.id AND iv.instance_id = "+mycfg.instance_id
                                    + " LEFT JOIN ( SELECT count(iv.id) AS count, wv.vehicle_id FROM instance_vehicle iv JOIN world_vehicle wv ON iv.world_vehicle_id = wv.id"
                                    + " WHERE instance_id ="+mycfg.instance_id+" GROUP BY wv.vehicle_id) vc ON vc.vehicle_id = v.id"
                                    + " WHERE wv.world_id ="+mycfg.world_id+" AND iv.id IS null AND (round(rand(), 3) < wv.chance)"
                                    + " and (vc.count IS null OR vc.count BETWEEN v.limit_min AND v.limit_max) GROUP BY wv.worldspace";
                    MySqlDataReader reader = cmd.ExecuteReader();

                    int spawn_count = 0;
                    int max_vehicle = int.Parse(textBoxVehicleMax.Text);

                    List<string> queries = new List<string>();

                    while (reader.Read() && (spawn_count + vehicle_count < max_vehicle))
                    {
                        UInt64 world_vehicle_id = reader.GetUInt64(0);
                        UInt16 vehicle_id = reader.GetUInt16(1);
                        string worldspace = reader.GetString(2);
                        string inventory = reader.GetString(3);
                        string parts = reader.GetString(4);
                        byte limit_max = reader.GetByte(5);
                        double damage = reader.GetDouble(6);
                        double fuel = reader.GetDouble(7);

                        // Generate parts damage
                        Random rand = new Random();
                        string[] to_parts = parts.Split(',');
                        string health = "[";
                        foreach(string part in to_parts)
                        {
                            if(rand.NextDouble() > 0.25)
                                health += "[\"" + part + "\",1],";
                        }
                        health = health.TrimEnd(',');
                        health += "]";

                        queries.Add("INSERT INTO instance_vehicle (world_vehicle_id, worldspace, inventory, parts, damage, fuel, instance_id, created) values ("
                                    + world_vehicle_id + ", '"
                                    + worldspace + "', '"
                                    + inventory + "', '"
                                    + health + "', "
                                    + damage + ", "
                                    + fuel + ", "
                                    + mycfg.instance_id + ", current_timestamp())" );

                        spawn_count++;
                    }

                    reader.Close();

                    foreach (string query in queries)
                    {
                        cmd.CommandText = query;
                        int res = cmd.ExecuteNonQuery();
                    }

                    textBoxCmdStatus.Text = "spawned " + queries.Count + " new vehicles.";
                }
                catch (Exception ex)
                {
                    textBoxCmdStatus.Text = ex.ToString();
                }

                mtxUpdateDB.ReleaseMutex();
                this.Cursor = Cursors.Arrow;
            }
        }
        private void buttonRemoveBodies_Click(object sender, EventArgs e)
        {
            int limit = int.Parse(textBoxOldBodyLimit.Text);

            string query = "DELETE FROM survivor WHERE world_id=" + mycfg.world_id + " AND is_dead=1 AND last_updated < now() - interval " + limit + " day";
            int res = ExecuteSqlNonQuery(query);

            textBoxCmdStatus.Text = "removed " + res + " bodies older than " + limit + " days.";
        }
        private void buttonRemoveTents_Click(object sender, EventArgs e)
        {
            int limit = int.Parse(textBoxOldTentLimit.Text);

            string query = "DELETE FROM id using instance_deployable id inner join deployable d on id.deployable_id = d.id"
                         + " inner join survivor s on id.owner_id = s.id and s.is_dead=1"
                         + " WHERE id.instance_id=" + mycfg.instance_id + " AND d.class_name = 'TentStorage' AND id.last_updated < now() - interval " + limit + " day";
            int res = ExecuteSqlNonQuery(query);
        }
        private void imgMap_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (checkBoxShowTrail.Checked )
                {
                    foreach (iconDB idb in listIcons)
                    {
                        UIDType type = GetUIDType(idb.uid);

                        if ((type == UIDType.TypePlayer) || (type == UIDType.TypeVehicle))
                            GetUIDdata(idb.uid).Display(e.Graphics, MapSize);
                    }
                }

                foreach (iconDB idb in listIcons)
                {
                    e.Graphics.DrawImage(idb.icon.Image, idb.icon.Location.X, idb.icon.Location.Y, 24, 24);
                }
            }
            catch (Exception ex)
            {
                textBoxCmdStatus.Text = ex.ToString();
            }
        }
        private void checkBoxShowTrail_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == false)
            {
                foreach (KeyValuePair<UInt64, UIDdata> pair in dicUIDdata)
                    pair.Value.path.Reset();
            }
        }
        private int ExecuteSqlNonQuery(string query)
        {
            if (!bConnected)
                return 0;

            int res = 0;

            this.Cursor = Cursors.WaitCursor;
            mtxUpdateDB.WaitOne();

            try
            {
                MySqlCommand cmd = cnx.CreateCommand();

                cmd.CommandText = query;

                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                textBoxCmdStatus.Text = ex.ToString();
            }

            mtxUpdateDB.ReleaseMutex();
            this.Cursor = Cursors.Arrow;

            return res;
        }
        private void dataGridViewMaps_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells.  
            if (e.RowIndex < 0 || e.ColumnIndex != dataGridViewMaps.Columns["ColumnChoosePath"].Index)
                return;

            DialogResult res = openFileDialog1.ShowDialog();

            if (res == DialogResult.OK)
            {
                dataGridViewMaps["ColumnPath", e.RowIndex].Value = openFileDialog1.FileName;

                if (dataGridViewMaps["ColumnID", e.RowIndex].Value.ToString() == mycfg.world_id.ToString())
                    SetCurrentMap();
            }
        }
        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    Control sourceControl = owner.SourceControl;

                    iconDB idb = sourceControl.Tag as iconDB;

                    UInt64 id = GetUIDData(idb.uid);
                    int res = ExecuteSqlNonQuery("DELETE FROM instance_vehicle WHERE id=" + id + " AND instance_id=" + mycfg.instance_id);
                    if (res == 1)
                        textBoxCmdStatus.Text = "removed vehicle id " + id;
                }
            }
        }
        private void toolStripMenuItemDeleteSpawn_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    Control sourceControl = owner.SourceControl;

                    iconDB idb = sourceControl.Tag as iconDB;

                    UInt64 id = GetUIDData(idb.uid);
                    int res = ExecuteSqlNonQuery("DELETE FROM world_vehicle WHERE id=" + id + " AND world_id=" + mycfg.world_id);
                    if (res == 1)
                        textBoxCmdStatus.Text = "removed vehicle spawnpoint id " + id;
                }
            }
        }
        internal UIDdata GetUIDdata(UInt64 uid)
        {
            UIDdata uiddata = null;

            if (dicUIDdata.TryGetValue(uid, out uiddata) == false)
                dicUIDdata[uid] = uiddata = new UIDdata();

            return uiddata;
        }
        //
        //
        //
        internal static ArrayList ParseInventoryString(string str)
        {
            Stack<ArrayList> stack = new Stack<ArrayList>();
            ArrayList main = null;
            ArrayList curr = null;
            string value = "";
            bool bValue = false;

            foreach (char c in str)
            {
                switch (c)
                {
                    case '[': 
                        if( curr != null ) stack.Push(curr);
                        curr = new ArrayList();
                        if (stack.Count > 0) stack.Peek().Add(curr);
                        if (main == null)
                            main = curr;
                        break;
                    
                    case ']':
                        if (value != "") curr.Add(value);
                        value = "";
                        bValue = false;
                        if (stack.Count > 0) curr = stack.Pop();
                        else curr = null;
                        break;

                    case '"':
                        bValue = true;
                        break;

                    case ',':
                        if (bValue) curr.Add(value);
                        value = "";
                        bValue = false;
                        break;

                    default:
                        bValue = true;
                        value += c;
                        break;
                }
            }

            return main;
        }
        internal static bool NullOrEmpty(string str)
        {
            return ((str == null) || (str == ""));
        }
        //
        //
        //
        public class iconDB
        {
            public InvisibleControl icon;
            public DataRow row;
            public PointF pos;
            public UInt64 uid = 0;
        };
        internal class EntryConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
            {
                if (destType == typeof(string) && value is Entry)
                {
                    Entry entry = (Entry)value;
                    return entry.count.ToString();
                }
                return base.ConvertTo(context, culture, value, destType);
            }
        }
        internal class NullConverter<T> : ExpandableObjectConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
            {
                if (destType == typeof(string))
                {
                    if (value is Cargo)
                    {
                        Cargo cargo = value as Cargo;
                        return "total = " + cargo.Total;
                    }
                    else if (value is T)
                    {
                        return "";
                    }
                }
                return base.ConvertTo(context, culture, value, destType);
            }
        }
        internal class NullEditor : CollectionEditor
        {
            public NullEditor(Type type) : base(type) { }
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.None;
            }
        }
        internal class CargoPropertyDescriptor : PropertyDescriptor
        {
            private Cargo collection = null;
            private int index = -1;

            public CargoPropertyDescriptor(Cargo coll, int idx) :
                base("#" + idx.ToString(), null)
            {
                this.collection = coll;
                this.index = idx;
            }

            public override AttributeCollection Attributes { get { return new AttributeCollection(null); } }
            public override bool CanResetValue(object component) { return true; }
            public override Type ComponentType { get { return this.collection.GetType(); } }

            public override string DisplayName
            {
                get
                {
                    Entry entry = this.collection[index];
                    return entry.name;
                }
            }

            public override string Description
            {
                get
                {
                    Entry entry = this.collection[index];
                    StringBuilder sb = new StringBuilder();
                    sb.Append(entry.name + " : " + entry.count);
                    return sb.ToString();
                }
            }

            public override object GetValue(object component) { return this.collection[index]; }
            public override bool IsReadOnly { get { return true; } }
            public override string Name { get { return "#" + index.ToString(); } }
            public override Type PropertyType { get { return this.collection[index].GetType(); } }
            public override void ResetValue(object component) { }
            public override bool ShouldSerializeValue(object component) { return true; }
            public override void SetValue(object component, object value) { }
        };
        [TypeConverterAttribute(typeof(NullConverter<Cargo>)), EditorAttribute(typeof(NullEditor), typeof(UITypeEditor))]
        public class Cargo : CollectionBase, ICustomTypeDescriptor
        {
            #region Collection Implementation

            public void Add(Entry zone) { this.List.Add(zone); }
            public void Remove(Entry zone) { this.List.Remove(zone); }
            public Entry this[int index] { get { return (Entry)this.List[index]; } }

            #endregion

            // Implementation of interface ICustomTypeDescriptor 
            #region ICustomTypeDescriptor impl

            public String GetClassName() { return TypeDescriptor.GetClassName(this, true); }
            public AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(this, true); }
            public String GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
            public TypeConverter GetConverter() { return TypeDescriptor.GetConverter(this, true); }
            public EventDescriptor GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
            public PropertyDescriptor GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
            public object GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
            public EventDescriptorCollection GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
            public EventDescriptorCollection GetEvents() { return TypeDescriptor.GetEvents(this, true); }
            public object GetPropertyOwner(PropertyDescriptor pd) { return this; }
            public PropertyDescriptorCollection GetProperties(Attribute[] attributes) { return GetProperties(); }
            public PropertyDescriptorCollection GetProperties()
            {
                // Create a collection object to hold property descriptors
                PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

                // Iterate the list of employees
                for (int i = 0; i < this.List.Count; i++)
                {
                    // Create a property descriptor for the zone item and add to the property descriptor collection
                    CargoPropertyDescriptor pd = new CargoPropertyDescriptor(this, i);
                    pds.Add(pd);
                }
                // return the property descriptor collection
                return pds;
            }

            #endregion

            public int Total
            {
                get
                {
                    int total = 0;
                    foreach (Entry entry in List)
                        total += entry.count;
                    return total;
                }
            }
        }
        [TypeConverter(typeof(EntryConverter))]
        public class Entry
        {
            public Entry(string n = "", int c = 0)
            {
                name = n;
                count = c;
            }

            public string name { get; set; }
            public int count { get; set; }
        };
        [TypeConverterAttribute(typeof(NullConverter<Storage>))]
        public class Storage
        {
            public Storage()
            {
                weapons = new Cargo();
                items = new Cargo();
                bags = new Cargo();
            }

            public Cargo weapons { get; set; }
            public Cargo items { get; set; }
            public Cargo bags { get; set; }
        };
        public abstract class PropObjectBase
        {
            public PropObjectBase(iconDB idb)
            {
                this.idb = idb;
            }

            public iconDB idb;

            public abstract void Rebuild();
        }
        public class Survivor : PropObjectBase
        {
            public Survivor(iconDB idb)
                : base(idb)
            {
                this.inventory = new Cargo();
                this.backpack = new Cargo();
                this.tools = new Cargo();
            }

            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public string name { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public UInt64 uid { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public int humanity { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public int blood { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public string hunger { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public string thirst { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public string medical { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public string weapon { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public Cargo inventory { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public string backpackclass { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public Cargo backpack { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public Cargo tools { get; set; }
            public override void Rebuild()
            {
                this.inventory.Clear();
                this.backpack.Clear();
                this.tools.Clear();

                Dictionary<string, int> dicInventory = new Dictionary<string, int>();

                ArrayList arr = ParseInventoryString(idb.row.Field<string>("medical"));
                // arr[0] = is dead
                // arr[1] = unconscious
                // arr[2] = infected
                // arr[3] = injured
                // arr[4] = in pain
                // arr[5] = is cardiac
                // arr[6] = low blood
                // arr[7] = blood quantity
                // arr[8] = [wounds]
                // arr[9] = [legs, arms]
                // arr[10] = unconscious time
                // arr[11] = [hunger, thirst]
                
                this.medical = "";
                if (bool.Parse(arr[1] as string)) this.medical += " unconscious, ";
                if (bool.Parse(arr[2] as string)) this.medical += " infected, ";
                if (bool.Parse(arr[3] as string)) this.medical += " injured, ";
                if (bool.Parse(arr[4] as string)) this.medical += " in pain, ";
                if (bool.Parse(arr[6] as string)) this.medical += " low blood, ";
                this.medical = this.medical.Trim();
                this.medical = this.medical.TrimEnd(',');

                this.blood = (int)double.Parse(arr[7] as string, CultureInfo.InvariantCulture.NumberFormat);
                this.hunger = ((int)(double.Parse((arr[11] as ArrayList)[0] as string, CultureInfo.InvariantCulture.NumberFormat) / 21.60f)).ToString() + "%";
                this.thirst = ((int)(double.Parse((arr[11] as ArrayList)[1] as string, CultureInfo.InvariantCulture.NumberFormat) / 14.40f)).ToString() + "%";

                arr = ParseInventoryString(idb.row.Field<string>("inventory"));

                if (arr.Count > 0)
                {
                    // arr[0] = tools
                    // arr[1] = items

                    foreach (string o in arr[0] as ArrayList)
                        this.tools.Add(new Entry(o, 1));

                    foreach (object o in (arr[1] as ArrayList))
                    {
                        if (o is string)
                        {
                            string name = o as string;
                            if (dicInventory.ContainsKey(name) == false)
                                dicInventory.Add(name, 1);
                            else
                                dicInventory[name]++;
                        }
                        else if (o is ArrayList)
                        {
                            ArrayList oal = o as ArrayList;

                            string name = oal[0] as string;
                            if (dicInventory.ContainsKey(name) == false)
                                dicInventory.Add(name, 1);
                            else
                                dicInventory[name]++;
                        }
                    }

                    foreach (KeyValuePair<string, int> pair in dicInventory)
                        this.inventory.Add(new Entry(pair.Key, pair.Value));
                }

                arr = ParseInventoryString(idb.row.Field<string>("backpack"));
                // arr[0] = backpack's class
                // arr[1] = weapons
                // arr[2] = items

                if (arr.Count > 0)
                {
                    this.backpackclass = arr[0] as string;

                    {
                        ArrayList aWeapons = arr[1] as ArrayList;
                        ArrayList aTypes = aWeapons[0] as ArrayList;
                        ArrayList aCount = aWeapons[1] as ArrayList;
                        for (int i = 0; i < aTypes.Count; i++)
                            this.backpack.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                    }

                    {
                        ArrayList aItems = arr[2] as ArrayList;
                        ArrayList aTypes = aItems[0] as ArrayList;
                        ArrayList aCount = aItems[1] as ArrayList;
                        for (int i = 0; i < aTypes.Count; i++)
                            this.backpack.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                    }
                }

                arr = ParseInventoryString(idb.row.Field<string>("state"));

                if (arr.Count > 0)
                {
                    this.weapon = arr[0] as string;
                    if (this.weapon == "")
                        this.weapon = "none";
                }

                this.name = idb.row.Field<string>("name");
                this.uid = UInt64.Parse(idb.row.Field<string>("unique_id"));
                this.humanity = idb.row.Field<int>("humanity");
            }
        }
        public class Tent : PropObjectBase
        {
            public Tent(iconDB idb)
                : base(idb)
            {
                this.inventory = new Storage();
            }

            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public string owner { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public Storage inventory { get; set; }
            public override void Rebuild()
            {
                this.inventory.weapons.Clear();
                this.inventory.items.Clear();
                this.inventory.bags.Clear();

                this.owner = "who knows...";

                ArrayList arr = ParseInventoryString(idb.row.Field<string>("inventory"));
                ArrayList aItems;
                ArrayList aTypes;
                ArrayList aCount;

                if (arr.Count > 0)
                {
                    // arr[0] = weapons
                    // arr[1] = items
                    // arr[2] = bags
                    aItems = arr[0] as ArrayList;
                    aTypes = aItems[0] as ArrayList;
                    aCount = aItems[1] as ArrayList;
                    for (int i = 0; i < aTypes.Count; i++)
                        this.inventory.weapons.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
 
                    aItems = arr[1] as ArrayList;
                    aTypes = aItems[0] as ArrayList;
                    aCount = aItems[1] as ArrayList;
                    for (int i = 0; i < aTypes.Count; i++)
                        this.inventory.items.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));

                    aItems = arr[2] as ArrayList;
                    aTypes = aItems[0] as ArrayList;
                    aCount = aItems[1] as ArrayList;
                    for (int i = 0; i < aTypes.Count; i++)
                        this.inventory.bags.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                }
            }
        }
        public class Vehicle : PropObjectBase
        {
            public Vehicle(iconDB idb)
                : base(idb)
            {
                this.inventory = new Storage();
            }

            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public string classname { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public UInt64 uid { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public UInt64 spawn_id { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public double fuel { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public double damage { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public Storage inventory { get; set; }
            public override void Rebuild()
            {
                this.inventory.weapons.Clear();
                this.inventory.items.Clear();
                this.inventory.bags.Clear();

                this.uid = idb.row.Field<UInt64>("id");
                this.spawn_id = idb.row.Field<UInt64>("spawn_id");
                this.fuel = idb.row.Field<double>("fuel");
                this.damage = idb.row.Field<double>("damage");
                this.classname = idb.row.Field<string>("class_name");
                {
                    ArrayList arr = ParseInventoryString(idb.row.Field<string>("inventory"));
                    ArrayList aItems;
                    ArrayList aTypes;
                    ArrayList aCount;

                    if (arr.Count > 0)
                    {
                        // arr[0] = weapons
                        // arr[1] = items
                        // arr[2] = bags
                        aItems = arr[0] as ArrayList;
                        aTypes = aItems[0] as ArrayList;
                        aCount = aItems[1] as ArrayList;
                        for (int i = 0; i < aTypes.Count; i++)
                            this.inventory.weapons.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));

                        aItems = arr[1] as ArrayList;
                        aTypes = aItems[0] as ArrayList;
                        aCount = aItems[1] as ArrayList;
                        for (int i = 0; i < aTypes.Count; i++)
                            this.inventory.items.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));

                        aItems = arr[2] as ArrayList;
                        aTypes = aItems[0] as ArrayList;
                        aCount = aItems[1] as ArrayList;
                        for (int i = 0; i < aTypes.Count; i++)
                            this.inventory.bags.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                    }
                }
            }
        }
        public delegate void DlgUpdateIcons();
        public class myConfig
        {
            public myConfig()
            {
                worlds_def = new DataSet();
                vehicle_types = new DataSet();
                db_map_size = Size.Empty;
            }
            public string url { get; set; }
            public string port { get; set; }
            public string basename { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string instance_id { get; set; }
            public string vehicle_limit { get; set; }
            public string body_time_limit { get; set; }
            public string tent_time_limit { get; set; }
            public string online_time_limit { get; set; }
            public DataSet worlds_def { get; set; }
            public DataSet vehicle_types { get; set; }
            //
            [XmlIgnore]
            public UInt16 world_id = 0;
            [XmlIgnore]
            public Size db_map_size;
        }
        public class InvisibleControl : Control
        {
            private Image _image;

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle |= 0x20;
                    return cp;
                }
            }
            protected override void OnPaint(PaintEventArgs e) { }
            protected override void OnPaintBackground(PaintEventArgs e) { }

            public Image Image
            {
                get { return _image; }
                set { _image = value; RecreateHandle(); }
            }
        }
        public class UIDdata
        {
            public PointF invalidPos = new PointF(0,1);

            public UIDdata()
            {
                Random rnd = new Random();

                byte[] rgb = { 0, 0, 0 };
                rnd.NextBytes(rgb);

                pen = new Pen(Color.FromArgb(128, 128 + rgb[0] / 2, 128 + rgb[1] / 2, 128 + rgb[2] / 2), 3);
            }

            public void AddPoint(PointF pos)
            {
                if ((unitPositions.Count == 0) || (pos != unitPositions.Last()))
                    unitPositions.Add(pos);
            }

            public void Display(Graphics gfx, Size mapSize)
            {
                path.Reset();

                PointF last = invalidPos;
                foreach (PointF pt in unitPositions)
                {
                    PointF newpt = new PointF(pt.X * mapSize.Width, pt.Y * mapSize.Height);

                    // if a point is invalid, break the continuity
                    if ((last != invalidPos) && (pt != invalidPos))
                        path.AddLine(last, newpt);

                    last = newpt;
                }

                gfx.DrawPath(pen, path);
            }

            public GraphicsPath path = new GraphicsPath();
            public Pen pen;
            public List<PointF> unitPositions = new List<PointF>();
        }
        public enum UIDType : ulong
        {
            TypePlayer = 0x100000000,
            TypeVehicle = 0x200000000,
            TypeSpawn = 0x400000000,
            TypeTent = 0x800000000,
            TypeMask = 0xffff00000000
        };
        internal UInt64 GetUIDData(UInt64 uid)
        {
            return (UInt64)(uid & ~(UInt64)UIDType.TypeMask);
        }
        internal UIDType GetUIDType(UInt64 uid)
        {
            return (UIDType)(uid & (UInt64)UIDType.TypeMask);
        }
    }
}
