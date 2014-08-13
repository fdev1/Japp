namespace JappUI
{
    partial class MainForm
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuOpenHEX = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
			this.aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCheckDevice = new System.Windows.Forms.ToolStripMenuItem();
			this.programDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuDiagnostics = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabOptions = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.numVddStart = new System.Windows.Forms.NumericUpDown();
			this.chkVaryVdd = new System.Windows.Forms.CheckBox();
			this.chkVerify = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.rbPartialProgramming = new System.Windows.Forms.RadioButton();
			this.rbFullProgramming = new System.Windows.Forms.RadioButton();
			this.txtPMEnd = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtPMStart = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnBrowseHEX = new System.Windows.Forms.Button();
			this.txtHEXFile = new System.Windows.Forms.TextBox();
			this.tabPMem = new System.Windows.Forms.TabPage();
			this.lvPMem = new System.Windows.Forms.ListView();
			this.colAddress = new System.Windows.Forms.ColumnHeader();
			this.colMemData = new System.Windows.Forms.ColumnHeader();
			this.colMnemonic = new System.Windows.Forms.ColumnHeader();
			this.tabConfig = new System.Windows.Forms.TabPage();
			this.dgConfigBits = new System.Windows.Forms.DataGridView();
			this.colBitName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colRegAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colValue = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.tabOutput = new System.Windows.Forms.TabPage();
			this.txtOutput = new System.Windows.Forms.TextBox();
			this.ctxMnuOutput = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnuClearOutput = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.comboCOMPorts = new System.Windows.Forms.ToolStripComboBox();
			this.mnuConnect = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this.comboDeviceFamilies = new System.Windows.Forms.ToolStripComboBox();
			this.comboDevices = new System.Windows.Forms.ToolStripComboBox();
			this.tbCheckDevice = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuClear = new System.Windows.Forms.ToolStripButton();
			this.tbOpenHEX = new System.Windows.Forms.ToolStripButton();
			this.tbErase = new System.Windows.Forms.ToolStripButton();
			this.mnuReadPM = new System.Windows.Forms.ToolStripButton();
			this.tbProgram = new System.Windows.Forms.ToolStripButton();
			this.tbVerify = new System.Windows.Forms.ToolStripButton();
			this.tbReset = new System.Windows.Forms.ToolStripButton();
			this.menuStrip1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabOptions.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numVddStart)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPMem.SuspendLayout();
			this.tabConfig.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgConfigBits)).BeginInit();
			this.tabOutput.SuspendLayout();
			this.ctxMnuOutput.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(714, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpenHEX,
            this.toolStripMenuItem1,
            this.mnuExit});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// mnuOpenHEX
			// 
			this.mnuOpenHEX.Enabled = false;
			this.mnuOpenHEX.Name = "mnuOpenHEX";
			this.mnuOpenHEX.Size = new System.Drawing.Size(137, 22);
			this.mnuOpenHEX.Text = "Open HEX...";
			this.mnuOpenHEX.Click += new System.EventHandler(this.mnuOpenHEX_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(134, 6);
			// 
			// mnuExit
			// 
			this.mnuExit.Name = "mnuExit";
			this.mnuExit.Size = new System.Drawing.Size(137, 22);
			this.mnuExit.Text = "Exit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// aToolStripMenuItem
			// 
			this.aToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCheckDevice,
            this.programDeviceToolStripMenuItem});
			this.aToolStripMenuItem.Name = "aToolStripMenuItem";
			this.aToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.aToolStripMenuItem.Text = "Action";
			// 
			// mnuCheckDevice
			// 
			this.mnuCheckDevice.Name = "mnuCheckDevice";
			this.mnuCheckDevice.Size = new System.Drawing.Size(167, 22);
			this.mnuCheckDevice.Text = "Check Device...";
			this.mnuCheckDevice.Click += new System.EventHandler(this.mnuCheckDevice_Click);
			// 
			// programDeviceToolStripMenuItem
			// 
			this.programDeviceToolStripMenuItem.Name = "programDeviceToolStripMenuItem";
			this.programDeviceToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.programDeviceToolStripMenuItem.Text = "Program Device...";
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDiagnostics});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.toolsToolStripMenuItem.Text = "Tools";
			// 
			// mnuDiagnostics
			// 
			this.mnuDiagnostics.Name = "mnuDiagnostics";
			this.mnuDiagnostics.Size = new System.Drawing.Size(135, 22);
			this.mnuDiagnostics.Text = "Diagnostics";
			this.mnuDiagnostics.Click += new System.EventHandler(this.mnuDiagnostics_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAbout});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// mnuAbout
			// 
			this.mnuAbout.Name = "mnuAbout";
			this.mnuAbout.Size = new System.Drawing.Size(116, 22);
			this.mnuAbout.Text = "About...";
			this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabOptions);
			this.tabControl1.Controls.Add(this.tabPMem);
			this.tabControl1.Controls.Add(this.tabConfig);
			this.tabControl1.Controls.Add(this.tabOutput);
			this.tabControl1.Location = new System.Drawing.Point(9, 48);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(696, 255);
			this.tabControl1.TabIndex = 5;
			// 
			// tabOptions
			// 
			this.tabOptions.Controls.Add(this.groupBox3);
			this.tabOptions.Controls.Add(this.groupBox2);
			this.tabOptions.Controls.Add(this.groupBox1);
			this.tabOptions.Location = new System.Drawing.Point(4, 22);
			this.tabOptions.Margin = new System.Windows.Forms.Padding(2);
			this.tabOptions.Name = "tabOptions";
			this.tabOptions.Padding = new System.Windows.Forms.Padding(2);
			this.tabOptions.Size = new System.Drawing.Size(688, 229);
			this.tabOptions.TabIndex = 0;
			this.tabOptions.Text = "Options";
			this.tabOptions.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.numericUpDown1);
			this.groupBox3.Controls.Add(this.numVddStart);
			this.groupBox3.Controls.Add(this.chkVaryVdd);
			this.groupBox3.Controls.Add(this.chkVerify);
			this.groupBox3.Location = new System.Drawing.Point(4, 133);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox3.Size = new System.Drawing.Size(596, 71);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Verify";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(453, 43);
			this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(58, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "High VDD:";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(456, 19);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Low VDD:";
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.DecimalPlaces = 2;
			this.numericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numericUpDown1.Location = new System.Drawing.Point(520, 39);
			this.numericUpDown1.Margin = new System.Windows.Forms.Padding(2);
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(68, 20);
			this.numericUpDown1.TabIndex = 3;
			this.numericUpDown1.Value = new decimal(new int[] {
            33,
            0,
            0,
            65536});
			// 
			// numVddStart
			// 
			this.numVddStart.DecimalPlaces = 2;
			this.numVddStart.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numVddStart.Location = new System.Drawing.Point(520, 17);
			this.numVddStart.Margin = new System.Windows.Forms.Padding(2);
			this.numVddStart.Name = "numVddStart";
			this.numVddStart.Size = new System.Drawing.Size(68, 20);
			this.numVddStart.TabIndex = 2;
			this.numVddStart.Value = new decimal(new int[] {
            33,
            0,
            0,
            65536});
			// 
			// chkVaryVdd
			// 
			this.chkVaryVdd.AutoSize = true;
			this.chkVaryVdd.Location = new System.Drawing.Point(14, 39);
			this.chkVaryVdd.Margin = new System.Windows.Forms.Padding(2);
			this.chkVaryVdd.Name = "chkVaryVdd";
			this.chkVaryVdd.Size = new System.Drawing.Size(73, 17);
			this.chkVaryVdd.TabIndex = 1;
			this.chkVaryVdd.Text = "Vary VDD";
			this.chkVaryVdd.UseVisualStyleBackColor = true;
			// 
			// chkVerify
			// 
			this.chkVerify.AutoSize = true;
			this.chkVerify.Location = new System.Drawing.Point(14, 17);
			this.chkVerify.Margin = new System.Windows.Forms.Padding(2);
			this.chkVerify.Name = "chkVerify";
			this.chkVerify.Size = new System.Drawing.Size(76, 17);
			this.chkVerify.TabIndex = 0;
			this.chkVerify.Text = "Auto-verify";
			this.chkVerify.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.rbPartialProgramming);
			this.groupBox2.Controls.Add(this.rbFullProgramming);
			this.groupBox2.Controls.Add(this.txtPMEnd);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.txtPMStart);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(4, 58);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox2.Size = new System.Drawing.Size(682, 71);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Program Memory";
			// 
			// rbPartialProgramming
			// 
			this.rbPartialProgramming.AutoSize = true;
			this.rbPartialProgramming.Location = new System.Drawing.Point(14, 42);
			this.rbPartialProgramming.Margin = new System.Windows.Forms.Padding(2);
			this.rbPartialProgramming.Name = "rbPartialProgramming";
			this.rbPartialProgramming.Size = new System.Drawing.Size(224, 17);
			this.rbPartialProgramming.TabIndex = 5;
			this.rbPartialProgramming.TabStop = true;
			this.rbPartialProgramming.Text = "Program/read the selected address range.";
			this.rbPartialProgramming.UseVisualStyleBackColor = true;
			// 
			// rbFullProgramming
			// 
			this.rbFullProgramming.AutoSize = true;
			this.rbFullProgramming.Checked = true;
			this.rbFullProgramming.Location = new System.Drawing.Point(14, 20);
			this.rbFullProgramming.Margin = new System.Windows.Forms.Padding(2);
			this.rbFullProgramming.Name = "rbFullProgramming";
			this.rbFullProgramming.Size = new System.Drawing.Size(214, 17);
			this.rbFullProgramming.TabIndex = 4;
			this.rbFullProgramming.TabStop = true;
			this.rbFullProgramming.Text = "Program/read the entire device memory.";
			this.rbFullProgramming.UseVisualStyleBackColor = true;
			this.rbFullProgramming.CheckedChanged += new System.EventHandler(this.rbFullProgramming_CheckedChanged);
			// 
			// txtPMEnd
			// 
			this.txtPMEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtPMEnd.Location = new System.Drawing.Point(599, 41);
			this.txtPMEnd.Margin = new System.Windows.Forms.Padding(2);
			this.txtPMEnd.Name = "txtPMEnd";
			this.txtPMEnd.ReadOnly = true;
			this.txtPMEnd.Size = new System.Drawing.Size(76, 20);
			this.txtPMEnd.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(521, 44);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "End Address:";
			// 
			// txtPMStart
			// 
			this.txtPMStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtPMStart.Location = new System.Drawing.Point(599, 19);
			this.txtPMStart.Margin = new System.Windows.Forms.Padding(2);
			this.txtPMStart.Name = "txtPMStart";
			this.txtPMStart.ReadOnly = true;
			this.txtPMStart.Size = new System.Drawing.Size(76, 20);
			this.txtPMStart.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(521, 21);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Start Address:";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnBrowseHEX);
			this.groupBox1.Controls.Add(this.txtHEXFile);
			this.groupBox1.Location = new System.Drawing.Point(4, 5);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox1.Size = new System.Drawing.Size(682, 48);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "HEX File";
			// 
			// btnBrowseHEX
			// 
			this.btnBrowseHEX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseHEX.Enabled = false;
			this.btnBrowseHEX.Location = new System.Drawing.Point(618, 17);
			this.btnBrowseHEX.Margin = new System.Windows.Forms.Padding(2);
			this.btnBrowseHEX.Name = "btnBrowseHEX";
			this.btnBrowseHEX.Size = new System.Drawing.Size(56, 20);
			this.btnBrowseHEX.TabIndex = 1;
			this.btnBrowseHEX.Text = "Browse...";
			this.btnBrowseHEX.UseVisualStyleBackColor = true;
			this.btnBrowseHEX.Click += new System.EventHandler(this.btnBrowseHEX_Click);
			// 
			// txtHEXFile
			// 
			this.txtHEXFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtHEXFile.Location = new System.Drawing.Point(4, 17);
			this.txtHEXFile.Margin = new System.Windows.Forms.Padding(2);
			this.txtHEXFile.Name = "txtHEXFile";
			this.txtHEXFile.ReadOnly = true;
			this.txtHEXFile.Size = new System.Drawing.Size(610, 20);
			this.txtHEXFile.TabIndex = 0;
			// 
			// tabPMem
			// 
			this.tabPMem.Controls.Add(this.lvPMem);
			this.tabPMem.Location = new System.Drawing.Point(4, 22);
			this.tabPMem.Margin = new System.Windows.Forms.Padding(2);
			this.tabPMem.Name = "tabPMem";
			this.tabPMem.Size = new System.Drawing.Size(688, 229);
			this.tabPMem.TabIndex = 2;
			this.tabPMem.Text = "Program Memory";
			this.tabPMem.UseVisualStyleBackColor = true;
			// 
			// lvPMem
			// 
			this.lvPMem.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAddress,
            this.colMemData,
            this.colMnemonic});
			this.lvPMem.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvPMem.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lvPMem.FullRowSelect = true;
			this.lvPMem.Location = new System.Drawing.Point(0, 0);
			this.lvPMem.Margin = new System.Windows.Forms.Padding(2);
			this.lvPMem.Name = "lvPMem";
			this.lvPMem.Size = new System.Drawing.Size(688, 229);
			this.lvPMem.TabIndex = 0;
			this.lvPMem.UseCompatibleStateImageBehavior = false;
			this.lvPMem.View = System.Windows.Forms.View.Details;
			this.lvPMem.SelectedIndexChanged += new System.EventHandler(this.lvPMem_SelectedIndexChanged);
			// 
			// colAddress
			// 
			this.colAddress.Text = "Offset";
			this.colAddress.Width = 160;
			// 
			// colMemData
			// 
			this.colMemData.Text = "Program Data";
			this.colMemData.Width = 200;
			// 
			// colMnemonic
			// 
			this.colMnemonic.Text = "Mnemonic";
			this.colMnemonic.Width = 250;
			// 
			// tabConfig
			// 
			this.tabConfig.Controls.Add(this.dgConfigBits);
			this.tabConfig.Location = new System.Drawing.Point(4, 22);
			this.tabConfig.Margin = new System.Windows.Forms.Padding(2);
			this.tabConfig.Name = "tabConfig";
			this.tabConfig.Size = new System.Drawing.Size(688, 229);
			this.tabConfig.TabIndex = 3;
			this.tabConfig.Text = "Configuration Bits";
			this.tabConfig.UseVisualStyleBackColor = true;
			// 
			// dgConfigBits
			// 
			this.dgConfigBits.AllowUserToAddRows = false;
			this.dgConfigBits.AllowUserToDeleteRows = false;
			this.dgConfigBits.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgConfigBits.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBitName,
            this.colRegAddress,
            this.colDesc,
            this.colValue});
			this.dgConfigBits.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgConfigBits.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.dgConfigBits.Location = new System.Drawing.Point(0, 0);
			this.dgConfigBits.Margin = new System.Windows.Forms.Padding(2);
			this.dgConfigBits.Name = "dgConfigBits";
			this.dgConfigBits.ReadOnly = true;
			this.dgConfigBits.RowTemplate.Height = 24;
			this.dgConfigBits.Size = new System.Drawing.Size(688, 229);
			this.dgConfigBits.TabIndex = 0;
			// 
			// colBitName
			// 
			dataGridViewCellStyle1.NullValue = null;
			this.colBitName.DefaultCellStyle = dataGridViewCellStyle1;
			this.colBitName.HeaderText = "Name";
			this.colBitName.Name = "colBitName";
			this.colBitName.ReadOnly = true;
			// 
			// colRegAddress
			// 
			dataGridViewCellStyle2.Format = "X6";
			dataGridViewCellStyle2.NullValue = "0";
			this.colRegAddress.DefaultCellStyle = dataGridViewCellStyle2;
			this.colRegAddress.HeaderText = "Address";
			this.colRegAddress.Name = "colRegAddress";
			this.colRegAddress.ReadOnly = true;
			// 
			// colDesc
			// 
			this.colDesc.HeaderText = "Description";
			this.colDesc.Name = "colDesc";
			this.colDesc.ReadOnly = true;
			this.colDesc.Width = 250;
			// 
			// colValue
			// 
			this.colValue.HeaderText = "Value";
			this.colValue.Name = "colValue";
			this.colValue.ReadOnly = true;
			this.colValue.Width = 200;
			// 
			// tabOutput
			// 
			this.tabOutput.Controls.Add(this.txtOutput);
			this.tabOutput.Location = new System.Drawing.Point(4, 22);
			this.tabOutput.Margin = new System.Windows.Forms.Padding(2);
			this.tabOutput.Name = "tabOutput";
			this.tabOutput.Padding = new System.Windows.Forms.Padding(2);
			this.tabOutput.Size = new System.Drawing.Size(688, 229);
			this.tabOutput.TabIndex = 1;
			this.tabOutput.Text = "Output";
			this.tabOutput.UseVisualStyleBackColor = true;
			// 
			// txtOutput
			// 
			this.txtOutput.ContextMenuStrip = this.ctxMnuOutput;
			this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtOutput.Location = new System.Drawing.Point(2, 2);
			this.txtOutput.Margin = new System.Windows.Forms.Padding(2);
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ReadOnly = true;
			this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtOutput.Size = new System.Drawing.Size(684, 225);
			this.txtOutput.TabIndex = 7;
			this.txtOutput.TextChanged += new System.EventHandler(this.txtOutput_TextChanged);
			// 
			// ctxMnuOutput
			// 
			this.ctxMnuOutput.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuClearOutput});
			this.ctxMnuOutput.Name = "ctxMnuOutput";
			this.ctxMnuOutput.Size = new System.Drawing.Size(102, 26);
			// 
			// mnuClearOutput
			// 
			this.mnuClearOutput.Name = "mnuClearOutput";
			this.mnuClearOutput.Size = new System.Drawing.Size(101, 22);
			this.mnuClearOutput.Text = "Clear";
			this.mnuClearOutput.Click += new System.EventHandler(this.mnuClearOutput_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.comboCOMPorts,
            this.mnuConnect,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.comboDeviceFamilies,
            this.comboDevices,
            this.tbCheckDevice,
            this.toolStripSeparator2,
            this.mnuClear,
            this.tbOpenHEX,
            this.tbErase,
            this.mnuReadPM,
            this.tbProgram,
            this.tbVerify,
            this.tbReset});
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(714, 25);
			this.toolStrip1.TabIndex = 7;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(38, 22);
			this.toolStripLabel1.Text = "COM:";
			// 
			// comboCOMPorts
			// 
			this.comboCOMPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCOMPorts.Name = "comboCOMPorts";
			this.comboCOMPorts.Size = new System.Drawing.Size(114, 25);
			this.comboCOMPorts.DropDown += new System.EventHandler(this.comboCOMPorts_DropDown);
			// 
			// mnuConnect
			// 
			this.mnuConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.mnuConnect.Image = ((System.Drawing.Image)(resources.GetObject("mnuConnect.Image")));
			this.mnuConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mnuConnect.Name = "mnuConnect";
			this.mnuConnect.Size = new System.Drawing.Size(23, 22);
			this.mnuConnect.Text = "Connect";
			this.mnuConnect.Click += new System.EventHandler(this.mnuConnect_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripLabel2
			// 
			this.toolStripLabel2.Name = "toolStripLabel2";
			this.toolStripLabel2.Size = new System.Drawing.Size(45, 22);
			this.toolStripLabel2.Text = "Device:";
			// 
			// comboDeviceFamilies
			// 
			this.comboDeviceFamilies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDeviceFamilies.DropDownWidth = 250;
			this.comboDeviceFamilies.Name = "comboDeviceFamilies";
			this.comboDeviceFamilies.Size = new System.Drawing.Size(92, 25);
			this.comboDeviceFamilies.DropDown += new System.EventHandler(this.comboDeviceFamilies_DropDown);
			// 
			// comboDevices
			// 
			this.comboDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDevices.DropDownWidth = 250;
			this.comboDevices.Name = "comboDevices";
			this.comboDevices.Size = new System.Drawing.Size(121, 25);
			this.comboDevices.SelectedIndexChanged += new System.EventHandler(this.comboDevices_SelectedIndexChanged);
			this.comboDevices.DropDown += new System.EventHandler(this.comboDevices_DropDown);
			// 
			// tbCheckDevice
			// 
			this.tbCheckDevice.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbCheckDevice.Enabled = false;
			this.tbCheckDevice.Image = ((System.Drawing.Image)(resources.GetObject("tbCheckDevice.Image")));
			this.tbCheckDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tbCheckDevice.Name = "tbCheckDevice";
			this.tbCheckDevice.Size = new System.Drawing.Size(23, 22);
			this.tbCheckDevice.Text = "Check Device ID";
			this.tbCheckDevice.ToolTipText = "Check Device...";
			this.tbCheckDevice.Click += new System.EventHandler(this.tbCheckDevice_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// mnuClear
			// 
			this.mnuClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.mnuClear.Image = ((System.Drawing.Image)(resources.GetObject("mnuClear.Image")));
			this.mnuClear.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mnuClear.Name = "mnuClear";
			this.mnuClear.Size = new System.Drawing.Size(23, 22);
			this.mnuClear.Text = "Clear";
			this.mnuClear.Click += new System.EventHandler(this.mnuClear_Click);
			// 
			// tbOpenHEX
			// 
			this.tbOpenHEX.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbOpenHEX.Enabled = false;
			this.tbOpenHEX.Image = ((System.Drawing.Image)(resources.GetObject("tbOpenHEX.Image")));
			this.tbOpenHEX.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tbOpenHEX.Name = "tbOpenHEX";
			this.tbOpenHEX.Size = new System.Drawing.Size(23, 22);
			this.tbOpenHEX.Text = "Open HEX File";
			this.tbOpenHEX.ToolTipText = "Open HEX File...";
			this.tbOpenHEX.Click += new System.EventHandler(this.tbOpenHEX_Click);
			// 
			// tbErase
			// 
			this.tbErase.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbErase.Enabled = false;
			this.tbErase.Image = ((System.Drawing.Image)(resources.GetObject("tbErase.Image")));
			this.tbErase.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tbErase.Name = "tbErase";
			this.tbErase.Size = new System.Drawing.Size(23, 22);
			this.tbErase.Text = "Erase Part";
			this.tbErase.Click += new System.EventHandler(this.tbErase_Click);
			// 
			// mnuReadPM
			// 
			this.mnuReadPM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.mnuReadPM.Enabled = false;
			this.mnuReadPM.Image = ((System.Drawing.Image)(resources.GetObject("mnuReadPM.Image")));
			this.mnuReadPM.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.mnuReadPM.Name = "mnuReadPM";
			this.mnuReadPM.Size = new System.Drawing.Size(23, 22);
			this.mnuReadPM.Text = "Read Program Memory";
			this.mnuReadPM.Click += new System.EventHandler(this.mnuReadPM_Click);
			// 
			// tbProgram
			// 
			this.tbProgram.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbProgram.Enabled = false;
			this.tbProgram.Image = ((System.Drawing.Image)(resources.GetObject("tbProgram.Image")));
			this.tbProgram.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tbProgram.Name = "tbProgram";
			this.tbProgram.Size = new System.Drawing.Size(23, 22);
			this.tbProgram.Text = "Program";
			this.tbProgram.ToolTipText = "Program Device";
			this.tbProgram.Click += new System.EventHandler(this.tbProgram_Click);
			// 
			// tbVerify
			// 
			this.tbVerify.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbVerify.Enabled = false;
			this.tbVerify.Image = ((System.Drawing.Image)(resources.GetObject("tbVerify.Image")));
			this.tbVerify.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tbVerify.Name = "tbVerify";
			this.tbVerify.Size = new System.Drawing.Size(23, 22);
			this.tbVerify.Text = "Verify";
			this.tbVerify.ToolTipText = "Verify...";
			this.tbVerify.Click += new System.EventHandler(this.tbVerify_Click);
			// 
			// tbReset
			// 
			this.tbReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tbReset.Enabled = false;
			this.tbReset.Image = ((System.Drawing.Image)(resources.GetObject("tbReset.Image")));
			this.tbReset.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tbReset.Name = "tbReset";
			this.tbReset.Size = new System.Drawing.Size(23, 22);
			this.tbReset.Text = "Reset";
			this.tbReset.Click += new System.EventHandler(this.tbReset_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(714, 313);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MinimumSize = new System.Drawing.Size(428, 320);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "JAPP - PIC Programming Utility";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabOptions.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numVddStart)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPMem.ResumeLayout(false);
			this.tabConfig.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgConfigBits)).EndInit();
			this.tabOutput.ResumeLayout(false);
			this.tabOutput.PerformLayout();
			this.ctxMnuOutput.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuOpenHEX;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuDiagnostics;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.TabPage tabOutput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.ToolStripMenuItem aToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuCheckDevice;
        private System.Windows.Forms.ToolStripMenuItem programDeviceToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnBrowseHEX;
		private System.Windows.Forms.TextBox txtHEXFile;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tbCheckDevice;
        private System.Windows.Forms.ToolStripButton tbOpenHEX;
        private System.Windows.Forms.ToolStripButton tbProgram;
        private System.Windows.Forms.ToolStripButton tbVerify;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox comboCOMPorts;
        private System.Windows.Forms.ToolStripButton mnuConnect;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox comboDeviceFamilies;
        private System.Windows.Forms.ToolStripButton mnuClear;
        private System.Windows.Forms.ToolStripComboBox comboDevices;
        private System.Windows.Forms.ContextMenuStrip ctxMnuOutput;
        private System.Windows.Forms.ToolStripMenuItem mnuClearOutput;
        private System.Windows.Forms.TabPage tabPMem;
        private System.Windows.Forms.ListView lvPMem;
        private System.Windows.Forms.ColumnHeader colAddress;
        private System.Windows.Forms.ColumnHeader colMemData;
        private System.Windows.Forms.ColumnHeader colMnemonic;
        private System.Windows.Forms.ToolStripButton mnuReadPM;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtPMEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPMStart;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolStripButton tbErase;
		private System.Windows.Forms.ToolStripButton tbReset;
		private System.Windows.Forms.TabPage tabConfig;
		private System.Windows.Forms.DataGridView dgConfigBits;
		private System.Windows.Forms.RadioButton rbPartialProgramming;
		private System.Windows.Forms.RadioButton rbFullProgramming;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.NumericUpDown numVddStart;
		private System.Windows.Forms.CheckBox chkVaryVdd;
		private System.Windows.Forms.CheckBox chkVerify;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.DataGridViewTextBoxColumn colBitName;
		private System.Windows.Forms.DataGridViewTextBoxColumn colRegAddress;
		private System.Windows.Forms.DataGridViewTextBoxColumn colDesc;
		private System.Windows.Forms.DataGridViewComboBoxColumn colValue;
    }
}