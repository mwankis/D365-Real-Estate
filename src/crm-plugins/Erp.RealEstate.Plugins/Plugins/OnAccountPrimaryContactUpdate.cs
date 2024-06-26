using Crm.Entities;
using Microsoft.Xrm.Sdk;
using System;

namespace Erp.RealEstate.Plugins.Plugins
{
    public class OnAccountPrimaryContactUpdate : IPlugin
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

            if (context.PostEntityImages.ContainsKey("TargetImage") &&
                context.PreEntityImages.ContainsKey("TargetImage"))
            {
                var postContactImage = context.PostEntityImages["TargetImage"];
                var preContactImage = context.PreEntityImages["TargetImage"];

                var postAccountModel = postContactImage.ToEntity<Account>();
                var preAccountModel = preContactImage.ToEntity<Account>();

                if (postAccountModel.PrimaryContactId != null &&
                    preAccountModel.PrimaryContactId == null)
                {
                    var contactToUpdate = new Entity("contact")
                    {
                        Id = postAccountModel.PrimaryContactId.Id
                    };
                    contactToUpdate["erp_isprimarycontact"] = true;
                    orgService.Update(contactToUpdate);
                }

                if (postAccountModel.PrimaryContactId != null &&
                    preAccountModel.PrimaryContactId != null)
                {
                    var postContactToUpdate = new Entity("contact")
                    {
                        Id = postAccountModel.PrimaryContactId.Id
                    };
                    postContactToUpdate["erp_isprimarycontact"] = true;
                    orgService.Update(postContactToUpdate);

                    var preContactToUpdate = new Entity("contact")
                    {
                        Id = preAccountModel.PrimaryContactId.Id
                    };
                    preContactToUpdate["erp_isprimarycontact"] = false;
                    orgService.Update(preContactToUpdate);
                }

                if (postAccountModel.PrimaryContactId == null &&
                    preAccountModel.PrimaryContactId != null)
                {
                    var preContactToUpdate = new Entity("contact")
                    {
                        Id = preAccountModel.PrimaryContactId.Id
                    };
                    preContactToUpdate["erp_isprimarycontact"] = false;
                    orgService.Update(preContactToUpdate);
                }
            }
        }
    }
}
