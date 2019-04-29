

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;  //To get current folder location

using Autodesk.Revit.DB;
using RevitUtility;

namespace RevitModelBuilder {

    public partial class Menu : System.Windows.Forms.Form {

        #region Module Variables                        // ****************************** Module Variables ******************************************

        private const string CSV_VIEWER_EXE = "HummingbirdCsvViewer.exe";

        //public System.Windows.Forms.Form ChildForm { get; set; }
        public string CommandToLaunch { get; set; }
        public bool AutoClose { get; set; }

        private UtilitySettings settings;
        private UtilityElements elements;
        private ICollection<ElementId> currentSelectionSet;

        #endregion

        #region Constructor                             // ****************************** Constructor ***********************************************

        public Menu(UtilitySettings utilitySettings, UtilityElements utilityElements, ICollection<ElementId> currentSelectionSet) {
            InitializeComponent();
            this.settings = utilitySettings;
            this.elements = utilityElements;
            this.currentSelectionSet = currentSelectionSet;
        }

        #endregion

        #region Event Handlers                          // ****************************** Event Handlers ********************************************

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

        //Reload the default values.
        private void buttonReloadSettings_Click(object sender, EventArgs e) {
            this.settings.ReloadDefaultValues();
            this.settings.WriteIniFile();
        }

        private void buttonCreateElements_Click(object sender, EventArgs e) {
            CreateElements dialog = new CreateElements(this.settings, this.elements);
            dialog.ShowDialog();
            Close();
        }

        private void buttonExportElements_Click(object sender, EventArgs e) {
            ExportElements dialog = new ExportElements(this.settings, this.elements, this.currentSelectionSet);
            dialog.ShowDialog();
            Close();
        }

        private void buttonCsvViewer_Click(object sender, EventArgs e) {
            string currentFolder = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf("\\"));
            string path = Path.Combine(currentFolder, CSV_VIEWER_EXE);
            if (File.Exists(path)) {
                try {
                    System.Diagnostics.Process.Start(path);
                }
                catch {}
            }
        }

        #endregion
    }
}
