using System.Collections;
using FluentAssertions;

namespace DomainGuard.Tests;

public class GuardTests
{
    private enum TestEnum
    {
        None = 0,
        First = 1,
        Second = 2
    }

    // ---------------------------------------------------------
    // NULL CHECKS
    // ---------------------------------------------------------

    [Fact]
    public void EnsureNonNull_WhenValueIsNotNull_ShouldReturnOriginalValue()
    {
        // Arrange
        string value = "hello";

        // Act
        var result = value.EnsureNonNull();

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void EnsureNonNull_WhenValueIsNull_ShouldThrowGuardException()
    {
        // Arrange
        string? value = null;

        // Act
        Action act = () => value.EnsureNonNull();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*value*cannot be null*");
    }

    [Fact]
    public void EnsureNull_WhenValueIsNull_ShouldReturnNull()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.EnsureNull();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void EnsureNull_WhenValueIsNotNull_ShouldThrowGuardException()
    {
        // Arrange
        string? value = "not-null";

        // Act
        Action act = () => value.EnsureNull();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*value*must be null*");
    }

    // ---------------------------------------------------------
    // DEFAULT CHECKS
    // ---------------------------------------------------------

    [Fact]
    public void EnsureNotDefault_WhenStructIsNotDefault_ShouldReturnValue()
    {
        // Arrange
        int value = 10;

        // Act
        var result = value.EnsureNotDefault();

        // Assert
        result.Should().Be(10);
    }

    [Fact]
    public void EnsureNotDefault_WhenStructIsDefault_ShouldThrowGuardException()
    {
        // Arrange
        int value = default;

        // Act
        Action act = () => value.EnsureNotDefault();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*value*cannot be default*");
    }

    [Fact]
    public void EnsureNotDefault_WhenNullableStructHasNonDefaultValue_ShouldReturnValue()
    {
        // Arrange
        int? value = 5;

        // Act
        var result = value.EnsureNotDefault();

        // Assert
        result.Should().Be(5);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void EnsureNotDefault_WhenNullableStructIsNullOrDefault_ShouldThrowGuardException(int? value)
    {
        // Arrange

        // Act
        Action act = () => value.EnsureNotDefault();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*value*cannot be null or default*");
    }

    // ---------------------------------------------------------
    // NUMERIC CHECKS (INumber<T>)
    // ---------------------------------------------------------

    [Theory]
    [InlineData(1)]
    [InlineData(-5)]
    public void EnsureNonZero_WhenValueIsNonZero_ShouldReturnValue(int value)
    {
        // Arrange

        // Act
        var result = value.EnsureNonZero();

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void EnsureNonZero_WhenValueIsZero_ShouldThrowGuardException()
    {
        // Arrange
        int value = 0;

        // Act
        Action act = () => value.EnsureNonZero();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*value*cannot be zero*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void EnsurePositive_WhenValueIsGreaterThanZero_ShouldReturnValue(int value)
    {
        // Arrange

        // Act
        var result = value.EnsurePositive();

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void EnsurePositive_WhenValueIsZeroOrNegative_ShouldThrowGuardException(int value)
    {
        // Arrange

        // Act
        Action act = () => value.EnsurePositive();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*value*must be positive*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void EnsureNonNegative_WhenValueIsZeroOrPositive_ShouldReturnValue(int value)
    {
        // Arrange

        // Act
        var result = value.EnsureNonNegative();

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void EnsureNonNegative_WhenValueIsNegative_ShouldThrowGuardException()
    {
        // Arrange
        int value = -1;

        // Act
        Action act = () => value.EnsureNonNegative();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*value*cannot be negative*");
    }

    [Theory]
    [InlineData(10, 5)]
    [InlineData(0, -1)]
    public void EnsureGreaterThan_WhenValueIsGreaterThanMin_ShouldReturnValue(int value, int min)
    {
        // Arrange

        // Act
        var result = value.EnsureGreaterThan(min);

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [InlineData(5, 5)]
    [InlineData(5, 10)]
    public void EnsureGreaterThan_WhenValueIsLessThanOrEqualToMin_ShouldThrowGuardException(int value, int min)
    {
        // Arrange

        // Act
        Action act = () => value.EnsureGreaterThan(min);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be greater than*");
    }

    [Theory]
    [InlineData(5, 5)]
    [InlineData(10, 5)]
    public void EnsureAtLeast_WhenValueIsGreaterThanOrEqualToMin_ShouldReturnValue(int value, int min)
    {
        // Arrange

        // Act
        var result = value.EnsureAtLeast(min);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void EnsureAtLeast_WhenValueIsLessThanMin_ShouldThrowGuardException()
    {
        // Arrange
        int value = 4;
        int min = 5;

        // Act
        Action act = () => value.EnsureAtLeast(min);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be at least*");
    }

    [Theory]
    [InlineData(5, 0, 10)]
    [InlineData(0, 0, 10)]
    [InlineData(10, 0, 10)]
    public void EnsureWithinRange_WhenValueWithinInclusiveRange_ShouldReturnValue(int value, int min, int max)
    {
        // Arrange

        // Act
        var result = value.EnsureWithinRange(min, max);

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [InlineData(-1, 0, 10)]
    [InlineData(11, 0, 10)]
    public void EnsureWithinRange_WhenValueOutsideInclusiveRange_ShouldThrowGuardException(int value, int min, int max)
    {
        // Arrange

        // Act
        Action act = () => value.EnsureWithinRange(min, max);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be between*");
    }

    [Fact]
    public void EnsureWithinRange_WhenExcludeMinAndValueEqualsMin_ShouldThrowGuardException()
    {
        // Arrange
        int value = 0;
        int min = 0;
        int max = 10;

        // Act
        Action act = () => value.EnsureWithinRange(min, max, excludeMin: true);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be between*");
    }

    [Fact]
    public void EnsureWithinRange_WhenExcludeMaxAndValueEqualsMax_ShouldThrowGuardException()
    {
        // Arrange
        int value = 10;
        int min = 0;
        int max = 10;

        // Act
        Action act = () => value.EnsureWithinRange(min, max, excludeMin: false, excludeMax: true);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be between*");
    }

    // ---------------------------------------------------------
    // STRING CHECKS
    // ---------------------------------------------------------

    [Fact]
    public void EnsureNonEmpty_WhenStringHasValue_ShouldReturnString()
    {
        // Arrange
        string value = "abc";

        // Act
        var result = value.EnsureNonEmpty();

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EnsureNonEmpty_WhenStringIsNullOrEmpty_ShouldThrowGuardException(string? value)
    {
        // Arrange

        // Act
        Action act = () => value.EnsureNonEmpty();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be empty*");
    }

    [Fact]
    public void EnsureNonBlank_WhenStringHasNonWhitespace_ShouldReturnString()
    {
        // Arrange
        string value = "  abc  ";

        // Act
        var result = value.EnsureNonBlank();

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EnsureNonBlank_WhenStringIsNullEmptyOrWhitespace_ShouldThrowGuardException(string? value)
    {
        // Arrange

        // Act
        Action act = () => value.EnsureNonBlank();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be blank*");
    }

    [Fact]
    public void EnsureMatchesPattern_WhenValueMatchesPattern_ShouldReturnValue()
    {
        // Arrange
        string value = "ABC123";
        string pattern = "^[A-Z0-9]+$";

        // Act
        var result = value.EnsureMatchesPattern(pattern);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void EnsureMatchesPattern_WhenValueDoesNotMatchPattern_ShouldThrowGuardException()
    {
        // Arrange
        string value = "abc-123";
        string pattern = "^[A-Z0-9]+$";

        // Act
        Action act = () => value.EnsureMatchesPattern(pattern);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*does not match the required pattern*");
    }

    [Theory]
    [InlineData("https://example.com/image.jpg")]
    [InlineData("http://example.com/path/image.PNG")]
    [InlineData("https://example.org/img/avatar.webp?size=200")]
    public void EnsureImageUrl_WhenValidImageUrl_ShouldReturnValue(string value)
    {
        // Arrange

        // Act
        var result = value.EnsureImageUrl();

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("https://example.com/image.txt")]
    [InlineData("ftp://example.com/image.jpg")]
    public void EnsureImageUrl_WhenInvalidImageUrl_ShouldThrowGuardException(string value)
    {
        // Arrange

        // Act
        Action act = () => value.EnsureImageUrl();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*is not a valid image URL*");
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@domain.co")]
    public void EnsureValidEmail_WhenValidEmail_ShouldReturnValue(string email)
    {
        // Arrange

        // Act
        var result = email.EnsureValidEmail();

        // Assert
        result.Should().Be(email);
    }

    [Theory]
    [InlineData("plain-address")]
    [InlineData("user@")]
    [InlineData("@domain.com")]
    public void EnsureValidEmail_WhenInvalidEmail_ShouldThrowGuardException(string email)
    {
        // Arrange

        // Act
        Action act = () => email.EnsureValidEmail();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*is not a valid email address*");
    }

    [Fact]
    public void EnsureExactLength_WhenLengthMatches_ShouldReturnValue()
    {
        // Arrange
        string value = "hello";

        // Act
        var result = value.EnsureExactLength(5);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void EnsureExactLength_WhenLengthDoesNotMatch_ShouldThrowGuardException()
    {
        // Arrange
        string value = "hello";

        // Act
        Action act = () => value.EnsureExactLength(3);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be exactly 3 characters*");
    }

    [Theory]
    [InlineData("hello", 3, 10)]
    [InlineData("abc", 3, 3)]
    [InlineData("1234567890", 1, 10)]
    public void EnsureLengthInRange_WhenLengthWithinRange_ShouldReturnValue(string value, int min, int max)
    {
        // Arrange

        // Act
        var result = value.EnsureLengthInRange(min, max);

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [InlineData("hi", 3, 10)]
    [InlineData("this is too long", 1, 5)]
    public void EnsureLengthInRange_WhenLengthOutsideRange_ShouldThrowGuardException(string value, int min, int max)
    {
        // Arrange

        // Act
        Action act = () => value.EnsureLengthInRange(min, max);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*length must be between*");
    }

    // ---------------------------------------------------------
    // COLLECTION CHECKS
    // ---------------------------------------------------------

    [Fact]
    public void EnsureAny_WhenEnumerableHasElements_ShouldReturnEnumerable()
    {
        // Arrange
        IEnumerable<int> values = new[] { 1, 2, 3 };

        // Act
        var result = values.EnsureAny();

        // Assert
        result.Should().BeEquivalentTo(values);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EnsureAny_WhenEnumerableIsNullOrEmpty_ShouldThrowGuardException(bool useNull)
    {
        // Arrange
        IEnumerable<int>? values = useNull ? null : Array.Empty<int>();

        // Act
        Action act = () => values.EnsureAny();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be empty*");
    }

    [Fact]
    public void EnsureNonEmpty_WhenICollectionHasElements_ShouldReturnCollection()
    {
        // Arrange
        ICollection list = new ArrayList { 1, 2 };

        // Act
        var result = list.EnsureNonEmpty();

        // Assert
        result.Should().BeSameAs(list);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EnsureNonEmpty_WhenICollectionIsNullOrEmpty_ShouldThrowGuardException(bool useNull)
    {
        // Arrange
        ICollection? collection = useNull ? null : new ArrayList();

        // Act
        Action act = () => collection.EnsureNonEmpty();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be empty*");
    }

    // ---------------------------------------------------------
    // ENUM CHECKS
    // ---------------------------------------------------------

    [Fact]
    public void EnsureEnumValueDefined_WhenEnumValueIsDefined_ShouldReturnValue()
    {
        // Arrange
        TestEnum value = TestEnum.First;

        // Act
        var result = value.EnsureEnumValueDefined();

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void EnsureEnumValueDefined_WhenEnumValueIsUndefined_ShouldThrowGuardException()
    {
        // Arrange
        TestEnum value = (TestEnum)999;

        // Act
        Action act = () => value.EnsureEnumValueDefined();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*is not a valid TestEnum value*");
    }

    [Fact]
    public void EnsureEnumValueDefined_WhenStringRepresentsValidEnum_ShouldReturnParsedValue()
    {
        // Arrange
        string value = "First";

        // Act
        var result = value.EnsureEnumValueDefined<TestEnum>();

        // Assert
        result.Should().Be(TestEnum.First);
    }

    [Fact]
    public void EnsureEnumValueDefined_WhenStringIsInvalidEnum_ShouldThrowGuardException()
    {
        // Arrange
        string value = "InvalidEnumValue";

        // Act
        Action act = () => value.EnsureEnumValueDefined<TestEnum>();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*is not a valid TestEnum value*");
    }

    // ---------------------------------------------------------
    // DICTIONARIES
    // ---------------------------------------------------------

    [Fact]
    public void EnsureKeyExists_WhenKeyExists_ShouldReturnValue()
    {
        // Arrange
        var dict = new Dictionary<string, int>
        {
            ["age"] = 30
        };

        // Act
        var result = dict.EnsureKeyExists("age");

        // Assert
        result.Should().Be(30);
    }

    [Fact]
    public void EnsureKeyExists_WhenKeyDoesNotExist_ShouldThrowGuardException()
    {
        // Arrange
        var dict = new Dictionary<string, int>();

        // Act
        Action act = () => dict.EnsureKeyExists("missingKey");

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*does not contain key 'missingKey'*");
    }

    // ---------------------------------------------------------
    // BOOLEAN CHECKS
    // ---------------------------------------------------------

    [Fact]
    public void EnsureTrue_WhenValueIsTrue_ShouldReturnTrue()
    {
        // Arrange
        bool value = true;

        // Act
        var result = value.EnsureTrue();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EnsureTrue_WhenValueIsFalse_ShouldThrowGuardException()
    {
        // Arrange
        bool value = false;

        // Act
        Action act = () => value.EnsureTrue();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be true*");
    }

    [Fact]
    public void EnsureFalse_WhenValueIsFalse_ShouldReturnFalse()
    {
        // Arrange
        bool value = false;

        // Act
        var result = value.EnsureFalse();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EnsureFalse_WhenValueIsTrue_ShouldThrowGuardException()
    {
        // Arrange
        bool value = true;

        // Act
        Action act = () => value.EnsureFalse();

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be false*");
    }
}
