import { Component, OnInit } from '@angular/core';
import { PrimeNGConfig } from 'primeng/api';
import { AuthService } from './shared/services/auth.service';
import { Router } from '@angular/router';
import { LOGIN_URL } from './shared/constants/urls.const';

@Component({
  selector: 'app-root',
  template: ` <router-outlet></router-outlet> `,
})
export class AppComponent implements OnInit {
  menuMode = 'static';

  constructor(
    private primeNGConfig: PrimeNGConfig,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.primeNGConfig.ripple = true;
    document.documentElement.style.fontSize = '14px';

    if (this.authService.isAuthenticate() === false) {
      this.router.navigate([LOGIN_URL]);
    }
  }
}
