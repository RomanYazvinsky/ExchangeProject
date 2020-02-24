import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {Role} from '../models/role';
import {AuthService} from './auth.service';

@Injectable()
export class PermissionService {
  constructor(private authService: AuthService) {
  }

  isUserAllowed(role: Role): Observable<boolean>;
  isUserAllowed(roles: Role[]): Observable<boolean>;
  isUserAllowed(role: Role | Role[]): Observable<boolean> {
    const roles: Role[] = [];
    if (!Array.isArray(role)) {
      roles.push(role);
    } else {
      roles.push(...role);
    }
    return this.authService.currentUser$.pipe(map(user => roles.some(grantedRole => Role[grantedRole] === user?.role)));
  }

}
