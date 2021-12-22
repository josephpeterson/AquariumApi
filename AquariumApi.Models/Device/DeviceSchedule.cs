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
        [Required]
        public int? Id { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public bool Deployed { get; set; }

        [ForeignKey("DeviceId")]
        public AquariumDevice Device { get; set; }


        public ICollection<DeviceScheduleTaskAssignment> TaskAssignments { get; set; }

        [NotMapped]
        public bool Running { get; set; }


        public List<ScheduledJob> ExpandTasks(DateTime startDate)
        {
            var allScheduledJobs = new List<ScheduledJob>();

            //get all task assignments that start by the time trigger
            var taskAssignment = TaskAssignments
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
    }
}
