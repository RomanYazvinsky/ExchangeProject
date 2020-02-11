import {ChangeDetectionStrategy, Component, OnInit} from '@angular/core';
import {Observable} from 'rxjs';
import {AuthService, UserDto} from '../../../../services/auth.service';
import {UserService} from '../../services/user.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserListComponent implements OnInit {

  readonly displayedColumns: (keyof UserDto)[] = ['username', 'email', 'role'];
  users$: Observable<UserDto[]> = this.userService.getUsers();
  readonly roles = ['Administrator', 'Customer', 'Operator', 'Disabled'];

  readonly currentUser: UserDto | null = this.authService.getUserInfo();

  constructor(private userService: UserService, private authService: AuthService) {
  }

  ngOnInit() {
  }

  saveRole(userDto: UserDto) {
    this.userService.modifyUserRole(userDto).subscribe();
  }

}
