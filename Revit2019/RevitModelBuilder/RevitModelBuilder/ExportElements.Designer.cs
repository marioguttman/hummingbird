namespace RevitModelBuilder {
    partial class ExportElements {
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
            this.textBoxPathCsvFolder = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxSelection = new System.Windows.Forms.GroupBox();
            this.radioButtonSelectView = new System.Windows.Forms.RadioButton();
            this.radioButtonSelectAll = new System.Windows.Forms.RadioButton();
            this.radioButtonSelectCurrent = new System.Windows.Forms.RadioButton();
            this.checkBoxRoundPoints = new System.Windows.Forms.CheckBox();
            this.comboBoxDecimals = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBoxConvert = new System.Windows.Forms.GroupBox();
            this.radioButtonConvertFilledRegions = new System.Windows.Forms.RadioButton();
            this.radioButtonConvertDetailLines = new System.Windows.Forms.RadioButton();
            this.radioButtonConvertModelLines = new System.Windows.Forms.RadioButton();
            this.radioButtonConvertNone = new System.Windows.Forms.RadioButton();
            this.checkBoxAutoClose = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxCsvFileName = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.textBoxMaxErrors = new System.Windows.Forms.TextBox();
            this.checkBoxListErrors = new System.Windows.Forms.CheckBox();
            this.label27 = new System.Windows.Forms.Label();
            this.groupBoxSelection.SuspendLayout();
            this.groupBoxConvert.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(606, 235);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(124, 28);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonProcess
            // 
            this.buttonProcess.Location = new System.Drawing.Point(437, 235);
            this.buttonProcess.Name = "buttonProcess";
            this.buttonProcess.Size = new System.Drawing.Size(124, 28);
            this.buttonProcess.TabIndex = 1;
            this.buttonProcess.Text = "Process";
            this.buttonProcess.UseVisualStyleBackColor = true;
            this.buttonProcess.Click += new System.EventHandler(this.buttonProcess_Click);
            // 
            // textBoxPathCsvFolder
            // 
            this.textBoxPathCsvFolder.Location = new System.Drawing.Point(29, 34);
            this.textBoxPathCsvFolder.Name = "textBoxPathCsvFolder";
            this.textBoxPathCsvFolder.Size = new System.Drawing.Size(668, 20);
            this.textBoxPathCsvFolder.TabIndex = 2;
            this.textBoxPathCsvFolder.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPathCsvFolder_KeyPress);
            this.textBoxPathCsvFolder.Leave += new System.EventHandler(this.textBoxPathCsvFolder_Leave);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowse.Location = new System.Drawing.Point(703, 29);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(27, 28);
            this.buttonBrowse.TabIndex = 3;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowseCsvFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Path to Folder for CSV File:";
            // 
            // groupBoxSelection
            // 
            this.groupBoxSelection.Controls.Add(this.radioButtonSelectView);
            this.groupBoxSelection.Controls.Add(this.radioButtonSelectAll);
            this.groupBoxSelection.Controls.Add(this.radioButtonSelectCurrent);
            this.groupBoxSelection.Location = new System.Drawing.Point(29, 119);
            this.groupBoxSelection.Name = "groupBoxSelection";
            this.groupBoxSelection.Size = new System.Drawing.Size(183, 105);
            this.groupBoxSelection.TabIndex = 9;
            this.groupBoxSelection.TabStop = false;
            this.groupBoxSelection.Text = "Selection to Process";
            // 
            // radioButtonSelectView
            // 
            this.radioButtonSelectView.AutoSize = true;
            this.radioButtonSelectView.Location = new System.Drawing.Point(15, 46);
            this.radioButtonSelectView.Name = "radioButtonSelectView";
            this.radioButtonSelectView.Size = new System.Drawing.Size(122, 17);
            this.radioButtonSelectView.TabIndex = 2;
            this.radioButtonSelectView.TabStop = true;
            this.radioButtonSelectView.Text = "Active view in Revit.";
            this.radioButtonSelectView.UseVisualStyleBackColor = true;
            // 
            // radioButtonSelectAll
            // 
            this.radioButtonSelectAll.AutoSize = true;
            this.radioButtonSelectAll.Location = new System.Drawing.Point(15, 74);
            this.radioButtonSelectAll.Name = "radioButtonSelectAll";
            this.radioButtonSelectAll.Size = new System.Drawing.Size(115, 17);
            this.radioButtonSelectAll.TabIndex = 1;
            this.radioButtonSelectAll.TabStop = true;
            this.radioButtonSelectAll.Text = "All model elements.";
            this.radioButtonSelectAll.UseVisualStyleBackColor = true;
            // 
            // radioButtonSelectCurrent
            // 
            this.radioButtonSelectCurrent.AutoSize = true;
            this.radioButtonSelectCurrent.Location = new System.Drawing.Point(15, 19);
            this.radioButtonSelectCurrent.Name = "radioButtonSelectCurrent";
            this.radioButtonSelectCurrent.Size = new System.Drawing.Size(146, 17);
            this.radioButtonSelectCurrent.TabIndex = 0;
            this.radioButtonSelectCurrent.TabStop = true;
            this.radioButtonSelectCurrent.Text = "Current selection in Revit.";
            this.radioButtonSelectCurrent.UseVisualStyleBackColor = true;
            // 
            // checkBoxRoundPoints
            // 
            this.checkBoxRoundPoints.AutoSize = true;
            this.checkBoxRoundPoints.Location = new System.Drawing.Point(437, 138);
            this.checkBoxRoundPoints.Name = "checkBoxRoundPoints";
            this.checkBoxRoundPoints.Size = new System.Drawing.Size(131, 17);
            this.checkBoxRoundPoints.TabIndex = 10;
            this.checkBoxRoundPoints.Text = "Round Real Numbers:";
            this.checkBoxRoundPoints.UseVisualStyleBackColor = true;
            // 
            // comboBoxDecimals
            // 
            this.comboBoxDecimals.FormattingEnabled = true;
            this.comboBoxDecimals.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.comboBoxDecimals.Location = new System.Drawing.Point(675, 136);
            this.comboBoxDecimals.Name = "comboBoxDecimals";
            this.comboBoxDecimals.Size = new System.Drawing.Size(55, 21);
            this.comboBoxDecimals.TabIndex = 11;
            this.comboBoxDecimals.Text = "2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(586, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Decimal Places:";
            // 
            // groupBoxConvert
            // 
            this.groupBoxConvert.Controls.Add(this.radioButtonConvertFilledRegions);
            this.groupBoxConvert.Controls.Add(this.radioButtonConvertDetailLines);
            this.groupBoxConvert.Controls.Add(this.radioButtonConvertModelLines);
            this.groupBoxConvert.Controls.Add(this.radioButtonConvertNone);
            this.groupBoxConvert.Location = new System.Drawing.Point(231, 119);
            this.groupBoxConvert.Name = "groupBoxConvert";
            this.groupBoxConvert.Size = new System.Drawing.Size(181, 144);
            this.groupBoxConvert.TabIndex = 13;
            this.groupBoxConvert.TabStop = false;
            this.groupBoxConvert.Text = "Convert Elements";
            // 
            // radioButtonConvertFilledRegions
            // 
            this.radioButtonConvertFilledRegions.AutoSize = true;
            this.radioButtonConvertFilledRegions.Location = new System.Drawing.Point(16, 102);
            this.radioButtonConvertFilledRegions.Name = "radioButtonConvertFilledRegions";
            this.radioButtonConvertFilledRegions.Size = new System.Drawing.Size(107, 17);
            this.radioButtonConvertFilledRegions.TabIndex = 3;
            this.radioButtonConvertFilledRegions.TabStop = true;
            this.radioButtonConvertFilledRegions.Text = "To Filled Regions";
            this.radioButtonConvertFilledRegions.UseVisualStyleBackColor = true;
            // 
            // radioButtonConvertDetailLines
            // 
            this.radioButtonConvertDetailLines.AutoSize = true;
            this.radioButtonConvertDetailLines.Location = new System.Drawing.Point(15, 46);
            this.radioButtonConvertDetailLines.Name = "radioButtonConvertDetailLines";
            this.radioButtonConvertDetailLines.Size = new System.Drawing.Size(96, 17);
            this.radioButtonConvertDetailLines.TabIndex = 2;
            this.radioButtonConvertDetailLines.TabStop = true;
            this.radioButtonConvertDetailLines.Text = "To Detail Lines";
            this.radioButtonConvertDetailLines.UseVisualStyleBackColor = true;
            // 
            // radioButtonConvertModelLines
            // 
            this.radioButtonConvertModelLines.AutoSize = true;
            this.radioButtonConvertModelLines.Location = new System.Drawing.Point(15, 74);
            this.radioButtonConvertModelLines.Name = "radioButtonConvertModelLines";
            this.radioButtonConvertModelLines.Size = new System.Drawing.Size(98, 17);
            this.radioButtonConvertModelLines.TabIndex = 1;
            this.radioButtonConvertModelLines.TabStop = true;
            this.radioButtonConvertModelLines.Text = "To Model Lines";
            this.radioButtonConvertModelLines.UseVisualStyleBackColor = true;
            // 
            // radioButtonConvertNone
            // 
            this.radioButtonConvertNone.AutoSize = true;
            this.radioButtonConvertNone.Location = new System.Drawing.Point(15, 19);
            this.radioButtonConvertNone.Name = "radioButtonConvertNone";
            this.radioButtonConvertNone.Size = new System.Drawing.Size(99, 17);
            this.radioButtonConvertNone.TabIndex = 0;
            this.radioButtonConvertNone.TabStop = true;
            this.radioButtonConvertNone.Text = "Do Not Convert";
            this.radioButtonConvertNone.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoClose
            // 
            this.checkBoxAutoClose.AutoSize = true;
            this.checkBoxAutoClose.Location = new System.Drawing.Point(437, 194);
            this.checkBoxAutoClose.Name = "checkBoxAutoClose";
            this.checkBoxAutoClose.Size = new System.Drawing.Size(156, 17);
            this.checkBoxAutoClose.TabIndex = 14;
            this.checkBoxAutoClose.Text = "Automatically Close window";
            this.checkBoxAutoClose.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 36;
            this.label3.Text = "CSV File Name:";
            // 
            // textBoxCsvFileName
            // 
            this.textBoxCsvFileName.Location = new System.Drawing.Point(31, 77);
            this.textBoxCsvFileName.Name = "textBoxCsvFileName";
            this.textBoxCsvFileName.Size = new System.Drawing.Size(381, 20);
            this.textBoxCsvFileName.TabIndex = 35;
            this.textBoxCsvFileName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCsvFileName_KeyPress);
            this.textBoxCsvFileName.Leave += new System.EventHandler(this.textBoxCsvFileName_Leave);
            // 
            // textBoxMaxErrors
            // 
            this.textBoxMaxErrors.Location = new System.Drawing.Point(544, 165);
            this.textBoxMaxErrors.Name = "textBoxMaxErrors";
            this.textBoxMaxErrors.Size = new System.Drawing.Size(38, 20);
            this.textBoxMaxErrors.TabIndex = 38;
            // 
            // checkBoxListErrors
            // 
            this.checkBoxListErrors.AutoSize = true;
            this.checkBoxListErrors.Location = new System.Drawing.Point(437, 166);
            this.checkBoxListErrors.Name = "checkBoxListErrors";
            this.checkBoxListErrors.Size = new System.Drawing.Size(107, 17);
            this.checkBoxListErrors.TabIndex = 39;
            this.checkBoxListErrors.Text = "List Errors    Max:";
            this.checkBoxListErrors.UseVisualStyleBackColor = true;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(30, 263);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(10, 13);
            this.label27.TabIndex = 41;
            this.label27.Text = ".";
            // 
            // ExportElements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 290);
            this.ControlBox = false;
            this.Controls.Add(this.label27);
            this.Controls.Add(this.textBoxMaxErrors);
            this.Controls.Add(this.checkBoxListErrors);
            this.Controls.Add(this.textBoxCsvFileName);
            this.Controls.Add(this.checkBoxAutoClose);
            this.Controls.Add(this.groupBoxConvert);
            this.Controls.Add(this.comboBoxDecimals);
            this.Controls.Add(this.checkBoxRoundPoints);
            this.Controls.Add(this.groupBoxSelection);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxPathCsvFolder);
            this.Controls.Add(this.buttonProcess);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Name = "ExportElements";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export Elements";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExportElements_FormClosed);
            this.groupBoxSelection.ResumeLayout(false);
            this.groupBoxSelection.PerformLayout();
            this.groupBoxConvert.ResumeLayout(false);
            this.groupBoxConvert.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonProcess;
        private System.Windows.Forms.TextBox textBoxPathCsvFolder;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxSelection;
        private System.Windows.Forms.RadioButton radioButtonSelectView;
        private System.Windows.Forms.RadioButton radioButtonSelectAll;
        private System.Windows.Forms.RadioButton radioButtonSelectCurrent;
        private System.Windows.Forms.CheckBox checkBoxRoundPoints;
        private System.Windows.Forms.ComboBox comboBoxDecimals;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBoxConvert;
        private System.Windows.Forms.RadioButton radioButtonConvertDetailLines;
        private System.Windows.Forms.RadioButton radioButtonConvertModelLines;
        private System.Windows.Forms.RadioButton radioButtonConvertNone;
        private System.Windows.Forms.CheckBox checkBoxAutoClose;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxCsvFileName;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox textBoxMaxErrors;
        private System.Windows.Forms.CheckBox checkBoxListErrors;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.RadioButton radioButtonConvertFilledRegions;
    }
}