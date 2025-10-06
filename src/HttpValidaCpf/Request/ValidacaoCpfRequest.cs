using System.Text.Json.Serialization;

namespace HttpValidaCpf.Request;

public record ValidacaoCpfRequest
{
    [JsonPropertyName("cpf")]
    public string Cpf { get; init; }

    public ValidacaoCpfRequest(string cpf)
    {
        Cpf = cpf;
    }
}