using System;
using System.Collections.Generic;

namespace AquariumApi.Models
{
    public class DispatchedNotification
    {
        public int Id { get; set; }
        public NotificationTypes Type { get; set; }
        public int DispatcherId { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }

        public ICollection<Notification> Notifications { get; set; }
        public AquariumUser Dispatcher { get; set; }
    }
}
