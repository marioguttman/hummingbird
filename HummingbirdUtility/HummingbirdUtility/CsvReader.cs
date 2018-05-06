using System;
using System.Collections.Generic;
////using System.Linq;
//using System.Text;
using System.Data;

using System.Windows.Forms;
using System.IO;

using DataCsv;

namespace HummingbirdUtility {
    public class CsvReader {

        // *********************************************************** Module Variables ********************************************************
        private string constantProgramName = "HummingbirdUtility";

        public string Version { get { return "2015-04-12"; } }                  // doing it this way to accomodate DesignScript 
        public DataTable DataTable = new DataTable();
        public List<HbInputItem> HbInputItems = null;

        private CsvAdapter csvAdapter;        

        // ************************************************************ Constructor ***********************************************************
        public CsvReader() {
            DataTable.Columns.Add("RowId", typeof(string));
            DataTable.Columns.Add("ElementId", typeof(string));
            DataTable.Columns.Add("Action", typeof(string));
            DataTable.Columns.Add("Object", typeof(string));
            DataTable.Columns.Add("Value01", typeof(string));
            DataTable.Columns.Add("Value02", typeof(string));
            DataTable.Columns.Add("Value03", typeof(string));
            DataTable.Columns.Add("Value04", typeof(string));
        }
        //~CsvWriter() {
        //    GC.Collect();
        //}
        public void Dispose() {
        }

        // ******************************************************* Public Functions - Csv ********************************************************
        public string ConnectToFile(string pathCsvFile) {
            try {
                using (StreamReader streamReader = new StreamReader(pathCsvFile)) {
                    // Just to test if path is OK and can read the file
                }
            }
            catch {
                return "Unable to connect to .csv file. File must exist.";
            }
            csvAdapter = new CsvAdapter(pathCsvFile, this.DataTable, this.constantProgramName);
            if (csvAdapter == null) return "Unable to connect to .csv file: " + pathCsvFile + ". File must exist.";
            else return "";  // Empty string if no error.
        }
        public string ReadFile() {
            if (!csvAdapter.ReadFile()) return "Unable to read file.  Connection must already exist.";
// TODO I don't think this is necessary anymore since we fixed the CsvAdapter to not allow rows with only blanks in them
// This probably isn't hurting anything
            // Handle blank rows in the DataTable.  This is necessary since the CsvAdapter will allow a valid row if
            // it contains a blank value.  We want a blank to mean the end of the data, even if there are some additional 
            // rows that follow, since we use a blank line to stop processing while testing a file with many lines.  In 
            // this case, if a line has all blanks/nulls, then delete it and any remaining lines.
            // Problem also arises with leftover ElementId values when we are saving them but switch to a shorter output.
            int blankRowNumber = 0;
            bool blankRowFound = false;
            for (int i = 0; i < this.DataTable.Rows.Count; i++) {
                DataRow dataRow = this.DataTable.Rows[i];
                if (dataRow[2] is DBNull || dataRow[2].ToString() == "") {  // Looking at Action column
                    blankRowNumber = i;
                    blankRowFound = true;
                    break;
                }
            }
            if (blankRowFound) {
                int rowIndexMax = DataTable.Rows.Count - 1;
                for (int i = rowIndexMax; i >= blankRowNumber; i--) {
                    DataTable.Rows[i].Delete();
                }
            }

            // Fix all of the lower/upper case issues
            FixKeyWordCase();

            return "";
        }
        public string CheckSyntax() {
            SyntaxChecker syntaxChecker = new SyntaxChecker();
            for (int rowIndex = 0; rowIndex < this.DataTable.Rows.Count; rowIndex++) {
                DataRow dataRow = this.DataTable.Rows[rowIndex];
                string result = syntaxChecker.ValidateRow(dataRow);
                if (result != "") return "Row " + rowIndex.ToString() + ": " + result + ".";
            }
            return "";
        }
        public string GetInput() {
            InputParser inputParser = new InputParser(DataTable);
            string returnValue = inputParser.GetInputItems();
            this.HbInputItems = inputParser.InputItems;
            if (returnValue != "") return "Error in InputParser: " + returnValue;
            else return "";
        }

        private void FixKeyWordCase() {
            List<string> validKeyWords = new List<string> {
                "AdaptiveComponent", "AdaptiveComponentType", "Add", "Arc", "Area", "AreaBoundaryLine", "Architectural", "Beam", "BeamJustification", "BeamRotation", "BeamType", "Bottom",
                "Center", "Column", "ColumnHeight", "ColumnMode", "ColumnRotation", "CurtainGridUAdd", "CurtainGridVAdd", "CurveArray",  "CurveArrArray", "CurveByPoints", "DetailArc",
                "DetailEllipse", "DetailLine", "DetailNurbSpline", "Draw", "False", "FamilyExtrusion", "FamilyFlipped", "FamilyInstance", "FamilyMirrored", "FamilyRotation", "FamilyType", 
                "FilledRegion", "FilledRegionType", "Floor", "FloorType",
                "Full", "Grid", "Half", "Level", "Line", "LoftForm", "Model", "ModelArc", "ModelEllipse", "ModelLine", "ModelNurbSpline", "Modify", "MullionType", "MullionUAdd", "MullionVAdd",
                "NurbSpline", "NurbsPointsSet", "Other", "ParameterSet", "Points", "ReferenceArray", "ReferencePoint", "Room", "RoomSeparationLine", "Set", "StructuralVertical", "StructuralPointDriven",
                "StructuralAngleDriven", "Top", "True", "Use", "Wall", "WallHeight", "WallType"};
            foreach (DataRow dataRow in this.DataTable.Rows) {
                if (!validKeyWords.Contains(dataRow[2].ToString())) dataRow[2] = SetCase(dataRow[2].ToString());
                if (!validKeyWords.Contains(dataRow[3].ToString())) dataRow[3] = SetCase(dataRow[3].ToString());
            }
        }

        private string SetCase(string source) {
            switch (source.ToLower()) {
                case "adaptivecomponent": return "AdaptiveComponent";
                case "adaptivecomponenttype": return "AdaptiveComponentType";
                case "add": return "Add";
                case "arc": return "Arc";
                case "area": return "Area";
                case "areaboundaryline": return "AreaBoundaryLine";
                case "architectural": return "Architectural";
                case "beam": return "Beam";
                case "beamjustification": return "BeamJustification";
                case "beamrotation": return "BeamRotation";
                case "beamtype": return "BeamType";
                case "bottom": return "Bottom";
                case "center": return "Center";
                case "column": return "Column";
                case "columnheight": return "ColumnHeight";
                case "columnmode": return "ColumnMode";
                case "columnrotation": return "ColumnRotation";
                case "curtaingriduadd": return "CurtainGridUAdd";
                case "curtaingridvadd": return "CurtainGridVAdd";
                case "curvearray": return "CurveArray";
                case "curvearrarray": return "CurveArrArray";
                case "curvebypoints": return "CurveByPoints";
                case "detailarc": return "DetailArc";
                case "detailellipse": return "DetailEllipse";
                case "detailline": return "DetailLine";
                case "detailnurbspline": return "DetailNurbSpline";
                case "draw": return "Draw";
                case "false": return "False";
                case "familyextrusion": return "FamilyExtrusion";
                case "familyflipped": return "FamilyFlipped";
                case "familyinstance": return "FamilyInstance";
                case "familymirrored": return "FamilyMirrored";
                case "familyrotation": return "FamilyRotation";
                case "familytype": return "FamilyType";
                case "filledregion": return "FilledRegion";
                case "filledregiontype": return "FilledRegionType";
                case "floor": return "Floor";
                case "floortype": return "FloorType";
                case "full": return "Full";
                case "grid": return "Grid";
                case "half": return "Half";
                case "level": return "Level";
                case "line": return "Line";
                case "loftform": return "LoftForm";
                case "model": return "Model";
                case "modelarc": return "ModelArc";
                case "modelellipse": return "ModelEllipse";
                case "modelline": return "ModelLine";
                case "modelnurbspline": return "ModelNurbSpline";
                case "modify": return "Modify";
                case "mulliontype": return "MullionType";
                case "mullionuadd": return "MullionUAdd";
                case "mullionvadd": return "MullionVAdd";
                case "nurbspline": return "NurbSpline";
                case "nurbspointsset": return "NurbsPointsSet";
                case "other": return "Other";
                case "parameterset": return "ParameterSet";
                case "points": return "Points";
                case "referencearray": return "ReferenceArray";
                case "referencepoint": return "ReferencePoint";
                case "room": return "Room";
                case "roomseparationline": return "RoomSeparationLine";
                case "set": return "Set";
                case "structuralvertical": return "StructuralVertical";
                case "structuralpointdriven": return "StructuralPointDriven";
                case "structuralangledriven": return "StructuralAngleDriven";
                case "top": return "Top";
                case "true": return "True";
                case "use": return "Use";
                case "wall": return "Wall";
                case "wallheight": return "WallHeight";
                case "walltype": return "WallType";
                default: return source;
            }
        }

    }
}
