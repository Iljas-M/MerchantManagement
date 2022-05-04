using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using System.Net.Http;
using APIs.Model;
using Azure;

namespace APIs.Functions
{
  /// <summary>
  /// Delete Merchant Account.
  /// </summary>
  public static class Delete
    {

    /// <summary>
    /// Update Merchant Account.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="blobClient"></param>
    /// <param name="id"></param>
    /// <param name="log"></param>
    /// <returns>The HttpResponseMessage.</returns>
    [FunctionName(nameof(DeleteMerchant))]
    public static async Task<HttpResponseMessage> DeleteMerchant(
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
      HttpResponseMessage response = new();
      ResponseModel responseMessage = new();

      try
      {
        // Delete Blob if Exists.
        var blobResponse = blobClient.DeleteIfExistsAsync().Result?.GetRawResponse();

        log.LogInformation($"'{methodName}' - '{id}' - Blob was Successful {blobResponse?.ReasonPhrase} - '{blobResponse?.Status}'");

        log.LogInformation($"'{methodName}' - '{id}' - Blob was Successful Deleted");
      }
      catch (Exception)
      {

        throw;
      }

      return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
    }

  }
}
