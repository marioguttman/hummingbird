using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RevitUtility {
    public class ControlSetting {

        // ********************************************** Public Variables ******************************************

        public string ControlName { get { return mControlName; } }
        public string ValueCurrent { set { mValueCurrent = value; } get { return mValueCurrent; } }
        public string ValueDefault { get { return mValueDefault; } }

        // ********************************************** Module variables *******************************************

        private string mControlName;   
        private string mValueCurrent;
        private string mValueDefault;

        // *********************************************** Constructor **********************************************
        public ControlSetting(string controlName, string valueCurrent, string valueDefault) {
            mControlName = controlName;
            mValueCurrent = valueCurrent;
            mValueDefault = valueDefault;
        }

        // ************************************************ Public Functions ******************************************

        public bool SetControl(System.Windows.Forms.Form parentForm) {
            try {
                System.Windows.Forms.Control controlToSet = UtilityStatic.FindControlOnForm(parentForm, mControlName);
                if (controlToSet == null) {
                    System.Windows.Forms.MessageBox.Show(
                        "Unable to find control named: '" + mControlName + "' in ControlSetting.SetControl.");
                    return false;
                }
                switch (controlToSet.GetType().Name) {
                    case "Label":
                        System.Windows.Forms.Label label = (System.Windows.Forms.Label)controlToSet;
                        label.Text = mValueCurrent;
                        break;
                    case "TextBox":
                        System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)controlToSet;
                        textBox.Text = mValueCurrent;
                        break;
                    case "ComboBox":
                        System.Windows.Forms.ComboBox comboBox = (System.Windows.Forms.ComboBox)controlToSet;
                        comboBox.Text = mValueCurrent;
                        break;
                    case "ListBox":
                        System.Windows.Forms.ListBox listBox = (System.Windows.Forms.ListBox)controlToSet;
                        if (listBox.Items.Contains(mValueCurrent)) listBox.Text = mValueCurrent;
                        break;
                    case "CheckBox":
                        System.Windows.Forms.CheckBox checkBox = (System.Windows.Forms.CheckBox)controlToSet;
                        if (mValueCurrent == "true") checkBox.Checked = true;
                        else checkBox.Checked = false;
                        break;
                    case "RadioButton":
                        System.Windows.Forms.RadioButton radioButton = (System.Windows.Forms.RadioButton)controlToSet;
                        if (mValueCurrent == "true") radioButton.Checked = true;
                        else radioButton.Checked = false;
                        break;
                    case "GroupBox":
                        System.Windows.Forms.GroupBox groupBox = (System.Windows.Forms.GroupBox)controlToSet;
                        System.Windows.Forms.RadioButton radioButtonInGroup;
                        foreach (System.Windows.Forms.Control controlTest in groupBox.Controls) {
                            if (controlTest.GetType() == typeof(System.Windows.Forms.RadioButton)) {
                                radioButtonInGroup = (System.Windows.Forms.RadioButton)controlTest;
                                if (radioButtonInGroup.Name == mValueCurrent) {
                                    radioButtonInGroup.Checked = true;
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        System.Windows.Forms.MessageBox.Show(
                            "Unknown case in switch statement in ControlSetting.SetControl");
                        return true;
                }
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                return false;
            }

        }
        public bool SaveControl(System.Windows.Forms.Form parentForm) {
            try {
                System.Windows.Forms.Control controlToSet = null;
                controlToSet = UtilityStatic.FindControlOnForm(parentForm, mControlName);
                if (controlToSet == null) return false;
                switch (controlToSet.GetType().Name) {
                    case "Label":
                        System.Windows.Forms.Label label = (System.Windows.Forms.Label)controlToSet;
                        mValueCurrent = label.Text;
                        break;
                    case "TextBox":
                        System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)controlToSet;
                        mValueCurrent = textBox.Text;
                        break;
                    case "ComboBox":
                        System.Windows.Forms.ComboBox comboBox = (System.Windows.Forms.ComboBox)controlToSet;
                        mValueCurrent = comboBox.Text;
                        break;
                    case "ListBox":
                        System.Windows.Forms.ListBox listBox = (System.Windows.Forms.ListBox)controlToSet;
                        mValueCurrent = listBox.Text;
                        break;
                    case "CheckBox":
                        System.Windows.Forms.CheckBox checkBox = (System.Windows.Forms.CheckBox)controlToSet;
                        if (checkBox.Checked) mValueCurrent = "true";
                        else mValueCurrent = "false";
                        break;
                    case "RadioButton":
                        System.Windows.Forms.RadioButton radioButton = (System.Windows.Forms.RadioButton)controlToSet;
                        if (radioButton.Checked) mValueCurrent = "true";
                        else mValueCurrent = "false";
                        break;
                    case "GroupBox":
                        System.Windows.Forms.GroupBox groupBox = (System.Windows.Forms.GroupBox)controlToSet;
                        System.Windows.Forms.RadioButton radioButtonInGroup;
                        foreach (System.Windows.Forms.Control controlTest in groupBox.Controls) {
                            if (controlTest.GetType() == typeof(System.Windows.Forms.RadioButton)) {
                                radioButtonInGroup = (System.Windows.Forms.RadioButton)controlTest;
                                if (radioButtonInGroup.Checked) {
                                    mValueCurrent = radioButtonInGroup.Name;
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        System.Windows.Forms.MessageBox.Show(
                            "Unknown case in switch statement in ControlSetting.SaveControl.");
                        return false;
                }
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                return false;
            }
            return true;
        }

        public void RestoreDefault() {
            mValueCurrent = mValueDefault;
        }


    }
}
