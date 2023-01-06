import { WirelessDeviceSensor } from './WirelessDeviceSensor';


export class WirelessDeviceStatus {
  sessionId: number;
  hostname: string;
  sessionStartTime: number;
  sessionExpirationTime: number;
  valves: WirelessDeviceSensor[];
  actions: string;
}
