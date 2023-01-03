import { DeviceScheduleTaskTypes } from "../../../models/types/DeviceScheduleTaskTypes";
import { DeviceSensor } from "./DeviceSensor";
import { GpioPinValue } from "./GpioPinValue";

export class DeviceAction {
  id: number; //= Math.floor(Math.random() * 100);
  targetSensorId: number;
  targetSensorValue: GpioPinValue;
  targetSensor: DeviceSensor;
}
