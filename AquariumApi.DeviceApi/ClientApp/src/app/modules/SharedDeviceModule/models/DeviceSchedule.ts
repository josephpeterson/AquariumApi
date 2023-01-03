import { DeviceConfiguration } from './DeviceConfiguration'
import { DeviceScheduleTask } from './DeviceScheduleTask'

export class DeviceSchedule
{
  id: number //= Math.floor(Math.random() * 100);
  name: string
  deviceId: number
  startTime: string

  tasks: DeviceScheduleTask[] = []

  repeat: boolean
  repeatInterval: number
  repeatEndTime: string
  dateConditions: string

  static expandSchedule(schedule: DeviceSchedule, device: DeviceConfiguration):DeviceSchedule {
    var schedule = {...schedule}
    if(!schedule.tasks)
      schedule.tasks = [];
    schedule.tasks = schedule.tasks.map(t => {
      return DeviceScheduleTask.expandTask(t,device);
    })
    return schedule;
  }
}
