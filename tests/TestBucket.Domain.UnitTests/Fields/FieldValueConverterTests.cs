using TestBucket.Domain.Fields;

namespace TestBucket.Domain.UnitTests.Fields
{
    /// <summary>
    /// Unit tests for the <see cref="FieldValueConverter"/> class.
    /// Verifies the behavior of the <see cref="FieldValueConverter.TryAssignValue"/> method for all supported field types.
    /// </summary>
    [UnitTest]
    [Component("Fields")]
    [Feature("Custom Fields")]
    [EnrichedTest]
    [FunctionalTest]
    public class FieldValueConverterTests
    {
        /// <summary>
        /// Verifies that a mismatched DoubleValue property is set to null when assigning a value.
        /// </summary>
        [Fact]
        public void TryAssignValue_DoubleField_MismatchedProperty_IsSetToNull()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "DoubleField", Type = FieldType.Double };
            var fieldValue = new FieldValue { FieldDefinitionId = 1, LongValue = 123 };
            var values = new[] { "123.45" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Null(fieldValue.LongValue);
            Assert.Equal(123.45, fieldValue.DoubleValue);
        }

        /// <summary>
        /// Verifies that a mismatched LongValue property is set to null when assigning a value.
        /// </summary>
        [Fact]
        public void TryAssignValue_IntegerField_MismatchedProperty_IsSetToNull()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "IntegerField", Type = FieldType.Integer };
            var fieldValue = new FieldValue { FieldDefinitionId = 2, DoubleValue = 123.45 };
            var values = new[] { "123" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Null(fieldValue.DoubleValue);
            Assert.Equal(123, fieldValue.LongValue);
        }

        /// <summary>
        /// Verifies that a mismatched DateValue property is set to null when assigning a value.
        /// </summary>
        [Fact]
        public void TryAssignValue_DateOnlyField_MismatchedProperty_IsSetToNull()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "DateOnlyField", Type = FieldType.DateOnly };
            var fieldValue = new FieldValue { FieldDefinitionId = 3, TimeSpanValue = TimeSpan.FromHours(1) };
            var values = new[] { "2023-01-01" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Null(fieldValue.TimeSpanValue);
            Assert.Equal(DateOnly.Parse("2023-01-01"), fieldValue.DateValue);
        }

        /// <summary>
        /// Verifies that a mismatched TimeSpanValue property is set to null when assigning a value.
        /// </summary>
        [Fact]
        public void TryAssignValue_TimeSpanField_MismatchedProperty_IsSetToNull()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "TimeSpanField", Type = FieldType.TimeSpan };
            var fieldValue = new FieldValue { FieldDefinitionId = 4, DateValue = DateOnly.Parse("2023-01-01") };
            var values = new[] { "01:30:00" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Null(fieldValue.DateValue);
            Assert.Equal(TimeSpan.Parse("01:30:00"), fieldValue.TimeSpanValue);
        }

        /// <summary>
        /// Verifies that a mismatched DateTimeOffsetValue property is set to null when assigning a value.
        /// </summary>
        [Fact]
        public void TryAssignValue_DateTimeOffsetField_MismatchedProperty_IsSetToNull()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "DateTimeOffsetField", Type = FieldType.DateTimeOffset };
            var fieldValue = new FieldValue { FieldDefinitionId = 5, BooleanValue = true };
            var values = new[] { "2023-01-01T12:00:00+00:00" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Null(fieldValue.BooleanValue);
            Assert.Equal(DateTimeOffset.Parse("2023-01-01T12:00:00+00:00"), fieldValue.DateTimeOffsetValue);
        }

        /// <summary>
        /// Verifies that a mismatched property in FieldValue is set to null when assigning a value.
        /// </summary>
        [Fact]
        public void TryAssignValue_MismatchedProperty_IsSetToNull()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "BooleanField", Type = FieldType.Boolean };
            var fieldValue = new FieldValue { FieldDefinitionId = 1, StringValue = "abc" };
            var values = new[] { "true" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Null(fieldValue.StringValue);
            Assert.True(fieldValue.BooleanValue);
        }

        /// <summary>
        /// Verifies that TryAssignValue reutrns false if trying to assign a value to a boolean field that already has the value.
        /// </summary>
        [Fact]
        public void TryAssignValue_BooleanField_WithExistingValue_ReturnsFalse()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "BooleanField", Type = FieldType.Boolean };
            var fieldValue = new FieldValue { FieldDefinitionId = 1, BooleanValue = true };
            var values = new[] { "true" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that TryAssignValue reutrns false if trying to assign a value to a string field that already has the value.
        /// </summary>
        [Fact]
        public void TryAssignValue_StringField_WithExistingValue_ReturnsFalse()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "Field", Type = FieldType.String };
            var fieldValue = new FieldValue { FieldDefinitionId = 1, StringValue = "abc" };
            var values = new[] { "abc" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that a boolean field is correctly assigned a valid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_BooleanField_ValidValue_ReturnsTrue()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "BooleanField", Type = FieldType.Boolean };
            var fieldValue = new FieldValue { FieldDefinitionId = 1 };
            var values = new[] { "true" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.True(fieldValue.BooleanValue);
        }

        /// <summary>
        /// Verifies that an integer field is correctly assigned a valid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_IntegerField_ValidValue_ReturnsTrue()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "IntegerField", Type = FieldType.Integer };
            var fieldValue = new FieldValue { FieldDefinitionId = 2 };
            var values = new[] { "123" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Equal(123, fieldValue.LongValue);
        }

        /// <summary>
        /// Verifies that a TimeSpan field is correctly assigned a valid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_TimeSpanField_ValidValue_ReturnsTrue()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "TimeSpanField", Type = FieldType.TimeSpan };
            var fieldValue = new FieldValue { FieldDefinitionId = 3 };
            var values = new[] { "01:30:00" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Equal(TimeSpan.Parse("01:30:00"), fieldValue.TimeSpanValue);
        }

        /// <summary>
        /// Verifies that a DateTimeOffset field is correctly assigned a valid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_DateTimeOffsetField_ValidValue_ReturnsTrue()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "DateTimeOffsetField", Type = FieldType.DateTimeOffset };
            var fieldValue = new FieldValue { FieldDefinitionId = 4 };
            var values = new[] { "2023-01-01T12:00:00+00:00" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Equal(DateTimeOffset.Parse("2023-01-01T12:00:00+00:00"), fieldValue.DateTimeOffsetValue);
        }

        /// <summary>
        /// Verifies that a DateOnly field is correctly assigned a valid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_DateOnlyField_ValidValue_ReturnsTrue()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "DateOnlyField", Type = FieldType.DateOnly };
            var fieldValue = new FieldValue { FieldDefinitionId = 5 };
            var values = new[] { "2023-01-01" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Equal(DateOnly.Parse("2023-01-01"), fieldValue.DateValue);
        }

        /// <summary>
        /// Verifies that a double field is correctly assigned a valid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_DoubleField_ValidValue_ReturnsTrue()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "DoubleField", Type = FieldType.Double };
            var fieldValue = new FieldValue { FieldDefinitionId = 6 };
            var values = new[] { "123.45" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Equal(123.45, fieldValue.DoubleValue);
        }

        /// <summary>
        /// Verifies that a string array field is correctly assigned valid values.
        /// </summary>
        [Fact]
        public void TryAssignValue_StringArrayField_ValidValues_ReturnsTrue()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "StringArrayField", Type = FieldType.StringArray };
            var fieldValue = new FieldValue { FieldDefinitionId = 7 };
            var values = new[] { "value1", "value2" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Equal(values, fieldValue.StringValuesList);
        }

        /// <summary>
        /// Verifies that a string field is correctly assigned a valid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_StringField_ValidValue_ReturnsTrue()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "StringField", Type = FieldType.String };
            var fieldValue = new FieldValue { FieldDefinitionId = 8 };
            var values = new[] { "test" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.True(result);
            Assert.Equal("test", fieldValue.StringValue);
        }

        /// <summary>
        /// Verifies that an integer field is not assigned an invalid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_IntegerField_InvalidValue_ReturnsFalse()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "IntegerField", Type = FieldType.Integer };
            var fieldValue = new FieldValue { FieldDefinitionId = 2 };
            var values = new[] { "invalid" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.False(result);
            Assert.Null(fieldValue.LongValue);
        }

        /// <summary>
        /// Verifies that a TimeSpan field is not assigned an invalid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_TimeSpanField_InvalidValue_ReturnsFalse()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "TimeSpanField", Type = FieldType.TimeSpan };
            var fieldValue = new FieldValue { FieldDefinitionId = 3 };
            var values = new[] { "invalid" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.False(result);
            Assert.Null(fieldValue.TimeSpanValue);
        }

        /// <summary>
        /// Verifies that a DateTimeOffset field is not assigned an invalid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_DateTimeOffsetField_InvalidValue_ReturnsFalse()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "DateTimeOffsetField", Type = FieldType.DateTimeOffset };
            var fieldValue = new FieldValue { FieldDefinitionId = 4 };
            var values = new[] { "invalid" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.False(result);
            Assert.Null(fieldValue.DateTimeOffsetValue);
        }

        /// <summary>
        /// Verifies that a DateOnly field is not assigned an invalid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_DateOnlyField_InvalidValue_ReturnsFalse()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "DateOnlyField", Type = FieldType.DateOnly };
            var fieldValue = new FieldValue { FieldDefinitionId = 5 };
            var values = new[] { "invalid" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.False(result);
            Assert.Null(fieldValue.DateValue);
        }

        /// <summary>
        /// Verifies that a double field is not assigned an invalid value.
        /// </summary>
        [Fact]
        public void TryAssignValue_DoubleField_InvalidValue_ReturnsFalse()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "DoubleField", Type = FieldType.Double };
            var fieldValue = new FieldValue { FieldDefinitionId = 6 };
            var values = new[] { "invalid" };

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.False(result);
            Assert.Null(fieldValue.DoubleValue);
        }

        /// <summary>
        /// Verifies that a string array field is not assigned an empty value array.
        /// </summary>
        [Fact]
        public void TryAssignValue_StringArrayField_EmptyValues_ReturnsFalse()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "StringArrayField", Type = FieldType.StringArray };
            var fieldValue = new FieldValue { FieldDefinitionId = 7 };
            var values = Array.Empty<string>();

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.False(result);
            Assert.Null(fieldValue.StringValuesList);
        }

        /// <summary>
        /// Verifies that a string field is not assigned an empty value.
        /// </summary>
        [Fact]
        public void TryAssignValue_StringField_EmptyValue_ReturnsFalse()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Name = "StringField", Type = FieldType.String };
            var fieldValue = new FieldValue { FieldDefinitionId = 8 };
            var values = Array.Empty<string>();

            // Act
            var result = FieldValueConverter.TryAssignValue(fieldDefinition, fieldValue, values);

            // Assert
            Assert.False(result);
            Assert.Null(fieldValue.StringValue);
        }
    }
}
