import { DeviceScheduleTask } from "./DeviceScheduleTask";

export class DeviceScheduledJob {
    public id: number;
    public deviceId: number;
    public taskId: number;
    public status: string;
    public endReason: string;
    public endTime: string;
    public maximumEndTime: number;
    public updatedAt: string;

    public task: DeviceScheduleTask;
  }
  