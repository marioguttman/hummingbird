using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RevitUtility {
    public class VariableSetting {

        // *********************************************** Constructor **********************************************
        public VariableSetting(string valueCurrent, string valueDefault) {
            mValueCurrent = valueCurrent;
            mValueDefault = valueDefault;
        }


        // ********************************************** Public Variables ******************************************
        public string ValueCurrent { set { mValueCurrent = value; } get { return mValueCurrent; } }
        public string ValueDefault { get { return mValueDefault; } }

        // ********************************************** Module variables ******************************************
        private string mValueCurrent;
        private string mValueDefault;

        // ************************************************ Public Functions ******************************************

        public void RestoreDefault() {
            mValueCurrent = mValueDefault;
        }


    }
}
