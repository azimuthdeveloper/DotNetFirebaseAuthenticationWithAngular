import {ApplicationConfig, importProvidersFrom, provideZoneChangeDetection} from '@angular/core';
import {provideRouter} from '@angular/router';

import {routes} from './app.routes';
// import firebaseui from "firebaseui";
import {BrowserModule} from "@angular/platform-browser";
import {FormsModule} from "@angular/forms";
import {AngularFireAuthModule} from "@angular/fire/compat/auth";
import {AngularFireModule} from "@angular/fire/compat";
import {firebase, firebaseui, FirebaseUIModule} from 'firebaseui-angular';
import {provideHttpClient} from "@angular/common/http";


const firebaseConfig = {
  apiKey: "{{YOUR-API-KEY}}",
  authDomain: "{{YOUR-APP-ID}}.firebaseapp.com",
  projectId: "{{YOUR-APP-ID}}",
  storageBucket: "{{YOUR-APP-ID}}.firebasestorage.app",
  messagingSenderId: "***",
  appId: "***"
};

const firebaseUiAuthConfig: firebaseui.auth.Config = {
  signInFlow: 'popup',
  signInOptions: [
    firebase.auth.GoogleAuthProvider.PROVIDER_ID
  ],
  tosUrl: 'www.tos.com',
  privacyPolicyUrl: 'www.privacy.com',
  credentialHelper: firebaseui.auth.CredentialHelper.GOOGLE_YOLO
};

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({eventCoalescing: true}),
    provideRouter(routes),
    provideHttpClient(),

    importProvidersFrom(BrowserModule,
      FormsModule,
      FirebaseUIModule.forRoot(firebaseUiAuthConfig),
      AngularFireModule.initializeApp(firebaseConfig),),
    AngularFireAuthModule,
  ],
};
