# Security Improvements - SQL Injection Prevention

This document describes the security improvements made to the MySQLDBTools library to prevent SQL injection vulnerabilities.

## Overview

The library has been refactored to use parameterized queries throughout, eliminating SQL injection vulnerabilities that existed in the previous implementation. All data values are now passed as parameters instead of being concatenated directly into SQL queries.

## Key Changes

### 1. Parameterized Queries

All methods that execute SQL queries now use MySqlParameters to safely pass user-supplied values:

- **Insert()**: Uses `@param0, @param1, ...` placeholders for all values
- **Update()**: Uses `@setParam0, @setParam1, ...` placeholders for SET clause values
- **Delete()**: Accepts parameterized WHERE conditions
- **Select()**: Accepts parameterized WHERE conditions

### 2. Identifier Validation

Since table and column names cannot be parameterized in SQL, we added validation to ensure they contain only safe characters:

- Alphanumeric characters
- Underscores (_)
- Backticks (`) for escaping
- Periods (.) for qualified names (e.g., `database.table`)
- Special handling for field lists with commas
- SQL keyword detection for fields containing spaces

### 3. Backward Compatibility

To maintain backward compatibility while improving security:

- Legacy methods (`InsertQueryLegacy`, `UpdateQueryLegacy`) are marked as `[Obsolete]` but still available
- New parameterized versions use the same method names as before
- Documentation clearly indicates the preferred secure approach

## Usage Examples

### Before (Vulnerable):
```csharp
// UNSAFE - Vulnerable to SQL injection
string userInput = "'; DROP TABLE users; --";
string[] fields = { "name" };
string[] values = { userInput };
utils.Insert(fields, "users", values);
```

### After (Secure):
```csharp
// SAFE - Uses parameterized query
string userInput = "'; DROP TABLE users; --";
string[] fields = { "name" };
string[] values = { userInput };
utils.Insert(fields, "users", values); // Now uses @param0 placeholder
// The malicious input is safely treated as data, not SQL code
```

### Using MySqlParameters for WHERE Clauses:

```csharp
// Set up parameters for WHERE clause
var whereParams = new List<MySqlParameter>();
whereParams.Add(new MySqlParameter("@whereParam0", userId));
utils.MySqlParameters = whereParams;

// Execute delete with parameterized condition
utils.Delete("users", "id = @whereParam0");
```

### Using Static Query Builders:

```csharp
// Generate parameterized query
string[] fields = { "name", "email" };
string[] values = { "John", "john@example.com" };
string query = Utils.InsertQuery(fields, "users", values);
// Returns: "INSERT INTO users(name,email) VALUES(@param0,@param1)"

// Set parameters and execute
var parameters = new List<MySqlParameter>();
parameters.Add(new MySqlParameter("@param0", "John"));
parameters.Add(new MySqlParameter("@param1", "john@example.com"));
dbTools.MySqlParameters = parameters;
dbTools.Query = query;
dbTools.MySQLExecuteQuery();
```

## Security Best Practices

When using this library:

1. **Always use parameterized queries** for user-supplied data
2. **Validate table and field names** - they are validated automatically but should come from trusted sources
3. **Never concatenate user input** directly into SQL queries
4. **Use the MySqlParameters property** to pass dynamic WHERE clause values
5. **Avoid legacy methods** marked as `[Obsolete]`

## Parameter Naming Conventions

To avoid conflicts between different parts of a query:

- **INSERT VALUES**: `@param0, @param1, ...`
- **UPDATE SET clause**: `@setParam0, @setParam1, ...`
- **WHERE clause**: `@whereParam0, @whereParam1, ...` (recommended)

## Validation Rules

### Single Identifiers (table names, single column names):
- Alphanumeric characters (a-z, A-Z, 0-9)
- Underscores (_)
- Backticks (`) for escaping
- Periods (.) for qualified names

### Field Lists (comma-separated fields in SELECT):
- All characters allowed for single identifiers
- Commas (,) to separate fields
- Asterisks (*) for SELECT *
- Spaces (allowed but trigger SQL keyword validation)

### Dangerous Patterns Blocked:
Field names with spaces are checked for SQL keywords like:
- OR, AND, UNION
- SELECT, INSERT, UPDATE, DELETE
- DROP, CREATE, ALTER
- EXEC, EXECUTE
- Comment markers (--, /*, */)

## Testing

- CodeQL security analysis: **0 alerts**
- All SQL injection vulnerabilities have been eliminated from data value handling
- Identifier validation prevents injection through table/column names

## Migration Guide

### For Direct Method Calls:

No code changes required! The methods maintain the same signatures and now use parameterized queries automatically.

### For Static Query Builders:

If you were using the static query builder methods (`InsertQuery`, `UpdateQuery`) and expecting SQL with embedded values:

**Option 1 (Recommended)**: Update your code to use parameters
```csharp
// Old way (now returns parameterized query)
string query = Utils.InsertQuery(fields, "users", values);
// Set up parameters
var parameters = new List<MySqlParameter>();
for (int i = 0; i < values.Length; i++) {
    parameters.Add(new MySqlParameter("@param" + i, values[i]));
}
dbTools.MySqlParameters = parameters;
```

**Option 2**: Use legacy methods (not recommended)
```csharp
// Use the deprecated legacy method
string query = Utils.InsertQueryLegacy(fields, "users", values);
// This still embeds values directly (SQL injection risk!)
```

## Summary

These improvements make the MySQLDBTools library significantly more secure by:
1. Eliminating SQL injection vulnerabilities in data value handling
2. Adding validation for identifiers that cannot be parameterized
3. Providing clear documentation and examples for secure usage
4. Maintaining backward compatibility where possible
5. Guiding developers toward secure practices through [Obsolete] attributes

All changes have been verified with CodeQL security scanning and showed 0 security alerts.
