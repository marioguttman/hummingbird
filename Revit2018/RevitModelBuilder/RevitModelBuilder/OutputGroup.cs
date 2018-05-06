using System;
using System.Collections.Generic;
using System.Linq;     // For controlPoints.ElementAt(indexPoint)
using System.Collections; //For IEnumerable without type

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using RevitUtility;


// -------------------------------------------------- Documentation ---------------------------------------------------------------------
//
// Every Revit iten that is going to be written to the .CSV file is associated with a new "OutputGroup" item, created in "ExportElement.ProcessRecords()".
// 
// The OutpuGroup contains one or more "OutputItem"s.  Each of these will be processed by "ExportElement.ProcessRecords()" to create a line in the .CSV file.
//
// The OutputGroup constructor determines:
// - Determines if it is a supported Revit type?  (If not, then the OutputGroup.Valid value is false and OutputGroup.ErrorMessage explains why.)
// - Creates set statements as needed.
// - Check for a complex data requirement:
//   - Is it based on a Sketch?  Then get the sketch (possibly with nested loops) and make all of the related OutputItems 
//        - If there is a conversion mode set then all of the sketch items are the final product.
//        - If no conversion is set, then the sketch items will be used with a final "Add" line below. 
//   - If it was not a Sketch type, then is it a NurbSpline? (It can't be both a Sketch and a NurbSpline.) 
//        - Then make the OutputItems for the spline before the final "Add" line below.
//   - Unless the sketch was the final output (a conversion mode was set and the element was based on a sketch) run the "Simple case.  
//        - This creates the "Add" line.


namespace RevitModelBuilder {

    // Global Enums for output conditions
    public enum ConversionMode {None, DetailLines, ModelLines, FilledRegions};
    public enum FamilyInstanceCondition { NotSet, Column, Beam, AdaptiveComponent };

    class OutputGroup: IEnumerable, IEnumerator {

        #region Module Variables                        // ****************************** Module Variables ******************************************

        public Element Element { set; get; }
        public List<OutputItem> OutputItems { set; get; }
        public bool Valid { set; get; }
        public string ErrorMessage { set; get; }

        // Local version of parameters
        private int precision;
        private UtilitySettings settings;
        private UtilityElements elements;

        private RevitTypeNames revitTypes;

        private int position = -1;    // For Enumerator
        private int sequenceNumber = 0;
        private string rowIdRoot;
        private string rowId;

        private Level levelThis;   
        private WallType wallTypeThis = null;
        private double wallHeightThis = 0;  
        private FloorType floorTypeThis = null;
        private FilledRegionType filledRegionTypeThis = null;
        private string columnModeThis = "";
        private string columnFamilyThis = "";
        private string columnTypeThis = "";
        private double columnHeightThis = 0;
        private double columnRotationThis = 0;
        private string beamFamilyThis= "";
        private string beamTypeThis= "";
        private double beamRotationThis = 0;
        private string beamJustificationThis= "";
        private string adaptiveComponentFamilyThis= "";
        private string adaptiveComponentTypeThis= "";

        #endregion

        #region Constructor                             // ****************************** Constructor ***********************************************

        public OutputGroup(string rowIdRoot, Element element, int precision, ConversionMode conversionMode, UtilitySettings utilitySettings, UtilityElements utilityElements) {

            this.rowIdRoot = rowIdRoot;
            Element = element;
            this.precision = precision;
            this.settings = utilitySettings;
            this.elements = utilityElements;
            this.revitTypes = new RevitTypeNames();
            this.levelThis = (Level)utilitySettings.DocumentDb.GetElement(element.LevelId);            
            bool outputGroupMode = false;
            OutputItems = new List<OutputItem>();

            // Check if supported type and return error message to calling function if not
            this.ErrorMessage = "";
            this.Valid = true;
            if (!this.revitTypes.SimpleRevitOutputTypes.Contains(element.GetType())) {
                this.Valid = false;
                this.ErrorMessage = "Revit element type: '" + element.GetType().Name + "' not currently supported."; 
                return;
            }
            if (conversionMode == ConversionMode.FilledRegions && !this.revitTypes.SketchRevitOutputTypes.Contains(element.GetType())) {
                this.Valid = false;
                this.ErrorMessage = "Revit element type: '" + element.GetType().Name + "' cannot be output as a filled region."; 
                return;
            }
          
            //Reused local variables
            OutputItem outputItem = null; 
            LocationPoint locationPoint = null;
            LocationCurve locationCurve = null;
            XYZ pointStart = null;
            XYZ pointEnd = null;
            IList<XYZ> controlPointsNurb = null;
            RevitOutputType revitOutputType;
            RevitCurveType revitCurveType;

            // Start numbering
            this.rowId = rowIdRoot + "." + this.sequenceNumber.ToString();
            this.sequenceNumber++;

            // Set statements that apply to all elements. Don't include any sets with converted types
            if (conversionMode == ConversionMode.None) AddSetStatements("General");  

            // Intercept sketch types, as differentiated from simple types.
            if (this.revitTypes.SketchRevitOutputTypes.Contains(element.GetType())) {

                // Build the CurveArrArray as the same structure for all sketch types
                CurveArrArray curveArrArray= new CurveArrArray();
                if (element is Room) {
                    if (conversionMode != ConversionMode.None) {  // Don't draw any lines if just placing the room
                        outputGroupMode = true;
                        Room roomEvaluate = (Room)element;
                        //TODO need to set values for SpatialElementBoundaryOptions
                        SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
                        IList<IList<Autodesk.Revit.DB.BoundarySegment>> boundarySegmentsLoops = roomEvaluate.GetBoundarySegments(options);
                        foreach (List<Autodesk.Revit.DB.BoundarySegment> boundarySegmentsLoop in boundarySegmentsLoops) {
                            CurveArray curveArray = new CurveArray();
                            foreach (Autodesk.Revit.DB.BoundarySegment boundarySegment in boundarySegmentsLoop) {
                                // Deprecated 2016
                                //curveArray.Append(boundarySegment.Curve);
                                curveArray.Append(boundarySegment.GetCurve());
                            }
                            curveArrArray.Append(curveArray);
                        }
                    }
                }
                else if (element is Area) {
                    if (conversionMode != ConversionMode.None) {  // Don't draw any lines if just placing the area
                        outputGroupMode = true;
                        Area areaEvaluate = (Area)element;
                        //TODO need to set values for SpatialElementBoundaryOptions
                        SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions();
                        IList<IList<Autodesk.Revit.DB.BoundarySegment>> boundarySegmentsLoops = areaEvaluate.GetBoundarySegments(options);
                        foreach (List<Autodesk.Revit.DB.BoundarySegment> boundarySegmentsLoop in boundarySegmentsLoops) {
                            CurveArray curveArray = new CurveArray();
                            foreach (Autodesk.Revit.DB.BoundarySegment boundarySegment in boundarySegmentsLoop) {
                                // Deprecated 2016
                                //curveArray.Append(boundarySegment.Curve);
                                curveArray.Append(boundarySegment.GetCurve());
                            }
                            curveArrArray.Append(curveArray);
                        }
                    }
                }
                else if (element is FilledRegion) {
                    outputGroupMode = true;
                    FilledRegion filledRegion = (FilledRegion)element;
                    IList<CurveLoop> curveLoops = filledRegion.GetBoundaries();
                    foreach (CurveLoop curveLoop in curveLoops) {
                        CurveArray curveArray = new CurveArray();
                        foreach (Curve curve in curveLoop) {
                            curveArray.Append(curve);
                        }
                        curveArrArray.Append(curveArray);
                    }
                }
                else {    // Objects with a sketch like Wall or Floor
                    List<Sketch> sketches = this.elements.GetSketches(element);
                    if (sketches.Count > 0) {  // May be 0 if wall without sketch, for example
                        outputGroupMode = true;
                        // Note that if multiple sketches are found we are treating it as if it was one sketch with a CurveArrArray
                        // This is becasue of the way we need to create floors with separate openings instead of a single sketch 
                        // which would have occured if it had been created manually.
                        foreach (Sketch sketch in sketches) {
                            CurveArrArray curveArrArraySketch = sketch.Profile;
                            foreach (CurveArray curveArray in curveArrArraySketch) {
                                if (curveArray.Size > 1) curveArrArray.Append(curveArray);
                            }
                        }
                    }
                }

                // Set statements that need to be in advance of groups
                if (conversionMode == ConversionMode.None) {  // Don't include any sets with converted types

                    if (this.revitTypes.RevitOutputTypeDictionary.TryGetValue(element.GetType(), out revitOutputType)) {
                        switch (revitOutputType) {
                            case RevitOutputType.Wall:
                                Wall wallThis = (Wall)element;
                                this.wallTypeThis = wallThis.WallType;
                                this.wallHeightThis = this.elements.GetWallHeight(wallThis);
                                AddSetStatements("Wall");
                                break;
                            case RevitOutputType.Floor:
                                Floor floorThis = (Floor)element;
                                this.floorTypeThis = floorThis.FloorType;
                                AddSetStatements("Floor");
                                break;
                            case RevitOutputType.FilledRegion:
                                FilledRegion filledRegionThis = (FilledRegion)element;
                                this.filledRegionTypeThis = (FilledRegionType)this.settings.DocumentDb.GetElement(filledRegionThis.GetTypeId());
                                if (this.filledRegionTypeThis != null) AddSetStatements("FilledRegion");  // Null occurs with masking region; API doesn't support making these so we use first type
                                break;
                            default:
                                break;  // Ignore cases that don't apply
                        }
                    }
                }
                    
                // Process the CurveArrArray (which might have no CurveArrays in it if wall without sketch, for example.
                if (conversionMode == ConversionMode.FilledRegions) {
                    if (curveArrArray.Size == 0) {  // One of the allowed cases like wall but doesn't have an outline
                        this.Valid = false;
                        this.ErrorMessage = "Revit element type: '" + element.GetType().Name + "' cannot be output as a filled region.";
                        return;
                    }
                }
                foreach (CurveArray curveArray in curveArrArray) {
                    if (curveArray.Size > 1) {
                        if (conversionMode == ConversionMode.None || conversionMode == ConversionMode.FilledRegions) {
                            outputItem = new OutputItem(this.rowId, "Draw", "CurveArray");
                            OutputItems.Add(outputItem);
                            this.rowId = rowIdRoot + "." + this.sequenceNumber.ToString();
                            this.sequenceNumber++;
                        }
                    }
                    foreach (Curve curve in curveArray) {
                        if (this.revitTypes.RevitCurveTypeDictionary.TryGetValue(curve.GetType(), out revitCurveType)) {
                            switch (revitCurveType) {
                                case RevitCurveType.Line:
                                    Line line = (Line)curve;
                                    if (conversionMode == ConversionMode.None || conversionMode == ConversionMode.FilledRegions) outputItem = new OutputItem(this.rowId, "Draw", "Line");
                                    else if (conversionMode == ConversionMode.DetailLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailLine");
                                    else outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelLine");
                                    outputItem.Value01 = new OutputValue(line.GetEndPoint(0), this.precision);
                                    outputItem.Value02 = new OutputValue(line.GetEndPoint(1), this.precision);
                                    break;
                                case RevitCurveType.Arc:
                                    Arc arc = (Arc)curve;
                                    if (conversionMode == ConversionMode.None || conversionMode == ConversionMode.FilledRegions) outputItem = new OutputItem(this.rowId, "Draw", "Arc");
                                    else if (conversionMode == ConversionMode.DetailLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailArc");
                                    else outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelArc");
                                    outputItem.Value01 = new OutputValue(arc.GetEndPoint(0), this.precision);
                                    outputItem.Value02 = new OutputValue(arc.GetEndPoint(1), this.precision);
                                    outputItem.Value03 = new OutputValue(arc.Evaluate(0.5, true), this.precision);  // arc.Evaluate(0.5, true) finds the midpoint
                                    break;
                                case RevitCurveType.HermiteSpline:   // we treat as Nurbs.  Not sure what this means???
                                    // TODO This could be problematic since a hermite spline goes through the points (I think)
                                    // I think I recall tha these arose from the sketch of a floor or wall.
                                    HermiteSpline hermiteSpline = (HermiteSpline)curve;
                                    IList<XYZ> controlPointsHermite = hermiteSpline.ControlPoints;
                                    // TODO not handling case of 3 or 4 points in the same line
                                    OutputUsePoints(outputItem, controlPointsHermite);
                                    if (conversionMode == ConversionMode.None || conversionMode == ConversionMode.FilledRegions) outputItem = new OutputItem(this.rowId, "Draw", "NurbSpline");
                                    else if (conversionMode == ConversionMode.DetailLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailNurbSpline");
                                    else outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelNurbSpline");
                                    break;
                                case RevitCurveType.NurbSpline:
                                    NurbSpline nurbSpline = (NurbSpline)curve;
                                    controlPointsNurb = nurbSpline.CtrlPoints;
                                    // TODO not handling case of 3 or 4 points in the same line
                                    OutputUsePoints(outputItem, controlPointsNurb);
                                    if (conversionMode == ConversionMode.None || conversionMode == ConversionMode.FilledRegions) outputItem = new OutputItem(this.rowId, "Draw", "NurbSpline");
                                    else if (conversionMode == ConversionMode.DetailLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailNurbSpline");
                                    else outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelNurbSpline");
                                    break;
                                case RevitCurveType.Ellipse:
                                    Ellipse ellipse = (Ellipse)curve;
// TODO not handling ellipse

                                    break;
                                default:
                                    System.Windows.Forms.MessageBox.Show("Programming Error: Unknown case in 'OutputGroup' constructor.", this.settings.ProgramName);
                                    break;
                            }
                            OutputItems.Add(outputItem);
                            this.rowId = rowIdRoot + "." + this.sequenceNumber.ToString();
                            this.sequenceNumber++;
                        }
                        else {
                            //TODO some problem occured here
                        }
                    }
                }
                if (conversionMode == ConversionMode.FilledRegions) {
                    if (curveArrArray != null) outputItem = new OutputItem(this.rowId, element.Id, "Add", "FilledRegion");
                    OutputItems.Add(outputItem);
                    this.rowId = rowIdRoot + "." + this.sequenceNumber.ToString();
                    this.sequenceNumber++;
                    //TODO Some question here if the CurveArrArray is not valid for a filled region then what happens?
                }
            }

            // Intercept NurbSpline types, as differentiated from simple types and sketch types.
            // We don't want to allow both Sketch and NurbSpline on the same element
            else if (this.revitTypes.NurbSplineRevitOutputTypes.Contains(element.GetType())) {

                outputGroupMode = false;  // Note, a nurbs set is not considered a group.

                controlPointsNurb = null;
                if (element is DetailNurbSpline) {
                    DetailNurbSpline detailNurbSpline = (DetailNurbSpline)element;
                    NurbSpline nurbSplineDetail = (NurbSpline)detailNurbSpline.GeometryCurve;
                    controlPointsNurb = nurbSplineDetail.CtrlPoints;
                }
                else {  // ModelNurbSpline case
                    ModelNurbSpline modelNurbSpline = (ModelNurbSpline)element;
                    NurbSpline nurbSplineModel = (NurbSpline)modelNurbSpline.GeometryCurve;
                    controlPointsNurb = nurbSplineModel.CtrlPoints;
                }
            }

            if (!outputGroupMode || conversionMode == ConversionMode.None) {

                if (this.revitTypes.RevitOutputTypeDictionary.TryGetValue(element.GetType(), out revitOutputType)) {
                    switch (revitOutputType) {
                        //switch (element.GetType().Name) {
                        case RevitOutputType.Grid:
                            Grid grid = (Grid)element;
                            Curve curveGrid = grid.Curve;
                            if (conversionMode == ConversionMode.None) outputItem = new OutputItem(this.rowId, element.Id, "Add", "Grid");
                            else if (conversionMode == ConversionMode.DetailLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailLine");
                            else outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelLine");
                            // 2014 update
                            outputItem.Value01 = new OutputValue(curveGrid.GetEndPoint(0), this.precision);
                            outputItem.Value02 = new OutputValue(curveGrid.GetEndPoint(1), this.precision);
                            if (curveGrid is Arc) {
                                outputItem.Value03 = new OutputValue(curveGrid.Evaluate(0.5, true), this.precision);  // arc.Evaluate(0.5, true) finds the midpoint
                            }
                            break;
                        case RevitOutputType.DetailLine:
                            DetailLine detailLine = (DetailLine)element;
                            Curve curveDetailLine = detailLine.GeometryCurve;
                            if (conversionMode != ConversionMode.ModelLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailLine");
                            else outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelLine");
                            outputItem.Value01 = new OutputValue(curveDetailLine.GetEndPoint(0), this.precision);
                            outputItem.Value02 = new OutputValue(curveDetailLine.GetEndPoint(1), this.precision);
                            break;
                        case RevitOutputType.DetailArc:
                            DetailArc detailArc = (DetailArc)element;
                            Curve curveDetailArc = detailArc.GeometryCurve;
                            if (conversionMode != ConversionMode.ModelLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailArc");
                            else outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelArc");
                            outputItem.Value01 = new OutputValue(curveDetailArc.GetEndPoint(0), this.precision);
                            outputItem.Value02 = new OutputValue(curveDetailArc.GetEndPoint(1), this.precision);
                            outputItem.Value03 = new OutputValue(curveDetailArc.Evaluate(0.5, true), this.precision);  // arc.Evaluate(0.5, true) finds the midpoint
                            break;
                        case RevitOutputType.DetailNurbSpline:
                            if (!outputGroupMode) {
                                int totalPoints = controlPointsNurb.Count;
                                int linesCount = totalPoints / 4;
                                if (linesCount * 4 < totalPoints) linesCount++;
                                // TODO not handling case of 3 or 4 points in the same line
                                // TODO Convert to  OutputUsePoints(outputItem, ... form?
                                for (int i = 0; i < linesCount; i++) {
                                    outputItem = new OutputItem(this.rowId, element.Id, "Use", "Points");
                                    int index = i * 4;
                                    outputItem.Value01 = new OutputValue(controlPointsNurb[index], this.precision); // Always get first point
                                    index++;
                                    if (index < totalPoints) outputItem.Value02 = new OutputValue(controlPointsNurb[index], this.precision);
                                    index++;
                                    if (index < totalPoints) outputItem.Value03 = new OutputValue(controlPointsNurb[index], this.precision);
                                    index++;
                                    if (index < totalPoints) outputItem.Value04 = new OutputValue(controlPointsNurb[index], this.precision);
                                    OutputItems.Add(outputItem);
                                    this.rowId = rowIdRoot + "." + this.sequenceNumber.ToString();
                                    this.sequenceNumber++;
                                }
                            }
                            if (conversionMode != ConversionMode.ModelLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailNurbSpline");
                            else outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelNurbSpline");
                            break;
                        case RevitOutputType.ModelLine:
                            ModelLine modelLine = (ModelLine)element;
                            Curve curveModelLine = modelLine.GeometryCurve;
                            if (conversionMode != ConversionMode.DetailLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelLine");
                            else outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailLine");
                            outputItem.Value01 = new OutputValue(curveModelLine.GetEndPoint(0), this.precision);
                            outputItem.Value02 = new OutputValue(curveModelLine.GetEndPoint(1), this.precision);
                            break;
                        case RevitOutputType.ModelArc:
                            ModelArc modelArc = (ModelArc)element;
                            Curve curveModelArc = modelArc.GeometryCurve;
                            if (conversionMode != ConversionMode.DetailLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelArc");
                            else outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailArc");
                            outputItem.Value01 = new OutputValue(curveModelArc.GetEndPoint(0), this.precision);
                            outputItem.Value02 = new OutputValue(curveModelArc.GetEndPoint(1), this.precision);
                            outputItem.Value03 = new OutputValue(curveModelArc.Evaluate(0.5, true), this.precision);  // arc.Evaluate(0.5, true) finds the midpoint
                            break;
                        case RevitOutputType.ModelNurbSpline:
                            if (!outputGroupMode) {
                                int totalPoints = controlPointsNurb.Count;
                                int linesCount = totalPoints / 4;
                                if (linesCount * 4 < totalPoints) linesCount++;
                                //TODO not handling case of 3 or 4 points in the same line
                                // TODO Convert to  OutputUsePoints(outputItem, ... form?
                                for (int i = 0; i < linesCount; i++) {
                                    outputItem = new OutputItem(this.rowId, element.Id, "Use", "Points");
                                    int index = i * 4;
                                    outputItem.Value01 = new OutputValue(controlPointsNurb[index], this.precision); // Always get first point
                                    index++;
                                    if (index < totalPoints) outputItem.Value02 = new OutputValue(controlPointsNurb[index], this.precision);
                                    index++;
                                    if (index < totalPoints) outputItem.Value03 = new OutputValue(controlPointsNurb[index], this.precision);
                                    index++;
                                    if (index < totalPoints) outputItem.Value04 = new OutputValue(controlPointsNurb[index], this.precision);
                                    OutputItems.Add(outputItem);
                                    this.rowId = rowIdRoot + "." + this.sequenceNumber.ToString();
                                    this.sequenceNumber++;
                                }
                            }
                            if (conversionMode != ConversionMode.DetailLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelNurbSpline");
                            else outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailNurbSpline");
                            break;
                        case RevitOutputType.Wall:
                            if (outputGroupMode) {
                                outputItem = new OutputItem(this.rowId, element.Id, "Add", "Wall");
                                //outputItem.Value01 = new OutputValue("DrawSet");
                            }
                            else {
                                Wall wall = (Wall)element;
                                locationCurve = (LocationCurve)wall.Location;
                                this.wallTypeThis = wall.WallType;
                                this.wallHeightThis = this.elements.GetWallHeight(wall);
                                if (conversionMode == ConversionMode.None) AddSetStatements("Wall");

                                // No true conversion options available at this time but this is a simple alternative
                                if (conversionMode == ConversionMode.None) outputItem = new OutputItem(this.rowId, element.Id, "Add", "Wall");
                                else if (conversionMode == ConversionMode.DetailLines) outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailLine");
                                else outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelLine");
                                outputItem.Value01 = new OutputValue(locationCurve.Curve.GetEndPoint(0), this.precision);
                                outputItem.Value02 = new OutputValue(locationCurve.Curve.GetEndPoint(1), this.precision);
                                if (locationCurve.Curve is Arc) {
                                    outputItem.Value03 = new OutputValue(locationCurve.Curve.Evaluate(0.5, true), this.precision);  // arc.Evaluate(0.5, true) finds the midpoint
                                }
                            }
                            break;
                        case RevitOutputType.Floor:
                            if (outputGroupMode) {
                                outputItem = new OutputItem(this.rowId, element.Id, "Add", "Floor");
                            }
                            //else - No simple mode option, we assume floor has a profile which also served for conversions. 
                            break;
                        case RevitOutputType.FilledRegion:
                            if (outputGroupMode) {
                                outputItem = new OutputItem(this.rowId, element.Id, "Add", "FilledRegion");
                            }
                            //else - No simple mode option, we assume filled region has boundary loops which also served for conversions. 
                            break;
                        case RevitOutputType.Room:
                            if (outputGroupMode) {
                                // Note: Group mode doesn't really make sense here.  We could do something like draw Room Separation Lines but that sort of breaks the pattern.
                                // Presumably if user is outputting to Rhino Input they will use model or drafting line conversion and not insertion point
                            }
                            Room room = (Room)element;
                            outputItem = new OutputItem(this.rowId, element.Id, "Add", "Room");
                            LocationPoint roomLocationPoint = (LocationPoint)room.Location;
                            outputItem.Value01 = new OutputValue(roomLocationPoint.Point, this.precision);                            
                            break;
                        case RevitOutputType.Area:
                            if (outputGroupMode) {
                                // Note: Group mode doesn't really make sense here.  We could do something like draw Area Boundary Lines but that sort of breaks the pattern.
                                // Presumably if user is outputting to Rhino Input they will use model or drafting line conversion and not insertion point
                            }
                            Area area = (Area)element;
                            outputItem = new OutputItem(this.rowId, element.Id, "Add", "Area");
                            LocationPoint areaLocationPoint = (LocationPoint)area.Location;
                            outputItem.Value01 = new OutputValue(areaLocationPoint.Point, this.precision);
                            break;
                        case RevitOutputType.FamilyInstance:
                            FamilyInstance familyInstance = (FamilyInstance)element;
                            FamilySymbol familySymbol = familyInstance.Symbol;
                            Family family = familySymbol.Family;

                            // determine which of the family instance cases (If any) that we have
                            //string caseFamilyInstance = "";
                            FamilyInstanceCondition familyInstanceCondition = FamilyInstanceCondition.NotSet;
                            if (this.elements.ColumnArchitecturalTypes.ContainsKey(family.Name)) {
                                if (this.elements.ColumnArchitecturalTypes[family.Name].ContainsKey(familySymbol.Name)) {
                                    familyInstanceCondition = FamilyInstanceCondition.Column;
                                    this.columnModeThis = "Architectural";
                                    this.columnFamilyThis = family.Name;
                                    this.columnTypeThis = familySymbol.Name;  // Note that symbol name is actually the type name
                                }
                            }
                            else {
                                if (this.elements.ColumnStructuralTypes.ContainsKey(family.Name)) {
                                    if (this.elements.ColumnStructuralTypes[family.Name].ContainsKey(familySymbol.Name)) {
                                        familyInstanceCondition = FamilyInstanceCondition.Column;
                                        this.columnFamilyThis = family.Name;
                                        this.columnTypeThis = familySymbol.Name;  // Note that symbol name is actually the type name
                                        if (!familyInstance.IsSlantedColumn) this.columnModeThis = "StructuralVertical";
                                        else {
                                            Parameter parameterColumnSlantType = familyInstance.get_Parameter(BuiltInParameter.SLANTED_COLUMN_TYPE_PARAM);
                                            if (parameterColumnSlantType.AsInteger() == Convert.ToInt32(SlantedOrVerticalColumnType.CT_EndPoint)) this.columnModeThis = "StructuralPointDriven";
                                            else this.columnModeThis = "StructuralAngleDriven";
                                        }
                                    }
                                }
                            }
                            if (familyInstanceCondition == FamilyInstanceCondition.NotSet) {  // Didn't find one of the column cases
                                if (this.elements.BeamTypes.ContainsKey(family.Name)) {
                                    if (this.elements.BeamTypes[family.Name].ContainsKey(familySymbol.Name)) {
                                        familyInstanceCondition = FamilyInstanceCondition.Beam;
                                        this.beamFamilyThis = family.Name;
                                        this.beamTypeThis = familySymbol.Name;  // Note that symbol name is actually the type name
                                    }
                                }
                            }
                            if (familyInstanceCondition == FamilyInstanceCondition.NotSet) {  // Didn't find one of the column cases or beam
                                // ?? Could be problematic if it is both a column and an adaptive component?  (Not sure if that is possible.)
                                if (this.elements.AdaptiveComponentTypes.ContainsKey(family.Name)) {
                                    if (this.elements.AdaptiveComponentTypes[family.Name].ContainsKey(familySymbol.Name)) {
                                        familyInstanceCondition = FamilyInstanceCondition.AdaptiveComponent;
                                        this.adaptiveComponentFamilyThis = family.Name;
                                        this.adaptiveComponentTypeThis = familySymbol.Name;  // Note that symbol name is actually the type name
                                    }
                                }
                            }
                            if (familyInstanceCondition == FamilyInstanceCondition.NotSet) {  // Didn't find any case that made sense so report an error
                                this.Valid = false;
                                this.ErrorMessage = "Family instance: '" + family.Name + "' not currently supported."; 
                                return;
                            }

                            // Get common values
                            if (familyInstance.Location is LocationPoint) {
                                locationPoint = (LocationPoint)familyInstance.Location;
                                pointStart = locationPoint.Point;
                            }
                            else if (familyInstance.Location is LocationCurve) {
                                locationCurve = (LocationCurve)familyInstance.Location;
                                pointStart = locationCurve.Curve.GetEndPoint(0);
                                pointEnd = locationCurve.Curve.GetEndPoint(1);
                            }

                            // Handle cases
                            switch (familyInstanceCondition) {
                                case FamilyInstanceCondition.Column:
                                    if (conversionMode == ConversionMode.None) {
                                        this.columnRotationThis = this.elements.GetFramingRotation(familyInstance, locationPoint, "Column");
                                        if (this.columnModeThis == "Architectural" || this.columnModeThis == "StructuralVertical") {
                                            this.columnHeightThis = this.elements.GetFamilyTop(familyInstance);
                                        }
                                        AddSetStatements("Column");
                                        outputItem = new OutputItem(this.rowId, element.Id, "Add", "Column");
                                        outputItem.Value01 = new OutputValue(pointStart, this.precision);
                                        if (!(this.columnModeThis == "Architectural" || this.columnModeThis == "StructuralVertical")) {
                                            outputItem.Value02 = new OutputValue(pointEnd, this.precision);
                                        }
                                    }
                                    else {
                                        if (conversionMode == ConversionMode.DetailLines) {
                                            // don't do anything with detail lines.  Output would be a point.
                                        }
                                        else {  // ModelLine case
                                            outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelLine");
                                            outputItem.Value01 = new OutputValue(pointStart, this.precision);
                                            if (this.columnModeThis == "Architectural" || this.columnModeThis == "StructuralVertical") {
                                                pointEnd = new XYZ(pointStart.X, pointStart.Y, pointStart.Z + this.columnHeightThis);
                                            }
                                            outputItem.Value02 = new OutputValue(pointEnd, this.precision);
                                        }
                                    }
                                    break;
                                case FamilyInstanceCondition.Beam:
                                    if (conversionMode == ConversionMode.None) {
                                        this.beamRotationThis = this.elements.GetFramingRotation(familyInstance, locationPoint, "Beam");
                                        this.beamJustificationThis = this.elements.GetBeamJustification(familyInstance);
                                        AddSetStatements("Beam");
                                        outputItem = new OutputItem(this.rowId, element.Id, "Add", "Beam");

                                    }
                                    else {
                                        if (conversionMode == ConversionMode.DetailLines) {
                                            outputItem = new OutputItem(this.rowId, element.Id, "Add", "DetailLine");
                                        }
                                        else {  // ModelLine case
                                            outputItem = new OutputItem(this.rowId, element.Id, "Add", "ModelLine");
                                        }
                                    }
                                    // TODO really should flatten the Z with detail lines but ignore for now.
                                    outputItem.Value01 = new OutputValue(pointStart, this.precision);
                                    outputItem.Value02 = new OutputValue(pointEnd, this.precision);
                                    break;
                                case FamilyInstanceCondition.AdaptiveComponent:
                                    if (conversionMode == ConversionMode.None) {  // Ignore conversion to model or detail lines
                                        AddSetStatements("AdaptiveComponent");
                                        List<FamilyPointPlacementReference> familyPoints = (List<FamilyPointPlacementReference>)familyInstance.GetFamilyPointPlacementReferences();
                                        List<XYZ> listPoints = MakePointsList(familyPoints);
                                        int countPoints = listPoints.Count;
                                        if (countPoints == 0) break;  // Not sure why this occurs; seems to be an instance of the nested family?
                                        if (countPoints <= 4) {
                                            outputItem = new OutputItem(this.rowId, element.Id, "Add", "AdaptiveComponent");
                                            outputItem.Value01 = new OutputValue(listPoints[0], this.precision);
                                            outputItem.Value02 = new OutputValue(listPoints[1], this.precision);
                                            if (countPoints >= 3) {
                                                outputItem.Value03 = new OutputValue(listPoints[2], this.precision);
                                            }
                                            if (countPoints == 4) {
                                                outputItem.Value04 = new OutputValue(listPoints[3], this.precision);
                                            }
                                        }
                                        else {
                                            OutputUsePoints(outputItem, listPoints);
                                            outputItem = new OutputItem(this.rowId, element.Id, "Add", "AdaptiveComponent");
                                        }
                                    }
                                    break;
                                default:     // Ignore unknown cases. outputItem will be null and not get processed
                                    break;
                            }
                            break;
                        case RevitOutputType.ReferencePoint:
                            if (conversionMode == ConversionMode.None) {  // Ignore conversion to model or detail lines
                                ReferencePoint referencePoint = (ReferencePoint)element;
                                CurveByPointsArray testForInterpolatingCurve = referencePoint.GetInterpolatingCurves();
                                if (!testForInterpolatingCurve.IsEmpty) break;  // If it is in a curve then we will get the points separately when we process the curve
                                outputItem = new OutputItem(this.rowId, element.Id, "Add", "ReferencePoint");
                                outputItem.Value01 = new OutputValue(referencePoint.Position, this.precision);
                            }
                            break;
                        case RevitOutputType.CurveByPoints:
                            if (conversionMode == ConversionMode.None) {  // Ignore conversion to model or detail lines
                                CurveByPoints curveByPoints = (CurveByPoints)element;
                                List<XYZ> listPoints = MakePointsList(curveByPoints.GetPoints());
                                int countPoints = listPoints.Count;
                                if (countPoints <= 4) {
                                    outputItem = new OutputItem(this.rowId, element.Id, "Add", "CurveByPoints");
                                    outputItem.Value01 = new OutputValue(listPoints[0], this.precision);
                                    outputItem.Value02 = new OutputValue(listPoints[1], this.precision);
                                    if (countPoints >= 3) {
                                        outputItem.Value03 = new OutputValue(listPoints[2], this.precision);
                                    }
                                    if (countPoints == 4) {
                                        outputItem.Value04 = new OutputValue(listPoints[3], this.precision);
                                    }
                                }
                                else {
                                    OutputUsePoints(outputItem, listPoints);
                                    outputItem = new OutputItem(this.rowId, element.Id, "Add", "CurveByPoints");
                                }

                            }
                            break;
                        case RevitOutputType.Form:
                            if (conversionMode == ConversionMode.None) {  // Ignore conversion to model or detail lines
                                Form form = (Form)element;
                                int countProfiles = form.ProfileCount;
                                //PathCurveCount = 0 and form.get_PathCurveReference causes a bad error

                                for (int i = 0; i < countProfiles; i++) {
                                    //int countCurveLoop = form.get_ProfileCurveLoopCount(i);
                                    ReferenceArray referenceArrayProfiles = form.get_CurveLoopReferencesOnProfile(i, 0);
                                    Reference reference = referenceArrayProfiles.get_Item(0);
                                    List<XYZ> listPoints = MakePointsList(form.GetControlPoints(reference));
                                    outputItem = new OutputItem(this.rowId, "Model", "ReferenceArray");
                                    // TO DO allow short form of reference array with 4 or fewer points??
                                    OutputItems.Add(outputItem);
                                    this.rowId = rowIdRoot + "." + this.sequenceNumber.ToString();
                                    this.sequenceNumber++;
                                    OutputUsePoints(outputItem, listPoints);
                                }
                                outputItem = new OutputItem(this.rowId, element.Id, "Add", "LoftForm");
                            }
                            break;
                        default:
                            break;
                    }
                }
                else {
                    //TODO some problem occured here
                }
                if (outputItem != null) {
                    OutputItems.Add(outputItem);
                    this.rowId = rowIdRoot + "." + this.sequenceNumber.ToString();
                    this.sequenceNumber++;
                }
            }
            
        }

        #endregion

        #region Public Functions                        // ****************************** Pubic Functions *******************************************

        // Implementation of IEnumerable and IEnumerator
        public IEnumerator GetEnumerator() { // Needed since Implementing IEnumerable 
            return (IEnumerator)this;
        }
        public bool MoveNext() { // Needed since Implementing IEnumerator
            if (OutputItems == null) return false;
            if (this.position >= OutputItems.Count - 1) return false;
            this.position++;
            return true;
        }
        public void Reset() {
            this.position = -1;
        }
        public object Current {
            get {
                if (OutputItems == null) return null;
                return OutputItems[this.position];
            }
        }

        #endregion

        #region Private Functions                       // ****************************** Private Functions *****************************************
        private void AddSetStatements(string type) {
            switch (type) {
                case "General":
//TODO Logic seems incorrect.  See Filled Region
                    if (this.elements.LevelCurrent == null || this.levelThis == null) {  // Occurs when running in 3D view??
                        this.elements.LevelCurrent = this.levelThis;
                    }
                    else {
                        if (this.elements.LevelCurrent.Name != this.levelThis.Name) {
                            OutputItem outputItem = new OutputItem(this.rowId, "Set", "Level");
                            outputItem.Value01 = new OutputValue(this.levelThis.Name);
                            OutputItems.Add(outputItem);
                            this.elements.LevelCurrent = this.levelThis;
                            this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                            this.sequenceNumber++;
                        }
                    }
                    break;

                case "Wall":
//TODO Logic seems incorrect.  See Filled Region
                    if (this.elements.WallTypeCurrent == null || this.wallTypeThis == null) {  // Occurs when running in 3D view??
                        this.elements.WallTypeCurrent = this.wallTypeThis;
                    }
                    else {
                        if (this.elements.WallTypeCurrent.Name != this.wallTypeThis.Name) {
                            OutputItem outputItem = new OutputItem(this.rowId, "Set", "WallType");
                            outputItem.Value01 = new OutputValue(this.wallTypeThis.Name);
                            OutputItems.Add(outputItem);
                            this.elements.WallTypeCurrent = this.wallTypeThis;
                            this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                            this.sequenceNumber++;
                        }
                    }
                    if (this.elements.WallHeightCurrent != this.wallHeightThis) {
                        OutputItem outputItem = new OutputItem(this.rowId, "Set", "WallHeight");
                        outputItem.Value01 = new OutputValue(this.wallHeightThis, this.precision);
                        OutputItems.Add(outputItem);
                        this.elements.WallHeightCurrent = this.wallHeightThis;
                        this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                        this.sequenceNumber++;
                    }
                    break;
                case "Floor":
//TODO Logic seems incorrect.  See Filled Region
                    // Note we had problem with matching floor types failing if (this.elements.FloorTypeCurrent != this.floorTypeThis)
                    // this problem doesn't seem to occur with wall types??
                    if (this.elements.FloorTypeCurrent == null || this.floorTypeThis == null) {  // Occurs when running in 3D view??
                        this.elements.FloorTypeCurrent = this.floorTypeThis;
                    }
                    else {
                        bool newFloorType = false;
                        if (this.elements.FloorTypeCurrent == null) newFloorType = true;
                        else if (this.elements.FloorTypeCurrent.Name != this.floorTypeThis.Name) newFloorType = true;
                        if (newFloorType) {
                            OutputItem outputItem = new OutputItem(this.rowId, "Set", "FloorType");
                            outputItem.Value01 = new OutputValue(this.floorTypeThis.Name);
                            OutputItems.Add(outputItem);
                            this.elements.FloorTypeCurrent = this.floorTypeThis;
                            this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                            this.sequenceNumber++;
                        }
                    }
                    break;
                case "FilledRegion":
                    //if (this.elements.FilledRegionTypeCurrent == null || this.filledRegionTypeThis == null) {  
                    //    this.elements.FilledRegionTypeCurrent = this.filledRegionTypeThis;
                    //}
                    bool newFilledRegionType = false;
                    if (this.elements.FilledRegionTypeCurrent == null) newFilledRegionType = true;
                    else if (this.elements.FilledRegionTypeCurrent.Name != this.filledRegionTypeThis.Name) newFilledRegionType = true;
                    if (newFilledRegionType) {
                        OutputItem outputItem = new OutputItem(this.rowId, "Set", "FilledRegionType");
                        outputItem.Value01 = new OutputValue(this.filledRegionTypeThis.Name);
                        OutputItems.Add(outputItem);
                        this.elements.FilledRegionTypeCurrent = this.filledRegionTypeThis;
                        this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                        this.sequenceNumber++;
                    }
                    break;
                case "Column":
                    if ((this.elements.ColumnModeCurrent != this.columnModeThis)    //"Architectural" "StructuralVertical"  "StructuralPointDriven",  "StructuralAngleDriven";
                    ||  (this.elements.ColumnFamilyCurrent != this.columnFamilyThis)
                    ||  (this.elements.ColumnTypeCurrent != this.columnTypeThis)) {
                        OutputItem outputItem = new OutputItem(this.rowId, "Set", "ColumnMode");
                        outputItem.Value01 = new OutputValue(this.columnModeThis);
                        outputItem.Value02 = new OutputValue(this.columnFamilyThis);
                        outputItem.Value03 = new OutputValue(this.columnTypeThis);
                        OutputItems.Add(outputItem);
                        this.elements.ColumnModeCurrent = this.columnModeThis;
                        this.elements.ColumnFamilyCurrent = this.columnFamilyThis;
                        this.elements.ColumnTypeCurrent = this.columnTypeThis;
                        this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                        this.sequenceNumber++;
                    }

//???? Might need to use tolerance with these equal statements????
                    if (this.columnModeThis == "Architectural" || this.columnModeThis == "StructuralVertical") {
                        if (this.elements.ColumnHeightCurrent != this.columnHeightThis) {
                            OutputItem outputItem = new OutputItem(this.rowId, "Set", "ColumnHeight");
                            outputItem.Value01 = new OutputValue(this.columnHeightThis, this.precision);
                            OutputItems.Add(outputItem);
                            this.elements.ColumnHeightCurrent = this.columnHeightThis;
                            this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                            this.sequenceNumber++;
                        }
                    }
                    if (this.elements.ColumnRotationCurrent != this.columnRotationThis) {
                        OutputItem outputItem = new OutputItem(this.rowId, "Set", "ColumnRotation");
                        outputItem.Value01 = new OutputValue(this.columnRotationThis, this.precision);
                        OutputItems.Add(outputItem);
                        this.elements.ColumnRotationCurrent = this.columnRotationThis;
                        this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                        this.sequenceNumber++;
                    }
                    break;
                case "Beam":
                    if (this.elements.BeamFamilyCurrent != this.beamFamilyThis || this.elements.BeamTypeCurrent != this.beamTypeThis) {
                        OutputItem outputItem = new OutputItem(this.rowId, "Set", "BeamType");
                        outputItem.Value01 = new OutputValue(this.beamFamilyThis);
                        outputItem.Value02 = new OutputValue(this.beamTypeThis);
                        OutputItems.Add(outputItem);
                        this.elements.BeamFamilyCurrent = this.beamFamilyThis;
                        this.elements.BeamTypeCurrent = this.beamTypeThis;
                        this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                        this.sequenceNumber++;
                    }
                    if (this.elements.BeamRotationCurrent != this.beamRotationThis) {
                        OutputItem outputItem = new OutputItem(this.rowId, "Set", "BeamRotation");
                        outputItem.Value01 = new OutputValue(this.beamRotationThis, this.precision);
                        OutputItems.Add(outputItem);
                        this.elements.BeamRotationCurrent = this.beamRotationThis;
                        this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                        this.sequenceNumber++;
                    }
                    if (this.elements.BeamJustificationCurrent != this.beamJustificationThis) {
                        OutputItem outputItem = new OutputItem(this.rowId, "Set", "BeamJustification");
                        outputItem.Value01 = new OutputValue(this.beamJustificationThis);
                        OutputItems.Add(outputItem);
                        this.elements.BeamJustificationCurrent = this.beamJustificationThis;
                        this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                        this.sequenceNumber++;
                    }
                    break;
                case "AdaptiveComponent":
                    if (this.elements.AdaptiveComponentFamilyCurrent != this.adaptiveComponentFamilyThis
                        || this.elements.AdaptiveComponentTypeCurrent != this.adaptiveComponentTypeThis) {
                        OutputItem outputItem = new OutputItem(this.rowId, "Set", "AdaptiveComponentType");
                        outputItem.Value01 = new OutputValue(this.adaptiveComponentFamilyThis);
                        outputItem.Value02 = new OutputValue(this.adaptiveComponentTypeThis);
                        OutputItems.Add(outputItem);
                        this.elements.AdaptiveComponentFamilyCurrent = this.adaptiveComponentFamilyThis;
                        this.elements.AdaptiveComponentTypeCurrent = this.adaptiveComponentTypeThis;
                        this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                        this.sequenceNumber++;
                    }
                    break;
                default:
                    System.Windows.Forms.MessageBox.Show("Programming Error: Unknown case in 'AddSetStatements()'.", this.settings.ProgramName);
                    break;
            }
        }

        #endregion

        #region Utility Functions                       // ****************************** Utility Functions *****************************************

        private List<XYZ> MakePointsList (ReferencePointArray referencePointArray) {
            List<XYZ> pointsList = new List<XYZ>();
            for (int i = 0; i < referencePointArray.Size; i++) {
                pointsList.Add(referencePointArray.get_Item(i).Position);
            }
            return pointsList;
        }
        private List<XYZ> MakePointsList(ReferenceArray referenceArray) {
            List<XYZ> pointsList = new List<XYZ>();
            for (int i = 0; i < referenceArray.Size; i++) {
                try {
                    Reference reference = referenceArray.get_Item(i);
                    GeometryObject geometryObject = Element.GetGeometryObjectFromReference(reference);
                    Point point = (Point)geometryObject;
                    pointsList.Add(point.Coord);
                }
                catch { }
            }
            return pointsList;
        }
        private List<XYZ> MakePointsList(List<FamilyPointPlacementReference> listPoints) {
            List<XYZ> pointsList = new List<XYZ>();
            for (int i = 0; i < listPoints.Count; i++) {
                try {
                    FamilyPointPlacementReference reference = listPoints[i];                    
                    pointsList.Add(reference.Location.Origin);
                }
                catch { }
            }
            return pointsList;
        }
        
        private void OutputUsePoints(OutputItem outputItem, IList<XYZ> controlPoints) {
            // Detrmine how many NurbsPoints lines are needed in NurbsPointsSet
            // Note: We allow a single NurbsPoints line with 1, 2, 3 or 4 points
            //       If more than that requires a preceeding NurbsPointsSet with count of folowing NurbsPoints lines
            int numberNurbsPoints = controlPoints.Count;
            int numberNurbsPointSets = 1;
            if (controlPoints.Count > 4) {
                numberNurbsPointSets = (numberNurbsPoints / 4);   // This will round down
                if ((numberNurbsPointSets * 4) < numberNurbsPoints) numberNurbsPointSets++;  // Adds an extra NurbsPoints line if needed
            }
            for (int j = 0; j < numberNurbsPointSets; j++) {
                outputItem = new OutputItem(this.rowId, "Use", "Points");
                int indexPoint = 4 * j;
                if (indexPoint < numberNurbsPoints) outputItem.Value01 = new OutputValue(controlPoints.ElementAt(indexPoint), this.precision);
                indexPoint++;
                if (indexPoint < numberNurbsPoints) outputItem.Value02 = new OutputValue(controlPoints.ElementAt(indexPoint), this.precision);
                indexPoint++;
                if (indexPoint < numberNurbsPoints) outputItem.Value03 = new OutputValue(controlPoints.ElementAt(indexPoint), this.precision);
                indexPoint++;
                if (indexPoint < numberNurbsPoints) outputItem.Value04 = new OutputValue(controlPoints.ElementAt(indexPoint), this.precision);
                OutputItems.Add(outputItem);
                this.rowId = this.rowIdRoot + "." + this.sequenceNumber.ToString();
                this.sequenceNumber++;
            }         
        }

        #endregion
    }
}
