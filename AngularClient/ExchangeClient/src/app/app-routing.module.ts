import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {NoAuthGuard} from './services/guards/no-auth.guard';
import {AdministrationZoneGuard} from './services/guards/administration-zone.guard';


const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./modules/auth-module/auth-module.module').then(m => m.AuthModuleModule),
    canActivateChild: [NoAuthGuard],
    canActivate: [NoAuthGuard],
  },
  {
    path: 'admin',
    loadChildren: () => import('./modules/admin-module/admin.module').then(m => m.AdminModuleModule),
    canActivate: [
      AdministrationZoneGuard
    ]
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
