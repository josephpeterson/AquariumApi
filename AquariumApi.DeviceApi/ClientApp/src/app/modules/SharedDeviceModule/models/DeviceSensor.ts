import { DeviceSensorPolarity } from './DeviceSensorPolarity'
import { DeviceSensorTypes } from './DeviceSensorTypes'
import { GpioPinTypes } from './GpioPinTypes'

export class DeviceSensor
{
  id: number //= Math.floor(Math.random() * 100);
  name: string
  type: number = DeviceSensorTypes.Sensor
  polarity:DeviceSensorPolarity = DeviceSensorPolarity.Write
  pin: GpioPinTypes
  alwaysOn: boolean
  value: any
}
