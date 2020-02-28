﻿import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {User} from '../../../models/user';

@Injectable()
export class UserService {
  constructor(private http: HttpClient) {
  }

  getUsers(): Observable<User[]> {
    return this.http.get('/api/users') as Observable<User[]>
  }

  modifyUserRole(user: User): Observable<void> {
    return this.http.post<void>('/api/userRole', user)
  }

  removeUser(user: User): Observable<void> {
    return this.http.post<void>('/api/removeUser', user)
  }
}
