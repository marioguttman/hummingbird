namespace RevitModelBuilder {
    partial class Menu {
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
            this.label10 = new System.Windows.Forms.Label();
            this.buttonReloadSettings = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonExportElements = new System.Windows.Forms.Button();
            this.buttonCreateElements = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCsvViewer = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(-2, 259);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(10, 13);
            this.label10.TabIndex = 57;
            this.label10.Text = ".";
            // 
            // buttonReloadSettings
            // 
            this.buttonReloadSettings.Location = new System.Drawing.Point(374, 243);
            this.buttonReloadSettings.Name = "buttonReloadSettings";
            this.buttonReloadSettings.Size = new System.Drawing.Size(188, 27);
            this.buttonReloadSettings.TabIndex = 55;
            this.buttonReloadSettings.Text = "Reload Default Settings";
            this.buttonReloadSettings.UseVisualStyleBackColor = true;
            this.buttonReloadSettings.Click += new System.EventHandler(this.buttonReloadSettings_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(29, 253);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(166, 13);
            this.label7.TabIndex = 54;
            this.label7.Text = "Version R2019 - Build 2018-05-03";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(588, 243);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(188, 27);
            this.buttonClose.TabIndex = 53;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.buttonExportElements);
            this.groupBox1.Controls.Add(this.buttonCreateElements);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(26, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(588, 111);
            this.groupBox1.TabIndex = 59;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Import/Export Elements";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(233, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(199, 26);
            this.label5.TabIndex = 6;
            this.label5.Text = "Process current selection.\r\nExport descriptive language to .CSV File.";
            // 
            // buttonExportElements
            // 
            this.buttonExportElements.Location = new System.Drawing.Point(18, 24);
            this.buttonExportElements.Name = "buttonExportElements";
            this.buttonExportElements.Size = new System.Drawing.Size(193, 27);
            this.buttonExportElements.TabIndex = 21;
            this.buttonExportElements.Text = "Export Elements to File";
            this.buttonExportElements.UseVisualStyleBackColor = true;
            this.buttonExportElements.Click += new System.EventHandler(this.buttonExportElements_Click);
            // 
            // buttonCreateElements
            // 
            this.buttonCreateElements.Location = new System.Drawing.Point(19, 63);
            this.buttonCreateElements.Name = "buttonCreateElements";
            this.buttonCreateElements.Size = new System.Drawing.Size(194, 27);
            this.buttonCreateElements.TabIndex = 21;
            this.buttonCreateElements.Text = "Create Elements from File";
            this.buttonCreateElements.UseVisualStyleBackColor = true;
            this.buttonCreateElements.Click += new System.EventHandler(this.buttonCreateElements_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(233, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(232, 26);
            this.label3.TabIndex = 6;
            this.label3.Text = "Read .CSV file containing descriptive language.\r\nBuild elements in Revit.";
            // 
            // buttonCsvViewer
            // 
            this.buttonCsvViewer.Location = new System.Drawing.Point(19, 24);
            this.buttonCsvViewer.Name = "buttonCsvViewer";
            this.buttonCsvViewer.Size = new System.Drawing.Size(187, 28);
            this.buttonCsvViewer.TabIndex = 61;
            this.buttonCsvViewer.Text = "Launch CSV Viewer";
            this.buttonCsvViewer.UseVisualStyleBackColor = true;
            this.buttonCsvViewer.Click += new System.EventHandler(this.buttonCsvViewer_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(233, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(446, 26);
            this.label2.TabIndex = 6;
            this.label2.Text = "Launches a viewer for .CSV files in the Model Builder format.\r\n(Use a shortcut to" +
    " \"Hummingbird\\Program\\HummingbirdCsvViewer.exe\" to run without Revit.)";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.buttonCsvViewer);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox3.Location = new System.Drawing.Point(26, 148);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(750, 76);
            this.groupBox3.TabIndex = 62;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "CSV File Viewer";
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 299);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.buttonReloadSettings);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.buttonClose);
            this.Name = "Menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hummingbird - Revit Model Builder";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonReloadSettings;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonExportElements;
        private System.Windows.Forms.Button buttonCreateElements;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCsvViewer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}