import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginInformationModel } from '../models/LoginInformation.model';

const httpOptions = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': 'my-auth-token'
    })
};


@Injectable({
    providedIn: "root"
})
export class ClientService {
    private _url: string;
    private _cdn: string;

    public aquariumId: number;

    constructor(private http: HttpClient) {
        this._url = environment.url;
    }

    public attemptLogin(loginInformation: LoginInformationModel) {
        return this.http.post(this._url + "ClientApp/Login", loginInformation);
    }
    public logout() {
        return this.http.delete(this._url + "ClientApp/Logout").subscribe(location.reload, location.reload);
    }
    public getDetailedInformation() {
        return this.http.get(this._url + "ClientApp");
    }

}
