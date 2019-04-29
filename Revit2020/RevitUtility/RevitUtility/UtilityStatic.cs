using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevitUtility {
    public class UtilityStatic {

        // **************************************************** Public ****************************************************

        public static System.Windows.Forms.Control FindControlOnForm(System.Windows.Forms.Form parentForm, string controlName) {
            System.Windows.Forms.Control controlToSet = null;
            System.Windows.Forms.Control.ControlCollection formControls = parentForm.Controls;
            foreach (System.Windows.Forms.Control formControlTest in formControls) {
                controlToSet = FindControlRecursive(formControlTest, controlName);
                if (!(controlToSet == null)) break;
            }
            return controlToSet;
        }

        // **************************************************** Private ****************************************************

        private static System.Windows.Forms.Control FindControlRecursive(System.Windows.Forms.Control container, string name) {
            if (container.Name == name)
                return container;
            foreach (System.Windows.Forms.Control controlTest in container.Controls) {
                System.Windows.Forms.Control controlFound = FindControlRecursive(controlTest, name);
                if (controlFound != null)
                    return controlFound;
            }
            return null;
        }
    }
}
