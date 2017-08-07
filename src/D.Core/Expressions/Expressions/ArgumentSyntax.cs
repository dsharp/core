﻿using System;

namespace D.Syntax
{
    public class ArgumentSyntax : SyntaxNode
    {
        public ArgumentSyntax(SyntaxNode value)
        {
            Value = value;
        }

        public ArgumentSyntax(Symbol name, SyntaxNode value)
        {
            Name = name;
            Value = value;
        }

        public Symbol Name { get; }

        public SyntaxNode Value { get; }

        Kind IObject.Kind => Kind.Argument;
    }
}