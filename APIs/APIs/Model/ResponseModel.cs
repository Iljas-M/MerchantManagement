// <copyright file="ResponseModel.cs" company="PlanB. GmbH">
// Copyright (c) PlanB. GmbH. All rights reserved.
// </copyright>

using System.Collections.Generic;

using Newtonsoft.Json;

namespace APIs.Model
{
  /// <summary>
  /// The Response Model.
  /// </summary>
  public class ResponseModel
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseModel"/> class.
    /// The ResponseModel.
    /// </summary>
    public ResponseModel()
    {
      this.ExtensionData = new Dictionary<string, object>();
    }

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
    /// Gets or sets the extension data.
    /// </summary>
    /// <value>The extension data.</value>
    [JsonExtensionData]
    public Dictionary<string, object> ExtensionData { get; set; }
  }
}
