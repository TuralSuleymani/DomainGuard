using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace DomainGuard;

/// <summary>
/// Provides high-performance guard clause validations for domain models and application code.
/// 
/// This class contains extension methods used to validate input values,
/// enforce invariants, and ensure correct domain behavior.
/// </summary>
public static class Guard
{
    private static GuardException Invalid(string message)
        => new GuardException(message);

    // ---------------------------------------------------------
    // NULL CHECKS
    // ---------------------------------------------------------

    /// <summary>
    /// Ensures that a reference type value is not <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The reference type.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">Automatically inferred argument name.</param>
    /// <returns>The original value if valid.</returns>
    /// <exception cref="GuardException">Thrown when <paramref name="value"/> is null.</exception>
    public static T EnsureNonNull<T>(this T? value,
        [CallerArgumentExpression("value")] string name = "")
        where T : class =>
        value ?? throw Invalid($"{name} cannot be null.");

    /// <summary>
    /// Ensures that a reference type value is <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The reference type.</typeparam>
    /// <param name="value">The value being validated.</param>
    /// <param name="name">Automatically inferred argument name.</param>
    /// <returns><c>null</c> if valid.</returns>
    /// <exception cref="GuardException">Thrown when <paramref name="value"/> is not null.</exception>
    public static T? EnsureNull<T>(this T? value,
        [CallerArgumentExpression("value")] string name = "")
        where T : class =>
        value is null ? value : throw Invalid($"{name} must be null.");

    // ---------------------------------------------------------
    // DEFAULT CHECKS
    // ---------------------------------------------------------

    /// <summary>
    /// Ensures that a struct value is not equal to its default value.
    /// </summary>
    /// <typeparam name="T">A value type.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">Automatically inferred argument name.</param>
    /// <returns>The original value if valid.</returns>
    /// <exception cref="GuardException">Thrown if value equals <c>default(T)</c>.</exception>
    public static T EnsureNotDefault<T>(this T value,
        [CallerArgumentExpression("value")] string name = "")
        where T : struct =>
        value.Equals(default(T))
            ? throw Invalid($"{name} cannot be default.")
            : value;

    /// <summary>
    /// Ensures that a nullable struct value is not <c>null</c> and not equal to its default value.
    /// </summary>
    /// <typeparam name="T">A struct type.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">Automatically inferred argument name.</param>
    /// <returns>The unwrapped struct value.</returns>
    /// <exception cref="GuardException">Thrown if value is null or equals default.</exception>
    public static T EnsureNotDefault<T>(this T? value,
        [CallerArgumentExpression("value")] string name = "")
        where T : struct =>
        value.GetValueOrDefault().Equals(default(T))
            ? throw Invalid($"{name} cannot be null or default.")
            : value.Value;

    // ---------------------------------------------------------
    // NUMERIC CHECKS (INumber<T>)
    // ---------------------------------------------------------

    /// <summary>
    /// Ensures that a numeric value is not zero.
    /// </summary>
    /// <typeparam name="T">A numeric type implementing <see cref="INumber{T}"/>.</typeparam>
    /// <param name="value">Value being validated.</param>
    /// <param name="name">Parameter name.</param>
    /// <returns>The original value.</returns>
    /// <exception cref="GuardException">Thrown when value is zero.</exception>
    public static T EnsureNonZero<T>(this T value,
        [CallerArgumentExpression("value")] string name = "")
        where T : INumber<T> =>
        value != T.Zero ? value : throw Invalid($"{name} cannot be zero.");

    /// <summary>
    /// Ensures that a numeric value is greater than zero.
    /// </summary>
    public static T EnsurePositive<T>(this T value,
        [CallerArgumentExpression("value")] string name = "")
        where T : INumber<T> =>
        value > T.Zero ? value : throw Invalid($"{name} must be positive.");

    /// <summary>
    /// Ensures that a numeric value is greater than or equal to zero.
    /// </summary>
    public static T EnsureNonNegative<T>(this T value,
        [CallerArgumentExpression("value")] string name = "")
        where T : INumber<T> =>
        value >= T.Zero ? value : throw Invalid($"{name} cannot be negative.");

    /// <summary>
    /// Ensures that a value is strictly greater than a specified minimum.
    /// </summary>
    public static T EnsureGreaterThan<T>(this T value, T min,
        [CallerArgumentExpression("value")] string valueName = "",
        [CallerArgumentExpression("min")] string minName = "")
        where T : IComparable<T> =>
        value.CompareTo(min) > 0
            ? value
            : throw Invalid($"{valueName}={value} must be greater than {minName}={min}.");

    /// <summary>
    /// Ensures that a value is greater than or equal to a specified minimum.
    /// </summary>
    public static T EnsureAtLeast<T>(this T value, T min,
        [CallerArgumentExpression("value")] string valueName = "",
        [CallerArgumentExpression("min")] string minName = "")
        where T : IComparable<T> =>
        value.CompareTo(min) >= 0
            ? value
            : throw Invalid($"{valueName}={value} must be at least {min}.");

    /// <summary>
    /// Ensures that a value falls within a specific range.
    /// </summary>
    public static T EnsureWithinRange<T>(this T value, T min, T max,
        bool excludeMin = false, bool excludeMax = false,
        [CallerArgumentExpression("value")] string valueName = "",
        [CallerArgumentExpression("min")] string minName = "",
        [CallerArgumentExpression("max")] string maxName = "")
        where T : IComparable<T>
    {
        bool tooLow = excludeMin ? value.CompareTo(min) <= 0 : value.CompareTo(min) < 0;
        bool tooHigh = excludeMax ? value.CompareTo(max) >= 0 : value.CompareTo(max) > 0;

        if (tooLow || tooHigh)
            throw Invalid($"{valueName}={value} must be between {minName}={min} and {maxName}={max}.");

        return value;
    }

    // ---------------------------------------------------------
    // STRING CHECKS
    // ---------------------------------------------------------

    /// <summary>
    /// Ensures that a string is not null or empty.
    /// </summary>
    public static string EnsureNonEmpty(this string? value,
        [CallerArgumentExpression("value")] string name = "") =>
        string.IsNullOrEmpty(value)
            ? throw Invalid($"{name} cannot be empty.")
            : value!;

    /// <summary>
    /// Ensures that a string is not null, empty, or whitespace.
    /// </summary>
    public static string EnsureNonBlank(this string? value,
        [CallerArgumentExpression("value")] string name = "") =>
        string.IsNullOrWhiteSpace(value)
            ? throw Invalid($"{name} cannot be blank.")
            : value!;

    /// <summary>
    /// Ensures that a string matches a specified regular expression pattern.
    /// </summary>
    public static string EnsureMatchesPattern(this string value, string pattern,
        [CallerArgumentExpression("value")] string name = "") =>
        Regex.IsMatch(value, pattern)
            ? value
            : throw Invalid($"{name} does not match the required pattern.");

    /// <summary>
    /// Ensures that a string is a valid image URL (jpg, png, gif, webp).
    /// </summary>
    public static string EnsureImageUrl(this string value,
        [CallerArgumentExpression("value")] string name = "") =>
        Regex.IsMatch(value, @"^https?:\/\/.+\.(png|gif|webp|jpeg|jpg)(\?.*)?$", RegexOptions.IgnoreCase)
            ? value
            : throw Invalid($"{name} is not a valid image URL.");

    /// <summary>
    /// Ensures that a string is formatted as a valid email address.
    /// </summary>
    public static string EnsureValidEmail(this string value,
        [CallerArgumentExpression("value")] string name = "") =>
        new EmailAddressAttribute().IsValid(value)
            ? value
            : throw Invalid($"{value} is not a valid email address.");

    /// <summary>
    /// Ensures that a string is exactly a specified length.
    /// </summary>
    public static string EnsureExactLength(this string value, int length,
        [CallerArgumentExpression("value")] string name = "") =>
        value.Length == length
            ? value
            : throw Invalid($"{name} must be exactly {length} characters.");

    /// <summary>
    /// Ensures that a string length falls within a specified range.
    /// </summary>
    public static string EnsureLengthInRange(this string value, int min, int max,
        [CallerArgumentExpression("value")] string name = "")
    {
        if (value.Length < min || value.Length > max)
            throw Invalid($"{name} length must be between {min} and {max}, but was {value.Length}.");
        return value;
    }

    // ---------------------------------------------------------
    // COLLECTIONS
    // ---------------------------------------------------------

    /// <summary>
    /// Ensures that an <see cref="IEnumerable{T}"/> contains at least one element.
    /// </summary>
    public static IEnumerable<T> EnsureAny<T>(this IEnumerable<T>? value,
        [CallerArgumentExpression("value")] string name = "") =>
        value is not null && value.Any()
            ? value
            : throw Invalid($"{name} cannot be empty.");

    /// <summary>
    /// Ensures that a non-generic <see cref="ICollection"/> is not null and contains elements.
    /// </summary>
    public static ICollection EnsureNonEmpty(this ICollection? value,
        [CallerArgumentExpression("value")] string name = "") =>
        value != null && value.Count > 0
            ? value
            : throw Invalid($"{name} cannot be empty.");

    // ---------------------------------------------------------
    // ENUM CHECKS
    // ---------------------------------------------------------

    /// <summary>
    /// Ensures that the enum value is a defined member of its enum type.
    /// </summary>
    public static T EnsureEnumValueDefined<T>(this T value,
        [CallerArgumentExpression("value")] string name = "")
        where T : struct, Enum =>
        Enum.IsDefined(value)
            ? value
            : throw Invalid($"{name}={value} is not a valid {typeof(T).Name} value.");

    /// <summary>
    /// Ensures that a string can be parsed into a valid enum value.
    /// </summary>
    public static T EnsureEnumValueDefined<T>(this string value,
        [CallerArgumentExpression("value")] string name = "")
        where T : struct, Enum =>
        Enum.TryParse<T>(value, true, out var parsed) && Enum.IsDefined(parsed)
            ? parsed
            : throw Invalid($"{name}={value} is not a valid {typeof(T).Name} value.");

    // ---------------------------------------------------------
    // DICTIONARIES
    // ---------------------------------------------------------

    /// <summary>
    /// Ensures that the specified dictionary contains the given key.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="dict">The dictionary to check.</param>
    /// <param name="key">The key that must exist in the dictionary.</param>
    /// <param name="name">Automatically inferred argument name.</param>
    /// <returns>The value associated with the key.</returns>
    /// <exception cref="GuardException">Thrown when the key does not exist.</exception>
    public static TValue EnsureKeyExists<TKey, TValue>(
        this IDictionary<TKey, TValue> dict,
        TKey key,
        [CallerArgumentExpression("dict")] string name = "")
    {
        dict.EnsureNonNull(name);

        if (!dict.ContainsKey(key))
            throw Invalid($"{name} does not contain key '{key}'.");

        return dict[key];
    }

    // ---------------------------------------------------------
    // BOOLEAN CHECKS
    // ---------------------------------------------------------

    /// <summary>
    /// Ensures that a boolean value is <c>true</c>.
    /// </summary>
    /// <param name="value">Boolean expression to validate.</param>
    /// <param name="name">Parameter name.</param>
    /// <returns><c>true</c> if valid.</returns>
    /// <exception cref="GuardException">Thrown when value is false.</exception>
    public static bool EnsureTrue(this bool value,
        [CallerArgumentExpression("value")] string name = "") =>
        value ? value : throw Invalid($"{name} must be true.");

    /// <summary>
    /// Ensures that a boolean value is <c>false</c>.
    /// </summary>
    /// <param name="value">Boolean expression to validate.</param>
    /// <param name="name">Parameter name.</param>
    /// <returns><c>false</c> if valid.</returns>
    /// <exception cref="GuardException">Thrown when value is true.</exception>
    public static bool EnsureFalse(this bool value,
        [CallerArgumentExpression("value")] string name = "") =>
        !value ? value : throw Invalid($"{name} must be false.");
}
