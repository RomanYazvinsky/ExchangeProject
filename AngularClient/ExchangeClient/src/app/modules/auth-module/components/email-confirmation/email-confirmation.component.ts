import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {RegistrationService} from '../../services/registration.service';
import {EmailConfirmation} from './model/email-confirmation';

@Component({
  selector: 'app-email-confirmation',
  templateUrl: './email-confirmation.component.html',
  styleUrls: ['./email-confirmation.component.scss']
})
export class EmailConfirmationComponent implements OnInit {

  constructor(private registrationService: RegistrationService, private route: ActivatedRoute) { }

  text = 'Email is confirming';
  email?: string = null;

  ngOnInit(): void {
    const data: string = this.route.snapshot.params['data'];
    const emailConfirmation: EmailConfirmation = JSON.parse(this.fromBase64(data));
    this.email = emailConfirmation.email;
    this.registrationService.confirmEmail(emailConfirmation).subscribe(() => this.text = 'Confirmed')
  }

  private fromBase64(base64String: string): string {
    return decodeURIComponent(atob(base64String).split('').map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));
  }

}
