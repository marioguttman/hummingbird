
using System;
using System.Collections.Generic;

using RevitUtility;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace RevitModelBuilder {

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Command : IExternalCommand {

        // ********************************************************* Module Variables     ******************************************************

        private const string PROGRAM_NAME = "Model Builder";
        private const string INI_FILE_NAME = "ModelBuilder";

        private UtilitySettings settings;
        private UtilityElements elements;

        // ************************************************************ Constructor ************************************************************

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet) {
            try {

                //Create the setting object, add the settings defined below, and try to read any existing .ini file.
                this.settings = new UtilitySettings(commandData, ref message, elementSet, PROGRAM_NAME, INI_FILE_NAME);
                if (this.settings.Cancelled) return Result.Succeeded;
                if (!this.settings.ValidInstallation) {
                    message = "This version of Hummingbird is not compatible with this version of Revit.  Program cannot continue.";
                    return Result.Failed;
                }

                // Save the current selection since the process of creating UtilityElements causes it to be lost (apparently whenever you do a transaction?)
                ICollection<ElementId> currentSelectionSet = this.settings.DocumentUi.Selection.GetElementIds();

                // Now create the utility elements
                this.elements = new UtilityElements(this.settings);
                InitializeSettings initializeSettings = new InitializeSettings(this.settings);
                if (!this.settings.ReadIniFile()) this.settings.ReloadDefaultValues();

                // open the main menu as modal dialog
                Menu menu = new Menu(this.settings, this.elements, currentSelectionSet);
                menu.ShowDialog();

                // Return control when menu is closed
                return Result.Succeeded;

            }
            catch (Exception e) {
                message = e.Message;
                return Result.Failed;
            }
        }
    }


    // ****************************************************************************************************************************************
    // *********************************************************** Initialize Settings Class **************************************************
    // ****************************************************************************************************************************************

    class InitializeSettings {
        public InitializeSettings(UtilitySettings utilitySettings) {

            //ExportElements
            utilitySettings.AddControlSettingArray("ExportElements", new string[,] {
            { "ExportElementsPathCsvFolder",        "textBoxPathCsvFolder",             "" },
            { "ExportElementsCsvFileName",          "textBoxCsvFileName",               "" },
            { "ExportElementsMode",                 "groupBoxSelection",                "radioButtonSelectCurrent" },
            { "ExportElementsConvert",              "groupBoxConvert",                  "radioButtonConvertNone" },
            { "ExportElementsRoundPoints",          "checkBoxRoundPoints",              "true"},
            { "ExportElementsDecimals",             "comboBoxDecimals",                 "2" },
            { "ExportElementsListErrors",           "checkBoxListErrors",               "true" },
            { "ExportElementsMaxErrors",            "textBoxMaxErrors",                 "10" },
            { "ExportElementsAutoClose",            "checkBoxAutoClose",                "false"}
            });

            //CreateElements
            utilitySettings.AddControlSettingArray("CreateElements", new string[,] {
            { "CreateElementsPathFolder",           "textBoxPathFolder",                 "" },
            { "CreateElementsMode",                 "groupBoxMode",                     "radioButtonModeAdd" },
            { "CreateElementsUnits",                "groupBoxUnits",                    "radioButtonUnitsProject" },
            { "CreateElementsUnitsFactor",          "textBoxUnitsFactor",               "1.0" },
            { "CreateElementsOriginX",              "textBoxOriginX",                   "0.0" },
            { "CreateElementsOriginY",              "textBoxOriginY",                   "0.0" },
            { "CreateElementsOriginZ",              "textBoxOriginZ",                   "0.0" },
            { "CreateElementsSuppressMessages",     "checkBoxSuppressMessages",         "true" },
            { "CreateElementsListErrors",           "checkBoxListErrors",               "true" },
            { "CreateElementsMaxErrors",            "textBoxMaxErrors",                 "10" },
            { "CreateElementsFamilyFamily",         "comboBoxFamilyFamily",             "" },
            { "CreateElementsFamilyType",           "comboBoxFamilyType",               "" },
            { "CreateElementsColumnMode",           "comboBoxColumnMode",               "Architectural" },
            { "CreateElementsColumnArchFamily",     "comboBoxColumnArchFamily",         "" },
            { "CreateElementsColumnArchType",       "comboBoxColumnArchType",           "" },
            { "CreateElementsColumnStructFamily",   "comboBoxColumnStructFamily",       "" },
            { "CreateElementsColumnStructType",     "comboBoxColumnStructType",         "" },
            { "CreateElementsBeamFamily",           "comboBoxBeamFamily",               "" },
            { "CreateElementsBeamType",             "comboBoxBeamType",                 "" },
            { "CreateElementsAdaptiveCompFamily",   "comboBoxAdaptiveCompFamily",       "" },
            { "CreateElementsAdaptiveCompType",     "comboBoxAdaptiveCompType",         "" },
            { "CreateElementsMullionFamily",        "comboBoxMullionFamily",            "" },
            { "CreateElementsMullionType",          "comboBoxMullionType",              "" },
            { "CreateElementsAutoClose",            "checkBoxAutoClose",                "false"},
            { "CreateElementsPathTemplate",         "textBoxPathTemplate",              "" },
            { "CreateElementsTempFolder",           "textBoxTempFolder",                "" } 
            });

        }
    }
}
