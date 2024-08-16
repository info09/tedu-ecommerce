import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginRequestDto } from '../models/login-request.dto';
import { Observable } from 'rxjs';
import { LoginResponseDto } from '../models/login-response.dto';
import { environment } from 'src/environments/environment';
import { ACCESS_TOKEN, REFRESH_TOKEN } from '../constants/keys.const';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient) {}
  public login(input: LoginRequestDto): Observable<LoginResponseDto> {
    var body = {
      username: input.username,
      password: input.password,
      client_id: environment.oAuthConfig.clientId,
      client_secret: environment.oAuthConfig.dummyClientSecret,
      grant_type: 'password',
      scope: environment.oAuthConfig.scope,
    };

    const data = Object.keys(body)
      .map((key, index) => `${key}=${encodeURIComponent(body[key])}`)
      .join('&');
    return this.http.post<LoginResponseDto>(
      `${environment.oAuthConfig.issuer}connect/token`,
      data,
      { headers: { 'Content-type': 'application/x-www-form-urlencoded' } }
    );
  }

  public refreshToken(refreshToken: string): Observable<LoginResponseDto> {
    var body = {
      client_id: environment.oAuthConfig.clientId,
      client_secret: environment.oAuthConfig.dummyClientSecret,
      grant_type: 'refresh_token',
      refresh_token: refreshToken,
    };

    const data = Object.keys(body)
      .map((key, index) => `${key}=${encodeURIComponent(body[key])}`)
      .join('&');
    return this.http.post<LoginResponseDto>(
      `${environment.oAuthConfig.issuer}connect/token`,
      data,
      { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }
    );
  }

  public logout() {
    window.sessionStorage.removeItem(ACCESS_TOKEN);
    window.sessionStorage.removeItem(REFRESH_TOKEN);
  }

  public isAuthenticate(): boolean {
    return window.sessionStorage.getItem(ACCESS_TOKEN) != null;
  }
}
