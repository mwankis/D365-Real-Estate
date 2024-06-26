using Crm.Entities;
using Erp.RealEstate.Plugins.BusinessLogic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Text.RegularExpressions;

namespace Erp.RealEstate.Plugins.Plugins
{
    public class OnNotesCreateOrUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider
                          .GetService(typeof(IPluginExecutionContext));
            var tracingService = (ITracingService)serviceProvider
                                 .GetService(typeof(ITracingService));

            var serviceFactory =
                  (IOrganizationServiceFactory)serviceProvider.GetService
                  (typeof(IOrganizationServiceFactory));
            var orgService = serviceFactory.CreateOrganizationService(context.UserId);
            try
            {
                if (context.PostEntityImages.ContainsKey("TargetImage"))
                {
                    var noteImage = context.PostEntityImages["TargetImage"];
                    UpdateOpportunityNotes(orgService, noteImage);
                }
                else if(context.PreEntityImages.ContainsKey("TargetImage"))
                {
                    var noteImage = context.PreEntityImages["TargetImage"];
                    UpdateOpportunityNotes(orgService, noteImage);
                }               
            }
            catch (Exception ex)
            {
               tracingService.Trace("Something went wrong: " + ex.Message);
            }            
        }

        public void UpdateOpportunityNotes(IOrganizationService organizationService, Entity note)
        {
            var regarding = note.GetAttributeValue<EntityReference>("objectid");
            if (regarding.LogicalName.Equals("opportunity"))
            {
                string opportunityNotes = string.Empty;
                var ac_opportunityid = regarding.Id;
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
            var notes = textWithoutDivs.Replace("&nbsp;", string.Empty).Replace("amp;",string.Empty);
            return notes;
        }
    }
}
