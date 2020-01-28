import { DeviceScheduleTask } from './DeviceScheduleTask'

export class DeviceSchedule
{
  id: number //= Math.floor(Math.random() * 100);
  name: string
  deviceId: number
  deployed: boolean

  tasks: DeviceScheduleTask[] = []
}
