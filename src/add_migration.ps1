param($migrationName)
dotnet ef migrations --project ./TestBucket.Data --startup-project ./TestBucket add $migrationName