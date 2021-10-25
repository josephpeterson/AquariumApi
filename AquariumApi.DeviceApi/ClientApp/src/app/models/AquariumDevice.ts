import { Aquarium } from './Aquarium';
import { CameraConfiguration } from './CameraConfiguration';
import { DeviceScheduleAssignment } from './DeviceScheduleAssignment';
import { DeviceSensor } from './DeviceSensor';

export class AquariumDevice
{
  id?: number //= Math.floor(Math.random() * 100);
  aquariumId: number
  type: string
  port: string
  address: string
  privateKey: string
  name: string
  hardwareModel: string
  enabledTemperature: boolean
  enabledPhoto: boolean
  enabledPh: boolean
  enabledNitrate: boolean
  enabledLighting: boolean
  cameraConfiguration: CameraConfiguration
  
  aquarium: Aquarium
  sensors: DeviceSensor[]
  scheduleAssignments: DeviceScheduleAssignment[]
}
