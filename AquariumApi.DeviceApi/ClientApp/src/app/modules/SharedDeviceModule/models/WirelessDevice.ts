import { WirelessDeviceStatus } from "./WirelessDeviceStatus";
import { WirelessDeviceSensor } from "./WirelessDeviceSensor";

export class WirelessDevice {
  id: number | null;
  hostname: string;
  sensors: WirelessDeviceSensor[]
  lastConnected: string;
  status: WirelessDeviceStatus
}


