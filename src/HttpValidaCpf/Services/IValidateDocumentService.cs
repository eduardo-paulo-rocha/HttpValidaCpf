using Microsoft.Extensions.Logging;

namespace HttpValidaCpf.Services;

public interface IValidateDocumentService
{
    bool ValidateCpf(string cpf);
}