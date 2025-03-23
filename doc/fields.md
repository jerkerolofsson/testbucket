# Fields

Fields are user defined values that are associated with an entity (a test, a requirement, a test suite etc..).

## Trait Names

A field may have a trait name. The trait is a standardized name which is used to:
- Identify fields with special meaning
- Map properties and traits when importing tests and test results

The trait name is mostly used behind the scenes, and is not shown to normal users.
It can be set when editing the field definition.

## Targets

A field is defining the usage by targeting one or more entity types:
- Test Case
- Test Run
- Test Case Run
- Requirement

A field may target one or more entity types.

## Field inheritance

Field values can be inherited when new entities are created.

### Test Case Runs

A test case run inherits the field values from:
1. Test Run
2. Test Case

## Special Fields

Some fields have special meaning, and is identified by a Trait Name. 

- Release
- Ticket ID

