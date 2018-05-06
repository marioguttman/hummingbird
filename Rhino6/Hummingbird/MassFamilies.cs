using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HummingbirdUtility;

namespace Hummingbird
{
    public class MassFamilies : GH_Component {

        private string DEFAULT_FILE_NAME = "MassFamilies.csv";  // "const" not allowed with GH

        public MassFamilies() : base("MassFamilies", "MassFamilies", "Add Mass Family Extrusion", "Extra", "Hummingbird") {
        }
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);                     // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                            // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);                           // 2
            pManager[2].Optional = true;
            pManager.AddTextParameter("Name", "Family Name", "Revit Mass Family Name", GH_ParamAccess.item);                                   // 3
            pManager[3].Optional = true;
            pManager.AddCurveParameter("Profile", "Profile", "A tree of curves defining mass family extrusion profiles", GH_ParamAccess.tree); // 4
            pManager[4].Optional = true;
            pManager.AddPointParameter("Pnt", "Insertion Point", "A list of Insertion Points for mass families", GH_ParamAccess.list);         // 5
            pManager[5].Optional = true;
            pManager.AddNumberParameter("Height", "Height", "A list of extrusion heights for mass families", GH_ParamAccess.list);             // 6
            pManager[6].Optional = true;
            pManager.AddTextParameter("ParameterNames", "Params", "A List of parameter names to set for each floor", GH_ParamAccess.list);     // 7
            pManager[7].Optional = true;
            pManager.AddTextParameter("ParameterValues", "Values", "A tree of parameter values for floor instances", GH_ParamAccess.tree);     // 8
            pManager[8].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting Family Extrusions.");
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

            string famName = null;
            utility.GetInput(3, ref famName);                   // Family Name is optional

            GH_Structure<GH_Curve> dataTree = null;             // Profiles in Data Tree required
            if (!utility.GetInput(4, out dataTree)) return;

            List<GH_Point> placementPnt = null;
            utility.GetInput(5, ref placementPnt);   // placement point for profiles. optional

            List<double> heights = null;             // heights of extrusions
            if (!utility.GetInput(6, ref heights)) return;
            
            List<string> parameterNames = null;                 // Parameter names list and values tree are optional but both must be provided if used.
            GH_Structure<GH_String> parameterValues = null;
            string[,] parameterArray = null;

            utility.GetParameterValueArray(7, 8, ref parameterNames, out parameterValues, out parameterArray);
            int iMaxCountParam = 0, jMaxCountParam = 0;

            if (parameterArray != null) {
                iMaxCountParam = parameterArray.GetLength(0);
                jMaxCountParam = parameterArray.GetLength(1);
                if (parameterNames.Count < jMaxCountParam) jMaxCountParam = parameterNames.Count;
            }

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

                    // Loop through the data tree of curves and process each one.
                    for (int i = 0; i < dataTree.Branches.Count; i++) {
                        // Parse the profile curves
                        List<List<HbCurve>> curvesListListRevit = new List<List<HbCurve>>();
                        if (!utility.ReadDataTreeBranch(dataTree.Branches[i], ref curvesListListRevit)) {
                            utility.Print("ReadDataTreeBranch() failed");
                            utility.WriteOut();
                            return;
                        }

                        // set height assume user has matched the list lengths
                        if (heights != null) {
                            csvWriter.SetFamilyExtrusionHeight(heights[i]);
                            instructionData.Add("Set Height:");
                        }

                        if (placementPnt.Count == 0 & famName == "") {
                            csvWriter.AddFamilyExtrusion(curvesListListRevit);
                            instructionData.Add("Add Family Extrusion:");
                        }
                        else if (placementPnt.Count == 0 & famName != "") {
                            csvWriter.AddFamilyExtrusion(curvesListListRevit, famName);
                            instructionData.Add("Add Family Extrusion:");
                        }
                        else if (placementPnt.Count != 0 & famName == "") {
                            HbXYZ hbXYZ = new HbXYZ(placementPnt[i].Value.X, placementPnt[i].Value.Y, placementPnt[i].Value.Z);
                            csvWriter.AddFamilyExtrusion(curvesListListRevit, hbXYZ);
                            instructionData.Add("Add Family Extrusion:");
                        }
                        else {
                            HbXYZ hbXYZ = new HbXYZ(placementPnt[i].Value.X, placementPnt[i].Value.Y, placementPnt[i].Value.Z);
                            csvWriter.AddFamilyExtrusion(curvesListListRevit, famName, hbXYZ);
                            instructionData.Add("Add Family Extrusion:");
                        }


                        // Set parameters. Assume user has matched the list lengths.  Error handling silently truncates if they don't match.
                        if (parameterArray != null) {
                            for (int j = 0; j < parameterNames.Count; j++) {
                                if (i < iMaxCountParam && j < jMaxCountParam) {
                                    csvWriter.ModifyParameterSet(parameterNames[j], parameterArray[i, j]);
                                    instructionData.Add("Set Param: " + parameterNames[j] + ", " + parameterArray[i, j]);
                                }
                            }
                        }

                    }
                    csvWriter.WriteFile();
                    utility.Print("Mass Family Extrusions completed successfully.");
                    
                }
                catch (Exception exception) {
                    utility.Print(exception.Message);
                }
            }
            utility.WriteOut();
            DA.SetDataList(1, instructionData);
        }

        // ------------------------------------------------------- General housekeeping for all components ----------------------------------------------------------

        public override GH_Exposure Exposure {
            get { return GH_Exposure.tertiary; }
        }
        public override Guid ComponentGuid {
            get { return new Guid("9F943D4F-14AF-4BA7-A9BC-DD15E84F2EF6"); }
        }
        protected override Bitmap Icon  {
            get { return new Bitmap(Hummingbird.Properties.Resources.MassFamily); }
        }

    }
}
