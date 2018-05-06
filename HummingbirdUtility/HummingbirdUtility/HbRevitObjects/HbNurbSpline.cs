using System.Collections.Generic;

namespace HummingbirdUtility {

    public class HbNurbSpline : HbCurve {

        public List<HbXYZ> Points { set; get; } 

        public HbNurbSpline() {
            Points = new List<HbXYZ>();
        }

        public HbNurbSpline(List<HbXYZ> points) {
            Points = points;
        }

        // For use with DesignScript
        public static HbNurbSpline New(List<HbXYZ> points) {
            return new HbNurbSpline(points);
        }

    }
}
