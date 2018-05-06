namespace WhiteFeet.HummingbirdWriterTester {
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
            this.buttonProcess = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textBoxCsvFolder = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCsvFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxPreserveId = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // buttonProcess
            // 
            this.buttonProcess.Location = new System.Drawing.Point(489, 139);
            this.buttonProcess.Name = "buttonProcess";
            this.buttonProcess.Size = new System.Drawing.Size(102, 23);
            this.buttonProcess.TabIndex = 8;
            this.buttonProcess.Text = "Process";
            this.buttonProcess.UseVisualStyleBackColor = true;
            this.buttonProcess.Click += new System.EventHandler(this.buttonProcess_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(614, 139);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(102, 23);
            this.buttonClose.TabIndex = 7;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textBoxCsvFolder
            // 
            this.textBoxCsvFolder.Location = new System.Drawing.Point(20, 30);
            this.textBoxCsvFolder.Name = "textBoxCsvFolder";
            this.textBoxCsvFolder.Size = new System.Drawing.Size(650, 20);
            this.textBoxCsvFolder.TabIndex = 6;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowse.Location = new System.Drawing.Point(687, 26);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(29, 26);
            this.buttonBrowse.TabIndex = 5;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Full path to CSV Folder:";
            // 
            // textBoxCsvFileName
            // 
            this.textBoxCsvFileName.Location = new System.Drawing.Point(20, 74);
            this.textBoxCsvFileName.Name = "textBoxCsvFileName";
            this.textBoxCsvFileName.Size = new System.Drawing.Size(294, 20);
            this.textBoxCsvFileName.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "CSV File Name: ";
            // 
            // checkBoxPreserveId
            // 
            this.checkBoxPreserveId.AutoSize = true;
            this.checkBoxPreserveId.Location = new System.Drawing.Point(415, 76);
            this.checkBoxPreserveId.Name = "checkBoxPreserveId";
            this.checkBoxPreserveId.Size = new System.Drawing.Size(151, 17);
            this.checkBoxPreserveId.TabIndex = 12;
            this.checkBoxPreserveId.Text = "Preserve Revit Element ID";
            this.checkBoxPreserveId.UseVisualStyleBackColor = true;
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 198);
            this.Controls.Add(this.checkBoxPreserveId);
            this.Controls.Add(this.textBoxCsvFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonProcess);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.textBoxCsvFolder);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.label1);
            this.Name = "Menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hummingbird Writer Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonProcess;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textBoxCsvFolder;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCsvFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxPreserveId;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

