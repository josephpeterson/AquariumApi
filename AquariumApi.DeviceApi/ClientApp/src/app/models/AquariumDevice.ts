import { Aquarium } from './Aquarium';

export class AquariumDevice
{
  id?: number //= Math.floor(Math.random() * 100);
  aquariumId: number
  type: string
  port: string
  address: string
  privateKey: string
  name: string
  enabledTemperature: boolean
  enabledPhoto: boolean
  enabledPh: boolean
  enabledNitrate: boolean
  enabledLighting: boolean
  cameraConfiguration: any
  
  aquarium: Aquarium
}
