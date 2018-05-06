using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace HummingbirdUtility {
    class OutputValue {
        public string Text { set; get; }

        //Constructors
        public OutputValue(string source) {  // ??? Is this handling null case? Probably OK but I don't like writing null value to Excel so avoid using
            Text = source;
        }
        public OutputValue(int source) {
            Text = source.ToString();
        }
        public OutputValue(double source) { // Not using this form but leave for completeness
            Text = source.ToString();
        }
        public OutputValue(double source, int precision) {
            Text = Round(source, precision).ToString();
        }
        //public OutputValue(ElementId source) {
        //    Text = source.ToString();
        //}
        public OutputValue(HbXYZ source) {
            Text = source.X.ToString() + ", " + source.Y.ToString() + ", " + source.Z.ToString();
        }
        public OutputValue(HbXYZ source, int precision) {
            if (source == null) Text = "";
            else Text = Round(source.X, precision) + ", " + Round(source.Y, precision) + ", " + Round(source.Z, precision);
        }
        public OutputValue(bool source) {
            if (source) Text = "True";
            else Text = "False";
        }

        private string Round(double source, int precision) {
            if (precision < 0) return source.ToString();
            string stringValue = Math.Round(source, precision).ToString();
            if (precision > 0) {
                int positionDot = stringValue.LastIndexOf(".");
                if (positionDot == -1) {
                    stringValue = stringValue + ".";
                    positionDot = stringValue.Length - 1;
                }
                int numberOfDecimals = (stringValue.Length - positionDot) - 1;
                int zerosNeeded = precision - numberOfDecimals;
                for (int i = 0; i < zerosNeeded; i++) {
                    stringValue = stringValue + "0";
                }
            }
            return stringValue;
        }
    }
}
