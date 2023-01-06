import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { WirelessDevice } from 'src/app/modules/SharedDeviceModule/models/WirelessDevice';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { ToastrService } from 'ngx-toastr';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { DeviceInformation } from '../../models/DeviceInformation';
import { WirelessDeviceStatus } from '../../models/WirelessDeviceStatus';
import { Store } from '@ngrx/store';
import { connectToDevice, connectToMixingStation } from '../../store/device.actions';
import { WirelessDeviceSensor } from '../../models/WirelessDeviceSensor';
@Component({
  selector: 'device-wireless-device-sensor-list-item',
  templateUrl: './device-wireless-device-sensor-list-item.component.html',
  styleUrls: ['./device-wireless-device-sensor-list-item.component.scss']
})
export class DeviceWirelessDeviceSensorListItemComponent implements OnInit {
  @Input() valve: WirelessDeviceSensor;
  @Input() selectable: any = null;
  @Input() used: any = false;
  @Input() disabled: any = null;
  @Input() selected: any = null;
  @Input() tooltip: string = null;
  @Output() onClick = new EventEmitter();

  constructor(public service: AquariumDeviceService,
    private store: Store,
    private notifier: ToastrService) { }
  ngOnInit(): void {
    if (this.tooltip == null)
      this.tooltip = `Sensor ` + this.valve.id;
  }
  public clickValve() {
    this.onClick.emit(this.valve);
  }
}
