import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SharedDeviceModule } from "../SharedDeviceModule/shared-device.module";
import { environment } from '../../../environments/environment';
import { DeviceEndpoints } from '../SharedDeviceModule/models/DeviceEndpoints';
import { Observable } from 'rxjs';
/* This file is treated as an interface. AquariumDashboard has an implementation and DeviceAPI Dashboard has one as well. 
    This allows us to completely copy the SharedDeviceModule over with ease.
*/

@Injectable({
    providedIn: SharedDeviceModule,
})
export class AquariumDeviceHttpClient {
    private _url: string;
    constructor(private http: HttpClient) {
        this._url = environment.url;
        if(this._url[this._url.length-1] != "/")
            this._url += "/";
    }
    public get<T>(endpoint: DeviceEndpoints, body?: object): Observable<T> {
        console.log("getting",this._url);
        return this.http.get<T>(this._url + endpoint, body);
    }
    public post<T>(endpoint: DeviceEndpoints, body?: object): Observable<T> {
        return this.http.post<T>(this._url + endpoint, body);
    }
    public put<T>(endpoint: DeviceEndpoints, body?: object) {
        return this.http.put(this._url + endpoint, body);
    }
    public delete(endpoint: DeviceEndpoints, body?: object) {
        return this.http.delete(this._url + endpoint, body);
    }
}