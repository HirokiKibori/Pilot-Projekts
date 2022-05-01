using System.Linq;

using AttributeGrammar.Basic;

using Sawmill;

namespace TestAttributeGrammar.Parsers
{
    public class BasicParserTest : BasicTest
    {
        public BasicParserTest(ITestOutputHelper output)
              : base(output) { }

        int CountExpressions(Node node)
            => node
                .SelfAndDescendants()
                .OfType<Expression>()
                .Count();

        //TODO: Try to implement 'int Calculate(Node node)' and test expressions for the correct result

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
        [InlineData("()")]
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

        [Fact]
        public void ParseExpression_Integer_overflow()
        {
            string input = "" + (1.0d + int.MaxValue);

            var parseInput = () => BasicParser.ParseExpression(input);
            var result = Assert.Throws<OverflowException>(parseInput);

            output.WriteLine(result.Message);
        }

        [Fact]
        public void ParseExpression_Integer_underflow()
        {
            string input = "" + (int.MinValue - 1.0d);

            var parseInput = () => BasicParser.ParseExpression(input);
            var result = Assert.Throws<OverflowException>(parseInput);

            output.WriteLine(result.Message);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("-1", 1)]
        [InlineData("+1", 1)]
        [InlineData("(1)", 2)]
        [InlineData(" ( 1 ) ", 2)]
        [InlineData("((1))", 3)]
        [InlineData("1 + -2", 2)]
        [InlineData("1 + +2", 2)]
        [InlineData("-1 + 2", 2)]
        [InlineData("+1 + 2", 2)]
        [InlineData("1 * 2 * 3", 1)]
        [InlineData("1 + 2 + 3", 3)]
        [InlineData("1 + 2 * 3 * ( 4 + (5 * 6) + (7))", 7)]
        [InlineData("((7) + (6 * 5) + 4) * 3 * 2 + 1", 7)]
        public void ParseExpression_successfull(string input, int expectedExpressions)
        {
            var expression = BasicParser.ParseExpression(input);
            var result = CountExpressions(expression);

            Assert.Equal(expectedExpressions, result);

            output.WriteLine(expression.ToString());
        }
    }
}
