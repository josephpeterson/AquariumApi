import { Aquarium } from '../../../models/Aquarium';

export class AquariumAccount
{
  id: number //= Math.floor(Math.random() * 100);
  username: string
  role: string
  email: string;
  profile: any
  aquariums: Aquarium[]
  seniorityDate: string
}
