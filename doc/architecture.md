# Architecture Requirements

# 1. Layer Rules

## 1.1 Domain layer should not have dependencies to data or UI

# 2. Security Responsibilities

## 2.1 API/UI controller responsibility
API and UI controllers has the responsibility to identify the user and pass the user information to the domain layer

## 2.2 Domain layer responsibility
- Defines business logic
- Defines specifications for querying
- Entity related classes should use the "Manager" suffix: RequirementManager, FieldDefinitionManager, TestSuiteManager..

### 2.3 Data layer
- Data layer contains repository implementations
- Querying is done with specification pattern

# 3. Naming Conventions
- Use default C# ```editorconfig```
- Classes that do one specific thing should be named with the action

## 3.1 API
- API Controllers may be named after the API + ApiController

## 3.2 UI Controllers
- Feature controllers should be named after the feature, TestBrowserController, TestEditorController
- Entity related classes should be named like EntityController (RequirementController, TestCaseController)

# 4. Features
- Features should be isolated to a folder (may have sub folders) within each layer
- Some features may span both domain and UI, and should have the same naming convention
