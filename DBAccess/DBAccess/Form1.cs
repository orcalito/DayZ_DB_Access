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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DBAccess
{
    public partial class Form1 : Form
    {
        static ModuleVersion curCfgVersion = new ModuleVersion(2, 0);

        private static int bUserAction = 0;
        private static Mutex mtxUpdateDB = new Mutex();
        private static Mutex mtxUseDS = new Mutex();
        public DlgUpdateIcons dlgUpdateIcons;
        //
        private MySqlConnection cnx;
        private bool bConnected = false;
        private RadioButton currDisplayedItems;
        //
        public myConfig mycfg = new myConfig();
        private string configPath;
        private string configFilePath;
        private VirtualMap virtualMap = new VirtualMap();
        private double zoomFactor = 1.0;
        //
        private DataSet dsInstances = new DataSet();
        private DataSet dsDeployables = new DataSet();
        private DataSet dsAlivePlayers = new DataSet();
        private DataSet dsOnlinePlayers = new DataSet();
        private DataSet dsVehicles = new DataSet();
        private DataSet dsVehicleSpawnPoints = new DataSet();

        private Dictionary<UInt64, UIDGraph> dicUIDGraph = new Dictionary<UInt64, UIDGraph>();
        private List<iconDB> listIcons = new List<iconDB>();
        private List<iconDB> iconsDB = new List<iconDB>();
        private List<myIcon> iconPlayers = new List<myIcon>();
        private List<myIcon> iconVehicles = new List<myIcon>();
        private List<myIcon> iconDeployables = new List<myIcon>();
        private DragNDrop dragndrop = new DragNDrop();

        private MapHelper mapHelper;

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

            bgWorker.RunWorkerAsync();

            Enable(false);
        }
        void ApplyMapChanges()
        {
            if (!virtualMap.Enabled)
                return;

            Size sizePanel = splitContainer1.Panel1.Size;

            Point halfPanel = new Point((int)(sizePanel.Width * 0.5f), (int)(sizePanel.Height * 0.5f));

            virtualMap.Position.X = Math.Min(halfPanel.X - 16, virtualMap.Position.X);
            virtualMap.Position.Y = Math.Min(halfPanel.Y - 16, virtualMap.Position.Y);

            virtualMap.Position.X = Math.Max(halfPanel.X - virtualMap.Size.Width + 16, virtualMap.Position.X);
            virtualMap.Position.Y = Math.Max(halfPanel.Y - virtualMap.Size.Height + 16, virtualMap.Position.Y);

            RefreshIcons();
        }
        void Form1Resize(object sender, EventArgs e)
        {
            ApplyMapChanges();
        }
        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            interpolationMode = InterpolationMode.NearestNeighbor;

            System.Threading.Interlocked.CompareExchange(ref bUserAction, 1, 0);

            if (e.Button.HasFlag(MouseButtons.Right) && (mapHelper != null))
            {
                if (mapHelper.enabled)
                {
                    Point mousePos = new Point(e.Location.X - virtualMap.Position.X,
                                               e.Location.Y - virtualMap.Position.Y);
                    mapHelper.isDraggingCtrlPoint = mapHelper.IntersectControl(mousePos, 5);

                    if (mapHelper.isDraggingCtrlPoint > 0)
                    {
                        // Will drag selected Control point
                        Point pt = new Point((int)(mapHelper.controls[mapHelper.isDraggingCtrlPoint].X * virtualMap.SizeCorrected.Width + virtualMap.Position.X),
                                             (int)(mapHelper.controls[mapHelper.isDraggingCtrlPoint].Y * virtualMap.SizeCorrected.Height + virtualMap.Position.Y));
                        dragndrop.Start(pt);
                    }
                    else
                    {
                        // Will drag all Control points
                        List<Point> points = new List<Point>(4);
                        for (int i = 0; i < 4; i++)
                        {
                            Point pt = new Point((int)(mapHelper.controls[i].X * virtualMap.SizeCorrected.Width + virtualMap.Position.X),
                                                 (int)(mapHelper.controls[i].Y * virtualMap.SizeCorrected.Height + virtualMap.Position.Y));
                            points.Add(pt);
                        }
                        dragndrop.Start(points);
                    }

                    splitContainer1.Panel1.Invalidate();
                    return;
                }
            }

            //
            dragndrop.Start(virtualMap.Position);

            Rectangle mouseRec = new Rectangle(e.Location, Size.Empty);
            Rectangle r = new Rectangle(Point.Empty, new Size(24, 24)    /* !!!! annoying shortcut, remove it later !!!! */);
            foreach (iconDB idb in listIcons)
            {
                r.Location = idb.icon.Location;

                if (mouseRec.IntersectsWith(r))
                {
                    idb.icon.OnClick(this, e);
                    break;
                }
            }
        }
        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Right) && (mapHelper != null))
            {
                if (mapHelper.enabled)
                {
                    dragndrop.Update();

                    if (mapHelper.isDraggingCtrlPoint >= 0)
                    {
                        Point newPos = dragndrop.Position(0);
                        PointF pt = new PointF((newPos.X - virtualMap.Position.X) / (float)virtualMap.SizeCorrected.Width,
                                                (newPos.Y - virtualMap.Position.Y) / (float)virtualMap.SizeCorrected.Height);

                        mapHelper.controls[mapHelper.isDraggingCtrlPoint] = pt;
                        mapHelper.ControlPointUpdated(mapHelper.isDraggingCtrlPoint);
                    }
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Point newPos = dragndrop.Position(i);
                            PointF pt = new PointF((newPos.X - virtualMap.Position.X) / (float)virtualMap.SizeCorrected.Width,
                                                    (newPos.Y - virtualMap.Position.Y) / (float)virtualMap.SizeCorrected.Height);

                            mapHelper.controls[i] = pt;
                        }
                        mapHelper.ControlPointUpdated(0);
                    }
    
                    ApplyMapChanges();
                }
            }
            else if (e.Button.HasFlag(MouseButtons.Left))
            {
                dragndrop.Update();
                virtualMap.Position = dragndrop.Position(0);
                ApplyMapChanges();
            }
            else
            {
                if (mapHelper != null)
                {
                    Point mousePos = new Point(e.Location.X - virtualMap.Position.X,
                                               e.Location.Y - virtualMap.Position.Y);
                    mapHelper.isDraggingCtrlPoint = mapHelper.IntersectControl(mousePos, 5);
                    splitContainer1.Panel1.Invalidate();
                }

                dragndrop.Stop();

                if (interpolationMode == InterpolationMode.NearestNeighbor)
                {
                    interpolationMode = InterpolationMode.Bicubic;
                    splitContainer1.Panel1.Invalidate();
                }
            }
        }
        private void Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if(mapHelper != null)
                mapHelper.isDraggingCtrlPoint = -1;

            System.Threading.Interlocked.CompareExchange(ref bUserAction, 0, 1);
            interpolationMode = InterpolationMode.Bicubic;
            splitContainer1.Panel1.Invalidate();
        }
        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!virtualMap.Enabled)
                return;

            interpolationMode = InterpolationMode.NearestNeighbor;
            System.Threading.Interlocked.CompareExchange(ref bUserAction, 1, 0);

            Point mousePos = new Point(e.Location.X - virtualMap.Position.X,
                                       e.Location.Y - virtualMap.Position.Y);
            PointF mouseUnit = new PointF((e.Location.X - virtualMap.Position.X) / (float)virtualMap.SizeCorrected.Width,
                                          (e.Location.Y - virtualMap.Position.Y) / (float)virtualMap.SizeCorrected.Height);

            double newZoom = zoomFactor * (1.0 + ((e.Delta / 120.0) * 0.1));
            //
            newZoom = virtualMap.ResizeFromZoom(newZoom);

            double deltaZ = newZoom - zoomFactor;
            if( deltaZ != 0.0 )
            {
                Point newPos = Point.Truncate(new PointF(mouseUnit.X * virtualMap.SizeCorrected.Width,
                                                         mouseUnit.Y * virtualMap.SizeCorrected.Height));

                Size delta = new Size(newPos.X - mousePos.X, newPos.Y - mousePos.Y);
                virtualMap.Position = Point.Subtract(virtualMap.Position, delta);

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

                switch (mycfg.game_type)
                {
                    case "Epoch":
                        DBEpoch_OnConnection();
                        DBEpoch_OnRefresh();
                        break;
                    default:
                        DB_OnConnection();
                        DB_OnRefresh();
                        break;
                }

                SetCurrentMap();
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

            dsDeployables.Clear();
            dsAlivePlayers.Clear();
            dsOnlinePlayers.Clear();
            dsVehicleSpawnPoints.Clear();
            dsVehicles.Clear();

            try
            {
                foreach( KeyValuePair<UInt64,UIDGraph> pair in dicUIDGraph)
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
                textBoxCmdStatus.Text = ex.ToString();
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
                textBoxCmdStatus.Text = ex.ToString();
            }

            mtxUseDS.ReleaseMutex();
        }
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

            if (GetUIDType(pb.iconDB.uid) == UIDType.TypeVehicle)
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
        }
        private void OnDeployableClick(object sender, EventArgs e)
        {
            myIcon pb = sender as myIcon;

            Deployable deployable = new Deployable(pb.iconDB);
            deployable.Rebuild();
            propertyGrid1.SelectedObject = deployable;
            propertyGrid1.ExpandAllGridItems();
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
                        Rectangle recTile = virtualMap.TileRectangle(x, y);

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
                    idb.icon.Location = virtualMap.VirtualPosition(idb);

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

                idb.uid = UInt64.Parse(row.Field<string>("unique_id")) | (UInt64)UIDType.TypePlayer;
                idb.row = row;
                idb.pos = GetUnitPosFromString(row.Field<string>("worldspace"));

                if (idx >= iconPlayers.Count)
                {
                    myIcon icon = new myIcon();
                    icon.Size = new Size(24, 24);
                    icon.Click += OnPlayerClick;
                    iconPlayers.Add(icon);
                }

                idb.icon = iconPlayers[idx];
                idb.icon.image = global::DBAccess.Properties.Resources.iconOnline;
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

                idb.uid = UInt64.Parse(row.Field<string>("unique_id")) | (UInt64)UIDType.TypePlayer;
                idb.row = row;
                idb.pos = GetUnitPosFromString(row.Field<string>("worldspace"));

                if (idx >= iconPlayers.Count)
                {
                    myIcon icon = new myIcon();
                    icon.Size = new Size(24, 24);
                    icon.Click += OnPlayerClick;
                    iconPlayers.Add(icon);
                }

                idb.icon = iconPlayers[idx];
                idb.icon.image = global::DBAccess.Properties.Resources.iconAlive;
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
            int idx=0;
            foreach (DataRow row in dsVehicles.Tables[0].Rows)
            {
                double damage = row.Field<double>("damage");

                if (idx >= iconsDB.Count)
                    iconsDB.Add(new iconDB());

                iconDB idb = iconsDB[idx];

                idb.uid = row.Field<UInt64>("id") | (UInt64)UIDType.TypeVehicle;
                idb.row = row;
                idb.pos = GetUnitPosFromString(row.Field<string>("worldspace"));

                if( idx >= iconVehicles.Count)
                {
                    myIcon icon = new myIcon();
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
                }
                else
                {
                    idb.icon.image = global::DBAccess.Properties.Resources.iconDestroyed;
                }

                Control tr = new Control();
                tr.ContextMenuStrip = null;
                idb.icon.iconDB = idb;
                idb.icon.contextMenuStrip = contextMenuStripVehicle;

                //toolTip1.SetToolTip(idb.icon, row.Field<UInt64>("id").ToString() + ": "+ row.Field<string>("class_name"));

                listIcons.Add(idb);

                if (checkBoxShowTrail.Checked)
                    GetUIDGraph(idb.uid).AddPoint(idb.pos);

                idx++;
            }
        }
        private void BuildSpawnIcons()
        {
            if (dsVehicleSpawnPoints.Tables.Count == 0)
                return;

            int idx = 0;
            foreach (DataRow row in dsVehicleSpawnPoints.Tables[0].Rows)
            {
                if (idx >= iconsDB.Count)
                    iconsDB.Add(new iconDB());

                iconDB idb = iconsDB[idx];

                idb.uid = row.Field<UInt64>("id") | (UInt64)UIDType.TypeSpawn;
                idb.row = row;
                idb.pos = GetUnitPosFromString(row.Field<string>("worldspace"));

                if( idx >= iconVehicles.Count)
                {
                    myIcon icon = new myIcon();
                    icon.Size = new Size(24, 24);
                    icon.Click += OnVehicleClick;
                    iconVehicles.Add(icon);
                }

                idb.icon = iconVehicles[idx];

                DataRow rowT = mycfg.vehicle_types.Tables[0].Rows.Find(row.Field<string>("class_name"));

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
                idb.icon.contextMenuStrip = contextMenuStripSpawn;

                //toolTip1.SetToolTip(idb.icon, row.Field<UInt64>("id").ToString() + ": " + row.Field<string>("class_name"));

                listIcons.Add(idb);

                idx++;
            }
        }
        private void BuildDeployableIcons()
        {
            int idx = 0;
            foreach (DataRow row in dsDeployables.Tables[0].Rows)
            {
                if (idx >= iconsDB.Count)
                    iconsDB.Add(new iconDB());

                iconDB idb = iconsDB[idx];

                idb.uid = row.Field<UInt64>("id") | (UInt64)UIDType.TypeDeployable;
                idb.row = row;
                idb.pos = GetUnitPosFromString(row.Field<string>("worldspace"));

                if (idx >= iconDeployables.Count)
                {
                    myIcon icon = new myIcon();
                    icon.Size = new Size(24, 24);
                    icon.Click += OnDeployableClick;
                    iconDeployables.Add(icon);
                }

                idb.icon = iconDeployables[idx];
                idb.icon.iconDB = idb;
                UInt64 did = row.Field<UInt64>("deployable_id");
                idb.icon.image = (did == 1) ? global::DBAccess.Properties.Resources.tent : global::DBAccess.Properties.Resources.stach;

                listIcons.Add(idb);

                idx++;
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

            textBoxURL.Enabled = !bState;
            textBoxBaseName.Enabled = !bState;
            textBoxPort.Enabled = !bState;
            textBoxUser.Enabled = !bState;
            textBoxPassword.Enabled = !bState;
            comboBoxGameType.Enabled = !bState;

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
                using (StreamReader re = new StreamReader(configFilePath))
                {
                    mycfg = xs.Deserialize(re) as myConfig;
                }
            }
            catch (Exception)
            {
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

                DataColumn[] keys = new DataColumn[1];
                keys[0] = mycfg.worlds_def.Tables[0].Columns[0];
                mycfg.worlds_def.Tables[0].PrimaryKey = keys;

                System.Data.DataColumn col = new DataColumn();

                table.Rows.Add(1, "Chernarus", "", 0, 0, 0, 0, 0, 0, 14700, 15360 );
                table.Rows.Add(2, "Lingor", "", 0, 0, 0, 0, 0, 0, 10000, 10000);
                table.Rows.Add(3, "Utes", "", 0, 0, 0, 0, 0, 0, 5100, 5100);
                table.Rows.Add(4, "Takistan", "", 0, 0, 0, 0, 0, 0, 14000, 14000);
                table.Rows.Add(5, "Panthera2", "", 0, 0, 0, 0, 0, 0, 10200, 10200);
                table.Rows.Add(6, "Fallujah", "", 0, 0, 0, 0, 0, 0, 10200, 10200);
                table.Rows.Add(7, "Zargabad", "", 0, 0, 0, 0, 0, 0, 8000, 8000);
                table.Rows.Add(8, "Namalsk", "", 0, 0, 0, 0, 0, 0, 12000, 12000);
                table.Rows.Add(9, "Celle2", "", 0, 0, 0, 0, 0, 0, 13000, 13000);
                table.Rows.Add(10, "Taviana", "", 0, 0, 0, 0, 0, 0, 25600, 25600);
            }

            // -> v1.8
            if (mycfg.cfgVersion < curCfgVersion)
            {
                DataColumnCollection cols = mycfg.worlds_def.Tables[0].Columns;
                
                if (cols.Contains("Width")) cols["Width"].ColumnName = "DB_refWidth";
                if (cols.Contains("Height")) cols["Height"].ColumnName = "DB_refHeight";

                if (!cols.Contains("RatioX")) cols.Add(new DataColumn("RatioX", typeof(float), "", MappingType.Hidden));
                if (!cols.Contains("RatioY")) cols.Add(new DataColumn("RatioY", typeof(float), "", MappingType.Hidden));
                if (!cols.Contains("TileSizeX")) cols.Add(new DataColumn("TileSizeX", typeof(int), "", MappingType.Hidden));
                if (!cols.Contains("TileSizeY")) cols.Add(new DataColumn("TileSizeY", typeof(int), "", MappingType.Hidden));
                if (!cols.Contains("TileDepth")) cols.Add(new DataColumn("TileDepth", typeof(int), "", MappingType.Hidden));

                if (!cols.Contains("DB_X")) cols.Add(new DataColumn("DB_X", typeof(int), "", MappingType.Hidden));
                if (!cols.Contains("DB_Y")) cols.Add(new DataColumn("DB_Y", typeof(int), "", MappingType.Hidden));

                if (!cols.Contains("DB_Width"))
                {
                    cols.Add(new DataColumn("DB_Width", typeof(UInt32), "", MappingType.Hidden));
                    foreach (DataRow row in mycfg.worlds_def.Tables[0].Rows)
                        row.SetField<UInt32>("DB_Width", row.Field<UInt32>("DB_refWidth"));
                }
                if (!cols.Contains("DB_Height"))
                {
                    cols.Add(new DataColumn("DB_Height", typeof(UInt32), "", MappingType.Hidden));
                    foreach (DataRow row in mycfg.worlds_def.Tables[0].Rows)
                        row.SetField<UInt32>("DB_Height", row.Field<UInt32>("DB_refHeight"));
                }

                foreach (DataRow row in mycfg.worlds_def.Tables[0].Rows)
                {
                    row.SetField<float>("RatioX", 0);
                    row.SetField<float>("RatioY", 0);
                    row.SetField<int>("TileSizeX", 0);
                    row.SetField<int>("TileSizeY", 0);
                    row.SetField<int>("TileDepth", 0);
                    row.SetField<int>("DB_X", 0);
                    row.SetField<int>("DB_Y", 0);
                }
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
                comboBoxGameType.SelectedItem = mycfg.game_type;
                textBoxVehicleMax.Text = mycfg.vehicle_limit;
                textBoxOldBodyLimit.Text = mycfg.body_time_limit;
                textBoxOldTentLimit.Text = mycfg.tent_time_limit;

                dataGridViewMaps.Columns["ColumnID"].DataPropertyName = "World ID";
                dataGridViewMaps.Columns["ColumnName"].DataPropertyName = "World Name";
                dataGridViewMaps.Columns["ColumnPath"].DataPropertyName = "Filepath";
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
            catch (Exception)
            {
            }
        }
        private PointF GetUnitPosFromString(string from)
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

            return new PointF((float)x, (float)y);
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

                    virtualMap.nfo.defTileSize = new Size(rowW.Field<int>("TileSizeX"), rowW.Field<int>("TileSizeY"));
                    virtualMap.nfo.depth = rowW.Field<int>("TileDepth");
                    virtualMap.SetRatio(new SizeF(rowW.Field<float>("RatioX"), rowW.Field<float>("RatioY")));
                    virtualMap.nfo.dbMapSize = new Size((int)rowW.Field<UInt32>("DB_Width"), (int)rowW.Field<UInt32>("DB_Height"));
                    virtualMap.nfo.dbRefMapSize = new SizeF(rowW.Field<UInt32>("DB_refWidth"), rowW.Field<UInt32>("DB_refHeight"));
                    virtualMap.nfo.dbMapOffsetUnit = new PointF(rowW.Field<int>("DB_X") / virtualMap.nfo.dbRefMapSize.Width,
                                                                rowW.Field<int>("DB_Y") / virtualMap.nfo.dbRefMapSize.Height);
                }

                if ( virtualMap.Enabled )
                    zoomFactor = virtualMap.ResizeFromZoom(1.0f);

                Size sizePanel = splitContainer1.Panel1.Size;
                Point halfPanel = new Point((int)(sizePanel.Width * 0.5f), (int)(sizePanel.Height * 0.5f));
                virtualMap.Position.X = (int)(halfPanel.X - virtualMap.Size.Width * 0.5f);
                virtualMap.Position.Y = (int)(halfPanel.Y - virtualMap.Size.Height * 0.5f);

                mapHelper = new MapHelper(virtualMap);

                ApplyMapChanges();
            }
            catch (Exception ex)
            {
                textBoxCmdStatus.Text += ex.ToString();
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
                foreach (DataRow row in _dsAllVehicleTypes.Tables[0].Rows)
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
                    cmd.CommandText = "SELECT w.id id, w.worldspace worldspace, w.chance chance, v.inventory inventory, v.class_name class_name FROM world_vehicle as w, vehicle as v"
                                    + " WHERE w.world_id=" + mycfg.world_id + " AND w.vehicle_id=v.id";
                    _dsVehicleSpawnPoints.Clear();
                    adapter.Fill(_dsVehicleSpawnPoints);

                    //
                    //  Deployables
                    //
                    cmd.CommandText = "SELECT id, CAST(deployable_id AS UNSIGNED) deployable_id, worldspace, inventory, FROM instance_deployable WHERE instance_id=" + mycfg.instance_id + " AND (deployable_id=1 OR deployable_id=66 OR deployable_id=67)";
                    _dsDeployables.Clear();
                    adapter.Fill(_dsDeployables);
                }
                catch (Exception ex)
                {
                    textBoxCmdStatus.Text = ex.ToString();
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
                MySqlCommand cmd = cnx.CreateCommand();
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
                this.textBoxCmdStatus.Text += ex.ToString();
            }

            mtxUpdateDB.ReleaseMutex();

            mtxUseDS.WaitOne();

            try
            {
                foreach (DataRow row in _dsVehicles.Tables[0].Rows)
                {
                    string name = row.Field<string>("class_name");

                    if (mycfg.vehicle_types.Tables[0].Rows.Find(name) == null)
                        mycfg.vehicle_types.Tables[0].Rows.Add(name, "Car");
                }
            }
            catch (Exception ex)
            {
                textBoxCmdStatus.Text += ex.ToString();
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
                    MySqlCommand cmd = cnx.CreateCommand();
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
                    //  Deployables                                 à Remapper
                    //
                    cmd.CommandText = "SELECT CAST(ObjectID AS UNSIGNED) id, CAST(0 AS UNSIGNED) deployable_id, worldspace, inventory FROM object_data WHERE CharacterID!=0";
                    _dsDeployables.Clear();
                    adapter.Fill(_dsDeployables);
                }
                catch (Exception ex)
                {
                    textBoxCmdStatus.Text = ex.ToString();
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
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            while (!bw.CancellationPending)
            {
                Thread.Sleep(5000);

                switch (mycfg.game_type)
                {
                    case "Epoch": DBEpoch_OnRefresh(); break;
                    default: DB_OnRefresh(); break;
                }

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
                    Enable(false);
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

        class tileReq
        {
            public string path;
            public Rectangle rec;
        }
        class tileNfo
        {
            public tileNfo(string path)
            {
                bFileExists = File.Exists(path);
                if (bFileExists)
                {
                    this.path = path;
                    //this.bitmap = new Bitmap(path);
                    using (var bmpTemp = new Bitmap(path))
                    {
                        this.bitmap = new Bitmap(bmpTemp);
                    }
                }
                ticks = DateTime.Now.Ticks;
            }
            ~tileNfo()
            {
                if (bitmap != null)
                    bitmap.Dispose();
            }

            public bool bFileExists = false;
            public string path;
            public Bitmap bitmap;
            public long ticks;
        }
        List<tileReq> tileRequests = new List<tileReq>();
        List<tileNfo> tileCache = new List<tileNfo>();

        InterpolationMode interpolationMode = InterpolationMode.Bicubic;
        PixelOffsetMode pixelOffsetMode = PixelOffsetMode.Half;


        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (virtualMap.Enabled)
                {
                    e.Graphics.CompositingMode = CompositingMode.SourceCopy;
                    e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                    e.Graphics.InterpolationMode = interpolationMode;
                    e.Graphics.PixelOffsetMode = pixelOffsetMode;

                    int nb_tilesDrawn = 0;
                    foreach (tileReq req in tileRequests)
                    {
                        tileNfo nfo = tileCache.Find(x => req.path == x.path);

                        if( nfo != null )
                        {
                            e.Graphics.DrawImage(nfo.bitmap, req.rec);
                            nb_tilesDrawn++;
                        }
                    }

                    e.Graphics.CompositingMode = CompositingMode.SourceOver;

                    if (!mapHelper.enabled)
                    {
                        if (checkBoxShowTrail.Checked)
                        {
                            foreach (iconDB idb in listIcons)
                            {
                                UIDType type = GetUIDType(idb.uid);

                                if ((type == UIDType.TypePlayer) || (type == UIDType.TypeVehicle))
                                    GetUIDGraph(idb.uid).DisplayInMap(e.Graphics, virtualMap);
                            }
                        }

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

                    //textBoxCmdStatus.Text = "\r\nTiles Requested=" + tileRequests.Count + "\r\nTiles displayed: " + nb_tilesDrawn;
                    //textBoxCmdStatus.Text += "\r\nIcons Requested=" + listIcons.Count + "\r\nIcons displayed: " + nb_iconsDrawn;
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
                foreach (KeyValuePair<UInt64, UIDGraph> pair in dicUIDGraph)
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
                Enable(false);
            }

            mtxUpdateDB.ReleaseMutex();
            this.Cursor = Cursors.Arrow;

            return res;
        }
        private void dataGridViewMaps_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewMaps.Rows.Count))
                return;

            // Ignore clicks that are not on button cells.  
            if (e.ColumnIndex == dataGridViewMaps.Columns["ColumnChoosePath"].Index)
            {
                DialogResult res = openFileDialog1.ShowDialog();

                if (res == DialogResult.OK)
                {
                    dataGridViewMaps["ColumnPath", e.RowIndex].Value = openFileDialog1.FileName;

                    try
                    {
                        string filepath = openFileDialog1.FileName;
                        int world_id = int.Parse(dataGridViewMaps["ColumnID", e.RowIndex].Value.ToString());

                        if (File.Exists(filepath))
                        {
                            FileInfo fi = new FileInfo(filepath);

                            string tileBasePath = configPath + "\\World" + world_id + "\\LOD";

                            MessageBox.Show("Please wait while generating tiles...\r\nThis is done once when selecting a new map.");

                            tileCache.Clear();

                            this.Cursor = Cursors.WaitCursor;

                            DirectoryInfo di = new DirectoryInfo(configPath + "\\World" + world_id);
                            if( di.Exists )
                                di.Delete(true);

                            Tuple<Size, Size, Size> sizes = Tool.CreateTiles(filepath, tileBasePath, 256);

                            DataRow rowW = mycfg.worlds_def.Tables[0].Rows.Find(world_id);
                            rowW.SetField<float>("RatioX", sizes.Item1.Width / (float)sizes.Item2.Width);
                            rowW.SetField<float>("RatioY", sizes.Item1.Height / (float)sizes.Item2.Height);
                            rowW.SetField<int>("TileSizeX", sizes.Item3.Width);
                            rowW.SetField<int>("TileSizeY", sizes.Item3.Height);

                            int tileCount = sizes.Item2.Width / 256;
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

                    if (dataGridViewMaps["ColumnID", e.RowIndex].Value.ToString() == mycfg.world_id.ToString())
                        SetCurrentMap();
                }
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
        internal UIDGraph GetUIDGraph(UInt64 uid)
        {
            UIDGraph uidgraph = null;

            if (dicUIDGraph.TryGetValue(uid, out uidgraph) == false)
                dicUIDGraph[uid] = uidgraph = new UIDGraph(new Pen(Color.Red, 2));

            return uidgraph;
        }
        //
        //
        //
        public class iconDB
        {
            public myIcon icon;
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

                ArrayList arr = Tool.ParseInventoryString(idb.row.Field<string>("medical"));
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

                arr = Tool.ParseInventoryString(idb.row.Field<string>("inventory"));

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

                arr = Tool.ParseInventoryString(idb.row.Field<string>("backpack"));
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

                arr = Tool.ParseInventoryString(idb.row.Field<string>("state"));

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
        public class Deployable : PropObjectBase
        {
            public Deployable(iconDB idb)
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

                ArrayList arr = Tool.ParseInventoryString(idb.row.Field<string>("inventory"));
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
            public PointF position { get; set; }
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

                ArrayList arr = Tool.ParseInventoryString(idb.row.Field<string>("worldspace"));
                //  arr[0] = angle
                //  arr[1] = [x, y, z]
                double x = double.Parse((arr[1] as ArrayList)[0] as string, CultureInfo.InvariantCulture.NumberFormat);
                double y = double.Parse((arr[1] as ArrayList)[1] as string, CultureInfo.InvariantCulture.NumberFormat);
                this.position = new PointF((float)x, (float)y);


                arr = Tool.ParseInventoryString(idb.row.Field<string>("inventory"));
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
        public class Spawn : PropObjectBase
        {
            public Spawn(iconDB idb)
                : base(idb)
            {
                this.inventory = new Storage();
            }

            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public string classname { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public UInt64 uid { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public PointF position { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public Decimal chance { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public Storage inventory { get; set; }
            public override void Rebuild()
            {
                this.inventory.weapons.Clear();
                this.inventory.items.Clear();
                this.inventory.bags.Clear();

                this.classname = idb.row.Field<string>("class_name");
                this.uid = idb.row.Field<UInt64>("id");
                this.chance = idb.row.Field<Decimal>("chance");

                ArrayList arr = Tool.ParseInventoryString(idb.row.Field<string>("worldspace"));
                //  arr[0] = angle
                //  arr[1] = [x, y, z]
                double x = double.Parse((arr[1] as ArrayList)[0] as string, CultureInfo.InvariantCulture.NumberFormat);
                double y = double.Parse((arr[1] as ArrayList)[1] as string, CultureInfo.InvariantCulture.NumberFormat);
                this.position = new PointF((float)x, (float)y);


                arr = Tool.ParseInventoryString(idb.row.Field<string>("inventory"));
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
        public delegate void DlgUpdateIcons();
        public class DragNDrop
        {
            public void Start(List<Point> refPos)
            {
                refPositions = refPos;
                offset = Size.Empty;
                lastPositionMouse = MousePosition;
            }
            public void Start(Point refPos)
            {
                refPositions = new List<Point>();
                refPositions.Add(refPos);
                offset = Size.Empty;
                lastPositionMouse = MousePosition;
            }
            public void Update()
            {
                offset = new Size((int)(MousePosition.X - lastPositionMouse.X), (int)(MousePosition.Y - lastPositionMouse.Y));
            }
            public void Stop()
            {
                offset = Size.Empty;
            }

            public Size Offset { get { return offset; } }
            public Point Position(int idx) { return Point.Add(refPositions[idx], offset); }

            private List<Point> refPositions;
            private Size offset = Size.Empty;
            private Point lastPositionMouse;
        }
        public class myConfig
        {
            public myConfig()
            {
                cfgVersion = curCfgVersion;
                worlds_def = new DataSet();
                vehicle_types = new DataSet();
            }
            public string game_type;
            public string url;
            public string port;
            public string basename;
            public string username;
            public string password;
            public string instance_id;
            public string vehicle_limit;
            public string body_time_limit;
            public string tent_time_limit;
            public string online_time_limit;
            public ModuleVersion cfgVersion { get; set; }
            public DataSet worlds_def { get; set; }
            public DataSet vehicle_types { get; set; }
            //
            [XmlIgnore]
            public UInt16 world_id = 0;
        }
        public class myIcon
        {
            public myIcon()
            {
            }

            // Wrap event invocations inside a protected virtual method 
            // to allow derived classes to override the event invocation behavior 
            public void OnClick(Control parent, MouseEventArgs e)
            {
                if (e.Button.HasFlag(MouseButtons.Left) && (Click != null))
                {
                    Click(this, e);
                }
                else if (e.Button.HasFlag(MouseButtons.Right) && (contextMenuStrip != null))
                {
                    contextMenuStrip.Show(parent, e.Location);
                }
            }

            public Image image;
            public iconDB iconDB;
            public Rectangle rectangle;
            public Point Location { get { return rectangle.Location; } set { rectangle.Location = value; } }
            public Size Size { get { return rectangle.Size; } set { rectangle.Size = value; } }

            public event MouseEventHandler Click;
            public ContextMenuStrip contextMenuStrip;
        }
        public class UIDGraph
        {
            public static PointF InvalidPos = new PointF(float.NaN,float.NaN);

            public UIDGraph(Pen pen)
            {
                Random rnd = new Random();

                byte[] rgb = { 0, 0, 0 };
                rnd.NextBytes(rgb);

                this.pen = pen;
            }
            public void AddPoint(PointF pos)
            {
                if ((positions.Count == 0) || (pos != positions.Last()))
                    positions.Add(pos);
            }
            public void DisplayInMap(Graphics gfx, VirtualMap map)
            {
                path.Reset();

                PointF last = InvalidPos;
                foreach (PointF pt in positions)
                {
                    PointF newpt = map.VirtualPosition(pt);

                    // if a point is invalid, break the continuity
                    if ((last != InvalidPos) && (pt != InvalidPos))
                        path.AddLine(last, newpt);

                    last = newpt;
                }

                gfx.DrawPath(pen, path);
            }

            public GraphicsPath path = new GraphicsPath();
            public Pen pen;
            public List<PointF> positions = new List<PointF>();
        }
        public enum UIDType : ulong
        {
            TypePlayer = 0x100000000,
            TypeVehicle = 0x200000000,
            TypeSpawn = 0x400000000,
            TypeDeployable = 0x800000000,
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
        private class MySplitContainer : SplitContainer
        {
            public MySplitContainer()
            {
                MethodInfo mi = typeof(Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
                object[] args = new object[] { ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true };
                mi.Invoke(this.Panel1, args);
                mi.Invoke(this.Panel2, args);
            }
        }
        public class VirtualMap
        {
            public VirtualMap()
            {
                Position = Point.Empty;
                Size = new Size(400, 400);
            }

            public bool Enabled { get { return nfo.depth > 0; } }

            public Point Position;

            public Size Size 
            {
                get { return _size; }
                set { _size = value; UpdateData(); }
            }

            public BitmapNfo nfo = new BitmapNfo();

            public Size SizeCorrected { get { return _sizeCorrected; } }
            public Size DefTileSize { get { return nfo.defTileSize; } }
            public Size TileCount { get { return _tileCount; } }
            public Size TileSize { get { return _tileSize; } }
            public int Depth { get { return _depth; } }

            public Rectangle TileRectangle(int x, int y)
            {
                return new Rectangle(Point.Add(Position, new Size(x * _tileSize.Width, y * _tileSize.Height)), _tileSize);
            }
            public PointF VirtualPosition(PointF from)
            {
                return new PointF(Position.X + from.X * SizeCorrected.Width, 
                                  Position.Y + from.Y * SizeCorrected.Height);
            }
            public Point VirtualPosition(iconDB from)
            {
                float ratioX = (nfo.dbMapSize.Width / nfo.dbRefMapSize.Width);
                float ratioY = (nfo.dbMapSize.Height / nfo.dbRefMapSize.Height);
                float unitPosX = from.pos.X * ratioX + nfo.dbMapOffsetUnit.X;
                float unitPosY = from.pos.Y * ratioY + nfo.dbMapOffsetUnit.Y;

                float x = Position.X + unitPosX * SizeCorrected.Width - from.icon.Size.Width * 0.5f;
                float y = Position.Y + unitPosY * SizeCorrected.Height - from.icon.Size.Height * 0.5f;
                
                return new Point((int)x, (int)y);
            }

            public double ResizeFromZoom(double zoom)
            {
                int max_sizeX = /*2**/nfo.defTileSize.Width << (nfo.depth - 1);
                int max_sizeY = /*2**/nfo.defTileSize.Height << (nfo.depth - 1);

                Size temp = new Size((int)(nfo.defTileSize.Width * zoom),
                                     (int)(nfo.defTileSize.Height * zoom));

                Size = new Size(Math.Max(nfo.defTileSize.Width, Math.Min(max_sizeX, temp.Width)),
                                Math.Max(nfo.defTileSize.Height, Math.Min(max_sizeY, temp.Height)));

                return Size.Width / (double)nfo.defTileSize.Width;
            }

            public void SetRatio(SizeF ratio) { _ratio = ratio; }

            //
            //
            //
            private int _depth;
            private Size _size;
            private Size _sizeCorrected;
            private Size _tileCount;
            private Size _tileSize;
            private SizeF _ratio;

            private void UpdateData()
            {
                int reqTileCountX = Tool.NextPowerOf2((_size.Width / (float)nfo.defTileSize.Width));
                int reqTileCountY = Tool.NextPowerOf2((_size.Width / (float)nfo.defTileSize.Width));

                _depth = (int)Math.Log(Math.Max(reqTileCountX, reqTileCountY), 2);

                // Clamp to max depth
                _depth = Math.Min(_depth, nfo.depth-1);

                // Clamp tile count
                _tileCount = new Size(Math.Min(reqTileCountX, 1<<_depth),
                                      Math.Min(reqTileCountY, 1<<_depth));

                _tileSize = Size.Ceiling(new SizeF(_size.Width / (float)_tileCount.Width, _size.Height / (float)_tileCount.Height));

                // and re-adjust size from new tile size
                _size = new Size(_tileSize.Width * _tileCount.Width, _tileSize.Height * _tileCount.Height);
                _sizeCorrected = Size.Ceiling(new SizeF(_size.Width * _ratio.Width, _size.Height * _ratio.Height));
            }

            public class BitmapNfo
            {
                public string tileBasePath = "";
                public int depth = 0;
                public Size defTileSize = new Size(1,1);
                public SizeF dbMapSize = new SizeF(1, 1);
                public PointF dbMapOffsetUnit = PointF.Empty;
                public SizeF dbRefMapSize = new SizeF(1, 1);
           }
        }
        public class MapHelper
        {
            public PointF[] defBoundaries = new PointF[2];
            public PointF[] boundaries = new PointF[2];
            public PointF[] controls = new PointF[4];
            public int isDraggingCtrlPoint;
            public bool enabled;

            public MapHelper(VirtualMap map)
            {
                this.map = map;

                PathDef def;

                //  DB NEAF
                def = new PathDef();
                paths.Add(def);
                def.points.Add(new PointF(11777, 12848));
                def.points.Add(new PointF(11767, 12823));
                def.points.Add(new PointF(12470, 12566));
                def.points.Add(new PointF(12480, 12593));
                def.points.Add(new PointF(11777, 12848));
                //  DB NWAF
                def = new PathDef();
                paths.Add(def);
                def.points.Add(new PointF(5055, 9732));
                def.points.Add(new PointF(4778, 9572));
                def.points.Add(new PointF(4057, 10820));
                def.points.Add(new PointF(4335, 10979));
                def.points.Add(new PointF(5055, 9732));
                //  DB SWAF
                def = new PathDef();
                paths.Add(def);
                def.points.Add(new PointF(4617, 2583));
                def.points.Add(new PointF(4605, 2565));
                def.points.Add(new PointF(5230, 2203));
                def.points.Add(new PointF(5241, 2222));
                def.points.Add(new PointF(4617, 2583));

                PointF min = new PointF(9999999, 9999999);
                PointF max = new PointF(-9999999, -9999999);
                foreach (PathDef _def in paths)
                {
                    foreach (PointF pt in _def.points)
                    {
                        min.X = Math.Min(min.X, pt.X);
                        min.Y = Math.Min(min.Y, pt.Y);
                        max.X = Math.Max(max.X, pt.X);
                        max.Y = Math.Max(max.Y, pt.Y);
                    }
                }
                SizeF size = new SizeF(max.X - min.X, max.Y - min.Y);

                //  DB Map boundaries
                def = new PathDef();
                paths.Add(def);
                def.points.Add(new PointF(0, 15360));
                def.points.Add(new PointF(15360, 15360));
                def.points.Add(new PointF(15360, 0));
                def.points.Add(new PointF(0, 0));
                def.points.Add(new PointF(0, 15360));

                foreach (PathDef _def in paths)
                {
                    for (int i = 0; i < _def.points.Count; i++)
                    {
                        PointF pt = _def.points[i];

                        pt.X = (pt.X - min.X) / size.Width;
                        pt.Y = (pt.Y - min.Y) / size.Height;
                        _def.points[i] = pt;
                    }
                }

                defBoundaries[0] = new PointF(def.points[0].X, def.points[0].Y);
                defBoundaries[1] = new PointF(def.points[2].X, def.points[2].Y);

                //  DB bounding box
                def = new PathDef();
                paths.Add(def);
                def.points.Add(new PointF(0, 0));
                def.points.Add(new PointF(0, 1));
                def.points.Add(new PointF(1, 1));
                def.points.Add(new PointF(1, 0));
                def.points.Add(new PointF(0, 0));

                // DB map boundaries
                boundaries[0] = map.nfo.dbMapOffsetUnit;
                boundaries[1] = PointF.Add(boundaries[0], new SizeF(map.nfo.dbMapSize.Width / map.nfo.dbRefMapSize.Width,
                                                                    map.nfo.dbMapSize.Height / map.nfo.dbRefMapSize.Height));
                // Control points
                SizeF Csize = new SizeF((boundaries[1].X - boundaries[0].X) / (defBoundaries[1].X - defBoundaries[0].X),
                                        (boundaries[1].Y - boundaries[0].Y) / (defBoundaries[1].Y - defBoundaries[0].Y));

                controls[0] = new PointF(boundaries[0].X - defBoundaries[0].X * Csize.Width,
                                         boundaries[0].Y - defBoundaries[0].Y * Csize.Height);
                controls[1] = new PointF(controls[0].X + Csize.Width,
                                         controls[0].Y + Csize.Height);
                controls[2] = new PointF(controls[1].X, controls[0].Y);
                controls[3] = new PointF(controls[0].X, controls[1].Y);
            }
            public int IntersectControl(PointF pos, float radius)
            {
                SizeF delta = new SizeF();
                for (int i = 0; i < 4; i++)
                {
                    PointF posInMap = new PointF(controls[i].X * map.SizeCorrected.Width,
                                                 controls[i].Y * map.SizeCorrected.Height);

                    delta.Width = (posInMap.X - pos.X);
                    delta.Height = (posInMap.Y - pos.Y);

                    float distance = (float)Math.Sqrt(delta.Width*delta.Width + delta.Height*delta.Height);

                    if (distance <= radius)
                        return i;
                }

                return -1;
            }
            public void Display(Graphics gfx)
            {
                PointF offset = new PointF(controls[0].X * map.SizeCorrected.Width + map.Position.X,
                                           controls[0].Y * map.SizeCorrected.Height + map.Position.Y);

                SizeF size = SizeF.Subtract(new SizeF(controls[1]), new SizeF(controls[0]));
                size.Width *= map.SizeCorrected.Width;
                size.Height *= map.SizeCorrected.Height;

                foreach (PathDef def in paths)
                {
                    def.path.Reset();

                    PointF last = new PointF(offset.X + (def.points[0].X * size.Width),
                                             offset.Y + (def.points[0].Y * size.Height));
                    for (int i = 1; i < def.points.Count; i++)
                    {
                        PointF pt = def.points[i];
                        PointF newpt = new PointF(offset.X + (pt.X * size.Width),
                                                  offset.Y + (pt.Y * size.Height));

                        // if a point is invalid, break the continuity
                        def.path.AddLine(last, newpt);
                        last = newpt;
                    }

                    gfx.DrawPath(pen, def.path);
                }

                int j = 0;
                foreach (PointF point in controls)
                {
                    Point pt = Point.Truncate(new PointF(point.X * map.SizeCorrected.Width + map.Position.X,
                                                         point.Y * map.SizeCorrected.Height + map.Position.Y));

                    Brush brush = (isDraggingCtrlPoint == j) ? brushSelected : brushUnselected;
                    gfx.FillEllipse(brush, new Rectangle(pt.X - 5, pt.Y - 5, 11, 11));
                    j++;
                }

                j = 0;
                foreach (PointF point in boundaries)
                {
                    Point pt = Point.Truncate(new PointF(point.X * map.SizeCorrected.Width + map.Position.X,
                                                         point.Y * map.SizeCorrected.Height + map.Position.Y));

                    gfx.FillEllipse(brushSelected, new Rectangle(pt.X - 8, pt.Y - 8, 17, 17));
                    j++;
                }
            }
            public void ControlPointUpdated(int idx)
            {
                if (idx < 2)
                {
                    // Update 2 & 3
                    controls[2] = new PointF(controls[1].X, controls[0].Y);
                    controls[3] = new PointF(controls[0].X, controls[1].Y);
                }
                else
                {
                    // Update 1 & 2
                    controls[0] = new PointF(controls[3].X, controls[2].Y);
                    controls[1] = new PointF(controls[2].X, controls[3].Y);
                }

                SizeF size = new SizeF(controls[1].X - controls[0].X,
                                       controls[1].Y - controls[0].Y);

                boundaries[0] = new PointF(controls[0].X + defBoundaries[0].X * size.Width,
                                           controls[0].Y + defBoundaries[0].Y * size.Height);
                boundaries[1] = new PointF(controls[0].X + defBoundaries[1].X * size.Width,
                                           controls[0].Y + defBoundaries[1].Y * size.Height);
            }

            private class PathDef
            {
                public GraphicsPath path = new GraphicsPath();
                public List<PointF> points = new List<PointF>();
            }
            private Pen pen = new Pen(Color.Red, 1.5f);
            private SolidBrush brushUnselected = new SolidBrush(Color.Red);
            private SolidBrush brushSelected = new SolidBrush(Color.Green);
            private List<PathDef> paths = new List<PathDef>();
            private VirtualMap map;
        }
        private void comboBoxGameType_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            mycfg.game_type = cb.Items[cb.SelectedIndex] as string;
            textBoxInstanceId.Enabled = (!bConnected && (cb.SelectedIndex == 0));
        }

        private void checkBoxMapHelper_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            if (mapHelper == null)
                return;

            mapHelper.enabled = cb.Checked;
            
            if (!cb.Checked)
            {
                // Apply map helper's new size
                DataRow row = mycfg.worlds_def.Tables[0].Rows.Find(mycfg.world_id);

                UInt32 refWidth = row.Field<UInt32>("DB_refWidth");
                UInt32 refHeight = row.Field<UInt32>("DB_refHeight");

                PointF offUnit = mapHelper.boundaries[0];
                SizeF sizeUnit = new SizeF(mapHelper.boundaries[1].X - mapHelper.boundaries[0].X,
                                           mapHelper.boundaries[1].Y - mapHelper.boundaries[0].Y);

                int offsetX = (int)(offUnit.X * refWidth);
                int offsetY = (int)(offUnit.Y * refWidth);
                UInt32 width = (UInt32)(sizeUnit.Width * refWidth);
                UInt32 height = (UInt32)(sizeUnit.Height * refWidth);

                virtualMap.nfo.dbMapOffsetUnit = new PointF(offsetX / (float)refWidth, offsetY / (float)refHeight);
                virtualMap.nfo.dbMapSize = new SizeF(width, height);
                virtualMap.nfo.dbRefMapSize = new SizeF(refWidth, refHeight);

                row.SetField<int>("DB_X", offsetX);
                row.SetField<int>("DB_Y", offsetY);
                row.SetField<UInt32>("DB_Width", width);
                row.SetField<UInt32>("DB_Height", height);
            }

            splitContainer1.Panel1.Invalidate();
        }
    }
    internal class Tool
    {
        public static int NextPowerOf2(int v)
        {
            int r = 0;
            while (v > (1 << r))
                r++;
            return (1 << r);
        }
        public static int NextPowerOf2(float f)
        {
            int r = 0;
            while (f > (float)(1 << r))
                r++;
            return (1 << r);
        }
        public static ArrayList ParseInventoryString(string str)
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
                        if (curr != null) stack.Push(curr);
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
        public static bool NullOrEmpty(string str)
        {
            return ((str == null) || (str == ""));
        }
        public static void SaveJpeg(string path, Bitmap img, long quality)
        {
            // Encoder parameter for image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // Jpeg image codec
            ImageCodecInfo jpegCodec = getEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);
        }
        public static Bitmap ResizeImage(Bitmap imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.Bilinear/*NearestNeighbor*/;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return b;
        }
        public static Bitmap CropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpCrop = img.Clone(cropArea, img.PixelFormat);
            return bmpCrop;
        }
        public static Bitmap IncreaseImageSize(Bitmap imgToResize, Size newSize)
        {
            Bitmap b = new Bitmap(newSize.Width, newSize.Height);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            g.Clear(Color.White);
            g.DrawImage(imgToResize, 0, 0, imgToResize.Width, imgToResize.Height);
            g.Dispose();

            return b;
        }
        public static Tuple<Size, Size, Size> CreateTiles(string filepath, string basepath, int limit)
        {
            Bitmap input = new Bitmap(filepath);

            Size inSize = input.Size;
            Size sqSize = new Size(Tool.NextPowerOf2(input.Width), Tool.NextPowerOf2(input.Height));

            Bitmap sqInput = IncreaseImageSize(input, sqSize);

            input.Dispose();

            double iMax = 1.0 / (float)Math.Min(sqSize.Width, sqSize.Height);

            Size limits = new Size((int)(sqSize.Width * limit * iMax), (int)(sqSize.Height * limit * iMax));
            RecursCreateTiles(Point.Empty, Size.Empty, basepath, "Tile", sqInput, limits, 0);
            sqInput.Dispose();

            return new Tuple<Size, Size, Size>(inSize, sqSize, limits);
        }
        private static void RecursCreateTiles(Point father, Size child, string basepath, string name, Bitmap input, Size limits, int recCnt)
        {
            Point pos = Point.Add(father, child);

            if (Directory.Exists(basepath + recCnt) == false)
                Directory.CreateDirectory(basepath + recCnt);

            Bitmap resized = ResizeImage(input, limits);
            //  TEST
            bool bReject = true;
            {
                Bitmap b = new Bitmap(4, 4, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = InterpolationMode.Bilinear;
                g.DrawImage(resized, 0, 0, 4, 4);
                g.Dispose();
                BitmapData data = b.LockBits(new Rectangle(0, 0, 4, 4), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                IntPtr ptr = data.Scan0;
                int numBytes = (data.Stride * b.Height);
                byte[] rgbValues = new byte[numBytes];
                Marshal.Copy(ptr, rgbValues, 0, numBytes);

                for (int i = 0; i < numBytes; i++)
                    if( rgbValues[i] != 255 )
                        bReject = false;

                // Unlock the bits.
                b.UnlockBits(data);

            }
            if (!bReject)
            {
                SaveJpeg(basepath + recCnt + "\\" + name + pos.Y.ToString("000") + pos.X.ToString("000") + ".jpg", resized, 90);
                resized.Dispose();

                bool bSplitH = (input.Width > limits.Width);
                bool bSplitV = (input.Height > limits.Height);

                if (bSplitH || bSplitV)
                {
                    recCnt++;
                    Size cropSize = new Size(Math.Max(input.Width / 2, limits.Width), Math.Max(input.Height / 2, limits.Height));

                    father = new Point(pos.X * 2, pos.Y * 2);

                    RecursCreateTiles(father, new Size(0, 0), basepath, name, CropImage(input, new Rectangle(new Point(0, 0), cropSize)), limits, recCnt);

                    if (bSplitH)
                        RecursCreateTiles(father, new Size(1, 0), basepath, name, CropImage(input, new Rectangle(new Point(cropSize.Width, 0), cropSize)), limits, recCnt);

                    if (bSplitV)
                        RecursCreateTiles(father, new Size(0, 1), basepath, name, CropImage(input, new Rectangle(new Point(0, cropSize.Height), cropSize)), limits, recCnt);

                    if (bSplitH && bSplitV)
                        RecursCreateTiles(father, new Size(1, 1), basepath, name, CropImage(input, new Rectangle(new Point(cropSize.Width, cropSize.Height), cropSize)), limits, recCnt);
                }
            }
            else
            {
                resized.Dispose();
            }

            input.Dispose();
        }
        private static ImageCodecInfo getEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }
    }
    [Serializable]
    public class ModuleVersion : ICloneable, IComparable
    {
        private int major;
        private int minor;
        public int Major
        {
            get
            {
                return major;
            }
            set
            {
                major = value;
            }
        }
        public int Minor
        {
            get
            {
                return minor;
            }
            set
            {
                minor = value;
            }
        }
        public ModuleVersion()
        {
            this.major = 0;
            this.minor = 0;
        }
        public ModuleVersion(int major, int minor)
        {
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", "ArgumentOutOfRange_Version");
            }
            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", "ArgumentOutOfRange_Version");
            }
            this.major = major;
            this.minor = minor;
        }
        #region ICloneable Members
        public object Clone()
        {
            ModuleVersion version1 = new ModuleVersion();
            version1.major = this.major;
            version1.minor = this.minor;
            return version1;
        }
        #endregion
        #region IComparable Members
        public int CompareTo(object version)
        {
            if (version == null)
            {
                return 1;
            }
            if (!(version is ModuleVersion))
            {
                throw new ArgumentException("Arg_MustBeVersion");
            }
            ModuleVersion version1 = (ModuleVersion)version;
            if (this.major != version1.Major)
            {
                if (this.major > version1.Major)
                {
                    return 1;
                }
                return -1;
            }
            if (this.minor != version1.Minor)
            {
                if (this.minor > version1.Minor)
                {
                    return 1;
                }
                return -1;
            }
            return 0;
        }
        #endregion
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is ModuleVersion))
            {
                return false;
            }
            ModuleVersion version1 = (ModuleVersion)obj;
            if ((this.major == version1.Major) && (this.minor == version1.Minor))
            {
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            int num1 = 0;
            num1 |= ((this.major & 15) << 0x1c);
            num1 |= ((this.minor & 0xff) << 20);
            return num1;
        }
        public static bool operator ==(ModuleVersion v1, ModuleVersion v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator >(ModuleVersion v1, ModuleVersion v2)
        {
            return (v2 < v1);
        }
        public static bool operator >=(ModuleVersion v1, ModuleVersion v2)
        {
            return (v2 <= v1);
        }
        public static bool operator !=(ModuleVersion v1, ModuleVersion v2)
        {
            return (v1 != v2);
        }
        public static bool operator <(ModuleVersion v1, ModuleVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) < 0);
        }
        public static bool operator <=(ModuleVersion v1, ModuleVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) <= 0);
        }
    }
}
