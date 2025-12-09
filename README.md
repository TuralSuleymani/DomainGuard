# DomainGuard

<p align="center">
  <img src="assets/domain-guard-banner.png" alt="DomainGuard Banner" width="800">
</p>

<p align="center">
  <a href="https://github.com/YOUR_USERNAME/DomainGuard/actions">
    <img src="https://img.shields.io/github/actions/workflow/status/YOUR_USERNAME/DomainGuard/ci.yml?style=for-the-badge&logo=github" alt="Build Status">
  </a>
  <a href="https://www.nuget.org/packages/DomainGuard/">
    <img src="https://img.shields.io/nuget/v/DomainGuard.svg?style=for-the-badge&logo=nuget" alt="NuGet Version">
  </a>
  <a href="https://www.nuget.org/packages/DomainGuard/">
    <img src="https://img.shields.io/nuget/dt/DomainGuard.svg?style=for-the-badge&color=blue" alt="NuGet Downloads">
  </a>
  <a href="https://YOUR_USERNAME.github.io/DomainGuard/coverage">
    <img src="https://YOUR_USERNAME.github.io/DomainGuard/coverage-badge.json" alt="Coverage Badge">
  </a>
  <a href="LICENSE">
    <img src="https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge" alt="MIT License">
  </a>
</p>

---

# 📘 Detailed Explanation

DomainGuard is a lightweight, high-performance guard clause library designed specifically for:

- Domain-Driven Design (DDD)
- Clean Architecture
- Enterprise applications
- Value Objects, Aggregates, Entities, Domain Services
- High-performance APIs and microservices

### Key Principles

**Zero dependencies**  
DomainGuard does not rely on external packages such as FluentValidation. It is intentionally minimal.

**Zero allocations**  
Only pure value checks. No memory pressure, no object creation.

**Modern Generic Math API**  
Uses `.NET INumber<T>` for universal numeric validation without boxing.

**Expressive Errors**  
Errors are automatically enriched using `CallerArgumentExpression`.

**Fast**  
Pure inlineable guard clauses with no reflection, no attributes, no scanning.

---

# 🚀 Usage

## Install via NuGet

```bash
dotnet add package DomainGuard
```

## Basic Examples

```csharp
using DomainGuard;

// Null Checks
name.EnsureNonNull();
address.EnsureNull();

// Default Checks
id.EnsureNotDefault();

// Numeric Checks
quantity.EnsurePositive();
balance.EnsureNonNegative();
amount.EnsureGreaterThan(5);
price.EnsureWithinRange(0, 100);

// String Checks
email.EnsureValidEmail();
title.EnsureNonBlank();
fileUrl.EnsureImageUrl();
sku.EnsureMatchesPattern(@"^[A-Z0-9]+$");

// Collections
items.EnsureAny();
collection.EnsureNonEmpty();

// Enums
status.EnsureEnumValueDefined();

// Dictionaries
dict.EnsureKeyExists("key");

// Booleans
isValid.EnsureTrue();
isDeleted.EnsureFalse();
```

---

## Value Object Example

```csharp
public sealed class ProductName
{
    public string Value { get; }

    public ProductName(string value)
    {
        Value = value
            .EnsureNonBlank()
            .EnsureLengthInRange(3, 100);
    }
}
```

---

## Aggregate Example

```csharp
public void ChangePrice(decimal newPrice)
{
    newPrice.EnsurePositive();

    Price = newPrice;
}
```

---

# 🧪 Unit Tests

DomainGuard includes full test coverage with:

- xUnit  
- FluentAssertions  
- Coverage via Coverlet + ReportGenerator  

Naming style:

```
MethodName_WhenCondition_ShouldExpectedBehavior
```

Example:

```csharp
EnsurePositive_WhenValueIsGreaterThanZero_ShouldReturnValue()
```

---



# 🎯 Why DomainGuard?

| Feature | Benefit |
|--------|---------|
| **Zero Dependencies** | No heavy packages, minimalistic and clean |
| **Fast** | No reflection, no boxing |
| **CallerArgumentExpression** | Beautiful domain error messages |
| **Generic Math (`INumber<T>`)** | Universal numeric rule support |
| **100% tested** | Confidence in production |
| **Designed for DDD** | Perfect for Value Objects & Aggregates |
| **NuGet-ready** | Metadata, docs, CI/CD |

---

# 👥 Contributors

| Name | Role |
|------|------|
| **Tural Suleymani** | Author, Maintainer |

---

# 📄 License

This project is licensed under the **MIT License**.  
See the `LICENSE` file for details.

---

# 💬 Support

- **Issues:** https://github.com/YOUR_USERNAME/DomainGuard/issues  
- **PRs:** Welcome  
- **Wiki:** Coming soon  

---

# ⭐ If you like DomainGuard...

Please **star the repo** on GitHub . It helps visibility and supports future development!

