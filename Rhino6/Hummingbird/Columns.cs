using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GH_IO.Serialization;

using HummingbirdUtility;

namespace Hummingbird {

    // Note: Revit types
    // Architectural
    // StructuralVertical
    // StructuralPointDriven
    // StructuralAngleDriven

    public class Column : GH_Component {

        private string DEFAULT_FILE_NAME = "Columns.csv";  // "const" not allowed with GH

        private enum ColMode {
            None,
            Arch,
            StrucVert,
            StrucPoint,
            StrucAngle
        }
        private ColMode colMode;

        public Column(): base("Columns", "Columns", "Add Revit Columns", "Extra", "Hummingbird") {
            this.colMode = ColMode.Arch;
            this.Hidden = false;
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)  {
            ToolStripMenuItem item = GH_DocumentObject.Menu_AppendItem(menu, "Architectural", new EventHandler(this.Menu_ArchClicked), true, this.colMode == ColMode.Arch);
            ToolStripMenuItem item2 = GH_DocumentObject.Menu_AppendItem(menu, "Structural Vertical", new EventHandler(this.Menu_StrucVertClicked), true, this.colMode == ColMode.StrucVert);
            ToolStripMenuItem item3 = GH_DocumentObject.Menu_AppendItem(menu, "Structural EndPoints", new EventHandler(this.Menu_StrucPointClicked), true, this.colMode == ColMode.StrucPoint);
            ToolStripMenuItem item4 = GH_DocumentObject.Menu_AppendItem(menu, "Structural By Angle", new EventHandler(this.Menu_StrucAngleClicked), true, this.colMode == ColMode.StrucAngle);
            item.ToolTipText = "Create Revit Architectural Columns";
            item2.ToolTipText = "Create Revit Structural Columns, Defined vertically from start point";
            item3.ToolTipText = "Create Revit Structural Columns, Defined by start and end points";
            item4.ToolTipText = "Create Revit Structural Columns, Defined from start point and angle";
        }

        private void Menu_ArchClicked(object sender, EventArgs e) {
            if (this.colMode != ColMode.Arch) {
                this.RecordUndoEvent("Architectural Column Mode");
                this.colMode = ColMode.Arch;
                this.ExpireSolution(true);
            }
        }

        private void Menu_StrucVertClicked(object sender, EventArgs e) {
            if (this.colMode != ColMode.StrucVert) {
                this.RecordUndoEvent("Vertical Structural Column Mode");
                this.colMode = ColMode.StrucVert;
                this.ExpireSolution(true);
            }
        }

        private void Menu_StrucPointClicked(object sender, EventArgs e) {
            if (this.colMode != ColMode.StrucPoint) {
                this.RecordUndoEvent("Point Driven Structural Column Mode");
                this.colMode = ColMode.StrucPoint;
                this.ExpireSolution(true);
            }
        }

        private void Menu_StrucAngleClicked(object sender, EventArgs e) {
            if (this.colMode != ColMode.StrucAngle) {
                this.RecordUndoEvent("Angle Driven Structural Column Mode");
                this.colMode = ColMode.StrucAngle;
                this.ExpireSolution(true);
            }
        }

        public override bool Read(GH_IReader reader) {
            int num = 2;
            reader.TryGetInt32("ColMode", ref num);
            this.colMode = (ColMode)num;
            return base.Read(reader);
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddBooleanParameter("Write Toggle", "Write", "Set to 'True' to write CSV file", GH_ParamAccess.item);                      // 0
            pManager.AddTextParameter("FolderPath", "Path", "Path to folder", GH_ParamAccess.item);                                             // 1
            pManager.AddTextParameter("CsvFileName", "File", "Name of CSV File to be created", GH_ParamAccess.item);                            // 2
            pManager[2].Optional = true;
            pManager.AddTextParameter("RevitFamilyName", "Family", "Revit Column Family Name", GH_ParamAccess.item);                            // 3
            pManager[3].Optional = true;
            pManager.AddTextParameter("RevitTypeName", "Type", "Revit Column Type Name", GH_ParamAccess.item);                                  // 4
            pManager[4].Optional = true;
            pManager.AddPointParameter("Points", "Points", "A Tree of Placement Points for Columns", GH_ParamAccess.tree);                      // 5
            pManager[5].Optional = true;
            pManager.AddNumberParameter("Height", "Height", "Specify height for columns", GH_ParamAccess.list);                                 // 6
            pManager[6].Optional = true;
            pManager.AddNumberParameter("Rotation", "Rotation", "Specify rotation for columns", GH_ParamAccess.list);                           // 7
            pManager[7].Optional = true;
            pManager.AddTextParameter("ParameterNames", "Params", "A List of Parameter Names to Set for Each Column", GH_ParamAccess.list);     // 8
            pManager[8].Optional = true;
            pManager.AddTextParameter("ParameterValues", "Values", "A Tree of Parameter Values for Column Instances", GH_ParamAccess.tree);     // 9
            pManager[9].Optional = true;
     
        }
        
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "out", "Output Messages", GH_ParamAccess.item);
            pManager.AddTextParameter("Data", "data", "Instruction Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            // Set up Utility object and start process
            Utility utility = new Utility(DA);
            utility.Print("Starting Columns.");
            List<string> instructionData = new List<string>();     

            bool write = false;

            if (DA.Iteration == 0) {
                switch (this.colMode) {
                    case ColMode.None:
                        this.Message = "Select Mode";
                        break;
                    case ColMode.Arch:
                        this.Message = "Architectural";
                        break;
                    case ColMode.StrucVert:
                        this.Message = "Structural Vertical";
                        break;
                    case ColMode.StrucPoint:
                        this.Message = "Structural Point";
                        break;
                    case ColMode.StrucAngle:
                        this.Message = "Structural Angle";
                        break;
                }
            }

            // Get Inputs
            string folderPath = null, fileName = null;
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

            string familyName = null, typeName = null;         // Family and type are optional but both must be provided if used.
            utility.GetInput(3, ref familyName);
            utility.GetInput(4, ref typeName);

            GH_Structure<GH_Point> points = null;               // Points in Data Tree required
            if (!utility.GetInput(5, out points)) return;

            List<double> heights = null;                        // Heights list optional
            utility.GetInput(6, ref heights);
            int iMaxCountHeight = 0;
            if (heights != null) iMaxCountHeight = heights.Count;

            List<double> rotations = null;                      // Rotations list optional
            utility.GetInput(7, ref rotations);
            int iMaxCountRotate = 0;
            if (rotations != null) iMaxCountRotate = rotations.Count;
            
            List<string> parameterNames = null;                 // Parameter names list and values tree are optional but both must be provided if used.
            GH_Structure<GH_String> parameterValues = null;
            string[,] parameterArray = null;
            utility.GetParameterValueArray(8, 9, ref parameterNames, out parameterValues, out parameterArray);
            int iMaxCountParam = 0, jMaxCountParam = 0;
            if (parameterArray != null)
            {
                iMaxCountParam = parameterArray.GetLength(0);
                jMaxCountParam = parameterArray.GetLength(1);
                if (parameterNames.Count < jMaxCountParam) jMaxCountParam = parameterNames.Count;
            }

            if (write) {
                // Create RevitModelBuilderUtility object and link to CSV file            
                CsvWriter csvWriter = new CsvWriter();
                utility.Print("CsvWriter Version: " + csvWriter.Version);
                if (!utility.EstablishCsvLink(csvWriter, folderPath, fileName)) {
                    utility.Print("EstablishCsvLink() failed");
                    utility.WriteOut();
                    return;
                }

                try {
                    // Switch on Column Mode
                    if (familyName != null && typeName != null) {
                        switch (this.colMode) {
                            case ColMode.Arch: 
                                csvWriter.SetColumnMode("Architectural", familyName, typeName);
                                break;                               
                            case ColMode.StrucPoint:
                                csvWriter.SetColumnMode("StructuralPointDriven", familyName, typeName);
                                break;                                
                            case ColMode.StrucVert: 
                                csvWriter.SetColumnMode("StructuralVertical", familyName, typeName);
                                break;                                
                            case ColMode.StrucAngle: 
                                csvWriter.SetColumnMode("StructuralAngleDriven", familyName, typeName);
                                break;                                
                            default: 
                                break;                                
                        }
                    }
                }
                catch (Exception exception) {
                    utility.Print(exception.Message);
                    utility.WriteOut();
                    return;
                }

                // Loop for each column, get points, and place component; then set parameter values
                try {
                    double lastRotationValue = 0;
                    double lastHeightValue = 0;

                    for (int i = 0; i < points.Branches.Count; i++) {

                        // Set rotation if necessary
                        if (rotations != null) {
                            if (i < iMaxCountRotate) {
                                if (Math.Abs(lastRotationValue - rotations[i]) > 0.00000001) {
                                    csvWriter.SetColumnRotation(rotations[i]);
                                    lastRotationValue = rotations[i];
                                }
                            }
                        }

                        // Set Height if not in points placement mode.
                        if (colMode != ColMode.StrucPoint) {
                            if (i < iMaxCountHeight) {
                                if (Math.Abs(lastHeightValue - heights[i]) > 0.00000001) {
                                    csvWriter.SetColumnHeight(heights[i]);
                                    lastHeightValue = heights[i];
                                }
                            }
                        }

                        // Add the Column
                        List<List<HbCurve>> curvesListListRevit = new List<List<HbCurve>>();
                        List<Grasshopper.Kernel.Types.GH_Point> branch = points.Branches[i];
                        HbXYZ hbXYZ1 = new HbXYZ(branch[0].Value.X, branch[0].Value.Y, branch[0].Value.Z);

                        if (colMode == ColMode.StrucPoint) {
                            HbXYZ hbXYZ2 = new HbXYZ(branch[1].Value.X, branch[1].Value.Y, branch[1].Value.Z);
                            csvWriter.AddColumn(hbXYZ1, hbXYZ2);
                            instructionData.Add("Add Column:");
                        }
                        else {
                            csvWriter.AddColumn(hbXYZ1);
                            instructionData.Add("Add Column:");
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
                    utility.Print("Columns completed successfully.");
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
        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("ColMode", (int)this.colMode);
            return base.Write(writer);
        }

        public override GH_Exposure Exposure {
            get { return GH_Exposure.secondary; }
        }

        public override Guid ComponentGuid {
            get { return new Guid("F9F019D3-27AC-4837-9099-B54E8C22205D"); }
        }

        protected override Bitmap Icon {
            get { return new Bitmap(Hummingbird.Properties.Resources.Column); }
        }

    }
}
