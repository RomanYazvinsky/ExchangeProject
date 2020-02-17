import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, CanActivateChild, RouterStateSnapshot} from '@angular/router';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {Role} from '../../models/role';
import {PermissionService} from '../permission.service';

@Injectable()
export class NoAuthGuard implements CanActivateChild, CanActivate {
  constructor(private permissionService: PermissionService) {
  }

  canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    const anies: Role[] = Object.keys(Role).map((key) => Role[key]).filter(v => !!v);
    return this.permissionService.isUserAllowed(anies).pipe(map(isAllowed => !isAllowed));
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.canActivateChild(route, state);
  }

}
