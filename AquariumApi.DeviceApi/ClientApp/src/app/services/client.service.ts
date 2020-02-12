import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginInformationModel } from '../models/LoginInformation.model';
import { LoginInformationResponse } from '../models/LoginInformationResponse';
import { DeviceScheduleTask } from '../models/DeviceScheduleTask';
import { Aquarium } from '../models/Aquarium';

const httpOptions = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': 'my-auth-token'
    })
};


@Injectable({
    providedIn: "root"
})
export class ClientService {
    private _url: string;
    private _cdn: string;

    public loginInformation: LoginInformationResponse;

    constructor(private http: HttpClient) {
        this._url = environment.url;
    }

    public attemptLogin(loginInformation: LoginInformationModel) {
        return this.http.post(this._url + "/ClientApp/Login", loginInformation);
    }
    public logout() {
        function reload() {
            window.location.reload();
        }
        return this.http.delete(this._url + "/ClientApp/Logout").subscribe(reload, reload);
    }
    public getDetailedInformation() {
        return this.http.get(this._url + "/ClientApp");
    }
    public getDeviceScheduleInformation() {
        return this.http.get(this._url + "/ClientApp/Schedule");
    }
    public getDeviceLog() {
        return this.http.get(this._url + "/ClientApp/Log",{ responseType: "text" });
    }
    public performScheduleTask(task: DeviceScheduleTask) {
        return this.http.post(this._url + "/ClientApp/PerformTask",task);
    }
    public renewAuthToken() {
        return this.http.get(this._url + "/ClientApp/Auth/Renew");
    }
    public scanHardware() {
        return this.http.get<Aquarium>(this._url + "/ClientApp/Hardware/Scan");
    }
}
