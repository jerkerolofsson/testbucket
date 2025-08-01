$version="1.1.0"
$package="TestBucket.Traits.Core"
cd src/Package/Traits/${package}
dotnet pack -p:PackageVersion=$version
nuget push bin/Release/${package}.${version}.nupkg -Source https://api.nuget.org/v3/index.json
cd ../../../..

$package="TestBucket.Traits.MSTest"
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
