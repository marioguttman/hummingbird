namespace HummingbirdUtility {

    public class HbArc : HbCurve {

        public HbXYZ PointStart { set; get; }
        public HbXYZ PointEnd { set; get; }
        public HbXYZ PointMid { set; get; }

        public HbArc() { }

        public HbArc(HbXYZ pointStart, HbXYZ pointEnd, HbXYZ pointMid) {
            PointStart = pointStart;
            PointEnd = pointEnd;
            PointMid = pointMid;
        }

        // For use with DesignScript
        public static HbArc New(HbXYZ pointStart, HbXYZ pointEnd, HbXYZ pointMid) {
            return new HbArc(pointStart, pointEnd, pointMid);
        }
        //public static HbArc StaticHbArc(HbXYZ pointStart, HbXYZ pointEnd, HbXYZ pointMid) {
        //    return new HbArc(pointStart, pointEnd, pointMid);
        //}
 }
}
