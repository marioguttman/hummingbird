
using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HummingbirdUtility;

namespace Hummingbird {
    public class AdapComps : GH_Component {

        private string DEFAULT_FILE_NAME = "AdapComps.csv";  // "const" not allowed with GH

        public AdapComps() : base("AdapComps", "AdapComps", "Add Revit Adaptive Components", "Extra", "Hummingbird") {            
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);                      // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                             // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);                            // 2
            pManager[2].Optional = true;
            pManager.AddTextParameter("RevitFamilyName", "Family", "Revit Adaptive Component Family Name", GH_ParamAccess.item);                // 3
            pManager[3].Optional = true;
            pManager.AddTextParameter("RevitTypeName", "Type", "Revit Adaptive Component Type Name", GH_ParamAccess.item);                      // 4
            pManager[4].Optional = true;
            pManager.AddPointParameter("Points", "Points", "A Tree of Placement Points for Adaptive Component", GH_ParamAccess.tree);           // 5
            pManager[5].Optional = true;
            pManager.AddTextParameter("ParameterNames", "Params", "A List of Parameter Names to Set for Each Component", GH_ParamAccess.list);  // 7
            pManager[6].Optional = true;
            pManager.AddTextParameter("ParameterValues", "Values", "A Tree of Parameter Values for Component Instances", GH_ParamAccess.tree);  // 7
            pManager[7].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting Adaptive Components.");
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

            string familyName = null, typeName = null;             // Family and type are optional but both must be provided if used.
            utility.GetInput(3, ref familyName);
            utility.GetInput(4, ref typeName);

            GH_Structure<GH_Point> points = null;                  // Points in Data Tree required
            if (!utility.GetInput(5, out points)) return;

            List<string> parameterNames = null;                    // Parameter names list and values tree are optional but both must be provided if used.
            GH_Structure<GH_String> parameterValues = null;
            string[,] parameterArray = null;
            utility.GetParameterValueArray(6, 7, ref parameterNames, out parameterValues, out parameterArray);
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

                    // Set Family and Type
                    if (familyName != null && typeName != null) csvWriter.SetAdaptiveComponentType(familyName, typeName);

                    // Loop for each adaptive component, get points, and place component; then set parameter values
                    for (int i = 0; i < points.Branches.Count; i++) {

                        // Add Adaptive Component
                        List<GH_Point> branch = points.Branches[i];
                        HbXYZ hbXYZ1, hbXYZ2, hbXYZ3, hbXYZ4;
                        if (branch.Count <= 4) {
                            hbXYZ1 = new HbXYZ(branch[0].Value.X, branch[0].Value.Y, branch[0].Value.Z);
                            if (branch.Count > 1) hbXYZ2 = new HbXYZ(branch[1].Value.X, branch[1].Value.Y, branch[1].Value.Z);
                            else hbXYZ2 = null;
                            if (branch.Count > 2) hbXYZ3 = new HbXYZ(branch[2].Value.X, branch[2].Value.Y, branch[2].Value.Z);
                            else hbXYZ3 = null;
                            if (branch.Count > 3) hbXYZ4 = new HbXYZ(branch[3].Value.X, branch[3].Value.Y, branch[3].Value.Z);
                            else hbXYZ4 = null;
                            csvWriter.AddAdaptiveComponent(hbXYZ1, hbXYZ2, hbXYZ3, hbXYZ4);
                            instructionData.Add("Add Adaptive Component:");
                        }
                        else {
                            List<HbXYZ> pointsListRevit = new List<HbXYZ>();
                            for (int j = 0; j < branch.Count; j++) {
                                HbXYZ hbXYZ = new HbXYZ(branch[j].Value.X, branch[j].Value.Y, branch[j].Value.Z);
                                pointsListRevit.Add(hbXYZ);
                            }
                            csvWriter.AddAdaptiveComponent(pointsListRevit);
                            instructionData.Add("Add Adaptive ComponentX:");
                        }

                        // Set parameters if needed. Assume user has matched the list lengths.  Error handling silently truncates if they don't match.
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
                    utility.Print("Adaptive Components completed successfully.");
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
            get { return new Guid("5F30997B-797C-45BE-A82B-2EA72C4559AD"); }
        }

        protected override Bitmap Icon {
            get { return new Bitmap(Hummingbird.Properties.Resources.Adap); }
        }

    }
}
