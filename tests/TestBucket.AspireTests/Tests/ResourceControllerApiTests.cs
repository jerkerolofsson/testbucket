using System.Text.Json;
using TestBucket.AspireTests.TestProject.Data;
using TestBucket.Traits.TUnit;

namespace TestBucket.AspireTests.TestProject.Tests
{
    [ApiTest()]
    [ClassDataSource<HttpClientDataClass>(Shared = SharedType.PerAssembly)]
    public class ResourceControllerApiTests(HttpClientDataClass httpClientData)
    {
        [Test]
        public async Task GetApiResourceAsync_WithInvalidId_Returns404()
        {
            // Arrange
            var httpClient = httpClientData.HttpClient;

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/resources/-100");
            request.Headers.Add("ApiKey", "123456");

            // Act
            var response = await httpClient.SendAsync(request);
            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        }
    }
}