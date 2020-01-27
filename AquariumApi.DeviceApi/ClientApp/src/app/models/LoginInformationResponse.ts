import { AquariumAccount } from './AquariumAccount';
import { Aquarium } from './Aquarium';
export class LoginInformationResponse {
    public account: AquariumAccount;
    public aquarium: Aquarium;
    public aquariumId: number | null;
    public token: string | null;
}
