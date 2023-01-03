import { AquariumMixingStationValve } from './AquariumMixingStationValve';


export class AquariumMixingStationStatus {
  sessionId: number;
  hostname: string;
  sessionStartTime: number;
  sessionExpirationTime: number;
  valves: AquariumMixingStationValve[];
  actions: string;
}
