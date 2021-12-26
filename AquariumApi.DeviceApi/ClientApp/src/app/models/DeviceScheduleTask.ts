import * as moment from "moment"
import { DeviceScheduleTaskTypes } from "./types/DeviceScheduleTaskTypes"
import { DeviceSensor } from "./DeviceSensor"
import { GpioPinValue } from "./types/GpioPinValue"
import { DeviceScheduleTriggerTypes } from "./types/DeviceScheduleTriggerTypes"

export class DeviceScheduleTask {
  id: number //= Math.floor(Math.random() * 100);
  name: string
  deviceId: number
  //startTime: any | null
  //endTime: any | null
  interval: number | null = 30
  taskTypeId: number
  targetSensorId: number
  targetSensor: DeviceSensor
  targetSensorValue: GpioPinValue
  endTriggerTypeId: DeviceScheduleTriggerTypes
  triggerSensorId: number | null
  triggerSensor: DeviceSensor | null
  triggerSensorValue: GpioPinValue | null

  static getTaskNameFromId(taskId: number) {
    var types = DeviceScheduleTaskTypes;
    for (var name in types) {
      if (DeviceScheduleTaskTypes[name] == `${taskId}`)
        return name;
    }
    return "Unknown";
  }
  /*
  static getETAForTask(task: DeviceScheduleTask) {
    var d = moment(task.startTime).diff(moment());
    return moment.duration(d).humanize();
  }
  */
}

