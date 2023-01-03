import { createReducer, on } from "@ngrx/store";
import { AquariumMixingStationStatus } from "../models/AquariumMixingStationStatus";
import { DeviceInformation } from "../models/DeviceInformation";
import { DeviceSensorTypes } from "../models/DeviceSensorTypes";
import { DeviceConnectionStatus } from "../models/RaspberyPiModels";
import { connectToDevice, connectToMixingStation, deviceConnectionFailure, deviceConnectionSuccess, deviceGetScheduleStatusSuccess, deviceGetSensorValuesSuccess, deviceMixingStationConnectionFailure, deviceMixingStationConnectionSuccess, deviceTypeListingSuccess, disconnectMixingStation } from "./device.actions";

export interface DeviceConnectionState {
    error: string;
    deviceConnection: DeviceInformation,
    deviceConnectionStatus: DeviceConnectionStatus,
    mixingStationConnection: AquariumMixingStationStatus,
    mixingStationConnectionStatus: DeviceConnectionStatus,

    sensorTypes: {
        DeviceSensorTypes: Array<any>
    }
}
export const initialState: DeviceConnectionState = {
    error: null,
    deviceConnection: null,
    deviceConnectionStatus: DeviceConnectionStatus.Failed,
    mixingStationConnection: null,
    mixingStationConnectionStatus: DeviceConnectionStatus.Failed,
    sensorTypes: {
        DeviceSensorTypes: []
    }
}
export const deviceReducer = createReducer(
    initialState,
    on(deviceConnectionSuccess, (state, content) => ({
        ...state,
        deviceConnection: content,
        deviceConnectionStatus: DeviceConnectionStatus.Connected
    })),
    on(deviceGetSensorValuesSuccess, (state, { content }) => {
        console.log(content,state.mixingStationConnection.valves);
        var mixingStationSensors = state.mixingStationConnection.valves.map(v => ({ ...v, value: content.filter(s => s.pin === v.pin && s.type == DeviceSensorTypes.MixingStation)[0]?.value }))
        return {
            ...state,
            mixingStationConnection: {
                ...state.mixingStationConnection,
                valves: mixingStationSensors
            },
            deviceConnection: {
                ...state.deviceConnection,
                configuredDevice: {
                    ...state.deviceConnection?.configuredDevice,
                    sensors: content
                },
            }
        }
    }),
    on(deviceGetScheduleStatusSuccess, (state, { content }) => {
        return {
            ...state,
            deviceConnection: {
                ...state.deviceConnection,
                scheduleStatus: content
            }
        }
    }),
    on(deviceConnectionFailure, (state, { content }) => ({
        ...state,
        error: content,
        deviceConnectionStatus: DeviceConnectionStatus.Failed
    })),
    on(connectToDevice, (state) => ({
        ...state,
        error: null,
        deviceConnectionStatus: DeviceConnectionStatus.Connecting
    })),
    on(connectToMixingStation, (state) => ({
        ...state,
        error: null,
        mixingStationConnectionStatus: DeviceConnectionStatus.Connecting
    })),
    on(disconnectMixingStation, (state) => ({
        ...state,
        error: null,
        mixingStationConnection: null,
        mixingStationConnectionStatus: DeviceConnectionStatus.Failed
    })),
    on(deviceMixingStationConnectionFailure, (state, { content }) => ({
        ...state,
        error: content,
        mixingStationConnectionStatus: DeviceConnectionStatus.Failed
    })),
    on(deviceMixingStationConnectionSuccess, (state, status) => ({
        ...state,
        error: null,
        mixingStationConnection: status,
        mixingStationConnectionStatus: DeviceConnectionStatus.Connected
    })),


    on(deviceTypeListingSuccess, (state, types) => {
        var s = {
            ...state,
            sensorTypes: { ...state.sensorTypes }
        }
        s.sensorTypes[types.selectType] = types.content;
        return s;
    }),
)