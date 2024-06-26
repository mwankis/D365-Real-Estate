using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System.Text.RegularExpressions;

namespace Erp.RealEstate.Processes
{
    public class RemoveHtmlFromString : CodeActivity
    {      

        [Input("InputString")]
        public InArgument<string> InputString { get; set; }

        [Output("OutputString")]
        public OutArgument<string> OutputString { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            var inputString = @"" + InputString.Get(executionContext);
            var indexOf = inputString.IndexOf("</style>");
            var textWithoutStyles = inputString;
            if (indexOf != -1)
            {
                textWithoutStyles = inputString.Substring(indexOf + 8);
            }           
            var textWithoutDivs = Regex.Replace(textWithoutStyles, "<.*?>", "");
            var notes = textWithoutDivs.Replace("&nbsp;", string.Empty).Replace("amp;", string.Empty); ;            
            executionContext.SetValue(OutputString, notes);
        }
    }  
}
