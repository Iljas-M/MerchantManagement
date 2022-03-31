using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Azure.Storage.Blobs;
using APIs.Model;
using System.Text;
using Azure.Storage.Blobs.Models;
using Azure;
using System.Net;

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
    /// <param name="req"></param>
    /// <param name="blobContainer"></param>
    /// <param name="id"></param>
    /// <param name="log"></param>
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

        log.LogInformation($"'{methodName}' - '{id}' - Blob was Successful Stored");
      }
      catch (Exception ex)
      {

        throw;
      }
      finally
      {

      }

      return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
    }
  }
}
