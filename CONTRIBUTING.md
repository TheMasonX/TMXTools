# Contributing to TMXTools

Thank you for your interest in contributing to TMXTools! This document provides guidelines and instructions for contributing.

## Code of Conduct

Be respectful, inclusive, and professional. We're all here to build something great together.

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 Community Edition (or later)
- Git

### Setup Development Environment

1. Clone the repository:
```bash
git clone https://github.com/TheMasonX/TMXTools.git
cd TMXTools
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

4. Run tests:
```bash
dotnet test
```

## Development Workflow

### Before You Start

1. Check [Issues](https://github.com/TheMasonX/TMXTools/issues) for existing discussions
2. Create a discussion or issue if your change is significant
3. Review the [MODERNIZATION_PLAN.md](./MODERNIZATION_PLAN.md) to understand current priorities

### Creating a Branch

Use descriptive branch names:
```bash
git checkout -b feature/my-new-feature
git checkout -b fix/issue-description
git checkout -b docs/update-readme
```

### Code Style & Standards

The project uses `.editorconfig` for code formatting. Your IDE should automatically apply these settings.

**Key conventions:**
- Use PascalCase for classes, interfaces, methods, and properties
- Use camelCase for local variables and parameters
- Use _camelCase for private fields
- Use `is null` instead of `== null`
- Prefer composition over inheritance
- Use guard clauses for early returns
- Add explicit error handling
- Include XML documentation comments for public APIs

**Example:**
```csharp
/// <summary>
/// Reads the specified file asynchronously.
/// </summary>
/// <param name="filePath">The path to the file to read.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>The file contents as a byte array.</returns>
/// <exception cref="FileNotFoundException">Thrown when the file doesn't exist.</exception>
public async Task<byte[]> ReadFileAsync(string filePath, CancellationToken cancellationToken = default)
{
    if (string.IsNullOrWhiteSpace(filePath))
    {
        throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
    }

    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException($"File not found: {filePath}");
    }

    return await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);
}
```

### Writing Tests

**Test naming convention:** `[MethodName]_[Scenario]_[ExpectedOutcome]`

```csharp
[TestFixture]
public class FileReaderTests
{
    private IFileReader _reader = null!;

    [SetUp]
    public void Setup()
    {
        _reader = new FileReader();
    }

    [Test]
    public async Task ReadFileAsync_WithValidPath_ReturnsContent()
    {
        // Arrange
        const string testFile = "test.txt";
        const string expectedContent = "Hello, World!";
        await File.WriteAllTextAsync(testFile, expectedContent);

        try
        {
            // Act
            var result = await _reader.ReadFileAsync(testFile);

            // Assert
            Assert.That(result, Is.EqualTo(expectedContent));
        }
        finally
        {
            // Cleanup
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
            }
        }
    }

    [Test]
    public void ReadFileAsync_WithNullPath_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _reader.ReadFileAsync(null));
    }
}
```

### Making Changes

1. **Keep commits atomic**: One logical change per commit
2. **Write meaningful commit messages**: Describe what and why, not how
3. **Update documentation**: Keep README and docs in sync with code changes
4. **Add tests**: Ensure new features have corresponding tests
5. **Run tests locally**: `dotnet test`

Good commit message example:
```
Add async file reading support

- Add IFileReader interface
- Implement FileReader class with ReadFileAsync method
- Add comprehensive unit tests
- Update FileUtils with deprecation notice
- Fixes #123
```

### Pull Request Process

1. **Before submitting:**
   - Run `dotnet build` - no build warnings/errors
   - Run `dotnet test` - all tests pass
   - Review your own code first
   - Update any relevant documentation

2. **Submit PR:**
   - Use a descriptive title
   - Reference any related issues (#123)
   - Explain the motivation and changes
   - Include any screenshots/diagrams if applicable
   - Request review from maintainers

3. **Respond to feedback:**
   - Address all review comments
   - Push updates to the same branch
   - Resolve conversations
   - Request re-review

## Project Structure

```
TMXTools/
├── TMXTools/                    # Core library (.NET 8)
│   ├── Utils/                   # Utility classes
│   ├── Extensions/              # Extension methods
│   └── Models/                  # Data models
├── TMXTools.WPF/                # WPF library (.NET 8-Windows)
│   ├── Converters/              # XAML value converters
│   ├── Controls/                # Custom controls
│   ├── Extensions/              # WPF extensions
│   ├── Utils/                   # Utility classes
│   └── Resources/               # XAML resources
├── TMXTools.Tests/              # Core library tests
├── TMXTools.WPF.Tests/          # WPF library tests
├── .github/workflows/           # CI/CD pipelines
├── task_plans/                  # Implementation plans
└── MODERNIZATION_PLAN.md        # Overall modernization roadmap
```

## Testing Requirements

- **Minimum coverage**: 80% for TMXTools, 75% for TMXTools.WPF
- **Test all public APIs**
- **Test error cases**
- **Test async methods** with cancellation tokens
- **Use meaningful assertions**

Run coverage locally:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Performance Considerations

- Avoid blocking calls (use async/await)
- Use `ConfigureAwait(false)` in library code
- Consider memory allocations in hot paths
- Profile before optimizing
- Document performance-critical code

## Documentation Requirements

**All public APIs must have:**
- Summary XML documentation comment
- Parameter descriptions (if applicable)
- Return value description (if applicable)
- Exception documentation
- Usage examples (for complex APIs)

## Modernization Priorities

The project is prioritizing:

1. **Unit testing** - Comprehensive test coverage
2. **CI/CD automation** - Reliable build and release pipeline
3. **Async/await patterns** - Modern async code
4. **Interface-based design** - Dependency injection ready
5. **File model improvements** - Type-safe file handling
6. **Advanced WPF features** - Drawing, undo/redo, etc.

See [MODERNIZATION_PLAN.md](./MODERNIZATION_PLAN.md) for details.

## Questions or Need Help?

- Open an [issue](https://github.com/TheMasonX/TMXTools/issues) with a question label
- Start a [discussion](https://github.com/TheMasonX/TMXTools/discussions)
- Check existing documentation in task_plans/

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Recognition

Contributors will be recognized in:
- Release notes
- GitHub contributors page
- Project README (for significant contributions)

Thank you for contributing to TMXTools! 🎉
