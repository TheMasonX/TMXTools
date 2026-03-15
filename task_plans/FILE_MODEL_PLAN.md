# File Handling & Model Improvements Plan

**Status**: Planned  
**Priority**: Medium  

## Overview

Comprehensive refactoring of file handling in TMXTools with a focus on abstraction, flexibility, and type safety using interfaces and generic patterns.

---

## Current State

### FileUtils.cs Analysis
- Utility static methods for file operations
- Basic file I/O operations
- Needs encapsulation in model classes

---

## Proposed Architecture

### 1. IFileModel Interface

Base interface for all file models:

```csharp
/// <summary>
/// Base interface for file models providing common file operations.
/// </summary>
public interface IFileModel
{
    /// <summary>
    /// Gets the file path associated with this model.
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// Gets a value indicating whether the file exists.
    /// </summary>
    bool FileExists { get; }

    /// <summary>
    /// Gets the file size in bytes.
    /// </summary>
    long FileSizeBytes { get; }

    /// <summary>
    /// Gets the last modification time of the file.
    /// </summary>
    DateTime LastModifiedTime { get; }

    /// <summary>
    /// Saves the file asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task SaveAsync();

    /// <summary>
    /// Deletes the file asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteAsync();

    /// <summary>
    /// Exports the file to a specified path asynchronously.
    /// </summary>
    /// <param name="exportPath">The path to export to.</param>
    /// <returns>A task representing the asynchronous export operation.</returns>
    Task ExportAsync(string exportPath);

    /// <summary>
    /// Reloads the file from disk asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous reload operation.</returns>
    Task ReloadAsync();
}
```

### 2. Specialized Interfaces

```csharp
/// <summary>
/// Interface for file models that load content from disk.
/// </summary>
public interface ILoadableFileModel : IFileModel
{
    /// <summary>
    /// Loads the file from disk asynchronously.
    /// </summary>
    /// <param name="filePath">The path to load from.</param>
    /// <returns>A task representing the asynchronous load operation.</returns>
    Task LoadAsync(string filePath);
}

/// <summary>
/// Interface for file models with serializable content.
/// </summary>
public interface ISerializableFileModel : IFileModel
{
    /// <summary>
    /// Gets the serialized content of the file.
    /// </summary>
    /// <returns>The serialized content as bytes.</returns>
    byte[] GetSerialized();

    /// <summary>
    /// Gets the serialized content of the file as a string.
    /// </summary>
    /// <returns>The serialized content as a string.</returns>
    string GetSerializedString();
}

/// <summary>
/// Interface for undoable file operations.
/// </summary>
public interface IUndoableFileModel : IFileModel
{
    /// <summary>
    /// Gets a value indicating whether undo is available.
    /// </summary>
    bool CanUndo { get; }

    /// <summary>
    /// Gets a value indicating whether redo is available.
    /// </summary>
    bool CanRedo { get; }

    /// <summary>
    /// Undoes the last operation.
    /// </summary>
    void Undo();

    /// <summary>
    /// Redoes the last undone operation.
    /// </summary>
    void Redo();
}
```

### 3. BaseFileModel Abstract Class

```csharp
/// <summary>
/// Base class for file models, providing common file operations.
/// </summary>
public abstract class BaseFileModel : IFileModel
{
    /// <summary>
    /// Gets the file path associated with this model.
    /// </summary>
    public string FilePath { get; protected set; } = null!;

    /// <summary>
    /// Gets a value indicating whether the file exists.
    /// </summary>
    public virtual bool FileExists => File.Exists(FilePath);

    /// <summary>
    /// Gets the file size in bytes.
    /// </summary>
    public virtual long FileSizeBytes
    {
        get
        {
            if (!FileExists)
                return 0;

            var info = new FileInfo(FilePath);
            return info.Length;
        }
    }

    /// <summary>
    /// Gets the last modification time of the file.
    /// </summary>
    public virtual DateTime LastModifiedTime
    {
        get
        {
            if (!FileExists)
                return DateTime.MinValue;

            var info = new FileInfo(FilePath);
            return info.LastWriteTime;
        }
    }

    /// <summary>
    /// Opens a file stream for reading or writing.
    /// </summary>
    /// <param name="mode">The file mode.</param>
    /// <param name="access">The file access.</param>
    /// <returns>A file stream.</returns>
    protected FileStream OpenFileStream(FileMode mode, FileAccess access)
    {
        if (string.IsNullOrWhiteSpace(FilePath))
            throw new InvalidOperationException("FilePath must be set before opening a file stream.");

        return new FileStream(FilePath, mode, access);
    }

    /// <summary>
    /// Saves the file asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public abstract Task SaveAsync();

    /// <summary>
    /// Deletes the file asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public virtual async Task DeleteAsync()
    {
        if (FileExists)
        {
            try
            {
                File.Delete(FilePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete file at '{FilePath}'.", ex);
            }
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <summary>
    /// Exports the file to a specified path asynchronously.
    /// </summary>
    /// <param name="exportPath">The path to export to.</param>
    /// <returns>A task representing the asynchronous export operation.</returns>
    public virtual async Task ExportAsync(string exportPath)
    {
        if (!FileExists)
            throw new FileNotFoundException($"Source file not found: '{FilePath}'");

        if (string.IsNullOrWhiteSpace(exportPath))
            throw new ArgumentException("Export path cannot be null or empty.", nameof(exportPath));

        try
        {
            File.Copy(FilePath, exportPath, overwrite: true);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to export file to '{exportPath}'.", ex);
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <summary>
    /// Reloads the file from disk asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous reload operation.</returns>
    public abstract Task ReloadAsync();
}
```

---

## Concrete Implementations

### 1. TextFileModel

```csharp
/// <summary>
/// A file model for text files.
/// </summary>
public class TextFileModel : BaseFileModel, ILoadableFileModel, ISerializableFileModel
{
    /// <summary>
    /// Gets or sets the content of the text file.
    /// </summary>
    public string Content { get; set; } = "";

    /// <summary>
    /// Gets or sets the text encoding.
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextFileModel"/> class.
    /// </summary>
    public TextFileModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextFileModel"/> class with a file path.
    /// </summary>
    /// <param name="filePath">The path to the text file.</param>
    public TextFileModel(string filePath) => FilePath = filePath;

    /// <summary>
    /// Loads the file from disk asynchronously.
    /// </summary>
    /// <param name="filePath">The path to load from.</param>
    /// <returns>A task representing the asynchronous load operation.</returns>
    public async Task LoadAsync(string filePath)
    {
        FilePath = filePath;
        await ReloadAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Saves the file asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public override async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
            throw new InvalidOperationException("FilePath must be set before saving.");

        try
        {
            using (var stream = OpenFileStream(FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream, Encoding))
            {
                await writer.WriteAsync(Content).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save file to '{FilePath}'.", ex);
        }
    }

    /// <summary>
    /// Reloads the file from disk asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous reload operation.</returns>
    public override async Task ReloadAsync()
    {
        if (!FileExists)
            throw new FileNotFoundException($"File not found: '{FilePath}'");

        try
        {
            using (var stream = OpenFileStream(FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream, Encoding))
            {
                Content = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to reload file from '{FilePath}'.", ex);
        }
    }

    /// <summary>
    /// Gets the serialized content of the file.
    /// </summary>
    /// <returns>The serialized content as bytes.</returns>
    public byte[] GetSerialized() => Encoding.GetBytes(Content);

    /// <summary>
    /// Gets the serialized content of the file as a string.
    /// </summary>
    /// <returns>The serialized content as a string.</returns>
    public string GetSerializedString() => Content;
}
```

### 2. Generic CustomFileModel<TData>

```csharp
/// <summary>
/// A generic file model for custom data types using functional programming patterns.
/// </summary>
/// <typeparam name="TData">The type of data the model contains.</typeparam>
public class CustomFileModel<TData> : BaseFileModel, ILoadableFileModel, ISerializableFileModel
    where TData : notnull
{
    private TData _data = default!;

    /// <summary>
    /// Gets or sets the data contained in this file model.
    /// </summary>
    public TData Data
    {
        get => _data;
        set => _data = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Function to read data from a file stream.
    /// </summary>
    private readonly Func<FileStream, Task<TData>> _readFunction;

    /// <summary>
    /// Function to save data to a file stream.
    /// </summary>
    private readonly Func<FileStream, TData, Task> _saveFunction;

    /// <summary>
    /// Function to serialize data to bytes.
    /// </summary>
    private readonly Func<TData, byte[]> _serializeFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomFileModel{TData}"/> class.
    /// </summary>
    /// <param name="readFunction">Function to read data from a file stream.</param>
    /// <param name="saveFunction">Function to save data to a file stream.</param>
    /// <param name="serializeFunction">Function to serialize data to bytes.</param>
    /// <exception cref="ArgumentNullException">Thrown when any function is null.</exception>
    public CustomFileModel(
        Func<FileStream, Task<TData>> readFunction,
        Func<FileStream, TData, Task> saveFunction,
        Func<TData, byte[]> serializeFunction)
    {
        _readFunction = readFunction ?? throw new ArgumentNullException(nameof(readFunction));
        _saveFunction = saveFunction ?? throw new ArgumentNullException(nameof(saveFunction));
        _serializeFunction = serializeFunction ?? throw new ArgumentNullException(nameof(serializeFunction));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomFileModel{TData}"/> class with a file path.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="readFunction">Function to read data from a file stream.</param>
    /// <param name="saveFunction">Function to save data to a file stream.</param>
    /// <param name="serializeFunction">Function to serialize data to bytes.</param>
    public CustomFileModel(
        string filePath,
        Func<FileStream, Task<TData>> readFunction,
        Func<FileStream, TData, Task> saveFunction,
        Func<TData, byte[]> serializeFunction)
        : this(readFunction, saveFunction, serializeFunction)
    {
        FilePath = filePath;
    }

    /// <summary>
    /// Loads the file from disk asynchronously.
    /// </summary>
    /// <param name="filePath">The path to load from.</param>
    /// <returns>A task representing the asynchronous load operation.</returns>
    public async Task LoadAsync(string filePath)
    {
        FilePath = filePath;
        await ReloadAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Saves the file asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public override async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
            throw new InvalidOperationException("FilePath must be set before saving.");

        try
        {
            using (var stream = OpenFileStream(FileMode.Create, FileAccess.Write))
            {
                await _saveFunction(stream, Data).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save file to '{FilePath}'.", ex);
        }
    }

    /// <summary>
    /// Reloads the file from disk asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous reload operation.</returns>
    public override async Task ReloadAsync()
    {
        if (!FileExists)
            throw new FileNotFoundException($"File not found: '{FilePath}'");

        try
        {
            using (var stream = OpenFileStream(FileMode.Open, FileAccess.Read))
            {
                Data = await _readFunction(stream).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to reload file from '{FilePath}'.", ex);
        }
    }

    /// <summary>
    /// Gets the serialized content of the file.
    /// </summary>
    /// <returns>The serialized content as bytes.</returns>
    public byte[] GetSerialized() => _serializeFunction(Data);

    /// <summary>
    /// Gets the serialized content of the file as a string.
    /// </summary>
    /// <returns>The serialized content as a string.</returns>
    public string GetSerializedString()
    {
        var bytes = GetSerialized();
        return Encoding.UTF8.GetString(bytes);
    }
}
```

---

## Usage Examples

### TextFileModel

```csharp
// Load and modify a text file
var textModel = new TextFileModel();
await textModel.LoadAsync("myfile.txt");

textModel.Content = textModel.Content.ToUpper();

await textModel.SaveAsync();

// Export to another location
await textModel.ExportAsync("backup.txt");
```

### CustomFileModel with JSON

```csharp
// Custom model for JSON serialization
public class JsonFileModel : CustomFileModel<Dictionary<string, object>>
{
    public JsonFileModel(string filePath)
        : base(
            filePath,
            async stream => JsonSerializer.Deserialize<Dictionary<string, object>>(
                await File.ReadAllBytesAsync(filePath)) ?? new(),
            async (stream, data) => await File.WriteAllTextAsync(
                filePath,
                JsonSerializer.Serialize(data)),
            data => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data))
        )
    {
    }
}
```

---

## Implementation Steps

1. Create `IFileModel` and related interfaces
2. Implement `BaseFileModel` abstract class
3. Implement `TextFileModel`
4. Implement `CustomFileModel<TData>` generic class
5. Create unit tests for all file models
6. Refactor existing `FileUtils` to use new models
7. Create documentation and usage examples
8. Migrate applications to use new file models

---

## Benefits

- ✅ Type-safe file operations
- ✅ Flexible and extensible architecture
- ✅ Async/await throughout
- ✅ Composition over inheritance
- ✅ Functional programming patterns
- ✅ Easy unit testing with mocks
- ✅ Clear separation of concerns
- ✅ Support for custom file types

---

## Testing Strategy

- Unit tests for each file model implementation
- Mock file systems for testing
- Test error handling and edge cases
- Test async behavior
- Benchmark file operations

---

## Backward Compatibility

- Keep existing `FileUtils` static methods
- Mark old methods as `[Obsolete]` with migration guidance
- Provide migration path to new models
- Support both old and new APIs during transition
