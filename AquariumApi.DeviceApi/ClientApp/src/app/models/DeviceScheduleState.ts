import { DeviceSchedule } from './DeviceSchedule'
import { FutureTask } from './FutureTask'

export class DeviceScheduleState
{
  public running: boolean
  public nextTask: FutureTask
  public schedules: DeviceSchedule[]
}
