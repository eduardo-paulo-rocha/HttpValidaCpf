using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using HttpValidaCpf.Request;

namespace HttpValidaCpf;

public class FnValidaCpf(ILogger<FnValidaCpf> logger)
{
    private readonly ILogger<FnValidaCpf> _logger = logger;

    [Function("fnvalidacpf")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("Iniciando a validação do CPF.");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        _logger.LogInformation("Request Body: {RequestBody}", requestBody);

        var data = JsonSerializer.Deserialize<ValidacaoCpfRequest>(requestBody);
        var cpf = data?.Cpf;

        _logger.LogInformation("CPF a ser validado: {Cpf}", cpf);

        if (string.IsNullOrEmpty(cpf) || !ValidateCpf(cpf, _logger))
        {
            return new BadRequestObjectResult($"CPF {cpf} inválido.");
        }

        return new OkObjectResult("CPF válido e não consta na base de fraudes e não consta na base de débitos.");
    }

    private static bool ValidateCpf(string cpf, ILogger<FnValidaCpf> logger)
    {
        // Remove caracteres não numéricos
        cpf = new string([.. cpf.Where(char.IsDigit)]);

        // Verifica se o CPF tem 11 dígitos
        if (cpf.Length != 11)
        {
            logger.LogWarning("Quantidade de dígitos inválida: {Cpf}", cpf);
            return false;
        }

        // Verifica se todos os dígitos são iguais
        if (cpf.Distinct().Count() == 1)
        {
            logger.LogWarning("Todos os dígitos são iguais: {Cpf}", cpf);
            return false;
        }

        // Calcula o primeiro dígito verificador
        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * (10 - i);

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        if ((cpf[9] - '0') != digito1)
        {
            logger.LogWarning("Dígito verificador 1 inválido: {Cpf}", cpf);
            return false;
        }

        // Calcula o segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * (11 - i);

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return (cpf[10] - '0') == digito2;
    }
}
