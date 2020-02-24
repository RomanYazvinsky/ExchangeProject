import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {RegistrationService} from '../../services/registration.service';

@Component({
  selector: 'app-email-confirmation',
  templateUrl: './email-confirmation.component.html',
  styleUrls: ['./email-confirmation.component.scss']
})
export class EmailConfirmationComponent implements OnInit {

  constructor(private registrationService: RegistrationService, private route: ActivatedRoute) { }

  text = 'Email is confirming';

  ngOnInit(): void {
    const id = this.route.snapshot.params['id'];
    this.registrationService.confirmEmail(id).subscribe(() => this.text = 'Confirmed')
  }

}
