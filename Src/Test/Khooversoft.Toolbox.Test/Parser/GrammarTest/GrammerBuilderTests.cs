using FluentAssertions;
using Khooversoft.Toolbox.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Khooversoft.Toolbox.Test.Parser
{
    [Trait("Category", "Toolbox.Parser")]
    public class GrammerBuilderTests
    {
        private enum TokenType
        {
            Space,
            Equal,
            LogicalEqual,
            Process,
            Reduce,
            Produce,
            On,
            Comma,
            Using,
            SemiColon,
            Where
        }

        [Fact]
        public void GrammarBuilderTest()
        {
            var builder = new GrammarBuilder<TokenType>()
            {
                new Grammar<TokenType>(TokenType.Space, " "),
                new Grammar<TokenType>(TokenType.Equal, "="),
                new Grammar<TokenType>(TokenType.Process, "PROCESS"),
                new Grammar<TokenType>(TokenType.Produce, "PRODUCE"),
                new Grammar<TokenType>(TokenType.Comma, ","),
                new Grammar<TokenType>(TokenType.Using, "USING"),
                new Grammar<TokenType>(TokenType.SemiColon, ";"),
            };

            builder.Grammar.Count.Should().Be(7);
        }

        [Fact]
        public void ParserReduceGrammarTest()
        {
            Tokenizer<TokenType> parser = CreateParserForReduce();

            var list = new List<string>
            {
                "VdpCountry = REDUCE VdpCountry",
                "             ON CountryId",
                "             PRODUCE CountryId, AdminLevel1, AdminType1, EntityTypeKeyName1",
                "             USING VdpCountryProcessor;",
            };

            string line = string.Join(" ", list);

            var tokenList = new List<IToken>
            {
                new TokenValue("VdpCountry"),
                new Token<TokenType>(TokenType.Equal),
                new Token<TokenType>(TokenType.Reduce),
                new TokenValue("VdpCountry"),
                new Token<TokenType>(TokenType.On),
                new TokenValue("CountryId"),
                new Token<TokenType>(TokenType.Produce),
                new TokenValue("CountryId"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("AdminLevel1"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("AdminType1"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("EntityTypeKeyName1"),
                new Token<TokenType>(TokenType.Using),
                new TokenValue("VdpCountryProcessor"),
                new Token<TokenType>(TokenType.SemiColon),
            };

            List<IToken> parsedTokens = parser.Parse(line).ToList();
            parsedTokens.Count.Should().Be(tokenList.Count);

            parsedTokens
                .Zip(tokenList, (o, i) => new { o, i })
                .All(x => x.o.Equals(x.i))
                .Should().BeTrue();
        }

        [Fact]
        public void ParserReduceCaseInsensitiveGrammarTest()
        {
            Tokenizer<TokenType> parser = CreateParserForReduce(StringComparison.OrdinalIgnoreCase);

            var list = new List<string>
            {
                "VdpCountry = reduce VdpCountry",
                "             ON CountryId",
                "             produce CountryId, AdminLevel1, AdminType1, EntityTypeKeyName1",
                "             USING VdpCountryProcessor;",
            };

            string line = string.Join(" ", list);

            var tokenList = new List<IToken>
            {
                new TokenValue("VdpCountry"),
                new Token<TokenType>(TokenType.Equal),
                new Token<TokenType>(TokenType.Reduce),
                new TokenValue("VdpCountry"),
                new Token<TokenType>(TokenType.On),
                new TokenValue("CountryId"),
                new Token<TokenType>(TokenType.Produce),
                new TokenValue("CountryId"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("AdminLevel1"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("AdminType1"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("EntityTypeKeyName1"),
                new Token<TokenType>(TokenType.Using),
                new TokenValue("VdpCountryProcessor"),
                new Token<TokenType>(TokenType.SemiColon),
            };

            List<IToken> parsedTokens = parser.Parse(line).ToList();
            parsedTokens.Count.Should().Be(tokenList.Count);

            parsedTokens
                .Zip(tokenList, (o, i) => new { o, i })
                .All(x => x.o.Equals(x.i))
                .Should().BeTrue();
        }

        [Fact]
        public void ParserProcessGrammarTest()
        {
            Tokenizer<TokenType> parser = CreateParserForProcess();

            var list = new List<string>
            {
                "AdminEntities = PROCESS Admins",
                "               PRODUCE EntityID, EntityTypeKeyName, ContextID, IsAnonymous, DoNotExport, ISOCode",
                "               USING AdminAreaProcessor;",
            };

            string line = string.Join(" ", list);

            var tokenList = new List<IToken>
            {
                new TokenValue("AdminEntities"),
                new Token<TokenType>(TokenType.Equal),
                new Token<TokenType>(TokenType.Process),
                new TokenValue("Admins"),
                new Token<TokenType>(TokenType.Produce),
                new TokenValue("EntityID"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("EntityTypeKeyName"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("ContextID"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("IsAnonymous"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("DoNotExport"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("ISOCode"),
                new Token<TokenType>(TokenType.Using),
                new TokenValue("AdminAreaProcessor"),
                new Token<TokenType>(TokenType.SemiColon),
            };

            List<IToken> parsedTokens = parser.Parse(line).ToList();
            parsedTokens.Count.Should().Be(tokenList.Count);

            parsedTokens
                .Zip(tokenList, (o, i) => new { o, i })
                .All(x => x.o.Equals(x.i))
                .Should().BeTrue();
        }

        [Fact]
        public void TokenizerProcessLogicalGrammarTest()
        {
            Tokenizer<TokenType> tokenizer = new GrammarBuilder<TokenType>()
            {
                new Grammar<TokenType>(TokenType.Space, " "),
                new Grammar<TokenType>(TokenType.Equal, "="),
                new Grammar<TokenType>(TokenType.LogicalEqual, "=="),
                new Grammar<TokenType>(TokenType.Process, "PROCESS"),
                new Grammar<TokenType>(TokenType.Produce, "PRODUCE"),
                new Grammar<TokenType>(TokenType.Comma, ","),
                new Grammar<TokenType>(TokenType.Using, "USING"),
                new Grammar<TokenType>(TokenType.Where, "WHERE"),
                new Grammar<TokenType>(TokenType.SemiColon, ";"),
            }.Build();

            var list = new List<string>
            {
                "AdminEntities = PROCESS Admins",
                "               PRODUCE EntityID, EntityTypeKeyName",
                "               USING AdminAreaProcessor",
                "               WHERE EntityTypeKeyName == \"value\";",
            };

            string line = string.Join(" ", list);

            var tokenList = new List<IToken>
            {
                new TokenValue("AdminEntities"),
                new Token<TokenType>(TokenType.Equal),
                new Token<TokenType>(TokenType.Process),
                new TokenValue("Admins"),
                new Token<TokenType>(TokenType.Produce),
                new TokenValue("EntityID"),
                new Token<TokenType>(TokenType.Comma),
                new TokenValue("EntityTypeKeyName"),
                new Token<TokenType>(TokenType.Using),
                new TokenValue("AdminAreaProcessor"),
                new Token<TokenType>(TokenType.Where),
                new TokenValue("EntityTypeKeyName"),
                new Token<TokenType>(TokenType.LogicalEqual),
                new TokenValue("\"value\""),
                new Token<TokenType>(TokenType.SemiColon),
            };

            List<IToken> parsedTokens = tokenizer.Parse(line).ToList();
            parsedTokens.Count.Should().Be(tokenList.Count);

            parsedTokens
                .Zip(tokenList, (o, i) => new { o, i })
                .All(x => x.o.Equals(x.i))
                .Should().BeTrue();
        }

        private static Tokenizer<TokenType> CreateParserForProcess()
        {
            var builder = new GrammarBuilder<TokenType>()
            {
                new Grammar<TokenType>(TokenType.Space, " "),
                new Grammar<TokenType>(TokenType.Equal, "="),
                new Grammar<TokenType>(TokenType.Process, "PROCESS"),
                new Grammar<TokenType>(TokenType.Produce, "PRODUCE"),
                new Grammar<TokenType>(TokenType.Comma, ","),
                new Grammar<TokenType>(TokenType.Using, "USING"),
                new Grammar<TokenType>(TokenType.SemiColon, ";"),
            };

            return builder.Build();
        }

        private static Tokenizer<TokenType> CreateParserForReduce(StringComparison stringComparison = StringComparison.Ordinal)
        {
            var builder = new GrammarBuilder<TokenType>(stringComparison)
            {
                new Grammar<TokenType>(TokenType.Space, " "),
                new Grammar<TokenType>(TokenType.Equal, "="),
                new Grammar<TokenType>(TokenType.Reduce, "REDUCE"),
                new Grammar<TokenType>(TokenType.Produce, "PRODUCE"),
                new Grammar<TokenType>(TokenType.On, "ON"),
                new Grammar<TokenType>(TokenType.Comma, ","),
                new Grammar<TokenType>(TokenType.Using, "USING"),
                new Grammar<TokenType>(TokenType.SemiColon, ";"),
            };

            return builder.Build();
        }
    }
}
