import { Aquarium } from './Aquarium';
import { DeviceSchedule } from './DeviceSchedule';
import { DeviceScheduleTask } from './DeviceScheduleTask';
import { DeviceSensor } from './DeviceSensor';

export class DeviceConfiguration {
  aquarium: Aquarium
  mixingStation: object
  schedules: DeviceSchedule[]
  sensors: DeviceSensor[];
  tasks: DeviceScheduleTask[];
}
