import { Component, Inject, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBarRef, MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';
import { Title } from '@angular/platform-browser';
import { Aquarium } from 'src/app/models/Aquarium';
import { faSignOutAlt, faSlidersH, faPhotoVideo, faFish, faCogs, faChartLine, faDesktop, faWater, faUser, faBook, faCalendar } from '@fortawesome/free-solid-svg-icons';
import { AquariumAccount } from 'src/app/modules/SharedDeviceModule/models/AquariumAccount';

@Component({
  selector: 'device-notification-dialog',
  templateUrl: './notification-dialog.component.html',
  styleUrls: ['./notification-dialog.component.scss']
})
export class NotificationDialogComponent {

  public faLogout = faSignOutAlt;
  public faSliders = faSlidersH;
  public faPhotos = faPhotoVideo;
  public faFish = faFish;
  public faMaintenance = faCogs;
  public faDashboard = faChartLine;
  public faDevice = faDesktop;
  public faParameters = faWater;

  public icon_Aquariums = faWater;
  public icon_Profile = faUser;
  public icon_Journal = faBook;
  public icon_Calendar = faCalendar;
  public icon_Photos = faPhotoVideo;
  public icon_Species = faWater;
  
  public componentLifeCycle = new Subject();


  public aquariums: Aquarium[];
  public user: AquariumAccount;




  constructor(public route: ActivatedRoute,
    public dialog: MatDialog,
    private titleService: Title,
    public sbRef: MatSnackBarRef<NotificationDialogComponent>,
    @Inject(MAT_SNACK_BAR_DATA) public data: any) { }

  ngOnDestroy() {
    this.componentLifeCycle.next();
    this.componentLifeCycle.unsubscribe();
  }
}
