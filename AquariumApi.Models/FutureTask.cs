using AquariumApi.Models;
using System;

namespace AquariumApi.Models
{
    public class FutureTask
    {
        public int Index;
        public TimeSpan eta;
        public DeviceScheduleTask task;
    }
}
