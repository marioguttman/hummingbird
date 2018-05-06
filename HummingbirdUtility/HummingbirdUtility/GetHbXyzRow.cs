using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

namespace HummingbirdUtility {
    public class GetHbXyzRow {

            //        DataTable.Columns.Add("RowId", typeof(string));
            //DataTable.Columns.Add("ElementId", typeof(string));
            //DataTable.Columns.Add("Action", typeof(string));
            //DataTable.Columns.Add("Object", typeof(string));
            //DataTable.Columns.Add("Value01", typeof(string));
            //DataTable.Columns.Add("Value02", typeof(string));
            //DataTable.Columns.Add("Value03", typeof(string));
            //DataTable.Columns.Add("Value04", typeof(string));

        public List<HbXYZ> HbPoints = new List<HbXYZ>();

        // ****************************************************** Constructor ***********************************************************

        public GetHbXyzRow(DataRow dataRow) {
            GetOneRow(dataRow);
        }

        // ***************************************************** Public Functions *******************************************************

        public void NextDataRow(DataRow dataRow) {
            GetOneRow(dataRow);
        }

        private void GetOneRow(DataRow dataRow) {
            HbXYZ point;
            point =  new GetHbValue(dataRow[4].ToString()).AsHbXYZ();   // these values may silently return null if blank entry or other data type
            if (point != null) HbPoints.Add(point);
            point = new GetHbValue(dataRow[5].ToString()).AsHbXYZ();
            if (point != null) HbPoints.Add(point);
            point = new GetHbValue(dataRow[6].ToString()).AsHbXYZ();
            if (point != null) HbPoints.Add(point);
            point = new GetHbValue(dataRow[7].ToString()).AsHbXYZ();
            if (point != null) HbPoints.Add(point);
        }
    }
}
