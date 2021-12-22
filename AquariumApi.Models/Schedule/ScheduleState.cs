using AquariumApi.Models;
using System.Collections.Generic;

namespace AquariumApi.Models
{
    public class ScheduleState {
        public bool Running;
        public ScheduledJob NextTask;
        public List<DeviceSchedule> Schedules;
        public int TaskCount;
        public List<ScheduledJob> ScheduledTasks;
    }
}














