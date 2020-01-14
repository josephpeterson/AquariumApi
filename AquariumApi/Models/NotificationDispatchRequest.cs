using System.Collections.Generic;
using AquariumApi.Models;

namespace AquariumApi.Controllers
{
    public class NotificationDispatchRequest
    {
        public List<int> AccountIds { get; set; }
        public DispatchedNotification Notification { get; set; }
    }
}