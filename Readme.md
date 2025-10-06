# CPF Validator Azure Function

This project implements a CPF (Brazilian individual taxpayer registry) validation service using Azure Functions. The service exposes an HTTP endpoint that validates CPF numbers according to the official calculation algorithm.

## Project Structure

```
├── src/
│   └── HttpValidaCpf/
│       ├── FnValidaCpf.cs              # Main Azure Function handler
│       ├── Program.cs                  # Application startup and DI configuration
│       ├── Services/
│       │   ├── IValidateDocumentService.cs    # Service interface
│       │   └── ValidateDocumentService.cs     # CPF validation implementation
│       └── Request/
│           └── ValidacaoCpfRequest.cs         # Request model
└── tests/
    └── ValidadorCpf.Tests/
        ├── FnValidaCpfTests.cs               # Function tests
        └── ValidateDocumentServiceTests.cs    # Service tests
```

## Features

- Validates CPF numbers through an HTTP POST endpoint
- Removes non-numeric characters (handles formatted CPFs)
- Validates CPF structure:
  - Must be 11 digits long
  - Cannot have all digits equal
  - Validates check digits using the official algorithm
- Provides detailed validation feedback
- Includes comprehensive test coverage

## API Specification

### Endpoint

`POST /api/fnvalidacpf`

### Request Format

```json
{
    "cpf": "string"
}
```

### Response Format

- Success (200 OK):
```json
"[timestamp] CPF válido e não consta na base de fraudes e não consta na base de débitos."
```

- Error (400 Bad Request):
```json
"[timestamp] CPF {cpf} inválido."
```

## Technical Details

- Built with .NET 8 and Azure Functions v4
- Uses dependency injection for services
- Includes Application Insights integration
- Implements logging throughout the application
- Uses modern C# features (record types, primary constructors)

## Testing

The project includes comprehensive unit tests covering:
- Function level validation
- Service level CPF validation
- Different CPF formats (with/without punctuation)
- Edge cases (null, empty, invalid formats)
- Valid and invalid scenarios

## Dependencies

- Microsoft.Azure.Functions.Worker
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging
- System.Text.Json

## Running Locally

1. Install the Azure Functions Core Tools
2. Clone the repository
3. Navigate to the src/HttpValidaCpf directory
4. Run:
```sh
func start
```

## Development

The solution follows clean architecture principles with:
- Clear separation of concerns
- Interface-based design
- Dependency injection
- Comprehensive logging
- Full test coverage

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request