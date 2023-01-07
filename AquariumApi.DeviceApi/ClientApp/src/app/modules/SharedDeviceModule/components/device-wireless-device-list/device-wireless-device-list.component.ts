import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { WirelessDevice } from 'src/app/modules/SharedDeviceModule/models/WirelessDevice';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { ToastrService } from 'ngx-toastr';
import { WirelessDeviceStatus } from '../../models/WirelessDeviceStatus';
import { Store } from '@ngrx/store';
import { selectConfiguredDevice, selectMixingStationStatus } from '../../store/device.selectors';
import { connectToDevice, connectToMixingStation } from '../../store/device.actions';
import { GpioPinTypes } from '../../models/GpioPinTypes';
import { DeviceSensor } from '../../models/DeviceSensor';
@Component({
  selector: 'device-wireless-device-list',
  templateUrl: './device-wireless-device-list.component.html',
})
export class DeviceWirelessDeviceListComponent implements OnInit {
  @Input() configuredDevice: DeviceConfiguration;
  @Input() selectable: any = null;
  @Input() selectablePin: any = null;
  @Input() searchable: any = null;
  @Input() hideConnected: any = null;
  @Input() disabled: any = null;
  @Input() small: any = null;
  public searching: boolean = false;
  public wirelessDevices: WirelessDevice[] = [];
  @Input() inputModel: WirelessDevice;
  @Output() inputModelChange = new EventEmitter<WirelessDevice>();
  @Output() onChange = new EventEmitter();
  @Input() sensorInputModel: DeviceSensor = new DeviceSensor();
  @Output() sensorInputModelChange = new EventEmitter<DeviceSensor>();

  constructor(public service: AquariumDeviceService,
    private store: Store,
    private notifier: ToastrService) { }
  public ngOnInit(): void {
    if (this.configuredDevice.wirelessDevices && this.hideConnected == null)
      this.wirelessDevices = this.configuredDevice.wirelessDevices;

    if (this.sensorInputModel != null)
      this.inputModel = this.wirelessDevices.filter(x => x.id == this.sensorInputModel.locationId)[0];
  }
  public clickSearchByHostname() {
    this.disabled = true;
    this.searching = true;
    this.service.searchForMixingStation().subscribe((data: WirelessDevice[]) => {
      this.wirelessDevices = data;
      if (this.hideConnected != null)
        this.wirelessDevices = data.filter(x => isNaN(x.id) || x.id == null);
      this.disabled = false;
      this.searching = false;
    }, () => {
      this.searching = false;
      this.disabled = false;
    });
  }
  public clickSelectAction(station: WirelessDevice) {
    this.inputModelChange.emit(station);
    this.sensorInputModelChange.emit({
      ...this.sensorInputModel,
      locationId: station.id,
      pin: this.sensorInputModel.pin
    });
  }
}
