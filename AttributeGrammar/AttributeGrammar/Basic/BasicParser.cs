using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace AttributeGrammar.Basic
{
    public class BasicParser
    {
        #region skip-whitespace-symbols
        private static Parser<char, T> Symbol<T>(Parser<char, T> parser)
            => parser.Before(SkipWhitespaces);
        private static Parser<char, char> Symbol(char value)
            => Symbol(Char(value));
        #endregion

        #region symbols
        private static readonly Parser<char, char> _add = Symbol('+');
        private static readonly Parser<char, char> _mult = Symbol('*');
        private static readonly Parser<char, char> _o_brac = Symbol('(');
        private static readonly Parser<char, char> _c_brac = Symbol(')');
        #endregion

        #region helper
        private static Parser<char, T> PositionParser<T>(Parser<char, T> parser) where T : Node
             => Map((startPosition, result, endPosition)
                     => result with { Start = startPosition, End = endPosition },
                     CurrentPos,
                     parser,
                     CurrentPos
                ).Labelled("positioned");

        private static Parser<char, string> FailIfThereIsSomething
            => Any.ManyString().Assert(s => string.IsNullOrWhiteSpace(s));
        #endregion

        #region token
        private static Parser<char, int> Integer
            => Symbol(
                    from sign in OneOf(Char('+'), Char('-')).Or(Return('+'))
                    from @value in Digit.AtLeastOnceString()
                    select int.Parse(sign + @value)
               ).Labelled("Int32");
        #endregion

        #region productions
        private static readonly Parser<char, Factor> Fac
            = Rec(() =>
                OneOf(
                     PositionParser(_o_brac.Then(Expr!.Before(_c_brac)).Select(expr => new Factor { Expr = expr })),
                     Integer.Select(@integer => new Factor { Integer = @integer })
                )
            ).Labelled("factor");

        private static readonly Parser<char, Term> Ter
            = Rec(() =>
                OneOf(
                    PositionParser(Try(Fac.Before(_mult)).Then(Ter!, (fac, ter) => new Term { Ter = ter, Fac = fac })),
                    PositionParser(Fac.Select(fac => new Term { Fac = fac }))
                )
            ).Labelled("term");

        private static readonly Parser<char, Expression> Expr
            = Rec(() =>
                OneOf(
                    PositionParser(Try(Ter.Before(_add)).Then(Expr!, (ter, expr) => new Expression { Expr = expr, Ter = ter })),
                    PositionParser(Ter.Select(ter => new Expression { Ter = ter }))
                )
            ).Labelled("expression");

        private static readonly Parser<char, Expression> S = (
                from expression in SkipWhitespaces.Then(Expr)
                from _ in FailIfThereIsSomething
                select expression
            ).Labelled("one expression");
        #endregion

        #region interface
        public static Expression ParseExpression(string input)
            => S.ParseOrThrow(input ?? throw new ArgumentNullException(nameof(input)));
        #endregion
    }
}