import { Component, OnInit, Input, ViewChild, ElementRef, SimpleChanges } from '@angular/core';

@Component({
  selector: 'application-log-view',
  templateUrl: './application-log-view.component.html',
  styleUrls: ['./application-log-view.component.scss']
})
export class ApplicationLogViewComponent implements OnInit {

  @Input("log") log: string;
  @ViewChild("scrollWindow") private scrollContainer: ElementRef;


  public filters = [
    {
      name: "Information",
      match: "INFO",
      value: true
    },
    {
      name: "Debug",
      match: "DEBUG",
      value: true
    },
    {
      name: "Errors",
      match: "ERROR",
      value: true
    },
    {
      name: "Warnings",
      match: "WARN",
      value: true
    }
  ]
  clearingLog: boolean;

  constructor() { }

  ngOnInit() {
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }
  ngOnChanges(changes: SimpleChanges) {
    //setTimeout(() => this.scrollToBottom(), 100);
  }
  scrollToBottom() {
    this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
  }


  getFilteredLog() {
    if (!this.log)
      return;
    var appliedFilters = [];
    for (var i in this.filters) {
      var filter = this.filters[i];
      if (filter.value)
        appliedFilters.push(`(${filter.match})`);
    }
    if (!appliedFilters.length)
      return;
    var filterstr = appliedFilters.join("|");
    var reg = '^.*(\\|(' + filterstr + ').*$)';
    var regex = new RegExp(reg, 'gm');
    var matches = this.log.match(regex);
    if (matches)
      return matches.join("\n");
  }
}
