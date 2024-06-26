using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace Erp.RealEstate.Processes
{
    public class Create1YearTermActivitySeries : CodeActivity
    {
        [RequiredArgument]
        [Input("Account")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> Account { get; set; }

        [RequiredArgument]
        [Input("TaskOwner")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> TaskOwner { get; set; }

        [RequiredArgument]
        [Input("LeaseEndDate")]
        public InArgument<DateTime> LeaseEndDate { get; set; }

        [RequiredArgument]
        [Input("NumberOfYears")]
        public InArgument<int> NumberOfYears { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            var context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var organizationService = serviceFactory.CreateOrganizationService(context.UserId);
            var account = Account.Get(executionContext);
            var taskOwner = TaskOwner.Get(executionContext);
            var leaseEndDateVal = LeaseEndDate.Get(executionContext);
            var numberOfYears = NumberOfYears.Get(executionContext);

            var leaseEndDate = new DateTime(leaseEndDateVal.Year, leaseEndDateVal.Month,
                leaseEndDateVal.Day, 7, 00, 00);

            CommonHelper.CreateTasksSeries(organizationService, account, numberOfYears, leaseEndDate, taskOwner);          
           
        }        
       
    }
}
