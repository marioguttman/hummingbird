using System;
using System.Collections.Generic;
using System.IO;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;

using HummingbirdUtility;

namespace Hummingbird {

    class Utility {

        // *********************************************************** Module Variables **************************************************************        
        private IGH_DataAccess dataAccess;
        private List<string> outStrings = new List<string>();

        // ************************************************************ Constructor *******************************************************************
        public Utility(IGH_DataAccess dataAccess) {
            this.dataAccess = dataAccess;
        }

        // ************************************************ RevitModelBuilder CSV file Functions *****************************************************
        public bool EstablishCsvLink(CsvWriter csvWriter, string folderPath, string fileName) {
            if (!folderPath.EndsWith(@"\")) folderPath = folderPath + @"\";
            if (!fileName.ToLower().EndsWith(".csv")) fileName = fileName + ".csv";  
            string filePath = folderPath + fileName;
            string returnValue = csvWriter.ConnectToFile(filePath);
            if (returnValue != "") {
                Print(returnValue);
                return false;
            }
            Print("Connection to CSV file established.");

            // Read existing ElementIds if the file already exists.
            // Note that we are doing this by default and no user option is provided.  Alternatively we could have a user setting.
            if (File.Exists(filePath)) {
                try {
                    returnValue = csvWriter.ReadElementIds();
                    if (returnValue != "") {
                        Print(returnValue);
                        return false;
                    }
                }
                catch (Exception exception) {
                    Print("Exception at Utility.EstablishCsvLink(): " + exception.Message);
                    return false;
                }
            }


            return true;
        }
        public bool EstablishCsvLink(CsvReader csvReader, string folderPath, string fileName) {
            if (!folderPath.EndsWith(@"\")) folderPath = folderPath + @"\";
            if (!fileName.ToLower().EndsWith(".csv")) fileName = fileName + ".csv";
            string filePath = folderPath + fileName;
            string returnValue = csvReader.ConnectToFile(filePath);
            if (returnValue != "") {
                Print(returnValue);
                return false;
            }
            Print("Connection to CSV file established.");
            return true;
        }

        // ************************************************* Data Output Functions ******************************************************************

        public string formatPoint(HbXYZ point) {
            string x = point.X.ToString();
            string y = point.Y.ToString();
            string z = point.Z.ToString();
            return "(" + x + ", " + y + ", " + z + ")";
        }
        public string formatPoints(HbXYZ point1, HbXYZ point2) {
            string string1 = formatPoint(point1);
            string string2 = formatPoint(point2);
            return string1 + ", " + string2;
        }
        public string formatPoints(HbXYZ point1, HbXYZ point2, HbXYZ point3) {
            string string1 = formatPoint(point1);
            string string2 = formatPoint(point2);
            string string3 = formatPoint(point3);
            return string1 + ", " + string2 + ", " + string3;
        }
        public string formatPoints(HbXYZ point1, HbXYZ point2, HbXYZ point3, HbXYZ point4) {
            string string1 = formatPoint(point1);
            string string2 = formatPoint(point2);
            string string3 = formatPoint(point3);
            string string4 = formatPoint(point4);
            return string1 + ", " + string2 + ", " + string3 + ", " + string4;
        }
        public string formatCurves(List<HbCurve> curves) {
            string buildString = formatCurve(curves[0]);
            for (int i = 1; i < curves.Count; i++) {
                buildString += ", " + formatCurve(curves[i]);
            }
            return buildString;
        }
        public string formatCurve(HbCurve curve) {            
            string curveName = curve.GetType().Name.Substring(2);  //Strips the "Hb"
            string buildString = curveName + ": ";
            switch (curveName) {
                case "Arc":
                    HbArc arc = (HbArc)curve;
                    buildString += formatPoints(arc.PointStart, arc.PointEnd, arc.PointMid);
                    break;
                case "Line":
                    HbLine line = (HbLine) curve;
                    buildString += formatPoints(line.PointStart, line.PointEnd);
                    break;
                case "Ellipse":
                case "HermiteSpline":
                case "NurbSpline":
                default:                    
                    break;
            }
            return buildString;
        }


        // ************************************************ RevitModelBuilder Data Functions *****************************************************
        public HbCurve Hb(NurbsCurve nurbsCurveRhino) {
        
            HbNurbSpline nurbSplineRevit = new HbNurbSpline();
            Rhino.Geometry.Collections.NurbsCurvePointList points = nurbsCurveRhino.Points;
            for (int k = 0; k < points.Count; k++)
            {
                Rhino.Geometry.ControlPoint controlPoint = points[k];
                Point3d xyzRhino = controlPoint.Location;
                HbXYZ xyzRevit = new HbXYZ(xyzRhino.X, xyzRhino.Y, xyzRhino.Z);
                nurbSplineRevit.Points.Add(xyzRevit);
            }
            HbCurve hbCurve = nurbSplineRevit;
                return hbCurve;
        }

        public HbCurve Hb(LineCurve lineCurveRhino) {
            HbCurve hbCurve = new HbCurve();
            Point3d xyzRhinoStart = lineCurveRhino.PointAtStart;
            Point3d xyzRhinoEnd = lineCurveRhino.PointAtEnd;
            //Not sure why we get the midpoint here for a line object?
            //Point3d xyzRhinoMid = lineCurveRhino.PointAt(0.5);
            if (lineCurveRhino.IsLinear()) {
                HbLine hbLineRevit = new HbLine();
                hbLineRevit.PointStart = new HbXYZ(xyzRhinoStart.X, xyzRhinoStart.Y, xyzRhinoStart.Z);
                hbLineRevit.PointEnd = new HbXYZ(xyzRhinoEnd.X, xyzRhinoEnd.Y, xyzRhinoEnd.Z);
                hbCurve = hbLineRevit;
            }
            return hbCurve;
        }

        public HbCurve Hb(ArcCurve arcCurveRhino) {
            HbCurve hbCurve = new HbCurve();
            Point3d xyzRhinoStart = arcCurveRhino.PointAtStart;
            Point3d xyzRhinoEnd = arcCurveRhino.PointAtEnd;
            Point3d xyzRhinoMid = arcCurveRhino.PointAtNormalizedLength(0.5);
            if (arcCurveRhino.IsArc())
            {
                    HbArc hbArcRevit = new HbArc();
                    hbArcRevit.PointStart = new HbXYZ(xyzRhinoStart.X, xyzRhinoStart.Y, xyzRhinoStart.Z);
                    hbArcRevit.PointEnd = new HbXYZ(xyzRhinoEnd.X, xyzRhinoEnd.Y, xyzRhinoEnd.Z);
                    hbArcRevit.PointMid = new HbXYZ(xyzRhinoMid.X, xyzRhinoMid.Y, xyzRhinoMid.Z);
                    hbCurve = hbArcRevit;
            }
            return hbCurve;
        }

        //************************************************ Rhino Functions *****************************************************

        public bool GetInput(int dataAccessIndex, ref string variable) {
            return GetInput(dataAccessIndex, ref variable, true, true, false);
        }
        public bool GetInput(int dataAccessIndex, ref string variable, bool checkNull, bool checkEmpty) {
            return GetInput(dataAccessIndex, ref variable, checkNull, checkEmpty, false);
        }
        public bool GetInput(int dataAccessIndex, ref string variable, bool checkNull, bool checkEmpty, bool silent) {
            if (!this.dataAccess.GetData(dataAccessIndex, ref variable)) {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }
            if (checkNull) {
                if (variable == null) {
                    if (!silent) Print("DataAccess.GetData() returned null at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            if (checkEmpty) {
                if (variable.Length == 0) {
                    if (!silent) Print("DataAccess.GetData() returned empty string at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, ref bool variable) {
            return GetInput(dataAccessIndex, ref variable, false);
        }
        public bool GetInput(int dataAccessIndex, ref bool variable, bool silent) {
            if (!this.dataAccess.GetData(dataAccessIndex, ref variable)) {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, ref double variable) {
            return GetInput(dataAccessIndex, ref variable, false);
        }
        public bool GetInput(int dataAccessIndex, ref double variable, bool silent) {
            if (!this.dataAccess.GetData(dataAccessIndex, ref variable))  {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, ref List<Brep> values) {
            return GetInput(dataAccessIndex, ref values, false);
        }
        public bool GetInput(int dataAccessIndex, ref List<Brep> values, bool silent) {
            if (!this.dataAccess.GetDataList<Brep>(dataAccessIndex, values)) {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, ref List<bool> values) {
            return GetInput(dataAccessIndex, ref values, false);
        }
        public bool GetInput(int dataAccessIndex, ref List<bool> values, bool silent) {
            values = new List<bool>();
            if (!this.dataAccess.GetDataList(dataAccessIndex, values)) {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, ref List<string> values) {
            return GetInput(dataAccessIndex, ref values, true, true, false);
        }
        public bool GetInput(int dataAccessIndex, ref List<string> values, bool checkNull, bool checkEmpty, bool silent) {
            values = new List<string>();
            if (!this.dataAccess.GetDataList(dataAccessIndex, values)) {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }
            if (checkNull) {
                if (values == null) {
                    if (!silent) Print("DataAccess.GetData() returned null at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            if (checkEmpty) {
                if (values.Count == 0) {
                    if (!silent) Print("DataAccess.GetData() returned empty list at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, ref List<double> values) {
            return GetInput(dataAccessIndex, ref values, true, true, false);
        }
        public bool GetInput(int dataAccessIndex, ref List<double> values, bool checkNull, bool checkEmpty, bool silent) {
            values = new List<double>();
            if (!this.dataAccess.GetDataList(dataAccessIndex, values)) {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }
            if (checkNull) {
                if (values == null) {
                    if (!silent) Print("DataAccess.GetData() returned null at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            if (checkEmpty) {
                if (values.Count == 0) {
                    if (!silent) Print("DataAccess.GetData() returned empty list at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, ref List<GH_Point> values) {
            return GetInput(dataAccessIndex, ref values, true, true, false);
        }
        public bool GetInput(int dataAccessIndex, ref List<GH_Point> values, bool checkNull, bool checkEmpty, bool silent) {
            values = new List<GH_Point>();
            if (!this.dataAccess.GetDataList(dataAccessIndex, values)) {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }
            if (checkNull) {
                if (values == null) {
                    if (!silent) Print("DataAccess.GetData() returned null at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            if (checkEmpty) {
                if (values.Count == 0) {
                    if (!silent) Print("DataAccess.GetData() returned empty list at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, ref List<GH_Curve> dataList) {
            return GetInput(dataAccessIndex, ref dataList, true, true, false);
        }
        public bool GetInput(int dataAccessIndex, ref List<GH_Curve> dataList, bool checkNull, bool checkEmpty, bool silent) {
            if (!this.dataAccess.GetDataList<GH_Curve>(dataAccessIndex, dataList))  {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }

            if (checkNull) {
                if (dataList == null) {
                    if (!silent) Print("DataAccess.GetData() returned null at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            if (checkEmpty)  {
                if (dataList.Count == 0)  {
                    if (!silent) Print("DataAccess.GetData() returned empty list at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, ref List<GH_Surface> dataList) {
            return GetInput(dataAccessIndex, ref dataList, true, true, false);
        }
        public bool GetInput(int dataAccessIndex, ref List<GH_Surface> dataList, bool checkNull, bool checkEmpty, bool silent)
        {
            if (!this.dataAccess.GetDataList<GH_Surface>(dataAccessIndex, dataList))
            {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                dataList = null;
                return false;
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, out Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Curve> dataTree) {
            return GetInput(dataAccessIndex, out dataTree, true, true, false);
        }
        public bool GetInput(int dataAccessIndex, out Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_Curve> dataTree, bool checkNull, bool checkEmpty, bool silent) {
            if (!this.dataAccess.GetDataTree<Grasshopper.Kernel.Types.GH_Curve>(dataAccessIndex, out dataTree)) {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }
            if (checkNull) {
                if (dataTree == null) {
                    if (!silent) Print("DataAccess.GetData(" + dataAccessIndex.ToString() + ", dataTree)  returned null.");
                    return false;
                }
            }
            if (checkEmpty) {
                if (dataTree.Branches.Count == 0) {
                    if (!silent) Print("DataAccess.GetData() returned data tree with no branches at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, out Grasshopper.Kernel.Data.GH_Structure<GH_Point> dataTree) {
            return GetInput(dataAccessIndex, out dataTree, true, true, false);
        }
        public bool GetInput(int dataAccessIndex, out Grasshopper.Kernel.Data.GH_Structure<GH_Point> dataTree, bool checkNull, bool checkEmpty, bool silent) {
            if (!this.dataAccess.GetDataTree<GH_Point>(dataAccessIndex, out dataTree)) {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }
            if (checkNull) {
                if (dataTree == null) {
                    if (!silent) Print("DataAccess.GetData(" + dataAccessIndex.ToString() + ", dataTree) returned null.");
                    return false;
                }
            }
            if (checkEmpty) {
                if (dataTree.Branches.Count == 0) {
                    if (!silent) Print("DataAccess.GetData() returned data tree with no branches at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            return true;
        }

        public bool GetInput(int dataAccessIndex, out Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_String> dataTree) {
            return GetInput(dataAccessIndex, out dataTree, true, true, false);
        }
        public bool GetInput(int dataAccessIndex, out Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.GH_String> dataTree, bool checkNull, bool checkEmpty, bool silent) {
            if (!this.dataAccess.GetDataTree<Grasshopper.Kernel.Types.GH_String>(dataAccessIndex, out dataTree)) {
                if (!silent) Print("DataAccess.GetData() failed at index: " + dataAccessIndex.ToString() + ".");
                return false;
            }
            if (checkNull) {
                if (dataTree == null) {
                    if (!silent) Print("DataAccess.GetData(" + dataAccessIndex.ToString() + ", dataTree) returned null.");
                    return false;
                }
            }
            if (checkEmpty) {
                if (dataTree.Branches.Count == 0) {
                    if (!silent) Print("DataAccess.GetData() returned data tree with no branches at index: " + dataAccessIndex.ToString() + ".");
                    return false;
                }
            }
            return true;
        }

        public bool GetParameterValueArray(int dataAccessIndexNames, int dataAccessIndexValues, ref List<string> parameterNames, out GH_Structure<GH_String> parameterValues, out string[,] parameterArray) {
            GetInput(dataAccessIndexNames, ref parameterNames);
            GetInput(dataAccessIndexValues, out parameterValues);
            if ((parameterNames == null) || (parameterValues == null)) {
                parameterArray = null;
                return false;
            }
            if ((parameterValues.Branches.Count == 0) || (parameterNames.Count == 0)) {
                parameterArray = null;
                return false;
            }
            parameterArray = new string[parameterValues.Branches.Count, parameterNames.Count];
            for (int i = 0; i < parameterValues.Branches.Count; i++) {
                List<Grasshopper.Kernel.Types.GH_String> branch = parameterValues.Branches[i];
                for (int j = 0; j < branch.Count; j++) {
                    string valueString = null;
                    GH_Convert.ToString(branch[j], out valueString, GH_Conversion.Both);
                    parameterArray[i, j] = valueString;
                }
            }
            return true;
        }

        public bool ReadDataTree(GH_Structure<GH_Point> tree, ref List<List<HbXYZ>> pointsListListRevit) {
            try {
                for (int i = 0; i < tree.Branches.Count; i++) {
                    List<HbXYZ> pointsListRevit = new List<HbXYZ>();
                    List<GH_Point> branch = tree[i];
                    for (int j = 0; j < branch.Count; j++) {
                        pointsListRevit.Add(new HbXYZ(branch[j].Value.X, branch[j].Value.Y, branch[j].Value.Z));
                    }
                    pointsListListRevit.Add(pointsListRevit);
                }
                return true;
            }
            catch (Exception exception) {
                Print("Exception in 'ReadDataTree': " + exception.Message);
                return false;
            }
        }       

        public bool ReadDataTreeBranch(List<Grasshopper.Kernel.Types.GH_Curve> branches, ref List<List<HbCurve>> curvesListListRevit) {
            return ReadDataTreeBranch(branches, ref curvesListListRevit, true, true);
        }
        public bool ReadDataTreeBranch(List<Grasshopper.Kernel.Types.GH_Curve> branches, ref List<List<HbCurve>> curvesListListRevit, bool AllowNurbs) {
            return ReadDataTreeBranch(branches, ref curvesListListRevit, true, true);
        }
        public bool ReadDataTreeBranch(List<Grasshopper.Kernel.Types.GH_Curve> branches, ref List<List<HbCurve>> curvesListListRevit, bool AllowNurbs, bool NestBranch) {
            try {
                Rhino.Geometry.NurbsCurve nurbsCurveRhino;
                Rhino.Geometry.LineCurve lineCurveRhino;
                Rhino.Geometry.ArcCurve arcCurveRhino;
                List<HbCurve> curvesListRevit = new List<HbCurve>();

                for (int i = 0; i < branches.Count; i++) {

                    if (NestBranch)  curvesListRevit.Clear();
                    GH_Curve branch = branches[i];
                    Curve curveBranch = null;
                    Curve curveBranch2 = null;
                    GH_Convert.ToCurve(branch, ref curveBranch, GH_Conversion.Both);
                    if (curveBranch == null) {
                        Print("Null branch value in ReadDataTreeBranch() ignored.");
                        continue;
                    }
                    
                    switch (curveBranch.GetType().Name) {
                        case "PolyCurve":
                            // Note:  The simplify functions only used with a PolyCureve so moved inside the switch statement.
                            // Note:  Logic of this step is not clear.  It may be that only the CurveSimplifyOptions.All
                            if (curveBranch.Degree == 1) {
                                curveBranch2 = curveBranch.Simplify(CurveSimplifyOptions.RebuildLines, 0, 0);
                                if (curveBranch2 == null) Print("Simplify() with CurveSimplifyOptions.RebuildLines returned null.");
                                else curveBranch = curveBranch2;
                            }
                            if (curveBranch.Degree == 2) {
                                curveBranch2 = curveBranch.Simplify(CurveSimplifyOptions.RebuildArcs, 0, 0);
                                if (curveBranch2 == null) Print("Simplify() with CurveSimplifyOptions.RebuildArcs returned null.");
                                else curveBranch = curveBranch2;
                            }
                            curveBranch2 = curveBranch.Simplify(CurveSimplifyOptions.All, .0001, 1);
                            if (curveBranch2 == null) Print("Simplify() with CurveSimplifyOptions.All returned null.");
                            else curveBranch = curveBranch2;
                            PolyCurve polyCurve = (PolyCurve)curveBranch;                              
                            for (int j = 0; j < polyCurve.SegmentCount; j++) {
                                Curve curveRhino = polyCurve.SegmentCurve(j);
                                switch (curveRhino.GetType().Name) {
                                    case "LineCurve":
                                        lineCurveRhino = (LineCurve)curveRhino;
                                        curvesListRevit.Add(Hb(lineCurveRhino));
                                        break;
                                    case "NurbsCurve":
                                        if (!AllowNurbs) break;
                                        
                                        nurbsCurveRhino = (NurbsCurve)curveRhino;
                                        curvesListRevit.Add(Hb(nurbsCurveRhino));
                                        break;
                                    case "ArcCurve":
                                        arcCurveRhino = (ArcCurve)curveRhino;
                                        curvesListRevit.Add(Hb(arcCurveRhino));
                                        break;

                                    case "PolylineCurve":
                                        //NurbsCurve nurbsLineCurve = (NurbsCurve)curveBranch.ToNurbsCurve();
                                        NurbsCurve nurbsLineCurve = (NurbsCurve)curveRhino.ToNurbsCurve();
                                        Rhino.Geometry.Collections.NurbsCurvePointList points = nurbsLineCurve.Points;

                                        for (int k = 0; k < points.Count - 1; k++)
                                        {
                                            HbLine lineRevit = new HbLine();
                                            Rhino.Geometry.ControlPoint controlPointStart = points[k];
                                            Rhino.Geometry.ControlPoint controlPointEnd = points[k + 1];
                                            Point3d xyzRhinoStart = controlPointStart.Location;
                                            Point3d xyzRhinoEnd = controlPointEnd.Location;

                                            HbXYZ xyzRevitStart = new HbXYZ(xyzRhinoStart.X, xyzRhinoStart.Y, xyzRhinoStart.Z);
                                            HbXYZ xyzRevitEnd = new HbXYZ(xyzRhinoEnd.X, xyzRhinoEnd.Y, xyzRhinoEnd.Z);

                                            lineRevit.PointStart = new HbXYZ(xyzRevitStart.X, xyzRevitStart.Y, xyzRevitStart.Z);
                                            lineRevit.PointEnd = new HbXYZ(xyzRevitEnd.X, xyzRevitEnd.Y, xyzRevitEnd.Z);
                                            curvesListRevit.Add(lineRevit);
                                        }

                                        break;

                                    default:
                                        Print("Unknown Rhino PolyCurve curve type: " + curveRhino.GetType().Name);
                                        break;
                                }
                            }
                            break;
                        case "NurbsCurve":
                            if (!AllowNurbs) break;
                           
                            nurbsCurveRhino = (NurbsCurve)curveBranch;
                            //if !nurbsCurveRhino.IsClosed()
                            curvesListRevit.Add(Hb(nurbsCurveRhino));
                            break;

                        case "PolylineCurve":
                            NurbsCurve nurbsLineCurve2 = (NurbsCurve)curveBranch.ToNurbsCurve();
                            Rhino.Geometry.Collections.NurbsCurvePointList points2 = nurbsLineCurve2.Points;

                            for (int k = 0; k < points2.Count-1; k++)
                            {
                                HbLine lineRevit = new HbLine();
                                Rhino.Geometry.ControlPoint controlPointStart = points2[k];
                                Rhino.Geometry.ControlPoint controlPointEnd = points2[k + 1];
                                Point3d xyzRhinoStart = controlPointStart.Location;
                                Point3d xyzRhinoEnd = controlPointEnd.Location;

                                HbXYZ xyzRevitStart = new HbXYZ(xyzRhinoStart.X, xyzRhinoStart.Y, xyzRhinoStart.Z);
                                HbXYZ xyzRevitEnd = new HbXYZ(xyzRhinoEnd.X, xyzRhinoEnd.Y, xyzRhinoEnd.Z);

                                lineRevit.PointStart = new HbXYZ(xyzRevitStart.X, xyzRevitStart.Y, xyzRevitStart.Z);
                                lineRevit.PointEnd = new HbXYZ(xyzRevitEnd.X, xyzRevitEnd.Y, xyzRevitEnd.Z);
                                curvesListRevit.Add(lineRevit);
                            }

                            break;

                        case "LineCurve":
                            lineCurveRhino = (LineCurve)curveBranch;
                            curvesListRevit.Add(Hb(lineCurveRhino));
                            break;

                        case "ArcCurve":
                            arcCurveRhino = (ArcCurve)curveBranch;
                            curvesListRevit.Add(Hb(arcCurveRhino));
                            break;

                        default:
                            Print("Unknown Rhino curve type: " + curveBranch.GetType().Name);
                            break;
                    }
                    if (NestBranch)
                    {
                        curvesListListRevit.Add(curvesListRevit);
                    }
                }

                if (!NestBranch)
                {
                    curvesListListRevit.Add(curvesListRevit);
                }

                return true;
            }
            catch (Exception exception) {
                Print("Exception in 'ReadDataTreeBranch': " + exception.Message);
                return false;
            }
        }

        // ************************************* Replicated Versions of GH C# Built-in Functions **********************************
        public void Print(string item) {
            this.outStrings.Add(item);
        }
        public void WriteOut() {
            this.dataAccess.SetDataList(0, this.outStrings);            
        }

        //// ********************************************** From GH C# Component ***************************************************

        //#region Members
        ///// <summary>List of error messages. Do not modify this list directly.</summary>
        //public List<string> __err = new List<string>();

        ///// <summary>List of print messages. Do not modify this list directly, use the Print() and Reflect() functions instead.</summary>
        //public List<string> __out = new List<string>();

        ///// <summary>Represents the current Rhino document.</summary>
        //public RhinoDoc doc = RhinoDoc.ActiveDoc;

        ///// <summary>Represents the Script component which maintains this script.</summary>
        //public IGH_ActiveObject owner;
        //#endregion
        //#region Utility functions
        ///// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
        ///// <param name="text">String to print.</param>
        //private void Print(string text) {
        //    __out.Add(text);
        //}

        ///// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
        ///// <param name="format">String format.</param>
        ///// <param name="args">Formatting parameters.</param>
        //public void Print(string format, params object[] args) {
        //    __out.Add(string.Format(format, args));
        //}

        ///// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
        ///// <param name="obj">Object instance to parse.</param>
        //public void Reflect(object obj) {
        //    __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj));
        //}

        ///// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
        ///// <param name="obj">Object instance to parse.</param>
        //public void Reflect(object obj, string method_name) {
        //    __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj, method_name));
        //}
        //#endregion
    }
}
