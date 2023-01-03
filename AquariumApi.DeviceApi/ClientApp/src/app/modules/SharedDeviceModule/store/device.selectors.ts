import { SharedDeviceModuleState } from "../shared-device-module.state";
export const selectDeviceConnection = (state: SharedDeviceModuleState) => state.device
export const selectMixingStationStatus = (state: SharedDeviceModuleState) => state.device.mixingStationConnectionStatus
export const selectMixingStationConnection = (state: SharedDeviceModuleState) => state.device.mixingStationConnection
export const selectDeviceInformation = (state: SharedDeviceModuleState) => state.device.deviceConnection
export const selectDeviceAccount = (state: SharedDeviceModuleState) => state.device.deviceConnection?.account
export const selectConfiguredDevice = (state: SharedDeviceModuleState) => state.device.deviceConnection?.configuredDevice
export const selectDeviceSensorTypes = (state: SharedDeviceModuleState) => state.device.sensorTypes.DeviceSensorTypes
export const selectDeviceTypes = (state: SharedDeviceModuleState) => state.device.sensorTypes