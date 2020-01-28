import { DeviceSchedule } from './DeviceSchedule'
import { AquariumDevice } from './AquariumDevice'

export class DeviceScheduleAssignment
{
  id: number //= Math.floor(Math.random() * 100);
  scheduleId: number
  deviceId: number
  deployed: boolean

  schedule: DeviceSchedule
  device: AquariumDevice
}
