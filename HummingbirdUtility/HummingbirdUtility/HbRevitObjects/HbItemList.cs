using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace HummingbirdUtility.HbRevitObjects {
    public class HbItemList : IEnumerable {

        public List<HbCurve> HbList { set; get; }

        public HbItemList() {
            HbList = new List<HbCurve>();
        }

        public static void Add(HbItemList list, HbCurve hbItem) {
            list.HbList.Add(hbItem);
        }

        public void Add(HbCurve hbItem) {
            HbList.Add(hbItem);
        }

        public IEnumerator GetEnumerator() {
            foreach (HbCurve hbItem in HbList) {
                yield return hbItem;
            }
        }

        public int Count() {
            return HbList.Count;
        }

    }
}
