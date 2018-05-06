using System;
using System.Collections.Generic;

using Autodesk.Revit.DB;
using HummingbirdUtility;

namespace RevitModelBuilder {
    public class InputItem {

        #region Module Variables                        // ****************************** Module Variables ******************************************

        public HbInputItem HbInputItem { set; get; }
        public Curve Curve { set { curve = Curve; } get { return curve; } }
        public List<XYZ> ListXYZ { set; get; }
        public CurveArrArray CurveArrArray { set; get; }
        public List<string> StringValues { set; get; }
        public List<int> IntValues { set; get; }
        public List<double> DoubleValues { set; get; }
        public List<bool> BoolValues { set; get; }
        public string String0  { set; get; }
        public string String1  { set; get; }
        public string String2  { set; get; }
        public double Double0  { set; get; }
        public double Double1 { set; get; }
        public int Int0 { set; get; }
        public bool Bool0 { set; get; }
        public bool Bool1 { set; get; }

        public string ErrorMessage { set; get; }

        // Local versions
        private Curve curve;  
        private double unitsFactor;
        private XYZ translation = null;

        // Note: not building the ReferencePoint, ReferenceArray, or ReferenceArrayArray here since they require a model transaction

        #endregion

        #region Constructor                             // ****************************** Constructor ***********************************************

        public InputItem(HbInputItem hbInputItem, Double unitsFactor, XYZ translation, bool unitsOrTranslationNeeded) {
            this.ErrorMessage = "";
            if (unitsOrTranslationNeeded) {
                this.unitsFactor = unitsFactor;
                this.translation = translation;
                if (hbInputItem.HbCurve != null) {
                    hbInputItem.HbCurve = UnitsAndTranslation(hbInputItem.HbCurve);
                    if (hbInputItem.HbCurve == null) {
                        if (this.ErrorMessage == "") this.ErrorMessage = "Error creating 'InputItem' object.";
                        return;
                    }
                }
                if (hbInputItem.ListHbXYZ != null) {
                    hbInputItem.ListHbXYZ = UnitsAndTranslation(hbInputItem.ListHbXYZ);
                    if (hbInputItem.ListHbXYZ == null) {
                        if (this.ErrorMessage == "") this.ErrorMessage = "Error creating 'InputItem' object.";
                        return;
                    }
                }
                if (hbInputItem.HbCurveArrArray != null) {
                    hbInputItem.HbCurveArrArray = UnitsAndTranslation(hbInputItem.HbCurveArrArray);
                    if (hbInputItem.HbCurveArrArray == null) {
                        if (this.ErrorMessage == "") this.ErrorMessage = "Error creating 'InputItem' object.";
                        return;
                    }
                }
                if (hbInputItem.HbReferencePoint != null) {
                    hbInputItem.HbReferencePoint = UnitsAndTranslation(hbInputItem.HbReferencePoint);
                    if (hbInputItem.HbReferencePoint == null) {
                        if (this.ErrorMessage == "") this.ErrorMessage = "Error creating 'InputItem' object.";
                        return;
                    }
                }
                if (hbInputItem.HbReferenceArray != null) {
                    hbInputItem.HbReferenceArray = UnitsAndTranslation(hbInputItem.HbReferenceArray);
                    if (hbInputItem.HbReferenceArray == null) {
                        if (this.ErrorMessage == "") this.ErrorMessage = "Error creating 'InputItem' object.";
                        return;
                    }
                }
                if (hbInputItem.HbReferenceArrayArray != null) {
                    hbInputItem.HbReferenceArrayArray = UnitsAndTranslation(hbInputItem.HbReferenceArrayArray);
                    if (hbInputItem.HbReferenceArrayArray == null) {
                        if (this.ErrorMessage == "") this.ErrorMessage = "Error creating 'InputItem' object.";
                        return;
                    }
                }
            }

            this.HbInputItem = hbInputItem;
            if (HbInputItem.HbCurve != null) {
                if (!ConvertHbCurve(HbInputItem.HbCurve, ref this.curve)) {
                    this.curve = null;
                    if (this.ErrorMessage == "") this.ErrorMessage = "Error creating 'InputItem' object.";
                    return;
                }
            }
            else this.Curve = null;

            if (HbInputItem.ListHbXYZ != null && HbInputItem.ListHbXYZ.Count > 0) {
                this.ListXYZ = new List<XYZ>();
                if (!ConvertListHbXyz(HbInputItem.ListHbXYZ, this.ListXYZ)) {
                    this.ListXYZ = null;
                    if (this.ErrorMessage == "") this.ErrorMessage = "Error creating 'InputItem' object.";
                    return;
                }
            }
            else this.ListXYZ = null;

            if (HbInputItem.HbCurveArrArray != null && HbInputItem.HbCurveArrArray.Count() > 0) {
                this.CurveArrArray = new Autodesk.Revit.DB.CurveArrArray();
                if (!ConvertHbCurveArrArray(HbInputItem.HbCurveArrArray, this.CurveArrArray)) {
                    this.CurveArrArray = null;
                    if (this.ErrorMessage == "") this.ErrorMessage = "Error creating 'InputItem' object.";
                    return;
                }
            }
            else this.CurveArrArray = null;

            if (HbInputItem.StringValues != null) {
                this.StringValues = HbInputItem.StringValues;
                if (this.StringValues.Count > 0) this.String0 = this.StringValues[0];
                else this.String0 = null;
                if (this.StringValues.Count > 1) this.String1 = this.StringValues[1];
                else this.String1 = null;
                if (this.StringValues.Count > 2) this.String2 = this.StringValues[2];
                else this.String2 = null;
            }

            if (HbInputItem.IntValues != null) {
                this.IntValues = HbInputItem.IntValues;
                if (this.IntValues.Count > 0) this.Int0 = this.IntValues[0];
                else this.Int0 = 0;
            }

            if (HbInputItem.DoubleValues != null) {
                this.DoubleValues = HbInputItem.DoubleValues;
                if (this.DoubleValues.Count > 0) this.Double0 = this.DoubleValues[0];
                else this.Double0 = 0;
                if (this.DoubleValues.Count > 1) this.Double1 = this.DoubleValues[1];
                else this.Double1 = 0;
            }

            if (HbInputItem.BoolValues != null) {
                this.BoolValues = HbInputItem.BoolValues;
                if (this.BoolValues.Count > 0) this.Bool0 = this.BoolValues[0];
                else this.Bool0 = false;
                if (this.BoolValues.Count > 1) this.Bool1 = this.BoolValues[1];
                else this.Bool1 = false;
            }

        }

        #endregion

        #region Public Functions                        // ****************************** Public Functions ******************************************

        public void RecordElementId(ElementId elementId) {
            if (elementId == null) HbInputItem.DataRowPrimary[1] = "";
            else HbInputItem.DataRowPrimary[1] = elementId.ToString();
        }

        public string DisplayValues() {
            string rowIdValue = "NULL";
            string elementIdValue = "NULL";
            string actionValue = "NULL";
            string objectValue = "NULL";
            string modifierValue = "NULL";
            string value01Value = "NULL";
            string value02Value = "NULL";
            string value03Value = "NULL";
            string value04Value = "NULL";
            if (HbInputItem == null) return "HbInputItem is NULL";
            else {
                if (HbInputItem.RowId != null) rowIdValue = HbInputItem.RowId;
                if (HbInputItem.ElementId != null) elementIdValue = HbInputItem.ElementId.ElementIdValue.ToString();
                if (HbInputItem.CommandAction != null) actionValue = HbInputItem.CommandAction;
                if (HbInputItem.CommandObject != null) objectValue = HbInputItem.CommandObject;
                if (HbInputItem.CommandModifier != null) modifierValue = HbInputItem.CommandModifier;
                if (HbInputItem.DataRowPrimary == null) {
                    return
                        "RowId: " + rowIdValue + "\r\n"
                      + "ElementId: " + elementIdValue + "\r\n"
                      + "Action: " + actionValue + "\r\n"
                      + "Object: " + objectValue + "\r\n"
                      + "Modifier: " + modifierValue + "\r\n"
                      + "HbInputItem.DataRowPrimary is NULL";
                }
                if (HbInputItem.DataRowPrimary[4] != null) value01Value = HbInputItem.DataRowPrimary[4].ToString();
                if (HbInputItem.DataRowPrimary[5] != null) value02Value = HbInputItem.DataRowPrimary[5].ToString();
                if (HbInputItem.DataRowPrimary[6] != null) value03Value = HbInputItem.DataRowPrimary[6].ToString();
                if (HbInputItem.DataRowPrimary[7] != null) value04Value = HbInputItem.DataRowPrimary[7].ToString();
                return
                    "RowId: " + rowIdValue + "\r\n"
                  + "ElementId: " + elementIdValue + "\r\n"
                  + "Action: " + actionValue + "\r\n"
                  + "Object: " + objectValue + "\r\n"
                  + "Modifier: " + modifierValue + "\r\n"
                  + "Value01: " + value01Value + "\r\n"
                  + "Value02: " + value02Value + "\r\n"
                  + "Value03: " + value03Value + "\r\n"
                  + "Value04: " + value04Value;
            }
        }

        #endregion

        #region Private Functions                       // ****************************** Private Functions *****************************************
        // No need for these to be public at this time but could be useful in the future 

        private bool ConvertHbXyz(HbXYZ hbXyz, out XYZ xyz) {
            if (hbXyz == null) { // For optional value cases like parameters with Add FamilyExtrusion
                xyz = null;
                return true;  
            }
            try {
                xyz = new XYZ(hbXyz.X, hbXyz.Y, hbXyz.Z);
                return true;
            }
            catch {
                xyz = null;
                return false;
            }

        }
        private bool ConvertListHbXyz(List<HbXYZ> listHbXyz, List<XYZ> listXyz) {
            try {
                foreach (HbXYZ hbXyz in listHbXyz) {
                    XYZ xyz;
                    if (!ConvertHbXyz(hbXyz, out xyz)) {
                        listXyz = null;
                        return false;
                    }
                    listXyz.Add(xyz);
                }
                return true;
            }
            catch {
                listXyz = null;
                return false;
            }

        }
        private bool ConvertHbCurve(HbCurve hbCurve, ref Curve curve) {
            try {
                curve = null;
                XYZ xyz1, xyz2, xyz3;
                if (hbCurve is HbLine) {
                    HbLine hbLine = (HbLine)hbCurve;
                    if (!ConvertHbXyz(hbLine.PointStart, out xyz1)) {
                        return false;
                    }
                    if (!ConvertHbXyz(hbLine.PointEnd, out xyz2)) {
                        return false;
                    }
                    curve = Line.CreateBound(xyz1, xyz2);
                }
                else if (hbCurve is HbArc) {
                    HbArc hbArc = (HbArc)hbCurve;
                    if (!ConvertHbXyz(hbArc.PointStart, out xyz1)) {
                        return false;
                    }
                    if (!ConvertHbXyz(hbArc.PointEnd, out xyz2)) {
                        return false;
                    }
                    if (!ConvertHbXyz(hbArc.PointMid, out xyz3)) {
                        return false;
                    }
                    curve = Arc.Create(xyz1, xyz2, xyz3);
                }
                else if (hbCurve is HbEllipse) {
                    HbEllipse hbEllipse = (HbEllipse)hbCurve;
                    if (!ConvertHbXyz(hbEllipse.PointFirst, out xyz1)) {
                        return false;
                    }
                    if (!ConvertHbXyz(hbEllipse.PointSecond, out xyz2)) {
                        return false;
                    }
                    XYZ center, xVec, yVec;
                    double radX, radY, param0, param1;
                    CalculateEllipsePoints(xyz1, xyz2, hbEllipse.RadiusY, hbEllipse.Mode, out center, out radX, out radY, out xVec, out yVec, out param0, out param1);
                    curve = Ellipse.CreateCurve(center, radX, radY, xVec, yVec, param0, param1);
                    //curve = Ellipse.Create(center, radX, radY, xVec, yVec, param0, param1);  //2018 upgrade
                }
                else if (hbCurve is HbNurbSpline) {
                    HbNurbSpline hbNurbSpline = (HbNurbSpline)hbCurve;
                    List<XYZ> points = new List<XYZ>();
                    foreach (HbXYZ point in hbNurbSpline.Points) {
                        if (!ConvertHbXyz(point, out xyz1)) {
                            return false;
                        }
                        points.Add(xyz1);
                    }
                    List<double> weights = new List<double>();
                    // Note that we are just using fixed weights for now.  It doesn't appear that Revit is actually using these values anyway.
                    for (int i = 0; i < hbNurbSpline.Points.Count; i++) {
                        weights.Add(1.0);
                    }
                    //curve = NurbSpline.Create(points, weights);  // Fails if < 4 points although manually can create with 2 or 3 points.
                    curve = NurbSpline.CreateCurve(points, weights);
                    //TODO This failed in older versions if < 4 points although manually can create with 2 or 3 points.  Is this fixed in 2017?
                }
                else if (hbCurve is HbHermiteSpline) {
                    HbHermiteSpline hbHermiteSpline = (HbHermiteSpline)hbCurve;
                    List<XYZ> points = new List<XYZ>();
                    foreach (HbXYZ point in hbHermiteSpline.Points) {
                        if (!ConvertHbXyz(point, out xyz1)) {
                            return false;
                        }
                        points.Add(xyz1);
                    }
                    curve = HermiteSpline.Create(points, false);
                }
                return true;
            }
            catch (Exception exception) {
                this.ErrorMessage = "Error in ConvertHbCurve: " + exception.Message + ".";
                curve = null;
                return false;
            }
        }
        private bool ConvertHbCurveArrArray(HbCurveArrArray hbCurveArrArray, CurveArrArray curveArrArray) {
            try {
                foreach (HbCurveArray hbCurveArray in hbCurveArrArray) {
                    CurveArray curveArray = new CurveArray();
                    foreach (HbCurve hbCurve in hbCurveArray) {
                        Curve curve = null;
                        if (!ConvertHbCurve(hbCurve, ref curve)) {
                            curveArrArray = null;
                            return false;
                        }
                        curveArray.Append(curve);
                    }
                    curveArrArray.Append(curveArray);
                }
                return true;
            }
            catch {
                curveArrArray = null;
                return false;
            }
        }

        private void CalculateEllipsePoints(XYZ point1, XYZ point2, double radiusY, string mode,
            out XYZ center, out double radX, out double radY, out XYZ xVec, out XYZ yVec, out double param0, out double param1) {
            // mode = "Full": point1 - center; point2 - end; point3 - side
            // mode = "Half": point1 - start;  point2 - end; point3 - side

            double deltaX;
            double deltaY;
            if (mode == "Full") {
                center = point1;
                param0 = 0.0;
                param1 = 2 * Math.PI;
            }
            else { // "Half" case
                center = new XYZ((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, point1.Z);
                param0 = 0.0;
                param1 = Math.PI;
            }
            deltaX = point2.X - center.X;
            deltaY = point2.Y - center.Y;
            radX = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            xVec = new XYZ(deltaX / radX, deltaY / radX, 0);   // radX happens to be the length of the vectors so this will normalize them
            yVec = new XYZ(-deltaY / radX, deltaX / radX, 0);  // This is a perpendicular for vectors at origin.  Assumes deltaX and DeltaY are not 0.
            radY = radiusY;
        }

        // ********************************************* Translation ****************************************************
        private HbXYZ UnitsAndTranslation(HbXYZ hbXyz) {
            if (hbXyz == null) return null; // This occurs when there is an optional parameter, such as with extrusion placement point.
            return new HbXYZ((hbXyz.X + this.translation.X) * this.unitsFactor,
                             (hbXyz.Y + this.translation.Y) * this.unitsFactor,
                             (hbXyz.Z + this.translation.Z) * this.unitsFactor);
        }
        private HbCurve UnitsAndTranslation(HbCurve hbCurve) {
            try {
                if (hbCurve is HbLine) {
                    HbLine hbLine = (HbLine)hbCurve;
                    return new HbLine(UnitsAndTranslation(hbLine.PointStart), UnitsAndTranslation(hbLine.PointEnd));
                }
                else if (hbCurve is HbArc) {
                    HbArc hbArc = (HbArc)hbCurve;
                    return new HbArc(UnitsAndTranslation(hbArc.PointStart), UnitsAndTranslation(hbArc.PointEnd), UnitsAndTranslation(hbArc.PointMid));
                }
                else if (hbCurve is HbEllipse) {
                    HbEllipse hbEllipse = (HbEllipse)hbCurve;
                    return new HbEllipse(UnitsAndTranslation(hbEllipse.PointFirst), UnitsAndTranslation(hbEllipse.PointSecond), hbEllipse.RadiusY * this.unitsFactor, hbEllipse.Mode);
                }
                else if (hbCurve is HbNurbSpline) {
                    HbNurbSpline hbNurbSpline = (HbNurbSpline)hbCurve;
                    List<HbXYZ> listHbXyz = new List<HbXYZ>();
                    foreach (HbXYZ hbXyz in hbNurbSpline.Points) {
                        listHbXyz.Add(UnitsAndTranslation(hbXyz));
                    }
                    return new HbNurbSpline(listHbXyz);
                }
                else if (hbCurve is HbHermiteSpline) {
                    HbHermiteSpline hbHermiteSpline = (HbHermiteSpline)hbCurve;
                    List<HbXYZ> listHbXyz = new List<HbXYZ>();
                    foreach (HbXYZ hbXyz in hbHermiteSpline.Points) {
                        listHbXyz.Add(UnitsAndTranslation(hbXyz));
                    }
                    return new HbHermiteSpline(listHbXyz);
                }
                else return null;
            }
            catch (Exception exception) {
                this.ErrorMessage = "Error in UnitsAndTranslation: " + exception.Message + ".";
                return null;
            }

        }
        private List<HbXYZ> UnitsAndTranslation(List<HbXYZ> listHbXyz) {
            List<HbXYZ> listHbXyzNew = new List<HbXYZ>();
            foreach (HbXYZ hbXyz in listHbXyz) {
                listHbXyzNew.Add(UnitsAndTranslation(hbXyz));
                if (this.ErrorMessage != "") return null;
            }
            return listHbXyzNew;
        }
        private HbCurveArray UnitsAndTranslation(HbCurveArray hbCurveArray) {
            HbCurveArray hbCurveArrayNew = new HbCurveArray();
            foreach (HbCurve hbCurve in hbCurveArray) {
                hbCurveArrayNew.Add(UnitsAndTranslation(hbCurve));
                if (this.ErrorMessage != "") return null;
            }
            return hbCurveArrayNew;
        }
        private HbCurveArrArray UnitsAndTranslation(HbCurveArrArray hbCurveArrArray) {
            HbCurveArrArray hbCurveArrArrayNew = new HbCurveArrArray();
            foreach (HbCurveArray hbCurveArray in hbCurveArrArray) {
                hbCurveArrArrayNew.Add(UnitsAndTranslation(hbCurveArray));
            }
            return hbCurveArrArrayNew;
        }
        private HbReferencePoint UnitsAndTranslation(HbReferencePoint hbReferencePoint) {
            hbReferencePoint = new HbReferencePoint((hbReferencePoint.X + this.translation.X) * this.unitsFactor,
                                        (hbReferencePoint.Y + this.translation.Y) * this.unitsFactor,
                                        (hbReferencePoint.Z + this.translation.Z) * this.unitsFactor);
            if (this.ErrorMessage != "") return null;
            else return hbReferencePoint;
        }
        private HbReferenceArray UnitsAndTranslation(HbReferenceArray hbReferenceArray) {
            HbReferenceArray hbReferenceArrayNew = new HbReferenceArray();
            foreach (HbReferencePoint hbReferencePoint in hbReferenceArray) {
                hbReferenceArrayNew.Add(UnitsAndTranslation(hbReferencePoint));
                if (this.ErrorMessage != "") return null;
            }
            return hbReferenceArrayNew;
        }
        private HbReferenceArrayArray UnitsAndTranslation(HbReferenceArrayArray hbReferenceArrayArray) {
            HbReferenceArrayArray hbReferenceArrayArrayNew = new HbReferenceArrayArray();
            foreach (HbReferenceArray hbReferenceArray in hbReferenceArrayArray) {
                hbReferenceArrayArrayNew.Add(UnitsAndTranslation(hbReferenceArray));
                if (this.ErrorMessage != "") return null;
            }
            return hbReferenceArrayArrayNew;
        }

        #endregion
    }
}
