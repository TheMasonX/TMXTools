# Async/Await Modernization Plan

**Status**: Planned  
**Priority**: High  
**Impact**: Performance, responsiveness, resource utilization  

## Overview

Comprehensive migration to async/await patterns throughout TMXTools, ensuring non-blocking operations and proper resource cleanup.

---

## Current State Assessment

### Areas to Review
- [ ] File I/O operations (should be async)
- [ ] Dispatcher operations (should be async)
- [ ] Event handlers (should support async operations)
- [ ] Data loading/initialization (should be async)

---

## Async Best Practices

### 1. Never Block on Async Code
```csharp
// ❌ BAD
var result = asyncMethod().Result;
var result = asyncMethod().Wait();

// ✅ GOOD
var result = await asyncMethod();
```

### 2. Use ConfigureAwait(false) in Libraries
```csharp
// ✅ Recommended for library code
public async Task DoWorkAsync()
{
    var data = await GetDataAsync().ConfigureAwait(false);
    return await ProcessAsync(data).ConfigureAwait(false);
}
```

### 3. Async All the Way
```csharp
// ❌ Don't mix sync and async
public async Task MyMethodAsync()
{
    var syncResult = SyncMethod();
    var asyncResult = await SomethingAsync();
}

// ✅ Use async equivalents
public async Task MyMethodAsync()
{
    var asyncResult1 = await GetDataAsync();
    var asyncResult2 = await ProcessAsync(asyncResult1);
}
```

### 4. Proper Exception Handling
```csharp
// ✅ GOOD
try
{
    await operationAsync();
}
catch (OperationCanceledException)
{
    // Handle cancellation
}
catch (Exception ex)
{
    // Handle other exceptions
}
```

---

## API Migration Strategy

### File Operations

```csharp
// Current FileUtils
public static class FileUtils
{
    public static byte[] ReadAllBytes(string path)
        => File.ReadAllBytes(path);
}

// Modernized with async
public interface IFileReader
{
    Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);
}

public class FileReader : IFileReader
{
    public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        => await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
}
```

### Dispatcher Operations

```csharp
// New async dispatcher interface
public interface IDispatcherService
{
    Task InvokeAsync(Action action);
    Task<T> InvokeAsync<T>(Func<Task<T>> function);
}

// Implementation
public class WPFDispatcherService : IDispatcherService
{
    public async Task InvokeAsync(Action action)
    {
        if (Thread.CurrentThread == Application.Current?.Dispatcher?.Thread)
        {
            action();
        }
        else
        {
            var tcs = new TaskCompletionSource<bool>();
            Application.Current?.Dispatcher?.BeginInvoke(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            await tcs.Task.ConfigureAwait(false);
        }
    }

    public async Task<T> InvokeAsync<T>(Func<Task<T>> function)
    {
        var tcs = new TaskCompletionSource<T>();
        Application.Current?.Dispatcher?.BeginInvoke(async () =>
        {
            try
            {
                var result = await function().ConfigureAwait(false);
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        return await tcs.Task.ConfigureAwait(false);
    }
}
```

---

## Common Patterns

### Async Initialization

```csharp
// ✅ Factory pattern
public class DataModel
{
    private DataModel() { }

    public static async Task<DataModel> CreateAsync(string filePath)
    {
        var model = new DataModel();
        await model.InitializeAsync(filePath);
        return model;
    }

    private async Task InitializeAsync(string filePath)
    {
        // Async initialization
    }
}

// Usage
var model = await DataModel.CreateAsync(path);
```

### Async Properties (C# 6.0+ Pattern)

```csharp
// Convert blocking properties to methods
// ❌ Before
public class FileModel
{
    public byte[] Content { get; set; }
}

// ✅ After
public class FileModel
{
    private byte[]? _content;
    
    public async Task<byte[]> GetContentAsync()
    {
        if (_content is not null)
            return _content;
        
        _content = await LoadContentAsync();
        return _content;
    }
}
```

### Cancellation Support

```csharp
// Always support CancellationToken
public async Task ProcessAsync(string filePath, CancellationToken cancellationToken = default)
{
    var data = await ReadFileAsync(filePath, cancellationToken).ConfigureAwait(false);
    
    cancellationToken.ThrowIfCancellationRequested();
    
    var result = await ProcessDataAsync(data, cancellationToken).ConfigureAwait(false);
    
    return result;
}
```

---

## Implementation Checklist

### Phase 1: File Operations
- [ ] Convert FileUtils.ReadAllBytes → ReadAllBytesAsync
- [ ] Convert FileUtils.WriteAllText → WriteAllTextAsync
- [ ] Add IFileReader, IFileWriter interfaces
- [ ] Update FileUtils to use async internally
- [ ] Update all file operations tests

### Phase 2: Dispatcher Operations
- [ ] Create IDispatcherService
- [ ] Implement async dispatcher methods
- [ ] Update DispatcherExtensions
- [ ] Replace synchronous dispatcher calls

### Phase 3: Data Models
- [ ] Convert IFileModel to use async
- [ ] Update BaseFileModel async methods
- [ ] Update TextFileModel async methods
- [ ] Update CustomFileModel async methods

### Phase 4: Controls & UI
- [ ] Review NumericBox for async operations
- [ ] Update AppStatus for async state management
- [ ] Support async property changes
- [ ] Update event handlers to support async

### Phase 5: Testing
- [ ] Add async test patterns
- [ ] Test cancellation tokens
- [ ] Test exception handling
- [ ] Test timeout scenarios

---

## Testing Async Code

```csharp
[Test]
public async Task ReadFileAsync_WithValidPath_ReturnsContent()
{
    // Arrange
    var filePath = "test.txt";
    File.WriteAllText(filePath, "content");
    var reader = new FileReader();

    // Act
    var result = await reader.ReadAllBytesAsync(filePath);

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(Encoding.UTF8.GetString(result), Is.EqualTo("content"));
}

[Test]
public async Task ReadFileAsync_WithCancellation_ThrowsOperationCanceledException()
{
    // Arrange
    var cts = new CancellationTokenSource();
    cts.Cancel();
    var reader = new FileReader();

    // Act & Assert
    Assert.ThrowsAsync<OperationCanceledException>(
        () => reader.ReadAllBytesAsync("test.txt", cts.Token));
}
```

---

## Performance Considerations

- Async improves responsiveness, not speed
- Use Task.Run sparingly for CPU-bound work
- Avoid async void (except event handlers)
- Use ValueTask for frequently-called async methods
- Monitor thread pool behavior

---

## Documentation Updates

All async methods should document:
- Cancellation token support
- Possible exceptions
- When to use vs. sync versions
- Return values and side effects

---

## Success Metrics

- [ ] All I/O operations are async
- [ ] All public APIs support CancellationToken
- [ ] No blocking calls in async code
- [ ] All tests pass with async patterns
- [ ] No deadlocks from async usage
- [ ] Performance metrics maintained or improved
