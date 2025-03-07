cd tests/TestBucket.AspireTests
dotnet test  --logger "xunit;LogFileName=TestBucket.AspireTests.xunit.xml"

cd ../TestBucket.Formats.UnitTests 

dotnet test  --logger "xunit;LogFileName=TestBucket.Formats.UnitTests.xunit.xml"

cd ../TestBucket.Domain.UnitTests
dotnet test  --logger "xunit;LogFileName=TestBucket.Domain.UnitTests.xunit.xml"

#dotnet test  --logger "trx;LogFileName=TestBucket.Formats.UnitTests.trx"
#dotnet test  --logger "junit;LogFileName=TestBucket.Formats.UnitTests.junit.xml"
#dotnet test  --logger "nunit;LogFileName=TestBucket.Formats.UnitTests.nunit.xml"

cd ../..
