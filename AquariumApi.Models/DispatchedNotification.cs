using System;
using System.Collections.Generic;

namespace AquariumApi.Models
{
    public class DispatchedNotification
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int DispatcherId { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpireDate { get; set; }

        public ICollection<Notification> Notifications;
        public AquariumUser Dispatcher;
    }
}
