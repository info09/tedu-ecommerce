import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginRequestDto } from '../models/login-request.dto';
import { Observable } from 'rxjs';
import { LoginResponseDto } from '../models/login-response.dto';
import { environment } from 'src/environments/environment';

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
}
