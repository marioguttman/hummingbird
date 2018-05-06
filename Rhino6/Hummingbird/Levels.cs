using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using HummingbirdUtility;

namespace Hummingbird {

    public class Levels : GH_Component {

        private string DEFAULT_FILE_NAME = "Levels.csv";  // "const" not allowed with GH

        public Levels() : base("Levels", "Levels", "Add Revit Levels", "Extra", "Hummingbird") {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);                                            // 0                                 
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                                                   // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);                                                  // 2
            pManager[2].Optional = true;
            pManager.AddNumberParameter("LevelElevation", "Elev", "Rhino curves to convert to Revit grid lines, Only works with Lines & Arcs", GH_ParamAccess.list);  // 3                     // 3
            pManager[3].Optional = true;
            pManager.AddTextParameter("LevelName", "Name", "Revit Level Name. Command will fail if level name already exists.", GH_ParamAccess.list);                 // 4                     
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting Levels.");

            // Get Inputs
            string folderPath = null, fileName = null;
            bool write = false;
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

            List<double> elevList = null;               // List of elevations
            if (!utility.GetInput(3, ref elevList)) return;

            List<string> levelName = null;             //  Level Names /optional
            utility.GetInput(4, ref levelName);

            if (write) {

                if (levelName.Count != 0) {
                    if (elevList.Count != levelName.Count) {
                        utility.Print("List of level names does not match length of level elevations list.");
                        utility.WriteOut();
                        return;
                    }
                }

                try {
                    // Create RevitModelBuilderUtility object and link to CSV file            
                    CsvWriter csvWriter = new CsvWriter();
                    utility.Print("CsvWriter Version: " + csvWriter.Version);
                    if (!utility.EstablishCsvLink(csvWriter, folderPath, fileName)) {
                        utility.Print("EstablishCsvLink() failed");
                        utility.WriteOut();
                        return;
                    }


                    // Loop through the data list of heights and names.
                    for (int i = 0; i < elevList.Count; i++) {
                        if (levelName.Count == 0) {
                            csvWriter.AddLevel(elevList[i]);                            
                            instructionData.Add("Add Level:" + elevList[i].ToString());
                        }
                        else {
                            csvWriter.AddLevel(elevList[i], levelName[i]);
                            instructionData.Add("Add Level:" + elevList[i].ToString() + ", " + levelName[i]);
                        }
                    }
                    csvWriter.WriteFile();
                    utility.Print("Levels completed successfully.");
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
            get { return new Guid("B87B8D33-16EB-4235-B07D-7CC1E016242A"); }
        }

        protected override Bitmap Icon {
            get { return new Bitmap(Hummingbird.Properties.Resources.Level); }
        }
    }
}
