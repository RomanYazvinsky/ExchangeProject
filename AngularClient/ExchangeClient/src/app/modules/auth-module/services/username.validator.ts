﻿import {AbstractControl, AsyncValidator, ValidationErrors} from '@angular/forms';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {RegistrationService} from './registration.service';
import {map} from 'rxjs/operators';

@Injectable()
export class UsernameValidator implements AsyncValidator {
  constructor(private authService: RegistrationService) {
  }

  validate(control: AbstractControl): Observable<ValidationErrors | null> {
    return this.authService.checkUsername(control.value).pipe(
      map(isUsed => {
        if (!isUsed) {
          return null;
        }
        return {'alreadyUsed': 'The name is already used by someone'};
      }));
  }

}
