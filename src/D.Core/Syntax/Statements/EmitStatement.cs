﻿namespace D.Syntax
{
    public class EmitStatement : SyntaxNode
    {
        public EmitStatement(SyntaxNode expression)
        {
            Expression = expression;
        }

        public SyntaxNode Expression { get; }

        Kind IObject.Kind => Kind.EmitStatement;
    }
}