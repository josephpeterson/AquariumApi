import { DeviceSchedule } from "./DeviceSchedule"
import { DeviceScheduleTask } from "./DeviceScheduleTask"
import { DeviceSensor } from "./DeviceSensor"

export class DeviceScheduleTaskAssignment {
    id: number
    scheduleId: number
    taskId: number
    startTime: string
    triggerTypeId: number = 0
    triggerTaskId: number
    triggerSensorId: number
    triggerSensorValue: number
    repeat: boolean
    repeatInterval: number
    repeatEndTime: string

    schedule: DeviceSchedule | null
    task: DeviceScheduleTask | null
    triggerSensor: DeviceSensor | null
    triggerTask: DeviceScheduleTask | null
}
