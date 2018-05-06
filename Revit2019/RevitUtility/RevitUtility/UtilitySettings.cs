using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;  //for File, StreamWriter

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace RevitUtility {

    public class UtilitySettings {

        // ************************************************* Module Variables *******************************************

        private const string VERSION = "2019";
        private const string DELIMITER = "\t";
        private const string TEMP_FILE_NAME = "TempFile.tmp";      // Used in RefreshRevitValues() to confirm that user has write access to folder

        // Note that the path to the user folder is duplicated in the RevitUtilityMenu.UserButtons module since this UtilitySettings is not availble 
        private const string USER_FOLDER_HUMMINGBIRD = "Hummingbird";   // Used to store all Hummingbird data
        private const string USER_FOLDER_REVIT_VERSION = "Revit 2019";  // Used to store .ini files, etc. for this version of Revit  

        //Standard values
        public bool ValidInstallation = true;
        public string ProgramName { set; get; }        
        public string UserFolderPath { set; get; }
        public bool Cancelled;

        //Common Revit values
        public ExternalCommandData CommandData { set; get; }
        public UIApplication ApplicationUi { set; get; }   //DB.Applicaion
        public Application ApplicationAs { set; get; }     //ApplicaionServices.Application
        public UIDocument DocumentUi { set; get; }         //UIApplication.ActiveUIDocument;
        public Document DocumentDb { set; get; }           //UIDocument.Document;
        public View ActiveView { set; get; }
        public ViewPlan ActiveViewPlan { set; get; }
        public Level CurrentLevel { set; get; }
        public Phase CurrentPhase { set; get; }
        public bool CurrentViewIsPlan { set; get; }
        public ICollection<ElementId> ElementIdsPreSelected { set; get; }
        public ElementSet ElementsPreSelected { set; get; }

        public SortedDictionary<string, Category> CategoriesByName = new SortedDictionary<string, Category>();

        public string IniFilePath = "";   //Path to folder with file name
        public string TempFilePath = "";  // Path to folder used to place temporary files (set in ModelBuilder.CreateElements)

        //Container for the variable settings
        private Dictionary<string, VariableSetting> mVariableSettings = new Dictionary<string, VariableSetting>();

        //Container for the control settings
        private Dictionary<string, ControlSetting> mControlSettings = new Dictionary<string, ControlSetting>();

        //Convenient lookup for which controls on a form have a control setting associated with them.
        private Dictionary<string, List<string>> mFormSettingsMap = new Dictionary<string, List<string>>();

        //Local only
        private string iniFileRootName;        

        // ***************************************************** Constructor ***************************************************

        public UtilitySettings(ExternalCommandData commandData, ref string message, ElementSet elementSet, string programName, string iniFileRootName) {
            
            // Check Revit version
            if (!commandData.Application.Application.VersionName.Contains(VERSION)) {
                System.Windows.Forms.MessageBox.Show("Revit version is not " + VERSION + ".  This version of the tools cannot be used.");
                this.Cancelled = true;
                this.ValidInstallation = false;
                return;
            }

            this.CommandData = commandData;
            this.ProgramName = programName;
            //this.iniFileNameStandard = iniFileRootName;

            //Need to refresh Revit values first so we can use mDBDocument to get folder path.
            if (!RefreshRevitValues()) System.Windows.Forms.MessageBox.Show("Error in UtilitySettings.RefreshRevitValues().", programName);

            // Calculate UserFolderPath and use as location to store ini files.
            this.iniFileRootName = iniFileRootName;
            FindOrMakeUserFolder();  // Not looking for error.  this.UserFolderPath = "" is indication of failure

        }

        // ************************************************** Public Functions *************************************************
        
        //Variable Settings:
        public bool AddVariableSetting(string settingId, string valueDefault) {
            try {
                mVariableSettings.Add(settingId, new VariableSetting("", valueDefault));
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                return false;
            }
        }
        //Note added the "Save" version below as equivalent to the "Set" version since it is more consistent with control terminology.
        public bool SaveVariableSetting(string settingId, string valueToSet) {
            return SetVariableSetting(settingId, valueToSet);
        }
        public bool SetVariableSetting(string settingId, string valueToSet) {
            try {
                if (!mVariableSettings.ContainsKey(settingId)) {
                    System.Windows.Forms.MessageBox.Show(
                          "Warning: UtilitySettings.SetVariableSetting called with settingId: '" + settingId + "'\n"
                        + "however no entry has been added with that key value.");
                    return false;
                }
                mVariableSettings[settingId].ValueCurrent = valueToSet;
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                return false;
            }
        }
        public bool GetVariableSetting(string settingId, ref string valueToReturn) {
            try {
                if (!mVariableSettings.ContainsKey(settingId)) {
                    System.Windows.Forms.MessageBox.Show(
                          "Warning: UtilitySettings.GetVariableSetting called with settingId: '" + settingId + "'\n"
                        + "however no entry has been added with that key value.");
                    return false;
                }
                valueToReturn = mVariableSettings[settingId].ValueCurrent;
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                return false;
            }
        }
       
        //Control Settings:
        //This version can be used for a one-off situation but more typically it is called by the array version below.
        public bool AddControlSetting(string formName, string settingId, string controlName, string valueDefault) {
            try {
                mControlSettings.Add(settingId, new ControlSetting(controlName, "", valueDefault));
                if (mFormSettingsMap.ContainsKey(formName)) mFormSettingsMap[formName].Add(settingId);
                else mFormSettingsMap.Add(formName, new List<string> {settingId} );
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                return false;
            }
        }
        //Primary tool for adding settings.
        public bool AddControlSettingArray(string formName, string[,] settingValues) {
            try {
                for (int i = 0; i <= settingValues.GetUpperBound(0); i++) {
                    mControlSettings.Add(settingValues[i, 0], new ControlSetting(settingValues[i, 1], "", settingValues[i, 2]));
                    if (mFormSettingsMap.ContainsKey(formName)) mFormSettingsMap[formName].Add(settingValues[i, 0]);
                    else mFormSettingsMap.Add(formName, new List<string> { settingValues[i, 0] });
                }
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                return false;
            }
        }

        //Reloads all default values
        public bool ReloadDefaultValues() {

            if (this.IniFilePath == "") return true; // Fail silently if not able to use ini file.{

            try {
                foreach (VariableSetting variableSetting in mVariableSettings.Values) {
                    variableSetting.ValueCurrent = variableSetting.ValueDefault;
                }
                foreach (ControlSetting controlSetting in mControlSettings.Values) {
                    controlSetting.ValueCurrent = controlSetting.ValueDefault;
                }
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                return false;
            }
        }

        public bool ReadIniFile() {
            
            if (this.IniFilePath == "") return true; // Fail silently if not able to use ini file.{

            try {

                //Reload the defaults so if there are any mismatches with the file the setting will get the default value.
                ReloadDefaultValues();

                if (File.Exists(this.IniFilePath)) {
                    using (StreamReader streamReader = File.OpenText(this.IniFilePath)) {
                        for (int i = 0; i < 1000; i++) {  //just to prevent endless loop
                            string line = streamReader.ReadLine();
                            if (line == null) {
                                if (i == 0) {//Case that an empty settings file was opened.
                                    System.Windows.Forms.MessageBox.Show(
                                    "Empty file found in UtilitySettings.ReadIniFile.",this. ProgramName);
                                    streamReader.Close();
                                    return true;
                                }
                                break;  //Typical case of finding end of file.
                            }
                            string[] inputLine = line.Split(DELIMITER.ToCharArray());
                            if (inputLine.GetLength(0) != 2) {  //This really shoudln't happen!
                                System.Windows.Forms.MessageBox.Show(
                                    "Error reading line in UtilitySettings.ReadIniFile.",this. ProgramName);
                                streamReader.Close();
                                return true;
                            }
                            string settingId = inputLine[0];
                            if (mVariableSettings.ContainsKey(settingId)) {     //Variable case
                                string valueCurrent = inputLine[1];
                                mVariableSettings[settingId].ValueCurrent = valueCurrent;
                            }
                            else {
                                if (mControlSettings.ContainsKey(settingId)) {  //Control case
                                    string valueCurrent = inputLine[1];
                                    mControlSettings[settingId].ValueCurrent = valueCurrent;
                                }
                                else {                                          //Something bad case
                                    System.Windows.Forms.MessageBox.Show(
                                        "Invalid settingId value in UtilitySettings.ReadIniFile.",this. ProgramName);
                                    streamReader.Close();
                                    return false;
                                }
                            }
                        }
                        streamReader.Close();
                    }
                    return true;
                }
                else return false;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(
                          "Error reading .ini file in UtilitySettings.ReadIniFile."
                    + "\n\nSystem message: " + exception.Message,this. ProgramName);
                return false;
            }
        }

        //Writes the file.
        public bool WriteIniFile() {

            if (this.IniFilePath == "") return true; // Fail silently if not able to use ini file.

            try {
                //Note, if file doesn't exist it is created
                // The using statement also closes the StreamWriter.
                using (StreamWriter sw = new StreamWriter(this.IniFilePath)) {
                    foreach (string settingId in mVariableSettings.Keys) {
                        sw.WriteLine(settingId + DELIMITER + mVariableSettings[settingId].ValueCurrent);
                    }
                    foreach (string settingId in mControlSettings.Keys) {
                        sw.WriteLine(settingId + DELIMITER + mControlSettings[settingId].ValueCurrent);
                    }
                }
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(
                    "Error writing .ini file in UtilitySettings.WriteIniFile."
                  + "\n\nSystem message: " + exception.Message,this. ProgramName);
                return false;
            }
        }

        public bool SetFormControls(System.Windows.Forms.Form ownerForm) {
            try {
                if (!mFormSettingsMap.ContainsKey(ownerForm.Name)) {
                    System.Windows.Forms.MessageBox.Show(
                          "Warning: UtilitySettings.SetFormControls called with owner form name: '" + ownerForm.Name + "'\n"
                        + "however no control settings have been defined with that form name.");
                    return false;
                }
                foreach (string settingId in mFormSettingsMap[ownerForm.Name]) {
                        mControlSettings[settingId].SetControl(ownerForm);
                }
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                return false;
            }
        }

        public bool SaveFormControls(System.Windows.Forms.Form ownerForm) {
            try {
                if (!mFormSettingsMap.ContainsKey(ownerForm.Name)) {
                    System.Windows.Forms.MessageBox.Show(
                          "Warning: UtilitySettings.SaveFormControls called with owner form name: '" + ownerForm.Name + "'\n"
                        + "however no control settings have been defined with that form name.");
                    return false;
                }
                foreach (string settingId in mFormSettingsMap[ownerForm.Name]) {
                    mControlSettings[settingId].SaveControl(ownerForm);
                }
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                return false;
            }
        }

        // ************************************************* Private Functions *******************************************

        private bool FindOrMakeUserFolder() {
            try {
                string hummingbirdPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), USER_FOLDER_HUMMINGBIRD);
                if (!Directory.Exists(hummingbirdPath)) {
                    try {
                        Directory.CreateDirectory(hummingbirdPath);
                    }
                    catch {
                        this.UserFolderPath = "";
                        return false;  // Returning silently.  this.UserFolderPath = "" is indication of failure
                    }
                }
                this.UserFolderPath = Path.Combine(hummingbirdPath, USER_FOLDER_REVIT_VERSION);
                if (!Directory.Exists(this.UserFolderPath)) {
                    try {
                        Directory.CreateDirectory(this.UserFolderPath);
                    } catch {
                        this.UserFolderPath = "";
                        return false;  // Returning silently.  this.UserFolderPath = "" is indication of failure
                    }
                }
                if (Directory.Exists(this.UserFolderPath)) {
                    this.IniFilePath = Path.Combine(this.UserFolderPath, this.iniFileRootName + ".ini");  // If not here this.iniFilePath has default value of "".
                    //this.TempFilePath = this.UserFolderPath;  // Just to have some value in case user doesn't set it; otherwise has default value of "".
                }
                return true;
            }
            catch {
                // Returning silently for now
                return false;
            }
        }

        private bool RefreshRevitValues() {
        //Note: Currently these are all read-only and there is no need to refresh them; If the API allows re-entry this could change.
            //Gets Revit settings for document, view, etc;
            try {
                this.ApplicationUi = this.CommandData.Application;
                this.DocumentUi = this.ApplicationUi.ActiveUIDocument;
                this.DocumentDb = this.DocumentUi.Document;
                this.ApplicationAs = this.DocumentDb.Application;
                this.ActiveView = this.DocumentDb.ActiveView;
                this.CurrentViewIsPlan = true;
                try {
                    this.ActiveViewPlan = (ViewPlan)this.ActiveView;
                }
                catch {
                    this.CurrentViewIsPlan = false;
                }
                this.CurrentLevel = this.ActiveView.GenLevel;
                try {
                    Parameter parameter = this.ActiveView.get_Parameter(BuiltInParameter.VIEW_PHASE);
                    ElementId elementId = parameter.AsElementId();
                    this.CurrentPhase = this.DocumentDb.GetElement(elementId) as Phase;
                }
                catch {
                    this.CurrentPhase = null;
                }
                   
                // 2015 Deprecated 
                //mElementsPreSelected = mDocumentUi.Selection.Elements;
                this.ElementIdsPreSelected = this.DocumentUi.Selection.GetElementIds();
                this.ElementsPreSelected = new ElementSet();
                foreach (ElementId elementId in this.ElementIdsPreSelected) {
                    this.ElementsPreSelected.Insert(this.DocumentDb.GetElement(elementId));
                } 
               
                // Get Category values for use in combo boxes
                //This is the revised version for 2012 upgrade that solved missing items issues
                //- The first pass, with DocumentDb.Settings.Categories gets most of the values
                //- The second pass, which looks at all the types, gets some more (such as Property Line)
                //We no longer restrict to ones that actually exist in the project.
                FilteredElementCollector collector = new FilteredElementCollector(this.DocumentDb);
                ICollection<Element> elements;

                //Categories that Revit reports
                foreach (Category category in this.DocumentDb.Settings.Categories) {
                    if (category == null) continue;
                    string categoryName = category.Name;
                    if (this.CategoriesByName.ContainsKey(categoryName)) continue;
                    this.CategoriesByName.Add(categoryName, category);
                }

                //Categories that are associated with a type
                elements = collector.WhereElementIsElementType().ToElements();

                foreach (Element element in elements) {
                    if (element.Category == null) continue;
                    string categoryName = element.Category.Name;
                    if (this.CategoriesByName.ContainsKey(categoryName)) continue;
                    this.CategoriesByName.Add(categoryName, element.Category);
                }
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show("Unhandled exception in 'UtilitySettings.RefreshRevitValues'.\nSystem Message: " + exception.Message, this.ProgramName);
                return false;
            }
        }




    }
}
