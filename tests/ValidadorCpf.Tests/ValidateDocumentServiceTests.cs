using Microsoft.Extensions.Logging;
using Moq;
using HttpValidaCpf.Services;

namespace HttpValidaCpf.Tests;

public class ValidateDocumentServiceTests
{
    private readonly ILogger<ValidateDocumentService> _logger;
    private readonly ValidateDocumentService _service;

    public ValidateDocumentServiceTests()
    {
        _logger = Mock.Of<ILogger<ValidateDocumentService>>();
        _service = new ValidateDocumentService(_logger);
    }

    [Theory]
    [InlineData("52998224725")] // Valid CPF
    public void ValidateCpf_WithValidCpf_ReturnsTrue(string cpf)
    {
        // Act
        var result = _service.ValidateCpf(cpf);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("11111111111")] // All same digits
    [InlineData("12345678900")] // Invalid check digits
    [InlineData("123456789")] // Too short
    [InlineData("123456789012")] // Too long
    public void ValidateCpf_WithInvalidCpf_ReturnsFalse(string cpf)
    {
        // Act
        var result = _service.ValidateCpf(cpf);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("529.982.247-25")] // With punctuation
    public void ValidateCpf_WithFormattedCpf_ValidatesCorrectly(string cpf)
    {
        // Act
        var result = _service.ValidateCpf(cpf);

        // Assert
        Assert.True(result);
    }
}