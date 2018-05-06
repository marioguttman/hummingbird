
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using HummingbirdUtility;

namespace WhiteFeet.HummingbirdWriterTester {

    public partial class Menu : Form {

        // ************************************************* Module Variables *****************************************************
        private const string PROGRAM_NAME = "HummingbirdWriterTester";
        private const string INI_FILE_NAME = "HummingbirdWriterTester.ini";
        private const string USER_FOLDER_HUMMINGBIRD = "Hummingbird";   // Used to store all Hummingbird data
        private const string DELIMITER = "\t";

        private string userFolderPath = "";
        private string iniFilePath = "";

        private string csvFolderPath;
        private string csvFileName;
        private string csvFilePath;

        // ********************************************************* Constructor ******************************************************
        public Menu() {
            InitializeComponent();
            FindOrMakeUserFolder();  // Not looking for error.  this.UserFolderPath = "" is indication of failure
            GetIniFilePath();
            ReadIniFile();
            textBoxCsvFolder.Text = this.csvFolderPath;
            textBoxCsvFileName.Text = this.csvFileName;            
        }

        // ******************************************************** Private Functions ********************************************************
        private bool RunProcess() {
            Cursor.Current = Cursors.WaitCursor;
            try {

                CsvWriter csvWriter = new CsvWriter();

                // ********************************************************************************************************************************
                // ************************************* HummingbirdUtility Basic Function Signatures ***************************************
                // ********************************************************************************************************************************
                //                
                // "Version" is a property of the ExcelWriter object.

                // Revit classes "XYZ", "Line", "Arc", "Ellipse", "NurbSpline", and "HermiteSpline" have corresponding classes in Hummingbird named
                // "HbXYZ", "HbLine", "HbArc", "HbEllipse", "HbNurbSpline", and "HbHermiteSpline" which derive from a parent class of "HbItem".
                // These have constructors:
                // HbXYZ(double x, double y, double z)
                // HbLine(HbXYZ pointStart, HbXYZ pointEnd)
                // HbArc(HbXYZ pointStart, HbXYZ pointEnd, HbXYZ pointMid)
                // HbEllipse(HbXYZ pointFirst, HbXYZ pointSecond, double radiusY, string mode)  radiusY is perpendicular to points; mode "Half" or "Full".
                // HbNurbSpline(List<HbXYZ> points)
                // HbHermiteSpline(List<HbXYZ> points)
                // (Note, these also have a default constructor with no parameters and properties that correspond to the parameters that must be set individually.)
                // They also have special functions specifically for use with Autodesk DesignScript
                // static HbXYZ New(double x, double y, double z)
                // static HbLine New(HbXYZ pointStart, HbXYZ pointEnd)
                // static HbArc New(HbXYZ pointStart, HbXYZ pointEnd, HbXYZ pointMid)
                // static HbEllipse New(HbXYZ pointFirst, HbXYZ pointSecond, double radiusY, string mode)  radiusY is perpendicular to points; mode "Half" or "Full".
                // static HbNurbSpline New(List<HbXYZ> points)
                // static HbHermiteSpline New(List<HbXYZ> points)
                // There are also some list functions that workaround the lack of a native "List" object in DesignScript
                // HbItemList()
                // static void Add(HbItemList list, HbItem hbItem)
                // HbItemListList()
                // static void Add(HbItemListList listList, HbItemList list)
                //
                // The following are methods of the ExcelWriter object.
                //
                // List of worksheet names:
                // List<string> GetWorksheetNames()                    
                //
                // Set Actions:
                // void SetLevel(string levelName)
                // void SetWallType(string wallTypeName)
                // void SetWallHeight(double height)
                // void SetFloorType(string floorTypeName)
                // void SetFamilyType(string familyName, string typeName)
                // void SetFamilyFlipped(bool flipHand, bool flipFacing)
                // void SetFamilyMirrored(bool mirrorX, bool mirrorY)
                // void SetFamilyRotation(double rotation)
                // void SetColumnMode(string mode, string familyName, string typeName)
                // void SetColumnHeight(double height)
                // void SetColumnRotation(double rotation)
                // void SetBeamType(string familyName, string typeName)
                // void SetBeamJustification(string justification) {
                // void SetBeamRotation(double rotation) {
                // void SetAdaptiveComponentType(string familyName, string typeName)
                // void SetFamilyExtrusionHeight(double height)
                // void SetMullionType(string familyName, string typeName)
                //
                // Add Actions:
                // void AddGrid(HbXYZ point1, HbXYZ point2)                  // Line case
                // void AddGrid(HbXYZ point1, HbXYZ point2, HbXYZ point3)    // Arc case (point1 = start; point2 = end; point3 = any point in arc.)
                // void AddLevel(double elevation, string name = null) {     // name is optional
                // void AddDetailLine(HbXYZ point1, HbXYZ point2)
                // void AddDetailArc(HbXYZ point1, HbXYZ point2, HbXYZ point3)
                // void AddDetailEllipse(HbXYZ point1, HbXYZ point2, double radiusY, string mode)
                // void AddDetailNurbsSpline(HbXYZ point1, HbXYZ point2, HbXYZ point3 = null, HbXYZ point4 = null) // points 3 and 4 are optional
                // void AddDetailNurbsSpline(List<HbXYZ> points)   
                // void AddDetailCurves(List<HbItem> curves)
                // void AddModelLine(HbXYZ point1, HbXYZ point2)
                // void AddModelArc(HbXYZ point1, HbXYZ point2, HbXYZ point3)
                // void AddModelEllipse(HbXYZ point1, HbXYZ point2, double radiusY, string mode)
                // void AddModelNurbsSpline(HbXYZ point1, HbXYZ point2, HbXYZ point3 = null, HbXYZ point4 = null) // points 3 and 4 are optional
                // void AddModelNurbsSpline(List<HbXYZ> points)  
                // void AddModelCurves(List<HbItem> curves)
                // void AddTopographySurface(HbXYZ point1, HbXYZ point2, HbXYZ point3, HbXYZ point4 = null) {
                // void AddTopographySurface(List<HbXYZ> points) {

                // void AddWall(HbXYZ point1, HbXYZ point2)                   // Straight line in plan.
                // void AddWall(HbXYZ point1, HbXYZ point2, HbXYZ point3)  // Arc in plan
                // void AddWall(List<HbItem> curvesList)                         // List of lines, arcs, ellipses, or splines in plan
                // void AddWall(List<List<HbItem>> curvesList)                   // List of Lists controls profile in elevation               
                // void AddFloor(HbXYZ point1, HbXYZ point2, HbXYZ point3, HbXYZ point4)
                // void AddFloor(List<List<HbItem>> curvesList)
                // void AddFamilyInstance(HbXYZ point1)
                // void AddColumn(HbXYZ point1)
                // void AddColumn(HbXYZ point1, HbXYZ point2)
                // void AddBeam(HbXYZ point1, HbXYZ point2)
                // void AddAdaptiveComponent(HbXYZ point1, HbXYZ point2 = null, HbXYZ point3 = null, HbXYZ point4 = null)   // points 2, 3, and 4 are optional
                // void AddAdaptiveComponent(List<HbXYZ> points)
                // void AddAreaBoundaryLine(HbXYZ point1, HbXYZ point2)                                // Line case
                // void AddAreaBoundaryLine(HbXYZ point1, HbXYZ point2, HbXYZ point3)               // Arc case
                // void AddAreaBoundaryLine(HbXYZ point1, HbXYZ point2, double radiusY, string mode)   // Ellipse case
                // void AddAreaBoundaryLine(List<HbXYZ> points)                                           // Spline case
                // void AddAreaBoundaryLine(List<HbItem> curves)                                          // List of lines, arcs, ellipses, or splines in plan
                // void AddRoomSeparationLine(HbXYZ point1, HbXYZ point2)                              // Line case
                // void AddRoomSeparationLine(HbXYZ point1, HbXYZ point2, HbXYZ point3)             // Arc case
                // void AddRoomSeparationLine(HbXYZ point1, HbXYZ point2, double radiusY, string mode) // Ellipse case
                // void AddRoomSeparationLine(List<HbXYZ> points)                                         // Spline case
                // void AddRoomSeparationLine(List<HbItem> curves)                                        // List of lines, arcs, ellipses, or splines in plan                                                 
                // void AddArea(HbXYZ point1);
                // void AddRoom(HbXYZ point1); 
                // void AddReferencePoint(HbXYZ point)                 
                // void AddCurveByPoints(List<HbXYZ> points)       
                // void AddloftForm(List<List<HbXYZ>> points)
                // void AddFamilyExtrusion(List<List<HbItem>> curvesList, string nameFamily = null, HbXYZ pointInsert = null) {
                // void AddFamilyExtrusion(List<List<HbItem>> curvesList, HbXYZ pointInsert) {
                //
                // Modify Actions:
                // void ModifyParameterSet(string parameterName, string value)
                // void ModifyCurtainGridUAdd(double offsetPrimary);
                // void ModifyCurtainGridUAdd(double offsetPrimary, double offsetSecondary);
                // void ModifyCurtainGridVAdd(double offsetPrimary);
                // void ModifyCurtainGridVAdd(double offsetPrimary, double offsetSecondary);
                // void ModifyMullionUAdd(double offsetPrimary);
                // void ModifyMullionUAdd(double offsetPrimary, double offsetSecondary);
                // void ModifyMullionVAdd(double offsetPrimary);
                // void ModifyMullionVAdd(double offsetPrimary, double offsetSecondary);

                // ************************************************************************************************************************************
                // ************************************* RevitModelBuilderUtility Component Function Signatures ***************************************
                // ************************************************************************************************************************************
                //
                // Users will typically avoid using these in favor of the basic forms.  Provided here only for completeness
                //
                // Use-Add Actions:
                // void UsePoints(HbXYZ point1, HbXYZ point2 = null, HbXYZ point3 = null, HbXYZ point4 = null) { // points 2, 3, and 4 are optional
                // void UsePoints(List<HbXYZ> points) // General form of more than four points
                // void AddDetailNurbsSpline()  // Must follow UsePoints
                // void AddModelNurbsSpline()   // Must follow UsePoints
                // void AddAdaptiveComponent()  // Must follow UsePoints
                // void AddAreaBoundaryLine()   // Spline - Must follow UsePoints (Note: no simple form of spline is currently offered
                // void AddRoomSeparationLine() // Spline - Must follow UsePoints (Note: no simple form of spline is currently offered
                // void AddCurveByPoints()      // Must follow UsePoints
                // void AddTopographySurface()  // Must follow UsePoints
                //
                // Draw-Use-Add Actions:
                // (Also see UsePoints() above)
                // void DrawCurveArray()
                // void DrawLine(HbXYZ point1, HbXYZ point2)
                // void DrawArc(HbXYZ point1, HbXYZ point2, HbXYZ point3)
                // void AddWall()
                // void AddFloor()
                // void AddFamilyExtrusion(string nameFamily, HbXYZ pointInsert = null) {  // Must follow a Curve Array Set
                // void DrawCurveArray(List<HbItem> curves) {   // Combined form; curves must be HbItem type               
                //
                // Model-Use-Add Actions:
                // (Also see UsePoints() above)
                // void ModelReferenceArray()  // Must be first statement
                // void AddLoftForm()          // Must follow ModelReferenceArray() and UsePoints()
                //

                // ****************************************************************************************************************
                // ******************************************* Start of Sample Code Section ***************************************
                // ****************************************************************************************************************
                //
                // This is the portion of the code that is typically emulated for a particular project use.

                // Sample Revit hb items and lists
                HbXYZ point11 = new HbXYZ(0, 0, 0); HbXYZ point12 = new HbXYZ(10, 0, 0); HbXYZ point13 = new HbXYZ(20, 0, 0); HbXYZ point14 = new HbXYZ(15, 5, 0);
                HbXYZ point21 = new HbXYZ(0, 10, 0); HbXYZ point22 = new HbXYZ(10, 10, 0); HbXYZ point23 = new HbXYZ(20, 10, 0); HbXYZ point24 = new HbXYZ(15, 15, 0);
                List<HbXYZ> pointsList1 = new List<HbXYZ>(); pointsList1.Add(point11); pointsList1.Add(point12); pointsList1.Add(point13); pointsList1.Add(point14);
                List<HbXYZ> pointsList2 = new List<HbXYZ>(); pointsList2.Add(point21); pointsList2.Add(point22); pointsList2.Add(point23); pointsList2.Add(point24);
                //List<HbXYZ> pointsList3 = new List<HbXYZ>(); pointsList3.Add(point21); pointsList3.Add(point22); pointsList3.Add(point23);
                //                                                   pointsList3.Add(point24); pointsList3.Add(point24); pointsList3.Add(point24);
                List<List<HbXYZ>> pointsListList = new List<List<HbXYZ>>(); pointsListList.Add(pointsList1); pointsListList.Add(pointsList2);
                HbLine line1 = new HbLine(); line1.PointStart = point11; line1.PointEnd = point12;
                HbLine line2 = new HbLine(); line2.PointStart = point21; line2.PointEnd = point22;
                HbArc arc1 = new HbArc(); arc1.PointStart = point11; arc1.PointEnd = point12; arc1.PointMid = point13;
                HbArc arc2 = new HbArc(); arc2.PointStart = point21; arc2.PointEnd = point22; arc2.PointMid = point23;
                HbNurbSpline nurbSpline1 = new HbNurbSpline(); nurbSpline1.Points.Add(point21); nurbSpline1.Points.Add(point12); nurbSpline1.Points.Add(point23);
                HbNurbSpline nurbSpline2 = new HbNurbSpline(); nurbSpline2.Points.Add(point11); nurbSpline2.Points.Add(point22); nurbSpline2.Points.Add(point13);
                HbNurbSpline nurbSpline3 = new HbNurbSpline(); nurbSpline3.Points.Add(point11); nurbSpline3.Points.Add(point22); nurbSpline3.Points.Add(point13);
                                                               nurbSpline3.Points.Add(point11); nurbSpline3.Points.Add(point22); nurbSpline3.Points.Add(point13);
                List<HbCurve> curvesList1 = new List<HbCurve>(); curvesList1.Add(line1); curvesList1.Add(arc1); curvesList1.Add(nurbSpline1);
                List<HbCurve> curvesList2 = new List<HbCurve>(); curvesList2.Add(line2); curvesList2.Add(arc2); curvesList2.Add(nurbSpline3);
                List<List<HbCurve>> curvesListList = new List<List<HbCurve>>(); curvesListList.Add(curvesList1); curvesListList.Add(curvesList2);

                double levelElevation = 100;    //  for Level
                string levelName = "A New Level";

                double radiusY = 10;   // for ellipse
                string mode = "Full";

                double offsetPrimary = 5;   // for curtain gridds and mullions
                double offsetSecondary = 10;
                
                // Display the version
                MessageBox.Show("csvWriter.Version: " + csvWriter.Version);

                // Start .csv file
                textBoxCsvFolder.Text = textBoxCsvFolder.Text.Trim();
                if (!textBoxCsvFolder.Text.EndsWith(@"\")) textBoxCsvFolder.Text = textBoxCsvFolder.Text + @"\";
                this.csvFolderPath = textBoxCsvFolder.Text;
                textBoxCsvFileName.Text = textBoxCsvFileName.Text.Trim();
                if (!textBoxCsvFileName.Text.ToLower().EndsWith(".csv")) textBoxCsvFileName.Text = textBoxCsvFileName.Text + ".csv";
                this.csvFileName = textBoxCsvFileName.Text;
                this.csvFilePath = this.csvFolderPath + this.csvFileName;
                string returnMMessage = csvWriter.ConnectToFile(this.csvFilePath);
                if (returnMMessage != "") {
                    MessageBox.Show(returnMMessage);
                    return false;
                }

                // If checked, get ID values
                if (checkBoxPreserveId.Checked) {
                    // Note that if file doesn't exist yet we ignore option to preserve ID values
                    if (File.Exists(this.csvFilePath)) {
                        try {
                            string returnValue = csvWriter.ReadElementIds();
                            if (returnValue != "") {
                                MessageBox.Show("csvWriter.ReadElementIds() failed: " + returnValue, PROGRAM_NAME);
                                return false;
                            }
                        }
                        catch (Exception exception) {
                            MessageBox.Show("Exception in RunProcess() at csvWriter.ReadElementIds(): " + exception.Message, PROGRAM_NAME);
                            return false;
                        }
                    }
                }

                // Set Actions:
                csvWriter.SetLevel("Level 1");
                csvWriter.SetWallType("WallTypeName");
                csvWriter.SetWallHeight(10);
                csvWriter.SetFloorType("FloorTypeName");
                csvWriter.SetFamilyType("FamilyFamilyName", "FamilyTypeName");
                csvWriter.SetFamilyFlipped(true, false);
                csvWriter.SetFamilyMirrored(false, true);
                csvWriter.SetFamilyRotation(45);
                csvWriter.SetColumnMode("Architectural", "ColumnFamilyName", "ColumnTypeName");
                csvWriter.SetColumnHeight(10);
                csvWriter.SetColumnRotation(45);
                csvWriter.SetBeamType("BeamFamilyName", "BeamTypeName");
                csvWriter.SetBeamJustification("Top");
                csvWriter.SetBeamRotation(45);
                csvWriter.SetAdaptiveComponentType("ACompFamilyName", "ACompTypeName");
                csvWriter.SetFamilyExtrusionHeight(40);
                csvWriter.SetMullionType("AMullionFamilyName", "AMullionTypeName");
                //
                // Add Actions:
                csvWriter.AddGrid(point11, point12);           // Line case
                csvWriter.AddGrid(point11, point12, point13);  // Arc case
                levelElevation = 100;    //  for Level
                csvWriter.AddLevel(levelElevation, levelName);  
                csvWriter.AddLevel(levelElevation);              // name is optional
                csvWriter.AddDetailLine(point11, point12);
                csvWriter.AddDetailArc(point11, point12, point13);
                csvWriter.AddDetailEllipse(point11, point12, radiusY, mode);
                csvWriter.AddDetailNurbsSpline(point11, point12, point13, point14);
                csvWriter.AddDetailNurbsSpline(pointsList1);
                csvWriter.AddDetailCurves(curvesList1);
                csvWriter.AddDetailCurves(curvesList2);
                csvWriter.AddModelLine(point11, point12);
                csvWriter.AddModelArc(point11, point12, point13);
                csvWriter.AddModelEllipse(point11, point12, radiusY, mode);
                csvWriter.AddModelNurbsSpline(point11, point12);   // points 3 and 4 are optional
                csvWriter.AddModelNurbsSpline(point11, point12, point13);
                csvWriter.AddModelNurbsSpline(point11, point12, point13, point14);
                csvWriter.AddModelNurbsSpline(pointsList1);
                csvWriter.AddModelCurves(curvesList1);
                csvWriter.AddTopographySurface(point11, point12, point13);
                csvWriter.AddTopographySurface(point11, point12, point13, point14);
                csvWriter.AddTopographySurface(pointsList1);
                csvWriter.AddWall(point11, point12);
                csvWriter.AddWall(point11, point12, point13);
                csvWriter.AddWall(curvesList1);     // Plan list
                csvWriter.AddWall(curvesListList);  // Profile list of lists
                csvWriter.AddFloor(point11, point12, point13, point14);
                csvWriter.AddFloor(curvesListList);
                csvWriter.AddFamilyInstance(point11);
                csvWriter.AddColumn(point11);
                csvWriter.AddColumn(point11, point12);
                csvWriter.AddBeam(point11, point12);
                csvWriter.AddAdaptiveComponent(point11);  // points 2, 3, and 4 are optional
                csvWriter.AddAdaptiveComponent(point11, point12);
                csvWriter.AddAdaptiveComponent(point11, point12, point13);
                csvWriter.AddAdaptiveComponent(point11, point12, point13, point14);
                csvWriter.AddAdaptiveComponent(pointsList1);
                csvWriter.AddAreaBoundaryLine(point11, point12);                  // Line case
                csvWriter.AddAreaBoundaryLine(point11, point12, point13);         // Arc case
                csvWriter.AddAreaBoundaryLine(point11, point12, radiusY, mode);   // Ellipse case
                csvWriter.AddAreaBoundaryLine(pointsList1);                       // Spline case
                csvWriter.AddAreaBoundaryLine(curvesList1);                       // List of lines, arcs, ellipses, or splines in plan
                csvWriter.AddRoomSeparationLine(point11, point12);                // Line case
                csvWriter.AddRoomSeparationLine(point11, point12, point13);       // Arc case
                csvWriter.AddRoomSeparationLine(point11, point12, radiusY, mode); // Ellipse case
                csvWriter.AddRoomSeparationLine(pointsList1);                     // Spline case
                csvWriter.AddRoomSeparationLine(curvesList1);                     // List of lines, arcs, ellipses, or splines in plan
                csvWriter.AddArea(point11);
                csvWriter.AddRoom(point11); 
                csvWriter.AddReferencePoint(point11);                
                csvWriter.AddCurveByPoints(pointsList1);      
                csvWriter.AddLoftForm(pointsListList);
                ////TODO Not handling this case for now.  It is not allowed in the specification and not sure how it got inot the sample data
                //csvWriter.AddFamilyExtrusion(curvesListList, "ExtrusionFamilyName", point11);
                //csvWriter.AddFamilyExtrusion(curvesListList, "ExtrusionFamilyName");
                //csvWriter.AddFamilyExtrusion(curvesListList, point11);
                //csvWriter.AddFamilyExtrusion(curvesListList);
                //
                // Modify Actions:
                csvWriter.ModifyParameterSet("ParameterName", "Value");
                csvWriter.ModifyCurtainGridUAdd(offsetPrimary);
                csvWriter.ModifyCurtainGridUAdd(offsetPrimary, offsetSecondary);
                csvWriter.ModifyCurtainGridVAdd(offsetPrimary);
                csvWriter.ModifyCurtainGridVAdd(offsetPrimary, offsetSecondary);
                csvWriter.ModifyMullionUAdd(offsetPrimary);
                csvWriter.ModifyMullionUAdd(offsetPrimary, offsetSecondary);
                csvWriter.ModifyMullionVAdd(offsetPrimary);
                csvWriter.ModifyMullionVAdd(offsetPrimary, offsetSecondary);

                // These commands can also be used but it is proably better to try to use one of the more compact forms above.

                // Use-Add Actions:
                csvWriter.UsePoints(point11, point12, point13, point14);
                csvWriter.UsePoints(pointsList1);
                csvWriter.AddDetailNurbsSpline();
                csvWriter.UsePoints(pointsList1);
                csvWriter.AddModelNurbsSpline();
                csvWriter.UsePoints(pointsList1);
                csvWriter.AddAdaptiveComponent();
                csvWriter.UsePoints(pointsList1);
                csvWriter.AddAreaBoundaryLine();   // Spline - Must follow UsePoints (Note: no simple form of spline is currently offered
                csvWriter.UsePoints(pointsList1);
                csvWriter.AddRoomSeparationLine(); // Spline - Must follow UsePoints (Note: no simple form of spline is currently offered
                csvWriter.UsePoints(pointsList1);
                csvWriter.AddCurveByPoints(); 
                
                // Draw-Use-Add Actions:                
                csvWriter.DrawCurveArray();          //This is non-combined form
                csvWriter.DrawLine(point11, point12);
                csvWriter.DrawArc(point11, point12, point13);
                csvWriter.AddWall();
                csvWriter.DrawCurveArray();
                csvWriter.DrawLine(point11, point12);
                csvWriter.DrawArc(point11, point12, point13);
                csvWriter.AddFloor();
                csvWriter.DrawCurveArray();
                csvWriter.DrawLine(point11, point12);
                csvWriter.DrawArc(point11, point12, point13);
                csvWriter.AddFamilyExtrusion("ExtrusionFamilyName", point11);
                csvWriter.DrawCurveArray();
                csvWriter.DrawLine(point11, point12);
                csvWriter.DrawArc(point11, point12, point13);
                csvWriter.AddFamilyExtrusion("ExtrusionFamilyName");
                csvWriter.DrawCurveArray(curvesList1);  //This is combined form
                csvWriter.AddFamilyExtrusion();

                // Model-Use-Add Actions:
                // (These examples are not complete)
                csvWriter.ModelReferenceArray();  
                csvWriter.AddLoftForm();
          
                //  Write the file
                csvWriter.WriteFile();


                // ****************************************************************************************************************
                // ******************************************** End of Sample Code Section ****************************************
                // ****************************************************************************************************************

                MessageBox.Show("Process Completed", PROGRAM_NAME);

                return true;
            }
            catch (Exception exception) {
                MessageBox.Show("Error in 'RunProcess()'.\nSystem message: " + exception.Message, PROGRAM_NAME);
                return false;
            }
            finally {
                Cursor.Current = Cursors.Default;
            }

        }

        // ********************************************************** Ini File Functions ***************************************************
        private bool GetIniFilePath() {
            try {
                if (this.userFolderPath == "") {
                    this.iniFilePath = "";
                } else {
                    this.iniFilePath = Path.Combine(this.userFolderPath, INI_FILE_NAME);
                }
                return true;
            }
            catch {
                return false;
            }
        }
        private bool ReadIniFile() {
            if (this.iniFilePath == "") return true; //no warning, just no action
            try {

                if (File.Exists(this.iniFilePath)) {
                    using (StreamReader streamReader = File.OpenText(this.iniFilePath)) {
                        this.csvFolderPath = GetOneSettingString(streamReader);
                        this.csvFileName = GetOneSettingString(streamReader);
                        checkBoxPreserveId.Checked = GetOneSettingBool(streamReader);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        private string GetOneSettingString(StreamReader streamReader) {
            string[] inputLine;
            string line = streamReader.ReadLine();
            if (line == null) return "";
            inputLine = line.Split(DELIMITER.ToCharArray());
            if (inputLine.GetLength(0) != 2) return "";
            return inputLine[1];
        }
        private bool GetOneSettingBool(StreamReader streamReader) {
            string[] inputLine;
            string line = streamReader.ReadLine();
            if (line == null) return false;
            inputLine = line.Split(DELIMITER.ToCharArray());
            if (inputLine.GetLength(0) != 2) return false;
            if (inputLine[1] == "true") return true;
            else return false;
        }

        private bool WriteIniFile() {
            if (this.iniFilePath == "") return true; //no warning, just no action
            try {
                //Note, if file doesn't exist it is created
                // The using statement also closes the StreamWriter.
                using (StreamWriter sw = new StreamWriter(this.iniFilePath)) {
                    sw.WriteLine("CsvFolderPath" + DELIMITER + this.csvFolderPath);
                    sw.WriteLine("CsvFileName" + DELIMITER + this.csvFileName);
                    if (checkBoxPreserveId.Checked) sw.WriteLine("PreserveId" + DELIMITER + "true");
                    else sw.WriteLine("PreserveId" + DELIMITER + "false");
                }
                return true;
            }
            catch { return false; }
        }


        // ********************************************************* Utility Functions *****************************************************
        private bool FindOrMakeUserFolder() {
            try {
                this.userFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), USER_FOLDER_HUMMINGBIRD);
                if (!Directory.Exists(this.userFolderPath)) {
                    try {
                        Directory.CreateDirectory(this.userFolderPath);
                    } catch {
                        this.userFolderPath = "";
                        return false;  // Returning silently.  this.UserFolderPath = "" is indication of failure
                    }
                }
                return true;
            } catch {
                // Returning silently for now
                return false;
            }
        }


        // ********************************************* Event Handlers ******************************************
        private void buttonClose_Click(object sender, EventArgs e) {
            this.csvFolderPath = textBoxCsvFolder.Text;
            this.csvFileName = textBoxCsvFileName.Text;
            WriteIniFile();
            Close();
        }

        private void buttonProcess_Click(object sender, EventArgs e) {
            if (textBoxCsvFolder.Text == "") {
                MessageBox.Show("CSV Folder Path must be specified.");
                return;
            }
            this.csvFolderPath = textBoxCsvFolder.Text;
//TODO deal with .csv extension missing
//if (textBoxCsvFileName.Text == "") textBoxCsvFileName.Text = "Tab Name"; ;
            this.csvFileName = textBoxCsvFileName.Text;
            RunProcess();
        }

        private void buttonBrowse_Click(object sender, EventArgs e) {
            System.Windows.Forms.DialogResult dialogResult;
            string path = textBoxCsvFolder.Text;
            if (path.EndsWith("\\")) path = path.Substring(0, path.Length - 1);  //In case user added a "\"
            if (Directory.Exists(path)) folderBrowserDialog1.SelectedPath = path;
            else folderBrowserDialog1.SelectedPath = "C:\\";
            dialogResult = folderBrowserDialog1.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK) {
                path = folderBrowserDialog1.SelectedPath.ToString();
                if (Directory.Exists(path)) textBoxCsvFolder.Text = path;
                else textBoxCsvFolder.Text = "";
                this.csvFolderPath = textBoxCsvFolder.Text;
            }
        }
    }
}
