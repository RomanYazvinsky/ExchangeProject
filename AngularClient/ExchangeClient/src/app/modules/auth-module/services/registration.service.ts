﻿import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {UserRegistration} from '../../../models/user-registration';
import {EmailConfirmation} from '../components/email-confirmation/model/email-confirmation';

@Injectable()
export class RegistrationService {
  constructor(private http: HttpClient) {
  }

  checkUsername(username: string): Observable<boolean> {
    return this.http.get('/api/username', {params: {username}}) as Observable<boolean>
  }
  register(user: {username: string, password: string, email: string}): Observable<void> {
    return this.http.post<void>('/api/register', {...user, emailConfirmationUrl: 'emailConfirmation'} as UserRegistration)
  }

  confirmEmail(confirmation: EmailConfirmation): Observable<void> {
    return this.http.post<void>('/api/confirmEmail', confirmation);
  }
}
