﻿using System;

using E.Expressions;
using E.Symbols;

namespace E
{
    public partial class Compiler
    {
        public Type GetReturnType(BlockExpression block)
        {
            foreach (var expression in block.Statements)
            {
                if (expression is ReturnStatement returnStatement)
                {
                    return GetType(returnStatement.Expression);
                }
            }

            throw new Exception("Block has no return statement:" + block[0].ToString());
        }

        public Type GetReturnType(LambdaExpression lambda)
        {
            return GetType(lambda.Expression);
        }

        public Type GetType(IObject expression)
        {
            if (expression is Symbol name)
            {
                return env.Get<Type>(flow.Infer(name.Name).Name);

                /*
                if (env.TryGetValue(name, out Type obj))
                {
                    return obj;
                }
                */
            }

            switch (expression)
            {
                case MemberAccessExpression access:
                    Type type = GetType(access.Left) ?? throw new Exception("no type found for left");

                    var member = type.GetProperty(access.MemberName);

                    if (member is null) return Type.Get(ObjectType.Object);

                    // if (member is null) throw new Exception($"{type.Name}::{access.MemberName} not found");

                    return member.Type;
                    
                case CallExpression call:
                    return call.ReturnType ?? Type.Get(ObjectType.Object);

                case InterpolatedStringExpression _:
                    return Type.Get(ObjectType.String);

                case TypeSymbol symbol:
                    return new Type(symbol.Name, Type.Get(ObjectType.Object), null, null);

                case BinaryExpression b:
                    if (b.Operator.IsComparision || b.Operator.IsLogical)
                    {
                        return Type.Get(ObjectType.Boolean);
                    }
                    else
                    {
                        // throw new Exception(b.Left.GetType().ToString() + "/" + b.Right.GetType().ToString());

                        var lhsType = GetType(b.Left);
                        var rhsType = GetType(b.Right);

                        /*
                        var apply = D.Inference.Node.Apply(D.Inference.Node.Variable("operator-symbol"), new[] { 
                            D.Inference.Node.Constant(lhs), // lhs
                            D.Inference.Node.Constant(rhs)  // rhs
                        });


                        var r = flow.Infer(apply);
                        */

                        if (ReferenceEquals(lhsType, rhsType))
                        {
                            return lhsType;
                        }
                        else
                        {
                            return Type.Get(ObjectType.Object);
                        }
                    }


                case UnaryExpression _:
                case IndexAccessExpression _:
                case Symbol _:
                case MatchExpression _:
                    return Type.Get(ObjectType.Object);

                case TypeInitializer initializer:
                    return env.Get<Type>(initializer.Type);

                case ArrayInitializer array:
                    return new Type(ObjectType.Array, (Type)array.ElementType);
            }

            if (expression.Kind is not ObjectType.Object)
            {
                return Type.Get(expression.Kind);
            }

            throw new Exception("Unexpected expression:" + expression.Kind + "/" + expression.ToString());
        }
    }
}