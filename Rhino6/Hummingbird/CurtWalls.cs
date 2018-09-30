using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HummingbirdUtility;

namespace Hummingbird
{
    public class CurtWalls : GH_Component {

        private string DEFAULT_FILE_NAME = "CurtWalls.csv";  // "const" not allowed with GH

        public CurtWalls() : base("CurtWalls", "CurtWalls", "Add Revit Curtain Walls", "Extra", "Hummingbird") {            
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);                                  // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                                         // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);                                        // 2
            pManager[2].Optional = true;
            pManager.AddCurveParameter("Curves", "Curves", "A Tree of Curves for Curtain Wall Instances", GH_ParamAccess.tree);                             // 3
            pManager[3].Optional = true;
            pManager.AddTextParameter("RevitTypeName", "WallType", "Revit Curtain Wall Type Name", GH_ParamAccess.item);                                    // 4
            pManager[4].Optional = true;
            pManager.AddNumberParameter("RevitWallHeight", "Height", "Set Revit Wall Height",GH_ParamAccess.item);                                          // 5
            pManager[5].Optional = true;
            pManager.AddTextParameter("Grids/Mullions", "Grid/Mullions", "A tree of lists of coded text values that place the grid/mullions", GH_ParamAccess.tree);  // 6
            pManager[6].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting Curtain Walls.");
            List<string> instructionData = new List<string>();     

            // Get Inputs
            string folderPath = null, fileName = null;               
            bool write = false;
            if (!utility.GetInput(0, ref write)) {                   // Write command is required
                utility.WriteOut();
                return;
            }
            if (!utility.GetInput(1, ref folderPath)) {              // Folder path is required
                utility.WriteOut();
                return;
            }
            utility.GetInput(2, ref fileName, true, true, true);     // File name is optional
            if (fileName == null) fileName = this.DEFAULT_FILE_NAME;

            GH_Structure<GH_Curve> curvesDataTree = null;                  // curves in Data Tree required
            if (!utility.GetInput(3, out curvesDataTree)) return;

            string wallTypeName = null;                                  // Type is optional (no family, just type)
            utility.GetInput(4, ref wallTypeName);
          
            double wallHeight = 0;                                   // Wall Height is optional
            utility.GetInput(5, ref wallHeight);

            GH_Structure<GH_String> gridMullionsDataTree = null;                           // Grids/Mullions are optional; might make sense during development
            utility.GetInput(6, out gridMullionsDataTree);


            if (write) {
                try {

                    // Create RevitModelBuilderUtility object and link to CSV file            
                    CsvWriter csvWriter = new CsvWriter();
                    utility.Print("CsvWriter Version: " + csvWriter.Version);
                    if (!utility.EstablishCsvLink(csvWriter, folderPath, fileName)) {
                        utility.Print("EstablishCsvLink() failed");
                        utility.WriteOut();
                        return;
                    }

                    // Set Wall Type
                    if (wallTypeName != null) {
                        csvWriter.SetWallType(wallTypeName);
                    }

                    //Set Wall Height
                    double revitWallHeight = 10.0;
                    if (wallHeight != 0) {
                        GH_Convert.ToDouble(wallHeight, out revitWallHeight, GH_Conversion.Both);
                        csvWriter.SetWallHeight(revitWallHeight);
                    }

                    List<List<HbCurve>> curvesListListRevit = new List<List<HbCurve>>();
                    // Loop through the data tree of curves and process each one.
                    for (int i = 0; i < curvesDataTree.Branches.Count; i++) {
                        if (!utility.ReadDataTreeBranch(curvesDataTree.Branches[i], ref curvesListListRevit)) {
                            utility.Print("ReadDataTreeBranch() failed at curvesDataTree");
                            utility.WriteOut();
                            return;
                        }
                    }
//TODO test missing input
                    List<List<String>> stringsListListRevit = new List<List<string>>();
                    // Loop through the data tree of mullions and process each one.
                    for (int i = 0; i < gridMullionsDataTree.Branches.Count; i++) {
                        if (!utility.ReadDataTreeBranch(gridMullionsDataTree.Branches[i], ref stringsListListRevit)) {
                            utility.Print("ReadDataTreeBranch() failed at stringsListListRevit");
                            utility.WriteOut();
                            return;
                        }
                    }


                    string familyNameCurrent = null;
                    string typeNameCurrrent = null;
                    for (int i = 0; i < curvesListListRevit.Count; i++) {
                        List<HbCurve> curvesListRevit = curvesListListRevit[i];

                        csvWriter.AddWall(curvesListRevit);
                        instructionData.Add("Add Curtain Wall:");
                        // Stop
                        if (stringsListListRevit.Count > i) { // Ignore mismatched list?  Better would be to reuse?
                            //outputStrings.Add("<" + direction + "><" + primaryOffset.ToString() + "><" + secondaryOffset.ToString() + "><" + familyName + "><" + typeName + ">");
                            string direction;
                            string primaryOffsetString;
                            string secondaryOffsetString;
                            string familyName;
                            string typeName;
                            //bool oneSegmentOnly;

                            foreach (string stringSource in stringsListListRevit[i]) {
                                if (!parseString(stringSource, out direction, out primaryOffsetString, out secondaryOffsetString, out familyName, out typeName)) continue; // Silent ignore bad values
                                direction = direction.ToUpper();
                                if (!(direction == "U" || direction == "V")) continue;
                                double primaryOffset, secondaryOffset;
                                double.TryParse(primaryOffsetString, out primaryOffset);
                                if (double.IsNaN(primaryOffset) || double.IsInfinity(primaryOffset)) continue;
                                double.TryParse(secondaryOffsetString, out secondaryOffset);
                                if (double.IsNaN(secondaryOffset) || double.IsInfinity(secondaryOffset)) secondaryOffset = 0.0;
                                if (familyName != null && familyName != "" && typeName != null && typeName != "") {
                                    if (familyName != familyNameCurrent || typeName != typeNameCurrrent) {
                                        if (!(familyName == "$none$" || typeName == "$none$")) {
                                            csvWriter.SetMullionType(familyName, typeName);
                                        }
                                        familyNameCurrent = familyName;
                                        typeNameCurrrent = typeName;
                                    }
                                }
                                if (direction == "U") {
                                    if (secondaryOffset == 0) {                                // full length case
                                        if (familyName == "$none$" || typeName == "$none$") {  //  - grid case
                                            csvWriter.ModifyCurtainGridUAdd(primaryOffset);
                                        } else {                                               //  - mullion case
                                            csvWriter.ModifyMullionUAdd(primaryOffset);
                                        }
                                    } else {                                                   // oneSegmentOnly case
                                        if (familyName == "$none$" || typeName == "$none$") {  //  - grid case
                                            csvWriter.ModifyCurtainGridUAdd(primaryOffset, secondaryOffset);
                                        } else {                                               //  - mullion case
                                            csvWriter.ModifyMullionUAdd(primaryOffset, secondaryOffset);
                                        }
                                    }                                    
                                } else { // direction == "V" case
                                    if (secondaryOffset == 0) {                                // full length case
                                        if (familyName == "$none$" || typeName == "$none$") {  //  - grid case
                                            csvWriter.ModifyCurtainGridVAdd(primaryOffset);
                                        } else {                                               //  - mullion case
                                            csvWriter.ModifyMullionVAdd(primaryOffset);
                                        }
                                    } else {                                                   // oneSegmentOnly case
                                        if (familyName == "$none$" || typeName == "$none$") {  //  - grid case
                                            csvWriter.ModifyCurtainGridVAdd(primaryOffset, secondaryOffset);
                                        } else {                                               //  - mullion case
                                            csvWriter.ModifyMullionVAdd(primaryOffset, secondaryOffset);
                                        }
                                    }

                                }
                                instructionData.Add("Add Grid/Mullion:");
                            }
                        }   


                        //if (gridMullionsDataTree == null) continue;     // Not sure if this occurs
                        //if (gridMullionsDataTree.Count == 0) continue;  // Valid but no grids or mullions included
                        //string currentMullionFamilyName = "";
                        //string currentMullionTypeName = "";
                        //for (int j = 0; j < gridMullionsDataTree.Count; j++) {

                        //}
                    }
                    csvWriter.WriteFile();
                    utility.Print("Add Curtain Wall completed successfully.");
                }
                catch (Exception exception) {
                    utility.Print(exception.Message);
                }
            }
            utility.WriteOut();
            DA.SetDataList(1, instructionData);

// Temp
            //DA.SetDataList(1, gridMullionsDataTree);

        }

        private bool parseString(string source, out string direction, out string primaryOffsetString, out string secondaryOffsetString, out string familyName, out string typeName) {
            direction = parseNode(ref source, false);
            primaryOffsetString = parseNode(ref source, false);
            secondaryOffsetString = parseNode(ref source, false);
            familyName = parseNode(ref source, false);
            typeName = parseNode(ref source, true);
            return true;
        }

        private string parseNode(ref string source, bool lastNode) {
            if (source.Length < 3) return null;  // Bad string
            if ((source.Substring(0, 1) != "<")) return null;  // Bad string
            int pos = source.IndexOf(">");
            string value = source.Substring(1, pos - 1);
            if (!lastNode) {
                if (source.Length < pos + 2) return null;  // Bad string
                source = source.Substring(pos + 1);
            }
            return value;
        }

        // ------------------------------------------------------- General housekeeping for all components ----------------------------------------------------------

        public override GH_Exposure Exposure {
            get { return GH_Exposure.secondary; }
        }

        public override Guid ComponentGuid {
            get { return new Guid("33E6A1AD-581A-4B1A-AA52-88BA7DF11DA4"); }
        }
        protected override Bitmap Icon {
            get { return new Bitmap(Hummingbird.Properties.Resources.CurtWall); }
        }

    }
}
