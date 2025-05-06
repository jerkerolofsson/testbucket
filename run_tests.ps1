
$projects = @("tests/TestBucket.Formats.UnitTests/TestBucket.Formats.UnitTests.csproj", "tests/TestBucket.Domain.UnitTests/TestBucket.Domain.UnitTests.csproj", "src/Package/Traits/TestBucket.Traits.Core.UnitTests/TestBucket.Traits.Core.UnitTests.csproj")

foreach ($csproj in $projects)
{
	echo "=================================================="
	echo "Testing ${csproj}.."
	$dirName = [System.IO.Path]::GetDirectoryName($csproj)
	$codeCoverageSettingsFile = [System.IO.Path]::Combine($dirName, "codecoverage.xml")

	if (Test-Path $codeCoverageSettingsFile -PathType Leaf) {
		echo "Testing with codecoverage.xml"
		dotnet test $csproj -- --report-xunit --report-xunit-filename xunit.xml --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml --coverage-settings codecoverage.xml
	} else {
		echo "Testing without codecoverage.xml"
		dotnet test $csproj -- --report-xunit --report-xunit-filename xunit.xml --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml
	}
}
