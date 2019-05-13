(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["main"],{

/***/ "./src/$$_lazy_route_resource lazy recursive":
/*!**********************************************************!*\
  !*** ./src/$$_lazy_route_resource lazy namespace object ***!
  \**********************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

function webpackEmptyAsyncContext(req) {
	// Here Promise.resolve().then() is used instead of new Promise() to prevent
	// uncaught exception popping up in devtools
	return Promise.resolve().then(function() {
		var e = new Error("Cannot find module '" + req + "'");
		e.code = 'MODULE_NOT_FOUND';
		throw e;
	});
}
webpackEmptyAsyncContext.keys = function() { return []; };
webpackEmptyAsyncContext.resolve = webpackEmptyAsyncContext;
module.exports = webpackEmptyAsyncContext;
webpackEmptyAsyncContext.id = "./src/$$_lazy_route_resource lazy recursive";

/***/ }),

/***/ "./src/app/app.module.ts":
/*!*******************************!*\
  !*** ./src/app/app.module.ts ***!
  \*******************************/
/*! exports provided: AppModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppModule", function() { return AppModule; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/platform-browser */ "./node_modules/@angular/platform-browser/fesm5/platform-browser.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_forms__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/forms */ "./node_modules/@angular/forms/fesm5/forms.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var _angular_router__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @angular/router */ "./node_modules/@angular/router/fesm5/router.js");
/* harmony import */ var _angular_material_table__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/material/table */ "./node_modules/@angular/material/esm5/table.es5.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var _angular_material_tabs__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/material/tabs */ "./node_modules/@angular/material/esm5/tabs.es5.js");
/* harmony import */ var _angular_material_sort__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! @angular/material/sort */ "./node_modules/@angular/material/esm5/sort.es5.js");
/* harmony import */ var _angular_material_menu__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @angular/material/menu */ "./node_modules/@angular/material/esm5/menu.es5.js");
/* harmony import */ var _angular_material_icon__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! @angular/material/icon */ "./node_modules/@angular/material/esm5/icon.es5.js");
/* harmony import */ var _angular_material_toolbar__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! @angular/material/toolbar */ "./node_modules/@angular/material/esm5/toolbar.es5.js");
/* harmony import */ var _angular_material_form_field__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! @angular/material/form-field */ "./node_modules/@angular/material/esm5/form-field.es5.js");
/* harmony import */ var _angular_material_paginator__WEBPACK_IMPORTED_MODULE_14__ = __webpack_require__(/*! @angular/material/paginator */ "./node_modules/@angular/material/esm5/paginator.es5.js");
/* harmony import */ var _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_15__ = __webpack_require__(/*! @angular/platform-browser/animations */ "./node_modules/@angular/platform-browser/fesm5/animations.js");
/* harmony import */ var _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_16__ = __webpack_require__(/*! @angular/material/progress-spinner */ "./node_modules/@angular/material/esm5/progress-spinner.es5.js");
/* harmony import */ var _components_app_root_app_component__WEBPACK_IMPORTED_MODULE_17__ = __webpack_require__(/*! ./components/app-root/app.component */ "./src/app/components/app-root/app.component.ts");
/* harmony import */ var _components_nav_menu_nav_menu_component__WEBPACK_IMPORTED_MODULE_18__ = __webpack_require__(/*! ./components/nav-menu/nav-menu.component */ "./src/app/components/nav-menu/nav-menu.component.ts");
/* harmony import */ var _components_dashboard_dashboard_component__WEBPACK_IMPORTED_MODULE_19__ = __webpack_require__(/*! ./components/dashboard/dashboard.component */ "./src/app/components/dashboard/dashboard.component.ts");
/* harmony import */ var _components_aquarium_preview_aquarium_preview_component__WEBPACK_IMPORTED_MODULE_20__ = __webpack_require__(/*! ./components/aquarium-preview/aquarium-preview.component */ "./src/app/components/aquarium-preview/aquarium-preview.component.ts");
/* harmony import */ var _components_fish_fish_component__WEBPACK_IMPORTED_MODULE_21__ = __webpack_require__(/*! ./components/fish/fish.component */ "./src/app/components/fish/fish.component.ts");
/* harmony import */ var _components_maintenance_maintenance_component__WEBPACK_IMPORTED_MODULE_22__ = __webpack_require__(/*! ./components/maintenance/maintenance.component */ "./src/app/components/maintenance/maintenance.component.ts");
/* harmony import */ var _components_settings_settings_component__WEBPACK_IMPORTED_MODULE_23__ = __webpack_require__(/*! ./components/settings/settings.component */ "./src/app/components/settings/settings.component.ts");
/* harmony import */ var _components_task_list_task_list_component__WEBPACK_IMPORTED_MODULE_24__ = __webpack_require__(/*! ./components/task-list/task-list.component */ "./src/app/components/task-list/task-list.component.ts");
/* harmony import */ var _components_operations_operations_component__WEBPACK_IMPORTED_MODULE_25__ = __webpack_require__(/*! ./components/operations/operations.component */ "./src/app/components/operations/operations.component.ts");
/* harmony import */ var _components_task_table_task_table_component__WEBPACK_IMPORTED_MODULE_26__ = __webpack_require__(/*! ./components/task-table/task-table.component */ "./src/app/components/task-table/task-table.component.ts");
/* harmony import */ var _components_aquarium_preview_scroller_aquarium_preview_scroller_component__WEBPACK_IMPORTED_MODULE_27__ = __webpack_require__(/*! ./components/aquarium-preview-scroller/aquarium-preview-scroller.component */ "./src/app/components/aquarium-preview-scroller/aquarium-preview-scroller.component.ts");
/* harmony import */ var _components_lighting_lighting_component__WEBPACK_IMPORTED_MODULE_28__ = __webpack_require__(/*! ./components/lighting/lighting.component */ "./src/app/components/lighting/lighting.component.ts");
/* harmony import */ var ngx_color_picker__WEBPACK_IMPORTED_MODULE_29__ = __webpack_require__(/*! ngx-color-picker */ "./node_modules/ngx-color-picker/dist/ngx-color-picker.es5.js");






//Material Imports












//Component declarations












//Color picker

var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["NgModule"])({
            declarations: [
                _components_app_root_app_component__WEBPACK_IMPORTED_MODULE_17__["AppComponent"],
                _components_nav_menu_nav_menu_component__WEBPACK_IMPORTED_MODULE_18__["NavMenuComponent"],
                _components_dashboard_dashboard_component__WEBPACK_IMPORTED_MODULE_19__["DashboardComponent"],
                _components_fish_fish_component__WEBPACK_IMPORTED_MODULE_21__["FishComponent"],
                _components_lighting_lighting_component__WEBPACK_IMPORTED_MODULE_28__["LightingComponent"],
                _components_maintenance_maintenance_component__WEBPACK_IMPORTED_MODULE_22__["MaintenanceComponent"],
                _components_settings_settings_component__WEBPACK_IMPORTED_MODULE_23__["SettingsComponent"],
                _components_aquarium_preview_aquarium_preview_component__WEBPACK_IMPORTED_MODULE_20__["AquariumPreviewComponent"],
                _components_aquarium_preview_scroller_aquarium_preview_scroller_component__WEBPACK_IMPORTED_MODULE_27__["AquariumPreviewScrollerComponent"],
                _components_task_list_task_list_component__WEBPACK_IMPORTED_MODULE_24__["TaskListComponent"],
                _components_operations_operations_component__WEBPACK_IMPORTED_MODULE_25__["OperationsComponent"],
                _components_task_table_task_table_component__WEBPACK_IMPORTED_MODULE_26__["TaskTableComponent"],
            ],
            entryComponents: [
            //Modal components here
            ],
            imports: [
                _angular_platform_browser__WEBPACK_IMPORTED_MODULE_1__["BrowserModule"].withServerTransition({ appId: 'ng-cli-universal' }),
                _angular_common_http__WEBPACK_IMPORTED_MODULE_4__["HttpClientModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_3__["FormsModule"],
                _angular_router__WEBPACK_IMPORTED_MODULE_5__["RouterModule"].forRoot([
                    { path: '', component: _components_dashboard_dashboard_component__WEBPACK_IMPORTED_MODULE_19__["DashboardComponent"], pathMatch: 'full' },
                    { path: 'maintenance/:tab', component: _components_maintenance_maintenance_component__WEBPACK_IMPORTED_MODULE_22__["MaintenanceComponent"], },
                    { path: 'fish', component: _components_fish_fish_component__WEBPACK_IMPORTED_MODULE_21__["FishComponent"], pathMatch: 'full' },
                    { path: 'lighting', component: _components_lighting_lighting_component__WEBPACK_IMPORTED_MODULE_28__["LightingComponent"], pathMatch: 'full' },
                    { path: 'settings', component: _components_settings_settings_component__WEBPACK_IMPORTED_MODULE_23__["SettingsComponent"], pathMatch: 'full' },
                ]),
                _angular_material_progress_spinner__WEBPACK_IMPORTED_MODULE_16__["MatProgressSpinnerModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_7__["MatCardModule"],
                _angular_material_table__WEBPACK_IMPORTED_MODULE_6__["MatTableModule"],
                _angular_material_tabs__WEBPACK_IMPORTED_MODULE_8__["MatTabsModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_7__["MatCheckboxModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_7__["MatInputModule"],
                _angular_material_sort__WEBPACK_IMPORTED_MODULE_9__["MatSortModule"],
                _angular_material_menu__WEBPACK_IMPORTED_MODULE_10__["MatMenuModule"],
                _angular_material_icon__WEBPACK_IMPORTED_MODULE_11__["MatIconModule"],
                _angular_platform_browser_animations__WEBPACK_IMPORTED_MODULE_15__["BrowserAnimationsModule"],
                _angular_material_toolbar__WEBPACK_IMPORTED_MODULE_12__["MatToolbarModule"],
                _angular_material_form_field__WEBPACK_IMPORTED_MODULE_13__["MatFormFieldModule"],
                _angular_material_paginator__WEBPACK_IMPORTED_MODULE_14__["MatPaginatorModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_7__["MatSelectModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_7__["MatDialogModule"],
                _angular_material__WEBPACK_IMPORTED_MODULE_7__["MatButtonModule"],
                _angular_forms__WEBPACK_IMPORTED_MODULE_3__["ReactiveFormsModule"],
                ngx_color_picker__WEBPACK_IMPORTED_MODULE_29__["ColorPickerModule"]
            ],
            providers: [
            //Providers for authenticaion
            //{ provide: HTTP_INTERCEPTORS, useClass: ConfigVaultInterceptor, multi: true },
            //{ provide: 'OAuth.Environment', useValue: environment.environmentTag },
            //{ provide: 'OAuth.ClientName', useValue: environment.appName },
            ],
            bootstrap: [_components_app_root_app_component__WEBPACK_IMPORTED_MODULE_17__["AppComponent"]]
        })
    ], AppModule);
    return AppModule;
}());



/***/ }),

/***/ "./src/app/components/app-root/app.component.html":
/*!********************************************************!*\
  !*** ./src/app/components/app-root/app.component.html ***!
  \********************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<body>\n  <app-nav-menu></app-nav-menu>\n  <div class=\"container\">\n    <router-outlet></router-outlet>\n  </div>\n</body>\n"

/***/ }),

/***/ "./src/app/components/app-root/app.component.scss":
/*!********************************************************!*\
  !*** ./src/app/components/app-root/app.component.scss ***!
  \********************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "@media (max-width: 767px) {\n  /* On small screens, the nav menu spans the full width of the screen. Leave a space for it. */\n  .body-content {\n    padding-top: 50px; } }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy9hcHAtcm9vdC9KOlxcSm9zZXBoXFxEb2N1bWVudHNcXEdpdEh1YlxcQXF1YXJpdW1EYXNoYm9hcmQvc3JjXFxhcHBcXGNvbXBvbmVudHNcXGFwcC1yb290XFxhcHAuY29tcG9uZW50LnNjc3MiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBQUE7RUFDSSw2RkFBQTtFQUNBO0lBQ0UsaUJBQWlCLEVBQUEsRUFDbEIiLCJmaWxlIjoic3JjL2FwcC9jb21wb25lbnRzL2FwcC1yb290L2FwcC5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIkBtZWRpYSAobWF4LXdpZHRoOiA3NjdweCkge1xuICAgIC8qIE9uIHNtYWxsIHNjcmVlbnMsIHRoZSBuYXYgbWVudSBzcGFucyB0aGUgZnVsbCB3aWR0aCBvZiB0aGUgc2NyZWVuLiBMZWF2ZSBhIHNwYWNlIGZvciBpdC4gKi9cbiAgICAuYm9keS1jb250ZW50IHtcbiAgICAgIHBhZGRpbmctdG9wOiA1MHB4O1xuICAgIH1cbiAgfSJdfQ== */"

/***/ }),

/***/ "./src/app/components/app-root/app.component.ts":
/*!******************************************************!*\
  !*** ./src/app/components/app-root/app.component.ts ***!
  \******************************************************/
/*! exports provided: AppComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppComponent", function() { return AppComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");


var AppComponent = /** @class */ (function () {
    function AppComponent() {
        this.title = 'app';
    }
    AppComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-root',
            template: __webpack_require__(/*! ./app.component.html */ "./src/app/components/app-root/app.component.html"),
            styles: [__webpack_require__(/*! ./app.component.scss */ "./src/app/components/app-root/app.component.scss")]
        })
    ], AppComponent);
    return AppComponent;
}());



/***/ }),

/***/ "./src/app/components/aquarium-preview-scroller/aquarium-preview-scroller.component.html":
/*!***********************************************************************************************!*\
  !*** ./src/app/components/aquarium-preview-scroller/aquarium-preview-scroller.component.html ***!
  \***********************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"aquarium-preview-scroller-component\" tabindex=\"0\" (keydown.arrowup)=\"handleKey($event)\" (keydown.arrowdown)=\"handleKey($event)\">\n    <div class=\"currentImage\" *ngIf=\"data[selectedId]; let current\">\n        <img src={{current.src}}/>\n        <div class=\"banner\">\n            {{current.date.toLocaleDateString('en-US')}} {{current.date.toLocaleTimeString('en-US')}}\n            {{current.parameters.temperature}}F\n            PH: {{current.parameters.ph}}\n            Nitrite: {{current.parameters.nitrite}}\n            Nitrate: {{current.parameters.nitrate}}\n        </div>\n    </div>\n    <div class=\"scroll-area\" #scroller>\n        <ng-container *ngFor=\"let snapshot of data;let i = index\">\n            <div [style.background-image]=\"'url(' + snapshot.src + ')'\" [ngClass]=\"{'selected': selectedId === i }\" class=\"content\" (click)=\"setSelectedId(i)\">{{snapshot.id}}</div>\n        </ng-container>\n    </div>\n</div>\n<div style=\"clear:both;\"></div>"

/***/ }),

/***/ "./src/app/components/aquarium-preview-scroller/aquarium-preview-scroller.component.scss":
/*!***********************************************************************************************!*\
  !*** ./src/app/components/aquarium-preview-scroller/aquarium-preview-scroller.component.scss ***!
  \***********************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".aquarium-preview-scroller-component {\n  display: block;\n  outline: none; }\n  .aquarium-preview-scroller-component .currentImage {\n    position: relative;\n    width: 400px;\n    height: 400px;\n    float: left;\n    background-color: #F6F6F6;\n    border: 1px solid #A8A8A8; }\n  .aquarium-preview-scroller-component .currentImage img {\n      width: 100%;\n      height: 100%; }\n  .aquarium-preview-scroller-component .currentImage .banner {\n      position: absolute;\n      bottom: 0px;\n      width: 100%;\n      background-color: rgba(50, 50, 50, 0.4);\n      color: white;\n      padding: 5px;\n      font-size: 10pt; }\n  .aquarium-preview-scroller-component .scroll-area {\n    float: left;\n    background-color: #F6F6F6;\n    border: 1px solid #A8A8A8;\n    border-left: 0px;\n    width: 150px;\n    height: 400px;\n    overflow: auto; }\n  .aquarium-preview-scroller-component .scroll-area .content {\n      background-color: darkgray;\n      background-position: center;\n      background-size: 100% 100%;\n      margin: 3px;\n      height: 120px;\n      width: 120px;\n      border: 3px solid gray; }\n  .aquarium-preview-scroller-component .scroll-area .selected {\n      background-color: blue;\n      border-color: limegreen; }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy9hcXVhcml1bS1wcmV2aWV3LXNjcm9sbGVyL0o6XFxKb3NlcGhcXERvY3VtZW50c1xcR2l0SHViXFxBcXVhcml1bURhc2hib2FyZC9zcmNcXGFwcFxcY29tcG9uZW50c1xcYXF1YXJpdW0tcHJldmlldy1zY3JvbGxlclxcYXF1YXJpdW0tcHJldmlldy1zY3JvbGxlci5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUVFLGNBQWM7RUFDZCxhQUFZLEVBQUE7RUFIZDtJQU9JLGtCQUFrQjtJQUNsQixZQUFXO0lBQ1gsYUFBWTtJQUNaLFdBQVU7SUFFVix5QkFBd0I7SUFFeEIseUJBQXdCLEVBQUE7RUFkNUI7TUFpQk0sV0FBVTtNQUNWLFlBQVcsRUFBQTtFQWxCakI7TUFxQk0sa0JBQWlCO01BQ2pCLFdBQVU7TUFDVixXQUFVO01BQ1YsdUNBQW1DO01BQ25DLFlBQVc7TUFDWCxZQUFXO01BQ1gsZUFBYyxFQUFBO0VBM0JwQjtJQWlDSSxXQUFVO0lBQ1YseUJBQXdCO0lBQ3hCLHlCQUF3QjtJQUN4QixnQkFBZTtJQUNmLFlBQVc7SUFDWCxhQUFZO0lBRVosY0FBYyxFQUFBO0VBeENsQjtNQTRDTSwwQkFBeUI7TUFDekIsMkJBQTJCO01BQzNCLDBCQUEwQjtNQUMxQixXQUFVO01BQ1YsYUFBWTtNQUNaLFlBQVc7TUFFWCxzQkFBcUIsRUFBQTtFQW5EM0I7TUF3RE0sc0JBQXFCO01BQ3JCLHVCQUFzQixFQUFBIiwiZmlsZSI6InNyYy9hcHAvY29tcG9uZW50cy9hcXVhcml1bS1wcmV2aWV3LXNjcm9sbGVyL2FxdWFyaXVtLXByZXZpZXctc2Nyb2xsZXIuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIuYXF1YXJpdW0tcHJldmlldy1zY3JvbGxlci1jb21wb25lbnRcbntcbiAgZGlzcGxheTogYmxvY2s7XG4gIG91dGxpbmU6bm9uZTtcblxuICAuY3VycmVudEltYWdlXG4gIHtcbiAgICBwb3NpdGlvbjogcmVsYXRpdmU7XG4gICAgd2lkdGg6NDAwcHg7XG4gICAgaGVpZ2h0OjQwMHB4O1xuICAgIGZsb2F0OmxlZnQ7XG5cbiAgICBiYWNrZ3JvdW5kLWNvbG9yOiNGNkY2RjY7XG5cbiAgICBib3JkZXI6MXB4IHNvbGlkICNBOEE4QTg7XG5cbiAgICBpbWcge1xuICAgICAgd2lkdGg6MTAwJTtcbiAgICAgIGhlaWdodDoxMDAlO1xuICAgIH1cbiAgICAuYmFubmVyIHtcbiAgICAgIHBvc2l0aW9uOmFic29sdXRlO1xuICAgICAgYm90dG9tOjBweDtcbiAgICAgIHdpZHRoOjEwMCU7XG4gICAgICBiYWNrZ3JvdW5kLWNvbG9yOnJnYmEoNTAsNTAsNTAsMC40KTtcbiAgICAgIGNvbG9yOndoaXRlO1xuICAgICAgcGFkZGluZzo1cHg7XG4gICAgICBmb250LXNpemU6MTBwdDtcbiAgICB9XG4gIH1cblxuICAuc2Nyb2xsLWFyZWFcbiAge1xuICAgIGZsb2F0OmxlZnQ7XG4gICAgYmFja2dyb3VuZC1jb2xvcjojRjZGNkY2O1xuICAgIGJvcmRlcjoxcHggc29saWQgI0E4QThBODtcbiAgICBib3JkZXItbGVmdDowcHg7XG4gICAgd2lkdGg6MTUwcHg7XG4gICAgaGVpZ2h0OjQwMHB4O1xuXG4gICAgb3ZlcmZsb3c6IGF1dG87XG5cbiAgICAuY29udGVudFxuICAgIHtcbiAgICAgIGJhY2tncm91bmQtY29sb3I6ZGFya2dyYXk7XG4gICAgICBiYWNrZ3JvdW5kLXBvc2l0aW9uOiBjZW50ZXI7XG4gICAgICBiYWNrZ3JvdW5kLXNpemU6IDEwMCUgMTAwJTtcbiAgICAgIG1hcmdpbjozcHg7XG4gICAgICBoZWlnaHQ6MTIwcHg7XG4gICAgICB3aWR0aDoxMjBweDtcblxuICAgICAgYm9yZGVyOjNweCBzb2xpZCBncmF5O1xuICAgIH1cblxuICAgIC5zZWxlY3RlZFxuICAgIHtcbiAgICAgIGJhY2tncm91bmQtY29sb3I6Ymx1ZTtcbiAgICAgIGJvcmRlci1jb2xvcjpsaW1lZ3JlZW47XG4gICAgfVxuICB9XG59Il19 */"

/***/ }),

/***/ "./src/app/components/aquarium-preview-scroller/aquarium-preview-scroller.component.ts":
/*!*********************************************************************************************!*\
  !*** ./src/app/components/aquarium-preview-scroller/aquarium-preview-scroller.component.ts ***!
  \*********************************************************************************************/
/*! exports provided: AquariumPreviewScrollerComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AquariumPreviewScrollerComponent", function() { return AquariumPreviewScrollerComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! src/app/models/AquariumSnapshot */ "./src/app/models/AquariumSnapshot.ts");




var AquariumPreviewScrollerComponent = /** @class */ (function () {
    function AquariumPreviewScrollerComponent(dialog) {
        this.dialog = dialog;
        this.data = [
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
            new src_app_models_AquariumSnapshot__WEBPACK_IMPORTED_MODULE_3__["AquariumSnapshot"](),
        ];
        this.selectedId = 0;
    }
    AquariumPreviewScrollerComponent.prototype.ngOnInit = function () {
    };
    //Deprecated
    AquariumPreviewScrollerComponent.prototype.scrollScroller = function (event) {
        var scroller = this.scroller.nativeElement;
        scroller.scrollTop += event.deltaY;
        event.preventDefault();
    };
    AquariumPreviewScrollerComponent.prototype.handleKey = function (event) {
        event.preventDefault();
        event.key === 'ArrowUp' ? this.propagate(-1) : this.propagate(1);
    };
    AquariumPreviewScrollerComponent.prototype.setSelectedId = function (id) {
        //Handle overflow
        if (id < 0)
            id = this.data.length - 1;
        else if (id >= this.data.length)
            id = 0;
        this.selectedId = id;
    };
    AquariumPreviewScrollerComponent.prototype.propagate = function (num) {
        this.setSelectedId(this.selectedId + num);
        var snapshots = this.scroller.nativeElement.getElementsByClassName("content");
        snapshots[this.selectedId].scrollIntoView({ behavior: "auto", block: "end", inline: "nearest" });
    };
    tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ViewChild"])("scroller"),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:type", Object)
    ], AquariumPreviewScrollerComponent.prototype, "scroller", void 0);
    AquariumPreviewScrollerComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'aquarium-preview-scroller',
            template: __webpack_require__(/*! ./aquarium-preview-scroller.component.html */ "./src/app/components/aquarium-preview-scroller/aquarium-preview-scroller.component.html"),
            styles: [__webpack_require__(/*! ./aquarium-preview-scroller.component.scss */ "./src/app/components/aquarium-preview-scroller/aquarium-preview-scroller.component.scss")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialog"]])
    ], AquariumPreviewScrollerComponent);
    return AquariumPreviewScrollerComponent;
}());



/***/ }),

/***/ "./src/app/components/aquarium-preview/aquarium-preview.component.html":
/*!*****************************************************************************!*\
  !*** ./src/app/components/aquarium-preview/aquarium-preview.component.html ***!
  \*****************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"aquarium-preview-component row m-2\">\n    <div class=\"card\">\n\n        <div class=\"card-body\">\n            <div class=\"card-title\">\n                <h3><img src=\"assets/record.png\" height=32px /> Live Aquarium</h3>\n            </div>\n            <div class=\"container\">\n                <div class=\"row\">\n                    <div class=\"col-sm-2\">\n                        <div class=\"row\">\n                            <h6 class=\"m-1\">Parameters</h6>\n\n                        </div>\n                        <table>\n                            <tr>\n                                <td>Temperature: </td>\n                                <td>70F</td>\n                            </tr>\n                            <tr>\n                                    <td>Water PH: </td>\n                                    <td>6.5ppm</td>\n                                </tr>\n                            <tr>\n                                <td>Ammonia: </td>\n                                <td>0</td>\n                            </tr>\n                            <tr>\n                                <td>Nitrate: </td>\n                                <td>0</td>\n                            </tr>\n                            <tr>\n                                <td>Nitrate: </td>\n                                <td>0</td>\n                            </tr>\n                            <tr>\n                                <td>Fish Count: </td>\n                                <td>4</td>\n                            </tr>\n                        </table>\n                        <div class=\"row\">\n                            <h6 class=\"m-1\">Today's Tasks</h6>\n\n                        </div>\n\n                        <a class=\"link\" [routerLink]='[\"/maintenance\",\"tasks\"]'\n                            [routerLinkActive]='[\"link-active\"]'>View\n                            All\n                            Tasks</a>\n                        <div class=\"row\">\n                            <h6 class=\"m-1\">Lighting</h6>\n\n                        </div>\n                        <div class=\"mt-1\">\n                            <button class=\"btn btn-danger\">Turn Off</button>\n                        </div>\n                        <div class=\"mt-1\">\n                            <a class=\"link\" [routerLink]='[\"/maintenance\",\"tasks\"]'\n                                [routerLinkActive]='[\"link-active\"]'>Modify lights...</a>\n                        </div>\n                    </div>\n                    <div class=\"col-8\">\n                        <aquarium-preview-scroller></aquarium-preview-scroller>\n\n                    </div>\n                </div>\n\n            </div>\n        </div>\n    </div>\n</div>"

/***/ }),

/***/ "./src/app/components/aquarium-preview/aquarium-preview.component.scss":
/*!*****************************************************************************!*\
  !*** ./src/app/components/aquarium-preview/aquarium-preview.component.scss ***!
  \*****************************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".aquarium-preview-component .card {\n  width: 100%;\n  -webkit-user-select: none;\n     -moz-user-select: none;\n      -ms-user-select: none;\n          user-select: none; }\n\n.aquarium-preview-component .left {\n  text-align: right; }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy9hcXVhcml1bS1wcmV2aWV3L0o6XFxKb3NlcGhcXERvY3VtZW50c1xcR2l0SHViXFxBcXVhcml1bURhc2hib2FyZC9zcmNcXGFwcFxcY29tcG9uZW50c1xcYXF1YXJpdW0tcHJldmlld1xcYXF1YXJpdW0tcHJldmlldy5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUdJLFdBQVU7RUFDVix5QkFBZ0I7S0FBaEIsc0JBQWdCO01BQWhCLHFCQUFnQjtVQUFoQixpQkFBZ0IsRUFBQTs7QUFKcEI7RUFPSSxpQkFBZ0IsRUFBQSIsImZpbGUiOiJzcmMvYXBwL2NvbXBvbmVudHMvYXF1YXJpdW0tcHJldmlldy9hcXVhcml1bS1wcmV2aWV3LmNvbXBvbmVudC5zY3NzIiwic291cmNlc0NvbnRlbnQiOlsiLmFxdWFyaXVtLXByZXZpZXctY29tcG9uZW50XG57XG4gIC5jYXJkIHtcbiAgICB3aWR0aDoxMDAlO1xuICAgIHVzZXItc2VsZWN0Om5vbmU7XG4gIH1cbiAgLmxlZnQge1xuICAgIHRleHQtYWxpZ246cmlnaHQ7XG4gIH1cbn0iXX0= */"

/***/ }),

/***/ "./src/app/components/aquarium-preview/aquarium-preview.component.ts":
/*!***************************************************************************!*\
  !*** ./src/app/components/aquarium-preview/aquarium-preview.component.ts ***!
  \***************************************************************************/
/*! exports provided: AquariumPreviewComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AquariumPreviewComponent", function() { return AquariumPreviewComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");



var AquariumPreviewComponent = /** @class */ (function () {
    function AquariumPreviewComponent(dialog) {
        this.dialog = dialog;
    }
    AquariumPreviewComponent.prototype.ngOnInit = function () {
    };
    AquariumPreviewComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'aquarium-preview',
            template: __webpack_require__(/*! ./aquarium-preview.component.html */ "./src/app/components/aquarium-preview/aquarium-preview.component.html"),
            styles: [__webpack_require__(/*! ./aquarium-preview.component.scss */ "./src/app/components/aquarium-preview/aquarium-preview.component.scss")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialog"]])
    ], AquariumPreviewComponent);
    return AquariumPreviewComponent;
}());



/***/ }),

/***/ "./src/app/components/dashboard/dashboard.component.html":
/*!***************************************************************!*\
  !*** ./src/app/components/dashboard/dashboard.component.html ***!
  \***************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<aquarium-preview></aquarium-preview>\n<task-list></task-list>\n<operations></operations>"

/***/ }),

/***/ "./src/app/components/dashboard/dashboard.component.scss":
/*!***************************************************************!*\
  !*** ./src/app/components/dashboard/dashboard.component.scss ***!
  \***************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "table {\n  width: 100%; }\n\n.mat-column-select {\n  overflow: initial; }\n\nth.mat-sort-header-sorted {\n  color: black; }\n\n.red-number {\n  color: red; }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy9kYXNoYm9hcmQvSjpcXEpvc2VwaFxcRG9jdW1lbnRzXFxHaXRIdWJcXEFxdWFyaXVtRGFzaGJvYXJkL3NyY1xcYXBwXFxjb21wb25lbnRzXFxkYXNoYm9hcmRcXGRhc2hib2FyZC5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNJLFdBQVcsRUFBQTs7QUFFZjtFQUNFLGlCQUFpQixFQUFBOztBQUVuQjtFQUNFLFlBQVksRUFBQTs7QUFHZDtFQUNFLFVBQVUsRUFBQSIsImZpbGUiOiJzcmMvYXBwL2NvbXBvbmVudHMvZGFzaGJvYXJkL2Rhc2hib2FyZC5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbInRhYmxlIHtcbiAgICB3aWR0aDogMTAwJTtcbn1cbi5tYXQtY29sdW1uLXNlbGVjdCB7XG4gIG92ZXJmbG93OiBpbml0aWFsO1xufVxudGgubWF0LXNvcnQtaGVhZGVyLXNvcnRlZCB7XG4gIGNvbG9yOiBibGFjaztcbn1cblxuLnJlZC1udW1iZXJ7XG4gIGNvbG9yOiByZWQ7XG59Il19 */"

/***/ }),

/***/ "./src/app/components/dashboard/dashboard.component.ts":
/*!*************************************************************!*\
  !*** ./src/app/components/dashboard/dashboard.component.ts ***!
  \*************************************************************/
/*! exports provided: DashboardComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "DashboardComponent", function() { return DashboardComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");



var DashboardComponent = /** @class */ (function () {
    function DashboardComponent(dialog) {
        this.dialog = dialog;
    }
    DashboardComponent.prototype.ngOnInit = function () {
    };
    DashboardComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'dashboard-page-component',
            template: __webpack_require__(/*! ./dashboard.component.html */ "./src/app/components/dashboard/dashboard.component.html"),
            styles: [__webpack_require__(/*! ./dashboard.component.scss */ "./src/app/components/dashboard/dashboard.component.scss")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialog"]])
    ], DashboardComponent);
    return DashboardComponent;
}());



/***/ }),

/***/ "./src/app/components/fish/fish.component.html":
/*!*****************************************************!*\
  !*** ./src/app/components/fish/fish.component.html ***!
  \*****************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/components/fish/fish.component.scss":
/*!*****************************************************!*\
  !*** ./src/app/components/fish/fish.component.scss ***!
  \*****************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "table {\n  width: 100%; }\n\n.mat-column-select {\n  overflow: initial; }\n\nth.mat-sort-header-sorted {\n  color: black; }\n\n.red-number {\n  color: red; }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy9maXNoL0o6XFxKb3NlcGhcXERvY3VtZW50c1xcR2l0SHViXFxBcXVhcml1bURhc2hib2FyZC9zcmNcXGFwcFxcY29tcG9uZW50c1xcZmlzaFxcZmlzaC5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNJLFdBQVcsRUFBQTs7QUFFZjtFQUNFLGlCQUFpQixFQUFBOztBQUVuQjtFQUNFLFlBQVksRUFBQTs7QUFHZDtFQUNFLFVBQVUsRUFBQSIsImZpbGUiOiJzcmMvYXBwL2NvbXBvbmVudHMvZmlzaC9maXNoLmNvbXBvbmVudC5zY3NzIiwic291cmNlc0NvbnRlbnQiOlsidGFibGUge1xuICAgIHdpZHRoOiAxMDAlO1xufVxuLm1hdC1jb2x1bW4tc2VsZWN0IHtcbiAgb3ZlcmZsb3c6IGluaXRpYWw7XG59XG50aC5tYXQtc29ydC1oZWFkZXItc29ydGVkIHtcbiAgY29sb3I6IGJsYWNrO1xufVxuXG4ucmVkLW51bWJlcntcbiAgY29sb3I6IHJlZDtcbn0iXX0= */"

/***/ }),

/***/ "./src/app/components/fish/fish.component.ts":
/*!***************************************************!*\
  !*** ./src/app/components/fish/fish.component.ts ***!
  \***************************************************/
/*! exports provided: FishComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FishComponent", function() { return FishComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");



var FishComponent = /** @class */ (function () {
    function FishComponent(dialog) {
        this.dialog = dialog;
    }
    FishComponent.prototype.ngOnInit = function () {
    };
    FishComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'fish-page-component',
            template: __webpack_require__(/*! ./fish.component.html */ "./src/app/components/fish/fish.component.html"),
            styles: [__webpack_require__(/*! ./fish.component.scss */ "./src/app/components/fish/fish.component.scss")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialog"]])
    ], FishComponent);
    return FishComponent;
}());



/***/ }),

/***/ "./src/app/components/lighting/lighting.component.html":
/*!*************************************************************!*\
  !*** ./src/app/components/lighting/lighting.component.html ***!
  \*************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"lighting-component\">\n\t<div class=\"row\">\n\t\t<div class=\"card\">\n\n\t\t\t<div class=\"card-body\">\n\t\t\t\t<div class=\"card-title\">\n\t\t\t\t\t<h3>Light Frames</h3>\n\n\t\t\t\t</div>\n\t\t\t\t<mat-form-field>\n\t\t\t\t\t<mat-label>Light Frame</mat-label>\n\t\t\t\t\t<mat-select>\n\t\t\t\t\t\t<mat-option *ngFor=\"let food of foods\" [value]=\"food.value\">\n\t\t\t\t\t\t\t{{food.viewValue}}\n\t\t\t\t\t\t</mat-option>\n\t\t\t\t\t</mat-select>\n\t\t\t\t</mat-form-field>\n\t\t\t\t<ng-container>\n\t\t\t\t\t<div class=\"ledRow\" *ngFor=\"let row of rows\">\n\t\t\t\t\t\t<ng-container *ngFor=\"let led of row\">\n\t\t\t\t\t\t\t<div [(colorPicker)]=\"led.color\" [style.background]=\"led.color\" *ngIf=\"led.active\" class=\"led active\">{{led.id}}</div>\n\t\t\t\t\t\t\t<div *ngIf=\"led.active == false\" class=\"led\">-</div>\n\t\t\t\t\t\t</ng-container>\n\t\t\t\t\t</div>\n\t\t\t\t</ng-container>\n\t\t\t\t<button class=\"btn btn-primary\" (click)=\"sendUpdate();\">Preview</button>\n\t\t\t\t<button class=\"btn btn-primary\" (click)=\"sendUpdate();\">Save Frame...</button>\n\n\t\t\t</div>\n\n\t\t</div>\n\t</div>\n\t<div class=\"row\">\n\t\t<div class=\"card\">\n\n\t\t\t<div class=\"card-body\">\n\t\t\t\t<div class=\"card-title\">\n\t\t\t\t\t<h3>Schedule</h3>\n\t\t\t\t\t<mat-form-field>\n\t\t\t\t\t\t<mat-label>Schedule Name</mat-label>\n\t\t\t\t\t\t<mat-select>\n\t\t\t\t\t\t\t<mat-option *ngFor=\"let food of foods\" [value]=\"food.value\">\n\t\t\t\t\t\t\t\t{{food.viewValue}}\n\t\t\t\t\t\t\t</mat-option>\n\t\t\t\t\t\t</mat-select>\n\t\t\t\t\t</mat-form-field>\n\t\t\t\t</div>\n\t\t\t</div>\n\n\t\t</div>\n\t</div>\n</div>"

/***/ }),

/***/ "./src/app/components/lighting/lighting.component.scss":
/*!*************************************************************!*\
  !*** ./src/app/components/lighting/lighting.component.scss ***!
  \*************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".lighting-component .card {\n  width: 100%;\n  -webkit-user-select: none;\n     -moz-user-select: none;\n      -ms-user-select: none;\n          user-select: none; }\n\n.lighting-component .ledRow {\n  padding-top: 5px; }\n\n.lighting-component .ledRow .led {\n    display: inline-block;\n    height: 40px;\n    width: 40px;\n    background-color: lightgray;\n    margin: 5px;\n    text-align: center;\n    line-height: 35px;\n    color: gray; }\n\n.lighting-component .ledRow .led.active {\n    background-color: black; }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy9saWdodGluZy9KOlxcSm9zZXBoXFxEb2N1bWVudHNcXEdpdEh1YlxcQXF1YXJpdW1EYXNoYm9hcmQvc3JjXFxhcHBcXGNvbXBvbmVudHNcXGxpZ2h0aW5nXFxsaWdodGluZy5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUdJLFdBQVU7RUFDVix5QkFBZ0I7S0FBaEIsc0JBQWdCO01BQWhCLHFCQUFnQjtVQUFoQixpQkFBZ0IsRUFBQTs7QUFKcEI7RUFTSSxnQkFBZSxFQUFBOztBQVRuQjtJQWFNLHFCQUFxQjtJQUNyQixZQUFXO0lBQ1gsV0FBVTtJQUNWLDJCQUEwQjtJQUMxQixXQUFVO0lBRVYsa0JBQWtCO0lBQ2xCLGlCQUFpQjtJQUNqQixXQUFXLEVBQUE7O0FBckJqQjtJQXlCTSx1QkFBc0IsRUFBQSIsImZpbGUiOiJzcmMvYXBwL2NvbXBvbmVudHMvbGlnaHRpbmcvbGlnaHRpbmcuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIubGlnaHRpbmctY29tcG9uZW50XG57XG4gIC5jYXJkIHtcbiAgICB3aWR0aDoxMDAlO1xuICAgIHVzZXItc2VsZWN0Om5vbmU7XG4gIH1cblxuICAubGVkUm93XG4gIHtcbiAgICBwYWRkaW5nLXRvcDo1cHg7XG4gICAgXG4gICAgLmxlZFxuICAgIHtcbiAgICAgIGRpc3BsYXk6IGlubGluZS1ibG9jaztcbiAgICAgIGhlaWdodDo0MHB4O1xuICAgICAgd2lkdGg6NDBweDtcbiAgICAgIGJhY2tncm91bmQtY29sb3I6bGlnaHRncmF5O1xuICAgICAgbWFyZ2luOjVweDtcblxuICAgICAgdGV4dC1hbGlnbjogY2VudGVyO1xuICAgICAgbGluZS1oZWlnaHQ6IDM1cHg7XG4gICAgICBjb2xvcjogZ3JheTtcbiAgICB9XG4gICAgLmxlZC5hY3RpdmVcbiAgICB7XG4gICAgICBiYWNrZ3JvdW5kLWNvbG9yOmJsYWNrO1xuICAgIH1cbiAgfVxufSJdfQ== */"

/***/ }),

/***/ "./src/app/components/lighting/lighting.component.ts":
/*!***********************************************************!*\
  !*** ./src/app/components/lighting/lighting.component.ts ***!
  \***********************************************************/
/*! exports provided: LightingComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LightingComponent", function() { return LightingComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");
/* harmony import */ var src_app_models_LEDElement__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! src/app/models/LEDElement */ "./src/app/models/LEDElement.ts");
/* harmony import */ var src_app_services_aquarium_service_aquarium_service__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! src/app/services/aquarium-service/aquarium.service */ "./src/app/services/aquarium-service/aquarium.service.ts");





var map = "04 03 02 01                                     25 26 27 28\n05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24\n48 47 46 45 44 43 42 41 40 39 38 37 36 35 34 33 32 31 30 29\n49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68\n84 83 72 81 80 79 78 77             76 75 74 73 72 71 70 69 \n".trim();
//To rows
var rows = [];
var r = map.split("\n");
var count = 0;
for (var i = 0; i < r.length; i++) {
    var row = r[i].replace(/\s\s\s/g, " XX").split(" ");
    var d = [];
    for (var j = 0; j < row.length; j++) {
        var id = row[j];
        var l = new src_app_models_LEDElement__WEBPACK_IMPORTED_MODULE_3__["LEDElement"]();
        if (id == "XX") {
            l.active = false;
            l.id = 0;
        }
        else {
            l.id = parseInt(id);
            l.active = true;
        }
        d.push(l);
    }
    rows[i] = d;
}
var LightingComponent = /** @class */ (function () {
    function LightingComponent(aquariumService, dialog) {
        this.aquariumService = aquariumService;
        this.dialog = dialog;
        this.rows = rows;
    }
    LightingComponent.prototype.ngOnInit = function () {
        console.log(this.rows);
    };
    LightingComponent.prototype.sendUpdate = function () {
        //Get all the leds
        var leds = [];
        for (var i = 0; i < this.rows.length; i++)
            this.rows[i].forEach(function (l) {
                if (!l.active)
                    return;
                var color = hexToRgb(l.color);
                if (color) {
                    l.r = color.r;
                    l.g = color.g;
                    l.b = color.b;
                }
                leds.push(l);
            });
        var config = {
            ledData: leds
        };
        this.aquariumService.SendLightingConfiguration(config);
    };
    LightingComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'lighting-page-component',
            template: __webpack_require__(/*! ./lighting.component.html */ "./src/app/components/lighting/lighting.component.html"),
            styles: [__webpack_require__(/*! ./lighting.component.scss */ "./src/app/components/lighting/lighting.component.scss")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [src_app_services_aquarium_service_aquarium_service__WEBPACK_IMPORTED_MODULE_4__["AquariumService"],
            _angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialog"]])
    ], LightingComponent);
    return LightingComponent;
}());

function hexToRgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}


/***/ }),

/***/ "./src/app/components/maintenance/maintenance.component.html":
/*!*******************************************************************!*\
  !*** ./src/app/components/maintenance/maintenance.component.html ***!
  \*******************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<mat-tab-group>\n    <mat-tab label=\"Tasks\"><task-table></task-table></mat-tab>\n    <mat-tab label=\"Water\"> Content 2 </mat-tab>\n    <mat-tab label=\"Parameters\"> Content 3 </mat-tab>\n  </mat-tab-group>"

/***/ }),

/***/ "./src/app/components/maintenance/maintenance.component.scss":
/*!*******************************************************************!*\
  !*** ./src/app/components/maintenance/maintenance.component.scss ***!
  \*******************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "table {\n  width: 100%; }\n\n.mat-column-select {\n  overflow: initial; }\n\nth.mat-sort-header-sorted {\n  color: black; }\n\n.red-number {\n  color: red; }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy9tYWludGVuYW5jZS9KOlxcSm9zZXBoXFxEb2N1bWVudHNcXEdpdEh1YlxcQXF1YXJpdW1EYXNoYm9hcmQvc3JjXFxhcHBcXGNvbXBvbmVudHNcXG1haW50ZW5hbmNlXFxtYWludGVuYW5jZS5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNJLFdBQVcsRUFBQTs7QUFFZjtFQUNFLGlCQUFpQixFQUFBOztBQUVuQjtFQUNFLFlBQVksRUFBQTs7QUFHZDtFQUNFLFVBQVUsRUFBQSIsImZpbGUiOiJzcmMvYXBwL2NvbXBvbmVudHMvbWFpbnRlbmFuY2UvbWFpbnRlbmFuY2UuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyJ0YWJsZSB7XG4gICAgd2lkdGg6IDEwMCU7XG59XG4ubWF0LWNvbHVtbi1zZWxlY3Qge1xuICBvdmVyZmxvdzogaW5pdGlhbDtcbn1cbnRoLm1hdC1zb3J0LWhlYWRlci1zb3J0ZWQge1xuICBjb2xvcjogYmxhY2s7XG59XG5cbi5yZWQtbnVtYmVye1xuICBjb2xvcjogcmVkO1xufSJdfQ== */"

/***/ }),

/***/ "./src/app/components/maintenance/maintenance.component.ts":
/*!*****************************************************************!*\
  !*** ./src/app/components/maintenance/maintenance.component.ts ***!
  \*****************************************************************/
/*! exports provided: MaintenanceComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "MaintenanceComponent", function() { return MaintenanceComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");



var MaintenanceComponent = /** @class */ (function () {
    function MaintenanceComponent(dialog) {
        this.dialog = dialog;
    }
    MaintenanceComponent.prototype.ngOnInit = function () {
    };
    MaintenanceComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'maintenance-page-component',
            template: __webpack_require__(/*! ./maintenance.component.html */ "./src/app/components/maintenance/maintenance.component.html"),
            styles: [__webpack_require__(/*! ./maintenance.component.scss */ "./src/app/components/maintenance/maintenance.component.scss")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialog"]])
    ], MaintenanceComponent);
    return MaintenanceComponent;
}());



/***/ }),

/***/ "./src/app/components/nav-menu/nav-menu.component.html":
/*!*************************************************************!*\
  !*** ./src/app/components/nav-menu/nav-menu.component.html ***!
  \*************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<header>\n  <nav class='navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3'>\n    <div class=\"container\">\n      <a class=\"navbar-brand\" [routerLink]='[\"/\"]'>MyAquarium</a>\n      <a class=\"navbar-brand\" [routerLink]='[\"/\"]'>Main Tank</a>\n      <button class=\"navbar-toggler\" type=\"button\" data-toggle=\"collapse\" data-target=\".navbar-collapse\" aria-label=\"Toggle navigation\"\n        [attr.aria-expanded]=\"isExpanded\" (click)=\"toggle()\">\n        <span class=\"navbar-toggler-icon\"></span>\n      </button>\n      <div class=\"navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse\" [ngClass]='{\"show\": isExpanded}'>\n        <ul class=\"navbar-nav flex-grow\">\n          <li class=\"nav-item\">\n            <a class=\"nav-link text-dark\" [routerLink]='[\"/\"]'  [routerLinkActiveOptions]='{ exact: true }' [routerLinkActive]='[\"link-active\"]'>Overview</a>\n          </li>\n          <li class=\"nav-item\" >\n            <a class=\"nav-link text-dark\" [routerLink]='[\"/maintenance\",\"tasks\"]' [routerLinkActive]='[\"link-active\"]'>Maintenance</a>\n          </li>\n          <li class=\"nav-item\" >\n              <a class=\"nav-link text-dark\" [routerLink]='[\"/fish\"]' [routerLinkActive]='[\"link-active\"]'>Fish</a>\n            </li>\n            <li class=\"nav-item\" >\n                <a class=\"nav-link text-dark\" [routerLink]='[\"/lighting\"]' [routerLinkActive]='[\"link-active\"]'>Lighting</a>\n              </li>\n          <li class=\"nav-item\" >\n            <a class=\"nav-link text-dark\" [routerLink]='[\"/settings\"]' [routerLinkActive]='[\"link-active\"]'>Settings</a>\n          </li>\n        </ul>\n      </div>\n    </div>\n  </nav>\n</header>\n"

/***/ }),

/***/ "./src/app/components/nav-menu/nav-menu.component.scss":
/*!*************************************************************!*\
  !*** ./src/app/components/nav-menu/nav-menu.component.scss ***!
  \*************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "a.navbar-brand {\n  white-space: normal;\n  text-align: center;\n  word-break: break-all; }\n\nhtml {\n  font-size: 14px; }\n\n@media (min-width: 768px) {\n  html {\n    font-size: 16px; } }\n\n.box-shadow {\n  box-shadow: 0 0.25rem 0.75rem rgba(0, 0, 0, 0.05); }\n\n.link-active {\n  font-weight: 500;\n  border-bottom: 3px solid #859BD3; }\n\n::ng-deep .mat-select-content {\n  max-height: 20vh !important; }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy9uYXYtbWVudS9KOlxcSm9zZXBoXFxEb2N1bWVudHNcXEdpdEh1YlxcQXF1YXJpdW1EYXNoYm9hcmQvc3JjXFxhcHBcXGNvbXBvbmVudHNcXG5hdi1tZW51XFxuYXYtbWVudS5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNFLG1CQUFtQjtFQUNuQixrQkFBa0I7RUFDbEIscUJBQXFCLEVBQUE7O0FBR3ZCO0VBQ0UsZUFBZSxFQUFBOztBQUVqQjtFQUNFO0lBQ0UsZUFBZSxFQUFBLEVBQ2hCOztBQUdIO0VBQ0UsaURBQThDLEVBQUE7O0FBRWhEO0VBQ0UsZ0JBQWU7RUFDZixnQ0FBK0IsRUFBQTs7QUFFakM7RUFDRSwyQkFBMkIsRUFBQSIsImZpbGUiOiJzcmMvYXBwL2NvbXBvbmVudHMvbmF2LW1lbnUvbmF2LW1lbnUuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyJhLm5hdmJhci1icmFuZCB7XG4gIHdoaXRlLXNwYWNlOiBub3JtYWw7XG4gIHRleHQtYWxpZ246IGNlbnRlcjtcbiAgd29yZC1icmVhazogYnJlYWstYWxsO1xufVxuXG5odG1sIHtcbiAgZm9udC1zaXplOiAxNHB4O1xufVxuQG1lZGlhIChtaW4td2lkdGg6IDc2OHB4KSB7XG4gIGh0bWwge1xuICAgIGZvbnQtc2l6ZTogMTZweDtcbiAgfVxufVxuXG4uYm94LXNoYWRvdyB7XG4gIGJveC1zaGFkb3c6IDAgLjI1cmVtIC43NXJlbSByZ2JhKDAsIDAsIDAsIC4wNSk7XG59XG4ubGluay1hY3RpdmUge1xuICBmb250LXdlaWdodDo1MDA7XG4gIGJvcmRlci1ib3R0b206M3B4IHNvbGlkICM4NTlCRDM7XG59XG46Om5nLWRlZXAgLm1hdC1zZWxlY3QtY29udGVudCB7XG4gIG1heC1oZWlnaHQ6IDIwdmggIWltcG9ydGFudDtcbn0iXX0= */"

/***/ }),

/***/ "./src/app/components/nav-menu/nav-menu.component.ts":
/*!***********************************************************!*\
  !*** ./src/app/components/nav-menu/nav-menu.component.ts ***!
  \***********************************************************/
/*! exports provided: NavMenuComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "NavMenuComponent", function() { return NavMenuComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");


var NavMenuComponent = /** @class */ (function () {
    function NavMenuComponent() {
        this.isExpanded = false;
    }
    NavMenuComponent.prototype.collapse = function () {
        this.isExpanded = false;
    };
    NavMenuComponent.prototype.toggle = function () {
        this.isExpanded = !this.isExpanded;
    };
    NavMenuComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'app-nav-menu',
            template: __webpack_require__(/*! ./nav-menu.component.html */ "./src/app/components/nav-menu/nav-menu.component.html"),
            styles: [__webpack_require__(/*! ./nav-menu.component.scss */ "./src/app/components/nav-menu/nav-menu.component.scss")]
        })
    ], NavMenuComponent);
    return NavMenuComponent;
}());



/***/ }),

/***/ "./src/app/components/operations/operations.component.html":
/*!*****************************************************************!*\
  !*** ./src/app/components/operations/operations.component.html ***!
  \*****************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<h3>Tank Operations</h3>\n<div class=\"aquarium-preview-component row\">\n    <button class=\"btn btn-primary\">LEDs On</button>\n</div>"

/***/ }),

/***/ "./src/app/components/operations/operations.component.scss":
/*!*****************************************************************!*\
  !*** ./src/app/components/operations/operations.component.scss ***!
  \*****************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".aquarium-preview-component {\n  margin-left: auto;\n  margin-right: auto; }\n  .aquarium-preview-component .left {\n    text-align: right; }\n  .aquarium-preview-img {\n  display: block;\n  width: 70%;\n  min-height: 400px;\n  border: 1px solid black; }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy9vcGVyYXRpb25zL0o6XFxKb3NlcGhcXERvY3VtZW50c1xcR2l0SHViXFxBcXVhcml1bURhc2hib2FyZC9zcmNcXGFwcFxcY29tcG9uZW50c1xcb3BlcmF0aW9uc1xcb3BlcmF0aW9ucy5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUVFLGlCQUFnQjtFQUNoQixrQkFBaUIsRUFBQTtFQUhuQjtJQU1JLGlCQUFnQixFQUFBO0VBR3BCO0VBRUUsY0FBYztFQUNkLFVBQVM7RUFDVCxpQkFBZ0I7RUFDaEIsdUJBQXNCLEVBQUEiLCJmaWxlIjoic3JjL2FwcC9jb21wb25lbnRzL29wZXJhdGlvbnMvb3BlcmF0aW9ucy5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5hcXVhcml1bS1wcmV2aWV3LWNvbXBvbmVudFxue1xuICBtYXJnaW4tbGVmdDphdXRvO1xuICBtYXJnaW4tcmlnaHQ6YXV0bztcbiAgXG4gIC5sZWZ0IHtcbiAgICB0ZXh0LWFsaWduOnJpZ2h0O1xuICB9XG59XG4uYXF1YXJpdW0tcHJldmlldy1pbWdcbntcbiAgZGlzcGxheTogYmxvY2s7XG4gIHdpZHRoOjcwJTtcbiAgbWluLWhlaWdodDo0MDBweDtcbiAgYm9yZGVyOjFweCBzb2xpZCBibGFjaztcbn0iXX0= */"

/***/ }),

/***/ "./src/app/components/operations/operations.component.ts":
/*!***************************************************************!*\
  !*** ./src/app/components/operations/operations.component.ts ***!
  \***************************************************************/
/*! exports provided: OperationsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "OperationsComponent", function() { return OperationsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");



var OperationsComponent = /** @class */ (function () {
    function OperationsComponent(dialog) {
        this.dialog = dialog;
    }
    OperationsComponent.prototype.ngOnInit = function () {
    };
    OperationsComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'operations',
            template: __webpack_require__(/*! ./operations.component.html */ "./src/app/components/operations/operations.component.html"),
            styles: [__webpack_require__(/*! ./operations.component.scss */ "./src/app/components/operations/operations.component.scss")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialog"]])
    ], OperationsComponent);
    return OperationsComponent;
}());



/***/ }),

/***/ "./src/app/components/settings/settings.component.html":
/*!*************************************************************!*\
  !*** ./src/app/components/settings/settings.component.html ***!
  \*************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "asdsa"

/***/ }),

/***/ "./src/app/components/settings/settings.component.scss":
/*!*************************************************************!*\
  !*** ./src/app/components/settings/settings.component.scss ***!
  \*************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "table {\n  width: 100%; }\n\n.mat-column-select {\n  overflow: initial; }\n\nth.mat-sort-header-sorted {\n  color: black; }\n\n.red-number {\n  color: red; }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy9zZXR0aW5ncy9KOlxcSm9zZXBoXFxEb2N1bWVudHNcXEdpdEh1YlxcQXF1YXJpdW1EYXNoYm9hcmQvc3JjXFxhcHBcXGNvbXBvbmVudHNcXHNldHRpbmdzXFxzZXR0aW5ncy5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNJLFdBQVcsRUFBQTs7QUFFZjtFQUNFLGlCQUFpQixFQUFBOztBQUVuQjtFQUNFLFlBQVksRUFBQTs7QUFHZDtFQUNFLFVBQVUsRUFBQSIsImZpbGUiOiJzcmMvYXBwL2NvbXBvbmVudHMvc2V0dGluZ3Mvc2V0dGluZ3MuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyJ0YWJsZSB7XG4gICAgd2lkdGg6IDEwMCU7XG59XG4ubWF0LWNvbHVtbi1zZWxlY3Qge1xuICBvdmVyZmxvdzogaW5pdGlhbDtcbn1cbnRoLm1hdC1zb3J0LWhlYWRlci1zb3J0ZWQge1xuICBjb2xvcjogYmxhY2s7XG59XG5cbi5yZWQtbnVtYmVye1xuICBjb2xvcjogcmVkO1xufSJdfQ== */"

/***/ }),

/***/ "./src/app/components/settings/settings.component.ts":
/*!***********************************************************!*\
  !*** ./src/app/components/settings/settings.component.ts ***!
  \***********************************************************/
/*! exports provided: SettingsComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SettingsComponent", function() { return SettingsComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");



var SettingsComponent = /** @class */ (function () {
    function SettingsComponent(dialog) {
        this.dialog = dialog;
    }
    SettingsComponent.prototype.ngOnInit = function () {
    };
    SettingsComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'settings-page-component',
            template: __webpack_require__(/*! ./settings.component.html */ "./src/app/components/settings/settings.component.html"),
            styles: [__webpack_require__(/*! ./settings.component.scss */ "./src/app/components/settings/settings.component.scss")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialog"]])
    ], SettingsComponent);
    return SettingsComponent;
}());



/***/ }),

/***/ "./src/app/components/task-list/task-list.component.html":
/*!***************************************************************!*\
  !*** ./src/app/components/task-list/task-list.component.html ***!
  \***************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<div class=\"aquarium-preview-component row m-2\">\n    <div class=\"card\">\n\n        <div class=\"card-body\">\n            <div class=\"card-title\">\n                <h3>Tasks</h3>\n            </div>\n            <div class=\"container\">\n                <div class=\"card m-2\">\n                    <div class=\"card-body\">\n                        <h5 class=\"card-title\">Water Change</h5>\n                        <h6 class=\"card-subtitle mb-2 text-muted\">3 days</h6>\n                        <p class=\"card-text\">This tank requires a water change to maintence adequete living environments\n                            for\n                            fish.\n                        </p>\n                        <a href=\"#\" class=\"card-link\">Complete</a>\n                        <a href=\"#\" class=\"card-link\">Dismiss</a>\n                    </div>\n                </div>\n                <div class=\"card m-2\">\n                    <div class=\"card-body\">\n                        <h5 class=\"card-title\">Water Test</h5>\n                        <h6 class=\"card-subtitle mb-2 text-muted\">5 days</h6>\n                        <p class=\"card-text\">A water test needs to be completed. PH, Nitrite, Nitrate, Ammonia</p>\n                        <a href=\"#\" class=\"card-link\">Complete</a>\n                        <a href=\"#\" class=\"card-link\">Dismiss</a>\n                    </div>\n                </div>\n            </div>\n\n        </div>\n    </div>\n\n</div>"

/***/ }),

/***/ "./src/app/components/task-list/task-list.component.scss":
/*!***************************************************************!*\
  !*** ./src/app/components/task-list/task-list.component.scss ***!
  \***************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".aquarium-preview-component {\n  margin-left: auto;\n  margin-right: auto; }\n  .aquarium-preview-component .left {\n    text-align: right; }\n  .aquarium-preview-img {\n  display: block;\n  width: 70%;\n  min-height: 400px;\n  border: 1px solid black; }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy90YXNrLWxpc3QvSjpcXEpvc2VwaFxcRG9jdW1lbnRzXFxHaXRIdWJcXEFxdWFyaXVtRGFzaGJvYXJkL3NyY1xcYXBwXFxjb21wb25lbnRzXFx0YXNrLWxpc3RcXHRhc2stbGlzdC5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUVFLGlCQUFnQjtFQUNoQixrQkFBaUIsRUFBQTtFQUhuQjtJQU1JLGlCQUFnQixFQUFBO0VBR3BCO0VBRUUsY0FBYztFQUNkLFVBQVM7RUFDVCxpQkFBZ0I7RUFDaEIsdUJBQXNCLEVBQUEiLCJmaWxlIjoic3JjL2FwcC9jb21wb25lbnRzL3Rhc2stbGlzdC90YXNrLWxpc3QuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIuYXF1YXJpdW0tcHJldmlldy1jb21wb25lbnRcbntcbiAgbWFyZ2luLWxlZnQ6YXV0bztcbiAgbWFyZ2luLXJpZ2h0OmF1dG87XG4gIFxuICAubGVmdCB7XG4gICAgdGV4dC1hbGlnbjpyaWdodDtcbiAgfVxufVxuLmFxdWFyaXVtLXByZXZpZXctaW1nXG57XG4gIGRpc3BsYXk6IGJsb2NrO1xuICB3aWR0aDo3MCU7XG4gIG1pbi1oZWlnaHQ6NDAwcHg7XG4gIGJvcmRlcjoxcHggc29saWQgYmxhY2s7XG59Il19 */"

/***/ }),

/***/ "./src/app/components/task-list/task-list.component.ts":
/*!*************************************************************!*\
  !*** ./src/app/components/task-list/task-list.component.ts ***!
  \*************************************************************/
/*! exports provided: TaskListComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TaskListComponent", function() { return TaskListComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");



var TaskListComponent = /** @class */ (function () {
    function TaskListComponent(dialog) {
        this.dialog = dialog;
    }
    TaskListComponent.prototype.ngOnInit = function () {
    };
    TaskListComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'task-list',
            template: __webpack_require__(/*! ./task-list.component.html */ "./src/app/components/task-list/task-list.component.html"),
            styles: [__webpack_require__(/*! ./task-list.component.scss */ "./src/app/components/task-list/task-list.component.scss")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialog"]])
    ], TaskListComponent);
    return TaskListComponent;
}());



/***/ }),

/***/ "./src/app/components/task-table/task-table.component.html":
/*!*****************************************************************!*\
  !*** ./src/app/components/task-table/task-table.component.html ***!
  \*****************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = "<table mat-table [dataSource]=\"dataSource\" class=\"task-table-component mat-elevation-z8\">\n\n    <!--- Note that these columns can be defined in any order.\n              The actual rendered columns are set as a property on the row definition\" -->\n\n    <!-- Position Column -->\n    <ng-container matColumnDef=\"position\">\n        <th mat-header-cell *matHeaderCellDef>Task No.</th>\n        <td mat-cell *matCellDef=\"let element\"> {{element.id}} </td>\n    </ng-container>\n\n    <!-- Name Column -->\n    <ng-container matColumnDef=\"name\">\n        <th mat-header-cell *matHeaderCellDef>Type</th>\n        <td mat-cell *matCellDef=\"let element\"> {{element.type}} </td>\n    </ng-container>\n\n    <!-- Symbol Column -->\n    <ng-container matColumnDef=\"symbol\">\n        <th mat-header-cell *matHeaderCellDef> Est. Due Date </th>\n        <td mat-cell *matCellDef=\"let element\"> {{element.dueDate.toDateString()}} </td>\n    </ng-container>\n\n    <tr mat-header-row *matHeaderRowDef=\"displayedColumns\"></tr>\n    <tr mat-row *matRowDef=\"let row; columns: displayedColumns;\"></tr>\n</table>"

/***/ }),

/***/ "./src/app/components/task-table/task-table.component.scss":
/*!*****************************************************************!*\
  !*** ./src/app/components/task-table/task-table.component.scss ***!
  \*****************************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = ".task-table-component {\n  width: 100%;\n  border: 1px solid #dee2e6;\n  border-top: 0px; }\n\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbInNyYy9hcHAvY29tcG9uZW50cy90YXNrLXRhYmxlL0o6XFxKb3NlcGhcXERvY3VtZW50c1xcR2l0SHViXFxBcXVhcml1bURhc2hib2FyZC9zcmNcXGFwcFxcY29tcG9uZW50c1xcdGFzay10YWJsZVxcdGFzay10YWJsZS5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUVJLFdBQVc7RUFDWCx5QkFBeUI7RUFDekIsZUFBYyxFQUFBIiwiZmlsZSI6InNyYy9hcHAvY29tcG9uZW50cy90YXNrLXRhYmxlL3Rhc2stdGFibGUuY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyIudGFzay10YWJsZS1jb21wb25lbnRcbntcbiAgICB3aWR0aDogMTAwJTtcbiAgICBib3JkZXI6IDFweCBzb2xpZCAjZGVlMmU2O1xuICAgIGJvcmRlci10b3A6MHB4O1xufSJdfQ== */"

/***/ }),

/***/ "./src/app/components/task-table/task-table.component.ts":
/*!***************************************************************!*\
  !*** ./src/app/components/task-table/task-table.component.ts ***!
  \***************************************************************/
/*! exports provided: TaskTableComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "TaskTableComponent", function() { return TaskTableComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_material__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/material */ "./node_modules/@angular/material/esm5/material.es5.js");



var ELEMENT_DATA = [
    { id: 1, priority: 1, type: "Water Change", issueDate: new Date(), dueDate: new Date() },
    { id: 1, priority: 1, type: "Parameter Test", issueDate: new Date(), dueDate: new Date() },
    { id: 1, priority: 1, type: "Feed Fish", issueDate: new Date(), dueDate: new Date() },
    { id: 1, priority: 1, type: "Parameter Test", issueDate: new Date(), dueDate: new Date() },
    { id: 1, priority: 1, type: "Water Change", issueDate: new Date(), dueDate: new Date() },
    { id: 1, priority: 1, type: "Water Change", issueDate: new Date(), dueDate: new Date() },
];
var TaskTableComponent = /** @class */ (function () {
    function TaskTableComponent(dialog) {
        this.dialog = dialog;
        this.displayedColumns = ['position', 'name', 'symbol'];
        this.dataSource = ELEMENT_DATA;
    }
    TaskTableComponent.prototype.ngOnInit = function () {
    };
    TaskTableComponent = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["Component"])({
            selector: 'task-table',
            template: __webpack_require__(/*! ./task-table.component.html */ "./src/app/components/task-table/task-table.component.html"),
            styles: [__webpack_require__(/*! ./task-table.component.scss */ "./src/app/components/task-table/task-table.component.scss")]
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_angular_material__WEBPACK_IMPORTED_MODULE_2__["MatDialog"]])
    ], TaskTableComponent);
    return TaskTableComponent;
}());



/***/ }),

/***/ "./src/app/environments/environment.ts":
/*!*********************************************!*\
  !*** ./src/app/environments/environment.ts ***!
  \*********************************************/
/*! exports provided: environment */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "environment", function() { return environment; });
// dev config
var environment = {
    production: false,
    urls: {
        aquariumApi: "https://localhost:44325/api"
    },
    environmentTag: "DEV"
};


/***/ }),

/***/ "./src/app/models/AquariumParameters.ts":
/*!**********************************************!*\
  !*** ./src/app/models/AquariumParameters.ts ***!
  \**********************************************/
/*! exports provided: AquariumParameters */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AquariumParameters", function() { return AquariumParameters; });
var AquariumParameters = /** @class */ (function () {
    function AquariumParameters() {
        this.temperature = 70;
        this.ph = 7.5;
        this.nitrate = 1;
        this.nitrite = 2;
    }
    return AquariumParameters;
}());



/***/ }),

/***/ "./src/app/models/AquariumSnapshot.ts":
/*!********************************************!*\
  !*** ./src/app/models/AquariumSnapshot.ts ***!
  \********************************************/
/*! exports provided: AquariumSnapshot */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AquariumSnapshot", function() { return AquariumSnapshot; });
/* harmony import */ var _AquariumParameters__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./AquariumParameters */ "./src/app/models/AquariumParameters.ts");

var MockSnapshotImages = [
    "data:image/jpeg;base64,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",
    "https://yournews.com/wp-content/uploads/Reuters_Direct_Media/USOnlineReportScienceNews/tagreuters.com2018binary_LYNXNPEE9H1WJ-VIEWIMAGE.jpg",
    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRqlNYTKsBdx6iJ_1hoQO2uJi8ngPWx6VjIektDcGxC-oZJ1QTGxQ"
];
function randomDate(start, end) {
    return new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()));
}
var AquariumSnapshot = /** @class */ (function () {
    function AquariumSnapshot() {
        this.id = Math.floor(Math.random() * 100);
        this.date = randomDate(new Date(2012, 0, 1), new Date());
        this.src = MockSnapshotImages[Math.floor(Math.random() * (MockSnapshotImages.length))];
        this.parameters = new _AquariumParameters__WEBPACK_IMPORTED_MODULE_0__["AquariumParameters"]();
    }
    return AquariumSnapshot;
}());



/***/ }),

/***/ "./src/app/models/LEDElement.ts":
/*!**************************************!*\
  !*** ./src/app/models/LEDElement.ts ***!
  \**************************************/
/*! exports provided: LEDElement */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LEDElement", function() { return LEDElement; });
var LEDElement = /** @class */ (function () {
    function LEDElement() {
        this.r = 0;
        this.g = 0;
        this.b = 0;
    }
    return LEDElement;
}());



/***/ }),

/***/ "./src/app/services/aquarium-service/aquarium.service.ts":
/*!***************************************************************!*\
  !*** ./src/app/services/aquarium-service/aquarium.service.ts ***!
  \***************************************************************/
/*! exports provided: AquariumService */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AquariumService", function() { return AquariumService; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "./node_modules/tslib/tslib.es6.js");
/* harmony import */ var _angular_common_http__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/common/http */ "./node_modules/@angular/common/fesm5/http.js");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _environments_environment__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../environments/environment */ "./src/app/environments/environment.ts");




var httpOptions = {
    headers: new _angular_common_http__WEBPACK_IMPORTED_MODULE_1__["HttpHeaders"]({
        'Content-Type': 'application/json',
        'Authorization': 'my-auth-token'
    })
};
var AquariumService = /** @class */ (function () {
    function AquariumService(http) {
        this.http = http;
        this._url = _environments_environment__WEBPACK_IMPORTED_MODULE_3__["environment"].urls.aquariumApi;
    }
    AquariumService.prototype.SendLightingConfiguration = function (config) {
        return this.http.post(this._url + "/lighting", config, httpOptions).toPromise()
            .then(function (data) {
            alert("here?!");
        })
            .catch(function (data) {
            alert("I expected to get an error!");
            console.log(data);
        });
    };
    AquariumService = tslib__WEBPACK_IMPORTED_MODULE_0__["__decorate"]([
        Object(_angular_core__WEBPACK_IMPORTED_MODULE_2__["Injectable"])({
            providedIn: "root"
        }),
        tslib__WEBPACK_IMPORTED_MODULE_0__["__metadata"]("design:paramtypes", [_angular_common_http__WEBPACK_IMPORTED_MODULE_1__["HttpClient"]])
    ], AquariumService);
    return AquariumService;
}());



/***/ }),

/***/ "./src/main.ts":
/*!*********************!*\
  !*** ./src/main.ts ***!
  \*********************/
/*! exports provided: getBaseUrl */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "getBaseUrl", function() { return getBaseUrl; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "./node_modules/@angular/core/fesm5/core.js");
/* harmony import */ var _angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/platform-browser-dynamic */ "./node_modules/@angular/platform-browser-dynamic/fesm5/platform-browser-dynamic.js");
/* harmony import */ var _app_app_module__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./app/app.module */ "./src/app/app.module.ts");
/* harmony import */ var _app_environments_environment__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./app/environments/environment */ "./src/app/environments/environment.ts");




function getBaseUrl() {
    return document.getElementsByTagName('base')[0].href;
}
var providers = [
    { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] }
];
if (_app_environments_environment__WEBPACK_IMPORTED_MODULE_3__["environment"].production) {
    Object(_angular_core__WEBPACK_IMPORTED_MODULE_0__["enableProdMode"])();
}
Object(_angular_platform_browser_dynamic__WEBPACK_IMPORTED_MODULE_1__["platformBrowserDynamic"])(providers).bootstrapModule(_app_app_module__WEBPACK_IMPORTED_MODULE_2__["AppModule"])
    .catch(function (err) { return console.log(err); });


/***/ }),

/***/ 0:
/*!***************************!*\
  !*** multi ./src/main.ts ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(/*! J:\Joseph\Documents\GitHub\AquariumDashboard\src\main.ts */"./src/main.ts");


/***/ })

},[[0,"runtime","vendor"]]]);
//# sourceMappingURL=main.js.map