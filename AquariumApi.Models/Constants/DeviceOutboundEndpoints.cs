using System;
using System.Collections.Generic;
using System.Text;

namespace AquariumApi.Models.Constants
{
    /// <summary>
    /// These are the endpoints that the AquariumApi will send to the DeviceApi
    /// </summary>
    public class DeviceOutboundEndpoints
    {
        public const string PING =          "/v1/Device/Ping";
        public const string UPDATE =        "/v1/Device/Update";
        public const string SYSTEM_REBOOT =        "/v1/Device/Reboot";
        public const string SYSTEM_LOG_RETRIEVE =           "/v1/Device/Log";
        public const string SYSTEM_LOG_CLEAR =     "/v1/Device/Log/Clear";
        public const string SYSTEM_FACTORY_RESET =     "/v1/Device/FactoryReset";
        public const string MIXING_STATION_UPDATE = "/v1/Device/MixingStation";
        public const string MIXING_STATION_DELETE = "/v1/Device/MixingStation";
        public const string MIXING_STATION_STATUS = "/v1/Device/MixingStation";
        public const string MIXING_STATION_TEST_VALVE = "/v1/Device/MixingStation/Test/{valveId}";
        public const string MIXING_STATION_PING = "/v1/Device/MixingStation/Ping";
        public const string MIXING_STATION_SEARCH = "/v1/Device/MixingStation/Search";
        public const string MIXING_STATION_STOP = "/v1/Device/MixingStation/Stop";


        public const string AUTH_CURRENT =  "/v1/Auth/";
        public const string AUTH_RENEW =    "/v1/Auth/Renew";
        public const string AUTH_LOGIN =    "/v1/Auth/Login";
        public const string AUTH_LOGOUT =   "/v1/Auth/Logout";

        public const string SELECT_FORM_TYPES = "/v1/Form/Select/{selectType}";


        public const string WATER_CHANGE_BEGIN =    "/v1/WaterChange/ATO";
        public const string WATER_CHANGE_STOP =     "/v1/WaterChange/ATO/Stop";
        public const string WATER_CHANGE_STATUS =   "/v1/WaterChange/ATO/Status";

        public const string SENSOR_UPDATE = "/v1/Sensor";
        public const string SENSOR_DELETE = "/v1/Sensor/Delete";
        public const string SENSOR_RETRIEVE = "/v1/Sensor/Values";
        public const string SENSOR_TEST =        "/v1/Sensor/Test/";

        public const string SCHEDULE_UPSERT =  "/v1/Schedule";
        public const string SCHEDULE_DELETE =  "/v1/Schedule/Delete";
        public const string SCHEDULE_RETRIEVE =  "/v1/Schedule/All";
        public const string SCHEDULE_RETRIEVE_TASK_TYPES =  "/v1/Schedule/Task/Types";
        public const string SCHEDULE_START =            "/v1/Schedule/Start/";
        public const string SCHEDULE_STOP =             "/v1/Schedule/Stop/";
        public const string SCHEDULE_STATUS =           "/v1/Schedule/Status/";
        public const string SCHEDULE_TASK_PERFORM =     "/v1/Schedule/Tasks/Perform";
        public const string SCHEDULE_REMAINING_TASKS =  "/v1/Schedule/Tasks/Remaining";

        public const string SCHEDULE_SCHEDULEDJOB_STOP = "/v1/Schedule/Tasks/Stop";

        public const string TASK_RETRIEVE = "/v1/Tasks";
        public const string TASK_UPSERT = "/v1/Tasks";
        public const string TASK_DELETE = "/v1/Tasks/Delete";


        public const string HARDWARE_RETRIEVE_CAMERA_DEVICES = "/v1/Hardware/CameraDevices";
        public const string HARDWARE_TAKE_PHOTO = "/v1/Hardware/Camera/Take";


    }
}
