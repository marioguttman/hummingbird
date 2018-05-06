using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HummingbirdUtility {
    public class HbElementId : HbCurve {

        public int ElementIdValue { set; get; }
        public double Y { set; get; }
        public double Z { set; get; }

        public HbElementId() { }

        public HbElementId(int elementIdValue) {
            ElementIdValue = elementIdValue;
        }

        // For use with DesignScript
        public static HbElementId New(int elementIdValue) {
            return new HbElementId(elementIdValue);
        }
    }
}
