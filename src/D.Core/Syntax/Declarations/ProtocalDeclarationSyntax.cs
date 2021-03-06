﻿using E.Protocols;
using E.Symbols;

namespace E.Syntax
{
    // A protocol { }

    public sealed class ProtocolDeclarationSyntax : ISyntaxNode
    {
        public ProtocolDeclarationSyntax(
            Symbol name,
            IProtocolMessage[] messages, 
            FunctionDeclarationSyntax[] members)
        {
            Name    = name;
            Messages = messages;
            Members = members;
        }

        public Symbol Name { get; }

        public IProtocolMessage[] Messages { get; } 

        public FunctionDeclarationSyntax[] Members { get; }

        SyntaxKind ISyntaxNode.Kind => SyntaxKind.ProtocolDeclaration;
    }
}