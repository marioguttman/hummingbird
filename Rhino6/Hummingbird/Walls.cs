using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HummingbirdUtility;

namespace Hummingbird
{
    public class Walls : GH_Component {

        private string DEFAULT_FILE_NAME = "Walls.csv";  // "const" not allowed with GH

        public Walls() : base("Walls", "Walls", "WhiteFeet ModelBuilder - Add Walls", "Extra", "Hummingbird") {            
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);      // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                             // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);            // 2
            pManager[2].Optional = true;
            pManager.AddCurveParameter("Curves", "Curves", "A Tree of Curves for Wall Instances", GH_ParamAccess.tree);         // 3
            pManager[3].Optional = true;
            pManager.AddTextParameter("RevitTypeName", "Type", "Revit Wall Type Name", GH_ParamAccess.item);                    // 4
            pManager[4].Optional = true;
            pManager.AddNumberParameter("RevitWallHeight", "Height", "Set Revit Wall Height",GH_ParamAccess.item);              // 5
            pManager[5].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting Wall By Points.");
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

            GH_Structure<GH_Curve> dataTree = null;                  // curves in Data Tree required
            if (!utility.GetInput(3, out dataTree)) return;

            string typeName = null;                                  // Type is optional (no family, just type)
            utility.GetInput(4, ref typeName);
          
            double wallHeight = 0;                                   // Wall Height is optional
            utility.GetInput(5, ref wallHeight);

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
                    if (typeName != null) {
                        csvWriter.SetWallType(typeName);
                    }

                    //Set Wall Height
                    double revitWallHeight = 10.0;
                    if (wallHeight != 0) {
                        GH_Convert.ToDouble(wallHeight, out revitWallHeight, GH_Conversion.Both);
                        csvWriter.SetWallHeight(revitWallHeight);
                    }

                    List<List<HbCurve>> curvesListListRevit = new List<List<HbCurve>>();
                    // Loop through the data tree of curves and process each one.
                    for (int i = 0; i < dataTree.Branches.Count; i++) {
                        if (!utility.ReadDataTreeBranch(dataTree.Branches[i], ref curvesListListRevit)) {
                            utility.Print("ReadDataTreeBranch() failed");
                            utility.WriteOut();
                            return;
                        }
                    }

                    for (int j = 0; j < curvesListListRevit.Count; j++) {
                        List<HbCurve> curvesListRevit = curvesListListRevit[j];
                        csvWriter.AddWall(curvesListRevit);
                        instructionData.Add("Add Wall:");
                    }
                    csvWriter.WriteFile();
                    utility.Print("Add Wall completed successfully.");
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
            get { return new Guid("AEF061F8-52B1-480E-B6BF-94F9B81E1064"); }
        }
        protected override Bitmap Icon {
            get { return new Bitmap(Hummingbird.Properties.Resources.Wall); }
        }

    }
}
