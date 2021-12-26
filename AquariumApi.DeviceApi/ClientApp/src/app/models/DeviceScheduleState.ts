import { DeviceSchedule } from './DeviceSchedule'
import { DeviceScheduleTask } from './DeviceScheduleTask'
import { FutureTask } from './FutureTask'

export class DeviceScheduleState
{
  public running: boolean
  public nextTask: DeviceScheduleTask
  public schedules: DeviceSchedule[]
  public taskCount: number
}
