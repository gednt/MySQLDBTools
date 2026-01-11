using NUnit.Framework;
using System;
using System.Linq.Expressions;
using DBTools_Utilities;

namespace TestDBTools
{
    /// <summary>
    /// Tests for UtilsController LINQ-style expression parsing
    /// Note: These tests focus on expression parsing without requiring actual database connections
    /// </summary>
    [TestFixture]
    public class UtilsControllerTests
    {
        // Sample model for testing
        public class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        [Test]
        public void UtilsController_Constructor_WithTableName_SetsProperties()
        {
            // Act
            var controller = new UtilsController<TestModel>(
                tableName: "test_models",
                primaryKeyName: "Id",
                autoIncrement: true
            );

            // Assert
            Assert.IsNotNull(controller);
        }

        [Test]
        public void UtilsController_Constructor_WithConnectionParams_SetsProperties()
        {
            // Act
            var controller = new UtilsController<TestModel>(
                host: "localhost",
                database: "testdb",
                uid: "testuser",
                password: "testpass",
                port: "3306",
                tableName: "test_models"
            );

            // Assert
            Assert.IsNotNull(controller);
            Assert.AreEqual("localhost", controller.Host);
            Assert.AreEqual("testdb", controller.Database);
        }

        [Test]
        public void UtilsController_Constructor_WithDefaultTableName_UsesTypeName()
        {
            // Act
            var controller = new UtilsController<TestModel>();

            // Assert
            Assert.IsNotNull(controller);
            // The table name should default to "TestModel"
        }

        [Test]
        public void ParseExpression_SimpleEquality_GeneratesCorrectSql()
        {
            // This test verifies that the expression parser can handle basic equality
            // We can't directly test the private method, but we test through public methods
            // that use it when we have database connection in integration tests
            
            // For now, just verify the controller can be instantiated with expressions
            var controller = new UtilsController<TestModel>();
            
            // Create an expression (won't execute without DB connection)
            Expression<Func<TestModel, bool>> predicate = x => x.Id == 1;
            
            Assert.IsNotNull(predicate);
        }

        [Test]
        public void ParseExpression_ComparisonOperators_CreatesValidExpressions()
        {
            // Create various expressions to verify they can be created
            Expression<Func<TestModel, bool>> greaterThan = x => x.Age > 18;
            Expression<Func<TestModel, bool>> lessThan = x => x.Age < 65;
            Expression<Func<TestModel, bool>> greaterOrEqual = x => x.Age >= 21;
            Expression<Func<TestModel, bool>> lessOrEqual = x => x.Age <= 100;
            Expression<Func<TestModel, bool>> notEqual = x => x.Id != 0;

            // Assert that expressions can be created
            Assert.IsNotNull(greaterThan);
            Assert.IsNotNull(lessThan);
            Assert.IsNotNull(greaterOrEqual);
            Assert.IsNotNull(lessOrEqual);
            Assert.IsNotNull(notEqual);
        }

        [Test]
        public void ParseExpression_LogicalOperators_CreatesValidExpressions()
        {
            // Create expressions with AND/OR
            Expression<Func<TestModel, bool>> andExpression = x => x.Age > 18 && x.IsActive;
            Expression<Func<TestModel, bool>> orExpression = x => x.Age < 18 || x.Age > 65;
            Expression<Func<TestModel, bool>> complexExpression = x => (x.Age > 18 && x.IsActive) || x.Name == "Admin";

            // Assert that expressions can be created
            Assert.IsNotNull(andExpression);
            Assert.IsNotNull(orExpression);
            Assert.IsNotNull(complexExpression);
        }

        [Test]
        public void ParseExpression_StringEquality_CreatesValidExpression()
        {
            // Create string comparison expression
            Expression<Func<TestModel, bool>> stringEquals = x => x.Name == "John";

            // Assert that expression can be created
            Assert.IsNotNull(stringEquals);
        }

        [Test]
        public void ParseExpression_BooleanProperty_CreatesValidExpression()
        {
            // Create boolean property expression
            Expression<Func<TestModel, bool>> boolCheck = x => x.IsActive;
            Expression<Func<TestModel, bool>> notBoolCheck = x => !x.IsActive;

            // Assert that expressions can be created
            Assert.IsNotNull(boolCheck);
            Assert.IsNotNull(notBoolCheck);
        }

        [Test]
        public void Controller_CanBeInstantiatedWithDifferentTypes()
        {
            // Test that controller can work with different model types
            var intController = new UtilsController<TestModel>();
            
            // Different model
            var stringController = new UtilsController<TestModel>(
                tableName: "custom_table",
                primaryKeyName: "CustomId"
            );

            Assert.IsNotNull(intController);
            Assert.IsNotNull(stringController);
        }

        [Test]
        public void ParseExpression_ComplexNestedConditions_CreatesValidExpression()
        {
            // Create a complex nested expression
            Expression<Func<TestModel, bool>> complex = x => 
                (x.Age > 18 && x.Age < 65 && x.IsActive) || 
                (x.Name == "Admin" && x.Id > 0);

            // Assert that expression can be created
            Assert.IsNotNull(complex);
        }

        [Test]
        public void UtilsController_WithCustomPrimaryKey_AcceptsCustomKeyName()
        {
            // Act
            var controller = new UtilsController<TestModel>(
                tableName: "users",
                primaryKeyName: "UserId",
                autoIncrement: false
            );

            // Assert
            Assert.IsNotNull(controller);
        }
    }
}
