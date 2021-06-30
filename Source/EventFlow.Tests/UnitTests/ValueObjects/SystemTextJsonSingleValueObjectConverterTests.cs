// The MIT License (MIT)
// 
// Copyright (c) 2015-2020 Rasmus Mikkelsen
// Copyright (c) 2015-2020 eBay Software Foundation
// https://github.com/eventflow/EventFlow
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using EventFlow.TestHelpers;
using EventFlow.ValueObjects;
using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventFlow.Tests.UnitTests.ValueObjects
{
    [Category(Categories.Unit)]
    public class SystemTextJsonSingleValueObjectConverterTests
    {
        public enum MagicEnum
        {
            Two = 2,
            Zero = 0,
            Three = 3,
            One = 1
        }
        
        [TestCase("test  test", "\"test  test\"")]
        [TestCase("42", "\"42\"")]
        [TestCase("", "\"\"")]
        [TestCase(null, "null")]
        public void StringSerilization(string value, string expectedJson)
        {
            // Arrange
            var stringSvo = new StringSVO(value);

            // Act
            var json = JsonSerializer.Serialize(stringSvo);

            // Assert
            json.Should().Be(expectedJson);
        }

        [TestCase("\"\"", "")]
        [TestCase("\"test\"", "test")]
        public void StringDeserialization(string json, string expectedValue)
        {
            // Act
            var stringSvo = JsonSerializer.Deserialize<StringSVO>(json);

            // Assert
            stringSvo.Value.Should().Be(expectedValue);
        }

        [TestCase(0, "0")]
        [TestCase(42, "42")]
        [TestCase(-1, "-1")]
        public void IntSerialization(int value, string expectedJson)
        {
            // Arrange
            var intSvo = new IntSVO(value);

            // Act
            var json = JsonSerializer.Serialize(intSvo);

            // Assert
            json.Should().Be(expectedJson);
        }

        [TestCase("0", 0)]
        [TestCase("42", 42)]
        [TestCase("-1", -1)]
        public void IntDeserialization(string json, int expectedValue)
        {
            // Act
            var intSvo = JsonSerializer.Deserialize<IntSVO>(json);

            // Assert
            intSvo.Value.Should().Be(expectedValue);
        }

        [TestCase("1", MagicEnum.One)]
        [TestCase("2", MagicEnum.Two)]
        public void EnumDeserilization(string json, MagicEnum expectedValue)
        {
            // Act
            var intSvo = JsonSerializer.Deserialize<EnumSVO>(json);

            // Assert
            intSvo.Value.Should().Be(expectedValue);
        }

        [TestCase(MagicEnum.Zero, "0")]
        [TestCase(MagicEnum.One, "1")]
        [TestCase(MagicEnum.Two, "2")]
        [TestCase(MagicEnum.Three, "3")]
        public void EnumSerialization(int value, string expectedJson)
        {
            // Arrange
            var intSvo = new IntSVO(value);

            // Act
            var json = JsonSerializer.Serialize(intSvo);

            // Assert
            json.Should().Be(expectedJson);
        }
       
        [JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<StringSVO, string>))]
        public class StringSVO : SingleValueObject<string>
        {
            public StringSVO(string value) : base(value) { }
        }

        [JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<IntSVO, int>))]
        public class IntSVO : SingleValueObject<int>
        {
            public IntSVO(int value) : base(value) { }
        }

        [JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<EnumSVO, MagicEnum>))]
        public class EnumSVO : SingleValueObject<MagicEnum>
        {
            public EnumSVO(MagicEnum value) : base(value) { }
        }
    }
}
