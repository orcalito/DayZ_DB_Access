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
            this.splitContainer1 = new DBAccess.Form1.MySplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxInstanceId = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxGameType = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxStatus = new System.Windows.Forms.TextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textBoxURL = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxBaseName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxWorld = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbAlivePlayers = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbOnlinePlayers = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbDeployables = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbVehicleSpawn = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbVehicles = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBoxMapHelper = new System.Windows.Forms.CheckBox();
            this.checkBoxShowTrail = new System.Windows.Forms.CheckBox();
            this.radioButtonDeployables = new System.Windows.Forms.RadioButton();
            this.radioButtonSpawn = new System.Windows.Forms.RadioButton();
            this.groupBoxInfo = new System.Windows.Forms.GroupBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.radioButtonVehicles = new System.Windows.Forms.RadioButton();
            this.radioButtonOnline = new System.Windows.Forms.RadioButton();
            this.radioButtonAlive = new System.Windows.Forms.RadioButton();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textBoxOldTentLimit = new System.Windows.Forms.TextBox();
            this.buttonRemoveTents = new System.Windows.Forms.Button();
            this.textBoxCmdStatus = new System.Windows.Forms.TextBox();
            this.textBoxOldBodyLimit = new System.Windows.Forms.TextBox();
            this.textBoxVehicleMax = new System.Windows.Forms.TextBox();
            this.buttonRemoveBodies = new System.Windows.Forms.Button();
            this.buttonSpawnNew = new System.Windows.Forms.Button();
            this.buttonRemoveDestroyed = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dataGridViewMaps = new System.Windows.Forms.DataGridView();
            this.ColumnID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnChoosePath = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.dataGridViewVehicleTypes = new System.Windows.Forms.DataGridView();
            this.ColumnClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.dataGridViewDeployableTypes = new System.Windows.Forms.DataGridView();
            this.bgWorkerDatabase = new System.ComponentModel.BackgroundWorker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStripVehicle = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemDeleteVehicle = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripSpawn = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemDeleteSpawn = new System.Windows.Forms.ToolStripMenuItem();
            this.bgWorkerFast = new System.ComponentModel.BackgroundWorker();
            this.ClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBoxInfo.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMaps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetBindingSource)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVehicleTypes)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDeployableTypes)).BeginInit();
            this.contextMenuStripVehicle.SuspendLayout();
            this.contextMenuStripSpawn.SuspendLayout();
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
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel1_Paint);
            this.splitContainer1.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseClick);
            this.splitContainer1.Panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseMove);
            this.splitContainer1.Panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Panel1_MouseUp);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2MinSize = 220;
            this.splitContainer1.Size = new System.Drawing.Size(915, 522);
            this.splitContainer1.SplitterDistance = 591;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(316, 518);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(308, 492);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Database";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxInstanceId);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.comboBoxGameType);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBoxStatus);
            this.groupBox2.Controls.Add(this.buttonConnect);
            this.groupBox2.Controls.Add(this.textBoxURL);
            this.groupBox2.Controls.Add(this.textBoxPort);
            this.groupBox2.Controls.Add(this.textBoxPassword);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBoxUser);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxBaseName);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(236, 230);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database";
            // 
            // textBoxInstanceId
            // 
            this.textBoxInstanceId.Location = new System.Drawing.Point(174, 173);
            this.textBoxInstanceId.MaxLength = 64;
            this.textBoxInstanceId.Name = "textBoxInstanceId";
            this.textBoxInstanceId.Size = new System.Drawing.Size(56, 20);
            this.textBoxInstanceId.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 176);
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
            this.comboBoxGameType.Location = new System.Drawing.Point(81, 146);
            this.comboBoxGameType.MaxDropDownItems = 4;
            this.comboBoxGameType.Name = "comboBoxGameType";
            this.comboBoxGameType.Size = new System.Drawing.Size(149, 21);
            this.comboBoxGameType.TabIndex = 6;
            this.comboBoxGameType.SelectedValueChanged += new System.EventHandler(this.comboBoxGameType_SelectedValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 146);
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
            // textBoxStatus
            // 
            this.textBoxStatus.Location = new System.Drawing.Point(81, 199);
            this.textBoxStatus.MaxLength = 256;
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.ReadOnly = true;
            this.textBoxStatus.Size = new System.Drawing.Size(149, 20);
            this.textBoxStatus.TabIndex = 107;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(6, 197);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(69, 23);
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
            this.textBoxPort.Location = new System.Drawing.Point(174, 42);
            this.textBoxPort.MaxLength = 6;
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(56, 20);
            this.textBoxPort.TabIndex = 1;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(43, 120);
            this.textBoxPassword.MaxLength = 64;
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(187, 20);
            this.textBoxPassword.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 104;
            this.label4.Text = "Pass";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 101;
            this.label2.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 102;
            this.label3.Text = "Base";
            // 
            // textBoxUser
            // 
            this.textBoxUser.Location = new System.Drawing.Point(43, 94);
            this.textBoxUser.MaxLength = 256;
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.Size = new System.Drawing.Size(187, 20);
            this.textBoxUser.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 103;
            this.label5.Text = "User";
            // 
            // textBoxBaseName
            // 
            this.textBoxBaseName.Location = new System.Drawing.Point(43, 68);
            this.textBoxBaseName.MaxLength = 256;
            this.textBoxBaseName.Name = "textBoxBaseName";
            this.textBoxBaseName.Size = new System.Drawing.Size(187, 20);
            this.textBoxBaseName.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.textBoxWorld);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.tbAlivePlayers);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.tbOnlinePlayers);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tbDeployables);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbVehicleSpawn);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tbVehicles);
            this.groupBox1.Location = new System.Drawing.Point(6, 242);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(236, 182);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Info";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 13);
            this.label12.TabIndex = 13;
            this.label12.Text = "World";
            // 
            // textBoxWorld
            // 
            this.textBoxWorld.Location = new System.Drawing.Point(126, 23);
            this.textBoxWorld.Name = "textBoxWorld";
            this.textBoxWorld.ReadOnly = true;
            this.textBoxWorld.Size = new System.Drawing.Size(104, 20);
            this.textBoxWorld.TabIndex = 12;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 52);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(72, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Players online";
            // 
            // tbAlivePlayers
            // 
            this.tbAlivePlayers.Location = new System.Drawing.Point(126, 75);
            this.tbAlivePlayers.Name = "tbAlivePlayers";
            this.tbAlivePlayers.ReadOnly = true;
            this.tbAlivePlayers.Size = new System.Drawing.Size(104, 20);
            this.tbAlivePlayers.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Players alive";
            // 
            // tbOnlinePlayers
            // 
            this.tbOnlinePlayers.Location = new System.Drawing.Point(126, 49);
            this.tbOnlinePlayers.Name = "tbOnlinePlayers";
            this.tbOnlinePlayers.ReadOnly = true;
            this.tbOnlinePlayers.Size = new System.Drawing.Size(104, 20);
            this.tbOnlinePlayers.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 156);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Tents - Staches";
            // 
            // tbDeployables
            // 
            this.tbDeployables.Location = new System.Drawing.Point(126, 153);
            this.tbDeployables.Name = "tbDeployables";
            this.tbDeployables.ReadOnly = true;
            this.tbDeployables.Size = new System.Drawing.Size(104, 20);
            this.tbDeployables.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Spawn Points";
            // 
            // tbVehicleSpawn
            // 
            this.tbVehicleSpawn.Location = new System.Drawing.Point(126, 127);
            this.tbVehicleSpawn.Name = "tbVehicleSpawn";
            this.tbVehicleSpawn.ReadOnly = true;
            this.tbVehicleSpawn.Size = new System.Drawing.Size(104, 20);
            this.tbVehicleSpawn.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 104);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Vehicles";
            // 
            // tbVehicles
            // 
            this.tbVehicles.Location = new System.Drawing.Point(126, 101);
            this.tbVehicles.Name = "tbVehicles";
            this.tbVehicles.ReadOnly = true;
            this.tbVehicles.Size = new System.Drawing.Size(104, 20);
            this.tbVehicles.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.checkBoxMapHelper);
            this.tabPage2.Controls.Add(this.checkBoxShowTrail);
            this.tabPage2.Controls.Add(this.radioButtonDeployables);
            this.tabPage2.Controls.Add(this.radioButtonSpawn);
            this.tabPage2.Controls.Add(this.groupBoxInfo);
            this.tabPage2.Controls.Add(this.radioButtonVehicles);
            this.tabPage2.Controls.Add(this.radioButtonOnline);
            this.tabPage2.Controls.Add(this.radioButtonAlive);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(308, 492);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Display";
            // 
            // checkBoxMapHelper
            // 
            this.checkBoxMapHelper.AutoSize = true;
            this.checkBoxMapHelper.Location = new System.Drawing.Point(114, 80);
            this.checkBoxMapHelper.Name = "checkBoxMapHelper";
            this.checkBoxMapHelper.Size = new System.Drawing.Size(81, 17);
            this.checkBoxMapHelper.TabIndex = 6;
            this.checkBoxMapHelper.Text = "Map Helper";
            this.checkBoxMapHelper.UseVisualStyleBackColor = true;
            this.checkBoxMapHelper.CheckedChanged += new System.EventHandler(this.checkBoxMapHelper_CheckedChanged);
            // 
            // checkBoxShowTrail
            // 
            this.checkBoxShowTrail.AutoSize = true;
            this.checkBoxShowTrail.Location = new System.Drawing.Point(6, 80);
            this.checkBoxShowTrail.Name = "checkBoxShowTrail";
            this.checkBoxShowTrail.Size = new System.Drawing.Size(72, 17);
            this.checkBoxShowTrail.TabIndex = 5;
            this.checkBoxShowTrail.Text = "Show trail";
            this.checkBoxShowTrail.UseVisualStyleBackColor = true;
            this.checkBoxShowTrail.CheckedChanged += new System.EventHandler(this.checkBoxShowTrail_CheckedChanged);
            // 
            // radioButtonDeployables
            // 
            this.radioButtonDeployables.AutoSize = true;
            this.radioButtonDeployables.Location = new System.Drawing.Point(6, 57);
            this.radioButtonDeployables.Name = "radioButtonDeployables";
            this.radioButtonDeployables.Size = new System.Drawing.Size(100, 17);
            this.radioButtonDeployables.TabIndex = 4;
            this.radioButtonDeployables.Text = "Tents - Staches";
            this.radioButtonDeployables.UseVisualStyleBackColor = true;
            this.radioButtonDeployables.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonSpawn
            // 
            this.radioButtonSpawn.AutoSize = true;
            this.radioButtonSpawn.Location = new System.Drawing.Point(114, 34);
            this.radioButtonSpawn.Name = "radioButtonSpawn";
            this.radioButtonSpawn.Size = new System.Drawing.Size(90, 17);
            this.radioButtonSpawn.TabIndex = 3;
            this.radioButtonSpawn.Text = "Spawn Points";
            this.radioButtonSpawn.UseVisualStyleBackColor = true;
            this.radioButtonSpawn.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // groupBoxInfo
            // 
            this.groupBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxInfo.Controls.Add(this.propertyGrid1);
            this.groupBoxInfo.Location = new System.Drawing.Point(3, 103);
            this.groupBoxInfo.Name = "groupBoxInfo";
            this.groupBoxInfo.Size = new System.Drawing.Size(241, 386);
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
            this.propertyGrid1.Size = new System.Drawing.Size(235, 367);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Control;
            // 
            // radioButtonVehicles
            // 
            this.radioButtonVehicles.AutoSize = true;
            this.radioButtonVehicles.Location = new System.Drawing.Point(6, 34);
            this.radioButtonVehicles.Name = "radioButtonVehicles";
            this.radioButtonVehicles.Size = new System.Drawing.Size(65, 17);
            this.radioButtonVehicles.TabIndex = 2;
            this.radioButtonVehicles.Text = "Vehicles";
            this.radioButtonVehicles.UseVisualStyleBackColor = true;
            this.radioButtonVehicles.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonOnline
            // 
            this.radioButtonOnline.AutoSize = true;
            this.radioButtonOnline.Checked = true;
            this.radioButtonOnline.Location = new System.Drawing.Point(6, 11);
            this.radioButtonOnline.Name = "radioButtonOnline";
            this.radioButtonOnline.Size = new System.Drawing.Size(55, 17);
            this.radioButtonOnline.TabIndex = 0;
            this.radioButtonOnline.TabStop = true;
            this.radioButtonOnline.Text = "Online";
            this.radioButtonOnline.UseVisualStyleBackColor = true;
            this.radioButtonOnline.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonAlive
            // 
            this.radioButtonAlive.AutoSize = true;
            this.radioButtonAlive.Location = new System.Drawing.Point(114, 11);
            this.radioButtonAlive.Name = "radioButtonAlive";
            this.radioButtonAlive.Size = new System.Drawing.Size(48, 17);
            this.radioButtonAlive.TabIndex = 0;
            this.radioButtonAlive.Text = "Alive";
            this.radioButtonAlive.UseVisualStyleBackColor = true;
            this.radioButtonAlive.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Transparent;
            this.tabPage3.Controls.Add(this.textBoxOldTentLimit);
            this.tabPage3.Controls.Add(this.buttonRemoveTents);
            this.tabPage3.Controls.Add(this.textBoxCmdStatus);
            this.tabPage3.Controls.Add(this.textBoxOldBodyLimit);
            this.tabPage3.Controls.Add(this.textBoxVehicleMax);
            this.tabPage3.Controls.Add(this.buttonRemoveBodies);
            this.tabPage3.Controls.Add(this.buttonSpawnNew);
            this.tabPage3.Controls.Add(this.buttonRemoveDestroyed);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(308, 492);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Scripts";
            // 
            // textBoxOldTentLimit
            // 
            this.textBoxOldTentLimit.Location = new System.Drawing.Point(160, 95);
            this.textBoxOldTentLimit.MaxLength = 4;
            this.textBoxOldTentLimit.Name = "textBoxOldTentLimit";
            this.textBoxOldTentLimit.Size = new System.Drawing.Size(47, 20);
            this.textBoxOldTentLimit.TabIndex = 7;
            this.textBoxOldTentLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.textBoxOldTentLimit, "Day limit");
            // 
            // buttonRemoveTents
            // 
            this.buttonRemoveTents.Location = new System.Drawing.Point(6, 93);
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
            this.textBoxCmdStatus.Location = new System.Drawing.Point(7, 122);
            this.textBoxCmdStatus.MaxLength = 4096;
            this.textBoxCmdStatus.Multiline = true;
            this.textBoxCmdStatus.Name = "textBoxCmdStatus";
            this.textBoxCmdStatus.ReadOnly = true;
            this.textBoxCmdStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxCmdStatus.Size = new System.Drawing.Size(235, 362);
            this.textBoxCmdStatus.TabIndex = 8;
            // 
            // textBoxOldBodyLimit
            // 
            this.textBoxOldBodyLimit.Location = new System.Drawing.Point(160, 66);
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
            this.textBoxVehicleMax.Location = new System.Drawing.Point(160, 37);
            this.textBoxVehicleMax.MaxLength = 4;
            this.textBoxVehicleMax.Name = "textBoxVehicleMax";
            this.textBoxVehicleMax.Size = new System.Drawing.Size(47, 20);
            this.textBoxVehicleMax.TabIndex = 5;
            this.textBoxVehicleMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.textBoxVehicleMax, "Expected vehicle count");
            // 
            // buttonRemoveBodies
            // 
            this.buttonRemoveBodies.Location = new System.Drawing.Point(6, 64);
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
            this.buttonSpawnNew.Location = new System.Drawing.Point(6, 35);
            this.buttonSpawnNew.Name = "buttonSpawnNew";
            this.buttonSpawnNew.Size = new System.Drawing.Size(148, 23);
            this.buttonSpawnNew.TabIndex = 2;
            this.buttonSpawnNew.Text = "Spawn new vehicles";
            this.buttonSpawnNew.UseVisualStyleBackColor = true;
            this.buttonSpawnNew.Click += new System.EventHandler(this.buttonSpawnNew_Click);
            // 
            // buttonRemoveDestroyed
            // 
            this.buttonRemoveDestroyed.Location = new System.Drawing.Point(6, 6);
            this.buttonRemoveDestroyed.Name = "buttonRemoveDestroyed";
            this.buttonRemoveDestroyed.Size = new System.Drawing.Size(148, 23);
            this.buttonRemoveDestroyed.TabIndex = 1;
            this.buttonRemoveDestroyed.Text = "Remove destroyed vehicles";
            this.buttonRemoveDestroyed.UseVisualStyleBackColor = true;
            this.buttonRemoveDestroyed.Click += new System.EventHandler(this.buttonRemoveDestroyed_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.Transparent;
            this.tabPage4.Controls.Add(this.dataGridViewMaps);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(308, 492);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Maps";
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
            this.ColumnID,
            this.ColumnName,
            this.ColumnChoosePath,
            this.ColumnPath});
            this.dataGridViewMaps.DataSource = this.dataSetBindingSource;
            this.dataGridViewMaps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMaps.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewMaps.MultiSelect = false;
            this.dataGridViewMaps.Name = "dataGridViewMaps";
            this.dataGridViewMaps.RowHeadersVisible = false;
            this.dataGridViewMaps.Size = new System.Drawing.Size(302, 486);
            this.dataGridViewMaps.TabIndex = 0;
            this.dataGridViewMaps.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewMaps_CellClick);
            // 
            // ColumnID
            // 
            this.ColumnID.FillWeight = 10F;
            this.ColumnID.HeaderText = "World ID";
            this.ColumnID.Name = "ColumnID";
            this.ColumnID.ReadOnly = true;
            // 
            // ColumnName
            // 
            this.ColumnName.FillWeight = 30F;
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // ColumnChoosePath
            // 
            this.ColumnChoosePath.FillWeight = 8F;
            this.ColumnChoosePath.HeaderText = "Select";
            this.ColumnChoosePath.Name = "ColumnChoosePath";
            this.ColumnChoosePath.Text = "...";
            this.ColumnChoosePath.ToolTipText = "Select your file on disk";
            this.ColumnChoosePath.UseColumnTextForButtonValue = true;
            // 
            // ColumnPath
            // 
            this.ColumnPath.FillWeight = 45F;
            this.ColumnPath.HeaderText = "Path";
            this.ColumnPath.Name = "ColumnPath";
            // 
            // dataSetBindingSource
            // 
            this.dataSetBindingSource.DataSource = typeof(System.Data.DataSet);
            this.dataSetBindingSource.Position = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.Color.Transparent;
            this.tabPage5.Controls.Add(this.dataGridViewVehicleTypes);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(308, 492);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Vehicles";
            // 
            // dataGridViewVehicleTypes
            // 
            this.dataGridViewVehicleTypes.AllowUserToAddRows = false;
            this.dataGridViewVehicleTypes.AllowUserToDeleteRows = false;
            this.dataGridViewVehicleTypes.AllowUserToResizeRows = false;
            this.dataGridViewVehicleTypes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewVehicleTypes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnClassName,
            this.ColumnType});
            this.dataGridViewVehicleTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewVehicleTypes.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewVehicleTypes.MultiSelect = false;
            this.dataGridViewVehicleTypes.Name = "dataGridViewVehicleTypes";
            this.dataGridViewVehicleTypes.RowHeadersVisible = false;
            this.dataGridViewVehicleTypes.ShowEditingIcon = false;
            this.dataGridViewVehicleTypes.Size = new System.Drawing.Size(302, 486);
            this.dataGridViewVehicleTypes.TabIndex = 0;
            // 
            // ColumnClassName
            // 
            this.ColumnClassName.HeaderText = "ClassName";
            this.ColumnClassName.Name = "ColumnClassName";
            this.ColumnClassName.ReadOnly = true;
            // 
            // ColumnType
            // 
            this.ColumnType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ColumnType.HeaderText = "Type";
            this.ColumnType.Items.AddRange(new object[] {
            "Air",
            "Bicycle",
            "Boat",
            "Bus",
            "Car",
            "Helicopter",
            "Motorcycle",
            "Truck"});
            this.ColumnType.Name = "ColumnType";
            // 
            // tabPage6
            // 
            this.tabPage6.BackColor = System.Drawing.Color.Transparent;
            this.tabPage6.Controls.Add(this.dataGridViewDeployableTypes);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(308, 492);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Deployables";
            // 
            // dataGridViewDeployableTypes
            // 
            this.dataGridViewDeployableTypes.AllowUserToAddRows = false;
            this.dataGridViewDeployableTypes.AllowUserToDeleteRows = false;
            this.dataGridViewDeployableTypes.AllowUserToResizeRows = false;
            this.dataGridViewDeployableTypes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewDeployableTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewDeployableTypes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ClassName,
            this.Type});
            this.dataGridViewDeployableTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDeployableTypes.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewDeployableTypes.MultiSelect = false;
            this.dataGridViewDeployableTypes.Name = "dataGridViewDeployableTypes";
            this.dataGridViewDeployableTypes.RowHeadersVisible = false;
            this.dataGridViewDeployableTypes.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewDeployableTypes.ShowEditingIcon = false;
            this.dataGridViewDeployableTypes.Size = new System.Drawing.Size(302, 486);
            this.dataGridViewDeployableTypes.TabIndex = 0;
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
            // bgWorkerFast
            // 
            this.bgWorkerFast.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerFast_DoWork);
            // 
            // ClassName
            // 
            this.ClassName.HeaderText = "ClassName";
            this.ClassName.Name = "ClassName";
            this.ClassName.ReadOnly = true;
            // 
            // Type
            // 
            this.Type.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Type.HeaderText = "Type";
            this.Type.Items.AddRange(new object[] {
            "Unknown",
            "Tent",
            "Stach",
            "Small Build",
            "Large Build"});
            this.Type.Name = "Type";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 522);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(628, 497);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBoxInfo.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMaps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetBindingSource)).EndInit();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVehicleTypes)).EndInit();
            this.tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDeployableTypes)).EndInit();
            this.contextMenuStripVehicle.ResumeLayout(false);
            this.contextMenuStripSpawn.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MySplitContainer splitContainer1;
        private System.Windows.Forms.RadioButton radioButtonDeployables;
        private System.Windows.Forms.RadioButton radioButtonVehicles;
        private System.Windows.Forms.RadioButton radioButtonOnline;
        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxBaseName;
        private System.Windows.Forms.TextBox textBoxURL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioButtonSpawn;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.GroupBox groupBoxInfo;
        private System.Windows.Forms.RadioButton radioButtonAlive;
        private System.ComponentModel.BackgroundWorker bgWorkerDatabase;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbDeployables;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbVehicleSpawn;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbVehicles;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbAlivePlayers;
        private System.Windows.Forms.TextBox textBoxInstanceId;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox textBoxOldBodyLimit;
        private System.Windows.Forms.TextBox textBoxVehicleMax;
        private System.Windows.Forms.Button buttonRemoveBodies;
        private System.Windows.Forms.Button buttonSpawnNew;
        private System.Windows.Forms.Button buttonRemoveDestroyed;
        private System.Windows.Forms.TextBox textBoxCmdStatus;
        private System.Windows.Forms.TextBox textBoxOldTentLimit;
        private System.Windows.Forms.Button buttonRemoveTents;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbOnlinePlayers;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBoxShowTrail;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.DataGridView dataGridViewMaps;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripVehicle;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDeleteVehicle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripSpawn;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDeleteSpawn;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.DataGridView dataGridViewVehicleTypes;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnClassName;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnType;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxWorld;
        private System.Windows.Forms.ComboBox comboBoxGameType;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBoxMapHelper;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnChoosePath;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPath;
        private System.Windows.Forms.BindingSource dataSetBindingSource;
        private System.ComponentModel.BackgroundWorker bgWorkerFast;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.DataGridView dataGridViewDeployableTypes;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClassName;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;

    }
}

