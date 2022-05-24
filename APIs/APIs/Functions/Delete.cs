// <copyright file="Delete.cs" company="PlanB. GmbH">
// Copyright (c) PlanB. GmbH. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

using APIs.Model;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace APIs.Functions
{
  /// <summary>
  /// Delete Merchant Account.
  /// </summary>
  public static class Delete
    {
    /// <summary>
    /// Delete Merchant Account.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <param name="blobClient">The blob client.</param>
    /// <param name="id">The id.</param>
    /// <param name="log">The log.</param>
    /// <param name="context">The context.</param>
    /// <returns>The HttpResponseMessage.</returns>
    [FunctionName(nameof(DeleteMerchant))]
    public static HttpResponseMessage DeleteMerchant(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "delete/merchant/{id}")] HttpRequestMessage req,
            [Blob("merchants/{id}", FileAccess.Write, Connection = "AzureWebJobsStorage")] BlobClient blobClient,
            string id,
            ILogger log,
            ExecutionContext context)
    {
      // Param Check.
      if (req is null)
      {
        throw new ArgumentNullException(nameof(req));
      }

      if (blobClient is null)
      {
        throw new ArgumentNullException(nameof(blobClient));
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
      HttpResponseMessage response = new ();
      ResponseModel responseMessage = new ();

      try
      {
        // Delete Blob if Exists.
        var blobResponse = blobClient.DeleteIfExistsAsync().Result?.GetRawResponse();

        log.LogInformation($"'{methodName}' - '{id}' - Blob was Successful {blobResponse?.ReasonPhrase} - '{blobResponse?.Status}'");

        log.LogInformation($"'{methodName}' - '{id}' - Blob was Successful Deleted");

        // Create ResponseMessage.
        response.StatusCode = HttpStatusCode.OK;
        responseMessage.Status = (int)HttpStatusCode.OK;
        responseMessage.Message = "Merchant Successfully Deleted";
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
