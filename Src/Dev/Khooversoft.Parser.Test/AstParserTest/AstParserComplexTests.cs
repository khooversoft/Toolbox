using FluentAssertions;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Parser.Test.AstParserTest
{
    [Trait("Category", "Toolbox.Parser")]
    public class AstParserComplexTests
    {
        private enum TokenType
        {
            Space,
            Select,
            Distinct,
            From,
            Left,
            Outer,
            Inner,
            Join,
            As,
            On,
            Equal,
            Comma,
            SemiColon,
            LeftParen,
            RightParent,

            ColumnReference,
            Rowset,
            RefRowset,
            RefRowsetAlias,
            ColumnName,
            JoinReference,
        };

        private static class Language
        {
            public static Grammar<TokenType> Space { get; } = new Grammar<TokenType>(TokenType.Space, " ");

            public static Symbol<TokenType> SymSelect { get; } = new Symbol<TokenType>(TokenType.Select, "SELECT");
            public static Symbol<TokenType> SymDistinct { get; } = new Symbol<TokenType>(TokenType.Distinct, "DISTINCT");
            public static Symbol<TokenType> SymFrom { get; } = new Symbol<TokenType>(TokenType.From, "FROM");
            public static Symbol<TokenType> SymLeft { get; } = new Symbol<TokenType>(TokenType.Left, "LEFT");
            public static Symbol<TokenType> SymJoin { get; } = new Symbol<TokenType>(TokenType.Join, "JOIN");
            public static Symbol<TokenType> SymOuter { get; } = new Symbol<TokenType>(TokenType.Outer, "OUTER");
            public static Symbol<TokenType> SymInner { get; } = new Symbol<TokenType>(TokenType.Inner, "INNER");
            public static Symbol<TokenType> SymAs { get; } = new Symbol<TokenType>(TokenType.As, "AS");
            public static Symbol<TokenType> SymOn { get; } = new Symbol<TokenType>(TokenType.On, "ON");

            public static Symbol<TokenType> SymEqual { get; } = new Symbol<TokenType>(TokenType.Equal, "=");
            public static Symbol<TokenType> SymComma { get; } = new Symbol<TokenType>(TokenType.Comma, ",");
            public static Symbol<TokenType> SymSemiColon { get; } = new Symbol<TokenType>(TokenType.SemiColon, ";");
            public static Symbol<TokenType> SymLeftParen { get; } = new Symbol<TokenType>(TokenType.LeftParen, "(");
            public static Symbol<TokenType> SymRightParen { get; } = new Symbol<TokenType>(TokenType.RightParent, ")");

            public static Expression<TokenType> Rowset { get; } = new Expression<TokenType>(TokenType.Rowset);
            public static Expression<TokenType> RefRowset { get; } = new Expression<TokenType>(TokenType.RefRowset);
            public static Expression<TokenType> RefRowsetAlias { get; } = new Expression<TokenType>(TokenType.RefRowsetAlias);
            public static Expression<TokenType> ColumnName { get; } = new Expression<TokenType>(TokenType.ColumnName);
            public static Expression<TokenType> ColumnReference { get; } = new Expression<TokenType>(TokenType.ColumnReference);
            public static Expression<TokenType> JoinReference { get; } = new Expression<TokenType>(TokenType.JoinReference);
        }

        private AstProductionRules<TokenType> _rules;

        public AstParserComplexTests()
        {
            /// {rowset} = SELECT ([refColumnName AS] {columnName}) [(, {columnName})] FROM {rowset} [{joins} WHERE {where clauses}]";

            var joinKey = new AstNode("joinKey")
                + (new Optional() + Language.SymLeft)
                + (new Optional() + Language.SymOuter)
                + (new Optional() + Language.SymInner)
                + Language.SymJoin;

            var refRowset = new AstNode()
                + Language.RefRowset
                + (new Optional() + Language.RefRowsetAlias);

            var join = new Repeat("join")
                + joinKey
                + refRowset
                + Language.SymOn
                + Language.JoinReference
                + Language.SymEqual
                + Language.JoinReference;

            var optionalAs = new Optional()
                + Language.ColumnReference
                + Language.SymAs;

            var columns = new AstNode()
                + optionalAs
                + Language.ColumnName
                + (new Optional() + (new Repeat() + Language.SymComma + optionalAs + Language.ColumnName));

            _rules = new AstProductionRules<TokenType>()
            {
                new AstNode("main")
                    + (new Optional() + Language.Rowset + Language.SymEqual)
                    + Language.SymSelect
                    + (new Optional() + Language.SymDistinct)
                    + columns
                    + Language.SymFrom
                    + refRowset
                    + (new Optional() + join)
                    + Language.SymSemiColon,

                Language.Space,
                new Bracket<TokenType>(Language.SymLeftParen, Language.SymRightParen)
            };
        }

        [Fact]
        public void SimpleQueryTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "linkIds =",
                "    SELECT link_id",
                "    FROM LinkTable;",
            };

            var result = new QueryNode(
                "linkIds",
                new Rowset[] { new Rowset("LinkTable", null) },
                new ColumnName[] { new ColumnName("link_id", null) }
                );

            RunTest(data, result);
        }

        [Fact]
        public void ImplicitRowsetSimpleQueryTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "SELECT link_id",
                "FROM LinkTable;",
            };

            var result = new QueryNode(
                null,
                new Rowset[] { new Rowset("LinkTable", null) },
                new ColumnName[] { new ColumnName("link_id", null) }
                );

            RunTest(data, result);
        }

        [Fact]
        public void SimpleMultipleColumnsTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "linkIds =",
                "    SELECT link_id, first, second, third",
                "    FROM LinkTable;",
            };

            var result = new QueryNode(
                "linkIds",
                new Rowset[] { new Rowset("LinkTable", null) },
                new ColumnName[] { new ColumnName("link_id", null), new ColumnName("first", null), new ColumnName("second", null), new ColumnName("third", null) }
                );

            RunTest(data, result);
        }

        [Fact]
        public void SimpleAsSingleolumnsTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "VdpCountry =",
                "    SELECT  country_id AS CountryId",
                "    FROM    RdfCountryOrig;",
            };

            var result = new QueryNode(
                "VdpCountry",
                new Rowset[] { new Rowset("RdfCountryOrig", null) },
                new ColumnName[] { new ColumnName("CountryId", "country_id") }
                );

            RunTest(data, result);
        }

        [Fact]
        public void SimpleAsMultipleColumnsTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "VdpCountry =",
                "    SELECT  country_id AS CountryId,",
                "                1 AS AdminLevel,",
                "                admin_level_admin_type_1 AS AdminType,",
                "                EntityType.CountryRegion.ToString() AS EntityTypeKeyName,",
                "                iso_country_code,",
                "                max_admin_level",
                "    FROM    RdfCountryOrig;",
            };

            var result = new QueryNode(
                "VdpCountry",
                new Rowset[] { new Rowset("RdfCountryOrig", null) },
                new ColumnName[] {
                    new ColumnName("CountryId", "country_id"),
                    new ColumnName("AdminLevel", "1"),
                    new ColumnName("AdminType", "admin_level_admin_type_1"),
                    new ColumnName("EntityTypeKeyName", "EntityType.CountryRegion.ToString"),
                    new ColumnName("iso_country_code", null),
                    new ColumnName("max_admin_level", null)
                    }
                );

            RunTest(data, result);
        }

        [Fact]
        public void SimpleAliasTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "SELECT link_id",
                "FROM LinkTable a;",
            };

            var result = new QueryNode(
                null,
                new Rowset[] { new Rowset("LinkTable", "a") },
                new ColumnName[] { new ColumnName("link_id", null) }
                );

            RunTest(data, result);
        }

        [Fact]
        public void SimpleJoinNoAliasTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "SELECT Orders.OrderID, Customers.CustomerName, Orders.OrderDate",
                "FROM Orders",
                "INNER JOIN Customers ON Orders.CustomerID=Customers.CustomerID;",
            };

            var result = new QueryNode(
                null,
                new Rowset[] { new Rowset("Orders", null), new Rowset("Customers", null) },
                new ColumnName[] { new ColumnName("Orders.OrderID", null), new ColumnName("Customers.CustomerName", null), new ColumnName("Orders.OrderDate", null) },
                new Join[] { new Join("Orders.CustomerID", "Customers.CustomerID") }
                );

            RunTest(data, result);
        }

        [Fact]
        public void SimpleJoinAliasTest()
        {
            IEnumerable<string> data = new List<string>
            {
                "SELECT Orders.OrderID, Customers.CustomerName, Orders.OrderDate",
                "FROM Orders o",
                "INNER JOIN Customers c ON o.CustomerID=c.CustomerID;",
            };

            var result = new QueryNode(
                null,
                new Rowset[] { new Rowset("Orders", "o"), new Rowset("Customers", "c") },
                new ColumnName[] { new ColumnName("Orders.OrderID", null), new ColumnName("Customers.CustomerName", null), new ColumnName("Orders.OrderDate", null) },
                new Join[] { new Join("o.CustomerID", "c.CustomerID") }
                );

            RunTest(data, result);
        }

        private void RunTest(IEnumerable<string> data, QueryNode result)
        {
            QueryNode codeNode = ParseData(data, result != null);
            if (codeNode == null)
            {
                return;
            }

            result.Should().NotBeNull();

            codeNode.RowsetName.Should().Be(result.RowsetName);

            codeNode.RowsetReferences.Count.Should().Be(result.RowsetReferences.Count);
            codeNode.RowsetReferences
                .OrderBy(x => x.Name)
                .Zip(result.RowsetReferences.OrderBy(x => x.Name), (o, i) => new { o, i })
                .All(x => x.o.Equals(x.i))
                .Should().BeTrue("rowset references does not match");

            codeNode.ColumnNames.Count.Should().Be(result.ColumnNames.Count);
            codeNode.ColumnNames
                .OrderBy(x => x.Name)
                .Zip(result.ColumnNames.OrderBy(x => x.Name), (o, i) => new { o, i })
                .All(x => x.o.Equals(x.i))
                .Should().BeTrue("column names does not match");

            codeNode.Joins.Count.Should().Be(result.Joins.Count);
            codeNode.Joins
                .OrderBy(x => x.Left)
                .Zip(result.Joins.OrderBy(x => x.Left), (o, i) => new { o, i })
                .All(x => x.o.Equals(x.i))
                .Should().BeTrue();
        }

        private QueryNode ParseData(IEnumerable<string> data, bool shouldPass)
        {
            string rawData = string.Join(" ", data);

            var parser = new AstParser<TokenType>(_rules);
            ParserResult result = parser.Parse(rawData);
            result.IsSuccess.Should().Be(shouldPass, result?.LastGood?.ToString() ?? "<no good last>");
            if (!result.IsSuccess)
            {
                return null;
            }

            // ====================================================================================
            string rowsetName = result.RootNode
                .OfType<Expression<TokenType>>()
                .FirstOrDefault(x => Language.Rowset.Equals(x))
                ?.Value;

            // ====================================================================================
            IEnumerable<Rowset> rowsetList = result.RootNode
                .OfType<Expression<TokenType>>()
                .Where(x => Language.RefRowset.Equals(x) || Language.RefRowsetAlias.Equals(x))
                .Partition((l, x) => !Language.RefRowsetAlias.Equals(x))
                .Select(x => new Rowset(x));

            // ====================================================================================
            IEnumerable<ColumnName> columnList = result.RootNode
                .Where(x => Language.ColumnReference.Equals(x) || Language.SymAs.Equals(x) || Language.ColumnName.Equals(x))
                .Partition((l, x) => Language.ColumnName.Equals(l.LastOrDefault()))
                .Select(x => new ColumnName(x));

            IEnumerable<Join> joinList = result.RootNode
                .Where(x => Language.JoinReference.Equals(x))
                .Partition(2)
                .Select(x => new Join(x));

            return new QueryNode(rowsetName, rowsetList, columnList, joinList);
        }

        private class QueryNode
        {
            public QueryNode(string rowsetName, IEnumerable<Rowset> rowsetReferences, IEnumerable<ColumnName> columnNames, IEnumerable<Join> joins = null)
            {
                RowsetName = rowsetName;
                RowsetReferences = rowsetReferences?.ToList() ?? new List<Rowset>();
                ColumnNames = columnNames?.ToList() ?? new List<ColumnName>();
                Joins = joins?.ToList() ?? new List<Join>();
            }

            public string RowsetName { get; }

            public IReadOnlyList<Rowset> RowsetReferences { get; }

            public IReadOnlyList<ColumnName> ColumnNames { get; }

            public IReadOnlyList<Join> Joins { get; }
        }

        private class Rowset
        {
            public Rowset(IEnumerable<IAstNode> nodes)
            {
                Name = nodes
                    .Where(x => Language.RefRowset.Equals(x))
                    .OfType<Expression<TokenType>>()
                    .Select(x => x.Value)
                    .First();

                Alias = nodes
                    .Where(x => Language.RefRowsetAlias.Equals(x))
                    .OfType<Expression<TokenType>>()
                    .Select(x => x.Value)
                    .FirstOrDefault();
            }

            public Rowset(string name, string alias)
            {
                Name = name;
                Alias = alias;
            }

            public string Name { get; }
            public string Alias { get; }

            public override bool Equals(object obj)
            {
                Rowset rowset = obj as Rowset;
                if (obj == null)
                {
                    return false;
                }

                return Name == rowset.Name
                    && Alias == rowset.Alias;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ Alias.GetHashCode();
            }
        }

        public class ColumnName
        {
            public ColumnName(IEnumerable<IAstNode> nodes)
            {
                Name = nodes
                    .Where(x => Language.ColumnName.Equals(x))
                    .OfType<Expression<TokenType>>()
                    .Select(x => x.Value)
                    .First();

                ColumnReference = nodes
                    .Where(x => Language.ColumnReference.Equals(x))
                    .OfType<Expression<TokenType>>()
                    .Select(x => x.Value)
                    .FirstOrDefault();
            }

            public ColumnName(string name, string columnReference)
            {
                Name = name;
                ColumnReference = columnReference;
            }

            public string Name { get; }
            public string ColumnReference { get; }

            public override bool Equals(object obj)
            {
                ColumnName columnName = obj as ColumnName;
                if (obj == null)
                {
                    return false;
                }

                return Name == columnName.Name
                    && ColumnReference == columnName.ColumnReference;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ ColumnReference.GetHashCode();
            }
        }

        public class Join
        {
            public Join(IEnumerable<IAstNode> nodes)
            {
                Left = nodes
                    .Where(x => Language.JoinReference.Equals(x))
                    .OfType<Expression<TokenType>>()
                    .Select(x => x.Value)
                    .First();

                Right = nodes
                    .Where(x => Language.JoinReference.Equals(x))
                    .Skip(1)
                    .OfType<Expression<TokenType>>()
                    .Select(x => x.Value)
                    .FirstOrDefault();
            }

            public Join(string left, string right)
            {
                Left = left;
                Right = right;
            }

            public string Left { get; }
            public string Right { get; }

            public override bool Equals(object obj)
            {
                Join join = obj as Join;
                if (obj == null)
                {
                    return false;
                }

                return Left == join.Left
                    && Right == join.Right;
            }

            public override int GetHashCode()
            {
                return Left.GetHashCode() ^ Right.GetHashCode();
            }
        }
    }
}
