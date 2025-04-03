$version="1.0.0"
$package="TestBucket.Traits.Core"

cd src/Traits/${package}
dotnet pack -p:PackageVersion=$version
nuget push bin/Release/${package}.${version}.nupkg  			-Source https://api.nuget.org/v3/index.json
cd ../../..

$package="TestBucket.Traits.Xunit"

cd src/Traits/${package}
dotnet pack -p:PackageVersion=$version
nuget push bin/Release/${package}.${version}.nupkg  			-Source https://api.nuget.org/v3/index.json
cd ../../..

$package="TestBucket.Traits.TUnit"

cd src/Traits/${package}
dotnet pack -p:PackageVersion=$version
nuget push ./bin/Release/${package}.${version}.nupkg  			-Source https://api.nuget.org/v3/index.json
cd ../../..
