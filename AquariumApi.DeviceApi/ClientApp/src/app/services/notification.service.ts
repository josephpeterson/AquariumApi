import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationDialogComponent } from '../shared/notification-dialog/notification-dialog.component';
@Injectable({
  providedIn: "root"
})
export class NotificationService {
  public aquariumId: number;


  constructor(private _snackBar: MatSnackBar) {
    
  }

  public notify(level:string,body:string) {
    this._snackBar.openFromComponent(NotificationDialogComponent, {
      data: {
        body: body,
        level: level
      },
      panelClass: ["notification",level],
      horizontalPosition: "right",
      verticalPosition: "top",
      duration: 5000,
    });
  }
}