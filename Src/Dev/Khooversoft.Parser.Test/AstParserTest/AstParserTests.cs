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
    public class AstParserTests
    {
        private enum TokenType
        {
            Space,
            Equal,
            SStream,
            SemiColon,
            Declare,
            Plus,
            Reference,
            Using,
            As,
            Comma,
            Select,
            Where,
            From,
            Rowset,
            Variable,
            VariableType,
            Value,
            ReferenceColumnName,
            ColumnName,
        }

        private static readonly Grammar<TokenType> _space = new Grammar<TokenType>(TokenType.Space, " ");

        [Fact]
        public void AstSingleRuleTest()
        {
            var rowset = new Expression<TokenType>(TokenType.Rowset);
            var variable = new Expression<TokenType>(TokenType.Variable);
            var equals = new Symbol<TokenType>(TokenType.Equal, "=");
            var sstream = new Symbol<TokenType>(TokenType.SStream, "SSTREAM");
            var semiColon = new Symbol<TokenType>(TokenType.SemiColon, ";");

            var rules = new AstProductionRules<TokenType>()
            {
                new AstNode() + rowset + equals + sstream + variable + semiColon,
            } + _space;

            AstNode tree = new AstParser<TokenType>(rules)
                .Parse("RdfCountryOrig = SSTREAM @RdfCountrySS;")
                ?.RootNode;

            tree.Should().NotBeNull();

            var check = new AstNode
            {
                new Expression<TokenType>(TokenType.Rowset, "RdfCountryOrig"),
                equals,
                sstream,
                new Expression<TokenType>(TokenType.Variable, "@RdfCountrySS"),
                semiColon,
            };

            tree.Count.Should().Be(check.Count);

            var checkList = tree
                .Zip(check, (o, i) => new { o, i })
                .ToList();

            checkList.All(x => x.o.Equals(x.i)).Should().BeTrue();
        }

        [Fact]
        public void AstMultipleRuleTest()
        {
            var symEqual = new Symbol<TokenType>(TokenType.Equal, "=");
            var symSstream = new Symbol<TokenType>(TokenType.SStream, "SSTREAM");
            var symReference = new Symbol<TokenType>(TokenType.Reference, "REFERENCE");
            var symDeclare = new Symbol<TokenType>(TokenType.Declare, "#DECLARE");
            var symSemiColon = new Symbol<TokenType>(TokenType.SemiColon, ";");
            var symPlus = new Symbol<TokenType>(TokenType.Plus, "+");
            var symUsing = new Symbol<TokenType>(TokenType.Using, "USING");

            var rowset = new Expression<TokenType>(TokenType.Rowset);
            var variable = new Expression<TokenType>(TokenType.Variable);
            var variableType = new Expression<TokenType>(TokenType.VariableType);
            var value = new Expression<TokenType>(TokenType.Value);

            var rules = new AstProductionRules<TokenType>()
            {
                new AstNode()
                    + (new Choice()
                        + (new AstNode() + rowset + symEqual + symSstream + variable + symSemiColon)                   // {rowset} = SSTREAM {variable};
                        + (new AstNode() + symDeclare + variable + variableType + symEqual + value + symSemiColon)     // #DECLARE {variable} int = {value};
                        + (new AstNode() + symReference + value + symSemiColon)                                        // REFERENCE {referenceAssembly};
                        + (new AstNode() + symUsing + value + symSemiColon)                                            // USING {value};
                        ),
                _space,
            };

            var variations = new[]
            {
                new {
                    RawData = "# PoiNamedPlace int = 4444;",
                    Result = (AstNode)null,
                },
                new {
                    RawData = "#DECLARE PoiNamedPlace int = 4444;",
                    Result = new AstNode {
                        symDeclare,
                        new Expression<TokenType>(TokenType.Variable, "PoiNamedPlace"),
                        new Expression<TokenType>(TokenType.VariableType, "int"),
                        symEqual,
                        new Expression<TokenType>(TokenType.Value, "4444"),
                        symSemiColon,
                    }
                },

                new {
                    RawData = "RdfCountryOrig = SS @RdfCountrySS; ",
                    Result = (AstNode)null,
                },
                new {
                    RawData = "RdfCountryOrig = SSTREAM @RdfCountrySS;  ",
                    Result = new AstNode {
                        new Expression<TokenType>(TokenType.Rowset, "RdfCountryOrig"),
                        symEqual,
                        symSstream,
                        new Expression<TokenType>(TokenType.Variable, "@RdfCountrySS"),
                        symSemiColon,
                    }
                },

                new {
                    RawData = "REFERENCE \"System.Data.dll\"",
                    Result = (AstNode)null,
                },
                new {
                    RawData = "REFERENCE \"System.Data.dll\";",
                    Result = new AstNode {
                        symReference,
                        new Expression<TokenType>(TokenType.Value, "\"System.Data.dll\""),
                        symSemiColon,
                    }
                },
            };

            int testNumber = -1;
            Func<string> why = () => $"TestNumber: {testNumber}";

            foreach (var test in variations)
            {
                testNumber++;

                AstNode tree = new AstParser<TokenType>(rules)
                    .Parse(test.RawData)
                    ?.RootNode;

                if (test.Result == null)
                {
                    tree.Should().BeNull(why());
                    continue;
                }

                tree.Should().NotBeNull(why());
                tree.Count.Should().Be(test.Result.Count, why());

                var checkList = tree
                    .Zip(test.Result, (o, i) => new { o, i })
                    .ToList();

                checkList.All(x => x.o.Equals(x.i)).Should().BeTrue(why());
            }
        }

        [Fact]
        public void AstRepeatRuleTest()
        {
            var equal = new Symbol<TokenType>(TokenType.Equal, "=");
            var semiColon = new Symbol<TokenType>(TokenType.SemiColon, ";");
            var declare = new Symbol<TokenType>(TokenType.Declare, "#DECLARE");
            var plus = new Symbol<TokenType>(TokenType.Plus, "+");

            var variable = new Expression<TokenType>(TokenType.Variable);
            var variableType = new Expression<TokenType>(TokenType.VariableType);
            var value = new Expression<TokenType>(TokenType.Value);
            var repeatPlusValue = new Optional() + (new Repeat() + plus + value);

            var rules = new AstProductionRules<TokenType>()
            {
                new AstNode() + declare + variable + variableType + equal + value + repeatPlusValue + semiColon,
            } + _space;

            var variations = new[]
{
                new {
                    RawData = "#DECLARE EntityTSV string = @@OutputPath@@ + ;",
                    Result = (AstNode)null,
                },
                new {
                    RawData = "#DECLARE EntityTSV string = @@OutputPath@@;",
                    Result = new AstNode {
                        declare,
                        new Expression<TokenType>(TokenType.Variable, "EntityTSV"),
                        new Expression<TokenType>(TokenType.VariableType, "string"),
                        equal,
                        new Expression<TokenType>(TokenType.Value, "@@OutputPath@@"),
                        semiColon,
                    }
                },
                new {
                    RawData = "#DECLARE EntityTSV string = @@OutputPath@@ + \"Entity.tsv\";",
                    Result = new AstNode {
                        declare,
                        new Expression<TokenType>(TokenType.Variable, "EntityTSV"),
                        new Expression<TokenType>(TokenType.VariableType, "string"),
                        equal,
                        new Expression<TokenType>(TokenType.Value, "@@OutputPath@@"),
                        plus,
                        new Expression<TokenType>(TokenType.Value, "\"Entity.tsv\""),
                        semiColon,
                    }
                },
                new {
                    RawData = "#DECLARE EntityTSV string = @@OutputPath@@ + \"Entity.tsv\" +;",
                    Result = (AstNode)null,
                },
                new {
                    RawData = "#DECLARE EntityTSV string = @@OutputPath@@ + \"Entity.tsv\" + \"/testPath\";",
                    Result = new AstNode {
                        declare,
                        new Expression<TokenType>(TokenType.Variable, "EntityTSV"),
                        new Expression<TokenType>(TokenType.VariableType, "string"),
                        equal,
                        new Expression<TokenType>(TokenType.Value, "@@OutputPath@@"),
                        plus,
                        new Expression<TokenType>(TokenType.Value, "\"Entity.tsv\""),
                        plus,
                        new Expression<TokenType>(TokenType.Value, "\"/testPath\""),
                        semiColon,
                    }
                },
            };

            foreach (var test in variations)
            {
                AstNode tree = new AstParser<TokenType>(rules)
                    .Parse(test.RawData)
                    ?.RootNode;

                if (test.Result == null)
                {
                    tree.Should().BeNull();
                    continue;
                }

                tree.Should().NotBeNull(test.RawData);

                tree.Count.Should().Be(test.Result.Count);

                var checkList = tree
                    .Zip(test.Result, (o, i) => new { o, i })
                    .ToList();

                checkList.All(x => x.o.Equals(x.i)).Should().BeTrue();
            }
        }

        [Fact]
        public void AstOptionalRuleTest()
        {
            var symEqual = new Symbol<TokenType>(TokenType.Equal, "=");
            var symSemiColon = new Symbol<TokenType>(TokenType.SemiColon, ";");
            var symSelect = new Symbol<TokenType>(TokenType.Select, "SELECT");
            var symAs = new Symbol<TokenType>(TokenType.As, "AS");
            var symComma = new Symbol<TokenType>(TokenType.Comma, ",");
            var symFrom = new Symbol<TokenType>(TokenType.From, "FROM");
            var symWhere = new Symbol<TokenType>(TokenType.Where, "WHERE");

            var rowset = new Expression<TokenType>(TokenType.Rowset);
            var variable = new Expression<TokenType>(TokenType.Variable);
            var referenceColumnName = new Expression<TokenType>(TokenType.ReferenceColumnName);
            var columnName = new Expression<TokenType>(TokenType.ColumnName);

            var optionalAs = new Optional() + referenceColumnName + symAs;
            var repeatColumns = new Repeat() + symComma + optionalAs + columnName;

            var rules = new AstProductionRules<TokenType>()
            {
                new AstNode() + rowset + symEqual + symSelect + optionalAs + columnName + repeatColumns + symFrom + rowset + symSemiColon,
            } + _space;

            var variations = new[]
{
                new {
                    RawData = new List<string> { "a = SELECT FROM;" },
                    Result = (AstNode)null,
                },
                new {
                    RawData = new List<string> {
                        "RdfCartoLink =",
                        "   SELECT carto_id.ToString() AS carto_id,",
                        "           link_id,",
                        "           long_haul,",
                        "           coverage_indicator,",
                        "           line_of_control,",
                        "           claimed_by,",
                        "           controlled_by,",
                        "           expanded_inclusion",
                        "    FROM RdfCartoLinkOrig;",
                    },
                    Result = new AstNode {
                        new Expression<TokenType>(TokenType.Rowset, "RdfCartoLink"),
                        symEqual,
                        symSelect,
                        new Expression<TokenType>(TokenType.ReferenceColumnName, "carto_id.ToString()"),
                        symAs,
                        new Expression<TokenType>(TokenType.ColumnName, "carto_id"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "link_id"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "long_haul"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "coverage_indicator"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "line_of_control"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "claimed_by"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "controlled_by"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "expanded_inclusion"),
                        symFrom,
                        new Expression<TokenType>(TokenType.Rowset, "RdfCartoLinkOrig"),
                        symSemiColon,
                    }
                },
                new {
                    RawData = new List<string> {
                        "RdfLink =",
                        "    SELECT link_id,",
                        "           ref_node_id,",
                        "           nonref_node_id,",
                        "           left_admin_place_id.ToString() AS left_admin_place_id,",
                        "           right_admin_place_id.ToString() AS right_admin_place_id,",
                        "           left_postal_area_id,",
                        "           right_postal_area_id,",
                        "           bridge,",
                        "           tunnel,",
                        "           map_edge_link",
                        "    FROM RdfLinkOrig;",
                    },
                    Result = new AstNode {
                        new Expression<TokenType>(TokenType.Rowset, "RdfLink"),
                        symEqual,
                        symSelect,
                        new Expression<TokenType>(TokenType.ColumnName, "link_id"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "ref_node_id"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "nonref_node_id"),
                        symComma,

                        new Expression<TokenType>(TokenType.ReferenceColumnName, "left_admin_place_id.ToString()"),
                        symAs,
                        new Expression<TokenType>(TokenType.ColumnName, "left_admin_place_id"),
                        symComma,

                        new Expression<TokenType>(TokenType.ReferenceColumnName, "right_admin_place_id.ToString()"),
                        symAs,
                        new Expression<TokenType>(TokenType.ColumnName, "right_admin_place_id"),
                        symComma,

                        new Expression<TokenType>(TokenType.ColumnName, "left_postal_area_id"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "right_postal_area_id"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "bridge"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "tunnel"),
                        symComma,
                        new Expression<TokenType>(TokenType.ColumnName, "map_edge_link"),
                        symFrom,
                        new Expression<TokenType>(TokenType.Rowset, "RdfLinkOrig"),
                        symSemiColon,
                    }
                },
            };

            foreach (var test in variations)
            {
                AstNode tree = new AstParser<TokenType>(rules)
                    .Parse(string.Join(" ", test.RawData))
                    ?.RootNode;

                if (test.Result == null)
                {
                    tree.Should().BeNull();
                    continue;
                }

                tree.Count.Should().Be(test.Result.Count);

                var checkList = tree
                    .Zip(test.Result, (o, i) => new { o, i })
                    .ToList();

                checkList.All(x => x.o.Equals(x.i)).Should().BeTrue();
            }
        }
    }
}
