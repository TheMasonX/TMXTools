# Interface-Based Design & Abstraction Plan

**Status**: Planned  
**Priority**: High  
**Impact**: Architecture, testability, dependency injection  

## Overview

Refactor TMXTools and TMXTools.WPF to use interface-based design patterns, improving testability, maintainability, and enabling dependency injection.

---

## Key Principles

### 1. Interface Segregation Principle (ISP)
- Create small, focused interfaces
- Clients shouldn't depend on methods they don't use
- Example: `IFileReader`, `IFileWriter`, `IFileValidator` instead of monolithic `IFileService`

### 2. Dependency Inversion Principle (DIP)
- Depend on abstractions, not concretions
- High-level modules shouldn't depend on low-level modules
- Both should depend on abstractions

### 3. Single Responsibility Principle (SRP)
- One reason to change per class
- Clear, focused interfaces
- Easy to understand and maintain

---

## TMXTools Core Library Interfaces

### File Operations

```csharp
/// <summary>
/// Interface for reading file content.
/// </summary>
public interface IFileReader
{
    /// <summary>
    /// Reads all bytes from a file asynchronously.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The file content as bytes.</returns>
    Task<byte[]> ReadAllBytesAsync(string filePath);

    /// <summary>
    /// Reads all text from a file asynchronously.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The file content as text.</returns>
    Task<string> ReadAllTextAsync(string filePath);

    /// <summary>
    /// Reads lines from a file asynchronously.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>An enumerable of file lines.</returns>
    IAsyncEnumerable<string> ReadLinesAsync(string filePath);
}

/// <summary>
/// Interface for writing file content.
/// </summary>
public interface IFileWriter
{
    /// <summary>
    /// Writes bytes to a file asynchronously.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="content">The content to write.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAllBytesAsync(string filePath, byte[] content);

    /// <summary>
    /// Writes text to a file asynchronously.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="content">The content to write.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAllTextAsync(string filePath, string content);
}

/// <summary>
/// Interface for file validation and inspection.
/// </summary>
public interface IFileValidator
{
    /// <summary>
    /// Determines whether a file exists.
    /// </summary>
    /// <param name="filePath">The path to check.</param>
    /// <returns>True if the file exists; otherwise, false.</returns>
    bool FileExists(string filePath);

    /// <summary>
    /// Gets the size of a file in bytes.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The file size in bytes, or 0 if the file doesn't exist.</returns>
    long GetFileSizeBytes(string filePath);

    /// <summary>
    /// Validates that a file path is valid.
    /// </summary>
    /// <param name="filePath">The path to validate.</param>
    /// <returns>True if the path is valid; otherwise, false.</returns>
    bool IsValidFilePath(string filePath);
}
```

### String Operations

```csharp
/// <summary>
/// Interface for string manipulation operations.
/// </summary>
public interface IStringFormatter
{
    /// <summary>
    /// Determines whether a string is null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if the string is null or empty; otherwise, false.</returns>
    bool IsNullOrEmpty(string? value);

    /// <summary>
    /// Trims whitespace from a string safely.
    /// </summary>
    /// <param name="value">The string to trim.</param>
    /// <returns>The trimmed string, or an empty string if input is null.</returns>
    string TrimSafely(string? value);

    /// <summary>
    /// Truncates a string to a maximum length.
    /// </summary>
    /// <param name="value">The string to truncate.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="suffix">Optional suffix to append if truncated.</param>
    /// <returns>The truncated string.</returns>
    string Truncate(string? value, int maxLength, string suffix = "");
}
```

---

## TMXTools.WPF Interfaces

### Converters

```csharp
/// <summary>
/// Interface for boolean value conversion.
/// </summary>
public interface IBooleanConverter
{
    /// <summary>
    /// Converts a boolean value to another type.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="trueValue">The value to return when true.</param>
    /// <param name="falseValue">The value to return when false.</param>
    /// <returns>The converted value.</returns>
    object Convert(bool value, object trueValue, object falseValue);
}

/// <summary>
/// Interface for mathematical operations in converters.
/// </summary>
public interface IMathConverter
{
    /// <summary>
    /// Performs a mathematical operation.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="operation">The operation to perform.</param>
    /// <param name="operand">The operand for the operation.</param>
    /// <returns>The result of the operation.</returns>
    double Calculate(double value, string operation, double operand);
}
```

### Dispatcher & Threading

```csharp
/// <summary>
/// Interface for dispatcher-based operations.
/// </summary>
public interface IDispatcherService
{
    /// <summary>
    /// Determines whether the current thread is the UI thread.
    /// </summary>
    bool IsUIThread { get; }

    /// <summary>
    /// Executes an action on the UI thread asynchronously.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InvokeAsync(Action action);

    /// <summary>
    /// Executes an action on the UI thread asynchronously with a result.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="function">The function to execute.</param>
    /// <returns>A task representing the operation, with the result.</returns>
    Task<T> InvokeAsync<T>(Func<T> function);

    /// <summary>
    /// Queues an action to run on the UI thread.
    /// </summary>
    /// <param name="action">The action to queue.</param>
    void BeginInvoke(Action action);
}

/// <summary>
/// Interface for safe resource access with locking.
/// </summary>
public interface ILockableResource
{
    /// <summary>
    /// Acquires a read lock for accessing the resource.
    /// </summary>
    /// <returns>A disposable lock handle.</returns>
    IDisposable AcquireReadLock();

    /// <summary>
    /// Acquires a write lock for modifying the resource.
    /// </summary>
    /// <returns>A disposable lock handle.</returns>
    IDisposable AcquireWriteLock();

    /// <summary>
    /// Performs an operation while holding a read lock.
    /// </summary>
    /// <param name="action">The action to perform.</param>
    void WithReadLock(Action action);

    /// <summary>
    /// Performs an operation while holding a write lock.
    /// </summary>
    /// <param name="action">The action to perform.</param>
    void WithWriteLock(Action action);
}
```

### Controls & UI Components

```csharp
/// <summary>
/// Interface for numeric input controls.
/// </summary>
public interface INumericInput
{
    /// <summary>
    /// Gets or sets the numeric value.
    /// </summary>
    double? Value { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    double? Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    double? Maximum { get; set; }

    /// <summary>
    /// Gets or sets the value increment for buttons.
    /// </summary>
    double Increment { get; set; }

    /// <summary>
    /// Gets or sets the number of decimal places.
    /// </summary>
    int DecimalPlaces { get; set; }

    /// <summary>
    /// Occurs when the value changes.
    /// </summary>
    event EventHandler<ValueChangedEventArgs>? ValueChanged;
}

/// <summary>
/// Interface for status tracking.
/// </summary>
public interface IAppStatus
{
    /// <summary>
    /// Gets the current application status.
    /// </summary>
    string CurrentStatus { get; }

    /// <summary>
    /// Gets the current progress percentage (0-100).
    /// </summary>
    int ProgressPercentage { get; }

    /// <summary>
    /// Gets a value indicating whether an operation is in progress.
    /// </summary>
    bool IsOperationInProgress { get; }

    /// <summary>
    /// Sets the status message.
    /// </summary>
    /// <param name="message">The status message.</param>
    void SetStatus(string message);

    /// <summary>
    /// Updates the progress.
    /// </summary>
    /// <param name="percentage">The progress percentage.</param>
    void UpdateProgress(int percentage);

    /// <summary>
    /// Starts an operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    void StartOperation(string operationName);

    /// <summary>
    /// Completes the current operation.
    /// </summary>
    /// <param name="success">Whether the operation succeeded.</param>
    void CompleteOperation(bool success = true);

    /// <summary>
    /// Occurs when the status changes.
    /// </summary>
    event EventHandler<StatusChangedEventArgs>? StatusChanged;
}
```

---

## Dependency Injection Setup

### Service Registration

```csharp
public static void RegisterTMXToolsServices(IServiceCollection services)
{
    // Core library services
    services.AddSingleton<IFileReader, FileReader>();
    services.AddSingleton<IFileWriter, FileWriter>();
    services.AddSingleton<IFileValidator, FileValidator>();
    services.AddSingleton<IStringFormatter, StringFormatter>();
}

public static void RegisterWPFServices(IServiceCollection services)
{
    // WPF dispatcher service
    services.AddSingleton<IDispatcherService, WPFDispatcherService>();
    
    // WPF converters
    services.AddSingleton<IBooleanConverter, BooleanConverter>();
    services.AddSingleton<IMathConverter, MathConverter>();
    
    // WPF utilities
    services.AddSingleton<IAppStatus, AppStatus>();
}
```

### Service Locator Pattern (Legacy Support)

For existing code that can't use DI yet:

```csharp
public static class ServiceLocator
{
    private static IServiceProvider? _serviceProvider;

    public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public static T? GetService<T>() where T : class
    {
        return _serviceProvider?.GetService(typeof(T)) as T;
    }
}
```

---

## Migration Strategy

### Phase 1: Define Interfaces
- Create all interfaces in a separate namespace
- Ensure no circular dependencies
- Document each interface thoroughly

### Phase 2: Create Default Implementations
- Implement each interface with current logic
- Keep backward compatibility
- Mark old static methods as deprecated

### Phase 3: Update Existing Code
- Refactor to use interfaces where possible
- Maintain backward compatibility
- Add unit tests for new code paths

### Phase 4: Enable Dependency Injection
- Set up DI container
- Update entry points to use DI
- Migrate application code gradually

### Phase 5: Retire Legacy Code
- Remove deprecated static methods (major version bump)
- Update all examples and documentation
- Archive old implementation details

---

## Benefits

- ✅ **Testability**: Easy to mock and stub interfaces
- ✅ **Flexibility**: Swap implementations easily
- ✅ **Maintainability**: Clear contracts and responsibilities
- ✅ **Extensibility**: Add new implementations without modifying existing code
- ✅ **Decoupling**: Reduce dependencies between components
- ✅ **Documentation**: Interfaces serve as contracts

---

## Example: Refactoring FileUtils

### Before (Current)

```csharp
public static class FileUtils
{
    public static bool FileExists(string filePath) => File.Exists(filePath);
    
    public static long GetFileSizeBytes(string filePath)
    {
        if (!File.Exists(filePath))
            return 0;
        return new FileInfo(filePath).Length;
    }
}

// Usage
if (FileUtils.FileExists(path))
{
    var size = FileUtils.GetFileSizeBytes(path);
}
```

### After (With Interfaces)

```csharp
public class FileValidator : IFileValidator
{
    public bool FileExists(string filePath) => File.Exists(filePath);
    
    public long GetFileSizeBytes(string filePath)
    {
        if (!File.Exists(filePath))
            return 0;
        return new FileInfo(filePath).Length;
    }
    
    public bool IsValidFilePath(string filePath)
    {
        try
        {
            _ = new FileInfo(filePath);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

// Usage with DI
public class MyService
{
    private readonly IFileValidator _fileValidator;
    
    public MyService(IFileValidator fileValidator)
    {
        _fileValidator = fileValidator;
    }
    
    public void CheckFile(string path)
    {
        if (_fileValidator.FileExists(path))
        {
            var size = _fileValidator.GetFileSizeBytes(path);
        }
    }
}

// Testing
[Test]
public void CheckFile_WithNonexistentFile_HandlesGracefully()
{
    // Arrange
    var mockValidator = new Mock<IFileValidator>();
    mockValidator.Setup(v => v.FileExists(It.IsAny<string>()))
        .Returns(false);
    
    var service = new MyService(mockValidator.Object);
    
    // Act & Assert
    Assert.DoesNotThrow(() => service.CheckFile("nonexistent.txt"));
}
```

---

## Backward Compatibility Strategy

### Keep Old API with Deprecation

```csharp
/// <summary>
/// Provides file validation operations.
/// </summary>
[Obsolete("Use IFileValidator from dependency injection instead. See migration guide at https://...")]
public static class FileUtils
{
    public static bool FileExists(string filePath)
    {
        var validator = ServiceLocator.GetService<IFileValidator>() 
            ?? new FileValidator();
        return validator.FileExists(filePath);
    }
}
```

### Migration Guide in XML Docs

```csharp
/// <summary>
/// Gets whether a file exists.
/// </summary>
/// <remarks>
/// <para>
/// This method is deprecated. Use <see cref="IFileValidator"/> instead:
/// </para>
/// <code>
/// public class MyClass
/// {
///     private readonly IFileValidator _validator;
///     
///     public MyClass(IFileValidator validator)
///     {
///         _validator = validator;
///     }
///     
///     public bool Check(string path)
///     {
///         return _validator.FileExists(path);
///     }
/// }
/// </code>
/// <para>
/// Register the service in your DI container:
/// <code>
/// services.AddSingleton&lt;IFileValidator, FileValidator&gt;();
/// </code>
/// </para>
/// </remarks>
public static bool FileExists(string filePath) { ... }
```

---

## Testing Strategy

All interfaces should have corresponding test implementations:

```csharp
public class MockFileValidator : IFileValidator
{
    public Dictionary<string, bool> ExistingFiles { get; } = new();
    public Dictionary<string, long> FileSizes { get; } = new();

    public bool FileExists(string filePath)
        => ExistingFiles.ContainsKey(filePath) && ExistingFiles[filePath];

    public long GetFileSizeBytes(string filePath)
        => FileSizes.TryGetValue(filePath, out var size) ? size : 0;

    public bool IsValidFilePath(string filePath) => !string.IsNullOrEmpty(filePath);
}
```

---

## Success Criteria

- [ ] All core components have interfaces
- [ ] Default implementations provided
- [ ] DI container configured
- [ ] Unit tests use mocked interfaces
- [ ] Documentation updated
- [ ] Backward compatibility maintained
- [ ] Performance metrics unchanged
- [ ] Zero breaking changes (major version only)
