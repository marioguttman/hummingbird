using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.IO;

using HummingbirdUtility;

namespace HummingbirdCSExample {
    class SpiralForm {

        // ****************************************** Module Variables ************************************************
        private const string FILE_NAME_GRID = "SpiralGrid.csv";
        private const string FILE_NAME_LINE = "SpiralLine.csv";
        private const string FILE_NAME_ARC = "SpiralArc.csv";
        private const string FILE_NAME_WALL = "SpiralWall.csv";
        private XY centerPoint = null;  // Actually a constant but needs to be initialized 

        public double FactorA { set; get; }
        public double FactorB { set; get; }
        public int NumberOfPoints{ set; get; }

        private string csvFolderPath;
        private bool preserveId;

        // ********************************************* Constructor *****************************************************
        public SpiralForm(string csvFolderPath, bool preserveId) {

            //mModelBuilder.Precision = 6;
            this.csvFolderPath = csvFolderPath;
            this.preserveId = preserveId;

            // Set default values (Maybe not necessary?)
            centerPoint = new XY(0, 0);
            FactorA = 1.0;
            FactorB = 1.0;
            NumberOfPoints = 10;
        }

        
        // ************************************************** Public Functions ***********************************************
        public bool CreateGrids() {
            string filePath = this.csvFolderPath + FILE_NAME_GRID;
            CsvWriter csvWriter = new CsvWriter();
            csvWriter.Precision = 4;
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

            Spiral spiral = new Spiral(this.centerPoint);
            spiral.FactorA = FactorA;
            spiral.FactorB = FactorB;
            HbXYZ currentPoint;
            HbXYZ centerPoint = new HbXYZ(0.0, 0.0, 0.0);
            for (int i = 0; i <= NumberOfPoints; i++) {
                XY xy = spiral.CalculatePoint(i);
                currentPoint = new HbXYZ(2 * xy.X, 2 * xy.Y, 0.0);
                csvWriter.AddGrid(centerPoint, currentPoint);
                csvWriter.ModifyParameterSet("Name", (i + 1).ToString());
            }
            csvWriter.WriteFile();
            return true;
        }
        public bool CreateLines() {
            string filePath = this.csvFolderPath + FILE_NAME_LINE;
            CsvWriter csvWriter = new CsvWriter();
            csvWriter.Precision = 4;
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

            Spiral spiral = new Spiral(centerPoint);
            spiral.FactorA = FactorA;
            spiral.FactorB = FactorB;
            HbXYZ currentPoint, lastPoint = null;
            for (int i = 0; i < NumberOfPoints; i++) {
                XY xy = spiral.CalculatePoint(i);
                currentPoint = new HbXYZ(xy.X, xy.Y, 0.0);
                if (i > 0) csvWriter.AddModelLine(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }
            csvWriter.WriteFile();
            return true;
        }
        public bool CreateArcs() {
            string filePath = this.csvFolderPath + FILE_NAME_ARC;
            CsvWriter csvWriter = new CsvWriter();
            csvWriter.Precision = 4;
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

            Spiral spiral = new Spiral(centerPoint);
            spiral.FactorA = FactorA;
            spiral.FactorB = FactorB;
            for (int i = 0; i < NumberOfPoints; i++) {
                XY xyStart = spiral.CalculatePoint(i);
                XY xyEnd = spiral.CalculatePoint(i + 1);
                XY xyMid = spiral.CalculatePoint(i + 0.5);
                HbXYZ xyzStart = new HbXYZ(xyStart.X, xyStart.Y, 0.0);
                HbXYZ xyzEnd = new HbXYZ(xyEnd.X, xyEnd.Y, 0.0);
                HbXYZ xyzMid = new HbXYZ(xyMid.X, xyMid.Y, 0.0);
                csvWriter.AddModelArc(xyzStart, xyzEnd, xyzMid);
            }
            csvWriter.WriteFile();
            return true;
        }
        public bool CreateWalls() {
            string filePath = this.csvFolderPath + FILE_NAME_WALL;
            CsvWriter csvWriter = new CsvWriter();
            csvWriter.Precision = 4;
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

            Spiral spiral = new Spiral(centerPoint);
            spiral.FactorA = FactorA;
            spiral.FactorB = FactorB;
            for (int i = 0; i < NumberOfPoints; i++) {
                XY xyStart = spiral.CalculatePoint(i);
                XY xyEnd = spiral.CalculatePoint(i + 1);
                XY xyMid = spiral.CalculatePoint(i + 0.5);
                HbXYZ xyzStart = new HbXYZ(xyStart.X, xyStart.Y, 0.0);
                HbXYZ xyzEnd = new HbXYZ(xyEnd.X, xyEnd.Y, 0.0);
                HbXYZ xyzMid = new HbXYZ(xyMid.X, xyMid.Y, 0.0);
                csvWriter.AddWall(xyzStart, xyzEnd, xyzMid);
            }
            csvWriter.WriteFile();
            return true;
        }

    }
}
