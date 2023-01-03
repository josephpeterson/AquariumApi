import { DeviceSchedule } from '../modules/SharedDeviceModule/models/DeviceSchedule'
import { DeviceScheduledJob, RunningScheduledJob } from '../modules/SharedDeviceModule/models/DeviceScheduledJob'
import { DeviceScheduleTask } from '../modules/SharedDeviceModule/models/DeviceScheduleTask'
import { FutureTask } from './FutureTask'

export class DeviceScheduleState
{
  public running: boolean
  public nextTasks: DeviceScheduledJob[]
  public scheduled: DeviceScheduledJob[]
  public runningJobs: RunningScheduledJob[]
}
