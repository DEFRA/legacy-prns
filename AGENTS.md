# Agents Guidelines

## Coding conventions
- Do not use the `Async` suffix for asynchronous methods
- Add blank line before return statements
- Use constants for values that are used more than once; inline values that are only used once
- Use camelCase for constants declared within methods
- Use PascalCase for constants declared at type scope
- Lint files changed or created using `dotnet csharpier format .`
- Remove unused usings after each refactor
- Name expressions with `x => x.` syntax where possible
- Specify variables as `const` in tests where possible
- Use collection expressions where possible
- Use UTF-8 string literals for static strings that are immediately converted to bytes or base64-encoded
- Use object initializers where possible
- Put each class or record in its own file
- Do not use Arrange Act Assert comments in tests
- Use `_camelCase` for private instance fields
- Avoid adding private fields that only copy constructor or options values; read from the constructor parameter or options object directly unless a snapshot is needed
- Prefer AwesomeAssertions for assertions; where `Should().NotBeNull()` provides nullable flow information, do not add redundant null suppression operators or extra null guards
- Keep assertion style consistent within a test or helper
- Place new `appsettings.json` and related environment variant config sections at the bottom of existing settings
- Do not use Newtonsoft.Json in code; use System.Text.Json for JSON serialization and deserialization. If a direct Newtonsoft.Json package reference is required only to pin a secure transitive dependency version, comment that reason on the package reference.
- Prefer `System.Threading.Lock` for private static lock fields

## Change iterations
- Work backwards through tests to assess changes
- Keep the API skeleton free of runtime dependency registrations until a real service dependency is added
- When adding a runtime dependency, add its health check registration through `AddHealth` and tag dependency checks with `WebApplicationExtensions.Extended` so `/health/all` reflects what is wired in
- Check work has been successful by building the solution
- Run `Api.Tests` after any change
- Run `Api.IntegrationTests` after changes to startup, health, Docker, dependency wiring, or GitHub Actions
- Prefer framework options binding and validation over bespoke checks; bind each options section once, use `ValidateDataAnnotations()` with validation attributes such as `ValidateObjectMembers` for nested options, and call `ValidateOnStart()` for startup failures

## Service integration structure
- Place downstream service integration code under `src/Api/Services/<ServiceName>/`
- Keep each downstream integration self-contained in its service-specific folder, including wire DTOs, service options, HTTP clients, service interfaces, service registration extensions, and mappers
- Name service folders after the downstream dependency or bounded integration, for example `PrnCommonBackend`
- Use records for downstream DTOs and persistence-facing document records unless an external library requires otherwise
- Place Mongo persistence-facing entity records under `src/Api/Data/Entities`
- Keep pure mapping logic close to the downstream DTOs it maps from
- Keep PRN Common Backend raw-data DTOs in `src/Api/Services/PrnCommonBackend` and legacy PRN Mongo entity records in `src/Api/Data/Entities`
- Do not add runtime service registrations for DTOs, document records, or pure mappers
- Keep Mongo collection access behind `IDbContext` and focused persistence services/repositories; jobs should not request `IMongoDatabase` directly
- The legacy PRN migration reads PRN Common Backend raw data using the literal query value `sourceSystemId=null` so only null-source legacy PRNs are migrated
- The legacy PRN migration requests 50 PRNs per page, deletes all existing legacy PRNs before reading raw data, and inserts each migrated batch as fresh documents with Mongo `ObjectId` ids
- PRN Common Backend integration tests should stub OAuth token requests and downstream responses through the compose WireMock service

## Build guidance
- In the sandbox environment, avoid plain `dotnet build` because it can hang or take significantly longer due to workload notification or build-server delays
- Build with `DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=1 dotnet build legacy-prns.slnx --no-restore -m:1 -nodeReuse:false --disable-build-servers -v:minimal`
- If a build is unexpectedly slow, stop it, run `dotnet build-server shutdown`, and retry the sandbox build command above

## Test guidance
- Run `Api.Tests` with `DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=1 dotnet test tests/Api.Tests/Api.Tests.csproj --no-restore -m:1 -nodeReuse:false --disable-build-servers -v:minimal`
- In the sandbox environment, `Api.Tests` may need escalation because VSTest binds a local socket for test host communication

## Integration tests
- Keep integration tests focused on integration boundaries. For this skeleton, they smoke-test the containerized API health endpoint
- Run the local environment with `docker compose up --build --wait -d`
- Run `Api.IntegrationTests` with `DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=1 dotnet test tests/Api.IntegrationTests/Api.IntegrationTests.csproj --no-restore -m:1 -nodeReuse:false --disable-build-servers -v:minimal`
- Stop the local environment with `docker compose down -v --remove-orphans`
- In the sandbox environment, `Api.IntegrationTests` need escalation because VSTest binds a local socket and the tests access the Docker Compose service
