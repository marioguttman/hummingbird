using System.Collections.Generic;

using Autodesk.Revit.DB;

namespace RevitModelBuilder {

    public class FailureHandler : IFailuresPreprocessor {

        public string ErrorMessage { set; get; }
        public string ErrorSeverity { set; get; }

        public FailureHandler() {
            ErrorMessage = "";
            ErrorSeverity = "";
        }

        public FailureProcessingResult PreprocessFailures( FailuresAccessor failuresAccessor) {
            IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
            foreach (FailureMessageAccessor failureMessageAccessor in failureMessages) {                
                FailureDefinitionId id = failureMessageAccessor.GetFailureDefinitionId();
                try {
                    ErrorMessage = failureMessageAccessor.GetDescriptionText();
                    }
                catch {
                    ErrorMessage = "Unknown Error";
                }
                try {                    
                    FailureSeverity failureSeverity = failureMessageAccessor.GetSeverity();
                    ErrorSeverity = failureSeverity.ToString();
                    // delete all of the warning level failures and roll back any others
                    if (failureSeverity == FailureSeverity.Warning) {
                        failuresAccessor.DeleteWarning(failureMessageAccessor);
                    }
                    else {
                        return FailureProcessingResult.ProceedWithRollBack;
                    }   
                }
                catch { }
            }
            return FailureProcessingResult.Continue;
        }        
    }
}

