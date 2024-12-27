import {Component, OnInit} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {FirebaseuiAngularLibraryComponent} from "firebaseui-angular";
import {AngularFireAuth} from "@angular/fire/compat/auth";
import {firstValueFrom, Observable} from "rxjs";
import {AsyncPipe, JsonPipe} from "@angular/common";
import {HttpClient, HttpHeaders} from "@angular/common/http";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, FirebaseuiAngularLibraryComponent, AsyncPipe, JsonPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'authtest';
  private apiUrl = 'http://localhost:5126/User/GetAuthenticatedUserDetail';
  user?: Observable<any>;
  token?: string;

  protectedResponseData?: string;


  constructor(protected firebase: AngularFireAuth, private http: HttpClient) {
  }

  async useProtectedResource() {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.token}`
    });
    const response = await firstValueFrom(this.http.get<string>(this.apiUrl, {headers}));
    this.protectedResponseData = response;

  }

  ngOnInit(): void {
    this.user = this.firebase.authState;
    this.firebase.authState.subscribe(async x => {
      this.token = await x?.getIdToken()
    });
  }

}
