
$projects = @("tests/TestBucket.IntegrationTests/TestBucket.IntegrationTests.csproj")

foreach ($csproj in $projects)
{
	echo "Testing ${csproj}.."
	dotnet test $csproj  -- --report-xunit --report-xunit-filename=xunit.xml
}
