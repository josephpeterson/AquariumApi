import { DeviceScheduledJob, RunningScheduledJob } from "./DeviceScheduledJob"

export class DeviceScheduleState
{
  public running: boolean
  public nextTasks: DeviceScheduledJob[]
  public scheduled: DeviceScheduledJob[]
  public runningJobs: RunningScheduledJob[]
}
