using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace HummingbirdUtility.HbRevitObjects {
    public class HbItemListList : IEnumerable {

        public List<HbItemList> HbListList { set; get; }

        public HbItemListList() {
            HbListList = new List<HbItemList>();
        }

        public static void Add(HbItemListList listList, HbItemList list) {
            listList.HbListList.Add(list);
        }

        public void Add(HbItemList hbItemList) {
            HbListList.Add(hbItemList);
        }

        public IEnumerator GetEnumerator() {
            foreach (HbItemList hbItemList in HbListList) {
                yield return hbItemList;
            }
        }

        public int Count() {
            return HbListList.Count;
        }
    }
}
