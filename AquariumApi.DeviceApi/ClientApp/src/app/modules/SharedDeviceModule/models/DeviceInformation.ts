import { AquariumAccount } from './AquariumAccount'
import { DeviceConfiguration } from './DeviceConfiguration'
import { DeviceScheduleState } from './DeviceScheduleState'

export class DeviceInformation {
  version: string
  config: object
  account: AquariumAccount
  configuredDevice: DeviceConfiguration
  scheduleStatus: DeviceScheduleState
}

