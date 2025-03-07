cd tests/TestBucket.Formats.UnitTests 

echo "start"

dotnet test -- --report-ctrf -report-ctrf-filename TestBucket.Formats.UnitTests.crtf.json --report-junit -report-junit-filename TestBucket.Formats.UnitTests.junit.xml --report-xunit -report-xunit-filename TestBucket.Formats.UnitTests.xunit.xml  --report-nunit --report-nunit-filename TestBucket.Formats.UnitTests.nunit.xml --report-xunit-trx --report-xunit-trx-filename TestBucket.Formats.UnitTests.xunit.trx

#cd ../TestBucket.Domain.UnitTests
#dotnet test  --logger "xunit;LogFileName=TestBucket.Domain.UnitTests.xunit.xml"

#cd ../TestBucket.AspireTests
#dotnet test  --logger "xunit;LogFileName=TestBucket.AspireTests.xunit.xml"

#dotnet test  --logger "trx;LogFileName=TestBucket.Formats.UnitTests.trx"

#dotnet test  --logger "nunit;LogFileName=TestBucket.Formats.UnitTests.nunit.xml"

cd ../..
