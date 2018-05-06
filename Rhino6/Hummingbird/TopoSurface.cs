using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Rhino;
using Rhino.Geometry;

using HummingbirdUtility;

namespace Hummingbird {

    public class TopoSurface : GH_Component {

        private string DEFAULT_FILE_NAME = "TopoSurface.csv";  // "const" not allowed with GH

        public TopoSurface() : base("TopoSurface", "TopoSurface", "Add Revit Topo Surface", "Extra", "Hummingbird") {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);   // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                          // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);         // 2
            pManager.AddPointParameter("Points", "Points", "A List of Topo Points", GH_ParamAccess.list);                    // 3
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting TopoSurface.");
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

            List<GH_Point> points = null;                            // Points in Data Tree required
            if (!utility.GetInput(3, ref points)) return;

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

                    List<HbXYZ> pointsListRevit = new List<HbXYZ>();

                    // Loop for each topo point tree or list
                    for (int i = 0; i < points.Count; i++) {

                        // Add the point
                        HbXYZ hbXYZ = new HbXYZ(points[i].Value.X, points[i].Value.Y, points[i].Value.Z);
                        pointsListRevit.Add(hbXYZ);
                    }

                    csvWriter.AddTopographySurface(pointsListRevit);
                    instructionData.Add("Add Topo Surface:");
                    csvWriter.WriteFile();
                    utility.Print("TopoSurface completed successfully.");
                }
                catch (Exception exception) {
                    utility.Print(exception.Message);
                }
                utility.WriteOut();
                DA.SetDataList(1, instructionData);
            }
        }

        // ------------------------------------------------------- General housekeeping for all components ----------------------------------------------------------

        public override GH_Exposure Exposure {
            get { return GH_Exposure.tertiary; }
        }

        public override Guid ComponentGuid {
            get { return new Guid("A81FEB0B-3EAF-4703-AF00-815E9F45CEA6"); }
        }
        protected override Bitmap Icon {
            get { return new Bitmap(Hummingbird.Properties.Resources.Topo); }
        }

    }
}
