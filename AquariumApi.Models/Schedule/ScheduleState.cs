using AquariumApi.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    public class ScheduleState {
        public bool Running { get; set; }
        public List<ScheduledJob> NextTasks { get; set; }
        public List<ScheduledJob> Scheduled { get; set; } = new List<ScheduledJob>();
        public List<RunningScheduledJob> RunningJobs { get; set; } = new List<RunningScheduledJob>();
    }
    public class RunningScheduledJob
    {
        public ScheduledJob ScheduledJob { get; set; }
        public Task RunningTask { get; set; }
        [JsonIgnore]
        public CancellationTokenSource CancellationSource { get; set; }
    }
}














