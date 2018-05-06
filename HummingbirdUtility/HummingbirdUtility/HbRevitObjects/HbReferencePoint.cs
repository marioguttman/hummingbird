namespace HummingbirdUtility {

    public class HbReferencePoint {

        public double X { set; get; }
        public double Y { set; get; }
        public double Z { set; get; }

        public HbReferencePoint() { }

        public HbReferencePoint(double x, double y, double z) {
            X = x;
            Y = y;
            Z = z;
        }
        public HbReferencePoint(HbXYZ hbXyz) {
            X = hbXyz.X;
            Y = hbXyz.Y;
            Z = hbXyz.Z;
        }

        // For use with DesignScript
        public static HbReferencePoint New(double x, double y, double z) {
            return new HbReferencePoint(x, y, z);
        }
    }
}
