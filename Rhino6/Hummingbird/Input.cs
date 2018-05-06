using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;

using HummingbirdUtility;

namespace Hummingbird {
   
    // Used with toposurface; clean up points (Vertices)
    public struct TopoSurfVertex {
        public double x;
        public double y;
        public double z;
    }
    //Created Triangles, vv# are the vertex pointers
    public struct TopoSurfTriangle {
        public int vv0;
        public int vv1;
        public int vv2;
    }

    public class Input : GH_Component {

        // Enumeration and dictionary to enable switching on Hb types.
        private enum HbTypes { HbLine,  HbArc,  HbEllipse, HbNurbSpline}
        private Dictionary<Type, HbTypes> hbTypeDictionary = new Dictionary<Type, HbTypes> {
            {typeof(HbLine),       HbTypes.HbLine}, 
            {typeof(HbArc),        HbTypes.HbArc}, 
            {typeof(HbEllipse),    HbTypes.HbEllipse}, 
            {typeof(HbNurbSpline), HbTypes.HbNurbSpline}
        };

        private Utility utility;

        private DataTree<Point3d> dataTreePoints;
        private DataTree<Curve> dataTreeCurves;
        private DataTree<Brep> dataTreeBreps;
        private int indexPoints;
        private int indexCurves;
        private int indexBreps;

        bool createGeometryPoints = false;
        bool createGeometryCurves = false;
        bool createGeometrySurfaces = false;

        private double wallHeight = 10;

        public Input() : base("Input", "Input", "Read Revit Data Into Rhino", "Extra", "Hummingbird") {
            this.Hidden = false;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("ReadToggle", "Read", "Set to 'True' to read CSV file", GH_ParamAccess.item);                       // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                          // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be read", GH_ParamAccess.item);                            // 2 
            pManager.AddBooleanParameter("AddGeometryPoints", "Add P", "Set to 'True' to create point geometry", GH_ParamAccess.item);       // 3
            pManager[3].Optional = true;
            pManager.AddBooleanParameter("AddGeometryCurves", "Add C", "Set to 'True' to create curve geometry", GH_ParamAccess.item);       // 4
            pManager[4].Optional = true;
            pManager.AddBooleanParameter("AddGeometrySurfaces", "Add S", "Set to 'True' to create surface geometry", GH_ParamAccess.item);   // 5
            pManager[5].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "Out", "Output Messages", GH_ParamAccess.item);                                              // 0
            pManager.AddGeometryParameter("Points", "Pts", "Points output", GH_ParamAccess.tree);                                            // 1
            pManager.AddGeometryParameter("Curves", "Crv", "Curves output", GH_ParamAccess.tree);                                            // 2
            pManager.AddGeometryParameter("Surfaces", "Srf", "Surfaces output", GH_ParamAccess.tree);                                        // 3
        }

        protected override void SolveInstance(IGH_DataAccess DA) {            

            // Set up Utility object and start process
            this.utility = new Utility(DA);
            utility.Print("Starting Input.");

            // Switch to avoid running calculations every time
            bool read = false;

            // Get Inputs
            string folderPath = null, fileName = null;
            if (!this.utility.GetInput(0, ref read)) {              // Write command is required
                this.utility.WriteOut();
                return;
            }

            if (!this.utility.GetInput(1, ref folderPath)) {        // Folder path is required
                this.utility.WriteOut();
                return;
            }
            if (!this.utility.GetInput(2, ref fileName)) {          // File name is required
                this.utility.WriteOut();
                return;
            }
            DA.GetData(3, ref this.createGeometryPoints);          // Create geometry commands optional; default is true
            DA.GetData(4, ref this.createGeometryCurves);
            DA.GetData(5, ref this.createGeometrySurfaces);

            if (read) {
                string returnValue;
                try {
                    // Initialize the output lists
                    this.dataTreePoints = new DataTree<Point3d>();
                    this.dataTreeCurves = new DataTree<Curve>();
                    this.dataTreeBreps = new DataTree<Brep>();  // Used to create surfaces
                    this.indexPoints = 0;
                    this.indexCurves = 0;
                    this.indexBreps = 0;

                    // Setup the reader
                    CsvReader csvReader = new CsvReader();
                    this.utility.Print("CsvWriter Version: " + csvReader.Version);
                    if (!this.utility.EstablishCsvLink(csvReader, folderPath, fileName)) {
                        this.utility.Print("EstablishCsvLink() failed");
                        this.utility.WriteOut();
                        return;
                    }
                    // Read the file
                    returnValue = csvReader.ReadFile(); ;
                    if (returnValue != "") {
                        this.utility.Print("Error reading file: " + returnValue + ".");
                        this.utility.WriteOut();
                        return;
                    }
                    // Check syntax
                    returnValue = csvReader.CheckSyntax();
                    if (returnValue != "") {
                        System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                              "Syntax error detected in file: " + returnValue + "\n\n"
                            + "Select 'OK' to continue or 'Cancel' to stop processing.",
                            "Hummingbird", System.Windows.Forms.MessageBoxButtons.OKCancel);
                        if (dialogResult != System.Windows.Forms.DialogResult.OK) {
                            this.utility.Print("Processing cancelled by user.");
                            this.utility.WriteOut();
                            return;
                        }
                    }
                    // Get the pre-parsed data
                    returnValue = csvReader.GetInput();
                    List<Point3d> points;
                    // Process all of the items
                    foreach (HbInputItem hbInputItem in csvReader.HbInputItems) {
// TODO Various places we may want to eliminate duplicate points?
                        string hbAction = hbInputItem.CommandAction;
                        string hbObject = hbInputItem.CommandObject;
                        string hbModifier = hbInputItem.CommandModifier;
                        switch (hbAction) {
                            case "Add":
                                switch (hbObject) {
                                    case "Level":                                          
                                        points = new List<Point3d> {
                                        new Point3d(-10, -10, hbInputItem.DoubleValues[0]),
                                        new Point3d( 10, -10, hbInputItem.DoubleValues[0]),
                                        new Point3d( 10,  10, hbInputItem.DoubleValues[0]),
                                        new Point3d(-10,  10, hbInputItem.DoubleValues[0])};
                                        ProcessPlaneSurfaceByPoints(new Point3d(0, 0, hbInputItem.DoubleValues[0]), points, new Interval(-10, 10), new Interval(-10, 10));
                                        break;
                                    case "Grid":                          //hbInputItem.HbCurve                - HbLine or HbArc 
                                    case "DetailLine":                    //hbInputItem.HbCurve                - HbLine
                                    case "ModelLine":                                        
                                    case "DetailArc":                     //hbInputItem.HbCurve                - HbArc
                                    case "ModelArc":                                        
                                    case "DetailEllipse":                 //hbInputItem.HbCurve                - HbEllipse
                                    case "ModelEllipse":
                                    case "DetailNurbSpline":              //hbInputItem.HbCurve                - HbNurbSpline
                                    case "ModelNurbSpline":
                                    case "AreaBoundaryLine":
                                    case "RoomSeparationLine":
                                        switch (hbModifier) {
                                            case "":                      // Grid may be only one?
                                            case "Curve":                 //hbInputItem.HbCurve                - HbLine, HbArc, HbEllipse, or HbNurbSpline
                                                    ProcesssCurveItem(hbInputItem);
                                                break;
                                            case "CurveArrArray":         //hbInputItem.HbCurveArrArray        - (multiple HbLine, HbArc, HbNurbSpline)
                                                ProcessCurveArrArray(hbInputItem.HbCurveArrArray);    // various kinds of lines, vertical wall, or planar floor                                         
                                                break;
                                        }
                                        break;
                                    case "TopographySurface":             //hbInputItem.HbXyzList              - (multiple HbXYZ)
                                        points = new List<Point3d>();
                                        foreach (HbXYZ hbXYZ in hbInputItem.ListHbXYZ) {
                                            points.Add(new Point3d(hbXYZ.X, hbXYZ.Y, hbXYZ.Z));
                                        }
                                        ProcessTopographySurface(points);
                                        break;
                                    case "AdaptiveComponent":                                        
                                        points = new List<Point3d>();
                                        foreach (HbXYZ hbXYZ in hbInputItem.ListHbXYZ) {
                                            points.Add(new Point3d(hbXYZ.X, hbXYZ.Y, hbXYZ.Z));
                                        }
                                        ProcessSinglePoints(points);
                                        break;
                                    case "Wall":
                                        switch (hbModifier) {
                                            case "Curve":                 // hbInputItem.hbCurve               - HbLine or HbArc                                                  
                                                ProcesssCurveItem(hbInputItem, wallHeight);
                                                break;
                                            case "CurveArrArray":         // hbInputItem.HbCurveArrArray       - (multiple HbLine, HbArc, HbNurbSpline)
                                                ProcessCurveArrArray(hbInputItem.HbCurveArrArray);                                
                                                break;
                                        }
                                        break;
                                    case "Floor":
                                    case "FilledRegion":
                                        switch (hbModifier) {
                                            case "Points":                // hbInputItem.HbXyzList             - (multiple HbXYZ)
                                                ProcessPointsToBrep(hbInputItem.ListHbXYZ[0], hbInputItem.ListHbXYZ[1], hbInputItem.ListHbXYZ[2], hbInputItem.ListHbXYZ[3]);               
                                                break;
                                            case "CurveArrArray":         // hbInputItem.HbCurveArrayArray     - (multiple HbLine, HbArc, HbNurbSpline)
                                                ProcessCurveArrArray(hbInputItem.HbCurveArrArray);                                       
                                                break;
                                        }
                                        break;
                                    case "FamilyInstance":                //hbInputItem.HbXyzList              - (multiple HbXYZ) 
                                    case "Column":                        //                                     One for family instance, area, and room; 
                                    case "Beam":                          //                                     One or two with column and beam.
                                    case "Area":
                                    case "Room":
                                        points = new List<Point3d>();
                                        points.Add(new Point3d(hbInputItem.ListHbXYZ[0].X, hbInputItem.ListHbXYZ[0].Y, hbInputItem.ListHbXYZ[0].Z));
                                        if (hbInputItem.ListHbXYZ.Count == 2) points.Add( new Point3d(hbInputItem.ListHbXYZ[1].X, hbInputItem.ListHbXYZ[1].Y, hbInputItem.ListHbXYZ[1].Z));
                                        ProcessSinglePoints(points);
                                        break;
                                    case "ReferencePoint":                //hbInputItem.HbReferencePoint       - HbReferencePoint
                                        points = new List<Point3d>();     //                                     Apparently only a single point is allowed?
                                        points.Add(new Point3d(hbInputItem.HbReferencePoint.X, hbInputItem.HbReferencePoint.Y, hbInputItem.HbReferencePoint.Z));
                                        ProcessSinglePoints(points);
                                        break;
                                    case "CurveByPoints":                 //hbInputItem.HbReferenceArray       - (multiple HbReferencePoint)
                                        ProcessCurveByPoints(hbInputItem.HbReferenceArray);             
                                        break;
                                    case "LoftForm":                      //hbInputItem.HbReferenceArrayArray  - (multiple HbReferenceArray)
//TODO More work                                        
                                        ProcessLoftForm(hbInputItem.HbReferenceArrayArray);
                                        break;
                                    case "FamilyExtrusion":
//TODO More work
                                        //hbInputItem.ListHbXyz[0]           - HbXYZ (insertion point, optional)
                                        //hbInputItem.HbCurveArrayArray      - (multiple HbLine, HbArc, HbNurbSpline)

                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "Set":    // Most cases are not handled
                                switch (hbObject) {
                                    case "WallHeight":
                                        wallHeight = hbInputItem.DoubleValues[0];
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            default:  // Case "Modify" is not being handled
                                break;
                        }

                    }

                    // Write the output geometry
                    // TODO: This would be a good place to eliminate duplicate geometry.  Maybe add a bool switch as to whether or not to do that?

                    DA.SetDataTree(1, this.dataTreePoints);
                    DA.SetDataTree(2, this.dataTreeCurves);
                    DA.SetDataTree(3, this.dataTreeBreps);              

                    // Write the output messages
                    this.utility.Print("Input completed successfully.");
                    this.utility.WriteOut();
                }
                catch (Exception exception) {
                    this.utility.Print(exception.Message);
                    this.utility.WriteOut();
                }
            }
        }

        // ----------------------------------------------------------- Local Subroutines for Solve Instance ----------------------------------------------

        private bool ProcessSinglePoints(List<Point3d> points) {
            try {
                foreach (Point3d point3d in points) {
                    // Add to outputs
                    this.dataTreePoints.AddRange(new List<Point3d> { point3d }, new GH_Path(indexPoints));
                    indexPoints++;
                    // Create geometry
                    if (createGeometryPoints) Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(point3d);                        
                }
                return true;
            }
            catch {
                return false;
            }
        }

        private bool ProcesssCurveItem(HbInputItem hbInputItem) {
            return ProcesssCurveItem(hbInputItem, 0);  // 0 is code to not try to make a vertical extrusion
        }

        private bool ProcesssCurveItem(HbInputItem hbInputItem, double verticalHeight) {
            try {
                List<Point3d> points;
                Curve curve;
                Extrusion extrusion = null;
                //Brep brep = null;
                HbToRhino(hbInputItem.HbCurve, out points, out curve);

                // Add to outputs
                this.dataTreePoints.AddRange(points, new GH_Path(indexPoints));
                indexPoints++;
                this.dataTreeCurves.AddRange(new List<Curve> { curve }, new GH_Path(indexCurves));
                indexCurves++;
                if (verticalHeight > 0) {
                    extrusion = Extrusion.Create(curve, verticalHeight, false);
                    this.dataTreeBreps.AddRange(new List<Brep> { Brep.CreateFromSurface(extrusion) }, new GH_Path(indexBreps));
                    indexBreps++;
                }

                // Create Geometry
                if (createGeometryPoints) {
                    if (points != null) {
                        foreach (Point3d point3d in points) {
                            Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(point3d);
                        }
                    }
                }
                if (createGeometryCurves) {
                    if (curve != null) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddCurve(curve);
                    }
                }
                if (createGeometrySurfaces) {
                    if (extrusion != null) Rhino.RhinoDoc.ActiveDoc.Objects.AddExtrusion(extrusion);
                }                
                return true;
            }
            catch {
                return false;
            }
        }

        private bool ProcessPlaneSurfaceByPoints(Point3d center, List<Point3d> points, Interval intervalX, Interval intervalY) {
            try {
                
                // Calculate
                List<LineCurve> lineCurves = new List<LineCurve>();
                switch (points.Count) {
                    case 0:
                        return false;
                    case 1:
                        break;
                    default:
                        Point3d pointFirst = new Point3d(0, 0, 0);    // Dummy value for compiler
                        Point3d pointPrevious = new Point3d(0, 0, 0); // Dummy value for compiler
                        bool first = true;
                        foreach (Point3d point in points) {
                            if (first) {
                                pointFirst = point;
                                first = false;
                            }
                            else lineCurves.Add(new LineCurve(new Line(pointPrevious, point)));
                            pointPrevious = point;
                        }
                        lineCurves.Add(new LineCurve(new Line(pointPrevious, pointFirst)));      
                        break;
                }
                PlaneSurface rhPlaneSurface = new PlaneSurface(new Plane(center, new Vector3d(0, 0, 1)), intervalX, intervalY);

                // Add to outputs
                this.dataTreePoints.AddRange(points, new GH_Path(indexPoints));
                indexPoints++;
                if (lineCurves.Count > 0) {
                    this.dataTreeCurves.AddRange(lineCurves, new GH_Path(indexCurves));
                    indexCurves++;
                }
                this.dataTreeBreps.AddRange(new List<Brep> { Brep.CreateFromSurface(rhPlaneSurface) }, new GH_Path(indexBreps));
                indexBreps++;

                // Create geometry
                if (createGeometryPoints) {
                    foreach (Point3d point in points) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(point);
                    }
                }
                if (createGeometryCurves) {
                    foreach (LineCurve lineCurve in lineCurves) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddCurve(lineCurve);
                    }
                }
                if (createGeometrySurfaces) {
                    Rhino.RhinoDoc.ActiveDoc.Objects.AddSurface(rhPlaneSurface);
                }
                return true;
            }
            catch {
                return false;
            }
        }

        private bool ProcessPointsToBrep(HbXYZ hbXYZ1, HbXYZ hbXYZ2, HbXYZ hbXYZ3, HbXYZ hbXYZ4) {   // planar floor
            try {

                double brepTolerence = GH_Component.DocumentTolerance();  // Not sure what this is but suggested by McNeel
                if (brepTolerence <= 0.0) brepTolerence = 0.001;          // Since we are not sure what the effect would be

                if (hbXYZ1 == null || hbXYZ2 == null || hbXYZ3 == null || hbXYZ4 == null) return true; // Don't process bad data
                List<Point3d> points = new List<Point3d> {
                    new Point3d(hbXYZ1.X, hbXYZ1.Y, hbXYZ1.Z),
                    new Point3d(hbXYZ2.X, hbXYZ2.Y, hbXYZ2.Z),
                    new Point3d(hbXYZ3.X, hbXYZ3.Y, hbXYZ3.Z),
                    new Point3d(hbXYZ4.X, hbXYZ4.Y, hbXYZ4.Z) };
                List<Curve> curves = new List<Curve>() {
                    new LineCurve(points[0], points[1]),
                    new LineCurve(points[1], points[2]),
                    new LineCurve(points[2], points[3]),
                    new LineCurve(points[3], points[0]) };

                PolyCurve polyCurve = new PolyCurve();
                foreach (LineCurve curve in curves) {
                    polyCurve.Append(curve);
                }

                // Obsolete w Revit 6
                // Brep brep = Brep.CreatePlanarBreps(curves)[0];
                Brep brep = Brep.CreatePlanarBreps(curves, brepTolerence)[0];

                // Add to outputs
                this.dataTreePoints.AddRange(points, new GH_Path(indexPoints));
                indexPoints++;
                this.dataTreeCurves.AddRange(curves, new GH_Path(indexCurves));
                indexCurves++;
                this.dataTreeBreps.AddRange(new List<Brep> { brep }, new GH_Path(indexBreps));
                indexBreps++;

                // Create Geometry
                if (createGeometryPoints) {
                    foreach (Point3d point3d in points) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(point3d);
                    }
                }
                if (createGeometryCurves) {
                    foreach (Curve curveItem in curves) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddCurve(curveItem);
                    }
                }
                if (createGeometrySurfaces) {
                    Rhino.RhinoDoc.ActiveDoc.Objects.AddBrep(brep);
                }
                return true;
            }
            catch {
                return false;
            }

        }

        private bool ProcessCurveArrArray(HbCurveArrArray hbCurveArrArray) {   // various kinds of lines, vertical wall, or planar floor
            try {

                double brepTolerence = GH_Component.DocumentTolerance();  // Not sure what this is but suggested by McNeel
                if (brepTolerence <= 0.0) brepTolerence = 0.001;          // Since we are not sure what the effect would be

                List<Point3d> pointsInnerLoop;
                Curve curveInnerLoop;
                List<Point3d> points = new List<Point3d>();
                List<Curve> curves = new List<Curve>();
                PolyCurve polyCurve;
                foreach (HbCurveArray hbCurveArray in hbCurveArrArray) {
                    //List<Curve> curvesInnerLoop = new List<Curve>();
                    polyCurve = new PolyCurve();
                    foreach (HbCurve hbCurve in hbCurveArray) {                        
                        HbTypes hbType;
                        if (hbTypeDictionary.TryGetValue(hbCurve.GetType(), out hbType)) {
                            HbToRhino(hbCurve, out pointsInnerLoop, out curveInnerLoop);
                            if (pointsInnerLoop != null) {
                                foreach (Point3d point3d in pointsInnerLoop) {
                                    points.Add(point3d);
                                }
                            }
                            if (curveInnerLoop != null) {
                                polyCurve.Append(curveInnerLoop);
                            }
                        }
                    }
                    if (polyCurve.SegmentCount > 0) {
                        curves.Add(polyCurve);
                    }

                }
                // Obsolete w Revit 6
                // Brep brep = Brep.CreatePlanarBreps(curves)[0];
                Brep brep = Brep.CreatePlanarBreps(curves, brepTolerence)[0];

                // Add to outputs
                this.dataTreePoints.AddRange(points, new GH_Path(indexPoints));
                indexPoints++;
                this.dataTreeCurves.AddRange(curves, new GH_Path(indexCurves));
                indexCurves++;

                this.dataTreeBreps.AddRange(new List<Brep> { brep }, new GH_Path(indexBreps));
                indexBreps++;

                // Create Geometry
                if (createGeometryPoints) {
                    foreach (Point3d point3d in points) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(point3d);
                    }
                }
                if (createGeometryCurves) {
                    foreach (Curve curveItem in curves) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddCurve(curveItem);
                    }
                }
                if (createGeometrySurfaces) {
                    if (brep.Faces.Count > 0) Rhino.RhinoDoc.ActiveDoc.Objects.AddBrep(brep);
                } 

                return true;
            }
            catch {
                return false;
            }
        }

        private bool ProcessTopographySurface(List<Point3d> points) {
            try {
                //int MaxVertices = 500;
                //int MaxTriangles = 1000;
                // TODO: Not sure if these estimates are good or how the system will handle going over.
                int MaxVertices = points.Count * 2;
                int MaxTriangles = MaxVertices * 2;

                // Points used.  !!Note: is a 1-based array
                TopoSurfVertex[] topoSurfVertex = new TopoSurfVertex[MaxVertices];
                // Triangles created. !!Note: is a 1-based array
                TopoSurfTriangle[] topoSurfTriangle = new TopoSurfTriangle[MaxTriangles];

                int numberOfPoints = 0;
                int numberOfTriangles = 0;            
                foreach (Point3d point3d in points) {
                    numberOfPoints++;
                    topoSurfVertex[numberOfPoints].x = point3d.X;
                    topoSurfVertex[numberOfPoints].y = point3d.Y;
                    topoSurfVertex[numberOfPoints].z = point3d.Z;                    
                }

                TriangulatePoints triangulatePoints = new TriangulatePoints(ref topoSurfVertex, out topoSurfTriangle, numberOfPoints, MaxVertices, MaxTriangles, ref numberOfTriangles);
                List<Curve> curves = new List<Curve>();
                for (int i = 1; i <= numberOfTriangles; i++) {
                    Line line = new Line(topoSurfVertex[topoSurfTriangle[i].vv0].x, topoSurfVertex[topoSurfTriangle[i].vv0].y, topoSurfVertex[topoSurfTriangle[i].vv0].z, topoSurfVertex[topoSurfTriangle[i].vv1].x, topoSurfVertex[topoSurfTriangle[i].vv1].y, topoSurfVertex[topoSurfTriangle[i].vv1].z);
                    curves.Add(new LineCurve(line));
                    line =      new Line(topoSurfVertex[topoSurfTriangle[i].vv1].x, topoSurfVertex[topoSurfTriangle[i].vv1].y, topoSurfVertex[topoSurfTriangle[i].vv1].z, topoSurfVertex[topoSurfTriangle[i].vv2].x, topoSurfVertex[topoSurfTriangle[i].vv2].y, topoSurfVertex[topoSurfTriangle[i].vv2].z);
                    curves.Add(new LineCurve(line));
                    line =      new Line(topoSurfVertex[topoSurfTriangle[i].vv0].x, topoSurfVertex[topoSurfTriangle[i].vv0].y, topoSurfVertex[topoSurfTriangle[i].vv0].z, topoSurfVertex[topoSurfTriangle[i].vv2].x, topoSurfVertex[topoSurfTriangle[i].vv2].y, topoSurfVertex[topoSurfTriangle[i].vv2].z);
                    curves.Add(new LineCurve(line));
                }
                Mesh mesh = new Mesh();
                for (int i = 1; i <= numberOfPoints; i++) {
                    mesh.Vertices.Add(new Point3d(topoSurfVertex[i].x, topoSurfVertex[i].y, topoSurfVertex[i].z));
                }
                for (int i = 1; i <= numberOfTriangles; i++) {  // Note subtracting 1 from index due to 1-based array
                    mesh.Faces.AddFace(topoSurfTriangle[i].vv0-1, topoSurfTriangle[i].vv1-1, topoSurfTriangle[i].vv2-1);
                }
                mesh.Normals.ComputeNormals();
                mesh.Compact();
                Brep brep = Brep.CreateFromMesh(mesh, true);
                    
                // Add to outputs
                this.dataTreePoints.AddRange(points, new GH_Path(indexPoints));
                indexPoints++;
                this.dataTreeCurves.AddRange(curves, new GH_Path(indexCurves));
                indexCurves++;
                this.dataTreeBreps.AddRange(new List<Brep> { brep }, new GH_Path(indexBreps));
                indexBreps++;

                // Create geometry
                if (createGeometryPoints) {
                    foreach (Point3d point3d in points) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(point3d);
                    }
                }
                if (createGeometryCurves) {
                    foreach (Curve curveItem in curves) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddCurve(curveItem);
                    }
                }
                if (createGeometrySurfaces) {
                    Rhino.RhinoDoc.ActiveDoc.Objects.AddBrep(brep);
                }

                return true;
            }
            catch {
                return false;
            }
        }

        public bool ProcessCurveByPoints(HbReferenceArray hbReferenceArray) {
            try {
                List<Point3d> points = new List<Point3d>();
                foreach (HbReferencePoint hbReferencePoint in hbReferenceArray) {
                    points.Add(new Point3d(hbReferencePoint.X, hbReferencePoint.Y, hbReferencePoint.Z));
                }
                Curve curve = NurbsCurve.Create(false, 3, points);

                // Add to outputs
                this.dataTreePoints.AddRange(points, new GH_Path(indexPoints));
                indexPoints++;
                this.dataTreeCurves.AddRange(new List<Curve> { curve }, new GH_Path(indexCurves));
                indexCurves++;

                // Create geometry
                if (createGeometryPoints) {
                    foreach (Point3d point3d in points) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(point3d);
                    }
                }
                if (createGeometryCurves) {
                    Rhino.RhinoDoc.ActiveDoc.Objects.AddCurve(curve);
                }
                return true;
            }
            catch {
                return false;
            }
        }

        public bool ProcessLoftForm(HbReferenceArrayArray hbReferenceArrayArray) {  //hbInputItem.HbReferenceArrayArray  - (multiple HbReferenceArray)
            try {
                List<Point3d> points = new List<Point3d>();
                List<Curve> curves = new List<Curve>();
                Brep brep = null;
                int uCount = hbReferenceArrayArray.Count();
                int vCount = 0;  // Dummy value for compiler
                bool first = true;
                foreach (HbReferenceArray hbReferenceArray in hbReferenceArrayArray) {
                    if (first) {
                        vCount = hbReferenceArray.Count();
                        first = false;
                    }
                    List<Point3d> pointsCurve = new List<Point3d>();
                    foreach (HbReferencePoint hbReferencePoint in hbReferenceArray) {
                        Point3d point3d = new Point3d(hbReferencePoint.X, hbReferencePoint.Y, hbReferencePoint.Z);
                        points.Add(point3d);
                        pointsCurve.Add(point3d);
                    }
                    curves.Add(NurbsCurve.Create(false, 3, pointsCurve));
                }
                try {
                    NurbsSurface nurbsSurface = NurbsSurface.CreateFromPoints(points, uCount, vCount, 3, 3);
                    brep = nurbsSurface.ToBrep();
                }
                catch { }
                

                // Add to outputs
                this.dataTreePoints.AddRange(points, new GH_Path(indexPoints));
                indexPoints++;
                this.dataTreeCurves.AddRange(curves, new GH_Path(indexCurves));
                indexCurves++;
                if (brep != null) {
                    this.dataTreeBreps.AddRange(new List<Brep> { brep }, new GH_Path(indexBreps));
                    indexBreps++;
                }

                // Create geometry
                if (createGeometryPoints) {
                    foreach (Point3d point3d in points) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(point3d);
                    }
                }
                if (createGeometryCurves) {
                    foreach (Curve curve in curves) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddCurve(curve);
                    }
                }
                if (createGeometrySurfaces) {
                    if (brep != null) {
                        Rhino.RhinoDoc.ActiveDoc.Objects.AddBrep(brep);
                    }
                }
                return true;
            }
            catch {
                return false;
            }
        }


        // ------------------------------------------------------------ Conversion Utilities ---------------------------------------------------------------------

         private bool HbToRhino(Object hbObject, out List<Point3d> points, out Curve curve) {
            points = null;
            curve = null;
            try {
                HbTypes hbType;
                if (hbTypeDictionary.TryGetValue(hbObject.GetType(), out hbType)) {
                    switch (hbType) {
                        case HbTypes.HbLine:
                            points = new List<Point3d>();
                            HbLine hbLine = (HbLine)hbObject;
                            points.Add(new Point3d(hbLine.PointStart.X, hbLine.PointStart.Y, hbLine.PointStart.Z));
                            points.Add(new Point3d(hbLine.PointEnd.X, hbLine.PointEnd.Y, hbLine.PointEnd.Z));
                            Line rhLine = new Line(points[0], points[1]);
                            curve = new LineCurve(rhLine);
                            break;
                        case HbTypes.HbArc:
                            points = new List<Point3d>();
                            HbArc hbArc = (HbArc)hbObject;
                            points.Add(new Point3d(hbArc.PointStart.X, hbArc.PointStart.Y, hbArc.PointStart.Z));
                            points.Add(new Point3d(hbArc.PointMid.X, hbArc.PointMid.Y, hbArc.PointMid.Z));
                            points.Add(new Point3d(hbArc.PointEnd.X, hbArc.PointEnd.Y, hbArc.PointEnd.Z));
                            curve = new ArcCurve(new Arc(points[0], points[1], points[2]));
                            break;
                        case HbTypes.HbEllipse:
                            //points = new List<Point3d>();
                            //HbEllipse hbEllipse = (HbEllipse)hbObject;
                            //HbXYZ hbXYZ1 = hbEllipse.PointFirst;
                            //HbXYZ hbXYZ2 = hbEllipse.PointSecond;
                            //points.Add(new Point3d(hbXYZ1.X, hbXYZ1.Y, hbXYZ1.Z));
                            //points.Add(new Point3d(hbXYZ1.X, hbXYZ1.Y, hbXYZ1.Z));
                            //double radiusY = hbEllipse.RadiusY;
                            //if (hbEllipse.Mode.ToLower() == "full") {
                            //    Ellipse ellipse = new Ellipse(
                            //}
                            //else {  // "half" case
                            //}
    //TODO finish; requires some geometry

                            break;
                        case HbTypes.HbNurbSpline:
                            points = new List<Point3d>();
                            HbNurbSpline hbNurbSpline = (HbNurbSpline)hbObject;
                            int numberOfPoints = hbNurbSpline.Points.Count;

                            foreach (HbXYZ hbXYZ in hbNurbSpline.Points) {
                                points.Add(new Point3d(hbXYZ.X, hbXYZ.Y, hbXYZ.Z));
                            }
                            curve = NurbsCurve.Create(false, 3, points);
                        break;
                        default:   // Ignore unknown cases

                            break;
                    }
                }
                 return true;
             }
             catch {
                 return false;
             }
         }

        // ------------------------------------------------------- General housekeeping for all components ----------------------------------------------------------

        public override GH_Exposure Exposure {
            get { return GH_Exposure.quarternary; }
        }

        public override Guid ComponentGuid {
            get { return new Guid("77257A76-4455-4955-86B1-9D541A435CE6"); }
        }

        protected override Bitmap Icon {
            get { return new Bitmap(Hummingbird.Properties.Resources.Input); }
        }

    }
}
