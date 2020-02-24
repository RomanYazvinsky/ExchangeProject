import {Component, OnInit} from '@angular/core';
import {AbstractControlOptions, FormBuilder, FormGroup, Validators} from '@angular/forms';
import {RegistrationService} from '../../services/registration.service';
import {UsernameValidator} from '../../services/username.validator';

@Component({
  selector: 'app-user-registration',
  templateUrl: './user-registration.component.html',
  styleUrls: ['./user-registration.component.scss']
})
export class UserRegistrationComponent implements OnInit {


  _form: FormGroup = this.formBuilder.group({
    'username': ['', {
      validators: [Validators.required],
      asyncValidators: [this.usernameValidator.validate.bind(this.usernameValidator)],
      updateOn: 'blur'
    } as AbstractControlOptions
    ],
    'password': ['', [Validators.required]],
    'email': ['', [Validators.required, Validators.email]],
  });

  constructor(private formBuilder: FormBuilder,
              private authService: RegistrationService,
              private usernameValidator: UsernameValidator,
  ) {
  }

  ngOnInit() {
  }

  register() {
    const {username, password, email} = this._form.value;
    this.authService.register({username, password, email})
      .subscribe(() => {});
  }
}
