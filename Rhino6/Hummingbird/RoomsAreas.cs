using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HummingbirdUtility;

namespace Hummingbird
{
    public class RoomsAreas : GH_Component
    {
        private string DEFAULT_FILE_NAME = "RoomsAreas.csv";  // "const" not allowed with GH

// TODO: Why do we have "None" option?
        private enum AreaMode {
            None,
            Areas,
            Rooms
        }
        private AreaMode areaMode;

        public RoomsAreas(): base("Rooms/Areas", "Rooms/Areas", "Add Revit Rooms & Areas", "Extra", "Hummingbird") {
            this.areaMode = AreaMode.Areas;
            this.Menu_AreaClicked(null, null);
            this.Message = "Areas";
            this.Hidden = false;
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
            ToolStripMenuItem item = GH_DocumentObject.Menu_AppendItem(menu, "Areas", new EventHandler(this.Menu_AreaClicked), true, this.areaMode == AreaMode.Areas);
            ToolStripMenuItem item2 = GH_DocumentObject.Menu_AppendItem(menu, "Rooms", new EventHandler(this.Menu_RoomClicked), true, this.areaMode == AreaMode.Rooms);
            item.ToolTipText = "Create Revit Areas.";
            item2.ToolTipText = "Create Revit Rooms.";
        }

        private void Menu_AreaClicked(object sender, EventArgs e) {
            if (this.areaMode != AreaMode.Areas) {
                this.RecordUndoEvent("Areas");
                this.areaMode = AreaMode.Areas;
                this.ExpireSolution(true);
            }
        }

        private void Menu_RoomClicked(object sender, EventArgs e) {
            if (this.areaMode != AreaMode.Rooms) {
                this.RecordUndoEvent("Rooms");
                this.areaMode = AreaMode.Rooms;
                this.ExpireSolution(true);
            }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);// 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                   // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);                    // 2
            pManager[2].Optional = true;
            pManager.AddPointParameter("Points", "Points", "A list of Placement Points for Rooms or Areas", GH_ParamAccess.tree);                        // 3
            pManager[3].Optional = true;
            pManager.AddTextParameter("ParameterNames", "Params", "A list of parameter names to set for each room or area", GH_ParamAccess.list);     // 4
            pManager[4].Optional = true;
            pManager.AddTextParameter("ParameterValues", "Values", "A tree of parameter values for each room or area instance", GH_ParamAccess.tree);     // 5
            pManager[5].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)  {
            if (DA.Iteration == 0) {
                switch (this.areaMode) {
                    case AreaMode.None:
                        this.Message = "Select Mode";
                        break;
                    case AreaMode.Rooms:
                        this.Message = "Rooms";
                        break;

                    case AreaMode.Areas:
                        this.Message = "Areas";
                        break;
                }
            }

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

           
            GH_Structure<GH_Point> points = null;               // Points in Data Tree required
            if (!utility.GetInput(3, out points)) return;

            List<string> parameterNames = null;                 // Parameter names list and values tree are optional but both must be provided if used.
            GH_Structure<GH_String> parameterValues = null;
            string[,] parameterArray = null;
            utility.GetParameterValueArray(4, 5, ref parameterNames, out parameterValues, out parameterArray);
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

                    // Loop for each room, get points, and place component; then set parameter values
                    for (int i = 0; i < points.Branches.Count; i++) {
                        // Add the room
                        List<List<HbCurve>> curvesListListRevit = new List<List<HbCurve>>();
                        List<Grasshopper.Kernel.Types.GH_Point> branch = points.Branches[i];
                        HbXYZ hbXYZ = new HbXYZ(branch[0].Value.X, branch[0].Value.Y, branch[0].Value.Z);

                        //switch on room or area
                        switch (this.areaMode) {
                            case AreaMode.Rooms: {
                                    csvWriter.AddRoom(hbXYZ);
                                    instructionData.Add("Add Room: " + utility.formatPoint(hbXYZ));
                                    break;
                                }

                            case AreaMode.Areas: {
                                    csvWriter.AddArea(hbXYZ);
                                    instructionData.Add("Add Room: " + utility.formatPoint(hbXYZ));
                                    break;
                                }

                            default: {
                                    break;
                                }
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
                    utility.Print("Rooms_Areas completed successfully.");
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

        // This is an example of making the settings "stickey" using the features of GH_IO.Serialization (see "using" statement above).  The writer saves the value and the reader restores it.
        // Note that we have overwritten the base.Write and base.Read functions so we call them after restoring our custom values.
        public override bool Write(GH_IWriter writer) {
            writer.SetInt32("AreaMode", (int)this.areaMode);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader) {
            int num = 2;
            reader.TryGetInt32("AreaMode", ref num);
            this.areaMode = (AreaMode)num;
            return base.Read(reader);
        }

        public override Guid ComponentGuid {
            get { return new Guid("523AC678-EC28-4049-9826-C10FCDDB440B"); }
        }

        public override GH_Exposure Exposure {
            get { return GH_Exposure.primary; }
        }

        protected override Bitmap Icon { 
            get { return new Bitmap(Hummingbird.Properties.Resources.RoomsAreas); }
        }


    }
}
