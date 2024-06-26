using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace Erp.RealEstate.Processes
{
    public class CompareTwoDates : CodeActivity
    {
        [Input("FirstDate")]
        public InArgument<DateTime> FirstDate { get; set; }


        [Input("SecondDate")]
        public InArgument<DateTime> SecondDate { get; set; }


        [Output("Result")]
        public OutArgument<bool> Result { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var firstDate = FirstDate.Get(context);
            var secondDate = SecondDate.Get(context);

            var result =  firstDate < secondDate;
            context.SetValue(Result, result);
        }
    }
}
