cd tests/TestBucket.Formats.UnitTests 

dotnet test  --logger "trx;LogFileName=TestBucket.Formats.UnitTests.trx"
dotnet test  --logger "xunit;LogFileName=TestBucket.Formats.UnitTests.xunit.xml"
dotnet test  --logger "junit;LogFileName=TestBucket.Formats.UnitTests.junit.xml"
dotnet test  --logger "nunit;LogFileName=TestBucket.Formats.UnitTests.nunit.xml"

cd ../..
