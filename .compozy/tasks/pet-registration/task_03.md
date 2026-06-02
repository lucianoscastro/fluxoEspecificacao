---
status: completed
title: Implement Integration Tests
type: test
complexity: medium
dependencies:
  - task_01
  - task_02
---

# Task 3: Implement Integration Tests

## Overview
This task implements integration testing for the pet registration API. By leveraging `WebApplicationFactory`, you will build automated tests that send HTTP requests to the in-memory API and verify the endpoint's compliance with functional and technical requirements.

<critical>
- ALWAYS READ the PRD and TechSpec before starting
- REFERENCE TECHSPEC for implementation details — do not duplicate here
- FOCUS ON "WHAT" — describe what needs to be accomplished, not how
- MINIMIZE CODE — show code only to illustrate current structure or problem areas
- TESTS REQUIRED — every task MUST include tests in deliverables
</critical>

<requirements>
- Tests MUST use `WebApplicationFactory<Program>` to launch the API in memory.
- Implement tests verifying successful registration (HTTP 201 Created, location header present, non-empty Guid ID returned, and matching payload values).
- Implement tests verifying validation failures:
  - Missing or empty strings for `Name`, `Breed`, or `TutorName` must yield HTTP 400 Bad Request.
  - A future date of birth must yield HTTP 400 Bad Request.
- Error response tests MUST verify that the response body is formatted as RFC 7807 problem details containing appropriate keys.
</requirements>

## Subtasks
- [x] 3.1 Setup `WebApplicationFactory` fixture inside the test class.
- [x] 3.2 Implement a test verifying successful pet registration (happy path).
- [x] 3.3 Implement tests verifying HTTP 400 responses for empty fields.
- [x] 3.4 Implement tests verifying HTTP 400 responses for future birth dates.
- [x] 3.5 Verify the schema of validation error bodies aligns with RFC 7807 problem details.

## Implementation Details
Refer to the "Testing Approach" and "Integration Tests" sections of the TechSpec for test scenarios and error structures.

### Relevant Files
- `fluxoEspecificacao.Tests/UnitTest1.cs` — The test class to implement the integration tests (can be renamed to a more descriptive name if desired, or updated in place).

### Dependent Files
- `fluxoEspecificacao/Program.cs` — The application logic targeted by these integration tests.

### Related ADRs
- [ADR-002: Technical Stack and Design Decisions](../adrs/adr-002.md) — Decision to use WebApplicationFactory for HTTP-level testing and RFC 7807 validation responses.

## Deliverables
- Fully implemented integration test suite validating happy path and validation errors.
- Test execution output showing all tests passing.
- 80%+ test coverage of the endpoint code path.

## Tests
- Unit tests:
  - N/A (tested via integration tests).
- Integration tests:
  - [x] Happy path: Valid pet payload returns HTTP 201 Created and correct response JSON.
  - [x] Error path: Empty name, empty breed, or empty tutor returns HTTP 400 Bad Request with validation details.
  - [x] Error path: Future birth date returns HTTP 400 Bad Request with validation details.
- Test coverage target: >=80%
- All tests must pass.

## Success Criteria
- All integration tests execute and pass successfully.
- Code coverage of the registration logic in `Program.cs` is at least 80%.
