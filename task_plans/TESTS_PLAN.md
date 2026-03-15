# Unit Testing Strategy & Implementation Plan

**Status**: In Progress  
**Target Coverage**: TMXTools: 80%+, TMXTools.WPF: 75%+  

## Overview

Comprehensive unit testing strategy for TMXTools ecosystem, including test project structure, testing patterns, and integration with CI/CD.

---

## Project Structure

### TMXTools.Tests (NEW)
- **Target Framework**: net8.0
- **Framework**: NUnit 4.3.2
- **Purpose**: Core library unit tests
- **Location**: `/TMXTools.Tests/`

**Current Status**: Not created yet

**Contents to include**:
```
TMXTools.Tests/
├── Utils/
│   ├── FileUtilsTests.cs
│   └── [Add more tests as methods expand]
├── Extensions/
│   ├── StringExtensionsTests.cs
│   └── [Add more tests]
└── [Additional test folders matching project structure]
```

### TMXTools.WPF.Tests (EXISTING)
- **Target Framework**: net8.0-windows
- **Framework**: NUnit 4.3.2
- **Purpose**: WPF-specific unit tests
- **Location**: `/TMXTools.WPF.Tests/`

**Current Status**: Basic test structure exists

**Expansion Plan**:
```
TMXTools.WPF.Tests/
├── Converters/
│   ├── BooleanConverterTests.cs
│   ├── MathConverterTests.cs
│   ├── GestureConverterTests.cs
│   └── [Add more as needed]
├── Extensions/
│   ├── DispatcherExtensionsTests.cs (EXISTS)
│   ├── ColorExtensionsTests.cs
│   └── [Add more]
├── Controls/
│   ├── NumericBoxTests.cs
│   └── [Add more as needed]
├── Utils/
│   ├── AppStatusTests.cs
│   ├── BindingProxyTests.cs
│   └── [Add more]
└── [Additional test folders]
```

---

## Testing Patterns & Conventions

### Naming Conventions
- **Test Classes**: `[ClassUnderTest]Tests.cs`
- **Test Methods**: `[MethodName]_[Scenario]_[ExpectedOutcome]`
  
**Example**:
```csharp
public class StringExtensionsTests
{
    [Test]
    public void IsNullOrEmpty_WithNullString_ReturnsTrue()
    {
        // Arrange
        string? input = null;
        
        // Act
        var result = input.IsNullOrEmpty();
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void IsNullOrEmpty_WithEmptyString_ReturnsTrue()
    {
        // Arrange
        var input = "";
        
        // Act
        var result = input.IsNullOrEmpty();
        
        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void IsNullOrEmpty_WithValidString_ReturnsFalse()
    {
        // Arrange
        var input = "hello";
        
        // Act
        var result = input.IsNullOrEmpty();
        
        // Assert
        Assert.That(result, Is.False);
    }
}
```

### AAA Pattern (Arrange-Act-Assert)
All tests follow the AAA pattern:
1. **Arrange**: Set up test data and mocks
2. **Act**: Execute the code under test
3. **Assert**: Verify the results

### Test Data & Fixtures
- Use NUnit `[TestFixture]` attributes
- Create `TestData` helper classes for common test data
- Use `[TestCase]` and `[TestCaseSource]` for parameterized tests

**Example**:
```csharp
[TestFixture]
public class MathConverterTests
{
    private MathConverter _converter = null!;
    
    [SetUp]
    public void Setup()
    {
        _converter = new MathConverter();
    }
    
    [TestCase(2, 3, 5)]
    [TestCase(-1, 1, 0)]
    [TestCase(0, 0, 0)]
    public void Convert_Addition_ReturnsCorrectSum(int a, int b, int expected)
    {
        // Arrange & Act
        var result = _converter.Add(a, b);
        
        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}
```

### Mocking & Dependencies
- Use NSubstitute or Moq for mocking
- Keep mocks close to test code
- Mock external dependencies, not internal interfaces (when possible)

### Test Organization
- One test class per public class/interface
- Tests organized by method or feature
- Use `[Category("FeatureName")]` for organization

---

## Testing Priorities

### Phase 1: Core Utilities (High Priority)
1. **FileUtils**: Critical utility, should have 95%+ coverage
2. **StringExtensions**: Commonly used, target 90%+ coverage
3. **Dispatcher Extensions**: WPF critical, target 85%+ coverage

### Phase 2: Converters (Medium Priority)
1. **BooleanConverter**: Common use, target 85%+ coverage
2. **MathConverter**: Utility converter, target 80%+ coverage
3. **GestureConverter**: UI-specific, target 70%+ coverage

### Phase 3: Controls & Components (Medium Priority)
1. **NumericBox**: Input control, target 75%+ coverage
2. **AppStatus**: State management, target 80%+ coverage
3. **BindingProxy**: Helper utility, target 85%+ coverage

### Phase 4: Advanced Features (Lower Priority)
1. UI-heavy components (minimal unit testing)
2. Integration tests for complex interactions
3. End-to-end scenarios

---

## Code Coverage Strategy

### Tools & Configuration
- **Coverage Tool**: coverlet.collector 6.0.4 (already in project)
- **Report Format**: OpenCover (for CI/CD integration)
- **Threshold Checks**: Enforce minimum coverage per project

### Coverage Configuration
- **TMXTools**: 80% minimum (strict)
- **TMXTools.WPF**: 75% minimum (allowing UI components exceptions)
- **Exclude from coverage**:
  - Auto-generated code
  - UI Designer code
  - XAML code-behind (minimal logic)
  - Test utilities and helpers

### Coverage Reports
- Generate on every CI/CD build
- Export to GitHub as artifacts
- Track coverage trends over time
- Comment on PRs with coverage impact

---

## NUnit Best Practices

### Assertions
Use modern NUnit assertion syntax:
```csharp
// Good
Assert.That(value, Is.EqualTo(expected));
Assert.That(list, Has.Count.EqualTo(3));
Assert.That(string, Does.StartWith("Hello"));

// Avoid
Assert.AreEqual(expected, value);  // Old style
Assert.IsTrue(value == expected);  // Implicit assertion
```

### Async Tests
```csharp
[Test]
public async Task AsyncMethod_SomeBehavior_ReturnsExpected()
{
    // Arrange
    var service = new SomeService();
    
    // Act
    var result = await service.GetDataAsync();
    
    // Assert
    Assert.That(result, Is.Not.Null);
}
```

### Exception Testing
```csharp
[Test]
public void Method_WithInvalidInput_ThrowsArgumentException()
{
    // Arrange
    var invalid = null as string;
    
    // Act & Assert
    Assert.Throws<ArgumentNullException>(() => SomeMethod(invalid));
}

// Alternative using throws delegate
[Test]
public void Method_WithInvalidInput_ThrowsArgumentException()
{
    Assert.That(
        () => SomeMethod(null),
        Throws.TypeOf<ArgumentNullException>()
    );
}
```

---

## Test Execution Strategy

### Local Development
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test TMXTools.Tests

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test class
dotnet test --filter "ClassName=FileUtilsTests"

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

### CI/CD Integration
- Run on every commit
- Run on pull requests
- Generate coverage reports
- Fail build if coverage drops below threshold
- Fail build if tests fail

---

## Documentation & Maintenance

### Test Documentation
- Add comments for complex test scenarios
- Document test data choices
- Explain non-obvious assertions

### Test Maintenance
- Keep tests independent (no shared state)
- Update tests when API changes
- Remove obsolete tests
- Refactor duplicate test code

### Test Reviews
- Review tests as part of code review
- Ensure coverage for new features
- Check test quality and clarity

---

## Success Metrics

- [ ] TMXTools.Tests project created with initial test suite
- [ ] 80%+ code coverage for TMXTools
- [ ] 75%+ code coverage for TMXTools.WPF
- [ ] All critical utilities tested
- [ ] CI/CD integration reporting coverage
- [ ] New features include tests before merge
- [ ] Test execution time < 30 seconds locally

---

## Next Steps

1. **Create TMXTools.Tests project**
   - Copy NUnit configuration from TMXTools.WPF.Tests
   - Add project reference to TMXTools

2. **Implement core utility tests**
   - FileUtilsTests.cs
   - StringExtensionsTests.cs

3. **Expand WPF tests**
   - Add converter tests
   - Add control tests
   - Add utility tests

4. **Set up coverage reporting**
   - Configure coverlet
   - Integrate with CI/CD
   - Set up baseline metrics

5. **Document patterns**
   - Create testing guide for contributors
   - Document mocking strategies
   - Share best practices
