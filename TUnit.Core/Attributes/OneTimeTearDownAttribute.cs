﻿namespace TUnit.Core.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class OneTimeTearDownAttribute : TUnitAttribute;