---
status: completed
title: Implement Pet Registration Endpoint and Models
type: backend
complexity: medium
dependencies: []
---

# Task 2: Implement Pet Registration Endpoint and Models

## Overview
This task implements the core business logic and API surface for registering pets. You will define the data structures, configure the thread-safe in-memory store, and write the validation and routing logic for the registration endpoint.

<critical>
- ALWAYS READ the PRD and TechSpec before starting
- REFERENCE TECHSPEC for implementation details — do not duplicate here
- FOCUS ON "WHAT" — describe what needs to be accomplished, not how
- MINIMIZE CODE — show code only to illustrate current structure or problem areas
- TESTS REQUIRED — every task MUST include tests in deliverables
</critical>

<requirements>
- Define C# positional record models for `Pet` and `RegisterPetRequest` in `Program.cs` (see TechSpec 'Core Interfaces').
- The registration endpoint MUST map to `POST /pets` and accept JSON payload.
- The system MUST store registrations in a thread-safe `ConcurrentDictionary<Guid, Pet>` registered as a singleton service.
- The endpoint MUST validate the following parameters:
  - `Name`, `Breed`, and `TutorName` must be non-empty and non-whitespace strings.
  - `BirthDate` must be a valid date in the past (less than or equal to the current UTC date).
- If validation fails, the endpoint MUST return HTTP 400 Bad Request formatted as an RFC 7807 validation problem.
- If validation succeeds, the endpoint MUST generate a new Guid `Id`, store the pet, and return HTTP 201 Created with a `Location` header pointing to `/pets/{id}` and the complete pet JSON in the body.
</requirements>

## Subtasks
- [x] 2.1 Define C# positional record types for `Pet` and `RegisterPetRequest` in `Program.cs`.
- [x] 2.2 Register a singleton `ConcurrentDictionary<Guid, Pet>` in the Dependency Injection container.
- [x] 2.3 Map the `POST /pets` endpoint in the application routing.
- [x] 2.4 Implement validation logic checking for empty strings and future birth dates.
- [x] 2.5 Generate unique Guid, persist the pet in memory, and return a 201 Created response.

## Implementation Details
Refer to the "Implementation Design", "Data Models", and "API Endpoints" sections of the TechSpec for schemas, status codes, and model structures.

### Relevant Files
- `fluxoEspecificacao/Program.cs` — The entry point of the application where services, routes, models, and endpoint handlers are defined.

### Dependent Files
- `fluxoEspecificacao.Tests/UnitTest1.cs` — The test class that will interact with this endpoint.

### Related ADRs
- [ADR-001: Product Approach for Pet Registration](../adrs/adr-001.md) — Agreement to capture tutor as a simple text field.
- [ADR-002: Technical Stack and Design Decisions](../adrs/adr-002.md) — Decision to use in-memory store and keep code consolidated in `Program.cs`.

## Deliverables
- `Program.cs` updated with model definitions and endpoint mapping.
- Working `POST /pets` endpoint handling validations, storage, and correct response codes.
- Local manual verification of endpoint responses (using a `.http` file or similar tool).

## Tests
- Unit tests:
  - Internal helper methods (if any are extracted) should be tested.
- Integration tests:
  - Deferred to Task 3 (integration tests will cover HTTP status codes, validation error details, and successful serialization).
- Test coverage target: >=80%
- All tests must pass.

## Success Criteria
- Valid HTTP requests to `POST /pets` succeed with 201 Created and contain the generated Guid.
- Invalid requests return 400 Bad Request with RFC 7807 compliant error details.
- Data successfully resides in memory after creation.
