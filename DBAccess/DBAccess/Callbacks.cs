using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBAccess
{
    public partial class Form1 : Form
    {
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

                    if (!mapHelper.enabled)
                    {
                        if (checkBoxShowTrail.Checked)
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

                    if (((mapHelper == null) || !mapHelper.enabled) && cbCartographer.Checked)
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

            if ((mapHelper != null) && mapHelper.enabled)
                return;

            if (cbCartographer.Checked)
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
            }
            else
            {
                Rectangle mouseRec = new Rectangle(e.Location, Size.Empty);
                foreach (iconDB idb in listIcons)
                {
                    if (mouseRec.IntersectsWith(idb.icon.rectangle))
                    {
                        // Call Click event from icon
                        idb.icon.OnClick(this, e);

                        break;
                    }
                }
            }
        }
        private void _Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            System.Threading.Interlocked.CompareExchange(ref bUserAction, 1, 0);

            if (e.Button.HasFlag(MouseButtons.Right) && (mapHelper != null) && mapHelper.enabled)
            {
                Tool.Point mousePos = (Tool.Point)(e.Location - virtualMap.Position);

                mapHelper.isDraggingCtrlPoint = mapHelper.IntersectControl(mousePos, 5);

                if (mapHelper.isDraggingCtrlPoint > 0)
                {
                    // Will drag selected Control point
                    Tool.Point pt = mapHelper.controls[mapHelper.isDraggingCtrlPoint] * virtualMap.SizeCorrected + virtualMap.Position;
                    dragndrop.Start(pt);
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
                    dragndrop.Start(points);
                }

                splitContainer1.Panel1.Invalidate();
            }
            else
            {
                dragndrop.Start(virtualMap.Position);
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
                    dragndrop.Update();

                    if (mapHelper.isDraggingCtrlPoint >= 0)
                    {
                        Tool.Point newPos = dragndrop.Position(0);
                        Tool.Point pt = (Tool.Point)((newPos - virtualMap.Position) / virtualMap.SizeCorrected);

                        mapHelper.controls[mapHelper.isDraggingCtrlPoint] = pt;
                        mapHelper.ControlPointUpdated(mapHelper.isDraggingCtrlPoint);
                    }
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Tool.Point newPos = dragndrop.Position(i);
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
                dragndrop.Update();
                virtualMap.Position = dragndrop.Position(0);
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

                dragndrop.Stop();
            }

            if (((mapHelper == null) || !mapHelper.enabled) && cbCartographer.Checked)
            {
                string pathstr = "public static Point[] ptsXXX = new Point[]\r\n{";
                foreach (Tool.Point pt in cartographer.positions)
                {
                    Tool.Point npt = pt;
                    npt.Y = 1.0f - npt.Y;
                    npt = npt * virtualMap.nfo.dbRefMapSize;
                    pathstr += "\r\nnew Point" + npt.ToStringInt() + ",";
                }
                pathstr = pathstr.TrimEnd(',');
                pathstr += "\r\n};";
                textBoxCmdStatus.Text = pathstr;
            }

            // Database coordinates
            Tool.Point dbp = virtualMap.UnitToDB(virtualMap.PanelToUnit(e.Location));
            // Map coordinates
            Tool.Point mp = virtualMap.UnitToMap(virtualMap.PanelToUnit(e.Location));

            if ((mp.X > -100000) && (mp.X < 100000))
            {
                toolStripStatusCoordDB.Text = ((int)dbp.X).ToString() + " : " + ((int)dbp.Y).ToString();
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

                    int res = ExecuteSqlNonQuery("DELETE FROM instance_vehicle WHERE id=" + idb.uid + " AND instance_id=" + mycfg.instance_id);
                    if (res == 1)
                        textBoxCmdStatus.Text = "removed vehicle id " + idb.uid;
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

                    int res = ExecuteSqlNonQuery("DELETE FROM world_vehicle WHERE id=" + idb.uid + " AND world_id=" + mycfg.world_id);
                    if (res == 1)
                        textBoxCmdStatus.Text = "removed vehicle spawnpoint id " + idb.uid;
                }
            }
        }
        //
        //  Database's Tab
        //
        private void _buttonConnect_Click(object sender, EventArgs e)
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

            mycfg.instance_id = numericUpDownInstanceId.Text;

            sqlCnx = new MySqlConnection(strCnx);

            try
            {
                sqlCnx.Open();

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

                groupBoxCnx.Enabled = false;
                groupBoxCnx.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
                Enable(false);
            }

            toolStripStatusCnx.Text = (bConnected) ? "connected" : "disconnected";

            this.Cursor = Cursors.Arrow;

            mtxUpdateDB.ReleaseMutex();
        }
        private void _comboBoxGameType_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            mycfg.game_type = cb.Items[cb.SelectedIndex] as string;
            numericUpDownInstanceId.Enabled = (!bConnected && (cb.SelectedIndex == 0));
        }
        //
        //  Display's Tab
        //
        private void _radioButton_CheckedChanged(object sender, EventArgs e)
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
        private void _checkBoxShowTrail_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == false)
            {
                foreach (KeyValuePair<string, UIDGraph> pair in dicUIDGraph)
                    pair.Value.path.Reset();
            }
        }
        private void _cbCartographer_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == true)
                cartographer.positions.Clear();
        }
        private void _checkBoxMapHelper_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            if (mapHelper == null)
                return;

            mapHelper.enabled = cb.Checked;

            if (!cb.Checked)
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
            }

            splitContainer1.Panel1.Invalidate();
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

                foreach(DataRow row in mycfg.vehicle_types.Tables[0].Rows)
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
            saveFileDialog1.FileName = "Backup " + sqlCnx.Database + " " + s_date + ".sql";
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "SQL file|*.sql";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                BackupDatabase(saveFileDialog1.FileName);
            }
        }
        private void _buttonRemoveDestroyed_Click(object sender, EventArgs e)
        {
            int res = ExecuteSqlNonQuery("DELETE FROM instance_vehicle WHERE instance_id=" + mycfg.instance_id + " AND damage=1");

            textBoxCmdStatus.Text = "removed " + res + " destroyed vehicles.";
        }
        private void _buttonSpawnNew_Click(object sender, EventArgs e)
        {
            if (bConnected)
            {
                this.Cursor = Cursors.WaitCursor;
                mtxUpdateDB.WaitOne();

                try
                {
                    MySqlCommand cmd = sqlCnx.CreateCommand();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    cmd.CommandText = "SELECT count(*) FROM instance_vehicle WHERE instance_id =" + mycfg.instance_id;
                    object result = cmd.ExecuteScalar();
                    long vehicle_count = (long)result;

                    cmd.CommandText = "SELECT wv.id world_vehicle_id, v.id vehicle_id, wv.worldspace, v.inventory, coalesce(v.parts, '') parts, v.limit_max,"
                                    + " round(least(greatest(rand(), v.damage_min), v.damage_max), 3) damage, round(least(greatest(rand(), v.fuel_min), v.fuel_max), 3) fuel"
                                    + " FROM world_vehicle wv JOIN vehicle v ON wv.vehicle_id = v.id LEFT JOIN instance_vehicle iv ON iv.world_vehicle_id = wv.id AND iv.instance_id = " + mycfg.instance_id
                                    + " LEFT JOIN ( SELECT count(iv.id) AS count, wv.vehicle_id FROM instance_vehicle iv JOIN world_vehicle wv ON iv.world_vehicle_id = wv.id"
                                    + " WHERE instance_id =" + mycfg.instance_id + " GROUP BY wv.vehicle_id) vc ON vc.vehicle_id = v.id"
                                    + " WHERE wv.world_id =" + mycfg.world_id + " AND iv.id IS null AND (round(rand(), 3) < wv.chance)"
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
                        foreach (string part in to_parts)
                        {
                            if (rand.NextDouble() > 0.25)
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
                                    + mycfg.instance_id + ", current_timestamp())");

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
                    MessageBox.Show(ex.Message, "Exception found");
                    Enable(false);
                }

                mtxUpdateDB.ReleaseMutex();
                this.Cursor = Cursors.Arrow;
            }
        }
        private void _buttonRemoveBodies_Click(object sender, EventArgs e)
        {
            int limit = int.Parse(textBoxOldBodyLimit.Text);

            string query = "DELETE FROM survivor WHERE world_id=" + mycfg.world_id + " AND is_dead=1 AND last_updated < now() - interval " + limit + " day";
            int res = ExecuteSqlNonQuery(query);

            textBoxCmdStatus.Text = "removed " + res + " bodies older than " + limit + " days.";
        }
        private void _buttonRemoveTents_Click(object sender, EventArgs e)
        {
            int limit = int.Parse(textBoxOldTentLimit.Text);

            string query = "DELETE FROM id using instance_deployable id inner join deployable d on id.deployable_id = d.id"
                         + " inner join survivor s on id.owner_id = s.id and s.is_dead=1"
                         + " WHERE id.instance_id=" + mycfg.instance_id + " AND d.class_name = 'TentStorage' AND id.last_updated < now() - interval " + limit + " day";
            int res = ExecuteSqlNonQuery(query);
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

            mtxUpdateDB.WaitOne();
            this.Cursor = Cursors.WaitCursor;
            try
            {
                FileInfo fi = new FileInfo(fullpath);

                switch (fi.Extension.ToLowerInvariant())
                {
                    case ".sql":
                        StreamReader sr = fi.OpenText();
                        string queries = sr.ReadToEnd();

                        MySqlScript script = new MySqlScript(sqlCnx, queries);

                        script.Error += new MySqlScriptErrorEventHandler(sqlScript_Error);
                        script.ScriptCompleted += new EventHandler(sqlScript_ScriptCompleted);

                        int count = script.Execute();

                        this.textBoxCmdStatus.Text += "\r\n" + count + " statements executed.";

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
            mtxUpdateDB.ReleaseMutex();
        }
        private void sqlScript_ScriptCompleted(object sender, EventArgs e)
        {
            this.textBoxCmdStatus.Text += "\r\ndone.";
        }
        private void sqlScript_Error(Object sender, MySqlScriptErrorEventArgs args)
        {
            MessageBox.Show(args.Exception.Message, "Script error");
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

                switch (mycfg.game_type)
                {
                    case "Epoch": DBEpoch_OnRefresh(); break;
                    default: DB_OnRefresh(); break;
                }

                if (System.Threading.Interlocked.CompareExchange(ref bUserAction, 1, 0) == 0)
                {
                    dlgUpdateIcons = this.BuildIcons;
                        
                    if (bConnected)
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

            DB_OnConnection();
        }
    }
}
