using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;


//using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace RevitModelBuilder {

    public class InputValue {

        #region Module Variables                        // ****************************** Module Variables ******************************************

        public string Text { set;  get; }
        private NumberFormatInfo provider = new NumberFormatInfo();

        #endregion

        #region Constructor                             // ****************************** Constructor ***********************************************

        public InputValue(string source) {
            Text = source;
            this.provider.NumberDecimalSeparator = ".";
        }

        #endregion

        #region Public Functions                        // ****************************** Public Functions ******************************************
        public string AsString() {
            return Text;
        }
        public bool? AsBoolean() {
            try {
               if (Text.ToLower() == "true") return true;
               else {
                   if (Text.ToLower() == "false") return false;
                   else return null;
               }                
            }
            catch {
                return null;
            }
        }
        public int? AsInteger() {
            try {
                // Commennted below since I don't think we are using
                //// These occur when setting parameters since users are used to seeing values like yes/no or true/false but 
                //// Revit will interpret these as 1/0 since the paramter is actually an integer type.
                //if (Text.ToLower() == "yes") return 1;
                //if (Text.ToLower() == "no") return 0;
                //if (Text.ToLower() == "true") return 1;
                //if (Text.ToLower() == "false") return 0;
                //if (Text.ToLower() == "y") return 1;
                //if (Text.ToLower() == "n") return 0;
                //if (Text.ToLower() == "t") return 1;
                //if (Text.ToLower() == "f") return 0;
                //if (Text.ToLower() == "x") return 1;
                //if (Text.ToLower() == "") return 0;
                return Convert.ToInt32(Text);
            }
            catch {
                return null;
            }
        }
        public double? AsDouble() {
            try {
                return Convert.ToDouble(Text, this.provider);
            }
            catch {
                return null;
            }
        }
        public XYZ AsXYZ() {
            string stringInput = Text.Trim();
            if (stringInput == "") return null;
            int position = stringInput.IndexOf(",");
            if (position < 1) return null;   //At least one character before ","
            if (stringInput.Length < position + 1) return null; //At least on e character after ","
            string stringX = stringInput.Substring(0, position).Trim();
            stringInput = stringInput.Substring(position + 1);
            position = stringInput.IndexOf(",");
            if (position < 1) return null;   //At least one character before ","
            if (stringInput.Length < position + 1) return null; //At least one character after ","
            string stringY = stringInput.Substring(0, position).Trim();
            string stringZ = stringInput.Substring(position + 1).Trim();
            Double x, y, z;
            try {
                x = Convert.ToDouble(stringX, this.provider);
                y = Convert.ToDouble(stringY, this.provider);
                z = Convert.ToDouble(stringZ, this.provider);
            }
            catch {
                return null;
            }
            return new XYZ(x, y, z);
        }
        public ElementId AsElementId() {
            string stringInput = Text.Trim();
            if (stringInput == "") return null;
            int elementInt;
            try {
                elementInt = Convert.ToInt32(stringInput);
            }
            catch {
                return null;
            }
            return new ElementId(elementInt);
        }

        #endregion

    }
}
