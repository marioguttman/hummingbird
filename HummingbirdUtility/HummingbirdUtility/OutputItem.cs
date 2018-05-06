//using System;
//using System.Collections.Generic;
//using System.Text;
using System.Data;

namespace HummingbirdUtility {
    class OutputItem {
        public string RowId { set; get; }
        //public DataRow DataRow { set; get; }
        public OutputValue ElementId { set; get; }
        public string Action { set; get; }
        public string Object { set; get; }
        public OutputValue Value01 { set; get; }
        public OutputValue Value02 { set; get; }
        public OutputValue Value03 { set; get; }
        public OutputValue Value04 { set; get; }

        private DataTable dataTable;

        public OutputItem(DataTable dataTable, string rowId, string actionToSet, string objectToSet) {
            this.dataTable = dataTable;
            this.RowId = rowId;
            this.Action = actionToSet;
            this.Object = objectToSet;
            this.Value01 = new OutputValue("");
            this.Value02 = new OutputValue("");
            this.Value03 = new OutputValue("");
            this.Value04 = new OutputValue("");
        }

        public bool WriteDataRow() {
            DataRow dataRow = dataTable.NewRow();
            dataRow[0] = RowId;
            dataRow[1] = ElementId;
            dataRow[2] = Action;
            dataRow[3] = Object;
            dataRow[4] = Value01.Text;
            dataRow[5] = Value02.Text;
            dataRow[6] = Value03.Text;
            dataRow[7] = Value04.Text;
            this.dataTable.Rows.Add(dataRow);
            return true;
        }
    }
}
