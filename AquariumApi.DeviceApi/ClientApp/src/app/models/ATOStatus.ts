import { GpioPinValue } from "./types/GpioPinValue";

export class ATOStatus {
  id: number;
  aquariumId: number;
  deviceId: number;
  sensorValue: string;
  pumpRunning: boolean;
  mlPerSec: number;
  maxRuntime: number;
  floatSensorValue: GpioPinValue;
  runtimeRemaining: number;
  runIndefinitely: boolean;
  startTime: string;
  nextRunTime: string | null;
  actualEndTime: string;
  estimatedEndTime: string;
  enabled: boolean;
}