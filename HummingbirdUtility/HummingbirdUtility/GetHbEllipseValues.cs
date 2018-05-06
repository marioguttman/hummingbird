using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

namespace HummingbirdUtility {
    class GetHbEllipseValues {

        public HbXYZ PointFirst { set; get; }  // Center in Full case; First end in Half case
        public HbXYZ PointSecond { set; get; } // Second End
        public double RadiusY { set; get; }    // Radius in the perpendicular direction since primary radius was set by the points
        public string Mode { set; get; }       // "Half" or "Full" for whether to draw a full or half ellipse

        // ****************************************************** Constructor ***********************************************************
        public GetHbEllipseValues(DataRow dataRow) {
            this.PointFirst = new GetHbValue(dataRow[4].ToString()).AsHbXYZ();   // these values may silently return null if blank entry or other data type
            this.PointSecond = new GetHbValue(dataRow[5].ToString()).AsHbXYZ();   
            this.RadiusY = new GetHbValue(dataRow[6].ToString()).AsDouble().Value;   
            this.Mode = dataRow[7].ToString();
        }
    }
}

