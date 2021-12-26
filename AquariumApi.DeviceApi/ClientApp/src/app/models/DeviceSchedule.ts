import { DeviceScheduleTask } from './DeviceScheduleTask'
import { DeviceScheduleTaskAssignment } from './DeviceScheduleTaskAssignment'

export class DeviceSchedule
{
  id: number //= Math.floor(Math.random() * 100);
  name: string
  deviceId: number
  deployed: boolean

  taskAssignments: DeviceScheduleTaskAssignment[] = []
}
