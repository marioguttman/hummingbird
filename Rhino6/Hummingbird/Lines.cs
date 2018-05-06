using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GH_IO.Serialization;

using HummingbirdUtility;

namespace Hummingbird
{
    public class Lines : GH_Component
    {
        private string DEFAULT_FILE_NAME = "Lines.csv";  // "const" not allowed with GH


// TODO: Why do we have a "None" option?
        private enum LineMode {
            None,
            ModelLine,
            DetailLine,
            AreaBoundLine,
            RoomSepLine
        }
        private LineMode lineMode;

        public Lines(): base("Lines", "Lines", "Add Revit Lines", "Extra", "Hummingbird") {
            this.lineMode = LineMode.DetailLine;
            this.Menu_DetailClicked(null, null);
            this.Message = "Detail Lines";
            this.Hidden = false;
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
            ToolStripMenuItem item = GH_DocumentObject.Menu_AppendItem(menu, "Detail Lines", new EventHandler(this.Menu_DetailClicked), true, this.lineMode == LineMode.DetailLine);
            ToolStripMenuItem item2 = GH_DocumentObject.Menu_AppendItem(menu, "Model Lines", new EventHandler(this.Menu_ModelClicked), true, this.lineMode == LineMode.ModelLine);
            ToolStripMenuItem item3 = GH_DocumentObject.Menu_AppendItem(menu, "Area Boundary Lines", new EventHandler(this.Menu_AreaClicked), true, this.lineMode == LineMode.AreaBoundLine);
            ToolStripMenuItem item4 = GH_DocumentObject.Menu_AppendItem(menu, "Room Separation Lines", new EventHandler(this.Menu_RoomClicked), true, this.lineMode == LineMode.RoomSepLine);             
            item.ToolTipText = "Create Revit Detail Lines.  This will project lines to current view's workplane.";
            item2.ToolTipText = "Create Revit Model Lines";
            item3.ToolTipText = "Create Revit Area Boundary Lines";
            item4.ToolTipText = "Create Revit Room Separation Lines";
        }

        private void Menu_DetailClicked(object sender, EventArgs e) {
            if (this.lineMode != LineMode.DetailLine) {
                this.RecordUndoEvent("Detail Line");
                this.lineMode = LineMode.DetailLine;
                this.ExpireSolution(true);
            }
        }
        private void Menu_ModelClicked(object sender, EventArgs e) {
            if (this.lineMode != LineMode.ModelLine) {
                this.RecordUndoEvent("Model Line");
                this.lineMode = LineMode.ModelLine;
                this.ExpireSolution(true);
            }
        }
        private void Menu_AreaClicked(object sender, EventArgs e) {
            if (this.lineMode != LineMode.AreaBoundLine) {
                this.RecordUndoEvent("Area Boundary Line");
                this.lineMode = LineMode.AreaBoundLine;
                this.ExpireSolution(true);
            }
        }
        private void Menu_RoomClicked(object sender, EventArgs e) {
            if (this.lineMode != LineMode.RoomSepLine) {
                this.RecordUndoEvent("Room Separation Line");
                this.lineMode = LineMode.RoomSepLine;
                this.ExpireSolution(true);
            }
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)  {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);   // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                          // 1         
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);         // 2          
            pManager[2].Optional = true;
            pManager.AddCurveParameter("Curves", "Curves", "Rhino curves to convert to Revit lines", GH_ParamAccess.tree);   // 3
            //pManager[3].Optional = true;
            
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {            
            if (DA.Iteration == 0)  {
                switch (this.lineMode)  {
                    case LineMode.None:
                        this.Message = "Revit Lines";
                        break;
                    case LineMode.DetailLine:
                        this.Message = "Detail Lines";
                        break;
                    case LineMode.ModelLine:
                        this.Message = "Model Lines";
                        break;                    
                    case LineMode.AreaBoundLine:
                        this.Message = "Area Bndry Lines";
                        break;
                    case LineMode.RoomSepLine:
                        this.Message = "Room Sep Lines";
                        break;
                }
            }
            bool write = false;
            string folderPath = null;
            string fileName = null;
            GH_Structure<GH_Curve> dataTree = null;               // curves in Data Tree required
            List<string> instructionData = new List<string>();     

            // Set up Utility object and start process
            Utility utility = new Utility(DA);

            // Start input
            utility.Print("Initializing.");

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
            if (!utility.GetInput(3, out dataTree)) {
                utility.WriteOut();
                return;
            }

            if (write) {

                try  {
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
                        if (!utility.ReadDataTreeBranch(dataTree.Branches[i], ref curvesListListRevit)) {
                            utility.Print("ReadDataTreeBranch() failed");
                            utility.WriteOut();
                            return;
                        }
                    }

                    for (int j = 0; j < curvesListListRevit.Count; j++) {
                        List<HbCurve> curvesListRevit = curvesListListRevit[j];
                        switch (this.lineMode) {
                            case LineMode.DetailLine:
                                csvWriter.AddDetailCurves(curvesListRevit);
                                instructionData.Add("Add Detail Line: " + utility.formatCurves(curvesListRevit));
                                break;
                            case LineMode.ModelLine:
                                csvWriter.AddModelCurves(curvesListRevit);
                                instructionData.Add("Add Model Line: " + utility.formatCurves(curvesListRevit));
                                break;
                            case LineMode.AreaBoundLine:
                                csvWriter.AddAreaBoundaryLine(curvesListRevit);
                                instructionData.Add("Add Area Bndry Line: " + utility.formatCurves(curvesListRevit));
                                break;
                            case LineMode.RoomSepLine:
                                csvWriter.AddRoomSeparationLine(curvesListRevit);
                                instructionData.Add("Add Room Sep Line: " + utility.formatCurves(curvesListRevit));
                                break;
                            default:
                                break;
                        }
                    }
                    utility.Print("Add Lines completed successfully.");
                    csvWriter.WriteFile();
                }
                catch (Exception exception) {
                    utility.Print(exception.Message);
                }
            }
            utility.WriteOut();
            DA.SetDataList(1, instructionData);
        }

        // ------------------------------------------------------- General housekeeping for all components ----------------------------------------------------------

        // This is an example of making the settings "stickey" using the features of GH_IO.Serialization (see "using" statement above).  The writer saves the value and the reader restores it.
        // Note that we have overwritten the base.Write and base.Read functions so we call them after restoring our custom values.
        public override bool Write(GH_IWriter writer) {
            writer.SetInt32("LineMode", (int)this.lineMode);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader) {
            int num = 2;
            reader.TryGetInt32("LineMode", ref num);
            this.lineMode = (LineMode)num;
            return base.Read(reader);
        }

        public override Guid ComponentGuid {
            get { return new Guid("8519AE2F-2546-499F-A5A2-441AD88E142D"); }
        }

        public override GH_Exposure Exposure {
            get { return GH_Exposure.primary; }
        }

        protected override Bitmap Icon {
            get { return new Bitmap(Hummingbird.Properties.Resources.Lines); }
        }

    }
}
