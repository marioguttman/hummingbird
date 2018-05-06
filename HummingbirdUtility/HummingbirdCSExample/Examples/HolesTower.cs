using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.IO;

using HummingbirdUtility;

namespace HummingbirdCSExample {
    class HolesTower {

        // ****************************************** Module Variables ************************************************
        private const string FILE_NAME_FLOORS = "HolesFloors.csv";

        private string csvFolderPath;

        private bool preserveId;

        // ********************************************* Constructor *****************************************************
        public HolesTower(string csvFolderPath, bool preserveId) {
            this.csvFolderPath = csvFolderPath;
            this.preserveId = preserveId;
        }

        // ************************************************** Public Functions ***********************************************
        public bool CreateFloors() {

            string filePath = this.csvFolderPath + FILE_NAME_FLOORS;
            CsvWriter csvWriter = new CsvWriter();
            string returnValue = csvWriter.ConnectToFile(filePath);
            if (returnValue != "") {
                MessageBox.Show(returnValue);
                return false;
            }
            if (this.preserveId) {
                // Note that if file doesn't exist yet we ignore option to preserve ID values
                if (File.Exists(filePath)) {
                    try {
                        returnValue = csvWriter.ReadElementIds();
                        if (returnValue != "") {
                            MessageBox.Show(returnValue);
                            return false;
                        }
                    }
                    catch (Exception exception) {
                        MessageBox.Show("Exception at csvWriter.ReadElementIds(): " + exception.Message);
                        return false;
                    }
                }
            }

            HbXYZ point1,point2, point3, point4;
            int floors = 60;
            for (int j = 0; j < floors; j++) {
                double elevationFloor = j * 10;
                double w = 100;
                double a = 1 + (floors - j) / 3;
                double b = (w - 3 * a) / 2;
                double c = 2 * a + b;

                point1 = new HbXYZ(0, 0, elevationFloor);
                point2 = new HbXYZ(w, 0, elevationFloor);
                point3 = new HbXYZ(w, w, elevationFloor);
                point4 = new HbXYZ(0, w, elevationFloor);
                csvWriter.DrawCurveArray();  // Outer loop always comes first
                csvWriter.DrawLine(point1, point2);
                csvWriter.DrawLine(point2, point3);
                csvWriter.DrawLine(point3, point4);
                csvWriter.DrawLine(point4, point1);

                point1 = new HbXYZ(0 + a, 0 + a, elevationFloor);
                point2 = new HbXYZ(a + b, 0 + a, elevationFloor);
                point3 = new HbXYZ(a + b, a + b, elevationFloor);
                point4 = new HbXYZ(0 + a, a + b, elevationFloor);
                csvWriter.DrawCurveArray();
                csvWriter.DrawLine(point1, point2);
                csvWriter.DrawLine(point2, point3);
                csvWriter.DrawLine(point3, point4);
                csvWriter.DrawLine(point4, point1);

                point1 = new HbXYZ(0 + c, 0 + a, elevationFloor);
                point2 = new HbXYZ(c + b, 0 + a, elevationFloor);
                point3 = new HbXYZ(c + b, a + b, elevationFloor);
                point4 = new HbXYZ(0 + c, a + b, elevationFloor);
                csvWriter.DrawCurveArray();
                csvWriter.DrawLine(point1, point2);
                csvWriter.DrawLine(point2, point3);
                csvWriter.DrawLine(point3, point4);
                csvWriter.DrawLine(point4, point1);

                point1 = new HbXYZ(0 + a, 0 + c, elevationFloor);
                point2 = new HbXYZ(a + b, 0 + c, elevationFloor);
                point3 = new HbXYZ(a + b, c + b, elevationFloor);
                point4 = new HbXYZ(0 + a, c + b, elevationFloor);
                csvWriter.DrawCurveArray();
                csvWriter.DrawLine(point1, point2);
                csvWriter.DrawLine(point2, point3);
                csvWriter.DrawLine(point3, point4);
                csvWriter.DrawLine(point4, point1);

                point1 = new HbXYZ(0 + c, 0 + c, elevationFloor);
                point2 = new HbXYZ(c + b, 0 + c, elevationFloor);
                point3 = new HbXYZ(c + b, c + b, elevationFloor);
                point4 = new HbXYZ(0 + c, c + b, elevationFloor);
                csvWriter.DrawCurveArray();
                csvWriter.DrawLine(point1, point2);
                csvWriter.DrawLine(point2, point3);
                csvWriter.DrawLine(point3, point4);
                csvWriter.DrawLine(point4, point1);

                csvWriter.AddFloor();                 
            }

            csvWriter.WriteFile();

            return true;
        }
    }
}
