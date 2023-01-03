import { Component, OnInit, Input, ViewChild, ElementRef, SimpleChanges } from '@angular/core';

@Component({
  selector: 'device-application-log-view',
  templateUrl: './application-log-view.component.html',
  styleUrls: ['./application-log-view.component.scss']
})
export class ApplicationLogViewComponent {

  @Input() log: string;
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


  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  scrollToBottom() {
    this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
  }


  getFilteredLog() {
    if (!this.log)
      return "";
    const appliedFilters = [];
    for (const i in this.filters) {
      const filter = this.filters[i];
      if (filter.value)
        appliedFilters.push(`(${filter.match})`);
    }
    if (!appliedFilters.length)
      return "";
    const filterstr = appliedFilters.join("|");
    const reg = '^.*(\\|(' + filterstr + ').*$)';
    const regex = new RegExp(reg, 'gm');
    const matches = this.log.match(regex);
    if (matches)
      return matches.join("\n");
    return "";
  }
}
