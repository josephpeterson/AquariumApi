import { Aquarium } from './Aquarium'
import { AquariumAccount } from './AquariumAccount'
import { ATOStatus } from './ATOStatus'
import { DeviceConfiguration } from './DeviceConfiguration'
import { DeviceSchedule } from './DeviceSchedule'
import { DeviceScheduledJob } from './DeviceScheduledJob'
import { GpioPinTypes } from './GpioPinTypes'

export class DeviceInformation {
  version: string
  config: object
  account: AquariumAccount
  configuredDevice: DeviceConfiguration
  runningJobs: DeviceScheduledJob[]
  scheduledJobs: DeviceScheduledJob[]
}

