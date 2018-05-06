using System;
using System.Collections.Generic;
////using System.Linq;
//using System.Text;
using System.Data;

using System.Windows.Forms;
using System.IO;

using DataCsv;

namespace HummingbirdUtility {
    public class CsvWriter {

        // *********************************************************** Module Variables ********************************************************
        private string constantProgramName = "HummingbirdUtility";

        public string Version { get { return "2015-04-12"; } }                  // doing it this way to accomodate DesignScript  
        public bool RoundValues {set; get; }
        public int Precision {set; get; }
        public DataTable DataTable { set; get; }
        public string[] ElementIds { set; get; } 

        private CsvAdapter csvAdapter;
        

        private int mIntRowId;
        private int mIntRowPointValue;
        private string mStringRowId;

        // ************************************************************ Constructor ***********************************************************
        public CsvWriter() {
            this.DataTable = new DataTable();
            this.ElementIds = null;
            RoundValues = true;
            Precision = 8;
            DataTable.Columns.Add("RowId", typeof(string));
            DataTable.Columns.Add("ElementId", typeof(string));
            DataTable.Columns.Add("Action", typeof(string));
            DataTable.Columns.Add("Object", typeof(string));
            DataTable.Columns.Add("Value01", typeof(string));
            DataTable.Columns.Add("Value02", typeof(string));
            DataTable.Columns.Add("Value03", typeof(string));
            DataTable.Columns.Add("Value04", typeof(string));
        }
        public CsvWriter(DataTable dataTable) {  // This signature is used to write ElementId values back to the original .csv file
            this.DataTable = dataTable;
            RoundValues = true;
            Precision = 8;
        }
        //~CsvWriter() {
        //    GC.Collect();
        //}

        // ******************************************************* Public Functions - Csv ********************************************************
        public string ConnectToFile(string pathCsvFile) {
            try {
                using (StreamWriter streamWriter = new StreamWriter(pathCsvFile, true)) {  // true for append so it doesn't erase file.
                    // Just to test if we can write to file

                }
            }
            catch {
                return "Unable to create or connect to .csv file. Folder must exist and existing file must not be in use.";
            }
            this.csvAdapter = new CsvAdapter(pathCsvFile, this.DataTable, this.constantProgramName);
            if (csvAdapter == null) return "Unable to create or connect to file: " + pathCsvFile + ". Folder must exist and existing file must not be in use.";
            return "";  // Empty string if no error.
        }
        public string ReadElementIds() {
            // Assuming check for file exist before call
            if (!this.csvAdapter.ReadFile()) return "ReadElementIds() failed.  this.csvAdapter.ReadFile() returned false.";
            try {
                this.ElementIds = new string[this.DataTable.Rows.Count];
                for (int i = 0; i < this.DataTable.Rows.Count; i++) {
                    ElementIds[i] = this.DataTable.Rows[i]["ElementId"].ToString();
                }
                this.csvAdapter.DataTable.Clear();  // Clear the data table so that it is ready for the new data to be written.
            }
            catch (Exception exception) {
                return "Exception in ReadElementIds(): " + exception.Message;
            }            
            return ""; // Empty string if no error.
        }
        public string WriteFile() {
            if (this.ElementIds != null) {
                for (int i = 0; i <= this.ElementIds.GetUpperBound(0); i++) {
                    if (i >= this.DataTable.Rows.Count) break;
                    this.DataTable.Rows[i]["ElementId"] = ElementIds[i];
                }
            }
            if (!this.csvAdapter.WriteFile()) return "csvAdapter.WriteFile() failed.";
            return ""; // Empty string if no error.
        }

        // ********************************************* Public Functions - General Output ****************************************************

        // ****************** Set ********************
        public void SetLevel(string levelName) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Set", "Level");
            outputItem.Value01 = new OutputValue(levelName);
            outputItem.WriteDataRow();
        }
        public void SetWallType(string wallTypeName) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "WallType");
            outputItem.Value01 = new OutputValue(wallTypeName);
            outputItem.WriteDataRow();
        }
        public void SetWallHeight(double height) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "WallHeight");
            outputItem.Value01 = new OutputValue(height, Precision);
            outputItem.WriteDataRow();
        }
        public void SetFloorType(string floorTypeName) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "FloorType");
            outputItem.Value01 = new OutputValue(floorTypeName);
            outputItem.WriteDataRow();
        }
        public void SetFilledRegionType(string filledRegionTypeName) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Set", "FilledRegionType");
            outputItem.Value01 = new OutputValue(filledRegionTypeName);
            outputItem.WriteDataRow();
        }

        public void SetFamilyType(string familyName, string typeName) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "FamilyType");
            outputItem.Value01 = new OutputValue(familyName);
            outputItem.Value02 = new OutputValue(typeName);
            outputItem.WriteDataRow();
        }
        public void SetFamilyFlipped(bool flipHand, bool flipFacing) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "FamilyFlipped");
            outputItem.Value01 = new OutputValue(flipHand);
            outputItem.Value02 = new OutputValue(flipFacing);
            outputItem.WriteDataRow();
        }
        public void SetFamilyMirrored(bool mirrorX, bool mirrorY) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "FamilyMirrored");
            outputItem.Value01 = new OutputValue(mirrorX);
            outputItem.Value02 = new OutputValue(mirrorY);
            outputItem.WriteDataRow();
        }
        public void SetFamilyRotation(double rotation) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "FamilyRotation");
            outputItem.Value01 = new OutputValue(rotation);
            outputItem.WriteDataRow();
        }

        public void  SetColumnMode(string mode, string familyName, string typeName) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "ColumnMode");
            outputItem.Value01 = new OutputValue(mode);
            outputItem.Value02 = new OutputValue(familyName);
            outputItem.Value03 = new OutputValue(typeName);
            outputItem.WriteDataRow();
        }
        public void SetColumnHeight(double height) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "ColumnHeight");
            outputItem.Value01 = new OutputValue(height, Precision);
            outputItem.WriteDataRow();
        }
        public void SetColumnRotation(double rotation) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "ColumnRotation");
            outputItem.Value01 = new OutputValue(rotation, Precision);
            outputItem.WriteDataRow();
        }

        public void SetBeamType(string familyName, string typeName) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "BeamType");
            outputItem.Value01 = new OutputValue(familyName);
            outputItem.Value02 = new OutputValue(typeName);
            outputItem.WriteDataRow();
        }
        public void SetBeamJustification(string justification) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "BeamJustification");
            outputItem.Value01 = new OutputValue(justification);
            outputItem.WriteDataRow();
        }
        public void SetBeamRotation(double rotation) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "BeamRotation");
            outputItem.Value01 = new OutputValue(rotation, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }

        public void SetAdaptiveComponentType(string familyName, string typeName) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "AdaptiveComponentType");
            outputItem.Value01 = new OutputValue(familyName);
            outputItem.Value02 = new OutputValue(typeName);
            outputItem.WriteDataRow();
        }

        public void SetFamilyExtrusionHeight(double height) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Set", "FamilyExtrusionHeight");
            outputItem.Value01 = new OutputValue(height, Precision);
            outputItem.WriteDataRow();
        }

        public void SetMullionType(string familyName, string typeName) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Set", "MullionType");
            outputItem.Value01 = new OutputValue(familyName);
            outputItem.Value02 = new OutputValue(typeName);
            outputItem.WriteDataRow();
        }

        // ****************** Add - Simple **********************
        public void AddGrid(HbXYZ point1, HbXYZ point2) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Grid");  // Line case
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddGrid(HbXYZ point1, HbXYZ point2, HbXYZ point3) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Grid");  // Arc case
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddLevel(double elevation, string name = null) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Level");
            outputItem.Value01 = new OutputValue(elevation, Precision);
            if (name != null) outputItem.Value02 = new OutputValue(name);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddDetailLine(HbXYZ point1, HbXYZ point2) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "DetailLine");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddDetailArc(HbXYZ point1, HbXYZ point2, HbXYZ point3) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "DetailArc");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddDetailEllipse(HbXYZ point1, HbXYZ point2, double radiusY, string mode) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "DetailEllipse");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(radiusY, Precision);
            outputItem.Value04 = new OutputValue(mode);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddDetailNurbsSpline(HbXYZ point1, HbXYZ point2, HbXYZ point3 = null, HbXYZ point4 = null) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "DetailNurbSpline");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.Value04 = new OutputValue(point4, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddModelLine(HbXYZ point1, HbXYZ point2) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "ModelLine");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddModelArc(HbXYZ point1, HbXYZ point2, HbXYZ point3) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "ModelArc");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddModelEllipse(HbXYZ point1, HbXYZ point2, double radiusY, string mode) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "ModelEllipse");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(radiusY, Precision);
            outputItem.Value04 = new OutputValue(mode);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddModelNurbsSpline(HbXYZ point1, HbXYZ point2, HbXYZ point3 = null, HbXYZ point4 = null) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "ModelNurbSpline");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.Value04 = new OutputValue(point4, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }

        public void AddTopographySurface(HbXYZ point1, HbXYZ point2, HbXYZ point3, HbXYZ point4 = null) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "TopographySurface");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.Value04 = new OutputValue(point4, Precision);
            outputItem.WriteDataRow();
        }

        public void AddWall(HbXYZ point1, HbXYZ point2) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Wall"); // Straight line case
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.WriteDataRow();
        }
        public void AddWall(HbXYZ point1, HbXYZ point2, HbXYZ point3) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Wall");  // Arc case
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.WriteDataRow();
        }
        public void AddFloor(HbXYZ point1, HbXYZ point2, HbXYZ point3, HbXYZ point4) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Floor");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.Value04 = new OutputValue(point4, Precision);
            outputItem.WriteDataRow();
        }
        public void AddFilledRegion(HbXYZ point1, HbXYZ point2, HbXYZ point3, HbXYZ point4) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Add", "FilledRegion");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.Value04 = new OutputValue(point4, Precision);
            outputItem.WriteDataRow();
        }
        public void AddFamilyInstance(HbXYZ point1) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "FamilyInstance");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddColumn(HbXYZ point1) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Column");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddColumn(HbXYZ point1, HbXYZ point2) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Column");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddBeam(HbXYZ point1, HbXYZ point2) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Beam");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddAdaptiveComponent(HbXYZ point1, HbXYZ point2 = null, HbXYZ point3 = null, HbXYZ point4 = null) {  // points 2, 3, and 4 are optional
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "AdaptiveComponent");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.Value04 = new OutputValue(point4, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }

        public void AddAreaBoundaryLine(HbXYZ point1, HbXYZ point2) {                                // Line case
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "AreaBoundaryLine");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddAreaBoundaryLine(HbXYZ point1, HbXYZ point2, HbXYZ point3) {             // Arc case
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "AreaBoundaryLine");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddAreaBoundaryLine(HbXYZ point1, HbXYZ point2, double radiusY, string mode) {   // Ellipse case
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "AreaBoundaryLine");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(radiusY, Precision);
            outputItem.Value04 = new OutputValue(mode);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }

        public void AddRoomSeparationLine(HbXYZ point1, HbXYZ point2) {                             // Line case
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "RoomSeparationLine");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddRoomSeparationLine(HbXYZ point1, HbXYZ point2, HbXYZ point3) {            // Arc case
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "RoomSeparationLine");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddRoomSeparationLine(HbXYZ point1, HbXYZ point2, double radiusY, string mode) {// Ellipse case
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "RoomSeparationLine");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(radiusY, Precision);
            outputItem.Value04 = new OutputValue(mode);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }

        public void AddArea(HbXYZ point1) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Area");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddRoom(HbXYZ point1) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Room");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
       
        public void AddReferencePoint(HbXYZ point) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "ReferencePoint");
            outputItem.Value01 = new OutputValue(point, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }

        // ****************** Add - Compound **********************
        public void AddDetailCurves(List<HbCurve> curves) {
            OutputItem outputItem;
            foreach (HbCurve hbItem in curves) {
                switch (hbItem.GetType().Name) {
                    case "HbLine":
                        HbLine hbLine = (HbLine)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "DetailLine");
                        outputItem.Value01 = new OutputValue(hbLine.PointStart, Precision);
                        outputItem.Value02 = new OutputValue(hbLine.PointEnd, Precision);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbArc":
                        HbArc hbArc = (HbArc)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "DetailArc");
                        outputItem.Value01 = new OutputValue(hbArc.PointStart, Precision);
                        outputItem.Value02 = new OutputValue(hbArc.PointEnd, Precision);
                        outputItem.Value03 = new OutputValue(hbArc.PointMid, Precision);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbEllipse":
                        HbEllipse hbEllipse = (HbEllipse)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "DetailEllipse");
                        outputItem.Value01 = new OutputValue(hbEllipse.PointFirst, Precision);
                        outputItem.Value02 = new OutputValue(hbEllipse.PointSecond, Precision);
                        outputItem.Value03 = new OutputValue(hbEllipse.RadiusY, Precision);
                        outputItem.Value04 = new OutputValue(hbEllipse.Mode);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbNurbSpline":
                        HbNurbSpline hbNurbSpline = (HbNurbSpline)hbItem;
                        int count = hbNurbSpline.Points.Count;
                        SetRowId();
                        if (count <= 4) {
                            outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "DetailNurbSpline");
                            if (count > 0) outputItem.Value01 = new OutputValue(hbNurbSpline.Points[0], Precision);
                            if (count > 1) outputItem.Value02 = new OutputValue(hbNurbSpline.Points[1], Precision);
                            if (count > 2) outputItem.Value03 = new OutputValue(hbNurbSpline.Points[2], Precision);
                            if (count > 3) outputItem.Value04 = new OutputValue(hbNurbSpline.Points[3], Precision);
                            outputItem.WriteDataRow();
                            //mRowCurrent++;
                        }
                        else {
                            AddDetailNurbsSpline(hbNurbSpline.Points);
                        }
                        break;
                    default:
                        break; // Ignoring HbXYZ values
                }
            }
        }
        public void AddModelCurves(List<HbCurve> curves) {
            OutputItem outputItem;
            foreach (HbCurve hbItem in curves) {
                switch (hbItem.GetType().Name) {
                    case "HbLine":
                        HbLine hbLine = (HbLine)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "ModelLine");
                        outputItem.Value01 = new OutputValue(hbLine.PointStart, Precision);
                        outputItem.Value02 = new OutputValue(hbLine.PointEnd, Precision);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbArc":
                        HbArc hbArc = (HbArc)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "ModelArc");
                        outputItem.Value01 = new OutputValue(hbArc.PointStart, Precision);
                        outputItem.Value02 = new OutputValue(hbArc.PointEnd, Precision);
                        outputItem.Value03 = new OutputValue(hbArc.PointMid, Precision);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbEllipse":
                        HbEllipse hbEllipse = (HbEllipse)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "ModelEllipse");
                        outputItem.Value01 = new OutputValue(hbEllipse.PointFirst, Precision);
                        outputItem.Value02 = new OutputValue(hbEllipse.PointSecond, Precision);
                        outputItem.Value03 = new OutputValue(hbEllipse.RadiusY, Precision);
                        outputItem.Value04 = new OutputValue(hbEllipse.Mode);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbNurbSpline":
                        HbNurbSpline hbNurbSpline = (HbNurbSpline)hbItem;
                        int count = hbNurbSpline.Points.Count;
                        SetRowId();
                        if (count <= 4) {
                            outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "ModelNurbSpline");
                            if (count > 0) outputItem.Value01 = new OutputValue(hbNurbSpline.Points[0], Precision);
                            if (count > 1) outputItem.Value02 = new OutputValue(hbNurbSpline.Points[1], Precision);
                            if (count > 2) outputItem.Value03 = new OutputValue(hbNurbSpline.Points[2], Precision);
                            if (count > 3) outputItem.Value04 = new OutputValue(hbNurbSpline.Points[3], Precision);
                            outputItem.WriteDataRow();
                            //mRowCurrent++;
                        }
                        else {
                            AddModelNurbsSpline(hbNurbSpline.Points);
                        }
                        break;
                    default:
                        break; // Ignoring HbXYZ values
                }
            }
        }

        public void AddWall(List<HbCurve> curves) {  // A single level list adds multiple simple walls; see list of list for profile version
            OutputItem outputItem;
            foreach (HbCurve hbItem in curves) {
                switch (hbItem.GetType().Name) {
                    case "HbLine":
                        HbLine hbLine = (HbLine)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Wall");
                        outputItem.Value01 = new OutputValue(hbLine.PointStart, Precision);
                        outputItem.Value02 = new OutputValue(hbLine.PointEnd, Precision);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbArc":
                        HbArc hbArc = (HbArc)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Wall");
                        outputItem.Value01 = new OutputValue(hbArc.PointStart, Precision);
                        outputItem.Value02 = new OutputValue(hbArc.PointEnd, Precision);
                        outputItem.Value03 = new OutputValue(hbArc.PointMid, Precision);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbNurbSpline":
                        // Note that Revit walls can't be based on a spline.  This code approximates the shape with lines
                        HbNurbSpline hbNurbSpline = (HbNurbSpline)hbItem;                        
                        int count = hbNurbSpline.Points.Count;
                        if (count < 2) break; // single point wall doesn't make any sense so ignore
                        for (int i = 0; i < count - 1; i++) {
                            SetRowId();
                            outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Wall");
                            outputItem.Value01 = new OutputValue(hbNurbSpline.Points[i], Precision);
                            outputItem.Value02 = new OutputValue(hbNurbSpline.Points[i+1], Precision);
                            outputItem.WriteDataRow();
                            //mRowCurrent++;
                        }
                        break;
                    default:
                        break; // Ignoring HbXYZ values
                }
            }
        }

        public void AddAreaBoundaryLine(List<HbCurve> curves) {                                     // List of lines, arcs, ellipses, or splines in plan
            OutputItem outputItem;
            foreach (HbCurve hbItem in curves) {
                switch (hbItem.GetType().Name) {
                    case "HbLine":
                        HbLine hbLine = (HbLine)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "AreaBoundaryLine");
                        outputItem.Value01 = new OutputValue(hbLine.PointStart, Precision);
                        outputItem.Value02 = new OutputValue(hbLine.PointEnd, Precision);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbArc":
                        HbArc hbArc = (HbArc)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "AreaBoundaryLine");
                        outputItem.Value01 = new OutputValue(hbArc.PointStart, Precision);
                        outputItem.Value02 = new OutputValue(hbArc.PointEnd, Precision);
                        outputItem.Value03 = new OutputValue(hbArc.PointMid, Precision);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbEllipse":
                        HbEllipse hbEllipse = (HbEllipse)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "AreaBoundaryLine");
                        outputItem.Value01 = new OutputValue(hbEllipse.PointFirst, Precision);
                        outputItem.Value02 = new OutputValue(hbEllipse.PointSecond, Precision);
                        outputItem.Value03 = new OutputValue(hbEllipse.RadiusY, Precision);
                        outputItem.Value04 = new OutputValue(hbEllipse.Mode);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbNurbSpline":
                        HbNurbSpline hbNurbSpline = (HbNurbSpline)hbItem;                      
                        AddAreaBoundaryLine(hbNurbSpline.Points);   // Points to Use-Add method
                        break;
                    default:
                        break; // Ignoring HbXYZ values
                }
            }
        }
        public void AddRoomSeparationLine(List<HbCurve> curves) {                                   // List of lines, arcs, ellipses, or splines in plan
            OutputItem outputItem;
            foreach (HbCurve hbItem in curves) {
                switch (hbItem.GetType().Name) {
                    case "HbLine":
                        HbLine hbLine = (HbLine)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "RoomSeparationLine");
                        outputItem.Value01 = new OutputValue(hbLine.PointStart, Precision);
                        outputItem.Value02 = new OutputValue(hbLine.PointEnd, Precision);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbArc":
                        HbArc hbArc = (HbArc)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "RoomSeparationLine");
                        outputItem.Value01 = new OutputValue(hbArc.PointStart, Precision);
                        outputItem.Value02 = new OutputValue(hbArc.PointEnd, Precision);
                        outputItem.Value03 = new OutputValue(hbArc.PointMid, Precision);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbEllipse":
                        HbEllipse hbEllipse = (HbEllipse)hbItem;
                        SetRowId();
                        outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "RoomSeparationLine");
                        outputItem.Value01 = new OutputValue(hbEllipse.PointFirst, Precision);
                        outputItem.Value02 = new OutputValue(hbEllipse.PointSecond, Precision);
                        outputItem.Value03 = new OutputValue(hbEllipse.RadiusY, Precision);
                        outputItem.Value04 = new OutputValue(hbEllipse.Mode);
                        outputItem.WriteDataRow();
                        //mRowCurrent++;
                        break;
                    case "HbNurbSpline":
                        HbNurbSpline hbNurbSpline = (HbNurbSpline)hbItem;
                        AddRoomSeparationLine(hbNurbSpline.Points);   // Points to Use-Add method
                        break;
                    default:
                        break; // Ignoring HbXYZ values
                }
            }
        }

        // ****************** Use-Add *********************
        public void UsePoints(HbXYZ point1, HbXYZ point2 = null, HbXYZ point3 = null, HbXYZ point4 = null) { // points 2, 3, and 4 are optional
            SetRowId(true, false);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Use", "Points");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.Value04 = new OutputValue(point4, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddAreaBoundaryLine() {    
            SetRowId(true, true);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "AreaBoundaryLine");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddDetailNurbsSpline() {
            SetRowId(true, true);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "DetailNurbSpline");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddRoomSeparationLine() {
            SetRowId(true, true);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "RoomSeparationLine");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddModelNurbsSpline() {
            SetRowId(true, true);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "ModelNurbSpline");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddAdaptiveComponent() {
            SetRowId(true, true);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "AdaptiveComponent");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddCurveByPoints() {
            SetRowId(true, true);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "CurveByPoints");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }

        public void AddTopographySurface() {
            SetRowId(true, true);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "TopographySurface");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }

        public void UsePoints(List<HbXYZ> points) {
            //SetRowId(true, false);
            int countPoints = points.Count;
            int countRows = countPoints / 4;
            if (countRows * 4 < countPoints) countRows++;
            for (int i = 0; i < countRows; i++) {
                SetRowId(true, false);
                int index = i * 4;
                OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Use", "Points");
                outputItem.Value01 = new OutputValue(points[index], Precision);
                index++;
                if (index < countPoints) {
                    outputItem.Value02 = new OutputValue(points[index], Precision);
                    index++;
                }
                if (index < countPoints) {
                    outputItem.Value03 = new OutputValue(points[index], Precision);
                    index++;
                }
                if (index < countPoints) {
                    outputItem.Value04 = new OutputValue(points[index], Precision);
                }
                outputItem.WriteDataRow();
                //mRowCurrent++;
            }
            //rowCurrent++;
        }
        public void AddDetailNurbsSpline(List<HbXYZ> points) {
            UsePoints(points);
            AddDetailNurbsSpline();
        }
        public void AddModelNurbsSpline(List<HbXYZ> points) {
            UsePoints(points);
            AddModelNurbsSpline();
        }
        public void AddAdaptiveComponent(List<HbXYZ> points) {
            UsePoints(points);
            AddAdaptiveComponent();
        }
        public void AddAreaBoundaryLine(List<HbXYZ> points) {
            UsePoints(points);
            AddAreaBoundaryLine();
        }
        public void AddRoomSeparationLine(List<HbXYZ> points) {
            UsePoints(points);
            AddRoomSeparationLine();
        }
        public void AddCurveByPoints(List<HbXYZ> points) {
            UsePoints(points);
            AddCurveByPoints();
        }
        public void AddTopographySurface(List<HbXYZ> points) {
            UsePoints(points);
            AddTopographySurface();
        }

        // ****************** Draw-Use-Add *********************
        // Also see UsePoints above
        public void DrawCurveArray() {
            SetRowId(true, false);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Draw", "CurveArray");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void DrawLine(HbXYZ point1, HbXYZ point2) {
            SetRowId(true, false);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Draw", "Line");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void DrawArc(HbXYZ point1, HbXYZ point2, HbXYZ point3) {
            SetRowId(true, false);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Draw", "Arc");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(point3, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void DrawNurbSpline() {  // Must follow a Use-Points set
            SetRowId(true, false);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Draw", "NurbSpline");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void DrawNurbSpline(List<HbXYZ> points) {
            UsePoints(points);
            SetRowId(true, false);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Draw", "NurbSpline");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void DrawNurbSpline(HbXYZ point1, HbXYZ point2, HbXYZ point3 = null, HbXYZ point4 = null) { // points 3 and 4 are optional
            UsePoints(point1, point2, point3, point4);
            SetRowId(true, false);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Draw", "NurbSpline");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        // No current use of 'splitfull' boolean below; only using true case.
        public void DrawEllipse(HbXYZ point1, HbXYZ point2, double radiusY, string mode) {
            DrawEllipse(point1, point2, radiusY, mode, true);
        }
        public void DrawEllipse(HbXYZ point1, HbXYZ point2, double radiusY, string mode, bool splitFull) {
            // There are issues with using a full ellipse with extruded forms like the FamilyExtrusion which (I think) don't like closed forms.
            // Rather than solving this on the ModelBuilder side we make two half-ellipses here so that there will be two distinct ellipses in
            // in order to hold the ElementID.
            SetRowId(true, false);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Draw", "Ellipse");
            outputItem.Value01 = new OutputValue(point1, Precision);
            outputItem.Value02 = new OutputValue(point2, Precision);
            outputItem.Value03 = new OutputValue(radiusY, Precision);
            if (splitFull) {
                outputItem.Value04 = new OutputValue("Half");
            }
            else {
                outputItem.Value04 = new OutputValue(mode);
            }
            outputItem.WriteDataRow();
            if (mode == "Full" && splitFull) {
                outputItem = new OutputItem(this.DataTable, mStringRowId,"Draw", "Ellipse");
                outputItem.Value01 = new OutputValue(point2, Precision);
                outputItem.Value02 = new OutputValue(point1, Precision);
                outputItem.Value03 = new OutputValue(radiusY, Precision);
                outputItem.Value04 = new OutputValue("Half");
                outputItem.WriteDataRow();
            }
        }
        public void DrawHermiteSpline(List<HbXYZ> points) {
            UsePoints(points);
            SetRowId(true, false);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Draw", "HermiteSpline");
            outputItem.WriteDataRow();
        }
        public void AddWall() {  // Must follow a Curve Array Set
            SetRowId(true, true);  // Note that here the RowId root value gets reset.
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Wall");
            outputItem.WriteDataRow();
        }
        public void AddFloor() {  // Must follow a Curve Array Set
            SetRowId(true, true);  // Note that here the RowId root value gets reset.
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Floor");
            outputItem.WriteDataRow();
        }
        public void AddFilledRegion() {  // Must follow a Curve Array Set
            SetRowId(true, true);  // Note that here the RowId root value gets reset.
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Add", "FilledRegion");
            outputItem.WriteDataRow();
        }

        // public void AddFamilyExtrusion(HbXYZ pointInsert) {  is not possible due to ambiguous call but can supply null as a parameter

        public void AddFamilyExtrusion(string nameFamily = null, HbXYZ pointInsert = null) {  // Must follow a Curve Array Set
            SetRowId(true, true);  // Note that here the RowId root value gets reset.
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "FamilyExtrusion");
            outputItem.Value01 = new OutputValue(nameFamily);
            outputItem.Value02 = new OutputValue(pointInsert, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void DrawCurveArray(List<HbCurve> curves) {   // Combined form
            DrawCurveArray();
            foreach (HbCurve drawItem in curves) {
                if (drawItem is HbLine) {
                    HbLine line = (HbLine)drawItem;
                    DrawLine(line.PointStart, line.PointEnd);
                }
                else {
                    if (drawItem is HbArc) {
                        HbArc arc = (HbArc)drawItem;
                        DrawArc(arc.PointStart, arc.PointEnd, arc.PointMid);
                    }
                    else {
                        if (drawItem is HbNurbSpline) {
                            HbNurbSpline nurbSpline = (HbNurbSpline)drawItem;
                            DrawNurbSpline(nurbSpline.Points);
                        }
                        else {
                            if (drawItem is HbEllipse) {
                                HbEllipse hbEllipse = (HbEllipse)drawItem;
                                DrawEllipse(hbEllipse.PointFirst, hbEllipse.PointSecond, hbEllipse.RadiusY, hbEllipse.Mode);
                            }
                            else {  // HermiteSpline case
                                HbHermiteSpline hbHermiteSpline = (HbHermiteSpline)drawItem;
                                DrawHermiteSpline(hbHermiteSpline.Points);
                            }
                        }
                    }
                }
            }
        }
        public void AddWall(List<List<HbCurve>> curvesList) {  // Combined form 
            foreach (List<HbCurve> curves in curvesList) {
                DrawCurveArray(curves);
            }
            SetRowId(true, true);  // Note that here the RowId root value gets reset.
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Wall");
            outputItem.WriteDataRow();
        }
        public void AddFloor(List<List<HbCurve>> curvesList) {  // Combined form 
            foreach (List<HbCurve> curves in curvesList) {
                DrawCurveArray(curves);
            }
            SetRowId(true, true);  // Note that here the RowId root value gets reset.
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "Floor");
            outputItem.WriteDataRow();
        }
        public void AddFilledRegion(List<List<HbCurve>> curvesList) {  // Combined form 
            foreach (List<HbCurve> curves in curvesList) {
                DrawCurveArray(curves);
            }
            SetRowId(true, true);  // Note that here the RowId root value gets reset.
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Add", "FilledRegion");
            outputItem.WriteDataRow();
        }
        public void AddFamilyExtrusion(List<List<HbCurve>> curvesList, HbXYZ pointInsert) {
            AddFamilyExtrusion(curvesList, null, pointInsert);
        }
        public void AddFamilyExtrusion(List<List<HbCurve>> curvesList, string nameFamily = null, HbXYZ pointInsert = null) {
            foreach (List<HbCurve> curves in curvesList) {
                DrawCurveArray(curves);
            }
            SetRowId(true, true);  // Note that here the RowId root value gets reset.
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "FamilyExtrusion");
            outputItem.Value01 = new OutputValue(nameFamily);
            outputItem.Value02 = new OutputValue(pointInsert, Precision);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }

        // ****************** Model-Use-Add *********************
        // Also see UsePoints above
        public void ModelReferenceArray() {
            SetRowId(true, false);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Model", "ReferenceArray");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void AddLoftForm() {
            SetRowId(true, true);  // Note that here the RowId root value gets reset.
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Add", "LoftForm");
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }
        public void ModelReferenceArray(List<HbXYZ> points) {
            SetRowId(true, false);
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Model", "ReferenceArray");
            outputItem.WriteDataRow();
            //mRowCurrent++;
            UsePoints(points);
        }
        public void AddLoftForm(List<List<HbXYZ>> pointsOuterList) {            
            foreach (List<HbXYZ> innerList in pointsOuterList) {
                ModelReferenceArray(innerList);
            }
            AddLoftForm();
        }


        // ****************** Modify **********************

        public void ModifyParameterSet(string parameterName, string value) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId,"Modify", "ParameterSet");
            outputItem.Value01 = new OutputValue(parameterName);
            outputItem.Value02 = new OutputValue(value);
            outputItem.WriteDataRow();
            //mRowCurrent++;
        }

        public void ModifyCurtainGridUAdd(double offsetPrimary) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Modify", "CurtainGridUAdd");
            outputItem.Value01 = new OutputValue(offsetPrimary);
            outputItem.WriteDataRow();
        }
        public void ModifyCurtainGridUAdd(double offsetPrimary, double offsetSecondary) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Modify", "CurtainGridUAdd");
            outputItem.Value01 = new OutputValue(offsetPrimary);
            outputItem.Value02 = new OutputValue(offsetSecondary);
            outputItem.WriteDataRow();
        }
        public void ModifyCurtainGridVAdd(double offsetPrimary) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Modify", "CurtainGridVAdd");
            outputItem.Value01 = new OutputValue(offsetPrimary);
            outputItem.WriteDataRow();
        }
        public void ModifyCurtainGridVAdd(double offsetPrimary, double offsetSecondary) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Modify", "CurtainGridVAdd");
            outputItem.Value01 = new OutputValue(offsetPrimary);
            outputItem.Value02 = new OutputValue(offsetSecondary);
            outputItem.WriteDataRow();
        }
        public void ModifyMullionUAdd(double offsetPrimary) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Modify", "MullionUAdd");
            outputItem.Value01 = new OutputValue(offsetPrimary);
            outputItem.WriteDataRow();
        }
        public void ModifyMullionUAdd(double offsetPrimary, double offsetSecondary) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Modify", "MullionUAdd");
            outputItem.Value01 = new OutputValue(offsetPrimary);
            outputItem.Value02 = new OutputValue(offsetSecondary);
            outputItem.WriteDataRow();
        }
        public void ModifyMullionVAdd(double offsetPrimary) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Modify", "MullionVAdd");
            outputItem.Value01 = new OutputValue(offsetPrimary);
            outputItem.WriteDataRow();
        }
        public void ModifyMullionVAdd(double offsetPrimary, double offsetSecondary) {
            SetRowId();
            OutputItem outputItem = new OutputItem(this.DataTable, mStringRowId, "Modify", "MullionVAdd");
            outputItem.Value01 = new OutputValue(offsetPrimary);
            outputItem.Value02 = new OutputValue(offsetSecondary);
            outputItem.WriteDataRow();
        }


        // ************************************************************ Utility Functions *******************************************************
        private void SetRowId() {
            SetRowId(false, false);
        }
        private void SetRowId(bool usePointValue, bool nextRootValue) {
            if (usePointValue) {
                mStringRowId = mIntRowId.ToString() + "." + mIntRowPointValue.ToString();
                if (nextRootValue) {
                    mIntRowId++;
                    mIntRowPointValue = 0;
                }
                else mIntRowPointValue++;
            }
            else {
                mStringRowId = mIntRowId.ToString();
                mIntRowId++;
            }
        }

    }
}
