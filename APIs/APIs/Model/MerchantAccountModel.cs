// <copyright file="MerchantAccountModel.cs" company="PlanB. GmbH">
// Copyright (c) PlanB. GmbH. All rights reserved.
// </copyright>

using System.Collections.Generic;

using Newtonsoft.Json;

namespace APIs.Model
{
  /// <summary>
  /// The Merchant Account Model.
  /// </summary>
  public class MerchantAccountModel
  {
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [JsonProperty(propertyName: "id", Required = Required.Default)]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the payment provider.
    /// </summary>
    /// <value>
    /// The payment provider.
    /// </value>
    [JsonProperty(propertyName: "paymentProvider", Required = Required.Default)]
    public string PaymentProvider { get; set; }

    /// <summary>
    /// Gets or sets the name of the legal entity.
    /// </summary>
    /// <value>
    /// The name of the legal entity.
    /// </value>
    [JsonProperty(propertyName: "legalEntityName", Required = Required.Default)]
    public string LegalEntityName { get; set; }

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    /// <value>
    /// The country code.
    /// </value>
    [JsonProperty(propertyName: "countryCode", Required = Required.Default)]
    public string CountryCode { get; set; }

    /// <summary>
    /// Gets or sets the currency.
    /// </summary>
    /// <value>
    /// The currency.
    /// </value>
    [JsonProperty(propertyName: "currency", Required = Required.Default)]
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the currency.
    /// </summary>
    /// <value>
    /// The currency.
    /// </value>
    [JsonProperty(propertyName: "storePaymentMethodMode", Required = Required.Default)]
    public List<string> StorePaymentMethodMode { get; set; } = new List<string>();
  }
}
