import { DeviceScheduleTask } from "./DeviceScheduleTask";
import { JobStatus } from "./types/JobStatus";

export class DeviceScheduledJob {
    public id: number;
    public deviceId: number;
    public taskId: number;
    public status: JobStatus;
    public endReason: string;
    public endTime: string;
    public maximumEndTime: number;
    public updatedAt: string;

    public task: DeviceScheduleTask;
  }
  