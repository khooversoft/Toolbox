using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Parser.Test.AstParserTest
{
    [Trait("Category", "Toolbox.Parser")]
    public class ParserBodyTests
    {
        private enum TokenType
        {
            Space,
            Public,
            Static,
            Colon,
            Class,
            ClassName,
            SubclassName,
            SymLeftBrace,
            SymRightBrace,
        };

        private static class Language
        {
            public static Grammar<TokenType> Space { get; } = new Grammar<TokenType>(TokenType.Space, " ");

            public static Symbol<TokenType> SymPublic { get; } = new Symbol<TokenType>(TokenType.Public, "public");
            public static Symbol<TokenType> SymStatic { get; } = new Symbol<TokenType>(TokenType.Static, "static");
            public static Symbol<TokenType> SymColon { get; } = new Symbol<TokenType>(TokenType.Colon, ":");
            public static Symbol<TokenType> SymLeftBrace { get; } = new Symbol<TokenType>(TokenType.SymLeftBrace, "{");
            public static Symbol<TokenType> SymRightBrace { get; } = new Symbol<TokenType>(TokenType.SymRightBrace, "}");
            public static Symbol<TokenType> SymClass { get; } = new Symbol<TokenType>(TokenType.Class, "class");

            public static Expression<TokenType> ClassName { get; } = new Expression<TokenType>(TokenType.ClassName);
            public static Expression<TokenType> SubclassName { get; } = new Expression<TokenType>(TokenType.SubclassName);
        }

        private AstProductionRules<TokenType> _rules;

        public ParserBodyTests()
        {
            // (public | static) class [: subclass]
            var modifier = new AnyOrder()
                + Language.SymPublic
                + Language.SymStatic;

            var classType = new Choice()
                + modifier
                + Language.SymPublic;

            var optionalSubclass = new Optional()
                + Language.SymColon
                + Language.SubclassName;

            var body = new Body<TokenType>(Language.SymLeftBrace, Language.SymRightBrace, true);

            _rules = new AstProductionRules<TokenType>()
            {
                new AstNode()
                + classType
                + Language.SymClass
                + Language.ClassName
                + optionalSubclass + body,

                Language.Space
            };
        }

        [Fact]
        public void FullClassTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "public static class NameCleaner",
                "{",
                "    public static string Cleanse(string name)",
                "    {",
                "        Dictionary<string, string> changes = new Dictionary<string, string>();",
                "",
                "        changes.Add(\" Twp\", \" Township\");",
                "        changes.Add(\", Town of\", \"\");",
                "        changes.Add(\" Town of\", \"\");",
                "        changes.Add(\", Village of\", \"\");",
                "",
                "        foreach (KeyValuePair<string, string> kvp in changes)",
                "        {",
                "            if (true == name.EndsWith(kvp.Key))",
                "            {",
                "                name = name.Substring(0, name.Length - kvp.Key.Length) + kvp.Value;",
                "            }",
                "        }",
                "",
                "        return name;",
                "    }",
                "}",
            };

            var parser = new AstParser<TokenType>(_rules);
            ParserResult result = parser.Parse(string.Join(" ", data));
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            result.RootNode.OfType<Symbol<TokenType>>().Any(x => Language.SymStatic.Equals(x)).Should().BeTrue();
            result.RootNode.OfType<Expression<TokenType>>().Single(x => Language.ClassName.Equals(x)).Value.Should().Be("NameCleaner");
            result.RootNode.OfType<Expression<TokenType>>().Any(x => Language.SubclassName.Equals(x)).Should().BeFalse();
        }

        [Fact]
        public void GetStaticClassNameTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "static public class NameCleaner",
                "{",
                "    public static string Cleanse(string name)",
                "}",
            };

            var parser = new AstParser<TokenType>(_rules);
            ParserResult result = parser.Parse(string.Join(" ", data));
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            result.RootNode.OfType<Symbol<TokenType>>().Any(x => Language.SymStatic.Equals(x)).Should().BeTrue();
            result.RootNode.OfType<Expression<TokenType>>().Single(x => Language.ClassName.Equals(x)).Value.Should().Be("NameCleaner");
            result.RootNode.OfType<Expression<TokenType>>().Any(x => Language.SubclassName.Equals(x)).Should().BeFalse();
        }

        [Fact]
        public void GetSubClassName()
        {
            IEnumerable<string> data = new List<string>
            {
                "static public class TestClass : BaseClass",
                "{",
                "    public static string Cleanse(string name)",
                "}",
            };

            var parser = new AstParser<TokenType>(_rules);
            ParserResult result = parser.Parse(string.Join(" ", data));
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            result.RootNode.OfType<Symbol<TokenType>>().Any(x => Language.SymStatic.Equals(x)).Should().BeTrue();
            result.RootNode.OfType<Expression<TokenType>>().Single(x => Language.ClassName.Equals(x)).Value.Should().Be("TestClass");
            result.RootNode.OfType<Expression<TokenType>>().Single(x => Language.SubclassName.Equals(x)).Value.Should().Be("BaseClass");
        }

        [Fact]
        public void NoStaticTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "public class TestClass",
                "{",
                "    public static string Cleanse(string name)",
                "}",
            };

            var parser = new AstParser<TokenType>(_rules);
            ParserResult result = parser.Parse(string.Join(" ", data));
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            result.RootNode.OfType<Symbol<TokenType>>().Any(x => Language.SymStatic.Equals(x)).Should().BeFalse();
            result.RootNode.OfType<Expression<TokenType>>().Single(x => Language.ClassName.Equals(x)).Value.Should().Be("TestClass");
            result.RootNode.OfType<Expression<TokenType>>().Any(x => Language.SubclassName.Equals(x)).Should().BeFalse();
        }

        [Fact]
        public void NoClassFailTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "public NameCleaner",
                "{",
                "    public static string Cleanse(string name)",
                "}",
            };

            var parser = new AstParser<TokenType>(_rules);
            ParserResult result = parser.Parse(string.Join(" ", data));
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void NoClassNameFailTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "public class",
                "{",
                "    public static string Cleanse(string name)",
                "}",
            };

            var parser = new AstParser<TokenType>(_rules);
            ParserResult result = parser.Parse(string.Join(" ", data));
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
        }
    }
}
