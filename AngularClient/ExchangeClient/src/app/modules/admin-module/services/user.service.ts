﻿import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {UserDto} from '../../../services/auth.service';
import {Observable} from 'rxjs';

@Injectable()
export class UserService {
  constructor(private http: HttpClient) {
  }

  getUsers(): Observable<UserDto[]> {
    return this.http.get('/api/users') as Observable<UserDto[]>
  }

  modifyUserRole(user: UserDto): Observable<void> {
    return this.http.post<void>('/api/userRole', user)
  }
}
