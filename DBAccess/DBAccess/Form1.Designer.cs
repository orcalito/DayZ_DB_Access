namespace DBAccess
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.contextMenuStripAddVehicle = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemAddVehicle = new System.Windows.Forms.ToolStripMenuItem();
            this.dataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.contextMenuStripResetTypes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemResetTypes = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripSpawn = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemDeleteSpawn = new System.Windows.Forms.ToolStripMenuItem();
            this.bgWorkerDatabase = new System.ComponentModel.BackgroundWorker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.trackBarLastUpdated = new System.Windows.Forms.TrackBar();
            this.cbCartographer = new System.Windows.Forms.CheckBox();
            this.buttonSelectCustom3 = new System.Windows.Forms.Button();
            this.buttonSelectCustom2 = new System.Windows.Forms.Button();
            this.buttonSelectCustom1 = new System.Windows.Forms.Button();
            this.buttonCustom3 = new System.Windows.Forms.Button();
            this.buttonCustom2 = new System.Windows.Forms.Button();
            this.buttonCustom1 = new System.Windows.Forms.Button();
            this.textBoxOldTentLimit = new System.Windows.Forms.TextBox();
            this.buttonRemoveTents = new System.Windows.Forms.Button();
            this.textBoxOldBodyLimit = new System.Windows.Forms.TextBox();
            this.textBoxVehicleMax = new System.Windows.Forms.TextBox();
            this.buttonRemoveBodies = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStripVehicle = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemDeleteVehicle = new System.Windows.Forms.ToolStripMenuItem();
            this.bgWorkerFast = new System.ComponentModel.BackgroundWorker();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusWorld = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusOnline = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusAlive = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusVehicle = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusSpawn = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusDeployable = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusTrail = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusMapHelper = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusHelp = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusCoordMap = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusCoordDB = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusCnx = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusWorldL = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusOnlineL = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusAliveL = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusVehicleL = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusSpawnL = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusDeployableL = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel9 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusCoordMapL = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusCoordDBL = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainer1 = new DBAccess.MySplitContainer();
            this.dataGridViewMaps = new System.Windows.Forms.DataGridView();
            this.ColGVMID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColGVMName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColGVMChoosePath = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColGVMPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxCnx = new System.Windows.Forms.GroupBox();
            this.numericUpDownInstanceId = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxGameType = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textBoxURL = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxBaseName = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageDisplay = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.labelLastUpdate = new System.Windows.Forms.Label();
            this.groupBoxInfo = new System.Windows.Forms.GroupBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabPageScripts = new System.Windows.Forms.TabPage();
            this.textBoxCmdStatus = new System.Windows.Forms.TextBox();
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
            this.toolStripContainer3 = new System.Windows.Forms.ToolStripContainer();
            this.contextMenuStripAddVehicle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetBindingSource)).BeginInit();
            this.contextMenuStripResetTypes.SuspendLayout();
            this.contextMenuStripSpawn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLastUpdated)).BeginInit();
            this.contextMenuStripVehicle.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMaps)).BeginInit();
            this.groupBoxCnx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInstanceId)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageDisplay.SuspendLayout();
            this.groupBoxInfo.SuspendLayout();
            this.tabPageScripts.SuspendLayout();
            this.tabPageVehicles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVehicleTypes)).BeginInit();
            this.tabPageDeployables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDeployableTypes)).BeginInit();
            this.toolStripContainer2.ContentPanel.SuspendLayout();
            this.toolStripContainer2.SuspendLayout();
            this.toolStripContainer3.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer3.ContentPanel.SuspendLayout();
            this.toolStripContainer3.SuspendLayout();
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
            // contextMenuStripAddVehicle
            // 
            this.contextMenuStripAddVehicle.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddVehicle});
            this.contextMenuStripAddVehicle.Name = "contextMenuStripAddVehicle";
            this.contextMenuStripAddVehicle.Size = new System.Drawing.Size(163, 26);
            this.contextMenuStripAddVehicle.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripAddVehicle_Opening);
            // 
            // toolStripMenuItemAddVehicle
            // 
            this.toolStripMenuItemAddVehicle.Name = "toolStripMenuItemAddVehicle";
            this.toolStripMenuItemAddVehicle.Size = new System.Drawing.Size(162, 22);
            this.toolStripMenuItemAddVehicle.Text = "Add SpawnPoint";
            this.toolStripMenuItemAddVehicle.Click += new System.EventHandler(this.toolStripMenuItemAddVehicle_Click);
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
            // cbCartographer
            // 
            this.cbCartographer.AutoSize = true;
            this.cbCartographer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbCartographer.ForeColor = System.Drawing.Color.Black;
            this.cbCartographer.Location = new System.Drawing.Point(6, 37);
            this.cbCartographer.Name = "cbCartographer";
            this.cbCartographer.Size = new System.Drawing.Size(78, 17);
            this.cbCartographer.TabIndex = 7;
            this.cbCartographer.Text = "Cartograph";
            this.toolTip1.SetToolTip(this.cbCartographer, "internal use, will be removed later");
            this.cbCartographer.UseVisualStyleBackColor = true;
            this.cbCartographer.CheckedChanged += new System.EventHandler(this.cbCartographer_CheckedChanged);
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
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // contextMenuStripVehicle
            // 
            this.contextMenuStripVehicle.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemDeleteVehicle});
            this.contextMenuStripVehicle.Name = "contextMenuStripVehicle";
            this.contextMenuStripVehicle.Size = new System.Drawing.Size(148, 26);
            // 
            // toolStripMenuItemDeleteVehicle
            // 
            this.toolStripMenuItemDeleteVehicle.Name = "toolStripMenuItemDeleteVehicle";
            this.toolStripMenuItemDeleteVehicle.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItemDeleteVehicle.Text = "Delete vehicle";
            this.toolStripMenuItemDeleteVehicle.Click += new System.EventHandler(this.toolStripMenuItemDelete_Click);
            // 
            // bgWorkerFast
            // 
            this.bgWorkerFast.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerFast_DoWork);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "sql";
            this.saveFileDialog1.Filter = "SQL Files|*.sql";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusWorld,
            this.toolStripStatusOnline,
            this.toolStripStatusAlive,
            this.toolStripStatusVehicle,
            this.toolStripStatusSpawn,
            this.toolStripStatusDeployable,
            this.toolStripStatusMapHelper,
            this.toolStripStatusTrail,
            this.toolStripStatusHelp,
            this.toolStripStatusCoordMap,
            this.toolStripStatusCoordDB});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 27);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusWorld
            // 
            this.toolStripStatusWorld.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusWorld.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusWorld.Image = global::DBAccess.Properties.Resources.World;
            this.toolStripStatusWorld.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusWorld.Name = "toolStripStatusWorld";
            this.toolStripStatusWorld.Size = new System.Drawing.Size(81, 22);
            this.toolStripStatusWorld.Text = "chernarus";
            this.toolStripStatusWorld.ToolTipText = "Set maps for each world";
            this.toolStripStatusWorld.Click += new System.EventHandler(this.toolStripStatusWorld_Click);
            // 
            // toolStripStatusOnline
            // 
            this.toolStripStatusOnline.AutoSize = false;
            this.toolStripStatusOnline.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusOnline.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusOnline.Image = global::DBAccess.Properties.Resources.iconOnline;
            this.toolStripStatusOnline.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusOnline.Name = "toolStripStatusOnline";
            this.toolStripStatusOnline.Size = new System.Drawing.Size(82, 22);
            this.toolStripStatusOnline.Spring = true;
            this.toolStripStatusOnline.Text = "888";
            this.toolStripStatusOnline.ToolTipText = "Show online players";
            this.toolStripStatusOnline.Click += new System.EventHandler(this.toolStripStatusOnline_Click);
            // 
            // toolStripStatusAlive
            // 
            this.toolStripStatusAlive.AutoSize = false;
            this.toolStripStatusAlive.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusAlive.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusAlive.Image = global::DBAccess.Properties.Resources.iconAlive;
            this.toolStripStatusAlive.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusAlive.Name = "toolStripStatusAlive";
            this.toolStripStatusAlive.Size = new System.Drawing.Size(82, 22);
            this.toolStripStatusAlive.Spring = true;
            this.toolStripStatusAlive.Text = "888";
            this.toolStripStatusAlive.ToolTipText = "Show alive players";
            this.toolStripStatusAlive.Click += new System.EventHandler(this.toolStripStatusAlive_Click);
            // 
            // toolStripStatusVehicle
            // 
            this.toolStripStatusVehicle.AutoSize = false;
            this.toolStripStatusVehicle.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusVehicle.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusVehicle.Image = global::DBAccess.Properties.Resources.Vehicle;
            this.toolStripStatusVehicle.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusVehicle.Name = "toolStripStatusVehicle";
            this.toolStripStatusVehicle.Size = new System.Drawing.Size(82, 22);
            this.toolStripStatusVehicle.Spring = true;
            this.toolStripStatusVehicle.Text = "888";
            this.toolStripStatusVehicle.ToolTipText = "Show vehicles";
            this.toolStripStatusVehicle.Click += new System.EventHandler(this.toolStripStatusVehicle_Click);
            // 
            // toolStripStatusSpawn
            // 
            this.toolStripStatusSpawn.AutoSize = false;
            this.toolStripStatusSpawn.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusSpawn.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusSpawn.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStatusSpawn.Image")));
            this.toolStripStatusSpawn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusSpawn.Name = "toolStripStatusSpawn";
            this.toolStripStatusSpawn.Size = new System.Drawing.Size(82, 22);
            this.toolStripStatusSpawn.Spring = true;
            this.toolStripStatusSpawn.Text = "888";
            this.toolStripStatusSpawn.ToolTipText = "Show spawn points";
            this.toolStripStatusSpawn.Click += new System.EventHandler(this.toolStripStatusSpawn_Click);
            // 
            // toolStripStatusDeployable
            // 
            this.toolStripStatusDeployable.AutoSize = false;
            this.toolStripStatusDeployable.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusDeployable.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusDeployable.Image = global::DBAccess.Properties.Resources.deployable;
            this.toolStripStatusDeployable.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusDeployable.Name = "toolStripStatusDeployable";
            this.toolStripStatusDeployable.Size = new System.Drawing.Size(82, 22);
            this.toolStripStatusDeployable.Spring = true;
            this.toolStripStatusDeployable.Text = "888";
            this.toolStripStatusDeployable.ToolTipText = "Show deployables";
            this.toolStripStatusDeployable.Click += new System.EventHandler(this.toolStripStatusDeployable_Click);
            // 
            // toolStripStatusTrail
            // 
            this.toolStripStatusTrail.AutoSize = false;
            this.toolStripStatusTrail.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusTrail.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusTrail.Image = global::DBAccess.Properties.Resources.Trail;
            this.toolStripStatusTrail.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusTrail.Name = "toolStripStatusTrail";
            this.toolStripStatusTrail.Size = new System.Drawing.Size(32, 22);
            this.toolStripStatusTrail.ToolTipText = "Display moves for players/vehicles";
            this.toolStripStatusTrail.Click += new System.EventHandler(this.toolStripStatusTrail_Click);
            // 
            // toolStripStatusMapHelper
            // 
            this.toolStripStatusMapHelper.AutoSize = false;
            this.toolStripStatusMapHelper.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusMapHelper.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusMapHelper.Image = global::DBAccess.Properties.Resources.Tool;
            this.toolStripStatusMapHelper.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusMapHelper.Name = "toolStripStatusMapHelper";
            this.toolStripStatusMapHelper.Size = new System.Drawing.Size(32, 22);
            this.toolStripStatusMapHelper.ToolTipText = "Set link between bitmap and the database coordinates";
            this.toolStripStatusMapHelper.Click += new System.EventHandler(this.toolStripStatusMapHelper_Click);
            // 
            // toolStripStatusHelp
            // 
            this.toolStripStatusHelp.AutoSize = false;
            this.toolStripStatusHelp.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusHelp.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusHelp.Image = global::DBAccess.Properties.Resources.help;
            this.toolStripStatusHelp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusHelp.Name = "toolStripStatusHelp";
            this.toolStripStatusHelp.Size = new System.Drawing.Size(32, 22);
            this.toolStripStatusHelp.Click += new System.EventHandler(this.toolStripStatusHelp_Click);
            // 
            // toolStripStatusCoordMap
            // 
            this.toolStripStatusCoordMap.AutoSize = false;
            this.toolStripStatusCoordMap.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusCoordMap.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusCoordMap.Image = global::DBAccess.Properties.Resources.Map;
            this.toolStripStatusCoordMap.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusCoordMap.Name = "toolStripStatusCoordMap";
            this.toolStripStatusCoordMap.Size = new System.Drawing.Size(98, 22);
            this.toolStripStatusCoordMap.Text = "-";
            this.toolStripStatusCoordMap.ToolTipText = "Map coordinates";
            // 
            // toolStripStatusCoordDB
            // 
            this.toolStripStatusCoordDB.AutoSize = false;
            this.toolStripStatusCoordDB.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusCoordDB.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.toolStripStatusCoordDB.Image = global::DBAccess.Properties.Resources.DB;
            this.toolStripStatusCoordDB.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusCoordDB.Name = "toolStripStatusCoordDB";
            this.toolStripStatusCoordDB.Size = new System.Drawing.Size(96, 22);
            this.toolStripStatusCoordDB.Text = "-";
            this.toolStripStatusCoordDB.ToolTipText = "DB coordinates";
            // 
            // toolStripStatusCnx
            // 
            this.toolStripStatusCnx.Name = "toolStripStatusCnx";
            this.toolStripStatusCnx.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripStatusWorldL
            // 
            this.toolStripStatusWorldL.Name = "toolStripStatusWorldL";
            this.toolStripStatusWorldL.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripStatusOnlineL
            // 
            this.toolStripStatusOnlineL.Name = "toolStripStatusOnlineL";
            this.toolStripStatusOnlineL.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripStatusAliveL
            // 
            this.toolStripStatusAliveL.Name = "toolStripStatusAliveL";
            this.toolStripStatusAliveL.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripStatusVehicleL
            // 
            this.toolStripStatusVehicleL.Name = "toolStripStatusVehicleL";
            this.toolStripStatusVehicleL.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripStatusSpawnL
            // 
            this.toolStripStatusSpawnL.Name = "toolStripStatusSpawnL";
            this.toolStripStatusSpawnL.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripStatusDeployableL
            // 
            this.toolStripStatusDeployableL.Name = "toolStripStatusDeployableL";
            this.toolStripStatusDeployableL.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripStatusLabel9
            // 
            this.toolStripStatusLabel9.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabel9.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedOuter;
            this.toolStripStatusLabel9.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusLabel9.Name = "toolStripStatusLabel9";
            this.toolStripStatusLabel9.Size = new System.Drawing.Size(75, 22);
            this.toolStripStatusLabel9.Spring = true;
            // 
            // toolStripStatusCoordMapL
            // 
            this.toolStripStatusCoordMapL.Name = "toolStripStatusCoordMapL";
            this.toolStripStatusCoordMapL.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripStatusCoordDBL
            // 
            this.toolStripStatusCoordDBL.Name = "toolStripStatusCoordDBL";
            this.toolStripStatusCoordDBL.Size = new System.Drawing.Size(23, 23);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(800, 495);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(800, 495);
            this.toolStripContainer1.TabIndex = 3;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanelVisible = false;
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
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewMaps);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxCnx);
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
            this.splitContainer1.Size = new System.Drawing.Size(800, 495);
            this.splitContainer1.SplitterDistance = 533;
            this.splitContainer1.TabIndex = 1;
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
            this.dataGridViewMaps.Size = new System.Drawing.Size(529, 491);
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
            // groupBoxCnx
            // 
            this.groupBoxCnx.Controls.Add(this.numericUpDownInstanceId);
            this.groupBoxCnx.Controls.Add(this.label10);
            this.groupBoxCnx.Controls.Add(this.comboBoxGameType);
            this.groupBoxCnx.Controls.Add(this.label13);
            this.groupBoxCnx.Controls.Add(this.label1);
            this.groupBoxCnx.Controls.Add(this.buttonConnect);
            this.groupBoxCnx.Controls.Add(this.textBoxURL);
            this.groupBoxCnx.Controls.Add(this.textBoxPort);
            this.groupBoxCnx.Controls.Add(this.textBoxPassword);
            this.groupBoxCnx.Controls.Add(this.label4);
            this.groupBoxCnx.Controls.Add(this.label3);
            this.groupBoxCnx.Controls.Add(this.textBoxUser);
            this.groupBoxCnx.Controls.Add(this.label5);
            this.groupBoxCnx.Controls.Add(this.textBoxBaseName);
            this.groupBoxCnx.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBoxCnx.Location = new System.Drawing.Point(112, 155);
            this.groupBoxCnx.Name = "groupBoxCnx";
            this.groupBoxCnx.Size = new System.Drawing.Size(312, 192);
            this.groupBoxCnx.TabIndex = 9;
            this.groupBoxCnx.TabStop = false;
            this.groupBoxCnx.Text = "Connection";
            // 
            // numericUpDownInstanceId
            // 
            this.numericUpDownInstanceId.Location = new System.Drawing.Point(255, 120);
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
            this.numericUpDownInstanceId.TabIndex = 107;
            this.numericUpDownInstanceId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(189, 123);
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
            "Classic",
            "Epoch"});
            this.comboBoxGameType.Location = new System.Drawing.Point(80, 120);
            this.comboBoxGameType.MaxDropDownItems = 4;
            this.comboBoxGameType.Name = "comboBoxGameType";
            this.comboBoxGameType.Size = new System.Drawing.Size(92, 21);
            this.comboBoxGameType.TabIndex = 6;
            this.comboBoxGameType.SelectedValueChanged += new System.EventHandler(this.comboBoxGameType_SelectedValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(5, 123);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(62, 13);
            this.label13.TabIndex = 106;
            this.label13.Text = "Game Type";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 100;
            this.label1.Text = "URL";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(121, 160);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(70, 23);
            this.buttonConnect.TabIndex = 7;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // textBoxURL
            // 
            this.textBoxURL.Location = new System.Drawing.Point(41, 16);
            this.textBoxURL.MaxLength = 256;
            this.textBoxURL.Name = "textBoxURL";
            this.textBoxURL.Size = new System.Drawing.Size(189, 20);
            this.textBoxURL.TabIndex = 0;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(255, 16);
            this.textBoxPort.MaxLength = 6;
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(50, 20);
            this.textBoxPort.TabIndex = 1;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(41, 94);
            this.textBoxPassword.MaxLength = 64;
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(187, 20);
            this.textBoxPassword.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 104;
            this.label4.Text = "Pass";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 102;
            this.label3.Text = "Base";
            // 
            // textBoxUser
            // 
            this.textBoxUser.Location = new System.Drawing.Point(41, 68);
            this.textBoxUser.MaxLength = 256;
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.Size = new System.Drawing.Size(187, 20);
            this.textBoxUser.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 103;
            this.label5.Text = "User";
            // 
            // textBoxBaseName
            // 
            this.textBoxBaseName.Location = new System.Drawing.Point(41, 42);
            this.textBoxBaseName.MaxLength = 256;
            this.textBoxBaseName.Name = "textBoxBaseName";
            this.textBoxBaseName.Size = new System.Drawing.Size(187, 20);
            this.textBoxBaseName.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageDisplay);
            this.tabControl1.Controls.Add(this.tabPageScripts);
            this.tabControl1.Controls.Add(this.tabPageVehicles);
            this.tabControl1.Controls.Add(this.tabPageDeployables);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(259, 491);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPageDisplay
            // 
            this.tabPageDisplay.BackColor = System.Drawing.Color.Transparent;
            this.tabPageDisplay.Controls.Add(this.label2);
            this.tabPageDisplay.Controls.Add(this.labelLastUpdate);
            this.tabPageDisplay.Controls.Add(this.trackBarLastUpdated);
            this.tabPageDisplay.Controls.Add(this.cbCartographer);
            this.tabPageDisplay.Controls.Add(this.groupBoxInfo);
            this.tabPageDisplay.Location = new System.Drawing.Point(4, 22);
            this.tabPageDisplay.Name = "tabPageDisplay";
            this.tabPageDisplay.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDisplay.Size = new System.Drawing.Size(251, 465);
            this.tabPageDisplay.TabIndex = 1;
            this.tabPageDisplay.Text = "Display";
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
            // groupBoxInfo
            // 
            this.groupBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxInfo.Controls.Add(this.propertyGrid1);
            this.groupBoxInfo.Location = new System.Drawing.Point(3, 60);
            this.groupBoxInfo.Name = "groupBoxInfo";
            this.groupBoxInfo.Size = new System.Drawing.Size(242, 404);
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
            this.propertyGrid1.Size = new System.Drawing.Size(236, 385);
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
            this.tabPageScripts.Size = new System.Drawing.Size(251, 465);
            this.tabPageScripts.TabIndex = 2;
            this.tabPageScripts.Text = "Scripts";
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
            this.textBoxCmdStatus.Size = new System.Drawing.Size(238, 215);
            this.textBoxCmdStatus.TabIndex = 8;
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
            this.tabPageVehicles.Size = new System.Drawing.Size(251, 465);
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
            this.dataGridViewVehicleTypes.Size = new System.Drawing.Size(245, 459);
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
            this.tabPageDeployables.Size = new System.Drawing.Size(251, 465);
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
            this.dataGridViewDeployableTypes.Size = new System.Drawing.Size(245, 459);
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
            "LargeBuild"});
            this.ColGVDTType.Name = "ColGVDTType";
            this.ColGVDTType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
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
            this.toolStripContainer2.ContentPanel.Size = new System.Drawing.Size(800, 495);
            this.toolStripContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer2.LeftToolStripPanelVisible = false;
            this.toolStripContainer2.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer2.Name = "toolStripContainer2";
            this.toolStripContainer2.RightToolStripPanelVisible = false;
            this.toolStripContainer2.Size = new System.Drawing.Size(800, 495);
            this.toolStripContainer2.TabIndex = 4;
            this.toolStripContainer2.Text = "toolStripContainer2";
            this.toolStripContainer2.TopToolStripPanelVisible = false;
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
            this.toolStripContainer3.ContentPanel.Size = new System.Drawing.Size(800, 495);
            this.toolStripContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer3.LeftToolStripPanelVisible = false;
            this.toolStripContainer3.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer3.Name = "toolStripContainer3";
            this.toolStripContainer3.RightToolStripPanelVisible = false;
            this.toolStripContainer3.Size = new System.Drawing.Size(800, 522);
            this.toolStripContainer3.TabIndex = 5;
            this.toolStripContainer3.Text = "toolStripContainer3";
            this.toolStripContainer3.TopToolStripPanelVisible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 522);
            this.Controls.Add(this.toolStripContainer3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(628, 497);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.contextMenuStripAddVehicle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataSetBindingSource)).EndInit();
            this.contextMenuStripResetTypes.ResumeLayout(false);
            this.contextMenuStripSpawn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLastUpdated)).EndInit();
            this.contextMenuStripVehicle.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMaps)).EndInit();
            this.groupBoxCnx.ResumeLayout(false);
            this.groupBoxCnx.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInstanceId)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageDisplay.ResumeLayout(false);
            this.tabPageDisplay.PerformLayout();
            this.groupBoxInfo.ResumeLayout(false);
            this.tabPageScripts.ResumeLayout(false);
            this.tabPageScripts.PerformLayout();
            this.tabPageVehicles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVehicleTypes)).EndInit();
            this.tabPageDeployables.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDeployableTypes)).EndInit();
            this.toolStripContainer2.ContentPanel.ResumeLayout(false);
            this.toolStripContainer2.ResumeLayout(false);
            this.toolStripContainer2.PerformLayout();
            this.toolStripContainer3.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer3.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer3.ContentPanel.ResumeLayout(false);
            this.toolStripContainer3.ResumeLayout(false);
            this.toolStripContainer3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MySplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxBaseName;
        private System.Windows.Forms.TextBox textBoxURL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxUser;
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
        private System.Windows.Forms.GroupBox groupBoxCnx;
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
        private System.ComponentModel.BackgroundWorker bgWorkerFast;
        private System.Windows.Forms.TabPage tabPageDeployables;
        private System.Windows.Forms.DataGridView dataGridViewDeployableTypes;
        private System.Windows.Forms.CheckBox cbCartographer;
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
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusWorldL;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusOnlineL;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusAliveL;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusVehicleL;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusSpawnL;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusCnx;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.NumericUpDown numericUpDownInstanceId;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusCoordMapL;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusCoordDBL;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusDeployableL;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAddVehicle;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddVehicle;
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
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel9;
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
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusHelp;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusCoordMap;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusCoordDB;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusMapHelper;
        private System.Windows.Forms.ToolStripContainer toolStripContainer2;
        private System.Windows.Forms.ToolStripContainer toolStripContainer3;
        private System.Windows.Forms.Label label2;
    }
}

