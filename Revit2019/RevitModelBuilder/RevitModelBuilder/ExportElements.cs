
using RevitUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;  //For File

using Autodesk.Revit.DB;
using HummingbirdUtility;

namespace RevitModelBuilder {
    public partial class ExportElements : System.Windows.Forms.Form {

        #region Module Varaibles                        // ****************************** Module Variables ******************************************

        private UtilitySettings settings;
        private UtilityElements elements;
        private RevitTypeNames revitTypes;

        private string pathCsvFolder;
        private string csvFileName;
        private ICollection<ElementId> currentSelectionSet;

        private string errorMessageBuild;
        private int errorMessageCount;
        private int maxErrorsToShow = 10;

        #endregion

        #region Constructor and Form Closed Event       // ****************************** Constructor and Form Closed Event *************************
        // 2015 Deprecated
        //public ExportElements(UtilitySettings utilitySettings, UtilityElements utilityElements, SelElementSet currentSelectionSet) {
        public ExportElements(UtilitySettings utilitySettings, UtilityElements utilityElements, ICollection<ElementId> currentSelectionSet) {
            InitializeComponent();
            this.settings = utilitySettings;
            this.elements = utilityElements;
            this.currentSelectionSet = currentSelectionSet;

            this.revitTypes = new RevitTypeNames();

            this.settings.SetFormControls(this);
            textBoxPathCsvFolder.Text = textBoxPathCsvFolder.Text.Trim();
            if (textBoxPathCsvFolder.Text.EndsWith(@"\")) textBoxPathCsvFolder.Text = textBoxPathCsvFolder.Text.Substring(0, textBoxPathCsvFolder.Text.Length - 1);
            this.pathCsvFolder = textBoxPathCsvFolder.Text;
            textBoxCsvFileName.Text = textBoxCsvFileName.Text.Trim();
            if (!textBoxCsvFileName.Text.ToLower().EndsWith(".csv")) textBoxCsvFileName.Text = textBoxCsvFileName.Text + ".csv";
            this.csvFileName = textBoxCsvFileName.Text;

        }

        private void ExportElements_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e) {
            this.settings.SaveFormControls(this);
            this.settings.WriteIniFile();
            GC.Collect();
        }

        #endregion

        #region Private Functions                       // ****************************** Private Functions *****************************************        

        private bool ProcessRecords() {

            this.errorMessageBuild = "One or more errors encountered:";
            this.errorMessageCount = 0;

            // Validation
            if (!Directory.Exists(this.pathCsvFolder)) {
                System.Windows.Forms.MessageBox.Show("Folder for CSV files does not exist.  Halting process.", this.settings.ProgramName);
                return false;
            }
            string pathCsvFile = this.pathCsvFolder + @"\" + this.csvFileName;  // Trailing "\" has already been removed from this.pathCsvFolder; ".csv" extension has been added to this.csvFileName.
            try {
                    this.maxErrorsToShow = Convert.ToInt32(textBoxMaxErrors.Text);
            }
            catch {
                this.maxErrorsToShow = 10;
                textBoxMaxErrors.Text = "10";
            }

            // Selection mode for elements.
            string selectionMode;
            if (radioButtonSelectCurrent.Checked) selectionMode = "current";
            else if (radioButtonSelectView.Checked) selectionMode = "view";
            else selectionMode = "all";

            // Rounding value
            int precision = 2;
            if (checkBoxRoundPoints.Checked) {                
                try { precision = Convert.ToInt16(comboBoxDecimals.Text); }
                catch { } //Just leave at default value
            }
            else precision = -1;  // -1 value indicates no rounding

            // Prepare to create Set statemtents by blanking all of the settings so first one gets a Set statement
            this.elements.InitializeSettings();

            ProgressBar progressBarForm = null;

            try {

                // Start CsvWriter and create file.
                CsvWriter csvWriter = new CsvWriter();
                string returnValue;
                returnValue = csvWriter.ConnectToFile(pathCsvFile);
                if (returnValue != "") {
                    System.Windows.Forms.MessageBox.Show(
                        "Error returned from 'CsvWriter.ConnectToFile()'.  Halting process.\n\n"
                      + "Internal message: " + returnValue, this.settings.ProgramName);
                    return false;
                }

                DataTable dataTable = csvWriter.DataTable;

                // Setup columns and headings
                int row = 1;                  

                // Get the Revit selection                
                ElementSet elementsToProcess = null;
                if (selectionMode == "current") {
                    // 2015 Deprecated
                    elementsToProcess = new ElementSet();
                    foreach(ElementId elementId in this.currentSelectionSet) {
                        elementsToProcess.Insert(this.settings.DocumentDb.GetElement(elementId));
                    }
                    //elementsToProcess = this.currentSelectionSet;  // Saved from initial startup since the process of finding adaptive components loses the selection
                }
                else {
                    FilteredElementCollector collector;
                    if (selectionMode == "view") {
                        collector = new FilteredElementCollector(this.settings.DocumentDb, this.settings.ActiveView.Id);
                    }
                    else {  //"all" case
                        collector = new FilteredElementCollector(this.settings.DocumentDb);
                    }
                    ICollection<Element> elementsCollection = collector.WhereElementIsNotElementType().ToElements();
                    elementsToProcess = new ElementSet();
                    foreach (Element element in elementsCollection) {
                        elementsToProcess.Insert(element);
                    }
                }

                // Process the selection
                if (elementsToProcess.Size == 0) {
                    System.Windows.Forms.MessageBox.Show("No elements selected.  Stopping process.", this.settings.ProgramName);
                    return false;
                }
                row = 2;  //Since headings was 1
                int itemCount = 0;  //Used to develop rowId value
                ConversionMode conversionMode;
                if (radioButtonConvertNone.Checked) conversionMode = ConversionMode.None;
                else if (radioButtonConvertDetailLines.Checked) conversionMode = ConversionMode.DetailLines;
                else if (radioButtonConvertModelLines.Checked) conversionMode = ConversionMode.ModelLines;
                else conversionMode = ConversionMode.FilledRegions;  // radioButtonConvertFilledRegions.Checked case

                //Start the progress bar
                int numberOfElements = elementsToProcess.Size;
                progressBarForm = new ProgressBar("Processing " + numberOfElements.ToString() + " elements.", numberOfElements + 1);
                progressBarForm.SetLabel("");
                progressBarForm.Show();
                progressBarForm.Increment();  //To avoid transparent look while waiting
                //int currentElement = 1;

                // Process the families
                bool userCancel = false;
                foreach (Element element in elementsToProcess) {
                    string rowIdRoot = itemCount.ToString();
                    OutputGroup outputGroup = new OutputGroup(rowIdRoot, element, precision, conversionMode, this.settings, this.elements);
                    if (!outputGroup.Valid) {
                        ProcessError("Unable to export object: " + outputGroup.ErrorMessage);
                        continue;
                    }
                    foreach (OutputItem outputItem in outputGroup) {
                        outputItem.AddDataRow(dataTable);
                        
                        System.Windows.Forms.Application.DoEvents(); //Clunky (but effective) way to allow the user to cancel the loop
                        if (progressBarForm.Cancel == true) {
                            userCancel = true;
                            break;
                        }
                        progressBarForm.SetValue(itemCount + 1);
                        progressBarForm.SetLabel(
                              "Processing item " + (itemCount + 1).ToString() + " of " + numberOfElements.ToString() + ": "
                            + outputItem.RowId + " - " + outputItem.Action + " - " + outputItem.Object);
                        progressBarForm.Refresh();
                        
                        row++;
                    }
                    itemCount++;
                    if (userCancel) break;
                }

                // Write the csv file
                returnValue = csvWriter.WriteFile();
                if (returnValue != "") {
                    System.Windows.Forms.MessageBox.Show(
                        "Error returned from 'CsvWriter.WriteFile()'.  Halting process.\n\n"
                      + "Internal message: " + returnValue, this.settings.ProgramName);
                    if (!(progressBarForm == null)) progressBarForm.Close();
                    return false;
                }
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show("Error in 'ExcelToElements.ProcessRecords'.\nSystem message: " + exception.Message, this.settings.ProgramName);
                return false;
            }

            finally {
                if (!(progressBarForm == null)) progressBarForm.Close();
                if ((this.errorMessageCount > 0 && checkBoxListErrors.Checked)) {
                    DisplayErrors displayErrorsDialog = new DisplayErrors(this.errorMessageBuild);
                    displayErrorsDialog.ShowDialog();
                }                    
            } 
            
        }

        #endregion

        #region Utility Functions                       // ****************************** Utility Functions *****************************************
        private void ProcessError(string errorText) {
            if (this.errorMessageCount < this.maxErrorsToShow) this.errorMessageBuild = this.errorMessageBuild  + "\r\n\r\n" + errorText;
            this.errorMessageCount++;
            return;
        }

        #endregion

        #region Event Handlers                          // ****************************** Event Handlers ********************************************

        private void buttonProcess_Click(object sender, EventArgs e) {
            ProcessRecords();
            if (this.checkBoxAutoClose.Checked) Close();
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void buttonBrowseCsvFolder_Click(object sender, EventArgs e) {
            System.Windows.Forms.DialogResult dialogResult;
            string path = textBoxPathCsvFolder.Text;
            if (path.EndsWith(@"\")) path = path.Substring(0, path.Length - 1);  //In case user added a "\"
            if (Directory.Exists(path)) folderBrowserDialog.SelectedPath = path;
            else folderBrowserDialog.SelectedPath = @"C:\";
            dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK) {
                path = folderBrowserDialog.SelectedPath.ToString();
                if (Directory.Exists(path)) textBoxPathCsvFolder.Text = path;
                else textBoxPathCsvFolder.Text = "";
                this.pathCsvFolder = textBoxPathCsvFolder.Text;
            }
        }

        private void textBoxPathCsvFolder_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
            if (e.KeyChar.ToString() == "\r") textBoxPathCsvFolder_Leave(sender, e);
        }

        private void textBoxPathCsvFolder_Leave(object sender, EventArgs e) {
            textBoxPathCsvFolder.Text = textBoxPathCsvFolder.Text.Trim();
            if (textBoxPathCsvFolder.Text.EndsWith(@"\")) textBoxPathCsvFolder.Text = textBoxPathCsvFolder.Text.Substring(0, textBoxPathCsvFolder.Text.Length - 1);
            this.pathCsvFolder = textBoxPathCsvFolder.Text;
        }

        private void textBoxCsvFileName_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
            if (e.KeyChar.ToString() == "\r") textBoxCsvFileName_Leave(sender, e);
        }

        private void textBoxCsvFileName_Leave(object sender, EventArgs e) {
            textBoxCsvFileName.Text = textBoxCsvFileName.Text.Trim();
            if (!textBoxCsvFileName.Text.ToLower().EndsWith(".csv")) textBoxCsvFileName.Text = textBoxCsvFileName.Text + ".csv";
            this.csvFileName = textBoxCsvFileName.Text;
        }

        #endregion
    }
}
