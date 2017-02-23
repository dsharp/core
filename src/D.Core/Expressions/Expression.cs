﻿namespace D.Expressions
{
    public static class Expression
    {
        public static CallExpression Call(Symbol name, IArguments arguments)
           => new CallExpression(null, name, arguments, false);

        public static Parameter Parameter(string name)
            => new Parameter(name);

        #region Logic

        public static BinaryExpression And (IExpression x, IExpression y)
           => new BinaryExpression(Operator.LogicalAnd, x, y);

        public static BinaryExpression Or(IExpression x, IExpression y)
            => new BinaryExpression(Operator.LogicalOr, x, y);

        public static UnaryExpression Not(IExpression x)
            => new UnaryExpression(Operator.Not, x);

        #endregion

        #region Arthimetic

        public static IExpression GreaterThan(IExpression x, IExpression y)
          => new BinaryExpression(Operator.GreaterThan, x, y);

        public static IExpression GreaterThanOrEqual (IExpression x, IExpression y)
            => new BinaryExpression(Operator.GreaterOrEqual, x, y);

        public static IExpression Less (IExpression x, IExpression y)
         => new BinaryExpression(Operator.LessThan, x, y);

        public static IExpression LessThan (IExpression x, IExpression y)
            => new BinaryExpression(Operator.LessOrEqual, x, y);

        #endregion
    }
}