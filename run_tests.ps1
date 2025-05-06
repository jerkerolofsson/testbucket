
$projects = @("tests/TestBucket.Formats.UnitTests/TestBucket.Formats.UnitTests.csproj", "tests/TestBucket.Domain.UnitTests/TestBucket.Domain.UnitTests.csproj", "src/Package/Traits/TestBucket.Traits.Core.UnitTests/TestBucket.Traits.Core.UnitTests.csproj")

foreach ($csproj in $projects)
{
	echo "Testing ${csproj}.."
	dotnet test $csproj -- --report-xunit --report-xunit-filename xunit.xml --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml
}
