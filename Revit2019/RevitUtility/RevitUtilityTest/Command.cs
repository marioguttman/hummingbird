using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using RevitUtility;

namespace RevitUtilityTest {
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class Command : IExternalCommand {

        // ************************************************* Module Variables ********************************************
        private const string PROGRAM_NAME = "RevitUtilityTest";
        private const string INI_FILE_NAME = "RevitUtilityTest";

        private UtilitySettings utilitySettings;


        // *********************************************** Execute ***************************************************************
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet) {
            try {

                //Create the setting object, add the settings defined below, and try to read any existing .ini file.
                this.utilitySettings = new UtilitySettings(commandData, ref message, elementSet, PROGRAM_NAME, INI_FILE_NAME);
                if (!this.utilitySettings.ValidInstallation) return Result.Succeeded;

                InitializeSettings();
                if (!this.utilitySettings.ReadIniFile()) this.utilitySettings.ReloadDefaultValues();

                // TODO This is poor solution to eliminate automatic transctions.  Should move deeper into functions
                using (Transaction transaction = new Transaction(this.utilitySettings.DocumentDb)) {
                    transaction.Start("Testing RevitUtility");

                    SampleStart dialog = new SampleStart(this.utilitySettings);
                    dialog.ShowDialog();

                    transaction.Commit();
                }

                //Capture the final state into the main ini file even when a named group is in use.
                //Note that named group settings are not saved unless the user explicitly saves them
                //if (!this.utilitySettings.SetVariableSetting("NamedSettingGroup", this.utilitySettings.NamedSettingGroupName)) {
                //    System.Windows.Forms.MessageBox.Show("Error in this.utilitySettings.SetVariableSetting()");
                //    return Result.Failed;
                //}
                if (!this.utilitySettings.WriteIniFile()) {
                    System.Windows.Forms.MessageBox.Show("Error in this.utilitySettings.WriteIniFile()");
                    return Result.Failed;
                }
                return Result.Succeeded;
            }
            catch (Exception e) {
                message = e.Message;
                return Result.Failed;
            }
        }



        private void InitializeSettings() {
        //This function is where the actual variables, controls and their default values are defined.
        //Variables:
        this.utilitySettings.AddVariableSetting("SampleValue", "default value");
        this.utilitySettings.AddVariableSetting("NamedSettingGroup", "");
            


        //Controls:  
        //In this example there is only one
        //form but if there were others they woudl also be listed here.

            //Note on Radio Buttons:  Radio button groups may be set by specifically setting each button in its own line.  The
            //program needs to insure that both the true and the false values are set in the right order.
            //Alternatively (and preferable) a group box format is used.  In this form the group name and the name of the default radio
            //button are listed.  There can be any number of radio buttons in the group.

            //Single Line Format:  This form is allowed for cases where there are only a small number of settings.  Typically,
            //the array format below is more convenient with multiple settings.  (The radio button group form is not available.)
            //Single Line - Typical call to single value control:
            //    mthis.utilitySettings.AddControlSetting("SampleStart",    "SampleUseParamList1",      "textBoxParameterList1",    "Number");
            //    mthis.utilitySettings.AddControlSetting("SampleStart",    "SampleUseInclude1",        "checkBoxParameterList1",   "true");
            //    mthis.utilitySettings.AddControlSetting("SampleStart",    "SampleUseOption1",         "radioButtonOption1",       "true");
            //    mthis.utilitySettings.AddControlSetting("SampleStart",    "SampleUseOption2",         "radioButtonOption2",       "false");
            
            //Array Format:  Used to create multiple settings in a single command:
            //    mthis.utilitySettings.AddControlSettingArray("SampleStart", new string[,] {
            //    { "SampleStartParamList1",      "textBoxParameterList1",      "Number" },
            //    { "SampleStartInclude1",        "checkBoxParameterList1",     "true" },
            //    {"SampleStartGroupBox",         "groupBox1",                  "radioButton1" }
            //    } );

            //Form: "SampleStart":
            this.utilitySettings.AddControlSettingArray("SampleStart", new string[,] {
            { "SampleStartLabel",              "labelSample",          "Default Label" },
            { "SampleStartTextBox1",           "textBox1",             "Default text 1" },
            { "SampleStartTextBox2",           "textBox2",             "Default text 2" },
            { "SampleStartTextBox3",           "textBox3",             "" },
            { "SampleStartComboBox1",          "comboBox1",            "Default" },
            { "SampleStartComboBox2",          "comboBox2",            "" },
            { "SampleStartListBox1",           "listBox1",             "Default" },
            { "SampleStartCheckBox1",          "checkBox1",            "true" },
            { "SampleStartCheckBox2",          "checkBox2",            "false" },
            { "SampleStartGroupBox",           "groupBox1",            "radioButton1" }
            });
        }
    }
}
