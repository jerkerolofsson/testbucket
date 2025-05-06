
$projects = @("tests/TestBucket.Domain.IntegrationTests/TestBucket.Domain.IntegrationTests.csproj", "tests/TestBucket.IntegrationTests/TestBucket.IntegrationTests.csproj")

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
