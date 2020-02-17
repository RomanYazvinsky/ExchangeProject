﻿import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, CanActivateChild, RouterStateSnapshot} from '@angular/router';
import {Observable} from 'rxjs';
import {Role} from '../../models/role';
import {PermissionService} from '../permission.service';

@Injectable()
export class AdministrationZoneGuard implements CanActivate, CanActivateChild {
  constructor(private permissionService: PermissionService) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.permissionService.isUserAllowed(Role.Administrator);
  }

  canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.canActivate(childRoute, state);
  }

}
