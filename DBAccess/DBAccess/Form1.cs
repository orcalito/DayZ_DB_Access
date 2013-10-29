using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        #region Fields
        static ModuleVersion curCfgVersion = new ModuleVersion(4, 1);

        private static int bUserAction = 0;
        private static EventWaitHandle eventFastBgWorker = new EventWaitHandle(false, EventResetMode.AutoReset);
        private DlgUpdateIcons dlgUpdateIcons;
        private DlgUpdateIcons dlgRefreshMap;
        //
        private myConfig mycfg = new myConfig(curCfgVersion);
        private string configPath;
        private string configFilePath;
        private VirtualMap virtualMap = new VirtualMap();
        //
        private myDatabase myDB = new myDatabase();
        //
        private Dictionary<string, UIDGraph> dicUIDGraph = new Dictionary<string, UIDGraph>();
        private List<iconDB> listIcons = new List<iconDB>();
        private List<iconDB> iconsDB = new List<iconDB>();
        private List<myIcon> iconPlayers = new List<myIcon>();
        private List<myIcon> iconVehicles = new List<myIcon>();
        private List<myIcon> iconDeployables = new List<myIcon>();
        private MapPan mapPan = new MapPan();
        private MapZoom mapZoom = new MapZoom(eventFastBgWorker);
        private MapHelper mapHelper;
        private UIDGraph cartographer = new UIDGraph(new Pen(Color.Black, 2));
        private Tool.Point positionInDB = new Tool.Point();

        private List<tileReq> tileRequests = new List<tileReq>();
        private List<tileNfo> tileCache = new List<tileNfo>();
        private bool bShowTrails = false;
        private bool bCartographer = false;

        private bool IsMapHelperEnabled { get { return (mapHelper != null) && mapHelper.enabled; } }

        private Dictionary<displayMode, ToolStripStatusLabel> ModeButtons = new Dictionary<displayMode, ToolStripStatusLabel>();
        private displayMode _lastMode = displayMode.InTheVoid;
        private displayMode lastMode { get { return _lastMode;  } }
        private displayMode _currentMode = displayMode.InTheVoid;
        private displayMode currentMode
        {
            get
            {
                return _currentMode;
            }
            set
            {
                _lastMode = _currentMode;
                _currentMode = value;

                if (_lastMode != _currentMode)
                {
                    switch (_lastMode)
                    {
                        case displayMode.MapHelper:
                            _MapHelperStateChanged();
                            mapHelper.enabled = false;
                            break;
                        default:
                            break;
                    }

                    switch (_currentMode)
                    {
                        case displayMode.MapHelper:
                            mapHelper.enabled = true;
                            splitContainer1.Panel1.Invalidate();
                            break;

                        case displayMode.ShowOnline:
                        case displayMode.ShowAlive:
                        case displayMode.ShowVehicle:
                        case displayMode.ShowSpawn:
                        case displayMode.ShowDeployable:
                            this.Cursor = Cursors.WaitCursor;
                            propertyGrid1.SelectedObject = null;
                            BuildIcons();
                            this.Cursor = Cursors.Arrow;
                            break;

                        default: 
                            break;
                    }

                    foreach(var item in ModeButtons)
                    {
                        item.Value.Font = new System.Drawing.Font("Segoe UI", 9F);
                        item.Value.BorderSides = ToolStripStatusLabelBorderSides.None;
                    }
                    
                    if(_currentMode != displayMode.InTheVoid)
                    {
                        var selected = ModeButtons[_currentMode];
                        selected.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                        selected.BorderSides = ToolStripStatusLabelBorderSides.All;
                    }

                    dataGridViewMaps.Visible = (value == displayMode.SetMaps);
                }
            }
        }
        #endregion

        public Form1()
        {
            InitializeComponent();

            //
            ModeButtons.Add(displayMode.SetMaps, toolStripStatusWorld);
            ModeButtons.Add(displayMode.ShowOnline, toolStripStatusOnline);
            ModeButtons.Add(displayMode.ShowAlive, toolStripStatusAlive);
            ModeButtons.Add(displayMode.ShowVehicle, toolStripStatusVehicle);
            ModeButtons.Add(displayMode.ShowSpawn, toolStripStatusSpawn);
            ModeButtons.Add(displayMode.ShowDeployable, toolStripStatusDeployable);
            ModeButtons.Add(displayMode.MapHelper, toolStripStatusMapHelper);

            //
            configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DayZDBAccess";
            configFilePath = configPath + "\\config.xml";
            //
            currentMode = displayMode.InTheVoid;
            //
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
            myDB.CloseConnection();

            propertyGrid1.SelectedObject = null;
            currentMode = displayMode.InTheVoid;

            try
            {
                foreach (KeyValuePair<string, UIDGraph> pair in dicUIDGraph)
                    pair.Value.ResetPaths();

                listIcons.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }

            Enable(false);
        }
        private void BuildIcons()
        {
            if (myDB.Connected)
            {
                myDB.UseDS(true);

                try
                {
                    toolStripStatusOnline.Text = (myDB.OnlinePlayers.Tables.Count > 0) ? myDB.OnlinePlayers.Tables[0].Rows.Count.ToString() : "-";
                    toolStripStatusAlive.Text = (myDB.AlivePlayers.Tables.Count > 0) ? myDB.AlivePlayers.Tables[0].Rows.Count.ToString() : "-";
                    toolStripStatusVehicle.Text = (myDB.Vehicles.Tables.Count > 0) ? myDB.Vehicles.Tables[0].Rows.Count.ToString() : "-";
                    toolStripStatusSpawn.Text = (myDB.SpawnPoints.Tables.Count > 0) ? myDB.SpawnPoints.Tables[0].Rows.Count.ToString() : "-";
                    toolStripStatusDeployable.Text = (myDB.Deployables.Tables.Count > 0) ? myDB.Deployables.Tables[0].Rows.Count.ToString() : "-";

                    if ((propertyGrid1.SelectedObject != null) && (propertyGrid1.SelectedObject is PropObjectBase))
                    {
                        PropObjectBase obj = propertyGrid1.SelectedObject as PropObjectBase;

                        obj.Rebuild();

                        propertyGrid1.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception found");
                }

                try
                {
                    listIcons.Clear();

                    switch (currentMode)
                    {
                        case displayMode.ShowOnline: BuildOnlineIcons(); break;
                        case displayMode.ShowAlive: BuildAliveIcons(); break;
                        case displayMode.ShowVehicle: BuildVehicleIcons(); break;
                        case displayMode.ShowSpawn: BuildSpawnIcons(); break;
                        case displayMode.ShowDeployable: BuildDeployableIcons(); break;
                    }

                    RefreshIcons();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception found");
                }

                myDB.UseDS(false);
            }
        }
        private void RefreshIcons()
        {
            myDB.UseDS(true);

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

            foreach (iconDB idb in listIcons)
                idb.icon.Location = virtualMap.UnitToPanel(idb);

            splitContainer1.Panel1.Invalidate();

            myDB.UseDS(false);
        }
        private void BuildOnlineIcons()
        {
            int idx = 0;
            foreach (DataRow row in myDB.OnlinePlayers.Tables[0].Rows)
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

                if (bShowTrails == true)
                    GetUIDGraph(idb.uid).AddPoint(idb.pos);

                idx++;
            }
        }
        private void BuildAliveIcons()
        {
            int idx = 0;
            foreach (DataRow row in myDB.AlivePlayers.Tables[0].Rows)
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

                if (bShowTrails == true)
                    GetUIDGraph(idb.uid).AddPoint(idb.pos);

                idx++;
            }
        }
        private void BuildVehicleIcons()
        {
            int idx = 0;
            foreach (DataRow row in myDB.Vehicles.Tables[0].Rows)
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

                    if (bShowTrails == true)
                        GetUIDGraph(idb.uid).AddPoint(idb.pos);

                    idx++;
                }
            }
        }
        private void BuildSpawnIcons()
        {
            if (myDB.SpawnPoints.Tables.Count == 0)
                return;

            int idx = 0;
            foreach (DataRow row in myDB.SpawnPoints.Tables[0].Rows)
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
            foreach (DataRow row in myDB.Deployables.Tables[0].Rows)
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
            
            buttonConnect.Enabled = !bState;

            toolStripStatusWorld.Enabled = bState;
            toolStripStatusOnline.Enabled = bState;
            toolStripStatusAlive.Enabled = bState;
            toolStripStatusVehicle.Enabled = bState;
            toolStripStatusSpawn.Enabled = bState && !bEpochGameType;
            toolStripStatusDeployable.Enabled = bState;
            toolStripStatusMapHelper.Enabled = bState;

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
            numericUpDownInstanceId.Enabled = !(bState || bEpochGameType);
            buttonRemoveDestroyed.Enabled = bState && !bEpochGameType;
            buttonSpawnNew.Enabled = bState && !bEpochGameType;
            buttonRemoveBodies.Enabled = bState && !bEpochGameType;
            buttonRemoveTents.Enabled = bState && !bEpochGameType;

            if (!bState)
            {
                toolStripStatusAlive.Text = "-";
                toolStripStatusOnline.Text = "-";
                toolStripStatusVehicle.Text = "-";
                toolStripStatusSpawn.Text = "-";
                toolStripStatusDeployable.Text = "-";
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
            catch (Exception /*ex*/)
            {
                //MessageBox.Show(ex.Message, "Exception found");
                Enable(false);
            }

            if (mycfg.cfgVersion == null) mycfg.cfgVersion = new ModuleVersion();
            if (Tool.NullOrEmpty(mycfg.game_type)) mycfg.game_type = comboBoxGameType.Items[0] as string;
            if (Tool.NullOrEmpty(mycfg.url)) mycfg.url = "my.database.url";
            if (Tool.NullOrEmpty(mycfg.port)) mycfg.port = "3306";
            if (Tool.NullOrEmpty(mycfg.basename)) mycfg.basename = "basename";
            if (Tool.NullOrEmpty(mycfg.username)) mycfg.username = "username";
            if (Tool.NullOrEmpty(mycfg.password)) mycfg.password = "password";
            if (Tool.NullOrEmpty(mycfg.instance_id)) mycfg.instance_id = "1";
            if (Tool.NullOrEmpty(mycfg.vehicle_limit)) mycfg.vehicle_limit = "50";
            if (Tool.NullOrEmpty(mycfg.body_time_limit)) mycfg.body_time_limit = "7";
            if (Tool.NullOrEmpty(mycfg.tent_time_limit)) mycfg.tent_time_limit = "7";
            if (Tool.NullOrEmpty(mycfg.online_time_limit)) mycfg.online_time_limit = "5";
            if (mycfg.filter_last_updated == 0) mycfg.filter_last_updated = 7;

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

            if (mycfg.vehicle_types.Tables.Count == 0)
            {
                DataTable table = mycfg.vehicle_types.Tables.Add();
                table.Columns.Add(new DataColumn("ClassName", typeof(string)));
                table.Columns.Add(new DataColumn("Type", typeof(string)));
                table.Columns.Add(new DataColumn("Show", typeof(bool)));
                table.Columns.Add(new DataColumn("Id", typeof(UInt16), "", MappingType.Hidden));
                DataColumn[] keys = new DataColumn[1];
                keys[0] = mycfg.vehicle_types.Tables[0].Columns["ClassName"];
                mycfg.vehicle_types.Tables[0].PrimaryKey = keys;
            }

            // -> v3.0
            if (mycfg.vehicle_types.Tables[0].Columns.Contains("Show") == false)
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

            // -> v4.0
            if (mycfg.vehicle_types.Tables[0].Columns.Contains("Id") == false)
            {
                DataColumnCollection cols;
                // Add Column 'Id' to vehicle_types
                cols = mycfg.vehicle_types.Tables[0].Columns;
                cols.Add(new DataColumn("Id", typeof(UInt16), "", MappingType.Hidden));
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
                numericUpDownInstanceId.Value = Decimal.Parse(mycfg.instance_id);
                comboBoxGameType.SelectedItem = mycfg.game_type;
                textBoxVehicleMax.Text = mycfg.vehicle_limit;
                textBoxOldBodyLimit.Text = mycfg.body_time_limit;
                textBoxOldTentLimit.Text = mycfg.tent_time_limit;
                trackBarLastUpdated.Value = Math.Min(trackBarLastUpdated.Maximum, Math.Max(trackBarLastUpdated.Minimum, mycfg.filter_last_updated));

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
                mycfg.instance_id = numericUpDownInstanceId.Value.ToString();
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
                    toolStripStatusWorld.Text = rowW.Field<string>("World Name");

                    string filepath = rowW.Field<string>("Filepath");

                    if (File.Exists(filepath))
                    {
                        virtualMap.nfo.tileBasePath = configPath + "\\World" + mycfg.world_id + "\\LOD"; ;
                    }
                    else
                    {
                        MessageBox.Show("Please select a bitmap for your world, and don't forget to adjust the map to your bitmap with the map helper...", "No bitmap selected");
                        //tabControl1.SelectedTab = tabPageMaps;
                        currentMode = displayMode.SetMaps;
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
        internal UIDGraph GetUIDGraph(string uid)
        {
            UIDGraph uidgraph = null;

            if (dicUIDGraph.TryGetValue(uid, out uidgraph) == false)
                dicUIDGraph[uid] = uidgraph = new UIDGraph(pens[(penCount++) % 6]);

            return uidgraph;
        }
        public delegate void DlgUpdateIcons();

        #region Callbacks
        private void comboBoxGameType_SelectedValueChanged(object sender, EventArgs e)
        {
            _comboBoxGameType_SelectedValueChanged(sender, e);
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            _buttonConnect_Click(sender, e);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            _Panel1_Paint(sender, e);
        }

        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            _Panel1_MouseClick(sender, e);
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            _Panel1_MouseDown(sender, e);
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            _Panel1_MouseMove(sender, e);
        }

        private void Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            _Panel1_MouseUp(sender, e);
        }

        private void cbCartographer_CheckedChanged(object sender, EventArgs e)
        {
            _cbCartographer_CheckedChanged(sender, e);
        }

        private void buttonSelectCustom_Click(object sender, EventArgs e)
        {
            _buttonSelectCustom_Click(sender, e);
        }

        private void buttonCustom_Click(object sender, EventArgs e)
        {
            _buttonCustom_Click(sender, e);
        }

        private void buttonRemoveTents_Click(object sender, EventArgs e)
        {
            _buttonRemoveTents_Click(sender, e);
        }

        private void buttonRemoveBodies_Click(object sender, EventArgs e)
        {
            _buttonRemoveBodies_Click(sender, e);
        }

        private void buttonSpawnNew_Click(object sender, EventArgs e)
        {
            _buttonSpawnNew_Click(sender, e);
        }

        private void buttonBackup_Click(object sender, EventArgs e)
        {
            _buttonBackup_Click(sender, e);
        }

        private void buttonRemoveDestroyed_Click(object sender, EventArgs e)
        {
            _buttonRemoveDestroyed_Click(sender, e);
        }

        private void dataGridViewMaps_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            _dataGridViewMaps_CellClick(sender, e);
        }

        private void dataGridViewVehicleTypes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            _dataGridViewVehicleTypes_CellContentClick(sender, e);
        }

        private void dataGridViewVehicleTypes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            _dataGridViewVehicleTypes_CellValueChanged(sender, e);
        }

        private void dataGridViewVehicleTypes_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _dataGridViewVehicleTypes_ColumnHeaderMouseDoubleClick(sender, e);
        }

        private void dataGridViewDeployableTypes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            _dataGridViewDeployableTypes_CellContentClick(sender, e);
        }

        private void dataGridViewDeployableTypes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            _dataGridViewDeployableTypes_CellValueChanged(sender, e);
        }

        private void dataGridViewDeployableTypes_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _dataGridViewDeployableTypes_ColumnHeaderMouseDoubleClick(sender, e);
        }
        #endregion

        private void toolStripMenuItemAddVehicle_Click(object sender, EventArgs e)
        {
            if (dataGridViewVehicleTypes.SelectedCells.Count == 1)
            {
                var row = dataGridViewVehicleTypes.Rows[dataGridViewVehicleTypes.SelectedCells[0].RowIndex];
                string classname = row.Cells["ColGVVTClassName"].Value as string;

                var rowT = mycfg.vehicle_types.Tables[0].Rows.Find(classname);
                if (rowT != null)
                {
                    var vehicle_id = rowT.Field<UInt16>("Id");

                    bool bRes = myDB.AddVehicle((currentMode == displayMode.ShowVehicle), classname, vehicle_id, positionInDB);
                    if (!bRes)
                    {
                        MessageBox.Show("Error while trying to insert vehicle instane '" + classname + "' into database");
                    }
                }
            }
        }

        private void contextMenuStripAddVehicle_Opening(object sender, CancelEventArgs e)
        {
            if (!((currentMode == displayMode.ShowVehicle || currentMode == displayMode.ShowSpawn) && (mycfg.game_type != "Epoch")))
            {
                e.Cancel = true;
            }
            else
            {
                if (dataGridViewVehicleTypes.SelectedCells.Count == 1)
                {
                    var row = dataGridViewVehicleTypes.Rows[dataGridViewVehicleTypes.SelectedCells[0].RowIndex];
                    string sType = "";
                    if (currentMode == displayMode.ShowSpawn)
                        sType = "Spawnpoint";

                    toolStripMenuItemAddVehicle.Text = "Add " + row.Cells["ColGVVTType"].Value + " '" + row.Cells["ColGVVTClassName"].Value + "' " + sType;
                    toolStripMenuItemAddVehicle.Enabled = true;
                }
                else
                {
                    toolStripMenuItemAddVehicle.Text = "Select a vehicle from tab 'Vehicles'";
                    toolStripMenuItemAddVehicle.Enabled = false;
                }
            }
        }

        private void trackBarLastUpdated_ValueChanged(object sender, EventArgs e)
        {
            var track = sender as TrackBar;

            labelLastUpdate.Text = (track.Value == track.Maximum) ? "-" : track.Value.ToString();

            mycfg.filter_last_updated = (track.Value == track.Maximum) ? 999 : track.Value;
            myDB.FilterLastUpdated = mycfg.filter_last_updated;
        }

        private void toolStripStatusMapHelper_Click(object sender, EventArgs e)
        {
            currentMode = displayMode.MapHelper;
        }

        private void toolStripStatusDeployable_Click(object sender, EventArgs e)
        {
            currentMode = displayMode.ShowDeployable;
        }

        private void toolStripStatusSpawn_Click(object sender, EventArgs e)
        {
            currentMode = displayMode.ShowSpawn;
        }

        private void toolStripStatusVehicle_Click(object sender, EventArgs e)
        {
            currentMode = displayMode.ShowVehicle;
        }

        private void toolStripStatusAlive_Click(object sender, EventArgs e)
        {
            currentMode = displayMode.ShowAlive;
        }

        private void toolStripStatusOnline_Click(object sender, EventArgs e)
        {
            currentMode = displayMode.ShowOnline;
        }

        private void toolStripStatusWorld_Click(object sender, EventArgs e)
        {
            currentMode = displayMode.SetMaps;
        }

        private void toolStripStatusTrail_Click(object sender, EventArgs e)
        {
            bShowTrails = !bShowTrails;

            if (!bShowTrails)
            {
                foreach (KeyValuePair<string, UIDGraph> pair in dicUIDGraph)
                    pair.Value.ResetPaths();
            }

            toolStripStatusTrail.BorderSides = (bShowTrails) ? ToolStripStatusLabelBorderSides.All : ToolStripStatusLabelBorderSides.None;
        }

        enum displayMode
	    {
            InTheVoid = 0,
            SetMaps,
	        ShowOnline,
            ShowAlive,
            ShowVehicle,
            ShowSpawn,
            ShowDeployable,
            MapHelper,
	    }

        private void toolStripStatusHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.Text + "\n\ndevelopped by Orcalito / 2013\nhttps://github.com/orcalito/DayZ_DB_Access", "Info");
        }
         //
        //  Form
        //
        private void Form1Resize(object sender, EventArgs e)
        {
            ApplyMapChanges();
        }
        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!virtualMap.Enabled)
                return;

            mapZoom.Start(virtualMap,
                          (Tool.Point)(e.Location - virtualMap.Position),
                          Math.Sign(e.Delta));
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseConnection();

            //
            SaveConfigFile();
        }
        //
        //  Panel
        //
        private void _Panel1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (virtualMap.Enabled)
                {
                    e.Graphics.CompositingMode = CompositingMode.SourceCopy;
                    e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                    e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

                    int nb_tilesDrawn = 0;
                    foreach (tileReq req in tileRequests)
                    {
                        tileNfo nfo = tileCache.Find(x => req.path == x.path);

                        if (nfo != null)
                        {
                            e.Graphics.DrawImage(nfo.bitmap, req.rec);
                            nb_tilesDrawn++;
                        }
                    }

                    e.Graphics.CompositingMode = CompositingMode.SourceOver;

                    if (!IsMapHelperEnabled)
                    {
                        if (bShowTrails == true)
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            foreach (iconDB idb in listIcons)
                            {
                                if ((idb.type == UIDType.TypePlayer) || (idb.type == UIDType.TypeVehicle))
                                    GetUIDGraph(idb.uid).DisplayInMap(e.Graphics, virtualMap);
                            }
                        }

                        e.Graphics.SmoothingMode = SmoothingMode.None;
                        Rectangle recPanel = new Rectangle(Point.Empty, splitContainer1.Panel1.Size);
                        int nb_iconsDrawn = 0;
                        foreach (iconDB idb in listIcons)
                        {
                            if (recPanel.IntersectsWith(idb.icon.rectangle))
                            {
                                e.Graphics.DrawImage(idb.icon.image, idb.icon.rectangle);
                                nb_iconsDrawn++;
                            }
                        }
                    }
                    else
                    {
                        mapHelper.Display(e.Graphics);
                    }

                    if (!IsMapHelperEnabled && bCartographer)
                        cartographer.DisplayInMap(e.Graphics, virtualMap);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }
        }
        private void _Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            System.Threading.Interlocked.CompareExchange(ref bUserAction, 1, 0);

            if (IsMapHelperEnabled)
                return;

            if (bCartographer)
            {
                if (e.Button.HasFlag(MouseButtons.Left))
                {
                    Tool.Point mp = virtualMap.UnitToDB(virtualMap.PanelToUnit(e.Location));
                    mp = mp / virtualMap.nfo.dbRefMapSize;
                    mp.Y = 1.0f - mp.Y;
                    cartographer.AddPoint(mp);
                }
                else if (e.Button.HasFlag(MouseButtons.Right))
                {
                    cartographer.RemoveLastPoint();
                }
                else if (e.Button.HasFlag(MouseButtons.Middle))
                {
                    cartographer.AddPoint(new Tool.Point(0, 1));
                }
            }
            else
            {
                selectedIcon = null;
                Rectangle mouseRec = new Rectangle(e.Location, Size.Empty);
                foreach (iconDB idb in listIcons)
                {
                    if (mouseRec.IntersectsWith(idb.icon.rectangle))
                    {
                        // Call Click event from icon
                        selectedIcon = idb;
                        idb.icon.OnClick(this, e);
                        break;
                    }
                }
                if ((selectedIcon == null) && e.Button.HasFlag(MouseButtons.Right))
                {
                    contextMenuStripAddVehicle.Show(this, e.Location);
                }
            }
        }
        private void _Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            System.Threading.Interlocked.CompareExchange(ref bUserAction, 1, 0);

            if (e.Button.HasFlag(MouseButtons.Right) && IsMapHelperEnabled)
            {
                Tool.Point mousePos = (Tool.Point)(e.Location - virtualMap.Position);

                mapHelper.isDraggingCtrlPoint = mapHelper.IntersectControl(mousePos, 5);

                if (mapHelper.isDraggingCtrlPoint > 0)
                {
                    // Will drag selected Control point
                    Tool.Point pt = mapHelper.controls[mapHelper.isDraggingCtrlPoint] * virtualMap.SizeCorrected + virtualMap.Position;
                    mapPan.Start(pt);
                }
                else
                {
                    // Will drag all Control points
                    List<Tool.Point> points = new List<Tool.Point>(4);
                    for (int i = 0; i < 4; i++)
                    {
                        Tool.Point pt = mapHelper.controls[i] * virtualMap.SizeCorrected + virtualMap.Position;
                        points.Add(pt);
                    }
                    mapPan.Start(points);
                }

                splitContainer1.Panel1.Invalidate();
            }
            else
            {
                mapPan.Start(virtualMap.Position);
            }
        }
        private void _Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle recPanel = new Rectangle(Point.Empty, splitContainer1.Panel1.Size);
            Rectangle recMouse = new Rectangle(e.Location, Size.Empty);

            if (!recPanel.IntersectsWith(recMouse))
                return;

            if (e.Button.HasFlag(MouseButtons.Right) && (mapHelper != null))
            {
                if (mapHelper.enabled)
                {
                    mapPan.Update();

                    if (mapHelper.isDraggingCtrlPoint >= 0)
                    {
                        Tool.Point newPos = mapPan.Position(0);
                        Tool.Point pt = (Tool.Point)((newPos - virtualMap.Position) / virtualMap.SizeCorrected);

                        mapHelper.controls[mapHelper.isDraggingCtrlPoint] = pt;
                        mapHelper.ControlPointUpdated(mapHelper.isDraggingCtrlPoint);
                    }
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Tool.Point newPos = mapPan.Position(i);
                            Tool.Point pt = (Tool.Point)((newPos - virtualMap.Position) / virtualMap.SizeCorrected);

                            mapHelper.controls[i] = pt;
                        }
                        mapHelper.ControlPointUpdated(0);
                    }

                    ApplyMapChanges();
                }
            }
            else if (e.Button.HasFlag(MouseButtons.Left))
            {
                mapPan.Update();
                virtualMap.Position = mapPan.Position(0);
                ApplyMapChanges();
            }
            else
            {
                if (mapHelper != null)
                {
                    Tool.Point mousePos = (Tool.Point)(e.Location - virtualMap.Position);
                    mapHelper.isDraggingCtrlPoint = mapHelper.IntersectControl(mousePos, 5);
                    splitContainer1.Panel1.Invalidate();
                }

                mapPan.Stop();
            }

            if (!IsMapHelperEnabled && bCartographer)
            {
                string pathstr = "public static Point[] ptsXXX = new Point[]\r\n{";
                foreach (PathDef def in cartographer.paths)
                {
                    foreach (Tool.Point pt in def.points)
                    {
                        Tool.Point npt = pt;
                        npt.Y = 1.0f - npt.Y;
                        npt = npt * virtualMap.nfo.dbRefMapSize;
                        pathstr += "\r\nnew Point" + npt.ToStringInt() + ",";
                    }
                }
                pathstr = pathstr.TrimEnd(',');
                pathstr += "\r\n};";
                textBoxCmdStatus.Text = pathstr;
            }

            // Database coordinates
            positionInDB = virtualMap.UnitToDB(virtualMap.PanelToUnit(e.Location));
            // Map coordinates
            Tool.Point mp = virtualMap.UnitToMap(virtualMap.PanelToUnit(e.Location));

            if ((mp.X > -100000) && (mp.X < 100000))
            {
                toolStripStatusCoordDB.Text = ((int)positionInDB.X).ToString() + " : " + ((int)positionInDB.Y).ToString();
                toolStripStatusCoordMap.Text = ((int)mp.X).ToString() + " : " + ((int)mp.Y).ToString();
            }
        }
        private void _Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mapHelper != null)
                mapHelper.isDraggingCtrlPoint = -1;

            System.Threading.Interlocked.CompareExchange(ref bUserAction, 0, 1);
        }
        //
        //  myIcon
        //
        private void OnPlayerClick(object sender, EventArgs e)
        {
            myIcon pb = sender as myIcon;

            Survivor player = new Survivor(pb.iconDB);
            player.Rebuild();
            propertyGrid1.SelectedObject = player;
            propertyGrid1.ExpandAllGridItems();
        }
        private void OnVehicleClick(object sender, EventArgs e)
        {
            myIcon pb = sender as myIcon;

            if (pb.iconDB.type == UIDType.TypeVehicle)
            {
                Vehicle vehicle = new Vehicle(pb.iconDB);
                vehicle.Rebuild();
                propertyGrid1.SelectedObject = vehicle;
                propertyGrid1.ExpandAllGridItems();
            }
            else
            {
                Spawn spawn = new Spawn(pb.iconDB);
                spawn.Rebuild();
                propertyGrid1.SelectedObject = spawn;
                propertyGrid1.ExpandAllGridItems();
            }

            // Select class name in Vehicles table
            foreach (DataGridViewRow row in dataGridViewVehicleTypes.Rows)
            {
                if (row.Cells["ColGVVTClassName"].Value as string == pb.iconDB.row.Field<string>("class_name"))
                    row.Cells["ColGVVTType"].Selected = true;
            }
        }
        private void OnDeployableClick(object sender, EventArgs e)
        {
            myIcon pb = sender as myIcon;

            Deployable deployable = new Deployable(pb.iconDB);
            deployable.Rebuild();
            propertyGrid1.SelectedObject = deployable;
            propertyGrid1.ExpandAllGridItems();

            // Select class name in Deployables table
            foreach (DataGridViewRow row in dataGridViewDeployableTypes.Rows)
            {
                if (row.Cells["ColGVDTClassName"].Value as string == pb.iconDB.row.Field<string>("class_name"))
                    row.Cells["ColGVDTType"].Selected = true;
            }
        }
        //
        //  Contextual menu
        //
        private void repairRefuelVehicleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedIcon != null)
            {
                int res = myDB.ExecuteSqlNonQuery("UPDATE instance_vehicle SET parts='[]',fuel='1',damage='0' WHERE (id=" + selectedIcon.uid + ")");
                if (res == 1)
                    textBoxCmdStatus.Text = "repaired & refueled vehicle id " + selectedIcon.uid;
            }
            selectedIcon = null;
        }
        private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            if (selectedIcon != null)
            {
                int res = myDB.ExecuteSqlNonQuery("DELETE FROM instance_vehicle WHERE id=" + selectedIcon.uid + " AND instance_id=" + mycfg.instance_id);
                if (res == 1)
                    textBoxCmdStatus.Text = "removed vehicle id " + selectedIcon.uid;
            }
            selectedIcon = null;
        }
        private void toolStripMenuItemDeleteSpawn_Click(object sender, EventArgs e)
        {
            if (selectedIcon != null)
            {
                int res = myDB.ExecuteSqlNonQuery("DELETE FROM world_vehicle WHERE id=" + selectedIcon.uid + " AND world_id=" + mycfg.world_id);
                if (res == 1)
                    textBoxCmdStatus.Text = "removed vehicle spawnpoint id " + selectedIcon.uid;
            }
            selectedIcon = null;
        }
        //
        //  Database's Tab
        //
        private void _buttonConnect_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            myDB.Connect(textBoxURL.Text,
                         int.Parse(textBoxPort.Text),
                         textBoxBaseName.Text,
                         textBoxUser.Text,
                         textBoxPassword.Text,
                         int.Parse(numericUpDownInstanceId.Text),
                         mycfg);

            if(myDB.Connected)
            {
                Enable(true);

                mycfg.instance_id = myDB.InstanceId.ToString();
                mycfg.world_id = (UInt16)myDB.WorldId;

                this.textBoxCmdStatus.Text = "";
                this.toolStripStatusWorld.Text = myDB.WorldName;

                SetCurrentMap();

                groupBoxCnx.Enabled = false;
                groupBoxCnx.Visible = false;
            }

            this.Cursor = Cursors.Arrow;
        }
        private void _comboBoxGameType_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            mycfg.game_type = cb.Items[cb.SelectedIndex] as string;
            numericUpDownInstanceId.Enabled = (cb.SelectedIndex == 0);
        }
        private void _cbCartographer_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == true)
                cartographer.paths.Clear();
        }
        private void _MapHelperStateChanged()
        {
            if (mapHelper == null)
                return;

            if (!mapHelper.enabled)
            {
                // Apply map helper's new size
                DataRow row = mycfg.worlds_def.Tables[0].Rows.Find(mycfg.world_id);

                Tool.Size refSize = new Tool.Size(row.Field<UInt32>("DB_refWidth"),
                                                  row.Field<UInt32>("DB_refHeight"));

                Tool.Point offUnit = mapHelper.boundaries[0];
                Tool.Size sizeUnit = mapHelper.boundaries[1] - mapHelper.boundaries[0];
                Tool.Point offset = offUnit * refSize;
                Tool.Size size = sizeUnit * refSize;

                virtualMap.nfo.dbMapOffsetUnit = offset / refSize;
                virtualMap.nfo.dbMapSize = size;
                virtualMap.nfo.dbRefMapSize = refSize;

                row.SetField<int>("DB_X", (int)offset.X);
                row.SetField<int>("DB_Y", (int)offset.Y);
                row.SetField<UInt32>("DB_Width", (UInt32)size.Width);
                row.SetField<UInt32>("DB_Height", (UInt32)size.Height);

                splitContainer1.Panel1.Invalidate();
            }
        }
        //
        //  Data grids
        //
        private void _dataGridViewMaps_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewMaps.Rows.Count))
                return;

            // Ignore clicks that are not on button cells.  
            if (e.ColumnIndex == dataGridViewMaps.Columns["ColGVMChoosePath"].Index)
            {
                openFileDialog1.FileName = "";
                openFileDialog1.CheckFileExists = true;
                openFileDialog1.CheckPathExists = true;
                openFileDialog1.Multiselect = false;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    dataGridViewMaps["ColGVMPath", e.RowIndex].Value = openFileDialog1.FileName;

                    try
                    {
                        string filepath = openFileDialog1.FileName;
                        int world_id = int.Parse(dataGridViewMaps["ColGVMID", e.RowIndex].Value.ToString());

                        if (File.Exists(filepath))
                        {
                            FileInfo fi = new FileInfo(filepath);

                            string tileBasePath = configPath + "\\World" + world_id + "\\LOD";

                            MessageBox.Show("Please wait while generating tiles...\r\nThis is done once when selecting a new map.");

                            tileCache.Clear();

                            this.Cursor = Cursors.WaitCursor;

                            DirectoryInfo di = new DirectoryInfo(configPath + "\\World" + world_id);
                            if (di.Exists)
                                di.Delete(true);

                            Tuple<Tool.Size, Tool.Size, Tool.Size> sizes = Tool.CreateTiles(filepath, tileBasePath, 256);

                            DataRow rowW = mycfg.worlds_def.Tables[0].Rows.Find(world_id);
                            rowW.SetField<float>("RatioX", sizes.Item1.Width / sizes.Item2.Width);
                            rowW.SetField<float>("RatioY", sizes.Item1.Height / sizes.Item2.Height);
                            rowW.SetField<int>("TileSizeX", (int)sizes.Item3.Width);
                            rowW.SetField<int>("TileSizeY", (int)sizes.Item3.Height);

                            int tileCount = (int)(sizes.Item2.Width / 256);
                            int depth = (int)Math.Log(tileCount, 2) + 1;

                            rowW.SetField<int>("TileDepth", depth);

                            this.Cursor = Cursors.Arrow;
                            MessageBox.Show("Tiles generation done.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error while generating tiles !\r\nMaybe the bitmap is too large to be processed...");
                        textBoxCmdStatus.Text += ex.ToString();
                        this.Cursor = Cursors.Arrow;
                    }

                    if (dataGridViewMaps["ColGVMID", e.RowIndex].Value.ToString() == mycfg.world_id.ToString())
                        SetCurrentMap();
                }
            }
        }
        private void _dataGridViewVehicleTypes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewVehicleTypes.Rows.Count))
                return;

            // Ignore clicks that are not on checkbox cells.  
            if (e.ColumnIndex == dataGridViewVehicleTypes.Columns["ColGVVTShow"].Index)
            {
                dataGridViewVehicleTypes.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void _dataGridViewVehicleTypes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewVehicleTypes.Rows.Count))
                return;

            bool bState = (bool)dataGridViewVehicleTypes["ColGVVTShow", e.RowIndex].Value;

            DataRow row = mycfg.vehicle_types.Tables[0].Rows.Find(dataGridViewVehicleTypes["ColGVVTClassName", e.RowIndex].Value);

            row.SetField<bool>("Show", bState);
        }
        private static bool GVVT_bCurrentState = true;
        private void _dataGridViewVehicleTypes_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == dataGridViewVehicleTypes.Columns["ColGVVTShow"].Index)
            {
                GVVT_bCurrentState = !GVVT_bCurrentState;

                foreach (DataRow row in mycfg.vehicle_types.Tables[0].Rows)
                    row.SetField<bool>("Show", GVVT_bCurrentState);
            }
        }
        private void _dataGridViewDeployableTypes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewDeployableTypes.Rows.Count))
                return;

            // Ignore clicks that are not on checkbox cells.  
            if (e.ColumnIndex == dataGridViewDeployableTypes.Columns["ColGVDTShow"].Index)
            {
                dataGridViewDeployableTypes.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void _dataGridViewDeployableTypes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewDeployableTypes.Rows.Count))
                return;

            bool bState = (bool)dataGridViewDeployableTypes["ColGVDTShow", e.RowIndex].Value;

            DataRow row = mycfg.deployable_types.Tables[0].Rows.Find(dataGridViewDeployableTypes["ColGVDTClassName", e.RowIndex].Value);

            row.SetField<bool>("Show", bState);
        }
        private static bool GVDT_bCurrentState = true;
        private void _dataGridViewDeployableTypes_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == dataGridViewDeployableTypes.Columns["ColGVDTShow"].Index)
            {
                GVDT_bCurrentState = !GVDT_bCurrentState;

                foreach (DataRow row in mycfg.deployable_types.Tables[0].Rows)
                    row.SetField<bool>("Show", GVDT_bCurrentState);
            }
        }
        //
        // Scripts
        //
        private void _buttonBackup_Click(object sender, EventArgs e)
        {
            string s_date = DateTime.Now.Year + "-"
                          + DateTime.Now.Month.ToString("00") + "-"
                          + DateTime.Now.Day.ToString("00") + " "
                          + DateTime.Now.Hour.ToString("00") + "h"
                          + DateTime.Now.Minute.ToString("00");
            saveFileDialog1.FileName = "Backup " + textBoxBaseName.Text + " " + s_date + ".sql";
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "SQL file|*.sql";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;

                string result = myDB.BackupToFile(saveFileDialog1.FileName);

                textBoxCmdStatus.Text = result;

                this.Cursor = Cursors.Arrow;
            }
        }
        private void _buttonRemoveDestroyed_Click(object sender, EventArgs e)
        {
            int res = myDB.ExecuteSqlNonQuery("DELETE FROM instance_vehicle WHERE instance_id=" + mycfg.instance_id + " AND damage=1");

            textBoxCmdStatus.Text = "removed " + res + " destroyed vehicles.";
        }
        private void _buttonSpawnNew_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string sResult = myDB.SpawnNewVehicles(int.Parse(textBoxVehicleMax.Text));

            textBoxCmdStatus.Text = sResult;
            
            this.Cursor = Cursors.Arrow;
        }
        private void _buttonRemoveBodies_Click(object sender, EventArgs e)
        {
            int limit = int.Parse(textBoxOldBodyLimit.Text);

            string query = "DELETE FROM survivor WHERE world_id=" + mycfg.world_id + " AND is_dead=1 AND last_updated < now() - interval " + limit + " day";
            int res = myDB.ExecuteSqlNonQuery(query);

            textBoxCmdStatus.Text = "removed " + res + " bodies older than " + limit + " days.";
        }
        private void _buttonRemoveTents_Click(object sender, EventArgs e)
        {
            int limit = int.Parse(textBoxOldTentLimit.Text);

            string query = "DELETE FROM id using instance_deployable id inner join deployable d on id.deployable_id = d.id"
                         + " inner join survivor s on id.owner_id = s.id and s.is_dead=1"
                         + " WHERE id.instance_id=" + mycfg.instance_id + " AND d.class_name = 'TentStorage' AND id.last_updated < now() - interval " + limit + " day";
            int res = myDB.ExecuteSqlNonQuery(query);
        }
        //
        //  Custom scripts
        //
        private void _buttonSelectCustom_Click(object sender, EventArgs e)
        {
            Button btSel = sender as Button;

            openFileDialog1.FileName = "";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "SQL & BAT files|*.sql;*.bat|SQL files|*.sql|Batch files|*.bat";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Button btExe = null;
                switch (btSel.Name)
                {
                    case "buttonSelectCustom1": btExe = this.buttonCustom1; mycfg.customscript1 = openFileDialog1.FileName; break;
                    case "buttonSelectCustom2": btExe = this.buttonCustom2; mycfg.customscript2 = openFileDialog1.FileName; break;
                    case "buttonSelectCustom3": btExe = this.buttonCustom3; mycfg.customscript3 = openFileDialog1.FileName; break;
                }
                if (btExe != null)
                {
                    FileInfo fi = new FileInfo(openFileDialog1.FileName);
                    btExe.Text = fi.Name;
                    btExe.Enabled = true;
                }
            }
        }
        private void _buttonCustom_Click(object sender, EventArgs e)
        {
            Button bt = sender as Button;

            string fullpath = null;
            switch (bt.Name)
            {
                case "buttonCustom1": fullpath = mycfg.customscript1; break;
                case "buttonCustom2": fullpath = mycfg.customscript2; break;
                case "buttonCustom3": fullpath = mycfg.customscript3; break;
            }

            if (Tool.NullOrEmpty(fullpath))
                return;

            this.Cursor = Cursors.WaitCursor;
            try
            {
                FileInfo fi = new FileInfo(fullpath);

                switch (fi.Extension.ToLowerInvariant())
                {
                    case ".sql":
                        StreamReader sr = fi.OpenText();
                        string queries = sr.ReadToEnd();

                        string result = myDB.ExecuteScript(queries);

                        this.textBoxCmdStatus.Text += result;

                        sr.Close();
                        break;

                    case ".bat":
                        System.Diagnostics.Process proc = System.Diagnostics.Process.Start(fullpath);
                        proc.WaitForExit();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }
            this.Cursor = Cursors.Arrow;
        }
        //
        //  Background Workers
        //
        private void bgWorkerRefreshDatabase_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            while (!bw.CancellationPending)
            {
                Thread.Sleep(5000);

                myDB.Refresh();

                if (System.Threading.Interlocked.CompareExchange(ref bUserAction, 1, 0) == 0)
                {
                    dlgUpdateIcons = this.BuildIcons;

                    if (myDB.Connected)
                        this.Invoke(dlgUpdateIcons);

                    System.Threading.Interlocked.Exchange(ref bUserAction, 0);
                }
            }
        }
        private void bgWorkerFast_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            while (!bw.CancellationPending)
            {
                eventFastBgWorker.WaitOne();

                if (virtualMap.Enabled)
                {
                    if (System.Threading.Interlocked.CompareExchange(ref bUserAction, 1, 0) == 0)
                    {
                        while (mapZoom.Update(virtualMap))
                        {
                            dlgRefreshMap = this.ApplyMapChanges;
                            this.Invoke(dlgRefreshMap);

                            Thread.Sleep(20);
                        }

                        dlgRefreshMap = this.ApplyMapChanges;
                        this.Invoke(dlgRefreshMap);
                    }

                    System.Threading.Interlocked.CompareExchange(ref bUserAction, 0, 1);
                }
            }
        }
        //
        static int penCount = 0;
        static Pen[] pens = new Pen[6]
        {
            new Pen(Color.Red, 2),
            new Pen(Color.Green, 2),
            new Pen(Color.Blue, 2),
            new Pen(Color.Yellow, 2),
            new Pen(Color.Orange, 2),
            new Pen(Color.Violet, 2)
        };

        private void toolStripMenuItemResetTypes_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var menu = item.Owner as ContextMenuStrip;

            switch (menu.SourceControl.Name)
            {
                case "dataGridViewVehicleTypes":
                    mycfg.vehicle_types.Tables[0].Rows.Clear();
                    break;
                case "dataGridViewDeployableTypes":
                    mycfg.deployable_types.Tables[0].Rows.Clear();
                    break;
            }

            myDB.OnConnection();
        }

        private iconDB selectedIcon;
    }
}