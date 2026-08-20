# Legacy PRNs

A skeleton ASP.NET API for the Legacy PRNs service.

The service exposes health endpoints and Hangfire background job support backed by MongoDB. Serilog, CDP trace header propagation, proxy-capable HTTP client helpers, and the custom trust store hook are retained so future dependencies can be added consistently.

## Prerequisites

- .NET 10
- Docker

## Running locally via Docker

```bash
docker compose up --build --wait -d
```

The API is available on http://localhost:8085.
The Hangfire dashboard is available on http://localhost:8085/hangfire.
Local Docker and `Development` runs use `developer` / `password` for the dashboard.

## Running via .NET

```bash
dotnet run --project ./src/Api --launch-profile Api
```

## Health endpoints

- `GET /health` is the lightweight readiness endpoint
- `GET /health/all` returns JSON for every registered health check

The extended health endpoint includes the MongoDB dependency used by Hangfire.

## Tests

Running tests without Docker dependencies:

```bash
dotnet test --filter "Category!=IntegrationTests"
```

Running integration tests against the Docker Compose service:

```bash
docker compose up --build --wait -d
dotnet test --filter "Category=IntegrationTests"
```

Running all tests after the compose service is up:

```bash
dotnet test
```

## Build pipeline

- [Pull requests](.github/workflows/check-pull-request.yml)
  - Run unit tests
  - Run integration tests against Docker Compose
  - Build Docker image
  - Check image with Trivy
  - Run SonarCloud scan
- [Publish](.github/workflows/publish.yml)
  - Merge PR to main
  - Run unit and integration tests
  - Build Docker image and publish to CDP
  - Run SonarCloud scan

## Dependency management

Dependabot is configured for NuGet and GitHub Actions updates.

See [.github/dependabot.yml](.github/dependabot.yml).

## Licence Information

THIS INFORMATION IS LICENSED UNDER THE CONDITIONS OF THE OPEN GOVERNMENT LICENCE found at:

http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3

### About the licence

The Open Government Licence (OGL) was developed by the Controller of His Majesty's Stationery Office (HMSO) to enable information providers in the public sector to license the use and re-use of their information under a common open licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.
