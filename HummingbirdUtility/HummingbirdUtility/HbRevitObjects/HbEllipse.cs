namespace HummingbirdUtility {

    public class HbEllipse : HbCurve {

        public HbXYZ PointFirst { set; get; }  // Center in Full case; First end in Half case
        public HbXYZ PointSecond { set; get; } // Second End
        public double RadiusY { set; get; }       // Radius in the perpendicular direction since primary radius was set by the points
        public string Mode { set; get; }          // "Half" or "Full" for whether to draw a full or half ellipse

        public HbEllipse() { }

        public HbEllipse(HbXYZ pointFirst, HbXYZ pointSecond, double radiusY, string mode) {
            PointFirst = pointFirst;
            PointSecond = pointSecond;
            RadiusY = radiusY;
            Mode = mode;
        }

        // For use with DesignScript
        public static HbEllipse New(HbXYZ pointFirst, HbXYZ pointSecond, double radiusY, string mode) {
            return new HbEllipse(pointFirst, pointSecond, radiusY, mode);
        }
    }
}
