import { Injectable } from "@angular/core";
import { ActivationStart } from "@angular/router";
import { Store } from "@ngrx/store";
import { AquariumDeviceService } from "../aquarium-device.service";
import { SharedDeviceModuleState } from "../shared-device-module.state";
import { connectToDevice, connectToMixingStation, deviceConnectionFailure, deviceConnectionSuccess, deviceGetScheduleStatus, deviceGetScheduleStatusSuccess, deviceGetSensorValues, deviceGetSensorValuesSuccess, deviceMixingStationConnectionFailure, deviceMixingStationConnectionSuccess, deviceTypeListingSuccess, loadDeviceTypesByType } from "./device.actions";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { of } from "rxjs";
import { switchMap, map, catchError, withLatestFrom, mergeMap } from "rxjs/operators";
import { DeviceConfiguration } from "../models/DeviceConfiguration";
import { DeviceInformation } from "../models/DeviceInformation";
import { AquariumMixingStationStatus } from "../models/AquariumMixingStationStatus";
import { selectDeviceTypes } from "./device.selectors";
import { DeviceSensor } from "../models/DeviceSensor";

@Injectable()
export class DeviceEffects {
    constructor(
        private actions$: Actions,
        private store: Store<SharedDeviceModuleState>,
        private aquariumDeviceService: AquariumDeviceService
    ) { }

    connectToDevice = createEffect(() =>
        this.actions$.pipe(
            ofType(connectToDevice),
            switchMap(() => this.aquariumDeviceService.getDeviceInformation().pipe(
                map((info: DeviceInformation) => deviceConnectionSuccess(info)),
                catchError((d) => of(deviceConnectionFailure(d))),
            ))
        ))
    deviceGetSensorValues = createEffect(() =>
        this.actions$.pipe(
            ofType(deviceConnectionSuccess,deviceGetSensorValues),
            switchMap(() => this.aquariumDeviceService.getDeviceSensorValues().pipe(
                map((content) => deviceGetSensorValuesSuccess({ content: content }))
            ))
        ))
    deviceGetScheduleStatus = createEffect(() =>
        this.actions$.pipe(
            ofType(deviceGetScheduleStatus),
            switchMap(() => this.aquariumDeviceService.getDeviceScheduleStatus().pipe(
                map((content) => deviceGetScheduleStatusSuccess({ content: content }))
            ))
        ))
    connectToMixingStation = createEffect(() =>
        this.actions$.pipe(
            ofType(connectToMixingStation),
            switchMap(() => this.aquariumDeviceService.getMixingStationStatus().pipe(
                map((info: AquariumMixingStationStatus) => deviceMixingStationConnectionSuccess(info)),
                catchError((d) => of(deviceMixingStationConnectionFailure(d)))
            ))
        ))
    loadDeviceTypes = createEffect(() =>
        this.actions$.pipe(
            ofType(loadDeviceTypesByType),
            withLatestFrom(this.store.select(selectDeviceTypes)),
            switchMap(([action, types]) => {
                //Check if we can skip this call
                if (types[action.payload]?.length > 0)
                    return of(deviceTypeListingSuccess({ selectType: action.payload, content: types[action.payload] }));

                return this.aquariumDeviceService.getSelectOptionsByType(action.payload).pipe(
                    map((types) => deviceTypeListingSuccess({ selectType: action.payload, content: types })),
                )
            })
        ))
}