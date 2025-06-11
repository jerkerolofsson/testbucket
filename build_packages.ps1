$version="1.0.3"
$package="TestBucket.Traits.Core"
cd src/Package/Traits/${package}
dotnet pack -p:PackageVersion=$version
nuget push bin/Release/${package}.${version}.nupkg -Source https://api.nuget.org/v3/index.json
cd ../../../..

$package="TestBucket.Traits.Xunit"
cd src/Package/Traits/${package}
dotnet pack -p:PackageVersion=$version
nuget push bin/Release/${package}.${version}.nupkg -Source https://api.nuget.org/v3/index.json
cd ../../../..

$package="TestBucket.Traits.TUnit"
cd src/Package/Traits/${package}
dotnet pack -p:PackageVersion=$version
nuget push ./bin/Release/${package}.${version}.nupkg -Source https://api.nuget.org/v3/index.json
cd ../../../..

$package="TestBucket.McpTest.Xunit"
cd src/Package/McpTest/${package}
dotnet pack -p:PackageVersion=$version
nuget push ./bin/Release/${package}.${version}.nupkg -Source https://api.nuget.org/v3/index.json
cd ../../../..
