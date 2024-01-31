using TUnit.Assertions.AssertConditions.Operators;

namespace TUnit.Assertions.AssertConditions;

public abstract class BaseAssertCondition<TActual>
{
    internal BaseAssertCondition()
    {
        And = new And<TActual>(this);
        Or = new Or<TActual>(this);
    }
    
    protected TActual? ActualValue { get; set; } = default!;
    
    protected internal abstract string Message { get; }
    
    protected abstract string DefaultMessage { get; }
    
    internal bool Assert(TActual? actualValue)
    {
        ActualValue = actualValue;
        return Passes(actualValue);
    }

    protected internal abstract bool Passes(TActual? actualValue);

    public And<TActual> And { get; }
    public Or<TActual> Or { get; }
}