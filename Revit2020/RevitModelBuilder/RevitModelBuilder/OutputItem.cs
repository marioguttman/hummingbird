using System.Data;

using Autodesk.Revit.DB;

namespace RevitModelBuilder {
    class OutputItem {

        #region Module Variables                        // ****************************** Module Variables ******************************************

        public string RowId { set; get; }
        public DataRow DataRow { set; get; }
        public OutputValue ElementId { set; get; }
        public string Action { set; get; }
        public string Object { set; get; }
        public OutputValue Value01 { set; get; }
        public OutputValue Value02 { set; get; }
        public OutputValue Value03 { set; get; }
        public OutputValue Value04 { set; get; }

        #endregion

        #region Constructors                            // ****************************** Constructors **********************************************

        public OutputItem(string rowId, string actionToSet, string objectToSet) {
            RowId = rowId;
            ElementId = new OutputValue("");
            Action = actionToSet;
            Object = objectToSet;
            Value01 = new OutputValue("");
            Value02 = new OutputValue("");
            Value03 = new OutputValue("");
            Value04 = new OutputValue("");
        }

        public OutputItem(string rowId, ElementId elementId, string actionToSet, string objectToSet) {
            RowId = rowId;
            ElementId = new OutputValue(elementId); 
            Action = actionToSet;
            Object = objectToSet;
            Value01 = new OutputValue("");
            Value02 = new OutputValue("");
            Value03 = new OutputValue("");
            Value04 = new OutputValue("");
        }

        #endregion

        #region Public Functions                        // ****************************** Public Functions ******************************************

        public bool AddDataRow(DataTable dataTable) {
            DataRow dataRow = dataTable.NewRow();
            dataRow[0] = RowId;
            dataRow[1] = ElementId.Text;
            dataRow[2] = Action;
            dataRow[3] = Object;
            dataRow[4] = Value01.Text;
            dataRow[5] = Value02.Text;
            dataRow[6] = Value03.Text;
            dataRow[7] = Value04.Text;
            dataTable.Rows.Add(dataRow);
            return true;
        }

        #endregion

    }
}
