using System.Collections.Generic;

namespace HummingbirdUtility {

    public class HbHermiteSpline : HbCurve {

        public List<HbXYZ> Points { set; get; } 

        public HbHermiteSpline() {
            Points = new List<HbXYZ>();
        }

        public HbHermiteSpline(List<HbXYZ> points) {
            Points = points;
        }

        // For use with DesignScript
        public static HbHermiteSpline New(List<HbXYZ> points) {
            return new HbHermiteSpline(points);
        }
    }
}
