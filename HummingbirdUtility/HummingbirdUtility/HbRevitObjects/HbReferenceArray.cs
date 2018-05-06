using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace HummingbirdUtility {
    public class HbReferenceArray : IEnumerable {

        public List<HbReferencePoint> HbReferencePoints = new List<HbReferencePoint>();

        public HbReferenceArray() {
        }

        public HbReferenceArray(List<HbReferencePoint> listHbReferencePoints) {
            this.HbReferencePoints = listHbReferencePoints;
        }
        public HbReferenceArray(List<HbXYZ> listHbXyzPoints) {
            foreach (HbXYZ hbXyz in listHbXyzPoints) {
                this.HbReferencePoints.Add(new HbReferencePoint(hbXyz));
            }
        }

        public void Add(HbReferencePoint hbReferencePoint) {
            HbReferencePoints.Add(hbReferencePoint);
        }

        public IEnumerator GetEnumerator() {
            foreach (HbReferencePoint hbReferencePoint in HbReferencePoints) {
                yield return hbReferencePoint;
            }
        }

        public int Count() {
            return HbReferencePoints.Count;
        }
    }
}
