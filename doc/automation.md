# 1. Automation

## 1.1 CI/CD integration

Test pipelines can be triggered from within Test Bucket to run automated tests.

External CI/CD system is configured in project settings, and a project can have any number of external systems linked.
An external system contains the project ID of the external system, for Gitlab this is the numerical project ID, and for GitHub this is the organization and project name matching the URL in GitHub.

A test suite in TestBucket refers to one of these external systems and can also define a default CI/CD Reference. (For example a default branch). When running automated tests there is an option to change the ref.

The following steps are required to integrate with an external system and configure the test suite:

1. Generate an access token in the external system
2. Add integration for Test Bucket project
3. Select which system a test suite will use, and optionally configure variables for conditional jobs in the pipeline.

### 1.1.1 Generate an access token

TBD

### 1.1.2 Add an integration to Gitlab, GitHub or other supported CI/CD system.

1. Go to Settings / Project / Integrations and add a new integration
2. Configure the external system with suitable project identifiers, URLs and access tokens.

For more information, see [GitLab documentation](gitlab.md),  [Github documentation](github.md)

Note: Access tokens need to have permissions to create pipelines and read artifacts.
## Configure test suite

A test suite is linked to an external system (such as Gitlab or Github).
It may also define variables that are provided to the external system when creating pipelines and this can be used to conditionally execute specific jobs.

### 1.1.3 Creating new test suites

When creating a new test suite, select the integration in the CI/CD field (e.g. Gitlab).

### 1.1.4 Configure existing test suites

1. Select the test suite in the test tree view in the testing module of Test Bucket
2. Scroll down to the automation section and select the CI/CD system from the dropdown.

Note: If no CI/CD system can be found, make sure the integration is enabled.

## 1.2 Configuring CI/CD Pipelines

### 1.2.1 Conditional job execution

Conditionally executing specific jobs in a pipeline by defining environment variables in the TestBucket test suite.

1. Start by defining a variable in the test suite, for example "TEST_SUITE" = "IntegrationTests"
2. Configure the CI/CD pipeline to conditionally execute jobs when this variable is defined. 

##### Gitlab example for conditional job execution
```yaml
integration_tests:
  stage: test

  rules:
    - if: $TEST_SUITE == "IntegrationTests"
  script: 
  - echo "Running integration tests.."

```

### 1.2.1 Test Results

#### Using pipeline job artifacts for test results

If the pipeline saves the test results as a job artifact, Test Bucket will download and import the results once the pipeline completes.
The artifacts will be scanned and matched against a "glob-pattern". 
The pattern can be configured for the integration in project settings.

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

#### Upload files

Alternatively, instead of saving the test results as artifacts the pipeline job can upload the results.

When tests are started from within Test Bucket a short lived access token is created and added as a variable (TB_TOKEN) to the pipeline.
This access token contains information about the run and project and when the results are uploaded they will be added to the correct test run.

#### URL for upload

The API endpoint ```/api/results``` imports results and requires a bearer token created for this specific intent.
A personal access token cannot be used as the personal access tokens are not bound to a specific run.

### Media Types

Test Bucket supports various test result formats: junit, xunit, CTRF and will attempt to detect the format when uploading. To guarantee correct results, specify the format using the ContentType header:

| Content Type | Format |
| ------------------------- | -------------------------- | 
| text/xml+xunit            | JUnit                      | 
| text/xml+junit            | xUnit                      | 
| application/json+ctrf     | CTRF                       | 


#### Powershell example

```powershell
$path = "./xunit.xml"
$file = Get-ChildItem $path
$data = [System.IO.File]::ReadAllBytes($file.FullName) 
$headers = @{
    "Authorization"  = "Bearer ${env:TB_TOKEN}"
}
Invoke-RestMethod -uri ${env:TB_PUBLIC_ENDPOINT}/api/results -Method PUT -Body $data -ContentType 'text/xml+xunit' -Headers ${headers}
```

## 1.3 Running automated tests

- Right-click on the test suite and select "Run". 
- In the dialog window, enter a reference to the branch/tag/commit to run. 

If the test suite contains manual tests these will be scheduled. 
