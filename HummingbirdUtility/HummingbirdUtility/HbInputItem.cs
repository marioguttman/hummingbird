//using System;
using System.Collections.Generic;
//using System.Text;
using System.Data;

namespace HummingbirdUtility {
    public class HbInputItem {

        public string RowId { set; get; }
        public DataRow DataRowPrimary { set; get; }  // the data row that will hold the ElementId
        public HbElementId ElementId { set; get; }
        public string CommandAction { set; get; }
        public string CommandObject { set; get; }
        public string CommandModifier { set; get; }
        public List<HbXYZ> ListHbXYZ { set; get; }
        public HbCurve HbCurve { set; get; }        
        public HbCurveArrArray HbCurveArrArray { set; get; }
        public HbReferencePoint HbReferencePoint { set; get; }
        public HbReferenceArray HbReferenceArray { set; get; }
        public HbReferenceArrayArray HbReferenceArrayArray { set; get; }
        public List<string> StringValues { set; get; }
        public List<int> IntValues { set; get; }
        public List<double> DoubleValues { set; get; }
        public List<bool> BoolValues { set; get; }
        public HbInputItem() {
            this.RowId = null;
            this.DataRowPrimary = null;
            this.ElementId = null;
            this.CommandAction = null;
            this.CommandObject = null;
            this.CommandModifier = null;
            this.ListHbXYZ = null;
            this.HbCurve = null;
            this.HbCurveArrArray = null;
            this.HbReferencePoint = null;
            this.HbReferenceArray = null;
            this.HbReferenceArrayArray = null;
            this.StringValues = null;
            this.IntValues = null;
            this.DoubleValues = null;
            this.BoolValues = null;
        }

        public void RecordElementId (HbElementId hbElementId) {
            this.ElementId = hbElementId;
            DataRowPrimary["ElementId"] = this.ElementId.ElementIdValue.ToString();
        }
    }
}

// Notes:
// This analysis is based on the HummingbirdUtility.InputParser()
// These should be the only cases that "foreach (HbInputItem hbInputItem in csvReader.HbInputItems)" can generate:
//
// foreach (HbInputItem hbInputItem in csvReader.HbInputItems) {
//     string hbAction = hbInputItem.CommandAction;
//     string hbObject = hbInputItem.CommandObject;
//     string hbModifier = hbInputItem.CommandModifier;
//     switch (hbAction) {
//         case "Add":
//            switch (hbObject) {
//
//                case "Level": 
//							  hbInputItem.DoubleValues[0]        - double (datum )
//							  hbInputItem.StringValues[0]        - string (name, optional)
//
//                case "Grid":
//							  hbInputItem.HbCurve                - HbLine or HbArc 
//
//                case "DetailLine":
//                case "ModelLine": 
//							  hbInputItem.HbCurve                - HbLine
//
//                case "DetailArc":
//                case "ModelArc": 
//							  hbInputItem.HbCurve                - HbArc
//
//                case "DetailEllipse":
//                case "ModelEllipse": 
//							  hbInputItem.HbCurve                - HbEllipse
//
//                case "DetailNurbSpline":
//                case "ModelNurbSpline": 
//							  hbInputItem.HbCurve                - HbNurbSpline
//
//                case "AreaBoundaryLine":
//                case "RoomSeparationLine":
//                    switch (hbModifier) {
//                        case "Curve":
//                            hbInputItem.hbCurve                - HbLine, HbArc, HbEllipse, HbNurbSpline
//                        case "CurveArrArray":
//                            hbInputItem.HbCurveArrayArray      - (multiple HbLine, HbArc, HbNurbSpline)
//
//                case "TopographySurface":
//                case "AdaptiveComponent": 
//                            hbInputItem.HbXyzList              - (multiple HbXYZ)
//
//                case "Wall": 
//                    switch (hbModifier) {
//                        case "Curve":
//							  hbInputItem.hbCurve                - HbLine or HbArc
//                        case "CurveArrArray":
//                            hbInputItem.HbCurveArrayArray      - (multiple HbLine, HbArc, HbNurbSpline)
//
//                case "Floor": 
//                    switch (hbModifier) {
//                        case "Points":
//                            hbInputItem.HbXyzList              - (multiple HbXYZ)
//                        case "CurveArrArray":
//                            hbInputItem.HbCurveArrayArray      - (multiple HbLine, HbArc, HbNurbSpline)
//
//                case "FamilyInstance":
//                case "Column":
//                case "Beam":
//                case "Area":
//                case "Room": 
//                            hbInputItem.HbXyzList              - (multiple HbXYZ) // Only one for family instance, area, and room; two with column and beam.
//
//                case "ReferencePoint":
//                            hbInputItem.HbReferencePoint       - HbReferencePoint
//
//                case "CurveByPoints": 
//                            hbInputItem.HbReferenceArray       - (multiple HbReferencePoint)
//
//                case "LoftForm":
//                            hbInputItem.HbReferenceArrayArray  - (multiple HbReferenceArray)
//
//                case "FamilyExtrusion"  List<HbXYZ>
//                            hbInputItem.ListHbXyz[0]           - HbXYZ (insertion point, optional)
//                            hbInputItem.HbCurveArrayArray      - (multiple HbLine, HbArc, HbNurbSpline)
