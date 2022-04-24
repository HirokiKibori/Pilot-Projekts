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
        private static Parser<char, T> PositionParser<T>(Parser<char, T> parser) where T : Positioned
             => Map((startPosition, result, endPosition)
                 =>
                    {
                        result.SetPosition(startPosition, endPosition);
                        return result;
                    },
                 CurrentPos,
                 parser,
                 CurrentPos);

        private static Parser<char, string> FailIfThereIsSomething
            => Any.ManyString().Assert(s => string.IsNullOrWhiteSpace(s));
        #endregion

        #region token
        private static readonly Parser<char, int> _integer
            = DecimalNum.Before(SkipWhitespaces).Labelled("Int32");
        #endregion

        #region productions
        private static readonly Parser<char, Factor> Fac
            = Rec(() =>
                OneOf(
                     PositionParser(_o_brac.Then(Expr!.Before(_c_brac)).Select(expr => new Factor(expr))),
                     _integer.Select(integer => new Factor(integer))
                ).Labelled("factor")
            );

        private static readonly Parser<char, Term> Ter
            = Rec(() =>
                OneOf(
                    PositionParser(Try(Fac.Before(_mult)).Then(Ter!, (fac, ter) => new Term(ter, fac))),
                    PositionParser(Fac.Select(fac => new Term(fac)))
                ).Labelled("term")
            );

        private static readonly Parser<char, Expression> Expr
            = Rec(() =>
                OneOf(
                    PositionParser(Try(Ter.Before(_add)).Then(Expr!, (ter, expr) => new Expression(expr, ter))),
                    PositionParser(Ter.Select(ter => new Expression(ter)))
                ).Labelled("expression")
            );

        private static readonly Parser<char, Expression> S
            = from expression in SkipWhitespaces.Then(Expr)
              from _ in FailIfThereIsSomething
              select expression;
        #endregion

        #region interface
        public static Expression ParseExpression(string input)
            => S.ParseOrThrow(input ?? throw new ArgumentNullException(nameof(input)));
        #endregion
    }
}