using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
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
        private static Mutex mtxUpdateDB = new Mutex();
        private static Mutex mtxUseDS = new Mutex();
        public DlgUpdateIcons dlgUpdateIcons;
        public myConfig mycfg = new myConfig();
        private System.Drawing.Bitmap bitmapHQ;
        //private System.Drawing.Bitmap bitmapLQ;
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

        public Form1()
        {
            InitializeComponent();

            Assembly asb = System.Reflection.Assembly.GetExecutingAssembly();
            this.Text = asb.GetName().Name + " - v" + asb.GetName().Version.ToString();

            this.Resize += Form1Resize;
            this.MouseWheel += imgMap_MouseWheel;

            //
            LoadConfigFile();

            bgWorker.RunWorkerAsync();

            Enable(false);

            if( imgMap.Image != null )
                zoomFactor = MapSize.Width / (float)imgMap.Image.Size.Width;

            Size sizePanel = splitContainer1.Panel1.Size;
            Point halfPanel = new Point((int)(sizePanel.Width * 0.5f), (int)(sizePanel.Height * 0.5f));
            MapPos.X = (int)(halfPanel.X - MapSize.Width * 0.5f);
            MapPos.Y = (int)(halfPanel.Y - MapSize.Height * 0.5f);
            ApplyMapChanges();
        }

        void ApplyMapChanges()
        {
            if (imgMap.Image == null)
                return;

            imgMap.SuspendLayout();

            Size sizePanel = splitContainer1.Panel1.Size;

            Point halfPanel = new Point((int)(sizePanel.Width * 0.5f), (int)(sizePanel.Height * 0.5f));

            MapPos.X = Math.Min(halfPanel.X - 16, MapPos.X);
            MapPos.Y = Math.Min(halfPanel.Y - 16, MapPos.Y);

            MapPos.X = Math.Max(halfPanel.X - MapSize.Width + 16, MapPos.X);
            MapPos.Y = Math.Max(halfPanel.Y - MapSize.Height + 16, MapPos.Y);

            imgMap.Size = MapSize;
            imgMap.Location = MapPos;
            
            RefreshIcons();
            
            imgMap.ResumeLayout();
        }

        void Form1Resize(object sender, EventArgs e)
        {
            ApplyMapChanges();
        }

        private void imgMap_MouseClick(object sender, MouseEventArgs e)
        {
            lastPositionMouse = MousePosition;
        }

        private void imgMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left))
            {
                offsetMap = new Size((int)(MousePosition.X - lastPositionMouse.X), (int)(MousePosition.Y - lastPositionMouse.Y));

                this.imgMap.SuspendLayout();
                MapPos = Point.Add(MapPosTmp, offsetMap);
                ApplyMapChanges();
                this.imgMap.ResumeLayout();
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

            float newZoom = zoomFactor * (1.0f + ((e.Delta / 120.0f) * 0.1f));
            //
            MapSize = new Size((int)(imgMap.Image.Size.Width * newZoom), (int)(imgMap.Image.Size.Height * newZoom));
            MapSize = new Size(Math.Max(400, Math.Min(16383, MapSize.Width)),
                                Math.Max(400, Math.Min(16383, MapSize.Height)));
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

                _DoWork();

                textBoxStatus.Text = "connected";
            }
            catch(Exception)
            {
                textBoxStatus.Text = "Error !";
                Enable(false);
            }

            this.Cursor = Cursors.Arrow;

            mtxUpdateDB.ReleaseMutex();
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            CloseConnection();
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
                this.imgMap.SuspendLayout();
                foreach (iconDB icon in listIcons)
                    imgMap.Controls.Remove(icon.icon);

                listIcons.Clear();
                this.imgMap.ResumeLayout();

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

            RadioButton senderRB = sender as RadioButton;
            if (senderRB.Checked)
            {
                propertyGrid1.SelectedObject = null;

                currDisplayedItems = senderRB;

                BuildIcons();
            }
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
                }
            }
            catch (Exception)
            {
            }
            mtxUseDS.ReleaseMutex();

            if (currDisplayedItems == null)
                return;

            mtxUseDS.WaitOne();

            this.imgMap.SuspendLayout();

            try
            {
                foreach (iconDB icon in listIcons)
                    imgMap.Controls.Remove(icon.icon);

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
            catch (Exception)
            {
                textBoxStatus.Text = "Error !";
            }

            this.imgMap.ResumeLayout();

            mtxUseDS.ReleaseMutex();
        }

        private void OnPlayerClick(object sender, EventArgs e)
        {
            mtxUseDS.WaitOne();

            try
            {
                PictureBox pb = sender as PictureBox;

                iconDB idb = pb.Tag as iconDB;

                Survivor player = new Survivor();

                {
                    ArrayList arr = ParseInventoryString(idb.row.Field<string>("medical"));

                    player.blood = (int) float.Parse(arr[7] as string);
                }

                {
                    Dictionary<string, int> dicInventory = new Dictionary<string,int>();

                    ArrayList arr = ParseInventoryString(idb.row.Field<string>("inventory"));

                    if (arr.Count > 0)
                    {
                        // arr[0] = tools
                        // arr[1] = items

                        foreach (string o in arr[0] as ArrayList)
                        {
                            player.tools.Add(new Entry(o, 1));
                        }

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
                            player.inventory.Add(new Entry(pair.Key, pair.Value));
                    }
                }

                {
                    ArrayList arr = ParseInventoryString(idb.row.Field<string>("backpack"));
                    // arr[0] = backpack's class
                    // arr[1] = weapons
                    // arr[2] = items

                    if (arr.Count > 0)
                    {
                        player.backpackclass = arr[0] as string;

                        {
                            ArrayList aWeapons = arr[1] as ArrayList;
                            ArrayList aTypes = aWeapons[0] as ArrayList;
                            ArrayList aCount = aWeapons[1] as ArrayList;
                            for (int i = 0; i < aTypes.Count; i++)
                                player.backpack.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                        }

                        {
                            ArrayList aItems = arr[2] as ArrayList;
                            ArrayList aTypes = aItems[0] as ArrayList;
                            ArrayList aCount = aItems[1] as ArrayList;
                            for (int i = 0; i < aTypes.Count; i++)
                                player.backpack.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                        }
                    }
                }

                {
                    ArrayList arr = ParseInventoryString(idb.row.Field<string>("state"));

                    if (arr.Count > 0)
                    {
                        player.weapon = arr[0] as string;
                        if (player.weapon == "")
                            player.weapon = "none";
                    }
                }

                player.name = idb.row.Field<string>("name");
                player.uid = UInt64.Parse(idb.row.Field<string>("unique_id"));
                player.humanity = idb.row.Field<int>("humanity");

                propertyGrid1.SelectedObject = player;
            }
            catch (Exception)
            {
            }

            mtxUseDS.ReleaseMutex();
        }

        private void OnVehicleClick(object sender, EventArgs e)
        {
            mtxUseDS.WaitOne();

            try
            {
                PictureBox pb = sender as PictureBox;

                iconDB idb = pb.Tag as iconDB;

                Vehicle vehicle = new Vehicle();

                vehicle.uid = idb.row.Field<UInt64>("id");
                vehicle.fuel = idb.row.Field<double>("fuel");
                vehicle.damage = idb.row.Field<double>("damage");
                vehicle.classname = idb.row.Field<string>("class_name");
                {
                    ArrayList arr = ParseInventoryString(idb.row.Field<string>("inventory"));

                    if (arr.Count > 0)
                    {
                        // arr[0] = weapons
                        // arr[1] = items
                        // arr[2] = bags
                        {
                            ArrayList aWeapons = arr[0] as ArrayList;
                            ArrayList aTypes = aWeapons[0] as ArrayList;
                            ArrayList aCount = aWeapons[1] as ArrayList;
                            for (int i = 0; i < aTypes.Count; i++)
                                vehicle.inventory.weapons.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                        }
                        {
                            ArrayList aItems = arr[1] as ArrayList;
                            ArrayList aTypes = aItems[0] as ArrayList;
                            ArrayList aCount = aItems[1] as ArrayList;
                            for (int i = 0; i < aTypes.Count; i++)
                                vehicle.inventory.items.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                        }
                        {
                            ArrayList aBags = arr[2] as ArrayList;
                            ArrayList aTypes = aBags[0] as ArrayList;
                            ArrayList aCount = aBags[1] as ArrayList;
                            for (int i = 0; i < aTypes.Count; i++)
                                vehicle.inventory.bags.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                        }
                    }
                }

                propertyGrid1.SelectedObject = vehicle;
            }
            catch (Exception)
            {
            }

            mtxUseDS.ReleaseMutex();
        }

        private void OnTentClick(object sender, EventArgs e)
        {
            mtxUseDS.WaitOne();

            try
            {
                PictureBox pb = sender as PictureBox;

                iconDB idb = pb.Tag as iconDB;

                Tent tent = new Tent();

                tent.owner = "who knows...";

                {
                    ArrayList arr = ParseInventoryString(idb.row.Field<string>("inventory"));

                    if (arr.Count > 0)
                    {
                        // arr[0] = weapons
                        // arr[1] = items
                        // arr[2] = bags
                        {
                            ArrayList aWeapons = arr[0] as ArrayList;
                            ArrayList aTypes = aWeapons[0] as ArrayList;
                            ArrayList aCount = aWeapons[1] as ArrayList;
                            for (int i = 0; i < aTypes.Count; i++)
                                tent.inventory.weapons.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                        }
                        {
                            ArrayList aItems = arr[1] as ArrayList;
                            ArrayList aTypes = aItems[0] as ArrayList;
                            ArrayList aCount = aItems[1] as ArrayList;
                            for (int i = 0; i < aTypes.Count; i++)
                                tent.inventory.items.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                        }
                        {
                            ArrayList aBags = arr[2] as ArrayList;
                            ArrayList aTypes = aBags[0] as ArrayList;
                            ArrayList aCount = aBags[1] as ArrayList;
                            for (int i = 0; i < aTypes.Count; i++)
                                tent.inventory.bags.Add(new Entry(aTypes[i] as string, int.Parse(aCount[i] as string)));
                        }
                    }
                }

                propertyGrid1.SelectedObject = tent;
            }
            catch (Exception)
            {
            }

            mtxUseDS.ReleaseMutex();
        }

        private void RefreshIcons()
        {
            if (currDisplayedItems == null)
                return;

            mtxUseDS.WaitOne(100);

            foreach (iconDB idb in listIcons)
                idb.icon.Location = GetMapPosFromIcon(idb);

            mtxUseDS.ReleaseMutex();
        }

        private void BuildOnlineIcons()
        {
            foreach (DataRow row in dsOnlinePlayers.Tables[0].Rows)
            {
                iconDB idb = new iconDB();

                idb.row = row;

                idb.pos = GetMapPosFromString(row.Field<string>("worldspace"));

                idb.icon = new PictureBox();
                idb.icon.Image = global::DBAccess.Properties.Resources.Online;
                idb.icon.Size = new System.Drawing.Size(16, 16);
                idb.icon.Tag = idb;
                idb.icon.BackColor = Color.Transparent;
                idb.icon.Click += OnPlayerClick;

                toolTip1.SetToolTip(idb.icon, row.Field<string>("name"));

                imgMap.Controls.Add(idb.icon);

                listIcons.Add(idb);
            }
        }

        private void BuildAliveIcons()
        {
            foreach (DataRow row in dsAlivePlayers.Tables[0].Rows)
            {
                iconDB idb = new iconDB();

                idb.row = row;

                idb.pos = GetMapPosFromString(row.Field<string>("worldspace"));

                idb.icon = new PictureBox();

                idb.icon.Image = global::DBAccess.Properties.Resources.Alive;
                idb.icon.Size = new System.Drawing.Size(16, 16);
                idb.icon.Tag = idb;
                idb.icon.BackColor = Color.Transparent;
                idb.icon.Click += OnPlayerClick;

                toolTip1.SetToolTip(idb.icon, row.Field<string>("name"));
                
                imgMap.Controls.Add(idb.icon);

                listIcons.Add(idb);
            }
        }

        private void BuildVehicleIcons()
        {
            foreach (DataRow row in dsVehicles.Tables[0].Rows)
            {
                double damage = row.Field<double>("damage");

                iconDB idb = new iconDB();

                idb.row = row;

                idb.pos = GetMapPosFromString(row.Field<string>("worldspace"));

                idb.icon = new PictureBox();
                idb.icon.Image = (damage < 1.0f) ? global::DBAccess.Properties.Resources.Vehicle : global::DBAccess.Properties.Resources.Destroyed;
                idb.icon.Size = new System.Drawing.Size(16, 16);
                idb.icon.Tag = idb;
                idb.icon.BackColor = Color.Transparent;
                idb.icon.Click += OnVehicleClick;

                toolTip1.SetToolTip(idb.icon, row.Field<UInt64>("id").ToString() + ": "+ row.Field<string>("class_name"));

                imgMap.Controls.Add(idb.icon);

                listIcons.Add(idb);
            }
        }

        private void BuildSpawnIcons()
        {
            foreach (DataRow row in dsVehicleSpawnPoints.Tables[0].Rows)
            {
                iconDB idb = new iconDB();

                idb.row = row;

                idb.pos = GetMapPosFromString(row.Field<string>("worldspace"));

                idb.icon = new PictureBox();
                idb.icon.Image = global::DBAccess.Properties.Resources.Spawn;
                idb.icon.BackColor = Color.Transparent;
                idb.icon.Tag = idb;
                idb.icon.Size = new System.Drawing.Size(16, 16);

                toolTip1.SetToolTip(idb.icon, row.Field<UInt64>("id").ToString() + ": " + row.Field<string>("class_name"));

                imgMap.Controls.Add(idb.icon);

                listIcons.Add(idb);
            }
        }

        private void BuildTentIcons()
        {
            foreach (DataRow row in dsTents.Tables[0].Rows)
            {
                iconDB idb = new iconDB();

                idb.row = row;

                idb.icon = new PictureBox();

                idb.icon.Image = global::DBAccess.Properties.Resources.Tent;
                idb.icon.Size = new System.Drawing.Size(16, 16);
                idb.icon.Tag = idb;
                idb.icon.BackColor = Color.Transparent;
                idb.icon.Click += OnTentClick;

                idb.pos = GetMapPosFromString(row.Field<string>("worldspace"));

                imgMap.Controls.Add(idb.icon);

                listIcons.Add(idb);
            }
        }

        private void Enable(bool bState)
        {
            bConnected = bState;

            this.SuspendLayout();
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
            this.ResumeLayout();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseConnection();

            //
            SaveConfigFile();
        }

        private void LoadConfigFile()
        {
            // Read file if exist
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(myConfig));
                using (StreamReader re = new StreamReader("config.xml"))
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
            if (mycfg.map_path_HQ == null) mycfg.map_path_HQ = "Celle_HQ.jpg";
            //if (mycfg.map_path_LQ == null) mycfg.map_path_LQ = "Celle_LQ.jpg";
            if (mycfg.db_from == Point.Empty) mycfg.db_from = new Point(0, 0);
            if (mycfg.db_to == Point.Empty) mycfg.db_to = new Point(12288, 12288);

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

                bitmapHQ = new Bitmap(mycfg.map_path_HQ);
                imgMap.Image = bitmapHQ;
                //bitmapLQ = new Bitmap(mycfg.map_path_LQ);
            }
            catch (Exception)
            {
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
                using (StreamWriter wr = new StreamWriter("config.xml"))
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
            // [angle, [X, Y, Z]]
            string posStr = from;

            posStr = posStr.Replace('[', ' ');
            posStr = posStr.Replace(']', ' ');
            string[] raw = posStr.Split(',');

            float x = 0;
            float y = 0;

            if (raw.Count() >= 2)
            {
                x = float.Parse(raw[1]);
                y = float.Parse(raw[2]);
            }

            x = (x - mycfg.db_from.X) / (mycfg.db_to.X - mycfg.db_from.X);
            y = 1.0f - ((y - mycfg.db_from.Y) / (mycfg.db_to.Y - mycfg.db_from.Y));

            return new PointF(x, y);
        }

        private Point GetMapPosFromIcon(iconDB from)
        {
            float x = from.pos.X * MapSize.Width - 8;
            float y = from.pos.Y * MapSize.Height - 8;

            return new Point((int)x, (int)y);
        }

        private void _DoWork()
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
                                    + " FROM survivor as s, profile as p WHERE s.unique_id=p.unique_id AND s.world_id=" + mycfg.instance_id + " AND s.is_dead=0";
                    _dsAlivePlayers.Clear();
                    adapter.Fill(_dsAlivePlayers);
                    //DataColumn[] keyAlive = new DataColumn[1];
                    //keyProfiles[0] = _dsAlivePlayers.Tables[0].Columns[1];
                    //_dsAlivePlayers.Tables[0].PrimaryKey = keyProfiles;

                    //
                    //  Players online
                    //
                    cmd.CommandText += " AND s.last_updated > now() - interval " + mycfg.online_time_limit + " minute";
                    _dsOnlinePlayers.Clear();
                    adapter.Fill(_dsOnlinePlayers);

                    //
                    //  Vehicles
                    //
                    cmd.CommandText = "SELECT iv.id id, v.class_name class_name, iv.worldspace worldspace, iv.inventory inventory,"
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
                                    + " WHERE w.world_id=" + mycfg.instance_id + " AND w.vehicle_id=v.id";
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
                    textBoxCmdStatus.Text = ex.Message;
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

                _DoWork();

                dlgUpdateIcons = this.BuildIcons;
                this.Invoke(dlgUpdateIcons);
            }
        }
        private ArrayList ParseInventoryString(string str)
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
                        curr = new System.Collections.ArrayList();
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
        private void buttonRemoveDestroyed_Click(object sender, EventArgs e)
        {
            if (bConnected)
            {
                this.Cursor = Cursors.WaitCursor;
                mtxUpdateDB.WaitOne();

                try
                {
                    MySqlCommand cmd = cnx.CreateCommand();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    cmd.CommandText = "DELETE FROM instance_vehicle WHERE instance_id=" + mycfg.instance_id + " AND damage=1";
                    int res = cmd.ExecuteNonQuery();
                    textBoxCmdStatus.Text = "removed " + res + " destroyed vehicles.";
                }
                catch (Exception ex)
                {
                    textBoxCmdStatus.Text = ex.Message;
                }

                mtxUpdateDB.ReleaseMutex();
                this.Cursor = Cursors.Arrow;
            }
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
                                    + " WHERE wv.world_id ="+mycfg.instance_id+" AND iv.id IS null AND (round(rand(), 3) < wv.chance)"
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
                    textBoxCmdStatus.Text = ex.Message;
                }

                mtxUpdateDB.ReleaseMutex();
                this.Cursor = Cursors.Arrow;
            }
        }
        internal bool NullOrEmpty(string str)
        {
            return ((str == null) || (str == ""));
        }
        private void buttonRemoveBodies_Click(object sender, EventArgs e)
        {
            if (bConnected)
            {
                this.Cursor = Cursors.WaitCursor;
                mtxUpdateDB.WaitOne();

                try
                {
                    MySqlCommand cmd = cnx.CreateCommand();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    int limit = int.Parse(textBoxOldBodyLimit.Text);

                    cmd.CommandText = "DELETE FROM survivor WHERE world_id=" + mycfg.instance_id + " AND is_dead=1 AND last_updated < now() - interval " + limit + " day";
                    int res = cmd.ExecuteNonQuery();
                    textBoxCmdStatus.Text = "removed " + res + " bodies older than " + limit + " days.";
                }
                catch (Exception ex)
                {
                    textBoxCmdStatus.Text = ex.Message;
                }

                mtxUpdateDB.ReleaseMutex();
                this.Cursor = Cursors.Arrow;
            }
        }
        private void buttonRemoveTents_Click(object sender, EventArgs e)
        {
            if (bConnected)
            {
                this.Cursor = Cursors.WaitCursor;
                mtxUpdateDB.WaitOne();

                try
                {
                    MySqlCommand cmd = cnx.CreateCommand();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    int limit = int.Parse(textBoxOldBodyLimit.Text);

                    cmd.CommandText = "DELETE FROM id using instance_deployable id inner join deployable d on id.deployable_id = d.id"
                                    + " inner join survivor s on id.owner_id = s.id and s.is_dead=1"
                                    + " WHERE id.instance_id=" + mycfg.instance_id + " AND d.class_name = 'TentStorage' AND id.last_updated < now() - interval " + limit + " day";
                    int res = cmd.ExecuteNonQuery();
                    textBoxCmdStatus.Text = "removed " + res + " tents older than " + limit + " days.";
                }
                catch (Exception ex)
                {
                    textBoxCmdStatus.Text = ex.Message;
                }

                mtxUpdateDB.ReleaseMutex();
                this.Cursor = Cursors.Arrow;
            }

        }
        //
        //
        //
        public class iconDB
        {
            public PictureBox icon;
            public DataRow row;
            public PointF pos;
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
                if (destType == typeof(string) && value is T) { return ""; }
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
        public class Survivor
        {
            public Survivor()
            {
                inventory = new Cargo();
                backpack = new Cargo();
                tools = new Cargo();
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
            public string weapon { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public Cargo inventory { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public string backpackclass { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public Cargo backpack { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public Cargo tools { get; set; }
        };
        public class Tent
        {
            public Tent()
            {
                inventory = new Storage();
            }

            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public string owner { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public Storage inventory { get; set; }
        };
        public class Vehicle
        {
            public Vehicle()
            {
                inventory = new Storage();
            }

            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public string classname { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public UInt64 uid { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public double fuel { get; set; }
            [CategoryAttribute("Info"), ReadOnlyAttribute(true)]
            public double damage { get; set; }
            [CategoryAttribute("Inventory"), ReadOnlyAttribute(true)]
            public Storage inventory { get; set; }
        };
        public delegate void DlgUpdateIcons();
        public class myConfig
        {
            public myConfig()
            {
                db_from = Point.Empty;
                db_to = Point.Empty;
            }
            public string url { get; set; }
            public string port { get; set; }
            public string basename { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string instance_id { get; set; }
            public string map_path_HQ { get; set; }
            public string map_path_LQ { get; set; }
            public Point db_from { get; set; }
            public Point db_to { get; set; }
            public string vehicle_limit { get; set; }
            public string body_time_limit { get; set; }
            public string tent_time_limit { get; set; }
            public string online_time_limit { get; set; }
        };
    }
}
