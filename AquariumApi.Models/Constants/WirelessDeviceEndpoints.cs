using System;
using System.Collections.Generic;
using System.Text;

namespace AquariumApi.Models.Constants
{
    /// <summary>
    /// These are the endpoints that the AquariumApi will send to the DeviceApi
    /// </summary>
    public static class WirelessDeviceEndpoints
    {
        public const string PING =          "/actionStatus";
        public const string TEST_VALVE =          "/test?pin={pin}";
        public const string UPSERT_SESSION = "/action?sessionMins={sessionMins}";
        public const string STOP =          "/stop";
    }
}
