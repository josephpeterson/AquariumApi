import { Component, Output, EventEmitter, Input } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    selector: 'device-generic-object-select',
    templateUrl: './device-generic-object-select.component.html',
    styleUrls: [],
})
export class DeviceGenericObjectSelectComponent {
    @Input() public availableObjects: any[] = [];

    public selectControl: FormControl = new FormControl();

    @Input() set disabled(condition: boolean) {
        if (condition)
            this.selectControl.disable();
        else
            this.selectControl.enable();
    }
    @Input() inputModel: number;
    @Output() inputModelChange = new EventEmitter<number>();
    @Output() onChange = new EventEmitter();
    @Input() label:string = "Select Object"
    private componentLifeCycle$ = new Subject();

    constructor() {
    }
    ngOnInit() {
        this.selectControl.setValue(this.inputModel);
        this.selectControl.valueChanges.pipe(takeUntil(this.componentLifeCycle$)).subscribe(val => {
            if(!val && !this.inputModel)
                return;
            this.inputModelChange.emit(val);
            this.onChange.emit(val);
        });
    }
    ngOnDestory() {
        this.componentLifeCycle$.next();
        this.componentLifeCycle$.unsubscribe();
    }
}