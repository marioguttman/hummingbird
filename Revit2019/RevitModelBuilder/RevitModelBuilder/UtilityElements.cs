using RevitUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using HummingbirdUtility;  // For HbReference definitions

namespace RevitModelBuilder {
    public class UtilityElements {

        #region Development Notes                       // ****************************** Development Notes **************************************************

        // Our use of levels is confusing.  Some things get set to the currentl level (like placing a floor) but is their position
        // based on the floor or World coordinates?

        #endregion

        #region Module Variables                        // ****************************** Module Variables *****************************************************

        public bool TrapErrors { set; get; }
        public string InnerErrorMessage { set; get; }
        public string InnerErrorSeverity { set; get; }
        public string LocalErrorMessage { set; get; }

        public SortedDictionary<string, Level> Levels { get { return levels; } }
        public SortedDictionary<string, WallType> WallTypes { get { return wallTypes; } }
        public SortedDictionary<string, FloorType> FloorTypes { get { return floorTypes; } }
        public SortedDictionary<string, FilledRegionType> FilledRegionTypes { get { return filledRegionTypes; } }
        public SortedDictionary<string, SortedDictionary<string, FamilySymbol>> FamilyTypes { get { return familyTypes; } }
        public SortedDictionary<string, SortedDictionary<string, FamilySymbol>> ColumnArchitecturalTypes { get { return columnArchitecturalTypes; } }
        public SortedDictionary<string, SortedDictionary<string, FamilySymbol>> ColumnStructuralTypes { get { return columnStructuralTypes; } }
        public SortedDictionary<string, SortedDictionary<string, FamilySymbol>> BeamTypes { get { return beamTypes; } }
        public SortedDictionary<string, SortedDictionary<string, FamilySymbol>> AdaptiveComponentTypes { get { return adaptiveComponentTypes; } }
        public SortedDictionary<string, SortedDictionary<string, MullionType>> MullionTypes { get { return mullionTypes; } }


        public Level LevelCurrent { set { levelCurrent = value; } get { return levelCurrent; } }
        public WallType WallTypeCurrent { set { wallTypeCurrent = value; } get { return wallTypeCurrent; } }
        public double WallHeightCurrent { set { wallHeightCurrent = value; } get { return wallHeightCurrent; } }
        public FloorType FloorTypeCurrent { set { floorTypeCurrent = value; } get { return floorTypeCurrent; } }
        public FilledRegionType FilledRegionTypeCurrent { set { mFilledRegionTypeCurrent = value; } get { return mFilledRegionTypeCurrent; } }
        public string FamilyFamilyCurrent { set { familyFamilyCurrent = value; } get { return familyFamilyCurrent; } }
        public string FamilyTypeCurrent { set { familyTypeCurrent = value; } get { return familyTypeCurrent; } }
        public double FamilyRotationCurrent { set { familyRotationCurrent = value; } get { return familyRotationCurrent; } }
        public bool FamilyMirrorXCurrent { set { familyMirrorXCurrent = value; } get { return familyMirrorXCurrent; } }
        public bool FamilyMirrorYCurrent { set { familyMirrorYCurrent = value; } get { return familyMirrorYCurrent; } }
        public bool FamilyFlipHandCurrent { set { familyFlipHandCurrent = value; } get { return familyFlipHandCurrent; } }
        public bool FamilyFlipFacingCurrent { set { familyFlipFacingCurrent = value; } get { return familyFlipFacingCurrent; } }
        public string ColumnModeCurrent { set { columnModeCurrent = value; } get { return columnModeCurrent; } }
        public string ColumnFamilyCurrent { set { columnFamilyCurrent = value; } get { return columnFamilyCurrent; } }
        public string ColumnTypeCurrent { set { columnTypeCurrent = value; } get { return columnTypeCurrent; } }
        public double ColumnHeightCurrent { set { columnHeightCurrent = value; } get { return columnHeightCurrent; } }
        public double ColumnRotationCurrent { set { columnRotationCurrent = value; } get { return columnRotationCurrent; } }
        public string BeamFamilyCurrent { set { beamFamilyCurrent = value; } get { return beamFamilyCurrent; } }
        public string BeamTypeCurrent { set { beamTypeCurrent = value; } get { return beamTypeCurrent; } }
        public double BeamRotationCurrent { set { beamRotationCurrent = value; } get { return beamRotationCurrent; } }
        public string BeamJustificationCurrent { set { beamJustificationCurrent = value; } get { return beamJustificationCurrent; } }
        public string AdaptiveComponentFamilyCurrent { set { adaptiveComponentFamilyCurrent = value; } get { return adaptiveComponentFamilyCurrent; } }
        public string AdaptiveComponentTypeCurrent { set { adaptiveComponentTypeCurrent = value; } get { return adaptiveComponentTypeCurrent; } }
        public double FamilyExtrusionHeightCurrent { set { familyExtrusionHeightCurrent = value; } get { return familyExtrusionHeightCurrent; } }
        public string MullionFamilyCurrent { set { mullionFamilyCurrent = value; } get { return mullionFamilyCurrent; } }
        public string MullionTypeCurrent { set { mullionTypeCurrent = value; } get { return mullionTypeCurrent; } }

        private UtilitySettings settings;
        private Document documentDb;

        private SortedDictionary<string, SortedDictionary<string, FamilySymbol>> familyTypes = new SortedDictionary<string, SortedDictionary<string, FamilySymbol>>();
        private SortedDictionary<string, Level> levels = new SortedDictionary<string, Level>();
        private SortedDictionary<string, WallType> wallTypes = new SortedDictionary<string, WallType>();
        private SortedDictionary<string, FloorType> floorTypes = new SortedDictionary<string, FloorType>();
        private SortedDictionary<string, FilledRegionType> filledRegionTypes = new SortedDictionary<string, FilledRegionType>();
        private SortedDictionary<string, SortedDictionary<string, FamilySymbol>> columnArchitecturalTypes = new SortedDictionary<string, SortedDictionary<string, FamilySymbol>>();
        private SortedDictionary<string, SortedDictionary<string, FamilySymbol>> columnStructuralTypes = new SortedDictionary<string, SortedDictionary<string, FamilySymbol>>();
        private SortedDictionary<string, SortedDictionary<string, FamilySymbol>> beamTypes = new SortedDictionary<string, SortedDictionary<string, FamilySymbol>>();
        private SortedDictionary<string, SortedDictionary<string, FamilySymbol>> adaptiveComponentTypes = new SortedDictionary<string, SortedDictionary<string, FamilySymbol>>();
        private SortedDictionary<string, SortedDictionary<string, MullionType>> mullionTypes = new SortedDictionary<string, SortedDictionary<string, MullionType>>();


        private FailureHandler failureHandler;
        private FailureHandlingOptions failureHandlingOptions;
       
        private Level levelCurrent;
        private WallType wallTypeCurrent;
        private double wallHeightCurrent;
        private FloorType floorTypeCurrent;
        private FilledRegionType mFilledRegionTypeCurrent;
        private string familyFamilyCurrent;
        private string familyTypeCurrent;
        private double familyRotationCurrent;
        private bool familyMirrorXCurrent;
        private bool familyMirrorYCurrent;
        private bool familyFlipHandCurrent;
        private bool familyFlipFacingCurrent;
        private string columnModeCurrent;
        private string columnFamilyCurrent; 
        private string columnTypeCurrent;    
        private double columnHeightCurrent;  
        private double columnRotationCurrent; 
        private string beamFamilyCurrent;    
        private string beamTypeCurrent;      
        private double beamRotationCurrent;   
        private string beamJustificationCurrent;   
        private string adaptiveComponentFamilyCurrent;
        private string adaptiveComponentTypeCurrent;
        private double familyExtrusionHeightCurrent;
        private string mullionFamilyCurrent;
        private string mullionTypeCurrent;


        private Dictionary<Family, int[]>  AdaptivePointOrders = new Dictionary<Family, int[]>();

        #endregion

        #region Constructor                             // ****************************** Constructor **********************************************************

        public UtilityElements(UtilitySettings utilitySettings) {
            this.settings = utilitySettings;
            this.documentDb = utilitySettings.DocumentDb;
            TrapErrors = false;

            GetValues();

            //// Set origin at 0,0,0 unless specified otherwise later
            //OriginX = 0.0; OriginY = 0.0; OriginZ = 0.0;
            //mUseOrigin = false;

            //Set default level as current level if plan view, or lowest level if 3D view.
            if (this.settings.CurrentLevel != null) this.levelCurrent = this.settings.CurrentLevel;
            else {
                this.levelCurrent = this.levels.First().Value;
                foreach (string levelName in this.levels.Keys) {
                    if (this.levels[levelName].Elevation < this.levelCurrent.Elevation) this.levelCurrent = this.levels[levelName];
                }
            }

            this.wallHeightCurrent = 1;  //Practical value as default

        }

        #endregion

        #region Public Functions                        // ****************************** Public Functions *******************************************************

        public void InitializeSettings() {  // Designed for use with ElementToExcel
            LevelCurrent = null;
            FloorTypeCurrent = null;
            WallTypeCurrent = null;
            FilledRegionTypeCurrent = null;
            WallHeightCurrent = 0;            
            FamilyFamilyCurrent = "";
            FamilyTypeCurrent = "";
            FamilyRotationCurrent= 0;
            FamilyMirrorXCurrent = false;
            FamilyMirrorYCurrent = false;
            FamilyFlipHandCurrent = false;
            FamilyFlipFacingCurrent = false;
            ColumnModeCurrent = "";
            ColumnFamilyCurrent = "";
            ColumnTypeCurrent = "";
            ColumnHeightCurrent = 0;
            ColumnRotationCurrent = 0;
            BeamFamilyCurrent = "";
            BeamTypeCurrent = "";
            BeamRotationCurrent = 0;
            BeamJustificationCurrent = "Center";
            AdaptiveComponentFamilyCurrent = "";
            AdaptiveComponentTypeCurrent = "";
            this.familyExtrusionHeightCurrent = 0;
            this.mullionFamilyCurrent= "";
            this.mullionTypeCurrent= "";
        }

        // **************** Delete Element ***************
        public void Delete(Element element) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Deleting Element");
                try {
                    this.settings.DocumentDb.Delete(element.Id);
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                }
                return;
            }
        }

        // **************** Settings *********************
        public bool SetLevel(string levelName) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            if (!this.levels.ContainsKey(levelName)) return false;
            this.levelCurrent = this.levels[levelName];
            return true;
        }
        public bool SetWallType(string wallTypeName) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            if (!this.wallTypes.ContainsKey(wallTypeName)) return false;
            this.wallTypeCurrent = this.wallTypes[wallTypeName];
            return true;
        }
        public bool SetWallHeight(double height) {
            this.wallHeightCurrent = height;
            return true;
        }
        public bool SetFloorType(string floorTypeName) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            if (!this.floorTypes.ContainsKey(floorTypeName)) return false;
            this.floorTypeCurrent = this.floorTypes[floorTypeName];
            return true;
        }
        public bool SetFilledRegionType(string filledRegionTypeName) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            if (!this.filledRegionTypes.ContainsKey(filledRegionTypeName)) return false;
            mFilledRegionTypeCurrent = this.filledRegionTypes[filledRegionTypeName];
            return true;
        }
        public bool SetFamilyType(string familyName, string typeName) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            if (this.familyTypes.ContainsKey(familyName)) {
                if (this.familyTypes[familyName].ContainsKey(typeName)) {
                    this.familyFamilyCurrent = familyName;
                    this.familyTypeCurrent = typeName;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public bool SetFamilyRotation(double rotation) {
            this.familyRotationCurrent = rotation;
            return true;
        }
        public bool SetFamilyMirrored(bool mirrorX, bool mirrorY) {
            this.familyMirrorXCurrent = mirrorX;
            this.familyMirrorYCurrent = mirrorY;
            return true;
        }
        public bool SetFamilyFlipped(bool flipHand, bool flipFacing) {
            this.familyFlipHandCurrent = flipHand;
            this.familyFlipFacingCurrent = flipFacing;
            return true;
        }
        public bool SetColumnMode(string mode, string familyName, string typeName) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            if ((mode != null) && (familyName != null) && (typeName != null)) {
                this.columnModeCurrent = mode;
                switch (mode) {
                    case "Architectural":
                        if (this.columnArchitecturalTypes.ContainsKey(familyName)) {
                            if (this.columnArchitecturalTypes[familyName].ContainsKey(typeName)) {
                                this.columnFamilyCurrent = familyName;
                                this.columnTypeCurrent = typeName;
                            }
                            else return false;
                        }
                        else return false;
                        return true;
                    case "StructuralVertical":
                    case "StructuralPointDriven":
                    case "StructuralAngleDriven":
                        if (this.columnStructuralTypes.ContainsKey(familyName)) {
                            if (this.columnStructuralTypes[familyName].ContainsKey(typeName)) {
                                this.columnFamilyCurrent = familyName;
                                this.columnTypeCurrent = typeName;
                            }
                            else return false;
                        }
                        else return false;
                        return true;
                    default:
                        System.Windows.Forms.MessageBox.Show("Programming error:  Unknown case in 'UtilityElements.SetColumnMode'.");
                        return false;
                }
            }
            else return false;
        }
        public bool SetColumnHeight(double height) {
            this.columnHeightCurrent = height;
            return true;
        }
        public bool SetColumnRotation(double rotation) {
            this.columnRotationCurrent = rotation;
            return true;
        }

        public bool SetBeamType(string familyName, string typeName) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            if (this.beamTypes.ContainsKey(familyName)) {
                if (this.beamTypes[familyName].ContainsKey(typeName)) {
                    this.beamFamilyCurrent = familyName;
                    this.beamTypeCurrent = typeName;
                    return true;
                }
                else return false;
            }
            else return false;            
        }
        public bool SetBeamRotation(double rotation) {
            this.beamRotationCurrent = rotation;
            return true;
        }
        public bool SetBeamJustification(string justifcation) {
            this.beamJustificationCurrent = justifcation;
            return true;
        }

        public bool SetAdaptiveComponentType(string familyName, string typeName) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            if (this.familyTypes.ContainsKey(familyName)) {
                if (this.familyTypes[familyName].ContainsKey(typeName)) {
                    this.adaptiveComponentFamilyCurrent = familyName;
                    this.adaptiveComponentTypeCurrent = typeName;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public bool SetFamilyExtrusionHeight(double height) {
            this.familyExtrusionHeightCurrent = height;
            return true;
        }
        public bool SetMullionType(string familyName, string typeName) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            if (this.mullionTypes.ContainsKey(familyName)) {
                if (this.mullionTypes[familyName].ContainsKey(typeName)) {
                    this.mullionFamilyCurrent = familyName;
                    this.mullionTypeCurrent = typeName;
                    return true;
                }
                else return false;
            }
            else return false;
        }

        // **************** Workplane ******************************UIApplication uiApp = commandData.Application;

        // There is already a MakeSketchPlane set in the private functions.  What is left in the model if we make one?

    //  UIDocument uiDoc = uiApp.ActiveUIDocument;
    //  Transaction transaction = 
    //    new Transaction(uiDoc.Document, "WorkPlane");
    //  transaction.Start();
    //  Plane plane = 
    //    new Plane(
    //      uiDoc.Document.ActiveView.ViewDirection, 
    //      uiDoc.Document.ActiveView.Origin);
    //  SketchPlane sp = uiDoc.Document.Create.NewSketchPlane(plane);
    //  uiDoc.Document.ActiveView.SketchPlane = sp;
    //  uiDoc.Document.ActiveView.ShowActiveWorkPlane();
    //  transaction.Commit();
    //  return Result.Succeeded;
    //}  

        //public SketchPlane MakeSketchPlane() {

        //    SketchPlane sketchPlane = null;
        //    return sketchPlane;
        //}

        // **************** Annotation ************************
        public Grid MakeGrid(Curve curve) {   // line or arc
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            Grid grid = null;
            LocalErrorMessage = "";            
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making Grid");
                try {
                    // Deprecated 2016
                    //if (curve is Line) grid = this.settings.DocumentDb.Create.NewGrid((Line)curve);
                    //else if (curve is Arc) grid = this.settings.DocumentDb.Create.NewGrid((Arc)curve);
                    if (curve is Line) grid = Grid.Create(this.settings.DocumentDb, (Line)curve);
                    else if (curve is Arc) grid = Grid.Create(this.settings.DocumentDb, (Arc)curve);
                    else {
                        LocalErrorMessage = "MakeGrid called with invalid Curve argument";
                        return null;
                    }
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") grid = null;
                }
                return grid;
            }
        }

        public Level MakeLevel(double elevation, string name = null) {   
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            Level level = null;
            LocalErrorMessage = "";
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making Level");
                try {
                    // Deprecated 2016
                    //level = this.settings.DocumentDb.Create.NewLevel(elevation);
                    level = Level.Create(this.settings.DocumentDb, elevation);
                    if (name != null) level.Name = name;
                }
                catch (Exception exception) {  // Generally because name was not unique
                    LocalErrorMessage = exception.Message;
                    this.settings.DocumentDb.Delete(level.Id);
                    level = null;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") level = null;
                }
                return level;
            }
        }

        // **************** Detail Lines *********************
        public DetailLine MakeDetailLine(Line line) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            DetailLine detailLine = null;
            LocalErrorMessage = "";
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making DetailLine");
                try {                    
                    detailLine = (DetailLine)this.settings.DocumentDb.Create.NewDetailCurve(this.settings.ActiveView, line);
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") detailLine = null; 
                }
                return detailLine;
            }
        }
        public DetailArc MakeDetailArc(Arc arc) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            DetailArc detailArc = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making DetailArc");
                try {
                    detailArc = (DetailArc)this.settings.DocumentDb.Create.NewDetailCurve(this.settings.ActiveView, arc);
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") detailArc = null; 
                }
                return detailArc;
            }
        }
        public DetailEllipse MakeDetailEllipse(Ellipse ellipse) {            
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            DetailEllipse detailEllipse = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making DetailEllipse");
                try {
                    detailEllipse = (DetailEllipse)this.settings.DocumentDb.Create.NewDetailCurve(this.settings.ActiveView, ellipse);
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") detailEllipse = null; 
                }
                return detailEllipse;
            }
        }
        public DetailNurbSpline MakeDetailNurbSpline(NurbSpline nurbSpline) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            DetailNurbSpline detailNurbSpline = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making DetailNurbSpline");
                try {
                    detailNurbSpline = (DetailNurbSpline)this.settings.DocumentDb.Create.NewDetailCurve(this.settings.ActiveView, nurbSpline);
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") detailNurbSpline = null; 
                }
                return detailNurbSpline;
            }
        }

        // ************************** Symbolic Lines (same as detail lines but in family) **************
        public SymbolicCurve MakeSymbolicCurveLine(Line line) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                SymbolicCurve symbolicCurve = null;
                transaction.Start("Making SymbolicCurveLine");
                try {
                    symbolicCurve = (SymbolicCurve)this.settings.DocumentDb.FamilyCreate.NewSymbolicCurve(line, MakeSketchPlane(line.GetEndPoint(0), line.GetEndPoint(1)));
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") symbolicCurve = null; 
                }
                return symbolicCurve;
            }
        }
        public SymbolicCurve MakeSymbolicCurveArc(Arc arc) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            SymbolicCurve symbolicCurve = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making SymbolicCurveArc");
                try {
                    symbolicCurve = (SymbolicCurve)this.settings.DocumentDb.FamilyCreate.NewSymbolicCurve(arc, MakeSketchPlane(arc.GetEndPoint(0), arc.GetEndPoint(1), arc.Evaluate(0.5, true)));
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") symbolicCurve = null; 
                }
                return symbolicCurve;
            }
        }
        public SymbolicCurve MakeSymbolicCurveEllipse(Ellipse ellipse) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            SymbolicCurve symbolicCurve = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making symbolicCurveEllipse");
                try {
                    symbolicCurve = (SymbolicCurve)this.settings.DocumentDb.FamilyCreate.NewSymbolicCurve(ellipse, MakeSketchPlaneNormal(ellipse.Normal, ellipse.Center));
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") symbolicCurve = null; 
                }
                return symbolicCurve;
            }
        }
        public SymbolicCurve MakeSymbolicCurveNurbSpline(NurbSpline nurbSpline) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            SymbolicCurve symbolicCurve = null;            
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making SymbolicCurveNurbsSpline");
                try {
                    symbolicCurve = (SymbolicCurve)this.settings.DocumentDb.FamilyCreate.NewSymbolicCurve(nurbSpline, MakeSketchPlane(nurbSpline.CtrlPoints[0], nurbSpline.CtrlPoints[1]));
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") symbolicCurve = null; 
                }
                return symbolicCurve;
            }
        }

        // ****************** Model lines (both project and family) *******************
        public ModelLine MakeModelLine(Line line) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            ModelLine modelLine = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making ModelLine");
                try {
                    if (this.settings.DocumentDb.IsFamilyDocument) modelLine = (ModelLine)this.settings.DocumentDb.FamilyCreate.NewModelCurve(line, MakeSketchPlane(line.GetEndPoint(0), line.GetEndPoint(1)));
                    else modelLine = (ModelLine)this.settings.DocumentDb.Create.NewModelCurve(line, MakeSketchPlane(line.GetEndPoint(0), line.GetEndPoint(1)));
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") modelLine = null; 
                }
                return modelLine;
            }
        }
        public ModelArc MakeModelArc(Arc arc) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                ModelArc modelArc = null;
                transaction.Start("Making ModelArc");
                try {
                    if (this.settings.DocumentDb.IsFamilyDocument) {
                        modelArc = (ModelArc)this.settings.DocumentDb.FamilyCreate.NewModelCurve(arc, MakeSketchPlane(arc.GetEndPoint(0), arc.GetEndPoint(1), arc.Evaluate(0.5, true)));
                    }
                    else {
                        modelArc = (ModelArc)this.settings.DocumentDb.Create.NewModelCurve(arc, MakeSketchPlane(arc.GetEndPoint(0), arc.GetEndPoint(1), arc.Evaluate(0.5, true)));
                    }
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") modelArc = null; 
                }
                return modelArc;
            }
        }
        public ModelEllipse MakeModelEllipse(Ellipse ellipse) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            ModelEllipse modelEllipse = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making ModelEllipse");
                try {
                    if (this.settings.DocumentDb.IsFamilyDocument) {
                        modelEllipse = (ModelEllipse)this.settings.DocumentDb.FamilyCreate.NewModelCurve(ellipse, MakeSketchPlaneNormal(ellipse.Normal, ellipse.Center));
                    }
                    else {
                        modelEllipse = (ModelEllipse)this.settings.DocumentDb.Create.NewModelCurve(ellipse, MakeSketchPlaneNormal(ellipse.Normal, ellipse.Center));
                    }                    
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") modelEllipse = null; 
                }
                return modelEllipse;
            }
        }
        public ModelNurbSpline MakeModelNurbSpline(NurbSpline nurbSpline) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            ModelNurbSpline modelNurbSpline = null;            
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making ModelNurbSpline");
                try {
                    if (this.settings.DocumentDb.IsFamilyDocument) {
                        modelNurbSpline = (ModelNurbSpline)this.settings.DocumentDb.FamilyCreate.NewModelCurve(nurbSpline, MakeSketchPlane(nurbSpline.CtrlPoints[0], nurbSpline.CtrlPoints[1]));
                    }
                    else {
                        modelNurbSpline = (ModelNurbSpline)this.settings.DocumentDb.Create.NewModelCurve(nurbSpline, MakeSketchPlane(nurbSpline.CtrlPoints[0], nurbSpline.CtrlPoints[1]));
                    }
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") modelNurbSpline = null; 
                }
                return modelNurbSpline;
            }
        }

        // **************** Area Boundary Lines and Room Separation Lines **************************        
        // Model only
        public ModelCurve MakeAreaBoundaryLine(Curve curve) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            ModelCurve areaBoundaryLine = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making AreaBoundaryLine");
                try {
                    SketchPlane sketchPlane = MakeSketchPlane(curve.GetEndPoint(0), curve.GetEndPoint(1));
                    areaBoundaryLine = this.settings.DocumentDb.Create.NewAreaBoundaryLine(sketchPlane, curve, this.settings.ActiveViewPlan);
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") areaBoundaryLine = null;
                }
                return areaBoundaryLine;
            }
        }
        public ModelCurve MakeAreaBoundaryLine(CurveArrArray curveArrArray) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            ModelCurve areaBoundaryLine = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making AreaBoundaryLine");
                try {
                    if (curveArrArray.Size == 0) {
                        InnerErrorMessage = "Make AreaBoundaryLine called with empty CurveArrArray";
                    }
                    else {
                        CurveArray curveArray = curveArrArray.get_Item(0);
                        if (curveArray.Size == 0) {
                            InnerErrorMessage = "Make AreaBoundaryLine called with empty CurveArray";
                            Curve curve = curveArray.get_Item(0);
                            SketchPlane sketchPlane = MakeSketchPlane(curve.GetEndPoint(0), curve.GetEndPoint(1));
                            areaBoundaryLine = this.settings.DocumentDb.Create.NewAreaBoundaryLine(sketchPlane, curve, this.settings.ActiveViewPlan);
                        }
                    }
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") areaBoundaryLine = null;
                }
                return areaBoundaryLine;
            }
        }

        public ModelCurve MakeRoomSeparationLine(Curve curve) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            ModelCurve roomSeparationLine = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making RoomSeparationLine");
                try {
                    CurveArray curveArray = new CurveArray();
                    curveArray.Append(curve);
                    SketchPlane sketchPlane = MakeSketchPlane(curve.GetEndPoint(0), curve.GetEndPoint(1));
                    ModelCurveArray modelCurveArray = this.settings.DocumentDb.Create.NewRoomBoundaryLines(sketchPlane, curveArray, this.settings.ActiveView);
                    roomSeparationLine = modelCurveArray.get_Item(0);
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") roomSeparationLine = null;
                }
                return roomSeparationLine;
            }
        }
        public ModelCurve MakeRoomSeparationLine(CurveArrArray curveArrArray) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            ModelCurve roomSeparationLine = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making RoomSeparationLine");
                try {
                    if (curveArrArray.Size == 0) {
                        InnerErrorMessage = "Make RoomSeparationLine called with empty CurveArrArray";
                    }
                    else {
                        CurveArray curveArray = curveArrArray.get_Item(0);
                        if (curveArray.Size == 0) {
                            InnerErrorMessage = "Make RoomSeparationLine called with empty CurveArray";
                            Curve curve = curveArray.get_Item(0);
                            SketchPlane sketchPlane = MakeSketchPlane(curve.GetEndPoint(0), curve.GetEndPoint(1));
                            ModelCurveArray modelCurveArray = this.settings.DocumentDb.Create.NewRoomBoundaryLines(sketchPlane, curveArray, this.settings.ActiveView);
                            roomSeparationLine = modelCurveArray.get_Item(0);
                        }
                    }
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") roomSeparationLine = null;
                }
                return roomSeparationLine;
            }
        }

        // *************** Site Elements ************************************
        public TopographySurface MakeTopographySurface(List<XYZ> pointsList) { // General case can handle any number of points
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            //TranslateOrigin(ref pointsList);
            TopographySurface topographySurface = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);                
                transaction.Start("Making TopographySurface");
                try {
                    //topographySurface = this.settings.DocumentDb.Create.NewTopographySurface(pointsList); 
                    topographySurface = TopographySurface.Create(this.settings.DocumentDb, pointsList);           
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") topographySurface = null;  // For some reason rollback of transaction does not actually set back to null?
                }
            }
            return topographySurface;
        }

        // *************** Walls, Floors (project only) *********************
        public Wall MakeWall(Curve curve) {  // Assume line or arc in plan
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            //TranslateOrigin(ref point1, ref point2);
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                Wall wall = null;
                transaction.Start("Making Wall");
                try {
                    if (this.wallTypeCurrent == null) {
                        wall = Wall.Create(this.settings.DocumentDb, curve, this.levelCurrent.Id, false);
                        //Parameter parameterBaseOffset = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
                        //parameterBaseOffset.Set(curve.GetEndPoint(0).Z);
                        //Parameter parameterHeight = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);  // Unconnected Height of instance (not set for some reason?)
                        //parameterHeight.Set(this.wallHeightCurrent);
                    }
                    else {
                        wall = Wall.Create(this.settings.DocumentDb, curve, this.wallTypeCurrent.Id, this.levelCurrent.Id, this.wallHeightCurrent, curve.GetEndPoint(0).Z, false, false);
                    }
                    // 2013-09-24 moved this block since it logically need to be here.  Hoping that wasn't a mistake?
                    Parameter parameterBaseOffset = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
                    parameterBaseOffset.Set(curve.GetEndPoint(0).Z);
                    Parameter parameterHeight = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);  // Unconnected Height of instance (not set for some reason?)
                    parameterHeight.Set(this.wallHeightCurrent);
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") wall = null;  // For some reason rollback of transaction does not actually set the wall back to null?
                }
                return wall;
            }
        }
        public Wall MakeWall(CurveArrArray curveArrArray) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            //TranslateOrigin(ref curveArrArray);
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                CurveArray curveArray = curveArrArray.get_Item(0); // First array is outer boundary
                Wall wall = null;
                // Note the use of separate transaction in the following.  This is necessary due to a bug with Revit where it
                // gets confused if we try to insert the opening before the main shape is committed.  Using a subtransaction
                // didn't solve this.  Possible we coudl use one transaction for all the openings but not tested.                
                transaction.Start("Making Wall");
                try {
                    List<Curve> profile = new List<Curve>();
                    foreach (Curve curve in curveArray) {
                        profile.Add(curve);
                    }
                    if (this.floorTypeCurrent == null) {
                        wall = Wall.Create(this.settings.DocumentDb, profile, false);
                    }
                    else {
                        wall = Wall.Create(this.settings.DocumentDb, profile, this.wallTypeCurrent.Id, this.levelCurrent.Id, false);
                    }
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") wall = null; 
                }
                if (LocalErrorMessage != "") return null;
                // Apparently walls can only get rectangular openings???  Code below fins a rectangle based on min and max values
                for (int i = 1; i < curveArrArray.Size; i++) {  // Holes, if any.  ??? Islands in holes might be a problem ??????
                    CurveArray curveArrayOpening = curveArrArray.get_Item(i);
                    Double? xMin = null, yMin = null, zMin = null, xMax = null, yMax = null, zMax = null;
                    for (int j = 0; j < curveArrayOpening.Size; j++) {
                        Curve curve = curveArrayOpening.get_Item(j); //Assuming line or arc but could be more complex
                        FindMinMax(curve.GetEndPoint(0), ref xMin, ref yMin, ref zMin, ref xMax, ref yMax, ref zMax);
                        FindMinMax(curve.GetEndPoint(1), ref xMin, ref yMin, ref zMin, ref xMax, ref yMax, ref zMax);
                        if (!(curve is Line)) {
                            FindMinMax(curve.Evaluate(0.5, true), ref xMin, ref yMin, ref zMin, ref xMax, ref yMax, ref zMax);
                        }
                    }                        
                    XYZ pointStart = new XYZ(xMin.Value, yMin.Value, zMin.Value);
                    XYZ pointEnd = new XYZ(xMax.Value, yMax.Value, zMax.Value);                                               
                    //Note that these settings must be reset after any .Commit() or .RollBack().
                    if (TrapErrors) AddFailureHandler(transaction);
                    transaction.Start("Making Wall Opening");
                    try {
                        this.settings.DocumentDb.Create.NewOpening(wall, pointStart, pointEnd);
                    }
                    catch (Exception exception) {
                        LocalErrorMessage = exception.Message;
                    }
                    transaction.Commit();
                    if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                        InnerErrorMessage = this.failureHandler.ErrorMessage;
                        InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                        if (InnerErrorSeverity == "Error") wall = null; 
                        // Allow loop to continue; only last error will be reported.
                    }
                }
                return wall;
            }
        }

        public Floor MakeFloor(List<XYZ> points) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                Floor floor = null;
                transaction.Start("Making Floor");
                try {
                    if (points.Count > 2) {
                        CurveArray curveArray = new CurveArray();
                        Line line;
                        for (int i = 0; i < points.Count - 1; i++) {
                            line = Line.CreateBound(points[i], points[i+1]);
                            curveArray.Append(line);
                        }
                        line = Line.CreateBound(points[points.Count-1], points[0]);
                        curveArray.Append(line);
                        if (this.floorTypeCurrent == null) {
                            floor = this.settings.DocumentDb.Create.NewFloor(curveArray, false);
                        }
                        else {
                            floor = this.settings.DocumentDb.Create.NewFloor(curveArray, this.floorTypeCurrent, this.levelCurrent, false);
                        }
                    }
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") floor = null;
                }
                return floor;
            }
        }
        public Floor MakeFloor(CurveArrArray curveArrArray) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            //TranslateOrigin(ref curveArrArray);
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);                                     
                CurveArray curveArray = curveArrArray.get_Item(0); // First array is outer boundary
                Floor floor = null;
                // Note the use of separate transaction in the following.  This is necessary due to a bug with Revit where it
                // gets confused if we try to insert the opening before the main shape is committed.  Using a subtransaction
                // didn't solve this.  Possible we coudl use one transaction for all the openings but not tested.                
                transaction.Start("Making Floor");
                try {
                    if (this.floorTypeCurrent == null) {
                        floor = this.settings.DocumentDb.Create.NewFloor(curveArray, false);
                    }
                    else {
                        floor = this.settings.DocumentDb.Create.NewFloor(curveArray, this.floorTypeCurrent, this.levelCurrent, false);
                    }
                    transaction.Commit();
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") floor = null; 
                }
                if (LocalErrorMessage != "") return null;
                for (int i = 1; i < curveArrArray.Size; i++) {  // Holes, if any.  ??? Islands in holes might be a problem ??????
                    //Note that these settings must be reset after any .Commit() or .RollBack().
                    if (TrapErrors) AddFailureHandler(transaction);        
                    transaction.Start("Making Floor Opening");
                    try {
                        this.settings.DocumentDb.Create.NewOpening(floor, curveArrArray.get_Item(i), true);
                        transaction.Commit();
                        if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                            InnerErrorMessage = this.failureHandler.ErrorMessage;
                            InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                            if (InnerErrorSeverity == "Error") floor = null; 
                            // Allow loop to continue; only last error will be reported.
                        }
                    }
                    catch (Exception exception) {
                        LocalErrorMessage = exception.Message;
                        break;
                    }
                }
                return floor;               
            }
        }

        // ****************** Family Instance ***************************
        public FamilyInstance MakeFamilyInstance(XYZ point1) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            if (this.familyFamilyCurrent == "" || this.familyTypeCurrent == "" || this.familyFamilyCurrent == null || this.familyTypeCurrent == null) {
                LocalErrorMessage = "No Family Instance Family-Type specified";
                return null;
            }
            //TranslateOrigin(ref point1);
            FamilyInstance familyInstance = null;
            
            try {
                using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                    if (TrapErrors) AddFailureHandler(transaction);
                    // Make FamilyInstance
                    FamilySymbol familySymbol = null;
                    transaction.Start("Making FamilyInstance");
                    familySymbol = this.familyTypes[this.familyFamilyCurrent][this.familyTypeCurrent];
                    if (familySymbol == null) {
                        LocalErrorMessage = "Invalid Column Family-Type";
                        return null;
                    }
                    if (!familySymbol.IsActive) {
                        familySymbol.Activate();
                    }
                    if (this.settings.DocumentDb.IsFamilyDocument) {
                        familyInstance = this.settings.DocumentDb.FamilyCreate.NewFamilyInstance(point1, familySymbol, this.levelCurrent, StructuralType.NonStructural);
                    }
                    else {
                        familyInstance = this.settings.DocumentDb.Create.NewFamilyInstance(point1, familySymbol, this.levelCurrent, StructuralType.NonStructural);
                    }
                    // Flip Hand and/or Flip Facing
                    if (this. familyFlipHandCurrent) {
                        if (familyInstance.CanFlipHand) {
                            familyInstance.flipHand();
                        }
                    }
                    if (this.familyFlipFacingCurrent) {
                        if (familyInstance.CanFlipFacing) {
                            familyInstance.flipFacing();
                        }
                    }
// TODO Not dealing with hosted famlies.
                    transaction.Commit();            
                }
                // Note that the order of the transformations is: MirrorX, MirrorY, then Rotation.  The order does affect the outcome but, as long as there is a convention
                // there shouldn't be a problem.  This seems like the most logical order.
                // Mirror, if needed.  Note this function deletes the original and sets familyInstance to the new element.
                if (this.familyMirrorXCurrent) MirrorFamilyInstance(ref familyInstance, point1, "X");
                if (this.familyMirrorYCurrent) MirrorFamilyInstance(ref familyInstance, point1, "Y");
                // Rotate, if needed.
                if (this.familyRotationCurrent != 0) {
                    //Line lineRotation = this.settings.ApplicationAs.Create.NewLineBound(point1, new XYZ(point1.X, point1.Y, point1.Z + 100));
                    Line lineRotation = Line.CreateBound(point1, new XYZ(point1.X, point1.Y, point1.Z + 100));
                    double angleRadians = this.familyRotationCurrent * Math.PI / 180;
                    using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                        if (TrapErrors) AddFailureHandler(transaction);
                        transaction.Start("Rotating FamilyInstance");
                        ElementTransformUtils.RotateElement(this.settings.DocumentDb, familyInstance.Id, lineRotation, angleRadians);
                        transaction.Commit();
                    }
                    if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                        InnerErrorMessage = this.failureHandler.ErrorMessage;
                        InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                        if (InnerErrorSeverity == "Error") familyInstance = null;
                    }
                }

            }
            catch (Exception exception) {
                LocalErrorMessage = exception.Message;
                familyInstance = null;
            }
            if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                InnerErrorMessage = this.failureHandler.ErrorMessage;
                InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                if (InnerErrorSeverity == "Error") familyInstance = null; 
            }

            return familyInstance;    
            
        }

        // **************** Columns and Beams **************************

        public FamilyInstance MakeColumn(List<XYZ> points) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            if (this.columnFamilyCurrent == "" || this.columnTypeCurrent == "" || this.columnFamilyCurrent == null || this.columnTypeCurrent == null) {
                LocalErrorMessage = "No Column Family-Type specified";
                return null;
            }
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);                    
                FamilyInstance familyInstanceColumn = null;
                FamilySymbol familySymbol = null;
                transaction.Start("Making Column");
                try {
                    StructuralType structuralType = new StructuralType();
                    Line lineTwoPointBounded = null;
                    if (this.columnModeCurrent == "Architectural") {
                        familySymbol = this.columnArchitecturalTypes[this.columnFamilyCurrent][this.columnTypeCurrent];
                        if (familySymbol == null) {
                            LocalErrorMessage = "Invalid Column Family-Type";
                            return null;
                        }
                        structuralType = StructuralType.NonStructural;
                    }
                    else {
                        familySymbol = this.columnStructuralTypes[this.columnFamilyCurrent][this.columnTypeCurrent];
                        if (familySymbol == null) {
                            LocalErrorMessage = "Invalid Column Family-Type";
                            return null;
                        }
                        structuralType = StructuralType.Column;
                    }
                    if (!familySymbol.IsActive) {
                        familySymbol.Activate();
                    }
                    if (this.columnModeCurrent == "Architectural" || this.columnModeCurrent == "StructuralVertical") {
                        familyInstanceColumn = this.settings.DocumentDb.Create.NewFamilyInstance(points[0], familySymbol, this.levelCurrent, structuralType);
                        Parameter parameterBaseLevelOffset = familyInstanceColumn.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM);
                        parameterBaseLevelOffset.Set(points[0].Z);   // Using Z-value of point to set the position relative to current level.
                        //Parameter parameterTopLevel = familyInstanceColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);
                        Parameter parameterTopLevelOffset = familyInstanceColumn.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);
                        //parameterTopLevel.Set(this.levelCurrent.Id);
                        //parameterTopLevelOffset.Set(mColumnHeightCurrent);
                        parameterTopLevelOffset.Set(this.columnHeightCurrent + points[0].Z);  // Setting top to bottom offset plus height.
                    }
                    else {  // Structural angled cases; "StructuralPointDriven",  "StructuralAngleDriven";
                        // Always placed by end points; angle driven case only controls subsequent behavior.
                        if (points.Count < 2) return null;
                        lineTwoPointBounded = Line.CreateBound(points[0], points[1]);  // bounded required for both creation and rotation
                        familyInstanceColumn = this.settings.DocumentDb.Create.NewFamilyInstance(lineTwoPointBounded, familySymbol, this.levelCurrent, structuralType);

                        if (this.columnModeCurrent == "StructuralAngleDriven") {  // Default is SlantedOrVerticalColumnType.CT_EndPoint
                            Parameter parameterColumnSlantType = familyInstanceColumn.get_Parameter(BuiltInParameter.SLANTED_COLUMN_TYPE_PARAM);
                            parameterColumnSlantType.Set(Convert.ToInt32(SlantedOrVerticalColumnType.CT_Angle));
                        }
                    }
                    if (this.columnRotationCurrent != 0) {  // Note that rotation is about centerline of angled cases
                        Line lineRotation;
                        if (lineTwoPointBounded != null) lineRotation = lineTwoPointBounded;
                        else {
                            XYZ pointVertical = new XYZ(points[0].X, points[0].Y, points[0].Z + 100);
                            lineRotation = Line.CreateBound(points[0], pointVertical);
                        }
                        double angleRadians = this.columnRotationCurrent * Math.PI / 180;
                        ElementTransformUtils.RotateElement(this.settings.DocumentDb, familyInstanceColumn.Id, lineRotation, angleRadians);
                    }
                    transaction.Commit();
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") familyInstanceColumn = null; 
                }                
                return familyInstanceColumn;
            }
        }

        public FamilyInstance MakeBeam(List<XYZ> points) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            FamilyInstance familyInstanceBeam = null;
            FamilySymbol familySymbol = null;
            if (this.beamFamilyCurrent == "" || this.beamTypeCurrent == "" || this.beamFamilyCurrent == null || this.beamTypeCurrent == null) {
                LocalErrorMessage = "No Beam Family-Type specified";
                return null;
            }
            if (points.Count < 2) return null;
            Line lineBound = Line.CreateBound(points[0], points[1]);  // bounded required for both creation and rotation; 
            familySymbol = this.beamTypes[this.beamFamilyCurrent][this.beamTypeCurrent];
            if (familySymbol == null) {
                LocalErrorMessage = "Invalid Beam Family-Type";
                return null;
            }
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                familySymbol = this.beamTypes[this.beamFamilyCurrent][this.beamTypeCurrent];
                if (familySymbol == null) {
                    LocalErrorMessage = "Invalid Beam Family-Type";
                    return null;
                }
                transaction.Start("Making Beam");
                if (!familySymbol.IsActive) {
                    familySymbol.Activate();                
                }
                try {
                    familyInstanceBeam = this.settings.DocumentDb.Create.NewFamilyInstance(lineBound, familySymbol, this.levelCurrent, StructuralType.Beam);
                    // No longer using enum with justification.  Not sure we had the right one; integer values work better.
                    if (this.beamJustificationCurrent != "Top") {  // Top = value 0; default value
                        Parameter parameterJustification = familyInstanceBeam.get_Parameter(BuiltInParameter.BEAM_V_JUSTIFICATION);  // Z Justification
                        if (this.beamJustificationCurrent != "Center") parameterJustification.Set(1);
                        else {
                            if (this.beamJustificationCurrent != "Bottom") parameterJustification.Set(2);
                            else {
                                if (this.beamJustificationCurrent != "Other") parameterJustification.Set(3);
                            }
                        }                        
                    }
                    if (this.beamRotationCurrent != 0) {  // Note that rotation is about centerline of angled cases                   
                        Parameter parameterRotation = familyInstanceBeam.get_Parameter(BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE);
                        double angleRadians = this.beamRotationCurrent * Math.PI / 180;
                        parameterRotation.Set(angleRadians);
                        // ** This code does not work although it does not give an error.  Splitting the transaction didn't help.
                        // ** TODO Explore with ADN and R2013.               
                        //ElementTransformUtils.RotateElement(this.settings.DocumentDb, familyInstanceBeam.Id, lineBound, angleRadians);                
                    }
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") familyInstanceBeam = null; 
                }  
            }
            return familyInstanceBeam;
        }

        // **************** Adaptive Component **************************
        public FamilyInstance MakeAdaptiveComponent(List<XYZ> points) {  // General case can handle any number of points
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            if (points.Count == 0) {
                LocalErrorMessage = "No points in list";
                return null;
            }
            if (this.adaptiveComponentFamilyCurrent == "" || this.adaptiveComponentTypeCurrent == "" || this.adaptiveComponentFamilyCurrent == null || this.adaptiveComponentTypeCurrent == null) {
                LocalErrorMessage = "No Adaptive Component Family-Type specified";
                return null;
            }
            //// *********** Study *************
            //// We need the order of the adaptive points but we don't want to do it every time so save the values in a dictionary. 
            //int[] adaptivePointOrder = null;
            //Family family = FamilyTypes[mAdaptiveComponentFamilyCurrent][mAdaptiveComponentTypeCurrent].Family;
            //if (AdaptivePointOrders.ContainsKey(family)) adaptivePointOrder = AdaptivePointOrders[family];
            //else {
            //    adaptivePointOrder = GetAdaptivePointOrder(family);
            //    AdaptivePointOrders.Add(family, adaptivePointOrder);
            //}

            // ************************
            FamilyInstance familyInstanceAdaptComp = null;
            
            //TranslateOrigin(ref points);
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                // Find a center point. The actual point doesn't matter since we are going to adjust the corner points in the following.
                // However, using a fixed point like 0,0,0 raises errors of duplicate families at the same point. By picking the actual center
                // this way it seems like we are leaving the user with some logical point in case they see it in later edits.
                double centerX = 0; double centerY = 0; double centerZ = 0;
                for (int index = 0; index < points.Count; index++) {
                    centerX = centerX + points[index].X;
                    centerY = centerY + points[index].Y;
                    centerZ = centerZ + points[index].Z;
                }
                centerX = centerX / points.Count;
                centerY = centerY / points.Count;
                centerZ = centerZ / points.Count;
                XYZ pointInsert = new XYZ(centerX, centerY, centerZ);                
                transaction.Start("Making Adaptive Component");                
                try {
                    Document documentDb = this.settings.DocumentDb;
                    FamilySymbol familySymbol = FamilyTypes[this.adaptiveComponentFamilyCurrent][this.adaptiveComponentTypeCurrent];
                    
                    if (familySymbol == null) {
                        LocalErrorMessage = "Invalid Adaptive Component Family-Type";
                        return null;
                    }
                    if (!familySymbol.IsActive) {
                        familySymbol.Activate();
                    }
                    if (this.settings.DocumentDb.IsFamilyDocument) {
                        familyInstanceAdaptComp = this.settings.DocumentDb.FamilyCreate.NewFamilyInstance(pointInsert, familySymbol, this.levelCurrent, StructuralType.NonStructural);
                    }
                    else {
                        familyInstanceAdaptComp = this.settings.DocumentDb.Create.NewFamilyInstance(pointInsert, familySymbol, this.levelCurrent, StructuralType.NonStructural);
                    }
                    // Note that we need to commit the transaction before we get the placement points
                    // With content upgraded fro 2011 this was not necessary.
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") familyInstanceAdaptComp = null; 
                }
            }
            if (LocalErrorMessage != "") return null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Fitting Adaptive Component");
                try {
                    //IList<FamilyPointPlacementReference> familyPoints = familyInstanceAdaptComp.GetFamilyPointPlacementReferences();
                    //IList<ElementId> familyPointIds = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(familysymbol)
                    IList<ElementId> familyPointIds = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(familyInstanceAdaptComp);
                    if (familyPointIds.Count() == points.Count) {
                        for (int index = 0; index < points.Count; index++) {
                            ReferencePoint referencePoint = (ReferencePoint)this.settings.DocumentDb.GetElement(familyPointIds[index]);
                            XYZ translation = points[index].Subtract(referencePoint.Position);
                            ElementTransformUtils.MoveElement(this.settings.DocumentDb, familyPointIds[index], translation);

                            //XYZ origin = orderedPoints[index].Location.Origin;
                            //XYZ point = points[index];
                            //XYZ transform = new XYZ(point.X - origin.X, point.Y - origin.Y, point.Z - origin.Z);
                            //ElementTransformUtils.MoveElement(this.settings.DocumentDb, orderedPoints[index].PointReference.ElementId, transform);
                        }
                    }
                    else {
                        LocalErrorMessage = "Adaptive points found: " + familyPointIds.Count.ToString() + " Does not match expected number";
                    }
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") familyInstanceAdaptComp = null; 
                }
            }
            return familyInstanceAdaptComp;           
        }

        // **************** Areas and Rooms **************************
        public Area MakeArea(XYZ point1) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            Area area = null;
            //TranslateOrigin(ref point1);          
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making Area");
                try {
                    area = this.settings.DocumentDb.Create.NewArea(this.settings.ActiveViewPlan, new UV(point1.X, point1.Y));
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") area = null; 
                }
            }            
            return area;
        }

        public Room MakeRoom(XYZ point1) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            Room room = null;
            //TranslateOrigin(ref point1);           
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making Room");
                try {
                    room = this.settings.DocumentDb.Create.NewRoom(this.levelCurrent, new UV(point1.X, point1.Y));
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") room = null; 
                }
            }             
            return room;
        }

        // **************** Regions **************************
        public FilledRegion MakeFilledRegion(List<XYZ> points) {
            FilledRegion filledRegion = null;
            if (points.Count > 2) {
                CurveLoop curveLoop = new CurveLoop();
                Line line;
                for (int i = 0; i < points.Count - 1; i++) {
                    line = Line.CreateBound(points[i], points[i + 1]);
                    curveLoop.Append(line);
                }
                line = Line.CreateBound(points[points.Count - 1], points[0]);
                curveLoop.Append(line);
                List<CurveLoop> curveLoops = new List<CurveLoop> { curveLoop };
                filledRegion = MakeFilledRegionCommon(curveLoops);
            }
            return filledRegion;
        }
        public FilledRegion MakeFilledRegion(CurveArrArray curveArrArray) {
            FilledRegion filledRegion = null;
            List<CurveLoop> curveLoops = new List<CurveLoop>();
            foreach (CurveArray curveArray in curveArrArray) {
                CurveLoop curveLoop = new CurveLoop();
                foreach (Curve curve in curveArray) {
                    curveLoop.Append(curve);
                }
                curveLoops.Add(curveLoop);
            }
            filledRegion = MakeFilledRegionCommon(curveLoops);
            return filledRegion;
        }
        public FilledRegion MakeFilledRegionCommon(List<CurveLoop> curveLoops) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                FilledRegion filledRegion = null;
                transaction.Start("Making FilledRegion");
                try {                    
                    if (mFilledRegionTypeCurrent == null) {  // As a workaround just take the first one.
                        if (this.filledRegionTypes.Count > 0) mFilledRegionTypeCurrent = this.filledRegionTypes.FirstOrDefault().Value;
                        else {  // May not ever happen that there are no types at all?
                            LocalErrorMessage = "No FilledRegion types found.";  //TODO this message not getting reported to user
                        }
                    }
                    if (mFilledRegionTypeCurrent != null) {
                        filledRegion = FilledRegion.Create(this.settings.DocumentDb, mFilledRegionTypeCurrent.Id, this.settings.ActiveView.Id, curveLoops);
                    }
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") filledRegion = null;
                }
                return filledRegion;
            }
        }

 
        // *************** Reference Planes ********************************
        //    XYZ bubbleEnd, freeEnd, cutVec, thirdPoint;
        //    ReferencePlane referencePlaneF = this.settings.DocumentDb.FamilyCreate.NewReferencePlane(bubbleEnd, freeEnd, cutVec, this.settings.ActiveView);
        //    ReferencePlane referencePlaneM = this.settings.DocumentDb.Create.NewReferencePlane(bubbleEnd, freeEnd, cutVec, this.settings.ActiveView);
        //    ReferencePlane referencePlane2F = this.settings.DocumentDb.FamilyCreate.NewReferencePlane2(bubbleEnd, freeEnd, thirdPoint, this.settings.ActiveView);
        //    ReferencePlane referencePlane2M = this.settings.DocumentDb.Create.NewReferencePlane2(bubbleEnd, freeEnd, thirdPoint, this.settings.ActiveView);

        // *************** Reference Points, CurveByPoints, and LoftForm ********************************
        // Note: A slightly different patern is used with these elements; the HbItem is preserved all of the way through the function call.  Also, the functions do not
        //       use eachother to avoid repeating the same code.   Both of these characteristics are due to the desire to avoid adding new transactions in Revit.
        public ReferencePoint MakeReferencePoint(HbReferencePoint hbReferencePoint) {  // Only available in family
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            ReferencePoint referencePoint = null;
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making ReferencePoint");
                if (this.settings.DocumentDb.IsFamilyDocument) {
                    try {
                        XYZ xyz = new XYZ(hbReferencePoint.X, hbReferencePoint.Y, hbReferencePoint.Z);
                        referencePoint = (ReferencePoint)this.settings.DocumentDb.FamilyCreate.NewReferencePoint(xyz);
                    }
                    catch (Exception exception) {
                        LocalErrorMessage = exception.Message;
                    }
                }
                else {
                    LocalErrorMessage = "ReferencePoint can only be created in Family Editor";
                    return null;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") referencePoint = null; 
                }
                return referencePoint;
            }
        }
        public CurveByPoints MakeCurveByPoints(HbReferenceArray hbReferenceArray) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            CurveByPoints curveByPoints = null;          
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making CurveByPoints");
                if (this.settings.DocumentDb.IsFamilyDocument) {
                    try {
                        ReferencePointArray referencePointArray = new ReferencePointArray();
                        foreach (HbReferencePoint hbReferencePoint in hbReferenceArray) {
                            XYZ xyz = new XYZ(hbReferencePoint.X, hbReferencePoint.Y, hbReferencePoint.Z);
                            ReferencePoint referencePoint = (ReferencePoint)this.settings.DocumentDb.FamilyCreate.NewReferencePoint(xyz);
                            referencePointArray.Append(referencePoint);
                        }
                        curveByPoints = (CurveByPoints)this.settings.DocumentDb.FamilyCreate.NewCurveByPoints(referencePointArray);
                    }
                    catch (Exception exception) {
                        LocalErrorMessage = exception.Message;
                    }                    
                }
                else {
                    LocalErrorMessage = "CurveByPoints can only be created in Family Editor";
                    return null;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") curveByPoints = null; 
                }
                return curveByPoints;
            }
        }
        public Form MakeLoftForm(HbReferenceArrayArray hbReferenceArrayArray) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            Form loftForm = null;          
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making LoftForm");
                if (this.settings.DocumentDb.IsFamilyDocument) {
                    try {
                        ReferenceArrayArray referenceArrayArray = new ReferenceArrayArray();                               
                        foreach (HbReferenceArray hbReferenceArray in hbReferenceArrayArray) {
                            ReferencePointArray referencePointArray = new ReferencePointArray();                                          
                            foreach (HbReferencePoint hbReferencePoint in hbReferenceArray) {
                                XYZ xyz = new XYZ(hbReferencePoint.X, hbReferencePoint.Y, hbReferencePoint.Z);
                                ReferencePoint referencePoint = (ReferencePoint)this.settings.DocumentDb.FamilyCreate.NewReferencePoint(xyz);
                                referencePointArray.Append(referencePoint);                                                                                             
                            }
                            CurveByPoints curveByPoints = (CurveByPoints)this.settings.DocumentDb.FamilyCreate.NewCurveByPoints(referencePointArray);
                            if (curveByPoints != null) {
                                ReferenceArray referenceArray = new ReferenceArray();
                                referenceArray.Append(curveByPoints.GeometryCurve.Reference);                            
                                referenceArrayArray.Append(referenceArray);
                            }
                        }
                        loftForm = (Form)this.settings.DocumentDb.FamilyCreate.NewLoftForm(true, referenceArrayArray);
                    }
                    catch (Exception exception) {
                        LocalErrorMessage = exception.Message;
                    }
                }
                else {
                    LocalErrorMessage = "LoftForm can only be created in Family Editor";
                    return null;
                }
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") loftForm = null; 
                }
                return loftForm;
            }
        }


        // *************** New Families; based on extrusion ********************************

        public FamilyInstance MakeFamilyExtrusion(CurveArrArray curveArrArray, string familyName, string pathTemplate, XYZ insertionPoint = null) {
// TODO Evaluate effect of Ellipse and HermiteSpline in CurveArrArray
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";

            if (insertionPoint == null) insertionPoint = new XYZ(0, 0, 0);

            if (this.settings.TempFilePath == "") {
                LocalErrorMessage = "Unable to save family because 'TempFilePath' is blank.";
                return null;
            }

            if (Math.Abs(this.familyExtrusionHeightCurrent) < 0.01) {    // Not certain what actual limit is but this worked in testing and very small values didn't.
                LocalErrorMessage = "Cannot create geometry due to zero or very small extrusion height.";
                return null;
            }

            //CurveArray curveArray = curveArrArray.get_Item(0); // First array is outer boundary
            FamilyInstance familyInstance = null;               

            CurveArray curveArraySample = curveArrArray.get_Item(0);
            Curve curveSample = curveArraySample.get_Item(0);
            double elevation = curveSample.GetEndPoint(0).Z;  // Use Z value of first point
            Document familyDocument = this.settings.ApplicationAs.NewFamilyDocument(pathTemplate);                
            using (Transaction transaction = new Transaction(familyDocument)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Making Family");
                try {
                    Extrusion extrusion = null;
                    Form formMain = null;
                    //Plane plane = this.settings.ApplicationAs.Create.NewPlane(new XYZ(0, 0, 1), new XYZ(0, 0, elevation));
                    Plane plane = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), new XYZ(0, 0, elevation));
                    SketchPlane sketchPlane = SketchPlane.Create(familyDocument, plane);
                    try {
                        // Note:  Using the exception to test if the family is based on a mass family template or not.  Kind of ugly but don't know if a better way to determine.
                        extrusion = familyDocument.FamilyCreate.NewExtrusion(true, curveArrArray, sketchPlane, this.familyExtrusionHeightCurrent);
                    }
                    catch {
                        int innerListIndex = 0;
                        foreach (CurveArray innerList in curveArrArray) {
                            ReferenceArray referenceArray = new ReferenceArray();
                            foreach (Curve curve in innerList) {
                                ModelCurve modelCurve = null;
                                switch (curve.GetType().Name) {
                                    case "Line":
                                        Line line = (Line)curve;
                                        modelCurve = familyDocument.FamilyCreate.NewModelCurve(line, sketchPlane);
                                        break;
                                    case "Arc":
                                        Arc arc = (Arc)curve;
                                        modelCurve = familyDocument.FamilyCreate.NewModelCurve(arc, sketchPlane);
                                        break;
                                    case "HermiteSpline":
                                        HermiteSpline hermiteSpline = (HermiteSpline)curve;
                                        modelCurve = familyDocument.FamilyCreate.NewModelCurve(hermiteSpline, sketchPlane);
                                        break;
                                    case "NurbSpline":
                                        NurbSpline nurbSpline = (NurbSpline)curve;
                                        modelCurve = familyDocument.FamilyCreate.NewModelCurve(nurbSpline, sketchPlane);
                                        break;
                                    case "Ellipse":
                                        Ellipse ellipse = (Ellipse)curve;
                                        modelCurve = familyDocument.FamilyCreate.NewModelCurve(ellipse, sketchPlane);
                                        break;
                                    default:
                                        // This is an unknown case.
                                        break;
                                }
                                referenceArray.Append(modelCurve.GeometryCurve.Reference);
                            }
                            if (innerListIndex == 0) {
                                formMain = familyDocument.FamilyCreate.NewExtrusionForm(true, referenceArray, new XYZ(0, 0, this.familyExtrusionHeightCurrent));
                            }
                            else {
                                Form formVoid = familyDocument.FamilyCreate.NewExtrusionForm(false, referenceArray, new XYZ(0, 0, this.familyExtrusionHeightCurrent));
                            }
                            innerListIndex++;
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") familyDocument = null;
                }
            } // using

            // If we have failed to make a good family then quit at this point
            if (familyDocument == null) return null;

            //Try to save family as a file.  This is necessary due to know problems with API that makes
            string filePathTemp;
            try {
                filePathTemp = this.settings.TempFilePath + @"\" + familyName + ".rfa";
                SaveAsOptions saveAsOptions = new SaveAsOptions();
                saveAsOptions.OverwriteExistingFile = true;
                familyDocument.SaveAs(filePathTemp, saveAsOptions);
                familyDocument.Close(false);
            }
            catch (Exception exception) {
                LocalErrorMessage = exception.Message;
                return null;
            }

            //Make instance
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Placing FamilyExtrusion");
                Family family;
                try {
                    this.documentDb.LoadFamily(filePathTemp, out family);
                    if (family == null) {
                        System.Windows.Forms.MessageBox.Show("Null family value.  Halting process.", this.settings.ProgramName);
                        return null;
                    }
                    ISet<ElementId> familySymbols = family.GetFamilySymbolIds();
                    FamilySymbol familySymbol = (FamilySymbol)this.documentDb.GetElement(familySymbols.First());
                    if (!familySymbol.IsActive) {
                        familySymbol.Activate();
                        this.settings.DocumentDb.Regenerate();
                    }
                    familyInstance = this.documentDb.Create.NewFamilyInstance(insertionPoint, familySymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    if (File.Exists(filePathTemp)) File.Delete(filePathTemp);
                    if (familyInstance == null) {
                        System.Windows.Forms.MessageBox.Show("Unable to create family instance: \n" + filePathTemp + ".\nStopping process.", this.settings.ProgramName);
                        return null;
                    }
                    transaction.Commit();
                }
                catch (Exception exception) {
                    LocalErrorMessage = exception.Message;
                }
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") familyInstance = null;
                }
                return familyInstance;
            }
        }


        // **************** Parameters **************************
//TODO we are handling error messages differently with this case.  Maybe change back 

        public void ParameterSet(Element element, string parameterName, string stringValue, double unitsFactor) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            if (element == null) {
                LocalErrorMessage = "Element was null";
                return;
            }
            if (parameterName == null || parameterName == "") {
                LocalErrorMessage = "Missing 'Parameter Name' value";
                return;
            }
            Autodesk.Revit.DB.Parameter parameter = element.LookupParameter(parameterName);
            if (parameter == null) {
                LocalErrorMessage = "Parameter not bound to element";
                return;
            }
            if (parameter.IsReadOnly) {
                LocalErrorMessage = "Parameter is read-only";
                return;
            }

            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Modifying Parameter Value");
                try {
                    if (parameter.StorageType == StorageType.String) parameter.Set(stringValue);
                    else {
                        if (parameter.StorageType == StorageType.Integer) parameter.Set(Convert.ToInt32(stringValue));
                        else {
                            if (parameter.StorageType == StorageType.Double) {
                                if (parameter.Definition.UnitType == UnitType.UT_Area) {
                                    parameter.Set(Convert.ToDouble(stringValue) * unitsFactor * unitsFactor);
                                }
                                else if (parameter.Definition.UnitType == UnitType.UT_Length) {
                                    parameter.Set(Convert.ToDouble(stringValue) * unitsFactor);
                                }
                                else parameter.Set(Convert.ToDouble(stringValue));
                            }
                            else {
                                if (parameter.StorageType == StorageType.ElementId) parameter.Set(new ElementId(Convert.ToInt32(stringValue)));
                                else {
                                    LocalErrorMessage = "Unable to set unhandled parameter type";  // Catchall may never occur
                                }
                            }
                        }
                    }
                    transaction.Commit();
                    if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                        InnerErrorMessage = this.failureHandler.ErrorMessage;
                        InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                        LocalErrorMessage = "Internal error";
                    }
                    return;
                }
                catch {
                    LocalErrorMessage = "Unhandled exception while attempting to set parameter";
                    return;
                }
            }
        }

        // ********************************************************** Mullions ***************************************************

        public void CurtainGridUAdd(Element element, double doubleValue0, double doubleValue1) {
            GridMullionAdd(element, doubleValue0, doubleValue1, true, false);
        }
        public void CurtainGridVAdd(Element element, double doubleValue0, double doubleValue1) {
            GridMullionAdd(element, doubleValue0, doubleValue1, false, false);
        }
        public void MullionUAdd(Element element, double doubleValue0, double doubleValue1) {
            GridMullionAdd(element, doubleValue0, doubleValue1, true, true);
        }
        public void MullionVAdd(Element element, double doubleValue0, double doubleValue1) {
            GridMullionAdd(element, doubleValue0, doubleValue1, false, true);
        }
        private void GridMullionAdd(Element element, double doubleValue0, double doubleValue1, bool isGridU, bool includeMullion) {
            InnerErrorMessage = "";
            InnerErrorSeverity = "";
            LocalErrorMessage = "";
            if (element == null) {
                LocalErrorMessage = "Element was null";
                return;
            }

            try {
                Wall wall = (Wall)element;
                if (wall == null) {
                    LocalErrorMessage = "Element was not a wall";
                    return;
                }

                MullionType mullionType = null;
                if (includeMullion) {
                    if (this.mullionFamilyCurrent == "" || this.mullionTypeCurrent == "" || this.mullionFamilyCurrent == null || this.mullionTypeCurrent == null) {
                        LocalErrorMessage = "No Mullion Family-Type specified";
                        return;
                    }
                    if (this.mullionTypes.ContainsKey(this.mullionFamilyCurrent)) {
                        if (this.mullionTypes[this.mullionFamilyCurrent].ContainsKey(this.mullionTypeCurrent)) mullionType = this.mullionTypes[this.mullionFamilyCurrent][this.mullionTypeCurrent];
                    }
                    if (mullionType == null) {
                        LocalErrorMessage = "Invalid Mullion Family-Type";
                        return;
                    }
                }

                double? primaryOffsetInput = doubleValue0;
                if (primaryOffsetInput == null) {
                    LocalErrorMessage = "Null 'Primary Offset'value";
                    return;
                }
                double primaryOffset = primaryOffsetInput.Value;
                if (primaryOffset <= 0 || primaryOffset >= 1) {
                    LocalErrorMessage = "'Primary Offset'value not between 0 and 1";
                    return;
                }
                bool oneSegmentOnly = true;  //oneSegmentOnly = false; means draw the whole grid without breaks at perpendicular mullions
                double secondaryOffset;
                // Note: A blank value for secondary offset seems to be read as 0.0 which we will interpret as intending oneSegmentOnly = false;
                if (doubleValue1 == 0.0) {
                    oneSegmentOnly = false;
                } else {
                    if (doubleValue1 < 0 || doubleValue1 >= 1) {
                        LocalErrorMessage = "'Secondary Offset'value not between 0 and 1";
                        return;
                    }
                }
                secondaryOffset = doubleValue1;

                CurtainGrid curtainGrid;
                if (wall.CurtainGrid == null) {
                    LocalErrorMessage = "Wall type does not support use of curtain grid";
                    return;
                } else curtainGrid = wall.CurtainGrid;

                XYZ xyzInsertionPoint;
                List<ElementId> elementIdsLines;
                if (!FindWallPoint(wall, isGridU, primaryOffset, secondaryOffset, out xyzInsertionPoint)) {
                    LocalErrorMessage = "Error calculating insertion point from offsets";
                    return;
                }
                if (isGridU) elementIdsLines = (List<ElementId>)curtainGrid.GetUGridLineIds();
                else elementIdsLines = (List<ElementId>)curtainGrid.GetVGridLineIds();

                CurtainGridLine curtainGridLine = null;
                Curve curtainGridFullCurve = null;
                Curve curtainGridSegment = null;
                string transactionName;
                if (includeMullion) transactionName = "Add Grid and Mullion";
                else transactionName = "Add Grid";
                using (Transaction transaction = new Transaction(this.documentDb)) {
                    if (TrapErrors) AddFailureHandler(transaction);
                    try {
                        transaction.Start(transactionName);

                        // Find or add the full grid line
                        bool curtainGridLineFound = false;
                        CurveArray existingSegments = null;
                        CurveArray skippedSegments = null;
                        foreach (ElementId elementIdLine in elementIdsLines) {
                            curtainGridLine = (CurtainGridLine)this.documentDb.GetElement(elementIdLine);
                            curtainGridFullCurve = curtainGridLine.FullCurve;
                            XYZ startPoint = curtainGridFullCurve.GetEndPoint(0);
                            if (isGridU) {
                                if (Math.Abs(startPoint.Z - xyzInsertionPoint.Z) < 0.0000001) curtainGridLineFound = true;
                            } else {
                                if ((Math.Abs(startPoint.X - xyzInsertionPoint.X) < 0.0000001) &&
                                    (Math.Abs(startPoint.Y - xyzInsertionPoint.Y) < 0.0000001)) curtainGridLineFound = true;
                            }
                            if (curtainGridLineFound) break;
                        }
                        if (!curtainGridLineFound) {
                            curtainGridLine = curtainGrid.AddGridLine(isGridU, xyzInsertionPoint, oneSegmentOnly);
                            curtainGridFullCurve = curtainGridLine.FullCurve;
                        }

                        // Find or add the segment
                        existingSegments = curtainGridLine.ExistingSegmentCurves;
                        skippedSegments = curtainGridLine.SkippedSegmentCurves;
                        bool segmentFound = false;
                        if (oneSegmentOnly) {
                            if (existingSegments != null) {
                                foreach (Curve segment in existingSegments) {
                                    if (Math.Abs(segment.Distance(xyzInsertionPoint)) < 0.0000001) {
                                        curtainGridSegment = segment;
                                        segmentFound = true;
                                        break;
                                    }
                                }
                            }
                            if (!segmentFound) {
                                if (skippedSegments != null) {
                                    foreach (Curve segment in skippedSegments) {
                                        if (Math.Abs(segment.Distance(xyzInsertionPoint)) < 0.0000001) {
                                            curtainGridLine.AddSegment(segment);
                                            curtainGridSegment = segment;
                                            segmentFound = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        } else {
                            curtainGridLine.AddAllSegments();
                        }

                        // Add the mullion(s)
                        if (includeMullion) {
                            if (oneSegmentOnly && segmentFound) curtainGridLine.AddMullions(curtainGridSegment, mullionType, true);
                            else curtainGridLine.AddMullions(curtainGridFullCurve, mullionType, false);
                        }

                        transaction.Commit();
                        if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                            InnerErrorMessage = this.failureHandler.ErrorMessage;
                            InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                            LocalErrorMessage = "Internal error";
                        }
                    } catch (Exception exception) {
                        LocalErrorMessage = exception.Message;
                        return;
                    }
                }
                return;
            } catch (Exception exception) {
                LocalErrorMessage = exception.Message;
                return;
            }
        }
        private bool FindWallPoint(Wall wall, bool isGridU, double primaryOffset, double secondaryOffset, out XYZ xyzPoint) {
            // Note: Not sure of historical reason for using a bounding box here.  Might be because wall height parameter might not be correct
            // depending on how wall is actually constructed?  Leaving this way for now.
            BoundingBoxXYZ boundingBoxXYZ = wall.get_BoundingBox(this.settings.ActiveView);
            double baseWall = boundingBoxXYZ.Min.Z;
            double heightWall = boundingBoxXYZ.Max.Z - boundingBoxXYZ.Min.Z;
            LocationCurve locationCurve = (LocationCurve)wall.Location;
            Curve curve = locationCurve.Curve;
            try {
                XYZ xyzPointU;
                if (isGridU) {
                    xyzPointU = curve.Evaluate(secondaryOffset, true);
                    xyzPoint = new XYZ(xyzPointU.X, xyzPointU.Y, baseWall + primaryOffset * heightWall);
                } else {
                    xyzPointU = curve.Evaluate(primaryOffset, true);
                    xyzPoint = new XYZ(xyzPointU.X, xyzPointU.Y, baseWall + secondaryOffset * heightWall);
                }
                return true;
            } catch {
                xyzPoint = null;
                return false;
            }
        }


        // ******************************************** Public Functions used in Output CurveArray set ******************************** 

        public double GetFamilyTop(FamilyInstance familyInstance) {
            Double heightLevelTop = 0;
            Parameter parameterToplevel = familyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);
            if (parameterToplevel.AsElementId() != null) { //top  constrained
                Level levelTop = (Level)this.documentDb.GetElement(parameterToplevel.AsElementId());
                heightLevelTop = levelTop.Elevation;
            }
            Parameter parameterTopOffset = familyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);
            Double offsetTop = parameterTopOffset.AsDouble();
            if (this.settings.CurrentLevel == null) return heightLevelTop + offsetTop;
            else return heightLevelTop + offsetTop - this.settings.CurrentLevel.Elevation;
        }
        public double GetWallHeight(Wall wall) {
            Parameter parameterWallHeightType = wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE);
            if (parameterWallHeightType != null) {
                Element element = this.documentDb.GetElement(parameterWallHeightType.AsElementId());
                if (element != null) {
                    Level levelTop = (Level)element;
                    Parameter parameterTopOffset = wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET);
                    if (this.settings.CurrentLevel == null) return levelTop.Elevation + parameterTopOffset.AsDouble();
                    else return levelTop.Elevation + parameterTopOffset.AsDouble() - this.settings.CurrentLevel.Elevation;
                }
                else {
                    Parameter parameterUserHeight = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
                    if (this.settings.CurrentLevel == null) return parameterUserHeight.AsDouble();
                    else return this.settings.CurrentLevel.Elevation + parameterUserHeight.AsDouble();
                }
            }
            else {
                Parameter parameterUserHeight = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
                if (this.settings.CurrentLevel == null) return parameterUserHeight.AsDouble();
                return this.settings.CurrentLevel.Elevation + parameterUserHeight.AsDouble();
            }
        }
        public double GetFramingRotation(FamilyInstance familyInstance, LocationPoint locationPoint, string specialCase) {
            double angleRadians = 0;
            if (locationPoint != null) {
                angleRadians = locationPoint.Rotation;
            }
            else {
                Parameter parameterRotation = familyInstance.get_Parameter(BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE);
                if (parameterRotation != null) {
                    if (specialCase == "Column") angleRadians = -1 * parameterRotation.AsDouble();  // Not sure why  we need the minus one but seems to work
                    else  angleRadians = parameterRotation.AsDouble(); // "Beam" case                    
                }
            }
            double angleDegrees = angleRadians * 180 / Math.PI;
            if (angleDegrees < 0) angleDegrees = angleDegrees + 360;
            return angleDegrees;
        }
        public string GetBeamJustification(FamilyInstance familyInstance) {
            Parameter parameterJustificaiton = familyInstance.get_Parameter(BuiltInParameter.BEAM_V_JUSTIFICATION);
            if (parameterJustificaiton.AsInteger() == (int)BeamSystemJustifyType.Center) return "Center";
            else return "Top";
        }

        public List<Sketch> GetSketches(Element element) {
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                transaction.Start("Getting Sketch");
                ICollection<ElementId> elementIds = this.settings.DocumentDb.Delete(element.Id);
                transaction.RollBack();
                List<Sketch> sketches = new List<Sketch>();
                foreach (ElementId elementId in elementIds) {
                    Element elementTest = this.settings.DocumentDb.GetElement(elementId);
                    if (elementTest is Sketch) {
                        Sketch sketch = (Sketch)elementTest;
                        sketches.Add(sketch);
                    }
                }
                return sketches;
            }
        }

        #endregion

        #region Private Functions                       // ****************************** Private Functions *******************************************************

        private void MirrorFamilyInstance(ref FamilyInstance familyInstance, XYZ pointMirror, string axis) {
            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                if (TrapErrors) AddFailureHandler(transaction);
                transaction.Start("Mirroring FamilyInstance " + axis);
                Plane plane;
                //if (axis == "X") plane = new Plane(XYZ.BasisX, pointMirror);
                if (axis == "X") plane = Plane.CreateByNormalAndOrigin(XYZ.BasisX, pointMirror);
                //else plane = new Plane(XYZ.BasisY, pointMirror);  // "Y" case
                else plane = Plane.CreateByNormalAndOrigin(XYZ.BasisY, pointMirror);  // "Y" case
                ElementId lastElementId = familyInstance.Id;
                ElementTransformUtils.MirrorElement(this.settings.DocumentDb, familyInstance.Id, plane);
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") {
                        familyInstance = null;
                        return;
                    }
                }
            }

            // Code from Jeremy Tammik that I don't really understand; It gets the element(s) after a given ElementId value.
            FilteredElementCollector a = new FilteredElementCollector(this.settings.DocumentDb).WhereElementIsNotElementType();
            BuiltInParameter bip = BuiltInParameter.ID_PARAM;
            ParameterValueProvider provider = new ParameterValueProvider(new ElementId(bip));
            FilterNumericRuleEvaluator evaluator = new FilterNumericGreater();
            FilterRule rule = new FilterElementIdRule(provider, evaluator, familyInstance.Id);
            ElementParameterFilter filter = new ElementParameterFilter(rule);
            a = a.WherePasses(filter);

            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {
                transaction.Start("Deleting FamilyInstance");
                this.settings.DocumentDb.Delete(familyInstance.Id);   // Delete the old one that was mirrored
                transaction.Commit();
                if (TrapErrors && this.failureHandler.ErrorMessage != "") {
                    InnerErrorMessage = this.failureHandler.ErrorMessage;
                    InnerErrorSeverity = this.failureHandler.ErrorSeverity;
                    if (InnerErrorSeverity == "Error") familyInstance = null;
                }
            }
            familyInstance = (FamilyInstance)a.First();           // Assuming only one such element and it is the new one                                    
        }                    
        
        private void FindMinMax(XYZ point, ref double? xMin, ref double? yMin, ref double? zMin, ref double? xMax, ref double? yMax, ref double? zMax) {
            if (xMin == null) xMin = point.X;
            else if (xMin > point.X) xMin = point.X;
            if (yMin == null) yMin = point.Y;
            else if (yMin > point.Y) yMin = point.Y;
            if (zMin == null) zMin = point.Z;
            else if (zMin > point.Z) zMin = point.Z;
            if (xMax == null) xMax = point.X;
            else if (xMax < point.X) xMax = point.X;
            if (yMax == null) yMax = point.Y;
            else if (yMax < point.Y) yMax = point.Y;
            if (zMax == null) zMax = point.Z;
            else if (zMax < point.Z) zMax = point.Z;
        }

        private void GetValues() {
            FilteredElementCollector collector;  // Reused

            // Types
            collector = new FilteredElementCollector(this.settings.DocumentDb);
            ICollection<Element> wallTypes = collector
                .OfClass(typeof(WallType))
                .ToElements();
            foreach (WallType wallType in wallTypes) {
                if (!this.wallTypes.ContainsKey(wallType.Name)) this.wallTypes.Add(wallType.Name, wallType);
            }
            collector = new FilteredElementCollector(this.settings.DocumentDb);
            ICollection<Element> floorTypes = collector
                .OfClass(typeof(FloorType))
                .ToElements();
            foreach (FloorType floorType in floorTypes) {
                if (!this.floorTypes.ContainsKey(floorType.Name)) this.floorTypes.Add(floorType.Name, floorType);
            }
            collector = new FilteredElementCollector(this.settings.DocumentDb);
            ICollection<Element> filledRegionTypes = collector
                .OfClass(typeof(FilledRegionType))
                .ToElements();
            foreach (FilledRegionType filledRegionType in filledRegionTypes) {
                if (!this.filledRegionTypes.ContainsKey(filledRegionType.Name)) this.filledRegionTypes.Add(filledRegionType.Name, filledRegionType);
            }

            // Levels
            ICollection<Element> elements;
            collector = new FilteredElementCollector(this.settings.DocumentDb);
            elements = collector.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements();
            foreach (Element element in elements) {
                Level level = (Level)element;
                if (!this.levels.ContainsKey(level.Name)) this.levels.Add(level.Name, level);
            }

            // Families
            Categories categories = this.settings.DocumentDb.Settings.Categories;
            BuiltInCategory bicColumnArchitectural = BuiltInCategory.OST_Columns;
            BuiltInCategory bicColumnStructural = BuiltInCategory.OST_StructuralColumns;
            BuiltInCategory bicFramingStructural = BuiltInCategory.OST_StructuralFraming;
            BuiltInCategory bicGenericModel = BuiltInCategory.OST_GenericModel;
            ElementId elementIdCategoryColumnArch = categories.get_Item(bicColumnArchitectural).Id;
            ElementId elementIdCategoryColumnStruct = categories.get_Item(bicColumnStructural).Id;
            ElementId elementIdCategoryFramingStruct = categories.get_Item(bicFramingStructural).Id;
            ElementId elementIdCategoryGenericModel = categories.get_Item(bicGenericModel).Id;
            FilteredElementCollector collectorFamilies = new FilteredElementCollector(this.settings.DocumentDb);
            ICollection<Element> elementsFamily = collectorFamilies.OfClass(typeof(Family)).ToElements();
            SortedDictionary<string, FamilySymbol> typesDictionary;
            foreach (Family family in elementsFamily) {
                string familyName = family.Name;
                if (!this.familyTypes.ContainsKey(familyName)) {
                    Category category = null;
                    typesDictionary = new SortedDictionary<string, FamilySymbol>();
                    // 2015 Deprecated                                                         
                    //while (symbolItor.MoveNext()) {
                    //    familySymbol = (FamilySymbol)symbolItor.Current;
// TODO Look at the logic here.  Why do we loop through all the family symbols and then use value about 10 lines later when we make an instance;  Shouldn't the loop be outside?
                    ISet<ElementId> familySymbols = family.GetFamilySymbolIds();
                    FamilySymbol familySymbol = null;
                    foreach (ElementId symbolId in familySymbols) {
                        familySymbol = (FamilySymbol)this.settings.DocumentDb.GetElement(symbolId);                        
                        string typeName = familySymbol.Name;  // Note that symbol name is actually the type name
                        if (!typesDictionary.ContainsKey(typeName)) typesDictionary.Add(typeName, familySymbol);
                        category = familySymbol.Category;  // Note that the FamilySymbol has a category but the Family does not.                        
                    }
                    this.familyTypes.Add(familyName, typesDictionary);                    
                    if (category != null) {
                        ElementId categoryId = category.Id;
                        // Columns - Architectural
                        if (categoryId == elementIdCategoryColumnArch) this.columnArchitecturalTypes.Add(familyName, typesDictionary);
                        // Columns - Structural
                        else if (categoryId == elementIdCategoryColumnStruct) this.columnStructuralTypes.Add(familyName, typesDictionary);
                        // Beams (Note that to Revit it is "Framing"; not sure how to resolve this
                        else if (categoryId == elementIdCategoryFramingStruct) this.beamTypes.Add(familyName, typesDictionary);
                        // AdaptiveComponent
                        else if (categoryId == elementIdCategoryGenericModel) {
                            FamilyInstance familyInstance = null;
                            using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {                                                         
                                AddFailureHandler(transaction);
                                transaction.Start("Making Temporary Instance");
                                try {
                                    if (this.settings.DocumentDb.IsFamilyDocument)        
                                        familyInstance = this.settings.DocumentDb.FamilyCreate.NewFamilyInstance(new XYZ(0, 0, 0), familySymbol, StructuralType.NonStructural);
                                    else familyInstance = this.settings.DocumentDb.Create.NewFamilyInstance(new XYZ(0, 0, 0), familySymbol, this.settings.CurrentLevel, StructuralType.NonStructural);
                                }
                                catch {
                                    familyInstance = null;
                                }
                                transaction.Commit();
                            }
                            if (familyInstance != null) {
                                using (Transaction transaction = new Transaction(this.settings.DocumentDb)) {  // Note that we need new transaction to get points
                                    try {
                                        AddFailureHandler(transaction);
                                        transaction.Start("Using and Deleting Temporary Instance");
                                        IList<FamilyPointPlacementReference> familyPoints = familyInstance.GetFamilyPointPlacementReferences();
                                        if (familyPoints.Count > 0) this.adaptiveComponentTypes.Add(familyName, typesDictionary);
                                        this.settings.DocumentDb.Delete(familyInstance.Id);
                                        transaction.Commit();
                                    }
                                    catch {  }
                                }
                            }
                        }
                    }                                                            
                }
            }

            // Mullions
            SortedDictionary<string, MullionType> mullionTypesNew;
            foreach (MullionType mullionType in this.settings.DocumentDb.MullionTypes) {
                string familyName = mullionType.Family.Name;
                if (this.mullionTypes.ContainsKey(familyName)) {
                    mullionTypesNew = this.mullionTypes[familyName];
                    if (!mullionTypesNew.ContainsKey(mullionType.Name)) mullionTypesNew.Add(mullionType.Name, mullionType);
                }
                else {
                    mullionTypesNew = new SortedDictionary<string, MullionType>();
                    mullionTypesNew.Add(mullionType.Name, mullionType);
                    this.mullionTypes.Add(familyName, mullionTypesNew);
                }                               
            }
        }

        private SketchPlane MakeSketchPlaneNormal(XYZ normal, XYZ center) {
            Plane plane = Plane.CreateByNormalAndOrigin(normal, center);
            return SketchPlane.Create(this.settings.DocumentDb, plane);
        }
        private SketchPlane MakeSketchPlane() {
            XYZ origin = new XYZ(0, 0, 0);
            XYZ norm = new XYZ(0, 0, 1);
            Plane plane = Plane.CreateByNormalAndOrigin(norm, origin);
            return SketchPlane.Create(this.settings.DocumentDb, plane);
        }
        private SketchPlane MakeSketchPlane(XYZ point1, XYZ point2) {                        
            // We need any point that is not one of the other points.  This is one of many possible planes.  
            // It has the nice feature that if the two given Z values are equal the plane will be horizontal.
            double point3X = Math.Abs(point1.X) + Math.Abs(point2.X) + 10;
            double point3Y = Math.Abs(point1.Y) + Math.Abs(point2.Y) + 10;
            double point3Z = point1.Z;
            return MakeSketchPlane(point1, point2, new XYZ(point3X, point3Y, point3Z));
        }
        private SketchPlane MakeSketchPlane(XYZ point1, XYZ point2, XYZ point3) {
            //Line line1 = Line.CreateBound(point1, point2);
            Line line1 = Line.CreateBound(point2, point1);
            Line line2 = Line.CreateBound(point1, point3); 
            CurveLoop curveLoop = new CurveLoop();
            curveLoop.Append(line1);
            curveLoop.Append(line2);
            Plane plane = curveLoop.GetPlane();  
            return SketchPlane.Create(this.settings.DocumentDb, plane);
        }

        private void AddFailureHandler(Transaction transaction) {
            this.failureHandler = new FailureHandler();
            this.failureHandlingOptions = transaction.GetFailureHandlingOptions();
            this.failureHandlingOptions.SetFailuresPreprocessor(this.failureHandler);
            this.failureHandlingOptions.SetClearAfterRollback(true);
            transaction.SetFailureHandlingOptions(this.failureHandlingOptions);
        }
    }

    #endregion
}
