
$projects = @("tests/TestBucket.Formats.UnitTests/TestBucket.Formats.UnitTests.csproj", "tests/TestBucket.Blazor.Tests/TestBucket.Blazor.Tests.csproj", "tests/TestBucket.CodeCoverage.Tests/TestBucket.CodeCoverage.Tests.csproj", "tests/TestBucket.Domain.UnitTests/TestBucket.Domain.UnitTests.csproj", "src/Package/Traits/TestBucket.Traits.Core.UnitTests/TestBucket.Traits.Core.UnitTests.csproj")

foreach ($csproj in $projects)
{
	# When running tests from Test Bucket, we add the test suite variables as inputs to the Github workflow
	# and we use this to selectively run automation tests.
	# This allows us to control which tests to run.
	if (-not ([string]::IsNullOrEmpty(${env:TB_TEST_SUITE})))
	{
		if(-not (${csproj}.Contains(${env:TB_TEST_SUITE})))
		{
			echo "Skipping '${csproj}' as it did not contain ${env:TEST_SUITE}.."
			continue;
		}
	}


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
