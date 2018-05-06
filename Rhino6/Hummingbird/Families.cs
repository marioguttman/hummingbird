using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HummingbirdUtility;

namespace Hummingbird {

    public class Families : GH_Component {

        private string DEFAULT_FILE_NAME = "Families.csv";  // "const" not allowed with GH

        public Families() : base("Families", "Families", "Add Revit Families", "Extra", "Hummingbird") {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);                                            // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                                                   // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);                                                  // 2
            pManager[2].Optional = true;
            pManager.AddTextParameter("RevitFamilyName", "Family", "Revit Family Name", GH_ParamAccess.item);                                                         // 3
            pManager[3].Optional = true;
            pManager.AddTextParameter("RevitTypeName", "Type", "Revit Family Type Name", GH_ParamAccess.item);                                                        // 4
            pManager[4].Optional = true;
            pManager.AddPointParameter("Point", "Point", "A Tree of Placement Points for Families, Revit family must be 'work-plane based'", GH_ParamAccess.tree);    // 5
            pManager[5].Optional = true;
            pManager.AddNumberParameter("Rotate", "Rotate", "A List of Rotation Values for Family Instances", GH_ParamAccess.list);                                   // 6
            pManager[6].Optional = true;
            pManager.AddBooleanParameter("Flip Hand", "Flip Hand", "A Boolean to flip hand for Family Instances", GH_ParamAccess.list);                               // 7
            pManager[7].Optional = true;
            pManager.AddBooleanParameter("Flip Facing", "Flip Facing", "A Boolean to flip facing for Family Instances", GH_ParamAccess.list);                         // 8
            pManager[8].Optional = true;
            pManager.AddBooleanParameter("MirrorX", "MirrorX", "A Boolean to Mirror about X-axis for Family Instances", GH_ParamAccess.list);                         // 9
            pManager[9].Optional = true;
            pManager.AddBooleanParameter("MirrorY", "MirrorY", "A Boolean to Mirror about Y-axis for Family Instances", GH_ParamAccess.list);                         // 10
            pManager[10].Optional = true;
            pManager.AddTextParameter("ParameterNames", "Params", "A List of Parameter Names to Set for Each Family", GH_ParamAccess.list);                           // 11
            pManager[11].Optional = true;
            pManager.AddTextParameter("ParameterValues", "Values", "A Tree of Parameter Values for Family Instances", GH_ParamAccess.tree);                           // 12
            pManager[12].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting Families.");
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

            string familyName = null, typeName = null;               // Family and type are optional
            utility.GetInput(3, ref familyName);
            utility.GetInput(4, ref typeName);

            GH_Structure<GH_Point> points = null;                    // Points in Data Tree required
            if (!utility.GetInput(5, out points)) return;

            List<double> rotation = null;                            // Rotation list optional
            utility.GetInput(6, ref rotation);
            int iMaxCountRotate = 0;
            if (rotation != null) iMaxCountRotate = rotation.Count;

            //7-10 flipx,flipy, mirrx,mirry - Booleans to flip and/or mirror family
            List<bool> flipHand = null, flipFace = null;
            List<bool> mirrorX = null, mirrorY = null;
            utility.GetInput(7, ref flipHand);
            utility.GetInput(8, ref flipFace);
            utility.GetInput(9, ref mirrorX);
            utility.GetInput(10, ref mirrorY);

            List<string> parameterNames = null;                      // Parameter names list and values tree are optional but both must be provided if used.
            GH_Structure<GH_String> parameterValues = null;
            string[,] parameterArray = null;
            utility.GetParameterValueArray(11, 12, ref parameterNames, out parameterValues, out parameterArray);
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
                    if (familyName != null && typeName != null) csvWriter.SetFamilyType(familyName, typeName);

                    // Loop for each Family, get points, and place component; then set parameter values
                    double lastRotationValue = 0;
                    bool lastFlipHandValue = false, lastFlipFaceValue = false;
                    bool lastMirrorXValue = false, lastMirrorYValue = false;

                    for (int i = 0; i < points.Branches.Count; i++) {

                        // Set rotation if necessary
                        if (i < iMaxCountRotate) {
                            if (Math.Abs(lastRotationValue - rotation[i]) > 0.00000001) {
                                csvWriter.SetFamilyRotation(rotation[i]);
                                instructionData.Add("Set Rotate: " + rotation[i].ToString());
                                lastRotationValue = rotation[i];
                            }
                        }

                        // Set Flip if necessary
                        if (flipHand.Count != 0 || flipFace.Count != 0) {
                            bool flipHandThis = false;
                            bool flipFaceThis = false;
                            if (i < flipHand.Count) flipHandThis = flipHand[i];
                            if (i < flipFace.Count) flipFaceThis = flipFace[i];
                            if (flipHandThis != lastFlipHandValue || flipFaceThis != lastFlipFaceValue) {
                                csvWriter.SetFamilyFlipped(flipHandThis, flipFaceThis);
                                instructionData.Add("Set Flipped: " + flipHandThis.ToString() + ", " + flipFaceThis.ToString());
                                lastFlipHandValue = flipHandThis;
                                lastFlipFaceValue = flipFaceThis;
                            }
                        }

                        // Set Mirror if necessary
                        if (mirrorX.Count != 0 || mirrorY.Count != 0) {
                            bool mirrorXThis = false;
                            bool mirrorYThis = false;
                            if (i < mirrorX.Count) mirrorXThis = mirrorX[i];
                            if (i < mirrorY.Count) mirrorYThis = mirrorY[i];
                            if (mirrorXThis != lastMirrorXValue || mirrorYThis != lastMirrorYValue) {
                                csvWriter.SetFamilyMirrored(mirrorXThis, mirrorYThis);
                                instructionData.Add("Set Mirrored: " + mirrorXThis.ToString() + ", " + mirrorYThis.ToString());
                                lastMirrorXValue = mirrorXThis;
                                lastMirrorYValue = mirrorYThis;
                            }
                        }

                        // Add the Family
                        List<List<HbCurve>> curvesListListRevit = new List<List<HbCurve>>();
                        List<Grasshopper.Kernel.Types.GH_Point> branch = points.Branches[i];
                        HbXYZ hbXYZ = new HbXYZ(branch[0].Value.X, branch[0].Value.Y, branch[0].Value.Z);
                        csvWriter.AddFamilyInstance(hbXYZ);
                        instructionData.Add("Add Family:");

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
                    utility.Print("Family Instances completed successfully.");

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
            get { return GH_Exposure.tertiary;  }
        }

        public override Guid ComponentGuid  {
            get { return new Guid("5DCCB97E-B4FE-4A2E-B47D-86F1F5E0A68F"); }
        }
        protected override Bitmap Icon  {
            get { return new Bitmap(Hummingbird.Properties.Resources.Family); }
        }

    }
}
