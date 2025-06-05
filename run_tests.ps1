 param(
        [Parameter(Mandatory=$false)][string]$testSuite
    )

$srcRootPath = (Get-Location).Path
Write-Host "TestSuite: ${testSuite}"
Write-Host "Path: $srcRootPath"

$projects = @("tests/TestBucket.Formats.UnitTests/TestBucket.Formats.UnitTests.csproj", "tests/TestBucket.Blazor.Tests/TestBucket.Blazor.Tests.csproj", "tests/TestBucket.CodeCoverage.Tests/TestBucket.CodeCoverage.Tests.csproj", "tests/TestBucket.Domain.UnitTests/TestBucket.Domain.UnitTests.csproj", "src/Package/Traits/TestBucket.Traits.Core.UnitTests/TestBucket.Traits.Core.UnitTests.csproj")

foreach ($csproj in $projects)
{
    	# Only run selected test suites, if requested
	if (-not ([string]::IsNullOrEmpty($testSuite)))
	{
		if(-not (${csproj}.Contains($testSuite)))
		{
			echo "Skipping '${csproj}' as it did not contain $testSuite.."
			continue;
		}
	}

	echo "=================================================="
	echo "Testing ${csproj}.."
	$dirName = [System.IO.Path]::GetDirectoryName($csproj)
	$suiteName = [System.IO.Path]::GetFileNameWithoutExtension($csproj)
	$codeCoverageSettingsFile = [System.IO.Path]::Combine($dirName, "codecoverage.xml")
	$codeCoverageReportFile = "${suiteName}.coverage.cobertura.xml"
	$sourceReportFile = "${suiteName}.coverage.cobertura.xml.source.json"
	$xunitReportFile = "${suiteName}.xunit.xml"

	if (Test-Path $codeCoverageSettingsFile -PathType Leaf) {
		echo "Testing with codecoverage.xml"
		dotnet test $csproj -- --report-xunit --report-xunit-filename $xunitReportFile  --coverage --coverage-output-format cobertura --coverage-output $codeCoverageReportFile  --coverage-settings codecoverage.xml
	} else {
		echo "Testing without codecoverage.xml"
		dotnet test $csproj -- --report-xunit --report-xunit-filename $xunitReportFile  --coverage --coverage-output-format cobertura --coverage-output $codeCoverageReportFile 
	}

	# Change the coverage file to contain relative paths instead of absolute
	$fullResultFile = (Get-ChildItem -Recurse $codeCoverageReportFile).FullName

	$filenameReplace1 = "filename=`"$srcRootPath/"
	$filenameReplace2 = "filename=`"$srcRootPath`\"

	$content = Get-Content -Path $fullResultFile  -Raw
	$content = $content.Replace($filenameReplace1 , "filename=`"")
	$content = $content.Replace($filenameReplace2 , "filename=`"")
	$content | Set-Content -Path $fullResultFile 
}
