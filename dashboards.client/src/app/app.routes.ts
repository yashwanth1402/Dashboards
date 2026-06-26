import { Routes } from '@angular/router';
import { ReportsComponent } from './reports/reports.component';
import { LoginComponent } from './auth/pages/login/login.component';
import { RolesAccessComponent } from './user-management/roles-access/roles-access.component';
import { UsersComponent } from './user-management/users/users.component';
import { authGuard } from './auth/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'reports',
    component: ReportsComponent,
    canActivate: [authGuard]
  },
  {
    path: 'user-management/roles-access',
    component: RolesAccessComponent,
    canActivate: [authGuard]
  },
  {
    path: 'user-management/users',
    component: UsersComponent,
    canActivate: [authGuard]
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login'
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
