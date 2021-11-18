namespace AquariumApi.Models.Constants
{
    public class DeviceInboundEndpoints
    {
        public const string CONNECT_DEVICE = "/v1/DeviceInteraction/Device";
        public const string RECEIVE_PING = "/v1/DeviceInteraction/Device";
        public const string RETRIEVE_DEVICE = "/v1/DeviceInteraction/Device";
        public const string DISPATCH_SNAPSHOT = "/v1/DeviceInteraction/Snapshot";
        public const string DISPATCH_ATO = "/v1/DeviceInteraction/ATO";
        public const string DISPATCH_EXCEPTION = "/v1/DeviceInteraction/Exception";
        public const string DISPATCH_SCHEDULEDJOB = "/v1/DeviceInteraction/Schedule/Jobs";
    }
}
