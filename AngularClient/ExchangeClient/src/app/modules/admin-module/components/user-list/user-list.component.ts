import { Component, OnInit } from '@angular/core';
import {UserService} from '../../services/user.service';
import {tap} from 'rxjs/operators';
import {UserInfo} from '../../../../services/auth.service';
import {Observable} from 'rxjs';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {

  readonly displayedColumns: (keyof UserInfo)[] = ['username', 'email', 'role']
  users$: Observable<UserInfo[]> = this.userService.getUsers();

  constructor(private userService: UserService) { }

  ngOnInit() {
  }

}
