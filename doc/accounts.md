# Accounts

Test accounts contain credentials or account settings and can be provisioned to test cases to avoid duplicate usage of the same account when running concurrent tests.

When running automated tests, sometimes accounts or credentials to services are needed and concurrency could cause failures.

For example one test case deletes all emails in the inbox while another test case waits to receive an email.

Test bucket can manage these accounts and avoid concurrent usage by locking an account while a test is running.

Another scenario is running tests in different environments, sometimes an account may be applicable in one environment but not in another environment (for example Wi-Fi credentials while running tests in two different physical buildings).

## Account attributes

Accounts have a type identifying the type of service:
- email
- wifi
- spotify

Accounts can have any number of custom fields associated with them.

## Test suites account requirements

Test suites can define required accounts, and tests will only start once the required account is available. 
The account is provisioned as environment variables, making it possible to read the account information no matter what tool is used for automation.

## Account environment variables

Each account will generate several environment variables, each containing a value unique to the account.

The environment variables will be named according to a specific pattern:

```account_{type}_{number}_{key} = {value}```

For example, the first email account will have a prefix ```account_email_0``` and the key-value pairs will depend on the actual contents of the account.

## Wi-Fi account example

A wi-fi account that has the following attributes:
```
type: wifi
ssid: network_name
password: hunter2
```

Will be serialized as environment variables:
```
account_wifi_0_ssid: network_name
account_wifi_0_password: hunter2
```

## Multiple accounts

If a test requires multiple accounts, the number will be incremented by one for each account:
```
account_wifi_0_ssid: network_name
account_wifi_0_password: hunter2

account_wifi_1_ssid: other_network_name
account_wifi_1_password: hunter3
```
