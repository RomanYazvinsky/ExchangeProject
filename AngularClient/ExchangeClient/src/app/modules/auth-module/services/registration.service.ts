﻿import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ValidationErrors} from '@angular/forms';
import {Observable} from 'rxjs';
import {UserRegistration} from '../../../models/user-registration';
import {EmailConfirmation} from '../components/email-confirmation/model/email-confirmation';

@Injectable()
export class RegistrationService {
  constructor(private http: HttpClient) {
  }

  checkUsername(username: string): Observable<ValidationErrors | null> {
    return this.http.post<ValidationErrors | null>('/api/validateUsername', {username});
  }
  checkPassword(password: string): Observable<ValidationErrors | null> {
    return this.http.post<ValidationErrors | null>('/api/validatePassword', {password});
  }
  checkEmail(email: string): Observable<ValidationErrors | null> {
    return this.http.post<ValidationErrors | null>('/api/validateEmail', {email});
  }
  register(user: UserRegistration): Observable<void> {
    return this.http.post<void>('/api/register', user)
  }

  confirmEmail(confirmation: EmailConfirmation): Observable<void> {
    return this.http.post<void>('/api/confirmEmail', confirmation);
  }
}
