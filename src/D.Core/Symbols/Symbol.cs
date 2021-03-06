﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using E.Expressions;
using E.Syntax;

namespace E.Symbols
{
    public abstract class Symbol : IExpression, ISyntaxNode
    {
        public Symbol(string name, SymbolFlags flags = SymbolFlags.None)
        {
            Name = name;
            Flags = flags;
            Arguments = Array.Empty<ArgumentSymbol>();
        }

        public Symbol(string name, IReadOnlyList<Symbol> arguments, SymbolFlags flags = SymbolFlags.None)
        {
            Name = name;
            Arguments = arguments;
            Flags = flags;
        }

        public Symbol(
            ModuleSymbol? module,
            string name,
            IReadOnlyList<Symbol> arguments,
            SymbolFlags flags = SymbolFlags.None)
        {
            Module = module;
            Name = name;
            Arguments = arguments;
            Flags = flags;
        }

        public ModuleSymbol? Module { get; }

        public string Name { get; }

        public IReadOnlyList<Symbol>? Arguments { get; }

        public SymbolFlags Flags { get; }

        // TODO: Scope

        // Constructor + Self

        #region Initializization / Binding

        public SymbolStatus Status { get; protected set; } = SymbolStatus.Unresolved;
        
        public Symbol? ContainingType { get; set; } // if a member of a type

        public Symbol? ContainingModule { get; set; } // if a member of a module

        #endregion

        ObjectType IObject.Kind => ObjectType.Symbol;

        public override string ToString()
        {
            if (Module is null && (Arguments is null || Arguments.Count == 0))
            {
                return Name;
            }

            var sb = new StringBuilder();

            WriteTo(sb);

            return sb.ToString();
        }

        public void WriteTo(StringBuilder sb)
        {
            if (Module is null && (Arguments is null || Arguments.Count == 0))
            {
                sb.Append(Name);

                return;
            }

            if (Module is not null)
            {
                sb.Append(Module);
                sb.Append("::");
            }

            sb.Append(Name);

            if (Arguments is { Count: > 0 })
            {
                sb.Append('<');

                var i = 0;

                foreach (var arg in Arguments)
                {
                    if (++i > 1)
                    {
                        sb.Append(',');
                    }

                    arg.WriteTo(sb);
                }

                sb.Append('>');
            }
        }

        public virtual bool TryGetValue(string name, out Symbol? value)
        {
            value = default;

            return false;
        }

        public virtual void Add(Symbol child)
        {
            throw new NotImplementedException();
        }

        public static LabelSymbol Label(string name) =>
            new LabelSymbol(name);

        public static VariableSymbol Variable(string name) =>
            new VariableSymbol(name);

        public static Symbol Argument(string name) =>
            new ArgumentSymbol(name);

        public static TypeSymbol Type(string name) => new TypeSymbol(name);

        public static Symbol Type(string name, params Symbol[] arguments) =>
            new TypeSymbol(name, arguments);

        [return:NotNullIfNotNull("symbol")]
        public static implicit operator string? (Symbol? symbol) => symbol?.ToString();

        SyntaxKind ISyntaxNode.Kind => SyntaxKind.Symbol;
    }
}

// Symbols include:
// - Types: Object, Decimal, Array<string>
// - Parameter Names
// - Property Names
// - Function Names
// - Variable Names (locals)

// Symbol scopes
// - Immediate block
// - Module