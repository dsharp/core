﻿namespace D.Expressions
{
    public class ForStatement : IExpression
    {
        public ForStatement(IExpression variable, IExpression generator, BlockStatement body)
        {
            VariableExpression = variable;
            GeneratorExpression = generator;
            Body = body;
        }

        // name | tuple pattern
        //  x   |    (x, x)
        public IExpression VariableExpression { get; set; }

        // variable |  range
        //    c     | 1...100
        public IExpression GeneratorExpression { get; }

        public BlockStatement Body { get; }

        Kind IObject.Kind => Kind.ForStatement;
    }
}