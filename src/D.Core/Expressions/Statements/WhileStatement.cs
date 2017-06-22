﻿namespace D.Expressions
{
    // rust: loop { } 
    // swift: repeat { } while condition, while condition

    // onIteration  (i++)
    // precondition
    // postcondition
    public class WhileStatement : IExpression
    {
        public WhileStatement(IExpression condition, BlockExpression body)
        {
            Condition = condition;
            Body = body;
        }

        public IExpression Condition { get; }

        public BlockExpression Body { get; }

        Kind IObject.Kind => Kind.WhileStatement;
    }
}