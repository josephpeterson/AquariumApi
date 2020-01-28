using System.Collections.Generic;
using System.Text;

namespace AquariumApi.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public int TargetId { get; set; }
        public bool Dismissed { get; set; }


        public DispatchedNotification Source { get; set; }
        public AquariumUser Target { get; set; }
    }
}
