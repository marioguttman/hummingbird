using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HummingbirdUtility;

namespace Hummingbird {

    public class Grids : GH_Component {

        private string DEFAULT_FILE_NAME = "Grids.csv";  // "const" not allowed with GH

        public Grids() : base("Grids", "Grids", "Add Revit Grid Lines", "Extra", "Hummingbird") {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);                                       // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                                              // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);                                             // 2
            pManager[2].Optional = true;
            pManager.AddCurveParameter("Curves", "Curves", "Rhino curves to convert to Revit grid lines, Only works with Lines & Arcs", GH_ParamAccess.tree);    // 3
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting Grids.");

            bool write = false;
            string folderPath = null;
            string fileName = null;
            GH_Structure<GH_Curve> dataTree = null;
            List<string> instructionData = new List<string>();     

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

            if (!utility.GetInput(3, out dataTree)) {                // curves in Data Tree required
                utility.WriteOut();
                return;
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

                    List<List<HbCurve>> curvesListListRevit = new List<List<HbCurve>>();

                    // Loop through the data tree of curves and process each one.
                    for (int i = 0; i < dataTree.Branches.Count; i++) {                       
                        if (!utility.ReadDataTreeBranch(dataTree.Branches[i], ref curvesListListRevit, false)) return;
                    }

                    for (int j = 0; j < curvesListListRevit.Count; j++)  {
                        List<HbCurve> curvesListRevit = curvesListListRevit[j];
                        for (int k = 0; k < curvesListRevit.Count; k++) {
                            if (curvesListRevit[k].GetType().Name == "HbArc")  {
                                HbArc hbArc = (HbArc)curvesListRevit[k];
                                csvWriter.AddGrid(hbArc.PointStart, hbArc.PointEnd, hbArc.PointMid);
                                instructionData.Add("Add Grid-Arc:" + utility.formatPoints(hbArc.PointStart, hbArc.PointEnd, hbArc.PointMid));
                            }                            
                            else if (curvesListRevit[k].GetType().Name == "HbLine") {
                                HbLine hbLine = (HbLine)curvesListRevit[k];
                                csvWriter.AddGrid(hbLine.PointStart, hbLine.PointEnd);
                                instructionData.Add("Add Grid-Line: " + utility.formatPoints(hbLine.PointStart, hbLine.PointEnd));
                            }
                        }
                        csvWriter.WriteFile();                        
                    }
                    utility.Print("Grids completed successfully.");
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
            get { return GH_Exposure.primary; }
        }

        public override Guid ComponentGuid {
            get { return new Guid("3A5F4F28-7D08-4B71-BE0D-EF95E09929F2"); }
        }

        protected override Bitmap Icon  {
            get { return new Bitmap(Hummingbird.Properties.Resources.Grid); }
        }
    }
}
