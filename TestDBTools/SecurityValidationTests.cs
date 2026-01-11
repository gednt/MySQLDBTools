using NUnit.Framework;
using System;
using DBTools_Utilities;

namespace TestDBTools
{
    /// <summary>
    /// Tests for security validation methods in Utils class
    /// </summary>
    [TestFixture]
    public class SecurityValidationTests
    {
        [Test]
        public void SelectQuery_WithValidInputs_ReturnsCorrectQuery()
        {
            // Arrange
            string fields = "id,name,email";
            string table = "users";
            string condition = "id = @whereParam0";

            // Act
            string result = Utils.SelectQuery(fields, table, condition);

            // Assert
            Assert.AreEqual("SELECT id,name,email FROM users WHERE id = @whereParam0", result);
        }

        [Test]
        public void SelectQuery_WithNoCondition_ReturnsQueryWithoutWhere()
        {
            // Arrange
            string fields = "id,name";
            string table = "users";
            string condition = "";

            // Act
            string result = Utils.SelectQuery(fields, table, condition);

            // Assert
            Assert.AreEqual("SELECT id,name FROM users", result);
        }

        [Test]
        public void SelectQuery_WithSqlInjectionInTable_ThrowsArgumentException()
        {
            // Arrange
            string fields = "id";
            string table = "users; DROP TABLE users--";
            string condition = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, condition));
        }

        [Test]
        public void SelectQuery_WithSqlInjectionInCondition_ThrowsArgumentException()
        {
            // Arrange
            string fields = "id";
            string table = "users";
            string condition = "id = 1 OR 1=1";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, condition));
            StringAssert.Contains("dangerous SQL pattern", ex.Message);
        }

        [Test]
        public void SelectQuery_WithCommentMarkersInCondition_ThrowsArgumentException()
        {
            // Arrange
            string fields = "id";
            string table = "users";
            string condition = "id = 1 --";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, condition));
            StringAssert.Contains("comment markers", ex.Message);
        }

        [Test]
        public void InsertQuery_WithValidInputs_ReturnsParameterizedQuery()
        {
            // Arrange
            string[] fields = new string[] { "name", "email", "age" };
            string table = "users";
            string[] values = new string[] { "John", "john@example.com", "30" };

            // Act
            string result = Utils.InsertQuery(fields, table, values);

            // Assert
            Assert.AreEqual("INSERT INTO users(name,email,age) VALUES(@param0,@param1,@param2)", result);
        }

        [Test]
        public void InsertQuery_WithInvalidTableName_ThrowsArgumentException()
        {
            // Arrange
            string[] fields = new string[] { "name" };
            string table = "users'; DROP TABLE users--";
            string[] values = new string[] { "John" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.InsertQuery(fields, table, values));
        }

        [Test]
        public void UpdateQuery_WithValidInputs_ReturnsParameterizedQuery()
        {
            // Arrange
            string[] fields = new string[] { "name", "email" };
            string table = "users";
            string[] values = new string[] { "Jane", "jane@example.com" };
            string condition = "id = @whereParam0";

            // Act
            string result = Utils.UpdateQuery(fields, table, values, condition);

            // Assert
            Assert.AreEqual("UPDATE users SET name=@setParam0,email=@setParam1 WHERE id = @whereParam0", result);
        }

        [Test]
        public void UpdateQuery_WithNoCondition_ReturnsQueryWithoutWhere()
        {
            // Arrange
            string[] fields = new string[] { "name" };
            string table = "users";
            string[] values = new string[] { "Jane" };
            string condition = "";

            // Act
            string result = Utils.UpdateQuery(fields, table, values, condition);

            // Assert
            Assert.AreEqual("UPDATE users SET name=@setParam0", result);
        }

        [Test]
        public void DeleteQuery_WithValidInputs_ReturnsCorrectQuery()
        {
            // Arrange
            string table = "users";
            string condition = "id = @whereParam0";

            // Act
            string result = Utils.DeleteQuery(table, condition);

            // Assert
            Assert.AreEqual("DELETE FROM users WHERE id = @whereParam0", result);
        }

        [Test]
        public void DeleteQuery_WithInvalidTableName_ThrowsArgumentException()
        {
            // Arrange
            string table = "users; DROP TABLE users--";
            string condition = "id = 1";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Utils.DeleteQuery(table, condition));
        }

        [Test]
        public void SelectQuery_WithValidBackticksInIdentifiers_Succeeds()
        {
            // Arrange
            string fields = "`user_id`,`user_name`";
            string table = "`users`";
            string condition = "";

            // Act
            string result = Utils.SelectQuery(fields, table, condition);

            // Assert
            Assert.AreEqual("SELECT `user_id`,`user_name` FROM `users`", result);
        }

        [Test]
        public void SelectQuery_WithValidQualifiedNames_Succeeds()
        {
            // Arrange
            string fields = "users.id,users.name";
            string table = "users";
            string condition = "";

            // Act
            string result = Utils.SelectQuery(fields, table, condition);

            // Assert
            Assert.AreEqual("SELECT users.id,users.name FROM users", result);
        }

        [Test]
        public void SelectQuery_WithWildcard_Succeeds()
        {
            // Arrange
            string fields = "*";
            string table = "users";
            string condition = "";

            // Act
            string result = Utils.SelectQuery(fields, table, condition);

            // Assert
            Assert.AreEqual("SELECT * FROM users", result);
        }

        [Test]
        public void SelectQuery_WithNullOrEmptyTableName_ThrowsArgumentException()
        {
            // Arrange
            string fields = "id";
            string table = "";
            string condition = "";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Utils.SelectQuery(fields, table, condition));
            StringAssert.Contains("table", ex.Message.ToLower());
        }

        [Test]
        public void InsertQuery_WithSingleField_ReturnsCorrectQuery()
        {
            // Arrange
            string[] fields = new string[] { "name" };
            string table = "users";
            string[] values = new string[] { "John" };

            // Act
            string result = Utils.InsertQuery(fields, table, values);

            // Assert
            Assert.AreEqual("INSERT INTO users(name) VALUES(@param0)", result);
        }
    }
}
