// <copyright file="Read.cs" company="PlanB. GmbH">
// Copyright (c) PlanB. GmbH. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using APIs.Model;
using Microsoft.Azure.Storage.Blob;
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
    /// <param name="container">The container.</param>
    /// <param name="log">The log.</param>
    /// <param name="context">The context.</param>
    /// <returns>The HttpResponseMessage.</returns>
    [FunctionName(nameof(GetMerchants))]
    public static HttpResponseMessage GetMerchants(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/merchants")]
            [Blob("merchants", FileAccess.Read, Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            ILogger log,
            ExecutionContext context)
    {
      // Init.
      string methodName = context.FunctionName;
      HttpResponseMessage response = new HttpResponseMessage();
      ResponseModel responseMessage = new ResponseModel();
      JArray merchantAccounts = new JArray();

      try
      {
        log.LogInformation("---------------------------------------------------------------------------------------------");
        log.LogInformation($"'{methodName}' - processed a request.");

        // Get Blob Directory.
        CloudBlobDirectory cloudBlobDirectory = container.GetDirectoryReference(string.Empty);

        // Get Blob Segments from Blob Storage.
        BlobResultSegment blobs = cloudBlobDirectory.ListBlobsSegmentedAsync(
            useFlatBlobListing: false,
            blobListingDetails: BlobListingDetails.None,
            maxResults: null,
            currentToken: null,
            options: null,
            operationContext: null)
         .Result;

        log.LogInformation($"'{methodName}' - started");
        log.LogInformation($"'{methodName}' - '{blobs?.Results.ToList().Count}' - Merchants Are Found");

        // Check if any blobs are found.
        if (blobs != null){

          // Go through all merchants.
          foreach (var blob in blobs?.Results)
          {
            // Download Single Blob.
            CloudBlockBlob cloudBlockBlob = blob as CloudBlockBlob;
            var content = cloudBlockBlob.DownloadTextAsync().Result;

            // Append Merchant.
            merchantAccounts.Add(JObject.Parse(content));

            log.LogInformation($"'{methodName}' - '{JObject.Parse(content)["id"]}' - was successfully merged.");
          }

          // Set ResponseMessage.
          responseMessage.Status = (int)HttpStatusCode.OK;
          responseMessage.Message = "The merchant accounts were successfully fetched";
          responseMessage.MerchantAccounts = merchantAccounts;
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
        responseMessage.Message = $"'{methodName}' - failed \r\n {ex.Message} \r\n {ex.StackTrace}";
      }
      finally
      {
        log.LogInformation($"'{methodName}' - finished");
      }

      log.LogInformation($"'{methodName}' - MerchantAccounts: {responseMessage.MerchantAccounts}");

      // Set the Response Content.
      response.Content = new StringContent(JsonConvert.SerializeObject(responseMessage), Encoding.UTF8, "application/json");

      log.LogInformation("---------------------------------------------------------------------------------------------");
      return response;
    }
  }
}
