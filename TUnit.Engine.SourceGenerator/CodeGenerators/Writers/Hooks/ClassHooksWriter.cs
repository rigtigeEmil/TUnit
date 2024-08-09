﻿using Microsoft.CodeAnalysis;
using TUnit.Engine.SourceGenerator.CodeGenerators.Helpers;
using TUnit.Engine.SourceGenerator.Enums;
using TUnit.Engine.SourceGenerator.Models;

namespace TUnit.Engine.SourceGenerator.CodeGenerators.Writers.Hooks;

internal static class ClassHooksWriter
{
    public static void Execute(SourceProductionContext context, HooksDataModel model, HookType hookType)
    {
        var className = $"ClassHooks_{model.MinimalTypeName}";
        var fileName = $"{className}_{Guid.NewGuid():N}";

        using var sourceBuilder = new SourceCodeWriter();
                
        sourceBuilder.WriteLine("// <auto-generated/>");
        sourceBuilder.WriteLine("using global::System.Linq;");
        sourceBuilder.WriteLine("using global::System.Reflection;");
        sourceBuilder.WriteLine("using global::System.Runtime.CompilerServices;");
        sourceBuilder.WriteLine("using global::TUnit.Core;");
        sourceBuilder.WriteLine("using global::TUnit.Core.Interfaces;");
        sourceBuilder.WriteLine("using global::TUnit.Engine;");
        sourceBuilder.WriteLine("using global::TUnit.Engine.Helpers;");
        sourceBuilder.WriteLine("using global::TUnit.Engine.Hooks;");
        sourceBuilder.WriteLine();
        sourceBuilder.WriteLine("namespace TUnit.Engine;");
        sourceBuilder.WriteLine();
        sourceBuilder.WriteLine("[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sourceBuilder.WriteLine($"file partial class {className}");
        sourceBuilder.WriteLine("{");
        sourceBuilder.WriteLine("[ModuleInitializer]");
        sourceBuilder.WriteLine("public static void Initialise()");
        sourceBuilder.WriteLine("{");

        if (hookType == HookType.SetUp)
        {
            sourceBuilder.WriteLine(
                $$"""
                  ClassHookOrchestrator.RegisterSetUp(typeof({{model.FullyQualifiedTypeName}}), new StaticMethod<ClassHookContext>
                  		{ 
                             MethodInfo = typeof({{model.FullyQualifiedTypeName}}).GetMethod("{{model.MethodName}}", 0, [{{string.Join(", ", model.ParameterTypes.Select(x => $"typeof({x})"))}}]),
                             Body = (context, cancellationToken) => AsyncConvert.Convert(() => {{model.FullyQualifiedTypeName}}.{{model.MethodName}}({{GetArgs(model)}})),
                             HookExecutor = {{HookExecutorHelper.GetHookExecutor(model.HookExecutor)}},
                  		});
                  """);
        }
        else if (hookType == HookType.CleanUp)
        {
            sourceBuilder.WriteLine(
                $$"""
                 ClassHookOrchestrator.RegisterCleanUp(typeof({{model.FullyQualifiedTypeName}}), new StaticMethod<ClassHookContext>
                 		{ 
                             MethodInfo = typeof({{model.FullyQualifiedTypeName}}).GetMethod("{{model.MethodName}}", 0, [{{string.Join(", ", model.ParameterTypes.Select(x => $"typeof({x})"))}}]),
                             Body = (context, cancellationToken) => AsyncConvert.Convert(() => {{model.FullyQualifiedTypeName}}.{{model.MethodName}}({{GetArgs(model)}})),
                             HookExecutor = {{HookExecutorHelper.GetHookExecutor(model.HookExecutor)}},
                 		});
                 """);
        }

        sourceBuilder.WriteLine("}");
        sourceBuilder.WriteLine("}");

        context.AddSource($"{fileName}.Generated.cs", sourceBuilder.ToString());
    }

    private static string GetArgs(HooksDataModel model)
    {
        List<string> args = [];
        
        foreach (var type in model.ParameterTypes)
        {
            if (type == WellKnownFullyQualifiedClassNames.ClassHookContext.WithGlobalPrefix)
            {
                args.Add("context");
            }
            
            if (type == WellKnownFullyQualifiedClassNames.CancellationToken.WithGlobalPrefix)
            {
                args.Add("cancellationToken");
            }
        }

        return string.Join(", ", args);
    }
}