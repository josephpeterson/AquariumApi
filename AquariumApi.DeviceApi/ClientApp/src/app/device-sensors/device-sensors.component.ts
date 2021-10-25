import { Component, OnInit, Input } from '@angular/core';
import { LoginInformationResponse } from '../models/LoginInformationResponse';
import { DeviceScheduleState } from '../models/DeviceScheduleState';
import { ClientService } from '../services/client.service';
import * as moment from 'moment';
import { DeviceScheduleTask, DeviceScheduleTaskTypes } from '../models/DeviceScheduleTask';
import { NotifierService } from 'angular-notifier';
import { AquariumDevice } from '../models/AquariumDevice';
import { DeviceSensor } from '../models/DeviceSensor';
import { DetailedDeviceInformation } from '../models/DetailedDeviceInformation';
import { DeviceSensorTestRequest } from '../models/requests/DeviceSensorTestRequest';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'device-sensors',
  templateUrl: './device-sensors.component.html',
  styleUrls: ['./device-sensors.component.scss']
})
export class DeviceSensorsComponent implements OnInit {

  @Input() loginInformation: LoginInformationResponse;
  scheduleState: DeviceScheduleState;
  public scanning: boolean;
  public device: AquariumDevice;


  constructor(public service: ClientService,
    private notifier: NotifierService) { }

  ngOnInit() {

    this.clickGetDeviceScheduleStatus();
  }
  clickGetDeviceScheduleStatus() {
    this.scanning = true;
    this.service.getDetailedInformation().subscribe(
      (deviceInfo: DetailedDeviceInformation) => {
        var device = deviceInfo.aquarium.device;
        this.device = device;
      }, err => {
        this.scanning = false;
        //this.notifier.notify("error", "Could not retrieve device information");
      })
  }
  clickTestDeviceSensor(sensor: DeviceSensor) {
    var request = new DeviceSensorTestRequest();
    request.sensorId = sensor.id;
    request.runtime = 3;

    this.service.testDeviceSensor(request).subscribe(
      (request: DeviceSensorTestRequest) => {
        this.notifier.notify("success", `Successfully ran test for sensor: ${sensor.name}`);
      }, (err: HttpErrorResponse) => {
        this.scanning = false;
        this.notifier.notify("error", `Could not run test for device sensor: ${sensor.name}: ${err.message}`);
      })
  }
}
