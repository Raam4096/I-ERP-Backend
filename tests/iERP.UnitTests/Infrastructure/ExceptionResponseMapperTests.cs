using iERP.Infrastructure.Exceptions;
using iERP.SharedKernel.Exceptions;

namespace iERP.UnitTests.Infrastructure;

public sealed class ExceptionResponseMapperTests
{
    [Theory]
    [InlineData(typeof(ValidationException), 400, ErrorCodes.ValidationError)]
    [InlineData(typeof(NotFoundException), 404, ErrorCodes.NotFound)]
    [InlineData(typeof(ForbiddenException), 403, ErrorCodes.Forbidden)]
    [InlineData(typeof(UnauthorizedException), 401, ErrorCodes.Unauthorized)]
    [InlineData(typeof(BusinessRuleException), 409, ErrorCodes.BusinessRuleViolation)]
    public void Map_known_domain_exceptions_to_status_and_error_code(
        Type exceptionType,
        int expectedStatus,
        string expectedErrorCode)
    {
        var exception = Create(exceptionType);

        var (status, error) = ExceptionResponseMapper.Map(exception);

        status.Should().Be(expectedStatus);
        error.Success.Should().BeFalse();
        error.Error.Should().Be(expectedErrorCode);
        error.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Map_duplicate_record_uses_caller_error_code_and_message()
    {
        var exception = new BusinessRuleException(
            ErrorCodes.DuplicateRecord,
            "A lead with the same email or phone already exists.");

        var (status, error) = ExceptionResponseMapper.Map(exception);

        status.Should().Be(409);
        error.Error.Should().Be(ErrorCodes.DuplicateRecord);
        error.Message.Should().Be("A lead with the same email or phone already exists.");
    }

    [Fact]
    public void Map_validation_includes_field_and_errors()
    {
        var exception = new ValidationException(
            "One or more validation errors occurred.",
            "Email",
            ["'Email' must not be empty."]);

        var (status, error) = ExceptionResponseMapper.Map(exception);

        status.Should().Be(400);
        error.Field.Should().Be("Email");
        error.Errors.Should().ContainSingle().Which.Should().Contain("Email");
    }

    [Fact]
    public void Map_unknown_exception_to_generic_500()
    {
        var (status, error) = ExceptionResponseMapper.Map(new InvalidOperationException("secret internals"));

        status.Should().Be(500);
        error.Error.Should().Be(ErrorCodes.InternalError);
        error.Message.Should().Be("An unexpected error occurred.");
        error.Message.Should().NotContain("secret");
    }

    private static Exception Create(Type type) => type.Name switch
    {
        nameof(ValidationException) => new ValidationException("One or more validation errors occurred."),
        nameof(NotFoundException) => new NotFoundException("Lead was not found."),
        nameof(ForbiddenException) => new ForbiddenException("Tenant context is required."),
        nameof(UnauthorizedException) => new UnauthorizedException("Invalid credentials."),
        nameof(BusinessRuleException) => new BusinessRuleException(
            ErrorCodes.BusinessRuleViolation,
            "Lead has already been converted to an opportunity."),
        _ => throw new InvalidOperationException($"Unsupported type {type.Name}")
    };
}
