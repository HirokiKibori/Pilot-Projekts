using Sawmill;

namespace AttributeGrammar.Basic
{
    public abstract record Node : IRewritable<Node>
    {
        public SourcePos? Start = null;
        public SourcePos? End = null;

        public abstract int CountChildren();

        public abstract void GetChildren(Span<Node> childrenReceiver);

        public abstract Node SetChildren(ReadOnlySpan<Node> newChildren);

        public override string ToString()
        {
            if(Start.HasValue && End.HasValue)
                return $" [({Start.Value.Line}|{Start.Value.Col}) - ({End.Value.Line}|{End.Value.Col})]";

            return string.Empty;
        }
    }

    public record Expression : Node
    {
        public Expression? Expr = null;
        public Term? Ter;

        public override int CountChildren() => Expr is not null ? 2 : 1;

        public override void GetChildren(Span<Node> childrenReceiver)
        {
            childrenReceiver[0] = Ter!;

            if(Expr is not null)
                childrenReceiver[1] = Expr!;
        }

        public override Node SetChildren(ReadOnlySpan<Node> newChildren)
            => newChildren.Length < 2
                ? new Expression { Ter = newChildren[0] as Term }
                : new Expression { Ter = newChildren[0] as Term, Expr = newChildren[1] as Expression };

        public override string ToString()
        {
            var buffer = (Expr is not null) ? Environment.NewLine + Expr : "";
            return $"Expression\t{base.ToString()}:{Ter}{buffer}";
        }
    }

    public record Term : Node
    {
        public Term? Ter = null;
        public Factor? Fac;

        public override int CountChildren() => Ter is not null ? 2 : 1;

        public override void GetChildren(Span<Node> childrenReceiver)
        {
            childrenReceiver[0] = Fac!;

            if(Ter is not null)
                childrenReceiver[1] = Ter!;
        }

        public override Node SetChildren(ReadOnlySpan<Node> newChildren)
            => newChildren.Length < 2
                ? new Term { Fac = newChildren[0] as Factor }
                : new Term { Fac = newChildren[0] as Factor, Ter = newChildren[1] as Term };

        public override string ToString()
        {
            return $"{Environment.NewLine}Term\t\t{base.ToString()}: {Fac}{Ter?.ToString()}";
        }
    }

    public record Factor : Node
    {
        public Expression? Expr = null;
        public int? Integer = null;

        public override int CountChildren() => Expr is not null ? 1 : 0;

        public override void GetChildren(Span<Node> childrenReceiver)
        {
            if(Expr is not null)
                childrenReceiver[0] = Expr!;
        }

        public override Node SetChildren(ReadOnlySpan<Node> newChildren)
            => newChildren.Length > 0 ? new Factor { Expr = newChildren[0] as Expression } : this;

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