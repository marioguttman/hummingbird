namespace HummingbirdUtility {
    public class HbLine : HbCurve {

        public HbXYZ PointStart { set; get; }
        public HbXYZ PointEnd { set; get; }

        public HbLine() { }

        public HbLine(HbXYZ pointStart, HbXYZ pointEnd) {
            PointStart = pointStart;
            PointEnd = pointEnd;
        }

        // For use with DesignScript
        public static HbLine New(HbXYZ pointStart, HbXYZ pointEnd) {
            return new HbLine(pointStart, pointEnd);
        }

    }
}
