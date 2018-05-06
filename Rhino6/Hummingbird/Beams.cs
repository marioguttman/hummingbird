using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HummingbirdUtility;

namespace Hummingbird {
    public class Beams : GH_Component {

        private string DEFAULT_FILE_NAME = "Beams.csv";  // "const" not allowed with GH

        public Beams() : base("Beams", "Beams", "Add Revit Beams", "Extra", "Hummingbird") {            
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);                  // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                         // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);                        // 2
            pManager[2].Optional = true;
            pManager.AddTextParameter("RevitFamilyName", "Family", "Revit Beam Family Name", GH_ParamAccess.item);                          // 3
            pManager[3].Optional = true;
            pManager.AddTextParameter("RevitTypeName", "Type", "Revit Beam Type Name", GH_ParamAccess.item);                                // 4
            pManager[4].Optional = true;
            pManager.AddPointParameter("Points", "Points", "A Tree of Placement Points for Beams", GH_ParamAccess.tree);                    // 5
            pManager[5].Optional = true;
            pManager.AddNumberParameter("Rotations", "Rotate", "A List of Rotation Values for Beam Instances", GH_ParamAccess.list);        // 6
            pManager[6].Optional = true;
            pManager.AddTextParameter("ParameterNames", "Params", "A List of Parameter Names to Set for Each Beam", GH_ParamAccess.list);   // 7
            pManager[7].Optional = true;
            pManager.AddTextParameter("ParameterValues", "Values", "A Tree of Parameter Values for Beam Instances", GH_ParamAccess.tree);   // 8
            pManager[8].Optional = true;     
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting Beams.");
            List<string> instructionData = new List<string>();     

            // Get Inputs
            string folderPath = null, fileName = null;            
            bool write = false;
            if (!utility.GetInput(0, ref write)) {                   // 0 - Write command is required
                utility.WriteOut();
                return;
            }
            if (!utility.GetInput(1, ref folderPath)) {              // 1 - Folder path is required
                utility.WriteOut();
                return;
            }
            utility.GetInput(2, ref fileName, true, true, true);     // 2 - File name is optional
            if (fileName == null) fileName = this.DEFAULT_FILE_NAME;

            string familyName = null, typeName = null;               // 3, 4 - Family and type are optional but both must be provided if used.
            utility.GetInput(3, ref familyName);
            utility.GetInput(4, ref typeName);

            GH_Structure<GH_Point> points = null;                    // 5 - Points in Data Tree required
            if (!utility.GetInput(5, out points)) return;

            List<double> rotations = null;                           // 6 - Rotations list optional
            utility.GetInput(6, ref rotations);
            int iMaxCountRotate = 0;
            if (rotations != null) iMaxCountRotate = rotations.Count;

            List<string> parameterNames = null;                      // 7, 8 - Parameter names list and values tree are optional but both must be provided if used.
            GH_Structure<GH_String> parameterValues = null;
            string[,] parameterArray = null;
            utility.GetParameterValueArray(7, 8, ref parameterNames, out parameterValues, out parameterArray);
            int iMaxCountParam = 0, jMaxCountParam = 0;
            if (parameterArray != null)
            {
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
                    if (familyName != null && typeName != null) csvWriter.SetBeamType(familyName, typeName);

                    // Loop for each beam, get points, and place component; then set parameter values
                    double lastRotationValue = 0;
                    for (int i = 0; i < points.Branches.Count; i++) {

                        // Set rotation if necessary
                        if (rotations != null) {
                            if (i < iMaxCountRotate) {
                                if (Math.Abs(lastRotationValue - rotations[i]) > 0.00000001) {
                                    csvWriter.SetBeamRotation(rotations[i]);
                                    lastRotationValue = rotations[i];
                                }
                            }
                        }

                        // Add the beam
                        List<List<HbCurve>> curvesListListRevit = new List<List<HbCurve>>();
                        List<Grasshopper.Kernel.Types.GH_Point> branch = points.Branches[i];
                        HbXYZ hbXYZ1 = new HbXYZ(branch[0].Value.X, branch[0].Value.Y, branch[0].Value.Z);
                        HbXYZ hbXYZ2 = new HbXYZ(branch[1].Value.X, branch[1].Value.Y, branch[1].Value.Z);
                        csvWriter.AddBeam(hbXYZ1, hbXYZ2);

                        // Set parameters if needed. Assume user has matched the list lengths.  Error handling silently truncates if they don't match.
                        if (parameterArray != null) {
                            for (int j = 0; j < parameterNames.Count; j++) {
                                if (i < iMaxCountParam && j < jMaxCountParam) {
                                    csvWriter.ModifyParameterSet(parameterNames[j], parameterArray[i, j]);
                                    instructionData.Add("Add Beam:");
                                }
                            }
                        }
                    }

                    csvWriter.WriteFile();
                    utility.Print("Beams completed successfully.");
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
            get { return GH_Exposure.secondary; }
        }

        public override Guid ComponentGuid {
            get { return new Guid("AB987ECE-B3E0-433F-9EDC-BDB7C09A7CC3"); }
        }
        protected override Bitmap Icon {
            get { return new Bitmap(Hummingbird.Properties.Resources.Beam); }
        }

    }
}
