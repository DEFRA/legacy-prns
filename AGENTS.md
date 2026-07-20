# Agents Guidelines

## Coding conventions
- Do not use the `Async` suffix for asynchronous methods
- Add blank line before return statements
- Use constants for values that are used more than once; inline values that are only used once
- Use camelCase for constants declared within methods
- Lint files changed or created using `dotnet csharpier format .`
- Name expressions with `x => x.` syntax where possible
- Specify variables as `const` in tests where possible
- Use collection expressions where possible
- Use object initializers where possible
- Do not use Arrange Act Assert comments in tests
- Use `_camelCase` for private instance fields
- Prefer AwesomeAssertions for assertions; where `Should().NotBeNull()` provides nullable flow information, do not add redundant null suppression operators or extra null guards
- Keep assertion style consistent within a test or helper
- Place new `appsettings.json` and related environment variant config sections at the bottom of existing settings

## Change iterations
- Work backwards through tests to assess changes
- Keep the API skeleton free of runtime dependency registrations until a real service dependency is added
- When adding a runtime dependency, add its health check registration through `AddHealth` and tag dependency checks with `WebApplicationExtensions.Extended` so `/health/all` reflects what is wired in
- Check work has been successful by building the solution
- Run `Api.Tests` after any change
- Run `Api.IntegrationTests` after changes to startup, health, Docker, dependency wiring, or GitHub Actions

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
