using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace HummingbirdUtility {

    public class HbReferenceArrayArray : IEnumerable {

        public List<HbReferenceArray> HbReferenceArrays = new List<HbReferenceArray>();

        public HbReferenceArrayArray() {
        }

        public HbReferenceArrayArray(List<HbReferenceArray> hbReferenceArrays) {
            this.HbReferenceArrays = hbReferenceArrays;
        }

        public void Add(HbReferenceArray hbReferenceArray) {
            HbReferenceArrays.Add(hbReferenceArray);
        }

        public IEnumerator GetEnumerator() {
            foreach (HbReferenceArray hbReferenceArray in HbReferenceArrays) {
                yield return hbReferenceArray;
            }
        }

        public int Count() {
            return HbReferenceArrays.Count;
        }
    }
}
