
using HummingbirdUtility;
using RevitUtility;
using System;
using System.Collections.Generic;
using System.Data;

using System.Globalization;
using System.IO;  //For File
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace RevitModelBuilder {
    public partial class CreateElements : System.Windows.Forms.Form {

        #region Module Variables                        // ****************************** Module Variables *****************************************************                
        private const string UNIQUE_FAMILY_NAME_SEED = "ModelBuilder";
        
        private UtilitySettings settings;
        private UtilityElements elements;
        private SortedDictionary<string, string> filePaths;
        private string pathFolder;

        private string errorMessageBuild;
        private int errorMessageCount;
        private int maxErrorsToShow = 10;
        
        private InputItem currentInputItem;
        private Element lastElementAdded;

        private List<string> existingFamilySymbols = null;  // We will only fill this if we need to

        private int indexCurrentItem; // global so we can report in exception handler.

        private double unitsFactor = 1.0;  // Default value is just for compiler

        private NumberFormatInfo provider = new NumberFormatInfo();

        #endregion

        #region Constructor and Form Load/Close Events  // ****************************** Constructor and Form Load/Close Events **********************************************

        public CreateElements(UtilitySettings utilitySettings, UtilityElements utilityElements) {
            InitializeComponent();
            this.settings = utilitySettings;
            this.elements = utilityElements;

            this.settings.SetFormControls(this);
            textBoxPathFolder.Text = textBoxPathFolder.Text.Trim();
            this.pathFolder = textBoxPathFolder.Text;

            this.provider.NumberDecimalSeparator = ".";
        }
        private void CreateElements_Load(object sender, EventArgs e) {
            // Fill the .csv file list box
            FillListBox();

            // Fill and restore the family combo boxes
            InitializeComboBoxes();

            // Set default values
            SetDefaultsGeneral();
            SetDefaultsColumn();
            SetDefaultsBeam();
            SetDefaultsAdaptiveComponent();
            SetDefaultsMullion();

            // Save the path to the temporary folder to settings for use with CreateElements
            this.settings.TempFilePath = textBoxTempFolder.Text;
        }
        private void CreateElements_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e) {
            this.settings.SaveFormControls(this);
            this.settings.WriteIniFile();
            GC.Collect();
        }
        #endregion

        #region Private Functions                       // ****************************** Private Functions ********************************************************

        private bool FillListBox() {

            listBoxCsvFiles.Items.Clear();
            filePaths = new SortedDictionary<string,string>();

            // If folder path hasn't been selected yet just return.
            if (this.pathFolder == "") return true;
            if (!Directory.Exists(this.pathFolder)) return true;

            string[] files = Directory.GetFiles(this.pathFolder, "*.csv");
            IEnumerable<string> sortedFilePaths = files.OrderBy(s => s);    // Can also do this returning a "var" value
            foreach (string pathFile in sortedFilePaths) {
                string fileName = Path.GetFileName(pathFile);
                filePaths.Add(fileName, pathFile);
                listBoxCsvFiles.Items.Add(fileName);
            }
            if (listBoxCsvFiles.Items.Count > 0) listBoxCsvFiles.SelectedIndex = 0;

            return true;
        }

        private bool ProcessRecords() {

            this.errorMessageBuild = "One or more errors encountered:";
            this.errorMessageCount = 0;

            ProgressBar progressBarForm = null;

            try {
                // Reused
                string returnValue;

                // Establish mode
                string mode = "";
                if (radioButtonModeAdd.Checked) mode = "Add";      // Add all new elements.  Do not delete any existing elements.
                else {
                    if (radioButtonModeKeep.Checked) mode = "Keep"; // Keep elements with matching ElementID values; or make new element.
                    else mode = "Delete";                          // Delete elements with matching ElementID values and make all new elements.
                }

                // Setup for error trapping
                this.elements.TrapErrors = checkBoxSuppressMessages.Checked;

                //Validate Input
                string pathCsvFile;
                listBoxCsvFiles.Text = listBoxCsvFiles.Text.Trim();
                if (listBoxCsvFiles.Text == null || listBoxCsvFiles.Text == "") {
                    System.Windows.Forms.MessageBox.Show("File must be selected before running command.", this.settings.ProgramName);
                    pathCsvFile = "";
                    return false;
                }
                else pathCsvFile = filePaths[listBoxCsvFiles.Text];

                try {
                    this.maxErrorsToShow = Convert.ToInt32(textBoxMaxErrors.Text);
                }
                catch {
                    this.maxErrorsToShow = 10;
                    textBoxMaxErrors.Text = "10";
                }

                // Set up units
                if (radioButtonUnitsProject.Checked) {
                    Units units = this.settings.DocumentDb.GetUnits();
                    // 2015 Deprecated
                    //DisplayUnitType displayUnitType = units.GetDisplayUnitType();
                    DisplayUnitType displayUnitType = units.GetFormatOptions(UnitType.UT_Length).DisplayUnits;                    
                    //Decimal Feet                     DisplayUnitType.DUT_DECIMAL_FEET;
                    //Feet and fractional inches       DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES
                    //Decimal inches                   DisplayUnitType.DUT_DECIMAL_INCHES
                    //Fractional inches                DisplayUnitType.DUT_FRACTIONAL_INCHES
                    //Meters                           DisplayUnitType.DUT_METERS
                    //Decimeters                       DisplayUnitType.DUT_DECIMETERS
                    //Centimeters                      DisplayUnitType.DUT_CENTIMETERS
                    //Millimeters                      DisplayUnitType.DUT_MILLIMETERS
                    //Meters and Centimeters  (Meters) DisplayUnitType.DUT_METERS_CENTIMETERS
                    switch (displayUnitType) {
                        case DisplayUnitType.DUT_DECIMAL_FEET:
                        case DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES:  // Uses Length Feet
                            this.unitsFactor = 1.0;
                            break;
                        case DisplayUnitType.DUT_DECIMAL_INCHES:
                        case DisplayUnitType.DUT_FRACTIONAL_INCHES:
                            this.unitsFactor = 0.083333333;
                            break;
                        case DisplayUnitType.DUT_METERS:
                        case DisplayUnitType.DUT_METERS_CENTIMETERS:  // Uses Length Meters
                            this.unitsFactor = 3.28083989501;
                            break;
                        case DisplayUnitType.DUT_DECIMETERS:
                            this.unitsFactor = 0.328083989501;
                            break;
                        case DisplayUnitType.DUT_CENTIMETERS:
                            this.unitsFactor = 0.0328083989501;
                            break;
                        case DisplayUnitType.DUT_MILLIMETERS:
                            this.unitsFactor = 0.00328083989501;
                            break;
                        default:
                            System.Windows.Forms.MessageBox.Show("Unknown DisplayUnitType value: '" + displayUnitType.ToString() + "'.",   this.settings.ProgramName);
                            break;
                    }
                }
                else {
                    if (textBoxUnitsFactor.Text == "" || textBoxUnitsFactor.Text == null) textBoxUnitsFactor.Text = "1.0";
                    
                    try {
                        this.unitsFactor = Convert.ToDouble(textBoxUnitsFactor.Text, this.provider);
                    }
                    catch {
                        textBoxUnitsFactor.Text = "1.0";
                        this.unitsFactor = 1.0;
                    }
                    if (this.unitsFactor <= 0) {
                        System.Windows.Forms.MessageBox.Show("Units Factor must be greater than zero.", this.settings.ProgramName);
                        return false;
                    }
                }

                // Set up origin translation
                if (textBoxOriginX.Text == "" || textBoxOriginX.Text == null) textBoxOriginX.Text = "0";
                if (textBoxOriginY.Text == "" || textBoxOriginY.Text == null) textBoxOriginY.Text = "0";
                if (textBoxOriginZ.Text == "" || textBoxOriginZ.Text == null) textBoxOriginZ.Text = "0";
                XYZ translation = null;
                // This version is designed to handle cultures where decimal point is not a dot.
                double originX, originY, originZ;
                bool badOriginData = false;
                if (!double.TryParse(textBoxOriginX.Text, out originX)) {
                    textBoxOriginX.Text = "0";
                    originX = 0.0;
                    badOriginData = true;
                }
                if (!double.TryParse(textBoxOriginY.Text, out originY)) {
                    textBoxOriginY.Text = "0";
                    originY = 0.0;
                    badOriginData = true;
                }
                if (!double.TryParse(textBoxOriginZ.Text, out originZ)) {
                    textBoxOriginZ.Text = "0";
                    originZ = 0.0;
                    badOriginData = true;
                }
                if (badOriginData) {
                    System.Windows.Forms.MessageBox.Show("One or more 'Placement Offset' values was invalid.  Zero value sustituted.", this.settings.ProgramName);
                    // Note that we don't stop processing; just warn the user.
                }
                translation = new XYZ(originX, originY, originZ);
                //try {
                //    translation = new XYZ(Convert.ToDouble(textBoxOriginX.Text), Convert.ToDouble(textBoxOriginY.Text), Convert.ToDouble(textBoxOriginZ.Text));
                //    // We now require a translation value in every case because of the way we are processing units.
                //    //if (translation.X == 0 && translation.Y == 0 && translation.Z == 0) translation = null;  // signals InputItem not to do any translation.                    
                //}
                //catch {
                //    System.Windows.Forms.MessageBox.Show("Translate Origin values must be blank or numerical.", this.utilitySettings.ProgramName);
                //    return false;
                //}
                
                // Determine if unit conversion or translation is needed
                bool unitsOrTranslationNeeded = true;
                if (translation.X == 0.0 && translation.Y == 0.0 && translation.Z == 0.0 && this.unitsFactor == 1.0) unitsOrTranslationNeeded = false;

                // Connect to the .csv file
                CsvReader csvReader = new CsvReader();                
                returnValue = csvReader.ConnectToFile(pathCsvFile);
                if (returnValue != "") {
                    System.Windows.Forms.MessageBox.Show(
                          "Unable to connect to file with path: \n" + pathCsvFile + ".\n\n"
                        + returnValue + "\n\n"
                        + "Stopping process.",
                        this.settings.ProgramName, System.Windows.Forms.MessageBoxButtons.OKCancel);
                    return false;
                }
                returnValue = csvReader.ReadFile();
                if (returnValue != "") {
                    System.Windows.Forms.MessageBox.Show(
                          "Unable to read file : " + returnValue + "\n\n"
                        + "Stopping process.",
                        this.settings.ProgramName, System.Windows.Forms.MessageBoxButtons.OKCancel);
                    return false;
                }

                // Check syntax
                returnValue = csvReader.CheckSyntax();
                if (returnValue != "") {
                    System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                          "Syntax error detected in file: " + returnValue + "\n\n"
                        + "Select 'OK' to continue or 'Cancel' to stop processing.", 
                        this.settings.ProgramName, System.Windows.Forms.MessageBoxButtons.OKCancel);
                    if (dialogResult != System.Windows.Forms.DialogResult.OK) return false;
                }

                // Get the pre-parsed data
                returnValue = csvReader.GetInput();

                //Start the progress bar
                int numberOfElements = csvReader.HbInputItems.Count;
                progressBarForm = new ProgressBar("Processing " + numberOfElements.ToString() + " elements.", numberOfElements);
                progressBarForm.SetLabel("");
                progressBarForm.Show();
                progressBarForm.Increment();  //To avoid transparent look while waiting

                this.lastElementAdded = null;

                this.indexCurrentItem = 0;
                foreach (HbInputItem hbInputItem in csvReader.HbInputItems) {
                    this.currentInputItem = new InputItem(hbInputItem, this.unitsFactor, translation, unitsOrTranslationNeeded);
                    if (this.currentInputItem.ErrorMessage != "") {
                        ProcessError(this.currentInputItem.ErrorMessage); 
                        goto ContinueLoop;
                    }
                    string string0 = this.currentInputItem.String0;
                    string string1 = this.currentInputItem.String1;
                    string string2 = this.currentInputItem.String2;
                    double double0 = this.currentInputItem.Double0;
                    bool bool0 = this.currentInputItem.Bool0;
                    bool bool1 = this.currentInputItem.Bool1;
                    switch (hbInputItem.CommandAction) {
                        case "Set":
                            switch (this.currentInputItem.HbInputItem.CommandObject) {
                                case "Level":                 if (!this.elements.SetLevel(                 string0))                    ProcessError("Invalid 'Level' value"); break;
                                case "WallType":              if (!this.elements.SetWallType(              string0))                    ProcessError("Invalid 'WallType' value"); break;
                                case "WallHeight":            if (!this.elements.SetWallHeight(            double0 * this.unitsFactor)) ProcessError("Invalid 'WallHeight' value"); break;
                                case "FloorType":             if (!this.elements.SetFloorType(             string0))                    ProcessError("Invalid 'FloorType' value"); break;
                                case "FamilyType":            if (!this.elements.SetFamilyType(            string0, string1))           ProcessError("Invalid 'Family Type' values"); break;
                                case "FamilyRotation":        if (!this.elements.SetFamilyRotation(        double0))                    ProcessError("Invalid 'Family Rotatation' value"); break;
                                case "FamilyMirrored":        if (!this.elements.SetFamilyMirrored(        bool0, bool1))               ProcessError("Invalid 'Family Mirror' values"); break;
                                case "FamilyFlipped":         if (!this.elements.SetFamilyFlipped(         bool0, bool1))               ProcessError("Invalid 'Family Mirror' values"); break;
                                case "ColumnMode":            if (!this.elements.SetColumnMode(            string0, string1, string2))  ProcessError("Invalid 'Column Mode' values"); break;
                                case "ColumnHeight":          if (!this.elements.SetColumnHeight(          double0 * this.unitsFactor)) ProcessError("Invalid 'Column Height' value"); break;
                                case "ColumnRotation":        if (!this.elements.SetColumnRotation(        double0))                    ProcessError("Invalid 'Column Rotation' value"); break;
                                case "BeamType":              if (!this.elements.SetBeamType(              string0, string1))           ProcessError("Invalid 'Beam Type' values"); break;
                                case "BeamRotation":          if (!this.elements.SetBeamRotation(          double0))                    ProcessError("Invalid 'Beam Rotation' value"); break;
                                case "BeamJustification":     if (!this.elements.SetBeamJustification(     string0))                    ProcessError("Invalid 'Beam Justification' value"); break;
                                case "AdaptiveComponentType": if (!this.elements.SetAdaptiveComponentType( string0, string1))           ProcessError("Invalid 'AdaptiveComponent' Family/Type value"); break;
                                case "FamilyExtrusionHeight": if (!this.elements.SetFamilyExtrusionHeight( double0 * unitsFactor))      ProcessError("Invalid 'FamilyExtrusionHeight' value"); break;
                                case "MullionType":           if (!this.elements.SetMullionType(           string0, string1))           ProcessError("Invalid 'Mullion' Family/Type value"); break;
                                case "FilledRegionType":      if (!this.elements.SetFilledRegionType(string0))                          ProcessError("Invalid 'Filled Region' Type value"); break;
                                default: ProcessError("Invalid 'hbInputItem.CommandAction' switch case:" + hbInputItem.CommandAction + " with 'Set'."); break;
                            }
                            break;
                        case "Add":
                            //if ( mode ==  "Add") {}      // Add all new elements.  Do not delete any existing elements.
                            if (mode == "Keep" || mode == "Delete") {
                                if (this.currentInputItem.HbInputItem.ElementId != null) {
                                    ElementId elementId = new ElementId(this.currentInputItem.HbInputItem.ElementId.ElementIdValue);
                                    Element element = this.settings.DocumentDb.GetElement(elementId);
                                    if (mode == "Keep") {          // Keep elements with matching ElementID values; or make new element.
                                        if (element != null) goto ContinueLoop;
                                    }
                                    else if (mode == "Delete") {   // Delete elements with matching ElementID values and make all new elements.
                                        if (element != null) {
                                            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                                                transaction.Start("Deleting Element");
                                                try {
                                                    this.settings.DocumentDb.Delete(elementId);
                                                }
                                                catch {
                                                    ProcessError("Delete of existing element failed");
                                                }
                                                transaction.Commit();
                                            }
                                        }
                                    }
                                }
                            }

                            switch (hbInputItem.CommandObject) {
                                case "Grid":               returnValue = ProcessAddGrid(); break;
                                case "Level":              returnValue = ProcessAddLevel(); break;
                                case "DetailLine":         returnValue = ProcessAddDetailLine(); break;
                                case "DetailArc":          returnValue = ProcessAddDetailArc(); break;
                                case "DetailEllipse":      returnValue = ProcessAddDetailEllipse(); break;
                                case "DetailNurbSpline":   returnValue = ProcessAddDetailNurbSpline(); break;
                                case "ModelLine":          returnValue = ProcessAddModelLine(); break;
                                case "ModelArc":           returnValue = ProcessAddModelArc(); break;
                                case "ModelEllipse":       returnValue = ProcessAddModelEllipse(); break;
                                case "ModelNurbSpline":    returnValue = ProcessAddModelNurbSpline(); break;
                                case "AreaBoundaryLine":   returnValue = ProcessAddAreaBoundaryLine(); break;
                                case "RoomSeparationLine": returnValue = ProcessAddRoomSeparationLine(); break;
                                case "TopographySurface":  returnValue = ProcessAddTopographySurface(); break;                                    
                                case "Wall":               returnValue = ProcessAddWall(); break;
                                case "Floor":              returnValue = ProcessAddFloor(); break;
                                case "FilledRegion":       returnValue = ProcessAddFilledRegion(); break;
                                case "FamilyInstance":     returnValue = ProcessAddFamilyInstance(); break;
                                case "Column":             returnValue = ProcessAddColumn(); break;
                                case "Beam":               returnValue = ProcessAddBeam(); break;
                                case "AdaptiveComponent":  returnValue = ProcessAddAdaptiveComponent(); break;
                                case "Area":               returnValue = ProcessAddArea(); break;
                                case "Room":               returnValue = ProcessAddRoom(); break;
                                case "ReferencePoint":     returnValue = ProcessAddReferencePoint(); break;
                                case "CurveByPoints":      returnValue = ProcessAddCurveByPoints(); break;
                                case "LoftForm":           returnValue = ProcessAddLoftForm(); break;
                                case "FamilyExtrusion":    returnValue = ProcessAddFamilyExtrusion(); break;
                                default:
                                    ProcessError("Invalid 'Object' value");
                                    break;
                            }
                            if (returnValue != "") {
                                ProcessError(returnValue);
                                lastElementAdded = null;
                            }
                            else {
                                if (this.elements.InnerErrorMessage != "") ProcessError("Warning ignored");
                                if (lastElementAdded != null && lastElementAdded.IsValidObject)  this.currentInputItem.HbInputItem.RecordElementId(new HbElementId(lastElementAdded.Id.IntegerValue));
                                else {
                                    ProcessError("Invalid element");
                                    lastElementAdded = null;
                                }
                            }
                            break;
                        case "Modify":
                            switch (hbInputItem.CommandObject) {
                                case "ParameterSet":
                                    this.elements.ParameterSet(this.lastElementAdded, this.currentInputItem.String0, this.currentInputItem.String1, this.unitsFactor);
                                    if (this.elements.LocalErrorMessage != "") ProcessError("Unable to set parameter");
                                    break;
                                case "CurtainGridUAdd":
                                    this.elements.CurtainGridUAdd(this.lastElementAdded, this.currentInputItem.Double0, this.currentInputItem.Double1);
                                    if (this.elements.LocalErrorMessage != "") ProcessError("Unable to add U curtain grid");
                                    break;
                                case "CurtainGridVAdd":
                                    this.elements.CurtainGridVAdd(this.lastElementAdded, this.currentInputItem.Double0, this.currentInputItem.Double1);
                                    if (this.elements.LocalErrorMessage != "") ProcessError("Unable to add V curtain grid");
                                    break;
                                case "MullionUAdd":
                                    this.elements.MullionUAdd(this.lastElementAdded, this.currentInputItem.Double0, this.currentInputItem.Double1);
                                    if (this.elements.LocalErrorMessage != "") ProcessError("Unable to add U mullion");
                                    break;
                                case "MullionVAdd":
                                    this.elements.MullionVAdd(this.lastElementAdded, this.currentInputItem.Double0, this.currentInputItem.Double1);
                                    if (this.elements.LocalErrorMessage != "") ProcessError("Unable to add V mullion");
                                    break;
                                default:
                                    ProcessError("Invalid 'Object' value in Modify case of switch (hbInputItem.CommandAction).");
                                    break;
                            }                      
                            break;
                        default:
                            ProcessError("Invalid 'Action' value in switch (hbInputItem.CommandAction)");
                            break;
                    }

 ContinueLoop:      System.Windows.Forms.Application.DoEvents(); //Clunky (but effective) way to allow the user to cancel the loop
                    if (progressBarForm.Cancel == true) break;
                    progressBarForm.SetValue(this.indexCurrentItem);
                    progressBarForm.SetLabel(
                          "Processing item " + this.indexCurrentItem.ToString() + " of " + numberOfElements.ToString() + ": "
                        + this.currentInputItem.HbInputItem.RowId + " - " + this.currentInputItem.HbInputItem.CommandAction + " - " + this.currentInputItem.HbInputItem.CommandObject);
                    progressBarForm.Refresh();

                    this.indexCurrentItem++;  //Note explicitly manage While loop index
                }

                // Write the ElementId values back to the .CSV file
                CsvWriter csvWriter = new CsvWriter(csvReader.DataTable);
                returnValue = csvWriter.ConnectToFile(pathCsvFile);
                if (returnValue != "") {
                    ProcessError("Unable to write ElementId values back to .CSV file.  " + returnValue);
                }
                else {
                    returnValue = csvWriter.WriteFile();
                    if (returnValue != "") {
                        ProcessError("Unable to write ElementId values back to .CSV file.  " + returnValue);
                    }
                }

                return true;
            }
            catch (Exception exception) {
                if (this.currentInputItem != null) {
                    System.Windows.Forms.MessageBox.Show("Error in 'CreateElements.ProcessRecords' at this.currentInputItem: " + this.indexCurrentItem.ToString() + ".\nSystem message: " 
                        + exception.Message + "\n\n"
                        + this.currentInputItem.DisplayValues()
                        , this.settings.ProgramName);                
                }
                else {
                    System.Windows.Forms.MessageBox.Show("Error in 'CreateElements.ProcessRecords' at this.currentInputItem: " + this.indexCurrentItem.ToString() + ".\nSystem message: " + exception.Message, this.settings.ProgramName);                
                }
                return false;
            }
            finally {
                if (!(progressBarForm == null)) progressBarForm.Close();
                if ((this.errorMessageCount > 0 && checkBoxListErrors.Checked)) {
                    DisplayErrors displayErrorsDialog = new DisplayErrors(this.errorMessageBuild);
                    displayErrorsDialog.ShowDialog();
                }                    
                this.settings.DocumentUi.RefreshActiveView();
            }            
        }

        private string ProcessAddGrid() {
            Grid grid = this.elements.MakeGrid(this.currentInputItem.Curve);
            if (grid == null) return "Unable to create Grid";               
            this.lastElementAdded = grid;
            return "";
        }

        private string ProcessAddLevel() {
            double double0 = this.currentInputItem.Double0;
            string string0 = this.currentInputItem.String0;
            Level level = this.elements.MakeLevel(double0 * this.unitsFactor, string0);
            if (level == null) return "Unable to create Level";
            this.lastElementAdded = level;
            return "";
        }
        
        private string ProcessAddDetailLine() {
            Line line = (Line)this.currentInputItem.Curve;
            if (this.settings.DocumentDb.IsFamilyDocument) {
                SymbolicCurve symbolicCurve = this.elements.MakeSymbolicCurveLine(line);
                if (symbolicCurve == null) return "Unable to create SymbolicCurve";
                this.lastElementAdded = symbolicCurve;
            }
            else {
                DetailLine detailLine = this.elements.MakeDetailLine(line);
                if (detailLine == null) return "Unable to create DetailLine";
                this.lastElementAdded = detailLine;
            }
            return "";
        }

        private string ProcessAddDetailArc() {
            Arc arc = (Arc)this.currentInputItem.Curve;
            if (this.settings.DocumentDb.IsFamilyDocument) {
                SymbolicCurve symbolicCurve = this.elements.MakeSymbolicCurveArc(arc);
                if (symbolicCurve == null) return "Unable to create SymbolicCurveArc";
                this.lastElementAdded = symbolicCurve;
            }
            else {
                DetailArc detailArc = this.elements.MakeDetailArc(arc);
                if (detailArc == null) return "Unable to create DetailArc";
                this.lastElementAdded = detailArc;
            }
            return "";
        }

        private string ProcessAddDetailEllipse() {
            Ellipse ellipse = (Ellipse)this.currentInputItem.Curve;
            if (this.settings.DocumentDb.IsFamilyDocument) {
                SymbolicCurve symbolicCurve = this.elements.MakeSymbolicCurveEllipse(ellipse);
                if (symbolicCurve == null) return "Unable to create SymbolicCurveEllipse";
                this.lastElementAdded = symbolicCurve;
            }
            else {
                DetailEllipse detailEllipse = this.elements.MakeDetailEllipse(ellipse);
                if (detailEllipse == null) return "Unable to create DetailEllipse";
                this.lastElementAdded = detailEllipse;
            }
            return "";
        }
        private string ProcessAddDetailNurbSpline() {
            NurbSpline nurbSpline = (NurbSpline)this.currentInputItem.Curve;
            if (this.settings.DocumentDb.IsFamilyDocument) {
                SymbolicCurve symbolicCurveNurbSpline = this.elements.MakeSymbolicCurveNurbSpline(nurbSpline);
                if (symbolicCurveNurbSpline == null) return "Unable to create SymbolicCurveNurbSpline";
                this.lastElementAdded = symbolicCurveNurbSpline;
            }
            else {
                DetailNurbSpline detailNurbSpline = this.elements.MakeDetailNurbSpline(nurbSpline);
                if (detailNurbSpline == null) return "Unable to create DetailNurbSpline";
                this.lastElementAdded = detailNurbSpline;
            }
            return "";
        }

        private string ProcessAddModelLine() {
            Line line = (Line)this.currentInputItem.Curve;
            ModelLine modelLine = this.elements.MakeModelLine(line);
            if (modelLine == null) return "Unable to create ModelLine";
            this.lastElementAdded = modelLine;
            return "";
        }
        private string ProcessAddModelArc() {
            Arc arc = (Arc)this.currentInputItem.Curve;
            ModelArc modelArc = this.elements.MakeModelArc(arc);
            if (modelArc == null) return "Unable to create ModelArc";
            this.lastElementAdded = modelArc;
            return "";
        }
        private string ProcessAddModelEllipse() {
            Ellipse ellipse = (Ellipse)this.currentInputItem.Curve;
            ModelEllipse modelEllipse = this.elements.MakeModelEllipse(ellipse);
            if (modelEllipse == null) return "Unable to create ModelEllipse";
            this.lastElementAdded = modelEllipse;
            return "";
        }
        private string ProcessAddModelNurbSpline() {
            NurbSpline nurbSpline = (NurbSpline)this.currentInputItem.Curve;
            ModelNurbSpline modelNurbSpline = this.elements.MakeModelNurbSpline(nurbSpline);
            if (modelNurbSpline == null) return "Unable to create ModelNurbSpline";
            this.lastElementAdded = modelNurbSpline;
            return "";
        }

        //private void ProcessAddAreaBoundaryLine(bool inputUseMode, List<XYZ> pointsListCurrent, ref Element elementLast) {
        private string ProcessAddAreaBoundaryLine() {
            if (!(this.settings.ActiveView.ViewType == ViewType.AreaPlan)) return "Current view must be of type 'AreaPlan' to add AreaBoundaryLine";
            ModelCurve modelCurve;            
            if (this.currentInputItem.HbInputItem.CommandModifier == "Curve") {
                Curve curve = this.currentInputItem.Curve;
                modelCurve = this.elements.MakeAreaBoundaryLine(curve);
            }
            else if (this.currentInputItem.HbInputItem.CommandModifier == "CurveArrArray") {
                CurveArrArray curveArrArray = this.currentInputItem.CurveArrArray;
                modelCurve = this.elements.MakeAreaBoundaryLine(curveArrArray);
            }
            else {
                modelCurve = null;
                return "Unknown 'Command Modifier' value creating AreaBoundaryLine";
            }
            if (modelCurve == null) return "Unable to create AreaBoundaryLine";
            this.lastElementAdded = modelCurve;
            return "";
        }

        private string ProcessAddRoomSeparationLine() {
            ModelCurve modelCurve;
            if (this.currentInputItem.HbInputItem.CommandModifier == "Curve") {
                Curve curve = this.currentInputItem.Curve;
                modelCurve = this.elements.MakeRoomSeparationLine(curve);
            }
            else if (this.currentInputItem.HbInputItem.CommandModifier == "CurveArrArray") {
                CurveArrArray curveArrArray = this.currentInputItem.CurveArrArray;
                modelCurve = this.elements.MakeRoomSeparationLine(curveArrArray);
            }
            else {
                modelCurve = null;
                return "Unknown 'Command Modifier' value creating RoomSeparationLine";
            }
            if (modelCurve == null) return "Unable to create RoomSeparationLine";
            this.lastElementAdded = modelCurve;
            return "";
        }

        private string ProcessAddTopographySurface() {
            TopographySurface topographySurface = this.elements.MakeTopographySurface(this.currentInputItem.ListXYZ);
            if (topographySurface == null) return "Unable to create TopographySurface";
            this.lastElementAdded = topographySurface;
            return "";
        }

        private string ProcessAddWall() {
// TODO Evaluate effect of Ellipse and HermiteSpline in CurveArrArray
            Wall wall;
            if (this.currentInputItem.HbInputItem.CommandModifier == "Curve") {
                Curve curve = this.currentInputItem.Curve;
                wall = this.elements.MakeWall(curve);
            }
            else if (this.currentInputItem.HbInputItem.CommandModifier == "CurveArrArray") {
                CurveArrArray curveArrArray = this.currentInputItem.CurveArrArray;
                wall = this.elements.MakeWall(curveArrArray);
            }
            else {
                wall = null;
                return "Unknown 'Command Modifier' value creating Wall";
            }
            if (wall == null) return "Unable to create Wall";
            this.lastElementAdded = wall;
            return "";
        }

        private string ProcessAddFloor() {
// TODO Evaluate effect of Ellipse and HermiteSpline in CurveArrArray
            Floor floor;
            if (this.currentInputItem.HbInputItem.CommandModifier == "Points") {
                List<XYZ> points = this.currentInputItem.ListXYZ;
                floor = this.elements.MakeFloor(points);
            }
            else if (this.currentInputItem.HbInputItem.CommandModifier == "CurveArrArray") {
                CurveArrArray curveArrArray = this.currentInputItem.CurveArrArray;
                floor = this.elements.MakeFloor(curveArrArray);
            }
            else {
                floor = null;
                return "Unknown 'Command Modifier' value creating Floor";
            }
            if (floor == null) return "Unable to create Floor";
            this.lastElementAdded = floor;
            return "";
        }

        private string ProcessAddFilledRegion() {
// TODO Evaluate effect of Ellipse and HermiteSpline in CurveArrArray
            FilledRegion filledRegion;
            if (this.currentInputItem.HbInputItem.CommandModifier == "Points") {
                List<XYZ> points = this.currentInputItem.ListXYZ;
                filledRegion = this.elements.MakeFilledRegion(points);
            }
            else if (this.currentInputItem.HbInputItem.CommandModifier == "CurveArrArray") {
                CurveArrArray curveArrArray = this.currentInputItem.CurveArrArray;
                filledRegion = this.elements.MakeFilledRegion(curveArrArray);
            }
            else {
                filledRegion = null;
                return "Unknown 'Command Modifier' value creating FilledRegion";
            }
            if (filledRegion == null) return "Unable to create FilledRegion";
            this.lastElementAdded = filledRegion;
            return "";
        }

        private string ProcessAddFamilyInstance() {
            List<XYZ> points = this.currentInputItem.ListXYZ;  // Only using 1 point
            FamilyInstance familyInstance = this.elements.MakeFamilyInstance(points[0]);  
            if (familyInstance == null) return "Unable to create FamilyInstance";
            this.lastElementAdded = familyInstance;
            return "";
        }

        private string ProcessAddColumn() {
            List<XYZ> points = this.currentInputItem.ListXYZ;  // Only using 1 or 2 points
            FamilyInstance column = this.elements.MakeColumn(points);           
            if (column == null) return "Unable to create Column";
            this.lastElementAdded = column;
            return "";
        }

        private string ProcessAddBeam() {
            List<XYZ> points = this.currentInputItem.ListXYZ;  // Only using 1 or 2 points
            FamilyInstance beam = this.elements.MakeBeam(points);
            if (beam == null) return "Unable to create Beam";
            this.lastElementAdded = beam;
            return "";
        }

        private string ProcessAddAdaptiveComponent() {
            List<XYZ> points = this.currentInputItem.ListXYZ;
            FamilyInstance adaptiveComponent = null;
            adaptiveComponent = this.elements.MakeAdaptiveComponent(points);
            if (adaptiveComponent == null) return "Unable to create AdaptiveComponent";
            this.lastElementAdded = adaptiveComponent;
            return "";
        }

        private string ProcessAddArea() {
            List<XYZ> points = this.currentInputItem.ListXYZ;  // Only using 1 point
            Area area = this.elements.MakeArea(points[0]);
            if (area == null) return "Unable to create Area";
            this.lastElementAdded = area;
            return "";
        }
        private string ProcessAddRoom() {
            List<XYZ> points = this.currentInputItem.ListXYZ;  // Only using 1 point
            Room room = this.elements.MakeRoom(points[0]);
            if (room == null) return "Unable to create Room";
            this.lastElementAdded = room;
            return "";
        }

        private string ProcessAddReferencePoint() {
            HbReferencePoint hbReferencePoint = this.currentInputItem.HbInputItem.HbReferencePoint;  // Only using 1 point which is actually a Hb
            ReferencePoint referencePoint = this.elements.MakeReferencePoint(hbReferencePoint);
            if (referencePoint == null) return "Unable to create ReferencePoint";
            this.lastElementAdded = referencePoint;
            return "";
        }
        private string ProcessAddCurveByPoints() {
            HbReferenceArray hbReferenceArray = this.currentInputItem.HbInputItem.HbReferenceArray;
            CurveByPoints curveByPoints = this.elements.MakeCurveByPoints(hbReferenceArray);
            if (curveByPoints == null) return "Unable to create CurveByPoints";
            this.lastElementAdded = curveByPoints;
            return "";
        }
        private string ProcessAddLoftForm() {
            HbReferenceArrayArray hbReferenceArrayArray = this.currentInputItem.HbInputItem.HbReferenceArrayArray;
            Form loftForm = this.elements.MakeLoftForm(hbReferenceArrayArray);
            if (loftForm == null) return "Unable to create LoftForm";
            this.lastElementAdded = loftForm;
            return "";
        }

        private string ProcessAddFamilyExtrusion() {
            textBoxPathTemplate.Text = textBoxPathTemplate.Text.Trim();
            if (textBoxPathTemplate.Text == null) textBoxPathTemplate.Text = "";
            string pathTemplate = textBoxPathTemplate.Text;
            if (pathTemplate == "") return "Path to Family Template File must be specified";
            if (!File.Exists(pathTemplate)) return "Family Template File specified does not exist";
            // Generate a unique name.
            string familyName = GenerateUniqueFamilySymbolName(this.currentInputItem.String0, 4);
            // Insertion point: Note that this.currentInputItem.String1 == null is OK since point is optional and function will default to new XYZ(0, 0, 0);
            FamilyInstance familyInstance = this.elements.MakeFamilyExtrusion(this.currentInputItem.CurveArrArray, familyName, pathTemplate, this.currentInputItem.ListXYZ[0]);
            if (familyInstance == null) return "Unable to create FamilyInstance";
            this.lastElementAdded = familyInstance;
            return "";
        }

        #endregion

        #region Utility Functions                       // ****************************** Utility Functions *****************************************************



        private string GenerateUniqueFamilySymbolName(string seed, int minNumericDigits) {

            // Find the existing family symbol names so we don't duplicate; Only do this first time we need to.
            if (this.existingFamilySymbols == null) GetExistingFamilySymbolNames();

            // Not doing this.  We will always number the files, starting with 0000
            ////If user has provided a name we assume they are trying to be explicit and only append numbers if necessary
            //if (seed != "") {
            //    if (!this.existingFamilySymbols.Contains(seed)) {
            //        this.existingFamilySymbols.Add(seed);
            //        return seed;
            //    }
            //}

            string buildString;
            if (seed != "") buildString = seed;
            else buildString = UNIQUE_FAMILY_NAME_SEED;

            string extensionString = null;
            for (int i = 0; i < 10000000; i++) {   // A big number to avoid having an infinite loop
                extensionString = i.ToString();
                while (extensionString.Length < minNumericDigits) {
                    extensionString = "0" + extensionString;
                }
                if (!this.existingFamilySymbols.Contains(buildString + "-" + extensionString)) {
                    buildString = buildString + "-" + extensionString;
                    this.existingFamilySymbols.Add(buildString);
                    return buildString;
                }
            }
            return null;  // should not ever reach this point.
        }

        private void GetExistingFamilySymbolNames() {
            // Get a list of existing fmaily symbols so we can check for uniqueness
            this.existingFamilySymbols = new List<string>();
            FilteredElementCollector collectorFamilies = new FilteredElementCollector(this.settings.DocumentDb).OfClass(typeof(FamilySymbol));
            ICollection<Element> elementsFamilies = collectorFamilies.ToElements();
            foreach (FamilySymbol familySymbol in elementsFamilies) {
                if (!this.existingFamilySymbols.Contains(familySymbol.Name)) this.existingFamilySymbols.Add(familySymbol.Name);
            }
        }

        private void InitializeComboBoxes() {
            // **** Note that we do not validate default values.  It is annoying when you open one file that doesn't contain a family and it wipes out the saved values
            // since you then might change to a new project that does have those values.
            // Fill and restore the family combo boxes
            comboBoxFamilyFamily.Text = comboBoxFamilyFamily.Text.Trim();
            comboBoxFamilyType.Text = comboBoxFamilyType.Text.Trim();
            string saveFamilyFamily = comboBoxFamilyFamily.Text;
            string saveFamilyType = comboBoxFamilyType.Text;
            FillComboFamilyFamily();
            comboBoxFamilyFamily.Text = saveFamilyFamily;
            FillComboFamilyType();
            comboBoxFamilyType.Text = saveFamilyType;

            comboBoxColumnArchFamily.Text = comboBoxColumnArchFamily.Text.Trim();
            comboBoxColumnArchType.Text = comboBoxColumnArchType.Text.Trim();
            string saveColumnArchFamily = comboBoxColumnArchFamily.Text;
            string saveColumnArchType = comboBoxColumnArchType.Text;
            FillComboColumnArchFamily();

            comboBoxColumnArchFamily.Text = saveColumnArchFamily;
            FillComboColumnArchType();
            comboBoxColumnArchType.Text = saveColumnArchType;

            comboBoxColumnStructFamily.Text = comboBoxColumnStructFamily.Text.Trim();
            comboBoxColumnStructType.Text = comboBoxColumnStructType.Text.Trim();
            string saveColumnStructFamily = comboBoxColumnStructFamily.Text;
            string saveColumnStructType = comboBoxColumnStructType.Text;
            FillComboColumnStructFamily();
            comboBoxColumnStructFamily.Text = saveColumnStructFamily;
            FillComboColumnStructType();
            comboBoxColumnStructType.Text = saveColumnStructType;

            comboBoxBeamFamily.Text = comboBoxBeamFamily.Text.Trim();
            comboBoxBeamType.Text = comboBoxBeamType.Text.Trim();
            string saveBeamFamily = comboBoxBeamFamily.Text;
            string saveBeamType = comboBoxBeamType.Text;
            FillComboBeamFamily();
            comboBoxBeamFamily.Text = saveBeamFamily;
            FillComboBeamType();;
            comboBoxBeamType.Text = saveBeamType;

            comboBoxAdaptiveCompFamily.Text = comboBoxAdaptiveCompFamily.Text.Trim();
            comboBoxAdaptiveCompType.Text = comboBoxAdaptiveCompType.Text.Trim();
            string saveAdaptiveCompFamily = comboBoxAdaptiveCompFamily.Text;
            string saveAdaptiveCompType = comboBoxAdaptiveCompType.Text;
            FillComboAdaptiveCompFamily();
            comboBoxAdaptiveCompFamily.Text = saveAdaptiveCompFamily;
            FillComboAdaptiveCompType();
            comboBoxAdaptiveCompType.Text = saveAdaptiveCompType;

            comboBoxMullionFamily.Text = comboBoxMullionFamily.Text.Trim();
            comboBoxMullionType.Text = comboBoxMullionType.Text.Trim();
            string saveMullionFamily = comboBoxMullionFamily.Text;
            string saveMullionType = comboBoxMullionType.Text;
            FillComboMullionFamily();
            comboBoxMullionFamily.Text = saveMullionFamily;
            FillComboMullionType();
            comboBoxMullionType.Text = saveMullionType;
        }

        private void SetDefaultsGeneral() {
            //Set default level as current level if plan view, or lowest level if 3D view.
            if (this.settings.CurrentLevel != null) this.elements.LevelCurrent = this.settings.CurrentLevel;
            else {
                this.elements.LevelCurrent = this.elements.Levels.First().Value;
                foreach (string levelName in this.elements.Levels.Keys) {
                    if (this.elements.Levels[levelName].Elevation < this.elements.LevelCurrent.Elevation) this.elements.LevelCurrent = this.elements.Levels[levelName];
                }
            }
            this.elements.WallTypeCurrent = null;  // Will use current value
            this.elements.WallHeightCurrent = 10;  // Note: Set as feet; does not get scaled by units factor
            this.elements.FloorTypeCurrent = null; // Will use current value
        }

        private void SetDefaultsFamily() {
            if (this.elements == null) return;
            this.elements.FamilyFamilyCurrent = null;
            this.elements.FamilyTypeCurrent = null;
            if (this.elements.FamilyTypes.ContainsKey(comboBoxFamilyFamily.Text)) {
                if (this.elements.FamilyTypes[comboBoxFamilyFamily.Text].ContainsKey(comboBoxFamilyType.Text)) {
                    this.elements.FamilyFamilyCurrent = comboBoxFamilyFamily.Text;
                    this.elements.FamilyTypeCurrent = comboBoxFamilyType.Text;
                }
            }
        }
        
        private void SetDefaultsColumn() {
            if (this.elements == null) return;
            this.elements.ColumnModeCurrent = null;
            this.elements.ColumnFamilyCurrent = null;
            this.elements.ColumnTypeCurrent = null;
            this.elements.ColumnHeightCurrent = 10;  // Note: Set as feet; does not get scaled by units factor
            if (   comboBoxColumnMode.Text == "Architectural"         || comboBoxColumnMode.Text == "StructuralVertical" 
                || comboBoxColumnMode.Text == "StructuralPointDriven" || comboBoxColumnMode.Text == "StructuralAngleDriven") {
                this.elements.ColumnModeCurrent = comboBoxColumnMode.Text;
                if (comboBoxColumnMode.Text == "Architectural") {
                    if (this.elements.ColumnArchitecturalTypes.ContainsKey(comboBoxColumnArchFamily.Text)) {
                        if (this.elements.ColumnArchitecturalTypes[comboBoxColumnArchFamily.Text].ContainsKey(comboBoxColumnArchType.Text)) {
                            this.elements.ColumnFamilyCurrent = comboBoxColumnArchFamily.Text;
                            this.elements.ColumnTypeCurrent = comboBoxColumnArchType.Text;
                        }
                    }
                }
                else {
                    if (this.elements.ColumnStructuralTypes.ContainsKey(comboBoxColumnStructFamily.Text)) {
                        if (this.elements.ColumnStructuralTypes[comboBoxColumnStructFamily.Text].ContainsKey(comboBoxColumnStructType.Text)) {
                            this.elements.ColumnFamilyCurrent = comboBoxColumnStructFamily.Text;
                            this.elements.ColumnTypeCurrent = comboBoxColumnStructType.Text;
                        }
                    }
                }
             }
        }

        private void SetDefaultsBeam() {
            if (this.elements == null) return;
            this.elements.BeamFamilyCurrent = null;
            this.elements.BeamTypeCurrent = null;
            if (this.elements.BeamTypes.ContainsKey(comboBoxBeamFamily.Text)) {
                if (this.elements.BeamTypes[comboBoxBeamFamily.Text].ContainsKey(comboBoxBeamType.Text)) {
                    this.elements.BeamFamilyCurrent = comboBoxBeamFamily.Text;
                    this.elements.BeamTypeCurrent = comboBoxBeamType.Text;
                }
            }
        }

        private void SetDefaultsAdaptiveComponent() {
            if (this.elements == null) return;
            this.elements.AdaptiveComponentFamilyCurrent = null;
            this.elements.AdaptiveComponentTypeCurrent = null;            
            if (this.elements.FamilyTypes.ContainsKey(comboBoxAdaptiveCompFamily.Text)) {
                if (this.elements.FamilyTypes[comboBoxAdaptiveCompFamily.Text].ContainsKey(comboBoxAdaptiveCompType.Text)) {
                    this.elements.AdaptiveComponentFamilyCurrent = comboBoxAdaptiveCompFamily.Text;
                    this.elements.AdaptiveComponentTypeCurrent = comboBoxAdaptiveCompType.Text;
                }
            }
        }

        private void SetDefaultsMullion() {
            if (this.elements == null) return;
            this.elements.MullionFamilyCurrent = null;
            this.elements.MullionTypeCurrent = null;
            if (this.elements.MullionTypes.ContainsKey(comboBoxMullionFamily.Text)) {
                if (this.elements.MullionTypes[comboBoxMullionFamily.Text].ContainsKey(comboBoxMullionType.Text)) {
                    this.elements.MullionFamilyCurrent = comboBoxMullionFamily.Text;
                    this.elements.MullionTypeCurrent = comboBoxMullionType.Text;
                }
            }
        }

        private void FillComboFamilyFamily() {
            comboBoxFamilyFamily.Text = "";
            comboBoxFamilyFamily.Items.Clear();
            foreach (string familyName in this.elements.FamilyTypes.Keys) {
                comboBoxFamilyFamily.Items.Add(familyName);
            }
        }
        private void FillComboFamilyType() {
            comboBoxFamilyType.Text = "";
            comboBoxFamilyType.Items.Clear();
            comboBoxFamilyFamily.Text = comboBoxFamilyFamily.Text.Trim();
            if (this.elements.FamilyTypes.ContainsKey(comboBoxFamilyFamily.Text)) {
                foreach (string typeName in this.elements.FamilyTypes[comboBoxFamilyFamily.Text].Keys) {
                    comboBoxFamilyType.Items.Add(typeName);
                }
            }
            if (comboBoxFamilyType.Items.Count == 1) comboBoxFamilyType.SelectedIndex = 0;
        }

        private void FillComboColumnArchFamily() {
            comboBoxColumnArchFamily.Text = "";
            comboBoxColumnArchFamily.Items.Clear();
            foreach (string familyName in this.elements.ColumnArchitecturalTypes.Keys) {
                comboBoxColumnArchFamily.Items.Add(familyName);
            }
        }
        private void FillComboColumnArchType() {
            comboBoxColumnArchType.Text = "";
            comboBoxColumnArchType.Items.Clear();
            comboBoxColumnArchFamily.Text = comboBoxColumnArchFamily.Text.Trim();
            if (this.elements.ColumnArchitecturalTypes.ContainsKey(comboBoxColumnArchFamily.Text)) {
                foreach (string typeName in this.elements.ColumnArchitecturalTypes[comboBoxColumnArchFamily.Text].Keys) {
                    comboBoxColumnArchType.Items.Add(typeName);
                }
            }
            if (comboBoxColumnArchType.Items.Count > 0) comboBoxColumnArchType.SelectedIndex = 0;
        }

        private void FillComboColumnStructFamily() {
            comboBoxColumnStructFamily.Text = "";
            comboBoxColumnStructFamily.Items.Clear();
            foreach (string familyName in this.elements.ColumnStructuralTypes.Keys) {
                comboBoxColumnStructFamily.Items.Add(familyName);
            }
        }
        private void FillComboColumnStructType() {
            comboBoxColumnStructType.Text = "";
            comboBoxColumnStructType.Items.Clear();
            comboBoxColumnStructFamily.Text = comboBoxColumnStructFamily.Text.Trim();
            if (this.elements.ColumnStructuralTypes.ContainsKey(comboBoxColumnStructFamily.Text)) {
                foreach (string typeName in this.elements.ColumnStructuralTypes[comboBoxColumnStructFamily.Text].Keys) {
                    comboBoxColumnStructType.Items.Add(typeName);
                }
            }
            if (comboBoxColumnStructType.Items.Count > 0) comboBoxColumnStructType.SelectedIndex = 0;
        }

        private void FillComboBeamFamily() {
            comboBoxBeamFamily.Text = "";
            comboBoxBeamFamily.Items.Clear();
            foreach (string familyName in this.elements.BeamTypes.Keys) {
                comboBoxBeamFamily.Items.Add(familyName);
            }
        }
        private void FillComboBeamType() {
            comboBoxBeamType.Text = "";
            comboBoxBeamType.Items.Clear();
            comboBoxBeamFamily.Text = comboBoxBeamFamily.Text.Trim();
            if (this.elements.BeamTypes.ContainsKey(comboBoxBeamFamily.Text)) {
                foreach (string typeName in this.elements.BeamTypes[comboBoxBeamFamily.Text].Keys) {
                    comboBoxBeamType.Items.Add(typeName);
                }
            }
            if (comboBoxBeamType.Items.Count == 1) comboBoxBeamType.SelectedIndex = 0;
        }

        private void FillComboAdaptiveCompFamily() {
            comboBoxAdaptiveCompFamily.Text = "";
            comboBoxAdaptiveCompFamily.Items.Clear();
            foreach (string familyName in this.elements.AdaptiveComponentTypes.Keys) {
                comboBoxAdaptiveCompFamily.Items.Add(familyName);
            }
        }
        private void FillComboAdaptiveCompType() {
            comboBoxAdaptiveCompType.Text = "";
            comboBoxAdaptiveCompType.Items.Clear();
            comboBoxAdaptiveCompFamily.Text = comboBoxAdaptiveCompFamily.Text.Trim();
            if (this.elements.AdaptiveComponentTypes.ContainsKey(comboBoxAdaptiveCompFamily.Text)) {
                foreach (string typeName in this.elements.AdaptiveComponentTypes[comboBoxAdaptiveCompFamily.Text].Keys) {
                    comboBoxAdaptiveCompType.Items.Add(typeName);
                }
            }
            if (comboBoxAdaptiveCompType.Items.Count > 0) comboBoxAdaptiveCompType.SelectedIndex = 0;
        }

        private void FillComboMullionFamily() {
            comboBoxMullionFamily.Text = "";
            comboBoxMullionFamily.Items.Clear();
            foreach (string familyName in this.elements.MullionTypes.Keys) {
                comboBoxMullionFamily.Items.Add(familyName);
            }
        }
        private void FillComboMullionType() {
            comboBoxMullionType.Text = "";
            comboBoxMullionType.Items.Clear();
            comboBoxMullionFamily.Text = comboBoxMullionFamily.Text.Trim();
            if (this.elements.MullionTypes.ContainsKey(comboBoxMullionFamily.Text)) {
                foreach (string typeName in this.elements.MullionTypes[comboBoxMullionFamily.Text].Keys) {
                    comboBoxMullionType.Items.Add(typeName);
                }
            }
            if (comboBoxMullionType.Items.Count > 0) comboBoxMullionType.SelectedIndex = 0;
        }

        private void ProcessError(string errorText) {
            if (this.errorMessageCount < this.maxErrorsToShow) {
                this.errorMessageBuild = this.errorMessageBuild + "\r\n\r\n" + errorText;
                if (this.elements.LocalErrorMessage != null) {
                    if (this.elements.LocalErrorMessage != "") {
                        this.errorMessageBuild = this.errorMessageBuild
                            + "\r\nLocal Error Message: " + this.elements.LocalErrorMessage;
                    }
                }
                if (this.elements.InnerErrorMessage != null) {
                    if (this.elements.InnerErrorMessage != "") {
                        this.errorMessageBuild = this.errorMessageBuild
                            + "\r\nInner Error Severity: " + this.elements.InnerErrorSeverity
                            + "\r\nInner Error Message: " + this.elements.InnerErrorMessage;
                    }
                }
                if (this.currentInputItem != null) {
                    this.errorMessageBuild = this.errorMessageBuild
                        + "\r\n" + this.currentInputItem.DisplayValues();
                }
            }
            this.errorMessageCount++;
            return;
        }
        #endregion

        #region Event Handlers                          // ****************************** Event Handlers **********************************************************

        private void buttonProcess_Click(object sender, EventArgs e) {
            ProcessRecords();
            if (this.checkBoxAutoClose.Checked)  Close();
        }

        private void buttonClose_Click(object sender, EventArgs e) {
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
                if (Directory.Exists(path)) textBoxPathFolder.Text = path;
                else textBoxPathFolder.Text = "";
                this.pathFolder = textBoxPathFolder.Text;
                FillListBox();
            }
        }

        private void textBoxPathFolder_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
            if ((e.KeyChar.ToString() == "\r") || (e.KeyChar.ToString() == "\t")) textBoxPathFolder_Leave(sender, e);
        }
        private void textBoxPathFolder_Leave(object sender, EventArgs e) {
            textBoxPathFolder.Text = textBoxPathFolder.Text.Trim();
            this.pathFolder = textBoxPathFolder.Text;
            FillListBox();
        }

        private void comboBoxFamilyFamily_SelectedIndexChanged(object sender, EventArgs e) {
            FillComboFamilyType();
            SetDefaultsFamily();
        }
        private void comboBoxFamilyType_SelectedIndexChanged(object sender, EventArgs e) {
            SetDefaultsFamily();
        }

        private void comboBoxColumnMode_SelectedIndexChanged(object sender, EventArgs e) {
            SetDefaultsColumn();
        }

        private void comboBoxColumnArchFamily_SelectedIndexChanged(object sender, EventArgs e) {
            FillComboColumnArchType();
            SetDefaultsColumn();
        }
        private void comboBoxColumnArchType_SelectedIndexChanged(object sender, EventArgs e) {
            SetDefaultsColumn();
        }

        private void comboBoxColumnStructFamily_SelectedIndexChanged(object sender, EventArgs e) {            
            FillComboColumnStructType();
            SetDefaultsColumn();
        }
        private void comboBoxColumnStructType_SelectedIndexChanged(object sender, EventArgs e) {
            SetDefaultsColumn();
        }

        private void comboBoxBeamFamily_SelectedIndexChanged(object sender, EventArgs e) {
            FillComboBeamType();
            SetDefaultsBeam();
        }

        private void comboBoxBeamType_SelectedIndexChanged(object sender, EventArgs e) {
            SetDefaultsBeam();
        }

        private void comboBoxAdaptiveCompFamily_SelectedIndexChanged(object sender, EventArgs e) {
            FillComboAdaptiveCompType();
            SetDefaultsAdaptiveComponent();
        }
        private void comboBoxAdaptiveCompType_SelectedIndexChanged(object sender, EventArgs e) {
            SetDefaultsAdaptiveComponent();
        }

        private void comboBoxMullionFamily_SelectedIndexChanged(object sender, EventArgs e) {
            FillComboMullionType();
            SetDefaultsMullion();
        }

        private void comboBoxMullionType_SelectedIndexChanged(object sender, EventArgs e) {
            SetDefaultsMullion();
        }

        private void buttonBrowseTemplate_Click(object sender, EventArgs e) {
            //Show a browse dialog to select template file.
            openFileDialog1.Filter = "Revit Family Templates (*.rft)|*.rft";
            if (File.Exists(textBoxPathTemplate.Text))
                openFileDialog1.FileName = textBoxPathTemplate.Text;
            else {
                openFileDialog1.FileName = "";
                string path = textBoxPathTemplate.Text;
                int pos = path.LastIndexOf(@"\");
                if (pos > 1 && pos < path.Length - 2) {
                    path = textBoxPathTemplate.Text.Substring(0, pos);
                    if (Directory.Exists(path))
                        openFileDialog1.InitialDirectory = path;
                }
            }
            System.Windows.Forms.DialogResult result = openFileDialog1.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.Cancel) {
                textBoxPathTemplate.Text = openFileDialog1.FileName;
            }
        }

//TODO This appears to be disabled
        private void textBoxPathTemplate_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
            if ((e.KeyChar.ToString() == "\r") || (e.KeyChar.ToString() == "\t")) textBoxPathTemplate_Leave(sender, e);
        }
        private void textBoxPathTemplate_Leave(object sender, EventArgs e) {
            //FillListBox();
        }

//TODO complete use of this path.  Add to settings?  needs to be availble to Utility Elements
        private void buttonBrowseTempFolder_Click(object sender, EventArgs e) {

            System.Windows.Forms.DialogResult dialogResult;
            string path = textBoxTempFolder.Text;
            if (path.EndsWith("\\")) path = path.Substring(0, path.Length - 1);  //In case use added a "\"
            if (Directory.Exists(path)) folderBrowserDialog1.SelectedPath = path;
            else folderBrowserDialog1.SelectedPath = "C:\\";
            dialogResult = folderBrowserDialog1.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK) {
                path = folderBrowserDialog1.SelectedPath.ToString();
                if (Directory.Exists(path)) {
                    textBoxTempFolder.Text = path;
                    this.settings.TempFilePath = path;
                } else {
                    textBoxTempFolder.Text = "";
                    this.settings.TempFilePath = "";
                }
                //FillViewsList();
            }

        }

        #endregion


    }
}
