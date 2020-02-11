import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {UserListComponent} from './components/user-list/user-list.component';
import {UserEditComponent} from './components/user-edit/user-edit.component';
import {RouterModule} from '@angular/router';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
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
            component: UserListComponent,
            children: [{
                path: 'user/:userId',
                component: UserEditComponent
            }, {
                path: 'user',
                redirectTo: '/'
            }
            ]
        }]),
        MatTableModule,
        MatSelectModule
    ]
})
export class AdminModuleModule {
}
