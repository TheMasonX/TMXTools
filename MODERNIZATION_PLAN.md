# TMXTools Modernization Plan

**Status**: In Progress  
**Target Framework**: .NET 8  
**Start Date**: 2025  

## Overview

This plan outlines the modernization of TMXTools and TMXTools.WPF projects using industry best practices, comprehensive unit testing, and a robust CI/CD pipeline.

---

## Phase 1: Foundation & Infrastructure (Current)

### 1.1 Unit Testing Structure
- **Status**: In Progress
- **Details**: See [TESTS_PLAN.md](./task_plans/TESTS_PLAN.md)
- [ ] Create `TMXTools.Tests` project for core library unit tests
- [ ] Establish testing patterns and conventions
- [ ] Expand `TMXTools.WPF.Tests` with comprehensive test coverage
- [ ] Set up code coverage reporting
- [ ] Configure test integration with CI/CD

### 1.2 CI/CD Pipeline
- **Status**: In Progress
- **Details**: See [CICD_PLAN.md](./task_plans/CICD_PLAN.md)
- [ ] GitHub Actions workflow for build, test, and code coverage
- [ ] Automated NuGet package generation
- [ ] Release automation
- [ ] Benchmark execution and tracking

### 1.3 Project Structure & Configuration
- **Status**: In Progress
- [ ] Enable nullable reference types (already enabled)
- [ ] Enable implicit usings (already enabled)
- [ ] Add EditorConfig for consistency
- [ ] Configure analyzer rules (stylecop, roslyn)
- [ ] Add pre-commit hooks

---

## Phase 2: Code Quality & Best Practices

### 2.1 Async/Await Modernization
- **Status**: Planned
- **Details**: See [ASYNC_MODERNIZATION.md](./task_plans/ASYNC_MODERNIZATION.md)
- [ ] Audit codebase for blocking calls
- [ ] Convert to async/await patterns
- [ ] Add async versions of public APIs
- [ ] Ensure proper task handling

### 2.2 Null Safety & Nullability
- **Status**: Planned
- [ ] Review all public APIs for nullability annotations
- [ ] Add missing `[NotNull]`, `[CanBeNull]` attributes
- [ ] Enable nullable reference type warnings
- [ ] Document null handling patterns

### 2.3 Interface-Based Design
- **Status**: Planned
- **Details**: See [INTERFACES_DESIGN.md](./task_plans/INTERFACES_DESIGN.md)
- [ ] Create interfaces for core abstractions
- [ ] Refactor existing implementations to use interfaces
- [ ] Enable dependency injection patterns
- [ ] Support unit testing through mocking

### 2.4 Code Analysis & Style
- **Status**: Planned
- [ ] Enable StyleCop analyzers
- [ ] Configure StyleCop rules to match project conventions
- [ ] Add FxCop security rules
- [ ] Set up code metrics

---

## Phase 3: Feature Implementation

### 3.1 File Handling Improvements
- **Status**: Planned
- **Details**: See [FILE_MODEL_PLAN.md](./task_plans/FILE_MODEL_PLAN.md)
- [ ] Create `IFileModel` interface and `BaseFileModel` abstract class
- [ ] Implement `TextFileModel` 
- [ ] Create generic `CustomFileModel<TData>`
- [ ] Add safe file stream wrappers
- [ ] Support for custom file types using functional programming

### 3.2 Undo/Redo Command Pattern
- **Status**: Planned
- **Details**: See [UNDO_REDO_PLAN.md](./task_plans/UNDO_REDO_PLAN.md)
- [ ] Research and design pattern implementation
- [ ] Create command pattern utilities
- [ ] Implement configurable history stack
- [ ] Support for WPF behaviors and converters
- [ ] Document usage patterns

### 3.3 Drawing Libraries & Bitmap Operations
- **Status**: Planned
- **Details**: See [DRAWING_LIBRARIES_PLAN.md](./task_plans/DRAWING_LIBRARIES_PLAN.md)
- [ ] Safe wrappers for WriteableBitmap interactions
- [ ] Extension methods for bitmap conversions
- [ ] Higher-order methods for bitmap operations
- [ ] Media/Drawing type conversions
- [ ] Graphics helper methods

### 3.4 WPF Enhancements
- **Status**: Planned
- **Details**: See [WPF_ENHANCEMENTS_PLAN.md](./task_plans/WPF_ENHANCEMENTS_PLAN.md)
- [ ] Compositional file command handlers
- [ ] Enhanced logging support
- [ ] Zoomable/pannable canvas with drawing
- [ ] XAML x:Names for automated UI testing
- [ ] Proper locking and dispatching patterns

---

## Phase 4: Documentation

### 4.1 Code Documentation
- **Status**: Planned
- [ ] Add XML documentation comments to all public APIs
- [ ] Generate Doxygen documentation
- [ ] Document architecture and patterns

### 4.2 Style Guide
- **Status**: Planned
- [ ] Create comprehensive style guide
- [ ] Document preferred patterns:
  - Composition over inheritance
  - Explicit error handling
  - Guard clauses
  - Use of `is null` checks
  - Nullability annotations
  - CodeAnalysis attributes

### 4.3 Copilot Instructions
- **Status**: Planned
- [ ] Create `.copilot-instructions.md` for AI assistance
- [ ] Document project patterns and conventions
- [ ] Include testing requirements

---

## Current Architecture

### TMXTools (Core Library)
- **Target Framework**: net8.0
- **Key Utilities**: 
  - FileUtils
  - StringExtensions
- **Dependencies**: None (minimal)
- **Status**: Core infrastructure in place

### TMXTools.WPF (WPF Library)
- **Target Framework**: net8.0-windows
- **Key Components**:
  - Converters (Boolean, Math, Gesture)
  - Controls (NumericBox)
  - Extensions (Dispatcher, Color)
  - Utilities (AppStatus, BindingProxy, GridLengthAnimation)
- **Dependencies**: 
  - TMXTools
  - CommunityToolkit.Mvvm 8.4.0
  - System.Drawing.Common 9.0.7
- **Status**: Active development

### TMXTools.WPF.Tests
- **Target Framework**: net8.0-windows
- **Test Framework**: NUnit 4.3.2
- **Coverage Tools**: coverlet 6.0.4
- **Status**: Basic tests present, needs expansion

---

## Quality Metrics

### Code Coverage Targets
- **TMXTools**: 80%+ coverage
- **TMXTools.WPF**: 75%+ coverage (excluding UI-heavy components)

### Performance Targets
- Maintain backward compatibility
- Zero performance regressions
- Benchmark suite for tracking

### Documentation Targets
- 100% of public APIs documented
- Contributing guide
- Architecture documentation

---

## Timeline & Priorities

### Immediate (This Sprint)
1. Set up CI/CD pipeline
2. Create TMXTools.Tests project
3. Establish testing patterns

### Short Term (1-2 Sprints)
1. Expand test coverage
2. Enable code analysis
3. Add documentation

### Medium Term (3-4 Sprints)
1. File model improvements
2. Async/await modernization
3. Interface-based refactoring

### Long Term
1. Undo/Redo implementation
2. Drawing libraries
3. Advanced WPF features

---

## Success Criteria

- [ ] Green CI/CD builds on every commit
- [ ] 80%+ code coverage for TMXTools
- [ ] 75%+ code coverage for TMXTools.WPF
- [ ] Zero code analysis warnings (by configuration)
- [ ] Automated releases to NuGet
- [ ] Comprehensive documentation
- [ ] Benchmark suite with regression tracking
- [ ] Clear contributing guidelines

---

## Related Documents

- [TESTS_PLAN.md](./task_plans/TESTS_PLAN.md) - Unit testing strategy and implementation
- [CICD_PLAN.md](./task_plans/CICD_PLAN.md) - CI/CD pipeline details
- [ASYNC_MODERNIZATION.md](./task_plans/ASYNC_MODERNIZATION.md) - Async/await refactoring
- [INTERFACES_DESIGN.md](./task_plans/INTERFACES_DESIGN.md) - Interface-based design patterns
- [FILE_MODEL_PLAN.md](./task_plans/FILE_MODEL_PLAN.md) - File handling improvements
- [UNDO_REDO_PLAN.md](./task_plans/UNDO_REDO_PLAN.md) - Command pattern for undo/redo
- [DRAWING_LIBRARIES_PLAN.md](./task_plans/DRAWING_LIBRARIES_PLAN.md) - Drawing and bitmap operations
- [WPF_ENHANCEMENTS_PLAN.md](./task_plans/WPF_ENHANCEMENTS_PLAN.md) - WPF-specific improvements
