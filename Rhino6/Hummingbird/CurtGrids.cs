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
    public class CurtGrids : GH_Component
    {
        private string DEFAULT_FILE_NAME = "Lines.csv";  // "const" not allowed with GH

        private enum GridMode {
            UGridFull,
            UGridPart,
            UMullionFull,
            UMullionPart,
            VGridFull,
            VGridPart,
            VMullionFull,
            VMullionPart
        }
        private GridMode gridMode;

        public CurtGrids(): base("CurtGrids", "CurtGrids", "Create Grids/Mullions for input to CurtWall component", "Extra", "Hummingbird") {
            this.gridMode = GridMode.UGridFull;
            this.Menu_UFullGridClicked(null, null);
            this.Message = "U Full Grid Line";
            this.Hidden = false;
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
            ToolStripMenuItem item0 = GH_DocumentObject.Menu_AppendItem(menu, "U Full Grid Line", new EventHandler(this.Menu_UFullGridClicked), true, this.gridMode == GridMode.UGridFull);
            ToolStripMenuItem item1 = GH_DocumentObject.Menu_AppendItem(menu, "U Partial Grid Segment", new EventHandler(this.Menu_UPartGridClicked), true, this.gridMode == GridMode.UGridPart);
            ToolStripMenuItem item2 = GH_DocumentObject.Menu_AppendItem(menu, "U Full Grid with Mullion", new EventHandler(this.Menu_UFullMullClicked), true, this.gridMode == GridMode.UMullionFull);
            ToolStripMenuItem item3 = GH_DocumentObject.Menu_AppendItem(menu, "U Partial Grid with Mullion", new EventHandler(this.Menu_UPartMullClicked), true, this.gridMode == GridMode.UMullionPart);
            ToolStripMenuItem item4 = GH_DocumentObject.Menu_AppendItem(menu, "V Full Grid Line", new EventHandler(this.Menu_VFullGridClicked), true, this.gridMode == GridMode.VGridFull);
            ToolStripMenuItem item5 = GH_DocumentObject.Menu_AppendItem(menu, "V Partial Grid Segment", new EventHandler(this.Menu_VPartGridClicked), true, this.gridMode == GridMode.VGridPart);
            ToolStripMenuItem item6 = GH_DocumentObject.Menu_AppendItem(menu, "V Full Grid with Mullion", new EventHandler(this.Menu_VFullMullClicked), true, this.gridMode == GridMode.VMullionFull);
            ToolStripMenuItem item7 = GH_DocumentObject.Menu_AppendItem(menu, "V Partial Grid with Mullion", new EventHandler(this.Menu_VPartMullClicked), true, this.gridMode == GridMode.VMullionPart);

            item0.ToolTipText = "Add a horizontal grid line the full width of the curtain wall.";
            item1.ToolTipText = "Add a horizontal grid line segment between vertical grids/edges.";
            item2.ToolTipText = "Add a horizontal grid line with a mullion the full width of the curtain wall.";
            item3.ToolTipText = "Add a horizontal grid line segment with a mullion between vertical grids/edges.";
            item4.ToolTipText = "Add a vertical grid line the full height of the curtain wall.";
            item5.ToolTipText = "Add a vertical grid line segment between horizontal grids/edges.";
            item6.ToolTipText = "Add a vertical grid line with a mullion the full height of the curtain wall.";
            item7.ToolTipText = "Add a vertical grid line segment with a mullion between horizontal grids/edges.";
        }

        private void Menu_UFullGridClicked(object sender, EventArgs e) {
            if (this.gridMode != GridMode.UGridFull) {
                this.RecordUndoEvent("U Full Grid");
                this.gridMode = GridMode.UGridFull;
                this.ExpireSolution(true);
            }
        }
        private void Menu_UPartGridClicked(object sender, EventArgs e) {
            if (this.gridMode != GridMode.UGridPart) {
                this.RecordUndoEvent("U Part Grid");
                this.gridMode = GridMode.UGridPart;
                this.ExpireSolution(true);
            }
        }
        private void Menu_UFullMullClicked(object sender, EventArgs e) {
            if (this.gridMode != GridMode.UMullionFull) {
                this.RecordUndoEvent("U Full Mull");
                this.gridMode = GridMode.UMullionFull;
                this.ExpireSolution(true);
            }
        }
        private void Menu_UPartMullClicked(object sender, EventArgs e) {
            if (this.gridMode != GridMode.UMullionPart) {
                this.RecordUndoEvent("U Part Mull");
                this.gridMode = GridMode.UMullionPart;
                this.ExpireSolution(true);
            }
        }
        private void Menu_VFullGridClicked(object sender, EventArgs e) {
            if (this.gridMode != GridMode.VGridFull) {
                this.RecordUndoEvent("V Full Grid");
                this.gridMode = GridMode.VGridFull;
                this.ExpireSolution(true);
            }
        }
        private void Menu_VPartGridClicked(object sender, EventArgs e) {
            if (this.gridMode != GridMode.VGridPart) {
                this.RecordUndoEvent("V Part Grid");
                this.gridMode = GridMode.VGridPart;
                this.ExpireSolution(true);
            }
        }
        private void Menu_VFullMullClicked(object sender, EventArgs e) {
            if (this.gridMode != GridMode.VMullionFull) {
                this.RecordUndoEvent("V Full Mull");
                this.gridMode = GridMode.VMullionFull;
                this.ExpireSolution(true);
            }
        }
        private void Menu_VPartMullClicked(object sender, EventArgs e) {
            if (this.gridMode != GridMode.VMullionPart) {
                this.RecordUndoEvent("V Part Mull");
                this.gridMode = GridMode.VMullionPart;
                this.ExpireSolution(true);
            }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)  {
            pManager.AddNumberParameter("Primary Offset", "Primary", "Primary Offset proportion of side (0 < x < 1).", GH_ParamAccess.list);         // 0
            pManager.AddNumberParameter("Secondary Offset", "Secondary", "Secondary Offset proportion of grid (0 < x < 1).", GH_ParamAccess.list);   // 1
            pManager[1].Optional = true;
            pManager.AddTextParameter("MullionFamilyName", "Family", "Revit Mullion Family Name", GH_ParamAccess.item);                              // 2
            pManager[2].Optional = true;
            pManager.AddTextParameter("MullionTypeName", "Type", "Revit Mullion Family Type Name", GH_ParamAccess.item);                             // 3
            pManager[3].Optional = true;
           
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Mullions", "M", "Mullion definition strings.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {            
            if (DA.Iteration == 0)  {
                switch (this.gridMode)  {
                    case GridMode.UGridFull:
                        this.Message = "U Full Grid";
                        break;
                    case GridMode.UGridPart:
                        this.Message = "U Part Grid";
                        break;
                    case GridMode.UMullionFull:
                        this.Message = "U Full Mull";
                        break;
                    case GridMode.UMullionPart:
                        this.Message = "U Part Mull";
                        break;
                    case GridMode.VGridFull:
                        this.Message = "V Full Grid";
                        break;
                    case GridMode.VGridPart:
                        this.Message = "V Part Grid";
                        break;
                    case GridMode.VMullionFull:
                        this.Message = "V Full Mull";
                        break;
                    case GridMode.VMullionPart:
                        this.Message = "V Part Mull";
                        break;
                }
            }



            // Set up Utility object and start process
            Utility utility = new Utility(DA);

            // Start input

            //utility.Print("Initializing.");
            List<double> primaryOffsets = new List<double>();     // Primary offsets required
            if (!utility.GetInput(0, ref primaryOffsets)) {
                utility.WriteOut();
                return;
            }

            List<double> secondaryOffsets = new List<double>();  // Secondary offsets optional
            utility.GetInput(1, ref secondaryOffsets);              

            string familyName = "$none$";             // Family and type are optional but both required if used
            string typeName = "$none$";
            utility.GetInput(2, ref familyName);      // If no input leaves default "<none>" value
            utility.GetInput(3, ref typeName);        // Allowing one set in case it makes sense with value set in dialog box

            string direction = "";
            bool oneSegmentOnly;
            switch (this.gridMode) {
                case GridMode.UGridFull:
                    direction = "U";
                    oneSegmentOnly = false;
                    break;
                case GridMode.UGridPart:
                    direction = "U";
                    oneSegmentOnly = true;
                    break;
                case GridMode.UMullionFull:
                    direction = "U";
                    oneSegmentOnly = false;
                    break;
                case GridMode.UMullionPart:
                    direction = "U";
                    oneSegmentOnly = true;
                    break;
                case GridMode.VGridFull:
                    direction = "V";
                    oneSegmentOnly = false;
                    break;
                case GridMode.VGridPart:
                    direction = "V";
                    oneSegmentOnly = true;
                    break;
                case GridMode.VMullionFull:
                    direction = "V";
                    oneSegmentOnly = false;
                    break;
                case GridMode.VMullionPart:
                    direction = "V";
                    oneSegmentOnly = true;
                    break;
                default:
                    oneSegmentOnly = false;  // Just for compiler
                    // Should not be here
                    break;
            }

            List<string> outputStrings = new List<string>();
            for (int i = 0; i < primaryOffsets.Count; i++) {
                double primaryOffset = primaryOffsets[i];
                double secondaryOffset = 0.0;
                if (oneSegmentOnly) {
                    if (secondaryOffsets.Count > i) {
                        if (!(secondaryOffsets[i] <= 0.0 || secondaryOffsets[i] >= 1.0)) secondaryOffset = secondaryOffsets[i];  // Silently ignore invalid values                    
                    }
                }
                outputStrings.Add("<" + direction + "><" + primaryOffset.ToString() + "><" + secondaryOffset.ToString() + "><" + familyName + "><" + typeName + ">");

            }

            DA.SetDataList(0, outputStrings);
        }

        // ------------------------------------------------------- General housekeeping for all components ----------------------------------------------------------

        // This is an example of making the settings "stickey" using the features of GH_IO.Serialization (see "using" statement above).  The writer saves the value and the reader restores it.
        // Note that we have overwritten the base.Write and base.Read functions so we call them after restoring our custom values.
        public override bool Write(GH_IWriter writer) {
            writer.SetInt32("GridMode", (int)this.gridMode);
            return base.Write(writer);
        }
        public override bool Read(GH_IReader reader) {
            int num = 0;    // Was 2 with lines?  Maybe 0?
            reader.TryGetInt32("GridMode", ref num);
            this.gridMode = (GridMode)num;
            return base.Read(reader);
        }

        public override Guid ComponentGuid {
            get { return new Guid("5E16E1B9-19BE-45FD-B9CC-2C2F2732B5F1"); }
        }

        public override GH_Exposure Exposure {
            get { return GH_Exposure.secondary; }
        }

        protected override Bitmap Icon {
            get { return new Bitmap(Hummingbird.Properties.Resources.CurtGrid); }
        }

    }
}
