namespace AquariumApi.Models.Constants
{
    /// <summary>
    /// These endpoints are the endpoints the DeviceAPI will call the AquariumAPI
    /// </summary>
    public class DeviceInboundEndpoints
    {
        public const string CONNECT_DEVICE = "/v1/DeviceInteraction/Device";
        public const string RECEIVE_PING = "/v1/DeviceInteraction/Device";
        public const string DISPATCH_SNAPSHOT = "/v1/DeviceInteraction/Snapshot";
        public const string DISPATCH_ATO = "/v1/DeviceInteraction/ATO";
        public const string DISPATCH_WATERCHANGE = "/v1/DeviceInteraction/WaterChange";
        public const string DISPATCH_EXCEPTION = "/v1/DeviceInteraction/Exception";
        public const string DISPATCH_SCHEDULEDJOB = "/v1/DeviceInteraction/Schedule/Jobs";
    }
}
