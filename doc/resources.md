# Test Resources

Test resources are external devices or services that are created by "resource servers".

Resources include, but is not limited to:
- Docker containers
- MCP servers
- Android Phones
- Playwright servers

## How to use test resources in a test

Test suites declare what resource are required, the resource will be allocated and locked while the test is running.
If running tests manually, the resource is locked while the single test is executed. If running tests in a CI pipeline, the resource is allocated until the pipeline is completed.

## How to use test resources in AI Chat
To use a test resource in the AI chat: 
- Click the + icon and select "Test resources". 
- Select the resource

Information about the resource will be added to the chat context.

# Configure a resource server

A resource server is configured by specifying environment variables:
- TB_SERVER_UUID. A unique identifier for the resource server.
- TB_INFORM_URL. This is the URL to the test bucket server, for example: https+http://testbucket/api/test-resources
- TB_AUTH_HEADER. The HTTP Authorization header, it should be set to "Bearer <TOKEN>"

## Token / API Keys

A token can be generated in "Settings > API Keys > Add". 

Select "resource-server" as token type.

## Configure public IP / port

If the resource server is running in Docker or behind a reverse proxy, use the 
- TB_PUBLIC_IP to set IP address or hostname
- TB_PUBLIC_PORT to set the public port to use


