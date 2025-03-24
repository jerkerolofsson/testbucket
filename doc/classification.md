# Classification

TestBucket uses role based authorization for handling of tenants, teams and projects

| Role           | Description                                                                           |
| -------------- | ------------------------------------------------------------------------------------- |
| SUPERADMIN     | Users in this role can add and delete tenants and manage server settings such as AI integrations |
| ADMIN          | Users in this role can modify create and delete projects, teams for a specific tenant |

## Granular access control

More detailed access control levels are supported within teams and projects.

Members within a project can get read access, or write access to specific types of entities, such as tests, requirements