using NUnit.Framework;
using System;
using DBTools_Utilities;
using System.Data;

namespace TestDBTools
{
    /// <summary>
    /// Tests for Utils query builder methods
    /// Note: These tests focus on query building logic without requiring actual database connections
    /// </summary>
    [TestFixture]
    public class UtilsTests
    {
        [Test]
        public void SelectQuery_WithMultipleFields_BuildsCorrectQuery()
        {
            // Arrange
            string fields = "id,name,email,created_at";
            string table = "customers";
            string condition = "status = @whereParam0 AND created_at > @whereParam1";

            // Act
            string result = Utils.SelectQuery(fields, table, condition);

            // Assert
            Assert.That(result, Does.Contain("SELECT id,name,email,created_at FROM customers"));
            Assert.That(result, Does.Contain("WHERE status = @whereParam0 AND created_at > @whereParam1"));
        }

        [Test]
        public void InsertQuery_WithMultipleFields_UsesParameterizedQuery()
        {
            // Arrange
            string[] fields = new string[] { "first_name", "last_name", "email", "phone" };
            string table = "contacts";
            string[] values = new string[] { "John", "Doe", "john.doe@example.com", "123-456-7890" };

            // Act
            string result = Utils.InsertQuery(fields, table, values);

            // Assert
            StringAssert.Contains("INSERT INTO contacts", result);
            StringAssert.Contains("first_name,last_name,email,phone", result);
            StringAssert.Contains("@param0,@param1,@param2,@param3", result);
        }

        [Test]
        public void UpdateQuery_WithMultipleFields_UsesParameterizedQuery()
        {
            // Arrange
            string[] fields = new string[] { "status", "last_modified" };
            string table = "orders";
            string[] values = new string[] { "shipped", "2026-01-11" };
            string condition = "order_id = @whereParam0";

            // Act
            string result = Utils.UpdateQuery(fields, table, values, condition);

            // Assert
            StringAssert.Contains("UPDATE orders", result);
            StringAssert.Contains("status=@setParam0", result);
            StringAssert.Contains("last_modified=@setParam1", result);
            StringAssert.Contains("WHERE order_id = @whereParam0", result);
        }

        [Test]
        public void DeleteQuery_WithCondition_BuildsCorrectQuery()
        {
            // Arrange
            string table = "temp_data";
            string condition = "created_at < @whereParam0";

            // Act
            string result = Utils.DeleteQuery(table, condition);

            // Assert
            Assert.AreEqual("DELETE FROM temp_data WHERE created_at < @whereParam0", result);
        }

        [Test]
        public void SelectQuery_WithEmptyCondition_OmitsWhereClause()
        {
            // Arrange
            string fields = "product_id,product_name,price";
            string table = "products";
            string condition = "";

            // Act
            string result = Utils.SelectQuery(fields, table, condition);

            // Assert
            Assert.That(result, Does.Not.Contain("WHERE"));
            Assert.AreEqual("SELECT product_id,product_name,price FROM products", result);
        }

        [Test]
        public void InsertQuery_VerifyParameterCount_MatchesFieldCount()
        {
            // Arrange
            int fieldCount = 5;
            string[] fields = new string[fieldCount];
            string[] values = new string[fieldCount];
            for (int i = 0; i < fieldCount; i++)
            {
                fields[i] = "field" + i;
                values[i] = "value" + i;
            }
            string table = "test_table";

            // Act
            string result = Utils.InsertQuery(fields, table, values);

            // Assert
            for (int i = 0; i < fieldCount; i++)
            {
                StringAssert.Contains("@param" + i, result);
                StringAssert.Contains("field" + i, result);
            }
        }

        [Test]
        public void UpdateQuery_VerifySetParamPrefix_AvoidsWhereParamConflict()
        {
            // Arrange
            string[] fields = new string[] { "value" };
            string table = "settings";
            string[] values = new string[] { "new_value" };
            string condition = "key = @whereParam0";

            // Act
            string result = Utils.UpdateQuery(fields, table, values, condition);

            // Assert
            StringAssert.Contains("@setParam0", result);
            StringAssert.Contains("@whereParam0", result);
            Assert.That(result, Does.Not.Contain("@param0"));
        }

        [Test]
        public void SelectQuery_WithFieldListContainingSpaces_HandlesCorrectly()
        {
            // Arrange
            string fields = "id, name, email";
            string table = "users";
            string condition = "";

            // Act
            string result = Utils.SelectQuery(fields, table, condition);

            // Assert
            Assert.AreEqual("SELECT id, name, email FROM users", result);
        }

        [Test]
        public void UpdateQuery_WithEmptyCondition_CreatesUpdateWithoutWhere()
        {
            // Arrange
            string[] fields = new string[] { "last_access" };
            string table = "users";
            string[] values = new string[] { "2026-01-11" };
            string condition = "";

            // Act
            string result = Utils.UpdateQuery(fields, table, values, condition);

            // Assert
            Assert.That(result, Does.Not.Contain("WHERE"));
            StringAssert.Contains("UPDATE users SET last_access=@setParam0", result);
        }

        [Test]
        public void InsertQuery_WithSpecialCharactersInFieldNames_HandlesUnderscore()
        {
            // Arrange
            string[] fields = new string[] { "user_id", "first_name", "last_name" };
            string table = "user_profiles";
            string[] values = new string[] { "1", "John", "Doe" };

            // Act
            string result = Utils.InsertQuery(fields, table, values);

            // Assert
            StringAssert.Contains("user_id,first_name,last_name", result);
        }
    }
}
