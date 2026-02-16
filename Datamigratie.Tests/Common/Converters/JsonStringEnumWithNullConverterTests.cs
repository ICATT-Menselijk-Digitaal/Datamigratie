using System.Text.Json;
using System.Text.Json.Serialization;
using Datamigratie.Common.Converters;

namespace Datamigratie.Tests.Common.Converters
{
    public class JsonStringEnumWithNullConverterTests
    {
        [JsonConverter(typeof(JsonStringEnumWithBlankConverter<TestEnum>))]
        private enum TestEnum
        {
            Blank,
            FirstValue,
            SecondValue
        }

        private class TestModel
        {
            public TestEnum Value { get; set; }
        }

        [Fact]
        public void Serialize_BlankValue_WritesEmptyString()
        {
            var model = new TestModel { Value = TestEnum.Blank };

            var json = JsonSerializer.Serialize(model);

            Assert.Equal("""{"Value":""}""", json);
        }

        [Fact]
        public void Serialize_NonBlankValue_WritesEnumName()
        {
            var model = new TestModel { Value = TestEnum.FirstValue };

            var json = JsonSerializer.Serialize(model);

            Assert.Equal("""{"Value":"FirstValue"}""", json);
        }

        [Fact]
        public void Deserialize_EmptyString_ReturnsBlank()
        {
            var json = """{"Value":""}""";

            var model = JsonSerializer.Deserialize<TestModel>(json);

            Assert.Equal(TestEnum.Blank, model!.Value);
        }

        [Fact]
        public void Deserialize_NullValue_ReturnsBlank()
        {
            var json = """{"Value":null}""";

            var model = JsonSerializer.Deserialize<TestModel>(json);

            Assert.Equal(TestEnum.Blank, model!.Value);
        }

        [Fact]
        public void Deserialize_ValidEnumName_ReturnsEnumValue()
        {
            var json = """{"Value":"SecondValue"}""";

            var model = JsonSerializer.Deserialize<TestModel>(json);

            Assert.Equal(TestEnum.SecondValue, model!.Value);
        }

        [Fact]
        public void Deserialize_UnknownValue_ThrowsJsonException()
        {
            var json = """{"Value":"InvalidValue"}""";

            var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TestModel>(json));

            Assert.Contains("Unknown TestEnum value: InvalidValue", ex.Message);
        }
    }
}
