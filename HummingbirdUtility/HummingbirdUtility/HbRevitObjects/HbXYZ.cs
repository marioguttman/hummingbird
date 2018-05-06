namespace HummingbirdUtility {

    public class HbXYZ {

        public double X { set; get; }
        public double Y { set; get; }
        public double Z { set; get; }

        public HbXYZ() { }

        public HbXYZ(double x, double y, double z) {
            X = x;
            Y = y;
            Z = z;
        }

        // For use with DesignScript
        public static HbXYZ New(double x, double y, double z) {
            return new HbXYZ(x, y, z);
        }
    }
}
