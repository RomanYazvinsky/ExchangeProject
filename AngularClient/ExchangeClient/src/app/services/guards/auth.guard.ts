import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, CanActivateChild, RouterStateSnapshot} from '@angular/router';
import {Observable} from 'rxjs';
import {Role} from '../../models/role';
import {PermissionService} from '../permission.service';

@Injectable()
export class AuthGuard implements CanActivateChild, CanActivate {
  constructor(private permissionService: PermissionService) {
  }

  canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.permissionService.isUserAllowed([Role.Customer, Role.Operator, Role.Administrator]);
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.canActivateChild(route, state);
  }

}
