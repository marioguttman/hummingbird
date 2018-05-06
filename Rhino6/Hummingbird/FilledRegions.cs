using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HummingbirdUtility;

namespace Hummingbird
{
    public class FilledRegions : GH_Component
    {
        private string DEFAULT_FILE_NAME = "FilledRegions.csv";  // "const" not allowed with GH

        public FilledRegions() : base("FilledRegions", "FilledRegions", "Add Revit FilledRegions", "Extra", "Hummingbird") {
            this.Message = "FilledRegions";
            this.Hidden = false;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);                              // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                                     // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);                                    // 2
            pManager[2].Optional = true;
            pManager.AddTextParameter("RevitTypeName", "Type", "Revit Filled Region Type Name", GH_ParamAccess.item);                                   // 3
            pManager[3].Optional = true;
            pManager.AddCurveParameter("DataTreeSource", "Curves", "A Tree of Curves Defining Filled Regions", GH_ParamAccess.tree);                    // 4
            pManager[4].Optional = true;
            pManager.AddTextParameter("ParameterNames", "Params", "A list of parameter names to set for each filled region", GH_ParamAccess.list);      // 5
            pManager[4].Optional = true;
            pManager.AddTextParameter("ParameterValues", "Values", "A tree of parameter values for each filled region instance", GH_ParamAccess.tree);  // 6
            pManager[5].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)  {

            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting " + this.Message + ".");
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

            string typeName = null;                                  // Filled Region type is optional
            utility.GetInput(3, ref typeName);

            GH_Structure<GH_Curve> dataTree = null;                  // Points in Data Tree required
            if (!utility.GetInput(4, out dataTree)) return;

            List<string> parameterNames = null;                      // Parameter names list and values tree are optional but both must be provided if used.
            GH_Structure<GH_String> parameterValues = null;
            string[,] parameterArray = null;
            utility.GetParameterValueArray(5, 6, ref parameterNames, out parameterValues, out parameterArray);
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
                    if (typeName != null) {
                        csvWriter.SetFilledRegionType(typeName);
                        instructionData.Add("Set FilledRegionType: " + typeName);
                    }

                    // Loop through the data tree of curves and process each one.
                    for (int i = 0; i < dataTree.Branches.Count; i++) {

                        // Add the filled region
                        List<List<HbCurve>> curvesListListRevit = new List<List<HbCurve>>();
                        if (!utility.ReadDataTreeBranch(dataTree.Branches[i], ref curvesListListRevit)) return;
                        csvWriter.AddFilledRegion(curvesListListRevit);
                        instructionData.Add("Add FilledRegion:");

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
                    utility.Print("Filled Regions completed successfully.");
                }
                catch (Exception exception) {
                    utility.Print(exception.Message);

                    return;
                }
            }
            utility.WriteOut();
            DA.SetDataList(1, instructionData);
        }

        // ------------------------------------------------------- General housekeeping for all components ----------------------------------------------------------

        public override Guid ComponentGuid {
            get { return new Guid("459E42DE-72A2-40CF-A928-374C5961C579"); }
        }

        public override GH_Exposure Exposure {
            get { return GH_Exposure.primary; }
        }

        protected override Bitmap Icon { 
            get { return new Bitmap(Hummingbird.Properties.Resources.FilledRegions); }
        }


    }
}
