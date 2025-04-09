function Upload-Result-File {
	
	param(
		$project
	)
	$xmlPath="tests/${project}/TestResults/results.xml"
	$trxPath="tests/${project}/TestResults/results.trx"

	$path = ""
	$contentType = ""
	if ([System.IO.File]::Exists($xmlPath)) 
	{
		$path = $xmlPath
		$contentType = "application/json+xunit"
	}
	if ([System.IO.File]::Exists($trxPath)) 
	{
		$path = $trxPath
		$contentType = "application/trx"
	}

	Write-Host "Uploading to ${env:TB_PUBLIC_ENDPOINT}/api/results"

	$file = Get-ChildItem $path
	$data = [System.IO.File]::ReadAllBytes($file.FullName) 
	$headers = @{
	    "Authorization"  = "Bearer ${env:TB_TOKEN}"
	}
	Invoke-RestMethod -uri ${env:TB_PUBLIC_ENDPOINT}/api/results -Method PUT -Body $data -ContentType $$contentType -Headers ${headers}

}

# --report-ctrf -report-ctrf-filename TestBucket.Formats.UnitTests.crtf.json 
# --report-junit -report-junit-filename TestBucket.Formats.UnitTests.junit.xml 
# --report-xunit -report-xunit-filename TestBucket.Formats.UnitTests.xunit.xml  
# --report-nunit --report-nunit-filename TestBucket.Formats.UnitTests.nunit.xml 
# --report-xunit-trx --report-xunit-trx-filename TestBucket.Formats.UnitTests.xunit.trx

$projects = @("TestBucket.Formats.UnitTests", "TestBucket.Domain.UnitTests")

foreach ($project in $projects)
{
	$csproj="tests/${project}/${project}.csproj"
	dotnet test $csproj  -- --report-xunit --report-xunit-filename=results.xml
}

if ( $env:TB_TOKEN -ne $null) 
{
	foreach ($project in $projects)
	{
		Upload-Result-File $project
	}
}
else
{
	echo "TB_TOKEN was not set. Will not upload results"
}