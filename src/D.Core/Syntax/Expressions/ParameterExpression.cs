﻿using E.Symbols;

namespace E.Syntax
{
    public sealed class ParameterSyntax
    {
        public ParameterSyntax(
            Symbol name, 
            Symbol? type = null,
            ISyntaxNode? defaultValue = null,
            ISyntaxNode? condition = null,
            AnnotationSyntax[]? annotations = null,
            int index = 0)
        {
            Name         = name;
            Type         = type;
            DefaultValue = defaultValue;
            Condition    = condition;
            Index        = index;
            Annotations  = annotations;
        }

        public Symbol Name { get; }

        public Symbol? Type { get; }

        public int Index { get; }

        public ISyntaxNode? DefaultValue { get; }

        public ISyntaxNode? Condition { get; }

        public AnnotationSyntax[]? Annotations { get; }
    }
}