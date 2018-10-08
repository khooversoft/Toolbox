// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Toolbox.Test.Extensions
{
    [Trait("Category", "Toolbox")]
    public class DictionaryPropertyStringTests
    {
        [Fact]
        public void SimplePropertyStringTest()
        {
            var variations = new []
            {
                new {
                    Source = (string)null,
                    Count = 0,
                    Compare = "",
                },
                new {
                    Source = "key=value",
                    Count = 1,
                    Compare = "key=value",
                },
                new {
                    Source = "key= value",
                    Count = 1,
                    Compare = "key=value",
                },
                new {
                    Source = "key = value",
                    Count = 1,
                    Compare = "key=value",
                },
                new {
                    Source = "key1=value1;key2=value2",
                    Count = 2,
                    Compare = "key1=value1;key2=value2",
                },
                new {
                    Source = "key1=value1;key2=value2;key3=value3",
                    Count = 3,
                    Compare = "key1=value1;key2=value2;key3=value3",
                },
            };

            foreach (var test in variations)
            {
                IDictionary<string, string> result = test.Source.ParsePropertyString();
                result.Should().NotBeNull();
                result.Count.Should().Be(test.Count);

                string propertyString = result.ToPropertyString();
                propertyString.Should().Be(test.Compare);
            }
        }
    }
}
