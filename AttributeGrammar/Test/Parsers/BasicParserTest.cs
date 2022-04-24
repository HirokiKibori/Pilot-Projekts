using AttributeGrammar.Basic;

namespace TestAttributeGrammar.Parsers
{
    public class BasicParserTest : BasicTest
    {
        private static string INT32_OVERFLOW = $"{1.0d + int.MaxValue}";

        public BasicParserTest(ITestOutputHelper output)
              : base(output) { }

        [Fact]
        public void ParseExpression_null()
        {
            var parseNull = () => BasicParser.ParseExpression(null!);
            var result = Assert.Throws<ArgumentNullException>(parseNull);

            output.WriteLine(result.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ParseExpression_empty(string input)
        {
            var parseInput = () => BasicParser.ParseExpression(input);
            var result = Assert.Throws<Pidgin.ParseException>(parseInput);

            output.WriteLine(result.Message);
        }

        [Theory]
        [InlineData("<")]
        [InlineData("())")]
        [InlineData("(()")]
        [InlineData("1 * * 1")]
        [InlineData("1 + + 1")]
        [InlineData("1 +")]
        [InlineData("+ 1")]
        [InlineData("1 *")]
        [InlineData("* 1")]
        [InlineData("- 1")]
        [InlineData("1 1 + 2")]
        [InlineData("1 + 2 2")]
        [InlineData("0.5")]
        public void ParseExpression_not_parseable(string input)
        {
            var parseInput = () => BasicParser.ParseExpression(input);
            var result = Assert.Throws<Pidgin.ParseException>(parseInput);

            output.WriteLine(result.Message);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("-1", 1)]
        [InlineData("+1", 1)]
        [InlineData("(1)", 2)]
        [InlineData("((1))", 3)]
        [InlineData("1 + -2", 2)]
        [InlineData("1 + +2", 2)]
        [InlineData("1 * 2 * 3", 1)]
        [InlineData("1 + 2 + 3", 3)]
        [InlineData("1 + 2 * 3 * ( 4 + (5 * 6) + (7))", 7)]
        [InlineData("((7) + (6 * 5) + 4) * 3 * 2 + 1", 7)]
        public void ParseExpression_successfull(string input, int expectedExpressions)
        {
            var expression = BasicParser.ParseExpression(input);
            var result = Help.CountExpressions(expression);

            Assert.Equal(expectedExpressions, result);

            output.WriteLine(expression.ToString());
        }
    }

    class Help
    {
        public static int CountExpressions(Expression? expression)
        {
            if (expression is null)
                return 0;

            return 1 + CountExpressions(expression.Expr) + CountExpressionsInTerm(expression.Ter);
        }

        #region count expressions
        private static int CountExpressionsInTerm(Term? term)
        {
            if (term is null)
                return 0;

            return CountExpressionsInTerm(term.Ter) + CountExpressionsInFactor(term.Fac);
        }

        private static int CountExpressionsInFactor(Factor factor)
        {
            if (factor.Expr is not null)
                return CountExpressions(factor.Expr);

            return 0;
        }
        #endregion
    }
}
