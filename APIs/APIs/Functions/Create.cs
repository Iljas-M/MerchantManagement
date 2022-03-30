// <copyright file="Create.cs" company="PlanB. GmbH">
// Copyright (c) PlanB. GmbH. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

using APIs.Model;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace APIs.Functions
{
  /// <summary>
  /// Create Merchant Account.
  /// </summary>
  public static class Create
  {
    /// <summary>
    /// Creates the merchant.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="container">The container.</param>
    /// <param name="log">The log.</param>
    /// <param name="context">The context.</param>
    /// <returns>The HttpResponseMessage.</returns>
    [FunctionName(nameof(CreateMerchant))]
    public static HttpResponseMessage CreateMerchant(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/merchant")] HttpRequestMessage req,
            [Blob("merchants", FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            ILogger log,
            ExecutionContext context)
    {
      // Init.
      string methodName = context.FunctionName;
      string id = Guid.NewGuid().ToString();
      HttpResponseMessage response = new HttpResponseMessage();
      ResponseModel responseMessage = new ResponseModel();

      try
      {
        log.LogInformation("---------------------------------------------------------------------------------------------");
        log.LogInformation($"'{methodName}' - '{id}' - processed a request.");

        // Read Merchant from FrondEnd.
        MerchantAccountModel merchantAccount = req.Content?.ReadAsAsync<MerchantAccountModel>().Result;

        // Handle if Merchant is null.
        if (merchantAccount == null)
        {
          // Create Response.
          response.StatusCode = HttpStatusCode.BadRequest;
          responseMessage.Status = (int)HttpStatusCode.BadRequest;
          responseMessage.Message = "The Merchent Account cannot be null!";
          response.Content = new StringContent(JsonConvert.SerializeObject(responseMessage), Encoding.UTF8, "application/json");

          return response;
        }

        // Fill the Id.
        merchantAccount.Id = id;

        // If the Blob container doesn't already exist, create new one.
        container.CreateIfNotExistsAsync().Wait();

        log.LogInformation($"'{methodName}' - '{id}' - store to Blob started");

        // Upload Merchant to Blob.
        CloudBlockBlob blob = container.GetBlockBlobReference(merchantAccount.Id);
        blob.UploadTextAsync(JsonConvert.SerializeObject(merchantAccount)).Wait();

        log.LogInformation($"'{methodName}' - '{id}' - store to Blob successful");

        // Create ResponseMessage.
        response.StatusCode = HttpStatusCode.Created;
        responseMessage.Status = (int)HttpStatusCode.Created;
        responseMessage.Message = "New Merchant Successfully Created";
      }
      catch (Exception ex)
      {
        // Handle Exception.
        log.LogInformation($"Exception", ex.Message);
        log.LogInformation($"Exception StackTrace", ex.StackTrace);

        // Set Response.
        response.StatusCode = HttpStatusCode.BadRequest;
        responseMessage.Status = (int)HttpStatusCode.BadRequest;
        responseMessage.Message = $"'{methodName}' - '{id}' - failed \r\n {ex.Message} \r\n {ex.StackTrace}";
      }
      finally
      {
        log.LogInformation($"'{methodName}' - '{id}' - finished");
      }

      log.LogInformation($"'{methodName}' - '{id}' - responseMessage: {JsonConvert.SerializeObject(responseMessage)}");

      // Set the Response Content.
      response.Content = new StringContent(JsonConvert.SerializeObject(responseMessage), Encoding.UTF8, "application/json");

      log.LogInformation("---------------------------------------------------------------------------------------------");
      return response;
    }
  }
}
