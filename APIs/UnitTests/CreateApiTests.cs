using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Extensions.Logging;
using System.IO;
using Moq;
using Microsoft.Azure.Storage.Blob;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using APIs.Model;
using Azure.Storage.Blobs;

namespace UnitTests
{
  /// <summary>
  /// The Create Api Tests.
  /// </summary>
  [TestClass]
  public class CreateApiTests
  {
    /// <summary>
    /// The correlation identifier.
    /// </summary>
    private string id;
    
    /// <summary>
    /// The context.
    /// </summary>
    private Microsoft.Azure.WebJobs.ExecutionContext context;

    /// <summary>
    /// The log.
    /// </summary>
    private ILogger log;

    /// <summary>
    /// The CloudBlockBlob.
    /// </summary>
    private Mock<BlobContainerClient> blobContainerClient;

    /// <summary>
    /// The httpRequestMessage.
    /// </summary>
    private HttpRequestMessage request;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
      // Init.
      this.id = Guid.NewGuid().ToString();
      this.context = new Microsoft.Azure.WebJobs.ExecutionContext() { FunctionName = nameof(CreateApiTests) };
      this.log = Mock.Of<ILogger>();
      this.request = new HttpRequestMessage();

      // Create Mock for BlobClient.
      this.blobContainerClient = new Mock<BlobContainerClient>();

      // Setup BlobClient Mock.
      this.blobContainerClient.Setup(i => i.AccountName).Returns("merchants");
    }

    /// <summary>
    /// Create Merchant OK Test.
    /// </summary>
    [TestMethod]
    public void CreateMerchantOkTest()
    {
      // Set Merchant Sample Data.
      string merchant = File.ReadAllText("./Samples/MerchantAccount.json");

      // Prepare Request.
      this.request.Content = new StringContent(merchant, Encoding.UTF8, "application/json");
      this.request.Method = HttpMethod.Post;

      // Prepare Response.
      ResponseModel response = new ResponseModel()
      {
        Status = (int)HttpStatusCode.Created,
        Message = "New Merchant Successfully Created",
      };

      // Run the Create Function.
      HttpResponseMessage result = APIs.Functions.Create.CreateMerchantAsync(this.request, this.blobContainerClient.Object, this.log, this.context).Result;

      // Get the content from result.
      ResponseModel content = result.Content.ReadAsAsync<ResponseModel>().Result;

      // Check Response.
      Assert.IsNotNull(result);
      Assert.AreEqual((int)result.StatusCode, content.Status);
    }

    /// <summary>
    /// Create Merchant null Test.
    /// </summary>
    [TestMethod]
    public void CreateMerchantNullTest()
    {
      // Prepare Request.
      this.request.Method = HttpMethod.Post;

      // Prepare Response.
      ResponseModel response = new ResponseModel()
      {
        Status = (int)HttpStatusCode.BadRequest,
        Message = "The Merchent Account cannot be null!",
      };

      // Run the Create Function.
     //  HttpResponseMessage result = APIs.Functions.Create.CreateMerchant(this.request, this.container.Object, this.log, this.context);

      // Get the content from result.

    }

    /// <summary>
    /// Creates the type of the merchant incorrect content.
    /// </summary>
    [TestMethod]
    public void CreateMerchantIncorrectContentType()
    {
      // Read Merchant Sample Data.
      string merchant = File.ReadAllText("./Samples/MerchantAccount.json");

      // Prepare Request.
      this.request.Content = new StringContent(merchant);
      this.request.Method = HttpMethod.Post;

      // Prepare Response.
      ResponseModel response = new ResponseModel()
      {
        Status = (int)HttpStatusCode.BadRequest,
        Message = "New Merchant Successfully Created",
      };

 
    }
  }
}
