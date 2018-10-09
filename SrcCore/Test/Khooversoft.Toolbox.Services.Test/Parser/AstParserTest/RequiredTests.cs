using FluentAssertions;
using Khooversoft.Toolbox.Parser;
using Xunit;

namespace Khooversoft.Toolbox.Services.Test.Parser.AstParserTest
{
    [Trait("Category", "Toolbox.Parser")]
    public class RequiredTests
    {
        private enum TokenType
        {
            Space,
            Let,
            VariableName,
            Equal,
            Variable,
            SemiColon,
            Plus,
        };

        private static class Language
        {
            public static Grammar<TokenType> Space { get; } = new Grammar<TokenType>(TokenType.Space, " ");

            public static Symbol<TokenType> SymLet { get; } = new Symbol<TokenType>(TokenType.Let, "let");
            public static Symbol<TokenType> SymEqual { get; } = new Symbol<TokenType>(TokenType.Equal, "=");
            public static Symbol<TokenType> SymSemiColon { get; } = new Symbol<TokenType>(TokenType.SemiColon, ";");

            public static Expression<TokenType> VariableName { get; } = new Expression<TokenType>(TokenType.VariableName);
            public static Expression<TokenType> Variable { get; } = new Expression<TokenType>(TokenType.Variable);
        }

        private readonly ParserProductionRules<TokenType>[] _Rules;

        private readonly ParserProductionRules<TokenType> _forumalRules;

        public RequiredTests()
        {
            _Rules = new ParserProductionRules<TokenType>[]
            {
                new ParserProductionRules<TokenType>() { new RootNode() + Language.SymLet },
                new ParserProductionRules<TokenType>() { new RootNode() + Language.SymEqual},
                new ParserProductionRules<TokenType>() { new RootNode() + Language.SymLet + Language.SymEqual },
                new ParserProductionRules<TokenType>() { new RootNode() + Language.SymLet + Language.SymEqual + Language.SymSemiColon },
            };

            _forumalRules = new ParserProductionRules<TokenType>()
            {
                new RootNode()
                    + Language.SymLet
                    + Language.VariableName
                    + Language.SymEqual
                    + Language.Variable
                    + Language.SymSemiColon,

                Language.Space,
            };
        }

        [Theory]
        [InlineData("", 0, false)]
        [InlineData("let", 0, true)]
        [InlineData("fail", 0, false)]
        [InlineData("", 1, false)]
        [InlineData("=", 1, true)]
        [InlineData("fail", 1, false)]
        [InlineData("let=", 2, true)]
        [InlineData("let", 2, false)]
        [InlineData("let+", 2, false)]
        [InlineData("let=;", 3, true)]
        [InlineData("let=", 3, false)]
        [InlineData("let;=", 3, false)]
        public void RequiredSequenceTest(string data, int ruleNumber, bool shouldPass)
        {
            var parser = new LexicalParser<TokenType>(_Rules[ruleNumber]);
            ParserResult result = parser.Parse(data);
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(shouldPass);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("let a = 3;", true)]
        [InlineData("fail", false)]
        [InlineData("let variable = number;", true)]
        [InlineData("let variable = number", false)]
        [InlineData("leta = 3;", true)]
        [InlineData("leta=3;", true)]
        [InlineData("leta3;", false)]
        public void ForumlaTest(string data, bool shouldPass)
        {
            var parser = new LexicalParser<TokenType>(_forumalRules);
            ParserResult result = parser.Parse(data);
            result.Should().NotBeNull();
            result.IsSuccess.Should().Be(shouldPass);
        }
    }
}
