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

        public const string ACCOUNT_RETRIEVE_ACTIVITY = "/v1/Activity/{activityId}";

        public const string DEVICE_RETRIEVE = "/v1/Device/{deviceId}";
        public const string DEVICE_DELETE = "/v1/Device/{deviceId}/Delete";
        public const string DEVICE_UPDATE = "/v1/Device/Update";
        public const string DEVICE_CREATE = "/v1/Device/Add";
        public const string DEVICE_DISPATCH_SCAN = "/v1/Device/{deviceId}/Scan";
        public const string DEVICE_DISPATCH_PING = "/v1/Device/{deviceId}/Ping";
        public const string DEVICE_DISPATCH_AUTH_RENEW = "/v1/Device/{deviceId}/Renew";

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
        public const string DEVICE_SCHEDULE_STATUS = "/v1/Device/{deviceId}/Schedule/Status";
        public const string DEVICE_DISPATCH_TASK = "/v1/Device/{deviceId}/Schedule/PerformTask";
        public const string DEVICE_TASK_CREATE = "/v1/Device/{deviceId}/Task/Create";
        public const string DEVICE_TASK_DELETE = "/v1/Device/{deviceId}/Task/{taskId}/Delete";


        public const string AQUARIUM_RETRIEVE_ALL = "/v1/Aquarium/All";
        public const string AQUARIUM_RETRIEVE_DETAILED = "/v1/Aquarium/{id}";
        public const string AQUARIUM_CREATE = "/v1/Aquarium/Add";
        public const string AQUARIUM_UPDATE = "/v1/Aquarium/Update";
        public const string AQUARIUM_DELETE = "/v1/Aquarium/Delete";
        public const string AQUARIUM_RETRIEVE_TEMPERATURE = "/v1/Aquarium/TemperatureHistogram";
        public const string AQUARIUM_RETRIEVE_TEMPERATURE_ALL = "/v1/Aquarium/TemperatureHistogram/All";
        public const string AQUARIUM_RETRIEVE_SNAPSHOTS = "/v1/Aquarium/{aquariumId}/Snapshots";
        public const string AQUARIUM_DELETE_SNAPSHOTS = "/v1/Aquarium/{aquariumId}/Snapshots";

        public const string AUTH_RENEW = "/v1/Auth/Renew";
        public const string AUTH_LOGIN = "/v1/Auth/Login";
        public const string AUTH_LOGIN_DEVICE = "/v1/Auth/Login/Device";
        public const string AUTH_SIGNUP = "/v1/Auth/Signup";
        public const string AUTH_PASSWORD_RESET = "/v1/Auth/PasswordReset/Attempt";
        public const string AUTH_PASSWORD_RESET_UPGRADE = "/v1/Auth/PasswordReset/Upgrade";
        public const string AUTH_PASSWORD_RESET_SUBMIT = "/v1/Auth/PasswordReset/Submit";

        public const string BUG_SUBMIT = "/v1/Bug/Submit";

        public const string DEVICE_ATO_STATUS = "/v1/Device/{deviceId}/ATO";
        public const string DEVICE_ATO_HISTORY = "/v1/Device/{deviceId}/ATO/History";
        public const string DEVICE_ATO_RUN = "/v1/Device/{deviceId}/ATO";
        public const string DEVICE_ATO_STOP = "/v1/Device/{deviceId}/ATO/Stop";

        public const string SCHEDULE_CREATE = "/v1/Device/{deviceId}/Schedule/Create";
        public const string SCHEDULE_RETRIEVE = "/v1/Schedule";
        public const string SCHEDULE_RETRIEVE_TASKTYPES = "/v1/Schedule/Tasks";
        public const string SCHEDULE_RETRIEVE_SCHEDULED_JOBS = "/v1/Device/{deviceId}/Schedule/Jobs";
        public const string SCHEDULE_DELETE = "/v1/Device/{deviceId}/Schedule/{scheduleId}/Delete";
        public const string SCHEDULE_UPDATE = "/v1/Schedule/Update";
        public const string SCHEDULE_SCHEDULED_JOB_STOP = "/v1/Device/{deviceId}/Schedule/Job/Stop";
        public const string SCHEDULE_RETRIEVE_SCHEDULED_JOBS_ON_DEVICE = "/v1/Device/{deviceId}/Schedule/Job/Deployed";


    }
}
