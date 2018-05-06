using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Windows.Forms;

using System.Globalization;

using System.Data;

namespace HummingbirdUtility {
    class InputParser {

        public DataTable dataTable { set; get; }
        public List<HbInputItem> InputItems { set; get; }

        private string lastAction;
        private string lastObject;

        private NumberFormatInfo provider = new NumberFormatInfo();

        // ******************************************************* Constructor *****************************************************************
        public InputParser(DataTable dataTable) {
            this.dataTable = dataTable;
            this.InputItems = new List<HbInputItem>();
            this.lastAction = "";
            this.lastObject = "";

            this.provider.NumberDecimalSeparator = ".";
        }

        // **************************************************************** Public Functions *********************************************************************
        public string GetInputItems() {            
            int dataTableIndex = 0;
            while (true) {
                HbInputItem inputItem;
                string returnValue = GetNextInputItem(ref dataTableIndex, out inputItem);
                if (returnValue == "") {
                    this.InputItems.Add(inputItem);
                }
                else {
                    if (returnValue == "EOF") break;
                    else return returnValue;
                }                       
            }
            if (dataTableIndex < this.dataTable.Rows.Count - 1) return "Input terminated at row: " + dataTableIndex;
            return "";
        }

        private string GetNextInputItem(ref int dataTableIndex, out HbInputItem inputItem) {
            inputItem = new HbInputItem();

            // Manage initial end of file conditions
            // This is not an error; it indicates that the last item has been processed
            if (dataTableIndex > dataTable.Rows.Count - 1) return "EOF";

            bool endOfFileCondition = false;
            if (dataTableIndex == dataTable.Rows.Count - 1) endOfFileCondition = true;

            // Create the new InputItem to be returned at the end.
            inputItem = new HbInputItem();
            string returnValue = "";

            // Get the initial data rows and set initial state
            DataRow dataRowCurrent = this.dataTable.Rows[dataTableIndex];
            DataRow dataRowNext = null;
            if (!endOfFileCondition) {
                dataRowNext = this.dataTable.Rows[dataTableIndex + 1];
            }
            string arrayState = ""; // = "", "InCurveArray", "InReferenceArray"  //"InUseSet", 

            // Primary loop
            List<HbXYZ> currentHbXyzList = null; // Null values just for compiler; a new one of these three is created whenever it is used
            HbCurveArrArray currentHbCurveArrayArray = null;  
            HbReferenceArrayArray currentHbReferenceArrayArray = null;
            
            bool continueLoop = true;
            while (continueLoop) {

                string commandActionCurrent = dataRowCurrent[2].ToString();
                string commandObjectCurrent = dataRowCurrent[3].ToString();
                string commandActionNext = null;
                string commandObjectNext = null;
                if (!endOfFileCondition) {
                    commandActionNext = dataRowNext[2].ToString();
                    commandObjectNext = dataRowNext[3].ToString();
                }

                // Branch based on array state and current action
                HbCurve hbCurve = null;
                List<HbXYZ> listHbXyz = null;
                GetHbEllipseValues getHbEllipseValues = null;
                List<string> stringValues = null;
                List<int> intValues = null;
                List<double> doubleValues = null;
                List<bool> boolValues = null;
                switch (arrayState) {
                    case "":
                        switch (commandActionCurrent) {
                            //TODO Not really checking any of the input for valid data here.  We should?
                            case "Add":
                                switch (commandObjectCurrent) {
                                    case "Level":
                                        doubleValues = new List<double> { Convert.ToDouble(dataRowCurrent[4].ToString(), this.provider) }; // Level datum 
                                        stringValues = new List<string> { dataRowCurrent[5].ToString() };                   // Level Name (optional)
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, null, stringValues, null, doubleValues, null);
                                        break;
                                    case "Grid":
                                        listHbXyz = new GetHbXyzRow(dataRowCurrent).HbPoints;
                                        if (listHbXyz.Count > 2) hbCurve = new HbArc(listHbXyz[0], listHbXyz[1], listHbXyz[1]);
                                        else hbCurve = new HbLine(listHbXyz[0], listHbXyz[1]);
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, "", hbCurve);
                                        break;
                                    case "DetailLine":
                                    case "ModelLine":
                                        listHbXyz = new GetHbXyzRow(dataRowCurrent).HbPoints;
                                        hbCurve = new HbLine(listHbXyz[0], listHbXyz[1]);
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, "Curve", hbCurve);
                                        break;
                                    case "DetailArc":
                                    case "ModelArc":
                                        listHbXyz = new GetHbXyzRow(dataRowCurrent).HbPoints;
                                        hbCurve = new HbArc(listHbXyz[0], listHbXyz[1], listHbXyz[2]);
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, "Curve", hbCurve);
                                        break;
                                    case "DetailEllipse":
                                    case "ModelEllipse":
                                        getHbEllipseValues = new GetHbEllipseValues(dataRowCurrent);
                                        currentHbXyzList = new List<HbXYZ>();
                                        currentHbXyzList.Add(getHbEllipseValues.PointFirst);
                                        currentHbXyzList.Add(getHbEllipseValues.PointSecond);
                                        hbCurve = new HbEllipse(getHbEllipseValues.PointFirst, getHbEllipseValues.PointSecond, getHbEllipseValues.RadiusY, getHbEllipseValues.Mode);
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, "Curve", hbCurve);
                                        break;
                                    case "DetailNurbSpline":
                                    case "ModelNurbSpline":
                                        if (currentHbXyzList != null) listHbXyz = currentHbXyzList;
                                        else listHbXyz = new GetHbXyzRow(dataRowCurrent).HbPoints;
                                        hbCurve = new HbNurbSpline(listHbXyz);
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, "Curve", hbCurve);
                                        break;
                                    case "AreaBoundaryLine":
                                    case "RoomSeparationLine":
                                        if (currentHbXyzList != null) hbCurve = new HbNurbSpline(currentHbXyzList);
                                        else {
                                            if (dataRowCurrent[7].ToString().ToLower() == "full" || dataRowCurrent[7].ToString().ToLower() == "half") {
                                                getHbEllipseValues = new GetHbEllipseValues(dataRowCurrent);
                                                currentHbXyzList = new List<HbXYZ>();
                                                currentHbXyzList.Add(getHbEllipseValues.PointFirst);
                                                currentHbXyzList.Add(getHbEllipseValues.PointSecond);
                                                hbCurve = new HbEllipse(getHbEllipseValues.PointFirst, getHbEllipseValues.PointSecond, getHbEllipseValues.RadiusY, getHbEllipseValues.Mode);
                                            }
                                            else {
                                                listHbXyz = new GetHbXyzRow(dataRowCurrent).HbPoints;
                                                if (listHbXyz.Count == 2) hbCurve = new HbLine(listHbXyz[0], listHbXyz[1]);
                                                else hbCurve = new HbArc(listHbXyz[0], listHbXyz[1], listHbXyz[2]);
                                            }
                                        }
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, "Curve", hbCurve);
                                        break;
                                    case "TopographySurface":
                                    case "AdaptiveComponent":
                                        if (currentHbXyzList != null) listHbXyz = currentHbXyzList;
                                        else listHbXyz = new GetHbXyzRow(dataRowCurrent).HbPoints;
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, "", listHbXyz);
                                        break;
                                    case "Wall":
                                        listHbXyz = new GetHbXyzRow(dataRowCurrent).HbPoints;
                                        if (listHbXyz.Count < 2) break;
                                        if (listHbXyz.Count == 2) hbCurve = new HbLine(listHbXyz[0], listHbXyz[1]);
                                        else hbCurve = new HbArc(listHbXyz[0], listHbXyz[1], listHbXyz[2]);
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, "Curve", hbCurve);
                                        break;
                                    case "Floor":
                                        if (currentHbXyzList != null) listHbXyz = currentHbXyzList;  // Note that use group input of points is not documented but is a good option
                                        else listHbXyz = new GetHbXyzRow(dataRowCurrent).HbPoints;
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, "Points", listHbXyz);
                                        break;
                                    case "FamilyInstance":
                                    case "Column":
                                    case "Beam":
                                    case "Area":
                                    case "Room":                                    
                                        listHbXyz = new GetHbXyzRow(dataRowCurrent).HbPoints;  // Only one for family instance, area, and room; two with column and beam.
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, "", listHbXyz);
                                        break;
                                    case "ReferencePoint":                                        
                                        HbReferencePoint hbReferencePoint = new GetHbReferencePoint(dataRowCurrent).HbReferencePoint;  // Only one point
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, null, hbReferencePoint, null, null);  
                                        break;
                                    case "CurveByPoints":
                                        HbReferenceArray hbReferenceArray = new HbReferenceArray();
                                        //if (currentHbXyzList.Count != 0) listHbXyz = currentHbXyzList;  // Note that use single line input of points is not documented but is a good option
                                        if (currentHbXyzList != null) listHbXyz = currentHbXyzList;  // Note that use single line input of points is not documented but is a good option
                                        else listHbXyz = new GetHbXyzRow(dataRowCurrent).HbPoints;
                                        foreach (HbXYZ hbXyz in listHbXyz) {
                                            hbReferenceArray.Add(new HbReferencePoint(hbXyz));
                                        }
                                        AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, null, null, hbReferenceArray, null);  
                                        break;

                                        //TODO Not handling this case for now.  It is not allowed in the specification and not sure how it got inot the sample data
                                        //case "FamilyExtrusion":
                                        ////    //csvWriter.AddFamilyExtrusion(curvesListList, "ExtrusionFamilyName", point11);
                                        ////    //csvWriter.AddFamilyExtrusion(curvesListList, "ExtrusionFamilyName");
                                        ////    //csvWriter.AddFamilyExtrusion(curvesListList, point11);
                                        ////    //csvWriter.AddFamilyExtrusion(curvesListList);

                                        ////if (commandObjectCurrent == "FamilyExtrusion") {
                                        ////    stringValues = new List<string> { dataRowCurrent[4].ToString() };           // Name
                                        ////    listHbXyz = new List<HbXYZ>();
                                        ////    listHbXyz.Add(new GetHbValue(dataRowCurrent[5].ToString()).AsHbXYZ());      // Insertion point optional (function will return null)
                                        ////    AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, null, listHbXyz, null, currentHbCurveArrayArray, null, null, null, stringValues, null, null, null);
                                        //break;
                                    default:  
                                        MessageBox.Show("Program error: Unknown 'CommandObject' in HummingbirdUtility.GetNextInputItem: " + commandObjectCurrent);     
                                        break;
                                }
                                currentHbXyzList = null; // reset for next use loop or other use
                                //inputItem.DataRowPrimary = dataRowCurrent;                                
                                continueLoop = false;
                                break;
                            case "Draw":  // Implicit that action is CurveArray
                                currentHbCurveArrayArray = new HbCurveArrArray();
                                arrayState = "InCurveArrArray";
                                break;
                            case "Model":
                                arrayState = "InReferenceArrayArray";
                                break;
                            case "Use":
                                returnValue = ProcessUse(dataTable, ref dataTableIndex, out currentHbXyzList);
                                if (returnValue != "") return returnValue;
                                break;
                            case "Set":
                                switch (commandObjectCurrent) {
                                    case "WallType":
                                    case "FloorType":
                                    case "Level":
                                    case "BeamJustification":
                                    case "FilledRegionType":
                                        stringValues = new List<string> { dataRowCurrent[4].ToString() };                   
                                        break;
                                    case "FamilyType":
                                    case "BeamType":
                                    case "AdaptiveComponentType":
                                    case "MullionType":
                                        stringValues = new List<string> { dataRowCurrent[4].ToString(), dataRowCurrent[5].ToString() };                   
                                        break;
                                    case "ColumnMode":
                                        stringValues = new List<string> { dataRowCurrent[4].ToString(), dataRowCurrent[5].ToString(), dataRowCurrent[6].ToString() };              
                                        break;
                                    case "WallHeight":                                                                        
                                    case "FamilyRotation":
                                    case "ColumnHeight":
                                    case "ColumnRotation":
                                    case "BeamRotation":
                                    case "FamilyExtrusionHeight":
                                        doubleValues = new List<double> { Convert.ToDouble(dataRowCurrent[4].ToString(), this.provider) }; 
                                        break;
                                    case "FamilyMirrored":
                                    case "FamilyFlipped":
                                        boolValues = new List<bool> { Convert.ToBoolean(dataRowCurrent[4].ToString()), Convert.ToBoolean(dataRowCurrent[5].ToString()) };                                       
                                        break;
                                    default:  
                                        MessageBox.Show("Program error: Unknown 'CommandObject'.");     
                                        break;
                                }
                                AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, null, stringValues, intValues, doubleValues, boolValues);
                                continueLoop = false;
                                //inputItem.DataRowPrimary = dataRowCurrent;       
                                break;
                            case "Modify": 
                                switch (commandObjectCurrent) {
                                    case  "ParameterSet":
                                        stringValues = new List<string> { dataRowCurrent[4].ToString(), dataRowCurrent[5].ToString() };  // Note that second item is kept as string until we actually set the parameter
                                        break;
                                    case  "CurtainGridUAdd":
                                    case  "CurtainGridVAdd":
                                    case  "MullionUAdd":
                                    case  "MullionVAdd":
                                        if (dataRowCurrent[5].ToString() == "" || dataRowCurrent[5].ToString() == null)
                                            doubleValues = new List<double> { Convert.ToDouble(dataRowCurrent[4].ToString(), this.provider) }; 
                                        else
                                            doubleValues = new List<double> { Convert.ToDouble(dataRowCurrent[4].ToString()), Convert.ToDouble(dataRowCurrent[5].ToString(), this.provider) }; 
                                        break;
                                    default:
                                        MessageBox.Show("Program error: Unknown 'CommandObject'.");
                                        break;
                                }
                                AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, null, stringValues, intValues, doubleValues, boolValues);
                                //inputItem.DataRowPrimary = dataRowCurrent;       
                                continueLoop = false;
                                break;
                            default:
                                MessageBox.Show("Program error: Unknown 'actionValue'.");
                                break;
                        }
                        break;
                    case "InCurveArrArray":
                        switch (commandActionCurrent) {
                            // Valid cases
                            case "Add":
                                if (commandObjectCurrent == "FamilyExtrusion") {                                    
                                    stringValues = new List<string> { dataRowCurrent[4].ToString() };           // Name
                                    HbXYZ insertionPoint = new GetHbValue(dataRowCurrent[5].ToString()).AsHbXYZ();      // Insertion point optional (function will return null)
                                    if (insertionPoint == null) {
                                        listHbXyz = null;
                                    } else {
                                        listHbXyz = new List<HbXYZ>();
                                        listHbXyz.Add(insertionPoint);      
                                    }
                                    //listHbXyz = new List<HbXYZ>();
                                    //listHbXyz.Add(new GetHbValue(dataRowCurrent[5].ToString()).AsHbXYZ());      // Insertion point optional (function will return null)                                       
                                    AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, null, listHbXyz, null, currentHbCurveArrayArray, null, null, null, stringValues, null, null, null);
                                }
                                else {  // Floor, Wall, AreaBoundaryLine etc. with curvearray
                                    AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, "CurveArrArray", currentHbCurveArrayArray);
                                }
                                //inputItem.DataRowPrimary = dataRowCurrent;
                                continueLoop = false;
                                break;
                            case "Use":
                            case "Draw":
                                ProcessDraw(dataTable, ref dataTableIndex, out currentHbCurveArrayArray);
                                if (returnValue != "") return returnValue;
                                break;
                            // Error cases                            
                            case "Set":
                            case "Model":
                            case "Modify":
                                MessageBox.Show("Program error: '" + commandActionCurrent + "' value encountered during Curve Array.");
                                break;
                            default:
                                MessageBox.Show("Program error: Unknown 'actionValue'.");
                                break;
                        }
                        break;
                    case "InReferenceArrayArray":
                        switch (commandActionCurrent) {
                            // Valid cases
                            case "Add":                                
                                AddCommand(inputItem, commandActionCurrent, commandObjectCurrent, null, null, null, currentHbReferenceArrayArray);
                                //inputItem.DataRowPrimary = dataRowCurrent;
                                continueLoop = false;
                                break;
                            case "Use":                            
                                returnValue = ProcessModel(dataTable, ref dataTableIndex, out currentHbReferenceArrayArray);
                                if (returnValue != "") return returnValue;
                                break;
                            //Error cases
                            case "Set":
                            case "Model":  // Currently "Model will never occur; we are past the "Model - ReferenceArray" line at this point
                            case "Draw":
                            case "Modify":
                                MessageBox.Show("Program error: '" + commandActionCurrent + "' value encountered while in 'ReferenceArray' state.");
                                break;
                            default:
                                MessageBox.Show("Program error: Unknown 'actionValue'.");
                                break;
                        }
                        break;
                    default:
                        return "Unknown arrayState: " + arrayState;
                }

                // Always do these actions at the end
                this.lastAction = commandActionCurrent;
                this.lastObject = commandObjectCurrent;

                // Increment index and get the next row if continuing loop.
                dataTableIndex++;
                if (continueLoop) {                    
                    if (dataTableIndex > dataTable.Rows.Count - 1) {
                        return "Program error: End of file encountered.";
                    }
                    dataRowCurrent = this.dataTable.Rows[dataTableIndex];
                    if (dataTableIndex == dataTable.Rows.Count - 1) {
                        endOfFileCondition = true;
                        dataRowNext = null;
                    }
                    else dataRowNext = this.dataTable.Rows[dataTableIndex + 1];                                           
                }

                if (continueLoop == false) {  // About to exit the loop so capture the current row
                    inputItem.DataRowPrimary = dataRowCurrent;
                }

            }  //end loop

            // Set RowId based on last action
            inputItem.RowId = dataRowCurrent[0].ToString();

            // Set the ElementId based on the last Add
            if (!(this.lastAction == "Set" || this.lastAction == "Modify")) {
                inputItem.ElementId = new GetHbValue(dataRowCurrent[1].ToString()).AsHbElementId();
            }

            return "";            
        }


       // ***************************************************** Private Functions *************************************************

        // For single points and sets of points
        private bool AddCommand(HbInputItem inputItem, string commandAction, string commandObject, string commandModifier, List<HbXYZ> hbXyzList ) {
            return AddCommand(inputItem, commandAction, commandObject, commandModifier, hbXyzList, null, null, null, null, null, null, null, null, null);
        }
        // Simple Curve (Line/Arc/Ellipse/NurbSpline/HermiteSpline)
        private bool AddCommand(HbInputItem inputItem, string commandAction, string commandObject, string commandModifier, HbCurve hbCurve) {
            return AddCommand(inputItem, commandAction, commandObject, commandModifier, null, hbCurve, null, null, null, null, null, null, null, null);
        }
        // For CurveArrayArray  (also covers CurveArray due to Revit convention)
        private bool AddCommand(HbInputItem inputItem, string commandAction, string commandObject, string commandModifier, HbCurveArrArray hbCurveArrayArray) {
            return AddCommand(inputItem, commandAction, commandObject, commandModifier, null, null, hbCurveArrayArray, null, null, null, null, null, null, null);
        }
        // For ReferencePoint, ReferenceArray and ReferenceArrayArray
        private bool AddCommand(HbInputItem inputItem, string commandAction, string commandObject, string commandModifier, HbReferencePoint hbReferencePoint, HbReferenceArray hbReferenceArray, HbReferenceArrayArray hbReferenceArrayArray) {
            return AddCommand(inputItem, commandAction, commandObject, commandModifier, null, null, null, hbReferencePoint, hbReferenceArray, hbReferenceArrayArray, null, null, null, null);
        }
        // For set and modify; Level
        private bool AddCommand(HbInputItem inputItem, string commandAction, string commandObject, string commandModifier, List<string> stringValues, List<int> intValues, List<double> doubleValues, List<bool> boolValues) {
            return AddCommand(inputItem, commandAction, commandObject, commandModifier, null, null, null, null, null, null, stringValues, intValues, doubleValues, boolValues);
        }
        // Full signature
        private bool AddCommand(HbInputItem inputItem, string commandAction, string commandObject, string commandModifier, List<HbXYZ> hbXyzList, HbCurve hbCurve, HbCurveArrArray hbCurveArrayArray,
            HbReferencePoint hbReferencePoint, HbReferenceArray hbReferenceArray, HbReferenceArrayArray hbReferenceArrayArray, List<string> stringValues, List<int> intValues, List<double> doubleValues, List<bool> boolValues) {
            // Always required
            inputItem.CommandAction = commandAction;
            inputItem.CommandObject = commandObject;
            // Requirements depend on command; null values are OK
            inputItem.CommandModifier = commandModifier;
            inputItem.ListHbXYZ = hbXyzList;
            inputItem.HbCurve = hbCurve;   // includes all Hb curves but not ReferencePoint            
            inputItem.HbCurveArrArray = hbCurveArrayArray;            // includes HbCurveArray as single item
            inputItem.HbReferencePoint = hbReferencePoint;
            inputItem.HbReferenceArray = hbReferenceArray;
            inputItem.HbReferenceArrayArray = hbReferenceArrayArray;  // includes HbReferenceArray as single item
            inputItem.StringValues = stringValues;
            inputItem.IntValues = intValues;
            inputItem.DoubleValues = doubleValues;
            inputItem.BoolValues = boolValues;
            return true;
        }

        private string ProcessUse(DataTable dataTable, ref int dataTableIndex, out List<HbXYZ> hbXyzList) {
            hbXyzList = new List<HbXYZ>();
            if (dataTableIndex > dataTable.Rows.Count - 1) return "Program error: dataTable index value exceeded range.";
            DataRow dataRow = dataTable.Rows[dataTableIndex];
            GetHbXyzRow getHbXyzRow = new GetHbXyzRow(dataRow);
            int testNextIndex = dataTableIndex + 1;
            if (testNextIndex > dataTable.Rows.Count - 1) return "End of file encountered during Use Set.";
            bool continueLoop = true;
            while (continueLoop) {
                dataRow = dataTable.Rows[testNextIndex];
                if (dataRow[2].ToString() == "Use") {
                    getHbXyzRow.NextDataRow(dataRow);
                    testNextIndex++;
                    if (testNextIndex > dataTable.Rows.Count - 1) return "End of file encountered during Use Set.";
                }
                else continueLoop = false;  // Next row is not a Use so break loop
            }
            dataTableIndex = testNextIndex - 1;
            hbXyzList = getHbXyzRow.HbPoints;
            return "";
        }

        private string ProcessDraw(DataTable dataTable, ref int dataTableIndex, out HbCurveArrArray hbCurveArrArray) {
            string returnValue = "";
            hbCurveArrArray = new HbCurveArrArray();
            HbCurveArray hbCurveArray = null;
            if (dataTableIndex >= dataTable.Rows.Count - 1) return "Program error: dataTable index value exceeded range.";
            DataRow dataRow = dataTable.Rows[dataTableIndex];
            string actionCurrent = dataRow[2].ToString();
            int testNextIndex = dataTableIndex + 1;
            if (testNextIndex > dataTable.Rows.Count - 1) return "End of file encountered during Curve Array.";
            bool continueLoop = true;
            hbCurveArray = new HbCurveArray(); // Need to make new array at the start
            List<HbXYZ> hbXyzList = null;
            while (continueLoop) {
                dataRow = dataTable.Rows[dataTableIndex];
                actionCurrent = dataRow[2].ToString();
                string objectCurrent = dataRow[3].ToString();                
                switch (actionCurrent) {
                    case "Use":
                        returnValue = ProcessUse(dataTable, ref dataTableIndex, out hbXyzList);
                        if (returnValue != "") return returnValue;
                        testNextIndex = dataTableIndex + 1;
                        if (testNextIndex > dataTable.Rows.Count - 1) return "End of file encountered during Curve Array.";
                        break;
                    case "Draw":
                        // objectCurrent is not "CurveArray" first time since we are past that line when we entered the Draw state.
                        if (objectCurrent == "CurveArray") {    // Starting new loop
                            hbCurveArrArray.Add(hbCurveArray);  // Capture the last array
                            hbCurveArray = new HbCurveArray();  // Start new array
                        }
                        else {  //Valid objectCurrent: "Line", "Arc", "NurbSpline", "Ellipse", "HermiteSpline"                             
                            if (hbXyzList == null) {
                                hbXyzList = new GetHbXyzRow(dataRow).HbPoints;
                            }
                            HbCurve hbCurve;  
                            returnValue = ProcessHbCurve(objectCurrent, dataRow, hbXyzList, out hbCurve);
                            if (returnValue != "") return returnValue;
                            hbCurveArray.HbCurves.Add(hbCurve);
                            hbXyzList = null; // So that the test above will work with the next item
                        }                        
                        break;

                    default:
                        return "Action '" + actionCurrent + "' encountered during Curve Array.";
                }
                testNextIndex = dataTableIndex + 1;
                if (testNextIndex > dataTable.Rows.Count - 1) return "End of file encountered during Curve Array.";
                dataRow = dataTable.Rows[testNextIndex];
                if (dataRow[2].ToString() == "Add") {
                    continueLoop = false;  // Next row will be the add statement so break loop
                    hbCurveArrArray.Add(hbCurveArray); // Capture the last one
                }
                else dataTableIndex++;
            } // Loop
            return "";
        }

        private string ProcessModel(DataTable dataTable, ref int dataTableIndex, out HbReferenceArrayArray hbReferenceArrayArray) {
            string returnValue = "";
            hbReferenceArrayArray = new HbReferenceArrayArray();
            if (dataTableIndex >= dataTable.Rows.Count - 1) return "Program error: dataTable index value exceeded range.";
            DataRow dataRow = dataTable.Rows[dataTableIndex];
            string actionCurrent = dataRow[2].ToString();
            int testNextIndex = dataTableIndex + 1;
            if (testNextIndex > dataTable.Rows.Count - 1) return "End of file encountered during Reference Array.";
            bool continueLoop = true;
            while (continueLoop) {
                dataRow = dataTable.Rows[dataTableIndex];
                actionCurrent = dataRow[2].ToString();
                string objectCurrent = dataRow[3].ToString();
                List<HbXYZ> hbXyzList = null;
                switch (actionCurrent) {
                    case "Use":
                        returnValue = ProcessUse(dataTable, ref dataTableIndex, out hbXyzList);
                        if (returnValue != "") return returnValue;
                        testNextIndex = dataTableIndex + 1;
                        if (testNextIndex > dataTable.Rows.Count - 1) return "End of file encountered during Reference Array.";
                        hbReferenceArrayArray.HbReferenceArrays.Add(new HbReferenceArray(hbXyzList));
                        break;
                    case "Model":  // Implicit ReferenceArray object
                        //Don't need to do anything here since it is all captured in the Use case.                        
                        break;
                    default:
                        return "Action '" + actionCurrent + "' encountered during Reference Array.";
                }
                testNextIndex = dataTableIndex + 1;
                if (testNextIndex > dataTable.Rows.Count - 1) return "End of file encountered during Reference Array.";
                dataRow = dataTable.Rows[testNextIndex];
                if (dataRow[2].ToString() == "Add") continueLoop = false;  // Next row will be the add statement so break loop
                else dataTableIndex++;
            } // Loop
            return "";
        }

        private string ProcessHbCurve(string objectCurrent, List<HbXYZ> hHbXyzList, out HbCurve hbCurve) {
            return ProcessHbCurve(objectCurrent, null, hHbXyzList, out hbCurve);
        }
        private string ProcessHbCurve(string objectCurrent, DataRow dataRow, out HbCurve hbCurve) {
            return ProcessHbCurve(objectCurrent, dataRow, null, out hbCurve);
        }
        private string ProcessHbCurve(string objectCurrent, DataRow dataRow, List<HbXYZ>hHbXyzList, out HbCurve hbCurve) {
            switch (objectCurrent) {
                case "Line":
                    if (hHbXyzList.Count < 2) {
                        hbCurve = null;
                        return "";
                    }
                    hbCurve = new HbLine(hHbXyzList[0], hHbXyzList[1]);
                    break;
                case "Arc":
                    if (hHbXyzList.Count < 3) {
                        hbCurve = null;
                        return "";
                    }
                    hbCurve = new HbArc(hHbXyzList[0], hHbXyzList[1], hHbXyzList[2]);
                    break;
                case "NurbSpline":
                    if (hHbXyzList.Count < 2) {
                        hbCurve = null;
                        return "";
                    }
                    hbCurve = new HbNurbSpline(hHbXyzList);
                    break;
                case "Ellipse":
                    HbXYZ point1 = new GetHbValue(dataRow[4].ToString()).AsHbXYZ();
                    HbXYZ point2 = new GetHbValue(dataRow[5].ToString()).AsHbXYZ();
                    if (point1 == null || point2 == null) {
                        hbCurve = null;
                        return "";
                    }
                    double radius = (double)new GetHbValue(dataRow[6].ToString()).AsDouble();
                    string mode = dataRow[6].ToString();
                    hbCurve = new HbEllipse(point1, point2, radius, mode);
                    break;
                case "HermiteSpline":
                    if (hHbXyzList.Count < 2) {
                        hbCurve = null;
                        return "";
                    }
                    hbCurve = new HbHermiteSpline(hHbXyzList);
                    break;
                default:
                    hbCurve = null;
                    return "Object '" + objectCurrent + "' encountered during ProcessHbCurve.";
            }
            return "";
        }
    }
}


