﻿namespace D.Syntax
{
    // let a: Integer = 5
    // let a of type Integer equal 5
    public class VariableDeclarationSyntax : SyntaxNode
    {
        public VariableDeclarationSyntax(string name, Symbol type, bool mutable, SyntaxNode value = null)
        {
            Name = name;
            Type = type;
            IsMutable = mutable;
            Value = value;
        }

        public string Name { get; }

        public Symbol Type { get; }

        public bool IsMutable { get; }

        public SyntaxNode Value { get; }

        Kind IObject.Kind => Kind.VariableDeclaration;
    }

    public class CompoundVariableDeclaration : SyntaxNode
    {
        public CompoundVariableDeclaration(VariableDeclarationSyntax[] declarations)
        {
            Declarations = declarations;
        }

        public VariableDeclarationSyntax[] Declarations { get; }

        Kind IObject.Kind => Kind.CompoundVariableDeclaration;
    }
}

/*
let a: Integer = 1;
let a: Integer > 1 = 5;
let mutable a = 1;
var a = 1
*/