using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace HummingbirdUtility {
    public class HbCurveArray : IEnumerable {

        public List<HbCurve> HbCurves = new List<HbCurve>();

        public HbCurveArray() {

        }

        public HbCurveArray(List<HbCurve> hbCurves) {
            this.HbCurves = hbCurves;
        }

        public void Add(HbCurve hbItem) {
            HbCurves.Add(hbItem);
        }

        public IEnumerator GetEnumerator() {
            foreach (HbCurve hbItem in HbCurves) {
                yield return hbItem;
            }
        }

        public int Count() {
            return HbCurves.Count;
        }

    }
}
