import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {RegistrationService} from '../../services/registration.service';
import {AuthService} from '../../../../services/auth.service';
import {filter} from 'rxjs/operators';
import {Router} from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  _form: FormGroup = this.formBuilder.group({
    'username': ['', [Validators.required]],
    'password': ['', [Validators.required]]
  });

  constructor(private formBuilder: FormBuilder,
              private authService: AuthService,
              private userAuthService: RegistrationService,
              private router: Router
              ) {
  }

  ngOnInit() {
  }

  login() {
    const {username, password} = this._form.value;
    this.authService.login(username, password)
      .subscribe(value => {
      this.router.navigate(['/'])
    });
  }

}
