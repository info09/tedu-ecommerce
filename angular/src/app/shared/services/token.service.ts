import { Injectable } from '@angular/core';
import { ACCESS_TOKEN, REFRESH_TOKEN } from '../constants/keys.const';

@Injectable({
  providedIn: 'root',
})
export class TokenStorageService {
  constructor() {}

  signOut(): void {
    window.sessionStorage.clear();
  }

  public saveToken(token: string): void {
    window.sessionStorage.removeItem(ACCESS_TOKEN);
    window.sessionStorage.setItem(ACCESS_TOKEN, token);
  }

  public getToken(): string {
    return window.sessionStorage.getItem(ACCESS_TOKEN);
  }

  public saveRefreshToken(token: string) {
    window.sessionStorage.removeItem(REFRESH_TOKEN);
    window.sessionStorage.setItem(REFRESH_TOKEN, token);
  }

  public getRefreshToken(): string {
    return window.sessionStorage.getItem(REFRESH_TOKEN);
  }
}
