﻿import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {UserInfo} from '../../../services/auth.service';
import {Observable} from 'rxjs';

@Injectable()
export class UserService {
  constructor(private http: HttpClient) {
  }

  getUsers(): Observable<UserInfo[]> {
    return this.http.get('/api/users') as Observable<UserInfo[]>
  }
}
