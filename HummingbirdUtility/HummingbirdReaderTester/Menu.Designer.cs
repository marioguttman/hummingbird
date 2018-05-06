namespace WhiteFeet.HummingbirdReaderTester {
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
            this.checkBoxPreserveId = new System.Windows.Forms.CheckBox();
            this.buttonProcess = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textBoxPathCsv = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.listBoxResults = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // checkBoxPreserveId
            // 
            this.checkBoxPreserveId.AutoSize = true;
            this.checkBoxPreserveId.Location = new System.Drawing.Point(32, 83);
            this.checkBoxPreserveId.Name = "checkBoxPreserveId";
            this.checkBoxPreserveId.Size = new System.Drawing.Size(151, 17);
            this.checkBoxPreserveId.TabIndex = 20;
            this.checkBoxPreserveId.Text = "Preserve Revit Element ID";
            this.checkBoxPreserveId.UseVisualStyleBackColor = true;
            // 
            // buttonProcess
            // 
            this.buttonProcess.Location = new System.Drawing.Point(501, 860);
            this.buttonProcess.Name = "buttonProcess";
            this.buttonProcess.Size = new System.Drawing.Size(102, 23);
            this.buttonProcess.TabIndex = 16;
            this.buttonProcess.Text = "Process";
            this.buttonProcess.UseVisualStyleBackColor = true;
            this.buttonProcess.Click += new System.EventHandler(this.buttonProcess_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(626, 860);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(102, 23);
            this.buttonClose.TabIndex = 15;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textBoxPathCsv
            // 
            this.textBoxPathCsv.Location = new System.Drawing.Point(32, 48);
            this.textBoxPathCsv.Name = "textBoxPathCsv";
            this.textBoxPathCsv.Size = new System.Drawing.Size(650, 20);
            this.textBoxPathCsv.TabIndex = 14;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowse.Location = new System.Drawing.Point(699, 44);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(29, 26);
            this.buttonBrowse.TabIndex = 13;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Full path to CSV File:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // listBoxResults
            // 
            this.listBoxResults.FormattingEnabled = true;
            this.listBoxResults.Location = new System.Drawing.Point(34, 122);
            this.listBoxResults.Name = "listBoxResults";
            this.listBoxResults.Size = new System.Drawing.Size(693, 706);
            this.listBoxResults.TabIndex = 21;
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 909);
            this.Controls.Add(this.listBoxResults);
            this.Controls.Add(this.checkBoxPreserveId);
            this.Controls.Add(this.buttonProcess);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.textBoxPathCsv);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.label1);
            this.Name = "Menu";
            this.Text = "Hummingbird Reader Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxPreserveId;
        private System.Windows.Forms.Button buttonProcess;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textBoxPathCsv;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ListBox listBoxResults;
    }
}

