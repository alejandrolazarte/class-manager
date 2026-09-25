# Cloud environment (Claude Code on the web)

`scripts/cloud-setup.sh` prepares the ephemeral cloud container. It is not wired as a hook: it runs from the cloud environment's **Setup script** field, so it never runs on developer machines.

Setup script:

```bash
#!/bin/bash
bash /home/user/class-manager/scripts/cloud-setup.sh
```

## What it does

1. Enables the repository git hooks (`core.hooksPath .githooks`), so the pre-push hook that blocks pushes to `main` also protects cloud sessions.
2. Installs the .NET 10 SDK from the Ubuntu apt feed (`dotnet-sdk-10.0`). `dot.net/v1/dotnet-install.sh` is used only as a fallback because the cloud network policy blocks Microsoft's download hosts.
3. Restores NuGet packages for `class-manager.slnx`.
4. Ensures Node.js and pnpm exist and runs `pnpm install` in `app/` and `e2e/`. Chromium for Playwright is preinstalled in the cloud image (`PLAYWRIGHT_BROWSERS_PATH=/opt/pw-browsers`).
5. Starts the Docker daemon. Podman is not available in the cloud container; Testcontainers talks to Docker through `/var/run/docker.sock` instead.
6. Pre-pulls `mcr.microsoft.com/mssql/server:2022-latest`, the image used by `SqlServerContainerFixture`.

The script is idempotent. If a resumed session has no Docker daemon running (`docker info` fails), run `bash scripts/cloud-setup.sh` again: it only starts what is missing.

## Relation to local development

Locally, SQL Server runs on Podman; in the cloud container it runs on Docker. Everything else (API, Expo, tests) runs directly in both environments.
