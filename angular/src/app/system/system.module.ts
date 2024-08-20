import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { EditorModule } from 'primeng/editor';
import { TeduSharedModule } from '../shared/modules/tedu-shared.module';
import { BadgeModule } from 'primeng/badge';
import { ImageModule } from 'primeng/image';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { CalendarModule } from 'primeng/calendar';
import { SystemRoutingModule } from './system-routing.module';
import { RoleComponent } from './roles/role.component';
import { RoleDetailComponent } from './roles/role-detail/role-detail.component';
import { PermissionGrantComponent } from './roles/permission-grant/permission-grant.component';
import { UserComponent } from './users/user.component';
import { UserDetailComponent } from './users/user-detail/user-detail.component';
import { RoleAssignComponent } from './users/role-assign/role-assign.component';
import { PickListModule } from 'primeng/picklist';
import { SetPasswordComponent } from './users/set-password/set-password.component';
import { KeyFilterModule } from 'primeng/keyfilter';

@NgModule({
  declarations: [
    RoleComponent,
    RoleDetailComponent,
    PermissionGrantComponent,
    UserComponent,
    UserDetailComponent,
    RoleAssignComponent,
    SetPasswordComponent,
  ],
  imports: [
    SharedModule,
    PanelModule,
    TableModule,
    PaginatorModule,
    BlockUIModule,
    ButtonModule,
    DropdownModule,
    InputTextModule,
    ProgressSpinnerModule,
    DynamicDialogModule,
    InputNumberModule,
    CheckboxModule,
    InputTextareaModule,
    EditorModule,
    TeduSharedModule,
    BadgeModule,
    ImageModule,
    ConfirmDialogModule,
    CalendarModule,
    SystemRoutingModule,
    PickListModule,
    KeyFilterModule,
  ],
  entryComponents: [
    RoleDetailComponent,
    PermissionGrantComponent,
    UserDetailComponent,
    RoleAssignComponent,
    SetPasswordComponent,
  ],
})
export class SystemModule {}
