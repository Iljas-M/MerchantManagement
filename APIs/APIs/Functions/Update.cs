// <copyright file="Update.cs" company="PlanB. GmbH">
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
  /// Update Merchant Account via Id.
  /// </summary>
  public static class Update
  {
    /// <summary>
    /// Update Merchant Account.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="blobClient">The blob client.</param>
    /// <param name="id">The id.</param>
    /// <param name="log">The log.</param>
    /// <param name="context">The context.</param>
    /// <returns>The HttpResponseMessage.</returns>
    [FunctionName(nameof(UpdateMerchantAsync))]
    public static async Task<HttpResponseMessage> UpdateMerchantAsync(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "update/merchant/{id}")] HttpRequestMessage req,
            [Blob("merchants/{id}", FileAccess.Write, Connection = "AzureWebJobsStorage")] BlobClient blobClient,
            string id,
            ILogger log,
            ExecutionContext context)
    {
      // Init.
      string methodName = context.FunctionName;
      HttpResponseMessage response = new ();
      ResponseModel responseMessage = new ();

      try
      {
        log.LogInformation("---------------------------------------------------------------------------------------------");
        log.LogInformation($"'{methodName}' - '{id}' - Processed");

        // Read Merchant from FrondEnd.
        MerchantAccountModel merchantAccount = req.Content?.ReadAsAsync<MerchantAccountModel>().Result;

        log.LogInformation($"'{methodName}' - '{id}' - RequestBody: \n {JsonConvert.SerializeObject(merchantAccount)}");

        // Fill the Id.
        merchantAccount.Id = id;

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

        // Upload Merchant to Blob.
        using (MemoryStream memoryStream = new ())
        {
          // Read Merchant as Bytes and then Write as Stream.
          await memoryStream.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(merchantAccount)));
          memoryStream.Position = 0;

          // Upload.
          Response containerResult = blobClient.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = "application/json" }).Result?.GetRawResponse();

          log.LogInformation($"'{methodName}' - '{id}' - Blob was Successful {containerResult?.ReasonPhrase} - '{containerResult?.Status}'");
        }

        log.LogInformation($"'{methodName}' - '{id}' - Blob was Successful Updated");

        // Create ResponseMessage.
        response.StatusCode = HttpStatusCode.Accepted;
        responseMessage.Status = (int)HttpStatusCode.Accepted;
        responseMessage.Message = "Merchant Successfully Updated";
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

      // Set the Response Content.
      response.Content = new StringContent(JsonConvert.SerializeObject(responseMessage), Encoding.UTF8, "application/json");

      log.LogInformation("---------------------------------------------------------------------------------------------");
      return response;
    }
  }
}
