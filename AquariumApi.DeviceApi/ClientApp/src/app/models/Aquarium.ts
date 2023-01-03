import { AquariumDevice } from "./AquariumDevice"

export class Aquarium
{
  id: number //= Math.floor(Math.random() * 100);
  ownerId: number //= Math.floor(Math.random() * 100);
  name: string
  gallons: number
  type: string
  startDate: string
  fish: any[]
  waterSalinity: number

  equipment: any[]
  substrate: any
  plan: any
  
  feedings: any[] | null
  device?: AquariumDevice | null
  snapshots: any[]
}
