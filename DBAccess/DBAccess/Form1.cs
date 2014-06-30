using BattleNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DBAccess
{
    public partial class MainWindow : Form
    {
        #region Fields
        public static bool IsDebug { get; private set; }
        static ModuleVersion curCfgVersion = new ModuleVersion(5, 1);

        private static int bUserAction = 0;
        //
        private myConfig mycfg = new myConfig(curCfgVersion);
        private string configPath;
        private string configFilePath;
        private VirtualMap virtualMap = new VirtualMap();
        //
        private myDatabase myDB = new myDatabase();
        private string localIP = "";
        //
        internal BattlEyeClient rCon = null;
        private DataSet PlayerNamesOnline = new DataSet("Players Online DS");
        private DataSet PlayersOnline = new DataSet("Players Online DS");
        private DataSet AdminsOnline = new DataSet("Admins Online DS");
        //
        private AboutBox diagAbout = new AboutBox();
        private MessageToPlayer diagMsgToPlayer = null;
        //
        private Dictionary<string, UIDGraph> dicUIDGraph = new Dictionary<string, UIDGraph>();
        private List<iconDB> listIcons = new List<iconDB>();
        private List<iconDB> iconsDB = new List<iconDB>();
        private List<myIcon> iconPlayers = new List<myIcon>();
        private List<myIcon> iconVehicles = new List<myIcon>();
        private List<myIcon> iconDeployables = new List<myIcon>();
        private MapPan mapPan = new MapPan();
        private MapZoom mapZoom = new MapZoom(eventMapZoomBgWorker);
        private MapHelper mapHelper;
        private UIDGraph cartographer = new UIDGraph(new Pen(Color.Black, 2));
        private Tool.Point positionInDB = new Tool.Point();
        //
        private List<tileReq> tileRequests = new List<tileReq>();
        private List<tileNfo> tileCache = new List<tileNfo>();
        private bool bShowTrails = false;
        private bool bCartographer = false;
        
        private bool IsMapHelperEnabled { get { return (mapHelper != null) && mapHelper.enabled; } }

        private ToolIcon toolIconTrails = null;
        private ToolIcon toolIconChat = null;
        private Dictionary<displayMode, ToolIcon> ModeButtons = new Dictionary<displayMode, ToolIcon>();
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
                        case displayMode.ShowDead:
                        case displayMode.ShowVehicle:
                        case displayMode.ShowSpawn:
                        case displayMode.ShowTraders:
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
                        item.Value.Select = false;
                    }

                    if (_currentMode != displayMode.InTheVoid)
                    {
                        ModeButtons[_currentMode].Select = true;
                    }

                    dataGridViewMaps.Visible = (value == displayMode.SetMaps);
                }
            }
        }
        private iconDB selectedIcon = null;
        private iconDB hoverIcon = null;
        private iconDB prevHoverIcon = null;

        private System.Drawing.Imaging.ImageAttributes attrSelected = new System.Drawing.Imaging.ImageAttributes();
        private System.Drawing.Imaging.ImageAttributes attrUnselected = new System.Drawing.Imaging.ImageAttributes();
        private Dictionary<string, System.Drawing.Imaging.ImageAttributes> attrColorPlayers = new Dictionary<string, System.Drawing.Imaging.ImageAttributes>();
        internal InterpolationMode gfxIntplMode = InterpolationMode.NearestNeighbor;

        #endregion

        public bool IsEpochSchema { get { return myDB.Schema.Type == "Epoch"; } }
        public MainWindow()
        {
            InitializeComponent();

            //--- Fill ColGVVTType & ColGVDTType with compatible icon types ---
            foreach (IconType v in Enum.GetValues(typeof(IconType)))
            {
                FieldInfo fi = v.GetType().GetField(v.ToString());
                var attributes = (UsableInAttribute[])fi.GetCustomAttributes(typeof(UsableInAttribute), false);
                if (attributes.Length > 0)
                {
                    foreach (var attr in attributes)
                    {
                        switch (attr.target)
                        {
                            case "Vehicle": this.ColGVVTType.Items.Add(v.ToString()); break;
                            case "Deployable": this.ColGVDTType.Items.Add(v.ToString()); break;
                        }
                    }
                }
            }

            //
            splitContainerGlobal.Panel2Collapsed = true;

            //
            diagMsgToPlayer = new MessageToPlayer(this);

            //
            ModeButtons.Add(displayMode.SetMaps, new ToolIcon(toolStripStatusWorld, global::DBAccess.Properties.Resources.Tool_World, global::DBAccess.Properties.Resources.Tool_World_S));
            ModeButtons.Add(displayMode.ShowOnline, new ToolIcon(toolStripStatusOnline, global::DBAccess.Properties.Resources.Tool_Online, global::DBAccess.Properties.Resources.Tool_Online_S));
            ModeButtons.Add(displayMode.ShowAlive, new ToolIcon(toolStripStatusAlive, global::DBAccess.Properties.Resources.Tool_Alive, global::DBAccess.Properties.Resources.Tool_Alive_S));
            ModeButtons.Add(displayMode.ShowDead, new ToolIcon(toolStripStatusDead, global::DBAccess.Properties.Resources.Tool_Dead, global::DBAccess.Properties.Resources.Tool_Dead_S));
            ModeButtons.Add(displayMode.ShowVehicle, new ToolIcon(toolStripStatusVehicle, global::DBAccess.Properties.Resources.Tool_Vehicle, global::DBAccess.Properties.Resources.Tool_Vehicle_S));
            ModeButtons.Add(displayMode.ShowSpawn, new ToolIcon(toolStripStatusSpawn, global::DBAccess.Properties.Resources.Tool_Spawn, global::DBAccess.Properties.Resources.Tool_Spawn_S));
            ModeButtons.Add(displayMode.ShowTraders, new ToolIcon(toolStripStatusTraders, global::DBAccess.Properties.Resources.Tool_Traders, global::DBAccess.Properties.Resources.Tool_Traders));
            ModeButtons.Add(displayMode.ShowDeployable, new ToolIcon(toolStripStatusDeployable, global::DBAccess.Properties.Resources.Tool_Deployable, global::DBAccess.Properties.Resources.Tool_Deployable_S));
            ModeButtons.Add(displayMode.MapHelper, new ToolIcon(toolStripStatusMapHelper, global::DBAccess.Properties.Resources.Tool_MapHelper, global::DBAccess.Properties.Resources.Tool_MapHelper_S));

            //
            toolIconTrails = new ToolIcon(toolStripStatusTrail, global::DBAccess.Properties.Resources.Tool_Trail, global::DBAccess.Properties.Resources.Tool_Trail_S);
            toolIconChat = new ToolIcon(toolStripStatusChat, global::DBAccess.Properties.Resources.Tool_Chat, global::DBAccess.Properties.Resources.Tool_Chat_S);

            //
            float[][] colorMatrixUnselected =
            { 
                new float[] {1,  0,  0,  0,  0},
                new float[] {0,  1,  0,  0,  0},
                new float[] {0,  0,  1,  0,  0},
                new float[] {0,  0,  0,  1,  0},
                new float[] {0,  0,  0,  0,  1}  
            };
            attrUnselected.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(colorMatrixUnselected));
            //
            float[][] colorMatrixSelected =
            { 
                new float[] {1,  0,  0,  0,  0},
                new float[] {0,  1,  0,  0,  0},
                new float[] {0,  0,  1,  0,  0},
                new float[] {0,  0,  0,  1,  0},
                new float[] {-0.2f,  0.5f,  -0.2f,  0,  1}  
            };
            attrSelected.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(colorMatrixSelected));
                        

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
                    cbCartographer.Enabled = true;
                    IsDebug = true;
                }
            }
            catch
            {
                this.Text = "DayZ DB Access unknown version";
            }

            this.MouseWheel += cb_Form1_MouseWheel;

            //
            LoadConfigFile();

            //
            myDB.ReconnectDelay = mycfg.db_refreshrate;

            //
            DataTable tablePn = PlayerNamesOnline.Tables.Add();
            tablePn.Columns.Add(new DataColumn("Name", typeof(string)));
            tablePn.Columns.Add(new DataColumn("Status", typeof(string)));
            DataColumn[] keysPn = new DataColumn[1];
            keysPn[0] = tablePn.Columns[0];   // Search by Name only
            tablePn.PrimaryKey = keysPn;

            //
            DataTable tableP = PlayersOnline.Tables.Add();
            tableP.Columns.Add(new DataColumn("Id", typeof(int)));
            tableP.Columns.Add(new DataColumn("Name", typeof(string)));
            tableP.Columns.Add(new DataColumn("GUID", typeof(string)));
            tableP.Columns.Add(new DataColumn("IP", typeof(string)));
            tableP.Columns.Add(new DataColumn("Status", typeof(string)));
            DataColumn[] keysP = new DataColumn[1];
            keysP[0] = tableP.Columns[0];   // Search by ID only
            tableP.PrimaryKey = keysP;
            dataGridViewPlayers.DataSource = PlayersOnline.Tables[0];

            //
            DataTable tableA = AdminsOnline.Tables.Add();
            tableA.Columns.Add(new DataColumn("Id", typeof(int)));
            tableA.Columns.Add(new DataColumn("IP", typeof(string)));
            tableA.Columns.Add(new DataColumn("Port", typeof(int)));
            DataColumn[] keysA = new DataColumn[1];
            keysA[0] = tableA.Columns[0];   // Search by ID only
            tableA.PrimaryKey = keysA;
            dataGridViewAdmins.DataSource = AdminsOnline.Tables[0];

            //
            localIP = "";
            RetrieveExternalLocalIP();

            bgWorkerDatabase.RunWorkerAsync();
            bgWorkerBattlEye.RunWorkerAsync();
            bgWorkerMapZoom.RunWorkerAsync();
            bgWorkerLoadTiles.RunWorkerAsync();
            bgWorkerFocus.RunWorkerAsync();
            bgWorkerRefreshLeds.RunWorkerAsync();

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
            if (rCon != null)
                rCon.Disconnect();

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
                    toolStripStatusOnline.Text = (PlayersOnline.Tables.Count > 0) ? PlayersOnline.Tables[0].Rows.Count.ToString() : "-";
                    toolStripStatusAlive.Text = (myDB.PlayersAlive.Tables.Count > 0) ? myDB.PlayersAlive.Tables[0].Rows.Count.ToString() : "-";
                    toolStripStatusDead.Text = (myDB.PlayersDead.Tables.Count > 0) ? myDB.PlayersDead.Tables[0].Rows.Count.ToString() : "-";
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
                        case displayMode.ShowDead: BuildDeadIcons(); break;
                        case displayMode.ShowVehicle: BuildVehicleIcons(); break;
                        case displayMode.ShowSpawn: BuildSpawnIcons(); break;
                        case displayMode.ShowDeployable: BuildDeployableIcons(); break;
                    }

                    RefreshIcons();
                    prevHoverIcon = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception found");
                }

                myDB.UseDS(false);
            }
        }
        private void RecBuildReqTileList(int curDepth, int expDepth, int x, int y, Rectangle recPanel)
        {
            //  [x, y] are from [0, 0] to [tileCountX, tileCountY]
            int ioff = (1 << (expDepth - curDepth)) / 2;
            int tsize = (128 << (expDepth - curDepth));
            Tool.Size size = new Tool.Size(tsize, tsize);

            if (curDepth < expDepth)
            {
                //  Q00
                if (recPanel.IntersectsWith(virtualMap.TileRectangleEx(new Tool.Point(x, y), size)))
                    RecBuildReqTileList(curDepth + 1, expDepth, x, y, recPanel); 

                //  Q01
                if (recPanel.IntersectsWith(virtualMap.TileRectangleEx(new Tool.Point(x + ioff, y), size)))
                    RecBuildReqTileList(curDepth + 1, expDepth, x + ioff, y, recPanel);

                //  Q10
                if (recPanel.IntersectsWith(virtualMap.TileRectangleEx(new Tool.Point(x, y + ioff), size)))
                    RecBuildReqTileList(curDepth + 1, expDepth, x, y + ioff, recPanel); 

                //  Q11
                if (recPanel.IntersectsWith(virtualMap.TileRectangleEx(new Tool.Point(x + ioff, y + ioff), size)))
                    RecBuildReqTileList(curDepth + 1, expDepth, x + ioff, y + ioff, recPanel);
            }
            else
            {
                bool keepLoaded = (curDepth == virtualMap.nfo.min_depth);
                tileReq req;

                req = new tileReq(x, y, curDepth, keepLoaded, false);
                req.path = virtualMap.nfo.tileBasePath + curDepth + "\\Tile" + y.ToString("000") + x.ToString("000") + ".jpg";
                req.rec = virtualMap.TileRectangle(new Tool.Point(x, y));
                tileRequests.Add(req);

                // Clamp requests to last available level of mipmap
                if (expDepth >= virtualMap.nfo.max_depth)
                {
                    int hpow = 1;
                    while (expDepth > virtualMap.nfo.min_depth)
                    {
                        expDepth--;
                        x /= 2;
                        y /= 2;
                        hpow *= 2;
                        req = new tileReq(x, y, expDepth, false, true);
                        req.path = virtualMap.nfo.tileBasePath + expDepth + "\\Tile" + y.ToString("000") + x.ToString("000") + ".jpg";
                        if (TileFileExists(req))
                        {
                            if (tileRequests.Find(n => req.Key == n.Key) == null)
                                tileRequests.Add(req);
                            break;
                        }
                    }
                }
            }
        }

        private void RefreshIcons()
        {
            myDB.UseDS(true);

            if (virtualMap.Enabled)
            {
                mtxTileRequest.WaitOne();

                Rectangle recPanel = new Rectangle(Point.Empty, splitContainer1.Panel1.Size);
                int tileDepth = virtualMap.Depth;
                Size tileCount = virtualMap.TileCount;
                bool keepLoaded = (tileDepth == virtualMap.nfo.min_depth);

                tileRequests.Clear();
                //  QuadTree-like handling of tile visibility
                RecBuildReqTileList(0, tileDepth, 0, 0, recPanel);

                mtxTileRequest.ReleaseMutex();
            }

            foreach (iconDB idb in listIcons)
                idb.icon.Location = virtualMap.UnitToPanel(idb);

            splitContainer1.Panel1.Invalidate();

            myDB.UseDS(false);
        }
        private void BuildOnlineIcons()
        {
            int idx = 0;
            foreach (DataRow row in myDB.PlayersAlive.Tables[0].Rows)
            {
                // Check the list of online players from rCon
                DataRow rowOnline = PlayerNamesOnline.Tables[0].Rows.Find(row.Field<string>("name"));
                if (rowOnline != null)
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

                    bool isOnline = (rowOnline.Field<string>("Status") == "Ingame");

                    idb.icon = iconPlayers[idx];
                    idb.icon.image = isOnline ? global::DBAccess.Properties.Resources.iconOnline : global::DBAccess.Properties.Resources.iconLobby;
                    idb.icon.Size = idb.icon.image.Size;
                    idb.icon.iconDB = idb;
                    idb.icon.contextMenuStrip = isOnline ? null : contextMenuStripPlayerMenu;

                    //toolTip1.SetToolTip(idb.icon, row.Field<string>("name"));

                    listIcons.Add(idb);

                    if (bShowTrails == true)
                        GetUIDGraph(idb.uid).AddPoint(idb.pos);

                    idx++;
                }
            }
        }
        private void BuildAliveIcons()
        {
            int idx = 0;
            foreach (DataRow row in myDB.PlayersAlive.Tables[0].Rows)
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
                idb.icon.contextMenuStrip = contextMenuStripPlayerMenu;

                //toolTip1.SetToolTip(idb.icon, row.Field<string>("name"));

                listIcons.Add(idb);

                if (bShowTrails == true)
                    GetUIDGraph(idb.uid).AddPoint(idb.pos);

                idx++;
            }
        }
        private void BuildDeadIcons()
        {
            int idx = 0;
            foreach (DataRow row in myDB.PlayersDead.Tables[0].Rows)
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
                idb.icon.image = global::DBAccess.Properties.Resources.iconDead;
                idb.icon.Size = idb.icon.image.Size;
                idb.icon.iconDB = idb;
                idb.icon.contextMenuStrip = contextMenuStripDeadMenu;

                listIcons.Add(idb);

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

                    string classname = (rowT != null) ? rowT.Field<string>("Type") : "";
                    idb.icon.image = GetBitmapFromClass(classname, false, (damage >= 1.0f));
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

        private static Bitmap GetBitmapFromClass(string classname, bool def_is_unknown, bool crashed)
        {
            switch (classname)
            {
                case "Air": return (!crashed) ? global::DBAccess.Properties.Resources.air : global::DBAccess.Properties.Resources.air_crashed;
                case "Atv": return (!crashed) ? global::DBAccess.Properties.Resources.atv : global::DBAccess.Properties.Resources.atv_crashed;
                case "Bicycle": return (!crashed) ? global::DBAccess.Properties.Resources.bike : global::DBAccess.Properties.Resources.bike_crashed;
                case "Boat": return (!crashed) ? global::DBAccess.Properties.Resources.boat : global::DBAccess.Properties.Resources.boat_crashed;
                case "Bus": return (!crashed) ? global::DBAccess.Properties.Resources.bus : global::DBAccess.Properties.Resources.bus_crashed;
                case "Car": return (!crashed) ? global::DBAccess.Properties.Resources.car : global::DBAccess.Properties.Resources.car_crashed;
                case "Helicopter": return (!crashed) ? global::DBAccess.Properties.Resources.helicopter : global::DBAccess.Properties.Resources.helicopter_crashed;
                case "Motorcycle": return (!crashed) ?  global::DBAccess.Properties.Resources.motorcycle : global::DBAccess.Properties.Resources.motorcycle_crashed;
                case "SUV": return (!crashed) ? global::DBAccess.Properties.Resources.suv : global::DBAccess.Properties.Resources.suv_crashed;
                case "Tractor": return (!crashed) ? global::DBAccess.Properties.Resources.tractor : global::DBAccess.Properties.Resources.tractor_crashed;
                case "Truck": return (!crashed) ?  global::DBAccess.Properties.Resources.truck : global::DBAccess.Properties.Resources.truck_crashed;
                case "UAZ": return (!crashed) ?  global::DBAccess.Properties.Resources.uaz : global::DBAccess.Properties.Resources.uaz_crashed;
                case "Tent": return global::DBAccess.Properties.Resources.tent;
                case "Stach": return global::DBAccess.Properties.Resources.stach;
                case "SmallBuild": return global::DBAccess.Properties.Resources.small_build;
                case "LargeBuild": return global::DBAccess.Properties.Resources.large_build;
            }

            //if(def_is_unknown)
                return global::DBAccess.Properties.Resources.unknown;

            //return (!crashed) ? global::DBAccess.Properties.Resources.car : global::DBAccess.Properties.Resources.car_crashed;
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

                    idb.icon.image = GetBitmapFromClass(classname, false, false);
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
                    idb.icon.contextMenuStrip = null;

                    string classname = (rowT != null) ? rowT.Field<string>("Type") : "";
                    idb.icon.image = GetBitmapFromClass(classname, true, false);
                    idb.icon.Size = idb.icon.image.Size;

                    listIcons.Add(idb);

                    idx++;
                }
            }
        }
        private void Enable(bool bState)
        {
            bool bEpochGameType = this.IsEpochSchema;
            
            buttonConnect.Enabled = !bState;

            toolStripStatusWorld.Enabled = bState;
            toolStripStatusOnline.Enabled = bState;
            toolStripStatusAlive.Enabled = bState;
            toolStripStatusDead.Enabled = bState;
            toolStripStatusVehicle.Enabled = bState;
            toolStripStatusSpawn.Enabled = bState;
            toolStripStatusTraders.Enabled = /*bState*/false;
            toolStripStatusDeployable.Enabled = bState;
            toolStripStatusMapHelper.Enabled = bState;
            toolStripStatusTrail.Enabled = bState;
            toolStripStatusChat.Enabled = bState;

            //
            textBoxDBURL.Enabled = !bState;
            textBoxDBBaseName.Enabled = !bState;
            numericUpDownDBPort.Enabled = !bState;
            textBoxDBUser.Enabled = !bState;
            textBoxDBPassword.Enabled = !bState;

            // Script buttons
            buttonBackup.Enabled = bState;
            buttonSelectCustom1.Enabled = bState;
            buttonSelectCustom2.Enabled = bState;
            buttonSelectCustom3.Enabled = bState;
            buttonCustom1.Enabled = (bState) ? !Tool.NullOrEmpty(mycfg.customscript1) : false;
            buttonCustom2.Enabled = (bState) ? !Tool.NullOrEmpty(mycfg.customscript2) : false;
            buttonCustom3.Enabled = (bState) ? !Tool.NullOrEmpty(mycfg.customscript3) : false;

            // Epoch disabled controls...
            if (bEpochGameType)
            {
                toolStripStatusSpawn.Visible = false;
                toolStripStatusTraders.Visible = true;
            }
            else
            {
                numericUpDownInstanceId.Enabled = !bState;
                toolStripStatusSpawn.Visible = true;
                toolStripStatusTraders.Visible = false;
            }
            buttonRemoveDestroyed.Enabled = bState && !bEpochGameType;
            buttonSpawnNew.Enabled = bState && !bEpochGameType;
            buttonRemoveBodies.Enabled = bState;
            buttonRemoveTents.Enabled = bState && !bEpochGameType;

            if (!bState)
            {
                toolStripStatusAlive.Text = "-";
                toolStripStatusDead.Text = "-";
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
            if (Tool.NullOrEmpty(mycfg.rcon_port)) mycfg.rcon_port = "2302";
            if (Tool.NullOrEmpty(mycfg.rcon_url)) mycfg.rcon_url = "";
            if (Tool.NullOrEmpty(mycfg.rcon_password)) mycfg.rcon_password = "";
            if (Tool.NullOrEmpty(mycfg.rcon_adminname)) mycfg.rcon_adminname = "Change Me";
            if (mycfg.filter_last_updated == 0) mycfg.filter_last_updated = 7;
            if (mycfg.bitmap_mag_level == 0) mycfg.bitmap_mag_level = 4;
            if (mycfg.db_refreshrate == 0) mycfg.db_refreshrate = 5.0M;
            if (mycfg.be_refreshrate == 0) mycfg.be_refreshrate = 10.0M;

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
                keys[0] = table.Columns[0];
                mycfg.deployable_types.Tables[0].PrimaryKey = keys;
            }

            foreach (DataRow row in mycfg.vehicle_types.Tables[0].Rows)
                row.SetField<bool>("Show", true);

            foreach (DataRow row in mycfg.deployable_types.Tables[0].Rows)
                row.SetField<bool>("Show", true);

            // -> v5.0
            if (mycfg.cfgVersion < new ModuleVersion(5,0))
            {
                mycfg.player_state = new DataSet();
                DataTable table = mycfg.player_state.Tables.Add();
                table.Columns.Add(new DataColumn("UID", typeof(string)));
                table.Columns.Add(new DataColumn("Inventory", typeof(string)));
                table.Columns.Add(new DataColumn("Backpack", typeof(string)));
                DataColumn[] keys = new DataColumn[1];
                keys[0] = table.Columns[0];
                mycfg.player_state.Tables[0].PrimaryKey = keys;
            }
            if (mycfg.cfgVersion < new ModuleVersion(5, 1))
            {
                DataTable table = mycfg.player_state.Tables[0];
                table.Columns.Add(new DataColumn("State", typeof(string)));
                table.Columns.Add(new DataColumn("Model", typeof(string)));
            }

            try
            {
                textBoxDBURL.Text = mycfg.url;
                numericUpDownDBPort.Text = mycfg.port;
                textBoxDBBaseName.Text = mycfg.basename;
                textBoxDBUser.Text = mycfg.username;
                textBoxDBPassword.Text = mycfg.password;
                numericUpDownInstanceId.Text = mycfg.instance_id;
                textBoxVehicleMax.Text = mycfg.vehicle_limit;
                textBoxOldBodyLimit.Text = mycfg.body_time_limit;
                textBoxOldTentLimit.Text = mycfg.tent_time_limit;
                numericUpDownrConPort.Text = mycfg.rcon_port;
                textBoxrConURL.Text = mycfg.rcon_url;
                textBoxrConPassword.Text = mycfg.rcon_password;
                textBoxrConAdminName.Text = mycfg.rcon_adminname;
                trackBarLastUpdated.Value = Math.Min(trackBarLastUpdated.Maximum, Math.Max(trackBarLastUpdated.Minimum, mycfg.filter_last_updated));
                trackBarMagLevel.Value = Math.Min(trackBarMagLevel.Maximum, Math.Max(trackBarMagLevel.Minimum, mycfg.bitmap_mag_level));
                numericDBRefreshRate.Value = Math.Min(numericDBRefreshRate.Maximum, Math.Max(numericDBRefreshRate.Minimum, mycfg.db_refreshrate));
                numericBERefreshRate.Value = Math.Min(numericBERefreshRate.Maximum, Math.Max(numericBERefreshRate.Minimum, mycfg.be_refreshrate));

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
                mycfg.url = textBoxDBURL.Text;
                mycfg.port = numericUpDownDBPort.Text;
                mycfg.basename = textBoxDBBaseName.Text;
                mycfg.username = textBoxDBUser.Text;
                mycfg.password = textBoxDBPassword.Text;
                mycfg.instance_id = numericUpDownInstanceId.Text;
                mycfg.cfgVersion = curCfgVersion;
                mycfg.vehicle_limit = textBoxVehicleMax.Text;
                mycfg.body_time_limit = textBoxOldBodyLimit.Text;
                mycfg.tent_time_limit = textBoxOldTentLimit.Text;
                mycfg.rcon_port = numericUpDownrConPort.Text;
                mycfg.rcon_url = textBoxrConURL.Text;
                mycfg.rcon_password = textBoxrConPassword.Text;
                mycfg.rcon_adminname = textBoxrConAdminName.Text;

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
                    toolStripStatusWorld.ToolTipText = rowW.Field<string>("World Name");

                    string filepath = rowW.Field<string>("Filepath");

                    if (File.Exists(filepath) && Directory.Exists(configPath + "\\World" + mycfg.world_id))
                    {
                        virtualMap.nfo.tileBasePath = configPath + "\\World" + mycfg.world_id + "\\LOD";
                        virtualMap.Calibrate();
                    }
                    else
                    {
                        MessageBox.Show("Please select a bitmap for your world, and don't forget to adjust the map to your bitmap with the map helper...", "No bitmap selected");
                        //tabControl1.SelectedTab = tabPageMaps;
                        currentMode = displayMode.SetMaps;
                    }

                    virtualMap.nfo.defTileSize = new Tool.Size(rowW.Field<int>("TileSizeX"), rowW.Field<int>("TileSizeY"));
                    virtualMap.nfo.max_depth = rowW.Field<int>("TileDepth");
                    virtualMap.nfo.mag_depth = virtualMap.nfo.max_depth + mycfg.bitmap_mag_level;
                    tileReq.max_depth = virtualMap.nfo.max_depth;
                    virtualMap.SetRatio(new Tool.Size(rowW.Field<float>("RatioX"), rowW.Field<float>("RatioY")));
                    virtualMap.nfo.dbMapSize = new Tool.Size(rowW.Field<UInt32>("DB_Width"), rowW.Field<UInt32>("DB_Height"));
                    virtualMap.nfo.dbRefMapSize = new Tool.Size(rowW.Field<UInt32>("DB_refWidth"), rowW.Field<UInt32>("DB_refHeight"));
                    virtualMap.nfo.dbMapOffsetUnit = new Tool.Point(rowW.Field<int>("DB_X") / virtualMap.nfo.dbRefMapSize.Width,
                                                                    rowW.Field<int>("DB_Y") / virtualMap.nfo.dbRefMapSize.Height);
                }

                if (virtualMap.Enabled)
                    mapZoom.currDepth = Math.Log(virtualMap.ResizeFromZoom((float)Math.Pow(2, mapZoom.currDepth)), 2);

                Tool.Size sizePanel = splitContainer1.Panel1.Size;
                virtualMap.Position = (Tool.Point)((sizePanel - virtualMap.SizeCorrected) * 0.5f);

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

        //
        private void _MapHelperStateChanged()
        {
            if (mapHelper == null)
                return;

            if (mapHelper.enabled)
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
        private void RetrieveExternalLocalIP()
        {
            new Thread(new ThreadStart(delegate
                {
                    WebClient wc = new WebClient();
                    string strIP = wc.DownloadString("http://checkip.dyndns.org");
                    strIP = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(strIP).Value;
                    wc.Dispose();

                    localIP = strIP;
                })).Start();
        }
        private string LocalResolveIP(string ip)
        {
            DataRow found;

            if (ip.CompareTo(mycfg.url) == 0)
                ip = "server IP";
            else if (ip.CompareTo(mycfg.rcon_url) == 0)
                ip = "rCon IP";
            else if((found = PlayersOnline.Tables[0].Rows.FindFrom("IP", ip)) != null)
                ip = found.Field<string>("Name");
            else if (ip.CompareTo(localIP) == 0)
                ip = "local IP";

            return ip;
        }
        private System.Drawing.Imaging.ImageAttributes GetPlayerColor(string uid)
        {
            System.Drawing.Imaging.ImageAttributes value = null;
            if (attrColorPlayers.TryGetValue(uid, out value) == false)
                attrColorPlayers[uid] = value = new System.Drawing.Imaging.ImageAttributes();

            uint crc = CRC32.Compute(Helpers.String2Bytes(uid));
            Random rand = new Random();
            float r = (float)((crc & 255) / 255.0f) * 0.5f;
            float g = (float)(((crc >> 8) & 255) / 255.0f) * 0.5f;
            float b = (float)(((crc >> 16) & 255) / 255.0f) * 0.5f;
            float[][] matrix =
            { 
                new float[] {r+0.5f,  0,  0,  0,  0},
                new float[] {0,  g+0.5f,  0,  0,  0},
                new float[] {0,  0,  b+0.5f,  0,  0},
                new float[] {0,  0,  0,  1,  0},
                new float[] {0,  0,  0,  0,  1}  
            };
            value.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(matrix));

            return value;
        }

        #region CBfromDesigner
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            cb_buttonConnect_Click(sender, e);
        }

        private void dataGridViewMaps_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            cb_dataGridViewMaps_CellClick(sender, e);
        }

        private void splitContainer1_Panel1_SizeChanged(object sender, EventArgs e)
        {
            cb_splitContainer1_Panel1_SizeChanged(sender, e);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            cb_Panel1_Paint(sender, e);
        }

        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            cb_Panel1_MouseClick(sender, e);
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            cb_Panel1_MouseDown(sender, e);
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            cb_Panel1_MouseMove(sender, e);
        }

        private void Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            cb_Panel1_MouseUp(sender, e);
        }

        private void trackBarMagLevel_ValueChanged(object sender, EventArgs e)
        {
            cb_trackBarMagLevel_ValueChanged(sender, e);
        }

        private void trackBarLastUpdated_ValueChanged(object sender, EventArgs e)
        {
            cb_trackBarLastUpdated_ValueChanged(sender, e);
        }

        private void buttonSelectCustom_Click(object sender, EventArgs e)
        {
            cb_buttonSelectCustom_Click(sender, e);
        }

        private void buttonCustom_Click(object sender, EventArgs e)
        {
            cb_buttonCustom_Click(sender, e);
        }

        private void buttonRemoveTents_Click(object sender, EventArgs e)
        {
            cb_buttonRemoveTents_Click(sender, e);
        }

        private void buttonRemoveBodies_Click(object sender, EventArgs e)
        {
            cb_buttonRemoveBodies_Click(sender, e);
        }

        private void buttonSpawnNew_Click(object sender, EventArgs e)
        {
            cb_buttonSpawnNew_Click(sender, e);
        }

        private void buttonBackup_Click(object sender, EventArgs e)
        {
            cb_buttonBackup_Click(sender, e);
        }

        private void buttonRemoveDestroyed_Click(object sender, EventArgs e)
        {
            cb_buttonRemoveDestroyed_Click(sender, e);
        }

        private void dataGridViewVehicleTypes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            cb_dataGridViewVehicleTypes_CellContentClick(sender, e);
        }

        private void dataGridViewVehicleTypes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            cb_dataGridViewVehicleTypes_CellValueChanged(sender, e);
        }

        private void dataGridViewVehicleTypes_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            cb_dataGridViewVehicleTypes_ColumnHeaderMouseDoubleClick(sender, e);
        }

        private void dataGridViewDeployableTypes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            cb_dataGridViewDeployableTypes_CellContentClick(sender, e);
        }

        private void dataGridViewDeployableTypes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            cb_dataGridViewDeployableTypes_CellValueChanged(sender, e);
        }

        private void dataGridViewDeployableTypes_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            cb_dataGridViewDeployableTypes_ColumnHeaderMouseDoubleClick(sender, e);
        }

        private void numericBERefreshRate_ValueChanged(object sender, EventArgs e)
        {
            cb_numericBERefreshRate_ValueChanged(sender, e);
        }

        private void numericDBRefreshRate_ValueChanged(object sender, EventArgs e)
        {
            cb_numericDBRefreshRate_ValueChanged(sender, e);
        }

        private void comboSelectInstance_SelectedValueChanged(object sender, EventArgs e)
        {
            cb_comboSelectInstance_SelectedValueChanged(sender, e);
        }

        private void comboSelectEpochWorld_SelectedValueChanged(object sender, EventArgs e)
        {
            cb_comboSelectEpochWorld_SelectedValueChanged(sender, e);
        }

        private void cbCartographer_CheckedChanged(object sender, EventArgs e)
        {
            cb_cbCartographer_CheckedChanged(sender, e);
        }

        private void textBoxChatInput_TextChanged(object sender, EventArgs e)
        {
            cb_textBoxChatInput_TextChanged(sender, e);
        }

        private void richTextBoxChat_TextChanged(object sender, EventArgs e)
        {
            cb_richTextBoxChat_TextChanged(sender, e);
        }

        private void bgWorkerRefreshBattEye_DoWork(object sender, DoWorkEventArgs e)
        {
            cb_bgWorkerRefreshBattEye_DoWork(sender, e);
        }

        private void bgWorkerRefreshLeds_DoWork(object sender, DoWorkEventArgs e)
        {
            cb_bgWorkerRefreshLeds_DoWork(sender, e);
        }

        private void toolStripStatusDeployable_Click(object sender, EventArgs e)
        {
            cb_toolStripStatusDeployable_Click(sender, e);
        }

        private void textBoxrConAdminName_TextChanged(object sender, EventArgs e)
        {
            mycfg.rcon_adminname = textBoxrConAdminName.Text;
        }

        private void toolStripMenuItemHealPlayer_Click(object sender, EventArgs e)
        {
            cb_toolStripMenuItemHealPlayer_Click(sender, e);
        }

        private void contextMenuStripItemPlayerMenu_Opening(object sender, CancelEventArgs e)
        {
            cb_contextMenuStripItemPlayerMenu_Opening(sender, e);
        }

        private void toolStripMenuItemRevivePlayer_Click(object sender, EventArgs e)
        {
            cb_toolStripMenuItemRevivePlayer_Click(sender, e);
        }

        private void toolStripMenuItemSavePlayerState_Click(object sender, EventArgs e)
        {
            cb_toolStripMenuItemSavePlayerState_Click(sender, e);
        }

        private void toolStripMenuItemRestorePlayerState_Click(object sender, EventArgs e)
        {
            cb_toolStripMenuItemRestorePlayerState_Click(sender, e);
        }
        #endregion

        #region Icons

        private enum displayMode
	    {
            InTheVoid = 0,
            SetMaps,
	        ShowOnline,
            ShowAlive,
            ShowDead,
            ShowVehicle,
            ShowSpawn,
            ShowTraders,
            ShowDeployable,
            MapHelper,
	    }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = true) ]
        public class UsableInAttribute : Attribute
        {
             public string target;
             public UsableInAttribute(string target)
             {
                 this.target = target;
             }
        }

        private enum IconType
        {
            [UsableIn("Vehicle"), UsableIn("Deployable")]
            Air,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            Atv,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            Bicycle,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            Boat,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            Bus,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            Car,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            Helicopter,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            Motorcycle,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            SUV,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            Tractor,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            Truck,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            UAZ,

            [UsableIn("Deployable")]
            Tent,

            [UsableIn("Deployable")]
            Stach,

            [UsableIn("Deployable")]
            SmallBuild,

            [UsableIn("Deployable")]
            LargeBuild,

            [UsableIn("Vehicle"), UsableIn("Deployable")]
            Unknown
        }

        private class ToolIcon
        {
            public ToolIcon(ToolStripStatusLabel control, Bitmap normal, Bitmap selected)
            {
                this.control = control;
                this.normal = normal;
                this.selected = selected;
            }

            public bool Select
            {
                set
                {
                    if(value)
                    {
                        control.Image = this.selected;
                        control.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                        //control.BorderSides = ToolStripStatusLabelBorderSides.All;
                    }
                    else
                    {
                        control.Image = this.normal;
                        control.Font = new System.Drawing.Font("Segoe UI", 9F);
                        //control.BorderSides = ToolStripStatusLabelBorderSides.None;
                    }
                }
            }

            private ToolStripStatusLabel control;
            private Bitmap normal;
            private Bitmap selected;
        }
        #endregion

        private void toolStripStatusTrail_MouseDown(object sender, MouseEventArgs e)
        {
            cb_toolStripStatusTrail_MouseDown(sender, e);
        }
    }
}
public static class DataRowCollectionExtensions
{
    public static DataRow FindFrom(this DataRowCollection rowCollection, string columnName, object value)
    {
        if(rowCollection.Count > 0)
        {
            int idxCol = rowCollection[0].Table.Columns.IndexOf(columnName);
            if (idxCol >= 0)
            {
                foreach (DataRow row in rowCollection)
                {
                    object v = row.ItemArray[idxCol];

                    if (value.Equals(v))
                        return row;
                }
            }
        }

        return null;
    }
}

