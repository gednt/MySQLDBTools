# TestDBTools

This is the unit test project for the MySQLDBTools library.

## Test Framework

The project uses **NUnit 3.13.3** as the testing framework with the NUnit3 Test Adapter for Visual Studio integration.

## Test Coverage

The test suite includes the following test classes:

### 1. SecurityValidationTests
Tests for security validation methods that prevent SQL injection:
- Validation of table and field names
- Detection of SQL injection patterns
- Parameterized query validation
- Tests for `SelectQuery`, `InsertQuery`, `UpdateQuery`, and `DeleteQuery` methods

### 2. UtilsTests
Tests for the Utils class query builder methods:
- SELECT query building with various conditions
- INSERT query parameterization
- UPDATE query with SET clause and WHERE conditions
- DELETE query building
- Field and parameter counting
- Query formatting with special characters

### 3. UtilsControllerTests
Tests for the UtilsController<T> LINQ-style operations:
- Constructor tests with various configurations
- Expression parsing for LINQ predicates
- Support for comparison operators (=, !=, <, >, <=, >=)
- Support for logical operators (AND, OR, NOT)
- Complex nested conditions
- Custom primary key configuration

### 4. DataExportTests
Tests for the DataExport CSV functionality:
- CSV generation with column headers and types
- Custom separator support
- DBNull value handling
- Multiple rows and columns
- Optional column/type display

## Running the Tests

### Using Visual Studio
1. Open `DBTools.sln` in Visual Studio
2. Build the solution
3. Open Test Explorer (Test > Test Explorer)
4. Click "Run All" to execute all tests

### Using NUnit Console Runner
```bash
# Install NUnit Console Runner (if not already installed)
nuget install NUnit.ConsoleRunner -Version 3.16.0

# Run tests
.\NUnit.ConsoleRunner.3.16.0\tools\nunit3-console.exe TestDBTools\bin\Debug\TestDBTools.dll
```

### Using dotnet test
If you migrate to .NET Core/.NET 5+:
```bash
dotnet test
```

## Notes

- Most tests are unit tests that don't require a database connection
- They focus on testing query generation, validation logic, and expression parsing
- Integration tests requiring actual database connections should be added separately
- The tests use AAA (Arrange-Act-Assert) pattern for clarity

## Test Organization

- **Test Fixtures**: Each test class is marked with `[TestFixture]`
- **Test Methods**: Individual tests are marked with `[Test]`
- **Naming Convention**: `MethodName_Scenario_ExpectedBehavior`

## Dependencies

- NUnit 3.13.3
- NUnit3TestAdapter 4.2.1
- .NET Framework 4.5
- DBTools project reference
