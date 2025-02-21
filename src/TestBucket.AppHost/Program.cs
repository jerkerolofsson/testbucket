var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("testbucket-postgres")
    .WithDataVolume("testbucket-dev", isReadOnly: false); 
var db = postgres.AddDatabase("testbucketdb");

builder.AddProject<Projects.TestBucket>("testbucket")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
