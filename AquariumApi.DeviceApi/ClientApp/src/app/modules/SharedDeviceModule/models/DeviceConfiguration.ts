import { Aquarium } from '../../../models/Aquarium';
import { AquariumMixingStation } from './AquariumMixingStation';
import { DeviceSchedule } from './DeviceSchedule';
import { DeviceScheduledJob } from './DeviceScheduledJob';
import { DeviceScheduleTask } from './DeviceScheduleTask';
import { DeviceSensor } from './DeviceSensor';

export class DeviceConfiguration {
  aquarium: Aquarium
  boardType: string
  mixingStation: AquariumMixingStation
  schedules: DeviceSchedule[]
  sensors: DeviceSensor[];
  tasks: DeviceScheduleTask[];
  scheduledJobs: DeviceScheduledJob[];
}
