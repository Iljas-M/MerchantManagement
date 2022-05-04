// <copyright file="Create.cs" company="PlanB. GmbH">
// Copyright (c) PlanB. GmbH. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using APIs.Model;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
    /// <param name="blobContainer">The blob container.</param>
    /// <param name="log">The log.</param>
    /// <param name="context">The context.</param>
    /// <returns>The HttpResponseMessage.</returns>
    [FunctionName(nameof(CreateMerchantAsync))]
    public static async Task<HttpResponseMessage> CreateMerchantAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/merchant")] HttpRequestMessage req,
            [Blob("merchants", FileAccess.Write, Connection = "AzureWebJobsStorage")] BlobContainerClient blobContainer,
            ILogger log,
            ExecutionContext context)
    {

      // Param Check.
      if (req is null)
      {
        throw new ArgumentNullException(nameof(req));
      }

      if (blobContainer is null)
      {
        throw new ArgumentNullException(nameof(blobContainer));
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
      string id = Guid.NewGuid().ToString();
      HttpResponseMessage response = new ();
      ResponseModel responseMessage = new ();

      try
      {
        log.LogInformation("---------------------------------------------------------------------------------------------");
        log.LogInformation($"'{methodName}' - '{id}' - Processed");

        // Read Merchant from FrondEnd.
        MerchantAccountModel merchantAccount = req.Content?.ReadAsAsync<MerchantAccountModel>().Result;

        log.LogInformation($"'{methodName}' - '{id}' - RequestBody: \n {JsonConvert.SerializeObject(merchantAccount)}");

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
        Response blobContainerResult = blobContainer.CreateIfNotExistsAsync().Result?.GetRawResponse();

        // Log the Result of creating Blob Container.
        if (blobContainerResult != null)
        {
          log.LogInformation($"'{methodName}' - '{id}' - Container was Successful {blobContainerResult?.ReasonPhrase} - '{blobContainerResult?.Status}'");
        }

        // Upload Merchant to Blob.
        using (MemoryStream memoryStream = new ())
        {
          // Get Blob Client.
          BlobClient blobClient = blobContainer.GetBlobClient(merchantAccount.Id);

          // Read Merchant as Bytes and then Write as Stream.
          await memoryStream.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(merchantAccount)));
          memoryStream.Position = 0;

          // Upload.
          Response containerResult = blobClient?.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = "application/json" }).Result?.GetRawResponse();

          log.LogInformation($"'{methodName}' - '{id}' - Blob was Successful {containerResult?.ReasonPhrase} - '{containerResult?.Status}'");
        }

        log.LogInformation($"'{methodName}' - '{id}' - Blob was Successful Stored");

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
        responseMessage.Message = $"'{methodName}' - '{id}' - Failed \r\n {ex.Message} \r\n {ex.StackTrace}";
      }
      finally
      {
        log.LogInformation($"'{methodName}' - '{id}' - Finished");
      }

      log.LogInformation($"'{methodName}' - '{id}' - ResponseBody: \n {JsonConvert.SerializeObject(responseMessage)}");

      // Set the Response Content.
      response.Content = new StringContent(JsonConvert.SerializeObject(responseMessage), Encoding.UTF8, "application/json");

      log.LogInformation("---------------------------------------------------------------------------------------------");
      return response;
    }
  }
}
