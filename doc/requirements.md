# Requirements

## Anatomy

Requirements must belong to a specification. They may be placed in a folder.

A folder may have any number of sub folders.

```
Requirement Specification -> Folder -> Folder -> Requirement

Requirement Specification -> Folder -> Requirement

Requirement Specification -> Requirement
```

## Requirement AI Chat

A chat client should support asking about requirements using RAG 

## Fields

Requirements should support custom fields. 
These can be defined in the project settings similar to test case fields.

## Import

### PDF
It is possible to import PDF documents. They will be converted to markdown format.

### Markdown
It is possible to import markdown documents.
For the correct processing the markdown documents should have the .md extension.

#### Fields
Special format to store fields in markdown

TBD

```
Trait: Value
```

or

```
Trait=Value
```

#### External ID
Special format(s) to store requirement IDs in the markdown should be supported

Example
```
[TB-SRS-001]
```

### Excel/CSV
TBD - future version

## Version Handling
TBD - future version

## Approval
TBD - future version

## Requirement Lifecycle

- When deleting a folder all folders and requirements will be deleted.
- When deleting a specification, all requirements and requirement links to tests will be deleted.
- When deleting a requirement all requirement links to tests will be deleted.

## UI

The requirements user interface can be opened by select the "Requirements" tab

### Requirement Editor

#### Coverage

The requirement editor should have the possibility to show how many tests are covering it, and contain links to those test cases.

### Requirement Tree View

The requirement tree view is displayed to the left side when the requirement tab is opened.

#### Deleting requirements

A requirement can be deleted by right-clicking on the requirement and selecting "Delete".

### Link a requirement to a test case

Right-click on a requirement and select "Test Links" and select a test case.
Click OK to create the link
