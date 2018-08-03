using FluentAssertions;
using Khooversoft.Toolbox.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Khooversoft.Toolbox.Test.Parser
{
    [Trait("Category", "Toolbox.Parser")]
    public class StringTokenizerTests
    {
        public StringTokenizerTests()
        {
        }

        [Theory]
        [InlineData("", new string[0])]
        [InlineData("This is a test", new string[] { "This", " ", "is", " ", "a", " ", "test" })]
        [InlineData("This", new string[] { "This" })]
        [InlineData("This ", new string[] { "This", " " })]
        [InlineData("This is a, test", new string[] { "This", " ", "is", " ", "a", ",", " ", "test" })]
        public void SimpleTokenTest(string data, string[] expectedTokens)
        {
            string[] delimiters = new string[] { " ", "," };

            IReadOnlyList<string> tokens = data.Tokenize(delimiters);

            tokens.Count.Should().Be(expectedTokens.Length);
            if( tokens.Count == 0)
            {
                return;
            }

            tokens
                .Zip(expectedTokens, (o, i) => new { o, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("select name, lastName from table", new string[] { "select", " ", "name", ",", " ", "lastName", " ", "from", " ", "table" })]
        [InlineData("this.is.a.test", new string[] { "this.is.a.test" })]
        [InlineData("this.is.a.test from my-table", new string[] { "this.is.a.test", " ", "from", " ", "my-table" })]
        public void ComplexTokenTest(string data, string[] expectedTokens)
        {
            string[] delimiters = new string[] { " ", ",", ";", "select", "from" };

            IReadOnlyList<string> tokens = data.Tokenize(delimiters);

            tokens.Count.Should().Be(expectedTokens.Length);
            if (tokens.Count == 0)
            {
                return;
            }

            tokens
                .Zip(expectedTokens, (o, i) => new { o, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("select name lastName from table", new string[] { "select", " ", "name", " ", "lastName", " ", "from", " ", "table" })]
        [InlineData("select name selectAll from table", new string[] { "select", " ", "name", " ", "selectAll", " ", "from", " ", "table" })]
        [InlineData("let a = b == c;", new string[] { "let", " ", "a", " ", "=", " ", "b", " ", "==", " ", "c;" })]
        [InlineData("let a=b==c;", new string[] { "let", " ", "a", "=", "b", "==", "c;" })]
        [InlineData("selecta=b==c;", new string[] { "select", "a", "=", "b", "==", "c;" })]
        public void ComplexSimilarTokenTest(string data, string[] expectedTokens)
        {
            string[] delimiters = new string[] { " ", "select", "from", "selectAll", "=", "==" };

            IReadOnlyList<string> tokens = data.Tokenize(delimiters);

            tokens.Count.Should().Be(expectedTokens.Length);
            if (tokens.Count == 0)
            {
                return;
            }

            tokens
                .Zip(expectedTokens, (o, i) => new { o, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }


        [Theory]
        [InlineData("SELECT name lastName from table", new string[] { "SELECT", " ", "name", " ", "lastName", " ", "from", " ", "table" })]
        [InlineData("Select name selectAll FrOM table", new string[] { "Select", " ", "name", " ", "selectAll", " ", "FrOM", " ", "table" })]
        [InlineData("Select name SeleCtAll from table", new string[] { "Select", " ", "name", " ", "SeleCtAll", " ", "from", " ", "table" })]
        public void ComplexSimilarCaseInsensitiveTokenTest(string data, string[] expectedTokens)
        {
            string[] delimiters = new string[] { " ", "select", "from", "selectAll", "=", "==" };

            IReadOnlyList<string> tokens = data.Tokenize(delimiters, StringComparison.OrdinalIgnoreCase);

            tokens.Count.Should().Be(expectedTokens.Length);
            if (tokens.Count == 0)
            {
                return;
            }

            tokens
                .Zip(expectedTokens, (o, i) => new { o, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }
    }
}
