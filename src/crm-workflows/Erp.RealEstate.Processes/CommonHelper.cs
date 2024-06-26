using Crm.Erp.RealEstate.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Erp.RealEstate.Processes
{
    public static class CommonHelper
    {

        public static void CreateTasksSeries(IOrganizationService organizationService,
            EntityReference regardingObj, int numberOfYears, DateTime leaseExpriyDate, 
            EntityReference taskOwner)
        {
            var tasksConfigs = GetTaskSeriesConfigs(organizationService);

            foreach (var tasksConfig in tasksConfigs)
            {
                var numberOfMonths = tasksConfig.erp_numberofmonths.Value;
                var subject = tasksConfig.erp_tasksubject;
                var dueDate = GetDueDate(numberOfMonths, leaseExpriyDate);

                if (dueDate <= leaseExpriyDate)
                {
                    if (numberOfMonths == 0)
                    {
                        CreateTask(subject, false, regardingObj, dueDate, organizationService, taskOwner);
                    }
                    else
                    {
                        CreateTask(subject, true, regardingObj, dueDate, organizationService, taskOwner);
                    }
                }
               
            }
        }

        private static List<erp_taskseriesconfig> GetTaskSeriesConfigs(IOrganizationService organizationService)
        {
            var query = new QueryExpression("erp_taskseriesconfig");

            query.ColumnSet.AddColumns("erp_taskseriesconfigid", "erp_tasksubject", "erp_numberofmonths");

            var ac = query.AddLink("erp_activityseriesconfig", "erp_activityseriesconfigid", "erp_activityseriesconfigid");
            ac.EntityAlias = "ac";

            var entityCollection = organizationService.RetrieveMultiple(query);

            var taskseriesconfigs = entityCollection.Entities.Select(x => x.ToEntity<erp_taskseriesconfig>());

            return taskseriesconfigs.ToList();
        }

        private static void CreateTask(string subject, bool rollOver,
            EntityReference regardingObj,DateTime dueDate, 
            IOrganizationService organizationService, EntityReference taskOwner)
        {
            try
            {
                var currentDate = DateTime.UtcNow;
                if (dueDate > currentDate)
                {
                    var newTask = new Task()
                    {
                        RegardingObjectId = regardingObj,
                        erp_timeless = rollOver,
                        Description = subject,
                        Subject = subject,
                        ScheduledEnd = dueDate,
                        OwnerId = taskOwner
                    };
                    organizationService.Create(newTask);
                }               
            }
            catch (Exception)
            {

            }            
        }
        public static DateTime GetDueDate(int numberOfMonth,
            DateTime leaseExpriyDate)
        {
            var dueDate = leaseExpriyDate.AddMonths(-numberOfMonth);
            return new DateTime(dueDate.Year, dueDate.Month, dueDate.Day, 7, 00, 00);
        }

    }

   
}
