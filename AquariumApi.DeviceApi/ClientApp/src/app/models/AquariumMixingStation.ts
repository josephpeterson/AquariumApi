import { AquariumMixingStationValve } from './AquariumMixingStationValve';

export class AquariumMixingStation {
  sessionId: string;
  hostname: string;
  sessionStartTime: number;
  sessionExpirationTime: number;
  valves: AquariumMixingStationValve[];
  actions: string;
}
