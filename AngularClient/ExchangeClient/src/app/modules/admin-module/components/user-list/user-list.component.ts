import {ChangeDetectionStrategy, Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {Observable} from 'rxjs';
import {Role} from '../../../../models/role';
import {UserDto} from '../../../../models/user.dto';
import {AuthService} from '../../../../services/auth.service';
import {UserService} from '../../services/user.service';

@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserListComponent implements OnInit {

    readonly displayedColumns: (keyof UserDto)[] = ['username', 'email', 'isEmailConfirmed', 'role'];
    readonly roles: (string)[] = Object.keys(Role).filter(key => typeof Role[key] === 'number');
    readonly currentUser: Observable<UserDto | null> = this.authService.currentUser$;

    users$: Observable<UserDto[]> = this.userService.getUsers();
    selectedUser?: UserDto;

    constructor(
        private userService: UserService,
        private authService: AuthService,
        private router: Router) {
    }

    ngOnInit() {
    }

    selectRow(user: UserDto) {
        this.selectedUser = user;
    }

    saveRole(userDto: UserDto) {
        this.userService.modifyUserRole(userDto).subscribe();
    }

    openUser(user: UserDto) {
        this.router.navigate(['admin', 'user', user.id]);
    }

}
