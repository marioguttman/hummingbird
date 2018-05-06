
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using HummingbirdUtility;

namespace WhiteFeet.HummingbirdReaderTester {
    public partial class Menu : Form {

        private const string USER_FOLDER_HUMMINGBIRD = "Hummingbird";   // Used to store all Hummingbird data
        private const string INI_FILE_NAME = "HummingbirdReaderTester.ini";
        private const string DELIMITER = "\t";

        private string userFolderPath = "";
        private string iniFilePath = "";

        private string pathCsvFile;

        public Menu() {
            InitializeComponent();
            FindOrMakeUserFolder();  // Not looking for error.  this.UserFolderPath = "" is indication of failure
            GetIniFilePath();
            ReadIniFile();
            textBoxPathCsv.Text = this.pathCsvFile;

// Work Around
//textBoxPathCsv.Text = "C:\\Temp\\InputTest.csv";
        }

        // ************************************************** Private Functions ****************************************************
        private void Process() {

            listBoxResults.Items.Clear();
            listBoxResults.Items.Add("");

            CsvReader csvReader = new CsvReader();
            csvReader.ConnectToFile(textBoxPathCsv.Text);
            string returnValue = csvReader.ReadFile();
            if (returnValue != "") MessageBox.Show(returnValue);
            returnValue = csvReader.GetInput();
            if (returnValue != "") MessageBox.Show(returnValue);
            else {
                //csvReader.
                List<HbInputItem> inputItems = csvReader.HbInputItems;
                foreach (HbInputItem inputItem in inputItems) {
                    string valueElementId;
                    if (inputItem.ElementId == null) valueElementId = "NULL";
                    else valueElementId = inputItem.ElementId.ElementIdValue.ToString();
                    listBoxResults.Items.Add("RowId: " + inputItem.RowId + " || ElementId: " + valueElementId);
                    listBoxResults.Items.Add(inputItem.CommandAction + " || " + inputItem.CommandObject + " || " + inputItem.CommandModifier);
                    if (inputItem.ListHbXYZ == null) listBoxResults.Items.Add("ListHbXYZ = NULL");
                    else {
                        listBoxResults.Items.Add("ListHbXYZ:");
                        foreach (HbXYZ hbXyz in inputItem.ListHbXYZ) {
                            if (hbXyz == null) listBoxResults.Items.Add("     (Null value for hbXyz.)");
                            else listBoxResults.Items.Add("     (" + hbXyz.X.ToString() + ", " + hbXyz.Y.ToString() + ", " + hbXyz.Z.ToString() + ")");
                        }
                    }
                    if (inputItem.HbCurveArrArray == null) listBoxResults.Items.Add("HbCurveArrayArray = NULL");
                    else {
                        listBoxResults.Items.Add("HbCurveArrayArray:");
                        foreach (HbCurveArray hbCurveArray in inputItem.HbCurveArrArray) {
                            listBoxResults.Items.Add("     CurveArray:" + hbCurveArray.ToString());
                            foreach (HbCurve hbCurve in hbCurveArray) {
                                string hbName = hbCurve.ToString();
                                string hbType = hbName.Substring(hbName.LastIndexOf(".") + 1);
                                switch (hbType) {
                                    case "HbLine":
                                        HbLine hbLine = (HbLine)hbCurve;
                                        listBoxResults.Items.Add("          HbLine: (" + hbLine.PointStart.X.ToString() + ", " + hbLine.PointStart.Y.ToString() + ", "+ hbLine.PointStart.Z.ToString() + ") "
                                                                               + "(" + hbLine.PointEnd.X.ToString() + ", " + hbLine.PointEnd.Y.ToString() + ", "+ hbLine.PointEnd.Z.ToString() + ")");                                        
                                        break;
                                    case "HbArc":
                                        HbArc hbArc = (HbArc)hbCurve;
                                        listBoxResults.Items.Add("          HbArc: (" + hbArc.PointStart.X.ToString() + ", " + hbArc.PointStart.Y.ToString() + ", " + hbArc.PointStart.Z.ToString() + ") "
                                                                                + "(" + hbArc.PointMid.X.ToString() + ", " + hbArc.PointMid.Y.ToString() + ", " + hbArc.PointMid.Z.ToString() + ") "
                                                                                + "(" + hbArc.PointEnd.X.ToString() + ", " + hbArc.PointEnd.Y.ToString() + ", " + hbArc.PointEnd.Z.ToString() + ")");
                                        break;
                                    default:
                                        listBoxResults.Items.Add("          " + hbCurve.ToString());
                                        break;
                                }                                
                            }
                        }                        
                    }
                    if (inputItem.HbReferenceArrayArray == null) listBoxResults.Items.Add("HbReferenceArrayArray = NULL");
                    else {
                        listBoxResults.Items.Add("HbReferenceArrayArray:");
                        foreach (HbReferenceArray hbReferenceArray in inputItem.HbReferenceArrayArray) {
                            listBoxResults.Items.Add("     " + hbReferenceArray.ToString()); 
                        }                        
                    }
                    //if (inputItem.Variables == null) listBoxResults.Items.Add("Variables = NULL");
                    //else {
                    //    listBoxResults.Items.Add("Variables:");
                    //    foreach (string variable in inputItem.Variables) {
                    //        listBoxResults.Items.Add("     " + variable); 
                    //    }                        
                    //}
                    listBoxResults.Items.Add("");
                    listBoxResults.Items.Add("-------------------------------------------------------------------------------------------------");
                    listBoxResults.Items.Add("");
                    //string valueElementId;
                    //string valuesListHbXYZ;
                    //string valuesHbCurveArrayArray;
                    //string valuesHbReferenceArrayArray;
                    //string valuesVariables;
                    //string valueElementId;
                    //string valuesListHbXYZ;
                    //string valuesHbCurveArrayArray;
                    //string valuesHbReferenceArrayArray;
                    //string valuesVariables;
                    //if (inputItem.ElementId == null) valueElementId = "NULL";
                    //else valueElementId = inputItem.ElementId.ElementIdValue.ToString();
                    //if (inputItem.ListHbXYZ == null) valuesListHbXYZ = "ListHbXYZ = NULL";
                    //else {
                    //    valuesListHbXYZ = "ListHbXYZ:";
                    //    foreach (HbXYZ hbXyz in inputItem.ListHbXYZ) {
                    //        valuesListHbXYZ += "\n     (" + hbXyz.X.ToString() + ", " + hbXyz.Y.ToString() + ", " + hbXyz.Z.ToString() + ")";
                    //    }
                    //}
                    //if (inputItem.HbCurveArrayArray == null) valuesHbCurveArrayArray = "HbCurveArrayArray = NULL";
                    //else {
                    //    valuesHbCurveArrayArray = "HbCurveArrayArray:";
                    //    foreach (HbCurveArray hbCurveArray in inputItem.HbCurveArrayArray) {
                    //        valuesHbCurveArrayArray += "\n     " + hbCurveArray.ToString();
                    //    }
                    //}
                    //if (inputItem.HbReferenceArrayArray == null) valuesHbReferenceArrayArray = "HbReferenceArrayArray = NULL";
                    //else {
                    //    valuesHbReferenceArrayArray = "HbReferenceArrayArray:";
                    //    foreach (HbReferenceArray hbReferenceArray in inputItem.HbReferenceArrayArray) {
                    //        valuesHbReferenceArrayArray += "\n     " + hbReferenceArray.ToString();
                    //    }
                    //}
                    //if (inputItem.Variables == null) valuesVariables = "Variables = NULL";
                    //else {
                    //    valuesVariables = "Variables:";
                    //    foreach (string variable in inputItem.Variables) {
                    //        valuesVariables += "\n     " + variable;
                    //    }
                    //}
                    //DialogResult result = MessageBox.Show(
                    //    "RowId: " + inputItem.RowId + " || ElementId: " + valueElementId + "\n"
                    //  + inputItem.CommandAction + " || " + inputItem.CommandObject + " || " + inputItem.CommandModifier + "\n"
                    //  + valuesListHbXYZ + "\n"
                    //  + valuesHbCurveArrayArray + "\n"
                    //  + valuesHbReferenceArrayArray + "\n"
                    //  + valuesVariables + "\n"
                    //    , constantProgramName, MessageBoxButtons.OKCancel);
                    //if (result != DialogResult.OK) break;
                }
            }


        }
        // ********************************************************** Ini File Functions ***************************************************
        private bool GetIniFilePath() {
            try {
                if (this.userFolderPath == "") {
                    this.iniFilePath = "";
                } else {
                    this.iniFilePath = Path.Combine(this.userFolderPath, INI_FILE_NAME);
                }
                return true;
            }
            catch {
                return false;
            }
        }
        private bool ReadIniFile() {
            if (this.iniFilePath == "") return true; //no warning, just no action
            try {

                if (File.Exists(this.iniFilePath)) {
                    using (StreamReader streamReader = File.OpenText(this.iniFilePath)) {
                        this.pathCsvFile = GetOneSettingString(streamReader);
                        checkBoxPreserveId.Checked = GetOneSettingBool(streamReader);
                    }
                }
                return true;
            }
            catch { return false; }
        }
        private string GetOneSettingString(StreamReader streamReader) {
            string[] inputLine;
            string line = streamReader.ReadLine();
            if (line == null) return "";
            inputLine = line.Split(DELIMITER.ToCharArray());
            if (inputLine.GetLength(0) != 2) return "";
            return inputLine[1];
        }
        private bool GetOneSettingBool(StreamReader streamReader) {
            string[] inputLine;
            string line = streamReader.ReadLine();
            if (line == null) return false;
            inputLine = line.Split(DELIMITER.ToCharArray());
            if (inputLine.GetLength(0) != 2) return false;
            if (inputLine[1] == "true") return true;
            else return false;
        }

        private bool WriteIniFile() {
            if (this.iniFilePath == "") return true; //no warning, just no action
            try {
                //Note, if file doesn't exist it is created
                // The using statement also closes the StreamWriter.
                using (StreamWriter sw = new StreamWriter(this.iniFilePath)) {
                    sw.WriteLine("PathCsvFile" + DELIMITER + this.pathCsvFile);
                    if (checkBoxPreserveId.Checked) sw.WriteLine("PreserveId" + DELIMITER + "true");
                    else sw.WriteLine("PreserveId" + DELIMITER + "false");
                }
                return true;
            }
            catch { return false; }
        }

        // ********************************************************* Utility Functions *****************************************************
        private bool FindOrMakeUserFolder() {
            try {
                this.userFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), USER_FOLDER_HUMMINGBIRD);
                if (!Directory.Exists(this.userFolderPath)) {
                    try {
                        Directory.CreateDirectory(this.userFolderPath);
                    } catch {
                        this.userFolderPath = "";
                        return false;  // Returning silently.  this.UserFolderPath = "" is indication of failure
                    }
                }
                return true;
            } catch {
                // Returning silently for now
                return false;
            }
        }


        // *************************************************** Event Handlers ********************************************************
        private void buttonBrowse_Click(object sender, EventArgs e) {
            openFileDialog1.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
            if (File.Exists(textBoxPathCsv.Text))
                openFileDialog1.FileName = textBoxPathCsv.Text;
            else {
                openFileDialog1.FileName = "";
                string path = textBoxPathCsv.Text;
                int pos = path.LastIndexOf(@"\");
                if (pos > 1 && pos < path.Length - 2) {
                    path = textBoxPathCsv.Text.Substring(0, pos);
                    if (Directory.Exists(path))
                        openFileDialog1.InitialDirectory = path;
                }
            }
            System.Windows.Forms.DialogResult result = openFileDialog1.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.Cancel) {
                textBoxPathCsv.Text = openFileDialog1.FileName;
                this.pathCsvFile = textBoxPathCsv.Text;
                //comboBoxTabName.Text = "";
                //FillComboBox();
            }
        }

        private void buttonProcess_Click(object sender, EventArgs e) {
            Process();
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            this.pathCsvFile = textBoxPathCsv.Text;
            WriteIniFile();
            Close();
        }
    }
}
