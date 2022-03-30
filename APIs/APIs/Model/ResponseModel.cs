// <copyright file="ResponseModel.cs" company="PlanB. GmbH">
// Copyright (c) PlanB. GmbH. All rights reserved.
// </copyright>

using Newtonsoft.Json.Linq;

namespace APIs.Model
{
  /// <summary>
  /// The Response Model.
  /// </summary>
  public class ResponseModel
  {
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    /// <value>
    /// The status.
    /// </value>
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the body.
    /// </summary>
    /// <value>
    /// The body.
    /// </value>
    public JArray MerchantAccounts { get; set; }
  }
}
