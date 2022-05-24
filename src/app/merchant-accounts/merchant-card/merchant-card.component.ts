import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { createNullMerchantAccount as createNullEmptyMerchantAccount, Merchant } from '../../interfaces/merchant-account';
import { createDefaultMerchantAccount } from '../../interfaces/merchant-account';
import { MerchantService } from 'src/app/services/merchant.service';
import { Subscription } from 'rxjs';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-merchant-card',
  templateUrl: './merchant-card.component.html',
  styleUrls: ['./merchant-card.component.scss']
})
export class MerchantCardComponent implements OnInit {
  
  // Init.
  public merchantAccount: Merchant = createDefaultMerchantAccount();
  public merchantAccountByIdExist: Merchant | undefined = undefined;
  public id: string | null = null;
  public merchantAccountIsActive: RegExpMatchArray | null = null;
  public merchantAccountForm: FormGroup;
  public isInvalid: { [id:string]:string } =  {};
  private subscriptions: Subscription[] = [];

  constructor(
    private router: ActivatedRoute,
    private MerchantAccountsApiService: MerchantService,
    private route: Router,
    private formBuilder: FormBuilder
  ) {

    this.merchantAccountForm = this.formBuilder.group({});
  }

  ngOnInit(): void {

    // Get from Route the Id.
    this.id = this.router.snapshot.paramMap.get('id');

    // Get the Merchant with selectedId.
    if (this.id != null) {
      this.merchantAccountByIdExist = this.MerchantAccountsApiService.getMerchantAccount(this.id);

      if (typeof this.merchantAccountByIdExist === "undefined") return console.info(`Merchant Account with id "${this.id}" not found!`)
      this.merchantAccount = this.merchantAccountByIdExist;
      // throw new TypeError(`Merchant Account with id "${this.id}" not found!`)
    }

    // Handle the create merchant account.
    this.merchantAccountIsActive = this.route.url.match('/merchants/account');

    if (this.merchantAccountIsActive != null) {
      this.merchantAccount = createNullEmptyMerchantAccount();
    }

    // Create React Form.
    this.merchantAccountForm = this.formBuilder.group({
      id: [this.merchantAccount.id,
      [
        Validators.required, Validators.pattern("^[0-9]{4}-[0-9]{4}-[A-Z]{2}-[0-9]{2}-[0-9]{2}$")
      ]
      ],
      relatedContract: [this.merchantAccount.paymentProvider, [ Validators.required ]],
      senderId: [this.merchantAccount.legalEntityName, [
        Validators.required,
        Validators.minLength(1)
      ]],    
      countryCode: [this.merchantAccount.countryCode,
      [
        Validators.required,
        Validators.pattern("^[A-Z]{2}$")
      ]
      ],
      currency: [this.merchantAccount.currency,
      [
        Validators.required,
        Validators.pattern("^[A-Z]{3}$")
      ]
      ]
    }); 

  }


  inputChanged(key: string) {
    // Handle when the field is valid or invalid.
    this.isInvalid[key] = this.merchantAccountForm.controls[key].valid ? "is-valid" : "is-invalid";
  }

  // Create Merchant.
  createUpdateButton(click: MouseEvent) {
    click.preventDefault();

    const observable = this.MerchantAccountsApiService.createMerchant({ MerchantAccount: this.merchantAccount });
    const subscription = observable.subscribe();
    this.subscriptions.push(subscription);
  }

  // Delete Merchant.
  deleteButton(click: MouseEvent) {
    click.preventDefault();

    const observable = this.MerchantAccountsApiService.deleteMerchant(this.id!)
    const subscription = observable.subscribe();
    this.subscriptions.push(subscription);
  }

  ngOnDestroy() {
    // Unsubscribe the Requests.
    this.subscriptions.forEach(subscription => subscription.unsubscribe())
  }

}
