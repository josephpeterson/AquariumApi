using AquariumApi.Models;
using System.Collections.Generic;

namespace AquariumApi.Models
{
    public class ScheduleState {
        public bool Running;
        public FutureTask NextTask;
        public List<DeviceSchedule> Schedules;
        }
}














