namespace AttributeGrammar.Basic
{
    public class Positioned
    {
        public SourcePos? Start { get; protected set; } = null;
        public SourcePos? End { get; protected set; } = null;

        public Positioned() => (Start, End) = (default, default);

        public void SetPosition(SourcePos start, SourcePos end) => (Start, End) = (start, end);

        public override string ToString()
        {
            if(Start.HasValue && End.HasValue)
                return $" [({Start.Value.Line}|{Start.Value.Col}) - ({End.Value.Line}|{End.Value.Col})]";

            return string.Empty;
        }
    }

    public class Expression : Positioned
    {
        public Expression? Expr { get; } = null;
        public Term Ter { get; }

        public Expression(Term term) => Ter = term;

        public Expression(Expression expression, Term term) => (Expr, Ter) = (expression, term);

        public override string ToString()
        {
            var buffer = (Expr is not null) ? Environment.NewLine + Expr : "";
            return $"Expression\t{base.ToString()}:{Ter}{buffer}";
        }
    }

    public class Term : Positioned
    {
        public Term? Ter { get; } = null;
        public Factor Fac { get; }

        public Term(Factor factor) => Fac = factor;

        public Term(Term term, Factor factor) => (Ter, Fac) = (term, factor);


        public override string ToString()
        {
            return $"{Environment.NewLine}Term\t\t{base.ToString()}: {Fac}{Ter?.ToString()}";
        }
    }

    public class Factor : Positioned
    {
        public Expression? Expr { get; } = null;
        public int? Integer { get; } = null;

        public Factor(Expression? expr) => Expr = expr;
        public Factor(int integer) => Integer = integer;

        public override string ToString()
        {
            if(Expr is not null)
            {
                return $"{Environment.NewLine}Factor\t\t{base.ToString()}:{Environment.NewLine}{Environment.NewLine + Expr}";
            }

            return $"Factor{base.ToString()}: {Integer?.ToString()}";
        }
    }
}