# Quick Start Guide - TMXTools Modernization

Get started with the modernized TMXTools project in 5 minutes.

## Prerequisites

- .NET 8 SDK installed
- Visual Studio 2022 or VS Code
- Git

## First-Time Setup

### 1. Clone & Build (1 min)

```bash
git clone https://github.com/TheMasonX/TMXTools.git
cd TMXTools
dotnet restore
dotnet build
```

### 2. Run Tests (1 min)

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### 3. Enable EditorConfig (1 min)

**Visual Studio:**
- ✅ Automatically enabled (built-in)
- Tools → Options → Text Editor → C# → Code Style → Formatting → "Code style rules"

**VS Code:**
- Install "EditorConfig for VS Code" extension
- Restart VS Code

**Rider:**
- ✅ Automatically enabled (built-in)

## Understanding the Project

### Read These First

**5 minutes:**
- `IMPLEMENTATION_SUMMARY.md` - Overview of what's been done
- `STYLE_GUIDE.md` - Coding standards (quick scan)

**15 minutes:**
- `MODERNIZATION_PLAN.md` - Full roadmap
- `CONTRIBUTING.md` - How to contribute

**Reference as needed:**
- `.copilot-instructions.md` - During development
- `task_plans/` - For feature-specific guidance

### Project Structure

```
TMXTools/                  ← Core library
├── Utils/                 ← Utility classes
├── Extensions/            ← Extension methods
└── TMXTools.csproj

TMXTools.WPF/              ← WPF library
├── Converters/
├── Controls/
├── Extensions/
├── Utils/
└── TMXTools.WPF.csproj

TMXTools.Tests/            ← Core tests (NEW - to create)
TMXTools.WPF.Tests/        ← WPF tests (existing)
```

## Common Tasks

### Create a Test File

```csharp
// File: TMXTools.Tests/Utils/FileUtilsTests.cs
using NUnit.Framework;
using TMXTools.Utils;

namespace TMXTools.Tests.Utils;

[TestFixture]
public class FileUtilsTests
{
    [Test]
    public void FileExists_WithExistingFile_ReturnsTrue()
    {
        // Arrange
        const string testFile = "test.txt";
        File.WriteAllText(testFile, "content");

        try
        {
            // Act
            var result = FileUtils.FileExists(testFile);

            // Assert
            Assert.That(result, Is.True);
        }
        finally
        {
            if (File.Exists(testFile))
                File.Delete(testFile);
        }
    }
}
```

### Create a Feature

1. **Plan it**: Check task_plans/ for guidance
2. **Create interfaces**: Use ISomething pattern
3. **Implement**: Inherit from base/abstract class
4. **Test it**: Follow [Method]_[Scenario]_[Expected] pattern
5. **Document it**: Add XML comments
6. **Open PR**: Reference related issues

### Run Specific Tests

```bash
# Test a specific class
dotnet test --filter "ClassName=FileUtilsTests"

# Test with pattern matching
dotnet test --filter "FullyQualifiedName~FileUtilsTests"

# Run async tests
dotnet test --filter "MethodName=ReadFileAsync*"

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

### Check Code Quality

```bash
# Build with style analysis
dotnet build /p:EnforceCodeStyleInBuild=true

# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Analyze specific project
dotnet analyze TMXTools/TMXTools.csproj
```

## Key Principles to Remember

✅ **Do:**
- Use async/await for I/O
- Include cancellation tokens
- Document public APIs
- Write tests before features
- Use guard clauses
- Handle errors explicitly
- Check `is null`, not `== null`
- Support dependency injection

❌ **Don't:**
- Block async code with `.Result`
- Silently catch exceptions
- Create `async void` (except handlers)
- Use static methods for dependencies
- Leave TODO comments
- Skip tests
- Nest too deeply
- Ignore cancellation tokens

## Code Review Checklist

Before submitting a PR, verify:

- [ ] `dotnet build` succeeds
- [ ] `dotnet test` passes
- [ ] Coverage didn't decrease
- [ ] No style violations (run EditorConfig check)
- [ ] Public APIs documented with XML comments
- [ ] Error handling is explicit
- [ ] Uses async/await for I/O
- [ ] Follows naming conventions
- [ ] No blocking async calls

## Getting Help

### Documentation
- **Coding Standards**: See `STYLE_GUIDE.md`
- **How to Contribute**: See `CONTRIBUTING.md`
- **Project Roadmap**: See `MODERNIZATION_PLAN.md`
- **Feature Plans**: See `task_plans/`
- **AI Guidance**: See `.copilot-instructions.md`

### Common Issues

**Tests not running:**
```bash
# Make sure test framework is installed
dotnet add TMXTools.Tests package NUnit
dotnet add TMXTools.Tests package Microsoft.NET.Test.Sdk
dotnet add TMXTools.Tests package NUnit3TestAdapter
```

**EditorConfig not working:**
```bash
# VS Code: Restart
# Visual Studio: Clean cache
# Rider: File → Invalidate Caches
```

**Build errors:**
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

## Next Steps

1. **Read** `STYLE_GUIDE.md` (10 min)
2. **Create** `TMXTools.Tests` project if not exists
3. **Write** tests for one utility class
4. **Run** tests and verify coverage
5. **Read** a task plan for your feature
6. **Implement** following the guidelines
7. **Submit** PR with tests and documentation

## Useful Commands

```bash
# Development
dotnet watch build                    # Auto-rebuild on changes
dotnet watch test                     # Auto-test on changes

# Testing
dotnet test --no-build                # Skip rebuild
dotnet test --filter "TestName"       # Run specific test
dotnet test --verbosity detailed      # Full output

# Building
dotnet build -c Release               # Release configuration
dotnet build /p:WarningLevel=4        # Strict warnings

# Cleanup
dotnet clean                          # Remove build artifacts
dotnet nuget locals all --clear       # Clear NuGet cache
```

## Architecture Quick Reference

### Current Focus Areas

**Phase 1: Foundation (Current)**
- ✅ Testing infrastructure
- ✅ CI/CD pipelines
- ⏳ Code quality baseline

**Phase 2: Modernization**
- ⏳ Async/await throughout
- ⏳ Interface-based design
- ⏳ Null safety

**Phase 3: Features**
- ⏳ File model improvements
- ⏳ Undo/redo system
- ⏳ Drawing libraries

**Phase 4: Documentation**
- ⏳ API documentation
- ⏳ Architecture docs
- ⏳ Tutorials

## Standards at a Glance

### Naming
```csharp
public class MyClass { }               // PascalCase
public interface IMyInterface { }       // I prefix
public void MyMethod() { }              // PascalCase
private string _myField = "";           // _camelCase
public async Task DoAsync() { }         // Async suffix
```

### Patterns
```csharp
// Guard clause
if (string.IsNullOrEmpty(input))
    return;

// Null check
if (value is null)
    return;

// Error handling
try
{
    await operation();
}
catch (Exception ex)
{
    throw new InvalidOperationException("Failed", ex);
}

// Async with cancellation
public async Task DoAsync(CancellationToken ct = default)
{
    await something().ConfigureAwait(false);
    ct.ThrowIfCancellationRequested();
}
```

## Need More Details?

- **What to build**: See `MODERNIZATION_PLAN.md`
- **How to build it**: See `task_plans/[FEATURE]_PLAN.md`
- **Code style**: See `STYLE_GUIDE.md`
- **AI help**: See `.copilot-instructions.md`
- **How to contribute**: See `CONTRIBUTING.md`

---

**Ready to contribute?** Start with the [Contributing Guide](CONTRIBUTING.md)! 🚀
