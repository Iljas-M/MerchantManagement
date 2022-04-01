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
    /// The coudBlobContainer.
    /// </summary>
    private Mock<CloudBlobContainer> container;

    /// <summary>
    /// The CloudBlockBlob.
    /// </summary>
    private Mock<CloudBlockBlob> blob;

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

      // Create Mock for Blob and Container.
      this.container = new Mock<CloudBlobContainer>(new Uri("http://localhost/container"));
      this.blob = new Mock<CloudBlockBlob>(new Uri("http://localhost/blob"));

      // Setup Mock and Container.
      this.blob.Setup(n => n.UploadTextAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
      this.container.Setup(n => n.GetBlockBlobReference(It.IsAny<string>())).Returns(this.blob.Object);
    }

    /// <summary>
    /// Create Merchant OK Test.
    /// </summary>
    [TestMethod]
    [Ignore]
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
      // HttpResponseMessage result = APIs.Functions.Create.CreateMerchant(this.request, this.container.Object, this.log, this.context);

      // Get the content from result.
     //  ResponseModel content = result.Content.ReadAsAsync<ResponseModel>().Result;

      // Check Response.

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
