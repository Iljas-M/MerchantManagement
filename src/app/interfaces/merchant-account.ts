export interface MerchantAccount {
    MerchantAccount: Merchant
}

export interface Merchant {
    id: string | null,
    paymentProvider: string | null,
    legalEntityName: string | null,
    countryCode: string | null,
    currency: string | null,
    storePaymentMethodMode: [] | null
}

export function createDefaultMerchantAccount(): Merchant {
    return {
        id: "-1",
        paymentProvider: "-1",
        legalEntityName: "-1",
        countryCode: "-1",
        currency: "-1",
        storePaymentMethodMode: [],
    }
}

export function createNullMerchantAccount(): Merchant {
    return {
        id: null,
        paymentProvider: null,
        legalEntityName: null,
        countryCode: null,
        currency: null,
        storePaymentMethodMode: [],
    }
}