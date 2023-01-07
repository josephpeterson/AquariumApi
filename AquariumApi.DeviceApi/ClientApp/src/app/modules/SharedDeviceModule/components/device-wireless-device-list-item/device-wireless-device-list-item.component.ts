import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { WirelessDevice } from 'src/app/modules/SharedDeviceModule/models/WirelessDevice';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { ToastrService } from 'ngx-toastr';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { DeviceInformation } from '../../models/DeviceInformation';
import { WirelessDeviceStatus } from '../../models/WirelessDeviceStatus';
import { Store } from '@ngrx/store';
import { connectToDevice, connectToMixingStation } from '../../store/device.actions';
import { faEdit } from '@fortawesome/free-solid-svg-icons';
@Component({
  selector: 'device-wireless-device-list-item',
  templateUrl: './device-wireless-device-list-item.component.html'
})
export class DeviceWirelessDeviceListItemComponent {
  @Input() wirelessDevice: WirelessDevice;
  @Input() public testable: any = null;
  @Input() public hideEdit: any = null;
  @Input() wirelessDeviceStatus: WirelessDevice;
  @Output() onClick: EventEmitter<WirelessDevice> = new EventEmitter();
  @Input() public sensorInputModel;
  @Output() sensorInputModelChange = new EventEmitter();


  public faEdit = faEdit;
  public disabled = false;
  public searching = false;

  constructor(public service: AquariumDeviceService,
    private store: Store,
    private notifier: ToastrService) { }
  public clickAction() {
    this.onClick.emit(this.wirelessDevice);
  }
}
