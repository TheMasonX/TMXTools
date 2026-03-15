# TMXTools
## Overview
{Insert Github Actions badge here}

Standard C# and WPF tools and utilities. Free use under the MIT license and open to contributions.

Two main components: a standard class library and a WPF library.
- TMXTools is designed to be a lightweight and flexible library that can be used in various applications. It's cross-platform.
- TMXTools.WPF provides a set of controls and utilities for WPF applications. Restricted to Windows due to the WPF dependency.

These are two separate nuget packages for the purposes of minimizing dependencies if WPF is not needed.


## Tasks
### CI/CD Pipeline
- [ ] Unit tests project for TMXTools
    - Move any applicable from the TMXTools.WPF.Tests
- [ ] Github actions to build
    - [ ] Run library tests and export code coverage results
    - [ ] Artifact handling
    - [ ] Releases
    - [ ] Run benchmarks and export results
        - [ ] If possible, track regressions
- [ ] More tests for TMXTools.WPF
- [ ] More benchmarks

### Refactoring
- [ ] .NET 11 updates and modernization
- [ ] Proper async/await usage
- [ ] Interfaces whenever possible
- [ ] XAML x:Names to support automated UI testing
- [ ] Locking and dispatching

### Documentation
- [ ] Create style guide
    - Prefer composition
    - Explicit error handling
    - Guard clauses
    - Use of `is null`
    - Nullability annotations
        - CodeAnalysis attributes
- [ ] Copilot instructions markdown
- [ ] Code documentation
- [ ] Doxygen setup
- [ ] Review codebase and create an improvement plan document

### File tasks
- [ ] More safe file wrappers
- [ ] File model improvements
    - [ ] `BaseFileModel` and an `IFileModel` interface (with sub interfaces as appropriate)
        - Public methods using filestreams will get the filestream from the base class' protected method such as `using base.OpenFileStream(file);`
        - Different filetypes would have their own model implementations
    - [ ] Example implementation of a `TextFileModel`
    - [ ] Support for a custom filetype using generics and functional programming higher order functions
        - e.g. `new CustomFile<TData>(readMethod, saveMethod, getData);`

### Undo/Redo Command Pattern
- [ ] Research and create an implementation plan for Undo/Redo utilitlies 
    - Initial thought is a List based "stack" with configurable max size (set to something like 20 as default)
    - How to handle operations? Command pattern w/ functional programming?
    - Make it easy to use anywhere
        - Support for behaviors and converters?

### Drawing Libraries
- [ ] WriteableBitmapEx and System.Drawing.Common
- [ ] Safe wrappers for WriteableBitmap interaction using the DispatcherUtils
- [ ] Extension methods to convert writeable bitmaps `ToBitmap()` using the backbuffer
- [ ] Higher order method to perform a function on the `Bitmap` representation of a `WriteableBitmap` while it's locked
- [ ] Other conversion methods between System.Windows.Media and System.Drawing
- [ ] More Bitmap/Graphics.Drawing helper methods

### WPF Tasks
- [ ] Compositional file command handlers that ViewModels can incorporate
- [ ] Better logging support
- [ ] Zoomable/panable canvas with drawing support
    - [ ] Use the drawing libraries and extensions