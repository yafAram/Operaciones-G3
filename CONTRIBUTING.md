# Contributing

## Branches

- `main`: stable branch; changes enter through pull requests.
- `feat/<short-name>`: new functionality.
- `fix/<short-name>`: defect corrections.
- `security/<short-name>`: security remediation.
- `docs/<short-name>`: documentation-only changes.
- `test/<short-name>`: test coverage and test infrastructure.
- `chore/<short-name>`: maintenance and repository configuration.

Create each branch from the latest `main`. Keep a branch focused on one coherent change.

## Commits

Use concise imperative commit messages with a conventional prefix:

- `feat:` functionality
- `fix:` defect correction
- `security:` security remediation
- `test:` tests
- `docs:` documentation
- `refactor:` internal restructuring without behavior changes
- `chore:` maintenance

Do not include credentials, personal data, or generated build output in commits.

## Pull requests

1. Explain the problem and intended behavior.
2. List the files and areas affected.
3. Include tests executed and their actual results.
4. Document configuration or database migration requirements.
5. Describe security implications and rollback considerations when relevant.
6. Link related issues where applicable.
7. Keep the pull request focused and reviewable.

Do not mark checks as passed unless they actually ran successfully. Do not merge a pull request with failing required checks.

## Local validation

From the API project directory:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test
```

If no test project exists yet, state that fact rather than claiming that tests passed.

## Configuration

Use .NET User Secrets for local credentials. Never commit real secrets in appsettings files or environment files. See [Security](docs/SECURITY.md).
