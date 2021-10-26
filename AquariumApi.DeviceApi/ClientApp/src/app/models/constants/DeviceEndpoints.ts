export class DeviceEndpoints {
    static PING: string =          "/v1/Device/Ping";
    static UPDATE: string =        "/v1/Device/Update";
    static REBOOT: string =        "/v1/Device/Reboot";
    static LOG: string =           "/v1/Device/Log";
    static LOG_CLEAR: string =     "/v1/Device/Log/Clear";

    static AUTH_CURRENT: string =  "/v1/Auth/";
    static AUTH_RENEW: string =    "/v1/Auth/Renew";
    static AUTH_LOGIN: string =    "/v1/Auth/Login";
    static AUTH_LOGOUT: string =   "/v1/Auth/Logout";

    static WATER_CHANGE_BEGIN: string =    "/v1/WaterChange/ATO";
    static WATER_CHANGE_STOP: string =     "/v1/WaterChange/ATO/Stop";
    static WATER_CHANGE_STATUS: string =   "/v1/WaterChange/ATO/Status";

    static DEVICE_SENSOR_TEST: string =        "/v1/Sensor/Test/";

    static SCHEDULE_START: string =            "/v1/Schedule/Start/";
    static SCHEDULE_STOP: string =             "/v1/Schedule/Stop/";
    static SCHEDULE_STATUS: string =           "/v1/Schedule/Status/";
    static SCHEDULE_TASK_PERFORM: string =     "/v1/Schedule/Tasks/Perform";
    static SCHEDULE_REMAINING_TASKS: string =  "/v1/Schedule/Tasks/Remaining";
}