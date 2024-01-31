﻿namespace TUnit.Assertions.AssertConditions.Throws;

public abstract class DelegateAssertCondition<T>
{
    private Func<T?, Exception?, string>? MessageFactory { get; set; }

    protected Exception? Exception { get; set; }
    protected T? ActualValue { get; set; } = default!;

    public bool Assert(DelegateInvocationResult<T> delegateInvocationResult)
    {
        ActualValue = delegateInvocationResult.Result;
        Exception = delegateInvocationResult.Exception;
        
        return Passes(ActualValue, Exception);
    }

    protected abstract string DefaultMessage { get; }

    protected internal abstract bool Passes(T? actualValue, Exception? exception);

    public string Message => MessageFactory?.Invoke(ActualValue, Exception) ?? DefaultMessage;
    
    public DelegateAssertCondition<T> WithMessage(Func<T?, Exception?, string> messageFactory)
    {
        MessageFactory = messageFactory;
        return this;
    }
}

public abstract class DelegateAssertCondition
{
    private Func<Exception?, string>? MessageFactory { get; set; }

    protected Exception? Exception { get; private set; }

    public bool Assert(Exception? exception)
    {
        Exception = exception;
        
        return Passes(Exception);
    }

    protected abstract string DefaultMessage { get; }

    protected internal abstract bool Passes(Exception? exception);

    public string Message => MessageFactory?.Invoke(Exception) ?? DefaultMessage;
    
    public DelegateAssertCondition WithMessage(Func<Exception?, string> messageFactory)
    {
        MessageFactory = messageFactory;
        return this;
    }
}