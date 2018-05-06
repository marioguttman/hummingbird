using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Data;
using System.Windows.Forms;

namespace HummingbirdUtility {
    
    class SyntaxChecker {

        public Dictionary<string, List<string>> ActionsObjects { set; get; }  // No reason to be public but just in case someone wants to use these values

        private string arrayState; // = "", "InCurveArray", "InReferenceArray"
        private string lastAction;
        private string lastObject;

        public SyntaxChecker() {

            ActionsObjects = new Dictionary<string, List<string>>();
            ActionsObjects.Add("Set", new List<string> { "Level", "WallType", "WallHeight", "FloorType", "FilledRegionType", "FamilyType", "FamilyRotation", "FamilyMirrored", "FamilyFlipped",  
                "ColumnMode", "ColumnHeight", "ColumnRotation", "BeamType", "BeamRotation", "BeamJustification", "AdaptiveComponentType", "FamilyExtrusionHeight", "MullionType" });
            ActionsObjects.Add("Add", new List<string> { "Grid", "Level", "DetailLine", "DetailArc", "DetailEllipse", "DetailNurbSpline", 
                "ModelLine", "ModelArc", "ModelEllipse", "ModelNurbSpline", "AreaBoundaryLine", "RoomSeparationLine", "TopographySurface", "FamilyExtrusion",
                "Wall", "Floor", "FamilyInstance", "Column", "Beam", "AdaptiveComponent", "Area", "Room", "FilledRegion", "ReferencePoint", "CurveByPoints", "LoftForm" });
            ActionsObjects.Add("Draw", new List<string> { "CurveArray", "Line", "Arc", "NurbSpline", "Ellipse", "HermiteSpline" });
            ActionsObjects.Add("Model", new List<string> { "ReferenceArray" });
            ActionsObjects.Add("Use", new List<string> { "Points" });
            ActionsObjects.Add("Modify", new List<string> { "ParameterSet", "CurtainGridUAdd", "CurtainGridVAdd", "MullionUAdd", "MullionVAdd" });

            this.lastAction = "";
            this.lastObject = "";
            this.arrayState = "";
        }

        public string ValidateRow(DataRow dataRow) {
            string returnValue = "";
            string actionValue = dataRow[2].ToString();
            string objectValue = dataRow[3].ToString();

            // Basic check for valid commands
            if (!(ActionsObjects.ContainsKey(actionValue))) return "Unknown 'Action': " + actionValue;
            if (!(ActionsObjects[actionValue].Contains(objectValue))) return "Unknown 'Action-Object pair': " + actionValue + "-" + objectValue; 

// TODO add checks based on Value01 etc.

            // Checks based on array state and current action
            switch (arrayState) {                    
                case "":
                    switch (actionValue) {
                        // All valid cases
                        case "Add":
                        case "Draw":  
                        case "Model":
                        case "Use":
                            break;
                        case "Set":
                            if (!(lastAction == "" || lastAction == "Add" || lastAction == "Set" || lastAction == "Modify")) returnValue = "'Set' must be at start or follow a 'Set', 'Add' or 'Modify' action.";
                            break;
                        case "Modify":
                            if (!(lastAction == "Add" || lastAction == "Modify")) returnValue = "'Modify' must follow an 'Add' action or another 'Modify' action.";
                            break;
                        default:
                            MessageBox.Show("Program error: Unknown 'actionValue'.");
                            break;
                    } 
                    break;
                case "InCurveArray":
                    switch (actionValue) {
                        // Valid cases
                        case "Add":
                        case "Draw":
                        case "Use":
                            break;
                        // Error cases                        
                        case "Set":                            
                        case "Model":
                        case "Modify":
                            returnValue = "'" + actionValue + "' value encountered during Curve Array.";
                            break;
                        default:
                            MessageBox.Show("Program error: Unknown 'actionValue'.");
                            break;
                    }
                    break;
                case "InReferenceArray":
                    if (actionValue == "Add") this.arrayState = "";
                    switch (actionValue) {
                        // Valid cases
                        case "Add":                            
                        case "Model":
                        case "Use":
                            break;
                        //Error cases
                        case "Set":
                        case "Draw":
                        case "Modify":
                            returnValue = "'" + actionValue + "' value encountered while in 'ReferenceArray' state.";
                            break;
                        default:
                            MessageBox.Show("Program error: Unknown 'actionValue'.");
                            break;
                    }
                    break;
                default:
                    returnValue = "Unknown arrayState: " + arrayState;
                    break;
            }

            // Checks based on array state and last action
            if (returnValue == "") {
                switch (arrayState) {
                    case "":
                        switch (lastAction) {
                            // Valid cases
                            case "":        // Initial condition
                            case "Add":     // Can be followed by any command  
                            case "Set":     // Can be followed by any command
                            case "Modify":  // Can be followed by any command
                                break;
                            case "Use":     // Already insured that it is a Use - Points pair.
                                if (!(actionValue == "Use" || actionValue == "Add")) returnValue = "In 'Non-Array' state 'Use' must be followed by 'Use' or 'Add' command.";
                                break;
                            // Error cases
                            case "Draw":
                            case "Model":
                                returnValue = "Invalid last action in 'Non-Array' state.";
                                break;
                            default:
                                returnValue = "Program error: Unknown 'lastAction' value.";
                                break;
                        }
                        break;
                    case "InCurveArray":
                        switch (lastAction) {
                            // Valid cases
                            case "Draw":    // Start or continue condition  
                            case "Use":     // Continue condition  
                                if (!(actionValue == "Draw" || actionValue == "Use" || actionValue == "Add")) returnValue = "In 'CurveArray' state '" + lastAction + "' must be followed by 'Draw', 'Use' or 'Add' command.";
                                break;
                            // Error cases
                            case "Add":
                            case "Set":
                            case "Model":
                            case "Modify":
                                returnValue = "Program error: Invalid last action but in 'CurveArray' state.";
                                break;
                            default:
                                returnValue = "Program error: Unknown 'lastAction' value.";
                                break;
                        }

                        break;
                    case "InReferenceArray":
                        switch (lastAction) {
                            // Valid cases
                            case "Model":    // Start condition  
                            case "Use":      // Continue condition 
                                if (!(actionValue == "Use" || actionValue == "Model")) returnValue = "In 'ReferenceArray' state '" + lastAction + "' must be followed by 'Model', or 'Use' command.";
                                break;
                            // Error cases
                            case "Add":
                            case "Set":
                            case "Draw":
                            case "Modify":
                                returnValue = "Program error: Invalid last action in 'ReferenceArray' state.";
                                break;
                            default:
                                returnValue = "Program error: Unknown 'lastAction' value.";
                                break;
                        }
                        break;
                    default:
                        returnValue = "Unknown arrayState: " + arrayState;
                        break;
                }
            }

            // Set array state for next cycle
            if (returnValue == "") {
                switch (arrayState) {                    
                    case "":
                        if (actionValue == "Draw") this.arrayState = "InCurveArray";           // Start of curve array
                        else if (actionValue == "Model") this.arrayState = "InReferenceArray"; // Start of reference array
                        break;
                    case "InCurveArray":
                    case "InReferenceArray":
                        if (actionValue == "Add") this.arrayState = "";
                        break;
                    default:
                            returnValue = "Unknown arrayState: " + arrayState;
                            break;
                }
            }

            // Always do these actions at the end
            this.lastAction = actionValue;
            this.lastObject = objectValue;
            return returnValue;
        }

        
    }

}
