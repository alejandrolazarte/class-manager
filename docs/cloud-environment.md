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
4. Ensures Node.js exists, enables pnpm through corepack (so the version pinned by `packageManager` is used) and runs `pnpm install` in `app/` and `e2e/`.
5. Installs the Chromium revision that the e2e Playwright version expects (`pnpm exec playwright install chromium`, into `PLAYWRIGHT_BROWSERS_PATH=/opt/pw-browsers`). The Chromium preinstalled in the cloud image is older than the one Playwright pins. The download comes from `cdn.playwright.dev`, which must be in the environment's allowed domains (Network access).
Steps 2–3 (backend) and 4–5 (frontend) run in parallel; the apt calls wait for the dpkg lock instead of failing. The script does not start Docker nor pull the SQL Server image (about 2.3 GB): that was the slowest part of the setup, and the session hook below does it in the background.

The script is idempotent.

## Every session

Processes started by the Setup script do not survive into the session. `scripts/cloud-session-start.sh` runs as a `SessionStart` hook (`.claude/settings.json`) and does nothing outside the cloud (`CLAUDE_CODE_REMOTE` is not `true`). In the cloud it:

1. Exports `E2E_DATABASE_CONNECTION_STRING` pointing to the `ClassManagerE2E` database on the `class-manager-sql` container.
2. In the background, so the session starts without waiting: starts the Docker daemon if `docker info` fails (Podman is not available in the cloud container; Testcontainers talks to Docker through `/var/run/docker.sock`), then creates or starts the `class-manager-sql` SQL Server container on port 1433 with a throwaway SA password, pulling the image the first time. Output goes to `/tmp/cloud-session-start.log`.

Before the e2e suite, wait until SQL Server is ready with `bash scripts/cloud-session-start.sh --wait`, then run it as described in [running-e2e-tests.md](e2e/running-e2e-tests.md): `cd e2e; pnpm db:migrate; pnpm test`. Integration tests create their own containers through Testcontainers and only need the Docker daemon, which `--wait` also ensures.

## Relation to local development

Locally, SQL Server runs on Podman; in the cloud container it runs on Docker. Everything else (API, Expo, tests) runs directly in both environments.
