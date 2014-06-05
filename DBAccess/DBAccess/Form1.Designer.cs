namespace DBAccess
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.contextMenuStripMapMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuMapAddVehicle = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuMapTeleportPlayer = new System.Windows.Forms.ToolStripMenuItem();
            this.dataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.contextMenuStripResetTypes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemResetTypes = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripSpawn = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemDeleteSpawn = new System.Windows.Forms.ToolStripMenuItem();
            this.bgWorkerDatabase = new System.ComponentModel.BackgroundWorker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStripVehicle = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.repairRefuelVehicleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDeleteVehicle = new System.Windows.Forms.ToolStripMenuItem();
            this.bgWorkerMapZoom = new System.ComponentModel.BackgroundWorker();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusWorld = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusOnline = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusAlive = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusDead = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusVehicle = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusSpawn = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusDeployable = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusMapHelper = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusTrail = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusChat = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusCoordMap = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusCoordDB = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusNews = new System.Windows.Forms.ToolStripStatusLabel();
            this.ttootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPlayersAliveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showVehiclesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSpawnPointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDeployablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapHelperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cartographerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showTrailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabelWorld = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripMenuItemOnline = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAlive = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemVehicle = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSpawn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDeployable = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMapHelper = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCartographer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemTrails = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabelCnx = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer2 = new System.Windows.Forms.ToolStripContainer();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.contextMenuPlayersOnline = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.messageToPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kickPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer3 = new System.Windows.Forms.ToolStripContainer();
            this.bgWorkerLoadTiles = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStripPlayerMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemHeal = new System.Windows.Forms.ToolStripMenuItem();
            this.bgWorkerFocus = new System.ComponentModel.BackgroundWorker();
            this.splitContainerGlobal = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new DBAccess.MySplitContainer();
            this.panelCnx = new System.Windows.Forms.Panel();
            this.groupBoxrCon = new System.Windows.Forms.GroupBox();
            this.numericUpDownrConPort = new System.Windows.Forms.NumericUpDown();
            this.textBoxrConPassword = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxrConURL = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBoxDB = new System.Windows.Forms.GroupBox();
            this.numericUpDownDBPort = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownInstanceId = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxGameType = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDBURL = new System.Windows.Forms.TextBox();
            this.textBoxDBPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDBUser = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxDBBaseName = new System.Windows.Forms.TextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.dataGridViewMaps = new System.Windows.Forms.DataGridView();
            this.ColGVMID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColGVMName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColGVMChoosePath = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColGVMPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageDisplay = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.labelMagLevel = new System.Windows.Forms.Label();
            this.trackBarMagLevel = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.labelLastUpdate = new System.Windows.Forms.Label();
            this.trackBarLastUpdated = new System.Windows.Forms.TrackBar();
            this.groupBoxInfo = new System.Windows.Forms.GroupBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabPageScripts = new System.Windows.Forms.TabPage();
            this.buttonSelectCustom3 = new System.Windows.Forms.Button();
            this.buttonSelectCustom2 = new System.Windows.Forms.Button();
            this.buttonSelectCustom1 = new System.Windows.Forms.Button();
            this.buttonCustom3 = new System.Windows.Forms.Button();
            this.buttonCustom2 = new System.Windows.Forms.Button();
            this.buttonCustom1 = new System.Windows.Forms.Button();
            this.textBoxOldTentLimit = new System.Windows.Forms.TextBox();
            this.buttonRemoveTents = new System.Windows.Forms.Button();
            this.textBoxCmdStatus = new System.Windows.Forms.TextBox();
            this.textBoxOldBodyLimit = new System.Windows.Forms.TextBox();
            this.textBoxVehicleMax = new System.Windows.Forms.TextBox();
            this.buttonRemoveBodies = new System.Windows.Forms.Button();
            this.buttonSpawnNew = new System.Windows.Forms.Button();
            this.buttonBackup = new System.Windows.Forms.Button();
            this.buttonRemoveDestroyed = new System.Windows.Forms.Button();
            this.tabPageVehicles = new System.Windows.Forms.TabPage();
            this.dataGridViewVehicleTypes = new System.Windows.Forms.DataGridView();
            this.ColGVVTShow = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColGVVTClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColGVVTType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tabPageDeployables = new System.Windows.Forms.TabPage();
            this.dataGridViewDeployableTypes = new System.Windows.Forms.DataGridView();
            this.ColGVDTShow = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColGVDTClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColGVDTType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tabPagePlayers = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dataGridViewPlayers = new System.Windows.Forms.DataGridView();
            this.dataGridViewAdmins = new System.Windows.Forms.DataGridView();
            this.tabPageSetup = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonReconnectrCon = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericDBRefreshRate = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.comboSelectInstance = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboSelectEpochWorld = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cbCartographer = new System.Windows.Forms.CheckBox();
            this.splitContainerChat = new System.Windows.Forms.SplitContainer();
            this.richTextBoxChat = new System.Windows.Forms.RichTextBox();
            this.textBoxChatInput = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetBindingSource)).BeginInit();
            this.contextMenuStripResetTypes.SuspendLayout();
            this.contextMenuStripSpawn.SuspendLayout();
            this.contextMenuStripVehicle.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer2.ContentPanel.SuspendLayout();
            this.toolStripContainer2.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.contextMenuPlayersOnline.SuspendLayout();
            this.toolStripContainer3.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer3.ContentPanel.SuspendLayout();
            this.toolStripContainer3.SuspendLayout();
            this.contextMenuStripPlayerMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerGlobal)).BeginInit();
            this.splitContainerGlobal.Panel1.SuspendLayout();
            this.splitContainerGlobal.Panel2.SuspendLayout();
            this.splitContainerGlobal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelCnx.SuspendLayout();
            this.groupBoxrCon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownrConPort)).BeginInit();
            this.groupBoxDB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDBPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInstanceId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMaps)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMagLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLastUpdated)).BeginInit();
            this.groupBoxInfo.SuspendLayout();
            this.tabPageScripts.SuspendLayout();
            this.tabPageVehicles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVehicleTypes)).BeginInit();
            this.tabPageDeployables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDeployableTypes)).BeginInit();
            this.tabPagePlayers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPlayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAdmins)).BeginInit();
            this.tabPageSetup.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericDBRefreshRate)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerChat)).BeginInit();
            this.splitContainerChat.Panel1.SuspendLayout();
            this.splitContainerChat.Panel2.SuspendLayout();
            this.splitContainerChat.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.AutoScroll = true;
            this.ContentPanel.Size = new System.Drawing.Size(791, 553);
            // 
            // contextMenuStripMapMenu
            // 
            this.contextMenuStripMapMenu.Name = "contextMenuStripItemMenu";
            this.contextMenuStripMapMenu.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStripMapMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripItemMenu_Opening);
            // 
            // toolStripMenuMapAddVehicle
            // 
            this.toolStripMenuMapAddVehicle.Name = "toolStripMenuMapAddVehicle";
            this.toolStripMenuMapAddVehicle.Size = new System.Drawing.Size(162, 22);
            this.toolStripMenuMapAddVehicle.Text = "Add SpawnPoint";
            this.toolStripMenuMapAddVehicle.Click += new System.EventHandler(this.toolStripMenuItemAddVehicle_Click);
            // 
            // toolStripMenuMapTeleportPlayer
            // 
            this.toolStripMenuMapTeleportPlayer.Name = "toolStripMenuMapTeleportPlayer";
            this.toolStripMenuMapTeleportPlayer.Size = new System.Drawing.Size(162, 22);
            this.toolStripMenuMapTeleportPlayer.Text = "Teleport";
            this.toolStripMenuMapTeleportPlayer.Click += new System.EventHandler(this.toolStripMenuItemTeleportPlayer_Click);
            // 
            // dataSetBindingSource
            // 
            this.dataSetBindingSource.DataSource = typeof(System.Data.DataSet);
            this.dataSetBindingSource.Position = 0;
            // 
            // contextMenuStripResetTypes
            // 
            this.contextMenuStripResetTypes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemResetTypes});
            this.contextMenuStripResetTypes.Name = "contextMenuStripVehicle";
            this.contextMenuStripResetTypes.Size = new System.Drawing.Size(121, 26);
            // 
            // toolStripMenuItemResetTypes
            // 
            this.toolStripMenuItemResetTypes.Name = "toolStripMenuItemResetTypes";
            this.toolStripMenuItemResetTypes.Size = new System.Drawing.Size(120, 22);
            this.toolStripMenuItemResetTypes.Text = "Reset list";
            this.toolStripMenuItemResetTypes.Click += new System.EventHandler(this.toolStripMenuItemResetTypes_Click);
            // 
            // contextMenuStripSpawn
            // 
            this.contextMenuStripSpawn.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemDeleteSpawn});
            this.contextMenuStripSpawn.Name = "contextMenuStripSpawn";
            this.contextMenuStripSpawn.Size = new System.Drawing.Size(174, 26);
            // 
            // toolStripMenuItemDeleteSpawn
            // 
            this.toolStripMenuItemDeleteSpawn.Name = "toolStripMenuItemDeleteSpawn";
            this.toolStripMenuItemDeleteSpawn.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItemDeleteSpawn.Text = "Delete Spawnpoint";
            this.toolStripMenuItemDeleteSpawn.Click += new System.EventHandler(this.toolStripMenuItemDeleteSpawn_Click);
            // 
            // bgWorkerDatabase
            // 
            this.bgWorkerDatabase.WorkerSupportsCancellation = true;
            this.bgWorkerDatabase.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerRefreshDatabase_DoWork);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 250;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // contextMenuStripVehicle
            // 
            this.contextMenuStripVehicle.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.repairRefuelVehicleToolStripMenuItem,
            this.toolStripMenuItemDeleteVehicle});
            this.contextMenuStripVehicle.Name = "contextMenuStripVehicle";
            this.contextMenuStripVehicle.Size = new System.Drawing.Size(207, 48);
            // 
            // repairRefuelVehicleToolStripMenuItem
            // 
            this.repairRefuelVehicleToolStripMenuItem.Name = "repairRefuelVehicleToolStripMenuItem";
            this.repairRefuelVehicleToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.repairRefuelVehicleToolStripMenuItem.Text = "Repair and Refuel vehicle";
            this.repairRefuelVehicleToolStripMenuItem.Click += new System.EventHandler(this.repairRefuelVehicleToolStripMenuItem_Click);
            // 
            // toolStripMenuItemDeleteVehicle
            // 
            this.toolStripMenuItemDeleteVehicle.Name = "toolStripMenuItemDeleteVehicle";
            this.toolStripMenuItemDeleteVehicle.Size = new System.Drawing.Size(206, 22);
            this.toolStripMenuItemDeleteVehicle.Text = "Delete vehicle";
            this.toolStripMenuItemDeleteVehicle.Click += new System.EventHandler(this.toolStripMenuItemDelete_Click);
            // 
            // bgWorkerMapZoom
            // 
            this.bgWorkerMapZoom.WorkerSupportsCancellation = true;
            this.bgWorkerMapZoom.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerMapZoom_DoWork);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "sql";
            this.saveFileDialog1.Filter = "SQL Files|*.sql";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusWorld,
            this.toolStripStatusOnline,
            this.toolStripStatusAlive,
            this.toolStripStatusDead,
            this.toolStripStatusVehicle,
            this.toolStripStatusSpawn,
            this.toolStripStatusDeployable,
            this.toolStripStatusMapHelper,
            this.toolStripStatusTrail,
            this.toolStripStatusChat,
            this.toolStripStatusCoordMap,
            this.toolStripStatusCoordDB,
            this.toolStripStatusNews});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(879, 41);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusWorld
            // 
            this.toolStripStatusWorld.AutoToolTip = true;
            this.toolStripStatusWorld.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusWorld.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusWorld.Image = global::DBAccess.Properties.Resources.Tool_World;
            this.toolStripStatusWorld.Name = "toolStripStatusWorld";
            this.toolStripStatusWorld.Size = new System.Drawing.Size(48, 36);
            this.toolStripStatusWorld.Text = "-";
            this.toolStripStatusWorld.ToolTipText = "Set maps for each world";
            this.toolStripStatusWorld.Click += new System.EventHandler(this.toolStripStatusWorld_Click);
            // 
            // toolStripStatusOnline
            // 
            this.toolStripStatusOnline.AutoSize = false;
            this.toolStripStatusOnline.AutoToolTip = true;
            this.toolStripStatusOnline.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusOnline.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusOnline.Image = global::DBAccess.Properties.Resources.Tool_Online;
            this.toolStripStatusOnline.Name = "toolStripStatusOnline";
            this.toolStripStatusOnline.Size = new System.Drawing.Size(64, 36);
            this.toolStripStatusOnline.Text = "888";
            this.toolStripStatusOnline.ToolTipText = "Show online players";
            this.toolStripStatusOnline.Click += new System.EventHandler(this.toolStripStatusOnline_Click);
            // 
            // toolStripStatusAlive
            // 
            this.toolStripStatusAlive.AutoSize = false;
            this.toolStripStatusAlive.AutoToolTip = true;
            this.toolStripStatusAlive.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusAlive.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusAlive.Image = global::DBAccess.Properties.Resources.Tool_Alive;
            this.toolStripStatusAlive.Name = "toolStripStatusAlive";
            this.toolStripStatusAlive.Size = new System.Drawing.Size(64, 36);
            this.toolStripStatusAlive.Text = "888";
            this.toolStripStatusAlive.ToolTipText = "Show alive players";
            this.toolStripStatusAlive.Click += new System.EventHandler(this.toolStripStatusAlive_Click);
            // 
            // toolStripStatusDead
            // 
            this.toolStripStatusDead.AutoSize = false;
            this.toolStripStatusDead.AutoToolTip = true;
            this.toolStripStatusDead.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusDead.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusDead.Image = global::DBAccess.Properties.Resources.Dead;
            this.toolStripStatusDead.Name = "toolStripStatusDead";
            this.toolStripStatusDead.Size = new System.Drawing.Size(64, 36);
            this.toolStripStatusDead.Text = "888";
            this.toolStripStatusDead.ToolTipText = "Show dead players";
            this.toolStripStatusDead.Click += new System.EventHandler(this.toolStripStatusDead_Click);
            // 
            // toolStripStatusVehicle
            // 
            this.toolStripStatusVehicle.AutoSize = false;
            this.toolStripStatusVehicle.AutoToolTip = true;
            this.toolStripStatusVehicle.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusVehicle.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusVehicle.Image = global::DBAccess.Properties.Resources.Tool_Vehicle;
            this.toolStripStatusVehicle.Name = "toolStripStatusVehicle";
            this.toolStripStatusVehicle.Size = new System.Drawing.Size(64, 36);
            this.toolStripStatusVehicle.Text = "888";
            this.toolStripStatusVehicle.ToolTipText = "Show vehicles";
            this.toolStripStatusVehicle.Click += new System.EventHandler(this.toolStripStatusVehicle_Click);
            // 
            // toolStripStatusSpawn
            // 
            this.toolStripStatusSpawn.AutoSize = false;
            this.toolStripStatusSpawn.AutoToolTip = true;
            this.toolStripStatusSpawn.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusSpawn.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusSpawn.Image = global::DBAccess.Properties.Resources.Tool_Spawn;
            this.toolStripStatusSpawn.Name = "toolStripStatusSpawn";
            this.toolStripStatusSpawn.Size = new System.Drawing.Size(64, 36);
            this.toolStripStatusSpawn.Text = "888";
            this.toolStripStatusSpawn.ToolTipText = "Show spawn points";
            this.toolStripStatusSpawn.Click += new System.EventHandler(this.toolStripStatusSpawn_Click);
            // 
            // toolStripStatusDeployable
            // 
            this.toolStripStatusDeployable.AutoSize = false;
            this.toolStripStatusDeployable.AutoToolTip = true;
            this.toolStripStatusDeployable.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusDeployable.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusDeployable.Image = global::DBAccess.Properties.Resources.Tool_Deployable;
            this.toolStripStatusDeployable.Name = "toolStripStatusDeployable";
            this.toolStripStatusDeployable.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripStatusDeployable.Size = new System.Drawing.Size(64, 36);
            this.toolStripStatusDeployable.Text = "888";
            this.toolStripStatusDeployable.ToolTipText = "Show deployables";
            this.toolStripStatusDeployable.Click += new System.EventHandler(this.toolStripStatusDeployable_Click);
            // 
            // toolStripStatusMapHelper
            // 
            this.toolStripStatusMapHelper.AutoSize = false;
            this.toolStripStatusMapHelper.AutoToolTip = true;
            this.toolStripStatusMapHelper.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusMapHelper.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusMapHelper.Image = global::DBAccess.Properties.Resources.Tool_MapHelper;
            this.toolStripStatusMapHelper.Name = "toolStripStatusMapHelper";
            this.toolStripStatusMapHelper.Size = new System.Drawing.Size(48, 36);
            this.toolStripStatusMapHelper.ToolTipText = "Set link between bitmap and the database coordinates";
            this.toolStripStatusMapHelper.Click += new System.EventHandler(this.toolStripStatusMapHelper_Click);
            // 
            // toolStripStatusTrail
            // 
            this.toolStripStatusTrail.AutoSize = false;
            this.toolStripStatusTrail.AutoToolTip = true;
            this.toolStripStatusTrail.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusTrail.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusTrail.Image = global::DBAccess.Properties.Resources.Tool_Trail;
            this.toolStripStatusTrail.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusTrail.Name = "toolStripStatusTrail";
            this.toolStripStatusTrail.Size = new System.Drawing.Size(48, 36);
            this.toolStripStatusTrail.ToolTipText = "Display moves for players/vehicles";
            this.toolStripStatusTrail.Click += new System.EventHandler(this.toolStripStatusTrail_Click);
            // 
            // toolStripStatusChat
            // 
            this.toolStripStatusChat.AutoSize = false;
            this.toolStripStatusChat.AutoToolTip = true;
            this.toolStripStatusChat.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusChat.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusChat.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusChat.Image = global::DBAccess.Properties.Resources.Tool_Chat;
            this.toolStripStatusChat.Name = "toolStripStatusChat";
            this.toolStripStatusChat.Size = new System.Drawing.Size(48, 36);
            this.toolStripStatusChat.ToolTipText = "Chat window";
            this.toolStripStatusChat.Click += new System.EventHandler(this.toolStripStatusHelp_Click);
            // 
            // toolStripStatusCoordMap
            // 
            this.toolStripStatusCoordMap.AutoSize = false;
            this.toolStripStatusCoordMap.AutoToolTip = true;
            this.toolStripStatusCoordMap.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusCoordMap.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusCoordMap.Image = global::DBAccess.Properties.Resources.Tool_Map;
            this.toolStripStatusCoordMap.Name = "toolStripStatusCoordMap";
            this.toolStripStatusCoordMap.Size = new System.Drawing.Size(120, 36);
            this.toolStripStatusCoordMap.Text = "-";
            this.toolStripStatusCoordMap.ToolTipText = "Map coordinates";
            // 
            // toolStripStatusCoordDB
            // 
            this.toolStripStatusCoordDB.AutoSize = false;
            this.toolStripStatusCoordDB.AutoToolTip = true;
            this.toolStripStatusCoordDB.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusCoordDB.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusCoordDB.Image = global::DBAccess.Properties.Resources.Tool_DB;
            this.toolStripStatusCoordDB.Name = "toolStripStatusCoordDB";
            this.toolStripStatusCoordDB.Size = new System.Drawing.Size(120, 36);
            this.toolStripStatusCoordDB.Text = "-";
            this.toolStripStatusCoordDB.ToolTipText = "DB coordinates";
            // 
            // toolStripStatusNews
            // 
            this.toolStripStatusNews.AutoSize = false;
            this.toolStripStatusNews.AutoToolTip = true;
            this.toolStripStatusNews.Image = global::DBAccess.Properties.Resources.Tool_About;
            this.toolStripStatusNews.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusNews.Name = "toolStripStatusNews";
            this.toolStripStatusNews.Size = new System.Drawing.Size(48, 36);
            this.toolStripStatusNews.ToolTipText = "What\'s new...";
            this.toolStripStatusNews.Click += new System.EventHandler(this.toolStripStatusNews_Click);
            // 
            // ttootToolStripMenuItem
            // 
            this.ttootToolStripMenuItem.Checked = true;
            this.ttootToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ttootToolStripMenuItem.Name = "ttootToolStripMenuItem";
            this.ttootToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.ttootToolStripMenuItem.Text = "Players online";
            // 
            // showPlayersAliveToolStripMenuItem
            // 
            this.showPlayersAliveToolStripMenuItem.Name = "showPlayersAliveToolStripMenuItem";
            this.showPlayersAliveToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.showPlayersAliveToolStripMenuItem.Text = "Players alive";
            // 
            // showVehiclesToolStripMenuItem
            // 
            this.showVehiclesToolStripMenuItem.Name = "showVehiclesToolStripMenuItem";
            this.showVehiclesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.showVehiclesToolStripMenuItem.Text = "Vehicles";
            // 
            // showSpawnPointsToolStripMenuItem
            // 
            this.showSpawnPointsToolStripMenuItem.Name = "showSpawnPointsToolStripMenuItem";
            this.showSpawnPointsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.showSpawnPointsToolStripMenuItem.Text = "Spawn points";
            // 
            // showDeployablesToolStripMenuItem
            // 
            this.showDeployablesToolStripMenuItem.Name = "showDeployablesToolStripMenuItem";
            this.showDeployablesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.showDeployablesToolStripMenuItem.Text = "Deployables";
            // 
            // mapHelperToolStripMenuItem
            // 
            this.mapHelperToolStripMenuItem.Name = "mapHelperToolStripMenuItem";
            this.mapHelperToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.mapHelperToolStripMenuItem.Text = "Map Helper";
            // 
            // cartographerToolStripMenuItem
            // 
            this.cartographerToolStripMenuItem.Name = "cartographerToolStripMenuItem";
            this.cartographerToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.cartographerToolStripMenuItem.Text = "Cartographer";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(144, 6);
            // 
            // showTrailsToolStripMenuItem
            // 
            this.showTrailsToolStripMenuItem.Name = "showTrailsToolStripMenuItem";
            this.showTrailsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.showTrailsToolStripMenuItem.Text = "Trails";
            // 
            // toolStripStatusLabelWorld
            // 
            this.toolStripStatusLabelWorld.Name = "toolStripStatusLabelWorld";
            this.toolStripStatusLabelWorld.Size = new System.Drawing.Size(12, 17);
            this.toolStripStatusLabelWorld.Text = "-";
            // 
            // toolStripMenuItemOnline
            // 
            this.toolStripMenuItemOnline.Name = "toolStripMenuItemOnline";
            this.toolStripMenuItemOnline.Size = new System.Drawing.Size(92, 22);
            this.toolStripMenuItemOnline.Text = "Players online";
            // 
            // toolStripMenuItemAlive
            // 
            this.toolStripMenuItemAlive.Name = "toolStripMenuItemAlive";
            this.toolStripMenuItemAlive.Size = new System.Drawing.Size(83, 22);
            this.toolStripMenuItemAlive.Text = "Players alive";
            // 
            // toolStripMenuItemVehicle
            // 
            this.toolStripMenuItemVehicle.Name = "toolStripMenuItemVehicle";
            this.toolStripMenuItemVehicle.Size = new System.Drawing.Size(62, 22);
            this.toolStripMenuItemVehicle.Text = "Vehicles";
            // 
            // toolStripMenuItemSpawn
            // 
            this.toolStripMenuItemSpawn.Name = "toolStripMenuItemSpawn";
            this.toolStripMenuItemSpawn.Size = new System.Drawing.Size(90, 22);
            this.toolStripMenuItemSpawn.Text = "Spawn points";
            // 
            // toolStripMenuItemDeployable
            // 
            this.toolStripMenuItemDeployable.Name = "toolStripMenuItemDeployable";
            this.toolStripMenuItemDeployable.Size = new System.Drawing.Size(83, 22);
            this.toolStripMenuItemDeployable.Text = "Deployables";
            // 
            // toolStripMenuItemMapHelper
            // 
            this.toolStripMenuItemMapHelper.Name = "toolStripMenuItemMapHelper";
            this.toolStripMenuItemMapHelper.Size = new System.Drawing.Size(81, 22);
            this.toolStripMenuItemMapHelper.Text = "Map Helper";
            // 
            // toolStripMenuItemCartographer
            // 
            this.toolStripMenuItemCartographer.Name = "toolStripMenuItemCartographer";
            this.toolStripMenuItemCartographer.Size = new System.Drawing.Size(89, 22);
            this.toolStripMenuItemCartographer.Text = "Cartographer";
            // 
            // toolStripMenuItemTrails
            // 
            this.toolStripMenuItemTrails.Name = "toolStripMenuItemTrails";
            this.toolStripMenuItemTrails.Size = new System.Drawing.Size(47, 22);
            this.toolStripMenuItemTrails.Text = "Trails";
            // 
            // toolStripStatusLabelCnx
            // 
            this.toolStripStatusLabelCnx.Name = "toolStripStatusLabelCnx";
            this.toolStripStatusLabelCnx.Size = new System.Drawing.Size(12, 17);
            this.toolStripStatusLabelCnx.Text = "-";
            // 
            // toolStripContainer2
            // 
            // 
            // toolStripContainer2.ContentPanel
            // 
            this.toolStripContainer2.ContentPanel.AutoScroll = true;
            this.toolStripContainer2.ContentPanel.Controls.Add(this.toolStripContainer1);
            this.toolStripContainer2.ContentPanel.Size = new System.Drawing.Size(879, 401);
            this.toolStripContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer2.LeftToolStripPanelVisible = false;
            this.toolStripContainer2.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer2.Name = "toolStripContainer2";
            this.toolStripContainer2.RightToolStripPanelVisible = false;
            this.toolStripContainer2.Size = new System.Drawing.Size(879, 401);
            this.toolStripContainer2.TabIndex = 4;
            this.toolStripContainer2.Text = "toolStripContainer2";
            this.toolStripContainer2.TopToolStripPanelVisible = false;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainerGlobal);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(879, 401);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(879, 401);
            this.toolStripContainer1.TabIndex = 3;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanelVisible = false;
            // 
            // contextMenuPlayersOnline
            // 
            this.contextMenuPlayersOnline.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.messageToPlayerToolStripMenuItem,
            this.kickPlayerToolStripMenuItem});
            this.contextMenuPlayersOnline.Name = "contextMenuPlayersOnline";
            this.contextMenuPlayersOnline.Size = new System.Drawing.Size(170, 48);
            // 
            // messageToPlayerToolStripMenuItem
            // 
            this.messageToPlayerToolStripMenuItem.Name = "messageToPlayerToolStripMenuItem";
            this.messageToPlayerToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.messageToPlayerToolStripMenuItem.Text = "Message to player";
            this.messageToPlayerToolStripMenuItem.Click += new System.EventHandler(this.messageToPlayerToolStripMenuItem_Click);
            // 
            // kickPlayerToolStripMenuItem
            // 
            this.kickPlayerToolStripMenuItem.Name = "kickPlayerToolStripMenuItem";
            this.kickPlayerToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.kickPlayerToolStripMenuItem.Text = "Kick player";
            this.kickPlayerToolStripMenuItem.Click += new System.EventHandler(this.kickPlayerToolStripMenuItem_Click);
            // 
            // toolStripContainer3
            // 
            // 
            // toolStripContainer3.BottomToolStripPanel
            // 
            this.toolStripContainer3.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer3.ContentPanel
            // 
            this.toolStripContainer3.ContentPanel.AutoScroll = true;
            this.toolStripContainer3.ContentPanel.Controls.Add(this.toolStripContainer2);
            this.toolStripContainer3.ContentPanel.Size = new System.Drawing.Size(879, 401);
            this.toolStripContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer3.LeftToolStripPanelVisible = false;
            this.toolStripContainer3.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer3.Name = "toolStripContainer3";
            this.toolStripContainer3.RightToolStripPanelVisible = false;
            this.toolStripContainer3.Size = new System.Drawing.Size(879, 442);
            this.toolStripContainer3.TabIndex = 5;
            this.toolStripContainer3.Text = "toolStripContainer3";
            this.toolStripContainer3.TopToolStripPanelVisible = false;
            // 
            // bgWorkerLoadTiles
            // 
            this.bgWorkerLoadTiles.WorkerSupportsCancellation = true;
            this.bgWorkerLoadTiles.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerLoadTiles_DoWork);
            // 
            // contextMenuStripPlayerMenu
            // 
            this.contextMenuStripPlayerMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemHeal});
            this.contextMenuStripPlayerMenu.Name = "contextMenuStripPlayer";
            this.contextMenuStripPlayerMenu.Size = new System.Drawing.Size(134, 26);
            this.contextMenuStripPlayerMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripItemPlayerMenu_Opening);
            // 
            // toolStripMenuItemHeal
            // 
            this.toolStripMenuItemHeal.Name = "toolStripMenuItemHeal";
            this.toolStripMenuItemHeal.Size = new System.Drawing.Size(133, 22);
            this.toolStripMenuItemHeal.Text = "Heal player";
            this.toolStripMenuItemHeal.Click += new System.EventHandler(this.toolStripMenuItemHealPlayer_Click);
            // 
            // bgWorkerFocus
            // 
            this.bgWorkerFocus.WorkerSupportsCancellation = true;
            this.bgWorkerFocus.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerFocus_DoWork);
            // 
            // splitContainerGlobal
            // 
            this.splitContainerGlobal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerGlobal.Location = new System.Drawing.Point(0, 0);
            this.splitContainerGlobal.Name = "splitContainerGlobal";
            this.splitContainerGlobal.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerGlobal.Panel1
            // 
            this.splitContainerGlobal.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainerGlobal.Panel2
            // 
            this.splitContainerGlobal.Panel2.Controls.Add(this.splitContainerChat);
            this.splitContainerGlobal.Size = new System.Drawing.Size(879, 401);
            this.splitContainerGlobal.SplitterDistance = 313;
            this.splitContainerGlobal.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.panelCnx);
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewMaps);
            this.splitContainer1.Panel1.SizeChanged += new System.EventHandler(this.splitContainer1_Panel1_SizeChanged);
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel1_Paint);
            this.splitContainer1.Panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseClick);
            this.splitContainer1.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseDown);
            this.splitContainer1.Panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseMove);
            this.splitContainer1.Panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseUp);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2MinSize = 220;
            this.splitContainer1.Size = new System.Drawing.Size(879, 313);
            this.splitContainer1.SplitterDistance = 558;
            this.splitContainer1.TabIndex = 1;
            // 
            // panelCnx
            // 
            this.panelCnx.Controls.Add(this.groupBoxrCon);
            this.panelCnx.Controls.Add(this.groupBoxDB);
            this.panelCnx.Controls.Add(this.buttonConnect);
            this.panelCnx.Location = new System.Drawing.Point(124, 48);
            this.panelCnx.Margin = new System.Windows.Forms.Padding(2);
            this.panelCnx.Name = "panelCnx";
            this.panelCnx.Size = new System.Drawing.Size(376, 251);
            this.panelCnx.TabIndex = 10;
            // 
            // groupBoxrCon
            // 
            this.groupBoxrCon.Controls.Add(this.numericUpDownrConPort);
            this.groupBoxrCon.Controls.Add(this.textBoxrConPassword);
            this.groupBoxrCon.Controls.Add(this.label8);
            this.groupBoxrCon.Controls.Add(this.textBoxrConURL);
            this.groupBoxrCon.Controls.Add(this.label9);
            this.groupBoxrCon.Controls.Add(this.label7);
            this.groupBoxrCon.Location = new System.Drawing.Point(14, 138);
            this.groupBoxrCon.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxrCon.Name = "groupBoxrCon";
            this.groupBoxrCon.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxrCon.Size = new System.Drawing.Size(348, 68);
            this.groupBoxrCon.TabIndex = 10;
            this.groupBoxrCon.TabStop = false;
            this.groupBoxrCon.Text = "rCon (optional)";
            // 
            // numericUpDownrConPort
            // 
            this.numericUpDownrConPort.Location = new System.Drawing.Point(291, 15);
            this.numericUpDownrConPort.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownrConPort.Name = "numericUpDownrConPort";
            this.numericUpDownrConPort.Size = new System.Drawing.Size(51, 20);
            this.numericUpDownrConPort.TabIndex = 7;
            this.numericUpDownrConPort.Value = new decimal(new int[] {
            2302,
            0,
            0,
            0});
            // 
            // textBoxrConPassword
            // 
            this.textBoxrConPassword.Location = new System.Drawing.Point(212, 41);
            this.textBoxrConPassword.MaxLength = 64;
            this.textBoxrConPassword.Name = "textBoxrConPassword";
            this.textBoxrConPassword.Size = new System.Drawing.Size(130, 20);
            this.textBoxrConPassword.TabIndex = 9;
            this.textBoxrConPassword.UseSystemPasswordChar = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(176, 43);
            this.label8.Name = "label8";
            this.label8.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 114;
            this.label8.Text = "Pass";
            // 
            // textBoxrConURL
            // 
            this.textBoxrConURL.Location = new System.Drawing.Point(41, 41);
            this.textBoxrConURL.MaxLength = 256;
            this.textBoxrConURL.Name = "textBoxrConURL";
            this.textBoxrConURL.Size = new System.Drawing.Size(130, 20);
            this.textBoxrConURL.TabIndex = 8;
            this.toolTip1.SetToolTip(this.textBoxrConURL, "optional URL for simulation purpose");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Enabled = false;
            this.label9.Location = new System.Drawing.Point(8, 44);
            this.label9.Name = "label9";
            this.label9.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label9.Size = new System.Drawing.Size(29, 13);
            this.label9.TabIndex = 113;
            this.label9.Text = "URL";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(261, 17);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label7.Size = new System.Drawing.Size(26, 13);
            this.label7.TabIndex = 110;
            this.label7.Text = "Port";
            // 
            // groupBoxDB
            // 
            this.groupBoxDB.Controls.Add(this.numericUpDownDBPort);
            this.groupBoxDB.Controls.Add(this.label6);
            this.groupBoxDB.Controls.Add(this.numericUpDownInstanceId);
            this.groupBoxDB.Controls.Add(this.label10);
            this.groupBoxDB.Controls.Add(this.comboBoxGameType);
            this.groupBoxDB.Controls.Add(this.label13);
            this.groupBoxDB.Controls.Add(this.label1);
            this.groupBoxDB.Controls.Add(this.textBoxDBURL);
            this.groupBoxDB.Controls.Add(this.textBoxDBPassword);
            this.groupBoxDB.Controls.Add(this.label4);
            this.groupBoxDB.Controls.Add(this.label3);
            this.groupBoxDB.Controls.Add(this.textBoxDBUser);
            this.groupBoxDB.Controls.Add(this.label5);
            this.groupBoxDB.Controls.Add(this.textBoxDBBaseName);
            this.groupBoxDB.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBoxDB.Location = new System.Drawing.Point(14, 11);
            this.groupBoxDB.Name = "groupBoxDB";
            this.groupBoxDB.Size = new System.Drawing.Size(348, 121);
            this.groupBoxDB.TabIndex = 9;
            this.groupBoxDB.TabStop = false;
            this.groupBoxDB.Text = "Database";
            // 
            // numericUpDownDBPort
            // 
            this.numericUpDownDBPort.Location = new System.Drawing.Point(291, 16);
            this.numericUpDownDBPort.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownDBPort.Name = "numericUpDownDBPort";
            this.numericUpDownDBPort.Size = new System.Drawing.Size(51, 20);
            this.numericUpDownDBPort.TabIndex = 1;
            this.numericUpDownDBPort.Value = new decimal(new int[] {
            3306,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(260, 18);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 108;
            this.label6.Text = "Port";
            // 
            // numericUpDownInstanceId
            // 
            this.numericUpDownInstanceId.Location = new System.Drawing.Point(292, 95);
            this.numericUpDownInstanceId.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownInstanceId.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownInstanceId.Name = "numericUpDownInstanceId";
            this.numericUpDownInstanceId.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownInstanceId.TabIndex = 6;
            this.numericUpDownInstanceId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(226, 97);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 13);
            this.label10.TabIndex = 105;
            this.label10.Text = "Instance Id";
            // 
            // comboBoxGameType
            // 
            this.comboBoxGameType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGameType.FormattingEnabled = true;
            this.comboBoxGameType.Items.AddRange(new object[] {
            "Auto",
            "Classic",
            "Epoch"});
            this.comboBoxGameType.Location = new System.Drawing.Point(79, 94);
            this.comboBoxGameType.MaxDropDownItems = 4;
            this.comboBoxGameType.Name = "comboBoxGameType";
            this.comboBoxGameType.Size = new System.Drawing.Size(92, 21);
            this.comboBoxGameType.TabIndex = 5;
            this.comboBoxGameType.SelectedValueChanged += new System.EventHandler(this.comboBoxGameType_SelectedValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(4, 97);
            this.label13.Name = "label13";
            this.label13.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label13.Size = new System.Drawing.Size(62, 13);
            this.label13.TabIndex = 106;
            this.label13.Text = "Game Type";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 100;
            this.label1.Text = "URL";
            // 
            // textBoxDBURL
            // 
            this.textBoxDBURL.Location = new System.Drawing.Point(41, 16);
            this.textBoxDBURL.MaxLength = 256;
            this.textBoxDBURL.Name = "textBoxDBURL";
            this.textBoxDBURL.Size = new System.Drawing.Size(162, 20);
            this.textBoxDBURL.TabIndex = 0;
            // 
            // textBoxDBPassword
            // 
            this.textBoxDBPassword.Location = new System.Drawing.Point(212, 67);
            this.textBoxDBPassword.MaxLength = 64;
            this.textBoxDBPassword.Name = "textBoxDBPassword";
            this.textBoxDBPassword.Size = new System.Drawing.Size(130, 20);
            this.textBoxDBPassword.TabIndex = 4;
            this.textBoxDBPassword.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(177, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 104;
            this.label4.Text = "Pass";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 102;
            this.label3.Text = "Base";
            // 
            // textBoxDBUser
            // 
            this.textBoxDBUser.Location = new System.Drawing.Point(41, 68);
            this.textBoxDBUser.MaxLength = 256;
            this.textBoxDBUser.Name = "textBoxDBUser";
            this.textBoxDBUser.Size = new System.Drawing.Size(130, 20);
            this.textBoxDBUser.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 103;
            this.label5.Text = "User";
            // 
            // textBoxDBBaseName
            // 
            this.textBoxDBBaseName.Location = new System.Drawing.Point(41, 42);
            this.textBoxDBBaseName.MaxLength = 256;
            this.textBoxDBBaseName.Name = "textBoxDBBaseName";
            this.textBoxDBBaseName.Size = new System.Drawing.Size(162, 20);
            this.textBoxDBBaseName.TabIndex = 2;
            this.toolTip1.SetToolTip(this.textBoxDBBaseName, "Base name in mySQL");
            // 
            // buttonConnect
            // 
            this.buttonConnect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonConnect.Location = new System.Drawing.Point(160, 216);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(70, 23);
            this.buttonConnect.TabIndex = 10;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // dataGridViewMaps
            // 
            this.dataGridViewMaps.AllowUserToAddRows = false;
            this.dataGridViewMaps.AllowUserToDeleteRows = false;
            this.dataGridViewMaps.AllowUserToOrderColumns = true;
            this.dataGridViewMaps.AllowUserToResizeRows = false;
            this.dataGridViewMaps.AutoGenerateColumns = false;
            this.dataGridViewMaps.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewMaps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewMaps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColGVMID,
            this.ColGVMName,
            this.ColGVMChoosePath,
            this.ColGVMPath});
            this.dataGridViewMaps.DataSource = this.dataSetBindingSource;
            this.dataGridViewMaps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMaps.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewMaps.MultiSelect = false;
            this.dataGridViewMaps.Name = "dataGridViewMaps";
            this.dataGridViewMaps.RowHeadersVisible = false;
            this.dataGridViewMaps.Size = new System.Drawing.Size(554, 309);
            this.dataGridViewMaps.TabIndex = 0;
            this.dataGridViewMaps.Visible = false;
            this.dataGridViewMaps.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewMaps_CellClick);
            // 
            // ColGVMID
            // 
            this.ColGVMID.FillWeight = 10F;
            this.ColGVMID.HeaderText = "World ID";
            this.ColGVMID.Name = "ColGVMID";
            this.ColGVMID.ReadOnly = true;
            // 
            // ColGVMName
            // 
            this.ColGVMName.FillWeight = 30F;
            this.ColGVMName.HeaderText = "Name";
            this.ColGVMName.Name = "ColGVMName";
            this.ColGVMName.ReadOnly = true;
            // 
            // ColGVMChoosePath
            // 
            this.ColGVMChoosePath.FillWeight = 8F;
            this.ColGVMChoosePath.HeaderText = "Select";
            this.ColGVMChoosePath.Name = "ColGVMChoosePath";
            this.ColGVMChoosePath.Text = "...";
            this.ColGVMChoosePath.ToolTipText = "Select your file on disk";
            this.ColGVMChoosePath.UseColumnTextForButtonValue = true;
            // 
            // ColGVMPath
            // 
            this.ColGVMPath.FillWeight = 45F;
            this.ColGVMPath.HeaderText = "Path";
            this.ColGVMPath.Name = "ColGVMPath";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageDisplay);
            this.tabControl1.Controls.Add(this.tabPageScripts);
            this.tabControl1.Controls.Add(this.tabPageVehicles);
            this.tabControl1.Controls.Add(this.tabPageDeployables);
            this.tabControl1.Controls.Add(this.tabPagePlayers);
            this.tabControl1.Controls.Add(this.tabPageSetup);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(313, 309);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPageDisplay
            // 
            this.tabPageDisplay.BackColor = System.Drawing.Color.Transparent;
            this.tabPageDisplay.Controls.Add(this.label11);
            this.tabPageDisplay.Controls.Add(this.labelMagLevel);
            this.tabPageDisplay.Controls.Add(this.trackBarMagLevel);
            this.tabPageDisplay.Controls.Add(this.label2);
            this.tabPageDisplay.Controls.Add(this.labelLastUpdate);
            this.tabPageDisplay.Controls.Add(this.trackBarLastUpdated);
            this.tabPageDisplay.Controls.Add(this.groupBoxInfo);
            this.tabPageDisplay.Location = new System.Drawing.Point(4, 22);
            this.tabPageDisplay.Name = "tabPageDisplay";
            this.tabPageDisplay.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDisplay.Size = new System.Drawing.Size(305, 283);
            this.tabPageDisplay.TabIndex = 1;
            this.tabPageDisplay.Text = "Display";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 51);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Mag level";
            // 
            // labelMagLevel
            // 
            this.labelMagLevel.AutoSize = true;
            this.labelMagLevel.Location = new System.Drawing.Point(232, 51);
            this.labelMagLevel.Name = "labelMagLevel";
            this.labelMagLevel.Size = new System.Drawing.Size(10, 13);
            this.labelMagLevel.TabIndex = 12;
            this.labelMagLevel.Text = "-";
            this.labelMagLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // trackBarMagLevel
            // 
            this.trackBarMagLevel.LargeChange = 1;
            this.trackBarMagLevel.Location = new System.Drawing.Point(82, 37);
            this.trackBarMagLevel.Maximum = 6;
            this.trackBarMagLevel.Name = "trackBarMagLevel";
            this.trackBarMagLevel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.trackBarMagLevel.Size = new System.Drawing.Size(144, 45);
            this.trackBarMagLevel.TabIndex = 11;
            this.trackBarMagLevel.TickFrequency = 5;
            this.trackBarMagLevel.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.toolTip1.SetToolTip(this.trackBarMagLevel, "Map maximal level of zoom");
            this.trackBarMagLevel.Value = this.trackBarMagLevel.Maximum;
            this.trackBarMagLevel.ValueChanged += new System.EventHandler(this.trackBarMagLevel_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Filter old items";
            // 
            // labelLastUpdate
            // 
            this.labelLastUpdate.AutoSize = true;
            this.labelLastUpdate.Location = new System.Drawing.Point(232, 21);
            this.labelLastUpdate.Name = "labelLastUpdate";
            this.labelLastUpdate.Size = new System.Drawing.Size(10, 13);
            this.labelLastUpdate.TabIndex = 9;
            this.labelLastUpdate.Text = "-";
            this.labelLastUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // trackBarLastUpdated
            // 
            this.trackBarLastUpdated.LargeChange = 7;
            this.trackBarLastUpdated.Location = new System.Drawing.Point(82, 7);
            this.trackBarLastUpdated.Maximum = 30;
            this.trackBarLastUpdated.Minimum = 1;
            this.trackBarLastUpdated.Name = "trackBarLastUpdated";
            this.trackBarLastUpdated.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.trackBarLastUpdated.Size = new System.Drawing.Size(144, 45);
            this.trackBarLastUpdated.TabIndex = 8;
            this.trackBarLastUpdated.TickFrequency = 5;
            this.trackBarLastUpdated.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.toolTip1.SetToolTip(this.trackBarLastUpdated, "Filters items not updated since X days");
            this.trackBarLastUpdated.Value = this.trackBarLastUpdated.Maximum;
            this.trackBarLastUpdated.ValueChanged += new System.EventHandler(this.trackBarLastUpdated_ValueChanged);
            // 
            // groupBoxInfo
            // 
            this.groupBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxInfo.Controls.Add(this.propertyGrid1);
            this.groupBoxInfo.Location = new System.Drawing.Point(3, 79);
            this.groupBoxInfo.Name = "groupBoxInfo";
            this.groupBoxInfo.Size = new System.Drawing.Size(296, 197);
            this.groupBoxInfo.TabIndex = 3;
            this.groupBoxInfo.TabStop = false;
            this.groupBoxInfo.Text = "Info";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 16);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.Size = new System.Drawing.Size(290, 178);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Control;
            // 
            // tabPageScripts
            // 
            this.tabPageScripts.BackColor = System.Drawing.Color.Transparent;
            this.tabPageScripts.Controls.Add(this.buttonSelectCustom3);
            this.tabPageScripts.Controls.Add(this.buttonSelectCustom2);
            this.tabPageScripts.Controls.Add(this.buttonSelectCustom1);
            this.tabPageScripts.Controls.Add(this.buttonCustom3);
            this.tabPageScripts.Controls.Add(this.buttonCustom2);
            this.tabPageScripts.Controls.Add(this.buttonCustom1);
            this.tabPageScripts.Controls.Add(this.textBoxOldTentLimit);
            this.tabPageScripts.Controls.Add(this.buttonRemoveTents);
            this.tabPageScripts.Controls.Add(this.textBoxCmdStatus);
            this.tabPageScripts.Controls.Add(this.textBoxOldBodyLimit);
            this.tabPageScripts.Controls.Add(this.textBoxVehicleMax);
            this.tabPageScripts.Controls.Add(this.buttonRemoveBodies);
            this.tabPageScripts.Controls.Add(this.buttonSpawnNew);
            this.tabPageScripts.Controls.Add(this.buttonBackup);
            this.tabPageScripts.Controls.Add(this.buttonRemoveDestroyed);
            this.tabPageScripts.Location = new System.Drawing.Point(4, 22);
            this.tabPageScripts.Name = "tabPageScripts";
            this.tabPageScripts.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageScripts.Size = new System.Drawing.Size(305, 283);
            this.tabPageScripts.TabIndex = 2;
            this.tabPageScripts.Text = "Scripts";
            // 
            // buttonSelectCustom3
            // 
            this.buttonSelectCustom3.Location = new System.Drawing.Point(161, 209);
            this.buttonSelectCustom3.Name = "buttonSelectCustom3";
            this.buttonSelectCustom3.Size = new System.Drawing.Size(32, 23);
            this.buttonSelectCustom3.TabIndex = 12;
            this.buttonSelectCustom3.Text = "...";
            this.toolTip1.SetToolTip(this.buttonSelectCustom3, "Change association");
            this.buttonSelectCustom3.UseVisualStyleBackColor = true;
            this.buttonSelectCustom3.Click += new System.EventHandler(this.buttonSelectCustom_Click);
            // 
            // buttonSelectCustom2
            // 
            this.buttonSelectCustom2.Location = new System.Drawing.Point(161, 180);
            this.buttonSelectCustom2.Name = "buttonSelectCustom2";
            this.buttonSelectCustom2.Size = new System.Drawing.Size(32, 23);
            this.buttonSelectCustom2.TabIndex = 12;
            this.buttonSelectCustom2.Text = "...";
            this.toolTip1.SetToolTip(this.buttonSelectCustom2, "Change association");
            this.buttonSelectCustom2.UseVisualStyleBackColor = true;
            this.buttonSelectCustom2.Click += new System.EventHandler(this.buttonSelectCustom_Click);
            // 
            // buttonSelectCustom1
            // 
            this.buttonSelectCustom1.Location = new System.Drawing.Point(161, 151);
            this.buttonSelectCustom1.Name = "buttonSelectCustom1";
            this.buttonSelectCustom1.Size = new System.Drawing.Size(32, 23);
            this.buttonSelectCustom1.TabIndex = 12;
            this.buttonSelectCustom1.Text = "...";
            this.toolTip1.SetToolTip(this.buttonSelectCustom1, "Change association");
            this.buttonSelectCustom1.UseVisualStyleBackColor = true;
            this.buttonSelectCustom1.Click += new System.EventHandler(this.buttonSelectCustom_Click);
            // 
            // buttonCustom3
            // 
            this.buttonCustom3.Location = new System.Drawing.Point(7, 209);
            this.buttonCustom3.Name = "buttonCustom3";
            this.buttonCustom3.Size = new System.Drawing.Size(148, 23);
            this.buttonCustom3.TabIndex = 11;
            this.buttonCustom3.Text = "<Custom 3>";
            this.toolTip1.SetToolTip(this.buttonCustom3, "Custom SQL or BAT file");
            this.buttonCustom3.UseVisualStyleBackColor = true;
            this.buttonCustom3.Click += new System.EventHandler(this.buttonCustom_Click);
            // 
            // buttonCustom2
            // 
            this.buttonCustom2.Location = new System.Drawing.Point(7, 180);
            this.buttonCustom2.Name = "buttonCustom2";
            this.buttonCustom2.Size = new System.Drawing.Size(148, 23);
            this.buttonCustom2.TabIndex = 10;
            this.buttonCustom2.Text = "<Custom 2>";
            this.toolTip1.SetToolTip(this.buttonCustom2, "Custom SQL or BAT file");
            this.buttonCustom2.UseVisualStyleBackColor = true;
            this.buttonCustom2.Click += new System.EventHandler(this.buttonCustom_Click);
            // 
            // buttonCustom1
            // 
            this.buttonCustom1.Location = new System.Drawing.Point(7, 151);
            this.buttonCustom1.Name = "buttonCustom1";
            this.buttonCustom1.Size = new System.Drawing.Size(148, 23);
            this.buttonCustom1.TabIndex = 9;
            this.buttonCustom1.Text = "<Custom 1>";
            this.toolTip1.SetToolTip(this.buttonCustom1, "Custom SQL or BAT file");
            this.buttonCustom1.UseVisualStyleBackColor = true;
            this.buttonCustom1.Click += new System.EventHandler(this.buttonCustom_Click);
            // 
            // textBoxOldTentLimit
            // 
            this.textBoxOldTentLimit.Location = new System.Drawing.Point(160, 124);
            this.textBoxOldTentLimit.MaxLength = 4;
            this.textBoxOldTentLimit.Name = "textBoxOldTentLimit";
            this.textBoxOldTentLimit.Size = new System.Drawing.Size(47, 20);
            this.textBoxOldTentLimit.TabIndex = 7;
            this.textBoxOldTentLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.textBoxOldTentLimit, "Day limit");
            // 
            // buttonRemoveTents
            // 
            this.buttonRemoveTents.Location = new System.Drawing.Point(6, 122);
            this.buttonRemoveTents.Name = "buttonRemoveTents";
            this.buttonRemoveTents.Size = new System.Drawing.Size(148, 23);
            this.buttonRemoveTents.TabIndex = 4;
            this.buttonRemoveTents.Text = "Remove old tents";
            this.toolTip1.SetToolTip(this.buttonRemoveTents, "Remove tents from DB\r\nolder than X days");
            this.buttonRemoveTents.UseVisualStyleBackColor = true;
            this.buttonRemoveTents.Click += new System.EventHandler(this.buttonRemoveTents_Click);
            // 
            // textBoxCmdStatus
            // 
            this.textBoxCmdStatus.AcceptsReturn = true;
            this.textBoxCmdStatus.AcceptsTab = true;
            this.textBoxCmdStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCmdStatus.Location = new System.Drawing.Point(7, 238);
            this.textBoxCmdStatus.MaxLength = 4096;
            this.textBoxCmdStatus.Multiline = true;
            this.textBoxCmdStatus.Name = "textBoxCmdStatus";
            this.textBoxCmdStatus.ReadOnly = true;
            this.textBoxCmdStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxCmdStatus.Size = new System.Drawing.Size(292, 51);
            this.textBoxCmdStatus.TabIndex = 8;
            // 
            // textBoxOldBodyLimit
            // 
            this.textBoxOldBodyLimit.Location = new System.Drawing.Point(160, 95);
            this.textBoxOldBodyLimit.MaxLength = 4;
            this.textBoxOldBodyLimit.Name = "textBoxOldBodyLimit";
            this.textBoxOldBodyLimit.Size = new System.Drawing.Size(47, 20);
            this.textBoxOldBodyLimit.TabIndex = 6;
            this.textBoxOldBodyLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.textBoxOldBodyLimit, "Day limit");
            // 
            // textBoxVehicleMax
            // 
            this.textBoxVehicleMax.ImeMode = System.Windows.Forms.ImeMode.On;
            this.textBoxVehicleMax.Location = new System.Drawing.Point(160, 66);
            this.textBoxVehicleMax.MaxLength = 4;
            this.textBoxVehicleMax.Name = "textBoxVehicleMax";
            this.textBoxVehicleMax.Size = new System.Drawing.Size(47, 20);
            this.textBoxVehicleMax.TabIndex = 5;
            this.textBoxVehicleMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.textBoxVehicleMax, "Expected vehicle count");
            // 
            // buttonRemoveBodies
            // 
            this.buttonRemoveBodies.Location = new System.Drawing.Point(6, 93);
            this.buttonRemoveBodies.Name = "buttonRemoveBodies";
            this.buttonRemoveBodies.Size = new System.Drawing.Size(148, 23);
            this.buttonRemoveBodies.TabIndex = 3;
            this.buttonRemoveBodies.Text = "Remove old bodies";
            this.toolTip1.SetToolTip(this.buttonRemoveBodies, "Remove dead survivors from the DB\r\nolder than X days.");
            this.buttonRemoveBodies.UseVisualStyleBackColor = true;
            this.buttonRemoveBodies.Click += new System.EventHandler(this.buttonRemoveBodies_Click);
            // 
            // buttonSpawnNew
            // 
            this.buttonSpawnNew.Location = new System.Drawing.Point(6, 64);
            this.buttonSpawnNew.Name = "buttonSpawnNew";
            this.buttonSpawnNew.Size = new System.Drawing.Size(148, 23);
            this.buttonSpawnNew.TabIndex = 2;
            this.buttonSpawnNew.Text = "Spawn new vehicles";
            this.buttonSpawnNew.UseVisualStyleBackColor = true;
            this.buttonSpawnNew.Click += new System.EventHandler(this.buttonSpawnNew_Click);
            // 
            // buttonBackup
            // 
            this.buttonBackup.Location = new System.Drawing.Point(6, 6);
            this.buttonBackup.Name = "buttonBackup";
            this.buttonBackup.Size = new System.Drawing.Size(148, 23);
            this.buttonBackup.TabIndex = 1;
            this.buttonBackup.Text = "Backup database";
            this.buttonBackup.UseVisualStyleBackColor = true;
            this.buttonBackup.Click += new System.EventHandler(this.buttonBackup_Click);
            // 
            // buttonRemoveDestroyed
            // 
            this.buttonRemoveDestroyed.Location = new System.Drawing.Point(6, 35);
            this.buttonRemoveDestroyed.Name = "buttonRemoveDestroyed";
            this.buttonRemoveDestroyed.Size = new System.Drawing.Size(148, 23);
            this.buttonRemoveDestroyed.TabIndex = 1;
            this.buttonRemoveDestroyed.Text = "Remove destroyed vehicles";
            this.buttonRemoveDestroyed.UseVisualStyleBackColor = true;
            this.buttonRemoveDestroyed.Click += new System.EventHandler(this.buttonRemoveDestroyed_Click);
            // 
            // tabPageVehicles
            // 
            this.tabPageVehicles.BackColor = System.Drawing.Color.Transparent;
            this.tabPageVehicles.Controls.Add(this.dataGridViewVehicleTypes);
            this.tabPageVehicles.Location = new System.Drawing.Point(4, 22);
            this.tabPageVehicles.Name = "tabPageVehicles";
            this.tabPageVehicles.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageVehicles.Size = new System.Drawing.Size(305, 283);
            this.tabPageVehicles.TabIndex = 4;
            this.tabPageVehicles.Text = "Vehicles";
            // 
            // dataGridViewVehicleTypes
            // 
            this.dataGridViewVehicleTypes.AllowUserToAddRows = false;
            this.dataGridViewVehicleTypes.AllowUserToDeleteRows = false;
            this.dataGridViewVehicleTypes.AllowUserToResizeRows = false;
            this.dataGridViewVehicleTypes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewVehicleTypes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColGVVTShow,
            this.ColGVVTClassName,
            this.ColGVVTType});
            this.dataGridViewVehicleTypes.ContextMenuStrip = this.contextMenuStripResetTypes;
            this.dataGridViewVehicleTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewVehicleTypes.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewVehicleTypes.MultiSelect = false;
            this.dataGridViewVehicleTypes.Name = "dataGridViewVehicleTypes";
            this.dataGridViewVehicleTypes.RowHeadersVisible = false;
            this.dataGridViewVehicleTypes.ShowEditingIcon = false;
            this.dataGridViewVehicleTypes.Size = new System.Drawing.Size(299, 277);
            this.dataGridViewVehicleTypes.TabIndex = 0;
            this.dataGridViewVehicleTypes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewVehicleTypes_CellContentClick);
            this.dataGridViewVehicleTypes.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewVehicleTypes_CellValueChanged);
            this.dataGridViewVehicleTypes.ColumnHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewVehicleTypes_ColumnHeaderMouseDoubleClick);
            // 
            // ColGVVTShow
            // 
            this.ColGVVTShow.FillWeight = 45.68528F;
            this.ColGVVTShow.HeaderText = "Show";
            this.ColGVVTShow.Name = "ColGVVTShow";
            this.ColGVVTShow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ColGVVTClassName
            // 
            this.ColGVVTClassName.FillWeight = 127.1574F;
            this.ColGVVTClassName.HeaderText = "ClassName";
            this.ColGVVTClassName.Name = "ColGVVTClassName";
            this.ColGVVTClassName.ReadOnly = true;
            // 
            // ColGVVTType
            // 
            this.ColGVVTType.FillWeight = 127.1574F;
            this.ColGVVTType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ColGVVTType.HeaderText = "Type";
            this.ColGVVTType.Items.AddRange(new object[] {
            "Air",
            "Atv",
            "Bicycle",
            "Boat",
            "Bus",
            "Car",
            "Helicopter",
            "Motorcycle",
            "Tractor",
            "Truck",
            "UAZ"});
            this.ColGVVTType.Name = "ColGVVTType";
            this.ColGVVTType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // tabPageDeployables
            // 
            this.tabPageDeployables.BackColor = System.Drawing.Color.Transparent;
            this.tabPageDeployables.Controls.Add(this.dataGridViewDeployableTypes);
            this.tabPageDeployables.Location = new System.Drawing.Point(4, 22);
            this.tabPageDeployables.Name = "tabPageDeployables";
            this.tabPageDeployables.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDeployables.Size = new System.Drawing.Size(305, 283);
            this.tabPageDeployables.TabIndex = 5;
            this.tabPageDeployables.Text = "Deployables";
            // 
            // dataGridViewDeployableTypes
            // 
            this.dataGridViewDeployableTypes.AllowUserToAddRows = false;
            this.dataGridViewDeployableTypes.AllowUserToDeleteRows = false;
            this.dataGridViewDeployableTypes.AllowUserToResizeRows = false;
            this.dataGridViewDeployableTypes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewDeployableTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewDeployableTypes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColGVDTShow,
            this.ColGVDTClassName,
            this.ColGVDTType});
            this.dataGridViewDeployableTypes.ContextMenuStrip = this.contextMenuStripResetTypes;
            this.dataGridViewDeployableTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDeployableTypes.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewDeployableTypes.MultiSelect = false;
            this.dataGridViewDeployableTypes.Name = "dataGridViewDeployableTypes";
            this.dataGridViewDeployableTypes.RowHeadersVisible = false;
            this.dataGridViewDeployableTypes.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewDeployableTypes.ShowEditingIcon = false;
            this.dataGridViewDeployableTypes.Size = new System.Drawing.Size(299, 277);
            this.dataGridViewDeployableTypes.TabIndex = 0;
            this.dataGridViewDeployableTypes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDeployableTypes_CellContentClick);
            this.dataGridViewDeployableTypes.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDeployableTypes_CellValueChanged);
            this.dataGridViewDeployableTypes.ColumnHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewDeployableTypes_ColumnHeaderMouseDoubleClick);
            // 
            // ColGVDTShow
            // 
            this.ColGVDTShow.FillWeight = 45.68528F;
            this.ColGVDTShow.HeaderText = "Show";
            this.ColGVDTShow.Name = "ColGVDTShow";
            this.ColGVDTShow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ColGVDTClassName
            // 
            this.ColGVDTClassName.FillWeight = 127.1574F;
            this.ColGVDTClassName.HeaderText = "ClassName";
            this.ColGVDTClassName.Name = "ColGVDTClassName";
            this.ColGVDTClassName.ReadOnly = true;
            // 
            // ColGVDTType
            // 
            this.ColGVDTType.FillWeight = 127.1574F;
            this.ColGVDTType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ColGVDTType.HeaderText = "Type";
            this.ColGVDTType.Items.AddRange(new object[] {
            "Unknown",
            "Tent",
            "Stach",
            "SmallBuild",
            "LargeBuild",
            "Car",
            "Truck",
            "Helicopter",
            "Air",
            "Boat"});
            this.ColGVDTType.Name = "ColGVDTType";
            this.ColGVDTType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // tabPagePlayers
            // 
            this.tabPagePlayers.Controls.Add(this.splitContainer2);
            this.tabPagePlayers.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlayers.Name = "tabPagePlayers";
            this.tabPagePlayers.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePlayers.Size = new System.Drawing.Size(305, 283);
            this.tabPagePlayers.TabIndex = 6;
            this.tabPagePlayers.Text = "Online";
            this.tabPagePlayers.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dataGridViewPlayers);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dataGridViewAdmins);
            this.splitContainer2.Size = new System.Drawing.Size(299, 277);
            this.splitContainer2.SplitterDistance = 200;
            this.splitContainer2.TabIndex = 0;
            // 
            // dataGridViewPlayers
            // 
            this.dataGridViewPlayers.AllowUserToAddRows = false;
            this.dataGridViewPlayers.AllowUserToDeleteRows = false;
            this.dataGridViewPlayers.AllowUserToResizeRows = false;
            this.dataGridViewPlayers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewPlayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewPlayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPlayers.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewPlayers.MultiSelect = false;
            this.dataGridViewPlayers.Name = "dataGridViewPlayers";
            this.dataGridViewPlayers.ReadOnly = true;
            this.dataGridViewPlayers.RowHeadersVisible = false;
            this.dataGridViewPlayers.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewPlayers.RowTemplate.ContextMenuStrip = this.contextMenuPlayersOnline;
            this.dataGridViewPlayers.ShowEditingIcon = false;
            this.dataGridViewPlayers.Size = new System.Drawing.Size(299, 200);
            this.dataGridViewPlayers.TabIndex = 0;
            // 
            // dataGridViewAdmins
            // 
            this.dataGridViewAdmins.AllowUserToAddRows = false;
            this.dataGridViewAdmins.AllowUserToDeleteRows = false;
            this.dataGridViewAdmins.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewAdmins.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewAdmins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAdmins.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewAdmins.MultiSelect = false;
            this.dataGridViewAdmins.Name = "dataGridViewAdmins";
            this.dataGridViewAdmins.ReadOnly = true;
            this.dataGridViewAdmins.RowHeadersVisible = false;
            this.dataGridViewAdmins.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewAdmins.ShowEditingIcon = false;
            this.dataGridViewAdmins.Size = new System.Drawing.Size(299, 73);
            this.dataGridViewAdmins.TabIndex = 0;
            // 
            // tabPageSetup
            // 
            this.tabPageSetup.Controls.Add(this.groupBox3);
            this.tabPageSetup.Controls.Add(this.groupBox2);
            this.tabPageSetup.Controls.Add(this.groupBox1);
            this.tabPageSetup.Controls.Add(this.cbCartographer);
            this.tabPageSetup.Location = new System.Drawing.Point(4, 22);
            this.tabPageSetup.Name = "tabPageSetup";
            this.tabPageSetup.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSetup.Size = new System.Drawing.Size(305, 283);
            this.tabPageSetup.TabIndex = 7;
            this.tabPageSetup.Text = "Setup";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonReconnectrCon);
            this.groupBox3.Location = new System.Drawing.Point(6, 176);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(180, 51);
            this.groupBox3.TabIndex = 113;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "rCon";
            // 
            // buttonReconnectrCon
            // 
            this.buttonReconnectrCon.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonReconnectrCon.Enabled = false;
            this.buttonReconnectrCon.Location = new System.Drawing.Point(40, 19);
            this.buttonReconnectrCon.Name = "buttonReconnectrCon";
            this.buttonReconnectrCon.Size = new System.Drawing.Size(100, 23);
            this.buttonReconnectrCon.TabIndex = 108;
            this.buttonReconnectrCon.Text = "Reconnect";
            this.buttonReconnectrCon.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericDBRefreshRate);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.comboSelectInstance);
            this.groupBox2.Location = new System.Drawing.Point(6, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(180, 78);
            this.groupBox2.TabIndex = 112;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database";
            // 
            // numericDBRefreshRate
            // 
            this.numericDBRefreshRate.DecimalPlaces = 1;
            this.numericDBRefreshRate.Location = new System.Drawing.Point(109, 45);
            this.numericDBRefreshRate.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericDBRefreshRate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericDBRefreshRate.Name = "numericDBRefreshRate";
            this.numericDBRefreshRate.Size = new System.Drawing.Size(57, 20);
            this.numericDBRefreshRate.TabIndex = 108;
            this.numericDBRefreshRate.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericDBRefreshRate.ValueChanged += new System.EventHandler(this.numericDBRefreshRate_ValueChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(38, 47);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 13);
            this.label15.TabIndex = 109;
            this.label15.Text = "Refresh rate";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 21);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 13);
            this.label12.TabIndex = 106;
            this.label12.Text = "Select Instance Id";
            // 
            // comboSelectInstance
            // 
            this.comboSelectInstance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSelectInstance.Enabled = false;
            this.comboSelectInstance.FormattingEnabled = true;
            this.comboSelectInstance.Location = new System.Drawing.Point(109, 18);
            this.comboSelectInstance.Name = "comboSelectInstance";
            this.comboSelectInstance.Size = new System.Drawing.Size(57, 21);
            this.comboSelectInstance.TabIndex = 107;
            this.toolTip1.SetToolTip(this.comboSelectInstance, "Instance IDs found in DB");
            this.comboSelectInstance.SelectedValueChanged += new System.EventHandler(this.comboSelectInstance_SelectedValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboSelectEpochWorld);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Location = new System.Drawing.Point(6, 116);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(180, 51);
            this.groupBox1.TabIndex = 111;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Epoch";
            // 
            // comboSelectEpochWorld
            // 
            this.comboSelectEpochWorld.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSelectEpochWorld.Enabled = false;
            this.comboSelectEpochWorld.FormattingEnabled = true;
            this.comboSelectEpochWorld.Location = new System.Drawing.Point(109, 18);
            this.comboSelectEpochWorld.Name = "comboSelectEpochWorld";
            this.comboSelectEpochWorld.Size = new System.Drawing.Size(57, 21);
            this.comboSelectEpochWorld.TabIndex = 110;
            this.toolTip1.SetToolTip(this.comboSelectEpochWorld, "Instance IDs found in DB");
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(14, 21);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(89, 13);
            this.label14.TabIndex = 109;
            this.label14.Text = "Select WorldMap";
            // 
            // cbCartographer
            // 
            this.cbCartographer.AutoSize = true;
            this.cbCartographer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbCartographer.ForeColor = System.Drawing.Color.Black;
            this.cbCartographer.Location = new System.Drawing.Point(6, 6);
            this.cbCartographer.Name = "cbCartographer";
            this.cbCartographer.Size = new System.Drawing.Size(78, 17);
            this.cbCartographer.TabIndex = 8;
            this.cbCartographer.Text = "Cartograph";
            this.toolTip1.SetToolTip(this.cbCartographer, "internal use, will be removed later");
            this.cbCartographer.UseVisualStyleBackColor = true;
            // 
            // splitContainerChat
            // 
            this.splitContainerChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerChat.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerChat.IsSplitterFixed = true;
            this.splitContainerChat.Location = new System.Drawing.Point(0, 0);
            this.splitContainerChat.Name = "splitContainerChat";
            this.splitContainerChat.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerChat.Panel1
            // 
            this.splitContainerChat.Panel1.Controls.Add(this.richTextBoxChat);
            // 
            // splitContainerChat.Panel2
            // 
            this.splitContainerChat.Panel2.Controls.Add(this.textBoxChatInput);
            this.splitContainerChat.Panel2MinSize = 30;
            this.splitContainerChat.Size = new System.Drawing.Size(879, 84);
            this.splitContainerChat.SplitterDistance = 34;
            this.splitContainerChat.TabIndex = 2;
            // 
            // richTextBoxChat
            // 
            this.richTextBoxChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxChat.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxChat.Name = "richTextBoxChat";
            this.richTextBoxChat.ReadOnly = true;
            this.richTextBoxChat.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxChat.Size = new System.Drawing.Size(879, 34);
            this.richTextBoxChat.TabIndex = 1;
            this.richTextBoxChat.Text = "";
            // 
            // textBoxChatInput
            // 
            this.textBoxChatInput.AcceptsReturn = true;
            this.textBoxChatInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxChatInput.Location = new System.Drawing.Point(0, 0);
            this.textBoxChatInput.Multiline = true;
            this.textBoxChatInput.Name = "textBoxChatInput";
            this.textBoxChatInput.Size = new System.Drawing.Size(879, 46);
            this.textBoxChatInput.TabIndex = 0;
            this.textBoxChatInput.TextChanged += new System.EventHandler(this.textBoxChatInput_TextChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 442);
            this.Controls.Add(this.toolStripContainer3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(770, 480);
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.Form1Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataSetBindingSource)).EndInit();
            this.contextMenuStripResetTypes.ResumeLayout(false);
            this.contextMenuStripSpawn.ResumeLayout(false);
            this.contextMenuStripVehicle.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer2.ContentPanel.ResumeLayout(false);
            this.toolStripContainer2.ResumeLayout(false);
            this.toolStripContainer2.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.contextMenuPlayersOnline.ResumeLayout(false);
            this.toolStripContainer3.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer3.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer3.ContentPanel.ResumeLayout(false);
            this.toolStripContainer3.ResumeLayout(false);
            this.toolStripContainer3.PerformLayout();
            this.contextMenuStripPlayerMenu.ResumeLayout(false);
            this.splitContainerGlobal.Panel1.ResumeLayout(false);
            this.splitContainerGlobal.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerGlobal)).EndInit();
            this.splitContainerGlobal.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelCnx.ResumeLayout(false);
            this.groupBoxrCon.ResumeLayout(false);
            this.groupBoxrCon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownrConPort)).EndInit();
            this.groupBoxDB.ResumeLayout(false);
            this.groupBoxDB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDBPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInstanceId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMaps)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageDisplay.ResumeLayout(false);
            this.tabPageDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMagLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLastUpdated)).EndInit();
            this.groupBoxInfo.ResumeLayout(false);
            this.tabPageScripts.ResumeLayout(false);
            this.tabPageScripts.PerformLayout();
            this.tabPageVehicles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVehicleTypes)).EndInit();
            this.tabPageDeployables.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDeployableTypes)).EndInit();
            this.tabPagePlayers.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPlayers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAdmins)).EndInit();
            this.tabPageSetup.ResumeLayout(false);
            this.tabPageSetup.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericDBRefreshRate)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainerChat.Panel1.ResumeLayout(false);
            this.splitContainerChat.Panel2.ResumeLayout(false);
            this.splitContainerChat.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerChat)).EndInit();
            this.splitContainerChat.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MySplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox textBoxDBPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxDBBaseName;
        private System.Windows.Forms.TextBox textBoxDBURL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDBUser;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.GroupBox groupBoxInfo;
        private System.ComponentModel.BackgroundWorker bgWorkerDatabase;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageDisplay;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.GroupBox groupBoxDB;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage tabPageScripts;
        private System.Windows.Forms.TextBox textBoxOldBodyLimit;
        private System.Windows.Forms.TextBox textBoxVehicleMax;
        private System.Windows.Forms.Button buttonRemoveBodies;
        private System.Windows.Forms.Button buttonSpawnNew;
        private System.Windows.Forms.Button buttonRemoveDestroyed;
        private System.Windows.Forms.TextBox textBoxCmdStatus;
        private System.Windows.Forms.TextBox textBoxOldTentLimit;
        private System.Windows.Forms.Button buttonRemoveTents;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.DataGridView dataGridViewMaps;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripVehicle;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDeleteVehicle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripSpawn;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDeleteSpawn;
        private System.Windows.Forms.TabPage tabPageVehicles;
        private System.Windows.Forms.DataGridView dataGridViewVehicleTypes;
        private System.Windows.Forms.ComboBox comboBoxGameType;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.BindingSource dataSetBindingSource;
        private System.ComponentModel.BackgroundWorker bgWorkerMapZoom;
        private System.Windows.Forms.TabPage tabPageDeployables;
        private System.Windows.Forms.DataGridView dataGridViewDeployableTypes;
        private System.Windows.Forms.Button buttonSelectCustom3;
        private System.Windows.Forms.Button buttonSelectCustom2;
        private System.Windows.Forms.Button buttonSelectCustom1;
        private System.Windows.Forms.Button buttonCustom3;
        private System.Windows.Forms.Button buttonCustom2;
        private System.Windows.Forms.Button buttonCustom1;
        private System.Windows.Forms.Button buttonBackup;
        //
        private System.Windows.Forms.DataGridViewTextBoxColumn ColGVMID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColGVMName;
        private System.Windows.Forms.DataGridViewButtonColumn ColGVMChoosePath;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColGVMPath;
        //
        //
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColGVVTShow;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColGVVTClassName;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColGVVTType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColGVDTShow;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColGVDTClassName;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColGVDTType;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripResetTypes;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemResetTypes;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.NumericUpDown numericUpDownInstanceId;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMapMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuMapAddVehicle;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuMapTeleportPlayer;
        private System.Windows.Forms.TrackBar trackBarLastUpdated;
        private System.Windows.Forms.Label labelLastUpdate;
        private System.Windows.Forms.ToolStripMenuItem ttootToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showPlayersAliveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showVehiclesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSpawnPointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDeployablesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapHelperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cartographerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showTrailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelWorld;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOnline;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAlive;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemVehicle;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSpawn;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDeployable;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMapHelper;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCartographer;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTrails;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelCnx;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusOnline;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusAlive;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusVehicle;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusSpawn;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusWorld;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusTrail;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusDeployable;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusChat;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusCoordMap;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusCoordDB;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusMapHelper;
        private System.Windows.Forms.ToolStripContainer toolStripContainer2;
        private System.Windows.Forms.ToolStripContainer toolStripContainer3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem repairRefuelVehicleToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bgWorkerLoadTiles;
        private System.Windows.Forms.TabPage tabPagePlayers;
        private System.Windows.Forms.DataGridView dataGridViewPlayers;
        private System.Windows.Forms.ContextMenuStrip contextMenuPlayersOnline;
        private System.Windows.Forms.ToolStripMenuItem messageToPlayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kickPlayerToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerGlobal;
        private System.Windows.Forms.RichTextBox richTextBoxChat;
        private System.Windows.Forms.TextBox textBoxChatInput;
        private System.Windows.Forms.SplitContainer splitContainerChat;
        private System.Windows.Forms.Panel panelCnx;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBoxrCon;
        private System.Windows.Forms.TextBox textBoxrConPassword;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxrConURL;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDownDBPort;
        private System.Windows.Forms.NumericUpDown numericUpDownrConPort;
        private System.Windows.Forms.DataGridView dataGridViewAdmins;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labelMagLevel;
        private System.Windows.Forms.TrackBar trackBarMagLevel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusDead;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPlayerMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHeal;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusNews;
        private System.Windows.Forms.TabPage tabPageSetup;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonReconnectrCon;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboSelectInstance;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboSelectEpochWorld;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox cbCartographer;
        private System.Windows.Forms.NumericUpDown numericDBRefreshRate;
        private System.Windows.Forms.Label label15;
        private System.ComponentModel.BackgroundWorker bgWorkerFocus;
    }
}

