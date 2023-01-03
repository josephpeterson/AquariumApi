import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { LoginInformationModel } from './models/LoginInformation.model';
import { DeviceScheduleTask } from './models/DeviceScheduleTask';
import { BaseException } from './models/BaseException';
import { DeviceSensorTestRequest } from './models/DeviceSensorTestRequest';
import { DeviceEndpoints } from './models/DeviceEndpoints';
import { DeviceScheduledJob } from './models/DeviceScheduledJob';
import { DeviceInformation } from './models/DeviceInformation';
import { AquariumMixingStation } from './models/AquariumMixingStation';
import { Observable } from 'rxjs';
import { AquariumAccount } from './models/AquariumAccount';
import { DeviceSensor } from './models/DeviceSensor';
import { DeviceConfiguration } from './models/DeviceConfiguration';

/* Inherit correct one (either deviceAPI or AquariumDashboard) */
import { AquariumDeviceHttpClient } from '../CoreModule/aquarium-device-client.service';
import { AquariumMixingStationStatus } from './models/AquariumMixingStationStatus';
import { DeviceSchedule } from 'src/app/modules/SharedDeviceModule/models/DeviceSchedule';

const httpOptions = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': 'my-auth-token'
    })
};


@Injectable({
    providedIn: "root"
})
export class AquariumDeviceService {
    private _cdn: string;
    constructor(private http: AquariumDeviceHttpClient) {
    }

    public attemptLogin(loginInformation: LoginInformationModel) {
        return this.http.post(DeviceEndpoints.AUTH_LOGIN, loginInformation);
    }
    public logout() {
        return this.http.delete(DeviceEndpoints.AUTH_LOGOUT);
    }
    public renewAuthToken() {
        return this.http.get(DeviceEndpoints.AUTH_RENEW);
    }
    public getExceptions() {
        return this.http.get<BaseException[]>("/v1/Exception");
    }
    public factoryReset() {
        return this.http.post(DeviceEndpoints.SYSTEM_FACTORY_RESET);
    }
    /* Device */
    public getDeviceInformation(): Observable<DeviceInformation> {
        return this.http.get(DeviceEndpoints.PING);
    }
    public applyDeviceConfiguration(configuredDevice: DeviceConfiguration) {
        return this.http.post(DeviceEndpoints.UPDATE, configuredDevice);
    }
    public getSelectOptionsByType(selectType: string) {
        return this.http.get(DeviceEndpoints.SELECT_FORM_TYPES.aggregate(selectType));
    }
    public getDeviceLog(): Observable<string> {
        return this.http.get(DeviceEndpoints.SYSTEM_LOG_RETRIEVE, { responseType: "text" });
    }

    /* Schedules */
    public getDeviceScheduleStatus() {
        console.log("Getting device schedule status.");
        return this.http.get(DeviceEndpoints.SCHEDULE_STATUS);
    }
    public performScheduleTask(task: DeviceScheduleTask) {
        return this.http.post(DeviceEndpoints.SCHEDULE_TASK_PERFORM, task);
    }
    public stopScheduledJob(scheduledJob: DeviceScheduledJob) {
        return this.http.post(DeviceEndpoints.SCHEDULE_SCHEDULEDJOB_STOP, scheduledJob);
    }

    /* Device Sensors */
    public deleteDeviceSensor(deviceSensor: DeviceSensor) {
        return this.http.post<DeviceSensor[]>(DeviceEndpoints.SENSOR_DELETE, deviceSensor);
    }
    public updateDeviceSensor(deviceSensor: DeviceSensor) {
        return this.http.put<DeviceSensor[]>(DeviceEndpoints.SENSOR_UPDATE, deviceSensor);
    }
    public testDeviceSensor(testRequest: DeviceSensorTestRequest): Observable<DeviceSensorTestRequest> {
        return this.http.post<DeviceSensorTestRequest>(DeviceEndpoints.SENSOR_TEST, testRequest);

    }
    public getDeviceSensorValues(): Observable<DeviceInformation> {
        console.log("Getting accurate sensor values...");
        return this.http.post(DeviceEndpoints.SENSOR_RETRIEVE);
    }

    /* Device Schedule */
    public upsertDeviceSchedule(deviceSchedule: DeviceSchedule) {
        return this.http.post<DeviceSchedule[]>(DeviceEndpoints.SCHEDULE_UPSERT, deviceSchedule);
    }
    public deleteDeviceSchedule(deviceSchedule: DeviceSchedule) {
        return this.http.post<DeviceSchedule[]>(DeviceEndpoints.SCHEDULE_DELETE, deviceSchedule);
    }
    public upsertDeviceTask(deviceTask: DeviceScheduleTask) {
        return this.http.post<DeviceScheduleTask>(DeviceEndpoints.TASK_UPSERT, deviceTask);
    }
    public deleteDeviceTask(deviceTask: DeviceScheduleTask) {
        return this.http.post(DeviceEndpoints.TASK_DELETE, deviceTask);
    }


    /* Mixing Station */
    public upsertMixingStation(mixingStation: AquariumMixingStationStatus) {
        return this.http.post(DeviceEndpoints.MIXING_STATION_UPDATE, mixingStation);
    }
    public disconnectMixingStation() {
        return this.http.delete(DeviceEndpoints.MIXING_STATION_DELETE);
    }
    public getMixingStationStatus() {
        return this.http.get(DeviceEndpoints.MIXING_STATION_STATUS);
    }
    public searchForMixingStation() {
        return this.http.post(DeviceEndpoints.MIXING_STATION_SEARCH, null);
    }
    public stopMixingStationProcedures() {
        return this.http.get(DeviceEndpoints.MIXING_STATION_STOP);
    }
    public testMixingStationValve(valveId: number) {
        var url = DeviceEndpoints.MIXING_STATION_TEST_VALVE.aggregate(valveId);
        return this.http.get(url);
    }
}
