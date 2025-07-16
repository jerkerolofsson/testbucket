using System.Net;
using System.Security.Claims;

using Aspire.Hosting;

using TestBucket.Contracts;
using TestBucket.Contracts.Identity;
using TestBucket.Domain.Identity;

var builder = DistributedApplication.CreateBuilder(args);

// Setup users and keys for seeding
string email = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_ADMIN_USER) ?? "admin@admin.com";
string tenant = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_DEFAULT_TENANT) ?? "jerkerolofsson";
string symmetricKey = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_SYMMETRIC_KEY) ?? "01234567890123456789012345678901234567890123456789";
string issuer = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_ISS) ?? "testbucket";
string audience = Environment.GetEnvironmentVariable(TestBucketEnvironmentVariables.TB_JWT_AUD) ?? "testbucket";
var principal = Impersonation.Impersonate(tenant);

bool isTest = false;
if(Environment.GetEnvironmentVariable("TB_IS_INTEGRATION_TEST") == "yes")
{
    isTest = true;
}

// For development, we inject a symmetric key and generate an API key to use so services can communicate
var apiKeyGenerator = new ApiKeyGenerator(symmetricKey, issuer, audience);
string accessToken = apiKeyGenerator.GenerateAccessToken(principal, DateTime.UtcNow.AddDays(100));
string runnerAccessToken = apiKeyGenerator.GenerateAccessToken("runner", principal, DateTime.UtcNow.AddDays(100));

IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("testbucket-postgres")
    .WithImage("ankane/pgvector")
    .WithImageTag("latest");

if (!isTest)
{
    postgres.WithDataVolume("testbucket-dev", isReadOnly: false).WithLifetime(ContainerLifetime.Persistent); 
}
else
{
    //postgres.WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));
}
    var db = postgres.AddDatabase("testbucketdb");

//var publicEndpoint = $"http://{Dns.GetHostName()}:5002";
//var publicEndpoint = $"http://192.168.0.241:5002";
var hostname = Dns.GetHostName();
var addresses = Dns.GetHostAddresses(hostname);
var publicEndpoint = $"http://{addresses.Where(x=>x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First()}:5002";

var testBucket = builder.AddProject<Projects.TestBucket>("testbucket")
    .WithReference(db)
    .WithEnvironment(TestBucketEnvironmentVariables.TB_DEFAULT_TENANT, tenant)
    .WithEnvironment(TestBucketEnvironmentVariables.TB_ADMIN_USER, email)
    .WithEnvironment(TestBucketEnvironmentVariables.TB_JWT_SYMMETRIC_KEY, symmetricKey)
    .WithEnvironment(TestBucketEnvironmentVariables.TB_JWT_ISS, issuer)
    .WithEnvironment(TestBucketEnvironmentVariables.TB_JWT_AUD, audience)
    .WithEnvironment(TestBucketEnvironmentVariables.TB_ADMIN_ACCESS_TOKEN, accessToken)
    .WithEnvironment("TB_OLLAMA_BASE_URL", "http://localhost:11434")
    .WithEnvironment(TestBucketEnvironmentVariables.TB_PUBLIC_ENDPOINT, publicEndpoint)
    .WithEnvironment("TB_HTTPS_REDIRECT", "disabled") // Disable HTTPS redirect so we can test without proper certificates
    .WithHttpHealthCheck("/health")
    .WaitFor(db);

builder.AddProject<Projects.TestBucket_Servers_AdbProxy>("testbucket-adbproxy")
    .WithReference(testBucket)
    .WithEnvironment("SERVER_UUID", "11111111")
    .WithEnvironment("TB_INFORM_URL", "https+http://testbucket/api/test-resources")
    .WithEnvironment("TB_AUTH_HEADER", $"Bearer {accessToken}")
    .WaitFor(testBucket);


builder.AddProject<Projects.TestBucket_Servers_NodeResourceServer>("testbucket-noderesourceserver")
    .WithReference(testBucket)
    .WithEnvironment("SERVER_UUID", "22222222")
    .WithEnvironment("TB_PLAYWRIGHT_INSTANCES", "1")
    .WithEnvironment("TB_INFORM_URL", "https+http://testbucket/api/test-resources")
    .WithEnvironment("TB_AUTH_HEADER", $"Bearer {accessToken}")
    .WaitFor(testBucket);

builder.AddProject<Projects.TestBucket_Runner>("testbucket-runner")
    .WithReference(testBucket)
    .WithEnvironment("TB_ACCESS_TOKEN", runnerAccessToken)
    .WithReference(testBucket)
    .WaitFor(testBucket);

builder.Build().Run();
