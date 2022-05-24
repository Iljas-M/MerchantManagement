import { AfterViewInit, Component, OnInit } from '@angular/core';
import { filter, finalize, map, Observable, tap } from 'rxjs';
import { createDefaultMerchantAccount, Merchant, MerchantAccount } from '../interfaces/merchant-account';
import { MerchantService } from '../services/merchant.service';

@Component({
  selector: 'app-merchant-accounts',
  templateUrl: './merchant-accounts.component.html',
  styleUrls: ['./merchant-accounts.component.scss']
})
export class MerchantAccountsComponent implements OnInit, AfterViewInit {

  // Init.
  public merchantAccounts$: Observable<Merchant[]> = new Observable<Merchant[]>();

  constructor(private MerchantAccountsApiService: MerchantService) { }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {
    // Get the MerchantsAccounts.
    this.merchantAccounts$ = this.MerchantAccountsApiService.getMerchantAccounts().pipe(map(data => {
      return data.MerchantAccounts;
    }));
  }

  // Get the current clicked Merchant Account.
  editMerchant(merchant: Merchant) {
    // Transfer the merchant via service.
    this.MerchantAccountsApiService.setMerchantAccount(merchant);
  }
}