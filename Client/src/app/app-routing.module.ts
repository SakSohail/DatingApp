import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages/messages.component';
import { AuthGuard } from './_guards/auth.gaurd';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { MemberDetailedResolver } from './_resolvers/member-detailed.resolver';
const routes: Routes = [
    {path: '', component: HomeComponent},
    {
      path: '',
      runGuardsAndResolvers: 'always',
      canActivate: [AuthGuard],
      children: [
        // {path: 'members', component: MemberListComponent, canActivate: [AuthGuard]},
        {path: 'members', component: MemberListComponent},
        // {path: 'members/:id', component: MemberDetailsComponent},
        {path: 'members/:username', component: MemberDetailsComponent, resolve: {member: MemberDetailedResolver}},
        {path: 'member/edit', component: MemberEditComponent},
        {path: 'lists', component: ListsComponent},
        {path: 'messages', component: MessagesComponent},
        {path: 'member/edit', component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard]}//prevent save changes when swithichg to another component
        ,
      ]
    },
    {path: 'errors', component: TestErrorsComponent},
    {path: 'not-found', component: NotFoundComponent},
    {path: 'server-error', component: ServerErrorComponent},
    {path: '**', component: HomeComponent, pathMatch: 'full'},
    
  ];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }