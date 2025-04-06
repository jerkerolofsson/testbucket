# Gitlab Integration

The Test Bucket Gitlab integration provides the following functionalities:

1. Run pipelines
2. Read milestones
3. Read issues

## Enable Gitlab integration

Gitlab integrations are created by project. 

### Create Gitlab access token

- Open the project in Gitlab
- The project ID is visible on the main project page below the title. Remember it as it is needed later.
- Open Settings -> Access Tokens. 
- Select an expiry date
- Select the "api" scope
- Generate the token and copy it

### Add integration
- Select the team and project in Test Bucket
- Click the … menu in the top right corner
- Select settings
- Click on the project name in the list of settings
- Click Integrations
- Click on [Add Integration] under Gitlab
- Enter the base URL to the GitLab server, for example: https://gitlab.net
- Enter the Gitlab Project ID
- Enter a Gitlab access token.
