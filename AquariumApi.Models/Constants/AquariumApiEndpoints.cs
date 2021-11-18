namespace AquariumApi.Models.Constants
{
    public class AquariumApiEndpoints
    {
        public const string ADMIN_RETRIEVE_ACCOUNTS = "/v1/admin/Users";
        public const string ADMIN_RETRIEVE_BUGS = "/v1/admin/Bugs";
        public const string ADMIN_RETRIEVE_NOTIFICATIONS = "/v1/admin/Notifications";
        public const string ADMIN_NOTIFICATION_DISPATCH = "/v1/admin/Notification";
        public const string ADMIN_NOTIFICATION_DISMISS = "/v1/admin/Notification/Dismiss";
        public const string ADMIN_NOTIFICATION_DELETE_ALL = "/v1/admin/Notification/Delete";

        public const string ACCOUNT_GET_DETAILED = "/v1/Account/{id}";
        public const string ACCOUNT_GET_CLAIMS = "/v1/Account/Claims";
        public const string ACCOUNT_GET_CURRENT = "/v1/Account/Current";
        public const string ACCOUNT_RETRIEVE_NOTIFICATIONS = "/v1/Account/Notifications";
        public const string ACCOUNT_NOTIFICATION_DISMISS = "/v1/Account/Notifications/Dismiss";
        public const string ACCOUNT_UPDATE = "/v1/Account/Update";

        public const string DEVICE_RETRIEVE = "/v1/Device/{deviceId}";
        public const string DEVICE_DELETE = "/v1/Device/{deviceId}/Delete";
        public const string DEVICE_UPDATE = "/v1/Device/Update";
        public const string DEVICE_CREATE = "/v1/Device/Add";
        public const string DEVICE_DISPATCH_SCAN = "/v1/Device/{deviceId}/Scan";
        public const string DEVICE_DISPATCH_PING = "/v1/Device/{deviceId}/Ping";
        public const string DEVICE_DISPATCH_SNAPSHOT_CONFIGURATION = "/v1/Device/{deviceId}/CameraConfiguration";
        public const string DEVICE_LOG = "/v1/Device/{deviceId}/Log";
        public const string DEVICE_LOG_CLEAR = "/v1/Device/{deviceId}/Log/Clear";
        public const string DEVICE_RETRIEVE_DETAILED = "/v1/Device/{deviceId}/Information";
        public const string DEVICE_UPDATE_CONFIGURATION = "/v1/Device/{deviceId}/UpdateConfigurationFile";
        public const string DEVICE_SENSORS = "/v1/Device/{deviceId}/Sensors";
        public const string DEVICE_SENSOR_CREATE = "/v1/Device/{deviceId}/Sensor/Create";
        public const string DEVICE_SENSOR_DELETE = "/v1/Device/{deviceId}/Sensor/Remove";
        public const string DEVICE_SENSOR_UPDATE = "/v1/Device/{deviceId}/Sensor/Update";
        public const string DEVICE_SENSOR_TEST = "/v1/Device/{deviceId}/Sensor/Test";
        public const string DEVICE_SCHEDULE_DEPLOY = "/v1/Device/{deviceId}/DeploySchedule/{scheduleId}";
        public const string DEVICE_SCHEDULE_REMOVE = "/v1/Device/{deviceId}/RemoveSchedule/{scheduleId}";
        public const string DEVICE_SCHEDULE_STATUS = "/v1/Device/{deviceId}/Schedule/Status";
        public const string DEVICE_DISPATCH_TASK = "/v1/Device/{deviceId}/Schedule/PerformTask";
    }
}
