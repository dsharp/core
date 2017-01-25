﻿using System;
using System.Linq;

using Xunit;

namespace D.Parsing.Tests
{
    using Syntax;

    public class CallTests : TestBase
    {
        [Fact]
        public void Parethensis()
        {
            var a = Parse<CallExpressionSyntax>("(a - b).negate(1)");

            var left = (BinaryExpressionSyntax)a.Callee;

            Assert.Equal(Operator.Subtraction , left.Operator);
            Assert.Equal("negate"             , a.FunctionName);
            Assert.Equal(1                    , (NumberLiteral)a.Arguments[0].Value);
        }

        [Fact]
        public void CeilingAndFloor()
        {
            Assert.Equal("ceiling", Parse<CallExpressionSyntax>("ceiling(5.5)").FunctionName);
            Assert.Equal("floor",   Parse<CallExpressionSyntax>("floor(5.9)").FunctionName);

        }
        [Fact]
        public void Lambda()
        {
            var call = Parse<CallExpressionSyntax>("hi(a => a * 2, b, c)");

            var lambda = (FunctionDeclarationSyntax)call.Arguments[0].Value;

            Assert.True(lambda.IsAnonymous);
            Assert.Equal("a",           lambda.Parameters[0].Name);
            Assert.Equal(Kind.MultiplyExpression, lambda.Body.Kind);

            Assert.Equal("b", call.Arguments[1].Value.ToString());
        }

        [Fact]
        public void Call()
        {
            var call = Parse<CallExpressionSyntax>("run(x, y, z)");

            Assert.Equal("run", call.FunctionName);

            Assert.Equal(3, call.Arguments.Length);

            Assert.Equal("x", (Symbol)call.Arguments[0].Value);
            Assert.Equal("y", (Symbol)call.Arguments[1].Value);
            Assert.Equal("z", (Symbol)call.Arguments[2].Value);
        }

        [Fact]
        public void CallNamed()
        {
            var call = Parse<CallExpressionSyntax>("move(x: 1, y: 2, z: 3)");

            Assert.Equal("move", call.FunctionName);

            Assert.Equal(3, call.Arguments.Length);

            Assert.Equal(1, (NumberLiteral)call.Arguments[0].Value);
            Assert.Equal(2, (NumberLiteral)call.Arguments[1].Value);
            Assert.Equal(3, (NumberLiteral)call.Arguments[2].Value);

            var args = call.Arguments.ToArray();

            Assert.Equal(1, (NumberLiteral)args[0].Value);
            Assert.Equal(2, (NumberLiteral)args[1].Value);
            Assert.Equal(3, (NumberLiteral)args[2].Value);

            Assert.Equal("x", args[0].Name);
            Assert.Equal("y", args[1].Name);
            Assert.Equal("z", args[2].Name);
        }

        [Fact]
        public void IndexAccess()
        {
            var call = Parse<CallExpressionSyntax>("move(x: 1, y: 2, z: 3)");

            // Assert.Equal(1, (NumberLiteral)call.Arguments["x"]);
            // Assert.Equal(2, (NumberLiteral)call.Arguments["y"]);
            // Assert.Equal(3, (NumberLiteral)call.Arguments["z"]);

            // Assert.Throws<Exception>(() => call.Arguments["a"]);
        }
    }
}