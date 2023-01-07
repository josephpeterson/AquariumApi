import { Aquarium } from '../../../models/Aquarium';
import { WirelessDevice } from './WirelessDevice';
import { DeviceSchedule } from './DeviceSchedule';
import { DeviceScheduledJob } from './DeviceScheduledJob';
import { DeviceScheduleTask } from './DeviceScheduleTask';
import { DeviceSensor } from './DeviceSensor';

export class DeviceConfiguration {
  aquarium: Aquarium
  //boardType: string
  wirelessDevices: WirelessDevice[]
  schedules: DeviceSchedule[]
  sensors: DeviceSensor[];
  tasks: DeviceScheduleTask[];
  scheduledJobs: DeviceScheduledJob[];
  settings: DeviceConfigurationSettings
  privateSettings: DeviceConfigurationPrivateSettings | null
}
export class DeviceConfigurationSettings {
  boardType: string
  deviceName: string
  port: number
  hostname: string
}
export class DeviceConfigurationPrivateSettings {
  allowRemoteConnections: boolean
}