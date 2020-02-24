import {Component, OnInit} from '@angular/core';
import {AbstractControlOptions, FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {RegistrationService} from '../../services/registration.service';

@Component({
  selector: 'app-user-registration',
  templateUrl: './user-registration.component.html',
  styleUrls: ['./user-registration.component.scss']
})
export class UserRegistrationComponent implements OnInit {

  _form: FormGroup = this.formBuilder.group({
    'username': ['', {
      validators: [Validators.required],
      asyncValidators: [control => this.authService.checkUsername(control.value)],
      updateOn: 'blur'
    } as AbstractControlOptions
    ],
    'password': ['', {
      validators: [Validators.required],
      asyncValidators: [control => this.authService.checkPassword(control.value)],
      updateOn: 'blur'
    }],
    'email': ['', {
      validators: [Validators.required, Validators.email],
      asyncValidators: [control => this.authService.checkEmail(control.value)],
      updateOn: 'blur'
    }],
  });

  constructor(private formBuilder: FormBuilder,
              private authService: RegistrationService,
              private router: Router
  ) {
  }

  ngOnInit() {
  }

  register() {
    const {username, password, email} = this._form.value;
    this.authService.register({username, password, email})
      .subscribe(() => {
        this.router.navigate(['login'])
      });
  }
}
