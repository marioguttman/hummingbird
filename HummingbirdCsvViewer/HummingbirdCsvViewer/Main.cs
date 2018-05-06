using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Reflection;

using DataCsv;

namespace HummingbirdCsvViewer {
    public partial class Form1 : Form {

        // ***************************************************** Module Variables ******************************************************

        private const string USER_FOLDER_NAME = "Hummingbird";   // Used to store license usage file, fallback for .ini files, and possibly future user settings  

        private const string constantProgramName = "HummingbirdCsvViewer";
        private const string constantIniFileName = "HummingbirdCsvViewer.ini";

        private string userFolderPath = "";
        private string pathFolder = "";
        private string pathFile = "";
        private string pathIniFile = "";
        private string currentfile = "";

        private CsvAdapter csvAdapter;
        private DataTable dataTable = new DataTable();

        // ********************************************* Constructor and Form Events *****************************************************
        public Form1() {
            InitializeComponent();            
            InitializeDataTable();
            FindOrMakeUserFolder();
            this.pathIniFile = Path.Combine(this.userFolderPath, constantIniFileName);

            //System.Environment.SpecialFolder userMyDocumentsFolder = System.Environment.SpecialFolder.MyDocuments;
            //string userDocumentsPath = System.Environment.GetFolderPath(userMyDocumentsFolder, Environment.SpecialFolderOption.Create);  //returns "" if not found
            //this.pathIniFile = userDocumentsPath + @"\" + constantIniFileName;
            if (File.Exists(this.pathIniFile)) {
                try {
                    using (StreamReader streamReader = File.OpenText(pathIniFile)) {
                        this.pathFolder = streamReader.ReadLine().Trim();
                        this.textBoxPathFolder.Text = this.pathFolder;
                        this.currentfile = streamReader.ReadLine().Trim();
                    }
                }
                catch { }
            }
    
        }

        private void Form1_Shown(object sender, EventArgs e) {                        
            buttonRefresh.Focus(); // triggers a Leave event which sets to first name or balnk and calls ReadFile();
            this.comboBoxFile.Text = this.currentfile;
            this.pathFile = this.pathFolder + @"\" + comboBoxFile.Text + ".csv";
            ReadFile();   // Second call to ReadFile is wasteful and we could prevent with a "FirstTime" boolean. 

            // Try to point to sample data if no path has been entered
            if (string.IsNullOrWhiteSpace(textBoxPathFolder.Text)) {
                string currentFolder = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf("\\"));
                string parentFolder = Directory.GetParent(currentFolder).ToString();
                string dataFolder = Path.Combine(parentFolder, "Data");
                if (Directory.Exists(dataFolder)) {
                    textBoxPathFolder.Text = dataFolder;
                    this.pathFolder = dataFolder;
                    GetFileNames();
                    
                }
            } 
        }

        private void Main_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) {
            try {
                if (Directory.Exists(this.userFolderPath)) {
                    using (StreamWriter streamWriter = new StreamWriter(this.pathIniFile)) {
                        streamWriter.WriteLine(this.textBoxPathFolder.Text);
                        streamWriter.WriteLine(this.comboBoxFile.Text);
                    }
                }
            }
            catch { }
        }
        // *********************************************** Private Functions *******************************************************

        private void InitializeDataTable() {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("RowId", typeof(string));
            this.dataTable.Columns.Add("ElementId", typeof(string));
            this.dataTable.Columns.Add("Action", typeof(string));
            this.dataTable.Columns.Add("Object", typeof(string));
            this.dataTable.Columns.Add("Value01", typeof(string));
            this.dataTable.Columns.Add("Value02", typeof(string));
            this.dataTable.Columns.Add("Value03", typeof(string));
            this.dataTable.Columns.Add("Value04", typeof(string));
            this.dataGridView1.DataSource = this.dataTable;
            this.dataGridView1.Columns[0].Width = 80;   // RowId
            this.dataGridView1.Columns[1].Width = 90;   // ElementId
            this.dataGridView1.Columns[2].Width = 100;  // Action
            this.dataGridView1.Columns[3].Width = 150;  // Object
            this.dataGridView1.Columns[4].Width = 250;  // Value01
            this.dataGridView1.Columns[5].Width = 250;  // Value02
            this.dataGridView1.Columns[6].Width = 250;  // Value03
            this.dataGridView1.Columns[7].Width = 250;  // Value04
            //this.dataGridView1.Dock = DockStyle.Bottom;
        }

        private void GetFileNames() {
            comboBoxFile.Items.Clear();
            comboBoxFile.Text = "";
            List<string> fileNames = new List<string>();
            if (Directory.Exists(this.pathFolder)) {
                string[] files = Directory.GetFiles(this.pathFolder, "*.csv", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Count(); i++) {
                    fileNames.Add(Path.GetFileNameWithoutExtension(files[i]));
                }
            }
            fileNames.Sort();
            foreach (string name in fileNames) {
                comboBoxFile.Items.Add(name);
            }
            if (comboBoxFile.Items.Count > 0) comboBoxFile.SelectedIndex = 0;
        }
        
        private void ReadFile() {
            if (File.Exists(this.pathFile)) {
                if (this.dataTable == null) InitializeDataTable();           
                else this.dataTable.Clear();
                csvAdapter = new CsvAdapter(this.pathFile, this.dataTable, constantProgramName);
                csvAdapter.ReadFile();
            }
            else {
                if (this.dataTable != null) this.dataTable.Clear();
                //this.csvAdapter = null;
                //this.dataTable = null;
            }
        }

        private bool FindOrMakeUserFolder() {
            try {
                this.userFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), USER_FOLDER_NAME);
                if (!Directory.Exists(this.userFolderPath)) {
                    try {
                        Directory.CreateDirectory(this.userFolderPath);
                    }
                    catch {
                        this.userFolderPath = "";
                        return false;  // Returning silently.  this.UserFolderPath = "" is indication of failure
                    }
                }
                return true;
            }
            catch {
                // Returning silently for now
                return false;
            }
        }


        // *********************************************** Event Handlers *******************************************************

        private void buttonBrowseFolder_Click(object sender, EventArgs e) {
            System.Windows.Forms.DialogResult dialogResult;
            string path = textBoxPathFolder.Text;
            if (path.EndsWith("\\")) path = path.Substring(0, path.Length - 1);  //In case user added a "\"
            if (Directory.Exists(path)) folderBrowserDialog1.SelectedPath = path;
            else folderBrowserDialog1.SelectedPath = "C:\\";
            dialogResult = folderBrowserDialog1.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK) {
                path = folderBrowserDialog1.SelectedPath.ToString();
                if (Directory.Exists(path)) textBoxPathFolder.Text = path;
                else textBoxPathFolder.Text = "";
                this.pathFolder = textBoxPathFolder.Text;
                GetFileNames();
            }
        }

        // Not using Leave event since it tends to trigger too much.
        private void textBoxPathFolder_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
            if (e.KeyChar.ToString() == "\r") {
                textBoxPathFolder_Leave(sender, e);
                //textBoxPathFolder.Text = textBoxPathFolder.Text.Trim();
                //this.pathFolder = textBoxPathFolder.Text;
                //GetFileNames();
            }
        }
        private void textBoxPathFolder_Leave(object sender, EventArgs e) {                        
            textBoxPathFolder.Text = textBoxPathFolder.Text.Trim();
            this.pathFolder = textBoxPathFolder.Text;
            GetFileNames();            
            comboBoxFile_SelectedIndexChanged(sender, e);
        }

        private void comboBoxFile_SelectedIndexChanged(object sender, EventArgs e) {
            buttonRefresh.Focus(); // triggers a Leave event which is wasteful but not really noticable.
            this.pathFile = this.pathFolder + @"\" + comboBoxFile.Text + ".csv";
            ReadFile();
        }

        private void buttonRefresh_Click(object sender, EventArgs e) {
            this.dataTable.Clear();
            csvAdapter.ReadFile();
        }

        private void buttonWrite_Click(object sender, EventArgs e) {
            csvAdapter.WriteFile();
        }



    }
}
