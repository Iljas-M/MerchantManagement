import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MerchantsComponent } from './merchants/merchants.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'home',
    component: HomeComponent
  },
  {
    path: 'merchants',
    component: MerchantsComponent
  },
  
  // Define default router that is always routed back to home.
  {
    path: '**',
    redirectTo: 'home'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
