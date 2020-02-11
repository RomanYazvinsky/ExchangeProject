﻿
import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, CanActivateChild, RouterStateSnapshot} from '@angular/router';
import {AuthService} from '../auth.service';

@Injectable()
export class UserPermissionsGuard implements CanActivate, CanActivateChild {
  constructor(private userAuthService: AuthService) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const userInfo = this.userAuthService.getUserInfo();
    return userInfo && userInfo.role === 'Administrator';
  }

  canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    return this.canActivate(childRoute, state);
  }

}
