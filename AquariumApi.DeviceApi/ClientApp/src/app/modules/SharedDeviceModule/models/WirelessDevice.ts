import { WirelessDeviceStatus } from "./WirelessDeviceStatus";
import { WirelessDeviceSensor } from "./WirelessDeviceSensor";

export class WirelessDevice {
  id: number | null;
  name: string;
  hostname: string;
  sensors: WirelessDeviceSensor[]
  lastConnected: string;
  status: WirelessDeviceStatus
}


