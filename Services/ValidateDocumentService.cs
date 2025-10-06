using Microsoft.Extensions.Logging;

namespace HttpValidaCpf.Services;

public class ValidateDocumentService : IValidateDocumentService
{
    private readonly ILogger<ValidateDocumentService> _logger;

    public ValidateDocumentService(ILogger<ValidateDocumentService> logger)
    {
        _logger = logger;
    }

    public bool ValidateCpf(string cpf)
    {
        // Remove caracteres não numéricos
        cpf = new string([.. cpf.Where(char.IsDigit)]);

        // Verifica se o CPF tem 11 dígitos
        if (cpf.Length != 11)
        {
            _logger.LogWarning("Quantidade de dígitos inválida: {Cpf}", cpf);
            return false;
        }

        // Verifica se todos os dígitos são iguais
        if (cpf.Distinct().Count() == 1)
        {
            _logger.LogWarning("Todos os dígitos são iguais: {Cpf}", cpf);
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
            _logger.LogWarning("Dígito verificador 1 inválido: {Cpf}", cpf);
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