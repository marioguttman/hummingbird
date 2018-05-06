using System;
using System.Windows.Forms;

using System.IO;

namespace HummingbirdCSExample {

    public partial class Menu : Form {

        // ************************************************* Module Variables *****************************************************
        private const string PROGRAM_NAME = "HummingbirdCSExample";
        private const string INI_FILE_NAME = "HummingbirdCSExample.ini";
        private const string DELIMITER = "\t";
        private const string USER_FOLDER_HUMMINGBIRD = "Hummingbird";   // Used to store all Hummingbird data

        private string userFolderPath = "";
        private string iniFilePath = "";

        private string csvFolderPath;

        // ********************************************************* Constructor ******************************************************
        public Menu() {
            InitializeComponent();
            FindOrMakeUserFolder();  // Not looking for error.  this.UserFolderPath = "" is indication of failure
            GetIniFilePath();
            ReadIniFile();
            textBoxPathFolder.Text = this.csvFolderPath;

            // Initialize text boxes if not set by .ini file.
            // Twisty Tower
            if (textBoxTwistFloors.Text == "" || textBoxTwistFloors.Text == null) textBoxTwistFloors.Text = "60";
            if (textBoxTwistHeight.Text == "" || textBoxTwistHeight.Text == null) textBoxTwistHeight.Text = "10.0";
            if (textBoxTwistTaper.Text == "" || textBoxTwistTaper.Text == null) textBoxTwistTaper.Text = "0.8";
            if (textBoxTwistTwist.Text == "" || textBoxTwistTwist.Text == null) textBoxTwistTwist.Text = "0.2";

            // Conceptual Mass
            if (textBoxConceptGridX.Text == "" || textBoxConceptGridX.Text == null) textBoxConceptGridX.Text = "10";
            if (textBoxConceptGridY.Text == "" || textBoxConceptGridY.Text == null) textBoxConceptGridY.Text = "10";
            if (textBoxConceptCellX.Text == "" || textBoxConceptCellX.Text == null) textBoxConceptCellX.Text = "100";
            if (textBoxConceptCellY.Text == "" || textBoxConceptCellY.Text == null) textBoxConceptCellY.Text = "100";
            if (textBoxConceptGrowX.Text == "" || textBoxConceptGrowX.Text == null) textBoxConceptGrowX.Text = "10";
            if (textBoxConceptGrowY.Text == "" || textBoxConceptGrowY.Text == null) textBoxConceptGrowY.Text = "10";
            if (textBoxConceptHeight.Text == "" || textBoxConceptHeight.Text == null) textBoxConceptHeight.Text = "100";

            // Spiral
            if (textBoxSpiralFactorA.Text == "" || textBoxSpiralFactorA.Text == null) textBoxSpiralFactorA.Text = "1.0";
            if (textBoxSpiralFactorB.Text == "" || textBoxSpiralFactorB.Text == null) textBoxSpiralFactorB.Text = "1.0";
            if (textBoxSpiralPoints.Text == "" || textBoxSpiralPoints.Text == null) textBoxSpiralPoints.Text = "10";

            // Site
            if (textBoxSiteSizeX.Text == "" || textBoxSiteSizeX.Text == null) textBoxSiteSizeX.Text = "200";
            if (textBoxSiteSizeY.Text == "" || textBoxSiteSizeY.Text == null) textBoxSiteSizeY.Text = "100";
            if (textBoxSiteDivisionsX.Text == "" || textBoxSiteDivisionsX.Text == null) textBoxSiteDivisionsX.Text = "20";
            if (textBoxSiteDivisionsY.Text == "" || textBoxSiteDivisionsY.Text == null) textBoxSiteDivisionsY.Text = "10";
            if (textBoxSiteElevTopLeft.Text == "" || textBoxSiteElevTopLeft.Text == null) textBoxSiteElevTopLeft.Text = "0.0";
            if (textBoxSiteElevBotLeft.Text == "" || textBoxSiteElevBotLeft.Text == null) textBoxSiteElevBotLeft.Text = "-20.0";
            if (textBoxSiteElevTopRight.Text == "" || textBoxSiteElevTopRight.Text == null) textBoxSiteElevTopRight.Text = "10.0";
            if (textBoxSiteElevBotRight.Text == "" || textBoxSiteElevBotRight.Text == null) textBoxSiteElevBotRight.Text = "30.0";
            if (textBoxSiteRandom.Text == "" || textBoxSiteRandom.Text == null) textBoxSiteRandom.Text = "10";
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
                        this.csvFolderPath = GetOneSettingString(streamReader);
                        checkBoxPreserveId.Checked = GetOneSettingBool(streamReader);

                        // TwistyTower
                        textBoxTwistFloors.Text = GetOneSettingString(streamReader);
                        textBoxTwistHeight.Text = GetOneSettingString(streamReader);
                        textBoxTwistTaper.Text = GetOneSettingString(streamReader);
                        textBoxTwistTwist.Text = GetOneSettingString(streamReader);

                        // ConceptModel
                        textBoxConceptGridX.Text = GetOneSettingString(streamReader);
                        textBoxConceptGridY.Text = GetOneSettingString(streamReader);
                        textBoxConceptCellX.Text = GetOneSettingString(streamReader);
                        textBoxConceptCellY.Text = GetOneSettingString(streamReader);
                        textBoxConceptGrowX.Text = GetOneSettingString(streamReader);
                        textBoxConceptGrowY.Text = GetOneSettingString(streamReader);
                        textBoxConceptHeight.Text = GetOneSettingString(streamReader);

                        // Spiral
                        textBoxSpiralFactorA.Text = GetOneSettingString(streamReader);
                        textBoxSpiralFactorB.Text = GetOneSettingString(streamReader);
                        textBoxSpiralPoints.Text = GetOneSettingString(streamReader);

                        // Site
                        textBoxSiteSizeX.Text = GetOneSettingString(streamReader);
                        textBoxSiteSizeY.Text = GetOneSettingString(streamReader);
                        textBoxSiteDivisionsX.Text = GetOneSettingString(streamReader);
                        textBoxSiteDivisionsY.Text = GetOneSettingString(streamReader);
                        textBoxSiteElevTopLeft.Text = GetOneSettingString(streamReader);
                        textBoxSiteElevBotLeft.Text = GetOneSettingString(streamReader);
                        textBoxSiteElevTopRight.Text = GetOneSettingString(streamReader);
                        textBoxSiteElevBotRight.Text = GetOneSettingString(streamReader);
                        textBoxSiteRandom.Text = GetOneSettingString(streamReader);
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
                    sw.WriteLine("textBoxCsvFilePath" + DELIMITER + this.csvFolderPath);
                    if (checkBoxPreserveId.Checked) sw.WriteLine("checkBoxPreserveId" + DELIMITER + "true");
                    else sw.WriteLine("checkBoxPreserveId" + DELIMITER + "false");

                    // TwistyTower
                    sw.WriteLine("textBoxTwistFloors" + DELIMITER + textBoxTwistFloors.Text);
                    sw.WriteLine("textBoxTwistHeight" + DELIMITER + textBoxTwistHeight.Text);
                    sw.WriteLine("textBoxTwistTaper" + DELIMITER + textBoxTwistTaper.Text);
                    sw.WriteLine("textBoxTwistTwist" + DELIMITER + textBoxTwistTwist.Text);

                    // ConceptModel
                    sw.WriteLine("textBoxConceptGridX" + DELIMITER + textBoxConceptGridX.Text);
                    sw.WriteLine("textBoxConceptGridY" + DELIMITER + textBoxConceptGridY.Text);
                    sw.WriteLine("textBoxConceptCellX" + DELIMITER + textBoxConceptCellX.Text);
                    sw.WriteLine("textBoxConceptCellY" + DELIMITER + textBoxConceptCellY.Text);
                    sw.WriteLine("textBoxConceptGrowX" + DELIMITER + textBoxConceptGrowX.Text);
                    sw.WriteLine("textBoxConceptGrowY" + DELIMITER + textBoxConceptGrowY.Text);
                    sw.WriteLine("textBoxConceptHeight" + DELIMITER + textBoxConceptHeight.Text);

                    // Spiral
                    sw.WriteLine("textBoxSpiralFactorA" + DELIMITER + textBoxSpiralFactorA.Text);
                    sw.WriteLine("textBoxSpiralFactorB" + DELIMITER + textBoxSpiralFactorB.Text);
                    sw.WriteLine("textBoxSpiralPoints" + DELIMITER + textBoxSpiralPoints.Text);

                    // Site
                    sw.WriteLine("textBoxSiteSizeX" + DELIMITER + textBoxSiteSizeX.Text);
                    sw.WriteLine("textBoxSiteSizeY" + DELIMITER + textBoxSiteSizeY.Text);
                    sw.WriteLine("textBoxSiteDivisionsX" + DELIMITER + textBoxSiteDivisionsX.Text);
                    sw.WriteLine("textBoxSiteDivisionsY" + DELIMITER + textBoxSiteDivisionsY.Text);
                    sw.WriteLine("textBoxSiteElevTopLeft" + DELIMITER + textBoxSiteElevTopLeft.Text);
                    sw.WriteLine("textBoxSiteElevBotLeft" + DELIMITER + textBoxSiteElevBotLeft.Text);
                    sw.WriteLine("textBoxSiteElevTopRight" + DELIMITER + textBoxSiteElevTopRight.Text);
                    sw.WriteLine("textBoxSiteElevBotRight" + DELIMITER + textBoxSiteElevBotRight.Text);
                    sw.WriteLine("textBoxSiteRandom" + DELIMITER + textBoxSiteRandom.Text);

                }
                return true;
            }
            catch { return false; }
        }


        // ********************************************************* Utility Functions *****************************************************

        private bool ItitializeCsvSettings() {
            textBoxPathFolder.Text = textBoxPathFolder.Text.Trim();
            if (textBoxPathFolder.Text == "") {
                MessageBox.Show("Path to folder must be specified.");
                this.csvFolderPath = null;
                return false;
            }
            if (!textBoxPathFolder.Text.EndsWith(@"\")) textBoxPathFolder.Text = textBoxPathFolder.Text + @"\";
            this.csvFolderPath = textBoxPathFolder.Text;            
            return true;
        }

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

        // ********************************************* Event Handlers ******************************************
        private void buttonClose_Click(object sender, EventArgs e) {
            GC.Collect();
            WriteIniFile();
            Close();
        }

        private void buttonBrowse_Click(object sender, EventArgs e) {
            System.Windows.Forms.DialogResult dialogResult;
            string path = textBoxPathFolder.Text;
            if (path.EndsWith("\\")) path = path.Substring(0, path.Length - 1);  //In case user added a "\"
            if (Directory.Exists(path)) folderBrowserDialog1.SelectedPath = path;
            else folderBrowserDialog1.SelectedPath = "C:\\";
            dialogResult = folderBrowserDialog1.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK) {
                path = folderBrowserDialog1.SelectedPath.ToString();
                if (Directory.Exists(path)) {
                    textBoxPathFolder.Text = path;
                    if (!textBoxPathFolder.Text.EndsWith(@"\")) textBoxPathFolder.Text = textBoxPathFolder.Text + @"\";
                }
                else textBoxPathFolder.Text = "";
                this.csvFolderPath = textBoxPathFolder.Text;
            }
        }

        private void buttonTwistyTower_Click(object sender, EventArgs e) {
            if (!ItitializeCsvSettings()) return;
            Cursor.Current = Cursors.WaitCursor;
            TwistyTower twistyTower = new TwistyTower(this.csvFolderPath, checkBoxPreserveId.Checked);
            twistyTower.NumberFloors = Convert.ToInt16(textBoxTwistFloors.Text);
            twistyTower.FloorHeight = Convert.ToDouble(textBoxTwistHeight.Text);
            twistyTower.TaperFactor = Convert.ToDouble(textBoxTwistTaper.Text);
            twistyTower.TwistFactor = Convert.ToDouble(textBoxTwistTwist.Text);
            twistyTower.CreateStructure();
            twistyTower.CreateFloors();
            twistyTower.CreateWalls();
            twistyTower.CreateAdaptiveComponents();
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Process Completed", PROGRAM_NAME);
        }

        private void buttonHoles_Click(object sender, EventArgs e) {
            if (!ItitializeCsvSettings()) return;
            Cursor.Current = Cursors.WaitCursor;
            HolesTower holesTower = new HolesTower(this.csvFolderPath, checkBoxPreserveId.Checked);
            holesTower.CreateFloors();
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Process Completed", PROGRAM_NAME);
        }

        private void buttonConceptMass_Click(object sender, EventArgs e) {
            if (!ItitializeCsvSettings()) return;
            Cursor.Current = Cursors.WaitCursor;
            ConceptModel conceptModel = new ConceptModel(this.csvFolderPath, checkBoxPreserveId.Checked);
            conceptModel.GridSizeX = Convert.ToInt16(textBoxConceptGridX.Text);
            conceptModel.GridSizeY = Convert.ToInt16(textBoxConceptGridY.Text);
            conceptModel.CellSizeX = Convert.ToDouble(textBoxConceptCellX.Text);
            conceptModel.CellSizeY = Convert.ToDouble(textBoxConceptCellY.Text);
            conceptModel.FactorGrowX = Convert.ToDouble(textBoxConceptGrowX.Text);
            conceptModel.FactorGrowY = Convert.ToDouble(textBoxConceptGrowY.Text);
            conceptModel.FactorHeight = Convert.ToDouble(textBoxConceptHeight.Text);
            conceptModel.CreatePoints();
            conceptModel.CreateCurveByPoints();
            conceptModel.CreateLoftForm();
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Process Completed", PROGRAM_NAME);
        }

        private void buttonSpiral_Click(object sender, EventArgs e) {
            if (!ItitializeCsvSettings()) return;
            Cursor.Current = Cursors.WaitCursor;
            SpiralForm spiralForm = new SpiralForm(this.csvFolderPath, checkBoxPreserveId.Checked);
            spiralForm.FactorA = Convert.ToDouble(textBoxSpiralFactorA.Text);
            spiralForm.FactorB = Convert.ToDouble(textBoxSpiralFactorB.Text);
            spiralForm.NumberOfPoints = Convert.ToInt32(textBoxSpiralPoints.Text);
            spiralForm.CreateGrids();
            spiralForm.CreateLines();
            spiralForm.CreateArcs();
            spiralForm.CreateWalls();
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Process Completed", PROGRAM_NAME);
        }

        private void buttonSite_Click(object sender, EventArgs e) {
            if (!ItitializeCsvSettings()) return;
            Cursor.Current = Cursors.WaitCursor;
            Site site = new Site(this.csvFolderPath, checkBoxPreserveId.Checked);
            site.SizeX = Convert.ToDouble(textBoxSiteSizeX.Text);
            site.SizeY = Convert.ToDouble(textBoxSiteSizeY.Text);
            site.DivisionsX = Convert.ToInt32(textBoxSiteDivisionsX.Text);
            site.DivisionsY = Convert.ToInt32(textBoxSiteDivisionsY.Text);
            site.ElevTopLeft = Convert.ToDouble(textBoxSiteElevTopLeft.Text);
            site.ElevBotLeft = Convert.ToDouble(textBoxSiteElevBotLeft.Text);
            site.ElevTopRight = Convert.ToDouble(textBoxSiteElevTopRight.Text);
            site.ElevBotRight = Convert.ToDouble(textBoxSiteElevBotRight.Text);
            site.RandomFactor = Convert.ToDouble(textBoxSiteRandom.Text);
            site.CreateTopographySurface();
            site.CreateFamilyExtrusions();
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Process Completed", PROGRAM_NAME);
        }

    }

}
