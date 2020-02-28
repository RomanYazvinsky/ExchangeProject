import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {MatButtonModule} from '@angular/material/button';
import {MatSelectModule} from '@angular/material/select';
import {MatTableModule} from '@angular/material/table';
import {RouterModule} from '@angular/router';
import {UserEditComponent} from './components/user-edit/user-edit.component';
import {UserListComponent} from './components/user-list/user-list.component';
import {UserService} from './services/user.service';

@NgModule({
  declarations: [UserListComponent, UserEditComponent],
  providers: [
    UserService
  ],
  imports: [
    CommonModule,
    RouterModule.forChild([{
      path: '',
      children: [{
        path: 'user/:userId',
        component: UserEditComponent
      }, {
        path: 'user',
        redirectTo: ''
      }, {
        path: '',
        component: UserListComponent,
      }
      ]
    }]),
    MatTableModule,
    MatSelectModule,
    MatButtonModule
  ]
})
export class AdminModuleModule {
}
