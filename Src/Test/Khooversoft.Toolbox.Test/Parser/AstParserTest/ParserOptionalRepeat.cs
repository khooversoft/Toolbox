using FluentAssertions;
using Khooversoft.Toolbox.Parser;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Khooversoft.Toolbox.Test.Parser
{
    [Trait("Category", "Toolbox.Parser")]
    public class ParserOptionalRepeat
    {
        private enum TokenType
        {
            Space,
            Let,
            VariableName,
            VariableType,
            Equal,
            Variable,
            SemiColon,
            Plus,
            Minus,
            Multiple,
            Divide,
            LeftParen,
            RightParent,
        };

        private static class Language
        {
            public static Grammar<TokenType> Space { get; } = new Grammar<TokenType>(TokenType.Space, " ");

            public static Symbol<TokenType> SymLet { get; } = new Symbol<TokenType>(TokenType.Let, "let");
            public static Symbol<TokenType> SymEqual { get; } = new Symbol<TokenType>(TokenType.Equal, "=");
            public static Symbol<TokenType> SymSemiColon { get; } = new Symbol<TokenType>(TokenType.SemiColon, ";");
            public static Symbol<TokenType> SymPlus { get; } = new Symbol<TokenType>(TokenType.Plus, "+");
            public static Symbol<TokenType> SymMinus { get; } = new Symbol<TokenType>(TokenType.Minus, "-");
            public static Symbol<TokenType> SymMultiple { get; } = new Symbol<TokenType>(TokenType.Multiple, "*");
            public static Symbol<TokenType> SymDivide { get; } = new Symbol<TokenType>(TokenType.Divide, "/");
            public static Symbol<TokenType> SymLeftParen { get; } = new Symbol<TokenType>(TokenType.LeftParen, "(");
            public static Symbol<TokenType> SymRightParen { get; } = new Symbol<TokenType>(TokenType.RightParent, ")");

            public static Expression<TokenType> VariableName { get; } = new Expression<TokenType>(TokenType.VariableName);
            public static Expression<TokenType> VariableType { get; } = new Expression<TokenType>(TokenType.VariableType);
            public static Expression<TokenType> Variable { get; } = new Expression<TokenType>(TokenType.Variable);
        }

        private ParserProductionRules<TokenType> _onlyPlusRules;
        private ParserProductionRules<TokenType> _Rules;

        public ParserOptionalRepeat()
        {
            // let {variableName} {variableType} = {variable} [+ {variable}]
            var repeatPlusValue = new Optional() + (new Repeat() + Language.SymPlus + Language.Variable);

            _onlyPlusRules = new ParserProductionRules<TokenType>()
            {
                new RootNode()
                + Language.SymLet
                + Language.VariableName
                + Language.VariableType
                + Language.SymEqual
                + Language.Variable
                + repeatPlusValue
                + Language.SymSemiColon,

                Language.Space,
            };

            // let {variableName} {variableType} = {variable} [(+,-,/,*) {variable}]
            var opsSign = new Choice()
                + Language.SymPlus
                + Language.SymMinus
                + Language.SymMultiple
                + Language.SymDivide;

            var repeatMathValue = new Optional() + (new Repeat() + opsSign + Language.Variable);

            _Rules = new ParserProductionRules<TokenType>()
            {
                new RootNode()
                + Language.SymLet
                + Language.VariableName
                + Language.VariableType
                + Language.SymEqual
                + Language.Variable
                + repeatMathValue
                + Language.SymSemiColon,

                Language.Space,
                new Bracket<TokenType>(Language.SymLeftParen, Language.SymRightParen),
            };
        }

        [Fact]
        public void LetNodeSingleVariableTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "let EmptyLong long? = null;",
            };

            var parser = new LexicalParser<TokenType>(_onlyPlusRules);
            ParserResult result = parser.Parse(string.Join(" ", data));
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            result.RootNode.OfType<Expression<TokenType>>().Single(x => Language.VariableName.Equals(x)).Value.Should().Be("EmptyLong");
            result.RootNode.OfType<Expression<TokenType>>().Single(x => Language.VariableType.Equals(x)).Value.Should().Be("long?");

            var values = result.RootNode.OfType<Expression<TokenType>>()
                .Where(x => Language.Variable.Equals(x));

            values.Count().Should().Be(1);
            values.First().Value.Should().Be("null");
        }

        [Theory]
        [InlineData("EmptyLong long? = null;")]
        [InlineData("let EmptyLong = null;")]
        [InlineData("let EmptyLong long? null;")]
        [InlineData("let EmptyLong long? = null")]
        [InlineData("let EmptyLong long? =;")]
        [InlineData("let long? = null;")]
        public void LetNodeSingleFailTests(string data)
        {
            var parser = new LexicalParser<TokenType>(_onlyPlusRules);
            ParserResult result = parser.Parse(data);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
        }

        [Theory]
        [InlineData("let EmptyLong long? = 5 + 6;", new string[] { "5", "6" })]
        [InlineData("let EmptyLong long? = 5 + 6 + 7;", new string[] { "5", "6", "7" })]
        [InlineData("let EmptyLong long? = 5 + var1 + 7 + 10;", new string[] { "5", "var1", "7", "10" })]
        public void LetMultipleValues(string data, string[] testValues)
        {
            var parser = new LexicalParser<TokenType>(_onlyPlusRules);
            ParserResult result = parser.Parse(data);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            result.RootNode.OfType<Expression<TokenType>>().Single(x => Language.VariableName.Equals(x)).Value.Should().Be("EmptyLong");
            result.RootNode.OfType<Expression<TokenType>>().Single(x => Language.VariableType.Equals(x)).Value.Should().Be("long?");

            var values = result.RootNode.OfType<Expression<TokenType>>()
                .Where(x => Language.Variable.Equals(x));

            values.Count().Should().Be(testValues.Length);

            values.Select(x => x.Value).OrderBy(x => x)
                .Zip(testValues.OrderBy(x => x), (o, i) => new { o, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData("let MyVariable double = 5 * 6;", new string[] { "5", "6" })]
        [InlineData("let MyVariable double = 5 + 6 - 7;", new string[] { "5", "6", "7" })]
        [InlineData("let MyVariable double = 5 + var1 + 7 / 10;", new string[] { "5", "var1", "7", "10" })]
        public void LetDifferentOpsValues(string data, string[] testValues)
        {
            var parser = new LexicalParser<TokenType>(_Rules);
            ParserResult result = parser.Parse(data);
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            result.RootNode.OfType<Expression<TokenType>>().Single(x => Language.VariableName.Equals(x)).Value.Should().Be("MyVariable");
            result.RootNode.OfType<Expression<TokenType>>().Single(x => Language.VariableType.Equals(x)).Value.Should().Be("double");

            var values = result.RootNode.OfType<Expression<TokenType>>()
                .Where(x => Language.Variable.Equals(x));

            values.Count().Should().Be(testValues.Length);

            values.Select(x => x.Value).OrderBy(x => x)
                .Zip(testValues.OrderBy(x => x), (o, i) => new { o, i })
                .All(x => x.o == x.i)
                .Should().BeTrue();
        }
    }
}
