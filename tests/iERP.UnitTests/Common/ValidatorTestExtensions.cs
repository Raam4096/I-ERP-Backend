using FluentValidation;
using FluentValidation.Results;

namespace iERP.UnitTests.Common;

public static class ValidatorTestExtensions
{
    public static async Task ShouldHaveValidationErrorForAsync<T>(
        this IValidator<T> validator,
        T instance,
        string expectedPropertyOrMessageHint)
    {
        var result = await validator.ValidateAsync(instance);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(
            e => e.PropertyName.Contains(expectedPropertyOrMessageHint, StringComparison.OrdinalIgnoreCase)
                 || e.ErrorMessage.Contains(expectedPropertyOrMessageHint, StringComparison.OrdinalIgnoreCase),
            because: $"expected a validation error involving '{expectedPropertyOrMessageHint}'. Errors: {Format(result)}");
    }

    public static async Task ShouldBeValidAsync<T>(this IValidator<T> validator, T instance)
    {
        var result = await validator.ValidateAsync(instance);
        result.IsValid.Should().BeTrue(because: Format(result));
    }

    private static string Format(ValidationResult result) =>
        string.Join("; ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
}
