# Backups

## Backup Format

The default backup format is a zip file.

The zip has items in the following structure

/source/entityType/entityId

..where:
- source is the origin of the data, typically this would correspond to a repository.
- entityType is the type of object, for example a test case, a test suite
- entityId is an identifier unique to the source and entityType. This file contains the serialized entity. 

Typically the file is serialized as JSON, as the DTO, having the same format of data as the API.

