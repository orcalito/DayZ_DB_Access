using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DBAccess
{
    class myDatabase
    {
        public int FilterLastUpdated;
        public int OnlineTimeLimit;

        public bool Connected { get { return bConnected;  } }
        public int InstanceId { get { return instanceId; } }
        public int WorldId { get { return worldId; } }
        public string WorldName { get { return worldName; } }
        public string GameType { get { return gameType; } }
        public DataSet WorldDefs { get { return dsWorldDefs; } }
        public DataSet VehicleTypes { get { return dsVehicleTypes; } }
        public DataSet DeployableTypes { get { return dsDeployableTypes; } }
        public DataSet Instances { get { return dsInstances; } }
        public DataSet Deployables { get { return dsDeployables; } }
        public DataSet AlivePlayers { get { return dsAlivePlayers; } }
        public DataSet OnlinePlayers { get { return dsOnlinePlayers; } }
        public DataSet Vehicles { get { return dsVehicles; } }
        public DataSet SpawnPoints { get { return dsSpawnPoints; } }

        public void UseDS(bool state)
        {
            switch (state)
            {
                case true: mtxUseDS.WaitOne(); break;
                case false: mtxUseDS.ReleaseMutex(); break;
            }
        }
        public void Connect(string server, int port, string dbname, string user, string pass, int instance_id, myConfig cfg)
        {
            mtxUpdate.WaitOne();

            //  "Server=localhost;Database=testdb;Uid=root;Pwd=pass;";
            string strCnx = "Server=" + server + ";Port=" + port + ";Database=" + dbname + ";Uid=" + user + ";Pwd=" + pass + ";";

            sqlCnx = new MySqlConnection(strCnx);

            try
            {
                sqlCnx.Open();

                this.bConnected = true;
                this.FilterLastUpdated = cfg.filter_last_updated;
                this.OnlineTimeLimit = int.Parse(cfg.online_time_limit);
                this.gameType = cfg.game_type;
                this.instanceId = instance_id;

                dsWorldDefs = cfg.worlds_def;
                dsVehicleTypes = cfg.vehicle_types;
                dsDeployableTypes = cfg.deployable_types;

                switch (GameType)
                {
                    case "Epoch": EpochDB_OnConnection(); break;
                    default: ClassicDB_OnConnection(); break;
                }

                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }
            mtxUpdate.ReleaseMutex();
        }
        public void CloseConnection()
        {
            mtxUseDS.WaitOne();

            dsOnlinePlayers.Clear();
            dsAlivePlayers.Clear();
            dsVehicles.Clear();
            dsSpawnPoints.Clear();
            dsDeployables.Clear();

            mtxUseDS.ReleaseMutex();

            mtxUpdate.WaitOne();

            if (sqlCnx != null)
                sqlCnx.Close();

            mtxUpdate.ReleaseMutex();
        }
        public void OnConnection()
        {
            switch (GameType)
            {
                case "Epoch": EpochDB_OnConnection(); break;
                default: ClassicDB_OnConnection(); break;
            }
        }
        public void Refresh()
        {
            switch (GameType)
            {
                case "Epoch": EpochDB_Refresh(); break;
                default: ClassicDB_Refresh(); break;
            }
        }
        public int ExecuteSqlNonQuery(string query)
        {
            if (!Connected)
                return 0;

            int res = 0;

            mtxUpdate.WaitOne();

            try
            {
                MySqlCommand cmd = sqlCnx.CreateCommand();

                cmd.CommandText = query;

                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }

            mtxUpdate.ReleaseMutex();

            return res;
        }
        public string ExecuteScript(string queries)
        {
            string result = "";

            mtxUpdate.WaitOne();
            try
            {
                MySqlScript script = new MySqlScript(sqlCnx, queries);

                script.Error += new MySqlScriptErrorEventHandler(sqlScript_Error);

                int count = script.Execute();

                result += "\r\n" + count + " statements executed.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }
            mtxUpdate.ReleaseMutex();

            return result;
        }
        public string BackupToFile(string filename)
        {
            if (!Connected)
                return "";

            mtxUpdate.WaitOne();

            string result = "";

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
                    result += "\r\nReading table `" + table + "`...";

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

                        result += " done.";
                    }
                    else
                    {
                        result += " ignored.";
                    }
                }

                foreach (string query in queries_tables)
                {
                    sw.Write(query + ";\r\n");
                }

                sw.Close();

                result += "\r\nBackup of " + tables.Count + " tables done.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }

            mtxUpdate.ReleaseMutex();

            return result;
        }
        public bool AddVehicle(bool instanceOrSpawn, string classname, int vehicle_id, Tool.Point position)
        {
            int res;
            string worldspace = "\"[0,[" + position.X.ToString() + "," + position.Y.ToString() + ",0.0015]]\"";

            if (instanceOrSpawn == false /* instance */)
            {
                // we need to find a spawn point with this vehicle_id, to have a valid world_vehicle_id
                foreach (DataRow spawn in dsSpawnPoints.Tables[0].Rows)
                {
                    if (spawn.Field<UInt16>("vid") == vehicle_id)
                    {
                        var wv_id = spawn.Field<UInt64>("id");
                        float fuel = 0.7f;
                        float damage = 0.1f;
                        string inventory = "\"[]\"";
                        string parts = "\"[]\"";
                        
                        res = ExecuteSqlNonQuery("INSERT INTO instance_vehicle (`world_vehicle_id`, `instance_id`, `worldspace`, `inventory`, `parts`, `fuel`, `damage`, `created`) VALUES(" + wv_id + "," + InstanceId + "," + worldspace + "," + inventory + "," + parts + "," + fuel + "," + damage + ", now());");
                        return (res != 0);
                    }
                }

                return false;
            }

            /*else spawn point */
            res = ExecuteSqlNonQuery("INSERT INTO world_vehicle (`vehicle_id`, `world_id`, `worldspace`, `description`, `chance`) VALUES(" + vehicle_id + "," + WorldId + "," + worldspace + ",\"" + classname + "\", 0.7);");
            return (res != 0);
        }
        public bool RepairAndRefuelVehicle(string uid)
        {
            int res;
            switch (GameType)
            {
                case "Epoch":
                    res = ExecuteSqlNonQuery("UPDATE object_data SET Hitpoints='[]',Fuel='1',Damage='0' WHERE (ObjectID=" + uid + ")");
                    break;
                default:
                    res = ExecuteSqlNonQuery("UPDATE instance_vehicle SET parts='[]',fuel='1',damage='0' WHERE (id=" + uid + ")");
                    break;
            }
            return (res == 1);
        }
        public bool DeleteVehicle(string uid)
        {
            int res;
            switch (GameType)
            {
                case "Epoch":
                    res = ExecuteSqlNonQuery("DELETE FROM object_data WHERE ObjectID=" + uid);
                    break;
                default:
                    res = ExecuteSqlNonQuery("DELETE FROM instance_vehicle WHERE id=" + uid + " AND instance_id=" + InstanceId);
                    break;
            }
            return (res == 1);
        }
        public bool DeleteSpawn(string uid)
        {
            int res;
            switch (GameType)
            {
                case "Epoch":
                    res = 0;
                    break;
                default:
                    res = ExecuteSqlNonQuery("DELETE FROM world_vehicle WHERE id=" + uid + " AND world_id=" + WorldId);
                    break;
            }
            return (res == 1);
        }
        public string SpawnNewVehicles(int max_vehicles)
        {
            if (!Connected)
                return "";

            string sResult = "";

            mtxUpdate.WaitOne();

            try
            {
                MySqlCommand cmd = sqlCnx.CreateCommand();
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                cmd.CommandText = "SELECT count(*) FROM instance_vehicle WHERE instance_id =" + InstanceId;
                object result = cmd.ExecuteScalar();
                long vehicle_count = (long)result;

                cmd.CommandText = "SELECT wv.id world_vehicle_id, v.id vehicle_id, wv.worldspace, v.inventory, coalesce(v.parts, '') parts, v.limit_max,"
                                + " round(least(greatest(rand(), v.damage_min), v.damage_max), 3) damage, round(least(greatest(rand(), v.fuel_min), v.fuel_max), 3) fuel"
                                + " FROM world_vehicle wv JOIN vehicle v ON wv.vehicle_id = v.id LEFT JOIN instance_vehicle iv ON iv.world_vehicle_id = wv.id AND iv.instance_id = " + InstanceId
                                + " LEFT JOIN ( SELECT count(iv.id) AS count, wv.vehicle_id FROM instance_vehicle iv JOIN world_vehicle wv ON iv.world_vehicle_id = wv.id"
                                + " WHERE instance_id =" + InstanceId + " GROUP BY wv.vehicle_id) vc ON vc.vehicle_id = v.id"
                                + " WHERE wv.world_id =" + WorldId + " AND iv.id IS null AND (round(rand(), 3) < wv.chance)"
                                + " and (vc.count IS null OR vc.count BETWEEN v.limit_min AND v.limit_max) GROUP BY wv.worldspace";
                MySqlDataReader reader = cmd.ExecuteReader();

                int spawn_count = 0;

                List<string> queries = new List<string>();

                while (reader.Read() && (spawn_count + vehicle_count < max_vehicles))
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
                                + InstanceId + ", current_timestamp())");

                    spawn_count++;
                }

                reader.Close();

                foreach (string query in queries)
                {
                    cmd.CommandText = query;
                    int res = cmd.ExecuteNonQuery();
                }

                sResult = "spawned " + queries.Count + " new vehicles.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }

            mtxUpdate.ReleaseMutex();

            return sResult;
        }

        private MySqlConnection sqlCnx;
        private bool bConnected = false;
        private int instanceId;
        private int worldId;
        private string worldName;
        private string gameType;
        private Mutex mtxUpdate = new Mutex();
        private Mutex mtxUseDS = new Mutex();
        private DataSet dsWorldDefs { get; set; }
        private DataSet dsVehicleTypes { get; set; }
        private DataSet dsDeployableTypes { get; set; }
        private DataSet dsInstances = new DataSet();
        private DataSet dsDeployables = new DataSet();
        private DataSet dsAlivePlayers = new DataSet();
        private DataSet dsOnlinePlayers = new DataSet();
        private DataSet dsVehicles = new DataSet();
        private DataSet dsSpawnPoints = new DataSet();

        private void ClassicDB_OnConnection()
        {
            if (!bConnected)
                return;

            mtxUpdate.WaitOne();

            DataSet _dsWorlds = new DataSet();
            DataSet _dsAllVehicleTypes = new DataSet();

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
                DataColumn[] keysI = new DataColumn[1];
                keysI[0] = dsInstances.Tables[0].Columns[0];
                dsInstances.Tables[0].PrimaryKey = keysI;

                //
                //  Worlds
                //
                cmd.CommandText = "SELECT * FROM world";
                _dsWorlds.Clear();
                adapter.Fill(_dsWorlds);

                //
                //  Vehicle types
                //
                cmd.CommandText = "SELECT id,class_name FROM vehicle";
                _dsAllVehicleTypes.Clear();
                adapter.Fill(_dsAllVehicleTypes);
                DataColumn[] keysV = new DataColumn[1];
                keysV[0] = _dsAllVehicleTypes.Tables[0].Columns[0];
                _dsAllVehicleTypes.Tables[0].PrimaryKey = keysV;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }

            mtxUpdate.ReleaseMutex();

            mtxUseDS.WaitOne();

            try
            {
                foreach (DataRow row in _dsAllVehicleTypes.Tables[0].Rows)
                {
                    var id = row.Field<UInt16>("id");
                    var name = row.Field<string>("class_name");

                    var rowT = dsVehicleTypes.Tables[0].Rows.Find(name);

                    if (rowT == null)
                        dsVehicleTypes.Tables[0].Rows.Add(name, "Car", true, id);
                    else
                        rowT.SetField<UInt16>("Id", id);
                }

                foreach (DataRow row in _dsWorlds.Tables[0].Rows)
                {
                    DataRow rowWD = dsWorldDefs.Tables[0].Rows.Find(row.Field<UInt16>("id"));
                    if (rowWD == null)
                    {
                        dsWorldDefs.Tables[0].Rows.Add(row.Field<UInt16>("id"),
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

                DataRow rowI = this.dsInstances.Tables[0].Rows.Find(InstanceId);
                if (rowI != null)
                {
                    worldId = rowI.Field<UInt16>("world_id");

                    DataRow rowW = dsWorldDefs.Tables[0].Rows.Find(worldId);
                    worldName = (rowW != null) ? rowW.Field<string>("World Name") : "unknown";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }

            mtxUseDS.ReleaseMutex();

            if (WorldId == 0)
            {
                CloseConnection();
                MessageBox.Show("Instance id '" + InstanceId + "' not found in database", "Warning");
            }
        }
        private void ClassicDB_Refresh()
        {
            if (bConnected)
            {
                DataSet _dsAlivePlayers = new DataSet();
                DataSet _dsOnlinePlayers = new DataSet();
                DataSet _dsDeployables = new DataSet();
                DataSet _dsVehicles = new DataSet();
                DataSet _dsVehicleSpawnPoints = new DataSet();

                mtxUpdate.WaitOne();

                try
                {
                    MySqlCommand cmd = sqlCnx.CreateCommand();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    //
                    //  Players alive
                    //
                    cmd.CommandText = "SELECT s.id id, s.unique_id unique_id, p.name name, p.humanity humanity, s.worldspace worldspace,"
                                    + " s.inventory inventory, s.backpack backpack, s.medical medical, s.state state, s.last_updated last_updated"
                                    + " FROM survivor as s, profile as p WHERE s.unique_id=p.unique_id AND s.world_id=" + WorldId + " AND s.is_dead=0";
                    cmd.CommandText += " AND s.last_updated > now() - interval " + FilterLastUpdated + " day";
                    _dsAlivePlayers.Clear();
                    adapter.Fill(_dsAlivePlayers);

                    //
                    //  Players online
                    //
                    cmd.CommandText = "SELECT s.id id, s.unique_id unique_id, p.name name, p.humanity humanity, s.worldspace worldspace,"
                                    + " s.inventory inventory, s.backpack backpack, s.medical medical, s.state state, s.last_updated last_updated"
                                    + " FROM survivor as s, profile as p WHERE s.unique_id=p.unique_id AND s.world_id=" + WorldId + " AND s.is_dead=0"
                                    + " AND s.last_updated > now() - interval " + OnlineTimeLimit + " minute";
                    _dsOnlinePlayers.Clear();
                    adapter.Fill(_dsOnlinePlayers);

                    //
                    //  Vehicles
                    //
                    cmd.CommandText = "SELECT iv.id id, wv.id spawn_id, v.class_name class_name, iv.worldspace worldspace, iv.inventory inventory,"
                                    + " iv.fuel fuel, iv.damage damage, iv.last_updated last_updated, iv.parts parts"
                                    + " FROM vehicle as v, world_vehicle as wv, instance_vehicle as iv"
                                    + " WHERE iv.instance_id=" + InstanceId
                                    + " AND iv.world_vehicle_id=wv.id AND wv.vehicle_id=v.id"
                                    + " AND iv.last_updated > now() - interval " + FilterLastUpdated + " day";
                    _dsVehicles.Clear();
                    adapter.Fill(_dsVehicles);

                    //
                    //  Vehicle Spawn points
                    //
                    cmd.CommandText = "SELECT w.id id, w.vehicle_id vid, w.worldspace worldspace, w.chance chance, v.inventory inventory, v.class_name class_name FROM world_vehicle as w, vehicle as v"
                                    + " WHERE w.world_id=" + WorldId + " AND w.vehicle_id=v.id";
                    _dsVehicleSpawnPoints.Clear();
                    adapter.Fill(_dsVehicleSpawnPoints);

                    //
                    //  Deployables
                    //
                    cmd.CommandText = "SELECT id.id id, d.class_name class_name, id.worldspace, id.inventory"
                                    + " FROM instance_deployable as id, deployable as d"
                                    + " WHERE instance_id=" + InstanceId
                                    + " AND id.deployable_id=d.id"
                                    + " AND id.last_updated > now() - interval " + FilterLastUpdated + " day";
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

                    if (dsDeployableTypes.Tables[0].Rows.Find(name) == null)
                        dsDeployableTypes.Tables[0].Rows.Add(name, "Unknown", true);
                }

                mtxUpdate.ReleaseMutex();

                mtxUseDS.WaitOne();

                dsDeployables = _dsDeployables.Copy();
                dsAlivePlayers = _dsAlivePlayers.Copy();
                dsOnlinePlayers = _dsOnlinePlayers.Copy();
                dsVehicles = _dsVehicles.Copy();
                dsSpawnPoints = _dsVehicleSpawnPoints.Copy();

                mtxUseDS.ReleaseMutex();
            }
        }
        private void EpochDB_OnConnection()
        {
            if (!bConnected)
                return;

            mtxUpdate.WaitOne();

            DataSet _dsVehicles = new DataSet();

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

            mtxUpdate.ReleaseMutex();

            mtxUseDS.WaitOne();

            try
            {
                foreach (DataRow row in _dsVehicles.Tables[0].Rows)
                {
                    string name = row.Field<string>("class_name");

                    if (dsVehicleTypes.Tables[0].Rows.Find(name) == null)
                        dsVehicleTypes.Tables[0].Rows.Add(name, "Car", true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
            }

            mtxUseDS.ReleaseMutex();
        }
        private void EpochDB_Refresh()
        {
            if (bConnected)
            {
                DataSet _dsAlivePlayers = new DataSet();
                DataSet _dsOnlinePlayers = new DataSet();
                DataSet _dsDeployables = new DataSet();
                DataSet _dsVehicles = new DataSet();

                mtxUpdate.WaitOne();

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
                    cmd.CommandText += " AND cd.LastLogin > now() - interval " + FilterLastUpdated + " day";
                    _dsAlivePlayers.Clear();
                    adapter.Fill(_dsAlivePlayers);

                    //
                    //  Players online
                    //
                    cmd.CommandText = "SELECT cd.CharacterID id, pd.PlayerUID unique_id, pd.PlayerName name, cd.Humanity humanity, cd.worldspace worldspace,"
                                    + " cd.inventory inventory, cd.backpack backpack, cd.medical medical, cd.CurrentState state, cd.DateStamp last_updated"
                                    + " FROM character_data as cd, player_data as pd"
                                    + " WHERE cd.PlayerUID=pd.PlayerUID AND cd.Alive=1";
                    cmd.CommandText += " AND cd.LastLogin > now() - interval " + OnlineTimeLimit + " minute";
                    _dsOnlinePlayers.Clear();
                    adapter.Fill(_dsOnlinePlayers);

                    //
                    //  Vehicles
                    cmd.CommandText = "SELECT CAST(ObjectID AS UNSIGNED) id, CAST(0 AS UNSIGNED) spawn_id, ClassName class_name, worldspace, inventory, Hitpoints parts,"
                                    + " fuel, damage, DateStamp last_updated"
                                    + " FROM object_data WHERE CharacterID=0";
                    cmd.CommandText += " AND Datestamp > now() - interval " + FilterLastUpdated + " day";
                    _dsVehicles.Clear();
                    adapter.Fill(_dsVehicles);

                    //
                    //  Deployables
                    //
                    cmd.CommandText = "SELECT CAST(ObjectID AS UNSIGNED) id, Classname class_name, worldspace, inventory FROM object_data WHERE CharacterID!=0";
                    cmd.CommandText += " AND Datestamp > now() - interval " + FilterLastUpdated + " day";
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

                    if (dsDeployableTypes.Tables[0].Rows.Find(name) == null)
                        dsDeployableTypes.Tables[0].Rows.Add(name, "Unknown", true);
                }

                mtxUpdate.ReleaseMutex();

                mtxUseDS.WaitOne();

                dsDeployables = _dsDeployables.Copy();
                dsAlivePlayers = _dsAlivePlayers.Copy();
                dsOnlinePlayers = _dsOnlinePlayers.Copy();
                dsVehicles = _dsVehicles.Copy();

                worldId = 1;

                mtxUseDS.ReleaseMutex();
            }
        }
        private void sqlScript_Error(Object sender, MySqlScriptErrorEventArgs args)
        {
            MessageBox.Show(args.Exception.Message, "Script error");
        }
    }
}
