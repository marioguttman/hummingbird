
using System;
using System.Reflection;             //To get current folder location
using System.Windows.Media.Imaging;  //For bitmap image  Reference is to "PresentationCore"

using Autodesk.Revit.UI;

namespace RevitHummingbird {

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Application : Autodesk.Revit.UI.IExternalApplication {

        public Result OnStartup(UIControlledApplication application) {

            string startPath = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf("\\") + 1);

            RibbonPanel ribbonPanel = application.CreateRibbonPanel("HBird");

            PushButtonData pushButtonDataModelBuilder = new PushButtonData("pushButtonModelBuilder", "Model\nBuilder", startPath + "RevitModelBuilder.dll", "RevitModelBuilder.Command");
            pushButtonDataModelBuilder.ToolTip = "Revit ModelBuilder tool for use with Hummingbird.";
            pushButtonDataModelBuilder.Image = new BitmapImage(new Uri(startPath + "Hummingbird16.png", UriKind.Absolute));
            pushButtonDataModelBuilder.LargeImage = new BitmapImage(new Uri(startPath + "Hummingbird32.png", UriKind.Absolute));
            PushButtonData pushButtonDataDocumentation = new PushButtonData("pushButtonDocumentation", "Documentation", startPath + "RevitDocumentation.dll", "RevitDocumentation.Command");
            pushButtonDataDocumentation.ToolTip = "Documentation for Hummingbird.";
            SplitButtonData splitButtonData = new SplitButtonData("splitButton", "Split");   // Neither value is visible
            SplitButton splitButton = ribbonPanel.AddItem(splitButtonData) as SplitButton;
            splitButton.AddPushButton(pushButtonDataModelBuilder);
            splitButton.AddPushButton(pushButtonDataDocumentation);
            splitButton.IsSynchronizedWithCurrentItem = false;
     
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application) {
            return Result.Succeeded;
        }

    }
}
