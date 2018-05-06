using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;

namespace HummingbirdUtility {
    public class GetHbValue {

        public string Text { set;  get; }   // May not need to be public

        private NumberFormatInfo provider = new NumberFormatInfo();

        // ****************************************** Constructor ****************************************************
        public GetHbValue(string source) {
            this.Text = source;
            this.provider.NumberDecimalSeparator = ".";
        }

        // **************
        public string AsString() {           // Don't really need but included for completeness
            return Text;
        }
        public bool? AsBoolean() {           // Not sure we are using this
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
                return Convert.ToInt32(Text);
            }
            catch {
                return null;
            }
        }
        public double? AsDouble() {                        
            try {
                //return double.Parse(Text, NumberStyles.Any, CultureInfo.InvariantCulture);
                return Convert.ToDouble(Text, this.provider);
            }
            catch {
                return null;
            }
        }
        public HbXYZ AsHbXYZ() {
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
                //x = double.Parse(stringX, NumberStyles.Any, CultureInfo.InvariantCulture);
                //y = double.Parse(stringY, NumberStyles.Any, CultureInfo.InvariantCulture);
                //z = double.Parse(stringZ, NumberStyles.Any, CultureInfo.InvariantCulture);
                x = Convert.ToDouble(stringX, this.provider);
                y = Convert.ToDouble(stringY, this.provider);
                z = Convert.ToDouble(stringZ, this.provider);
            }
            catch {
                return null;
            }
            return new HbXYZ(x, y, z);
        }
        public HbElementId AsHbElementId() {
            string stringInput = Text.Trim();
            if (stringInput == "") return null;
            int elementInt;
            try {
                elementInt = Convert.ToInt32(stringInput);
            }
            catch {
                return null;
            }
            return new HbElementId(elementInt);
        }

    }
}
