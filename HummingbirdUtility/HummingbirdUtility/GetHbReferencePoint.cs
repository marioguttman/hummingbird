using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

namespace HummingbirdUtility {
    public class GetHbReferencePoint {

        public HbReferencePoint HbReferencePoint {set; get;}

        // ****************************************************** Constructor ***********************************************************

        public GetHbReferencePoint(DataRow dataRow) {
            this.HbReferencePoint = new HbReferencePoint(new GetHbValue(dataRow[4].ToString()).AsHbXYZ());   
        }

        // ***************************************************** Public Functions *******************************************************


    }
}
