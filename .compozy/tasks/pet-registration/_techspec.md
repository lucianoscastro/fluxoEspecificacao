# Technical Specification: Pet Registration API

## Executive Summary

This document specifies the technical design for the Pet Registration API endpoint. The endpoint will allow client applications to register a pet with its name, breed, date of birth, and tutor's name.

The implementation strategy uses ASP.NET Core Minimal APIs to define the endpoint and store the records in a thread-safe in-memory `ConcurrentDictionary` collection. The primary technical trade-off is the choice of in-memory storage, which offers instantaneous setup and zero database overhead but does not persist data across application restarts.

## System Architecture

### Component Overview

The system consists of a single application component (`fluxoEspecificacao`) and its corresponding test project (`fluxoEspecificacao.Tests`).

- **HTTP Request Pipeline**: Handled by ASP.NET Core middleware, which parses JSON payloads and handles routing.
- **Endpoint Route Handler**: Registered directly in `Program.cs` to intercept `POST /pets` requests.
- **Validation Engine**: Performs inline check of the request payload rules before storage.
- **In-Memory Store**: A singleton registration of `ConcurrentDictionary<Guid, Pet>` providing thread-safe storage.

```
[Client App] ---> [POST /pets] ---> [Program.cs Middleware]
                                           |
                                   [Validation Logic]
                                           |
                                  (If Valid: Store)
                                           |
                                           v
                             [ConcurrentDictionary<Guid, Pet>]
```

## Implementation Design

### Core Interfaces

The core data models are defined as C# positional records to represent request payloads and stored entities.

```csharp
namespace fluxoEspecificacao;

public record Pet(Guid Id, string Name, string Breed, DateTime BirthDate, string TutorName);

public record RegisterPetRequest(string Name, string Breed, DateTime BirthDate, string TutorName);
```

### Data Models

#### Request Payload (`RegisterPetRequest`)
- `Name`: String, required, non-empty, non-whitespace.
- `Breed`: String, required, non-empty. Free-text.
- `BirthDate`: DateTime, required. Must be in the past (less than or equal to current system time UTC).
- `TutorName`: String, required, non-empty.

#### Stored Entity & Response Payload (`Pet`)
- `Id`: Guid, required. System-generated unique identifier.
- `Name`: String.
- `Breed`: String.
- `BirthDate`: DateTime.
- `TutorName`: String.

### API Endpoints

#### Register Pet
- **Method**: `POST`
- **Path**: `/pets`
- **Request Format**: `application/json`
- **Response Format**: `application/json`

##### Success Response (201 Created)
- **Header**: `Location: /pets/{id}`
- **Body**: Standard JSON serialization of the `Pet` record.

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Bidu",
  "breed": "SRD",
  "birthDate": "2020-05-15T00:00:00Z",
  "tutorName": "Maria Silva"
}
```

##### Validation Error Response (400 Bad Request)
- **Format**: RFC 7807 Problem Details (`application/problem+json`).
- **Body**: Detailed dictionary of fields that failed validation.

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "BirthDate": ["Birth date must be in the past."]
  }
}
```

## Integration Points

No external integration points are required. This feature interacts only with internal memory.

## Impact Analysis

| Component | Impact Type | Description and Risk | Required Action |
|-----------|-------------|---------------------|-----------------|
| `Program.cs` | Modified | Added endpoint mapping, model records, validation logic, and dependency injection. Low risk. | Implement the model and map the route handler. |
| `fluxoEspecificacao.csproj` | Modified | Add `Microsoft.AspNetCore.Mvc.Testing` package to the test project dependencies. Low risk. | Add PackageReference. |
| `UnitTest1.cs` | Modified | Replace template test with real integration tests using `WebApplicationFactory`. Low risk. | Implement HTTP E2E tests. |

## Testing Approach

### Integration Tests

We will use `WebApplicationFactory<Program>` to spin up the API in-memory and perform HTTP tests using `HttpClient`.

- **Critical Scenarios**:
  - **Success Case**: Send a valid JSON payload to `POST /pets`. Assert 201 Created status, a valid Guid ID in response, and correct Echo data.
  - **Missing Fields Case**: Send empty strings for required fields. Assert 400 Bad Request and validation messages in the RFC 7807 response.
  - **Future Date Case**: Send a birth date in the future. Assert 400 Bad Request and check the error details for the "BirthDate" field.

## Development Sequencing

### Build Order

1. **Dependency Installation**: Add the NuGet package `Microsoft.AspNetCore.Mvc.Testing` to the tests project `fluxoEspecificacao.Tests.csproj`. (No dependencies)
2. **Define Data Records**: Add the `Pet` and `RegisterPetRequest` records in `Program.cs`. (Depends on step 1)
3. **Register Storage and Endpoint**: Configure DI for `ConcurrentDictionary<Guid, Pet>` and map `POST /pets` endpoint in `Program.cs` with validation rules. (Depends on step 2)
4. **Implement Integration Tests**: Write WebApplicationFactory-based tests in the test project. (Depends on step 3)

### Technical Dependencies

- Installation of `Microsoft.AspNetCore.Mvc.Testing` NuGet library in the test project.

## Monitoring and Observability

- **Logging**: The application will log intake requests and validation failures using the standard ASP.NET Core `ILogger`.
- **Metrics**: Standard ASP.NET Core endpoint request count and response time metrics.

## Technical Considerations

### Key Decisions

- **In-Memory Dictionary**: Chosen for simplicity and speed. It avoids the need to set up local DB installations or migrations, focusing purely on getting the API behavior working first.
- **Single-File Structure**: Fits minimal API style perfectly and keeps files focused.
- **RFC 7807 Compliant Errors**: Leverages built-in validation problem results so the API uses standard Web conventions out of the box.

### Known Risks

- **Memory Expansion**: Without database persistence, if many pets are registered, memory consumption will increase. Since this is an MVP/prototype task, the risk is mitigated.
- **Restarts**: Application restarts wipe the registered database. This is acceptable for current testing requirements.

## Architecture Decision Records

- [ADR-001: Product Approach for Pet Registration](adrs/adr-001.md) — Chose a single-step standard pet registration with a simple text field for tutor names.
- [ADR-002: Technical Stack and Design Decisions for Pet Registration API](adrs/adr-002.md) — Decisions on in-memory storage, single-file architecture, and WebApplicationFactory testing.
