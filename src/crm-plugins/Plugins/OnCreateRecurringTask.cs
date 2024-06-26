using Crm.Entities;
using Erp.RealEstate.Plugins.BusinessLogic;
using Microsoft.Xrm.Sdk;
using System;

namespace Erp.RealEstate.Plugins.Plugins
{
    public class OnCreateRecurringTask : IPlugin
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

            if (context.PostEntityImages.ContainsKey("TargetImage"))
            {
                var taskImage = context.PostEntityImages["TargetImage"];
                var taskModel = taskImage.ToEntity<Task>();

                RecurringTasksLogic.CreateNextTask(orgService, tracingService, taskModel);
            }
        }
    }
}
