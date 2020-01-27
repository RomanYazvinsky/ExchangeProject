﻿import {ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot} from '@angular/router';
import {AuthService} from '../auth.service';

export class UserPermissionsGuard implements CanActivate {
  constructor(private userAuthService: AuthService) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const userInfo = this.userAuthService.getUserInfo();
    return userInfo && userInfo.role === 'Administrator';
  }

}
