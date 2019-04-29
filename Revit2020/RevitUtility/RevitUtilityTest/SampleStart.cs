using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using RevitUtility;

namespace RevitUtilityTest {
    public partial class SampleStart : Form {

        //********************************************** Module Variables ****************************************************
        private UtilitySettings utilitySettings;

        //************************************************* Constructor *******************************************************

        public SampleStart(UtilitySettings utilitySettings) {
            InitializeComponent();
            this.utilitySettings = utilitySettings;

            //Put some sample data in the list controls just for illustrative purposes.
            listBox1.Items.Add("Default");
            listBox1.Items.Add("Second Item");
            listBox1.Items.Add("Third Item");
            comboBox1.Items.Add("Default");
            comboBox1.Items.Add("Second Item");
            comboBox1.Items.Add("Third Item");

            //*** Key Statements - Open Condition ***
            //These statements set variables
            string returnValue = "";
            if (!this.utilitySettings.GetVariableSetting("SampleValue", ref returnValue)) {
                MessageBox.Show("Error in this.utilitySettings.GetVariableSetting.", this.utilitySettings.ProgramName);
                return;
            }
            labelDisplayValue.Text = returnValue;
            if (!this.utilitySettings.GetVariableSetting("NamedSettingGroup", ref returnValue)) {
                MessageBox.Show("Error in this.utilitySettings.GetVariableSetting.", this.utilitySettings.ProgramName);
                return;
            }
            //this.utilitySettings.NamedSettingGroupName = returnValue;
  
            //These set control values.
            if (!this.utilitySettings.SetFormControls(this)) {
                MessageBox.Show("Invalid setting value used with this.utilitySettings.SetFormControls.", this.utilitySettings.ProgramName);
                return;
            }
        }


        //************************************************ Event Handlers *****************************************************

        //The default values are reloaded into whichever (or none) Named Setting Group is current.
        private void buttonReloadDefaults_Click(object sender, EventArgs e) {
            this.utilitySettings.ReloadDefaultValues();
            this.utilitySettings.SetFormControls(this);
        }

        //An explicit close button is used in order to clean up some data.  In particular,
        //we want to record whether a named setting group was in use into the main .ini file
        //so that state will reload automatically at next use of the program.
        private void buttonClose_Click(object sender, EventArgs e) {

            //*** Key Statement - Close Condition ***
            //This statement is the key component of saving the settings to a .ini file.
            //A statement like this is necessary with every form that uses saved settings.
            //Depending on the logic, it may be necessary to explicitly save varaible values at this point as well.
            //We typically do not write the ini here, instead doing that at the final close of Main().
            this.utilitySettings.SaveFormControls(this);
            Close();
        }


        private void buttonShowVariable_Click(object sender, EventArgs e) {
            string returnValue = "";
            if (! this.utilitySettings.GetVariableSetting("SampleValue", ref returnValue)) {
                System.Windows.Forms.MessageBox.Show("Error in 'this.utilitySettings.GetVariableSetting'.", this.utilitySettings.ProgramName);
                return;
            }
            if (returnValue == "default value") {
                if (!this.utilitySettings.SetVariableSetting("SampleValue", "toggle")) {
                    System.Windows.Forms.MessageBox.Show("Error in 'this.utilitySettings.SetVariableSetting'.", this.utilitySettings.ProgramName);                   
                    return;
                }
                labelDisplayValue.Text = "toggle";
            }
            else {
                if (!this.utilitySettings.SetVariableSetting("SampleValue", "default value")) {
                    System.Windows.Forms.MessageBox.Show("Error in 'this.utilitySettings.SetVariableSetting'.", this.utilitySettings.ProgramName);                    
                    return;
                }
                labelDisplayValue.Text = "default value";
            }
        }

        private void buttonSetLabel_Click(object sender, EventArgs e) {
            //DateTime.Now.ToString("dd/MM/yyyy h:MM:ss tt")
            //yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffzz
            labelSample.Text = labelSample.Text.Substring(0, 13) + " " + DateTime.Now.ToString("h:MM:ss tt");


        }
    }
}
