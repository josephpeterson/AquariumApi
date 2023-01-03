using System;
using System.Collections.Generic;
using System.Text;

namespace AquariumApi.Models.Constants
{
    /// <summary>
    /// These are the endpoints that the AquariumApi will send to the DeviceApi
    /// </summary>
    public static class MixingStationEndpoints
    {
        public const string PING =          "/actionStatus";
        public const string TEST_VALVE =          "/test?valveId={valveId}";
        public const string STOP =          "/stop";
    }
}
