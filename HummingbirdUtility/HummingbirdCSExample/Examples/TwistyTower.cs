using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.IO;

using HummingbirdUtility;

namespace HummingbirdCSExample {
    class TwistyTower {

        // ****************************************** Module Variables ************************************************
        private const string FILE_NAME_STRUCTURE = "TwistStruct.csv";
        private const string FILE_NAME_FLOORS = "TwistFloors.csv";
        private const string FILE_NAME_WALLS = "TwistWalls.csv";
        private const string FILE_NAME_ADAPTIVE_COMPONENT = "TwistAdap.csv";
        private const string ADAPTIVE_COMPONENT_TYPE = "AdapTwist";

        private double constantFloorThickness = 1.0;    //1'-0"
        private double constantWallThickness = 0.6667;  // 8"

        public int NumberFloors { set; get; }
        public double FloorHeight { set; get; }
        public double TaperFactor { set; get; }  // TaperFactor: amount of total taper overall (0 < x < 1)         
        public double TwistFactor { set; get; }         // TwistFactor: the amount of twist overall (0 < x < unlimited) 1.0 is one revolution.

        private string csvFolderPath;
        private bool preserveId;

        // ********************************************* Constructor *****************************************************
        public TwistyTower(string csvFolderPath, bool preserveId) {

            this.csvFolderPath = csvFolderPath;
            this.preserveId = preserveId;

            // Set default values (This is supposed to be unnessary)
            NumberFloors = 20;
            FloorHeight = 10;
            TaperFactor = 0.5;
            TwistFactor = 0.1;
        }

        // ************************************************** Public Functions ***********************************************
        public bool CreateStructure() {
            string filePath = this.csvFolderPath + FILE_NAME_STRUCTURE;
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

            csvWriter.SetBeamJustification("Top");

            HbXYZ point1A, point2A, point3A, point4A;
            HbXYZ point1B = new HbXYZ(0, 0, 0); HbXYZ point2B = new HbXYZ(0, 0, 0);
            HbXYZ point3B = new HbXYZ(0, 0, 0); HbXYZ point4B = new HbXYZ(0, 0, 0);
            //mModelBuilder.  beam tops?
            //double elevationFloor, elevationWall; //, x1, x2, x3, x4, y1, y2, y3, y4;
            XY p1, p2, p3, p4;
            for (int i = 0; i < NumberFloors; i++) {
                //Calculate(i, out elevationFloor, out elevationWall, out p1, out p2, out p3, out p4);
                double elevationBeam = i * FloorHeight - constantFloorThickness;
                Calculate(i, - 4 * constantWallThickness, out p1, out p2, out p3, out p4);
                point1A = new HbXYZ(p1.X, p1.Y, elevationBeam);
                point2A = new HbXYZ(p2.X, p2.Y, elevationBeam);
                point3A = new HbXYZ(p3.X, p3.Y, elevationBeam);
                point4A = new HbXYZ(p4.X, p4.Y, elevationBeam);
                if (i != 0) {
                    csvWriter.AddColumn(point1B, point1A);
                    csvWriter.AddColumn(point2B, point2A);
                    csvWriter.AddColumn(point3B, point3A);
                    csvWriter.AddColumn(point4B, point4A);
                }
                csvWriter.AddBeam(point1A, point2A);
                csvWriter.AddBeam(point2A, point3A);
                csvWriter.AddBeam(point3A, point4A);
                csvWriter.AddBeam(point4A, point1A);
                point1B = new HbXYZ(point1A.X, point1A.Y, point1A.Z);
                point2B = new HbXYZ(point2A.X, point2A.Y, point2A.Z);
                point3B = new HbXYZ(point3A.X, point3A.Y, point3A.Z);
                point4B = new HbXYZ(point4A.X, point4A.Y, point4A.Z);
            }
            csvWriter.WriteFile();
            return true;
        }

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

            HbXYZ point1, point2, point3, point4;
            csvWriter.SetWallHeight(FloorHeight);
            XY p1, p2, p3, p4;
            for (int i = 0; i < NumberFloors; i++) {
                double elevationFloor = i * FloorHeight;
                Calculate(i, 0, out p1, out p2, out p3, out p4);
                point1 = new HbXYZ(p1.X, p1.Y, elevationFloor);
                point2 = new HbXYZ(p2.X, p2.Y, elevationFloor);
                point3 = new HbXYZ(p3.X, p3.Y, elevationFloor);
                point4 = new HbXYZ(p4.X, p4.Y, elevationFloor);
                csvWriter.AddFloor(point1, point2, point3, point4);
            }
            csvWriter.WriteFile();
            return true;
        }

        public bool CreateWalls() {
            string filePath = this.csvFolderPath + FILE_NAME_WALLS;
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

            HbXYZ point1, point2, point3, point4;
            csvWriter.SetWallHeight(FloorHeight - constantFloorThickness);
            XY p1, p2, p3, p4;
            for (int i = 1; i < NumberFloors; i++) {
                double elevationWall = (i - 1) * FloorHeight;
                Calculate(i, - 2 * constantWallThickness, out p1, out p2, out p3, out p4);
                point1 = new HbXYZ(p1.X ,p1.Y, elevationWall);
                point2 = new HbXYZ(p2.X, p2.Y, elevationWall);
                point3 = new HbXYZ(p3.X, p3.Y, elevationWall);
                point4 = new HbXYZ(p4.X, p4.Y, elevationWall);
                csvWriter.AddWall(point1, point2);
                csvWriter.AddWall(point2, point3);
                csvWriter.AddWall(point3, point4);
                csvWriter.AddWall(point4, point1);
            }
            csvWriter.WriteFile();
            return true;
        }

        public bool CreateAdaptiveComponents() {
            string filePath = this.csvFolderPath + FILE_NAME_ADAPTIVE_COMPONENT;
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

            HbXYZ point1A, point2A, point3A, point4A;
            HbXYZ point1B = new HbXYZ(0, 0, 0); HbXYZ point2B = new HbXYZ(0, 0, 0);
            HbXYZ point3B = new HbXYZ(0, 0, 0); HbXYZ point4B = new HbXYZ(0, 0, 0);
            csvWriter.SetAdaptiveComponentType(ADAPTIVE_COMPONENT_TYPE, ADAPTIVE_COMPONENT_TYPE);
            XY p1, p2, p3, p4;
            for (int i = 0; i < NumberFloors; i++) {
                double elevationWall = i * FloorHeight;
                Calculate(i, 0, out p1, out p2, out p3, out p4);
                point1A = new HbXYZ(p1.X, p1.Y, elevationWall);
                point2A = new HbXYZ(p2.X, p2.Y, elevationWall);
                point3A = new HbXYZ(p3.X, p3.Y, elevationWall);
                point4A = new HbXYZ(p4.X, p4.Y, elevationWall);
                if (i != 0) {
                    csvWriter.AddAdaptiveComponent(point1A, point2A, point2B, point1B);
                    csvWriter.AddAdaptiveComponent(point2A, point3A, point3B, point2B);
                    csvWriter.AddAdaptiveComponent(point3A, point4A, point4B, point3B);
                    csvWriter.AddAdaptiveComponent(point4A, point1A, point1B, point4B);
                }
                point1B = new HbXYZ(point1A.X, point1A.Y, point1A.Z);
                point2B = new HbXYZ(point2A.X, point2A.Y, point2A.Z);
                point3B = new HbXYZ(point3A.X, point3A.Y, point3A.Z);
                point4B = new HbXYZ(point4A.X, point4A.Y, point4A.Z);
            }
            csvWriter.WriteFile();
            return true;
        }

        // ******************************************************* Private Functions ********************************************************88
        private void Calculate(int i, double offset, out XY p1, out XY p2, out XY p3, out XY p4) {
            // offset positive value moves point outward
            
            // Assuming fixed base values:
            double baseX = 100;
            double baseY = 100;
            XY center = new XY(0, 0);

            // Taper: 
            // Taper Factor: amount of total taper overall (0 < x < 1) 
            double doubleI = i;
            double doubleFloors = NumberFloors - 1;
            // Taper curve factor: gives a slight inward curve to the taper.
            // Value goes from 1.0 to taperCurveIntensity to 1.0 as i goes from 0 to NumberFloors - 1
            double taperCurveIntensity = 0.2;  // Amount that the curve affects the taper; 0 means no effect; 0.2 is a good amount
            double taperCurveFactor = (1.0 - Math.Cos((doubleI / doubleFloors) * (Math.PI / 2))) * taperCurveIntensity;
            // Current taper factor is the amount to subtract from the floor dimentsion at this level.
            double currentTaperFactor = ((doubleI / doubleFloors) * (TaperFactor - taperCurveFactor));
            double currentSizeX = baseX * (1 - currentTaperFactor) + 2 * offset;
            double currentSizeY = baseY * (1 - currentTaperFactor) + 2 * offset;

            // Twist:  Note effect is currently constant for each level
            double currentTwist = ((i * TwistFactor) / NumberFloors ) * 2 * Math.PI;  // angle this level in radians
            double halfX = currentSizeX / 2;
            double halfY = currentSizeY / 2;
            XY p1Initial = new XY(center.X - halfX, center.Y - halfY);
            XY p2Initial = new XY(center.X + halfX, center.Y - halfY);
            XY p3Initial = new XY(center.X + halfX, center.Y + halfY);
            XY p4Initial = new XY(center.X - halfX, center.Y + halfY);
            p1 = p1Initial.Rotate(center, currentTwist);
            p2 = p2Initial.Rotate(center, currentTwist);
            p3 = p3Initial.Rotate(center, currentTwist);
            p4 = p4Initial.Rotate(center, currentTwist);
        }



    }
}
