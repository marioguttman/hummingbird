using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HummingbirdUtility;

namespace Hummingbird {

    public class LoftForms : GH_Component {

        private string DEFAULT_FILE_NAME = "LoftForms.csv";  // "const" not allowed with GH

//TODO: Why do we have a "None" option?
        private enum LoftMode {
            None,
            Points,
            Lines,
            Loft
        }
        private LoftMode loftMode;

        public LoftForms() : base("LoftForms", "LoftForms", "Add LoftForm from Reference Points", "Extra", "Hummingbird") {
            this.loftMode = LoftMode.Points;
            this.Menu_PointsClicked(null, null);
            this.Message = "Ref Points";
            this.Hidden = false;
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
            ToolStripMenuItem item = GH_DocumentObject.Menu_AppendItem(menu, "Ref Points", new EventHandler(this.Menu_PointsClicked), true, this.loftMode == LoftMode.Points);
            ToolStripMenuItem item2 = GH_DocumentObject.Menu_AppendItem(menu, "Lines", new EventHandler(this.Menu_LinesClicked), true, this.loftMode == LoftMode.Lines);
            ToolStripMenuItem item3 = GH_DocumentObject.Menu_AppendItem(menu, "Surface", new EventHandler(this.Menu_LoftClicked), true, this.loftMode == LoftMode.Loft);

            item.ToolTipText = "Create Revit Reference Points";
            item2.ToolTipText = "Create Revit Curve by Points";
            item3.ToolTipText = "Create Revit Surface by Curves on Points";
        }

        private void Menu_PointsClicked(object sender, EventArgs e) {
            if (this.loftMode != LoftMode.Points) {
                this.RecordUndoEvent("Ref Points");
                this.loftMode = LoftMode.Points;
                this.ExpireSolution(true);
            }
        }

        private void Menu_LinesClicked(object sender, EventArgs e) {
            if (this.loftMode != LoftMode.Lines)  {
                this.RecordUndoEvent("Lines");
                this.loftMode = LoftMode.Lines;
                this.ExpireSolution(true);
            }
        }
        private void Menu_LoftClicked(object sender, EventArgs e) {
            if (this.loftMode != LoftMode.Loft) {
                this.RecordUndoEvent("Loft");
                this.loftMode = LoftMode.Loft;
                this.ExpireSolution(true);
            }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);                      // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                             // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);                            // 2
            pManager.AddPointParameter("Points", "Points", "A Tree of Placement Points for Curves for a single LoftForm", GH_ParamAccess.tree); // 3
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            bool write = false;
            string folderPath = null;
            string fileName = null;
            GH_Structure<GH_Point> points = null;
            List<string> instructionData = new List<string>();     

            if (DA.Iteration == 0)
            {
                switch (this.loftMode)
                {
                    case LoftMode.None:
                        this.Message = "Default";
                        break;

                    case LoftMode.Points:
                        this.Message = "Ref Points";
                        break;

                    case LoftMode.Lines:
                        this.Message = "Lines";
                        break;

                    case LoftMode.Loft:
                        this.Message = "Surface";
                        break;
                }
            }

            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting Loft Form.");

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
            if (!utility.GetInput(3, out points)) return;

            if (write) {
                // Create RevitModelBuilderUtility object and link to CSV file            
                CsvWriter csvWriter = new CsvWriter();
                utility.Print("CsvWriter Version: " + csvWriter.Version);
                

                if (!utility.EstablishCsvLink(csvWriter, folderPath, fileName)) {
                    utility.Print("EstablishCsvLink() failed");
                    utility.WriteOut();
                    return;
                }
                switch (this.loftMode) {
                    case LoftMode.Loft:
                        // Loop for each branch, get points, and create form
                        List<List<HbXYZ>> pointsListListRevit = new List<List<HbXYZ>>();
                        if (!utility.ReadDataTree(points, ref pointsListListRevit)) return;
                        csvWriter.AddLoftForm(pointsListListRevit);
                        instructionData.Add("Add Loft Form:");
                        csvWriter.WriteFile();
                        utility.Print("Loft Form completed successfully.");
                        break;
                    case LoftMode.Lines:
                        // Loop for each branch, get points, and place curve
                        for (int i = 0; i < points.Branches.Count; i++)  {
                            List<GH_Point> branch = points.Branches[i];
                            List<HbXYZ> pointsListRevit = new List<HbXYZ>();
                            for (int j = 0; j < branch.Count; j++) {
                                HbXYZ hbXYZ = new HbXYZ(branch[j].Value.X, branch[j].Value.Y, branch[j].Value.Z);
                                pointsListRevit.Add(hbXYZ);
                            }
                            csvWriter.AddCurveByPoints(pointsListRevit);
                            instructionData.Add("Add Curve by Points:");
                        }
                        csvWriter.WriteFile();
                        utility.Print("Curves By Points completed successfully.");
                        break;
                    case LoftMode.Points:
                        List<GH_Point> points_list = null;
                        points_list = points.FlattenData();
                        // Loop for each point and place reference point.
                        for (int i = 0; i < points_list.Count; i++) {
                            HbXYZ hbXYZ = new HbXYZ(points_list[i].Value.X, points_list[i].Value.Y, points_list[i].Value.Z);
                            csvWriter.AddReferencePoint(hbXYZ);
                            instructionData.Add("Add Reference Point:");
                        }
                        csvWriter.WriteFile();
                        utility.Print("Reference Points completed successfully.");
                        break;
                }
             }
            utility.WriteOut();
            DA.SetDataList(1, instructionData);
        }


        // ------------------------------------------------------- General housekeeping for all components ----------------------------------------------------------

        // This is an example of making the settings "stickey" using the features of GH_IO.Serialization (see "using" statement above).  The writer saves the value and the reader restores it.
        // Note that we have overwritten the base.Write and base.Read functions so we call them after restoring our custom values.
        public override bool Write(GH_IWriter writer) {
            writer.SetInt32("LoftMode", (int)this.loftMode);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader) {
            int num = 2;
            reader.TryGetInt32("LoftMode", ref num);
            this.loftMode = (LoftMode)num;
            return base.Read(reader);
        }        

        public override GH_Exposure Exposure {
            get { return GH_Exposure.tertiary;
            }
        }

        public override Guid ComponentGuid {
            get { return new Guid("D49CCF46-5B1B-4770-9EC3-713888FAD594"); }
        }

        protected override Bitmap Icon {
            get { return new Bitmap(Hummingbird.Properties.Resources.LoftForm); }
        }

    }
}
