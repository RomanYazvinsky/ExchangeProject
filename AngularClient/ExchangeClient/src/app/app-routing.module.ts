import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {NoAuthGuard} from './guards/no-auth.guard';


const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./modules/auth-module/auth-module.module').then(m => m.AuthModuleModule),
    canActivateChild: [NoAuthGuard],
    canActivate: [NoAuthGuard],
  },
  {
    path: 'admin',
    loadChildren: () => import('./modules/admin-module/admin-module.module').then(m => m.AdminModuleModule),
  },
  {
    path: '',
    redirectTo: '',
    pathMatch: 'full'
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
