import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RoleComponent } from './roles/role.component';
import { UserComponent } from './users/user.component';
import { PermissionGuard } from '@abp/ng.core';

const routes: Routes = [
  {
    path: 'role',
    component: RoleComponent,
    canActivate: [PermissionGuard],
    data: { requiredPolicy: 'AbpIdentity.Roles' },
  },
  {
    path: 'user',
    component: UserComponent,
    canActivate: [PermissionGuard],
    data: {
      requiredPolicy: 'AbpIdentity.Users',
    },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SystemRoutingModule {}
