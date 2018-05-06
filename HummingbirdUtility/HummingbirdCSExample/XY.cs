using System;

namespace HummingbirdCSExample {
    class XY {
        public double X { set; get; }
        public double Y { set; get; }
        public XY(double x, double y) {
            X = x;
            Y = y;
        }
        public XY Rotate(XY center, double angle) {
            //If you rotate point (px, py) around point (ox, oy) by angle theta you'll get:
            //p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
            //p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy
            double deltaX = X - center.X;
            double deltaY = Y - center.Y;
            double x = Math.Cos(angle) * (deltaX) - Math.Sin(angle) * (deltaY) + center.X;
            double y = Math.Sin(angle) * (deltaX) + Math.Cos(angle) * (deltaY) + center.Y;
            return new XY(x, y); ;
        }
    }

}
