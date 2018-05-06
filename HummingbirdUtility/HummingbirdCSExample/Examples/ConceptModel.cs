using System;
using System.Collections.Generic;

using System.Windows.Forms;
using System.IO;

using HummingbirdUtility;

namespace HummingbirdCSExample {
    class ConceptModel {

        // ****************************************** Module Variables ************************************************
        private const string FILE_NAME_POINTS = "ConceptPoints.csv";
        private const string FILE_NAME_CURVES = "ConceptCurves.csv";
        private const string FILE_NAME_FORM = "ConceptForm.csv";

        public int GridSizeX;
        public int GridSizeY;
        public double CellSizeX;
        public double CellSizeY;
        public double FactorGrowX;
        public double FactorGrowY;
        public double FactorHeight;

        private string csvFolderPath;
        private bool preserveId;

        // ********************************************* Constructor *****************************************************
        public ConceptModel(string csvFolderPath, bool preserveId) {
            this.csvFolderPath = csvFolderPath;
            this.preserveId = preserveId;
        }

        // ************************************************** Public Functions ***********************************************
        public bool CreatePoints() {
            string filePath = this.csvFolderPath + FILE_NAME_POINTS;
            CsvWriter csvWriter = new CsvWriter();
            string returnValue;
            returnValue = csvWriter.ConnectToFile(filePath);
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

            double spaceX, spaceY, height;

            for (int indexX = 0; indexX <= GridSizeX; indexX++) {
                List<HbXYZ> curve = new List<HbXYZ>();
                spaceX = indexX * FactorGrowX + CellSizeX;
                double lineX = indexX * spaceX;
                for (int indexY = 0; indexY <= GridSizeY; indexY++) {
                    height = CalculateHeight(indexX, indexY);
                    spaceY = indexY * FactorGrowY + CellSizeY;
                    double lineY = indexY * spaceY;
                    HbXYZ point = new HbXYZ(lineX, lineY, height);
                    csvWriter.AddReferencePoint(point);
                }

            }

            csvWriter.WriteFile();

            return true;
        }

        public bool CreateCurveByPoints() {
            string filePath = this.csvFolderPath + FILE_NAME_CURVES;
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

            double spaceX, spaceY, height;

            for (int indexX = 0; indexX <= GridSizeX; indexX++) {
                List<HbXYZ> curve = new List<HbXYZ>();
                spaceX = indexX * FactorGrowX + CellSizeX;
                double lineX = indexX * spaceX;
                for (int indexY = 0; indexY <= GridSizeY; indexY++) {
                    height = CalculateHeight(indexX, indexY);
                    spaceY = indexY * FactorGrowY + CellSizeY;
                    double lineY = indexY * spaceY;
                    HbXYZ point = new HbXYZ(lineX, lineY, height);
                    curve.Add(point);
                }
                csvWriter.AddCurveByPoints(curve);
            }

            csvWriter.WriteFile();

            return true;
        }

        public bool CreateLoftForm() {
            string filePath = this.csvFolderPath + FILE_NAME_FORM;
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

            double spaceX, spaceY, height;

            List<List<HbXYZ>> curves = new List<List<HbXYZ>>();
            for (int indexX = 0; indexX <= GridSizeX; indexX++) {
                List<HbXYZ> curve = new List<HbXYZ>();
                spaceX = indexX * FactorGrowX + CellSizeX;
                double lineX = indexX * spaceX;
                for (int indexY = 0; indexY <= GridSizeY; indexY++) {
                    height = CalculateHeight(indexX, indexY);
                    spaceY = indexY * FactorGrowY + CellSizeY;
                    double lineY = indexY * spaceY;
                    HbXYZ point = new HbXYZ(lineX, lineY, height);
                    curve.Add(point);
                }
                curves.Add(curve);
            }
            csvWriter.AddLoftForm(curves);

            csvWriter.WriteFile();

            return true;
        }

        private double CalculateHeight(int x, int y) {
            return (Math.Sin(x) + Math.Cos(y)) * FactorHeight;
        }


    }
}
