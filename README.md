# TUnit

<a href="https://trendshift.io/repositories/11781" target="_blank"><img src="https://trendshift.io/api/badge/repositories/11781" alt="thomhurst%2FTUnit | Trendshift" style="width: 250px; height: 55px;" width="250" height="55"/></a>

A modern, flexible and fast testing framework for .NET 8 and up. With Native AOT and Trimmed Single File application support included! 

TUnit is designed to aid with all testing types:
- Unit
- Integration
- Acceptance
- and more!


[![nuget](https://img.shields.io/nuget/v/TUnit.svg)](https://www.nuget.org/packages/TUnit/) ![Nuget](https://img.shields.io/nuget/dt/TUnit) ![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/thomhurst/TUnit/dotnet.yml) ![GitHub last commit (branch)](https://img.shields.io/github/last-commit/thomhurst/TUnit/main) ![License](https://img.shields.io/github/license/thomhurst/TUnit) 

## Documentation

See here: <https://thomhurst.github.io/TUnit/>

## IDE

TUnit is built on top of the newer Microsoft.Testing.Platform, as opposed to the older VSTest platform. Because the infrastructure behind the scenes is new and different, you may need to enable some settings. This should just be a one time thing.

### Visual Studio

Visual Studio is supported on the Preview version currently. 

- Install the [latest preview version](https://visualstudio.microsoft.com/vs/preview/)
- Open Visual Studio and go to Tools > Manage Preview Features
- Enable "Use testing platform server mode"

<img src="/docs/static/img/visual-studio.png" height="300px">

### Rider

Rider is supported. The [Enable Testing Platform support](https://www.jetbrains.com/help/rider/Reference__Options__Tools__Unit_Testing__VSTest.html) option must be selected in Settings > Build, Execution, Deployment > Unit Testing > VSTest.

<img src="/docs/static/img/rider.png" height="300px">

## VS Code
Visual Studio Code is supported.

- Install the extension Name: [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- Go to the C# Dev Kit extension's settings
- Enable Dotnet > Test Window > Use Testing Platform Protocol

<img src="/docs/static/img/visual-studio-code.png" height="300px">

### CLI
`dotnet` CLI - Fully supported. Tests should be runnable with `dotnet test`, `dotnet run`, `dotnet exec` or executing an executable directly. See the docs for more information!

## Features

- Native AOT / Trimmed Single File application support
- Source generated tests
- Property injection
- Full async support
- Parallel by default, with mechanisms to:
    - Run specific tests completely on their own
    - Run specific tests not in parallel with other specific tests
    - Limit the parallel limit on a per-test, class or assembly level
- Tests can depend on other tests to form chains, useful for if one test depends on state from another action. While not recommended for unit tests, this can be useful in integration testing where state matters
- Easy to read assertions - though you're also free to use whichever assertion library you like
- Injectable test data via classes, methods, compile-time args, or matrices
- Hooks before and after: 
    - TestDiscover
    - TestSession
    - Assembly
    - Class
    - Test
- Designed to avoid common pitfalls such as leaky test states
- Dependency injection support ([See here](https://thomhurst.github.io/TUnit/docs/tutorial-extras/class-constructors))
- Ability to view and interrogate metadata and results from various assembly/class/test context objects

## Installation

`dotnet add package TUnit --prerelease`

## Example test

```csharp
    private static readonly TimeOnly Midnight = TimeOnly.FromTimeSpan(TimeSpan.Zero);
    private static readonly TimeOnly Noon = TimeOnly.FromTimeSpan(TimeSpan.FromHours(12));
    
    [Test]
    public async Task IsMorning()
    {
        var time = GetTime();

        await Assert.That(time).IsAfterOrEqualTo(Midnight)
            .And.IsBefore(Noon);
    }
```

or with more complex test orchestration needs

```csharp
    [Before(Class)]
    public static async Task ClearDatabase(ClassHookContext context) { ... }

    [After(Class)]
    public static async Task AssertDatabaseIsAsExpected(ClassHookContext context) { ... }

    [Before(Test)]
    public async Task CreatePlaywrightBrowser(TestContext context) { ... }

    [After(Test)]
    public async Task DisposePlaywrightBrowser(TestContext context) { ... }

    [Retry(3)]
    [Test, DisplayName("Register an account")]
    [MethodData(nameof(GetAuthDetails))]
    public async Task Register(string username, string password) { ... }

    [Repeat(5)]
    [Test, DependsOn(nameof(Register))]
    [MethodData(nameof(GetAuthDetails))]
    public async Task Login(string username, string password) { ... }

    [Test, DependsOn(nameof(Login), [typeof(string), typeof(string)])]
    [MethodData(nameof(GetAuthDetails))]
    public async Task DeleteAccount(string username, string password) { ... }

    [Category("Downloads")]
    [Timeout(300_000)]
    [Test, NotInParallel(Order = 1)]
    public async Task DownloadFile1() { ... }

    [Category("Downloads")]
    [Timeout(300_000)]
    [Test, NotInParallel(Order = 2)]
    public async Task DownloadFile2() { ... }

    [Repeat(10)]
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [DisplayName("Go to the page numbered $page")]
    public async Task GoToPage(int page) { ... }

    [Category("Cookies")]
    [Test, Skip("Not yet built!")]
    public async Task CheckCookies() { ... }

    [Test, Explicit, WindowsOnlyTest, RetryHttpServiceUnavailable(5)]
    [Property("Some Key", "Some Value")]
    public async Task Ping() { ... }

    [Test]
    [ParallelLimit<LoadTestParallelLimit>]
    [Repeat(1000)]
    public async Task LoadHomepage() { ... }

    public static IEnumerable<(string Username, string Password)> GetAuthDetails()
    {
        yield return ("user1", "password1");
        yield return ("user2", "password2");
        yield return ("user3", "password3");
    }

    public class WindowsOnlyTestAttribute : SkipAttribute
    {
        public WindowsOnlyTestAttribute() : base("Windows only test")
        {
        }

        public override Task<bool> ShouldSkip(TestContext testContext)
        {
            return Task.FromResult(!OperatingSystem.IsWindows());
        }
    }

    public class RetryHttpServiceUnavailableAttribute : RetryAttribute
    {
        public RetryHttpServiceUnavailableAttribute(int times) : base(times)
        {
        }

        public override Task<bool> ShouldRetry(TestInformation testInformation, Exception exception, int currentRetryCount)
        {
            return Task.FromResult(exception is HttpRequestException { StatusCode: HttpStatusCode.ServiceUnavailable });
        }
    }

    public class LoadTestParallelLimit : IParallelLimit
    {
        public int Limit => 50;
    }
```

## Motivations

TUnit is inspired by NUnit and xUnit - two of the most popular testing frameworks for .NET.

It aims to build upon the useful features of both while trying to address any pain points that they may have.

[Read more here](https://thomhurst.github.io/TUnit/docs/comparison/framework-differences)

## Prerelease

You'll notice that version 1.0 isn't out yet. While this framework is mostly feature complete, I'm waiting for a few things:

- Full Rider support for all features
- Full VS support for all features
- Open to feedback on existing features
- Open to ideas on new features

As such, the API may change. I'll try to limit this but it's a possibility.

## Benchmark

### Scenario: Building the test project

#### macos-latest

```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100-rc.1.24452.12
  [Host]   : .NET 9.0.0 (9.0.24.43107), Arm64 RyuJIT AdvSIMD
  .NET 8.0 : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method       | Mean     | Error    | StdDev   |
|------------- |---------:|---------:|---------:|
| Build_TUnit  | 872.6 ms | 16.96 ms | 22.64 ms |
| Build_NUnit  | 791.5 ms |  9.00 ms |  8.42 ms |
| Build_xUnit  | 794.5 ms |  9.59 ms |  8.97 ms |
| Build_MSTest | 834.8 ms | 14.28 ms | 21.37 ms |



#### ubuntu-latest

```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100-rc.1.24452.12
  [Host]   : .NET 9.0.0 (9.0.24.43107), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method       | Mean    | Error    | StdDev   |
|------------- |--------:|---------:|---------:|
| Build_TUnit  | 1.605 s | 0.0298 s | 0.0279 s |
| Build_NUnit  | 1.463 s | 0.0128 s | 0.0120 s |
| Build_xUnit  | 1.465 s | 0.0119 s | 0.0111 s |
| Build_MSTest | 1.531 s | 0.0104 s | 0.0097 s |



#### windows-latest

```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2700) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100-rc.1.24452.12
  [Host]   : .NET 9.0.0 (9.0.24.43107), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method       | Mean    | Error    | StdDev   |
|------------- |--------:|---------:|---------:|
| Build_TUnit  | 1.723 s | 0.0291 s | 0.0346 s |
| Build_NUnit  | 1.575 s | 0.0292 s | 0.0273 s |
| Build_xUnit  | 1.583 s | 0.0292 s | 0.0228 s |
| Build_MSTest | 1.630 s | 0.0325 s | 0.0319 s |


### Scenario: A single test that completes instantly (including spawning a new process and initialising the test framework)

#### macos-latest

```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100-rc.1.24452.12
  [Host]   : .NET 9.0.0 (9.0.24.43107), Arm64 RyuJIT AdvSIMD
  .NET 8.0 : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method    | Mean      | Error     | StdDev    |
|---------- |----------:|----------:|----------:|
| TUnit_AOT |  68.84 ms |  0.484 ms |  0.404 ms |
| TUnit     | 424.49 ms |  5.217 ms |  4.880 ms |
| NUnit     | 706.54 ms |  5.344 ms |  4.737 ms |
| xUnit     | 692.50 ms | 12.383 ms | 10.977 ms |
| MSTest    | 623.48 ms |  9.478 ms |  7.915 ms |



#### ubuntu-latest

```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100-rc.1.24452.12
  [Host]   : .NET 9.0.0 (9.0.24.43107), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method    | Mean        | Error     | StdDev    |
|---------- |------------:|----------:|----------:|
| TUnit_AOT |    39.15 ms |  1.494 ms |  4.406 ms |
| TUnit     |   773.22 ms | 14.951 ms | 15.354 ms |
| NUnit     | 1,352.68 ms | 13.526 ms | 12.652 ms |
| xUnit     | 1,336.49 ms | 12.047 ms | 11.268 ms |
| MSTest    | 1,204.24 ms |  7.689 ms |  6.817 ms |



#### windows-latest

```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2700) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100-rc.1.24452.12
  [Host]   : .NET 9.0.0 (9.0.24.43107), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method    | Mean        | Error     | StdDev    |
|---------- |------------:|----------:|----------:|
| TUnit_AOT |    78.63 ms |  1.188 ms |  1.414 ms |
| TUnit     |   766.43 ms | 15.142 ms | 24.017 ms |
| NUnit     | 1,311.79 ms | 12.504 ms | 11.084 ms |
| xUnit     | 1,290.60 ms | 10.468 ms |  9.792 ms |
| MSTest    | 1,182.05 ms | 15.090 ms | 14.115 ms |


### Scenario: A test that takes 50ms to execute, repeated 100 times (including spawning a new process and initialising the test framework)

#### macos-latest

```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100-rc.1.24452.12
  [Host]   : .NET 9.0.0 (9.0.24.43107), Arm64 RyuJIT AdvSIMD
  .NET 8.0 : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method    | Mean        | Error     | StdDev    |
|---------- |------------:|----------:|----------:|
| TUnit_AOT |    243.4 ms |  11.73 ms |  34.59 ms |
| TUnit     |    595.5 ms |  20.91 ms |  61.67 ms |
| NUnit     | 14,145.9 ms | 280.87 ms | 491.92 ms |
| xUnit     | 14,566.1 ms | 291.21 ms | 620.59 ms |
| MSTest    | 14,438.9 ms | 282.33 ms | 501.84 ms |



#### ubuntu-latest

```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100-rc.1.24452.12
  [Host]   : .NET 9.0.0 (9.0.24.43107), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method    | Mean        | Error     | StdDev    |
|---------- |------------:|----------:|----------:|
| TUnit_AOT |    92.28 ms |  1.861 ms |  5.487 ms |
| TUnit     |   837.00 ms | 16.298 ms | 20.015 ms |
| NUnit     | 6,326.99 ms | 18.509 ms | 16.408 ms |
| xUnit     | 6,373.32 ms | 16.481 ms | 15.416 ms |
| MSTest    | 6,308.73 ms | 18.206 ms | 17.030 ms |



#### windows-latest

```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2700) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100-rc.1.24452.12
  [Host]   : .NET 9.0.0 (9.0.24.43107), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method    | Mean       | Error    | StdDev   |
|---------- |-----------:|---------:|---------:|
| TUnit_AOT |   141.8 ms |  2.83 ms |  3.14 ms |
| TUnit     |   882.9 ms | 17.63 ms | 27.45 ms |
| NUnit     | 7,588.3 ms | 17.75 ms | 15.73 ms |
| xUnit     | 7,570.2 ms | 29.45 ms | 27.55 ms |
| MSTest    | 7,508.2 ms | 11.21 ms |  9.94 ms |



