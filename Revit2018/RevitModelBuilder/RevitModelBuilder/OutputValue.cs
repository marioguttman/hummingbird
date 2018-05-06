
using System;

using Autodesk.Revit.DB;

namespace RevitModelBuilder {

    class OutputValue {

        #region Module Variables                        // ****************************** Module Variables ******************************************

        public string Text { set;  get; }

        #endregion

        #region Constructors                            // ****************************** Constructors **********************************************
        public OutputValue(string source) {
            Text = source;
        }
        public OutputValue(int source) {
            Text = source.ToString();
        }
        //public OutputValue(double source) { // Not using this form but leave for completeness
        //    Text = source.ToString();
        //}
        public OutputValue(double source, int precision) {
            Text = Round(source, precision).ToString();
        }
        public OutputValue(ElementId source) {
            Text = source.ToString();
        }
        //public OutputValue(XYZ source) { // Not using this form but leave for completeness
        //    Text = source.X.ToString() + ", " + source.Y.ToString() + ", " + source.Z.ToString();
        //}
        public OutputValue(XYZ source, int precision) {
            Text = Round(source.X, precision) + ", " + Round(source.Y, precision) + ", " + Round(source.Z, precision);
        }

        #endregion

        #region Private Functions                       // ****************************** Private Functions *****************************************

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

        #endregion
    }
}
