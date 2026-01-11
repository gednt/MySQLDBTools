using NUnit.Framework;
using System;
using System.Collections.Generic;
using DBTools_Utilities;
using DBTools.Model;

namespace TestDBTools
{
    /// <summary>
    /// Tests for DataExport functionality
    /// </summary>
    [TestFixture]
    public class DataExportTests
    {
        private DataExport _dataExport;

        [SetUp]
        public void SetUp()
        {
            _dataExport = new DataExport();
        }

        [Test]
        public void ToCsv_WithSimpleData_GeneratesCorrectCsv()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new string[] { "id", "name", "age" },
                    types = new string[] { "Int32", "String", "Int32" },
                    values = new object[] { 1, "John Doe", 30 }
                },
                new GenericObject
                {
                    columns = new string[] { "id", "name", "age" },
                    types = new string[] { "Int32", "String", "Int32" },
                    values = new object[] { 2, "Jane Smith", 25 }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: true, showTypes: true);

            // Assert
            StringAssert.Contains("Int32,String,Int32", result);
            StringAssert.Contains("id,name,age", result);
            StringAssert.Contains("1,John Doe,30", result);
            StringAssert.Contains("2,Jane Smith,25", result);
        }

        [Test]
        public void ToCsv_WithShowColumsFalse_DoesNotIncludeColumnNames()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new string[] { "id", "name" },
                    types = new string[] { "Int32", "String" },
                    values = new object[] { 1, "John" }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: false, showTypes: false);

            // Assert
            Assert.That(result, Does.Not.Contain("id,name"));
            Assert.That(result, Does.Not.Contain("Int32,String"));
            StringAssert.Contains("1,John", result);
        }

        [Test]
        public void ToCsv_WithShowTypesFalse_DoesNotIncludeTypes()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new string[] { "id" },
                    types = new string[] { "Int32" },
                    values = new object[] { 1 }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: true, showTypes: false);

            // Assert
            Assert.That(result, Does.Not.Contain("Int32"));
            StringAssert.Contains("id", result);
        }

        [Test]
        public void ToCsv_WithCustomSeparator_UsesCorrectSeparator()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new string[] { "a", "b", "c" },
                    types = new string[] { "String", "String", "String" },
                    values = new object[] { "1", "2", "3" }
                }
            };
            char separator = ';';

            // Act
            string result = _dataExport.ToCsv(genericObjects, separator, showColums: true, showTypes: true);

            // Assert
            StringAssert.Contains("a;b;c", result);
            StringAssert.Contains("1;2;3", result);
        }

        [Test]
        public void ToCsv_WithDBNullValue_HandlesCorrectly()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new string[] { "id", "nullable_field" },
                    types = new string[] { "Int32", "String" },
                    values = new object[] { 1, DBNull.Value }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: false, showTypes: false);

            // Assert
            StringAssert.Contains("1,", result); // DBNull should result in empty string
        }

        [Test]
        public void ToCsv_WithMultipleRows_GeneratesAllRows()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new string[] { "num" },
                    types = new string[] { "Int32" },
                    values = new object[] { 1 }
                },
                new GenericObject
                {
                    columns = new string[] { "num" },
                    types = new string[] { "Int32" },
                    values = new object[] { 2 }
                },
                new GenericObject
                {
                    columns = new string[] { "num" },
                    types = new string[] { "Int32" },
                    values = new object[] { 3 }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: false, showTypes: false);

            // Assert
            StringAssert.Contains("1", result);
            StringAssert.Contains("2", result);
            StringAssert.Contains("3", result);
        }

        [Test]
        public void ToCsv_WithSingleColumn_GeneratesCorrectOutput()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new string[] { "value" },
                    types = new string[] { "String" },
                    values = new object[] { "test" }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, ',', showColums: true, showTypes: true);

            // Assert
            StringAssert.Contains("String", result);
            StringAssert.Contains("value", result);
            StringAssert.Contains("test", result);
        }

        [Test]
        public void ToCsv_WithPipeSeparator_GeneratesCorrectOutput()
        {
            // Arrange
            var genericObjects = new List<GenericObject>
            {
                new GenericObject
                {
                    columns = new string[] { "col1", "col2" },
                    types = new string[] { "String", "String" },
                    values = new object[] { "val1", "val2" }
                }
            };

            // Act
            string result = _dataExport.ToCsv(genericObjects, '|', showColums: true, showTypes: false);

            // Assert
            StringAssert.Contains("col1|col2", result);
            StringAssert.Contains("val1|val2", result);
        }
    }
}
