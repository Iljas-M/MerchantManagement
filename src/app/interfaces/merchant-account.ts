export interface MerchantAccount {
    MerchantAccounts: Merchant[],
    Status: string,
    Message: string
}

export interface Merchant {
    id: string | null,
    paymentProvider: string | null,
    legalEntityName: string | null,
    countryCode: string | null,
    currency: string | null,
    storePaymentMethodMode: [] | null,
    senderId: string | null,
    country: string | null,
}

export function createDefaultMerchantAccount(): Merchant {
    return {
        id: "-1",
        paymentProvider: "-1",
        legalEntityName: "-1",
        countryCode: "-1",
        currency: "-1",
        storePaymentMethodMode: [],
        senderId: "-1",
        country: "-1"
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
        senderId: null,
        country: null
    }
}