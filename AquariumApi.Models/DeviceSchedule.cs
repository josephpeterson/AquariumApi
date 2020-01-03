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
                var tasks = new List<DeviceScheduleTask>();
                var indvidualTasks = Tasks.ToList();
                indvidualTasks.ForEach(t =>
                {
                    tasks.Add(new DeviceScheduleTask()
                    {
                        TaskId = t.TaskId,
                        StartTime = t.StartTime,
                        ScheduleId = Id,
                        Schedule = this
                    });
                    if (t.Interval != null)
                    {
                        var endTime = t.EndTime;
                        if (endTime < t.StartTime)
                            endTime = t.StartTime.AddDays(1);
                        TimeSpan length = endTime.Subtract(t.StartTime);
                        var lengthInMinutes = length.TotalMinutes;
                        var mod = lengthInMinutes % t.Interval;

                        for (var i = 1; i < (lengthInMinutes - mod) / t.Interval; i++)
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
