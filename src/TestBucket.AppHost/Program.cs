var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("testbucket-postgres")
    .WithDataVolume("testbucket-dev", isReadOnly: false); 
var db = postgres.AddDatabase("testbucketdb");

var testBucket = builder.AddProject<Projects.TestBucket>("testbucket")
    .WithReference(db)
    .WithEnvironment("DEFAULT_TENANT", "jerkerolofsson")
    .WithEnvironment("ADMIN_API_KEY", "123456")
    .WithEnvironment("OLLAMA_BASE_URL", "http://localhost:11434")
    //.WithEnvironment("OLLAMA_BASE_URL", "http://172.18.70.121:17050")
    //.WithEnvironment("OLLAMA_BASE_URL", "http://172.18.30.118:17335")
    //.WithEnvironment("OLLAMA_BASE_URL", "http://172.18.70.121:17050")
    .WaitFor(db);

builder.AddProject<Projects.TestBucket_Servers_AdbProxy>("testbucket-adbproxy")
    .WithReference(testBucket)
    .WithEnvironment("ADB_PROXY_INFORM_URL", "https+http://testbucket/api/integrations/adb-proxy/inform")
    .WithEnvironment("ADB_PROXY_AUTH_HEADER", "Bearer 123456")
    .WaitFor(testBucket);

builder.Build().Run();
