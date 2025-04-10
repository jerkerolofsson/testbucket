# Automation

## CI/CD integration

Test pipelines can be triggered from within Test Bucket.

### Required Configuration

#### Add an integration to Gitlab, GitHub or other supported CI/CD system.

See [GitLab documentation](gitlab.md)

#### Create a new test suite
- When creating a new test suite, select the integration in the CI/CD field (e.g. Gitlab).

#### Running automated tests
- Right-click on the test suite and select "Run". 
- In the dialog window, enter a reference to the branch/tag/commit to run. 

If the test suite contains manual tests these will be scheduled. 

A new pipeline will be started for automation. 
The CI/CD configuration file should run the test cases.

If the pipeline saves the test results as a job artifact, Test Bucket will download and import the results once the pipeline completes.

The artifacts will be scanned and matched against a "glob-pattern". The pattern can be configured for the integration in project settings.

Example: Any TRX file in any directory
```
**/*.trx
```

Example: Match both XML and TRX files in any directory
```
**/*.trx;**/*.xml
```

Example: Only XML files in the specific directory
```
./TestResults/*.xml
```

Example: Only XML files in the specific directory or any sub directory
```
./TestResults/**/*.xml
```

## Upload files

Alternatively, instead of saving the test results as artifacts the pipeline job can upload the results.

When tests are started from within Test Bucket a short lived access token is created and added as a variable (TB_TOKEN) to the pipeline.
This access token contains information about the run and project and when the results are uploaded they will be added to the correct test run.

### URL for upload

The API endpoint ```/api/results``` imports results and requires a bearer token created for this specific intent.
A personal access token cannot be used as the personal access tokens are not bound to a specific run.

### Media Types

Test Bucket supports various test result formats: junit, xunit, CTRF and will attempt to detect the format when uploading. To guarantee correct results, specify the format using the ContentType header:

| Content Type | Format |
| ------------------------- | -------------------------- | 
| text/xml+xunit            | JUnit                      | 
| text/xml+junit            | xUnit                      | 
| application/json+ctrf     | CTRF                       | 


### Powershell example

```powershell
$path = "./xunit.xml"
$file = Get-ChildItem $path
$data = [System.IO.File]::ReadAllBytes($file.FullName) 
$headers = @{
    "Authorization"  = "Bearer ${env:TB_TOKEN}"
}
Invoke-RestMethod -uri ${env:TB_PUBLIC_ENDPOINT}/api/results -Method PUT -Body $data -ContentType 'text/xml+xunit' -Headers ${headers}
```

