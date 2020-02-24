import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {of} from 'rxjs';
import {map} from 'rxjs/operators';
import {Role} from '../../models/role';
import {AuthService} from '../../services/auth.service';
import {PermissionService} from '../../services/permission.service';
import {ToolbarButton} from './button/toolbar-button';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss']
})
export class ToolbarComponent implements OnInit {

  _toolbarButtons: ToolbarButton[] = [];

  constructor(public authService: AuthService, private router: Router, private permissionService: PermissionService) {
  }

  ngOnInit(): void {
    this._toolbarButtons = this.buildDefaultToolbarButtons();
  }

  private buildDefaultToolbarButtons(): ToolbarButton[] {
    return [{
      executeAsync: () => this.openHome(),
      isAvailable$: of(true),
      isEnabled$: of(true),
      label: 'Go home'
    }, {
      label: 'Register',
      isEnabled$: of(true),
      isAvailable$: this.authService.authentication$.pipe(map(a => !a)),
      executeAsync: () => this.openRegistration()
    }, {
      label: 'Login',
      isEnabled$: of(true),
      isAvailable$: this.authService.authentication$.pipe(map(a => !a)),
      executeAsync: () => this.openLogin()
    },  {
      label: 'Logout',
      isEnabled$: of(true),
      isAvailable$: this.authService.authentication$.pipe(map(a => !!a)),
      executeAsync: () => this.logout()
    }, {
      label: 'Open Admin page',
      isEnabled$: of(true),
      isAvailable$: this.permissionService.isUserAllowed(Role.Administrator),
      executeAsync: () => this.openAdminPage()
    }, {
      label: 'Open Products',
      isEnabled$: of(true),
      isAvailable$: this.permissionService.isUserAllowed([Role.Customer, Role.Operator, Role.Administrator]),
      executeAsync: () => this.openProducts()
    }];
  }

  openHome(): Promise<boolean> {
    return this.router.navigate(['/']);
  }

  openRegistration(): Promise<boolean> {
    return this.router.navigate(['register']);
  }
  openLogin(): Promise<boolean> {
    return this.router.navigate(['login']);
  }

  openAdminPage(): Promise<boolean> {
    return this.router.navigate(['admin']);
  }

  openProducts(): Promise<boolean> {
    return this.router.navigate(['products']);
  }

  logout() {
    this.authService.logout()
      .subscribe(() => {
        this.openHome();
      });
  }

}
