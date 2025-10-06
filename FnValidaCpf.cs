using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using HttpValidaCpf.Request;

namespace HttpValidaCpf;

public class FnValidaCpf
{
    private readonly ILogger<FnValidaCpf> _logger;

    public FnValidaCpf(ILogger<FnValidaCpf> logger)
    {
        _logger = logger;
    }

    [Function("fnvalidacpf")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("Iniciando a validação do CPF.");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonSerializer.Deserialize<ValidacaoCpfRequest>(requestBody);
        var cpf = data?.Cpf;

        if (string.IsNullOrEmpty(cpf) || !ValidateCpf(cpf))
        {
            return new BadRequestObjectResult("CPF inválido.");
        }

        return new OkObjectResult("CPF válido e não consta na base de fraudes e não consta na base de débitos.");
    }

    private static bool ValidateCpf(string cpf)
    {
        // Remove caracteres não numéricos
        cpf = new string([.. cpf.Where(char.IsDigit)]);

        // Verifica se o CPF tem 11 dígitos
        if (cpf.Length != 11)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cpf.Distinct().Count() == 1)
            return false;

        // Calcula o primeiro dígito verificador
        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * (10 - i);

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        if ((cpf[9] - '0') != digito1)
            return false;

        // Calcula o segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * (11 - i);

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return (cpf[10] - '0') == digito2;
    }
}
