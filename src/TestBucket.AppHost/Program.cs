var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("testbucket-postgres")
    .WithDataVolume("testbucket-dev", isReadOnly: false); 
var db = postgres.AddDatabase("testbucketdb");

builder.AddProject<Projects.TestBucket>("testbucket")
    .WithReference(db)
    .WithEnvironment("DEFAULT_TENANT", "jerkerolofsson")
    .WithEnvironment("ADMIN_API_KEY", "123456")
    //.WithEnvironment("OLLAMA_BASE_URL", "http://localhost:11434")
    .WithEnvironment("OLLAMA_BASE_URL", "http://127.18.20.51:11434")
    .WaitFor(db);

builder.Build().Run();
