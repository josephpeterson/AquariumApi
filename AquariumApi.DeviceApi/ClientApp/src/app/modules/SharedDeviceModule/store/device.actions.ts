import { createAction, props } from "@ngrx/store";
import { AquariumMixingStationStatus } from "../models/AquariumMixingStationStatus";
import { DeviceInformation } from "../models/DeviceInformation";
import { DeviceSensor } from "../models/DeviceSensor";
export const connectToDevice = createAction(
    '[Device] Device connecting',
    //props<{ content: string }>()
);
export const deviceConnectionSuccess = createAction(
    '[Device] Device connection success',
    props<DeviceInformation>()
);
export const deviceConnectionFailure = createAction(
    '[Device] Device connection failure',
    props<{ content: string }>()
);
export const deviceGetSensorValues = createAction(
    '[Device] Device Get Device Sensor Values'
);
export const deviceGetSensorValuesSuccess = createAction(
    '[Device] Device Get Device Sensor Values success',
    props<any>()
);
export const deviceGetScheduleStatus = createAction(
    '[Device] Device Get Schedule Status'
);
export const deviceGetScheduleStatusSuccess = createAction(
    '[Device] Device Get Schedule Status success',
    props<any>()
);
export const connectToMixingStation = createAction(
    '[Device] Device connecting to mixing station',
    //props<{ content: string }>()
);
export const disconnectMixingStation = createAction(
    '[Device] Device disconnecting mixing station',
    //props<{ content: string }>()
);
export const deviceMixingStationConnectionSuccess = createAction(
    '[Device] Device mixing station success',
    props<AquariumMixingStationStatus>()
);
export const deviceMixingStationConnectionFailure = createAction(
    '[Device] Device mixing station failure',
    props<{ content: string }>()
);
export const loadDeviceTypesByType = createAction(
    '[Device] Loading device types',
    props<{ payload: any }>()
);
export const deviceTypeListingSuccess = createAction(
    '[Device] Device type listing success',
    props<any>()
);