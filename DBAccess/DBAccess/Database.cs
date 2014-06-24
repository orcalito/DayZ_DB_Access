using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace DBAccess
{
    public abstract class DBSchema
    {
        public int WorldId = 0;
        public int InstanceId = 0;
        public string WorldName = "";
        public int FilterLastUpdated = 0;
        public int OnlineTimeLimit = 0;

        public DBSchema(myDatabase db) { this.db = db; }

        public abstract string Type { get; }

        public abstract string BuildQueryInstanceList();

        public abstract bool QueryUpdatePlayerPosition(string worldspace, string uid);
        public abstract bool QueryHealPlayer(string uid);
        public abstract bool QueryRevivePlayer(string uid, string char_id);
        public abstract bool QuerySavePlayerState(string uid);
        public abstract bool QueryRestorePlayerState(string uid);
        public abstract bool QueryRepairAndRefuel(string uid);
        public abstract bool QueryDeleteVehicle(string uid);
        public abstract bool QueryDeleteSpawn(string uid);
        public abstract bool QueryAddVehicle(bool instanceOrSpawn, string classname, int vehicle_id, Tool.Point position);
        public abstract int QuerySpawnVehicles(int max_vehicles);
        public abstract int QueryRemoveBodies(int time_limit);

        public abstract bool OnConnection();
        public abstract bool Refresh();

        public myDatabase db = null;
        public DataSet dsWorldDefs = new DataSet();
        public DataSet dsVehicleTypes = new DataSet();
        public DataSet dsDeployableTypes = new DataSet();
        public DataSet dsInstances = new DataSet();
        public DataSet dsDeployables = new DataSet();
        public DataSet dsAlivePlayers = new DataSet();
        public DataSet dsDeadPlayers = new DataSet();
        public DataSet dsVehicles = new DataSet();
        public DataSet dsSpawnPoints = new DataSet();
        public DataSet dsPlayerStates = new DataSet();
    }

    public class myDatabase
    {
        public class LoginData
        {
            public LoginData()
            {
                Server = "";
                Port = 0;
                DBname = "";
                Username = "";
                Password = "";
                InstanceId = -1;
                Cfg = null;
            }
            public string Server { get; set; }
            public int Port { get; set; }
            public string DBname { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int InstanceId { get; set; }
            public myConfig Cfg { get; set; }
        }

        public myDatabase()
        {
            Connected = false;
        }

        public DBSchema Schema { get { return schema; } }
        public bool Connected { get; private set; }
        public decimal ReconnectDelay
        {
            get
            {
                return reconnectDelay;
            }

            set
            {
                reconnectDelay = value;
                if (value > 0)
                {
                    if (reconnectThread == null)
                    {
                        reconnectThread = new Thread(ThreadReconnect);
                        reconnectThread.Start();
                    }
                }
                else
                {
                    reconnectThread = null;
                }
            }
        }

        //public int InstanceId { get { return schema.InstanceId; } }
        //public int WorldId { get { return schema.WorldId; } }
        //public string WorldName { get { return schema.WorldName; } }
        //public string GameType { get { return schema.Type; } }
        
        //public DataSet WorldDefs { get { return dsWorldDefs; } }
        //public DataSet VehicleTypes { get { return dsVehicleTypes; } }
        //public DataSet DeployableTypes { get { return dsDeployableTypes; } }
        //public DataSet Instances { get { return dsInstances; } }
        public DataSet Deployables  { get { return schema.dsDeployables; } }
        public DataSet PlayersAlive { get { return schema.dsAlivePlayers; } }
        public DataSet PlayersDead  { get { return schema.dsDeadPlayers; } }
        public DataSet Vehicles     { get { return schema.dsVehicles; } }
        public DataSet SpawnPoints  { get { return schema.dsSpawnPoints; } }
        public DataSet PlayerStates { get { return schema.dsPlayerStates; } }
        public MySqlConnection Cnx { get { return sqlCnx; } }
        public void AccessDB(bool state)
        {
            switch (state)
            {
                case true: mtxAccessDB.WaitOne(); break;
                case false: mtxAccessDB.ReleaseMutex(); break;
            }
        }
        public void UseDS(bool state)
        {
            switch (state)
            {
                case true: mtxUseDS.WaitOne(); break;
                case false: mtxUseDS.ReleaseMutex(); break;
            }
        }
        public void Connect(LoginData login)
        {
            AccessDB(true);

            loginData = login;

            //  "Server=localhost;Database=testdb;Uid=root;Pwd=pass;";
            string strCnx = "Server=" + login.Server + ";Port=" + login.Port + ";Database=" + login.DBname + ";Uid=" + login.Username + ";Pwd=" + login.Password + ";";

            try
            {
                sqlCnx = new MySqlConnection(strCnx);

                sqlCnx.Open();

                DetermineGameSchema();

                schema.WorldId = login.Cfg.world_id;
                schema.InstanceId = login.InstanceId;
                schema.dsWorldDefs = login.Cfg.worlds_def;
                schema.dsVehicleTypes = login.Cfg.vehicle_types;
                schema.dsDeployableTypes = login.Cfg.deployable_types;
                schema.FilterLastUpdated = login.Cfg.filter_last_updated;
                schema.OnlineTimeLimit = int.Parse(login.Cfg.online_time_limit);
                schema.dsPlayerStates = login.Cfg.player_state;

                this.Connected = true;

                Connected = schema.OnConnection();
                    
                Refresh();

                loginAccepted = true;
            }
            catch
            {
                this.Connected = false;
            }
            AccessDB(false);
        }
        public void CloseConnection()
        {
            mtxUseDS.WaitOne();
            AccessDB(true);

            this.Connected = false;
            keepRunning = false;
            loginAccepted = false;


            schema.dsAlivePlayers.Clear();
            schema.dsVehicles.Clear();
            schema.dsSpawnPoints.Clear();
            schema.dsPlayerStates.Clear();
            schema.dsDeployables.Clear();
            schema = new NullSchema(null);

            if (sqlCnx != null)
                sqlCnx.Close();

            AccessDB(false);
            mtxUseDS.ReleaseMutex();
        }
        public void Refresh()
        {
            if (Connected)
                Connected = schema.Refresh();
        }
        public List<int> GetInstanceList()
        {
            List<int> listIDs = new List<int>();

            MySqlCommand cmd = sqlCnx.CreateCommand();

            cmd.CommandText = schema.BuildQueryInstanceList();

            AccessDB(true);

            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                listIDs.Add(int.Parse(reader.GetString(0)));
            }
            reader.Close();

            AccessDB(false);

            return listIDs;
        }
        public int ExecuteSqlNonQuery(string query)
        {
            if (!Connected)
                return 0;

            int res = 0;

            AccessDB(true);

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

            AccessDB(false);

            return res;
        }
        public string ExecuteScript(string queries)
        {
            string result = "";

            AccessDB(true);
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
            AccessDB(false);

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

            AccessDB(true);

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

            AccessDB(false);

            return result;
        }
        public bool AddVehicle(bool instanceOrSpawn, string classname, int vehicle_id, Tool.Point position)
        {
            bool res = false;

            if (Connected)
            {
                res = schema.QueryAddVehicle(instanceOrSpawn, classname, vehicle_id, position);
            }
            return res;
        }
        public bool TeleportPlayer(string uid, Tool.Point position)
        {
            bool res = false;

            if (Connected)
            {
                string worldspace = "\"[0,[" + position.X.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + position.Y.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0.0015]]\"";
                res = schema.QueryUpdatePlayerPosition(worldspace, uid);
            }
            return res;
        }
        public bool HealPlayer(string uid)
        {
            bool res = false;

            if (Connected)
            {
                res = schema.QueryHealPlayer(uid);
            }
            return res;
        }
        public bool RevivePlayer(string uid, string char_id)
        {
            bool res = false;

            if (Connected)
            {
                res = schema.QueryRevivePlayer(uid, char_id);
            }
            return res;
        }
        public bool SavePlayerState(string uid)
        {
            bool res = false;

            if (Connected)
            {
                res = schema.QuerySavePlayerState(uid);
            }
            return res;
        }
        public bool RestorePlayerState(string uid)
        {
            bool res = false;

            if (Connected)
            {
                res = schema.QueryRestorePlayerState(uid);
            }
            return res;
        }
        public bool RepairAndRefuelVehicle(string uid)
        {
            bool res = false;

            if (Connected)
            {
                res = schema.QueryRepairAndRefuel(uid);
            }
            return res;
        }
        public bool DeleteVehicle(string uid)
        {
            bool res = false;

            if (Connected)
            {
                res = schema.QueryDeleteVehicle(uid);
            }
            return res;
        }
        public bool DeleteSpawn(string uid)
        {
            bool res = false;

            if (Connected)
            {
                res = schema.QueryDeleteSpawn(uid);
            }
            return res;
        }
        public int RemoveBodies(int time_limit)
        {
            int res = 0;

            if (Connected)
            {
                res = schema.QueryRemoveBodies(time_limit);
            }
            return res;
        }
        public int SpawnNewVehicles(int max_vehicles)
        {
            int res = 0;

            if (Connected)
            {
                res = schema.QuerySpawnVehicles(max_vehicles);
            }
            return res;
        }
        private void DetermineGameSchema()
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

            string gameType = "Classic";
            if (bFoundEpoch && !bFoundClassic)
                gameType = "Epoch";
            else if (!bFoundEpoch && bFoundClassic)
                gameType = "Classic";
            else
                MessageBox.Show("Can't determine the type of database between epoch and classic DayZ schema, using classic schema");

            switch (gameType)
            {
                case "Epoch": schema = new EpochSchema(this); break;
                default: schema = new ClassicSchema(this); break;
            }
        }
        private void sqlScript_Error(Object sender, MySqlScriptErrorEventArgs args)
        {
            MessageBox.Show(args.Exception.Message, "Script error");
        }
        private void ThreadReconnect()
        {
            long remaining_ticks = (long)(ReconnectDelay * 10000000);

            keepRunning = true;

            while (keepRunning && (ReconnectDelay > 0))
            {
                long last_ticks = DateTime.Now.Ticks;
                Thread.Sleep(250);
                remaining_ticks -= (DateTime.Now.Ticks - last_ticks);

                if (remaining_ticks <= 0)
                {
                    remaining_ticks = (long)(ReconnectDelay * 10000000);

                    if (loginAccepted && !Connected)
                    {
                        Connect(loginData);
                    }
                }
            }
        }

        private LoginData loginData = null;
        private MySqlConnection sqlCnx = null;
        private DBSchema schema = new NullSchema(null);
        private Mutex mtxAccessDB = new Mutex();
        private Mutex mtxUseDS = new Mutex();
        private bool loginAccepted = false;
        private bool keepRunning = false;
        private Thread reconnectThread = null;
        private decimal reconnectDelay = 0;
    }

    public class NullSchema : DBSchema
    {
        public NullSchema(myDatabase db) : base(null) { }

        public override string Type { get { return "Null"; } }
        public override string BuildQueryInstanceList() { return ""; }
        public override bool QueryUpdatePlayerPosition(string worldspace, string uid) { return true; }
        public override bool QueryHealPlayer(string uid) { return true; }
        public override bool QueryRevivePlayer(string uid, string char_id) { return true; }
        public override bool QuerySavePlayerState(string uid) { return true; }
        public override bool QueryRestorePlayerState(string uid) { return true; }
        public override bool QueryRepairAndRefuel(string uid) { return true; }
        public override bool QueryDeleteVehicle(string uid) { return true; }
        public override bool QueryDeleteSpawn(string uid) { return true; }
        public override int QueryRemoveBodies(int time_limit) { return 0; }
        public override bool QueryAddVehicle(bool instanceOrSpawn, string classname, int vehicle_id, Tool.Point position) { return true; }
        public override int QuerySpawnVehicles(int max_vehicles) { return 0; }
        public override bool OnConnection() { return false; }
        public override bool Refresh() { return false; }
    }
    public class ClassicSchema : DBSchema
    {
        public ClassicSchema(myDatabase db) : base(db) { }

        public override string Type { get { return "Classic"; } }
        public override string BuildQueryInstanceList()
        {
            return "SELECT instance_id, COUNT(*) FROM instance_vehicle GROUP BY instance_id";
        }
        public override bool QueryUpdatePlayerPosition(string worldspace, string uid)
        {
            return 1 == db.ExecuteSqlNonQuery("UPDATE `survivor` SET worldspace=" + worldspace + " WHERE (world_id=" + WorldId + " AND unique_id=" + uid + " AND is_dead=0) ORDER BY id DESC LIMIT 1");
        }
        public override bool QueryHealPlayer(string uid)
        {
            return 1 == db.ExecuteSqlNonQuery("UPDATE survivor SET medical='[false,false,false,false,false,false,true,12000,[],[0,0],0,[0,0]]' WHERE (unique_id='" + uid + "' AND is_dead='0')");
        }
        public override bool QueryRevivePlayer(string uid, string char_id)
        {
            return true;
        }
        public override bool QuerySavePlayerState(string uid)
        {
            return true;
        }
        public override bool QueryRestorePlayerState(string uid)
        {
            return true;
        }
        public override bool QueryRepairAndRefuel(string uid)
        {
            return 1 == db.ExecuteSqlNonQuery("UPDATE instance_vehicle SET parts='[]',fuel='1',damage='0' WHERE (id=" + uid + ")");
        }
        public override bool QueryDeleteVehicle(string uid)
        {
            return 1 == db.ExecuteSqlNonQuery("DELETE FROM instance_vehicle WHERE id=" + uid + " AND instance_id=" + InstanceId);
        }
        public override bool QueryDeleteSpawn(string uid)
        {
            return 1 == db.ExecuteSqlNonQuery("DELETE FROM world_vehicle WHERE id=" + uid + " AND world_id=" + WorldId);
        }
        public override int QueryRemoveBodies(int time_limit)
        {
            return db.ExecuteSqlNonQuery("DELETE FROM survivor WHERE world_id=" + InstanceId + " AND is_dead=1 AND last_updated < now() - interval " + time_limit + " day");
        }
        public override bool QueryAddVehicle(bool instanceOrSpawn, string classname, int vehicle_id, Tool.Point position)
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

                        res = db.ExecuteSqlNonQuery("INSERT INTO instance_vehicle (`world_vehicle_id`, `instance_id`, `worldspace`, `inventory`, `parts`, `fuel`, `damage`, `created`) VALUES(" + wv_id + "," + InstanceId + "," + worldspace + "," + inventory + "," + parts + "," + fuel + "," + damage + ", now());");
                        return (res != 0);
                    }
                }

                return false;
            }

            /* else spawn point */
            res = db.ExecuteSqlNonQuery("INSERT INTO world_vehicle (`vehicle_id`, `world_id`, `worldspace`, `description`, `chance`) VALUES(" + vehicle_id + "," + WorldId + "," + worldspace + ",\"" + classname + "\", 0.7);");
            return (res != 0);
        }
        public override int QuerySpawnVehicles(int max_vehicles)
        {
            int res = 0;

            try
            {
                MySqlCommand cmd = db.Cnx.CreateCommand();
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
                    cmd.ExecuteNonQuery();
                }

                res = queries.Count;
            }
            catch
            {
                res = 0;
            }

            return res;
        }
        public override bool OnConnection()
        {
            bool bRes = true;

            DataSet _dsWorlds = new DataSet();
            DataSet _dsAllVehicleTypes = new DataSet();

            db.AccessDB(true);
            try
            {
                MySqlCommand cmd = db.Cnx.CreateCommand();
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
            catch
            {
                bRes = false;
            }
            db.AccessDB(false);

            if (bRes == false)
                return false;

            db.UseDS(true);
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
                                                       "", 0, 0, 0, 0, 0, 0, 0,
                                                       (UInt32)row.Field<Int32>("max_x"),
                                                       (UInt32)row.Field<Int32>("max_y"));
                    }
                    else
                    {
                        rowWD.SetField<string>("World Name", row.Field<string>("name"));
                    }
                }

                DataRow rowI = this.dsInstances.Tables[0].Rows.Find(InstanceId);
                if (rowI != null)
                {
                    WorldId = rowI.Field<UInt16>("world_id");

                    DataRow rowW = dsWorldDefs.Tables[0].Rows.Find(WorldId);
                    WorldName = (rowW != null) ? rowW.Field<string>("World Name") : "unknown";
                }
            }
            catch
            {
                bRes = true;
            }
            db.UseDS(false);

            return bRes;
        }
        public override bool Refresh()
        {
            bool bRes = true;

            DataSet _dsAlivePlayers = new DataSet();
            DataSet _dsDeadPlayers = new DataSet();
            DataSet _dsDeployables = new DataSet();
            DataSet _dsVehicles = new DataSet();
            DataSet _dsVehicleSpawnPoints = new DataSet();

            db.AccessDB(true);
            try
            {
                if (db.Connected)
                {
                    MySqlCommand cmd = db.Cnx.CreateCommand();
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
            }
            catch
            {
                bRes = false;
            }
            db.AccessDB(false);

            if (bRes == false)
                return false;

            db.UseDS(true);
            {
                foreach (DataRow row in _dsDeployables.Tables[0].Rows)
                {
                    string name = row.Field<string>("class_name");

                    if (dsDeployableTypes.Tables[0].Rows.Find(name) == null)
                        dsDeployableTypes.Tables[0].Rows.Add(name, "Unknown", true);
                }

                dsDeployables = _dsDeployables.Copy();
                dsAlivePlayers = _dsAlivePlayers.Copy();
                dsDeadPlayers = _dsDeadPlayers.Copy();
                dsVehicles = _dsVehicles.Copy();
                dsSpawnPoints = _dsVehicleSpawnPoints.Copy();
            }
            db.UseDS(false);

            return bRes;
        }
    }
    public class EpochSchema : DBSchema
    {
        public EpochSchema(myDatabase db) : base(db) { }

        public override string Type { get { return "Epoch"; } }
        public override string BuildQueryInstanceList()
        {
            return "SELECT Instance, COUNT(*) FROM object_data GROUP BY Instance";
        }
        public override bool QueryUpdatePlayerPosition(string worldspace, string uid)
        {
            return 1 == db.ExecuteSqlNonQuery("UPDATE character_data SET Worldspace=" + worldspace + " WHERE (PlayerUID='" + uid + "' AND Alive='1' AND InstanceID='" + InstanceId + "') ORDER BY CharacterID DESC LIMIT 1");
        }
        public override bool QueryHealPlayer(string uid)
        {
            return 1 == db.ExecuteSqlNonQuery("UPDATE character_data SET Medical='[false,false,false,false,false,false,true,12000,[],[0,0],0,[0,0]]' WHERE (PlayerUID='" + uid + "' AND Alive='1' AND InstanceID='" + InstanceId + "')");
        }
        public override bool QueryRevivePlayer(string uid, string char_id)
        {
            db.ExecuteSqlNonQuery("UPDATE character_data SET Alive='0' WHERE (PlayerUID='" + uid + "' AND Alive='1' AND InstanceID='" + InstanceId + "')");
            return 1 == db.ExecuteSqlNonQuery("UPDATE character_data SET Alive='1' WHERE (CharacterID='"+char_id+"' AND PlayerUID='" + uid + "' AND Alive='0' AND InstanceID='" + InstanceId + "')");
        }
        public override bool QuerySavePlayerState(string uid)
        {
            MySqlCommand cmd = db.Cnx.CreateCommand();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

            cmd.CommandText = "SELECT Inventory, Backpack FROM character_data WHERE (PlayerUID='" + uid + "' AND Alive='1' AND InstanceID=" + InstanceId + ")";

            DataSet _dsAlivePlayers = new DataSet();

            db.AccessDB(true);
            try
            {
                _dsAlivePlayers.Clear();
                adapter.Fill(_dsAlivePlayers);
            }
            catch
            {
            }
            db.AccessDB(false);

            db.UseDS(true);
            try
            {
                if (_dsAlivePlayers.Tables.Count == 1 && _dsAlivePlayers.Tables[0].Rows.Count == 1)
                {
                    DataRow to = (dsPlayerStates.Tables[0].Rows.Count > 0) ? dsPlayerStates.Tables[0].Rows.Find(uid) : null;
                    DataRow from = _dsAlivePlayers.Tables[0].Rows[0];
                    if (to == null)
                    {
                        dsPlayerStates.Tables[0].Rows.Add(uid, from.Field<string>("Inventory"), from.Field<string>("Backpack"));
                    }
                    else
                    {
                        to.SetField<string>("Inventory", from.Field<string>("Inventory"));
                        to.SetField<string>("Backpack", from.Field<string>("Backpack"));
                    }
                }
            }
            catch
            {
            }
            db.UseDS(false);

            return true;
        }
        public override bool QueryRestorePlayerState(string uid)
        {
            DataRow row = dsPlayerStates.Tables[0].Rows.Find(uid);
            if(row == null)
                return false;

            return 1 == db.ExecuteSqlNonQuery("UPDATE character_data SET Inventory='" + row.Field<string>("Inventory") + "', Backpack='" + row.Field<string>("Backpack") + "' WHERE (PlayerUID='" + uid + "' AND Alive='1' AND InstanceID='" + InstanceId + "')");
        }
        public override bool QueryRepairAndRefuel(string uid)
        {
            return 1 == db.ExecuteSqlNonQuery("UPDATE object_data SET Hitpoints='[]',Fuel='1',Damage='0' WHERE (ObjectID='" + uid + "' AND Instance='" + InstanceId + "')");
        }
        public override bool QueryDeleteVehicle(string uid)
        {
            return 1 == db.ExecuteSqlNonQuery("DELETE FROM object_data WHERE (ObjectID='" + uid + "' AND Instance='" + InstanceId + "')");
        }
        public override int QueryRemoveBodies(int time_limit)
        {
            return db.ExecuteSqlNonQuery("DELETE FROM character_data WHERE InstanceID='" + InstanceId + "' AND Alive='0' AND LastLogin < now() - interval " + time_limit + " day");
        }
        public override bool OnConnection()
        {
            bool bRes = true;

            // World ID not used in Epoch
            this.WorldId = -1;

            DataSet _dsVehicles = new DataSet();

            db.AccessDB(true);
            try
            {
                MySqlCommand cmd = db.Cnx.CreateCommand();
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                //
                //  Vehicle types
                //
                cmd.CommandText = "SELECT Classname class_name FROM object_data WHERE CharacterID=0 AND Instance=" + InstanceId;
                _dsVehicles.Clear();
                adapter.Fill(_dsVehicles);
            }
            catch
            {
                bRes = false;
            }
            db.AccessDB(false);

            if (bRes)
            {
                db.UseDS(true);
                try
                {
                    foreach (DataRow row in _dsVehicles.Tables[0].Rows)
                    {
                        string name = row.Field<string>("class_name");

                        if (dsVehicleTypes.Tables[0].Rows.Find(name) == null)
                            dsVehicleTypes.Tables[0].Rows.Add(name, "Car", true);
                    }
                }
                catch
                {
                    bRes = false;
                }
                db.UseDS(false);
            }

            return bRes;
        }
        public override bool Refresh()
        {
            bool bRes = true;

            if (db.Connected)
            {
                DataSet _dsAlivePlayers = new DataSet();
                DataSet _dsDeadPlayers = new DataSet();
                DataSet _dsDeployables = new DataSet();
                DataSet _dsVehicles = new DataSet();

                db.AccessDB(true);
                try
                {
                    MySqlCommand cmd = db.Cnx.CreateCommand();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    //
                    //  Players alive
                    //
                    {
                        cmd.CommandText = "SELECT cd.CharacterID id, pd.PlayerUID unique_id, pd.PlayerName name, cd.Humanity humanity, cd.worldspace worldspace,"
                                        + " cd.inventory inventory, cd.backpack backpack, cd.medical medical, cd.CurrentState state, cd.DateStamp last_updated"
                                        + " FROM character_data as cd, player_data as pd"
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
                                        + " FROM character_data as cd, player_data as pd"
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
                                    + " FROM object_data WHERE CharacterID=0"
                                    + " AND Instance=" + InstanceId
                                    + " AND Datestamp > now() - interval " + FilterLastUpdated + " day";
                    _dsVehicles.Clear();
                    adapter.Fill(_dsVehicles);

                    //
                    //  Deployables
                    //
                    cmd.CommandText = "SELECT CAST(ObjectID AS UNSIGNED) id, CharacterID keycode, Classname class_name, worldspace, inventory FROM object_data WHERE CharacterID!=0"
                                    + " AND Instance=" + InstanceId
                                    + " AND Datestamp > now() - interval " + FilterLastUpdated + " day";
                    _dsDeployables.Clear();
                    adapter.Fill(_dsDeployables);
                }
                catch
                {
                    bRes = false;
                }
                db.AccessDB(false);

                if (bRes == false)
                    return false;

                db.UseDS(true);
                {
                    foreach (DataRow row in _dsDeployables.Tables[0].Rows)
                    {
                        string name = row.Field<string>("class_name");

                        if (dsDeployableTypes.Tables[0].Rows.Find(name) == null)
                            dsDeployableTypes.Tables[0].Rows.Add(name, "Unknown", true);
                    }

                    dsDeployables = _dsDeployables.Copy();
                    dsAlivePlayers = _dsAlivePlayers.Copy();
                    dsDeadPlayers = _dsDeadPlayers.Copy();
                    dsVehicles = _dsVehicles.Copy();
                }
                db.UseDS(false);
            }

            return bRes;
        }
        public override bool QueryDeleteSpawn(string uid) { return true; }
        public override bool QueryAddVehicle(bool instanceOrSpawn, string classname, int vehicle_id, Tool.Point position) { return true; }
        public override int QuerySpawnVehicles(int max_vehicles) { return 0; }
    }
}
