# Environments

An environment contains information about the system under test, expressed through variables

## Test case variables and environments

When testing with a selected environment, variables in test cases are read from the environment and shown directly to the user.

### Example for manual testing

Test case:

```
Open the web browser and click on {{APP_URL}}
```

Environment:
```
APP_URL = http://www.app.com
```

The test, as displyed to users, will look like this:

```
Open the web browser and click on http://www.app.com
```

## Automated tests

Variables from the environment will be provided to the automation running as environment variables.

