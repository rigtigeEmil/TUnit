﻿namespace TUnit.Assertions.AssertConditions.Connectors;

public sealed class AssertConditionAnd<TActual> : BaseAssertCondition<TActual>
{
    private readonly BaseAssertCondition<TActual> _condition1;
    private readonly BaseAssertCondition<TActual> _condition2;

    public AssertConditionAnd(BaseAssertCondition<TActual> condition1, BaseAssertCondition<TActual> condition2)
    {
        _condition1 = condition1;
        _condition2 = condition2;
    }

    protected internal override string Message =>
        !_condition1.Passes(ActualValue) ? _condition1.Message :
        !_condition2.Passes(ActualValue) ? _condition2.Message : string.Empty;

    protected override string DefaultMessage => string.Empty;
    
    protected internal override bool Passes(TActual? actualValue)
    {
        return _condition1.Assert(actualValue) && _condition2.Assert(actualValue);
    }
}