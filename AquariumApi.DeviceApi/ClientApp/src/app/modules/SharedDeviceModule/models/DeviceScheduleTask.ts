import { DeviceScheduleTaskTypes } from "../../../models/types/DeviceScheduleTaskTypes"
import { DeviceSensor } from "./DeviceSensor"
import { GpioPinValue } from "./GpioPinValue"
import { DeviceAction } from "./DeviceAction"
import { DeviceConfiguration } from "./DeviceConfiguration"

export class DeviceScheduleTask {
  id: number //= Math.floor(Math.random() * 100);
  name: string
  actions: DeviceAction[] = []
  triggerSensorId: number | null
  triggerSensor: DeviceSensor | null
  triggerSensorValue: GpioPinValue | null
  maximumRuntime: number


  static expandTask(task: DeviceScheduleTask, device: DeviceConfiguration):DeviceScheduleTask {
    var task = {...task}
    task.actions = task.actions.map(a => {
      if(isNaN(a.targetSensorId)) return {...a}
      return {
        ...a,
        targetSensor: device.sensors.filter(x => x.id == a.targetSensorId)[0]
      }
    })
    if(isNaN(task.triggerSensorId)) return {...task}
    return {
      ...task,
      triggerSensor: device.sensors.filter(x => x.id == task.triggerSensorId)[0]
    }
  }
  /*
  static getETAForTask(task: DeviceScheduleTask) {
    var d = moment(task.startTime).diff(moment());
    return moment.duration(d).humanize();
  }
  */
}

