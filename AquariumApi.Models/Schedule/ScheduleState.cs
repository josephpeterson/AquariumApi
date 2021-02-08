using AquariumApi.Models;
using System.Collections.Generic;

namespace AquariumApi.Models
{
    public class ScheduleState {
        public bool Running;
        public DeviceScheduleTask NextTask;
        public List<DeviceSchedule> Schedules;
        public int TaskCount;
        public List<DeviceScheduleTask> ScheduledTasks;
    }
}














