import { Aquarium } from './Aquarium'
import { ATOStatus } from './ATOStatus'
import { DeviceSchedule } from './DeviceSchedule'
import { DeviceScheduledJob } from './DeviceScheduledJob'
import { DeviceSensor } from './DeviceSensor'
import { GpioPinTypes } from './GpioPinTypes'

export class DeviceInformation
{
  aquarium: Aquarium
  config: object
  schedules: DeviceSchedule[]
  sensors: DeviceSensor[]
  runningJobs: DeviceScheduledJob[]
  scheduledJobs: DeviceScheduledJob[]
  atoStatus: ATOStatus
}
