# TMXTools Code Style Guide

This guide documents the preferred coding style and patterns for the TMXTools project.

## Table of Contents

1. [Naming Conventions](#naming-conventions)
2. [Code Organization](#code-organization)
3. [Preferred Patterns](#preferred-patterns)
4. [Error Handling](#error-handling)
5. [Async/Await](#asyncawait)
6. [Documentation](#documentation)
7. [Testing](#testing)

---

## Naming Conventions

### Types

- **Classes, Interfaces, Records, Enums**: `PascalCase`
  ```csharp
  public class FileReader { }
  public interface IFileValidator { }
  public record FileMetadata(string Path, long Size) { }
  public enum FileType { Text, Binary, Image }
  ```

- **Interfaces**: Always prefix with `I`
  ```csharp
  public interface IFileReader { }
  public interface IAsyncOperation { }
  ```

### Members

- **Methods, Properties, Events**: `PascalCase`
  ```csharp
  public void ReadFile() { }
  public string FilePath { get; set; }
  public event EventHandler FileLoaded;
  ```

- **Constants**: `PascalCase`
  ```csharp
  public const int DefaultBufferSize = 4096;
  private const string DefaultPath = ".";
  ```

- **Local Variables, Parameters**: `camelCase`
  ```csharp
  public void Process(string filePath)
  {
      var content = ReadFile(filePath);
      int lineCount = CountLines(content);
  }
  ```

- **Private Fields**: `_camelCase` (underscore prefix)
  ```csharp
  public class FileReader
  {
      private string _filePath = "";
      private readonly object _lockObject = new Lock();
  }
  ```

### Async Methods

Always suffix async methods with `Async`:
```csharp
public async Task ReadFileAsync(string path) { }
public async Task<string> GetContentAsync() { }
public async ValueTask RefreshAsync() { }
```

---

## Code Organization

### File Structure

```csharp
// 1. Using statements (System first, then others alphabetically)
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using TMXTools.Utils;

// 2. Namespace
namespace TMXTools.IO
{
    /// <summary>
    /// Summary documentation.
    /// </summary>
    public class FileReader
    {
        // 3. Constants
        private const int DefaultBufferSize = 4096;

        // 4. Static fields
        

        // 5. Instance fields
        private string _filePath = "";

        // 6. Constructors
        public FileReader()
        {
        }

        // 7. Properties
        public string FilePath
        {
            get => _filePath;
            set => _filePath = value ?? "";
        }

        // 8. Methods (public, then private)
        public async Task<string> ReadAsync()
        {
            // Implementation
        }

        private void Validate()
        {
            // Implementation
        }
    }
}
```

### Method Organization

Order methods logically:
1. Public methods
2. Protected methods
3. Internal methods
4. Private methods

Within each group, order by:
1. Query/read methods
2. Command/write methods
3. Utility/helper methods

---

## Preferred Patterns

### 1. Guard Clauses

Use guard clauses for early returns, avoid deep nesting:

```csharp
// ✅ Good
public bool Process(string? input)
{
    if (input is null || input.Length == 0)
        return false;
    
    // Main logic
}

// ❌ Avoid
public void Process(string? input)
{
    if (input is not null)
    {
        if (input.Length > 0)
        {
            // Main logic
        }
    }
}
```

### 2. Null Checking

Prefer `is null` and `is not null`:

```csharp
// ✅ Good
if (value is null)
    return;

if (value is not null)
    Process(value);

// ⚠️ Acceptable but less preferred
if (value == null)
    return;

// ❌ Avoid
if (!string.IsNullOrEmpty(value))
    Process(value);
```

### 3. Composition Over Inheritance

```csharp
// ❌ Inheritance chains
public class BaseFile { }
public class TextFile : BaseFile { }
public class JsonFile : TextFile { }

// ✅ Composition with interfaces
public interface IFileModel
{
    string FilePath { get; }
    Task SaveAsync();
}

public class FileModelBase : IFileModel
{
    // Common implementation
}

public class TextFile : FileModelBase { }
public class JsonFile : FileModelBase { }
```

### 4. Explicit Error Handling

Always explicitly handle errors, but avoid throwing exceptions:

```csharp
// ✅ Good
try
{
    await ReadFileAsync(path, token);
}
catch (OperationCancelledException)
{
    // Expected when cancelling
}
catch (Exception ex)
{
    logger.Error($"File not found: {path}", ex);
    // The return of the function should indicate an error occured, not throw an exception
}

// ❌ Avoid
try
{
    await ReadFileAsync(path);
}
catch { } // Silent failure
```

### 5. Property Initialization

```csharp
// ✅ Use auto-properties
public string Name { get; set; } = "";
public int Count { get; private set; }

// ✅ Use init-only properties for immutability
public record FileInfo(string Path, long Size);

public class Config
{
    public string Directory { get; init; } = ".";
    public int Timeout { get; init; } = 5000;
}

// ❌ Unnecessary complexity
private string _name = "";
public string Name
{
    get => _name;
    set => _name = value;
}
```

---

## Error Handling

### Exceptions

1. **Indicate error in the return, not by excepting, but always log**
   ```csharp
   public async Task<bool> ProcessAsync(string? path)
   {
       if (string.IsNullOrWhiteSpace(path))
       {
           _logger.LogError("Error Processing: Path cannot be empty");
           return false
       }
       
       // Process
   }
   ```

2. **Null-coalescing for defaults**
   ```csharp
   public async Task<string> ReadAsync(string? path = null)
   {
       var actualPath = path ?? DefaultPath; // "" is a great default too
       return await File.ReadAllTextAsync(actualPath);
   }
   ```

---

## Async/Await

### Best Practices

1. **Async all the way**
   ```csharp
   // ✅ Good
   public async Task ProcessAsync()
   {
       var data = await GetDataAsync();
       var result = await AnalyzeAsync(data);
       return result;
   }
   ```

2. **Use ConfigureAwait(false) in libraries**
   ```csharp
   public async Task<string> ReadAsync(string path)
   {
       return await File.ReadAllTextAsync(path).ConfigureAwait(false);
   }
   ```

3. **Support CancellationToken**
   ```csharp
   public async Task ProcessAsync(
       string path,
       CancellationToken cancellationToken = default)
   {
       var data = await File.ReadAllBytesAsync(path, cancellationToken)
           .ConfigureAwait(false);
       
       cancellationToken.ThrowIfCancellationRequested();
       
       return await ProcessAsync(data, cancellationToken);
   }
   ```

4. **Never block on async**
   ```csharp
   // ❌ Never
   var result = asyncMethod().Result;
   var result = asyncMethod().Wait();
   Task.WaitAll(tasks);
   
   // ✅ Always
   var result = await asyncMethod();
   await Task.WhenAll(tasks);
   ```

---

## Documentation

### XML Documentation

All public APIs must have documentation:

```csharp
/// <summary>
/// Reads file content asynchronously from the specified path.
/// </summary>
/// <param name="filePath">The path to the file to read.</param>
/// <param name="cancellationToken">Token to cancel the operation.</param>
/// <returns>
/// A task representing the asynchronous operation. 
/// The task result contains the file content as bytes.
/// </returns>
/// <exception cref="ArgumentException">
/// Thrown when <paramref name="filePath"/> is null, empty, or whitespace.
/// </exception>
/// <exception cref="FileNotFoundException">
/// Thrown when the file at <paramref name="filePath"/> does not exist.
/// </exception>
/// <exception cref="OperationCanceledException">
/// Thrown when <paramref name="cancellationToken"/> is canceled.
/// </exception>
/// <remarks>
/// This method is asynchronous and will not block the calling thread.
/// </remarks>
/// <example>
/// <code>
/// var bytes = await reader.ReadAsync("file.txt");
/// var content = Encoding.UTF8.GetString(bytes);
/// </code>
/// </example>
public async Task<byte[]> ReadAsync(
    string filePath,
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### Comments

Use comments for **why**, not **what**:

```csharp
// ✅ Good
// Use ReaderWriterLockSlim to allow multiple concurrent readers
// while serializing writes to maintain data consistency
private readonly ReaderWriterLockSlim _lock = new();

// ❌ Avoid
// Initialize the lock
private readonly ReaderWriterLockSlim _lock = new();

// ❌ Avoid
// Increment counter
counter++;  // This is obvious from the code
```

---

## Testing

### Test Structure

Follow the AAA pattern: **Arrange-Act-Assert**

```csharp
[Test]
public async Task ReadAsync_WithValidPath_ReturnsContent()
{
    // Arrange
    const string testPath = "test.txt";
    const string expectedContent = "Hello";
    await File.WriteAllTextAsync(testPath, expectedContent);
    var reader = new FileReader();

    try
    {
        // Act
        var result = await reader.ReadAsync(testPath);

        // Assert
        Assert.That(result, Is.EqualTo(expectedContent));
    }
    finally
    {
        // Cleanup
        if (File.Exists(testPath))
            File.Delete(testPath);
    }
}
```

### Test Naming

Use: `[MethodName]_[Scenario]_[ExpectedOutcome]`

```csharp
[Test]
public void Parse_WithValidInput_ReturnsObject() { }

[Test]
public void Parse_WithNullInput_ThrowsArgumentNullException() { }

[Test]
public async Task LoadAsync_WhenFileMissing_ThrowsFileNotFoundException() { }
```

---

## Summary

Key takeaways:
1. ✅ Use guard clauses for clarity
2. ✅ Prefer `is null` checks
3. ✅ Compose instead of inherit
4. ✅ Handle errors explicitly
5. ✅ Use async/await throughout
6. ✅ Document public APIs
7. ✅ Test all behavior
8. ✅ Support cancellation tokens
9. ✅ Follow naming conventions
10. ✅ Keep it simple and readable
