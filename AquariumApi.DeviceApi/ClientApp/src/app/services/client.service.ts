import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginInformationModel } from '../models/LoginInformation.model';
import { LoginInformationResponse } from '../models/LoginInformationResponse';
import { DeviceScheduleTask } from '../models/DeviceScheduleTask';
import { Aquarium } from '../models/Aquarium';
import { BaseException } from '../models/BaseException';
import { DeviceSensor } from '../models/DeviceSensor';
import { DetailedDeviceInformation } from '../models/DetailedDeviceInformation';
import { DeviceSensorTestRequest } from '../models/requests/DeviceSensorTestRequest';
import { DeviceEndpoints } from '../models/constants/DeviceEndpoints';
import { DeviceScheduledJob } from '../models/DeviceScheduledJob';
import { DeviceInformation } from '../models/DeviceInformation';
import { AquariumMixingStation } from '../models/AquariumMixingStation';

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

    public deviceInformation: DeviceInformation;

    constructor(private http: HttpClient) {
        this._url = environment.url;
    }

    public attemptLogin(loginInformation: LoginInformationModel) {
        return this.http.post(this._url + DeviceEndpoints.AUTH_LOGIN, loginInformation);
    }
    public logout() {
        function reload() {
            window.location.reload();
        }
        return this.http.delete(this._url + DeviceEndpoints.AUTH_LOGOUT).subscribe(reload, reload);
    }
    public getDeviceScheduleInformation() {
        return this.http.get(this._url + DeviceEndpoints.SCHEDULE_STATUS);
    }
    public getDeviceLog() {
        return this.http.get(this._url + DeviceEndpoints.LOG,{ responseType: "text" });
    }
    public performScheduleTask(task: DeviceScheduleTask) {
        return this.http.post(this._url + DeviceEndpoints.SCHEDULE_TASK_PERFORM,task);
    }
    public renewAuthToken() {
        return this.http.get(this._url + DeviceEndpoints.AUTH_RENEW);
    }
    public getExceptions() {
        return this.http.get<BaseException[]>(this._url + "/v1/Exception");
    }
    public getDeviceInformation() {
        return this.http.get(this._url + DeviceEndpoints.PING);
    }
    public stopScheduledJob(scheduledJob:DeviceScheduledJob) {
        return this.http.post(this._url + DeviceEndpoints.SCHEDULE_SCHEDULEDJOB_STOP,scheduledJob);
    }
    public testDeviceSensor(testRequest:DeviceSensorTestRequest) {
        return this.http.post<DeviceSensorTestRequest>(this._url + DeviceEndpoints.DEVICE_SENSOR_TEST,testRequest);
    }
    public upsertMixingStation(mixingStation:AquariumMixingStation) {
        return this.http.post(this._url + DeviceEndpoints.MIXING_STATION_UPDATE,mixingStation);
    }
    public disconnectMixingStation() {
        return this.http.delete(this._url + DeviceEndpoints.MIXING_STATION_DELETE);
    }
    public getMixingStationStatus() {
        return this.http.get(this._url + DeviceEndpoints.MIXING_STATION_STATUS);
    }
    public searchMixingStationByHostname(hostname:string) {
        return this.http.post(this._url + DeviceEndpoints.MIXING_STATION_SEARCH + "?hostname=" + hostname,null);
    }
}
