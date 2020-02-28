import {SelectionModel} from '@angular/cdk/collections';
import {ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {Observable} from 'rxjs';
import {Role} from '../../../../models/role';
import {User} from '../../../../models/user';
import {AuthService} from '../../../../services/auth.service';
import {UserService} from '../../services/user.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserListComponent implements OnInit {

  readonly displayedColumns: (keyof User | string)[] = ['username', 'email', 'isEmailConfirmed', 'role', 'removeUserButton'];
  readonly roles: (string)[] = Object.keys(Role).filter(key => typeof Role[key] === 'number');
  readonly currentUser: Observable<User | null> = this.authService.currentUser$;

  selection: SelectionModel<User> = new SelectionModel<User>(false, []);
  users$: Observable<User[]> = this.userService.getUsers();

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private changeDetectorRef: ChangeDetectorRef,
    private router: Router) {
  }

  ngOnInit() {
  }

  saveRole(userDto: User): void {
    this.userService.modifyUserRole(userDto).subscribe();
  }

  openUser(user: User): void {
    this.router.navigate(['admin', 'user', user.id]);
  }

  removeUser(user: User) {
    this.userService.removeUser(user).subscribe(() => {
      this.users$ = this.userService.getUsers();
      this.changeDetectorRef.markForCheck();
    });
  }
}
