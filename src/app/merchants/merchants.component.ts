import { Component, OnInit } from '@angular/core';
import { filter, Observable, tap } from 'rxjs';
import { createDefaultMerchantAccount, Merchant, MerchantAccount } from '../interfaces/merchant-account';
import { MerchantService } from '../services/merchant.service';
@Component({
  selector: 'app-merchants',
  templateUrl: './merchants.component.html',
  styleUrls: ['./merchants.component.scss']
})
export class MerchantsComponent implements OnInit {

    // Init.
  public merchantAccounts$: Observable<MerchantAccount[]> = new Observable<MerchantAccount[]>();
  public currentMerchant: MerchantAccount[] = [{ MerchantAccount: createDefaultMerchantAccount() }];

  constructor(private MerchantAccountsApiService: MerchantService) { }

  ngOnInit(): void {
        // Get the MerchantsAccounts.
        this.merchantAccounts$ = this.MerchantAccountsApiService.getMerchantAccounts().pipe();
  }

  // Get the current clicked Merchant Account.
  editMerchant(merchant: Merchant) {
    // Transfer the merchant via service.
    this.MerchantAccountsApiService.setMerchantAccount(merchant);
  }
}
