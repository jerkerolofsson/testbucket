using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using TestBucket.Contracts.Runners.Models;

namespace TestBucket.Runner.Registration
{
    public class TestBucketApiClient(HttpClient Client)
    {
        public async Task<RunRequest?> GetJobAsync(string runnerId, string accessToken)
        {
            var url = $"https+http://testbucket/api/runner/{runnerId}/jobs";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RunRequest>();
        }

        /// <summary>
        /// Registers with the server
        /// </summary>
        /// <param name="connectRequest"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task RegisterAsync(ConnectRequest connectRequest, string accessToken)
        {
            var url = "https+http://testbucket/api/runner/connect";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(JsonSerializer.Serialize(connectRequest), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        internal async Task PostJobResponseAsync(string runnerId, RunResponse runResponse, string accessToken)
        {
            var url = $"https+http://testbucket/api/runner/{runnerId}/jobs/{runResponse.Guid}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(JsonSerializer.Serialize(runResponse), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        internal async Task UploadArtifactAsync(string runnerId, RunResponse runResponse, FileInfo file, string relativePath, string accessToken)
        {
            var bytes = await File.ReadAllBytesAsync(file.FullName);

            var url = $"https+http://testbucket/api/runner/{runnerId}/jobs/{runResponse.Guid}/artifacts&filename={WebUtility.UrlEncode(relativePath)}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new ByteArrayContent(bytes);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.TryAddWithoutValidation("tb-artifact-filename", relativePath);

            using var response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
