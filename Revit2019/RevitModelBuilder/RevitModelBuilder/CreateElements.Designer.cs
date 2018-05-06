namespace RevitModelBuilder {
    partial class CreateElements {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonProcess = new System.Windows.Forms.Button();
            this.textBoxPathFolder = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxCsvFiles = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxMode = new System.Windows.Forms.GroupBox();
            this.radioButtonModeDelete = new System.Windows.Forms.RadioButton();
            this.radioButtonModeKeep = new System.Windows.Forms.RadioButton();
            this.radioButtonModeAdd = new System.Windows.Forms.RadioButton();
            this.checkBoxSuppressMessages = new System.Windows.Forms.CheckBox();
            this.textBoxMaxErrors = new System.Windows.Forms.TextBox();
            this.checkBoxListErrors = new System.Windows.Forms.CheckBox();
            this.comboBoxColumnArchType = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.comboBoxMullionFamily = new System.Windows.Forms.ComboBox();
            this.comboBoxMullionType = new System.Windows.Forms.ComboBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.comboBoxFamilyFamily = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.comboBoxFamilyType = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.comboBoxBeamFamily = new System.Windows.Forms.ComboBox();
            this.comboBoxBeamType = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBoxAdaptiveCompFamily = new System.Windows.Forms.ComboBox();
            this.comboBoxAdaptiveCompType = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBoxColumnMode = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxColumnStructFamily = new System.Windows.Forms.ComboBox();
            this.comboBoxColumnStructType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxColumnArchFamily = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxAutoClose = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.textBoxOriginX = new System.Windows.Forms.TextBox();
            this.textBoxOriginY = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.textBoxOriginZ = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonBrowseTempFolder = new System.Windows.Forms.Button();
            this.textBoxTempFolder = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.buttonBrowseTemplate = new System.Windows.Forms.Button();
            this.textBoxPathTemplate = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBoxUnits = new System.Windows.Forms.GroupBox();
            this.textBoxUnitsFactor = new System.Windows.Forms.TextBox();
            this.radioButtonUnitsFactor = new System.Windows.Forms.RadioButton();
            this.radioButtonUnitsProject = new System.Windows.Forms.RadioButton();
            this.label27 = new System.Windows.Forms.Label();
            this.groupBoxMode.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxUnits.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(614, 698);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(124, 28);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonProcess
            // 
            this.buttonProcess.Location = new System.Drawing.Point(430, 698);
            this.buttonProcess.Name = "buttonProcess";
            this.buttonProcess.Size = new System.Drawing.Size(124, 28);
            this.buttonProcess.TabIndex = 1;
            this.buttonProcess.Text = "Process";
            this.buttonProcess.UseVisualStyleBackColor = true;
            this.buttonProcess.Click += new System.EventHandler(this.buttonProcess_Click);
            // 
            // textBoxPathFolder
            // 
            this.textBoxPathFolder.Location = new System.Drawing.Point(29, 34);
            this.textBoxPathFolder.Name = "textBoxPathFolder";
            this.textBoxPathFolder.Size = new System.Drawing.Size(676, 20);
            this.textBoxPathFolder.TabIndex = 2;
            this.textBoxPathFolder.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPathFolder_KeyPress);
            this.textBoxPathFolder.Leave += new System.EventHandler(this.textBoxPathFolder_Leave);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowse.Location = new System.Drawing.Point(711, 29);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(27, 28);
            this.buttonBrowse.TabIndex = 3;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Full Path to Folder with CSV Files:";
            // 
            // listBoxCsvFiles
            // 
            this.listBoxCsvFiles.FormattingEnabled = true;
            this.listBoxCsvFiles.Location = new System.Drawing.Point(31, 81);
            this.listBoxCsvFiles.Name = "listBoxCsvFiles";
            this.listBoxCsvFiles.Size = new System.Drawing.Size(202, 186);
            this.listBoxCsvFiles.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Select File to Process:";
            // 
            // groupBoxMode
            // 
            this.groupBoxMode.Controls.Add(this.radioButtonModeDelete);
            this.groupBoxMode.Controls.Add(this.radioButtonModeKeep);
            this.groupBoxMode.Controls.Add(this.radioButtonModeAdd);
            this.groupBoxMode.Location = new System.Drawing.Point(259, 67);
            this.groupBoxMode.Name = "groupBoxMode";
            this.groupBoxMode.Size = new System.Drawing.Size(479, 114);
            this.groupBoxMode.TabIndex = 7;
            this.groupBoxMode.TabStop = false;
            this.groupBoxMode.Text = "Mode";
            // 
            // radioButtonModeDelete
            // 
            this.radioButtonModeDelete.AutoSize = true;
            this.radioButtonModeDelete.Location = new System.Drawing.Point(16, 82);
            this.radioButtonModeDelete.Name = "radioButtonModeDelete";
            this.radioButtonModeDelete.Size = new System.Drawing.Size(389, 17);
            this.radioButtonModeDelete.TabIndex = 2;
            this.radioButtonModeDelete.TabStop = true;
            this.radioButtonModeDelete.Text = "Delete elements with matching ElementID values and make all new elements.";
            this.radioButtonModeDelete.UseVisualStyleBackColor = true;
            // 
            // radioButtonModeKeep
            // 
            this.radioButtonModeKeep.AutoSize = true;
            this.radioButtonModeKeep.Location = new System.Drawing.Point(16, 45);
            this.radioButtonModeKeep.Name = "radioButtonModeKeep";
            this.radioButtonModeKeep.Size = new System.Drawing.Size(359, 30);
            this.radioButtonModeKeep.TabIndex = 1;
            this.radioButtonModeKeep.TabStop = true;
            this.radioButtonModeKeep.Text = "Keep elements with matching ElementID values; or make new element.\r\n(Does not upd" +
    "ate elements that have changed.)";
            this.radioButtonModeKeep.UseVisualStyleBackColor = true;
            // 
            // radioButtonModeAdd
            // 
            this.radioButtonModeAdd.AutoSize = true;
            this.radioButtonModeAdd.Location = new System.Drawing.Point(16, 22);
            this.radioButtonModeAdd.Name = "radioButtonModeAdd";
            this.radioButtonModeAdd.Size = new System.Drawing.Size(304, 17);
            this.radioButtonModeAdd.TabIndex = 0;
            this.radioButtonModeAdd.TabStop = true;
            this.radioButtonModeAdd.Text = "Add all new elements.  Do not delete any existing elements.";
            this.radioButtonModeAdd.UseVisualStyleBackColor = true;
            // 
            // checkBoxSuppressMessages
            // 
            this.checkBoxSuppressMessages.AutoSize = true;
            this.checkBoxSuppressMessages.Location = new System.Drawing.Point(29, 698);
            this.checkBoxSuppressMessages.Name = "checkBoxSuppressMessages";
            this.checkBoxSuppressMessages.Size = new System.Drawing.Size(149, 17);
            this.checkBoxSuppressMessages.TabIndex = 8;
            this.checkBoxSuppressMessages.Text = "Suppress Revit Messages";
            this.checkBoxSuppressMessages.UseVisualStyleBackColor = true;
            // 
            // textBoxMaxErrors
            // 
            this.textBoxMaxErrors.Location = new System.Drawing.Point(136, 720);
            this.textBoxMaxErrors.Name = "textBoxMaxErrors";
            this.textBoxMaxErrors.Size = new System.Drawing.Size(38, 20);
            this.textBoxMaxErrors.TabIndex = 9;
            // 
            // checkBoxListErrors
            // 
            this.checkBoxListErrors.AutoSize = true;
            this.checkBoxListErrors.Location = new System.Drawing.Point(29, 721);
            this.checkBoxListErrors.Name = "checkBoxListErrors";
            this.checkBoxListErrors.Size = new System.Drawing.Size(107, 17);
            this.checkBoxListErrors.TabIndex = 10;
            this.checkBoxListErrors.Text = "List Errors    Max:";
            this.checkBoxListErrors.UseVisualStyleBackColor = true;
            // 
            // comboBoxColumnArchType
            // 
            this.comboBoxColumnArchType.FormattingEnabled = true;
            this.comboBoxColumnArchType.Location = new System.Drawing.Point(463, 83);
            this.comboBoxColumnArchType.Name = "comboBoxColumnArchType";
            this.comboBoxColumnArchType.Size = new System.Drawing.Size(227, 21);
            this.comboBoxColumnArchType.TabIndex = 11;
            this.comboBoxColumnArchType.SelectedIndexChanged += new System.EventHandler(this.comboBoxColumnArchType_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.comboBoxMullionFamily);
            this.groupBox1.Controls.Add(this.comboBoxMullionType);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.comboBoxFamilyFamily);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.comboBoxFamilyType);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.comboBoxBeamFamily);
            this.groupBox1.Controls.Add(this.comboBoxBeamType);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.comboBoxAdaptiveCompFamily);
            this.groupBox1.Controls.Add(this.comboBoxAdaptiveCompType);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.comboBoxColumnMode);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.comboBoxColumnStructFamily);
            this.groupBox1.Controls.Add(this.comboBoxColumnStructType);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxColumnArchFamily);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBoxColumnArchType);
            this.groupBox1.Location = new System.Drawing.Point(29, 280);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(709, 259);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Defaults";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(425, 224);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(34, 13);
            this.label24.TabIndex = 44;
            this.label24.Text = "Type:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(140, 224);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(39, 13);
            this.label25.TabIndex = 43;
            this.label25.Text = "Family:";
            // 
            // comboBoxMullionFamily
            // 
            this.comboBoxMullionFamily.FormattingEnabled = true;
            this.comboBoxMullionFamily.Location = new System.Drawing.Point(185, 221);
            this.comboBoxMullionFamily.Name = "comboBoxMullionFamily";
            this.comboBoxMullionFamily.Size = new System.Drawing.Size(227, 21);
            this.comboBoxMullionFamily.TabIndex = 42;
            this.comboBoxMullionFamily.SelectedIndexChanged += new System.EventHandler(this.comboBoxMullionFamily_SelectedIndexChanged);
            // 
            // comboBoxMullionType
            // 
            this.comboBoxMullionType.FormattingEnabled = true;
            this.comboBoxMullionType.Location = new System.Drawing.Point(463, 221);
            this.comboBoxMullionType.Name = "comboBoxMullionType";
            this.comboBoxMullionType.Size = new System.Drawing.Size(227, 21);
            this.comboBoxMullionType.TabIndex = 41;
            this.comboBoxMullionType.SelectedIndexChanged += new System.EventHandler(this.comboBoxMullionType_SelectedIndexChanged);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(18, 224);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(79, 13);
            this.label26.TabIndex = 40;
            this.label26.Text = "Curtain Mullion:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(426, 24);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(34, 13);
            this.label20.TabIndex = 39;
            this.label20.Text = "Type:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(141, 24);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(39, 13);
            this.label21.TabIndex = 38;
            this.label21.Text = "Family:";
            // 
            // comboBoxFamilyFamily
            // 
            this.comboBoxFamilyFamily.FormattingEnabled = true;
            this.comboBoxFamilyFamily.Location = new System.Drawing.Point(186, 21);
            this.comboBoxFamilyFamily.Name = "comboBoxFamilyFamily";
            this.comboBoxFamilyFamily.Size = new System.Drawing.Size(227, 21);
            this.comboBoxFamilyFamily.TabIndex = 37;
            this.comboBoxFamilyFamily.SelectedIndexChanged += new System.EventHandler(this.comboBoxFamilyFamily_SelectedIndexChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(18, 24);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(83, 13);
            this.label22.TabIndex = 36;
            this.label22.Text = "Family Instance:";
            // 
            // comboBoxFamilyType
            // 
            this.comboBoxFamilyType.FormattingEnabled = true;
            this.comboBoxFamilyType.Location = new System.Drawing.Point(464, 21);
            this.comboBoxFamilyType.Name = "comboBoxFamilyType";
            this.comboBoxFamilyType.Size = new System.Drawing.Size(227, 21);
            this.comboBoxFamilyType.TabIndex = 35;
            this.comboBoxFamilyType.SelectedIndexChanged += new System.EventHandler(this.comboBoxFamilyType_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(425, 150);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(34, 13);
            this.label13.TabIndex = 34;
            this.label13.Text = "Type:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(140, 150);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(39, 13);
            this.label14.TabIndex = 33;
            this.label14.Text = "Family:";
            // 
            // comboBoxBeamFamily
            // 
            this.comboBoxBeamFamily.FormattingEnabled = true;
            this.comboBoxBeamFamily.Location = new System.Drawing.Point(185, 147);
            this.comboBoxBeamFamily.Name = "comboBoxBeamFamily";
            this.comboBoxBeamFamily.Size = new System.Drawing.Size(227, 21);
            this.comboBoxBeamFamily.TabIndex = 32;
            this.comboBoxBeamFamily.SelectedIndexChanged += new System.EventHandler(this.comboBoxBeamFamily_SelectedIndexChanged);
            // 
            // comboBoxBeamType
            // 
            this.comboBoxBeamType.FormattingEnabled = true;
            this.comboBoxBeamType.Location = new System.Drawing.Point(463, 147);
            this.comboBoxBeamType.Name = "comboBoxBeamType";
            this.comboBoxBeamType.Size = new System.Drawing.Size(227, 21);
            this.comboBoxBeamType.TabIndex = 31;
            this.comboBoxBeamType.SelectedIndexChanged += new System.EventHandler(this.comboBoxBeamType_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(18, 150);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(37, 13);
            this.label15.TabIndex = 30;
            this.label15.Text = "Beam:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(425, 187);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(34, 13);
            this.label10.TabIndex = 29;
            this.label10.Text = "Type:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(140, 187);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(39, 13);
            this.label11.TabIndex = 28;
            this.label11.Text = "Family:";
            // 
            // comboBoxAdaptiveCompFamily
            // 
            this.comboBoxAdaptiveCompFamily.FormattingEnabled = true;
            this.comboBoxAdaptiveCompFamily.Location = new System.Drawing.Point(185, 184);
            this.comboBoxAdaptiveCompFamily.Name = "comboBoxAdaptiveCompFamily";
            this.comboBoxAdaptiveCompFamily.Size = new System.Drawing.Size(227, 21);
            this.comboBoxAdaptiveCompFamily.TabIndex = 27;
            this.comboBoxAdaptiveCompFamily.SelectedIndexChanged += new System.EventHandler(this.comboBoxAdaptiveCompFamily_SelectedIndexChanged);
            // 
            // comboBoxAdaptiveCompType
            // 
            this.comboBoxAdaptiveCompType.FormattingEnabled = true;
            this.comboBoxAdaptiveCompType.Location = new System.Drawing.Point(463, 184);
            this.comboBoxAdaptiveCompType.Name = "comboBoxAdaptiveCompType";
            this.comboBoxAdaptiveCompType.Size = new System.Drawing.Size(227, 21);
            this.comboBoxAdaptiveCompType.TabIndex = 26;
            this.comboBoxAdaptiveCompType.SelectedIndexChanged += new System.EventHandler(this.comboBoxAdaptiveCompType_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 187);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(109, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Adaptive Component:";
            // 
            // comboBoxColumnMode
            // 
            this.comboBoxColumnMode.FormattingEnabled = true;
            this.comboBoxColumnMode.Items.AddRange(new object[] {
            "Architectural",
            "StructuralVertical",
            "StructuralPointDriven",
            "StructuralAngleDriven"});
            this.comboBoxColumnMode.Location = new System.Drawing.Point(185, 55);
            this.comboBoxColumnMode.Name = "comboBoxColumnMode";
            this.comboBoxColumnMode.Size = new System.Drawing.Size(227, 21);
            this.comboBoxColumnMode.TabIndex = 24;
            this.comboBoxColumnMode.Text = "Architectural";
            this.comboBoxColumnMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxColumnMode_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 23;
            this.label9.Text = "Column Mode:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(425, 114);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Type:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(140, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Family:";
            // 
            // comboBoxColumnStructFamily
            // 
            this.comboBoxColumnStructFamily.FormattingEnabled = true;
            this.comboBoxColumnStructFamily.Location = new System.Drawing.Point(185, 111);
            this.comboBoxColumnStructFamily.Name = "comboBoxColumnStructFamily";
            this.comboBoxColumnStructFamily.Size = new System.Drawing.Size(227, 21);
            this.comboBoxColumnStructFamily.TabIndex = 20;
            this.comboBoxColumnStructFamily.SelectedIndexChanged += new System.EventHandler(this.comboBoxColumnStructFamily_SelectedIndexChanged);
            // 
            // comboBoxColumnStructType
            // 
            this.comboBoxColumnStructType.FormattingEnabled = true;
            this.comboBoxColumnStructType.Location = new System.Drawing.Point(463, 111);
            this.comboBoxColumnStructType.Name = "comboBoxColumnStructType";
            this.comboBoxColumnStructType.Size = new System.Drawing.Size(227, 21);
            this.comboBoxColumnStructType.TabIndex = 19;
            this.comboBoxColumnStructType.SelectedIndexChanged += new System.EventHandler(this.comboBoxColumnStructType_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(425, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(140, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Family:";
            // 
            // comboBoxColumnArchFamily
            // 
            this.comboBoxColumnArchFamily.FormattingEnabled = true;
            this.comboBoxColumnArchFamily.Location = new System.Drawing.Point(185, 83);
            this.comboBoxColumnArchFamily.Name = "comboBoxColumnArchFamily";
            this.comboBoxColumnArchFamily.Size = new System.Drawing.Size(227, 21);
            this.comboBoxColumnArchFamily.TabIndex = 16;
            this.comboBoxColumnArchFamily.SelectedIndexChanged += new System.EventHandler(this.comboBoxColumnArchFamily_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Column - Structural:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Column - Architectural:";
            // 
            // checkBoxAutoClose
            // 
            this.checkBoxAutoClose.AutoSize = true;
            this.checkBoxAutoClose.Location = new System.Drawing.Point(214, 698);
            this.checkBoxAutoClose.Name = "checkBoxAutoClose";
            this.checkBoxAutoClose.Size = new System.Drawing.Size(156, 17);
            this.checkBoxAutoClose.TabIndex = 15;
            this.checkBoxAutoClose.Text = "Automatically Close window";
            this.checkBoxAutoClose.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(258, 254);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(91, 13);
            this.label16.TabIndex = 16;
            this.label16.Text = "Placement Offset:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(371, 254);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(17, 13);
            this.label17.TabIndex = 17;
            this.label17.Text = "X:";
            // 
            // textBoxOriginX
            // 
            this.textBoxOriginX.Location = new System.Drawing.Point(389, 251);
            this.textBoxOriginX.Name = "textBoxOriginX";
            this.textBoxOriginX.Size = new System.Drawing.Size(78, 20);
            this.textBoxOriginX.TabIndex = 18;
            // 
            // textBoxOriginY
            // 
            this.textBoxOriginY.Location = new System.Drawing.Point(515, 251);
            this.textBoxOriginY.Name = "textBoxOriginY";
            this.textBoxOriginY.Size = new System.Drawing.Size(78, 20);
            this.textBoxOriginY.TabIndex = 20;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(497, 254);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(17, 13);
            this.label18.TabIndex = 19;
            this.label18.Text = "Y:";
            // 
            // textBoxOriginZ
            // 
            this.textBoxOriginZ.Location = new System.Drawing.Point(640, 251);
            this.textBoxOriginZ.Name = "textBoxOriginZ";
            this.textBoxOriginZ.Size = new System.Drawing.Size(78, 20);
            this.textBoxOriginZ.TabIndex = 22;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(622, 254);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(17, 13);
            this.label19.TabIndex = 21;
            this.label19.Text = "Z:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonBrowseTempFolder);
            this.groupBox2.Controls.Add(this.textBoxTempFolder);
            this.groupBox2.Controls.Add(this.label28);
            this.groupBox2.Controls.Add(this.buttonBrowseTemplate);
            this.groupBox2.Controls.Add(this.textBoxPathTemplate);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Location = new System.Drawing.Point(29, 551);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(709, 125);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Paths";
            // 
            // buttonBrowseTempFolder
            // 
            this.buttonBrowseTempFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowseTempFolder.Location = new System.Drawing.Point(664, 78);
            this.buttonBrowseTempFolder.Name = "buttonBrowseTempFolder";
            this.buttonBrowseTempFolder.Size = new System.Drawing.Size(27, 28);
            this.buttonBrowseTempFolder.TabIndex = 28;
            this.buttonBrowseTempFolder.Text = "...";
            this.buttonBrowseTempFolder.UseVisualStyleBackColor = true;
            this.buttonBrowseTempFolder.Click += new System.EventHandler(this.buttonBrowseTempFolder_Click);
            // 
            // textBoxTempFolder
            // 
            this.textBoxTempFolder.Location = new System.Drawing.Point(18, 83);
            this.textBoxTempFolder.Name = "textBoxTempFolder";
            this.textBoxTempFolder.Size = new System.Drawing.Size(634, 20);
            this.textBoxTempFolder.TabIndex = 27;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(17, 67);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(214, 13);
            this.label28.TabIndex = 29;
            this.label28.Text = "Path to Folder to be used for temporary files.";
            // 
            // buttonBrowseTemplate
            // 
            this.buttonBrowseTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowseTemplate.Location = new System.Drawing.Point(663, 30);
            this.buttonBrowseTemplate.Name = "buttonBrowseTemplate";
            this.buttonBrowseTemplate.Size = new System.Drawing.Size(27, 28);
            this.buttonBrowseTemplate.TabIndex = 25;
            this.buttonBrowseTemplate.Text = "...";
            this.buttonBrowseTemplate.UseVisualStyleBackColor = true;
            this.buttonBrowseTemplate.Click += new System.EventHandler(this.buttonBrowseTemplate_Click);
            // 
            // textBoxPathTemplate
            // 
            this.textBoxPathTemplate.Location = new System.Drawing.Point(17, 35);
            this.textBoxPathTemplate.Name = "textBoxPathTemplate";
            this.textBoxPathTemplate.Size = new System.Drawing.Size(634, 20);
            this.textBoxPathTemplate.TabIndex = 24;
            this.textBoxPathTemplate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPathTemplate_KeyPress);
            this.textBoxPathTemplate.Leave += new System.EventHandler(this.textBoxPathTemplate_Leave);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(16, 19);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(285, 13);
            this.label23.TabIndex = 26;
            this.label23.Text = "Full Path to Family Template File ( Generic Model or Mass ):";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBoxUnits
            // 
            this.groupBoxUnits.Controls.Add(this.textBoxUnitsFactor);
            this.groupBoxUnits.Controls.Add(this.radioButtonUnitsFactor);
            this.groupBoxUnits.Controls.Add(this.radioButtonUnitsProject);
            this.groupBoxUnits.Location = new System.Drawing.Point(261, 185);
            this.groupBoxUnits.Name = "groupBoxUnits";
            this.groupBoxUnits.Size = new System.Drawing.Size(476, 54);
            this.groupBoxUnits.TabIndex = 24;
            this.groupBoxUnits.TabStop = false;
            this.groupBoxUnits.Text = "Units";
            // 
            // textBoxUnitsFactor
            // 
            this.textBoxUnitsFactor.Location = new System.Drawing.Point(380, 21);
            this.textBoxUnitsFactor.Name = "textBoxUnitsFactor";
            this.textBoxUnitsFactor.Size = new System.Drawing.Size(78, 20);
            this.textBoxUnitsFactor.TabIndex = 19;
            // 
            // radioButtonUnitsFactor
            // 
            this.radioButtonUnitsFactor.AutoSize = true;
            this.radioButtonUnitsFactor.Location = new System.Drawing.Point(214, 21);
            this.radioButtonUnitsFactor.Name = "radioButtonUnitsFactor";
            this.radioButtonUnitsFactor.Size = new System.Drawing.Size(161, 17);
            this.radioButtonUnitsFactor.TabIndex = 1;
            this.radioButtonUnitsFactor.TabStop = true;
            this.radioButtonUnitsFactor.Text = "Use Factor ( To internal FT ):";
            this.radioButtonUnitsFactor.UseVisualStyleBackColor = true;
            // 
            // radioButtonUnitsProject
            // 
            this.radioButtonUnitsProject.AutoSize = true;
            this.radioButtonUnitsProject.Location = new System.Drawing.Point(14, 21);
            this.radioButtonUnitsProject.Name = "radioButtonUnitsProject";
            this.radioButtonUnitsProject.Size = new System.Drawing.Size(143, 17);
            this.radioButtonUnitsProject.TabIndex = 0;
            this.radioButtonUnitsProject.TabStop = true;
            this.radioButtonUnitsProject.Text = "Use current project units.";
            this.radioButtonUnitsProject.UseVisualStyleBackColor = true;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(25, 740);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(10, 13);
            this.label27.TabIndex = 25;
            this.label27.Text = ".";
            // 
            // CreateElements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 758);
            this.ControlBox = false;
            this.Controls.Add(this.label27);
            this.Controls.Add(this.groupBoxUnits);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.textBoxOriginZ);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.textBoxOriginY);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.textBoxOriginX);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.checkBoxAutoClose);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxMaxErrors);
            this.Controls.Add(this.checkBoxSuppressMessages);
            this.Controls.Add(this.groupBoxMode);
            this.Controls.Add(this.listBoxCsvFiles);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxPathFolder);
            this.Controls.Add(this.buttonProcess);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxListErrors);
            this.Name = "CreateElements";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Elements";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CreateElements_FormClosed);
            this.Load += new System.EventHandler(this.CreateElements_Load);
            this.groupBoxMode.ResumeLayout(false);
            this.groupBoxMode.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxUnits.ResumeLayout(false);
            this.groupBoxUnits.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonProcess;
        private System.Windows.Forms.TextBox textBoxPathFolder;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxCsvFiles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBoxMode;
        private System.Windows.Forms.RadioButton radioButtonModeDelete;
        private System.Windows.Forms.RadioButton radioButtonModeKeep;
        private System.Windows.Forms.RadioButton radioButtonModeAdd;
        private System.Windows.Forms.CheckBox checkBoxSuppressMessages;
        private System.Windows.Forms.TextBox textBoxMaxErrors;
        private System.Windows.Forms.CheckBox checkBoxListErrors;
        private System.Windows.Forms.ComboBox comboBoxColumnArchType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxColumnArchFamily;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxColumnStructFamily;
        private System.Windows.Forms.ComboBox comboBoxColumnStructType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxColumnMode;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBoxAdaptiveCompFamily;
        private System.Windows.Forms.ComboBox comboBoxAdaptiveCompType;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox comboBoxBeamFamily;
        private System.Windows.Forms.ComboBox comboBoxBeamType;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox checkBoxAutoClose;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBoxOriginX;
        private System.Windows.Forms.TextBox textBoxOriginY;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox textBoxOriginZ;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ComboBox comboBoxFamilyFamily;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox comboBoxFamilyType;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonBrowseTemplate;
        private System.Windows.Forms.TextBox textBoxPathTemplate;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.ComboBox comboBoxMullionFamily;
        private System.Windows.Forms.ComboBox comboBoxMullionType;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBoxUnits;
        private System.Windows.Forms.TextBox textBoxUnitsFactor;
        private System.Windows.Forms.RadioButton radioButtonUnitsFactor;
        private System.Windows.Forms.RadioButton radioButtonUnitsProject;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button buttonBrowseTempFolder;
        private System.Windows.Forms.TextBox textBoxTempFolder;
        private System.Windows.Forms.Label label28;
    }
}