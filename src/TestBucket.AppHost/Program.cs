using System.Net;
using System.Security.Claims;

using Aspire.Hosting;

using TestBucket.Contracts.Identity;
using TestBucket.Domain.Identity;

var builder = DistributedApplication.CreateBuilder(args);

// Setup users and keys for seeding
string email = "admin@admin.com";
string tenant = "jerkerolofsson";
string symmetricKey = "01234567890123456789012345678901234567890123456789";
string issuer = "testbucket";
string audience = "testbucket";
var principal = Impersonation.Impersonate(tenant);

// For development, we inject a symmetric key and generate an API key to use so services can communicate
var apiKeyGenerator = new ApiKeyGenerator(symmetricKey, issuer, audience);
string accessToken = apiKeyGenerator.GenerateAccessToken(principal, DateTime.UtcNow.AddDays(100));
string runnerAccessToken = apiKeyGenerator.GenerateAccessToken("runner", principal, DateTime.UtcNow.AddDays(100));

var postgres = builder.AddPostgres("testbucket-postgres")
    //.WithPgAdmin()
    .WithDataVolume("testbucket-dev", isReadOnly: false); 
var db = postgres.AddDatabase("testbucketdb");

//var publicEndpoint = $"http://{Dns.GetHostName()}:5002";
//var publicEndpoint = $"http://192.168.0.241:5002";
var hostname = Dns.GetHostName();
var addresses = Dns.GetHostAddresses(hostname);
var publicEndpoint = $"http://{addresses.Where(x=>x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First()}:5002";

var testBucket = builder.AddProject<Projects.TestBucket>("testbucket")
    .WithReference(db)
    .WithEnvironment("TB_DEFAULT_TENANT", tenant)
    .WithEnvironment("TB_ADMIN_USER", email)
    .WithEnvironment("TB_JWT_SYMMETRIC_KEY", symmetricKey)
    .WithEnvironment("TB_JWT_ISS", issuer)
    .WithEnvironment("TB_JWT_AUD", audience)
    .WithEnvironment("TB_ADMIN_ACCESS_TOKEN", accessToken)
    .WithEnvironment("TB_OLLAMA_BASE_URL", "http://localhost:11434")
    .WithEnvironment("TB_PUBLIC_ENDPOINT", publicEndpoint)
    .WithEnvironment("TB_HTTPS_REDIRECT", "disabled") // Disable HTTPS redirect so we can test without proper certificates
    .WaitFor(db);

builder.AddProject<Projects.TestBucket_Servers_AdbProxy>("testbucket-adbproxy")
    .WithReference(testBucket)
    .WithEnvironment("SERVER_UUID", "11111111")
    .WithEnvironment("ADB_PROXY_INFORM_URL", "https+http://testbucket/api/test-resources")
    .WithEnvironment("ADB_PROXY_AUTH_HEADER", $"Bearer {accessToken}")
    .WaitFor(testBucket);


builder.AddProject<Projects.TestBucket_Runner>("testbucket-runner")
    .WithReference(testBucket)
    .WithEnvironment("TB_ACCESS_TOKEN", runnerAccessToken)
    .WithReference(testBucket)
    .WaitFor(testBucket);

builder.Build().Run();
