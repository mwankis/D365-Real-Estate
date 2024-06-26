using Crm.Erp.RealEstate.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Linq;

namespace Erp.RealEstate.Processes
{
    public class ProcessNextContactDate : CodeActivity
    {      

        [ReferenceTarget("opportunity")]
        [Input("RegardingOpportunity")]
        public InArgument<EntityReference> RegardingOpportunity { get; set; }               

        protected override void Execute(CodeActivityContext executionContext)
        {
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var organizationService = serviceFactory.CreateOrganizationService(null);
            var regardingObj = RegardingOpportunity.Get(executionContext);          

            var earliestDateObj = GetEarliestDate(organizationService, regardingObj);
            var opportunityToUpdate = new Entity("opportunity")
            {
                Id = regardingObj.Id
            };
            if (earliestDateObj.ActivityCount == 0)
            {
                opportunityToUpdate["erp_nextcontact"] = null;
            }
            else
            {
                opportunityToUpdate["erp_nextcontact"] = earliestDateObj.DueDate;
            }
            organizationService.Update(opportunityToUpdate);
        }

        private DueDateObj GetEarliestDate(IOrganizationService organizationService,
            EntityReference regardingObj)
        {
            var ae_opportunityid = regardingObj.Id;
            var query_statecode = 0;
            var query_statecode_2 = 3;

            var query = new QueryExpression("activitypointer");
            query.ColumnSet.AddColumns("subject", "activityid", "scheduledend");
            query.AddOrder("scheduledend", OrderType.Ascending);
            query.Criteria.AddCondition("statecode", ConditionOperator.In, 
                query_statecode, query_statecode_2);
            query.TopCount = 1;

            var ae = query.AddLink("opportunity", "regardingobjectid", "opportunityid");
            ae.EntityAlias = "ae";
            ae.LinkCriteria.AddCondition("opportunityid", ConditionOperator.Equal, ae_opportunityid);

            var entityCollection = organizationService.RetrieveMultiple(query);
            if (entityCollection.Entities.Count == 0)
            {
                return new DueDateObj
                {
                    ActivityCount = 0
                };
            }

            var activityEntity = entityCollection.Entities.First();
            var activityModel = activityEntity.ToEntity<ActivityPointer>();
            return new DueDateObj{
              DueDate =  activityModel.ScheduledEnd.Value,
              ActivityCount = 1
            };
        }       
    }

    public class DueDateObj
    {
        public DateTime DueDate { get; set; }

        public int ActivityCount { get; set; } 
    }
}
