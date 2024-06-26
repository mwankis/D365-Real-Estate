using Crm.Entities;
using Microsoft.Xrm.Sdk;
using System;

namespace Erp.RealEstate.Plugins.BusinessLogic
{
    public static class RecurringTasksLogic
    {
        public static void CreateNextTask(IOrganizationService organizationService,
            ITracingService tracingService, Task task)
        {
            try
            {
                if (task.erp_recurring.HasValue && task.erp_recurring.Value
                    && task.erp_enddate.HasValue)
                {
                    tracingService.Trace($"Task with id: {task.Id} is recurring");
                    var dueDateDay = task.ScheduledEnd.Value.Day;
                    var taskNextDueDate = GetNextDueDate(task.ScheduledEnd.Value,
                        task.erp_recurrencepatten.Value, dueDateDay);
                    tracingService.Trace("First due date: " + taskNextDueDate);
                    EntityReference previousTask = task.ToEntityReference();
                    do
                    {
                        var newTask = new Task()
                        {
                            erp_enddate = task.erp_enddate,
                            erp_NewSubject = task.erp_NewSubject,
                            erp_previoustask = previousTask,
                            erp_parenttask = task.ToEntityReference(),
                            RegardingObjectId = task.RegardingObjectId,
                            erp_timeless = task.erp_timeless,
                            Description = task.Description,
                            erp_recurrencepatten = task.erp_recurrencepatten.Value,
                            ScheduledStart = task.ScheduledStart,
                            Subject = task.Subject
                        };
                        newTask["erp_timeless"] = task["erp_timeless"];
                        newTask.ScheduledEnd = taskNextDueDate;
                        var rec = (int)task.erp_recurrencepatten.Value;
                        newTask["erp_recurrencepatten"] = new OptionSetValue(rec);
                        tracingService.Trace("Attempting to create a task..");
                        var id = organizationService.Create(newTask);
                        var updateTask = new Task
                        {
                            Id = id
                        };
                        updateTask.erp_recurring = task.erp_recurring.Value;
                        organizationService.Update(updateTask);
                        tracingService.Trace($"Task created: {id}");
                        taskNextDueDate = GetNextDueDate(taskNextDueDate,
                        task.erp_recurrencepatten.Value, dueDateDay);
                        previousTask = new EntityReference(Task.EntityLogicalName, id);
                        tracingService.Trace("Next First due date: " + taskNextDueDate);
                    } while (taskNextDueDate.Date <= task.erp_enddate.Value.Date);
                }

            }
            catch (InvalidPluginExecutionException ex)
            {
                tracingService.Trace("Error msg: " + ex.Message);
                throw new InvalidPluginExecutionException(ex.Message);
            }

            catch (Exception ex)
            {
                tracingService.Trace("Error msg: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        private static DateTime GetNextDueDate(DateTime curDueDate,
            Task_erp_recurrencepatten recurrencepatten, int dueDateDay)
        {
            DateTime dueDate = DateTime.Now;
            switch (recurrencepatten)
            {
                case Task_erp_recurrencepatten.Quarterly:
                    var dueDateDaily = curDueDate.AddMonths(3);
                    dueDate = GetDueDate(dueDateDaily);
                    break;

                case Task_erp_recurrencepatten.Weekly:
                    var dueDateWeekly = curDueDate.AddDays(7);
                    dueDate = GetDueDate(dueDateWeekly);
                    break;

                case Task_erp_recurrencepatten.Monthly:
                    var dueDateMonthly = curDueDate.AddMonths(1);
                    dueDate = GetMonthlyNextDueDate(dueDateMonthly, dueDateDay);
                    break;

                case Task_erp_recurrencepatten.SemiAnnual:
                    var dueDateSemiAnnual = curDueDate.AddMonths(6);
                    dueDate = GetDueDate(dueDateSemiAnnual);
                    break;

                case Task_erp_recurrencepatten.Yearly:
                    var dueDateYearly = curDueDate.AddYears(1);
                    dueDate = GetDueDate(dueDateYearly);
                    break;

                default:
                    break;
            }
            
            return new DateTime(dueDate.Year, dueDate.Month, dueDate.Day, 7,00,00);
        }

        private static DateTime GetMonthlyNextDueDate(DateTime date, int day)
        {
            if (date.Day == 1)
            {
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                return firstDayOfMonth;
            }
            else
            {
                try
                {
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, day);
                    return firstDayOfMonth;
                }
                catch (Exception)
                {
                    return date;
                }              
            }           
        }

        private static DateTime GetDueDate(DateTime dateTime)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Saturday)
            {
                return dateTime.AddDays(2);
            }
            else if (dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return dateTime.AddDays(1);
            }
            return dateTime;
        }
      
    }
}
