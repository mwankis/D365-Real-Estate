using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System.Text.RegularExpressions;

namespace Erp.RealEstate.Processes
{
    public class FixOpportunityNotes : CodeActivity
    {      

        [ReferenceTarget("opportunity")]
        [Input("RegardingOpportunity")]
        public InArgument<EntityReference> RegardingOpportunity { get; set; }               

        protected override void Execute(CodeActivityContext executionContext)
        {
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var organizationService = serviceFactory.CreateOrganizationService(null);
            var regardingObj = RegardingOpportunity.Get(executionContext);
            UpdateOpportunityNotes(organizationService, regardingObj);
        }

        public void UpdateOpportunityNotes(IOrganizationService organizationService, EntityReference opp)
        {
            string opportunityNotes = string.Empty;
            var ac_opportunityid = opp.Id;
            var query = new QueryExpression("annotation");

            query.ColumnSet.AddColumns("subject", "notetext", "annotationid", "modifiedon", "createdon");
            query.AddOrder("createdon", OrderType.Ascending);

            var ac = query.AddLink("opportunity", "objectid", "opportunityid");
            ac.EntityAlias = "ac";
            ac.LinkCriteria.AddCondition("opportunityid", ConditionOperator.Equal, ac_opportunityid);

            var entityCollection = organizationService.RetrieveMultiple(query);
            foreach (var entity in entityCollection.Entities)
            {
                var noteText = entity["notetext"] as string;
                noteText = StripHtmlTags(noteText);
                if (string.IsNullOrEmpty(opportunityNotes))
                {
                    opportunityNotes += noteText;
                }
                else
                {
                    opportunityNotes += $". {noteText}";
                }
            }
            var space = @"&nbsp;";
            opportunityNotes = opportunityNotes.Replace(space, "");
            var opportunityToUpdate = new Entity("opportunity")
            {
                Id = ac_opportunityid
            };
            opportunityToUpdate["description"] = opportunityNotes;
            organizationService.Update(opportunityToUpdate);
        }

        public static string StripHtmlTags(string html)
        {      
            var inputString = @"" + html;
            var indexOf = inputString.IndexOf("</style>");
            var textWithoutStyles = inputString;
            if (indexOf != -1)
            {
                textWithoutStyles = inputString.Substring(indexOf + 8);
            }
            var textWithoutDivs = Regex.Replace(textWithoutStyles, "<.*?>", "");
            var notes = textWithoutDivs.Replace("&nbsp;", string.Empty).Replace("amp;", string.Empty); ;
            return notes;
        }
       
    }

  
}
