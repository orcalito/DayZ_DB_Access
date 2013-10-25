using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DBAccess
{
    public partial class Form1 : Form
    {
        static ModuleVersion curCfgVersion = new ModuleVersion(3, 0);

        private static int bUserAction = 0;
        private static Mutex mtxUpdateDB = new Mutex();
        private static Mutex mtxUseDS = new Mutex();
        private static EventWaitHandle eventFastBgWorker = new EventWaitHandle(false, EventResetMode.AutoReset);
        public DlgUpdateIcons dlgUpdateIcons;
        public DlgUpdateIcons dlgRefreshMap;
        //
        private MySqlConnection sqlCnx;
        private bool bConnected = false;
        private RadioButton currDisplayedItems;
        //
        public myConfig mycfg = new myConfig(curCfgVersion);
        private string configPath;
        private string configFilePath;
        private VirtualMap virtualMap = new VirtualMap();
        //
        private DataSet dsInstances = new DataSet();
        private DataSet dsDeployables = new DataSet();
        private DataSet dsAlivePlayers = new DataSet();
        private DataSet dsOnlinePlayers = new DataSet();
        private DataSet dsVehicles = new DataSet();
        private DataSet dsVehicleSpawnPoints = new DataSet();

        private Dictionary<string, UIDGraph> dicUIDGraph = new Dictionary<string, UIDGraph>();
        private List<iconDB> listIcons = new List<iconDB>();
        private List<iconDB> iconsDB = new List<iconDB>();
        private List<myIcon> iconPlayers = new List<myIcon>();
        private List<myIcon> iconVehicles = new List<myIcon>();
        private List<myIcon> iconDeployables = new List<myIcon>();
        private DragNDrop dragndrop = new DragNDrop();
        private MapZoom mapZoom = new MapZoom(eventFastBgWorker);

        private MapHelper mapHelper;
        private UIDGraph cartographer = new UIDGraph(new Pen(Color.Black, 2));

        private List<tileReq> tileRequests = new List<tileReq>();
        private List<tileNfo> tileCache = new List<tileNfo>();

        public Form1()
        {
            InitializeComponent();

            //
            configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DayZDBAccess";
            configFilePath = configPath + "\\config.xml";

            try
            {
                Assembly asb = System.Reflection.Assembly.GetExecutingAssembly();
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    Version version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                    this.Text = asb.GetName().Name + " - v" + version.ToString();
                }
                else
                {
                    this.Text = asb.GetName().Name + " - Test version";
                }
            }
            catch
            {
                this.Text = "DayZ DB Access unknown version";
            }

            this.Resize += Form1Resize;
            this.MouseWheel += Form1_MouseWheel;

            //
            LoadConfigFile();

            bgWorkerDatabase.RunWorkerAsync();
            bgWorkerFast.RunWorkerAsync();

            Enable(false);
        }
        void ApplyMapChanges()
        {
            if (!virtualMap.Enabled)
                return;

            Tool.Size sizePanel = splitContainer1.Panel1.Size;
            Tool.Point halfPanel = (Tool.Point)(sizePanel * 0.5f);
            Tool.Size delta = new Tool.Size(16, 16);

            virtualMap.Position = Tool.Point.Min(halfPanel - delta, virtualMap.Position);
            virtualMap.Position = Tool.Point.Max(halfPanel - virtualMap.Size + delta, virtualMap.Position);

            RefreshIcons();
        }
        private void CloseConnection()
        {
            mtxUseDS.WaitOne();

            propertyGrid1.SelectedObject = null;

            currDisplayedItems = null;

            dsDeployables.Clear();
            dsAlivePlayers.Clear();
            dsOnlinePlayers.Clear();
            dsVehicleSpawnPoints.Clear();
            dsVehicles.Clear();

            try
            {
                foreach (KeyValuePair<string, UIDGraph> pair in dicUIDGraph)
                    pair.Value.path.Reset();

                listIcons.Clear();

                Enable(false);
                textBoxStatus.Text = "disconnected";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
                Enable(false);
            }

            mtxUseDS.ReleaseMutex();

            mtxUpdateDB.WaitOne();

            if (sqlCnx != null)
                sqlCnx.Close();

            mtxUpdateDB.ReleaseMutex();
        }
        private void BuildIcons()
        {
            mtxUseDS.WaitOne();
            try
            {
                if (bConnected)
                {
                    tbOnlinePlayers.Text = (dsOnlinePlayers.Tables.Count > 0) ? dsOnlinePlayers.Tables[0].Rows.Count.ToString() : "-";
                    tbAlivePlayers.Text = (dsAlivePlayers.Tables.Count > 0) ? dsAlivePlayers.Tables[0].Rows.Count.ToString() : "-";
                    tbVehicles.Text = (dsVehicles.Tables.Count > 0) ? dsVehicles.Tables[0].Rows.Count.ToString() : "-";
                    tbVehicleSpawn.Text = (dsVehicleSpawnPoints.Tables.Count > 0) ? dsVehicleSpawnPoints.Tables[0].Rows.Count.ToString() : "-";
                    tbDeployables.Text = (dsDeployables.Tables.Count > 0) ? dsDeployables.Tables[0].Rows.Count.ToString() : "-";

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
                MessageBox.Show(ex.Message, "Exception found");
            }
            mtxUseDS.ReleaseMutex();

            if (currDisplayedItems == null)
                return;

            mtxUseDS.WaitOne();

            try
            {
                listIcons.Clear();

                switch (currDisplayedItems.Name)
                {
                    case "radioButtonOnline": BuildOnlineIcons(); break;
                    case "radioButtonAlive": BuildAliveIcons(); break;
                    case "radioButtonVehicles": BuildVehicleIcons(); break;
                    case "radioButtonSpawn": BuildSpawnIcons(); break;
                    case "radioButtonDeployables": BuildDeployableIcons(); break;
                }

                RefreshIcons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }

            mtxUseDS.ReleaseMutex();
        }
        private void RefreshIcons()
        {
            mtxUseDS.WaitOne();

            if (virtualMap.Enabled)
            {
                tileRequests.Clear();

                Rectangle recPanel = new Rectangle(Point.Empty, splitContainer1.Panel1.Size);
                int tileDepth = virtualMap.Depth;
                Size tileCount = virtualMap.TileCount;

                for (int x = 0; x < tileCount.Width; x++)
                {
                    for (int y = 0; y < tileCount.Height; y++)
                    {
                        Rectangle recTile = virtualMap.TileRectangle(new Tool.Point(x, y));

                        if (recPanel.IntersectsWith(recTile))
                        {
                            tileReq req = new tileReq();
                            req.path = virtualMap.nfo.tileBasePath + tileDepth + "\\Tile" + y.ToString("000") + x.ToString("000") + ".jpg";
                            req.rec = recTile;
                            tileRequests.Add(req);
                        }
                    }
                }

                long now_ticks = DateTime.Now.Ticks;

                foreach (tileReq req in tileRequests)
                {
                    tileNfo nfo = tileCache.Find(x => req.path == x.path);
                    if (nfo == null)
                        tileCache.Add(nfo = new tileNfo(req.path));
                    else
                        nfo.ticks = now_ticks;
                }

                tileCache.RemoveAll(x => now_ticks - x.ticks > 10 * 10000000L);
            }

            if (currDisplayedItems != null)
                foreach (iconDB idb in listIcons)
                    idb.icon.Location = virtualMap.UnitToPanel(idb);

            splitContainer1.Panel1.Invalidate();

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

                idb.uid = row.Field<string>("unique_id");
                idb.type = UIDType.TypePlayer;
                idb.row = row;
                idb.pos = GetUnitPosFromString(row.Field<string>("worldspace"));

                if (idx >= iconPlayers.Count)
                {
                    myIcon icon = new myIcon();
                    icon.Click += OnPlayerClick;
                    iconPlayers.Add(icon);
                }

                idb.icon = iconPlayers[idx];
                idb.icon.image = global::DBAccess.Properties.Resources.iconOnline;
                idb.icon.Size = idb.icon.image.Size;
                idb.icon.iconDB = idb;

                //toolTip1.SetToolTip(idb.icon, row.Field<string>("name"));

                listIcons.Add(idb);

                if (checkBoxShowTrail.Checked)
                    GetUIDGraph(idb.uid).AddPoint(idb.pos);

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

                idb.uid = row.Field<string>("unique_id");
                idb.type = UIDType.TypePlayer;
                idb.row = row;
                idb.pos = GetUnitPosFromString(row.Field<string>("worldspace"));

                if (idx >= iconPlayers.Count)
                {
                    myIcon icon = new myIcon();
                    icon.Click += OnPlayerClick;
                    iconPlayers.Add(icon);
                }

                idb.icon = iconPlayers[idx];
                idb.icon.image = global::DBAccess.Properties.Resources.iconAlive;
                idb.icon.Size = idb.icon.image.Size;
                idb.icon.iconDB = idb;

                //toolTip1.SetToolTip(idb.icon, row.Field<string>("name"));

                listIcons.Add(idb);

                if (checkBoxShowTrail.Checked)
                    GetUIDGraph(idb.uid).AddPoint(idb.pos);

                idx++;
            }
        }
        private void BuildVehicleIcons()
        {
            int idx = 0;
            foreach (DataRow row in dsVehicles.Tables[0].Rows)
            {
                DataRow rowT = mycfg.vehicle_types.Tables[0].Rows.Find(row.Field<string>("class_name"));
                if (rowT.Field<bool>("Show"))
                {
                    double damage = row.Field<double>("damage");

                    if (idx >= iconsDB.Count)
                        iconsDB.Add(new iconDB());

                    iconDB idb = iconsDB[idx];

                    idb.uid = row.Field<UInt64>("id").ToString();
                    idb.type = UIDType.TypeVehicle;
                    idb.row = row;
                    idb.pos = GetUnitPosFromString(row.Field<string>("worldspace"));

                    if (idx >= iconVehicles.Count)
                    {
                        myIcon icon = new myIcon();
                        icon.Click += OnVehicleClick;
                        iconVehicles.Add(icon);
                    }

                    idb.icon = iconVehicles[idx];

                    if (damage < 1.0f)
                    {
                        string classname = (rowT != null) ? rowT.Field<string>("Type") : "";
                        switch (classname)
                        {
                            case "Air": idb.icon.image = global::DBAccess.Properties.Resources.air; break;
                            case "Atv": idb.icon.image = global::DBAccess.Properties.Resources.atv; break;
                            case "Bicycle": idb.icon.image = global::DBAccess.Properties.Resources.bike; break;
                            case "Boat": idb.icon.image = global::DBAccess.Properties.Resources.boat; break;
                            case "Bus": idb.icon.image = global::DBAccess.Properties.Resources.bus; break;
                            case "Car": idb.icon.image = global::DBAccess.Properties.Resources.car; break;
                            case "Helicopter": idb.icon.image = global::DBAccess.Properties.Resources.helicopter; break;
                            case "Motorcycle": idb.icon.image = global::DBAccess.Properties.Resources.motorcycle; break;
                            case "Tractor": idb.icon.image = global::DBAccess.Properties.Resources.tractor; break;
                            case "Truck": idb.icon.image = global::DBAccess.Properties.Resources.truck; break;
                            case "UAZ": idb.icon.image = global::DBAccess.Properties.Resources.uaz; break;
                            default: idb.icon.image = global::DBAccess.Properties.Resources.car; break;
                        }
                    }
                    else
                    {
                        string classname = (rowT != null) ? rowT.Field<string>("Type") : "";
                        switch (classname)
                        {
                            case "Air": idb.icon.image = global::DBAccess.Properties.Resources.helicopter_crashed; break;
                            case "Atv": idb.icon.image = global::DBAccess.Properties.Resources.atv_crashed; break;
                            case "Bicycle": idb.icon.image = global::DBAccess.Properties.Resources.bike_crashed; break;
                            case "Boat": idb.icon.image = global::DBAccess.Properties.Resources.boat_crashed; break;
                            case "Bus": idb.icon.image = global::DBAccess.Properties.Resources.bus_crashed; break;
                            case "Car": idb.icon.image = global::DBAccess.Properties.Resources.car_crashed; break;
                            case "Helicopter": idb.icon.image = global::DBAccess.Properties.Resources.helicopter_crashed; break;
                            case "Motorcycle": idb.icon.image = global::DBAccess.Properties.Resources.motorcycle_crashed; break;
                            case "Tractor": idb.icon.image = global::DBAccess.Properties.Resources.tractor_crashed; break;
                            case "Truck": idb.icon.image = global::DBAccess.Properties.Resources.truck_crashed; break;
                            case "UAZ": idb.icon.image = global::DBAccess.Properties.Resources.uaz_crashed; break;
                            default: idb.icon.image = global::DBAccess.Properties.Resources.car_crashed; break;
                        }
                    }

                    Control tr = new Control();
                    tr.ContextMenuStrip = null;
                    idb.icon.iconDB = idb;
                    idb.icon.Size = idb.icon.image.Size;
                    idb.icon.contextMenuStrip = contextMenuStripVehicle;

                    //toolTip1.SetToolTip(idb.icon, row.Field<UInt64>("id").ToString() + ": "+ row.Field<string>("class_name"));

                    listIcons.Add(idb);

                    if (checkBoxShowTrail.Checked)
                        GetUIDGraph(idb.uid).AddPoint(idb.pos);

                    idx++;
                }
            }
        }
        private void BuildSpawnIcons()
        {
            if (dsVehicleSpawnPoints.Tables.Count == 0)
                return;

            int idx = 0;
            foreach (DataRow row in dsVehicleSpawnPoints.Tables[0].Rows)
            {
                DataRow rowT = mycfg.vehicle_types.Tables[0].Rows.Find(row.Field<string>("class_name"));
                if (rowT.Field<bool>("Show"))
                {
                    if (idx >= iconsDB.Count)
                        iconsDB.Add(new iconDB());

                    iconDB idb = iconsDB[idx];

                    idb.uid = row.Field<UInt64>("id").ToString();
                    idb.type = UIDType.TypeSpawn;
                    idb.row = row;
                    idb.pos = GetUnitPosFromString(row.Field<string>("worldspace"));

                    if (idx >= iconVehicles.Count)
                    {
                        myIcon icon = new myIcon();
                        icon.Click += OnVehicleClick;
                        iconVehicles.Add(icon);
                    }

                    idb.icon = iconVehicles[idx];

                    string classname = (rowT != null) ? rowT.Field<string>("Type") : "";
                    switch (classname)
                    {
                        case "Air": idb.icon.image = global::DBAccess.Properties.Resources.air; break;
                        case "Bicycle": idb.icon.image = global::DBAccess.Properties.Resources.bike; break;
                        case "Boat": idb.icon.image = global::DBAccess.Properties.Resources.boat; break;
                        case "Bus": idb.icon.image = global::DBAccess.Properties.Resources.bus; break;
                        case "Car": idb.icon.image = global::DBAccess.Properties.Resources.car; break;
                        case "Helicopter": idb.icon.image = global::DBAccess.Properties.Resources.helicopter; break;
                        case "Motorcycle": idb.icon.image = global::DBAccess.Properties.Resources.motorcycle; break;
                        case "Truck": idb.icon.image = global::DBAccess.Properties.Resources.truck; break;
                        default: idb.icon.image = global::DBAccess.Properties.Resources.car; break;
                    }

                    idb.icon.iconDB = idb;
                    idb.icon.Size = idb.icon.image.Size;
                    idb.icon.contextMenuStrip = contextMenuStripSpawn;

                    //toolTip1.SetToolTip(idb.icon, row.Field<UInt64>("id").ToString() + ": " + row.Field<string>("class_name"));

                    listIcons.Add(idb);

                    idx++;
                }
            }
        }
        private void BuildDeployableIcons()
        {
            int idx = 0;
            foreach (DataRow row in dsDeployables.Tables[0].Rows)
            {
                string name = row.Field<string>("class_name");
                DataRow rowT = mycfg.deployable_types.Tables[0].Rows.Find(name);
                if (rowT.Field<bool>("Show"))
                {
                    if (idx >= iconsDB.Count)
                        iconsDB.Add(new iconDB());

                    iconDB idb = iconsDB[idx];

                    idb.uid = row.Field<UInt64>("id").ToString();
                    idb.type = UIDType.TypeDeployable;
                    idb.row = row;
                    idb.pos = GetUnitPosFromString(row.Field<string>("worldspace"));

                    if (idx >= iconDeployables.Count)
                    {
                        myIcon icon = new myIcon();
                        icon.Click += OnDeployableClick;
                        iconDeployables.Add(icon);
                    }

                    idb.icon = iconDeployables[idx];
                    idb.icon.iconDB = idb;

                    string classname = (rowT != null) ? rowT.Field<string>("Type") : "";
                    switch (classname)
                    {
                        case "Tent": idb.icon.image = global::DBAccess.Properties.Resources.tent; break;
                        case "Stach": idb.icon.image = global::DBAccess.Properties.Resources.stach; break;
                        case "SmallBuild": idb.icon.image = global::DBAccess.Properties.Resources.small_build; break;
                        case "LargeBuild": idb.icon.image = global::DBAccess.Properties.Resources.large_build; break;
                        default: idb.icon.image = global::DBAccess.Properties.Resources.unknown; break;
                    }

                    idb.icon.Size = idb.icon.image.Size;
                    listIcons.Add(idb);

                    idx++;
                }
            }
        }
        private void Enable(bool bState)
        {
            bool bEpochGameType = (mycfg.game_type == "Epoch");
            bConnected = bState;

            buttonConnect.Enabled = !bState;

            radioButtonOnline.Enabled = bState;
            radioButtonAlive.Enabled = bState;
            radioButtonVehicles.Enabled = bState;
            radioButtonSpawn.Enabled = bState && !bEpochGameType;
            radioButtonDeployables.Enabled = bState;

            //
            textBoxURL.Enabled = !bState;
            textBoxBaseName.Enabled = !bState;
            textBoxPort.Enabled = !bState;
            textBoxUser.Enabled = !bState;
            textBoxPassword.Enabled = !bState;
            comboBoxGameType.Enabled = !bState;

            // Script buttons
            buttonBackup.Enabled = bState;
            buttonSelectCustom1.Enabled = bState;
            buttonSelectCustom2.Enabled = bState;
            buttonSelectCustom3.Enabled = bState;
            buttonCustom1.Enabled = (bState) ? !Tool.NullOrEmpty(mycfg.customscript1) : false;
            buttonCustom2.Enabled = (bState) ? !Tool.NullOrEmpty(mycfg.customscript2) : false;
            buttonCustom3.Enabled = (bState) ? !Tool.NullOrEmpty(mycfg.customscript3) : false;

            // Epoch disabled controls...
            textBoxInstanceId.Enabled = !(bState || bEpochGameType);
            buttonRemoveDestroyed.Enabled = bState && !bEpochGameType;
            buttonSpawnNew.Enabled = bState && !bEpochGameType;
            buttonRemoveBodies.Enabled = bState && !bEpochGameType;
            buttonRemoveTents.Enabled = bState && !bEpochGameType;

            if (!bState)
            {
                radioButtonOnline.Checked = false;
                radioButtonAlive.Checked = false;
                radioButtonVehicles.Checked = false;
                radioButtonSpawn.Checked = false;
                radioButtonDeployables.Checked = false;

                tbAlivePlayers.Text = "";
                tbOnlinePlayers.Text = "";
                tbVehicles.Text = "";
                tbVehicleSpawn.Text = "";
                tbDeployables.Text = "";
            }
        }
        private void LoadConfigFile()
        {
            try
            {
                if (Directory.Exists(configPath) == false)
                    Directory.CreateDirectory(configPath);

                XmlSerializer xs = new XmlSerializer(typeof(myConfig));
                using (StreamReader re = new StreamReader(configFilePath))
                {
                    mycfg = xs.Deserialize(re) as myConfig;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
                Enable(false);
            }

            if (mycfg.cfgVersion == null) mycfg.cfgVersion = new ModuleVersion();
            if (Tool.NullOrEmpty(mycfg.game_type)) mycfg.game_type = comboBoxGameType.Items[0] as string;
            if (Tool.NullOrEmpty(mycfg.url)) mycfg.url = "";
            if (Tool.NullOrEmpty(mycfg.port)) mycfg.port = "3306";
            if (Tool.NullOrEmpty(mycfg.basename)) mycfg.basename = "basename";
            if (Tool.NullOrEmpty(mycfg.username)) mycfg.username = "username";
            if (Tool.NullOrEmpty(mycfg.password)) mycfg.password = "password";
            if (Tool.NullOrEmpty(mycfg.instance_id)) mycfg.instance_id = "1";
            if (Tool.NullOrEmpty(mycfg.vehicle_limit)) mycfg.vehicle_limit = "50";
            if (Tool.NullOrEmpty(mycfg.body_time_limit)) mycfg.body_time_limit = "7";
            if (Tool.NullOrEmpty(mycfg.tent_time_limit)) mycfg.tent_time_limit = "7";
            if (Tool.NullOrEmpty(mycfg.online_time_limit)) mycfg.online_time_limit = "5";

            // Custom scripts
            if (!Tool.NullOrEmpty(mycfg.customscript1))
            {
                FileInfo fi = new FileInfo(mycfg.customscript1);
                if (fi.Exists)
                    buttonCustom1.Text = fi.Name;
            }
            if (!Tool.NullOrEmpty(mycfg.customscript2))
            {
                FileInfo fi = new FileInfo(mycfg.customscript2);
                if (fi.Exists)
                    buttonCustom2.Text = fi.Name;
            }
            if (!Tool.NullOrEmpty(mycfg.customscript3))
            {
                FileInfo fi = new FileInfo(mycfg.customscript3);
                if (fi.Exists)
                    buttonCustom3.Text = fi.Name;
            }

            if (mycfg.worlds_def.Tables.Count == 0)
            {
                DataTable table = mycfg.worlds_def.Tables.Add();
                table.Columns.Add(new DataColumn("World ID", typeof(UInt16)));
                table.Columns.Add(new DataColumn("World Name", typeof(string)));
                table.Columns.Add(new DataColumn("Filepath", typeof(string)));
                table.Columns.Add(new DataColumn("RatioX", typeof(float), "", MappingType.Hidden));
                table.Columns.Add(new DataColumn("RatioY", typeof(float), "", MappingType.Hidden));
                table.Columns.Add(new DataColumn("TileSizeX", typeof(int), "", MappingType.Hidden));
                table.Columns.Add(new DataColumn("TileSizeY", typeof(int), "", MappingType.Hidden));
                table.Columns.Add(new DataColumn("TileDepth", typeof(int), "", MappingType.Hidden));
                table.Columns.Add(new DataColumn("DB_X", typeof(int), "", MappingType.Hidden));
                table.Columns.Add(new DataColumn("DB_Y", typeof(int), "", MappingType.Hidden));
                table.Columns.Add(new DataColumn("DB_Width", typeof(UInt32), "", MappingType.Hidden));
                table.Columns.Add(new DataColumn("DB_Height", typeof(UInt32), "", MappingType.Hidden));
                table.Columns.Add(new DataColumn("DB_refWidth", typeof(UInt32), "", MappingType.Hidden));
                table.Columns.Add(new DataColumn("DB_refHeight", typeof(UInt32), "", MappingType.Hidden));

                DataColumn[] keys = new DataColumn[1];
                keys[0] = mycfg.worlds_def.Tables[0].Columns[0];
                mycfg.worlds_def.Tables[0].PrimaryKey = keys;

                System.Data.DataColumn col = new DataColumn();

                table.Rows.Add(1, "Chernarus", "", 0, 0, 0, 0, 0, 0, 0, 14700, 15360, 14700, 15360);
                table.Rows.Add(2, "Lingor", "", 0, 0, 0, 0, 0, 0, 0, 10000, 10000, 10000, 10000);
                table.Rows.Add(3, "Utes", "", 0, 0, 0, 0, 0, 0, 0, 5100, 5100, 5100, 5100);
                table.Rows.Add(4, "Takistan", "", 0, 0, 0, 0, 0, 0, 0, 14000, 14000, 14000, 14000);
                table.Rows.Add(5, "Panthera2", "", 0, 0, 0, 0, 0, 0, 0, 10200, 10200, 10200, 10200);
                table.Rows.Add(6, "Fallujah", "", 0, 0, 0, 0, 0, 0, 0, 10200, 10200, 10200, 10200);
                table.Rows.Add(7, "Zargabad", "", 0, 0, 0, 0, 0, 0, 0, 8000, 8000, 8000, 8000);
                table.Rows.Add(8, "Namalsk", "", 0, 0, 0, 0, 0, 0, 0, 12000, 12000, 12000, 12000);
                table.Rows.Add(9, "Celle2", "", 0, 0, 0, 0, 0, 0, 0, 13000, 13000, 13000, 13000);
                table.Rows.Add(10, "Taviana", "", 0, 0, 0, 0, 0, 0, 0, 25600, 25600, 25600, 25600);
            }

            // -> v4.0
            if (mycfg.cfgVersion < curCfgVersion)
            {
                DataColumnCollection cols;

                // Add Column 'Show' to vehicle_types
                cols = mycfg.vehicle_types.Tables[0].Columns;
                cols.Add(new DataColumn("Show", typeof(bool)));
                foreach (DataRow row in mycfg.vehicle_types.Tables[0].Rows)
                    row.SetField<bool>("Show", true);

                // Add Column 'Show' to deployable_types
                cols = mycfg.deployable_types.Tables[0].Columns;
                cols.Add(new DataColumn("Show", typeof(bool)));
                foreach (DataRow row in mycfg.deployable_types.Tables[0].Rows)
                    row.SetField<bool>("Show", true);
            }

            if (mycfg.vehicle_types.Tables.Count == 0)
            {
                DataTable table = mycfg.vehicle_types.Tables.Add();
                table.Columns.Add(new DataColumn("ClassName", typeof(string)));
                table.Columns.Add(new DataColumn("Type", typeof(string)));
                table.Columns.Add(new DataColumn("Show", typeof(bool)));
                DataColumn[] keys = new DataColumn[1];
                keys[0] = mycfg.vehicle_types.Tables[0].Columns[0];
                mycfg.vehicle_types.Tables[0].PrimaryKey = keys;
            }

            if (mycfg.deployable_types.Tables.Count == 0)
            {
                DataTable table = mycfg.deployable_types.Tables.Add();
                table.Columns.Add(new DataColumn("ClassName", typeof(string)));
                table.Columns.Add(new DataColumn("Type", typeof(string)));
                table.Columns.Add(new DataColumn("Show", typeof(bool)));
                DataColumn[] keys = new DataColumn[1];
                keys[0] = mycfg.deployable_types.Tables[0].Columns[0];
                mycfg.deployable_types.Tables[0].PrimaryKey = keys;
            }

            foreach (DataRow row in mycfg.vehicle_types.Tables[0].Rows)
                row.SetField<bool>("Show", true);

            foreach (DataRow row in mycfg.deployable_types.Tables[0].Rows)
                row.SetField<bool>("Show", true);

            try
            {
                textBoxURL.Text = mycfg.url;
                textBoxPort.Text = mycfg.port;
                textBoxBaseName.Text = mycfg.basename;
                textBoxUser.Text = mycfg.username;
                textBoxPassword.Text = mycfg.password;
                textBoxInstanceId.Text = mycfg.instance_id;
                comboBoxGameType.SelectedItem = mycfg.game_type;
                textBoxVehicleMax.Text = mycfg.vehicle_limit;
                textBoxOldBodyLimit.Text = mycfg.body_time_limit;
                textBoxOldTentLimit.Text = mycfg.tent_time_limit;

                dataGridViewMaps.Columns["ColGVMID"].DataPropertyName = "World ID";
                dataGridViewMaps.Columns["ColGVMName"].DataPropertyName = "World Name";
                dataGridViewMaps.Columns["ColGVMPath"].DataPropertyName = "Filepath";
                dataGridViewMaps.DataSource = mycfg.worlds_def.Tables[0];
                dataGridViewMaps.Sort(dataGridViewMaps.Columns["ColGVMID"], ListSortDirection.Ascending);

                dataGridViewVehicleTypes.Columns["ColGVVTShow"].DataPropertyName = "Show";
                dataGridViewVehicleTypes.Columns["ColGVVTClassName"].DataPropertyName = "ClassName";
                dataGridViewVehicleTypes.Columns["ColGVVTType"].DataPropertyName = "Type";
                dataGridViewVehicleTypes.DataSource = mycfg.vehicle_types.Tables[0];
                dataGridViewVehicleTypes.Sort(dataGridViewVehicleTypes.Columns["ColGVVTClassName"], ListSortDirection.Ascending);

                dataGridViewDeployableTypes.Columns["ColGVDTShow"].DataPropertyName = "Show";
                dataGridViewDeployableTypes.Columns["ColGVDTClassName"].DataPropertyName = "ClassName";
                dataGridViewDeployableTypes.Columns["ColGVDTType"].DataPropertyName = "Type";
                dataGridViewDeployableTypes.DataSource = mycfg.deployable_types.Tables[0];
                dataGridViewDeployableTypes.Sort(dataGridViewDeployableTypes.Columns["ColGVDTClassName"], ListSortDirection.Ascending);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
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
                mycfg.cfgVersion = curCfgVersion;
                mycfg.game_type = comboBoxGameType.SelectedItem as string;
                mycfg.vehicle_limit = textBoxVehicleMax.Text;
                mycfg.body_time_limit = textBoxOldBodyLimit.Text;
                mycfg.tent_time_limit = textBoxOldTentLimit.Text;

                XmlSerializer xs = new XmlSerializer(typeof(myConfig));
                using (StreamWriter wr = new StreamWriter(configFilePath))
                {
                    xs.Serialize(wr, mycfg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }
        }
        private Tool.Point GetUnitPosFromString(string from)
        {
            ArrayList arr = Tool.ParseInventoryString(from);
            // [angle, [X, Y, Z]]

            double x = 0;
            double y = 0;

            if (arr.Count >= 2)
            {
                arr = arr[1] as ArrayList;
                x = double.Parse(arr[0] as string, CultureInfo.InvariantCulture.NumberFormat);
                y = double.Parse(arr[1] as string, CultureInfo.InvariantCulture.NumberFormat);
            }

            x /= virtualMap.nfo.dbRefMapSize.Width;
            y /= virtualMap.nfo.dbRefMapSize.Height;
            y = 1.0f - y;

            return new Tool.Point((float)x, (float)y);
        }
        private void SetCurrentMap()
        {
            try
            {
                mapHelper = null;

                DataRow rowW = mycfg.worlds_def.Tables[0].Rows.Find(mycfg.world_id);
                if (rowW != null)
                {
                    textBoxWorld.Text = rowW.Field<string>("World Name");

                    string filepath = rowW.Field<string>("Filepath");

                    if (File.Exists(filepath))
                    {
                        virtualMap.nfo.tileBasePath = configPath + "\\World" + mycfg.world_id + "\\LOD"; ;
                    }

                    virtualMap.nfo.defTileSize = new Tool.Size(rowW.Field<int>("TileSizeX"), rowW.Field<int>("TileSizeY"));
                    virtualMap.nfo.depth = rowW.Field<int>("TileDepth");
                    virtualMap.SetRatio(new Tool.Size(rowW.Field<float>("RatioX"), rowW.Field<float>("RatioY")));
                    virtualMap.nfo.dbMapSize = new Tool.Size(rowW.Field<UInt32>("DB_Width"), rowW.Field<UInt32>("DB_Height"));
                    virtualMap.nfo.dbRefMapSize = new Tool.Size(rowW.Field<UInt32>("DB_refWidth"), rowW.Field<UInt32>("DB_refHeight"));
                    virtualMap.nfo.dbMapOffsetUnit = new Tool.Point(rowW.Field<int>("DB_X") / virtualMap.nfo.dbRefMapSize.Width,
                                                                     rowW.Field<int>("DB_Y") / virtualMap.nfo.dbRefMapSize.Height);
                }

                if (virtualMap.Enabled)
                    mapZoom.currDepth = Math.Log(virtualMap.ResizeFromZoom((float)Math.Pow(2, mapZoom.currDepth)), 2);

                Tool.Size sizePanel = splitContainer1.Panel1.Size;
                virtualMap.Position = (Tool.Point)((sizePanel - virtualMap.Size) * 0.5f);

                mapHelper = new MapHelper(virtualMap, mycfg.world_id);

                ApplyMapChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }
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
                MySqlCommand cmd = sqlCnx.CreateCommand();
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
                MessageBox.Show(ex.Message, "Exception found");
            }

            mtxUpdateDB.ReleaseMutex();

            mtxUseDS.WaitOne();

            try
            {
                foreach (DataRow row in _dsAllVehicleTypes.Tables[0].Rows)
                {
                    string name = row.Field<string>("class_name");

                    if (mycfg.vehicle_types.Tables[0].Rows.Find(name) == null)
                        mycfg.vehicle_types.Tables[0].Rows.Add(name, "Car", true);
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }

            mtxUseDS.ReleaseMutex();

            if (mycfg.world_id == 0)
            {
                CloseConnection();
                MessageBox.Show("Instance id '" + mycfg.instance_id + "' not found in database", "Warning");
            }
        }
        private void DB_OnRefresh()
        {
            if (bConnected)
            {
                DataSet _dsAlivePlayers = new DataSet();
                DataSet _dsOnlinePlayers = new DataSet();
                DataSet _dsDeployables = new DataSet();
                DataSet _dsVehicles = new DataSet();
                DataSet _dsVehicleSpawnPoints = new DataSet();

                mtxUpdateDB.WaitOne();

                try
                {
                    MySqlCommand cmd = sqlCnx.CreateCommand();
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
                    cmd.CommandText = "SELECT w.id id, w.worldspace worldspace, w.chance chance, v.inventory inventory, v.class_name class_name FROM world_vehicle as w, vehicle as v"
                                    + " WHERE w.world_id=" + mycfg.world_id + " AND w.vehicle_id=v.id";
                    _dsVehicleSpawnPoints.Clear();
                    adapter.Fill(_dsVehicleSpawnPoints);

                    //
                    //  Deployables
                    //
                    cmd.CommandText = "SELECT id.id id, d.class_name class_name, id.worldspace, id.inventory"
                                    + " FROM instance_deployable as id, deployable as d"
                                    + " WHERE instance_id=" + mycfg.instance_id
                                    + " AND id.deployable_id=d.id";
                    _dsDeployables.Clear();
                    adapter.Fill(_dsDeployables);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception found");
                }

                foreach (DataRow row in _dsDeployables.Tables[0].Rows)
                {
                    string name = row.Field<string>("class_name");

                    if (mycfg.deployable_types.Tables[0].Rows.Find(name) == null)
                        mycfg.deployable_types.Tables[0].Rows.Add(name, "Unknown", true);
                }

                mtxUpdateDB.ReleaseMutex();

                mtxUseDS.WaitOne();

                dsDeployables = _dsDeployables.Copy();
                dsAlivePlayers = _dsAlivePlayers.Copy();
                dsOnlinePlayers = _dsOnlinePlayers.Copy();
                dsVehicles = _dsVehicles.Copy();
                dsVehicleSpawnPoints = _dsVehicleSpawnPoints.Copy();

                mtxUseDS.ReleaseMutex();
            }
        }
        private void DBEpoch_OnConnection()
        {
            if (!bConnected)
                return;

            mtxUpdateDB.WaitOne();

            DataSet _dsVehicles = new DataSet();

            this.textBoxCmdStatus.Text = "";

            try
            {
                MySqlCommand cmd = sqlCnx.CreateCommand();
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                //
                //  Vehicle types
                //
                cmd.CommandText = "SELECT ClassName class_name FROM object_data WHERE CharacterID=0";
                _dsVehicles.Clear();
                adapter.Fill(_dsVehicles);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }

            mtxUpdateDB.ReleaseMutex();

            mtxUseDS.WaitOne();

            try
            {
                foreach (DataRow row in _dsVehicles.Tables[0].Rows)
                {
                    string name = row.Field<string>("class_name");

                    if (mycfg.vehicle_types.Tables[0].Rows.Find(name) == null)
                        mycfg.vehicle_types.Tables[0].Rows.Add(name, "Car", true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }

            mtxUseDS.ReleaseMutex();
        }
        private void DBEpoch_OnRefresh()
        {
            if (bConnected)
            {
                DataSet _dsAlivePlayers = new DataSet();
                DataSet _dsOnlinePlayers = new DataSet();
                DataSet _dsDeployables = new DataSet();
                DataSet _dsVehicles = new DataSet();

                mtxUpdateDB.WaitOne();

                try
                {
                    MySqlCommand cmd = sqlCnx.CreateCommand();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    //
                    //  Players alive
                    //
                    cmd.CommandText = "SELECT cd.CharacterID id, pd.PlayerUID unique_id, pd.PlayerName name, cd.Humanity humanity, cd.worldspace worldspace,"
                                    + " cd.inventory inventory, cd.backpack backpack, cd.medical medical, cd.CurrentState state, cd.DateStamp last_updated"
                                    + " FROM character_data as cd, player_data as pd"
                                    + " WHERE cd.PlayerUID=pd.PlayerUID AND cd.Alive=1";
                    _dsAlivePlayers.Clear();
                    adapter.Fill(_dsAlivePlayers);

                    //
                    //  Players online
                    //
                    cmd.CommandText += " AND cd.DateStamp > now() - interval " + mycfg.online_time_limit + " minute";
                    _dsOnlinePlayers.Clear();
                    adapter.Fill(_dsOnlinePlayers);

                    //
                    //  Vehicles
                    cmd.CommandText = "SELECT CAST(ObjectID AS UNSIGNED) id, CAST(0 AS UNSIGNED) spawn_id, ClassName class_name, worldspace, inventory,"
                                    + " fuel, damage, DateStamp last_updated"
                                    + " FROM object_data WHERE CharacterID=0";
                    _dsVehicles.Clear();
                    adapter.Fill(_dsVehicles);

                    //
                    //  Deployables
                    //
                    cmd.CommandText = "SELECT CAST(ObjectID AS UNSIGNED) id, Classname class_name, worldspace, inventory FROM object_data WHERE CharacterID!=0";
                    _dsDeployables.Clear();
                    adapter.Fill(_dsDeployables);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception found");
                }

                foreach (DataRow row in _dsDeployables.Tables[0].Rows)
                {
                    string name = row.Field<string>("class_name");

                    if (mycfg.deployable_types.Tables[0].Rows.Find(name) == null)
                        mycfg.deployable_types.Tables[0].Rows.Add(name, "Unknown", true);
                }

                mtxUpdateDB.ReleaseMutex();

                mtxUseDS.WaitOne();

                dsDeployables = _dsDeployables.Copy();
                dsAlivePlayers = _dsAlivePlayers.Copy();
                dsOnlinePlayers = _dsOnlinePlayers.Copy();
                dsVehicles = _dsVehicles.Copy();

                mycfg.world_id = 1;

                mtxUseDS.ReleaseMutex();
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
                MySqlCommand cmd = sqlCnx.CreateCommand();

                cmd.CommandText = query;

                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
                Enable(false);
            }

            mtxUpdateDB.ReleaseMutex();
            this.Cursor = Cursors.Arrow;

            return res;
        }

        internal UIDGraph GetUIDGraph(string uid)
        {
            UIDGraph uidgraph = null;

            if (dicUIDGraph.TryGetValue(uid, out uidgraph) == false)
                dicUIDGraph[uid] = uidgraph = new UIDGraph(pens[(penCount++) % 6]);

            return uidgraph;
        }

        public delegate void DlgUpdateIcons();

        private void BackupDatabase(string filename)
        {
            if (!bConnected)
                return;

            this.Cursor = Cursors.WaitCursor;
            mtxUpdateDB.WaitOne();

            try
            {
                FileInfo fi = new FileInfo(filename);
                StreamWriter sw = fi.CreateText();

                MySqlCommand cmd = sqlCnx.CreateCommand();
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                MySqlDataReader reader;

                cmd.CommandText = "SHOW TABLES";
                reader = cmd.ExecuteReader();

                List<string> tables = new List<string>();
                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    tables.Add(name);
                }

                reader.Close();

                List<string> queries_tables = new List<string>();
                foreach (string table in tables)
                {
                    textBoxCmdStatus.Text += "\r\nReading table `" + table + "`...";

                    bool bSerializeTable = true;

                    cmd.CommandText = "SHOW CREATE TABLE " + table;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string str = reader.GetString(1);

                        if (!(str.Contains("VIEW") && str.Contains("ALGORITHM")))
                        {
                            queries_tables.Add("\r\nDROP TABLE IF EXISTS `" + table + "`");
                            queries_tables.Add(str);
                        }
                        else
                        {
                            queries_tables.Add("\r\n-- View `" + table + "` has been ignored. --");
                            bSerializeTable = false;
                        }
                    }
                    reader.Close();

                    if (bSerializeTable)
                    {
                        cmd.CommandText = "SELECT * FROM " + table;
                        reader = cmd.ExecuteReader();
                        List<string> list_values = new List<string>();
                        while (reader.Read())
                        {
                            string values = "";
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                Type type = reader.GetFieldType(i);

                                if (reader.IsDBNull(i))
                                {
                                    values += "NULL,";
                                }
                                else if (type == typeof(string))
                                {
                                    string res = "";
                                    string s = reader.GetString(i);
                                    foreach (char c in s)
                                    {
                                        if (c == '\'' || c == '\"')
                                            res += "\\";
                                        res += c;
                                    }
                                    values += "'" + res + "',";
                                }
                                else if (type == typeof(DateTime))
                                {
                                    //  'YYYY-MM-DD h24:mm:ss'
                                    DateTime dt = reader.GetDateTime(i).ToUniversalTime();
                                    values += "'" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                                }
                                else
                                {
                                    values += reader.GetString(i) + ",";
                                }
                            }
                            values = values.TrimEnd(',');
                            list_values.Add("(" + values + ")");
                        }
                        reader.Close();

                        string all_values = "";
                        foreach (string values in list_values)
                            all_values += values + ",";
                        all_values = all_values.TrimEnd(',');

                        if (list_values.Count > 0)
                        {
                            queries_tables.Add("LOCK TABLES `" + table + "` WRITE");
                            queries_tables.Add("REPLACE INTO `" + table + "` VALUES " + all_values);
                            queries_tables.Add("UNLOCK TABLES");
                        }

                        textBoxCmdStatus.Text += " done.";
                    }
                    else
                    {
                        textBoxCmdStatus.Text += " ignored.";
                    }
                }

                foreach (string query in queries_tables)
                {
                    sw.Write(query + ";\r\n");
                }

                sw.Close();

                textBoxCmdStatus.Text += "\r\nBackup of " + tables.Count + " tables done.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
                Enable(false);
            }

            mtxUpdateDB.ReleaseMutex();
            this.Cursor = Cursors.Arrow;
        }
    }
}