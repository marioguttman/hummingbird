using System;

namespace HummingbirdCSExample {
    class Spiral {

        public XY CenterPoint { set; get; }
        public double FactorA { set; get; }
        public double FactorB { set; get; }

        public Spiral(XY centerPoint) {
            CenterPoint = centerPoint;
            FactorA = 1;
            FactorB = 1;
        }

        public XY CalculatePoint(double t) {
            double exponent = FactorB * t;
            double r = FactorA * Math.Pow(Math.E, exponent);
            double x = r * Math.Cos(t);
            double y = r * Math.Sin(t);
            return new XY(x, y);
        }
    }
}
