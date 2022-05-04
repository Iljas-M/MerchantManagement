import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { createDefaultMerchantAccount, Merchant, MerchantAccount } from '../interfaces/merchant-account';

@Injectable({
  providedIn: 'root'
})
export class MerchantService {
  // Init.
  public merchantAccount: Merchant = createDefaultMerchantAccount();
  private contentType: string = 'application/json';
  private headers = new HttpHeaders({ 'Content-Type': this.contentType });
  private apiHostPath = " http://localhost:7071/api";

  constructor(private http: HttpClient) { }

  // Set the Merchant.
  setMerchantAccount(data: Merchant): void {
    this.merchantAccount = data;
  }

  // Get the Merchant by Id.
  getMerchantAccount(id: string): Merchant | undefined {
    const merchant = this.merchantAccount.id === id ? this.merchantAccount: undefined
    return merchant;
  }

  // Get all Merchant Accounts.
  getMerchantAccounts(): Observable<MerchantAccount[]> {
    return this.http.get<MerchantAccount[]>(`${this.apiHostPath}/get/merchants`, { headers: this.headers });
  }

  // Create and Update Merchant.
  createUpdateMerchant(merchant: Object): Observable<MerchantAccount> {
    return this.http.put<MerchantAccount>(`${this.apiHostPath}/create/merchant`, merchant, { headers: this.headers });
  }

  // Delete Merchant by Id.
  deleteMerchant(id: string) {
    return this.http.delete(`${this.apiHostPath}/delete/merchant/${id}`, { headers: this.headers });
  }

  getOpenApi(){
    return this.http.get<any>('assets/mock/dphOpenApi.json');
    return this.http.get<any>(`${this.apiHostPath}/private/api/management/portal/apis/zeiss-payment-hub-merchant-management/openapi`);
  }
}