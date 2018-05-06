using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace HummingbirdUtility {
    public class HbCurveArrArray : IEnumerable {

        public List<HbCurveArray> HbCurveArrays = new List<HbCurveArray>();

        public HbCurveArrArray() {
        }

        public HbCurveArrArray(List<HbCurveArray> hbCurveArrays) {
            this.HbCurveArrays = hbCurveArrays;
        }

        public void Add(HbCurveArray hbCurveArray) {
            HbCurveArrays.Add(hbCurveArray);
        }

        public IEnumerator GetEnumerator() {
            foreach (HbCurveArray hbCurveArray in HbCurveArrays) {
                yield return hbCurveArray;
            }
        }

        public int Count() {
            return HbCurveArrays.Count;
        }
    }
}
