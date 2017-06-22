﻿using System;
using System.Text;

namespace D
{
    using Expressions;
    using Syntax;

    public abstract class Symbol : IExpression, SyntaxNode
    {
        public Symbol(string name, SymbolFlags flags = SymbolFlags.None)
        {
            Name = name;
            Flags = flags;
            Arguments = Array.Empty<Symbol>();
        }

        public Symbol(string module, string name, Symbol[] arguments, SymbolFlags flags = SymbolFlags.None)
        {
            Module      = module;
            Name        = name;
            Arguments   = arguments;
            Flags       = flags;
        }

        public Symbol(string name, Symbol[] arguments, SymbolFlags flags = SymbolFlags.None)
        {
            Name        = name;
            Arguments   = arguments;
            Flags       = flags;
        }

        // AKA namespace
        public string Module { get; }

        public string Name { get; }
        
        // Declaration?

        public Symbol[] Arguments { get; }

        public SymbolFlags Flags { get; }

        public abstract SymbolType SymbolType { get; }

        #region Initializization

        public void Initialize(IType type)
        {
            ResolvedTyped = type;

            Status = SymbolStatus.Resolved;
        }

        public SymbolStatus Status { get; set; } = SymbolStatus.Unresolved;

        public IType ResolvedTyped { get; set; }

        #endregion

        Kind IObject.Kind => Kind.Symbol;

        public override string ToString()
        {
            if (Module == null && Arguments.Length == 0)
            {
                return Name;
            }

            var sb = new StringBuilder();

            if (Module != null)
            {
                sb.Append(Module);
                sb.Append("::");
            }

            sb.Append(Name);

            if (Arguments.Length > 0)
            {
                sb.Append("<");

                var i = 0;

                foreach (var arg in Arguments)
                {
                    if (++i > 1) sb.Append(",");

                    sb.Append(arg.ToString());
                }

                sb.Append(">");
            }

            return sb.ToString();
        }

        public static LabelSymbol Label(string name) =>
            new LabelSymbol(name);

        public static VariableSymbol Variable(string name) =>
            new VariableSymbol(name);

        public static Symbol Argument(string name) => 
            new ArgumentSymbol(name);

        public static TypeSymbol Type(string name) =>
            new TypeSymbol(name);

        public static Symbol Type(string name, params Symbol[] arguments) =>
            new TypeSymbol(name, arguments);

        public static implicit operator string(Symbol symbol) =>
            symbol?.ToString();
    }
}