﻿import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {UserRegistration} from '../../../models/user-registration';


@Injectable()
export class RegistrationService {
  constructor(private http: HttpClient) {
  }

  checkUsername(username: string): Observable<boolean> {
    return this.http.get('/api/username', {params: {username}}) as Observable<boolean>
  }
  register(user: UserRegistration): Observable<void> {
    return this.http.post('/api/register', user).pipe(map(() => {}))
  }
}
