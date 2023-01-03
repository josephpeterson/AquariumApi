import { Component, Output, EventEmitter, Input } from '@angular/core';
import { FormControl } from '@angular/forms';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    selector: 'device-date-time-select',
    templateUrl: './device-date-time-select.component.html',
    styleUrls: [],
})
export class DeviceDateTimeSelectComponent {
    public selectControl: FormControl = new FormControl();

    @Input() set disabled(condition: boolean) {
        if (condition)
            this.selectControl.disable();
        else
            this.selectControl.enable();
    }
    @Input() inputModel: string;
    @Output() inputModelChange = new EventEmitter(); //This needs to match inputModel
    @Output() onChange = new EventEmitter();
    @Input() label: string = "Select Time"
    private componentLifeCycle$ = new Subject();

    constructor() {
    }

    ngOnInit() {
        if (this.inputModel)
            this.selectControl.setValue(moment(this.inputModel).format("HH:mm:ss"));
        this.selectControl.valueChanges.pipe(takeUntil(this.componentLifeCycle$)).subscribe(val => {
            if (!val && !this.inputModel)
                return;
            var date = moment('2000-01-01 ' + val).utc().format();
            this.inputModelChange.emit(date);
            this.onChange.emit(date);
        });
    }
    ngOnDestory() {
        this.componentLifeCycle$.next();
        this.componentLifeCycle$.unsubscribe();
    }
}