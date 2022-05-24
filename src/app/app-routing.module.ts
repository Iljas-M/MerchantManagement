import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MerchantAccountsComponent } from './merchant-accounts/merchant-accounts.component';
import { MerchantCardComponent } from './merchant-accounts/merchant-card/merchant-card.component';

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
    component: MerchantAccountsComponent
  },
  {
    path: 'merchants/account',
    component: MerchantCardComponent
  },
  {
    path: 'merchants/:id',
    component: MerchantCardComponent
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
