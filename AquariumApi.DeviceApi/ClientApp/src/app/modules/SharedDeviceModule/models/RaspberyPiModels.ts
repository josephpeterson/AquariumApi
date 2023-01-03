import { RaspberryPi3ModelB } from "./RaspberryPi3ModelB";

export const RaspberryPiModels = [new RaspberryPi3ModelB()]
export enum DeviceConnectionStatus {
    Connecting = 1,
    Connected = 2,
    Failed = 3,
}