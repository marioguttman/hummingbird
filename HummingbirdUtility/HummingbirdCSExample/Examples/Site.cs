using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.IO;

using HummingbirdUtility;

namespace HummingbirdCSExample {
    class Site {

        // ****************************************** Module Variables ************************************************
        private const string FILE_NAME_TOPO_SURF = "TopoSurf.csv";
        private const string FILE_NAME_BUILDINGS = "Buildings.csv";

        public double SizeX;
        public double SizeY;
        public int DivisionsX;
        public int DivisionsY;
        public double ElevTopLeft;
        public double ElevBotLeft;
        public double ElevTopRight;
        public double ElevBotRight;

        public double RandomFactor;

        private string csvFolderPath;
        private bool preserveId;

        // ********************************************* Constructor *****************************************************
        public Site(string csvFolderPath, bool preserveId) {
            this.csvFolderPath = csvFolderPath;
            this.preserveId = preserveId;
        }

        // ************************************************** Public Functions ***********************************************
        public bool CreateFamilyExtrusions() {
            string filePath = this.csvFolderPath + FILE_NAME_BUILDINGS;
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

            //double segmentX = SizeX / DivisionsX;
            //double segmentY = SizeY / DivisionsY;
            double segmentX = 5;
            double segmentY = 5;

            // Boundary values
            double slopeTop = (ElevTopRight - ElevTopLeft) / SizeX;
            double slopeBot = (ElevBotRight - ElevBotLeft) / SizeX;
            double[] elevsTop = new double[DivisionsX];
            double[] elevsBot = new double[DivisionsX];
            for (int indexX = 0; indexX < DivisionsX; indexX++) {
                elevsTop[indexX] = ElevTopRight + indexX * segmentX * slopeTop;
                elevsBot[indexX] = ElevBotRight + indexX * segmentX * slopeBot;
            }

            // Arbitray approximation for height
            double elevTop = Math.Max(Math.Max(ElevTopLeft, ElevTopRight), Math.Max(ElevBotLeft, ElevBotRight));
            double elevBot = Math.Min(Math.Min(ElevTopLeft, ElevTopRight), Math.Min(ElevBotLeft, ElevBotRight));
            double height = (elevTop - elevBot);  // Arbitrary

            // Place three buildings
            double currentY = (SizeY * 0.50) - (4 * segmentY);
            double currentX;
            int currentDivisionX; 
            double elevation;
            currentX = (SizeX * 0.25) - (4 * segmentX);
            currentDivisionX = Convert.ToInt32(Convert.ToDouble(DivisionsX) * 0.25); // using nearest integer
            elevation = ((elevsTop[currentDivisionX] + elevsBot[currentDivisionX]) / 2) - (height * 0.25);
            if (!CreateOneFamilyExtrusion(csvWriter, segmentX, segmentY, elevation, height, currentX, currentY)) return false;
            currentX = (SizeX * 0.50) - (4 * segmentX);
            currentDivisionX = Convert.ToInt32(Convert.ToDouble(DivisionsX) * 0.50); // using nearest integer
            elevation = ((elevsTop[currentDivisionX] + elevsBot[currentDivisionX]) / 2) - (height * 0.25);
            if (!CreateOneFamilyExtrusion(csvWriter, segmentX, segmentY, elevation, height, currentX, currentY)) return false;
            currentX = (SizeX * 0.75) - (4 * segmentX);
            currentDivisionX = Convert.ToInt32(Convert.ToDouble(DivisionsX) * 0.75); // using nearest integer
            elevation = ((elevsTop[currentDivisionX] + elevsBot[currentDivisionX]) / 2) - (height * 0.25);
            if (!CreateOneFamilyExtrusion(csvWriter, segmentX, segmentY, elevation, height, currentX, currentY)) return false;

            csvWriter.WriteFile();

            return true;
        }


        public bool CreateOneFamilyExtrusion(CsvWriter csvWriter, double segmentX, double segmentY, double elevation, double height, double currentX, double currentY) {

            HbXYZ point01 = new HbXYZ(currentX + segmentX * 1, currentY + segmentY * 2, elevation);
            HbXYZ point02 = new HbXYZ(currentX + segmentX * 3, currentY + segmentY * 2, elevation);
            HbXYZ point03 = new HbXYZ(currentX + segmentX * 4, currentY + segmentY * 0, elevation);
            HbXYZ point04 = new HbXYZ(currentX + segmentX * 5, currentY + segmentY * 2, elevation);
            HbXYZ point05 = new HbXYZ(currentX + segmentX * 7, currentY + segmentY * 2, elevation);
            HbXYZ point06 = new HbXYZ(currentX + segmentX * 7, currentY + segmentY * 3, elevation);
            HbXYZ point07 = new HbXYZ(currentX + segmentX * 8, currentY + segmentY * 4, elevation);
            HbXYZ point08 = new HbXYZ(currentX + segmentX * 7, currentY + segmentY * 5, elevation);
            HbXYZ point09 = new HbXYZ(currentX + segmentX * 7, currentY + segmentY * 6, elevation);            
            HbXYZ point10 = new HbXYZ(currentX + segmentX * 5, currentY + segmentY * 7, elevation);
            HbXYZ point11 = new HbXYZ(currentX + segmentX * 4, currentY + segmentY * 6, elevation);
            HbXYZ point12 = new HbXYZ(currentX + segmentX * 3, currentY + segmentY * 7, elevation);
            HbXYZ point13 = new HbXYZ(currentX + segmentX * 1, currentY + segmentY * 6, elevation);
            HbXYZ point14 = new HbXYZ(currentX + segmentX * 1, currentY + segmentY * 5, elevation);
            HbXYZ point15 = new HbXYZ(currentX + segmentX * 0, currentY + segmentY * 4, elevation);
            HbXYZ point16 = new HbXYZ(currentX + segmentX * 1, currentY + segmentY * 3, elevation);

            HbXYZ point17 = new HbXYZ(currentX + segmentX * 2, currentY + segmentY * 4, elevation);
            HbXYZ point18 = new HbXYZ(currentX + segmentX * 6, currentY + segmentY * 4, elevation);

            HbXYZ point19 = new HbXYZ(currentX + segmentX * 1.0, currentY + segmentY * 3.5, elevation);
            HbXYZ point20 = new HbXYZ(currentX + segmentX * 0.5, currentY + segmentY * 4.0, elevation);
            HbXYZ point21 = new HbXYZ(currentX + segmentX * 1.0, currentY + segmentY * 4.5, elevation);
            HbXYZ point22 = new HbXYZ(currentX + segmentX * 1.5, currentY + segmentY * 4.0, elevation);

            HbXYZ point23 = new HbXYZ(currentX + segmentX * 7.0, currentY + segmentY * 3.5, elevation);
            HbXYZ point24 = new HbXYZ(currentX + segmentX * 6.5, currentY + segmentY * 4.0, elevation);
            HbXYZ point25 = new HbXYZ(currentX + segmentX * 7.0, currentY + segmentY * 4.5, elevation);
            HbXYZ point26 = new HbXYZ(currentX + segmentX * 7.5, currentY + segmentY * 4.0, elevation);

            List<List<HbCurve>> curvesCurvesList = new List<List<HbCurve>>();
            List<HbCurve> curvesList;            
            HbLine line;
            HbArc arc;
            HbNurbSpline spline;
            HbEllipse ellipse;
            HbHermiteSpline hermiteSpline;

            // Outer Loop
            curvesList = new List<HbCurve>();
            spline = new HbNurbSpline();
            spline.Points = new List<HbXYZ> { point01, point02, point03, point04, point05 };
            curvesList.Add(spline);

            line = new HbLine(); line.PointStart = point05; line.PointEnd = point06;
            curvesList.Add(line);
            arc = new HbArc(); arc.PointStart = point06; arc.PointMid = point07; arc.PointEnd = point08;
            curvesList.Add(arc);
            line = new HbLine(); line.PointStart = point08; line.PointEnd = point09;
            curvesList.Add(line);

            hermiteSpline = new HbHermiteSpline();
            hermiteSpline.Points = new List<HbXYZ> { point09, point10, point11, point12, point13 };
            curvesList.Add(hermiteSpline);

            line = new HbLine(); line.PointStart = point13; line.PointEnd = point14;
            curvesList.Add(line);
            arc = new HbArc(); arc.PointStart = point14; arc.PointMid = point15; arc.PointEnd = point16;
            curvesList.Add(arc);
            line = new HbLine(); line.PointStart = point16; line.PointEnd = point01;
            curvesList.Add(line);            
            curvesCurvesList.Add(curvesList);

            // Ellipse
            curvesList = new List<HbCurve>();
            ellipse = new HbEllipse(); ellipse.PointFirst = point17; ellipse.PointSecond = point18; ellipse.RadiusY = segmentY; ellipse.Mode = "Full";
            curvesList.Add(ellipse);      
            curvesCurvesList.Add(curvesList);

            // Circles
            curvesList = new List<HbCurve>();
            arc = new HbArc(); arc.PointStart = point19; arc.PointMid = point20; arc.PointEnd = point21;
            curvesList.Add(arc);
            arc = new HbArc(); arc.PointStart = point21; arc.PointMid = point22; arc.PointEnd = point19;
            curvesList.Add(arc);
            curvesCurvesList.Add(curvesList);
            curvesList = new List<HbCurve>();
            arc = new HbArc(); arc.PointStart = point23; arc.PointMid = point24; arc.PointEnd = point25;
            curvesList.Add(arc);
            arc = new HbArc(); arc.PointStart = point25; arc.PointMid = point26; arc.PointEnd = point23;
            curvesList.Add(arc);
            curvesCurvesList.Add(curvesList);

            // Extrusion
            csvWriter.SetFamilyExtrusionHeight(height);
            csvWriter.AddFamilyExtrusion(curvesCurvesList, "Bldg", new HbXYZ(0, 0, 0));

            return true;
        }

        public bool CreateTopographySurface() {
            string filePath = this.csvFolderPath + FILE_NAME_TOPO_SURF;
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

            Random random = new System.Random();
            double segmentX = SizeX / DivisionsX;
            double segmentY = SizeY / DivisionsY;
            double currentX, currentY;
            double elevTop =  Math.Max( Math.Max(ElevTopLeft, ElevTopRight), Math.Max(ElevBotLeft, ElevBotRight));
            double elevBot =  Math.Min( Math.Min(ElevTopLeft, ElevTopRight), Math.Min(ElevBotLeft, ElevBotRight));
            int elevRange = Convert.ToInt32((elevTop - elevBot) / 10) + 1;  // An arbitrary value to use with the random factor

            // Boundary values
            double slopeTop = (ElevTopRight - ElevTopLeft) / SizeX;
            double slopeBot = (ElevBotRight - ElevBotLeft) / SizeX;
            double[] elevsTop = new double[DivisionsX];
            double[] elevsBot = new double[DivisionsX];
            for (int indexX = 0; indexX < DivisionsX; indexX++) {
                elevsTop[indexX] = ElevTopRight + indexX * segmentX * slopeTop;
                elevsBot[indexX] = ElevBotRight + indexX * segmentX * slopeBot;
            }

            // Points in rectangle
            List<HbXYZ> points = new List<HbXYZ>();
            for (int indexX = 0; indexX < DivisionsX; indexX++) {                
                currentX = indexX * segmentX;
                double factorX = Math.Sin((currentX / SizeX) * Math.PI);
                //double factorX = Math.Sin((currentX / SizeX) * Math.PI * 2);
                double slopeVertical = (elevsTop[indexX] - elevsBot[indexX]) / SizeY;
                for (int indexY = 0; indexY < DivisionsY; indexY++) {
                    currentY = indexY * segmentY;
                    double factorY = Math.Cos((currentY / SizeY) * Math.PI);
                    //double factorY = Math.Cos((currentY / SizeY) * Math.PI * 2);
                    //double randomDouble = random.Next(0, elevRange);
                    double currentZ = elevsBot[indexX] + (indexY * segmentY * slopeVertical) + (RandomFactor * elevRange * (factorX * factorY + random.NextDouble() / 10));
                    points.Add(new HbXYZ(currentX, currentY, currentZ));
                }
            }

            // TopographySurface
            csvWriter.AddTopographySurface(points);

            csvWriter.WriteFile();

            return true;
        }

    }
}
