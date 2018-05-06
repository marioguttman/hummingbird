using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HummingbirdUtility {

    // The purpose of this class is to provide a way of instantiating objects with parameters in other applicaions.  Initially this was created
    // due to limitations in Autodesk DesignScript but has been generalized in case there turn out to be other uses.

    public class ObjectBuilder {

        // functions for managing lists of Hb items
        public List<HbCurve> MakeListHbItem() {
            return new List<HbCurve>();
        }
        public List<HbCurve> AddToListHbItem(List<HbCurve> list, HbCurve item) {
            list.Add(item);
            return list;
        }

        //public List<List<HbItem>> MakeListListHbItem() {
        //    return new List<List<HbItem>>();
        //}

    }

    //public class HbItemListWrapper {
    //    public List<HbItem> ListHbItems { set; get; }
    //    //public void Initialize() {
    //    //    ListHbItems = new List<HbItem>();
    //    //}
    //    public void Add(HbItem item) {
    //        if (ListHbItems == null) ListHbItems = new List<HbItem>();
    //        ListHbItems.Add(item);
    //    }
    //}
}
