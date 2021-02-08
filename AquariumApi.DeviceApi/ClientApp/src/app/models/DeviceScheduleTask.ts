export class DeviceScheduleTask {
  id: number //= Math.floor(Math.random() * 100);
  scheduleId: number
  startTime: string | null
  endTime: string | null
  interval: number | null
  taskId: number
}
export enum DeviceScheduleTaskTypes {
  Unknown = 0,
  Snapshot = 1,
  StartATO = 2,
}