using APIs.Model;
using Azure.Storage.Blobs;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Functions.UnitTests
{
  /// <summary>
  /// The Read Api Tests.
  /// </summary>
  [TestClass]
  public class ReadApiTests
  {
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
    /// The CloudBlobDirectory.
    /// </summary>
    private Mock<CloudBlobDirectory> directory;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    [TestInitialize]
    public void Init()
    {
      // Init.
      this.context = new Microsoft.Azure.WebJobs.ExecutionContext() { FunctionName = nameof(ReadApiTests) };
      this.log = Mock.Of<ILogger>();

      // Create Mock for Blob and Container.
      this.container = new Mock<CloudBlobContainer>(new Uri("http://localhost/container"));
      this.blob = new Mock<CloudBlockBlob>(new Uri("http://localhost/blob"));
      this.directory = new Mock<CloudBlobDirectory>();

      // Set Merchant Sample Data.
      string merchant = File.ReadAllText("./Samples/MerchantAccount.json");

      this.blob.Setup(n => n.UploadTextAsync(It.IsAny<string>())).Returns(Task.FromResult(merchant));



      // Create new Blob.
      this.blob.Setup(n => n.UploadTextAsync(merchant));

      // Setup Mock and Container.
      this.container.Setup(n => n.GetBlockBlobReference(It.IsAny<string>())).Returns(this.blob.Object);
      this.container.Setup(n => n.GetDirectoryReference(It.IsAny<string>())).Returns(this.directory.Object);
    }

    /// <summary>
    /// Get Merchant OK Test.
    /// </summary>
    [TestMethod]
    public void ReadMerchantOkTest()
    {




      // Prepare Response.
      ResponseModel response = new ResponseModel()
      {
        Status = (int)HttpStatusCode.OK,
        Message = "The merchant accounts were successfully fetched",
      };

      // Run the Get Function.
      HttpResponseMessage result = APIs.Functions.Read.GetMerchants(this.container.Object, this.log, this.context);

      // Get the content from result.
      ResponseModel content = result.Content.ReadAsAsync<ResponseModel>().Result;

      // Check Response.
      Assert.IsTrue(result.StatusCode == HttpStatusCode.OK);
      Assert.IsTrue(content.Message == response.Message);
      Assert.IsTrue(content.Status == response.Status);
      Assert.IsNotNull(content.MerchantAccounts);

    }
  }
}
