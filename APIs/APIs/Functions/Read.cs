// <copyright file="Read.cs" company="PlanB. GmbH">
// Copyright (c) PlanB. GmbH. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using APIs.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIs.Functions
{
  /// <summary>
  /// Get Merchant Accounts.
  /// </summary>
  public static class Read
  {
    /// <summary>
    /// Gets the merchants.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="blobs">The Blobs.</param>
    /// <param name="log">The log.</param>
    /// <param name="context">The context.</param>
    /// <returns>The HttpResponseMessage.</returns>
    [FunctionName(nameof(GetMerchants))]
    public static HttpResponseMessage GetMerchants(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/merchants")] HttpRequestMessage req,
            [Blob("merchants", FileAccess.Read, Connection = "AzureWebJobsStorage")] IEnumerable<string> blobs,
            ILogger log,
            ExecutionContext context)
    {
      // Param Check.
      if (req is null)
      {
        throw new ArgumentNullException(nameof(req));
      }

      if (blobs is null)
      {
        throw new ArgumentNullException(nameof(blobs));
      }

      if (log is null)
      {
        throw new ArgumentNullException(nameof(log));
      }

      if (context is null)
      {
        throw new ArgumentNullException(nameof(context));
      }

      // Init.
      string methodName = context.FunctionName;
      var blobsCount = blobs?.ToList().Count;

      HttpResponseMessage response = new ();
      ResponseModel responseMessage = new ();
      JArray merchantAccounts = new ();

      try
      {
        log.LogInformation("---------------------------------------------------------------------------------------------");
        log.LogInformation($"'{methodName}' - Processed");
        log.LogInformation($"'{methodName}' - '{blobsCount}' - Merchants Are Found");

        // Check if any blobs are found.
        if (blobs != null)
        {
          // Go through all merchants.
          foreach (var blob in blobs)
          {
            // Append Merchant.
            merchantAccounts.Add(JObject.Parse(blob));

            log.LogInformation($"'{methodName}' - '{JObject.Parse(blob)["id"]}' - Was Successfully Merged.");
          }

          // Set ResponseMessage.
          responseMessage.Status = (int)HttpStatusCode.OK;
          responseMessage.Message = "The merchant accounts were successfully fetched";
          responseMessage.ExtensionData["MerchantAccounts"] = merchantAccounts;
        }
        else
        {
          // Set Response.
          response.StatusCode = HttpStatusCode.NotFound;
          responseMessage.Status = (int)HttpStatusCode.NotFound;
          responseMessage.Message = "No merchants were found";
        }
      }
      catch (Exception ex)
      {
        // Handle Exception.
        log.LogInformation($"Exception", ex.Message);
        log.LogInformation($"Exception StackTrace", ex.StackTrace);

        // Set Response.
        response.StatusCode = HttpStatusCode.BadRequest;
        responseMessage.Status = (int)HttpStatusCode.BadRequest;
        responseMessage.Message = $"'{methodName}' - Failed \r\n {ex.Message} \r\n {ex.StackTrace}";
      }
      finally
      {
        log.LogInformation($"'{methodName}' - Finished");
      }

      log.LogInformation($"'{methodName}' - MerchantAccounts: \r\n{responseMessage.ExtensionData["MerchantAccounts"]}");

      // Set the Response Content.
      response.Content = new StringContent(JsonConvert.SerializeObject(responseMessage), Encoding.UTF8, "application/json");

      log.LogInformation("---------------------------------------------------------------------------------------------");
      return response;
    }
  }
}
