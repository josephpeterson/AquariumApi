using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblDeviceSchedule")]
    public class DeviceSchedule
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int DeviceId { get; set; }
        public DateTime StartTime { get; set; }
        public List<DeviceScheduleTask> Tasks { get; set; }
        public bool Repeat { get; set; }
        public int RepeatInterval { get; set; }
        public DateTime RepeatEndTime { get; set; }
        public string DateConditions{ get; set; }


        [ForeignKey("DeviceId")]
        public AquariumDevice Device { get; set; }

        /*
        public List<ScheduledJob> ExpandTasks(DateTime startDate)
        {
            var allScheduledJobs = new List<ScheduledJob>();

            //get all task assignments that start by the time trigger
            var taskAssignment = Tasks
                .Where(ta => ta.TriggerTypeId == TriggerTypes.Time)
                .ToList();



            taskAssignment.ForEach(ta =>
            {
                var tod = ta.StartTime.TimeOfDay;
                var startTime = startDate.Date + tod;
                if (startTime < startDate)
                    startTime = startTime.AddDays(1);


                var task = ta.Task;

                var scheduledJob = new ScheduledJob()
                {
                    TaskId = task.Id.Value,
                    Task = task,
                    StartTime = startTime,
                    //ScheduleId = Id
                };
                allScheduledJobs.Add(scheduledJob);

                //Check if this task assignment repeats
                if (ta.Repeat)
                {
                    var lengthInMinutes = TimeSpan.FromDays(1).TotalMinutes;
                    
                    lengthInMinutes = ta.RepeatEndTime.TimeOfDay.Subtract(ta.StartTime.TimeOfDay).TotalMinutes;

                    var mod = lengthInMinutes % ta.RepeatInterval;
                    for (var i = 1; i <= ((lengthInMinutes - mod) / ta.RepeatInterval); i++)
                    {
                        allScheduledJobs.Add(new ScheduledJob()
                        {
                            TaskId = scheduledJob.TaskId,
                            Task = scheduledJob.Task,
                            StartTime = scheduledJob.StartTime.AddMinutes(i * ta.RepeatInterval.Value),
                            //ScheduleId = Id
                        });
                    }
                }
            });
            return allScheduledJobs.OrderBy(t => t.StartTime).ToList();
        }
        */
    }
}
