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
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }

        [ForeignKey("AuthorId")]
        public AquariumUser Author { get; set; }

        [ForeignKey("ScheduleId")]
        public ICollection<DeviceScheduleTask> Tasks { get; set; }

        [ForeignKey("ScheduleId")]
        public ICollection<DeviceScheduleAssignment> ScheduleAssignments { get; set; }

        [NotMapped]
        public bool Running { get; set; }


        public List<DeviceScheduleTask> ExpandTasks()
        {
            if (Tasks == null) return new List<DeviceScheduleTask>();

            var tasks = new List<DeviceScheduleTask>();
            var indvidualTasks = Tasks.ToList();
            indvidualTasks.ForEach(t =>
            {
                //Reset the day
                tasks.Add(new DeviceScheduleTask()
                {
                    TaskId = t.TaskId,
                    StartTime = t.StartTime,
                    ScheduleId = Id,
                    Schedule = this
                });
                if (t.Interval != null)
                {
                    var lengthInMinutes = TimeSpan.FromDays(1).TotalMinutes;

                    if (t.EndTime.HasValue)
                        lengthInMinutes = t.EndTime.Value.TimeOfDay.Subtract(t.StartTime.TimeOfDay).TotalMinutes;

                    var mod = lengthInMinutes % t.Interval;
                    for (var i = 1; i < ((lengthInMinutes - mod) / t.Interval); i++)
                    {
                        tasks.Add(new DeviceScheduleTask()
                        {
                            TaskId = t.TaskId,
                            StartTime = t.StartTime.AddMinutes(i * t.Interval.Value),
                            ScheduleId = Id,
                            Schedule = this
                        });
                    }
                }
            });
            return tasks.OrderBy(t => t.StartTime).ToList();
        }
    }
}
