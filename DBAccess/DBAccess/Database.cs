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
        public enum NameType
        {
            PlayerTable,
            CharTable,
            ObjTable,
            VehTable,
            CharId,
            PlayerId,
            Worldspace,
            Inventory,
            Backpack,
            Medical,
            Alive,
            State,
            Model
        }

        public int WorldId = 0;
        public int InstanceId = 0;
        public string WorldName = "";
        public int FilterLastUpdated = 0;

        public DBSchema(myDatabase db) { this.db = db; }

        public abstract string Type { get; }

        public abstract string BuildQueryInstanceList();

        public abstract bool QueryUpdateVehiclePosition(string worldspace, string uid);
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

        public abstract string Name(NameType type);

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
            public myServerConfig Cfg { get; set; }
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
                schema.dsVehicleTypes = login.Cfg.vehicle_types;
                schema.dsDeployableTypes = login.Cfg.deployable_types;
                schema.FilterLastUpdated = login.Cfg.filter_last_updated;
                schema.dsPlayerStates = login.Cfg.player_state;

                this.Connected = true;

                Connected = schema.OnConnection();
                    
                Refresh();

                loginAccepted = true;
            }
            catch (Exception ex)
            {
                this.Connected = false;
                if (MainWindow.IsDebug)
                    MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
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

                        QuerySelect query = new QuerySelect();
                        query.AddTable(table);
                        query.AddField("*");

                        cmd.CommandText = query.Build;
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
        public bool TeleportVehicle(string uid, Tool.Point position)
        {
            bool res = false;

            if (Connected)
            {
                string worldspace = "[0,[" + position.X.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + position.Y.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0.0015]]";
                res = schema.QueryUpdateVehiclePosition(worldspace, uid);
            }
            return res;
        }
        public bool TeleportPlayer(string uid, Tool.Point position)
        {
            bool res = false;

            if (Connected)
            {
                string worldspace = "[0,[" + position.X.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + position.Y.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0.0015]]";
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
        public override string Name(NameType type) { return "Null"; }
        public override string BuildQueryInstanceList() { return ""; }
        public override bool QueryUpdateVehiclePosition(string worldspace, string uid) { return true; }
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
        public override string Name(NameType type)
        {
            switch (type)
            {
                case NameType.PlayerTable: return "profile";
                case NameType.CharTable: return "survivor";
                case NameType.ObjTable: return "instance_deployable";
                case NameType.VehTable: return "instance_vehicle";
                case NameType.CharId: return "id";
                case NameType.PlayerId: return "unique_id";
                case NameType.Worldspace: return "worldspace";
                case NameType.Inventory: return "inventory";
                case NameType.Backpack: return "backpack";
                case NameType.Medical: return "medical";
                case NameType.Alive: return "is_dead";
                case NameType.State: return "state";
                case NameType.Model: return "model";
            }
            return "Unknown";
        }
        public override string BuildQueryInstanceList()
        {
            QuerySelect query = new QuerySelect();
            query.AddTable(Name(NameType.VehTable));
            query.AddField("instance_id");
            query.AddExtra("GROUP BY instance_id");

            return query.Build;
        }
        public override bool QueryUpdateVehiclePosition(string worldspace, string uid)
        {
            QueryUpdate q = new QueryUpdate();
            q.AddTable(Name(NameType.VehTable));
            q.AddField("worldspace", worldspace);
            q.AddCondition("id='" + uid + "'");
            q.AddCondition("instance_id='" + InstanceId + "'");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryUpdatePlayerPosition(string worldspace, string uid)
        {
            QueryUpdate q = new QueryUpdate();
            q.AddTable(Name(NameType.CharTable));
            q.AddField("worldspace", worldspace);
            q.AddCondition("world_id='" + WorldId + "'");
            q.AddCondition("unique_id='" + uid + "'");
            q.AddCondition("is_dead='0'");
            q.AddExtra("ORDER BY id DESC LIMIT 1");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryHealPlayer(string uid)
        {
            QueryUpdate q = new QueryUpdate();
            q.AddTable(Name(NameType.CharTable));
            q.AddField("medical", "[false,false,false,false,false,false,true,12000,[],[0,0],0,[0,0]]");
            q.AddCondition("unique_id='" + uid + "'");
            q.AddCondition("is_dead='0'");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryRevivePlayer(string uid, string char_id)
        {
            QueryUpdate q1 = new QueryUpdate();
            q1.AddTable(Name(NameType.CharTable));
            q1.AddField("is_dead", "1");
            q1.AddCondition("unique_id='" + uid + "'");
            q1.AddCondition("is_dead='0'");
            q1.AddCondition("world_id='" + WorldId + "'");

            QueryUpdate q2 = new QueryUpdate();
            q2.AddTable(Name(NameType.CharTable));
            q2.AddField("is_dead", "0");
            q2.AddCondition("id='" + char_id + "'");
            q2.AddCondition("unique_id='" + uid + "'");
            q2.AddCondition("is_dead='1'");
            q2.AddCondition("world_id='" + WorldId + "'");
            
            db.ExecuteSqlNonQuery(q1.Build);
            return 1 == db.ExecuteSqlNonQuery(q2.Build);
        }
        public override bool QuerySavePlayerState(string uid)
        {
            MySqlCommand cmd = db.Cnx.CreateCommand();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

            QuerySelect query = new QuerySelect();
            query.AddTable(Name(NameType.CharTable));
            query.AddField("inventory");
            query.AddField("backpack");
            query.AddField("state");
            query.AddField("model");
            query.AddCondition("unique_id='"+ uid + "'");
            query.AddCondition("is_dead='0'");
            query.AddCondition("world_id='"+ WorldId + "'");
            cmd.CommandText = query.Build;

            DataSet _dsPlayerState = new DataSet();

            db.AccessDB(true);
            try
            {
                _dsPlayerState.Clear();
                adapter.Fill(_dsPlayerState);
            }
            catch (Exception ex)
            {
                if (MainWindow.IsDebug)
                    MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
            }
            db.AccessDB(false);

            db.UseDS(true);
            try
            {
                if (_dsPlayerState.Tables.Count == 1 && _dsPlayerState.Tables[0].Rows.Count == 1)
                {
                    DataRow to = (dsPlayerStates.Tables[0].Rows.Count > 0) ? dsPlayerStates.Tables[0].Rows.Find(uid) : null;
                    DataRow from = _dsPlayerState.Tables[0].Rows[0];
                    if (to == null)
                    {
                        dsPlayerStates.Tables[0].Rows.Add(uid, from.Field<string>("inventory"), from.Field<string>("backpack"), from.Field<string>("state"), from.Field<string>("model"));
                    }
                    else
                    {
                        to.SetField<string>("Inventory", from.Field<string>("inventory"));
                        to.SetField<string>("Backpack", from.Field<string>("backpack"));
                        to.SetField<string>("State", from.Field<string>("state"));
                        to.SetField<string>("Model", from.Field<string>("model"));
                    }
                }
            }
            catch (Exception ex)
            {
                if (MainWindow.IsDebug)
                    MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
            }
            db.UseDS(false);

            return true;
        }
        public override bool QueryRestorePlayerState(string uid)
        {
            DataRow row = dsPlayerStates.Tables[0].Rows.Find(uid);
            if (row == null)
                return false;

            QueryUpdate q = new QueryUpdate();
            q.AddTable(Name(NameType.CharTable));
            q.AddFieldFromRow("inventory", row);
            q.AddFieldFromRow("backpack", row);
            q.AddFieldFromRow("state", row);
            q.AddFieldFromRow("model", row);
            q.AddCondition("unique_id='" + uid + "'");
            q.AddCondition("is_dead='0'");
            q.AddCondition("world_id='" + WorldId + "'");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryRepairAndRefuel(string uid)
        {
            QueryUpdate q = new QueryUpdate();
            q.AddTable(Name(NameType.VehTable));
            q.AddField("parts", "[]");
            q.AddField("fuel", "1");
            q.AddField("damage", "0");
            q.AddCondition("id='" + uid + "'");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryDeleteVehicle(string uid)
        {
            QueryDelete q = new QueryDelete();
            q.AddTable(Name(NameType.VehTable));
            q.AddCondition("id='" + uid + "'");
            q.AddCondition("instance_id='" + InstanceId + "'");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryDeleteSpawn(string uid)
        {
            QueryDelete q = new QueryDelete();
            q.AddTable("world_vehicle");
            q.AddCondition("id='" + uid + "'");
            q.AddCondition("world_id='" + WorldId + "'");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override int QueryRemoveBodies(int time_limit)
        {
            QueryDelete q = new QueryDelete();
            q.AddTable(Name(NameType.CharTable));
            q.AddCondition("world_id='" + InstanceId + "'");
            q.AddCondition("is_dead='1'");
            q.AddCondition("last_updated < now() - interval " + time_limit + " day");

            return db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryAddVehicle(bool instanceOrSpawn, string classname, int vehicle_id, Tool.Point position)
        {
            int res;
            string worldspace = "[0,[" + position.X.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + position.Y.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0.0015]]";

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

                        var q = new QueryInsert();
                        q.AddTable(Name(NameType.VehTable));
                        q.AddField("world_vehicle_id", wv_id.ToString());
                        q.AddField("worldspace", worldspace);
                        q.AddField("inventory", "[]");
                        q.AddField("parts", "[]");
                        q.AddField("damage", damage.ToString());
                        q.AddField("fuel", fuel.ToString());
                        q.AddField("instance_id", InstanceId.ToString());
                        q.AddField("created", "now()", false);

                        res = db.ExecuteSqlNonQuery(q.Build);
                        return (res != 0);
                    }
                }

                return false;
            }

            /* else spawn point */

            var q2 = new QueryInsert();
            q2.AddTable("world_vehicle");
            q2.AddField("vehicle_id", vehicle_id.ToString());
            q2.AddField("world_id", WorldId.ToString());
            q2.AddField("worldspace", worldspace);
            q2.AddField("description", classname);
            q2.AddField("chance", "0.7");

            res = db.ExecuteSqlNonQuery(q2.Build);
            return (res != 0);
        }
        public override int QuerySpawnVehicles(int max_vehicles)
        {
            int res = 0;

            try
            {
                MySqlCommand cmd = db.Cnx.CreateCommand();
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                QuerySelect q = new QuerySelect();
                q.AddTable(Name(NameType.VehTable));
                q.AddField("count(*)");
                q.AddCondition("instance_id='" + InstanceId + "'");
                cmd.CommandText = q.Build;

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

                    var q2 = new QueryInsert();
                    q2.AddTable(Name(NameType.VehTable));
                    q2.AddField("world_vehicle_id", world_vehicle_id.ToString());
                    q2.AddField("worldspace", worldspace);
                    q2.AddField("inventory", inventory);
                    q2.AddField("parts", health.ToString());
                    q2.AddField("damage", damage.ToString());
                    q2.AddField("fuel", fuel.ToString());
                    q2.AddField("instance_id", InstanceId.ToString());
                    q2.AddField("created", "current_timestamp()", false);

                    queries.Add(q2.Build);

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
            catch (Exception ex)
            {
                res = 0;
                if (MainWindow.IsDebug)
                    MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
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
                {
                    QuerySelect q = new QuerySelect();
                    q.AddTable("instance");
                    q.AddField("*");
                    cmd.CommandText = q.Build;

                    this.dsInstances.Clear();
                    adapter.Fill(this.dsInstances);
                    DataColumn[] keysI = new DataColumn[1];
                    keysI[0] = dsInstances.Tables[0].Columns[0];
                    dsInstances.Tables[0].PrimaryKey = keysI;
                }

                //
                //  Worlds
                //
                {
                    QuerySelect q = new QuerySelect();
                    q.AddTable("world");
                    q.AddField("*");
                    cmd.CommandText = q.Build;

                    _dsWorlds.Clear();
                    adapter.Fill(_dsWorlds);
                }

                //
                //  Vehicle types
                //
                {
                    QuerySelect q = new QuerySelect();
                    q.AddTable("vehicle");
                    q.AddField("id");
                    q.AddField("class_name");
                    cmd.CommandText = q.Build;

                    _dsAllVehicleTypes.Clear();
                    adapter.Fill(_dsAllVehicleTypes);
                    DataColumn[] keysV = new DataColumn[1];
                    keysV[0] = _dsAllVehicleTypes.Tables[0].Columns[0];
                    _dsAllVehicleTypes.Tables[0].PrimaryKey = keysV;
                }
            }
            catch (Exception ex)
            {
                bRes = false;
                if (MainWindow.IsDebug)
                    MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
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
            catch (Exception ex)
            {
                bRes = true;
                if (MainWindow.IsDebug)
                    MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
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
                        QuerySelect query = new QuerySelect();
                        query.AddTable("survivor","s");
                        query.AddTable("profile", "p");
                        query.AddField("s.id", "id");
                        query.AddField("s.unique_id", "unique_id");
                        query.AddField("p.name", "name");
                        query.AddField("p.humanity", "humanity");
                        query.AddField("s.worldspace", "worldspace");
                        query.AddField("s.inventory", "inventory");
                        query.AddField("s.backpack", "backpack");
                        query.AddField("s.medical", "medical");
                        query.AddField("s.state", "state");
                        query.AddField("s.last_updated", "last_updated");
                        query.AddCondition("s.unique_id=p.unique_id");
                        query.AddCondition("s.world_id='" + WorldId + "'");
                        query.AddCondition("s.is_dead='0'");
                        query.AddCondition("s.last_updated > now() - interval " + FilterLastUpdated + " day");
                        cmd.CommandText = query.Build;

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
                        QuerySelect query = new QuerySelect();
                        query.AddTable("survivor", "s");
                        query.AddTable("profile", "p");
                        query.AddField("s.id", "id");
                        query.AddField("s.unique_id", "unique_id");
                        query.AddField("p.name", "name");
                        query.AddField("p.humanity", "humanity");
                        query.AddField("s.worldspace", "worldspace");
                        query.AddField("s.inventory", "inventory");
                        query.AddField("s.backpack", "backpack");
                        query.AddField("s.medical", "medical");
                        query.AddField("s.state", "state");
                        query.AddField("s.last_updated", "last_updated");
                        query.AddCondition("s.unique_id=p.unique_id");
                        query.AddCondition("s.world_id='" + WorldId + "'");
                        query.AddCondition("s.is_dead='1'");
                        query.AddCondition("s.last_updated > now() - interval " + FilterLastUpdated + " day");
                        cmd.CommandText = query.Build;

                        _dsDeadPlayers.Clear();
                        adapter.Fill(_dsDeadPlayers);
                        DataColumn[] keys = new DataColumn[1];
                        keys[0] = _dsDeadPlayers.Tables[0].Columns[0];
                        _dsDeadPlayers.Tables[0].PrimaryKey = keys;
                    }

                    //
                    //  Vehicles
                    //
                    {
                        QuerySelect query = new QuerySelect();
                        query.AddTable("vehicle", "v");
                        query.AddTable("world_vehicle", "wv");
                        query.AddTable(Name(NameType.VehTable), "iv");
                        query.AddField("iv.id", "id");
                        query.AddField("wv.id", "spawn_id");
                        query.AddField("v.class_name", "class_name");
                        query.AddField("iv.worldspace", "worldspace");
                        query.AddField("iv.inventory", "inventory");
                        query.AddField("iv.fuel", "fuel");
                        query.AddField("iv.damage", "damage");
                        query.AddField("iv.last_updated", "last_updated");
                        query.AddField("iv.parts", "parts");
                        query.AddCondition("iv.instance_id=" + InstanceId);
                        query.AddCondition("s.world_id='" + WorldId + "'");
                        query.AddCondition("iv.world_vehicle_id=wv.id");
                        query.AddCondition("wv.vehicle_id=v.id");
                        query.AddCondition("iv.last_updated > now() - interval " + FilterLastUpdated + " day");
                        cmd.CommandText = query.Build;

                        _dsVehicles.Clear();
                        adapter.Fill(_dsVehicles);
                    }
                    //
                    //  Vehicle Spawn points
                    //
                    {
                        QuerySelect query = new QuerySelect();
                        query.AddTable("world_vehicle", "w");
                        query.AddTable("vehicle", "v");
                        query.AddField("w.id", "id");
                        query.AddField("w.vehicle_id", "vid");
                        query.AddField("w.worldspace", "worldspace");
                        query.AddField("v.inventory", "inventory");
                        query.AddField("w.chance", "chance");
                        query.AddField("v.class_name", "class_name");
                        query.AddCondition("w.world_id='" + WorldId + "'");
                        query.AddCondition("w.vehicle_id=v.id");
                        cmd.CommandText = query.Build;

                        _dsVehicleSpawnPoints.Clear();
                        adapter.Fill(_dsVehicleSpawnPoints);
                    }

                    //
                    //  Deployables
                    //
                    {
                        QuerySelect query = new QuerySelect();
                        query.AddTable(Name(NameType.ObjTable), "id");
                        query.AddTable("deployable", "d");
                        query.AddField("id.id", "id");
                        query.AddField("CAST(0 AS UNSIGNED)", "keycode");
                        query.AddField("d.class_name", "class_name");
                        query.AddField("id.worldspace", "worldspace");
                        query.AddField("id.inventory", "inventory");
                        query.AddCondition("instance_id=" + InstanceId);
                        query.AddCondition("id.deployable_id=d.id");
                        query.AddCondition("id.last_updated > now() - interval " + FilterLastUpdated + " day");
                        cmd.CommandText = query.Build;

                        _dsDeployables.Clear();
                        adapter.Fill(_dsDeployables);
                    }
                }
            }
            catch (Exception ex)
            {
                bRes = false;
                if (MainWindow.IsDebug)
                    MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
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
        public override string Name(NameType type)
        {
            switch (type)
            {
                case NameType.PlayerTable: return "player_data";
                case NameType.CharTable: return "character_data";
                case NameType.ObjTable:
                case NameType.VehTable: return "object_data";
                case NameType.CharId: return "CharacterID";
                case NameType.PlayerId: return "PlayerUID";
                case NameType.Worldspace: return "Worldspace";
                case NameType.Inventory: return "Inventory";
                case NameType.Backpack: return "Backpack";
                case NameType.Medical: return "Medical";
                case NameType.Alive: return "Alive";
                case NameType.State: return "CurrentState";
                case NameType.Model: return "Model";
            }
            return "Unknown";
        }

        public override string BuildQueryInstanceList()
        {
            QuerySelect q = new QuerySelect();
            q.AddTable(Name(NameType.ObjTable));
            q.AddField("Instance");
            q.AddField("COUNT(*)");
            q.AddExtra("GROUP BY Instance");

            return q.Build;
        }
        public override bool QueryUpdateVehiclePosition(string worldspace, string uid)
        {
            QueryUpdate q = new QueryUpdate();
            q.AddTable(Name(NameType.ObjTable));
            q.AddField("Worldspace", worldspace);
            q.AddCondition("ObjectID='" + uid + "'");
            q.AddCondition("Instance='" + InstanceId + "'");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryUpdatePlayerPosition(string worldspace, string uid)
        {
            QueryUpdate q = new QueryUpdate();
            q.AddTable(Name(NameType.CharTable));
            q.AddField("Worldspace", worldspace);
            q.AddCondition("PlayerUID='" + uid + "'");
            q.AddCondition("Alive='1'");
            q.AddCondition("InstanceID='" + InstanceId + "'");
            q.AddExtra("ORDER BY CharacterID DESC LIMIT 1");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryHealPlayer(string uid)
        {
            QueryUpdate q = new QueryUpdate();
            q.AddTable(Name(NameType.CharTable));
            q.AddField("Medical", "[false,false,false,false,false,false,true,12000,[],[0,0],0,[0,0]]");
            q.AddCondition("PlayerUID='" + uid + "'");
            q.AddCondition("Alive='1'");
            q.AddCondition("InstanceID='" + InstanceId + "'");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryRevivePlayer(string uid, string char_id)
        {
            QueryUpdate q1 = new QueryUpdate();
            q1.AddTable(Name(NameType.CharTable));
            q1.AddField("Alive", "0");
            q1.AddCondition("PlayerUID='" + uid + "'");
            q1.AddCondition("Alive='1'");
            q1.AddCondition("InstanceID='" + InstanceId + "'");

            QueryUpdate q2 = new QueryUpdate();
            q2.AddTable(Name(NameType.CharTable));
            q2.AddField("Alive", "1");
            q2.AddField("Medical", "[false,false,false,false,false,false,true,12000,[],[0,0],0,[0,0]]");
            q2.AddCondition("CharacterID='" + char_id + "'");
            q2.AddCondition("PlayerUID='" + uid + "'");
            q2.AddCondition("Alive='0'");
            q2.AddCondition("InstanceID='" + InstanceId + "'");

            db.ExecuteSqlNonQuery(q1.Build);
            return 1 == db.ExecuteSqlNonQuery(q2.Build);
        }
        public override bool QuerySavePlayerState(string uid)
        {
            MySqlCommand cmd = db.Cnx.CreateCommand();
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

            QuerySelect q = new QuerySelect();
            q.AddTable(Name(NameType.CharTable));
            q.AddField("Inventory");
            q.AddField("Backpack");
            q.AddField("CurrentState");
            q.AddField("Model");
            q.AddCondition("PlayerUID='" + uid + "'");
            q.AddCondition("Alive='1'");
            q.AddCondition("InstanceID='" + InstanceId + "'");
            cmd.CommandText = q.Build;

            DataSet _dsAlivePlayers = new DataSet();

            db.AccessDB(true);
            try
            {
                _dsAlivePlayers.Clear();
                adapter.Fill(_dsAlivePlayers);
            }
            catch (Exception ex)
            {
                if (MainWindow.IsDebug)
                    MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
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
                        dsPlayerStates.Tables[0].Rows.Add(uid, from.Field<string>("Inventory"), from.Field<string>("Backpack"), from.Field<string>("CurrentState"), from.Field<string>("Model"));
                    }
                    else
                    {
                        to.SetField<string>("Inventory", from.Field<string>("Inventory"));
                        to.SetField<string>("Backpack", from.Field<string>("Backpack"));
                        to.SetField<string>("State", from.Field<string>("CurrentState"));
                        to.SetField<string>("Model", from.Field<string>("Model"));
                    }
                }
            }
            catch(Exception ex)
            {
                if (MainWindow.IsDebug)
                    MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
            }
            db.UseDS(false);

            return true;
        }
        public override bool QueryRestorePlayerState(string uid)
        {
            DataRow row = dsPlayerStates.Tables[0].Rows.Find(uid);
            if(row == null)
                return false;

            QueryUpdate q = new QueryUpdate();
            q.AddTable(Name(NameType.CharTable));
            q.AddFieldFromRow("Inventory", row);
            q.AddFieldFromRow("Backpack", row);
            q.AddFieldFromRow("CurrentState", row, "State");
            q.AddFieldFromRow("Model", row);
            q.AddCondition("PlayerUID='" + uid + "'");
            q.AddCondition("Alive='1'");
            q.AddCondition("InstanceID='" + InstanceId + "'");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryRepairAndRefuel(string uid)
        {
            QueryUpdate q = new QueryUpdate();
            q.AddTable(Name(NameType.ObjTable));
            q.AddField("Hitpoints", "[]");
            q.AddField("Fuel", "1");
            q.AddField("Damage", "0");
            q.AddCondition("ObjectID='" + uid + "'");
            q.AddCondition("Instance='" + InstanceId + "'");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override bool QueryDeleteVehicle(string uid)
        {
            QueryDelete q = new QueryDelete();
            q.AddTable(Name(NameType.ObjTable));
            q.AddCondition("ObjectID='" + uid + "'");
            q.AddCondition("Instance='" + InstanceId + "'");

            return 1 == db.ExecuteSqlNonQuery(q.Build);
        }
        public override int QueryRemoveBodies(int time_limit)
        {
            QueryDelete q = new QueryDelete();
            q.AddTable(Name(NameType.CharTable));
            q.AddCondition("InstanceID='" + InstanceId + "'");
            q.AddCondition("Alive='0'");
            q.AddCondition("LastLogin < now() - interval " + time_limit + " day");

            return db.ExecuteSqlNonQuery(q.Build);
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
                QuerySelect q = new QuerySelect();
                q.AddTable(Name(NameType.ObjTable));
                q.AddField("Classname", "class_name");
                q.AddCondition("CharacterID='0'");
                q.AddCondition("Instance='" + InstanceId + "'");
                cmd.CommandText = q.Build;

                _dsVehicles.Clear();
                adapter.Fill(_dsVehicles);
            }
            catch (Exception ex)
            {
                bRes = false;
                if (MainWindow.IsDebug)
                    MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
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
                catch (Exception ex)
                {
                    bRes = false;
                    if (MainWindow.IsDebug)
                        MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
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
                        QuerySelect q = new QuerySelect();
                        q.AddTable(Name(NameType.CharTable), "cd");
                        q.AddTable(Name(NameType.PlayerTable), "pd");
                        q.AddField("cd.CharacterID", "id");
                        q.AddField("pd.PlayerUID", "unique_id");
                        q.AddField("pd.PlayerName", "name");
                        q.AddField("cd.Humanity", "humanity");
                        q.AddField("cd.worldspace", "worldspace");
                        q.AddField("cd.inventory", "inventory");
                        q.AddField("cd.backpack", "backpack");
                        q.AddField("cd.medical", "medical");
                        q.AddField("cd.CurrentState", "state");
                        q.AddField("cd.DateStamp", "last_updated");
                        q.AddCondition("cd.PlayerUID=pd.PlayerUID");
                        q.AddCondition("cd.Alive='1'");
                        q.AddCondition("cd.InstanceID='" + InstanceId + "'");
                        q.AddCondition("cd.LastLogin > now() - interval " + FilterLastUpdated + " day");
                        cmd.CommandText = q.Build;

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
                        QuerySelect q = new QuerySelect();
                        q.AddTable(Name(NameType.CharTable), "cd");
                        q.AddTable(Name(NameType.PlayerTable), "pd");
                        q.AddField("cd.CharacterID", "id");
                        q.AddField("pd.PlayerUID", "unique_id");
                        q.AddField("pd.PlayerName", "name");
                        q.AddField("cd.Humanity", "humanity");
                        q.AddField("cd.worldspace", "worldspace");
                        q.AddField("cd.inventory", "inventory");
                        q.AddField("cd.backpack", "backpack");
                        q.AddField("cd.medical", "medical");
                        q.AddField("cd.CurrentState", "state");
                        q.AddField("cd.DateStamp", "last_updated");
                        q.AddCondition("cd.PlayerUID=pd.PlayerUID");
                        q.AddCondition("cd.Alive='0'");
                        q.AddCondition("cd.InstanceID='" + InstanceId + "'");
                        q.AddCondition("cd.LastLogin > now() - interval " + FilterLastUpdated + " day");
                        cmd.CommandText = q.Build;

                        _dsDeadPlayers.Clear();
                        adapter.Fill(_dsDeadPlayers);
                        DataColumn[] keys = new DataColumn[1];
                        keys[0] = _dsDeadPlayers.Tables[0].Columns[0];
                        _dsDeadPlayers.Tables[0].PrimaryKey = keys;
                    }

                    //
                    //  Vehicles
                    {
                        QuerySelect q = new QuerySelect();
                        q.AddTable(Name(NameType.ObjTable));
                        q.AddField("CAST(ObjectID AS UNSIGNED)", "id");
                        q.AddField("CAST(0 AS UNSIGNED)", "spawn_id");
                        q.AddField("ClassName", "class_name");
                        q.AddField("worldspace");
                        q.AddField("inventory");
                        q.AddField("Hitpoints", "parts");
                        q.AddField("fuel");
                        q.AddField("damage");
                        q.AddField("DateStamp", "last_updated");
                        q.AddCondition("CharacterID='0'");
                        q.AddCondition("Instance='" + InstanceId + "'");
                        q.AddCondition("Datestamp > now() - interval " + FilterLastUpdated + " day");
                        cmd.CommandText = q.Build;

                        _dsVehicles.Clear();
                        adapter.Fill(_dsVehicles);
                    }

                    //
                    //  Deployables
                    //
                    {
                        QuerySelect q = new QuerySelect();
                        q.AddTable(Name(NameType.ObjTable));
                        q.AddField("CAST(ObjectID AS UNSIGNED)", "id");
                        q.AddField("CharacterID", "keycode");
                        q.AddField("ClassName", "class_name");
                        q.AddField("worldspace");
                        q.AddField("inventory");
                        q.AddCondition("CharacterID!='0'");
                        q.AddCondition("Instance='" + InstanceId + "'");
                        q.AddCondition("Datestamp > now() - interval " + FilterLastUpdated + " day");
                        cmd.CommandText = q.Build;

                        _dsDeployables.Clear();
                        adapter.Fill(_dsDeployables);
                    }
                }
                catch (Exception ex)
                {
                    bRes = false;
                    if (MainWindow.IsDebug)
                        MessageBox.Show(ex.Message, "DATABASE EXCEPTION");
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
    public abstract class QueryBase
    {
        public string Build
        {
            get
            {
                System.Diagnostics.Debug.Assert(fields.Count > 0, "Query needs fields");
                System.Diagnostics.Debug.Assert(!needsFROM || from.Count > 0, "Query needs a FROM clause");
                System.Diagnostics.Debug.Assert(!needsWHERE || conditions.Count > 0, "Query needs a WHERE clause");

                return SpecBuild + extra;
            }
        }

        public void AddTable(string item, string value = null) { from.Add(item, value); }
        public void AddField(string item, string value = null) { fields.Add(item, (value != null) ? new CField(value) : null); }
        public void AddField(string item, string value, bool quotify) { fields.Add(item, new CField(value, quotify)); }
        public void AddFieldFromRow(string item, DataRow row, string colName=null) { fields.Add(item, new CField(row.Field<string>((colName!=null)?colName:item))); }
        public void AddCondition(string value) { conditions.Add(value); }
        public void AddExtra(string value) { extra += " " + value; }

        //
        //
        //
        protected abstract string SpecBuild { get; }
        protected virtual bool needsFROM { get { return false; } }
        protected virtual bool needsWHERE { get { return false; } }

        //
        //
        //
        protected class CField
        {
            public CField(string value, bool quotify = true) { Value = value; Quotify = quotify; }
            public string Value { get; set; }
            public bool Quotify { get; set; }
        }

        protected Dictionary<string, string> from = new Dictionary<string,string>();
        protected Dictionary<string, CField> fields = new Dictionary<string, CField>();
        protected List<string> conditions = new List<string>();
        protected string extra = "";
    }
    public class QuerySelect : QueryBase
    {
        protected override string SpecBuild
        {
            get
            {
                string result = "SELECT ";

                int counter = 0;
                foreach (KeyValuePair<string, CField> pair in fields)
                {
                    counter++;
                    result += pair.Key;
                    if (pair.Value != null)
                        result += " " + pair.Value.Value;
                    if (counter != fields.Count) result += ", ";
                }

                result += " FROM ";
                counter = 0;
                foreach (KeyValuePair<string, string> pair in from)
                {
                    counter++;
                    result += "`" + pair.Key + "`";
                    if (pair.Value != null) result += " " + pair.Value;
                    if (counter != from.Count) result += ", ";
                }

                if (conditions.Count > 0)
                {
                    result += " WHERE (";
                    counter = 0;
                    foreach (string value in conditions)
                    {
                        counter++;
                        result += value;
                        if (counter != conditions.Count) result += " AND ";
                    }
                    result += ")";
                }

                return result;
            }
        }

        protected override bool needsFROM { get { return true; } }
    }
    public class QueryUpdate : QueryBase
    {
        protected override string SpecBuild
        {
            get
            {
                string result = "UPDATE ";

                int counter = 0;
                foreach (KeyValuePair<string, string> pair in from)
                {
                    counter++;
                    result += "`" + pair.Key + "`";
                    if (counter != from.Count) result += ", ";
                }

                result += " SET ";

                counter = 0;
                foreach (KeyValuePair<string, CField> pair in fields)
                {
                    counter++;
                    result += pair.Key + "=";
                    string quote = (pair.Value.Quotify) ? "'" : "";
                    result += quote + pair.Value.Value + quote;
                    if (counter != fields.Count) result += ", ";
                }

                result += " WHERE (";
                counter = 0;
                foreach (string value in conditions)
                {
                    counter++;
                    result += value;
                    if (counter != conditions.Count) result += " AND ";
                }
                result += ")";

                return result;
            }
        }

        protected override bool needsFROM { get { return true; } }
        protected override bool needsWHERE { get { return true; } }
    }
    public class QueryDelete : QueryBase
    {
        protected override string SpecBuild
        {
            get
            {
                string result = "DELETE FROM ";

                int counter = 0;
                foreach (KeyValuePair<string, string> pair in from)
                {
                    counter++;
                    result += "`" + pair.Key + "`";
                    if (counter != from.Count) result += ", ";
                }

                result += " WHERE (";
                counter = 0;
                foreach (string value in conditions)
                {
                    counter++;
                    result += value;
                    if (counter != conditions.Count) result += " AND ";
                }
                result += ")";

                return result;
            }
        }

        protected override bool needsFROM { get { return true; } }
        protected override bool needsWHERE { get { return true; } }
    }
    public class QueryInsert : QueryBase
    {
        protected override string SpecBuild
        {
            get
            {
                string result = "INSERT INTO ";

                int counter = 0;
                foreach (KeyValuePair<string, string> pair in from)
                {
                    counter++;
                    result += "`" + pair.Key + "`";
                    if (counter != from.Count) result += ", ";
                }

                result += " (";
                counter = 0;
                foreach (KeyValuePair<string, CField> pair in fields)
                {
                    counter++;
                    result += "'" + pair.Key + "'";
                    if (counter != fields.Count) result += ", ";
                }
                result += ")";

                result += " VALUES(";
                counter = 0;
                foreach (KeyValuePair<string, CField> pair in fields)
                {
                    counter++;
                    string quote = (pair.Value.Quotify) ? "'" : "";
                    result += quote + pair.Value.Value + quote;
                    if (counter != fields.Count) result += ", ";
                }
                result += ")";

                return result;
            }
        }
    }
}
