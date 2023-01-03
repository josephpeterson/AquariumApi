export class DeviceEndpoints {
  static PING = "/v1/Device/Ping";
  static UPDATE = "/v1/Device/Update";
  static SYSTEM_REBOOT = "/v1/Device/Reboot";
  static SYSTEM_LOG_RETRIEVE = "/v1/Device/Log";
  static SYSTEM_LOG_CLEAR = "/v1/Device/Log/Clear";
  static SYSTEM_FACTORY_RESET = "/v1/Device/FactoryReset";

  static AUTH_CURRENT = "/v1/Auth/";
  static AUTH_RENEW = "/v1/Auth/Renew";
  static AUTH_LOGIN = "/v1/Auth/Login";
  static AUTH_LOGOUT = "/v1/Auth/Logout";

  static SELECT_FORM_TYPES = "/v1/Form/Select/{selectType}";

  static WATER_CHANGE_BEGIN = "/v1/WaterChange/ATO";
  static WATER_CHANGE_STOP = "/v1/WaterChange/ATO/Stop";
  static WATER_CHANGE_STATUS = "/v1/WaterChange/ATO/Status";

  static SENSOR_UPDATE = "/v1/Sensor";
  static SENSOR_DELETE = "/v1/Sensor/Delete";
  static SENSOR_RETRIEVE = "/v1/Sensor/Values";
  static SENSOR_TEST = "/v1/Sensor/Test/";

  static SCHEDULE_UPSERT = "/v1/Schedule";
  static SCHEDULE_DELETE = "/v1/Schedule/Delete";
  static SCHEDULE_RETRIEVE = "/v1/Schedule/All";
  static SCHEDULE_RETRIEVE_TASK_TYPES = "/v1/Schedule/Task/Types";
  static SCHEDULE_START = "/v1/Schedule/Start/";
  static SCHEDULE_STOP = "/v1/Schedule/Stop/";
  static SCHEDULE_STATUS = "/v1/Schedule/Status";
  static SCHEDULE_TASK_PERFORM = "/v1/Schedule/Tasks/Perform";
  static SCHEDULE_REMAINING_TASKS = "/v1/Schedule/Tasks/Remaining";

  static SCHEDULE_SCHEDULEDJOB_STOP = "/v1/Schedule/Tasks/Stop";

  static TASK_RETRIEVE = "/v1/Tasks";
  static TASK_UPSERT = "/v1/Tasks";
  static TASK_DELETE = "/v1/Tasks/Delete";

  static MIXING_STATION_UPDATE = "/v1/Device/MixingStation";
  static MIXING_STATION_DELETE = "/v1/Device/MixingStation";
  static MIXING_STATION_STATUS = "/v1/Device/MixingStation";
  static MIXING_STATION_PING = "/v1/Device/MixingStation/Ping";
  static MIXING_STATION_TEST_VALVE = "/v1/Device/MixingStation/Test/{valveId}";
  static MIXING_STATION_SEARCH = "/v1/Device/MixingStation/Search";
  static MIXING_STATION_STOP = "/v1/Device/MixingStation/Stop";
}
declare global {
  interface String {
    aggregate(...params): string;
  }
}

String.prototype.aggregate = function (...params: string[]) {
  let str = String(this);
  const paramsToReplace = str.match(/(?<=\{).+?(?=\})/g);
  paramsToReplace.forEach((p, i) => {
    str = str.replace(new RegExp(`{${p}}`, 'g'), params[i]);
  });
  return str;
};