import { OAuthService } from 'angular-oauth2-oidc';
import { Component } from '@angular/core';
import { AuthService } from '@abp/ng.core';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss'],
})
export class ProductComponent {
  blockedPanel: boolean = false;
  items = [];

  constructor(private oAuthService: OAuthService, private authService: AuthService) {}

  get hasLoggedIn(): boolean {
    return this.oAuthService.hasValidAccessToken();
  }

  login() {
    this.authService.navigateToLogin();
  }
}
