import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {Base64Converter} from '../../../../Utils/base64.converter';
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
    const emailConfirmation: EmailConfirmation = Base64Converter.convert(data);
    this.email = emailConfirmation.email;
    this.registrationService.confirmEmail(emailConfirmation).subscribe(() => this.text = 'Confirmed')
  }

}
