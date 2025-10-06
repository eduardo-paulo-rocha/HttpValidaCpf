using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using HttpValidaCpf.Request;
using HttpValidaCpf.Services;
using System.Text.Json;

namespace HttpValidaCpf.Tests;

public class FnValidaCpfTests
{
    private readonly Mock<ILogger<FnValidaCpf>> _loggerMock;
    private readonly Mock<IValidateDocumentService> _validateServiceMock;
    private readonly FnValidaCpf _function;

    public FnValidaCpfTests()
    {
        _loggerMock = new Mock<ILogger<FnValidaCpf>>();
        _validateServiceMock = new Mock<IValidateDocumentService>();
        _function = new FnValidaCpf(_loggerMock.Object, _validateServiceMock.Object);
    }

    [Fact]
    public async Task Run_WithValidCpf_ReturnsOkResult()
    {
        // Arrange
        var validCpf = "12345678909";
        var request = CreateHttpRequest(validCpf);
        _validateServiceMock.Setup(x => x.ValidateCpf(validCpf)).Returns(true);

        // Act
        var result = await _function.Run(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("CPF válido", okResult.Value?.ToString() ?? string.Empty);
    }

    [Fact]
    public async Task Run_WithInvalidCpf_ReturnsBadRequest()
    {
        // Arrange
        var invalidCpf = "11111111111";
        var request = CreateHttpRequest(invalidCpf);
        _validateServiceMock.Setup(x => x.ValidateCpf(invalidCpf)).Returns(false);

        // Act
        var result = await _function.Run(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("inválido", badRequestResult.Value?.ToString() ?? string.Empty);
    }

    [Fact]
    public async Task Run_WithEmptyCpf_ReturnsBadRequest()
    {
        // Arrange
        var request = CreateHttpRequest("");

        // Act
        var result = await _function.Run(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Run_WithNullCpf_ReturnsBadRequest()
    {
        // Arrange
        var request = CreateHttpRequest(null);

        // Act
        var result = await _function.Run(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    private static HttpRequest CreateHttpRequest(string? cpf)
    {
        var request = new Mock<HttpRequest>();
        var requestBody = JsonSerializer.Serialize(new ValidacaoCpfRequest(cpf ?? string.Empty));
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
        
        request.Setup(x => x.Body).Returns(stream);
        
        return request.Object;
    }
}