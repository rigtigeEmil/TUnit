﻿namespace TUnit.Engine.SourceGenerator.Models.Arguments;

internal record Argument
{
    public Argument(string type, string? invocation)
    {
        Type = type;
        Invocation = invocation ?? "null";
    }

    public string Type { get; }
    public string Invocation { get; } 
}