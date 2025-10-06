using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using HttpValidaCpf.Request;
using HttpValidaCpf.Services;

namespace HttpValidaCpf;

public class FnValidaCpf(ILogger<FnValidaCpf> logger, IValidateDocumentService validateDocumentService)
{
    private readonly ILogger<FnValidaCpf> _logger = logger;
    private readonly IValidateDocumentService _validateDocumentService = validateDocumentService;

    [Function("fnvalidacpf")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("Iniciando a validação do CPF.");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        _logger.LogInformation("Request Body: {RequestBody}", requestBody);

        var data = JsonSerializer.Deserialize<ValidacaoCpfRequest>(requestBody);
        var cpf = data?.Cpf;

        _logger.LogInformation("CPF a ser validado: {Cpf}", cpf);

        if (string.IsNullOrEmpty(cpf) || !_validateDocumentService.ValidateCpf(cpf))
        {
            return new BadRequestObjectResult($"[{DateTime.UtcNow}] CPF {cpf} inválido.");
        }

        return new OkObjectResult($"[{DateTime.UtcNow}] CPF válido e não consta na base de fraudes e não consta na base de débitos.");
    }
}
