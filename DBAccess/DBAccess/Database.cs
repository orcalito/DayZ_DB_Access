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
        public int InstanceId { get; set; }
        public int WorldId { get { return worldId; } }
        public string WorldName { get { return worldName; } }
        public string GameType { get { return gameType; } }
        public DataSet WorldDefs { get { return dsWorldDefs; } }
        public DataSet VehicleTypes { get { return dsVehicleTypes; } }
        public DataSet DeployableTypes { get { return dsDeployableTypes; } }
        public DataSet Instances { get { return dsInstances; } }
        public DataSet Deployables { get { return dsDeployables; } }
        public DataSet PlayersAlive { get { return dsAlivePlayers; } }
        public DataSet PlayersDead { get { return dsDeadPlayers; } }
        public DataSet Vehicles { get { return dsVehicles; } }
        public DataSet SpawnPoints { get { return dsSpawnPoints; } }
        public List<int> GetInstanceList()
        {
            List<int> listIDs = new List<int>();

            MySqlCommand cmd = sqlCnx.CreateCommand();

            switch (GameType)
            {
                case "Epoch":   cmd.CommandText = "SELECT Instance, COUNT(*) FROM " + epochObjectTable + " GROUP BY Instance"; break;
                default:        cmd.CommandText = "SELECT id, COUNT(*) FROM instance GROUP BY Instance"; break;
            }
            
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string instance = reader.GetString(0);

                listIDs.Add(int.Parse(instance));
            }
            reader.Close();

            return listIDs;
        }

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

                this.FilterLastUpdated = cfg.filter_last_updated;
                this.OnlineTimeLimit = int.Parse(cfg.online_time_limit);
                this.gameType = cfg.game_type;
                this.InstanceId = instance_id;

                dsWorldDefs = cfg.worlds_def;
                dsVehicleTypes = cfg.vehicle_types;
                dsDeployableTypes = cfg.deployable_types;

                if (gameType == "Auto")
                {
                    bool bFoundClassic = false;
                    bool bFoundEpoch = false;

                    MySqlCommand cmd = sqlCnx.CreateCommand();
                    cmd.CommandText = "SHOW TABLES";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);

                        bFoundClassic |= (name == "instance_vehicle");
                        bFoundEpoch |= (name == "object_data");
                    }
                    reader.Close();

                    if (bFoundEpoch && !bFoundClassic)
                        gameType = "Epoch";
                    else if (!bFoundEpoch && bFoundClassic)
                        gameType = "Classic";
                    else
                    {
                        MessageBox.Show("Can't determine the type of database between epoch and default DayZ schema, using default schema");
                        gameType = "Classic";
                    }
                }

                this.bConnected = true;

                OnConnection();

                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
                this.bConnected = false;
            }
            mtxUpdate.ReleaseMutex();
        }
        public void CloseConnection()
        {
            mtxUseDS.WaitOne();
            mtxUpdate.WaitOne();

            this.bConnected = false;

            dsAlivePlayers.Clear();
            dsVehicles.Clear();
            dsSpawnPoints.Clear();
            dsDeployables.Clear();

            if (sqlCnx != null)
                sqlCnx.Close();

            mtxUpdate.ReleaseMutex();
            mtxUseDS.ReleaseMutex();
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
            if (!Connected)
                return;

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
        private class TableQueries
        {
            public TableQueries(string name)
            {
                this.Name = name;
            }
            public string Name = null;
            public string dropQuery = null;
            public string createQuery = null;
            public string FillQuery = null;
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

                List<TableQueries> queries = new List<TableQueries>(tables.Count);
                List<string> comments = new List<string>();
                foreach (string table in tables)
                {
                    result += "\r\nReading table `" + table + "`...";

                    TableQueries tableQueries = null;

                    //
                    //  Drop & Create table queries
                    //
                    cmd.CommandText = "SHOW CREATE TABLE " + table;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string str = reader.GetString(1);

                        if (!(str.Contains("VIEW") && str.Contains("ALGORITHM")))
                        {
                            tableQueries = new TableQueries(table);
                            tableQueries.dropQuery = "DROP TABLE IF EXISTS `" + table + "`;\r\n";
                            tableQueries.createQuery = str + ";\r\n";
                        }
                        else
                        {
                            comments.Add("-- View `" + table + "` has been ignored. --\r\n");
                        }
                    }
                    reader.Close();

                    //
                    //  Fill table query
                    //
                    if (tableQueries != null)
                    {
                        queries.Add(tableQueries);

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
                            tableQueries.FillQuery = "LOCK TABLES `" + table + "` WRITE;\r\nREPLACE INTO `" + table + "` VALUES " + all_values + ";\r\nUNLOCK TABLES;\r\n\r\n";
                        }

                        result += " done.";
                    }
                    else
                    {
                        result += " ignored.";
                    }
                }

                //
                //  Reorder tables based on constraints between them
                //

                //
                //  1st pass, No constraints at top, constraints at end of the array
                //
                TableQueries[] arrayQ = new TableQueries[queries.Count];
                int headIdx=0;
                int tailIdx=queries.Count-1;
                for (int i = 0; i < queries.Count; i++)
                {
                    TableQueries tq = queries[i];

                    if (tq.createQuery.IndexOf("REFERENCES") >= 0)
                        arrayQ[tailIdx--] = tq;
                    else
                        arrayQ[headIdx++] = tq;
                }

                //
                //  2nd pass, Order from constraints between tables
                //
                for (int i = tailIdx+1; i < queries.Count; i++)
                {
                    TableQueries tq = arrayQ[i];

                    bool bMoveToTail = false;

                    int idx = tq.createQuery.IndexOf("REFERENCES");
                    while (idx >= 0)
                    {
                        int idEnd = tq.createQuery.IndexOf('(', idx);
                        string sub = tq.createQuery.Substring(idx, idEnd - idx + 1);
                        string name = sub.Split('`')[1];

                        //  subs[1] = table's name
                        for (int j = tailIdx+1; j < queries.Count; j++)
                            if ((arrayQ[j].Name == name) && (j >= i))
                                bMoveToTail = true;

                        idx = tq.createQuery.IndexOf("REFERENCES", idx + 1);
                    }

                    if (bMoveToTail)
                    {
                        for (int j = i; j < queries.Count - 1; j++)
                            arrayQ[j] = arrayQ[j + 1];

                        arrayQ[queries.Count - 1] = tq;

                        i--;
                    }
                }

                //
                //
                //
                foreach(var q in comments)
                    sw.Write(q);

                sw.Write("\r\n");

                for(int i=queries.Count-1; i>=0; i--)
                    sw.Write(arrayQ[i].dropQuery);

                sw.Write("\r\n");

                foreach (var q in arrayQ)
                {
                    sw.Write(q.createQuery);
                    sw.Write(q.FillQuery + "\r\n");
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
            string worldspace = "\"[0,[" + position.X.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + position.Y.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0.0015]]\"";

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
        public bool TeleportPlayer(string uid, Tool.Point position)
        {
            int res;
            string worldspace = "\"[0,[" + position.X.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + position.Y.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0.0015]]\"";

            switch (GameType)
            {
                case "Epoch":
                    res = ExecuteSqlNonQuery("UPDATE " + epochCharacterTable + " SET Worldspace=" + worldspace + " WHERE (PlayerUID=" + uid + " AND Alive=1 AND InstanceID=" + InstanceId + ") ORDER BY CharacterID DESC LIMIT 1");
                    break;
                default:
                    res = ExecuteSqlNonQuery("UPDATE `survivor` SET worldspace=" + worldspace + " WHERE (world_id=" + WorldId + " AND unique_id=" + uid + " AND is_dead=0) ORDER BY id DESC LIMIT 1");
                    break;
            }
            return (res == 1);
        }
        public bool HealPlayer(string uid)
        {
            int res;
            switch (GameType)
            {
                case "Epoch":
                    res = ExecuteSqlNonQuery("UPDATE " + epochCharacterTable + " SET Medical='[false,false,false,false,false,false,true,12000,[],[0,0],0,[0,0]]' WHERE (PlayerUID=" + uid + " AND Alive=1 AND InstanceID=" + InstanceId + ")");
                    break;
                default:
                    res = ExecuteSqlNonQuery("UPDATE survivor SET medical='[false,false,false,false,false,false,true,12000,[],[0,0],0,[0,0]]' WHERE (unique_id=" + uid + " AND is_dead=0)");
                    break;
            }
            return (res == 1);
        }
        public bool RepairAndRefuelVehicle(string uid)
        {
            int res;
            switch (GameType)
            {
                case "Epoch":
                    res = ExecuteSqlNonQuery("UPDATE " + epochObjectTable + " SET Hitpoints='[]',Fuel='1',Damage='0' WHERE (ObjectID=" + uid + " AND Instance=" + InstanceId + ")");
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
                    res = ExecuteSqlNonQuery("DELETE FROM " + epochObjectTable + " WHERE (ObjectID=" + uid + " AND Instance=" + InstanceId + ")");
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

        internal MySqlConnection sqlCnx;
        private bool bConnected = false;
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
        private DataSet dsDeadPlayers = new DataSet();
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
                this.bConnected = false;
            }

            mtxUpdate.ReleaseMutex();

            if (!Connected)
                return;

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
                this.bConnected = false;
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
                DataSet _dsDeadPlayers = new DataSet();
                DataSet _dsDeployables = new DataSet();
                DataSet _dsVehicles = new DataSet();
                DataSet _dsVehicleSpawnPoints = new DataSet();

                mtxUpdate.WaitOne();

                if (bConnected)
                {
                    try
                    {
                        MySqlCommand cmd = sqlCnx.CreateCommand();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                        //
                        //  Players alive
                        //
                        {
                            cmd.CommandText = "SELECT s.id id, s.unique_id unique_id, p.name name, p.humanity humanity, s.worldspace worldspace,"
                                            + " s.inventory inventory, s.backpack backpack, s.medical medical, s.state state, s.last_updated last_updated"
                                            + " FROM survivor as s, profile as p WHERE s.unique_id=p.unique_id AND s.world_id=" + WorldId + " AND s.is_dead=0";
                            cmd.CommandText += " AND s.last_updated > now() - interval " + FilterLastUpdated + " day";
                            _dsAlivePlayers.Clear();
                            adapter.Fill(_dsAlivePlayers);
                            DataColumn[] keys = new DataColumn[1];
                            keys[0] = _dsAlivePlayers.Tables[0].Columns[1];
                            _dsAlivePlayers.Tables[0].PrimaryKey = keys;
                        }

                        //
                        //  Players dead
                        //
                        {
                            cmd.CommandText = "SELECT s.id id, s.unique_id unique_id, p.name name, p.humanity humanity, s.worldspace worldspace,"
                                            + " s.inventory inventory, s.backpack backpack, s.medical medical, s.state state, s.last_updated last_updated"
                                            + " FROM survivor as s, profile as p WHERE s.unique_id=p.unique_id AND s.world_id=" + WorldId + " AND s.is_dead=1";
                            cmd.CommandText += " AND s.last_updated > now() - interval " + FilterLastUpdated + " day";
                            _dsDeadPlayers.Clear();
                            adapter.Fill(_dsDeadPlayers);
                            DataColumn[] keys = new DataColumn[1];
                            keys[0] = _dsDeadPlayers.Tables[0].Columns[0];
                            _dsDeadPlayers.Tables[0].PrimaryKey = keys;
                        }

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
                        cmd.CommandText = "SELECT id.id id, CAST(0 AS UNSIGNED) keycode, d.class_name class_name, id.worldspace, id.inventory"
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
                        this.bConnected = false;
                    }

                    foreach (DataRow row in _dsDeployables.Tables[0].Rows)
                    {
                        string name = row.Field<string>("class_name");

                        if (dsDeployableTypes.Tables[0].Rows.Find(name) == null)
                            dsDeployableTypes.Tables[0].Rows.Add(name, "Unknown", true);
                    }
                }

                mtxUpdate.ReleaseMutex();

                mtxUseDS.WaitOne();

                dsDeployables = _dsDeployables.Copy();
                dsAlivePlayers = _dsAlivePlayers.Copy();
                dsDeadPlayers = _dsDeadPlayers.Copy();
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
                cmd.CommandText = "SELECT Classname class_name FROM " + epochObjectTable + " WHERE CharacterID=0 AND Instance=" + InstanceId;
                _dsVehicles.Clear();
                adapter.Fill(_dsVehicles);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception found");
                this.bConnected = false;
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
                this.bConnected = false;
            }

            mtxUseDS.ReleaseMutex();
        }
        private void EpochDB_Refresh()
        {
            if (bConnected)
            {
                DataSet _dsAlivePlayers = new DataSet();
                DataSet _dsDeadPlayers = new DataSet();
                DataSet _dsDeployables = new DataSet();
                DataSet _dsVehicles = new DataSet();

                mtxUpdate.WaitOne();

                if (bConnected)
                {
                    try
                    {
                        MySqlCommand cmd = sqlCnx.CreateCommand();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                        //
                        //  Players alive
                        //
                        {
                            cmd.CommandText = "SELECT cd.CharacterID id, pd.PlayerUID unique_id, pd.PlayerName name, cd.Humanity humanity, cd.worldspace worldspace,"
                                            + " cd.inventory inventory, cd.backpack backpack, cd.medical medical, cd.CurrentState state, cd.DateStamp last_updated"
                                            + " FROM " + epochCharacterTable + " as cd, " + epochPlayerTable + " as pd"
                                            + " WHERE cd.PlayerUID=pd.PlayerUID AND cd.Alive=1"
                                            + " AND cd.InstanceID=" + InstanceId
                                            + " AND cd.LastLogin > now() - interval " + FilterLastUpdated + " day";
                            _dsAlivePlayers.Clear();
                            adapter.Fill(_dsAlivePlayers);
                            DataColumn[] keys = new DataColumn[1];
                            keys[0] = _dsAlivePlayers.Tables[0].Columns[1];
                            _dsAlivePlayers.Tables[0].PrimaryKey = keys;
                        }

                        //
                        //  Players dead
                        //
                        {
                            cmd.CommandText = "SELECT cd.CharacterID id, pd.PlayerUID unique_id, pd.PlayerName name, cd.Humanity humanity, cd.worldspace worldspace,"
                                            + " cd.inventory inventory, cd.backpack backpack, cd.medical medical, cd.CurrentState state, cd.DateStamp last_updated"
                                            + " FROM " + epochCharacterTable + " as cd, " + epochPlayerTable + " as pd"
                                            + " WHERE cd.PlayerUID=pd.PlayerUID AND cd.Alive=0"
                                            + " AND cd.InstanceID=" + InstanceId
                                            + " AND cd.LastLogin > now() - interval " + FilterLastUpdated + " day";
                            _dsDeadPlayers.Clear();
                            adapter.Fill(_dsDeadPlayers);
                            DataColumn[] keys = new DataColumn[1];
                            keys[0] = _dsDeadPlayers.Tables[0].Columns[0];
                            _dsDeadPlayers.Tables[0].PrimaryKey = keys;
                        }

                        //
                        //  Vehicles
                        cmd.CommandText = "SELECT CAST(ObjectID AS UNSIGNED) id, CAST(0 AS UNSIGNED) spawn_id, ClassName class_name, worldspace, inventory, Hitpoints parts,"
                                        + " fuel, damage, DateStamp last_updated"
                                        + " FROM " + epochObjectTable + " WHERE CharacterID=0"
                                        + " AND Instance=" + InstanceId
                                        + " AND Datestamp > now() - interval " + FilterLastUpdated + " day";
                        _dsVehicles.Clear();
                        adapter.Fill(_dsVehicles);

                        //
                        //  Deployables
                        //
                        cmd.CommandText = "SELECT CAST(ObjectID AS UNSIGNED) id, CharacterID keycode, Classname class_name, worldspace, inventory FROM " + epochObjectTable + " WHERE CharacterID!=0"
                                        + " AND Instance=" + InstanceId
                                        + " AND Datestamp > now() - interval " + FilterLastUpdated + " day";
                        _dsDeployables.Clear();
                        adapter.Fill(_dsDeployables);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Exception found");
                        this.bConnected = false;
                    }

                    foreach (DataRow row in _dsDeployables.Tables[0].Rows)
                    {
                        string name = row.Field<string>("class_name");

                        if (dsDeployableTypes.Tables[0].Rows.Find(name) == null)
                            dsDeployableTypes.Tables[0].Rows.Add(name, "Unknown", true);
                    }
                }

                mtxUpdate.ReleaseMutex();

                mtxUseDS.WaitOne();

                dsDeployables = _dsDeployables.Copy();
                dsAlivePlayers = _dsAlivePlayers.Copy();
                dsDeadPlayers = _dsDeadPlayers.Copy();
                dsVehicles = _dsVehicles.Copy();

                worldId = 1;

                mtxUseDS.ReleaseMutex();
            }
        }
        private void sqlScript_Error(Object sender, MySqlScriptErrorEventArgs args)
        {
            MessageBox.Show(args.Exception.Message, "Script error");
        }

        internal string epochCharacterTable = "character_data";
        internal string epochObjectTable = "object_data";
        internal string epochPlayerTable = "player_data";
        internal string epochTraderTable = "traders_data";
    }
}
