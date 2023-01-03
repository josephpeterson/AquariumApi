import { Component, Output, EventEmitter, Input } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { Store } from '@ngrx/store';
import { selectDeviceTypes } from '../../store/device.selectors';
import { loadDeviceTypesByType } from '../../store/device.actions';

@Component({
    selector: 'device-generic-select',
    templateUrl: './device-generic-select.component.html',
    styleUrls: [],
})
export class DeviceGenericSelectComponent {
    public listOptions: any[] = [];
    public loading: boolean = false;

    public componentLifeCycle$ = new Subject();

    public selectControl: FormControl = new FormControl();

    public types$ = this.store.select(selectDeviceTypes);


    @Input() set disabled(condition: boolean) {
        if (condition)
            this.selectControl.disable();
        else
            this.selectControl.enable();
    }

    @Input() inputModel: any;
    @Output() inputModelChange = new EventEmitter<any>();

    @Output() onChange = new EventEmitter();
    @Input() label: string;


    @Input() deviceSelectType: string;

    constructor(private store: Store) { }

    ngOnInit() {
        this.store.dispatch(loadDeviceTypesByType({payload: this.deviceSelectType}));
        this.selectControl.setValue(this.inputModel);
        this.selectControl.valueChanges.pipe(takeUntil(this.componentLifeCycle$)).subscribe(val => {
            this.inputModelChange.emit(val);
            this.onChange.emit(val);
        });

        this.types$.subscribe(types => {
            this.listOptions = types[this.deviceSelectType];
        });
    }
    ngOnDestory() {
        this.componentLifeCycle$.next();
        this.componentLifeCycle$.unsubscribe();
    }
}