import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RegistrationService} from './services/registration.service';
import {LoginComponent} from './components/login/login.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';
import {MatInputModule} from '@angular/material/input';
import {MatButtonModule} from '@angular/material/button';
import {MatFormFieldModule} from '@angular/material/form-field';
import {UserRegistrationComponent} from './components/user-registration/user-registration.component';
import {AuthFormComponent} from './components/auth-form/auth-form.component';
import { EmailConfirmationComponent } from './components/email-confirmation/email-confirmation.component'


@NgModule({
  declarations: [LoginComponent, UserRegistrationComponent, AuthFormComponent, EmailConfirmationComponent],
  providers: [RegistrationService],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    RouterModule.forChild([
      {
        path: '',
        component: AuthFormComponent,
        children: [
          {
            path: 'login',
            component: LoginComponent,
          }, {
            path: 'register',
            component: UserRegistrationComponent
          }, {
            path: 'confirmation/:data',
            component: EmailConfirmationComponent,
          }
        ]
      },
    ]),
  ]
})
export class AuthModuleModule {
  constructor() {
  }
}
