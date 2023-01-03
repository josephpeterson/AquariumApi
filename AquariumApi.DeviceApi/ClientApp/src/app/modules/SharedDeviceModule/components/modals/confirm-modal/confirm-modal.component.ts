import { Component, OnInit, Input, ViewChild, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'confirm-modal',
  templateUrl: './confirm-modal.component.html',
  styleUrls: ['./confirm-modal.component.scss']
})
export class ConfirmModalComponent implements OnInit {

  constructor(
    @Inject(MAT_DIALOG_DATA) private data,
    private _dialogRef: MatDialogRef<ConfirmModalComponent>,
  ) {
    if(data?.title) this.title = data.title;
    if(data?.body) this.body = data.body;
  }

  public title: string = "This is the default title";
  public body: string = "This is the default body";

  ngOnInit() {
  }
  clickCancel() {
    this._dialogRef.close(false);
  }
  clickConfirm() {
    this._dialogRef.close(true);
  }
}