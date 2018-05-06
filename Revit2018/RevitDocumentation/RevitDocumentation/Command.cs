using System;

using System.Reflection;             //To get current folder location
using System.IO;                     //for File, Directory, StreamWriter

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace RevitDocumentation {
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.ReadOnly)]
    public class Command : IExternalCommand {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet) {
            try {
                string startPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
                string parentPath = Directory.GetParent(startPath).FullName;
                string documentationPath = parentPath + @"\Documentation";
                if (!(Directory.Exists(documentationPath))) {
                    System.Windows.Forms.MessageBox.Show("Documentation folder not found at: " + documentationPath + ".");
                    return Result.Failed;
                }

                Documents dialog = new Documents(documentationPath);
                dialog.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception exception) {
                message = exception.Message;
                return Result.Failed;
            }
        }
    }
}
